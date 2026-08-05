import { CELL_TYPES } from "gias-ui-kit";
import { Injectable } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { KendoGridColumn, ModelEntry } from "gias-kendo-grid";
import { Validators } from "@angular/forms";

@Injectable()
export class DialogGridProdsSommConfigService {

  constructor(
    public translocoService: TranslocoService
  ) {}

  kendoColums: KendoGridColumn[] = [
    new KendoGridColumn(
      {
        field: 'Fabbricato_Des',
        title: this.translocoService.translate('Magazzino')},
        {
          resizable: true,
          filterable: false,
          editable: false,
          width: 150
        }
    ),
    new KendoGridColumn(
      {
        field: 'Descrizione',
        title: this.translocoService.translate('Descrizione')},
        {
          resizable: true,
          editable: false,
          width: 150
        }
    ),
    new KendoGridColumn(
      {
        field: 'CodiceAIC',
        title: this.translocoService.translate('CodiceAIC')},
        {
          resizable: true,
          editable: false,
          width: 80
        }
    ),
    new KendoGridColumn(
      {
        field: 'Lotto',
        title: this.translocoService.translate('Lotto2')},
        {
          resizable:true,
          editable: false,
          width: 135
        }
    ),
    new KendoGridColumn(
      {
        field: 'UdM',
        title: this.translocoService.translate('RisorsaUDM')},
        {
          resizable: true,
          editable: false,
          width: 50
        }
    ),
    new KendoGridColumn(
      {
        field: 'Qta',
        title: this.translocoService.translate('QtaGiacenza') },
        {
          resizable: true,
          editable: false,
          width: 50
        }
    ),
    new KendoGridColumn(
      {
        field: 'QtaToUse',
        title: this.translocoService.translate('QtaToUse') },
        {
          resizable: true,
          editable: true,
          width: 50,
          numeric: { min: 0 }
        }
    ),
  ];

  kendoModel = {
    //Codice: new ModelEntry(CELL_TYPES.NUMBER, false),
    Piva: new ModelEntry(CELL_TYPES.NUMBER, false),
    Sa_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Fabbricato_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
    Fabbricato_Des: new ModelEntry(CELL_TYPES.STRING, false),
    Descrizione: new ModelEntry(CELL_TYPES.STRING, false),
    Lotto: new ModelEntry(CELL_TYPES.STRING, false),
    CodiceAIC: new ModelEntry(CELL_TYPES.STRING, false),
    UdM: new ModelEntry(CELL_TYPES.STRING, false),
    Qta: new ModelEntry(CELL_TYPES.NUMBER, false),
    QtaToUse: new ModelEntry(
      CELL_TYPES.NUMBER,
      true,
      null,
      [Validators.min(0)]
    ),
  };

}