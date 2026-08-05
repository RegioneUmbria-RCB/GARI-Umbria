Imports AgronicaCoreDataProvider.Agro_Math
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Bilancio
    Inherits System.Web.UI.Page

    Private _rptBilancio As Rpt_Bilancio
    Private _rptSezioneA As Rpt_BilancioSezioneA
    Private _rptSezioneB As Rpt_BilancioSezioneB
    Private _rptSezioneC As Rpt_BilancioSezioneC
    Private _rptSezioneD As Rpt_BilancioSezioneD
    Private _rptSezioneE As Rpt_BilancioSezioneE

    Private _rptAttivita As Rpt_SP_Attivita
    Private _rptAttivitaDareAvere As Rpt_SPAttivita_DareAvere
    Private _rptPassivita As Rpt_SP_Passivita
    'Private rptPassivita_Dareavere As Rpt_SPPassivita_DareAvere
    Private _rptCostiRicavi As Rpt_CE_CostiRicavi

    Private _piva As String
    Private _dataInizio, _dataFine As String
    Private _dataInizioEco, _dataFineEco As String
    Private _dataInizioPat, _dataFinePat As String
    Private _anno As Integer
    Private _annoMin As Integer
    Private _esercizio As Integer
    Private _filtroConti As String
    Private _logErrori As String

    Private _sezionaleCod As Integer = SEZIONALE_NOFILTRO
    Private _sezionaleDes As String = ""
    Private _sezionaleChkDefault As Integer = -1
    Private _chkCE, _chkSP, _chkNs0 As Boolean
    Private _spCreditiClienti, _spBanche, _spCasse, _spDebitiFornitori As Boolean
    Private _contiUeTutti As Integer
    Private Sintetico0_Analitico1 As Integer
    Private CE_Layout_0Europeo_1CostiRicavi As Integer
    'Conti_0ImponibileNoZero_1Tutti
    'Conti_0Movimentati_1Tutti
    Private _gestContFlagConsideraSaldiIniziali As Boolean
    Private _gestContFlagConsideraSaldiInizialiEco, _gestContFlagConsideraSaldiInizialiPat As Boolean
    Private _gestContDataInizio As Date

    Private _objParametriServer As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Private _nuoviArrotondamenti As Boolean = False


#Region " REPORT BILANCIO "

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
        _rptBilancio = New Rpt_Bilancio
        _rptSezioneA = New Rpt_BilancioSezioneA
        _rptSezioneB = New Rpt_BilancioSezioneB
        _rptSezioneC = New Rpt_BilancioSezioneC
        _rptSezioneD = New Rpt_BilancioSezioneD
        _rptSezioneE = New Rpt_BilancioSezioneE
        _rptAttivita = New Rpt_SP_Attivita
        _rptAttivitaDareAvere = New Rpt_SPAttivita_DareAvere
        _rptPassivita = New Rpt_SP_Passivita
        _rptCostiRicavi = New Rpt_CE_CostiRicavi

    End Sub

