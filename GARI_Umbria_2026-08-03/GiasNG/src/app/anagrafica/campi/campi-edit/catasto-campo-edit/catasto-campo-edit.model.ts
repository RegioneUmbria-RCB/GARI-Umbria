import {
  KendoGridModel,
  ModelEntry,
  KendoServerResult,
  KendoGridRow,
  KendoGridColumn
} from 'gias-kendo-grid';

export class CatastoCampoKendoServerResult extends KendoServerResult {
  constructor(model: KendoGridModel, cols: KendoGridColumn[], rows: KendoGridRow[]) {
    super(model, cols, rows);
  }
}

export class KendoCatastoCampoModel extends KendoGridModel { // TODO Salvo: da sistemare nomi campi
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
