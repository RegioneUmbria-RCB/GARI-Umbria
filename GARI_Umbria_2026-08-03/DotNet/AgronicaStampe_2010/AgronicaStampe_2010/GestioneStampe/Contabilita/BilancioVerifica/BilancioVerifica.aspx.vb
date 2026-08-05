Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class BilancioVerifica
    Inherits System.Web.UI.Page

    Private _rptBilancio As Rpt_BilancioVerifica

    Private _gestContFlagConsideraSaldiIniziali As Boolean
    Private _gestContDataInizio As Date
    Private _piva As String
    'Private Data_Inizio As String
    'Private Data_Fine As String
    Private _dataSaldo As String
    Private _anno, _annoMin As Integer
    Private _filtroConti As String
    Private _logErrori As String

    Private _chkCe, _chkSp, _flagContiSaldo0 As Boolean
    Private _contiUeTutti As Integer
    Private _ricCodEco As Integer = 0
    Private _ricCodPat As Integer = 0
    Private _codContoEco As Integer = 0
    Private _codContoPat As Integer = 0
    Private _sezionaleCod As Integer = SEZIONALE_NOFILTRO
    Private _sezionaleDes As String = ""
    Private _sezionaleChkDefault As Integer = -1
    Private _codRisUm, _codLiquidita As Integer
    Private _tipoPianoConti As Integer

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _nuoviArrotondamenti As Boolean = False

    Private Sub form1_Init(sender As Object, e As System.EventArgs) Handles form1.Init
        _rptBilancio = New Rpt_BilancioVerifica
    End Sub

    '#####################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim report As Integer

        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))


        _anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        'Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        'Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _dataSaldo = Stringa_Decodifica(CStr(Request.QueryString("ds")), AgroKey_EncoderDecoder, Server)

        _filtroConti = Stringa_Decodifica(CStr(Request.QueryString("fc")), AgroKey_EncoderDecoder, Server)


        _chkCe = Stringa_Decodifica(CStr(Request.QueryString("chk_ce")), AgroKey_EncoderDecoder, Server)

        _chkSp = Stringa_Decodifica(CStr(Request.QueryString("chk_sp")), AgroKey_EncoderDecoder, Server)

        _contiUeTutti = Stringa_Decodifica(CStr(Request.QueryString("ue")), AgroKey_EncoderDecoder, Server)

        'Conti_0Movimentati_1Tutti = Stringa_Decodifica(CStr(Request.QueryString("mov")), AgroKey_EncoderDecoder, Server)

        _flagContiSaldo0 = Stringa_Decodifica(CStr(Request.QueryString("cs")), AgroKey_EncoderDecoder, Server)

        _ricCodEco = Stringa_Decodifica(CStr(Request.QueryString("rce")), AgroKey_EncoderDecoder, Server)

        _codContoEco = Stringa_Decodifica(CStr(Request.QueryString("cce")), AgroKey_EncoderDecoder, Server)

        _ricCodPat = Stringa_Decodifica(CStr(Request.QueryString("rcp")), AgroKey_EncoderDecoder, Server)

        _codContoPat = Stringa_Decodifica(CStr(Request.QueryString("ccp")), AgroKey_EncoderDecoder, Server)

        _sezionaleCod = CInt(Stringa_Decodifica(CStr(Request.QueryString("szc")), AgroKey_EncoderDecoder, Server))

        _sezionaleDes = Stringa_Decodifica(CStr(Request.QueryString("szd")), AgroKey_EncoderDecoder, Server)

        _sezionaleChkDefault = Stringa_Decodifica(CStr(Request.QueryString("szdf")), AgroKey_EncoderDecoder, Server)

        _codRisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("cru")), AgroKey_EncoderDecoder, Server))

        _codLiquidita = CInt(Stringa_Decodifica(CStr(Request.QueryString("liq")), AgroKey_EncoderDecoder, Server))

        _tipoPianoConti = CInt(Stringa_Decodifica(CStr(Request.QueryString("tpc")), AgroKey_EncoderDecoder, Server))

        _gestContFlagConsideraSaldiIniziali = Stringa_Decodifica(CStr(Request.QueryString("gcfcsi")), AgroKey_EncoderDecoder, Server)

        _gestContDataInizio = Stringa_Decodifica(CStr(Request.QueryString("gcdi")), AgroKey_EncoderDecoder, Server)

        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim identificazioneDocumento As String = CStr(_anno)

        If _filtroConti <> "" Then
            identificazioneDocumento &= "_filtro"
        End If
        If _chkCe = 1 AndAlso _chkSp = 0 Then
            identificazioneDocumento &= "_CE"
        End If
        If _chkCe = 0 AndAlso _chkSp = 1 Then
            identificazioneDocumento &= "_SP"
        End If

        Dim nomeDocumento As String = "PianoDeiConti"

        If Not Me.IsPostBack Then

            Dim dsBilancio As New DS_BilancioVerifica

            Try

                _logErrori = ""

                Stampa_PianoDeiConti(dsBilancio)

            Catch exc As Exception
                _logErrori &= "- Stampa_PianoDeiConti: " & vbCrLf & exc.Message & vbCrLf
            End Try


            Dim nomeFilePdf As String = ""

            Try

                Dim dataInizioAllegati, dataFineAllegati As Date

                If _dataSaldo <> AGRODATAFINE Then
                    dataFineAllegati = _dataSaldo
                Else
                    dataFineAllegati = "31/12/" & CStr(_anno)
                End If

                dataInizioAllegati = "01/01/" & CStr(_anno)

                identificazioneDocumento &= "_" & Format(dataInizioAllegati, "yyyy_MM_dd") & "_" & Format(dataFineAllegati, "yyyy_MM_dd")
                nomeFilePdf = nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf"

                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.Bilancio, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(_rptBilancio,
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

            'Eliminato passaggio report in session per giro su file: Session("Report") = rptStampa
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptBilancio.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = ", anno = " & CStr(_anno) & ", Data_Saldo = " & CStr(_dataSaldo) & ", Filtro_Conti = " & CStr(_filtroConti)
            SalvaLogErrori_Generico(_logErrori, nomeDocumento, identificazioneDocumento, "PianoConti", "Stampe_Contabilita", infoInLog, _objParametriServer)
            '-----------------------------------------

            Try
                Dim pdf As String = Stringa_Decodifica(CStr(Request.QueryString("PDF")), AgroKey_EncoderDecoder, Server)

                If pdf = "1" Then
                    Response.Redirect("..\..\VisualizzatoreReport.aspx?tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                      "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))
                Else
                    Response.Redirect("..\..\VisualizzatoreReport.aspx?ForzaAnteprima=" & Stringa_Codifica("true", AgroKey_EncoderDecoder, Server) &
                                      "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))
                End If

            Catch ex As Exception
                _logErrori &= "- Export: " & vbCrLf & ex.Message & vbCrLf
            End Try

        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_PianoDeiConti_ContoEconomico(ByRef dsBilancio As DS_BilancioVerifica,
                                                    ByRef objPianoConti As AgronicaCoreStampeDAL.PianoConti,
                                                    ByRef dr As DS_BilancioVerifica.DT_BilancioVerificaRow)
        Dim dtEco As DataTable = Nothing

        Try

            'è il valore del radiobuttonlist sul filtro
            Select Case _tipoPianoConti

                Case 0
                    '--------------------------------------------
                    '------ PIANO DEI CONTI SENZA SALDO -----------
                    '--------------------------------------------
                    Dim objCe As New AgronicaCoreContabDAL.PianoConti_Economici_R

                    dtEco = objCe.PianoConti_ContoEconomico(_piva, _anno, "",
                                                            "", CE_CONTO_IMPUTABILE_NOFILTRO,
                                                            _ricCodEco,
                                                            "", _codContoEco,
                                                            _contiUeTutti,
                                                            "",
                                                            "", "",
                                                            _objParametriServer)


                Case 1, 2

                    Dim flagSoloSaldiIniziali As Boolean
                    Dim flagContiSaldo0 As Boolean
                    Dim dtCodificheEco As DataTable

                    If _tipoPianoConti = 1 Then
                        '--------------------------------------------
                        '------ PIANO DEI CONTI CON SALDO -----------
                        '--------------------------------------------

                        flagSoloSaldiIniziali = False
                        '  Giulia, 22/09/2016 11.44.10: anche se ho scelto di escludere i conti a zero qui li devo prendere perchè mi servono per fare le somme, 
                        '           poi li escluderò dalla stampa
                        flagContiSaldo0 = True

                    ElseIf _tipoPianoConti = 2 Then
                        '---------------------------------------------------------
                        '------ PIANO DEI CONTI CON SOLO SALDO RIPORTO -----------
                        '---------------------------------------------------------

                        flagSoloSaldiIniziali = True
                        flagContiSaldo0 = _flagContiSaldo0

                    End If

                    Try
                        Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_R
                        'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
                        dtCodificheEco = objRicXConti.Leggi_Codifica_ContiEconomici(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)
                    Catch ex As Exception
                        _logErrori &= "- Leggi_Codifica_ContiEconomici: " & vbCrLf & ex.Message & vbCrLf
                    End Try


                    dtEco = objPianoConti.Saldo_Conti_Economici(AGRODATAINIZIO,
                                                                _dataSaldo,
                                                                flagSoloSaldiIniziali,
                                                                _gestContFlagConsideraSaldiIniziali,
                                                                _gestContDataInizio,
                                                                _piva, _anno, _ricCodEco,
                                                                "", "", CE_CONTO_IMPUTABILE_NOFILTRO,
                                                                _codContoEco,
                                                                _contiUeTutti, "",
                                                                _sezionaleCod,
                                                                _sezionaleChkDefault,
                                                                _codRisUm,
                                                                flagContiSaldo0,
                                                                False,
                                                                "", "", "", "",
                                                                dtCodificheEco,
                                                                False,
                                                                _objParametriServer,
                                                                flagNuoviArrotondamenti:=_nuoviArrotondamenti)

            End Select


        Catch exc As Exception
            dtEco = Nothing
            _logErrori &= "- Query CONTI ECONOMICI: " & vbCrLf & exc.Message & vbCrLf
        End Try

        If Not IsNothing(dtEco) AndAlso dtEco.Rows.Count > 0 Then

            Dim saldoConFigli As Decimal = 0
            Dim saldo As Decimal
            Dim drSum() As DataRow
            Dim idRiclassificazione As String
            Dim i As Integer

            For i = 0 To dtEco.Rows.Count - 1

                'CodiceSplitGruppo = DTSaldi.Rows(i).Item("CodiceSplitGruppo")

                'azzero ad ogni giro
                saldoConFigli = 0

                dr = dsBilancio.DT_BilancioVerifica.NewRow

                dr.SezioneBilancio = "CE"
                dr.SezioneBilancioDescr = "Conto Economico"

                Select Case _tipoPianoConti

                    Case 0
                        '--------------------------------------------
                        '------ PIANO DEI CONTI SENZA SALDO -----------
                        '--------------------------------------------

                        dr.Cod_Conto = dtEco.Rows(i).Item("Cod_Conto")
                        dr.Conto_Descr = dtEco.Rows(i).Item("Conto_Descr")
                        dr.Id_Riclassificazione = dtEco.Rows(i).Item("Id_Riclassificazione")
                        dr.Dare = ""
                        dr.Avere = ""
                        dr.Saldo = ""

                        dsBilancio.DT_BilancioVerifica.Rows.Add(dr)

                    Case 1, 2

                        '--------------------------------------------
                        '------ PIANO DEI CONTI CON SALDO -----------
                        '------     O SOLO SALDO RIPORTO  -----------
                        '--------------------------------------------

                        'dr.CodiceSplitGruppo = CodiceSplitGruppo
                        dr.Cod_Conto = dtEco.Rows(i).Item("Cod_Conto_Eco")
                        dr.Conto_Descr = dtEco.Rows(i).Item("Conto_Eco_Descr")
                        dr.Id_Riclassificazione = dtEco.Rows(i).Item("Id_Riclassificazione")
                        dr.Dare = dtEco.Rows(i).Item("Saldo_Dare")
                        dr.Avere = dtEco.Rows(i).Item("Saldo_Avere")


                        'dr.Saldo = Format(dtEco.Rows(i).Item("Saldo"), "##,###,##0.00")

                        idRiclassificazione = dtEco.Rows(i).Item("Id_Riclassificazione")

                        'attenzione, percentuale solo in fondo 
                        'in modo da cercare tutti quelli che iniziano con quell'id_riclassificazione
                        'altrimenti, ad esempio: se filtro A.03
                        'mi trova anche A.01.a.03
                        'attenzione2, nella modalità costi/ricavi devo filtrare anche il dare/avere
                        'altrimenti i conti delle sezioni C,D,E che sono misti, influiscono nei totali di entrambi (costi/ricavi)

                        'MODIFICA DEL 19/01/2015:
                        'collina dei poeti ha sforato i conti oltre la z, quindi con il filtro precedente i zonti za, zb, ecc venivano considerati figli di z invece che fratelli
                        'dr = dtEco.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(idRiclassificazione) & "%' AND Dare_Avere = '" & Agro_SQL_SaveText(dare_avere) & "' ")
                        drSum = dtEco.Select(" ( Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(idRiclassificazione) & ".%' OR Id_Riclassificazione = '" & Agro_SQL_SaveText(idRiclassificazione) & "') ")

                        If Not IsNothing(drSum) AndAlso drSum.Length > 0 Then
                            For j = 0 To drSum.Length - 1
                                saldo = CDec(drSum(j).Item("Saldo"))
                                saldoConFigli += saldo
                                saldoConFigli = ArrotondaVal_2(saldoConFigli)
                            Next
                        Else
                            saldoConFigli = 0
                        End If

                        dr.Saldo = Format(saldoConFigli, "##,###,##0.00")

                        '  Giulia, 22/09/2016 11.07.12: se ho scelto di non mostrare i conti a zero non includo la riga nel RecordSet
                        If saldoConFigli <> 0 OrElse (saldoConFigli = 0 AndAlso _flagContiSaldo0 = True) Then
                            dsBilancio.DT_BilancioVerifica.Rows.Add(dr)
                        Else
                            dr = Nothing
                        End If

                End Select

            Next

        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_PianoDeiConti_StatoPatrimoniale(ByRef dsBilancio As DS_BilancioVerifica,
                                                       ByRef objPianoConti As AgronicaCoreStampeDAL.PianoConti,
                                                       ByRef dr As DS_BilancioVerifica.DT_BilancioVerificaRow)


        Dim dtSp As DataTable = Nothing
        Dim dtCodifichePat As DataTable

        Dim codContoPat_CreditiVersoClienti As Integer = 0
        Dim codContoPat_DebitiVersoFornitori As Integer = 0
        Dim codContoPat_DepositiBancariPostali As Integer = 0
        Dim codContoPat_DenaroValoriInCassa As Integer = 0
        Dim codContoPat_IvaACredito As Integer = 0
        Dim codContoPat_IvaACredito_AcqIntra As Integer = 0
        Dim codContoPat_IvaADebito As Integer = 0
        Dim codContoPat_IvaADebito_AcqIntra As Integer = 0

        Dim idRiclassificazione_CreditiVersoClienti As String = ""
        Dim idRiclassificazione_DebitiVersoFornitori As String = ""
        Dim idRiclassificazione_DepositiBancariPostali As String = ""
        Dim idRiclassificazione_DenaroValoriInCassa As String = ""
        Dim idRiclassificazione_IvaACredito As String = ""
        Dim idRiclassificazione_IvaACredito_AcqIntra As String = ""
        Dim idRiclassificazione_IvaADebito As String = ""
        Dim idRiclassificazione_IvaADebito_AcqIntra As String = ""

        Try


            Select Case _tipoPianoConti

                Case 0
                    '--------------------------------------------
                    '------ PIANO DEI CONTI SENZA SALDO -----------
                    '--------------------------------------------

                    Dim objSp As New AgronicaCoreContabDAL.PianoConti_Patrimoniali_R

                    dtSp = objSp.PianoConti_StatoPatrimoniale(_piva, _anno,
                                                              "",
                                                              "", SP_CONTO_IMPUTABILE_NOFILTRO,
                                                              _ricCodPat,
                                                              "", _codContoPat,
                                                              _contiUeTutti,
                                                              "", "",
                                                              _objParametriServer)

                Case 1, 2

                    Dim flagSoloSaldiIniziali As Boolean
                    Dim flagContiSaldo0 As Boolean
                    'se voglio anche i conti che hanno saldo = 0
                    'allora voglio vedere anche il conto master dei crediti/debiti/banche
                    Dim flagAggiungiContoPadreCreditiDebitiBanche As Boolean

                    If _tipoPianoConti = 1 Then
                        '--------------------------------------------
                        '------ PIANO DEI CONTI CON SALDO -----------
                        '--------------------------------------------

                        flagSoloSaldiIniziali = False
                        '  Giulia, 22/09/2016 11.08.13: se non voglio conti con saldi a zero, qui li devo comunque prendere perchè magari sono movimentati i figli
                        '       e mi servono per trovare la somma totale, poi se il conto (con i suoi figli) avrà saldo zero non lo inclderò nel recordset che mostro
                        flagContiSaldo0 = True
                        flagAggiungiContoPadreCreditiDebitiBanche = True

                    ElseIf _tipoPianoConti = 2 Then
                        '--------------------------------------------
                        '------ PIANO DEI CONTI CON SALDO RIPORTO ---
                        '--------------------------------------------

                        flagSoloSaldiIniziali = True
                        flagContiSaldo0 = _flagContiSaldo0
                        flagAggiungiContoPadreCreditiDebitiBanche = _flagContiSaldo0

                    End If

                    Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
                    'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
                    dtCodifichePat = objRicXConti.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

                    Dim objContabHlp As New AgronicaCoreContabHLP.Contabilita

                    objContabHlp.Recupera_CodContoPat_ContiPatrimoniali(dtCodifichePat,
                                                                        codContoPat_CreditiVersoClienti,
                                                                        codContoPat_DebitiVersoFornitori,
                                                                        codContoPat_DepositiBancariPostali,
                                                                        codContoPat_DenaroValoriInCassa,
                                                                        codContoPat_IvaACredito,
                                                                        codContoPat_IvaACredito_AcqIntra,
                                                                        codContoPat_IvaADebito,
                                                                        codContoPat_IvaADebito_AcqIntra)

                    objContabHlp.Recupera_IdRiclassificazione_ContiPatrimoniali(dtCodifichePat,
                                                                                idRiclassificazione_CreditiVersoClienti,
                                                                                idRiclassificazione_DebitiVersoFornitori,
                                                                                idRiclassificazione_DepositiBancariPostali,
                                                                                idRiclassificazione_DenaroValoriInCassa,
                                                                                idRiclassificazione_IvaACredito,
                                                                                idRiclassificazione_IvaACredito_AcqIntra,
                                                                                idRiclassificazione_IvaADebito,
                                                                                idRiclassificazione_IvaADebito_AcqIntra)


                    dtSp = objPianoConti.Saldo_Conti_Patrimoniali(AGRODATAINIZIO,
                                                                  _dataSaldo,
                                                                  flagSoloSaldiIniziali,
                                                                  _gestContFlagConsideraSaldiIniziali,
                                                                  _gestContDataInizio,
                                                                  _piva, _anno, _ricCodPat,
                                                                  "", "", SP_CONTO_IMPUTABILE_NOFILTRO,
                                                                  _codContoPat,
                                                                  _contiUeTutti, "",
                                                                  _sezionaleCod,
                                                                  _sezionaleChkDefault,
                                                                  _codRisUm,
                                                                  "",
                                                                  _codLiquidita,
                                                                  flagContiSaldo0,
                                                                  flagAggiungiContoPadreCreditiDebitiBanche,
                                                                  "", "", "", "",
                                                                  dtCodifichePat,
                                                                  _objParametriServer,
                                                                  flagNuoviArrotondamenti:=_nuoviArrotondamenti)

            End Select

        Catch exc As Exception
            dtSp = Nothing
            _logErrori &= "- Query CONTI PATRIMONIALI: " & vbCrLf & exc.Message & vbCrLf
        End Try

        'NOTA: il codice CodiceSplitGruppo si chiama così perchè è usato per splittare i gruppi nel report del mastrino contabile
        If Not IsNothing(dtSp) AndAlso dtSp.Rows.Count > 0 Then

            Dim codiceSplitGruppo As String

            Dim saldoConFigli As Decimal = 0
            Dim saldo As Decimal
            Dim drSum() As DataRow
            Dim idRiclassificazione As String
            Dim i As Integer

            For i = 0 To dtSp.Rows.Count - 1

                'azzero ad ogni giro
                saldoConFigli = 0

                dr = dsBilancio.DT_BilancioVerifica.NewRow

                If CStr(dtSp.Rows(i).Item("Dare_Avere")).ToUpper = "D" Then
                    dr.SezioneBilancio = "SP_1_ATTIVO"
                    dr.SezioneBilancioDescr = "Stato Patrimoniale - Attivo"
                Else
                    dr.SezioneBilancio = "SP_2_PASSIVO"
                    dr.SezioneBilancioDescr = "Stato Patrimoniale - Passivo"
                End If

                Select Case _tipoPianoConti

                    Case 0

                        '--------------------------------------------
                        '------ PIANO DEI CONTI SENZA SALDO ---------
                        '--------------------------------------------

                        dr.Cod_Conto = dtSp.Rows(i).Item("Cod_Conto_Pat")

                        dr.Id_Riclassificazione = dtSp.Rows(i).Item("Id_Riclassificazione")

                        dr.Conto_Descr = dtSp.Rows(i).Item("Conto_Pat_Descr")

                        dr.Dare = ""
                        dr.Avere = ""
                        dr.Saldo = ""

                        dsBilancio.DT_BilancioVerifica.Rows.Add(dr)

                    Case 1, 2

                        '--------------------------------------------
                        '------ PIANO DEI CONTI CON SALDO -----------
                        '------ O SOLO SALDO RIPORTO -----------
                        '--------------------------------------------

                        codiceSplitGruppo = dtSp.Rows(i).Item("CodiceSplitGruppo")

                        dr.Cod_Conto = dtSp.Rows(i).Item("Cod_Conto_Pat")

                        Select Case dtSp.Rows(i).Item("Cod_Conto_Pat")

                            Case codContoPat_CreditiVersoClienti,
                                codContoPat_DebitiVersoFornitori,
                                codContoPat_DepositiBancariPostali

                                'nel caso di questi conti, ce ne sono tanti quanti sono i clienti, i fornitori e le banche, 
                                'quindi per non mettere lo stesso Id_Riclassificazione, utilizzo il codicesplitgruppo

                                'tolgo il prefisso
                                idRiclassificazione = Replace(codiceSplitGruppo, "SP_", "")

                            Case Else
                                idRiclassificazione = dtSp.Rows(i).Item("Id_Riclassificazione")

                        End Select

                        dr.Id_Riclassificazione = idRiclassificazione

                        dr.Conto_Descr = dtSp.Rows(i).Item("Conto_Pat_Descr")

                        dr.Dare = dtSp.Rows(i).Item("Saldo_Dare")
                        dr.Avere = dtSp.Rows(i).Item("Saldo_Avere")


                        'DR.Saldo = Format(dtSp.Rows(i).Item("Saldo"), "##,###,##0.00")
                        '  Giulia, 22/09/2016 10.28.48: devo calcolare la somma di tutti i figli di un conto

                        'se si tratta dei conti automatici
                        If (CStr(idRiclassificazione).StartsWith(idRiclassificazione_CreditiVersoClienti) OrElse
                            CStr(idRiclassificazione).StartsWith(idRiclassificazione_DebitiVersoFornitori) OrElse
                            CStr(idRiclassificazione).StartsWith(idRiclassificazione_DenaroValoriInCassa) OrElse
                            CStr(idRiclassificazione).StartsWith(idRiclassificazione_DepositiBancariPostali)) AndAlso
                           (idRiclassificazione <> idRiclassificazione_CreditiVersoClienti AndAlso
                            idRiclassificazione <> idRiclassificazione_DebitiVersoFornitori AndAlso
                            idRiclassificazione <> idRiclassificazione_DenaroValoriInCassa AndAlso
                            idRiclassificazione <> idRiclassificazione_DepositiBancariPostali AndAlso
                            Not CStr(idRiclassificazione).StartsWith(idRiclassificazione_DepositiBancariPostali & ".")) Then

                            '  Giulia, 22/09/2016 09.31.26: qualcuno ha creato delle banche manuali, figlie di altre banche manuali (riconoscibili perchè hanno .### e non _B##)
                            '       in questo caso il saldo con figli va conteggiato normalmente come per tutte le altre categorie

                            'nel caso di crediti/debiti/banche
                            'essendoci il collegamento con i contatti e le risorse finanziarie
                            'se cerco per quell'id_riclassificazione (che contiene il codice del contatto, della banca) non troverò ovviamente niente
                            '(non esiste il conto fisico)
                            'e visto che non ci possono essere conti figli (il conto è foglia)
                            'saldoConFigli = Saldo
                            saldoConFigli = CDec(dtSp.Rows(i).Item("Saldo"))
                        Else

                            'CASO PARTICOLARE DEL PIANO DEI CONTI GIAS
                            'essendoci i conti C.002.d-bis e C.002.d-ter 
                            'che non sono figli di C.002.d ma fratelli
                            'per il calcolo del saldoconfigli del conto C.002.d
                            'non posso utilizzare il like come per tutti
                            'devo farne uno dedicato
                            'NOTA DEL 28/01/2016:
                            'con il migra 387 gli id sono stati trasformati tutti in numerici (tranne la prima parte)
                            'quindi questo caso sgaffo non c'è più
                            If idRiclassificazione = "C.002.d" Then
                                'i clienti con piano dei conti personalizzato
                                'non avranno mai questo caso

                                drSum = dtSp.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(idRiclassificazione) & "%' AND Id_Riclassificazione not like '%-bis%' AND Id_Riclassificazione not like '%-ter%' AND Dare_Avere = '" & Agro_SQL_SaveText(CStr(dtSp.Rows(i).Item("Dare_Avere")).ToUpper) & "'    ")

                                If Not IsNothing(dr) AndAlso drSum.Length > 0 Then

                                    For j = 0 To drSum.Length - 1
                                        saldo = CDec(drSum(j).Item("Saldo"))
                                        saldoConFigli += saldo
                                        saldoConFigli = ArrotondaVal_2(saldoConFigli)
                                    Next

                                Else
                                    saldoConFigli = 0
                                End If

                            Else

                                'attenzione, percentuale solo in fondo
                                'in modo da cercare tutti quelli che iniziano con quell'id_riclassificazione
                                'altrimenti, ad esempio: se filtro A.03
                                'mi trova anche A.01.a.03
                                'MODIFICA DEL 19/01/2015:
                                'collina dei poeti ha sforato i conti oltre la z, quindi con il filtro precedente i zonti za, zb, ecc venivano considerati figli di z invece che fratelli
                                'dr = dtSp.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(idRiclassificazione) & "%' AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")
                                drSum = dtSp.Select(" (Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(idRiclassificazione) & ".%' OR Id_Riclassificazione = '" & Agro_SQL_SaveText(idRiclassificazione) & "') AND Dare_Avere = '" & Agro_SQL_SaveText(CStr(dtSp.Rows(i).Item("Dare_Avere")).ToUpper) & "'    ")

                                If Not IsNothing(drSum) AndAlso drSum.Length > 0 Then
                                    For j = 0 To drSum.Length - 1
                                        saldo = CDec(drSum(j).Item("Saldo"))
                                        saldoConFigli += saldo
                                        saldoConFigli = ArrotondaVal_2(saldoConFigli)
                                    Next
                                Else
                                    saldoConFigli = 0
                                End If

                            End If 'conto specifico

                        End If ' conti automatici o no

                        'nel S.P. in avere ci sono le passività
                        'la query ritorna un valore negativo
                        'ma nel bilancio non devono esserci meno, quindi moltiplico * -1
                        'se dopo il * -1 ci sono negativi, allora sono errori dell'utente
                        If CStr(dtSp.Rows(i).Item("Dare_Avere")).ToUpper = "A" Then
                            saldoConFigli = saldoConFigli * -1
                            saldoConFigli = ArrotondaVal_2(saldoConFigli)
                        End If

                        dr.Saldo = Format(saldoConFigli, "##,###,##0.00")

                        '  Giulia, 22/09/2016 11.07.12: se ho scelto di non mostrare i conti a zero non includo la riga nel recordset
                        If saldoConFigli <> 0 OrElse (saldoConFigli = 0 AndAlso _flagContiSaldo0 = True) Then
                            dsBilancio.DT_BilancioVerifica.Rows.Add(dr)
                        Else
                            dr = Nothing
                        End If

                End Select

            Next

        End If

    End Sub

    'Tipo_PianoConti
    '#####################################################################
    Private Sub Stampa_PianoDeiConti(ByRef dsBilancio As DS_BilancioVerifica)

        ''filtro avanzato
        'Filtro_Conti = objPianoConti.Prepara_FiltroQuery_IdRicl(Filtro_Conti)

        Dim objPianoConti As New AgronicaCoreStampeDAL.PianoConti
        Dim dr As DS_BilancioVerifica.DT_BilancioVerificaRow = Nothing

        Try

            _annoMin = _gestContDataInizio.Year

            Dim strFiltro As String = ""
            Select Case _tipoPianoConti
                Case 0
                    CType(_rptBilancio.GroupHeaderSection1.ReportObjects("TxtSaldo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                Case 1
                    strFiltro = "Saldo al " & CStr(_dataSaldo)
                Case 2
                    strFiltro = "con Saldi di Riporto "
            End Select
            CType(_rptBilancio.Section1.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strFiltro

            Dim strIntTemp As String = ""

            If _sezionaleDes <> "" Then
                strIntTemp &= " - Sezionale: " & _sezionaleDes
            End If

            CType(_rptBilancio.Section1.ReportObjects("TxtAnnoContabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CStr(_anno)
            CType(_rptBilancio.Section1.ReportObjects("TxtIntervalloTemporale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strIntTemp

            Dim objIntest As New AgronicaCoreStampeDAL.DocContab
            Dim dtIntestazione As DataTable
            dtIntestazione = objIntest.DatiIntestazioneImpresa(_piva, Nothing, "", "", _objParametriServer)

            If Not IsNothing(dtIntestazione) AndAlso dtIntestazione.Rows.Count > 0 Then
                CType(_rptBilancio.Section1.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("Piva")
                CType(_rptBilancio.Section1.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("Codice_Fiscale")
                CType(_rptBilancio.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("rag_soc")
                CType(_rptBilancio.Section1.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtIntestazione.Rows(0).Item("ind_impresa") & " " &
                                                                                                                                     dtIntestazione.Rows(0).Item("CAP") & " " &
                                                                                                                                     dtIntestazione.Rows(0).Item("frz_des") & " - " &
                                                                                                                                     dtIntestazione.Rows(0).Item("LOCALITA") &
                                                                                                                                     " (" & dtIntestazione.Rows(0).Item("COMUNI_PROV") & ") "
            End If

        Catch ex As Exception
            _logErrori &= "- Intestazione report: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            If _chkCe = True Then
                Stampa_PianoDeiConti_ContoEconomico(dsBilancio, objPianoConti, dr)
            End If

            If _chkSp = True Then
                Stampa_PianoDeiConti_StatoPatrimoniale(dsBilancio, objPianoConti, dr)
            End If

        Catch exc As Exception
            _logErrori &= "- Popolamento DataSet: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Try

            'imposto il DataSet sul report
            _rptBilancio.SetDataSource(dsBilancio)

        Catch exc As Exception
            _logErrori &= "- Aggancio DataSet: " & vbCrLf & exc.Message & vbCrLf
        End Try

    End Sub

End Class