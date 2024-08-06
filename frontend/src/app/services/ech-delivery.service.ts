/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { EchDeliveryGeneratorResult } from '../models/ech-delivery-generator-result.model';

export const ECH_DELIVERY_SERVICE = new InjectionToken<EchDeliveryService<any>>('EchDeliveryService');

export interface EchDeliveryService<T> {
  importDataFromPaths(filePaths: string[], postSignatureValidationPaths: string[]): Observable<EchDeliveryGeneratorResult | T | undefined>;
}
