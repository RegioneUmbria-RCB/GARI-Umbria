Imports AgronicaCoreContabHLP.Contabilita
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class Mastrino
    Inherits System.Web.UI.Page

    Private _rptStampa As Rpt_Mastrino

    Private _gestContFlagConsideraSaldiIniziali As Boolean
    Private _gestContDataInizio As Date
    Private _piva As String
    Private _dataInizio As String
    Private _dataFine As String
    Private _anno As Integer
    Private _annoMin As Integer
    'Private Filtro_Conti As String
    Private _logErrori As String
    Private Chk_CE, Chk_SP As Boolean
    'Private Conti_UE_Tutti As Integer
    Private _ricCodEco As Integer = 0
    Private _ricCodPat As Integer = 0
    Private _codContoEco As Integer = 0
    Private _codContoPat As Integer = 0
    'Conti_0Movimentati_1Tutti
    'Conti_0ImponibileNoZero_1Tutti
    Private _sezionaleCod As Integer
    Private _sezionaleDes As String
    Private _sezionaleChkDefault As Integer
    Private _codRisUm, _codLiquidita As Integer
    Private _flagEscludiIvaIndetraibile As Boolean

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _nuoviArrotondamenti As Boolean = False

    'Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

    '    rptRegistriIVA = New Rpt_RegistriIVA_2
    '    rptSottoReportIva = New Rpt_SottoReport_IVA

    'End Sub

    '####################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim Report As Integer
        Dim parEscludiIvaIndetr As String = ""

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))


        _anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        _dataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        _dataFine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        Chk_CE = Stringa_Decodifica(CStr(Request.QueryString("chk_ce")), AgroKey_EncoderDecoder, Server)

        Chk_SP = Stringa_Decodifica(CStr(Request.QueryString("chk_sp")), AgroKey_EncoderDecoder, Server)

        _ricCodEco = Stringa_Decodifica(CStr(Request.QueryString("rce")), AgroKey_EncoderDecoder, Server)

        _codContoEco = Stringa_Decodifica(CStr(Request.QueryString("cce")), AgroKey_EncoderDecoder, Server)

        _ricCodPat = Stringa_Decodifica(CStr(Request.QueryString("rcp")), AgroKey_EncoderDecoder, Server)

        _codContoPat = Stringa_Decodifica(CStr(Request.QueryString("ccp")), AgroKey_EncoderDecoder, Server)

        _codRisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server))

        _codLiquidita = CInt(Stringa_Decodifica(CStr(Request.QueryString("liq")), AgroKey_EncoderDecoder, Server))

        _gestContFlagConsideraSaldiIniziali = Stringa_Decodifica(CStr(Request.QueryString("gcfcsi")), AgroKey_EncoderDecoder, Server)

        _gestContDataInizio = Stringa_Decodifica(CStr(Request.QueryString("gcdi")), AgroKey_EncoderDecoder, Server)

        '  Giulia, 03/08/2017 15:25:07: quando dal LAN si richiama la stampa di uno specifico mastrino, 
        '               questo parametro non è fondamentale e non viene passato
        parEscludiIvaIndetr = Stringa_Decodifica(CStr(Request.QueryString("chkii")), AgroKey_EncoderDecoder, Server)

        _flagEscludiIvaIndetraibile = IIf(parEscludiIvaIndetr <> "", parEscludiIvaIndetr, False)

        'Filtro_Conti = Stringa_Decodifica(CStr(Request.QueryString("fc")), AgroKey_EncoderDecoder, Server)

        'Conti_UE_Tutti = Stringa_Decodifica(CStr(Request.QueryString("ue")), AgroKey_EncoderDecoder, Server)

        'Conti_0Movimentati_1Tutti = Stringa_Decodifica(CStr(Request.QueryString("mov")), AgroKey_EncoderDecoder, Server)

        'Conti_0ImponibileNoZero_1Tutti = Stringa_Decodifica(CStr(Request.QueryString("saldo")), AgroKey_EncoderDecoder, Server)


        _sezionaleCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("szc")), AgroKey_EncoderDecoder, Server))

        _sezionaleDes = Stringa_Decodifica(CStr(Request.QueryString("szd")), AgroKey_EncoderDecoder, Server)

        _sezionaleChkDefault = Stringa_Decodifica(CStr(Request.QueryString("szdf")), AgroKey_EncoderDecoder, Server)

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        _rptStampa = New Rpt_Mastrino

        Dim identificazioneDocumento As String = CStr(_anno)
        Dim nomeDocumento As String = "Mastrino"

        'If Filtro_Conti <> "" Then
        '    identificazioneDocumento &= "_filtro"
        'End If
        If Chk_CE = 1 AndAlso Chk_SP = 0 Then
            identificazioneDocumento &= "_CE"
        End If
        If Chk_CE = 0 AndAlso Chk_SP = 1 Then
            identificazioneDocumento &= "_SP"
        End If

        If Not Me.IsPostBack Then


            Dim dsMastrino As New _DS_Mastrino

            Try

                _logErrori = ""

                Stampa_Mastrino(dsMastrino)

            Catch exc As Exception
                _logErrori &= "- Stampa: " & vbCrLf & exc.Message & vbCrLf
            End Try

            Try

                'rptBilancio.OpenSubreport("Rpt_BilancioSezioneA.rpt").SetDataSource(DSBilancio)
                'rptBilancio.OpenSubreport("Rpt_BilancioSezioneB.rpt").SetDataSource(DSBilancio)
                'rptBilancio.OpenSubreport("Rpt_BilancioSezioneC.rpt").SetDataSource(DSBilancio)
                'rptBilancio.OpenSubreport("Rpt_BilancioSezioneD.rpt").SetDataSource(DSBilancio)
                'rptBilancio.OpenSubreport("Rpt_BilancioSezioneE.rpt").SetDataSource(DSBilancio)

                'rptBilancio.OpenSubreport("Rpt_MovimentiMastro.rpt").SetDataSource(DSBilancio)

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

                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.Bilancio, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptStampa,
                                           enum_CategorieDocumenti.Bilancio,
                                           sottoCartella, nomeFilePdf,
                                           _objParametriServer,
                                           New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim allegatiDocumentiCod As Integer = objAllegati.SalvaAllegato(_piva,
                                                                                enum_CategorieDocumenti.Bilancio,
                                                                                nomeDocumento,
                                                                                nomeFilePdf, sottoCartella,
                                                                                "", "", "", "",
                                                                                dataInizioAllegati,
                                                                                dataFineAllegati,
                                                                                _objParametriServer)



            Catch ex As Exception
                _logErrori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Rimosso passaggio report in session per problema deallocazione: Session("Report") = rptStampa

            'MS Creazione report su disco per passaggio a visualizzatore
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptStampa.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = "anno = " & CStr(_anno) & ", Data_Inizio = " & CStr(_dataInizio) & ", Data_Fine = " & CStr(_dataFine)
            SalvaLogErrori_Generico(_logErrori, nomeDocumento, identificazioneDocumento, "Mastrino.aspx", "Stampe_Contabilita", infoInLog, _objParametriServer)

            'MS Dispose del report
            dsMastrino.Dispose()
            dsMastrino = Nothing

            _rptStampa.Close()
            _rptStampa.Dispose()
            _rptStampa = Nothing

            GC.Collect()

            Dim PDF As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder, Server)

            If PDF = "1" Then
                Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                  "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
            Else
                Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
            End If
 
        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_Mastrino(ByRef DSMastrino As _DS_Mastrino)

        ' Dim Data_Inizio_Saldo As Date
        'Dim Data_Fine_Saldo As Date
        Dim Parametro_Filtro As String = ""
        Dim Parametro_IntervalloTemp As String = ""
        Dim Parametro_Piva As String = ""
        Dim Parametro_CodiceFiscale As String = ""
        Dim Parametro_Rag_Soc As String = ""
        Dim Parametro_Indirizzo As String = ""
        Dim Parametro_Titolo As String = "Mastrino"

        Try

            If _flagEscludiIvaIndetraibile = True Then
                Parametro_Titolo = "Mastrino senza conteggio IVA indetraibile"
            End If

            If _dataInizio < _gestContDataInizio AndAlso _dataFine >= _gestContDataInizio Then
                'se la data inizio è precedente alla data di inizio gestione contabile e la data di fine è >=
                '-> allora la data di inizio diventa la data di inizio gestione contabile
                _dataInizio = _gestContDataInizio
            End If

            ''calcolo il saldo al giorno primo (saldo iniziale al quale sommare i movimenti del periodo)
            'Data_Fine_Saldo = DateAdd(DateInterval.Day, -1, CDate(Data_Inizio))

            _annoMin = CDate(_dataInizio).Year

            Dim str_inttemp As String = ""
            If _dataInizio <> AGRODATAINIZIO AndAlso _dataFine <> AGRODATAFINE Then
                str_inttemp = "Periodo dal " & _dataInizio & " al " & _dataFine
            End If

            Parametro_IntervalloTemp = str_inttemp

            Parametro_Filtro = "" 'str_filtro
            If _sezionaleDes <> "" Then
                Parametro_Filtro &= "Sezionale: " & _sezionaleDes
            End If

            'anno contabile disattivato
            'CType(rptStampa.Section1.ReportObjects("TxtAnnoContabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CStr(Anno)


            Dim objIntest As New AgronicaCoreStampeDAL.DocContab
            Dim dtIntestazione As DataTable
            dtIntestazione = objIntest.DatiIntestazioneImpresa(_piva, Nothing, "", "", _objParametriServer)

            If Not IsNothing(dtIntestazione) AndAlso dtIntestazione.Rows.Count > 0 Then
                Parametro_Piva = dtIntestazione.Rows(0).Item("Piva")
                Parametro_CodiceFiscale = dtIntestazione.Rows(0).Item("Codice_Fiscale")
                Parametro_Rag_Soc = dtIntestazione.Rows(0).Item("rag_soc")
                Parametro_Indirizzo = dtIntestazione.Rows(0).Item("ind_impresa") & " " & dtIntestazione.Rows(0).Item("CAP") & " " &
                                      dtIntestazione.Rows(0).Item("frz_des") & " - " &
                                      dtIntestazione.Rows(0).Item("LOCALITA") & " (" & dtIntestazione.Rows(0).Item("COMUNI_PROV") & ") "
            End If

        Catch ex As Exception
            _logErrori &= "- Intestazione report: " & vbCrLf & ex.Message & vbCrLf
        End Try


        '==================================
        '====== CONTO ECONOMICO ===========
        '==================================
        If Chk_CE = True Then

            Mastrino_ContiEconomici(DSMastrino)

        End If 'fine Chk_CE


        '==================================
        '====== STATO PATRIMONIALE ========
        '==================================
        If Chk_SP = True Then

            Mastrino_ContiPatrimoniali(DSMastrino)

        End If 'fine Chk_SP


        If Chk_CE = False AndAlso Chk_SP = False Then
            Mastrino_ContiEconomici(DSMastrino)
            Mastrino_ContiPatrimoniali(DSMastrino)
        End If


        Try

            'imposto il dataset sul report
            _rptStampa.SetDataSource(DSMastrino)

        Catch exc As Exception
            _logErrori &= "- Aggancio dataset: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Try
            _rptStampa.SetParameterValue("Titolo", Parametro_Titolo)
            _rptStampa.SetParameterValue("Filtro", Parametro_Filtro)
            _rptStampa.SetParameterValue("Intervallo_Temp", Parametro_IntervalloTemp)
            _rptStampa.SetParameterValue("Piva", Parametro_Piva)
            _rptStampa.SetParameterValue("CodiceFiscale", Parametro_CodiceFiscale)
            _rptStampa.SetParameterValue("Rag_Soc", Parametro_Rag_Soc)
            _rptStampa.SetParameterValue("Indirizzo", Parametro_Indirizzo)

        Catch ex As Exception
            _logErrori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub


    '###################################################
    Private Sub Mastrino_ContiEconomici(ByRef DSMastrino As _DS_Mastrino)

        Dim objConti As New AgronicaCoreStampeDAL.PianoConti
        Dim DT As DataTable = Nothing
        Dim DTSaldi As DataTable = Nothing
        Dim DT_Codifiche_Eco As DataTable = Nothing
        'Dim Data_Inizio_Saldo As Date
        Dim Data_Fine_Saldo As Date

        Try
            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche_Eco = objRicXConti.Leggi_Codifica_ContiEconomici(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

        Catch ex As Exception
            _logErrori &= "- Leggi_Codifica_ContiEconomici: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            If _dataInizio < _gestContDataInizio AndAlso _dataFine < _gestContDataInizio Then
                'se l'intervallo temporale selezionato per la stampa è precedente alla data di inizio gestione contabile
                '-> non devo stampare niente
                '(questo filtro viene già fatto anche nel filtro elaborati contabili)
            Else

                ''filtro avanzato
                'Filtro_Conti = objPianoConti.Prepara_FiltroQuery_IdRicl(Filtro_Conti)

                DT = objConti.MastrinoConti_Economici(_dataInizio, _dataFine,
                                                      _piva, _anno, _ricCodEco,
                                                      "", "", CE_CONTO_IMPUTABILE_NOFILTRO,
                                                      _codContoEco,
                                                      CONTO_UE_NOFILTRO, "",
                                                      _sezionaleCod,
                                                      _codRisUm,
                                                       _flagEscludiIvaIndetraibile,
                                                      "", "", "", "", "",
                                                      DT_Codifiche_Eco,
                                                      _objParametriServer,
                                                      flagNuoviArrotondamenti:=_nuoviArrotondamenti)

                'calcolo il saldo al giorno primo (saldo iniziale al quale sommare i movimenti del periodo)
                Data_Fine_Saldo = DateAdd(DateInterval.Day, -1, CDate(_dataInizio))

                ''parametro da passare alla query per fargli leggere o meno i saldi iniziali
                'Dim Flag_LeggiSaldiIniziali As Boolean = True

                ''se sull'impresa ho detto di  non considerare i saldi
                'If GestCont_Flag_ConsideraSaldiIniziali = False Then
                '    Flag_LeggiSaldiIniziali = False
                'Else
                '    If Sezionale_Cod = SEZIONALE_NOFILTRO Then
                '        'non c'è filtro sul sezionale, ok leggo i saldi
                '    Else
                '        'altrimenti sto facendo un filtro sui sezionali
                '        'e i saldi iniziali vanno stampati solo sul sezionale principale
                '        If Flag_Sezionale_Default <> 1 Then
                '            'se il sezionale non è quello di default
                '            Flag_LeggiSaldiIniziali = False
                '        End If
                '    End If
                'End If

                DTSaldi = objConti.Saldo_Conti_Economici(AGRODATAINIZIO,
                                                         Data_Fine_Saldo,
                                                         False,
                                                         _gestContFlagConsideraSaldiIniziali,
                                                         _gestContDataInizio,
                                                         _piva, _anno, _ricCodEco,
                                                         "", "", CE_CONTO_IMPUTABILE_NOFILTRO,
                                                          _codContoEco,
                                                         CONTO_UE_NOFILTRO, "",
                                                         _sezionaleCod,
                                                         _sezionaleChkDefault,
                                                         _codRisUm,
                                                         False,
                                                         _flagEscludiIvaIndetraibile,
                                                         "", "", "", "",
                                                         DT_Codifiche_Eco,
                                                         False,
                                                         _objParametriServer,
                                                         flagNuoviArrotondamenti:=_nuoviArrotondamenti)

            End If 'controllo con GestCont_DataInizio

        Catch exc As Exception
            DT = Nothing
            DTSaldi = Nothing
            _logErrori &= "- Query CONTI ECONOMICI: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Try

            GestioneConti(DT, DTSaldi, DSMastrino, Data_Fine_Saldo, CONTO_ECONOMICO)

        Catch exc As Exception
            _logErrori &= "- Gestione e somme CONTI ECONOMICI: " & vbCrLf & exc.Message & vbCrLf
        End Try


    End Sub

    Private Sub GestioneConti(ByVal DT As DataTable,
                              ByVal DTSaldi As DataTable,
                              ByVal DSMastrino As _DS_Mastrino,
                              ByVal Data_Fine_Saldo As Date,
                              ByVal tipologiaECO_PAT As String)

        Dim i As Integer
        Dim DR As _DS_Mastrino.DS_MastrinoRow
        Dim CodiceSplitGruppo As String
        Dim HT_ContiMovimentati As Hashtable
        Dim Cod_Conto As Integer
        Dim Importo_Avere As Decimal = 0
        Dim Importo_Dare As Decimal = 0
        Dim idAgendaTemp As Long
        Dim desLibTemp As String = ""
        Dim listTemp As New List(Of _DS_Mastrino.DS_MastrinoRow)
        Dim HT_Movimenti As New Hashtable
        Dim Saldo_Conto As Decimal = 0


        Try

            Dim dv As DataView = Nothing

            If Not IsNothing(DT) Then

                '§§§GESTIONE DEL SALDO INIZIALE COME RIGA A PARTE
                If Not IsNothing(DTSaldi) AndAlso DTSaldi.Rows.Count > 0 Then

                    Dim drSaldiIniziali As DataRow
                    For i = 0 To DTSaldi.Rows.Count - 1

                        drSaldiIniziali = DT.NewRow

                        drSaldiIniziali.Item("CodiceSplitGruppo") = DTSaldi.Rows(i).Item("CodiceSplitGruppo")
                        drSaldiIniziali.Item("Cod_Conto_" & tipologiaECO_PAT) = DTSaldi.Rows(i).Item("Cod_Conto_" & tipologiaECO_PAT)
                        drSaldiIniziali.Item("Conto_" & tipologiaECO_PAT & "_Descr") = DTSaldi.Rows(i).Item("Conto_" & tipologiaECO_PAT & "_Descr")
                        drSaldiIniziali.Item("Id_Riclassificazione") = DTSaldi.Rows(i).Item("Id_Riclassificazione")
                        drSaldiIniziali.Item("Id_Agenda") = 0
                        drSaldiIniziali.Item("Lav_Cod") = 0
                        drSaldiIniziali.Item("Des_Lib") = "Saldo al " & CStr(Data_Fine_Saldo)
                        drSaldiIniziali.Item("Data_Movimento") = AGRODATAINIZIO
                        drSaldiIniziali.Item("Numero_Doc") = ""
                        drSaldiIniziali.Item("Data_Registrazione") = AGRODATAINIZIO
                        drSaldiIniziali.Item("Progr_Protocollo") = 0
                        drSaldiIniziali.Item("Dare") = DTSaldi.Rows(i).Item("Saldo_Dare")
                        drSaldiIniziali.Item("Avere") = DTSaldi.Rows(i).Item("Saldo_Avere")

                        DT.Rows.Add(drSaldiIniziali)
                    Next
                End If

                dv = New DataView(DT)

                Select Case tipologiaECO_PAT
                    Case CONTO_ECONOMICO
                        dv.Sort = "CodiceSplitGruppo, Anno, Id_Riclassificazione, Data_Order, Progr_Protocollo, Progr_Registrazione"
                    Case CONTO_PATRIMONIALE
                        dv.Sort = "CodiceSplitGruppo, Anno, Data_Order, Progr_Protocollo, Progr_Registrazione"
                End Select

            End If


            '§§§GESTIONE DEL SALDO INIZIALE COME RIGA A PARTE
            'If Not IsNothing(DT) Then
            '    If DT.Rows.Count > 0 Then
            '        HT_ContiMovimentati = New Hashtable
            '       For i = 0 To DT.Rows.Count - 1
            '           With DT.Rows(i)

            If Not IsNothing(dv) Then
                If dv.Count > 0 Then
                    HT_ContiMovimentati = New Hashtable
                    For i = 0 To dv.Count - 1
                        With dv(i)

                            '  Try

                            idAgendaTemp = .Item("Id_Agenda")
                            desLibTemp = .Item("Des_Lib")

                            DR = DSMastrino.DS_Mastrino.NewRow

                            CodiceSplitGruppo = .Item("CodiceSplitGruppo")

                            DR.CodiceSplitGruppo = CodiceSplitGruppo

                            Cod_Conto = .Item("Cod_Conto_" & tipologiaECO_PAT)

                            If Not HT_ContiMovimentati.ContainsKey(CodiceSplitGruppo) Then
                                HT_ContiMovimentati.Add(CodiceSplitGruppo, .Item("Conto_" & tipologiaECO_PAT & "_Descr"))

                                '  Giulia, 25/10/2016 09.16.22: Se è la prima volta che incontro questo conto, devo resettare il saldo,
                                '           in modo da ripartire da zero e non usare i saldi di riporto del conto precedente
                                Saldo_Conto = 0
                            End If

                            '§§§GESTIONE DEL SALDO INIZIALE COME RIGA A PARTE
                            ''If Cod_Conto <> Cod_Conto_Temp Then
                            'If CodiceSplitGruppo <> CodiceSplitGruppo_Temp Then
                            '    'primo giro o è cambiato il conto
                            '    'leggo il saldo iniziale
                            '    'Saldo_Iniziale = objHLP.ValueDbl_from_Cod(DTSaldi, "Cod_Conto_Eco", "Saldo", Cod_Conto)
                            '    Saldo_Iniziale = objHLP.ValueDbl_from_CodStr(DTSaldi, "CodiceSplitGruppo", "Saldo", CodiceSplitGruppo)
                            '    'il saldo conto viene valorizzato col saldo iniziale e decrementato/incrementato con il dare/avere
                            '    Saldo_Conto = Saldo_Iniziale
                            '    '  Cod_Conto_Temp = Cod_Conto
                            '    CodiceSplitGruppo_Temp = CodiceSplitGruppo
                            'Else
                            '    'ricorrenze successive dello stesso conto
                            '    debug = True
                            'End If

                            DR.Cod_Conto = Cod_Conto
                            DR.Conto_Descr = .Item("Conto_" & tipologiaECO_PAT & "_Descr")
                            DR.Id_Riclassificazione = .Item("Id_Riclassificazione")
                            DR.Articolo = ""
                            DR.Id_Agenda = .Item("Id_Agenda")
                            DR.Lav_Cod = .Item("Lav_Cod")
                            DR.Des_Lib = .Item("Des_Lib")

                            If .Item("Data_Movimento") = AGRODATAINIZIO Then
                                DR.Data_Movimento = ""
                            Else
                                DR.Data_Movimento = CDate(.Item("Data_Movimento")).ToShortDateString
                            End If

                            DR.Numero_Doc = .Item("Numero_Doc")

                            If .Item("Data_Registrazione") = AGRODATAINIZIO Then
                                DR.Data_Registrazione = ""
                            Else
                                DR.Data_Registrazione = CDate(.Item("Data_Registrazione")).ToShortDateString
                            End If

                            If .Item("Progr_Protocollo") = 0 Then
                                DR.Progr_Protocollo = ""
                            Else
                                DR.Progr_Protocollo = .Item("Progr_Protocollo")
                            End If

                            Importo_Dare = CDec(.Item("Dare"))
                            Importo_Avere = CDec(.Item("Avere"))

                            Saldo_Conto += CalcolaSaldoDareAvere(tipologiaECO_PAT, Importo_Avere, Importo_Dare)

                            DR.Dare = Importo_Dare
                            DR.Avere = Importo_Avere
                            DR.Saldo_Riga = Saldo_Conto


                            '  Giulia, 21/10/2016 11.44.40: Se ho già incontrato dei movimenti per quella fattura, in questo conto, li devo raggruppare
                            RaggruppamentoDettagliFattura(DR, HT_Movimenti, listTemp, CodiceSplitGruppo, idAgendaTemp, desLibTemp, Importo_Dare, Importo_Avere, tipologiaECO_PAT)

                            'Catch exc As Exception
                            '    Log_Errori &= "- Elaborazione movimenti x conto: " & vbCrLf & exc.Message & vbCrLf
                            'End Try

                        End With
                    Next

                    'Aggiungo solo alla fine le righe, così le ho potute raggrupare
                    For Each row As _DS_Mastrino.DS_MastrinoRow In listTemp
                        DSMastrino.DS_Mastrino.Rows.Add(row)
                    Next

                End If 'dt vuoto
            End If

        Catch ex As Exception
            _logErrori &= "- Gestione conti ECONOMICI: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

    Private Sub RaggruppamentoDettagliFattura(ByVal dr As _DS_Mastrino.DS_MastrinoRow,
                                              ByRef htMovimenti As Hashtable,
                                              ByRef listTemp As List(Of _DS_Mastrino.DS_MastrinoRow),
                                              ByVal codiceSplitGruppo As String,
                                              ByVal idAgendaTemp As Long,
                                              ByVal desLibTemp As String,
                                              ByVal importoDare As Decimal,
                                              ByVal importoAvere As Decimal,
                                              ByVal tipologiaECO_PAT As String)

        Dim keyTemp = codiceSplitGruppo & "|" & idAgendaTemp & "|" & desLibTemp

        If Not htMovimenti.ContainsKey(keyTemp) Then
            'prima volta che incontro un dettaglio in questo documento
            htMovimenti.Add(keyTemp, dr)
            listTemp.Add(dr)

        Else
            'Se sono pagamenti non li devo accorpare perché deve rimanere tracciabilità
            If desLibTemp.Contains("Pagamento") OrElse desLibTemp.Contains("Incasso") Then
                listTemp.Add(dr)
            Else
                Dim rowTemp As _DS_Mastrino.DS_MastrinoRow = (From l In listTemp
                                                              Where l.CodiceSplitGruppo = codiceSplitGruppo AndAlso
                                                                    l.Id_Agenda = idAgendaTemp AndAlso
                                                                    l.Des_Lib = desLibTemp
                                                              Select l).First

                Dim rowCopy As _DS_Mastrino.DS_MastrinoRow = rowTemp

                rowCopy.Dare += importoDare
                rowCopy.Avere += importoAvere

                rowCopy.Saldo_Riga += CalcolaSaldoDareAvere(tipologiaECO_PAT, importoAvere, importoDare)

                htMovimenti.Item(keyTemp) = rowCopy

                'Sostituisco i valori
                listTemp.Item(listTemp.FindIndex(Function(x) x.Equals(rowTemp))) = rowCopy
            End If

        End If

    End Sub


    '###################################################
    Private Sub Mastrino_ContiPatrimoniali(ByRef DSMastrino As _DS_Mastrino)

        Dim objConti As New AgronicaCoreStampeDAL.PianoConti
        Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
        Dim DT As DataTable = Nothing
        Dim DTSaldi As DataTable = Nothing
        Dim DT_Codifiche_Pat As DataTable = Nothing
        'Dim Data_Inizio_Saldo As Date
        Dim Data_Fine_Saldo As Date

        Try

            If _dataInizio < _gestContDataInizio AndAlso _dataFine < _gestContDataInizio Then
                'se l'intervallo temporale selezionato per la stampa è precedente alla data di inizio gestione contabile
                '-> non devo stampare niente
            Else

                'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
                DT_Codifiche_Pat = objRicXConti.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

                ''filtro avanzato
                'Filtro_Conti = objPianoConti.Prepara_FiltroQuery_IdRicl(Filtro_Conti)

                DT = objConti.MastrinoConti_Patrimoniali(_dataInizio, _dataFine,
                                                         _piva, _anno, _ricCodPat,
                                                         "", "", SP_CONTO_IMPUTABILE_NOFILTRO,
                                                         _codContoPat,
                                                         CONTO_UE_NOFILTRO,
                                                         "",
                                                         _sezionaleCod,
                                                         _codRisUm,
                                                         _codLiquidita,
                                                         "", "", "", "", "",
                                                         DT_Codifiche_Pat,
                                                         _objParametriServer,
                                                         _nuoviArrotondamenti)

                Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = False

                'calcolo il saldo al giorno primo (saldo iniziale al quale sommare i movimenti del periodo)
                Data_Fine_Saldo = DateAdd(DateInterval.Day, -1, CDate(_dataInizio))

                DTSaldi = objConti.Saldo_Conti_Patrimoniali(AGRODATAINIZIO,
                                                            Data_Fine_Saldo,
                                                            False,
                                                             _gestContFlagConsideraSaldiIniziali,
                                                            _gestContDataInizio,
                                                            _piva, _anno, _ricCodPat,
                                                            "", "", SP_CONTO_IMPUTABILE_NOFILTRO,
                                                            _codContoPat,
                                                            CONTO_UE_NOFILTRO, "",
                                                            _sezionaleCod,
                                                            _sezionaleChkDefault,
                                                            _codRisUm,
                                                            "",
                                                            _codLiquidita,
                                                            False,
                                                            Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                            "", "", "", "",
                                                            DT_Codifiche_Pat,
                                                            _objParametriServer,
                                                            flagNuoviArrotondamenti:=_nuoviArrotondamenti)

            End If


        Catch exc As Exception
            DT = Nothing
            DTSaldi = Nothing
            _logErrori &= "- Query CONTI PATRIMONIALI: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Try
            GestioneConti(DT, DTSaldi, DSMastrino, Data_Fine_Saldo, CONTO_PATRIMONIALE)
        Catch exc As Exception
            _logErrori &= "- Gestione e somme CONTI PATRIMONIALI: " & vbCrLf & exc.Message & vbCrLf
        End Try

    End Sub

End Class