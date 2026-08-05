//Variabili Globali
var UtenteAbilitatoSLettura;
var UtenteAbilitatoScrittura;

var hfId_Elenco_val;

var elencoindici = "";
var elencoindicidb = "";
var elencodettagli = "";
var elencoaree = [];
var bAllegato_Modificato = false;
var allegati_documenti_cod = 0;
var id_area_old = 0;
var id_tipologia_old = 0;
var Estensione = "";
var bDatiNecessariInseriti = true;

var bcheckcontatto = false;
var bcheckmacchina = false;
var bcheckuma = false
var bcheckanalisi = false
var bcheckagenda = false;
var bcheckricette = false;
var bcheckparticellecatastali = false;

let tipoTipoEntitaCod = [];


var data_creazione = ""; // data riferimento per caricamento DDL Indici se in inserimento = data di sistema, se in modifica = data creazione documento
var tipoPermessoDaControllare = 1
var spanDownloadAllegato = "";

var indirizzohttp = "./Scad_CreaModificaItem.aspx";

var scadCreaModItemResx = [];
var resxArrPath = [
    "App_GlobalResources/AgronicaAgenda_2010.resx",
    "Scadenzario/App_LocalResources/Scad_CreaModificaItem.aspx.resx"
];

var FlagDataScadenzaObbligatoria = false;
var DataDefault;

//Anna 29 / 04 / 22: aggiunto multiselect tipologie al documento
var multiCmbTipologia = null;
var elementimultiCmbTipologia = 0;


//Anna 25/05/22: aggiunto componente kendoUpload per caricamento di multi Allegati 
var listaDocumenti = [];
var cancellaTuttiDocumenti = false;
var cancellaSingoloDocumento = false;

var dummy_UID = []; //variabile globale istanziata per poter accedere agli elementi del kendo upload all'interno del reader
var UID_elementoCancellazione = ""; //variabile globale istanziata per poter rintracciare l'elemento triggerato dopo il kendo confirm

var UploadMultiploAllegatiAbilitato = false;
var usaUploadMultiplo = "";
var CompressoDaGIAS = false;

var initialFiles = [];
var initialFiles_UID = null;
var rimuoviInitialFile = 1;
var divKendoUpload_InitialFiles;


//TIPO ENTITA SECONDARIO
var buttonGroupSelected_TipoEntitaSecondario = 2;
var grigliaAttivita_caricata = false;
var grigliaRicetteBrogliaccio_caricata = false;
var grigliaRiferimenti_caricata = false;
var grigliaVisite_caricata = false;
var grigliaParticelleCatastali_caricata = false;

var operazioneSelezionataModifica;
var ricettaBrogliaccioSelezionataModifica;

var indexGrigliaDaMostrare_TipoEntitaSecondario = -1;
var mostraEntrambeGriglie_Riferimenti_RicetteBrogliaccio = false;

//Non viene passato in query string come per ID_Agtenda, Richiesta_Cod etc... 
//(non esiste una pagina collegata al documentale per le particelle catastali), quindi utilizzo una variaible globale
var cId_ImpresexParticelle = 0;

var Provienienza_Edit_Audit = false;
var Pagina_Inizializzata = false;

var checkDataDocumentoObbligatoria = false;