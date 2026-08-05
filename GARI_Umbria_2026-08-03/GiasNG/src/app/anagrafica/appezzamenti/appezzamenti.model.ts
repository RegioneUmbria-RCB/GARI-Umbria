import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ModelEntry, KendoGridModel, KendoServerResult, KendoGridColumn,
    KendoGridRow, JsonKendoResult } from 'gias-kendo-grid';
import { KendoCentroModel } from '../centri/centri.models';

export class AppezzamentoModel extends KendoGridModel {
    sa_nome:ModelEntry;
    Campo_Des:ModelEntry;
    APP_NOME:ModelEntry;
    utilizzo:ModelEntry;
    rif_alfanumerico:ModelEntry;
    cod_biologico:ModelEntry;
    cod_kpin:ModelEntry;
    cod_block:ModelEntry;
    SUP_APP:ModelEntry;
    Validita_Inizio:ModelEntry;
    Validita_Fine:ModelEntry;
    Data_Modifica:ModelEntry;
    utente_modifica:ModelEntry;
    Data_Creazione:ModelEntry;
    utente_creazione:ModelEntry;
    isola:ModelEntry;
    MetodoProduzione_Cod:ModelEntry;
    MetodoProduzione_Des:ModelEntry;
    Blk_Flag:ModelEntry;
    Blk_Flag_Des:ModelEntry
}

export class AppezzamentiServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class IndirizziAppezzamentoModel extends KendoGridModel {
    codice: ModelEntry;
    cap: ModelEntry;
    frazione: ModelEntry;
    prov: ModelEntry;
    com: ModelEntry;
    note: ModelEntry;
    codice_stato: ModelEntry;
    descrizione_stato: ModelEntry;
    via: ModelEntry;
    tipo_indirizzo: ModelEntry;
}

export class IndirizziServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}
