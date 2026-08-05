import { Injectable, Injector, OnDestroy } from '@angular/core';
import { Observable, of, Subscription } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TranslocoService } from '@jsverse/transloco';
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
import { RiepilogoRischiRow } from '../models/rischi-meteo.model';
import { RischiMeteoService } from './rischi-meteo.service';
import { FiltersRischiMeteoService } from './filters-rischi-meteo.service';

export class RiepilogoRischiResult extends KendoServerResult {
  constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: RiepilogoRischiRow[]) {
    super(model, columns, rows as any[]);
  }
}

export class RiepilogoRischiGridModel extends KendoGridModel {
  rowKey: ModelEntry;
  nome_azienda: ModelEntry;
  nome_appezzamento: ModelEntry;
  nazione: ModelEntry;
  regione: ModelEntry;
  superficie_ha: ModelEntry;
  nome_specie: ModelEntry;
  nome_varieta: ModelEntry;
}

@Injectable()
export class RiepilogoRischiGridConfigService extends AbstractGridConfigService<RiepilogoRischiResult> implements OnDestroy {

  gridId = 'riepilogo-rischi-meteo-grid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'rowKey';
  toolbar = new ToolbarSettings(false, false);

  lastRows: RiepilogoRischiRow[] = [];
  private currentPivaFiliera: string | null = null;
  private filtersSub: Subscription;

  constructor(
    protected injector: Injector,
    protected translocoService: TranslocoService,
    private rischiMeteoService: RischiMeteoService,
    private filtersService: FiltersRischiMeteoService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.selectable.shouldShowCheckbox = true;
    this.selectable.selectable.enabled = true;
    this.selectable.selectable.mode = 'multiple';
    this.selectable.selectable.checkboxOnly = true;

    this.cmdColumn.editBtn = false;
    this.cmdColumn.removeBtn = false;
    this.cmdColumn.infoBtn = false;

    this.filtersSub = this.filtersService.pivaFiliera$.subscribe(piva => {
      this.currentPivaFiliera = piva;
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
      { field: 'nome_azienda', title: this.translocoService.translate('rischim_column_azienda') },
      { resizable: true, filterable: true, editable: false, width: 180 }
    ),
    new KendoGridColumn(
      { field: 'nome_appezzamento', title: this.translocoService.translate('rischim_column_appezzamento') },
      { resizable: true, filterable: true, editable: false, width: 180 }
    ),
    new KendoGridColumn(
      { field: 'nazione', title: this.translocoService.translate('rischim_column_nazione') },
      { resizable: true, filterable: true, editable: false, width: 80 }
    ),
    new KendoGridColumn(
      { field: 'regione', title: this.translocoService.translate('rischim_column_regione') },
      { resizable: true, filterable: true, editable: false, width: 120 }
    ),
    new KendoGridColumn(
      { field: 'superficie_ha', title: this.translocoService.translate('rischim_column_superficie') },
      { resizable: true, filterable: false, editable: false, width: 100, filter: 'numeric' }
    ),
    new KendoGridColumn(
      { field: 'nome_specie', title: this.translocoService.translate('rischim_column_specie_colturale') },
      { resizable: true, filterable: true, editable: false, width: 140 }
    ),
    new KendoGridColumn(
      { field: 'nome_varieta', title: this.translocoService.translate('rischim_column_varieta_colturale') },
      { resizable: true, filterable: true, editable: false, width: 140 }
    )
  ];

  gridModel: RiepilogoRischiGridModel = {
    rowKey: new ModelEntry(CELL_TYPES.STRING, false),
    nome_azienda: new ModelEntry(CELL_TYPES.STRING, false),
    nome_appezzamento: new ModelEntry(CELL_TYPES.STRING, false),
    nazione: new ModelEntry(CELL_TYPES.STRING, false),
    regione: new ModelEntry(CELL_TYPES.STRING, false),
    superficie_ha: new ModelEntry(CELL_TYPES.NUMBER, false),
    nome_specie: new ModelEntry(CELL_TYPES.STRING, false),
    nome_varieta: new ModelEntry(CELL_TYPES.STRING, false)
  };

  read(): Observable<RiepilogoRischiResult> {
    if (!this.currentPivaFiliera) {
      return of(new RiepilogoRischiResult(this.gridModel, this.columns, []));
    }

    return this.rischiMeteoService.getRiepilogoRischi(this.currentPivaFiliera).pipe(
      map(perimetro => {
        const rows: RiepilogoRischiRow[] = perimetro.map(item => ({
          ...item,
          rowKey: `${item.id_esercizio}|${item.id_appezzamento}`
        }));
        this.lastRows = rows;
        return new RiepilogoRischiResult(this.gridModel, this.columns, rows);
      }),
      catchError(() => of(new RiepilogoRischiResult(this.gridModel, this.columns, [])))
    );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    return of([]);
  }
}
