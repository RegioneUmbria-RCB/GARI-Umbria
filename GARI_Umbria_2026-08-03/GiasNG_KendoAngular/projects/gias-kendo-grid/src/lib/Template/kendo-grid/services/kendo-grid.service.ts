import { ElementRef, Inject, Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AddEvent, CellClickEvent, CellCloseEvent, EditEvent } from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { LocalStorageService } from 'ngx-webstorage';
import { EMPTY, from, Observable, onErrorResumeNext, ReplaySubject, Subject, take, zip } from 'rxjs';
import { catchError, concatAll, filter, map, skip, takeUntil, tap } from 'rxjs/operators';
import { NextDropdownValue } from '../components/grid-dropdownlists/grid-dropdown.service';
import { DeletionMode, GeneralSettings, MasterDetailSettings } from '../models/configuration.model';
import {
  DropdownListItem,
  EditingMode,
  GRID_HTTP_TOKEN,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult as GridServerResult,
} from '../models/grid.model';
import { AbstractGridConfigService, HttpAction } from './grid-config.service';
import { GridErrorService } from './grid-log.service';
import { GridPublicService } from './grid-public.service';
import { GridRootHelper } from './grid-root-helper.service';
import { _isNull } from './utilities';
import { TranslocoService } from "@jsverse/transloco";
import '@progress/kendo-angular-intl/locales/fr/calendar'
import '@progress/kendo-angular-intl/locales/pt/calendar'
import { State } from "@progress/kendo-data-query";
import { cloneDeep } from 'lodash';
import { GridDataWithFilter } from './grid-data-with-filter';
import { CELL_TYPES, DropdownEventType, DropdownListEvent } from 'gias-ui-kit';

/**
 * Various section:
 * - [Section] Grid state
 */

const itemIndex = (item: any, data: any[], chiave: string): number => {
  for (let idx = 0; idx < data.length; idx++) {
    if (data[idx][chiave] === item[chiave]) {
      return idx;
    }
  }

  return -1;
};

const itemIndexSimple = (data: any[], chiave: string, id: string): number => {
  for (let idx = 0; idx < data.length; idx++) {
    if (data[idx][id] === chiave) {
      return idx;
    }
  }

  return -1;
};

const cloneData = (data: KendoGridRow[] | KendoGridColumn[]) => {
  if (!data) {
    return [];
  }
  return data.map((item) => Object.assign({}, item));
};

type LoadFnResult = { loadFn: (_) => Observable<any>, loadOnEdit: boolean, col: KendoGridColumn };

@Injectable()
export class KendoGridService extends ReplaySubject<GridServerResult> {

  private getLoadFn(controlName: string): LoadFnResult {
    let col = this.kData.columns.find(s => s.field === controlName);

    if (col.ddl == null)
      return null;

    return { loadFn: col.ddl.loadFunction, loadOnEdit: col.ddl.loadOnEdit, col: col };
  }

  nextDropdownValue(controlName: string, value: any, formGroup: FormGroup): void {
    let lazyLoad: LoadFnResult = this.getLoadFn(controlName);
    let data: NextDropdownValue = {
      controlName: controlName, value: value,
      loadData: lazyLoad.loadFn, formGroup: formGroup,
      loadOnEdit: lazyLoad.loadOnEdit,
      currentRow: this.pubService.currentDataItem,
      column: lazyLoad.col
    };

    this.gridRootHelper.nextDropdownValue.next(data);
  }
  clearKendoGridService() {
    this._gridCtx = null;
    this.dropdownListSubject.complete();
    this.dropdownListSubject = new Subject();
    this.updateDropdownColumn.complete();
    this.updateDropdownColumn = new Subject();
    this.originalColumns = [];
    this.originalModel = new KendoGridModel();
    this.originalData = [];
    this.createdItems = [];
    this.updatedItems = [];
    this.deletedItems = [];
    this.multiIndex = null;
    this.kData = { rows: [], model: {}, columns: [] };
    this.pubService = null;
    this.freeMultiIndex = true;
    this.multiIndex = null;
    this.conf = null;
    this.customizationRowsSubject = new Subject();
    this.signal = null;
  }

  /** Component context */
  _gridCtx: any;
  /**
   * Subjects for emitting events
   **/
  dropdownListSubject: Subject<DropdownListEvent> = new Subject();
  /**
   * Non sottoscrivere a questo subject. Dovrebbe essere usato soltanto nel
   * questo modulo.
   */
  updateDropdownColumn: Subject<DropdownListEvent> = new Subject();

  /**
   *  In cell editing mode variables
   **/
  private originalColumns: KendoGridColumn[] = [];

