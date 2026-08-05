import { Optional } from '@angular/core';
import { CodiceAnagrafe } from './CodiceAnagrafe';
import { IntervalloTemporale } from './IntervalloTemporale';

export class CodiciAnagrafeValori {
    codiceAnagrafe: CodiceAnagrafe;
    valore: string;
    validita: IntervalloTemporale;
}

export class CodiciAnagrafeValoriChiave extends CodiciAnagrafeValori {
    chiave: number;
}
