import {Injectable, Injector} from "@angular/core";
import {TranslocoService} from "@jsverse/transloco";
import {RowClassArgs} from "@progress/kendo-angular-grid";
import {CommandsColumnSettings} from 'gias-kendo-grid';
import {
    KendoServerResult,
    KendoGridRow,
    KendoGridColumn,
    KendoGridModel,
    EditingMode,
    LoaderType,
    ModelEntry,
} from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import {AbstractGridConfigService, HttpAction} from 'gias-kendo-grid';
import {map, Observable, of} from "rxjs";
import {OperationEditService} from "../operation-edit.service";
import {MasterService} from "app/Service/master.service";

export class GridAddMacchineServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

export class MassEditMacchinaRowModel {
    Piva: string;
    sa_cod: string;
    Mac_Cod: string;
    CLASS_DESC: string;
    Mac_Des: string;
    Modello: string;
    Targa: string;
    Prezzo_Unitario: string;
    Unita_Misura: string;
    Impresa: string;
    Costo_Inizio: string;
    Costo_Fine: string;
    visibilita: string;
}

@Injectable()
export class GridAddMacchineService extends AbstractGridConfigService<GridAddMacchineServerResult> {
    editingMode: EditingMode = EditingMode.IN_PAGE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId = 'id';
    gridId = 'gridAddMech';

    private kendoColumns = [
        new KendoGridColumn({field: 'CLASS_DESC', title: this.transloco.translate('Classe')}),
        new KendoGridColumn({field: 'Mac_Des', title: this.transloco.translate('MacchinaAttrezzatura')}),
        new KendoGridColumn({field: 'Modello', title: this.transloco.translate('Modello')}),
        new KendoGridColumn({field: 'Targa', title: this.transloco.translate('Targa')}),
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
        Mac_Cod: new ModelEntry(CELL_TYPES.STRING),
        CLASS_DESC: new ModelEntry(CELL_TYPES.STRING),
        Mac_Des: new ModelEntry(CELL_TYPES.STRING),
        Modello: new ModelEntry(CELL_TYPES.STRING),
        Targa: new ModelEntry(CELL_TYPES.STRING),
        Prezzo_Unitario: new ModelEntry(CELL_TYPES.STRING),
        Unita_Misura: new ModelEntry(CELL_TYPES.STRING),
        Impresa: new ModelEntry(CELL_TYPES.STRING),
        Costo_Inizio: new ModelEntry(CELL_TYPES.STRING),
        Costo_Fine: new ModelEntry(CELL_TYPES.STRING),
        visibilita: new ModelEntry(CELL_TYPES.STRING),
    };

    constructor(injector: Injector,
                private translocoService: TranslocoService,
                private master: MasterService,
                private datashare: OperationEditService,
                private opEditService: OperationEditService
    ) {
        super(injector);
        this.customize();
    }

    onRowClass = (event: RowClassArgs) => this.coloraRigheOperazioni(event.dataItem);

