/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Voter } from './voter';

export interface VoterChoice {
  numberOfVoters: number;
  voter: Voter[];
}
