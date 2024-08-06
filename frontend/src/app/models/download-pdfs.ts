/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export class DownloadPdfsModel {
  filePath: string;
  fileName: string;
  status: 'unencrypted' | 'encrypted' | 'running' = 'unencrypted';
}
