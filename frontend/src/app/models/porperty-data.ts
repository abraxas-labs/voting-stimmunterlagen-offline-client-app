/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

export interface PropertyData {
  polldate: string;
  printings: PropertyPrintings[];
}

export interface PropertyPrintings {
  name: string;
  certificate: string;
  municipalities: PropertyMunicipalities[];
}

export interface PropertyMunicipalities {
  bfs: string;
  name: string;
  template: string;
  etemplate: string;
  textBlocks: PropertyTextBlocks;
}

export interface PropertyTextBlocks {
  columnQuantity: string;
  values: string[];
}
