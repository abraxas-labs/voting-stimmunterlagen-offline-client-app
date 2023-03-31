export class DownloadPdfsModel {
  filePath: string;
  fileName: string;
  status: 'unverschlüsselt' | 'verschlüsselt' | 'running' = 'unverschlüsselt';
}
