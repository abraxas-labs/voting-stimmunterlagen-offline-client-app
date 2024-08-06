/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Ech0228Model } from './ech0228/ech0228.model';
import { PostSignatureValidationResult } from './post-signature-validation-result.model';

export interface EchDeliveryGeneratorResult {
  delivery: Ech0228Model;
  postSignatureValidationResult: PostSignatureValidationResult;
}
