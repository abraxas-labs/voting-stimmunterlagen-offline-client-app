import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { Certificate } from '../models/certificate.model';

export const CRYPTIC_SERVICE = new InjectionToken<CrypticService>('CrypticService');

export interface CrypticService {
  getSenderCertificates(certificatePath: string, certificatePassword: string): Observable<Certificate[] | undefined>;
  getReceiverCertificates(certificatesPaths: string[]): Observable<Certificate[] | undefined>;
  encryptFiles(
    filePaths: string[],
    receiverCertificatePaths: string[],
    senderCertificatePath: string,
    senderCertificatePassword: string,
  ): Observable<boolean>;
}
