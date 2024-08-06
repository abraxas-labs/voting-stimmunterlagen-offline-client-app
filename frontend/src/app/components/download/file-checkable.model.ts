/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { DownloadPdfsModel } from '../../models/download-pdfs';

export interface FileCheckable {
  checked: boolean;
  file: DownloadPdfsModel;
}
