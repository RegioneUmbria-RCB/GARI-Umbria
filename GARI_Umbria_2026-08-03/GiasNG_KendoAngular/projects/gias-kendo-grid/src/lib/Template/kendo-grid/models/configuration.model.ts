import { faEdit, faTrashAlt, faUser } from '@fortawesome/free-solid-svg-icons';
import { GroupableSettings, PagerSettings, RowArgs, SelectableMode, SelectionEvent } from '@progress/kendo-angular-grid';
import { AggregateDescriptor, SortDescriptor, State } from '@progress/kendo-data-query';
import { cloneDeep } from 'lodash';
import { KendoGridRow } from './grid.model';
import { TemplateRef } from '@angular/core';
import { GridCommandItem } from '../../../shared/utils';
import { TranslocoService } from '@jsverse/transloco';
import { BehaviorSubject, Observable } from 'rxjs';


export class AgrSelectableSettings {
  get checkboxIsEnabled() {
    return this.selectable.enabled && this.shouldShowCheckbox;
  }

  constructor(
    public selectable?: SelectableSettings,
    public columnSettings?: SelectableColumnSettings,
    public preselectedRows?: PreselectedRowsSettings,
    public shouldShowCheckbox?: boolean) {
    this.selectable = new SelectableSettings({});
    this.columnSettings = new SelectableColumnSettings();
    this.shouldShowCheckbox = false;
    if (!preselectedRows) {
      this.preselectedRows = new PreselectedRowsSettings();
    }
    this.preselectedRows.isRowSelectedFn = () => false;
    this.preselectedRows.selectionChangeFn = () => {
      let a = 0; //Commento per funzione vuota SonarQube
    }
  }
}

/**
 * Angular Documentation SelectableSettings
 * https://www.telerik.com/kendo-angular-ui/components/grid/api/SelectableSettings/
 */
export class SelectableSettings {
  /**
   * cell? boolean (default: false)
   * Determines if cell selection is allowed.
   */
  cell: boolean;
  /**
   * checkboxOnly? boolean (default: true)
   * Determines if the selection is performed only through clicking a checkbox.
   * If enabled, clicking the row itself will not select the row.
   * Applicable if at least one checkbox column is present.
   */
  checkboxOnly: boolean;
  /**
   * drag? boolean (default: false)
   * Determines if drag selection is allowed.
   */
  drag: boolean;
  /**
   * enabled? boolean (default: true)
   * Determines if selection is allowed.
   */
  enabled: boolean;
  /**
   * mode? SelectableMode (default: "multiple")
   * The available values are: single, multiple
   */
  mode: SelectableMode;

  disableAllcheckbox: boolean;
  /**
   * metaKeyMultiSelect? boolean (default: true)
   * Determine whether pressing the `Ctrl` or `Command` keys is required for the selection of multiple rows
   * at the same time.
   * Referring to the method of selection over row, not through checkboxes.
   * The selection must be set to `"multiple"` to use this feature.
   */
  metaKeyMultiSelect: boolean;


  constructor(opts: Partial<SelectableSettings>) {
    this.cell = opts?.cell ?? false;
    this.checkboxOnly = opts?.checkboxOnly ?? true;
    this.drag = opts?.drag ?? false;
    this.enabled = opts?.enabled ?? false;
    this.mode = opts?.mode ?? 'multiple';
    this.disableAllcheckbox = opts?.disableAllcheckbox ?? false;
    this.metaKeyMultiSelect = opts?.metaKeyMultiSelect ?? true;
  }
}

export class PreselectedRowsSettings {

  /** For the following functions to work selectable.enabled must be true. */
  /**
   * Questa funziona viene chiamata per tutte le righe.
   */
  isRowSelectedFn: (e: RowArgs, component: any) => boolean;

  /**
   * Viene richiamata per ogni selezione soltanto una volta.
   */
  selectionChangeFn: (event: SelectionEvent, component: any) => void;

  /**
   * Default selected rows.
   * @deprecated
   */
  selectedRows: string[];

  constructor() {
    this.isRowSelectedFn = () => false;
    this.selectionChangeFn = () => {
      let a = 0; //Commento per funzione vuota SonarQube
    }
    this.selectedRows = [];
  }
}

export class SelectableColumnSettings {
  /**
   * Determines whether a select-all checkbox will be displayed in the header.
   */
  showSelectAll: boolean;

  /**
   * The title of the column.
   */
  title: string;

  /**
   * The width of the column.
   */
  width: number;

  /**
   *   Specifies if the column will be included in the column-chooser list.
   */
  includeInChooser: boolean;

