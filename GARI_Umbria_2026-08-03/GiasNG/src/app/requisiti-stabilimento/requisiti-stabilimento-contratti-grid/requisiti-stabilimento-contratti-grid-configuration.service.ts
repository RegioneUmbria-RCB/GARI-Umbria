import { Injectable, Injector } from '@angular/core';
import { EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { AggregateSettings, AgrSelectableSettings, CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { RequisitiStabilimentoService } from '../requisiti-stabilimento.service';
import { RequisitiStabilimentoAPIService, SaveRequisitiStabilimento } from '../../Service/RequisitiStabilimento/requisiti-stabilimento.service';
import { GiasMessageService } from '../../Service/gias-message.service';
import { ConfigTemplate } from 'gias-kendo-grid';
import { SMARTPHONE_WIDTH } from '../../Model/CostantiPersonalizzate';
import { map, Observable, of, startWith, tap, withLatestFrom } from 'rxjs';
import { PercentGridViewerComponent } from "../percent-grid-viewer/percent-grid-viewer.component";
import { CELL_TYPES } from 'gias-ui-kit';

const VALID_GROUPABLE_COLUMNS = ['Superfice', 'Quantita', 'TipoTrasportoCod'];
const VALID_GROUPABLE_MODEL_FIELDS = ['Superfice', 'Quantita', 'TipoTrasportoDes', 'TipoTrasportoCod', 'ContrattoCodFiglio', 'FaseCodFiglio'];

export class RequisitiStabilimentoKendoServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

@Injectable()
export class RequisitiStabilimentoContractsGridModel extends KendoGridModel {
  Piva: ModelEntry;
  rag_soc: ModelEntry;
  Contratto_Nome: ModelEntry;
  Contratto_Cod: ModelEntry;
  Fase_Cod: ModelEntry;
  Mat_Cod: ModelEntry;
  Mat_Des: ModelEntry;
  Superficie: ModelEntry;
  QtaPrevista: ModelEntry;
  ResaPrevista: ModelEntry;
  Sup_Assegnata: ModelEntry;
  Qta_Assegnata: ModelEntry;
  Percentuale_Sup_Assegnata: ModelEntry;
  Percentuale_Qta_Assegnata: ModelEntry;
}

@Injectable()
export class RequisitiStabilimentoContrattiGridConfigurationService extends AbstractGridConfigService<RequisitiStabilimentoKendoServerResult> {

  editingMode: EditingMode = EditingMode.IN_CELL_BATCH;
  gridId: string = 'requisiti-stabilimento-contratti-grid';
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'Contratto_Cod';

  aggregates = new AggregateSettings(
    {
      enabled: true,
      descriptors: [
        { field: 'Superficie', aggregate: 'sum', format: 'n0' },
        { field: 'QtaPrevista', aggregate: 'sum', format: 'n0' },
        { field: 'ResaPrevista', aggregate: 'average', format: 'n0' },
        { field: 'Sup_Assegnata', aggregate: 'sum', format: 'n0' },
        { field: 'Qta_Assegnata', aggregate: 'sum', format: 'n0' }
      ]
    }
  );

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'Contratto_Nome', title: this.transloco.translate('Contratto_Nome') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Mat_Des', title: this.transloco.translate('Prodotto') },
      { resizable: true, filterable: true, editable: false, width: 100 }
    ),
    new KendoGridColumn(
      { field: 'Superficie', title: this.transloco.translate('SuperficieHaAbbr') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n2' }
    ),
    new KendoGridColumn(
      { field: 'QtaPrevista', title: this.transloco.translate('QuantitaKg') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n0' }
    ),
    new KendoGridColumn(
      { field: 'ResaPrevista', title: this.transloco.translate('ResaMediaPerHa') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n0' }
    ),
    new KendoGridColumn(
      { field: 'Sup_Assegnata', title: this.transloco.translate('SupAssegnata') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n2' }
    ),
    new KendoGridColumn(
      { field: 'Qta_Assegnata', title: this.transloco.translate('QtaAssegnata') },
      { resizable: true, filterable: true, editable: false, width: 100, format: 'n0' }
    ),
    new KendoGridColumn(
      { field: 'Percentuale_Sup_Assegnata', title: this.transloco.translate('Percentuale_Sup_Assegnata') },
      { resizable: true, filterable: true, editable: false, width: 100, component: PercentGridViewerComponent }
    ),
    new KendoGridColumn(
      { field: 'Percentuale_Qta_Assegnata', title: this.transloco.translate('Percentuale_Qta_Assegnata') },
      { resizable: true, filterable: true, editable: false, width: 100, component: PercentGridViewerComponent }
    )
  ];

  standardModel: RequisitiStabilimentoContractsGridModel = {
    Piva: new ModelEntry(CELL_TYPES.STRING),
    rag_soc: new ModelEntry(CELL_TYPES.STRING),
    Contratto_Nome: new ModelEntry(CELL_TYPES.STRING),
    Contratto_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Fase_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Mat_Cod: new ModelEntry(CELL_TYPES.NUMBER),
    Mat_Des: new ModelEntry(CELL_TYPES.STRING),
    Superficie: new ModelEntry(CELL_TYPES.NUMBER),
    QtaPrevista: new ModelEntry(CELL_TYPES.NUMBER),
    ResaPrevista: new ModelEntry(CELL_TYPES.NUMBER),
    Sup_Assegnata: new ModelEntry(CELL_TYPES.NUMBER),
    Qta_Assegnata: new ModelEntry(CELL_TYPES.NUMBER),
    Percentuale_Sup_Assegnata: new ModelEntry(CELL_TYPES.CUSTOM),
    Percentuale_Qta_Assegnata: new ModelEntry(CELL_TYPES.CUSTOM)
  };

  constructor(
    injector: Injector,
    private requisitiStabilimentoService: RequisitiStabilimentoService,
    private requisitiStabilimentoAPIService: RequisitiStabilimentoAPIService,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.selectable = new AgrSelectableSettings();
    this.selectable.selectable.checkboxOnly = false;
    this.selectable.selectable.enabled = false;
    this.selectable.shouldShowCheckbox = false;

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = false;
    this.toolbar.resetChanges = false;

    this.resizable = new ResizableSettings(true, true);
    this.cmdColumn = new CommandsColumnSettings({
      editBtn: false,
      removeBtn: false,
    });

    this.groups.groupable.enabled = true;
    this.views.enabled = true;

    if (window.innerWidth < SMARTPHONE_WIDTH) {
      this.toolbar.newItem = false;
      this.cmdColumn.editBtn = false;
      this.groups.groupable.enabled = false;
    }
  }

  read(_: any): Observable<RequisitiStabilimentoKendoServerResult> {
    this.isLoading(true);

    return of(true)
      .pipe(
        withLatestFrom(this.requisitiStabilimentoService.contracts$.pipe(startWith([]))),
        map(([_, contracts]: [boolean, any]) => {
          if (contracts == null || contracts.length === 0) {
            return new RequisitiStabilimentoKendoServerResult([], this.columns, this.standardModel);
          }

          const validData = contracts.kendo_rows as any[];
          if (validData.length == 0) {
            this.giasMessageService.infoMessagge('RdS.NessunRequisitoDiStabilimentoConQuestiDati', false, true);
          }

          return new RequisitiStabilimentoKendoServerResult(contracts.kendo_rows, this.columns, this.standardModel);
        }),
        tap(() => this.isLoading(false))
      );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    if (actionType != HttpAction.BATCH_SAVE) {
      throw new Error('Operazione non consentita');
    }

    const payload: SaveRequisitiStabilimento = { rows: items.updated } as SaveRequisitiStabilimento;
    this.requisitiStabilimentoAPIService
      .saveRequisitiStabilimento(payload)
      .subscribe({
        next: () => {
          this.giasMessageService.successMessage('SalvataggioAvvenutoConSuccesso', false, true);
          this.requisitiStabilimentoService.nextReloadGrid();
        },
        error: () => this.giasMessageService.errorMessage('ErroreSalvataggio', false, true)
      });

    return of(items);
  }




}
