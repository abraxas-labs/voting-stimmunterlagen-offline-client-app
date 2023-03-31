import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

export const SECURE_DELETE_SERVICE = new InjectionToken<SecureDeleteService>('SecureDeleteService');

export interface SecureDeleteService {
  deleteDirectory(directoryName: string, recursive: boolean): Observable<boolean>;
}
