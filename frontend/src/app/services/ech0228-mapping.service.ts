import { Injectable } from '@angular/core';

@Injectable()
export class Ech0228MappingService {
  static BFS = { description: 'BFS', paths: ['Extension.MunicipalityRef'] };
  static POSTAGE_CODE = {
    description: 'POSTAGE_CODE',
    paths: ['Item.electoralAddress.addressInformation.swissZipCode', 'Item.electoralAddress.addressInformation.foreignZipCode'],
  };
  static STREET = { description: 'STREET', paths: ['Item.electoralAddress.addressInformation.street'] };
  static HOUSE_NUMBER = { description: 'HOUSE_NUMBER', paths: ['Item.electoralAddress.addressInformation.houseNumber'] };
  static COMUNICATION_LAUNGUAGE = {
    description: 'COMUNICATION_LAUNGUAGE',
    paths: [
      'Item.person.NationalityChoice.swissDomesticPerson.languageOfCorrespondance',
      'Item.person.NationalityChoice.foreignerPerson.languageOfCorrespondance',
      'Item.person.NationalityChoice.swissAbroadPerson.languageOfCorrespondance',
    ],
  };

  static PRINTING_REF = { description: 'PRINTING', paths: ['Extension.PrintingRef'] };
  static TEMPLATE_REF = { description: 'TEMPLATE', paths: ['template'] };
  static MUNICIPALITY_REF = { description: 'MUNICIPALITY', paths: ['Extension.MunicipalityRef'] };
  static ETEMPLATE_REF = { description: 'TEMPLATE', paths: ['etemplate'] };

  static CONTEST_DATE = { description: 'CONTEST_DATA', paths: ['votingCardDelivery.contestData.Item.contestDate'] };
}
