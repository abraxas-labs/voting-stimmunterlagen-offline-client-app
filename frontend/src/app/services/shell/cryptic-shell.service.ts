/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CommandParameter } from '../../models/shell/command-parameter';
import { Observable, of } from 'rxjs';
import { CrypticService } from '../cryptic.service';
import { catchError, map, tap } from 'rxjs/operators';
import { LogService } from '../log.service';
import { ElectronService } from '../electron.service';
import { Certificate } from '../../models/certificate.model';

@Injectable()
export class CrypticShellService implements CrypticService {
  constructor(
    private readonly electronService: ElectronService,
    private readonly logService: LogService,
  ) {}

  public getSenderCertificates(certificatePath: string, certificatePassword: string): Observable<Certificate[]> {
    const parameters = [
      new CommandParameter('-sjl'),
      new CommandParameter('-sf', certificatePath),
      new CommandParameter('-sfp', certificatePassword),
    ];
    return this.electronService.shellExecuteDecoded(environment.commands.decrypt, parameters).pipe(
      map(result => {
        console.log('senderCertificateSubjects', result.data);
        return JSON.parse(result.data);
      }),
    );
  }

  public getReceiverCertificates(certificatePaths: string[]): Observable<Certificate[]> {
    const parameters = [new CommandParameter('-rjl')];

    certificatePaths.forEach(r => parameters.push(new CommandParameter('-rf', r)));

    return this.electronService.shellExecuteDecoded(environment.commands.decrypt, parameters).pipe(
      map(result => {
        console.log('receiverCertificateSubjects', result.data);
        return JSON.parse(result.data);
      }),
    );
  }

  public encryptFiles(
    filePaths: string[],
    receiverCertificatePaths: string[],
    senderCertificatePath: string,
    senderCertificatePassword: string,
  ): Observable<any> {
    const parameters = [
      new CommandParameter('-sf', senderCertificatePath),
      new CommandParameter('-sfp', senderCertificatePassword),
      new CommandParameter('--logfile', this.logService.generateLogFilePath('encrypt')),
    ];

    receiverCertificatePaths.forEach(r => parameters.push(new CommandParameter('-rf', r)));

    filePaths.forEach((file: string) => {
      parameters.push(new CommandParameter('-i', file));
      parameters.push(new CommandParameter('-o', file));
    });
    return this.electronService.shellExecute(environment.commands.decrypt, parameters).pipe(
      tap(result => {
        if (result.exitCode !== 0) {
          console.log(result.data);
        }
      }),
      map(result => result.exitCode === 0),
      catchError(error => {
        console.error(error);
        return of(false);
      }),
    );
  }
}
