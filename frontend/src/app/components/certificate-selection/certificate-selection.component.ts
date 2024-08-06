/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, EventEmitter, Inject, Input, Output } from '@angular/core';
import { finalize } from 'rxjs';
import { Certificate } from '../../models/certificate.model';
import { CRYPTIC_SERVICE, CrypticService } from '../../services/cryptic.service';

@Component({
  selector: 'app-certificate-selection',
  templateUrl: './certificate-selection.component.html',
  styleUrls: ['./certificate-selection.component.scss'],
})
export class CertificateSelectionComponent {
  @Input() certificatePath: string;
  @Input() certificatePassword: string;

  @Output() certificatePathChange = new EventEmitter<string>();
  @Output() certificatePasswordChange = new EventEmitter<string>();
  @Output() certificateSubjectChange = new EventEmitter<string>();

  public readonly allowedCertificateFileExtensions = '.p12';
  public showCertificateSubject = false;
  public certificates: Certificate[] = [];
  public certificateSubject: string;

  constructor(@Inject(CRYPTIC_SERVICE) private readonly crypticService: CrypticService) {}

  public changeCertificatePassword(password: string): void {
    this.changeCertificateSubject('');
    this.certificatePassword = password;
    this.certificatePasswordChange.emit(password);
  }

  public changeCertificatePath(file: File): void {
    this.changeCertificateSubject('');
    this.certificatePath = file.path;
    this.certificatePathChange.emit(file.path);
  }

  public changeCertificateSubject(subject: string): void {
    this.certificateSubject = subject;
    this.certificateSubjectChange.emit(subject);

    if (!subject) {
      this.showCertificateSubject = false;
    }
  }

  public loadCertificateSubject(): void {
    if (!this.certificatePath || !this.certificatePassword) {
      return;
    }

    this.crypticService
      .getSenderCertificates(this.certificatePath, this.certificatePassword)
      .pipe(finalize(() => (this.showCertificateSubject = true)))
      .subscribe(list => {
        if (!list || list.length === 0) {
          return;
        }

        this.certificates = list;
        this.changeCertificateSubject(this.certificates[0].subject);
      });
  }
}
