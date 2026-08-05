Imports System.Drawing
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO

Public Class CostantiPersonalizzate

    Public Const OPERAZIONI_GESTITE_APP_DEMETRA As String = "1,162,18,74,103,155,14,26,123,8,9,120,10,78,12,17,19,21,81,22,23,25,27,31,32,33,34,82,83,84,40,41,85,47,48,87,88,49,53,75,2,71,125,168,172,173"

    Public Const OPERAZIONI_GESTITE_APP As String = "162,18,74,103,155,13,158,14,26,106,123,124,156,8,9,120,10,78,12,17,19,21,157,81,22,23,25,27,31,32,152,33,34,161,154,82,83,84,40,41,85,46,47,48,87,88,49,90,53,55,115,56,57,153,58,59,62,63,68,75,76,77,2,160,71,125,168,172,116,173,170"

    Public Shared OPERAZIONI_GESTITE_APP_DEMETRA_LIST As List(Of Integer) = New List(Of Integer) From {
        1, 162, 18, 74, 103, 155, 14, 26, 123, 8, 9, 120, 10, 78, 12, 17, 19, 21, 81, 22, 23, 25, 27, 31, 32, 33, 34, 82, 83, 84, 40, 41, 85, 47, 48, 87, 88, 49, 53, 75, 2, 71, 125, 168, 172, 173
    }

    Public Shared OPERAZIONI_GESTITE_APP_LIST As List(Of Integer) = New List(Of Integer) From {
        162, 18, 74, 103, 155, 13, 158, 14, 26, 106, 123, 124, 156, 8, 9, 120, 10, 78, 12, 17, 19, 21, 157, 81, 22, 23, 25, 27, 31, 32, 152, 33, 34, 161, 154, 82, 83, 84, 40, 41, 85, 46, 47, 48, 87, 88, 49, 90, 53, 55, 115, 56, 57, 153, 58, 59, 62, 63, 68, 75, 76, 77, 2, 160, 71, 125, 168, 172, 116, 173, 170
    }

    'Filtro per Operazioni non gestite Agenda
    '#########################################################################
    'lavcod da 1000 a 1070:
    'gruppo 10 e 6 tipo E lascio solo le operazioni per ddt e fatture e magazzino: 1022,1023,1020,1021,1033 per magazzino, 1000,1001,1025,1031 per ddt fatture
    'nascondo momentaneamente vendita e acquisto 1020, 1021

    ''' <summary>
    ''' Costante valida sia nell'AgronicaAgenda (Versione BS) che su NG
    ''' </summary>
    Public Const STR_OP_NON_GESTITE As String = " (  GruppoOperazioni.Tipo IN ('C','Z','P','E','V') AND GruppoOperazioni.Gru_Cod not in ( 5,7,8,9,11 ) AND Operazioni.Lav_Cod NOT IN (1021,1004,1005,1006,1007,1026,1027,1028,1029,1030,1032,1050,1051,1052,1053,1054,1055,1056,1057,1058,1060,1061,1062,1063,1064,1065,1066,1067,1068,1069,1070,1071,1072,1073,1074,1075,1076,1077,1078,30,3005,3006,3007,3008,3009,3010,3011,3012,3013,3014,3015,3016,3017,3018,3019,3024,3025,3026,3027,3028,3029,3031,3032,3022,3021,30233,5004) ) "

    ''' <summary>
    ''' Costante valida sia nell'AgronicaAgenda (Versione BS) che su NG
    ''' </summary>
    Public Const STR_OP_RICETTABILI As String = "162,18,74,103,155,13,158,14,26,106,123,124,156,8,171,9,120,10,78,12,17,19,21,157,81,22,23,25,27,31,32,152,33,34,161,154,82,83,84,40,41,85,46,47,48,87,88,49,90,53,55,115,56,57,153,58,59,62,63,75,76,77,2,71,160,68,168,125,1"

    ''' <summary>
    ''' Costante che indica quali operazioni non possono essere ricettabili nell'AgronicaAgenda (Versione BS) ma che sono gestite solo su NG
    ''' </summary>
    Public Const STR_OP_RICETTABILI_NG As String = "116,172,173,167,170,163,13,174,175,122,107"

    ''' <summary>
    ''' LAVCOD_FASI_FENOLOGICHE,
    ''' LAVCOD_RILIEVO_ERBE_INFESTANTI,
    ''' LAVCOD_RILIEVO_AVVERSITA_CAMPO,
    ''' LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE,
    ''' LAVCOD_RILIEVO_INDICI_MATURITA,
    ''' LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA
    ''' LAVCOD_DANNI_RACCOLTA
    ''' </summary>
    Public Const STR_OP_COLLEGABILI_A_VISITE_NG As String = "79,119,113,110,109,169,108"

    ''' <summary>
    '''  LAVCOD_FASI_FENOLOGICHE, LAVCOD_RILIEVO_ERBE_INFESTANTI LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE, LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA, LAVCOD_DANNI_RACCOLTA, LAVCOD_RILIEVO_PIOGGE
    ''' </summary>
    Public Const STR_OP_RILIEVI As String = "79,119,113,110,109,169,108,126"

    ''' <summary>
    ''' Costante che indica quali operazioni non si possono creare su NG ma che si possono creare solo su nell'AgronicaAgenda (Versione BS)
    ''' <example>
    ''' Escludo su NG le Operazioni Confusione Sessuale (lav_cod 118) e Disorientamento Sessuale (lav_cod 121) perchè si potranno creare solo nuove
    ''' Operazioni di tipo Confusione / Disorientamento Sessuale (lav_cod 172)
    ''' TODO Da ecludere poi anche i lav_Cod 107,122
    ''' </example>
    ''' </summary>
    Public Const STR_OP_NON_GESTITE_NG As String = "118,121,107,122"

    ''' <summary>
    ''' Costante che indica quali operazioni non si possono creare nell'AgronicaAgenda (Versione BS) ma che si possono creare solo su NG
    ''' </summary>
    Public Const STR_OP_NON_GESTITE_BS As String = " Operazioni.Lav_Cod NOT IN (172,173)"

    ''' <summary>
    ''' Costante che indica quali operazioni si possono creare su NG ma che non sono gestite sull'APP
    ''' </summary>
    Public Const STR_OP_NON_GESTITE_APP As String = "173,116"

    ''LAVCOD_SEMINA & "," & LAVCOD_SOVESCIO & "," & LAVCOD_TRAPIANTO & "," & LAVCOD_RILIEVO_AVVERSITA_CAMPO & ")"
    'Public STR_OP_RICETTABILI As String = LAVCOD_ALTRE_OPERAZIONI & "," & LAVCOD_DISERBO & "," & LAVCOD_TRATTAMENTO_ANTIPARASSITARIO & "," & LAVCOD_TRATTAMENTO_FITOREGOLATORE & "," & LAVCOD_GEODISINFESTAZIONE & "," & LAVCOD_CONCIA_SEME & "," & LAVCOD_DISSECCAMENTO & "," & LAVCOD_DISTRIBUZIONE_CONCIME & "," & LAVCOD_FERTIRRIGAZIONE & "," & LAVCOD_TRATTAMENTO_ANTIBUTTERATURA & "," & LAVCOD_CONCIMAZIONE_FOGLIARE & "," & LAVCOD_DISTRIBUZIONE_AMMENDANTI & "," & LAVCOD_SARCHIATURA_CONCIMAZIONE & "," & LAVCOD_ARATURA & "," & LAVCOD_ANDANAMENTO & "," & LAVCOD_ASPORTAZIONE_ORGANI_INFETTI & "," & LAVCOD_ASSOLCATURA & "," & LAVCOD_CARICO_MANUALE_FRUTTA & "," & LAVCOD_CIMATURA & "," & LAVCOD_DIRADAMENTO_MANUALE & "," & LAVCOD_DISSODAMENTO & "," & LAVCOD_ERPICATURA & "," & LAVCOD_ERPICATURA_ROTANTE & "," & LAVCOD_ESPIANTO & "," & LAVCOD_ESTIRPATURA & "," & LAVCOD_FALCIACONDIZIONATURA & "," & LAVCOD_FALCIATURA_ERBAI & "," & LAVCOD_FORMAZIONE_ARGINELLI & "," & LAVCOD_FRANGIZOLLATURA & "," & LAVCOD_FRESATURA & "," & LAVCOD_GEBIATURA & "," & LAVCOD_IMBALLO_FIENO_ROTOLI & "," & LAVCOD_INTERRAMENTO_PAGLIE & "," & LAVCOD_INTERVENTO_ANTIBRINA & "," & LAVCOD_LAVORAZIONE_CONBINATA & "," & LAVCOD_LAVORAZIONE_TRA_FILA & "," & LAVCOD_LAVORAZIONE_SU_FILA & "," & LAVCOD_LEGATURA & "," & LAVCOD_LIVELLAMENTO & "," & LAVCOD_MANUTENZIONE_ARGINI & "," & LAVCOD_MESSA_DIMORA_PIANTE & "," & LAVCOD_MIETITREBBIATURA & "," & LAVCOD_MINIMUM_TILLAGE & "," & LAVCOD_PACCIAMATURA & "," & LAVCOD_POTATURA_SECCA & "," & LAVCOD_POTATURA_VERDE & "," & LAVCOD_PRESSATURA & "," & LAVCOD__RACCOLTA_LEGNA_POTATURA & "," & LAVCOD_RANGHINATURA & "," & LAVCOD_RINCALZATURA & "," & LAVCOD_RIPPATURA & "," & LAVCOD_RIPUNTATURA & "," & LAVCOD_RIVOLTAMENTO_FORAGGIO & "," & LAVCOD_ROMPICROSTA & "," & LAVCOD_RULLATURA & "," & LAVCOD_SARCHIATURA & "," & LAVCOD_SCARIFICATURA & "," & LAVCOD_SCASSO & "," & LAVCOD_TRINCIATURA & "," & LAVCOD_VANGATURA & "," & LAVCOD_ZAPPATURA

    '#########################################################################
    Public Const PathFileINI As String = "c:\Agroconnessioni\Connessioni.ini"
    'I valori si trovano anche in TipiEnumerativi.enum_Id_Servizio
    Public Const Id_Servizio_Agronica As Integer = 1
    Public Const Id_Servizio_GiasPcGiasPro As Integer = 2
    Public Const Id_Servizio_NetFruit As Integer = 3
    Public Const Id_Servizio_SitoAgronica_2003 As Integer = 4
    Public Const Id_Servizio_GiasOnline As Integer = 5
    Public Const Id_Servizio_ManutenzioneTabelle As Integer = 6
    Public Const Id_Servizio_GiasOnlineCodex As Integer = 7
    Public Const Id_Servizio_GiasLAN As Integer = 8
    Public Const Id_Servizio_SitoAgronicaOld As Integer = 9
    Public Const Id_Servizio_AgronicaSementi As Integer = 11
    Public Const Id_Servizio_Profitosan As Integer = 20
    Public Const Id_Servizio_ProfitosanTunnel As Integer = 21
    Public Const Id_Servizio_BancheDatiEsterni As Integer = 30
    Public Const Id_Servizio_PianoConcimazione As Integer = 31
    Public Const Id_Servizio_AgroGSB As Integer = 40

    '#####################################################################
    Public Const AgroKey_EncoderDecoder As String = "cobaltoleccioplutone"
    Public Const AgroKey_EncoderDecoder_small As String = "coba"
    Public Const AgroKeyTunnel_EncoderDecoder As String = "costovanne"

    '#####################################################################
    'Generazione di BaseCode e TopCode 
    Public Const AgroCode_BitPerCodice As Integer = 17

    '#####################################################################
    'FRUTTAGEL: costanti 
    Public Const FRUTTAGEL_INDMATCOD_PUNTEGGIO As Integer = 999
    Public Const FRUTTAGEL_INDMATCOD_GRADOTEND As Integer = 12
    Public Const FRUTTAGEL_INDMATCOD_GRADOBRIX As Integer = 1
    Public Const FRUTTAGEL_COD_CLIENTE_FORNITORE As Integer = -11

    '#####################################################################
    'Definizione dei colori
#Region "Colori"

    'Bianco             #ffffff     255,255,255
    Public Shared AgroColor_Bianco As Color = Color.White

    'Nero               #000000     000,000,000
    Public Shared AgroColor_Nero As Color = Color.Black

    'Rosso
    Public Shared AgroColor_Rosso As Color = Color.Red

    'Grigio Chiaro      #e0e0e0     224,224,224
    Public Shared AgroColor_GrigioChiaro As Color = Color.FromArgb(224, 224, 224)

    'Blu Scuro          #00bfff     000,191,255
    Public Shared AgroColor_BluScuro As Color = Color.FromArgb(0, 191, 255)

    'Blu Medio          #30dfef     048,223,239
    Public Shared AgroColor_BluMedio As Color = Color.FromArgb(48, 223, 239)

    'Blu Chiaro         #afeeee     175,238,238
    'Public Shared AgroColor_BluChiaro As Color = Color.FromArgb(175, 238, 238)
    Public Shared AgroColor_BluChiaro As Color = Color.FromArgb(0, 191, 255)

    'Blu Chiaro2        #c0ffff     192,255,255
    Public Shared AgroColor_BluChiaro2 As Color = Color.FromArgb(192, 255, 255)

    'Verde Chiaro       #c0ffc0     192,255,192
    Public Shared AgroColor_VerdeChiaro As Color = Color.FromArgb(192, 255, 192)

    'Rosso Chiaro       #FFC0C0     255,192,192
    Public Shared AgroColor_RossoChiaro As Color = Color.FromArgb(255, 192, 192)

    'Giallo Chiaro      #FFFFC0     255,255,192
    Public Shared AgroColor_GialloChiaro As Color = Color.FromArgb(255, 255, 192)
#End Region

    '#####################################################################
    'Estremi delle date di validita
#Region "Validita"

    Public Shared Estremo_Validita_Inizio As Date = #1/1/1900#
    Public Shared Estremo_Validita_Fine As Date = #12/31/2100#

    Public Shared FinestraTemporale_Inizio As Date = #1/1/1900#
    Public Shared FinestraTemporale_Fine As Date = #12/31/2100#

    Public Const AGRODATAINIZIO As Date = #1/1/1900#
    Public Const AGRODATAFINE As Date = #12/31/2100#

    Public Const AgroDataInizializzata As Date = #2/1/1900#

    'Public Const STR_DATAINIZIO As String = "01/01/1900"
    'Public Const STR_DATAFINE As String = "2100-12-31"

    'Valori di Default
    Public Shared DataDefault_per_MODIFICA As Date = #1/1/1800#
    Public Shared StrDefault_per_MODIFICA As String = "-999"
    Public Shared IntDefault_per_MODIFICA As Integer = -999
    Public Shared DoubleDefault_per_MODIFICA As Decimal = -999
#End Region

    '#####################################################################
    'Etichette per gli elementi dell'albero delle imprese
#Region "Etichette Albero Imprese"

    'Utente
    Public Const AgroLabel_Utente As String = "Utente"

    'Impresa
    Public Const AgroLabel_Impresa As String = "Impresa"

    'Centro Aziendale
    Public Const AgroLabel_CentroAziendale As String = "Centro Aziendale"

    'Campo
    Public Const AgroLabel_Campo As String = "Campo"

    'Serra
    Public Const AgroLabel_Serra As String = "Serra"

    'Appezzamento
    Public Const AgroLabel_Appezzamento As String = "Appezzamento"

    'Impianto
    Public Const AgroLabel_Impianto As String = "Impianto Colturale"

    'Particella Catastale
    Public Const AgroLabel_ParticellaCatastale As String = "Particella Catastale"

    'Fabbricato    (Edifici, Silos, Magazzini, Celle Frigorifere)
    Public Const AgroLabel_Fabbricato As String = "Fabbricato"

    'Persona
    Public Const AgroLabel_Persona As String = "Persona"

    'Particella Catastale
    Public Const AgroLabel_Particella As String = "Particella Catastale"

#End Region

    '#####################################################################
    'Prefissi per gli elementi dell'albero delle imprese
#Region "Prefissi Albero Imprese"

    'Utente
    Public Const AgroPrefix_Utente As String = "{Utente} : "

    'Impresa
    Public Const AgroPrefix_Impresa As String = "{Impresa} : "

    'Centro Aziendale
    Public Const AgroPrefix_CentroAziendale As String = "{Centro} : "

    'Campo
    Public Const AgroPrefix_Campo As String = "{Campo} : "

    'Serra
    Public Const AgroPrefix_Serra As String = "{Serra} : "

    'Appezzamento
    Public Const AgroPrefix_Appezzamento As String = "{App} : "
    'Planning
    Public Const AgroPrefix_Planning As String = "{Plan} : "
    Public Const AgroPrefix_OperazioniAgenda As String = "{Op} : "
    Public Const AgroPrefix_DettaglioPlanning As String = "{Det} : "

    'Impianto
    Public Const AgroPrefix_Impianto As String = ""

    'Particella Catastale
    Public Const AgroPrefix_ParticellaCatastale As String = ""

    'Fabbricato    (Edifici, Silos, Magazzini, Celle Frigorifere)
    Public Const AgroPrefix_Fabbricato As String = ""

    'Persona
    Public Const AgroPrefix_Persona As String = ""

    'Particella Catastale
    Public Const AgroPrefix_Particella As String = ""
#End Region

    '#####################################################################
    'Immagini che cambiano a seconda del sito chiamato
#Region "Immagini Sito"

    'Logo 96x73
    Public Const AgroImg_Logo_96x73 As String = "../AB_Immagini/Logo/Logo_GiasOnline_Mini.jpg"

    'Logo 174x135
    Public Const AgroImg_Logo_174x135 As String = "../AB_Immagini/Logo/Logo_GiasOnline.jpg"

    'Logo 271x209
    Public Const AgroImg_Logo_271x209 As String = "../AB_Immagini/Logo/Logo_GiasOnline_Grande.jpg"
#End Region

    '#################################
    '##### CAUSALI/PERMESSI  #########
    '#################################
#Region "Casuali/Permessi"

    'Visite ispettive
    Public Const CAU_VISITE_ISPETTIVE As String = "500"
    Public Const CAU_CORPI_ESTRANEI As String = "501"

    'Profili Utenti
    Public Const CAU_PROFILI_UTENTI As String = "900"

    'Anagrafe
    Public Const CAU_IMPRESA As String = "1050"
    Public Const CAU_STRUTTURA As String = "1100"
    Public Const CAU_APPEZZAMENTO As String = "1200"
    Public Const CAU_IMPIANTO As String = "1300"
    Public Const CAU_CATASTO As String = "1500"
    Public Const CAU_STALLA As String = "1600"

    'Operazioni Colturali
    Public Const CAU_TRATTAMENTO As String = "2050"
    Public Const CAU_RILIEVO_CAMPO As String = "2100"
    Public Const CAU_RILIEVO_RACCOLTA As String = "2200"
    Public Const CAU_LAVORAZIONE As String = "2300"
    Public Const CAU_COSTI_ACCESSORI As String = "2600"

    'Operazioni Zootecniche
    Public Const CAU_ANIMALE As String = "3001"
    Public Const CAU_ANALISI_LATTE As String = "3100"
    Public Const CAU_ALIMENTAZIONE As String = "3200"
    Public Const CAU_LETTIERE As String = "3300"
    Public Const CAU_MUNGITURA As String = "3400"
    Public Const CAU_MACELLAZIONE As String = "3450"
    Public Const CAU_RILIEVI_PRODUZIONI As String = "3500"
    Public Const CAU_EVENTI As String = "3550"
    Public Const CAU_VISUALIZZAZIONE_CONSISTENZE As String = "3600"
    Public Const CAU_CARICO_CONSISTENZE As String = "3700"
    Public Const CAU_SCARICO_CONSISTENZE As String = "3750"
    Public Const CAU_PESATURA_ANIMALI As String = "3800"
    Public Const CAU_LAVORAZIONE_ZOO As String = "3850"
    Public Const CAU_TRATTAMENTO_ZOO As String = "3860"

    'Operazioni Contabili
    Public Const CAU_REGISTRAZIONI As String = "4000"
    '----- Usate per Accettazione e Contratti Affitto
    Public Const CAU_REGISTRAZIONI_ALLEGATE As String = "4050"
    Public Const CAU_REGISTRAZIONE_SECONDARIA As String = "4050"
    '-----
    Public Const CAU_REGISTRAZIONI_TERZIARIA As String = "4070"
    Public Const CAU_CONFERIMENTO As String = "4100"
    Public Const CAU_CONFERIMENTO_DIVERSI As String = "4200"
    Public Const CAU_COMPENSI As String = "4400"
    Public Const CAU_ABBUONI As String = "4500"

    'Cartografia
    Public Const CAU_CARTOGRAFIA As String = "5001"

    'contatti e Contabilità
    Public Const CAU_CONTATTO As String = "6001"
    Public Const CAU_RAPPORTO_CONTABILE As String = "6100"
    Public Const CAU_CORRISPETTIVO As String = "6200"
    Public Const CAU_MOVIMENTO_CONTABILE As String = "6300"
    Public Const CAU_MOVIMENTO_NON_CONTABILE As String = "6400"
    Public Const CAU_STATISTICHE As String = "6500"
    Public Const CAU_CLIENTE As String = "6600"
    Public Const CAU_FORNITORE As String = "6610"
    Public Const CAU_DIPENDENTE As String = "6620"
    Public Const CAU_TERZISTA As String = "6630"
    Public Const CAU_LEGALE As String = "6640"
    Public Const CAU_IMPUTAZIONE_MANODOPERA As String = "6800" 'Utilizzo di manodopera
    Public Const CAU_IMPUTAZIONE_TERZISTI As String = "6850" 'Utilizzo dei Terzi
    Public Const CAU_IMPUTAZIONE_TECNICO_RESPONSABILE As String = "6851" 'Utilizzo Tecnico Responsabile
    Public Const CAU_ASSEGNATARIO_VISITA As String = "6852" 'Operatore (tecnico o capoarea a cui è stata assegnata la visita da effettuare)

    'Magazzini
    Public Const CAU_MAGAZZINO As String = "7001"
    Public Const CAU_VISUALIZZAZIONE_GIACENZE As String = "7100"
    Public Const CAU_VISUALIZZAZIONE_INVESTIMENTO As String = "7200"
    Public Const CAU_CARICO As String = "7300"
    Public Const CAU_SCARICO As String = "7350"
    Public Const CAU_TRASFERIMENTO As String = "7380"
    Public Const CAU_IMPUTAZIONE_UTILIZZO_PRODOTTI As String = "7400"
    Public Const CAU_PRODOTTI_AZIENDALI As String = "7800"
    Public Const CAU_ACCETTAZIONE_BENI As String = "7900"
    Public Const CAU_ACCETTAZIONE_BENI_DA_DIVERSI_PRE As String = "7950"
    Public Const CAU_ACCETTAZIONE_BENI_DA_DIVERSI_POST As String = "7951"
    Public Const CAU_ACCETTAZIONE_BENI_DA_DIVERSI As String = "7920"

    'Parco Macchine
    Public Const CAU_ANAGRAFE_PARCOMACCHINE As String = "8001"
    Public Const CAU_IMPUTAZIONE_PARCOMACCHINE As String = "8100" 'Utilizzo del Parco Macchine
    Public Const CAU_MANUTENZIONE_PARCOMACCHINE As String = "8200" 'Manutenzione del Parco Macchine

    'Progetti
    Public Const CAU_PROGETTO As String = "9001"
    Public Const CAU_PROGETTO_PRODUZIONE As String = "9100"
    Public Const CAU_PROGETTO_ZOOTECNICO As String = "9150"
    Public Const CAU_PROGETTO_TECNICO As String = "9200"
    Public Const CAU_CONTRATTO_COLTURALE As String = "9300"
    Public Const CAU_ORDINE As String = "9320"
    Public Const CAU_PROGRAMMA_PRODUZIONE As String = "9325"
    Public Const CAU_CENTROCOSTO As String = "9350"
    Public Const CAU_IMPUTAZIONE_COSTISTANDARD As String = "9400"

    'Linee Produzione
    Public Const CAU_LINEA_PRODUZIONE As String = "10001"
    Public Const CAU_LINEA_VEGETALE As String = "10100"
    Public Const CAU_LINEA_ANIMALE As String = "10200"

    'Cantine
    Public Const CAU_IMBOTTIBLIAMENTO As String = "10500"

#End Region

    '=============================================================

    'TIPO INDIRIZZO DEI CONTATTI

    'Persona fisica
    Public Const INDIRIZZO_RESIDENZA As Integer = 3
    Public Const INDIRIZZO_LUOGO_NASCITA As Integer = 5
    Public Const INDIRIZZO_DOMICILIO As Integer = 2
    Public Const INDIRIZZO_RESIDENZA_ESTIVA As Integer = 4

    'Persona giuridica
    Public Const INDIRIZZO_SEDE_OPERATIVA As Integer = 1 'E' L'UNICO TIPO INDIRIZZO DELL'IMPRESA
    Public Const INDIRIZZO_SEDE_LEGALE As Integer = 101
    Public Const INDIRIZZO_SEDE_AZIENDALE As Integer = 102
    Public Const INDIRIZZO_STABILIMENTO As Integer = 103
    Public Const INDIRIZZO_STABILE_ORGANIZZAZIONE As Integer = 201 'indirizzo italiano per contatto estero

    'Valori default per i tipi di indirizzo
    Public Const INDIRIZZO_DEFAULT_CAP As String = "00000"
    Public Const INDIRIZZO_DEFAULT_COMUNE As String = "000"
    Public Const INDIRIZZO_DEFAULT_PROVINCIA As String = "000"
    Public Const INDIRIZZO_DEFAULT_REGIONE As String = "000"

    '=============================================================

    'TIPO IMPRESA GERARCHIA
    Public Const TIPO_IMPRESA As Integer = 1
    Public Const TIPO_COOP As Integer = 2
    Public Const TIPO_CONSORZIO As Integer = 3
    Public Const TIPO_OP As Integer = 4

    '=============================================================

    'TIPO PERSONA
    Public Const PERSONA_FISICA As Integer = 0
    Public Const PERSONA_GIURIDICA As Integer = 1
    Public Const CONTATTO_ESTERO As Integer = 2

    '=============================================================

    'VISIBILITA'
    Public Const SACOD_NOFILTRO As Integer = -99
    Public Const PRIVATO As Integer = 0
    Public Const PUBBLICO As Integer = -1

    Public Const ID_CF_NOFILTRO As Integer = -99

    '=============================================================

    Public Const CODLIQUIDITA_NOFILTRO As Integer = -1
    Public Const CODISTITUTO_NOFILTRO As Integer = -1

    '=============================================================

    'CODICI ANAGRAFE

    'IMPRESA
    Public Const CA_CUAA As Integer = 1010
    Public Const CA_COD_SOCIO As Integer = 1033
    Public Const CA_TITOLO_POSSESSO As Integer = 1016
    Public Const CA_TECNICO_RIF As Integer = 1088
    Public Const CA_COD_CLIENTE As Integer = 1089
    Public Const CA_COD_FORNITORE As Integer = 1090
    Public Const CA_COD_FORNITORE_2 As Integer = 1091
    Public Const CA_COD_LIBRO_SOCI As Integer = 1086
    Public Const CA_DATA_ISCRIZIONE_LIBRO_SOCI As Integer = 1087
    Public Const CA_PIVA_SUPERUSER_ORIGINE As Integer = 1107

    '=============================================================

    'FABBRICATO
    Public Const CA_COD_STABILIMENTO As Integer = 1114

    '=============================================================

    'VALORI DEL TITOLO POSSESSO
    Public Const TITOLOPOSSESSO_ALTRO As Integer = 0
    Public Const TITOLOPOSSESSO_PROPRIETA As Integer = 1
    Public Const TITOLOPOSSESSO_COMODATO As Integer = 2
    Public Const TITOLOPOSSESSO_AFFITTO_CONTRATTO As Integer = 3
    Public Const TITOLOPOSSESSO_AFFITTO_NO_CONTRATTO As Integer = 4
    Public Const TITOLOPOSSESSO_CONTRO_TERZI As Integer = 5
    'non sono aggiornati -> usare enum_TitoloPossesso

    '=============================================================

    'CATEGORIE MAGAZZINO
    Public Const RIGA_DESCRIZIONE_LIBERA As Integer = 502
    Public Const RIGA_DESCRIZIONE_LIBERA_DES As String = "Riga Descrizione Libera"
    Public Const ALTRI_BENI_AMMORTIZZABILI As Integer = 500
    Public Const ALTRI_BENI As Integer = 501
    Public Const ALTRI_BENI_DES As String = "Altri Beni Strumentali"
    Public Const SERVIZI As Integer = 555
    Public Const SERVIZI_DES As String = "Servizi"

    Public Const CORPI_ESTRANEI As Integer = -50
    Public Const CALI_LAVORAZIONE As Integer = -1

    Public Const ELEMCOD_MANODOPERA As Integer = 0
    Public Const MACCHINE As Integer = 1
    Public Const CARBURANTI As Integer = 2

    Public Const RIFIUTI As Integer = 4

    Public Const FERTILIZZANTI As Integer = 3
    Public Const FORMULATI As Integer = 191
    Public Const COADIUVANTI As Integer = 195
    Public Const INSETTI As Integer = 196
    Public Const TRAPPOLE As Integer = 197
    Public Const INNESCHI As Integer = 198

    Public Const SEMENTI As Integer = 10
    Public Const ALTRE_MATERIE As Integer = 200
    Public Const ZOO_CONSISTENZA As Integer = 300

    Public Const SEMILAVORATI_VEGETALI As Integer = 201
    Public Const MATERIE_VEGETALI As Integer = 204
    Public Const BENI_CONFEZ_VEGETALE As Integer = 205
    Public Const TRASFORMATI_VEGETALI As Integer = 210

    Public Const SEMILAVORATI_ANIMALI As Integer = 301
    Public Const MATERIE_ANIMALI As Integer = 304
    Public Const BENI_CONFEZ_ANIMALE As Integer = 305
    Public Const TRASFORMATI_ANIMALI As Integer = 310

    Public Const MANGIMI As Integer = 306
    Public Const FARMACI As Integer = 307

    Public Const CONFEZIONI_PRODOTTI As Integer = 400
    Public Const RICAMBI As Integer = 401

    Public Const RIGA_DESCRIZIONE As Integer = 502

    Public Const CAT_MAG_SERVIZI_PROFESSIONALI As Integer = 700

    '=====================================================================================================

    Public Const GRFICOD_MERCATO_FRESCO As Integer = 2
    Public Const GRFICOD_TRASF_USO_ALIMENTARE As Integer = 1

    '=====================================================================================================

    'CODICI IVA

    Public Const IVA_4 As Integer = 4
    Public Const IVA_10 As Integer = 10
    Public Const IVA_12 As Integer = 12
    Public Const IVA_20 As Integer = 20
    Public Const IVA_21 As Integer = 21
    Public Const FCI As Integer = 1
    Public Const NON_IVABILE As Integer = 0

    Public Const EsclArt15 As Integer = 61
    Public Const EsArt7 As Integer = 63
    Public Const Art74LettC As Integer = 64
    Public Const Art74LettE As Integer = 65
    Public Const NonImpArt9 As Integer = 66
    Public Const NonImpArt8 As Integer = 67
    Public Const NonImpArt40 As Integer = 68
    Public Const EsenteArt10 As Integer = 69
    Public Const EsArt14L537 As Integer = 70
    Public Const Art44comma As Integer = 71
    Public Const Art74Ter As Integer = 72
    Public Const NoNImpArt72 As Integer = 73
    Public Const NonImpArt26 As Integer = 74
    Public Const EsclusoArt5 As Integer = 75
    Public Const Art3SubA As Integer = 76
    Public Const EsclArt134 As Integer = 77
    Public Const Art4DL331 As Integer = 78
    Public Const Art18DPR633 As Integer = 79
    Public Const Art71DPR331 As Integer = 80
    Public Const Art10_16DPR633_72 As Integer = 81
    Public Const Art2DPR633_72 As Integer = 82
    Public Const NonImpArt8C1LetB As Integer = 83
    Public Const NonImpArt8C1LetC As Integer = 84
    Public Const NonImpArt8DPR633_72 As Integer = 95
    Public Const NATURA_ESCLUSIONE_NOFILTRO As String = "99"

    '=====================================================================================================

    'Elenco dei Cod_Rapporto (codici dei rapporti contabili di base)
    ''' Esiste anche Enum <see cref="TipiEnumerativi.enum_Rapporti_Contabili_Standard"/>
    Public Const COD_LEGALE As Integer = -1
    Public Const COD_CLIENTE As Integer = -2
    Public Const COD_FORNITORE As Integer = -3
    Public Const COD_CLIENTE_FORNITORE As Integer = -23 'fittizio, usato come raggruppamento nel gias online
    Public Const COD_DIPENDENTE As Integer = -4
    Public Const COD_TERZISTA As Integer = -5
    Public Const COD_DIPENDENTE_TERZISTA As Integer = -45  'fittizio, usato come raggruppamento nel gias online
    Public Const COD_TECNICO As Integer = -6
    Public Const COD_CENTRO_MACCHINE As Integer = -7
    Public Const COD_LAB_ANALISI As Integer = -8
    'creati x fruttagel ma mai usati
    'Public Const COD_SOCIO_CONFERENTE As Integer = -1001
    'Public Const COD_COOP_CONFERENTE As Integer = -1002
    'Public Const COD_PRODUTTORE_ASSOCIATO_COOP As Integer = -1003
    'Public Const COD_PRODUTTORE_INDIVIDUALE As Integer = -1004
    Public Const COD_SOCIO As Integer = -9      ' socio della ditta, titolare
    Public Const COD_TRASPORTATORE As Integer = -10
    Public Const COD_CLIENTEFORNITORE As Integer = -11
    Public Const COD_CLIENTE_FORNITORE_BolleAccett As Integer = -11 'utilizzato nelle bolle di accettazione, ad esempio da fruttagel
    Public Const COD_TECNICORESPONSABILE As Integer = -12
    Public Const COD_AGENTE As Integer = -13
    Public Const COD_REFERENTEAZIENDALE As Integer = -14
    Public Const COD_CONSULENTE As Integer = -15
    Public Const COD_SPEDIZIONIERE As Integer = -16
    Public Const COD_VIVAIO As Integer = -17
    Public Const COD_CONFERENTE As Integer = -18
    Public Const COD_LAB_CQ As Integer = -19
    Public Const COD_CAPO_AREA As Integer = -20
    Public Const COD_AVVENTIZIO As Integer = -21
    Public Const COD_SMALTITORE As Integer = -22
    Public Const COD_FORNITORE_ORTOFRUTTA As Integer = -24
    Public Const COD_ORGANISMO_REFERENTE As Integer = -25
    Public Const COD_RAPPRESENTANTE_FISCALE As Integer = -26
    Public Const COD_COADIUVANTE_FAMILIARE As Integer = -27
    Public Const COD_ORGANISMO_CONTROLLO As Integer = -28
    Public Const COD_RIFERIMENTO_TRASFERIMENTO_DATI As Integer = -29
    Public Const COD_FORNITORE_AGROFARMACI As Integer = -30
    Public Const COD_ALLEVATORE As Integer = -31
    Public Const COD_MACELLO As Integer = -32
    Public Const COD_VETERINARIO As Integer = -33
    Public Const COD_REFERENTE_CONFERIMENTO As Integer = -34
    Public Const COD_DITTA_SEMENTIERA As Integer = -35
    Public Const COD_TRATTORISTA As Integer = -36

    '=====================================================================================================

    'Impostazione del campo Jolly:Int = Movimentazione di Magazzino

    'CASO DI DOCUMENTO ASSOCIATO A UN ALTRO (ES. BOLLE - FATTURE)
    'IL COMPONENTE NON DEVE GESTIRE LE GIACENZE
    'Dettaglio che non comporta movimentazione di magazzino. 
    'Si verifica cioè una delle seguenti ipotesi:
    'a.Movimento che riguarda Servizi o Parco Macchine
    'b.Movimento di Fattura Allegata a Bolla di Accompagnamento già movimentata precedentemente
    'jolly_int = enum_TipoMovimentazioneMagazzino.MagazzinoNONMovimentato
    Public Const MagazzinoNONMovimentato As Integer = 1

    'CASO BUONO DI CARICO E SCARICO
    'IL COMPONENTE GESTISCE LE GIACENZE
    'jolly_int = enum_TipoMovimentazioneMagazzino.MagazzinoMovimentato
    Public Const MagazzinoMovimentato As Integer = 0

    '=====================================================================================================

    'Parametri del Campo 'Contabilizzato' nella tabella 'Movimenti_Dettagli'
    'CONTABILIZZATO POSITIVO -> IL COM+ GESTISCE LE GIACENZE
    Public Const NONCONTABILE As Integer = 1
    Public Const CONTABILE As Integer = 2
    Public Const CONTABILE_EVASO_FORZATAMENTE As Integer = 3
    Public Const CONTABILE_LOTTO_INDISPONIBILE As Integer = 4
    Public Const CONTABILE_PREPARAZIONE_AUTOMATICA_FIFO As Integer = 5
    Public Const CONTABILE_PREPARAZIONE_AUTOMATICA_DISTINTA As Integer = 6
    Public Const CONTABILE_PREPARAZIONE_AUTOMATICA_LOTTO_GIORNATA As Integer = 7

    'CONTABILIZZATO NEGATIVO -> E' UN'OPERAZIONE PENDENTE E NON VENGONO GESTITE LE GIACENZE
    'questo lo fa il componente
    'Public Const NONCONTABILE_PIANIFICATO As Integer = -1
    'Public Const CONTABILE_PIANIFICATO As Integer = -2

    Public Const SEZIONALE_NOFILTRO As Integer = -1

    'riclassificazione
    Public Const BILANCIO_PERSONALIZZATO As Integer = 2
    Public Const PIVA_BILANCIO_EUROPEO As String = "AAAAAAAAAAA"
    Public Const BILANCIO_EUROPEO As Integer = 1
    Public Const BILANCIO_EUROPEO_x_clienti_con_conti_personalizzati As Integer = -1

    'Parametri del Campo 'Flag_Ue' nella tabella 'Conti' e Conti_Patrimonio
    Public Const CONTO_UE_NOFILTRO As Integer = 0
    Public Const CONTO_UE As Integer = 1
    Public Const CONTO_NONUE As Integer = 2

    'Parametri del Campo 'Imputabile' nella tabella 'RicXConti'
    Public Const CE_CONTO_IMPUTABILE_NOFILTRO As Integer = 0
    Public Const CE_CONTO_IMPUTABILE As Integer = 1
    Public Const CE_CONTO_NONIMPUTABILE As Integer = 2

    'Parametri del Campo 'Imputabile' nella tabella 'RicXConti_Patrimonio'
    Public Const SP_CONTO_IMPUTABILE_NOFILTRO As Integer = -1
    Public Const SP_CONTO_IMPUTABILE As Integer = 1
    Public Const SP_CONTO_NONIMPUTABILE As Integer = 0

    'Parametri del Campo 'Per_Risorsa' nella tabella 'Ist_Credito'
    Public Const CC_BANCARIO As Integer = 1
    Public Const BANCO_POSTA As Integer = 2

    Public Const RISORSA_FINANZIARIA As Integer = 1
    Public Const LIQUIDITA_IMMEDIATA As Integer = 2
    'Causali Risorse Economiche
    Public Const CONTO_ECONOMICO_ATTIVO As Integer = 3
    Public Const CONTO_ECONOMICO_PASSIVO As Integer = 4


    'Modalità di Pagamento
    Public Const RIBA As Integer = 1
    Public Const BONIFICO As Integer = 2
    Public Const CONTANTI As Integer = 3
    'Causali per Movimento Finanziario
    Public Const BONIFICO_ATTIVO As Integer = 4
    Public Const BONIFICO_PASSIVO As Integer = 5
    Public Const GIROCONTO As Integer = 6
    Public Const PRELEVAMENTO As Integer = 7
    Public Const VERSAMENTO As Integer = 8
    Public Const AUMENTO_LIQUIDITA As Integer = 9
    Public Const DIMINUZIONE_LIQUIDITA As Integer = 10

    'Costanti per lo stato del pagamento di un documento contabile
    Public Const SALDATO As Integer = 1
    Public Const NONSALDATO As Integer = 2

    'Tipo fabbricati destinazioni Tabella 'Mov_Destinazioni'
    Public Const TIPO_DESTINAZIONE_IMPIANTO As Integer = 0
    Public Const TIPO_DESTINAZIONE_MAGAZZINO As Integer = 20
    Public Const TIPO_DESTINAZIONE_VASCA As Integer = 13
    Public Const MAGAZZINO As Integer = 20
    Public Const STALLA As Integer = 15
    Public Const CONSISTENZA As Integer = 18
    Public Const VASCA_ENOLOGICA As Integer = 13
    Public Const CELLA_FRIGORIFERA As Integer = 16
    Public Const SILOS As Integer = 17
    Public Const TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA As Integer = 21
    Public Const ESSICCATOIO As Integer = 222
    Public Const TIPO_DESTINAZIONE_ANIMALE As Integer = 1

    'nel caricamento combo di magazzini
    Public Const FABBRICATI_NO_STALLE As Integer = -999
    Public Const MAGAZZINI_E_VASCHE As Integer = -998

    '#####################################################################

    'CODICI IMPOSTAZIONE UTENTI
    ' utilizzare i tipi enumerativi
    'documentazione su: \\Rubino2\bk_documentazione\GIAS --- Utenti\Utenti_Impostazioni.doc

    '#####################################################################

    'queste costanti sono usate nel Filtro_ElaboratiContabili.aspx funzione Database_OperazioniPreliminari 
    'per l'inserimento dei conti mancanti (e necessari) nel piano dei conti gias
    '(questa funzione è stata messa per evitare dell'assistenza,
    'così i conti vengono inseriti in automatico, gias non da messaggi di errore
    'e qualcuno non si deve collegare per spiegare come inserirli
    '(specialmente a gente a cui il piano dei conti non interessa)
    'dal migra 387 gli id_riclassificazione non hanno più le lettere, ma i numeri
    'quindi le costanti sono state aggiornate

    'pre migra 387
    'SP
    Public Const GIAS_Id_Riclassificazione_IvaACredito As String = "C.002.d-bis.001"
    Public Const GIAS_Id_Riclassificazione_IvaACredito_AcqIntra As String = "C.002.d-bis.002"
    Public Const GIAS_Id_Riclassificazione_IvaADebito As String = "D.011.l"
    Public Const GIAS_Id_Riclassificazione_IvaADebito_AcqIntra As String = "D.011.m"
    Public Const GIAS_Id_Riclassificazione_ErarioRitenuteLavoroAutonomo As String = "D.011.g"
    Public Const GIAS_Id_Riclassificazione_DebitiVsEnasarco As String = "D.012.d"
    'CE
    Public Const GIAS_Id_Riclassificazione_OmaggiAllaClientela As String = "B.014.a"
    Public Const GIAS_Id_Riclassificazione_RicavixIVAincompensazione As String = "A.005.b"

    ''post migra 387
    ''SP
    'Public Const GIAS_Id_Riclassificazione_IvaACredito As String = "C.002.005.001"
    'Public Const GIAS_Id_Riclassificazione_IvaACredito_AcqIntra As String = "C.002.005.002"
    'Public Const GIAS_Id_Riclassificazione_IvaADebito As String = "D.011.012"
    'Public Const GIAS_Id_Riclassificazione_IvaADebito_AcqIntra As String = "D.011.013"
    'Public Const GIAS_Id_Riclassificazione_ErarioRitenuteLavoroAutonomo As String = "D.011.007"
    'Public Const GIAS_Id_Riclassificazione_DebitiVsEnasarco As String = "D.012.004"
    ''CE
    'Public Const GIAS_Id_Riclassificazione_OmaggiAllaClientela As String = "B.014.001"
    'Public Const GIAS_Id_Riclassificazione_RicavixIVAincompensazione As String = "A.005.002"

    '#####################################################################

    Public Const strContoConferimento As String = "Conto Conferimento"

    Public Const LOTTO_NONDEFINITO As String = "-999"
    Public Const CODPROGETTO_NONDEFINITO As Integer = -999
    Public Const SACOD_CONTATTO_NONDEFINITO As Integer = -999

    Public Const ID_CONF_DEFAULT As String = "1"

