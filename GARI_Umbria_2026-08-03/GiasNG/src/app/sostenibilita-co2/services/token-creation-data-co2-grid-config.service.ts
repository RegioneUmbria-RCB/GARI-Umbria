import { Injectable, Injector, OnDestroy } from '@angular/core';
import { Observable, of, Subscription } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
import { RowArgs, RowClassArgs, SelectionEvent } from '@progress/kendo-angular-grid';
import {
  AbstractGridConfigService,
  ConfigTemplate,
  EditingMode,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { TokenCreationFilters, TokenGenerabileRow } from '../models/sostenibilita-co2.model';
import { FiltersTokenCreationCo2Service } from './filters-token-creation-co2.service';
import { PerimetroCO2Service } from './perimetro-co2.service';

export class TokenCreationCo2Result extends KendoServerResult {
  constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: TokenGenerabileRow[]) {
    super(model, columns, rows as any[]);
  }
}

export class TokenCreationCo2GridModel extends KendoGridModel {
  rowKey: ModelEntry;
  idInvocazione: ModelEntry;
  dataInvocazione: ModelEntry;
  azienda: ModelEntry;
  aziendaLabel: ModelEntry;
  variazSocBiogenico: ModelEntry;
  nAppezzamenti: ModelEntry;
}

@Injectable()
export class TokenCreationCo2GridConfigService extends AbstractGridConfigService<TokenCreationCo2Result> implements OnDestroy {

  gridId = 'token-creation-data-co2-grid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'rowKey';
  toolbar = new ToolbarSettings(false, false);

  lastRows: TokenGenerabileRow[] = [];
  currentFilters: TokenCreationFilters | null = null;
  private _selectedRowKeys = new Set<string>();
  selectionCount = 0;

  get selectedRowKeys(): ReadonlySet<string> {
    return this._selectedRowKeys;
  }

  clearSelection(): void {
    this._selectedRowKeys.clear();
    this.selectionCount = 0;
  }

  private filtersSub: Subscription;

  constructor(
    protected injector: Injector,
    protected translocoService: TranslocoService,
    private filtersTokenCreationCo2Service: FiltersTokenCreationCo2Service,
    private perimetroCO2Service: PerimetroCO2Service
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    // Multi-selection: one selectable (isMostRecent) row per azienda group
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.enabled = true;
    this.selectable.selectable.mode = 'multiple';
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.preselectedRows.isRowSelectedFn = (e: RowArgs) => {
      const row = e.dataItem as TokenGenerabileRow;
      return row?.isMostRecent && this._selectedRowKeys.has(row.rowKey);
    };
    this.selectable.preselectedRows.selectionChangeFn = (event: SelectionEvent) => {
      event.selectedRows?.forEach(s => {
        const row = s.dataItem as TokenGenerabileRow;
        if (row?.isMostRecent) {
          this._selectedRowKeys.add(row.rowKey);
        }
      });
      event.deselectedRows?.forEach(d => {
        const row = d.dataItem as TokenGenerabileRow;
        if (row?.rowKey) {
          this._selectedRowKeys.delete(row.rowKey);
        }
      });
      this.selectionCount = this._selectedRowKeys.size;
    };

    // Read-only grid
    this.cmdColumn.editBtn = false;
    this.cmdColumn.removeBtn = false;
    this.cmdColumn.infoBtn = false;

    // Group by azienda (fixed, not user-changeable) sorted by most-recent first within each group
    this.groups.groupable.enabled = false;
    this.pagination.gridState.group = [{ field: 'aziendaLabel', dir: 'asc' }];
    this.pagination.gridState.sort = [{ field: 'dataInvocazione', dir: 'desc' }];
    this.sort.sort = [{ field: 'dataInvocazione', dir: 'desc' }];

    this.filtersSub = this.filtersTokenCreationCo2Service.filters$.subscribe(filters => {
      this.currentFilters = filters;
    });
  }

  ngOnDestroy(): void {
    this.filtersSub?.unsubscribe();
  }

  onRowClass = (e: RowClassArgs): { [k: string]: boolean } => ({
    'row-not-selectable': !(e.dataItem as TokenGenerabileRow)?.isMostRecent
  });

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'rowKey', title: '' },
      { hidden: true, editable: false, width: 0 }
    ),
    new KendoGridColumn(
      { field: 'aziendaLabel', title: this.translocoService.translate('sco2_column_azienda') },
      { hidden: true, editable: false, width: 0 }
    ),
    new KendoGridColumn(
      { field: 'idInvocazione', title: this.translocoService.translate('sco2_column_id_invocazione') },
      { resizable: true, filterable: true, editable: false, width: 160 }
    ),
    new KendoGridColumn(
      { field: 'dataInvocazione', title: this.translocoService.translate('sco2_column_data_invocazione') },
      { resizable: true, filterable: true, editable: false, width: 160, filter: 'date', format: 'dd/MM/yyyy HH:mm' }
    ),
    new KendoGridColumn(
      { field: 'azienda', title: this.translocoService.translate('sco2_column_azienda') },
      { resizable: true, filterable: true, editable: false, width: 200 }
    ),
    new KendoGridColumn(
      { field: 'variazSocBiogenico', title: this.translocoService.translate('sco2_column_variaz_soc_biogenico') },
      { resizable: true, filterable: false, editable: false, width: 160, filter: 'numeric' }
    ),
    new KendoGridColumn(
      { field: 'nAppezzamenti', title: this.translocoService.translate('sco2_column_n_appezzamenti') },
      { resizable: true, filterable: false, editable: false, width: 120, filter: 'numeric' }
    )
  ];

  gridModel: TokenCreationCo2GridModel = {
    rowKey: new ModelEntry(CELL_TYPES.STRING, false),
    idInvocazione: new ModelEntry(CELL_TYPES.STRING, false),
    dataInvocazione: new ModelEntry(CELL_TYPES.DATE, false),
    azienda: new ModelEntry(CELL_TYPES.STRING, false),
    aziendaLabel: new ModelEntry(CELL_TYPES.STRING, false),
    variazSocBiogenico: new ModelEntry(CELL_TYPES.NUMBER, false),
    nAppezzamenti: new ModelEntry(CELL_TYPES.NUMBER, false)
  };

  read(): Observable<TokenCreationCo2Result> {
    if (!this.currentFilters?.filiera || !this.currentFilters?.anno) {
      return of(new TokenCreationCo2Result(this.gridModel, this.columns, []));
    }

    return this.perimetroCO2Service
      .getTokenGenerabili(this.currentFilters.filiera, this.currentFilters.anno)
      .pipe(
        map(rows => {
          this.lastRows = rows;
          return new TokenCreationCo2Result(this.gridModel, this.columns, rows);
        }),
        catchError(() => {
          this.lastRows = [];
          return of(new TokenCreationCo2Result(this.gridModel, this.columns, []));
        })
      );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return of([]);
  }
}