    read(options?: any): Observable<GridAddMacchineServerResult> {
        this.master.set_isLoading({isLoading: true});
        return this.opEditService.listaMacchineModificaMultipla()
            .pipe(map(macchine => {
                const soloAziendali = this.opEditService.editForm.get('macchine').get('mostraSoloAziendali').value;
                if (soloAziendali) {
                    this.kendoRows = macchine.filter(i => i['sa_cod'] == 0);
                } else {
                    this.kendoRows = macchine;
                }
                this.datashare.editForm.get('macchine').get('macchine').patchValue(macchine);
                this.master.set_isLoading({isLoading: false});
                return new GridAddMacchineServerResult(this.kendoRows, this.kendoColumns, this.kendoModel);
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
        this.behavior.excelSettings.enabled = false;
        this.behavior.pdfSettings.enabled = false;
        this.columnMenu.kendoGridColumnChooser = true;
        this.views.enabled = false;
        this.groups.groupable.enabled = false;
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.columnSettings.width = 35;
    }

    private manageColumns() {
        let visible = ['CLASS_DESC', 'Mac_Des', 'Modello', 'Targa', 'Validita_Inizio', 'Validita_Fine', 'Impresa'];
        this.kendoColumns.forEach(c => c.hidden = !visible.includes(c.field));

        this.kendoColumns.push(
            new KendoGridColumn({field: 'prezzo', title: this.translocoService.translate('Prezzo')}),
            new KendoGridColumn({field: 'um', title: this.translocoService.translate('UnitaMisura')}),
            new KendoGridColumn({field: 'visibilita', title: this.translocoService.translate('Visibilità')}, {hidden: true})
        );

        this.kendoRows.forEach(row => {
            let saCod = parseInt(row['sa_cod'], 10);
            if (saCod === 0) {
                row['visibilita'] = this.translocoService.translate('Aziendale');
            } else if (saCod === -1) {
                row['visibilita'] = this.translocoService.translate('Pubblico');
            } else {
                row['visibilita'] = this.translocoService.translate('Centro_Aziendale');
            }
        });
    }

    private coloraRigheOperazioni(row: KendoGridRow) {
        let result: { [k: string]: boolean } = {};
        let saCod = parseInt(row['sa_cod'], 10);
        if (saCod === 0) {
            result.visibilitaAziendale = true;
        } else if (saCod === -1) {
            result.visibilitaPubblica = true;
        }
        return result;
    }

    // private async loadRows() {
    //     // const RB = await this.impreseService.leggiImprese().toPromise();
    //     const RM = await this.datashare.LeggiListaMacchine();
    //     // const RM = await this.macchineService.leggiMacchine(this.agenda.getObjParamValue())
    //     //                     .then(result => JSON.parse(result.RispostaStringa));

    //     this.kendoColumns = RM.kendo_columns;
    //     this.kendoRows = RM.kendo_rows;
    //     console.log(this.kendoRows);
    //     RM.kendo_model['Sa_Nome'] = { editable: false, type: 'string' };
    //     RM.kendo_model['Visibilita'] = { editable: false, type: 'string' };

    //     this.kendoColumns.forEach(column => column.hidden = true);
    //     this.kendoColumns.push(
    //         new KendoGridColumn({ field:'Sa_Nome', title:this.translocoService.translate('NomeCentro')}),
    //         new KendoGridColumn({ field:'Visibilita', title:this.translocoService.translate('Visibilità')},{hidden: true})
    //     );

    //     for (let title of ['tipologia','Macchina', 'Modello', 'Targa',
    //         'PREZZO UNITARIO', 'UNITA MISURA', 'Validita Inizio',
    //         'Validita Fine','chiave']) {

    //         let column = this.kendoColumns.find(col => col.title === title);
    //         if (!column) continue;

    //         column.hidden = false;
    //     }

    //     // REVIEW: dove trovare rag_soc associato alla macchina?
    //     this.kendoRows.forEach(row => {
    //         let objAgenda = this.agenda.getObjParamValue();
    //         row['Sa_Nome'] = (row['Piva'] === objAgenda.Piva) ?
    //                           objAgenda.RagSoc : '';

    //         if (row['Sa_Cod'] === -1) {
    //             row['Visibilita'] = this.transloco.translate('qdc.MacchinaPubblica');
    //         } else if (row['Sa_Cod'] === 0) {
    //             row['Visibilita'] = this.transloco.translate('qdc.MacchinaAziendale');
    //         } else {
    //             row['Visibilita'] = this.transloco.translate('qdc.Centro_Aziendale');
    //         }
    //     });

    //     this.kendoRows = this.kendoRows.filter(row => {
    //         let dateParams = row.Validita_Fine.substring(0,10).split('/');
    //         let validitaFine = new Date(
    //             dateParams[1] + '/' + dateParams[0] + '/' + dateParams[2]
    //         );

    //         return validitaFine > new Date();
    //     });

    //     return RM.kendo_model;
    // }

    // private handleRefresh() {
    //     this.datashare.editForm.get("macchine").get("mostraSoloAziendali")
    //         .valueChanges.GiasSubscribe(opts => {
    //             console.log(opts);
    //             this.gridPublicService.refresh(true);
    //         })
    // }

}
