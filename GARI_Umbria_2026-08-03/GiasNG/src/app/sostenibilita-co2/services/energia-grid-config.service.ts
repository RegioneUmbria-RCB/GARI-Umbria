import { Injectable, Injector } from '@angular/core';
import { Observable, of, Subject } from 'rxjs';
import { AbstractControl, ValidatorFn, Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import {
  AbstractGridConfigService,
  CommandsColumnSettings,
  ConfigTemplate,
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
import { EnergiaCO2Model } from '../models/sostenibilita-co2.model';
import { Co2DataStoreService } from './co2-data-store.service';

export class EnergiaGridResult extends KendoServerResult {
  constructor(model: KendoGridModel, columns: KendoGridColumn[], rows: EnergiaCO2Model[]) {
    super(model, columns, rows as any[]);
  }
}

export class EnergiaGridModel extends KendoGridModel {
  row_key: ModelEntry;
  azienda: ModelEntry;
  azienda_piva: ModelEntry;
  data_inizio: ModelEntry;
  data_fine: ModelEntry;
  consumo_kwh: ModelEntry;
  percentuale_rinnovabili: ModelEntry;
}

@Injectable()
export class EnergiaGridConfigService extends AbstractGridConfigService<EnergiaGridResult> {

  gridId = 'energia-co2-grid';
  loader: LoaderType = LoaderType.SERVICE;
  editingMode: EditingMode = EditingMode.IN_LINE;
  rowId = 'row_key';

  private rows: EnergiaCO2Model[] = [];

  private checkDates: ValidatorFn = (control: AbstractControl) => {
    const parent = control?.parent;
    if (!parent) { return null; }

    const dataInizio: Date | null = parent.get('data_inizio')?.value ?? null;
    const dataFine: Date | null = parent.get('data_fine')?.value ?? null;
    const rowKey: string = parent.get('row_key')?.value ?? null;

    // Regola 1: data_fine non deve essere minore di data_inizio
    if (dataInizio && dataFine && new Date(dataFine) < new Date(dataInizio)) {
      return { invalidDateRange: true };
    }

    // Regola 2: entrambe le date devono essere nell'anno del filtro
    const anno = this.co2DataStore.currentFilters?.anno;
    if (anno) {
      if (dataInizio && new Date(dataInizio).getFullYear() !== anno) {
        return { invalidAnno: true };
      }
      if (dataFine && new Date(dataFine).getFullYear() !== anno) {
        return { invalidAnno: true };
      }
    }

    // Regola 3: nessun'altra riga deve avere lo stesso intervallo per la stessa azienda
    if (dataInizio && dataFine) {
      const piva: string = parent.get('azienda_piva')?.value ?? '';
      const inizio = new Date(dataInizio).toDateString();
      const fine = new Date(dataFine).toDateString();
      const duplicate = this.rows.some(r =>
        r.row_key !== rowKey &&
        r.azienda_piva === piva &&
        r.data_inizio != null && new Date(r.data_inizio).toDateString() === inizio &&
        r.data_fine != null && new Date(r.data_fine).toDateString() === fine
      );
      if (duplicate) {
        return { duplicateInterval: true };
      }
    }

    // Reset degli errori sull'altro campo date quando quello corrente è valido
    const other = control === parent.get('data_inizio')
      ? parent.get('data_fine')
      : parent.get('data_inizio');
    if (other?.hasError('invalidDateRange') || other?.hasError('invalidAnno') || other?.hasError('duplicateInterval')) {
      other.setErrors(null);
    }

    return null;
  };

  readonly gridModel: EnergiaGridModel = {
    row_key:                new ModelEntry(CELL_TYPES.STRING, false),
    azienda:                new ModelEntry(CELL_TYPES.STRING, false),
    azienda_piva:           new ModelEntry(CELL_TYPES.DROPDOWNLIST, true),
    data_inizio:            new ModelEntry(CELL_TYPES.DATE, true),
    data_fine:              new ModelEntry(CELL_TYPES.DATE, true),
    consumo_kwh:            new ModelEntry(CELL_TYPES.NUMBER, true),
    percentuale_rinnovabili: new ModelEntry(CELL_TYPES.NUMBER, true)
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
      { field: 'data_inizio', title: this.translocoService.translate('gco2_column_data_inizio') },
      {
        resizable: true,
        editable: true,
        width: 130,
        filter: 'date',
        format: 'dd/MM/yyyy',
        date: { defaultValue: null },
        validators: [Validators.required, this.checkDates]
      }
    ),
    new KendoGridColumn(
      { field: 'data_fine', title: this.translocoService.translate('gco2_column_data_fine') },
      {
        resizable: true,
        editable: true,
        width: 130,
        filter: 'date',
        format: 'dd/MM/yyyy',
        date: { defaultValue: null },
        validators: [Validators.required, this.checkDates]
      }
    ),
    new KendoGridColumn(
      { field: 'consumo_kwh', title: this.translocoService.translate('gco2_column_consumo_kwh') },
      {
        resizable: true,
        editable: true,
        width: 130,
        filter: 'numeric',
        numeric: {},
        validators: [Validators.required, Validators.min(0.01)]
      }
    ),
    new KendoGridColumn(
      { field: 'percentuale_rinnovabili', title: this.translocoService.translate('gco2_column_percentuale_rinnovabili') },
      {
        resizable: true,
        editable: true,
        width: 180,
        filter: 'numeric',
        numeric: {},
        validators: [Validators.required, Validators.min(0), Validators.max(100)]
      }
    )
  ];

  constructor(
    protected injector: Injector,
    protected translocoService: TranslocoService,
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

  read(): Observable<EnergiaGridResult> {
    this.co2DataStore.setEnergiaSnapshot(this.rows);
    return of(new EnergiaGridResult(this.gridModel, this.kendoColumns, this.rows));
  }

  private toDay = (d: any): string => d ? new Date(d).toDateString() : '';

  perform(actionType: HttpAction, items: any): Observable<any[]> {
    const item: EnergiaCO2Model = Array.isArray(items) ? items[0] : items;
    switch (actionType) {
      case HttpAction.CREATE: {
        const newPiva = item.azienda_piva ?? this.co2DataStore.aziende[0]?.piva ?? '';
        const newInizio: Date | null = item.data_inizio ?? null;
        const newFine: Date | null = item.data_fine ?? null;
        if (newInizio && newFine && this.rows.some(r =>
          r.azienda_piva === newPiva &&
          this.toDay(r.data_inizio) === this.toDay(newInizio) &&
          this.toDay(r.data_fine) === this.toDay(newFine)
        )) {
          this.giasMessageService.errorMessage(this.translocoService.translate('gco2_error_duplicato_energia'));
          return of([]);
        }
        const aziendaFound = this.co2DataStore.aziende.find(a => a.piva === newPiva);
        const newRow: EnergiaCO2Model = {
          ...item,
          row_key: `en_${Date.now()}_${Math.random().toString(36).slice(2, 7)}`,
          azienda: aziendaFound?.azienda ?? item.azienda ?? '',
          azienda_piva: newPiva,
        };
        this.rows = [...this.rows, newRow];
        this.co2DataStore.setEnergiaSnapshot(this.rows);
        break;
      }
      case HttpAction.UPDATE: {
        const idx = this.rows.findIndex(r => r.row_key === item.row_key);
        if (idx >= 0) {
          const newPiva = item.azienda_piva ?? this.rows[idx].azienda_piva;
          const newInizio: Date | null = item.data_inizio ?? this.rows[idx].data_inizio ?? null;
          const newFine: Date | null = item.data_fine ?? this.rows[idx].data_fine ?? null;
          if (newInizio && newFine && this.rows.some(r =>
            r.row_key !== item.row_key &&
            r.azienda_piva === newPiva &&
            this.toDay(r.data_inizio) === this.toDay(newInizio) &&
            this.toDay(r.data_fine) === this.toDay(newFine)
          )) {
            this.giasMessageService.errorMessage(this.translocoService.translate('gco2_error_duplicato_energia'));
            return of([]);
          }
          const aziendaFound = this.co2DataStore.aziende.find(a => a.piva === newPiva);
          this.rows[idx] = {
            ...this.rows[idx],
            ...item,
            azienda: aziendaFound?.azienda ?? this.rows[idx].azienda,
            azienda_piva: newPiva,
          };
        }
        this.co2DataStore.setEnergiaSnapshot(this.rows);
        break;
      }
      case HttpAction.REMOVE: {
        this.rows = this.rows.filter(r => r.row_key !== item.row_key);
        this.co2DataStore.setEnergiaSnapshot(this.rows);
        break;
      }
    }
    return of([]);
  }

  private initDropdowns(): void {
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
  }
}
