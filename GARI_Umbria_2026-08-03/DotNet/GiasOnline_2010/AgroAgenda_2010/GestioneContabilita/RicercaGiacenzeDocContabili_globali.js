//VARIABILI GLOBALI

var TRASFORMATI_VEGETALI = 210;

//Elenchi Json
var Elenco_Parametri_Qualitativi = ""; 
var Elenco_Specie = ""; //[{ "Veg_Cod": 0, "Veg_Des": "Tutte le Specie Vegetali" }];
var Elenco_Varieta = ""; // [{ "Cul_Cod": 0, "Cul_Des": "Tutte le Varietà Colturali" }];
var ElencoCausali_Riga = [];
var Elenco_Centri_Aziendali = [];

var elencoUtilizzi = {
    Trattamenti: "Tr",
    CreazioneFattureAcquisto: "Fa",
    CreazioneFattureVendita: "Fv",
    CreazioneDdtVenditaOrdine: "Cv"
};

var elencoCausali = [

    { "TYPE": "A", "DOC_TYPE": "F", "LAV_COD": "1000", "LAV_DES": "Fattura ricevuta" },
    { "TYPE": "A", "DOC_TYPE": "F", "LAV_COD": "1002", "LAV_DES": "Nota di credito ricevuta" },
    { "TYPE": "V", "DOC_TYPE": "F", "LAV_COD": "1001", "LAV_DES": "Fattura emsessa" },
    { "TYPE": "V", "DOC_TYPE": "F", "LAV_COD": "1003", "LAV_DES": "Nota di credito emessa" },
    //{ "LAV_COD": "1020", "LAV_DES": "Corrispettivo di vendita" },
    { "TYPE": "A", "DOC_TYPE": "C", "LAV_COD": "1025", "LAV_DES": "DDT ricevuto" },
    //{ "LAV_COD": "1028", "LAV_DES": "Autoconsumo" },
    { "TYPE": "V", "DOC_TYPE": "C", "LAV_COD": "1031", "LAV_DES": "DDT emesso" },
    { "TYPE": "V", "DOC_TYPE": "C", "LAV_COD": "1069", "LAV_DES": "DDT corrispettivo" },
    //{ "LAV_COD": "1053", "LAV_DES": "Ricevuta fiscale" },
    //{ "LAV_COD": "1071", "LAV_DES": "MVV emesso" },
    { "TYPE": "V", "DOC_TYPE": "O", "LAV_COD": "2002", "LAV_DES": "Ordine di vendita" },
    { "TYPE": "A", "DOC_TYPE": "O", "LAV_COD": "2004", "LAV_DES": "Ordine di acquisto" }

];

var ancoraLivelliPossibili = true;
var buttonGroupSelected = 0;
var versioneJsonFiltri = "1.1";

var personalizzazioniGriglia = null;