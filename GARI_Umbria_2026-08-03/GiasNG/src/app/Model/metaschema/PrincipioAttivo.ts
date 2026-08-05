import { BaseCodeDescr } from '../baseClass/baseCodeDescr';

export class PrincipioAttivo extends BaseCodeDescr {

    titolo: number;
    peso: number;
    percentualeSuperficieTrattabile: number;

    constructor(codice: number) {
        super(codice);
    }
}
