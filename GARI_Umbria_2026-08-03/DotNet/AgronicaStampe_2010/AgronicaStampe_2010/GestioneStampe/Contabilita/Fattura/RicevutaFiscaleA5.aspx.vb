Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreStampeDAL

Partial Class RicevutaFiscaleA5
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private _rptRicFiscA5 As Rpt_RicevutaFiscaleA5

    Private _piva As String
    Private _lavCod As Integer
    Private _idAgenda As Integer
    Private _dataMovimento As Date
    'Private mode_preview As enum_StampaConSenzaPreview
    Private _identificazioneDocumento As String = ""
    Private _myRnd As New Random

    Private _printToPrinter As Integer
    Private _printName As String
    Private _numeroCopie As Integer
    Private _strFlagFascicola As String
    Private _startPage As Integer
    Private _endPage As Integer

    Private _objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objAgroWebConfig As AgronicaCoreGestioneRichieste.AgroWebConfig

    Private _nuoviArrotondamenti As Boolean = False
    Private _objConfigStampe As ConfigurazioneStampe = Nothing

    Const NUM_DETTAGLI_GESTITI As Integer = 20

#Region " RICEVUTA FISCALE A5 "

    'Chiamata richiesta da Progettazione Web Form.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTA: la seguente dichiarazione è richiesta da Progettazione Web Form.
    'Non spostarla o rimuoverla.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: questa chiamata al metodo è richiesta da Progettazione Web Form.
        'Non modificarla nell'editor del codice.
        InitializeComponent()

        _objConfigStampe = New ConfigurazioneStampe()

        _rptRicFiscA5 = New Rpt_RicevutaFiscaleA5

    End Sub

