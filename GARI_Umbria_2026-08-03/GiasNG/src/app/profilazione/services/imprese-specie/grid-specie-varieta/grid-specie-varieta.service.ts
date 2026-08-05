import { Injectable, Injector, OnInit } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { BaseCodeDescr } from "app/Model/baseClass/baseCodeDescr";
import { VarietaService } from "app/Service/Metaschema/varieta.service";
import { CommandsColumnSettings } from 'gias-kendo-grid';
import { KendoServerResult, EditingMode, LoaderType,  KendoGridColumn, ModelEntry, DropdownListWithForm, DropdownListItem, KendoGridRow, GridCustomizations } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of } from "rxjs";
import { ProfilazioneDataShareService } from "../../profilazione-data-share.service";


export class GridSpecieVarietaServerResult extends KendoServerResult{
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

@Injectable({ providedIn: 'root' })
export class GridSpecieVarietaConfigService extends AbstractGridConfigService<GridSpecieVarietaServerResult> {

    editingMode: EditingMode = EditingMode.IN_CELL;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'Specie_Cod';
    gridId = 'SpecieVarieta';

    kendoRows = [];
    kendoColumns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'Specie_Des', title: this.translocoService.translate('prof.SpecieVegetale')}, {editable: false}),
        new KendoGridColumn({ field: 'Varieta_Cod', title: this.translocoService.translate('prof.Varietà')})
    ];
    kendoModel = {
        Specie_Cod: new ModelEntry(CELL_TYPES.STRING),
        Specie_Des: new ModelEntry(CELL_TYPES.STRING),
        Varieta_Cod: new ModelEntry(CELL_TYPES.MULTI_DROPDOWNLIST),
        Varieta_Des: new ModelEntry(CELL_TYPES.STRING),
    };

    constructor(injector: Injector,
        private translocoService: TranslocoService,
        private httpService: ProfilazioneDataShareService,
        private varietaService: VarietaService,
    ) {
        super(injector);
        this.handleCustomization();
        this.handleEvents();
    }

    read(options?: any): Observable<GridSpecieVarietaServerResult> {
       this.configureDDL();

        const grid = new GridSpecieVarietaServerResult(
            this.kendoModel, this.kendoColumns, this.kendoRows
        );

        return of(grid);
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        switch (actionType) {
            case HttpAction.CREATE:
                break;
            case HttpAction.REMOVE:
                break;
            case HttpAction.UPDATE:
                break;
        }
        return from([]);
    }

    private handleCustomization() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
        });
        this.behavior.saveExternalChanges = false;
        this.columnMenu.filterable = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.groups.groupable.enabled = false;
        this.views.enabled = false;

        this.pagination.pageable = false;
        this.resizable.isResizable = true;

        this.behavior.excelSettings.enabled=false;
        this.behavior.pdfSettings.enabled=false;

        this.generalSettings.reordable = false;
        this.generalSettings.performOnEdit = false;
        this.generalSettings.height = 'auto';
    }

    private configureDDL() {
        let col = this.kendoColumns.find(s => s.field === 'Varieta_Cod');
        let data: DropdownListItem[] = [];

        col.ddl = new DropdownListWithForm('codice', 'Varieta_Cod', 'descrizione', data);
        col.ddl.valuePrimitive = true;
        col.ddl.descriptionField = 'Varieta_Des';

        col.ddl.loadOnEdit = true;
        col.ddl.loadFunction = this.loadCultivar.bind(this);
    }

    private loadCultivar(row): Observable<any> {
        return this.varietaService.leggiAsObs(new BaseCodeDescr( row.Specie_Cod,row.Specie_Des));
    }

    private handleEvents() {
        this.handleSpecieSelection();
        this.handleCultivarSelection();
    }

    private handleSpecieSelection() {
        this.httpService.specie.GiasSubscribe(data => {
            this.deleteRows(data);
            this.addNewRows(data);
            this.refresh();
        });
    }

    private handleCultivarSelection() {
        this.gridPublicService.changeDetected.GiasSubscribe( ch => {
            this.handleAllCultivars(ch.dataItem);
        });
    }

    private handleAllCultivars(row) {
        let cul_cods = row.Varieta_Cod;
        let cul_des = row.Varieta_Des;

        if (cul_cods.length && !cul_cods[0]) {
            cul_cods.shift();
        }
        if (!cul_cods.length) {
            cul_cods.push(0);
            cul_des.push(this.translocoService.translate('prof.TutteVarieta'));
        }
    }

    private addNewRows(data: BaseCodeDescr[]) {
        let toAdd = data.filter(specie =>
            !this.kendoRows.map(row => row.Specie_Cod)
                           .includes(specie.codice)
        );

        toAdd.forEach( row => this.kendoRows.push({
            Specie_Cod: row.codice,
            Specie_Des: row.descrizione,
            Varieta_Cod: [0],
            Varieta_Des: [this.translocoService.translate('prof.TutteVarieta')]
        }));
    }

    private deleteRows(data: BaseCodeDescr[]) {
        let toDel = this.kendoRows.findIndex(row =>
            !data.map(specie => specie.codice).includes(row.Specie_Cod)
        );

        if (toDel > -1) {
            this.kendoRows.splice(toDel, 1);
            this.deleteRows(data);
        }
    }

    private refresh() {
        const grid = new GridSpecieVarietaServerResult(
            this.kendoModel, this.kendoColumns, this.kendoRows
        );
        this.gridPublicService.refresh(true, grid);
    }

}