#Region "LAV_COD"
    '#####################################################################

    ' GRUPPO OPERAZIONE = 1 : Rilievi in Campo

    Public Const LAVCOD_RILIEVO_FALDA As Integer = 30
    Public Const LAVCOD_FASI_FENOLOGICHE As Integer = 79
    Public Const LAVCOD_INSTALLAZIONE_TRAPPOLE As Integer = 107
    Public Const LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE As Integer = 110
    Public Const LAVCOD_RILIEVO_AVVERSITA_CAMPO As Integer = 113
    Public Const LAVCOD_RILIEVO_ERBE_INFESTANTI As Integer = 119
    Public Const LAVCOD_RILIEVO_PIOGGE As Integer = 126
    Public Const LAVCOD_REINNESCO_TRAPPOLE As Integer = 150
    Public Const LAVCOD_MONITORAGGIO_ACQUE As Integer = 164

    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 2 : Rilievi alla Raccolta

    Public Const LAVCOD_DANNI_RACCOLTA As Integer = 108
    Public Const LAVCOD_RILIEVO_INDICI_MATURITA As Integer = 109
    Public Const LAVCOD_RACCOLTA As Integer = 125
    Public Const LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA As Integer = 169

    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 3 : Trattamenti

    Public Const LAVCOD_CONCIA_SEME As Integer = 13
    Public Const LAVCOD_DISERBO As Integer = 18
    Public Const LAVCOD_TRATTAMENTO_ANTIPARASSITARIO As Integer = 74
    Public Const LAVCOD_TRATTAMENTO_FITOREGOLATORE As Integer = 103
    Public Const LAVCOD_TRATTAMENTO_ANTIBUTTERATURA As Integer = 106
    Public Const LAVCOD_DISTRIBUZIONE_INSETTI As Integer = 116
    Public Const LAVCOD_CONFUSIONE_SESSUALE As Integer = 118
    Public Const LAVCOD_DISORIENTAMENTO_SESSUALE As Integer = 121
    Public Const LAVCOD_CATTURE_MASSA As Integer = 122
    Public Const LAVCOD_GEODISINFESTAZIONE As Integer = 155
    Public Const LAVCOD_DISSECCAMENTO As Integer = 158
    Public Const LAVCOD_TRATTAMENTO_POST_RACCOLTA As Integer = 163
    Public Const LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO As Integer = 165
    Public Const LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE As Integer = 172
    Public Const LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA As Integer = 173

    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 4 : Lavorazioni

    Public Const LAVCOD_IRRIGAZIONE As Integer = 1
    Public Const LAVCOD_SEMINA As Integer = 2

    Public Const LAVCOD_ARATURA As Integer = 8
    Public Const LAVCOD_ANDANAMENTO As Integer = 9


    Public Const LAVCOD_ASSOLCATURA As Integer = 10

    Public Const LAVCOD_CIMATURA As Integer = 12

    Public Const LAVCOD_DISTRIBUZIONE_CONCIME As Integer = 14

    Public Const LAVCOD_DIRADAMENTO_MANUALE As Integer = 17

    Public Const LAVCOD_DISSODAMENTO As Integer = 19


    Public Const LAVCOD_ERPICATURA As Integer = 21
    Public Const LAVCOD_ESTIRPATURA As Integer = 22
    Public Const LAVCOD_FALCIACONDIZIONATURA As Integer = 23

    Public Const LAVCOD_FALCIATURA_ERBAI As Integer = 25
    Public Const LAVCOD_FERTIRRIGAZIONE As Integer = 26
    Public Const LAVCOD_FORMAZIONE_ARGINELLI As Integer = 27


    Public Const LAVCOD_FRANGIZOLLATURA As Integer = 31
    Public Const LAVCOD_FRESATURA As Integer = 32
    Public Const LAVCOD_IMBALLO_FIENO_ROTOLI As Integer = 33
    Public Const LAVCOD_INTERRAMENTO_PAGLIE As Integer = 34


    Public Const LAVCOD_LIVELLAMENTO As Integer = 40
    Public Const LAVCOD_MANUTENZIONE_ARGINI As Integer = 41

    Public Const LAVCOD_MIETITREBBIATURA As Integer = 46
    Public Const LAVCOD_MINIMUM_TILLAGE As Integer = 47
    Public Const LAVCOD_PACCIAMATURA As Integer = 48
    Public Const LAVCOD_PRESSATURA As Integer = 49


    Public Const LAVCOD_RANGHINATURA As Integer = 53

    Public Const LAVCOD_RINCALZATURA As Integer = 55
    Public Const LAVCOD_RIPUNTATURA As Integer = 56
    Public Const LAVCOD_RIVOLTAMENTO_FORAGGIO As Integer = 57
    Public Const LAVCOD_RULLATURA As Integer = 58
    Public Const LAVCOD_SARCHIATURA As Integer = 59


    Public Const LAVCOD_SCARIFICATURA As Integer = 62
    Public Const LAVCOD_SCASSO As Integer = 63
    Public Const LAVCOD_SOD_SEDDING As Integer = 68


    Public Const LAVCOD_TRAPIANTO As Integer = 71

    Public Const LAVCOD_TRINCIATURA As Integer = 75
    Public Const LAVCOD_VANGATURA As Integer = 76
    Public Const LAVCOD_ZAPPATURA As Integer = 77
    Public Const LAVCOD_CARICO_MANUALE_FRUTTA As Integer = 78


    Public Const LAVCOD_ESPIANTO As Integer = 81
    Public Const LAVCOD_LAVORAZIONE_TRA_FILA As Integer = 82
    Public Const LAVCOD_LAVORAZIONE_SU_FILA As Integer = 83
    Public Const LAVCOD_LEGATURA As Integer = 84
    Public Const LAVCOD_MESSA_DIMORA_PIANTE As Integer = 85

    Public Const LAVCOD_POTATURA_SECCA As Integer = 87
    Public Const LAVCOD_POTATURA_VERDE As Integer = 88


    Public Const LAVCOD_RACCOLTA_LEGNA_POTATURA As Integer = 90
    Public Const LAVCOD_RACCOLTA_MANUALE As Integer = 91
    Public Const LAVCOD_RACCOLTA_MECCANICA As Integer = 92


    Public Const LAVCOD_RIPPATURA As Integer = 115


    Public Const LAVCOD_ASPORTAZIONE_ORGANI_INFETTI As Integer = 120

    Public Const LAVCOD_CONCIMAZIONE_FOGLIARE As Integer = 123
    Public Const LAVCOD_DISTRIBUZIONE_AMMENDANTI As Integer = 124


    Public Const LAVCOD_TRAPIANTO_IN_SERRA As Integer = 151
    Public Const LAVCOD_GEBIATURA As Integer = 152
    Public Const LAVCOD_ROMPICROSTA As Integer = 153
    Public Const LAVCOD_LAVORAZIONE_CONBINATA As Integer = 154

    Public Const LAVCOD_SARCHIATURA_CONCIMAZIONE As Integer = 156
    Public Const LAVCOD_ERPICATURA_ROTANTE As Integer = 157

    Public Const LAVCOD_MANUTENZIONE_IMPIANTI As Integer = 159


    Public Const LAVCOD_SOVESCIO As Integer = 160
    Public Const LAVCOD_INTERVENTO_ANTIBRINA As Integer = 161
    'Public Const LAVCOD_ALTRE_OPERAZIONI As Integer = 162
    Public Const LAVCOD_ALTRE_OPERAZIONI As Integer = 162
    Public Const LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO As Integer = 166
    Public Const LAVCOD_STRIGLIATURA As Integer = 167
    Public Const LAVCOD_PIRODISERBO As Integer = 168

    Public Const LAVCOD_ABBATTIMENTOIMPIANTI As Integer = 170
    Public Const LAVCOD_DEFOGLIAZIONE As Integer = 171

    Public Const LAVCOD_PASCOLAMENTO_PROPRIO As Integer = 174
    Public Const LAVCOD_PASCOLAMENTO_TERZI As Integer = 175


    Public Const LAVCOD_CURA As Integer = 5004

    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 5 : Altre operazioni colturali (non gestite)

    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 6 : Documenti contabili

    Public Const LAVCOD_BOLLA_EMESSA As Integer = 1031
    Public Const LAVCOD_BOLLA_RICEVUTA As Integer = 1025
    Public Const LAVCOD_DDT_CONTABILIZZATO_EMESSO As Integer = 1069

    Public Const LAVCOD_FATTURA_EMESSA As Integer = 1001
    Public Const LAVCOD_FATTURA_RICEVUTA As Integer = 1000
    Public Const LAVCOD_FATTURA_PROFESSIONISTI As Integer = 1070

    Public Const LAVCOD_FATTURA_PROFORMA As Integer = 1064

    Public Const LAVCOD_FATTURA_LIQ_CONF_EMESSA As Integer = 1055 'Emissione Fattura Liquidazione Conferimenti
    Public Const LAVCOD_FATTURA_LIQ_CONF_RICEVUTA As Integer = 1056 'Ricevimento Fattura Liquidazione Conferimenti
    Public Const LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA As Integer = 1057 'Emissione Autofattura Liquidazione Conferimenti
    Public Const LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA As Integer = 1058 'Ricevimento Autofattura Liquidazione Conferimenti

    Public Const LAVCOD_RICEVUTA_EMESSA As Integer = 1053

    Public Const LAVCOD_ALTRI_RICAVI As Integer = 1027
    Public Const LAVCOD_ALTRI_COSTI As Integer = 1026

    'RESI/ABBUONI SU ACQUISTI
    Public Const LAVCOD_NOTA_ACCREDITO_RICEVUTA As Integer = 1002

    'RESI/ABBUONI SU VENDITE
    Public Const LAVCOD_NOTA_ACCREDITO_EMESSA As Integer = 1003

    Public Const LAVCOD_MOV_FINANZIARIO As Integer = 1032
    Public Const LAVCOD_REG_COMPENSI As Integer = 1005

    Public Const LAVCOD_AUTOFATTURA_BENI_ESTERO As Integer = 1004
    Public Const LAVCOD_SPESE_PER_DIPENDENTI As Integer = 1006
    Public Const LAVCOD_SPESE_VARIE As Integer = 1007
    Public Const LAVCOD_ACQUISTO_MATERIE_PRIME_SOCI As Integer = 1060

    Public Const LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI As Integer = 1079

    Public Const LAVCOD_DOCO_EMESSO As Integer = 1061
    Public Const LAVCOD_DOCO_RICEVUTO As Integer = 1062

    Public Const LAVCOD_DAA_EMESSO As Integer = 1063

    Public Const LAVCOD_MVV_EMESSO As Integer = 1071
    Public Const LAVCOD_MVV_RICEVUTO As Integer = 1072

    Public Const LAVCOD_ALTRI_RICAVI_NERO As Integer = 1073
    Public Const LAVCOD_ALTRI_COSTI_NERO As Integer = 1074

    Public Const LAVCOD_COSTI_CDG As Integer = 4500
    Public Const LAVCOD_GHG As Integer = 4700

    '------------------------------------------------------------

    'GRUPPO OPERAZIONE = 7 : Documenti contabili - Beni e ammortamenti

    Public Const LAVCOD_ACQUISTO_BENI As Integer = 1008
    Public Const LAVCOD_CESSIONE_BENI As Integer = 1009

    Public Const LAVCOD_REG_AMMORTAMENTI As Integer = 1010
    Public Const LAVCOD_FINE_AMMORTAMENTI As Integer = 1011

    '-------------------------------------------------------------

    'GRUPPO OPERAZIONE = 8 : Documenti contabili - Pagamenti

    Public Const LAVCOD_PAGAMENTO_FATTURA As Integer = 1012
    Public Const LAVCOD_INCASSO_FATTURA As Integer = 1013

    Public Const LAVCOD_PAGAMENTI_DIVERSI As Integer = 1014
    Public Const LAVCOD_INCASSI_DIVERSI As Integer = 1015

    Public Const LAVCOD_PAGAMENTO_RATA_PRESTITO As Integer = 1024

    '--------------------------------------------------------------

    'GRUPPO OPERAZIONE = 9 : Documenti contabili - Scadenze

    Public Const LAVCOD_SCADENZA_FATTURA_EMESSA As Integer = 1017
    Public Const LAVCOD_SCADENZA_FATTURA_RICEVUTA As Integer = 1016

    Public Const LAVCOD_SCADENZA_PAGAMENTI_DIVERSI As Integer = 1018
    Public Const LAVCOD_SCADENZA_INCASSI_DIVERSI As Integer = 1019

    '--------------------------------------------------------------

    'GRUPPO OPERAZIONE = 10 : Documenti contabili - Varie

    Public Const LAVCOD_CARICO As Integer = 1022
    Public Const LAVCOD_SCARICO As Integer = 1023

    Public Const LAVCOD_PARTITA_DOPPIA As Integer = 1032
    Public Const LAVCOD_TRASFERIMENTO As Integer = 1033

    Public Const LAVCOD_AUTOCONSUMO As Integer = 1028
    Public Const LAVCOD_AUTOCONSUMO_VINO_SFUSO As Integer = 1066
    Public Const LAVCOD_PRODUZIONI As Integer = 1029

    Public Const LAVCOD_MOV_MAG_MATERIE_PRIME As Integer = 1030

    Public Const LAVCOD_CONFERIMENTO As Integer = 1050
    Public Const LAVCOD_ACCETTAZIONE As Integer = 1051
    Public Const LAVCOD_CONFERIMENTO_DIVERSI As Integer = 1052
    Public Const LAVCOD_ACCETTAZIONE_DIVERSI As Integer = 1054

    Public Const LAVCOD_VENDITA As Integer = 1020  'Corrispettivo Vendita
    Public Const LAVCOD_ACQUISTO As Integer = 1021
    Public Const LAVCOD_CORRISPETTIVO_VENDITA_SFUSO As Integer = 1065 'Corrispettivo Vendita Vino Sfuso

    Public Const LAVCOD_DISTINTA_CARICO As Integer = 1075
    Public Const LAVCOD_DISTINTA_CARICO_ACCETTAZIONE As Integer = 1076
    Public Const LAVCOD_AUTO_DDT_EMESSO As Integer = 1077
    Public Const LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE As Integer = 1078

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 11 : Documenti contabili - Note, rate, ordini, preventivi, contratti

    Public Const LAVCOD_NOTE As Integer = 2000
    Public Const LAVCOD_RATE As Integer = 2001
    Public Const LAVCOD_ORDINE_VENDITA As Integer = 2002
    Public Const LAVCOD_ORDINE_ACQUISTO As Integer = 2004
    Public Const LAVCOD_PREVENTIVO_VENDITA As Integer = 2003
    Public Const LAVCOD_TESTATE_ORDINE_LAVORAZIONE As Integer = 2005
    Public Const LAVCOD_CONTRATTO_AFFITTO As Integer = 2006

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 12 : Analisi latte

    Public Const LAVCOD_ANALISI_LATTE_SINGOLA As Integer = 3006
    Public Const LAVCOD_ANALISI_LATTE_MASSA As Integer = 3007

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 13 : Varie animali

    Public Const LAVCOD_NASCITA_ANIMALI As Integer = 3000

    Public Const LAVCOD_INCREMENTO_CONSISTENZE_ZOO As Integer = 3001
    Public Const LAVCOD_DECREMENTO_CONSISTENZE_ZOO As Integer = 3002

    Public Const LAVCOD_MORTE_ANIMALI As Integer = 3003
    Public Const LAVCOD_MACELLAZIONE_ANIMALI As Integer = 3004

    Public Const LAVCOD_SOSTITUZIONE_MARCA As Integer = 3005

    Public Const LAVCOD_VACCINAZIONI_ANIMALI As Integer = 3027
    Public Const LAVCOD_CUREMEDICAMENTI_ANIMALI As Integer = 3028

    Public Const LAVCOD_PESATURA_ANIMALI As Integer = 3033
    Public Const LAVCOD_ACQUISTO_ANIMALI As Integer = 3034
    Public Const LAVCOD_VENDITA_ANIMALI As Integer = 3035
    Public Const LAVCOD_TRASFERIMENTO_ANIMALI As Integer = 3037

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 14 : Rilievi produzioni

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 15 : Gestione mungitura

    Public Const LAVCOD_MUNGITURA_PREPARAZIONE = 3009
    Public Const LAVCOD_MUNGITURA_SECCHIO_POSTA = 3010
    Public Const LAVCOD_MUNGITURA_GRUPPI_POSTA = 3011
    Public Const LAVCOD_MUNGITURA_SALA_LATTE = 3012
    Public Const LAVCOD_MUNGITURA_LAVAGGIO_IMPIANTI = 3013
    Public Const LAVCOD_MUNGITURA_LAVAGGIO_SALA_LATTE = 3014

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 16 : Gestione lettiere

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 17 : Gestione alimentazione

    Public Const LAVCOD_ALIMENTAZIONE_PULIZIA_IMPIANTI = 3019
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI = 3020
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI = 3021
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI = 3022
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI = 3023
    Public Const LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 18 : Altri lavori di stalla

    Public Const LAVCOD_SPOSTAMENTI_ZOO As Integer = 3030
    Public Const LAVCOD_ALTRE_LAVORAZIONI_ZOO = 3036

    '------------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 19 : Macchine

    Public Const LAVCOD_MANUTENZIONE_MACCHINE As Integer = 1500
    Public Const LAVCOD_REVISIONE_MACCHINE As Integer = 4000

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 20 : Gestione visite ispettive

    Public Const LAVCOD_GESTIONE_RIFIUTI As Integer = 1080

    Public Const LAVCOD_MONITORAGGIO_TEMPI_RIENTRO As Integer = 5001
    Public Const LAVCOD_VISITA_GENERICA As Integer = 5002
    Public Const LAVCOD_MONITORAGGIO_CE As Integer = 5003

    Public Const LAVCOD_PRATICA_ECOLOGICA As Integer = 5005
    Public Const LAVCOD_FORMAZIONE As Integer = 5006
    Public Const LAVCOD_VISITA As Integer = 5007

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 100 : Linee produzione vegetale

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 101 : Linee produzione animale

    '------------------------------------------------------------

    'Le operazioni 5000 sono Trasformazioni/Preparazione e non appartengono ad alcun gruppo

    Public Const LAVCOD_TRASFORMAZIONI As Integer = 5000
    Public Const LAVCOD_PREPARAZIONE As Integer = 5000
