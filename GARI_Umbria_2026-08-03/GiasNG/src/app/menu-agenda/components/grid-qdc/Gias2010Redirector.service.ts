import {Injectable} from '@angular/core';
import {
  Attivita,
  enum_Tipo_Operazione_Agenda_Target,
  Tipo_Ricetta
} from 'app/Model/attivita/Attivita';
import {enum_PagineAgenda_2010, Enum_SiteRedirector} from 'app/Model/siti.enum';
import {enum_LAVCOD, enum_PagineGiasNG} from 'app/Model/TipiEnumerativi';
import {AgendaService, LeggiLink_Operazione} from 'app/Service/Agenda/Agenda.service';
import {
  GestioneRichiesteService,
  KeyValuePair,
  ParametriAggiuntivi_QueryString
} from 'app/Service/gestione-richieste.service';
import { ObjParametriAgendaService} from 'app/Service/obj-parametri-agenda.service';
import {DateToString, NumToStr, StringToDate} from 'app/Service/utils';
import {Enum_DBTypeOperation} from 'gias-ui-kit';
import {RibaltamentoTypes, TabTypes} from '../utils';
import {GestioneMagazziniQS, GestioneMagazziniRedirectLink, InfomodificaOperazioneSingola} from '../models';
import {
  AGRODATAFINE,
  AGRODATAINIZIO,
  LinkQdCAngular,
  LinkVisiteEditAngular
} from 'app/Model/CostantiPersonalizzate';
import {
  CodiciXOperazione,
  DropdownListCampo,
  GridImpiantoSelezionatoModel,
  QdCFormModel
} from "../../../quaderno-di-campagna/agenda-edit/quaderno-di-campagna-form/quaderno-di-campagna-form.model";
import {Lavorazione} from 'app/Model/attivita/Lavorazione';
import {CentroAziendale, PKCentroAziendale} from "../../../Model/anagrafiche/CentroAziendale";
import {Redirect_To_GiasNG_Page} from "../../../quaderno-di-campagna/agenda-edit/service/qdc.service";
import {DestinazioneUso} from "../../../Model/metaschema/utilizzi/DestinazioneUso";
import {Campo, PKCampo} from "../../../Model/anagrafiche/Campo";
import {TranslocoService} from "@jsverse/transloco";
import {Specie} from "../../../Model/metaschema/utilizzi/Specie";
import {Impresa} from "../../../Model/anagrafiche/Impresa";
import {GiasIFrameWindowService} from 'gias-ui-kit';
import {AjaxAgronicaAPIService} from '../../../Service/ajax-agronica.api.service';
import {concatMap, map, Observable, of, take} from 'rxjs';
import {GiasDialogService} from '../../../Service/gias-dialog.service';
import {MasterService} from "../../../Service/master.service";
import {PermessiUtenteService} from "../../../Service/permessi-utente.service";
import {ConfigurazioneSitiService} from "../../../Service/configurazione-siti.service";
import { ObjParametriAgenda, Stati, Tipo_Attivita } from 'gias-ui-kit';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';

@Injectable({ providedIn: "root" })
export class Gias2010Redirector {

    constructor(
        private agenda: ObjParametriAgendaService,
        private gestioneRichieste: GestioneRichiesteService,
        private APIService: AjaxAgronicaAPIService,
        private windowService: GiasIFrameWindowService,
        private dialogService: GiasDialogService,
        private translocoService: TranslocoService,
        private agendaservice: AgendaService,
        private ajaxAgronicaAPIService: AjaxAgronicaAPIService,
        private masterService: MasterService,
        private permessiUtenteService: PermessiUtenteService,
        private configurazioneSitiService: ConfigurazioneSitiService) { }

    /**
     * Reindirizza in MenuAgenda di Gias2010.
     * @param link ricavato dalla chiamata http infomodifica_operazione_singola.
     */
     public async reindirizza(link: string, agenda: ObjParametriAgenda): Promise<void> {

        const paginaRichiesta = parseLink(link);
        let queryParams = [];

        switch (paginaRichiesta) {
          case enum_PagineAgenda_2010.Pagina_Gestione_Costi:
            queryParams = GestioneCostiQueryParams(agenda, agenda.TipoOperazioneDB, agenda.Pagina_Provenienza);
            break;
          case enum_PagineAgenda_2010.Pagina_Liquidazione_soci:
            break;
          case enum_PagineAgenda_2010.Pagina_DocContabile:
            queryParams = this.docContabileParametri(agenda.Piva, agenda.TipoOperazioneDB, agenda.Sa_Cod,agenda.Id_Agenda,
              agenda.Lav_Cod,0,"","",new Date(),0,0,"false");
            break;
          case enum_PagineAgenda_2010.Pagina_FormProdotto:
            queryParams = await this.CaricaFormProdottoParametri(agenda, agenda.Pagina_Provenienza);
            break;
          case enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico:
            queryParams = DocumentoContabileGenericoParams(agenda);
            break;
          case enum_PagineAgenda_2010.Pagina_Fertilizzazione:
          case enum_PagineAgenda_2010.Pagina_Trattamenti:
          case enum_PagineAgenda_2010.Pagina_Distribuzione_Insetti:
          case enum_PagineAgenda_2010.Pagina_Lavorazioni:
          case enum_PagineAgenda_2010.Pagina_Installazione_Trapppole:
          case enum_PagineAgenda_2010.Pagina_Reinnesco_Trapppole:
          case enum_PagineAgenda_2010.Pagina_Rilievi:
          case enum_PagineAgenda_2010.Pagina_RilievoPiogge:
          case enum_PagineAgenda_2010.Pagina_Irrigazione:
          case enum_PagineAgenda_2010.Pagina_Semina_Trapianto:
            break;
          case enum_PagineAgenda_2010.Pagina_IrrigazioneBS:
            queryParams = this.IrrigazioneBSParametri(agenda);
            break;
        }

        if(agenda.Pagina_Provenienza === enum_PagineGiasNG.Pagina_Edit_Attivita) {
          queryParams = this.ImpostaQueryParamsQdC(agenda,queryParams);
        }

        this.VaiInAgenda(paginaRichiesta, agenda, queryParams)

    }

