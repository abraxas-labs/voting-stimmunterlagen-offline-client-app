export interface UploadFileMetadata {
  filePath: string;
  fileType: 'POST_CONFIG' | 'POST_CODE' | 'ECH0045' | '0NONE' | 'ZIP' | 'OPEN';
}
