/* eslint-disable */
import { Component, ElementRef, InjectionToken, Type } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { Event } from '@angular/router';
import {
  AddEvent,
  CancelEvent,
  CellClickEvent,
  CellCloseEvent, ColumnBase, ColumnVisibilityChangeEvent, EditEvent,
  GridComponent,
  SelectionEvent
} from '@progress/kendo-angular-grid';
import { BehaviorSubject, Subject } from 'rxjs';
import { ColumnSettings } from '../components/grid-customizations/model';
import { AbstractGridConfigService } from '../services/grid-config.service';
import { State } from "@progress/kendo-data-query";
import { AGRODATAFINE, AGRODATAINIZIO } from '../../../shared/CostantiPersonalizzate';
import { CustomComponent } from './custom-component';
import { CELL_TYPES } from 'gias-ui-kit';

export const GRID_HTTP_TOKEN = new InjectionToken<AbstractGridConfigService<KendoServerResult>>('app.grid.http');
export abstract class GridParentComponent extends Component {
}

export interface OnBeforeAfterEvents {
  /**
   * La funzione viene chiamata ogni volta si clicca su una cella.
   * Funzione chiamata in in cell editing.
   * @param event L'evento kendo grid.
   */
  onCellClick(event: CellClickEvent): void;

  /**
   * La funzione viene chiamata ogni volta si chiude una cella.
   * Funzione chiamata in in cell editing.
   * @param event L'evento kendo grid.
   */
  onCellClose(event: CellCloseEvent): void;

  /**
   * La funzione viene chiamata ogni volta che si deve creare un formGroup e si ha
   * necessita di crearlo dall'esterno della kendo grid.
   * @param event L'evento kendo grid.
   */
  onCreateExternalFormGroup(event: CellClickEvent | AddEvent | EditEvent): FormGroup;
}

/**
 * Modello Colonne KendoGridColumn
 * N.B. Se si deve aggiungere una nuova proprietà in questa classe che non è compresa tra quelle
 * standard di kendo(ColumnSettings) ricordarsi di aggiungere la nuova proprietà da memorizzare nelle viste della Griglia anche
 * nella funzione GridCustomizationsService.manageCellTypes (prendere come esempio showHTMLAsString).
 */
export class KendoGridColumn implements ColumnSettings {
  class?: string[] = null;
  ddl?: DropdownListWithForm = null;
  numeric?: NumericSettings = null;
  boolean?: BooleanSettings = null;
  date?: DateSettings = null;
  string?: StringSettings = null;
  validators?: any[] = [];
  editable: boolean = null;
  resizable: boolean = null;
  sticky: boolean = true;

  field: string = null;
  title: string = null;
  width: number = null;
  hidden: boolean = null;
  disabledRule: (rowData: any) => boolean;

  filter: 'text' | 'boolean' | 'numeric' | 'date' | 'datetime';
  filterOrdering: (rows: any) => any;
  format: string = null;
  filterable: boolean = null;
  orderIndex: number = null;
  leafIndex: number = null;
  includeInChooser: boolean = null;
  showHTMLAsString: boolean = null;
  media: string = null;
  customParent: string = null;
  customFooter: (rows: KendoGridRow) => string;

  style: { [key: string]: string } = null;
  footerStyle: { [key: string]: string } = null;
  component: Type<CustomComponent>;

  constructor(required: { field: string; title: string }, optional?: Partial<KendoGridColumn>) {
    this.class = optional?.class || null;
    this.ddl = optional?.ddl || null;
    this.numeric = optional?.numeric || null;
    this.boolean = optional?.boolean || null;
    this.date = optional?.date || null;
    this.string = optional?.string || null;
    this.validators = optional?.validators || null;
    this.editable = optional?.editable ?? true;
    this.resizable = optional?.resizable ?? true;

    this.field = required.field;
    this.title = required.title;
    this.width = optional?.width;// || 180;
    this.hidden = optional?.hidden ?? false;

    this.filter = optional?.filter || 'text';
    this.format = optional?.format || null;
    this.filterable = optional?.filterable ?? true;
    this.orderIndex = optional?.orderIndex || 0;
    this.leafIndex = optional?.leafIndex || 0;
    this.includeInChooser = optional?.includeInChooser ?? true;
    this.showHTMLAsString = optional?.showHTMLAsString ?? false;
    this.component = optional?.component ?? null;
    this.media = optional?.media ?? null;
    this.sticky = optional?.sticky ?? false;
    this.style = optional?.style || null;
    this.customParent = optional?.customParent || null;
    this.customFooter = optional?.customFooter || null;
    this.disabledRule = optional?.disabledRule || (() => false);
    this.filterOrdering = optional?.filterOrdering;
  }

}

