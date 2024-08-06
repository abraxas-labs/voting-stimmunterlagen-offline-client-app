/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { PropertyMunicipalities } from './porperty-data';

export interface Voter {
  data: any;
  pollDate: string;
  template: string;
  poll: any;
  communicationLanguage: string;
  yearOfBirth: number;
  receiverAddress: any;
  senderAddress: any;
  postageCode: any;
  printing: string;
  certificate: string;
  bfs: string;
  config: PropertyMunicipalities;
}
