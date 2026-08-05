import { Injectable, Injector } from '@angular/core';
import { CommandsColumnSettings, CommandsDropDownEvents, CommandsDropDownSettings, RemoveMultipleRowsParams, ToolbarSettings } from 'gias-kendo-grid';
import { EditingMode, GridCustomizations, KendoGridColumn, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, of } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { GridPublicService } from 'gias-kendo-grid';
import { TranslocoService } from '@jsverse/transloco';
import { ValutazioniKendoServerResult, ValutazioniModel } from './valutazioni.models';
import { ValutazioniService } from './service/valutazioni.service';
import { ValutazioneTestata } from 'app/Model/valutazioni/ValutazioneTestata';
import { DeleteMessageService } from 'app/Service/delete-message.service';
import { GridCommandItem } from 'app/menu-agenda/components/utils';
import { faPlusSquare } from '@fortawesome/free-solid-svg-icons';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { ObjParametriAgenda } from 'gias-ui-kit';

enum GriValutazioniActions {
    DOWNLOAD_EXCEL = -1,
}




@Injectable()
export class ValutazioniHttpService extends AbstractGridConfigService<any> {
    gridId = 'ValutazioniHttpService';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;
    rowId = 'Id_Testata';
    objParametriAgenda: ObjParametriAgenda;

    /** Optional parameters */
    toolbar = new ToolbarSettings(true, false);
    views = new GridCustomizations({ enabled: true });

    public permessoEdit: boolean;
    public permessoRemove: boolean;
    public permessoInfo: boolean;


    columns: KendoGridColumn[];
    model: ValutazioniModel;
    comuneColumn: KendoGridColumn;

