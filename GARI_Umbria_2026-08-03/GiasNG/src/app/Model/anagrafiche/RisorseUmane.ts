import { Contatto } from './Contatto';
import { IntervalloTemporale } from './IntervalloTemporale';
import { RapportoContabile } from './RapportoContabile';

export class RisorseUmane {
  codice: number;
  validita: IntervalloTemporale;
  settore: string;
  attivita: string;
  contatto: Contatto;
  rapportoContabile: RapportoContabile;
  flag_cancellazione: boolean;

  constructor() {
    this.flag_cancellazione = false;
    this.codice = 0;
    this.validita = new IntervalloTemporale();
    this.settore = '';
    this.attivita = '';
  }
}
