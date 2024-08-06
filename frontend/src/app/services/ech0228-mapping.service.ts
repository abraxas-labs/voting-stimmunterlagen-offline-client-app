/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Injectable } from '@angular/core';

@Injectable()
export class Ech0228MappingService {
  static VOTING_CARD_BFS = {
    description: 'BFS',
    paths: [
      'votingPerson.person.swiss.swissDomesticPerson.extension.municipalityRef',
      'votingPerson.person.swissAbroad.swissAbroadPerson.extension.municipalityRef',
      'votingPerson.person.foreigner.foreignerPerson.extension.municipalityRef',
    ],
  };
  static VOTING_CARD_POSTAGE_CODE = {
    description: 'POSTAGE_CODE',
    paths: [
      'votingPerson.deliveryAddress.addressInformation.swissZipCode',
      'votingPerson.deliveryAddress.addressInformation.foreignZipCode',
    ],
  };
  static VOTING_CARD_STREET = { description: 'STREET', paths: ['votingPerson.deliveryAddress.addressInformation.street'] };
  static VOTING_CARD_HOUSE_NUMBER = { description: 'HOUSE_NUMBER', paths: ['votingPerson.deliveryAddress.addressInformation.houseNumber'] };
  static VOTING_CARD_COMUNICATION_LAUNGUAGE = {
    description: 'COMUNICATION_LAUNGUAGE',
    paths: [
      'votingPerson.person.swiss.swissDomesticPerson.languageOfCorrespondance',
      'votingPerson.person.foreigner.foreignerPerson.languageOfCorrespondance',
      'votingPerson.person.swissAbroad.swissAbroadPerson.languageOfCorrespondance',
    ],
  };

  static VOTING_CARD_MUNICIPALITY_NAME = {
    description: 'MUNICIPALITY_NAME',
    paths: [
      'votingPerson.person.swiss.swissDomesticPerson.extension.municipalityName',
      'votingPerson.person.swissAbroad.swissAbroadPerson.extension.municipalityName',
      'votingPerson.person.foreigner.foreignerPerson.extension.municipalityName',
    ],
  };

  static MUNICIPALITY_TEMPLATE_REF = { description: 'TEMPLATE', paths: ['template'] };
  static MUNICIPALITY_ETEMPLATE_REF = { description: 'TEMPLATE', paths: ['etemplate'] };

  static CONTEST_DATE = { description: 'CONTEST_DATA', paths: ['votingCardDelivery.contestData.contest.contestDate'] };
  static CONTEST_ID = { description: 'CONTEST_ID', paths: ['votingCardDelivery.contestData.contest.contestIdentification'] };
}
