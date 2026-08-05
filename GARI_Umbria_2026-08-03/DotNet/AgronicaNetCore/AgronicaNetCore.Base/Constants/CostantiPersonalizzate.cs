using static AgronicaNetCore.Base.Constants.TipiEnumerativi;

namespace AgronicaNetCore.Base.Constants
{
    public class CostantiPersonalizzate
    {
        static CostantiPersonalizzate()
        {
            OPERAZIONI_GESTITE_APP_DEMETRA_LIST = new()
            {
                LAV_COD.LAVCOD_ALTRE_OPERAZIONI,
                LAV_COD.LAVCOD_ARATURA,
                LAV_COD.LAVCOD_DISERBO,
                LAV_COD.LAVCOD_DISTRIBUZIONE_CONCIME,
                LAV_COD.LAVCOD_ERPICATURA,
                LAV_COD.LAVCOD_FALCIATURA_ERBAI,
                LAV_COD.LAVCOD_FERTIRRIGAZIONE,
                LAV_COD.LAVCOD_FRANGIZOLLATURA,
                LAV_COD.LAVCOD_FRESATURA,
                LAV_COD.LAVCOD_GEODISINFESTAZIONE,
                LAV_COD.LAVCOD_IRRIGAZIONE,
                LAV_COD.LAVCOD_MIETITREBBIATURA,
                LAV_COD.LAVCOD_POTATURA_SECCA,
                LAV_COD.LAVCOD_POTATURA_VERDE,
                LAV_COD.LAVCOD_RANGHINATURA,
                LAV_COD.LAVCOD_SEMINA,
                LAV_COD.LAVCOD_FALCIACONDIZIONATURA,
                LAV_COD.LAVCOD_TRAPIANTO,
                LAV_COD.LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
                LAV_COD.LAVCOD_TRATTAMENTO_FITOREGOLATORE,
                LAV_COD.LAVCOD_TRINCIATURA,
                LAV_COD.LAVCOD_RACCOLTA,
                LAV_COD.LAVCOD_ASPORTAZIONE_ORGANI_INFETTI,
                LAV_COD.LAVCOD_CONCIMAZIONE_FOGLIARE,
                LAV_COD.LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE,
                LAV_COD.LAVCOD_MINIMUM_TILLAGE,
                LAV_COD.LAVCOD_PIRODISERBO,
                LAV_COD.LAVCOD_PASCOLAMENTO_PROPRIO,
                LAV_COD.LAVCOD_PASCOLAMENTO_TERZI,
                LAV_COD.LAVCOD_ANDANAMENTO,
                LAV_COD.LAVCOD_DEFOGLIAZIONE,
                LAV_COD.LAVCOD_ASSOLCATURA,
                LAV_COD.LAVCOD_CARICO_MANUALE_FRUTTA,
                LAV_COD.LAVCOD_CIMATURA,
                LAV_COD.LAVCOD_DIRADAMENTO_MANUALE,
                LAV_COD.LAVCOD_DISSODAMENTO,
                LAV_COD.LAVCOD_ESTIRPATURA,
                LAV_COD.LAVCOD_ESPIANTO,
                LAV_COD.LAVCOD_FORMAZIONE_ARGINELLI,
                LAV_COD.LAVCOD_IMBALLO_FIENO_ROTOLI,
                LAV_COD.LAVCOD_INTERRAMENTO_PAGLIE,
                LAV_COD.LAVCOD_LAVORAZIONE_TRA_FILA,
                LAV_COD.LAVCOD_LAVORAZIONE_SU_FILA,
                LAV_COD.LAVCOD_LEGATURA,
                LAV_COD.LAVCOD_LIVELLAMENTO,
                LAV_COD.LAVCOD_MANUTENZIONE_ARGINI,
                LAV_COD.LAVCOD_MESSA_DIMORA_PIANTE,
                LAV_COD.LAVCOD_PACCIAMATURA,
                LAV_COD.LAVCOD_PRESSATURA,
                LAV_COD.LAVCOD_RACCOLTA_LEGNA_POTATURA,
                LAV_COD.LAVCOD_RINCALZATURA,
                LAV_COD.LAVCOD_RIPPATURA,
                LAV_COD.LAVCOD_RIPUNTATURA,
                LAV_COD.LAVCOD_RIVOLTAMENTO_FORAGGIO,
                LAV_COD.LAVCOD_RULLATURA,
                LAV_COD.LAVCOD_SARCHIATURA,
                LAV_COD.LAVCOD_SCARIFICATURA,
                LAV_COD.LAVCOD_SCASSO,
                LAV_COD.LAVCOD_VANGATURA,
                LAV_COD.LAVCOD_ZAPPATURA,
                LAV_COD.LAVCOD_GEBIATURA,
                LAV_COD.LAVCOD_ROMPICROSTA,
                LAV_COD.LAVCOD_LAVORAZIONE_CONBINATA,
                LAV_COD.LAVCOD_ERPICATURA_ROTANTE,
                LAV_COD.LAVCOD_INTERVENTO_ANTIBRINA,
                LAV_COD.LAVCOD_STRIGLIATURA,
                LAV_COD.LAVCOD_ABBATTIMENTO_IMPIANTI,
            };
            OPERAZIONI_GESTITE_APP_DEMETRA = string.Join(",", OPERAZIONI_GESTITE_APP_DEMETRA_LIST);
        }

