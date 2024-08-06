/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { UploadFileMetadata } from '../../models/upload-file-metadata';

export interface FileInputListModel extends UploadFileMetadata {
  file: File;
  isRunning: boolean;
}
