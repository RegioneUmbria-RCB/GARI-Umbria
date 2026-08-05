import {BaseCodeDescr} from '../baseClass/baseCodeDescr';

export class FaseCicloColturale extends BaseCodeDescr {
    disciplinarePubblicoPrivato: boolean;

    constructor(codice: number, descrizione?: string) {
        super(codice, descrizione);
    }
}
