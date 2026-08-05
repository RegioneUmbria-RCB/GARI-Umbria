import { Injectable, Injector } from '@angular/core';
import { ConfigTemplate } from 'gias-kendo-grid';
import {  DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType, ModelEntry, NumericSettings } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { SMARTPHONE_WIDTH } from '../../Model/CostantiPersonalizzate';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, map, of, startWith, tap, withLatestFrom } from 'rxjs';
import { AggregateSettings, AgrSelectableSettings, CommandsColumnSettings, ResizableSettings, ToolbarSettings } from 'gias-kendo-grid';
import { RequisitiStabilimentoContrattoModel, RequisitiStabilimentoModel, RequisitiStabilimentoService } from '../requisiti-stabilimento.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { RequisitiStabilimentoAPIService, SaveRequisitiStabilimento } from 'app/Service/RequisitiStabilimento/requisiti-stabilimento.service';

const VALID_GROUPABLE_COLUMNS = ['Superfice', 'Quantita', 'TipoTrasportoCod'];
const VALID_GROUPABLE_MODEL_FIELDS = ['Superfice', 'Quantita', 'TipoTrasportoDes', 'TipoTrasportoCod', 'ContrattoCodFiglio', 'FaseCodFiglio'];

export class RequisitiStabilimentoKendoServerResult extends KendoServerResult {
  constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
    super(model, cols, rows);
  }
}

export class RequisitiStabilimentoGridModel extends KendoGridModel {
  PIVA: ModelEntry;
  rag_soc: ModelEntry;
  cuaa: ModelEntry;
  Piva_Padre: ModelEntry;
  GruppoRaccolta_Cod: ModelEntry;
  GruppoRaccolta_Des: ModelEntry;
  Mat_Cod: ModelEntry;
  Mat_Des: ModelEntry;
  Sup: ModelEntry;
  Superficie: ModelEntry;
  ResaPrevista: ModelEntry;
  ImpiantiResaMedia: ModelEntry;
  SupDaAssegnare: ModelEntry;
  ResaDaAssegnare: ModelEntry;
}

@Injectable()
export class RequisitiStabilimentoGridConfigurationService extends AbstractGridConfigService<RequisitiStabilimentoKendoServerResult> {

  editingMode: EditingMode = EditingMode.IN_CELL_BATCH;
  gridId: string = 'requisiti-stabilimento-grid';
  loader: LoaderType = LoaderType.SERVICE;
  rowId: string = 'PIVA';

  aggregates = new AggregateSettings({
    enabled: true,
    descriptors: [
      { field: 'Superficie', aggregate: 'sum', format: 'n0' },
      { field: 'ResaPrevista', aggregate: 'sum', format: 'n0' },
      { field: 'QtaPrevista', aggregate: 'average', format: 'n0' },
      { field: 'SupDaAssegnare', aggregate: 'sum', format: 'n0' },
      { field: 'ResaDaAssegnare', aggregate: 'sum', format: 'n0' },
      { field: 'ImpiantiResaMedia', aggregate: 'average', format: 'n0' }
    ]
  }
  );

  tipoTrasportoDDL: DropdownListItem[] = [
    { id: 0, name: '' },
    { id: 1, name: 'Franco Fabbrica' },
    { id: 2, name: 'Trasportato' },
    { id: 3, name: 'Misto' }
  ];

  currentData: any[] = [];

