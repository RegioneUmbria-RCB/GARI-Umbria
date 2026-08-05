import {
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  ModelEntry
} from 'gias-kendo-grid';

export class InvestomentoCatastaleAppezzamentoKendoModel extends KendoGridModel {
  chiave: ModelEntry;
  rag_soc: ModelEntry;
  sa_nome: ModelEntry;
  Piva: ModelEntry;
  SA_COD: ModelEntry;
  PROV: ModelEntry;
  COMUNI_PROV: ModelEntry;
  COM: ModelEntry;
  LOCALITA: ModelEntry;
  SEZIONE: ModelEntry;
  FOGLIO: ModelEntry;
  NUMERO: ModelEntry;
  SUBALTERNO: ModelEntry;
  ZVN: ModelEntry;
  Campo_Des: ModelEntry;
  APP_NOME: ModelEntry;
  SUP_APP: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
  AREA: ModelEntry;
  Utilizzo: ModelEntry;
  Attivo: ModelEntry;
}

export class InvestimentoCatastaleAppezzamentoKendoServerResult extends KendoServerResult {
  constructor(
    public model: KendoGridModel,
    public columns: KendoGridColumn[],
    public rows: KendoGridRow[]
  ) {
    super(model, columns, rows);
  }
}
