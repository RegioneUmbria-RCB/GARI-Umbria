Option Strict Off

Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.ConnessioniTransazioni
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports CrystalDecisions.Shared

Public Class Scheda_NPK
    Inherits System.Web.UI.Page


#Region "Dichiarazione Variabili"

    '----- Variabili globali nella pagina
    Dim Qs_Key As String
    Dim Qs_AnalisiTestataCod As String
    Dim Qs_Operazione As String
    Dim Qs_Piva As String
    Dim Qs_PCTestataCod As String
    Dim Qs_Anteprima As String

    Dim xChiave As String
    Dim xTipoNodo As enum_TipoNodo

    Dim xPiva As String
    Dim xSa_Cod As Integer
    Dim xCampo_Cod As Integer
    Dim xAppezza As Integer
    Dim xID_Imp As Integer
    Dim xPart_Cod As Integer
    Dim xFabbricato_Cod As Integer
    Dim xCodFiscale As String

    Dim xCodProvincia As String
    Dim xCodComune As String
    Dim xSezione As String
    Dim xFoglio As Integer
    Dim xNumero As Integer
    Dim xSubalterno As String

    Dim xProgetto_Cod As Integer

    Dim xPianoConcimazione_Testata_Cod As Integer

    '----- objParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim Log_Errori As String = ""
    Dim Nome_Documento As String = "Scheda_NPK"

    '----- Gestione Report
    Private rptSchedaNPK As RPT_Scheda_NPK
    Private rptFooterLogo As FooterLogo

    Dim personalizzazioniGraficheCliente As PersonalizzazioniGraficheCliente = Nothing

