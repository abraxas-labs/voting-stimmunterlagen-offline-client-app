import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { Ech228Model } from '../models/ech228.model';

export const ECH_DELIVERY_SERVICE = new InjectionToken<EchDeliveryService<any>>('EchDeliveryService');

export interface EchDeliveryService<T> {
  importDataFromPaths(filePaths: string[]): Observable<Ech228Model | T | undefined>;
}