  private originalModel: KendoGridModel = new KendoGridModel();
  private originalData: KendoGridRow[] = [];
  private createdItems: KendoGridRow[] = [];
  private updatedItems: KendoGridRow[] = [];
  private deletedItems: KendoGridRow[] = [];

  public clone<T>(obj: T): T {
    return  cloneDeep(obj);
  }

  /**
   * In line editing mode
   */
  private kData: GridServerResult = { rows: [], model: {}, columns: [] };
  public pubService: GridPublicService;

  public freeMultiIndex = true;
  public multiIndex: number;

  @Inject(GRID_HTTP_TOKEN)
  public conf: AbstractGridConfigService<GridServerResult>;
  constructor(
    @Inject(GRID_HTTP_TOKEN)
    public multiConfigs: AbstractGridConfigService<GridServerResult>,
    private transloco: TranslocoService,
    public pubServices: GridPublicService,
    private logService: GridErrorService,
    private intlService: IntlService,
    private gridRootHelper: GridRootHelper,
    private localStorage: LocalStorageService
  ) {
    super(1);

    if (!Array.isArray(this.pubServices)) {
      this.pubService = this.pubServices;
      this.conf = this.multiConfigs;
      this.freeMultiIndex = false;
    }

  }

  public initProviders(ctx: any) {
    // Could be of type array when multi: true in the providers array of the
    // container.
    if (Array.isArray(this.pubServices)) {
      this.mapFreePubService();
      this.mapMultiToken(ctx);
      ctx.privateService = this;
      this.freeMultiIndex = false;
      this.multiIndex = this.pubService.multiIndex;
    }
  }

  onCellClick(event: CellClickEvent) {
    if (typeof this.conf.onCellClick === 'function') {
      this.conf.onCellClick(event);
    }
  }

  preventEdit(event: CellClickEvent): boolean {
    return this.conf.preventEdit(event.dataItem, event.column);
  }

  onCellClose(event: CellCloseEvent, inputElementRef: ElementRef) {
    if (typeof this.conf.onCellClose === 'function') {
      this.conf.onCellClose(event, inputElementRef);
    }
  }

  onCreateExternalFormGroup(event: CellClickEvent | AddEvent | EditEvent): FormGroup {
    let FormGroup: FormGroup = null;

    if (typeof this.conf.onCreateExternalFormGroup === 'function') {
      FormGroup = this.conf.onCreateExternalFormGroup(event);
    }

    return FormGroup;
  }

  public ngOnInit(component: any) {
    this.signal = component.unsubSignal;
    this._gridCtx = component;
    this.pubService.init(this);

    this.registerSubscriptions();

    this.initParams(component);

    if (this.conf.behavior?.excelSettings?.enabled) {
      component.excelData = component.excelData.bind(component);
    }

    this.pubService.ngOnInit(component);
  }

  private initParams(c: any) {
    this._gridCtx = c;
    c.loader = this.conf.loader;
    c.editingMode = this.conf.editingMode;
    c.selectable = this.conf.selectable;
    c.toolbar = this.conf.toolbar;
    c.cmdColumn = this.conf.cmdColumn;
    c.cmdDropDown = this.conf.cmdDropDown;
    c.dettagliColumn = this.conf.dettagliColumn;
    c.customColumn = this.conf.customColumn;
    c.resizable = this.conf.resizable;
    c.generalSettings = this.clone(this.conf.generalSettings);

    if (!this.conf.masterdetailSettings) {
      c.masterdetailSettings = new MasterDetailSettings(false);
    } else {
      c.masterdetailSettings = this.conf.masterdetailSettings;
    }

    c.sort = this.conf.sort;
    c.aggregates = this.conf.aggregates;
    c.groups = this.conf.groups;

    c.columnMenu = this.conf.columnMenu;
    c.behavior = this.conf.behavior;
    c.key = this.conf.rowId;
    c.views = this.conf.views;
    c.pagination = this.clone(this.conf.pagination);
  }

  /** Read fresh data from the beginning of the read pipeline. */
  public idealRead(refresh: boolean, isPerformCallback = false, cols: KendoGridColumn[] = [], afterEdit: boolean = false) { // < ideale in quanto la tabella deve essere initializzata
    this._read(refresh, isPerformCallback, cols, afterEdit);
  }

