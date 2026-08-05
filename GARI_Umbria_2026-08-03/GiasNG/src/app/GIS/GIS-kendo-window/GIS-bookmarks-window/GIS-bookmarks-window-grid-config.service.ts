import { Injectable, Injector } from '@angular/core';
import { catchError, map, Observable, of, tap } from 'rxjs';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { CommandsColumnSettings, CustomColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { ConfigTemplate } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { GISBookmarksWindowService } from './GIS-bookmarks-window.service';
import { Bookmark } from 'app/Service/api.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { FunzioniComuniService } from 'app/Service/FunzioniComuni.service';

export class GISBookmarksWindowResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class GISBookmarksWindowGridModel extends KendoGridModel {
  Bookmark_Cod: ModelEntry;
  Bookmark_Des: ModelEntry;
  Center_Lat: ModelEntry;
  Center_Lng: ModelEntry;
  Zoom: ModelEntry;
}

@Injectable()
export class GISBookmarkWindowGridConfigService extends AbstractGridConfigService<GISBookmarksWindowResult> {
  gridId = 'GISBookmarksWindowGrid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'Bookmark_Cod';

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Bookmark_Cod', title: '' },
      { hidden: true }
    ),
    new KendoGridColumn(
      { field: 'Bookmark_Des', title: this.transloco.translate('Nome') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 150,
      },
    ),
    new KendoGridColumn(
      { field: 'Center_Lat', title: 'Lat' },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 75
      },
    ),
    new KendoGridColumn(
      { field: 'Center_Lng', title: 'Lng' },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 75
      },
    ),
    new KendoGridColumn(
      { field: 'Zoom', title: 'Zoom' },
      {
        resizable: true,
        filterable: true,
        editable: false,
        width: 75
      }
    )
  ];

  GISBookmarksWindowGridModel: GISBookmarksWindowGridModel = {
    Bookmark_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Bookmark_Des: new ModelEntry(CELL_TYPES.STRING),
    Center_Lat: new ModelEntry(CELL_TYPES.NUMBER),
    Center_Lng: new ModelEntry(CELL_TYPES.NUMBER),
    Zoom: new ModelEntry(CELL_TYPES.NUMBER),
  };

  constructor(
    protected injector: Injector,
    protected transloco: TranslocoService,
    private gisBookmarksWindowService: GISBookmarksWindowService,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings(true);
    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: true,
      onDisableRemoveBtn: (row: Bookmark) => row.Posizioni_Speciali != 0,
    });

    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = false;
    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.customColumn = new CustomColumnSettings({showColumn: true,useCustomColumnCellTemplate: true});
  }

  public read(): Observable<GISBookmarksWindowResult> {
    this.isLoading(true);
    return this.gisBookmarksWindowService.readBookmarks()
      .pipe(
        map(data => new GISBookmarksWindowResult(this.roundLatLng(data), this.columns, this.GISBookmarksWindowGridModel)),
        tap(() => this.isLoading(false))
      );
  }

  public perform(actionType: HttpAction, bookmark: Bookmark): Observable<KendoGridRow[]> {
    if (actionType == HttpAction.CREATE) {
      bookmark.Posizioni_Speciali = 0;
      return this.gisBookmarksWindowService
        .insertBookmark(bookmark)
        .pipe(
          catchError(err => {
            this.giasMessageService.errorMessage(FunzioniComuniService.getResponseError(err, this.transloco), false);
            return of(null);
          }),
          map(res => {
            if (res != null) {
              this.giasMessageService.successMessage("gis.PreferitoAggiuntoCorrettamente", false, true);
            }

            return [bookmark];
          })
        );
    } else if (actionType == HttpAction.REMOVE) {
      return this.gisBookmarksWindowService
        .deleteBookmark(bookmark.Bookmark_Cod)
        .pipe(
          catchError(err => {
            this.giasMessageService.errorMessage(FunzioniComuniService.getResponseError(err, this.transloco), false);
            return of(null);
          }),
          map(res => {
            if (res != null) {
              this.giasMessageService.successMessage("gis.PreferitoEliminatoCorrettamente", false, true);
            }

            return [bookmark];
          })
        );
    }

    return of([bookmark]);
  }

  private roundLatLng(bookmarks: Bookmark[]) {
    return bookmarks.map(x => ({ ...x, Center_Lat: roundTwoDecimals(x.Center_Lat), Center_Lng: roundTwoDecimals(x.Center_Lng) }));
  }
}

function roundTwoDecimals(num: number): number {
  return Math.round(num * 100) / 100;
}
