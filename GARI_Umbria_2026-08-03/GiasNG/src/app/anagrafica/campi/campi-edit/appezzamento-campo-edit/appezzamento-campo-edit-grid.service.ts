import { Inject, Injectable, Injector } from '@angular/core';
import { AgrSelectableSettings, CommandsColumnSettings, ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { forkJoin, from, Observable } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { map } from 'rxjs/operators';
import { Validators } from '@angular/forms';
import { AppezzamentoCampoId, AppezzamentoCampoService } from './appezzamento-campo.service';
import { AppezzamentiCampoKendoServerResult, KendoAppezzamentiCampoModel } from './appezzamenti-campo-edit.model';
import { IntlService } from '@progress/kendo-angular-intl';
import { SelectionEvent } from '@progress/kendo-angular-grid';
import { GiasKendoGridComponent } from 'gias-kendo-grid';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { CampiFactoryService, CAMPI_SERVICE_TOKEN } from 'app/Service/ServiceFactory/campi.factory.service';
import { TranslocoService } from '@jsverse/transloco';
import { IntervalloTemporale } from 'app/Model/anagrafiche/IntervalloTemporale';

export class AppezzamentiCampoKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class AppezzamentoCampoEditService extends AbstractGridConfigService<AppezzamentiCampoKendo> {
    gridId = 'AppezzamentiCampo';
    rowId = 'chiave';

    objParametriAgenda: ObjParametriAgenda;
    toolbar = new ToolbarSettings(false, false);

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_CELL;

    KendoAppezzamentiCampo: any;

    appezzamentiRows: any[];
    appezzamentiColumns: KendoGridColumn[];
    appezzamentiModel: KendoAppezzamentiCampoModel[];

    constructor(
        injector: Injector,
        private intlService: IntlService,
        @Inject(CAMPI_SERVICE_TOKEN) private campiService: CampiFactoryService,
        private appezzamentoCampoService: AppezzamentoCampoService,
        private objParametriAgendaService: ObjParametriAgendaService,
        protected transloco: TranslocoService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        // Nascondo la colonna Azioni con i bottoni di Info,Modifica e Cancella
        this.cmdColumn.editBtn = false;
        this.cmdColumn.infoBtn = false;
        this.cmdColumn.removeBtn = false;

        // this.resizable.autoFitColumns = true;
        this.resizable.isResizable = true;

        // Setting generali
        this.columnMenu.columnMenu = false;
        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        this.generalSettings.reordable = true;
        this.generalSettings.performOnEdit = true;
        this.groups.groupable.enabled = false;

        //* ************************
        this.handleCustomizations();
        this.views.enabled = false;

        this.selectable.preselectedRows.selectionChangeFn = this.selectionChange;

        this.objParametriAgendaService.currentObjParametriAgenda.GiasSubscribe(p => {
            this.gridPublicService.refresh(true);
        })
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        const appCampo = this.appezzamentoCampoService.getAppezzamentoCampo();

        switch (actionType) {
            case HttpAction.CREATE:
                break;
            case HttpAction.REMOVE:
                break;
            case HttpAction.UPDATE:
                break;
        }

        this.appezzamentoCampoService.setAppezzamentoCampo(appCampo);

        return from([]);
    }

    read(): Observable<AppezzamentiCampoKendo> {

        const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        const appezzamenti: Promise<any> = this.campiService.leggiAppezzamentiCampo(agenda);

        this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

        return forkJoin(appezzamenti).pipe(map(result => {
            this.KendoAppezzamentiCampo = result[0].RispostaStringa;
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
            const righe = <Array<any>>(this.KendoAppezzamentiCampo.kendo_rows);
            const model = this.setKendoModel();
            const cols = this.setKendoColumns();

            const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

            const columns = new Array<KendoGridColumn>();
            cols.forEach(elem => {

                const column: KendoGridColumn = new KendoGridColumn({ field: elem.field, title: elem.title},{
                    width: 200
                });

                column.editable = false;

                columns.push(column);

            });

            this.setKendoRows(righe);
            this.isRowSelected(this.appezzamentiRows);

            const tableData = new AppezzamentiCampoKendoServerResult(model,
                columns,
                this.appezzamentiRows
            );

            return tableData;

        }));
    }

    handleCustomizations(): void {
        this.selectable = new AgrSelectableSettings();
        this.selectable.selectable.checkboxOnly = true;
        this.selectable.selectable.enabled = true;
        this.selectable.shouldShowCheckbox = true;
        this.selectable.columnSettings.showSelectAll = true;


        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = false;
        this.toolbar.resetChanges = false;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            infoBtn: false,
            removeBtn: false,
            onDisableInfoBtn: () => false
        });;

        this.resizable.autoFitColumns = false;
        this.selectable.columnSettings.showSelectAll=true;
        this.selectable.shouldShowCheckbox = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB != Enum_DBTypeOperation.Read;
        this.selectable.columnSettings.title=' ';
        this.resizable.isResizable = true;
    }

    private setKendoColumns () {
        const kendoColumns: Array<KendoGridColumn> = [
            new KendoGridColumn(
                { field: 'app_nome', title: this.transloco.translate('Appezzamento') },
                { resizable: true, editable: false, validators: [Validators.required] }
            ),
            new KendoGridColumn(
                { field: 'sup_app', title: this.transloco.translate('SuperficieAppezzamentoAbbr') },
                { resizable: true, editable: false, validators: [Validators.required] }
            ),
            new KendoGridColumn(
                { field: 'validita_inizio', title: this.transloco.translate('ValiditàInizio') },
                { resizable: true, editable: false, validators: [Validators.required] }
            ),
            new KendoGridColumn(
                { field: 'validita_fine', title: this.transloco.translate('ValiditàFine') },
                { resizable: true, editable: false, validators: [Validators.required] }
            ),
            new KendoGridColumn(
                { field: 'coltura_corrente', title: this.transloco.translate('ColturaCorrente') },
                { resizable: true, editable: false, validators: [Validators.required] }
            ),
            new KendoGridColumn(
                { field: 'impianto_validita_inizio', title: this.transloco.translate('InizioValiditàColtura') },
                { resizable: true, editable: false, validators: [Validators.required] }
            ),
            new KendoGridColumn(
                { field: 'impianto_validita_fine', title: this.transloco.translate('FineValiditàColtura') },
                { resizable: true, editable: false, validators: [Validators.required] }
            )
        ];

        return kendoColumns;
    }

    private setKendoModel () {
        const kendoModel: KendoGridModel = {
            piva: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            sa_cod: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            campo_cod: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            appezza: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            sup_app: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            app_nome: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            validita_inizio: {
                editable: false,
                type: CELL_TYPES.DATE
            },
            validita_fine: {
                editable: false,
                type: CELL_TYPES.DATE
            },
            id_reg: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            impianto_validita_inizio: {
                editable: false,
                type: CELL_TYPES.DATE
            },
            impianto_validita_fine: {
                editable: false,
                type: CELL_TYPES.DATE
            },
            cul_cod: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            cul_des: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            veg_des: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            coltura_corrente: {
                editable: false,
                type: CELL_TYPES.STRING
            }
        };

        return kendoModel;
    }

    private setKendoRows(righe: any[]) {
        this.appezzamentiRows = righe.map((item) => ({
            chiave: item.PIVA + '_' + item.SA_COD + '_' + item.APPEZZA + '_' + item.ID_REG + '_' + item.Campo_Cod,
            piva: item.PIVA,
            sa_cod: parseInt(item.SA_COD),
            campo_cod: parseInt(item.Campo_Cod),
            appezza: parseInt(item.APPEZZA),
            sup_app: parseFloat(item.SUP_APP),
            app_nome: item.APP_NOME,
            validita_inizio: this.intlService.parseDate(item.Validita_Inizio),
            validita_fine: this.intlService.parseDate(item.Validita_Fine),
            id_reg: parseInt(item.ID_REG),
            impianto_validita_inizio: this.intlService.parseDate(item.Impianto_Validita_Inizio),
            impianto_validita_fine: this.intlService.parseDate(item.Impianto_Validita_Fine),
            cul_cod: parseInt(item.CUL_COD),
            cul_des: item.Cul_Des,
            veg_des: item.Veg_des,

            coltura_corrente: this.setColturaCorrente(item)
        }));
    }

    private setColturaCorrente(item: any) {
        if (item.CUL_COD != undefined) {
            if (item.CUL_COD != 0) {
                if (item.Cul_Des != undefined && item.Cul_Des != '') {
                    return (item.Veg_des != '' ? item.Veg_des + '-' : '') + item.Cul_Des;
                } else {
                    return '...';
                }
            } else {
                return 'Terreno Nudo';
            }
        } else {
            return '...';
        }
    }

    private isRowSelected(rows: any[]) {
        for (let i=0; i<rows.length; i++) {
            if ((rows[i].campo_cod == this.objParametriAgenda.Campo_Cod && this.objParametriAgenda.Campo_Cod != 0)) {
                rows[i].Selected = true;
            } else {
                rows[i].Selected = false;
            }
        }
    }

    public selectionChange = (event: SelectionEvent, component: GiasKendoGridComponent) => {
        const appezzamentoCampo = this.appezzamentoCampoService.getAppezzamentoCampo();

        const preselectedRows = component.selectable.preselectedRows.selectedRows;

        const selectedRows = event.selectedRows;

        const deselectedRows = event.deselectedRows;

        if (selectedRows.length !== 0) {

            event.selectedRows.forEach(row => {
                if(this.appezzamentoCampoService.ControllaSeSelezionareLaRiga(row.dataItem, appezzamentoCampo) === -1){

                    const appezzamento = new AppezzamentoCampoId();
                    appezzamento.piva = row.dataItem.chiave.split('_')[0];
                    appezzamento.sa_cod = parseInt(row.dataItem.chiave.split('_')[1]);
                    appezzamento.appezza = parseInt(row.dataItem.chiave.split('_')[2]);
                    appezzamento.id_reg =  parseInt(row.dataItem.chiave.split('_')[3]);
                    appezzamento.campo_cod = parseInt(row.dataItem.chiave.split('_')[4]);
                    appezzamento.app_nome = row.dataItem.app_nome;
                    appezzamento.validita = new IntervalloTemporale(row.dataItem.impianto_validita_inizio, row.dataItem.impianto_validita_fine)
                    const index = Math.max.apply(Math, appezzamentoCampo.map(function (o) {
                        return o.id;
                    }));
                    appezzamento.id = index + 1;

                    appezzamentoCampo.push(appezzamento);

                    // e.sender.editCell(event.rowIndex, event.columnIndex, this.formGroup);
                }
            });

        }

        if (deselectedRows.length !== 0 ) {

            event.deselectedRows.forEach(row => {
                const index=this.appezzamentoCampoService.ControllaSeSelezionareLaRiga(row.dataItem, appezzamentoCampo);

                if(index!==-1){

                    appezzamentoCampo.splice(index,1);

                }
            });

        }

        const Sup=0;


        appezzamentoCampo.forEach((app: AppezzamentoCampoId)=>{
            // Sup += app.area;
        });

        this.appezzamentoCampoService.setAppezzamentoCampo(appezzamentoCampo);
    };

}