#End Region

    '#####################################################################

    Public Const DatiAnagrafici_Azienda As String = "imprese"
    Public Const DatiAnagrafici_Catasto As String = "catasto"
    Public Const DatiAnagrafici_Possessi As String = "possessi"
    Public Const DatiAnagrafici_Contatti As String = "contatti"
    Public Const DatiAnagrafici_Macchine As String = "macchine"
    Public Const DatiAnagrafici_PianoColturale As String = "pianocolturale"
    Public Const DatiAnagrafici_Fabbricati As String = "fabbricati"
    Public Const DatiAnagrafici_Allevamenti As String = "allevamenti"

    '#####################################################################

    'SEQUENZA PROGRESSIVI TIPI

    Public Const SEQ_PROG_NUMPAG_RegCS_PomoContrattato As Integer = 6
    Public Const SEQ_PROG_NUMRIGA_RegCS_PomoContrattato As Integer = 7
    Public Const SEQ_PROG_NUMPAG_RegCS_PomoNoContrattato As Integer = 8
    Public Const SEQ_PROG_NUMRIGA_RegCS_PomoNoContrattato As Integer = 9

    Public Const UpperBoundTabelle_Per_SequenzaTabelle_Topcode As Integer = 2000000000

    '#####################################################################

    'FORMULE CONVERSIONE ELEMENTI

    Public Const Fattore_Conversione_P_P2O5 As Decimal = 2.291D
    Public Const Fattore_Conversione_P2O5_P As Decimal = 0.436D

    Public Const Fattore_Conversione_K_K2O As Decimal = 1.205D
    Public Const Fattore_Conversione_K2O_K As Decimal = 0.83D

    '#####################################################################
    'tabella hash degli utenti connessi
    Public Shared TableHashUtentiConnessi As New Hashtable

    '#####################################################################
    'Gruppo Coefficienti UBA
    Public Const AgroConst_GruppoCoefficientiUBA As Integer = 0

    ''Indirizzo del sito delle stampe
    ''Public Const AgroConst_SitoStampe As String = "http://localhost/AgronicaStampe/GestioneRichieste.aspx"
    Public Const AgroConst_PaginaRichiestaStampe As String = "../GestioneStampe/ChiamaStampe.aspx"

    '#####################################################################
    'Prefisso Codice Operatore
    Public Const AgroCode_CodiceOperatore As String = "IT CDX"

    '#####################################################################
    'Espressioni regolari
    'Public Const EXPREG_USERNAME As String = "^[a-zA-Z](.{1,9})$"
    'Public Const EXPREG_PASSWORD As String = "^[a-zA-Z](.{1,9})$"
    Public Const EXPREG_USERNAME As String = "[a-zA-Z0-9]+([.][a-zA-Z0-9]+)?"
    Public Const EXPREG_PASSWORD As String = "\A[a-zA-Z0-9!?_$%&]+\z"
    Public Const PASSWORD_CARATTERI_SPECIALI_AMMESSI As String = "! ? _  $ % &"

    '=====================================================================================================

    '#####################################################################

    Public Const FiltroElaborati_HEIGHT As String = "650"
    Public Const FiltroElaborati_WIDTH As String = "650"

    Public Const PopupConti_HEIGHT As String = "620"
    Public Const PopupConti_WIDTH As String = "800"

    '#####################################################################

    Public Const WaTable_Version_Edge = "1.10.1"

    '#####################################################################
    '   Utilizzati per tipo parametri in tabella Parametri di GIAS INTERSCAMBIO
    Public Const INTERSCAMBIO_PARAMETRI_CATEGORIE As String = "Categorie"
    Public Const INTERSCAMBIO_PARAMETRI_CAUSALI_TRASPORTO As String = "Causali_Trasporto"
    Public Const INTERSCAMBIO_PARAMETRI_GESTIONE_VETTORI As String = "Gestione_Vettori"
    Public Const INTERSCAMBIO_PARAMETRI_IVA As String = "IVA"
    Public Const INTERSCAMBIO_PARAMETRI_PAGAMENTI_CAUSALI As String = "Pagamenti_Causali"
    Public Const INTERSCAMBIO_PARAMETRI_TIPO_FATTURA As String = "Tipo_Fattura"
    Public Const INTERSCAMBIO_PARAMETRI_UNITA_MISURA As String = "Unita_Misura"
    Public Const INTERSCAMBIO_PARAMETRI_MAGAZZINO As String = "Magazzino"
    Public Const INTERSCAMBIO_PARAMETRI_CELLA As String = "Cella"
    Public Const INTERSCAMBIO_PARAMETRI_TIPO_OPERAZIONE_MAG As String = "Tipo_Operazione_Mag"

    Public Const TIPO_XML_CMAG As String = "CMAG"
    Public Const TIPO_XML_SMAG As String = "SMAG"
    Public Const TIPO_XML_GIACENZA As String = "GIACENZA"
    Public Const TIPO_XML_ROTTURA As String = "ROTTURA"
    Public Const TIPO_XML_CORRISPETTIVO_VENDITA As String = "CORRISPETTIVO"
    Public Const TIPO_XML_DDT_EMESSO As String = "DDT EMESSO"
    Public Const TIPO_XML_DDT_RICEVUTO As String = "DDT RICEVUTO"
    Public Const TIPO_XML_DDT_RESO_CONTO_LAVORO As String = "DDT RESO C/LAVORO"
    Public Const TIPO_XML_DDT_ACCETTAZIONE As String = "DDT ACCETTAZIONE"
    Public Const TIPO_XML_FATTURA_ACCOMPAGNATORIA As String = "FATTURA ACCOMPAGNATORIA"
    Public Const TIPO_XML_FATTURA_ACCOMPAGNATORIA_RICEVUTA As String = "FATTURA ACCOMPAGNATORIA RICEVUTA"
    Public Const TIPO_XML_NOTA_CREDITO As String = "NOTA DI CREDITO"
    Public Const TIPO_XML_ODA As String = "ODA"
    Public Const TIPO_XML_BLOCCO_LOTTO As String = "BLOCCO LOTTO"
    Public Const TIPO_XML_SBLOCCO_LOTTO As String = "SBLOCCO LOTTO"

    '#####################################################################
    '   Utilizzati per caricare il logo in fatture, ddt
    Public Const STAMPE_CONTAB_LOGO_HEADER As String = "LogoHeader"
    Public Const STAMPE_CONTAB_LOGO_FOOTER As String = "LogoFooter"
    Public Const STAMPE_CONTAB_LOGO_IN_ALTO As String = "LogoInAlto"
    Public Const STAMPE_CONTAB_LOGO_IN_BASSO As String = "Logo"

    '#####################################################################
    Public Const CONTO_ECONOMICO As String = "Eco"
    Public Const CONTO_PATRIMONIALE As String = "Pat"

    Public Const CONTO_ECO_RICAVI_VENDITE = 2          'A.001 - Ricavi delle Vendite e delle prestazioni
    Public Const CONTO_ECO_COSTI_MATERIE_PRIME = 8     'B.006 - Costi Della Produzione - Per Materie Prime, sussidiarie, di Consumo e di Merci
    Public Const CONTO_PAT_CREDITI_VS_CLIENTI = 34     'C.002.001 - Crediti Verso Clienti Meno Svalutazione Crediti
    Public Const CONTO_PAT_DEPOSITI = 46
    Public Const CONTO_PAT_CASSA = 48
    Public Const CONTO_PAT_DEBITI_VS_FORNITORI = 75    'D.006 - Debiti Verso Fornitori
    '#####################################################################

    Public Const FamRameici_cod As Integer = 43
    Public Const PaRameici_str As String = "(102,338,350,368,369,370,371,372,529,616,636,692,883)"

    '#######################     PDC       ################################
    '   Utilizzati per tipo parametri in tabella PDC_Mappature
    Public Const PDC_MAPPATURA_REGIONE As String = "REGIONE"
    Public Const PDC_MAPPATURA_NAZIONE As String = "NAZIONE"
    Public Const PDC_MAPPATURA_MOTIVO_CAMP As String = "MOTIVO_CAMP"
    Public Const PDC_MAPPATURA_MOLECOLA As String = "PA"
    Public Const PDC_MAPPATURA_PARAMETRI_MERCEOLOGICA As String = "PAR_MERCE"
    Public Const PDC_MAPPATURA_CULTIVAR As String = "CULTIVAR"

    Public Const PDC_REST_API_PEDONLAB As String = "API1"

    '#####################################################################

    Public Const LOTTO_VUOTO_ByPass As String = "DocContabile"  'Per fare in modo che se il lotto è non specificato, venga scritto effettivamente stringa vuota e non Indefinito

    '#####################################################################

    Public Enum CredenzialiWS_Tipo
        ImportAgrea_Coldiretti = 1
        ImportAgrea_Confagricoltura = 2
        ImportAgrea_Cia = 3
        ImportAgrea_LegaCoop = 4
    End Enum

    Public Enum LinkWS_Tipo
        AnagrafeBA = 1
    End Enum

    Public Class AdaptFrameworkPluginNames
        Public Shared DeereGen4 As String = "Deere-GS4_4600"
        Public Shared Deere2630 As String = "Deere-GS3_2630"
        Public Shared Deere2600 As String = "Deere-GS2_2600"
        Public Shared Deere1800 As String = "Deere-GS2_1800"
        Public Shared DeereGen2_CommandCenter As String = "Deere-GS2_CommandCenter"
        Public Shared AgGatewayIsoXml As String = "ISOv4Plugin"
        Public Shared AgGatewayApplicationDataModel As String = "ADM"
    End Class

    '#####################################################################
    Public Const XML_Log_Categoria_Agenda = "AGENDA"
    Public Const XML_Log_Categoria_Agenda_Costi = "AGENDA_COSTI"
    Public Const XML_Log_Categoria_Impianti = "REG_IMPIANTI"
    Public Const XML_Log_Categoria_Progetto = "IMPRESE_PROGETTI"
    '#####################################################################

    'Parametri qualitativi
    Public Const ParametriQualitativi_Fornitore As String = "ofornitore"
    Public Const ParametriQualitativi_Imballaggio As String = "oimballaggio"
    Public Const ParametriQualitativi_Contenitore As String = "ocontenitore"
    Public Const ParametriQualitativi_Confezione As String = "oconfezione"
    Public Const ParametriQualitativi_Calibro As String = "ocalibro"
    Public Const ParametriQualitativi_Qualita As String = "oqualità"

    'Placeholder contesto integrazione macchine lavorazione
    Public Const PlaceHolderContestoIntegrMacchineLav_InvioLavColleg As String = "#INVIO_LAV_COLLEG#"

    'Tipologie funzioni algoritmi integrazione macchine lavorazione
    Public Const TipoFunzioneAlgoritmoLottoTestata As String = "LottoTestata"

    'Tipo regolamento Bio
    Public Const Tipo_Regolamento_Bio As Integer = -2
    Public Const Descrizione_Regolamento_Bio As String = "Reg. UE 848/2018 (Ex Reg. CE 834/07) - BIO"
    Public Const Descrizione_Regolamento_Bio_STAMPE As String = "Reg. UE 848/2018 (Ex Reg. CE 834/07)"
    Public Const Descrizione_Regolamento_Bio_New As String = "BIO"

    'Valore standard per escludere le microgiacenze
    Public Const QTA_GiancenzeVisualizzate As Decimal = 0.00009D

    'Tipo ricerca documenti contabili
    Public Const DocContab_TipoRicerca_Acquisti As String = "A"
    Public Const DocContab_TipoRicerca_Vendite As String = "V"
    Public Const DocContab_TipoRicerca_Conferimenti As String = "C"
    Public Const DocContab_TipoRicerca_Contratti As String = "CO"

    'Tipo documento contabile
    Public Const DocContab_TipoDoc_Ordine As String = "O"
    Public Const DocContab_TipoDoc_Consegna As String = "C"
    Public Const DocContab_TipoDoc_Fattura As String = "F"
    Public Const DocContab_TipoDoc_Pomodoro As String = "P"
    Public Const DocContab_TipoDoc_ContrattoAffitto As String = "AF"

    'Documenti Contabili - Ordine_Det per righe di imballi
    Public Const RIGA_IMBALLI_CONTENTI_PRODOTTI As Integer = 1000
    Public Const RIGA_IMBALLI_VUOTI_IN_ENTRATA As Integer = 30000

    Public Const Str_TerrenoNudo As String = "Terreno Nudo"

    Public Const DEFAULT_UTENTE_MENU_NUMERO_ULTIME_AZIENDE_SELEZIONATE_DA_VISUALIZZARE = 4

    Public Const VERSIONE_MASTER_DEFAULT As String = "Agronica"

    Public Const VERSIONE_HEADER_DEFAULT As String = "Agronica"

    Public Const VERSIONE_LOGIN_DEFAULT As String = "Agronica"

    Public Const GIAS_BASE_DEFAULT_LINK As String = "/GiasBase"

    Public Const NEWLINE As String = "</br>"
    Public Const TEXTINDENT As String = " &nbsp  &nbsp  &nbsp  &nbsp"

    Public Const LinkQdCAngular As String = "/QdC/"

    Public Const LinkVisiteEditAngular As String = "/QdC/Visite-Edit"

    'Rilievi fioritura e fasi fenologiche
    Public Const ScalaBBCH As String = "BBCH"

    Public Const Notifiche_Piattaforma_Default = "Firebase"

    Public Const CodMascheraFiltroMappeSatellitari As Int32 = -1

    Public Const NessunaSpecieQdC As Integer = -10