        public const string STR_OP_NON_GESTITE = " (  GruppoOperazioni.Tipo IN ('C','Z','P','E','V') AND GruppoOperazioni.Gru_Cod not in ( 5,7,8,9,11 ) AND Operazioni.Lav_Cod NOT IN (1021,1004,1005,1006,1007,1026,1027,1028,1029,1030,1032,1050,1051,1052,1053,1054,1055,1056,1057,1058,1060,1061,1062,1063,1064,1065,1066,1067,1068,1069,1070,1071,1072,1073,1074,1075,1076,1077,1078,30,3005,3006,3007,3008,3009,3010,3011,3012,3013,3014,3015,3016,3017,3018,3019,3024,3025,3026,3027,3029,3031,3032,3022,3021,30233,5004) ) ";

        public const string AgroKey_EncoderDecoder = "cobaltoleccioplutone";
        public const string AGRODATAINIZIO = "1900-01-01T00:00:00";
        public const string AGRODATAFINE = "2100-12-31T00:00:00";
        public const string AgroPrefix_Utente = "{Utente} : ";

        public static readonly DateTime AGRODATAINIZIO_DATE = new(1900, 1, 1);
        public static readonly DateTime AGRODATAFINE_DATE = new(2100, 12, 31);

        public const int NessunaSpecieQdC = -10;

        public const int NONCONTABILE = 1;
        public const int CONTABILE = 2;

        public const int TIPOLOGIA_ANALISI_PDC = -9999999;
        public const string CARTELLA_ANALISI_PDC = "Analisi";

        // template record Serilog di default
        public const string DefaultLogRecord = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}";

        public static string OPERAZIONI_GESTITE_APP_DEMETRA;

