import {IntervalloTemporale} from "../anagrafiche/IntervalloTemporale";

export class PianoColturale {
  Piva: string;
  Programmazione_Cod: number;
  Programmazione_Des: string;
  Note: string;
  Validita_Inizio: Date;
  Validita_Fine: Date;
}

export interface IConfrontoPianoColturale {
  considerCatasto: boolean;
  considerVarieta: boolean;
  validita: IntervalloTemporale;
}
