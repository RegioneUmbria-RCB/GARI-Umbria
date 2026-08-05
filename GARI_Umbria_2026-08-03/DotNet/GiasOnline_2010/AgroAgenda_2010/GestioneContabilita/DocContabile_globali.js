// INIZIO VARIABILI GLOBALI docContabile_globali

var indirizzoHttp_DocContabile_WS = "../GestioneContabilita/DocContabile_WS.aspx";

var resxObj = [];
var resxArrPath = [
    "GestioneContabilita/App_LocalResources/DocContabile.aspx.resx",
    "App_GlobalResources/AgronicaAgenda_2010.resx"
];

function getYearVersioneKendo() {
    let yearKendo = 0;
    if (typeof kendo !== 'undefined' && kendo !== undefined && kendo.version !== undefined) {
        let arrVers = kendo.version.split(".");
        if (arrVers.length >= 1 && !isNaN(arrVers[0])) {
            yearKendo = parseInt(arrVers[0]);
        }
    }
    return yearKendo;
}
var yearVersioneKendo = getYearVersioneKendo();


var documentoLoaded = false;
var documentoValidatoPrimaVolta = false;

var gestioneContabilita = enum_Livello_GestContabilita_NonGestita;
var contattiAcc4ConGerarchia = false; //utilizzo di 4 contatti per l'accettazione (stile Fruttagel): Conferente, 1° Cooperativa, 2° Cooperativa, Produttore (sfruttando la gerarchia)
var FF_gest_materiale_vivaistico = false; // Utilizzo di numero la posto di Kg, tare imballaggi non obbligatoria
var flagSceltaImputazione = false;

var lavCodAccettazione = false;
var lavCodAccettazionePomodoro = false;
var parametriPomodoro = {};
var contrattoPomodoro = null;
var riepilogoPomodoro = null;
var lavCodDocEmesso = false;
var lavCodOrdine = false;
var lavCodFattura = false;
var lavCodDocAttivo = false;
var lavCodVendita = false;
var lavCodMovMagazzino = false;

var selezionaCentro = true;
var isContattoImpresaGias = false;
var isProduttoreImpresaGias = false;

var xSa_Cod = 0;
var xFabbricato_Cod = 0;
var xTipoDestinazione = 0; 

// Elenchi
var elencoUdmOptimized = null;
var elencoUdmOptimizedRegolamento = null;
var elencoPUA_Regolamenti = null;
var elencoPUA_Regolamenti_ElemCod = -9999;

var elencoCausali_Riga = null;
var elencoNumeratori = null;
var elencoLottiAccettazione_FormProdottoUC = null;
var elencoIVA_Aliquote_ScontoMerce = null;  //Sotto insieme ristretto per le aliquote sceglibili in caso di sconto merce

var elencoConfezionamentoLotto = null;

// Validatori
var validatorIntestazione = null;
var validatorTabTestata = null;
var validatorTabDettaglio = null;

// Flag gestione imballi FF
var paramQualGestiti_FF = null;
var gestitoImballaggio_FF = false;
var gestitoContenitore_FF = false;
var gestitoConfezione_FF = false;

var paramQual_FF_filtrospevar = null;
// DA GESTIRE IN FUTURO
//var gestitoImballaggio_FF_filtrospevar = false; 
//var gestitoContenitore_FF_filtrospevar = false;
//var gestitoConfezione_FF_filtrospevar = false;

// Stati Panelli
var statiPanelsBar = null;
var tabStripGiaApplicati = [];
var panelBarToccatiDopoLoad = [];
//var rifMovDettaglio = null;
var listRifMovDettaglio = [];

