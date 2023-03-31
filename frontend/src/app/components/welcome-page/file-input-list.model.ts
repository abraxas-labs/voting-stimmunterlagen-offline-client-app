import { UploadFileMetadata } from '../../models/upload-file-metadata';

export interface FileInputListModel extends UploadFileMetadata {
  file: File;
  isRunning: boolean;
}