  /**
   * Sticky selectable column.
   */
  sticky: boolean = true;

  /**
   * Specifies whether the column is reorderable or not.
   */
  reorderable: boolean = false;
}

export class ToolbarSettings {
  constructor(
    public newItem: boolean = false,
    public resetChanges: boolean = false,
    public title: string = 'Azioni',
    public width: number = 150,
    public title_save_button: string = '',
    public SaveBtn: boolean = true,
    public customToolbar: boolean = false
  ) { }

  showToolbar(): boolean {
    return this.newItem || this.resetChanges || this.customToolbar;
  }
}

export class AggregateSettings {
  enabled: boolean;
  descriptors: GiasAggregateDescriptor[];

  constructor(opts: Partial<AggregateSettings>) {
    this.enabled = opts?.enabled ?? false;
    this.descriptors = opts?.descriptors ?? [];
  }
}

export class GiasAggregateDescriptor implements AggregateDescriptor {
  field: string;
  aggregate: 'count' | 'sum' | 'average' | 'min' | 'max';
  format?: string;
}

export class GroupSettings {
  public groupable: GroupableSettings;

  constructor(options: Partial<GroupSettings>,
    private translocoService: TranslocoService) {
    this.groupable = options?.groupable ?? { enabled: false, showFooter: false, emptyText: this.translocoService.translate('giasgrid.grpEmptyMsg') };
  }
}

export class CommandsColumnSettings {
  public editBtn: boolean;
  public removeBtn: boolean;
  public infoBtn: boolean;
  public title: string;
  public editBtnTitle: string;
  public showEditBtn: (row: KendoGridRow) => boolean;
  public onDisableInfoBtn: (row: KendoGridRow) => boolean;
  public onDisableEditBtn: (row: KendoGridRow) => boolean;
  public onDisableRemoveBtn: (row: KendoGridRow) => boolean;
  public width: number;
  public locked: boolean;
  public media: string;
  public sticky: boolean = true;
  public reorderable: boolean = false;

  constructor(options?: Partial<CommandsColumnSettings>) {
    this.editBtn = options?.editBtn || false;
    this.removeBtn = options?.removeBtn || false;
    this.infoBtn = options?.infoBtn || false;
    this.title = options?.title || 'Comandi';
    this.editBtnTitle = options?.editBtnTitle || 'Modifica';
    this.showEditBtn = options?.showEditBtn || ((row): boolean => false);
    this.onDisableInfoBtn = options?.onDisableInfoBtn || ((row): boolean => false);
    this.onDisableEditBtn = options?.onDisableEditBtn || ((row): boolean => false);
    this.onDisableRemoveBtn = options?.onDisableRemoveBtn || ((row): boolean => false);
    this.width = options?.width || 0;
    this.locked = options?.locked;
    this.media = options?.media || null;
    this.sticky = options?.sticky ?? true;
    this.reorderable = options?.reorderable ?? false;

    /** L'info button viene gestito in un'altra colonna, quindi non devo
     * verificare nulla qui */
    if (this.width === 0) {
      switch (true) {
        case (this.editBtn && this.removeBtn):
          this.width = 80;
          break;
        case this.editBtn:
        case this.removeBtn:
          this.width = 30;
          break;
        default:
          this.width = 60;
      }
    }
  }

  public showCmdColumn(): boolean {
    return this.editBtn || this.removeBtn;
  }
}

export enum CommandsDropDownEvents {
  INLINE_EDIT = 8,
  FULL_EDIT = 10, // value chosen for compability with enum_menuAgendaGridCommands
  REMOVE = 11, // value chosen for compability with enum_menuAgendaGridCommands
  INFO = 9, // value chosen for compability with enum_menuAgendaGridCommands
  COPY = 3, // value chosen for compability with enum_menuAgendaGridCommands
  USER_BIND = 12 // value chosen for compability with enum_menuAgendaGridCommands
}

/**
 * Used to configure the grid commands' drop down button.
 *
 * ---
 * **Public APIs**
 *
 * `addCommand` - Adds an item in the dropdown. Takes as input an instance of
 * GridCommandItem.
 *
 * `removeCommand` - Removes an item from the dropdown. Takes as input the
 * command's action to remove.
 *
 * `resetCommands` - resets the commands to their default (only static are visible).
 *
 * `showCmdDropDown` - return whether the dropdown button is visible or not.
 *
 * ---
 * @usageNotes More commands can be added/removed dynamically on the dropdown's
 * opening by subscribing to `openCommands` from GridPublicService.
 */
