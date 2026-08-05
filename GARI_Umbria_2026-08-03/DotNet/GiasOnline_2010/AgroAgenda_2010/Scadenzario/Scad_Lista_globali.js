var indirizzohttp = "./Scad_Lista.aspx";

var UtenteAbilitatoScrittura;
var Elenco_Aree = "";
var Elenco_Tipologie = "";
//var elencoValidazioni;
var bcheckstoricoabilitato = false;


var filterable_validazioneTemplate;

var scadListaResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Scadenzario/App_LocalResources/Scad_Lista.aspx.resx"
];

//Anna 29 / 04 / 22: aggiunta campi al filtro di ricerca
var Elenco_Validazioni = [];
var Validazioni_Filtro = []; //elenco validazine +obj vuoto per selezione filtro

//Anna 02/05/22: modificato campo Storico in DDL
var Elenco_Storico = [];


var win_CompressoDaGIAS;
var windowCompressoDaGIASAperta = false;

var listaDocumenti = [];
var initialFiles = [];
var initialFiles_UID = null;

var UploadMultiploAllegatiAbilitato = false;

var workflowAbilitato = true;

//Anna Salvataggio filtri/personalizzazioni griglia
var personalizzazioniGriglie = null;
var versioneJsonFiltri = "1.1";

var currentPiva = '';