#Region "NOTE LOG ANAGRAFE"
    'NOTE LOG --> modifica anagrafe effettuata da ... -- UTILI PER L'ASSISTENZA
    Public Const NOTELOG_ANAGRAFE_NG As String = "Operazione registrata da Anagrafica (NG)"
    Public Const NOTELOG_ANAGRAFE_CONTATTI_DEMETRA = "Operazione registrata da Importazione (DEMETRA)"
    Public Const NOTELOG_ANAGRAFE_APP As String = "Operazione registrata da APP"
    Public Const NOTELOG_ANAGRAFE_BOOTSTRAP As String = "Operazione registrata da Anagrafica (bootstrap)"
#End Region

#Region "TIPI AZIENDA UMA"
    Public Const Azienda_Agricola_Privata = "Azienda Agricola Privata"
    Public Const Azienda_Terzista = "Azienda Terzista" 'EX TERZISTI
    Public Const Cooperativa_Agricola = "Cooperativa Agricola"
    Public Const Azienda_Agricola_Istituzioni_Pubbliche = "Azienda Agricola Pubblica"
    Public Const Consorzio_Bonifica_Irrigazione = "Consorzio di Bonifica e Irrigazione"
#End Region

#Region "RETAIL"
    Public Const Retail_MittenteSMS As String = ""
    Public Const Retail_testoSMS As String = "TESTO SMS"

    Public Const Retail_testoEMAILFileLocation As String = "Resources\BodyEmailConfermaRetail.html"
    Public Const Retail_OggettoEMAIL As String = "OGGETTO EMAIL"
    Public Const Retail_ChiaveMittenteEMAIL As String = "MailFrom_smtp"

    Public Const Retail_NuovoIdDaAppPrefisso = "APP"
