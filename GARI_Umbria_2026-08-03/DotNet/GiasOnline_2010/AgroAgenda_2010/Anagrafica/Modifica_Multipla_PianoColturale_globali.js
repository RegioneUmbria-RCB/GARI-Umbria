/*  V A R I A B I L I   */
var Cmb_Parametri;
var Cmb_Azioni;

var Cmb_Mod_Finalita;
var Cmb_Mod_Disciplinare;
var Cmb_Mod_IAF;
var Cmb_Mod_Tipologia;
var Cmb_Mod_StatoImpianto;
var txt_Mod_N;
var txt_Mod_P;
var txt_Mod_K;
var Cmb_Mod_Varieta;
var Cmb_Mod_Grva;
var Cmb_Metodo_Produzione;
var Txt_Data_Fine_Appezzamento;
var Txt_Data_Fine_Impianto;
var Txt_Data_Inizio_Impianto;
var Cmb_Copertura;
var Cmb_FormaAllevamento;
var Cmb_Portinnesto;
var Txt_Data_Inizio_Portinnesto;
var Txt_Resa;
var Txt_SuFila;
var Txt_TraFila;
var Txt_DataSemina;
var Txt_DataRaccolta;
var Txt_DataFioritura;
var Cmb_Mod_Esercizi;
var Txt_Data_Esercizi;
var Cmb_CapitolatoPrivato;
var Txt_Certificazione;
var Cmb_OrganismoReferente;
var Cmb_MagazzinoConferimento;
var Cmb_ImpIrrigazione;
var Cmb_Regolamento;
var Cmb_Disciplinare;
var Txt_nrAppBio;

var FlagSecondoRaccolto;

var CmbMulti_CertificazioneAziendale;
var CmbMulti_Contributi;
var Cmb_CertificazioneProdotto;
var Cmb_Residuo;
var Cmb_LicenzaColtivazione;
var Cmb_RiferimentoTrasferimentoDati;
var CmbMulti_Tecnico;
var Cmb_PianoSemina;
var Cmb_Prodotto;


var valore_parametri_modificati = {};
var selected_add;
var obj_ModificaMultipla;
var win_ModificaMultipla;
var win_GestioneEsercizi;

var parametri_multipli = false;
var capitolato_required = true;

var ischeckable;

var piva = ""
var findPiva = true

const Enum_ParametriModificaMultiplaPianoColturale = {
    IMP_Finalita: "1",
    ESE_DisciplinareMassimaleNPK: "2",
    IMP_Varieta: "3",
    IMP_GruppoVarietale: "4",
    APP_MetodoProduzione: "5",
    IMP_Copertura: "6",
    ESE_Resa: "7",
    ESE_DataSemina: "8",
    ESE_DataRaccolta: "9",
    ESE_DataFioritura: "10",
    ESE_CapitolatoPrivato: "12",
    ESE_OrganismoReferente: "13",
    ESE_MagazzinoConferimento: "14",
    ESE_Certificazione: "15",
    ESE_N: "16",
    ESE_P: "17",
    ESE_K: "18",
    IMP_ImpIrrigazione: "19",
    ESE_Regolamento: "20",
    ESE_DPI: "21",
    IMP_SuFila: "22",
    IMP_TraFila: "23",
    IMP_FormaAllevamento: "24",
    IMP_Portinnesto: "25",
    IMP_DataInizioPortinnesto: "26",
    APP_DataFineAppezzamento: "27",
    IMP_DataFineImpianto: "28",
    APP_NrAppBio: "29",
    ESE_FlagSecondoRaccolto: "30",
    ESE_CertificazioneAziendale: "31",
    ESE_Contributi: "32",
    ESE_CertificazioneProdotto: "33",
    ESE_Residuo: "34",
    ESE_LicenzaColtivazione: "35",
    ESE_RiferimentoTrasferimentoDati: "36",
    ESE_Tecnico: "37",
    ESE_PianoSemina: "38",
    ESE_Prodotto: "39",
    IMP_DataInizioImpianto: "40"
};

const ParametriEsercizio = [
    Enum_ParametriModificaMultiplaPianoColturale.ESE_DisciplinareMassimaleNPK,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Resa,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_DataSemina,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_DataRaccolta,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_DataFioritura,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_CapitolatoPrivato,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_OrganismoReferente,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_MagazzinoConferimento,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Certificazione,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_N,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_P,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_K,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Regolamento,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_DPI,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_FlagSecondoRaccolto,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneAziendale,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Contributi,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_CertificazioneProdotto,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Residuo,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_LicenzaColtivazione,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_RiferimentoTrasferimentoDati,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Tecnico,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_PianoSemina,
    Enum_ParametriModificaMultiplaPianoColturale.ESE_Prodotto,
];

const ParametriImpianto = [
    Enum_ParametriModificaMultiplaPianoColturale.IMP_Finalita,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_Varieta,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_GruppoVarietale,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_Copertura,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_ImpIrrigazione,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_SuFila,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_TraFila,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_FormaAllevamento,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_Portinnesto,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioPortinnesto,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_DataFineImpianto,
    Enum_ParametriModificaMultiplaPianoColturale.IMP_DataInizioImpianto,
]

const ParametriAppezzamento = [
    Enum_ParametriModificaMultiplaPianoColturale.APP_MetodoProduzione,
    Enum_ParametriModificaMultiplaPianoColturale.APP_DataFineAppezzamento,
    Enum_ParametriModificaMultiplaPianoColturale.APP_NrAppBio,
];

const Enum_EntitaModificaMultiplaPianoColturale = {
    Appezzamenti: "1",
    ImpiantiEsercizi: "2",
    Esercizi: "3"
};

var chiavi = [];