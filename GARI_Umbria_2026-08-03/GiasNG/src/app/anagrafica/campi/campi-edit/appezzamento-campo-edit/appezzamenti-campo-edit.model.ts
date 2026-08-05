import { KendoGridModel, ModelEntry, KendoServerResult } from 'gias-kendo-grid';

export class AppezzamentiCampoKendoServerResult extends KendoServerResult {
    constructor(public model, public columns, public rows) {
        super(model, columns, rows);
    }
}

export class KendoAppezzamentiCampoModel extends KendoGridModel {
    ID: ModelEntry;
    id: ModelEntry;
    APPEZZA: ModelEntry;
    APP_NOME: ModelEntry;
    CUL_COD: ModelEntry;
    Campo_Cod: ModelEntry;
    Cul_Des: ModelEntry;
    ID_REG: ModelEntry;
    PIVA: ModelEntry;
    SA_COD: ModelEntry;
    SUP_APP: ModelEntry;
    Veg_Des: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    Impianto_Validita_Inizio: ModelEntry;
    Impianto_Validita_Fine: ModelEntry;
}
