import { Component, OnDestroy, OnInit } from '@angular/core';
import { VotingCardService } from '../../services/voting-card.service';
import { from, Observable, Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { JobContext } from '../../services/jobs/job-context';
import { environment } from '../../../environments/environment';
import { Ech0228MappingService } from '../../services/ech0228-mapping.service';
import { SettingsService } from '../../services/settings.service';
import { AppStateService } from '../../services/app-state.service';
import { AppStateStep } from '../../models/app-state.model';
import { E_VOTING_CONFIG_DIR } from '../../common/path.constants';
import { pathCombine } from '../../services/utils/path.utils';

@Component({
  selector: 'app-preview-page',
  templateUrl: './preview-page.component.html',
  styleUrls: ['./preview-page.component.scss'],
})
export class PreviewPageComponent implements OnInit, OnDestroy {
  public isLoading = false;
  public isValid = false;
  public pdfs: Array<Uint8Array | undefined>;
  public previewIndex: number;
  public pdfList = {};
  public numberOfPages: number;
  public displayCertificateSelection = false;
  public selectedCertificate: string;
  private queueSubscription: Subscription;

  constructor(
    private readonly votingCardService: VotingCardService,
    public readonly context: JobContext,
    private readonly settingsService: SettingsService,
    private readonly appStateService: AppStateService,
  ) {}

  public async ngOnInit(): Promise<void> {
    await this.appStateService.updateStep(AppStateStep.Preview);
    this.setPreview(0);
    this.generateQueue(1);
  }

  public ngOnDestroy(): void {
    if (this.queueSubscription) {
      this.queueSubscription.unsubscribe();
    }
  }

  public getGroupDescription(votingCardData): string | undefined {
    let groupName = '';
    if (this.context.groupe1 && this.context.groupe1.description) {
      groupName += `${this.votingCardService.getGroupObject(votingCardData, this.context.groupe1.paths)}`;
    }
    if (this.context.groupe2 && this.context.groupe2.description) {
      groupName += `_${this.votingCardService.getGroupObject(votingCardData, this.context.groupe2.paths)}`;
    }
    if (this.context.groupe3 && this.context.groupe3.description) {
      groupName += `_${this.votingCardService.getGroupObject(votingCardData, this.context.groupe3.paths)}`;
    }
    if (groupName === '') {
      return;
    }
    return groupName;
  }

  public setPreview(index): void {
    this.previewIndex = index;
    if (this.pdfList[index] && environment.production) {
      return;
    }

    this.generatePdf(index).subscribe();
  }

  public generatePdf(index): Observable<any> {
    if (!this.context.ech228) {
      return from('no value');
    }
    const groupValue = this.context.ech228Grouped[index];
    const municipalityRef = this.votingCardService.getGroupObject(groupValue[0], [Ech0228MappingService.MUNICIPALITY_REF.paths[0]]);
    var template: any = pathCombine(
      E_VOTING_CONFIG_DIR,
      this.votingCardService.getGroupObject(
        this.context.ech228['extension']['Municipalities'][municipalityRef],
        this.context.templateMapping.paths,
      ),
    );
    this.context.ech228.votingCardDelivery.votingCardData = groupValue.slice(0, this.settingsService.numberOfPreview);

    return this.votingCardService
      .generateVotingCardsFor(template, this.context.ech228)
      .pipe(tap(pdfBinary => (this.pdfList[index] = pdfBinary)));
  }

  private generateQueue(index = 0): void {
    console.log('index', index);
    if (!this.context.ech228Grouped || !this.context.ech228Grouped[index]) {
      return;
    }

    if (this.pdfList[index]) {
      this.generateQueue(++index);
    }

    this.queueSubscription = this.generatePdf(index).subscribe(() => {
      this.generateQueue(++index);
    });
  }
}
