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
import { DownloadPdfsModel } from '../../models/download-pdfs';

const encryptFilesCountPerJob = 20;

@Component({
  selector: 'app-download',
  templateUrl: './download.component.html',
  styleUrls: ['./download.component.scss'],
  standalone: false,
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

    const jobs: DownloadPdfsModel[][] = [];

    for (let i = 0; i < files.length; i += encryptFilesCountPerJob) {
      jobs.push(files.slice(i, i + encryptFilesCountPerJob));
    }

    files.forEach(file => (file.status = 'running'));

    try {
      for (const jobFiles of jobs) {
        const filePaths = jobFiles.map(file => file.filePath);

        const response = await this.encryptFile(
          filePaths,
          this.encryptionCertificatePaths,
          this.selectedCertificatePath,
          this.selectedCertificatePassword,
        );

        if (response) {
          jobFiles.forEach(file => (file.status = 'encrypted'));
        } else {
          jobFiles.forEach(file => (file.status = 'unencrypted'));
          // Set all following files (which were not in the job) to unencrypted
          files.forEach(file => (file.status = file.status === 'running' ? 'unencrypted' : file.status));
        }

        await this.appStateService.update(s => (s.downloadPdfs = this.checkablePdfs.map(c => c.file)));
      }
    } finally {
      this.cryptFilesIsRunning = false;
    }
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

    await this.electronService.openPath(path);
  }

  public async downloadSingleFile(checkablePdf: FileCheckable): Promise<void> {
    try {
      const path = await this.electronService.showFolderPickerDialog();
      if (!path) {
        return; // User cancelled the dialog
      }

      // copy file to selected path
      const targetPath = pathCombine(path, checkablePdf.file.fileName);
      await this.electronService.copyFile(checkablePdf.file.filePath, targetPath);

      // Show the file in the system file explorer
      try {
        await this.electronService.showItemInFolder(targetPath);
      } catch (showError) {
        console.warn('Could not show item in folder:', showError);
      }
    } catch (error) {
      console.error('Error downloading file:', error);
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
