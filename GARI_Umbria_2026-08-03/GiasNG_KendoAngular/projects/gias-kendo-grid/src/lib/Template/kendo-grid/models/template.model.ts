import { Injectable } from '@angular/core';
import { TranslocoService } from '@jsverse/transloco';
import { CodiciTemplate } from '../../../shared/codici-config-template.model';
import {
    AggregateSettings,
    AgrSelectableSettings,
    BehaviorSettings,
    ColumnMenuSettings,
    CommandsColumnSettings,
    CommandsDropDownSettings,
    CustomColumnSettings,
    DettagliColumnSettings,
    ExcelSettings,
    GeneralSettings,
    GroupSettings,
    MasterDetailSettings,
    PaginationSettings,
    PDFSettings,
    PreselectedRowsSettings,
    ResizableSettings,
    SelectableColumnSettings,
    SelectableSettings,
    SortMode,
    SortSettings,
    ToolbarSettings
} from './configuration.model';
import { GridCustomizations } from './grid.model';
import { InCellTemplate } from './in-cell-template.model';
import { InLineTemplate } from './in-line-template.model';

export interface IOptionalConfigParameters {
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
}

@Injectable()
export class DefaultTemplate implements IOptionalConfigParameters {
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

        this.behavior = new BehaviorSettings(
            {
                createFormGroupFromOutside: false,
                excelSettings: new ExcelSettings({ enabled: true }),
                pdfSettings: new PDFSettings({ enabled: false })
            });
        this.behavior.createFormGroupFromOutside = false;
        this.behavior.excelSettings = new ExcelSettings({ enabled: true });
        this.behavior.pdfSettings = new PDFSettings({ enabled: false });

        const grpMsg = this.translocoService.translate('giasgrid.grpEmptyMsg');
        this.groups = new GroupSettings({
            groupable: { enabled: true, showFooter: true, emptyText: grpMsg }
        }, this.translocoService);

        this.aggregates = new AggregateSettings({ enabled: false, descriptors: [] });

        this.views = new GridCustomizations({ enabled: true });
        this.dettagliColumn = new DettagliColumnSettings({ sticky: true, title: this.translocoService.translate('giasgrid.Dettagli'), width: 150 });
        this.customColumn = new CustomColumnSettings({ sticky: true, title: '', width: 150 });
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
        result.height = 650;
        result.reordable = true;

        return result;
    }

    handleCmdColumn(): CommandsColumnSettings {
        const result = new CommandsColumnSettings();
        result.editBtn = true;
        result.infoBtn = true;
        result.removeBtn = true;
        result.title = 'Comandi';
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
        result.selectable = new SelectableSettings({});
        result.columnSettings = new SelectableColumnSettings();
        result.columnSettings.showSelectAll = false;
        result.columnSettings.title = 'Checkbox';
        result.columnSettings.width = 40;

        result.preselectedRows = new PreselectedRowsSettings();

        return result;
    }

    handleColumnMenu(): ColumnMenuSettings {
        const columnMenu = new ColumnMenuSettings();
        columnMenu.columnMenu = true;
        columnMenu.filterable = FilterMenuType.MENU;
        columnMenu.disabledColumns = [];
        columnMenu.kendoGridColumnChooser = true;

        return columnMenu;
    }

    handleSort(): SortSettings {
        const result = new SortSettings();
        result.sortable = { allowUnsort: true, mode: SortMode.MULTIPLE };
        result.sort = [];

        return result;
    }

    handlePagination(): PaginationSettings {
        const result = new PaginationSettings();
        result.gridState = {
            sort: [],
            skip: 0,
            group: [],
            take: 5,
            filter: {
                logic: 'and',
                filters: [],
            },
        };
        result.pageable = {
            buttonCount: 4,
            info: true,
            type: 'input',
            pageSizes: this.resizable.autoFitColumns ?
                [5, 7, 10, 25, 50] :
                [5, 7, 10, 25, 50, 100, {
                    text: this.translocoService.translate('giasgrid.Tutti'),
                    value: "all",
                } as any as number]
        };
        result.navigable = false;

        return result;
    }


}

export class FilterMenuType {
    static readonly ROW = true;
    static readonly MENU = 'menu';
}


/**
 * Aggiungendo un nuovo template bisgona soltanto aggiornare le due variabili qui sotto.
 */
export const serviceMap = {
    DefaultTemplate: DefaultTemplate,
    CodiciTemplate: CodiciTemplate,
    InLineTemplate: InLineTemplate,
    InCellTemplate: InCellTemplate
};

export enum ConfigTemplate {
    DefaultTemplate = 'DefaultTemplate',
    CodiciTemplate = 'CodiciTemplate',
    InLineTemplate = 'InLineTemplate',
    InCellTemplate = 'InCellTemplate'
}
