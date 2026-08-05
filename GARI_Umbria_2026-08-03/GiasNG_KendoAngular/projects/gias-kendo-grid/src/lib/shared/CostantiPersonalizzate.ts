export const PIVASUPERUSER_COLDIRETTI: string = "05644051004";

export const AGRODATAINIZIO: Date = new Date(1900, 0, 1, 0, 0, 0, 0);
export const AGRODATAFINE: Date = new Date(2100, 11, 31, 0, 0, 0, 0);

export var AGRODATAINIZIO_SERVER_STR = "1899-12-31T23:00:00";
export var AGRODATAFINE_SERVER_STR = "2100-12-30T23:00:00";

export const LOCALIZATION_LANGUAGES: string[] = ['it', 'en', 'fr', 'pt'];

// Tipo fabbricati destinazioni Tabella 'Mov_Destinazioni'
export const TIPO_DESTINAZIONE_IMPIANTO = 0;
export const TIPO_DESTINAZIONE_MAGAZZINO = 20;
export const TIPO_DESTINAZIONE_VASCA = 13;
export const MAGAZZINO = 20;
export const STALLA = 15;
export const CONSISTENZA = 18;
export const VASCA_ENOLOGICA = 13;
export const CELLA_FRIGORIFERA = 16;
export const SILOS = 17;
export const TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA = 21;
export const ESSICCATOIO = 222;
export const TIPO_DESTINAZIONE_ANIMALE = 1;

// CATEGORIE MAGAZZINO
export const RIGA_DESCRIZIONE_LIBERA = 502;
export const RIGA_DESCRIZIONE_LIBERA_DES = 'Riga Descrizione Libera';
export const ALTRI_BENI_AMMORTIZZABILI = 500;
export const ALTRI_BENI = 501;
export const ALTRI_BENI_DES = 'Altri Beni Strumentali';
export const SERVIZI = 555;
export const SERVIZI_DES = 'Servizi';

export const CORPI_ESTRANEI = -50;
export const CALI_LAVORAZIONE = -1;

export const ELEMCOD_MANODOPERA = 0;
export const MACCHINE = 1;
export const CARBURANTI = 2;

export const RIFIUTI = 4;

export const FERTILIZZANTI = 3;
export const FORMULATI = 191;
export const COADIUVANTI = 195;
export const INSETTI = 196;
export const TRAPPOLE = 197;
export const INNESCHI = 198;

export const SEMENTI = 10;
export const ALTRE_MATERIE = 200;
export const ZOO_CONSISTENZA = 300;

export const SEMILAVORATI_VEGETALI = 201;
export const MATERIE_VEGETALI = 204;
export const BENI_CONFEZ_VEGETALE = 205;
export const TRASFORMATI_VEGETALI = 210;

export const SEMILAVORATI_ANIMALI = 301;
export const MATERIE_ANIMALI = 304;
export const BENI_CONFEZ_ANIMALE = 305;
export const TRASFORMATI_ANIMALI = 310;

export const MANGIMI = 306;
export const FARMACI = 307;

export const CONFEZIONI_PRODOTTI = 400;
export const RICAMBI = 401;

export const RIGA_DESCRIZIONE = 502;

export const CAT_MAG_SERVIZI_PROFESSIONALI = 700;

// =====================================================================================================

