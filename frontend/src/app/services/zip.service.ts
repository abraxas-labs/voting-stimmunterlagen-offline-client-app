/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';

export const ZIP_SERVICE = new InjectionToken<ZipService>('ZipService');

export interface ZipService {
  unzip(zip, outputPath): Observable<boolean>;
}
