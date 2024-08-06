/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export interface ShellExecuteDecodedResult {
  exitCode: number;
  data: string;
}

export interface ShellExecuteBinaryResult {
  exitCode: number;
  data: Uint8Array;
}
