Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente

Public Class SchedaMateriePrimeBiologico
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_SchedaMateriePrimeBiologico
    Private rptFooterLogo As FooterLogo
    Private DSSchedaMateriePrimeBiologico As DS_SchedaMateriePrimeBiologico

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim Qs_Filtro_ElemCod As String
    Dim QS_Linea_Classe_Cod As Integer
    Dim Flag_StampaConsistenzeVasca As Boolean
    Dim Flag_StampaCodArticolo As Boolean
    Dim Qs_arrotonda As String
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
        rptStampa = New Rpt_SchedaMateriePrimeBiologico
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

        Flag_StampaConsistenzeVasca = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("vas")),
                            AgroKey_EncoderDecoder,
                            Server)

        Flag_StampaCodArticolo = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("chkca")),
                            AgroKey_EncoderDecoder,
                            Server)
        Qs_MostraData = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("mstdata")),
                                           AgroKey_EncoderDecoder,
                                           Server)

        Qs_MostraFirmaODC = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("odc")),
                                           AgroKey_EncoderDecoder,
                                           Server)

        If Not IsNothing(Request.QueryString("arr")) AndAlso Stringa_Decodifica(Request.QueryString("arr").ToString,
                            AgroKey_EncoderDecoder,
                            Server) <> "" Then
            Qs_arrotonda = Stringa_Decodifica(Request.QueryString("arr").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)
        Else
            Qs_arrotonda = "-1"
        End If

        Qs_RegioneCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("rc")),
                                              AgroKey_EncoderDecoder,
                                              Server)

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        '##############################################################
        '#####  Recupero piva, sa_cod e anno dalla stringa xml
        '##############################################################

        Dim Log_Errori As String = ""

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

        Dim Nome_Documento As String = "SchedaMateriePrimeBiologico"
        Dim NomeFilePDF As String = ""
        Dim IdentificazioneDocumento As String

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DSSchedaMateriePrimeBiologico As New DS_SchedaMateriePrimeBiologico
            Dim DSLogoFooter As New DS_LogoFooter

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

                Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

                rptFooterLogo.SetDataSource(DSLogoFooter)
                rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)
                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Carica_DSSchedaMateriePrimeBiologico_NEW(DSSchedaMateriePrimeBiologico, Log_Errori)

                Try

                    '--------------------------------------------
                    ' AGGANCIO DATI
                    '--------------------------------------------
                    'sorgente dati.....
                    rptStampa.SetDataSource(DSSchedaMateriePrimeBiologico)

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
                rptStampa.SetParameterValue("nome_completo", nome_completo)


            Catch ex As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
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
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Biologico_MateriePrime, "", "", objParametri_Server)

                NomeFilePDF = Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf"

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptStampa,
                                           enum_CategorieDocumenti.Biologico_MateriePrime,
                                           Sottocartella,
                                           NomeFilePDF,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva,
                                                                 enum_CategorieDocumenti.Biologico_MateriePrime,
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

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Str_Errore_Path, Nome_File As String


            '-----------------------------------------
            Try
                rptStampa.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Data inizio = " + CStr(QS_DataInizio) + ", Data fine = " + CStr(QS_DataFine) + ", Piva = " + CStr(Qs_Piva) + ", Sa_Cod = " + CStr(Qs_Sa_Cod) +
                                             vbCrLf + vbCrLf + Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Qs_Piva) + "_" + CStr(Qs_Sa_Cod)

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_BIO", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Biologico",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "SchedaMateriePrimeBiologico.aspx",
                                                 Log_Errori)

            End If

            DSSchedaMateriePrimeBiologico.Dispose()
            DSSchedaMateriePrimeBiologico = Nothing
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
        End If
    End Sub


    '###########################################################################
    Private Sub Carica_DSSchedaMateriePrimeBiologico_NEW(ByRef DSSchedaMateriePrimeBiologico As DS_SchedaMateriePrimeBiologico, _
                                                            ByRef Log_Errori As String)


        Dim RigaDs As DS_SchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologicoRow
        Dim i As Integer
        Dim Str_Formulati As String = ""
        Dim Str_Fertilizzanti As String = ""
        Dim Str_MateriePrime As String = ""
        Dim xFiltroAggiuntivo As String = ""
        Dim tempSa_Cod As Integer = 0
        Dim tempFabbricato_Cod As Integer = 0

        Dim DataGiornoPrima As Date
        DataGiornoPrima = DateAdd(DateInterval.Day, -1, CDate(QS_DataInizio))


        '  Galassi, 02/03/2017 10.53.12: 
        'Se ho Valorizzato il magazzino, filtro per magazzino e centro aziendale!!
        If Not IsNothing(Qs_Fabbricato_Cod) AndAlso Qs_Fabbricato_Cod <> "" AndAlso IsNumeric(Qs_Fabbricato_Cod) Then
            If Not IsNothing(Qs_Sa_Cod) AndAlso Qs_Sa_Cod <> "" AndAlso IsNumeric(Qs_Sa_Cod) Then
                tempSa_Cod = CInt(Qs_Sa_Cod)
                tempFabbricato_Cod = CInt(Qs_Fabbricato_Cod)
            End If
        End If

        Try

            If Flag_StampaConsistenzeVasca = True Then

                '-------------------------------------------------------------------------------------------------
                ' GIACENZE INIZIALI DI VASCA
                '-------------------------------------------------------------------------------------------------

                Dim objConsistenze As New AgronicaCoreStampeDAL.DocCantina
                Dim DT_Cons As DataTable

                'Dim Filtro_ElemCod As String
                'Dim xFiltroAggiuntivo_7 As String = ""
                'Dim xFiltroAggiuntivo_9 As String = ""
                'Dim xFiltroAggiuntivo_10 As String = ""

                'If Qs_Filtro_ElemCod <> "" Then
                '    Filtro_ElemCod = " AND Movimenti_Dettagli.Elem_Cod  IN  " + Qs_Filtro_ElemCod
                'End If

                'If QS_Linea_Classe_Cod <> 0 Then
                '    xFiltroAggiuntivo_7 = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                '    xFiltroAggiuntivo_9 = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                '    xFiltroAggiuntivo_10 = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                '    xFiltroAggiuntivo = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                'End If

                DT_Cons = objConsistenze.ConsistenzeEnologiche(Qs_Piva, _
                                                                 0, _
                                                                 0, _
                                                                 0, 0, _
                                                                 0, 0, _
                                                                 DataGiornoPrima, _
                                                                 True, _
                                                                  False, _
                                                                 enum_OrdinamentoStampaConsEnologiche.LineaProduttiva, _
                                                                 "", "", _
                                                                 objParametri_Server)

                If Not IsNothing(DT_Cons) AndAlso DT_Cons.Rows.Count > 0 Then

                    'Leggo i moduli installati
                    Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                    Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                    Dim Consistenza As Decimal

                    For i = 0 To DT_Cons.Rows.Count - 1

                        Consistenza = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal(DT_Cons.Rows(i).Item("Qta"), CInt(Qs_arrotonda))
                        Select Case Qs_arrotonda
                            Case -1, 4
                                Consistenza = Format(Consistenza, "###,##0.0000")
                            Case 0
                                Consistenza = Format(Consistenza, "###,##0")
                            Case 1
                                Consistenza = Format(Consistenza, "###,##0.0")
                            Case 2
                                Consistenza = Format(Consistenza, "###,##0.00")
                            Case 3
                                Consistenza = Format(Consistenza, "###,##0.000")
                        End Select


                        If Consistenza <> 0 And Not (Consistenza < QTA_GiancenzeVisualizzate And Consistenza > -QTA_GiancenzeVisualizzate) Then

                            '---------------------------------------------------------------------
                            RigaDs = DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.NewDS_SchedaMateriePrimeBiologicoRow
                            '---------------------------------------------------------------------

                            RigaDs.Id_Agenda = 0 'metto 0 perchè non ho l'id_agenda dalla query

                            RigaDs.Sa_Cod = 0 'DT_Cons.Rows(i).Item("Sa_Cod") nella query non c'è, tanto non serve

                            RigaDs.Data = QS_DataInizio
                            RigaDs.Data_Movimento = CDate(QS_DataInizio)

                            RigaDs.Elem_Cod = DT_Cons.Rows(i).Item("Elem_Cod")

                            RigaDs.Pro_Cod = DT_Cons.Rows(i).Item("Pro_Cod")
                            RigaDs.Mat_Cod = DT_Cons.Rows(i).Item("Mat_Cod")

                            If Not IsNothing(DT_Cons.Rows(i).Item("LineaProduttiva").split("§")(1)) Then
                                RigaDs.Regolamento = DT_Cons.Rows(i).Item("LineaProduttiva").split("§")(1)
                            Else
                                RigaDs.Regolamento = enum_Cod_Regolamento.Regolamento_Nessuno
                            End If

                            Select Case RigaDs.Regolamento

                                Case enum_Cod_Regolamento.Regolamento_bio
                                    RigaDs.Qta_Conv = "0"
                                    RigaDs.Qta_Bio = Consistenza
                                Case Else
                                    RigaDs.Qta_Bio = "0"
                                    RigaDs.Qta_Conv = Consistenza
                            End Select

                            RigaDs.Prodotto = DT_Cons.Rows(i).Item("mat_des")

                            Gestione_Lotto(moduliCliente,
                                DT_Cons.Rows(i).Item("elem_cod"),
                                DT_Cons.Rows(i).Item("pro_cod"),
                                DT_Cons.Rows(i).Item("mat_cod"),
                                DT_Cons.Rows(i).Item("lotto"),
                                RigaDs.Prodotto)

                            RigaDs.Udm_Cod = DT_Cons.Rows(i).Item("Udm_Cod")
                            RigaDs.Udm_Sim = DT_Cons.Rows(i).Item("Udm_Sim")

                            'If DT_Cons.Rows(i).Item("Cod_Articolo") <> "" Then
                            '    RigaDs.Etichetta = "Cod." & DT_Cons.Rows(i).Item("Cod_Articolo")
                            'Else
                            '    RigaDs.Etichetta = ""
                            'End If
                            RigaDs.Etichetta = ""

                            RigaDs.Doc_Numero = 0
                            RigaDs.Fornitore = ""
                            RigaDs.Indirizzo = ""
                            RigaDs.Cod_Fisc = ""
                            RigaDs.Qualifica = ""

                            '---------------------------------------------------------------------
                            DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.Rows.Add(RigaDs)
                            '---------------------------------------------------------------------
                        End If

                    Next

                End If

            End If 'chk


        Catch ex As Exception
            Log_Errori += "- LETTURA CONSISTENZE DI VASCA: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################
        DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.AcceptChanges()
        '#########################################################



        Try


            '-------------------------------------------------------------------------------------------------
            ' GIACENZE INIZIALI DI MAGAZZINO
            '-------------------------------------------------------------------------------------------------

            Dim objGiacenze As New AgronicaCoreStampeDAL.Magazzino
            Dim DT_Giacenze As DataTable
            Dim Filtro_ElemCod As String
            Dim xFiltroAggiuntivo_7 As String = ""
            Dim xFiltroAggiuntivo_9 As String = ""
            Dim xFiltroAggiuntivo_10 As String = ""

            ''ci vuole l'AND
            'Filtro_ElemCod = " AND Movimenti_Dettagli.Elem_Cod  IN ( " + _
            '                   CStr(FERTILIZZANTI) + "," + _
            '                   CStr(FORMULATI) + "," + _
            '                    CStr(MATERIE_VEGETALI) + "," + _
            '                    CStr(MATERIE_ANIMALI) + "," + _
            '                   CStr(SEMENTI) + "" + _
            '                   " ) "

            If Qs_Filtro_ElemCod <> "" Then
                Filtro_ElemCod = " AND Movimenti_Dettagli.Elem_Cod  IN  " + Qs_Filtro_ElemCod
            End If

            If QS_Linea_Classe_Cod <> 0 Then
                xFiltroAggiuntivo_7 = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                xFiltroAggiuntivo_9 = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                xFiltroAggiuntivo_10 = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
                xFiltroAggiuntivo = " AND (Materie_Prime.Cat_Cod = " + Agro_SQL_SaveNum(QS_Linea_Classe_Cod) + ") "
            End If

            'CStr(ALTRE_MATERIE) + "," + _
            '   CStr(COADIUVANTI) + _

            'ma va fatto il filtro sul Qs_Sa_Cod?
            'ha senso se c'è anche fabbricato_cod

            'CORRETTO BUG 01/03/2013:
            'non devo passare QS_DataInizio, ma il giorno prima
            'altrimenti se esattamente in data QS_DataInizio sono state fatte operazioni,
            'i carichi vengono conteggiati due volte
            'Dim DataGiornoPrima As Date
            'DataGiornoPrima = DateAdd(DateInterval.Day, -1, CDate(QS_DataInizio))


            DT_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataGiornoPrima, _
                                                             Qs_Piva, _
                                                             tempSa_Cod, _
                                                             tempFabbricato_Cod, _
                                                             0, 0, 0, 0, 0, 0, 0, _
                                                             LOTTO_NONDEFINITO, _
                                                             True, _
                                                             Filtro_ElemCod, _
                                                             "", "", "", "", "", "", _
                                                             xFiltroAggiuntivo_7, _
                                                             "", _
                                                             xFiltroAggiuntivo_9, _
                                                             xFiltroAggiuntivo_10, _
                                                             "", "", _
                                                             objParametri_Server, objParametri_Utenti)

            If Not IsNothing(DT_Giacenze) AndAlso DT_Giacenze.Rows.Count > 0 Then

                Dim Giacenza As Decimal

                For i = 0 To DT_Giacenze.Rows.Count - 1

                    '  Giacenza = Format(DT_Giacenze.Rows(i).Item("Giacenza"), "###,##0.####")
                    Giacenza = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal(DT_Giacenze.Rows(i).Item("Giacenza"), CInt(Qs_arrotonda))

                    If Giacenza <> 0 And Not (Giacenza < QTA_GiancenzeVisualizzate And Giacenza > -QTA_GiancenzeVisualizzate) Then

                        '---------------------------------------------------------------------
                        RigaDs = DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.NewDS_SchedaMateriePrimeBiologicoRow
                        '---------------------------------------------------------------------

                        RigaDs.Id_Agenda = 0 'metto 0 perchè non ho l'id_agenda dalla query

                        RigaDs.Sa_Cod = DT_Giacenze.Rows(i).Item("Sa_Cod")

                        RigaDs.Data = QS_DataInizio
                        RigaDs.Data_Movimento = CDate(QS_DataInizio)

                        RigaDs.Elem_Cod = DT_Giacenze.Rows(i).Item("Elem_Cod")

                        RigaDs.Pro_Cod = DT_Giacenze.Rows(i).Item("Pro_Cod")
                        RigaDs.Mat_Cod = DT_Giacenze.Rows(i).Item("Mat_Cod")

                        RigaDs.Regolamento = 0

                        Select Case RigaDs.Elem_Cod
                            Case FERTILIZZANTI
                                Str_Fertilizzanti &= CStr(RigaDs.Pro_Cod) & ","
                                RigaDs.Prodotto = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")

                            Case FORMULATI
                                Str_Formulati &= CStr(RigaDs.Pro_Cod) & ","
                                RigaDs.Prodotto = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto") & " (Num.Reg." & CStr(RigaDs.Pro_Cod) & ")"

                            Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, MATERIE_ANIMALI, SEMILAVORATI_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI,
                                 SEMILAVORATI_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI
                                Str_MateriePrime &= CStr(RigaDs.Mat_Cod) & ","
                                RigaDs.Prodotto = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")

                            Case Else
                                RigaDs.Prodotto = DT_Giacenze.Rows(i).Item("Descrizione_Prodotto")
                        End Select

                        RigaDs.Udm_Cod = DT_Giacenze.Rows(i).Item("Udm_Cod")
                        RigaDs.Udm_Sim = DT_Giacenze.Rows(i).Item("Udm_Sim")

                        If Flag_StampaCodArticolo = True Then
                            If DT_Giacenze.Rows(i).Item("Cod_Articolo") <> "" Then
                                RigaDs.Etichetta = "Cod." & DT_Giacenze.Rows(i).Item("Cod_Articolo")
                            Else
                                RigaDs.Etichetta = ""
                            End If
                        Else
                            RigaDs.Etichetta = ""
                        End If

                        If CStr(DT_Giacenze.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DT_Giacenze.Rows(i).Item("Lotto") <> "" Then
                            If RigaDs.Etichetta <> "" Then
                                RigaDs.Etichetta &= " - "
                            End If
                            RigaDs.Etichetta &= "Lotto: " + DT_Giacenze.Rows(i).Item("Lotto")
                        End If

                        RigaDs.Doc_Numero = 0
                        RigaDs.Fornitore = ""
                        RigaDs.Indirizzo = ""
                        RigaDs.Cod_Fisc = ""
                        RigaDs.Qualifica = ""

                        RigaDs.Qta_Conv = "0"

                        Select Case RigaDs.Udm_Cod
                            Case enum_UnitaMisura.Numero, enum_UnitaMisura.Num_Piante,
                                enum_UnitaMisura.Numero_Inneschi, enum_UnitaMisura.Numero_Trappole
                                RigaDs.Qta_Bio = Format(Giacenza, "###,##0")
                            Case Else
                                Select Case Qs_arrotonda
                                    Case -1, 4
                                        RigaDs.Qta_Bio = Format(Giacenza, "###,##0.0000")
                                    Case 0
                                        RigaDs.Qta_Bio = Format(Giacenza, "###,##0")
                                    Case 1
                                        RigaDs.Qta_Bio = Format(Giacenza, "###,##0.0")
                                    Case 2
                                        RigaDs.Qta_Bio = Format(Giacenza, "###,##0.00")
                                    Case 3
                                        RigaDs.Qta_Bio = Format(Giacenza, "###,##0.000")
                                End Select

                        End Select

                        '04/03/2020: patch, spostato l'add row qui altrimenti veniva generata eccezione 
                        '(se si finiva nell'else per giacenza scartata post query, tentava di aggiungere la riga aggiunta al giro precedente)
                        '---------------------------------------------------------------------
                        DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.Rows.Add(RigaDs)
                        '---------------------------------------------------------------------
                    Else
                        Dim debug As Boolean = True
                    End If 'giacenza

                Next

            End If


        Catch ex As Exception
            Log_Errori += "- LETTURA GIACENZE: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################
        DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.AcceptChanges()
        '#########################################################

        Try

            Dim objContab As New AgronicaCoreContabDAL.Contabilita_R
            Dim DT As DataTable
            Dim objCarichi As New AgronicaCoreStampeDAL.SchedaMateriePrimeBio
            Dim objCodiciContatto As New AgronicaCoreAnagrafeDAL.Contatti_Codici_R
            Dim Vet_DatiDoc() As String
            Dim FiltroAgg As String
            Dim cod_contatto, c_piva, c_codficale As String
            Dim Flag_PersonaPrivato As Boolean
            Dim Lav_Cod As Integer
            Dim Numero_Doc As String
            Dim Qta As Decimal

            If Qs_Filtro_ElemCod <> "" Then
                FiltroAgg = "  Movimenti_Dettagli.Elem_Cod  IN  " & Qs_Filtro_ElemCod
            End If
            FiltroAgg &= xFiltroAggiuntivo

            'ma va fatto il filtro sul Qs_Sa_Cod?
            'ha senso se c'è anche fabbricato_cod
            DT = objCarichi.LeggiMovimentiCarico(Qs_Piva, _
                                                    tempSa_Cod, _
                                                    tempFabbricato_Cod, _
                                                    QS_DataInizio, _
                                                    QS_DataFine, _
                                                    FiltroAgg, "", _
                                                    objParametri_Server)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Dim objRif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim DT_Rif As DataTable
                Dim rif_fattura As String = ""
                Dim filtro_fatt As String
                Dim numeroResi As Integer

                filtro_fatt = " Mov_Dettagli_Riferimenti.Lav_Cod IN ( " & CStr(LAVCOD_FATTURA_RICEVUTA) & ")"

                DT_Rif = objRif.Leggi_DDT_Nota_aggancio_Fattura(Qs_Piva, _
                                                                0, _
                                                                0, _
                                                                0, _
                                                                 0, _
                                                                 0, _
                                                                 "", _
                                                                 Qs_Piva, _
                                                                 0, _
                                                                 0, 0, 0, _
                                                                 LAVCOD_BOLLA_RICEVUTA, _
                                                                 "", _
                                                                 filtro_fatt, "", _
                                                                 objParametri_Server)


                For i = 0 To DT.Rows.Count - 1

                    ''NON SERVE PIU' L'IF, VIENE FILTRATO DIRETTAMENTE DA QUERY
                    ''se l'elem_cod letto è nel filtro selezionato
                    'If InStr(Qs_Filtro_ElemCod, CStr(DT.Rows(i).Item("Elem_Cod"))) > 0 Then

                    'Select Case DT.Rows(i).Item("Elem_Cod")

                    '    'filtro i record che mi servono:
                    '    'Elem_Cod --> 0=Dati Bolla; 3=Fertilizzanti; 10=Sementi; 191=Formulati, ecc.
                    '    'Case 0, FERTILIZZANTI, FORMULATI, SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, MATERIE_ANIMALI, COADIUVANTI
                    'Case 0, FERTILIZZANTI, FORMULATI, SEMENTI, MATERIE_VEGETALI, MATERIE_ANIMALI

                    '---------------------------------------------------------------------
                    RigaDs = DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.NewDS_SchedaMateriePrimeBiologicoRow
                    '---------------------------------------------------------------------

                    Lav_Cod = DT.Rows(i).Item("Lav_Cod")

                    RigaDs.Id_Agenda = DT.Rows(i).Item("Id_Agenda")
                    RigaDs.Sa_Cod = DT.Rows(i).Item("Sa_Cod")
                    RigaDs.Data_Movimento = DT.Rows(i).Item("Data_Movimento")
                    RigaDs.Cau_Mov = DT.Rows(i).Item("Cau_Mov")

                    RigaDs.Lav_Cod = DT.Rows(i).Item("Lav_Cod")

                    RigaDs.Elem_Cod = DT.Rows(i).Item("Elem_Cod")

                    RigaDs.Mat_Cod = DT.Rows(i).Item("Mat_Cod")
                    RigaDs.Pro_Cod = DT.Rows(i).Item("Pro_Cod")

                    Select Case RigaDs.Elem_Cod

                        Case FERTILIZZANTI
                            Str_Fertilizzanti &= CStr(RigaDs.Pro_Cod) & ","

                            If RigaDs.Mat_Cod <> 0 Then
                                RigaDs.Prodotto = DT.Rows(i).Item("Mat_Des")
                            Else
                                RigaDs.Prodotto = DT.Rows(i).Item("Fer_Des")
                            End If

                        Case FORMULATI
                            Str_Formulati &= CStr(RigaDs.Pro_Cod) & ","
                            RigaDs.Prodotto = DT.Rows(i).Item("Fr_Des") & " (Num.Reg." + CStr(RigaDs.Pro_Cod) + ")"

                        Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, MATERIE_ANIMALI, SEMILAVORATI_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI, _
                                SEMILAVORATI_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI

                            Str_MateriePrime &= CStr(RigaDs.Mat_Cod) & ","

                            RigaDs.Prodotto = DT.Rows(i).Item("Mat_Des")

                        Case COADIUVANTI, INSETTI, TRAPPOLE, INNESCHI, MANGIMI, FARMACI
                            RigaDs.Prodotto = objContab.LeggiProdotto(objParametri_Server, _
                                                                         Nothing, _
                                                                         Qs_Piva, _
                                                                         RigaDs.Elem_Cod, _
                                                                         RigaDs.Pro_Cod, _
                                                                         RigaDs.Mat_Cod, _
                                                                         )

                    End Select


                    RigaDs.Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                    RigaDs.Udm_Sim = DT.Rows(i).Item("Udm_Sim")

                    Qta = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal(DT.Rows(i).Item("Qta"), CInt(Qs_arrotonda))

                    Select Case RigaDs.Udm_Cod
                        Case enum_UnitaMisura.Numero, enum_UnitaMisura.Num_Piante, _
                            enum_UnitaMisura.Numero_Inneschi, enum_UnitaMisura.Numero_Trappole
                            RigaDs.Qta_Bio = Format(Qta, "###,##0")
                        Case Else
                            Select Case Qs_arrotonda
                                Case -1, 4
                                    RigaDs.Qta_Bio = Format(Qta, "###,##0.0000")
                                Case 0
                                    RigaDs.Qta_Bio = Format(Qta, "###,##0")
                                Case 1
                                    RigaDs.Qta_Bio = Format(Qta, "###,##0.0")
                                Case 2
                                    RigaDs.Qta_Bio = Format(Qta, "###,##0.00")
                                Case 3
                                    RigaDs.Qta_Bio = Format(Qta, "###,##0.000")
                            End Select
                    End Select

                    RigaDs.Qta_Conv = "0"


                    If Flag_StampaCodArticolo = True Then
                        If DT.Rows(i).Item("Etichetta") <> "" Then
                            RigaDs.Etichetta = "Cod." & DT.Rows(i).Item("Etichetta")
                        Else
                            RigaDs.Etichetta = ""
                        End If
                    Else
                        RigaDs.Etichetta = ""
                    End If

                    If CStr(DT.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DT.Rows(i).Item("Lotto") <> "" Then
                        If RigaDs.Etichetta <> "" Then
                            RigaDs.Etichetta &= " - "
                        End If
                        RigaDs.Etichetta &= "Lotto: " + DT.Rows(i).Item("Lotto")
                    End If

                    If DT.Rows(i).Item("Dati_Documento") <> "" Then
                        Vet_DatiDoc = CStr(DT.Rows(i).Item("Dati_Documento")).Split("|")

                        RigaDs.Fornitore = Trim(Vet_DatiDoc(1))

                        Vet_DatiDoc(4) = Replace(Vet_DatiDoc(4), "&nbsp;", "")
                        Vet_DatiDoc(4) = Replace(Vet_DatiDoc(4), "Non Definita", "")
                        Vet_DatiDoc(4) = Replace(Vet_DatiDoc(4), "(00)", "")
                        RigaDs.Indirizzo = Trim(Vet_DatiDoc(4))

                        Dim preNumeroDoc As String = ""
                        Dim sufNumeroDoc As String = ""
                        Select Case Lav_Cod

                            Case LAVCOD_FATTURA_RICEVUTA '(immediata)
                                preNumeroDoc = "Fattura"
                                '-------------------------
                            Case LAVCOD_ACQUISTO
                                preNumeroDoc = "Corrispettivo"
                                '-------------------------
                            Case LAVCOD_DOCO_RICEVUTO
                                preNumeroDoc = "DOCO"
                                '-------------------------
                            Case LAVCOD_MVV_RICEVUTO
                                preNumeroDoc = "MVV"
                                '-------------------------
                            Case LAVCOD_BOLLA_RICEVUTA
                                preNumeroDoc = "DDT"
                                'leggere se c'è fattura allegata
                                Verifica_Fattura_Collegata(DT_Rif,
                                                   DT.Rows(i).Item("Id_agenda"),
                                                  sufNumeroDoc)
                                '-------------------------
                            Case LAVCOD_BOLLA_EMESSA, LAVCOD_NOTA_ACCREDITO_EMESSA, LAVCOD_NOTA_ACCREDITO_RICEVUTA, LAVCOD_FATTURA_EMESSA

                                If Lav_Cod = LAVCOD_BOLLA_EMESSA Then
                                    preNumeroDoc = "DDT"
                                ElseIf Lav_Cod = LAVCOD_FATTURA_EMESSA Then
                                    preNumeroDoc = "Fattura"
                                Else
                                    preNumeroDoc = "NDC"
                                End If

                                Dim noteDoc As String = DT.Rows(i).Item("Extra_Str")

                                sufNumeroDoc = "Reso" & If(String.IsNullOrWhiteSpace(noteDoc), "", " - " & noteDoc)

                                numeroResi += 1
                                '-------------------------
                        End Select

                        RigaDs.Doc_Numero = preNumeroDoc & " n." & Vet_DatiDoc(0) & " " & sufNumeroDoc

                        'RigaDs.Cod_Fisc = Vet_DatiDoc(2)
                        cod_contatto = Vet_DatiDoc(2)
                        c_piva = ""
                        c_codficale = ""
                        Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(cod_contatto, Vet_DatiDoc(5), c_piva, c_codficale, Flag_PersonaPrivato)

                        If Flag_PersonaPrivato = True Then
                            RigaDs.Cod_Fisc = c_codficale
                        Else
                            If c_codficale <> "" Then
                                RigaDs.Cod_Fisc = c_codficale
                            Else
                                RigaDs.Cod_Fisc = c_piva
                            End If
                        End If

                        'CONCATENO IL cod_OperatoreBIO
                        Dim operatBIO As DataTable = objCodiciContatto.LeggiOperatBioContatto_daCod_Contatto(Qs_Piva, RigaDs.Cod_Fisc, objParametri_Server)
                        If Not IsNothing(operatBIO) AndAlso operatBIO.Rows.Count > 0 Then
                            RigaDs.Cod_Fisc &= " - " + operatBIO.Rows(0).Item("Organismo_Sigla") + " " + operatBIO.Rows(0).Item("Codice")
                        End If
                        'FINE cod_BIO

                        RigaDs.Qualifica = Vet_DatiDoc(3)
                    Else
                        Select Case Lav_Cod
                            Case LAVCOD_CARICO
                                RigaDs.Doc_Numero = "carico di magazzino"
                                '-------------------------
                            Case LAVCOD_TRASFERIMENTO
                                RigaDs.Doc_Numero = "trasferimento di magazzino " &
                                    If(DT.Rows(i).Item("Cau_Mov") = CAU_CARICO, "(carico)",
                                        If(DT.Rows(i).Item("Cau_Mov") = CAU_SCARICO, "(scarico)", ""))
                                '-------------------------
                            Case Else
                                RigaDs.Doc_Numero = ""
                        End Select
                        RigaDs.Fornitore = ""
                        RigaDs.Indirizzo = ""
                        RigaDs.Cod_Fisc = ""
                        RigaDs.Qualifica = ""
                    End If

                    'RigaDs.Doc_Numero = DT.Rows(i).Item("Doc_Numero")
                    'RigaDs.Fornitore = DT.Rows(i).Item("Fornitore")
                    'RigaDs.Indirizzo = DT.Rows(i).Item("Indirizzo")
                    'RigaDs.Cod_Fisc = DT.Rows(i).Item("Cod_Fisc")
                    'RigaDs.Qualifica = DT.Rows(i).Item("Qualifica")


                    RigaDs.Regolamento = 0

                    Select Case LCase(RigaDs.Qualifica)
                        Case "produttore agricolo"
                            RigaDs.Qualifica = "PA"
                        Case "produttore industriale"
                            RigaDs.Qualifica = "PI"
                        Case "dettagliante"
                            RigaDs.Qualifica = "DE"
                        Case "Grossista"
                            RigaDs.Qualifica = "G"
                        Case "distributore"
                            RigaDs.Qualifica = "DI"
                        Case "importatore"
                            RigaDs.Qualifica = "I"
                        Case "esportatore"
                            RigaDs.Qualifica = "E"
                        Case "trasformatore"
                            RigaDs.Qualifica = "T"
                        Case "consumatore finale"
                            RigaDs.Qualifica = "CF"
                        Case "condizionatore"
                            RigaDs.Qualifica = "CO"
                        Case "industria di conservazione", "industria conservazione"
                            RigaDs.Qualifica = "IC"
                        Case "altro"
                            RigaDs.Qualifica = "AA"
                        Case Else
                            RigaDs.Qualifica = ""
                    End Select

                    '---------------------------------------------------------------------
                    DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.Rows.Add(RigaDs)
                    '---------------------------------------------------------------------

                    'End Select
                    ' End If

                Next

                If numeroResi > 0 Then
                    CType(rptStampa.Section1.ReportObjects("TextIntestazioneProdotto"),
                        CrystalDecisions.CrystalReports.Engine.TextObject).Text = "PRODOTTO ACQUISITO / RESO" 'Normalmente il titolo è "PRODOTTO ACQUISITO"
                End If

            End If

        Catch ex As Exception
            Log_Errori += "- LETTURA MOVIMENTI: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################
        DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.AcceptChanges()
        '#########################################################

        Try

            '-----------------------------------------------------------
            ' Recupero l'informazione se il prodotto è bio o no
            '-----------------------------------------------------------

            Dim HT_Fert As Hashtable
            Dim DT_Fito As DataTable
            Dim HT_MP As Hashtable

            Try

                If Str_Fertilizzanti <> "" Then
                    'tolgo l'ultima virgola
                    Str_Fertilizzanti = Left(Str_Fertilizzanti, Str_Fertilizzanti.Length - 1)

                    Dim objFert As New AgronicaCoreMetaSchemaDAL.RegolamentixFertilizza_R

                    'chiamo il core che mi legge se i fertilizzanti sono bio o no
                    HT_Fert = objFert.BIO_1_0_from_ElencoFertilizzanti( _
                                        Str_Fertilizzanti, _
                                        enum_Cod_Regolamento.Regolamento_bio, _
                                         False, _
                                        "", _
                                        "", _
                                        objParametri_Server)

                End If

                If Str_Formulati <> "" Then
                    'tolgo l'ultima virgola
                    Str_Formulati = Left(Str_Formulati, Str_Formulati.Length - 1)

                    'chiamo il ws che mi legge se i fito sono bio o no
                    Dim objDPI As New AgronicaCoreDpiBIZ.Fitofarmaci_Leggi

                    DT_Fito = objDPI.Formulati_Bio(Str_Formulati, "", _
                                            Session("ASG_ProgressivoGIAS"), _
                                            Session("ASG_SuperUser_Password"), _
                                            objParametri_Utenti)

                End If

                If Str_MateriePrime <> "" Then
                    'tolgo l'ultima virgola
                    Str_MateriePrime = Left(Str_MateriePrime, Str_MateriePrime.Length - 1)

                    'chiamo il core che mi legge se le materie prime sono bio o no
                    Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R

                    'chiamo il core che mi legge se le mp sono bio o no
                    HT_MP = objMP.BIO_1_0_from_ElencoMateriePrime( _
                                        Str_MateriePrime, _
                                        enum_Cod_Regolamento.Regolamento_bio, _
                                        False, _
                                        "", _
                                        "", _
                                        objParametri_Server)


                End If


            Catch ex As Exception
                Log_Errori += "- LETTURA PRODOTTI BIO: " + vbCrLf + ex.Message + vbCrLf
            End Try



            'ciclo su tutte le righe del dataset per inserire il resto dei dati e formattarne altri
            For i = 0 To DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.Rows.Count - 1

                Dim drR As DS_SchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologicoRow = DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.Rows(i)

                drR.Data = Left(CStr(drR.Data_Movimento), 10)

                'controllo se l'elemento è convenzionale o biologico...
                Select Case drR.Elem_Cod

                    Case FERTILIZZANTI
                        Qta_Bio_Conv_from_HT(HT_Fert, drR.Pro_Cod, drR)

                    Case FORMULATI
                        Qta_Bio_Conv_from_DT(DT_Fito, drR.Pro_Cod, drR)

                    Case SEMENTI, ALTRE_MATERIE, MATERIE_VEGETALI, MATERIE_ANIMALI, SEMILAVORATI_VEGETALI, BENI_CONFEZ_VEGETALE, TRASFORMATI_VEGETALI, _
                            SEMILAVORATI_ANIMALI, BENI_CONFEZ_ANIMALE, TRASFORMATI_ANIMALI
                        Qta_Bio_Conv_from_HT(HT_MP, drR.Mat_Cod, drR)

                End Select

                ''NON SERVE PIU' PERCHE' LE GIACENZE VENGONO GIA' FILTRATE SULLO 0
                'E IL SA_COD VIENE FILTRATO IN ENTRAMBE LE QUERY
                ''cancello le giacenze = 0 (elementi movimentati ma nn più presenti)
                ''e i dati relativi ad ALTRI CENTRI AZIENDALI
                'If (drR.Qta_Bio = "0" And drR.Qta_Conv = "0") Or _
                '    drR.Sa_Cod <> CInt(Qs_Sa_Cod) Then
                '    drR.Delete()
                'End If

            Next

            '#########################################################
            DSSchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologico.AcceptChanges()
            '#########################################################


        Catch ex As Exception
            Log_Errori += "- ELABORAZIONE DATASET: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub


    '###########################################################
    Private Sub Qta_Bio_Conv_from_HT(ByVal HT As Hashtable, _
                                    ByVal Codice As Integer, _
                                        ByRef drR As DS_SchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologicoRow)

        Try
            Dim temp As Decimal
            Dim debug As Boolean

            If Not IsNothing(HT) Then

                If HT.Contains(Codice) Then

                    Select Case HT(Codice)

                        Case 1 'è BIO
                            'la qta è già stata impostata come bio, lascio quindi così com'è
                            temp = drR.Qta_Bio
                            debug = True
                        Case Else
                            'il prodotto non è bio
                            drR.Qta_Conv = drR.Qta_Bio
                            drR.Qta_Bio = "0"
                    End Select
                Else
                    'il prodotto non è nel risultato
                    'quindi il prodotto non è bio,
                    'lo imposto come convenzionale
                    drR.Qta_Conv = drR.Qta_Bio
                    drR.Qta_Bio = "0"
                End If
            Else
                'l'ht è vuoto
                'quindi non so se il prodotto è bio,
                'lo imposto come convenzionale
                drR.Qta_Conv = drR.Qta_Bio
                drR.Qta_Bio = "0"
            End If

        Catch ex As Exception
            'errore
            drR.Qta_Conv = drR.Qta_Bio
            drR.Qta_Bio = "0"
        End Try


    End Sub

    '###########################################################
    Private Sub Qta_Bio_Conv_from_DT(ByVal DT As DataTable, _
                                    ByVal Codice As Integer, _
                                        ByRef drR As DS_SchedaMateriePrimeBiologico.DS_SchedaMateriePrimeBiologicoRow)

        Try
            Dim temp As Decimal
            Dim debug As Boolean

            If Not IsNothing(DT) Then

                Dim Vet_Dr() As DataRow
                Dim str_select As String = "fr_cod = " + CStr(Codice)

                Vet_Dr = DT.Select(str_select)

                If Not IsNothing(Vet_Dr) AndAlso Not IsNothing(Vet_Dr(0)) Then

                    Select Case Vet_Dr(0).Item("bio")

                        Case 1 'è BIO
                            'la qta è già stata impostata come bio, lascio quindi così com'è
                            temp = drR.Qta_Bio
                            debug = True
                        Case Else
                            'il prodotto non è bio
                            drR.Qta_Conv = drR.Qta_Bio
                            drR.Qta_Bio = "0"

                    End Select

                Else
                    'il prodotto non è nel risultato
                    'quindi il prodotto non è bio,
                    'lo imposto come convenzionale
                    drR.Qta_Conv = drR.Qta_Bio
                    drR.Qta_Bio = "0"
                End If

            Else
                'il dt è vuoto
                'quindi non so se il prodotto è bio,
                'lo imposto come convenzionale
                drR.Qta_Conv = drR.Qta_Bio
                drR.Qta_Bio = "0"
            End If

        Catch ex As Exception
            'errore
            drR.Qta_Conv = drR.Qta_Bio
            drR.Qta_Bio = "0"
        End Try


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
            Dim DettagliLotto As String = ""
            Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

            DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva, Elem_Cod, Mat_Cod, Lotto, moduliCliente, objParametri_Server)

            If DettagliLotto <> "" Then
                Nome_Prodotto += " " + DettagliLotto
            End If
            '----------------------------------
        ElseIf Pro_Cod <> 0 Then
            If Lotto.ToLower <> "indefinito" AndAlso Lotto <> "" Then
                Nome_Prodotto &= " Lotto: " & Lotto
            End If
        End If


    End Sub

    '###########################################################################
    Private Sub Verifica_Fattura_Collegata(ByVal Dt_rif As DataTable,
                                            ByVal Id_Agenda_DDT As Integer,
                                            ByRef Rif_Fattura As String)

        Dim Numero_DocAllegato, Data_DocAllegato As String
        Rif_Fattura = ""

        If Not IsNothing(Dt_rif) AndAlso Dt_rif.Rows.Count > 0 Then

            Dim DrDDT As DataRow()

            DrDDT = Dt_rif.Select(" Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda_DDT))

            If Not IsNothing(DrDDT) AndAlso DrDDT.Length > 0 Then

                Numero_DocAllegato = "n." & CStr(DrDDT(0).Item("Doc_Numero_Sin_FATT")) &
                    CStr(DrDDT(0).Item("Doc_Numero_FATT")) &
                    CStr(DrDDT(0).Item("Doc_Numero_Des_FATT"))

                Data_DocAllegato = DrDDT(0).Item("Data_movimento_FATT")
                Rif_Fattura = "(Rif. Fattura " & Numero_DocAllegato & " del " & Data_DocAllegato & ")"

            End If

        End If

    End Sub

End Class
