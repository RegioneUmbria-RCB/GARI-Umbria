Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports CrystalDecisions.Shared

Public Class SchedaVenditeBiologico
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_SchedaVenditeBiologico
    Private rptFooterLogo As FooterLogo
    Private DSSchedaVenditeBiologico As DS_SchedaVenditeBiologico

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim Log_Errori As String = ""
    Dim QS_Linea_Classe_Cod As Integer
    Dim QS_Mat_Cod As Integer
    Dim QS_Cod_RisUm As Integer
    Dim Qs_Filtro_ElemCod As String
    Dim Qs_Stampalotto As Integer
    Dim Flag_StampaCodArticolo As Boolean
    Dim Qs_RegioneCod As String
    Dim Qs_MostraData As Boolean
    Dim Qs_MostraFirmaODC As Boolean

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

    Dim TipoReport As Integer

#Region " Codice generato da Progettazione Web Form "

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
        'istanzio l'oggetto report
        rptStampa = New Rpt_SchedaVenditeBiologico
        rptFooterLogo = New FooterLogo
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        If Session("ASG_objParametri_Server") Is Nothing Then
            Response.Redirect("~/Custom500.aspx")
        End If


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################


        QS_DataInizio = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("di")),
                         AgroKey_EncoderDecoder,
                         Server)

        QS_DataFine = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("df")),
                         AgroKey_EncoderDecoder,
                         Server)

        Qs_Piva = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("p")),
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Sa_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("s")),
                           AgroKey_EncoderDecoder,
                           Server)

        Qs_Fabbricato_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("f")),
                           AgroKey_EncoderDecoder,
                           Server)

        Qs_Filtro_ElemCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("fec")),
                    AgroKey_EncoderDecoder,
                    Server)

        QS_Linea_Classe_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lcp")),
                                AgroKey_EncoderDecoder,
                                Server)

        QS_Mat_Cod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("m")),
                            AgroKey_EncoderDecoder,
                            Server)

        QS_Cod_RisUm = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("ru")),
                    AgroKey_EncoderDecoder,
                    Server)

        Qs_Stampalotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sl")),
                               AgroKey_EncoderDecoder,
                               Server))

        Flag_StampaCodArticolo = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("chkca")),
                                  AgroKey_EncoderDecoder,
                                  Server)

        Qs_RegioneCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("rc")),
                                           AgroKey_EncoderDecoder,
                                           Server)

        Qs_MostraData = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("mstdata")),
                                           AgroKey_EncoderDecoder,
                                           Server)

        Qs_MostraFirmaODC = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("odc")),
                                           AgroKey_EncoderDecoder,
                                           Server)

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        '##############################################################
        '#####  Recupero piva, sa_cod e anno dalla stringa xml
        '##############################################################

        Dim rag_soc As String = ""
        Dim sa_nome As String = ""
        Dim ind_des As String = ""
        Dim frz_des As String = ""
        Dim CAP As String = ""
        Dim com_des As String = ""
        Dim pro_cod As String = ""
        Dim pro_cod_istat As String = ""
        Dim com_cod_istat As String = ""
        Dim OrganismoControllo As String = ""

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "SchedaVenditeBiologico"
        Dim NomeFilePDF As String = ""
        Dim IdentificazioneDocumento As String
        Dim reportTemporano As String

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DSSchedaVenditeBiologico As New DS_SchedaVenditeBiologico
            Dim DSLogoFooter As New DS_LogoFooter()

            Try

                'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

                'CrystalReportViewer1.Style.Add("LEFT", "-275px")
                'CrystalReportViewer1.Style.Add("TOP", "0px")
                'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

                Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim pivaReale As String = objImp.Leggi_PivaReale(Qs_Piva, objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = AgronicaCoreStampeDAL.Stampe_QDC.Pulisci_Piva_Fittizia(pivaReale)

                If Qs_Sa_Cod <> "0" Then
                    '-----------------------------------------------
                    'INDIRIZZO DEL CENTRO
                    'Indirizzo_from_PivaSaCod(Server, Session, Page, Qs_Piva, Qs_Sa_Cod, rag_soc, sa_nome, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat)
                    Dim objInd As New AgronicaCoreAnagrafeDAL.CentrixIndirizzi_Read
                    objInd.Indirizzo_from_PivaSaCod2(Qs_Piva, Qs_Sa_Cod, rag_soc, sa_nome, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat, objParametri_Server)
                    CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = sa_nome
                    '-----------------------------------------------
                    'CODICE OPERATORE
                    Dim Codice_Operatore As String
                    Dim objCentricodici As New AgronicaCoreAnagrafeDAL.Centri_Codici_Read
                    Codice_Operatore = objCentricodici.ValCod_from_SaCodIdCod(Qs_Piva,
                                                                        Qs_Sa_Cod,
                                                                        enum_CodiciAnagrafe.CodiceCentro_Attuale,
                                                                        objParametri_Server)
                    CType(rptStampa.Section1.ReportObjects("TextCodice"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Codice_Operatore
                    '-----------------------------------------------
                    'ORGANISMO DI CONTROLLO
                    Dim ODC_sigla As String = ""
                    Dim ODC_des As String = ""
                    Dim ODC_codice As String = ""
                    objCentricodici.OrganismodiControllo_from_PivaSaCod(Qs_Piva,
                                                                        Qs_Sa_Cod,
                                                                        ODC_sigla,
                                                                         ODC_des,
                                                                         ODC_codice,
                                                                         objParametri_Server
                                                                        )

                    If ODC_codice <> "" Then
                        OrganismoControllo = ODC_codice
                    ElseIf ODC_des <> "" Then
                        OrganismoControllo = ODC_des
                    End If
                    '------------------------------------------
                Else
                    '-----------------------------------------------
                    'INDIRIZZO DELL'IMPRESA
                    'Indirizzo_from_Piva(Server, Session, Page, Qs_Piva, rag_soc, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat)
                    Dim objInd As New AgronicaCoreAnagrafeDAL.ImpresexIndirizzi_R
                    objInd.Indirizzo_from_Piva(Qs_Piva, rag_soc, ind_des, frz_des, CAP, com_des, pro_cod, pro_cod_istat, com_cod_istat, objParametri_Server)
                    '-----------------------------------------------
                End If

                Dim objLegale As New AgronicaCoreAnagrafeDAL.Contatti_R
                Dim Legale As String
                Legale = objLegale.LegaleRappresentante_from_PivaImpresa(Qs_Piva,
                                                                            "",
                                                                            objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextResponsabile"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Legale



                Dim CentrixRubrica_Read As New AgronicaCoreAnagrafeDAL.CentrixRubrica_Read
                CType(rptStampa.Section1.ReportObjects("TextTel"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CentrixRubrica_Read.Recupera_Telefono_Centro(Qs_Piva, Qs_Sa_Cod, objParametri_Server)

                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = rag_soc
                CType(rptStampa.Section6.ReportObjects("TextAzienda2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = rag_soc
                CType(rptStampa.Section1.ReportObjects("TextIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ind_des
                CType(rptStampa.Section1.ReportObjects("TextFrazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = frz_des
                CType(rptStampa.Section1.ReportObjects("TextComune"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = com_des
                CType(rptStampa.Section1.ReportObjects("TextProvincia"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = pro_cod
                CType(rptStampa.Section1.ReportObjects("TextCap"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CAP
                CType(rptStampa.Section1.ReportObjects("TxtRegolamentoBIO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descrizione_Regolamento_Bio_STAMPE

                '11/11/2019: la regione si seleziona nel filtro stampa
                'Dim Province As New AgronicaCoreMetaSchemaDAL.Lista_Province_R
                'CType(rptStampa.Section6.ReportObjects("TextRegione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Province.Regione_from_Provincia(pro_cod, "", Nothing, Nothing, objParametri_Server)
                Dim objRegioni As New AgronicaCoreMetaSchemaDAL.Lista_Regioni_R
                CType(rptStampa.Section6.ReportObjects("TextRegione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objRegioni.RegioneDes_from_REG(Qs_RegioneCod, objParametri_Server)

                CType(rptStampa.Section6.ReportObjects("TextAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_DataInizio & " - " & QS_DataFine

                'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
                'If customLoghi IsNot Nothing Then
                '    rptStampa.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
                '    rptStampa.Section5.ReportObjects("Text3").ObjectFormat.EnableSuppress = True
                'End If

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Carica_DSSchedaVenditeBiologico_NEW(DSSchedaVenditeBiologico)


                Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)


                Try

                    '--------------------------------------------
                    ' AGGANCIO DATI
                    '--------------------------------------------
                    'sorgente dati.....
                    rptStampa.SetDataSource(DSSchedaVenditeBiologico)
                    rptFooterLogo.SetDataSource(DSLogoFooter)
                    rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)

                Catch ex As Exception
                    Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
                End Try


                'PARAMETRI
                Dim CUAA As String
                Dim objcodici As New AgronicaCoreAnagrafeDAL.Imprese_Codici_Read
                CUAA = objcodici.Leggi_CUAA(Qs_Piva, objParametri_Server)
                rptStampa.SetParameterValue("CUAA", CUAA)
                rptStampa.SetParameterValue("OrganismoControllo", OrganismoControllo)

                Dim dataOdierna As String = String.Empty
                If Qs_MostraData OrElse (Not IsNothing(Session("MostraDataOdiernaStampa")) AndAlso
                    Session("MostraDataOdiernaStampa") = True) Then
                    dataOdierna = DateTime.Today.ToString("dd/MM/yyyy")
                End If
                rptStampa.SetParameterValue("dataOdierna", dataOdierna)

                '--------------------------------------------
                ' ESTRAZIONE Nr Domanda ACA
                '--------------------------------------------
                Dim regImpiantiRead As New AgronicaCoreAnagrafeDAL.Reg_Impianti_Codici_R
                objParametri_Server.FinestraTemporaleInizio = QS_DataInizio
                objParametri_Server.FinestraTemporaleFine = QS_DataFine
                Dim nrDomandeAca As String() = regImpiantiRead.Leggi_Codici_ACA_from_Contributi(Qs_Piva, Qs_Sa_Cod, 0, 0,
                                                                                                     objParametri_Server)
                objParametri_Server.FinestraTemporaleInizio = AGRODATAINIZIO
                objParametri_Server.FinestraTemporaleFine = AGRODATAFINE
                Dim numeriDomandeAca As String = String.Empty
                rptStampa.ReportHeaderSection3.SectionFormat.EnableSuppress = IsNothing(nrDomandeAca) OrElse (Not nrDomandeAca.Any())
                If Not rptStampa.ReportHeaderSection3.SectionFormat.EnableSuppress Then
                    numeriDomandeAca = String.Join(", ", nrDomandeAca)
                End If
                rptStampa.SetParameterValue("nrDomandeACA", numeriDomandeAca)

                Dim nome_completo = String.Empty
                If Qs_MostraFirmaODC Then
                    Dim utenti_dettaglio_dal = New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
                    nome_completo = utenti_dettaglio_dal.NomeCognome_From_Username(objParametri_Utenti.UtenteUsername, objParametri_Utenti)
                    rptStampa.ReportDefinition.Sections("ReportFooterSection1").SectionFormat.EnableSuppress = False
                End If
                rptStampa.SetParameterValue("Nome_Completo", nome_completo)

            Catch ex As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                If QS_DataInizio <> AGRODATAINIZIO And QS_DataFine <> AGRODATAFINE Then
                    Data_Inizio_Allegati = QS_DataInizio
                    Data_Fine_Allegati = QS_DataFine
                    IdentificazioneDocumento += "_" + Format(QS_DataInizio, "yyyy_MM_dd") + "_" + Format(QS_DataFine, "yyyy_MM_dd")
                    'Else
                    '    Data_Inizio_Allegati = "01/01/" & CStr(Anno)
                    '    Data_Fine_Allegati = "31/12/" & CStr(Anno)
                End If

                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Biologico_Vendite, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                NomeFilePDF = Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf"
                objGestFile.SalvaReportPdf(rptStampa,
                                           enum_CategorieDocumenti.Biologico_Vendite,
                                           Sottocartella,
                                           NomeFilePDF,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva,
                                                                 enum_CategorieDocumenti.Biologico_Vendite,
                                                                 Nome_Documento,
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf",
                                                                 Sottocartella,
                                                                 "", "", "", "",
                                                                 Data_Inizio_Allegati,
                                                                 Data_Fine_Allegati,
                                                                 objParametri_Server)



            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            reportTemporano = CrystalHelper.getFileReportTemporaneo()

            Try
                rptStampa.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            'Dim Path_Errore, Str_Errore_Path As String
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Data inizio = " + CStr(QS_DataInizio) + ", Data fine = " + CStr(QS_DataFine) + ", Piva = " + CStr(Qs_Piva) + ", Sa_Cod = " + CStr(Qs_Sa_Cod) +
                                             vbCrLf + vbCrLf + Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Qs_Piva) + "_" + CStr(Qs_Sa_Cod) & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Biologico",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "SchedaVenditeBiologico.aspx",
                                                 Log_Errori)


            End If
            '-----------------------------------------

            '    CrystalReportViewer1.DisplayToolbar = True

            '    'faccio il databind col visualizzatore dei reports...
            '    CrystalReportViewer1.ReportSource = rptStampa
            '    CrystalReportViewer1.DataBind()

            '    'array di dataset e data table
            '    Dim dsRpt() As DataSet = {DSSchedaVenditeBiologico}

            '    'salvo il report nella sessione
            '    Session("DS") = dsRpt


            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptStampa.SetDataSource(CType(Session("DS")(0), DataSet))

            '        'faccio il databind col visualizzatore dei reports...
            '        CrystalReportViewer1.ReportSource = rptStampa
            '        CrystalReportViewer1.DataBind()

            '    End If

        End If

        '==================================================================

        If DSSchedaVenditeBiologico IsNot Nothing Then
            DSSchedaVenditeBiologico.Dispose()
            DSSchedaVenditeBiologico = Nothing
        End If

        rptStampa.Close()
        rptStampa.Dispose()
        rptStampa = Nothing

        GC.Collect()

        Try
            'Session("Report") = rptStampa
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(NomeFilePDF, AgroKey_EncoderDecoder, Server))

        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try
    End Sub

    '###########################################################################
    Private Sub Verifica_Fattura_Collegata(ByVal Dt_rif As DataTable,
                                            ByVal Id_Agenda_DDT As Integer,
                                            ByVal Numero_DDT As String,
                                            ByVal Data_DDT As String,
                                            ByRef Rif_Fattura As String)

        Dim Numero_DocAllegato, Data_DocAllegato As String
        Numero_DocAllegato = ""
        Data_DocAllegato = ""
        Rif_Fattura = ""

        If Not IsNothing(Dt_rif) AndAlso Dt_rif.Rows.Count > 0 Then

            Dim DrDDT As DataRow()

            DrDDT = Dt_rif.Select(" Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_DDT))

            If Not IsNothing(DrDDT) AndAlso DrDDT.Length > 0 Then

                Numero_DocAllegato = "n." & CStr(DrDDT(0).Item("Doc_Numero_Sin_FATT")) &
                    CStr(DrDDT(0).Item("Doc_Numero_FATT")) &
                    CStr(DrDDT(0).Item("Doc_Numero_Des_FATT"))

                Data_DocAllegato = DrDDT(0).Item("Data_movimento_FATT")
                Rif_Fattura = " (Rif. Fattura " & Numero_DocAllegato & " del " & Data_DocAllegato & ")"

            End If

        End If

    End Sub

    '###########################################################################
    Private Sub Gestione_Lotto(ByVal moduliCliente As List(Of Integer),
                               ByVal Elem_Cod As Integer,
                               ByVal Pro_Cod As Integer,
                               ByVal Mat_Cod As Integer,
                               ByVal Lotto As String,
                               ByRef Nome_Prodotto As String)

        If Mat_Cod <> 0 Then

            '----------------------------------
            'GESTIONE LOTTO PRODOTTI
            If Qs_Stampalotto = 1 Then
                'STAMPA IN BASE A CONFIGURAZIONE SU PRODOTTO
                Dim DettagliLotto As String = ""
                Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva, Elem_Cod, Mat_Cod, Lotto, moduliCliente, objParametri_Server)

                If DettagliLotto <> "" Then
                    Nome_Prodotto += " " + DettagliLotto
                End If

            Else
                'STAMPA SEMPRE
                If Lotto <> "" AndAlso Lotto.ToLower <> "indefinito" Then
                    Nome_Prodotto += "Lotto: " & Lotto
                End If
            End If
            '----------------------------------
        ElseIf Pro_Cod <> 0 Then
            If Lotto.ToLower <> "indefinito" AndAlso Lotto <> "" Then
                Nome_Prodotto &= "Lotto: " & Lotto
            End If
        End If


    End Sub

    '###########################################################################
    Private Sub Carica_DSSchedaVenditeBiologico_NEW(ByRef DSSchedaVenditeBiologico As DS_SchedaVenditeBiologico)

        Dim objBio As New AgronicaCoreStampeDAL.SchedaVenditeBio
        Dim objCodiciContatto As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R
        Dim DT As DataTable
        Dim i As Integer
        Dim DrR As DS_SchedaVenditeBiologico.DS_SchedaVenditeBiologicoRow
        Dim Acquirente, Numero_Doc, Indirizzo As String
        Dim Cessionario_Diverso As String = String.Empty
        Dim Indirizzo_Cess_Diverso As String = String.Empty
        Dim Cod_Contatto_Cess_Diverso As String = String.Empty
        Dim Vet_DatiDoc() As String
        Dim xFiltroAggiuntivo As String
        Dim Lav_Cod As Integer
        Dim tempSa_Cod As Integer = 0
        Dim tempFabbricato_Cod As Integer = 0
        Dim cod_contatto, c_piva, c_codficale As String
        Dim Flag_PersonaPrivato As Boolean

        '  Galassi, 02/03/2017 10.53.12: 
        'Se ho Valorizzato il magazzino, filtro per magazzino e centro aziendale!!
        If Not IsNothing(Qs_Fabbricato_Cod) AndAlso Qs_Fabbricato_Cod <> "" AndAlso IsNumeric(Qs_Fabbricato_Cod) Then
            If Not IsNothing(Qs_Sa_Cod) AndAlso Qs_Sa_Cod <> "" AndAlso IsNumeric(Qs_Sa_Cod) Then
                tempSa_Cod = CInt(Qs_Sa_Cod)
                tempFabbricato_Cod = CInt(Qs_Fabbricato_Cod)
            End If
        End If

        Try

            If Qs_Filtro_ElemCod <> "" Then
                xFiltroAggiuntivo = " AND Movimenti_Dettagli.Elem_Cod  IN  " + Qs_Filtro_ElemCod
            End If

            If QS_Linea_Classe_Cod <> 0 Then
                xFiltroAggiuntivo += " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
            End If

            'ma va fatto il filtro sul Qs_Sa_Cod?
            'ha senso se c'è anche fabbricato_cod
            DT = objBio.LeggiVendite(Qs_Piva,
                                        tempSa_Cod,
                                        tempFabbricato_Cod,
                                        QS_Mat_Cod,
                                        QS_Cod_RisUm,
                                        QS_DataInizio, QS_DataFine,
                                        xFiltroAggiuntivo,
                                        "",
                                        objParametri_Server)



            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim DT_Rif As DataTable
                Dim rif_fattura As String = ""
                Dim filtro_fatt As String
                Dim lotto As String

                filtro_fatt = " Mov_Dettagli_Riferimenti.Lav_Cod IN ( " +
                                            CStr(LAVCOD_FATTURA_EMESSA) + "," +
                                            CStr(LAVCOD_FATTURA_LIQ_CONF_EMESSA) + "," +
                                            CStr(LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA) +
                                            ")"

                DT_Rif = objRif.Leggi_DDT_Nota_aggancio_Fattura(Qs_Piva,
                                                                0,
                                                                0,
                                                                0,
                                                                 0,
                                                                 0,
                                                                 "",
                                                                 Qs_Piva,
                                                                 0,
                                                                 0, 0, 0,
                                                                 LAVCOD_BOLLA_EMESSA,
                                                                 "",
                                                                 filtro_fatt, "",
                                                                 objParametri_Server)

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                For i = 0 To DT.Rows.Count - 1

                    '---------------------------------------------------------------------
                    DrR = DSSchedaVenditeBiologico.DS_SchedaVenditeBiologico.NewDS_SchedaVenditeBiologicoRow
                    '---------------------------------------------------------------------

                    With DT.Rows(i)

                        rif_fattura = ""

                        Lav_Cod = .Item("Lav_Cod")

                        DrR.Data = CDate(.Item("Data_Movimento")).ToShortDateString
                        DrR.Prodotto = .Item("mat_des")

                        If Flag_StampaCodArticolo = True Then
                            DrR.Etichetta = .Item("Etichetta")
                        Else
                            DrR.Etichetta = ""
                        End If

                        lotto = ""
                        Gestione_Lotto(
                            moduliCliente,
                            .Item("elem_cod"),
                            .Item("pro_cod"),
                            .Item("mat_cod"),
                            .Item("lotto"),
                            lotto)

                        'nel caso di configurazione lotto (usata dalle cantine)
                        'visualizzo il lotto nella desc del prodotto (visto che ci sarà l'anno di produzione)
                        If Qs_Stampalotto = 1 Then
                            DrR.Prodotto &= lotto
                        Else
                            'nel caso di visualizza sempre il lotto (giasonline)
                            'visualizzo il lotto nella colonna etichetta
                            If DrR.Etichetta <> "" Then
                                DrR.Etichetta &= " - "
                            End If
                            DrR.Etichetta &= lotto
                        End If

                        DrR.Udm_Sim = .Item("Udm_Sim")
                        DrR.Qta = .Item("Qta")

                        If DT.Rows(i).Item("Dati_Documento") <> "" Then
                            Vet_DatiDoc = CStr(DT.Rows(i).Item("Dati_Documento")).Split("|")
                            '----------------------------------------------------------
                            'Letture aggiuntive per ricavare operazioni collegate
                            Select Case Lav_Cod
                                Case LAVCOD_AUTOCONSUMO, LAVCOD_AUTOCONSUMO_VINO_SFUSO
                                    Acquirente = ""
                                    Numero_Doc = "autoconsumo"
                                    '-------------------------
                                Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO
                                    Acquirente = ""
                                    Numero_Doc = "Corrispettivo n." + Vet_DatiDoc(0)
                                    '-------------------------
                                Case LAVCOD_FATTURA_EMESSA, LAVCOD_FATTURA_LIQ_CONF_EMESSA, LAVCOD_AUTOFATTURA_LIQ_CONF_RICEVUTA '(immediata)
                                    Numero_Doc = "Fattura n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '------------------------
                                Case LAVCOD_RICEVUTA_EMESSA
                                    Numero_Doc = "Ricevuta n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case LAVCOD_BOLLA_EMESSA, LAVCOD_DDT_CONTABILIZZATO_EMESSO

                                    Numero_Doc = Trim("DDT " & CStr(Vet_DatiDoc(6)).ToLower) & " n." & Vet_DatiDoc(0)
                                    'leggere se c'è fattura allegata
                                    'Verifica_Fattura_Collegata(DT_Rif, _
                                    '                            .Item("Id_agenda"), _
                                    '                            Numero_Doc, _
                                    '                            DrR.Data, _
                                    '                            Numero_Doc, _
                                    '                            DrR.Data)
                                    Verifica_Fattura_Collegata(DT_Rif,
                                                       .Item("Id_agenda"),
                                                       Numero_Doc,
                                                       DrR.Data,
                                                      rif_fattura)
                                    If rif_fattura <> "" Then
                                        Numero_Doc &= rif_fattura
                                    End If

                                    Acquirente = Vet_DatiDoc(1)
                                    If Vet_DatiDoc.Count > 7 Then
                                        Cessionario_Diverso = Vet_DatiDoc(7)
                                    End If

                                    '-------------------------
                                Case LAVCOD_DOCO_EMESSO
                                    Numero_Doc = "DOCO n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case LAVCOD_MVV_EMESSO
                                    Numero_Doc = "MVV n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case LAVCOD_DAA_EMESSO
                                    Numero_Doc = "DAA n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case LAVCOD_CONFERIMENTO_DIVERSI 'like agrisfera/gentili
                                    'leggere se c'è accettazione per aggiornare la qta
                                    'leggere se c'è fattura
                                    Numero_Doc = "DDT n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case LAVCOD_CONFERIMENTO 'like agribologna
                                    Numero_Doc = "DDT n." + Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case LAVCOD_ACCETTAZIONE_DIVERSI 'like fruttagel
                                    Numero_Doc = "DDT n." + Vet_DatiDoc(0) '+ " (Rif. Bolla Accett. n." + Numero_Doc_Allegato + " del " + Data_Movimento_Allegato + ")"
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                                Case Else
                                    Numero_Doc = Vet_DatiDoc(0)
                                    Acquirente = Vet_DatiDoc(1)
                                    '-------------------------
                            End Select

                            If InStr(Vet_DatiDoc(4).ToLower, "non definita") > 0 Then
                                Indirizzo = ""
                            Else
                                Indirizzo = Vet_DatiDoc(4)
                            End If

                            If Vet_DatiDoc.Count > 7 Then
                                If InStr(Vet_DatiDoc(9).ToLower, "non definita") > 0 Then
                                    Indirizzo_Cess_Diverso = ""
                                Else
                                    Indirizzo_Cess_Diverso = Vet_DatiDoc(9)
                                End If
                            End If

                            If Vet_DatiDoc.Count > 7 Then
                                Cod_Contatto_Cess_Diverso = Vet_DatiDoc(10)
                            End If

                            DrR.Doc_Numero = Numero_Doc

                            If String.IsNullOrEmpty(Cessionario_Diverso) Then
                                DrR.Acquirente = Trim(Acquirente)
                            Else
                                DrR.Acquirente = String.Format("{0}{1}{2}", Acquirente, vbCrLf & "---------------" & vbCrLf, Cessionario_Diverso)
                            End If

                            If String.IsNullOrEmpty(Indirizzo_Cess_Diverso) Then
                                DrR.Indirizzo = Trim(Indirizzo)
                            Else
                                DrR.Indirizzo = String.Format("{0}{1}{2}", Indirizzo, vbCrLf & "---------------" & vbCrLf, Indirizzo_Cess_Diverso)
                            End If


                            cod_contatto = Vet_DatiDoc(2)
                            c_piva = ""
                            c_codficale = ""
                            Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(cod_contatto, Vet_DatiDoc(5), c_piva, c_codficale, Flag_PersonaPrivato)

                            If Flag_PersonaPrivato = True Then
                                If String.IsNullOrEmpty(Cod_Contatto_Cess_Diverso) Then
                                    DrR.Cod_Fisc = Trim(c_codficale)
                                Else
                                    DrR.Cod_Fisc = String.Format("{0}{1}{2}", c_codficale, vbCrLf & vbCrLf & "---------------" & vbCrLf & vbCrLf, Cod_Contatto_Cess_Diverso)
                                End If
                            Else
                                If c_codficale <> "" Then
                                    If String.IsNullOrEmpty(Cod_Contatto_Cess_Diverso) Then
                                        DrR.Cod_Fisc = Trim(c_codficale)
                                    Else
                                        DrR.Cod_Fisc = String.Format("{0}{1}{2}", c_codficale, vbCrLf & vbCrLf & "---------------" & vbCrLf & vbCrLf, Cod_Contatto_Cess_Diverso)
                                    End If
                                Else 'piva
                                    If String.IsNullOrEmpty(Cod_Contatto_Cess_Diverso) Then
                                        DrR.Cod_Fisc = Trim(c_piva)
                                    Else
                                        DrR.Cod_Fisc = String.Format("{0}{1}{2}", c_piva, vbCrLf & vbCrLf & "---------------" & vbCrLf & vbCrLf, Cod_Contatto_Cess_Diverso)
                                    End If
                                End If
                            End If

                            'CONCATENO IL cod_OperatoreBIO
                            Dim operatBIO As DataTable = objCodiciContatto.LeggiOperatBioContatto_daCod_Contatto(Qs_Piva, DrR.Cod_Fisc, objParametri_Server)
                            If Not IsNothing(operatBIO) AndAlso operatBIO.Rows.Count > 0 Then
                                DrR.Cod_Fisc &= " - " & operatBIO.Rows(0).Item("Organismo_Sigla") & " " & operatBIO.Rows(0).Item("Codice")
                            End If
                            DrR.Qualifica = Vet_DatiDoc(3)
                        Else
                            Select Case Lav_Cod
                                Case LAVCOD_SCARICO
                                    Select Case .Item("pendente")
                                        Case enum_Pendenza.Smaltimento
                                            DrR.Doc_Numero = "smaltimento / perdita di lavorazione"
                                        Case enum_Pendenza.Furto, 22
                                            DrR.Doc_Numero = "scarico a seguito di furto"
                                        Case enum_Pendenza.ScaricoFuoriRegione
                                            DrR.Doc_Numero = "utilizzo prodotto fuori regione"
                                        Case enum_Pendenza.ResoFornitore
                                            DrR.Doc_Numero = "reso a fornitore"
                                        Case enum_Pendenza.Rottura
                                            DrR.Doc_Numero = "scarico per rottura"
                                        Case enum_Pendenza.Altra_Pendenza
                                            DrR.Doc_Numero = .Item("des_lib")
                                        Case enum_Pendenza.AutoConsumo
                                            DrR.Doc_Numero = "autoconsumo"
                                    End Select
                                    DrR.Acquirente = ""
                                    DrR.Indirizzo = ""
                                    DrR.Cod_Fisc = ""
                                    DrR.Qualifica = ""
                                Case LAVCOD_VENDITA, LAVCOD_CORRISPETTIVO_VENDITA_SFUSO
                                    DrR.Doc_Numero = "Corrispettivo di vendita"
                                    DrR.Acquirente = ""
                                    DrR.Indirizzo = ""
                                    DrR.Cod_Fisc = ""
                                    DrR.Qualifica = ""
                                Case LAVCOD_TRASFERIMENTO
                                    DrR.Doc_Numero = "trasferimento di magazzino"
                                    DrR.Acquirente = ""
                                    DrR.Indirizzo = ""
                                    DrR.Cod_Fisc = ""
                                    DrR.Qualifica = ""
                                Case Else
                                    DrR.Doc_Numero = ""
                                    DrR.Acquirente = ""
                                    DrR.Indirizzo = ""
                                    DrR.Cod_Fisc = ""
                                    DrR.Qualifica = ""
                            End Select
                        End If 'Dati_Documento

                        Select Case LCase(DrR.Qualifica)
                            Case "produttore agricolo"
                                DrR.Qualifica = "PA"
                            Case "produttore industriale"
                                DrR.Qualifica = "PI"
                            Case "dettagliante"
                                DrR.Qualifica = "DE"
                            Case "Grossista"
                                DrR.Qualifica = "G"
                            Case "distributore"
                                DrR.Qualifica = "DI"
                            Case "importatore"
                                DrR.Qualifica = "I"
                            Case "esportatore"
                                DrR.Qualifica = "E"
                            Case "trasformatore"
                                DrR.Qualifica = "T"
                            Case "consumatore finale"
                                DrR.Qualifica = "CF"
                            Case "condizionatore"
                                DrR.Qualifica = "CO"
                            Case "industria di conservazione", "industria conservazione"
                                DrR.Qualifica = "IC"
                            Case "altro"
                                DrR.Qualifica = "AA"
                            Case Else
                                DrR.Qualifica = ""
                        End Select

                    End With

                    '---------------------------------------------------------------------
                    DSSchedaVenditeBiologico.DS_SchedaVenditeBiologico.Rows.Add(DrR)
                    '---------------------------------------------------------------------

                Next

                DSSchedaVenditeBiologico.DS_SchedaVenditeBiologico.AcceptChanges()

            End If

        Catch ex As Exception
            Log_Errori += "- LETTURA MOVIMENTI: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub

End Class
