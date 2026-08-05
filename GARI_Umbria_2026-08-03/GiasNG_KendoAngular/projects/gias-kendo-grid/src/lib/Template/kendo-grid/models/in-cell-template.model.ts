import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import {
    AggregateSettings,
    AgrSelectableSettings,
    BehaviorSettings,
    ColumnMenuSettings,
    CommandsColumnSettings,
    CommandsDropDownSettings,
    DettagliColumnSettings,
    ExcelSettings,
    GeneralSettings,
    GroupSettings,
    MasterDetailSettings,
    CustomColumnSettings,
    PaginationSettings,
    PDFSettings,
    PreselectedRowsSettings,
    ResizableSettings,
    SelectableColumnSettings,
    SortMode,
    SortSettings,
    ToolbarSettings
} from './configuration.model';
import { GridCustomizations } from './grid.model';
import { IOptionalConfigParameters } from './template.model';


@Injectable({ providedIn: 'root' })
export class InCellTemplate implements IOptionalConfigParameters {
    selectable: AgrSelectableSettings;
    toolbar: ToolbarSettings;
    cmdColumn: CommandsColumnSettings;
    cmdDropDown: CommandsDropDownSettings;
    resizable: ResizableSettings;
    generalSettings: GeneralSettings;
    sort: SortSettings;
    pagination: PaginationSettings;
    columnMenu: ColumnMenuSettings;
    behavior: BehaviorSettings;
    views: GridCustomizations;
    aggregates: AggregateSettings;
    groups: GroupSettings;
    dettagliColumn: DettagliColumnSettings;
    customColumn: CustomColumnSettings;
    masterdetail: MasterDetailSettings;

    constructor(private translocoService: TranslocoService) {
        this.selectable = this.handleSelectable();
        this.toolbar = this.handleToolbarSettings();
        this.cmdColumn = this.handleCmdColumn();
        this.cmdDropDown = this.handleCmdDropDown();
        this.resizable = this.handleResizable();
        this.generalSettings = this.handleGeneralSettings();
        this.sort = this.handleSort();
        this.pagination = this.handlePagination();

        this.columnMenu = this.handleColumnMenu();

        this.behavior = new BehaviorSettings({});
        this.behavior.createFormGroupFromOutside = false;
        this.behavior.excelSettings = new ExcelSettings({ enabled: true });
        this.behavior.pdfSettings = new PDFSettings({ enabled: false });

        this.aggregates = new AggregateSettings({ enabled: false });
        this.views = new GridCustomizations({ enabled: false });

        this.groups = new GroupSettings({}, this.translocoService);
        this.dettagliColumn = new DettagliColumnSettings({ sticky: true, title: this.translocoService.translate('giasgrid.Dettagli'), width: 100 });
        this.customColumn = new CustomColumnSettings({ sticky: true, title: '', width: 100 });
        this.masterdetail = new MasterDetailSettings(false);
    }



    handleToolbarSettings(): ToolbarSettings {
        const result = new ToolbarSettings();
        result.newItem = false;
        result.resetChanges = false;
        result.title = 'Azioni';
        result.width = 150;

        return result;
    }

    handleGeneralSettings(): GeneralSettings {
        const result = new GeneralSettings();
        result.height = 600;

        return result;
    }

    handleCmdColumn(): CommandsColumnSettings {
        const result = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false
        });

        return result;
    }

    handleCmdDropDown(): CommandsDropDownSettings {
        const result = new CommandsDropDownSettings();
        result.inlineEditBtn = true;
        result.infoBtn = true;
        result.removeBtn = true;
        result.title = 'Comandi';
        return result;
    }

    handleResizable() {
        const result = new ResizableSettings();
        result.isResizable = true;
        result.autoFitColumns = false;

        return result;
    }

    handleSelectable(): AgrSelectableSettings {
        const result = new AgrSelectableSettings();
        result.selectable.enabled = false;
        result.columnSettings = new SelectableColumnSettings();
        result.columnSettings.showSelectAll = false;
        result.columnSettings.title = 'Checkbox';
        result.columnSettings.width = 30;

        result.preselectedRows = new PreselectedRowsSettings();

        return result;
    }
    handleColumnMenu(): ColumnMenuSettings {
        const columnMenu = new ColumnMenuSettings();
        columnMenu.columnMenu = true;
        columnMenu.filterable = true;
        columnMenu.disabledColumns = [];
        columnMenu.kendoGridColumnChooser = true;

        return columnMenu;
    }

    handleSort(): SortSettings {
        const result = new SortSettings();
        result.sortable = { allowUnsort: true, mode: SortMode.SINGLE };
        result.sort = [];

        return result;
    }

    handlePagination(): PaginationSettings {
        const result = new PaginationSettings();
        result.gridState = {
            sort: [],
            skip: 0,
            take: 5,
            filter: {
                logic: 'and',
                filters: [],
            },
        };
        result.pageable = true;
        result.navigable = false;

        return result;
    }
}
