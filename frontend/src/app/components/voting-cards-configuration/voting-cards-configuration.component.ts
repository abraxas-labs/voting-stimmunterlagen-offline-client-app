/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Ech0228MappingService } from '../../services/ech0228-mapping.service';
import { Component, OnInit } from '@angular/core';
import { AppStateStep } from '../../models/app-state.model';
import { AppStateService } from '../../services/app-state.service';
import { Router } from '@angular/router';
import { JobContext } from '../../services/jobs/job-context';
import { VotingCardService } from '../../services/voting-card.service';
import { StepActionsService } from '../../services/step-actions.service';

@Component({
  selector: 'app-voting-cards-configuration',
  templateUrl: './voting-cards-configuration.component.html',
  styleUrls: ['./voting-cards-configuration.component.scss'],
  standalone: false,
})
export class VotingCardsConfigurationComponent implements OnInit {
  public groupOptionsValue = [
    Ech0228MappingService.VOTING_CARD_BFS,
    Ech0228MappingService.VOTING_CARD_COMUNICATION_LAUNGUAGE,
    Ech0228MappingService.VOTING_CARD_POSTAGE_CODE,
  ];

  public sortOptions = [Ech0228MappingService.VOTING_CARD_POSTAGE_CODE, Ech0228MappingService.VOTING_CARD_STREET];
  public isGrouping = false;
  public sort1: { isASC: boolean; reference: any } = { isASC: true, reference: {} };
  public sort2: { isASC: boolean; reference: any } = { isASC: true, reference: {} };
  public fingerprint: string;
  public fingerprintRequired = false;

  constructor(
    public readonly context: JobContext,
    public readonly votingCardService: VotingCardService,
    private readonly router: Router,
    private readonly appStateService: AppStateService,
    private readonly stepActionsService: StepActionsService,
  ) {}

  public async ngOnInit(): Promise<void> {
    this.context.sorting = [];
    this.context.groupe1 = { description: '', paths: [] };
    this.context.groupe2 = { description: '', paths: [] };
    this.context.groupe3 = { description: '', paths: [] };
    await this.appStateService.updateStep(AppStateStep.VotingCardsConfiguration);

    this.fingerprintRequired =
      !!this.context.ech228?.votingCardDelivery.contestData &&
      !!this.context.ech228?.votingCardDelivery.contestData?.contest &&
      !!this.context.ech228?.votingCardDelivery.contestData?.contest?.eVotingPeriod &&
      !!this.context.ech228?.votingCardDelivery.contestData?.contest?.eVotingPeriod?.eVotingPeriodFrom &&
      !!this.context.ech228?.votingCardDelivery.contestData?.contest?.eVotingPeriod?.eVotingPeriodTill;
  }

  public getGroupOptions(value): { paths: string[]; description: string }[] {
    return this.groupOptionsValue.filter(groupElement => {
      return (
        groupElement === value ||
        (this.context.groupe1.description !== groupElement.description &&
          this.context.groupe2.description !== groupElement.description &&
          this.context.groupe3.description !== groupElement.description)
      );
    });
  }

  public getSortOptions(value): { paths: string[]; description: string }[] {
    return this.sortOptions.filter(groupElement => {
      return (
        groupElement === value ||
        (this.sort1.reference.description !== groupElement.description && this.sort2.reference.description !== groupElement.description)
      );
    });
  }

  private addToSort(value): void {
    if (!value.reference.description) {
      return;
    }
    this.context.sorting.push(value);
    if (value.reference.description === Ech0228MappingService.VOTING_CARD_STREET.description) {
      this.context.sorting.push({ isASC: value.isASC, reference: Ech0228MappingService.VOTING_CARD_HOUSE_NUMBER });
    }
  }

  public async createGroup(): Promise<void> {
    this.isGrouping = true;
    if (this.sort1.reference.description) {
      this.addToSort(this.sort1);
    }
    if (this.sort1.reference.description) {
      this.addToSort(this.sort2);
    }

    if (!!this.fingerprint) {
      this.stepActionsService.addFingerprintToMunicipalities(this.fingerprint);
    }

    await this.votingCardService.initVotingCardGroups(this.context);
    this.isGrouping = false;

    await this.appStateService.update(s => {
      s.grouping = [this.context.groupe1, this.context.groupe2, this.context.groupe3];
      s.sorting = this.context.sorting;
      s.fingerprint = this.fingerprint;
    });
    this.router.navigate(['/preview']);
  }
}
