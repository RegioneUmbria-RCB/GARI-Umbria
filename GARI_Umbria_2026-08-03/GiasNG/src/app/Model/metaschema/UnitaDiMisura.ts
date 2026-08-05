import {BaseCodeDescr} from '../baseClass/baseCodeDescr';

export class UnitaDiMisura {
    codice: number;
    descrizione: string;
    simbolo?: string = '';
    tipoControllo?: BaseCodeDescr = new BaseCodeDescr(0);

    constructor(codice: number, descrizione: string, simbolo?: string) {
        this.codice = codice;
        this.descrizione = descrizione;
        this.simbolo = simbolo;
    }
}
