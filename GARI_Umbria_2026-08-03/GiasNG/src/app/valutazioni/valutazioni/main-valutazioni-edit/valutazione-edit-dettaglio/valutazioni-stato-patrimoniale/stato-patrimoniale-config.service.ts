import { Injectable, Injector } from "@angular/core";
import { TranslocoService } from "@jsverse/transloco";
import { GroupDescriptor } from "@progress/kendo-data-query";
import { SMARTPHONE_WIDTH } from "app/Model/CostantiPersonalizzate";
import { GiasDialogService } from "app/Service/gias-dialog.service";
import { GiasMessageService } from "app/Service/gias-message.service";
import { ObjParametriAgendaService } from "app/Service/obj-parametri-agenda.service";
import {
  ToolbarSettings,
  CommandsColumnSettings,
  CommandsDropDownSettings,
  CommandsDropDownEvents,
  RemoveMultipleRowsParams,
  MasterDetailSettings,
  CustomColumnSettings
} from 'gias-kendo-grid';
import { EditingMode, GridCustomizations, KendoGridColumn, ModelEntry,  LoaderType } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { ConfigTemplate } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { GridPublicService } from 'gias-kendo-grid';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { StatoPatrimonialeService } from "app/valutazioni/valutazioni/service/stato-patrimoniale.service";
import { ValutazioniService } from "app/valutazioni/valutazioni/service/valutazioni.service";
import { StatoPatrimonialeModel, StatoPatrimonialeKendoServerResult } from "app/valutazioni/valutazioni/valutazioni.models";
import { takeUntil, take, Observable, map, of, switchMap } from "rxjs";
import { ObjParametriAgenda } from 'gias-ui-kit';

@Injectable()
export class StatoPatrimonialeHttpService extends AbstractGridConfigService<any> {
    gridId = 'StatoPatrimonialeHttpService';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;
    rowId = 'Dettaglio_Key';
    objParametriAgenda: ObjParametriAgenda;

    /** Optional parameters */
    toolbar = new ToolbarSettings(true, false);
    views = new GridCustomizations({ enabled: true });

    public permessoEdit: boolean;
    public permessoRemove: boolean;
    public permessoInfo: boolean;

    public initialize: boolean = false;


    columns: KendoGridColumn[];
    model: StatoPatrimonialeModel;
    comuneColumn: KendoGridColumn;

    constructor(injector: Injector,
        private objParametriService: ObjParametriAgendaService,
        private gridPublicSerivce: GridPublicService,
        private translocoService: TranslocoService,
        private statoPatrimonialeService: StatoPatrimonialeService,
        private valutazioneService: ValutazioniService,
        private giasMessageService : GiasMessageService,
        private giasDialogService : GiasDialogService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.permessoEdit = false;
        this.permessoRemove = false;
        this.permessoInfo = false;

        this.gridPublicSerivce.changeDetected.pipe(
            takeUntil(this.signal)
        ).subscribe((event: any) => {
            console.log(event);
            if (event.action == 'add') {
                this.gridPublicSerivce.formGroup.pipe(take(1)).subscribe((fg) => {
                    fg.controls['Rag_Soc'].setValue(0);
                })
            }
        })

        this.columns = [
            new KendoGridColumn({ field: 'Dettaglio_Key', title: this.translocoService.translate('ChiaveRiga') }, { resizable: true, editable: this.permessoEdit, hidden: true, width: 135 }),
            new KendoGridColumn({ field: 'Valutazione_Conto_Des', title: this.translocoService.translate('Conto') }, { resizable: true, editable: this.permessoEdit, width: 135 }),
        ]

        this.model = {
            Dettaglio_Key: new ModelEntry(CELL_TYPES.STRING, false),
            Valutazione_Conto_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Attivo_Passivo_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Valutazione_Gruppo_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Valutazione_Sezione_Des: new ModelEntry(CELL_TYPES.STRING, false),
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
        this.groups.groupable.enabled = true;
        this.masterdetailSettings = new MasterDetailSettings(true,{flag_grid_detail: false,flag_grid_master: true,showDetailTemplate: (dataitem:any,rowIndex:number)=> dataitem.Dettaglio_Specifico != '[]'});

        this.customColumn = new CustomColumnSettings({showColumn: true,includeInChooser: false,width: 70,useCustomColumnCellTemplate: true});

        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.cmdColumn.editBtn = false;
            this.toolbar.newItem = false;
        }

        this.pagination.gridState.group = [
            { field: 'Attivo_Passivo_Des'} as GroupDescriptor,
            { field: 'Valutazione_Gruppo_Des' } as GroupDescriptor,
            { field: 'Valutazione_Sezione_Des'} as GroupDescriptor
        ];
    }

