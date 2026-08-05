import { Injectable, Injector, OnDestroy } from '@angular/core';
import { Observable, of, Subscription } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
import {
  AbstractGridConfigService,
  ConfigTemplate,
  EditingMode,
  GiasMessageService,
  HttpAction,
  KendoGridColumn,
  KendoGridModel,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  ToolbarSettings
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { PerimetroFilters, RiepilogoPerimetroRow } from '../models/sostenibilita-co2.model';
import { FiltersPerimetroService } from './filters-perimetro.service';
import { PerimetroCO2Service } from './perimetro-co2.service';

export class RiepilogoPerimetroResult extends KendoServerResult {
  constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: RiepilogoPerimetroRow[]) {
    super(model, columns, rows as any[]);
  }
}

export class RiepilogoPerimetroGridModel extends KendoGridModel {
  rowKey: ModelEntry;
  azienda: ModelEntry;
  piva: ModelEntry;
  appezzamento: ModelEntry;
  esercizio: ModelEntry;
  nazione: ModelEntry;
  regione: ModelEntry;
  superficie_ha: ModelEntry;
  specie_colturale: ModelEntry;
  prodotti_raccolti: ModelEntry;
  codici_lotti: ModelEntry;
  data_ultima_raccolta: ModelEntry;
  totale_raccolto_kg: ModelEntry;
}

@Injectable()
export class RiepilogoPerimetroGridConfigService extends AbstractGridConfigService<RiepilogoPerimetroResult> implements OnDestroy {

  gridId = 'riepilogo-perimetro-co2-grid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'rowKey';
  toolbar = new ToolbarSettings(false, false);

  lastRows: RiepilogoPerimetroRow[] = [];
  currentFilters: PerimetroFilters | null = null;

  private filtersSub: Subscription;

  constructor(
    protected injector: Injector,
    protected translocoService: TranslocoService,
    private filtersPerimetroService: FiltersPerimetroService,
    private perimetroCO2Service: PerimetroCO2Service,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    // Enable multi-row checkbox selection
    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.enabled = true;
    this.selectable.selectable.mode = 'multiple';
    this.selectable.selectable.checkboxOnly = true;
    this.selectable.columnSettings.showSelectAll = true;

    // Read-only grid: disable all command column buttons
    this.cmdColumn.editBtn = false;
    this.cmdColumn.removeBtn = false;
    this.cmdColumn.infoBtn = false;

    this.filtersSub = this.filtersPerimetroService.filters$.subscribe(filters => {
      this.currentFilters = filters;
    });
  }

  ngOnDestroy(): void {
    this.filtersSub?.unsubscribe();
  }

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'rowKey', title: '' },
      { hidden: true, editable: false, width: 0 }
    ),
    new KendoGridColumn(
      { field: 'azienda', title: this.translocoService.translate('sco2_column_azienda') },
      { resizable: true, filterable: true, editable: false, width: 180 }
    ),
    new KendoGridColumn(
      { field: 'appezzamento', title: this.translocoService.translate('sco2_column_appezzamento') },
      { resizable: true, filterable: true, editable: false, width: 130 }
    ),
    new KendoGridColumn(
      { field: 'esercizio', title: this.translocoService.translate('sco2_column_esercizio') },
      { resizable: true, filterable: true, editable: false, width: 90 }
    ),
    new KendoGridColumn(
      { field: 'nazione', title: this.translocoService.translate('sco2_column_nazione') },
      { resizable: true, filterable: true, editable: false, width: 80 }
    ),
    new KendoGridColumn(
      { field: 'regione', title: this.translocoService.translate('sco2_column_regione') },
      { resizable: true, filterable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'superficie_ha', title: this.translocoService.translate('sco2_column_superficie') },
      { resizable: true, filterable: false, editable: false, width: 100, filter: 'numeric' }
    ),
    new KendoGridColumn(
      { field: 'specie_colturale', title: this.translocoService.translate('sco2_column_specie_colturale') },
      { resizable: true, filterable: true, editable: false, width: 140 }
    ),
    new KendoGridColumn(
      { field: 'prodotti_raccolti', title: this.translocoService.translate('sco2_column_prodotti_raccolti') },
      { resizable: true, filterable: true, editable: false, width: 160 }
    ),
    new KendoGridColumn(
      { field: 'codici_lotti', title: this.translocoService.translate('sco2_column_codice_lotti') },
      { resizable: true, filterable: true, editable: false, width: 150 }
    ),
    new KendoGridColumn(
      { field: 'data_ultima_raccolta', title: this.translocoService.translate('sco2_column_data_ultima_raccolta') },
      { resizable: true, filterable: true, editable: false, width: 140, filter: 'date', format: 'dd/MM/yyyy' }
    ),
    new KendoGridColumn(
      { field: 'totale_raccolto_kg', title: this.translocoService.translate('sco2_column_raccolta_kg') },
      { resizable: true, filterable: false, editable: false, width: 110, filter: 'numeric' }
    )
  ];

  gridModel: RiepilogoPerimetroGridModel = {
    rowKey: new ModelEntry(CELL_TYPES.STRING, false),
    azienda: new ModelEntry(CELL_TYPES.STRING, false),
    piva: new ModelEntry(CELL_TYPES.STRING, false),
    appezzamento: new ModelEntry(CELL_TYPES.STRING, false),
    esercizio: new ModelEntry(CELL_TYPES.STRING, false),
    nazione: new ModelEntry(CELL_TYPES.STRING, false),
    regione: new ModelEntry(CELL_TYPES.STRING, false),
    superficie_ha: new ModelEntry(CELL_TYPES.NUMBER, false),
    specie_colturale: new ModelEntry(CELL_TYPES.STRING, false),
    prodotti_raccolti: new ModelEntry(CELL_TYPES.STRING, false),
    codici_lotti: new ModelEntry(CELL_TYPES.STRING, false),
    data_ultima_raccolta: new ModelEntry(CELL_TYPES.DATE, false),
    totale_raccolto_kg: new ModelEntry(CELL_TYPES.NUMBER, false)
  };

  read(): Observable<RiepilogoPerimetroResult> {
    if (!this.currentFilters) {
      return of(new RiepilogoPerimetroResult(this.gridModel, this.columns, []));
    }

    return this.perimetroCO2Service
      .getRiepilogoRaccolti(this.currentFilters.filiera, this.currentFilters.anno)
      .pipe(
        map(response => {
          const rows = this.perimetroCO2Service.mapToRiepilogoRows(
            response.Righe,
            this.currentFilters.modalita,
            this.currentFilters.coltura
          );
          this.lastRows = rows;
          return new RiepilogoPerimetroResult(this.gridModel, this.columns, rows);
        }),
        catchError(() => {
          this.lastRows = [];
          this.giasMessageService.errorMessage(this.translocoService.translate('sco2_error_message'));
          return of(new RiepilogoPerimetroResult(this.gridModel, this.columns, []));
        })
      );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return of([]);
  }
}
