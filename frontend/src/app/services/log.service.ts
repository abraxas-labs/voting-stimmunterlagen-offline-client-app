/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Injectable } from '@angular/core';
import { LOG_DIR } from '../common/path.constants';
import { pathCombine } from './utils/path.utils';

@Injectable()
export class LogService {
  public generateLogFilePath(fileNameWithoutExt: string): string {
    return pathCombine(LOG_DIR, fileNameWithoutExt + '-' + this.getDateTimeString() + '.txt');
  }

  // returns a date time string in format 'YYYYMMDD-HHMM'
  private getDateTimeString(): string {
    return new Date()
      .toISOString()
      .substring(0, 16)
      .replace(/([:\-])/g, '')
      .replace('T', '-');
  }
}
