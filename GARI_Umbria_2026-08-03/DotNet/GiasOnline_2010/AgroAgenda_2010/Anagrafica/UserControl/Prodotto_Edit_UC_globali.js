var Prodotto_Edit_UC_Elenco_Specie = null;
var Prodotto_Edit_UC_Elenco_Categorie_Magazzino = null;
var Prodotto_Edit_UC_Elenco_Categorie_Commerciali = null;
var Prodotto_Edit_UC_Elenco_Varieta = "";
var Prodotto_Edit_UC_Keys = null;
var ElencoUnitadimisura = null;
var ElencoContatti = null;
var Prodotto_Edit_UC_Elenco_Prodotti_Extra_Privata = "";

var elencoTecnologieSementi = []

var tabStripAperti = [];

var Movimentazioni_Prodotto = [];

const categorieProdotti = {
    MACCHINE_ATTREZZATURE: 1,
    CARBURANTI: 2,
    FERTILIZZANTI: 3,
    RIFIUTI: 4,
    SEMENTI: 10,
    FROMULATI_COADIUVANTI_CORROBOANTI_FISIOFARMACI: 191,
    COADIUVANTI: 195,
    ALTRE_RISORSE: 200,
    SEMILAVORATI_PRODUZIONE_VEGETALE: 201,
    MATERIE_PRIME_VEGETALI: 204,
    BENI_CONFEZIONAMENTO_VEGETALI: 205,
    TRASFORMATI_VEGETALI: 210,
    CONSISTENZA_ZOOTECNICA: 300,
    SEMILAVORATI_PRODUZIONE_ANIMALE: 301,
    MATERIE_PRIME_ANIMALI: 304,
    BENI_CONFEZIONAMENTO_ANIMALI: 305,
    MANGIMI: 306,
    FARMACI: 307,
    TRASFORMATI_ANIMALI: 310,
    CONFEZIONI_PRODOTTI: 400,
    RICAMBI: 401,
    ALTRI_BENI_AMMORTIZZABILI: 500,
    SERVIZI_PROFESSIONALI: 700
}

const enum_tipoOperazione = {
    Lettura: 0,
    Scrittura: 1,
    Modifica: 2,
    Cancella: 3,
    Duplica: 10
}

const enum_TipoPeso = {
    Peso_Lordo: 0,
    Peso_Netto: 1
}

const enum_OmniTabelle = {
    ot_NESSUNO: 0,
    ot_CALIBRO: 1,
    ot_QUALITA: 3,
    ot_IMBALLAGGI_FF: 4,
    ot_CONFEZIONI_FF: 5,
    ot_CONTENITORI_FF: 8,
    ot_CERTIFICAZIONE_FF: 12,
    ot_RUGGINOSITA: 22
}

const enum_Omni_Modulo_Generazione = {
    Nessuno : 0,
    Cantine : 1,
    FreshFood : 2,
    Tabacco : 3,
    Olio : 4,
    Zoo : 5
}

var categorieProdotti_FiltrabiliXSpecieVarieta = [
    //Elem_Cod=10
    categorieProdotti.SEMENTI,
    //Elem_Cod=201
    categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE,
    //Elem_Cod=210
    categorieProdotti.TRASFORMATI_VEGETALI,
    //Elem_Cod=700
    categorieProdotti.SERVIZI_PROFESSIONALI
];

var categorieProdotti_FiltrabiliXCategoriaCommerciale = [
    //Elem_Cod=200
    categorieProdotti.ALTRE_RISORSE,
    //Elem_Cod=204
    categorieProdotti.MATERIE_PRIME_VEGETALI,
    //Elem_Cod=2
    categorieProdotti.CARBURANTI,
    //Elem_Cod=10
    categorieProdotti.SEMENTI,
    //Elem_Cod=500 per ora escludo altri beni ammortizzabili
    //categorieProdotti.ALTRI_BENI_AMMORTIZZABILI,
    //Elem_Cod=205
    categorieProdotti.BENI_CONFEZIONAMENTO_VEGETALI,
    //Elem_Cod=201
    categorieProdotti.SEMILAVORATI_PRODUZIONE_VEGETALE,
    //Elem_Cod=401
    categorieProdotti.RICAMBI,
    //Elem_Cod=700
    categorieProdotti.SERVIZI_PROFESSIONALI,
    //Elem_Cod=210
    categorieProdotti.TRASFORMATI_VEGETALI,
    //Elem_Cod=210
    categorieProdotti.TRASFORMATI_VEGETALI,
];

const enum_Produzione_Cod = {
    Produzione_Propria: 1,
    Prodotto_Commercializzato: 0
}

//Per gestire l'Immagine nella tab Dati Vendita Dettaglio
var TipidiImmagineCaricabili = [".gif", ".jpg", ".bmp", ".png"];

var OpenExecuted = false;

//Lo imposto a true se è attivo il Modulo Fresh & Food
var Modulo_FF = false;

//Lo imposto a true se è attivo il Modulo Zoo
var Modulo_Zoo = false;

//Oggetti che contiene i dati delle dropdown dinamiche dei Parametri qualitativi
var paramQual_FF_filtrospevar = null;
var paramQual_FF_indici_GHG = null;
var paramQual_FF_indici_dettagli_GHG = null;
var chiaveETD = "";
var chiaveETD_UDM = "";
var chiaveETD_QTY = "";
var chiaveGHG_Total = "";


var foundParamQualIndici = false;  
var parametri_indici_creati = false;

//Oggetto che contiene il dataSource delle dropdown dinamiche dei Parametri qualitativi
var valoriparamQual_FF_filtrospevar = [];

var Prodotto_Materia_Prima_Letto = null;

var Codice_Articolo_Originale = "";

var Prodotto_Movimentato = false;

var righeInseriteGrid_Prezzi = "";
var righeModificateGrid_Prezzi = "";
var righeEliminateGrid_Prezzi = "";
var righeTutteGrid_Prezzi = "";

var righeInseriteGrid_Traduzioni = "";
var righeModificateGrid_Traduzioni = "";
var righeEliminateGrid_Traduzioni = "";
var righeTutteGrid_Traduzioni = "";


var righeSelezionateGrid_Calibri = "";
var righeSelezionateGrid_Indici = "";


var righeInseriteGrid_Alias = "";
var righeModificateGrid_Alias = "";
var righeEliminateGrid_Alias = "";
var righeTutteGrid_Alias = "";

var erroriSubmitGriglie = false;
var xTipoOperazioneDatiCOntabiliAltriDati = enum_tipoOperazione.Scrittura;

const opzione_codice_duplicato = "SUPERUSER_Consenti_ProdottoCod_Duplicato";
const opzione_codice_max_length = "SUPERUSER_PRODOTTOCOD_MAX_LENGTH";

var Mostra_chkconfezione = false;

var ParametriQualitativiGestitiPerCheckBoxBeniConf = [];

var Prodotto_Edit_UC_DefaultCategoriaProdotto = 0;

//Array che utilizzo SOLO nel caso in cui debba duplicare un materia prima per evitare di fare delle letture
//due volte le faccio già nella Edit_Prodotto, per le tab già valorizzate.
var tabProdotto_Edit_UC_DaDuplicare = [];

var Impostazione_utenteTabConfigurazione = null;

var Cambia_Codice_Articolo = false;

var Prodotto_Edit_UC_Importato = false;

var Prodotto_Edit_UC_Componi_Cod_ArticoloDaCodice_Esterno = false;

var Prodotto_Edit_UC_Alias_Associato_A_Materia_Prima = null;

var Is_OMNI = false;

var resxProdottoEditUC = [];