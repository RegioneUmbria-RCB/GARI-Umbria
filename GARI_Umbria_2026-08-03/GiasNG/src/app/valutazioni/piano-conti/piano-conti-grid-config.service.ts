import { Injectable, Injector } from '@angular/core';
import { CommandsColumnSettings, CommandsDropDownEvents, CommandsDropDownSettings, ToolbarSettings } from 'gias-kendo-grid';
import {  EditingMode, GridCustomizations, KendoGridColumn, LoaderType, ModelEntry } from 'gias-kendo-grid';
import { CELL_TYPES } from 'gias-ui-kit';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { Observable, from, of } from 'rxjs';
import { ConfigTemplate } from 'gias-kendo-grid';
import { map, switchMap, take, tap } from 'rxjs/operators';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Security_Attivita } from 'app/Model/TipiEnumerativi';
import { SMARTPHONE_WIDTH } from 'app/Model/CostantiPersonalizzate';
import { TranslocoService } from '@jsverse/transloco';
import { ValutazioneTestata } from 'app/Model/valutazioni/ValutazioneTestata';
import { DeleteMessageService } from 'app/Service/delete-message.service';
import { PianoContiKendoServerResult, PianoContiModel } from './piano-conti.models';
import { PianoContiService } from './service/pianoconti.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { DialogResult } from '@progress/kendo-angular-dialog';
import { PKPianoConti, PianoConti } from 'app/Model/valutazioni/PianoConti';
import { ObjParametriAgenda } from 'gias-ui-kit';



@Injectable()
export class PianoContiHttpService extends AbstractGridConfigService<PianoContiKendoServerResult> {
    gridId = 'PianoContiHttpService';
    loader: LoaderType = LoaderType.SERVICE;
    editingMode: EditingMode = EditingMode.IN_LINE;
    rowId = 'Valutazione_Piano_Cod';
    objParametriAgenda: ObjParametriAgenda;

    /** Optional parameters */
    toolbar = new ToolbarSettings(true, false);
    views = new GridCustomizations({ enabled: true });

    public permessoEdit: boolean;
    public permessoRemove: boolean;
    public permessoInfo: boolean;


    columns: KendoGridColumn[];
    model: PianoContiModel;
    comuneColumn: KendoGridColumn;