#End Region

    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim Report As Integer


        _piva = Stringa_Decodifica(CStr(Request.QueryString("p")), AgroKey_EncoderDecoder, Server)

        Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), AgroKey_EncoderDecoder, Server))

        _anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), AgroKey_EncoderDecoder, Server))

        _esercizio = CInt(Stringa_Decodifica(CStr(Request.QueryString("es")), AgroKey_EncoderDecoder, Server))

        _dataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")), AgroKey_EncoderDecoder, Server)

        _dataFine = Stringa_Decodifica(CStr(Request.QueryString("df")), AgroKey_EncoderDecoder, Server)

        _filtroConti = Stringa_Decodifica(CStr(Request.QueryString("fc")), AgroKey_EncoderDecoder, Server)

        _chkCE = Stringa_Decodifica(CStr(Request.QueryString("chk_ce")), AgroKey_EncoderDecoder, Server)

        _chkSP = Stringa_Decodifica(CStr(Request.QueryString("chk_sp")), AgroKey_EncoderDecoder, Server)

        _contiUeTutti = Stringa_Decodifica(CStr(Request.QueryString("ue")), AgroKey_EncoderDecoder, Server)

        _chkNs0 = Stringa_Decodifica(CStr(Request.QueryString("chk_ns0")), AgroKey_EncoderDecoder, Server)

        Sintetico0_Analitico1 = Stringa_Decodifica(CStr(Request.QueryString("sa")), AgroKey_EncoderDecoder, Server)

        CE_Layout_0Europeo_1CostiRicavi = Stringa_Decodifica(CStr(Request.QueryString("lce")), AgroKey_EncoderDecoder, Server)

        _gestContFlagConsideraSaldiIniziali = Stringa_Decodifica(CStr(Request.QueryString("gcfcsi")), AgroKey_EncoderDecoder, Server)

        _gestContDataInizio = Stringa_Decodifica(CStr(Request.QueryString("gcdi")), AgroKey_EncoderDecoder, Server)


        _spCreditiClienti = Stringa_Decodifica(CStr(Request.QueryString("sp_cc")), AgroKey_EncoderDecoder, Server)
        _spBanche = Stringa_Decodifica(CStr(Request.QueryString("sp_b")), AgroKey_EncoderDecoder, Server)
        _spCasse = Stringa_Decodifica(CStr(Request.QueryString("sp_c")), AgroKey_EncoderDecoder, Server)
        _spDebitiFornitori = Stringa_Decodifica(CStr(Request.QueryString("sp_df")), AgroKey_EncoderDecoder, Server)

        'Se la stampa è per esercizio
        If _esercizio <> 0 Then
            'Dati da passare alle funzioni del conto economico 
            'Tutti i movimenti dell'esercizio fino a fine esercizio (proposto) o data fine che deve essere inclusa nell'esercizio 
            'compresi i saldi di Riporto a inizio gestione contabile se siamo in quell'esercizio
            _dataInizioEco = "01/01/" & CStr(_esercizio)
            _dataFineEco = _dataFine 'Editabile 
            If Year(_gestContDataInizio) = _esercizio Then
                _dataInizioEco = _gestContDataInizio
                _gestContFlagConsideraSaldiInizialiEco = True
            Else
                _gestContFlagConsideraSaldiInizialiEco = False
            End If

            'Dati da passare alle funzioni dello Stato Patrimoniale 
            'Tutti i movimenti fino a fine esercizio (proposto) o data fine che deve essere inclusa nell'esercizio
            'compresi i saldi di Riporto a inizio gestione contabile
            _dataInizioPat = _gestContDataInizio
            _dataFinePat = _dataFine 'Editabile 

            _gestContFlagConsideraSaldiInizialiPat = True


        Else 'come prima
            _dataInizioEco = _dataInizio
            _dataFineEco = _dataFine
            _dataInizioPat = _dataInizio
            _dataFinePat = _dataFine
            _gestContFlagConsideraSaldiInizialiEco = _gestContFlagConsideraSaldiIniziali
            _gestContFlagConsideraSaldiInizialiPat = _gestContFlagConsideraSaldiIniziali
        End If


        _objParametriServer = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        _nuoviArrotondamenti = UsaNuoviArrotondamenti(_objParametriServer)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim nomeDocumento As String = "Bilancio"
        Dim identificazioneDocumento As String = CStr(_anno)

        If _filtroConti <> "" Then
            identificazioneDocumento &= "_filtro"
        End If

        'If Chk_CE = 1 And Chk_SP = 0 Then
        '    identificazioneDocumento &= "_CE"
        'End If
        'If Chk_CE = 0 And Chk_SP = 1 Then
        '    identificazioneDocumento &= "_SP"
        'End If

        'Select Case Report
        '    Case enum_CodificaStampe.Bilancio_DiVerifica
        '        nomeDocumento = "Bilancio_DiVerifica"
        '    Case enum_CodificaStampe.Bilancio_Civilistico
        '        nomeDocumento = "Bilancio_Civilistico"
        '        'Case enum_CodificaStampe.Mastrino
        '        '    nomeDocumento = "MovimentixConto"
        'End Select

        If Not Me.IsPostBack Then

            Dim dsBilancio As New DS_Bilancio
            Dim dsAttivita As New DS_SP_Attivita
            Dim dsPassivita As New DS_SP_Passivita
            Dim dsCostiRicavi As New DS_CE_CostiRicavi

            Try

                _logErrori = ""

                Stampa_Bilancio(dsBilancio, dsAttivita, dsPassivita, dsCostiRicavi)

            Catch exc As Exception
                _logErrori &= "- StampaBilancio: " & vbCrLf & exc.Message & vbCrLf
            End Try


            Dim nomeFilePdf As String = ""

            Try

                Dim dataInizioAllegati, dataFineAllegati As Date

                If _dataInizio <> AGRODATAINIZIO AndAlso _dataFine <> AGRODATAFINE Then
                    dataInizioAllegati = _dataInizio
                    dataFineAllegati = _dataFine
                Else
                    dataInizioAllegati = "01/01/" & CStr(_anno)
                    dataFineAllegati = "31/12/" & CStr(_anno)
                End If

                If _esercizio <> 0 Then
                    dataInizioAllegati = _dataInizioEco
                    dataFineAllegati = _dataFine
                End If

                identificazioneDocumento &= "_" & Format(dataInizioAllegati, "yyyy-MM-dd") & "_" & Format(dataFineAllegati, "yyyy-MM-dd")
                nomeFilePdf = nomeDocumento & "_p" & _piva & "_" & identificazioneDocumento & ".pdf"
                
                ' leggo la sottocartella da CategorieDocumenti
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Dim sottoCartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.Bilancio, "", "", _objParametriServer)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                Dim strpath As String = objGestFile.SalvaReportPdf(_rptBilancio,
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

            'MS Eliminato passaggio in session per evitare problema deallocazione report: Session("Report") = rptBilancio
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                _rptBilancio.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                _logErrori &= "- Salvataggio report temporaneo: " & vbCrLf & ex.Message & vbCrLf
            End Try

            'MS Una volta persistito il report con i dati su file distruggo rpt e dataset ed eseguo un GC.Collect
            'In questo modo la memoria e i thread non rimangono allocati e non si blocca più dopo alcune stampe.

            dsBilancio.Dispose()
            dsBilancio = Nothing

            dsAttivita.Dispose()
            dsAttivita = Nothing

            dsPassivita.Dispose()
            dsPassivita = Nothing

            dsCostiRicavi.Dispose()
            dsCostiRicavi = Nothing

            _rptSezioneA.Close()
            _rptSezioneA.Dispose()
            _rptSezioneA = Nothing

            _rptSezioneB.Close()
            _rptSezioneB.Dispose()
            _rptSezioneB = Nothing

            _rptSezioneC.Close()
            _rptSezioneC.Dispose()
            _rptSezioneC = Nothing

            _rptSezioneD.Close()
            _rptSezioneD.Dispose()
            _rptSezioneD = Nothing

            _rptSezioneE.Close()
            _rptSezioneE.Dispose()
            _rptSezioneE = Nothing

            _rptAttivita.Close()
            _rptAttivita.Dispose()
            _rptAttivita = Nothing

            _rptAttivitaDareAvere.Close()
            _rptAttivitaDareAvere.Dispose()
            _rptAttivitaDareAvere = Nothing

            _rptPassivita.Close()
            _rptPassivita.Dispose()
            _rptPassivita = Nothing

            _rptCostiRicavi.Close()
            _rptCostiRicavi.Dispose()
            _rptCostiRicavi = Nothing

            _rptBilancio.Close()
            _rptBilancio.Dispose()
            _rptBilancio = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim infoInLog As String = "anno = " & CStr(_anno) & ", Data_Inizio = " & CStr(_dataInizio) &
                                      ", Data_Fine = " & CStr(_dataFine) & ", Filtro_Conti = " & CStr(_filtroConti)
            SalvaLogErrori_Generico(_logErrori, nomeDocumento, identificazioneDocumento, "Bilancio.aspx", "Stampe_Contabilita", infoInLog, _objParametriServer)

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
    Private Sub Stampa_ContoEconomico_LayoutEuropeo(ByRef DSBilancio As DS_Bilancio,
                                                    ByRef objPianoConti As AgronicaCoreStampeDAL.PianoConti,
                                                    ByRef Parametro_Tot_A_B As String,
                                                    ByRef Parametro_Tot_C15C16_C17 As String,
                                                    ByRef Parametro_TOT_D18_D19 As String,
                                                    ByRef Parametro_Tot_E20_E21 As String,
                                                    ByRef Parametro_Risultato_Prima_Imposte As String,
                                                    ByRef Parametro_Tot_Imposte As String,
                                                    ByRef Parametro_Tot_Utile As String,
                                                    ByRef Parametro_Lbl_Perdita As String,
                                                    ByRef Parametro_Lbl_Utile As String,
                                                    ByRef Utile_Esercizio As Decimal,
                                                    ByRef Perdita_Esercizio As Decimal)

        Dim DT_Eco As DataTable = Nothing
        Dim DT_Codifiche_Eco As DataTable = Nothing
        Dim Utile_Perdita_Esercizio As Decimal = 0

        Dim flagNoSaldo0 As Boolean = False

        '------------------------------------------------------------------
        '----------------------- CONTO ECONOMICO --------------------------
        '------------------------------------------------------------------

        'BILANCIO CIVILISTICO - CONTO ECONOMICO
        _rptBilancio.Section6.SectionFormat.EnableSuppress = False
        _rptBilancio.Section7.SectionFormat.EnableSuppress = False
        _rptBilancio.Section8.SectionFormat.EnableSuppress = False
        _rptBilancio.Section9.SectionFormat.EnableSuppress = False
        _rptBilancio.Section10.SectionFormat.EnableSuppress = False
        _rptBilancio.Section11.SectionFormat.EnableSuppress = False
        _rptBilancio.Section12.SectionFormat.EnableSuppress = False
        _rptBilancio.Section13.SectionFormat.EnableSuppress = False
        _rptBilancio.Section14.SectionFormat.EnableSuppress = False

        Try
            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche_Eco = objRicXConti.Leggi_Codifica_ContiEconomici(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

        Catch ex As Exception
            _logErrori &= "- Query Leggi_Codifica_ContiEconomici: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            flagNoSaldo0 = _chkNs0
            DT_Eco = objPianoConti.Saldo_Conti_Economici(_dataInizioEco,
                                                         _dataFineEco,
                                                         False,
                                                         _gestContFlagConsideraSaldiInizialiEco,
                                                         _gestContDataInizio,
                                                         _piva, _anno, BILANCIO_PERSONALIZZATO,
                                                         "", "", CE_CONTO_IMPUTABILE_NOFILTRO,
                                                         0,
                                                         CONTO_UE_NOFILTRO, "",
                                                         SEZIONALE_NOFILTRO,
                                                         -1,
                                                         0,
                                                         True,
                                                         False,
                                                         "", "", "", "",
                                                         DT_Codifiche_Eco,
                                                         False,
                                                         _objParametriServer,
                                                         flagNuoviArrotondamenti:=_nuoviArrotondamenti)

        Catch exc As Exception
            _logErrori &= "- Query Saldo_Conti_Economici: " & vbCrLf & exc.Message & vbCrLf
        End Try

        If Not IsNothing(DT_Eco) AndAlso DT_Eco.Rows.Count > 0 Then

            'CodiceSplitGruppo, Cod_Conto_Eco, Conto_Eco_Descr, Id_Riclassificazione, 
            'SUM(Dare) AS Saldo_Dare, SUM(Avere) AS Saldo_Avere, SUM(Avere) - SUM(Dare) AS Saldo

            Dim saldoConFigli As Decimal = 0
            Dim saldo As Decimal
            Dim dr() As DataRow
            Dim Id_Riclassificazione As String
            Dim Dare_Avere As String

            DT_Eco.Columns.Add("Saldo_ConFigli", GetType(Decimal))

            For i = 0 To DT_Eco.Rows.Count - 1

                'azzeroa d ogni giro
                saldoConFigli = 0

                Id_Riclassificazione = DT_Eco.Rows(i).Item("Id_Riclassificazione")
                Dare_Avere = CStr(DT_Eco.Rows(i).Item("Dare_Avere")).ToUpper

                'attenzione, percentuale solo in fondo
                'in modo da cercare tutti quelli che iniziano con quell'id_riclassificazione
                'altrimenti, ad esempio: se filtro A.03
                'mi trova anche A.01.a.03
                'MODIFICA DEL 19/01/2015:
                'collina dei poeti ha sforato i conti oltre la z, quindi con il filtro precedente i zonti za, zb, ecc venivano considerati figli di z invece che fratelli
                'Dr = DT_Eco.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%'")
                dr = DT_Eco.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & ".%' OR Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "'")

                If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                    For j = 0 To dr.Length - 1
                        saldo = CDec(dr(j).Item("Saldo"))
                        saldoConFigli += saldo
                    Next
                Else
                    saldoConFigli = 0
                End If

                'nel C.E. in dare ci sono i costi
                'la query ritorna un valore negativo
                'ma nel bilancio non devono esserci meno, quindi moltiplico * -1
                'se dopo il * -1 ci sono negativi, allora sono errori dell'utente
                If Dare_Avere = "D" Then
                    saldoConFigli = saldoConFigli * -1
                End If

                DT_Eco.Rows(i).Item("Saldo_ConFigli") = saldoConFigli

            Next ' popolamento Saldo_ConFigli

            Dim Somma_A As Decimal = 0
            Dim Somma_B As Decimal = 0
            Dim Somma_C As Decimal = 0
            Dim Somma_D As Decimal = 0
            Dim Somma_E As Decimal = 0
            Dim A_B As Decimal = 0
            Dim C15C16_C17 As Decimal = 0
            Dim D18_D19 As Decimal = 0
            Dim E20_E21 As Decimal = 0
            Dim E22 As Decimal = 0
            Dim E26 As Decimal = 0
            Dim Risultato As Decimal = 0

            Dim drEcoA As DS_Bilancio.DS_ARow
            Dim drEcoB As DS_Bilancio.DS_BRow
            Dim drEcoC As DS_Bilancio.DS_CRow
            Dim drEcoD As DS_Bilancio.DS_DRow
            Dim drEcoE As DS_Bilancio.DS_ERow


            '  Giulia, 15/09/2016 15.25.22: mi serve una lista dei conti ue che è obbligatorio stampare sul sintetico a prescindere dal loro livello o dal loro saldo
            Dim listBlock As List(Of String) = GetListContiEcoBlock()

            'Try

            For i = 0 To DT_Eco.Rows.Count - 1

                Id_Riclassificazione = CStr(DT_Eco.Rows(i).Item("Id_Riclassificazione"))
                saldoConFigli = CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli"))
                Dim flagUe As Integer = DT_Eco.Rows(i).Item("Flag_UE")

                Select Case Id_Riclassificazione
                    Case "A"
                        Somma_A = saldoConFigli
                    Case "B"
                        Somma_B = saldoConFigli
                    Case "C"
                        Somma_C = saldoConFigli
                    Case "D"
                        Somma_D = saldoConFigli
                    Case "E"
                        Somma_E = saldoConFigli
                    Case "C.015", "C.016"
                        C15C16_C17 += saldoConFigli
                    Case "C.017"
                        C15C16_C17 = C15C16_C17 - saldoConFigli
                    Case "D.018"
                        D18_D19 += saldoConFigli
                    Case "D.019"
                        D18_D19 = D18_D19 - saldoConFigli
                    Case "E.020"
                        E20_E21 += saldoConFigli
                    Case "E.021"
                        E20_E21 = E20_E21 - saldoConFigli
                    Case "E.022"
                        E22 = saldoConFigli
                End Select

                '###################################################

                Dim flagStampaRiga As Boolean = DecidiSeStampareEco(Id_Riclassificazione, listBlock, flagNoSaldo0, flagUe, saldoConFigli)

                If flagStampaRiga = True Then

                    If Id_Riclassificazione.StartsWith("A") Then

                        drEcoA = DSBilancio.DS_A.NewRow
                        AggiungiRigaDsEcoEuropeo(DT_Eco.Rows(i), drEcoA)
                        DSBilancio.DS_A.Rows.Add(drEcoA)

                    ElseIf Id_Riclassificazione.StartsWith("B") Then

                        drEcoB = DSBilancio.DS_B.NewRow
                        AggiungiRigaDsEcoEuropeo(DT_Eco.Rows(i), drEcoB)
                        DSBilancio.DS_B.Rows.Add(drEcoB)

                    ElseIf Id_Riclassificazione.StartsWith("C") Then

                        drEcoC = DSBilancio.DS_C.NewRow
                        AggiungiRigaDsEcoEuropeo(DT_Eco.Rows(i), drEcoC)
                        DSBilancio.DS_C.Rows.Add(drEcoC)

                    ElseIf Id_Riclassificazione.StartsWith("D") Then

                        drEcoD = DSBilancio.DS_D.NewRow
                        AggiungiRigaDsEcoEuropeo(DT_Eco.Rows(i), drEcoD)
                        DSBilancio.DS_D.Rows.Add(drEcoD)

                    ElseIf Id_Riclassificazione = "E" OrElse
                           Id_Riclassificazione = "E.020" OrElse
                           Id_Riclassificazione = "E.021" Then

                        drEcoE = DSBilancio.DS_E.NewRow
                        AggiungiRigaDsEcoEuropeo(DT_Eco.Rows(i), drEcoE)
                        DSBilancio.DS_E.Rows.Add(drEcoE)

                    End If

                End If 'sintetico/analitico
            Next

            DSBilancio.AcceptChanges()

            A_B = Somma_A - Somma_B

            Risultato = A_B + Somma_C + Somma_D + E20_E21 '+ E22

            E26 = Risultato - E22
            Utile_Perdita_Esercizio = E26

            Parametro_Tot_A_B = Format(A_B, "#,###,##0.00")
            Parametro_Tot_C15C16_C17 = Format(C15C16_C17, "#,###,##0.00")
            Parametro_TOT_D18_D19 = Format(D18_D19, "#,###,##0.00")
            Parametro_Tot_E20_E21 = Format(E20_E21, "#,###,##0.00")
            Parametro_Risultato_Prima_Imposte = Format(Risultato, "#,###,##0.00")
            Parametro_Tot_Imposte = Format(E22, "#,###,##0.00")
            Parametro_Tot_Utile = Format(Utile_Perdita_Esercizio, "#,###,##0.00")

            'se Utile_Perdita_Esercizio >0 -> utile
            'se Utile_Perdita_Esercizio <0 -> perdita
            'nel caso di UTILE, entrerebbe nello stato patrimoniale in AVERE
            'quindi dovrei moltiplicare per -1 per metterlo in passivo (per poi rimoltiplicare per -1 per visualizzarlo positivo,
            'quindi non sto a farlo)
            'nel caso di PERDITA, entrerebbe nello stato patrimoniale in DARE
            'quindi dovrei moltiplicare per -1 per metetrlo in attivo (per poi rimoltiplicare per -1 per visualizzarlo negativo,
            'quindi non sto a farlo)

            'If Utile_Perdita_Esercizio >= 0 Then
            '    'moltiplico per -1 perchè l'utile viene riportato nello stato patrimoniale in avere
            '    Utile_Esercizio = Utile_Perdita_Esercizio * -1
            '    Utile_Perdita_Esercizio = Utile_Perdita_Esercizio * -1
            'Else
            '    'moltiplico per -1 perchè la perdita viene riportata nello stato patrimoniale in dare
            '    Perdita_Esercizio = Utile_Perdita_Esercizio * -1
            '    Utile_Perdita_Esercizio = Utile_Perdita_Esercizio * -1
            'End If

            If Utile_Perdita_Esercizio >= 0 Then
                Utile_Esercizio = Utile_Perdita_Esercizio
                Parametro_Lbl_Utile = "UTILE DI ESERCIZIO"
            Else
                'moltiplico per -1 per visualizzarlo positivo
                Perdita_Esercizio = Utile_Perdita_Esercizio * -1
                Parametro_Lbl_Perdita = "PERDITA DI ESERCIZIO"
            End If

        End If

    End Sub

    '#####################################################################
    Private Sub AggiungiRigaDsEcoEuropeo(ByVal drEco As DataRow, ByRef drEcoEu As Object)

        drEcoEu.Saldo = Format(drEco.Item("Saldo_ConFigli"), "#,###,##0.00")
        drEcoEu.Conto_Descr = drEco.Item("Conto_Eco_Descr")
        drEcoEu.Id_Riclassificazione = drEco.Item("Id_Riclassificazione")

    End Sub

    '#####################################################################
    Private Shared Function GetListContiEcoBlock() As List(Of String)

        Return New List(Of String)(New String() {"A", "A.001", "A.002", "A.003", "A.004", "A.005", "A.005.001",
                                                 "B", "B.006", "B.007", "B.008", "B.009", "B.009.001", "B.009.002", "B.009.003", "B.009.004", "B.009.005",
                                                 "B.010", "B.010.001", "B.010.002", "B.010.003", "B.010.004", "B.011", "B.012", "B.013", "B.014",
                                                 "C", "C.015", "C.015.001", "C.015.002", "C.016", "C.016.001", "C.016.001.001", "C.016.001.002", "C.016.001.003",
                                                 "C.016.002", "C.016.003", "C.016.004", "C.016.004.001", "C.016.004.002", "C.016.004.003",
                                                 "C.017", "C.017.001", "C.017.002", "C.017.003",
                                                 "D", "D.018", "D.018.001", "D.018.002", "D.018.003", "D.019", "D.019.001", "D.019.002", "D.019.003",
                                                 "E", "E.020", "E.020.001", "E.021", "E.021.001", "E.021.002", "E.022", "E.026"})
    End Function

    '#####################################################################
    Private Sub Stampa_ContoEconomico_CostiRicavi(ByRef DSCostiRicavi As DS_CE_CostiRicavi,
                                                  ByRef objPianoConti As AgronicaCoreStampeDAL.PianoConti,
                                                  ByRef Parametro_Lbl_Perdita As String,
                                                  ByRef Parametro_Lbl_Utile As String,
                                                  ByRef Utile_Esercizio As Decimal,
                                                  ByRef Perdita_Esercizio As Decimal,
                                                  ByRef Parametro_CE_Totale_Costi As String,
                                                  ByRef Parametro_CE_Totale_Ricavi As String,
                                                  ByRef Parametro_CE_Pareggio_Dare As String,
                                                  ByRef Parametro_CE_Pareggio_Avere As String)

        Dim DT_Eco As DataTable = Nothing
        Dim DT_Codifiche_Eco As DataTable = Nothing
        Dim Utile_Perdita_Esercizio As Decimal = 0
        Dim Flag_UE As Integer
        Dim debug As Boolean

        Dim flagNoSaldo0 As Boolean = False

        '------------------------------------------------------------------
        '----------------------- CONTO ECONOMICO --------------------------
        '------------------------------------------------------------------

        _rptBilancio.DetailSection4.SectionFormat.EnableSuppress = False
        _rptBilancio.DetailSection5.SectionFormat.EnableSuppress = False

        Try
            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche_Eco = objRicXConti.Leggi_Codifica_ContiEconomici(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

        Catch ex As Exception
            _logErrori &= "- Query Leggi_Codifica_ContiEconomici: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            flagNoSaldo0 = _chkNs0
            DT_Eco = objPianoConti.Saldo_Conti_Economici(_dataInizioEco,
                                                         _dataFineEco,
                                                         False,
                                                         _gestContFlagConsideraSaldiInizialiEco,
                                                         _gestContDataInizio,
                                                         _piva, _anno, BILANCIO_PERSONALIZZATO,
                                                         "", "", CE_CONTO_IMPUTABILE_NOFILTRO,
                                                         0,
                                                         CONTO_UE_NOFILTRO, "",
                                                         SEZIONALE_NOFILTRO,
                                                         -1,
                                                         0,
                                                         True,
                                                         False,
                                                         "", "", "", "",
                                                         DT_Codifiche_Eco,
                                                         True,
                                                         _objParametriServer,
                                                         flagNuoviArrotondamenti:=_nuoviArrotondamenti)

        Catch exc As Exception
            _logErrori &= "- Query Saldo_Conti_Economici: " & vbCrLf & exc.Message & vbCrLf
        End Try

        If Not IsNothing(DT_Eco) AndAlso DT_Eco.Rows.Count > 0 Then

            'CodiceSplitGruppo, Cod_Conto_Eco, Conto_Eco_Descr, Id_Riclassificazione, 
            'SUM(Dare) AS Saldo_Dare, SUM(Avere) AS Saldo_Avere, SUM(Avere) - SUM(Dare) AS Saldo

            Dim Saldo_ConFigli As Decimal = 0
            Dim Saldo_ConFigli_Dare As Decimal = 0
            Dim Saldo_ConFigli_Avere As Decimal = 0
            Dim Saldo As Decimal
            'dim Saldo_Dare, Saldo_Avere
            Dim dr() As DataRow
            Dim Id_Riclassificazione As String
            Dim Dare_Avere As String

            DT_Eco.Columns.Add("Saldo_ConFigli", GetType(Decimal))
            DT_Eco.Columns.Add("Saldo_ConFigli_Dare", GetType(Decimal))
            DT_Eco.Columns.Add("Saldo_ConFigli_Avere", GetType(Decimal))

            For i = 0 To DT_Eco.Rows.Count - 1

                'azzero ad ogni giro
                Saldo_ConFigli = 0
                '  Saldo_ConFigli_Dare = 0
                ' Saldo_ConFigli_Avere = 0

                Id_Riclassificazione = DT_Eco.Rows(i).Item("Id_Riclassificazione")
                Dare_Avere = CStr(DT_Eco.Rows(i).Item("Dare_Avere")).ToUpper

                'attenzione, percentuale solo in fondo 
                'in modo da cercare tutti quelli che iniziano con quell'id_riclassificazione
                'altrimenti, ad esempio: se filtro A.03
                'mi trova anche A.01.a.03
                'attenzione2, nella modalità costi/ricavi devo filtrare anche il dare/avere
                'altrimenti i conti delle sezioni C,D,E che sono misti, influiscono nei totali di entrambi (costi/ricavi)

                'MODIFICA DEL 19/01/2015:
                'collina dei poeti ha sforato i conti oltre la z, quindi con il filtro precedente i zonti za, zb, ecc venivano considerati figli di z invece che fratelli
                'Dr = DT_Eco.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%' AND Dare_Avere = '" & Agro_SQL_SaveText(dare_avere) & "' ")
                dr = DT_Eco.Select(" ( Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & ".%' OR Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "') AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'  ")

                If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                    For j = 0 To dr.Length - 1
                        Saldo = CDec(dr(j).Item("Saldo"))
                        ' Saldo_Dare = Dr(j).Item("Saldo_Dare")
                        'Saldo_Avere = Dr(j).Item("Saldo_Avere")
                        Saldo_ConFigli += Saldo
                        ' Saldo_ConFigli_Dare += Saldo_dare
                        'Saldo_ConFigli_Avere += Saldo_avere
                    Next
                Else
                    Saldo_ConFigli = 0
                End If
                DT_Eco.Rows(i).Item("Saldo_ConFigli") = Saldo_ConFigli

                'nel C.E. in dare ci sono i costi
                'la query ritorna un valore negativo
                'ma nel bilancio non devono esserci meno, quindi moltiplico * -1
                'se dopo il * -1 ci sono negativi, allora sono errori dell'utente
                If Dare_Avere = "D" Then
                    DT_Eco.Rows(i).Item("Saldo_ConFigli_Dare") = Saldo_ConFigli * -1
                    DT_Eco.Rows(i).Item("Saldo_ConFigli_Avere") = 0
                Else
                    DT_Eco.Rows(i).Item("Saldo_ConFigli_Avere") = Saldo_ConFigli
                    DT_Eco.Rows(i).Item("Saldo_ConFigli_Dare") = 0
                End If

            Next ' popolamento Saldo_ConFigli

            '-----------------------------------------
            'azzero
            Dare_Avere = ""
            Saldo_ConFigli_Dare = 0
            Saldo_ConFigli_Avere = 0

            Dim Dr_CR As DS_CE_CostiRicavi.DT_Eco_CostiRicaviRow
            Dim Dare_Avere_Temp As String
            Dim Saldo_Conto As Decimal
            'Dim Saldo_Conto_Dare, Saldo_Conto_Avere As Decimal
            'Dim Saldo_Avere, Saldo_Dare, SaldoConFigli As Decimal
            Dim Saldo_Totale_Avere, Saldo_Totale_Dare As Decimal
            Dim Saldo_Totale_Costi, Saldo_Totale_Ricavi As Decimal


            '  Giulia, 15/09/2016 15.25.22: mi serve una lista dei conti ue che è obbligatorio stampare sul sintetico a prescindere dal loro livello o dal loro saldo
            Dim listBlock As List(Of String) = GetListContiEcoBlock()


            For i = 0 To DT_Eco.Rows.Count - 1

                Id_Riclassificazione = CStr(DT_Eco.Rows(i).Item("Id_Riclassificazione"))
                Flag_UE = DT_Eco.Rows(i).Item("Flag_UE")
                Dare_Avere = CStr(DT_Eco.Rows(i).Item("Dare_Avere")).ToUpper()


                '///////////////////////////////////////////////////
                '1)
                'in questa parte vengono gestite tutte le righe
                '///////////////////////////////////////////////////


                Saldo_Conto = CDec(DT_Eco.Rows(i).Item("Saldo"))

                If Dare_Avere <> Dare_Avere_Temp Then
                    Dare_Avere_Temp = Dare_Avere
                    'prima riga oppure finiti i costi e iniziano i ricavi
                    Saldo_Totale_Avere = 0
                    Saldo_Totale_Dare = 0
                Else
                    'stessi record del gruppo costi o ricavi
                    debug = True
                End If

                If Dare_Avere = "D" Then
                    'totale costi (sommo il saldo di ogni singolo conto)
                    Saldo_Totale_Costi += Saldo_Conto

                    'per i totali dare e avere, sommo i saldi con figli dei macrogruppi
                    Select Case Id_Riclassificazione

                        Case "B", "C.017", "D.019", "E.021", "E.022", "E.026"

                            Saldo_Totale_Dare += CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli_Dare"))
                            Saldo_Totale_Avere += CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli_Avere"))

                    End Select

                Else
                    'totale ricavi (sommo il saldo di ogni singolo conto)
                    Saldo_Totale_Ricavi += Saldo_Conto

                    'per i totali dare e avere, sommo i saldi con figli dei macrogruppi
                    Select Case Id_Riclassificazione

                        Case "A", "C", "D", "E"
                            Saldo_Totale_Dare += CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli_Dare"))
                            Saldo_Totale_Avere += CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli_Avere"))

                    End Select

                End If


                '///////////////////////////////////////////////////
                '2)
                'in questa parte vengono gestite solo le righe che devono essere visualizzate
                'e la valorizzazione dei campi del dataset
                '///////////////////////////////////////////////////


                Dim flagStampaRiga As Boolean = DecidiSeStampareEco(Id_Riclassificazione, listBlock, flagNoSaldo0, Flag_UE, CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli")))


                If flagStampaRiga = True Then

                    'If Sintetico0_Analitico1 = 0 And Flag_UE = CONTO_NONUE Then
                    '    '----------------------------------------------
                    '    'è stato scelto il bilancio sintetico e, 
                    '    'visto che il conto non è UE (quindi personalizzato)
                    '    'non lo inserisco nel dataset
                    '    '----------------------------------------------
                    '    debug = True
                    'Else

                    Dr_CR = DSCostiRicavi.DT_Eco_CostiRicavi.NewRow

                    Dr_CR.Cod_Conto = DT_Eco.Rows(i).Item("Cod_Conto_Eco")
                    Dr_CR.Conto_Descr = DT_Eco.Rows(i).Item("Conto_Eco_Descr")
                    Dr_CR.Id_Riclassificazione = Id_Riclassificazione
                    Dr_CR.Dare_Avere = Dare_Avere

                    'campi non utilizzati
                    Dr_CR.Id_Riclassificazione_Padre = ""
                    Dr_CR.conto_descr_Padre = ""
                    Dr_CR.Saldo_Str = ""
                    Dr_CR.SaldoTotale = 0
                    Dr_CR.Perc_Avere = 0
                    Dr_CR.Perc_Dare = 0

                    'questi saldi sono in positivo e conteggiano i saldi dei figli
                    'vanno visualizzati per riga ma non conteggiati nei totali
                    Saldo_ConFigli_Dare = CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli_Dare"))
                    Saldo_ConFigli_Avere = CDec(DT_Eco.Rows(i).Item("Saldo_ConFigli_Avere"))

                    'questi sono gli importi di riga
                    Dr_CR.Saldo_Dare = Format(Saldo_ConFigli_Dare, "#,###,##0.00")
                    Dr_CR.Saldo_Avere = Format(Saldo_ConFigli_Avere, "#,###,##0.00")

                    ''questo ha + o - a seconda di costo/ricavo
                    'SaldoConFigli = DT_Eco.Rows(i).Item("Saldo_ConFigli")
                    'Dr_CR.Saldo = Format(DT_Eco.Rows(i).Item("Saldo_ConFigli"), "#,###,##0.00")

                    Dr_CR.SaldoTotale_Avere = Format(Saldo_Totale_Avere, "#,###,##0.00")
                    Dr_CR.SaldoTotale_Dare = Format(Saldo_Totale_Dare, "#,###,##0.00")

                    DSCostiRicavi.DT_Eco_CostiRicavi.Rows.Add(Dr_CR)

                End If 'sintetico/analitico

            Next 'popolamento dataset

            'Saldo_Totale_Costi ha già il -
            Utile_Perdita_Esercizio = Saldo_Totale_Ricavi + Saldo_Totale_Costi
            Saldo_Totale_Costi = Saldo_Totale_Costi * -1

            If Utile_Perdita_Esercizio >= 0 Then
                Utile_Esercizio = Utile_Perdita_Esercizio
                Parametro_Lbl_Utile = "UTILE DI ESERCIZIO"
            Else
                'moltiplico per -1 per visualizzarlo positivo
                Perdita_Esercizio = Utile_Perdita_Esercizio * -1
                Parametro_Lbl_Perdita = "PERDITA DI ESERCIZIO"
            End If

            Parametro_CE_Totale_Costi = Format(Saldo_Totale_Costi, "#,###,##0.00")
            Parametro_CE_Totale_Ricavi = Format(Saldo_Totale_Ricavi, "#,###,##0.00")
            Parametro_CE_Pareggio_Dare = Format(Saldo_Totale_Costi + Utile_Esercizio, "#,###,##0.00")
            Parametro_CE_Pareggio_Avere = Format(Saldo_Totale_Ricavi + Perdita_Esercizio, "#,###,##0.00")

        End If

    End Sub

    '#####################################################################
    Private Sub Stampa_StatoPatrimoniale_SaldoUnico(ByRef DSAttivita As DS_SP_Attivita,
                                                    ByRef DSPassivita As DS_SP_Passivita,
                                                    ByRef objPianoConti As AgronicaCoreStampeDAL.PianoConti,
                                                    ByRef Totale_Attivita As Decimal,
                                                    ByRef Totale_Passivita As Decimal)


        Dim dtSp As DataTable = Nothing
        Dim DT_Codifiche_Pat As DataTable
        Dim objHLP As New AgronicaCoreContabHLP.Contabilita

        Dim flagNoSaldo0 As Boolean = False

        '------------------------------------------------------------------
        '----------------------- STATO PATRIMONIALE -----------------------
        '------------------------------------------------------------------

        _rptBilancio.DetailSection1.SectionFormat.EnableSuppress = False
        _rptBilancio.DetailSection2.SectionFormat.EnableSuppress = False
        _rptBilancio.DetailSection3.SectionFormat.EnableSuppress = False


        Try

            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche_Pat = objRicXConti.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

            Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = True
            flagNoSaldo0 = _chkNs0

            dtSp = objPianoConti.Saldo_Conti_Patrimoniali_x_Bilancio(_dataInizioPat,
                                                                     _dataFinePat,
                                                                     False,
                                                                     _gestContFlagConsideraSaldiInizialiPat,
                                                                     _gestContDataInizio,
                                                                     _piva,
                                                                     _anno,
                                                                     BILANCIO_PERSONALIZZATO,
                                                                     "",
                                                                     "",
                                                                     SP_CONTO_IMPUTABILE_NOFILTRO,
                                                                     0,
                                                                     CONTO_UE_NOFILTRO,
                                                                     "",
                                                                     SEZIONALE_NOFILTRO,
                                                                     -1,
                                                                     0,
                                                                     "",
                                                                     -1,
                                                                     True,
                                                                     Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     "",
                                                                     DT_Codifiche_Pat,
                                                                     _objParametriServer,
                                                                     flagNuoviArrotondamenti:=_nuoviArrotondamenti)

        Catch exc As Exception
            _logErrori &= "- Query Saldo_Conti_Patrimoniali: " & vbCrLf & exc.Message & vbCrLf
        End Try

        'Try

        If Not IsNothing(dtSp) AndAlso dtSp.Rows.Count > 0 Then

            Dim idRiclCreditiVersoClienti As String = ""
            Dim idRiclDebitiVersoFornitori As String = ""
            Dim idRiclDepositiBancariPostali As String = ""
            Dim idRiclDenaroValoriInCassa As String = ""
            Dim idRiclIvaACredito As String = ""
            Dim idRiclIvaACreditoAcqIntra As String = ""
            Dim idRiclIvaADebito As String = ""
            Dim idRiclIvaADebitoAcqIntra As String = ""
            'Dim Id_Riclassificazione_ErarioRitenuteLavoroAutonomo As String
            'Dim Id_Riclassificazione_DebitiVsEnasarco As String

            objHLP.Recupera_IdRiclassificazione_ContiPatrimoniali(DT_Codifiche_Pat,
                                                                  idRiclCreditiVersoClienti,
                                                                  idRiclDebitiVersoFornitori,
                                                                  idRiclDepositiBancariPostali,
                                                                  idRiclDenaroValoriInCassa,
                                                                  idRiclIvaACredito,
                                                                  idRiclIvaACreditoAcqIntra,
                                                                  idRiclIvaADebito,
                                                                  idRiclIvaADebitoAcqIntra)



            Dim saldoConFigli As Decimal = 0
            Dim saldo As Decimal
            Dim dr() As DataRow
            Dim Id_Riclassificazione, Dare_Avere, conto_descr_Padre As String

            'aggiungo le colonne che mi servono per il gruppo sul report
            dtSp.Columns.Add("Id_riclassificazione_Padre", GetType(String))
            dtSp.Columns.Add("conto_descr_Padre", GetType(String))

            'aggiungo la colonna col saldo totale conto e suoi sottoconti
            dtSp.Columns.Add("Saldo_ConFigli", GetType(Decimal))

            For i = 0 To dtSp.Rows.Count - 1

                'azzero ad ogni giro
                saldoConFigli = 0

                'uso CodiceSplitGruppo invece di Id_Riclassificazione 
                'perchè per i conti che si agganciano ai contatti e alle banche, ho bisogno dell'id_riclassificazione + il codice del contatto/banca
                'Id_Riclassificazione = DT_SP.Rows(i).Item("Id_Riclassificazione")
                'tolgo il prefisso
                Id_Riclassificazione = Replace(dtSp.Rows(i).Item("CodiceSplitGruppo"), "SP_", "")

                dtSp.Rows(i).Item("Id_riclassificazione") = Id_Riclassificazione

                Dare_Avere = dtSp.Rows(i).Item("Dare_Avere")



                'Id_Riclassificazione_DenaroValoriInCassa

                'se si tratta dei conti automatici
                If (Id_Riclassificazione.StartsWith(idRiclCreditiVersoClienti) = True OrElse
                    Id_Riclassificazione.StartsWith(idRiclDebitiVersoFornitori) = True OrElse
                    Id_Riclassificazione.StartsWith(idRiclDenaroValoriInCassa) = True OrElse
                    Id_Riclassificazione.StartsWith(idRiclDepositiBancariPostali) = True) AndAlso
                   (Id_Riclassificazione <> idRiclCreditiVersoClienti AndAlso
                    Id_Riclassificazione <> idRiclDebitiVersoFornitori AndAlso
                    Id_Riclassificazione <> idRiclDenaroValoriInCassa AndAlso
                    Id_Riclassificazione <> idRiclDepositiBancariPostali AndAlso
                    Id_Riclassificazione.StartsWith(idRiclDepositiBancariPostali & ".") = False) Then
                    '  Giulia, 22/09/2016 09.31.26: qualcuno ha creato delle banche manuali, figlie di altre banche manuali (riconoscibili perchè hanno .### e non _B##)
                    '       in questo caso il saldo con figli va conteggiato normalmente come per tutte le altre categorie

                    'nel caso di crediti/debiti/banche
                    'essendoci il collegamento con i contatti e le risorse finanziarie
                    'se cerco per quell'id_riclassificazione (che contiene il codice del contatto, della banca) non troverò ovviamente niente
                    '(non esiste il conto fisico)
                    'e visto che non ci possono essere conti figli (il conto è foglia)
                    'Saldo_ConFigli = Saldo
                    saldoConFigli = dtSp.Rows(i).Item("Saldo")
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
                    If Id_Riclassificazione = "C.002.d" Then
                        'i clienti con piano dei conti personalizzato
                        'non avranno mai questo caso

                        dr = dtSp.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%' AND Id_Riclassificazione not like '%-bis%' AND Id_Riclassificazione not like '%-ter%' AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")

                        If Not IsNothing(dr) AndAlso dr.Length > 0 Then

                            For j = 0 To dr.Length - 1
                                saldo = dr(j).Item("Saldo")
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
                        'Dr = DT_SP.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%' AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")
                        dr = dtSp.Select(" (Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & ".%' OR Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "') AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")

                        If Not IsNothing(dr) AndAlso dr.Length > 0 Then
                            For j = 0 To dr.Length - 1
                                saldo = CDec(dr(j).Item("Saldo"))
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


                '27/08/2015: 
                ''UTILE DI ESERCIZIO
                'If Id_Riclassificazione = "A.009" And DT_SP.Rows(i).Item("Dare_Avere").toupper = "A" Then
                '    'utile di esercizio:
                '    'si legge il saldo delle operazioni, 
                '    ' + la differenza derivata dal conto economico

                '    'Saldo_ConFigli += Utile_Esercizio
                '    Saldo_ConFigli += Utile_perdita_esercizio
                'End If

                ''PATRIMONIO NETTO
                'If Id_Riclassificazione = "A" And DT_SP.Rows(i).Item("Dare_Avere").toupper = "A" Then
                '    'patrimonio netto:
                '    'devo aggiungere l'utile e la perdita
                '    Saldo_ConFigli += Utile_Perdita_Esercizio
                'End If


                dtSp.Rows(i).Item("Saldo_ConFigli") = saldoConFigli

                dtSp.Rows(i).Item("Id_riclassificazione_Padre") = Left(Id_Riclassificazione, 1)

                'non serve più conto_pat_descr_padre -> non mis erve il nome del gruppo
                'Dr = DT_SP.Select(" Id_Riclassificazione = '" & Agro_SQL_SaveText(Left(Id_Riclassificazione, 1)) & "' AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")

                'If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                '    For j = 0 To Dr.Length - 1
                '        conto_descr_Padre = Dr(j).Item("Conto_Pat_Descr")
                '        Exit For
                '    Next
                'Else
                '    'non deve succedere
                '    conto_descr_Padre = ""
                'End If
                conto_descr_Padre = ""
                dtSp.Rows(i).Item("conto_descr_Padre") = conto_descr_Padre

            Next ' popolamento Saldo_ConFigli



            '  Giulia, 15/09/2016 15.25.22: mi serve una lista dei conti ue che è obbligatorio stampare sul sintetico a prescindere dal loro livello o dal loro saldo
            Dim listBlockAtt As New List(Of String)(New String() {"B", "B.001", "B.002", "B.003", "C", "C.001", "C.002", idRiclCreditiVersoClienti, "C.002.005",
                                                                  idRiclIvaACredito, idRiclIvaACreditoAcqIntra, "C.003", "C.004",
                                                                  idRiclDepositiBancariPostali, idRiclDenaroValoriInCassa})
            Dim listBlockPas As New List(Of String)(New String() {"A", "A.001", "A.002", "A.003", "A.004", "A.005", "A.006", "A.007", "A.008", "A.009", "B", "C", "D",
                                                                  idRiclDebitiVersoFornitori, idRiclIvaADebito, idRiclIvaADebitoAcqIntra})



            Dim drAtt As DS_SP_Attivita.DS_SP_AttivitaRow
            Dim drPass As DS_SP_Passivita.DS_SP_PassivitaRow

            For i = 0 To dtSp.Rows.Count - 1

                Dim flagUe As Integer = dtSp.Rows(i).Item("Flag_UE")
                Dim flagDareAvere As String = dtSp.Rows(i).Item("Dare_Avere")

                Id_Riclassificazione = CStr(dtSp.Rows(i).Item("Id_Riclassificazione"))
                saldoConFigli = CDec(dtSp.Rows(i).Item("Saldo_ConFigli"))

                If Id_Riclassificazione.Length = 1 AndAlso flagDareAvere = "D" Then
                    Totale_Attivita += saldoConFigli

                ElseIf Id_Riclassificazione.Length = 1 AndAlso flagDareAvere = "A" Then
                    Totale_Passivita += saldoConFigli
                End If

                Dim flagStampaRiga As Boolean = DecidiSeStamparePat(saldoConFigli, idRiclCreditiVersoClienti, idRiclDebitiVersoFornitori, idRiclDepositiBancariPostali, idRiclDenaroValoriInCassa, Id_Riclassificazione, flagDareAvere, listBlockAtt, listBlockPas, flagNoSaldo0, flagUe)

                If flagStampaRiga = True Then

                    Select Case flagDareAvere

                        Case "D"
                            '-----------------------------------
                            '-------- ATTIVITA' - DARE ---------
                            '-----------------------------------                                  

                            drAtt = DSAttivita._DS_SP_Attivita.NewRow
                            AggiungiRigaDsPat(drAtt, dtSp.Rows(i))
                            drAtt.Saldo_Dare = Format(0, "#,###,##0.00")
                            drAtt.Saldo_Avere = Format(0, "#,###,##0.00")

                            DSAttivita._DS_SP_Attivita.Rows.Add(drAtt)

                        Case "A"
                            '--------------------------------------
                            '-------- PASSIVITA' - AVERE ---------
                            '--------------------------------------                                   

                            drPass = DSPassivita._DS_SP_Passivita.NewRow
                            AggiungiRigaDsPat(drPass, dtSp.Rows(i))
                            DSPassivita._DS_SP_Passivita.Rows.Add(drPass)

                    End Select

                End If

            Next 'dt_sp

        End If

        DSPassivita.AcceptChanges()
        DSAttivita.AcceptChanges()

        'Catch ex As Exception
        '    Log_Errori &= "- Caricamento dataset: " & vbCrLf & ex.Message & vbCrLf
        'End Try

    End Sub

    '#####################################################################
    Private Sub AggiungiRigaDsPat(ByRef drAttPas As Object, ByVal riga As DataRow)

        drAttPas.Id_Riclassificazione_Padre = riga.Item("Id_Riclassificazione_Padre")
        drAttPas.conto_descr_Padre = riga.Item("conto_descr_Padre")

        drAttPas.Cod_Conto = riga.Item("Cod_Conto_Pat")
        drAttPas.Id_Riclassificazione = riga.Item("Id_Riclassificazione")
        drAttPas.Dare_Avere = riga.Item("Dare_Avere")
        drAttPas.Conto_Descr = riga.Item("Conto_Pat_Descr")

        drAttPas.Saldo = 0
        drAttPas.SaldoTotale = CDec(riga.Item("Saldo_ConFigli"))
        drAttPas.Saldo_Str = Format(CDec(riga.Item("Saldo_ConFigli")), "#,###,##0.00")

    End Sub


    '06-04-2016 : NOTE MAGA
    'Questa funzione è la corrispondente della Stampa_ContoEconomico_CostiRicavi (x stampa sezioni contrapposte)
    'ma per lo stato patrimoniale. Collina dei poeti lo aveva chiesto x il conto economico,
    'poi la Maga lo stava facendo x lo stato patrimoniale --> 
    'quindi questa funzione è per l'SP ma non è mai stata provata  
    '#####################################################################
    Private Sub Stampa_StatoPatrimoniale_DareAvere(ByRef DSAttivita As DS_SP_Attivita,
                                                   ByRef DSPassivita As DS_SP_Passivita,
                                                   ByRef objPianoConti As AgronicaCoreStampeDAL.PianoConti,
                                                   ByRef Totale_Attivita As Decimal,
                                                   ByRef Totale_Passivita As Decimal)


        Dim DT_SP As DataTable = Nothing
        Dim DT_Codifiche_Pat As DataTable
        Dim Flag_UE As Integer
        Dim debug As Boolean
        Dim objHLP As New AgronicaCoreContabHLP.Contabilita

        '------------------------------------------------------------------
        '----------------------- STATO PATRIMONIALE -----------------------
        '------------------------------------------------------------------

        _rptBilancio.DetailSection6.SectionFormat.EnableSuppress = False
        '_rptBilancio.DetailSection7.SectionFormat.EnableSuppress = False
        _rptBilancio.DetailSection3.SectionFormat.EnableSuppress = False


        Try

            Dim objRicXConti As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
            'leggi DT con codifiche conti personalizzati-gias (x chi ha i conti gias, i codici coincidono)
            DT_Codifiche_Pat = objRicXConti.Leggi_Codifica_ContiPatrimoniali(_piva, BILANCIO_PERSONALIZZATO, _anno, _annoMin, "", _objParametriServer)

            Dim Flag_AggiungiContoPadre_CreditiDebitiBanche As Boolean = True

            DT_SP = objPianoConti.Saldo_Conti_Patrimoniali(_dataInizioPat,
                                                           _dataFinePat,
                                                           False,
                                                           _gestContFlagConsideraSaldiInizialiPat,
                                                           _gestContDataInizio,
                                                           _piva, _anno, BILANCIO_PERSONALIZZATO,
                                                           "", "", SP_CONTO_IMPUTABILE_NOFILTRO,
                                                           0,
                                                           CONTO_UE_NOFILTRO, "",
                                                           SEZIONALE_NOFILTRO,
                                                           -1,
                                                           0,
                                                           "",
                                                           -1,
                                                           True,
                                                           Flag_AggiungiContoPadre_CreditiDebitiBanche,
                                                           "", "", "", "",
                                                           DT_Codifiche_Pat,
                                                           _objParametriServer,
                                                           flagNuoviArrotondamenti:=_nuoviArrotondamenti)


        Catch exc As Exception
            _logErrori &= "- Query Saldo_Conti_Patrimoniali: " & vbCrLf & exc.Message & vbCrLf
        End Try

        'Try

        If Not IsNothing(DT_SP) AndAlso DT_SP.Rows.Count > 0 Then

            '  Dim objRicXContiPat As New AgronicaCoreContabDAL.RicxConti_Patrimonio_R
            '    Dim DT_ContiPat As DataTable

            Dim Id_Riclassificazione_CreditiVersoClienti As String = ""
            Dim Id_Riclassificazione_DebitiVersoFornitori As String = ""
            Dim Id_Riclassificazione_DepositiBancariPostali As String = ""
            Dim Id_Riclassificazione_DenaroValoriInCassa As String = ""
            Dim Id_Riclassificazione_IvaACredito As String = ""
            Dim Id_Riclassificazione_IvaACredito_AcqIntra As String = ""
            Dim Id_Riclassificazione_IvaADebito As String = ""
            Dim Id_Riclassificazione_IvaADebito_AcqIntra As String = ""
            'Dim Id_Riclassificazione_ErarioRitenuteLavoroAutonomo As String
            'Dim Id_Riclassificazione_DebitiVsEnasarco As String


            objHLP.Recupera_IdRiclassificazione_ContiPatrimoniali(DT_Codifiche_Pat,
                                                                  Id_Riclassificazione_CreditiVersoClienti,
                                                                  Id_Riclassificazione_DebitiVersoFornitori,
                                                                  Id_Riclassificazione_DepositiBancariPostali,
                                                                  Id_Riclassificazione_DenaroValoriInCassa,
                                                                  Id_Riclassificazione_IvaACredito,
                                                                  Id_Riclassificazione_IvaACredito_AcqIntra,
                                                                  Id_Riclassificazione_IvaADebito,
                                                                  Id_Riclassificazione_IvaADebito_AcqIntra)



            Dim Saldo_ConFigli As Decimal = 0
            Dim Saldo_ConFigli_Dare As Decimal = 0
            Dim Saldo_ConFigli_Avere As Decimal = 0
            Dim Saldo As Decimal
            Dim Dr() As DataRow
            Dim Id_Riclassificazione, Dare_Avere, conto_descr_Padre As String

            'aggiungo le colonne che mi servono per il gruppo sul report
            DT_SP.Columns.Add("Id_riclassificazione_Padre", GetType(String))
            DT_SP.Columns.Add("conto_descr_Padre", GetType(String))

            'aggiungo la colonna col saldo totale conto e suoi sottoconti
            DT_SP.Columns.Add("Saldo_ConFigli", GetType(Decimal))
            DT_SP.Columns.Add("Saldo_ConFigli_Dare", GetType(Decimal))
            DT_SP.Columns.Add("Saldo_ConFigli_Avere", GetType(Decimal))

            For i = 0 To DT_SP.Rows.Count - 1

                'azzero ad ogni giro
                Saldo_ConFigli = 0

                'uso CodiceSplitGruppo invece di Id_Riclassificazione 
                'perchè per i conti che si agganciano ai contatti e alle banche, ho bisogno dell'id_riclassificazione + il codice del contatto/banca
                'Id_Riclassificazione = DT_SP.Rows(i).Item("Id_Riclassificazione")
                'tolgo il prefisso per la visualizzazione
                Id_Riclassificazione = Replace(DT_SP.Rows(i).Item("CodiceSplitGruppo"), "SP_", "")

                DT_SP.Rows(i).Item("Id_riclassificazione") = Id_Riclassificazione

                Dare_Avere = DT_SP.Rows(i).Item("Dare_Avere")

                'se si tratta dei conti automatici
                If (CStr(Id_Riclassificazione).StartsWith(Id_Riclassificazione_CreditiVersoClienti) = True OrElse
                        CStr(Id_Riclassificazione).StartsWith(Id_Riclassificazione_DebitiVersoFornitori) = True OrElse
                        CStr(Id_Riclassificazione).StartsWith(Id_Riclassificazione_DenaroValoriInCassa) = True OrElse
                        CStr(Id_Riclassificazione).StartsWith(Id_Riclassificazione_DepositiBancariPostali) = True) AndAlso
                            (Id_Riclassificazione <> Id_Riclassificazione_CreditiVersoClienti AndAlso
                             Id_Riclassificazione <> Id_Riclassificazione_DebitiVersoFornitori AndAlso
                             Id_Riclassificazione <> Id_Riclassificazione_DenaroValoriInCassa AndAlso
                             Id_Riclassificazione <> Id_Riclassificazione_DepositiBancariPostali) Then

                    'nel caso di crediti/debiti/banche
                    'essendoci il collegamento con i contatti e le risorse finanziarie
                    'se cerco per quell'id_riclassificazione (che contiene il codice del contatto, della banca) non troverò ovviamente niente
                    '(non esiste il conto fisico)
                    'e visto che non ci possono essere conti figli (il conto è foglia)
                    'Saldo_ConFigli = Saldo
                    Saldo_ConFigli = DT_SP.Rows(i).Item("Saldo")
                Else

                    'CASO PARTICOLARE DEL PIANO DEI CONTI GIAS
                    'essendoci i conti C.002.d-bis e C.002.d-ter 
                    'che non sono figli di C.002.d ma fratelli
                    'per il calcolo del saldoconfigli del conto C.002.d
                    'non posso utilizzare il like come per tutti
                    'devo farne uno dedicato
                    'NOTA DEL 28/01/2016:
                    'con il migra 387 gli id sono stati trasformati tutti in numerici (tranne la prima parte)
                    If Id_Riclassificazione = "C.002.d" Then
                        'i clienti con piano dei conti personalizzato
                        'non avranno mai questo caso

                        Dr = DT_SP.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%' AND Id_Riclassificazione not like '%-bis%' AND Id_Riclassificazione not like '%-ter%' AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")

                        If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                            For j = 0 To Dr.Length - 1
                                Saldo = Dr(j).Item("Saldo")
                                Saldo_ConFigli += Saldo
                                Saldo_ConFigli = ArrotondaVal_2(Saldo_ConFigli)
                            Next
                        Else
                            Saldo_ConFigli = 0
                        End If
                    Else
                        'attenzione, percentuale solo in fondo
                        'in modo da cercare tutti quelli che iniziano con quell'id_riclassificazione
                        'altrimenti, ad esempio: se filtro A.03
                        'mi trova anche A.01.a.03
                        'MODIFICA DEL 19/01/2015:
                        'collina dei poeti ha sforato i conti oltre la z, quindi con il filtro precedente i zonti za, zb, ecc venivano considerati figli di z invece che fratelli
                        'Dr = DT_SP.Select(" Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & "%' AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")
                        Dr = DT_SP.Select(" (Id_Riclassificazione LIKE '" & Agro_SQL_SaveText(Id_Riclassificazione) & ".%' OR Id_Riclassificazione = '" & Agro_SQL_SaveText(Id_Riclassificazione) & "') AND Dare_Avere = '" & Agro_SQL_SaveText(Dare_Avere) & "'    ")

                        If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                            For j = 0 To Dr.Length - 1
                                Saldo = Dr(j).Item("Saldo")
                                Saldo_ConFigli += Saldo
                                Saldo_ConFigli = ArrotondaVal_2(Saldo_ConFigli)
                            Next
                        Else
                            Saldo_ConFigli = 0
                        End If
                    End If 'conto specifico
                End If ' conti automatici o no

                'nel S.P. in avere ci sono le passività
                'la query ritorna un valore negativo
                'ma nel bilancio non devono esserci meno, quindi moltiplico * -1
                'se dopo il * -1 ci sono negativi, allora sono errori dell'utente
                If Dare_Avere.ToUpper = "A" Then
                    Saldo_ConFigli = Saldo_ConFigli * -1
                    Saldo_ConFigli = ArrotondaVal_2(Saldo_ConFigli)

                    DT_SP.Rows(i).Item("Saldo_ConFigli_Avere") = Saldo_ConFigli
                    DT_SP.Rows(i).Item("Saldo_ConFigli_Dare") = 0
                Else
                    DT_SP.Rows(i).Item("Saldo_ConFigli_Dare") = Saldo_ConFigli
                    DT_SP.Rows(i).Item("Saldo_ConFigli_Avere") = 0
                End If


                DT_SP.Rows(i).Item("Saldo_ConFigli") = Saldo_ConFigli

                DT_SP.Rows(i).Item("Id_riclassificazione_Padre") = Left(Id_Riclassificazione, 1)

                'non serve più conto_pat_descr_padre -> non mi serve il nome del gruppo
                conto_descr_Padre = ""
                DT_SP.Rows(i).Item("conto_descr_Padre") = conto_descr_Padre

            Next ' popolamento Saldo_ConFigli
            '//////////////////////////////////////////////////////


            Dim DrAtt As DS_SP_Attivita.DS_SP_AttivitaRow
            Dim DrPass As DS_SP_Passivita.DS_SP_PassivitaRow

            For i = 0 To DT_SP.Rows.Count - 1

                '///////////////////////////////////////////////////
                '1)
                'in questa parte vengono gestite tutte le righe
                '///////////////////////////////////////////////////

                Flag_UE = DT_SP.Rows(i).Item("Flag_UE")
                Dare_Avere = DT_SP.Rows(i).Item("Dare_Avere")

                Select Case Dare_Avere.ToUpper

                    Case "D"
                        If CStr(DT_SP.Rows(i).Item("Id_Riclassificazione")).Length = 1 Then
                            Totale_Attivita += DT_SP.Rows(i).Item("Saldo_ConFigli")
                        End If
                    Case "A"
                        If CStr(DT_SP.Rows(i).Item("Id_Riclassificazione")).Length = 1 Then
                            Totale_Passivita += DT_SP.Rows(i).Item("Saldo_ConFigli")
                        End If
                End Select



                '///////////////////////////////////////////////////
                '2)
                'in questa parte vengono gestite solo le righe che devono essere visualizzate
                'e la valorizzazione dei campi del dataset
                '///////////////////////////////////////////////////

                If Sintetico0_Analitico1 = 0 AndAlso Flag_UE = CONTO_NONUE Then
                    '----------------------------------------------
                    'è stato scelto il bilancio sintetico e, 
                    'visto che il conto non è UE (quindi personalizzato)
                    'non lo inserisco nel dataset
                    '----------------------------------------------
                    debug = True
                Else

                    With DT_SP.Rows(i)

                        Select Case .Item("Dare_Avere")

                            Case "D"
                                '-----------------------------------
                                '-------- ATTIVITA' - DARE ---------
                                '-----------------------------------                                  

                                DrAtt = DSAttivita._DS_SP_Attivita.NewRow

                                DrAtt.Id_Riclassificazione_Padre = .Item("Id_Riclassificazione_Padre")
                                DrAtt.conto_descr_Padre = .Item("conto_descr_Padre")

                                DrAtt.Cod_Conto = .Item("Cod_Conto_Pat")
                                DrAtt.Id_Riclassificazione = .Item("Id_Riclassificazione")
                                DrAtt.Dare_Avere = .Item("Dare_Avere")
                                DrAtt.Conto_Descr = .Item("Conto_Pat_Descr")

                                DrAtt.Saldo = 0
                                DrAtt.SaldoTotale = DT_SP.Rows(i).Item("Saldo_ConFigli")
                                DrAtt.Saldo_Str = Format(CDec(DT_SP.Rows(i).Item("Saldo_ConFigli")), "#,###,##0.00")

                                'questi saldi sono in positivo e conteggiano i saldi dei figli
                                'vanno visualizzati per riga ma non conteggiati nei totali
                                Saldo_ConFigli_Dare = DT_SP.Rows(i).Item("Saldo_ConFigli_Dare")
                                Saldo_ConFigli_Avere = DT_SP.Rows(i).Item("Saldo_ConFigli_Avere")

                                'questi sono gli importi di riga
                                DrAtt.Saldo_Dare = Format(Saldo_ConFigli_Dare, "#,###,##0.00")
                                DrAtt.Saldo_Avere = Format(Saldo_ConFigli_Avere, "#,###,##0.00")

                                DSAttivita._DS_SP_Attivita.Rows.Add(DrAtt)

                            Case "A"
                                '--------------------------------------
                                '-------- PASSIVITA' - AVERE ---------
                                '--------------------------------------                                   

                                DrPass = DSPassivita._DS_SP_Passivita.NewRow

                                DrPass.Id_Riclassificazione_Padre = .Item("Id_Riclassificazione_Padre")
                                DrPass.conto_descr_Padre = .Item("conto_descr_Padre")

                                DrPass.Cod_Conto = .Item("Cod_Conto_Pat")
                                DrPass.Id_Riclassificazione = .Item("Id_Riclassificazione")
                                DrPass.Dare_Avere = .Item("Dare_Avere")
                                DrPass.Conto_Descr = .Item("Conto_Pat_Descr")

                                DrPass.Saldo = 0
                                DrPass.SaldoTotale = DT_SP.Rows(i).Item("Saldo_ConFigli")
                                DrPass.Saldo_Str = Format(CDec(DT_SP.Rows(i).Item("Saldo_ConFigli")), "#,###,##0.00")

                                ''questi saldi sono in positivo e conteggiano i saldi dei figli
                                ''vanno visualizzati per riga ma non conteggiati nei totali
                                'Saldo_ConFigli_Dare = DT_Eco.Rows(i).Item("Saldo_ConFigli_Dare")
                                'Saldo_ConFigli_Avere = DT_Eco.Rows(i).Item("Saldo_ConFigli_Avere")

                                ''questi sono gli importi di riga
                                'Dr_CR.Saldo_Dare = Format(Saldo_ConFigli_Dare, "#,###,##0.00")
                                'Dr_CR.Saldo_Avere = Format(Saldo_ConFigli_Avere, "#,###,##0.00")

                                DSPassivita._DS_SP_Passivita.Rows.Add(DrPass)

                        End Select

                    End With

                End If

            Next 'dt_sp

        End If

        DSPassivita.AcceptChanges()
        DSAttivita.AcceptChanges()


        'Catch ex As Exception
        '    Log_Errori &= "- Caricamento dataset: " & vbCrLf & ex.Message & vbCrLf
        'End Try


    End Sub

    '#####################################################################
    Private Function DecidiSeStamparePat(saldoConFigli As Decimal,
                                         idRiclCreditiVersoClienti As String,
                                         idRiclDebitiVersoFornitori As String,
                                         idRiclDepositiBancariPostali As String,
                                         idRiclDenaroValoriInCassa As String,
                                         idRiclassificazione As String,
                                         flagDareAvere As String,
                                         listBlockAtt As List(Of String),
                                         listBlockPas As List(Of String),
                                         flagNoSaldo0 As Boolean,
                                         flagUe As Integer
                                         ) As Boolean

        Dim flagStampaRiga As Boolean = True

        If (flagDareAvere = "D" AndAlso listBlockAtt.Contains(idRiclassificazione)) OrElse
           (flagDareAvere = "A" AndAlso listBlockPas.Contains(idRiclassificazione)) Then
            '  Giulia, 15/09/2016 15.48.12: questi sono conti che è necessario che siano stampati in qualunque caso
            flagStampaRiga = True
        Else
            If Sintetico0_Analitico1 = 0 AndAlso idRiclassificazione.Length > 5 Then
                If idRiclassificazione.StartsWith(idRiclDenaroValoriInCassa) OrElse
                   idRiclassificazione.StartsWith(idRiclDepositiBancariPostali) Then
                    flagStampaRiga = True
                Else
                    flagStampaRiga = False
                End If
            End If


            If idRiclassificazione.StartsWith(idRiclCreditiVersoClienti) = True AndAlso
               idRiclassificazione <> idRiclCreditiVersoClienti Then

                flagStampaRiga = _spCreditiClienti

            ElseIf idRiclassificazione.StartsWith(idRiclDepositiBancariPostali) = True AndAlso
                   idRiclassificazione <> idRiclDepositiBancariPostali Then

                flagStampaRiga = _spBanche

            ElseIf idRiclassificazione.StartsWith(idRiclDenaroValoriInCassa) = True AndAlso
                   idRiclassificazione <> idRiclDenaroValoriInCassa Then

                flagStampaRiga = _spCasse

            ElseIf idRiclassificazione.StartsWith(idRiclDebitiVersoFornitori) = True AndAlso
                   idRiclassificazione <> idRiclDebitiVersoFornitori Then

                flagStampaRiga = _spDebitiFornitori
            End If


            If flagNoSaldo0 = True AndAlso ArrotondaVal_2(saldoConFigli) = 0 Then
                flagStampaRiga = False
            End If

            If _contiUeTutti = CONTO_UE AndAlso flagUe = CONTO_NONUE Then
                flagStampaRiga = False
            End If
        End If

        Return flagStampaRiga

    End Function

    '#####################################################################
    Private Function DecidiSeStampareEco(ByVal idRiclassificazione As String,
                                         ByVal listBlock As List(Of String),
                                         ByVal flagNoSaldo0 As Boolean,
                                         ByVal flagUe As Integer,
                                         ByVal saldoConFigli As Decimal) As Boolean

        Dim flagStampaRiga As Boolean = True


        If listBlock.Contains(idRiclassificazione) Then
            '  Giulia, 15/09/2016 15.48.12: questi sono conti che è necessario che siano stampati in qualunque caso
            flagStampaRiga = True
        Else
            If Sintetico0_Analitico1 = 0 AndAlso idRiclassificazione.Length > 5 Then
                flagStampaRiga = False
            End If

            If flagNoSaldo0 = True AndAlso ArrotondaVal_2(saldoConFigli) = 0 Then
                flagStampaRiga = False
            End If

            If _contiUeTutti = CONTO_UE AndAlso flagUe = CONTO_NONUE Then
                flagStampaRiga = False
            End If
        End If

        Return flagStampaRiga

    End Function

    '#####################################################################
    Private Sub Stampa_Bilancio(ByRef DSBilancio As DS_Bilancio,
                                ByVal DSAttivita As DS_SP_Attivita,
                                ByVal DSPassivita As DS_SP_Passivita,
                                ByRef DSCostiRicavi As DS_CE_CostiRicavi)

        Dim Parametro_Totale_Attivita As String = "" 'sia nel report che sottoreport
        Dim Parametro_Totale_Passivita As String = "" 'sia nel report che sottoreport

        'CONTO ECONOMICO
        Dim Parametro_Tot_A_B As String = ""
        Dim Parametro_Tot_C15C16_C17 As String = ""
        Dim Parametro_TOT_D18_D19 As String = ""
        Dim Parametro_Tot_E20_E21 As String = ""
        Dim Parametro_Risultato_Prima_Imposte As String = ""
        Dim Parametro_Tot_Imposte As String = ""
        Dim Parametro_Tot_Utile As String = ""

        Dim Parametro_CE_Totale_Costi As String = ""
        Dim Parametro_CE_Totale_Ricavi As String = ""
        Dim Parametro_CE_Pareggio_Dare As String = ""
        Dim Parametro_CE_Pareggio_Avere As String = ""
        'Dim Parametro_CE_Perdita As String = ""
        'Dim Parametro_CE_Utile As String = ""

        Dim Parametro_Lbl_Perdita As String = ""
        Dim Parametro_Lbl_Utile As String = ""
        Dim Utile_Esercizio As Decimal = 0
        Dim Perdita_Esercizio As Decimal = 0

        'STATO PATRIMONIALE
        Dim Parametro_Perdita_Esercizio As String = ""
        Dim Parametro_Utile_Esercizio As String = ""
        Dim Parametro_Pareggio_Dare As String = ""
        Dim Parametro_Pareggio_Avere As String = ""

        Dim Totale_Pareggio_Dare As Decimal = 0
        Dim Totale_Pareggio_Avere As Decimal = 0
        Dim Totale_Attivita As Decimal = 0
        Dim Totale_Passivita As Decimal = 0

        'Dim Flag_UE As Integer
        Dim objPianoConti As New AgronicaCoreStampeDAL.PianoConti


        Try

            Dim DT_Intestazione As DataTable
            Dim str_filtro As String = "Filtro: "

            'Select Case Conti_UE_Tutti
            '    Case 1
            '        str_filtro &= " Solo Conti UE "
            '    Case 0
            '        str_filtro &= " Conti UE e personalizzati "
            'End Select

            '        Select Case Conti_0Movimentati_1Tutti
            '            Case 0
            '                str_filtro &= " - Solo conti movimentati "
            '            Case 1
            '                str_filtro &= " - Conti movimentati e non "
            '        End Select
            '        'Select Case Conti_0ImponibileNoZero_1Tutti
            '        '    Case 0
            '        '        str_filtro &= " - Solo movimenti con imponibile <> 0 "
            '        '    Case 1
            '        '        str_filtro &= " - Tutti i movimenti "
            '        'End Select

            If _filtroConti <> "" Then
                str_filtro &= " - Conti selezionati: " & _filtroConti
            End If

            If str_filtro <> "" Then
                CType(_rptBilancio.Section1.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_filtro
            End If

            If _dataInizio < _gestContDataInizio AndAlso _dataFine >= _gestContDataInizio Then
                'se la data inizio è precedente alla data di inizio gestione contabile e la data di fine è >=
                '-> allora la data di inizio diventa la data di inizio gestione contabile
                _dataInizio = _gestContDataInizio
            End If

            _annoMin = CDate(_dataInizio).Year

            Dim str_inttemp As String = ""
            'If Anno <> 0 Then
            '    str_inttemp = "Anno contabile " & CStr(Anno)
            'Else
            If _dataInizio <> AGRODATAINIZIO AndAlso _dataFine <> AGRODATAFINE Then
                str_inttemp = "Periodo dal " & _dataInizio & " al " & _dataFine

                If _esercizio <> 0 Then
                    str_inttemp = "Periodo dal " & _dataInizioEco & " al " & _dataFine
                End If

            End If
            'End If

            CType(_rptBilancio.Section1.ReportObjects("TxtAnnoContabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CStr(_anno)
            CType(_rptBilancio.Section1.ReportObjects("TxtIntervalloTemporale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = str_inttemp

            Dim objIntest As New AgronicaCoreStampeDAL.DocContab
            DT_Intestazione = objIntest.DatiIntestazioneImpresa(_piva, Nothing, "", "", _objParametriServer)

            If Not IsNothing(DT_Intestazione) AndAlso DT_Intestazione.Rows.Count > 0 Then
                CType(_rptBilancio.Section1.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Piva")
                CType(_rptBilancio.Section1.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Codice_Fiscale")
                CType(_rptBilancio.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("rag_soc")
                CType(_rptBilancio.Section1.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("ind_impresa") & " " &
                                                                                                                                     DT_Intestazione.Rows(0).Item("CAP") & " " &
                                                                                                                                     DT_Intestazione.Rows(0).Item("frz_des") & " - " &
                                                                                                                                     DT_Intestazione.Rows(0).Item("LOCALITA") &
                                                                                                                                     " (" & DT_Intestazione.Rows(0).Item("COMUNI_PROV") & ") "
            End If

        Catch exc As Exception
            _logErrori &= "- Intestazione report: " & vbCrLf & exc.Message & vbCrLf
        End Try


        '########################################################################
        '########################################################################

        'BILANCIO CIVILISTICO - CONTO ECONOMICO - FORMATO EUROPEO
        _rptBilancio.Section6.SectionFormat.EnableSuppress = True
        _rptBilancio.Section7.SectionFormat.EnableSuppress = True
        _rptBilancio.Section8.SectionFormat.EnableSuppress = True
        _rptBilancio.Section9.SectionFormat.EnableSuppress = True
        _rptBilancio.Section10.SectionFormat.EnableSuppress = True
        _rptBilancio.Section11.SectionFormat.EnableSuppress = True
        _rptBilancio.Section12.SectionFormat.EnableSuppress = True
        _rptBilancio.Section13.SectionFormat.EnableSuppress = True
        _rptBilancio.Section14.SectionFormat.EnableSuppress = True

        'BILANCIO CIVILISTICO - CONTO ECONOMICO - COSTI RICAVI
        _rptBilancio.DetailSection4.SectionFormat.EnableSuppress = True
        _rptBilancio.DetailSection5.SectionFormat.EnableSuppress = True

        'BILANCIO CIVILISTICO - STATO PATRIMONIALE
        'ATTIVITA'
        _rptBilancio.DetailSection1.SectionFormat.EnableSuppress = True
        'PASSIVITA'
        _rptBilancio.DetailSection2.SectionFormat.EnableSuppress = True

        'ATTIVITA' DARE-AVERE
        _rptBilancio.DetailSection6.SectionFormat.EnableSuppress = True

        'Riepilogo Stato Patrimoniale
        _rptBilancio.DetailSection3.SectionFormat.EnableSuppress = True

        Try

            '------------------------------------------------------------------
            '----------------------- CONTO ECONOMICO --------------------------
            '------------------------------------------------------------------
            If _chkCE = True Then
                If CE_Layout_0Europeo_1CostiRicavi = 0 Then
                    Stampa_ContoEconomico_LayoutEuropeo(DSBilancio, objPianoConti,
                                                        Parametro_Tot_A_B,
                                                        Parametro_Tot_C15C16_C17,
                                                        Parametro_TOT_D18_D19,
                                                        Parametro_Tot_E20_E21,
                                                        Parametro_Risultato_Prima_Imposte,
                                                        Parametro_Tot_Imposte,
                                                        Parametro_Tot_Utile,
                                                        Parametro_Lbl_Perdita,
                                                        Parametro_Lbl_Utile,
                                                        Utile_Esercizio,
                                                        Perdita_Esercizio)

                ElseIf CE_Layout_0Europeo_1CostiRicavi = 1 Then

                    Stampa_ContoEconomico_CostiRicavi(DSCostiRicavi, objPianoConti,
                                                      Parametro_Lbl_Perdita,
                                                      Parametro_Lbl_Utile,
                                                      Utile_Esercizio,
                                                      Perdita_Esercizio,
                                                      Parametro_CE_Totale_Costi,
                                                      Parametro_CE_Totale_Ricavi,
                                                      Parametro_CE_Pareggio_Dare,
                                                      Parametro_CE_Pareggio_Avere)

                End If

            End If

            '------------------------------------------------------------------
            '----------------------- STATO PATRIMONIALE -----------------------
            '------------------------------------------------------------------
            If _chkSP = True Then
                Stampa_StatoPatrimoniale_SaldoUnico(DSAttivita, DSPassivita,
                                                    objPianoConti,
                                                    Totale_Attivita,
                                                    Totale_Passivita)

                'Stampa_StatoPatrimoniale_DareAvere(DSAttivita, DSPassivita,
                '                                   objPianoConti,
                '                                   Totale_Attivita,
                '                                   Totale_Passivita)

            End If


        Catch ex As Exception
            _logErrori &= "- Elaborazione Bilancio: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            _rptSezioneA.SetDataSource(DSBilancio)
            _rptSezioneB.SetDataSource(DSBilancio)
            _rptSezioneC.SetDataSource(DSBilancio)
            _rptSezioneD.SetDataSource(DSBilancio)
            _rptSezioneE.SetDataSource(DSBilancio)

            _rptAttivita.SetDataSource(DSAttivita)
            _rptAttivitaDareAvere.SetDataSource(DSAttivita)
            _rptPassivita.SetDataSource(DSPassivita)

            _rptCostiRicavi.SetDataSource(DSCostiRicavi)

        Catch exc As Exception
            _logErrori &= "- Aggancio dataset: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Try

            _rptBilancio.OpenSubreport("Rpt_BilancioSezioneA.rpt").SetDataSource(DSBilancio)
            _rptBilancio.OpenSubreport("Rpt_BilancioSezioneB.rpt").SetDataSource(DSBilancio)
            _rptBilancio.OpenSubreport("Rpt_BilancioSezioneC.rpt").SetDataSource(DSBilancio)
            _rptBilancio.OpenSubreport("Rpt_BilancioSezioneD.rpt").SetDataSource(DSBilancio)
            _rptBilancio.OpenSubreport("Rpt_BilancioSezioneE.rpt").SetDataSource(DSBilancio)
            _rptBilancio.OpenSubreport("Rpt_SP_Attivita.rpt").SetDataSource(DSAttivita)
            _rptBilancio.OpenSubreport("Rpt_SPAttivita_DareAvere.rpt").SetDataSource(DSAttivita)
            _rptBilancio.OpenSubreport("Rpt_SP_Passivita.rpt").SetDataSource(DSPassivita)
            _rptBilancio.OpenSubreport("Rpt_CE_CostiRicavi.rpt").SetDataSource(DSCostiRicavi)

        Catch exc As Exception
            _logErrori &= "- OpenSubreport: " & vbCrLf & exc.Message & vbCrLf
        End Try

        Try

            'STATO PATRIMONIALE
            Totale_Pareggio_Dare = Totale_Attivita + Perdita_Esercizio
            Totale_Pareggio_Avere = Totale_Passivita + Utile_Esercizio

            Parametro_Perdita_Esercizio = Format(Perdita_Esercizio, "#,###,##0.00")
            Parametro_Utile_Esercizio = Format(Utile_Esercizio, "#,###,##0.00")
            Parametro_Pareggio_Dare = Format(Totale_Pareggio_Dare, "#,###,##0.00")
            Parametro_Pareggio_Avere = Format(Totale_Pareggio_Avere, "#,###,##0.00")

            Parametro_Totale_Attivita = Format(Totale_Attivita, "#,###,##0.00")
            Parametro_Totale_Passivita = Format(Totale_Passivita, "#,###,##0.00")


            _rptBilancio.SetParameterValue("Totale_Passivita", Parametro_Totale_Passivita)
            _rptBilancio.SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita)
            _rptBilancio.SetParameterValue("Lbl_Perdita", Parametro_Lbl_Perdita)
            _rptBilancio.SetParameterValue("Lbl_Utile", Parametro_Lbl_Utile)
            _rptBilancio.SetParameterValue("Utile", Parametro_Utile_Esercizio)
            _rptBilancio.SetParameterValue("Perdita", Parametro_Perdita_Esercizio)
            _rptBilancio.SetParameterValue("Totale_Pareggio_Dare", Parametro_Pareggio_Dare)
            _rptBilancio.SetParameterValue("Totale_Pareggio_Avere", Parametro_Pareggio_Avere)

        Catch ex As Exception
            _logErrori &= "- impostazione parametri totali STATO PATRIMONIALE: " & vbCrLf & ex.Message & vbCrLf
        End Try


        Try

            'CONTO ECONOMICO
            If CE_Layout_0Europeo_1CostiRicavi = 0 Then

                _rptBilancio.SetParameterValue("Tot_A_B", Parametro_Tot_A_B)
                _rptBilancio.SetParameterValue("Tot_C15C16_C17", Parametro_Tot_C15C16_C17)
                _rptBilancio.SetParameterValue("TOT_D18_D19", Parametro_TOT_D18_D19)
                _rptBilancio.SetParameterValue("Tot_E20_E21", Parametro_Tot_E20_E21)
                _rptBilancio.SetParameterValue("Risultato_Prima_Imposte", Parametro_Risultato_Prima_Imposte)
                _rptBilancio.SetParameterValue("Tot_Imposte", Parametro_Tot_Imposte)
                _rptBilancio.SetParameterValue("Tot_Utile", Parametro_Tot_Utile)

                _rptBilancio.SetParameterValue("CE_Totale_Costi", "")
                _rptBilancio.SetParameterValue("CE_Totale_Ricavi", "")
                _rptBilancio.SetParameterValue("CE_Perdita", "")
                _rptBilancio.SetParameterValue("CE_Utile", "")
                _rptBilancio.SetParameterValue("CE_Pareggio_Dare", "")
                _rptBilancio.SetParameterValue("CE_Pareggio_Avere", "")

            ElseIf CE_Layout_0Europeo_1CostiRicavi = 1 Then

                _rptBilancio.SetParameterValue("Tot_A_B", "")
                _rptBilancio.SetParameterValue("Tot_C15C16_C17", "")
                _rptBilancio.SetParameterValue("TOT_D18_D19", "")
                _rptBilancio.SetParameterValue("Tot_E20_E21", "")
                _rptBilancio.SetParameterValue("Risultato_Prima_Imposte", "")
                _rptBilancio.SetParameterValue("Tot_Imposte", "")
                _rptBilancio.SetParameterValue("Tot_Utile", "")

                _rptBilancio.SetParameterValue("CE_Totale_Costi", Parametro_CE_Totale_Costi)
                _rptBilancio.SetParameterValue("CE_Totale_Ricavi", Parametro_CE_Totale_Ricavi)
                _rptBilancio.SetParameterValue("CE_Perdita", Parametro_Perdita_Esercizio)
                _rptBilancio.SetParameterValue("CE_Utile", Parametro_Utile_Esercizio)
                _rptBilancio.SetParameterValue("CE_Pareggio_Dare", Parametro_CE_Pareggio_Dare)
                _rptBilancio.SetParameterValue("CE_Pareggio_Avere", Parametro_CE_Pareggio_Avere)

            End If
        Catch ex As Exception
            _logErrori &= "- impostazione parametri totali CONTO ECONOMICO: " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try

            'SOTTOREPORT ATTIVITA'
            '            Parametro_Totale_Attivita = Format(Totale_Attivita, "#,###,##0.00")
            _rptAttivita.SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita)
            _rptAttivitaDareAvere.SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita)

            'uso il nome del file
            _rptBilancio.SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita, "Rpt_SP_Attivita.rpt")
            _rptBilancio.SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita, "Rpt_SPAttivita_DareAvere.rpt")

            ''uso il nome del sottoreport
            '_rptBilancio.SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita, "SP_Attivita")

            'questo genera eccezione
            '  _rptBilancio.OpenSubreport("Rpt_SP_Attivita.rpt").SetParameterValue("Totale_Attivita", Parametro_Totale_Attivita)

        Catch ex As Exception
            _logErrori &= "- impostazione parametri sul sottoreport ATTIVITA': " & vbCrLf & ex.Message & vbCrLf
        End Try

        Try
            'SOTTOREPORT PASSIVITA'
            'Parametro_Totale_Passivita = Format(Totale_Passivita, "#,###,##0.00")

            'uso il nome del file
            _rptBilancio.SetParameterValue("Totale_Passivita", Parametro_Totale_Passivita, "Rpt_SP_Passivita.rpt")


        Catch ex As Exception
            _logErrori &= "- impostazione parametri sul sottoreport PASSIVITA': " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class
