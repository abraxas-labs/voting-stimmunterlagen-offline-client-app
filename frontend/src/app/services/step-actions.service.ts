import { Inject, Injectable } from '@angular/core';
import { Municipality } from '../models/ech228.model';
import { AppStateService } from './app-state.service';
import { ChVoteDataService, CH_VOTE_DATA_SERVICE } from './ch-vote-data.service';
import { Ech0228MappingService } from './ech0228-mapping.service';
import { JobContext } from './jobs/job-context';
import { VotingCardService } from './voting-card.service';

@Injectable()
export class StepActionsService {
  constructor(
    private readonly jobContext: JobContext,
    private readonly appStateService: AppStateService,
    @Inject(CH_VOTE_DATA_SERVICE) private readonly chVoteDataService: ChVoteDataService<any>,
    private readonly votingCardService: VotingCardService,
  ) {}

  public async initializePrepareStep(): Promise<void> {
    this.jobContext.ech228 = undefined;
    this.jobContext.ech228Grouped = undefined;

    const uploads = this.appStateService.state.uploads;

    if (!uploads || uploads.length < 2) {
      console.error('Value Prepare: No data');
      return;
    }

    if (uploads.some(f => f.fileType === 'ECH0045')) {
      this.jobContext.templateMapping = Ech0228MappingService.TEMPLATE_REF;
    } else {
      this.jobContext.templateMapping = Ech0228MappingService.ETEMPLATE_REF;
    }

    var response = await this.chVoteDataService.importDataFromPaths(uploads.map(fileItem => fileItem.filePath)).toPromise();

    this.jobContext.ech228 = response;
  }

  public async restoreGroupAndSortStep(): Promise<void> {
    const state = this.appStateService.state;
    this.jobContext.groupe1 = state.grouping[0];
    this.jobContext.groupe2 = state.grouping[1];
    this.jobContext.groupe3 = state.grouping[2];
    this.jobContext.sorting = state.sorting;

    if (!!state.fingerprint) {
      this.addFingerprintToMunicipalities(state.fingerprint);
    }

    await this.votingCardService.groupValue(this.jobContext).toPromise();
  }

  public addFingerprintToMunicipalities(fingerprint: string): void {
    const municipalities = Object.values(this.jobContext.ech228?.['extension']['Municipalities']) as Municipality[];
    for (const municipality of municipalities) {
      if (!!municipality.vcteVotingFingerprint) {
        continue;
      }

      municipality.vcteVotingFingerprint = fingerprint;
    }
  }
}
