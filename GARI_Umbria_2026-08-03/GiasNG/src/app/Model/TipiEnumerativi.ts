export enum enum_PagineGiasNG {
  Pagina_Corrente = 0,
  Pagina_Menu_Anagrafica = 1,
  Pagina_Menu_Anagrafica_Imprese = 10,
  Pagina_Menu_Anagrafica_Centri = 11,
  Pagina_Menu_Anagrafica_Catasto = 12,
  Pagina_Menu_Anagrafica_Campi = 13,
  Pagina_Menu_Anagrafica_Impianti = 14,
  Pagina_Menu_Anagrafica_Macchine = 15,
  Pagina_Menu_Anagrafica_Contatti = 16,
  Pagina_Menu_Anagrafica_Fabbricati = 17,
  Pagina_Edit_Impresa = 2,
  Pagina_Edit_Centro = 3,
  Pagina_Edit_Campo = 4,
  Pagina_Edit_Catasto = 5,
  Pagina_Edit_AppezzamentoGlobal = 6,
  Pagina_Edit_Fabbricato = 7,
  Pagina_Edit_Contatto = 8,
  Pagina_Edit_Macchina = 9,
  Pagina_Menu_Agenda = 20,
  Pagina_Edit_Attivita = 21,
  Pagina_Configurazione_Operazioni_Culturali = 22,
  Pagina_Menu_Profilazione = 30,
  Pagina_Profilazione_Import_Utenti = 31,
  Pagina_Menu_Gruppi_Merce_Anagrafica = 40,
  Pagina_Menu_Gruppi_Merce_Autorizzazioni = 41,
  Pagina_AmministrazioneSistema_ConsultaSincroDatiApp = 50,
  Pagina_Menu_Visite = 60,
  Pagina_Edit_Visite = 61,
  Pagina_Valutazioni = 70,
  Pagina_Valutazioni_Edit = 71,
  Pagina_PianoConti = 80,
  Pagina_Menu_Zoo = 90,
  Pagina_Budget_Menu_Anagrafica_Imprese = 100,
  Pagina_Budget_Menu_Anagrafica_Centri = 110,
  Pagina_Budget_Menu_Anagrafica_Catasto = 120,
  Pagina_Budget_Menu_Anagrafica_Campi = 130,
  Pagina_Budget_Menu_Anagrafica_Impianti = 140,
  Pagina_Budget_Menu_Anagrafica_Macchine = 150,
  GestioneDisciplinari_Verifica_Disciplinare = 160,
  Pagina_GIS = 200,
  Pagina_GIS_cfg_proiezioni = 201,
  Pagina_Dashboard = 999,
  Pagina_Preferiti_Config = 1000,
  Pagina_Gruppi_Raccolta = 300,
  Pagina_Requisiti_Stabilimento = 301,
  Pagina_Requisiti_Stabilimento_PianoColturale = 302,
  Pagina_Requisiti_Stabilimento_Contratti = 303,
  Pagina_Dati_Previsionali_Colture = 305,
  Pagina_Requisiti_Stabilimento_VD_PianoColturale = 306,
  Pagina_Requisiti_Stabilimento_VD_Contratti = 307,
  Pagina_Confronto_Piano_Colturale = 308,
  Pagina_Filtro_Ricerca = 309,
  Pagina_Export_QdC_To_Agea = 310,
  Pagina_Profilazione_Imprese = 311,
  Pagina_Analisi_Terreno_Menu = 312,
  Pagina_Analisi_Terreno_Edit = 313,
  Pagina_Report_Impiego_Prodotti_Fitosanitari = 314,
  Pagina_Lettura_Contatori = 315,
  Pagina_Report_Abilitazione_PdC = 316,
  Pagina_Domanda_Irrigua = 320,
  Pagina_Terapie = 321,
  Pagina_Menu_Rilievi = 322,
  Pagina_Trattamento_Zoo = 323,
  Pagina_Codifiche_Sistemi_Esterni = 324,
  Pagina_Terapia_Zoo = 325,
  Pagina_Exe_Terapia_Zoo = 326,
  Pagina_Gestione_Carro = 327,
  Pagina_SostenibitaCO2_SelezionePerimetro = 329,
  //Pagina_SostenibitaCO2_GestioneCO2 = 328,
  Pagina_RischiMeteo_CalcoloRischi = 330,
  Pagina_SostenibitaCO2_CreazioneToken = 331,
  Pagina_RischiH20_SelezionePerimetro = 332,
  Pagina_Scadenza_Reinnesco_Trappole = 333,
  Pagina_PesateAccrescimento = 334,
  Pagina_DSS_Nutrizione = 335
}

export enum enum_PagineAgronicaSincro {
  MenuPrincipale = 0,
  Sincro_Harvard = 1,
  Sincro_Agrea_OP = 2,
  Import_DDT_Seled = 3,
  Importazione_Giacenze = 4,
  MenuAnagrafiche = 5,
  MenuMagazzino = 6,
  ImportazioneDaAgrea = 7,
  ImportazioneDaAnagrafeEmiliaRomagna = 8,
  ImportazioneDaAvepa = 9,
  ImportazioneDaAnagrafeBA = 10,
  ImportazioneDaAgreaMassiva = 11,
  Import_Anagrafiche_Seled = 12,
  Import_Notifica_AgriBio = 13,
  SincroCatasto_ImportPC_AccessOP = 14,
  Export_Sigpa = 15,
  ImportaContatti_XLS2GIAS = 16,
  ImportaImprese_XLS2GIAS = 17,
  ImportazioneMassivaDaAnagrafeBA = 18,
  Sincronizzatore_Apofruit = 19,
  ImportazioneMagazzinoXMLPubblico = 20,
  esportazioneSpesometro = 21,
  Import_Anagrafiche_Agenda_Seled = 22,
  ImportazioneDaAvepa_Viticolo = 23,
  esportazioneProfis = 24,
  Importatore_UNIFORMA = 25,
  ImportazioneDaAnagrafeBA_Veneto = 26,
  ImportazioneDaBrogliaccioSIAN = 27,
  INDICODE_Edi_Euritmo = 28,
  /** non c'è una pagina, sono in agenda AnalisiRitiriTabacco.aspx, RintracciaLotto.aspx e
   *  nel sincro Import_Macchine_Forni.aspx
   */
  OPTA_ImportaRicevimenti_TracciaColli = 29,
  ImportazioneDaAvepaCatasto_Excel = 30,
  FFConferimenti_AltriDB = 31,
  EsportazioneBolleAccettazione2AltroCliente = 32,
  EsportazioneConferimentiPomodoro_Agrea = 33,
  EsportazioneGias2JDEdwards = 34,
  ImportazioneAPOT = 35,
  ImportazioneSISCO = 36,
  ImportazioneCASALASCO = 37,
  EsportazioneSQNPI = 38,
  EsportazioneZESPRI = 39,
  Import_Arpea = 40,
  Import_Artea = 41,
  ImportazionePC_Anteprima = 42,
  Import_SIARL = 43,
  ImportazioneCIO = 44,
  EsportazioneConferimentiFF = 45,
  Import_AGEA = 46,
  EsportazioneAGEATxtCsv = 47,
  Import_AGREA_CSV = 48,
  EsportazioneSISCO = 49,
  Export_EURESYS = 50,
  Configurazione_Servizi = 51,
  Import_Agea_Coordinamento_Massiva = 52,
  Import_Agea_GIS = 53,
  Import_Agrea_Grafico = 54,
  Import_Bizerba = 55,
  Import_JDE = 56,
  EsportazioneGias2JDEdwards_BS = 57,
  Sincronizzatore_ArcView = 58,
  Import_dbwin_DateRaccolta = 59,
  Import_Anagrafiche_Zespri = 60,
  EsportazioneBolleAccettazione2AltroCliente_BS = 61,
  ImportazioneCASALASCO_Raccolte = 62,
  Piano_Prenotazione_Piante = 63,
  Import_AnalisiCampioniMaselli = 64,
  Codifica_ProdottiAPP = 65,
  Import_Conferimenti_Pomodoro = 66,
  Export_Conferimenti_Pomodoro = 67,
  Import_Anagrafiche_COPROB = 68,
  Esportazione_Trapianti_Agribologna = 69,
  Codifica_Prodotti_Aziendali = 70,
  Codifica_Specie_Vegetali = 71,
  Codifica_Varieta = 72,
  ImportazionePcgAvepa = 73,
  Esportazione_Enogis = 74,
  Esportazione_ARTEA = 75,
  ImportPianoColturaleDaShapeFile = 76,
  Esportazione_xFarm = 77,
  SincronizzatoreBDN = 78,
  ImportazioneParmaFrance = 79,
  ImportPianoColturaleDaKOBO = 80,
  ZooNogmo = 81,
  ImportMatricoleMadri = 82,
  ImportazioneUfficiZona = 83,
  G2GReverse = 84,
  ConsorzioBDN = 85,
  VisualizzatoreSincro = 86,
  Esportazione_Horta = 87,
  ImportazioneFileAnalisiZoo = 88,
  RegistrazioneMassivaBDN_Ingressi = 89,
  RegistrazioneMassivaBDN_Uscite = 90,
  SincronizzazioneMassimaStalleVetInfo = 91,
  SincronizzazioneStalleBDN = 92,
  SincronizzazioneMassivaStalleBDN = 93,
  Esportazione_Horta_Orzo = 94,
  InvioTrattamentiZooVetInfo = 95
}

export enum enum_TipoSincronizzazione_BDN {
  SincronizzaBDN = 1,
  InviaCapiBDN = 2,
  Genera_Modello4 = 3,
  GeneraCarichi = 4,
  GeneraScarichi = 5,
  Registra_Modello4 = 6,
  CancellaCarichi = 7,
  CancellaScarichi = 8,
  SincronizzaVetInfo = 9,
  VisualizzaSincro = 10,
  GeneraCarichiMassivo = 11,
  GeneraScarichiMassivo = 12,
  SincronizzaStalleMassivoVetInfo = 13
}

export enum enum_TipoSincronizzazione_Treatments {
  GeneraTrattamenti = 1,
  CancellaTrattamenti = 2,
  GeneraTrattamentiMassivo = 3,
  ControlloGiacenzeVetInfo = 4
}

export enum enum_AnagraficaNgTabs {
  imprese = 1,
  centri = 2,
  fabbricati = 3,
  catasto = 4,
  campi = 5,
  impianti = 6,
  contatti = 7,
  macchine = 8
}

export function getMasterWithEditPages(): Array<any> {
  return [
    {
      masterPage: enum_PagineGiasNG.Pagina_Menu_Anagrafica,
      editPage: [enum_PagineGiasNG.Pagina_Edit_Campo, enum_PagineGiasNG.Pagina_Edit_Contatto,
      enum_PagineGiasNG.Pagina_Edit_Fabbricato, enum_PagineGiasNG.Pagina_Edit_Catasto,
      enum_PagineGiasNG.Pagina_Edit_AppezzamentoGlobal, enum_PagineGiasNG.Pagina_Edit_Centro,
      enum_PagineGiasNG.Pagina_Edit_Impresa, enum_PagineGiasNG.Pagina_Edit_Macchina]
    },
    {
      masterPage: enum_PagineGiasNG.Pagina_Menu_Agenda,
      editPage: [enum_PagineGiasNG.Pagina_Edit_Attivita]
    },
    {
      masterPage: enum_PagineGiasNG.Pagina_Valutazioni,
      editPage: [enum_PagineGiasNG.Pagina_Valutazioni_Edit]
    },
    {
      masterPage: enum_PagineGiasNG.Pagina_Menu_Visite,
      editPage: [enum_PagineGiasNG.Pagina_Edit_Visite]
    },
    {
      masterPage: enum_PagineGiasNG.Pagina_Analisi_Terreno_Menu,
      editPage: [enum_PagineGiasNG.Pagina_Analisi_Terreno_Edit]
    }
  ];
}

/**
 * Codifica i permessi posseduti da un utente.
 * Utilizzare {@link enum_Security_Operazione} per verificare la tipologia di permesso.
 */
export enum enum_Security_Attivita {

  Gest_Menu = 1,
  Gest_AvvisiMessaggi = 2,
  Gest_Messaggi_AccessoMenu = 2,
  /** Gestisce accesso alla gestione di utenti e permessi nel gias vecchio.
   * Per la gestione della profilazione NG vedi {@link Profilazione_NG}.
   */
  Gest_UtentiPermessi = 3,
  Gest_AnagraficaAzienda = 4,
  Anagrafica_AccessoMenu = 4,
  Gest_Contabilita = 5,
  Sementieri_UtenteVedeTutto = 6,
  Gest_Stampe = 7,
  Gest_Certificazioni = 8,
  Gest_VerificaScadenze = 9,
  Gest_Magazzino = 10,
  Agenda_AccessoMenu = 11,
  Gest_CartografiaAziendale = 12,
  /**
   * IN SCRITTURA:
   * - Usato nella pagina Utenti e Permessi per modificare le impostazioni utente
   */
  Gest_UtentiImpostazioni = 13,

  // BUCO 14 15 16 17

  Gest_ImpiantiVegetali = 18, // vecchio!!! non va più usato!!!

  Gest_Prodotti = 19,
  Gest_Stalle = 20,
  Gest_AnalisiCosti = 21,
  Gest_ValidazioneDati = 22,
  Gest_Certificati_AccessoMenu = 23,
  Gest_Certificati_AttivaDisattiva = 24,
  Gest_Certificati_M005 = 25,
  Gest_Certificati_M006 = 26,
  Gest_Certificati_M007 = 27,
  Gest_Certificati_M014 = 28,
  Gest_CartellaAziendale_PAP_Vegetale = 29,
  Gest_CartellaAziendale_Notifiche = 30,
  Gest_CartellaAziendale_VisiteIspettive = 31,
  Gest_CartellaAziendale_Irregolarita = 32,
  Gest_Ingredienti = 33,
  Gest_CartellaAziendale_DelibereComm = 34,
  Gest_CartellaAziendale_PAP_Zootecnico = 35,
  Gest_CartellaAziendale_PAP_Preparazioni = 36,
  Gest_CartellaAziendale_Deroghe = 37,
  Gest_CartellaAziendale_AccessoMenu = 38,
  Gest_CartellaAziendale_CompartoFiliera = 39,
  Gest_Messaggi_SegreteriaTecnica = 40,
  // ??????????????
  // Ricezione_Messaggi_UST = 40

  // ManutenzioneArchivi_CoefficientiUBA = 41
  Gest_CartellaAziendale_PeriodiControllo = 42,
  ManutenzioneArchivi_SuperficieParticelle = 43,
  ManutenzioneArchivi_AccessoMenu = 44,

  // BUCO 45