  columns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'partitaIvaReale', title: this.transloco.translate('PartitaIVA') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 1, customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'rag_soc', title: this.transloco.translate('RagioneSociale') },
      { resizable: true, filterable: true, editable: false, width: 200, orderIndex: 2, customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'cuaa', title: this.transloco.translate('CodiceUnicoAziendaAgricolaSigla') },
      { resizable: true, filterable: true, editable: false, width: 150, orderIndex: 3, customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'Rag_Soc_Padre', title: this.transloco.translate('ImpresaReferente') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 4, customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'GruppoRaccolta_Des', title: this.transloco.translate('GruppoRaccolta') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 5, customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'Mat_Des', title: this.transloco.translate('Prodotto') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 6, customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'Superficie', title: this.transloco.translate('SuperficieHaAbbr') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 7, format:'{0:n4}', customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'ResaPrevista', title: this.transloco.translate('QuantitaKg') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 8, format:'{0:n0}', customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'ImpiantiResaMedia', title: this.transloco.translate('ResaMediaPerHa') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 9, format:'{0:n0}', customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'SupDaAssegnare', title: this.transloco.translate('SupDaAssegnare') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 10, format:'{0:n4}', customParent: 'Dati Impresa' }
    ),
    new KendoGridColumn(
      { field: 'ResaDaAssegnare', title: this.transloco.translate('ResaDaAssegnare') },
      { resizable: true, filterable: true, editable: false, width: 100, orderIndex: 11, format:'{0:n0}', customParent: 'Dati Impresa' }
    ),
  ];

  standardModel: RequisitiStabilimentoGridModel = {
    PIVA: new ModelEntry(CELL_TYPES.STRING),
    rag_soc: new ModelEntry(CELL_TYPES.STRING),
    cuaa: new ModelEntry(CELL_TYPES.STRING),
    Piva_Padre: new ModelEntry(CELL_TYPES.STRING),
    Rag_Soc_Padre: new ModelEntry(CELL_TYPES.STRING),
    GruppoRaccolta_Cod: new ModelEntry(CELL_TYPES.NUMBER),// DROPDOWNLIST
    GruppoRaccolta_Des: new ModelEntry(CELL_TYPES.STRING),
    Mat_Cod: new ModelEntry(CELL_TYPES.NUMBER),// DROPDOWNLIST
    Mat_Des: new ModelEntry(CELL_TYPES.STRING),
    Sup: new ModelEntry(CELL_TYPES.NUMBER),
    Superficie: new ModelEntry(CELL_TYPES.NUMBER),
    ResaPrevista: new ModelEntry(CELL_TYPES.NUMBER),
    ImpiantiResaMedia: new ModelEntry(CELL_TYPES.NUMBER),
    SupDaAssegnare: new ModelEntry(CELL_TYPES.NUMBER),
    ResaDaAssegnare: new ModelEntry(CELL_TYPES.NUMBER),
    partitaIvaReale: new ModelEntry(CELL_TYPES.STRING),
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

    this.groups.groupable.enabled = false;
    this.views.enabled = false;
    this.generalSettings.performOnEdit = true;

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
        withLatestFrom(this.requisitiStabilimentoService.result$.pipe(startWith(null)), this.requisitiStabilimentoService.contracts$.pipe(startWith([]))),
        map(([_, data, contracts]: [boolean, any[] | string, any[] | string]) => {
          if (data == null || data === '' || contracts == null || contracts === '') {
            return new RequisitiStabilimentoKendoServerResult([], this.columns, this.standardModel);
          }

          const validData = data as any[];
          if (validData.length == 0) {
            this.giasMessageService.infoMessagge('RdS.NessunRequisitoDiStabilimentoConQuestiDati', false, true);
          }

          this.currentData = validData;
          const validContracts = ((contracts as any).kendo_rows ?? []) as RequisitiStabilimentoContrattoModel[];
          const result = this.getCompleteColumnsAndModel(validData, validContracts);
          this.handleUpdates(this.currentData);
          return result;
        }),
        tap(() => this.isLoading(false))
      );
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    if (actionType == HttpAction.UPDATE) {
      this.handleUpdates(items);
      return of(items);
    }

    if (actionType != HttpAction.BATCH_SAVE) {
      return of(items);
    }

    const payload = { rows: items.updated } as SaveRequisitiStabilimento;
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

  private getCompleteColumnsAndModel(data: RequisitiStabilimentoModel[], contracts: RequisitiStabilimentoContrattoModel[]): RequisitiStabilimentoKendoServerResult {
    if (data.length == 0) {
      return new RequisitiStabilimentoKendoServerResult(data, this.columns, this.standardModel);
    }

    const columns = [...this.columns];
    const model = { ...this.standardModel };

    for (const key in data[0]) {
      const match = key.match(/(^\d+)_(\d+)_(.+$)/);
      if (match == null || VALID_GROUPABLE_MODEL_FIELDS.find(x => x == match[3]) == null) {
        continue;
      }

      const parent = this.getParent(contracts, match);

      // TODO Salvo: sistemare tipo model entry
      if (VALID_GROUPABLE_COLUMNS.find(x => x == match[3]) != null) {
        let column: KendoGridColumn;
        if (match[3] == VALID_GROUPABLE_COLUMNS[2]) {
          model[key] = new ModelEntry(CELL_TYPES.DROPDOWNLIST);
          column = this.handleDropdownColumn(key, match, parent);
        } else {
          model[key] = new ModelEntry(CELL_TYPES.NUMBER);
          column = this.handleNumericColumn(key, match, parent);
        }
        column.orderIndex = 99;
        columns.push(column);
      } else if (VALID_GROUPABLE_MODEL_FIELDS.find(x => x == match[3]) != null) {
        model[key] = new ModelEntry(CELL_TYPES.STRING);
      }
    }

    return new RequisitiStabilimentoKendoServerResult(data, columns, model);
  }

  private getParent(contracts: RequisitiStabilimentoContrattoModel[], match: RegExpMatchArray): string {
    const contract = contracts.find(x => x.Contratto_Cod == +match[1] && x.Fase_Cod == +match[2]);
    if (contract == null) {
      return "Contratto";
    }

    if (contract.Contratto_Nome != "") {
      return contract.Contratto_Nome;
    }

    return contract.Azienda;
  }

  private handleNumericColumn(key: string, match: RegExpMatchArray, parent: string): KendoGridColumn {
    let format = '{0:n0}';
    let numericSettings = new NumericSettings({defaultValue: 0, min: 0, format: 'n0'});
    let title =match[3]
    if (key.includes('Superfi')){
      format = '{0:n4}';
      numericSettings = new NumericSettings({defaultValue: 0, min: 0, format: 'n4'});
      title = this.transloco.translate('SuperficieHaAbbr');
    } else if (key.includes('Quanti')){
      format = '{0:n0}';
      numericSettings = new NumericSettings({defaultValue: 0, min: 0, format: 'n0'});
      title = this.transloco.translate('QuantitaKg');
    } else {
      format = '{0:n0}';
      numericSettings = new NumericSettings({defaultValue: 0, min: 0, format: 'n0'});
    }
    const c = new KendoGridColumn(
      { field: key, title: title },
      {
        resizable: true,
        filterable: true,
        editable: true,
        numeric: numericSettings,
        width: 100,
        format:format,
        customParent: parent,
      }
    );

    if (this.aggregates.descriptors.findIndex((el) => el.field == key) == -1) {
      this.aggregates.descriptors.push({ field: key, aggregate: 'sum', format: 'n0' });
    }

    return c;
  }

  private handleDropdownColumn(key: string, match: RegExpMatchArray, parent: string): KendoGridColumn {
    const c: KendoGridColumn = new KendoGridColumn(
      { field: key, title: this.transloco.translate('TipoTrasporto') },
      {
        resizable: true,
        filterable: true,
        editable: true,
        width: 100,
        customParent: parent
      }
    );

    c.ddl = new DropdownListWithForm('id', key, match[3], this.tipoTrasportoDDL);
    c.ddl.valuePrimitive = true;
    c.ddl.descriptionField = match[1] + '_' + match[2] + '_TipoTrasportoDes';
    return c;
  }

  private handleUpdates(updates: any[]): void {
    for (const update of updates) {
      const currentData = this.currentData.find(x => x.PIVA == update.PIVA);
      if (currentData == null) {
        continue;
      }

      currentData.SupDaAssegnare = parseFloat((currentData.Superficie - this.getSum(update, 'Superfice')).toFixed(4));
      currentData.ResaDaAssegnare = parseInt((currentData.ResaPrevista - this.getSum(update, 'Quantita')).toFixed(0));
    }
  }

  private getSum(data: any, field: string): number {
    let result = 0;
    for (const key in data) {
      const match = key.match(/(^\d+)_(\d+)_(.+$)/);
      if (match == null || !match[3].includes(field)) {
        continue;
      }

      result += +data[key];
    }

    return result;
  }
}
