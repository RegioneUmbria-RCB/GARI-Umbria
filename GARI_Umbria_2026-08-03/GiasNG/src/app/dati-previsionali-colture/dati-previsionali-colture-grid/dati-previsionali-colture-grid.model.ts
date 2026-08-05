import {KendoGridModel, KendoServerResult, ModelEntry} from 'gias-kendo-grid';

export class DatiPrevisionaliColtureKendoServerResult extends KendoServerResult {
  constructor(public model, public columns, public rows) {
    super(model, columns, rows);
  }
}

export class KendoDatiPrevisionaliColtureModel extends KendoGridModel {
  Piva: ModelEntry;
  Id: ModelEntry;
  Veg_Cod: ModelEntry;
  veg_des: ModelEntry;
  Cul_Cod: ModelEntry;
  cul_des: ModelEntry;
  Codice: ModelEntry;
  parametro_des: ModelEntry;
  Valore: ModelEntry;
  grfi_cod: ModelEntry;
  grfi_des: ModelEntry;
  grva_cod: ModelEntry;
  grva_des: ModelEntry;
  dettSpeciePersonalizzatoCod: ModelEntry;
  dettSpeciePersonalizzatoDes: ModelEntry;
  stato_cod: ModelEntry;
  stato_des: ModelEntry;
  codice_stato: ModelEntry;
  codice_stato_des: ModelEntry;
  reg: ModelEntry;
  Regione_Des: ModelEntry;
  prov: ModelEntry;
  PROVINCIA: ModelEntry;
  port_cod: ModelEntry;
  port_des: ModelEntry;
  foral_cod: ModelEntry;
  foral_des: ModelEntry;
  reg_cod: ModelEntry;
  regolamento_des: ModelEntry;
  udm_cod: ModelEntry;
  udm_des: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
}

