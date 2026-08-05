var Separatori_Conferimento_LottiUC = [
    { Separatore_Config: 0, Separatore_Config_Des: "" },
    { Separatore_Config: 1, Separatore_Config_Des: "." },
    { Separatore_Config: 2, Separatore_Config_Des: "-" }
];

const enLotto_Config_FF = {
    lcff_CODICE_FORNITORE: 1,
    lcff_DATA: 2,
    lcff_ORA: 3,
    lcff_LOTTO_IMPIANTO: 4,
    lcff_DOCUMENTO: 5,
    lcff_AZIENDA: 6,
    lcff_CODICE_DESTINAZIONE: 7,
    //lcff_SIGLA_SPECIE_VARIETA: 8,
    lcff_CONTATORE_UNIVOCO: 9,
    lcff_ACCETTAZIONE: 10,
    lcff_ANNO_AA: 11,
    lcff_CODICE_LINEA: 12,
    lcff_CODICE_ARTICOLO: 13,
    lcff_CONTATORE_UNIVOCO_PARAMETRI: 14,
    lcff_ANNO_SOCIO_LINEA_GAP: 15,
    lcff_NUMERO_SETTIMANA: 16,
    lcff_GIORNO_GIULIANO: 17,
    lcff_TABELLA_ASSEGNA: 18,
    lcff_CODICE_APPEZZAMENTO: 19,
    lcff_CODICE_SITO_PRODUZIONE: 20
};

var ParametriLotto_Conferimento_LottiUC = [
    { Algoritmo_Config: enLotto_Config_FF.lcff_CODICE_FORNITORE, Algoritmo_Config_Des: "Codice Fornitore" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_DATA, Algoritmo_Config_Des: "Data Ingresso Merce" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_ORA, Algoritmo_Config_Des: "Ora Ingresso Merce" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_LOTTO_IMPIANTO, Algoritmo_Config_Des: "Lotto Impianto Colturale" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_DOCUMENTO, Algoritmo_Config_Des: "Numero Documento" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_AZIENDA, Algoritmo_Config_Des: "Associazione Azienda-Referenza" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CODICE_DESTINAZIONE, Algoritmo_Config_Des: "Codice Magazzino/Cella Stoccaggio" },
    //{ Algoritmo_Config: enLotto_Config_FF.lcffProd_SIGLA_SPECIE_VARIETA, Algoritmo_Config_Des: "Sigla Specie e Varietà" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CONTATORE_UNIVOCO, Algoritmo_Config_Des: "Contatore Univoco di Carico (6 Cifre)" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_ACCETTAZIONE, Algoritmo_Config_Des: "Numero Accettazione" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_ANNO_AA, Algoritmo_Config_Des: "Anno - AA" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CODICE_LINEA, Algoritmo_Config_Des: "Codice Linea" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CODICE_ARTICOLO, Algoritmo_Config_Des: "Codice Articolo" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CONTATORE_UNIVOCO_PARAMETRI, Algoritmo_Config_Des: "Contatore Univoco Prodotto/Calibro/Qualità" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_ANNO_SOCIO_LINEA_GAP, Algoritmo_Config_Des: "Anno-Socio-Linea-Certificazione" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_NUMERO_SETTIMANA, Algoritmo_Config_Des: "Numero Settimana" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_GIORNO_GIULIANO, Algoritmo_Config_Des: "Giorno Giuliano" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_TABELLA_ASSEGNA, Algoritmo_Config_Des: "Assegnazione tramite associazione fornitore/prodotto" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CODICE_APPEZZAMENTO, Algoritmo_Config_Des: "Codice Appezzamento" },
    { Algoritmo_Config: enLotto_Config_FF.lcff_CODICE_SITO_PRODUZIONE, Algoritmo_Config_Des: "Codice Sito di Produzione" }
];

var dsParametriLotto = null;
var attivaModifica = false;

var Preparazione_Cod_Conferimento_LottiUC = 0;
var Separatore_Config_Conferimento_LottiUC = 0;
var Parametro_Cod_Conferimento_LottiUC = 0;

