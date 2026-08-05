import {KendoGridModel, KendoServerResult, ModelEntry} from 'gias-kendo-grid';

export class PianoColturaleKendoServerResult extends KendoServerResult {
  constructor(public model, public columns, public rows) {
    super(model, columns, rows);
  }
}

export class PianoColturaleKendoModel extends KendoGridModel {
  Piva: ModelEntry;
  Programmazione_Cod: ModelEntry;
  Programmazione_Des: ModelEntry;
  Note: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
}
