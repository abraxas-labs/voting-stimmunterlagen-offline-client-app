/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, OnDestroy, OnInit } from '@angular/core';
import { Job } from '../../models/generation/job';
import { VotingCardService } from '../../services/voting-card.service';
import { JobContext } from '../../services/jobs/job-context';
import { AppStateService } from '../../services/app-state.service';
import { AppStateStep } from '../../models/app-state.model';

@Component({
  selector: 'app-job-overview-page',
  templateUrl: './job-overview-page.component.html',
  styleUrls: ['./job-overview-page.component.scss'],
})
export class JobOverviewPageComponent implements OnInit, OnDestroy {
  public isLoading = true;
  public jobGroups: Job[][];
  private subscribes: any;
  public groupTitleValue = {};

  constructor(
    public readonly context: JobContext,
    private readonly votingCardService: VotingCardService,
    private readonly appStateService: AppStateService,
  ) {}

  public async ngOnInit(): Promise<void> {
    await this.appStateService.updateStep(AppStateStep.Generation);
    this.subscribes = this.votingCardService.generateVotingCards(this.context).subscribe({
      next: jobList => (this.jobGroups = jobList),
      error: error => console.error('pdfRendering', error),
      complete: () => {
        console.log('finish', this.jobGroups);
        this.isLoading = false;
      },
    });
  }

  public generateJobTitle(group: Job[]): any {
    const groupValue: any = {};
    groupValue.title = this.getName(group);
    groupValue.start = this.getTime(group[0].start);
    groupValue.end = this.getTime(group[group.length - 1].end);
    groupValue.completed = !group.find((job: Job) => !job.completed);
    return groupValue;
  }

  public getName(group: Job[]): string {
    return this.votingCardService.createFileName(group, this.context);
  }

  public expandSwitch(event): void {
    if (event.currentTarget.classList.contains('expand')) {
      event.currentTarget.classList.remove('expand');
      return;
    }
    event.currentTarget.classList.add('expand');
  }

  public ngOnDestroy(): void {
    this.subscribes.unsubscribe();
  }

  public getTime(datum: Date): string | undefined {
    if (!datum) {
      return;
    }
    const houers = '0' + datum.getHours();
    const minutes = '0' + datum.getMinutes();
    const seconds = '0' + datum.getSeconds();
    return houers.slice(-2) + ':' + minutes.slice(-2) + ':' + seconds.slice(-2);
  }
}
