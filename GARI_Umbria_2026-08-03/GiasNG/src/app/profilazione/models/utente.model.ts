import {GruppoUtente} from "./gruppi-utenti/GruppoUtente.model";
import {TipologiaUtente} from "./profili-permessi/tipologia-utente.model";
import {AGRODATAFINE, AGRODATAINIZIO} from "../../Model/CostantiPersonalizzate";
import {BaseCodeDescr} from "../../Model/baseClass/baseCodeDescr";
import {DatiModifica, DatiModificaStandard} from "../../Model/DatiModifica.model";
import {IUtenteFinestraTemp} from "./IUtente.model";
import {IntervalloTemporale} from "../../Model/anagrafiche/IntervalloTemporale";
import {UtentePermessi} from "./profili-permessi/UtentePermessi.model";
import {UtenteDTO} from "./utente-dto.model";

export enum enum_Azienda_Persona {
  Impresa = 1,
  Persona = 2
}

export class UtenteFinestraTemp implements IUtenteFinestraTemp {
  FinestraTemporale: IntervalloTemporale;
  UserName: string;

  constructor(username:string, inizio?: Date, fine?: Date) {
    this.UserName = username;
    this.FinestraTemporale = new IntervalloTemporale(inizio, fine);
  }
}

export class Utente_DettagliAWS {
  public UserName: string;
  public Cognome: string;
  public Nome: string;
  public UltimoAccesso: Date;
  public NumeroAccessi: number;
}

/**
 * @usageNotes usato come modello per le righe nella griglia utenti della profilazione
 */
export class UtenteFlatModel {
  Attivo: string;
  Azienda_Persona: string;
  CodFisc: string;
  Cognome: string;
  Username_Creazione: string;
  Username_Modifica: string;
  Data_Creazione: Date;
  Data_Modifica: Date;
  Dettagli: string;
  Email: string;
  Flag_Azienda_Persona: number | enum_Azienda_Persona;
  Flag_Encrypted: number;
  GDPR: string;
  GruppiCod: Array<number> | string;
  GruppiDes: Array<string> | string;
  kendoKey: string;
  Lingua_Cod: number;
  Nome: string;
  NumeroAccessi: number;
  Password: string;
  PIVA: string;
  Piva_SuperUser: string;
  Rag_Soc: string;
  ServiziAttivi: string;
  Tel: string;
  Tipologia_Cod: number;
  Tipologia_Des: string;
  UltimoAccesso: string;
  UserName: string;
  UserNameCommerciale: string;
  Utente: string;
  Utente_Profilo: string;
  Validita_Fine: Date;
  Validita_Inizio: Date;
  FinestraTemporaleInizio: Date;
  FinestraTemporaleFine: Date;

  public toUtente(): Utente {
    let u = new Utente();
    copyCommonFields(u, this);
    u.IsAttivo = this.Attivo === "Si";
    u.IsAzienda = this.Flag_Azienda_Persona === 1;
    u.DatiAccesso = new Utente_DettagliAWS();
    u.DatiAccesso.UltimoAccesso = new Date(this.UltimoAccesso);
    u.DatiAccesso.NumeroAccessi = this.NumeroAccessi;
    u.GDPR = this.GDPR === "Si";
    u.Lingua = new BaseCodeDescr(+this.Lingua_Cod);
    u.IsPasswordEncrypted = this.Flag_Encrypted === 1;
    u.KendoKey = this.kendoKey;
    u.Gruppi = [];
    if (this.GruppiCod.length > 0) {
      if (this.GruppiCod.constructor.name === 'string') {
        u.Gruppi.push(new GruppoUtente(+this.GruppiCod, this.GruppiDes as string));
      } else {
        (this.GruppiCod as Array<number>).forEach((cod, i) =>
          u.Gruppi.push(new GruppoUtente(+this.GruppiCod[i], this.GruppiDes[i]))
        );
      }
    }
    u.Tipologia = new TipologiaUtente(+this.Tipologia_Cod, this.Tipologia_Des);
    u.ValiditaInizioPermessi = this.Validita_Inizio || AGRODATAINIZIO;
    u.ValiditaFinePermessi = this.Validita_Fine || AGRODATAFINE;
    u.DatiModifica.DataCreazione = this.Data_Creazione;
    u.DatiModifica.DataModifica = this.Data_Modifica;
    u.DatiModifica.UsernameCreazione = this.Username_Creazione;
    u.DatiModifica.UsernameModifica = this.Username_Modifica;
    return u;
  }
}

export class Utente extends UtenteDTO implements UtentePermessi, IUtenteFinestraTemp {
  Piva_SuperUser: string;
  Password: string;
  UserName: string;

  Nome: string;
  Cognome: string;
  CodFisc: string;

  PIVA: string;
  Rag_Soc: string;

  IsAttivo: boolean;
  IsAzienda = false;
  DatiAccesso: Utente_DettagliAWS;
  GDPR: boolean;
  ServiziAttivi: string;
  Lingua: BaseCodeDescr;
  IsPasswordEncrypted: boolean;
  Dettagli: string;
  KendoKey: string;