export interface TableData {
  kendo_model: KendoGridModel;
  kendo_columns: KendoGridColumn[];
  kendo_rows: [];
}

export class KendoGridModel {
  [key: string]: ModelEntry;
}

export class ModelEntry {
  constructor(
    public type: string | CELL_TYPES,
    public editable?: boolean,
    public format?: string,
    public validators?: Array<Validators>,
    public editor?: (container: any) => any) { }
}


export abstract class KendoServerResult {
  constructor(public model: KendoGridModel,
    public columns: KendoGridColumn[],
    public rows: KendoGridRow[]) {
    const newModel: KendoGridModel = this.model;

    for (const prop in newModel) {
      if (Object.prototype.hasOwnProperty.call(newModel, prop)) {
        if (!newModel[prop]) {
          continue;
        }
        if (typeof (newModel[prop]?.type) === 'string') {
          continue;
        }

        switch (newModel[prop].type) {
          case 'date':
            newModel[prop].type = CELL_TYPES.DATE;
            break;
          case 'datetime':
            newModel[prop].type = CELL_TYPES.DATETIME;
            break;
          case 'number':
            newModel[prop].type = CELL_TYPES.NUMBER;
            break;
          case 'dropdownlist':
            newModel[prop].type = CELL_TYPES.DROPDOWNLIST;
            break;
          case 'string':
            newModel[prop].type = CELL_TYPES.STRING;
            break;
          default:
            alert('Operation type not yet implemented (value: ' + newModel[prop].type + ')');
        }
      }
    }
    this.model = newModel;
  }
}

export class KendoServerResultImpl extends KendoServerResult {
  constructor(model, columns, rows) {
    super(model, columns, rows);
  }
}

export class ServerResult {
  public kendo_rows: KendoGridRow[];
  public kendo_columns: KendoGridColumn[];
  public kendo_model: KendoGridModel;
}

export class GridInfoCommandEvent {
  action: string;
  dataItem: any;
  isNew: boolean;
  rowIndex: number;
  sender: GridComponent;
}

export class NumericSettings {
  defaultValue?: number;
  min?: number;
  max?: number;
  format?: string;
  autoCorrect?: boolean;
  decimals?: number;
  step?: number;
  multiCheckFiltering?: boolean

  constructor(settings?: NumericSettings) {
    this.defaultValue = settings?.defaultValue || 0;
    this.min = settings?.min || 0;
    this.max = settings?.max || Number.MAX_VALUE;
    this.format = settings?.format || 'n2';
    this.autoCorrect = settings?.autoCorrect ?? false;
    this.decimals = settings?.decimals || null;
    this.step = settings?.step || null;
    this.multiCheckFiltering = settings?.multiCheckFiltering || false;
  }
}

export class DateSettings {
  defaultValue?: Date;
  min?: Date;
  max?: Date;
  format?: string;
  placeholder?: string;

  constructor(opts?: DateSettings) {
    this.defaultValue = opts?.defaultValue || new Date();
    this.min = opts?.min || AGRODATAINIZIO;
    this.max = opts?.max || AGRODATAFINE;
    this.format = opts?.format || 'dd/MM/yyyy';
    this.placeholder = opts?.placeholder || '';
  }
}

export class DateTimeSettings {
  defaultValue?: Date;
  min?: Date;
  max?: Date;
  format?: string;
  placeholder?: string;

  constructor(opts?: DateTimeSettings) {
    this.defaultValue = opts?.defaultValue || new Date();
    this.min = opts?.min || AGRODATAINIZIO;
    this.max = opts?.max || AGRODATAFINE;
    this.format = opts?.format || 'dd/MM/yyyy HH:mm:ss';
    this.placeholder = opts?.placeholder || '';
  }
}

export class StringSettings {
  defaultValue?: string;

  constructor(settings?: StringSettings) {
    this.defaultValue = settings?.defaultValue || '';
  }
}

