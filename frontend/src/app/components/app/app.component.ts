/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { LOG_DIR } from '../../common/path.constants';
import { AppStateService } from '../../services/app-state.service';
import { ElectronService } from '../../services/electron.service';
import { TranslationService } from '../../services/translation/translation.service';
import { pdfDefaultOptions } from 'ngx-extended-pdf-viewer';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  public showContent = false;
  public isDeleting = false;

  constructor(
    translationService: TranslationService,
    private readonly cd: ChangeDetectorRef,
    private readonly electronService: ElectronService,
    private readonly appStateService: AppStateService,
  ) {
    // Prevent execution of javascript in the Pdf Viewer.
    pdfDefaultOptions.enableScripting = false;

    translationService.languageChange$.subscribe(() => this.reloadContent());
  }

  public async ngOnInit(): Promise<void> {
    await this.electronService.createDirectory(LOG_DIR);
    await this.appStateService.init();
    this.showContent = true;

    if (!this.electronService.isProd()) {
      console.log('Electron main process runs in dev mode');
    }
  }

  private reloadContent(): void {
    this.showContent = false;
    this.cd.detectChanges();
    this.showContent = true;
    this.cd.detectChanges();
  }
}