  private _read(forceRefresh: boolean = false, isPerformCallback = false, cols: KendoGridColumn[] = [], afterEdit: boolean = false) {

    switch (this.conf.editingMode) {
      case EditingMode.IN_CELL:
      case EditingMode.IN_LINE:
      case EditingMode.IN_PAGE:
      case EditingMode.IN_LINE_BATCH:
      case EditingMode.IN_CELL_BATCH:
        this.allPurposeRead(forceRefresh, isPerformCallback, cols, afterEdit);
        break;
      default:
        this.logService.logErr('Something went wrong, could not resolve editing mode: ' + this.conf.editingMode);
    }
  }
  /**
   * isPerformCallback: è true quando si rileggono i dati appena perform è completato.
   */
  private allPurposeRead(forceRefresh: boolean, isPerformCallback: boolean, cols: KendoGridColumn[], afterEdit: boolean = false) {
    if (!forceRefresh && this.kData.rows?.length >= 0) {

      if (cols.length > 0) {
        cols.forEach(c => {
          let index = this.kData.columns.findIndex(x => x.field === c.field);

          if (index > -1) {
            Object.keys(c).forEach(p => {
              this.kData.columns[index][p] = c[p];
            });
          }

        })
      }

      return this.next(this.kData);
    }

    this.kData = { rows: [], model: {}, columns: [] };
    // Non necessario fare unsubscribe a una chiamata http
    this.conf.read(this.pubService?.Current_Grid_Master_RowExpanded)
      .pipe(
        tap((data: GridServerResult) => {
          this.logService.checkThis(this.logService.checkRequiredFields,
            this.kData.model, this.kData.columns, this.kData.rows);
        })
      )
      .pipe(
        map((data: GridServerResult): GridServerResult => {
          data = this.manageDates(data);
          return data;
        })
      )
      .subscribe(
        {
          next: (data: GridServerResult) => {
            this.storeDefaultColumnOrder(data.columns);
            this.storeDefaultGridState(this.conf.pagination.gridState);
            this._gridCtx.resetMulticheckStore();

            this.kData = data;
            this.originalData = cloneData(data?.rows);
            this.originalColumns = data?.columns;
            Object.assign(this.originalModel, data?.model);

            if (!afterEdit) {
              let cols1 = this.kData.columns.sort((a, b) => a.orderIndex - b.orderIndex);
              this.kData.columns = cols1;
            } else {
              this.kData.columns = this.pubService.giasGridComponent.columns;
            }
            // Update the public service with the new values.
            let nextData = new GridDataWithFilter({ data: this.kData, stopPropagation: true });
            this.pubService.next(nextData);

            this.decideNextBehavior(isPerformCallback);
          }
        }
      );
  }
  /**
   * Utilizzato soltanto dalle viste.
   * @param cols Le colonne inizialmente caricate dal backend.
   */
  private storeDefaultColumnOrder(cols: KendoGridColumn[]) {
    if (!this.defaultColumnsOrder) {
      this.defaultColumnsOrder = cloneData(this.sortColumnsByIndex(cols));
    }
  }

  private storeDefaultGridState(state: State) {
    if (!this.defaultGridState)
      this.defaultGridState = state;
  }
  /**
   *
   * @param isPerformCallback utilizzato per gestire il ricaricamento dei dati appena
   * perform è stato eseguito, in modo che le modifiche sono aggiornate nella griglia.
   */
  private decideNextBehavior(isPerformCallback: boolean) {

    if (!this.conf.views.enabled) {
      this.next(this.kData);
    } else {
      if (isPerformCallback)
        this.next(this.kData);
      else {
        this._gridCtx.model = this.kData.model;
        this._gridCtx.columns = this.kData.columns;
        this._gridCtx.customizationService.init(this); //
      }
    }
  }

  public manageDates(data: GridServerResult): GridServerResult {
    if (!data) {
      return null;
    }

    data.columns.forEach((col) => {
      if (data.model[col.field] == undefined) {
        throw Error('il modello non ha il campo: ' + col.field);
      }
    });

    // let columnsDate = data.columns.filter((col) => data.model[col.field].type === CELL_TYPES.DATE);
    // data.rows = data.rows.map(row => {
    //   columnsDate.forEach(c => {
    //     if (typeof row[c.field] == "string"){
    //       row[c.field] = this.intlService.parseDate(row[c.field], '', this.transloco.getActiveLang())
    //         || this.intlService.parseDate(row[c.field]);
    //     }
    //   });
    //   return row;
    // });
    //
    // let columnsDatetime = data.columns.filter((col) => data.model[col.field].type === CELL_TYPES.DATETIME);
    // data.rows = data.rows.map(row => {
    //   columnsDatetime.forEach(c => {
    //     if (typeof row[c.field] == "string"){
    //       row[c.field] = this.intlService.parseDate(row[c.field]);
    //     }
    //   });
    //   return row;
    // });
    //
    // let columns = data.columns.filter((col) => data.model[col.field].type === CELL_TYPES.DROPDOWNLIST);
    // // data.rows = data.rows.map(row => {
    // //     columns.forEach(c => {
    // //         row[c.field] = String(row[c.field]);
    // //     });
    // //     return row;
    // // });
    //
    //Imposto come defaultValue 0 per le colonne numeriche
    let mappedColumns = data.columns.map(c => {
      if (data.model[c.field].type === CELL_TYPES.NUMBER && c.numeric) {
        if (!c.numeric.defaultValue) {
          c.numeric.defaultValue = 0;
        }
      }
    });


    return data;
  }

