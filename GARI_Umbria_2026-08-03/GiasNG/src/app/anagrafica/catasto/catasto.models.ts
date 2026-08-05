import { KendoGridModel, ModelEntry, KendoServerResult } from 'gias-kendo-grid';

export class KendoCatastoModel extends KendoGridModel {
    Codice: ModelEntry;
    chiave: ModelEntry;
    Sa_Nome: ModelEntry;
    Prov: ModelEntry;
    PROVINCIA: ModelEntry;
    Com: ModelEntry;
    COMUNE: ModelEntry;
    SEZIONE: ModelEntry;
    FOGLIO: ModelEntry;
    NUMERO: ModelEntry;
    SUBALTERNO: ModelEntry;
    Sup_Catastale: ModelEntry;
    Sup_Condotta: ModelEntry;
    Titolo_Possesso_Cod: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    Data_Creazione: ModelEntry;
    Data_Modifica: ModelEntry;
    Utente_Creazione: ModelEntry;
    Utente_Modifica: ModelEntry;
    Attivo: ModelEntry
}

export class KendoCatastoRow {
    chiave: string;
    Prov: string;
    PROVINCIA: string;
    Com: string;
    COMUNE: string;
    Sezione: string;
    FOGLIO: number;
    NUMERO: number;
    Subalterno: string;
    Sup_Catastale: number;
    Sup_Condotta: number;
    Titolo_Possesso: string;
    Validita_Inizio: Date;
    Validita_Fine: Date;
    Data_Creazione: Date;
    Data_Modifica: Date;
    Utente_Creazione: string;
    Utente_Modifica: string;
}

export class CatastoKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}