//Elenco dei Cod_Rapporto (codici dei rapporti contabili di base)
// Esiste anche Enum <see cref="TipiEnumerativi.enum_Rapporti_Contabili_Standard"/>
export const COD_LEGALE = -1
export const COD_CLIENTE = -2
export const COD_FORNITORE = -3
export const COD_CLIENTE_FORNITORE = -23 //fittizio, usato come raggruppamento nel gias online
export const COD_DIPENDENTE = -4
export const COD_TERZISTA = -5
export const COD_DIPENDENTE_TERZISTA = -45  //fittizio, usato come raggruppamento nel gias online
export const COD_TECNICO = -6
export const COD_CENTRO_MACCHINE = -7
export const COD_LAB_ANALISI = -8
//creati x fruttagel ma mai usati
//export const COD_SOCIO_CONFERENTE = -1001
//export const COD_COOP_CONFERENTE = -1002
//export const COD_PRODUTTORE_ASSOCIATO_COOP = -1003
//export const COD_PRODUTTORE_INDIVIDUALE = -1004
export const COD_SOCIO = -9      // socio della ditta, titolare
export const COD_TRASPORTATORE = -10
export const COD_CLIENTEFORNITORE = -11
export const COD_CLIENTE_FORNITORE_BolleAccett = -11 //utilizzato nelle bolle di accettazione, ad esempio da fruttagel
export const COD_TECNICORESPONSABILE = -12
export const COD_AGENTE = -13
export const COD_REFERENTEAZIENDALE = -14
export const COD_CONSULENTE = -15
export const COD_SPEDIZIONIERE = -16
export const COD_VIVAIO = -17
export const COD_CONFERENTE = -18
export const COD_LAB_CQ = -19
export const COD_CAPO_AREA = -20
export const COD_AVVENTIZIO = -21
export const COD_SMALTITORE = -22
export const COD_FORNITORE_ORTOFRUTTA = -24
export const COD_ORGANISMO_REFERENTE = -25
export const COD_RAPPRESENTANTE_FISCALE = -26
export const COD_COADIUVANTE_FAMILIARE = -27
export const COD_ORGANISMO_CONTROLLO = -28
export const COD_RIFERIMENTO_TRASFERIMENTO_DATI = -29

//=====================================================================================================

// =====================================================================================================

export const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI = 3020;
export const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI = 3021;
export const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI = 3022;
export const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI = 3023;


export const LAVCOD_CONCIA_SEME = 13;
export const LAVCOD_DISERBO = 18;
export const LAVCOD_TRATTAMENTO_ANTIPARASSITARIO = 74;
export const LAVCOD_TRATTAMENTO_FITOREGOLATORE = 103;
export const LAVCOD_DISTRIBUZIONE_INSETTI = 116;
export const LAVCOD_CONFUSIONE_SESSUALE = 118;
export const LAVCOD_DISORIENTAMENTO_SESSUALE = 121;
export const LAVCOD_GEODISINFESTAZIONE = 155;
export const LAVCOD_DISSECCAMENTO = 158;

export const LAVCOD_CATTURE_MASSA = 122;
export const LAVCOD_TRATTAMENTO_POST_RACCOLTA = 163;
export const LAVCOD_INSTALLAZIONE_TRAPPOLE = 107;
export const LAVCOD_REINNESCO_TRAPPOLE = 150;

export const LAVCOD_ARATURA = 8;
export const LAVCOD_ANDANAMENTO = 9;

export const LAVCOD_FERTIRRIGAZIONE = 26;
export const LAVCOD_DISTRIBUZIONE_CONCIME = 14;
export const LAVCOD_CONCIMAZIONE_FOGLIARE = 123;
export const LAVCOD_DISTRIBUZIONE_AMMENDANTI = 124;
export const LAVCOD_SARCHIATURA_CONCIMAZIONE = 156;
export const LAVCOD_TRATTAMENTO_ANTIBUTTERATURA = 106;