export class BooleanSettings {
  defaultValue?: boolean;
  /** Used together with rightLable to define the labels next to the switch component. */
  leftLabel?: string = "";
  /** Used together with leftLable to define the labels next to the switch component. */
  rightLabel?: string = "";
  constructor(settings?: BooleanSettings) {
    this.defaultValue = settings?.defaultValue || false;
  }
}

export class GridCustomizations {
  enabled: boolean;
  isSaveDisabledOnDefaultView: boolean;

  constructor(options?: Partial<GridCustomizations>) {
    this.enabled = options?.enabled || false;
    this.isSaveDisabledOnDefaultView = options?.isSaveDisabledOnDefaultView || false;
  }
}


export class DropdownList {
  public readonly textField: string = 'name';
  public readonly valueField: string = 'id';
  public valuePrimitive = true;
  public loadOnEdit = false;
  public descriptionField = '';
  public loadFunction: (dataItem: any) => Subject<any[]>;
  public itemDisabledFn: (i: any) => boolean = () => false;
  public loading: boolean = false;

  constructor(
    public id: string,
    public data: DropdownListItem[],
    public defaultValue?: DropdownListItem,
    public defaultOpen?: boolean
  ) {
    // this.data.unshift(new DropdownListItem("-1", "Predefinito"));
  }
}

/**
 * Una dropdown list usata dentro la kendo griglia.
 * @param id - Codice che identifica la dropdown in modo univoco nella pagina.
 * @param formControlName - il nome del campo creato nel formGroup corrispondente a una riga.
 * Ripresenta il codice che identifica univocamente ogni voce della dropdown.
 * @param formControlValue - la descrizione che compare nella cella della dropdown list.
 */
export class DropdownListWithForm extends DropdownList {
  constructor(
    id: string,
    public formControlName: string,
    public formControlValue: string,
    data: DropdownListItem[],
    defaultValue?: DropdownListItem,
    defaultOpen?: boolean,
    public reload: Subject<Boolean> = new Subject<Boolean>()
  ) {
    super(id, data, defaultValue, defaultOpen);
    reload.next(false);
  }
}

export class DropdownListItem {
  constructor(
    public readonly id: any,
    public readonly name: string,
    public readonly data?: any) { }
}


export class KendoGridRow {
  // todo
  //[prop: string]: string | number | DropdownListItem | Date;
  // Selected: boolean
}


export class GridEvent {
  constructor(public event: Event, public index: number) { }
}


export enum EditingMode {
  IN_LINE = 'in-line',
  IN_CELL = 'in-cell',
  IN_PAGE = 'in-page',
  IN_LINE_BATCH = "in-line-batch",
  IN_CELL_BATCH = "in-cell-batch",
}

export interface InCellBatchSaveEventObject {
  added: any[];
  updated: any[];
  deleted: any[];
}

export enum LoaderType {
  RESOLVER = 'resolver',
  HTML = 'html',
  SERVICE = 'service'
}


export class JsonKendoResult {
  public kendo_model: KendoGridModel;
  public kendo_rows: KendoGridRow[];
  public kendo_columns: KendoGridColumn[];
}

export class SelectedOpts {
  keys: string[] | number[];
  usePartialMatch?: boolean = false;
  resetPreviousSelection?: boolean = false;
}

export class NextSelection {
  value: SelectionEvent;
}

export class SelectionSettings {
  private _setSelected: BehaviorSubject<SelectedOpts> = new BehaviorSubject({ keys: [] })
  getSelectedValue: BehaviorSubject<NextSelection> = new BehaviorSubject(null);

  get setSelected() {
    return this._setSelected;
  }

  set setSelected(value) {
    this._setSelected = value;
  }
}

export class CustomizedColumnComponent extends ColumnBase {
  applyAutofit: boolean;
  field: string;
}

/**
 * These event types are emited each time:
 *   ReadEvent - the grid performs a read operation.
 *   ColumnVisibilityChangeEvent - a collumn becomes visible.
 *   CellCloseEvent - a cell is closed (in cell)
 *   CancelEvent - a row modification is canceled (in line)
 */
export type ReadEvent = { columns?: ColumnBase[] };
export type RendererGridEventType = ReadEvent | ColumnVisibilityChangeEvent | CellCloseEvent | CancelEvent | State;
export class RendererGridEvent {
  gridElRef: ElementRef;
  grid: GridComponent;
  event?: RendererGridEventType;
}