  Utente: string;
  Utente_Profilo: string;
  UserNameCommerciale: string;
  Gruppi: GruppoUtente[] = [];
  Tipologia: TipologiaUtente;
  Email: string;
  Tel: string;

  ValiditaInizioPermessi: Date = AGRODATAINIZIO;
  ValiditaFinePermessi: Date = AGRODATAFINE;
  DatiModifica: DatiModifica = new DatiModificaStandard();
  FinestraTemporale: IntervalloTemporale;

  public flatten(): UtenteFlatModel {
    let u = new UtenteFlatModel();
    copyCommonFields(u, this);
    u.kendoKey = this.KendoKey;
    u.Attivo = this.IsAttivo ? '1' : '0';
    u.Flag_Azienda_Persona = this.IsAzienda ? 1 : 2;
    u.Azienda_Persona = this.IsAzienda ? 'I' : 'P';
    u.UltimoAccesso = this.DatiAccesso?.UltimoAccesso?.toLocaleString() || "";
    u.NumeroAccessi = this.DatiAccesso?.NumeroAccessi || 0;
    u.GDPR = this.GDPR.toString();
    u.Lingua_Cod = this.Lingua.codice;
    u.Flag_Encrypted = this.IsPasswordEncrypted ? 1 : 0;
    u.GruppiCod = this.Gruppi.map(g => +g.codice);
    u.GruppiDes = this.Gruppi.map(g => g.descrizione);
    u.Tipologia_Cod = +this.Tipologia.codice;
    u.Tipologia_Des = this.Tipologia.descrizione;
    u.Validita_Inizio = this.ValiditaInizioPermessi || AGRODATAINIZIO;
    u.Validita_Fine = this.ValiditaFinePermessi || AGRODATAFINE;
    u.Data_Creazione = this.DatiModifica.DataCreazione;
    u.Data_Modifica = this.DatiModifica.DataModifica;
    u.Username_Creazione = this.DatiModifica.UsernameCreazione;
    u.Username_Modifica = this.DatiModifica.UsernameModifica;
    return u;
  }

  public fromFlat(flat) {
    this.Piva_SuperUser = flat.Piva_SuperUser;
    this.UserName = flat.UserName;
    this.Password = flat.Password;
    this.Nome = flat.Nome;
    this.Cognome = flat.Cognome;
    this.CodFisc = flat.CodFisc;
    this.PIVA = flat.PIVA;
    this.Rag_Soc = flat.Rag_Soc;
    this.IsAttivo = flat.Attivo === 1;
    this.IsAzienda = flat.Flag_Azienda_Persona === 1;
    this.DatiAccesso = new Utente_DettagliAWS();
    this.DatiAccesso.NumeroAccessi = flat.NumeroAccessi;
    this.GDPR = flat.GDPR === "True";
    this.ServiziAttivi = flat.ServiziAttivi;
    this.Lingua = new BaseCodeDescr(+flat.Lingua_Cod);
    this.IsPasswordEncrypted = flat.Flag_Encrypted === 1;
    this.Dettagli = flat.Dettagli;
    this.KendoKey = flat.kendoKey;
    this.Utente = flat.Utente;
    this.Utente_Profilo = flat.Utente_Profilo;
    this.UserNameCommerciale = flat.UserNameCommerciale;

    this.Gruppi = [];
    flat.GruppiCod.forEach((cod, i) => this.Gruppi.push(new GruppoUtente(cod, flat.GruppiDes[i])));

    this.Tipologia = new TipologiaUtente(+flat.Tipologia_Cod, flat.Tipologia_Des);
    this.Email = flat.Email;
    this.Tel = flat.Tel;
    this.ValiditaInizioPermessi = flat.Validita_Inizio || AGRODATAINIZIO;
    this.ValiditaFinePermessi = flat.Validita_Fine || AGRODATAFINE;
    let datiModifica = new DatiModificaStandard();
    datiModifica.init(flat.Username_Creazione, flat.Data_Creazione, flat.Username_Modifica, flat.Data_Modifica);
    this.DatiModifica = datiModifica;
    this.FinestraTemporale = new IntervalloTemporale(flat.FinestraTemporaleInizio, flat.FinestraTemporaleFine);

    this['isNew'] = flat['isNew'];
  }

}

function copyCommonFields(dst: Utente | UtenteFlatModel, src: Utente | UtenteFlatModel) {
  dst.Piva_SuperUser = src.Piva_SuperUser;
  dst.UserName = src.UserName;
  dst.Password = src.Password;
  dst.Nome = src.Nome;
  dst.Cognome = src.Cognome;
  dst.CodFisc = src.CodFisc;
  dst.PIVA = src.PIVA;
  dst.Rag_Soc = src.Rag_Soc;
  dst.Dettagli = src.Dettagli;
  dst.Utente = src.Utente;
  dst.Utente_Profilo = src.Utente_Profilo;
  dst.UserNameCommerciale = src.UserNameCommerciale;
  dst.ServiziAttivi = src.ServiziAttivi;
  dst.Email = src.Email;
  dst.Tel = src.Tel;
}
