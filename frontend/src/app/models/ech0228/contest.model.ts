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
