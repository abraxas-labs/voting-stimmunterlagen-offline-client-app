export interface Ech228Model {
  DeliveryHeader?: any;
  votingCardDelivery: VotingCardDelivery;
  minorVersion?: any;
  extension: Extension;
}

export interface Ech228MappingPath {
  description: string;
  paths: string[];
}

export interface ContestDescriptionInfo {
  language?: string;
  contestDescription?: string;
}

export interface ContestDescription {
  contestDescriptionInfo?: ContestDescriptionInfo[];
}

export interface EVotingPeriod {
  eVotingPeriodFrom?: string;
  eVotingPeriodTill?: string;
}

export interface Item {
  contestIdentification?: string;
  contestDate?: string;
  contestDescription?: ContestDescription;
  eVotingPeriod?: EVotingPeriod;
}

export interface ContestData {
  eVotingContestCodes?: any;
  eVotingUrlInfo?: any;
  Item?: Item;
}

export interface LocalPersonId {
  personIdCategory?: string;
  personId?: string;
}

export interface DateOfBirth {
  yearMonthDay?: string;
}

export interface Item2 {
  localPersonId?: LocalPersonId;
  officialName?: string;
  firstName?: string;
  sex?: number;
  dateOfBirth?: DateOfBirth;
}

export interface IndividualContestCode {
  codeDesignation?: string;
  codeValue?: string;
}

export interface VoteDescriptionInfo {
  language?: string;
  voteDescription?: string;
}

export interface VoteDescription {
  voteDescriptionInfo?: VoteDescriptionInfo[];
}

export interface BallotDescriptionInfo {
  language?: string;
  ballotDescriptionLong?: string;
  ballotDescriptionShort?: string;
}

export interface BallotDescription {
  ballotDescriptionInfo?: BallotDescriptionInfo[];
}

export interface AnswerTextInformation {
  Language?: number;
  answerTextShort?: any;
  answerText?: string;
}

export interface AnswerOption {
  AnswerIdentification?: string;
  answerSequenceNumber?: string;
  answerTextInformation?: AnswerTextInformation[];
  individualVoteVerificationCode?: string;
}

export interface BallotQuestionInfo {
  language?: string;
  ballotQuestionTitle?: string;
  ballotQuestion?: string;
}

export interface BallotQuestion {
  ballotQuestionInfo?: BallotQuestionInfo[];
}

export interface Item3 {
  questionInformation?: any;
  tieBreakInformation?: any;
  QuestionIdentification?: string;
  BallotQuestionNumber?: number;
  answerOption?: AnswerOption[];
  BallotQuestion?: BallotQuestion;
}

export interface Ballot {
  BallotIdentification?: string;
  BallotPosition?: number;
  BallotDescription?: BallotDescription;
  BallotGroup?: any;
  Item?: Item3;
  Extension?: any;
}

export interface Vote {
  VoteIdentification?: string;
  VoteDescription?: VoteDescription;
  ballot?: Ballot[];
  individualVoteVerificationCode?: any;
}

export interface VotingCardIndividualCodes {
  individualContestCodes?: IndividualContestCode[];
  vote?: Vote[];
  electionGroupBallot?: any;
}

export interface Printing {
  name?: string;
  certificate?: string;
}

export interface TextBlocks {
  columnQuantity?: any;
  values?: any;
}

export interface Value {
  title?: string;
  text?: string;
}

export interface ETextBlocks {
  columnQuantity?: string;
  values?: Value[];
}

export interface Municipality {
  bfs?: string;
  name?: string;
  logo?: string;
  template?: any;
  etemplate?: string;
  pollOpening?: string;
  pollClosing?: string;
  deliveryType?: string;
  forwardDeliveryType?: string;
  returnDeliveryType?: string;
  textBlocks?: TextBlocks;
  eTextBlocks?: ETextBlocks;
  vcteVotingFingerprint?: string;
}

export interface Extension {
  Printing?: Printing;
  Municipality?: Municipality;
  Certificates: string[];
}

export interface VotingCardData {
  votingCardSequenceNumber?: string;
  frankingArea?: any;
  Item?: Item2;
  votingPlaceInformation?: any;
  votingCardIndividualCodes?: VotingCardIndividualCodes;
  VotingCardReturnAddress?: any;
  individualLogisticCode?: any;
  extension?: Extension;
}

export interface VotingCardDelivery {
  contestData?: ContestData;
  votingCardData?: VotingCardData[];
  logisticCode?: any;
}
