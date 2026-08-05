import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ModelEntry, KendoGridModel, KendoServerResult, KendoGridColumn,
    KendoGridRow, JsonKendoResult } from 'gias-kendo-grid';
import { KendoCentroModel } from '../centri/centri.models';

export class FabbricatiModel extends KendoGridModel {
    chiave: ModelEntry;
    Piva: ModelEntry;
    Sa_Cod: ModelEntry;
    Fabbricato_Cod: ModelEntry;
    Fabbricato_des:ModelEntry;
    Sa_Nome: ModelEntry;
    Tipo: ModelEntry;
    Tipo_Fabbricato_Cod: ModelEntry;

    Data_Creazione: ModelEntry;
    Data_Modifica: ModelEntry;
    Utente_Creazione: ModelEntry;
    Utente_Modifica: ModelEntry;
}

export class KendoFabbricatoRow {
    chiave: string;
    Piva: string;
    Sa_Cod: number;
    Fabbricato_Cod: number;
    Fabbricato_des:string;
    Sa_Nome: string;
    Tipo: string;
    Tipo_Fabbricato_Cod: string;

    Data_Creazione: string;
    Data_Modifica: string;
    Utente_Creazione: string;
    Utente_Modifica: string;
}


export class FabbricatoKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}
