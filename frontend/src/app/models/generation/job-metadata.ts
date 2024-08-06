/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export interface JobMetadata {
  completed: boolean;
  start: Date;
  end: Date;
  failed: boolean;
  outputFilename: string;
}
