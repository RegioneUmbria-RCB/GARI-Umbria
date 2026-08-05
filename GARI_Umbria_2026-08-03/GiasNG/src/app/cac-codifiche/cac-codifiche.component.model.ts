import { KendoGridModel, KendoServerResult, ModelEntry } from "gias-kendo-grid";

export interface CacCodificaModel {
    ID: string;
    Sistema_Cod?: number;
    Codice_Esterno?: string;
    Descrizione_Esterno?: string;
    Tabella_Gias?: string;
    Codice_Gias?: string;
    Data_Creazione?: Date;
    Data_Modifica?: Date;
    Username_Creazione?: string;
    Username_Modifica?: string;
    Validita_Inizio?: Date;
    Validita_Fine?: Date;
    Sistema?: string;	
  }

export enum CacStatusCodMapping {
    Gias = -1,
    Enogis = 1,
    Artea = 2,
    Smarttractors = 3,
    Demetra = 4,
    Agea = 5,
    CAI = 6,
}

export class CacConvKendoGridModel extends KendoGridModel {
    chiave: ModelEntry;
    ID: ModelEntry;
    Sistema_Cod: ModelEntry;
    Codice_Esterno: ModelEntry;
    Descrizione_Esterno: ModelEntry;
    Tabella_Gias: ModelEntry;
    Codice_Gias: ModelEntry;
    Data_Creazione: ModelEntry;
    Data_Modifica: ModelEntry;
    Username_Creazione: ModelEntry;
    Username_Modifica: ModelEntry;
    Validita_Inizio: ModelEntry;
    Validita_Fine: ModelEntry;
    Sistema: ModelEntry
}

export class CodificheKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}