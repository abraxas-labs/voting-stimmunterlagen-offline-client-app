import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AppStateService } from '../../services/app-state.service';
import { AppStateStep } from '../../models/app-state.model';
import { StepActionsService } from '../../services/step-actions.service';

@Component({
  selector: 'app-initial-page',
  templateUrl: './initial-page.component.html',
})
export class InitialPageComponent implements OnInit {
  public isLoading = true;

  private readonly stepMapping = {
    [AppStateStep.Welcome]: '/welcome',
    [AppStateStep.Prepare]: '/prepare',
    [AppStateStep.VotingCardsConfiguration]: '/voting-cards-configuration',
    [AppStateStep.Preview]: '/preview',
    [AppStateStep.Generation]: '/job-overview',
    [AppStateStep.Download]: '/download',
  };

  constructor(
    private readonly router: Router,
    private readonly appStateService: AppStateService,
    private readonly stepActionsService: StepActionsService,
  ) {}

  public async ngOnInit(): Promise<void> {
    await this.appStateService.init();
    const currentStep = this.appStateService.state.step;

    try {
      if (currentStep > AppStateStep.Prepare) {
        await this.stepActionsService.initializePrepareStep();
      }

      if (currentStep > AppStateStep.VotingCardsConfiguration) {
        await this.stepActionsService.restoreGroupAndSortStep();
      }
    } catch (err) {
      console.error(err);
      console.log('Cannot restore the previous app state. App will be resetted...');
      this.router.navigate([this.stepMapping[AppStateStep.Welcome]]);
      return;
    }

    this.router.navigate([this.stepMapping[currentStep]]);
  }
}
