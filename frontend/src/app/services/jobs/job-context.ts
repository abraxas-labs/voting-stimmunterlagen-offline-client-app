import { Ech228MappingPath, Ech0228Model } from '../../models/ech0228/ech0228.model';
import { Injectable } from '@angular/core';

@Injectable()
export class JobContext {
  templateMapping: Ech228MappingPath;
  ech228?: Ech0228Model;
  votingCardGroups: any;
  sorting: { isASC: boolean; reference: any }[];
  groupe1: Ech228MappingPath;
  groupe2: Ech228MappingPath;
  groupe3: Ech228MappingPath;
}
