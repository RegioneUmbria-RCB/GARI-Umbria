Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Report_PresentazioneRIBA
    Inherits System.Web.UI.Page

    Private _rptRiba As Rpt_RegistroRiBa

    Private _logErrori As String = ""

    Private _report As Integer
    Private _piva As String
    ' Private Data_Inizio As String
    ' Private Data_Fine As String
    ' Private Anno As Integer
    Private _codRapporto As Integer
    Private _codRisUm As Integer
    Private _tipoScadenza, _tipoNumeroDoc, _numeroDoc, _annoDoc As Integer
    Private _scadenza, _dataPagamento As Date
    Private _codIstitutoDare, _codIstitutoAvere, _codLiquiditaDare, _codLiquiditaAvere As Integer
    Private _flagPagamentoNonAvvenuto As Boolean
    Private _tipo As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))

        'Anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        'Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        'Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _codRisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server)

        _codRapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")), AgroKey_EncoderDecoder, Server)

        _tipoScadenza = Stringa_Decodifica(CStr(Request.QueryString("tsc")), AgroKey_EncoderDecoder, Server)

        _scadenza = Stringa_Decodifica(CStr(Request.QueryString("sc")), AgroKey_EncoderDecoder, Server)

        _tipoNumeroDoc = Stringa_Decodifica(CStr(Request.QueryString("tnd")), AgroKey_EncoderDecoder, Server)

        _numeroDoc = Stringa_Decodifica(CStr(Request.QueryString("nd")), AgroKey_EncoderDecoder, Server)

        _annoDoc = Stringa_Decodifica(CStr(Request.QueryString("ad")), AgroKey_EncoderDecoder, Server)

        'riba / anticipo fattura
        _tipo = Stringa_Decodifica(CStr(Request.QueryString("tipo")), AgroKey_EncoderDecoder, Server)

        '--------------------------------
        'IMPRESA
        _codIstitutoDare = Stringa_Decodifica(CStr(Request.QueryString("cid")), AgroKey_EncoderDecoder, Server)

        _codLiquiditaDare = Stringa_Decodifica(CStr(Request.QueryString("cld")), AgroKey_EncoderDecoder, Server)
        '--------------------------------


        '--------------------------------
        'CONTATTO
        _codIstitutoAvere = Stringa_Decodifica(CStr(Request.QueryString("cia")), AgroKey_EncoderDecoder, Server)

        _codLiquiditaAvere = Stringa_Decodifica(CStr(Request.QueryString("cla")), AgroKey_EncoderDecoder, Server)
        '--------------------------------

        _flagPagamentoNonAvvenuto = Stringa_Decodifica(CStr(Request.QueryString("flapag")), AgroKey_EncoderDecoder, Server)

        _dataPagamento = Stringa_Decodifica(CStr(Request.QueryString("dp")), AgroKey_EncoderDecoder, Server)



        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim nomeDocumento As String = "Registro_RiBa"
        Dim identificazioneDocumento As String = ""

        _rptRiba = New Rpt_RegistroRiBa

        If Not Me.IsPostBack Then

            Try

                Dim dsRiba As New DS_RegistroRiBa

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_RegistroRiBa(dsRiba)

                Select Case _tipo
                    Case 0 'riba
                        'il titolo è già a posto
                    Case 1 'anticipo fatture
                        CType(_rptRiba.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Distinta richiesta anticipazioni: Fatture"
                End Select

                Try

                    '--------------------------------------------
                    ' AGGANCIO DATI
                    '--------------------------------------------
                    _rptRiba.SetDataSource(dsRiba)

                Catch ex As Exception
                    _logErrori &= "- Aggancio dataset al report: " & vbCrLf & ex.Message & vbCrLf
                End Try


            Catch exc As Exception
                _logErrori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Try

                Dim dataInizioAllegati As Date = AGRODATAINIZIO
                Dim dataFineAllegati As Date = AGRODATAFINE
                identificazioneDocumento &= "_" & Format(dataInizioAllegati, "yyyy_MM_dd") & "_" & Format(dataFineAllegati, "yyyy_MM_dd")


                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.PresentazioneRiba, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptRiba,
                                           enum_CategorieDocumenti.PresentazioneRiba,
                                           sottocartella,
                                           nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf",
                                           _objParametriServer,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                enum_CategorieDocumenti.PresentazioneRiba,
                                                                                nomeDocumento,
                                                                                nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf",
                                                                                sottocartella,
                                                                                "", "", "", "",
                                                                                dataInizioAllegati,
                                                                                dataFineAllegati,
                                                                                _objParametriServer)



            Catch ex As Exception
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'Eliminato passaggio report in session per giro su file: Session("Report") = rptRegCorrispettivi
            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptRiba.SaveAs(reportTemporano, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'Dispose del report per evitare problema deallocazione.
            _rptRiba.Close()
            _rptRiba.Dispose()
            _rptRiba = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = "Partita Iva = " & CStr(_piva) &
                                      ", Cod_Istituto_DARE = " & CStr(_codIstitutoDare) & ", Cod_Istituto_AVERE = " & CStr(_codIstitutoAvere) &
                                      ", Cod_Liquidita_DARE = " & CStr(_codLiquiditaDare) & ", Cod_Liquidita_AVERE = " & CStr(_codLiquiditaAvere) &
                                      ", Codice Risorsa Umana = " & CStr(_codRisUm) & ", Codice Rapporto Contabile = " & CStr(_codRapporto) &
                                      ", Scadenza = " & CStr(_scadenza)
            SalvaLogErrori_Generico(_logErrori, nomeDocumento, identificazioneDocumento, "Report_presentazioneRIBA.aspx", "Stampe_Contabilita", infoInLog, _objParametriServer)
            '-----------------------------------------

            Dim pdf As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder, Server)

            If pdf = "1" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                    "&tmpReportPath=" & Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
            End If


        End If

        '==================================================================

        'Try
        '    Session("Report") = rptRIBA
        '    Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

        'Catch ex As Exception
        '    Log_Errori &= "- Export: " & vbCrLf & ex.Message & vbCrLf
        'End Try

    End Sub


    '#####################################################################################################
    Private Sub Stampa_RegistroRiBa(ByRef dsRiba As DS_RegistroRiBa)

        Dim dt As DataTable
        Dim lavCod As Integer = 0

        Try

            Dim objRiba As New AgronicaCoreStampeDAL.RegistriContab

            dt = objRiba.RegistroRIBA(_piva,
                                      _codRisUm,
                                      _codRapporto,
                                      lavCod,
                                      _codIstitutoDare,
                                      _codIstitutoAvere,
                                      _codLiquiditaDare,
                                      _codLiquiditaAvere,
                                      _dataPagamento,
                                      _tipoScadenza,
                                      _scadenza,
                                      AGRODATAFINE,
                                      _tipoNumeroDoc,
                                      _numeroDoc,
                                      _annoDoc,
                                      _tipo,
                                      CBI_Riba_Causali.Nessuno,
                                      "", "",
                                      _objParametriServer)

            ValorizzazioneRegistroRiba(_piva, dt, dsRiba, _logErrori)

        Catch ex As Exception
            _logErrori &= "Lettura delle Ri.Ba: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '########################################################################
        '########################################################################

    End Sub

End Class