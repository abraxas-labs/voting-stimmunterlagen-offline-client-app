import { Observable, from } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CommandParameter } from '../../models/shell/command-parameter';
import { EchDeliveryService } from '../ech-delivery.service';
import { Ech0228Model } from '../../models/ech0228/ech0228.model';
import { LogService } from '../log.service';
import { ElectronService } from '../electron.service';
import { VotingCardData } from '../../models/ech0228/voting-card-data.model';

@Injectable()
export class EchDeliveryShellService<T> implements EchDeliveryService<T> {
  public constructor(private readonly electronService: ElectronService, private readonly logService: LogService) {}

  public importDataFromPaths(filePaths: string[]): Observable<Ech0228Model | T | undefined | any> {
    const parameters = [
      new CommandParameter('--outstream', ''),
      new CommandParameter('--logfile', this.logService.generateLogFilePath('import-data-from-paths')),
    ];

    filePaths.forEach(filePath => {
      parameters.push(new CommandParameter('--in', filePath));
    });

    let votingCardCounter = 0;

    return from(
      this.electronService.requestShellExecuteChunked<Ech0228Model>(
        environment.commands.echDeliveryToJsonConverter,
        parameters,
        null,
        (delivery, i, chunk) => {
          // chunk 1: delivery without voting cards
          if (i === 1) {
            return this.jsonParse<Ech0228Model>(chunk);
          }

          if (!delivery) {
            throw new Error('Delivery should not be null');
          }

          // chunk 2: count of voting cards
          if (i === 2) {
            console.log('Expected Voting Cards: ' + chunk);
            delivery.votingCardDelivery.votingCardData = new Array(+chunk);
            return delivery;
          }

          // chunk 3-n: voting card batches
          const votingCards = this.jsonParse<VotingCardData[]>(chunk);

          for (let votingCard of votingCards) {
            delivery.votingCardDelivery.votingCardData![votingCardCounter++] = votingCard;
          }

          console.log(votingCardCounter + ' voting cards imported');
          return delivery;
        },
      ),
    );
  }

  private jsonParse<TParsed>(chunk: string): TParsed {
    const response = chunk.length > 0 ? JSON.parse(chunk) : undefined;

    if (response === undefined || response['ErrorCode']) {
      console.error('Error on importDataFromPaths: ', response === undefined ? 'Response data is empty' : response);
      throw response;
    }

    return response;
  }
}
