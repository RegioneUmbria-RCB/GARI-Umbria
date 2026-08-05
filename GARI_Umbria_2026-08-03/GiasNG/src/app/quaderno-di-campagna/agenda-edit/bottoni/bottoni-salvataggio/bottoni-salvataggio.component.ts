import {Component, OnDestroy, OnInit, TemplateRef, ViewChild} from '@angular/core';
import { DropDownButtonComponent, PreventableEvent } from '@progress/kendo-angular-buttons';
import { Attivita } from 'app/Model/attivita/Attivita';
import { AgendaService, Attivita_Con_Parametri_Aggiuntivi, LeggiMenuRicette, MenuRicette, ScriviListaAttivita } from 'app/Service/Agenda/Agenda.service';
import { Acqua, CodiciXOperazione, DropdownListAttivitaPersonalizzata, DropdownListDisciplinare, Key_Parametri_Aggiuntivi_Attivita, Parametri_Aggiuntivi_Attivita, Superfici, Testata_Visita, Voci_Menu } from '../../quaderno-di-campagna-form/quaderno-di-campagna-form.model';
import { QdCService, Redirect_To_GiasNG_Page } from '../../service/qdc.service';
import { ErroreGias, ErroreGias_Severity, MasterService, rispostaStandard } from "../../../../Service/master.service";
import { enum_PagineGiasNG, enum_Tipo_Salvataggio_QdC } from "../../../../Model/TipiEnumerativi";
import { ObjParametriAgendaService } from "../../../../Service/obj-parametri-agenda.service";
import { GestioneRichiesteService, ParametriAggiuntivi_QueryString } from "../../../../Service/gestione-richieste.service";
import { enum_PagineAgenda_2010, enum_PaginePianoConcimazione_2017, Enum_SiteRedirector } from "../../../../Model/siti.enum";
import { QdCControlliSalvataggioService } from "../../service/qdc-controlli-salvataggio.service";
import { QdCFormToAttivitaService } from "../../service/quaderno-di-campagna-form/quaderno-di-campagna-form-to-attivita.service";
import { GiasMessageService } from "../../../../Service/gias-message.service";
import { TranslocoService } from "@jsverse/transloco";
import { Enum_DBTypeOperation } from 'gias-ui-kit';
import { Subscription } from "rxjs";
import { Lavorazione } from "../../../../Model/attivita/Lavorazione";
import { cloneDeep } from "lodash";
import { LinkQdCAngular } from "../../../../Model/CostantiPersonalizzate";
import { RibaltamentoTypes } from "../../../../menu-agenda/components/utils";
import { FunzioniComuniService } from "../../../../Service/FunzioniComuni.service";
import { messaggioPostMessage, ObjParametriAgenda, Stati, Tipo_Attivita } from 'gias-ui-kit';

@Component({
    standalone: false,
    selector: 'app-bottoni-salvataggio',
    templateUrl: './bottoni-salvataggio.component.html',
    styleUrls: ['./bottoni-salvataggio.component.scss']
})
export class BottoniSalvataggioComponent implements OnInit, OnDestroy {

    public Enum_Tipo_Salvataggio_QdC = enum_Tipo_Salvataggio_QdC;

    /*
    *@description:
    * Aggiunto questo flag per disabilitare subito i bottoni di salvataggio appena si clicca su salva per evitare che
    * vengano salvate più agende con il doppio click
    * */
    public disabilitaBtn_Salva: boolean = false;

    Subs: Subscription = new Subscription();

    VoceMenuRicetteSelezionata: Voci_Menu = null;

    dataMenuRicette: Array<Voci_Menu> = [];

    VoceMenuOperazioni_Compatibili_Visite: Voci_Menu = null;

    dataMenuOperazioni_Compatibili_Visite: Array<Voci_Menu> = [];

    objParametriAgenda: ObjParametriAgenda;

    @ViewChild("BtnMenuRicette") public MenuRicette: DropDownButtonComponent;

    @ViewChild("BtnMenuSalvaVisite") BtnMenuSalvaVisite: DropDownButtonComponent;

    @ViewChild("BtnMenuVaiVisite") BtnMenuVaiVisite: DropDownButtonComponent;

    constructor(public qdcservice: QdCService,
        private agendaservice: AgendaService,
        private masterService: MasterService,
        private qdccontrollisalvataggioservice: QdCControlliSalvataggioService,
        private qdcformtoattivitaservice: QdCFormToAttivitaService,
        private messageservice: GiasMessageService,
        private translocoService: TranslocoService,
        private objParametriAgendaService: ObjParametriAgendaService,
        private gestioneRichiesteService: GestioneRichiesteService,
        private funzioniComuniService: FunzioniComuniService) { }


    ngOnInit(): void {
        this.objParametriAgenda = this.objParametriAgendaService.getObjParamValue();
    }

    disabilitaBtn(Tipo_Salvataggio_QdC: number) {
        let disabilita = false;

        if (!this.qdcservice.QdCForm.valid || this.qdcservice.Sola_Lettura_QdCForm() || this.disabilitaBtn_Salva) {

            //Abilito solo il bottone Salva ed Esci se sto facendo un Reinnesco Trappole
            if(!(this.qdcservice.flag_Reinnesco && Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_ed_Esci)){
              disabilita = true;
            }

        }

        return disabilita;
    }

