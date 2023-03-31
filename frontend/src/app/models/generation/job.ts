import { JobRange } from './job-range';
import { JobMetadata } from './job-metadata';

export class Job implements JobMetadata {
  public completed: boolean;
  public start: Date;
  public end: Date;
  public failed: boolean;
  public outputFilename: string;

  constructor(public layoutPath: string, public model: any, public range: JobRange, public voter?: any) {}
}
