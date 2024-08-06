/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, Inject, OnInit } from '@angular/core';
import { CRYPTIC_SERVICE, CrypticService } from '../../services/cryptic.service';
import { AppStateService } from '../../services/app-state.service';
import { AppStateStep } from '../../models/app-state.model';
import { FileCheckable } from './file-checkable.model';
import { JobContext } from '../../services/jobs/job-context';
import { ElectronService } from '../../services/electron.service';
import { firstValueFrom } from 'rxjs';
import { E_VOTING_CONFIG_DIR } from '../../common/path.constants';
import { pathCombine } from '../../services/utils/path.utils';

@Component({
  selector: 'app-download',
  templateUrl: './download.component.html',
  styleUrls: ['./download.component.scss'],
})
export class DownloadComponent implements OnInit {
  public outputPdfs = [];
  public isBackQuestionVisible = false;
  public displaySigningCertificateSelection = false;
  public displayEncryptionCertificatesPreview = false;

  public selectedCertificatePath: string;
  public selectedCertificatePassword: string;
  public selectedCertificateSubject: string;

  public checkablePdfs: FileCheckable[];
  public allChecked = false;
  public cryptFilesIsRunning = false;

  public readonly encryptionCertificatePaths: string[];

  constructor(
    @Inject(CRYPTIC_SERVICE) private readonly crypticService: CrypticService,
    private readonly appStateService: AppStateService,
    jobContext: JobContext,
    private readonly electronService: ElectronService,
  ) {
    this.encryptionCertificatePaths = jobContext.ech228?.votingCardDelivery.extension.certificates ?? [];
    this.checkablePdfs = appStateService.state.downloadPdfs.map(f => ({
      file: f,
      checked: f.status === 'encrypted',
    }));
  }

  public async ngOnInit(): Promise<void> {
    await this.appStateService.updateStep(AppStateStep.Download);
    this.updateAllChecked();
  }

  public togglePdfChecked(checkablePdf: FileCheckable): void {
    checkablePdf.checked = !checkablePdf.checked;
    this.updateAllChecked();
  }

  public toggleAllChecked(): void {
    this.allChecked = !this.allChecked;

    for (const checkablePdf of this.checkablePdfs.filter(c => c.file.status === 'unencrypted')) {
      checkablePdf.checked = this.allChecked;
    }
  }

  public confirmEncryptionCertificates(): void {
    this.displayEncryptionCertificatesPreview = false;
    this.displaySigningCertificateSelection = true;
  }

  public async encryptFiles(): Promise<void> {
    this.displaySigningCertificateSelection = false;
    this.cryptFilesIsRunning = true;
    const files = this.checkablePdfs.filter(c => c.checked && c.file.status === 'unencrypted').map(c => c.file);

    if (files.length === 0) {
      return;
    }

    const filePaths = files.map(file => file.filePath);
    files.forEach(file => (file.status = 'running'));

    const response = await this.encryptFile(
      filePaths,
      this.encryptionCertificatePaths,
      this.selectedCertificatePath,
      this.selectedCertificatePassword,
    );

    if (response) {
      files.forEach(file => (file.status = 'encrypted'));
    } else {
      files.forEach(file => (file.status = 'unencrypted'));
      throw new Error('error while encrypt files');
    }

    await this.appStateService.update(s => (s.downloadPdfs = this.checkablePdfs.map(c => c.file)));
    this.cryptFilesIsRunning = false;
  }

  public async openDirectoryPickerDialog(): Promise<void> {
    const path = await this.electronService.showFolderPickerDialog();
    await this.downloadAll(path);
  }

  public async downloadAll(path: string): Promise<void> {
    if (!path) {
      return;
    }

    for (const checkablePdf of this.checkablePdfs) {
      await this.electronService.copyFile(checkablePdf.file.filePath, pathCombine(path, checkablePdf.file.fileName));
    }
  }

  private encryptFile(
    filePaths: string[],
    receiverCertificatePaths: string[],
    senderCertificatePath: string,
    senderCertificatePassword: string,
  ): Promise<boolean> {
    return firstValueFrom(
      this.crypticService.encryptFiles(
        filePaths,
        receiverCertificatePaths.map(r => pathCombine(E_VOTING_CONFIG_DIR, r)),
        senderCertificatePath,
        senderCertificatePassword,
      ),
    );
  }

  private updateAllChecked(): void {
    this.allChecked = this.checkablePdfs.length > 0 && this.checkablePdfs.every(c => c.checked);
  }
}
