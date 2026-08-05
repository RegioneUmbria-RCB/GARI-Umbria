import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import {
    CommandsColumnSettings,
    PaginationSettings,
    PreselectedRowsSettings,
    SelectableSettings
} from 'gias-kendo-grid';
import { KendoServerResult, KendoGridRow, KendoGridColumn, KendoGridModel, EditingMode, LoaderType,  ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { map, Observable, of } from "rxjs";
import { OperationEditService } from "./operation-edit.service";

export class GridOpEditServerRsult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridOpEditSrvice extends AbstractGridConfigService<GridOpEditServerRsult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'id_agenda';
    gridId = 'gridOp';

    kendoModel = {
        Data: new ModelEntry(CELL_TYPES.DATE),
        Operazione_DES: new ModelEntry(CELL_TYPES.STRING),
        id_agenda: new ModelEntry(CELL_TYPES.STRING),
        Utilizzo: new ModelEntry(CELL_TYPES.NUMBER)

    };
    kendoColumns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'Data', title: this.translocoService.translate('Data')}, {editable: false}),
        new KendoGridColumn({ field: 'Operazione_DES', title: this.translocoService.translate('Operazione')}, {editable: false} ),
       // new KendoGridColumn({ field: 'Utilizzo', title: this.translocoService.translate('Utilizzo')}, {editable: false} ),
    ];
    kendoRows = [{Data: new Date(), Operazione_DES: 'prova'}];

    constructor(injector: Injector,
        private translocoService: TranslocoService,
        private datashare: OperationEditService
    ) {
        super(injector);
        this.customize();

        this.gridPublicService.changeDetected.GiasSubscribe(ch => {
            let c = ch;
        });
    }

    read(options?: any): Observable<GridOpEditServerRsult> {
        this.kendoRows = this.datashare.editForm.get('attivita').value;
        this.kendoRows.forEach(row => {
            row['Utilizzo'] = 0;
        });

        const grid = new GridOpEditServerRsult(this.kendoRows, this.kendoColumns, this.kendoModel);
        return of(grid);
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return of([]);
    }

    private customize() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
        });
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.columnSettings.width = 35;

        this.columnMenu.kendoGridColumnChooser = false;
        this.groups.groupable.enabled = false;
        this.views.enabled = false;

        this.pagination = this.handlePagination();
        this.resizable.autoFitColumns = false;
        this.resizable.isResizable = true;

        this.behavior.excelSettings.enabled=false;
        this.behavior.pdfSettings.enabled=false;

        this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = false;
        this.generalSettings.height = 'auto';
    }

    private handlePagination() {
        const result: PaginationSettings = new PaginationSettings();
        result.gridState = {
            sort: [],
            skip: 0,
            group: [],
            take: 10,
            filter: {
                logic: 'and',
                filters: [],
            },
        };
        result.pageable = {
            buttonCount: 4,
            info: true,
            type: 'input',
            pageSizes: [10, 25, 50, 100, {
                text: this.transloco.translate('Tutti'),
                value: "all",
            } as any as number]
        };
        result.navigable = true;
        return result;
    }

}
