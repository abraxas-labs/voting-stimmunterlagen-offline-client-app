/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

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

  public async requestShellExecuteChunked<T>(
    commandInfo: CommandInfo,
    commandParameters: CommandParameter[],
    input: any,
    builder: (accumulator: T | undefined, i: number, chunk: string) => T,
  ): Promise<T | undefined> {
    return new Promise((resolve, reject) => {
      let chunkNumber = 1;
      let accumulator: T | undefined = undefined;

      // eslint-disable-next-line prefer-const
      let removeListener: any;

      const listener: (e, chunk) => void = (_, chunk) => {
        try {
          if (chunk && chunk.exitCode === 0) {
            removeListener();
            resolve(accumulator);
            return;
          }

          accumulator = builder(accumulator, chunkNumber++, chunk);
        } catch (err) {
          removeListener();
          reject(err);
        }
      };

      removeListener = this.backend.shellExecuteChunkedEmitter(listener);
      this.backend.requestShellExecuteChunked(environment.toolsDirectory, commandInfo, commandParameters, input);
    });
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

  public showItemInFolder(filePath: string): Promise<void> {
    return this.backend.showItemInFolder(filePath);
  }

  public openPath(path: string): Promise<void> {
    return this.backend.openPath(path);
  }

  public getPathForFile(file: File): Promise<string> {
    return this.backend.getPathForFile(file);
  }

  public isProd(): boolean {
    return this.backend.isProdSync();
  }

  public getAppVersion(): boolean {
    return this.backend.getAppVersionSync();
  }

  private mergeUInt8Arrays(myArrays: Uint8Array[]): Uint8Array {
    let length = 0;
    myArrays.forEach(item => {
      length += item.length;
    });

    // Create a new array with total length and merge all source arrays.
    const mergedArray = new Uint8Array(length);
    let offset = 0;
    myArrays.forEach(item => {
      mergedArray.set(item, offset);
      offset += item.length;
    });

    return mergedArray;
  }
}