export const LAVCOD_ASPORTAZIONE_ORGANI_INFETTI = 120;
export const LAVCOD_ASSOLCATURA = 10;
export const LAVCOD_CARICO_MANUALE_FRUTTA = 78;
export const LAVCOD_CIMATURA = 12;
export const LAVCOD_DIRADAMENTO_MANUALE = 17;
export const LAVCOD_DISSODAMENTO = 19;
export const LAVCOD_ERPICATURA = 21;
export const LAVCOD_ERPICATURA_ROTANTE = 157;
export const LAVCOD_ESPIANTO = 81;
export const LAVCOD_ESTIRPATURA = 22;
export const LAVCOD_FALCIACONDIZIONATURA = 23;
export const LAVCOD_FALCIATURA_ERBAI = 25;
export const LAVCOD_FORMAZIONE_ARGINELLI = 27;
export const LAVCOD_FRANGIZOLLATURA = 31;
export const LAVCOD_FRESATURA = 32;
export const LAVCOD_GEBIATURA = 152;
export const LAVCOD_IMBALLO_FIENO_ROTOLI = 33;
export const LAVCOD_INTERRAMENTO_PAGLIE = 34;
export const LAVCOD_INTERVENTO_ANTIBRINA = 161;
export const LAVCOD_LAVORAZIONE_CONBINATA = 154;
export const LAVCOD_LAVORAZIONE_TRA_FILA = 82;
export const LAVCOD_LAVORAZIONE_SU_FILA = 83;
export const LAVCOD_LEGATURA = 84;
export const LAVCOD_LIVELLAMENTO = 40;
export const LAVCOD_MANUTENZIONE_ARGINI = 41;
export const LAVCOD_MESSA_DIMORA_PIANTE = 85;
export const LAVCOD_MIETITREBBIATURA = 46;
export const LAVCOD_MINIMUM_TILLAGE = 47;
export const LAVCOD_PACCIAMATURA = 48;
export const LAVCOD_POTATURA_SECCA = 87;
export const LAVCOD_POTATURA_VERDE = 88;
export const LAVCOD_PRESSATURA = 49;
export const LAVCOD__RACCOLTA_LEGNA_POTATURA = 90;
export const LAVCOD_RANGHINATURA = 53;
export const LAVCOD_RINCALZATURA = 55;
export const LAVCOD_RIPPATURA = 115;
export const LAVCOD_RIPUNTATURA = 56;
export const LAVCOD_RIVOLTAMENTO_FORAGGIO = 57;
export const LAVCOD_ROMPICROSTA = 153;
export const LAVCOD_RULLATURA = 58;
export const LAVCOD_SARCHIATURA = 59;
export const LAVCOD_SCARIFICATURA = 62;
export const LAVCOD_SCASSO = 63;

export const LAVCOD_TRINCIATURA = 75;
export const LAVCOD_VANGATURA = 76;
export const LAVCOD_ZAPPATURA = 77;
export const LAVCOD_RACCOLTA_MANUALE = 91;
export const LAVCOD_RACCOLTA_MECCANICA = 92;
export const LAVCOD_RILIEVO_PRODUZIONE_E_DATA_RACCOLTA = 125;
export const LAVCOD_STRIGLIATURA = 167;
export const LAVCOD_PIRODISERBO = 168;
export const LAVCOD_ALTRE_OPERAZIONI = 162;
export const LAVCOD_ABBATTIMENTOIMPIANTI = 170;
export const LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE = 172;

export const SMARTPHONE_WIDTH = 1000;

export const NessunDpiNessunaEtichetta = "-999";

export const NessunDpi = "0";

export const DpiBio = "-2";

//VISIBILITA'
export const SACOD_NOFILTRO = -99;
export const PRIVATO = 0;
export const PUBBLICO = -1;

export const LinkQdCAngular = "/QdC/";

export const LinkVisiteEditAngular = "/QdC/Visite-Edit";

//Valore standard per escludere le microgiacenze
export const QTA_GiancenzeVisualizzate = 0.00009;

export const Qta_Ha_Distribuibile_Prodotto = 1000000;
export const Qta_Ha_Distribuibile_Prodotto_Ricette = 1000000;