//  CATEGORIE MAGAZZINO
var SENZA_CATEGORIA = 0;
var MACCHINE = 1;
var CARBURANTI = 2;
var FERTILIZZANTI = 3;
var RIFIUTI = 4;
var SEMENTI = 10;
var FORMULATI = 191;
var INSETTI = 196;
var TRAPPOLE = 197;
var INNESCHI = 198;
var ALTRE_MATERIE = 200;
var SEMILAVORATI_VEGETALI = 201;
var MATERIE_VEGETALI = 204;
var BENI_CONFEZ_VEGETALE = 205;
var TRASFORMATI_VEGETALI = 210;
var ZOO_CONSISTENZA = 300;
var SEMILAVORATI_ANIMALI = 301;
var MATERIE_ANIMALI = 304;
var BENI_CONFEZ_ANIMALE = 305;
var TRASFORMATI_ANIMALI = 310;
var MANGIMI = 306;
var FARMACI = 307;
var CONFEZIONI_PRODOTTI = 400;
var RICAMBI = 401;
var ALTRI_BENI = 501;
var RIGA_DESCRIZIONE_LIBERA = 502;
var SERVIZI = 555;
var ALTRI_BENI_AMMORTIZZABILI = 500;
var CAT_MAG_SERVIZI_PROFESSIONALI = 700;

// USO TRAPPOLE
const enum_TrappoleUso = {
    "Monitor": 1,
    "CattureDiMassa": 2,
    "ConfusioneSessuale": 3,
    "Disorientamento": 4
};

//Casistica Visibilità/Obbligatorietà del degrado
const enum_DegradoVisibilita = {
    "Facoltativo": 1,
    "Obbligatorio": 2,
    "Non_Gestito": 0
};

// ModuloGenerazione
var Modulo_Cantine = 1;
var Modulo_FreshFood = 2;
var Modulo_Tabacco = 3;
var Modulo_Zoo = 5;

// CauMov
var CAU_REGISTRAZIONE = "4000";
var CAU_REGISTRAZIONE_SECONDARIA = "4050";
var CAU_CARICO = "7300";
var CAU_SCARICO = "7350";
var CAU_TRASFERIMENTO = "7380";

// Causali Agenda
var CAU_ANIMALE = "3001";
var CAU_MAGAZZINO = "7001";         

// Variabili FormProdottoUC
var old_Lotto_FormProdottoUC = "";
var old_ConfezionamentoLotto_FormProdottoUC = "";
var riga_originale_entrata_FormProdottoUC = null;
var maxOrdineDet = 0;

const enum_posizioneConfezionamentoLotto = {
    "Nessuna": 0,
    "Inizio": 1,
    "Fine": 2
};

// Inizio Gestione lotti e giacenza
var impostazioni_Blocca_SottoGiacenza = false;
var impostazioni_Categorie_Giacenza = [];
var impostazioni_Categorie_Lotti = [];
var impostazioni_Edit_Lotto_Accettazione = false;
var impostazioni_DataScadenza_Lotto = [];

var enum_Gestione_Giacenze_SoloMovimentati = 0;
var enum_Gestione_Giacenze_SoloPresenti = 1;      //giacenza > 0
var enum_Gestione_Giacenze_TuttiProdotti = 2;
var enum_Gestione_Lotti_Nessuna = 0;
var enum_Gestione_Lotti_Obbligatoria = 1;
var enum_Gestione_Lotti_Facoltativa = 2;
// Fine Gestione lotti e giacenza

// Tipi Magazzino
var ESSICCATORIO = 222;
var MAGAZZINO = 20;
var STALLA = 15;
var CELLA = 16;
var FABBRICATI_NO_STALLE = -999;

//tabStrip_dettagli
var tabStrip_Dettagli;
var index_tabStrip_Dettagli_Riepilogo = 0;
var index_tabStrip_Dettagli_Dettaglio = 1;
var index_tabStrip_Dettagli_ScaricoGiacenza = 2;
var index_tabStrip_Dettagli_ImputazioneImpianti = 3;

// Pua Regolamenti
var enum_PUARegolamenti_Tipo_PianoComcimazione = 1;
var enum_PUARegolamenti_Tipo_PUA = 2;

// Livello Gestione Contabilità
var enum_Livello_GestContabilita_NonGestita = 0;
var enum_Livello_GestContabilita_Fatturazione = 1;
var enum_Livello_GestContabilita_Bilancio = 2;

// Utilizzati per decidere se ricaricare i parametri qualitativi
var paramQualGestiti_filtrospevar_FF = null;
var current_Elem_Cod = -1;
var current_Veg_Cod = -1;
var current_Cul_Cod = -1;
var current_Reg_Cod = 0;
var current_Mat_Cod_OMNI = -1;
var current_Prodotto_Cod = "";