  ManutenzioneArchivi_ModificaPIVA = 46,
  ManutenzioneArchivi_GestioneVarieta = 47,
  ManutenzioneArchivi_GestioneFormulati = 48,
  ManutenzioneArchivi_GestioneDisciplinari = 49,
  // ManutenzioneArchivi_ImportiSpecieVegetali = 50
  // ManutenzioneArchivi_GestioneFasiFenologiche = 51
  // ManutenzioneArchivi_GestioneCalibriFrutti = 52
  // ManutenzioneArchivi_ImportiQuoteControllo = 53

  // BUCO 54

  // ManutenzioneArchivi_GestioneIndiciMaturita = 55

  Anagrafica_Impresa = 56,
  Anagrafica_CentroAziendale = 57,
  Anagrafica_Campo = 58,
  Anagrafica_Appezzamento = 59,
  Anagrafica_Impianto = 60,
  Anagrafica_Fabbricato = 61,
  Anagrafica_ParticellaCatastale = 62,
  Anagrafica_Contatto = 63,
  Anagrafica_ParcoMacchine = 64,
  ManutenzioneArchivi_SbloccaAnagrafe = 65,
  // ManutenzioneArchivi_GestioneSpecieAnimali = 66

  Anagrafica_GestioneAllegati = 67,

  // ManutenzioneArchivi_PianificazioneInterventi = 68
  Agenda_OperazioniMultiAziendali = 68,
  ManutenzioneArchivi_MultiCancellazioneInterventi = 69,
  ManutenzioneArchivi_GestioneFertilizzanti = 70,
  Gest_Analisi_AccessoMenu = 71,
  Gest_Analisi_Cartografia = 72,

  // ManutenzioneArchivi_GestioneTipologieVarietali = 73
  ManutenzioneArchivi_GestioneSpecieVegetali = 74,
  Stampe_Esportazione_OP_Inv = 75,
  Stampe_Esportatore_Universale = 76,
  SupportoDecisioni_AccessoMenu = 77,
  SupportoDecisioni_PianoConcimazione = 78,
  Agenda_Operazioni_Blocco = 79,
  Agenda_Operazioni_Sblocco = 80,
  ManutenzioneArchivi_GestioneMigrazionePoliennale = 81,

  // Report Riconversione Varietale x AgriBologna
  Stampe_Riconversione_Varietale = 82,
  VerificaConformita_Richieste = 83,
  VerificaConformita_Gestione = 84,
  ManutenzioneArchivi_RevisioneDB = 85,

  // Esportazione Rintraccio x ARP
  Stampe_Esportazione_Rintraccio = 86,

  // Report Impegno Produzione Soci x AgriBologna
  Stampe_Impegno_Produzione_Soci = 87,
  Stampe_Esportazione_OP_Gest = 88,
  ManutenzioneArchivi_ImportaAnagrafiche_XLS2GIAS = 89,
  Gest_Ricette = 90,
  Anagrafica_RapportiContabili = 91,

  // Stampe_Bolle_Fatture = 92
  Stampe_Contabilita = 92,
  ACC_Configurazione = 93,
  ACC_Promozioni = 94,
  ACC_Vetrina = 95,
  ACC_AccessoMenu = 96,
  ManutenzioneArchivi_CodificaProdottiAziendali = 97,
  ManutenzioneArchivi_ModificaCodContatto = 98,
  ManutenzioneArchivi_ImportaAnagraficheAnimali_XLS2GIAS = 99,
  Gest_CartellaAziendale_Condizionalita = 100,
  ManutenzioneArchivi_Esporta_CodificheCultivar = 101,
  ManutenzioneArchivi_Esporta_CodificheSpecieVegetali = 102,
  Gest_Pianificazione_Produzione_Vegetale = 103,
  Gest_PUA = 104,
  Stampe_Esportazione_OP_Gest_Coop = 105,
  Registri_Cantina = 106,
  ManutenzioneArchivi_Importa_DDTRicevuti = 107,
  ManutenzioneArchivi_Importa_RaccolteConferimenti_DaRintraccio = 108,
  ManutenzioneArchivi_Importa_DDTRicevuti_AgriOK = 109,
  ManutenzioneArchivi_Importa_Anagrafe_EmiliaRomagna = 110,
  ManutenzioneArchivi_EsportaXMLAnagrafiche_GIAS = 111,
  ManutenzioneArchivi_EsportaConferimenti_JDEdwards = 112,
  ManutenzioneArchivi_Importa_AGREA = 113,
  ManutenzioneArchivi_Importa_AVEPA = 114,
  Contabilita_Operazioni_Blocco = 115,
  Contabilita_Operazioni_Sblocco = 116,
  ManutenzioneArchivi_ImpostazionePasswordServizi = 117,
  ManutenzioneArchivi_Importa_DDT_Pianocolturale = 118,
  Raccolta_Dati_PAC_UMA_Brogliaccio = 119,
  ManutenzioneArchivi_Importa_Anagrafe_BA = 120,
  CheckList_Sicurezza_Lavoro = 121,
  Report_Accettazione_DaDiversi = 122,
  ManutenzioneArchivi_EsportaConferimentiPomodoro_Agrea = 123,
  Gest_CartellaAziendale_MonitoraggioCE = 124,
  ManutenzioneArchivi_BolleAccettazione2AltroCliente = 125,
  Stampe_RegistroCaricoScarico_Pomodoro = 126,
  ProfilazioneImpresa = 127,
  Utilizzo_Dati_Meteo = 128,
  ManutenzioneArchivi_Importa_UfficiZona = 129,
  ManutenzioneArchivi_Importa_AGREA_Massiva = 130,
  PianiCampionamento_GestioneLotti = 131,
  PianiCampionamento_GestioneWorkFlow = 132,
  PianiCampionamento_GestioneAnalisi = 133,
  PianiCampionamento_InterrogazioneBloccoSblocco = 134,
  Anagrafica_MultiModificaSupImp = 135,
  ManutenzioneArchivi_SincronizzazioneHarvard = 136,
  Profilazione_DefaultSpecie_Globali = 137,
  Gest_CartellaAziendale_GlobalGap = 138,
  ManutenzioneArchivi_Importa_AGREA_OP = 139,
  ManutenzioneArchivi_Import_DDTFatture_Seled = 140,
  ManutenzioneArchivi_SincronizzazioneArcView = 141,
  ManutenzioneArchivi_Importa_Giacenze = 142,
  ManutenzioneArchivi_Importa_AVEPA_Vino = 143,
  ManutenzioneArchivi_ImportDateRaccolta_Harvard = 144,
  /**
   * IN SCRITTURA:
   * - Usato nella pagina Utenti e Profili per visualizzare le statistiche della pagina.
   * -
   */
  Statistiche_Sito = 145,
  ManutenzioneArchivi_Import_Anagrafiche_Seled = 146,
  ManutenzioneArchivi_Importa_Notifica_AgriBio = 147,
  ManutenzioneArchivi_SincroRaccoltaCCCI = 148,
  Anagrafica_Appezzamento_CancellazioneMultipla = 149,
  PianiSemina_Tabelle = 150,
  PianiSemina_Gestione = 151,
  Meteo_Tabelle = 152,
  Gestione_Etichette = 153,
  PianiCampionamento_GestioneAnalisiXLaboratori = 154,
  ManutenzioneArchivi_SincroCatastoImportPianificazioneAccessOP = 155,
  Gest_CartellaAziendale_CheckCOOP = 156,
  PianiCampionamento_GestioneImpostazioni = 157,
  PianiCampionamento_GestionePianoProduttivo = 158,
  PianiCampionamento_GestionePianoCampioni = 159,
  PianiCampionamento_ControlloValiditaCapitolati = 160,
  PianiCampionamento_Menu = 161,
  PianiCampionamento_ControlloValiditaQDC = 162,
  PianiCampionamento_GestioneBloccoSblocco = 163,
  PianiCampionamento_GestioneLaboratori = 164,
  SMART_RegistrazioneSmart = 165,
  SMART_AgendaOperazioniColturali = 166,
  SMART_GiasProfitosan = 167,
  SMART_GIS = 168,
  SMART_gestioneAnagrafica = 169,
  SMART_NuovaAzienda = 170,
  SMART_Nuovo_Centro = 171,
  SMART_NuovoAppezzamento = 172,
  SMART_NuovoContatto = 173,
  SMART_Gias = 174,
  SMART_Menu = 175,
  SMART_GestioneAnagrafica_Nodo = 176,
  ManutenzioneArchivi_AllineaCarenze = 177,
  manutenzioneArchivi_Esportazione_SIGPA = 178,
  Rilievo_Attivita = 179,
  Blocco_Modifica_PianificazioneVegetale = 180,
  Creazione_Automatica_CentriAziendali = 181,
  PianiCampionamento_Filtrone = 182,
  Budget_Menu = 183,
  Budget_Testata = 184,
  Budget_Import_Da_Anagrafe = 185,
  Budget_Dettagli_del_Piano = 186,
  Report_Incongruenze_CatastoVSAgrea = 187,
  Gestione_Servizi = 188,
  Stampa_Inglese = 189,
  Stampe_Italiano = 190,
  ManutenzioneArchivi_Importazione_Massiva_Anagrafe_BA = 191,
  ManutenzioneArchivi_Gias_2_Gias = 192,
  LinkGiasStrandard = 193,
  ManutenzioneArchivi_ImportaMagazzino_XmlPubblico = 194,
  ManutenzioneArchivi_Sincronizzatore_Apofruit = 195,
  Rilevamento_Smart_Gis = 196,
  ManutenzioneArchivi_Sincronizzatore_Valdoca = 197,
  Scadenziario_Menu = 198,
  Precision_Farming = 199,
  Link_AgronicaSementi = 200,
  Modifica_Contatti_Pubblici = 201,
  Cartografia_Catasto = 202,
  Cartografia_Esporta_Dati = 203,
  ManutenzioneArchivi_Importazione_Anagrafiche_Agenda_Seled = 204,
  ManutenzioneArchivi_Importazione_Anagrafiche_Viticolo_Avepa = 205,
  SupportoDecisioni_PianoConcimazioneMassivo = 206,
  PrenotazionePiante_Menu = 207,
  Creazione_Automatica_SemilavoratiVegetali = 208,
  ManutenzioneArchivi_Importazione_Catasto_Uniforma = 209,
  Consultazione_Meteo_Dati_Stazioni_Metos = 210,
  ManutenzioneArchivi_Importazione_Catasto_CantinaSoave = 211,
  // ManutenzioneArchivi_Importazione_Catasto_OPTA = 212

  PROFITOSAN_Elenco_Prodotti_DPI = 213,
  Gest_CartellaAziendale_Check_SchedaTecnicaTTI = 214,
  Gest_CartellaAziendale_Check_SchedaControlliTTI = 215,
  manutenzioneArchivi_VerificaEsportazioneAgendaVerso_SIGPA = 216,
  ManutenzioneArchivi_Importazione_OptaMacchine = 217,
  ManutenzioneArchivi_Importazione_OptaParticelle = 218,
  Gest_Pianificazione_Produzione_Vegetale_Ribalta = 219,
  ManutenzioneArchivi_Importa_Anagrafe_BA_Veneto = 220,
  Esportazione_RIBA_CBI = 221,
  NonConformita = 222,
  Agenda_Operazione_Di_Cura = 223,
  ManutenzioneArchivi_Importazione_Anagrafiche_Brogliaccio_SIAN = 224,
  SupportoDecisioni_LaboratorioControlloQualita = 225,
  Stampe_Esportazione_OP_Produttori = 226,
  Stampe_Esportazione_OP_Catasto = 227,
  Rintraccio_Operazione_Di_Cura = 228,
  ManutenzioneArchivi_Importa_AVEPA_Catasto = 229,
  Stampa_MovimentiMagazziniExcel = 230,
  SupportoDecisioni_LaboratorioControlloQualita_CreaApriDoc = 231,
  SupportoDecisioni_LaboratorioControlloQualita_Impostazioni = 232,
  SupportoDecisioni_LaboratorioControlloQualita_FiltraEsporta = 233,
  ManutenzioneArchivi_Importazione_Anagrafiche_COFRUTA = 234,
  Gest_CartellaAziendale_Check_AnalisiDatiCura = 235,
  ManutenzioneArchivi_Importazione_Anagrafiche_APOL = 236,
  AnalisiSchedeRilievi = 237,
  Rintracciabilità = 238,
  Stampa_SchedaCampagna_Massiva = 240,
  Gestione_Ordini_Piante = 241,
  ManutenzioneArchivi_Importazione_Anagrafiche_APOT = 242,
  /**
   * IN SCRITTURA:
   * - Usato nella pagina Utenti e Permessi per creare e modificare profili utenti
   */
  Gest_UtentiProfili = 243,
  /**
   * IN SCRITTURA:
   * - Usato nella pagina Utenti e Permessi per creare e modificare gruppi utenti e le impostazioni collegate a essi
   */
  Gest_UtentiGruppi = 244,
  ManutenzioneArchivi_Importazione_Anagrafiche_SISCO = 245,
  Macchine_Assegnazione_Pubblica = 246,
  Gest_UtentiAgendaBlocchi = 247,
  ManutenzioneArchivi_Importazione_Anagrafiche_Casalasco = 248,
  ReportPercorsi = 249,
  EsportazioneZespri = 250,
  EsportazioneSQNPI = 251,
  CheckList_Pratiche_Ecologiche_APOT = 252,
  CheckList_Formazione = 253,
  Gestione_Rifiuti = 254,
  ManutenzioneArchivi_MultiModificaInterventi = 255,
  NonConformita_Impostazioni = 256,
  NonConformita_FiltraEsporta = 257,
  NonConformita_Anagrafiche = 258,
  Cartografia_BufferZone = 259,
  Gest_UtentiImpostazioni_Avanzate = 265,
  ManutenzioneArchivi_Importa_ARPEA = 279,
  FiltraEdEsporta_PianiColturali = 280,
  ManutenzioneArchivi_Importa_ARTEA = 288,
  SupportoDecisioni_VerificaConformitaIAF = 289,
  Esportazione_RegioneER = 290,
  Importazione_SIARL = 291,
  ManutenzioneArchivi_Importazione_Anagrafiche_CIO = 292,
  ManutenzioneArchivi_EsportazioneConferimentiFF = 293,
  ManutenzioneArchivi_Importa_AGEA_RealTime = 296,
  ManutenzioneArchivi_Importa_AGEA_Coordinamento_Massiva = 298,
  ManutenzioneArchivi_Importa_AGEA_GIS = 299,
  // ############################################################
  // ------------ F&F Campionamento e Liquidazioni ------------
  FrashAndFood = 260,
  FF_Conferimenti = 261,
  FF_CampionamentoLiquidazioni_Anag = 262,
  FF_CampionamentoLiquidazioni_Movimenti = 263,
  FF_CampionamentoLiquidazioni_Liquidazioni = 264,
  FF_CampionamentoLiquidazioni_ValorUnaTantumAUltimoListino = 422,
  // ------------ F&F Campionamento e Liquidazioni ------------