#End Region

    'Public Const Json_Ser_DTZ_Handling AS Newtonsoft.Json.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind
    Public Const Json_Ser_DTZ_Handling As Newtonsoft.Json.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local

#Region "AUTHDISPATCHER"
    Public Const AuthDispatcher_Configurations As String = "AuthDispatcher_Configurations"
    Public Const AuthDispatcher_SignUrlAPI As String = "api/authenticate/sign-url"
    Public Const AuthDispatcher_AbacoUrlAPI As String = "api/authenticate/abaco-url"
    Public Const AuthDispatcher_AutenticaGEE As String = "api/authenticate/google-ee"
    Public Const AuthDispatcher_AutenticaGoogleCloud As String = "api/authenticate/google-cloud-storage"
#End Region

#Region "GEST_MAPPE_SATELLITARI"
    Public Const MappeSatellitari_EndpointLegacySuffisso = "Sentinel2"
    Public Const MappeSatellitari_NuovaChiaveConf = "GiasOnline_WS_Mappe_2023"
    Public Const MappeSatellitari_VecchiaChiaveConf = "GiasOnline_WS_Mappe_2013"

    Public Const SAT_BaseUrl_ConfKey As String = "urlEngine_SAT"
    Public Const SAT_ApiKey_ConfKey As String = "apiKeyEngine_SAT"
    Public Const SAT_Available_Indexes_ConfKey As String = "availableIndexesEngine_SAT"
    Public Const SAT_TenantName_ConfKey As String = "tenantNameEngine_SAT"
    Public Const SAT_IsActive_ConfKey As String = "isActiveEngine_SAT"
    Public Const SAT_MaxObservationWindowYears_ConfKey As String = "maxObservationWindowYears_SAT"
    Public Const SAT_PixelCoverageThreshold_ConfKey As String = "pixelCoverageThreshold_SAT"
