import { ElementRef } from '@angular/core';

export const AGRODATAINIZIO: Date = new Date(1900, 0, 1, 0, 0, 0, 0);
export const AGRODATAFINE: Date = new Date(2100, 11, 31, 0, 0, 0, 0);

export class BaseCodeDescr {
  codice: number;
  descrizione: string;

  constructor(codice: number, descrizione?: string) {
    this.codice = codice;
    this.descrizione = descrizione;
    if (descrizione == undefined || descrizione == null) {
      this.descrizione = '';
    }
  }
}

export class BaseCodeDescrStr {
  codice: string;
  descrizione: string;

  constructor(codice: string, descrizione?: string) {
    this.codice = codice;
    this.descrizione = descrizione;
  }
}

export class BaseCodeDescrVal extends BaseCodeDescr {
  public valore: string;

  constructor(codice?: number, descrizione?: string, valore?: string) {
    super(codice, descrizione)
    this.valore = valore;
  }
}

export class Codice {
  public chiave: number;
  public Descrizione: string;
  public Id_Cod: string;
  public Val_Cod: string;
  public Validita_Inizio: Date;
  public Validita_Fine: Date;
}

export class CodiciNazioniISO3166 {
  codice: string;
  descrizione: string;
  codiceNumerico: string;
  codiceAlpha3: string;
  gestioneGerarchia: number;

  constructor(codice: string, descrizione: string, codiceNumerico: string, codiceAlpha3: string, gestioneGerarchia: number) {
    this.codice = codice;
    this.descrizione = descrizione;
    this.codiceNumerico = codiceNumerico;
    this.codiceAlpha3 = codiceAlpha3;
    this.gestioneGerarchia = gestioneGerarchia;
  }
}

export enum enum_InputType {
  Gias_Classic = 0,
  Angular_Material = 1,
  Due_righe = 2
}
export class FormaGiuridica {
  Fg_Cod: number;
  Fg_Des: string;
}

export class Provincia {
  sigla: string;
  regione_cod: string;
  regione_des: string;
  Provincia_Des: string;
  Istat_Prov: string;
  Stato_Country: string;
  comuneDefault: string;
}

export class Comune {
  provincia: Provincia;
  codice: string;
  descrizione: string;
}

export class Stato {
  codice: string;
  descrizione: string;
}

export class DestinazioneUso {
  codice: number;
  descrizione: string;
}

export class SpecieVegetale {
  veg_cod: number;
  veg_des: string;

  constructor(codice?: number, descrizione?: string) {
    this.veg_cod = codice?.valueOf();
    this.veg_des = descrizione?.toString();
  }
}

export class Cultivar {
  cul_cod: number;
  cul_des: string;

  constructor(codice?: number, descrizione?: string) {
    this.cul_cod = codice?.valueOf();
    this.cul_des = descrizione?.toString();
  }
}

export class CultivarxSpecie {
  Veg_Cod: number;
  Cultivar: Cultivar[];
}

export class Finalita {
  grfi_cod: number;
  grfi_des: string;
}

export class FinalitaxSpecie {
  Veg_Cod: number;
  Finalita: Finalita[];
}

export class ColturePrecedenti {
  Codice: string;
  Descrizione: string;
}

export class Ditta {
  codice: number;
  descrizione: string;
}

export class Tipo {
  CLASS_CODE: string;
  CLASS_DESC: string;
}

export class Tipo1 {
  id: string;
  name: string;
}

export class Marca {
  codice: number;
  descrizione: string;
}

export class LoadingObject {
  isLoading = false;
  message?: string = '';
  component?: ElementRef<any>;
  zIndex?: number = 1000;
}

export enum enum_ErroreGias_Tipo {
  Generico = 1,
  NonGestito = 999
}

export enum ErroreGias_Severity {
  Bloccante = 0, //Errore in basso a dx rosso, l'utente non può procedere (sia errori gestiti che exceptions)
  Warning = 1, //Pop-up di n warning accodati con possibilità per l'utente di proseguire ("Si desidera proseguire?"   -   Annulla/Prosegui)
  Info = 2,
  WarningBloccante = 3 //Pop-up di n errori accodati senza possibilità per l'utente di proseguire ("Non è possibile proseguire" - OK --> l'utente DEVE sistemare dei dati, NON può procedere altrimenti )
}

export class ErroreGias {
  severity: ErroreGias_Severity;
  messaggio: string;
  ex: string;
  tipo: enum_ErroreGias_Tipo;
}

export class RispostaStandard {
  Sessione: boolean;
  RispostaOK: boolean;
  RispostaConferma: boolean;
  ParametroDue: boolean;
  ParametroDue_stringa: string;
  Tipo: string;

  RispostaCompressa: Uint8Array;
  Compressa: boolean;

  Errore: string;
  RispostaStringa: string;

  ErroriGias: ErroreGias[];
}