  customizationRowsSubject: Subject<KendoGridRow[]> = new Subject();

  defaultColumnsOrder: KendoGridColumn[] = null;

  defaultGridState: State = null;

  public reReadRows(model: KendoGridModel, cols: KendoGridColumn[], applyServerColumns: boolean) {
    // console.log(JSON.stringify(cols));
    let columns: KendoGridColumn[];
    if (applyServerColumns) {
      columns = this.sortColumnsByIndex(this.defaultColumnsOrder);
    } else {
      columns = this.sortColumnsByIndex(cols);
    }
    if (this.kData.rows) {
      this.pubService.next({
        stopPropagation: false,
        forceRefresh: false,
        data: {
          model: model,
          columns: columns,
          rows: this.kData.rows
        },
        afterEdit: false
      });
      // this.next({ model: model, columns: this.defaultColumnsOrder, rows: this.kData.rows });
    }
  }

  sortColumnsByIndex(columns: KendoGridColumn[]) {
    return columns.sort((a, b) => a.orderIndex - b.orderIndex);
  }

  public identifyRowsToSelect(ids: string[]): number[] {
    const result: number[] = [];
    ids.forEach((chiave: string) => {
      const index = itemIndexSimple(this.kData.rows, chiave, this.conf.rowId);
      const valid = index != -1;
      if (valid) {
        result.push(index);
      } else {
        //console.log('Qualcosa è andato storto...');
      }
    });

    return result;
  }

  public applyDefaultView() {
    this.next({
      rows: this.originalData,
      columns: this.originalColumns,
      model: this.originalModel
    });
  }

  updatePubService() {
    this.pubService.next(
      {
        data: {
          rows: this.kData.rows,
          columns: this.kData.columns,
          model: this.kData.model
        },
        forceRefresh: false,
        afterEdit: false
      }
    )
  }

  public save(data: KendoGridRow, id: string, isNew: boolean, rowIndex: number): Observable<any> {
    if (this.conf.generalSettings.performOnEdit && this.conf.editingMode === EditingMode.IN_CELL) {
      throw Error("Save changes was triggered when changes are saved on cell modification");
    }

    switch (this.conf.editingMode) {
      case EditingMode.IN_LINE:
        return this.inlineSave(data, isNew, rowIndex);
      case EditingMode.IN_CELL:
        return this.incellSaveChanges();
      case EditingMode.IN_PAGE:
      case EditingMode.IN_LINE_BATCH:
      case EditingMode.IN_CELL_BATCH:
        return null;
      default:
        this.logService.logErr('Questa modalità di modifica non esiste (' + this.conf.editingMode + ').');
        return null;
    }
  }

  public incellCreate(item: KendoGridRow) {
    if (this.conf.generalSettings.performOnEdit)
      this.incellCreatePerformOnEdit(item);
    else
      this.incellCreateDefault(item);


    this.updatePubService();
  }

  private incellCreatePerformOnEdit(item: KendoGridRow) {
    this.kData.rows.unshift(item);
    this.next(this.kData);
    this.conf.perform(HttpAction.CREATE, [item]);

    this.updatePubService();
  }

  private incellCreateDefault(item: KendoGridRow) {
    this.createdItems.push(item);
    this.kData.rows.unshift(item);
    this.next(this.kData);

    this.updatePubService();
  }

  public inCellUpdate(item: KendoGridRow): void {
    if (this.conf.editingMode == EditingMode.IN_CELL_BATCH)
      this.incellUpdateBatchEdit(item);
    else if (this.conf.generalSettings.performOnEdit)
      this.incellUpdatePerformOnEdit(item);
    else
      this.incellUpdateDefault(item);


    if (this.conf.editingMode != EditingMode.IN_CELL_BATCH) {
      this.updatePubService();
    }
  }
  private incellUpdatePerformOnEdit(item: KendoGridRow) {
    if (!this.isNew(item)) {
      const index = itemIndex(item, this.kData.rows, this.conf.rowId);
      if (index !== -1) {
        this.kData.rows.splice(index, 1, item);
      }
    } else {
      const index = this.kData.rows.indexOf(item);
      this.kData.rows.splice(index, 1, item);
    }

    if (!this.isNew(item)) {
      this.conf.perform(HttpAction.UPDATE, [item]).subscribe(
        () => this._read(true, true),
        () => this._read(true, true)
      );
    } else {
      this.conf.perform(HttpAction.CREATE, [item]);
    }

  }

