/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

// keep in sync with Voting.Stimmunterlagen.OfflineClient.Shared.ContestConfiguration
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

export interface TextBlocks {
  columnQuantity?: any;
  values?: any;
}

export interface ETextBlocks {
  columnQuantity?: string;
  values?: Value[];
}

export interface Value {
  title?: string;
  text?: string;
}