  NonConformita_Testata_Crea = 266,
  NonConformita_Testata_Modifica = 267,
  NonConformita_Testata_Chiudi = 268,
  NonConformita_Testata_Elimina = 269,
  NonConformita_Fase_Crea = 270,
  NonConformita_Fase_Modifica = 271,
  NonConformita_Fase_Chiudi = 272,
  NonConformita_Fase_Elimina = 273,
  Analisi_Dati_Meteo = 274,
  Analisi_Curve_Maturazione = 275,
  NonConformita_Lista = 276,
  NonConformita_Lista_AncheDiAltriUtenti = 277,
  Analisi_Modelli_Previsionali = 278,
  GestioneAvanzataETabelleLookUp = 281,
  ReportSostenibilità = 282,
  Scadenzario_Lista = 283,
  Scadenzario_IndiciRicerca = 284,
  Visite_Lista = 285,
  Visite_Anagrafiche = 286,
  invioSMS = 287,
  Scadenzario_Impostazioni = 294,
  Gest_Prodotti_VisibilitaPubblica = 295,
  ManutenzioneArchivi_Import_Utenti_Da_Excel = 297,

  // ############################################################
  // ------------ AGRONICA MANUTENZIONE ------------
  // i permessi dell'Agronica Manutenzione vanno dal 300 al 349
  AgronicaManutenzione_AccessoMenu = 300,
  AgronicaManutenzione_SpecieVegetali = 301,
  AgronicaManutenzione_Varieta = 302,
  AgronicaManutenzione_CalibriFrutti = 303,
  AgronicaManutenzione_IndiciMaturita = 304,
  AgronicaManutenzione_FasiFenologiche = 305,
  AgronicaManutenzione_TipologieVarietali = 306,
  AgronicaManutenzione_GruppoFinalita = 307,
  AgronicaManutenzione_FormeAllevamento = 308,
  AgronicaManutenzione_ImpiantiIrrigazione = 309,
  AgronicaManutenzione_Portinnesti = 310,
  AgronicaManutenzione_UnitaMisura = 311,
  AgronicaManutenzione_MisuraAvversita = 312,
  AgronicaManutenzione_Fertilizzanti = 313,
  AgronicaManutenzione_Formulati = 314,
  AgronicaManutenzione_Disciplinari = 315,
  AgronicaManutenzione_CoefficientiUBA = 316,
  AgronicaManutenzione_SpecieAnimali = 317,
  // ECC...
  // ------------ AGRONICA MANUTENZIONE ------------

  // ############################################################
  // Fresh & Food Magazzino
  FF_Magazzino = 350,
  FF_Lavorazioni_PC = 351,
  FF_Lavorazioni_Terminalino = 352,
  // ------------ Fresh & Food Magazzino ------------

  ManutenzioneArchivi_ImportazioneHarvard = 353,
  ReportRaccolteGIS = 354,
  Brogliaccio = 355,
  ManutenzioneArchivi_ImportazioneRicetteDaInterscambioApp = 356,
  ManutenzioneArchivi_ImportazioneAnagraficheZespri = 357,
  ManutenzioneArchivi_ImportazioneQDCZespri = 358,
  ManutenzioneArchivi_EsportazioneAGEA = 359,
  GIS_SAT_AnalisiDatiSatellitari = 360,

  // Fatturazione Elettronica
  Contabilita_FattElettronica = 361,

  // gestione di permessi per chiamate a web service di provisioning (AgronicaWebApiProfilatore)
  Provisioning_agronica = 362,
  ManutenzioneArchivi_EsportazioneAgendaSISCO = 363,
  ManutenzioneArchivi_EsportazioneEuresys = 364,
  ManutenzioneArchivi_ConfigurazioneServizi = 365,
  Gest_PUA_2 = 366,
  G2GFiltraDatiPerInvioGias2Gias = 367,
  GIS_VisualizzazioneGestioneWMS = 368,
  Gestione_Listini = 369,
  Ordini_Acquisto = 370,
  Consegne_Acquisto = 371,
  Fatture_Acquisto = 372,
  Ordini_Vendita = 373,
  Consegne_Vendita = 374,
  Fatture_Vendita = 375,
  Correlazione_Righe_Vendita = 376,
  Statistiche_Vendita = 377,
  Consegne_Conferimento = 378,
  Numerazione_Documenti = 379,
  Gestione_Imballaggi = 380,
  Nuovo_Conferimento = 381,
  Nuovo_DDT_Vendita = 382,
  ManutenzioneArchivi_ImportazioneAnagraficheJDE = 383,
  ManutenzioneArchivi_ImportazionePianoColturaleExcel = 384,
  ManutenzioneArchivi_ImportazioneBizerba = 385,
  Gestione_MenuControlliALP = 386,
  Gestione_MenuControlliALP_Admin = 387,
  Blocca_Sblocca_Pratiche = 388,
  Importazione_AGREA_CSV = 389,
  Importazione_AGREA_Grafico = 390,
  Report_Analisi_PdC = 391,
  Audit_BIO_COPROB = 392,
  Audit_SQNPI_COPROB = 393,
  Stime_Analisi_Produzione = 394,
  Trasferimenti_Magazzino = 395,

  // ############################################################
  // Controllo di gestione
  Gestione_Anagrafiche_CdG = 396,
  Inserimento_CostiRicavi_Da_QdC_CdG = 397,
  Inserimento_CostiRicavi_CdG = 398,
  Gestione_Report_CdG = 399,
  Gestione_Completa_CdG = 400,
  // ############################################################

  VivaiAttivitaAdempimenti = 401,
  FiltraEdEsporta_PianiConcimazionePUA = 402,
  NuovaVisitaDaFiltroDiRicerca = 403,
  Pratiche_Filtri_Avanzati = 404,
  Gest_PUA_2_ImportaFertilizzazioniDaRegistro = 405,
  Gestione_Prezzi = 406,
  Scadenzario_Inser = 407,
  Scadenzario_Canc = 408,
  Documentale_Lista = 409,
  Documentale_Inser = 410,
  Documentale_Canc = 411,
  Documentale_Valid = 412,
  Angrafica_Prodotti = 414,
  Valori_Economici_legati_alle_Attivita = 415,
  Progetto_Piante = 416,
  ReportPercorsi_Amministrtore = 417,
  Prenotazione_Piante_OrdineDaRichiesta = 418,
  Prenotazione_Piante_SoloOrdini = 419,
  Prenotazione_Piante_RichiestaMaterialeVivaistico = 420,
  Importazione_Anagrafiche_Zespri = 421,
  Meteo_Modifica_creazioneStazioniProprietaVirtuali = 423,
  Biologico_ReportBio = 424,
  ManutenzioneArchivi_ImportazioneAnalisiCampioniMaselli = 425,
  ManutenzioneArchivi_CodificaProdottiDaInterscambioApp = 426,
  Prenotazione_Piante_nuovo_ordine_a_vivaio = 427,
  Prenotazione_Piante_nuova_richiesta_materiale_vivaistico = 428,
  PianiCampionamento_MarketAccess = 429,
  Gestione_Contratti_Conferimento = 430,
  Gis_Gestione_Di_SR_e_Trasfromazioni_Fra_SR = 431,
  PianiCampionamento_RichiestaAnalisiMultiple = 432,
  Nuovo_Conferimento_Pomodoro = 433,
  PUA_Blocca_Sblocca = 434,
  PianoConcimazione_Blocca_Sblocca = 435,
  Contabilita_MVVElettronico = 436,
  PianiCampionamento_MarketAccess_Globale = 437,
  Contabilita_Clienti = 438,
  Contabilita_Fornitori = 439,
  Interferenze_MenuPrincipale_Accesso = 440   // Ex TipiEnumerativiSementieri MenuPrincipale_Accesso = 1
  ,
  Interferenze_UtentiPermessi_Gestione = 441  // Ex TipiEnumerativiSementieri UtentiPermessi_Gestione = 2
  ,
  Interferenze_Configurazione_Distanze = 442  // Ex TipiEnumerativiSementieri Interferenze_Configurazione_Distanze = 3
  ,
  Interferenze_Configurazione_Colore = 443    // Ex TipiEnumerativiSementieri Interferenze_Configurazione_Colore = 4
  ,
  Interferenze_Visualizzazione_Ridotta = 444  // Ex TipiEnumerativiSementieri Interferenze_Visualizzazione_Ridotta = 5
  ,
  Interferenze_Visualizzazione_Estesa = 445   // Ex TipiEnumerativiSementieri Interferenze_Visualizzazione_Estesa = 6
  ,
  Audit_Budwood_Projects = 446,
  Menu_Agenda_Visualizzazione_Dettagli_Operazioni = 447,
  Anagrafiche_Conferimento = 448,
  Configurazione_Lavorazioni = 449,
  Undo_Pratiche = 450,
  Gias2JohnDeere = 451,
  Statistiche_Acquisto = 452,
  ManutenzioneArchivi_ImportazioneConferimentiPomodoro = 453,
  Gestione_Carburanti_UMA = 454,
  Richiesta_UMA = 455,
  Rendicontazione_UMA = 456,
  Approvazione_Richiesta_UMA = 457,
  Approvazione_Rendicontazione_UMA = 458,
  Import_Anagrafiche_COPROB = 459,
  Configurazione_UMA = 460,
  Ordini_Lavorazioni = 461,
  Utility_Cambio_CF_Utente = 462,
  UMA_Vendite_Carburanti = 463,
  Esportazione_Trapianti_Agribologna = 464,
  PianiCampionamento_RisultatiAnalisiInGrigliaAnalisi = 465,
  Configurazione_Modelli_Previsionali = 466,

  Riepilogo_UMA = 467,
  ImportazionePcgAvepa = 468,
  Piano_Nutrizionale = 469,
  Visibilita_Aziende_UMA = 470,
  Contabilita_DAA_Elettronico = 471,
  Anagrafica_MultiModifica_PianoColturale = 472,
  Contabilita_CarichiScarichi_Magazzino = 473,
  Budget = 474,
  Budget_Gestione_Anagrafiche_CdG = 475,
  Budget_Inserimento_CostiRicavi = 476,
  Budget_Gestione_Report = 477,
  Budget_Valori_Economici_legati_alle_Attivita = 478,
  Budget_Anagrafiche_Colturali = 479,
  Audit_Convenzionale_Greenyard = 480,
  Audit_Biologico_Greenyard = 481,

  GIS_GestioneLayerPersonalizzati = 482,

  Esportazione_Enogis = 483,
  Esportazone_Artea = 484,

  PianiCampionamento_CompilazioneCapitolatiCliente = 485,

  DSS_Irrigazione = 486,
  Gruppi_Merce = 487,

  ReteAcqua_AnalisiDati = 494,

  Cartografia_VisualizzazioneTotale = 496,
  Cartografia_SetupVisualizzazione = 497,

  Agenda_AccessoMenu_NG = 502,
  Gest_CartografiaAziendale_NG = 503,
  Gest_AnagraficaAzienda_NG = 504,

  Anagrafica_Appezzamento_CopiaSposta = 506,
  Caricamento_Utilizzo_Mappe_Prescrizione_Personalizzate = 507,

  AggiornamentoMatricoleMadri = 508,

  SmartTractors = 509,
  SmartTractors_Parametrizzazione = 510,
  SmartTractors_InvioRicette = 511,

  GIS_Configurazione_Algoritmi_Cartografici = 512,
  GIS_Gestione_Parametri_Maschere_Raster = 513,

  /**
   * Gestisce l'accesso alla gestione di utenti e permessi NG.
   *
   * IN LETTURA:
   * - Usato nella pagina Utenti e Permessi per accedere alla pagina
   *
   * IN SCRITTURA:
   * - Usato nella pagina Utenti e Permessi per: creare un nuovo utente, modificare un utente, assegnare visibilità,
   * copiare visibilità, assegnare permessi, modificare intervallo validità permessi,
   * modificare intervallo finestre temporali, bottone privacy(?)
   */
  Profilazione_NG = 514,
  Visite_Lista_NG = 515,

  Gestione_GHG = 516,
  Configurazione_Operazioni_Colturali = 517,

  DomandaIrrigua = 518,
  DomandaIrrigua_Scheda = 519,
  DomandaIrrigua_Ricerca = 520,

  LettureContatoriAziendali = 521,
  ElaboraCalcoloTariffazione = 522,

  Valutazioni_Rischio = 523,
  Gruppi_Raccolta = 524,
  Requisiti_Stabilimento = 526,

  GisBulkExportSuLayer = 533,
  Budget_Ribaltamento_Su_Reale = 534,
  Dati_Previsionali_Colture = 535,
  Confronto_Piano_Colturale = 545,

  Widget_MultiAzienda = 546,
  Menu_Precedente = 547,
  UMA_Report_Controllo = 548,
  Visibilita_Viste_Grid = 549,
  Visibilita_Aziende_UMA_GestioneVisibilitaCompleta = 550,
  Menu_Stampe_New = 551,
  Filiera_Trasporti_COPROB = 552,
  FiltroRicerca_NG = 553,
  Esportazione_Agea_NG = 554,
  Statistometro_New = 555,
  Gestione_Servizi_NEW = 556,
  RegistrazioneMassiva_BDN = 557,
  UMA_Ricerca_Macrousi_Lavorazioni = 558,
  SincronizzazioneMassivaStalleVetInfo = 559,
  Visualizza_Comandi_Avanzati_Esportazione_Agea_NG = 560,
  ProfilazioneImprese_NG = 561,
  ImpostazioniImprese_NG = 562,
  /** Permette la lettura o modifica di tutti gli utenti (non solo sé stessi) */
  AmministrazioneUtenti = 564,
  Enquete_certification_Hevea_brasiliensis = 565,
  Banca_Cambiano_Azienda = 566,
  ImportazioneMassivaModelli4 = 567,
  CalcoloMassivo_Compliance_ISCC_AziendeInVisibilita = 568,
  InserisciDocNonConformeISCC = 569,
  ReportImpiegoProdottiFitosanitari = 570,
  LettureContatoriAziendali_NG = 571,
  GestioneAssociazioneAppezzamentiXParcoMacchine = 572,
  GestioneCache = 573,
  Terapie = 574,
  /** Gestisce la visibilità e l'accesso alla pagina delle operazioni zootecniche NG tramite il pulsante di menù
   * "Operazioni Zootcniche (new!)"*/
  MenuZooNG = 575,
  WidgetPrevisioniCostiRicaviAI = 576,
  MenuRilievi = 577,
  GestioneAppezzamentiTessitura = 578,
  GestioneAppezzamentiPendenza = 579,
  ModificaOperazioneinVerifica = 580,
  GestioneContributiACA = 581,
  TrattamentoZoo = 582,
  ZooProtocolliTerapeutici = 583,
  ZooIndicazioniTerapeutiche = 584,
  Audit_BIO_FILENI = 585,
  Audit_Fornitori_EUDR = 586,
  ManutenzioneArchivi_GestioneSistemi_Esterni = 587,
  Report_Abilitazione_PdC = 588,

