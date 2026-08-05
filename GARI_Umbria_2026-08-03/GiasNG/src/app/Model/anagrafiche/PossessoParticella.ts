import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { TitoloDiPossesso } from '../metaschema/TitoloDiPossesso';
import { IntervalloTemporale } from './IntervalloTemporale';

export class PossessoParticella {
    codice: number;
    titolo_Di_Possesso: TitoloDiPossesso;
    validita: IntervalloTemporale;
    Area: number;
    flag_cancellazione: boolean;
    codice_particella: string;

    constructor() {
        this.flag_cancellazione = false;
    }
}
