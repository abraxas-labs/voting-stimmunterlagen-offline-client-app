import { PDF_CREATOR_SERVCE, PdfCreatorService } from './pdf-creator.service';
import { firstValueFrom, from, groupBy, Observable, Observer, of, throwError } from 'rxjs';
import { ElectronService } from './electron.service';
import { Ech228MappingPath } from '../models/ech0228/ech0228.model';
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
import { resolveValue } from './utils/value-resolver.utils';
import { VotingCardData } from '../models/ech0228/voting-card-data.model';

@Injectable()
export class VotingCardService {
  constructor(
    @Inject(PDF_CREATOR_SERVCE) private readonly pdfCreatorService: PdfCreatorService,
    @Inject(PDFMERGE_SERVICE) private readonly pdfMergeService: PdfMergeService,
    @Inject(SECURE_DELETE_SERVICE) private readonly secureDeleteService: SecureDeleteService,
    private readonly settingsService: SettingsService,
    private readonly appStateService: AppStateService,
    private readonly electronService: ElectronService,
  ) {}

  public generateVotingCards(jobContext: JobContext): Observable<Job[][] | any> {
    if (!jobContext.votingCardGroups) {
      return of([]);
    }

    return from(this.electronService.createDirectory(TEMP_PDF_DIR)).pipe(
      switchMap(() => from(jobContext.votingCardGroups)),
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

  public async initVotingCardGroups(jobContext: JobContext): Promise<any> {
    if (!jobContext.ech228 || !jobContext.ech228.votingCardDelivery.votingCardData) {
      throw throwError(() => 'missing ech value');
    }
    jobContext.votingCardGroups = undefined;

    const obs = of(jobContext.ech228.votingCardDelivery.votingCardData).pipe(
      switchMap(groups => {
        if (jobContext.groupe1 && jobContext.groupe1.description) {
          return this.groupVotingCards(groups, jobContext.groupe1.paths);
        }
        return of(groups);
      }),
      switchMap(groups => {
        if (jobContext.groupe2 && jobContext.groupe2.description) {
          return this.groupVotingCards(groups, jobContext.groupe2.paths);
        }
        return of(groups);
      }),
      switchMap(groups => {
        if (jobContext.groupe3 && jobContext.groupe3.description) {
          return this.groupVotingCards(groups, jobContext.groupe3.paths);
        }
        return of(groups);
      }),
      switchMap(groups => this.groupVotingCards(groups, jobContext.templateMapping.paths)),
      map(groups => {
        if (!jobContext.sorting || jobContext.sorting.length === 0) {
          return groups;
        }
        return this.sortValue(groups, jobContext.sorting);
      }),
      toArray(),
      map(groupValue => {
        jobContext.votingCardGroups = groupValue;
        return jobContext;
      }),
    );

    await firstValueFrom(obs);
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
      const firstValue = resolveValue(a, sortCondition.reference.paths);
      const secondValue = resolveValue(b, sortCondition.reference.paths);
      if (sortCondition.reference.description === Ech0228MappingService.VOTING_CARD_HOUSE_NUMBER.description) {
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

  private groupVotingCards(votingCardDatas: VotingCardData[], groupPaths: string[]): Observable<VotingCardData[]> {
    return from(votingCardDatas).pipe(
      groupBy(votingCardData => {
        if (votingCardData) {
          return resolveValue(votingCardData, groupPaths);
        }
      }),
      mergeMap(groupValue => {
        console.log('group', groupValue);
        return groupValue.pipe(toArray());
      }),
    );
  }

  public generateJobGroup(context: JobContext, groupValue: any[]): Job[] {
    const municipalityRef = resolveValue(groupValue[0], Ech0228MappingService.VOTING_CARD_BFS.paths);
    const template: any = pathCombine(
      E_VOTING_CONFIG_DIR,
      resolveValue(context.ech228!.votingCardDelivery.extension.municipalities[municipalityRef], context.templateMapping.paths) ?? '',
    );
    const rangeSize = this.settingsService.jobSize;
    let startFrom = 0;
    const jobs: Job[] = [];
    let to = Math.min(rangeSize, groupValue.length);
    let votersOfGroup: any[] = [];
    const jobModel = context.ech228;

    context.ech228!.votingCardDelivery.votingCardData = [];
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
              status: 'unencrypted',
            });
          }),
        ),
      ),
      switchMap(() => from(this.appStateService.updateJobGroup(jobGroup, groupIndex))),
      switchMap(() => this.secureDeleteService.deleteDirectory(groupIndexDirectoryPath, true)),
    );
  }

  public createFileName(jobGroup: Job[], jobContext: JobContext): string {
    const contestDate = resolveValue(jobGroup[0].model, Ech0228MappingService.CONTEST_DATE.paths);

    let fileName = '' + this.convertDate(contestDate);

    const groupBaseValue = jobGroup[0].voter[0];
    const groupSegment = this.buildGroupsSegment([jobContext.groupe1, jobContext.groupe2, jobContext.groupe3], groupBaseValue);

    fileName += '_' + resolveValue(jobGroup[0].model, Ech0228MappingService.CONTEST_ID.paths);

    if (!!groupSegment) {
      fileName += '_' + groupSegment;
    }
    fileName += '_' + this.getNumberOfVotes(jobGroup);
    return fileName;
  }

  public buildGroupsSegment(groups: Ech228MappingPath[], votingCardData: any): string {
    let groupsSegment = '';
    let isFirst = true;

    for (let group of groups) {
      if (!group || !group.description) {
        continue;
      }

      if (group.description === Ech0228MappingService.VOTING_CARD_BFS.description) {
        // use municiaplity name instead of bfs in the file name
        group = Ech0228MappingService.VOTING_CARD_MUNICIPALITY_NAME;
      }

      const segmentValue = resolveValue(votingCardData, group.paths) + '';

      if (!segmentValue) {
        continue;
      }

      groupsSegment += (isFirst ? '' : '_') + segmentValue.replace(/ /g, '_');
      isFirst = false;
    }

    return groupsSegment;
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