#End Region


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _idAgenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), AgroKey_EncoderDecoder, Server))

        _lavCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), AgroKey_EncoderDecoder, Server))

        'mode_preview = CInt(Stringa_Decodifica(CStr(Request.QueryString("mp")), AgroKey_EncoderDecoder, Server))

        If Not IsNothing(Request.QueryString("ptp")) Then
            _printToPrinter = Stringa_Decodifica(CStr(Request.QueryString("ptp")), AgroKey_EncoderDecoder, Server)
        Else
            _printToPrinter = 0
        End If

        If Not IsNothing(Request.QueryString("pn")) Then
            _printName = Stringa_Decodifica(CStr(Request.QueryString("pn")), AgroKey_EncoderDecoder, Server)
        Else
            _printName = ""
        End If

        If Not IsNothing(Request.QueryString("nc")) Then
            _numeroCopie = CInt(Stringa_Decodifica(CStr(Request.QueryString("nc")), AgroKey_EncoderDecoder, Server))
        Else
            _numeroCopie = 1
        End If

        If Not IsNothing(Request.QueryString("ff")) Then
            _strFlagFascicola = Stringa_Decodifica(CStr(Request.QueryString("ff")), AgroKey_EncoderDecoder, Server)
        Else
            _strFlagFascicola = "true"
        End If

        If Not IsNothing(Request.QueryString("sp")) Then
            _startPage = CInt(Stringa_Decodifica(CStr(Request.QueryString("sp")), AgroKey_EncoderDecoder, Server))
        Else
            _startPage = 1
        End If

        If Not IsNothing(Request.QueryString("ep")) Then
            _endPage = CInt(Stringa_Decodifica(CStr(Request.QueryString("ep")), AgroKey_EncoderDecoder, Server))
        Else
            _endPage = 1
        End If

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        _objAgroWebConfig = New AgronicaCoreGestioneRichieste.AgroWebConfig

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        '  Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim nomeDocumento As String = "RicevutaFiscale"
        Dim logErrori As String = ""

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim ds As New DS_RicevutaFiscaleA5

            Dim catCod As Integer = enum_CategorieDocumenti.RicevutaFiscale

            logErrori = ""

            Try

                Stampa_RicevutaFiscaleA5(ds, logErrori)

            Catch exc As Exception
                logErrori &= "- Stampa_Fattura_NotaAccredito: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Try

                _identificazioneDocumento = nomeDocumento & "_p" & _piva & "_" & _identificazioneDocumento & "_A5"

                Dim nomeFile As String = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(_identificazioneDocumento) & ".pdf"

                ' leggo la sotto cartella da CategorieDocumenti
                Dim sottoCartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                sottoCartella = objCatDoc.Sottocartella(catCod, "", "", _objParametriServer)
                objCatDoc = Nothing

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptRicFiscA5,
                                           catCod,
                                           sottoCartella,
                                           nomeFile,
                                           _objParametriServer,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                catCod,
                                                                                "Fattura",
                                                                                nomeFile,
                                                                                sottoCartella,
                                                                                _idAgenda, "", "", "",
                                                                                _dataMovimento,
                                                                                _dataMovimento,
                                                                                _objParametriServer)

            Catch ex As Exception
                logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            SalvaLogErrori_Agenda(logErrori, nomeDocumento, _identificazioneDocumento, "RicevutaFiscaleA5.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)

            'array di DataSet e data table
            Dim dsRpt() As DataSet = {ds}

            'salvo il report nella sessione
            Session("DS") = dsRpt

        Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            If Not IsNothing(Session("DS")) Then

                'imposto la sorgente dati x il report...
                _rptRicFiscA5.SetDataSource(CType(Session("DS")(0), DataSet))

            End If

        End If

        '==================================================================

        If _printToPrinter = 0 Then

            'MS Eliminato passaggio report in session per problema deallocazione memoria:  Session("Report") = rptRicFiscA5

            'MS Passaggio report su file al visualizzatore
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptRicFiscA5.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Una volta persistito il report con i dati su file distruggo rpt e DataSet ed eseguo un GC.Collect
            'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.

            'MS Dispose del report nel caso di passaggio al visualizzatore
            _rptRicFiscA5.Close()
            _rptRicFiscA5.Dispose()
            _rptRicFiscA5 = Nothing

            GC.Collect()

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                              "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))

        Else

            Dim msg_stampa As String = ""
            If _printName = "" Then
                logErrori &= "Stampante non impostata."
                msg_stampa &= "Stampante non impostata."
            Else

                Dim Mode_AgroWinSrvc_PrintUtility_PtP As Integer = 1
                Dim PathDatiServizioWinStampa As String = ""
                Dim PathComandiServizioWinStampa As String = ""

                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                objConfigSiti.Leggi_Config_AgroWinSrvcPrintUtility(_objParametriServer,
                                                                   Mode_AgroWinSrvc_PrintUtility_PtP,
                                                                   PathComandiServizioWinStampa,
                                                                   PathDatiServizioWinStampa)
                Select Case Mode_AgroWinSrvc_PrintUtility_PtP

                    Case 0 'chiamata al servizio windows

                        Try

                            Dim exportOpts As ExportOptions = _rptRicFiscA5.ExportOptions

                            ' Imposta il formato di esportazione.
                            exportOpts.ExportFormatType = ExportFormatType.CrystalReport
                            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

                            ' Imposta le opzioni relative al file del disco.
                            Dim strPath, nomeFileRpt As String

                            'percorso dati
                            If Not PathDatiServizioWinStampa.EndsWith("\") Then PathDatiServizioWinStampa &= "\"

                            nomeFileRpt = nomeDocumento & "_" & _myRnd.Next.ToString() & "_" & CStr(Session("ASG_Utente_Username")) & "_" & Session.SessionID.ToString & ".rpt"

                            strPath = pathDatiServizioWinStampa & nomeFileRpt

                            Dim diskOpts As New DiskFileDestinationOptions With {.DiskFileName = strPath}
                            exportOpts.DestinationOptions = diskOpts

                            ' Esportazione del report.
                            _rptRicFiscA5.Export()

                            Try
                                'Dim risp As Boolean = InviaStampaToServizioUtility(Request, _rptRicFiscA5, msg_stampa, _printName, nomeFileRpt,
                                '                                                   _numeroCopie, _strFlagFascicola, _startPage, _endPage)

                                Dim risp As Boolean = InviaStampaToServizioUtility(Request, _rptRicFiscA5, msg_stampa, _printName, nomeFileRpt,
                                                                                    PathComandiServizioWinStampa, PathDatiServizioWinStampa, _objParametriServer,
                                                                                    _numeroCopie, _strFlagFascicola, _startPage, _endPage)
                            Catch ex As Exception
                                logErrori &= "InviaStampaToServizioUtility: Stampa del report non riuscita: " & ex.Message
                                msg_stampa &= "InviaStampaToServizioUtility: Stampa del report non riuscita: " & ex.Message
                            End Try

                        Catch ex As Exception
                            logErrori &= "InviaStampaToServizioUtility: Export del report non riuscita: " & ex.Message
                            msg_stampa &= "InviaStampaToServizioUtility: Export del report non riuscita: " & ex.Message
                        End Try

                        '==========================================================
                    Case 1 'funzione print to printer

                        Try
                            _rptRicFiscA5.PrintOptions.PrinterName = _printName
                            _rptRicFiscA5.PrintToPrinter(_numeroCopie, CBool(_strFlagFascicola), _startPage, _endPage)
                            msg_stampa &= "OK! Stampa riuscita sulla stampante " & _printName
                            'risp = True

                        Catch ex As Exception
                            'primo tentativo
                            logErrori &= "PrintToPrinter: Stampa del report non riuscita sulla stampante " & _printName & ": " & ex.Message & vbCrLf
                            msg_stampa &= "PrintToPrinter: Stampa del report non riuscita sulla stampante " & _printName & ": " & ex.Message & vbCrLf

                            Try
                                'obsoleto, sostituire con lettura chiave lista_stampanti sul config_siti
                                Dim vetStampanti As String()
                                vetStampanti = Recupera_Lista_Stampanti_ByWebConfig()

                                If Not IsNothing(vetStampanti) Then
                                    Dim i As Integer
                                    For i = 0 To vetStampanti.Length - 1
                                        _printName = vetStampanti(i)
                                        If _printName <> "" Then
                                            msg_stampa &= "Tentativo di stampa sulla stampante " & _printName & "."
                                            Try
                                                _rptRicFiscA5.PrintOptions.PrinterName = _printName
                                                _rptRicFiscA5.PrintToPrinter(_numeroCopie, CBool(_strFlagFascicola), _startPage, _endPage)
                                                msg_stampa &= "OK! Stampa riuscita sulla stampante " & _printName
                                                ' risp = True
                                                Exit For
                                            Catch ex3 As Exception
                                                'i-esimo tentativo
                                                logErrori &= "PrintToPrinter: Stampa del report non riuscita sulla stampante " & _printName & ": " & ex.Message
                                                msg_stampa &= "PrintToPrinter: Stampa del report non riuscita sulla stampante " & _printName & ": " & ex.Message
                                            End Try
                                        End If
                                    Next
                                End If
                            Catch ex2 As Exception
                                logErrori &= "Errore durante Recupera_Lista_Stampanti_ByWebConfig: " & ex2.Message
                            End Try

                        End Try

                        '==========================================================
                    Case Else
                        'non gestito
                        logErrori &= "Il file di configurazione dell'AgronicaStampe non è impostato correttamente."
                        msg_stampa &= "Il file di configurazione dell'AgronicaStampe non è impostato correttamente."
                        '==========================================================
                End Select

                '-----------------------------------------
                '---- Salvataggio Log Errori -------------
                '-----------------------------------------
                SalvaLogErrori_Agenda(logErrori, nomeDocumento, _identificazioneDocumento, "RicevutaFiscaleA5.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)

            End If

            If msg_stampa <> "" Then
                AgroMsgBox(msg_stampa, Page)
            End If

            Me.FindControl("Form1").Controls.Add(New LiteralControl("<script language='javascript'>window.close()</script>"))


            'MS Distruzione del report nel caso di stampa su stampante

            _rptRicFiscA5.Close()
            _rptRicFiscA5.Dispose()
            _rptRicFiscA5 = Nothing

            GC.Collect()

        End If


    End Sub


    '#####################################################################################################
    Private Sub Stampa_RicevutaFiscaleA5(ByRef DS As DS_RicevutaFiscaleA5,
                                         ByRef logErrori As String)

        Dim objRicevuta As New RicevutaFiscale_GestioneStampa
        Dim Flag_SuperatoNumDettagliGestiti As Boolean

        Dim Parametro_CONAI As String = ""

        Try

            objRicevuta.Stampa_RicevutaFiscale(_objParametriServer,
                                               _objParametriUtenti,
                                               DS,
                                               logErrori,
                                               enum_TipoStampaRicevutaFiscale.PdfA5,
                                               _identificazioneDocumento,
                                               _piva,
                                               _idAgenda,
                                               _lavCod,
                                               CInt(Session("ASG_ProgressivoGIAS")),
                                               NUM_DETTAGLI_GESTITI,
                                               Flag_SuperatoNumDettagliGestiti,
                                               _dataMovimento,
                                               False,
                                               _nuoviArrotondamenti,
                                               _objConfigStampe)


            Dim Str_NoteIntegrative1 As String = ""
            Dim Str_NoteIntegrative2 As String = ""
            Dim Str_NoteIntegrative3 As String = ""
            Dim Str_Articolo62 As String = ""
            Dim Flag_contributoCONAI As Boolean = False
            Dim Flag_StampaRifOrdine As Boolean = False
            Dim Flag_GradoAlcolico As Boolean = False
            Dim Flag_GestMaterialeVivaistico As Boolean = False
            Dim Flag_CentroAziendalePartenza As Boolean = False
            '########################## Impostazioni Utente Stampa ####################################
            Try

                ImpostazioniUtente_StampaDoc(_objParametriUtenti,
                                             logErrori,
                                             _lavCod,
                                             Str_NoteIntegrative1,
                                             Str_NoteIntegrative2,
                                             Str_NoteIntegrative3,
                                             Str_Articolo62,
                                             Flag_contributoCONAI,
                                             Flag_StampaRifOrdine,
                                             Flag_GradoAlcolico,
                                             Flag_GestMaterialeVivaistico,
                                             Flag_CentroAziendalePartenza)

                If Flag_contributoCONAI = True Then
                    Parametro_CONAI = "CONTRIBUTO CONAI ASSOLTO OVE DOVUTO"
                Else
                    Parametro_CONAI = ""
                End If

            Catch ex As Exception
                logErrori &= "- Lettura impostazioni utente stampa: " & vbCrLf & ex.Message & vbCrLf
            End Try

            If Flag_SuperatoNumDettagliGestiti = True Then

                _rptRicFiscA5.Section4.SectionFormat.EnableSuppress = False

            Else
                'numero dettagli ok

                If Not IsNothing(DS) Then

                    Dim i As Integer
                    For i = 0 To DS.Dettagli.Rows.Count - 1

                        Try

                            Compila_Txt_Fisse(i,
                                              DS.Dettagli.Rows(i).Item("Qta"),
                                              DS.Dettagli.Rows(i).Item("Udm_Sim"),
                                              DS.Dettagli.Rows(i).Item("Descrizione"),
                                              DS.Dettagli.Rows(i).Item("Importo"))


                        Catch ex As Exception
                            logErrori &= "- Visualizzazione dettaglio " & CStr(i + 1) & " della ricevuta Fiscale: " & vbCrLf & ex.Message & vbCrLf
                        End Try

                    Next 'dettagli

                End If

            End If 'Flag_SuperatoNumDettagliGestiti

            '====================================================================================

        Catch ex As Exception
            logErrori &= "- RicevutaFiscale_GestioneStampa: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            'imposto il DataSet sul report
            _rptRicFiscA5.SetDataSource(DS)

        Catch ex As Exception
            logErrori &= "- Aggancio DataSet: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '  Giulia, 05/09/2016 12.46.49: IMPORTANTE!!!! I parametri vanno valorizzati dopo aver fatto il data binding con rpt.SetDataSource(DS)
        Try
            _rptRicFiscA5.SetParameterValue("Contributo_CONAI", Parametro_CONAI)

        Catch ex As Exception
            logErrori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try


    End Sub

    '###########################################################
    Private Sub Compila_Txt_Fisse(ByVal i As Integer,
                                  ByVal qta As String,
                                  ByVal udmSim As String,
                                  ByVal descrizione As String,
                                  ByVal importoRiga As String)

        i += 1

        Dim rigaQta As String = "Riga" & CStr(i) & "Qta"
        Dim rigaUdm As String = "Riga" & CStr(i) & "Udm"
        Dim rigaDesc As String = "Riga" & CStr(i) & "Desc"
        Dim rigaImporto As String = "Riga" & CStr(i) & "Importo"

        CType(_rptRicFiscA5.Section3.ReportObjects(rigaQta), CrystalDecisions.CrystalReports.Engine.TextObject).Text = qta 'Format(qta, "#,###,##0.####")
        CType(_rptRicFiscA5.Section3.ReportObjects(rigaUdm), CrystalDecisions.CrystalReports.Engine.TextObject).Text = udmSim
        CType(_rptRicFiscA5.Section3.ReportObjects(rigaDesc), CrystalDecisions.CrystalReports.Engine.TextObject).Text = descrizione
        CType(_rptRicFiscA5.Section3.ReportObjects(rigaImporto), CrystalDecisions.CrystalReports.Engine.TextObject).Text = importoRiga 'Format(importoRiga, "#,###,##0.00##")

        'COPIA CLIENTE
        CType(_rptRicFiscA5.Section3.ReportObjects("C" & rigaQta), CrystalDecisions.CrystalReports.Engine.TextObject).Text = qta 'Format(qta, "#,###,##0.####")
        CType(_rptRicFiscA5.Section3.ReportObjects("C" & rigaUdm), CrystalDecisions.CrystalReports.Engine.TextObject).Text = udmSim
        CType(_rptRicFiscA5.Section3.ReportObjects("C" & rigaDesc), CrystalDecisions.CrystalReports.Engine.TextObject).Text = descrizione
        CType(_rptRicFiscA5.Section3.ReportObjects("C" & rigaImporto), CrystalDecisions.CrystalReports.Engine.TextObject).Text = importoRiga ' Format(importoRiga, "#,###,##0.00##")

    End Sub

End Class