  private incellUpdateDefault(item: KendoGridRow) {
    if (!this.isNew(item)) {
      const index = itemIndex(item, this.updatedItems, this.conf.rowId);
      if (index !== -1) {
        this.updatedItems.splice(index, 1, item);
      } else {
        this.updatedItems.push(item);
      }
    } else {
      const index = this.createdItems.indexOf(item);
      this.createdItems.splice(index, 1, item);
    }
  }

  private incellUpdateBatchEdit(item: KendoGridRow): void {
    const type = this.isNew(item) ? HttpAction.CREATE : HttpAction.UPDATE;
    const ret: Observable<any> = this.conf.perform(type, [item]);
    if (ret) ret.pipe(take(1)).subscribe();
  }

  /**
   * @param row the selected rows. If only one row is passed and the deletion mode is different from `HandleSingleRowDeletionOnly`, it tries to reread all the selected rows.
   */
  public remove(row: KendoGridRow | KendoGridRow[]) {
    if (this.conf.behavior.deletionMode === DeletionMode.HandleSingleRowDeletionOnly) {
      this.doOneByOneDeletion(Array.isArray(row) ? row : [row]);
    }
    const rows = Array.isArray(row) ? row : this.getAllSelectedRows(row);
    if (this.conf.behavior.deletionMode === DeletionMode.HandleAllRowsTogether) {
      this.doBatchDeletion(rows);
    }
    else if (this.conf.behavior.deletionMode === DeletionMode.HandleEachRowSeparately) {
      this.doOneByOneDeletion(rows);
    }
  }

  getAllSelectedRows(row: KendoGridRow) {
    let rows = new Array<KendoGridRow>();

    this._gridCtx.rows.forEach(row => {
      if (row['Selected'])
        rows.push(row);
    });

    if (rows.find((r) => r['chiave'] == row['chiave']) == undefined) {
      rows.push(row);
    }

    return rows;
  }

  doBatchDeletion(rows: KendoGridRow[]): void {
    switch (this.conf.editingMode) {
      case EditingMode.IN_PAGE:
      case EditingMode.IN_LINE:
        this.inlineRemove(rows).subscribe(() => this._read(true, true));
        break;
      case EditingMode.IN_CELL:
      default:
        throw Error("Batch deletion not implemented yet for this editing mode: " + this.conf.editingMode.toString());
    }
  }

  /**
   * @param rows rows to delete
   * @history
   * **(12/03/2024)**: Add flag to reload grid data after deletion. This flag also helps to prevent the refresh of
   * grids with batch editing.
   */
  doOneByOneDeletion(rows: KendoGridRow[]): void {
    let obs: Observable<any>[] = [];
    let forceRead = true;

    rows.forEach((r) => {
      switch (this.conf.editingMode) {
        case EditingMode.IN_LINE:
          obs.push(this.inlineRemove(r));
          break;
        case EditingMode.IN_CELL:
          if (this.conf.generalSettings.performOnEdit) {
            this.incellRemovePerformOnEdit(r);
          }
          else { // batch editing
            let elemPendingDeletion = this.highlightRowDeletion(r);
            this.incellRemoveDefault(r, elemPendingDeletion);
            forceRead = false;
          }
          this.updatePubService();
          break;
        case EditingMode.IN_PAGE:
          obs.push(this.inlineRemove(r));
          break;
        case EditingMode.IN_LINE_BATCH:
          break;
        case EditingMode.IN_CELL_BATCH:
          break;
        default:
          this.logService.logErr('Questa modalità di modifica non esiste (' + this.conf.editingMode + ').');
      }
    });

    (from(onErrorResumeNext(obs)).pipe(
      concatAll(),
      catchError((e) => {
        console.log('Error', e);
        return EMPTY;
      })
    )).subscribe({
      next: null,
      error: (e) => console.log('error', e),
      complete: () => {
        if (forceRead) {
          this._read(true, true, [], true);
        }
      }
    });
  }

  private inlineRemove(row: KendoGridRow | KendoGridRow[]) {
    return this.conf.perform(HttpAction.REMOVE, row)
  }

  private incellRemovePerformOnEdit(item: KendoGridRow) {
    const index = this.kData.rows.indexOf(item);
    this.kData.rows.splice(index, 1);

    this.conf.perform(HttpAction.REMOVE, [item]);

  }

