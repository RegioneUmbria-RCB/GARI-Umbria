import {IntervalloTemporale} from '../anagrafiche/IntervalloTemporale';

export class Istat {
  reg: string = '000';
  prov: string;
  com: string;
  cap: string;
  localita: string;
  comuni_prov: string;
  validita: IntervalloTemporale;
  codiceBelfiore: string;
}
