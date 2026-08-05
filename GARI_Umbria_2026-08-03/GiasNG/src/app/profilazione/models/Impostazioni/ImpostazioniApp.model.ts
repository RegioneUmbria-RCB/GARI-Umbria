
import { enum_Dati_App, enum_Import_App } from 'app/amministrazione-sistema/dati-app/consulta-sincro-dati-app/consulta-sincro.service';
import { enum_Impostazioni_Utenti } from 'app/Model/Impostazioni_Utenti.enum';
import { GenericGuidaValoreImpostazione, Impostazione, ImpostazioniFormItem } from 'gias-ui-kit';
import { enum_TipoControllo } from 'gias-ui-kit';

export const customAppSettings = ["Importazioni"];

export class ImpostazioniApp {
  /** token per aprire Gias da app */
  public Token: string = "";

  /** se true attiva modalità master app (SDF) */
  public Master: boolean = false;
  /** 0 = Default, 1 = Manuale, 2 = Automatico */
  public TipoSincro: number = 0;
  /** 0 = Nessuna (solo frontiera), 1 = Parziale (AgroGSB), 2 = Completa
   *
   * Vedi i valori default in {@link SettingsCategorieMagazzinoComponent.handleAppImportsRowValorize()}.
   * Vedi i valori default in {@link setPossibleValues()}.
   */
  public Importazioni: string = "";
  /** "" = Nessuno, "0" = Tutti, "lista lav_cod separati da virgola" x import su agenda */
  public ImportAgenda: string = "";
  /** sincronizza piano colturale ultimi n anni agrari */
  public SincroAnni: number = 0;
  /** "" o "0" = No, "1" = Tutti, "lista tipi dati app da ripristinare separati da virgola" */
  public RestoreDati: string = "";
  public RestoreGiorni: number = 0;