#End Region

#Region "ENGINE_MAPPE_PRESCRIZIONE"
    Public Const MappePrescrizione_BaseUrl_ConfKey As String = "urlEngine_MappePrescrizione"
    Public Const MappePrescrizione_TenantName_ConfKey As String = "tenantNameEngine_MappePrescrizione"
    Public Const MappePrescrizione_ApiKey_ConfKey As String = "apiKeyEngine_MappePrescrizione"
    Public Const MappePrescrizione_IsActive_ConfKey As String = "isActiveEngine_MappePrescrizione"
    Public Const MappePrescrizione_WriteNotifications_ConfKey As String = "writeNotificationsEngine_MappePrescrizione"
    Public Const MappePrescrizione_ModelCode_ConfKey As String = "modelCodeConcimazioneEngine_MappePrescrizione"
    Public Const MappePrescrizione_ModelCode_Semina_ConfKey As String = "modelCodeSeminaEngine_MappePrescrizione"
#End Region

#Region "LAYER"
    Public Const Layer_Trasparenza_Default As Double = 0.6
    Public Const Layer_Colore_Primario_Default As String = "D27A08"
    Public Const Layer_Trasparenza_Raster As Double = 0.0
    Public Const Layer_Colore_Primario_Raster As String = "E0EBF6"
