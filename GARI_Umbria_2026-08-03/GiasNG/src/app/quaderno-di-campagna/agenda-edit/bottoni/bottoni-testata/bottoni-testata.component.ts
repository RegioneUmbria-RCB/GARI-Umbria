import { AfterViewInit, Component, OnInit, ViewChild, TemplateRef } from '@angular/core';
import { Attivita } from 'app/Model/attivita/Attivita';
import { Lavorazione } from 'app/Model/attivita/Lavorazione';
import { Enum_SiteRedirector, enum_PagineAgenda_2010 } from 'app/Model/siti.enum';
import { enum_LAVCOD } from 'app/Model/TipiEnumerativi';
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService } from 'app/Service/obj-parametri-agenda.service';
import { QdCFormToAttivitaService } from '../../service/quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service';
import { QdCVisibilitaControlliTestataService } from "../../service/testata/visibilita-controlli-testata-service";
import { QdCService } from "../../service/qdc.service";
import { DpiBio, FORMULATI, NessunDpi, NessunDpiNessunaEtichetta } from "app/Model/CostantiPersonalizzate";
import { QdCControlliSalvataggioService } from "../../service/qdc-controlli-salvataggio.service";
import {
    Key_Parametri_Aggiuntivi, Obj_Errore_Gias_QdC,
    Parametri_Aggiuntivi_Attivita,
    Sezione_Prodotto_Fertilizzanti,
    Sezione_Prodotto_Formulati, Sezione_Prodotto_Raccolta, Sezione_Prodotto_Sementi
} from "../../quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import { enum_ErroreGias_Tipo, ErroreGias, ErroreGias_Severity } from "app/Service/master.service";
import { TranslocoService } from "@jsverse/transloco";
import { cloneDeep } from "lodash";
import { ObjParametriAgenda, GiasWindowsService } from 'gias-ui-kit';
import { PermessiUtenteService } from 'app/Service/permessi-utente.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { from, Observable, map, switchMap, tap, filter, take, of, catchError } from 'rxjs';
import { AgendaService, Attivita_Con_Parametri_Aggiuntivi, ScriviListaAttivita } from 'app/Service/Agenda/Agenda.service';
import { VerificaDisciplinariService, ResultDataSource } from 'app/qualita-tracciabilita/services/verifica-disciplinari.service';
import { AnalysisRequestDataItem } from 'app/qualita-tracciabilita/models/richiesta-analisi.model';
import { GiasDialogService, Dialog_Type } from 'app/Service/gias-dialog.service';

@Component({
    standalone: false,
    selector: 'btn-bottoni-testata',
    templateUrl: './bottoni-testata.component.html',
    styleUrls: ['./bottoni-testata.component.scss'],
})
export class BottoniTestataComponent implements OnInit, AfterViewInit {
    @ViewChild('aDetailsRef') public aDetailsRef: TemplateRef<any>;
    objParametriAgenda: ObjParametriAgenda;

    protected _error: string;
    /** 1 = warning */
    protected _errorSeverity: number;
    protected _idTestata: number;

    constructor(
        public qdcservice: QdCService,
        public agenda: AgendaService,
        public QdCHeaderVisibility: QdCVisibilitaControlliTestataService,
        private QdCSaveChecksService: QdCControlliSalvataggioService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private qdCFormToAttivitaService: QdCFormToAttivitaService,
        private gestioneRichieste: GestioneRichiesteService,
        private QdCSaveChecks: QdCControlliSalvataggioService,
        private compliance: VerificaDisciplinariService,
        private translocoService: TranslocoService,
        private windowService: GiasWindowsService,
        private dialog: GiasDialogService,
        private userSettings: PermessiUtenteService,
    ) { }

    ngOnInit(): void {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    }

    ngAfterViewInit() {
        this.Msg_Verifica_Conformita_Intervento_Ribaltamento();
    }

