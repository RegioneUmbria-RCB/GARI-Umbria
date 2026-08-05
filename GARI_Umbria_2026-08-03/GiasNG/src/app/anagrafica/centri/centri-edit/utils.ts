import { CodiciAnagrafeValori, CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { RubricaVoci } from 'app/Model/anagrafiche/RubricaVoci';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { DropdownListItem, KendoServerResult } from 'gias-kendo-grid';

export class RubricaKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

export class CodiciAnagraficiServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

export class CodiciServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}


export class GiasCentriDDLItem {
    constructor(
        public readonly codice: string | number,
        public readonly descrizione: string,
        public dati: string = '',
        numCodice: boolean = false) {
            if(numCodice)
                this.codice = Number.parseInt(this.codice as string);
        }
}


export const centriCodici = '';

export type CodiciLoaded =  { codici: CodiciAnagrafeValoriChiave[], ddl: DropdownListItem[] }

export type RubricaLoaded =  { rubricaVoci: RubricaVoci[] }

export type CodiceKendoGridRow = {
    chiave: number, valore: string,
    codice: number, descrizione: string,
    dal: Date, al: Date
};

export type RubricaKendoGridRow = {
    codice: number;
    tipologia: string;
    valore: string;
    flag_cancellazione: boolean;
    chiave: number;
};
