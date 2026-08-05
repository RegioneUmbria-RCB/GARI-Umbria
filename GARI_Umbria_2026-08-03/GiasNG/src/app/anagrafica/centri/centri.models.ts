import { Provincia, Stato } from 'app/Model/MetaschemaModel';
import { RispostaStandard } from 'app/Service/master.service';
import { KendoGridColumn, KendoGridModel, ModelEntry, KendoServerResult } from 'gias-kendo-grid';

export class KendoCentroModel extends KendoGridModel {
  chiave: ModelEntry;
  sa_cod: ModelEntry;
  sa_nome: ModelEntry;
  Piva: ModelEntry;
  Rag_Soc: ModelEntry;
  cod_indirizzo: ModelEntry;
  ind_des: ModelEntry;
  frz_des: ModelEntry;
  CAP: ModelEntry;
  Stato_Cod: ModelEntry;
  Stato2: ModelEntry;
  note: ModelEntry;
  com_des: ModelEntry;
  pro_cod: ModelEntry;
  pro_cod_istat: ModelEntry;
  com_cod_istat: ModelEntry;
  CodiceOperatoreBio: ModelEntry;
  tipoAttivitaCod: ModelEntry;
  Superficie_Catastale: ModelEntry;
  Superficie_Convenzionale: ModelEntry;
  Superficie_Conversione: ModelEntry;
  Superficie_Biologico: ModelEntry;
  Superficie_Totale: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
  Data_Creazione: ModelEntry;
  Data_Modifica: ModelEntry;
  Utente_Creazione: ModelEntry;
  Utente_Modifica: ModelEntry;
}

export class KendoCentroRow {
  CAP: string;
  chiave: string;
  com_des: string;
  Data_Creazione: Date;
  Data_Modifica: Date;
  ind_des: string;
  piva: string;
  pro_cod: string;
  rag_soc: string;
  sa_nome: string;
  SAU_Totale: number;
  Stato: string;
  Superficie_Tare: number;
  Superficie_Totale: number;
  Utente_Creazione: string;
  Utente_Modifica: string;
  Validita_Fine: Date;
}

export class CentriWrapper {
  centri: any[];
  prov: Provincia[];
  stati: Stato[];

  constructor(opts: Required<CentriWrapper>) {
    this.centri = opts.centri;
    this.prov = opts.prov;
    this.stati = opts.stati;
  }
}

export class CentroKendoServerResult extends KendoServerResult {
  constructor(public model: KendoCentroModel,
              public columns: KendoGridColumn[],
              public rows: KendoCentroRow[]) {
    super(model, columns, rows);
  }
}

export class CentriResolverResult {
}