  itemsPreviouslyUpdated: KendoGridRow[] = [];
  private incellRemoveDefault(item: KendoGridRow, elemPendingDeletion: boolean) {
    let helper = new BatchEditingHelper();

    const prepareElementForDeletion = () => {
      let index: number;
      if (this.isNew(item)) {
        index = this.createdItems.findIndex(i => i === item);
      } else {
        index = itemIndex(item, this.createdItems, this.conf.rowId);
      }

      if (index >= 0) {
        this.createdItems.splice(index, 1);
        // Since the deletion of a newly created item is reflected immediatly after
        // reloading the page, we get rid of the item all together.
        index = itemIndex(item, this.kData.rows, this.conf.rowId);
        this.kData.rows.splice(index, 1);

      } else {
        this.deletedItems.push(item);
        index = itemIndex(item, this.updatedItems, this.conf.rowId);
        if (index >= 0) {
          this.itemsPreviouslyUpdated.push(item);
          this.updatedItems.splice(index, 1);
        }
      }
    }

    const undoElementDeletion = () => {
      let index = itemIndex(item, this.deletedItems, this.conf.rowId);

      if (index >= 0) {
        this.deletedItems.splice(index, 1);

        // if item was edited previously to being removed, update updatedItems array.
        if ((index = helper.itemWasModifiedPreviouslyToBeingRemoved(item, this.itemsPreviouslyUpdated)) >= 0) {
          this.itemsPreviouslyUpdated.splice(index, 1);
          this.updatedItems.push(item);
        }
      }
    }

    if (elemPendingDeletion)
      prepareElementForDeletion();
    else
      undoElementDeletion();

  }

  public highlightRowDeletion(row: KendoGridRow): boolean {
    if (row[this.conf.rowId] == null)
      return true;

    row['pending-deletion-row'] = !row['pending-deletion-row'];
    return row['pending-deletion-row'];
  }

  private inlineSave(data: KendoGridRow, isNew: boolean, rowIndex: number) {
    const action = isNew ? HttpAction.CREATE : HttpAction.UPDATE;

    let oldRow: KendoGridRow = null;

    if (action === HttpAction.UPDATE) {
      oldRow = this.pubService.getValue().data.rows[rowIndex];
    }

    this.generateNewRowId(data);

    this.reset();

    let obs: Observable<any> = this.conf.perform(action, data, oldRow);
    // obs.subscribe(
    //     () => this._read(true, true),
    //     () => this._read(true, true)
    // );
    return obs
  }

  batchSave(data: any): Observable<any> {
    return this.conf.perform(HttpAction.BATCH_SAVE, data)
  }

  /*
  * @description:
  * Aggiunge il campo chiave della riga se non è presente
  * */
  private generateNewRowId(data: KendoGridRow) {

    if (data) {
      if (Object.keys(data).findIndex(x => x === this.conf.rowId) === -1) {
        let key_values = this.kData.rows.map(r => r[this.conf.rowId]);

        if (key_values && key_values.length > 0) {
          if (typeof key_values[0] === "number") {
            let new_key = Math.max(...key_values) + 1;

            data[this.conf.rowId] = new_key;
          } else if (typeof key_values[0] === "string") {

            let new_key = "";

            for (let k of key_values) {
              //new_key =  (Math.random() + 1).toString(36).substring(7);
              new_key = window.crypto.getRandomValues(new Uint32Array(1))[0].toString(16);
              if (new_key !== k) {
                break;
              }
            }

            data[this.conf.rowId] = new_key;

          }
        }
      }
    }
  }

  /**
   *
   * @param gridGeneralSettings general settings of the grid
   * @history (09/04/2024): If the grid doesn't perform the actions on edit,
   * it creates a single event with all the interested rows. Rows can be sorted as follows:
   *  - toDelete: will have a hidden field (`pending-deletion-row`) valorized as true;
   *  - toCreate: will probably have the field `rowId` not valorized (see: {@link isNew})
   *  - toUpdate: all the others
   */
  public incellSaveChanges(gridGeneralSettings?: GeneralSettings): Observable<any> {
    if (!this.incellHasChanges()) {
      return null;
    }

    const completed = [];
    if (gridGeneralSettings && !gridGeneralSettings.performOnEdit) {
      const touchedRows = [...this.deletedItems, ...this.updatedItems, ...this.createdItems];
      completed.push(this.conf.perform(HttpAction.BATCH_SAVE, touchedRows));
    } else {
      if (this.deletedItems.length) {
        completed.push(this.conf.perform(HttpAction.REMOVE, this.deletedItems));
      }
      if (this.updatedItems.length) {
        completed.push(this.conf.perform(HttpAction.UPDATE, this.updatedItems));
      }
      if (this.createdItems.length) {
        completed.push(this.conf.perform(HttpAction.CREATE, this.createdItems));
      }
    }
    this.incellReset();
    return zip(...completed);
    //obs.subscribe(() => this._read(false, true));
  }

