/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Injectable } from '@angular/core';
import { AppState, AppStateStep } from '../models/app-state.model';
import { Job } from '../models/generation/job';
import { JobMetadata } from '../models/generation/job-metadata';
import { Router } from '@angular/router';
import { ElectronService } from './electron.service';
import { APP_STATE_FILE } from '../common/path.constants';

@Injectable()
export class AppStateService {
  private stateValue: AppState;

  constructor(
    private readonly router: Router,
    private readonly electronService: ElectronService,
  ) {}

  public get state(): AppState {
    if (!this.stateValue) {
      throw new Error('app state is not initialized');
    }

    return this.parseAppStateJsonString(JSON.stringify(this.stateValue));
  }

  public updateStep(step: AppStateStep): Promise<void> {
    return this.update(x => (x.step = step));
  }

  public updateJobGroup(jobGroup: Job[], groupIndex: number): Promise<void> {
    const mappedJobGroup = jobGroup.map(job => this.copyJob(job) as Job);
    return this.update(s => (s.jobGroups[groupIndex] = mappedJobGroup));
  }

  public update(modifier: (state: AppState) => void): Promise<void> {
    modifier(this.stateValue);
    return this.syncFs();
  }

  public reset(): void {
    this.router.navigate(['/welcome']);
  }

  public create(): Promise<void> {
    this.stateValue = {
      step: AppStateStep.Welcome,
      uploads: [],
      keystorePaths: [],
      grouping: [],
      sorting: [],
      jobGroups: [],
      downloadPdfs: [],
    };
    return this.syncFs();
  }

  public init(): Promise<void> {
    return this.loadState();
  }

  private async loadState(): Promise<void> {
    let needCreate = false;

    try {
      const stateJsonString = await this.electronService.readFile(APP_STATE_FILE);

      if (!stateJsonString) {
        needCreate = true;
      } else {
        this.stateValue = this.parseAppStateJsonString(stateJsonString);
      }
    } catch (err) {
      console.log('cannot load state: ', err);
      needCreate = true;
    }

    if (needCreate) {
      await this.create();
    }
  }

  private async syncFs(): Promise<void> {
    await this.electronService.createOrUpdateFile(APP_STATE_FILE, JSON.stringify(this.state));
  }

  private parseAppStateJsonString(json: string): AppState {
    const appState = JSON.parse(json) as AppState;

    if (!appState.jobGroups) {
      return appState;
    }

    for (const jobGroup of appState.jobGroups) {
      for (const job of jobGroup) {
        if (!job.completed) {
          return appState;
        }

        job.start = new Date(job.start);
        job.end = new Date(job.end);
      }
    }
    return appState;
  }

  private copyJob(job: Job): JobMetadata {
    return {
      completed: job.completed,
      end: job.end,
      failed: job.failed,
      outputFilename: job.outputFilename,
      start: job.start,
    };
  }
}
