import { CodiciAnagrafeValori, CodiciAnagrafeValoriChiave } from 'app/Model/anagrafiche/CodiciAnagrafeValori';
import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { RubricaVoci } from 'app/Model/anagrafiche/RubricaVoci';
import { BaseCodeDescr } from 'app/Model/baseClass/baseCodeDescr';
import { DropdownListItem, KendoServerResult } from 'gias-kendo-grid';

export class CooperativeKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

export type CooperativeKendoGridRow = {
    chiave: number;
    codcooperativa: number;
    cooperativa: string;
    numero: number;
    data: Date;
    flag_cancellazione: boolean;
};