#End Region

#Region "AlgoritmiProiezione"
    Public Const CallBack_Piattaforma_GEE As String = "Gis/NuovaElaborazionePiattaformaGEE/"
    Public Const CallBack_PianoConcimazione_GEE As String = "Gis/NuovaElaborazionePianoConcimazioneGEE/"

    Public Const AlgoritmiProiezione_File_Raster_Max_Size As String = "Il file eccede le dimensioni massime per l'invio a Google Earth Engine."

    Public Const Google_Cloud_Conf_Key = "pathFileRaster_GoogleEarthEngine"
    Public Const Google_Earth_Engine_ConfKey As String = "Configurazioni_GoogleEarthEngine"

    Public Const Google_Cloud_Storage_Default_View = "DefaultView"
    Public Const WS_Mappe_ConfKey As String = "Configurazioni_WS_Mappe_2024"


#End Region

#Region "PRECISION_FARMING"
    'JSON input
    Public Const Precision_Farming_GEE_Product As String = "LANDSAT/LC08/C02/T1_TOA"
    Public Const Precision_Farming_GEE_Layer As String = "Mappe Prescrizione"
    Public Const Precision_Farming_GEE_Layer_Icon As String = "MappePrescrizione32.png"
    Public Const Precision_Farming_GEE_AttributeList As String = "CAP_N+,Prod#_secc"
    Public Const Precision_Farming_GEE_Index As String = "NDVI"

