//#region VARIABILI GLOBALI

var usaNuovaVisibilita = true;
var primo_ddlAzienda_OnDataBound = 0;

//ELENCHI JSON
var macroSuperfici;
var letturaMacrosuperfici = {};
var ElencoTabAperte = [];
var UpdateVal = {}
var nIscrizioneCameraDiCommercio = {};
var updatedRecords = [];
var newRecords = [];
var deletedRecords = [];

var elencoLavUMA = null;
var elencoLavGIAS = null;
var elencoAttivitaGIAS = null;
var elencoLavValidita = null;
var dataPassaggioDiStato = null;
var lavorazioni = null;
var lavorazioniSelezionate = [];
var elencoCarburanti = null;
var tabellaCalcoloCosti = null;

//VARIABILI
var tempAssegnato = 0;
var ID_Attivita_Base = 0;
var ID_Attivita_Base_Desc = "";
var bValore_Enabled = true;
var btrovaimpianti = true;
var battivita_enabled = true;
var PusantiContab = 0;
var richiesta_cod = -1;
var carburante_Richiesto_Benzina = 0;
var carburante_Richiesto_Gasolio = 0;
var carburante_Richiesto_Gasolio_Serra = 0;
var gruppo_col;
var enum_SUPERUSER_UTILIZZO_ORARI_INIZIO_FINE = 831;
var kendoAziende
var pivaInsertGrid = 0;
var Veg_Cod = 0;
var Id_Cod = 0;
var dtRichiesta = null;
var dtPraticheSuccessive = null;
var macro_cod = -1;
var program_cod = 0;
var pivaSelezionata = "";
var richiestaRinuncia = false;
var lav_incrociati = {};
var lav_alt = {};
var lav_NO = {};
var lav_alt_lim = {};
var lav_alt_lim_sup = {};
var lav_alt_superfici = {};
var lav_alt_terreni = {};
var colt_elem_orig = {};
var colt_elem_added = {};
var colt_elem = 0;
var verifica_terzisti = {};
var lav_note_obbligatorie = {};
var permessiAssegnamento = false;
var showPanels = "false";

var percentualeZonaPendenzaB = 0.2;
var percentualeTerrenoMedio = 0.2;
var percentualeTerrenoTenace = 0.4;

var gestioneBiologico = false;

var gestioneCarbResx = [{}];
var resxArrPath = [];

var permesso_richiesta;
var permesso_rendicontazione;
var permesso_approvazione_richiesta;
var permesso_approvazione_rendicontazione;

var modifica_richiesto = true;
var modifica_assegnato = false;
var modifica_rimanenze = false;
var modifica_rimanenze_conferma = false;
var modifica_rinuncia = false;

var TxtRimanenza_Gasolio;
var TxtRimanenza_Benzina;
var TxtRimanenza_Gasolio_Serra;

var AcquistatoAnnoPrecedente_Gasolio;
var AcquistatoAnnoPrecedente_Benzina;
var AcquistatoAnnoPrecedente_Gasolio_Serra;
var proseguiAnticipo = false

var permessoAcquaGiaRichiesto = 0

var gasolioTerz;
var benzinaTerz;
var gasolioSerraTerz;
var gasolioTerzAppro;
var benzinaTerzAppro;
var gasolioSerraTerzAppro;
var bZooInizializato = false;

var dataPrimoAcquisto;
var totaleAcquistato;
var integrativa = false;

var gasolioTotTerz;
var benzinaTotTerz;
var gasolioSerraTotTerz;
var gasolioTotTerzLAV = {};
var benzinaTotTerzLAV = {};
var gasolioSerraTotTerzLAV = {};

//var lavMaxQuattro = [10003, 10013, 10017, 10028, 10076, 10145, 10149, 10159, 10218, 10228]

var Percentuale_Decurtamento = 0;
var L_Maggiorazione_Trasferimenti = 6;
var Percentuale_Anticipo_Carb = 0; //aggiunta gloria
var Gestione_Rimanenze = 0;
var ddlFascicoli = {}

var RichiestaDocumenti = [];
var confRichiestaDocumentiResx = [{}];
var intRichiestaDocumenti = false;

var tabVerbIstrInizializzata = false;

var noRichiestaSuccessiva = false;

var percentualePrelievo = 0;
var programmazione_cod = 0;

var listaCUAARendicontati = []
var listaCUAAPresentiInRichiesta = []

var consentitoAggiungereModificareRendicontazione_daSetup = true
var Data_Inizio_Rendicontazione = ""
var Data_Fine_Rendicontazione = ""

var consentitoEditareDocumenti_daSetup = true
var Termine_Ultimo_Rendicontazione = ""

var consentitoInserireRendicontazioneContoProprio = true
var nonConsentitoInserireRendicontazioneContoProprio_Mess = ""

var personalizzazioniGrigliaDocumenti