    read(): Observable<StatoPatrimonialeKendoServerResult> {


        let objParametriAgenda = this.objParametriService.getObjParamValue();
        let testata_lettura = this.valutazioneService.getLeggiTestataValue();

        return this.statoPatrimonialeService.leggiStatoPatrimoniale(objParametriAgenda.Piva, testata_lettura.idTestata).pipe(
            map((data: any[]) => this.transform(data))
        );
    }

    private transform(data: any[]): StatoPatrimonialeKendoServerResult {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

        if (data.length > 0) {
            let dataObject: string[] = Object.keys(data[0]);

            let listaAnni: string[] = [];
            if (!this.initialize) {

                for (i = 0; i <= 2; i++) {
                    if (!isNaN(+dataObject[i])) {
                        listaAnni.push(dataObject[i]);
                    }
                }

                listaAnni.forEach(x => {
                    this.model[x] = new ModelEntry(CELL_TYPES.NUMBER, false);
                    let kendoGridColumn: KendoGridColumn = new KendoGridColumn({ field: x, title: x }, { resizable: true, editable: true, format:'{0:n2}', numeric: { defaultValue: 0, min: -100000000, format: 'n2' }, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)',style: { "text-align": "right" }, width: 135 });
                    this.columns.push(kendoGridColumn);
                });
            }
        }

        if (!this.initialize) {
            this.columns.push(new KendoGridColumn({ field: 'Attivo_Passivo_Des', title: this.translocoService.translate('Attivo_Passivo_Des') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 175 }));
            this.columns.push(new KendoGridColumn({ field: 'Valutazione_Gruppo_Des', title: this.translocoService.translate('ValutazioneGruppoDescrizione') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }));
            this.columns.push(new KendoGridColumn({ field: 'Valutazione_Sezione_Des', title: this.translocoService.translate('Valutazione_Sezione_Des') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }));
            this.initialize = true;
        }

        return {
            model: this.model,
            rows: data,
            columns: this.columns
        };
    }

    perform(actionType: HttpAction, items: any): Observable<any> {
        const item = items as any;
        console.log(item);
        switch (actionType) {

            case HttpAction.UPDATE:
                this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

                let testata_lettura = this.valutazioneService.getLeggiTestataValue();
                item.Id_Testata = testata_lettura.idTestata;
                item.Piva = testata_lettura.piva;
                item.Dettaglio_Specifico = [];


                console.log(item);
                let items : any[] = new Array();
                items.push(item)

                return this.valutazioneService.scriviDettaglioGriglia(JSON.stringify(items)).pipe(
                    switchMap((resp) => {
                        this.loadingService.set_isLoading({ isLoading: false, component: this.gridPublicService.gridElRef });
                        if (resp.RispostaOK) {
                            this.giasMessageService.successMessage(this.translocoService.translate('StatoPatrimonialeSalvatoCorettamente'));
                        }
                        return of([]);
                    })
                );

        }
        return this.read() as any;
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

        msg = this.translocoService.translate('Stato_Patrimoniale.Delete_Msg')
        opts.data.forEach(r => {
            msg = msg.concat('\n' + r['Rag_Soc']);
        })

        return msg;
    }



}