export class rispostaStandard<T> {
  Sessione: boolean;
  RispostaOK: boolean;
  RispostaConferma: boolean;
  ParametroDue: boolean;
  ParametroDue_stringa: string;
  Tipo: string;
  RispostaCompressa: Uint8Array;
  Compressa: boolean;

  Errore: string;
  RispostaStringa: T;

  ErroriGias: ErroreGias[];
}
export class Parametri {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
}

export class CoreWSRequest<T> {
  objP_super_server: string;
  objP_server: string;
  objP_utenti: string;
  InData: T;
}

export class ErrorMsg {
  show: boolean;
  msg: string;
  errorNumber: number;
}

export class WindowErrorMsg {
  show: boolean;
  msg: string;
  showWarningIcon?: boolean;
  TitleBarMessage?: string;
  errorNumber: number;
}

export class LoadingComponentObject {
  isLoading = false;
  message?: string = '';
  component: ElementRef<any>;
}

export class Leggi_Forme_Giuridiche_Request {
  FG_cod: number;
  objP_server: string;
}

export class ISTAT_GetProvincie_Request {
  valori_regioni: string;
  objP_Server: string;
}

export class ISTAT_GetComuni_Request {
  valori_provincia: string;
  objP_Server: string;
}

export class ISTAT_GetCAP_Request {
  Prov: string;
  Com: string;
}

export class GetProvince {
  stato: string;
  regione: string;
}

export class GetRegionsByProvince {
  stato: string;
  prov: string;
}

export class DropdownListEvent {
  constructor(
    public type: DropdownEventType,
    public id: string,
    public data: any,
    public listItems: any) { }
}

export enum DropdownEventType {
  ON_CHANGE_VALUE = 'value-changed',
  ON_PRE_UPDATE_DROPDOWN = 'pre-update-dropdown',
  ON_UPDATE_DROPDOWN = 'update-dropdown',
  ON_OPEN_DROPDOWN = 'open-dropdown',
  ON_CLOSE_DROPDOWN = 'close-dropdown',
  ON_FOCUS_DROPDOWN = 'focus-dropdown',
  ON_BLUR_DROPDOWN = 'blur-dropdown',
  ON_OPENED_DROPDOWN = 'opened-dropdown',
}

export class DropdownListFormItem {
  Id: string;
  FormControlName: string;
  Value: any;
}

export class DropdownListItem {
  constructor(
    public readonly id: any,
    public readonly name: string,
    public readonly data?: any) { }
}

export class Utente {
  constructor(
    public Username?: string,
    public Nome?: string,
    public Cognome?: string,
    public Rag_Soc?: string,
    public Cod_Fisc?: string,
    public UsernameCommerciale?: string,
    public Permessi: Utente_Permesso[] = [],
    public Impostazioni: Utente_Impostazioni[] = [],
    public ValiditaUtentePermessiLicenza?: Messaggio_Utente_Permessi
  ) { }
}

export class Messaggio_Utente_Permessi {
  Permessi_Scaduta_O_In_Scadenza: boolean;
  Licenza_Scaduta_O_In_Scadenza: boolean;
  Messaggio: string;
}

export class Utente_Permesso {
  /**
   * @param Permesso_ID corrisponde al codice attività del permesso
   * @param Permesso_Tipo livello di permessi sull'attività
   */
  constructor(
    public Permesso_ID?: number,
    public Permesso_Tipo?: number | enum_TipoPermesso
  ) { }
}

export enum enum_TipoPermesso {
  LETTURA = 0,
  DISABILITATO = 1,
  LETTURA_SCRITTURA = 2,
}

export class Utente_Impostazioni {
  constructor(
    public Impostazione_Cod?: number,
    public Valore?: string
  ) { }
}

export enum Enum_DBTypeOperation {
  Read = 0,
  Write = 1,
  Update = 2,
  Delete = 3
}

export class PostMessageStrutturata<T> {
  messaggio: messaggioPostMessage;
  contesto: contestoPostMessage;
  inData: T;

  constructor(
    messaggio: messaggioPostMessage,
    contesto: contestoPostMessage,
    inData: T
  ) {
    this.messaggio = messaggio;
    this.contesto = contesto,
      this.inData = inData
  }

}

export enum messaggioPostMessage {
  chiudiWindowGiasNG = "chiudiWindowGiasNG",
  chiudiFinestra = "chiudiFinestra"
}

export enum contestoPostMessage {
  SalvaAppezzamento = 1,
  SalvaCampo = 2,
  FiltroneImpianti = 3,
  FiltroneVisibilitaAziendale = 4
}

export class PostMessageSelezioneFiltrone {
  contestoPostMessage: contestoPostMessage;
  tipoChiavi: string;
  elencoChiavi: string[];
  parametri: string[][];
}

