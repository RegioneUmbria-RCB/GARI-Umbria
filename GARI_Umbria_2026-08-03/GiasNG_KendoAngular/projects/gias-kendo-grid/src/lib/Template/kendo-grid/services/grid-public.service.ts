
import { ElementRef, Inject, Injectable, OnDestroy, Optional } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { EditEvent, GridComponent } from '@progress/kendo-angular-grid';
import { BehaviorSubject, Subject } from 'rxjs';
import { skip, takeUntil } from 'rxjs/operators';
import { KendoGridRow, KendoServerResult as GridServerResult, SelectionSettings as SelectionHandler } from '../models/grid.model';
import { KendoGridService } from './kendo-grid.service';
import { IQueryParamsService, QryParamsResolver } from './utilities';
import { CompositeFilterDescriptor } from '@progress/kendo-data-query/dist/npm/filtering/filter-descriptor.interface';
import { cloneDeep } from "lodash";
import { InMemoryView } from '../components/grid-customizations/model';
import { GridCommandItem } from '../../../shared/utils';
import { GridDataWithFilter } from './grid-data-with-filter';


@Injectable()
export class GridPublicService
    extends BehaviorSubject<GridDataWithFilter>
    implements OnDestroy {

    /* Section 1 */

    clearGridPublicService() {
        this.formGroup = new BehaviorSubject(null);
        this.multiIndex = null;
        this.gridPrivate = null;
        this.changeDetected = new Subject();
        this.detachedSaveBtn = new Subject<void>();
        this.multiIndex = null;
        this.freeMultiIndex = true;
        this.selection.setSelected = new BehaviorSubject({ keys: [] });
        this.gridElRef = null;
        this.signal$ = null;
    }

    /*
    *@description:
    * Utilizzato nelle grid di tipo Master Detail indica il dataItem della Riga Master che ha generato la griglia di Detail
    * (cambia per ogni griglia di detail)
    * */
    private _Current_Grid_Master_RowExpanded: any;

    public get Current_Grid_Master_RowExpanded() {
        return cloneDeep(this._Current_Grid_Master_RowExpanded);
    }
    public set Current_Grid_Master_RowExpanded(RowExpanded: any) {
        this._Current_Grid_Master_RowExpanded = RowExpanded;
    }

    /// Il FormGroup della riga che viene modificata.
    /// Permette di aggiungere validatori e fare verifiche.
    public formGroup: BehaviorSubject<FormGroup> = new BehaviorSubject(null);
    public signal$: Subject<void>;

    /** A new value is emitted when pressing a row's commands' dropdown button.
     * @valueEmitted the row's dataItem
     */
    public openCommands: BehaviorSubject<any> = new BehaviorSubject(null);
    /** A new value is emitted when pressing a command from the commands'
     * dropdown button.
     * @valueEmitted object containing `command`, `dataItem` and `rowIndex`
     * @usageNotes to select the right command use a `switch` over command.action
     * the base events can be found in the enumeration `CommandsDropDownEvents`.
     * Other events should be declared as constant numbers.
     *
     * @example
     * ```
     * this.gridPublicSerivce.commandEvent.GiasSubscribe(ev => {
            if (!ev) return;
            switch (ev.command.action) {
                case CommandsDropDownEvents.FULL_EDIT:
                    this.onTemplateBtnClick(ev.dataItem);
                    break;
            }
        })
     * ```
     */
    public commandEvent: BehaviorSubject<{
        command: GridCommandItem,
        dataItem: any,
        rowIndex: number
    }> = new BehaviorSubject(null);


    // ! End public API
    public multiIndex: number;
    // ! Private API.

    private gridPrivate: KendoGridService;

    // ! End private API

    public gridElRef: ElementRef<any>;

    public gridComp: GridComponent;
    public giasGridComponent: any;

    public changeDetected: Subject<EditEvent> = new Subject();
    public detachedSaveBtn = new Subject<void>();
    public resetChanges$ = new Subject<void>();

    public freeMultiIndex = true;

    /** Setter and getter of newly selected values */
    public selection: SelectionHandler = new SelectionHandler();

    public currentDataItem: KendoGridRow;

    /** Contiene gli ultimi filtri applicati alle colonne della griglia.
     * @UsageNotes Possibile ricavare le righe filtrate tramite: <code>process(this.rows, { filter: this.grid.filter }).data;</code>
     */
    public filters: BehaviorSubject<CompositeFilterDescriptor> = new BehaviorSubject<CompositeFilterDescriptor>(null);

    public thereIsAForm: boolean = false;

    public applyViewFiltroJSON: BehaviorSubject<InMemoryView> = new BehaviorSubject<InMemoryView>(null);

    public afterSaveView: BehaviorSubject<string> = new BehaviorSubject<string>(null);

    public keyViewString: BehaviorSubject<string> = new BehaviorSubject<string>(null);

    public filtroJSONstring: string = '';

    constructor(
        @Optional()
        @Inject(QryParamsResolver) private qryParamServices: IQueryParamsService[]
    ) {
        super(null);
        this.detachedSaveBtn = new Subject();
    }

    ngOnDestroy(): void {
        this.formGroup.next(null);
        this.resetChanges$.complete();
    }

    changeFormValue(type: FormValueChange, controlName: string, value: any) {
        switch (type) {
            case FormValueChange.DropdownItem:
                this.gridPrivate.nextDropdownValue(controlName, value, this.formGroup.value);
                break;
            default:
                break;
        }
    }

    /** Disables the editing of the grid.
     * @usageNotes
     * To be able to use this function, both the field `model` and `columns` of the {@link AbstractGridConfigService}
     * must be valorized with the data used in the grid. You can valorize the fields from the `read()` function in
     * your grid's configuration service.
     */
    disable() {
        this.gridPrivate.disable();
    }
    /** Enables the editing of the grid.
     * @usageNotes
     * To be able to use this function, both the field `model` and `columns` of the {@link AbstractGridConfigService}
     * must be valorized with the data used in the grid. You can valorize the fields from the `read()` function in
     * your grid's configuration service.
     */
    enable() {
        this.gridPrivate.enable();
    }

    refresh(forceRefresh = false, newData: GridServerResult = null, afterEdit = false) {
        if (newData)
            this.next({ data: newData, stopPropagation: false, forceRefresh: forceRefresh, afterEdit: afterEdit });
        else
            this.next({ data: this.value?.data, stopPropagation: false, forceRefresh: forceRefresh, afterEdit: afterEdit });
    }

    init(ctx: KendoGridService) {
        this.gridPrivate = ctx;
        this.signal$ = ctx.signal;
        this.gridComp = ctx._gridCtx.grid;

        this.qryParamServices?.forEach((service) => {
            service.init(this.signal$);
        })
    }

    /** Executed after the data has been placed inside the grid. */
    dataBinded: boolean = false;
    public parseQueryParams() {
        if (this.qryParamServices && !this.dataBinded) {
            this.dataBinded = true;
            this.qryParamServices?.forEach((service) => {
                service.execute(this);
            });

        }
    }

    /**
     * Called inside the ngOnInit() life cycle hook of the grid.
     * @param ctx Grid context. Used for unsubscribing and updating grid
     * properties.
     */
    public ngOnInit(ctx: any) {
        this.initializeSubscriptions(ctx);
    }

    private initializeSubscriptions(ctx: any) {
        this.selection.setSelected.pipe(skip(1), takeUntil(this.signal$))
            .subscribe(data => {
                ctx.setSelectedRows({
                    keys: data.keys,
                    usePartialMatch: data.usePartialMatch,
                    resetPreviousSelection: data.resetPreviousSelection
                });
            });
    }

}


export enum FormValueChange {
    DropdownItem = 1,
}
