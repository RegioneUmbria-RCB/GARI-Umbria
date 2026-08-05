
import { Varieta } from 'app/Model/metaschema/utilizzi/Varieta';
import { RisorsaProdotto } from '../risorse/RisorsaProdotto';

export class DettaglioSemina extends RisorsaProdotto {
    varieta: Varieta;
    codArticolo: string;
    regolamento: number;

    constructor() {
        super();
        this.classType = 'DettaglioSemina';
    }
}
