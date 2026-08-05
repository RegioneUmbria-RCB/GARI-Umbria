Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreContabHLP.Contabilita

Public Class Pagamenti
    Inherits System.Web.UI.Page

    Private _rptPag As Rpt_Pagamenti
    Private _logErrori As String

    Private _report As Integer
    Private _piva, _ragSoc, _nomeAgente, _sezionaleDes As String
    Private _dataInizio As String
    Private _dataFine As String
    Private _anno As Integer
    Private _codRapporto As Integer
    Private _codRisUm As Integer
    Private _tipoScadenza As Integer
    Private _scadenza As String
    Private _tipoFiltro As Integer
    Private _codRisUmAgente As Integer
    Private _sezionaleCod As Integer
    Private _listaTipiPag As String
    Private _ordinamento As enum_ReportInsoluti_Ordinamento
    Private _nomeDocumento As String = ""
    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " SCADENZARIO CLIENTI FORNITORI "

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

        _rptPag = New Rpt_Pagamenti

    End Sub

#End Region


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        _report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))

        _anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        _dataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        _dataFine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _codRisUm = Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server)

        _codRapporto = Stringa_Decodifica(CStr(Request.QueryString("cr")), AgroKey_EncoderDecoder, Server)

        _tipoScadenza = Stringa_Decodifica(CStr(Request.QueryString("tsc")), AgroKey_EncoderDecoder, Server)

        _scadenza = Stringa_Decodifica(CStr(Request.QueryString("sc")), AgroKey_EncoderDecoder, Server)

        _ordinamento = Stringa_Decodifica(CStr(Request.QueryString("ord")), AgroKey_EncoderDecoder, Server)

        _sezionaleCod = Stringa_Decodifica(CStr(Request.QueryString("szc")), AgroKey_EncoderDecoder, Server)

        _sezionaleDes = Stringa_Decodifica(CStr(Request.QueryString("szd")), AgroKey_EncoderDecoder, Server)

        '0 = solo insoluti
        If Not IsNothing(Request.QueryString("tf")) Then
            _tipoFiltro = Stringa_Decodifica(CStr(Request.QueryString("tf")), AgroKey_EncoderDecoder, Server)
        Else
            _tipoFiltro = 0
        End If

        If Not IsNothing(Request.QueryString("cra")) Then
            _codRisUmAgente = Stringa_Decodifica(CStr(Request.QueryString("cra")), AgroKey_EncoderDecoder, Server)
            _nomeAgente = Stringa_Decodifica(CStr(Request.QueryString("ag")), AgroKey_EncoderDecoder, Server)
        Else
            _codRisUmAgente = 0
        End If

        If Not IsNothing(Request.QueryString("tp")) Then
            _listaTipiPag = Stringa_Decodifica(CStr(Request.QueryString("tp")), AgroKey_EncoderDecoder, Server)
        Else
            _listaTipiPag = ""
        End If

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim identificazioneDocumento As String = ""
        Dim catCod As enum_CategorieDocumenti

        Select Case _report
            Case enum_CodificaStampe.Lista_InsolutiClienti
                _nomeDocumento = "ScadenzarioClienti"
                catCod = enum_CategorieDocumenti.ScadenziarioPagamentiClienti
            Case enum_CodificaStampe.Lista_InsolutiFornitori
                _nomeDocumento = "ScadenzarioFornitori"
                catCod = enum_CategorieDocumenti.ScadenziarioPagamentiFornitori
        End Select

        If Not Me.IsPostBack Then

            Try

                Dim dsPag As New DS_Pagamenti

                _1_StampaInsoluti(dsPag)

                Try

                    _4_StampaDettagliContatto()

                Catch ex As Exception
                    _logErrori &= "- Lettura dettagli contatto: " & vbCrLf & ex.Message & vbCrLf
                End Try


            Catch exc As Exception
                _logErrori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Dim nomeFilePdf As String = ""

            Try

                Dim dataInizioAllegati As Date = _dataInizio
                Dim dataFineAllegati As Date = _dataFine
                identificazioneDocumento &= "_" & Format(dataInizioAllegati, "yyyy_MM_dd") & "_" & Format(dataFineAllegati, "yyyy_MM_dd")
                nomeFilePdf = _nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf"

                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(catCod, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptPag,
                                           catCod, sottoCartella, nomeFilePdf,
                                           _objParametriServer,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                catCod, _nomeDocumento,
                                                                                nomeFilePdf, sottoCartella,
                                                                                "", "", "", "",
                                                                                dataInizioAllegati,
                                                                                dataFineAllegati,
                                                                                _objParametriServer)

            Catch ex As Exception
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try


            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptPag
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptPag.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Dispose del report per evitare problema deallocazione.
            _rptPag.Close()
            _rptPag.Dispose()
            _rptPag = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = ", Partita Iva = " & CStr(_piva) &
                                      ", Data Inizio = " & CStr(_dataInizio) & ", Data Fine = " & CStr(_dataFine) &
                                      ", Codice Risorsa Umana = " & CStr(_codRisUm) & ", Codice Rapporto Contabile = " & CStr(_codRapporto) &
                                      ", Scadenza = " & CStr(_scadenza)
            SalvaLogErrori_Generico(_logErrori, _nomeDocumento, identificazioneDocumento, "Scadenzario_ClientiFornitori", "Stampe_Contabilita", infoInLog, _objParametriServer)
            '-----------------------------------------

            Dim pdf As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder, Server)

            If pdf = "1" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
            End If

        End If

    End Sub

    '#####################################################################################################
    Private Sub _4_StampaDettagliContatto()

        If _codRisUm <> 0 Then

            Dim objcont As New AgronicaCoreAnagrafeDAL.Contatti_R
            Dim dtCont As DataTable
            Dim i As Integer

            dtCont = objcont.Contatti_Contatto_Leggi("", "",
                                                     _codRisUm,
                                                     0, True,
                                                     True, 0, 0,
                                                     True, 0,
                                                     ID_CF_NOFILTRO,
                                                     0, "", True,
                                                     0, 0, 0, 0, 0,
                                                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                                                     "", "",
                                                     _objParametriServer)

            If Not IsNothing(dtCont) AndAlso dtCont.Rows.Count > 0 Then

                Dim Riga_Contatto1 As String = ""
                Dim Riga_Contatto2 As String = ""
                Dim Piva_contatto, Codice_Fiscale_contatto As String
                Dim Flag_PersonaPrivato As Boolean = False
                Dim Numero_rubrica, Descrizione_Rubrica As String
                Dim x_Telefono, x_Cell, x_Fax, x_Email, x_SitoWeb, x_PEC, x_Stato As String
                Dim x_IndDes, x_FrzDes, x_Cap, x_Comune, x_Provincia As String
                Dim Domicilio, Residenza, SedeAziendale, SedeLegale, SedeOperativa, Stabilimento As String

                _rptPag.Section6.SectionFormat.EnableSuppress = False

                For i = 0 To dtCont.Rows.Count - 1

                    With dtCont.Rows(i)

                        If i = 0 Then
                            Riga_Contatto1 = .Item("Rag_soc") & " " & .Item("Nome") & " " & .Item("Cognome")

                            If .Item("id_CF") = PERSONA_FISICA Then
                                Flag_PersonaPrivato = True
                            End If

                            Ricava_Piva_Codicefiscale(.Item("id_CF"),
                                                      .Item("cod_contatto"),
                                                      .Item("codice_fiscale"),
                                                        .Item("chkfittizio"),
                                                      Piva_contatto,
                                                      Codice_Fiscale_contatto,
                                                      Flag_PersonaPrivato)

                            If Piva_contatto <> "" Then
                                Riga_Contatto1 &= " - Piva: " & Piva_contatto
                            End If
                            If Codice_Fiscale_contatto <> "" Then
                                Riga_Contatto1 &= " - Codice Fiscale: " & Codice_Fiscale_contatto
                            End If
                        End If

                        x_IndDes = .Item("ind_des")
                        x_FrzDes = .Item("frz_des")
                        x_Cap = .Item("cap")

                        Select Case .Item("id_cf")

                            Case enum_Contatti_IdCf.ContattoEstero
                                'ricilo il campo x_Comune
                                If .Item("Stato_PaesiTerzi") <> "" Then
                                    x_Comune = .Item("Stato_PaesiTerzi")
                                ElseIf .Item("Stato_PaesiMembri") <> "" Then
                                    x_Comune = .Item("Stato_PaesiMembri")
                                End If
                                x_Provincia = ""

                            Case Else
                                x_Comune = .Item("com_des")
                                x_Provincia = .Item("pro_cod")
                                x_Stato = .Item("stato")
                                Sistema_Comune_Provincia(x_Comune, x_Provincia)

                        End Select

                        If x_IndDes <> "" AndAlso (x_Comune <> "" OrElse x_Provincia <> "") Then
                            Dim indirizzo As String = x_IndDes & " " & x_FrzDes & " " & x_Cap & " " & x_Comune & " " & x_Provincia & " " & x_Stato

                            Select Case .Item("Tipo_Indirizzo")

                                Case enum_IndirizzoTipo.Domicilio
                                    Domicilio = "Domicilio: " & indirizzo & " - "

                                Case enum_IndirizzoTipo.Residenza
                                    Residenza = "Residenza: " & indirizzo & " - "

                                Case enum_IndirizzoTipo.SedeAziendale
                                    SedeAziendale = "Sede az.: " & indirizzo & " - "

                                Case enum_IndirizzoTipo.SedeLegale
                                    SedeLegale = "Sede legale: " & indirizzo & " - "

                                Case enum_IndirizzoTipo.SedeOperativa
                                    SedeOperativa = "Sede op.: " & indirizzo & " - "

                                Case enum_IndirizzoTipo.Stabilimento
                                    Stabilimento = "Stabilimento: " & indirizzo & " - "
                            End Select

                        End If

                        Numero_rubrica = dtCont.Rows(i).Item("numero")
                        Descrizione_Rubrica = dtCont.Rows(i).Item("descr")

                        If InStr(1, Descrizione_Rubrica.ToLower, "tel", CompareMethod.Text) > 0 Then
                            x_Telefono = "Tel: " & Numero_rubrica
                        ElseIf InStr(1, Descrizione_Rubrica.ToLower, "cel", CompareMethod.Text) > 0 Then
                            x_Cell = "Cell: " & Numero_rubrica
                        ElseIf InStr(1, Descrizione_Rubrica.ToLower, "fax", CompareMethod.Text) > 0 Then
                            x_Fax = "Fax: " & Numero_rubrica
                        ElseIf InStr(1, Descrizione_Rubrica.ToLower, "mail", CompareMethod.Text) > 0 Then
                            x_Email = "Email: " & Numero_rubrica
                        ElseIf InStr(1, Descrizione_Rubrica.ToLower, "sito", CompareMethod.Text) > 0 Then
                            x_SitoWeb = "Sito: " & Numero_rubrica
                        ElseIf InStr(1, Descrizione_Rubrica.ToLower, "pec", CompareMethod.Text) > 0 Then
                            x_PEC = "PEC: " & Numero_rubrica
                        End If

                    End With

                Next

                If x_Telefono <> "" Then
                    Riga_Contatto1 &= " - " & x_Telefono
                End If

                If x_Email <> "" Then
                    Riga_Contatto1 &= " - " & x_Email
                End If

                'If x_SitoWeb <> "" Then
                '    Riga_Contatto1 &= " " & x_Telefono
                'End If

                If x_PEC <> "" Then
                    Riga_Contatto1 &= " - " & x_PEC
                End If

                If x_Fax <> "" Then
                    Riga_Contatto1 &= " - " & x_Fax
                End If

                If x_Cell <> "" Then
                    Riga_Contatto1 &= " - " & x_Cell
                End If

                If Domicilio <> "" Then
                    Riga_Contatto2 &= Domicilio
                End If
                If Residenza <> "" Then
                    Riga_Contatto2 &= Residenza
                End If

                If SedeLegale <> "" Then
                    Riga_Contatto2 &= SedeLegale
                End If
                If SedeAziendale <> "" Then
                    Riga_Contatto2 &= SedeAziendale
                End If
                If SedeOperativa <> "" Then
                    Riga_Contatto2 &= SedeOperativa
                End If
                If Stabilimento <> "" Then
                    Riga_Contatto2 &= Stabilimento
                End If

                CType(_rptPag.Section6.ReportObjects("TxtContatto1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Riga_Contatto1
                CType(_rptPag.Section6.ReportObjects("TxtContatto2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Riga_Contatto2

            End If

        End If 'impostato filtro sul contatto

    End Sub


    '#####################################################################################################
    Private Function _1_StampaInsoluti(ByRef dsPag As DS_Pagamenti)

        Dim flagDebiti As Boolean

        'CType(rptPag.Section1.ReportObjects("TxtDataInizio"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Inizio
        'CType(rptPag.Section1.ReportObjects("TxtDataFine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Fine

        '########################################################################
        '########################################################################

        Try

            Select Case _report

                Case enum_CodificaStampe.Lista_InsolutiClienti

                    flagDebiti = False

                    CType(_rptPag.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Scadenzario Clienti"
                    CType(_rptPag.Section2.ReportObjects("TxtImporto1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Importo  già  incassato"
                    CType(_rptPag.Section2.ReportObjects("TxtImporto2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Importo da incassare"
                    CType(_rptPag.Section4.ReportObjects("TxtTotale1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Totale  già  incassato"
                    CType(_rptPag.Section4.ReportObjects("TxtTotale2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Totale da Incassare"

                Case enum_CodificaStampe.Lista_InsolutiFornitori

                    flagDebiti = True

                    CType(_rptPag.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Scadenzario Fornitori"
                    CType(_rptPag.Section2.ReportObjects("TxtImporto1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Importo  già  pagato"
                    CType(_rptPag.Section2.ReportObjects("TxtImporto2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Importo da pagare"
                    CType(_rptPag.Section4.ReportObjects("TxtTotale1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Totale  già  pagato"
                    CType(_rptPag.Section4.ReportObjects("TxtTotale2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Totale da pagare"
            End Select

        Catch ex As Exception
            _logErrori &= "- _1_" & _nomeDocumento & ": " & vbCrLf & ex.Message & vbCrLf
        End Try

        '########################################################################
        '########################################################################

        Dim dtInsoluti As DataTable = Nothing
        Dim objInsoluti As New AgronicaCoreStampeDAL.RegistriContab

        Try

            dtInsoluti = objInsoluti.Lista_Insoluti_Movimenti(flagDebiti,
                                                              _piva,
                                                              _codRisUm,
                                                              _codRapporto,
                                                              "",
                                                              "",
                                                              _codRisUmAgente,
                                                              _sezionaleCod,
                                                              _dataInizio,
                                                              _dataFine,
                                                              _tipoScadenza,
                                                              _scadenza,
                                                              False,
                                                              _listaTipiPag,
                                                              "",
                                                              "",
                                                              "",
                                                              _ordinamento,
                                                              _objParametriServer)

        Catch ex As Exception
            _logErrori &= "- Lettura dei movimenti: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim Str_FiltroIdAgenda As String = ""
        Dim dtCodifichePat As DataTable
        Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R

        Try

            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            dtCodifichePat = objRicXConti.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, 0, CDate(_dataInizio).Year, "", _objParametriServer)

        Catch ex As Exception

        End Try


        Dim i As Integer

        Try

            If Not IsNothing(dtInsoluti) AndAlso dtInsoluti.Rows.Count > 0 Then

                Str_FiltroIdAgenda = " AND Pagamenti.Id_Agenda IN ( "

                For i = 0 To dtInsoluti.Rows.Count - 1
                    Str_FiltroIdAgenda &= CStr(dtInsoluti.Rows(i).Item("Id_Agenda")) & ","

                    If dtInsoluti.Rows(i).Item("lav_cod") = LAVCOD_FATTURA_PROFESSIONISTI Then
                        dtInsoluti.Rows(i).Item("num_protocollo") = dtInsoluti.Rows(i).Item("num_protocollo") - dtInsoluti.Rows(i).Item("Tot_Ritenute_Enasarco")
                    End If
                Next

                Str_FiltroIdAgenda = Left(Str_FiltroIdAgenda, Str_FiltroIdAgenda.Length - 1)
                Str_FiltroIdAgenda &= ")"

            End If

        Catch ex As Exception
            _logErrori &= "- Filtro id_agenda: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim dtPag As DataTable

        Try

            dtPag = objInsoluti.Lista_Insoluti_Pagamenti(_piva,
                                                         0,
                                                         0,
                                                         0,
                                                         0,
                                                         0, 0,
                                                         False,
                                                         False,
                                                         enum_Pagamento.NonDefinito,
                                                         Str_FiltroIdAgenda,
                                                         dtCodifichePat,
                                                         "",
                                                         _objParametriServer)

        Catch ex As Exception
            _logErrori &= "- Lettura dei pagamenti: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Dim TOT_Iva As Decimal = 0
        Dim TOT_Imponibile As Decimal = 0
        Dim TOT_Importo As Decimal = 0
        Dim TOT_Importo_DaIncassarePagare As Decimal = 0
        Dim TOT_Importo_RiscossoPagato As Decimal = 0
        Dim Importo_da_Pagare As Decimal = 0
        Dim Importo_gia_Pagato As Decimal = 0
        Dim flagRiscossioni As Integer = 0
        Dim Modalita_Pag_Prevista As String = ""
        Dim Modalita_Pag_Effettuata As String = ""

        Try

            If Not IsNothing(dtInsoluti) AndAlso dtInsoluti.Rows.Count > 0 Then

                For i = 0 To dtInsoluti.Rows.Count - 1

                    Try

                        'If i = 0 Then
                        '    CType(rptPag.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtInsoluti.Rows(i).Item("Impresa")
                        'End If

                        _2a_Analizza_Pagamenti(dtPag,
                                               dtInsoluti.Rows(i).Item("id_agenda"),
                                               dtInsoluti.Rows(i).Item("id_mov"),
                                               dtInsoluti.Rows(i).Item("lav_cod"),
                                               dtInsoluti.Rows(i).Item("num_protocollo"),
                                               Modalita_Pag_Prevista,
                                               Modalita_Pag_Effettuata,
                                               flagRiscossioni,
                                               Importo_gia_Pagato,
                                               Importo_da_Pagare)

                        Select Case _tipoFiltro

                            Case 0 'INSOLUTI: non riscosse e parzialmente riscosse

                                ' movimenti non riscossi interamente
                                If flagRiscossioni <> 3 Then

                                    _3_Elabora_Record(dsPag,
                                                      dtInsoluti.Rows(i),
                                                      Importo_gia_Pagato,
                                                      Importo_da_Pagare,
                                                      Modalita_Pag_Prevista,
                                                      Modalita_Pag_Effettuata,
                                                      TOT_Iva,
                                                      TOT_Imponibile,
                                                      TOT_Importo,
                                                      TOT_Importo_DaIncassarePagare,
                                                      TOT_Importo_RiscossoPagato)

                                End If

                                '===========================
                            Case 1 'solo non riscosse

                                ' movimenti da riscuotere interamente
                                If flagRiscossioni = 1 Then

                                    _3_Elabora_Record(dsPag,
                                                      dtInsoluti.Rows(i),
                                                      Importo_gia_Pagato,
                                                      Importo_da_Pagare,
                                                      Modalita_Pag_Prevista,
                                                      Modalita_Pag_Effettuata,
                                                      TOT_Iva,
                                                      TOT_Imponibile,
                                                      TOT_Importo,
                                                      TOT_Importo_DaIncassarePagare,
                                                      TOT_Importo_RiscossoPagato)

                                End If

                                '===========================
                            Case 2 'solo parzialmente  riscosse

                                ' movimenti riscossi parzialmente
                                If flagRiscossioni = 2 Then

                                    _3_Elabora_Record(dsPag,
                                                      dtInsoluti.Rows(i),
                                                      Importo_gia_Pagato,
                                                      Importo_da_Pagare,
                                                      Modalita_Pag_Prevista,
                                                      Modalita_Pag_Effettuata,
                                                      TOT_Iva,
                                                      TOT_Imponibile,
                                                      TOT_Importo,
                                                      TOT_Importo_DaIncassarePagare,
                                                      TOT_Importo_RiscossoPagato)

                                End If

                                '===========================
                            Case 3 ' solo riscosse

                                ' movimenti riscossi interamente
                                If flagRiscossioni = 3 Then

                                    _3_Elabora_Record(dsPag,
                                                      dtInsoluti.Rows(i),
                                                      Importo_gia_Pagato,
                                                      Importo_da_Pagare,
                                                      Modalita_Pag_Prevista,
                                                      Modalita_Pag_Effettuata,
                                                      TOT_Iva,
                                                      TOT_Imponibile,
                                                      TOT_Importo,
                                                      TOT_Importo_DaIncassarePagare,
                                                      TOT_Importo_RiscossoPagato)

                                End If

                                '===========================

                            Case 4 'tutte

                                _3_Elabora_Record(dsPag,
                                                  dtInsoluti.Rows(i),
                                                  Importo_gia_Pagato,
                                                  Importo_da_Pagare,
                                                  Modalita_Pag_Prevista,
                                                  Modalita_Pag_Effettuata,
                                                  TOT_Iva,
                                                  TOT_Imponibile,
                                                  TOT_Importo,
                                                  TOT_Importo_DaIncassarePagare,
                                                  TOT_Importo_RiscossoPagato)

                        End Select



                        ''filtro solo insoluti e movimenti non riscossi interamente
                        'If Tipo_Filtro = 0 And Flag_Riscossioni <> 3 Then

                        '    _3_Elabora_Record(dsPag,
                        '                      dtInsoluti.Rows(i),
                        '                      Importo_gia_Pagato,
                        '                      Importo_da_Pagare,
                        '                      Modalita_Pag_Prevista,
                        '                      Modalita_Pag_Effettuata,
                        '                      TOT_Iva,
                        '                      TOT_Imponibile,
                        '                      TOT_Importo,
                        '                      TOT_Importo_DaIncassarePagare,
                        '                      TOT_Importo_RiscossoPagato)

                        'End If


                    Catch ex As Exception
                        _logErrori &= "Errore al giro " & CStr(i) & ": " & vbCrLf & ex.Message & vbCrLf
                    End Try

                Next

            End If

        Catch ex As Exception
            _logErrori &= "- Lettura dei movimenti: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#########################################################

        Try

            'CType(rptPag.Section4.ReportObjects("TxtImportoTotalePag"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(ImportoTotalePagamento, "##,###,##0.00")
            'CType(rptPag.Section4.ReportObjects("TxtImportoTotaleFatt"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(ImportoTotaleDoc, "##,###,##0.00")
            'CType(rptPag.Section4.ReportObjects("TxtIvaTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(IvaTotale, "##,###,##0.00")
            'CType(rptPag.Section4.ReportObjects("TxtImponibileTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(ImponibileTotale, "##,###,##0.00")

            If DsPag.DS_Pagamenti.Rows.Count > 0 Then

                Dim ultimaRiga As Integer = dsPag.DS_Pagamenti.Rows.Count - 1
                Dim dr As DS_Pagamenti.DS_PagamentiRow

                dr = dsPag.DS_Pagamenti.Rows(ultimaRiga)
                dr.Tot_Imponibile = Format(TOT_Imponibile, "##,###,##0.00")
                dr.Tot_Iva = Format(TOT_Iva, "##,###,##0.00")
                dr.Tot_Importo = Format(TOT_Importo, "##,###,##0.00")
                dr.Tot_Importo_RiscossoPagato = Format(TOT_Importo_RiscossoPagato, "##,###,##0.00")
                dr.Tot_Importo_DaIncassarePagare = Format(TOT_Importo_DaIncassarePagare, "##,###,##0.00")

                DsPag.DS_Pagamenti.AcceptChanges()

            End If

        Catch ex As Exception
            _logErrori &= "- Valorizzazione totali: " & vbCrLf & ex.Message & vbCrLf
        End Try

        '#########################################################

        Try

            'imposto il dataset sul report
            _rptPag.SetDataSource(dsPag)

        Catch ex As Exception
            _logErrori &= "- SetDataSource: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            CType(_rptPag.Section1.ReportObjects("TxtPivaRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = _piva & "   " & _ragSoc

            Dim str_filtro As String
            str_filtro = "Periodo dal " & CStr(_dataInizio) & " al " & CStr(_dataFine) & " - "

            If _codRisUmAgente <> 0 Then
                str_filtro &= "Agente: " & CStr(_nomeAgente) & " - "
            End If

            Select Case _tipoFiltro
                Case 0 'INSOLUTI: non riscosse e parzialmente riscosse
                    str_filtro &= "Filtro: da riscuotere interamente o parzialmente - "
                Case 1 'solo non riscosse
                    str_filtro &= "Filtro: da riscuotere interamente - "
                Case 2 'solo parzialmente  riscosse
                    str_filtro &= "Filtro: da riscuotere parzialmente - "
                Case 3 ' solo riscosse
                    str_filtro &= "Filtro: riscossioni avvenute - "
                Case 4 'tutte
                    str_filtro &= "Filtro: nessuno - "
            End Select
            If _sezionaleCod > 0 Then
                str_filtro &= "Sezionale: " & CStr(_sezionaleDes)
            End If

            CType(_rptPag.Section1.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_filtro

        Catch ex As Exception
            _logErrori &= "- Intestazione: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Return _rptPag

    End Function


    '#####################################################################################################
    Public Sub _2a_Analizza_Pagamenti(ByVal Dt_Pag As DataTable,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Id_Mov As Integer,
                                      ByVal Lav_Cod As Integer,
                                      ByRef x_Num_Protocollo As Decimal,
                                      ByRef Modalita_Pag_Prevista As String,
                                      ByRef Modalita_Pag_Effettuata As String,
                                      ByRef Flag_Riscossioni As Integer,
                                      ByRef Tot_Importo_gia_Pagato As Decimal,
                                      ByRef Tot_Importo_da_Pagare As Decimal)

        Try

            Modalita_Pag_Prevista = ""
            Modalita_Pag_Effettuata = ""
            Flag_Riscossioni = 0
            Tot_Importo_gia_Pagato = 0
            Tot_Importo_da_Pagare = 0

            If Not IsNothing(Dt_Pag) AndAlso Dt_Pag.Rows.Count > 0 Then

                Dim j As Integer

                Dim x_Note_Pagamento As String
                Dim x_Percentuale_Pagamento As Decimal
                Dim x_Importo_Pagamento As Decimal
                Dim x_Data_Pagamento As Date
                '  Dim x_Cau_Pagamento As Integer
                Dim x_Cau_Pagamento_Sigla As String
                'Dim x_Cau_Pagamento_Des As String
                Dim x_Tipo_Pagamento As Integer

                Dim Mod_Pag_PercImporto, Mod_Banche As String
                Dim Numero_Tranche As Integer

                Dim Importo_Pagato_Corrente As Decimal = 0


                x_Num_Protocollo = ArrotondaVal_2(x_Num_Protocollo)

                '===============================================================================
                '================          PAGAMENTO PREVISTO        =======================
                '===============================================================================

                Dim drPagPrevisto() As DataRow
                drPagPrevisto = Dt_Pag.Select(" id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) &
                                                " AND id_mov = " & Agro_SQL_SaveNum(Id_Mov) &
                                                " AND Previsto_Avvenuto = 0 ")

                Modalita_Pag_Prevista = ""

                If Not IsNothing(drPagPrevisto) AndAlso drPagPrevisto.Length > 0 Then

                    Numero_Tranche = drPagPrevisto.Length
                    For j = 0 To Numero_Tranche - 1
                        With drPagPrevisto(j)

                            x_Percentuale_Pagamento = CDec(.Item("percentuale"))
                            x_Cau_Pagamento_Sigla = .Item("Cau_Pagamento_Sigla")
                            x_Importo_Pagamento = .Item("importo")
                            x_Data_Pagamento = .Item("data_pagamento")
                            x_Tipo_Pagamento = CInt(.Item("Tipo_Pagamento"))
                            x_Note_Pagamento = .Item("note")

                            If x_Importo_Pagamento = 0 Then
                                Select Case x_Percentuale_Pagamento
                                    Case 0
                                        'eccezione
                                        Mod_Pag_PercImporto = ""
                                    Case 100
                                        'unica tranche per importo totale ==> non lo scrivo, 
                                        'Mod_Pag_PercImporto = " per il 100%"
                                        Mod_Pag_PercImporto = ""
                                    Case Else
                                        'gestione con percentuali
                                        Mod_Pag_PercImporto = " per il " & CStr(x_Percentuale_Pagamento) & "%"
                                End Select
                            Else
                                If Numero_Tranche > 1 Then
                                    'lo scrivo solo se ci sono più tranche
                                    Mod_Pag_PercImporto = " per " & CStr(x_Importo_Pagamento) & "€"
                                Else
                                    Mod_Pag_PercImporto = ""
                                End If
                            End If

                            _2b_LeggiDati_IBAN(drPagPrevisto(j), x_Tipo_Pagamento, Mod_Banche)

                            If Modalita_Pag_Prevista <> "" Then
                                Modalita_Pag_Prevista &= "; "
                            End If
                            Modalita_Pag_Prevista &= x_Cau_Pagamento_Sigla & Mod_Pag_PercImporto & Mod_Banche

                            If x_Note_Pagamento <> "" Then
                                Modalita_Pag_Prevista &= " (" & x_Note_Pagamento & ")"
                            End If

                        End With
                    Next
                End If 'pag previsto


                '===============================================================================
                '================          PAGAMENTO EFFETTUATO        =======================
                '===============================================================================

                Dim drPagEffettuato() As DataRow

                drPagEffettuato = Dt_Pag.Select(" id_agenda = " & Agro_SQL_SaveNum(Id_Agenda) &
                                                " AND id_mov = " & Agro_SQL_SaveNum(Id_Mov) &
                                                " AND Previsto_Avvenuto = 1 ")

                If Not IsNothing(drPagEffettuato) AndAlso drPagEffettuato.Length > 0 Then
                    For j = 0 To drPagEffettuato.Length - 1
                        With drPagEffettuato(j)

                            x_Percentuale_Pagamento = CDec(.Item("percentuale"))
                            ' x_Cau_Pagamento = .Item("cau_pagamento")
                            'x_Cau_Pagamento_Des = .Item("Cau_Pagamento_Des")
                            x_Cau_Pagamento_Sigla = .Item("Cau_Pagamento_Sigla")
                            x_Importo_Pagamento = .Item("importo")
                            x_Data_Pagamento = .Item("data_pagamento")
                            x_Tipo_Pagamento = CInt(.Item("Tipo_Pagamento"))
                            x_Note_Pagamento = .Item("note")

                            Select Case x_Percentuale_Pagamento
                                Case 0
                                    'gestione importo fisso salvato dall'utente
                                    Importo_Pagato_Corrente = x_Importo_Pagamento
                                Case 100
                                    'modifica del 18/02/2013:
                                    'anche se la percentuale è 100%, può essere che l'importo non coincide con il totale
                                    'ad esempio la molinelli aveva questo caso:
                                    'totale= 930,47
                                    '1 tranche =930,46
                                    '2 tranche = 0,01
                                    'Importo_Pagato_Corrente = x_Num_Protocollo
                                    Importo_Pagato_Corrente = x_Importo_Pagamento
                                Case Else
                                    If x_Importo_Pagamento = 0 Then
                                        'TODO: ARROTONDAMENTO??? - direi che si può lasciare così, visto che è un caso che non si dovrebbe mai verificare
                                        'gestione con percentuali
                                        Importo_Pagato_Corrente = (x_Percentuale_Pagamento * x_Num_Protocollo) / 100
                                        Importo_Pagato_Corrente = ArrotondaVal_2(Importo_Pagato_Corrente)
                                    Else
                                        'gestione importo fisso salvato dall'utente
                                        Importo_Pagato_Corrente = x_Importo_Pagamento
                                    End If
                            End Select

                            _2b_LeggiDati_IBAN(drPagEffettuato(j), x_Tipo_Pagamento, Mod_Banche)

                            Tot_Importo_gia_Pagato += Importo_Pagato_Corrente

                            If Modalita_Pag_Effettuata <> "" Then
                                Modalita_Pag_Effettuata &= "; "
                            End If
                            Modalita_Pag_Effettuata &= x_Data_Pagamento.ToShortDateString & " " & x_Cau_Pagamento_Sigla & " per " & CStr(Importo_Pagato_Corrente) & " €" & Mod_Banche

                            If x_Note_Pagamento <> "" Then
                                Modalita_Pag_Effettuata &= " (" & x_Note_Pagamento & ")"
                            End If

                        End With

                    Next

                End If 'effettuato

                Tot_Importo_gia_Pagato = ArrotondaVal_2(Tot_Importo_gia_Pagato)

                'Flag_Riscossioni:
                '1 = totalmente da riscuotere+
                '2 = parzialmente da riscuotere
                '3 = riscossa interamente
                Select Case Tot_Importo_gia_Pagato
                    Case 0
                        Flag_Riscossioni = 1 '  totalmente da riscuotere
                    Case Is = x_Num_Protocollo
                        Flag_Riscossioni = 3 'totalmente riscossa
                    Case Else
                        Flag_Riscossioni = 2 '  parzialmente da riscuotere
                        'in realtà sono già entrambi arrotondati a 2 cifre, quindi la differenza non sarà mai inferiore al centesimo
                        If Math.Abs(x_Num_Protocollo - Tot_Importo_gia_Pagato) < 0.0000001 Then
                            Flag_Riscossioni = 3 'totalmente riscossa
                        End If
                End Select

                Tot_Importo_da_Pagare = x_Num_Protocollo - Tot_Importo_gia_Pagato

                Select Case Lav_Cod
                    Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                        Tot_Importo_da_Pagare = -1 * Tot_Importo_da_Pagare
                        Tot_Importo_gia_Pagato = -1 * Tot_Importo_gia_Pagato
                End Select

                Tot_Importo_da_Pagare = ArrotondaVal_2(Tot_Importo_da_Pagare)

            End If 'dt pagamenti

        Catch ex As Exception
            _logErrori &= "- Analizza_Pagamenti: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub


    '#####################################################################################################
    Private Sub _2b_LeggiDati_IBAN(ByVal dr As DataRow,
                                   ByVal x_Tipo_Pagamento As enum_PagamentiCausali,
                                   ByRef Mod_Banche As String)

        Try

            Dim NsBanca, VsBanca As String
            Dim Banca_DARE, Banca_AVERE As String

            Mod_Banche = ""

            With dr

                Select Case x_Tipo_Pagamento

                    'nei pagamenti in cui ci possono/sono le banche 
                    Case enum_PagamentiCausali.Bonifico,
                        enum_PagamentiCausali.RiBa,
                        enum_PagamentiCausali.RimessaDiretta

                        If .Item("cod_liquidita_dare") <> -1 Then

                            Banca_DARE = .Item("Istituto_Des_DARE")

                            If .Item("Nazione_DARE") <> "" And .Item("Nazione_DARE") <> "0" Then

                                Banca_DARE &= " " & Costruisci_IBAN(.Item("Nazione_DARE"), .Item("Cifre_Controllo_DARE"), .Item("Cin_DARE"),
                                                                    .Item("Abi_DARE"), .Item("Cab_DARE"), .Item("Numero_DARE"), False) & " "

                            End If

                            If .Item("Bic_DARE") <> "" And .Item("Bic_DARE") <> "0" Then
                                Banca_DARE &= " Bic/Swift: " & .Item("Bic_DARE")
                            End If
                            Banca_DARE = Trim(Banca_DARE)
                        Else
                            Banca_DARE = ""
                        End If

                        If .Item("cod_liquidita_avere") <> -1 Then

                            Banca_AVERE = .Item("Istituto_Des_AVERE")

                            If .Item("Nazione_AVERE") <> "" And .Item("Nazione_AVERE") <> "0" Then

                                Banca_AVERE &= " " & Costruisci_IBAN(.Item("Nazione_AVERE"), .Item("Cifre_Controllo_AVERE"), .Item("Cin_AVERE"),
                                                                     .Item("Abi_AVERE"), .Item("Cab_AVERE"), .Item("Numero_AVERE"), False) & " "

                            End If

                            If .Item("Bic_AVERE") <> "" And .Item("Bic_AVERE") <> "0" Then
                                Banca_AVERE &= " Bic/Swift: " & .Item("Bic_AVERE")
                            End If
                            Banca_AVERE = Trim(Banca_AVERE)
                        Else
                            Banca_AVERE = ""
                        End If

                        Select Case _report

                            Case enum_CodificaStampe.Lista_InsolutiClienti

                                NsBanca = Banca_DARE
                                VsBanca = Banca_AVERE

                                If NsBanca <> "" Then
                                    'Mod_Banche &= " NS banca: " & NsBanca & " - "
                                    Mod_Banche &= " " & NsBanca
                                End If
                                If x_Tipo_Pagamento = enum_PagamentiCausali.RiBa AndAlso VsBanca <> "" Then
                                    Mod_Banche &= " VS banca: " & VsBanca
                                End If

                            Case enum_CodificaStampe.Lista_InsolutiFornitori

                                NsBanca = Banca_AVERE
                                VsBanca = Banca_DARE

                                If VsBanca <> "" Then
                                    Mod_Banche &= " " & VsBanca
                                End If
                                If x_Tipo_Pagamento = enum_PagamentiCausali.RiBa AndAlso NsBanca <> "" Then
                                    Mod_Banche &= " NS banca: " & NsBanca
                                End If

                        End Select

                        'If NsBanca <> "" Then
                        '    'Mod_Banche &= " NS banca: " & NsBanca & " - "
                        '    Mod_Banche &= " " & NsBanca
                        'End If
                        'If x_Tipo_Pagamento = enum_PagamentiCausali.RiBa And VsBanca <> "" Then
                        '    Mod_Banche &= " VS banca: " & VsBanca
                        'End If

                End Select

            End With

        Catch ex As Exception
            _logErrori &= "- LeggiDati_IBAN: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    '#####################################################################################################
    Private Sub _3_Elabora_Record(ByRef dsPag As DS_Pagamenti,
                                  ByVal Riga_Dt As DataRow,
                                  ByVal Importo_gia_Pagato As Decimal,
                                  ByVal Importo_da_Pagare As Decimal,
                                  ByVal Modalita_Pag_Prevista As String,
                                  ByVal Modalita_Pag_Effettuata As String,
                                  ByRef TOT_Iva As Decimal,
                                  ByRef TOT_Imponibile As Decimal,
                                  ByRef TOT_Importo As Decimal,
                                  ByRef TOT_Importo_DaIncassarePagare As Decimal,
                                  ByRef TOT_Importo_RiscossoPagato As Decimal)

        Try

            Dim dr As DS_Pagamenti.DS_PagamentiRow

            Dim objHlp As New AgronicaCoreContabHLP.Contabilita
            Dim impNetto, iva, importo As Decimal

            Dim IdAgenda_Memo As Integer = 0
            Dim RagSoc_Memo As String = ""
            Dim NumProtocollo_Memo As Decimal = 0

            Dim Scadenza As String = ""
            Dim Riferimento As String = ""
            Dim Data_Movimento As String = ""
            Dim NumeroDoc As String = ""
            Dim Str_Imponibile_Tot As String
            Dim Str_Iva_Tot As String
            Dim Str_Importo_Tot As String
            Dim lav_cod As Integer
            Dim str_Doc As String

            With Riga_Dt

                lav_cod = .Item("Lav_Cod")

                Select Case lav_cod

                    Case LAVCOD_FATTURA_PROFESSIONISTI
                        str_Doc = "Fatt.Prof.n."

                    Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_RICEVUTA,
                        LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA
                        str_Doc = "Fatt.n."

                    Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                        str_Doc = "Nota acc.n."

                    Case LAVCOD_RICEVUTA_EMESSA
                        str_Doc = "Ric.Fisc.n."

                    Case LAVCOD_DDT_CONTABILIZZATO_EMESSO
                        str_Doc = "DDT n."

                    Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO, LAVCOD_ACQUISTO
                        str_Doc = "Corr.n."

                    Case LAVCOD_ALTRI_RICAVI, LAVCOD_ALTRI_COSTI
                        str_Doc = ""

                    Case LAVCOD_REG_COMPENSI
                        str_Doc = ""

                End Select


                '-----------------------------------------------------------------------------
                '-----------------------------------------------------------------------------
                '                          COMPENSO
                '-----------------------------------------------------------------------------
                '-----------------------------------------------------------------------------
                If lav_cod = LAVCOD_REG_COMPENSI Then

                    ' è la stessa operazione, quindi non ripeto la visualizzazione
                    If RagSoc_Memo = .Item("Rag_Soc_Contatto") And NumProtocollo_Memo = .Item("Num_Protocollo") Then

                        Scadenza = ""
                        Data_Movimento = ""
                        Riferimento = ""
                        NumeroDoc = ""
                        Str_Importo_Tot = ""
                        Str_Iva_Tot = ""
                        Str_Imponibile_Tot = ""

                        'TODO: ARROTONDAMENTO??? - in realtà sono arrotondati direttamente sulla query, quindi non sarebbe necessario riarrotondare
                        'imponibile è 0, per cui me lo ricavo
                        .Item("Imponibile_Netto_Doc") = Format(CDec(.Item("Num_Protocollo")) - CDec(.Item("Iva_doc")), "##,###,##0.00")
                        'no perché Imponibile_Doc contiene la somma di tutti gli imponibili della scheda corrispettivi
                        '.Item("Imponibile_Netto_Doc") = Format(.Item("Imponibile_Doc"), "##,###,##0.00")

                    Else

                        RagSoc_Memo = .Item("Rag_Soc_Contatto")
                        NumProtocollo_Memo = .Item("Num_Protocollo")

                        Data_Movimento = CDate(.Item("Data_Movimento")).ToShortDateString

                        'Nei corrispettivi la scadenza è salvata in Scadenza_Extra
                        Select Case .Item("Scadenza_Extra")
                            Case AGRODATAINIZIO, AGRODATAFINE
                                Scadenza = "n.d."
                            Case Else
                                Scadenza = CDate(.Item("Scadenza_Extra")).ToShortDateString
                        End Select

                        'riciclo il campo Extra_Str per memorizzare la descrizione
                        Riferimento = .Item("Des_Lib") & " ( " & .Item("Rag_Soc_Contatto") & " - " & .Item("Cod_Contatto") & " ) "

                        NumeroDoc = CStr(.Item("Doc_Numero"))

                        TOT_Importo += ArrotondaVal_2(CDec(.Item("Num_Protocollo")))
                        Str_Importo_Tot = ArrotondaVal_2(CDec(.Item("Num_Protocollo")))

                        TOT_Iva += ArrotondaVal_2(CDec(.Item("Iva_Doc")))
                        Str_Iva_Tot = ArrotondaVal_2(CDec(.Item("Iva_Doc")))

                        'TODO: ARROTONDAMENTO??? - in realtà sono arrotondati direttamente sulla query, quindi non sarebbe necessario riarrotondare
                        'imponibile è 0, per cui me lo ricavo
                        .Item("Imponibile_Netto_Doc") = Format(CDec(.Item("Num_Protocollo")) - CDec(.Item("Iva_doc")), "##,###,##0.00")
                        'no perché Imponibile_Doc contiene la somma di tutti gli imponibili della scheda corrispettivi
                        '.Item("Imponibile_Netto_Doc") = Format(.Item("Imponibile_Doc"), "##,###,##0.00")

                        TOT_Imponibile += CDec(.Item("Imponibile_Netto_Doc"))
                        Str_Imponibile_Tot = ArrotondaVal_2(CDec(.Item("Imponibile_Netto_Doc")))

                    End If
                    '-----------------------------------------------------------------------------
                    '-----------------------------------------------------------------------------
                    '                       FINE   COMPENSO
                    '-----------------------------------------------------------------------------
                    '-----------------------------------------------------------------------------

                Else
                    '-----------------------------------------------------------------------------
                    '-----------------------------------------------------------------------------
                    '            TUTTI I RESTANTI MOVIMENTI CHE NON SONO COMPENSI
                    '-----------------------------------------------------------------------------
                    '-----------------------------------------------------------------------------

                    ' è la stessa operazione, quindi non ripeto la visualizzazzione
                    If IdAgenda_Memo = .Item("id_agenda") Then

                        Scadenza = ""
                        Data_Movimento = ""
                        Riferimento = ""
                        NumeroDoc = ""
                        Str_Importo_Tot = ""
                        Str_Iva_Tot = ""
                        Str_Imponibile_Tot = ""

                    Else

                        IdAgenda_Memo = .Item("id_agenda")
                        Data_Movimento = CDate(.Item("Data_Movimento")).ToShortDateString

                        Select Case .Item("Scadenza")
                            Case AGRODATAINIZIO, AGRODATAFINE
                                Scadenza = "n.d."
                            Case Else
                                Scadenza = CDate(.Item("Scadenza")).ToShortDateString
                        End Select

                        Select Case .Item("Lav_Cod")
                            Case LAVCOD_ALTRI_COSTI, LAVCOD_ALTRI_RICAVI
                                .Item("Imponibile_Netto_Doc") = Format(.Item("Imponibile_Doc"), "##,###,##0.00")
                        End Select

                        impNetto = objHlp.Leggi_Imponibile_PositivoNegativo2(.Item("Lav_Cod"), CDec(.Item("Imponibile_Netto_Doc")))
                        Str_Imponibile_Tot = Format(impNetto, "##,###,##0.00")

                        iva = objHlp.Leggi_IVA_PositivaNegativa2(.Item("Lav_Cod"), CDec(.Item("Iva_Doc")))
                        Str_Iva_Tot = Format(iva, "##,###,##0.00")

                        Select Case .Item("Lav_Cod")

                            Case LAVCOD_FATTURA_RICEVUTA, LAVCOD_FATTURA_PROFESSIONISTI, LAVCOD_FATTURA_EMESSA,
                                LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_EMESSA,
                                LAVCOD_RICEVUTA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO,
                                LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA

                                NumeroDoc = .Item("Doc_Numero_Sin") & CStr(.Item("Doc_Numero")) & .Item("Doc_Numero_Des")

                                Select Case .Item("Lav_Cod")

                                    Case LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA
                                        Riferimento = .Item("Rag_Soc_Contatto") '& " (Nota di accredito)"

                                    Case Else
                                        If .Item("Mov_Desc") <> "" Then
                                            Riferimento = .Item("Rag_Soc_Contatto") & " (" & .Item("Mov_Desc") & ") "
                                        Else
                                            Riferimento = .Item("Rag_Soc_Contatto")
                                        End If

                                End Select

                                importo = objHlp.Leggi_Importo_PositivoNegativo(.Item("Lav_Cod"), .Item("Num_Protocollo"))

                                .Item("Num_Protocollo") = Format(importo, "##,###,##0.00")

                            Case LAVCOD_ACQUISTO, LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO,
                                LAVCOD_ALTRI_COSTI, LAVCOD_ALTRI_RICAVI

                                'il numero documento non esiste per questi movimenti
                                NumeroDoc = ""

                                importo = iva + impNetto

                                .Item("Num_Protocollo") = Format(importo, "##,###,##0.00")

                                Riferimento = .Item("Des_Lib")

                            Case Else
                                'x debug, non ci deve andare
                                NumeroDoc = ""
                                Riferimento = .Item("Des_Lib")

                        End Select

                        Str_Importo_Tot = Format(importo, "##,###,##0.00")

                        TOT_Importo += importo

                        TOT_Iva += iva

                        TOT_Imponibile += impNetto

                    End If

                    '-----------------------------------------------------------------------------
                    '-----------------------------------------------------------------------------
                    '      FINE      TUTTI I RESTANTI MOVIMENTI CHE NON SONO COMPENSI
                    '-----------------------------------------------------------------------------
                    '-----------------------------------------------------------------------------
                End If

                TOT_Importo_DaIncassarePagare += Importo_da_Pagare
                TOT_Importo_RiscossoPagato += Importo_gia_Pagato

                'Dim Dettagli_Pagamento As String = ""
                'If Modalita_Pag_Prevista <> "" Then
                '    Dettagli_Pagamento &= "Pagamento previsto: " & Modalita_Pag_Prevista
                'End If
                'If Modalita_Pag_Effettuata <> "" Then
                '    If Dettagli_Pagamento <> "" Then
                '        Dettagli_Pagamento &= " | "
                '    End If
                '    Dettagli_Pagamento &= "Riscossi: " & Modalita_Pag_Effettuata
                'End If

                ' .Item("dettagli_pagamento") = Modalita_Pag_Prevista & " " & Modalita_Pag_Effettuata
                '.Item("ImportoDaPagare") = Tot_Importo_da_Pagare

                Try

                    dr = dsPag.DS_Pagamenti.NewDS_PagamentiRow

                    dr.Piva = _piva
                    _ragSoc = .Item("Impresa")
                    dr.Impresa = _ragSoc
                    dr.Sezionale = ""
                    dr.Data_Inizio = _dataInizio
                    dr.Data_Fine = _dataFine

                    dr.Scadenza = Scadenza 'CDate(.Item("Scadenza")).ToShortDateString
                    dr.Riferimento = Riferimento
                    dr.Data = Data_Movimento
                    dr.Numero = str_Doc & NumeroDoc
                    dr.Imponibile_Doc = Str_Imponibile_Tot
                    dr.Iva_Doc = Str_Iva_Tot
                    dr.Importo_Doc = Str_Importo_Tot
                    dr.Modalita_Pag_Prevista = Modalita_Pag_Prevista
                    dr.Pag_Avvenuto = Modalita_Pag_Effettuata
                    dr.Importo_DaIncassarePagare = Format(Importo_da_Pagare, "##,###,##0.00")
                    dr.Importo_RiscossoPagato = Format(Importo_gia_Pagato, "##,###,##0.00")

                    dsPag.DS_Pagamenti.Rows.Add(dr)

                Catch ex As Exception
                    _logErrori &= "- Inserimento record nel dataset: " & vbCrLf & ex.Message & vbCrLf
                End Try

                '#########################################################
                dsPag.DS_Pagamenti.AcceptChanges()
                '#########################################################

            End With

        Catch ex As Exception
            _logErrori &= "- Elabora_Record: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class
