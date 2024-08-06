/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, OnInit } from '@angular/core';
import { AppStateStep } from '../../models/app-state.model';
import { UploadFileMetadata } from '../../models/upload-file-metadata';
import { AppStateService } from '../../services/app-state.service';
import { ImportErrorCode } from '../../models/import-error-code';
import { JobContext } from '../../services/jobs/job-context';
import { StepActionsService } from '../../services/step-actions.service';
import { POST_SIGNATURE_VALIDATION_SUCCESS, PostSignatureValidationResult } from '../../models/post-signature-validation-result.model';

@Component({
  selector: 'app-value-prepare',
  templateUrl: './value-prepare.component.html',
  styleUrls: ['./value-prepare.component.scss'],
})
export class ValuePrepareComponent implements OnInit {
  private readonly uploads: UploadFileMetadata[];

  public hasErrors = false;
  public isRunning = false;
  public errorValue: ImportErrorCode | undefined;
  public isNotFoundError = false;

  public countOfVotingCards?: number;
  public postSignatureValidationResult?: PostSignatureValidationResult;
  public postSignatureValidationSuccess = false;

  constructor(
    private readonly stepActionsService: StepActionsService,
    private readonly appStateService: AppStateService,
    private readonly jobContext: JobContext,
  ) {
    this.uploads = this.appStateService.state.uploads;
  }

  public async ngOnInit(): Promise<void> {
    this.isRunning = true;
    await this.appStateService.updateStep(AppStateStep.Prepare);

    try {
      await this.stepActionsService.initializePrepareStep();
      this.countOfVotingCards = this.jobContext.ech228?.votingCardDelivery?.votingCardData?.length;
      this.postSignatureValidationResult = this.jobContext.postSignatureValidationResult;
      this.postSignatureValidationSuccess = this.jobContext.postSignatureValidationResult?.code === POST_SIGNATURE_VALIDATION_SUCCESS;
      this.isRunning = false;
    } catch (error) {
      console.log(error);
      this.errorValue = error;
      this.isRunning = false;
      this.hasErrors = true;
      this.isNotFoundError = !!this.errorValue && this.errorValue.errorCode.includes('NOT_FOUND');
    }
  }
}