        public static List<int> OPERAZIONI_GESTITE_APP_DEMETRA_LIST;
    }

    public class LAV_COD
    {
        public const int LAVCOD_ALTRE_OPERAZIONI = 162;
        // GRUPPO OPERAZIONE = 2 : Rilievi alla Raccolta
        public const int LAVCOD_RACCOLTA = 125;


        // GRUPPO OPERAZIONE = 4 : Lavorazioni
        public const int LAVCOD_IRRIGAZIONE = 1;
        public const int LAVCOD_SEMINA = 2;
        public const int LAVCOD_ARATURA = 8;
        public const int LAVCOD_ANDANAMENTO = 9;
        public const int LAVCOD_ASSOLCATURA = 10;
        public const int LAVCOD_CIMATURA = 12;
        public const int LAVCOD_CONCIA_SEME = 13;
        public const int LAVCOD_DIRADAMENTO_MANUALE = 17;
        public const int LAVCOD_DISERBO = 18;
        public const int LAVCOD_DISSODAMENTO = 19;
        public const int LAVCOD_ERPICATURA = 21;
        public const int LAVCOD_ESTIRPATURA = 22;
        public const int LAVCOD_FALCIACONDIZIONATURA = 23;
        public const int LAVCOD_FALCIATURA_ERBAI = 25;
        public const int LAVCOD_FORMAZIONE_ARGINELLI = 27;
        public const int LAVCOD_FRANGIZOLLATURA = 31;
        public const int LAVCOD_FRESATURA = 32;
        public const int LAVCOD_IMBALLO_FIENO_ROTOLI = 33;
        public const int LAVCOD_INTERRAMENTO_PAGLIE = 34;
        public const int LAVCOD_LIVELLAMENTO = 40;
        public const int LAVCOD_MANUTENZIONE_ARGINI = 41;
        public const int LAVCOD_MIETITREBBIATURA = 46;
        public const int LAVCOD_MINIMUM_TILLAGE = 47;
        public const int LAVCOD_PACCIAMATURA = 48;
        public const int LAVCOD_PRESSATURA = 49;
        public const int LAVCOD_RANGHINATURA = 53;
        public const int LAVCOD_RINCALZATURA = 55;
        public const int LAVCOD_RIPUNTATURA = 56;
        public const int LAVCOD_RIVOLTAMENTO_FORAGGIO = 57;
        public const int LAVCOD_RULLATURA = 58;
        public const int LAVCOD_SARCHIATURA = 59;
        public const int LAVCOD_SCARIFICATURA = 62;
        public const int LAVCOD_SCASSO = 63;
        public const int LAVCOD_SOD_SEDDING = 68;
        public const int LAVCOD_TRAPIANTO = 71;
        public const int LAVCOD_TRATTAMENTO_ANTIPARASSITARIO = 74;
        public const int LAVCOD_TRINCIATURA = 75;
        public const int LAVCOD_VANGATURA = 76;
        public const int LAVCOD_ZAPPATURA = 77;
        public const int LAVCOD_CARICO_MANUALE_FRUTTA = 78;
        public const int LAVCOD_FASI_FENOLOGICHE = 79;
        public const int LAVCOD_ESPIANTO = 81;
        public const int LAVCOD_LAVORAZIONE_TRA_FILA = 82;
        public const int LAVCOD_LAVORAZIONE_SU_FILA = 83;
        public const int LAVCOD_LEGATURA = 84;
        public const int LAVCOD_MESSA_DIMORA_PIANTE = 85;
        public const int LAVCOD_POTATURA_SECCA = 87;
        public const int LAVCOD_POTATURA_VERDE = 88;
        public const int LAVCOD_RACCOLTA_LEGNA_POTATURA = 90;
        public const int LAVCOD_RACCOLTA_MANUALE = 91;
        public const int LAVCOD_RACCOLTA_MECCANICA = 92;
        public const int LAVCOD_TRATTAMENTO_FITOREGOLATORE = 103;
        public const int LAVCOD_INSTALLAZIONE_TRAPPOLE = 107;
        public const int LAVCOD_DANNI_RACCOLTA = 108;
        public const int LAVCOD_RILIEVO_INDICI_MATURITA = 109;
        public const int LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE = 110;
        public const int LAVCOD_RILIEVO_AVVERSITA_CAMPO = 113;
        public const int LAVCOD_RIPPATURA = 115;
        public const int LAVCOD_DISTRIBUZIONE_INSETTI = 116;
        public const int LAVCOD_CONFUSIONE_SESSUALE = 118;
        public const int LAVCOD_RILIEVO_ERBE_INFESTANTI = 119;
        public const int LAVCOD_ASPORTAZIONE_ORGANI_INFETTI = 120;
        public const int LAVCOD_DISORIENTAMENTO_SESSUALE = 121;
        public const int LAVCOD_CATTURE_MASSA = 122;
        public const int LAVCOD_RILIEVO_PIOGGE = 126;
        public const int LAVCOD_REINNESCO_TRAPPOLE = 150;
        public const int LAVCOD_GEBIATURA = 152;
        public const int LAVCOD_ROMPICROSTA = 153;
        public const int LAVCOD_LAVORAZIONE_CONBINATA = 154;
        public const int LAVCOD_GEODISINFESTAZIONE = 155;
        public const int LAVCOD_ERPICATURA_ROTANTE = 157;
        public const int LAVCOD_DISSECCAMENTO = 158;
        public const int LAVCOD_SOVESCIO = 160;
        public const int LAVCOD_INTERVENTO_ANTIBRINA = 161;
        public const int LAVCOD_TRATTAMENTO_POST_RACCOLTA = 163;
        public const int LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO = 165;
        public const int LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO = 166;
        public const int LAVCOD_STRIGLIATURA = 167;
        public const int LAVCOD_PIRODISERBO = 168;
        public const int LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA = 169;
        public const int LAVCOD_ABBATTIMENTO_IMPIANTI = 170;
        public const int LAVCOD_DEFOGLIAZIONE = 171;
        public const int LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE = 172;
        public const int LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA = 173;
        public const int LAVCOD_PASCOLAMENTO_PROPRIO = 174;
        public const int LAVCOD_PASCOLAMENTO_TERZI = 175;

        public const int LAVCOD_VENDITA = 1020;
        public const int LAVCOD_ACQUISTO = 1021;
        public const int LAVCOD_TRASFERIMENTO = 1033;

        // GRUPPO OPERAZIONE = 6 (tutte)
        public const int LAVCOD_BOLLA_EMESSA = 1031;
        public const int LAVCOD_BOLLA_RICEVUTA = 1025;
        public const int LAVCOD_DDT_CONTABILIZZATO_EMESSO = 1069;
        public const int LAVCOD_FATTURA_EMESSA = 1001;
        public const int LAVCOD_FATTURA_RICEVUTA = 1000;
        public const int LAVCOD_FATTURA_PROFESSIONISTI = 1070;
        public const int LAVCOD_FATTURA_PROFORMA = 1064;
        public const int LAVCOD_FATTURA_LIQ_CONF_EMESSA = 1055; //'Emissione Fattura Liquidazione Conferimenti
        public const int LAVCOD_FATTURA_LIQ_CONF_RICEVUTA = 1056; //'Ricevimento Fattura Liquidazione Conferimenti
        public const int LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA = 1057; //'Emissione Autofattura Liquidazione Conferimenti
        public const int LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA = 1058; //'Ricevimento Autofattura Liquidazione Conferimenti
        public const int LAVCOD_RICEVUTA_EMESSA = 1053;
        public const int LAVCOD_ALTRI_RICAVI = 1027;
        public const int LAVCOD_ALTRI_COSTI = 1026;
        public const int LAVCOD_NOTA_ACCREDITO_RICEVUTA = 1002; // RESI/ABBUONI SU ACQUISTI
        public const int LAVCOD_NOTA_ACCREDITO_EMESSA = 1003;   // RESI/ABBUONI SU VENDITE
        public const int LAVCOD_MOV_FINANZIARIO = 1032;
        public const int LAVCOD_REG_COMPENSI = 1005;
        public const int LAVCOD_AUTOFATTURA_BENI_ESTERO = 1004;
        public const int LAVCOD_SPESE_PER_DIPENDENTI = 1006;
        public const int LAVCOD_SPESE_VARIE = 1007;
        public const int LAVCOD_ACQUISTO_MATERIE_PRIME_SOCI = 1060;
        public const int LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI = 1079;

        // GRUPPO OPERAZIONE = 10 (tutte)
        public const int LAVCOD_CARICO = 1022;
        public const int LAVCOD_SCARICO = 1023;
        public const int LAVCOD_ACCETTAZIONE_DIVERSI = 1054;            // Conferimento
        public const int LAVCOD_DISTINTA_CARICO_ACCETTAZIONE = 1076;    // Conferimento
        public const int LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE = 1078;    // Conferimento

        //GRUPPO OPERAZIONE = 12 : Analisi latte
        public const int LAVCOD_ANALISI_LATTE_SINGOLA = 3006;
        public const int LAVCOD_ANALISI_LATTE_MASSA = 3007;

        //GRUPPO OPERAZIONE = 13 : Varie animali
        public const int LAVCOD_NASCITA_ANIMALI = 3000;
        public const int LAVCOD_INCREMENTO_CONSISTENZE_ZOO = 3001;
        public const int LAVCOD_DECREMENTO_CONSISTENZE_ZOO = 3002;
        public const int LAVCOD_MORTE_ANIMALI = 3003;
        public const int LAVCOD_MACELLAZIONE_ANIMALI = 3004;
        public const int LAVCOD_SOSTITUZIONE_MARCA = 3005;
        public const int LAVCOD_PRODUZIONI_LATTE_BOVINO = 3008;
        public const int LAVCOD_PREPARAZIONE_MUNGITURA = 3009;
        public const int LAVCOD_MUNGITURA_SECCHIO_POSTA = 3010;
        public const int LAVCOD_MUNGITURA_GRUPPI_POSTA = 3011;
        public const int LAVCOD_MUNGITURA_SALA_LATTE = 3012;
        public const int LAVCOD_LAVAGGIO_IMPIANTI_MUNGITURA = 3013;
        public const int LAVCOD_LAVAGGIO_SALA_LATTE = 3014;
        public const int LAVCOD_SISTEMAZIONE_PAGLIA_LETTIERE = 3015;
        public const int LAVCOD_RIMOZIONE_DEIEZIONI_MANUALE = 3016;
        public const int LAVCOD_RIMOZIONE_DEIEZIONI_NASTRO_TRASPORTATORE = 3017;
        public const int LAVCOD_SISTEMAZIONE_LETAME_CONCIMAIA = 3018;
        public const int LAVCOD_ASSISTENZA_PARTI = 3025;
        public const int LAVCOD_CONTROLLI_FECONDAZIONI_BOVINE = 3026;
        public const int LAVCOD_VACCINAZIONI_ANIMALI = 3027;
        public const int LAVCOD_CUREMEDICAMENTI_ANIMALI = 3028;
        public const int LAVCOD_ASSISTENZA_VETERINARIO = 3029;
        public const int LAVCOD_RACCOLTA_UOVA = 3031;
        public const int LAVCOD_RACCOLTA_MIELE = 3032;
        public const int LAVCOD_PESATURA_ANIMALI = 3033;
        public const int LAVCOD_ACQUISTO_ANIMALI = 3034;
        public const int LAVCOD_VENDITA_ANIMALI = 3035;
        public const int LAVCOD_TRASFERIMENTO_ANIMALI = 3037;

        public const int LAVCOD_DISTRIBUZIONE_CONCIME = 14;
        public const int LAVCOD_SARCHIATURA_CONCIMAZIONE = 156;
        public const int LAVCOD_DISTRIBUZIONE_AMMENDANTI = 124;
        public const int LAVCOD_CONCIMAZIONE_FOGLIARE = 123;
        public const int LAVCOD_FERTIRRIGAZIONE = 26;
        public const int LAVCOD_TRATTAMENTO_ANTIBUTTERATURA = 106;


        //GRUPPO OPERAZIONE = 17 : Gestione alimentazione
        public const int LAVCOD_ALIMENTAZIONE_PULIZIA_IMPIANTI = 3019;
        public const int LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI = 3020;
        public const int LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI = 3021;
        public const int LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI = 3022;
        public const int LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI = 3023;
        public const int LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024;

        //GRUPPO OPERAZIONE = 18 : Altri lavori di stalla
        public const int LAVCOD_SPOSTAMENTI_ZOO = 3030;
        public const int LAVCOD_ALTRE_LAVORAZIONI_ZOO = 3036;

        //Operazioni Zootecniche
        public const int MIN_OPERAZIONE_ZOO = 3000;
        public const int ANIMALE = 3001;
        public const int ANALISI_LATTE = 3100;
        public const int ALIMENTAZIONE = 3200;
        public const int LETTIERE = 3300;
        public const int MUNGITURA = 3400;
        public const int MACELLAZIONE = 3450;
        public const int RILIEVI_PRODUZIONI = 3500;
        public const int EVENTI = 3550;
        public const int VISUALIZZAZIONE_CONSISTENZE = 3600;
        public const int CARICO_CONSISTENZE = 3700;
        public const int SCARICO_CONSISTENZE = 3750;
        public const int PESATURA_ANIMALI = 3800;
        public const int MAX_OPERAZIONE_ZOO = 3999;
        ////'7300', '7350', '3700', '3750' movimenti
        //public const int LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024;
        //public const int LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024;
        //public const int LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024;
        //public const int LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024;

        //LAV Cod        
        //and a.lav_Cod >= 3000 And a.lav_cod < 4000 "
        public const int LAVCOD_COSTI_CDG = 4500;

        public const int LAVCOD_CURA = 5004;
        public const int LAVCOD_MANUTENZIONE_MACCHINE = 1500;
        public const int LAVCOD_REVISIONE_MACCHINE = 4000;
        public const int LAVCOD_GESTIONE_RIFIUTI = 1080;
        public const int LAVCOD_VISITA = 5007;

    }

    public class CAU_MOV
    {
        //Visite ispettive
        public const string CAU_VISITE_ISPETTIVE = "500";
        public const string CAU_CORPI_ESTRANEI = "501";

        //Profili Utenti
        public const string CAU_PROFILI_UTENTI = "900";

        //Anagrafe
        public const string CAU_IMPRESA = "1050";
        public const string CAU_STRUTTURA = "1100";
        public const string CAU_APPEZZAMENTO = "1200";
        public const string CAU_IMPIANTO = "1300";
        public const string CAU_CATASTO = "1500";
        public const string CAU_STALLA = "1600";

        //Operazioni Colturali
        public const string CAU_TRATTAMENTO = "2050";
        public const string CAU_RILIEVO_CAMPO = "2100";
        public const string CAU_RILIEVO_RACCOLTA = "2200";
        public const string CAU_LAVORAZIONE = "2300";
        public const string CAU_COSTI_ACCESSORI = "2600";

        //Operazioni Zootecniche
        public const string CAU_ANIMALE = "3001";
        public const string CAU_ANALISI_LATTE = "3100";
        public const string CAU_ALIMENTAZIONE = "3200";
        public const string CAU_LETTIERE = "3300";
        public const string CAU_MUNGITURA = "3400";
        public const string CAU_MACELLAZIONE = "3450";
        public const string CAU_RILIEVI_PRODUZIONI = "3500";
        public const string CAU_EVENTI = "3550";
        public const string CAU_VISUALIZZAZIONE_CONSISTENZE = "3600";
        public const string CAU_CARICO_CAPO = "3700";
        public const string CAU_CARICO_CONSISTENZE = "3700";
        public const string CAU_SCARICO_CAPO = "3750";
        public const string CAU_SCARICO_CONSISTENZE = "3750";
        public const string CAU_PESATURA_ANIMALI = "3800";
        public const string CAU_LAVORAZIONE_ZOO = "3850";
        public const string CAU_TRATTAMENTO_ZOO = "3860";

        //Operazioni Contabili
        public const string CAU_REGISTRAZIONI = "4000";
        public const string CAU_REGISTRAZIONI_ALLEGATE = "4050";   // Usata per Accettazione e Contratti Affitto
        public const string CAU_REGISTRAZIONE_SECONDARIA = "4050"; // Usata per Accettazione e Contratti Affitto
        public const string CAU_REGISTRAZIONI_TERZIARIA = "4070";
        public const string CAU_CONFERIMENTO = "4100";
        public const string CAU_CONFERIMENTO_DIVERSI = "4200";
        public const string CAU_COMPENSI = "4400";
        public const string CAU_ABBUONI = "4500";

        //Cartografia
        public const string CAU_CARTOGRAFIA = "5001";

        //Contatti e Contabilità
        public const string CAU_CONTATTO = "6001";
        public const string CAU_RAPPORTO_CONTABILE = "6100";
        public const string CAU_CORRISPETTIVO = "6200";
        public const string CAU_MOVIMENTO_CONTABILE = "6300";
        public const string CAU_MOVIMENTO_NON_CONTABILE = "6400";
        public const string CAU_STATISTICHE = "6500";
        public const string CAU_CLIENTE = "6600";
        public const string CAU_FORNITORE = "6610";
        public const string CAU_DIPENDENTE = "6620";
        public const string CAU_TERZISTA = "6630";
        public const string CAU_LEGALE = "6640";
        public const string CAU_IMPUTAZIONE_MANODOPERA = "6800";           // Utilizzo di manodopera
        public const string CAU_IMPUTAZIONE_TERZISTI = "6850";             // Utilizzo dei Terzi
        public const string CAU_IMPUTAZIONE_TECNICO_RESPONSABILE = "6851"; // Utilizzo Tecnico Responsabile
        public const string CAU_ASSEGNATARIO_VISITA = "6852";              // Operatore (tecnico o capoarea a cui è stata assegnata la visita da effettuare)

        //Magazzini
        public const string CAU_MAGAZZINO = "7001";
        public const string CAU_VISUALIZZAZIONE_GIACENZE = "7100";
        public const string CAU_VISUALIZZAZIONE_INVESTIMENTO = "7200";
        public const string CAU_CARICO = "7300";
        public const string CAU_SCARICO = "7350";
        public const string CAU_TRASFERIMENTO = "7380";
        public const string CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI = "7400";
        public const string CAU_PRODOTTI_AZIENDALI = "7800";
        public const string CAU_ACCETTAZIONE_BENI = "7900";
        public const string CAU_ACCETTAZIONE_BENI_DA_DIVERSI_PRE = "7950";
        public const string CAU_ACCETTAZIONE_BENI_DA_DIVERSI_POST = "7951";
        public const string CAU_ACCETTAZIONE_BENI_DA_DIVERSI = "7920";

        
        //Parco Macchine
        public const string CAU_ANAGRAFE_PARCOMACCHINE = "8001";
        public const string CAU_MANUTENZIONE_PARCOMACCHINE = "8200"; // Manutenzione del Parco Macchine
        public const string CAU_IMPUTAZIONE_PARCOMACCHINE = "8100"; //Utilizzo del Parco Macchine

        //Progetti
        public const string CAU_PROGETTO = "9001";
        public const string CAU_PROGETTO_PRODUZIONE = "9100";
        public const string CAU_PROGETTO_ZOOTECNICO = "9150";
        public const string CAU_PROGETTO_TECNICO = "9200";
        public const string CAU_CONTRATTO_COLTURALE = "9300";
        public const string CAU_ORDINE = "9320";
        public const string CAU_PROGRAMMA_PRODUZIONE = "9325";
        public const string CAU_CENTROCOSTO = "9350";
        public const string CAU_IMPUTAZIONE_COSTISTANDARD = "9400";

        //Linee Produzione
        public const string CAU_LINEA_PRODUZIONE = "10001";
        public const string CAU_LINEA_VEGETALE = "10100";
        public const string CAU_LINEA_ANIMALE = "10200";

        //Cantine
        public const string CAU_IMBOTTIBLIAMENTO = "10500";
    }

    public class CATEGORIE_MAGAZZINO
    {
        public const int RIGA_DESCRIZIONE_LIBERA = 502;
        public const string RIGA_DESCRIZIONE_LIBERA_DES = "Riga Descrizione Libera";
        public const int ALTRI_BENI_AMMORTIZZABILI = 500;
        public const int ALTRI_BENI = 501;
        public const string ALTRI_BENI_DES = "Altri Beni Strumentali";
        public const int SERVIZI = 555;
        public const string SERVIZI_DES = "Servizi";

        public const int CORPI_ESTRANEI = -50;
        public const int CALI_LAVORAZIONE = -1;

        public const int ELEMCOD_MANODOPERA = 0;
        public const int MACCHINE = 1;
        public const int CARBURANTI = 2;

        public const int RIFIUTI = 4;

        public const int FERTILIZZANTI = 3;
        public const int FORMULATI = 191;
        public const int COADIUVANTI = 195;
        public const int INSETTI = 196;
        public const int TRAPPOLE = 197;
        public const int INNESCHI = 198;

        public const int SEMENTI = 10;
        public const int ALTRE_MATERIE = 200;
        public const int ZOO_CONSISTENZA = 300;

        public const int SEMILAVORATI_VEGETALI = 201;
        public const int MATERIE_VEGETALI = 204;
        public const int BENI_CONFEZ_VEGETALE = 205;
        public const int TRASFORMATI_VEGETALI = 210;

        public const int SEMILAVORATI_ANIMALI = 301;
        public const int MATERIE_ANIMALI = 304;
        public const int BENI_CONFEZ_ANIMALE = 305;
        public const int TRASFORMATI_ANIMALI = 310;

        public const int MANGIMI = 306;
        public const int FARMACI = 307;

        public const int CONFEZIONI_PRODOTTI = 400;
        public const int RICAMBI = 401;

        public const int RIGA_DESCRIZIONE = 502;

        public const int CAT_MAG_SERVIZI_PROFESSIONALI = 700;
    }

    public class ELEM_COD
    {
        public const int ELEMCOD_MANODOPERA = 0;
        public const int MACCHINE = 1;
        public const int CARBURANTI = 2;

        public const int RIFIUTI = 4;

        public const int FERTILIZZANTI = 3;
        public const int FORMULATI = 191;
        public const int COADIUVANTI = 195;
        public const int INSETTI = 196;
        public const int TRAPPOLE = 197;
        public const int INNESCHI = 198;

        public const int SEMENTI = 10;
        public const int ALTRE_MATERIE = 200;
        public const int ZOO_CONSISTENZA = 300;

        public const int SEMILAVORATI_VEGETALI = 201;
        public const int MATERIE_VEGETALI = 204;
        public const int BENI_CONFEZ_VEGETALE = 205;
        public const int TRASFORMATI_VEGETALI = 210;

        public const int SEMILAVORATI_ANIMALI = 301;
        public const int MATERIE_ANIMALI = 304;
        public const int BENI_CONFEZ_ANIMALE = 305;
        public const int TRASFORMATI_ANIMALI = 310;

        public const int MANGIMI = 306;
        public const int FARMACI = 307;

        public const int CONFEZIONI_PRODOTTI = 400;
        public const int RICAMBI = 401;

        public const int RIGA_DESCRIZIONE = 502;

        public const int CAT_MAG_SERVIZI_PROFESSIONALI = 700;

    }

    public class ENTITA_VISIBILITA
    {
        public const int IMPRESA = 1;
        public const int CENTRO = 2;
    }

    public class CONTATTI_VISIBILITA
    {
        public const int SACOD_NOFILTRO = 1;
        public const int PRIVATO = 0;
        public const int PUBBLICO = -1;
    }


    public class TIPO_DESTINAZIONE
    {
        public const int TIPO_DESTINAZIONE_IMPIANTO = 0;
        public const int TIPO_DESTINAZIONE_MAGAZZINO = 20;
        public const int TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA = 21;
        public const int STALLA = 15;
        public const int TIPO_DESTINAZIONE_ANIMALE = 1;
    }

    public class CounterBaseTopCode
    {
        public const int BaseCode = 0;
        public const int TopCode = 20000000;
    }

    public class COSTANTI_GENERALI
    {
        public const string AGRODATAINIZIO = "01-01-1900";
        public const string LOTTO_NONDEFINITO = "-999";
        public const int CODPROGETTO_NONDEFINITO = -999;
        public const int SACOD_CONTATTO_NONDEFINITO = -999;
        public const int MAGAZZINO_MOVIMENTATO = 0;
    }

    public class DETT_TEC
    {
        public const int SOSPENSIONE_CARNE = 1;
        public const int SOSPENSIONE_LATTE = 2;
    }
    public class IMPRESE_CODICI
    {
        public const int CUAA       = 1010;
        public const int LATITUDINE = 1105;
        public const int LONGITUDINE = 1106;
    }

    public class REG_IMPIANTI_CODICI
    {
        public const int ANNO_IMPIANTO = 1362;
    }

    public class ID_SERVIZIO
    {
        public const int GIASONLINE_SERVICE_ID = 5;
    }

    public class TipoOperazioneStr
    {
        public const string Insert = "INS";
        public const string Update = "UPD";
        public const string Delete = "DEL";
    }

    public class MAT_COD
    {
        public const int MAT_COD_ACQUA_IRRIGAZIONE = -1;
    }

    /// <summary>
    /// Parametri del Campo 'Contabilizzato' nella tabella 'Movimenti_Dettagli'
    /// CONTABILIZZATO POSITIVO -> IL COM+ GESTISCE LE GIACENZE
    /// </summary>
    public class DETTAGLI_CONTABILI
    {
        public const int NON_CONTABILE = 1;
        public const int CONTABILE = 2;
        public const int CONTABILE_EVASO_FORZATAMENTE = 3;
        public const int CONTABILE_LOTTO_INDISPONIBILE = 4;
        public const int CONTABILE_PREPARAZIONE_AUTOMATICA_FIFO = 5;
        public const int CONTABILE_PREPARAZIONE_AUTOMATICA_DISTINTA = 6;
        public const int CONTABILE_PREPARAZIONE_AUTOMATICA_LOTTO_GIORNATA = 7;
    }

    public class EsitoStr
    {
        public const string OK = "OK";
        public const string KO = "KO";
    }

    public class TipoAgronicaLogAnagrafe
    {
        public const string Imprese = "Imprese";
        public const string CentriAziendali = "Centri_Aziendali";
        public const string Appezza = "Appezzamento";
        public const string Impianti = "Reg_Impianti";
        public const string Progetti = "Imprese_Progetti";
        public const string Fabbricati = "Fabbricati";
        public const string Campi = "Campi";
        public const string CampiCodici = "Campi_Codici";
        public const string Indirizzi = "Indirizzi";
        public const string Squadre = "Squadre";
        public const string UtentiXAppezzamenti = "UtentiXAppezzamenti";
        public const string AppezzamentiXParticelle = "AppezzamentiXParticelle";
        public const string AppezzamentiXParticelleXMacrousi = "AppezzamentiXParticellexMacrousi";
        public const string AppezzamentiXParticelleXMacrousiXUtilizzo = "AppezzamentiXParticellexMacrousixUtilizzo";
        public const string ZooAnimali = "Zoo_Animali";
        public const string ZooAnimaliDistinte = "Zoo_Animali_Distinte";
        public const string ParcoMacchine = "Parco_Macchine";
        public const string ProdottiCosti = "Prodotti_Costi";
        public const string ParticelleCatastali = "ParticelleCatastali";
        public const string ImpreseXParticelle = "ImpreseXParticelle";
    }

public class TabellaLog
    {
        public const string AgronicaLogAnagrafe = "Agronica_Log_Anagrafe";
        public const string AgronicaLogContatti = "Agronica_Log_Contatti";
    }
    
public class TIPO_FABBRICATI
    {
        public const int TIPO_FABBRICATO_MAGAZZINO = 20;
        public const int TIPO_FABBRICATO_STALLA = 178;
    }

    public struct OrigineApp
    {
        public const string GiasApp = "APP";
        public const string Demetra = "DEMETRA";
        public const string PUA = "PUA";
    }
}