  Invia_Stat_Matomo = 589,
  PianiCampionamento_PubblicazioneAnalisi = 590,
  AnagraficaGestioneRateiIrriguiMacchine = 591,
  Importazione_Farmacie = 592,
  UtentiGenerazioneCF = 594,
  Report_Zootecnia = 595,
  VerificaConformitaNG = 596,
  GIS_Configurazione_Algoritmi_Clustering = 597,
  ConfigurazioneModalitaPagamento = 598,
  WidgetReportChecklist = 599,
  InvioTrattamentiMassivoGiasVetInfo = 600,
  ZooTerapie = 601,

  GestioneCarro = 602,
  GestioneCarroConfig = 603,
  StatistometroReportDettagliServiziAzienda = 604,
  SostenibitaCO2_CalcoloSostenibitaCO2 = 605,
  RischiMeteo_CalcoloRischi = 606,
  SostenibitaCO2_CreazioneToken = 607,
  RischiH2O_CalcoloRischiH2O = 608,
  GestioneEserciziVincoli = 609,
  ZooPesatureAccrescimento = 610,
  DSS_Nutrizione = 611,
  // andare avanti da qui.....

  // ############################################################
  // GIASAPP

  GiasAPP_Permessi = 800,
  GiasAPP_NUOVA_RICETTA = 801,
  GiasAPP_NUOVO_INTERVENTO = 802,
  GiasAPP_INTERVENTI_DA_FARE = 803,
  GiasAPP_SCARICO_ORE = 804,
  GiasAPP_ENTRATAUSCITA = 805,
  GiasAPP_LAMIAPOSIZIONE = 806,
  GiasAPP_VISITE = 807,
  GiasAPP_DOCUMENTI = 808,
  GiasAPP_RILIEVI = 809,
  GiasAPP_GIS = 810,
  GiasAPP_InCab = 811,
  GiasAPP_PianoColturale = 812,
  GiasAPP_Magazzini = 813,
  GiasAPP_Macchine = 814,
  GiasAPP_Manutenzioni = 815,
  GiasAPP_DDT_Movimenti = 816,
  GiasAPP_Gias = 817,
  GiasAPP_Aziende = 818,
  GiasAPP_Centri = 819,
  GiasAPP_Isolamenti = 820,
  GiasAPP_DSS_Difesa = 821,
  GiasAPP_ConsultaSincroDatiAppDaWeb = 822,
  GiasAPP_Consiglio_Irriguo = 823,
  GiasAPP_Consiglio_Fertirriguo = 824,
  GiasAPP_Precision_Farming = 825,
  GiasAPP_Monitoraggio_Meteo = 826,

  // ############################################################
  // PROFITOSAN

  Profitosan_Home = 1000,
  Profitosan_Prodotto_Testata = 1001,
  Profitosan_Prodotto_Dettagli = 1002,
  Profitosan_Prodotto_Etichetta = 1003,
  Profitosan_Prodotto_SchedaSicurezza = 1004,
  Profitosan_RicercaProdotti = 1005           // codice/nome prodotto
  ,
  Profitosan_RicercaProdottiAvanzata = 1006   // campi impiego, sostanze attive, ditte, RMA
  ,
  Profitosan_RicercaProdottiSimili = 1007,
  Profitosan_Disciplinari = 1008,
  Profitosan_RMA = 1009,
  Profitosan_Prodotto_Decreto = 1010,
  Profitosan_Tunnel_Prodotto = 1011,

  // ############################################################
  // PianoConcimazione

  PianoConcimazione_CalcoloBilancio = 2000
}

export enum enum_TipoOperazioneDB {
  Lettura = 0,
  Scrittura = 1,
  Modifica = 2,
  Cancellazione = 3,
  Trasferimento = 4,
  Copia = 10
}

export enum enum_Security_Operazione {
  Lettura = 0,
  Scrittura = 1,
  Modifica = 2,
  Cancellazione = 3,
  Esecuzione = 4,
  Stampa = 5
}

export enum enum_LAVCOD {

  // #####################################################################
  // GRUPPO OPERAZIONE = 1 Rilievi in Campo
  RILIEVO_FALDA = 30,
  FASI_FENOLOGICHE = 79,
  INSTALLAZIONE_TRAPPOLE = 107,
  RILIEVO_AVVERSITA_TRAPPOLE = 110,
  RILIEVO_AVVERSITA_CAMPO = 113,
  RILIEVO_ERBE_INFESTANTI = 119,
  RILIEVO_PIOGGE = 126,
  REINNESCO_TRAPPOLE = 150,
  MONITORAGGIO_ACQUE = 164,

  // ------------------------------------------------------------
  // GRUPPO OPERAZIONE = 2 Rilievi alla Raccolta
  DANNI_RACCOLTA = 108,
  RILIEVO_INDICI_MATURITA = 109,
  RACCOLTA = 125,
  /** aka. Rilievo Produzione Prevista */
  RILIEVO_INDICI_RESE_RACCOLTA = 169,

  // ------------------------------------------------------------
  // GRUPPO OPERAZIONE = 3 Trattamenti
  CONCIA_SEME = 13,
  DISERBO = 18,
  TRATTAMENTO_ANTIPARASSITARIO = 74,
  TRATTAMENTO_FITOREGOLATORE = 103,
  DISTRIBUZIONE_INSETTI = 116,
  CONFUSIONE_SESSUALE = 118,
  DISORIENTAMENTO_SESSUALE = 121,
  CATTURE_MASSA = 122,
  GEODISINFESTAZIONE = 155,
  DISSECCAMENTO = 158,
  TRATTAMENTO_POST_RACCOLTA = 163,
  TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO = 165,
  CONFUSIONE_DISORIENTAMENTO_SESSUALE = 172,

  // ------------------------------------------------------------
  // GRUPPO OPERAZIONE = 4 LAVORAZIONI
  IRRIGAZIONE = 1,

  SEMINA = 2,
  TRAPIANTO = 71,
  TRAPIANTO_IN_SERRA = 151,
  SOVESCIO = 160,
  SOD_SEDDING = 68,

  ARATURA = 8,
  ANDANAMENTO = 9,

  FERTIRRIGAZIONE = 26,
  DISTRIBUZIONE_CONCIME = 14,
  CONCIMAZIONE_FOGLIARE = 123,
  DISTRIBUZIONE_AMMENDANTI = 124,
  SARCHIATURA_CONCIMAZIONE = 156,
  TRATTAMENTO_ANTIBUTTERATURA = 106,
  FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO = 166,

  MANUTENZIONE_IMPIANTI = 159,

  INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA = 173,
  //Stesso LAV_COD di ALTRE_OPERAZIONI
  RILIEVO_ATTIVITA = 162,

  ASPORTAZIONE_ORGANI_INFETTI = 120,
  ASSOLCATURA = 10,
  CARICO_MANUALE_FRUTTA = 78,
  CIMATURA = 12,
  DIRADAMENTO_MANUALE = 17,
  DISSODAMENTO = 19,
  ERPICATURA = 21,
  ERPICATURA_ROTANTE = 157,
  ESPIANTO = 81,
  ESTIRPATURA = 22,
  FALCIACONDIZIONATURA = 23,
  FALCIATURA_ERBAI = 25,
  FORMAZIONE_ARGINELLI = 27,
  FRANGIZOLLATURA = 31,
  FRESATURA = 32,
  GEBIATURA = 152,
  IMBALLO_FIENO_ROTOLI = 33,
  INTERRAMENTO_PAGLIE = 34,
  INTERVENTO_ANTIBRINA = 161,
  LAVORAZIONE_CONBINATA = 154,
  LAVORAZIONE_TRA_FILA = 82,
  LAVORAZIONE_SU_FILA = 83,
  LEGATURA = 84,
  LIVELLAMENTO = 40,
  MANUTENZIONE_ARGINI = 41,
  MESSA_DIMORA_PIANTE = 85,
  MIETITREBBIATURA = 46,
  MINIMUM_TILLAGE = 47,
  PACCIAMATURA = 48,
  POTATURA_SECCA = 87,
  POTATURA_VERDE = 88,
  PRESSATURA = 49,
  RACCOLTA_LEGNA_POTATURA = 90,
  RANGHINATURA = 53,
  RINCALZATURA = 55,
  RIPPATURA = 115,
  RIPUNTATURA = 56,
  RIVOLTAMENTO_FORAGGIO = 57,
  ROMPICROSTA = 153,
  RULLATURA = 58,
  SARCHIATURA = 59,
  SCARIFICATURA = 62,
  SCASSO = 63,

  TRINCIATURA = 75,
  VANGATURA = 76,
  ZAPPATURA = 77,
  RACCOLTA_MANUALE = 91,
  RACCOLTA_MECCANICA = 92,
  RILIEVO_PRODUZIONE_DATA_RACCOLTA = 125,
  STRIGLIATURA = 167,
  PIRODISERBO = 168,

  //Stesso LAV_COD di RILIEVO_ATTIVITA
  ALTRE_OPERAZIONI = 162,

  ABBATTIMENTOIMPIANTI = 170,

  LAVCOD_DEFOGLIAZIONE = 171,

  PASCOLAMENTO_PROPRIO = 174,
  PASCOLAMENTO_TERZI = 175,

  // ------------------------------------------------------------
  // GRUPPO OPERAZIONE = 5 Altre Operazioni Colturali(non gestite)
  // ------------------------------------------------------------
  // GRUPPO OPERAZIONE = 6 (tutte)
  BOLLA_EMESSA = 1031,
  BOLLA_RICEVUTA = 1025,
  DDT_CONTABILIZZATO_EMESSO = 1069,

  FATTURA_EMESSA = 1001,
  FATTURA_RICEVUTA = 1000,
  FATTURA_PROFESSIONISTI = 1070,

  FATTURA_PROFORMA = 1064,

  FATTURA_LIQ_CONF_EMESSA = 1055,
  FATTURA_LIQ_CONF_RICEVUTA = 1056,
  AUTOFATTURA_LIQ_CONF_EMESSA = 1057,
  AUTOFATTURA_LIQ_CONF_RICEVUTA = 1058,

  RICEVUTA_EMESSA = 1053,

  ALTRI_RICAVI = 1027,
  ALTRI_COSTI = 1026,

  // RESI/ABBUONI SU ACQUISTI
  NOTA_ACCREDITO_RICEVUTA = 1002,

  // RESI/ABBUONI SU VENDITE
  NOTA_ACCREDITO_EMESSA = 1003,

  MOV_FINANZIARIO = 1032,
  REG_COMPENSI = 1005,

  AUTOFATTURA_BENI_ESTERO = 1004,
  SPESE_PER_DIPENDENTI = 1006,
  SPESE_VARIE = 1007,
  ACQUISTO_MATERIE_PRIME_SOCI = 1060,

  PROCEDURA_LIQUIDAZIONE_SOCI = 1079,

  COSTI_CDG = 4500,

  // ------------------------------------------------------------
  // il 5000 è Trasformazioni e non è associato ad alcun gruppo
  TRASFORMAZIONI = 5000,
  CURA = 5004,
  // ------------------------------------------------------------
  // GRUPPO OPERAZIONE = 7 (tutte)
  ACQUISTO_BENI = 1008,
  CESSIONE_BENI = 1009,

  REG_AMMORTAMENTI = 1010,
  FINE_AMMORTAMENTI = 1011,

  // -------------------------------------------------------------
  // GRUPPO OPERAZIONE = 8 (tutte)
  PAGAMENTO_FATTURA = 1012,
  INCASSO_FATTURA = 1013,

  PAGAMENTI_DIVERSI = 1014,
  INCASSI_DIVERSI = 1015,

  PAGAMENTO_RATA_PRESTITO = 1024,

  // --------------------------------------------------------------
  // GRUPPO OPERAZIONE = 9 (tutte)
  SCADENZA_FATTURA_EMESSA = 1017,
  SCADENZA_FATTURA_RICEVUTA = 1016,

  SCADENZA_PAGAMENTI_DIVERSI = 1018,
  SCADENZA_INCASSI_DIVERSI = 1019,

  // --------------------------------------------------------------
  // GRUPPO OPERAZIONE = 10 (tutte)
  CARICO = 1022,
  SCARICO = 1023,

  PARTITA_DOPPIA = 1032,
  TRASFERIMENTO = 1033,

  AUTOCONSUMO = 1028,
  AUTOCONSUMO_VINO_SFUSO = 1066,
  PRODUZIONI = 1029,

  MOV_MAG_MATERIE_PRIME = 1030,

  CONFERIMENTO = 1050,
  ACCETTAZIONE = 1051,
  CONFERIMENTO_DIVERSI = 1052,
  ACCETTAZIONE_DIVERSI = 1054,

  VENDITA = 1020,
  ACQUISTO = 1021,
  CORRISPETTIVO_VENDITA_SFUSO = 1065,

  DOCO_EMESSO = 1061,
  DOCO_RICEVUTO = 1062,

  DAA_EMESSO = 1063,

  MVV_EMESSO = 1071,
  MVV_RICEVUTO = 1072,

  ALTRI_RICAVI_NERO = 1073,
  ALTRI_COSTI_NERO = 1074,
  DISTINTA_CARICO = 1075,
  DISTINTA_CARICO_ACCETTAZIONE = 1076,
  AUTO_DDT_EMESSO = 1077,
  AUTO_DDT_EMESSO_ACCETTAZIONE = 1078,
  GESTIONE_RIFIUTI = 1080,

  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 11 (tutte)
  NOTE = 2000,
  RATE = 2001,
  ORDINE_VENDITA = 2002,
  ORDINE_ACQUISTO = 2004,
  PREVENTIVO_VENDITA = 2003,

  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 12 (tutte)
  ANALISI_LATTE_SINGOLA = 3006,
  ANALISI_LATTE_MASSA = 3007,

  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 13 (tutte)
  NASCITA_ANIMALI = 3000,

  INCREMENTO_CONSISTENZE_ZOO = 3001,
  DECREMENTO_CONSISTENZE_ZOO = 3002,

  MORTE_ANIMALI = 3003,
  MACELLAZIONE_ANIMALI = 3004,

  SOSTITUZIONE_MARCA = 3005,