    constructor(injector: Injector,
        private objParametriService: ObjParametriAgendaService,
        private permessiUtenteService: PermessiUtenteService,
        private router: Router,
        private route: ActivatedRoute,
        private translocoService: TranslocoService,
        private _pianoConti: PianoContiService,
        private deleteMessageService: DeleteMessageService,
        private giasMessageService: GiasMessageService,
        private giasDialogService: GiasDialogService
    ) {
        super(injector, ConfigTemplate.DefaultTemplate);

        this.permessoEdit = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);
        this.permessoRemove = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 2);
        this.permessoInfo = this.permessiUtenteService.getPermesso(enum_Security_Attivita.Valutazioni_Rischio, 0);



        this.columns = [
            new KendoGridColumn({ field: 'Rag_Soc', title: this.translocoService.translate('Ragione_Sociale1') }, { resizable: true, editable: this.permessoEdit, width: 175 }),
            new KendoGridColumn({ field: 'partitaIvaReale', title: this.translocoService.translate('PartitaIVA') }, { resizable: true, editable: this.permessoEdit, hidden: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 120 }),
            new KendoGridColumn({ field: 'Valutazione_Piano_Des', title: this.translocoService.translate('Descrizione') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),
            new KendoGridColumn({ field: 'Valutazione_Piano_Cod', title: this.translocoService.translate('CodicePiano') }, { resizable: true, editable: this.permessoEdit, hidden: true, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),
            new KendoGridColumn({ field: 'Num_Valutazioni_Associate', title: this.translocoService.translate('Numero_Valutazioni_Associato') }, { resizable: true, editable: this.permessoEdit, media: '(min-width: ' + SMARTPHONE_WIDTH + 'px)', width: 115 }),

        ];

        this.model = {
            Rag_Soc: new ModelEntry(CELL_TYPES.STRING, false),
            Piva: new ModelEntry(CELL_TYPES.STRING, false),
            partitaIvaReale: new ModelEntry(CELL_TYPES.STRING, false),
            Valutazione_Piano_Des: new ModelEntry(CELL_TYPES.STRING, false),
            Valutazione_Piano_Cod: new ModelEntry(CELL_TYPES.STRING, false),
            Num_Valutazioni_Associate: new ModelEntry(CELL_TYPES.STRING, false),
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
        this.gridPublicService.commandEvent.GiasSubscribe(ev => {
            if (!ev) return;
            switch (ev.command.action) {
                case CommandsDropDownEvents.FULL_EDIT:
                    this.onTemplateBtnClick(ev.dataItem);
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

        this.gridPublicService.changeDetected.GiasSubscribe((event: any) => {

            console.log("remove");
            console.log(event);
            if (event?.action === 'remove') {
                let dataItem: any = event.dataItem;
                if (dataItem != undefined) {
                    if (dataItem.Num_Valutazioni_Associate != 0) {
                        this.giasMessageService.errorMessage(this.translocoService.translate("PianoConti.Delete_MsgNO"));
                        return;
                    }
                    this.EliminaPianoConti(dataItem);
                }
            }

        });


    }

    EliminaPianoConti(dataItem: any): any {
        const promise = new Promise((resolve, reject) => {
            this.giasDialogService.baseWarning(
                '', this.translocoService.translate('PianoConti.Delete_Msg') + " " + dataItem.Rag_Soc, false
            ).then((resp: DialogResult) => {
                if (!resp['returnObj']) return;

                let pk: PKPianoConti = {
                    Piva: dataItem.Piva,
                    Valutazione_Piano_Cod: +dataItem.Valutazione_Piano_Cod
                };

                let currentObject: PianoConti = {
                    primaryKey: pk,
                    Valutazione_Piano_Des: dataItem.Valutazione_Piano_Des,
                    flag_cancellazione: false
                };


                return this._pianoConti.scriviPianoConti(currentObject, true)
                    .pipe(take(1), tap((val) => { resolve(val); }))
                    .subscribe();
            });
        });

        const myObservable = from(promise);

        myObservable.pipe(switchMap(() => {
            this.giasMessageService.infoMessagge(this.translocoService.translate("PianoConti.Delete"));
            this.gridPublicService.refresh(true);
            return of([]);
        })).subscribe();


    }



    read(): Observable<PianoContiKendoServerResult> {
        // azzero Sa_Cod, altrimenti non vengono caricati tutti i centri dell'azienda
        let objParametriAgenda = this.objParametriService.getObjParamValue();
        objParametriAgenda.Sa_Cod = 0;
        objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Read;
        this.objParametriService.changeObjParametriAgenda(objParametriAgenda);
        return this._pianoConti.leggiPianoConti("", 0).pipe(
            map((data: any[]) => this.transform(data))
        );
    }

    private transform(data: any[]): PianoContiKendoServerResult {
        this.loadingService.set_isLoading({ isLoading: false, message: '', component: this.gridPublicService.gridElRef });

        console.log(data);
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

                /*  return this._valutazioni.leggiValutazioneObservableForDelete(items).pipe(
                      switchMap((r) => {
                          val_Testata = r[0] as ValutazioneTestata;

                          val_Testata.primaryKey = {
                              Id_Testata: items.Id_Testata,
                              Piva: items.Piva
                          }
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
                              }))
                      })
                  );*/
                break;

        }
        return this.read() as any;
    }

    onTemplateBtnClick(dataItem) {
        console.log("update");
        this.objParametriAgenda = this.objParametriService.getObjParamValue();
        this.objParametriAgenda.Piva = dataItem.Piva;
        this.objParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;
        this.objParametriService.changeObjParametriAgenda(this.objParametriAgenda);
        let leggi_Piano = {
            piva: dataItem.Piva,
            pianoCod: dataItem.Valutazione_Piano_Cod
        };
        this._pianoConti.changePianoConti(leggi_Piano);
        this._pianoConti.changeValAssociato(dataItem.Num_Valutazioni_Associate);
        this.router.navigate(['PianoConti-Edit'], { relativeTo: this.route });
    }
}