  // USATE SOLO DALL'APP
  /** default bozza (true = In Corso, false = Definitiva) */
  public Bozza: boolean = true;
  /** se true abilita lettura tag NFC */
  public NFC: boolean = false;
  /** esclude lavorazioni da gestione costi */
  public FiltroLavorazioni: string = "";
  /** se true mostra solo magazzini del centro selezionato nelle attività */
  public FiltroMagazzini: boolean = false;
  /** default descrizione visite */
  public VisiteDescrizione: string = "";
  /** gestione visite-rilievi */
  public VisiteRilievi: boolean = false;
  /** gestione visita con specie senza impianti */
  public VisiteSpecie: boolean = true;
  /** se true cambia label "ricetta" in "proposta di acquisto" */
  public PropostaAcquisto: boolean = false;
  /** versione custom dell'app (es: "1" per ENI Kenya) */
  public Custom: string = "";
  /** lista lingue utente gestite (es: "en", "it,en") */
  public Lingua: string = "";
  /** gestione piano colturale offline */
  public Offline: boolean = true;
  /** forza sincro anagrafiche prodotti (Es: "10" per sementi) */
  public Prodotti: string = "";
  /** se > 0 attiva gestione posizione nel disegno poligono */
  public Posizione: number = 0;
  /** se true attiva lettura acquisti da qr code */
  public QRCode: number = 0;
  /** link termini di utilizzo */
  public Termini: string = "";
  /** link termini della privacy */
  public Privacy: string = "";
  /** se > 0 attiva refresh del token dopo x minuti */
  public RefreshToken: number = 0;
  /** forza gestione per agenda NG */
  // public GestioneAgenda: string = "";
  /** sincronizza dati al login se cambia l'utente */
  public SincroLogin: boolean = true;
  /** se true controlla se è presente versione aggiornata dell'app */
  public CheckUpdate: boolean = true;
  /** se true la superficie non è modificabile se ricavata dal poligono */
  public BloccoArea: boolean = false;
  /** distanza minima in metri tra nuovo punto e l'ultimo aggiunto del poligono */
  public DistanzaMinima: number = 5;
  /** distanza massima in metri tra nuovo punto e l'ultimo aggiunto del poligono */
  public DistanzaMassima: number = 100;
  /** Se `true` attiva gestione gerarchia aziende per gruppo utente */
  public GerarchiaImprese: boolean = false;
  /** Gestione ivio dati APP -> WEB. Valori possibili:
   * 0: non sincronizzare dati aziende non scelte e non visibili
   * 1: sincronizza dati aziende non più scelte
   * 2: invio dati aziende non più visibili
   * 3: invio dati aziende non più scelte e non più visibili */
  public InviaDati: number = 0;
  /** Attiva gestione risposta compressa au app. `""` per disattivare. */
  public Encoding: string = "gzip";
  // lista specie per sincro prodotti offline separati da "|"
  public SpecieProdotti: string = "";
  // Tipologia periodo di validità dell'impianto (0 = Default, 1 = AnnoAgrario, 2 = Manuale)
  public TipoValidita: number = 0;
  /** Attiva cache dati più comuni per velocizzare sincro */
  public CacheDati: number = 1
  /** Se attivo, esegue speed test oltre al solito controllo di connessione */
  public TestConn: boolean = false;
  /** Attiva la comunicazione con la centralina BTM per il tracking dei trattori */
  public BTNConn: boolean = false;
  /** Se attivo, esegue il controllo della presenza del poligono per l'impianto */
  public CheckPoligono: boolean = false;
  /** Attiva la gestione SAT sul GIS (deve essere abilitato anche l'utente) */
  public PrecisionFarming: boolean = false;
  /** Se `1` possono essere aggiunti più prodotti alle raccolte (se `0` a massimo 1 prodotto) */
  public ProdottiRaccolta: number = 1;
  /** 0 = nessuna gestione, 1 = solo attività, 2 = attività e ricette */
  public MagazziniEsterni: number = 0;
  /** Se true fa mostrare le giacenze a 0 nell'app */
  public ShowZeroStock: boolean = false;
  public MaxAziende: number = 50;
  // public MasterAPP: boolean = false;
  public AutoSync: boolean = false;
  // public DeleteSync: boolean = true;
  /** Se > 0, forza la validità del login a `x` minuti. Default configurazione siti = 240. */
  public ValiditaLogin: number = 600;
  /** Se impostato a false movimenti, consistenze e prelievi nascono in stato eseguito. */
  public Bozza_Zoo: boolean = false;
  /** Determina la percentuale bloccante di sovrapposizione dei poligoni*/
  public BloccoSovrapposizione: number = 0;
  /** Attiva il menù degli overlay nella mappa. */
  public VisualizzaOverlay: boolean = false;
  /** Posso configurare il massimo livello di zoom nella mappa per la selezione delle aziende da sincronizzare */
  public MaxZoomMappaSync: number = 16;
  /** Posso configurare il minimo livello di zoom nella mappa per la selezione delle aziende da sincronizzare */
  public MinZoomMappaSync: number = 12;
  public LeggiStorico: boolean = false;
  /** L'intervallo temporale in minuti che passa fra un salvataggio temporaneo e l'altro (0,25 min = 15 secondi) */
  public SavingInterval: number = 0.25;
  /** soglia del test della banda (in kb) */
  public testConnectionValue: number = 400;
  /** esprime la massima dimensione dell'allegato che può essere spedito da APP a WEB */
  public DocumentSizeLimit: number = 20;
  public StazioniMeteo: boolean = true;
  public tipologiaDocumentoMeteo: number = -30;

  /////////////////////////////////////////////////////////////////////////////

  /**
   * Esegue il parsing delle impostazioni a partire da una stringa.
   * @param valueString stringa di cui eseguire il parsing
   */
  parse(valueString: string): void {
    let parsed;
    try {
      parsed = JSON.parse(valueString);
    } catch (e) {
      console.error("ImpostazioniApp: Error while parsing the string");
    }
    this.copyValue(parsed, true);
  }

  /**
   * Copia il valore delle impostazioni da un altro oggetto.
   * @param source oggetto da cui copiare i valori delle impostazioni
   */
  copyValue(source: any, elseUseDefault = false) {
    if (!source) return;
    for (let key in this) {
      this[key] = source[key] ?? this[key];
      // if (this.isMultiselect(key)) {
      //   this[key] = source[key].toString().split("|")
      // }
    }
    if (elseUseDefault) {
      this.TipoSincro = source["TipoSincro"] ?? 0;
      this.Bozza = source["Bozza"] ?? true;
      this.VisiteSpecie = source["VisiteSpecie"] ?? true;
      this.Offline = source["Offline"] ?? true;
      this.SincroLogin = source["SincroLogin"] ?? true;
      this.CheckUpdate = source["CheckUpdate"] ?? true;
      this.BloccoArea = source["BloccoArea"] ?? false;
      this.DistanzaMinima = source["DistanzaMinima"] ?? 5;
      this.DistanzaMassima = source["DistanzaMassima"] ?? 100;
    }
  }

