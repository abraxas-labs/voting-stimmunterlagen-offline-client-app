import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

export const PDF_CREATOR_SERVCE = new InjectionToken<PdfCreatorService>('PdfCreatorService');

export interface PdfCreatorService {
  generate(layoutPath: string, model: any): Observable<Uint8Array | undefined>;

  generateAndSave(layoutPath: string, model: any, filename: string): Observable<boolean>;
}
