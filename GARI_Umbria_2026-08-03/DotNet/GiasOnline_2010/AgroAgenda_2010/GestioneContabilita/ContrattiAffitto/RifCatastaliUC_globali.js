//VARIABILI GLOBALI

//Elenchi Json

var Elenco_Particelle;

// Variabili

var tipoOpTestataRifCatastali = enum_TipoOperazioneDB.Lettura.value;
var rigaInserita = new Object();
var rigaModificata = new Object();
var rigaCancellata = new Object();
var Elenco_Particelle_RifCatastali = "";
var Elenco_Particelle_RifCatastali_Validita_Iniz = new Date(1000,0,1);
var Elenco_Particelle_RifCatastali_Validita_Fine = new Date(1000,0,1);

// Costanti

const idTabGrigliaRifCatastali = "tab_griglia_rif_catastali";
const tabGrigliaRifCatastali = "#" + idTabGrigliaRifCatastali;
const decimaliSuperficie = 4;

const nomeColonnaParticella = "Particella";
const nomeColonnaSuperficieAffittata = "Superficie Affittata";
const nomeColonnaSuperficieCatastale = "Superficie Catastale";
const nomeColonnaRedditoDominicale = "Reddito Dominicale";
const nomeColonnaRedditoAgrario = "Reddito Agrario";
const nomeColonnaDataInizioAffitto = "Data Inizio Affitto";
const nomeColonnaDataFineAffitto = "Data Fine Affitto";
const nomeColonnaBioVincolo = "Vincolo Bio";
const nomeColonnaCodiceParticella = "Codice";

// Enum

const enum_TipoOperazioneRiga = {
    Inserimento: "inserimento",
    Modifica: "modifica"
};

const enum_forzaCancellaLegami = {
    Indefinita: 0,
    Si: 1,
    No: 2
};

//FINE VARIABILI GLOBALI