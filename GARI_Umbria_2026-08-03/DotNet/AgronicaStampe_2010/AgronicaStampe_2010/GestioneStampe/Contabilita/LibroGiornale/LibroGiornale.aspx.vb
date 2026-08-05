Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class LibroGiornale
    Inherits System.Web.UI.Page

    Private _rptStampa As Rpt_LibroGiornale

    Private _gestContFlagConsideraSaldiIniziali As Boolean
    Private _gestContDataInizio As Date
    Private _piva As String
    Private _dataInizio As String
    Private _dataFine As String
    Private _anno, _annoMin As Integer
    ' Dim Filtro_Conti As String
    Private _logErrori As String
    ' Dim Chk_CE, Chk_SP As Boolean
    ' Dim Conti_UE_Tutti, Conti_0Movimentati_1Tutti As Integer
    'Dim Ric_Cod_Eco As Integer = 0
    'Dim Ric_Cod_Pat As Integer = 0
    'Dim Cod_Conto_Eco As Integer = 0
    'Dim Cod_Conto_Pat As Integer = 0
    'Dim Sezionale_Cod As Integer
    'Dim Sezionale_Des As String
    Private _codRisUm, _codLiquidita As Integer
    Private _numPagina, _numRiga As Integer
    Private _qsStampaDefinitiva As enum_StampaDefinitivaDiProva

    Private _objParametriServer As New AgronicaCoreParametri
    Private _nuoviArrotondamenti As Boolean = False

    '####################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim report As Integer

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))


        _anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        _dataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        _dataFine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _gestContFlagConsideraSaldiIniziali = Stringa_Decodifica(CStr(Request.QueryString("gcfcsi")), AgroKey_EncoderDecoder, Server)

        _gestContDataInizio = Stringa_Decodifica(CStr(Request.QueryString("gcdi")), AgroKey_EncoderDecoder, Server)

        'Filtro_Conti = Stringa_Decodifica(CStr(Request.QueryString("fc")), AgroKey_EncoderDecoder, Server)


        'Chk_CE = Stringa_Decodifica(CStr(Request.QueryString("chk_ce")), AgroKey_EncoderDecoder, Server)

        'Chk_SP = Stringa_Decodifica(CStr(Request.QueryString("chk_sp")), AgroKey_EncoderDecoder, Server)

        'Conti_UE_Tutti = Stringa_Decodifica(CStr(Request.QueryString("ue")), AgroKey_EncoderDecoder, Server)

        'Conti_0Movimentati_1Tutti = Stringa_Decodifica(CStr(Request.QueryString("mov")), AgroKey_EncoderDecoder, Server)

        'Ric_Cod_Eco = Stringa_Decodifica(CStr(Request.QueryString("rce")), AgroKey_EncoderDecoder, Server)

        'Cod_Conto_Eco = Stringa_Decodifica(CStr(Request.QueryString("cce")), AgroKey_EncoderDecoder, Server)

        'Ric_Cod_Pat = Stringa_Decodifica(CStr(Request.QueryString("rcp")), AgroKey_EncoderDecoder, Server)

        'Cod_Conto_Pat = Stringa_Decodifica(CStr(Request.QueryString("ccp")), AgroKey_EncoderDecoder, Server)

        'Sezionale_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("szc")), AgroKey_EncoderDecoder, Server))

        'Sezionale_Des = Stringa_Decodifica(CStr(Request.QueryString("szd")), AgroKey_EncoderDecoder, Server)

        'Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server))

        'Cod_Liquidita = CInt(Stringa_Decodifica(CStr(Request.QueryString("liq")), AgroKey_EncoderDecoder, Server))

        _numPagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("np")), AgroKey_EncoderDecoder, Server))

        _numRiga = CInt(Stringa_Decodifica(CStr(Request.QueryString("nr")), AgroKey_EncoderDecoder, Server))

        ' se è una stampa definitiva salvo il pdf e visualizzo l'anteprima, altrimenti visualizzo solo l'anteprima
        If Not IsNothing(Request.QueryString("stDef")) AndAlso Stringa_Decodifica(Request.QueryString("stDef").ToString,
                         AgroKey_EncoderDecoder, Server) <> "" Then
            _qsStampaDefinitiva = Stringa_Decodifica(Request.QueryString("stDef").ToString, AgroKey_EncoderDecoder, Server)
        Else
            _qsStampaDefinitiva = enum_StampaDefinitivaDiProva.DiProva       ' stampa di prova
        End If

        _objParametriServer = New AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        _rptStampa = New Rpt_LibroGiornale

        Dim nomeDocumento, identificazioneDocumento As String
        nomeDocumento = "LibroGiornale"

        identificazioneDocumento = "AnnoCont" & CStr(_anno) '& "_" & "Sez-" & Left(Sezionale_Des, 4)

        'If Filtro_Conti <> "" Then
        '    identificazioneDocumento &= "_filtro"
        'End If
        'If Chk_CE = 1 And Chk_SP = 0 Then
        '    identificazioneDocumento &= "_CE"
        'End If
        'If Chk_CE = 0 And Chk_SP = 1 Then
        '    identificazioneDocumento &= "_SP"
        'End If

        If Not Me.IsPostBack Then


            Dim dsLibroGiornale As New DS_LibroGiornale

            Try

                _logErrori = ""

                Stampa_LibroGiornale(dsLibroGiornale)

            Catch exc As Exception
                _logErrori &= "- Stampa: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Try

                'rptBilancio.OpenSubreport("Rpt_BilancioSezioneA.rpt").SetDataSource(DSBilancio)

            Catch exc As Exception
                _logErrori &= "- OpenSubreport: " & vbCrLf & exc.Message & vbCrLf
            End Try


            Dim nomeFilePdf As String = ""

            Try

                Dim dataInizioAllegati, dataFineAllegati As Date

                If _dataInizio <> AGRODATAINIZIO AndAlso _dataFine <> AGRODATAFINE Then
                    dataInizioAllegati = _dataInizio
                    dataFineAllegati = _dataFine
                    identificazioneDocumento &= "_" & Format(CDate(_dataInizio), "yyyy_MM_dd") & "_" & Format(CDate(_dataFine), "yyyy_MM_dd")
                Else
                    dataInizioAllegati = "01/01/" & CStr(_anno)
                    dataFineAllegati = "31/12/" & CStr(_anno)
                End If
                nomeFilePdf = nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf"


                If _qsStampaDefinitiva = enum_StampaDefinitivaDiProva.Definitiva Then
                    
                    ' leggo la sottocartella da CategorieDocumenti
                    Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                    Dim sottoCartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.LibroGiornale, "", "", _objParametriServer)

                    ' salvo il report in formato PDF
                    Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                    objGestFile.SalvaReportPdf(_rptStampa,
                                               enum_CategorieDocumenti.LibroGiornale,
                                               sottoCartella, nomeFilePdf,
                                               _objParametriServer,
                                               New AgronicaCoreGestioneRichieste.AgroWebConfig)

                    Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                    Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                    enum_CategorieDocumenti.LibroGiornale,
                                                                                    nomeDocumento,
                                                                                    nomeFilePdf, sottoCartella,
                                                                                    "", "", "", "",
                                                                                    dataInizioAllegati,
                                                                                    dataFineAllegati,
                                                                                    _objParametriServer)

                End If

            Catch ex As Exception
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Eliminato passaggio report in session per giro su file: Session("Report") = rptStampa
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptStampa.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Dispose del report per evitare problema deallocazione.

            dsLibroGiornale.Dispose()
            dsLibroGiornale = Nothing

            _rptStampa.Close()
            _rptStampa.Dispose()
            _rptStampa = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = "anno = " & CStr(_anno) & ", Data_Inizio = " & CStr(_dataInizio) & ", Data_Fine = " & CStr(_dataFine)
            SalvaLogErrori_Generico(_logErrori, nomeDocumento, identificazioneDocumento, "LibroGiornale.aspx", "Stampe_Contabilita", infoInLog, _objParametriServer)
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

    '#####################################################################
    Private Sub Stampa_LibroGiornale(ByRef dsLibroGiornale As DS_LibroGiornale)

        If _dataInizio < _gestContDataInizio AndAlso _dataFine >= _gestContDataInizio Then
            'se la data inizio è precedente alla data di inizio gestione contabile e la data di fine è >=
            '-> allora la data di inizio diventa la data di inizio gestione contabile
            _dataInizio = _gestContDataInizio
        End If

        _annoMin = CDate(_dataInizio).Year

        Dim dataSaldo As Date
        'calcolo il saldo al giorno primo (saldo iniziale al quale sommare i movimenti del periodo)
        dataSaldo = DateAdd(DateInterval.Day, -1, CDate(_dataInizio))


        Try

            Dim dtIntestazione As DataTable

            CType(_rptStampa.Section1.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "" 'str_filtro

            Dim strInttemp As String = ""
            If _dataInizio <> AGRODATAINIZIO AndAlso _dataFine <> AGRODATAFINE Then
                strInttemp = "Periodo dal " & _dataInizio & " al " & _dataFine
            End If
            'If Sezionale_Des <> "" Then
            '    strInttemp &= " - Sezionale: " & Sezionale_Des
            'End If

            'CType(rptStampa.Section2.ReportObjects("TxtAnnoContabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CStr(Anno)
            CType(_rptStampa.Section2.ReportObjects("TxtIntervalloTemporale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strInttemp

            Dim objIntest As New AgronicaCoreStampeDAL.DocContab
            dtIntestazione = objIntest.DatiIntestazioneImpresa(_piva, Nothing, "", "", _objParametriServer)

            If Not IsNothing(dtIntestazione) AndAlso dtIntestazione.Rows.Count > 0 Then
                CType(_rptStampa.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("Piva")
                CType(_rptStampa.Section2.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("Codice_Fiscale")
                CType(_rptStampa.Section2.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("rag_soc")
                CType(_rptStampa.Section2.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("ind_impresa") & " " &
                                                                                                                                   dtIntestazione.Rows(0).Item("CAP") & " " &
                                                                                                                                   dtIntestazione.Rows(0).Item("frz_des") & " - " &
                                                                                                                                   dtIntestazione.Rows(0).Item("LOCALITA") &
                                                                                                                                   " (" & dtIntestazione.Rows(0).Item("COMUNI_PROV") & ") "
            End If

        Catch ex As Exception
            _logErrori &= "- Intestazione report: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '==================================
        '====== QUERY E CARICAMENTO DATASET ===========
        '==================================

        CaricaDataset_LibroGiornale(dsLibroGiornale, dataSaldo)


        Try

            'imposto il dataset sul report
            _rptStampa.SetDataSource(dsLibroGiornale)

        Catch exc As Exception
            _logErrori &= "- Aggancio dataset: " & vbCrLf & exc.Message & vbCrLf
        End Try

    End Sub

    '###################################################
    Private Sub CaricaDataset_LibroGiornale(ByRef dsLibroGiornale As DS_LibroGiornale, ByVal dataSaldo As Date)

        Dim objConti As New AgronicaCoreStampeDAL.PianoConti
        Dim objRicXContiPat As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
        Dim objRicXContiEco As New AgronicaCoreContabDAL.RicxConti_R

        Dim i As Integer
        Dim dt As DataTable
        ' Dim DTSaldi As DataTable
        Dim dtCodifichePat As DataTable
        Dim dtCodificheEco As DataTable

        Try

            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            dtCodifichePat = objRicXContiPat.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

            Try
                'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
                dtCodificheEco = objRicXContiEco.Leggi_Codifica_ContiEconomici(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)
            Catch ex As Exception
                _logErrori &= "- Leggi_Codifica_ContiEconomici: " & vbCrLf & ex.Message & vbCrLf
            End Try

            dt = objConti.Libro_Giornale_Contabile(_dataInizio, _dataFine,
                                                   _piva, _anno, BILANCIO_PERSONALIZZATO,
                                                   "", "",
                                                   CE_CONTO_IMPUTABILE_NOFILTRO,
                                                   SP_CONTO_IMPUTABILE_NOFILTRO,
                                                   0,
                                                   CONTO_UE_NOFILTRO, "",
                                                   SEZIONALE_NOFILTRO,
                                                   _codRisUm, -1,
                                                   "", "", "", "", "",
                                                   dtCodifichePat,
                                                   dtCodificheEco,
                                                   _objParametriServer,
                                                   flagNuoviArrotondamenti:=_nuoviArrotondamenti)

            ''calcolo il saldo al giorno primo (saldo iniziale al quale sommare i movimenti del periodo)
            'dataSaldo = DateAdd(DateInterval.Day, -1, CDate(_dataInizio))

            'DTSaldi = objConti.Saldo_Conti_Economici(dataSaldo,
            '                                        _piva, _anno, Ric_Cod_Eco,
            '                                        "", "", 0,
            '                                        Cod_Conto_Eco,
            '                                        Conti_UE_Tutti, "",
            '                                        Sezionale_Cod,
            '                                        Cod_RisUm,
            '                                        "", "", "", "", "",
            '                                        _objParametriServer)



        Catch exc As Exception
            dt = Nothing
            ' DTSaldi = Nothing
            _logErrori &= "- Query LibroGiornale: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Dim dr As DS_LibroGiornale.DTLibroGiornaleRow
        Dim codConto As Integer
        Dim importoDare As Decimal = 0
        Dim importoAvere As Decimal = 0
        Dim htIdAgendaDaBloccare As Hashtable
        Dim codiceSplitGruppo As String
        Dim numRegistrazione, idAgenda As Integer
        Dim desLib As String = ""
        Dim idAgendaTemp As Integer = 0
        Dim desLibTemp As String = ""
        '    Dim SaldoConto, ImportoRiga As Decimal

        Try

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then

                htIdAgendaDaBloccare = New Hashtable

                'visto che aggiungo +1 anche al primo giro, parto togliendo -1
                numRegistrazione = _numRiga - 1

                For i = 0 To dt.Rows.Count - 1

                    '  Try

                    desLib = dt.Rows(i).Item("Des_Lib")

                    If _gestContFlagConsideraSaldiIniziali = False AndAlso desLib = "Saldo Iniziale" Then
                        '  Giulia, 01/12/2016 14.41.02: se ho scelto di non mostrare i saldi di inizio gestione,
                        '               non devo mostrare le voci dei saldi iniziali e passare alla riga successiva
                        Continue For
                    End If

                    dr = dsLibroGiornale.DTLibroGiornale.NewRow

                    idAgenda = dt.Rows(i).Item("Id_Agenda")
                    dr.Id_Agenda = idAgenda

                    If Not htIdAgendaDaBloccare.ContainsKey(idAgenda) Then
                        htIdAgendaDaBloccare.Add(idAgenda, "")
                    End If

                    'questo campo è usato dal mastrino contabile, ma lo uso per recuperare le prime due cifre (mi dice se è CE o SP)
                    codiceSplitGruppo = dt.Rows(i).Item("CodiceSplitGruppo")
                    dr.CodiceSplitGruppo = codiceSplitGruppo

                    codConto = dt.Rows(i).Item("Cod_Conto_Pat")

                    If idAgenda <> idAgendaTemp OrElse desLib <> desLibTemp Then
                        'primo giro o è cambiata l'operazione
                        idAgendaTemp = idAgenda
                        desLibTemp = desLib

                        numRegistrazione += 1
                        dr.Num_Registrazione = numRegistrazione
                        dr.Num_OperazioneXSplit = numRegistrazione

                        'If DT.Rows(i).Item("Data_Registrazione") = AGRODATAINIZIO Then
                        '    DR.Data_Registrazione = ""
                        'Else
                        '    DR.Data_Registrazione = DT.Rows(i).Item("Data_Registrazione")
                        'End If
                        dr.Data_Registrazione = CDate(dt.Rows(i).Item("Data_Registrazione")).ToShortDateString

                        dr.Des_Lib = desLib
                        dr.Causale = desLib

                        'If DT.Rows(i).Item("Data_Movimento") = AGRODATAINIZIO Then
                        '    DR.Data_Movimento = ""
                        'Else
                        '    DR.Data_Movimento = CDate(DT.Rows(i).Item("Data_Movimento")).ToShortDateString
                        'End If
                        dr.Data_Movimento = CDate(dt.Rows(i).Item("Data_Movimento")).ToShortDateString

                        dr.Numero_Doc = dt.Rows(i).Item("Numero_Doc")

                        If dr.Numero_Doc <> "" Then
                            dr.Rif_Doc = dr.Numero_Doc
                        Else
                            dr.Rif_Doc = ""
                        End If

                        If dr.Data_Movimento <> "" Then
                            dr.Rif_Doc &= " del " & dr.Data_Movimento
                        End If

                    Else
                        'ricorrenze successive della stessa operazione
                        'non ripeto i dati fissi
                        dr.Data_Registrazione = ""
                        dr.Num_Registrazione = ""
                        dr.Des_Lib = ""
                        dr.Causale = ""
                        dr.Rif_Doc = ""
                        'viene usato per splittare il gruppo, non devo azzerarlo
                        dr.Num_OperazioneXSplit = numRegistrazione
                    End If

                    dr.Num_Pagina = _numPagina

                    If dt.Rows(i).Item("Progr_Protocollo") = 0 Then
                        dr.Progr_Protocollo = ""
                    Else
                        dr.Progr_Protocollo = dt.Rows(i).Item("Progr_Protocollo")
                    End If

                    dr.Cod_Conto = codConto
                    dr.Id_Riclassificazione = dt.Rows(i).Item("Id_Riclassificazione")
                    dr.Conto_Descr = Left(dr.CodiceSplitGruppo, 2) & " " & dr.Id_Riclassificazione & " " & dt.Rows(i).Item("Conto_Pat_Descr")
                    'DR.Articolo = ""

                    dr.Lav_Cod = dt.Rows(i).Item("Lav_Cod")

                    importoDare = CDec(dt.Rows(i).Item("Dare"))
                    importoAvere = CDec(dt.Rows(i).Item("Avere"))

                    dr.Dare = importoDare
                    dr.Avere = importoAvere

                    dsLibroGiornale.DTLibroGiornale.Rows.Add(dr)

                Next

            End If

        Catch ex As Exception
            _logErrori &= "- CaricaDataset_LibroGiornale: " & vbCrLf & ex.Message & vbCrLf
        End Try

        'Try
        '    'devo usare dtsaldi!
        '    DT = Nothing

        '    Dim flag_insert As Boolean
        '    ' stampa anche dei conti non movimentati ma con saldo
        '    If Conti_0Movimentati_1Tutti = 1 Then

        '        If Not IsNothing(DTSaldi) AndAlso DTSaldi.Rows.Count > 0 Then

        '            For i = 0 To DTSaldi.Rows.Count - 1

        '                flag_insert = False

        '                '   Cod_Conto = DTSaldi.Rows(i).Item("Cod_Conto_Eco")
        '                CodiceSplitGruppo = DT.Rows(i).Item("CodiceSplitGruppo")

        '                '        If Not IsNothing(HT_CodContoEco) Then
        '                'If Not HT_CodContoEco.ContainsKey(Cod_Conto) Then

        '                If Not IsNothing(HT_ContiMovimentati) Then
        '                    If Not HT_ContiMovimentati.ContainsKey(CodiceSplitGruppo) Then
        '                        'conto non movimentato
        '                        flag_insert = True
        '                    Else
        '                        'il conto è presente
        '                        'è già stato inserito nel dataset
        '                        flag_insert = False
        '                    End If
        '                Else
        '                    ' HT vuoto, nell'intervallo non ci sono conti movimentati
        '                    '-> vanno inseriti tutti
        '                    flag_insert = True
        '                End If

        '                If flag_insert = True Then

        '                    DR = DSMastrino.DS_Mastrino.NewRow

        '                    DR.CodiceSplitGruppo = DT.Rows(i).Item("CodiceSplitGruppo")
        '                    DR.Cod_Conto = Cod_Conto
        '                    DR.Conto_Descr = DTSaldi.Rows(i).Item("Conto_Eco_Descr")
        '                    DR.Id_Riclassificazione = DTSaldi.Rows(i).Item("Id_Riclassificazione")
        '                    DR.Articolo = ""
        '                    DR.Id_Agenda = 0
        '                    DR.Lav_Cod = 0
        '                    DR.Des_Lib = "Saldo al " & CStr(Data_Saldo)
        '                    DR.Data_Movimento = ""
        '                    DR.Numero_Doc = ""
        '                    DR.Data_Registrazione = ""
        '                    DR.Progr_Protocollo = ""
        '                    DR.Dare = DTSaldi.Rows(i).Item("Saldo_Dare")
        '                    DR.Avere = DTSaldi.Rows(i).Item("Saldo_Avere")
        '                    DR.Saldo_Riga = DTSaldi.Rows(i).Item("Saldo")

        '                    DSMastrino.DS_Mastrino.Rows.Add(DR)

        '                End If

        '            Next
        '        End If
        '    End If

        'Catch ex As Exception
        '    Log_Errori &= "- Lettura conti ECONOMICI non movimentati ma con saldo: " & vbCrLf & ex.Message & vbCrLf
        'End Try

    End Sub

End Class