#End Region

#Region "DATI_SENSORI"
    Public Const Dati_Sensori_Platform = "LANDSAT/LC08/C02/T1_TOA"
#End Region

#Region "BULK_EXPORT"
    Public Const BulkExportIncrement As Int32 = 500
#End Region

    Public Const UserDefaultWidgets As String = "-1"

#Region "TIPO_FABBRICATI"
    Public Const TIPO_FABBRICATO_MAGAZZINO As Integer = 20
    Public Const TIPO_FABBRICATO_STALLA As Integer = 178
#End Region

    Public Const MAT_COD_ACQUA_IRRIGAZIONE As Integer = -1

#Region "SEMENTIERI"
    Public Const Is_Sementieri As String = "Is_Sementieri"
#End Region

    Public Shared ReadOnly Classificazioni_Tutti_Formulati As New List(Of Integer) From {101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 300, 1000, 610, 611, 200, 201, 202, 203, 400, 401, 402, 403, 404, 405, 407, 408, 409, 411, 412, 413, 414, 415, 418, 608, 603, 607, 500, 501, 502, 503, 504, 505, 506, 606, 614, 615, 602, 613, 617, 1003}

    Public Shared ReadOnly Classificazioni_Trattamento_Antiparassitario As New List(Of Integer) From {101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 300, 1000, 610, 611, 606}

    Public Shared ReadOnly Classificazioni_Diserbo As New List(Of Integer) From {200, 201, 202, 203}

    Public Shared ReadOnly Classificazioni_Trattamenti_Fitoregolatori As New List(Of Integer) From {400, 401, 402, 403, 404, 405, 407, 408, 409, 411, 412, 413, 414, 415, 418}

    Public Shared ReadOnly Classificazioni_Coadiuvanti_Bagnanti_Antischiuma As New List(Of Integer) From {500, 501, 502, 503, 504, 505, 506}

    Public Shared ReadOnly Classificazioni_Concia As New List(Of Integer) From {608}

    Public Shared ReadOnly Classificazioni_Disseccamento As New List(Of Integer) From {603}

    Public Shared ReadOnly Classificazioni_Geodisinfestazione As New List(Of Integer) From {607}

    Public Shared ReadOnly Classificazioni_Trattamento_Antiparassitario_Concianti As New List(Of Integer) From {101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 300, 1000, 610, 611, 608, 606}

    Public Shared ReadOnly Classificazioni_Diserbo_Disseccanti As New List(Of Integer) From {200, 201, 202, 203, 603}

    Public Shared ReadOnly Classificazioni_Trattamento_Antiparassitario_Geodisinfestanti As New List(Of Integer) From {101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 300, 1000, 610, 611, 607, 606}

    Public Shared ReadOnly Classificazioni_Confusione_Disorientamento_Sessuale As New List(Of Integer) From {614, 615}

    Public Shared ReadOnly Classificazioni_Installazione_Trappole_Catture_Massa As New List(Of Integer) From {602, 613, 617, 1003}

    Public Shared ReadOnly Classificazioni_Corroboranti_Fisiofarmaci As New List(Of Integer) From {300, 1000}

    Public Shared ReadOnly Classificazioni_Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci As New List(Of Integer) From {101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 610, 611, 608, 607, 606, 400, 401, 402, 403, 404, 405, 407, 408, 409, 411, 412, 413, 414, 415, 418, 500, 501, 502, 503, 504, 505, 506, 300, 1000}

    Public Shared ReadOnly Classificazioni_Confusione_Sessuale As New List(Of Integer) From {614}

    Public Shared ReadOnly Classificazioni_Disorientamento_Sessuale As New List(Of Integer) From {615}

    Public Shared ReadOnly DictionaryTipoFormulatoClassificazione As New Dictionary(Of Integer, List(Of Integer)) From {
        {enum_TipoFormulato.Tutti, Classificazioni_Tutti_Formulati},
        {enum_TipoFormulato.Antiparassitari, Classificazioni_Trattamento_Antiparassitario},
        {enum_TipoFormulato.Diserbanti, Classificazioni_Diserbo},
        {enum_TipoFormulato.Fitoregolatori, Classificazioni_Trattamenti_Fitoregolatori},
        {enum_TipoFormulato.Coadiuvanti, Classificazioni_Coadiuvanti_Bagnanti_Antischiuma},
        {enum_TipoFormulato.Concianti, Classificazioni_Concia},
        {enum_TipoFormulato.Disseccanti, Classificazioni_Disseccamento},
        {enum_TipoFormulato.Geodisinfestanti, Classificazioni_Geodisinfestazione},
        {enum_TipoFormulato.Antiparassitari_Concianti, Classificazioni_Trattamento_Antiparassitario_Concianti},
        {enum_TipoFormulato.Diserbanti_Disseccanti, Classificazioni_Diserbo_Disseccanti},
        {enum_TipoFormulato.Antiparassitari_Geodisinfestanti, Classificazioni_Trattamento_Antiparassitario_Geodisinfestanti},
        {enum_TipoFormulato.Corroboranti_Fisiofarmaci, Classificazioni_Corroboranti_Fisiofarmaci},
        {enum_TipoFormulato.Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci, Classificazioni_Antiparassitari_Concianti_Geodisinfestanti_Fitoregolatori_Coadiuvanti_CorroborantiFisiofarmaci},
        {enum_TipoFormulato.ConfusioneSessuale, Classificazioni_Confusione_Sessuale},
        {enum_TipoFormulato.DisorientamentoSessuale, Classificazioni_Disorientamento_Sessuale},
        {enum_TipoFormulato.ConfusioneDisorientamentoSessuale, Classificazioni_Confusione_Disorientamento_Sessuale},
        {enum_TipoFormulato.InstallazioneTrappoleCattureMassa, Classificazioni_Installazione_Trappole_Catture_Massa}
        }

    Public Shared ReadOnly DictionaryLavCodTipoFormulato As New Dictionary(Of Integer, List(Of Integer)) From {
        {LAVCOD_TRATTAMENTO_ANTIPARASSITARIO, New List(Of Integer) From {enum_TipoFormulato.Antiparassitari, enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci}},
        {LAVCOD_CONCIA_SEME, New List(Of Integer) From {enum_TipoFormulato.Concianti, enum_TipoFormulato.Coadiuvanti}},
        {LAVCOD_DISSECCAMENTO, New List(Of Integer) From {enum_TipoFormulato.Disseccanti, enum_TipoFormulato.Coadiuvanti}},
        {LAVCOD_GEODISINFESTAZIONE, New List(Of Integer) From {enum_TipoFormulato.Geodisinfestanti, enum_TipoFormulato.Coadiuvanti}},
        {LAVCOD_TRATTAMENTO_FITOREGOLATORE, New List(Of Integer) From {enum_TipoFormulato.Fitoregolatori, enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci}},
        {LAVCOD_DISERBO, New List(Of Integer) From {enum_TipoFormulato.Diserbanti, enum_TipoFormulato.Coadiuvanti}},
        {LAVCOD_CONFUSIONE_SESSUALE, New List(Of Integer) From {enum_TipoFormulato.ConfusioneSessuale, enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci}},
        {LAVCOD_DISORIENTAMENTO_SESSUALE, New List(Of Integer) From {enum_TipoFormulato.DisorientamentoSessuale, enum_TipoFormulato.Coadiuvanti, enum_TipoFormulato.Corroboranti_Fisiofarmaci}},
        {LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE, New List(Of Integer) From {enum_TipoFormulato.ConfusioneDisorientamentoSessuale}},
        {LAVCOD_TRATTAMENTO_POST_RACCOLTA, New List(Of Integer) From {enum_TipoFormulato.Tutti}},
        {LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA, New List(Of Integer) From {enum_TipoFormulato.InstallazioneTrappoleCattureMassa}},
        {LAVCOD_REINNESCO_TRAPPOLE, New List(Of Integer) From {enum_TipoFormulato.InstallazioneTrappoleCattureMassa}}
        }

    Public Shared SQUADRE_LAVORATORI_LIST As New List(Of Integer) From {
        CInt(enum_Rapporti_Contabili_Standard.Dipendente),
        CInt(enum_Rapporti_Contabili_Standard.Terzista),
        CInt(enum_Rapporti_Contabili_Standard.Tecnico),
        CInt(enum_Rapporti_Contabili_Standard.Centro_Revisione_Manutenzione_Macchine),
        CInt(enum_Rapporti_Contabili_Standard.Laboratorio_Analisi),
        CInt(enum_Rapporti_Contabili_Standard.Trasportatore),
        CInt(enum_Rapporti_Contabili_Standard.Tecnico_Responsabile),
        CInt(enum_Rapporti_Contabili_Standard.Referente_Aziendale),
        CInt(enum_Rapporti_Contabili_Standard.Spedizioniere),
        CInt(enum_Rapporti_Contabili_Standard.Operatore_Lab_Controllo_Qualita),
        CInt(enum_Rapporti_Contabili_Standard.Avventizio),
        CInt(enum_Rapporti_Contabili_Standard.Coadiuvante_Familiare),
        CInt(enum_Rapporti_Contabili_Standard.Veterinario),
        CInt(enum_Rapporti_Contabili_Standard.Trattorista),
        CInt(enum_Rapporti_Contabili_Standard.Dirigente),
        CInt(enum_Rapporti_Contabili_Standard.Impiegato_Amministrativo),
        CInt(enum_Rapporti_Contabili_Standard.Addetto_Punto_Vendita),
        CInt(enum_Rapporti_Contabili_Standard.Autista)
    }
End Class
