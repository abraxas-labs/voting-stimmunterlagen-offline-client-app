import { DownloadPdfsModel } from '../../models/download-pdfs';

export interface FileCheckable {
  checked: boolean;
  file: DownloadPdfsModel;
}
