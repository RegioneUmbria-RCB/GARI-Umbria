import { Injectable, Injector } from "@angular/core";
import { AbstractControl, ValidatorFn, Validators } from "@angular/forms";
import { AGRODATAFINE, AGRODATAINIZIO } from "app/Model/CostantiPersonalizzate";
import { enum_LAVCOD } from "app/Model/TipiEnumerativi";
import { GiasMessageService } from "app/Service/gias-message.service";
import { LeggiOperazione, LeggiOperazioni_IN, OperazioneCausale_In, OperazioneClient } from "app/Service/net-core6-api.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { CommandsColumnSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  DropdownListWithForm, EditingMode, KendoGridColumn, KendoGridModel, KendoGridRow, KendoServerResult, LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Observable, catchError, map, of } from "rxjs";

export class GridOperazioneCausaleServerResult extends KendoServerResult {
    constructor(rows: KendoGridRow[], cols: KendoGridColumn[], model: KendoGridModel) {
        super(model, cols, rows);
    }
}

@Injectable()
export class GridOperazioneCausaleHttpService extends AbstractGridConfigService<GridOperazioneCausaleServerResult> {

    gridId = 'OperazioneCausaleId';
    rowId = 'operazioneCausaleIdRow';

    loader: LoaderType = LoaderType.SERVICE;
    // editingMode: EditingMode = EditingMode.IN_PAGE;
    editingMode: EditingMode = EditingMode.IN_LINE;

    allOperazioni = [];

    constructor(injector: Injector,
                public gridpublicService: GridPublicService,
                private giasMessageService: GiasMessageService,
                private objParametriAgendaService: ObjParametriAgendaService,
                private operazioneService: OperazioneClient) {

        super(injector, ConfigTemplate.DefaultTemplate);

        this.cmdColumn = new CommandsColumnSettings({ editBtn: true, infoBtn: false, removeBtn: true });
        this.toolbar = new ToolbarSettings(true, false);

        //carichiamo le operazioni che andranno a comporre la DDL in fase di inserimento
        let param = {
            GruppoOperazioni: [],
            Operazioni: [enum_LAVCOD.RILIEVO_INDICI_RESE_RACCOLTA]
        } as LeggiOperazioni_IN;
        
        this.operazioneService.operazioneGetOperazioni(param).subscribe(r => {
            this.allOperazioni = JSON.parse(r.RispostaStringa).DataTable;
        });
    }


    read(options?: any): Observable<GridOperazioneCausaleServerResult> { 
        
        this.loadingService.set_isLoading({ isLoading: true, component: this.gridPublicService.gridElRef });

        // let param = new LeggiListaAnalisiTerreno(this.objParametriAgenda.getObjParamValue().Piva, AGRODATAINIZIO);

        let param = {
            LavCod: -1,
            Data: null
        } as LeggiOperazione;

        return this.operazioneService.operazioneGetOperazioneCausale(param).pipe(catchError((err) => {
            this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });
            return of();
        }), map((r:any) => {

            this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });

            const causaliRows = JSON.parse(r.RispostaStringa);

            let gridListaAnalisiServerResult = new GridOperazioneCausaleServerResult(
                this.setRowGrid(causaliRows),
                this.setColumnsGrid(),
                this.setModelGrid()
            );

