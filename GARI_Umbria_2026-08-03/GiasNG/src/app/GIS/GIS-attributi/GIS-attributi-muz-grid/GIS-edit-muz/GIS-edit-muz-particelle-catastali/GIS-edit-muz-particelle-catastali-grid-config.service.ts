import { Injectable, Injector } from '@angular/core';
import { Observable, filter, map, of, startWith, tap, withLatestFrom } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import {
  CommandsColumnSettings,
  CustomColumnSettings,
  ResizableSettings,
  ToolbarSettings
} from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { MisuraPerAvversitaAnagrafica, ParticelleCatastali_MUZ } from 'app/Service/api.service';
import { GISAttributiMuzService } from '../../GIS-attributi-muz.service';

export class GISEditMuzParticelleCatastaliGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISParticelleCatastaliMuzGridModel extends KendoGridModel {
  rowId: ModelEntry;
  Provincia: ModelEntry;
  Comune: ModelEntry;
  Provincia_Esteso: ModelEntry;
  Comune_Esteso: ModelEntry;
  Sezione: ModelEntry;
  Foglio: ModelEntry;
  Numero: ModelEntry;
  Subalterno: ModelEntry;
  Titolo_Possesso: ModelEntry;
  Sup_Condotta: ModelEntry;
  Condotta_Validita_Inizio: ModelEntry;
  Condotta_Validita_Fine: ModelEntry;
}

@Injectable()
export class GISEditMuzParticelleCatastaliGridConfigService extends AbstractGridConfigService<GISEditMuzParticelleCatastaliGridResult> {
  gridId = 'GISEditMuzParticelleCatastaliGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_PAGE;
  rowId = 'rowId';

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'rowId', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'Provincia', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'Comune', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'Provincia_Esteso', title: this.transloco.translate('Provincia') },
      {
        resizable: true,
        filterable: true,
        editable: false,
      },
    ),
    new KendoGridColumn(
      { field: 'Comune_Esteso', title: this.transloco.translate('Comune') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'Sezione', title: this.transloco.translate('Sezione') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'Foglio', title: this.transloco.translate('Foglio') },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 75
      },
    ),
    new KendoGridColumn(
      { field: 'Numero', title: this.transloco.translate('Numero') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'Subalterno', title: this.transloco.translate('Subalterno') },
      {
        resizable: true,
        filterable: true,
        editable: false
      },
    ),
    new KendoGridColumn(
      { field: 'Titolo_Possesso', title: this.transloco.translate('TitoloDiPossesso') },
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
      { field: 'Condotta_Validita_Inizio', title: this.transloco.translate('Validita_Inizio') },
      {
        resizable: true,
        filterable: true,
        editable: false
      }
    ),
    new KendoGridColumn(
      { field: 'Condotta_Validita_Fine', title: this.transloco.translate('Validita_Fine') },
      {
        resizable: true,
        filterable: true,
        editable: false
      }
    )
  ];

  model: GISParticelleCatastaliMuzGridModel = {
    rowId: new ModelEntry(CELL_TYPES.STRING),
    Provincia: new ModelEntry(CELL_TYPES.STRING),
    Comune: new ModelEntry(CELL_TYPES.STRING),
    Provincia_Esteso: new ModelEntry(CELL_TYPES.STRING),
    Comune_Esteso: new ModelEntry(CELL_TYPES.STRING),
    Sezione: new ModelEntry(CELL_TYPES.STRING),
    Foglio: new ModelEntry(CELL_TYPES.NUMBER),
    Numero: new ModelEntry(CELL_TYPES.NUMBER),
    Subalterno: new ModelEntry(CELL_TYPES.STRING),
    Titolo_Possesso: new ModelEntry(CELL_TYPES.STRING),
    Sup_Condotta: new ModelEntry(CELL_TYPES.NUMBER),
    Condotta_Validita_Inizio: new ModelEntry(CELL_TYPES.DATE),
    Condotta_Validita_Fine: new ModelEntry(CELL_TYPES.DATE),
  };

  constructor(
    protected injector: Injector,
    private gisAttributiMuzService: GISAttributiMuzService
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

    this.customColumn = new CustomColumnSettings({showColumn: true,width: 50,useCustomColumnCellTemplate: true});
  }

  public read(): Observable<GISEditMuzParticelleCatastaliGridResult> {
    return of(true)
      .pipe(
        withLatestFrom(this.gisAttributiMuzService.muzToEdit$),
        map(([_, muz]) => muz),
        filter(x => x != null),
        tap(() => this.isLoading(true)),
        map(muz => new GISEditMuzParticelleCatastaliGridResult(muz.Particelle_Catastali, this.columns, this.model)),
        tap(() => this.isLoading(false))
      )
  }

  public perform(_: HttpAction, rows: Array<MisuraPerAvversitaAnagrafica>): Observable<KendoGridRow[]> {
    return of(rows);
  }

  public static getGisParticelleCatastaliMuzRowId(row: ParticelleCatastali_MUZ): string {
    return `${row.Provincia}-${row.Comune}-${row.Sezione}-${row.Foglio}-${row.Numero}-${row.Subalterno}`;
  }
}
