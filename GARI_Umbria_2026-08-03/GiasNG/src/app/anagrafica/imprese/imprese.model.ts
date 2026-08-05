import { Impresa } from 'app/Model/anagrafiche/Impresa';
import { ModelEntry, KendoGridModel, KendoServerResult, KendoGridColumn,
    KendoGridRow, JsonKendoResult } from 'gias-kendo-grid';
import { KendoCentroModel } from '../centri/centri.models';

export class ImpresaModel extends KendoGridModel {
  Rag_Soc: ModelEntry;
  Piva: ModelEntry;
  partitaIvaReale: ModelEntry;
  Codice_Cuaa: ModelEntry;
  Codice_Socio: ModelEntry;
  Piva_Padre: ModelEntry;
  codice_iscrizione_libro_soci: ModelEntry;
  data_iscrizione_libro_soci: ModelEntry;
  Stato_Cod: ModelEntry;
  Pro_Cod_Istat: ModelEntry;
  Com_Cod_Istat: ModelEntry;
  frz_des: ModelEntry;
  ind_des: ModelEntry;
  Cap: ModelEntry;
  Rapporti_Codice: ModelEntry;
  Rag_Soc_Proprietario: ModelEntry;
  GruppoRaccolta_Cod: ModelEntry;
  Superficie_Catastale: ModelEntry;
  Superficie_Convenzionale: ModelEntry;
  Superficie_Conversione: ModelEntry;
  Superficie_Biologico: ModelEntry;
  Superficie_Totale: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
  Data_Creazione: ModelEntry;
  Utente_Creazione: ModelEntry;
  Data_Modifica: ModelEntry;
  Utente_Modifica: ModelEntry;
  Attivo: ModelEntry;
  IndirizzoCompleto: ModelEntry;
  TipoDes: ModelEntry;
}


export class KendoImpresaRow {
    chiave: string;
    Rag_Soc: string;
    Piva: string;
    partitaIvaReale: string;
    Codice_Fiscale: string;
    Codice_Cuaa: string;
    Codice_Socio: string;
    Contratto_Produzione: string;
    Indirizzo: string;
    Tecnico_Referente: string;
    Cooperativa_Referente: string;
    Superficie_Totale: number;
    Superficie_Tare: number;
    SAU_Totale: number;

    Pro_Cod_Istat: string;
    Prov: string;
    Com_Cod_Istat: string;
    Com: string;
    Stato: string;
    Stato_Cod: string;
    frz_des: string;
    ind_des: string;

    Piva_Padre: string;
    Rag_Soc_Padre: string;
    Num_Padri: string;

    Validita_Inizio: Date;
    Validita_Fine: Date;
    Data_Creazione: Date;
    Data_Modifica: Date;
    Utente_Creazione: string;
    Utente_Modifica: string;
    Rapporti_Des: string;
    Piva_Proprietario: string;
    Rag_Soc_Proprietario: string;
    TipoDes: string;
}


export class ImpresaKendoServerResult extends KendoServerResult {
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}
