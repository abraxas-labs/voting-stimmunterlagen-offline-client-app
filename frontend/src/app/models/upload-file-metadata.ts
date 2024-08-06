/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export interface UploadFileMetadata {
  filePath: string;
  fileType: 'POST_CONFIG' | 'POST_CODE' | 'ECH0045' | '0NONE' | 'ZIP' | 'OPEN';
}