    VaiInAgenda(pagina: enum_PagineAgenda_2010, agenda: ObjParametriAgenda, queryParams: ParametriAggiuntivi_QueryString[]) {
        if (this.isPaginaPortataDomandaIrrigua(pagina)) {
            this.vaiInDomandaIrrigua(pagina, queryParams, agenda);
        } else {
            this.vaiInAgenda2010(pagina, queryParams, agenda);
        }
    }

    private isPaginaPortataDomandaIrrigua(pagina: enum_PagineAgenda_2010): boolean {
        return [
            enum_PagineAgenda_2010.Pagina_Trattamenti_B,
            enum_PagineAgenda_2010.Pagina_Installazione_Trapppole,
            enum_PagineAgenda_2010.Pagina_Reinnesco_Trapppole,
            enum_PagineAgenda_2010.Pagina_RilieviBS,
            enum_PagineAgenda_2010.Pagina_Raccolta,
            enum_PagineAgenda_2010.Pagina_TrattamentiPostRaccolta,
            enum_PagineAgenda_2010.Pagina_Distribuzione_Insetti,
        ].includes(pagina);
    }

    private vaiInDomandaIrrigua(paginaRichiesta: enum_PagineAgenda_2010, parametri: ParametriAggiuntivi_QueryString[],
        objAgenda: ObjParametriAgenda) {

        this.masterService.set_isLoading({
            isLoading: true,
            message: 'Caricamento in corso'
        });

        this.gestioneRichieste.gestionePassaggioAltroSito(
            Enum_SiteRedirector.Sito_AgronicaDomandaIrrigua,
            paginaRichiesta,
            parametri,
            objAgenda).then(resp => {
                window.location.href = resp

                window.onload = ()=>{
                    this.masterService.set_isLoading({
                        isLoading: false,
                        message: 'Caricamento in corso'
                    });
                }
        });

    }


    private ImpostaQueryParamsQdC(agenda: ObjParametriAgenda, queryParams: ParametriAggiuntivi_QueryString[]) {

        if(agenda.Ricetta_Cod > 0) {
            queryParams.push({
                key: "r",
                value: agenda.Ricetta_Cod.toString(),
                codifica: true
            });
        }

        return queryParams;

    }

    private CaricaFormProdottoParametri( agenda: ObjParametriAgenda, Pagina_Provenienza: number): Promise<ParametriAggiuntivi_QueryString[]> {
        return new Promise((resolve, reject) => {
            const data = () => {
                return {
                    lav_cod: agenda.Lav_Cod,
                    piva: agenda.Piva,
                    sa_cod: agenda.Sa_Cod
                }
            }

            this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(GestioneMagazziniRedirectLink, data()).pipe(map(resp => {
                const risposta: GestioneMagazziniQS = resp.RispostaStringa;
                const parametri: ParametriAggiuntivi_QueryString[] = formProdottoParametri(agenda, risposta,Pagina_Provenienza,agenda.TipoOperazioneDB);
                resolve(parametri);
            })).subscribe();
        });
    }


    public getAgenda(dataItem: any, Pagina_Provenienza: number,tipoOperazione: number): ObjParametriAgenda {

        const agenda = this.agenda.getObjParamValue();

        if(dataItem instanceof LeggiLink_Operazione){

          if(dataItem.impresa)
            agenda.Piva = dataItem.impresa.partitaIva;

          agenda.Data = dataItem.data;

          agenda.Id_Agenda = dataItem.id_agenda === "" ? 0 : + dataItem.id_agenda;

          if(dataItem.specie)
            agenda.Veg_Cod = dataItem.specie.codice;

          if(dataItem.centroaziendale)
            agenda.Sa_Cod = dataItem.centroaziendale.primaryKey.codice;

          if(dataItem.lavorazione)
            agenda.Lav_Cod = + dataItem.lavorazione.primaryKey.codice;

          agenda.TipoOperazioneAgenda = dataItem.tipo;
          agenda.TipoRicetta = dataItem.tiporicetta;
          agenda.Stato = dataItem.stato;
          agenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale;
          agenda.Programmazione_Cod = 0;

          // Imposto la pagina di provenienza a Menu Agenda se provengono dal QdC così nel redirect torna
          // al menu Agenda
          if(Pagina_Provenienza === enum_PagineGiasNG.Pagina_Edit_Attivita) {
            agenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
          } else {
            agenda.Pagina_Provenienza = Pagina_Provenienza;
          }

          agenda.TipoOperazioneDB = tipoOperazione;
        }else{
          const data = dataItem.chiave_composita.split("_")[0];
          const id_agenda = dataItem.chiave_composita.split("_")[1];
          const lav_cod = dataItem.chiave_composita.split("_")[2];



          agenda.Piva = dataItem.Piva;
          agenda.Data = StringToDate(data);
          agenda.Id_Agenda = + id_agenda;
          agenda.Sa_Cod = 0;
          agenda.Lav_Cod = + lav_cod;
          agenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
          agenda.TargetOperazione = enum_Tipo_Operazione_Agenda_Target.Reale;
          agenda.Programmazione_Cod = 0;
          agenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;

          agenda.TipoOperazioneDB = tipoOperazione;
        }

        return agenda;
    }


