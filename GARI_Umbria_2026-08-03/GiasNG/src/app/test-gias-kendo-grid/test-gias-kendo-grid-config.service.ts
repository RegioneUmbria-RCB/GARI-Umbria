import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { MisuraPerAvversitaAnagrafica } from "app/Service/api.service";
import {  ConfigTemplate, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { Observable, of } from "rxjs";


export class TestGiasKendoGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class TestGiasKendoGridModel extends KendoGridModel {
  first: ModelEntry;
  second: ModelEntry;
  third: ModelEntry;
  fourth: ModelEntry;
}


@Injectable()
export class TestGiasKendoGridConfigService extends AbstractGridConfigService<TestGiasKendoGridResult> {
  gridId = 'GISCfgProiezioniGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_PAGE;
  rowId = 'first';

  gridColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'first', title: 'first' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'second', title: 'second', },
      { resizable: true, filterable: true, editable: false, width: 200 },
    ),
    new KendoGridColumn(
      { field: 'third', title: 'third' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'fourth', title: 'fourth', },
      { resizable: true, filterable: true, editable: false, width: 200 },
    ),
  ];

  gridModel: TestGiasKendoGridModel = {
    first: new ModelEntry(CELL_TYPES.NUMBER),
    second: new ModelEntry(CELL_TYPES.STRING),
    third: new ModelEntry(CELL_TYPES.STRING),
    fourth: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

  }

  public read(): Observable<TestGiasKendoGridResult> {
    return of({
      rows: [
        { first: 1, second: 'second 1', third: 'third 1', fourth: 'fourth 1' },
        { first: 2, second: 'second 2', third: 'third 2', fourth: 'fourth 2' },
        { first: 3, second: 'second 3', third: 'third 3', fourth: 'fourth 3' },
        { first: 4, second: 'second 4', third: 'third 4', fourth: 'fourth 4' }
      ],
      columns: this.gridColumns,
      model: this.gridModel
    } as TestGiasKendoGridResult)
  }

  public perform(actionType: HttpAction, rows: Array<MisuraPerAvversitaAnagrafica>): Observable<KendoGridRow[]> {
    return of(rows);
  }
}

export enum EnumCfgProiezioniActions {
  EDIT,
  PERMISSIONS,
  ACTIVATION,
  CONFIGURATION
}
