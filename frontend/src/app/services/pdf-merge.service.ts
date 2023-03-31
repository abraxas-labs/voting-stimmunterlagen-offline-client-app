import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

export const PDFMERGE_SERVICE = new InjectionToken<PdfMergeService>('PdfMergeService');

export interface PdfMergeService {
  mergeFromFolder(sourcePath, outputPath): Observable<boolean>;
}
