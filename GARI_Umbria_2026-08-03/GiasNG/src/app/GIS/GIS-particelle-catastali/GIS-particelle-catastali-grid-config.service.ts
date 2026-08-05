import { Injectable, Injector } from '@angular/core';
import { Observable, filter, finalize, map, of, switchMap, tap, withLatestFrom } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { MisuraPerAvversitaAnagrafica } from 'app/Service/api.service';
import { GISParticelleCatastaliService } from './GIS-particelle-catastali.service';
import { MasterService } from 'app/Service/master.service';

export class GISParticelleCatastaliGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISParticelleCatastaliGridModel extends KendoGridModel {
  rowId: ModelEntry;
  Prov: ModelEntry;
  com: ModelEntry;
  PROVINCIA: ModelEntry;
  COMUNE: ModelEntry;
  SEZIONE: ModelEntry;
  FOGLIO: ModelEntry;
  NUMERO: ModelEntry;
  SUBALTERNO: ModelEntry;
  Titolo_possesso: ModelEntry;
  Sup_Condotta: ModelEntry;
  Validita_Inizio: ModelEntry;
  Validita_Fine: ModelEntry;
}

@Injectable()
export class GISParticelleCatastaliGridConfigService extends AbstractGridConfigService<GISParticelleCatastaliGridResult> {
  gridId = 'GISParticelleCatastaliGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_PAGE;
  rowId = 'rowId';

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'rowId', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'Prov', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'com', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'PROVINCIA', title: this.transloco.translate('Provincia') },
      {
        resizable: true,
        filterable: true,
        editable: false,
      },
    ),
    new KendoGridColumn(
      { field: 'COMUNE', title: this.transloco.translate('Comune') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'SEZIONE', title: this.transloco.translate('Sezione') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'FOGLIO', title: this.transloco.translate('Foglio') },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 50
      },
    ),
    new KendoGridColumn(
      { field: 'NUMERO', title: this.transloco.translate('Numero') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'SUBALTERNO', title: this.transloco.translate('Subalterno') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'Titolo_possesso', title: this.transloco.translate('TitoloDiPossesso') },
      {
        resizable: true,
        filterable: true,
        editable: false
      }
    ),
    new KendoGridColumn(
      { field: 'Sup_Condotta', title: this.transloco.translate('SuperficieCondottaAbbr') },
      {
        resizable: true,
        filterable: true,
        editable: false
      }
    ),
    new KendoGridColumn(
      { field: 'Validita_Inizio', title: this.transloco.translate('Validita_Inizio') },
      {
        resizable: true,
        filterable: true,
        editable: false
      }
    ),
    new KendoGridColumn(
      { field: 'Validita_Fine', title: this.transloco.translate('Validita_Fine') },
      {
        resizable: true,
        filterable: true,
        editable: false
      }
    )
  ];

  model: GISParticelleCatastaliGridModel = {
    rowId: new ModelEntry(CELL_TYPES.STRING),
    Prov: new ModelEntry(CELL_TYPES.STRING),
    com: new ModelEntry(CELL_TYPES.STRING),
    PROVINCIA: new ModelEntry(CELL_TYPES.STRING),
    COMUNE: new ModelEntry(CELL_TYPES.STRING),
    SEZIONE: new ModelEntry(CELL_TYPES.STRING),
    FOGLIO: new ModelEntry(CELL_TYPES.NUMBER),
    NUMERO: new ModelEntry(CELL_TYPES.NUMBER),
    SUBALTERNO: new ModelEntry(CELL_TYPES.STRING),
    Titolo_possesso: new ModelEntry(CELL_TYPES.STRING),
    Sup_Condotta: new ModelEntry(CELL_TYPES.NUMBER),
    Validita_Inizio: new ModelEntry(CELL_TYPES.STRING),
    Validita_Fine: new ModelEntry(CELL_TYPES.STRING),
  };

  constructor(
    protected injector: Injector,
    private gisParticelleCatastaliService: GISParticelleCatastaliService,
    private masterService: MasterService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings();
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false
    });

    this.behavior.excelSettings.enabled = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.toolbar.customToolbar = true;
    this.resizable.autoFitColumns = true;
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.selectable.enabled = true;
    this.selectable.columnSettings.showSelectAll = true;
    this.selectable.shouldShowCheckbox = true;
    this.selectable.columnSettings.title = '';
    this.pagination.gridState.take = 10;
  }

  public read(): Observable<GISParticelleCatastaliGridResult> {
    return of(true)
      .pipe(
        tap(() => this.masterService.set_isLoading({ isLoading: true })),
        withLatestFrom(this.gisParticelleCatastaliService.piva$.pipe(filter(x => x != null)), this.gisParticelleCatastaliService.existing$),
        switchMap(([_, piva, existings]) => this.gisParticelleCatastaliService.read(piva, existings)),
        map(rows => new GISParticelleCatastaliGridResult(rows, this.columns, this.model)),
        finalize(() => this.masterService.set_isLoading({ isLoading: false }))
      )
  }

  public perform(_: HttpAction, rows: Array<MisuraPerAvversitaAnagrafica>): Observable<KendoGridRow[]> {
    return of(rows);
  }
}