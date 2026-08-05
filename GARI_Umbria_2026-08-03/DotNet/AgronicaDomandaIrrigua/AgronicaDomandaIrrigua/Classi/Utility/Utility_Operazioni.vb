Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModello.ParametriAgenda_Temp
Imports AgronicaCoreModello.OperazioneAgenda_Temp
Imports AgronicaCoreDataProvider
Imports System.Web
Imports AgronicaCoreXML.XML_Stampe
Imports AgronicaCoreGestioneRichieste

Namespace Utility_NS

    Public Class Utility_Operazioni

        '#Region "Link"

        '        '##############################################################
        '        Public Function LinkPagina_from_LavCod(ByVal Lav_Cod As Integer,
        '                                                 Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True) As String

        '            Dim PaginaLink As String = ""

        '            Select Case Lav_Cod

        '                '---------------------------------------
        '                'FERTILIZZAZIONE
        '                Case LAVCOD_FERTIRRIGAZIONE,
        '                    LAVCOD_CONCIMAZIONE_FOGLIARE,
        '                    LAVCOD_DISTRIBUZIONE_CONCIME,
        '                    LAVCOD_DISTRIBUZIONE_AMMENDANTI,
        '                    LAVCOD_SARCHIATURA_CONCIMAZIONE,
        '                    LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

        '                    If LeggiFlagConfigurazioneSiti = True Then
        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Fertilizzazione.aspx"
        '                        End If

        '                    Else
        '                        PaginaLink = "../Operazioni/Fertilizzazione.aspx"
        '                    End If


        '                    '---------------------------------------
        '                    'TRATTAMENTI
        '                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
        '                    LAVCOD_DISERBO,
        '                    LAVCOD_DISSECCAMENTO,
        '                    LAVCOD_GEODISINFESTAZIONE,
        '                    LAVCOD_CONCIA_SEME,
        '                    LAVCOD_TRATTAMENTO_FITOREGOLATORE

        '                    If LeggiFlagConfigurazioneSiti = True Then
        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Trattamenti.aspx"
        '                        End If
        '                    Else
        '                        PaginaLink = "../Operazioni/Trattamenti.aspx"
        '                    End If

        '                    'If Not IsNothing(ConfigurationSettings.AppSettings("agenda_boot")) AndAlso ConfigurationSettings.AppSettings("agenda_boot") = True Then
        '                    '    PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                    'Else
        '                    '    PaginaLink = "../Operazioni/Trattamenti.aspx"
        '                    'End If

        '                    '---------------------------------------

        '                    '---------------------------------------
        '                    'DISTRIBUZIONE INSETTI
        '                Case LAVCOD_DISTRIBUZIONE_INSETTI
        '                    PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"
        '                    '---------------------------------------

        '                    '---------------------------------------
        '                    'LAVORAZIONi
        '                Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, 120, 10,
        '                    78, 12, 17, 19, 21, 22, 81, 23, 25, 27, 31, 32, 33,
        '                    34, 82, 83, 84, 40, 41, 85, 46, 47, 48, 87, 88, 49, 90, 91, 92, 53, 55, 115, 56,
        '                    57, 58, 59, 62, 63, 75, 76, 77, 152, 153, 154, 157, 161, 167, 168
        '                    'Andanamento, Aratura, Asportazione Organi Infetti, Assolcatura ,Carico Manuale Frutta, Cimatura
        '                    'Diradamento Manuale, Dissodamento, Erpicatura, Erstirpatura, Espianto, Falciacondizionatura, Falciatura erbai
        '                    'Frangizollatura, Fresatura, Imballo fieno e rotoli, Interramento paglie, Lavorazione tra fila
        '                    'Lavorazione su fila, Legatura, Livellamento, Manutenzione argini, Messa dimora piante, Mietitrebbiatura
        '                    'Minimum tillage, Pacciamatura , Potatura secca, Potatura verde, Pressatura, Raccolta legna potatura
        '                    'Raccolta manuale, Raccolta meccanica, Ranghinatura, Rincalzatura, Rippatura, Ripuntatura
        '                    'Rivoltaggio foraggio, Rullatura, Sarchiatura, Scarificatura, Scasso, Sod sedding
        '                    'Trinciatura, Vangatura, Zappatura, Gebiatura, Rompicrosta, Lavorazione Combinata,Erpicatura Rotante, Strigliatura, pirodiserbo

        '                    If LeggiFlagConfigurazioneSiti = True Then
        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Lavorazioni.aspx"
        '                        End If
        '                    Else
        '                        PaginaLink = "../Operazioni/Lavorazioni.aspx"
        '                    End If

        '                    '---------------------------------------


        '                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
        '                    LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
        '                    PaginaLink = "../Operazioni/Installazione_Trappole.aspx"

        '                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
        '                    PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx" '"../Classi/Agro_Pages/Agenda_Pages/Operazioni_Pages/Reinnesco_Trappole/Reinnesco_Trappole.aspx"


        '                Case LAVCOD_RILIEVO_AVVERSITA_CAMPO, LAVCOD_VISITA

        '                    If LeggiFlagConfigurazioneSiti = True Then

        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/RilieviBS.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Rilievi.aspx"
        '                        End If
        '                    Else

        '                        PaginaLink = "../Operazioni/Rilievi.aspx"

        '                    End If


        '                Case LAVCOD_RILIEVO_INDICI_MATURITA,
        '                        LAVCOD_RILIEVO_ERBE_INFESTANTI,
        '                     LAVCOD_FASI_FENOLOGICHE

        '                    PaginaLink = "../Operazioni/Rilievi.aspx"

        '                    '---------------------------------------
        '                    'Irrigazione
        '                Case LAVCOD_IRRIGAZIONE
        '                    PaginaLink = "../Operazioni/Irrigazione.aspx"

        '                    '---------------------------------------
        '                    'RILIEVO_PIOGGE
        '                Case LAVCOD_RILIEVO_PIOGGE
        '                    PaginaLink = "../Operazioni/RilievoPiogge.aspx"


        '                    '---------------------------------------
        '                    'LAVCOD_SEMINA, LAVCOD_TRAPIANTO
        '                Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
        '                    PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"

        '                Case LAVCOD_CURA
        '                    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"
        '                    '---------------------------------------

        '                Case LAVCOD_GESTIONE_RIFIUTI
        '                    PaginaLink = "../Operazioni/GestioneRifiuti.aspx"
        '                    '---------------------------------------

        '                Case Else
        '                    PaginaLink = ""

        '            End Select

        '            Return PaginaLink

        '        End Function

        '        '##############################################################
        '        Public Function LinkPagina_from_LavCod(ByVal Lav_Cod As Integer,
        '                                               ByRef objParametriAgenda As ParametriAgenda,
        '                                               Optional ByVal PaginaSitoAgendaOrigine As enum_PagineAgenda_2010 = enum_PagineAgenda_2010.Menu,
        '                                               Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True) As String

        '            Dim PaginaLink As String = ""

        '            If PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Menu Then
        '                Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        '                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "MenuAgendaBS", "", "", objParametriServer)
        '                If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                    PaginaSitoAgendaOrigine = enum_PagineAgenda_2010.Menu_BS
        '                End If
        '            End If

        '            Select Case Lav_Cod

        '                Case LAVCOD_PROCEDURA_LIQUIDAZIONE_SOCI
        '                    PaginaLink = "../GestioneContabilita/Liquidazione/Liquidazione.aspx"

        '                Case LAVCOD_SCARICO, LAVCOD_CARICO, LAVCOD_VENDITA, LAVCOD_ACQUISTO, LAVCOD_TRASFERIMENTO
        '                    PaginaLink = "../GestioneMagazzini/FormProdotto.aspx"

        '                    Dim xChiave As String = ""
        '                    Dim mode As String = ""
        '                    Dim caricoScarico As String = ""
        '                    Call Albero.ChiaveAlbero_Codifica(xChiave,
        '                               enum_TipoNodo.x_GiacenzeMagazzino,
        '                                objParametriAgenda.Piva, objParametriAgenda.Sa_Cod, , , , , , , , , , , , , , , , , , )

        '                    If Lav_Cod = LAVCOD_TRASFERIMENTO Then
        '                        caricoScarico = "T"
        '                        mode = "trasferimento"
        '                    End If
        '                    If Lav_Cod = LAVCOD_SCARICO Or Lav_Cod = LAVCOD_CARICO Then

        '                        If Lav_Cod = LAVCOD_CARICO Then
        '                            caricoScarico = "C"
        '                        ElseIf Lav_Cod = LAVCOD_SCARICO Then
        '                            caricoScarico = "S"
        '                        End If
        '                        mode = "magazzino"

        '                    End If
        '                    If Lav_Cod = LAVCOD_VENDITA Or Lav_Cod = LAVCOD_ACQUISTO Then

        '                        If Lav_Cod = LAVCOD_ACQUISTO Then
        '                            caricoScarico = "C"
        '                        ElseIf Lav_Cod = LAVCOD_VENDITA Then
        '                            caricoScarico = "S"
        '                        End If
        '                        mode = "compravendita"

        '                    End If

        '                    PaginaLink = PaginaLink &
        '                                    "?k=" & Stringa_Codifica(xChiave, AgroKey_EncoderDecoder) &
        '                                    "&c=" & Stringa_Codifica(caricoScarico, AgroKey_EncoderDecoder) &
        '                                    "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
        '                                    "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder) &
        '                                    "&mode=" & Stringa_Codifica(mode, AgroKey_EncoderDecoder) &
        '                                    "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) &
        '                                    "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) &
        '                                    "&s=" & Stringa_Codifica(objParametriAgenda.Sa_Cod, AgroKey_EncoderDecoder) &
        '                                    "&a=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder)


        '                    '---------------------------------------

        '                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA, LAVCOD_BOLLA_RICEVUTA, LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_NOTA_ACCREDITO_EMESSA
        '                    PaginaLink = "../GestioneContabilita/DocumentoContabileGenerico.aspx"
        '                    Dim tipo As Integer = 0
        '                    'objParametriAgenda.Sa_Cod=0 per tutti i doc contabili
        '                    PaginaLink &=
        '                            "?p=" & Stringa_Codifica(objParametriAgenda.Piva, AgroKey_EncoderDecoder) &
        '                            "&o=" & Stringa_Codifica(objParametriAgenda.Tipo_Operazione, AgroKey_EncoderDecoder) &
        '                            "&s=" & Stringa_Codifica(0, AgroKey_EncoderDecoder) &
        '                            "&i=" & Stringa_Codifica(objParametriAgenda.Id_Agenda, AgroKey_EncoderDecoder) +
        '                            "&l=" & Stringa_Codifica(Lav_Cod, AgroKey_EncoderDecoder) +
        '                            "&d=" & Stringa_Codifica(CStr(objParametriAgenda.Data), AgroKey_EncoderDecoder) +
        '                            "&tf=" & Stringa_Codifica(tipo, AgroKey_EncoderDecoder) +
        '                            "&rs=" & Stringa_Codifica(objParametriAgenda.RagSoc, AgroKey_EncoderDecoder) &
        '                            "&orig=" & Stringa_Codifica(PaginaSitoAgendaOrigine, AgroKey_EncoderDecoder)


        '                    '---------------------------------------



        '                    '---------------------------------------
        '                    'FERTILIZZAZIONE
        '                Case LAVCOD_FERTIRRIGAZIONE,
        '                        LAVCOD_CONCIMAZIONE_FOGLIARE,
        '                        LAVCOD_DISTRIBUZIONE_CONCIME,
        '                        LAVCOD_DISTRIBUZIONE_AMMENDANTI,
        '                        LAVCOD_SARCHIATURA_CONCIMAZIONE,
        '                        LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

        '                    If LeggiFlagConfigurazioneSiti = True Then

        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Fertilizzazione.aspx"
        '                        End If

        '                    Else

        '                        PaginaLink = "../Operazioni/Fertilizzazione.aspx"

        '                    End If


        '                    '---------------------------------------


        '                    '---------------------------------------
        '                    'TRATTAMENTI
        '                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
        '                    LAVCOD_DISERBO,
        '                    LAVCOD_DISSECCAMENTO,
        '                    LAVCOD_GEODISINFESTAZIONE,
        '                    LAVCOD_CONCIA_SEME,
        '                    LAVCOD_TRATTAMENTO_FITOREGOLATORE

        '                    If LeggiFlagConfigurazioneSiti = True Then

        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Trattamenti.aspx"
        '                        End If

        '                    Else

        '                        PaginaLink = "../Operazioni/Trattamenti.aspx"

        '                    End If



        '                    '---------------------------------------
        '                    'DISTRIBUZIONE INSETTI
        '                Case LAVCOD_DISTRIBUZIONE_INSETTI
        '                    PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                    'LAVORAZIONI
        '                Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, LAVCOD_ASPORTAZIONE_ORGANI_INFETTI, LAVCOD_ASSOLCATURA,
        '                     LAVCOD_CARICO_MANUALE_FRUTTA, LAVCOD_CIMATURA, LAVCOD_DIRADAMENTO_MANUALE, LAVCOD_DISSODAMENTO, LAVCOD_ERPICATURA, LAVCOD_ESTIRPATURA, LAVCOD_ESPIANTO,
        '                     LAVCOD_FALCIACONDIZIONATURA, LAVCOD_FALCIATURA_ERBAI, LAVCOD_FORMAZIONE_ARGINELLI, LAVCOD_FRANGIZOLLATURA, LAVCOD_FRESATURA, LAVCOD_IMBALLO_FIENO_ROTOLI,
        '                     LAVCOD_INTERRAMENTO_PAGLIE, LAVCOD_LAVORAZIONE_TRA_FILA, LAVCOD_LAVORAZIONE_SU_FILA, LAVCOD_LEGATURA, LAVCOD_LIVELLAMENTO, LAVCOD_MANUTENZIONE_ARGINI,
        '                     LAVCOD_MESSA_DIMORA_PIANTE, LAVCOD_MIETITREBBIATURA, LAVCOD_MINIMUM_TILLAGE, LAVCOD_PACCIAMATURA, LAVCOD_POTATURA_SECCA, LAVCOD_POTATURA_VERDE, LAVCOD_PRESSATURA,
        '                     LAVCOD__RACCOLTA_LEGNA_POTATURA, LAVCOD_STRIGLIATURA, LAVCOD_PIRODISERBO, LAVCOD_RANGHINATURA, LAVCOD_RINCALZATURA, LAVCOD_RIPPATURA, LAVCOD_RIPUNTATURA,
        '                     LAVCOD_RIVOLTAMENTO_FORAGGIO, LAVCOD_RULLATURA, LAVCOD_SARCHIATURA, LAVCOD_SCARIFICATURA, LAVCOD_SCASSO, LAVCOD_TRINCIATURA,
        '                     LAVCOD_VANGATURA, LAVCOD_ZAPPATURA, LAVCOD_GEBIATURA, LAVCOD_ROMPICROSTA, LAVCOD_LAVORAZIONE_CONBINATA, LAVCOD_ERPICATURA_ROTANTE, LAVCOD_INTERVENTO_ANTIBRINA,
        '                     LAVCOD_ALTRE_OPERAZIONI, 167, 168

        '                    If LeggiFlagConfigurazioneSiti = True Then

        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Lavorazioni.aspx"
        '                        End If

        '                    Else

        '                        PaginaLink = "../Operazioni/Lavorazioni.aspx"

        '                    End If

        '                    '---------------------------------------


        '                    '---------------------------------------
        '                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
        '                    LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
        '                    PaginaLink = "../Operazioni/Installazione_Trappole.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
        '                    PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx"
        '                    '---------------------------------------


        '                Case LAVCOD_RILIEVO_INDICI_MATURITA, LAVCOD_RILIEVO_AVVERSITA_CAMPO


        '                    If LeggiFlagConfigurazioneSiti = True Then

        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        Dim dtConfigSiti As DataTable = objConfigSiti.Leggi(0, "RilieviBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/RilieviBS.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Rilievi.aspx"
        '                        End If

        '                    Else
        '                        PaginaLink = "../Operazioni/Rilievi.aspx"

        '                    End If


        '                Case LAVCOD_VISITA, LAVCOD_DANNI_RACCOLTA

        '                    PaginaLink = "../Operazioni/RilieviBS.aspx"

        '                Case LAVCOD_RILIEVO_ERBE_INFESTANTI, LAVCOD_FASI_FENOLOGICHE

        '                    PaginaLink = "../Operazioni/Rilievi.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                Case LAVCOD_IRRIGAZIONE
        '                    PaginaLink = "../Operazioni/Irrigazione.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                Case LAVCOD_RILIEVO_PIOGGE
        '                    PaginaLink = "../Operazioni/RilievoPiogge.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
        '                    PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                Case LAVCOD_RACCOLTA


        '                    'gestione raccolta new
        '                    Try
        '                        If Not IsNothing(HttpContext.Current.Session("ASG_objParametri_Server")) Then
        '                            Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))
        '                            Dim cf As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                            Dim dt As DataTable = cf.Leggi(0, "RaccoltaNew", " valore = 'true' ", "", objParametriServer)
        '                            If dt.Rows.Count = 1 Then
        '                                'raccolta new
        '                                PaginaLink = "../Operazioni/Raccolta.aspx"
        '                                Return PaginaLink
        '                            Else
        '                                'raccolta 2003
        '                            End If


        '                        End If
        '                    Catch ex As Exception
        '                        'raccolta 2003
        '                    End Try

        '                    If objParametriAgenda.Sa_Cod = "0" Or objParametriAgenda.Sa_Cod = "" Then
        '                        Return ""
        '                    End If

        '                    Dim objGiasOnline As New ParametriGiasOnline With {
        '                        .Cul_Cod = objParametriAgenda.Cul_Cod,
        '                        .DataSelezionata = objParametriAgenda.Data,
        '                        .Id_Agenda = objParametriAgenda.Id_Agenda,
        '                        .Lavorazione = LAVCOD_RACCOLTA,
        '                        .Operazione = objParametriAgenda.Tipo_Operazione,
        '                        .PaginaRichiesta = enum_PagineGiasOnline.Agenda_Raccolta,
        '                        .Piva = objParametriAgenda.Piva,
        '                        .Sa_Cod = objParametriAgenda.Sa_Cod
        '                    }
        '                    Dim specie As Integer = 0
        '                    If IsNumeric(objParametriAgenda.Veg_Cod.Split("/")(0)) AndAlso CInt(objParametriAgenda.Veg_Cod.Split("/")(0)) > 0 Then
        '                        specie = CInt(objParametriAgenda.Veg_Cod.Split("/")(0))
        '                    End If
        '                    objGiasOnline.Veg_Cod = specie


        '                    PaginaLink = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(
        '                                                        Enum_SiteRedirector.Sito_AgronicaAgenda_2010,
        '                                                        objGiasOnline)

        '                    '---------------------------------------

        '                Case LAVCOD_TRATTAMENTO_POST_RACCOLTA
        '                    PaginaLink = "../Operazioni/Trattamenti_PostRaccolta.aspx"


        '                Case LAVCOD_FERTILIZZAZIONI_DICHIARAZIONE_NON_UTILIZZO, LAVCOD_TRATTAMENTO_DICHIARAZIONE_NON_UTILIZZO
        '                    PaginaLink = "../Operazioni/NonUtilizzo.aspx"

        '                    '---------------------------------------
        '                Case LAVCOD_CURA
        '                    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"
        '                    '---------------------------------------

        '                Case LAVCOD_MANUTENZIONE_MACCHINE, LAVCOD_REVISIONE_MACCHINE
        '                    PaginaLink = "../Operazioni/ManutenzioneMacchine.aspx"
        '                    '---------------------------------------
        '                Case LAVCOD_GESTIONE_RIFIUTI
        '                    PaginaLink = "../Operazioni/GestioneRifiuti.aspx"
        '                    '---------------------------------------

        '                Case Else
        '                    PaginaLink = ""

        '            End Select

        '            Return PaginaLink

        '        End Function


        '        Function LinkPagina_from_LavCod(Operazione_Colturale_Generica As AgronicaCoreModello.Agenda.Operazioni_Colturali.Operazione_Colturale.I_Operazione_Colturale,
        '                                               Optional ByVal LeggiFlagConfigurazioneSiti As Boolean = True) As String
        '            Dim PaginaLink As String = ""

        '            Select Case Operazione_Colturale_Generica.Lav_Cod

        '                '---------------------------------------
        '                'FERTILIZZAZIONE
        '                Case LAVCOD_FERTIRRIGAZIONE,
        '                    LAVCOD_CONCIMAZIONE_FOGLIARE,
        '                    LAVCOD_DISTRIBUZIONE_CONCIME,
        '                    LAVCOD_DISTRIBUZIONE_AMMENDANTI,
        '                    LAVCOD_SARCHIATURA_CONCIMAZIONE,
        '                    LAVCOD_TRATTAMENTO_ANTIBUTTERATURA

        '                    If LeggiFlagConfigurazioneSiti = True Then
        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Fertilizzazione.aspx"
        '                        End If
        '                    Else
        '                        PaginaLink = "../Operazioni/Fertilizzazione.aspx"
        '                    End If


        '                    '---------------------------------------
        '                    'TRATTAMENTI
        '                Case LAVCOD_TRATTAMENTO_ANTIPARASSITARIO,
        '                    LAVCOD_DISERBO,
        '                    LAVCOD_DISSECCAMENTO,
        '                    LAVCOD_GEODISINFESTAZIONE,
        '                    LAVCOD_CONCIA_SEME,
        '                    LAVCOD_TRATTAMENTO_FITOREGOLATORE

        '                    If LeggiFlagConfigurazioneSiti = True Then

        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Trattamenti.aspx"
        '                        End If
        '                    Else
        '                        PaginaLink = "../Operazioni/Trattamenti.aspx"
        '                    End If


        '                    '---------------------------------------
        '                    'DISTRIBUZIONE INSETTI
        '                Case LAVCOD_DISTRIBUZIONE_INSETTI
        '                    PaginaLink = "../Operazioni/Distribuzione_Insetti.aspx"
        '                    '---------------------------------------


        '                    '---------------------------------------
        '                    'LAVORAZIONi
        '                Case LAVCOD_ANDANAMENTO, LAVCOD_ARATURA, 120, 10,
        '                    78, 12, 17, 19, 21, 22, 81, 23, 25, 27, 31, 32, 33,
        '                    34, 82, 83, 84, 40, 41, 85, 46, 47, 48, 87, 88, 49, 90, 91, 92, 53, 55, 115, 56,
        '                    57, 58, 59, 62, 63, 75, 76, 77, 152, 153, 154, 157, 161, 167, 168
        '                    'Andanamento, Aratura, Asportazione Organi Infetti, Assolcatura ,Carico Manuale Frutta, Cimatura
        '                    'Diradamento Manuale, Dissodamento, Erpicatura, Erstirpatura, Espianto, Falciacondizionatura, Falciatura erbai
        '                    'Frangizollatura, Fresatura, Imballo fieno e rotoli, Interramento paglie, Lavorazione tra fila
        '                    'Lavorazione su fila, Legatura, Livellamento, Manutenzione argini, Messa dimora piante, Mietitrebbiatura
        '                    'Minimum tillage, Pacciamatura , Potatura secca, Potatura verde, Pressatura, Raccolta legna potatura
        '                    'Raccolta manuale, Raccolta meccanica, Ranghinatura, Rincalzatura, Rippatura, Ripuntatura
        '                    'Rivoltaggio foraggio, Rullatura, Sarchiatura, Scarificatura, Scasso, Sod sedding
        '                    'Trinciatura, Vangatura, Zappatura, Gebiatura, Rompicrosta, Lavorazione Combinata,Erpicatura Rotante, Strigliatura, pirodiserbo

        '                    If LeggiFlagConfigurazioneSiti = True Then
        '                        Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
        '                        Dim dtConfigSiti As DataTable
        '                        Dim objParametriServer As New AgronicaCoreParametri(HttpContext.Current.Session("ASG_objParametri_Server"))

        '                        dtConfigSiti = objConfigSiti.Leggi(0, "OperazioniAgendaBS", "", "", objParametriServer)

        '                        If Not IsNothing(dtConfigSiti) AndAlso dtConfigSiti.Rows.Count > 0 AndAlso LCase(dtConfigSiti.Rows(0).Item("Valore")) = "true" Then
        '                            PaginaLink = "../Operazioni/Trattamenti_2.aspx"
        '                        Else
        '                            PaginaLink = "../Operazioni/Lavorazioni.aspx"
        '                        End If
        '                    Else
        '                        PaginaLink = "../Operazioni/Lavorazioni.aspx"
        '                    End If

        '                    '---------------------------------------


        '                Case LAVCOD_INSTALLAZIONE_TRAPPOLE, LAVCOD_CATTURE_MASSA,
        '                    LAVCOD_CONFUSIONE_SESSUALE, LAVCOD_DISORIENTAMENTO_SESSUALE
        '                    PaginaLink = "../Operazioni/Installazione_Trappole.aspx"

        '                Case LAVCOD_REINNESCO_TRAPPOLE, LAVCOD_RILIEVO_AVVERSITA_TRAPPOLE
        '                    PaginaLink = "../Operazioni/Reinnesco_Rilievi_Trappole.aspx" '"../Classi/Agro_Pages/Agenda_Pages/Operazioni_Pages/Reinnesco_Trappole/Reinnesco_Trappole.aspx"

        '                    '---------------------------------------
        '                    'Irrigazione
        '                Case LAVCOD_IRRIGAZIONE
        '                    PaginaLink = "../Operazioni/Irrigazione.aspx"


        '                    '---------------------------------------
        '                    'LAVCOD_SEMINA, LAVCOD_TRAPIANTO
        '                Case LAVCOD_SEMINA, LAVCOD_TRAPIANTO, LAVCOD_SOVESCIO, LAVCOD_SOD_SEDDING
        '                    PaginaLink = "../Operazioni/Semina_E_Trapianto_1.aspx"

        '                Case LAVCOD_CURA
        '                    PaginaLink = "../GestioneMagazzini/OperazioneDiCura.aspx"
        '                    '---------------------------------------

        '                Case Else
        '                    PaginaLink = ""

        '            End Select

        '            Return PaginaLink
        '        End Function

        '        Public Shared Function Link_GiasOnline_STR(ByVal PaginaRichiesta As enum_PagineGiasOnline,
        '                                                   ByRef objParametriAgenda As ParametriAgenda)

        '            Dim objGiasOnline As New ParametriGiasOnline
        '            Dim xChiave As String = ""
        '            Call Albero.ChiaveAlbero_Codifica(xChiave,
        '                                              enum_TipoNodo.p_PortafoglioProdotti,
        '                                              objParametriAgenda.Piva,
        '                                              objParametriAgenda.Sa_Cod, , , , , , , , , , , ,)

        '            objGiasOnline.xChiave = xChiave
        '            objGiasOnline.Operazione = enum_TipoOperazioneDB.Scrittura
        '            objGiasOnline.DataSelezionata = objParametriAgenda.Data
        '            objGiasOnline.Piva = objParametriAgenda.Piva

        '            objGiasOnline.PaginaRichiesta = PaginaRichiesta
        '            objGiasOnline.Xml_Generico.Length = 0

        '            objGiasOnline.RisorsaEditQueryString = objParametriAgenda.StrGenericaXlinkGiasOnline

        '            If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
        '                objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
        '            Else
        '                objGiasOnline.LinkAgronicaAgenda2010 = ""
        '            End If

        '            Dim link As String = ""
        '            If objParametriAgenda.SitoOrigine = Enum_SiteRedirector.Sito_AgronicaAgenda_2010 Then
        '                link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_AgronicaAgenda_2010, objGiasOnline)
        '            Else
        '                link = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_GiasOnline_2010, objGiasOnline)
        '            End If

        '            Return link

        '        End Function

        '        Public Shared Sub Link_GiasOnline(ByVal PaginaRichiesta As enum_PagineGiasOnline,
        '                                          ByRef objParametriAgenda As ParametriAgenda,
        '                                          ByRef page As System.Web.UI.Page)

        '            Dim objGiasOnline As New ParametriGiasOnline
        '            Dim xChiave As String = ""
        '            Call Albero.ChiaveAlbero_Codifica(xChiave,
        '                                              enum_TipoNodo.p_PortafoglioProdotti,
        '                                              objParametriAgenda.Piva,
        '                                              objParametriAgenda.Sa_Cod, , , , , , , , , , , ,)

        '            objGiasOnline.xChiave = xChiave
        '            objGiasOnline.Operazione = enum_TipoOperazioneDB.Scrittura
        '            objGiasOnline.DataSelezionata = objParametriAgenda.Data
        '            objGiasOnline.Piva = objParametriAgenda.Piva

        '            objGiasOnline.PaginaRichiesta = PaginaRichiesta
        '            objGiasOnline.Xml_Generico.Length = 0

        '            If Not IsNothing(ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")) AndAlso ConfigurationManager.AppSettings("LinkAgronicaAgenda2010") <> "" Then
        '                objGiasOnline.LinkAgronicaAgenda2010 = ConfigurationManager.AppSettings("LinkAgronicaAgenda2010")
        '            Else
        '                objGiasOnline.LinkAgronicaAgenda2010 = ""
        '            End If

        '            Dim link As String = RedirectGestione.IndirizzoCompleto_SitoOnline_PassandoDirettamente_ParametriGiasOnline(Enum_SiteRedirector.Sito_GiasOnline_2010,
        '                                                                                           objGiasOnline)
        '            page.Response.Redirect(link)

        '        End Sub


        '        Public Shared Function PermessiOpContabiliEMagazzino(ByVal Lav_Cod As String,
        '                                                             ByVal Operazione As String,
        '                                                             ByRef objParametri_Server As AgronicaCoreParametri,
        '                                                             ByRef objParametri_Utenti As AgronicaCoreParametri,
        '                                                             ByRef session As System.Web.SessionState.HttpSessionState
        '                                                             ) As Boolean

        '            'operazione = 0 info,1 nuovo ,2 modif ,3 elimina ,4 copia

        '            Dim olav As New AgronicaCoreMetaSchemaDAL.Operazioni_R
        '            Dim gruOp As Integer = olav.Gru_Op_from_LavorazioneCod(CInt(Lav_Cod), objParametri_Server)
        '            Dim attivita As enum_Security_Attivita
        '            If gruOp <> 6 AndAlso gruOp <> 10 And gruOp <> 20 Then
        '                Return True
        '            End If
        '            If gruOp = 6 Then
        '                attivita = enum_Security_Attivita.Gest_Contabilita
        '            End If
        '            If gruOp = 10 Then
        '                attivita = enum_Security_Attivita.Gest_Magazzino
        '            End If
        '            If gruOp = 20 Then
        '                attivita = enum_Security_Attivita.CheckList_Pratiche_Ecologiche_APOT
        '            End If

        '            Dim acUtenti As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        '            Dim utenteAbilitato As Boolean
        '            If IsNumeric(Operazione) Then

        '                Select Case CInt(Operazione)

        '                    Case enum_TipoOperazioneDB.Lettura


        '                        utenteAbilitato = acUtenti.Controlla_Permessi_Utente(
        '                                           session("ASG_Utente_Username"),
        '                                           session("ASG_IdServizio"),
        '                                           attivita,
        '                                           enum_Security_Operazione.Lettura,
        '                                           Now, "", objParametri_Utenti
        '                                           )


        '                    Case enum_TipoOperazioneDB.Scrittura,
        '                        enum_TipoOperazioneDB.Modifica,
        '                        enum_TipoOperazioneDB.Cancellazione,
        '                        enum_TipoOperazioneDB.Trasferimento,
        '                        enum_TipoOperazioneDB.Copia

        '                        utenteAbilitato = acUtenti.Controlla_Permessi_Utente(
        '                                           session("ASG_Utente_Username"),
        '                                           session("ASG_IdServizio"),
        '                                           attivita,
        '                                           enum_Security_Operazione.Modifica,
        '                                           Now, "",
        '                                           objParametri_Utenti
        '                                          )



        '                End Select

        '                Return utenteAbilitato

        '            End If

        '            Return False

        '        End Function


        '#End Region


#Region "Gestione Magazzino Azienda Padre"

        Public Sub Gestisci_Magazzino_Aziendale(ByVal Agenda As Operazione_Agenda,
                                            ByRef Id_Agenda As Integer,
                                            ByRef objParametriAgenda As ParametriAgenda,
                                            ByRef objParametri_Server As AgronicaCoreParametri)


            'Questa parte era per gestire il caso non volessi far vedere o modificare il ddt, da cancellare se non si usa
            ' '' ''In modifica Devo gestire tre casi
            ' '' ''Magazzino iniziale interno e finale esterno -> proseguo normalmente  elimino tutte operazioni e  riscrivo (controllare)
            ' '' ''Magazzino iniziale e finale sono gli stessi ed esterni -> proseguo normalmente  elimino tutte operazioni e  riscrivo (controllare)
            ' '' ''magazzino iniziale esterno e finale interno e lo stesso di destinazione del ddt -> permetto il salvataggio ma non il cambio di prodotto e udm, cambio i riferimenti all'id agenda
            ' '' ''magazzino iniziale esterno e finale interno differente dal ddt e nessun magazzino -> non permetto la modifica
            '' ''If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then
            '' ''    Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
            '' ''    Dim seminaOldConAltraImpresa As Boolean
            '' ''    seminaOldConAltraImpresa = objRif.Verifica_Semina_ConMagazzinoAltraimpresa(Agenda.Piva, _
            '' ''                                                                                Agenda.Sa_Cod, _
            '' ''                                                                                Id_Agenda_Old, _
            '' ''                                                                                "", _
            '' ''                                                                                objParametri_Server)
            '' ''    If seminaOldConAltraImpresa Then
            '' ''        'dato che lì'operazione old era legata ad un magazzino esterno devo
            '' ''        'recuperare il magazzino esterno e prodotto dell'operazione dell'agenda old
            '' ''        'per fare i controlli
            '' ''        'leggo id agenda e mov legati all'operazione old
            '' ''        Dim dt As DataTable = objRif.Leggi_Specifica(Agenda.Piva, _
            '' ''                                                    Agenda.Sa_Cod, _
            '' ''                                                    Id_Agenda_Old, _
            '' ''                                                    0, -1, 2, 0, "", 0, 0, 0, 0, LAVCOD_SCARICO, CAU_SCARICO, "", "", objParametri_Server)
            '' ''        If dt.Rows.Count <> 1 Then
            '' ''            Throw New Exception("Per la semina o trapianto è prevista la presenza di un solo movimento di scarico esterno collegato.")
            '' ''        End If
            '' ''        Dim id_agenda_rif As Integer = dt.Rows(0).Item("Id_Agenda_Rif")
            '' ''        Dim id_mov_rif As Integer = dt.Rows(0).Item("Id_Mov_Rif")
            '' ''        'leggo la destinazione legata a quell'idageda e idmov
            '' ''        'è la destinazione dello scarico del magazino azienda esterna
            '' ''        Dim dt2 As DataTable = New AgronicaCoreContabDAL.Mov_Destinazioni_R().Leggi("", 0, id_agenda_rif, id_mov_rif, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)
            '' ''        If dt2.Rows.Count <> 1 Then
            '' ''            Throw New Exception("Per la semina o trapianto è prevista la presenza di un solo movimento di destinazione di scarico esterno.")
            '' ''        End If
            '' ''        'recupero la piva, sacod e id fabbricato per il confronto
            '' ''        Dim Magazzino_Piva_Operazione_Old As String = dt2.Rows(0).Item("Piva")
            '' ''        Dim Magazzino_Sa_Cod_Operazione_Old As Integer = dt2.Rows(0).Item("Sa_Cod")
            '' ''        Dim Magazzino_Fabbricato_Cod_Operazione_Old As Integer = dt2.Rows(0).Item("Id_Destinazione")

            '' ''        'recupero il mat_cod per confrontare il prodotto, deve essere lo stesso legato al ddt , scarico e operazione nuova
            '' ''        Dim dt3 As DataTable = New AgronicaCoreContabDAL.Movimenti_Dettagli_R().Leggi(Magazzino_Piva_Operazione_Old, Magazzino_Sa_Cod_Operazione_Old, id_agenda_rif, id_mov_rif, 0, 0, 0, 0, "", 0, 0, 0, 0, 0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
            '' ''        If dt3.Rows.Count <> 1 Then
            '' ''            Throw New Exception("Per la semina o trapianto è prevista la presenza di un solo movimento dettaglio di scarico esterno.")
            '' ''        End If
            '' ''        Dim Mat_Cod__Operazione_Old As Integer = dt3.Rows(0).Item("Mat_Cod")
            '' ''    Else
            '' ''        'semina old senza altra impresa, quindi è indifferente se la nuova operazione ha o no magazzino interno od esterno
            '' ''    End If
            '' ''End If


            '--------------------------------------------------------------------------------
            '--------- CREAZIONE SCRITTURA OGGETTI PER MOVIMENTI MAGAZZINO AZIENDALE   ------
            '--------------------------------------------------------------------------------

            ' se l'agenda ha dei movimenti del magazzino e se il magazzino selezionato è quello dell'impresa padre
            'devo creare lo scarico dal padre e il ddt
            If objParametriAgenda.Fabbricato <> "0" AndAlso Split(objParametriAgenda.Fabbricato, "|").Count = 3 AndAlso Split(objParametriAgenda.Fabbricato, "|")(2) <> objParametriAgenda.Piva Then

                Dim objAgendaScrivi As New Agenda_Operazione_Helper
                Dim objAgendaW As New AgronicaCoreContabBIZ.Agenda_W
                Dim objRifW As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                Dim objRifR As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R

                Dim idAgendaScaricoDaAzPadre As Integer = 0
                Dim idAgendaBollaFittizia As Integer = 0

                Dim flagInsert As Boolean = False
                Dim cancellataOperazione As Boolean = False

                If objParametriAgenda.Tipo_Operazione = enum_TipoOperazioneDB.Modifica Then

                    Dim dtRif As DataTable
                    Dim drRif() As DataRow

                    dtRif = objRifR.Recupera_DT_Rif_Unificato(Agenda.Piva,
                                                            Agenda.Sa_Cod,
                                                            Agenda.Id_Agenda,
                                                            0, 0, 0, "",
                                                            objParametri_Server)

                    If Not dtRif Is Nothing AndAlso dtRif.Rows.Count > 0 Then
                        drRif = dtRif.Select("Lav_Cod_Risultato=" & LAVCOD_SCARICO)
                        If Not drRif Is Nothing AndAlso drRif.Length > 0 Then
                            idAgendaScaricoDaAzPadre = drRif(0).Item("Id_Agenda_Risultato")
                        End If
                        drRif = dtRif.Select("Lav_Cod_Risultato=" & LAVCOD_BOLLA_RICEVUTA)
                        If Not drRif Is Nothing AndAlso drRif.Length > 0 Then
                            idAgendaBollaFittizia = drRif(0).Item("Id_Agenda_Risultato")
                        End If
                    End If

                    '---------------------------------------------------------------------------------------
                    'CANCELLAZIONE OPERAZIONI
                    'ricavo una matrice con tutti gli id agenda etc presenti nella tabella dei riferimenti
                    Dim matriceIdAgendaOld(,) As String

                    matriceIdAgendaOld = objRifR.MatriceChiaviAgenda_MovRiferiti(Agenda.Piva,
                                                                                 Agenda.Sa_Cod,
                                                                                 Agenda.Id_Agenda,
                                                                                 "",
                                                                                 objParametri_Server)

                    If Not IsNothing(matriceIdAgendaOld) Then


                        Dim pivaCanc As String
                        Dim saCodCanc As Integer
                        Dim idAgendaCanc As Integer

                        Dim objRifCanc As New AgronicaCoreContabDAL.Mov_Det_Riferimenti_W
                        Dim objMovR As New AgronicaCoreContabDAL.Movimenti_R

                        For i = 0 To UBound(matriceIdAgendaOld, 2)

                            pivaCanc = matriceIdAgendaOld(0, i)
                            saCodCanc = matriceIdAgendaOld(1, i)
                            idAgendaCanc = matriceIdAgendaOld(2, i)

                            Dim cancellabile As Boolean = True
                            Dim docNumero As Decimal = -1

                            If objMovR.Esiste_Almeno_Un_Doc_Contabile_Riferito(pivaCanc,
                                                                  idAgendaCanc,
                                                                   "", docNumero,
                                                                   objParametri_Server) Then

                                If docNumero <= 0 Then
                                    cancellabile = True
                                    'non è un ddt reale,m quindi posso cancellare il riferimento
                                Else
                                    'è un ddt reale quindi non lo cancello
                                    cancellabile = False
                                End If

                            Else
                                'non c'è un collegamento al ddt ma ad un magazzino, quindi posso cancellare
                                cancellabile = True
                            End If



                            'SE NON E' UN DDT BUONO, oppure è il riferimento allo scarico, ALLORA CANCELLO I RIFERIMENTI
                            If cancellabile Then

                                'cancello le operazioni collegate di scarico e ddt 
                                cancellataOperazione = objAgendaScrivi.Cancella(pivaCanc,
                                                                                saCodCanc,
                                                                                idAgendaCanc, False,
                                                                                objParametri_Server)
                                'cancello il riferimento 
                                cancellataOperazione = objRifCanc.Cancella_byChiaveRif(pivaCanc,
                                                                                       saCodCanc,
                                                                                       idAgendaCanc,
                                                                                       0, 0,
                                                                                       "",
                                                                                       objParametri_Server)

                            End If

                        Next

                    End If

                    cancellataOperazione = objAgendaScrivi.Cancella(objParametriAgenda.Piva,
                                                                    objParametriAgenda.Sa_Cod,
                                                                    objParametriAgenda.Id_Agenda, False,
                                                                    objParametri_Server)

                End If

                '--------------------------------------------------------------------------------
                '--------- SCRITTURA OPERAZIONE   -----------------------------------------------
                '--------------------------------------------------------------------------------

                Id_Agenda = objAgendaScrivi.Scrivi(Agenda, objParametri_Server)



                Dim dtDettagli As DataTable = New AgronicaCoreXML.XML_Contab().DtForXml_Genera_MovimentiDettagli
                Dim dtDestinazioni As DataTable = New AgronicaCoreXML.XML_Contab().DtForXml_Genera_MovDestinazioni

                'prendo il magazzino con sacod piu basso, devo modificare anche lo scarico dell'agenda, anche li se sono nel magazzino del
                'centro padre devo prendere lo stesso magazzino di default
                Dim sa_cod_magazzino_predefinito_azienda As Integer
                Dim Fabbricato_Cod_magazzino_predefinito_azienda As Integer
                Dim fabbricati As New AgronicaCoreAnagrafeDAL.Fabbricati_R()
                fabbricati.Ricava_PrimoMagazzino_Impresa(Agenda.Piva, sa_cod_magazzino_predefinito_azienda, Fabbricato_Cod_magazzino_predefinito_azienda, objParametri_Server)
                If sa_cod_magazzino_predefinito_azienda = 0 Or Fabbricato_Cod_magazzino_predefinito_azienda = 0 Then
                    Throw New Exception("L'azienda non ha un magazzino, per utilizzare un magazzino esterno è necessario avere un magazzino aziendale.")
                End If


                Dim objMov As New AgronicaCoreContabDAL.Movimenti_R

                'verifico che non ci sia una bolla reale collegata appena salvata, è impossibile
                If objRifR.Verifica_Operazione_SeminaTrapianto_Con_Bolle_Collegate(Agenda.Piva, Agenda.Sa_Cod, Agenda.Id_Agenda, Agenda.Lav_Cod, "", objParametri_Server) Then
                    Throw New Exception("Attenzione, l'operazione è collegata ad un ddt e ora la si vuole collegare al magazzino di un'altra impresa, questo non è consentito")
                End If


                Dim pivaMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(2)
                Dim saCodMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(1)
                Dim fabbricatoCodMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(0)


                Dim movimentiDetScarico As New List(Of Movimento_Dettaglio)

                For Each movimentoagenda As Movimento In Agenda.Movimenti
                    If movimentoagenda.Cau_Mov = CAU_SCARICO Then
                        movimentiDetScarico = movimentoagenda.Movimenti_Dettagli
                    End If
                Next


                For Each movdet As Movimento_Dettaglio In movimentiDetScarico

                    Dim elemCod As Integer = movdet.Elem_Cod ' SEMENTI 'fisso
                    Dim proCod As Integer = movdet.Pro_Cod '0 se magazzino, sem_cod altrimenti
                    Dim matCod As Integer = movdet.Mat_Cod 'Mat_Cod siempre
                    Dim udmCod As Integer = movdet.Udm_Cod
                    Dim lotto As String = movdet.Lotto
                    Dim qta As Decimal = movdet.Qta
                    Dim data As Date = movdet.Data
                    Dim tipo As Integer = MAGAZZINO

                    'Dim descrizioneProdotto As String = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod("", 0, matCod, "", "", "", objParametri_Server)

                    Dim descrizioneProdotto As String = New AgronicaCoreAnagrafeDAL.Materie_Prime_R().MatDes_from_MatCod_SenzaFiltroVisibilita(elemCod, matCod, Nothing, Nothing, Nothing, Nothing, objParametri_Server)


                    '--------------------------------------------------------------------------------
                    '--------- SCRITTURA MOVIMENTO SCARICO AZIENDA ESTERNA   ------------------------
                    '--------------------------------------------------------------------------------
                    Dim strMovScarico As String = ""

                    '(22/04/2020) azzerato id_agenda anche per la modifica perchè se si piu dettagli non funziona la scrittura
                    idAgendaScaricoDaAzPadre = 0
                    strMovScarico = Crea_Scarico_Da_Az_Padre(idAgendaScaricoDaAzPadre, Agenda, descrizioneProdotto, elemCod, matCod, udmCod, lotto, qta, data, objParametriAgenda, objParametri_Server)

                    '1) scrittura movimento scarico magazzino impresa padre
                    Dim OUTPUT_ID_Agenda_scarico As Integer
                    If strMovScarico <> "" Then
                        flagInsert = False
                        flagInsert = objAgendaW.Agenda_Scrivi(strMovScarico,
                                                              OUTPUT_ID_Agenda_scarico,
                                                              0,
                                                              5,
                                                              0,
                                                              "",
                                                              objParametri_Server)

                    Else
                        Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> <br>")
                    End If
                    If OUTPUT_ID_Agenda_scarico = 0 Or flagInsert = False Then
                        Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> <br>")
                    End If

                    '--------------------------------------------------------------------------------
                    '--------- SCRITTURA RIFERIMENTO CON SCARICO AZIENDA ESTERNA  ------------------
                    '--------------------------------------------------------------------------------
                    'scrittura riferimenti x collegare le operazioni
                    If OUTPUT_ID_Agenda_scarico <> 0 Then

                        'Ricavo id movimento scarico che mi servirà per collegare i movimenti nella tabella riferimenti
                        Dim idMovScarico As Integer
                        idMovScarico = objMov.IdMov_from_IdAgendaCauMov(pivaMagazzino,
                                                                        OUTPUT_ID_Agenda_scarico,
                                                                        CAU_SCARICO,
                                                                        "",
                                                                        objParametri_Server)

                        Dim idMovSemina As Integer
                        idMovSemina = objMov.IdMov_from_IdAgendaCauMov(Agenda.Piva,
                                                                       Id_Agenda,
                                                                       enum_Agenda_Causali.LAVORAZIONE,
                                                                       "",
                                                                       objParametri_Server)

                        '2) scrittura del ddt fittizio di carico del magazzino dell'impresa che fa la semina
                        ' semina agganciata a scarico magazzino
                        'magazzino non è quello dell'impresa, ma di un'impresa padre
                        flagInsert = False
                        flagInsert = objRifW.Scrivi(Agenda.Piva,
                                                    Agenda.Sa_Cod,
                                                    Id_Agenda,
                                                    idMovSemina,
                                                    -1,
                                                    objParametriAgenda.Lav_Cod,
                                                    objParametriAgenda.Cau_Mov,
                                                    pivaMagazzino,
                                                    saCodMagazzino,
                                                    OUTPUT_ID_Agenda_scarico,
                                                    idMovScarico,
                                                    -1,
                                                    LAVCOD_SCARICO,
                                                    CAU_SCARICO,
                                                    0,
                                                    Agenda.Data,
                                                    AGRODATAFINE,
                                                    objParametri_Server)

                        If flagInsert = False Then
                            Throw New Exception("<b>Non è riuscita l'aggancio dell' operazine di agenda con lo scarico dal magazzino dell'azienda padre </b> <br>")
                        End If

                    Else
                        Throw New Exception("<b>Non è riuscita l'aggancio delle operazini di agenda di scarico e DDT </b> <br>")
                    End If



                    '----------------------------------------------------------------------------------------------------------------
                    ' Aggiungo righe al DT dei dettagli da aggiungere all'unico DDT
                    Dim lavdes As String = New AgronicaCoreMetaSchemaDAL.Operazioni_R().LavorazioneDes_from_LavorazioneCod(objParametriAgenda.Lav_Cod, objParametri_Server)
                    Dim movDetDes As String = "DDT fittizio per " & lavdes & ": " & descrizioneProdotto
                    Aggiungi_Righe_Dettagli_Destinazioni_alle_Tabelle_DDT(Agenda, dtDettagli, dtDestinazioni, sa_cod_magazzino_predefinito_azienda, Fabbricato_Cod_magazzino_predefinito_azienda, elemCod, proCod, matCod, udmCod, lotto, qta, data, movDetDes, objParametriAgenda, objParametri_Server)
                    '----------------------------------------------------------------------------------------------------------------


                Next


                '----------------------------------------------------------------------------------
                '--------- SCRITTURA DDT FITTIZIO--------------------------------------------------
                '----------------------------------------------------------------------------------
                '----3) creazione del ddt Fittizio per il carico di magazzino dell'impresa e aggiunta degli n dettagli
                Dim str_DDTricevutoFittizio As String
                str_DDTricevutoFittizio = Crea_DDTricevutoFittizio(idAgendaBollaFittizia, Agenda, dtDettagli, dtDestinazioni, objParametriAgenda, objParametri_Server)
                Dim OUTPUT_ID_Agenda_ddt As Integer
                flagInsert = False
                If str_DDTricevutoFittizio <> "" Then
                    flagInsert = objAgendaW.Agenda_Scrivi(str_DDTricevutoFittizio,
                                                          OUTPUT_ID_Agenda_ddt,
                                                          0,
                                                          5,
                                                          0,
                                                          "",
                                                          objParametri_Server)
                Else
                    Throw New Exception("<b>Non è riuscita la creazione del DDT dall'azienda padre </b> <br>")
                End If
                If OUTPUT_ID_Agenda_ddt = 0 Or flagInsert = False Then
                    Throw New Exception("<b>Non è riuscita la creazione del DDT dall'azienda padre </b> <br>")
                End If

                '--------------------------------------------------------------------------------
                '--------- SCRITTURA RIFERIMENTO DDT FITTIZIO----------------  ------------------
                '--------------------------------------------------------------------------------
                '4) scrittura riferimenti x collegare le operazioni
                If OUTPUT_ID_Agenda_ddt <> 0 Then


                    Dim idMovSemina As Integer
                    idMovSemina = objMov.IdMov_from_IdAgendaCauMov(Agenda.Piva,
                                                                   Id_Agenda,
                                                                   enum_Agenda_Causali.LAVORAZIONE,
                                                                   "",
                                                                   objParametri_Server)


                    '3.2) semina agganciata a dtt ricevuto
                    'id_mov = id_mov semina
                    'id_mov_rif = id_mov movimento contabile del ddt (quello con cau_mov = 4000)
                    Dim idMovContDdt As Integer

                    idMovContDdt = objMov.IdMov_from_IdAgendaCauMov(Agenda.Piva,
                                                                    OUTPUT_ID_Agenda_ddt,
                                                                    CAU_REGISTRAZIONI,
                                                                    "",
                                                                    objParametri_Server)
                    flagInsert = False
                    flagInsert = objRifW.Scrivi(Agenda.Piva,
                                                Agenda.Sa_Cod,
                                                Id_Agenda,
                                                idMovSemina,
                                                -1,
                                                objParametriAgenda.Lav_Cod,
                                                objParametriAgenda.Cau_Mov,
                                                Agenda.Piva,
                                                0,
                                                OUTPUT_ID_Agenda_ddt,
                                                idMovContDdt,
                                                -1,
                                                LAVCOD_BOLLA_RICEVUTA,
                                                CAU_REGISTRAZIONI,
                                                0,
                                                Agenda.Data,
                                                AGRODATAFINE,
                                                objParametri_Server)

                    If flagInsert = False Then
                        Throw New Exception("<b>Non è riuscita l'aggancio dell' operazine di agenda con il DDT </b> <br>")
                    End If
                Else
                    Throw New Exception("<b>Non è riuscita l'aggancio delle operazini di agenda di scarico e DDT </b> <br>")
                End If



                '--------------------------------------------------------------------------------
                '---------FINE  SCRITTURA OGGETTI AGENDA  ---------------------------------------
                '--------------------------------------------------------------------------------

            End If



        End Sub

        Private Function Crea_Scarico_Da_Az_Padre(ByVal Id_Agenda As Integer,
                                                  ByVal Agenda As Operazione_Agenda,
                                                  ByVal Descrizione_Prodotto As String,
                                                  ByVal Elem_Cod As Integer,
                                                  ByVal Mat_Cod As Integer,
                                                  ByVal Udm_Cod As Integer,
                                                  ByVal lotto As String,
                                                  ByVal Qta As Decimal,
                                                  ByVal Data As Date,
                                                  ByRef objParametriAgenda As ParametriAgenda,
                                                  ByRef objParametri_Server As AgronicaCoreParametri
                                                  ) As String

            'magazzino non è quello dell'impresa, ma di un'impresa padre
            Dim pivaMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(2)
            Dim saCodMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(1)
            Dim fabbricatoCodMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(0)

            Dim strMovScarico As String

            Dim desLib As String = "Scarico di magazzino " & Descrizione_Prodotto
            Dim movDesc As String = "Scarico di magazzino " & Descrizione_Prodotto
            Dim movDetDes As String = "Scarico di magazzino per  " & objParametriAgenda.Lav_Cod & ": " & Descrizione_Prodotto

            Dim Log_Errori As String = ""
            Dim objXML As New AgronicaCoreXML.XML_Contab
            strMovScarico = objXML.MacroXML_CaricoScaricoMagazzino(Log_Errori,
                                                                   Nothing,
                                                                   2,
                                                                   pivaMagazzino,
                                                                   saCodMagazzino,
                                                                   fabbricatoCodMagazzino,
                                                                   desLib,
                                                                   Data,
                                                                   AGRODATAINIZIO,
                                                                   movDesc,
                                                                   movDetDes,
                                                                   Elem_Cod,
                                                                   0,
                                                                   Mat_Cod,
                                                                   0,
                                                                   0,
                                                                   lotto,
                                                                   0,
                                                                   Udm_Cod,
                                                                   Qta,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   enum_Pendenza.MovPendente,
                                                                   0,
                                                                   0,
                                                                   0,
                                                                   Agenda.BaseCode,
                                                                   Agenda.TopCode,
                                                                   1,
                                                                   objParametriAgenda.Data,
                                                                   objParametri_Server.UsernameOperazione,
                                                                   Id_Agenda)
            If Log_Errori <> "" Then
                Throw New Exception("<b>Non è riuscita la creazione dello scarico dall'azienda padre </b> " & Log_Errori & "<br>")
            End If
            Return strMovScarico
        End Function


        Private Shared Sub Aggiungi_Righe_Dettagli_Destinazioni_alle_Tabelle_DDT(ByVal Agenda As Operazione_Agenda,
                                                                                 ByRef DT_Dettagli As DataTable,
                                                                                 ByRef DT_Destinazioni As DataTable,
                                                                                 ByVal sa_cod_magazzino_predefinito_azienda As Integer,
                                                                                 ByVal Fabbricato_Cod_magazzino_predefinito_azienda As Integer,
                                                                                 ByVal Elem_Cod As Integer,
                                                                                 ByVal Pro_Cod As Integer,
                                                                                 ByVal Mat_Cod As Integer,
                                                                                 ByVal Udm_Cod As Integer,
                                                                                 ByVal lotto As String,
                                                                                 ByVal Qta As Decimal,
                                                                                 ByVal Data As Date,
                                                                                 ByVal Mov_Det_Des As String,
                                                                                 ByRef objParametriAgenda As ParametriAgenda,
                                                                                 ByRef objParametri_Server As AgronicaCoreParametri)
            Dim objXML As New AgronicaCoreXML.XML_Contab
            objXML.DtForXml_InserisciRiga_MovimentiDettagli(
                                        DT_Dettagli,
                                        enum_TipoOperazioneDB.Scrittura,
                                        Agenda.Piva,
                                        sa_cod_magazzino_predefinito_azienda,
                                         0, 0, 0,
                                        Mov_Det_Des,
                                        Elem_Cod,
                                        Pro_Cod,
                                        Mat_Cod,
                                        0,
                                        0,
                                        lotto,
                                        0,
                                        Udm_Cod,
                                        ,
                                        Qta,
                                        0,
                                        0, 0, 0, 0,
                                        0, 0, 0, 0,
                                        0, 0, 0,
                                        0,
                                        ,
                                        ,
                                        , , ,
                                        Data,
                                        AGRODATAFINE,
                                        0, 0, 0, 0, 0, 0, 0,
                                        Agenda.BaseCode,
                                        Agenda.TopCode,
                                        Fabbricato_Cod_magazzino_predefinito_azienda,
                                        0,
                                        CAU_CARICO)

            '-------------------------------------------
            '---- Creazione DT delle destinazioni ------
            'aggiungo righe al dt destinazioni
            objXML.DtForXml_InserisciRiga_MovDestinazioni(
                                        DT_Destinazioni,
                                        enum_TipoOperazioneDB.Scrittura,
                                        Agenda.Piva,
                                        sa_cod_magazzino_predefinito_azienda,
                                        0, 0, 0, 0,
                                        Fabbricato_Cod_magazzino_predefinito_azienda,
                                        MAGAZZINO,
                                        Qta,
                                        0,
                                        0, 0,
                                        Data,
                                        AGRODATAFINE,
                                        Agenda.BaseCode,
                                        Agenda.TopCode)
        End Sub


        Private Function Crea_DDTricevutoFittizio(ByVal Id_Agenda As Integer,
                                                  ByVal Agenda As Operazione_Agenda,
                                                  ByVal DT_Dettagli As DataTable,
                                                  ByVal DT_Destinazioni As DataTable,
                                                  ByRef objParametriAgenda As ParametriAgenda,
                                                  ByRef objParametri_Server As AgronicaCoreParametri
                                                  ) As String

            Dim strDdTricevutoFittizio As String
            Dim f_prefisso_numero_ddt As String = ""
            Dim f_numero_ddt As Integer = 0
            Dim f_suffisso_numero_ddt As String = ""
            Dim f_data_ddt As Date = objParametriAgenda.Data
            Dim f_numero_colli As Integer = 1
            Dim f_causale_trasporto As String = "CONTO ACQUISTO"
            Dim f_aspetto_beni As String = "VISIBILE"
            Dim naturaBeni As String = ""
            Dim f_peso As Integer = 0
            Dim f_data_consegna As Date = objParametriAgenda.Data
            Dim f_ora_consegna = AGRODATAINIZIO
            Dim numProtocollo As Integer = 0
            Dim f_note_ddt As String = "DDT fittizio creato in automatico per scarico da magazzino impresa padre"
            Dim jollyInt As Integer = MagazzinoMovimentato
            Dim contabilizzato As Integer = NONCONTABILE
            Dim pendente As Integer = enum_Pendenza.MovPendente
            Dim codIndirizzoRisum As Integer = 0
            Dim ddtBloccoFlag As Integer = 1
            Dim ddtBloccoData As Date = objParametriAgenda.Data
            Dim ddtBloccoUsername As String = objParametri_Server.UsernameOperazione
            Dim ragSocFornitore As String = ""
            Dim desLibDdt As String
            Dim movDescMagazzino As String

            'PREREQUISITO
            'verifica se l'impresa padre è già salvato come contatto fornitore
            'in caso contrario aggiungere il rapporto contabile fornitore
            'chiamare funzione in anagrafe biz che restituisce cod_risum e cod_indirizzorisum
            'magazzino non è quello dell'impresa, ma di un'impresa padre
            Dim pivaMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(2)
            Dim saCodMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(1)
            Dim fabbricatoCodMagazzino As String = Split(objParametriAgenda.Fabbricato, "|")(0)
            Dim contatti As New AgronicaCoreAnagrafeBIZ.Contatti_MultiHost_R
            Dim OUT_Piva_Contatto As String = ""
            ' Dim OUT_Sa_Cod As String
            Dim OUT_Cod_Risum As Integer
            Dim OUT_Cod_Indirizzo As Integer

            contatti.RicavaScrive_Contatto_Fornitore("",
                                                     pivaMagazzino,
                                                     OUT_Piva_Contatto,
                                                     OUT_Cod_Risum,
                                                     OUT_Cod_Indirizzo,
                                                     ragSocFornitore,
                                                     objParametri_Server)

            If ragSocFornitore = "" Then
                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                ragSocFornitore = objImp.RagSoc_from_Piva(pivaMagazzino, objParametri_Server)
            End If

            desLibDdt = "Documento di Trasporto Ricevuto (n.0 Rif:" & ragSocFornitore & ") "
            movDescMagazzino = "Carico di Articoli relativi al ricevimento DDT n.0 del " + f_data_ddt + " Rif: " + ragSocFornitore + " "

            Dim erroreMessaggio As String = ""
            strDdTricevutoFittizio = New AgronicaCoreXML.XML_Contab().MacroXML_Contabilita_DDT(
                                            erroreMessaggio,
                                            Nothing,
                                            objParametri_Server.PivaSuperUser,
                                            Agenda.Piva,
                                            LAVCOD_BOLLA_RICEVUTA,
                                            desLibDdt,
                                            f_prefisso_numero_ddt,
                                            f_numero_ddt,
                                            f_suffisso_numero_ddt,
                                            f_data_ddt,
                                            f_numero_colli,
                                            f_causale_trasporto,
                                            f_aspetto_beni,
                                            naturaBeni,
                                            f_peso,
                                            f_data_consegna,
                                            f_ora_consegna,
                                            0,
                                            f_note_ddt,
                                            OUT_Cod_Risum,
                                            codIndirizzoRisum,
                                            CAU_CARICO,
                                            movDescMagazzino,
                                            Agenda.BaseCode,
                                            Agenda.TopCode,
                                            Nothing,
                                            Nothing,
                                            DT_Dettagli,
                                            DT_Destinazioni,
                                            ddtBloccoFlag,
                                            ddtBloccoData,
                                            ddtBloccoUsername,
                                            0,
                                            0, 0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            0,
                                            "",
                                            "",
                                            0,
                                            AGRODATAINIZIO,
                                            0,
                                            0,
                                            Agenda.Data,
                                            0,
                                            0,
                                            0,
                                            Id_Agenda)

            If erroreMessaggio <> "" Then
                Throw New Exception("<b>Non è riuscita la creazione dello scarico dal DDT dall'azienda padre </b> " & erroreMessaggio & "<br>")
            End If

            Return strDdTricevutoFittizio

        End Function


        Public Sub ImpostaFabbricatoDelMagazzinoEsternoSePresente(ByVal Agenda As Operazione_Agenda,
                                                                  ByRef objParametriAgenda As ParametriAgenda,
                                                                  ByRef objParametri_Server As AgronicaCoreParametri)
            '----------------------------------------------------------------------------------------------------
            'gestione combo magazzino esterno
            'se l'operazione aveva utilizzato il magazzino del'azienda padre devo preselezionare quello e non quello del'operazione
            Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
            Dim OerezioneOldConAltraImpresa As Boolean
            Dim idAgendaOld As Integer = objParametriAgenda.Id_Agenda
            OerezioneOldConAltraImpresa = objRif.Verifica_Operazione_ConMagazzinoAltraimpresa(Agenda.Piva,
                                                                                        Agenda.Sa_Cod,
                                                                                        idAgendaOld,
                                                                                        objParametriAgenda.Lav_Cod,
                                                                                        "",
                                                                                        objParametri_Server)
            If OerezioneOldConAltraImpresa Then

                'recupero la piva, sacod e id fabbricato per il confronto
                Dim magazzinoPivaOperazioneOld As String = ""
                Dim magazzinoSaCodOperazioneOld As Integer = 0
                Dim magazzinoFabbricatoCodOperazioneOld As Integer = 0

                objRif.Recupera_ChiaveMagazzinoAltraImpresa_Operazione(Agenda.Piva,
                                                                       Agenda.Sa_Cod,
                                                                       idAgendaOld,
                                                                       objParametriAgenda.Lav_Cod,
                                                                       "",
                                                                       magazzinoPivaOperazioneOld,
                                                                       0,
                                                                       magazzinoSaCodOperazioneOld,
                                                                       magazzinoFabbricatoCodOperazioneOld,
                                                                       objParametri_Server)

                If magazzinoPivaOperazioneOld = "" Or magazzinoSaCodOperazioneOld = 0 Or magazzinoFabbricatoCodOperazioneOld = 0 Then
                    Throw New Exception("Non ho recupoerato il magazzino dell'azienda padre")
                End If

                objParametriAgenda.Fabbricato = magazzinoFabbricatoCodOperazioneOld & "|" & magazzinoSaCodOperazioneOld & "|" & magazzinoPivaOperazioneOld

            End If
            '----------------------------------------------------------------------------------------------------
        End Sub

#End Region

#Region "Collegamento con sito Stampe"

        Public Shared Function GetUrlStampaSpecifica(ByVal variabiliStampe() As ElementoStampe,
                                                     ByVal codiceReport As enum_CodificaStampe,
                                                     ByVal username As String,
                                                     ByVal progressivoGias As String
                                                     ) As String

            Dim paginaLink As String = ""

            Dim objVs As New AgronicaCoreXML.XML_Stampe
            Dim strNodo As String = objVs.XML_VariabiliStampe(variabiliStampe)
            Dim strNodiVariabili As String = strNodo

            Dim objAgronicaStampe As New ParametriAgronicaStampe With {
                .report = codiceReport,
                .username = username,
                .user_profilo = progressivoGias
            }
            objAgronicaStampe.Xml_Generico.Length = 0
            objAgronicaStampe.Xml_Generico.Append(strNodiVariabili)

            Dim strJs As String = RedirectGestione.ApriPopUp_SitoStampe_PassandoDirettamente_ParametriAgronicaStampe(
                    Enum_SiteRedirector.Sito_GiasOnline_2010,
                    objAgronicaStampe)

            Dim strSplit As String() = strJs.Split(New Char() {"'"c}, StringSplitOptions.RemoveEmptyEntries)
            paginaLink = strSplit(3)

            Return paginaLink

        End Function

#End Region

    End Class




End Namespace
