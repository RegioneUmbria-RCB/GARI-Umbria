import { Injectable, Injector } from '@angular/core';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  DettagliColumnSettings,
  EditingMode,
  ExcelSettings,
  HttpAction,
  KendoGridColumn,
  KendoServerResultImpl,
  LoaderType,
  PDFSettings,
  ToolbarSettings,
} from 'gias-kendo-grid';
import { RowClassArgs } from '@progress/kendo-angular-grid';
import { Observable, of } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { PesateAccrescimentoFilters, PesataCurvaAccrescimentoModel, PesataCurvaAccrescimentoRow, PesateCurvaAccrescimentoResult } from '../models/pesate-accrescimento.model';
import { PesateAccrescimentoService } from './pesate-accrescimento.service';
import { ConversionService } from 'gias-ui-kit';

/**
 * Grid configuration service for the pesate / curva di accrescimento grid.
 * Extends AbstractGridConfigService using LoaderType.SERVICE so data is fetched
 * via PesateAccrescimentoService rather than a direct HTTP endpoint action.
 *
 * See UIDS001 — "Pattern Griglia — gias-kendo-grid".
 */
@Injectable()
export class PesateAccrescimentoGridConfigService
  extends AbstractGridConfigService<KendoServerResultImpl> {

  editingMode = EditingMode.IN_PAGE;
  gridId = 'pesateAccrescimentoGrid';
  rowId = 'lid';
  loader = LoaderType.SERVICE;

  protected readonly _gridModel = PesataCurvaAccrescimentoModel;

  public gridColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'sta_des', title: this.transloco.translate('Stalla') },
      { width: 160, editable: false, filterable: true }
    ),
    new KendoGridColumn(
      { field: 'razza_des', title: this.transloco.translate('Razza') },
      { width: 140, editable: false, filterable: true }
    ),
    new KendoGridColumn(
      { field: 'lid', title: this.transloco.translate('LID') },
      { width: 160, editable: false, filterable: true }
    ),
    new KendoGridColumn(
      { field: 'peso_kg', title: this.transloco.translate('PesoKg') },
      { width: 110, editable: false, filterable: true, format: 'n2' }
    ),
    new KendoGridColumn(
      { field: 'peso_teorico', title: this.transloco.translate('PesoTeorico') },
      { width: 130, editable: false, filterable: true, format: 'n2' }
    ),
    new KendoGridColumn(
      { field: 'scostamento_pct', title: this.transloco.translate('Scostamento') },
      { width: 130, editable: false, filterable: true, format: 'n2' }
    ),
    new KendoGridColumn(
      { field: 'data_pesata', title: this.transloco.translate('DataPesata') },
      { width: 130, editable: false, filterable: true, format: 'dd/MM/yyyy' }
    ),
    new KendoGridColumn(
      { field: 'eta_giorni', title: this.transloco.translate('EtaGiorni') },
      { width: 110, editable: false, filterable: true }
    ),
    new KendoGridColumn(
      { field: 'alert_label', title: this.transloco.translate('FlagAlert') },
      { width: 90, editable: false, filterable: true }
    ),
  ];

  public lastResult: PesateCurvaAccrescimentoResult | null = null;
  protected _gridRows: PesataCurvaAccrescimentoRow[] = [];
  protected _serverTotalCount = 0;

  constructor(
    injector: Injector,
    private pesateService: PesateAccrescimentoService,
    private conversionService: ConversionService
  ) {
    super(injector);
    this.handleCustomizations();
  }

  protected handleCustomizations(): void {
    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.cmdColumn = new CommandsColumnSettings({ editBtn: false, infoBtn: false, removeBtn: false });
    this.dettagliColumn = new DettagliColumnSettings({ editBtn: false, infoBtn: false });
    this.columnMenu.kendoGridColumnChooser = true;
    this.columnMenu.columnMenu = true;
    this.behavior.excelSettings = new ExcelSettings({ enabled: false });
    this.behavior.pdfSettings = new PDFSettings({ enabled: false });
    this.views.enabled = true;
    this.resizable.autoFitColumns = true;
    this.resizable.isResizable = true;
    this.gridIsEditable = false;
    this.pagination.take(100);
    (this.pagination.pageable as any).pageSizes = [5, 10, 25, 50, 100, 200];

    // Row coloring based on alert_flag and deviation magnitude.
    // See UIDS001 — "Colorazione Condizionale per Riga".
    this.onRowClass = (event: RowClassArgs): { [k: string]: boolean } => {
      const row = event.dataItem as PesataCurvaAccrescimentoRow;
      if (!row?.alert_flag) return {};
      return Math.abs(row.scostamento_pct) >= 20
        ? { 'row-alert-red': true }
        : { 'row-alert-yellow': true };
    };
  }

  /**
   * Loads the paged weighing data from DS06.
   * Called by the component on filter / page / sort changes.
   * See UIDS001 — "Griglia Metriche Accrescimento — Pagina Dedicata (Fase 1)".
   */
  override read(filters?: PesateAccrescimentoFilters): Observable<KendoServerResultImpl> {
    if (!filters) {
      return of(new KendoServerResultImpl(this._gridModel, this.gridColumns, this._gridRows));
    }

    return this.pesateService.getPesateCurveAccrescimento(filters).pipe(
      tap((result) => {
        this.lastResult = result;
        this._serverTotalCount = result.metadata.pagination.total_pages * filters.limit;
      }),
      map((result) => {
        const converted = this.conversionService.ConversionDateInObject_Array(result.data as any[]);
        const rows = converted.map((r: any) => ({ ...r, alert_label: r.alert_flag ? 'S\u00ec' : 'No' }));
        this._gridRows = rows as unknown as PesataCurvaAccrescimentoRow[];
        return new KendoServerResultImpl(this._gridModel, this.gridColumns, rows);
      })
    );
  }

  override perform(_action: HttpAction, _items: any): Observable<any[]> {
    return of([]);
  }
}
