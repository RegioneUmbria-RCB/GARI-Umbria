Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreStampeDAL

Partial Class RicevutaFiscaleA4
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private _rptRicFiscA4 As Rpt_RicevutaFiscaleA4

    Private _piva As String
    Private _lavCod As Integer
    Private _idAgenda As Integer
    Private _dataMovimento As Date
    ' Private mode_preview As enum_StampaConSenzaPreview
    Private _identificazioneDocumento As String = ""

    Private _objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _objParametriUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private _nuoviArrotondamenti As Boolean = False
    Private _objConfigStampe As ConfigurazioneStampe = Nothing

    Const NUM_DETTAGLI_GESTITI As Integer = 30

#Region " RICEVUTA FISCALE A4 "

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

        _rptRicFiscA4 = New Rpt_RicevutaFiscaleA4

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

        'mode_preview = CInt(Stringa_Decodifica(CStr(Request.QueryString("mp")), AgroKey_EncoderDecoder,  Server))

        'PrintToPrinter = Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server)

        'PrintName = Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server)

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _objParametriUtenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim nomeDocumento As String = "RicevutaFiscale"
        Dim logErrori As String

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim ds As New DS_RicevutaFiscaleA5

            Dim catCod As Integer = enum_CategorieDocumenti.RicevutaFiscale

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            logErrori = ""

            Try
                Stampa_RicevutaFiscaleA4(ds, logErrori)
            Catch exc As Exception
                logErrori &= "- Stampa_RicevutaFiscaleA4: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Dim nomeFile As String = ""

            Try

                _identificazioneDocumento = nomeDocumento & "_p" & _piva & "_" & _identificazioneDocumento & "_A4"
                nomeFile = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(_identificazioneDocumento) & ".pdf"
                
                ' leggo la sotto cartella da CategorieDocumenti
                Dim sottoCartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                sottoCartella = objCatDoc.Sottocartella(catCod, "", "", _objParametriServer)
                objCatDoc = Nothing

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptRicFiscA4,
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


            'MS Eliminato passaggio report in session per problema deallocazione memoria: Session("Report") = rptRicFiscA4

            'MS Passaggio report su file al visualizzatore
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptRicFiscA4.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Una volta persistito il report con i dati su file distruggo rpt e DataSet ed eseguo un GC.Collect
            'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.
            ds.Dispose()
            ds = Nothing

            _rptRicFiscA4.Close()
            _rptRicFiscA4.Dispose()
            _rptRicFiscA4 = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            SalvaLogErrori_Agenda(logErrori, nomeDocumento, _identificazioneDocumento, "RicevutaFiscaleA4.aspx", "Stampe_Contabilita", _idAgenda, _objParametriServer)

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                              "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(nomeFile, AgroKey_EncoderDecoder, Server))
            
        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_RicevutaFiscaleA4(ByRef ds As DS_RicevutaFiscaleA5,
                                         ByRef logErrori As String)


        Dim objRicevuta As New RicevutaFiscale_GestioneStampa
        Dim Flag_SuperatoNumDettagliGestiti As Boolean

        Dim Parametro_CONAI As String = ""

        Try

            objRicevuta.Stampa_RicevutaFiscale(_objParametriServer,
                                               _objParametriUtenti,
                                               ds,
                                               logErrori,
                                               enum_TipoStampaRicevutaFiscale.PdfA4Singola,
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

                _rptRicFiscA4.Section4.SectionFormat.EnableSuppress = False

            Else
                'numero dettagli ok

                If Not IsNothing(ds) Then

                    Dim i As Integer
                    For i = 0 To ds.Dettagli.Rows.Count - 1

                        Try

                            Compila_Txt_Fisse(i,
                                              ds.Dettagli.Rows(i).Item("Qta"),
                                              ds.Dettagli.Rows(i).Item("Udm_Sim"),
                                              ds.Dettagli.Rows(i).Item("Descrizione"),
                                              ds.Dettagli.Rows(i).Item("Importo")
                                              )


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
            _rptRicFiscA4.SetDataSource(ds)

        Catch ex As Exception
            logErrori &= "- Aggancio DataSet: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '  Giulia, 05/09/2016 12.46.49: IMPORTANTE!!!! I parametri vanno valorizzati dopo aver fatto il data binding con rpt.SetDataSource(DS)
        Try
            _rptRicFiscA4.SetParameterValue("Contributo_CONAI", Parametro_CONAI)

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

        CType(_rptRicFiscA4.Section3.ReportObjects(rigaQta), CrystalDecisions.CrystalReports.Engine.TextObject).Text = qta 'Format(qta, "#,###,##0.####")
        CType(_rptRicFiscA4.Section3.ReportObjects(rigaUdm), CrystalDecisions.CrystalReports.Engine.TextObject).Text = udmSim
        CType(_rptRicFiscA4.Section3.ReportObjects(rigaDesc), CrystalDecisions.CrystalReports.Engine.TextObject).Text = descrizione
        CType(_rptRicFiscA4.Section3.ReportObjects(rigaImporto), CrystalDecisions.CrystalReports.Engine.TextObject).Text = importoRiga 'Format(importoRiga, "#,###,##0.00##")

    End Sub

End Class
