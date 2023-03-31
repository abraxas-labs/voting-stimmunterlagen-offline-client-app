import { InjectionToken } from '@angular/core';
import { Observable } from 'rxjs';
import { Ech228Model } from '../models/ech228.model';

export const CH_VOTE_DATA_SERVICE = new InjectionToken<ChVoteDataService<any>>('ChVoteDataService');

export interface ChVoteDataService<T> {
  importDataFromPaths(filePaths: string[]): Observable<Ech228Model | T | undefined>;
}