    gestisciTipoErrore_Warning(Tipo_Salvataggio_QdC: number, listaAttivita: Attivita[], listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[], rispostaStandard: rispostaStandard<Attivita[]>, complianceResultStatus: ComplianceResultStatus) {

        this.qdcservice.gestisci_ErroriGias(rispostaStandard.ErroriGias, true, false, "Sidesideraproseguire", null, "auto", "auto", complianceResultStatus).then((res: any) => {

            if (res) {
                switch (res.severity) {
                    case ErroreGias_Severity.WarningBloccante:
                        if (res.result.returnObj === 'continue'){
                            let index = listParametri_Aggiuntivi_Attivita.findIndex(e => e.key === Key_Parametri_Aggiuntivi_Attivita.IdTestataVerificaConformita);
                            if (index === -1) {

                                let parametroAggiuntivo: Parametri_Aggiuntivi_Attivita = {
                                    key: Key_Parametri_Aggiuntivi_Attivita.IdTestataVerificaConformita,
                                    value: complianceResultStatus.ComplianceResponse.IdTestata.toString(),
                                    operazione: null
                                };

                                listParametri_Aggiuntivi_Attivita.push(parametroAggiuntivo);
                            } else {
                                listParametri_Aggiuntivi_Attivita[index].value = complianceResultStatus.ComplianceResponse.IdTestata.toString();
                            }

                            this.SalvaQdC(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita);
                        } else {
                            return;
                        }
                        break;
                    case ErroreGias_Severity.Warning:
                        //Se l'utente ha cliccato su Sì allora eseguo il salvataggio saltando il warning a cui ha detto sì
                        if (res.result.returnObj === true) {

                            this.change_lista_MostraWarning(res.list_errori_filtrati, listParametri_Aggiuntivi_Attivita);

                            this.SalvaQdC(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita);

                        } else if (res.result.returnObj === 'continue'){
                            let index = listParametri_Aggiuntivi_Attivita.findIndex(e => e.key === Key_Parametri_Aggiuntivi_Attivita.IdTestataVerificaConformita);
                            if (index === -1) {

                                let parametroAggiuntivo: Parametri_Aggiuntivi_Attivita = {
                                    key: Key_Parametri_Aggiuntivi_Attivita.IdTestataVerificaConformita,
                                    value: complianceResultStatus.ComplianceResponse.IdTestata.toString(),
                                    operazione: null
                                };

                                listParametri_Aggiuntivi_Attivita.push(parametroAggiuntivo);
                            } else {
                                listParametri_Aggiuntivi_Attivita[index].value = complianceResultStatus.ComplianceResponse.IdTestata.toString();
                            }

                            this.SalvaQdC(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita);
                        } else {
                            return;
                        }
                        break;
                }
            }
        });
    }


    preSalvataggioQdC(Tipo_Salvataggio_QdC: number) {

        this.ChiudiDropDownButton();

        this.disabilitaBtn_Salva = true;

        this.qdccontrollisalvataggioservice.ControllaQdC().then((listerroriGias: ErroreGias[]) => {

            if (listerroriGias.length === 0) {

                let listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[] = [];

                let listaAttivita = this.qdcformtoattivitaservice.mapQdCFormToListaAttivita(null, listParametri_Aggiuntivi_Attivita, false);

                if (listaAttivita && listaAttivita.length > 0) {

                    this.qdcservice.setListaAttivita_Con_Parametri_Aggiuntivi(listaAttivita as Attivita_Con_Parametri_Aggiuntivi[]);

                    if(this.qdcservice.Is_Ribaltamento_Ricetta_Da_PUA()){
                      this.Controlla_Giacenza_Fertilizzanti_QdC(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita)
                    }else{
                      this.SalvaQdC(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita);
                    }
                }
            } else {
                this.disabilitaBtn_Salva = false;
            }
        });
    }

