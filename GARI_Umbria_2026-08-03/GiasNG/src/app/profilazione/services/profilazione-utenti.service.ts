import {MasterService, RispostaStandard} from "app/Service/master.service";
import {BehaviorSubject, filter, map, Observable, of, Subject, take} from "rxjs";
import {Injectable} from "@angular/core";
import {enum_TipoOperazioneDB} from "app/Model/TipiEnumerativi";
import {BaseCodeDescr} from "app/Model/baseClass/baseCodeDescr";
import {enum_TipoPermesso} from "../models/profilazione.model";
import {Utente, Utente_DettagliAWS, UtenteFlatModel} from "../models/utente.model";
import {AjaxAgronicaAPIService} from "app/Service/ajax-agronica.api.service";
import {TipologiaUtente} from '../models/profili-permessi/tipologia-utente.model';
import {Utente_Permesso} from "app/Model/utente/utente_permesso";
import {GiasDialogService} from '../../Service/gias-dialog.service';
import {TranslocoService} from "@jsverse/transloco";
import {GruppoUtente} from "../models/gruppi-utenti/GruppoUtente.model";
import {AGRODATAFINE, AGRODATAINIZIO} from "../../Model/CostantiPersonalizzate";
import {LingueService} from "../../Service/lingue.service";
import {IntlService} from "@progress/kendo-angular-intl";
import '@progress/kendo-angular-intl/locales/fr/calendar';
import '@progress/kendo-angular-intl/locales/pt/calendar';
import {DatiModificaStandard} from "../../Model/DatiModifica.model";
import {DatiBaseUtente, ListaUtenti} from "../../Service/api.service";
import {IUtenteFinestraTemp} from "../models/IUtente.model";
import {cloneDeep} from "lodash";
import {GiasMessageService} from "../../Service/gias-message.service";


const linkTipologiexPermessi = "AgronicaCoreUtentiBIZ/ListaTipologiexPermessi";

const linkSetPermessi = "AgronicaCoreUtentiBIZ/AssegnaPermessi";

export class Scrivi_Utenti {
  constructor(
    public Utenti?: UtenteFlatModel[],
    public Operazione?: enum_TipoOperazioneDB,
    public SettingsFromProfile: boolean = false
  ) { }
}

export class ClientePermesso {
  constructor(
    public Id_Attivita: number,
    public Id_Operazione: number,
    public Id_Servizio: number = 5, // GiasOnline
  ) { }
}

export class ClientePermessiScrivi {
  constructor(
    public UserName: string,
    public permessi: ClientePermesso[]
  ) { }
}

@Injectable({providedIn: 'root'})
export class ProfilazioneUtentiService extends BehaviorSubject<any[]> {
  public selectedPermission: any[];
  public userReloaded$ = new Subject<boolean>();
  /** Contiene gli ultimi utenti caricati */
  public $utenti: BehaviorSubject<UtenteFlatModel[]>;
  private readonly grupoCodDefault = "99999999";
  // public $profiloApplicato: BehaviorSubject<any[]> = new BehaviorSubject<any[]>(null);

  constructor(
    public intlService: IntlService,
    public dialogService: GiasDialogService,
    public message: GiasMessageService,
    public lingue: LingueService,
    public masterService: MasterService,
    private transloco: TranslocoService,
    private ajaxAgronicaAPIService: AjaxAgronicaAPIService
  ) {
    super([]);
  }

  /***** UTENTI *****/

