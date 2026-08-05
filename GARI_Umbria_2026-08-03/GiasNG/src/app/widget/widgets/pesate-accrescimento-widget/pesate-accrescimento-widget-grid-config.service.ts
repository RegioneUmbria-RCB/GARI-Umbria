import { Injectable, Injector } from '@angular/core';
import { Observable, finalize, of } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { CookieService } from 'ngx-cookie-service';
import { TranslocoService } from '@jsverse/transloco';

import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  ConfigTemplate,
  EditingMode,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  ResizableSettings,
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';

import { PesateAccrescimentoService } from '../../../zoo/services/pesate-accrescimento.service';
import {
  PesataAggregataRow,
  PesateAggregatedFilters,
  PesateWidgetCookie,
} from '../../../zoo/models/pesate-accrescimento.model';

const PESATE_WIDGET_COOKIE_NAME = 'PesateAccrescimentoWidgetCookie';

export class PesateWidgetGridResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class PesateAccrescimentoWidgetGridConfig extends AbstractGridConfigService<PesateWidgetGridResult> {
  gridId = 'PesateAccrescimentoWidgetGrid_v2';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_CELL;
  rowId = 'gruppo_key';

  param: PesateAggregatedFilters | null = null;
  chartRows: PesataAggregataRow[] = [];

  readonly gridModel: KendoGridModel = {
    gruppo_key:            new ModelEntry(CELL_TYPES.STRING),
    gruppo_des:            new ModelEntry(CELL_TYPES.STRING),
    numero_animali:        new ModelEntry(CELL_TYPES.NUMBER),
    numero_pesate:         new ModelEntry(CELL_TYPES.NUMBER),
    scostamento_medio_pct: new ModelEntry(CELL_TYPES.NUMBER),
    scostamento_max_pct:   new ModelEntry(CELL_TYPES.NUMBER),
    scostamento_min_pct:   new ModelEntry(CELL_TYPES.NUMBER),
    alert_count:           new ModelEntry(CELL_TYPES.NUMBER),
  };

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'gruppo_des', title: this.transloco.translate('widget_pesate_gruppo_des') },
      { resizable: true, filterable: true, editable: false, width: 200 }
    ),
    new KendoGridColumn(
      { field: 'numero_animali', title: this.transloco.translate('widget_pesate_numero_animali') },
      { resizable: true, filterable: false, editable: false, width: 80 }
    ),
    new KendoGridColumn(
      { field: 'numero_pesate', title: this.transloco.translate('widget_pesate_numero_pesate') },
      { resizable: true, filterable: false, editable: false, width: 80 }
    ),
    new KendoGridColumn(
      { field: 'scostamento_medio_pct', title: this.transloco.translate('widget_pesate_scostamento_medio') },
      { resizable: true, filterable: false, editable: false, width: 110 }
    ),
    new KendoGridColumn(
      { field: 'scostamento_max_pct', title: this.transloco.translate('widget_pesate_scostamento_max') },
      { resizable: true, filterable: false, editable: false, width: 110 }
    ),
    new KendoGridColumn(
      { field: 'scostamento_min_pct', title: this.transloco.translate('widget_pesate_scostamento_min') },
      { resizable: true, filterable: false, editable: false, width: 110 }
    ),
    new KendoGridColumn(
      { field: 'alert_count', title: this.transloco.translate('widget_pesate_alert_count') },
      { resizable: true, filterable: false, editable: false, width: 70 }
    ),
  ];

  constructor(
    private pesateService: PesateAccrescimentoService,
    protected override injector: Injector,
    protected transloco: TranslocoService,
    private cookies: CookieService,
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      infoBtn: false,
      removeBtn: false,
    });

    this.behavior.saveExternalChanges = true;
    this.behavior.excelSettings.enabled = false;
    this.columnMenu.kendoGridColumnChooser = true;
    this.views.enabled = true;
    this.groups.groupable.enabled = false;

    this.pagination.take(10);
    (this.pagination.pageable as any).pageSizes = [5, 10, 25, 50, 100];
  }

  public read(): Observable<PesateWidgetGridResult> {
    this.isLoading(true);

    if (!this.param) {
      this.chartRows = [];
      return of(new PesateWidgetGridResult([], this.columns, this.gridModel));
    }

    return this.pesateService.getAggregatedPesate(this.param).pipe(
      tap((result: any) => {
        this.chartRows = result.data as PesataAggregataRow[];
      }),
      switchMap((result: any) => {
        const rows = result.data as unknown as KendoGridRow[];
        return of(new PesateWidgetGridResult(rows, this.columns, this.gridModel));
      }),
      catchError(() => {
        this.chartRows = [];
        return of(new PesateWidgetGridResult([], this.columns, this.gridModel));
      }),
      finalize(() => this.isLoading(false))
    );
  }

  public perform(_: HttpAction, rows: Array<any>): Observable<KendoGridRow[]> {
    return of(rows);
  }

  getWidgetCookie(): PesateWidgetCookie | null {
    const raw = this.cookies.get(PESATE_WIDGET_COOKIE_NAME);
    return raw ? (JSON.parse(raw) as PesateWidgetCookie) : null;
  }

  setWidgetCookie(cookie: PesateWidgetCookie): void {
    this.cookies.set(PESATE_WIDGET_COOKIE_NAME, JSON.stringify(cookie), { path: '/' });
  }
}