    private postSalvataggioQdC(Tipo_Salvataggio_QdC: number, listaAttivitaSalvate: Array<Attivita>) {
        //La scrittura mi restituisce la lista delle attivita salvate perché così ho il codice valorizzato
        //anche se sono in nuovo

        if (listaAttivitaSalvate && listaAttivitaSalvate.length > 0) {

            this.messageservice.successMessage(this.translocoService.translate('RegistrazioneEffettuataConSuccesso'));

            let paginaProvenienza: number = this.objParametriAgenda.Pagina_Provenienza;


            //Resetto anche l'objParametriAgenda quando esco dalla pagina con salva ed esci e vai ai costi
            if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_ed_Esci ||
                Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {

                if (!this.gestioneRichiesteService.ObjPageToMemorize) {

                    let newobjParametriAgenda = this.objParametriAgendaService.resettaObjAgenda(new ObjParametriAgenda);

                    newobjParametriAgenda.Piva = this.objParametriAgenda.Piva;

                    newobjParametriAgenda.RagSoc = this.objParametriAgenda.RagSoc;

                    newobjParametriAgenda.Sito_Provenienza = this.objParametriAgenda.Sito_Provenienza;

                    newobjParametriAgenda.Pagina_Provenienza = this.objParametriAgenda.Pagina_Provenienza;

                    newobjParametriAgenda.Pagina_Provenienza_AltroSito = this.objParametriAgenda.Pagina_Provenienza_AltroSito;

                    //Mantengo memorizzato il Ricetta_Cod se ho aperto la pagina PUA_Dichiarazione_Effluenti del Piano Concimazione perchè
                    //mi serve per caricare correttamente poi la pagina aspx
                    if (newobjParametriAgenda.Sito_Provenienza === Enum_SiteRedirector.Sito_PianoConcimazione_2017 &&
                        newobjParametriAgenda.Pagina_Provenienza_AltroSito === enum_PaginePianoConcimazione_2017.PUA_Dichiarazione_Effluenti) {

                        newobjParametriAgenda.Ricetta_Cod = this.objParametriAgenda.Ricetta_Cod;

                    }

                    this.objParametriAgendaService.changeObjParametriAgenda(newobjParametriAgenda);
                }
            }

            this.disabilitaBtn_Salva = false;

            this.gestioneRichiesteService.getPathFromPagina_Richiesta(this.qdcservice.masterService.getCurrentPageAsValue()).then(link_pagina_corrente => {
                switch (Tipo_Salvataggio_QdC) {
                    case enum_Tipo_Salvataggio_QdC.Salva_ed_Esci:
                        this.gestioneRedirect();
                        break;
                    case enum_Tipo_Salvataggio_QdC.Salva_e_Nuovo:
                        this.clearQdCFormModelxSalva_e_Nuovo(link_pagina_corrente);
                        break;
                    case enum_Tipo_Salvataggio_QdC.Salva_e_Duplica:
                        //Rimango nella pagina
                        break;
                    case enum_Tipo_Salvataggio_QdC.Salva_e_CDG:
                        this.redirectToGestioneCosti(listaAttivitaSalvate);
                        break;
                    case enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio:
                        this.redirectToDettagliRicetta(listaAttivitaSalvate);
                        break;
                    case enum_Tipo_Salvataggio_QdC.Salva_Visita_Aggiungi_Operazione:
                        this.redirectToOperazionefromVisita(listaAttivitaSalvate);
                        break;
                }
            });


        }
    }

