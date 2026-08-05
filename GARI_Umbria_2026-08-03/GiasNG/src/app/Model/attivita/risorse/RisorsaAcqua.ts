import { Risorsa } from './Risorsa';

export class RisorsaAcqua extends Risorsa {
    acqua: number;
    doseAcqua: DoseAcqua;


    constructor() {
        super();
        this.classType = 'RisorsaAcqua';
    }
}

export enum DoseAcqua {
    TOTALE,
    HA
}
