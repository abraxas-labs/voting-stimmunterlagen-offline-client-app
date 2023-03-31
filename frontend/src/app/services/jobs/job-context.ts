import { Ech228MappingPath, Ech228Model } from '../../models/ech228.model';
import { Injectable } from '@angular/core';

@Injectable()
export class JobContext {
  templateMapping: Ech228MappingPath;
  ech228: Ech228Model | undefined;
  ech228Grouped: any;
  sorting: { isASC: boolean; reference: any }[];
  groupe1: Ech228MappingPath;
  groupe2: Ech228MappingPath;
  groupe3: Ech228MappingPath;
}
