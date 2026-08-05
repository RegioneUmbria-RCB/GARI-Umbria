import { DatePipe } from '@angular/common';
import {
  AfterViewChecked,
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ContentChild,
  ContentChildren,
  ElementRef,
  EventEmitter,
  Input,
  NgZone,
  OnDestroy,
  OnInit,
  Optional,
  Output,
  QueryList,
  Renderer2,
  TemplateRef,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { ControlContainer, FormControl, FormGroup, FormGroupDirective } from '@angular/forms';
import {
  faCaretDown,
  faCheckCircle,
  faCopy,
  faEdit,
  faEllipsis,
  faInfo,
  faPlusSquare,
  faSave,
  faTimes,
  faTrashAlt
} from '@fortawesome/free-solid-svg-icons';
import { toNumber, TranslocoService } from '@jsverse/transloco';
import { DialogService } from '@progress/kendo-angular-dialog';
import { ExcelExportData } from '@progress/kendo-angular-excel-export';
import {
  AddEvent,
  CancelEvent,
  CellClickEvent,
  CellCloseEvent,
  CheckboxColumnComponent,
  ColumnBase,
  ColumnComponent,
  ColumnReorderEvent,
  ColumnVisibilityChangeEvent,
  CommandColumnComponent,
  DetailCollapseEvent,
  DetailExpandEvent,
  EditEvent,
  GridComponent,
  GridDataResult,
  PageChangeEvent,
  RemoveEvent,
  RowArgs,
  RowClassArgs,
  SaveEvent,
  SelectAllCheckboxState,
  SelectionEvent
} from '@progress/kendo-angular-grid';
import { IntlService } from '@progress/kendo-angular-intl';
import { TooltipDirective } from '@progress/kendo-angular-tooltip';
import { aggregateBy, AggregateResult, process, SortDescriptor, State } from '@progress/kendo-data-query';
import { PropertyValidator } from '../../shared/ValidateProperty';
import { GridCommandItem } from '../../shared/utils';
import { AGRODATAFINE, AGRODATAINIZIO, SMARTPHONE_WIDTH } from '../../shared/CostantiPersonalizzate';
import { GridWorkerService } from '../../grid-worker.service';
import { fromEvent, Observable, of, Subject, Subscription, tap } from 'rxjs';
import { catchError, map, take, takeUntil } from 'rxjs/operators';
import { CustomizeDropdownService } from './components/grid-customizations/dropdown/customization-dropdown.service';
import { GridCustomizationsService } from './components/grid-customizations/grid-customizations.service';
import { CustomizationRequest, InMemoryView } from './components/grid-customizations/model';
import { GridDropdownService } from './components/grid-dropdownlists/grid-dropdown.service';
import { DropdownHelper } from './components/grid-multi-dropdownlist/grid-multi-dropdown.service';
import { GridMultiDropdownComponent } from './components/grid-multi-dropdownlist/grid-multi-dropdownlist.component';
import { IScrollingMode, ScrollingModeFactory } from './interfaces/scrollingMode.interface';
import {
  AggregateSettings,
  AgrSelectableSettings,
  BehaviorSettings,
  ColumnMenuSettings,
  CommandsColumnSettings,
  CommandsDropDownEvents,
  CommandsDropDownSettings,
  CustomColumnSettings,
  DeletionMode,
  DettagliColumnSettings,
  GeneralSettings,
  GroupSettings,
  MasterDetailSettings,
  PaginationSettings,
  ResizableSettings,
  ScrollingMode,
  SortSettings,
  ToolbarSettings
} from './models/configuration.model';
import {
  DropdownListItem,
  EditingMode,
  GridCustomizations,
  GridInfoCommandEvent,
  KendoGridColumn,
  KendoGridModel,
  KendoGridRow,
  KendoServerResult,
  LoaderType,
  ModelEntry,
  ReadEvent,
  RendererGridEvent,
  RendererGridEventType,
  SelectedOpts
} from './models/grid.model';
import { IOptionalConfigParameters } from './models/template.model';
import { AbstractGridConfigService } from './services/grid-config.service';
import { GridDialogService } from './services/grid-dialog.service';
import { GenericErrors, GridErrorService } from './services/grid-log.service';
import { GridPublicService } from './services/grid-public.service';
import { KendoGridService } from './services/kendo-grid.service';
import {
  _isNull,
  columnAlreadyInView as columnsAlreadyInView,
  findMatchedIndex,
  mapDateFilter,
  resetSelection
} from './services/utilities';
import { Shared } from './useCases/shared.service';
import { cloneDeep } from "lodash";
import { CompositeFilterDescriptor } from "@progress/kendo-data-query/dist/npm/filtering/filter-descriptor.interface";
import { GridDetailInfo, KendoGridMasterDetailService } from "./services/grid-master-detail.service";
import { GridBatchHandlerService } from './services/grid-batch-handler.service';
import { CellDropdownNamePipe } from './pipes/get-ddl-cell-name.pipe';
import { DropdownListEvent, DropdownEventType, CELL_TYPES } from 'gias-ui-kit';


/**
 * Various section:
 * - [Section] Grid state
 */

const visibleCols = (cols: ColumnBase[]) => cols?.filter(col => col.isVisible)


@Component({
  standalone: false,
  selector: 'gias-kendo-grid',
  templateUrl: './kendo-grid.component.html',
  styleUrls: ['./kendo-grid.component.scss'],
  viewProviders: [{ provide: ControlContainer, useExisting: FormGroupDirective },],
  providers: [
    GridCustomizationsService,
    GridDropdownService,
    DatePipe,
    CustomizeDropdownService,
    GridWorkerService,
    GridBatchHandlerService],
  encapsulation: ViewEncapsulation.None
})
export class GiasKendoGridComponent implements
  OnInit,
  AfterViewInit,
  AfterViewChecked,
  OnDestroy,
  IOptionalConfigParameters {

  /**
   * TODO_RV
   * Adjust column width based on contents (visible buttons): dettagliColumn and commandiColumn.
   * Features to add.
   * 1. Allow user to select between the loading animation of the grid and global loading animation mode.
   * 2. Nel metodo initView, dovrei togliergli la verifica if(!kData). Si spreca un evento al
   * caricamento della pagina prima che i dati siano caricati. Esempio sulla pagina dei campi (durante la
   * chiamata al server).
   * 3. Da inserire una funzione configurabile per la ricerca dentro una dropdown list.
   */

  public unsubSignal = new Subject<void>();

  @ContentChild('noRecordsTemplateRef') noRecordsTemplateRef: TemplateRef<any>;

  gridContext: GiasKendoGridComponent = this;

  readonly IN_CELL = EditingMode.IN_CELL;
  readonly IN_LINE = EditingMode.IN_LINE;
  readonly IN_LINE_BATCH = EditingMode.IN_LINE_BATCH;
  readonly IN_CELL_BATCH = EditingMode.IN_CELL_BATCH;

  /**
   *  Constants
   * */
  readonly STRING: string = CELL_TYPES.STRING;
  readonly DATE: string = CELL_TYPES.DATE;
  readonly DATETIME: string = CELL_TYPES.DATETIME;
  readonly DROPDOWNLIST: string = CELL_TYPES.DROPDOWNLIST;
  readonly MULTI_DROPDOWNLIST: string = CELL_TYPES.MULTI_DROPDOWNLIST;
  readonly NUMERIC: string = CELL_TYPES.NUMBER;
  readonly BOOLEAN: string = CELL_TYPES.BOOLEAN;
  readonly CUSTOM: string = CELL_TYPES.CUSTOM;

  /**
   * Font awesome.
   */
  faInfo = faInfo;
  faEdit = faEdit;
  faDelete = faTrashAlt;
  faCancel = faTimes;
  faNew = faPlusSquare;
  faCopy = faCopy;
  faSave = faSave; // icona usata per il salvataggio delle viste
  faCircleSave = faCheckCircle
  faArrow = faCaretDown;
  faDots = faEllipsis;

  /**
   * Required parameters
   */
  editingMode: EditingMode;
  loader: LoaderType;
  key: string;

  /**
   * General settings.
   * See the sortChange function.
   */
  generalSettings: GeneralSettings;

  /**
   * Pagination and sorting.
   */
  sort: SortSettings;
  pagination: PaginationSettings;

  masterdetailSettings: MasterDetailSettings = new MasterDetailSettings(false);

  /**
   * In cell editing.
   * */
  public view: Observable<GridDataResult>;

  public formGroup: FormGroup = new FormGroup({});
  /**
   * In line editing
   */
  private editedRowIndex: number;

  public isInEditMode: boolean;
  public inLineModificaIsDisabled: boolean = false;

  /**
   * Grid (optional) parameters
   * */
  public selectable: AgrSelectableSettings = new AgrSelectableSettings();
  public toolbar: ToolbarSettings = new ToolbarSettings();
  public cmdColumn: CommandsColumnSettings = new CommandsColumnSettings();
  public cmdDropDown: CommandsDropDownSettings = new CommandsDropDownSettings();
  public dettagliColumn: DettagliColumnSettings = null;
  public customColumn: CustomColumnSettings = null;

  public columnMenu: ColumnMenuSettings = new ColumnMenuSettings();
  public groups: GroupSettings;
  public columnGroups: Map<string, KendoGridColumn[]>;

  /**
   * Setting behavior and not UI related functionality.
   */
  public behavior: BehaviorSettings;
  public privateService: KendoGridService;
  public publicService: GridPublicService;

  public AGRODATA_INIZIO: Date = AGRODATAINIZIO;
  public AGRODATA_FINE: Date = AGRODATAFINE;

  public insideWindow = window;
  public SMARTPHONE_WIDTH = SMARTPHONE_WIDTH;
  public config: AbstractGridConfigService<KendoServerResult>;

  public gridDialogService: GridDialogService;

  /** Use cases */
  Scrollable = ScrollingMode.Scrollable;
  public currentScrollingMode: IScrollingMode;

  @Input() customCommandColumnCellTemplate: TemplateRef<ElementRef> = null;

  @Input() customCommandColumnHeaderTemplate: TemplateRef<any> = null;

  @Input() showSwitchViewsPrivatePublic = false;

  @Output() rowsLoaded = new EventEmitter<void>();

  constructor(
    public customizationService: GridCustomizationsService,
    public gridServices: KendoGridService,/* Could be of type array or object */
    private logService: GridErrorService,
    private intlService: IntlService,
    public changeDetector: ChangeDetectorRef,
    public ngZone: NgZone,
    private worker: GridWorkerService,
    private translocoService: TranslocoService,
    public renderer: Renderer2,
    private gridBatchHandlerService: GridBatchHandlerService,
    kendoDialogService: DialogService,
    @Optional() private kendoGridMasterService: KendoGridMasterDetailService
  ) {
    this.groups = new GroupSettings({}, this.translocoService);
    this.total = new Map<string, AggregateResult>();
    if (!Array.isArray(this.gridServices)) {
      this.privateService = this.gridServices;
      this.publicService = this.privateService.pubService;
      this.config = this.privateService.conf;
    }

    this.gridDialogService = new GridDialogService(this.publicService, kendoDialogService, this.config, translocoService);
  }

  getColumnsGroupValue(): string {
    let arr: any[] = [];
    this.columnGroups?.forEach((val) => {
      val.forEach((val1) => {
        if (!val1.hidden) {
          arr.push({
            field: val1.title,
            orderIndex: val1.orderIndex
          });
        }
      })
    });
    return JSON.stringify(arr);
  }

  getComandValue(): string {
    let arr: any[] = [];
    this.columnGroups?.forEach((val) => {
      val.forEach((val1) => {
        if (val1.title == 'Comandi') {
          arr.push(val1);
        }
      })
    });
    return JSON.stringify(arr);
  }

  get gridState() {
    return this.pagination.gridState;
  }

  get pageSize() {
    return this.pagination.gridState.take;
  }

  public rowClassCallback(e: RowClassArgs) {
    let rowClasses = {};
    if (typeof this.config.onRowClass === 'function')
      rowClasses = this.config.onRowClass(e);

    return {
      ...rowClasses,
      'pending-deletion-row': e.dataItem['pending-deletion-row'] ?? false
    };
  }

  public ShowHTMLAsString(cellType: CELL_TYPES, col: KendoGridColumn | ColumnComponent) {
    let show = false;

    let complete_column: KendoGridColumn = null;

    if (col instanceof ColumnComponent) {
      complete_column = this.columns?.find(c => c.field === col.field);
    } else {
      complete_column = col;
    }

    if (cellType === CELL_TYPES.STRING && complete_column.editable === false && complete_column.showHTMLAsString === true) {
      show = true;
    }

    return show;
  }

  /**
   * Utilizzato per determinare il corrispondente token quando multi: true.
   */
  @Input() gridId: string;

  ngOnDestroy(): void {
    this.unsubSignal.next();
    this.unsubSignal.complete();
    this.showCustomColumnSubscription?.unsubscribe();
    this.privateService.signal?.next();
    this.worker.workerInst?.terminate();

    try {
      this.privateService.clearKendoGridService();
      this.privateService.complete();
    } catch(e){
      console.log("error this.privateService.clearKendoGridService", e);
    }

    try {
      this.publicService.clearGridPublicService();
      this.publicService.complete();
    } catch(e){
      console.log("error this.publicService.clearGridPublicService", e);
    }
    if (this.pagination?.gridState?.skip !== undefined && this.pagination?.gridState?.skip !== null)
      this._storeCurrentGridPage();
  }

  ngOnInit(): void {
    this.publicService.gridElRef = this.gridElRef;
    if (Array.isArray(this.gridServices)) {
      for (let i = 0; i < this.gridServices.length; i++) {
        if (this.gridServices[i].freeMultiIndex) {
          (this.gridServices[i] as KendoGridService).initProviders(this);
          this.publicService = this.privateService.pubService;
          this.config = this.privateService.conf;
          break;
        }
      }
    }
    this.rowId = this.config.rowId;

    this.privateService.ngOnInit(this);

    this._restorePreviousGridPage();

    this.initParams();
    this.createUsecaseServices();

    if (this.loader === LoaderType.RESOLVER) {
      this.initializeUsingResolver();
    } else if (this.loader === LoaderType.HTML) {
      this.initializeUsingHTML();
    } else if (this.loader === LoaderType.SERVICE) {
      this.defaultInitializationMethod();
    }

    this.registerSubscriptions();

    this.manageWindowResizeEvents();

  }


  resizeObservable: any;
  manageWindowResizeEvents() {
    const onWindowResize = () => {
      this.resizeObservable = fromEvent(window, 'resize');
      return this.resizeObservable.pipe(takeUntil(this.unsubSignal));
    }

    onWindowResize().subscribe(event => {
      this.resizeFnWidth(event.target.innerWidth)
      this.adjustGridHeightBasedOnContent()
    });
    this.resizeFnWidth(window.innerWidth);
  }

  private resizeFnWidth(innerWidth: number) {
    const width = "width";
    const isMobileWidth = (value: number) => value < SMARTPHONE_WIDTH;
    const setProperty = (obj, prop, value) => !_isNull(obj) && (obj[prop] = value)
    const triggerSticky = (_switch: boolean) => {
      const sticky = "sticky";
      setProperty(this.dettagliColumn, sticky, _switch);
      setProperty(this.customColumn, sticky, _switch);
      setProperty(this.cmdColumn, sticky, _switch);
      setProperty(this.cmdDropDown, sticky, _switch);
      setProperty(this.selectable, sticky, _switch);
    }

    let infoBtnAbilitato = this.cmdColumn.infoBtn;
    let _isMobileWidth = isMobileWidth(innerWidth);
    let widthNum = this.dettagliColumn.computeWidth(_isMobileWidth, infoBtnAbilitato);
    if (_isMobileWidth) {
      triggerSticky(false);
      setProperty(this.dettagliColumn, width, widthNum);
    } else {
      triggerSticky(true);
      setProperty(this.dettagliColumn, width, widthNum);
    }
    let widthNumCustomClm = this.customColumn.computeWidth(_isMobileWidth, infoBtnAbilitato);
    if (_isMobileWidth) {
      triggerSticky(false);
      setProperty(this.customColumn, width, widthNumCustomClm);
    } else {
      triggerSticky(true);
      setProperty(this.customColumn, width, widthNumCustomClm);
    }
    this.changeDetector.detectChanges();
  }

  /**
   * [Section] Grid state.
   * [Item] Store current grid page in cache, to be restored when creating the
   * grid.
   */

  private _storeCurrentGridPage() {
    const skip = this.pagination.gridState.skip;
    this.privateService.storeCurrentGridPage(skip);
  }

  private _restorePreviousGridPage() {
    const state = this.privateService.conf.pagination.gridState;
    this.pagination.gridState = this.privateService.clone(state);

    const prevNavigationIndex = +this.privateService.checkCacheForPreviousPage();

    if (prevNavigationIndex != null)
      this.pagination.gridState.skip = Shared.changeGridPage(prevNavigationIndex, this.pageSize);
  }

  /**
   * [Item] adjust grid height based on content.
   */

  private adjustGridHeightBasedOnContent() {
    const computeTableHeight = () => {
      let gridElemRef = this.gridElRef.nativeElement;
      let configHeight = this.config.generalSettings.height;
      if (this._scrollbarIsVisibleOn(gridElemRef, configHeight)) {
        let pixels = null;
        if (typeof configHeight === 'number')
          pixels = configHeight + 'px';
        this.generalSettings.height = pixels;
      }
      else if (configHeight === 'auto')
        this.generalSettings.height = 'auto';
      else
        this._adaptHeightOnContainer()
      this.changeDetector.detectChanges();
    }

    this.ngZone.onStable
      .asObservable()
      .pipe(take(1))
      .subscribe(computeTableHeight);
  }

  /** Adatta l'altezza della griglia a quella del primo div padre con altezza fissa. */
  private _adaptHeightOnContainer() {
    let gridElemRef = this.gridElRef.nativeElement;
    this.generalSettings.height = 'auto';
    let parent = this._findFirstParentWithFixedHeight(gridElemRef);
    let parentHeight = parent?.clientHeight ?? 0;
    let margin = 40;
    // Se la griglia risulta più alta del div padre, adatto l'altezza specificando i pixel necessari
    if (gridElemRef.clientHeight > parentHeight) {
      this.generalSettings.height = (parentHeight - margin) + 'px';
    }
  }

  private _allRowsFit(nativeElement: any) {
    const tolerance = 20; // Usual height of a row is 40px
    const innerGridHeight = nativeElement.querySelector('.k-grid-content').clientHeight;
    const rows = nativeElement.querySelector('.k-grid-content').querySelectorAll('tr');
    let rowsHeight = 0;
    for (let row of rows) {
      rowsHeight += row.clientHeight;
      if (rowsHeight > (innerGridHeight + tolerance))
        return false;
    }
    return true;
  }

  private _findFirstParentWithFixedHeight(nativeElement: any) {
    let parent = nativeElement?.parentElement?.parentElement;
    while (parent?.clientHeight == 0) {
      parent = parent.parentElement;
    }
    return parent;
  }

  private _hasRowsScrollBar(nativeElement: any): boolean {
    let clientHeight = nativeElement.getElementsByClassName('k-grid-content')[0].clientHeight;
    let scrollHeight = nativeElement.getElementsByClassName('k-grid-content')[0].scrollHeight;
    return clientHeight < scrollHeight;
  }

  private _scrollbarIsVisibleOn(nativeElem: any, configHeight: number | 'auto' | string) {
    let inner = nativeElem.querySelector('kendo-grid-list .k-grid-content');
    if (configHeight === 'auto' || configHeight === "100%")
      return false;

    let height;
    if (typeof configHeight === 'string')
      height = parseInt(configHeight, 10);
    else
      height = configHeight;

    return inner.scrollHeight > height;
  }



  /** END [Section] Grid state */

  registerSubscriptions() {
    if (this.views.enabled) {
      this.customizationService.customizationRequest.pipe(
        takeUntil(this.unsubSignal)
      ).subscribe((req: CustomizationRequest) => {
        this.customizationService.mapGridSettingsAndSaveThem(this, req.dropdownItem, req.isNewView);
      });
    }
  }



  /**
   * Used for resizing the grid.
   */

  public resizable: ResizableSettings;
  @ViewChild('grid', { static: true }) grid: GridComponent;

  @ViewChild('CheckboxColumn') checkboxColumn: CheckboxColumnComponent;
  @ViewChild('CommandColumn') commandColumn: CommandColumnComponent;

  @ViewChild('grid', { static: true, read: ElementRef }) gridElRef: ElementRef;

  @ViewChild('input', { static: false }) input: ElementRef;

  /**
   * In-cell and in-line modification settings
   * */

  @Output() cellClick = new EventEmitter<CellClickEvent>();
  private resetGroupColumns() {
    if (this.columnGroups != null && !this.haveColumnsChanged()) {
      return;
    }

    const groups = new Map<string, KendoGridColumn[]>();

    for (const col of this.columns) {
      if (!groups.has(col.customParent)) {
        groups.set(col.customParent, []);
      }

      groups.get(col.customParent).push(col);
    }

    this.columnGroups = groups;

    this.changeDetector.detectChanges();
    const commandColumns = this.grid.columns
      .filter(x => !(x instanceof ColumnComponent))
      .reverse();
    for (const commandColumn of commandColumns) {
      this.grid.reorderColumn(commandColumn, 0, { before: true });
    }
  }

  private haveColumnsChanged(): boolean {
    const oldColumns = [...this.columnGroups.values()].flatMap(x => x);
    if (oldColumns.length != this.columns.length) {
      return true;
    }

    for (let i = 0; i < this.columns.length; i++) {
      const newColumn = this.columns[i];
      const oldColumn = oldColumns[i];
      if (oldColumn.field != newColumn.field || oldColumn.title != newColumn.title || oldColumn.width != newColumn.width ||
        oldColumn.orderIndex != newColumn.orderIndex || oldColumn.hidden != newColumn.hidden || oldColumn.format != newColumn.format) {
        return true;
      }
    }

    return false;
  }


  public cellClickHandler(event: CellClickEvent) {
    let item = event.dataItem;
    if (elementQueuedForDeletion(item) || this.privateService.preventEdit(event))
      return;
    this.isInEditMode = true;
    const isInCell = this.privateService.getEditingMode() === EditingMode.IN_CELL || this.privateService.getEditingMode() === EditingMode.IN_CELL_BATCH;
    if (isInCell && !this.inLineModificaIsDisabled && !event.isEdited) {

      if (this.behavior.createFormGroupFromOutside) {
        this.formGroup = this.privateService.onCreateExternalFormGroup(event);
      } else {
        this.createFormGroup(event.dataItem);
      }

      event.sender.editCell(event.rowIndex, event.columnIndex, this.formGroup);
      this.changeDetector.detectChanges();
    }
    this.privateService.onCellClick(event);
    this.cellClick.emit(event);

    this.isInEditMode = false;
  }

  public cellCloseHandler(event: CellCloseEvent) {
    const { formGroup, dataItem } = event;
    const isFormValid = this.editingMode == EditingMode.IN_CELL
      ? formGroup.controls[event.column.field].valid : formGroup.valid;

    if (!isFormValid) {
      event.preventDefault();
    } else if (formGroup.dirty) {
      this.privateService.assignValues(dataItem, formGroup.value);

      this.privateService.inCellUpdate(dataItem);
      // this.publicService.changeDetected.next(event);
    }

    this.privateService.onCellClose(event, this.input);
    this.applyRendererGiven(event);

    if (this.editingMode == EditingMode.IN_CELL_BATCH) {
      const item = this.rows.find(x => x[this.rowId] == dataItem[this.rowId]);
      const index = this.rows.indexOf(item);
      this.rows[index] = dataItem;
      this.gridBatchHandlerService.update(dataItem, this.rowId);
    }
  }

  public disableButtonsDuringEditMode(disabled: boolean) {
    this.inLineModificaIsDisabled = disabled;

    let btnsToDisable = this.gridElRef.nativeElement.querySelectorAll('.btnDisabledInModifica');
    for (let i = 0; i < btnsToDisable.length; ++i) {
      this.renderer.setProperty(btnsToDisable[i], 'disabled', disabled);
    }
  }

  public addHandler(event: AddEvent) {
    this.isInEditMode = true;
    this.publicService.currentDataItem = event.dataItem;

    this.closeEditor(event.sender);

    switch (this.editingMode) {
      case EditingMode.IN_LINE:
      case EditingMode.IN_CELL:
      case EditingMode.IN_LINE_BATCH:
      case EditingMode.IN_CELL_BATCH:
        if (this.behavior.createFormGroupFromOutside) {
          this.formGroup = this.privateService.onCreateExternalFormGroup(event);
        } else {
          this.createFormGroup(event.dataItem);
        }

        event.sender.addRow(this.formGroup);
        break;
      case EditingMode.IN_PAGE:
        // Nothing to do...
        break;
      default:
        this.logService.generalErrors(GenericErrors.EditingModeNotFound, this.editingMode);
    }

    this.handleDdlValue(event);

    this.publicService.changeDetected.next(event);

    this.isInEditMode = false;
    // if (this.resizable.autoFitColumns) {
    //     this.grid.autoFitColumns();
    // }

    this.disableButtonsDuringEditMode(true);
  }


  public cancelHandler(event: CancelEvent) {
    this.closeEditor(event.sender, event.rowIndex);

    const editingMode = this.privateService.getEditingMode();

    switch (editingMode) {
      case EditingMode.IN_LINE:
        this.closeEditor(event.sender);
        break;
      case EditingMode.IN_CELL:
        event.sender.closeRow(event.rowIndex);
        break;
      case EditingMode.IN_PAGE:
        // Nothing to do...
        break;
      case EditingMode.IN_LINE_BATCH:
      case EditingMode.IN_CELL_BATCH:
        this.closeEditor(event.sender);
        event.sender.closeRow(event.rowIndex);
        break;
      default:
        this.logService.generalErrors(GenericErrors.EditingModeNotFound, editingMode);
        break;
    }

    this.publicService.changeDetected.next(event);

    this.publicService.currentDataItem = null;
    this.applyRendererGiven(event);

    this.disableButtonsDuringEditMode(false);
  }

  public saveHandler(event: SaveEvent) {
    event.formGroup.markAllAsTouched();
    if (!event.formGroup.valid) {
      event['action'] = 'save-failed'
      this.publicService.changeDetected.next(event);
      return;
    }

    let obs: Observable<any>;
    let editingMode: EditingMode;
    if (event.formGroup.valid) {
      editingMode = this.privateService.getEditingMode();
      const row: any = event.formGroup.getRawValue();

      switch (editingMode) {
        case EditingMode.IN_LINE:
          obs = this.privateService.save(row, event.dataItem.chiave, event.isNew, event.rowIndex);
          break;
        case EditingMode.IN_CELL:
          this.privateService.incellCreate(row);
          break;
        case EditingMode.IN_PAGE:
          // Nothing to do...
          break;
        case EditingMode.IN_LINE_BATCH:
        case EditingMode.IN_CELL_BATCH:
          if (event.isNew) {
            this.gridBatchHandlerService.add(row);
            this.rows.push(row);
          } else {
            const item = this.rows.find(x => x[this.rowId] == row[this.rowId]);
            const index = this.rows.indexOf(item);
            this.rows[index] = row;
            this.gridBatchHandlerService.update(row, this.rowId);
          }

          this.publicService.refresh(false);
          break;
        default:
          this.logService.generalErrors(GenericErrors.EditingModeNotFound, editingMode);
      }
    }

    // TODO(RV) nel caso in cui il salvataggio non è andato a buon fine,
    // non chiudere la riga selezionata (e non rifa la lettura)
    if (obs != null) {
      obs.pipe(
        catchError((err) => {
          switch (editingMode) {
            case EditingMode.IN_LINE:
              this.privateService.idealRead(true, true)
              break;
            case EditingMode.IN_CELL:
              this.privateService.idealRead(false, true)
              break;
            case EditingMode.IN_PAGE:
            case EditingMode.IN_LINE_BATCH:
            case EditingMode.IN_CELL_BATCH:
              this.privateService.idealRead(false, true)
              break;
            default:
              this.privateService.idealRead(false, true)
              break;
          }
          return of(err);
        })
      ).subscribe((val) => {
        if (val !== false) {

          event.sender.closeRow(event.rowIndex);
          this.publicService.changeDetected.next(event);
          this.publicService.currentDataItem = null;
          this.disableButtonsDuringEditMode(false);


          switch (editingMode) {
            case EditingMode.IN_LINE:
              this.privateService.idealRead(true, true, [], true)
              break;
            case EditingMode.IN_CELL:
              this.privateService.idealRead(false, true, [], true)
              break;
            case EditingMode.IN_PAGE:
            case EditingMode.IN_LINE_BATCH:
            case EditingMode.IN_CELL_BATCH:
              this.privateService.idealRead(false, true)
              break;
            default:
              this.privateService.idealRead(false, true)
              break;
          }
        }
      });

    } else {
      event.sender.closeRow(event.rowIndex);
      this.publicService.changeDetected.next(event);
      this.publicService.currentDataItem = null;
      this.disableButtonsDuringEditMode(false);
    }
  }

  public async removeHandler(event: RemoveEvent) {
    const selected = this.privateService.getAllSelectedRows(event.dataItem);
    await this.gridDialogService.showDialog(() => {
      if (this.editingMode === EditingMode.IN_LINE_BATCH || this.editingMode === EditingMode.IN_CELL_BATCH) {
        this.batchRemove(event.dataItem);
      } else if (this.behavior.deletionMode === DeletionMode.HandleSingleRowDeletionOnly) {
        this.privateService.remove(event.dataItem);
      } else {
        selected.forEach(r => r['Selected'] = true)
        this.privateService.remove(selected);
      }
    }, event.dataItem);

    const editingMode = this.privateService.getEditingMode();

    switch (editingMode) {
      case EditingMode.IN_LINE:
        // Nothing to do...
        break;
      case EditingMode.IN_CELL:
        event.sender.cancelCell();
        break;
      case EditingMode.IN_PAGE:
        // Nothing to do...
        break;
      case EditingMode.IN_LINE_BATCH:
        break;
      case EditingMode.IN_CELL_BATCH:
        event.sender.cancelCell();
        break;
      default:
        this.logService.generalErrors(GenericErrors.EditingModeNotFound, editingMode);
    }

    this.publicService.changeDetected.next(event);
  }

  private batchRemove(row): void {
    if (this.behavior.deletionMode === DeletionMode.HandleSingleRowDeletionOnly) {
      this.gridBatchHandlerService.remove(row, this.rowId);
      return;
    }

    const rows = this.privateService.getAllSelectedRows(row);
    rows.forEach(row => { this.privateService.highlightRowDeletion(row); });
    this.gridBatchHandlerService.remove(rows, this.rowId);
    this.privateService.updatePubService();
  }

  public saveChanges(grid: any): void {
    grid.closeCell();
    grid.cancelCell();

    this.privateService.incellSaveChanges(this.config.generalSettings);
  }

  public batchSave(): void {
    const data = this.gridBatchHandlerService.getData();
    this.privateService.batchSave(data).pipe(
      tap((val) => {
        this.resetHandler();
        this.privateService.idealRead(true, true)
      })
    ).subscribe();
  }

  public cancelChanges(grid: any): void {
    grid.cancelCell();

    this.privateService.cancelChanges();
  }

  /**
   * In line edit handler
   */
  public editHandler(event: EditEvent) {

    this.isInEditMode = true;

    this.publicService.currentDataItem = event.dataItem;

    // TODO IN_CELL_BATCH
    if (this.editingMode === EditingMode.IN_LINE || this.editingMode === EditingMode.IN_LINE_BATCH) {
      this.closeEditor(event.sender);

      if (this.behavior.createFormGroupFromOutside) {
        this.formGroup = this.privateService.onCreateExternalFormGroup(event);
      } else {
        this.createFormGroup(event.dataItem);
        this.editedRowIndex = event.rowIndex;
      }
      event.sender.editRow(event.rowIndex, this.formGroup);
    }

    this.handleDdlValue(event);

    this.isInEditMode = false;
    // if (this.resizable.autoFitColumns) {
    //     this.grid.autoFitColumns();
    // }

    this.disableButtonsDuringEditMode(true);

    this.publicService.changeDetected.next(event);
  }

  private handleDdlValue(event: EditEvent | AddEvent) {

    let helper = new GridHelper();
    this.columns.forEach((col) => {

      if (event.dataItem && col.ddl && col.ddl.descriptionField && col.ddl.loadOnEdit) {
        if (helper.columnContainsSimpleDropdownList(this.model, col.field)) {
          const id = event.dataItem[col.field];
          const name = event.dataItem[col.ddl.descriptionField];
          const ddlItem = col.ddl.data.find((el) => (el.id == id && el.name == name));
          if (ddlItem == undefined) {
            col.ddl.data = [{ id: id, name: name }];
          }
        } else if (helper.columnContainsMultiDropdownList(this.model, col.field)) {
          const { missingItemInSource, newDDLData } = helper.getMultiDDLDataIfMissing(event.dataItem, col);
          if (missingItemInSource)
            col.ddl.data = [...newDDLData];
        }
      }
    });
  }

  public resetHandler() {
    this.privateService.reset();
    // Call it here to avoid injection problems
    if (this.editingMode === EditingMode.IN_LINE_BATCH || this.editingMode === EditingMode.IN_CELL_BATCH) {
      this.gridBatchHandlerService.reset();
    }
  }

  public resetFilter() {
    if (this.gridState.filter) {
      let filters = this.gridState.filter;
      let finalFilters: CompositeFilterDescriptor = {
        filters: [],
        logic: filters.logic
      };
      //this.gridServices.pubService.filters.next(finalFilters);
      this.gridState.filter = finalFilters;
      this.grid.filter = finalFilters;
      // this.grid.data = process((<any[]>this.grid.data), this.gridState);
      this.gridServices.pubService.refresh(false);
    }
  }


  public onStateChange(state: State) {
    const deselectPrev = false
    this.pagination.gridState = state;
    //this.autofitColumnComponents();

    if (this.views.enabled && !this.currentScrollingMode.isVirtual()) {
      this.updateViewCustomization(true);
    } else {

      if (this.pagination.scrollingType == ScrollingMode.Virtual) {
        const data = this.currentScrollingMode.loadData({ model: this.model, columns: this.columns, rows: this.rows });
        this.view = of(data);
      } else {
        this.privateService.idealRead(false);
      }
    }

    this.publicService.filters.next(state.filter);
    //TODO Chiedere ad Anny perchè h fatto questo perchè crea problemi quando cambi pagina si perde le righe

    /* Inizialmente era stato fatto a cause di questo:
     *     se nella griglia applico il filtro su una colonna (es. appezzamenti coinvolti) e clicco sul seleziona tutti,
     *     mi prende TUTTE le righe e non solamente quelle filtrate come ci si aspetterebbe.
     * Si era quindi pensato di resettare le righe selezionate al cambio dei filtri della griglia.
     * Ciò per evitare di avere righe selezionate che però non appaiono in griglia.
     *
     * 07/06/23 si decide che la selezione delle righe deve essere persistente al cambio di vista/filtri
    */
    if (deselectPrev) {
      // Deseleziono tutte le righe precedentemente selezionate
      this.rows.forEach(s => s['Selected'] = false);
      this.selectAllState = "unchecked";
    }
  }

  showToolbar: boolean;
  showCmdColumn: boolean;
  showCmdDDL: boolean;
  showDettagliColumn: boolean;
  showCustomColumn: boolean;
  showCustomColumnSubscription: Subscription;

  public initParams() {
    const computeCmdColumnWidth = () => {
      let count = 0;
      this.cmdColumn.editBtn && count++;
      this.cmdColumn.infoBtn && count++;
      this.cmdColumn.removeBtn && count++;
      return count == 1 ? 70 : count == 2 ? 110 : count == 3 ? 170 : 0
    }

    const showDettagliColumn = () => {
      if (this.dettagliColumn == null) {
        this.dettagliColumn = new DettagliColumnSettings();
      }
      return this.dettagliColumn?.editBtn || this.cmdColumn?.infoBtn;
    }

    const showCustomColumn = () => {
      if (this.customColumn == null) {
        this.customColumn = new CustomColumnSettings();
      }
      return this.customColumn?.showColumn;
    }

    this.showToolbar = this.toolbar.showToolbar() || this.columnMenu.kendoGridColumnChooser || this.views.enabled;

    this.showCmdColumn = this.cmdColumn.showCmdColumn();
    this.showCmdDDL = this.cmdDropDown.showCmdDropDown();
    this.showDettagliColumn = showDettagliColumn();
    this.showCustomColumn = showCustomColumn();
    this.cmdColumn.width = computeCmdColumnWidth();

    if (this.customColumn){
      this.showCustomColumnSubscription = this.customColumn.showColumn$.subscribe(showColumn => {
        this.showCustomColumn = showColumn;
      });
    }
  }


  @Output() pageChangeEvent = new EventEmitter<{ grid: GridComponent; event: PageChangeEvent }>();

  public pageChange(event: PageChangeEvent): void {
    if (this.pagination.scrollingType != ScrollingMode.Virtual) {
      this.pageChangeEvent.emit({
        grid: this.grid,
        event: event
      });
      this.pagination.gridState.skip = event.skip;
    } else {
      // This is being handled in onStateChange
    }
  }

  @Output() selectionChange = new EventEmitter<SelectionEvent>();
  public onSelectionChange(event: SelectionEvent) {
    event.selectedRows?.forEach(s => s.dataItem['Selected'] = true);
    event.deselectedRows?.forEach(s => s.dataItem['Selected'] = false);
    this.onSelectedKeysChange(null);

    if (this.selectable.selectable?.enabled && this.selectable.preselectedRows.selectionChangeFn) {
      setTimeout(() => {
        if (!this.selectAllChecked) {
          this.selectable.preselectedRows.selectionChangeFn(event, this);
        }
      }, 1);
    }

    this.publicService.next({
      stopPropagation: true,
      forceRefresh: false,
      data: this.GetDatiAttuali(),
      afterEdit: false
    })

    this.publicService.selection.getSelectedValue.next({ value: event });

    this.selectionChange.emit(event);
  }

  // public isRowSelected = (e: RowArgs) => {
  //     if(typeof this.selectable.preselectedRows.isRowSelectedFn !== 'function') {
  //         return;
  //     }
  //     return this.selectable.preselectedRows.isRowSelectedFn(e, this);
  // };

  public isRowSelected = (e: RowArgs) => e.dataItem['Selected'];

  /**
   * Signal column visibility change.
   */
  @Output() columnVisibilityChange = new EventEmitter<ColumnVisibilityChangeEvent>();
  public columnVisibilityChangeHandler(event: ColumnVisibilityChangeEvent) {
    //console.log(event);
    this.updateViewCustomization(true);
    this.applyRendererGiven(event);
    this.columnVisibilityChange.emit(event);
  }


  @Output() numericTextBox = new EventEmitter<number>();
  public onChange(event) {
    this.numericTextBox.emit(event);
  }


  /**
   *  Auxiliary functions
   **/
  public closeEditor(grid, rowIndex = this.editedRowIndex) {
    grid.closeRow(rowIndex);
    this.editedRowIndex = undefined;
    this.formGroup = undefined;
    this.disableButtonsDuringEditMode(false)
  }

  @ViewChild(TooltipDirective) public tooltipDir: TooltipDirective;
  public showTooltip(event: MouseEvent) {
    let element = event.target as HTMLElement;
    //let currCol = this.columns.find(c => c.title === element.innerText);
    if ((element.nodeName !== 'SPAN')                                  // is not a header
      || !this.columns?.map(c => c.title).includes(element.innerText)) { // is not a data column
      this.tooltipDir.hide();
      return;
    }

    this.tooltipDir.show(element);
  }

  /**
   * Aggregates.
   */
  public aggregates: AggregateSettings;
  public total: Map<string, AggregateResult>;

  private defaultInitializationMethod() {
    this.initView();

    this.logService.checkGridMasterDetailConfig(this.kendoGridMasterService, this.masterdetailSettings.enable)

    let dataItem: any = null;

    //Passo il dataItem della riga Master che si è aperta se sono in una grid master detail

    if (this.masterdetailSettings.IsGridMaster()) {
      this.kendoGridMasterService.GridMasterService = this.privateService;

      this.kendoGridMasterService.RowIdGridMaster = this.rowId;
    }

    if (this.masterdetailSettings.IsGridDetail()) {

      dataItem = this.kendoGridMasterService.Last_RowExpanded;

      this.kendoGridMasterService.GridDetailService = new GridDetailInfo(dataItem[this.kendoGridMasterService.RowIdGridMaster], this.privateService);

      //Rendo il gridId univoco
      this.gridId = this.gridId + "|_|" + dataItem[this.kendoGridMasterService.RowIdGridMaster];

      this.publicService.Current_Grid_Master_RowExpanded = dataItem;

    }

    this.privateService.idealRead(true, false);
  }

  ShowkendoGridDetailTemplate(): boolean {
    let show = false;

    if (this.masterdetailSettings.IsGridMaster() && this.masterdetailSettings.templateGridDetail) {
      show = true;
    }

    return show;
  }

  private initializeUsingResolver() {
    this.rows = this.privateService.getRows();
    this.model = this.privateService.getModel();
    this.columns = this.privateService.getColumns();

    this.initView();
    this.resetGroupColumns();

    this.rowsLoaded.emit();
  }

  private initializeUsingHTML() {
    this.privateService.setKendoData(this.rows, this.model, this.columns);
    this.initView();
  }

  colsLoaded: Subject<void> = new Subject();
  private initView() {
    this.view = this.privateService.pipe(map((kData: KendoServerResult) => {
      this.total = new Map<string, AggregateResult>();
      this._calculateAggregatesTotal(kData.rows, true, '', '');

      if (!kData || kData.model == undefined) {
        return this._feedEmptyGrid();
      }
      this._initializeGridData(kData);
      this._updateSelectAllColumnBasedOnSelectedRows();

      return this.currentScrollingMode.loadData(kData);
    }));

    this.changeDetector.detectChanges();
  }

  public refreshAggregateTotal(): void {
    this.total = new Map<string, AggregateResult>();
    this._calculateAggregatesTotal(this.rows, true, '', '');
  }

  public _calculateAggregatesTotal(rows: KendoGridRow[], footer: boolean, field: string, value: any): AggregateResult {
    if (this.aggregates.enabled) {
      let key = "";
      if (field != "") {
        key = JSON.stringify({ field: field, value: value });
      }
      if (this.total.has(key)) {
        return this.total.get(key);
      }
      let gridHelper = new GridHelper();
      let aggregate = gridHelper.calculateAggregate(rows, this.pagination, this.aggregates, footer);
      this.total.set(key, aggregate)
      return aggregate;
    }
    return {}
  }

  private _initializeGridData(kData: KendoServerResult) {
    this.model = kData.model;
    this.columns = kData.columns;
    this.rows = kData.rows;

    this.resetGroupColumns();

    this.rowsLoaded.emit();
  }

  private _feedEmptyGrid() {
    this.model = {};
    this.columns = [];

    this.resetGroupColumns();

    return process([], this.pagination.gridState);
  }


  private _updateSelectAllColumnBasedOnSelectedRows() {
    if (this.selectable.selectable.enabled)
      this.onSelectedKeysChange(null);
  }

  manageSelectedRows() {
    // if(this.selectable.selectable.enabled) {
    //   this.selectedRows = this.rows.filter(s => s['Selected']).map(s => s[this.rowId]);
    // this.selectable.preselectedRows.selectedRows =
    //}
  }



  //@ContentChildren(ColumnComponent) contentColumns!: QueryList<ColumnComponent>;
  // const inCols: Array<ColumnBase> =  this.contentCmdColumns.toArray().concat(this.contentColumns.toArray());
  // Le colonne inserite nel shadow dom della componente padre.
  @ContentChildren(CommandColumnComponent) contentCmdColumns!: QueryList<CommandColumnComponent>;

  initialized: boolean = false;
  createColumnsDynamically(kdata: KendoServerResult) {

    if (this.contentCmdColumns.length == 0) {
      return;
    }

    this.addShadowDomColumnsToTheGrid(this.grid.columns);
  }

  reorderMissplacedColumnsAfterUpdate() {
    // Must be called, otherwise we risk performing behavior on non updated data.
    this.changeDetector.detectChanges();

    let cmdColumns = this.grid.columns.filter((col) => col instanceof CommandColumnComponent);
    let colsToReorder = [...cmdColumns.reverse()];

    const alreadyApplied = columnsAlreadyInView(this.contentCmdColumns, this.grid.columns);
    if (!alreadyApplied) {
      colsToReorder = [...colsToReorder, , ...this.contentCmdColumns.toArray()];
    }


    colsToReorder.forEach(col => {
      this.grid.reorderColumn(col, 0, { before: false })
    });

    //Riordino le colonne in base a come erano state salvate nella vista se sono state
    //messe prime le colonne con i comandi
    if (colsToReorder && colsToReorder.length > 0 && this.loadedView && this.loadedView.Colonne && this.loadedView.Colonne.length > 0) {
      let NotCmdColumn = this.loadedView.Colonne.filter(c => c.field && c.field !== "");

      if (NotCmdColumn && NotCmdColumn.length > 0) {
        NotCmdColumn.forEach((c: KendoGridColumn, index: number) => {
          let columnbase: ColumnBase = this.grid.columns.toArray().find(columnbase => columnbase.title === c.title);
          let newIndex: number = colsToReorder.length + index;

          this.grid.reorderColumn(columnbase, newIndex, { before: false })
        });
      }
    }

    this.changeDetector.detectChanges();
  }

  addShadowDomColumnsToTheGrid(alreadyAppliedColumns: QueryList<ColumnBase>) {
    this.changeDetector.detectChanges();
    const applied = columnsAlreadyInView(this.contentCmdColumns, this.grid.columns);

    if (applied) {
      return;
    }


    let checkboxColumns = alreadyAppliedColumns.filter(s => s instanceof CheckboxColumnComponent);
    let cmdColumns = alreadyAppliedColumns.filter(s => s instanceof CommandColumnComponent);
    let colComp = alreadyAppliedColumns.filter(s => s instanceof ColumnComponent);
    this.changeDetector.detectChanges();

    let result = [...checkboxColumns, ...this.contentCmdColumns, ...cmdColumns, ...colComp];

    this.grid.columns.reset(result);
    this.changeDetector.detectChanges();
    this.reorderMissplacedColumnsAfterUpdate();
  }


  viewInit: Subject<void> = new Subject();
  /**
   * Called one time once the data is set inside the grid.
   */
  async ngAfterViewInit() {
    this.privateService.pipe(takeUntil(this.unsubSignal)).subscribe(
      (kdata) => {

        this.createColumnsDynamically(kdata);
        this.dataWasReadApplyRenderer();
        this.publicService.parseQueryParams();
        this.autofitColumns(this.grid.columns);
        this.adjustGridHeightBasedOnContent();
        // visibleCols(this.grid.columns.toArray())
      });

  }

  private _loadedViewApplied = false;
  ngAfterViewChecked(): void {
    let tbs = document.getElementsByTagName('table')
    for (let i = 0; i < tbs.length; i++) {
      tbs.item(i).style.width = '100%';
    }

    if (!this._loadedViewApplied && this.loadedView) {
      let cols = this.publicService.gridComp.columns.toArray()

      this.loadedView.Colonne.forEach(c => {
        let column = cols.find(col => col.title === c.title)
        if (!column) return;
        column.hidden = c.hidden || column.hidden;
        column.width = c.width || column.width;
      })
      this._loadedViewApplied = true;
    }
  }
  private createUsecaseServices() {
    this.currentScrollingMode = ScrollingModeFactory.Create({
      mode: this.pagination.scrollingType,
      componentContext: this
    });
  }


  autofitInitialized: boolean = false;
  private autofitColumns(columnsQL: QueryList<ColumnBase>) {
    if (!this.resizable.autoFitColumns) {
      return;
    }

    const getColumnsEligibleForAutofit = (columns) => {
      return columns.filter(column => _isNull(column.width) /*&& !column.isCheckboxColumn*/ && !column.hidden);
    }

    const autoFitColumns = () => {
      let columnAutofittable = getColumnsEligibleForAutofit(columnsQL)
      let chkCol = columnsQL.filter((column: any) => column.isCheckboxColumn);
      if (chkCol != undefined && chkCol[0] != undefined) {
        chkCol[0].width = 40;
      }

      if (columnAutofittable && columnAutofittable.length > 0)
        this.grid.autoFitColumns(columnAutofittable);
      // this.grid.autoFitColumns();
    }

    this.ngZone.onStable
      .asObservable()
      .pipe(take(1))
      .subscribe(autoFitColumns);
  }

  private dataWasReadApplyRenderer() {
    const readEvent: ReadEvent = {
      columns: this.grid.columns.toArray()
    }
    this.applyRendererGiven(readEvent);
  }

  private applyRendererGiven(eventType: RendererGridEventType) {
    const opts: RendererGridEvent = {
      grid: this.grid,
      gridElRef: this.gridElRef,
      event: eventType
    };

    const applyRendererIfDefined = (opts: RendererGridEvent) => {
      const _applyRendererRules = () => {

        this.changeDetector.detectChanges();

        this.publicService.gridElRef = this.gridElRef;
        this.publicService.gridComp = this.grid;
        this.publicService.giasGridComponent = this;

        this.config.applyRendererRules(opts);
        this._adaptHeightOnContainer();
        this.changeDetector.detectChanges();
      }

      this.ngZone.onStable
        .asObservable()
        .pipe(take(1))
        .subscribe(_applyRendererRules);
    }

    applyRendererIfDefined(opts);
  }

  /**
   * Editing
   */

  private createFormGroup(row: KendoGridRow) {
    this.formGroup = new FormGroup({});
    let helper = new GridHelper();

    Object.keys(this.model).forEach((key: string) => {
      let c: KendoGridColumn;
      let ctrlInput: any;

      c = this.privateService.getColumns().find((col) => col.field == key);
      const field: ModelEntry = this.model[key];

      if (c) {
        switch (field.type) {
          case this.STRING:
            ctrlInput = row?.[c.field] ?? (c.string?.defaultValue ?? '');
            break;

          case this.DATE:
          case this.DATETIME:
            ctrlInput = this.intlService.parseDate(<string>row?.[c.field])
              ?? (c.date?.defaultValue ?? new Date());
            break;

          case this.MULTI_DROPDOWNLIST:
            let mddlHelper = new DropdownHelper();
            ctrlInput = mddlHelper.getMultiDropdownCtrlInput(c, row);
            break;

          case this.DROPDOWNLIST:

            if (!c.ddl?.valuePrimitive) {
              let valueCod = row?.[c.ddl.formControlName] ?? (c.ddl?.defaultValue ?? null);
              if (typeof (valueCod) == 'number' || typeof (valueCod) == 'string') {
                const objDdl = {};
                objDdl[c.ddl.valueField] = valueCod;
                objDdl[c.ddl.textField] = row?.[c.ddl.formControlValue] ?? (c.ddl?.defaultValue ?? null);
                valueCod = objDdl;
              }

              helper.setCustomValidatorRow(c, row);

              const formCtrlValue = new FormControl(valueCod, c.validators);
              this.formGroup.addControl(c.ddl.formControlName, formCtrlValue);
              return;
            } else {

              helper.setCustomValidatorRow(c, row);
              ctrlInput = row?.[c.ddl.formControlName] ?? (c.ddl?.defaultValue ?? null);
            }
            break;

          case this.BOOLEAN:
            ctrlInput = row?.[c.field] ?? (c.boolean?.defaultValue ?? false);
            break;

          case this.NUMERIC:
            ctrlInput = row?.[c.field] ?? (c.numeric?.defaultValue ?? 0);
            break;

        }
        const formCtrl = new FormControl(ctrlInput, c.validators);
        if (c.disabledRule && c.disabledRule(row))
          formCtrl.disable();

        this.formGroup.addControl(c.field, formCtrl);
      } else {
        switch (field.type) {
          case this.STRING:
            ctrlInput = row?.[key];
            break;
          case this.DATE:
          case this.DATETIME:
            ctrlInput = this.intlService.parseDate(<string>row?.[key]);
            break;

          case this.DROPDOWNLIST:


          case this.BOOLEAN:
            ctrlInput = row?.[key];
            break;

          case this.NUMERIC:
            ctrlInput = row?.[key];
            break;
        }
        const formCtrl = new FormControl(ctrlInput);
        this.formGroup.addControl(key, formCtrl);
      }
    });

    // ! Hook to enable form group control from the public API.
    this.publicService.formGroup.next(this.formGroup);

  }


  public getDate(row: any, column: KendoGridColumn): any {
    const date = this.intlService.parseDate(<string>row[column.field]);
    return date;
  }


  public onOpenReloadDDL(event: DropdownListEvent, formgroup: FormGroup, forceReload: boolean) {
    let helper = new GridHelper();

    let component = null;
    if (event.data instanceof GridMultiDropdownComponent) {
      component = event.data as GridMultiDropdownComponent;
    }

    const dati = { component: component, columns: this.columns, row: formgroup.value };
    helper.loadDropdownListOnCellOpened(dati, forceReload);

  }


  /**
   * Used for displaying validation messages.
   */
  get formControls() {
    return this.formGroup.controls;
  }


  /**
   * Used for dinamically updating different dropdown lists.
   */
  public ddlStateChanged(event: DropdownListEvent, formgroup: FormGroup, col: KendoGridColumn) {
    this.privateService.dropdownListSubject.next({
      id: event.id,
      type: DropdownEventType.ON_CHANGE_VALUE,
      data: event.data,
      listItems: event.listItems
    });


    let helper = new GridHelper();


    if (col.ddl.descriptionField != null && col.ddl.descriptionField != '') {
      if (helper.columnContainsSimpleDropdownList(this.model, col.field)) {
        const val = event.listItems.find((el) => el.id == event.data);
        if (val) {
          formgroup.controls[col.ddl.descriptionField].setValue(val.name);
        }
      } else if (helper.columnContainsMultiDropdownList(this.model, col.field)) {
        const values = event.listItems.filter((el) => event.data.includes(el.id));
        if (values) {
          formgroup.controls[col.ddl.descriptionField].setValue(values.map(v => v.name));
          // Notify the component that there has been a value change
          // even if the multiselect's panel has not been closed yet
          const event: GridInfoCommandEvent = {
            action: 'multiselectValueChange',
            dataItem: values,
            isNew: false,
            rowIndex: col.orderIndex,
            sender: this.grid
          };
          this.publicService.changeDetected.next(event)
        }
      }
    }

  }


  public ddlOpenEvent(event: DropdownListEvent, formGroup: FormGroup) {
    //console.log(event.type, event.id, event.data);
    const gridDropdownListComponent = event.data;
    const cols = this.columns.find((col) => col.field == gridDropdownListComponent.controlName);
    if (cols.ddl.loadOnEdit) {
      cols.ddl.loadFunction(formGroup.getRawValue()).pipe(take(1)).subscribe((data) => {
        if (data.length != 0) {
          let item = data[0];
          if (!item.hasOwnProperty('id') || !item.hasOwnProperty('name')) { }
          //throw Error('Property id and descrizione are required.');
        }

        const ddlData = data.map(el => new DropdownListItem(el[cols.ddl.id], el[cols.ddl.formControlValue], el));
        gridDropdownListComponent.data = ddlData;
        gridDropdownListComponent.source = ddlData;
        cols.ddl.data = ddlData;
      });
    }
    //console.log(cols);
  }

  public ddlReload(event: DropdownListEvent, formGroup: FormGroup, col: KendoGridColumn) {
    //console.log(event.type, event.id, event.data);
    const gridDropdownListComponent = event.data;
    const cols = this.columns.find((col) => col.field == gridDropdownListComponent.controlName);
    cols.ddl.loadFunction(formGroup.getRawValue()).pipe(take(1)).subscribe((data) => {
      if (data.length != 0) {
        let item = data[0];
        if (!item.hasOwnProperty('id') || !item.hasOwnProperty('name')) { }
      }
      const ddlData = data.map(el => new DropdownListItem(el[cols.ddl.id], el[cols.ddl.formControlValue], el));
      gridDropdownListComponent.data = ddlData;
      gridDropdownListComponent.source = ddlData;
      cols.ddl.data = ddlData;

      const row = formGroup.getRawValue();
      const ddlItem = CellDropdownNamePipe.getDdlItem(cols, row);
      if (ddlItem == null && cols.ddl.loadOnEdit) {
        const obj = new DropdownListItem(row[cols.ddl.formControlName], row[cols.ddl.descriptionField]);
        cols.ddl.data = [obj];
      }
    });

    //console.log(cols);
  }

  public onOpenCommands(data: any) {
    this.cmdDropDown.resetCommands();
    this.publicService.openCommands.next(data);
  }

  public onCommand(cmd: GridCommandItem, dataItem: any, isNew: boolean, rowIndex: number) {
    switch (cmd.action) {
      case CommandsDropDownEvents.INLINE_EDIT:
        this.editHandler({
          dataItem: dataItem,
          isNew: isNew,
          rowIndex: rowIndex,
          sender: this.publicService.gridComp
        });
        break;
      case CommandsDropDownEvents.REMOVE:
        this.removeHandler({
          dataItem: dataItem,
          isNew: isNew,
          rowIndex: rowIndex,
          sender: this.publicService.gridComp
        });
        break;
      case CommandsDropDownEvents.INFO:
        this.infoCmdHandler(dataItem, isNew, rowIndex);
        break;
      case CommandsDropDownEvents.USER_BIND:
        this.userBindCmdHandler(dataItem, isNew, rowIndex);
        break;
    }
    this.publicService.commandEvent.next({
      command: cmd,
      dataItem: dataItem,
      rowIndex: rowIndex
    });
  }

  public ButtonGridCommandName(cmd: GridCommandItem): string {

    let name: string = "";

    if (cmd.paramsforTrasloco) {
      name = this.translocoService.translate(cmd.actionName, cmd.paramsforTrasloco);
    } else {
      name = this.translocoService.translate(cmd.actionName);
    }

    return name;
  }

  public editBtnHandler(data: any, isNew: boolean, rowIndex: number) {
    this.dettagliColumn.edit(data, isNew, rowIndex);
  }

  public actionBtnHandler(data: any, isNew: boolean, rowIndex: number) {
    this.customColumn.action(data, isNew, rowIndex);
  }

  public infoCmdHandler(data: any, isNew: boolean, rowIndex: number) {
    const event: GridInfoCommandEvent = {
      action: 'info',
      dataItem: data,
      isNew: isNew,
      rowIndex: rowIndex,
      sender: this.grid
    };
    this.publicService.changeDetected.next(event);
  }

  public userBindCmdHandler(data: any, isNew: boolean, rowIndex: number) {
    const event: GridInfoCommandEvent = {
      action: 'userBind',
      dataItem: data,
      isNew: isNew,
      rowIndex: rowIndex,
      sender: this.grid
    };
    this.publicService.changeDetected.next(event);
  }

  @Input() columns: Array<KendoGridColumn>;
  @Input() rows: KendoGridRow[];
  @Input() model: KendoGridModel;

  public applyColumnReorderPolicy(event: ColumnReorderEvent) {
    const columns = this.grid.columns.toArray().sort((a, b) => a.orderIndex - b.orderIndex);
    const movedColumn = columns[event.oldIndex];
    const targetColumn = columns[event.newIndex];

    // we are trying to move a column over a command column.
    // If the command column is not reorderable we prevent it.
    if (!(targetColumn instanceof ColumnComponent) && targetColumn.reorderable != true) {
      // however, we allow command columns to do so (mainly because we need to do it
      // by hand calling this.grid.reorderColumn)
      if (movedColumn instanceof ColumnComponent) {
        event.preventDefault();
        return;
      }
    }

    this.updateViewCustomization(null);
    // this.autofitColumns(visibleCols([event?.column]));
  }

  public applyColumnVisibilityChangePolicy(event: ColumnVisibilityChangeEvent) {

    this.updateViewCustomization(true);
    // this.autofitColumns(visibleCols(event?.columns));

  }

  /**
   * Grid customizations.
   */
  rereadRowsOnViewUpdate: boolean = true;
  public updateViewCustomization(leaveRereadRowsUntouched: boolean) {
    if (this.views.enabled) {
      this.customizationService.setCurrentViewModified();

      if (!leaveRereadRowsUntouched)
        this.rereadRowsOnViewUpdate = false;
      this.customizationViewChanged = false;
      this.changeDetector.detectChanges();
      this.customizationViewChanged = true;

      /*let col = this.mapKendoGridColumnfromColumnComponent(this.grid.columns);
      this.privateService.reReadRows(this.model, col, false);*/
    }
  }

  private mapKendoGridColumnfromColumnComponent(col: QueryList<ColumnBase>): KendoGridColumn[] {

    let kendogridCol: KendoGridColumn[] = [];

    const columns = col
      .toArray()
      .flatMap(x => 'children' in x ? (x as any).children.toArray() : x);


    columns.forEach(columnComponent => {

      if (columnComponent instanceof ColumnComponent) {
        const savedColumn: KendoGridColumn = new KendoGridColumn({ field: '', title: '' });

        Object.keys(savedColumn).forEach((key: string) => {
          savedColumn[key] = columnComponent[key];

          if (key === 'filter') {
            this.customizationService.manageCellTypes(columnComponent as ColumnComponent, savedColumn, key, this.columns);
          }
        });

        kendogridCol.push(savedColumn);
      }


    });

    return kendogridCol;
  }

  public views: GridCustomizations;
  public customizationViewChanged: boolean;
  /** Vista caricata tra quelle salvate dall'utente. */
  private loadedView: InMemoryView = null;

  private lastView: InMemoryView;
  public applyView(view: InMemoryView) {
    const restoreLostData = (cols: KendoGridColumn[]) => {
      return this.privateService.restoreLostData(cols);
    }

    if (!view) {

      //Reimposto lo stato di default della griglia
      if (this.privateService.defaultGridState)
        this.pagination.gridState = this.privateService.defaultGridState;
      //this.mostraCheckECmdColumn();
      this.privateService.reReadRows(this.model, [], true);
      return;
    }

    const settings = this.mapGridSettings(view);

    this.loadedView = settings;
    this.pagination.gridState = settings.Stato;
    let cambioVista: boolean = false;
    if (this.lastView?.NomeVista != view.NomeVista) {
      cambioVista = true;
    }
    if (this.rereadRowsOnViewUpdate || cambioVista) {
      //this.mostraCheckECmdColumn();
      this.privateService.reReadRows(this.model, restoreLostData(settings.Colonne), false);
    }
    else {
      this.rereadRowsOnViewUpdate = true;
    }
    this.changeDetector.detectChanges();
    this.lastView = cloneDeep(view);

  }

  private mostraCheckECmdColumn() {
    // let event: ColumnVisibilityChangeEvent;
    // event = new ColumnVisibilityChangeEvent([]);
    // if(this.checkboxColumn && this.selectable?.checkboxIsEnabled){
    //   this.checkboxColumn.hidden = false;
    //   this.checkboxColumn.orderIndex = 0;
    //   event.columns.push(this.checkboxColumn);
    // }
    // if (this.commandColumn){
    //   this.commandColumn.hidden = false;
    //   this.commandColumn.orderIndex = 0;
    //   event.columns.push(this.commandColumn);
    // }
    // this.initParams();
    // //this.columnVisibilityChangeHandler(event);
    // this.applyRendererGiven(event);
  }

  private mapGridSettings(view: InMemoryView): InMemoryView {
    const state = view.Stato;
    mapDateFilter(state.filter, this.model, this.columns);
    view.Colonne = this.mapColumns(view.Colonne, this.columns)

    return {
      IdVista: view.IdVista,
      IsModified: null,
      IsNew: null,
      NomeVista: view.NomeVista,
      NomeUtente: view.NomeUtente,
      GridId: view.GridId,
      Predefinita: view.Predefinita,
      Stato: state,
      Colonne: view.Colonne.sort((a, b) => a.orderIndex - b.orderIndex),//view.Colonne // view.Colonne.sort((a, b) => a.orderIndex - b.orderIndex)
      FiltroJSON: view.FiltroJSON,
      FlagPubblica: view.FlagPubblica
    };
  }

  /**
   *
   * @param viewColumns - le colonne della vista
   * @param columns - le colonne presenti in griglia (attuali)
   * @returns le colonne una volta applicata la vista
   */


  public onColumnResize() {
    this.rereadRowsOnViewUpdate = false;
    this.updateViewCustomization(true);
  }


  /// End grid customizations

  /**
   * Export data via excel.
   * Generates the data required for exporting to Excel.
   *
   * This method processes the current grid data, applying grouping, sorting,
   * and filtering based on the grid's state. It then maps the processed data
   * into a format suitable for Excel export using a helper function. The
   * mapping concerns the colums labeled as dropdown or multiselect.
   *
   * @returns {ExcelExportData} The formatted data ready for Excel export.
   *
   * If there's the need to handle the serialization of custom columns, modify
   * the mapping function in the helper {@link GridHelper.mapForExcelExport}.
   */
  public excelData() {
    let helper = new GridHelper();
    //let dataForExcel = helper.mapForExcelExport(this.rows.slice(), this.columns);
    let dataForExcel = this.rows.slice();
    let data = process(dataForExcel, {
      group: this.pagination.gridState.group,
      sort: this.pagination.gridState.sort,
      filter: this.grid.filter
    })
    let dataForExcel2 = helper.mapForExcelExport(data.data, this.columns);
    const result: ExcelExportData = {
      data: dataForExcel2
    };

    if (this.pagination.gridState.group && this.pagination.gridState.group.length > 0) {
      result.group = this.pagination.gridState.group;
    }

    console.log('result', result);
    return result;
  }

  public selectAllState: SelectAllCheckboxState = "unchecked";
  public rowId;

  private selectAllChecked = false;

  public onSelectAllChange(checkedState: SelectAllCheckboxState) {
    this.selectAllChecked = true;
    const filteredRows = process(this.rows, { filter: this.grid.filter }).data;

    if (checkedState === "checked") {
      // Checks only the filtered ones
      this.rows.forEach(s => s['Selected'] = filteredRows.includes(s));
      this.selectAllState = "checked";
    } else {
      // Unchecks everything
      this.rows.forEach(s => s['Selected'] = false);
      this.selectAllState = "unchecked";
    }

    this.publicService.next({
      stopPropagation: true,
      forceRefresh: false,
      data: this.GetDatiAttuali(),
      afterEdit: false
    });

    let selectedRow = this.rows.filter((r) => r['Selected'] == true)
    let deselectedRow = this.rows.filter((r) => r['Selected'] == false)

    let selectedRowArgs: RowArgs[] = selectedRow.map((e, index) => {
      return {
        dataItem: e,
        index: index
      }
    })
    let deselectedRowArgs: RowArgs[] = deselectedRow.map((e, index) => {
      return {
        dataItem: e,
        index: index
      }
    })

    if (this.selectable.preselectedRows.selectionChangeFn) {
      this.selectable.preselectedRows.selectionChangeFn({
        selectedRows: selectedRowArgs,
        deselectedRows: deselectedRowArgs
      }, this);
    }
    setTimeout(() => {
      this.selectAllChecked = false;
    }, 1);
  }

  public onSelectedKeysChange(ev: string[]) {
    const selected = this.rows.some(s => s['Selected']);
    const unselected = this.rows.some(s => !s['Selected'])

    if (selected && unselected) {
      this.selectAllState = "indeterminate";
    } else if (!selected) {
      this.selectAllState = "unchecked";
    } else {
      this.selectAllState = "checked";
    }
  }

  public GetDatiAttuali() {
    return {
      rows: this.rows,
      columns: this.columns,
      model: this.model
    }
  }

  public filterableGrid() {
    if (this.columns) {
      let index = this.columns.findIndex((c) => { return c.filterable == true; });
      if (index >= 0) {
        return true;
      }
    }
    return false;
  }

  showEditButton(dataItem): boolean {
    let show: boolean = false;

    if (this.cmdColumn.editBtn || this.cmdColumn.showEditBtn(dataItem))
      show = true;

    return show;
  }

  getColumnStyle(col: KendoGridColumn): { [p: string]: string } {
    if (col) {
      let field = this.model[col.field]

      if (field?.type == CELL_TYPES.NUMBER && col.style == undefined) {
        return { "text-align": "right" }
      } else {
        return col.style;
      }
    }
    return null;
  }

  getColumnFooterStyle(col: KendoGridColumn): { [p: string]: string } {
    if (col) {
      let field = this.model[col.field]
      if (field?.type == CELL_TYPES.NUMBER && col.footerStyle == undefined) {
        return { "text-align": "right" }
      } else {
        return col.footerStyle;
      }
    }
    return null;
  }

  setSelectedRows(opts: SelectedOpts) {
    if (opts.resetPreviousSelection)
      resetSelection(this.rows);

    let firstMatchIndex = -1;
    opts.keys.forEach((key: number | string) => {
      let index = findMatchedIndex(key, this.rows, this.rowId, opts);

      if (index >= 0) {
        this.rows[index]['Selected'] = true;
        if (firstMatchIndex === -1)
          firstMatchIndex = index;
      }
    });
    this.gridState.skip = Shared.changeGridPage(firstMatchIndex, this.pageSize);
    this.refresh();
  }

  public deselectRows(rows: RowArgs[]): void {
    resetSelection(this.rows.filter(row => {
      return rows.map(r => r.dataItem).includes(row);
    }));
  }

  public deselectAllRows(): void {
    resetSelection(this.rows);
  }

  resetMulticheckStore() {
    this.worker.resetStore();
  }

  refresh(): void {
    this.publicService.refresh(false, this.GetDatiAttuali());
  }

  forceReload(): void {
    this.publicService.refresh(true);
  }

  @Output() detailCollapse = new EventEmitter<DetailCollapseEvent>();
  detailMasterRowCollapse(event: DetailCollapseEvent) {

    if (this.masterdetailSettings.IsGridMaster()) {
      this.kendoGridMasterService.Last_RowExpanded = null;
    }

    this.detailCollapse.emit(event);
  }

  @Output() detailExpand = new EventEmitter<DetailExpandEvent>();
  detailMasterRowExpand(event: DetailExpandEvent) {

    if (this.masterdetailSettings.IsGridMaster()) {
      this.kendoGridMasterService.Last_RowExpanded = event.dataItem;
    }

    this.detailExpand.emit(event);
  }

  showCmdDdlButton(dataItem): boolean {
    let show: boolean = false;

    if (this.showCmdDDL && !this.cmdDropDown.hidecmdDropDown(dataItem))
      show = true;

    return show;
  }

  IsVisibleCmdClumn() {
    let visible: boolean = false;
    if (this.showCmdColumn || this.showCmdDDL) {
      visible = true;
    }

    return visible;
  }

  // Function used to keep the original order in the columnGroups
  defaultOrder(a: any, b: any) {
    return 0;
  }

  get hasBatchChanges(): boolean {
    return this.gridBatchHandlerService.hasChanges;
  }

  public useCustomColumnCellTemplate(): boolean {

    let applyCustomColumnTemplate: boolean = false;

    if (this.customCommandColumnCellTemplate && this.customColumn.useCustomColumnCellTemplate) {
      applyCustomColumnTemplate = true;
    }

    return applyCustomColumnTemplate;

  }

  mapColumns(viewColumns: KendoGridColumn[], columns: KendoGridColumn[]) {
    let colonneRet: KendoGridColumn[] = viewColumns.map((colonna) => {
      let col = columns.find((col) => col.field == colonna.field);
      let colCopy = cloneDeep(colonna);
      if (col) {
        colonna = col;
      }

      // for (var variableKey in colCopy){
      //     if (colCopy.hasOwnProperty(variableKey)){
      //         colonna[variableKey] = colCopy[variableKey];
      //     }
      // }
      colonna.width = colCopy.width;
      colonna.field = colCopy.field;
      //colonna.title = colCopy.title;
      colonna.hidden = colCopy.hidden;
      colonna.orderIndex = colCopy.orderIndex;

      return colonna;
      // if (col && col.date != null) {
      //     colonna.date = col.date;
      //     return colonna
      // } else {
      //     return colonna;
      // }
    });

    //Aggiungo le nuove colonne che potrebbero essere state aggiunte nel frattempo dallo sviluppatore
    //ma che non sono presenti nella vista
    if (colonneRet.length > 0) {
      columns.forEach(c => {
        if (colonneRet.findIndex(i => c.field && i.field === c.field) === -1) {
          colonneRet.push(c);
        }
      });
    }
    return colonneRet;
  }

  public useCustomColumnHeaderTemplate(): boolean {

    let applyCustomColumnTemplate: boolean = false;

    if (this.customCommandColumnHeaderTemplate && this.customColumn.useCustomColumnHeaderTemplate) {
      applyCustomColumnTemplate = true;
    }

    return applyCustomColumnTemplate;

  }

  public showBtncustomColumn(isNew: boolean): boolean {
    let show: boolean = false;

    if (!isNew && this.customColumn.showBtn) {
      show = true;
    }

    return show;
  }

  public useColumnGroup(group): boolean {

    let use: boolean = false;

    if (group.key && group.key !== "") {
      use = true;
    }

    return use;

  }

  public showAgronicakendoGridCellTemplate(cellType: CELL_TYPES, col: KendoGridColumn): boolean {
    return (cellType === CELL_TYPES.DATE || cellType === CELL_TYPES.DATETIME || cellType === CELL_TYPES.DROPDOWNLIST ||
      cellType === CELL_TYPES.MULTI_DROPDOWNLIST || cellType === CELL_TYPES.CUSTOM || cellType === CELL_TYPES.NUMBER ||
      cellType === CELL_TYPES.BOOLEAN || this.ShowHTMLAsString(cellType, col));
  }

  public showAgronicakendoGridGroupHeaderTemplate(cellType: CELL_TYPES) {
    let show: boolean = false;

    if (this.groups.groupable && (cellType === CELL_TYPES.DROPDOWNLIST || cellType === CELL_TYPES.MULTI_DROPDOWNLIST)) {

      show = true;

    }

    return show;
  }

  public showAgronicakendoGridFooterTemplate(cellType: CELL_TYPES) {
    let show: boolean = false;

    if (this.aggregates.enabled && cellType === CELL_TYPES.NUMBER) {

      show = true;

    }

    return show;
  }

  public getAggregateResult(aggregateSetting: AggregateResult, col: KendoGridColumn, aggregate: string): number {
    let n = aggregateSetting?.[col.field]?.[aggregate]
    return n;
  }

  public showkendoGridFilterMenuTemplate(cellType: CELL_TYPES, column: any): boolean {
    let show: boolean = false;

    if (this.showMultiCheckFilter(cellType, column) || this.showAgronicakendoGridFilterMenuTemplate(cellType)) {

      show = true;

    }

    return show;
  }

  public showAgronicakendoGridFilterMenuTemplate(cellType: CELL_TYPES): boolean {
    return false;
  }

  public showMultiCheckFilter(cellType: CELL_TYPES, column: KendoGridColumn) {
    if (cellType === CELL_TYPES.STRING || cellType === CELL_TYPES.DROPDOWNLIST || cellType === CELL_TYPES.DATE
      || cellType === CELL_TYPES.DATETIME || cellType === CELL_TYPES.MULTI_DROPDOWNLIST) {
      return true;
    }
    if (cellType === CELL_TYPES.NUMBER && column.numeric?.multiCheckFiltering) {
      return true;
    }
    return false;
  }

  public showAgronicakendoGridEditTemplate(cellType: CELL_TYPES) {
    return (cellType === CELL_TYPES.NUMBER || cellType === CELL_TYPES.DATE || cellType === CELL_TYPES.DATETIME ||
      cellType === CELL_TYPES.DROPDOWNLIST || cellType === CELL_TYPES.MULTI_DROPDOWNLIST ||
      cellType === CELL_TYPES.BOOLEAN || cellType === CELL_TYPES.CUSTOM);
  }

  public sortChange(sort: SortDescriptor[]): void {
    let sortNew: SortDescriptor[] = []
    let sortCopy: SortDescriptor[] = []
    let allDescriptionField: string[] = this.getAllDescriptionField();
    sort.forEach((s) => {
      if (allDescriptionField.indexOf(s.field) < 0) {
        sortCopy.push(s);
      }
    })
    sortCopy.forEach((s) => {
      if (s.dir != undefined) {
        let col = this.columns.find((c) => c.field == s.field)
        if (col && col.ddl && col.ddl?.descriptionField != "" && sortNew.findIndex((s1) => s1.field == col.ddl?.descriptionField) < 0) {
          let d = { dir: s.dir, field: col.ddl.descriptionField }
          let sDescriptionField: SortDescriptor = d
          sortNew.push(sDescriptionField);
        }
        sortNew.push(s);
      }
    })
    // console.log(sortNew);
    this.pagination.gridState.sort = sortNew;
  }

  getAllDescriptionField(): string[] {
    let df = this.columns.filter((c) => c.ddl != undefined).
      filter((c) => c.ddl.descriptionField != undefined)
      .map((c) => c.ddl.descriptionField);
    if (df) {
      return df;
    }
    return [];
  }
  
  /**
   * This will just run changeDetector.detectChanges()
   */
  repaint(): void {
    this.changeDetector.detectChanges();
  }
}


type DropdownInfo = { descriptionField: string, valueField: string };
class GridHelper {

  /**
   * Excel export.
   * Maps rows and columns of a Kendo Grid for Excel export by transforming dropdown fields
   * into their corresponding text descriptions.
   *
   * @param rows - An array of `KendoGridRow` objects representing the grid's data rows.
   * @param columns - An array of `KendoGridColumn` objects representing the grid's column definitions.
   * @returns An array of transformed rows where dropdown fields are replaced with their text descriptions.
   *
   * The method processes each row and updates fields specified in the column definitions
   * by replacing dropdown value fields with their corresponding text descriptions. If the field
   * contains an array of values, they are joined into a comma-separated string.
   *
   * Debugging:
   * Logs the rows, columns, and dropdown information to the console for debugging purposes.
   */
  public mapForExcelExport(rows: KendoGridRow[], columns: KendoGridColumn[]): any[] {
    let resultRows = [];
    let ddlInfos = this.getDropdownsDescriptionField(columns);
    console.debug('KENDO_GRID_DEBUG::MAP_ROWS::', rows, columns, ddlInfos);

    for (let row of rows) {
      ddlInfos.forEach(info => {
        let textDescription = "";
        if (Array.isArray(row[info.descriptionField]) && row[info.descriptionField].length > 0) {
          textDescription = row[info.descriptionField].join(', ');
        } else {
          textDescription = row[info.descriptionField]
        }
        row[info.valueField] = textDescription;
      });

      resultRows.push(row)
    }

    return resultRows;
  }

  private getDropdownsDescriptionField(columns: KendoGridColumn[]): DropdownInfo[] {

    return columns.filter(s => s.ddl != null).map(col => {
      let descField = col.ddl.descriptionField;
      let valueField = col.ddl.formControlName;

      if (descField == null || valueField == null)
        throw Error(`Una colonna non è stata configurata bene. ValueField: ${valueField}, descField: ${descField}`);

      const info: DropdownInfo = {
        descriptionField: descField,
        valueField: valueField
      };

      return info;
    });
  }

  // END Excel Export

  setCustomValidatorRow(c: KendoGridColumn, row: KendoGridRow) {
    c.validators?.forEach(s => {
      if (s instanceof PropertyValidator) {
        s.setCurrentRow(row);
      }
    })
  }

  private getDigitsFromFormat(format: string) {
    if (format != '' || format != undefined) {
      return toNumber(format.slice(1));
    } else {
      return 0;
    }
  }

  calculateAggregate(rows: KendoGridRow[], pagination: PaginationSettings, aggregates: AggregateSettings, footer: boolean): AggregateResult {
    const take = pagination.gridState.take;
    pagination.gridState.take = undefined;
    const proc = process(rows ?? [], pagination.gridState);
    let dataCopy = cloneDeep(proc.data);
    let data = this.appiattisciData(dataCopy);
    const total = aggregateBy(data, aggregates.descriptors);
    //const total = aggregateBy(proc.data, aggregates.descriptors);
    // const format = this.getDigitsFromFormat(aggregates.descriptors[0].format);
    // aggregates.descriptors.forEach(d => {
    //   d.field
    //   if (total[d.field] != undefined) {
    //     total[d.field].sum = toNumber(total[d.field]?.sum?.toFixed(format));
    //   }
    // });
    pagination.gridState.take = take;
    return total;
  }

  appiattisciData(data: any): KendoGridRow[] {
    let rows = []
    if (data?.length > 0 && data[0].field && data[0].aggregates) {
      data.forEach((item) => {
        let rowAppiattite = this.appiattisciData(item)
        rowAppiattite.forEach((el) => {
          rows.push(el)
        })
      });
    } else if (data?.length > 0) {
      data.forEach((item) => rows.push(item));
    } else if (data?.items?.length > 0) {
      data.items.forEach((item) => {
        let rowAppiattite = this.appiattisciData(item)
        rowAppiattite.forEach((el) => {
          rows.push(el)
        })
      });
    } else {
      rows.push(data);
    }
    return rows;
  }

  columnContainsSimpleDropdownList(model: KendoGridModel, field: string) {
    return model[field].type === CELL_TYPES.DROPDOWNLIST;
  }

  columnContainsMultiDropdownList(model: KendoGridModel, field: string) {
    return model[field].type === CELL_TYPES.MULTI_DROPDOWNLIST;
  }

  getMultiDDLDataIfMissing(row, col): { missingItemInSource: boolean; newDDLData: DropdownListItem[] } {
    const ids: any[] = row[col.field];
    const names = row[col.ddl.descriptionField];
    const missingInSource = col.ddl.data.some((el) => !ids.includes(el.id) && !names.includes(el.name));

    let items = [];
    if (missingInSource) {
      let data = ids.map((id, index) => new DropdownListItem(id, names[index]));
      items.push(...data)
    }

    return {
      missingItemInSource: missingInSource,
      newDDLData: items
    }
  }


  loadDropdownListOnCellOpened(dati: LoadDropdownListOnCellOpened, forceReload: boolean) {
    let columns = dati.columns;
    let component = dati.component;
    let row = dati.row;

    const col = columns.find((col) => col.field == component.controlName);
    if (col.ddl.loadOnEdit || forceReload) {
      col.ddl.loadFunction(row).pipe(take(1)).subscribe((data) => {
        const ddlData = data.map(el => new DropdownListItem(el[col.ddl.id], el[col.ddl.formControlValue], el));
        component.data = ddlData;
        component.source = ddlData;
        col.ddl.data = ddlData;
      });
    }
  }

}

type LoadDropdownListOnCellOpened = {
  component: any,
  columns: KendoGridColumn[],
  row: KendoGridRow;
}
function elementQueuedForDeletion(item: any) {
  return item['pending-deletion-row'];
}

