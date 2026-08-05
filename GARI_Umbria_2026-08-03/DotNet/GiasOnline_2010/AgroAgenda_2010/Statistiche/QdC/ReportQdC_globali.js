//VARIABILI GLOBALI

//Elenchi Json
var Elenco_Parametri_Qualitativi = ""; 
var Elenco_Specie = [{ "Veg_Cod": 0, "Veg_Des": "Tutte le Specie Vegetali" }];
var ddlTipoOperazioni = [{ "Gru_Cod": 0, "Gru_Des": "Tutti" }];
var Elenco_Operazioni = [{ "LAV_COD": 0, "LAV_DES": "Tutti" }];
var Elenco_Nazioni = undefined;
var Elenco_Regioni = [{ "REG": 0, "Regione_Des": "Tutti" }];
var Elenco_Province = [{ "PROV": 0, "PROVINCIA": "Tutti" }];
var Elenco_Comuni = [{ "COM": 0, "LOCALITA": "Tutti" }];
var Elenco_Referenti = undefined;
var Elenco_Aziende = undefined;
var Elenco_Centri = [{ "PivaSa": 0, "sa_nome": "Tutti" }];
var Elenco_Estrazione = [];
var Elenco_Varieta = ""; // [{ "Cul_Cod": 0, "Cul_Des": "Tutte le Varietà Colturali" }];

var ElencoMisureImpianti = {} 

var ElencoMisureOperazioni = {}

var ElencoMisureOperazioniImpianti = {}

var ElencoMisureOperazioniProdotti = {}

var ElencoMisureOperazioniProdottiImpianti = {}

var ElencoDimensioniImpianti = {}

var ElencoDimensioniOperazioni = {}

var ElencoDimensioniOperazioniImpianti = {}

var ElencoDimensioniProdotti = {}

var ElencoDimensioniOperazioniProdottiNoImpianti = {}

var ElencoDimensioniProdottiImpianti = {}

var personalizzazioni = null;
var personalizzazioniPivot = null;

var ancoraLivelliPossibili = true;

var buttonGroupSelected = 0;

const enum_Tipo_Report_PDF = {
    Statistica_mese_anno_confronto_fra_anni: "0",
    Statistica_mese_singolo_anno_su_quantità_altro_Valore: "1",
    Statistica_anno_con_scostamento_e_previsioni: "2"
}

const LAVCOD_CORRISPETTIVO_VENDITA = "1020";
const LAVCOD_AUTOCONSUMO = "1028";
