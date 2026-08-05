import {KendoGridModel, KendoServerResult, ModelEntry} from 'gias-kendo-grid';

export class GruppiRaccoltaKendoServerResult extends KendoServerResult {
    constructor(public model, public columns, public rows) {
        super(model, columns, rows);
    }
}

export class KendoGruppiRaccoltaModel extends KendoGridModel {
    GruppoRaccolta_Cod: ModelEntry;
    GruppoRaccolta_Des: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    Data_Creazione: ModelEntry;
    Utente_Creazione: ModelEntry;
    Data_Modifica: ModelEntry;
    Utente_Modifica: ModelEntry;
}
