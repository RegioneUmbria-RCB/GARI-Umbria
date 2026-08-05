import { Inject, Injectable, Injector, Renderer2 } from '@angular/core';
import { Validators } from '@angular/forms';
import { TranslocoService } from '@jsverse/transloco';
import { DialogCloseResult } from '@progress/kendo-angular-dialog';
import { GiasDialogService } from 'app/Service/gias-dialog.service';
import { GiasMessageService } from 'app/Service/gias-message.service';
import { HttpArgs, HttpService } from 'app/Service/http.service';
import { LOADING_TOKEN, LoadingService } from 'gias-ui-kit';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { DettagliColumnSettings } from 'gias-kendo-grid';
import { DropdownListItem, DropdownListWithForm, EditingMode, KendoGridColumn, LoaderType, RendererGridEvent } from 'gias-kendo-grid';
import { AbstractGridConfigService, HttpAction } from 'gias-kendo-grid';
import { UtilityFunctions } from 'app/Utility/UtilityFunctions';
import { map, Observable, of, switchMap } from 'rxjs';
import { EditGruppiMerceService } from '../editGruppiMerce.service';
import { EditGrMerceLinks as links, GruppiMerceGridModel, GruppiMerceGridRow, GruppiMerceModel, GruppiMerceRows, GruppiMerceServerResult, InsertionErrors, PublicServices, RisultatoCancellazione, USE_MOCK_DATA } from '../utils';
import { AjaxAgronicaAPIService } from 'app/Service/ajax-agronica.api.service';



@Injectable()
export class EditGruppiMerceGridService extends AbstractGridConfigService<GruppiMerceServerResult> {
    editingMode: EditingMode = EditingMode.IN_LINE;
    loader: LoaderType = LoaderType.SERVICE;
    rowId: string = 'chiave';
    gridId: string = 'GruppiMerceGridService'

    protected columns: KendoGridColumn[] = [
        new KendoGridColumn({ field: 'RagioneSociale', title: this.transloco.translate('Impresa') }, { editable: false }),
        new KendoGridColumn({ field: 'Sa_Cod', title: this.transloco.translate('Visibilità') }),
        new KendoGridColumn({ field: 'Codice', title: this.transloco.translate('Codice') }, { validators: [Validators.required] }),
        new KendoGridColumn({ field: 'Descrizione', title: this.transloco.translate('Descrizione') }),
        new KendoGridColumn({ field: 'Data_Creazione', title: this.transloco.translate('DataCreazione')}, { editable: false }),
        new KendoGridColumn({ field: 'Data_Modifica', title: this.transloco.translate('DataModifica')}, { editable: false }),
        new KendoGridColumn({ field: 'Username_Creazione', title: this.transloco.translate('UtenteCreazione') }, { editable: false }),
        new KendoGridColumn({ field: 'Username_Modifica', title: this.transloco.translate('UtenteModiica') }, { editable: false }),
    ];
    protected model: GruppiMerceGridModel = GruppiMerceModel;
    private rows: GruppiMerceGridRow[] = GruppiMerceRows;

    private pivaCorrente: string;

    constructor(
        injector: Injector,
        private httpClient: HttpService,
        private masterPageService: EditGruppiMerceService,
        @Inject(LOADING_TOKEN) private loading: LoadingService,
        private messages: GiasMessageService,
        private giasDialogService: GiasDialogService,
        private renderer: Renderer2,
        protected transloco: TranslocoService,
        agenda: ObjParametriAgendaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService) {
        super(injector);

        this.pivaCorrente = agenda.getObjParamValue().Piva;
        this.initializeGridBehavior();
        this.manageDDLColumns();
        this.masterPageService.initializePubService(PublicServices.EditGruppiMerce, this.gridPublicService);
    }


    initializeGridBehavior() {
        this.selectable.selectable.enabled = false
        this.selectable.shouldShowCheckbox = false;

        this.cmdColumn.removeBtn = true;
        this.cmdColumn.editBtn = true;
        this.cmdColumn.infoBtn = false;

        this.resizable.autoFitColumns = true;

        this.dettagliColumn = new DettagliColumnSettings({
            editBtn: false
        });

        this.toolbar.newItem = true;
    }

    manageDDLColumns() {
        let col = this.columns.find((c) => c.field === 'Sa_Cod');
        col.ddl = new DropdownListWithForm('id', 'Sa_Cod', 'name', []);
        col.ddl.valuePrimitive = true;
        col.ddl.loadOnEdit = true;
        col.ddl.descriptionField = 'Sa_Cod_Desc';
        col.ddl.defaultValue = new DropdownListItem(0, "Privato");
        col.ddl.loadFunction = this.caricaVisibilita.bind(this);
    }