  public getUsersCount(): Observable<number> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<string, number>(
      "Profilazione/GetUsersCount", '', false
    ).pipe(take(1), map(r => r.RispostaOK ? r.RispostaStringa : -1));
  }

  public saveUsers(s: Scrivi_Utenti): Observable<boolean> {
    if (s.Utenti.length === 0) return of(true);
    if (this.hasInvalidUsers(s.Utenti, s.Operazione)) return of(false);
    let u = s.Utenti;
    s.Utenti = cloneDeep(s.Utenti);
    this.flattenGruppiUtente(s.Utenti);
    return this.ajaxAgronicaAPIService.ajaxAPIPost<Scrivi_Utenti, any>(
      "AgronicaCoreUtentiBIZ/SalvaUtenti", s,
      false,true, true, false
    ).pipe(
      take(1),
      map(response => {
        if (response.RispostaOK) {
          let text = s.Operazione === enum_TipoOperazioneDB.Scrittura
            ? 'CreazioneAvvenutaConSuccesso' : 'ModificaAvvenutaConSuccesso';
          if (response.Errore) {
            text = this.transloco.translate(text);
            text += '.\n' + this.transloco.translate('prof.WarningRimangonoAvvisiUtenti') + ':\n';
            text +=  response.Errore;
            this.dialogService.baseWarning(this.transloco.translate('UtentiSalvati'), text, false);
          } else {
            void this.dialogService.baseSuccess('UtentiSalvati', text);
          }
        } else {
          const title = s.Operazione === enum_TipoOperazioneDB.Scrittura
            ? this.transloco.translate('ErroreSalvataggio')
            : this.transloco.translate('ErroreModifica');
          this.dialogService.baseError(title, response.Errore ?? response.RispostaStringa, false);
        }
        return response.RispostaOK;
      })
    );
  }

  private flattenGruppiUtente(utenti: UtenteFlatModel[]) {
    utenti.forEach(u => {
      // Gli utenti sono sempre associati ad almeno un gruppo. Il gruppo di default è quello con codice 99999999.
      if (!u.GruppiCod?.length) u.GruppiCod = this.grupoCodDefault;
      if (!u.GruppiDes?.length) u.GruppiDes = "";
      if (u.GruppiCod.constructor.name !== 'String')
        u.GruppiCod = (u.GruppiCod as any[]).reduce((a, b) => a + '|' + b);
      if (u.GruppiDes.constructor.name !== 'String')
        u.GruppiDes = (u.GruppiDes as string[]).reduce((a, b) => a + '|' + b);
      u['Flag_Accesso_SPID'] = u['Flag_Accesso_SPID'] ?? false;
    });
  }

  /**
   * Esegue la cancellazione degli utenti deattivandone i permessi.
   * @param users utenti da disattivare.
   */
  public deactivateUsers(users: UtenteFlatModel[]): Observable<boolean> {
    if (users.length > 0) {
      let uu = users.map(u => u.UserName);
      // const params = new Scrivi_Utenti(uu, op);
      return this.ajaxAgronicaAPIService.ajaxAPIPost<string[], any>(
        "AgronicaCoreUtentiBIZ/DisattivaUtenti", uu
      ).pipe(take(1), map(R => R.RispostaOK));
    } else {
      return of(true);
    }
  }

  public readUtenti(escludiSuperUser?: boolean, mostraSoloAttivi?: boolean): Observable<Utente[]> {
    return this.readUtentiFlat(escludiSuperUser, mostraSoloAttivi)
      .pipe(map(r => this.parseUserList(r)));
  }

  public readUtentiFlat(escludiSuperUser: boolean = true, mostraSoloAttivi: boolean = false, utente: string = ""): Observable<UtenteFlatModel[]> {
    let superuser = this.masterService.objP_server.PivaSuperUser;
    if (this.$utenti) {
      return this.$utenti.pipe(filter(x => !!x));
    }
    this.$utenti = new BehaviorSubject<UtenteFlatModel[]>(null);
    return this.ajaxAgronicaAPIService.ajaxAPIPost<string, UtenteFlatModel[]>(
      "Profilazione/CaricaUtenti", utente
    ).pipe(take(1), map((res) => {
      let users = res.RispostaStringa ?? [];
      if (users.length === 0) {
        this.message.infoMessagge('prof.noUsersLoadedInfoMessage', false, true);
      }
      if (mostraSoloAttivi)
        users = users.filter(user => user.Attivo);
      if (escludiSuperUser) {
        let i = users.findIndex(user => user.PIVA === superuser || user.CodFisc === superuser);
        if (i >= 0)
          users.splice(i, 1);
      }
      for (let user of users) {
        user.UltimoAccesso = (user.UltimoAccesso as unknown as Date)?.toLocaleDateString();
        user.GruppiCod = (user.GruppiCod as string).split("|")
          .filter(cod => cod !== '').map(cod => +cod);
        user.GruppiDes = (user.GruppiDes as string).split("|")
          .filter(des => des !== '');
      }
      this.$utenti.next(users);
      this.$utenti.complete();
      // Leave time for other functions to access the data in  $utenti
      setTimeout(() => this.$utenti = null, 1000);
      return users;
    }));
  }

  public readUtentiDatiBase(escludiSuperUser?:boolean, mostraSoloAttivi?: boolean): Observable<DatiBaseUtente[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIGet<any, ListaUtenti>(
      "Provisioning/ListaUtentiDatiBase", ''
    ).pipe(take(1), map((res) =>
      res.RispostaOK ? res.RispostaStringa.ListaDatiBaseUtente : [])
    );
  }

  public editFinestraTemporale(utenti: IUtenteFinestraTemp[]): Observable<boolean> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost('Profilazione/ModificaFinestraTemporale', utenti)
      .pipe(take(1), map((res) => res.RispostaOK));
  }

  public aggiornaClientePermessi(permessi: ClientePermessiScrivi): Observable<RispostaStandard> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      "/Profilazione/AggiornaCliente_Permessi", permessi
    );
  }

  // Lettura = 0
  // Scrittura = 1
  // Modifica = 2
  // Cancellazione = 3
  // Trasferimento = 4
  // Copia = 10

  public leggiClientePermessi(username: string): Observable<ClientePermesso[]> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, ClientePermesso[]>(
      "/Profilazione/LeggiCliente_Permessi", username
    ).pipe(take(1), map(r => r.RispostaOK ? r.RispostaStringa : []));
  }

  private parseUserList(rispostaStd: any[]): Utente[] {
    console.log('parsing', rispostaStd);
    return rispostaStd.map(data => {
      const u = new Utente();
      u.Piva_SuperUser = data.Piva_SuperUser;
      u.UserName = data.UserName;
      u.Password = data.Password;
      u.Nome = data.Nome;
      u.Cognome = data.Cognome;
      u.CodFisc = data.CodFisc;
      u.PIVA = data.PIVA;
      u.Rag_Soc = data.Rag_Soc;
      u.IsAttivo = data.Attivo === 1;
      u.IsAzienda = data.Flag_Azienda_Persona === 1;
      u.DatiAccesso = new Utente_DettagliAWS();
      u.DatiAccesso.UltimoAccesso = this.str2Date(data.UltimoAccesso);
      u.DatiAccesso.NumeroAccessi = data.NumeroAccessi;
      u.GDPR = data.GDPR === "True";
      u.ServiziAttivi = data.ServiziAttivi;
      u.Lingua = new BaseCodeDescr(+data.Lingua_Cod);
      u.IsPasswordEncrypted = data.Flag_Encrypted === 1;
      u.Dettagli = data.Dettagli;
      u.KendoKey = data.kendoKey;
      u.Utente = data.Utente;
      u.Utente_Profilo = data.Utente_Profilo;
      u.UserNameCommerciale = data.UserNameCommerciale;

      u.Gruppi = [];
      const gruppiCod = typeof(data.GruppiCod) === 'string'
        ? (data.GruppiCod as string).split("|").filter(cod => cod !== '')
        : data.GruppiCod as any[];
      const gruppiDes = typeof(data.GruppiCod) === 'string'
        ? (data.GruppiDes as string).split("|").filter(des => des !== '')
        : data.GruppiDes as any[];
      gruppiCod.forEach((cod, i) => u.Gruppi.push(new GruppoUtente(+cod, gruppiDes[i])));

      u.Tipologia = new TipologiaUtente(+data.Tipologia_Cod, data.Tipologia_Des);
      u.Email = data.Email;
      u.Tel = data.Tel;
      u.ValiditaInizioPermessi = data.Validita_Inizio || AGRODATAINIZIO;
      u.ValiditaFinePermessi = data.Validita_Fine || AGRODATAFINE;
      let datiModifica = new DatiModificaStandard();
      datiModifica.init(data.Username_Creazione, data.Data_Creazione, data.Username_Modifica, data.Data_Modifica);
      u.DatiModifica = datiModifica;
      return u;
    });
  }

  private hasInvalidUsers(utenti: UtenteFlatModel[], operazione: enum_TipoOperazioneDB): boolean {
    const invalidUsers = utenti.filter(user => !this.hasAllRequiredFields(user, operazione));
    const title = this.transloco.translate('ErroreSalvataggio');
    let text = this.transloco.translate('prof.DatiObbligatoriMancanti');
    if (invalidUsers.length > 0) {
      console.warn(invalidUsers.map(u => u.UserName + ': ' + u.CodFisc + ', ' + u.Cognome + ', ' + u.UserNameCommerciale + ', ' + u.Password + ', ' + u.Flag_Azienda_Persona));
      if (invalidUsers.some(u => !u.UserNameCommerciale)) {
        text += "\n" + this.transloco.translate('prof.WarnAlcuniUtentiNonHannoUsername');
      }
      if (invalidUsers.some(u => !u.Cognome)) {
        text += "\n" + this.transloco.translate('prof.NecessarioSpecificareCognome') + ": "
          + invalidUsers.filter(u => !u.Cognome && u.UserName).map(u => u.UserName).reduce((u1, u2) => u1 + ', ' + u2);
      }
      if (invalidUsers.some(u => !u.UserNameCommerciale)) {
        text += "\n" + this.transloco.translate('prof.NecessarioSpecificareNoteUtente') + ": "
          + invalidUsers.filter(u => !u.UserNameCommerciale && u.UserName).map(u => u.UserName).reduce((u1, u2) => u1 + ', ' + u2);
      }
      if (invalidUsers.some(u => !u.CodFisc)) {
        text += "\n" + this.transloco.translate('prof.NecessarioSpecificareCF') + ": "
          + invalidUsers.filter(u => !u.CodFisc && u.UserName).map(u => u.UserName).reduce((u1, u2) => u1 + ', ' + u2);
      }
      if (operazione === enum_TipoOperazioneDB.Scrittura && invalidUsers.some(u => !u.Password)) {
        text += "\n" + this.transloco.translate('prof.NecessarioReinserirePasswordUtenti') + ": "
          + invalidUsers.filter(u => !u.Password && u.UserName).map(u => u.UserName).reduce((u1, u2) => u1 + ', ' + u2);
      }
      this.dialogService.baseError(title, text);
    }
    return invalidUsers.length > 0;
  }

  private hasAllRequiredFields(utente: UtenteFlatModel, operazione: enum_TipoOperazioneDB): boolean {
    const hasBaseFields = Boolean(utente.CodFisc
      && (utente.Cognome || utente.Rag_Soc)
      && utente.UserNameCommerciale
      && utente.Flag_Azienda_Persona);
    if (operazione === enum_TipoOperazioneDB.Scrittura)
      return Boolean(hasBaseFields && utente.Password);
    return hasBaseFields;
  }

  /***** PERMESSI *****/

  public readGerarchiaPermessi(Tipologia_Cod:number=-1, Username_Utente?:string, leggiUtenti:boolean=false): Observable<any> {
    let args = {Tipologia_Cod: Tipologia_Cod, Username_Utente: Username_Utente, LeggiUtenti: leggiUtenti};
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      "AgronicaCoreUtentiBIZ/Carica_Gerarchia_Permessi", args
    ).pipe(map((risposta) => risposta.RispostaStringa));
  }

  public readGerarchiaPermessiAttivi(): Observable<any> {
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      "AgronicaCoreUtentiBIZ/Carica_Gerarchia_Permessi_Attivi", ''
    ).pipe(map((risposta) => risposta.RispostaStringa));
  }

  public readPermessixTipologia(Tipologia_Cod?: number, UserName?: string): Observable<any> {
    if (Tipologia_Cod === undefined) {
      return this.readAllPermessixTipologia();
    }
    return this.readGerarchiaPermessi(Tipologia_Cod, UserName).pipe(
      map((risposta) => {
        let permissions = [];
        for (let p of risposta['Permessi']) {
          permissions.push({
            Tipologia_Cod: Tipologia_Cod,
            Attivita_Cod: p.Attivita_Cod,
            Attivita_Des: p.Attivita_Des,
            MenuPrimoLivello: p.MenuPrimoLivello.descrizione,
            MenuSecondoLivello: p.MenuSecondoLivello.descrizione,
            Id_Operazione: p.Id_Operazione,
            Ordinamento: p.Ordinamento,
            Lettura: (p.Id_Operazione === 0 || p.Id_Operazione === 2) ? this.transloco.translate('Si') : this.transloco.translate('No'),
            Scrittura: (p.Id_Operazione === 2) ? this.transloco.translate('Si') : this.transloco.translate('No')
          });
        }
        return {Permessi: permissions, Utenti: risposta['Utenti']};
      })
    );
  }

  private readAllPermessixTipologia() {
    return this.ajaxAgronicaAPIService.ajaxAPIPost(
      linkTipologiexPermessi, JSON.stringify(true)
    ).pipe(map((risposta) => risposta.RispostaStringa));
  }

  public setPermessixTipologia(profilo: BaseCodeDescr, attivita: Utente_Permesso[],
                               permesso: enum_TipoPermesso): Observable<boolean> {
    const param = new TipologiaUtente(profilo.codice, profilo.descrizione);
    attivita.forEach(a => a.Permesso_Tipo = permesso);
    param.Permessi = attivita;
    return this.ajaxAgronicaAPIService.ajaxAPIPost<any, any>(
      linkSetPermessi, param
    ).pipe(map((risposta) => risposta.RispostaOK));
  }

  //-------------------------------------------------------------------------
  // UTILS

  private str2Date(dataStr: string): Date {
    return this.intlService.parseDate(dataStr, '', this.transloco.getActiveLang());
  }

}
