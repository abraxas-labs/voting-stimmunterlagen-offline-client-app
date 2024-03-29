import { Component } from '@angular/core';
import { SettingsService } from '../../services/settings.service';
import { AppStateService } from '../../services/app-state.service';
import { AppStateStep } from '../../models/app-state.model';
import { ElectronService } from '../../services/electron.service';

@Component({
  selector: 'app-settings-page',
  templateUrl: './settings-page.component.html',
  styleUrls: ['./settings-page.component.scss'],
})
export class SettingsPageComponent {
  public appVersion;
  public deleteMsg = '';
  public disableSettingJobSize = false;

  public numberOfPreview: number;
  public jobSize: number;

  constructor(
    public readonly settingsService: SettingsService,
    private readonly appStateService: AppStateService,
    electronService: ElectronService,
  ) {
    this.appVersion = electronService.getAppVersion();

    this.disableSettingJobSize = appStateService.state.step >= AppStateStep.Generation;
    this.numberOfPreview = this.settingsService.numberOfPreview;
    this.jobSize = this.settingsService.jobSize;
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
