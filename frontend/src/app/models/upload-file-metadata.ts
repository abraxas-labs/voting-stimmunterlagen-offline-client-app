export interface UploadFileMetadata {
  filePath: string;
  fileType: 'POST_CONFIG' | 'POST_CODE' | 'CHVOTE' | 'ECH0045' | '0NONE' | 'ZIP' | 'CRYPT' | 'OPEN';
}
