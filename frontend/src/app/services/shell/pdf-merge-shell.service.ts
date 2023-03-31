import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CommandParameter } from '../../models/shell/command-parameter';
import { Observable, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { PdfMergeService } from '../pdf-merge.service';
import { ElectronService } from '../electron.service';

@Injectable()
export class PdfMergeShellService implements PdfMergeService {
  constructor(private readonly electronService: ElectronService) {}

  public mergeFromFolder(sourcePath, outputPath): Observable<boolean> {
    const parameters = [new CommandParameter('-i', sourcePath), new CommandParameter('-o', outputPath)];
    return this.electronService.shellExecute(environment.commands.pdfMerge, parameters).pipe(
      map(result => result.exitCode === 0),
      catchError(error => {
        console.error(error);
        return of(false);
      }),
    );
  }
}