    private change_lista_MostraWarning(list_warning: ErroreGias[], listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]) {

        let distinct_warning: ErroreGias[] = [...new Map(list_warning.map(item => [item["ex"], item])).values()];

        let lista_warning_parametri_aggiuntivi = JSON.parse(JSON.stringify(listParametri_Aggiuntivi_Attivita.find(p => p.key === Key_Parametri_Aggiuntivi_Attivita.lista_MostraWarning)));

        if (distinct_warning && lista_warning_parametri_aggiuntivi) {

            let value_lista_warning_parametri_aggiuntivi: Parametri_Aggiuntivi_Attivita[] = JSON.parse(lista_warning_parametri_aggiuntivi.value);

            for (let v of value_lista_warning_parametri_aggiuntivi) {
                for (let w of distinct_warning) {
                    if (v.key === w.ex) {
                        v.value = "false";
                    }
                }
            }

            for (let p of listParametri_Aggiuntivi_Attivita) {
                if (p.key === Key_Parametri_Aggiuntivi_Attivita.lista_MostraWarning) {
                    p.value = JSON.stringify(value_lista_warning_parametri_aggiuntivi);
                    break;
                }
            }
        }

    }
    /*
        @description
        Salva l'attivita e fa il redirect alla pagina di Gestione Costi.
        N.B.: Se si deciderà in futuro di permettere una multi operazione con operazioni senza scarico di prodotto
        (esempio Aratura + Andamento) Sarà necessario sviluppare nella parte dei Costi anche questo caso perché non è gestito.
     */

    private redirectToGestioneCosti(listaAttivitaSalvate: Array<Attivita>) {

        this.masterService.set_isLoading({ message: '', isLoading: true });

        //Se c'è una operazione valorizzo l'id_agenda e lascio a 0 il raccoglitore_cod, se c'è più di una operazione
        //valorizzo il raccoglitore_cod e lascio a 0 l'id_agenda

        let id_agenda: string = "0";

        let raccoglitore_cod: string = "0";

        if (listaAttivitaSalvate.length === 1) {
            id_agenda = listaAttivitaSalvate[0].codice;
        } else {
            raccoglitore_cod = listaAttivitaSalvate[0].raccoglitore.toString();
        }

        let newobjParametriAgenda: ObjParametriAgenda = cloneDeep(this.objParametriAgenda);
        newobjParametriAgenda.Id_Agenda = 0;
        newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
        newobjParametriAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;

        let ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> =
            [
                /* {
                    key:"origine",
                    value: "",
                    codifica: true
                }, */
                {
                    key: "p",
                    // value: newobjParametriAgenda.Piva,
                    value: this.qdcservice.getImpresa_Model().partitaIva,
                    codifica: true
                },
                {
                    key: "id_agenda",
                    value: id_agenda,
                    codifica: true
                },
                {
                    key: "entrata_diretta",
                    value: "0",
                    codifica: true
                },
                {
                    key: "raccoglitore_cod",
                    value: raccoglitore_cod,
                    codifica: true
                }
            ];

        this.gestioneRichiesteService.gestionePassaggioAltroSito(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, enum_PagineAgenda_2010.Pagina_Gestione_Costi, ParametriAggiuntivi, newobjParametriAgenda).then(resp => window.location.href = resp);
    }

    private SalvaQdC(Tipo_Salvataggio_QdC: number, listaAttivita: Attivita[], listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]) {

        //Se eseguiVerificheConformita è true allora vengono mostrati i messaggi di warning in cui l'utente deve scegliere sì/no altrimenti se  eseguiVerificheConformita è false
        //allora vengono bypassati i messaggi di warning

        const ScriviListaAttivita = <ScriviListaAttivita>{
            attivita_list: listaAttivita,
            parametri_aggiuntivi_list: listParametri_Aggiuntivi_Attivita
        };

        this.agendaservice.ScriviListaAttivita(ScriviListaAttivita).then(rispostaStandard => {

            if (rispostaStandard.RispostaOK) {
                this.postSalvataggioQdC(Tipo_Salvataggio_QdC, rispostaStandard.RispostaStringa);
            } else {
                this.disabilitaBtn_Salva = false;
                let complianceResultStatus: ComplianceResultStatus = null;
                if (rispostaStandard.ParametroDue_stringa !== undefined && rispostaStandard.ParametroDue_stringa !== null && rispostaStandard.ParametroDue_stringa !== ""){
                    complianceResultStatus = JSON.parse(rispostaStandard.ParametroDue_stringa) as ComplianceResultStatus;
                }

                this.gestisciTipoErrore_Warning(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita, rispostaStandard, complianceResultStatus);
            }

        });
    }

    private clearQdCFormModelxSalva_e_Nuovo(link_pagina_corrente: string) {
        //replico quello che fa la Trattamenti_2 quando si clicca su Salva e Nuovo nella funzione fine_salvataggio
        //quindi pulisco gli Impianti Selezionati,Macchine,Operatori, Note,Prodotti e reimposto il Disciplinare

        let newObjPageToMemorize = new Redirect_To_GiasNG_Page()

        newObjPageToMemorize.objParametriAgenda = cloneDeep(this.objParametriAgenda);
        newObjPageToMemorize.QdCFormValue = cloneDeep(this.qdcservice.QdCForm.getRawValue());
        newObjPageToMemorize.QdCFormValue.Trattamento.Testata.MultiCentro = false;
        newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Raccoglitore = 0;
        newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);
        newObjPageToMemorize.QdCFormValue.Trattamento.CostiAccessori.Macchine = [];
        newObjPageToMemorize.QdCFormValue.Trattamento.CostiAccessori.Operatori = [];
        newObjPageToMemorize.QdCFormValue.Trattamento.Impianti.ImpiantiSelezionati = [];

        newObjPageToMemorize.QdCFormValue.Trattamento.Superfici = <Superfici>{
            Sup_Selezionata: 0,
            Sup_Trattata: 0
        };

        newObjPageToMemorize.QdCFormValue.Trattamento.Acqua = <Acqua>{
            Acqua_Ha: 0,
            Acqua_Tot: 0,
            Dose_Acqua: 0
        };

        newObjPageToMemorize.QdCFormValue.Trattamento.Sezioni_Prodotto = [];
        newObjPageToMemorize.QdCFormValue.Trattamento.Sezioni_Senza_Prodotto = [];
        newObjPageToMemorize.QdCFormValue.Trattamento.Nota_Testuale = "";
        newObjPageToMemorize.QdCFormValue.Trattamento.Note = [];

        this.gestioneRichiesteService.ObjPageToMemorize = newObjPageToMemorize;
        this.objParametriAgendaService.navigateTo(link_pagina_corrente, null, null, true);

    }

    //replica la funzione inizializzazionePulsanteSalvataggio() della OperazioneBootstrap.master
    inizializzazionePulsanteSalvataggio(Tipo_Salvataggio_QdC: number) {

        let mostra = true;

        if (!this.qdcservice.MostrabtnSalva) {
            mostra = false;

            return mostra;
        }

        if (this.objParametriAgenda) {
            switch (this.objParametriAgenda.TipoOperazioneDB) {
                case Enum_DBTypeOperation.Write:

                    if(!this.qdcservice.Sola_Lettura_QdCForm()){
                      if(this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna){
                        if(Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG){
                          if(this.qdcservice.RilievoSenzaImpianti()){
                            mostra = false;
                          }else{
                            mostra = this.qdcservice.flag_MostraBtnSalvaCDG;
                          }
                        }

                        if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio) {
                          mostra = false;
                        }

                      } else if (this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.Ricetta) {

                        if (this.objParametriAgenda.Stato === Stati.Da_Eseguire) {
                          if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Nuovo || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Duplica || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {
                            mostra = false;
                          }
                        } else if (this.objParametriAgenda.Stato === Stati.Eseguita) {
                          if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {
                            mostra = false;
                          }
                        }
                      }

                      //Durante i Ribaltamenti mostro solo il pulsante salva ed esci ed eventualmente se abilitato il pulsante BtnSalvaCDG
                      if (this.qdcservice.TipoRibaltamento !== RibaltamentoTypes.Nessuno) {
                        if (Tipo_Salvataggio_QdC !== enum_Tipo_Salvataggio_QdC.Salva_ed_Esci && Tipo_Salvataggio_QdC !== enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {
                          mostra = false;
                        }else if(Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG){
                          if(this.qdcservice.RilievoSenzaImpianti()){
                            mostra = false;
                          }else{
                            mostra = this.qdcservice.flag_MostraBtnSalvaCDG;
                          }
                        }
                      }

                    }else{

                      //Mostro solo il bottone Salva ed Esci se sto facendo un Reinnesco Trappole
                      if(!(this.qdcservice.flag_Reinnesco && Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_ed_Esci)){
                        mostra = false;
                      }
                    }
                    break;
                case Enum_DBTypeOperation.Read:
                    mostra = false;
                    break;
                case Enum_DBTypeOperation.Update:

                  if(!this.qdcservice.Sola_Lettura_QdCForm()){

                    if (this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna) {
                      if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Nuovo || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Duplica || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio) {
                        mostra = false;
                      }

                      if(Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {
                        if(this.qdcservice.RilievoSenzaImpianti()){
                          mostra = false;
                        }else{
                          mostra = this.qdcservice.flag_MostraBtnSalvaCDG;
                        }
                      }

                    } else if (this.objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.Ricetta) {

                      if (this.objParametriAgenda.Stato === Stati.Da_Eseguire) {
                        if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Nuovo || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Duplica || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {
                          mostra = false;
                        }
                      } else if (this.objParametriAgenda.Stato === Stati.Eseguita) {

                        if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Nuovo || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_Duplica || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio || Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_e_CDG) {
                          mostra = false;
                        }

                      }
                    }
                  }else{

                    //Mostro solo il bottone Salva ed Esci se sto facendo un Reinnesco Trappole
                    if(!(this.qdcservice.flag_Reinnesco && Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_ed_Esci)){
                      mostra = false;
                    }
                  }
                    break;
            }
        }


        if (Tipo_Salvataggio_QdC === enum_Tipo_Salvataggio_QdC.Salva_Visita_Aggiungi_Operazione) {
            mostra = this.Mostra_Pulsante_Menu_Operazioni_Compatibili_Visite();
        }



        return mostra;
    }

    public Mostra_Pulsante_Solo_Navigazione_Menu_Operazioni_Compatibili_Visite(): boolean {
        let mostra = false;

        if (!this.qdcservice.MostrabtnSalva &&
            this.Mostra_Pulsante_Menu_Operazioni_Compatibili_Visite() &&
            this.objParametriAgenda?.TipoOperazioneDB === Enum_DBTypeOperation.Update) {

            let attivita_collegate_distinct_raccoglitore_cod: Array<Attivita> = this.getAttivita_Collegate_Distinct_Raccoglitore_Cod();

            if (attivita_collegate_distinct_raccoglitore_cod && attivita_collegate_distinct_raccoglitore_cod.length > 0) {
                mostra = true;
            }
        }

        return mostra;
    }

    public Mostra_Pulsante_Menu_Operazioni_Compatibili_Visite(): boolean {

        let mostra = false;

        let flag_visita: boolean = this.qdcservice?.TestataForm?.get("flagVisita")?.getRawValue();

        let attivita_personalizzata: DropdownListAttivitaPersonalizzata = this.qdcservice?.TestataForm?.get("Attivita_Personalizzata")?.getRawValue();

        //Navigazione Visite
        if (flag_visita &&
            attivita_personalizzata &&
            attivita_personalizzata?.operazioni?.findIndex((o: Lavorazione) => this.qdcservice.Elenco_Operazioni_Compatibili_Con_Visite.includes(+ o.primaryKey.codice)) > -1) {

            mostra = true;
        } else {
            mostra = false;
        }

        return mostra;
    }

    MostraBtn() {

        let mostra = false;

        if (this.inizializzazionePulsanteSalvataggio(this.Enum_Tipo_Salvataggio_QdC.Salva_e_Nuovo) || this.inizializzazionePulsanteSalvataggio(this.Enum_Tipo_Salvataggio_QdC.Salva_e_Duplica) ||
            this.inizializzazionePulsanteSalvataggio(this.Enum_Tipo_Salvataggio_QdC.Salva_e_CDG) || this.inizializzazionePulsanteSalvataggio(this.Enum_Tipo_Salvataggio_QdC.Salva_ed_Esci) ||
            this.inizializzazionePulsanteSalvataggio(this.Enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio) || this.inizializzazionePulsanteSalvataggio(this.Enum_Tipo_Salvataggio_QdC.Salva_Visita_Aggiungi_Operazione) ||
            this.Mostra_Pulsante_Solo_Navigazione_Menu_Operazioni_Compatibili_Visite()) {
            mostra = true;
        }

        return mostra;
    }

    private gestioneRedirect() {
        if (this.qdcservice.inFrame == true) {
            window.parent.postMessage("chiudiFinestra", this.funzioniComuniService.getOrigins());
        } else if (this.qdcservice.inWindow == true) {
            window.parent.postMessage(messaggioPostMessage.chiudiWindowGiasNG, this.funzioniComuniService.getOrigins());
        } else {

            let pagina_richiesta = this.objParametriAgenda.Pagina_Provenienza;

            if (this.objParametriAgenda.Sito_Provenienza !== Enum_SiteRedirector.GiasNG) {
                pagina_richiesta = this.objParametriAgenda.Pagina_Provenienza_AltroSito;
            }

            //Torno indietro alla pagina di provenienza che sia il Menu Agenda NG o la pagina di un altro sito
            this.gestioneRichiesteService.gestionePassaggioAltroSito(this.objParametriAgenda.Sito_Provenienza,
                pagina_richiesta).then(link => {
                    if (link && link !== "") {
                        if (this.objParametriAgenda.Sito_Provenienza === Enum_SiteRedirector.GiasNG) {
                            this.objParametriAgendaService.navigateTo(link, null, null, false);
                        } else {
                            window.location.href = link;
                        }
                    }
                });
        }
    }

    async OpenMenuRicette(event: PreventableEvent) {

        //Faccio la chiamata per ottenere le voci del menu solamente se è vuoto
        if (!this.dataMenuRicette || this.dataMenuRicette.length === 0) {

            this.dataMenuRicette = [];

            let Codici_Attivita: CodiciXOperazione[] = this.qdcservice.TestataForm.get("Codici_Attivita").getRawValue();

            const LeggiMenuRicette = <LeggiMenuRicette>{
                codiciAttivita: Codici_Attivita
            };

            let r_op = await this.agendaservice.CaricaMenuRicette(LeggiMenuRicette);

            if (r_op && r_op.length > 0) {
                r_op.forEach(value => {
                    this.dataMenuRicette.push(<Voci_Menu>{
                        text: value.des,
                        value: value as MenuRicette,
                        disabled: value.ribaltata || value.inviata_ad_app
                    });
                });
            }
        }


    }

    OpenMenuOperazioni_Compatibili_Visite(event: PreventableEvent, salva_visita: boolean) {

        this.dataMenuOperazioni_Compatibili_Visite = [];

        let attivita_collegate_distinct_raccoglitore_cod: Array<Attivita> = this.getAttivita_Collegate_Distinct_Raccoglitore_Cod();

        if (attivita_collegate_distinct_raccoglitore_cod && attivita_collegate_distinct_raccoglitore_cod.length > 0) {

            attivita_collegate_distinct_raccoglitore_cod.forEach(a => {

                let lavorazione: Lavorazione = a.job as Lavorazione;

                let lav_des: string = a.job.descrizione;

                let data: string = this.qdcservice.datepipe.transform(a.inizio, 'dd/MM/yy', undefined, this.qdcservice.locale_id);

                let new_Voce_menu = <Voci_Menu>{
                    text: "",
                    value: new CodiciXOperazione(a.codice, "0", "0",
                        lavorazione, null, null, "", "0",
                        null),
                    disabled: false
                };

                if (salva_visita) {
                    new_Voce_menu.text = this.translocoService.translate("qdc.ModificaOpdellaVisita", { Operazione: lav_des, Data: data });
                } else {
                    new_Voce_menu.text = this.translocoService.translate("qdc.OperazionedelData", { Operazione: lav_des, Data: data });
                }

                this.dataMenuOperazioni_Compatibili_Visite.push(new_Voce_menu);
            });

        } else if (salva_visita) {
            //Solo se non ci sono delle Operazioni collegate alla Visita do la possibilità di aggiungerne altre

            let attivita_personalizzata: DropdownListAttivitaPersonalizzata = this.qdcservice.TestataForm.get("Attivita_Personalizzata").getRawValue();

            if (attivita_personalizzata && attivita_personalizzata.Codice_Concatenato !== "" && attivita_personalizzata.operazioni && attivita_personalizzata.operazioni.length > 0) {
                this.dataMenuOperazioni_Compatibili_Visite.push(<Voci_Menu>{
                    text: this.translocoService.translate("qdc.AggiungiNuovaOpAllaVisita", { Operazione: attivita_personalizzata.operazioni[0].descrizione }),
                    value: new CodiciXOperazione("0", "0", "0",
                        attivita_personalizzata.operazioni[0], null, null, "", "0",
                        null),
                    disabled: false
                });
            }
        }


    }

    async ClickMenuRicette(item: Voci_Menu) {

        this.VoceMenuRicetteSelezionata = item;

        if (this.VoceMenuRicetteSelezionata.value) {

            this.VoceMenuRicetteSelezionata.value = this.VoceMenuRicetteSelezionata.value as MenuRicette;

            //Se la ricetta è stata ribaltata o inviata ad APP non è possibile modificarla
            if (!this.VoceMenuRicetteSelezionata.value.ribaltata && !this.VoceMenuRicetteSelezionata.value.inviata_ad_app) {
                this.preSalvataggioQdC(enum_Tipo_Salvataggio_QdC.Salva_Ricetta_e_Nuovo_Dettaglio);
            }
        }
    }

    async ClickMenuOperazioni_Compatibili_Visite(item: Voci_Menu, salva_visita: boolean) {
        this.VoceMenuOperazioni_Compatibili_Visite = item;

        if (this.VoceMenuOperazioni_Compatibili_Visite.value) {

            this.VoceMenuOperazioni_Compatibili_Visite.value = this.VoceMenuOperazioni_Compatibili_Visite.value as CodiciXOperazione;

            if (salva_visita) {
                this.preSalvataggioQdC(enum_Tipo_Salvataggio_QdC.Salva_Visita_Aggiungi_Operazione);
            } else {
                this.redirectToOperazionefromVisita(this.qdcservice.getListaAttivita() as Array<Attivita>);
            }
        }
    }

    private redirectToDettagliRicetta(listaAttivitaSalvate: Array<Attivita>) {

        if (!this.VoceMenuRicetteSelezionata || !this.VoceMenuRicetteSelezionata.value)
            return;

        this.masterService.set_isLoading({ message: '', isLoading: true });

        let value = this.VoceMenuRicetteSelezionata.value as MenuRicette;

        let newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));


        //Se la voce selezionata è una Ricetta multi allora posso prendere il ricetta_operazione_cod del primo
        //tanto dopo nella lettura dell'attività mi verrà restituita una lista per l'operazione multi
        if (value.codiciAttivita.length > 0) {
            newobjParametriAgenda.Ricetta_Operazione_Cod = + value.codiciAttivita[0].CodiceOperazioneRicetta;
        }

        newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;

        //Voglio aggiungere un nuovo dettaglio a quella Ricetta Testata
        if (value.codiciAttivita.length === 1 && + value.codiciAttivita[0].CodiceOperazioneRicetta === 0) {

            //Faccio come la Trattamenti_2 che preimposta i dati della precedente ricetta se faccio nuovo dettaglio

            //I valori della testata ricetta li recupero dall'ultimo elemento della lista attivita in scrittura perché è
            //l'unico con il Ricetta_Des valorizzato
            let testata_ricetta = listaAttivitaSalvate[listaAttivitaSalvate.length - 1].testataRicetta;

            newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;

            let newObjPageToMemorize = new Redirect_To_GiasNG_Page();

            newObjPageToMemorize.objParametriAgenda = cloneDeep(newobjParametriAgenda);

            newObjPageToMemorize.QdCFormValue = cloneDeep(this.qdcservice.QdCForm.getRawValue());

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.MultiCentro = false;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Ricetta_Des = testata_ricetta.Ricetta_Des;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Ricetta_Des_Long = testata_ricetta.Ricetta_Des_Long;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Ricetta_Numero = testata_ricetta.Ricetta_Numero;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Ricetta_Note = testata_ricetta.Note;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Ricetta_Data_Da = testata_ricetta.Data_A;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Ricetta_Data_A = testata_ricetta.Data_A;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Raccoglitore = 0;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Origine = "";

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Codici_Attivita = newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Operazioni.map((o: Lavorazione) => {
                return new CodiciXOperazione("0",
                    testata_ricetta.Ricetta_Cod.toString(),
                    "0", o, null, null, "", "0", testata_ricetta.pua)
            });

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);

            newObjPageToMemorize.QdCFormValue.Trattamento.CostiAccessori.Macchine = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.CostiAccessori.Operatori = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Impianti.ImpiantiSelezionati = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Superfici = <Superfici>{
                Sup_Selezionata: 0,
                Sup_Trattata: 0
            };

            newObjPageToMemorize.QdCFormValue.Trattamento.Acqua = <Acqua>{
                Acqua_Ha: 0,
                Acqua_Tot: 0,
                Dose_Acqua: 0
            };

            newObjPageToMemorize.QdCFormValue.Trattamento.Sezioni_Prodotto = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Sezioni_Senza_Prodotto = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Nota_Testuale = "";

            newObjPageToMemorize.QdCFormValue.Trattamento.Note = [];

            this.gestioneRichiesteService.ObjPageToMemorize = newObjPageToMemorize;

            this.objParametriAgendaService.navigateTo(LinkQdCAngular, null, null, true);

        } else {
            //Voglio modificare una Ricetta Operazione con la stessa Ricetta Testata

            newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;

            this.objParametriAgendaService.navigateTo(LinkQdCAngular, null, newobjParametriAgenda, true);

        }

    }

    private redirectToOperazionefromVisita(listaAttivitaSalvate: Array<Attivita>) {

        if (!this.VoceMenuOperazioni_Compatibili_Visite || !this.VoceMenuOperazioni_Compatibili_Visite.value)
            return;

        this.masterService.set_isLoading({ message: '', isLoading: true });

        let value = this.VoceMenuOperazioni_Compatibili_Visite.value as CodiciXOperazione;

        let newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(this.objParametriAgenda));

        //Prendo l'Id_Attivita dalla voce di menu scelta
        if (value) {
            newobjParametriAgenda.Id_Agenda = + value.CodiceAttivita;
        }

        newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Visite;

        newobjParametriAgenda.Lav_Cod = 0;

        newobjParametriAgenda.Lav_Des = "";

        const aziendaVisita = this.qdcservice.TestataVisitaForm.get("Azienda_Visita").value;

        //fix per salvataggio Rilievo associato alle Visite
        if (aziendaVisita) {
            newobjParametriAgenda.Piva = aziendaVisita.partitaIva;
            newobjParametriAgenda.RagSoc = aziendaVisita.ragioneSociale;
        }

        //Voglio aggiungere una nuova Operazione collegata alla Visita
        if (value && + value.CodiceAttivita === 0) {

            //Pulisco le attivita collegate prechè all'Operazione gli passo solo la Visita salvate

            let VisitaSalvata: Attivita = cloneDeep(listaAttivitaSalvate[listaAttivitaSalvate.length - 1]);

            VisitaSalvata.attivitaCollegate = [];

            let CodiceVisita = VisitaSalvata.codice;

            newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;

            let newObjPageToMemorize = new Redirect_To_GiasNG_Page();

            newObjPageToMemorize.objParametriAgenda = cloneDeep(newobjParametriAgenda);

            newObjPageToMemorize.QdCFormValue = cloneDeep(this.qdcservice.QdCForm.getRawValue());

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.TestataVisita = new Testata_Visita();

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Tipo = newobjParametriAgenda.TipoOperazioneAgenda;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.TipoRicetta = newobjParametriAgenda.TipoRicetta;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Stato = newobjParametriAgenda.Stato as Stati;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.MultiCentro = false;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Raccoglitore = 0;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Origine = "";

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.flagVisita = false;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Operazioni = [value.Operazione];

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Visualizza_Solo_Operazioni_Preferite = false;

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Codici_Attivita = newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Operazioni.map((o: Lavorazione) => {
                return new CodiciXOperazione("0",
                    "0",
                    "0", o, null, null, "", CodiceVisita, null)
            });

            newObjPageToMemorize.QdCFormValue.Trattamento.Testata.Disciplinare = this.qdcservice.setCodDescrDdlDisciplinare(this.qdcservice.Obj_NessunDpiNessunaEtichetta as DropdownListDisciplinare);

            newObjPageToMemorize.QdCFormValue.Trattamento.CostiAccessori.Macchine = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.CostiAccessori.Operatori = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Superfici = <Superfici>{
                Sup_Selezionata: 0,
                Sup_Trattata: 0
            };

            newObjPageToMemorize.QdCFormValue.Trattamento.Acqua = <Acqua>{
                Acqua_Ha: 0,
                Acqua_Tot: 0,
                Dose_Acqua: 0
            };

            newObjPageToMemorize.QdCFormValue.Trattamento.Sezioni_Prodotto = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Sezioni_Senza_Prodotto = [];

            newObjPageToMemorize.QdCFormValue.Trattamento.Nota_Testuale = "";

            newObjPageToMemorize.QdCFormValue.Trattamento.Note = [];

            this.gestioneRichiesteService.ObjPageToMemorize = newObjPageToMemorize;

            this.objParametriAgendaService.navigateTo(LinkQdCAngular, null, null, true);

        } else {
            //Voglio modificare una Operazione collegata alla Visita

            newobjParametriAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Update;

            this.objParametriAgendaService.navigateTo(LinkQdCAngular, null, newobjParametriAgenda, true);

        }

    }

    getAttivita_Collegate_Distinct_Raccoglitore_Cod(): Array<Attivita> {

        let attivita_collegate_distinct_raccoglitore_cod: Array<Attivita> = [];

        let attivita_collegate: Array<Attivita> = this.qdcservice.TestataForm.get("Attivita_Collegate").getRawValue();

        if (attivita_collegate && attivita_collegate.length > 0) {
            //Faccio il distinct per raccoglitore_cod

            attivita_collegate_distinct_raccoglitore_cod = [...new Map(attivita_collegate.map(item => [item["raccoglitore"], item])).values()];

        }

        return attivita_collegate_distinct_raccoglitore_cod;
    }

   private Controlla_Giacenza_Fertilizzanti_QdC(Tipo_Salvataggio_QdC: number, listaAttivita: Attivita[], listParametri_Aggiuntivi_Attivita: Parametri_Aggiuntivi_Attivita[]) {

    const ScriviListaAttivita = <ScriviListaAttivita>{
      attivita_list: listaAttivita,
      parametri_aggiuntivi_list: listParametri_Aggiuntivi_Attivita
    };

    this.agendaservice.Controlla_Giacenza_Fertilizzanti(ScriviListaAttivita).subscribe(rispostaStandard => {

      if (rispostaStandard.RispostaOK) {
        this.SalvaQdC(Tipo_Salvataggio_QdC, listaAttivita,listParametri_Aggiuntivi_Attivita);
      } else {
        this.qdcservice.gestisci_ErroriGias(rispostaStandard.ErroriGias, true, false).then((res: any) => {
          if (res) {
            switch (res.severity) {
              case ErroreGias_Severity.WarningBloccante:
                break;
              case ErroreGias_Severity.Warning:
                //Se l'utente ha cliccato su Sì allora eseguo il salvataggio saltando il warning a cui ha detto sì
                if (res.result.returnObj === true) {

                  for (let v of listParametri_Aggiuntivi_Attivita) {
                    if (v.key === Key_Parametri_Aggiuntivi_Attivita.CaricoMagazzinoAutomatico) {
                      v.value = "true";
                    }
                  }

                }

                this.SalvaQdC(Tipo_Salvataggio_QdC, listaAttivita, listParametri_Aggiuntivi_Attivita);
                break;
            }
          }
        });
      }

    });
  }

  /*
  * @description Forzo la chiusura del menù a tendina del bottone
  * perchè in alcuni casi non si chiude da solo in automatico
  * */
  private ChiudiDropDownButton(){

      if(this.MenuRicette && this.MenuRicette.isOpen)
        this.MenuRicette.toggle(false);

      if(this.BtnMenuVaiVisite && this.BtnMenuVaiVisite.isOpen)
        this.BtnMenuVaiVisite.toggle(false);

      if(this.BtnMenuSalvaVisite && this.BtnMenuSalvaVisite.isOpen)
        this.BtnMenuSalvaVisite.toggle(false);

  }

  ngOnDestroy() {

      this.ChiudiDropDownButton();

      this.Subs.unsubscribe();
  }
}