  /**
   * Restituisce l'impostazione sotto forma di {@link ImpostazioniFormItem}.
   * @param field il campo che codifica l'impostazione d'interesse
   */
  getAdditionalData(field: string): ImpostazioniFormItem {
    let data = new ImpostazioniFormItem();
    data.guida = new Impostazione(0, field);
    data.valori = [];
    data.valoreCorrente = this[field]?.toString() || "";
    this.setControlType(data);
    this.setPossibleValues(data);
    this.setNotes(data);
    data.valori.forEach(v => v.valore = data.valoreCorrente || v.valore);
    return data;
  }

  private setControlType(data: ImpostazioniFormItem) {
    // Imposta default
    let type = typeof (this[data.guida.Impostazione_Des]);
    switch (type) {
      case "boolean":
        data.guida.Tipo_Campo = enum_TipoControllo.CASELLA_SPUNTA.toString();
        const stringValue = data.valoreCorrente.toString().toLowerCase();
        data.valoreCorrente = stringValue == 'true' || stringValue == '1';
        break;
      case "number":
        data.guida.Tipo_Campo = enum_TipoControllo.NUMERO_INTERO.toString();
        break;
      case "string":
        data.guida.Tipo_Campo = enum_TipoControllo.CASELLA_TESTO.toString();
        break;
      default:
        console.log(type);
        data.guida.Tipo_Campo = enum_TipoControllo.UNDEFINED.toString();
        break;
    }
    // Specifica casi particolari i.e. ddl, multiselect, ecc.
    if (this.isDdl(data.guida.Impostazione_Des)) {
      data.guida.Tipo_Campo = enum_TipoControllo.MENU_DISCESA.toString();
      data.valoreCorrente = +data.valoreCorrente;
    }
    if (this.isFloat(data.guida.Impostazione_Des)) {
      data.guida.Tipo_Campo = enum_TipoControllo.NUMERO_DECIMALE.toString();
    }
    if (this.isCustom(data.guida.Impostazione_Des)) {
      data.guida.Tipo_Campo = enum_TipoControllo.UNDEFINED.toString();
      data.guida.Impostazione_Cod = hashString(data.guida.Impostazione_Des) * 1000
        + enum_Impostazioni_Utenti.SUPERUSER_GiasAPP_IMPOSTAZIONI;
    }
    // if (this.isMultiselect(data.guida.Impostazione_Des)) {
    //   data.guida.Tipo_Campo = enum_TipoControllo.MULTISELECT.toString();
    //   data.valoreCorrente = data.valoreCorrente.toString().split("|");
    // }
  }

