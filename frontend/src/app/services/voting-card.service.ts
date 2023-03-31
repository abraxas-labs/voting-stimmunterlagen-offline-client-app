import { PDF_CREATOR_SERVCE, PdfCreatorService } from './pdf-creator.service';
import { firstValueFrom, from, groupBy, Observable, Observer, of, throwError } from 'rxjs';
import { ElectronService } from './electron.service';
import { VotingCardData } from '../models/ech228.model';
import { Ech0228MappingService } from './ech0228-mapping.service';
import { E_VOTING_CONFIG_DIR, OUT_PDF_DIR, TEMP_PDF_DIR } from '../common/path.constants';
import { map, mergeMap, switchMap, toArray } from 'rxjs/operators';
import { JobContext } from './jobs/job-context';
import { Job } from '../models/generation/job';
import { AppState } from '../models/app-state.model';
import { Inject, Injectable } from '@angular/core';
import { SECURE_DELETE_SERVICE, SecureDeleteService } from './secure-delete.service';
import { PDFMERGE_SERVICE, PdfMergeService } from './pdf-merge.service';
import { pathCombine } from './utils/path.utils';
import { AppStateService } from './app-state.service';
import { JobRange } from '../models/generation/job-range';
import { SettingsService } from './settings.service';
import { environment } from '../../environments/environment';

@Injectable()
export class VotingCardService {
  private debug = environment.production;

  constructor(
    @Inject(PDF_CREATOR_SERVCE) private readonly pdfCreatorService: PdfCreatorService,
    @Inject(PDFMERGE_SERVICE) private readonly pdfMergeService: PdfMergeService,
    @Inject(SECURE_DELETE_SERVICE) private readonly secureDeleteService: SecureDeleteService,
    private readonly settingsService: SettingsService,
    private readonly appStateService: AppStateService,
    private readonly electronService: ElectronService,
  ) {}

  public generateVotingCards(jobContext: JobContext): Observable<Job[][] | any> {
    if (!jobContext.ech228Grouped) {
      return of([]);
    }

    return from(this.electronService.createDirectory(TEMP_PDF_DIR)).pipe(
      switchMap(() => from(jobContext.ech228Grouped)),
      map((votingData: any) => this.generateJobGroup(jobContext, votingData)),
      toArray(),
      switchMap((jobGroups: Job[][]) => {
        const appState = this.appStateService.state;
        this.applyAppStateToJobGroups(appState, jobGroups);
        const startIndex = appState.downloadPdfs.length;

        return new Observable(observer => {
          observer.next(jobGroups);
          this._startNextJob(jobGroups, observer, jobContext, startIndex);
        });
      }),
    );
  }

  public groupValue(jobContext: JobContext): Observable<any> {
    if (!jobContext.ech228 || !jobContext.ech228.votingCardDelivery.votingCardData) {
      throw throwError(() => 'missing ech value');
    }
    jobContext.ech228Grouped = undefined;

    return this.chVoteGroup(jobContext.ech228.votingCardDelivery.votingCardData, Ech0228MappingService.PRINTING_NAME.paths).pipe(
      switchMap(groups => {
        if (jobContext.groupe1 && jobContext.groupe1.description) {
          return this.chVoteGroup(groups, jobContext.groupe1.paths);
        }
        return of(groups);
      }),
      switchMap(groups => {
        if (jobContext.groupe2 && jobContext.groupe2.description) {
          return this.chVoteGroup(groups, jobContext.groupe2.paths);
        }
        return of(groups);
      }),
      switchMap(groups => {
        if (jobContext.groupe3 && jobContext.groupe3.description) {
          return this.chVoteGroup(groups, jobContext.groupe3.paths);
        }
        return of(groups);
      }),
      switchMap(groups => this.chVoteGroup(groups, jobContext.templateMapping.paths)),
      map(groups => {
        if (!jobContext.sorting || jobContext.sorting.length === 0) {
          return groups;
        }
        const sorted = this.sortValue(groups, jobContext.sorting);
        /** debug value **/
        if (this.debug) {
          this.logGroupAndSortResult(sorted);
        }

        return sorted;
      }),

      toArray(),
      map(groupValue => {
        jobContext.ech228Grouped = groupValue;
        return jobContext;
      }),
    );
  }

  private logGroupAndSortResult(sorted: VotingCardData[]): void {
    const debugSortValue: any = [];
    for (const value of sorted) {
      const test = {
        post: this.getGroupObject(value, Ech0228MappingService.POSTAGE_CODE.paths),
        STREET: this.getGroupObject(value, Ech0228MappingService.STREET.paths),
        HOUSE_NUMBER: this.getGroupObject(value, Ech0228MappingService.HOUSE_NUMBER.paths),
      };
      debugSortValue.push(test);
    }
    console.log(debugSortValue);
  }

