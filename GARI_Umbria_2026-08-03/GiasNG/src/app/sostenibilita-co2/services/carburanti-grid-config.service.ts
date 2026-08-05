import { Injectable, Injector } from '@angular/core';
import { Observable, of, Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  ConfigTemplate,
  DropdownListItem,
  DropdownListWithForm,
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
import { AjaxAgronicaNetCore6ApiService } from 'app/Service/ajax-agronica-net-core6-api.service';
import { CarburantiCO2Model, TipoCarburanteItem } from '../models/sostenibilita-co2.model';
import { Co2DataStoreService } from './co2-data-store.service';

export class CarburantiGridResult extends KendoServerResult {
  constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: CarburantiCO2Model[]) {
    super(model, columns, rows as any[]);
  }
}

export class CarburantiGridModel extends KendoGridModel {
  row_key: ModelEntry;
  azienda: ModelEntry;
  azienda_piva: ModelEntry;
  tipo_carburante: ModelEntry;
  tipo_carburante_des: ModelEntry;
  quantita: ModelEntry;
  unita_misura: ModelEntry;
  unita_misura_des: ModelEntry;
}

@Injectable()
export class CarburantiGridConfigService extends AbstractGridConfigService<CarburantiGridResult> {

  gridId = 'carburanti-co2-grid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'row_key';

  private rows: CarburantiCO2Model[] = [];

  private readonly unitaMisuraOptions: Array<{ id: string; name: string }> = [
    { id: 'L',  name: 'Litri' },
    { id: 'm3', name: 'Metri Cubi' },
    { id: 'T',  name: 'Tonnellate' }
  ];

  readonly gridModel: CarburantiGridModel = {
    row_key:           new ModelEntry(CELL_TYPES.STRING, false),
    azienda:           new ModelEntry(CELL_TYPES.STRING, false),
    azienda_piva:      new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    tipo_carburante:   new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    tipo_carburante_des: new ModelEntry(CELL_TYPES.STRING, false),
    quantita:          new ModelEntry(CELL_TYPES.NUMBER, true),
    unita_misura:      new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    unita_misura_des:  new ModelEntry(CELL_TYPES.STRING, false)
  };

  readonly kendoColumns: KendoGridColumn[] = [
    new KendoGridColumn(
      { field: 'row_key', title: '' },
      { hidden: true, editable: false, width: 0 }
    ),
    new KendoGridColumn(
      { field: 'azienda_piva', title: this.translocoService.translate('gco2_column_azienda') },
      { resizable: true, editable: true, width: 200 }
    ),
    new KendoGridColumn(
      { field: 'tipo_carburante', title: this.translocoService.translate('gco2_column_tipo_carburante') },
      { resizable: true, editable: true, width: 160 }
    ),
    new KendoGridColumn(
      { field: 'quantita', title: this.translocoService.translate('gco2_column_quantita') },
      {
        resizable: true,
        editable: true,
        width: 110,
        filter: 'numeric',
        numeric: {},
        validators: [Validators.required, Validators.min(0.01)]
      }
    ),
    new KendoGridColumn(
      { field: 'unita_misura', title: this.translocoService.translate('gco2_column_unita_misura') },
      { resizable: true, editable: true, width: 130 }
    )
  ];

  constructor(
    protected injector: Injector,
    protected translocoService: TranslocoService,
    private ajaxService: AjaxAgronicaNetCore6ApiService,
    private co2DataStore: Co2DataStoreService,
    private giasMessageService: GiasMessageService
  ) {
    super(injector, ConfigTemplate.DefaultTemplate);

    this.toolbar = new ToolbarSettings();
    this.toolbar.newItem = true;
    this.toolbar.resetChanges = false;

    this.cmdColumn = new CommandsColumnSettings({
      editBtn: true,
      infoBtn: false,
      removeBtn: true
    });

    this.views.enabled = false;
    this.groups.groupable.enabled = false;
    this.columnMenu.columnMenu = false;

    this.initDropdowns();
  }

