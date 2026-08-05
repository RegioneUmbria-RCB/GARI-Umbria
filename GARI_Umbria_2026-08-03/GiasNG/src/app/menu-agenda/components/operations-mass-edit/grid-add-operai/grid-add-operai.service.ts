import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { RowClassArgs } from "@progress/kendo-angular-grid";
import { ContattiService } from "app/Service/Anagrafica/contatti.service";
import { MasterService } from "app/Service/master.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { CommandsColumnSettings } from 'gias-kendo-grid';
import { KendoServerResult, KendoGridRow, KendoGridColumn, KendoGridModel, EditingMode, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, map, Observable, of } from "rxjs";
import { OperationEditService } from "../operation-edit.service";
import {Contatto} from '../../../../Model/anagrafiche/Contatto';

export class GridAddOperaiServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class MassEditOperaioRowModel {
    Piva: string;
    sa_cod: string;
    Cod_Contatto: string;
    Rag_Soc_Nome_Cognome: string;
    Cod_Rapporto: string;
    Rapporto_Des: string;
    Prezzo_Unitario: string;
    Unita_Misura: string;
    Mezzo: string;
    Cod_RisUm: string;
    Impresa: string;
    Dipendente: string;
    Terzista: string;
    Costo_Inizio: string;
    Costo_Fine: string;
    visibilita: string;
}

@Injectable()
export class GridAddOperaiService extends AbstractGridConfigService<GridAddOperaiServerResult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'id';
    gridId = 'gridAddOp';

    private kendoColumns = [
        new KendoGridColumn({field: 'Rag_Soc_Nome_Cognome', title: this.transloco.translate('RagioneSocialeNomeCognome')}),
        new KendoGridColumn({field: 'Rapporto_Des', title: this.transloco.translate('RapportoContabile')}),
        new KendoGridColumn({field: 'Prezzo_Unitario', title: this.transloco.translate('PrezzoUnitario')}),
        new KendoGridColumn({field: 'Unita_Misura', title: this.transloco.translate('Unita_Misura')}),
        new KendoGridColumn({field: 'Costo_Inizio', title: this.transloco.translate('ValiditàInizio')}),
        new KendoGridColumn({field: 'Costo_Fine', title: this.transloco.translate('ValiditàFine')}),
        new KendoGridColumn({field: 'Impresa', title: this.transloco.translate('Impresa')}),
    ];
    private kendoRows = [];
    private kendoModel = {
        Piva: new ModelEntry(CELL_TYPES.STRING),
        sa_cod: new ModelEntry(CELL_TYPES.STRING),
        Cod_Contatto: new ModelEntry(CELL_TYPES.STRING),
        Rag_Soc_Nome_Cognome: new ModelEntry(CELL_TYPES.STRING),
        Cod_Rapporto: new ModelEntry(CELL_TYPES.STRING),
        Rapporto_Des: new ModelEntry(CELL_TYPES.STRING),
        Prezzo_Unitario: new ModelEntry(CELL_TYPES.STRING),
        Unita_Misura: new ModelEntry(CELL_TYPES.STRING),
        Mezzo: new ModelEntry(CELL_TYPES.STRING),
        Cod_RisUm: new ModelEntry(CELL_TYPES.STRING),
        Impresa: new ModelEntry(CELL_TYPES.STRING),
        Dipendente: new ModelEntry(CELL_TYPES.STRING),
        Terzista: new ModelEntry(CELL_TYPES.STRING),
        Costo_Inizio: new ModelEntry(CELL_TYPES.STRING),
        Costo_Fine: new ModelEntry(CELL_TYPES.STRING),
        visibilita: new ModelEntry(CELL_TYPES.STRING),
    };

    constructor(injector: Injector,
        private translocoService: TranslocoService,
        private master: MasterService,
        private datashare: OperationEditService,
        private agenda: ObjParametriAgendaService,
        private contattiService: ContattiService,
        private opEditService: OperationEditService
    ) {
        super(injector);
        this.customize();
    }

    onRowClass = (event: RowClassArgs) => this.coloraRigheOperazioni(event.dataItem);

    read(options?: any): Observable<GridAddOperaiServerResult> {
        this.master.set_isLoading({isLoading: true});

        return from(this.opEditService.LeggiListaPersone().then(res => {
            this.kendoRows = res;
            this.datashare.editForm
                .get('manodopera')
                .get('contatti').patchValue(this.kendoRows);
            const grid = new GridAddOperaiServerResult(
                this.kendoRows, this.kendoColumns, this.kendoModel
            );
            this.master.set_isLoading({isLoading: false});
            return grid;
        }));
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
        this.behavior.excelSettings.enabled=false;
        this.behavior.pdfSettings.enabled=false;
        this.columnMenu.kendoGridColumnChooser = true;
        this.views.enabled = false;
        this.groups.groupable.enabled = false;
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.columnSettings.width = 35;
    }

    private manageColumns() {
        let visible = ['Contatto_Des', 'Rapporto_Des', 'Validita_Inizio', 'Validita_Fine', 'Impresa'];
        this.kendoColumns.forEach(c => c.hidden = !visible.includes(c.field));

        this.kendoColumns.push(
            new KendoGridColumn({ field:'prezzo', title:this.translocoService.translate('Prezzo')}),
            new KendoGridColumn({ field:'um', title:this.translocoService.translate('UnitaMisura')}),
            new KendoGridColumn({ field:'visibilita', title:this.translocoService.translate('Visibilità')},{hidden: true})
        );

        this.kendoRows.forEach(row => {
            switch(parseInt(row['sa_cod'], 10)) {
                case 0:
                    row['visibilita'] = this.translocoService.translate('Aziendale');
                    break;
                case -1:
                    row['visibilita'] = this.translocoService.translate('Pubblico');
                    break;
                default:
                    row['visibilita'] = this.translocoService.translate('Centro_Aziendale');
            }
        });
    }

    private coloraRigheOperazioni(row: KendoGridRow) {
        let result: {[k: string]: boolean} = {};

        switch(parseInt(row['sa_cod'], 10)) {
            case 0:
                result.visibilitaAziendale = true;
                break;
            case -1:
                result.visibilitaPubblica = true;
                break;
        }
        return result;
    }

}