    gestisciRedirectToQdC(objParametriAgenda: ObjParametriAgenda,tipo_di_ribaltamento: RibaltamentoTypes = RibaltamentoTypes.Nessuno, flag_reinnesco:boolean = false): Promise<boolean> {

        return new Promise<boolean>(async(resolve,reject)=> {
            if(objParametriAgenda) {

                //Imposto come IdSezione Operazioni Colturali NG
                objParametriAgenda.IdSezione = 260;

                this.agenda.changeObjParametriAgenda(objParametriAgenda);

                const queryParams: { [key:string] : string | string[] } = {"t_r":tipo_di_ribaltamento.toString(),"f_r": flag_reinnesco ? "1" : "0"};

                const lavorazione = new Lavorazione(objParametriAgenda.Lav_Cod.toString());

                lavorazione.descrizione = objParametriAgenda.Lav_Des;

                const centro = new CentroAziendale({codice: objParametriAgenda.Sa_Cod, partitaIva: objParametriAgenda.Piva} as PKCentroAziendale);

                const leggiLink_Operazione = new LeggiLink_Operazione;

                leggiLink_Operazione.id_agenda = objParametriAgenda.Id_Agenda.toString();
                leggiLink_Operazione.codiceOperazioneRicetta = objParametriAgenda.Ricetta_Operazione_Cod.toString();
                leggiLink_Operazione.codiceRicetta =  objParametriAgenda.Ricetta_Cod.toString();

                leggiLink_Operazione.tipo_operazione = objParametriAgenda.TipoOperazioneDB;

                leggiLink_Operazione.impresa = <Impresa>{
                    partitaIva: objParametriAgenda.Piva,
                    ragioneSociale: objParametriAgenda.RagSoc
                };

                leggiLink_Operazione.centroaziendale = centro;

                leggiLink_Operazione.specie = <Specie>{
                    codice: objParametriAgenda.Veg_Cod,
                    descrizione: objParametriAgenda.Veg_Des
                };

                leggiLink_Operazione.data = objParametriAgenda.Data;

                leggiLink_Operazione.lavorazione = lavorazione;

                leggiLink_Operazione.tipo = objParametriAgenda.TipoOperazioneAgenda;

                leggiLink_Operazione.tiporicetta = objParametriAgenda.TipoRicetta;

                leggiLink_Operazione.stato = objParametriAgenda.Stato as Stati;

                const link = await this.agendaservice.Redirect_In_Base_Al_Lav_Cod(leggiLink_Operazione);

                switch(link){
                  case LinkQdCAngular:
                  case LinkVisiteEditAngular:

                    if(link === LinkQdCAngular && objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write && objParametriAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna && objParametriAgenda.Piva !== "") {

                      this.gestioneRichieste.ObjPageToMemorize = this.setQdCFormModelforRedirect(objParametriAgenda,tipo_di_ribaltamento);

                      let resp = await this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Edit_Attivita);

                      this.agenda.navigateTo(resp, queryParams,null,false);

                      resolve(true);
                    }else{

                      let resp = await this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Edit_Attivita);

                      if(link === LinkVisiteEditAngular)
                        resp = await this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.Pagina_Edit_Visite);

                      this.agenda.navigateTo(resp, queryParams,null,false);

                      resolve(true);

                    }

                    break;
                  default:

                    await this.reindirizza(link,this.getAgenda(leggiLink_Operazione,objParametriAgenda.Pagina_Provenienza,objParametriAgenda.TipoOperazioneDB));

                    resolve(true);
                    break;
                }

            } else {
                resolve(true);
            }
        });




    }

    private vaiInAgenda2010(paginaRichiesta: enum_PagineAgenda_2010, parametri: ParametriAggiuntivi_QueryString[],
        objAgenda: ObjParametriAgenda) {

        this.masterService.set_isLoading({
            isLoading: true,
            message: 'Caricamento in corso'
        });

        this.gestioneRichieste.gestionePassaggioAltroSito(
            Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
            paginaRichiesta,
            parametri,
            objAgenda).then(resp => {
                window.location.href = resp

                window.onload = ()=>{
                    this.masterService.set_isLoading({
                        isLoading: false,
                        message: 'Caricamento in corso'
                    });
                }
        });

    }


    public reindirizzaAiDettagli(action: string, dataItem: any) {
        const ObjParametriAgenda = this.agenda.getObjParamValue();
        let tipoOperazione: Enum_DBTypeOperation = null;
        if (action === 'info') {
          tipoOperazione = Enum_DBTypeOperation.Read;
        } else if (action === 'edit') {
          tipoOperazione = Enum_DBTypeOperation.Update;
        }
        this.OttieniLinkPaginaDiModifica(dataItem, tipoOperazione)
            .pipe(take(1))
            .subscribe(link => {
                if (DEBUGGING_MODE || link !== LinkQdCAngular){
                  this.reindirizza(link, this.getAgenda(dataItem, tipoOperazione,tipoOperazione));
                }
                else{
                  this.redirectToQdCfromMenuAgenda(
                    + dataItem.id_agenda, 0,
                    ObjParametriAgenda.Piva, ObjParametriAgenda.RagSoc,
                    + dataItem.Lav_cod, dataItem.Lav_Des,
                    0, tipoOperazione, TabTypes.QuadernoDiCampagna /* TODO(RV) this might need to change */,
                    dataItem.Data2, RibaltamentoTypes.Nessuno, ObjParametriAgenda.IdSezione
                  );
                }
            });
    }

    private getParametriInfoModifica(datiRiga: any, tipoOperazioneDB: number | Enum_DBTypeOperation) {
        const p = new InfomodificaOperazioneSingola();
        p.tipo = tipoOperazioneDB;
        p.dataOp = datiRiga.chiave_composita.split('_')[0];
        p.id_agenda = datiRiga.chiave_composita.split("_")[1];
        p.lav_cod = datiRiga.chiave_composita.split("_")[2];
        p.blocco_flag = datiRiga.chiave_composita.split("_")[5];
        p.veg_cod = datiRiga.chiave_composita.split("_")[6];
        p.veg_cod = p.veg_cod ? p.veg_cod : "0" ;
        p.piva = this.agenda.getObjParamValue().Piva;
        p.variabiliInSessione_NG = this.masterService.variabiliInSessione;
        return p;
    }

    private OttieniLinkPaginaDiModifica(datiRiga: any, tipoOperazioneDB: number | Enum_DBTypeOperation): Observable<string> {
        return this.APIService.ajaxAPIPost<InfomodificaOperazioneSingola, any>(
            'Agenda/infomodifica_operazione_singola_new',
            this.getParametriInfoModifica(datiRiga, tipoOperazioneDB),
            false, true, false, false
        ).pipe(map(R => {
            if (!R.RispostaOK) {
                this.dialogService.baseError('', R.Errore, false)
            }
            return R.RispostaOK ? R.RispostaStringa : "";
        }));
    }


    /**
     *  Gestisce il Redirect alla pagina del Quaderno di Campagna dal Menu Agenda
     */
    redirectToQdCfromMenuAgenda(Id_Agenda: number, Ricetta_Operazione_Cod: number, Piva: string, RagSoc: string,
        Lav_Cod: number, Lav_Des: string, tipo_ricetta: Tipo_Ricetta, TipoOperazioneDB: Enum_DBTypeOperation,
        Tipo: TabTypes, Data: Date, tipo_di_ribaltamento: RibaltamentoTypes = RibaltamentoTypes.Nessuno, idSezione: number = 0,
        flag_reinnesco: boolean = false) {

        // Gestisce il Redirect alla pagina del Quaderno di Campagna

        const objAgenda: ObjParametriAgenda = this.agenda.resettaObjAgenda(new ObjParametriAgenda);

        let Pagina_Provenienza: number = 0;

        let Pagina_Richiesta: number = 0;

        //faccio in modo che se passo dalle Visite, la pagina di provenienza sia la menu visite
        if (Lav_Cod === enum_LAVCOD.VISITA){
          Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Visite;
          Pagina_Richiesta = enum_PagineGiasNG.Pagina_Edit_Visite;
        }else{

          //Se sto facendo un reinnesco imposto la pagina di provenienza giusta per il redirect
          if(flag_reinnesco){
            Pagina_Provenienza = enum_PagineGiasNG.Pagina_Scadenza_Reinnesco_Trappole;
          }else{
            Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
          }

          Pagina_Richiesta = enum_PagineGiasNG.Pagina_Edit_Attivita;
        }


        objAgenda.Piva = Piva;
        objAgenda.RagSoc = RagSoc;
        objAgenda.Lav_Cod = Lav_Cod;
        objAgenda.Lav_Des = Lav_Des;
        objAgenda.Data = Data;
        objAgenda.Sito_Provenienza = Enum_SiteRedirector.GiasNG;
        objAgenda.Pagina_Provenienza = Pagina_Provenienza;
        objAgenda.Pagina_Richiesta = Pagina_Richiesta;

        objAgenda.IdSezione = idSezione;

        switch(tipo_di_ribaltamento) {
            case RibaltamentoTypes.Nessuno:
                objAgenda.TipoOperazioneDB = TipoOperazioneDB;

                switch (Tipo) {
                  case TabTypes.QuadernoDiCampagna:
                  case TabTypes.GestioneTrappole:
                        objAgenda.Id_Agenda = Id_Agenda;
                        objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
                        break;
                    case TabTypes.Ricette:
                        objAgenda.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod;
                        objAgenda.TipoOperazioneAgenda = Tipo_Attivita.Ricetta;
                        objAgenda.TipoRicetta = tipo_ricetta;
                        objAgenda.Stato = Stati.Da_Eseguire;
                        break;
                    case TabTypes.Brogliaccio:
                        objAgenda.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod;
                        objAgenda.TipoOperazioneAgenda = Tipo_Attivita.Ricetta;
                        objAgenda.TipoRicetta = tipo_ricetta;
                        objAgenda.Stato = Stati.Eseguita;
                        break;
                }
                break;
            case RibaltamentoTypes.Da_Brogliaccio_ad_Agenda:
                // TODO aggiungere il Programmazione_Cod il Target_Operazione non è più utilizzato invece
                objAgenda.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod;
                objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
                objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                break;
            case RibaltamentoTypes.Da_Ricetta_a_Brogliaccio:
                // TODO da finire di sviluppare
                objAgenda.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod;
                objAgenda.TipoOperazioneAgenda = Tipo_Attivita.Ricetta;
                objAgenda.TipoRicetta = Tipo_Ricetta.Standard_Destinazioni;
                objAgenda.Stato = Stati.Eseguita;
                objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                break;
            case RibaltamentoTypes.Da_Ricetta_ad_Agenda:
                // TODO da finire di sviluppare
                objAgenda.Ricetta_Operazione_Cod = Ricetta_Operazione_Cod;
                objAgenda.TipoOperazioneAgenda = Tipo_Attivita.QuadernoDiCampagna;
                objAgenda.TipoOperazioneDB = Enum_DBTypeOperation.Write;
                break;
        }

        const queryParams: { [key:string] : string | string[] } = {"t_r":tipo_di_ribaltamento.toString(),"f_r": flag_reinnesco ? "1" : "0"};

        if(objAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write && objAgenda.TipoOperazioneAgenda === Tipo_Attivita.QuadernoDiCampagna && objAgenda.Piva !== ""){

          this.gestioneRichieste.getPathFromPagina_Richiesta(objAgenda.Pagina_Richiesta).then(link=>{
            this.agenda.navigateTo(link, queryParams,objAgenda,false);
          });
        }else{

          this.gestioneRichieste.getPathFromPagina_Richiesta(objAgenda.Pagina_Richiesta).then(link=>{
            this.agenda.navigateTo(link, queryParams,objAgenda,false);
          });
        }
    }

    redirectToQdCFromDashboard(Id_Agenda: number, Piva: string, RagSoc: string, Lav_Cod: number, TipoOperazioneDB: Enum_DBTypeOperation, Data: Date): void {
        // Change IdSezione for breadcrumbs, this code will hopefully be removed in the future
        this.redirectToQdCfromMenuAgenda(Id_Agenda, 0, Piva, RagSoc, Lav_Cod, " ", null, TipoOperazioneDB, TabTypes.QuadernoDiCampagna, Data, RibaltamentoTypes.Nessuno, 260);
    }

    setQdCFormModelforRedirect(objParametriAgenda: ObjParametriAgenda,tipo_ribaltamento: RibaltamentoTypes): Redirect_To_GiasNG_Page {

        let ObjPageToMemorize = null;

        if(objParametriAgenda) {

            // Per aprire la pagina in scrittura con dei Dati già preimpostati valorizzo il QdCFormModel altrimenti
            // basta valorizzare l'ObjParametriAgenda per la modifica

            if(objParametriAgenda.TipoOperazioneDB === Enum_DBTypeOperation.Write && tipo_ribaltamento === RibaltamentoTypes.Nessuno) {
                const lavorazione = new Lavorazione(objParametriAgenda.Lav_Cod.toString());

                lavorazione.descrizione = objParametriAgenda.Lav_Des;

                const centro = new CentroAziendale({codice: objParametriAgenda.Sa_Cod, partitaIva: objParametriAgenda.Piva} as PKCentroAziendale)

                centro.nome = objParametriAgenda.SaNome;

                const qdCFormModel=new QdCFormModel();

                qdCFormModel.Trattamento.Testata.Codici_Attivita.push(new CodiciXOperazione("0","0","0", lavorazione,null,null,"", "0",null));

                qdCFormModel.Trattamento.Testata.Operazioni.push(lavorazione);

                qdCFormModel.Trattamento.Testata.MultiCentro = false;

                qdCFormModel.Trattamento.Testata.Data = objParametriAgenda.Data;

                qdCFormModel.Trattamento.Testata.Raccoglitore=0;

                qdCFormModel.Trattamento.Testata.Tipo=objParametriAgenda.TipoOperazioneAgenda;

                qdCFormModel.Trattamento.Testata.TipoRicetta=objParametriAgenda.TipoRicetta;

                qdCFormModel.Trattamento.Testata.Stato=objParametriAgenda.Stato as Stati;

                qdCFormModel.Trattamento.Testata.Centro_Aziendale=centro;

                qdCFormModel.Trattamento.Testata.Attivita_Personalizzata = null;

                qdCFormModel.Trattamento.Testata.Descrizione_Altre_Lavorazioni = "";

                qdCFormModel.Trattamento.Testata.Disciplinare = null;

                qdCFormModel.Trattamento.Testata.Visualizza_Solo_Operazioni_Preferite = false;

                if(objParametriAgenda.Veg_Cod !== 0 && objParametriAgenda.Veg_Des !== "") {
                    qdCFormModel.Trattamento.Testata.Specie = new Specie(objParametriAgenda.Veg_Cod);

                    qdCFormModel.Trattamento.Testata.Specie.descrizione = objParametriAgenda.Veg_Des;
                } else if(objParametriAgenda.Id_Cod !== 0 && objParametriAgenda.Id_Des !== "") {
                    qdCFormModel.Trattamento.Testata.Specie = new DestinazioneUso();

                    qdCFormModel.Trattamento.Testata.Specie.codice = objParametriAgenda.Id_Cod;

                    qdCFormModel.Trattamento.Testata.Specie.descrizione = objParametriAgenda.Id_Des;
                }

                const campo = new Campo({centroAziendalePK: qdCFormModel.Trattamento.Testata.Centro_Aziendale.primaryKey, codice: 0} as PKCampo);

                campo.descrizione = this.translocoService.translate("Tutti");

                const ddl_campo: DropdownListCampo = campo as DropdownListCampo;

                ddl_campo.Codice_Concatenato = ddl_campo.primaryKey.codice +"£"+ddl_campo.primaryKey.centroAziendalePK.codice +"£"+ddl_campo.primaryKey.centroAziendalePK.partitaIva;

                ddl_campo.Descrizione_Concatenata = ddl_campo.descrizione;

                qdCFormModel.Trattamento.Testata.Campo = ddl_campo;

                qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati = [];

                if(objParametriAgenda.Impianti && objParametriAgenda.Impianti.length > 0) {
                    objParametriAgenda.Impianti.forEach(i=> {

                        let Sup_Imp_help: number = i.Sup_Imp;

                        let Sup_Riduzione_BufferZone: number = 0;

                        let Perc_Riduzione_Deriva: number = 0;

                        if(i.Sup_Imp_help !== null && i.Sup_Imp_help !== undefined && i.Sup_Imp_help > 0 && i.Sup_Imp_help <= i.Sup_Imp)
                            Sup_Imp_help = i.Sup_Imp_help;

                        if(i.Sup_Riduzione_BufferZone !== null && i.Sup_Riduzione_BufferZone !== undefined && i.Sup_Riduzione_BufferZone > 0)
                            Sup_Riduzione_BufferZone = i.Sup_Riduzione_BufferZone;

                        if(i.Perc_Riduzione_Deriva !== null && i.Perc_Riduzione_Deriva !== undefined && i.Perc_Riduzione_Deriva > 0)
                            Perc_Riduzione_Deriva = i.Perc_Riduzione_Deriva;

                        qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati.push(new GridImpiantoSelezionatoModel(i.Piva,
                            i.Sa_Cod,
                            i.Appezza,
                            i.Id_Reg,
                            i.Progetto_Cod,
                            0,"",0,"",0,Sup_Imp_help,
                            Sup_Riduzione_BufferZone,
                            Perc_Riduzione_Deriva,0,"",[],
                            AGRODATAINIZIO,AGRODATAFINE,"",null,0,
                            AGRODATAINIZIO, AGRODATAINIZIO,"",null,i.Sup_Imp,0,0,0,0,0,"",""));
                    });
                } else {
                    qdCFormModel.Trattamento.Impianti.ImpiantiSelezionati = [];
                }

                ObjPageToMemorize = new Redirect_To_GiasNG_Page();

                ObjPageToMemorize.objParametriAgenda = objParametriAgenda;

                ObjPageToMemorize.QdCFormValue = qdCFormModel;

            }

        }


        return ObjPageToMemorize;

    }

     docContabileParametri(Piva:string,TipoOperazione:number,Sa_cod:number,Id_agenda:number,Lav_Cod: number,md:number,ricercatype:string,ricercadoc:string,
                           data: Date,prodotto_cod: number, ifr:number,apertodaGiasNG:string) {


        return [
            // KeyValuePair('orig', ...)
            KeyValuePair.Create('p', Piva,true),
            KeyValuePair.Create('o', NumToStr(TipoOperazione),true),
            KeyValuePair.Create('s', NumToStr(Sa_cod),true),
            KeyValuePair.Create('i', NumToStr(Id_agenda),true),
            KeyValuePair.Create('l', NumToStr(Lav_Cod),true),
            KeyValuePair.Create('md', NumToStr(md),true),
            KeyValuePair.Create('ricercatype', ricercatype,true),
            KeyValuePair.Create('ricercadoc', ricercadoc,true),
            KeyValuePair.Create('d', data.toLocaleString(),true),
            KeyValuePair.Create('prodotto_cod', NumToStr(prodotto_cod),true),
            KeyValuePair.Create('ifr', NumToStr(ifr),true),
            KeyValuePair.Create('apertodaGiasNG', apertodaGiasNG,true)
        ]
    }

    private IrrigazioneBSParametri(agenda: ObjParametriAgenda){
      return [
        KeyValuePair.Create('operazione_ricetta', NumToStr(agenda.Ricetta_Operazione_Cod),true),
        KeyValuePair.Create('r', NumToStr(agenda.Ricetta_Cod),true)
      ]
    }

    redirectTo_Verifica_Conformita(attivita: Attivita, paginaProvenienza: enum_PagineGiasNG) {
        // if (!attivita) {

            let impostazione = this.permessiUtenteService.getImpostazione_Utente(enum_Impostazioni_Utenti.SUPERUSER_ModalitaVerificaConformita);
            if (impostazione.Valore === "1"){

              let objParamAgenda  = this.agenda.getObjParamValue();
              objParamAgenda.Pagina_Provenienza = paginaProvenienza;

              this.gestioneRichieste.gestionePassaggioAltroSito(
                  Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
                  enum_PagineAgenda_2010.Pagina_Verifica_Conformita,
                  null, objParamAgenda
              ).then(link => window.location = link );

            } else if (impostazione.Valore === "2"){

              this.gestioneRichieste.gestionePassaggioStessoSito(enum_PagineGiasNG.GestioneDisciplinari_Verifica_Disciplinare).then(link => {
                let queryParams: { [key:string] : string | string[] } = {};

                this.agenda.navigateTo(link, queryParams,null,false);
              });

            } else {
              throw new Error("Setting value not implemented");
            }
            return;
        // }

        // // TODO: capire come fare con il multi operazione
        // const objParametriAgenda = this.agenda.getObjParamValue();
        // const newobjParametriAgenda: ObjParametriAgenda = JSON.parse(JSON.stringify(objParametriAgenda));
        //
        // newobjParametriAgenda.Id_Agenda = 0;
        // newobjParametriAgenda.Pagina_Provenienza = enum_PagineGiasNG.Pagina_Menu_Agenda;
        //
        // newobjParametriAgenda.GenericObj_string = JSON.stringify(attivita);
        //
        // const ParametriAggiuntivi: Array<ParametriAggiuntivi_QueryString> = [];
        //
        // if(attivita && attivita.disciplinare.codice === NessunDpiNessunaEtichetta) { return; }
        //
        // this.gestioneRichieste.gestionePassaggioAltroSito(
        //     Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
        //     enum_PagineAgenda_2010.Pagina_Verifica_Conformita,
        //     ParametriAggiuntivi,
        //     newobjParametriAgenda
        // ).then(link => window.location = link );
    }

    CheckServizioQDC(Piva: string,Data: Date,tipo: Tipo_Attivita): Observable<boolean>{
      if(tipo === Tipo_Attivita.QuadernoDiCampagna){
        return this.configurazioneSitiService.leggiChiave('Verifica_Sottoscrizione_Servizio_QDC').pipe(
          concatMap(config=>{
            if(config && config.Valore && config.Valore.toLowerCase() == 'true'){
              return this.permessiUtenteService.CheckUtenteTipologiaAccessoQDC(Piva,Data).pipe(map((risp)=>{
                if(!risp){
                  this.dialogService.errorPromise(
                    "",
                    "PassaggioAllePraticheInserimentoOpNonEffettuato",
                    true
                  ).then();
                }

                return risp;
              }));
            }else{
              return of(true);
            }
        }));



        /*this.configurazioneSitiService.leggiChiave('Verifica_Sottoscrizione_Servizio_QDC').pipe(map(config=>{
          if(config && config.Valore == '1'){
            return true;
          }else{
            return true;
          }
        }));*/

        /*return this.permessiUtenteService.CheckUtenteTipologiaAccessoQDC(Piva,Data).pipe(map((risp)=>{
          if(!risp){
            this.dialogService.errorPromise(
              "",
              "PassaggioAllePraticheInserimentoOpNonEffettuato",
              true
            ).then();
          }

          return risp;
        }),take(1));*/
      }else{
        return new Observable<boolean>(obs=>{
          obs.next(true);
        });
      }
    }
}


