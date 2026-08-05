//VARIABILI GLOBALI

var TRASFORMATI_VEGETALI = 210;
var TRASFORMATI_ANIMALI = 310;

var resxObj = [];
var resxArrPath = [
    "GestioneContabilita/App_LocalResources/RicercaDocContabili.aspx.resx",
    "App_GlobalResources/AgronicaAgenda_2010.resx"
];

//Elenchi Json
var Elenco_Parametri_Qualitativi = ""; 
var Elenco_Specie = ""; //[{ "Veg_Cod": 0, "Veg_Des": "Tutte le Specie Vegetali" }];
var Elenco_Varieta = ""; // [{ "Cul_Cod": 0, "Cul_Des": "Tutte le Varietà Colturali" }];
var ElencoCausali_Riga = [];
var Elenco_Centri_Aziendali = [];

var lavCod_ContrattoAffitto = 2006;

//Template azioni griglie -> Inizializzati in jQueryDocReady.js per le traduzioni
var templateStampaDocumento = "";
var templateModificaDocumento = "";
var templateEliminaDocumento = "";
var templateStampaEtichetteDettaglio = "";
var templateStampaEtichetteTestata = "";
var templateVisualizzaDocumento = "";
var templateSbloccaDocumento = "";
var templateBloccaDocumento = "";
var templateCampionamento = "";
var templateNuovoAllegato = "";
var templateGestioneAllegati = "";

//--------------------------------------------------------------------------------
// Si riportano a seguire le CostantiPersonalizzate relative a TYPE e DOC_TYPE
// per utilizzo lato Javascript
//--------------------------------------------------------------------------------
// TYPE
//--------------------------------------------------------------------------------
const DocContab_TipoRicerca_Acquisti = "A";
const DocContab_TipoRicerca_Vendite = "V";
const DocContab_TipoRicerca_Conferimenti = "C";
const DocContab_TipoRicerca_Contratti = "CO";
//--------------------------------------------------------------------------------
// DOC_TYPE
//--------------------------------------------------------------------------------
const DocContab_TipoDoc_Ordine = "O";
const DocContab_TipoDoc_Consegna = "C";
const DocContab_TipoDoc_Fattura = "F";
const DocContab_TipoDoc_Pomodoro = "P";
const DocContab_TipoDoc_ContrattoAffitto = "AF";
//--------------------------------------------------------------------------------

var elencoCausali = []; // Inizializzato in jQueryDocReady.js per le traduzioni

var ancoraLivelliPossibili = true;
var buttonGroupSelected = 0;
var versioneJsonFiltri = "1.1";

//var ultimaRigaSelezionataGrigliaTestata = { rowIndex: -1, id: -1, page: -1 };
//var ultimaRigaSelezionataGrigliaDettaglio = { rowIndex: -1, id: -1, page: -1 };
var stoRipristinandoFiltri = false;
var personalizzazioniGriglie = null;

// ModuloGenerazione
var Modulo_Cantine = 1;
var Modulo_FreshFood = 2;
var Modulo_Tabacco = 3;
var Modulo_Zoo = 5;

const LAV_COD_DESTINAZIONE = {
    FATTURE_EMESSE: 1001
}

// Costanti gruppo colonne
const gruppoColonneEconomico = "Economico";

/** Utilizzata nella gestione delle pratiche collegate ai documenti quando non è possibile determinare i permessi dell'utente 
 * sugli specifici stati delle pratiche */
var utente_conflittoPermessiWorkflow = "";