export class CommandsDropDownSettings {
  public sticky: boolean = true;
  public width: number;
  public title: string;
  public inlineEditBtn: boolean;
  public fullEditBtn: boolean;
  public removeBtn: boolean;
  public infoBtn: boolean;
  public userBindBtn: boolean;
  public cmdList: Array<GridCommandItem>;
  private _cmdList: Array<GridCommandItem>;
  public hidecmdDropDown: (row: KendoGridRow) => boolean;

  constructor(options?: Partial<CommandsDropDownSettings>) {
    this.inlineEditBtn = options?.inlineEditBtn || false;
    this.fullEditBtn = options?.fullEditBtn || false;
    this.removeBtn = options?.removeBtn || false;
    this.infoBtn = options?.infoBtn || false;
    this.userBindBtn = options?.userBindBtn || false;
    this.title = options?.title || 'Comandi';
    this.width = options?.width || 30;
    this.sticky = options?.sticky ?? true;
    this.cmdList = options?.cmdList || [];
    this._cmdList = options?.cmdList || [];
    this.hidecmdDropDown = options?.hidecmdDropDown || ((row): boolean => false);

    this.addDefaultButtons();
  }

  public addCommand(cmd: GridCommandItem): void {
    this.cmdList.push(cmd);
    if (cmd.isStatic) this._cmdList.push(cmd);
  }

  public addCommandAt(cmd: GridCommandItem, index: number): void {
    this.cmdList.splice(index, 0, cmd);
    if (cmd.isStatic) this._cmdList.splice(index, 0, cmd);
  }

  /** Only removes non-static buttons */
  public removeCommand(cmdAction: number): void {
    let index = this.cmdList.findIndex(x => x.action === cmdAction);
    if (index === -1) return;

    this.cmdList.splice(index, 1);
    if (cmdAction === CommandsDropDownEvents.FULL_EDIT) this.fullEditBtn = false;
    if (cmdAction === CommandsDropDownEvents.INLINE_EDIT) this.inlineEditBtn = false;
    if (cmdAction === CommandsDropDownEvents.REMOVE) this.removeBtn = false;
    if (cmdAction === CommandsDropDownEvents.INFO) this.infoBtn = false;
  };

  public removeAllCommand(): void {
    if (this.cmdList.findIndex(v => v.action === CommandsDropDownEvents.FULL_EDIT)) this.fullEditBtn = false;
    if (this.cmdList.findIndex(v => v.action === CommandsDropDownEvents.INLINE_EDIT)) this.inlineEditBtn = false;
    if (this.cmdList.findIndex(v => v.action === CommandsDropDownEvents.REMOVE)) this.removeBtn = false;
    if (this.cmdList.findIndex(v => v.action === CommandsDropDownEvents.INFO)) this.infoBtn = false;

    this.cmdList = [];

    this._cmdList = [];
  };

  public showCmdDropDown(): boolean {
    return this.cmdList.length > 0 || this._cmdList.length > 0;
  }

  public resetCommands() {
    this.cmdList = cloneDeep(this._cmdList);
  }

  private addDefaultButtons() {
    if (this.userBindBtn) {
      this.addCommand(new GridCommandItem(
        "AssociaUtente",
        CommandsDropDownEvents.USER_BIND,
        '',
        faUser
      ));
    }
    if (this.infoBtn) {
      this.addCommand(new GridCommandItem(
        "Informazioni",
        CommandsDropDownEvents.INFO,
        "faInfo"
      ));
    }
    if (this.inlineEditBtn) {
      this.addCommand(new GridCommandItem(
        "Modifica",
        CommandsDropDownEvents.INLINE_EDIT,
        '',
        faEdit
      ));
    }
    if (this.fullEditBtn) {
      this.addCommand(new GridCommandItem(
        "ModificaCompleta",
        CommandsDropDownEvents.FULL_EDIT,
        'faEditFull',
      ));
    }
    if (this.removeBtn) {
      this.addCommand(new GridCommandItem(
        "Cancella",
        CommandsDropDownEvents.REMOVE,
        '',
        faTrashAlt
      ));
    }
  }
}

export class DettagliColumnSettings {

  public editBtn: boolean;
  public infoBtn: boolean;
  public title: string;
  public edit: (data: any, isNew: boolean, rowIndex: number) => void;
  public onDisableEditBtn: (row: KendoGridRow) => boolean;
  public width: number;
  public locked: boolean;
  public sticky: boolean = true;