  private applyAppStateToJobGroups(appState: AppState, jobGroups: Job[][]): void {
    const persistedJobGroups = appState.jobGroups;

    for (let i = 0; i < jobGroups.length; i++) {
      const jobGroup = jobGroups[i];
      const persistedJobGroup = persistedJobGroups[i];

      if (!persistedJobGroup) {
        continue;
      }

      for (let j = 0; j < jobGroup.length; j++) {
        const job = jobGroup[j];
        const persistedJob = persistedJobGroup[j];

        if (!persistedJob) {
          continue;
        }

        if (!persistedJob.completed) {
          return;
        }

        job.completed = persistedJob.completed;
        job.start = persistedJob.start;
        job.end = persistedJob.end;
        job.failed = persistedJob.failed;
        job.outputFilename = persistedJob.outputFilename;
      }
    }
  }

  private sortValue(votingCardDatas: VotingCardData[], sortObjects: { isASC: boolean; reference: any }[]): VotingCardData[] {
    return votingCardDatas.sort((a, b) => this.sort(a, b, sortObjects));
  }

  private sort(a: VotingCardData, b: VotingCardData, sortObjects: { isASC: boolean; reference: any }[]): number {
    for (const sortCondition of sortObjects) {
      const firstValue = this.getGroupObject(a, sortCondition.reference.paths);
      const secondValue = this.getGroupObject(b, sortCondition.reference.paths);
      if (sortCondition.reference.description === Ech0228MappingService.HOUSE_NUMBER.description) {
        const firstStreetObject = this.splitNumberAndString(firstValue);
        const secondStreetObject = this.splitNumberAndString(secondValue);

        let sortOrder = this.sortCheckValue(firstStreetObject.number, secondStreetObject.number, sortCondition.isASC);
        if (sortOrder !== 0) {
          return sortOrder;
        }
        sortOrder = this.sortCheckValue(firstStreetObject.string, secondStreetObject.string, sortCondition.isASC);
        if (sortOrder !== 0) {
          return sortOrder;
        }
      } else {
        const sortOrder = this.sortCheckValue(firstValue, secondValue, sortCondition.isASC);
        if (sortOrder !== 0) {
          return sortOrder;
        }
      }
    }
    return 0;
  }

  private sortCheckValue(firstValue, secondValue, IsAsc = true): number {
    if (firstValue < secondValue || (firstValue === undefined && secondValue !== undefined)) {
      if (IsAsc) {
        return -1;
      }
      return 1;
    }
    if (firstValue > secondValue || (firstValue !== undefined && secondValue === undefined)) {
      if (IsAsc) {
        return 1;
      }
      return -1;
    }
    return 0;
  }

  private splitNumberAndString(value: string): { number: number | undefined; string: string } {
    let number;
    let string;
    if (!value) {
      return { number, string };
    }
    const numberValue = value.match(/\d+\.?\d?/g);
    const stringValue = value.match(/[^\d\.]+/g);
    if (numberValue) {
      number = Number(numberValue[0]);
    }
    if (stringValue) {
      string = stringValue[0];
    }
    return { number, string };
  }

  // grouping
  private chVoteGroup(votingCardDatas: VotingCardData[], groupPaths: string[]): Observable<any[]> {
    const source = from(votingCardDatas);
    return source.pipe(
      groupBy(votingCardData => {
        if (votingCardData) {
          return this.getGroupObject(votingCardData, groupPaths);
        }
      }),
      mergeMap(groupValue => {
        console.log('group', groupValue);
        return groupValue.pipe(toArray());
      }),
    );
  }

  public getGroupObject(votingCardData: VotingCardData, paths: string[]): any {
    for (const path of paths) {
      const value = this.getValueFromPaths(votingCardData, path);
      if (value !== undefined) {
        return value;
      }
    }
  }

  public getValueFromPaths(votingCardData: VotingCardData, path: string): any {
    const pathElements = path.split('.');
    let groupElement = votingCardData;
    for (const pathElement of pathElements) {
      groupElement = groupElement[pathElement];
      if (groupElement === undefined) {
        return undefined;
      }
    }
    return groupElement;
  }

  public generateJobGroup(context, groupValue: any[]): Job[] {
    const municipalityRef = this.getGroupObject(groupValue[0], [Ech0228MappingService.MUNICIPALITY_REF.paths[0]]);
    const template: any = pathCombine(
      E_VOTING_CONFIG_DIR,
      this.getGroupObject(context.ech228['extension']['Municipalities'][municipalityRef], context.templateMapping.paths) ?? '',
    );
    const rangeSize = this.settingsService.jobSize;
    let startFrom = 0;
    const jobs: Job[] = [];
    let to = Math.min(rangeSize, groupValue.length);
    let votersOfGroup: any[] = [];
    const jobModel = context.ech228;

    context.ech228.votingCardDelivery.votingCardData = undefined;
    do {
      votersOfGroup = groupValue.slice(startFrom, to);
      if (votersOfGroup.length) {
        const job = new Job(template, jobModel, new JobRange(startFrom + 1, to), votersOfGroup);
        jobs.push(job);
        startFrom = to;
        to = Math.min(groupValue.length, startFrom + rangeSize);
      }
    } while (votersOfGroup.length > 0);
    return jobs;
  }

