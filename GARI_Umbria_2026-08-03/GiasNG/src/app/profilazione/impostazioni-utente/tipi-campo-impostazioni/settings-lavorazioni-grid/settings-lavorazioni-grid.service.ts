import { Injectable, Injector } from "@angular/core";
import { CommandsColumnSettings } from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoServerResult, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable, of } from "rxjs";
import { TranslocoService } from "@jsverse/transloco";
import {ImpostazioniFormService} from "../../../services/impostazioni/impostazioni-form.service";



export class SettigsLavorazioniServerResult extends KendoServerResult{
    constructor(model, columns, rows) {
        super(model, columns, rows);
    }
}

@Injectable({ providedIn: 'root' })
export class ImpostazioniLavorazioniGridConfigService extends AbstractGridConfigService<SettigsLavorazioniServerResult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'rowId';
    gridId = 'gridId';

    kendoRows = [];
    kendoColumns: KendoGridColumn[] = [
        new KendoGridColumn({field: 'descrizione', title: this.translocoService.translate('Operazione')}, {editable: false}),
        // new KendoGridColumn({field: 'specie', title: this.translocoService.translate('Specie')}, { editable: false}),
        // new KendoGridColumn({field: 'macchine', title: this.translocoService.translate('MacchineAttrezzature')}, { editable: false}),
        // new KendoGridColumn({field: 'contatti', title: this.translocoService.translate('Contatti')}, { editable: false}),
        // new KendoGridColumn({field: 'h_ha', title:this.translocoService.translate('OreXEttaro')})
    ];
    kendoModel: KendoGridModel = {
        descrizione: new ModelEntry(CELL_TYPES.STRING),
        codice: new ModelEntry(CELL_TYPES.STRING),
        gr_des: new ModelEntry(CELL_TYPES.STRING),
        gr_cod: new ModelEntry(CELL_TYPES.STRING),
    };

    constructor(injector: Injector,
                private settingsService: ImpostazioniFormService,
                private translocoService: TranslocoService) {
        super(injector);
        this.handleCustomization() ;
        this.handleEvents();
    }

    read(options?: any): Observable<SettigsLavorazioniServerResult> {
        const grid = new SettigsLavorazioniServerResult(
            this.kendoModel, this.kendoColumns, this.kendoRows
        );
        return of(grid);
    }
    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return from([]);
    }

    private handleCustomization() {
        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: true,
            width: 30
        });
        this.behavior.excelSettings.enabled = false;
        this.behavior.pdfSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = false;
        this.views.enabled = false;
        this.groups.groupable = { emptyText: "", showFooter: false, enabled: false };
    }

    private handleEvents() {
        this.settingsService.addOperationsInFilter$.pipe().subscribe(ops => {
            console.log(ops);
            ops.map(o => ({
                codice: o.primaryKey.codice,
                descrizione: o.descrizione,
                gr_des: o.categoriaOperazione.descrizione,
                gr_cod: o.categoriaOperazione.codice
            })).forEach(o => this.kendoRows.push(o));
            this.gridPublicService.refresh(true, new SettigsLavorazioniServerResult(
                this.kendoModel, this.kendoColumns, this.kendoRows
            ));
        });
    }

}