  SPOSTAMENTI_ZOO = 3030,
  PESATURA_ANIMALI = 3033,
  ACQUISTO_ANIMALI = 3034,
  VENDITA_ANIMALI = 3035,
  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 14 Rilievi produzioni
  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 15 Gestione mungitura
  MUNGITURA_PREPARAZIONE = 3009,
  MUNGITURA_SECCHIO_POSTA = 3010,
  MUNGITURA_GRUPPI_POSTA = 3011,
  MUNGITURA_SALA_LATTE = 3012,
  MUNGITURA_LAVAGGIO_IMPIANTI = 3013,
  MUNGITURA_LAVAGGIO_SALA_LATTE = 3014,

  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 16 Gestione lettiere
  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 17 Gestione alimentazione
  ALIMENTAZIONE_PULIZIA_IMPIANTI = 3019,
  ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI = 3020,
  ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI = 3021,
  ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI = 3022,
  ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI = 3023,
  ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024,

  CURE_MEDICAMENTI_ANIMALI = 3028,
  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 18 Altri lavori di stalla
  ALTRE_LAVORAZIONI_ZOO = 3036,
  TRASFERIMENTO_ANIMALI = 3037,

  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 19 MACCHINE
  MANUTENZIONE_MACCHINE = 1500,
  REVISIONE_MACCHINE = 4000,

  // ------------------------------------------------------------------
  PREPARAZIONE = 5000,

  // GRUPPO OPERAZIONE = 20 Gestione Visite Ispettive
  MONITORAGGIO_TEMPI_RIENTRO = 5001,
  VISITA_GENERICA = 5002,
  MONITORAGGIO_CE = 5003,

  PRATICA_ECOLOGICA = 5005,
  FORMAZIONE = 5006,
  VISITA = 5007
  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 100 Linee Produzione Vegetale
  // ------------------------------------------------------------------
  // GRUPPO OPERAZIONE = 101 Linee Produzione Animale
  // ------------------------------------------------------------
  // #####################################################################
}

export enum enum_Disciplinare_Tipo_Testata {

  Difesa = 0,
  Diserbo = 1,
  Fitoregolatore = 2,
  Fertilizzazione = 4

}

export enum enum_Gestione_Lotti {

  Nessuna = 0, // default
  Obbligatoria = 1,
  Facoltativa = 2
}

export enum enum_Gestione_Giacenze {

  SoloMovimentati = 0, // default
  SoloPresenti = 1,
  TuttiProdotti = 2
}

export enum enum_RACCOLTA_TIPO {
  Fast = 10,                           // data e impianti
  Leggera = 20,                        // data, impianti e qta prodotto
  Leggera_Con_Dettagli_Magazzino = 30, // data, impianti, qta prodotto e carico magazzino
  Standard = 40,
  Raccolta_e_Cura = 50
}

// Tipi di Mezzo
export enum enum_TipoMezzo {
  Indefinito = -1,      // Mezzo per le Materie Prime
  Ettolitro = 0,
  Ettaro = 1,
  Ora = 2,
  Mensile = 3,
  Complessivo = 4,
  Chilometro = 5,
  Quintale = 6
}

export enum enum_UnitaMisura {
  KG = 2,              // 1
  Grammi = 3,          // 0,001 kg
  Quintali = 4,        // 100 kg
  Milligrammi = 2032,
  Tonnellate = 304,

  Millilitri = 101,
  CentimetriCubi = 104,
  Litri = 29,
  Metri_Cubi = 19,         // 1000 litri
  Ettolitro = 2121,

  Grammi__HA = 20,
  KG__HA = 88,
  UNITA__HA = 89,
  METRI3__HA = 90,
  Tonnellate__HA = 2098,
  NumUnita__HA = 176,
  QUINTALI__HA = 2120,
  Tonnellate__HA_Spighe = 2112,

  Litro__HA = 22,
  Millilitri__Ha = 163,

  Millilitri__Quintale = 165,
  KG__Quintale = 169,
  Grammi__Quintale = 174,
  Litri__Quintale = 303,

  CC__HL = 21,
  Grammi__HL = 23,
  Milligrammi__HL = 126,

  Millilitri__HL = 164,
  Litri__HL = 173,
  Millilitri__Litro = 2016,
  Chilogrammi__HL = 175,
  Grammi__Litro = 2003,
  Milligrammi__Litro = 5001006,

  Num_Piante = 92,
  Unita_Seme = 93,
  Confezioni = 1003,

  Ettaro = 2123,

  Metri = 25,
  MetriQuadri = 28,
  Ore = 141,
  Giorni = 1002,

  Numero_Trappole = 11,
  Numero_Inneschi = 38,
  Numero_Diffusori_HA = 119,

  Numero = 38,

  Anno = 2019,
  StagioneColturale = 2024,
  CicloColturale = 2025,

  Montegradi = 5001040,

  Millimetri = 18,

  UNITA = 5001053,
  Numero_Diffusori = 5001052,
  Numero_Adulti_Individui_Trappola = 5,
  Presenza = 168
}

export const SIMBOLO_M3_HA = "m3/Ha";

export const SIMBOLO_MM = "mm";

export enum enum_SEMINA_TIPO {
  //Fast = 10,
  //Leggera = 20,
  //Leggera_Con_Dettagli_Magazzino = 30,
  Solo_Semina_Default = 30, //solo semina (con o senza magazzino)
  Semina_e_Modifica_Appezzamenti_Default = 40, //semina (con o senza magazzino) + modifica appezzamento
  Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Default = 50, //frazionamento appezzamento + semina sui singoli (con magazzino)
  Solo_Semina_Vincolo = 60, //solo semina (con o senza magazzino)
  Semina_e_Modifica_Appezzamenti_Vincolo = 70, //semina (con o senza magazzino) + modifica appezzamento
  Frazionamento_Appezzamenti_perLotto_e_Semina_sui_Singoli_Vincolo = 80 //frazionamento appezzamento + semina sui singoli (con magazzino)
}

export enum enum_AlgoritmoCostiAccessori {
  CAB = 1,
  SBTF = 2,
  BASE = 3
}

export enum enum_PUARegolamenti_Tipo {

  PianoComcimazione = 1,
  PUA = 2,
  Pan = 3,
  PianoNutrizionale = 4,
  PianoNutrizionale_IBF = 5
}

export enum enum_CodiciAnagrafe {

  Codice_Ausl = 4,

  Centro_Sede_Legale = 101,
  Centro_Sede_Aziendale = 102,
  Centro_Stabilimento = 103,

  TipoAttivita = 1000,
  DataFineControllo = 1001,
  CodiceCentro_Precedente = 1002,
  CodiceCentro_Attuale = 1003, //codice operatore bio
  PartitaIva_Fittizia = 1004,
  Associazione_CODEX = 1005,
  Conformita_Biologico = 1006,
  Conformita_FilieraControllata = 1007,

  OTE = 1009,
  CodiceCUAA = 1010,
  DataFineImpiegoPNC = 1014,
  OrientamentoProduttivo = 1015,
  TitoloPossesso = 1016,
  MetodoDiProduzione = 1018,
  pers_LegaleRappresentante = 1019,
  pers_Conduttore = 1020,
  pers_Delegato = 1021,
  pers_Detentore = 1022,

  ImpresaPAT = 1278,

  Codice_Socio = 1033,

  Impianto_LimiteN = 1050,
  Impianto_LimiteN_Organico = 1316,
  Impianto_LimiteP = 1051,
  Impianto_LimiteK = 1052,
  Impianto_LimiteMg = 1053,
  Impianto_Ibrido = 1054,
  Impianto_CodiceB_Maschio = 1055,
  Impianto_CodiceB_Femmina = 1056,
  Impianto_Genetica_Maschio = 1057,
  Impianto_Genetica_Femmina = 1058,
  Impianto_OffType_Maschio = 1059,
  Impianto_OffType_Femmina = 1060,
  Impianto_TraFila_Maschio = 1061,
  Impianto_TraFila_Femmina = 1062,
  Impianto_SuFila_Maschio = 1063,
  Impianto_SuFila_Femmina = 1064,
  Impianto_Interbina = 1065,
  Impianto_Germinabilita = 1066,
  Impianto_PianoSemina = 1071,
  Impianto_LottoFitosanitarioOmogeneo = 1072,
  Impianto_GroverCode = 1076,
  Impianto_Nr_domanda_ACA = 1359,

  Appezzamento_CodiceContratto = 1073,
  Impianto_Cooperativa = 1074,
  Appezzamento_ConfiniRischio = 1075,

  Organismo_Referente = 1074,

  GestioneDocumentiAllegati = 1077,

  CodiceRigaRiferimentoQuadroP = 1078,

  Coltura_Precedente_1 = 1079,
  Coltura_Precedente_2 = 1080,
  Coltura_Precedente_3 = 1081,
  Coltura_Precedente_4 = 1082,

  Impianto_Taglio_Tuberi_Patate = 1083,
  Impianto_Parti_Tuberi_Patate = 1084,

  Codice_Appezza_Biologico = 1085,

  Codice_Libro_Soci = 1086,
  Data_Iscrizione_Libro_Soci = 1087,
  Tecnico = 1088,
  Codice_Cliente = 1089,
  Codice_Fornitore = 1090,
  Codice_Fornitore_2 = 1091,
  Codice_Fornitore_3 = 1092,

  Organismo_di_Controllo = 1205,
  Mandato = 1206,

  Codice_Capitolato_Privato = 1093,

  Riferimento_Alfanumerico_Appezzamento = 1104,
  Latitudine = 1105,
  Longitudine = 1106,


  Magazzino_Conferimento = 1122,

  Codice_Unione_OP = 1125,

  Certificazione = 1130,

  Codice_GlobalGap = 1261,
  Codice_GlobalGap2 = 1314,

  Tribunale_di_registrazione = 1275,
  Regolamento_Aziendale_Default = 1294,
  Disciplinare_Aziendale_Default = 1295,

  Impianto_IAF_ImpegniAggiuntiviFacoltativi = 1296,

  Programmazione_Impianto_Pratica_Cod = 1298,

  INDICODE_EDI_Euritmo_Codice_punto_consegna_NAD = 4013,
  INDICODE_EDI_Euritmo_Codice_magazzino_emissione_ordine_NAB = 4014,
  INDICODE_EDI_Euritmo_Codice_fornitore_NAS_CodForn = 4015,
  INDICODE_EDI_Euritmo_Tipo_Codice_fornitore_NAS_QCodForn = 4016,

  GS1_CompanyPrefix = 1276,
  BNDOO_BancaDatiOperatoriOrtofrutticoli = 1277,

  GestioneContabile_DataInizio = 1284,
  GestioneContabile_ConsideraSaldiIniziali = 1285,

  Tipo_Gestione_Impresa = 1286,

  Zespri_Codice_kPIN = 1287,
  Zespri_Block_Name = 1288,
  Zepri_Grower_Number = 1317,
  Zepri_Identificativo_Flex2B = 1329,

  Default_Sincro_DDT_Terremerse = 1289,

  CampoSperimentaleBZ = 3000,
  SetAside = 3001,
  IntercalareAzione3 = 3002,
  Azione09_SiepieBoschetti = 3003,
  Azione10_Laghetti = 3004,
  AzioneG = 3005,
  ForestazioneL2080 = 3006,
  Agricampeggio = 3007,
  Lago = 3008,
  Alberatura = 3009,
  AzioneF3 = 3010,
  AzioneD1 = 3011,
  AzioneF1 = 3012,
  VivaioOrnamentali = 3013,
  Azione_Agroambientale = 3014,
  Tara_improduttiva = 3015,
  Prato = 3016,
  Affitto = 3017,
  Misura_2H = 3018,
  Orto = 3019,
  Biomassa = 3020,
  Colture_a_perdere_per_fauna_selvatica = 3021,
  Frutteto = 3022,
  Erbaio = 3023,
  Bosco = 3024,
  NessunaMappaturaConGias = 3078,

  Riferimento_Alfanumerico_Campo = 1279,
  Coltura_Appezzamento_Precedente_1 = 1280,
  Coltura_Appezzamento_Precedente_2 = 1281,
  Coltura_Appezzamento_Precedente_3 = 1282,
  Coltura_Appezzamento_Precedente_4 = 1283,

  Coltura_Campo_Precedente_1 = 1290,
  Coltura_Campo_Precedente_2 = 1291,
  Coltura_Campo_Precedente_3 = 1292,
  Coltura_Campo_Precedente_4 = 1293,

  Capitolato_Privato = 1093,

  Gruppo_Varietale_1 = 1094,
  Gruppo_Varietale_2 = 1095,
  Gruppo_Varietale_3 = 1096,
  Gruppo_Varietale_4 = 1097,
  Gruppo_Varietale_5 = 1098,
  Gruppo_Varietale_6 = 1099,
  Gruppo_Varietale_7 = 1100,
  Gruppo_Varietale_8 = 1101,
  Gruppo_Varietale_9 = 1102,
  Gruppo_Varietale_10 = 1103,

  Dettaglio_Specie_Personalizzato = 1108,

  Centro_Aziendale_Esterno_Collegato = 1337,

  PivaSuperUser_Origine_Dato = 1107,

  Codice_ICQ = 1109,
  Impianto_Codice_Programmazione = 1110,

  CodiceREA = 1112,
  CodiceISO = 1111,
  NumIscrAlboSocCoop = 1113,
  NumRegImprese = 1119,

  CodiceStabilimento = 1114, //sul fabbricato
  CodiceConferente = 1115,
  CodiceProduttore = 1116,
  CodiceCooperativa = 1117,

  Codice_Zona = 1118,

  Flag_Smart_Impianto = 1131,
  Flag_Smart_Appezzamento = 1132,

  Codice_Specie_Agea = 1133,
  Codice_Cultivar_Agea = 1134,

  Isola = 1320,

  codice_RUOP = 1321,

  Codice_Sito_Vivaio = 1323,

  Contratto_Produzione = 1324,
  Sup_Contratto = 1325,
  Filiera = 1326,

  Riferimento_Trasferimento_Dati = 1327,

  Data_Inizio_Portinnesto = 1328, //dal 05/21 la data d'inzio del portinnesto viene salvata sul nuovo campo Reg_Impianti.Data_Inizio_Portinnesto (anzichè in reg_impianti_codici)

  CodiceCliente_FRUTTAGEL = 2025,

  TracciaEsportazioneSIGPA_Fabbricati_XFertilizzanti = 5007,
  TracciaEsportazioneSIGPA_Fabbricati_XFormulati = 5017,

  TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFertilizzanti = 5008,
  TracciaEsportazioneSIGPA_ChiusuraGiacenze_XFormulati = 5018,

  CodiceCliente_GranfruttaZani = 2029,
  CodiceCliente_Apofruit = 2013,
  CodiceCliente_Orogel = 2039,
  CodiceCliente_Apot = 2042,

  CodiceCliente_Tecnoterr = 2063,
  CodiceCliente_Casalasco = 2064,

  acciseDAA_CodiceAccisa_Mittente = 4003,
  acciseDAA_CodiceAccisa_UfficioDoganale = 4005,
  acciseDAA_CodiceAccisa_Destinatario = 4006,
  acciseDAA_CodiceAccisa_codiceUA = 4008,
  acciseDAA_Accise_Conto_Garanzia = 4009,
  acciseDAA_Codice_Accise_Prefisso = 4010,
  acciseDAA_VIDIMA_RegistroElettronica = 4011,

