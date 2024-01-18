import { DownloadPdfsModel } from './download-pdfs';
import { JobMetadata } from './generation/job-metadata';
import { UploadFileMetadata } from './upload-file-metadata';
import { Ech228MappingPath } from './ech0228/ech0228.model';

export interface AppState {
  step: AppStateStep;
  uploads: UploadFileMetadata[];
  grouping: Ech228MappingPath[];
  sorting: { isASC: boolean; reference: any }[];
  fingerprint?: string;
  jobGroups: JobMetadata[][];
  downloadPdfs: DownloadPdfsModel[];
}

export enum AppStateStep {
  Welcome,
  Prepare,
  VotingCardsConfiguration,
  Preview,
  Generation,
  Download,
}
