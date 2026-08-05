import { ObjParametriAgenda } from 'gias-ui-kit';
import { JsonKendoResult, KendoGridModel, KendoServerResult, ModelEntry } from 'gias-kendo-grid';

// export class CampiKendoServerResult extends KendoServerResult {

//     constructor(json: JsonKendoResult) {
//         super(json.kendo_model, json.kendo_columns, json.kendo_rows);
//     }
// }

export class CampiKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

export class KendoCampiModel extends KendoGridModel {
    sa_cod: ModelEntry;
    chiave: ModelEntry;
    Campo: ModelEntry;
    Campo_Des: ModelEntry;
    Campo_Cod: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    Gru_Cod: ModelEntry;
    Veg_Cod: ModelEntry;
    Utente_Modifica: ModelEntry;
    Data_Modifica: ModelEntry;
    Utente_Creazione: ModelEntry;
    Data_Creazione: ModelEntry;
    Gru_Des: ModelEntry;
    Veg_Des: ModelEntry;
    sa_nome: ModelEntry;
    PIVA: ModelEntry;
    Superficie_Totale: ModelEntry;
    Superficie_Convenzionale: ModelEntry;
    Superficie_Biologico: ModelEntry;
    Superficie_Conversione: ModelEntry;
    Superficie_Catastale: ModelEntry;
    rif_alfanumerico: ModelEntry;
    sup_contratto: ModelEntry;
    filiera: ModelEntry;
    Attivo: ModelEntry;
}

export class AppezzamentiRequest {
    objP_super_server: string;
    objP_server: string;
    objP_utenti: string;
    InData: ObjParametriAgenda;
}