  Contatto_OrganismoDiControllo_BIO = 4018,

  //CBI corporate interbancario
  CBI_CodiceSIA = 1264,
  Codice_Centro = 1265,

  //spesometro
  CodiceAteco2007 = 1266,
  codiceFiscaleProduttoreSoftware = 1267,

  Spesometro_intermediario_CodFisc = 1268,
  Spesometro_intermediario_AlboCAF = 1269,

  SISPAC_Codice_Anagrafico = 4007,
  //----CODICI APOFRUIT SIAGR
  //inizio 1135

  //impresa
  //V01CON00_VCCOD = 1213 //Codice Conferente
  //V01CON00_VCCOD = Codice_Socio //Codice Socio (Conferente) uso quello presente in gias, visualizzato SOCIO
  V01CON00_VCDNA = 1135, //Data di nascita
  V01CON00_VCLNA = 1136, //Località di nascita
  V01CON00_VCZON = 1137, //Zona
  V01CON00_VCSUA = 1138, //Superficie agricola
  V01CON00_VCQTE = 1139, // Codice socio Precedente ex Qta tassa altre coop
  V01CON00_VCGRU = 1140, //Gruppo trasportatori
  V01CON00_VCTRA = 1141, //Codice trasportatore
  V01CON00_VCNIS = 1142, //Numero inscrizione
  V01CON00_VCDIS = 1143, //Data iscrizione
  V01CON00_VCAIC = 1144, //Anno inizio conferimento
  V01CON00_VCIFA = 1145, //Flag Fattura/Autofattura
  V01CON00_VCLIS = 1146, //Codice Listino
  V01CON00_VCMEZ = 1147, //Flag Mezzadria
  V01CON00_VCVAL = 1240, //% meszzadro
  V01CON00_VCPRP = 1148, //Codice proprietario
  V01CON00_VCAT1 = 1149, //Attributo 1
  V01CON00_VCAT2 = 1150, //Attributo 2
  V01CON00_VCAT3 = 1151, //Attributo 3
  V01CON00_VCAT4 = 1152, //Attributo 4
  V01CON00_VCAT5 = 1153, //Attributo 5
  V01CON00_VCAT6 = 1154, //Attributo 6
  V01CON00_VCAT7 = 1155, //Attributo 7
  V01CON00_VCAT8 = 1156, //Attributo 8
  V01CON00_VCAT9 = 1157, //Attributo 9
  V01CON00_VCAT0 = 1158, //Attributo 10
  V01CON00_VCDUM = 1175, //Data ultima variazione
  V01CON00_VCANN = 1160, //Flag annullamento
  V01CON00_VTPCON = 1161, //Tipo conferimento
  V01CON00_VCCOIS = 1162, //Centro di conferimento
  Stabilimento = 1163, //Stabilimento di conferimento (FARLO PUBBLICO)
  V01CON00_VANAN = 1164, //Anni anzianità
  V01CON00_VANUL = 1165, //Anno ultima liquidazione
  V01CON00_VANULI = 1166, //Anno ultimo invecchiamento
  V01CON00_VLIATR = 1167, //Linea addebito trasporto
  V01CON00_VLIRTR = 1168, //Linea rimborso trasporto
  V01CON00_VCFOR = 1169, //Codice Gruppo

  //impresadettagli
  CATCON0F_CCRAP = 1170, //Codice Rappresentante
  CATCON0F_CCEST = 1171, //Superficie totale
  CATCON0F_CCRIF = 1172, //Rif. nucleo familiare
  CATCON0F_CCSES = 1173, //Sesso
  CATCON0F_CCDTN = 1174, //Data notifica
  //CATCON0F_CCDTV = 1175, //Data variazione/Taratura Atomiz. non lo sal,co, lo uso per la prima importaz e basta

  //centro
  CATANA0F_CAFON = 1262, //Numero Fondo
  CATANA0F_CAZON = 1176, //zona/frazione
  CATANA0F_CAZOP = 1177, //produttivita zona/frazione
  CATANA0F_CACON = 1241, //Note/Confinanti
  CATANA0F_CARES = 1242, //Responsabile Centro
  CATANA0F_CAFPR = 1243, //Flag Proprietario
  CATANA0F_CATCO = 1178, //Tipo Conduzione
  CATANA0F_CASUT = 1244, //Sup. Totale
  CATANA0F_CASUP = 1245, //Sup. Pianura
  CATANA0F_CASUC = 1246, //Sup. Collina
  CATANA0F_CATAR = 1247, //Sup. Tara
  CATANA0F_CASUI = 1248, //Sup. Incolta
  CATANA0F_CASUF = 1249, //Sup. Frutteto
  CATANA0F_CASUO = 1250, //Sup. Orticola
  CATANA0F_CASUV = 1251, //Sup. Vigneto
  CATANA0F_CASUS = 1252, //Sup. Seminativo
  CATANA0F_CASUA = 1253, //Sup. Altri
  CATANA0F_CAALT = 1254, //Altitudine
  CATANA0F_CAFLI = 1255, //Flag Irriguo
  CATANA0F_CAFLH = 1256, //Flag Acqua
  CATANA0F_CAFLR = 1257, //Reperibilità acqua
  CATANA0F_CAREA = 1258, //Reddito agrario
  CATANA0F_CARED = 1259, //Reddito Dominicale
  CATANA0F_CAPCA = 1260, //Perc. Abbattimento

  CATANA0F_CAA01 = 1179, //tecnico di riferimento zona
  CATANA0F_CAA02 = 1180, //azienda eurepgap
  CATANA0F_CAA03 = 1181, //sup totale per scaglioni
  CATANA0F_CAA04 = 1182, //sup ortofrutta per scaglioni
  CATANA0F_CAA05 = 1183, //tecnico di riferimento RER
  CATANA0F_CAA06 = 1184, //Prospettiva Aziendale
  CATANA0F_CAA07 = 1185, //Aumento-Diminuzione prodotti
  CATANA0F_CAA08 = 1186, //Fascicolo Aziendale

  //impianto
  CATIMP0F_CINPR = 1214, //Numero Progressivo Impianto
  CATIMP0F_CIFPC = 1187, //flag pianura collina
  CATIMP0F_CIFIR = 1188, //flag irriguo
  CATIMP0F_CISER = 1189, //flag serra
  //CATIMP0F_CILIN = 1190, //codice lotta intagrata, (uso capitolato)
  CATIMP0F_CIRAC = 1191, //raccolta manuale/meccanica
  //CATIMP0F_CIPIN = 1192, //Portinnesto
  //CATIMP0F_CIALL = 1193, //Allevamento
  CATIMP0F_CIIRR = 1194, //Irrigazione
  //CATIMP0F_CITPR = 1195, // tpo produzione-regolamento bio
  //CATIMP0F_CINAP = 1195, //numero appezzamento per consociazione (utilizzato enum_CodiciAnagrafe.Codice_Appezza_Biologico)
  //CATIMP0F_CIQTP = 1196, //raccolta ottimale
  //CATIMP0F_CIQTC = 1197, //raccolta corretta
  CATIMP0F_CIC02 = 1198, //Fumigazione
  CATIMP0F_CIC04 = 1199, //ocm
  //CATIMP0F_CITCO = 1200, //Tipo copertura
  CATIMP0F_CIDAL = 1201, //Data ammissione LI
  CATIMP0F_CIDEL = 1202, //Data esclusione LI
  CATIMP0F_CITIM = 1203, //Impianto consociato - successione
  CATIMP0F_CISEQ = 1204, //Sequenza successione - consociazione
  CATIMP0F_CISPE_CIVAR = 1205, //salvo la specie e varieta (loro codice)
  CATIMP0F_VarietaContratto = 1206, //Segnalo se varieta a contratto 0/1 campo VVAT4

  //impresa note codificate
  // V01NOC00_NCPRO_9000 = 1207, //codice ente certificazione bio
  V01NOC00_NCPRO_9001 = 1208, //Numero etichetta
  //V01NOC00_NCPRO_9002 = 1209, //Cellulare del socio
  //V01NOC00_NCPRO_9003 = 1210, //codice operatore bio
  V01NOC00_NCPRO_9004 = 1211, //passaggio al codice
  V01NOC00_NCPRO_9005 = 1212, //Coordinate gps
  V01NOC00_NCPRO_9006 = 1261, //CGN
  //impresa note generiche
  V01NOC00_NCPRO_1 = 1215,
  V01NOC00_NCPRO_2 = 1216,
  V01NOC00_NCPRO_3 = 1217,
  V01NOC00_NCPRO_4 = 1218,
  V01NOC00_NCPRO_5 = 1219,
  V01NOC00_NCPRO_6 = 1220,
  V01NOC00_NCPRO_7 = 1221,
  V01NOC00_NCPRO_8 = 1222,
  V01NOC00_NCPRO_9 = 1223,
  V01NOC00_NCPRO_10 = 1224,
  V01NOC00_NCPRO_11 = 1225,
  V01NOC00_NCPRO_12 = 1226,
  V01NOC00_NCPRO_13 = 1227,
  V01NOC00_NCPRO_14 = 1218,
  V01NOC00_NCPRO_15 = 1229,
  V01NOC00_NCPRO_16 = 1230,
  V01NOC00_NCPRO_17 = 1231,
  V01NOC00_NCPRO_18 = 1232,
  V01NOC00_NCPRO_19 = 1233,
  V01NOC00_NCPRO_20 = 1234,
  V01NOC00_NCPRO_21 = 1235,
  V01NOC00_NCPRO_22 = 1236,
  V01NOC00_NCPRO_23 = 1237,
  V01NOC00_NCPRO_24 = 1238,
  V01NOC00_NCPRO_25 = 1239,

  //----FINE CODICI APOFRUIT SIAGRultimo 1262

  ORGANISMO_DI_CONTROLLO_BIO = 1263, //organismo controllo salvato sui codici del centro, collegato a tabella BIO_Dati_OrganismiControllo

  Fabbricato_Forno_Combustibile = 1270,
  Fabbricato_Forno_Fiamma = 1271,
  Fabbricato_Forno_Cantiere = 1272,
  Fabbricato_Forno_Umidificazione = 1273,
  Fabbricato_Forno_Tipo = 1274,

  OrganismoRefIntestatarioQDC = 1299,
  Codice_Impianto = 1300,
  Distinta_Chiusa = 1301,
  Codice_Impianto_Ribaltato = 1311,
  Algoritmo_Codifica = 1312,

  ScontoContattoDefault = 4000,
  ListinoPrezziAcquistoDefault = 4001,
  ListinoPrezziVenditaDefault = 4002,
  CodiceAccisa = 4003,
  ModalitaPagamentoDefault = 4004,
  IBANDefault = 4012,
  NumeroIscrizioneAlboAutotrasportatori = 4017,
  PEC = 4019,
  SDI = 4020,

  // --- Per Fattura Elettronica
  CapitaleSociale = 1123,

  // <see cref="enumTipoSocieta"/>
  TipoSocieta = 1302,      //Spa, Sapa, Srl, Altro
  UfficioRea = 1304,
  NumeroRea = 1305,

  // <see cref="enumNumeroSoci"/>
  NumeroSoci = 1306,

  // <see cref="enumStatoLiquidazione"/>
  StatoLiquidazione = 1307,
  DataAttivazioneEFattura = 1309,
  DataUltimaRicezioneEFattura = 1310,

  // <see cref="enumTipoContattoFattura"/>
  TipoContattoFattura = 1303,
  PecContatto = 4019,
  CodiceSDI = 4020,
  RappresentanteFiscale = 4021,

  Visibile_da_App = 1308,

  UfficioICQRF = 1313,

  CodiceParticella = 1318,

  Finalita_Concimazione_Impianto = 1319,

  Centro_CodiceStabilimento = 1322,

  Zespri_Fasi_Fase = 1330,

  Zespri_Fasi_Tipo = 1331,

  Zespri_Fasi_Grower = 1332,

  Num_Piante_Femmine = 1333,

  Num_Piante_Maschi = 1334,

  Impresa_Pubblica = 1335,

  TipologiaDIInnestoTrapianto = 1336

}

export enum enum_Tipo_Salvataggio_QdC {
  Salva_ed_Esci = 1,
  Salva_e_Nuovo = 2,
  Salva_e_Duplica = 3,
  Salva_e_CDG = 4,
  Salva_Ricetta_e_Nuovo_Dettaglio = 5,
  Salva_Visita_Aggiungi_Operazione = 6
}

export enum enum_doseQuantitaTotale {
  Qta_Totale = 10,
  Dose = 11
}

export enum enum_TipoFormulato {
  Tutti = 0,
  Antiparassitari = 1,
  Diserbanti = 2,
  Fitoregolatori = 3,
  Coadiuvanti = 4,
  Concianti = 5,
  Disseccanti = 6,
  Geodisinfestanti = 7,
  Antiparassitari_Concianti = 8,
  Diserbanti_Disseccanti = 9,
  Antiparassitari_Geodisinfestanti = 10,
  Corroboranti_Fisiofarmaci = 11,
  PostRaccolta = 12,
  Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci = 13,
  ConfusioneSessuale = 14,
  DisorientamentoSessuale = 15
}

export enum enum_Servizi {
  QuadernoCampagnaStd = 1,
  QuadernoCampagnaGlobal = 2,
  RegistroTrattamenti = 3,
  RegistroFertilizzazioni = 4,
  RegistriCaricoScaricoMagazzino = 5,
  QuadernoCampagnaBio = 6,
  RegistroVenditeBio = 7,
  RegistroMateriePrimeBio = 8,
  PAPVegetale = 9,
  PAPZootecnico = 10,
  PAPPreparazioni = 11,
  NotificaBio = 12,
  PUA = 13,
  PianoConcimazione = 14,
  CheckListCondizionalita = 15,
  CheckListSicurezzaLavoro = 16,
  CheckListGlobal = 17,
  Teleregistri = 18,
  Sistema_QualitÃ_Nazionale_Produzione_Integrata = 19,
  Esportazione_in_formato_Zespri = 20,
  Esecuzione_ed_avanzamento_delle_ricette = 21,

  DocContabili_Ordine_Acquisto = 100,
  DocContabili_Bolla_Ricevuta = 101,
  DocContabili_Bolla_Emessa = 102,
  DocContabili_Accettazione = 103,

  Profitosan = 1001,
  QStandard = 1002,
  QPlus = 1003,
  QBio = 1004,
  QFert = 1005,
  QMaps = 1006,
  Condizionalita2018 = 1007,
  Profitosan_serverSide = 1008,
  QDemetra = 1017,
  Workflow_di_attivazione_aziende_GIAS = 1050,
  Registro_Trattamenti = 2001,
  Registro_Trattamenti_Bio = 2002,
  Quaderno_Campagna_Azienda = 2003,
  Quaderno_Campagna_Caa = 2004,
  Registro_Fertilizzazioni_PUA = 2005,
  Quaderno_Campagna_CBPA = 2006,
  Gestione_UMA = 2007,
  Gestione_Azienda = 3000,
  Azienda_NO_SQNPI_PUA = 2008
}

