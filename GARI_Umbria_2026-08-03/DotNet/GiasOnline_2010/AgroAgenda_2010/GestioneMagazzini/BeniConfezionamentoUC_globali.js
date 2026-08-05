         

//VARIABILI GLOBALI

//Elenchi Json

var resxBeniConfezionamentoUC = [];
var Elenco_Tipo_Beni_Confezionamento = [{ Tipo_BC: 0, Tipo_Des_BC: "", Elem_Cod: 0 }, { Tipo_BC: 1, Tipo_Des_BC: "Confezione", Elem_Cod: 205 }, { Tipo_BC: 2, Tipo_Des_BC: "Contenitore", Elem_Cod: 205 }, { Tipo_BC: 3, Tipo_Des_BC: "Imballaggio", Elem_Cod: 205 }];
var Elenco_Beni_Confezionamento;
// var Elenco_Codici_Articolo;

// Variabili


var righeInseriteGrid_Carico;
var righeModificateGrid_Carico;
var righeCancellateGrid_Carico;

var righeInseriteGrid_Scarico;
var righeModificateGrid_Scarico;
var righeCancellateGrid_Scarico;

var enum_GEN_MAGAZZINO_ORTOFRUTTA = 391;

var Sa_Cod = 0;

var tipoOpTestataScarico = enum_TipoOperazioneDB.Lettura.value;

var Sa_Cod_Carico = 0;
var Fabbricato_Cod_Scarico = 0;
var Numero_Colli_Carico = 0;

var Sa_Cod_Scarico = 0;
var Fabbricato_Cod_Carico = 0;
var Numero_Colli_Scarico = 0;

var enum_UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_GIACENZE = 180;
var enum_UTENTE_COD_FILTRO_CATEGORIAMAGAZZINO_LOTTI = 181;
var enum_UTENTE_COD_BLOCCA_SE_SUPERA_GIACENZE = 82;


//var enum_Gestione_Giacenze_SoloMovimentati = "0";
//var enum_Gestione_Giacenze_SoloPresenti = "1";
//var enum_Gestione_Giacenze_TuttiProdotti = "2";
//var enum_Gestione_Lotti_Nessuna = "0";
//var enum_Gestione_Lotti_Obbligatoria = "1";
//var enum_Gestione_Lotti_Facoltativa = "2";

//tabStrip_BeniConfezionamento
var tabStrip_BeniConfezionamento;
var index_tabStrip_BeniConfezionamento_Carico = 0;
var index_tabStrip_BeniConfezionamento_Scarico = 1;

//FINE VARIABILI GLOBALI
