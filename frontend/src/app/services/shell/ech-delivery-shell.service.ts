/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Observable, from } from 'rxjs';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { CommandParameter } from '../../models/shell/command-parameter';
import { EchDeliveryService } from '../ech-delivery.service';
import { Ech0228Model } from '../../models/ech0228/ech0228.model';
import { LogService } from '../log.service';
import { ElectronService } from '../electron.service';
import { VotingCardData } from '../../models/ech0228/voting-card-data.model';
import { PostSignatureValidationResult } from '../../models/post-signature-validation-result.model';
import { EchDeliveryGeneratorResult } from '../../models/ech-delivery-generator-result.model';

@Injectable()
export class EchDeliveryShellService<T> implements EchDeliveryService<T> {
  public constructor(private readonly electronService: ElectronService, private readonly logService: LogService) {}

  public importDataFromPaths(
    filePaths: string[],
    postSignatureValidationPaths: string[],
  ): Observable<EchDeliveryGeneratorResult | T | undefined | any> {
    const parameters = [
      new CommandParameter('--outstream', ''),
      new CommandParameter('--logfile', this.logService.generateLogFilePath('import-data-from-paths')),
    ];

    filePaths.forEach(filePath => {
      parameters.push(new CommandParameter('--in', filePath));
    });

    postSignatureValidationPaths.forEach(filePath => {
      parameters.push(new CommandParameter('--postSignatureValidationFile', filePath));
    });

    let votingCardCounter = 0;

    return from(
      this.electronService.requestShellExecuteChunked<EchDeliveryGeneratorResult>(
        environment.commands.echDeliveryToJsonConverter,
        parameters,
        null,
        (result, i, chunk) => {
          // chunk 1: delivery without voting cards
          if (i === 1) {
            return { delivery: this.jsonParse<Ech0228Model>(chunk) } as EchDeliveryGeneratorResult;
          }

          if (!result) {
            throw new Error('Generator result should not be null');
          }

          // chunk 2: post signature validation result
          if (i === 2) {
            result.postSignatureValidationResult = this.jsonParse<PostSignatureValidationResult>(chunk);
            return result;
          }

          // chunk 3: count of voting cards
          if (i === 3) {
            console.log('Expected Voting Cards: ' + chunk);
            result.delivery.votingCardDelivery.votingCardData = new Array(+chunk);
            return result;
          }

          // chunk 4-n: voting card batches
          const votingCards = this.jsonParse<VotingCardData[]>(chunk);

          for (const votingCard of votingCards) {
            result.delivery.votingCardDelivery.votingCardData![votingCardCounter++] = votingCard;
          }

          console.log(votingCardCounter + ' voting cards imported');
          return result;
        },
      ),
    );
  }

  private jsonParse<TParsed>(chunk: string): TParsed {
    const response = chunk.length > 0 ? JSON.parse(chunk) : undefined;

    if (response === undefined || response.errorCode) {
      console.error('Error on importDataFromPaths: ', response === undefined ? 'Response data is empty' : response);
      throw response;
    }

    return response;
  }
}
