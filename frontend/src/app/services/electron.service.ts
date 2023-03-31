import { Injectable } from '@angular/core';
import { from, map, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ShellExecuteBinaryResult, ShellExecuteDecodedResult } from '../models/electron.model';
import { CommandInfo } from '../models/shell/command-info';
import { CommandParameter } from '../models/shell/command-parameter';

@Injectable({
  providedIn: 'root',
})
export class ElectronService {
  private backend: any;

  constructor() {
    // backend is assigned by the electron preload script
    this.backend = (window as any).backend;
  }

  public isElectronEnvironment(): boolean {
    return window.navigator.userAgent.indexOf('Electron/') !== -1;
  }

  public shellExecute(commandInfo: CommandInfo, commandParameters: CommandParameter[], input?: any): Observable<ShellExecuteBinaryResult> {
    const result = this.backend.shellExecute(environment.toolsDirectory, commandInfo, commandParameters, input);
    return from(result).pipe(
      map((r: any) => {
        return {
          exitCode: r.exitCode,
          data: this.mergeUInt8Arrays(r.data),
        };
      }),
    );
  }

  public shellExecuteDecoded(
    commandInfo: CommandInfo,
    commandParameters: CommandParameter[],
    input?: any,
  ): Observable<ShellExecuteDecodedResult> {
    return this.shellExecute(commandInfo, commandParameters, input).pipe(
      map(r => ({
        exitCode: r.exitCode,
        data: new TextDecoder().decode(r.data),
      })),
    );
  }

  public readFile(path: string | undefined): Promise<string> {
    return this.backend.readFile(path);
  }

  public copyFile(sourcePath: string, targetPath: string): Promise<void> {
    return this.backend.copyFile(sourcePath, targetPath);
  }

  public createDirectory(path: string): Promise<void> {
    return this.backend.createDirectory(path);
  }

  public createOrUpdateFile(path: string, content: any): Promise<void> {
    return this.backend.createOrUpdateFile(path, content);
  }

  public showFolderPickerDialog(): Promise<string> {
    return this.backend.showFolderPickerDialog();
  }

  public isProd(): boolean {
    return this.backend.isProdSync();
  }

  private mergeUInt8Arrays(myArrays: Uint8Array[]): Uint8Array {
    let length = 0;
    myArrays.forEach(item => {
      length += item.length;
    });

    // Create a new array with total length and merge all source arrays.
    let mergedArray = new Uint8Array(length);
    let offset = 0;
    myArrays.forEach(item => {
      mergedArray.set(item, offset);
      offset += item.length;
    });

    return mergedArray;
  }
}