    // #region Dose
    redirectTo_Verifica_DoseConsigliataBS() {
        //Mostrato solo per il diserbo e passato solo il diserbo
        if (this.qdcservice.Sezioni_ProdottoFormArray
            && this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().length > 0
            && !this.hasErrorsDoseConsigliata()
        ) {
            this.redirectDoseConsigliataIFrame();
        }
    }

    private hasErrorsDoseConsigliata() {
        let listErroriGias: ErroreGias[] = [];

        //Deve essere scelto un disciplinare diverso da bio,NessunDpi e NessunDPINessunaEtichetta
        let disciplinare = this.qdcservice.getDisciplinareModelValue(enum_LAVCOD.DISERBO);
        if (!disciplinare || disciplinare.codice === NessunDpi
            || disciplinare.codice === NessunDpiNessunaEtichetta || disciplinare.codice === DpiBio
        ) {
            let DisciplinariDesc = this.qdcservice.Obj_DpiBIO.descrizione + " , " + this.qdcservice.Obj_NessunDpi.descrizione + " , " + this.qdcservice.Obj_NessunDpiNessunaEtichetta.descrizione;
            listErroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Bloccante,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate("qdc.ScegliereUnDisciplinareDiversoDa", { DisciplinariDesc })
            });
            if (listErroriGias && listErroriGias.length > 0) {
                this.qdcservice.gestisci_ErroriGias(listErroriGias, true, true).then();
                return true;
            }
        }

        this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().forEach(
            (s: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta) => {
                let lav_cod = + s.Operazione.primaryKey.codice;
                if (lav_cod === enum_LAVCOD.DISERBO) {
                    this.QdCSaveChecks.Controlla_Grid_Dosi_Prodotti(s.Categoria_Magazzino, lav_cod, s.DosiProdotti, listErroriGias, false);
                }
            }
        );
        if (listErroriGias && listErroriGias.length > 0) {
            this.qdcservice.gestisci_ErroriGias(listErroriGias, true, true).then();
            return true;
        }
        return false;
    }

    private redirectDoseConsigliataIFrame() {
        const attivita: Attivita = cloneDeep(this.qdCFormToAttivitaService.mapQdCFormToListaAttivita(null, [])
            .find(a => +a.job.primaryKey.codice === enum_LAVCOD.DISERBO));
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        const newobjParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgenda);
        newobjParametriAgenda.Id_Agenda = 0;
        newobjParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();;
        newobjParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
        newobjParametriAgenda.GenericObj_string = JSON.stringify(attivita);

        const parametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];
        this.gestioneRichieste.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_Verifica_DoseConsigliataBS,
            parametriAggiuntivi,
            newobjParametriAgenda,
            false,
            -1,
            'qdc.DettaglioDoseConsigliataDisciplinare.Text'
        ).then();
    }
    // #endregion

    // #region Sustainability
    redirectTo_Verifica_Sostenibilita() {
        if (this.qdcservice.Sezioni_ProdottoFormArray
            && this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().length > 0
            && !this.hasErrorsSostenibilita()
        ) {
            this.redirectSostenibilitaIFrame();
        }
    }

    private hasErrorsSostenibilita() {
        let listErroriGias: ErroreGias[] = [];
        this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().forEach(
            (s: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta) => {
                let lav_cod = + s.Operazione.primaryKey.codice;

                this.QdCSaveChecks.Controlla_Grid_Dosi_Prodotti(s.Categoria_Magazzino, lav_cod, s.DosiProdotti, listErroriGias, false);
            }
        );
        if (listErroriGias && listErroriGias.length > 0) {
            this.qdcservice.gestisci_ErroriGias(listErroriGias, true, true).then();
            return true;
        }
        return false;
    }

    private getListaAttivitaOrdinate(): Attivita[] {
        let lista_Attivita_Ordinate: Attivita[] = [];
        const lista_Attivita: Attivita[] = cloneDeep(this.qdCFormToAttivitaService.mapQdCFormToListaAttivita(null, []));

        //Per il lato server prima ordino la lista per i FORMULATI poi le altre attivita
        let Attivita_Formulati = lista_Attivita.filter(a => this.qdcservice.getCategoria_Magazzino(+ a.job.primaryKey.codice) === FORMULATI);
        if (Attivita_Formulati && Attivita_Formulati.length > 0) {
            Attivita_Formulati.forEach(a => lista_Attivita_Ordinate.push(a));
        }

        let Attivita_Non_Formulati = lista_Attivita.filter(a => this.qdcservice.getCategoria_Magazzino(+ a.job.primaryKey.codice) !== FORMULATI);
        if (Attivita_Non_Formulati && Attivita_Non_Formulati.length > 0) {
            Attivita_Non_Formulati.forEach(a => lista_Attivita_Ordinate.push(a));
        }

        return lista_Attivita_Ordinate;
    }

    private redirectSostenibilitaIFrame() {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        let newobjParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgenda);
        newobjParametriAgenda.Id_Agenda = 0;
        newobjParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();
        newobjParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
        newobjParametriAgenda.GenericObj_string = JSON.stringify(this.getListaAttivitaOrdinate());

        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];
        this.gestioneRichieste.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            enum_PagineAgenda_2010.Pagina_reportSostenibilita,
            ParametriAggiuntivi,
            newobjParametriAgenda,
            false,
            -1,
            'qdc.DoseConsigliata'
        ).then();
    }

    // #endregion
    //  #region Compliance
    redirectTo_Verifica_Conformita() {
        if (this.qdcservice.Sezioni_ProdottoFormArray && this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().length > 0) {
            let list_lav_cod_da_non_mappare: number[] = [];
            let listErroriGias: ErroreGias[] = [];
            this.valorizeOperationChecksLists(list_lav_cod_da_non_mappare, listErroriGias);

            if (listErroriGias && listErroriGias.length > 0) {
                this.qdcservice.gestisci_ErroriGias(listErroriGias, true, true).then();
                return;
            }

            let complianceMode = this.userSettings.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_ModalitaVerificaConformita);
            if (complianceMode && complianceMode.Valore === '2') {
                this.checkCompliance(); // new compliance flow
            } else {
                this.openOldComplianceInIFrame(list_lav_cod_da_non_mappare);
            }
        }
    }

    /**
     * Se ho una multiOperazione per alcuni lav_cod non devo fare il VerificaConformità
     * esempio MultiOperazione Semina + Distribuzione concime, devo escludere la Semina tra le Operazioni
     * da passare al VerificaConformita
     */
    private valorizeOperationChecksLists(list_lav_cod_da_non_mappare: number[], listErroriGias: ErroreGias[]) {
        this.qdcservice.Sezioni_ProdottoFormArray.getRawValue().forEach(
            (s: Sezione_Prodotto_Formulati | Sezione_Prodotto_Fertilizzanti | Sezione_Prodotto_Sementi | Sezione_Prodotto_Raccolta) => {
                let lav_cod = + s.Operazione.primaryKey.codice;
                if (this.QdCHeaderVisibility.MostraBtn_VerificaConformita([s.Operazione])) {
                    this.QdCSaveChecks.Controlla_Grid_Dosi_Prodotti(
                        s.Categoria_Magazzino, lav_cod, s.DosiProdotti, listErroriGias, false);
                } else {
                    list_lav_cod_da_non_mappare.push(lav_cod);
                }
            }
        );
    }

    private openOldComplianceInIFrame(list_lav_cod_da_non_mappare: number[]) {
        let listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[] = [];
        let list_attivita: Attivita[] = cloneDeep(this.qdCFormToAttivitaService
            .mapQdCFormToListaAttivita(null, listParametri_Aggiuntivi_Attivita, false, [], list_lav_cod_da_non_mappare));

        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
        let newobjParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgenda);
        newobjParametriAgenda.Id_Agenda = 0;
        newobjParametriAgenda.Pagina_Provenienza = this.qdcservice.masterService.getCurrentPageAsValue();
        newobjParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
        newobjParametriAgenda.GenericObj_string = JSON.stringify(list_attivita);

        //Se sono in modifica passo anche la lista_Codici_Attivita_x_CentriAziendali
        //perchè il Verifica Conformita ha bisogno di tutti gli id_agenda
        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];
        let Parametri_Aggiuntivi_MultiCentro = listParametri_Aggiuntivi_Attivita
            .find(p => p.key === Key_Parametri_Aggiuntivi.lista_Codici_Attivita_x_CentriAziendali);

        if (Parametri_Aggiuntivi_MultiCentro) {
            ParametriAggiuntivi.push({
                key: Parametri_Aggiuntivi_MultiCentro.key,
                value: Parametri_Aggiuntivi_MultiCentro.value,
                codifica: false
            });
        }

        this.gestioneRichieste.gestionePassaggioAltroSito_Aperto_in_Iframe(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Verifica_Conformita,
            ParametriAggiuntivi, newobjParametriAgenda, false, -1, 'qdc.ConformitaOperazione'
        ).then();
    }

    private checkCompliance() {
        // Eseguo i controlli pre-salvataggio
        from(this.QdCSaveChecksService.ControllaQdC()).pipe(
            filter((erroriGias: ErroreGias[]) => {
                const error = erroriGias?.map(x => x.messaggio).join(', ') ?? "";
                if (error != "") {
                    this.dialog.baseError(this.translocoService.translate('Errore_'), error);
                }
                return error == "";
            }),
            switchMap(() => {
                let additionalParams: Parametri_Aggiuntivi_Attivita[] = [];
                let listaAttivita = this.qdCFormToAttivitaService.mapQdCFormToListaAttivita(null, additionalParams, false);
                if (listaAttivita && listaAttivita.length > 0) {
                    this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita as Attivita_Con_Parametri_Aggiuntivi[]);
                    return this.CheckQdCCompliance(listaAttivita, additionalParams);
                }
                return of(-1);
            }),
            tap(idTestata => {
                if (idTestata > 0) {
                    this._idTestata = idTestata;
                }
            })
        ).subscribe((idTestata: number) => this.showComplianceErrors(idTestata));
    }

    /**
     * Executes compliance checks as the ones done before saving.
     * 
     * Se eseguiVerificheConformita è true allora vengono mostrati i messaggi di warning in cui l'utente deve scegliere
     * sì/no altrimenti se eseguiVerificheConformita è false allora vengono bypassati i messaggi di warning
     *
     * @param activities - Array of Attivita objects containing the activities to be checked
     * @param additionalParams - Array of Parametri_Aggiuntivi_Attivita objects containing additional parameters for the activities
     *
     * @returns Observable<number> Returns an Observable that emits:
     * - The IdTestata from the compliance response if successful
     * - -1 if the response parameter is undefined
     */
    private CheckQdCCompliance(activities: Attivita[], additionalParams: Parametri_Aggiuntivi_Attivita[]): Observable<number> {
        const scriviListaAttivita = <ScriviListaAttivita>{
            attivita_list: activities,
            parametri_aggiuntivi_list: additionalParams
        };
        return from(this.agenda.CheckActivityCompliance(scriviListaAttivita)).pipe(
            switchMap(rispostaStandard => {
                if (rispostaStandard.ParametroDue_stringa != undefined && rispostaStandard.ParametroDue_stringa != "") {
                    const compliance = JSON.parse(rispostaStandard.ParametroDue_stringa);
                    if (compliance.ComplianceResponse?.Error != "") {
                        this._error = compliance.ComplianceResponse?.Error;
                        return of(-1);
                    }
                    if (compliance.Timeout && compliance.ComplianceResponse?.IdTestata > 0) {
                        this._idTestata = compliance.ComplianceResponse?.IdTestata;
                        return this.showComplianceTimeoutPrompt(rispostaStandard.ErroriGias, activities, additionalParams);
                    }
                    return of(compliance.ComplianceResponse?.IdTestata ?? -1);
                }
                return of(-1);
            }),
            tap(id => this._idTestata = id),
            catchError(() => {
                this.qdcservice.masterService.set_isLoading({ isLoading: false });
                return of(-1);
            })
        );
    }

    private showComplianceTimeoutPrompt(listErroriGias: ErroreGias[], activities: Attivita[], additionalParams: Parametri_Aggiuntivi_Attivita[]) {
        let content: string = listErroriGias.filter(e => e.severity === ErroreGias_Severity.Warning)
            .map(x => x.messaggio).join(". ") ?? "";
        let actions = [
            { text: this.translocoService.translate('Annulla'), returnObj: false },
            { text: this.translocoService.translate('ContinuaLAttesa'), returnObj: true, primary: true },
        ];
        return this.dialog.dialogMessageObs_Result("", content, actions, "auto", "auto", null, Dialog_Type.info).pipe(
            take(2),
            switchMap(result => {
                if (result['returnObj']) {
                    if (!additionalParams.find(x => x.key == "IdTestataVerificaConformita")) {
                        additionalParams.push({
                            key: "IdTestataVerificaConformita",
                            value: this._idTestata.toString(),
                            operazione: null
                        } as Parametri_Aggiuntivi_Attivita);
                    }
                    return this.CheckQdCCompliance(activities, additionalParams);
                }
                return of(-1);
            })
        );
    }

    private showComplianceErrors(idTestata: number) {
        this.qdcservice.masterService.set_isLoading({ isLoading: false });
        if (this._error != null && this._error != '') {
            this.dialog.baseError(this.translocoService.translate('Errore_'), this._error);
        } else if (idTestata > 0) {
            this.showComplianceResults(idTestata);
        } else {
            console.error("Something went wrong during compliance check, but no error message was provided.");
        }
    }

    private showComplianceResults(idTestata: number) {
        this.compliance.analysisDataItem = new AnalysisRequestDataItem();
        this.compliance.analysisDataItem.IdTestata = idTestata;
        this.compliance.analysisDataItem['source'] = ResultDataSource.FromTable;
        this.compliance.readAnalysisResults().subscribe(() => {
            this.windowService.open({
                title: this.translocoService.translate("RisultatoAnalisiConformita"),
                content: this.aDetailsRef,
                height: window.innerHeight * 0.9,
                width: window.innerWidth * 0.9,
            }, false);
        });
    }

    // #end region
    // #region Ribaltamento
    /*
        @description
        Warning Sì/No che viene mostrato quando si sta ribaltando una Ricetta/Brogliaccio creta da APP in Agenda tranne per i casi
        in cui metto in edit i prodotti
     */
    Msg_Verifica_Conformita_Intervento_Ribaltamento() {
        let Operazioni: Array<Lavorazione> = this.qdcservice.TestataForm.get("Operazioni").getRawValue();
        if (this.qdcservice.Is_Ribaltamento_Ricetta_Da_Origine_Diversa()
            && this.QdCHeaderVisibility.MostraBtn_VerificaConformita(Operazioni)
            && !this.qdcservice.CheckDosiProdottiNonSalvate()
        ) {
            let listerroriGias: ErroreGias[] = [];
            listerroriGias.push(<ErroreGias>{
                severity: ErroreGias_Severity.Warning,
                tipo: enum_ErroreGias_Tipo.Generico,
                messaggio: this.translocoService.translate('qdc.VuoiVerificareConfIntervento')
            });

            this.qdcservice.gestisci_ErroriGias(listerroriGias, false, false, "").then((obj: Obj_Errore_Gias_QdC) => {
                if (obj.result.returnObj) {
                    this.redirectTo_Verifica_Conformita();
                }
            });
        }

    }

}
