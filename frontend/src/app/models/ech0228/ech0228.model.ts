/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { ContestData } from './contest.model';
import { Municipality } from './municipality.model';
import { VotingCardData } from './voting-card-data.model';

export interface Ech0228Model {
  votingCardDelivery: VotingCardDelivery;
}

export interface VotingCardDelivery {
  contestData: ContestData;
  votingCardData: VotingCardData[];
  extension: VotingCardDeliveryExtension;
}

export interface VotingCardDeliveryExtension {
  municipalities: Record<string, Municipality>;
  certificates: string[];
}

export interface Ech228MappingPath {
  description: string;
  paths: string[];
}