export enum enum_Cod_Regolamento {

  Non_Specificato = 0,
  Regolamento_Nessuno = 1,
  Regolamento_bio = 4,
  Regolamento_ProduzioneIntegrata = 10

}

export enum enum_TipoNodo {
  Utente = 1,
  Impresa = 2,
  Centro = 3,
  Campo = 4,
  Serra = 41,
  Appezzamento = 5,
  ImpiantoNudo = 6,
  ImpiantoArborea = 7,
  ImpiantoErbacea = 8,
  ImpiantoOrticola = 9,
  Impianto_Generico = 19,
  Particella = 10,
  Persona = 11,
  CatastoAziendale = 18,
  Fabbricato_Generico = 20,
  f_Abitazione = 21,
  f_Magazzino = 22,
  f_Silos = 23,
  f_CellaFrigorifera = 24,
  f_ImpiantoLavorazione = 25,
  f_Stalla = 26,
  f_Fienile = 33,
  f_Essiccatoio = 10034,
  p_PortafoglioProdotti = 27,
  p_Prodotto = 28,
  p_Preparazione = 29,
  x_ConsistenzeAnimali = 30,
  x_MovimentiMagazzino = 31,
  x_PreparazioniAlimentari = 32,
  x_GiacenzeMagazzino = 34,
  x_ParcoMacchine = 35,
  x_Contatti = 36,
  x_ListaFabbricatiAziendali = 37,
  x_Cooperativa = 38,
  x_Consorzio = 39,
  x_OP = 40,
  Analisi_Certificato = 42,
  Analisi_Testata = 43,
  Analisi_Dettaglio = 44,
  Analisi_Campione = 45,
  PianoConcimazione_Testata = 46,
  DistintaDiProduzione = 47,
  x_VariazioniConsistenzeAnimali = 48,
  Cantine_Piani = 49,
  Cantine_Vasche = 50,
  PlanningTestata = 51,
  PlanningEntita = 52,
  PlanningEntitaImpianto = 53,
  Agenda = 54,
  ricette_Testata = 60,
  ricette_dettaglio = 61,
  Anagrafica_Generica = 62,
  AgendaDestinazioni = 63,
  Precision = 64
}

export enum enum_PosizioniNodo {
  TipoNodo = 0,
  Piva = 1,
  Sa_Cod = 2,
  Campo_Cod = 3,
  Appezza = 4,
  Id_Imp = 5,
  p_Part_Cod = 6,
  p_Provincia_Cod = 7,
  p_Comune_Cod = 8,
  p_Sezione = 9,
  p_Foglio = 10,
  p_Numero = 11,
  p_Subalterno = 12,
  Cod_Fiscale = 13,
  Fabbricato_Cod = 14,
  Prodotto_Cod = 15,
  Data_Lavorazione = 16,
  Analisi_Certificato_Cod = 17,
  Analisi_Testata_Cod = 18,
  Analisi_Dettaglio_Cod = 19,
  Analisi_Campione_Cod = 20,
  PianoConcimazione_Testata_Cod = 21,
  Progetto_Cod = 22,
  Programmazione_Cod = 23,
  Programmazione_Entita_Cod = 24,
  id_agenda = 25,
  PivaPadre = 26
}

export enum Tipo_Polverulento {
  NonPolverulento = 0,
  Polverulento = 1
}

export enum enum_CodificaStampe {
  Nessuna = 0,
  SchedaCampagna_2078 = 1,
  RegistroTrattamenti = 2,
  SchedaRegistrazione = 3,
  SchedaCampagna_Biologico = 4,
  Bolle = 5,
  Fatture = 6,
  Quadro_P = 7,
  PianoRaccolta = 8,
  SchedaMagazzinoMovimenti = 9,
  SchedaMagazzinoGiacenze = 10,
  SchedaMagazzinoFertilizzanti = 11,
  SchedaMagazzinoProdottiFitosanitari = 12,
  SchedaCampagna_2078_Semplificata = 13,
  RegistroTrattamenti_Semplificata = 14,
  SchedaRegistrazione_Semplificata = 15,
  SchedaCampagna_Biologico_Semplificata = 16,
  ReportRisultatoFilrone = 17,
  RiepilogoImpiegoSuperfici = 18,
  Eurep_Gap = 19,
  SchedaColturale_Biologico = 20,
  SchedaMateriePrime_Biologico = 21,
  SchedaVendite_Biologico = 22,
  Esporta_GiasToSap = 23,
  PAP_Vegetale = 24,
  SchedaTracciabilita = 25,
  ReportConserveItalia = 26,
  SchedaCampagna_ConserveItalia = 27,
  RapportinoStrube = 28,
  Eurep_Gap_Semplificata = 29,
  DatiAnelloFilieraIngresso = 30,
  DatiAnelloFilieraLegameLotti = 31,
  GestioneAllegati = 32,
  AnalisiCosti_XLS = 33,
  AnalisiRicavi_XLS = 34,
  MarginiEconomici_XLS = 36,
  SchedaTracciabilita_ByLotto = 35,
  EstrattoreDatiGrafici = 37,
  LibroConferimenti = 38,
  PianoColturale = 39,
  SchedaCampagna_Pizzoli = 40,
  Esportazione_OP_Inv = 41,
  Esportatore_Universale_Impianti = 42,
  Esportatore_Universale_Imprese = 43,
  Esportatore_Universale_Centri = 44,
  Esportatore_Universale_Appezza = 45,
  Esportatore_Universale_Agenda = 46,
  AnalisiTerreno = 47,
  Report_RiconversioneVarietale = 48,
  Esportatore_Universale_Rintraccio = 49,
  Verifica_Conformita = 50,
  Report_ImpegnoProduzioneSoci = 51,
  Esportazione_OP_Gest = 52,
  ImportaAnagrafiche_XLS2GIAS = 53,
  GestioneRicette_Lista = 54,
  Registro_FattureAcquisto = 55,
  Registro_FattureVendita = 56,
  Registro_Corrispettivi = 57,
  Registro_AltriCosti = 58,
  Registro_AltriRicavi = 59,
  Registro_PrimaNota = 60,
  Lista_InsolutiClienti = 61,
  Lista_InsolutiFornitori = 62,
  LiquidazionePeriodica_IVA = 63,
  Bilancio_Civilistico = 64,
  PianoDeiConti = 65,
  Report_NPK_Totali = 66,
  Bolle_Conferimento_Soci = 67,
  Bolle_Conferimento_Diversi = 68,
  PAP_Zootecnico = 69,
  Costo_Manodopera_XLS = 70,
  Costo_ParcoMacchine_XLS = 71,
  Analisi_Vino = 72,
  RicevuteFiscali = 73,
  Atto_Notorio = 74,
  Adesione_Etico_Ambientale = 75,
  Tenuta_Scheda_Campagna = 76,
  Adesione_DPI = 77,
  Impegnativa_Eurep = 78,
  Impegnativa_QC = 79,
  Impegnativa_Confusione_Sessuale = 80,
  Codice_Condotta = 81,
  Modello4 = 82,
  Registro_Stalla = 83,
  ImportaAnagraficheAnimali_XLS2GIAS = 84,
  VasiVinari = 85,
  GestioneCondizionalita = 86,
  Esportatore_Codifiche_Cultivar = 87,
  Esportatore_Codifiche_SpecieVegetali = 88,
  Produzioni_XLS = 89,
  FiltroStrube = 90,
  Esportazione_AnagraficaProdotti = 91,
  PacchettoIgiene_RegistroFornitori = 92,
  PacchettoIgiene_RegistroClienti = 93,
  PacchettoIgiene_SchedaUsoAlimentiOGM = 94,
  PacchettoIgiene_RegistroAlimentazioneStalla = 95,
  PacchettoIgiene_RegistroRazionamento = 96,
  PacchettoIgiene_RegistroAnalisiNonConformi = 97,

  // BUCO NEL 98

  Esportazione_AnagraficaContatti = 99,
  Esportazione_CellulariContatti = 100,
  Analisi_Vino_Derivanti_Da_Travasi = 101,
  Bilanci_DiVerifica_Confronto = 102,
  Nota_Accredito = 103,
  Mastrino = 104,
  Esportazione_OP_Gest_Coop = 105,
  Registri_Preparazioni = 106,
  EtichetteVascheEnologiche = 107,

  // 'BUCO X NICOLETTA 108

  Bilancio_Fertilizzazioni = 109,
  Programmazione_Vegetale = 110,
  Esporta_GiasToXMLPubblico = 111,
  Esporta_Conferimenti_JDEdwards = 112,
  Buono_Accettazione_Diversi = 113, // stampa bolla del giaslan
  Buono_Accettazione = 114,
  Registro_Fertilizzazioni = 115,
  SchedaTracciabilita_Animale = 116,
  RaccoltaDatiAnagrafici = 117,
  Importa_RaccolteConferimenti_DaRintraccio = 118,
  ADD_Filtro_Report_Accettazione_DaDiversi = 119,
  ADD_Riepilogo_Conf_XSpecie = 120,
  ADD_EC_Bolle_Accettazione_DaDiversi = 121,
  ADD_EC_Imballi = 122,
  ADD_Saldo_Imballi = 123,
  ADD_Export_Bolle_Accettazione_DaDiversi = 124,
  ADD_Export_Traportatori = 125,
  Importazione_DDT_PianoColturale = 126,
  Mandato_Trasmissione_Telematica_Dati = 127,
  DOCO = 128,
  DAA = 129,
  Impegnativa_Orticole_Gest_Annuale = 130,
  Impegnativa_Orticole_Gest_Breve = 131,
  Impegnativa_Orticole_Industria = 132,
  Impegnativa_Pomodoro_Industria = 133,
  Impegnativa_Fagiolino_Mercato_Fresco = 134,
  ImportaContatti_XLS2GIAS = 135,
  Certificato_Pomodoro = 136,
  Certificato_Pomodoro_Interno = 137,
  Certificato_Pomodoro_Esterno = 138,
  Registro_CaricoScarico_Pomodoro = 139,
  Esporta_ConferimentiPomodoro_Agrea = 140,
  Esporta_BolleAccettazione2AltroClienteGias = 141,
  ExportExcel_MonitoraggioCE = 142,
  ADD_ExportExcel_CertificatiPomodoro = 143,
  SchedaCampagna_Ricette = 144,
  GestioneRicette_Edit = 145,
  ExportExcel_MonitoraggioCE_Aggregata = 146,
  GestioneEtichette_Trasformati = 147,
  LibroConferimenti_XLS = 148,
  RegistroTrattamenti_Veneto = 149,
  SchedaCampagnaMultiCentro = 150,
  RiepilogoImpiegoSuperfici_Multiazienda = 151,
  Eurep_Gap_Multicentro = 152,
  SchedaPreparati_Biologico = 153,
  Notifica_Biologico = 154,
  RiBa_Report_Presentazione = 155,
  RiBa_Export_CBI = 156,
  Listini_XLS = 157,
  DDT_Contabilizzato_Emesso = 158,
  Ordine = 159,
  AgentiProvvigioni_XLS = 160,
  Eurep_Gap_Multicentro_Immediata = 161,
  Report_Incongruenze_CatastoVSAgrea = 162,
  Preventivo_Vendita = 163,
  Scheda_Rilievi = 164,
  PianoColturaleCatasto = 166,
  GiornaleContabile = 167,
  Ordine_Acquisto = 168,
  FF_Etichette = 169,
  Liquidazione_Soci = 170,
  Allegato_CatastoeValorizzazioni = 171,
  Esportazione_OP_Produttori = 172,
  Esportazione_OP_Catasto = 173,
  ADD_ExcelTracciabilitaConferimenti = 174,
  FreshFood_BollaAccettazione = 175,
  FreshFood_DistintaCarico_Accettazione = 176,
  FreshFood_AutoDDT_Accettazione = 177,
  SchedaMagazzinoMovimentiExcel = 178,

  // ' OLD Buono_Accettazione_Diversi_DDTRicevuto = 165
  ConferimentoUva_DDTRicevuto = 165,
  ConferimentoUva_DistintaCarico = 179,
  ConferimentoUva_AutoDDT = 180,
  Registro_Fertilizzazioni_Massivo = 181,
  Registro_Trattamenti_Massivo = 182,
  Conf_FiltroStampe = 183,
  Conf_EC_Imballi = 184,
  Conf_Saldo_Imballi = 185,
  Conf_Riepilogo_Conferimenti = 186,
  Conf_Tracciabilita = 187,
  Bilancio_SezioniContrapposte = 188,
  EstrattoConto_Contatti = 189,
  Cespiti_FiltroStampe = 190,
  SchedaCampagna_ProvAut_Trento = 191,
  SchedaCampagna_Multi_Lombardia = 192,
  FreshFood_BollaCampionatura = 193,
  RisultatoAnalisiConformita = 194,
  FreshFood_FatturaLiquidazioneSoci = 195,
  FreshFood_PagatiSuConferito = 196,
  FreshFood_AutofatturaLiquidazioneSoci = 197,
  FreshFood_RiepilogoLiquidazioneSoci = 198,
  FreshFood_PagatiSuCampionato = 199,
  Analisi_Progetti = 200,
  SchedaInterventiAgronomici = 201,
  EsportazioneAgeaTxtCSV = 202,
  RegistroAziendaleUnico = 203,
  SchedaCatastoeUtilizzi = 204,
  RegistroTrattamentiVeneto_StdCondizionalita = 205,
  RiepiloghiAccise = 206,
  DAA_GaranzieCircolanti = 207,
  DAA_PariteSospensione = 208,
  MVV = 209,
  Report_Vendita_PDF = 210,
  ReportRisultatoFiltroneG2G = 211,
  EsportazionePomodoroIndustriaOINordItalia = 212,
  RiepilogoProdottiUtilizzati = 213,
  PassaportoMaterialeVivaistico = 214,
  ReportRisultatoFiltroneIncludiVisita = 215,
  Bilancio_Fertilizzazioni_Dettagliato = 216,
  Conf_Esportazione_BolleFF_XLS = 217,
  Filtro_StampeBiologico = 218,
  Conf_EsportazionexTrasportatori_XLS = 219,
  Conf_RiepilogoxArticolo = 220,
  Conf_EC_Bolle = 221,
  Conf_Certificato_Pomodoro = 222,
  Conf_Esportazione_CertificatiPomodoro_XLS = 223,
  UMA_RichiestaCarbPrevisioneLav = 224,
  UMA_VerbaleIstruttoriaRichCarb = 225,
  UMA_RendicontazioneCarb = 226,
  UMA_IstruttoriaRendCarb = 227,
  EstrazioneCatastoAffitti = 228,