export const timeZones = {
    "Dateline Standard Time": "",
    "UTC-11": "",
    "Hawaiian Standard Time": "",
    "Aleutian Standard Time": "",
    "Marquesas Standard Time": "",
    "Alaskan Standard Time": "",
    "UTC-09": "",
    "Pacific Standard Time (Mexico)": "",
    "UTC-08": "",
    "Pacific Standard Time": "",
    "US Mountain Standard Time": "",
    "Mountain Standard Time": "",
    "Mountain Standard Time (Mexico)": "",
    "Yukon Standard Time": "",
    "Central America Standard Time": "",
    "Central Standard Time": "",
    "Central Standard Time (Mexico)": "",
    "Easter Island Standard Time": "",
    "Canada Central Standard Time": "",
    "SA Pacific Standard Time": "",
    "Eastern Standard Time (Mexico)": "",
    "Eastern Standard Time": "",
    "Haiti Standard Time": "",
    "US Eastern Standard Time": "",
    "Cuba Standard Time": "",
    "Turks And Caicos Standard Time": "",
    "Paraguay Standard Time": "",
    "Venezuela Standard Time": "",
    "Central Brazilian Standard Time": "",
    "SA Western Standard Time": "",
    "Atlantic Standard Time": "",
    "Pacific SA Standard Time": "",
    "Newfoundland Standard Time": "",
    "Tocantins Standard Time": "",
    "E. South America Standard Time": "",
    "Argentina Standard Time": "",
    "SA Eastern Standard Time": "",
    "Greenland Standard Time": "",
    "Montevideo Standard Time": "",
    "Magallanes Standard Time": "",
    "Saint Pierre Standard Time": "",
    "Bahia Standard Time": "",
    "UTC-02": "",
    "Mid-Atlantic Standard Time": "",
    "Azores Standard Time": "",
    "Cape Verde Standard Time": "",
    "UTC": "",
    "GMT Standard Time": "",
    "Greenwich Standard Time": "",
    "Sao Tome Standard Time": "",
    "Morocco Standard Time": "",
    "W. Europe Standard Time": "Europe/Rome",
    "Central Europe Standard Time": "",
    "Romance Standard Time": "",
    "W. Central Africa Standard Time": "",
    "Central European Standard Time": "",
    "GTB Standard Time": "",
    "Middle East Standard Time": "",
    "Egypt Standard Time": "",
    "E. Europe Standard Time": "",
    "Syria Standard Time": "",
    "West Bank Standard Time": "",
    "Israel Standard Time": "",
    "South Africa Standard Time": "",
    "FLE Standard Time": "",
    "South Sudan Standard Time": "",
    "Kaliningrad Standard Time": "",
    "Sudan Standard Time": "",
    "Libya Standard Time": "",
    "Namibia Standard Time": "",
    "Jordan Standard Time": "",
    "Arabic Standard Time": "",
    "Turkey Standard Time": "",
    "Arab Standard Time": "",
    "Belarus Standard Time": "",
    "Russian Standard Time": "",
    "E. Africa Standard Time": "",
    "Volgograd Standard Time": "",
    "Iran Standard Time": "",
    "Arabian Standard Time": "",
    "Astrakhan Standard Time": "",
    "Azerbaijan Standard Time": "",
    "Russia Time Zone 3": "",
    "Mauritius Standard Time": "",
    "Saratov Standard Time": "",
    "Georgian Standard Time": "",
    "Caucasus Standard Time": "",
    "Afghanistan Standard Time": "",
    "West Asia Standard Time": "",
    "Ekaterinburg Standard Time": "",
    "Pakistan Standard Time": "",
    "Qyzylorda Standard Time": "",
    "India Standard Time": "",
    "Sri Lanka Standard Time": "",
    "Nepal Standard Time": "",
    "Central Asia Standard Time": "",
    "Bangladesh Standard Time": "",
    "Omsk Standard Time": "",
    "Myanmar Standard Time": "",
    "SE Asia Standard Time": "",
    "Altai Standard Time": "",
    "W. Mongolia Standard Time": "",
    "North Asia Standard Time": "",
    "N. Central Asia Standard Time": "",
    "Tomsk Standard Time": "",
    "North Asia East Standard Time": "",
    "Singapore Standard Time": "",
    "China Standard Time": "",
    "W. Australia Standard Time": "",
    "Taipei Standard Time": "",
    "Ulaanbaatar Standard Time": "",
    "Aus Central W. Standard Time": "",
    "Transbaikal Standard Time": "",
    "Tokyo Standard Time": "",
    "North Korea Standard Time": "",
    "Korea Standard Time": "",
    "Yakutsk Standard Time": "",
    "Cen. Australia Standard Time": "",
    "AUS Central Standard Time": "",
    "E. Australia Standard Time": "",
    "AUS Eastern Standard Time": "",
    "West Pacific Standard Time": "",
    "Tasmania Standard Time": "",
    "Vladivostok Standard Time": "",
    "Lord Howe Standard Time": "",
    "Bougainville Standard Time": "",
    "Russia Time Zone 10": "",
    "Central Pacific Standard Time": "",
    "Magadan Standard Time": "",
    "Norfolk Standard Time": "",
    "Sakhalin Standard Time": "",
    "Russia Time Zone 11": "",
    "New Zealand Standard Time": "",
    "UTC+12": "",
    "Fiji Standard Time": "",
    "Kamchatka Standard Time": "",
    "Chatham Islands Standard Time": "",
    "UTC+13": "",
    "Tonga Standard Time": "",
    "Samoa Standard Time": "",
    "Line Islands Standard Time": ""
}

export const FORBIDDEN_CHARS_FOR_NAMES: string[] = ['/', '.', '\'', ':', '*', '?', '"', '<', '>', '|'];
