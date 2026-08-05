import {KendoGridModel, KendoServerResult, ModelEntry} from 'gias-kendo-grid';

export class CatastoKendoServerResult extends KendoServerResult {
  constructor(public model, public columns, public rows) {
    super(model, columns, rows);
  }
}

export class CatastoKendoModel extends KendoGridModel {
  Prov: ModelEntry;
  Com: ModelEntry;
  Sezione: ModelEntry;
  Foglio: ModelEntry;
  Numero: ModelEntry;
  Subalterno: ModelEntry;
  Veg_cod: ModelEntry;
  Cul_Cod: ModelEntry;
  id_Cod: ModelEntry;
  Superficie_Ori: ModelEntry;
  Superficie_Act: ModelEntry;
}
