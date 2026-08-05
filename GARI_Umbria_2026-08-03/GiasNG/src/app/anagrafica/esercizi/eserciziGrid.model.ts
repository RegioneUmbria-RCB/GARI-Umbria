import { KendoGridModel, ModelEntry, KendoServerResult } from 'gias-kendo-grid';

export class KendoEserciziModel extends KendoGridModel {
    chiave: ModelEntry;
    PIVA: ModelEntry;
    SA_COD: ModelEntry;
    sa_nome: ModelEntry;
    APPEZZA: ModelEntry;
    id_Reg: ModelEntry;
    app_nome: ModelEntry;
    veg_cod: ModelEntry;
    veg_des: ModelEntry;
    // cul_cod: ModelEntry;
    // cul_des: ModelEntry;
    // GRFI_COD: ModelEntry;
    // Grfi_Des: ModelEntry;
    // //GRVA_Cod_VEG: ModelEntry;
    // GRVA_Cod_VEG: ModelEntry;
    // grva_des: ModelEntry;
    // sup_imp: ModelEntry;
    // Validita_Inizio: ModelEntry;
    // Validita_Fine: ModelEntry;
    // Data_Modifica: ModelEntry;
    // utente_modifica: ModelEntry;
    // Data_Creazione: ModelEntry;
    // utente_creazione: ModelEntry;
    // //Campo_Cod: ModelEntry;
    // Campo_Cod: ModelEntry;
    // Campo_Des: ModelEntry;
    // Codice_Impianto: ModelEntry;
    // tra_fila_m: ModelEntry;
    // su_fila_m: ModelEntry;
    // //port_cod: ModelEntry;
    // port_cod: ModelEntry;
    // port_des: ModelEntry;
    // //foral_cod: ModelEntry;
    // foral_cod: ModelEntry;
    // foral_des: ModelEntry;
    // Setup_Cod: ModelEntry;
    // //cop_cod: ModelEntry;
    // cop_cod: ModelEntry;
    // Cop_Des: ModelEntry;
    // COVER: ModelEntry;
    // MONITORATO: ModelEntry;
    // Blk_Flag: ModelEntry;
    // Data_Inizio_Portinnesto: ModelEntry;
    // Progetto_Cod: ModelEntry;
    // Progetto_Nome: ModelEntry;
    // Progetto_Des: ModelEntry;
    // Resa: ModelEntry;
    // cod_kpin: ModelEntry;
    // cod_block: ModelEntry;
    // cod_grower: ModelEntry;
    // Distinta_Chiusa: ModelEntry;
    // Utilizzo: ModelEntry;
    // Destinazione_Uso_Cod: ModelEntry;
    // Destinazione_Uso_Des: ModelEntry;
    // Attivo: ModelEntry;
    // Bloccato: ModelEntry;
    // // rif_alfanumerico: ModelEntry;
    // // cod_biologico: ModelEntry;
    // // MetodoProduzione_Cod: ModelEntry;
    // // MetodoProduzione_Des: ModelEntry;
}

export class CatastoKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}
