import { BaseCodeDescr } from '../baseClass/baseCodeDescr';
import { IntervalloTemporale } from './IntervalloTemporale';

export class CodiceAnagrafe extends BaseCodeDescr {

    lunghezza: number;
    picture: string;
    tipo: string;
    gruppo: string;
    genitore: number;
    creatore: string;
    validita: IntervalloTemporale;
    flag_cancellazione: boolean;

    constructor(codice: number) {
        super(codice);
        this.flag_cancellazione = false;
    }
}
