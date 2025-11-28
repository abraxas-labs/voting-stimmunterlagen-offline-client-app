/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component } from '@angular/core';
import { SettingsService } from '../../services/settings.service';
import { AppStateService } from '../../services/app-state.service';
import { AppStateStep } from '../../models/app-state.model';
import { ElectronService } from '../../services/electron.service';

@Component({
  selector: 'app-settings-page',
  templateUrl: './settings-page.component.html',
  styleUrls: ['./settings-page.component.scss'],
  standalone: false,
})
export class SettingsPageComponent {
  public readonly allowedJavaRuntimePathExtensions = '.exe';
  public readonly allowedPostSignatureValidatorPathExtensions = '.jar';

  public appVersion;
  public deleteMsg = '';
  public disableSettingJobSize = false;

  public numberOfPreview: number;
  public jobSize: number;
  public javaRuntimePath: string;
  public postSignatureValidatorPath: string;

  constructor(
    public readonly settingsService: SettingsService,
    private readonly appStateService: AppStateService,
    electronService: ElectronService,
  ) {
    this.appVersion = electronService.getAppVersion();

    this.disableSettingJobSize = appStateService.state.step >= AppStateStep.Generation;
    this.numberOfPreview = this.settingsService.numberOfPreview;
    this.jobSize = this.settingsService.jobSize;
    this.javaRuntimePath = this.settingsService.javaRuntimePath;
    this.postSignatureValidatorPath = this.settingsService.postSignatureValidatorPath;
  }

  public changeJavaRuntimePath(path: string): void {
    this.settingsService.javaRuntimePath = path;
    this.javaRuntimePath = path;
  }

  public changePostSignatureValidatorPath(path: string): void {
    this.settingsService.postSignatureValidatorPath = path;
    this.postSignatureValidatorPath = path;
  }

  public sDelete(): void {
    this.appStateService.reset();
  }

  public save(): void {
    this.settingsService.numberOfPreview = this.numberOfPreview;
    this.settingsService.jobSize = this.jobSize;
    window.history.back();
  }
}
