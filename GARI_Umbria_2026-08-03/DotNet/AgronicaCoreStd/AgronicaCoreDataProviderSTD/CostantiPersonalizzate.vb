Public Class CostantiPersonalizzate

    Public Const AGRODATAINIZIO = #01/01/1900#
    Public Const AGRODATAFINE = #12/31/2100#


    ' GRUPPO OPERAZIONE = 1 Rilievi in Campo

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

    ' GRUPPO OPERAZIONE = 2 Rilievi alla Raccolta

    Public Const LAVCOD_DANNI_RACCOLTA As Integer = 108
    Public Const LAVCOD_RILIEVO_INDICI_MATURITA As Integer = 109
    Public Const LAVCOD_RACCOLTA As Integer = 125
    Public Const LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA As Integer = 169

    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 3 Trattamenti

    Public Const LAVCOD_CONCIA_SEME As Integer = 13
    Public Const LAVCOD_DISERBO As Integer = 18
    Public Const LAVCOD_TRATTAMENTO_ANTIPARASSITARIO As Integer = 74
    Public Const LAVCOD_TRATTAMENTO_FITOREGOLATORE As Integer = 103
    Public Const LAVCOD_DISTRIBUZIONE_INSETTI As Integer = 116
    Public Const LAVCOD_CONFUSIONE_SESSUALE As Integer = 118
    Public Const LAVCOD_DISORIENTAMENTO_SESSUALE As Integer = 121
    Public Const LAVCOD_CATTURE_MASSA As Integer = 122
    Public Const LAVCOD_GEODISINFESTAZIONE As Integer = 155
    Public Const LAVCOD_DISSECCAMENTO As Integer = 158
    Public Const LAVCOD_TRATTAMENTO_POST_RACCOLTA As Integer = 163
    Public Const LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO As Integer = 165


    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 4 LAVORAZIONI

    Public Const LAVCOD_IRRIGAZIONE As Integer = 1

    Public Const LAVCOD_SEMINA As Integer = 2
    Public Const LAVCOD_TRAPIANTO As Integer = 71
    Public Const LAVCOD_TRAPIANTO_IN_SERRA As Integer = 151


    Public Const LAVCOD_ARATURA As Integer = 8
    Public Const LAVCOD_ANDANAMENTO As Integer = 9

    Public Const LAVCOD_FERTIRRIGAZIONE As Integer = 26
    Public Const LAVCOD_DISTRIBUZIONE_CONCIME As Integer = 14
    Public Const LAVCOD_CONCIMAZIONE_FOGLIARE As Integer = 123
    Public Const LAVCOD_DISTRIBUZIONE_AMMENDANTI As Integer = 124
    Public Const LAVCOD_SARCHIATURA_CONCIMAZIONE As Integer = 156
    Public Const LAVCOD_TRATTAMENTO_ANTIBUTTERATURA As Integer = 106
    Public Const LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO As Integer = 166

    Public Const LAVCOD_MANUTENZIONE_IMPIANTI As Integer = 159

    Public Const LAVCOD_SOVESCIO As Integer = 160

    Public Const LAVCOD_RILIEVO_ATTIVITA As Integer = 162

    Public Const LAVCOD_ASPORTAZIONE_ORGANI_INFETTI As Integer = 120
    Public Const LAVCOD_ASSOLCATURA As Integer = 10
    Public Const LAVCOD_CARICO_MANUALE_FRUTTA As Integer = 78
    Public Const LAVCOD_CIMATURA As Integer = 12
    Public Const LAVCOD_DIRADAMENTO_MANUALE As Integer = 17
    Public Const LAVCOD_DISSODAMENTO As Integer = 19
    Public Const LAVCOD_ERPICATURA As Integer = 21
    Public Const LAVCOD_ERPICATURA_ROTANTE As Integer = 157
    Public Const LAVCOD_ESPIANTO As Integer = 81
    Public Const LAVCOD_ESTIRPATURA As Integer = 22
    Public Const LAVCOD_FALCIACONDIZIONATURA As Integer = 23
    Public Const LAVCOD_FALCIATURA_ERBAI As Integer = 25
    Public Const LAVCOD_FORMAZIONE_ARGINELLI As Integer = 27
    Public Const LAVCOD_FRANGIZOLLATURA As Integer = 31
    Public Const LAVCOD_FRESATURA As Integer = 32
    Public Const LAVCOD_GEBIATURA As Integer = 152
    Public Const LAVCOD_IMBALLO_FIENO_ROTOLI As Integer = 33
    Public Const LAVCOD_INTERRAMENTO_PAGLIE As Integer = 34
    Public Const LAVCOD_INTERVENTO_ANTIBRINA As Integer = 161
    Public Const LAVCOD_LAVORAZIONE_CONBINATA As Integer = 154
    Public Const LAVCOD_LAVORAZIONE_TRA_FILA As Integer = 82
    Public Const LAVCOD_LAVORAZIONE_SU_FILA As Integer = 83
    Public Const LAVCOD_LEGATURA As Integer = 84
    Public Const LAVCOD_LIVELLAMENTO As Integer = 40
    Public Const LAVCOD_MANUTENZIONE_ARGINI As Integer = 41
    Public Const LAVCOD_MESSA_DIMORA_PIANTE As Integer = 85
    Public Const LAVCOD_MIETITREBBIATURA As Integer = 46
    Public Const LAVCOD_MINIMUM_TILLAGE As Integer = 47
    Public Const LAVCOD_PACCIAMATURA As Integer = 48
    Public Const LAVCOD_POTATURA_SECCA As Integer = 87
    Public Const LAVCOD_POTATURA_VERDE As Integer = 88
    Public Const LAVCOD_PRESSATURA As Integer = 49
    Public Const LAVCOD__RACCOLTA_LEGNA_POTATURA As Integer = 90
    Public Const LAVCOD_RANGHINATURA As Integer = 53
    Public Const LAVCOD_RINCALZATURA As Integer = 55
    Public Const LAVCOD_RIPPATURA As Integer = 115
    Public Const LAVCOD_RIPUNTATURA As Integer = 56
    Public Const LAVCOD_RIVOLTAMENTO_FORAGGIO As Integer = 57
    Public Const LAVCOD_ROMPICROSTA As Integer = 153
    Public Const LAVCOD_RULLATURA As Integer = 58
    Public Const LAVCOD_SARCHIATURA As Integer = 59
    Public Const LAVCOD_SCARIFICATURA As Integer = 62
    Public Const LAVCOD_SCASSO As Integer = 63
    Public Const LAVCOD_SOD_SEDDING As Integer = 68
    Public Const LAVCOD_TRINCIATURA As Integer = 75
    Public Const LAVCOD_VANGATURA As Integer = 76
    Public Const LAVCOD_ZAPPATURA As Integer = 77
    Public Const LAVCOD_ALTRE_OPERAZIONI As Integer = 162
    Public Const LAVCOD_PIRODISERBO As Integer = 168
    Public Const LAVCOD_CONFUSIONE_DISORIENTAMENTO_SESSUALE As Integer = 172
    Public Const LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA As Integer = 173


    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 5 Altre Operazioni Colturali(non gestite)



    '------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 6 (tutte)

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


    '------------------------------------------------------------

    'il 5000 è Trasformazioni e non è associato ad alcun gruppo
    Public Const LAVCOD_TRASFORMAZIONI As Integer = 5000
    Public Const LAVCOD_CURA As Integer = 5004
    '------------------------------------------------------------

    'GRUPPO OPERAZIONE = 7 (tutte)

    Public Const LAVCOD_ACQUISTO_BENI As Integer = 1008
    Public Const LAVCOD_CESSIONE_BENI As Integer = 1009

    Public Const LAVCOD_REG_AMMORTAMENTI As Integer = 1010
    Public Const LAVCOD_FINE_AMMORTAMENTI As Integer = 1011

    '-------------------------------------------------------------

    'GRUPPO OPERAZIONE = 8 (tutte)

    Public Const LAVCOD_PAGAMENTO_FATTURA As Integer = 1012
    Public Const LAVCOD_INCASSO_FATTURA As Integer = 1013

    Public Const LAVCOD_PAGAMENTI_DIVERSI As Integer = 1014
    Public Const LAVCOD_INCASSI_DIVERSI As Integer = 1015

    Public Const LAVCOD_PAGAMENTO_RATA_PRESTITO As Integer = 1024

    '--------------------------------------------------------------

    'GRUPPO OPERAZIONE = 9 (tutte)

    Public Const LAVCOD_SCADENZA_FATTURA_EMESSA As Integer = 1017
    Public Const LAVCOD_SCADENZA_FATTURA_RICEVUTA As Integer = 1016

    Public Const LAVCOD_SCADENZA_PAGAMENTI_DIVERSI As Integer = 1018
    Public Const LAVCOD_SCADENZA_INCASSI_DIVERSI As Integer = 1019

    '--------------------------------------------------------------

    'GRUPPO OPERAZIONE = 10 (tutte)

    Public Const LAVCOD_CARICO As Integer = 1022
    Public Const LAVCOD_SCARICO As Integer = 1023

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

    Public Const LAVCOD_DOCO_EMESSO As Integer = 1061
    Public Const LAVCOD_DOCO_RICEVUTO As Integer = 1062

    Public Const LAVCOD_DAA_EMESSO As Integer = 1063

    Public Const LAVCOD_MVV_EMESSO As Integer = 1071
    Public Const LAVCOD_MVV_RICEVUTO As Integer = 1072


    Public Const LAVCOD_ALTRI_RICAVI_NERO As Integer = 1073
    Public Const LAVCOD_ALTRI_COSTI_NERO As Integer = 1074
    Public Const LAVCOD_DISTINTA_CARICO As Integer = 1075
    Public Const LAVCOD_DISTINTA_CARICO_ACCETTAZIONE As Integer = 1076
    Public Const LAVCOD_AUTO_DDT_EMESSO As Integer = 1077
    Public Const LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE As Integer = 1078
    Public Const LAVCOD_GESTIONE_RIFIUTI As Integer = 1080

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 11 (tutte)

    Public Const LAVCOD_NOTE As Integer = 2000
    Public Const LAVCOD_RATE As Integer = 2001
    Public Const LAVCOD_ORDINE_VENDITA As Integer = 2002
    Public Const LAVCOD_ORDINE_ACQUISTO As Integer = 2004
    Public Const LAVCOD_PREVENTIVO_VENDITA As Integer = 2003

    '------------------------------------------------------------------


    'GRUPPO OPERAZIONE = 12 (tutte)

    Public Const LAVCOD_ANALISI_LATTE_SINGOLA As Integer = 3006
    Public Const LAVCOD_ANALISI_LATTE_MASSA As Integer = 3007

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 13 (tutte)

    Public Const LAVCOD_NASCITA_ANIMALI As Integer = 3000

    Public Const LAVCOD_INCREMENTO_CONSISTENZE_ZOO As Integer = 3001
    Public Const LAVCOD_DECREMENTO_CONSISTENZE_ZOO As Integer = 3002

    Public Const LAVCOD_MORTE_ANIMALI As Integer = 3003
    Public Const LAVCOD_MACELLAZIONE_ANIMALI As Integer = 3004

    Public Const LAVCOD_SOSTITUZIONE_MARCA As Integer = 3005

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 14 Rilievi produzioni

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 15 Gestione mungitura

    Public Const LAVCOD_MUNGITURA_PREPARAZIONE = 3009
    Public Const LAVCOD_MUNGITURA_SECCHIO_POSTA = 3010
    Public Const LAVCOD_MUNGITURA_GRUPPI_POSTA = 3011
    Public Const LAVCOD_MUNGITURA_SALA_LATTE = 3012
    Public Const LAVCOD_MUNGITURA_LAVAGGIO_IMPIANTI = 3013
    Public Const LAVCOD_MUNGITURA_LAVAGGIO_SALA_LATTE = 3014

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 16 Gestione lettiere

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 17 Gestione alimentazione

    Public Const LAVCOD_ALIMENTAZIONE_PULIZIA_IMPIANTI = 3019
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_FORAGGI = 3020
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_FORAGGI = 3021
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MANUALE_MANGIMI = 3022
    Public Const LAVCOD_ALIMENTAZIONE_DISTRIBUZIONE_MECCANICA_MANGIMI = 3023
    Public Const LAVCOD_ALIMENTAZIONE_CONTROLLO_REGOLAZIONE_SISTEMI = 3024

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 18 Altri lavori di stalla

    '------------------------------------------------------------------

    ' GRUPPO OPERAZIONE = 19 MACCHINE

    Public Const LAVCOD_MANUTENZIONE_MACCHINE As Integer = 1500
    Public Const LAVCOD_REVISIONE_MACCHINE As Integer = 4000

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 20 Gestione Visite Ispettive

    Public Const LAVCOD_MONITORAGGIO_TEMPI_RIENTRO As Integer = 5001
    Public Const LAVCOD_VISITA_GENERICA As Integer = 5002
    Public Const LAVCOD_MONITORAGGIO_CE As Integer = 5003

    Public Const LAVCOD_PRATICA_ECOLOGICA As Integer = 5005
    Public Const LAVCOD_FORMAZIONE As Integer = 5006
    Public Const LAVCOD_VISITA As Integer = 5007

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 100 Linee Produzione Vegetale

    '------------------------------------------------------------------

    'GRUPPO OPERAZIONE = 101 Linee Produzione Animale

    '------------------------------------------------------------

    '#####################################################################




    '#################################
    '##### CAUSALI/PERMESSI  #########
    '#################################

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

    'Operazioni Contabili
    Public Const CAU_REGISTRAZIONI As String = "4000"
    Public Const CAU_REGISTRAZIONI_ALLEGATE As String = "4050"
    Public Const CAU_REGISTRAZIONE_SECONDARIA As String = "4050"
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

    '=============================================================



    Public Const AgroSequenzeBase0 As Integer = 0

    Public Const AgroSequenzeEndUpperBound As Integer = 2000000000

    Public Const UserDefaultWidgets As String = "-1"

End Class
