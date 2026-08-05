import {
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  ModelEntry
} from 'gias-kendo-grid';

export class GisCfgProiezioniConfigServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GisCfgProiezioniConfigModel extends KendoGridModel {
  key: ModelEntry;
  value: ModelEntry;
}