// Utilizzata per sapere che sono in edit di una singola riga
var sonoInDettaglioRigaDoc = false;

// Utilizzata per sapere che sono in edit di una riga imballi nella griglia dentro alal form prodotto
var sonoInModificaImballi = false;

// Utilizzato per individuare il tipo di contatto nella gestione dei contatti
const enum_tipologia_Contatto = { "cedente": 1, "cessionario": 2, "agente": 3, "vettore": 4, "capoArea": 5 };

// Pesi e imballi riscontrati
var flagPesiRiscontrati = false;
var gestionePesiRiscontrati = false;
// Fine pesi e imballi riscontrati

// Utilizzato per memorizzare l'iva di default del prodotto per risettarla, quando si passa dallo sconto merce e si torna ad altro tipo di sconto
var Last_Iva_Default_Prodotto = null;

// Utilizzato per distinguere il primo caricamento del form di riga
var inizializzaFormDettaglioRiga = true;


var imputazioneImpianti_gestioneAbilitata = null;
var raccolteXConferimenti_gestioneAbilitata = null;

var raccolteXConferimenti_dsSelezionate = [];

var veg_cod_pomodoro = 52;

// Utilizzata per proposta dati riga
var propostaDatiRiga = undefined;

// Utilizzata temporaneamente per predisporre la gestione regolamento formulati
var gestioneRegolamentoFormulati = false;

// Varie per filtro descrizione prodotti

const FiltroQualsiasiProdotto = "%%%";
const LunghezzaMinimaFiltroProdotto = 3;
const LunghezzaMinimaFiltroContatto = 5;
const Evento_Prodotto_Open = "Prodotto_Open";
const Evento_Prodotto_Filtering = "Prodotto_Filtering";
const Evento_MagazzinoScarico_Change = "MagazzinoScarico_Change";
const Evento_DataEmissione_Change = "DataEmissione_Change";
var NoDataTemplateDefaultProdotto = "";

// Aggiornamento magazzino

const enum_JollyInt = {
    "MagazzinoMovimentato": 0,
    "MagazzinoNonMovimentato": 1
};

const CategorieMagazzinoNonMovimentato = [RIGA_DESCRIZIONE_LIBERA, ALTRI_BENI, SERVIZI];

// Gestione causale trasporto

var idControlloCausaleTrasporto = "inCausaleTrasporto";

//sportello
var validitaSportello = undefined;

// Contratti di Affitto

const enum_tipoDataValidita = {
    "Inizio": 1,
    "Fine": 2
};

const enum_ContestoControlloDate = {
    "OnChange": 1,
    "Salvataggio": 2
};

const paramQualSenzaResetCalCod = ["sustdeclnumber", "sustdecldate", "isccredcompliant", "chaincustody", "art29compliant", "waste", "ghgactual", "kmdistance"];
const paramQualValorizzatiInLavorazioni = [
    { tabellaCodDes: "sustdeclnumber", valDefault: 0 },
    { tabellaCodDes: "sustdecldate", valDefault: new Date().toLocaleDateString() },
    { tabellaCodDes: "isccredcompliant", valDefault: -98 },
    { tabellaCodDes: "chaincustody", valDefault: -101 },
    { tabellaCodDes: "art29compliant", valDefault: -104 },
    { tabellaCodDes: "waste", valDefault: -106 },
    { tabellaCodDes: "ghgactual", valDefault: -107  },
    //{ tabellaCodDes: "kmdistance", valDefault: 0 },
    //{ tabellaCodDes: "ghgforetd", valDefault: 0 },
];
const paramQualISCC = ["iscccorrente", "iscccorrentedate", "isccprecedente", "isccprecedentedate"];
var gestitiParamQualISCC = false;

var impedisciCreazioneCarichiMultiriga = false;

var ultimoFiltroServerFilteringCessionario = {
    inCedenteCessionario1: null,
    inCedenteCessionario2: null
};

var applicaServerFilteringSuiCessionari = false;

//FINE VARIABILI GLOBALI docContabile_globali