  public cancelChanges(): void {
    switch (this.conf.editingMode) {
      case EditingMode.IN_LINE:
        this.inlineCancelChanges();
        break;
      case EditingMode.IN_CELL:
        this.incellCancelChanges();
        break;
      case EditingMode.IN_PAGE:
        break;
      case EditingMode.IN_LINE_BATCH:
        this.incellCancelChanges();
        break;
      case EditingMode.IN_CELL_BATCH:
        this.incellCancelChanges();
        break;
      default:
        this.logService.logErr('Questa modalità di modifica non esiste (' + this.conf.editingMode + ').');
    }
  }

  private incellCancelChanges(): void {
    this.incellReset();
    this.originalData.filter(r => r['pending-deletion-row'])
      .forEach(r => r['pending-deletion-row'] = false);
    this.kData.rows = this.originalData;
    this.originalData = cloneData(this.originalData);
    this.next(this.kData);
  }

  private inlineCancelChanges() {
    //this.kData.rows = [];
    this.incellReset();

    this.kData.rows = this.originalData;
    this.originalData = cloneData(this.originalData);
    this.next(this.kData);
  }

  public reset() {
    switch (this.conf.editingMode) {
      case EditingMode.IN_LINE:
        this.inlineCancelChanges();
        break;
      case EditingMode.IN_CELL:
      case EditingMode.IN_CELL_BATCH:
        this.incellCancelChanges();
        break;
      case EditingMode.IN_PAGE:
        break;
      case EditingMode.IN_LINE_BATCH:
        break;
      default:
        this.logService.logErr('Questa modalità di modifica non esiste (' + this.conf.editingMode + ').');
    }
  }

  public inlineReset() {
    this.kData.rows = this.originalData;
    this.next(this.kData);
  }

  private incellReset() {
    this.kData.rows = [];
    this.deletedItems = [];
    this.updatedItems = [];
    this.createdItems = [];
  }

  public incellHasChanges(): boolean {
    return Boolean(
      this.deletedItems.length ||
      this.updatedItems.length ||
      this.createdItems.length
    );
  }

  /** Checks if the row is new.
   * @param item the row to check
   * @private
   * @return true if the field specified by `this.conf.rowId` is `null` or `undefined`
   */
  private isNew(item: KendoGridRow): boolean {
    return (item[this.conf.rowId] == null || item[this.conf.rowId] == undefined);
  }

  public assignValues(target: any, source: any) {
    Object.assign(target, source);
  }

  public signal: Subject<void>;
  private registerSubscriptions() {

    // Trigger the save event using a switch outside of the grid. conf
    this.pubService.detachedSaveBtn.pipe(takeUntil(this.signal))
      .subscribe(() => {
        if (this.conf.generalSettings.performOnEdit && this.conf.editingMode === EditingMode.IN_CELL)
          return;

        this.incellSaveChanges();
      });

    // For updating dropdown lists
    this.dropdownListSubject.pipe(takeUntil(this.signal))
      .subscribe((event: DropdownListEvent) => {
        if (event.type === DropdownEventType.ON_PRE_UPDATE_DROPDOWN) {
          const colToUpdate = this.kData.columns.filter(col => col.ddl?.id === event.id);

          if (colToUpdate.length === 1) {
            colToUpdate[0].ddl.data = event.data.map(s => new DropdownListItem(s.id, s.name));
          }

          this.updateDropdownColumn.next(
            {
              type: DropdownEventType.ON_UPDATE_DROPDOWN,
              id: event.id,
              data: colToUpdate,
              listItems: event.listItems
            });
          this.next(this.kData);
        }
      });

    // Listening to the public grid service for new grid source data.
    this.pubService.pipe(
      takeUntil(this.signal),
      skip(1),
      filter(x => !x.stopPropagation))
      .subscribe((kdata: GridDataWithFilter) => {
        this.kData = kdata.data ?? { rows: [], model: {}, columns: [] };
        this._read(kdata.forceRefresh, true, [], kdata.afterEdit);
      });

  }

  /**
   * Soon to be deprecated methods
   */
  getEditingMode() {
    return this.conf.editingMode;
  }
  /** Disables the editing of the grid. */
  disable() {
    this.conf.gridIsEditable = false;
    this.conf.disableGrid();
    this.conf.makeCellsUneditable();
  }
  /** Enables the editing of the grid. */
  enable() {
    this.conf.gridIsEditable = true;
    this.conf.disableGrid();
    this.conf.makeCellsUneditable();
  }


