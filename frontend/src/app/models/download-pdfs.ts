export class DownloadPdfsModel {
  filePath: string;
  fileName: string;
  status: 'unencrypted' | 'encrypted' | 'running' = 'unencrypted';
}