  public generateVotingCardsFor(layout: string, model: any): Observable<Uint8Array | undefined> {
    console.log('jobs', model);
    return this.pdfCreatorService.generate(layout, model);
  }

  private async _startNextJob(jobGroups: Job[][], observer: Observer<Job[][]>, jobContext, groupIndex = 0): Promise<void> {
    const jobGroup = jobGroups[groupIndex];

    if (!jobGroup) {
      observer.complete();
      return;
    }

    const next = jobGroup.find(job => !job.completed && !job.failed);
    if (!next) {
      if (jobGroups.length - 1 === groupIndex) {
        this.mergePdfs(jobGroup, groupIndex, jobContext).subscribe(() => observer.complete());
        return;
      }
      this.mergePdfs(jobGroup, groupIndex, jobContext).subscribe();
      groupIndex++;
      await this._startNextJob(jobGroups, observer, jobContext, groupIndex);
      return;
    }

    next.start = new Date();
    const filename = (next.outputFilename = await this._createJobFilename(next, groupIndex));

    next.model.votingCardDelivery.votingCardData = next.voter;

    const result = await firstValueFrom(this.pdfCreatorService.generateAndSave(next.layoutPath, next.model, filename));
    if (result) {
      next.completed = true;
    } else {
      next.failed = true;
    }
    next.end = new Date();

    await this.appStateService.updateJobGroup(jobGroup, groupIndex);
    await this._startNextJob(jobGroups, observer, jobContext, groupIndex);
  }

  private async _createJobFilename(job: Job, jobIndex: number): Promise<string> {
    const pathFolder = pathCombine(TEMP_PDF_DIR, jobIndex.toString());
    await this.electronService.createDirectory(pathFolder);

    const jobForm = String('0000000' + job.range.from).slice(-7);
    const jobTo = String('0000000' + job.range.to).slice(-7);
    return pathCombine(pathFolder, `${jobForm}-${jobTo}-${job.start.getTime()}.pdf`);
  }

  public mergePdfs(jobGroup: Job[], groupIndex: number, jobContext: JobContext): Observable<boolean> {
    const fileName = this.createFileName(jobGroup, jobContext);
    const filePath = pathCombine(OUT_PDF_DIR, fileName + '.pdf');
    const groupIndexDirectoryPath = pathCombine(TEMP_PDF_DIR, groupIndex.toString());

    return from(this.electronService.createDirectory(OUT_PDF_DIR)).pipe(
      switchMap(() => this.pdfMergeService.mergeFromFolder(groupIndexDirectoryPath, filePath)),
      switchMap(() =>
        from(
          this.appStateService.update(s => {
            s.downloadPdfs.push({
              filePath: filePath,
              fileName: fileName + '.pdf',
              status: 'unverschlüsselt',
            });
          }),
        ),
      ),
      switchMap(() => from(this.appStateService.updateJobGroup(jobGroup, groupIndex))),
      switchMap(() => this.secureDeleteService.deleteDirectory(groupIndexDirectoryPath, true)),
    );
  }

  public createFileName(jobGroup: Job[], jobContext: JobContext): string {
    const tempDateString = this.getGroupObject(jobGroup[0].model, Ech0228MappingService.CONTEST_DATE.paths);
    const printingRef = this.getGroupObject(jobGroup[0].voter[0], [Ech0228MappingService.PRINTING_REF.paths[0]]);
    let fileName = '' + this.convertDate(tempDateString);

    const groupBaseValue = jobGroup[0].voter[0];
    fileName += '_' + jobGroup[0].model.extension.Printings[printingRef].name;
    if (jobContext.groupe1 && jobContext.groupe1.description) {
      fileName += '_' + this.getGroupObject(groupBaseValue, jobContext.groupe1.paths);
    }
    if (jobContext.groupe2 && jobContext.groupe2.description) {
      fileName += '_' + this.getGroupObject(groupBaseValue, jobContext.groupe2.paths);
    }
    if (jobContext.groupe3 && jobContext.groupe3.description) {
      fileName += '_' + this.getGroupObject(groupBaseValue, jobContext.groupe3.paths);
    }

    fileName += '_' + this.getNumberOfVotes(jobGroup);
    return fileName;
  }

  private getNumberOfVotes(jobs): number {
    let numberOfVoters = 0;
    jobs.forEach(job => (numberOfVoters = numberOfVoters + job.voter.length));
    return numberOfVoters;
  }

  private convertDate(date: string): string {
    const dataArray = date.split(' ').join('.').split('.');
    const contestYear = dataArray[2];
    const contestMonth = dataArray[1];
    const contestDay = dataArray[0];
    return contestYear + contestMonth + contestDay;
  }
}
