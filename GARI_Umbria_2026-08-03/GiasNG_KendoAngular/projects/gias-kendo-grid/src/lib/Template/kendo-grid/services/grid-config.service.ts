import { Injectable, Injector, OnDestroy } from '@angular/core';
import { ColumnComponent } from '@progress/kendo-angular-grid';
import { from, Observable, Subject, Subscription, takeUntil } from 'rxjs';

import {
    AgrSelectableSettings,
    SortSettings,
    CommandsColumnSettings,
    GeneralSettings,
    ResizableSettings,
    ToolbarSettings,
    PaginationSettings,
    ColumnMenuSettings,
    BehaviorSettings,
    AggregateSettings,
    GroupSettings,
    DettagliColumnSettings,
    CustomColumnSettings,
    RemoveMultipleRowsParams,
    CommandsDropDownSettings,
    MasterDetailSettings
} from '../models/configuration.model';
import { GridErrorService as GridLogService } from './grid-log.service';
import {
    EditingMode,
    GridCustomizations as GridCustomizations,
    KendoGridColumn,
    KendoGridModel,
    KendoServerResult,
    LoaderType,
    OnBeforeAfterEvents as IOnBeforeAfterEvents,
    RendererGridEvent
} from '../models/grid.model';
import {
    ConfigTemplate,
    DefaultTemplate,
    IOptionalConfigParameters as IOptionalConfigParams,
    serviceMap
} from '../models/template.model';
import { CodiciTemplate } from '../../../shared/codici-config-template.model';
import { InCellTemplate } from '../models/in-cell-template.model';
import { InLineTemplate } from '../models/in-line-template.model';
import { GridPublicService } from './grid-public.service';
import { TranslocoService } from '@jsverse/transloco';
import { Enum_DBTypeOperation, LOADING_TOKEN, LoadingService, GIAS_PARAMETRI_AGENDA_TOKEN, IObjParametriAgendaService } from 'gias-ui-kit';


declare module 'rxjs' {
    interface Observable<T> {
        GiasSubscribe(next: (value: T) => void): Subscription;
    }
}

export interface IGridRequiredFields<Type> {
    read(options?: any): Observable<Type>;
    perform(action: HttpAction, items: any): Observable<any[]>;
    applyRendererRules(opts: RendererGridEvent): void;
    onRowClass(): any;

    editingMode: EditingMode;
    loader: LoaderType;
    rowId: string;
    gridId: string;
}

export interface IGridMessages {
    getRemoveMultipleRowsMessage(opts);
}

