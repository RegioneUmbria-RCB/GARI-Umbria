import { UnitaDiMisura } from '../metaschema/UnitaDiMisura';
import { IntervalloTemporale } from './IntervalloTemporale';

export class CostoUnitario {
    codice: number;
    unitaDiMisura: UnitaDiMisura;
    prezzo: number;
    validita: IntervalloTemporale;
    flag_cancellazione: boolean;

    constructor() {
        this.flag_cancellazione = false;
    }
}

export class CostoUnitarioChiave extends CostoUnitario {
    chiave = 0;
}
