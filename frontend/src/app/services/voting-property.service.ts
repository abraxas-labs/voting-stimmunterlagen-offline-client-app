/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Inject, Injectable } from '@angular/core';
import { ElectronService } from './electron.service';
import { from, Observable, throwError } from 'rxjs';
import { PropertyData } from '../models/porperty-data';
import { ZIP_SERVICE, ZipService } from './zip.service';
import { map, switchMap } from 'rxjs/operators';
import { SECURE_DELETE_SERVICE, SecureDeleteService } from './secure-delete.service';
import { E_VOTING_CONFIG_DIR, JSON_CONFIG_FILE } from '../common/path.constants';

@Injectable()
export class VotingPropertyService {
  private property: PropertyData | undefined;

  constructor(
    private readonly electronService: ElectronService,
    @Inject(ZIP_SERVICE) private readonly zipService: ZipService,
    @Inject(SECURE_DELETE_SERVICE) private readonly secureDeleteService: SecureDeleteService,
  ) {}

  public prepareProperty(zip: File): Observable<PropertyData | undefined> {
    this.property = undefined;
    return this.clearProperty().pipe(
      switchMap(() => from(this.electronService.createDirectory(E_VOTING_CONFIG_DIR))),
      switchMap(() => {
        return this.unzipFolder(zip).pipe(
          map(success => {
            if (!success) {
              throwError(() => 'error on unzip');
            }
          }),
        );
      }),
      switchMap(() => this.readProperty()),
    );
  }

  public getProperty(): PropertyData | undefined {
    return this.property;
  }

  private clearProperty(): Observable<boolean> {
    return this.secureDeleteService.deleteDirectory(E_VOTING_CONFIG_DIR, true);
  }

  private unzipFolder(zip): Observable<boolean> {
    return this.zipService.unzip(zip.path, E_VOTING_CONFIG_DIR);
  }

  private readProperty(): Observable<PropertyData> {
    return from(this.electronService.readFile(JSON_CONFIG_FILE)).pipe(map(r => JSON.parse(r)));
  }
}