@Injectable({ providedIn: 'root' })
export abstract class AbstractGridConfigService<Type> implements IGridRequiredFields<Type>,
    IOptionalConfigParams,
    IOnBeforeAfterEvents,
    OnDestroy,
    IGridMessages {
    /** Emesso onDestory */
    signal: Subject<void> = new Subject();

    /** Parametri obbligatori. */
    abstract editingMode: EditingMode;
    abstract loader: LoaderType;
    abstract rowId: string;              // chiave delle righe
    abstract gridId: string;

    applyRendererRules(opts: RendererGridEvent) {
        let a = 0; //Commento per funzione vuota SonarQube
    };

    onRowClass = null;


    /** Parametri opzionali. */
    selectable: AgrSelectableSettings;
    toolbar: ToolbarSettings;
    cmdColumn: CommandsColumnSettings;
    cmdDropDown: CommandsDropDownSettings;
    dettagliColumn: DettagliColumnSettings;
    customColumn: CustomColumnSettings;
    resizable: ResizableSettings;
    generalSettings: GeneralSettings;
    sort: SortSettings;
    pagination: PaginationSettings;
    columnMenu: ColumnMenuSettings;
    behavior: BehaviorSettings;
    views: GridCustomizations;
    aggregates: AggregateSettings;
    groups: GroupSettings;
    masterdetailSettings: MasterDetailSettings;

    /** Funzione utilizzata per caricare le righe, le colonne e il modello della griglia. */
    abstract read(options?: any): Observable<Type>;

    /** Crea, modifica e cancella righe della griglia.
     * @param items - array contenete le righe su cui è stata eseguita l'operazione.
     * In caso si stia usando una griglia con modalità di editing `EditingMode.IN_CELL_BATCH`,
     * items sarà un oggetto rispettante l'interfaccia `InCellBatchSaveEventObject`.
     * @usageNotes
     * Se non volete eseguire alcuna operazione particolare è consigliato ritornare `null`
     * per non compromettere le normali funzionalità della griglia.
     * Eccetto che si tratti di una griglia con batch editing. In quel caso, ritornare `of([])`
     * se si desidera resettare l'oggetto `items` o `null` per lasciarlo così com'è.
     */
    abstract perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any>;

    /** Configuration wide settings. */
    protected logService: GridLogService;
    protected gridPublicService: GridPublicService;
    protected transloco: TranslocoService;
    protected loadingService: LoadingService;

    /** Private params */
    protected injector: Injector;

    constructor(injector: Injector, template: ConfigTemplate = ConfigTemplate.DefaultTemplate) {
        this.injector = injector;
        this.logService = injector.get(GridLogService);
        this.gridPublicService = injector.get(GridPublicService);
        this.transloco = injector.get(TranslocoService);
        this.loadingService = injector.get(LOADING_TOKEN);

        this.init(template);

    }
    async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
        if (opts.data.length > 1 || this.behavior.showDeletionConfirmation)
            return this.deletionMessage(opts.data);

        const DO_NOT_SHOW_MODAL = "";
        return DO_NOT_SHOW_MODAL;
    }

    private deletionMessage(rows: any[]): string {
        let prefix = this.getMessagePrefix(rows);
        if (rows.length > 1)
            return prefix + this.transloco.translate('giasgrid.MultipleDeletionConfirmation', [rows.length]);
        else
            return prefix + this.transloco.translate('giasgrid.SoleDeletionConfirmation');
    }

    getMessagePrefix(rows:any[]): string {
        let prefix = '';
        let alsoBrogliaccio = rows.map(row => row?.Origine && row?.Origine.toLowerCase()).some(orig => orig === 'demetra');
        if (alsoBrogliaccio) {
            prefix += this.transloco.translate('giasgrid.MenuAgendaActivityDeletionWBrogliaccio') + ' ';
        }
        return prefix;
    }

    ngOnDestroy(): void {
        this.signal.next();
        this.signal.complete();
    }

    protected model: KendoGridModel;
    protected columns: KendoGridColumn[];

    /** Used to define if the grid is editable or not.
     * Both the function `disableGrid` and `makeCellsUneditable` depend on the value of this field.
     */
    public gridIsEditable: boolean;
    /** Shows the action buttons from the command column and the creation button from the toolbar
     * based on the value of `gridIsEditable`.
     */
    disableGrid() {
        this.cmdColumn.removeBtn = this.gridIsEditable;
        this.cmdColumn.editBtn = this.gridIsEditable;
        this.toolbar.newItem = this.gridIsEditable;
    }
    /** Enables/disables every column based on the value of `gridIsEditable`. */
    makeCellsUneditable() {
        if (this.model == null || this.columns == null)
            throw Error("Please initialize the model and columns before calling this method.");

        Object.keys(this.model).forEach(key => this.model[key].editable = this.gridIsEditable);
        this.columns.forEach(s => s.editable = this.gridIsEditable);
    }


    /**
   * Funzioni da eseguire prima dopo vari eventi.
   */
    public onCellClose = null;
    public onCellClick = null;
    /** Usato per gestire la prevenzione della modifica di una cella. Di default, non previene l'evento.
     * Possibile riassegnare la lambda.
     */
    public preventEdit: ((dataItem: any, column: ColumnComponent) => boolean) = () => false;
    public onCreateExternalFormGroup = null;

    public sayHi() {
        this.logService.logInfo('Loaded grid configuration for ' + this['__proto__'].constructor.name);
        this.logService.logInfo('Loader type: ' + this.loader);
    }

    private init(tempType: ConfigTemplate) {
        let that = this;
        this.checkIfGridIsEditable();

        Observable.prototype.GiasSubscribe = function <T>(next: (value: T) => void): Subscription {
            const source = this as Observable<T>;
            return source.pipe(takeUntil(that.signal)).subscribe(next);
        }


        if (serviceMap.hasOwnProperty(tempType)) {
            const template = GetTemplate(tempType, this.injector);
            Object.keys(template).forEach(key => this[key] = template[key]);
        } else {
            this.logService.logErr('La variabile di configurazione non è configurata correttamente');
        }
    }
    private checkIfGridIsEditable() {
        let objParametriAgendaService = this.injector.get(GIAS_PARAMETRI_AGENDA_TOKEN) as IObjParametriAgendaService;
        let objParametriAgenda = objParametriAgendaService.getObjParamValue();
        this.gridIsEditable = true;
        if (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Read) {
            this.gridIsEditable = false;
        }
    }

    protected isLoading(value: boolean) {
        this.loadingService.set_isLoading({ isLoading: value, component: this.gridPublicService.gridElRef });
    }
}

function GetTemplate(template: ConfigTemplate, injector: Injector) {
    switch (template) {
        case ConfigTemplate.DefaultTemplate:
            return new DefaultTemplate(injector.get(TranslocoService));
        case ConfigTemplate.CodiciTemplate:
            return new CodiciTemplate(injector.get(TranslocoService));
        case ConfigTemplate.InCellTemplate:
            return new InCellTemplate(injector.get(TranslocoService));
        case ConfigTemplate.InLineTemplate:
            return new InLineTemplate(injector.get(TranslocoService));
    }
}

@Injectable({ providedIn: 'root' })
export class DefaultHttpService extends AbstractGridConfigService<KendoServerResult> {
    gridId = 'DefaultGrid';
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'chiave';

    constructor(injector: Injector) {
        super(injector);
    }

    read(): Observable<KendoServerResult> {
        this.logService.logInfo('Un servizio http della griglia non è stato selezionato');
        return from([]);
    }
    perform(actionType: HttpAction, items: any): Observable<any[]> {
        this.logService.logInfo('Un servizio http della griglia non è stato selezionato');
        return from([]);
    }

}

export enum HttpAction {
    CREATE = 'create',
    UPDATE = 'update',
    REMOVE = 'destroy',
    BATCH_SAVE = 'save',
}