  Conf_ComunicazioneCredito = 229,

  CredenzialiPrivacy = 230,

  Report_OrdiniVivaio = 231,
  Report_PianoColturalePreventivoVivaio = 232,

  Impegnative_capitolati = 233,
  Adesione_ModuloGrasp = 234,
  Adesione_ProtocolloGlobalGAP = 235,
  Adesione_NurtureModule = 236,
  Adesione_Despar = 237,
  Adesione_Conad = 238,
  Accordo_Responsabilita_di_Filiera = 239,
  Dichiarazione_di_Responsabilita = 240,
  Fitoregolatori_Kiwi = 241,
  Adesione_StandardLeaf = 242,
  SchedaAziendale = 243,
  ImpegnoProduzioneSociDivisoxCentri = 244,
  ObiettivoDiProduzioneAsipo = 245,
  ImpegnativaColtivazioneConferimento = 246,
  QuestionarioValutazioneAzienda_Aggiornamento = 247,
  PianoColturaleCatastoGrid = 248,
  Statistometro = 249,
  AbilitazioniXSpecie = 258,

  GiasAPP_ConsultaSincroDatiAppDaWeb = 822
}

export enum enum_TipoFiltrone {
  /** Permette l'accesso al filtrone se l'utente possiede il permesso Gest_Stampe in modalità lettura. */
  Stampa = 1,
  Utenti = 2,
  Agenda = 3,
  AnalisiCosti = 4,
  ModificaImpianti = 5,
  PianificazioneInterventi = 6,
  EliminaInterventi = 7,
  Esportazione_OP_Inv = 8,
  Esportazione_OP_Gest = 17,
  Esportatore_Universale_Impianti = 9,
  Esportatore_Universale_Imprese = 10,
  Esportatore_Universale_Centri = 11,
  Esportatore_Universale_Appezza = 12,
  Esportatore_Universale_Agenda = 13,
  Esportatore_Universale_Rintraccio = 16,
  Blocco_OperazioniAgenda = 14,
  Richiesta_Verifica_Conformita = 15,
  Associa_Ricetta_Impianti = 18,
  Esportazione_CellulariTecnici = 19,
  Esportazione_OP_Gest_Coop = 20,
  Bilancio_Fertilizzazioni = 21,
  MultiModificaImpianti = 22,
  Esportatore_Contatti = 23,
  Associa_Ricetta_Interventi = 24,
  MultiModificaAgenda = 25,
  FiltroStrube = 26,
  PianiCampionamento_AggiungiImpianti = 27,
  CancellaAppezzamenti = 28,
  Gestione_Servizi = 29,
  PianoConcimazione = 30,
  ExportSigpa = 31,
  ModificaAperturaCampi = 32,
  OperazioniMultiAziendali = 33,
  Esportazione_AgeaTXTCSV = 34,
  EsportazionePomodoroIndustriaOINordItalia = 35,
  StimeProduzione = 36,
  Modifica_Multipla_PianoColturale = 37,
  DuplicaOperazione = 38
}

export enum enum_FiltroneParams {
  Sito_Origine = 's_o',
  Pagina_Origine = 'p_o',
  Pagina_Destinazione = 'p_d',
  Sito_Destinazione = 's_d',
  /** @usageNotes Da inserire tra i parametri una volta sola.
   * In caso contrario la pagina del FIltrone potrebbe parsare male il valore del parametro. */
  Tipo_Filtrone = 't_f',
  Codifica_Stampe = 'c_s',
  /** Header/Footer (se diverso da "" vengono nascosti) */
  Mostra_Header_Footer = 'hdr_ftr',
  Categoria = 'cat',
  /** 1 = blocca, 0 = lascia editable (default) */
  BloccaCategoria = 'cat_blk',
  Piva = 'piva',
  Includi_Visite = 'IncludiVisite',
  No_Piva = 'nopiva',
  Gias2Gias = 'g2g',
  Filtrino = 'f',
  Veg_Cod = 'v_c',
  Cul_Cod = 'c_c',
  Data_Inizio = 'd_i',
  Data_Fine = 'd_f',
  Dati_Di_Ritorno = 'ddr',
  /** Forza il redirect al filtrone se diverso da "" (ignora i check sui permessi).
   * Attualmente usato solo in combinazione con `enum_TipoFiltrone.Esportatore_Universale_Imprese`
   * per permettere l'assegnazione della visibilità dalla profilazione nuova senza
   * aver bisogno del permesso `Stampe_Esportatore_Universale`. */
  Forza_Redirect = 'f_r',
}

export enum enum_TipoOperatoreVisita {
  Capo = 0,
  Tecnico = 1,
  CapoTecnico = 2,
  Altro = -1
}

export enum enum_statiWorkflowQdC {
  Non_Definito = 0,
  Da_Eseguire = 400,
  Eseguito = 401
}

export enum enum_Menu_Agenda_NG_Mode {
  Standard = "",
  PUA = "AggiungiAlPua"
}

export enum enum_OrigineApp {
  GiasApp = "APP",
  Demetra = "DEMETRA",
  //Origine App per i dati provenienti da NewAgri
  PUA = "PUA"
}

export enum enum_salva_in{
  SalvaInAziendaSU = 1,
  SalvaInPrimaAziendaPadre = 2,
  SalvaInAziendaCorrente = 3,
}
/** Usata sia per i valori di `UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO`
 * che per quelli di `Utente_OpzioneChiusuraAbbattimenti`
 */
export enum enum_Valori_UTENTE_COD_RACCOLTA_TIPO_NUOVO_IMPIANTO {
  OLD_NON_APRIRE_NUOVI_ESERCIZI_NUOVI_IMPIANTI = 0, //Gestiti solo nella vecchia Raccolta.aspx
  OLD_APRI_NUOVI_ESERCIZI_NUOVI_IMPIANTI = 1,//Gestiti solo nella vecchia Raccolta.aspx
  LASCIARE_IMPIANTI_ATTIVI_DEFAULT = 10,
  CHIUDI_ESERCIZI_DEFAULT = 20,
  CHIUDI_APRI_ESERCIZI_DEFAULT = 30,
  CHIUDI_IMPIANTI_ESERCIZI_DEFAULT = 40,
  CHIUDI_APRI_IMPIANTI_ESERCIZI_DEFAULT = 50,
  CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_DEFAULT = 60,
  LASCIARE_IMPIANTI_ATTIVI_VINCOLO = 70,
  CHIUDI_ESERCIZI_VINCOLO = 80,
  CHIUDI_APRI_ESERCIZI_VINCOLO = 90,
  CHIUDI_IMPIANTI_ESERCIZI_VINCOLO = 100,
  CHIUDI_APRI_IMPIANTI_ESERCIZI_VINCOLO = 110,
  CHIUDI_APPEZZAMENTI_IMPIANTI_ESERCIZI_VINCOLO = 120
}

/**
 * Le imprese con tipo Cooperativa, Consorzio o OrganizzazioneProduttore
 * possono essere scelte come padri per altre imprese.
 */
export enum enum_TipoImpresaGerarchia {
  Impresa = 1,
  Cooperativa = 2,
  Consorzio = 3,
  OrganizzazioneProduttore = 4
}

export enum enum_Esportazioni_Sistema_Cod {
  Enogis = 1,
  Artea = 2,
  BDN = 3,
  XFarm = 4,
  OnPlantImport = 5,
  OnPlantExport = 6,
  HubIoT = 7,
  Nogmo = 8,
  Fex = 9,
  AsipoImport = 10,
  AsipoOrdiniMVExport = 11,
  AsipoOrdiniMVGetStato = 12,
  Recap_Ricette_CAI = 20,
  OrogelBI = 21,

  Demetra_Import_Analisi = 22,
  Demetra_Export_Analisi = 23,
  Demetra_Import_Attivita = 24,
  Demetra_Export_Attivita = 25,
  Demetra_Import_Fabbricati = 26,
  Demetra_Export_Fabbricati = 27,
  Demetra_Import_LavoratoriQDC = 28,
  Demetra_Export_LavoratoriQDC = 29,
  Demetra_Import_Fornitori = 30,
  Demetra_Export_Fornitori = 31,
  Demetra_Import_Macchine = 32,
  Demetra_Export_Macchine = 33,
  Demetra_Import_MovimentiMag = 34,
  Demetra_Export_MovimentiMag = 35,
  Demetra_Import_DataPublish = 36,
  Demetra_Export_DataPublish = 37
}

export enum enum_TipoConfrontoCatasto {
  Planning_PianoColturale = 0,
  PianoColturale_Planning = 1
}

export enum enum_TipoSelect_FiltroneSuperNova {
  Base = 0,
  ImpreseAlbero = 1,
  Imprese = 2,
  Imprese_Visibilita_Appoggio = 13,
  CentriAziendali = 3,
  CentriAziendali_Visibilita_Appoggio = 14,
  Appezzamenti = 4,
  Impianti = 5,
  Movimenti = 6,
  Contatti = 7,
  Campi = 8,
  Imprese_Codici = 9,
  Esercizi = 10,
  AppezzamentiRipartoCatasto = 11,
  Fabbricati = 12,
  Imprese_APP = 15,
  PianoColturale = 16,
  PianoColturaleBudget = 17
}

//#region Filtro Ricerca NEW
export enum Enum_TipoMostra_FiltroRicerca {
  Aziende = 1,
  CentriAziendali = 2,
  Campi = 3,
  Appezzamenti = 4,
  Impianti = 5,
  Esercizi = 6,
  PianoColturale = 7,
  PianoColturaleBudget = 8,
  Fabbricati = 9,
  Movimenti = 10
}

export enum Enum_Entita_FiltroRicerca {
  Azienda = 1,
  CentroAziendale = 2,
  Campo = 3,
  Appezzamento = 4,
  Impianto = 5,
  Esercizio = 6,
  Fabbricato = 7
}

export enum Enum_FiltroDestinazioneUso_FiltroRicerca {
  Tutto = 0,
  SoloDestinazioniUso = 1,
  EscludiDestinazioniUso = 2
}

export enum Enum_FiltroPoligoni_FiltroRicerca {
  Tutto = 0,
  ConPoligoni = 1,
  SenzaPoligoni = 2
}

export enum Enum_FiltroRipartoCatasto_FiltroRicerca {
  Tutto = 0,
  ConRiparto = 1,
  SenzaRiparto = 2
}

export enum Enum_ColonnaData_FiltroRicerca {
  ValiditaInizio = 0,
  ValiditaFine = 1,
  IntervalloValidita = 2,
  DataCreazioneAnagrafica = 3
}

export enum Enum_TipoConfronto_FiltroRicerca {
  Maggiore = 0,
  Minore = 1,
  MaggioreUguale = 2,
  MinoreUguale = 3,
  CompresoFra = 4
}

export enum Enum_ModalitaFiltroData_FiltroRicerca {
  Manuale = 0,
  AnnataAgraria = 1,
  Oggi = 2,
}

export enum Enum_FiltroOperatoreLogico_FiltroRicerca {
  OR_AlmenoUnaCondizioneTrue = 0,
  AND_TutteLeCondizioniTrue = 1,
}

export enum Enum_FiltroOperazioniSelezionate_FiltroRicerca {
  Tutto = 0,
  ConOperazioni = 1,
  SenzaOperazioni = 2,
}

export enum Enum_TipoComportamento_FiltroRicerca {
  Ricerca = 0,
  RicercaAvanzataAzienda = 1,
  EsportaPdf = 2,
  SelezionamentoEntita = 3,
  EsportaExcel = 4
}

//#endregion

//#region Analisi Del Terreno NEW

export enum enum_ID_Area_Alert {
  Contatti = 1,
  Macchine = 2,
  Analisi = 3,
  PianiConcimazione = 4,
  Nitrati = 5,
  AgricolturaDiPrecisione = 6,
  UMA_Carburanti = 7,
  Richiesta_Iscrizione_GIAS = 8,
  Report_Gias = 9,
  Documenti_Contabili = 10,
  Operazioni_Campagna_QDC = 11,
  Catasto = 12,
  Carichi_Scarichi = 13
}

export enum enum_ID_Area_Tipologia {
  Patentino_trattamenti = -1,
  Taratura_ugelli = -2,
  Carta_Identita = -3,
  Analisi_terreno = -4,
  Piano_Concimazione = -5,
  PUA = -6,
  Laboratorio = -7,
  OrganismoDiControllo = -8,
  AgricolturaDiPrecisione_MappaPrescrizione = -9,
  AgricolturaDiPrecisione_MappaProduzione = -10,
  AgricolturaDiPrecisione_FileBordoMacchina = -11,
  Doc_Template_Richiesta_Iscrizione_GIAS = -12,
  Richiesta_Iscrizione_GIAS = -13,
  Scheda_Campagna_Completa = -14,
  UMA_dichiarazione_pre_assegnazione = -15,
  Fatture_Attive_Da_Sistema_Esterno = -16,
  Fatture_Passive_Da_Sistema_Esterno = -17,
  Ordini_Acquisto = -18,
  DDT_Ricevuti = -19,
  Conferimenti = -20,
  DDT_Emessi = -21,
  Ordini_Vendita = -22,
  Fatture_Passive = -23,
  Fatture_Attive = -24,
  Operazioni_Campagna_QDC = -25,
  Contratti_Affitto = -26,
  Possesso_Particelle = -27,
  Carichi_Magazzino = -28,
  Scarichi_Magazzino = -29
}

export enum enum_AnalisiTipo {
  Analisi_Terreno = 1,
  Analisi_Terreno_Fanghi = 2,
  Analisi_Acqua = 3,
  Analisi_Residui_Fitofarmaci_Vegetali = 4,
  Analisi_Latte = 5,
  Analisi_Vino = 6,
  Analisi_Residui = 7,
  Analisi_Fitofarmaci = 8,
  Analisi_Organolettiche = 9,
  Analisi_Varie = 10,
  Analisi_Merceologiche = 11,
  Analisi_Del_Sangue = 12
}

export enum enum_AnalisiTipologia_Schema {
  Piano_Concimazione = -1,
  Area_Omogenea = -2
}

export enum enum_Entita_Analisi {
  NonDefinito = 0,
  Impresa = 1,
  Centro = 2,
  Campo = 3,
  Appezzamento = 4,
  Impianto = 5,
  Fabbricato = 6,
  Particella = 7,
  EntitaGrafica = 8
}

export enum enum_RapportiContabili_SaCod {
  Tutti = 0,
  PersoneGiuridiche = 1,
  PersoneFisiche = 2
}


//#endregion

export const SpecialNavigation = {
  None: '0',
  OnIframe: '1',
  OnNewWindow: '2'
} as const;
export type SpecialNavigationKey = typeof SpecialNavigation[keyof typeof SpecialNavigation];