  private setPossibleValues(data: ImpostazioniFormItem) {
    switch (data.guida.Impostazione_Des) {
      case 'TipoSincro':
        data.valori.push(new GenericGuidaValoreImpostazione<number>(0, 'Default'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(1, 'Manuale'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(2, 'Automatico'));
        break;
      case 'Importazioni':
        data.valori.push(new GenericGuidaValoreImpostazione<number>(0, 'default',
          enum_Dati_App.OperazioniDiCampagna + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.OreMacchineCdG + '_' + enum_Import_App.Nessuno + '|' +
          enum_Dati_App.Ricette + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.RilieviEVisiteConRilievi + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.Visite + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.DocumentiFileFotoFilmatiAudio + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.MovimentiDiMagazzino + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.Acquisti + '_' + enum_Import_App.Completo + '|' +
          enum_Dati_App.ManutenzioniMacchine + '_' + enum_Import_App.Completo
        ));
        break;
      case 'InviaDati':
        data.valori.push(new GenericGuidaValoreImpostazione<number>(0, 'prof.AppLabelInviaDatiNoInvio'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(1, 'prof.AppLabelInviaDatiInvioNonScelte'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(2, 'prof.AppLabelInviaDatiNonVisibili'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(3, 'prof.AppLabelInviaDatiNonScelteNonVisibili'));
        break;
      case 'TipoValidita':
        data.valori.push(new GenericGuidaValoreImpostazione<number>(0, 'Default'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(1, 'AnnoAgrario'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(2, 'Manuale'));
        break;
      case 'MagazziniEsterni':
        /** 0 = nessuna gestione, 1 = solo attività, 2 = attività e ricette */
        data.valori.push(new GenericGuidaValoreImpostazione<number>(0, 'prof.NessunaGestione'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(1, 'prof.SoloAttivita'));
        data.valori.push(new GenericGuidaValoreImpostazione<number>(2, 'prof.AttivitaERicette'));
        break;
      case 'RestoreDati':
        data.valori.push(new GenericGuidaValoreImpostazione<string>('0', 'Disabilitato'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('1', 'Abilitato'));
        break;
      case 'Prodotti':
        data.valori.push(new GenericGuidaValoreImpostazione<string>('', 'Disabilitato'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('10', 'Semente'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('210', 'TrasformatiVegetali'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('10,210', 'prof.SementeETrasformatiVegetali'));
        break;
      case 'FiltroLavorazioni':
        data.valori.push(new GenericGuidaValoreImpostazione<string>('', 'Nessuna'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('162', 'prof.AltreLavorazioni'));
        break;
      case 'Custom':
        data.valori.push(new GenericGuidaValoreImpostazione<string>('1', 'prof.AppValueLabelCustomMode'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('2', 'prof.AppValueLabelOfflineMode'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('3=10', 'prof.AppValueLabelForceSyncOverSemente'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('4', 'prof.AppValueLabelForceOfflineSyncOverPlants'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('6', 'prof.AppValueLabelSortPlantsCropFirst'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('7', 'prof.AppValueLabelForceSyncOverSupplier'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('8', 'prof.AppValueLabelSupInHa'));
        break;
      case 'Lingua':
        data.valori.push(new GenericGuidaValoreImpostazione<string>('it', 'Italiano'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('en', 'Inglese'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('fr', 'Francese'));
        data.valori.push(new GenericGuidaValoreImpostazione<string>('pt', 'Portoghese'));
        break;
      case 'tipologiaDocumentoMeteo':
        data.valori.push(new GenericGuidaValoreImpostazione<number>(-30, 'prof.AppValueLabelStazioneMeteo'));
        break;
      default:
        data.valori.push(new GenericGuidaValoreImpostazione<number>(0, 'prof.AppLabel' + data.guida.Impostazione_Des));
        break;
    }
  }

  private setNotes(data: ImpostazioniFormItem) {
    const withTooltip = ["SpecieProdotti", "Lingua", "Custom", "ImportAgenda", "SavingInterval"];
    if (withTooltip.includes(data.guida.Impostazione_Des)) {
      data.guida.Note = 'prof.AppTooltip' + data.guida.Impostazione_Des;
    }
  }

  private isDdl(field: string): boolean {
    const ddls: string[] = ["TipoValidita", "InviaDati", "TipoSincro", "MagazziniEsterni", "RestoreDati",
      "Prodotti", "FiltroLavorazioni", "tipologiaDocumentoMeteo"];
    return ddls.includes(field);
  }

  private isFloat(field: string): boolean {
    return ['SavingInterval'].includes(field);
  }

  private isMultiselect(field: string): boolean {
    const multi: string[] = []; //, "Custom", "Lingua"
    return multi.includes(field);
  }

  private isCustom(field: string): boolean {
    return customAppSettings.includes(field);
  }

}

/**
 * Funzione usata dalle impostazioni app per calcolare un hash per i campi dell'oggetto {@link ImpostazioniApp}.
 * @param str La stringa da hashare (nome del campo delle impostazini app).
 * @returns un intero che identifica la stringa.
 */
export function hashString(str: string): number {
  let hash = 0;
  if (str.length === 0) return hash;
  for (let i = 0; i < str.length; i++) {
    const char = str.charCodeAt(i);
    hash = (hash << 5) - hash + char;
    hash = Math.trunc(hash); // Convert to 32-bit integer
  }
  return Math.abs(hash); // Ensure the hash is positive
}