  constructor(options?: Partial<DettagliColumnSettings>) {
    this.editBtn = options?.editBtn ?? false;
    this.title = options?.title || 'Dettagli';
    this.edit = options?.edit || ((data): void => {
      let a = 0; //Commento per funzione vuota SonarQube
    });
    this.onDisableEditBtn = options?.onDisableEditBtn || ((row): boolean => false);
    this.width = options?.width || 156;
    this.locked = options?.locked;
    this.sticky = options?.sticky ?? true;
    this.infoBtn = options?.infoBtn ?? false;

  }

  computeWidth(isMobileWidth: boolean, infoBtn: boolean): any {
    let width = 0;
    const smallBtnWidth = 45;
    const lgBtnWidth = 80;
    const margin = 25;

    if (isMobileWidth) {
      if (this.editBtn)
        width += smallBtnWidth;

      width += margin;
      if (infoBtn)
        width += smallBtnWidth;
    }
    else {
      if (this.editBtn)
        width += lgBtnWidth;

      width += margin;
      if (infoBtn)
        width += smallBtnWidth;
    }

    return width;
  }

}

export class CustomColumnSettings {
  public title: string;
  public action: (data: any, isNew: boolean, rowIndex: number) => any;
  public onDisableActionBtn: (row: KendoGridRow) => boolean;
  public width: number;
  public locked: boolean;
  public sticky: boolean = true;
  public reorderable: boolean = false;
  set showColumn(value: boolean){
    this._showColumn$.next(value);
  }
  get showColumn(): boolean{
    return this._showColumn$.getValue();
  }

  public btnIcon: string;
  public btnTooltip: string;
  public fontawesomeIcon: any = null;
  public headerStyle: { [key: string]: string };
  public includeInChooser: boolean;
  public useCustomColumnCellTemplate: boolean;
  public useCustomColumnHeaderTemplate: boolean;
  public showBtn: boolean;

  private _showColumn$ = new BehaviorSubject<boolean>(true);
  showColumn$: Observable<boolean> = this._showColumn$.asObservable();

  constructor(options?: Partial<CustomColumnSettings>) {
    this.title = options?.title || '';
    this.action = options?.action || ((data): any => {
      let a = 0; //Commento per funzione vuota SonarQube
    });
    this.onDisableActionBtn = options?.onDisableActionBtn || ((row): boolean => false);
    this.width = options?.width || 156;
    this.locked = options?.locked;
    this.sticky = options?.sticky ?? true;
    this.reorderable = options?.reorderable ?? false;
    this.showColumn = options?.showColumn;
    this.btnIcon = options?.btnIcon ?? 'faEditFull';
    this.btnTooltip = options?.btnTooltip ?? '';
    this.fontawesomeIcon = options?.fontawesomeIcon ?? null;
    this.headerStyle = options?.headerStyle ?? { 'background-color': '#428BCA', color: 'white' };
    this.includeInChooser = options?.includeInChooser ?? true;
    this.useCustomColumnCellTemplate = options?.useCustomColumnCellTemplate ?? false;
    this.useCustomColumnHeaderTemplate = options?.useCustomColumnHeaderTemplate ?? false;
    this.showBtn = options?.showBtn ?? true;
  }

  computeWidth(isMobileWidth: boolean, infoBtn: boolean): any {
    let width = 0;
    const smallBtnWidth = 45;
    const lgBtnWidth = 80;
    const margin = 25;

    width += smallBtnWidth;

    width += margin;
    if (infoBtn)
      width += smallBtnWidth;

    return width;
  }

}

export class ResizableSettings {

  // public autoSize: boolean = false NON GESTITA
  constructor(public isResizable: boolean = true,
    public autoFitColumns: boolean = true) {
    if (!this.isResizable) {
      this.autoFitColumns = false;
    }
  }
}

export class GeneralSettings {
  public height: 'auto' | string | number = 700;
  public reordable: boolean;
  public performOnEdit: boolean;
}

/*+
* @description:
* Gestisce il master detail della kendo grid
* https://www.telerik.com/kendo-angular-ui/components/grid/master-detail/
* */
export class MasterDetailSettings {

  public enable: boolean = false;

  public flag_grid_master: boolean = null;

  public flag_grid_detail: boolean = null;

  public showDetailTemplate: (dataitem: any, rowIndex: number) => boolean;

  /*
  *@description: Da valorizzare con il Template Ref della griglia di detail
  * */

  public templateGridDetail: TemplateRef<any> = null;

  constructor(enable: boolean, optional?: Partial<MasterDetailSettings>) {
    this.enable = enable;
    this.templateGridDetail = optional?.templateGridDetail;
    this.flag_grid_master = optional?.flag_grid_master ?? true;
    this.flag_grid_detail = optional?.flag_grid_detail ?? true;
    this.showDetailTemplate = optional?.showDetailTemplate || ((dataitem: any, rowIndex: number): boolean => true);
  }

