/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

/**
 * Uses Node API to start a shell.
 */
import { PdfCreatorService } from '../pdf-creator.service';
import { Observable, of } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { catchError, map } from 'rxjs/operators';
import { CommandParameter } from '../../models/shell/command-parameter';
import { LogService } from '../log.service';
import { ElectronService } from '../electron.service';

@Injectable()
export class PdfCreatorShellService implements PdfCreatorService {
  constructor(private readonly electronService: ElectronService, private readonly logService: LogService) {}

  public generate(layoutPath: string, model: any): Observable<Uint8Array | undefined> {
    const params = [
      new CommandParameter('--template', layoutPath),
      new CommandParameter('--instream'),
      new CommandParameter('--outstream'),
      new CommandParameter('-l', this.logService.generateLogFilePath('pdf-generate')),
    ];

    return this.electronService.shellExecute(environment.commands.votingCardGenerator, params, model).pipe(
      map(result => result.data),
      catchError(error => {
        console.error(error);
        return of(undefined);
      }),
    );
  }

  public generateAndSave(layoutPath: string, model: any, filename: string): Observable<boolean> {
    const params = [
      new CommandParameter('--template', layoutPath),
      new CommandParameter('--instream'),
      new CommandParameter('--out', filename),
      new CommandParameter('-l', this.logService.generateLogFilePath('pdf-generate-and-save')),
    ];
    return this.electronService.shellExecute(environment.commands.votingCardGenerator, params, model).pipe(
      map(result => {
        return result.exitCode === 0;
      }),
    );
  }
}
