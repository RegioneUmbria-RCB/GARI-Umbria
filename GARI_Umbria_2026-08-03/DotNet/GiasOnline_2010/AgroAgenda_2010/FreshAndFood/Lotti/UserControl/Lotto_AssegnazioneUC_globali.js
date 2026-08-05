
var indirizzohttp_Lotto_AssegnazioneUC = "";
var rigaDuplicataKendoGrid = false;
var rigaDaCopiareKendoGrid;
var elencoQualitaAssegnaLotto = [];
var elencoCertificAssegnaLotto = [];
var elencoTipologieLavorazioniAssegnaLotto = [];
var rigaDaSalvareDaKendoWindowAssegnaLotto = null;


const enLotto_Config_Lavorazioni_FF = {
    lcffProd_CODICE_FORNITORE: 1,
    lcffProd_DATA: 2,
    //lcffProd_CODICE_DESTINAZIONE: 3,
    //lcffProd_SIGLA_SPECIE_VARIETA: 4,
    lcffProd_CONTATORE_UNIVOCO: 5,
    lcffProd_ANNO_AA: 6,
    lcffProd_CODICE_LINEA: 7,
    lcffProd_CODICE_ARTICOLO: 8,
    //lcffProd_CONTATORE_UNIVOCO_PARAMETRI: 9,
    lcffProd_NUMERO_SETTIMANA: 10,
    lcffProd_GIORNO_GIULIANO: 11,
    lcffProd_LOTTO_ENTRATA: 12,
    lcffProd_CALIBRO: 13,
    lcffProd_QUALITA: 14,
    lcffProd_LOTTO_TESTATA: 15
};

var elencoParametriAssegnaLotto = [
    { Algoritmo_Config: 0, Algoritmo_Config_Des: "" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_FORNITORE, Algoritmo_Config_Des: "Codice Fornitore" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_DATA, Algoritmo_Config_Des: "Data Ingresso Merce" },
    //{ Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_DESTINAZIONE, Algoritmo_Config_Des: "Codice Magazzino/Cella Stoccaggio" },
    //{ Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_SIGLA_SPECIE_VARIETA, Algoritmo_Config_Des: "Sigla Specie e Varietà" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CONTATORE_UNIVOCO, Algoritmo_Config_Des: "Contatore Univoco di Carico (6 Cifre)" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_ANNO_AA, Algoritmo_Config_Des: "Anno - AA" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_LINEA, Algoritmo_Config_Des: "Codice Linea" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CODICE_ARTICOLO, Algoritmo_Config_Des: "Codice Articolo" },
    //{ Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CONTATORE_UNIVOCO_PARAMETRI, Algoritmo_Config_Des: "Contatore Univoco Prodotto/Calibro/Qualità" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_NUMERO_SETTIMANA, Algoritmo_Config_Des: "Numero Settimana" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_GIORNO_GIULIANO, Algoritmo_Config_Des: "Giorno Giuliano" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_ENTRATA, Algoritmo_Config_Des: "Lotto Entrata in Lavorazione" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_CALIBRO, Algoritmo_Config_Des: "Calibro" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_QUALITA, Algoritmo_Config_Des: "Qualità" },
    { Algoritmo_Config: enLotto_Config_Lavorazioni_FF.lcffProd_LOTTO_TESTATA, Algoritmo_Config_Des: "Lotto Testata Lavorazione" }
];


var elencoSeparatoriAssegnaLotto = [
    { Separatore_Config: 0, Separatore_Config_Des: "" },
    { Separatore_Config: 1, Separatore_Config_Des: "." },
    { Separatore_Config: 2, Separatore_Config_Des: "-" }
];

var elencoRegolamentiAssegnaLotto = [
    { Reg_Cod: 0, Reg_Des: "" },
    { Reg_Cod: 1, Reg_Des: "Convenzionale (Reg. Nessuno)" },
    { Reg_Cod: 4, Reg_Des: "Biologico (Reg.CE 834/07 (Ex.Reg.CE 2092/91))" }
];