  public IsGridMaster(): boolean {
    return this.enable && this.flag_grid_master;
  }

  public IsGridDetail(): boolean {
    return this.enable && this.flag_grid_detail;
  }
}
export class SortSettings {
  /**
   * @param sortable Set to false to disable sorting.
   */
  constructor(public sortable?: Sortable | boolean,
    public sort?: SortDescriptor[]) {
    this.sortable = sortable ?? { allowUnsort: true, mode: SortMode.SINGLE };
  }

  sortEnabled(): Sortable | boolean {
    return this.sortable ?? false;
  }
}

export enum ScrollingMode {
  None = 'none',
  Scrollable = 'scrollable', // default scrolling mode
  Virtual = 'virtual'
}

export class VirtualScrollingOpts {
  rowHeight: number;
  viewportHeight: number;
}

export class PaginationSettings {
  /**
   * @param gridState Initial grid state. Take is page size.
   */
  public gridState: State;

  /**
   * @param pageable If pagination should be applied.
   */
  public pageable: boolean | PagerSettings;

  /**
   * @param navigable Allows to set keyboard shortcuts for navigating the grid.
   */
  public navigable: boolean;

  public scrollingType: ScrollingMode = ScrollingMode.Scrollable;

  /**
   * Virtual scrolling options.
   */
  public virtualScrolling: VirtualScrollingOpts = new VirtualScrollingOpts();

  /**
   * Sets the number of records shown in one page.
   * To do so, it sets the take property of gridState to the provided value n.
   * If gridState is null or undefined, it initializes gridState with default
   * values including setting take to the provided value n.
   * @param n the number of records shown in one page
   */
  public take(n: number) {
    if (this.gridState) {
      this.gridState.take = n;
    } else {
      this.gridState = {
        sort: [],
        skip: 0,
        group: [],
        take: n,
        filter: {
          logic: 'and',
          filters: [],
        },
      };
    }
  }
}

export enum DeletionMode {
  HandleSingleRowDeletionOnly,
  HandleEachRowSeparately,
  HandleAllRowsTogether
}

/**
 * Settings for interacting with functionality not UI related.
 */
export class BehaviorSettings {
  public createFormGroupFromOutside: boolean;
  public saveExternalChanges: boolean;
  public excelSettings: ExcelSettings;
  public pdfSettings: PDFSettings;
  public showDeletionConfirmation: boolean;
  public deletionMode: DeletionMode;

  constructor(opts: Partial<BehaviorSettings>) {
    this.createFormGroupFromOutside = opts?.createFormGroupFromOutside ?? false;
    this.saveExternalChanges = opts?.saveExternalChanges ?? false;
    this.excelSettings = opts?.excelSettings ?? new ExcelSettings(
      { enabled: false }
    );
    this.pdfSettings = opts?.pdfSettings ?? new PDFSettings(
      { enabled: false }
    );
    this.showDeletionConfirmation = false;
    this.deletionMode = opts?.deletionMode ?? DeletionMode.HandleEachRowSeparately;
  }
}

export class ExcelSettings {
  public enabled: boolean;

  constructor(options: Partial<ExcelSettings>) {
    this.enabled = options?.enabled ?? true;
  }
}

export class PDFSettings {
  public enabled: boolean;
  public allPages: boolean;

  constructor(options: Partial<PDFSettings>) {
    this.enabled = options?.enabled ?? true;
    this.allPages = options?.allPages ?? false;
  }
}

export class TemplateSettings {
  public createNewColumnWithTemplate: boolean;
}

export class ColumnMenuSettings {
  /**
   * Abilita la colonna del menu a livello globale.
   */
  public columnMenu: boolean;

  /**
   * Disabilitala colonna menu per colonne specifiche.
   * Questa funzionalità non ancora funziona.
   */
  public disabledColumns: number[];

  /**
   * Filtrable. Deve essere true se column menu è abilitato.
   * Set to true to add a new row allowing to sort the grid.
   * Set to 'menu' to add a dropdown list used for sorting.
   */

  public filterable: boolean | string;

  /**
   * Mostrare la Kendo Column Chooser. Fare comparire/scomparire alcune colonne.
   */
  public kendoGridColumnChooser: boolean;

}

export class Sortable {
  allowUnsort: boolean;
  mode: SortMode;
}
export enum SortMode {
  SINGLE = 'single',
  MULTIPLE = 'multiple'
}

export class RemoveMultipleRowsParams {
  data: KendoGridRow[];
}
