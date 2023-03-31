export interface JobMetadata {
  completed: boolean;
  start: Date;
  end: Date;
  failed: boolean;
  outputFilename: string;
}