  setKendoData(rows: KendoGridRow[], model: KendoGridModel, cols: KendoGridColumn[]) {
    if (this.originalData == null) {
      this.originalData = rows;
    }

    this.logService.checkRequiredFields(model, cols, rows);

    this.kData = this.manageDates({ rows: rows, columns: cols, model: model });
    this.next(this.kData);
  }

  getRows(): any[] {
    return this.kData.rows.slice();
  }

  getModel(): any {
    return Object.assign({}, this.kData.model);
  }

  getColumns() {
    return this.kData.columns;
  }

  /**
   * Used for mapping the public grid service of the grid to an unused multi
   * index of the public service.
   */
  private mapFreePubService() {
    const found = (this.pubServices as any as GridPublicService[])
      .find((s) => s.freeMultiIndex);

    if (!found) {
      throw new Error('Nessun servizio pubblico disponibile per essere mappato a KendoGridService.');
    }

    this.pubService = found;
    this.pubService.freeMultiIndex = false;
  }

  /**
   * Used for mapping a configuration service to its corresponding service
   * (when multi: true in the providers array).
   */
  private mapMultiToken(ctx: any) {
    const configs = this.multiConfigs as any as [];
    const found: AbstractGridConfigService<GridServerResult> = configs.find(
      (c: AbstractGridConfigService<GridServerResult>) =>
        c.gridId === ctx.gridId);

    if (!found) {
      throw Error(`Non ho riuscito mappare la griglia con l'ID '${ctx.gridId}' a nessun servizio di configurazione.`);
    }

    this.conf = found;
  }

  public setDataItem(dataItem: any) {
    this.pubService.currentDataItem = dataItem;
  }

  /**
   * [Section] Grid state
   */

  /**
   * [Context] Store/restore current page using cookies.
   */

  public storeCurrentGridPage(skip: number) {
    this.localStorage.store(this._lastAccessedPageCookieId(), '' + skip);
  }

  public checkCacheForPreviousPage() {
    return this.localStorage.retrieve(this._lastAccessedPageCookieId());
  }

  private _lastAccessedPageCookieId() {
    return `${this.conf?.gridId}-LastAccessedPage`;
  }

  /** END grid state */

  /** Some fields may be lost as a result of serialization. Here we restore
   * some of these fields (dropdown load functions and rendered custom
   * components).
   */
  restoreLostData(viewColumns: KendoGridColumn[]): KendoGridColumn[] {
    return viewColumns.flatMap((col) => {
      if (!col.field)
        return []; // remove the item

      let result = col;
      if (this.isOfType(col, CELL_TYPES.CUSTOM))
        result = this.mapComponentProp(result);

      if (this.isOfType(col, CELL_TYPES.DROPDOWNLIST) && col.ddl.loadOnEdit ||
        this.isOfType(col, CELL_TYPES.MULTI_DROPDOWNLIST) && col.ddl.loadOnEdit)
        result = this.mapLoadFn(result);

      return result;
    }, this);
  }

  mapComponentProp(col: KendoGridColumn) {
    col.component = this.originalColumns.find(oc => oc.field === col.field).component;
    return col;
  }

  mapLoadFn(col: KendoGridColumn) {
    col.ddl.loadFunction = this.originalColumns.find(oc => oc.field === col.field)?.ddl?.loadFunction;
    return col;
  }

  isOfType(col: KendoGridColumn, type: CELL_TYPES) {
    switch (type) {
      case CELL_TYPES.DROPDOWNLIST:
      case CELL_TYPES.MULTI_DROPDOWNLIST:
        return this.isDdl(col);
      case CELL_TYPES.CUSTOM:
        return this.isCustom(col);
    }
    return null;
  }

  isDdl(col: KendoGridColumn) {
    return this.originalModel[col.field]?.type === CELL_TYPES.DROPDOWNLIST ||
      this.originalModel[col.field]?.type === CELL_TYPES.MULTI_DROPDOWNLIST
  }

  isCustom(col: KendoGridColumn) {
    return this.originalModel[col.field]?.type === CELL_TYPES.CUSTOM;
  }

  getColumnType(field: any) {
    const column = this.originalModel[field];

    if (_isNull(column))
      throw new Error("Requested column field was not found in the model");

    return column.type;
  }
}

class BatchEditingHelper {
  itemWasModifiedPreviouslyToBeingRemoved(item: KendoGridRow, itemsPreviouslyUpdated: KendoGridRow[]) {
    return itemsPreviouslyUpdated.indexOf(item);
  }
}
