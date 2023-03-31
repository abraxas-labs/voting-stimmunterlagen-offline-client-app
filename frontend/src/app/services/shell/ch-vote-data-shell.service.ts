import { Observable } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CommandParameter } from '../../models/shell/command-parameter';
import { map } from 'rxjs/operators';
import { ChVoteDataService } from '../ch-vote-data.service';
import { Ech228Model } from '../../models/ech228.model';
import { LogService } from '../log.service';
import { ElectronService } from '../electron.service';

@Injectable()
export class ChVoteDataShellService<T> implements ChVoteDataService<T> {
  public constructor(private readonly electronService: ElectronService, private readonly logService: LogService) {}

  public importDataFromPaths(filePaths: string[]): Observable<Ech228Model | T | undefined | any> {
    const parameters = [
      new CommandParameter('--outstream', ''),
      new CommandParameter('--logfile', this.logService.generateLogFilePath('import-data-from-paths')),
    ];

    filePaths.forEach(filePath => {
      parameters.push(new CommandParameter('--in', filePath));
    });

    return this.electronService.shellExecuteDecoded(environment.commands.chVote, parameters).pipe(
      map(result => {
        const response = result.data.length > 0 ? (JSON.parse(result.data) as T) : undefined;

        if (response === undefined || response['ErrorCode']) {
          console.error('Error on importDataFromPaths: ', response === undefined ? 'Response data is empty' : response);
          throw response;
        }
        return response;
      }),
    );
  }
}