    constructor(injector: Injector,
        private objParametriService: ObjParametriAgendaService,
        private permessiUtenteService: PermessiUtenteService,
        private router: Router,
        private route: ActivatedRoute,
        private gridPublicSerivce: GridPublicService,
        private translocoService: TranslocoService,
        private _valutazioni: ValutazioniService,
        private deleteMessageService: DeleteMessageService,
        private giasMessageService: GiasMessageService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);
        this.permessoRemove = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);
        this.permessoInfo = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 0);

        this.gridPublicSerivce.changeDetected.pipe(
            takeUntil(this.signal)
        ).subscribe((event: any) => {
            if (event.action === 'add') {
                this.gridPublicSerivce.formGroup.pipe(take(1)).subscribe((fg) => {
                    fg.controls['Rag_Soc'].setValue(0);
                });
            }
        });

        this.columns = [
            new KendoGridColumn({ field: 'Rag_Soc', title: this.translocoService.translate('Ragione_Sociale1') }, { resizable: true, editable: this.permessoEdit, width: 175 }),
            new KendoGridColumn({ field: 'partitaIvaReale', title: this.translocoService.translate('PartitaIVA') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120 }),
            new KendoGridColumn({ field: 'Id_Testata', title: this.translocoService.translate('IDTestata') }, { resizable: true, editable: this.permessoEdit, hidden: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),
            new KendoGridColumn({ field: 'Data_Redazione', title: this.translocoService.translate('DataRedazione') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }),
            new KendoGridColumn({ field: 'Valutazione_Piano_Des', title: this.translocoService.translate('PianoDeiConti') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }),
            new KendoGridColumn({ field: 'Anno1', title: this.translocoService.translate('AnnoUno') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),
            new KendoGridColumn({ field: 'Anno1_Tipo_Des', title: this.translocoService.translate('AnnoUnoTipo') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }),
            new KendoGridColumn({ field: 'Anno2', title: this.translocoService.translate('AnnoDue') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),
            new KendoGridColumn({ field: 'Anno2_Tipo_Des', title: this.translocoService.translate('AnnoDueTipo') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }),
            new KendoGridColumn({ field: 'Anno3', title: this.translocoService.translate('AnnoTre') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),
            new KendoGridColumn({ field: 'Anno3_Tipo_Des', title: this.translocoService.translate('AnnoTreTipo') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 145 }),
            new KendoGridColumn({ field: 'Note', title: this.translocoService.translate('Note') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 135 }),

        ];

        this.model = {
            Rag_Soc: new ModelEntry(CELL_TYPES.STRING, false),
            Piva: new ModelEntry(CELL_TYPES.STRING, false),
            Id_Testata: new ModelEntry(CELL_TYPES.STRING, false),
            Data_Redazione: new ModelEntry(CELL_TYPES.DATE, false),
            Valutazione_Piano_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Anno1: new ModelEntry(CELL_TYPES.NUMBER, false),
            Anno1_Tipo_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Anno2: new ModelEntry(CELL_TYPES.NUMBER, false),
            Anno2_Tipo_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Anno3: new ModelEntry(CELL_TYPES.NUMBER, false),
            Anno3_Tipo_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Note: new ModelEntry(CELL_TYPES.STRING, false),
            partitaIvaReale: new ModelEntry(CELL_TYPES.STRING, false)
        };

        this.selectable.selectable.checkboxOnly = false;
        this.selectable.selectable.enabled = this.permessoEdit;

        this.toolbar = new ToolbarSettings();
        this.toolbar.newItem = false;
        this.toolbar.resetChanges = false;

        this.cmdColumn = new CommandsColumnSettings({
            editBtn: false,
            removeBtn: this.permessoRemove,
        });

        this.cmdDropDown = new CommandsDropDownSettings({
            fullEditBtn: this.permessoEdit,
            infoBtn: this.permessoInfo,
        });

        // TODO_EXCEL: Giulia 27/09/2023 = l'excel ancora non esiste quindi per il momento è commentato
        this.cmdDropDown.addCommand(new GridCommandItem(
            "DownloadExcel", GriValutazioniActions.DOWNLOAD_EXCEL,
            '', faPlusSquare
        ));


        this.gridPublicSerivce.commandEvent.GiasSubscribe(ev => {
            if (!ev) return;
            switch (ev.command.action) {
                case CommandsDropDownEvents.FULL_EDIT:
                    this.onTemplateBtnClick(ev.dataItem);
                    break;
                case GriValutazioniActions.DOWNLOAD_EXCEL:
                    this.downloadExcel(ev.dataItem);
                    break;
            }
        });

        this.resizable.autoFitColumns = false;
        this.selectable.columnSettings.showSelectAll = true;
        this.selectable.shouldShowCheckbox = false;
        this.selectable.columnSettings.title = ' ';
        this.resizable.isResizable = true;
        this.selectable.selectable.enabled = true;
        this.groups.groupable.enabled = false;
        if (window.innerWidth < SMARTPHONE_WIDTH) {
            this.groups.groupable.enabled = false;
            this.views.enabled = false;
            this.cmdColumn.editBtn = false;
            this.toolbar.newItem = false;
        }


    }

    downloadExcel(dataItem: any) {

        console.log(dataItem);
        this._valutazioni.exportToExcel(dataItem.Piva, dataItem.Id_Testata, "filename.xlsx");
        this.giasMessageService.successMessage(this.translocoService.translate('DownloadExcelSuccess'));

    }

    read(): Observable<ValutazioniKendoServerResult> {
        // azzero Sa_Cod, altrimenti non vengono caricati tutti i centri dell'azienda
        let objParametriAgenda = this.objParametriService.getObjParamValue();
        objParametriAgenda.Sa_Cod = 0;
        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        this.objParametriService.changeObjParametriAgenda(objParametriAgenda);
        return this._valutazioni.leggiValutazioni("").pipe(
            map((data: any[]) => this.transform(data))
        );
    }

    private transform(data: any[]): ValutazioniKendoServerResult {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

        return {
            model: this.model,
            rows: data,
            columns: this.columns
        };
    }

    perform(actionType: HttpAction, items: any): Observable<any> {
        const item = items as any;
        let val_Testata: ValutazioneTestata;
        let objP = this.objParametriService.getObjParamValue();
        objP.Piva = item.Piva;
        switch (actionType) {

            case HttpAction.REMOVE:
                this.loadingService.set_isLoading({ isLoading: true, message: '', component: this.gridPublicService.gridElRef });

                return this._valutazioni.leggiValutazioneObservableForDelete(items).pipe(
                    switchMap((r) => {
                        val_Testata = r[0] as ValutazioneTestata;

                        val_Testata.primaryKey = {
                            Id_Testata: items.Id_Testata,
                            Piva: items.Piva
                        };
                        val_Testata.flag_cancellazione = true;
                        return this._valutazioni.ScriviValutazione(val_Testata).pipe(
                            tap((r) => {
                                this.deleteMessageService.valutazioniDeleteMsg_Succ(r);
                                let objP = this.objParametriService.getObjParamValue();
                                objP.Piva = item.Piva;
                                objP.RagSoc = item.Rag_Soc;
                                this.objParametriService.changeObjParametriAgenda(objP);
                                this.gridPublicService.refresh();

                            }), switchMap(() => {
                                return of([]);
                            }));
                    })
                );

        }
        return this.read() as any;
    }

    onTemplateBtnClick(dataItem) {
        console.log(dataItem);
        console.log("update");
        this.objParametriAgenda = this.objParametriService.getObjParamValue();
        this.objParametriAgenda.Piva = dataItem.Piva;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
        let leggi_Testata = {
            piva: dataItem.Piva,
            idTestata: dataItem.Id_Testata,
            includiAnno: true
        };
        this._valutazioni.changeLeggiTestata(leggi_Testata);
        this.router.navigate(['Valutazioni-Edit'], { relativeTo: this.route });
    }

    onInfoBtnClick(dataItem) {
        console.log(dataItem);
        console.log("read");
        this.objParametriAgenda = this.objParametriService.getObjParamValue();
        const piva = (<string>dataItem.chiave).split('_')[0];
        const sa_cod = (<string>dataItem.chiave).split('_')[1];
        this.objParametriAgenda.Piva = piva;
        this.objParametriAgenda.Sa_Cod = parseInt(sa_cod);
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);

        this.router.navigate(['Valutazioni-Edit'], { relativeTo: this.route });
    }


    override async getRemoveMultipleRowsMessage(opts: RemoveMultipleRowsParams) {
        let msg: string;

        msg = this.translocoService.translate('valutazioni.Delete_Msg');
        opts.data.forEach(r => {
            msg = msg.concat('\n' + r['Rag_Soc']);
        });

        return msg;
    }
}