    caricaVisibilita() {
        return of([new DropdownListItem(0, "Privato"), new DropdownListItem(-1, "Pubblico")]);
    }

    read(options?: any): Observable<GruppiMerceServerResult> {
        if (USE_MOCK_DATA)
            return of(new GruppiMerceServerResult(this.model, this.columns, this.rows));


        this.isLoading(true);
        const httpFn = (...args: HttpArgs) => {
            const agendaService = args[0];
            const master = args[1];

            let agenda = agendaService.getObjParamValue();

            return {
                piva: agenda.Piva,
                objParam_server: master.ObjParametri_Server,
                objParam_utenti: master.ObjParametri_Utenti
            };
        }
        
        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(links.CARICA_MERCE_LINK, httpFn).pipe(map(resp => {
            let rows: GruppiMerceGridRow[] = resp.RispostaStringa;
            this.isLoading(false);

            rows.forEach((row) => {
                if (row.Sa_Cod === 0)
                    row.Sa_Cod_Desc = 'Privato';
                else
                    row.Sa_Cod_Desc = 'Pubblico';
            })

            return new GruppiMerceServerResult(this.model, this.columns, rows);
        }))

    }

    isLoading(onoff: boolean) {
        this.loading.set_isLoading({ isLoading: onoff, component: this.gridPublicService.gridElRef });
    }

    perform(actionType: HttpAction, item: any): Observable<any[]> {
        if (actionType == HttpAction.REMOVE) {
            if(!item.Cancellabile)
                return of([]);

            return this.TentativoCancellazioneGruppoMerce(item, false);
        }
        if(actionType == HttpAction.CREATE) {
            return this.CreaOAggiornaGruppoMerce(item);
        }

        if(actionType == HttpAction.UPDATE) {
            if(pulsanteModificaNascosto(item, this.pivaCorrente))
                return of([]);

            return this.CreaOAggiornaGruppoMerce(item);
        }

        return of([]);
    }



    TentativoCancellazioneGruppoMerce(gruppo: GruppiMerceGridModel, cancellazionePermessi: boolean): Observable<any[]> {
        const params = (...args: HttpArgs) => {
            const master = args[1];

            return {
                IdGruppoMerce: gruppo.Id_Gruppo_Merce,
                forzaCancellazione: cancellazionePermessi,
                objParam_server: master.ObjParametri_Server,
                objParam_utenti: master.ObjParametri_Utenti
            };
        }

        return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(links.TENTATIVO_CANCELLAZIONE_GRUPPO_MERCE, {
            IdGruppoMerce: gruppo.Id_Gruppo_Merce,
            forzaCancellazione: cancellazionePermessi }).pipe(map(resp => {
                let risp = resp.RispostaStringa;
                if (risp.Risultato == RisultatoCancellazione.SUCCESS) {
                    this.messages.infoMessagge("grMerci.CancellazioneSuccess", false, true);
                    return [];
                }

                if (risp.Risultato == RisultatoCancellazione.RICHIEDE_CONFERMA_CANCELLAZIONE) {
                    this.showDialogConfermaCancellazione(gruppo, risp.NumberConflicts).pipe(map(r => {
                        return r
                    })).subscribe();
                }

                if (risp.Risultato == RisultatoCancellazione.FAILURE) {
                    this.messages.errorMessage("grMerci.Errore", false, true);
                    return [];
                }

                throw Error("Strangely something went completely wrong...")
            }))
    }

    CreaOAggiornaGruppoMerce(item: GruppiMerceGridRow): Observable<any[]> {
        const params = (...args: HttpArgs) => {
            const agendaService = args[0];
            const master = args[1];

            let agenda = agendaService.getObjParamValue();

            item.Piva = agenda.Piva;

            return {
                gruppoMerce: item,
                objParam_server: master.ObjParametri_Server,
                objParam_utenti: master.ObjParametri_Utenti
            };
        }

        return this.ajaxAgronicaAPIService.ajaxAPIPost<any,any>(links.AGGIUNGI_MODIFICA_GRUPPO_MERCE, item).pipe(map(resp => {
            
            if (eCodiceDuplicatoPrivato(resp.Errore, item)) {
                this.messages.errorMessage("grMerci.CodiceDuplicatoPrivato", true, true, [ item.Codice ]);
                return [];
            }

            if (eCodiceDuplicatoPubblico(resp.Errore, item)) {
                this.messages.errorMessage("grMerci.CodiceDuplicatoPubblico", true, true, [ item.Codice ]);
                return [];
            }

            if (resp.Errore == InsertionErrors.GruppoUtilizzatoDaUnProdottoExtra) {
                this.messages.errorMessage("grMerci.GruppoUtilizzatoDaUnProdottoExtra", true, true);
                return [];
            }

            if (resp.Errore == InsertionErrors.UtilizzatoDaUnAltraImpresaComeGruppoMerceDefault) {
                this.messages.errorMessage("grMerci.UtilizzatoDaUnAltraImpresaComeGruppoMerceDefault", true, true);
                return [];
            }

            if (resp.RispostaOK)
                this.messages.infoMessagge("grMerci.Success", false, true);
            return [];
        }))

    }

