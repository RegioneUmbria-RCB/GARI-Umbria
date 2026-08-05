import {BaseCodeDescr} from '../baseClass/baseCodeDescr';
import {IntervalloTemporale} from '../anagrafiche/IntervalloTemporale';

export class GruppoRaccolta extends BaseCodeDescr {

    validita: IntervalloTemporale;
    constructor(codice: number) {
        super(codice);
    }
}
export class ScriviGruppoRaccolta {
    public gruppoRaccolta: GruppoRaccolta;

    public tipoOperazione: number;
}