#End Region
    '###########################################################################################################
    Private Sub Scheda_NPK_Init(sender As Object, e As System.EventArgs) Handles Me.Init

        ' rptSchedaNPK = New RPT_Scheda_NPK

    End Sub

    '###########################################################################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '##############################################################
        '#####  Verifico Credenziali di Accesso  ######################
        '##############################################################

        '----- Verifico che l'utente sia autenticato

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        '----- Verifico se l'utente dispone dei permessi per la visualizzazione della pagina

        Dim UtenteAbilitato As Boolean

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                            Session("ASG_Utente_Username"),
                            Session("ASG_IdServizio"),
                            TipiEnumerativi.enum_Security_Attivita.SupportoDecisioni_PianoConcimazione,
                            TipiEnumerativi.enum_Security_Operazione.Lettura,
                            Date.Now,
                            "",
                            objParametri_Utenti)

        If UtenteAbilitato = False Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
            'AgroMsgBox("Non si dispongono dei permessi necessari per la gestione dei piani di concimazione", Page)
            'Exit Sub
        End If

        '######################################################################################################################
        '######################################################################################################################

        Session.Timeout = 180

        '##############################################################
        '#####  Recupero le variabili                        ##########
        '##############################################################

        If Not IsNothing(Request.QueryString("t")) Then

            Qs_AnalisiTestataCod = Stringa_Decodifica(Request.QueryString("t").ToString,
                                                      AgroKey_EncoderDecoder,
                                                      Server)
        Else
            Qs_AnalisiTestataCod = ""
        End If

        If Not IsNothing(Request.QueryString("n")) Then
            'in caso di Inserimento è il nodo padre, in caso di modifica è il nodo "piano concimazione"
            Qs_Key = Stringa_Decodifica(Request.QueryString("n").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)
        Else
            Qs_Key = ""
        End If

        Qs_Operazione = Stringa_Decodifica(Request.QueryString("o").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Session("PartitaIVA") = Qs_Piva

        Qs_PCTestataCod = Stringa_Decodifica(Request.QueryString("q").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)

        If Not IsNothing(Request.QueryString("anteprima")) Then

            Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString,
                                                      AgroKey_EncoderDecoder,
                                                      Server)
        Else
            Qs_Anteprima = "0"
        End If

        personalizzazioniGraficheCliente = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        Dim Dt As New DataTable
        Dim Rag_Soc As String = ""
        Dim Sa_Nome As String = ""
        Dim strErr As String = ""
        Dim regolamento As AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        '  Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            rptSchedaNPK = New RPT_Scheda_NPK
            rptFooterLogo = New FooterLogo

            Dim Veg_Cod As Integer = 0
            Dim Grfi_Cod As Integer = 0
            Dim Fase_Cod As Integer = 0
            Dim Sup_Tot As Double = 0
            Dim N_Ammesso As Decimal = 0
            Dim P_Ammesso As Decimal = 0
            Dim K_Ammesso As Decimal = 0
            Dim Resa As Decimal = 0

            Dim Mas As Decimal = -1
            Dim MasSalvato As Boolean = False

            Try
                Dati_Piano(Qs_Piva, 0, Qs_PCTestataCod, regolamento, Veg_Cod, Grfi_Cod, Fase_Cod, Resa, Sup_Tot, Sa_Nome, N_Ammesso, P_Ammesso, K_Ammesso, Mas, MasSalvato, objParametri_Server, rptSchedaNPK)
            Catch ex As Exception
                Log_Errori += "- Dati_Piano: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim DsIncrementiN As New DS_FattoriCorrettivi
            Dim DsDecrementiN As New DS_FattoriCorrettivi
            Dim DsIncrementiP As New DS_FattoriCorrettivi
            Dim DsDecrementiP As New DS_FattoriCorrettivi
            Dim DsIncrementiK As New DS_FattoriCorrettivi
            Dim DsDecrementiK As New DS_FattoriCorrettivi
            Dim DS_LogoFooter As New DS_LogoFooter

            'carico i dati nei datatable 
            Dim Tot As Double = 0

            If Veg_Cod <> 0 Then

                Try

                    Dim objFattoriSalvati As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_R
                    Dim DtSel As DataTable = objFattoriSalvati.Leggi(regolamento,
                                                             Qs_PCTestataCod,
                                                             0,
                                                             "",
                                                             "",
                                                             objParametri_Server)


                    'publicCarica_DsFattori(DsIncrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Incremento", Tot, Qs_PCTestataCod, objParametri_Server)
                    publicCarica_DsFattori(DsIncrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Incremento", Tot, DtSel, objParametri_Server)
                    CType(rptSchedaNPK.Section1.ReportObjects("TextTotIncrementi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Incrementi: " & Tot.ToString

                    Tot = 0
                    'publicCarica_DsFattori(DsDecrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Decremento", Tot, Qs_PCTestataCod, objParametri_Server)
                    publicCarica_DsFattori(DsDecrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Decremento", Tot, DtSel, objParametri_Server)
                    CType(rptSchedaNPK.Section1.ReportObjects("TextTotDecrementi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Decrementi: " & Tot.ToString

                    Tot = 0
                    'publicCarica_DsFattori(DsIncrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Incremento", Tot, Qs_PCTestataCod, objParametri_Server)
                    publicCarica_DsFattori(DsIncrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Incremento", Tot, DtSel, objParametri_Server)
                    CType(rptSchedaNPK.Section5.ReportObjects("TextTotIncrementiP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Incrementi: " & Tot.ToString

                    Tot = 0
                    'publicCarica_DsFattori(DsDecrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Decremento", Tot, Qs_PCTestataCod, objParametri_Server)
                    publicCarica_DsFattori(DsDecrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Decremento", Tot, DtSel, objParametri_Server)
                    CType(rptSchedaNPK.Section5.ReportObjects("TextTotDecrementiP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Decrementi: " & Tot.ToString

                    Tot = 0
                    'publicCarica_DsFattori(DsIncrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Incremento", Tot, Qs_PCTestataCod, objParametri_Server)
                    publicCarica_DsFattori(DsIncrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Incremento", Tot, DtSel, objParametri_Server)
                    CType(rptSchedaNPK.Section3.ReportObjects("TextTotIncrementiK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Incrementi: " & Tot.ToString

                    Tot = 0
                    'publicCarica_DsFattori(DsDecrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Decremento", Tot, Qs_PCTestataCod, objParametri_Server)
                    publicCarica_DsFattori(DsDecrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Decremento", Tot, DtSel, objParametri_Server)
                    CType(rptSchedaNPK.Section3.ReportObjects("TextTotDecrementiK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Decrementi: " & Tot.ToString

                    'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
                    'If personalizzazioniGraficheCliente IsNot Nothing Then
                    '    rptSchedaNPK.Section5.ReportObjects("Text9").ObjectFormat.EnableSuppress = True
                    '    rptSchedaNPK.Section5.ReportObjects("Picture3").ObjectFormat.EnableSuppress = True
                    '    rptSchedaNPK.Section5.ReportObjects("Text1").ObjectFormat.EnableSuppress = True
                    'End If

                    rptSchedaNPK.OpenSubreport("RPT_Incrementi_N.rpt").SetDataSource(DsIncrementiN)
                    rptSchedaNPK.OpenSubreport("RPT_Decrementi_N.rpt").SetDataSource(DsDecrementiN)

                    Dim FattoreDes As String = ""
                    Dim DoseSel As String
                    Dim dbl As Double

                    dbl = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "N", "Standard", regolamento, DtSel, FattoreDes)
                    CType(rptSchedaNPK.Section1.ReportObjects("TextDoseStd"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose Standard: " & dbl.ToString

                    Dim strFattoreCorrettivo As String = ""

                    If MasSalvato = False Then
                        Mas = publicCaricaMAS(Veg_Cod, Grfi_Cod, regolamento, Resa, strFattoreCorrettivo, objParametri_Server)
                    End If

                    If strFattoreCorrettivo <> "" Then
                        If Mas >= 0 Then
                            CType(rptSchedaNPK.Section1.ReportObjects("TextLimiteMas"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS *: " & Mas.ToString
                        Else
                            CType(rptSchedaNPK.Section1.ReportObjects("TextLimiteMas"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS *: n.d."
                        End If
                        CType(rptSchedaNPK.Section1.ReportObjects("TextLimiteMasNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "* " & strFattoreCorrettivo
                    Else
                        If Mas >= 0 Then
                            CType(rptSchedaNPK.Section1.ReportObjects("TextLimiteMas"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS: " & Mas.ToString
                        Else
                            CType(rptSchedaNPK.Section1.ReportObjects("TextLimiteMas"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS: n.d."
                        End If
                        CType(rptSchedaNPK.Section1.ReportObjects("TextLimiteMasNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                    End If

                    dbl = publicCaricaMaxIncrementi(Veg_Cod, Grfi_Cod, "N", "Incremento", regolamento)
                    CType(rptSchedaNPK.Section1.ReportObjects("TextMaxIncrementi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Max Incrementi: " & dbl.ToString

                    DoseSel = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "P", "Standard", regolamento, DtSel, FattoreDes)
                    If FattoreDes = "" Then
                        CType(rptSchedaNPK.Section5.ReportObjects("TextDoseP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose Standard:  " & DoseSel
                    Else
                        CType(rptSchedaNPK.Section5.ReportObjects("TextDoseP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose " & FattoreDes & ": " & DoseSel
                    End If

                    DoseSel = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "K", "Standard", regolamento, DtSel, FattoreDes)
                    If FattoreDes = "" Then
                        CType(rptSchedaNPK.Section3.ReportObjects("TextDoseK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose Standard:  " & DoseSel
                    Else
                        CType(rptSchedaNPK.Section3.ReportObjects("TextDoseK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose " & FattoreDes & ": " & DoseSel
                    End If

                Catch ex As Exception
                    Log_Errori += "- imposta text report: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    rptSchedaNPK.OpenSubreport("RPT_Incrementi_P.rpt").SetDataSource(DsIncrementiP)
                    rptSchedaNPK.OpenSubreport("RPT_Decrementi_P.rpt").SetDataSource(DsDecrementiP)

                    rptSchedaNPK.OpenSubreport("RPT_Incrementi_K.rpt").SetDataSource(DsIncrementiK)
                    rptSchedaNPK.OpenSubreport("RPT_Decrementi_K.rpt").SetDataSource(DsDecrementiK)
                Catch ex As Exception
                    Log_Errori += "- sottoreport SetDataSource: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    publicCaricaDosi(N_Ammesso, P_Ammesso, K_Ammesso, Sup_Tot, Qs_Piva, Qs_PCTestataCod, rptSchedaNPK, objParametri_Server)
                Catch ex As Exception
                    Log_Errori += "- publicCaricaDosi: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Try
                    Dim drLogo = DS_LogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                    Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(personalizzazioniGraficheCliente, Log_Errori, objParametri_Server)
                    If logo.LogoStampe IsNot Nothing Then
                        drLogo.Logo = logo.LogoStampe
                        drLogo.TestoPostLogo = logo.TestoPostLogo
                        drLogo.TestoPreLogo = logo.TestoPreLogo
                    End If
                    DS_LogoFooter.DT_LogoFooter.Rows.Add(drLogo)
                    rptSchedaNPK.OpenSubreport("FooterLogo.rpt").SetDataSource(DS_LogoFooter)
                Catch ex As Exception
                    Log_Errori += "- Carica Loghi: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Dim strFile As String = ""
                'creo la variabile per valorizzarla nel SalvaPdf che poi mi server per la gestione_allegati
                Dim NomeFile As String = CreaNomeFile(Qs_Piva, Qs_PCTestataCod, objParametri_Server)

                Try
                    ' il pdf viene salvato sempre

                    Try
                        ' salvo il report in formato PDF
                        Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                        objGestFile.SalvaReportPdf(rptSchedaNPK,
                                           enum_CategorieDocumenti.PianoConcimazione,
                                           "Piano Concimazione",
                                           NomeFile,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                        'strFile = SalvaPdf(objParametri_Server, rptBilancioNPK, NomeFile)
                    Catch ex As Exception
                        Log_Errori += "- SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
                    End Try

                    'il pdf viene salvato sempre
                    'strFile = public_SalvaPdf(objParametri_Server, rptSchedaNPK, NomeFile)

                    'salvo il record allegato se è stato salvato il file pdf
                    If strFile <> "" Then
                        Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                        Dim AllegatiDocumentiCod As Integer

                        Dim anno As Integer
                        If Not IsNothing(Session("Anno")) AndAlso IsNumeric(Session("Anno")) Then
                            anno = CInt(Session("Anno"))
                        Else
                            anno = Now.Year
                        End If

                        AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva.ToString,
                                                                     enum_CategorieDocumenti.PianoConcimazione,
                                                                     "Piano Concimazione",
                                                                     NomeFile,
                                                                     Session("Sottocartella"),
                                                                     Qs_PCTestataCod, Qs_Piva.ToString, "", "",
                                                                     CDate("01/01/" & anno.ToString),
                                                                     CDate("31/12/" & anno.ToString),
                                                                     objParametri_Server)

                        Dim objPC_dettagli_W As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
                        'INSERISCO IL CODICE ALLEGATO NELLA TABELLA Piano_Concimazione_Dettagli del PC salvato
                        If Not IsNothing(AllegatiDocumentiCod) AndAlso AllegatiDocumentiCod <> 0 Then
                            If Not objPC_dettagli_W.UpdateCodAllegato(Qs_PCTestataCod, 0, Qs_Piva.ToString, AllegatiDocumentiCod, objParametri_Server) Then
                                Throw New Exception("Non sono riuscito ad associare l'allegato al corrente Piano di Concimanzione. PC_cod =" & Qs_PCTestataCod.ToString)
                            End If
                        End If
                    End If

                Catch ex As Exception
                    Log_Errori += "- gestione allegati: " + vbCrLf + ex.Message + vbCrLf
                End Try


                Try
                    Dim MostraDataFirma As Boolean = False

                    Dim objUtenti As New AgronicaCoreUtentiDAL.Utenti_Impostazioni_Read

                    Dim DtImpostazioni = objUtenti.Leggi(0, 1,
                                                     enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                     "", "", objParametri_Utenti)

                    If Not IsNothing(DtImpostazioni) Then

                        If DtImpostazioni.Select("Impostazione_Cod = " & enum_Impostazioni_Utenti.UTENTE_COD_FILTRO_STAMPA_PIANOCONCIMAZIONE_CAMPAGNA_DATA_FIRMA & "And Impostazione_Valore_1 = '1'").Length > 0 Then
                            MostraDataFirma = True
                        End If

                    End If


                    'Nascondo la Sezione con la Data e la Firma
                    If Not MostraDataFirma Then
                        rptSchedaNPK.ReportFooterSection1.SectionFormat.EnableSuppress = True
                    End If

                Catch ex As Exception
                    Log_Errori += "- Lettura Utenti_Impostazioni e Nascondi/Mostra Sezioni: " + vbCrLf + ex.Message + vbCrLf
                End Try

                Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
                Try
                    rptSchedaNPK.SaveAs(reportTemporano, True)
                Catch ex As Exception
                    Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
                End Try

                '-----------------------------------------
                '---- Salvataggio Log Errori -------------
                '-----------------------------------------
                If Log_Errori <> "" Then

                    Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Qs_Piva) +
             ", Qs_PCTestataCod = " + CStr(Qs_PCTestataCod) + vbCrLf + vbCrLf + Log_Errori

                    Dim Nome_File As String = "Log_Errori_" + Nome_Documento

                    Dim objLog As New AgronicaCoreDataProvider.LogProvider
                    objLog.Gestione_LogErrori(objParametri_Server,
                                  "PianoConcimazione",
                                   Nome_File & ".txt",
                                   Session("ASG_Utente_Username"),
                                    "Scheda_NPK",
                                     Log_Errori)

                End If

                If Qs_Anteprima = "1" Then

                    'Session("Report") = rptSchedaNPK

                    Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server))
                Else

                    'Session("Report") = rptSchedaNPK

                    Dim UrlStampa As String
                    Dim UrlFiltro As String

                    UrlStampa = "../VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server)

                    UrlFiltro = "../Filtro_StampaScadenza.aspx" &
                           "?a=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Attivita.SupportoDecisioni_PianoConcimazione), AgroKey_EncoderDecoder, Server) +
                           "&o=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Operazione.Modifica), AgroKey_EncoderDecoder, Server) &
                           "&piva=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                           "&pc=" + Stringa_Codifica(CStr(Qs_PCTestataCod), AgroKey_EncoderDecoder, Server) &
                           "&scadenziario=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                           "&pdf=" + Stringa_Codifica(strFile, AgroKey_EncoderDecoder, Server)

                    Dim strOpen As String = "<script language='javascript'>" & vbNewLine &
                                        "window.open('" & UrlStampa & "');" & vbNewLine &
                                        "location.href='" & UrlFiltro & "';" & vbNewLine &
                                        "</script>"

                    Me.Page.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))


                    Exit Sub

                End If

                '        CrystalReportViewer1.ReportSource = rptSchedaNPK
                '        CrystalReportViewer1.DataBind()

            End If

            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptSchedaNPK.SetDataSource(CType(Session("DS")(0), DataSet))

            '        'faccio il databind col visualizzatore dei reports...
            '        CrystalReportViewer1.ReportSource = rptSchedaNPK


            '        CrystalReportViewer1.DataBind()

            '    End If

        End If

        ''==================================================================



        '' Dichiara le variabili e restituisce le opzioni di esportazione.
        'Dim exportOpts As New ExportOptions
        'Dim diskOpts As New DiskFileDestinationOptions

        'exportOpts = rptSchedaNPK.ExportOptions

        '' Imposta il formato di esportazione.
        'exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        'exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        '' Imposta le opzioni relative al file del disco.
        'Dim strPath As String
        'Dim PathFileTemporanei As String

        'If ConfigurationSettings.AppSettings("CartellaFileTemporanei").ToString = "" Then
        '    PathFileTemporanei = Server.MapPath("../../File_Temporanei")
        'Else
        '    PathFileTemporanei = ConfigurationSettings.AppSettings("CartellaFileTemporanei").ToString
        'End If

        'strPath = PathFileTemporanei & "\Scheda_NPK_" & Qs_PCTestataCod & "_" & Session("ASG_Utente_Username") & ".pdf"
        'diskOpts.DiskFileName = strPath
        'exportOpts.DestinationOptions = diskOpts

        '' Esportazione del report.
        'rptSchedaNPK.Export()

        '' Con il seguente codice il file pdf viene scritto 
        ''  nel browser del client.
        'Response.ClearContent()
        'Response.ClearHeaders()
        'Response.ContentType = "application/pdf"
        'Response.WriteFile(strPath)
        'Response.Flush()
        'Response.Close()

        '' il file esportato viene eliminato dal disco
        'System.IO.File.Delete(strPath)


    End Sub


    Public Sub public_Dati_Contenuto(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Fase_Cod As Integer, ByVal Resa As Decimal,
                                     ByVal N_Ammesso As Decimal, ByVal P_Ammesso As Decimal, ByVal K_Ammesso As Decimal,
                                     ByVal Sup_Tot As Double, ByVal PCTestataCod As Integer, ByVal Piva As String, ByVal regolamento As AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti,
                                                                 ByVal Mas As Decimal, ByVal MasSalvato As Boolean,
                                     ByRef rpt As RPT_Scheda_NPK, objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        Dim DsIncrementiN As New DS_FattoriCorrettivi
        Dim DsDecrementiN As New DS_FattoriCorrettivi
        Dim DsIncrementiP As New DS_FattoriCorrettivi
        Dim DsDecrementiP As New DS_FattoriCorrettivi
        Dim DsIncrementiK As New DS_FattoriCorrettivi
        Dim DsDecrementiK As New DS_FattoriCorrettivi


        Dim objFattoriSalvati As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_R
        Dim DtSel As DataTable = objFattoriSalvati.Leggi(regolamento, PCTestataCod, 0, "", "", objParametri)

        'carico i dati nei datatable 
        Dim Tot As Double = 0
        'publicCarica_DsFattori(DsIncrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Incremento", Tot, PCTestataCod, objParametri)
        publicCarica_DsFattori(DsIncrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Incremento", Tot, DtSel, objParametri)
        CType(rpt.Section1.ReportObjects("TextTotIncrementi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Incrementi: " & Tot.ToString

        Tot = 0
        'publicCarica_DsFattori(DsDecrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Decremento", Tot, PCTestataCod, objParametri)
        publicCarica_DsFattori(DsDecrementiN, Veg_Cod, Grfi_Cod, "N", regolamento, "Decremento", Tot, DtSel, objParametri)
        CType(rpt.Section1.ReportObjects("TextTotDecrementi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Decrementi: " & Tot.ToString

        Tot = 0
        'publicCarica_DsFattori(DsIncrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Incremento", Tot, PCTestataCod, objParametri)
        publicCarica_DsFattori(DsIncrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Incremento", Tot, DtSel, objParametri)
        CType(rpt.Section5.ReportObjects("TextTotIncrementiP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Incrementi: " & Tot.ToString

        Tot = 0
        'publicCarica_DsFattori(DsDecrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Decremento", Tot, PCTestataCod, objParametri)
        publicCarica_DsFattori(DsDecrementiP, Veg_Cod, Grfi_Cod, "P", regolamento, "Decremento", Tot, DtSel, objParametri)
        CType(rpt.Section5.ReportObjects("TextTotDecrementiP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Decrementi: " & Tot.ToString

        Tot = 0
        'publicCarica_DsFattori(DsIncrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Incremento", Tot, PCTestataCod, objParametri)
        publicCarica_DsFattori(DsIncrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Incremento", Tot, DtSel, objParametri)
        CType(rpt.Section3.ReportObjects("TextTotIncrementiK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Incrementi: " & Tot.ToString

        Tot = 0
        'publicCarica_DsFattori(DsDecrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Decremento", Tot, PCTestataCod, objParametri)
        publicCarica_DsFattori(DsDecrementiK, Veg_Cod, Grfi_Cod, "K", regolamento, "Decremento", Tot, DtSel, objParametri)
        CType(rpt.Section3.ReportObjects("TextTotDecrementiK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Tot Decrementi: " & Tot.ToString

        rpt.OpenSubreport("RPT_Incrementi_N.rpt").SetDataSource(DsIncrementiN)
        rpt.OpenSubreport("RPT_Decrementi_N.rpt").SetDataSource(DsDecrementiN)

        Dim DoseSel As String
        Dim dbl As Double
        Dim FattoreDes As String = ""
        Dim strFattoreCorrettivo As String = ""

        dbl = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "N", "Standard", regolamento, DtSel, FattoreDes)
        'dbl = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "N", "Standard", regolamento)
        CType(rpt.Section1.ReportObjects("TextDoseStd"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose Standard: " & dbl.ToString

        If MasSalvato = False Then
            Mas = publicCaricaMAS(Veg_Cod, Grfi_Cod, regolamento, Resa, strFattoreCorrettivo, objParametri)
        End If

        If strFattoreCorrettivo <> "" Then
            CType(rpt.Section1.ReportObjects("TextLimiteMas"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS *: " & Mas.ToString
            CType(rpt.Section1.ReportObjects("TextLimiteMasNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "* " & strFattoreCorrettivo
        Else
            CType(rpt.Section1.ReportObjects("TextLimiteMas"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limite MAS: " & Mas.ToString
            CType(rpt.Section1.ReportObjects("TextLimiteMasNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        End If

        dbl = publicCaricaMaxIncrementi(Veg_Cod, Grfi_Cod, "N", "Incremento", regolamento)
        CType(rpt.Section1.ReportObjects("TextMaxIncrementi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Max Incrementi: " & dbl.ToString

        DoseSel = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "P", "Standard", regolamento, DtSel, FattoreDes)
        If FattoreDes = "" Then
            CType(rpt.Section5.ReportObjects("TextDoseP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose Standard:  " & DoseSel
        Else
            CType(rpt.Section5.ReportObjects("TextDoseP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose " & FattoreDes & ": " & DoseSel
        End If

        DoseSel = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "K", "Standard", regolamento, DtSel, FattoreDes)
        If FattoreDes = "" Then
            CType(rpt.Section3.ReportObjects("TextDoseK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose Standard:  " & DoseSel
        Else
            CType(rpt.Section3.ReportObjects("TextDoseK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose " & FattoreDes & ": " & DoseSel
        End If
        'DoseSel = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "P", "Standard", regolamento)
        'CType(rpt.Section5.ReportObjects("TextDoseP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose " & DoseSel
        'DoseSel = publicCaricaDoseStandard(Veg_Cod, Grfi_Cod, Fase_Cod, "K", "Standard", regolamento)
        'CType(rpt.Section3.ReportObjects("TextDoseK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dose " & DoseSel

        rpt.OpenSubreport("RPT_Incrementi_P.rpt").SetDataSource(DsIncrementiP)
        rpt.OpenSubreport("RPT_Decrementi_P.rpt").SetDataSource(DsDecrementiP)

        rpt.OpenSubreport("RPT_Incrementi_K.rpt").SetDataSource(DsIncrementiK)
        rpt.OpenSubreport("RPT_Decrementi_K.rpt").SetDataSource(DsDecrementiK)

        publicCaricaDosi(N_Ammesso, P_Ammesso, K_Ammesso, Sup_Tot, Piva, PCTestataCod, rpt, objParametri)


    End Sub

    '################################################################################
    Public Sub Dati_Piano(ByVal Piva As String, ByVal SaCod As Integer, ByVal PCTestataCod As Integer,
                                   ByRef regolamento As AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti,
                                   ByRef Veg_Cod As Integer, ByRef Grfi_Cod As Integer, ByRef Fase_Cod As Integer, ByRef Resa As Decimal,
                                   ByRef Sup_Tot As Double, ByRef Sa_Nome As String,
                                   ByRef N_Ammesso As Decimal, ByRef P_Ammesso As Decimal, ByRef K_Ammesso As Decimal,
                                   ByRef Mas As Decimal, ByRef MasSalvato As Boolean,
                                  ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef rptSchedaNPK As RPT_Scheda_NPK)

        Dim Dt As New DataTable
        Dim Rag_Soc As String = ""
        Dim strErr As String = ""

        '---------------------------------------------------------------------
        Dim CodSocio As String
        Dim objCodiciImpresa As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        CodSocio = objCodiciImpresa.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Codice_Socio, objParametri) & "   "
        Session("Socio") = CodSocio

        ''---------------------------------------------------------------------
        ''Ricavo la Ragione Sociale e Il Nome del Centro
        'Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        'Dt = objCentri.Anagrafica_Centri_Leggi(Piva,
        '                                      SaCod,
        '                                      strErr,
        '                                      enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
        '                                      "", "",
        '                                      objParametri)

        'If Dt.Rows.Count > 0 Then

        '    Rag_Soc = Dt.Rows(0).Item("Rag_Soc")
        '    CType(rptSchedaNPK.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Impresa : " & Rag_Soc

        '    If Dt.Rows.Count = 1 Then
        '        Sa_Nome = Dt.Rows(0).Item("Sa_Nome")
        '        CType(rptSchedaNPK.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Centro : " & Sa_Nome
        '    Else
        '        CType(rptSchedaNPK.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        '    End If

        'End If

        Dim DtPC_Testata As DataTable
        Dim objPC_Testata As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Testata_R
        Dim DtPC_Dettagli As DataTable
        Dim objPC_Dettagli As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_R

        '--- PC_TESTATA
        DtPC_Testata = objPC_Testata.Leggi_default(
                                            PCTestataCod,
                                            0, 0, enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "", "",
                                            objParametri)

        If Not IsNothing(DtPC_Testata) AndAlso DtPC_Testata.Rows.Count > 0 Then


            CType(rptSchedaNPK.Section1.ReportObjects("TextDescrizione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DtPC_Testata.Rows(0).Item("PC_Testata_Des")

            Dim ValiditaInizio As String
            Dim ValiditaFine As String
            ValiditaInizio = DtPC_Testata.Rows(0).Item("Validita_Inizio")
            ValiditaFine = DtPC_Testata.Rows(0).Item("Validita_Fine")

            If ValiditaInizio = "01/01/1900" Then
                ValiditaInizio = ""
            End If
            If ValiditaFine = "31/12/2100" Then
                ValiditaFine = ""
            End If

            If ValiditaInizio <> "" Or ValiditaFine <> "" Then
                CType(rptSchedaNPK.Section1.ReportObjects("TextValidita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ValiditaInizio &
                                                                                                                                IIf(ValiditaInizio <> "" And ValiditaFine <> "", " - ", "") _
                                                                                                                                & ValiditaFine
            Else
                CType(rptSchedaNPK.Section1.ReportObjects("TextValidita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            End If

            regolamento = DtPC_Testata.Rows(0).Item("Regolamento_Cod")

            CType(rptSchedaNPK.ReportHeaderSection1.ReportObjects("TextDichiarazioneNonUtilizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            If Not IsDBNull(DtPC_Testata.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) AndAlso CInt(DtPC_Testata.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) > 0 Then
                CType(rptSchedaNPK.ReportHeaderSection1.ReportObjects("TextDichiarazioneNonUtilizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dichiarazione di Non Utilizzo Fertilizzanti"
            End If

            CType(rptSchedaNPK.ReportHeaderSection1.ReportObjects("TextNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
            If Not IsDBNull(DtPC_Testata.Rows(0).Item("Note")) AndAlso CStr(DtPC_Testata.Rows(0).Item("Note")) <> "" Then
                CType(rptSchedaNPK.ReportHeaderSection1.ReportObjects("TextNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "NOTA :" & CStr(DtPC_Testata.Rows(0).Item("Note"))
            End If

            DtPC_Testata = Nothing

            '--- PC_DETTAGLI
            DtPC_Dettagli = objPC_Dettagli.Leggi_default(
                                                PCTestataCod,
                                                0, Piva,
                                                enumSelezioneVariabile.Selezione_TabellaCompleta,
                                                "", "",
                                                objParametri)


            If Not IsNothing(DtPC_Dettagli) AndAlso DtPC_Dettagli.Rows.Count > 0 Then

                '---------------------------------------------------------------------
                'Ricavo la Ragione Sociale e Il Nome del Centro
                Dim objCentri As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

                Dt = objCentri.Anagrafica_Centri_Leggi(Piva,
                                              DtPC_Dettagli.Rows(0).Item("PC_Dettagli_SaCod"),
                                              strErr,
                                              enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                              "", "",
                                              objParametri)

                If Dt.Rows.Count > 0 Then

                    Rag_Soc = CodSocio & " " & Dt.Rows(0).Item("Rag_Soc")
                    CType(rptSchedaNPK.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Impresa : " & Rag_Soc

                    If Dt.Rows.Count = 1 Then
                        Sa_Nome = Dt.Rows(0).Item("Sa_Nome")
                        CType(rptSchedaNPK.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Centro : " & Sa_Nome
                    Else
                        CType(rptSchedaNPK.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
                    End If

                End If


                CType(rptSchedaNPK.Section1.ReportObjects("TextZVN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = IIf(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_AreaVulnerabile") = 1, "ZVN", "")

                CType(rptSchedaNPK.Section1.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Anno " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Anno")
                CType(rptSchedaNPK.Section1.ReportObjects("TextAreaOmogenea"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Area Omogenea " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_AreaOmogenea")

                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")) Then
                    Veg_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ColturaPrincipale_Veg_Cod")
                End If

                Grfi_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Finalita_GRFI_COD")
                Fase_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_FaseCicloColturale_id_fase")
                Resa = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Resa")

                Dim Analisi_Cod As Integer = 0
                Dim Analisi_Des As String = ""
                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Analisi_Testata_Cod")) Then
                    Analisi_Cod = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Analisi_Testata_Cod")
                End If
                If Analisi_Cod <> 0 Then
                    Dim ObjAnalisiR As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
                    Dim DtAnalisi As New DataTable
                    DtAnalisi = ObjAnalisiR.LeggiConCertificati(Piva, DtPC_Dettagli.Rows(0).Item("PC_Dettagli_SaCod"),
                                                                Analisi_Cod, 0, "", "", objParametri)
                    If Not DtAnalisi Is Nothing AndAlso DtAnalisi.Rows.Count > 0 Then
                        Analisi_Des = " - Analisi "
                        If Not IsDBNull(DtAnalisi.Rows(0).Item("analisi_testata_des")) AndAlso DtAnalisi.Rows(0).Item("analisi_testata_des") <> "" Then
                            Analisi_Des &= DtAnalisi.Rows(0).Item("analisi_testata_des")
                        End If
                        If Not IsDBNull(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")) AndAlso IsDate(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")) AndAlso CDate(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")) <> AGRODATAINIZIO Then
                            Analisi_Des &= " del " & CDate(DtAnalisi.Rows(0).Item("analisi_testata_data_inizio")).ToShortDateString
                        End If
                        If Not IsDBNull(DtAnalisi.Rows(0).Item("Analisi_Certificato_Des")) AndAlso DtAnalisi.Rows(0).Item("Analisi_Certificato_Des") <> "" Then
                            Analisi_Des &= " - N° Certificato " & DtAnalisi.Rows(0).Item("Analisi_Certificato_Des")
                        End If
                        If Not IsDBNull(DtAnalisi.Rows(0).Item("Analisi_Testata_Note1")) AndAlso DtAnalisi.Rows(0).Item("Analisi_Testata_Note1") <> "" Then
                            Analisi_Des &= " - " & DtAnalisi.Rows(0).Item("Analisi_Testata_Note1")
                        End If
                    End If
                End If
                CType(rptSchedaNPK.ReportHeaderSection5.ReportObjects("TextAnalisi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Analisi_Des

                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextSabbia"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sabbia:  " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Sabbia") & " %"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextLimo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Limo: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Limo") & " %"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextArgilla"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Argilla: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Argilla") & " %"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextPH"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "pH: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Ph")
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextCalcTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Calc. Tot: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Caco3") & " %"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextCalcAtt"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Calc. Att.: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Caco3_Attivo") & " %"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextSO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "S.O.: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_So") & " %"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "N: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_ntot") & " g/kg"

                Dim PC_Dettagli_Flag_P As Integer = 0
                Dim PC_Dettagli_Flag_K As Integer = 0
                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_P")) Then
                    PC_Dettagli_Flag_P = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_P")
                End If
                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_K")) Then
                    PC_Dettagli_Flag_K = DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Flag_K")
                End If
                If PC_Dettagli_Flag_P = 0 Then
                    CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextP2O5"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "P2O5: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_p2o5") & " ppm"
                Else
                    CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextP2O5"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "P: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_p") & " ppm"
                End If
                If PC_Dettagli_Flag_K = 0 Then
                    CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextK2O"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "K2O: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_k2o") & " ppm"
                Else
                    CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextK2O"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "K: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_k") & " ppm"
                End If

                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextCN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "C/N: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_CN")
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextMg"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "MgO: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_Mg") & " ppm"
                CType(rptSchedaNPK.ReportHeaderSection6.ReportObjects("TextCsc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "C.S.C.: " & DtPC_Dettagli.Rows(0).Item("PC_Dettagli_CSC") & " meq/100 g"

                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("N_Ammesso")) Then
                    N_Ammesso = DtPC_Dettagli.Rows(0).Item("N_Ammesso")
                End If
                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("P_Ammesso")) Then
                    P_Ammesso = DtPC_Dettagli.Rows(0).Item("P_Ammesso")
                End If
                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("K_Ammesso")) Then
                    K_Ammesso = DtPC_Dettagli.Rows(0).Item("K_Ammesso")
                End If

                Mas = -1

                If Not IsDBNull(DtPC_Dettagli.Rows(0).Item("N_Mas")) Then
                    MasSalvato = True
                    If IsNumeric(DtPC_Dettagli.Rows(0).Item("N_Mas")) Then
                        Mas = CDec(DtPC_Dettagli.Rows(0).Item("N_Mas"))
                    End If
                End If

                DtPC_Dettagli = Nothing

            End If

            Dim objEntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R
            Dim objImpianti As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Read

            Dt = objEntitaxTestata.Leggi(PCTestataCod, 0, Piva, SaCod, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "",
                                         AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                         "", "",
                                         objParametri)

            Dim i, j As Integer
            Dim DtImp As DataTable
            Dim strAppezza1 As String = ""
            Dim strAppezza2 As String = ""
            Dim SupImp As Double = 0
            Dim ris As Integer

            Dim tempAppezza As Integer
            Dim tempId_Imp As Integer

            Dim FiltroImpianti As String = ""

            For i = 0 To Dt.Rows.Count - 1
                FiltroImpianti &= " (Reg_Impianti.PIVA='" + Dt.Rows(i).Item("Piva") + "' " +
                    " AND Reg_Impianti.SA_COD=" + Dt.Rows(i).Item("Sa_Cod").ToString +
                    " AND Reg_Impianti.APPEZZA=" + Dt.Rows(i).Item("appezza").ToString +
                    " AND Reg_Impianti.ID_REG=" + Dt.Rows(i).Item("Id_Imp").ToString +
                    " ) OR"
            Next

            If FiltroImpianti <> "" Then

                FiltroImpianti = Left(FiltroImpianti, FiltroImpianti.Length - 2)
                tempAppezza = Dt.Rows(0).Item("appezza")
                tempId_Imp = Dt.Rows(0).Item("Id_Imp")

                DtImp = objImpianti.Leggi_DescrizioniImpianti(Piva,
                                              SaCod,
                                              0,
                                              0,
                                              enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              FiltroImpianti, "", objParametri)

                For i = 0 To DtImp.Rows.Count - 1
                    Math.DivRem(i + 2, 2, ris)
                    If ris = 0 Then
                        strAppezza1 &= DtImp.Rows(i).Item("App_Nome") & "-" & DtImp.Rows(i).Item("Veg_Des") & "-" & DtImp.Rows(i).Item("Cul_Des") & " (" & Format(DtImp.Rows(i).Item("Sup_Imp"), "0.0000") & " ha)" & vbCrLf '& "-" & DtImp.Rows(0).Item("Grfi_Des") & vbCrLf
                    Else
                        strAppezza2 &= DtImp.Rows(i).Item("App_Nome") & "-" & DtImp.Rows(i).Item("Veg_Des") & "-" & DtImp.Rows(i).Item("Cul_Des") & " (" & Format(DtImp.Rows(i).Item("Sup_Imp"), "0.0000") & " ha)" & vbCrLf  '& "-" & DtImp.Rows(0).Item("Grfi_Des") & vbCrLf
                    End If
                    If Not IsNothing(DtImp.Rows(i).Item("Sup_Imp")) AndAlso IsNumeric(DtImp.Rows(i).Item("Sup_Imp")) Then
                        SupImp += DtImp.Rows(i).Item("Sup_Imp")
                    End If

                    If Veg_Cod = 0 And i = 0 Then
                        Veg_Cod = DtImp.Rows(0).Item("Veg_Cod")
                    End If
                Next

            End If

            Sup_Tot = SupImp

            CType(rptSchedaNPK.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""

            If Veg_Cod <> 0 Then

                Dim objFinalitaInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_input
                objFinalitaInput.Regolamento_Cod = regolamento
                objFinalitaInput.Veg_Cod = Veg_Cod
                Dim objFinalitaRer As New AgronicaCoreWebService.PianoConcimazione_WS
                Dim objFinalitaOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FinalitaRER_output
                objFinalitaOutput = objFinalitaRer.FinalitaRER(objFinalitaInput)

                Dim objFaseCicloInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_input
                objFaseCicloInput.Regolamento_Cod = regolamento
                objFaseCicloInput.Veg_Cod = Veg_Cod
                Dim objFaseCicloOutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
                Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
                objFaseCicloOutput = objPC_WS.FasiCicloColturale(objFaseCicloInput)

                If Not IsNothing(objFinalitaOutput) Then
                    Dim Finalita As New AgronicaCorePianoConcimazioneBIZ.Finalita
                    Dim Grfi_Cod_Tmp As Integer = Grfi_Cod
                    Finalita = objFinalitaOutput.ListaFinalita.Where(Function(x) x.Codice = Grfi_Cod_Tmp)(0)
                    If Not Finalita Is Nothing Then
                        CType(rptSchedaNPK.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Finalità: " & Finalita.Descrizione
                    End If
                End If

                If Not IsNothing(objFaseCicloOutput) Then
                    Dim Fase As New AgronicaCorePianoConcimazioneBIZ.Fase
                    Dim Fase_Cod_Tmp As Integer = Fase_Cod
                    Fase = objFaseCicloOutput.ListaFasi.Where(Function(x) x.Codice = Fase_Cod_Tmp)(0)
                    If Not Fase Is Nothing Then
                        CType(rptSchedaNPK.Section1.ReportObjects("TextFinalita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text &= " - Fase/Ciclo: " & Fase.Descrizione
                    End If
                End If


            End If

            CType(rptSchedaNPK.ReportHeaderSection2.ReportObjects("TextElencoAppezzamenti1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAppezza1
            CType(rptSchedaNPK.ReportHeaderSection2.ReportObjects("TextElencoAppezzamenti2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strAppezza2
            CType(rptSchedaNPK.Section1.ReportObjects("TextSupTot"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Superficie Totale Appezzamenti : " & Format(SupImp, "0.0000") & " ha"

            ' Note Intervento
            Dim objNote As New AgronicaCoreContabDAL.Note_Intervento_R
            Dim DtNote As DataTable
            DtNote = objNote.Leggi_con_Utilizzo(0, 0, enum_Note_Intervento_Utilizzo.PianoConcimazione, enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                            "", "", objParametri)

            Dim strNote As String = ""
            For i = 0 To DtNote.Rows.Count - 1
                strNote &= IIf(strNote <> "", vbCrLf, "") & DtNote.Rows(i).Item("Nota_Des")
            Next

            If strNote <> "" Then
                CType(rptSchedaNPK.Section4.ReportObjects("TextNoteIntervento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = strNote
            End If
            objNote = Nothing


        End If


    End Sub


    ''###########################################################################################################################
    'Private Sub Carica_DsFattori(ByRef DsFC As DS_FattoriCorrettivi, _
    '                             ByVal Veg_Cod As Integer, _
    '                             ByVal Grfi_Cod As Integer, _
    '                             ByVal Tipo As String, _
    '                             ByVal Variazione As String, _
    '                             ByRef Tot As Double)

    '    Dim Dt As DataTable
    '    Dim DtSel As DataTable
    '    Dim objFattori As New AgronicaCoreAnagrafeDAL.PianoConcimazione_FattoriCorrettivi_R

    '    Dim objFattoriCorrettivi As New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
    '    Dt = objFattoriCorrettivi.LeggiValoriVariazioni(4, 0, Tipo, Variazione, Veg_Cod, Grfi_Cod, " FC.Visibile = 1 AND FCS.Valore > 0 ", "", objParametri_Server)

    '    DtSel = objFattori.Leggi(AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti.PianoConcimazione_2012, _
    '                     Qs_PCTestataCod, _
    '                     0, _
    '                     " Tipo='" & Tipo & "' AND Variazione = '" & Variazione & "' ", _
    '                     "", _
    '                     objParametri_Server)

    '    Dim dr As DS_FattoriCorrettivi.DT_FattoriCorrettiviRow

    '    Dim i As Integer
    '    For i = 0 To Dt.Rows.Count - 1

    '        dr = DsFC.DT_FattoriCorrettivi.NewDT_FattoriCorrettiviRow

    '        dr.Fattore_Cod = Dt.Rows(i).Item("Fattore_Cod")
    '        dr.Fattore_Des = Dt.Rows(i).Item("Fattore_des")
    '        dr.Valore = Dt.Rows(i).Item("Valore").ToString
    '        dr.Selezionato = "0"

    '        Dim j As Integer
    '        For j = 0 To DtSel.Rows.Count - 1
    '            'dr.Selezionato = "0"
    '            If DtSel.Rows(j).Item("Fattore_Cod") = Dt.Rows(i).Item("Fattore_Cod") Then
    '                dr.Selezionato = "1"
    '                Tot += Dt.Rows(i).Item("Valore")
    '                Exit For
    '            End If
    '        Next

    '        DsFC.DT_FattoriCorrettivi.AddDT_FattoriCorrettiviRow(dr)

    '    Next


    'End Sub


    '###########################################################################################################################
    Public Sub publicCarica_DsFattori(ByRef DsFC As DS_FattoriCorrettivi,
                                 ByVal Veg_Cod As Integer,
                                 ByVal Grfi_Cod As Integer,
                                 ByVal Tipo As String,
                                 ByVal Regolamento_Cod As enum_PUARegolamenti,
                                 ByVal Variazione As String,
                                 ByRef Tot As Double,
                                 ByVal DtSel As DataTable,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)


        Dim objFattoriInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
        Dim objFattori As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objFattoriOutput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
        objFattoriInput.Veg_Cod = Veg_Cod
        objFattoriInput.Grfi_Cod = Grfi_Cod
        objFattoriInput.Variazione = Variazione
        objFattoriInput.Tipo = Tipo
        objFattoriInput.Regolamento_Cod = Regolamento_Cod
        'objFattoriInput.Fattore_Cod = 0 '?????? TO TEST
        objFattoriInput.SoloValorizzati = True
        objFattoriInput.SoloVisibili = False ' ?????? TO TEST

        objFattoriOutput = objFattori.FattoriCorrettivi_Leggi(objFattoriInput)

        'Dim objFattoriCorrettivi As New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
        'Dt = objFattoriCorrettivi.LeggiValoriVariazioni(Regolamento_Cod, 0, Tipo, Variazione, Veg_Cod, Grfi_Cod, " FC.Visibile = 1 AND FCS.Valore > 0 ", "", objParametri)

        'DtSel = objFattoriSalvati.Leggi(Regolamento_Cod, _
        '                 PCTestataCod, _
        '                 0, _
        '                 " Tipo='" & Tipo & "' AND Variazione = '" & Variazione & "' ", _
        '                 "", _
        '                 objParametri)

        'DtSel = objFattoriSalvati.Leggi(Regolamento_Cod,
        '                 PCTestataCod,
        '                 0,
        '                 "",
        '                 "",
        '                 objParametri)

        Dim dr As DS_FattoriCorrettivi.DT_FattoriCorrettiviRow

        If objFattoriOutput.ListaFattoriCorrettivi.Count > 0 Then

            For Each fat In objFattoriOutput.ListaFattoriCorrettivi

                dr = DsFC.DT_FattoriCorrettivi.NewDT_FattoriCorrettiviRow

                dr.Fattore_Cod = fat.Codice
                dr.Fattore_Des = fat.Descrizione 'Dt.Rows(i).Item("Fattore_des")
                dr.Valore = fat.Valore.ToString 'Dt.Rows(i).Item("Valore").ToString
                dr.Selezionato = "0"

                Dim j As Integer
                For j = 0 To DtSel.Rows.Count - 1
                    'dr.Selezionato = "0"
                    If DtSel.Rows(j).Item("Fattore_Cod") = fat.Codice Then
                        dr.Selezionato = "1"
                        Tot += fat.Valore
                        Exit For
                    End If
                Next

                DsFC.DT_FattoriCorrettivi.AddDT_FattoriCorrettiviRow(dr)

            Next
        Else
            If objFattoriOutput.MessaggioErrore <> "" Then
                Throw New Exception(objFattoriOutput.MessaggioErrore)
            End If
        End If


    End Sub

    Public Sub publicCarica_DsFattori_old(ByRef DsFC As DS_FattoriCorrettivi,
                                 ByVal Veg_Cod As Integer,
                                 ByVal Grfi_Cod As Integer,
                                 ByVal Tipo As String,
                                 ByVal Regolamento_Cod As enum_PUARegolamenti,
                                 ByVal Variazione As String,
                                 ByRef Tot As Double,
                                 ByVal PCTestataCod As Integer,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'Dim Dt As DataTable
        Dim DtSel As DataTable

        Dim objFattoriSalvati As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_FattoriCorrettivi_R

        Dim objFattoriInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
        Dim objFattori As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objFattoriOutput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output
        objFattoriInput.Veg_Cod = Veg_Cod
        objFattoriInput.Grfi_Cod = Grfi_Cod
        objFattoriInput.Variazione = Variazione
        objFattoriInput.Tipo = Tipo
        objFattoriInput.Regolamento_Cod = Regolamento_Cod
        'objFattoriInput.Fattore_Cod = 0 '?????? TO TEST
        objFattoriInput.SoloValorizzati = True
        objFattoriInput.SoloVisibili = False ' ?????? TO TEST

        objFattoriOutput = objFattori.FattoriCorrettivi_Leggi(objFattoriInput)

        'Dim objFattoriCorrettivi As New AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R
        'Dt = objFattoriCorrettivi.LeggiValoriVariazioni(Regolamento_Cod, 0, Tipo, Variazione, Veg_Cod, Grfi_Cod, " FC.Visibile = 1 AND FCS.Valore > 0 ", "", objParametri)

        'DtSel = objFattoriSalvati.Leggi(Regolamento_Cod, _
        '                 PCTestataCod, _
        '                 0, _
        '                 " Tipo='" & Tipo & "' AND Variazione = '" & Variazione & "' ", _
        '                 "", _
        '                 objParametri)

        DtSel = objFattoriSalvati.Leggi(Regolamento_Cod,
                         PCTestataCod,
                         0,
                         "",
                         "",
                         objParametri)

        Dim dr As DS_FattoriCorrettivi.DT_FattoriCorrettiviRow

        If objFattoriOutput.ListaFattoriCorrettivi.Count > 0 Then

            For Each fat In objFattoriOutput.ListaFattoriCorrettivi

                dr = DsFC.DT_FattoriCorrettivi.NewDT_FattoriCorrettiviRow

                dr.Fattore_Cod = fat.Codice
                dr.Fattore_Des = fat.Descrizione 'Dt.Rows(i).Item("Fattore_des")
                dr.Valore = fat.Valore.ToString 'Dt.Rows(i).Item("Valore").ToString
                dr.Selezionato = "0"

                Dim j As Integer
                For j = 0 To DtSel.Rows.Count - 1
                    'dr.Selezionato = "0"
                    If DtSel.Rows(j).Item("Fattore_Cod") = fat.Codice Then
                        dr.Selezionato = "1"
                        Tot += fat.Valore
                        Exit For
                    End If
                Next

                DsFC.DT_FattoriCorrettivi.AddDT_FattoriCorrettiviRow(dr)

            Next
        Else
            If objFattoriOutput.MessaggioErrore <> "" Then
                Throw New Exception(objFattoriOutput.MessaggioErrore)
            End If
        End If


    End Sub

    '#####################################################################################################################
    Public Function publicCaricaDoseStandard_old(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Fase_Cod As Integer,
                                             ByVal Tipo As String, ByVal Variazione As String,
                                             ByVal Regolamento_Cod As enum_PUARegolamenti) As Double

        Dim Dt As DataTable
        'Dim objFattoriCorrettivi As AgronicaCoreMetaSchemaDAL.FattoriCorrettivixSpecie_R

        Dim objFattoriInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
        Dim objFattori As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objFattoriOutput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output

        Dim dblDose As Decimal = 0
        Dim Fattore_Cod As Integer

        Try

            Select Case Tipo
                Case "N"
                    Select Case Fase_Cod
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento
                        Case Else
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard
                    End Select
                Case "P"
                    Select Case Fase_Cod
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento
                        Case Else
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.P_Dose_Standard
                    End Select
                Case "K"
                    Select Case Fase_Cod
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento
                        Case Else
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.K_Dose_Standard
                    End Select
            End Select

            If Fase_Cod <> enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto Then

                objFattoriInput.Veg_Cod = Veg_Cod
                objFattoriInput.Grfi_Cod = Grfi_Cod
                objFattoriInput.Variazione = Variazione
                objFattoriInput.Tipo = Tipo
                objFattoriInput.Regolamento_Cod = Regolamento_Cod
                'objFattoriInput.Fattore_Cod = 0 '?????? TO TEST
                objFattoriInput.SoloValorizzati = True
                objFattoriInput.SoloVisibili = False ' ?????? TO TEST

                ' Dose Standard
                objFattoriOutput = objFattori.FattoriCorrettivi_Leggi(objFattoriInput)

                Dim Lista_Dose As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
                Lista_Dose = objFattoriOutput.ListaFattoriCorrettivi.Where(Function(x) x.Codice = Fattore_Cod).ToList

                If Not IsNothing(Lista_Dose) Then
                    If Lista_Dose.Count > 0 Then
                        dblDose = Lista_Dose(0).Valore.ToString
                    End If
                End If

            End If

            Dt = Nothing

        Catch ex As Exception

        End Try

        Return dblDose


    End Function

    Public Function publicCaricaDoseStandard(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Fase_Cod As Integer,
                                             ByVal Tipo As String, ByVal Variazione As String,
                                             ByVal Regolamento_Cod As enum_PUARegolamenti,
                                                 ByVal DtSel As DataTable,
                                                 ByRef Fattore_Des As String) As Double

        Dim Dt As DataTable

        Dim objFattoriInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
        Dim objFattori As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objFattoriOutput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output

        Dim dblDose As Decimal = 0
        Fattore_Des = ""
        Dim Fattore_Cod As Integer = 0

        Try

            Select Case Tipo
                Case "N"
                    Select Case Fase_Cod
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_I_anno_allevamento
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_II_anno_allevamento
                        Case Else
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.N_Dose_Standard
                    End Select
                Case "P"
                    Select Case Fase_Cod
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.P_I_anno_allevamento
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.P_II_anno_allevamento
                    End Select
                Case "K"
                    Select Case Fase_Cod
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_I_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.K_I_anno_allevamento
                        Case enum_PianoConcimazione_FaseCicloColturale.Arb_II_Anno_Allevamento
                            Fattore_Cod = enum_PianoConcimazione_FattoriCorrettivi.K_II_anno_allevamento
                    End Select
            End Select

            If Fase_Cod <> enum_PianoConcimazione_FaseCicloColturale.Arb_PreImpianto Or
                     Fattore_Cod <> 0 Then

                objFattoriInput.Veg_Cod = Veg_Cod
                objFattoriInput.Grfi_Cod = Grfi_Cod
                objFattoriInput.Variazione = Variazione
                objFattoriInput.Tipo = Tipo
                objFattoriInput.Regolamento_Cod = Regolamento_Cod
                objFattoriInput.Fattore_Cod = Fattore_Cod
                objFattoriInput.SoloValorizzati = True
                objFattoriInput.SoloVisibili = False ' ?????? TO TEST

                ' Dose Standard
                objFattoriOutput = objFattori.FattoriCorrettivi_Leggi(objFattoriInput)

                If objFattoriOutput.ListaFattoriCorrettivi.Count > 0 Then

                    Select Case Fattore_Cod
                        Case 0
                            For Each fat In objFattoriOutput.ListaFattoriCorrettivi
                                Dim j As Integer
                                For j = 0 To DtSel.Rows.Count - 1
                                    If DtSel.Rows(j).Item("Fattore_Cod") = fat.Codice Then
                                        dblDose = fat.Valore.ToString
                                        Fattore_Des = fat.Descrizione
                                        Exit For
                                    End If
                                Next
                            Next
                        Case Else
                            dblDose = objFattoriOutput.ListaFattoriCorrettivi(0).Valore.ToString
                    End Select


                End If

            End If

            Dt = Nothing

        Catch ex As Exception

        End Try

        Return dblDose


    End Function

    Public Function publicCaricaMAS(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Regolamento_Cod As enum_PUARegolamenti, ByVal Resa As Decimal,
                                    ByRef strFattoreCorrettivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Double

        Dim N As Double

        Dim objLimitiAzotoxSpecie As AgronicaCoreWebService.PianoConcimazione_WS
        Dim objLimiteMASinput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input
        Dim objLimiteMASoutput As AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output


        Try

            objLimitiAzotoxSpecie = New AgronicaCoreWebService.PianoConcimazione_WS
            objLimiteMASinput = New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_input
            objLimiteMASoutput = New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_LimiteMAS_output
            objLimiteMASinput.Grfi_Cod = Grfi_Cod
            objLimiteMASinput.Regolamento_Cod = Regolamento_Cod
            objLimiteMASinput.Veg_Cod = Veg_Cod
            objLimiteMASinput.Stato_Cod = 102
            objLimiteMASoutput = objLimitiAzotoxSpecie.LimiteMAS(objLimiteMASinput)

            If objLimiteMASoutput.N <= 0 Then
                N = 0
            Else
                N = objLimiteMASoutput.N

                '(06/10/2020 fede) aggiunta eventuale considerazione del fattore correttivo per resa maggiore
                If objLimiteMASoutput.FattoreCorrettivo_N > 0 Then
                    If Resa > 0 AndAlso objLimiteMASoutput.Resa > 0 Then
                        If Resa - objLimiteMASoutput.Resa > 0 Then
                            N = N + ((Resa - objLimiteMASoutput.Resa) * objLimiteMASoutput.FattoreCorrettivo_N)
                            strFattoreCorrettivo = " (" & "considerando il Fattore Correttivo di " & objLimiteMASoutput.FattoreCorrettivo_N & " Kg N/t)"
                        End If
                    End If
                End If

            End If

        Catch ex As Exception

        End Try

        Return N

    End Function


    '#####################################################################################################################
    Public Function publicCaricaMaxIncrementi(ByVal Veg_Cod As Integer, ByVal Grfi_Cod As Integer, ByVal Tipo As String, ByVal Variazione As String, ByVal Regolamento_Cod As enum_PUARegolamenti) As Double

        Dim Dt As DataTable

        Dim objFattoriInput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_input
        Dim objFattori As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objFattoriOutput As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FattoriCorrettivi_output

        Dim dblMax As Double

        Try

            objFattoriInput.Veg_Cod = Veg_Cod
            objFattoriInput.Grfi_Cod = Grfi_Cod
            objFattoriInput.Variazione = Variazione
            objFattoriInput.Tipo = Tipo
            objFattoriInput.Regolamento_Cod = Regolamento_Cod
            'objFattoriInput.Fattore_Cod = 0 '?????? TO TEST
            objFattoriInput.SoloValorizzati = True
            objFattoriInput.SoloVisibili = False ' ?????? TO TEST

            ' Variazione Massima
            objFattoriOutput = objFattori.FattoriCorrettivi_Leggi(objFattoriInput)

            Dim Lista_Dose As New List(Of AgronicaCorePianoConcimazioneBIZ.FattoreCorrettivo)
            Lista_Dose = objFattoriOutput.ListaFattoriCorrettivi.Where(Function(x) x.Tipo = Tipo And UCase(x.Variazione) = Variazione And x.Visibile = 1 And x.Codice = 47).ToList

            If Not IsNothing(Lista_Dose) Then

                If Lista_Dose.Count > 0 Then

                    dblMax = Lista_Dose(0).Valore.ToString

                End If

            End If

            Dt = Nothing

        Catch ex As Exception

        End Try

        Return dblMax

    End Function

    '####################################################################################################
    Public Sub publicCaricaDosi(ByVal N_Ammesso As Decimal, ByVal P_Ammesso As Decimal, ByVal K_Ammesso As Decimal,
                                ByVal Sup_Tot As Double, ByVal Piva As String, ByVal PCTestataCod As Integer, ByRef rpt As RPT_Scheda_NPK, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri)

        'salvati modalita nuova nel dettaglio del piano
        If Not (N_Ammesso = 0 And P_Ammesso = 0 And K_Ammesso = 0) Then

            CType(rpt.Section1.ReportObjects("TextDoseRicalcolataN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & N_Ammesso.ToString & vbCrLf &
                                                                                                                                         "QTA' TOT AZOTO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * N_Ammesso, "0.0000")


            CType(rpt.Section5.ReportObjects("TextDoseRicalcolataP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & P_Ammesso.ToString & vbCrLf &
                                                                                                                                         "QTA' TOT FOSFORO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * P_Ammesso, "0.0000")


            CType(rpt.Section3.ReportObjects("TextDoseRicalcolataK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & K_Ammesso.ToString & vbCrLf &
                                                                                                                                         "QTA' TOT POTASSIO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * K_Ammesso, "0.0000")

        Else

            'letti modalita vecchia nelle entita

            Dim Dt As DataTable
            Dim objPC_EntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R

            Dt = objPC_EntitaxTestata.Leggi(PCTestataCod,
                                            0,
                                            Piva,
                                            0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "",
                                            enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            If Dt.Rows.Count > 0 Then

                Dim dbl As Double
                dbl = Dt.Rows(0).Item("QtaMaxN")
                CType(rpt.Section1.ReportObjects("TextDoseRicalcolataN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf &
                                                                                                                                             "QTA' TOT AZOTO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

                dbl = Dt.Rows(0).Item("QtaMaxP2O5")
                CType(rpt.Section5.ReportObjects("TextDoseRicalcolataP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf &
                                                                                                                                             "QTA' TOT FOSFORO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

                dbl = Dt.Rows(0).Item("QtaMaxK2O")
                CType(rpt.Section3.ReportObjects("TextDoseRicalcolataK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf &
                                                                                                                                             "QTA' TOT POTASSIO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

            Else

                Dim dbl As Double
                dbl = 0
                CType(rpt.Section1.ReportObjects("TextDoseRicalcolataN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf &
                                                                                                                                             "QTA' TOT AZOTO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

                dbl = 0
                CType(rpt.Section5.ReportObjects("TextDoseRicalcolataP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf &
                                                                                                                                             "QTA' TOT FOSFORO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

                dbl = 0
                CType(rpt.Section3.ReportObjects("TextDoseRicalcolataK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf &
                                                                                                                                             "QTA' TOT POTASSIO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

            End If


        End If

    End Sub

    ''####################################################################################################
    'Private Sub CaricaDosi(ByVal Sup_Tot As Double)

    '    Dim Dt As DataTable
    '    Dim objPC_EntitaxTestata As New AgronicaCoreAnagrafeDAL.PianoConcimazione_EntitaxTestata_R

    '    Dt = objPC_EntitaxTestata.Leggi(Qs_PCTestataCod, _
    '                                    0, _
    '                                    Qs_Piva, _
    '                                    0, 0, 0, 0, 0, "", "", "", 0, 0, "", 0, "", _
    '                                    enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
    '    Dim i As Integer
    '    If Dt.Rows.Count > 0 Then
    '        Dim dbl As Double
    '        dbl = Dt.Rows(0).Item("QtaMaxN")
    '        CType(rptSchedaNPK.Section1.ReportObjects("TextDoseRicalcolataN"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf & _
    '                                                                                                                                     "QTA' TOT AZOTO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

    '        dbl = Dt.Rows(0).Item("QtaMaxP2O5")
    '        CType(rptSchedaNPK.Section5.ReportObjects("TextDoseRicalcolataP"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf & _
    '                                                                                                                                     "QTA' TOT FOSFORO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

    '        dbl = Dt.Rows(0).Item("QtaMaxK2O")
    '        CType(rptSchedaNPK.Section3.ReportObjects("TextDoseRicalcolataK"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "DOSE RICALCOLATA [kg/ha]: " & dbl.ToString & vbCrLf & _
    '                                                                                                                                     "QTA' TOT POTASSIO DISTRIBUIBILE (su " & Format(Sup_Tot, "0.0000") & " ha) [kg]: " & Format(Sup_Tot * dbl, "0.0000")

    '    End If

    'End Sub


    ''################################################################################
    'Private Function SalvaPdf() As String

    '    ' Dichiara le variabili e restituisce le opzioni di esportazione.
    '    Dim exportOpts As New ExportOptions
    '    Dim diskOpts As New DiskFileDestinationOptions

    '    exportOpts = rptSchedaNPK.ExportOptions

    '    ' Imposta il formato di esportazione.
    '    exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
    '    exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

    '    ' Imposta le opzioni relative al file del disco.
    '    Dim strPath As String

    '    Dim PathCartella As String

    '    ' leggo la sottocartella da CategorieDocumenti
    '    Dim Sottocartella As String
    '    Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
    '    Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.PianoConcimazione, "", "", objParametri_Server)
    '    objCatDoc = Nothing

    '    Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
    '    PathCartella = objAgroWeb.GestioneAllegati_Repository & Sottocartella

    '    ' se il path non esiste, creo tutte le cartelle e sottocartelle
    '    If Not System.IO.Directory.Exists(PathCartella) Then
    '        System.IO.Directory.CreateDirectory(PathCartella)
    '    End If

    '    strPath = PathCartella & "\PC_" & Qs_PCTestataCod & "_" & Session.SessionID.ToString & ".pdf"
    '    diskOpts.DiskFileName = strPath
    '    exportOpts.DestinationOptions = diskOpts

    '    ' Esportazione del report.
    '    rptSchedaNPK.Export(exportOpts)

    '    Return strPath

    'End Function


    '################################################################################
    Public Function public_SalvaPdf(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                             ByRef rpt As RPT_Scheda_NPK, _
                             Optional ByRef NomeFile As String = "") As String

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        Dim strPath As String = ""
        Dim PathCartella As String

        Try

            exportOpts = rpt.ExportOptions

            ' Imposta il formato di esportazione.
            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.PianoConcimazione, "", "", objParametri)
            Session("Sottocartella") = Sottocartella    ' mi serve nel piano concimazione massivo
            objCatDoc = Nothing

            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

            If Sottocartella <> "" Then
                PathCartella = objAgroWeb.GestioneAllegati_Repository & Sottocartella
            Else
                If objAgroWeb.GestioneAllegati_Repository.EndsWith("\") Then
                    PathCartella = objAgroWeb.GestioneAllegati_Repository.Substring(0, objAgroWeb.GestioneAllegati_Repository.Length - 1)
                Else
                    PathCartella = objAgroWeb.GestioneAllegati_Repository
                End If

            End If
            PathCartella = objAgroWeb.GestioneAllegati_Repository & Sottocartella

            ' se il path non esiste, creo tutte le cartelle e sottocartelle
            If Not System.IO.Directory.Exists(PathCartella) Then
                System.IO.Directory.CreateDirectory(PathCartella)
            End If

            If NomeFile = "" Then

                NomeFile = CreaNomeFile(Qs_Piva, Qs_PCTestataCod, objParametri)
                'NomeFile = "\PC_" & Qs_PCTestataCod & "_" & Session.SessionID.ToString & ".pdf"
            End If

            strPath = PathCartella & "\" & NomeFile
            diskOpts.DiskFileName = strPath
            exportOpts.DestinationOptions = diskOpts

            ' Esportazione del report.
            rpt.Export()

        Catch ex As Exception
            Log_Errori += "- public_SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
            Return ""
        End Try

        Return strPath

    End Function


    '################################################################################
    Public Function CreaNomeFile(ByVal Piva As String, ByVal PC_TestataCod As Integer,
                                 ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String


        Dim strCentro As String = Session("strCentro")
        Dim strSpecie As String = Session("strSpecie")

        'strCentro = strCentro.Replace(" ", "")
        'strCentro = strCentro.Replace("'", "")
        'strCentro = strCentro.Replace("""", "")
        'strSpecie = strSpecie.Replace(" ", "")
        'strSpecie = strSpecie.Replace("'", "")


        strCentro = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strCentro)
        strSpecie = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(strSpecie)

        '  Galassi, 03/04/2017 09.55.39: Se non ho la specie va in errore la stampa perchè non riesce a trovare il nome del file quindi controllo il strSpecie
        If IsNothing(strSpecie) OrElse strSpecie.Length < 1 Then
            strSpecie = " "
        End If

        '  Galassi, 03/04/2017 09.55.39: per sicurezza lo faccio anche per il centro
        If IsNothing(strCentro) OrElse strCentro.Length < 1 Then
            strCentro = " "
        End If

        Dim CodSocio As String = Trim(Session("Socio"))

        CodSocio = CodSocio.Replace("/", "_")
        CodSocio = CodSocio.Replace("\", "_")
        CodSocio = CodSocio.Replace("|", "_")

        Dim CentroConferimento As String

        Dim objAnagrafe As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
        ' CodSocio = objAnagrafe.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.Codice_Socio, objParametri)

        CentroConferimento = objAnagrafe.Leggi_Codice_from_Imprese_Codici(Piva, enum_CodiciAnagrafe.V01CON00_VCCOIS, objParametri)
        objAnagrafe = Nothing

        Dim NomeFile As String = "PCS" &
                                 IIf(CentroConferimento <> "", "_" & CentroConferimento, "") &
                                 "_" & strCentro.Substring(0, Math.Min(15, strCentro.Length - 1)) &
                                 IIf(CodSocio <> "", "_" & CodSocio, "") &
                                 "_" & strSpecie.Substring(0, Math.Min(15, strSpecie.Length - 1)) &
                                 "_" & PC_TestataCod & ".pdf"

        Return NomeFile


    End Function

End Class