var elencoRegolamenti = [{ "Regolamento_Cod": 1, "Regolamento_Des": "Normale" }, { "Regolamento_Cod": 4, "Regolamento_Des": "Biologico" }]

var verificaAppezzamentiEffettuata = false
var anomalieSuperficiAppezzamenti = null

//STRINGHE CONTROLLI
var SuperficieOltreFascicolo = "La superficie indicata supera la superficie da fascicolo di: ";
var SuperficieNegativa = "La superficie indicata è negativa o nulla ";
var SuperficieOltre = "La superficie indicata supera la superficie massima dichiarata di: ";
var LavSovrapposte = "Rilevata sovrapposizione di ettari lavorati su questo terreno relativi a questa lavorazione: ";
var LavSovrapposteProprio = "Rilevata sovrapposizione di lavorazioni su questo terreno: "
var SommaVariTipi = "La somma delle superfici dei vari tipi di terreno ";
var CUAA_Fasc_GC = "Non è possibile inserire CUAA-Fascicolo-Gruppo colturale U.M.A più volte";
var volumeAcqua = "E' necessario specificare il volume di acqua disponibile e le note relative";
var eccessoAcqua = "I litri richiesti per le lavorazioni di irrigazione superano il massimo consentito dal permesso di attingimento acqua specificato"
var errorCell = "errorCell";
var warningCell = "warningCell";

//#endregion

//#region COSTANTI GLOBALI

//FASCICOLI SPECIALI
const codiceFascicoloColtureNonImputabili = -1
const codiceFascicoloAnticipi = -2
const codiceFascicoloTrasferimenti = -3
const codiceFascicoloPianoColturale = -4

const descrizioneFascicoloColtureNonImputabili = "Colture non imputabili al Fascicolo"
const descrizioneFascicoloAnticipi = "Anticipi"
const descrizioneFascicoloTrasferimenti = "Trasferimenti"
const descrizioneFascicoloPianoColturale = "Piano Colturale"

//MACROUSI E LAVORAZIONI SPECIALI
const macrousoCod_EccedenzaAnticipi = "9996"
const lavorazioneCod_QuotaAnticipoEccedente = "10236"
const lavorazioneCod_QuotaAnticipoEccedenteAccisePagate = "10237"
const macrousoCod_TrasferimentiEffettuati = "9995"
const lavorazioneCod_QuotaTrasferita = "10350"

//ENUMERATIVI RICHIESTA AGENDA
const DOCUMENTALE = 1
const ANAGRAFICA = 2

//ENUMERATIVI TIPO AZIENDA
const Azienda_Agricola_Privata = 1
const Azienda_Terzista = 2
const Cooperativa_Agricola = 3
const Azienda_Agricola_Istituzioni_Pubbliche = 4
const Consorzio_Bonifica_Irrigazione = 5

//ENUMERATIVI FORMA GIURIDICA

//---PUBBLICHE
const Università = 18
const Consorzio_Di_Bonifica = 19

//---PRIVATE
const Imprenditore_Individuale_Agricolo = 1
const Società_Semplice = 2
const Società_In_Nome_Collettivo = 3
const Società_In_Accomandita_Semplice = 4
const Studio_Associato_E_Società_Di_Professionisti = 5
const Società_Per_Azioni = 6
const Società_a_Responsabilità_limitata = 7
const Società_a_Responsabilità_limitata_Con_Un_Unico_Socio = 8
const Società_In_Accomandita_Per_Azioni = 9
const Società_Cooperativa_a_Mutualità_Prevalente = 10
const Società_Cooperativa_Diversa = 11
const Società_Cooperativa_Sociale = 12
const Società_Di_Mutua_Assicurazione = 13
const Società_Consortile = 14
const Associazione_o_Raggruppamento_temporaneo_Di_Imprese = 15
const Gruppo_Europeo_Di_Interesse_Economico = 16
const Altra_Forma_Di_Ente_Privato_Con_Personalità_Giuridica = 17

//ENUMERATIVI STATI WORKFLOW
const In_Compilazione = 2001
const Compilazione_Alla_Data_Completata_e_Verificata = 2007
const Rinuncia = 2009
const Verifica_In_Corso = 2002
const Verifica_Completata = 2003 //Verifica completata indica che la fase di verifica si è conclusa ancora senza un verdetto (è precedente allo stato completata con successo)
const Verifica_Intermedia_Completata_Con_Riserva = 2004
const Verifica_Intermedia_Completata_Con_Successo = 2005
const Verifica_Intermedia_Non_Superata = 2006
const Inserimento_Completato_Per_Il_Periodo_Di_Competenza = 2008

//ENUMERATIVI TIPO CARBURANTE
const Gasolio = 2
const Benzina = 3
const Gasolio_Serra = 8

//ENUMERATIVI REGOLAMENTO
const RegolamentoEntrambi = 0
const RegolamentoConvenzionale = 1
const RegolamentoBiologico = 4

//#endregion