export const DEBUGGING_MODE = false;
const LAVCOD_CORRENTE = enum_PagineAgenda_2010.Pagina_RilieviBS;

function parseLink(link: string): enum_PagineAgenda_2010 {

    if (DEBUGGING_MODE) {
        return LAVCOD_CORRENTE;
    }

    if (link.includes("../AnalisiCostiProduzione/GestioneCosti.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Gestione_Costi;
    }

    if (link.includes("../GestioneContabilita/Liquidazione/Liquidazione.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Liquidazione_soci;
    }

    if (link.includes("/GestioneContabilita/DocContabile.aspx")) {
        return enum_PagineAgenda_2010.Pagina_DocContabile;
    }

    if (link.includes("/GestioneMagazzini/FormProdotto.aspx")) {
        return enum_PagineAgenda_2010.Pagina_FormProdotto;
    }

    if (link.includes("/GestioneContabilita/DocumentoContabileGenerico.aspx")) {
        return enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico;
    }

    if (link.includes("/Operazioni/Fertilizzazione.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Fertilizzazione;
    }

    if (link.includes("/Operazioni/Trattamenti.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Trattamenti;
    }

    if (link.includes("/Operazioni/Distribuzione_Insetti.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Distribuzione_Insetti;
    }

    if (link.includes("/Operazioni/Lavorazioni.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Lavorazioni;
    }

    if (link.includes("/Operazioni/Installazione_Trappole.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Installazione_Trapppole;
    }

    if (link.includes("/Operazioni/Reinnesco_Rilievi_Trappole.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Reinnesco_Trapppole
    }

    if (link.includes("/Operazioni/Rilievi.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Rilievi;
    }

    if (link.includes("/Operazioni/RilieviBS.aspx")) {
        return enum_PagineAgenda_2010.Pagina_RilieviBS;
    }

    if (link.includes("/Operazioni/Irrigazione.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Irrigazione;
    }

    if (link.includes("/Operazioni/IrrigazioneBS.aspx")) {
        return enum_PagineAgenda_2010.Pagina_IrrigazioneBS;
    }

    if (link.includes("/Operazioni/RilievoPiogge.aspx")) {
        return enum_PagineAgenda_2010.Pagina_RilievoPiogge;
    }

    if (link.includes("/Operazioni/Semina_E_Trapianto_1.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Semina_Trapianto;
    }

    // Lav cod raccolta
    if (link.includes("/Operazioni/Trattamenti_2.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Trattamenti_B;
    }

    if (link.includes("/Operazioni/Raccolta.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Raccolta;
    }

    if (link.includes("/Operazioni/Trattamenti_PostRaccolta.aspx")) {
        return enum_PagineAgenda_2010.Pagina_TrattamentiPostRaccolta;
    }

    if (link.includes("/GestioneMagazzini/OperazioneDiCura.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Operazione_Di_Cura;
    }

    if (link.includes("/Operazioni/ManutenzioneMacchine.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Manutenzione_Macchine;
    }

    if (link.includes("/Operazioni/GestioneRifiuti.aspx")) {
        return enum_PagineAgenda_2010.Pagina_GestioneRifiuti;
    }

    if (link.includes("/Zoo/Zoo_Carico.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Zoo_Carico;
    }

    if (link.includes("/Zoo/Zoo_Spostamento.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Zoo_Spostamento;
    }

    if (link.includes("/Zoo/Zoo_Alimentazione.aspx")) {
        return enum_PagineAgenda_2010.Pagina_ZooAlimentazione;
    }

    if (link.includes("/Zoo/Zoo_Pesatura.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Zoo_Pesatura;
    }

    if (link.includes("/Zoo/Zoo_Scarico.aspx")) {
        return enum_PagineAgenda_2010.Pagina_Zoo_Scarico;
    }

    if (link.includes("/Zoo/Zoo_Altre_Lavorazioni.aspx")) {
        return enum_PagineAgenda_2010.Pagina_ZooAltreLavorazioni
    }

    throw Error("Link non gestito: " + link);
    // Passo 1
    // if(link.includes("/GestioneContabilita/DocumentoContabileGenerico.aspx"))
    //    return enum_PagineAgenda_2010.Pagina_DocumentoContabileGenerico; // 4, 5

}


function GestioneCostiQueryParams(agenda: ObjParametriAgenda, tipoOperazione: number, Pagina_Provenienza: number): ParametriAggiuntivi_QueryString[] {

  return [
    KeyValuePair.Create('p', agenda.Piva,true),
    KeyValuePair.Create('id_agenda', NumToStr(agenda.Id_Agenda),true),
    KeyValuePair.Create('entrata_diretta', NumToStr(0),true),
    KeyValuePair.Create('op', NumToStr(tipoOperazione),true)
  ]

}

const formProdottoParametri = (agenda: ObjParametriAgenda, qsDto: GestioneMagazziniQS, Pagina_Provenienza: number,tipoOperazione: number): ParametriAggiuntivi_QueryString[] => {
    const result: ParametriAggiuntivi_QueryString[] = [];

    switch(Pagina_Provenienza) {
        case enum_PagineGiasNG.Pagina_Menu_Agenda:
        case enum_PagineGiasNG.Pagina_Edit_Attivita:
            return [
                // KeyValuePair('orig', paginaSitoAgendaOrigine)
                KeyValuePair.Create('k', qsDto.k,true),
                KeyValuePair.Create('c', qsDto.c,true),
                KeyValuePair.Create('o', agenda.TipoOperazioneDB.toString(),true),
                KeyValuePair.Create('mode', qsDto.mode,true),
                KeyValuePair.Create('l', NumToStr(agenda.Lav_Cod),true),
                KeyValuePair.Create('d', DateToString(agenda.Data),true), // da verificare
                KeyValuePair.Create('s', NumToStr(agenda.Sa_Cod),true),
                KeyValuePair.Create('a', NumToStr(agenda.Id_Agenda),true)
            ]

    }

    return result;
}

function DocumentoContabileGenericoParams(agenda: ObjParametriAgenda): ParametriAggiuntivi_QueryString[] {
    const result: ParametriAggiuntivi_QueryString[] = [
        KeyValuePair.Create('p', agenda.Piva,true),
        KeyValuePair.Create('o', agenda.TipoOperazioneDB.toString(),true),
        KeyValuePair.Create('s', NumToStr(0),true),
        KeyValuePair.Create('i', NumToStr(agenda.Id_Agenda),true),
        KeyValuePair.Create('l', NumToStr(agenda.Lav_Cod),true),
        KeyValuePair.Create('d', DateToString(agenda.Data),true), // da verificare
        KeyValuePair.Create('tf', NumToStr(0),true),
        KeyValuePair.Create('rs', agenda.RagSoc,true),  // da verificare
        // KeyValuePair('orig', agenda.RagSoc), // ommesso
    ];
    return result;
}