export enum CELL_TYPES {
  STRING = 'string',
  DATE = 'date',
  DATETIME = 'datetime',
  DROPDOWNLIST = 'dropdownlist',
  MULTI_DROPDOWNLIST = 'multi_dropdownlist',
  NUMBER = 'number',
  BOOLEAN = 'boolean',
  OBJECT = 'object',
  CUSTOM = 'custom'
}


export enum enum_TipoControllo {
  UNDEFINED = 0,
  CASELLA_TESTO = 1,
  AREA_TESTO = 2,
  MENU_DISCESA = 3,
  CASELLA_SPUNTA = 4, // boolean switch / checkbox
  CALENDARIO = 5,
  ALLEGATO = 6,
  LINK = 7,
  PASSWORD = 8,
  MULTISELECT_ESTESA_SERVER = 9,
  PULSANTE_SCELTA = 10,
  IMMAGINE = 11,
  DDL_ESTESA_CLIENT = 12,
  GIS_VIEWER = 13,
  DDL_ESTESA_SERVER_LIGHT = 14,
  NUMERO_INTERO = 15,
  NUMERO_DECIMALE = 16,
  MULTISELECT = 17
}

export interface IGuidaValoreImpostazione {
  codice: any;
  descrizione: string;
  valore: string;
  Tipo_campo: string;
  Note: string;
}

/**
 * Modella i possibili valori legati a un'impostazione.
 * Modello creato sulla base della tabella Guida_Impostazioni_Valori.
 */
export class GuidaValoreImpostazione extends BaseCodeDescrVal implements IGuidaValoreImpostazione {
  constructor(
    codice?: number,
    descrizione?: string,
    valore?: string,
    public Tipo_campo: string = "0",
    public Note: string = "",
    public Username: string = ""
  ) {
    super(codice, descrizione, valore);
  }
}

export class GenericGuidaValoreImpostazione<T> implements IGuidaValoreImpostazione {
  constructor(
    public codice: T,
    public descrizione: string = "",
    public valore: string = null,
    public Tipo_campo: string = "0",
    public Note: string = ""
  ) {
  }
}

/**
 * Modello d'impostazione usato come base per il form impostazioni.
 */
export class ImpostazioniFormItem {
  guida: Impostazione;
  valoreCorrente: string | any;
  valori: IGuidaValoreImpostazione[];
}

/** Descrive la struttura delle impostazioni come salvate su DB.
 */
export class Impostazione {
  Data_Creazione: Date;
  Data_Modifica: Date;
  Data_Invio: Date;

  Impostazione_Cod: number;
  Impostazione_Des: string;
  Impostazione_SuperUser: number;
  Impostazione_Azienda: number;
  Impostazione_AziendaCentro: number;
  Impostazione_AziendaCentroSpecie: number;
  Flag_InApp: number;

  Inviato: number;
  Note: string;
  Ordine: number;
  Tipo_Campo: string;
  Valore_Default: string;

  Sezione_Cod: number;
  Sezione_Des: string;
  SottoSezione_Cod: number;
  SottoSezione_Des: string;
  Livello_Cod: number;
  Livello_Des: string;

  Username_Creazione: string;
  Username_Modifica: string;
  Validita_Inizio: Date;
  Validita_Fine: Date;

  value: string;
  descrizione: string;
  codice: number;

  constructor(Impostazione_Cod: number, Impostazione_Des?: string, tipoControllo = "0") {
    this.Impostazione_Cod = Impostazione_Cod;
    this.Impostazione_Des = Impostazione_Des;
    this.Tipo_Campo = tipoControllo;
  }

  public setVisibility(visibility: {
    isSuperUser?: 0 | 1; isAzienda?: 0 | 1;
    isUtente?: 0 | 1; isInApp?: 0 | 1;
    isAziendaCentro?: 0 | 1; isAziendaCentroSpecie?: 0 | 1;
  }) {
    this.Impostazione_SuperUser = visibility.isSuperUser || this.Impostazione_SuperUser || 0;
    //this.isAzienda = visibility.isAzienda || this.isAzienda || 0;
    this.Impostazione_AziendaCentro = visibility.isAziendaCentro || this.Impostazione_AziendaCentro || 0;
    this.Impostazione_AziendaCentroSpecie = visibility.isAziendaCentroSpecie || this.Impostazione_AziendaCentroSpecie || 0;
    //this.isUtente = visibility.isUtente || this.isUtente || 0;
    this.Flag_InApp = visibility.isInApp || this.Flag_InApp || 0;
  }

  public setSection(sezione?: number, sottoSezione?: number, livello?: number, ordine?: number) {
    this.Sezione_Cod = sezione !== undefined ? sezione : this.Sezione_Cod;
    this.SottoSezione_Cod = sottoSezione !== undefined ? sottoSezione : this.SottoSezione_Cod;
    this.Livello_Cod = livello !== undefined ? livello : this.Livello_Cod;
    this.Ordine = ordine !== undefined ? ordine : this.Ordine;
  }
}
