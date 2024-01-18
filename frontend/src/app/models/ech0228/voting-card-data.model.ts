export interface VotingCardData {
  votingPerson: VotingPerson;
}

export interface VotingPerson {
  person: Person;
}

export interface Person {
  swiss?: SwissPerson;
  swissAbroad?: SwissAbroadPerson;
  foreigner?: ForeignerPerson;
}

export interface SwissPerson {
  swissDomesticPerson: PersonData;
}

export interface SwissAbroadPerson {
  swissAbroadPerson: PersonData;
}

export interface ForeignerPerson {
  foreignerPerson: PersonData;
}

export interface PersonData {
  extension: PersonDataExtension;
}

export interface PersonDataExtension {
  municipalityRef: string;
  municipalityName: string;
  printingRef: string;
}