    showDialogConfermaCancellazione(gruppo: GruppiMerceGridModel, numOfConflicts: number) {
        const title = this.transloco.translate('grMerci.ConfermaTitle');
        const messaggio = numOfConflicts == 1 ? this.transloco.translate('grMerci.CancellazionePermesso') :
            this.transloco.translate('grMerci.CancellazionePermessi', [numOfConflicts]);


        return this.giasDialogService.dialogMessageObs_Result(
            title,
            messaggio,
            null,
            undefined,
            undefined,
            e => e instanceof DialogCloseResult
        ).pipe(
            switchMap((dialogRes: any) => {
                let confermato = dialogRes.returnObj;
                if (confermato) {
                    return this.TentativoCancellazioneGruppoMerce(gruppo, true);
                }
                return;
            })
        );
    }



    override applyRendererRules(opts: RendererGridEvent): void {
        this.nascondiPulsanteModifica(opts);
        this.nascondiPulsanteCancellazione(opts);
    }

    /**
     * Le regole:
     * Modificabile solo se privato
     *  pubblico è -1
     *  privato è 0
     * Notes: pubblico può essere modificabile solo dalla piva che l'ha creato.
     */
    nascondiPulsanteModifica(opts: RendererGridEvent) {
        const { grid, gridElRef } = { ...opts };
        let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
        grid.view.forEach((el, index) => {
            let nascosto = pulsanteModificaNascosto(el, this.pivaCorrente);
            if(nascosto)
                UtilityFunctions.setStyle(this.renderer, visibleRows[index], '.editBtn', 'display', 'none');
            else
                UtilityFunctions.setStyle(this.renderer, visibleRows[index], '.editBtn', 'display', 'block');
        });
    }

    /**
     * Le regole:
     * Cancellabile a secondo delle seguente verifiche:
     * a. Controllare Prodotti_Extra_Privata
     * b. Controllare gruppi merce default (Imprese_Impostazioni Where Impostazione_Cod = 1064)
     * Gli elementi per essere cancellabili non devono trovarsi in a) oppure b).
     * Nota: Quando provo a cancellare un elemento viene fatta un'ulteriore verifica nella tabella
     *       GruppiMercePerGruppiUtenti, gli elementi cui veranno cancellati se l'utente lo desidera.
     */
    nascondiPulsanteCancellazione(opts: RendererGridEvent) {
        const { grid, gridElRef } = { ...opts };
        let visibleRows: [] = gridElRef.nativeElement.querySelectorAll('tbody tr');
        grid.view.forEach((el, index) => {
            if (el.Cancellabile)
                UtilityFunctions.setStyle(this.renderer, visibleRows[index], '#btnDelete', 'display', 'block');
            else
                UtilityFunctions.setStyle(this.renderer, visibleRows[index], '#btnDelete', 'display', 'none');
        });
    }

}

const PUBBLICO = -1;
const PRIVATO = 0;

const aziendaDiCreazione = (pivaCreazione, pivaCorrente) => pivaCreazione === pivaCorrente;

const pulsanteModificaNascosto = (riga: any, pivaCorrente: string) => {
    if (riga.Sa_Cod === PUBBLICO) {
        if (aziendaDiCreazione(riga.Piva, pivaCorrente))
            return false;
        else
            return true;
    }
    else if(riga.Sa_Cod === PRIVATO)
        return false
}

const eCodiceDuplicatoPrivato = (errorCode, riga) =>
    errorCode == InsertionErrors.CodiceDuplicato && accessProperty(riga.Sa_Cod) == PRIVATO;

const eCodiceDuplicatoPubblico = (errorCode, riga) =>
    errorCode == InsertionErrors.CodiceDuplicato && accessProperty(riga.Sa_Cod) == PUBBLICO;

const accessProperty = (property) => {
    if(property instanceof DropdownListItem)
        return property.id;
    return property;
}


