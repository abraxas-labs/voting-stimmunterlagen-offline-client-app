import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { Ech0228Model } from '../models/ech0228/ech0228.model';

export const ECH_DELIVERY_SERVICE = new InjectionToken<EchDeliveryService<any>>('EchDeliveryService');

export interface EchDeliveryService<T> {
  importDataFromPaths(filePaths: string[]): Observable<Ech0228Model | T | undefined>;
}
