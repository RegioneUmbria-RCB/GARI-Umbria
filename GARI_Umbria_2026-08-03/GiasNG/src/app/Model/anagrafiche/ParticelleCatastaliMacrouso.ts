import { Macrouso } from '../metaschema/Macrouso';
import { IntervalloTemporale } from './IntervalloTemporale';

export class ParticelleCatastaliMacrouso {
    macrouso: Macrouso;
    validita: IntervalloTemporale;
    Area: number;

    Piva: string;
    NumeroFascicolo: string;
    DataValidazioneFascicolo: Date;

}
