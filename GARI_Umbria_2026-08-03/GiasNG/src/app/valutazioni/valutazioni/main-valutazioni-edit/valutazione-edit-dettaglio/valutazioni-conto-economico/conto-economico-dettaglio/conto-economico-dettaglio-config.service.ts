import { Injectable, Injector } from "@angular/core";
import { FormGroup } from "@angular/forms";
import { TranslocoService } from "@jsverse/transloco";
import { SMARTPHONE_WIDTH } from "app/Model/CostantiPersonalizzate";
import { GiasMessageService } from "app/Service/gias-message.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import { ToolbarSettings, CommandsColumnSettings, CommandsDropDownSettings, CommandsDropDownEvents, RemoveMultipleRowsParams, MasterDetailSettings } from 'gias-kendo-grid';
import { EditingMode, GridCustomizations, KendoGridColumn, ModelEntry,  LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { KendoGridMasterDetailService } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { ValutazioniService } from "app/valutazioni/valutazioni/service/valutazioni.service";
import { DettaglioSpecificoModel, DettaglioSpecificoServerResult } from "app/valutazioni/valutazioni/valutazioni.models";
import { takeUntil, take, Observable, of, switchMap, Subscription } from "rxjs";
import { ObjParametriAgenda } from 'gias-ui-kit';


@Injectable()
export class ContoEconomicoDettaglioConfigHttpService extends AbstractGridConfigService<any> {
    gridId = 'ContoEconomicoDettaglioHttpService';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;
    rowId = 'Dettaglio_Key';
    objParametriAgenda: ObjParametriAgenda;

    /** Optional parameters */
    toolbar = new ToolbarSettings(true, false);
    views = new GridCustomizations({ enabled: true });
    initialize: boolean = false;

    public permessoEdit: boolean;
    public permessoRemove: boolean;
    public permessoInfo: boolean;


    columns: KendoGridColumn[];
    model: DettaglioSpecificoModel;
    comuneColumn: KendoGridColumn;

    rows = [];

    Subs: Subscription = new Subscription();


    constructor(injector: Injector,
        private objParametriService: ObjParametriAgendaService,
        private gridPublicSerivce: GridPublicService,
        private translocoService: TranslocoService,
        private valutazioniService: ValutazioniService,
        private giasMessageService: GiasMessageService,
        private kendoGridMasterservice: KendoGridMasterDetailService,
        private gridpublicService: GridPublicService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.permessoEdit = false;
        this.permessoRemove = false;
        this.permessoInfo = false;

        this.gridPublicSerivce.changeDetected.pipe(
            takeUntil(this.signal)
        ).subscribe((event: any) => {
            if (event.action == 'add') {
                this.gridPublicSerivce.formGroup.pipe(take(1)).subscribe((fg) => {
                    fg.controls['Rag_Soc'].setValue(0);
                })
            }
        })

        this.columns = [
            new KendoGridColumn({ field: 'Dettaglio_Key', title: this.translocoService.translate('ChiaveRiga') }, { resizable: true, hidden: true, editable: false, width: 135 }),
            new KendoGridColumn({ field: 'Descrizione', title: this.translocoService.translate('Descrizione') }, { resizable: true, editable: false, width: 135 }),
        ]

        this.model = {
            Dettaglio_Key: new ModelEntry(CELL_TYPES.STRING, false),
            Descrizione: new ModelEntry(CELL_TYPES.STRING, false),
            Valutazione_Conto_Cod: new ModelEntry(CELL_TYPES.NUMBER, false),
        }

        this.selectable.selectable.checkboxOnly = false;
        this.selectable.selectable.enabled = this.permessoEdit;

        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = false;
        this.toolbar.resetChanges = false;
        this.pagination.gridState.take = 100;

        let objParametriAgenda = this.objParametriService.getObjParamValue();

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: (objParametriAgenda.TipoOperazioneDB == Enum_DBTypeOperation.Update),
            removeBtn: this.permessoRemove,
        });

        this.cmdDropDown = new CommandsDropDownSettings({
            fullEditBtn: this.permessoEdit,
            infoBtn: this.permessoInfo,
        });
        this.gridPublicSerivce.commandEvent.GiasSubscribe(ev => {
            if (!ev) return;
            switch (ev.command.action) {
                case CommandsDropDownEvents.FULL_EDIT:
                    this.onTemplateBtnClick(ev.dataItem);
                    break;
            }
        })

        this.resizable.autoFitColumns = false;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.shouldShowCheckbox = false;
        this.selectable.columnSettings.title = ' ';
        this.resizable.isResizable = true;
        this.selectable.selectable.enabled = true;
        this.groups.groupable.enabled = false;
        this.masterdetailSettings = new MasterDetailSettings(true, {
             flag_grid_detail: true,
              flag_grid_master: false
             });

        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.cmdColumn.editBtn = false;
            this.toolbar.newItem = false;
        }



    }

    read(dataItem: any): Observable<DettaglioSpecificoServerResult> {
        console.log(dataItem);
        let dettaglioSpecifico: any = JSON.parse(dataItem.Dettaglio_Specifico)

        return of(this.transform(dataItem, dettaglioSpecifico));
    }

    private transform(dataItem: any, data: any[]): DettaglioSpecificoServerResult {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });


        if (!this.initialize) {
            let dataObject: string[] = Object.keys(data[0]).sort((a, b) => a.localeCompare(b));

            dataObject = this.swapDataObject(dataObject);

            for (i = 0; i <= 11; i++) {
                if (dataObject[i] !== undefined) {

                    if (dataObject[i].includes("_Valore_Ha") ||
                        dataObject[i].includes("_Valore_Peso") ||
                        dataObject[i].includes("_Valore_Totale") ||
                        dataObject[i].includes("_Valore_Unitario")) {
                        this.model[dataObject[i]] = new ModelEntry(CELL_TYPES.NUMBER, false);
                        this.getTitleField(dataObject[i]);
                    }
                }

            };

            this.initialize = true;
        }


        let index = this.kendoGridMasterservice?.GridDataDetail?.findIndex(x => x.GridMasterRowId === dataItem[this.kendoGridMasterservice.RowIdGridMaster]);
        this.rows = data;
        if (index > -1 && this.kendoGridMasterservice.GridDataDetail[index]?.GridData?.data) {
            this.rows = this.kendoGridMasterservice.GridDataDetail[index].GridData.data.rows;
        }


        return {
            model: this.model,
            rows: this.rows,
            columns: this.columns
        };
    }

    swapDataObject(dataObject: string[]): string[] {
        for (i = 2; i <= 10; i = i + 4) {
            if (dataObject[i] !== undefined) {
                if (dataObject[i].includes("_Valore_Totale")) {
                    const tmp: string = dataObject[i + 1];
                    dataObject[i + 1] = dataObject[i];
                    dataObject[i] = tmp;
                }
            }
        }

        return dataObject
    }


    getTitleField(value: string): void {

        let limit: number = 4;

        const substring: string = value.substring(0, limit);
        if (!isNaN(+substring)) {

            if (value.includes("_Valore_Ha")) {
                this.columns.push(new KendoGridColumn({ field: value, title: this.translocoService.translate('Ha') + " " + substring }, { resizable: true, editable: false, format: '{0:n4}', numeric: { defaultValue: 0, min: 0, format: 'n4' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', style: { "text-align": "right" }, width: 115 }));
            }

            if (value.includes("_Valore_Peso")) {
                this.columns.push(new KendoGridColumn({ field: value, title: this.translocoService.translate('Resa_Kg_Ha') + " " + substring }, { resizable: true, editable: false, format: '{0:n2}', numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', style: { "text-align": "right" }, width: 150 }));
            }


            if (value.includes("_Valore_Totale")) {
                this.columns.push(new KendoGridColumn({ field: value, title: this.translocoService.translate('Euro_Totali') + " " + substring }, { resizable: true, editable: true, format: '{0:n2}', numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', style: { "text-align": "right" }, width: 135 }));
            }


            if (value.includes("_Valore_Unitario")) {
                this.columns.push(new KendoGridColumn({ field: value, title: this.translocoService.translate('Euro_Kg') + " " + substring }, { resizable: true, editable: true, format: '{0:n2}', numeric: { defaultValue: 0, min: 0, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', style: { "text-align": "right" }, width: 135 }));

                /*subscription per il cambio di valori*/
                this.Subs.add(this.gridPublicService.formGroup.subscribe(async (formGroup: FormGroup) => {

                    if (formGroup) {

                        formGroup.patchValue({
                            Riga_Salvata: false
                        }, { emitEvent: false });

                        const valUnitario = (formGroup.get(substring+'_Valore_Unitario') as FormGroup);
                        const valPeso = (formGroup.get(substring+'_Valore_Peso') as FormGroup);
                        const valHa = (formGroup.get(substring+'_Valore_Ha') as FormGroup);


                        this.Subs.add(valUnitario.valueChanges.subscribe((value: string) => {

                            let numUnitario: number = valUnitario.value;
                            let numPeso: number = valPeso.value;
                            let numHa: number = valHa.value;

                            formGroup.controls[substring+'_Valore_Totale'].setValue(numUnitario * numPeso * numHa, { emitEvent: false });

                        }));
                    }

                }));
            }
        }
    }


    perform(actionType: HttpAction, items: any): Observable<any> {
        const item = items as any;

        switch (actionType) {

            case HttpAction.UPDATE:
                this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

                let testata_lettura = this.valutazioniService.getLeggiTestataValue();
                item.Id_Testata = testata_lettura.idTestata;
                item.Piva = testata_lettura.piva;

                console.log(item);
                let items: any[] = new Array();
                items.push(item)

                return this.valutazioniService.scriviDettaglioGrigliaFiglia(JSON.stringify(items)).pipe(
                    switchMap((resp) => {
                        if (resp.RispostaOK) {
                            this.giasMessageService.successMessage(this.translocoService.translate('ContoEconomicoFiglioSalvatoCorettamente'));
                            this.rows[this.rows.findIndex(x => x[this.rowId] === item[this.rowId])] = item;

                            console.log("recupera chiave master");
                            //Recupero la chiave della master
                            let key = this.gridpublicService.Current_Grid_Master_RowExpanded[this.kendoGridMasterservice.RowIdGridMaster];

                            let GridData = this.kendoGridMasterservice?.GridDataDetail?.find(x => x.GridMasterRowId === key).GridData;

                            GridData.data.rows = this.rows;

                            this.kendoGridMasterservice?.get_GridDetailService(key).pubService.refresh(false, GridData.data);
                        }
                        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
                        return of([]);
                    })
                );

        }
    }

    onTemplateBtnClick(dataItem) {
        this.objParametriAgenda = this.objParametriService.getObjParamValue();
        const piva = this.objParametriAgenda.Piva;
        this.objParametriAgenda.Piva = piva;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
        let leggi_Testata = {
            piva: dataItem.Piva,
            idTestata: dataItem.Id_Testata,
            includiAnno: true
        }
        // this._valutazioni.changeLeggiTestata(leggi_Testata);
        //this.router.navigate(['Valutazioni-Edit'], { relativeTo: this.route });
    }

    onInfoBtnClick(dataItem) {
        this.objParametriAgenda = this.objParametriService.getObjParamValue();
        const piva = (<string>dataItem.chiave).split('_')[0];
        const sa_cod = (<string>dataItem.chiave).split('_')[1];
        this.objParametriAgenda.Piva = piva;
        this.objParametriAgenda.Sa_Cod = parseInt(sa_cod);
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);

        //this.router.navigate(['Valutazioni-Edit'], { relativeTo: this.route });
    }


    override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
        let msg: string;

        msg = this.translocoService.translate('Conto_Economico.Delete_Msg')
        opts.data.forEach(r => {
            msg = msg.concat('\n' + r['Rag_Soc']);
        })

        return msg;
    }



}
