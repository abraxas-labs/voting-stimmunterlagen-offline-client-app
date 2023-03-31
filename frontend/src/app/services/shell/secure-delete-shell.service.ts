import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { CommandParameter } from '../../models/shell/command-parameter';
import { SecureDeleteService } from '../secure-delete.service';
import { ElectronService } from '../electron.service';

@Injectable()
export class SecureDeleteShellService implements SecureDeleteService {
  private readonly amountOfPasses = 10;

  constructor(private readonly electronService: ElectronService) {}

  public deleteDirectory(directoryName: string, recursive: boolean = false): Observable<boolean> {
    return this.delete(directoryName, recursive);
  }

  private delete(target: string, recursive: boolean): Observable<boolean> {
    const params = this.buildArguments(target, recursive);

    return this.electronService.shellExecuteDecoded(environment.commands.secureDelete, params).pipe(
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

  private buildArguments(target: string, recursive: boolean): CommandParameter[] {
    const args = [new CommandParameter('-p', this.amountOfPasses), new CommandParameter('-r'), new CommandParameter('-accepteula')];

    if (recursive) {
      args.push(new CommandParameter('-s'));
    }

    args.push(new CommandParameter('', target));
    return args;
  }
}
