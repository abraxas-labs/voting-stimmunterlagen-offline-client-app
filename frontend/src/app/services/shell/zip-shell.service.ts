import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CommandParameter } from '../../models/shell/command-parameter';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { ZipService } from '../zip.service';
import { ElectronService } from '../electron.service';

@Injectable()
export class ZipShellService implements ZipService {
  constructor(private readonly electronService: ElectronService) {}

  public unzip(zip, outputPath): Observable<boolean> {
    const parameters = [new CommandParameter('-i', zip), new CommandParameter('-o', outputPath)];
    return this.electronService.shellExecute(environment.commands.zipTool, parameters).pipe(
      tap(result => {
        if (result.exitCode !== 0) {
          console.log(result.data.join('\n'));
        }
      }),
      map(result => {
        return result.exitCode === 0;
      }),
      catchError(error => {
        console.error(error);
        return of(false);
      }),
    );
  }
}