            return gridListaAnalisiServerResult;
        }));
    }

    setModelGrid(): KendoGridModel {
        let gridModel = new KendoGridModel();

        gridModel['Id'] = { editable: false, type: CELL_TYPES.NUMBER };
        gridModel['Causale'] = { editable: true, type: CELL_TYPES.STRING };
        
        gridModel['Lav_Cod'] = { editable: false, type: CELL_TYPES.DROPDOWNLIST };
        gridModel['LAV_DES'] = { editable: false, type: CELL_TYPES.STRING };

        gridModel['Validita_Inizio'] = { editable: false, type: CELL_TYPES.DATE };
        gridModel['Validita_Fine'] = { editable: false, type: CELL_TYPES.DATE };

        return gridModel;
    }

    setColumnsGrid(): KendoGridColumn[] {
        let columns: Array<KendoGridColumn> = [];

        // columns.push(
        //     new KendoGridColumn({ field: 'Id', title: this.transloco.translate('Id') }, { resizable: true, editable: false, hidden: false})
        // );

        columns.push(
            new KendoGridColumn({ field: 'Causale', title: this.transloco.translate('Causale') }, { resizable: true, editable: true, validators: [Validators.required] })
        );

        let ddl;

        const data = [];
        ddl = new DropdownListWithForm('LAV_COD', 'Lav_Cod', 'LAV_DES', data);
        ddl.valuePrimitive = true;
        ddl.loadOnEdit = true;
        ddl.loadFunction = this.loadDdl.bind(this);
        ddl.descriptionField = 'LAV_DES';

        columns.push(
            new KendoGridColumn({ field: 'Lav_Cod', title: this.transloco.translate('Operazione') }, { resizable: true, editable: true, ddl: ddl, validators: [Validators.required] })
        );

        columns.push(
            new KendoGridColumn({ field: 'Id', title: this.transloco.translate('Codice') }, { resizable: true, editable: false, hidden: true })
        );

        columns.push(
            new KendoGridColumn({ field: 'Validita_Inizio', title: this.transloco.translate('ValiditaInizio') }, { resizable: true, editable: true, date: { defaultValue: AGRODATAINIZIO, min: AGRODATAINIZIO, max: AGRODATAFINE, format: 'dd/MM/yyyy', placeholder: '' }/*, validators: [this.checkDates]*/ })
        );

        columns.push(
            new KendoGridColumn({ field: 'Validita_Fine', title: this.transloco.translate('ValiditaFine') }, { resizable: true, editable: true, date: { defaultValue: AGRODATAFINE, min: AGRODATAINIZIO, max: AGRODATAFINE, format: 'dd/MM/yyyy', placeholder: '' }/*, validators: [this.checkDates]*/ })
        );

        return columns;
    }

    // DCA 20250121: un modo per intercettare gli errori prima di cliccare sulla spunta verde
    // private checkDates: ValidatorFn = (control: AbstractControl) => {

    //     const validitaInizio = control?.parent?.get('Validita_Inizio')?.value;
    //     const validitaFine = control.parent?.get('Validita_Fine')?.value;
      
    //     if (validitaFine && validitaInizio && new Date(validitaFine) < new Date(validitaInizio)) {
    //         // this.giasMessageService.errorMessage(this.transloco.translate('DataInizioNonDeveEssereMaggioreDiDataFine'));
    //         return { invalidDateRange: true };
    //     }

    //     // console.log(control);
    //     control?.parent?.get('Validita_Fine')?.setErrors(null);
    //     control?.parent?.get('Validita_Inizio')?.setErrors(null);

    //     return null; // Validazione riuscita
    // };

    loadDdl() {
        return of(this.allOperazioni);
    }
    
    setRowGrid(responseRows: any): KendoGridRow[] {
        let rows: Array<KendoGridRow> = [];

        let index = 1;

        for (let itemOfRows of responseRows) {
            itemOfRows.operazioneCausaleIdRow = index++;

            rows.push(
                itemOfRows
            );
        }

        return rows;
    }

    perform(actionType: HttpAction, items: any, oldRow?: any): Observable<any> {

        switch(actionType) {
            case HttpAction.UPDATE:
            case HttpAction.CREATE:

                if (items.Validita_Fine < items.Validita_Inizio) {
                    this.giasMessageService.errorMessage(this.transloco.translate('DataInizioNonDeveEssereMaggioreDiDataFine'));
                    return of(false);
                }

                let param = {
                            Id: items.Id,
                            Causale: items.Causale,
                            LavCod: items.Lav_Cod,
                            inviato: 0,
                            validitaInizio: items.Validita_Inizio,
                            validitaFine: items.Validita_Fine,
                            isNew: (actionType == HttpAction.CREATE) ? true : false
                } as OperazioneCausale_In;

                this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});

                this.operazioneService.operazioneSaveOperazioneCausale(param).subscribe(r => {

                    this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

                    if (r.RispostaStringa === 'true')
                        this.gridPublicService.refresh(true);
                });
            break;

            case HttpAction.REMOVE: 

                this.loadingService.set_isLoading({isLoading: true, message: '', component: this.gridPublicService.gridElRef});
                
                this.operazioneService.operazioneCancellaOperazioneCausale(items.Id, items.Lav_Cod).subscribe(r => {

                    this.loadingService.set_isLoading({isLoading: false, message: '', component: this.gridPublicService.gridElRef});

                    if (r.RispostaStringa === 'true') {
                        this.gridPublicService.refresh(true);
                    } else {
                        this.giasMessageService.errorMessage(this.transloco.translate('CausaleInAlmenoUnAttivita'), false, false);
                    }
                });
            break;
        }

        return of([]);
    }

}