Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaCoreUtility
Imports CrystalDecisions.Shared

Partial Class SchedaProdottiFitosanitariMagazzino

    Inherits System.Web.UI.Page
    Private rptStampa As Rpt_SchedaProdottiFitosanitariMagazzino
    Private rptFooterLogo As FooterLogo
    Private DSProdottiFitosanitariMagazzino As DS_ProdottiFitosanitariMagazzino
    Private DSLogoFooter As New DS_LogoFooter

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim Qs_Arrotondamento As Integer
    'Dim Qs_Ordinamento As Integer
    Dim Qs_FlagVisualizzaComposizione As Integer


    Dim Matrice(0, 0) As Object
    Dim Num_Prodotti As Integer


    Dim LinkPaginaStampa As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

#Region " Web Form Designer Generated Code "

    'This call is required by the Web Form Designer.
    <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

    End Sub

    'NOTE: The following placeholder declaration is required by the Web Form Designer.
    'Do not delete or move it.
    Private designerPlaceholderDeclaration As System.Object

    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
        'CODEGEN: This method call is required by the Web Form Designer
        'Do not modify it using the code editor.
        InitializeComponent()
        'istanzio l'oggetto report
        rptStampa = New Rpt_SchedaProdottiFitosanitariMagazzino
        rptFooterLogo = New FooterLogo
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

        '#################################################################################
        '#####  Recupero i valori dalla QueryString 
        '#################################################################################

        QS_DataInizio = Stringa_Decodifica(Request.QueryString("dI").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        QS_DataFine = Stringa_Decodifica(Request.QueryString("dF").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString,
                            AgroKey_EncoderDecoder,
                            Server)

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        'Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString, _
        '            AgroKey_EncoderDecoder, _
        '            Server)

        'Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString, _
        '            AgroKey_EncoderDecoder, _
        '            Server)

        'Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString, _
        '            AgroKey_EncoderDecoder, _
        '            Server)

        Qs_Arrotondamento = Stringa_Decodifica(Request.QueryString("arr").ToString,
                 AgroKey_EncoderDecoder,
                 Server)

        'Qs_Ordinamento = Stringa_Decodifica(Request.QueryString("ord").ToString, _
        '      AgroKey_EncoderDecoder, _
        '      Server)

        Qs_FlagVisualizzaComposizione = Stringa_Decodifica(Request.QueryString("fvc").ToString,
                        AgroKey_EncoderDecoder,
                        Server)



        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        ''##############################################################
        ''#####  Recupero piva e sa_cod dalla stringa xml
        ''##############################################################

        'Dim strXmlVariabilistampe As String
        'Dim htVariabiliStampe As System.Collections.Hashtable
        'Dim strErr As String

        'Dim XmlDoc As New System.Xml.XmlDocument
        'Dim XML_FiltroStampa As System.Xml.XmlElement

        'strXmlVariabilistampe = Session("strXmlVariabilistampe")

        ''Carico la stringa xml in un nuovo documento
        'XmlDoc = New System.Xml.XmlDocument
        'XmlDoc.LoadXml(strXmlVariabilistampe)

        'If XmlDoc.HasChildNodes Then

        '    XML_FiltroStampa = XmlDoc.SelectSingleNode("FiltroStampa")

        '    'Ricavo i parametri che servono

        '    Session("ASG_Utente_Username") = XML_FiltroStampa.GetAttribute("username")

        '    If Not IsNothing(XML_FiltroStampa.GetAttribute("arrotondamento")) Then
        '        If XML_FiltroStampa.GetAttribute("arrotondamento") <> "" Then
        '            Qs_Arrotondamento = XML_FiltroStampa.GetAttribute("arrotondamento")
        '        Else
        '            Qs_Arrotondamento = 5
        '        End If
        '    Else
        '        Qs_Arrotondamento = 5
        '    End If

        '    XML_EstraiVariabiliStampe(strXmlVariabilistampe, htVariabiliStampe, strErr)

        '    Qs_Piva = CStr(htVariabiliStampe("piva"))
        '    Qs_Sa_Cod = CStr(htVariabiliStampe("sa_cod"))
        '    Qs_Fabbricato_Cod = CStr(htVariabiliStampe("fabbricato_cod"))


        'End If


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            Dim i As Integer

            CrystalReportViewer1.Style.Add("LEFT", "-275px")
            CrystalReportViewer1.Style.Add("TOP", "0px")
            CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Dim objFabbr As New AgronicaCoreAnagrafeDAL.Fabbricati_R
            Dim a As New AgronicaCoreAnagrafeDAL.Imprese_Read
            Dim RSc As String = a.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
            CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = RSc
            CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
            CType(rptStampa.Section1.ReportObjects("TextMagazzino"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objFabbr.FabbricatoDes_from_FabbricatoCod(Qs_Piva, Qs_Sa_Cod, Qs_Fabbricato_Cod, objParametri_Server)

            CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_DataInizio & " - " & QS_DataFine


            'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
            'If customLoghi IsNot Nothing Then
            '    rptStampa.Section7.ReportObjects("Text13").ObjectFormat.EnableSuppress = True
            '    rptStampa.Section7.ReportObjects("Picture1").ObjectFormat.EnableSuppress = True
            '    rptStampa.Section7.ReportObjects("Text14").ObjectFormat.EnableSuppress = True
            'End If

            VisualizzaLogoRegione()

            MostraRiferimentiRegolamentiAggiornati()

            Dim DSProdottiFitosanitariMagazzino As New DS_ProdottiFitosanitariMagazzino

            'carico i dati nel datatable 
            Carica_DSProdottiFitosanitariMagazzino_NEW(DSProdottiFitosanitariMagazzino)

            Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
            Dim Log_Errori As String = ""
            Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
            If logo.LogoStampe IsNot Nothing Then
                drLogo.Logo = logo.LogoStampe
                drLogo.TestoPostLogo = logo.TestoPostLogo
                drLogo.TestoPreLogo = logo.TestoPreLogo
            End If
            DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

            'sorgente dati.....
            rptStampa.SetDataSource(DSProdottiFitosanitariMagazzino)
            rptFooterLogo.SetDataSource(DSLogoFooter)
            rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)

            'faccio il databind col visualizzatore dei reports...
            CrystalReportViewer1.ReportSource = rptStampa
            CrystalReportViewer1.DataBind()

            'array di dataset e data table
            Dim dsRpt() As DataSet = {DSProdottiFitosanitariMagazzino}

            'salvo il report nella sessione
            Session("DS") = dsRpt


        Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            If Not IsNothing(Session("DS")) Then

                'imposto la sorgente dati x il report...
                rptStampa.SetDataSource(CType(Session("DS")(0), DataSet))

                'faccio il databind col visualizzatore dei reports...
                CrystalReportViewer1.ReportSource = rptStampa
                CrystalReportViewer1.DataBind()

            End If

        End If

        Dim nomeFile

        'MS Session("Report") = rptStampa
        Try
            Dim catCod As Integer = enum_CategorieDocumenti.Magazzino_Fitosanitari
            Dim identificazioneDocumento = "FitosanitariMagazzino" & "_p" & Qs_Piva & "_" & Format(Date.Now, "yyyy_MM_dd")
            nomeFile = Stringhe.EliminaCaratteriSpecialiFile(identificazioneDocumento) & ".pdf"

            ' leggo la sotto cartella da CategorieDocumenti
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Dim sottoCartella As String = objCatDoc.Sottocartella(catCod, "", "", objParametri_Server)
            objCatDoc = Nothing

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptStampa, catCod,
                               sottoCartella, nomeFile,
                               objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
        Catch ex As Exception
            'MS TODO Manca gestione log errori quindi commento.
            'Log_Errori += "- Salvataggio report PDF: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
        Try
            rptStampa.SaveAs(reportTemporano, True)
        Catch ex As Exception
            'MS TODO Manca gestione log errori quindi commento.
            'Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
        End Try


        'MS Dispose del report una volta salvato su disco.
        rptStampa.Close()
        rptStampa.Dispose()
        rptStampa = Nothing

        GC.Collect()

        Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                          "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                          "&NomePdf=" & Stringa_Codifica(nomeFile, AgroKey_EncoderDecoder, Server))

    End Sub


    '###########################################################################
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 05/03/2012
    'a Buzzi di agrisol dava fastidio che un carico di magazzino (rilievo giacenza) fatto alla data di inizio del filtro
    'venisse visualizzato nella colonna qta acquistata e non giacenza iniziale
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 31/03/2009
    'altrimenti comparivano in scheda anche i prodotti entrati in magazzino dopo l'intervallo temporale selezionato
    Private Sub Carica_DSProdottiFitosanitariMagazzino_NEW(ByRef DSProdottiFitosanitariMagazzino As DS_ProdottiFitosanitariMagazzino)



        Dim objSQL As New AgronicaCoreDataProvider.DatatableUtility
        Dim strErr As String
        Dim sSql As String
        Dim i As Integer

        Dim Qta As Decimal
        Dim Giacenza_Iniziale As String
        Dim Giacenza_Finale As String

        Dim PrimaVolta As Boolean = True
        Dim strFr_Cod As String = ""
        'Dim strMat_Cod As String = ""

        Dim Giacenza As Decimal
        Dim Count As Integer = 0

        Dim objGiacenze As New AgronicaCoreStampeDAL.Magazzino
        Dim DT_Giacenze As DataTable
        Dim DataFiltro_GiornoPrima As Date
        Dim DTComposizioneFormulati As DataTable
        Dim DTComposizioneFormulati2 As DataTable


        DataFiltro_GiornoPrima = CDate(QS_DataInizio).AddDays(-1)

        'lettura giacenze di tutti i formulati al giorno prima della data inizio del filtro
        DT_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltro_GiornoPrima,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        True,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)

        Dim DT_Rilievi As DataTable
        Dim FiltroRilievi As String
        Dim IdAgendaRilieviDaEscludere As String

        FiltroRilievi = " AND ( Agenda.Lav_Cod =" + Agro_SQL_SaveNum(LAVCOD_CARICO) +
                                " AND  Movimenti_Dettagli.Pendente =" + Agro_SQL_SaveNum(enum_Pendenza.GiacenzeIniziali) +
                                ")"

        'cerco i rilievi giacenze di tutti i formulati registrati alla data di inizio del filtro
        DT_Rilievi = objGiacenze.SchedaMovimentiMagazzino(QS_DataInizio,
                                                        QS_DataInizio,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        FiltroRilievi,
                                                        "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)

        FiltroRilievi = ""
        If Not IsNothing(DT_Rilievi) AndAlso DT_Rilievi.Rows.Count > 0 Then
            Dim id_agenda As Integer
            For i = 0 To DT_Rilievi.Rows.Count - 1
                id_agenda = DT_Rilievi.Rows(i).Item("Id_Agenda")
                FiltroRilievi += CStr(id_agenda) + ","
            Next
            FiltroRilievi = "(" + Left(FiltroRilievi, FiltroRilievi.Length - 1) + ")"
        End If

        'giacenze di tutti i formulati alla data di fine del filtro
        Dim Dt_Finali As DataTable
        Dt_Finali = objGiacenze.SchedaGiacenzeMagazzino(QS_DataFine,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        False,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)


        'NOTA
        'Tolgo la condizione perche' le Classi Tossicologiche vanno visualizzate sempre
        'If Qs_FlagVisualizzaComposizione = 1 Then
        DTComposizioneFormulati = SchedeMagazzinoHelper.ComposizioneFormulatiRecupera(objParametri_Server,
                                                                                        Dt_Finali,
                                                                                        Server, Session, Page)
        'End If

        Dim HT_ProCod_Carichi As New Hashtable

        'Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto          

        sSql = ""
        'sSql += " SELECT DISTINCT "
        sSql += " SELECT  "
        sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Formulati.Fr_Des, '') AS Fr_Des, '' AS MAt_Des, "
        sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento "

        sSql += " FROM    Agenda "
        sSql += " INNER JOIN Movimenti "
        sSql += " ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda"
        sSql += " INNER JOIN Movimenti_dettagli "
        sSql += " ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov "
        sSql += " INNER JOIN Mov_Destinazioni "
        sSql += " ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod "
        sSql += " AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov "
        sSql += " AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det "
        sSql += " INNER JOIN CategorieMagazzino ON Movimenti_dettagli.Elem_Cod = CategorieMagazzino.Elem_Cod "
        sSql += " INNER JOIN UnitaMisura ON Movimenti_dettagli.Udm_Cod = UnitaMisura.Udm_Cod "
        sSql += " INNER JOIN Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod "

        'sSql += " FROM         Materie_Prime RIGHT OUTER JOIN"
        'sSql += "               UnitaMisura INNER JOIN"
        'sSql += "               Mov_Destinazioni INNER JOIN"
        'sSql += "               CategorieMagazzino INNER JOIN"
        'sSql += "               Movimenti_dettagli ON CategorieMagazzino.Elem_Cod = Movimenti_dettagli.Elem_Cod INNER JOIN"
        'sSql += "               Movimenti INNER JOIN"
        'sSql += "               Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Id_Agenda = Agenda.Id_Agenda ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND "
        'sSql += "               Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ON "
        'sSql += "               Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND "
        'sSql += "               Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND "
        'sSql += "               Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ON UnitaMisura.UDM_COD = Movimenti_dettagli.Udm_Cod ON "
        'sSql += "               Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod LEFT OUTER JOIN"
        'sSql += "               Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod "

        sSql += " WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(QS_DataFine)
        sSql += " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(QS_DataInizio)
        sSql += " AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Qs_Piva) & "'   "
        sSql += " AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Qs_Sa_Cod) & "   "
        '--------------
        'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
        'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
        sSql += " AND   Movimenti_Dettagli.Jolly_Int = " + CStr(MagazzinoMovimentato) + "   "
        sSql += " AND   Movimenti_Dettagli.Contabilizzato >= 0  "
        '--------------
        sSql += " AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Qs_Fabbricato_Cod) & " "
        sSql += " AND Mov_Destinazioni.Tipo_Destinazione = " + CStr(MAGAZZINO) + " "
        sSql += " AND (Movimenti.Cau_Mov = '" + CAU_CARICO + "')"
        sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FORMULATI) + " "
        '--------------
        'Modifica del 05/03/2012: escludo dalla lettura i carichi rilievo giacenze alla data di inizio del filtro (vengono conteggiati nelel giacenze iniziali)
        If FiltroRilievi <> "" Then
            sSql += " AND Movimenti_Dettagli.Id_Agenda NOT IN " + Agro_SQL_Save_Clausola_IN(CStr(FiltroRilievi), False) + " "
        End If
        '--------------

        sSql += " ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento "


        'carico il dataset coi dati..
        Dim dataProvider As New AgronicaCoreDataProvider.DataProvider
        dataProvider.EseguiQuery_Lettura(
            objParametri_Server,
            sSql,
            "Carica_DSProdottiFitosanitariMagazzino_NEW",
            DSProdottiFitosanitariMagazzino,
            DSProdottiFitosanitariMagazzino.DataSetName
        )


        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility

            Dim sFr_Cod() As String = objSqlDis.SelectDistinct(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino, "Pro_Cod")

            If Not IsNothing(sFr_Cod) Then
                For i = 0 To UBound(sFr_Cod)
                    If sFr_Cod(i) <> "0" Then
                        If Not HT_ProCod_Carichi.Contains(sFr_Cod(i)) Then
                            'modifica del 26/07/2012:
                            'memorizzo nell'HT gli fr_cod con carichi nell'intervallo
                            HT_ProCod_Carichi.Add(CInt(sFr_Cod(i)), "")
                            strFr_Cod &= sFr_Cod(i) & ","
                        End If
                    End If
                Next
            End If
            If strFr_Cod <> "" Then
                strFr_Cod = Left(strFr_Cod, strFr_Cod.Length - 1)
            End If

            '------------------------

            Dim Fr_Cod(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Count - 1) As Integer
            Dim Udm_Cod(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Count - 1) As Integer

            'ciclo su tutte le righe del dataset per inserirgli l'anno
            For i = 0 To DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Count - 1

                Dim drR As DS_ProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzinoRow = DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows(i)

                drR.Annata = QS_DataInizio & " - " & QS_DataFine

                drR.Pro_Cod = IIf(Not IsDBNull(drR.Pro_Cod), drR.Pro_Cod, 0)
                drR.Mat_Cod = IIf(Not IsDBNull(drR.Mat_Cod), drR.Mat_Cod, 0)

                Select Case drR.Mat_Cod

                    Case 0
                        drR.Pro_Des = drR.Fr_Des
                        drR.Cat_Cod = drR.Pro_Cod
                        Fr_Cod(i) = drR.Pro_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                    Case Else
                        drR.Pro_Des = drR.Mat_Des
                        drR.Cat_Cod = drR.Mat_Cod
                        Fr_Cod(i) = drR.Mat_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                End Select


                '----------

                If drR.Pro_Des <> "" Then

                    'NOTA : Ora la classe tossicologica deve comparire sempre
                    drR.CLTOSS_COD = SchedeMagazzinoHelper.ComposizioneFormulatiClasseTossicologica(objParametri_Server, drR.Pro_Cod, DTComposizioneFormulati)

                    If Qs_FlagVisualizzaComposizione Then
                        drR.Pro_Des = drR.Pro_Des &
                                        SchedeMagazzinoHelper.ComposizioneFormulatiDescrizionePrincipiAttivi(objParametri_Server, drR.Pro_Cod, DTComposizioneFormulati)
                    End If
                Else
                    drR.Pro_Des = drR.Cat_Des
                End If

                '----------


                drR.Data_Movimento = CDate(drR.Data_Movimento).ToShortDateString

                'se sono già stati fatti carichi di questo prodotto
                If (PrimaVolta = False) Then

                    'il prodotto è lo stesso della riga precedente
                    If (Fr_Cod(i) = Fr_Cod(i - 1)) And (Udm_Cod(i) = Udm_Cod(i - 1)) Then

                        'drR.Giacenza_Iniziale = ""
                        Giacenza_Iniziale = ""

                        'modifica del 5/03/2012:
                        Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                 drR.Pro_Cod,
                                                 drR.Udm_Cod)

                        DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows(i - 1).Item("Giacenza_Finale") = ""

                    Else
                        'il prodotto è cambiato

                        'modifica del 05/03/2012
                        Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze,
                                                                    DT_Rilievi,
                                                                    drR.Pro_Cod,
                                                                    drR.Udm_Cod)

                        Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                drR.Pro_Cod,
                                                                drR.Udm_Cod)

                    End If

                Else
                    '--------------------------------------
                    'entra solo la prima volta
                    'primo carico analizzato per il prodotto
                    '--------------------------------------

                    'modifica del 05/03/2012
                    Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze,
                                                                DT_Rilievi,
                                                                drR.Pro_Cod,
                                                                drR.Udm_Cod)

                    Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                drR.Pro_Cod,
                                                                drR.Udm_Cod)

                    PrimaVolta = False

                End If

                '******************************************
                drR.Giacenza_Iniziale = Giacenza_Iniziale
                drR.Giacenza_Finale = Giacenza_Finale
                '******************************************

                Select Case Qs_Arrotondamento

                    Case 0 'nessun arrotondamento


                    Case 1 'arrotondamento all'intero
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 0, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 0, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 0, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If


                    Case 2 'arrotondamento al primo decimale
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 1, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 1, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 1, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                    Case 3 '2 decimali
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 2, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 2, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 2, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                    Case 4 '3 decimali
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 3, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 3, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 3, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                    Case 5 '4 decimali
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 4, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 4, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 4, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                End Select


            Next



        End If


        '##################################################################

        'Modifica del 31/03/2009: visto che ho eliminato la query di lettura delle giacenze non movimentate nel periodo temporale
        'devo aggiungere una query per leggere i carichi e gli scarichi dall'01/01/1900 alla data inizio,
        'verificare la quantità e, se <>0, visualizzarla nella scheda
        '-------->
        'MODIFICA DEL 26/07/2012:
        'ottimizzazione:leggo le giacenze alla data inizio del filtro
        'ed aggiungo al report i prodotti che non sono stati già elaborati sopra
        'la giacenza finale è già presente nel dt_finali 
        '(non solo quelli caricati nell'intervallo temporale)

        'lettura giacenze di tutti i formulati alla data inizio del filtro
        Dim DT_GiacInizio As DataTable
        DT_GiacInizio = objGiacenze.SchedaGiacenzeMagazzino(QS_DataInizio,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        True,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)


        'NOTA
        'Tolgo la condizione perche' le Classi Tossicologiche vanno visualizzate sempre
        'If Qs_FlagVisualizzaComposizione = 1 Then
        DTComposizioneFormulati2 = SchedeMagazzinoHelper.ComposizioneFormulatiRecupera(objParametri_Server, DT_GiacInizio, Server, Session, Page)
        'End If

        Dim Pro_Cod, UdmCod As Integer
        Dim ProCod_GiacenzaInizio, ProCod_GiacenzaFine As Decimal
        For i = 0 To DT_GiacInizio.Rows.Count - 1

            Pro_Cod = DT_GiacInizio.Rows(i).Item("pro_cod")

            ' se il prodotto non è presente tra quelli movimentati nell'intervallo selezionato
            'devo aggiungerlo alla scheda con la sua giacenza iniziale e finale
            If Not HT_ProCod_Carichi.Contains(Pro_Cod) Then

                ProCod_GiacenzaInizio = DT_GiacInizio.Rows(i).Item("Giacenza")

                'oltre al filtro nella query, devo fare anche il filtro su codice
                'perchè molti decimal vengono salvati come valori infinatamente piccoli
                'ad esempio 0.00003680000000017003
                If ProCod_GiacenzaInizio <> 0 And Not (ProCod_GiacenzaInizio < QTA_GiancenzeVisualizzate And ProCod_GiacenzaInizio > -QTA_GiancenzeVisualizzate) Then

                    UdmCod = DT_GiacInizio.Rows(i).Item("Udm_Cod")

                    ProCod_GiacenzaFine = Ricava_Giacenza_Finale(Dt_Finali,
                                                                Pro_Cod,
                                                                UdmCod)

                    Dim drR As DS_ProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzinoRow = DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.NewDS_ProdottiFitosanitariMagazzinoRow

                    drR.Annata = QS_DataInizio & " - " & QS_DataFine

                    drR.Elem_Cod = FORMULATI

                    drR.Pro_Cod = Pro_Cod
                    drR.Mat_Cod = 0
                    drR.Udm_Cod = UdmCod
                    drR.Udm_Sim = CStr(DT_GiacInizio.Rows(i).Item("Udm_Sim"))

                    drR.Fr_Des = DT_GiacInizio.Rows(i).Item("Descrizione_Prodotto")
                    drR.Mat_Des = ""

                    ''drR.Cau_Mov = Dt.Rows(i).Item("Cau_Mov")
                    ''drR.Piva = Dt.Rows(i).Item("Piva")
                    ''drR.Rag_Soc = Dt.Rows(i).Item("Rag_Soc")
                    ''drR.Sa_Cod = Dt.Rows(i).Item("Sa_Cod")
                    ''drR.Sa_Nome = Dt.Rows(i).Item("Sa_Nome")
                    ''drR.Fabbicato_Cod = Dt.Rows(i).Item("Fabbicato_Cod")
                    ''drR.Fabbricato_Des = Dt.Rows(i).Item("Fabbricato_Des")
                    ''drR.CLTOSS_COD = Dt.Rows(i).Item("CLTOSS_COD")
                    ''drR.Provincia = Dt.Rows(i).Item("Provincia")

                    drR.Pro_Des = DT_GiacInizio.Rows(i).Item("Descrizione_Prodotto")
                    drR.Cat_Cod = Pro_Cod
                    drR.Cat_Des = DT_GiacInizio.Rows(i).Item("NomeComune")

                    '-------------------------

                    'If drR.Pro_Des <> "" Then
                    '    drR.Pro_Des = drR.cat_des & " - " & drR.Pro_Des
                    'Else
                    '    drR.Pro_Des = drR.cat_des
                    'End If

                    If drR.Pro_Des <> "" Then
                        'NOTA : Ora la classe tossicologica deve comparire sempre
                        drR.CLTOSS_COD = SchedeMagazzinoHelper.ComposizioneFormulatiClasseTossicologica(objParametri_Server, drR.Pro_Cod, DTComposizioneFormulati2)

                        If Qs_FlagVisualizzaComposizione Then
                            drR.Pro_Des = drR.Pro_Des &
                                            SchedeMagazzinoHelper.ComposizioneFormulatiDescrizionePrincipiAttivi(objParametri_Server, drR.Pro_Cod, DTComposizioneFormulati2)
                        End If
                    Else
                        drR.Pro_Des = drR.Cat_Des
                    End If


                    '----------------------------



                    drR.Qta = ""

                    '******************************************
                    drR.Giacenza_Iniziale = ProCod_GiacenzaInizio
                    drR.Giacenza_Finale = ProCod_GiacenzaFine
                    '******************************************

                    Select Case Qs_Arrotondamento

                        Case 0 'nessun arrotondamento


                        Case 1 'arrotondamento all'intero
                            If drR.Giacenza_Iniziale <> "" Then
                                drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 0, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Giacenza_Finale <> "" Then
                                drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 0, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Qta <> "" Then
                                drR.Qta = Math.Round(CDec(drR.Qta), 0, MidpointRounding.AwayFromZero)
                                drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                            End If


                        Case 2 'arrotondamento al primo decimale
                            If drR.Giacenza_Iniziale <> "" Then
                                drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 1, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Giacenza_Finale <> "" Then
                                drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 1, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Qta <> "" Then
                                drR.Qta = Math.Round(CDec(drR.Qta), 1, MidpointRounding.AwayFromZero)
                                drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                            End If

                        Case 3 '2 decimali
                            If drR.Giacenza_Iniziale <> "" Then
                                drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 2, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Giacenza_Finale <> "" Then
                                drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 2, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Qta <> "" Then
                                drR.Qta = Math.Round(CDec(drR.Qta), 2, MidpointRounding.AwayFromZero)
                                drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                            End If

                        Case 4 '3 decimali
                            If drR.Giacenza_Iniziale <> "" Then
                                drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 3, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Giacenza_Finale <> "" Then
                                drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 3, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Qta <> "" Then
                                drR.Qta = Math.Round(CDec(drR.Qta), 3, MidpointRounding.AwayFromZero)
                                drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                            End If

                        Case 5 '4 decimali
                            If drR.Giacenza_Iniziale <> "" Then
                                drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 4, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Giacenza_Finale <> "" Then
                                drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 4, MidpointRounding.AwayFromZero)
                                drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                            End If

                            If drR.Qta <> "" Then
                                drR.Qta = Math.Round(CDec(drR.Qta), 4, MidpointRounding.AwayFromZero)
                                drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                            End If

                    End Select

                    'aggiungo la riga
                    DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Add(drR)

                End If 'prodotto con giacenza valorizzata


            End If 'prodotto non presente nell'intervallo

        Next 'ciclo giacenze alla data inizio






        '##################################################################

        'rendo permanenti le modifiche sul ds
        DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.AcceptChanges()



        '--------------------------------------------------------------------------------------
        'ORDINO IL DATASET
        '(per farlo importo nell'ordine le datarow ordinate e poi elimino le vecchie righe)
        '--------------------------------------------------------------------------------------

        Dim Righe_Old As Integer
        Dim dr As DataRow

        Count = 0

        Righe_Old = DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Count

        If Righe_Old > 0 Then


            'Ottiene una matrice di tutti gli oggetti DataRow che corrispondono al filtro, in base all'ordinamento specificato
            For Each dr In DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Select("", "Fr_Des, Mat_Des")

                Count += 1

                DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.ImportRow(dr)

            Next

            For i = 0 To Righe_Old - 1
                DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows(i).Delete()
            Next

            'rendo permanenti le modifiche sul ds
            DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.AcceptChanges()

        End If


    End Sub


    '###########################################################################
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 05/03/2012
    'a Buzzi di agrisol dava fastidio che un carico di magazzino (rilievo giacenza) fatto alla data di inizio del filtro
    'venisse visualizzato nella colonna qta acquistata e non giacenza iniziale
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 31/03/2009
    'altrimenti comparivano in scheda anche i prodotti entrati in magazzino dopo l'intervallo temporale selezionato
    Private Sub Carica_DSProdottiFitosanitariMagazzino_OLD(ByRef DSProdottiFitosanitariMagazzino As DS_ProdottiFitosanitariMagazzino)


        Dim objSQL As New AgronicaCoreDataProvider.DatatableUtility
        Dim strErr As String
        Dim sSql As String
        Dim i As Integer

        Dim Qta As Decimal
        Dim Giacenza_Iniziale As String
        Dim Giacenza_Finale As String

        Dim PrimaVolta As Boolean = True
        Dim strFr_Cod As String = ""
        'Dim strMat_Cod As String = ""

        Dim Giacenza As Decimal
        Dim Count As Integer = 0

        Dim objGiacenze As New AgronicaCoreStampeDAL.Magazzino
        Dim DT_Giacenze As DataTable
        Dim DataFiltro_GiornoPrima As Date

        DataFiltro_GiornoPrima = CDate(QS_DataInizio).AddDays(-1)

        'lettura giacenze al giorno prima della data inizio del filtro
        DT_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltro_GiornoPrima,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        True,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)

        Dim DT_Rilievi As DataTable
        Dim FiltroRilievi As String
        Dim IdAgendaRilieviDaEscludere As String

        FiltroRilievi = " AND ( Agenda.Lav_Cod =" + Agro_SQL_SaveNum(LAVCOD_CARICO) +
                                " AND  Movimenti_Dettagli.Pendente =" + Agro_SQL_SaveNum(enum_Pendenza.GiacenzeIniziali) +
                                ")"

        'cerco i rilievi giacenze registrati alla data di inizio del filtro
        DT_Rilievi = objGiacenze.SchedaMovimentiMagazzino(QS_DataInizio,
                                                        QS_DataInizio,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        FiltroRilievi,
                                                        "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)

        FiltroRilievi = ""
        If Not IsNothing(DT_Rilievi) AndAlso DT_Rilievi.Rows.Count > 0 Then
            Dim id_agenda As Integer
            For i = 0 To DT_Rilievi.Rows.Count - 1
                id_agenda = DT_Rilievi.Rows(i).Item("Id_Agenda")
                FiltroRilievi += CStr(id_agenda) + ","
            Next
            FiltroRilievi = "(" + Left(FiltroRilievi, FiltroRilievi.Length - 1) + ")"
        End If

        'giacenze alla data di fine del filtro
        Dim Dt_Finali As DataTable
        Dt_Finali = objGiacenze.SchedaGiacenzeMagazzino(QS_DataFine,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FORMULATI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        False,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)



        'Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto          

        sSql = ""
        sSql += " ( "
        sSql += " SELECT DISTINCT "
        sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Formulati.Fr_Des, '') AS Fr_Des, ISNULL(Materie_Prime.Mat_Des, '') AS MAt_Des, "
        sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento "

        sSql += " FROM         Materie_Prime RIGHT OUTER JOIN"
        sSql += "               UnitaMisura INNER JOIN"
        sSql += "               Mov_Destinazioni INNER JOIN"
        sSql += "               CategorieMagazzino INNER JOIN"
        sSql += "               Movimenti_dettagli ON CategorieMagazzino.Elem_Cod = Movimenti_dettagli.Elem_Cod INNER JOIN"
        sSql += "               Movimenti INNER JOIN"
        sSql += "               Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Id_Agenda = Agenda.Id_Agenda ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND "
        sSql += "               Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ON "
        sSql += "               Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND "
        sSql += "               Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND "
        sSql += "               Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ON UnitaMisura.UDM_COD = Movimenti_dettagli.Udm_Cod ON "
        sSql += "               Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod LEFT OUTER JOIN"
        sSql += "               Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod "

        sSql += " WHERE Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(QS_DataFine)
        sSql += " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(QS_DataInizio)
        sSql += " AND Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Qs_Piva) & "'   "
        sSql += " AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Qs_Sa_Cod) & "   "
        '--------------
        'Modifica del 28/05/2009: altrimenti i carichi e gli scarichi vengono sdoppiati nel caso di bolle agganciate a fatture
        'e vengono visualizzati anche i carichi/scarichi di operazioni pendenti
        sSql += " AND   Movimenti_Dettagli.Jolly_Int = " + CStr(MagazzinoMovimentato) + "   "
        sSql += " AND   Movimenti_Dettagli.Contabilizzato >= 0  "
        '--------------
        sSql += " AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Qs_Fabbricato_Cod) & " "
        sSql += " AND Mov_Destinazioni.Tipo_Destinazione = " + CStr(MAGAZZINO) + " "
        sSql += " AND (Movimenti.Cau_Mov = '" + CAU_CARICO + "')"
        sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FORMULATI) + " "
        '--------------
        'Modifica del 05/03/2012: escludo dalla lettura i carichi rilievo giacenze alla data di inizio del filtro (vengono conteggiati nelel giacenze iniziali)
        If FiltroRilievi <> "" Then
            sSql += " AND Movimenti_Dettagli.Id_Agenda NOT IN " + Agro_SQL_Save_Clausola_IN(CStr(FiltroRilievi), False) + " "
        End If
        '--------------

        sSql += " ) "

        sSql += "ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento"


        'carico il dataset coi dati..
        'carico il dataset coi dati..
        Dim dataProvider As New AgronicaCoreDataProvider.DataProvider
        dataProvider.EseguiQuery_Lettura(objParametri_Server,
                            sSql,
                            "",
                            DSProdottiFitosanitariMagazzino,
                            DSProdottiFitosanitariMagazzino.DataSetName
                        )


        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility

            Dim sFr_Cod() As String = objSqlDis.SelectDistinct(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino, "Pro_Cod")
            'Dim sMat_Cod() As String = objSqlDis.SelectDistinct(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino, "Mat_Cod")

            If Not IsNothing(sFr_Cod) Then
                For i = 0 To UBound(sFr_Cod)
                    If sFr_Cod(i) <> "0" Then
                        strFr_Cod &= sFr_Cod(i) & ","
                    End If
                Next
            End If
            If strFr_Cod <> "" Then
                strFr_Cod = Left(strFr_Cod, strFr_Cod.Length - 1)
            End If

            'If Not IsNothing(sMat_Cod) Then
            '    For i = 0 To UBound(sMat_Cod)
            '        If sMat_Cod(i) <> "0" Then
            '            strMat_Cod &= sMat_Cod(i) & ","
            '        End If
            '    Next
            'End If
            'If strMat_Cod <> "" Then
            '    strMat_Cod = Left(strMat_Cod, strMat_Cod.Length - 1)
            'End If

            '------------------------

            Dim Fr_Cod(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Count - 1) As Integer
            Dim Udm_Cod(DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Count - 1) As Integer

            'ciclo su tutte le righe del dataset per inserirgli l'anno
            For i = 0 To DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Count - 1

                Dim drR As DS_ProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzinoRow = DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows(i)

                drR.Annata = QS_DataInizio & " - " & QS_DataFine

                drR.Pro_Cod = IIf(Not IsDBNull(drR.Pro_Cod), drR.Pro_Cod, 0)
                drR.Mat_Cod = IIf(Not IsDBNull(drR.Mat_Cod), drR.Mat_Cod, 0)

                Select Case drR.Mat_Cod

                    Case 0
                        drR.Pro_Des = drR.Fr_Des
                        drR.Cat_Cod = drR.Pro_Cod
                        Fr_Cod(i) = drR.Pro_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                    Case Else
                        drR.Pro_Des = drR.Mat_Des
                        drR.Cat_Cod = drR.Mat_Cod
                        Fr_Cod(i) = drR.Mat_Cod
                        Udm_Cod(i) = drR.Udm_Cod


                End Select

                If drR.Pro_Des <> "" Then
                    drR.Pro_Des = drR.Cat_Des & " - " & drR.Pro_Des
                Else
                    drR.Pro_Des = drR.Cat_Des
                End If

                drR.Data_Movimento = CDate(drR.Data_Movimento).ToShortDateString

                ''Se il prodotto è stato MOVIMENTATO nell'intervallo (NON è solo GIACENZA)...
                'If drR.Data_Movimento <> "01/01/1900" Then

                'se sono già stati fatti carichi di questo prodotto
                If (PrimaVolta = False) Then

                    If (Fr_Cod(i) = Fr_Cod(i - 1)) And (Udm_Cod(i) = Udm_Cod(i - 1)) Then

                        'drR.Giacenza_Iniziale = ""
                        Giacenza_Iniziale = ""

                        'modifica del 5/03/2012:
                        'Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                        '                                    CStr(Qs_Piva), _
                        '                                    CInt(Qs_Sa_Cod), _
                        '                                    CInt(Qs_Fabbricato_Cod), _
                        '                                    drR.Elem_Cod, _
                        '                                    drR.Pro_Cod, _
                        '                                    drR.Mat_Cod, _
                        '                                    0, _
                        '                                    0, _
                        '                                    "", _
                        '                                    0, _
                        '                                    drR.Udm_Cod, _
                        '                                    CDate("01/01/1900"), _
                        '                                    CDate(QS_DataFine), _
                        '                                    , )


                        ''drR.Giacenza_Finale = Format(Qta, "##0.###")
                        'Giacenza_Finale = Qta
                        Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                    drR.Pro_Cod,
                                                                    drR.Udm_Cod)

                        DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows(i - 1).Item("Giacenza_Finale") = ""

                    Else

                        'modifica del 05/03/2012
                        ''controllo che il carico nn sia fatto il giorno iniziale del filtro..
                        ''se lo è la giacenza iniziale la forzo a zero
                        'If CDate(drR.Data_Movimento) <> CDate(QS_DataInizio) Then

                        '    Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                        '                                    CStr(Qs_Piva), _
                        '                                    CInt(Qs_Sa_Cod), _
                        '                                    CInt(Qs_Fabbricato_Cod), _
                        '                                    drR.Elem_Cod, _
                        '                                    drR.Pro_Cod, _
                        '                                    drR.Mat_Cod, _
                        '                                    0, _
                        '                                    0, _
                        '                                    "", _
                        '                                    0, _
                        '                                    drR.Udm_Cod, _
                        '                                    CDate("01/01/1900"), _
                        '                                    CDate(QS_DataInizio), _
                        '                                    , )

                        '    'drR.Giacenza_Iniziale = Format(Qta, "##0.###")
                        '    Giacenza_Iniziale = Qta
                        'Else
                        '    'drR.Giacenza_Iniziale = "0"
                        '    Giacenza_Iniziale = "0"
                        'End If


                        Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze,
                                                                    DT_Rilievi,
                                                                    drR.Pro_Cod,
                                                                    drR.Udm_Cod)

                        'modifica del 5/03/2012:
                        'Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                        '                                 CStr(Qs_Piva), _
                        '                                 CInt(Qs_Sa_Cod), _
                        '                                 CInt(Qs_Fabbricato_Cod), _
                        '                                 drR.Elem_Cod, _
                        '                                 drR.Pro_Cod, _
                        '                                 drR.Mat_Cod, _
                        '                                 0, _
                        '                                 0, _
                        '                                 "", _
                        '                                 0, _
                        '                                 drR.Udm_Cod, _
                        '                                 CDate("01/01/1900"), _
                        '                                 CDate(QS_DataFine), _
                        '                                 , )

                        ''drR.Giacenza_Finale = Format(Qta, "##0.###")
                        'Giacenza_Finale = Qta

                        Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                drR.Pro_Cod,
                                                drR.Udm_Cod)

                    End If

                Else
                    '--------------------------------------
                    'entra solo la prima volta
                    '--------------------------------------

                    'modifica del 05/03/2012
                    ''controllo che il carico nn sia fatto il giorno iniziale del filtro..
                    ''se lo è la giacenza iniziale la forzo a zero
                    'If CDate(drR.Data_Movimento) <> CDate(QS_DataInizio) Then

                    '    Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                    '                                                             CStr(Qs_Piva), _
                    '                                                             CInt(Qs_Sa_Cod), _
                    '                                                             CInt(Qs_Fabbricato_Cod), _
                    '                                                             drR.Elem_Cod, _
                    '                                                             drR.Pro_Cod, _
                    '                                                             drR.Mat_Cod, _
                    '                                                             0, _
                    '                                                             0, _
                    '                                                             "", _
                    '                                                             0, _
                    '                                                             drR.Udm_Cod, _
                    '                                                             CDate("01/01/1900"), _
                    '                                                             CDate(QS_DataInizio), _
                    '                                                             , )



                    '    'drR.Giacenza_Iniziale = Format(Qta, "##0.###")
                    '    Giacenza_Iniziale = Qta

                    'Else
                    '    'drR.Giacenza_Iniziale = "0"
                    '    Giacenza_Iniziale = "0"
                    'End If

                    Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze,
                                                                DT_Rilievi,
                                                                drR.Pro_Cod,
                                                                drR.Udm_Cod)

                    'modifica del 5/3/2012:
                    'Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                    '                                CStr(Qs_Piva), _
                    '                                CInt(Qs_Sa_Cod), _
                    '                                CInt(Qs_Fabbricato_Cod), _
                    '                                drR.Elem_Cod, _
                    '                                drR.Pro_Cod, _
                    '                                drR.Mat_Cod, _
                    '                                0, _
                    '                                0, _
                    '                                "", _
                    '                                0, _
                    '                                drR.Udm_Cod, _
                    '                                CDate("01/01/1900"), _
                    '                                CDate(QS_DataFine), _
                    '                                , )

                    ''drR.Giacenza_Finale = Format(Qta, "##0.###")
                    'Giacenza_Finale = Qta

                    Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                drR.Pro_Cod,
                                                                drR.Udm_Cod)

                    PrimaVolta = False

                End If

                'Else

                '    '31/03/2009: qui non ci dovrebbe più entrare, visto che ho tolto la query delle giacenze che impostava data_movimento = 01/01/1900

                '    'Per i record SOLO GIACENZE indico solo la giacenza iniziale e la finale 
                '    'che ovviamente coincidono

                '    'ATTENZIONE: modifica del 31/03/2009 !!!!
                '    'Mancava la verifica della quantità alla data di fine!
                '    drR.Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                '                                        CStr(Qs_Piva), _
                '                                        CInt(Qs_Sa_Cod), _
                '                                        CInt(Qs_Fabbricato_Cod), _
                '                                        drR.Elem_Cod, _
                '                                        drR.Pro_Cod, _
                '                                        drR.Mat_Cod, _
                '                                        0, _
                '                                        0, _
                '                                        "", _
                '                                        0, _
                '                                        drR.Udm_Cod, _
                '                                        CDate("01/01/1900"), _
                '                                        CDate(QS_DataFine), _
                '                                        , )




                '    'drR.Giacenza_Iniziale = Format(Cdec(drR.Qta), "##0.###")
                '    Giacenza_Iniziale = drR.Qta

                '    'drR.Giacenza_Finale = Format(Cdec(drR.Qta), "##0.###")
                '    Giacenza_Finale = drR.Qta

                '    drR.Data_Movimento = ""
                '    drR.Qta = ""

                'End If   'if SOLO GIACENZE!!!!!!!!!!!!!!


                'If drR.Giacenza_Iniziale <> "" Then
                '    drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                'End If

                'If drR.Giacenza_Finale <> "" Then
                '    drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                'End If

                'If drR.Qta <> "" Then
                '    drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                'End If

                '******************************************
                drR.Giacenza_Iniziale = Giacenza_Iniziale
                drR.Giacenza_Finale = Giacenza_Finale
                '******************************************

                Select Case Qs_Arrotondamento

                    Case 0 'nessun arrotondamento


                    Case 1 'arrotondamento all'intero
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 0, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 0, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 0, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If


                    Case 2 'arrotondamento al primo decimale
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 1, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 1, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 1, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                    Case 3 '2 decimali
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 2, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 2, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 2, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                    Case 4 '3 decimali
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 3, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 3, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 3, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                    Case 5 '4 decimali
                        If drR.Giacenza_Iniziale <> "" Then
                            drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 4, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Giacenza_Finale <> "" Then
                            drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 4, MidpointRounding.AwayFromZero)
                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                        End If

                        If drR.Qta <> "" Then
                            drR.Qta = Math.Round(CDec(drR.Qta), 4, MidpointRounding.AwayFromZero)
                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                        End If

                End Select


            Next



        End If


        '##################################################################


        'Modifica del 31/03/2009: visto che ho eliminato la query di lettura delle giacenze non movimentate nel periodo temporale
        'devo aggiungere una query per leggere i carichi e egli scarichi dall'01/01/1900 alla data inizio,
        'verificare la quantità e, se <>0, visualizzarla nella scheda

        'Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto          

        sSql = ""
        sSql += " ( "
        sSql += " SELECT DISTINCT "
        sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Formulati.Fr_Des, '') AS Fr_Des, ISNULL(Materie_Prime.Mat_Des, '') AS MAt_Des, "
        sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento "

        sSql += " FROM         Materie_Prime RIGHT OUTER JOIN"
        sSql += "               UnitaMisura INNER JOIN"
        sSql += "               Mov_Destinazioni INNER JOIN"
        sSql += "               CategorieMagazzino INNER JOIN"
        sSql += "               Movimenti_dettagli ON CategorieMagazzino.Elem_Cod = Movimenti_dettagli.Elem_Cod INNER JOIN"
        sSql += "               Movimenti INNER JOIN"
        sSql += "               Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Id_Agenda = Agenda.Id_Agenda ON Movimenti_dettagli.PIVA = Movimenti.PIVA AND "
        sSql += "               Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov ON "
        sSql += "               Mov_Destinazioni.Piva = Movimenti_dettagli.PIVA AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod AND "
        sSql += "               Mov_Destinazioni.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_dettagli.Id_Mov AND "
        sSql += "               Mov_Destinazioni.Id_Mov_Det = Movimenti_dettagli.Id_Mov_Det ON UnitaMisura.UDM_COD = Movimenti_dettagli.Udm_Cod ON "
        sSql += "               Materie_Prime.Elem_Cod = Movimenti_dettagli.Elem_Cod AND Materie_Prime.Mat_Cod = Movimenti_dettagli.Mat_Cod LEFT OUTER JOIN"
        sSql += "               Formulati ON Movimenti_dettagli.Pro_Cod = Formulati.Fr_Cod "

        sSql += " WHERE Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Qs_Piva) & "'   "
        sSql += " AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Qs_Sa_Cod) & "   "
        sSql += " AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Qs_Fabbricato_Cod) & " "
        sSql += " AND Mov_Destinazioni.Tipo_Destinazione = " + CStr(MAGAZZINO) + " "
        sSql += " AND (Movimenti.Cau_Mov = '" + CAU_CARICO + "')"
        sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FORMULATI) + " "

        'modifica del 5/3/2012:
        'metto in OR gli id_agenda che ho scartato sopra nell'elenco dei carichi
        'perchè se il prodotto, oltre al carico alla data di inizio, non è stato più movimentato
        'poi non viene visualizzato nel report
        sSql += " AND ( "
        sSql += " ( "
        'non <= perchè l'= è già considerato nella query sopra
        sSql += " Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(QS_DataInizio)
        sSql += " AND   Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(AGRODATAINIZIO)
        sSql += ")"
        If FiltroRilievi <> "" Then
            sSql += " OR "
            sSql += " ( "
            sSql += " Movimenti_Dettagli.Id_Agenda IN " + Agro_SQL_Save_Clausola_IN(CStr(FiltroRilievi), False) + " "
            sSql += ")"
        End If
        sSql += "   )"

        'escludo i prodotti già considerati sopra
        If strFr_Cod <> "" Then
            sSql += " AND Pro_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(strFr_Cod, False) & ")"
        End If

        sSql += " ) "

        'sSql += "ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento"
        sSql += "ORDER BY Fr_Des ASC "


        ''carico il dataset coi dati..
        'objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
        '                    Session("ASG_Connessione_Server"), _
        '                    sSql, _
        '                    DSProdottiFitosanitariMagazzino, _
        '                    DSProdottiFitosanitariMagazzino.DataSetName, _
        '                    strErr)

        Dim Dt As DataTable

        'carico il dataset coi dati..
        Dt = dataProvider.EseguiQuery_Lettura(objParametri_Server, sSql, "Carica_DSProdottiFitosanitariMagazzino_OLD")


        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            Dim Qta_Verifica_Inizio As Decimal = 0
            Dim Qta_Verifica_Fine As Decimal = 0

            Dim Hash_Formulati As New Hashtable

            Dim Chiave_Giacenza As String
            Dim Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod As Integer

            'ciclo su tutte le righe del dataset per inserirgli l'anno
            For i = 0 To Dt.Rows.Count - 1

                Elem_Cod = Dt.Rows(i).Item("Elem_Cod")
                Pro_Cod = Dt.Rows(i).Item("Pro_Cod")
                Mat_Cod = Dt.Rows(i).Item("Mat_Cod")
                Udm_Cod = Dt.Rows(i).Item("Udm_Cod")

                Chiave_Giacenza = CStr(Elem_Cod) + "|" + CStr(Pro_Cod) + "|" + CStr(Mat_Cod) + "|" + CStr(Udm_Cod)


                If Not Hash_Formulati.Contains(Chiave_Giacenza) Then

                    Hash_Formulati.Add(Chiave_Giacenza, "")

                    'NON è vero che per le giacenze che non sono movimentate (e quindi non risultano dalla query sopra)
                    'la giacenza iniziale è = a quella finale, perchè nel mezzo possono esserci stati degli scarichi!


                    'leggo la giacenza iniziale
                    'TODO: verifcare Id_Agenda_daNon_considerare
                    Dim agC As New AgronicaCoreContabDAL.Movimenti_Dettagli_R
                    Qta_Verifica_Inizio = agC.Verifica_Giacenze(
                        CStr(Qs_Piva),
                        CInt(Qs_Sa_Cod),
                        CInt(Qs_Fabbricato_Cod),
                        Elem_Cod,
                        Pro_Cod,
                        Mat_Cod,
                        0,
                        0,
                        "",
                        0,
                        Udm_Cod,
                        CDate("01/01/1900"),
                        CDate(QS_DataInizio),
                        0,
                        objParametri_Server
                     )




                    'leggo la giacenza finale
                    Qta_Verifica_Fine = agC.Verifica_Giacenze(
                        CStr(Qs_Piva),
                        CInt(Qs_Sa_Cod),
                        CInt(Qs_Fabbricato_Cod),
                        Elem_Cod,
                        Pro_Cod,
                        Mat_Cod,
                        0,
                        0,
                        "",
                        0,
                        Udm_Cod,
                        CDate("01/01/1900"),
                        CDate(QS_DataFine),
                        0,
                        objParametri_Server
                        )


                    'FACCIO UN ARROTONDAMENTO PERCHè MOLTE VOLTE CAPITA UNA QTA PICCOLA
                    Qta_Verifica_Inizio = Math.Round(Qta_Verifica_Inizio, 6, MidpointRounding.AwayFromZero)
                    Qta_Verifica_Fine = Math.Round(Qta_Verifica_Fine, 6, MidpointRounding.AwayFromZero)

                    'controllo la giacenza iniziale, perchè se è 0, è 0 anche la finale
                    '(a meno che uno non abbia scaricato quello non aveva!)
                    If Qta_Verifica_Inizio <> 0 Then


                        Dim drR As DS_ProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzinoRow = DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.NewDS_ProdottiFitosanitariMagazzinoRow

                        drR.Annata = QS_DataInizio & " - " & QS_DataFine

                        drR.Elem_Cod = Elem_Cod
                        drR.Cat_Des = Dt.Rows(i).Item("Cat_Des")
                        drR.Pro_Cod = IIf(Not IsDBNull(Pro_Cod), Pro_Cod, 0)
                        drR.Mat_Cod = IIf(Not IsDBNull(Mat_Cod), Mat_Cod, 0)
                        drR.Udm_Cod = Udm_Cod
                        drR.Udm_Sim = CStr(Dt.Rows(i).Item("Udm_Sim"))

                        drR.Fr_Des = Dt.Rows(i).Item("Fr_Des")
                        drR.Mat_Des = Dt.Rows(i).Item("Mat_Des")

                        ''drR.Cau_Mov = Dt.Rows(i).Item("Cau_Mov")
                        ''drR.Piva = Dt.Rows(i).Item("Piva")
                        ''drR.Rag_Soc = Dt.Rows(i).Item("Rag_Soc")
                        ''drR.Sa_Cod = Dt.Rows(i).Item("Sa_Cod")
                        ''drR.Sa_Nome = Dt.Rows(i).Item("Sa_Nome")
                        ''drR.Fabbicato_Cod = Dt.Rows(i).Item("Fabbicato_Cod")
                        ''drR.Fabbricato_Des = Dt.Rows(i).Item("Fabbricato_Des")
                        ''drR.CLTOSS_COD = Dt.Rows(i).Item("CLTOSS_COD")
                        ''drR.Provincia = Dt.Rows(i).Item("Provincia")

                        drR.Data_Movimento = ""

                        Select Case Mat_Cod
                            Case 0
                                drR.Pro_Des = Dt.Rows(i).Item("Fr_Des")
                                drR.Cat_Cod = Pro_Cod
                            Case Else
                                drR.Pro_Des = Dt.Rows(i).Item("Mat_Des")
                                drR.Cat_Cod = Mat_Cod
                        End Select

                        If drR.Pro_Des <> "" Then
                            drR.Pro_Des = Dt.Rows(i).Item("Cat_Des") & " - " & drR.Pro_Des
                        Else
                            drR.Pro_Des = Dt.Rows(i).Item("Cat_Des")
                        End If

                        'drR.Giacenza_Iniziale = Format(Cdec(drR.Qta), "##0.###")
                        Giacenza_Iniziale = Qta_Verifica_Inizio

                        'drR.Giacenza_Finale = Format(Cdec(drR.Qta), "##0.###")
                        Giacenza_Finale = Qta_Verifica_Fine

                        drR.Qta = ""

                        '******************************************
                        drR.Giacenza_Iniziale = Giacenza_Iniziale
                        drR.Giacenza_Finale = Giacenza_Finale
                        '******************************************

                        Select Case Qs_Arrotondamento

                            Case 0 'nessun arrotondamento


                            Case 1 'arrotondamento all'intero
                                If drR.Giacenza_Iniziale <> "" Then
                                    drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 0, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Giacenza_Finale <> "" Then
                                    drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 0, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Qta <> "" Then
                                    drR.Qta = Math.Round(CDec(drR.Qta), 0, MidpointRounding.AwayFromZero)
                                    drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                                End If


                            Case 2 'arrotondamento al primo decimale
                                If drR.Giacenza_Iniziale <> "" Then
                                    drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 1, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Giacenza_Finale <> "" Then
                                    drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 1, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Qta <> "" Then
                                    drR.Qta = Math.Round(CDec(drR.Qta), 1, MidpointRounding.AwayFromZero)
                                    drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                                End If

                            Case 3 '2 decimali
                                If drR.Giacenza_Iniziale <> "" Then
                                    drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 2, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Giacenza_Finale <> "" Then
                                    drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 2, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Qta <> "" Then
                                    drR.Qta = Math.Round(CDec(drR.Qta), 2, MidpointRounding.AwayFromZero)
                                    drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                                End If

                            Case 4 '3 decimali
                                If drR.Giacenza_Iniziale <> "" Then
                                    drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 3, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Giacenza_Finale <> "" Then
                                    drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 3, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Qta <> "" Then
                                    drR.Qta = Math.Round(CDec(drR.Qta), 3, MidpointRounding.AwayFromZero)
                                    drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                                End If

                            Case 5 '4 decimali
                                If drR.Giacenza_Iniziale <> "" Then
                                    drR.Giacenza_Iniziale = Math.Round(CDec(drR.Giacenza_Iniziale), 4, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Giacenza_Finale <> "" Then
                                    drR.Giacenza_Finale = Math.Round(CDec(drR.Giacenza_Finale), 4, MidpointRounding.AwayFromZero)
                                    drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
                                End If

                                If drR.Qta <> "" Then
                                    drR.Qta = Math.Round(CDec(drR.Qta), 4, MidpointRounding.AwayFromZero)
                                    drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
                                End If

                        End Select

                        Count += 1

                        'aggiungo la riga
                        DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows.Add(drR)

                    End If 'se la giacenza iniziale è <> 0

                Else
                    'formulato già inserito in scheda con qta verificata
                End If

            Next 'per ogni movimento

            'rendo permanenti le modifiche sul ds
            DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.AcceptChanges()

            Hash_Formulati = Nothing


        End If


        '##################################################################


        '--------------------------------------------------------------------------------------
        'ORDINO IL DATASET
        '(per farlo importo nell'ordine le datarow ordinate e poi elimino le vecchie righe)
        '--------------------------------------------------------------------------------------

        Dim Righe_Old As Integer
        Dim dr As DataRow

        Count = 0

        Righe_Old = DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Count

        If Righe_Old > 0 Then


            'Ottiene una matrice di tutti gli oggetti DataRow che corrispondono al filtro, in base all'ordinamento specificato
            For Each dr In DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Select("", "Fr_Des, Mat_Des")

                Count += 1

                DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.ImportRow(dr)

            Next

            For i = 0 To Righe_Old - 1
                DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.Rows(i).Delete()
            Next

            'rendo permanenti le modifiche sul ds
            DSProdottiFitosanitariMagazzino.DS_ProdottiFitosanitariMagazzino.AcceptChanges()

        End If


    End Sub

    '#####################################################################
    Private Function Ricava_Giacenza_Iniziale(ByVal Dt_Giacenze As DataTable, _
                                                ByVal Dt_Rilievi As DataTable, _
                                                ByVal Pro_cod As Integer, _
                                                ByVal Udm_cod As Integer) As Decimal

        Dim Giacenza_Iniziale As Decimal = 0

        If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then
            Dim DrGiac() As DataRow
            DrGiac = Dt_Giacenze.Select(" Pro_Cod = " + Agro_SQL_SaveNum(Pro_cod) + _
                                        " AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_cod))

            If Not IsNothing(DrGiac) AndAlso DrGiac.Length > 0 Then
                Giacenza_Iniziale = DrGiac(0).Item("Giacenza")
            End If
        End If

        If Not IsNothing(Dt_Rilievi) AndAlso Dt_Rilievi.Rows.Count > 0 Then
            Dim Dr() As DataRow
            Dr = Dt_Rilievi.Select(" Pro_Cod = " + Agro_SQL_SaveNum(Pro_cod) + _
                                        " AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_cod))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then
                Dim i As Integer
                For i = 0 To Dr.Length - 1
                    Giacenza_Iniziale += Dr(i).Item("Qta_Dest")
                Next
            End If
        End If

        Return Giacenza_Iniziale

    End Function

    '#####################################################################
    Private Function Ricava_Giacenza_Finale(ByVal Dt_Finale As DataTable, _
                                                ByVal Pro_cod As Integer, _
                                                ByVal Udm_cod As Integer) As Decimal

        Dim Giacenza_Finale As Decimal = 0

        If Not IsNothing(Dt_Finale) AndAlso Dt_Finale.Rows.Count > 0 Then
            Dim DrGiac() As DataRow
            DrGiac = Dt_Finale.Select(" Pro_Cod = " + Agro_SQL_SaveNum(Pro_cod) + _
                                        " AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_cod))

            If Not IsNothing(DrGiac) AndAlso DrGiac.Length > 0 Then
                Giacenza_Finale = CType(DrGiac(0).Item("Giacenza"), Decimal)
            End If
        End If

        Return Giacenza_Finale

    End Function

    '####################################################################################################################
    Private Sub VisualizzaLogoRegione()
        ' nascondo tutti i loghi delle regioni
        Dim r As Integer
        For r = 1 To 20
            Try
                rptStampa.Section6.ReportObjects("LogoRegione" & Right("000" & r.ToString, 3)).ObjectFormat.EnableSuppress = True
            Catch ex As Exception
                ' il logo non c'è
                ex = Nothing
            End Try
        Next

        'visualizzo il logo della regione richiesta
        If CStr(Session("Regione_Selezionata")) <> String.Empty Then
            Try
                rptStampa.Section6.ReportObjects("LogoRegione" & Right("000" & CStr(Session("Regione_Selezionata")), 3)).ObjectFormat.EnableSuppress = False
                If Session("Regione_Selezionata") = "008" Then
                    rptStampa.Section6.ReportObjects("Testo1").ObjectFormat.EnableSuppress = False
                End If
            Catch ex As Exception
                ' il logo non c'è
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Mostro i riferimenti dei Regolamenti aggiornati se sto stampando con Data Fine >= 2024 e se ho selezionato
    ''' come regione 'Emilia - Romagna' mostro anche il regolamento 'QC'
    ''' </summary>
    Private Sub MostraRiferimentiRegolamentiAggiornati()

        If Not String.IsNullOrEmpty(QS_DataFine) AndAlso IsDate(QS_DataFine) AndAlso CDate(QS_DataFine).Year >= 2024 Then

            rptStampa.Section1.ReportObjects("Text6").ObjectFormat.EnableSuppress = True

            If CStr(Session("Regione_Selezionata")) = "008" Then
                rptStampa.Section1.ReportObjects("TextRegolamenti2025ER").ObjectFormat.EnableSuppress = False
            Else
                rptStampa.Section1.ReportObjects("TextRegolamenti2025Generale").ObjectFormat.EnableSuppress = False
            End If
        End If

        Session("Regione_Selezionata") = Nothing

    End Sub

End Class