  read(): Observable<CarburantiGridResult> {
    this.co2DataStore.setCarburantiSnapshot(this.rows);
    return of(new CarburantiGridResult(this.gridModel, this.kendoColumns, this.rows));
  }

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    const item: CarburantiCO2Model = Array.isArray(items) ? items[0] : items;
    switch (actionType) {
      case HttpAction.CREATE: {
        const newPiva = item.azienda_piva ?? this.co2DataStore.aziende[0]?.piva ?? '';
        const newTipo = item.tipo_carburante ?? '';
        if (this.rows.some(r => r.azienda_piva === newPiva && r.tipo_carburante === newTipo)) {
          this.giasMessageService.errorMessage(this.translocoService.translate('gco2_error_duplicato_carburante'));
          return of([]);
        }
        const aziendaFound = this.co2DataStore.aziende.find(a => a.piva === newPiva);
        const newRow: CarburantiCO2Model = {
          ...item,
          row_key: `carb_${Date.now()}_${Math.random().toString(36).slice(2, 7)}`,
          azienda: aziendaFound?.azienda ?? item.azienda ?? '',
          azienda_piva: newPiva,
          tipo_carburante_des: item.tipo_carburante ?? '',
          unita_misura_des: this.unitaMisuraOptions.find(o => o.id === item.unita_misura)?.name ?? item.unita_misura ?? ''
        };
        this.rows = [...this.rows, newRow];
        this.co2DataStore.setCarburantiSnapshot(this.rows);
        break;
      }
      case HttpAction.UPDATE: {
        const idx = this.rows.findIndex(r => r.row_key === item.row_key);
        if (idx >= 0) {
          const newPiva = item.azienda_piva ?? this.rows[idx].azienda_piva;
          const newTipo = item.tipo_carburante ?? this.rows[idx].tipo_carburante;
          if (this.rows.some(r => r.row_key !== item.row_key && r.azienda_piva === newPiva && r.tipo_carburante === newTipo)) {
            this.giasMessageService.errorMessage(this.translocoService.translate('gco2_error_duplicato_carburante'));
            return of([]);
          }
          const aziendaFound = this.co2DataStore.aziende.find(a => a.piva === newPiva);
          this.rows[idx] = {
            ...this.rows[idx],
            ...item,
            azienda: aziendaFound?.azienda ?? this.rows[idx].azienda,
            azienda_piva: newPiva,
            tipo_carburante_des: item.tipo_carburante ?? this.rows[idx].tipo_carburante_des,
            unita_misura_des: this.unitaMisuraOptions.find(o => o.id === item.unita_misura)?.name ?? this.rows[idx].unita_misura_des
          };
        }
        this.co2DataStore.setCarburantiSnapshot(this.rows);
        break;
      }
      case HttpAction.REMOVE: {
        this.rows = this.rows.filter(r => r.row_key !== item.row_key);
        this.co2DataStore.setCarburantiSnapshot(this.rows);
        break;
      }
    }
    return of([]);
  }

  private initDropdowns(): void {
    // Azienda — lista statica dalle righe selezionate nel perimetro
    const aziendaCol = this.kendoColumns.find(c => c.field === 'azienda_piva');
    aziendaCol.ddl = new DropdownListWithForm('piva', 'azienda_piva', 'azienda', []);
    aziendaCol.ddl.valuePrimitive = true;
    aziendaCol.ddl.descriptionField = 'azienda';
    aziendaCol.ddl.loadOnEdit = true;
    aziendaCol.ddl.loadFunction = () => {
      const subj = new Subject<any[]>();
      setTimeout(() => {
        subj.next(this.co2DataStore.aziende as unknown as any[]);
        subj.complete();
      });
      return subj;
    };

    // Tipo Carburante — lazy load on edit
    const tipoCol = this.kendoColumns.find(c => c.field === 'tipo_carburante');
    tipoCol.ddl = new DropdownListWithForm('Etichetta', 'tipo_carburante', 'Etichetta', []);
    tipoCol.ddl.valuePrimitive = true;
    tipoCol.ddl.descriptionField = 'tipo_carburante_des';
    tipoCol.ddl.loadOnEdit = true;
    tipoCol.ddl.loadFunction = this.loadTipiCarburante.bind(this);

    // Unità Misura — static list with loadOnEdit to activate descriptionField fallback
    const unitaCol = this.kendoColumns.find(c => c.field === 'unita_misura');
    unitaCol.ddl = new DropdownListWithForm('id', 'unita_misura', 'name', this.unitaMisuraOptions as unknown as DropdownListItem[]);
    unitaCol.ddl.valuePrimitive = true;
    unitaCol.ddl.descriptionField = 'unita_misura_des';
    unitaCol.ddl.loadOnEdit = true;
    unitaCol.ddl.loadFunction = () => {
      const subj = new Subject<any[]>();
      setTimeout(() => {
        subj.next(this.unitaMisuraOptions as unknown as any[]);
        subj.complete();
      });
      return subj;
    };
  }

  private loadTipiCarburante(): Observable<TipoCarburanteItem[]> {
    return this.ajaxService.ajaxAPIGet<object, TipoCarburanteItem[]>(
      '/v1/sostenibilita/tipi-carburante',
      {}
    ).pipe(map(r => r.RispostaStringa as unknown as TipoCarburanteItem[]));
  }
}
