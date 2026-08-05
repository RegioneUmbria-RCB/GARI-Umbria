import { Injectable, Injector } from '@angular/core';
import { CommandsColumnSettings, ExcelSettings, PDFSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { from, Observable} from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service'; 
import { ObjParametriAgenda } from 'gias-ui-kit';
import { map } from 'rxjs/operators';
import { Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { ConsultaSincroLogKendoServerResult } from './consulta-sincro-log-grid-model';
import { ConsultaSincroServerResult, ConsultaSincroService, enum_Dati_App, enum_Stato_Applicazione, enum_Tipo_Aggiornamento, enum_Esito } from './consulta-sincro.service';
import { MasterService } from 'app/Service/master.service';
import { enum_Esportazioni_Sistema_Cod } from 'app/Model/TipiEnumerativi';

export class ConsultaSincroKendo extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class ConsultaSincroLogGridService extends AbstractGridConfigService<ConsultaSincroKendo> {
    gridId = 'sincroLogGrid';
    rowId = 'Chiave';

    objParametriAgenda: ObjParametriAgenda;
    toolbar = new ToolbarSettings(false, false);

    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    KendoSincroLog: any;

    sincroLogRows: any[];
    sincroLogColumns: KendoGridColumn[];

    isDemetra: boolean = false;
    //sincroLogModel: ConsultaSincroLogModel[];
    isFirstLoad: boolean = true;

    messageForDemetra: string = '';

    constructor(
        injector: Injector,
        private consultaSincroService: ConsultaSincroService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private translocoLoc: TranslocoService,
        private masterService: MasterService) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        // this.resizable.autoFitColumns = true;
        this.resizable.isResizable = true;

        // Setting generali
        this.columnMenu.columnMenu = true;
        this.behavior.saveExternalChanges = true;
        this.behavior.excelSettings = new ExcelSettings({enabled: false});
        this.behavior.pdfSettings = new PDFSettings({enabled: false});
        this.generalSettings.reordable = true;
        this.generalSettings.performOnEdit = true;
        this.groups.groupable.enabled = false;

        //* ************************
        this.handleCustomizations();

        /*this.objParametriAgendaService.currentObjParametriAgenda.GiasSubscribe(p => {
            this.gridPublicService.refresh(true);
        })*/
    }

    perform(actionType: HttpAction, items: any): Observable<any[]> {
        return from([]);
    }

    read(): Observable<ConsultaSincroKendo> {

        const agenda: ObjParametriAgenda = this.objParametriAgendaService.getObjParamValue();

        this.masterService.set_isLoading({ isLoading: true });

        const sincroLog: Observable<any> = this.isDemetra ? this.consultaSincroService.caricaLogInterscambio() : this.consultaSincroService.caricaSincroLog();

        if (this.isDemetra && this.isFirstLoad) {
            this.isFirstLoad = false;

            const model = this.setKendoModel();
            const cols = this.setKendoColumns();
            const tableData = new ConsultaSincroLogKendoServerResult(model,
                cols,
                []
            );
            this.masterService.set_isLoading({ isLoading: false });
            return from([tableData]);
        }

        return sincroLog.pipe(map(result => {

            let arrayResult: any[] = [];

            if (this.isDemetra) {
                arrayResult = result.ListaLog;
                this.messageForDemetra = result.MsgCodaDataPublish;
            } else {
                arrayResult = result;
            }

            const righe = arrayResult.map((elem: ConsultaSincroServerResult) => {
                return {
                    Chiave: elem.azienda_cod,
                    Utente: elem.utente,
                    Azienda: elem.azienda_des,
                    Data_Sincro: elem.data_sincro,
                    Dati: elem.Dati,
                    Tipo_Dato: this.translocoLoc.translate(this.isDemetra ? enum_Esportazioni_Sistema_Cod[elem.tipo_dato].toString() : enum_Dati_App[elem.tipo_dato].toString()),
                    Descrizione: elem.descrizione,
                    Tipo_Aggiornamento: this.translocoLoc.translate(enum_Tipo_Aggiornamento[parseInt(elem.tipo_aggiornamento)].toString()),
                    Stato_Applicazione: this.translocoLoc.translate(this.isDemetra ? enum_Esito[elem.stato_applicazione].toString().toUpperCase() : enum_Stato_Applicazione[elem.stato_applicazione].toString()),
                    Errori: elem.errori
                }
            });
            const model = this.setKendoModel();
            const cols = this.setKendoColumns();

            this.setKendoRows(righe);

            const tableData = new ConsultaSincroLogKendoServerResult(model,
                cols,
                this.sincroLogRows
            );
            this.masterService.set_isLoading({ isLoading: false });
            return tableData;

        }));
    }

    handleCustomizations(): void {
        //this.selectable = new AgrSelectableSettings();
        //this.selectable.selectable.checkboxOnly = true;
        //this.selectable.selectable.enabled = true;
        //this.selectable.shouldShowCheckbox = true;
        //this.selectable.columnSettings.showSelectAll = true;

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
        //this.selectable.columnSettings.showSelectAll=true;
        //this.selectable.shouldShowCheckbox = this.objParametriAgendaService.getObjParamValue().TipoOperazioneDB != Enum_DBTypeOperation.Read;
        //this.selectable.columnSettings.title=' ';
        this.resizable.isResizable = true;
        //this.selectable.selectable.checkboxOnly = true;
        //this.selectable.selectable.enabled = true;
        this.views.enabled = true
        this.columnMenu.columnMenu = false;
        this.behavior.excelSettings = new ExcelSettings({enabled:true});
        this.groups.groupable.enabled = true;
    }

    private setKendoColumns () {
        if(!this.gridPublicService?.giasGridComponent?.columns) {
            return this.setColumnsGridSincroLog();
        } else {
            let columns = this.gridPublicService?.giasGridComponent?.columns;
            columns.find(t => t.field == 'Descrizione').hidden = !this.consultaSincroService.getDettagliAggiuntivi();
            return columns;
        }
    }

    private setColumnsGridSincroLog () {
        const kendoColumns: Array<KendoGridColumn> = [
            new KendoGridColumn(
                { field: 'Utente', title: this.translocoLoc.translate('Utente') },
                { resizable: true, editable: false, validators: [Validators.required], width: 80 }
            ),
            new KendoGridColumn(
                { field: 'Azienda', title: this.translocoLoc.translate('Azienda') },
                { resizable: true, editable: false, validators: [Validators.required], width: 100 }
            ),
            new KendoGridColumn(
                { field: 'Data_Sincro', title: this.isDemetra ? this.translocoLoc.translate("Data") : this.translocoLoc.translate('DataSincronizzazione') },
                { resizable: true, editable: false, validators: [Validators.required], width: 120 }
            ),
            new KendoGridColumn(
                { field: 'Dati', title: this.translocoLoc.translate('Dati') },
                { resizable: true, editable: false, validators: [Validators.required], width: 100 }
            ),
            new KendoGridColumn(
                { field: 'Tipo_Dato', title: this.translocoLoc.translate('Tipo_Dato') },
                { resizable: true, editable: false, validators: [Validators.required], width: 100 }
            ),
            new KendoGridColumn(
                { field: 'Descrizione', title: this.translocoLoc.translate('Descrizione') },
                { resizable: true, editable: false, hidden: !this.consultaSincroService.getDettagliAggiuntivi(), validators: [Validators.required], width: 100, showHTMLAsString: true, includeInChooser: false}
            ),
            new KendoGridColumn(
                { field: 'Tipo_Aggiornamento', title: this.isDemetra ? this.translocoLoc.translate("Tipo_Operazione") : this.translocoLoc.translate('Tipo_Aggiornamento') },
                { resizable: true, editable: false, validators: [Validators.required],width: 120 }
            ),
            new KendoGridColumn(
                { field: 'Stato_Applicazione', title: this.isDemetra ? this.translocoLoc.translate("Stato") : this.translocoLoc.translate('Stato_Applicazione') },
                { resizable: true, editable: false, validators: [Validators.required], width: 120 }
            ),
            new KendoGridColumn(
                { field: 'Errori', title: this.translocoLoc.translate('Errori') },
                { resizable: true, editable: false, validators: [Validators.required], width: 140 }
            )
        ]

        return kendoColumns;
    }

    private setKendoModel () {
        const kendoModel: KendoGridModel = {
            Chiave: {
                editable: false,
                type: CELL_TYPES.NUMBER
            },
            Utente: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Azienda: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Data_Sincro: {
                editable: false,
                type: CELL_TYPES.DATETIME
            },
            Dati: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Tipo_Dato: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Descrizione: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Tipo_Aggiornamento: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Stato_Applicazione: {
                editable: false,
                type: CELL_TYPES.STRING
            },
            Errori: {
                editable: false,
                type: CELL_TYPES.STRING
            }
        };

        return kendoModel;
    }

    private setKendoRows(righe: any[]) {
        this.sincroLogRows = righe.map((item) => ({
            Chiave: item.Chiave,
            Utente: item.Utente,
            Azienda: item.Azienda,
            Dati: item.Dati,
            Data_Sincro: item.Data_Sincro,
            Tipo_Dato: item.Tipo_Dato,
            Descrizione: item.Descrizione,
            Tipo_Aggiornamento: item.Tipo_Aggiornamento,
            Stato_Applicazione: item.Stato_Applicazione,
            Errori: item.Errori
        }));
    }
}
