import { KendoGridModel, ModelEntry, KendoServerResult } from 'gias-kendo-grid';

export class MacchinaKendoServerResult extends KendoServerResult {
    constructor(public model, public columns, public rows) {
        super(model, columns, rows);
    }
}

export class KendoCostiMacchinaModel extends KendoGridModel {
    ID: ModelEntry;
    Prezzo: ModelEntry;
    Unita_Misura: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
}

export class KendoMacchineModel extends KendoGridModel {
    chiave: ModelEntry;
    Piva: ModelEntry;
    Sa_Cod: ModelEntry;
    Mac_Cod: ModelEntry;
    Visibilita: ModelEntry;
    Cod_Contatto: ModelEntry;
    Contatto_Des: ModelEntry;
    tipologia: ModelEntry;
    CLASS_CODE: ModelEntry;
    Ditta_Des: ModelEntry;
    Ditta_Cod: ModelEntry;
    Modello: ModelEntry;
    Macchina: ModelEntry;
    Telaio: ModelEntry;
    Targa: ModelEntry;
    Codice: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    Data_Creazione: ModelEntry;
    Utente_Creazione: ModelEntry;
    Data_Modifica: ModelEntry;
    Utente_Modifica: ModelEntry;
}
