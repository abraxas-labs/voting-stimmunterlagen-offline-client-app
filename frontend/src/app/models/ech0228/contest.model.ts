/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export interface ContestData {
  contest: Contest;
}

export interface Contest {
  contestIdentification: string;
  contestDate: string;
  eVotingPeriod?: EVotingPeriod;
}

export interface EVotingPeriod {
  eVotingPeriodFrom?: string;
  eVotingPeriodTill?: string;
}
