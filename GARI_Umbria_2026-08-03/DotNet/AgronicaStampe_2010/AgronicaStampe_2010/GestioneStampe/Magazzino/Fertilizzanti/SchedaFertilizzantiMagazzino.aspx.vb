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

Public Class SchedaFertilizzantiMagazzino

    Inherits System.Web.UI.Page
    Private rptStampa As Rpt_SchedaFertilizzantiMagazzino
    Private rptFooterLogo As FooterLogo
    Private DSFertilizzantiMagazzino As DS_FertilizzantiMagazzino
    Private DSLogoFooter As New DS_LogoFooter

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim Qs_Arrotondamento As Integer
    'Dim Qs_Ordinamento As Integer


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
        rptStampa = New Rpt_SchedaFertilizzantiMagazzino
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

        'TODO: verificare la funzione in agronicacorexml\xml_stampe\XML_EstraiVariabiliStampe (non viene letto il sa_Cod)
        If Qs_Sa_Cod = "" Then
            Qs_Sa_Cod = "0"
        End If

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString,
                    AgroKey_EncoderDecoder,
                    Server)

        If Qs_Fabbricato_Cod = "" Then
            Qs_Fabbricato_Cod = "0"
        End If

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

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

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
            '    rptStampa.Section5.ReportObjects("Text9").ObjectFormat.EnableSuppress = True
            '    rptStampa.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
            '    rptStampa.Section5.ReportObjects("Text14").ObjectFormat.EnableSuppress = True
            'End If

            VisualizzaLogoRegione()

            MostraRiferimentiRegolamentiAggiornati()

            Dim DSFertilizzantiMagazzino As New DS_FertilizzantiMagazzino

            'carico i dati nel datatable 
            Carica_DSFertilizzantiMagazzino(DSFertilizzantiMagazzino)

            Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
            Dim Log_Errori As String = ""
            Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
            If logo.LogoStampe IsNot Nothing Then
                drLogo.Logo = logo.LogoStampe
                drLogo.TestoPostLogo = logo.TestoPostLogo
                drLogo.TestoPreLogo = logo.TestoPreLogo
            End If
            DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

            'calcolo il basecode x l'utente
            Dim BaseCode As Integer
            Calcola_BaseCode_TopCode(BaseCode, Nothing, Session("ASG_ProgressivoGIAS"))

            'sorgente dati.....
            rptStampa.SetDataSource(DSFertilizzantiMagazzino)
            rptFooterLogo.SetDataSource(DSLogoFooter)
            rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)

            'faccio il databind col visualizzatore dei reports...
            CrystalReportViewer1.ReportSource = rptStampa
            CrystalReportViewer1.DataBind()

            'array di dataset e data table
            Dim dsRpt() As DataSet = {DSFertilizzantiMagazzino}

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
        'MS Eliminato passaggio in session per passaggio report su file: Session("Report") = rptStampa
        Try
            Dim catCod As Integer = enum_CategorieDocumenti.Magazzino_Fertilizzanti
            Dim identificazioneDocumento = "FertilizzantiMagazzino" & "_p" & Qs_Piva & Format(Date.Now(), "yyyy_MM_dd")
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


        Try
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                          "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                          "&NomePdf=" & Stringa_Codifica(nomeFile, AgroKey_EncoderDecoder, Server))

        Catch ex As Exception
            'MS TODO Manca gestione log errori quindi commento.
            'Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub

    '###########################################################################
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 08/03/2012
    'a Buzzi di agrisol dava fastidio che un carico di magazzino (rilievo giacenza) fatto alla data di inizio del filtro
    'venisse visualizzato nella colonna qta acquistata e non giacenza iniziale
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 31/03/2009
    'altrimenti comparivano in scheda anche i prodotti entrati in magazzino dopo l'intervallo temporale selezionato
    Private Sub Carica_DSFertilizzantiMagazzino_OLD(ByRef DSFertilizzantiMagazzino As DS_FertilizzantiMagazzino)


        Dim Fertilizzanti_R As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

        Dim objSQL As New AgronicaCoreDataProvider.DatatableUtility
        'Dim objSQL As New Codex_Utility_Sql_DistinctOnDT
        Dim strErr As String
        Dim sSql As String
        Dim i As Integer

        Dim Qta As Decimal
        Dim Giacenza_Iniziale As String
        Dim Giacenza_Finale As String

        Dim PrimaVolta As Boolean = True
        Dim Count As Integer = 0

        Dim strFer_Cod As String = ""
        Dim strMat_Cod As String = ""

        Dim objGiacenze As New AgronicaCoreStampeDAL.Magazzino
        Dim DT_Giacenze As DataTable
        Dim DataFiltro_GiornoPrima As Date

        DataFiltro_GiornoPrima = CDate(QS_DataInizio).AddDays(-1)

        DT_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltro_GiornoPrima,
                                                            Qs_Piva,
                                                            Qs_Sa_Cod,
                                                            Qs_Fabbricato_Cod,
                                                            FERTILIZZANTI,
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
                                                        FERTILIZZANTI,
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

        Dim Dt_Finali As DataTable
        Dt_Finali = objGiacenze.SchedaGiacenzeMagazzino(QS_DataFine,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FERTILIZZANTI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        False,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)


        'Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto    
        sSql = ""

        sSql += " SELECT DISTINCT "
        sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ISNULL(Materie_Prime.Mat_Des, '') AS MAt_Des, "
        sSql += " Fertilizzanti.N, Fertilizzanti.P2O5, Fertilizzanti.K2O, "
        sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, Movimenti_dettagli.Pendente "

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
        sSql += "               Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod "

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
        sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FERTILIZZANTI) + " "
        '--------------
        'Modifica del 08/03/2012: escludo dalla lettura i carichi rilievo giacenze alla data di inizio del filtro (vengono conteggiati nelel giacenze iniziali)
        If FiltroRilievi <> "" Then
            sSql += " AND Movimenti_Dettagli.Id_Agenda NOT IN " + Agro_SQL_Save_Clausola_IN(CStr(FiltroRilievi), False) + " "
        End If
        '--------------

        sSql += " ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento"



        'carico il dataset coi dati..
        Dim dataProvider As New AgronicaCoreDataProvider.DataProvider
        dataProvider.EseguiQuery_Lettura(
            objParametri_Server,
            sSql,
            "Carica_DSFertilizzantiMagazzino_OLD",
            DSFertilizzantiMagazzino,
            DSFertilizzantiMagazzino.DataSetName
        )


        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility

            Dim sFer_Cod() As String = objSqlDis.SelectDistinct(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino, "Pro_Cod")
            Dim sMat_Cod() As String = objSqlDis.SelectDistinct(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino, "Mat_Cod")

            If Not IsNothing(sFer_Cod) Then
                For i = 0 To UBound(sFer_Cod)
                    If sFer_Cod(i) <> "0" Then
                        strFer_Cod &= sFer_Cod(i) & ","
                    End If
                Next
            End If
            If strFer_Cod <> "" Then
                strFer_Cod = Left(strFer_Cod, strFer_Cod.Length - 1)
            End If

            If Not IsNothing(sMat_Cod) Then
                For i = 0 To UBound(sMat_Cod)
                    If sMat_Cod(i) <> "0" Then
                        strMat_Cod &= sMat_Cod(i) & ","
                    End If
                Next
            End If
            If strMat_Cod <> "" Then
                strMat_Cod = Left(strMat_Cod, strMat_Cod.Length - 1)
            End If

            '-----------------------------------------

            Dim Fer_Cod(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Count - 1) As Integer
            Dim Udm_Cod(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Count - 1) As Integer


            For i = 0 To DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Count - 1

                Dim drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows(i)

                'Inserisco l'ANNO
                drR.Annata = QS_DataInizio & " - " & QS_DataFine


                'Inserisco la DITTA
                drR.Pro_Cod = IIf(Not IsDBNull(drR.Pro_Cod), drR.Pro_Cod, 0)
                drR.Mat_Cod = IIf(Not IsDBNull(drR.Mat_Cod), drR.Mat_Cod, 0)

                Select Case drR.Mat_Cod

                    Case 0
                        drR.Pro_Des = drR.Fer_Des
                        drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(drR.Pro_Cod, Nothing, objParametri_Server)

                        Fer_Cod(i) = drR.Pro_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                    Case Else
                        drR.Pro_Des = drR.Mat_Des
                        drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(drR.Mat_Cod, Nothing, objParametri_Server)

                        Fer_Cod(i) = drR.Mat_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                End Select

                'If drR.Pro_Des <> "" Then
                '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des") & " - " & drR.Pro_Des
                'Else
                '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des")
                'End If

                'Formatto la DATA MOVIMENTO
                drR.Data_Movimento = CDate(drR.Data_Movimento).ToShortDateString


                'se sono già stati fatti carichi di questo prodotto
                If (PrimaVolta = False) Then

                    If (Fer_Cod(i) = Fer_Cod(i - 1)) And (Udm_Cod(i) = Udm_Cod(i - 1)) Then

                        'drR.Giacenza_Iniziale = ""
                        Giacenza_Iniziale = ""

                        'Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                        '           CStr(Qs_Piva), _
                        '           CInt(Qs_Sa_Cod), _
                        '           CInt(Qs_Fabbricato_Cod), _
                        '           drR.Elem_Cod, _
                        '           drR.Pro_Cod, _
                        '           drR.Mat_Cod, _
                        '           0, _
                        '           0, _
                        '           "", _
                        '           0, _
                        '           drR.Udm_Cod, _
                        '           CDate("01/01/1900"), _
                        '           CDate(QS_DataFine), _
                        '           , )


                        ''drR.Giacenza_Finale = Format(Qta, "##0.###")
                        'Giacenza_Finale = Qta

                        Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                drR.Pro_Cod,
                                                                drR.Udm_Cod)


                        DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows(i - 1).Item("Giacenza_Finale") = ""

                    Else
                        'modifica del 08/03/2012

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


                    End If

                Else
                    '--------------------------------------
                    'entra solo la prima volta
                    '--------------------------------------

                    'modifica del 08/03/2012

                    ''controllo che il carico nn sia fatto il giorno iniziale del filtro..
                    ''se lo è la giacenza iniziale la forzo a zero
                    'If CDate(drR.Data_Movimento) <> CDate(QS_DataInizio) Then

                    '    Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                    '                                                           CStr(Qs_Piva), _
                    '                                                           CInt(Qs_Sa_Cod), _
                    '                                                           CInt(Qs_Fabbricato_Cod), _
                    '                                                           drR.Elem_Cod, _
                    '                                                           drR.Pro_Cod, _
                    '                                                           drR.Mat_Cod, _
                    '                                                           0, _
                    '                                                           0, _
                    '                                                           "", _
                    '                                                           0, _
                    '                                                           drR.Udm_Cod, _
                    '                                                           CDate("01/01/1900"), _
                    '                                                           CDate(QS_DataInizio), _
                    '                                                           , )


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

                    'Qta = NewCom_Verifica_Giacenze(Server, Session, Page, _
                    '                       CStr(Qs_Piva), _
                    '                       CInt(Qs_Sa_Cod), _
                    '                       CInt(Qs_Fabbricato_Cod), _
                    '                       drR.Elem_Cod, _
                    '                       drR.Pro_Cod, _
                    '                       drR.Mat_Cod, _
                    '                       0, _
                    '                       0, _
                    '                       "", _
                    '                       0, _
                    '                       drR.Udm_Cod, _
                    '                       CDate("01/01/1900"), _
                    '                       CDate(QS_DataFine), _
                    '                       , )

                    ''drR.Giacenza_Finale = Format(Qta, "##0.###")
                    'Giacenza_Finale = Qta

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
        'devo aggiungere una query per leggere i carichi dall'01/01/1900 alla data inizio,
        'verificare la quantità e, se <> 0, visualizzarla nella scheda

        'Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto          

        sSql = ""
        sSql += " SELECT DISTINCT "
        sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ISNULL(Materie_Prime.Mat_Des, '') AS MAt_Des, "
        sSql += " Fertilizzanti.N, Fertilizzanti.P2O5, Fertilizzanti.K2O, Materie_Prime.N AS M_N, Materie_Prime.P2O5 AS M_P2O5, Materie_Prime.K2O AS M_K2O, "
        sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, Movimenti_dettagli.Pendente "

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
        sSql += "               Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod "

        sSql += " WHERE  Movimenti_Dettagli.Piva = '" & Agro_SQL_SaveText(Qs_Piva) & "'   "
        sSql += " AND Movimenti_Dettagli.Sa_Cod = " & Agro_SQL_SaveNum(Qs_Sa_Cod) & "   "
        sSql += " AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Qs_Fabbricato_Cod) & " "
        sSql += " AND Mov_Destinazioni.Tipo_Destinazione = " + CStr(MAGAZZINO) + " "
        sSql += " AND (Movimenti.Cau_Mov = '" + CAU_CARICO + "')"
        sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FERTILIZZANTI) + " "

        'modifica del 8/3/2012:
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
        If strFer_Cod <> "" Then
            sSql += " AND Movimenti_Dettagli.Pro_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(strFer_Cod, False) & ")"
        End If

        If strMat_Cod <> "" Then
            sSql += " AND Movimenti_Dettagli.Mat_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(strMat_Cod, False) & ")"
        End If


        'sSql += " ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento"

        sSql += "ORDER BY Fer_Des, Mat_Des ASC "


        Dim Dt As DataTable

        'carico il dataset coi dati..
        Dt = dataProvider.EseguiQuery_Lettura(objParametri_Server, sSql, "Carica_DSFertilizzantiMagazzino_OLD")



        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            Dim Qta_Verifica_Inizio As Decimal = 0
            Dim Qta_Verifica_Fine As Decimal = 0

            Dim Hash_Fertilizzanti As New Hashtable

            Dim Chiave_Giacenza As String
            Dim Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod As Integer

            'ciclo su tutte le righe del dataset per inserirgli l'anno
            For i = 0 To Dt.Rows.Count - 1

                Elem_Cod = Dt.Rows(i).Item("Elem_Cod")
                Pro_Cod = Dt.Rows(i).Item("Pro_Cod")
                Mat_Cod = Dt.Rows(i).Item("Mat_Cod")
                Udm_Cod = Dt.Rows(i).Item("Udm_Cod")

                Chiave_Giacenza = CStr(Elem_Cod) + "|" + CStr(Pro_Cod) + "|" + CStr(Mat_Cod) + "|" + CStr(Udm_Cod)

                If Not Hash_Fertilizzanti.Contains(Chiave_Giacenza) Then

                    Hash_Fertilizzanti.Add(Chiave_Giacenza, "")

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




                    'FACCIO UN ARROTONDAMENTO PERCHè MOLTE VOLTE CAPITA UNA QTA INFINITESAMENTE PICCOLA
                    Qta_Verifica_Inizio = Math.Round(Qta_Verifica_Inizio, 6, MidpointRounding.AwayFromZero)
                    Qta_Verifica_Fine = Math.Round(Qta_Verifica_Fine, 6, MidpointRounding.AwayFromZero)

                    'controllo la giacenza iniziale, perchè se è 0, è 0 anche la finale
                    '(a meno che uno non abbia scaricato quello non aveva!)
                    If Qta_Verifica_Inizio <> 0 Then

                        Dim drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.NewDS_FertilizzantiMagazzinoRow

                        '<xs:element name="Cat_Cod" type="xs:int" minOccurs="0" />
                        '<xs:element name="Cau_Mov" type="xs:string" minOccurs="0" />
                        '<xs:element name="Piva" type="xs:string" minOccurs="0" />
                        '<xs:element name="Rag_Soc" type="xs:string" minOccurs="0" />
                        '<xs:element name="Sa_Cod" type="xs:int" minOccurs="0" />
                        '<xs:element name="Sa_Nome" type="xs:string" minOccurs="0" />
                        '<xs:element name="Fabbicato_Cod" type="xs:int" minOccurs="0" />
                        '<xs:element name="Fabbricato_Des" type="xs:string" minOccurs="0" />
                        '<xs:element name="Ditta_Cod" type="xs:int" minOccurs="0" />
                        '<xs:element name="Provincia" type="xs:string" minOccurs="0" />

                        'If Not Dt.Rows(i).Item("N") Is DBNull.Value Then
                        '    drR.N = Dt.Rows(i).Item("N")
                        'Else
                        '    drR.N = Dt.Rows(i).Item("M_N")
                        'End If

                        'If Not Dt.Rows(i).Item("P2O5") Is DBNull.Value Then
                        '    drR.P2O5 = Dt.Rows(i).Item("P2O5")
                        'Else
                        '    drR.P2O5 = Dt.Rows(i).Item("M_P2O5")
                        'End If

                        'If Not Dt.Rows(i).Item("K2O") Is DBNull.Value Then
                        '    drR.K2O = Dt.Rows(i).Item("K2O")
                        'Else
                        '    drR.K2O = Dt.Rows(i).Item("M_K2O")
                        'End If

                        drR.Pendente = Dt.Rows(i).Item("Pendente")

                        'Inserisco l'ANNO
                        drR.Annata = QS_DataInizio & " - " & QS_DataFine

                        drR.Elem_Cod = Elem_Cod
                        drR.Cat_Des = Dt.Rows(i).Item("Cat_Des")
                        drR.Pro_Cod = IIf(Not IsDBNull(Pro_Cod), Pro_Cod, 0)
                        drR.Mat_Cod = IIf(Not IsDBNull(Mat_Cod), Mat_Cod, 0)
                        drR.Udm_Cod = Udm_Cod
                        drR.Udm_Sim = CStr(Dt.Rows(i).Item("Udm_Sim"))

                        drR.Fer_Des = Dt.Rows(i).Item("Fer_Des")
                        drR.Mat_Des = Dt.Rows(i).Item("Mat_Des")

                        drR.Data_Movimento = ""

                        Select Case Mat_Cod
                            Case 0
                                drR.Pro_Des = Dt.Rows(i).Item("Fer_Des")
                                drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(Pro_Cod, Nothing, objParametri_Server)

                                drR.N = Dt.Rows(i).Item("N")
                                drR.P2O5 = Dt.Rows(i).Item("P2O5")
                                drR.K2O = Dt.Rows(i).Item("K2O")

                            Case Else
                                drR.Pro_Des = Dt.Rows(i).Item("Mat_Des")
                                drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(Mat_Cod, Nothing, objParametri_Server)

                                drR.N = Dt.Rows(i).Item("M_N")
                                drR.P2O5 = Dt.Rows(i).Item("M_P2O5")
                                drR.K2O = Dt.Rows(i).Item("M_K2O")

                        End Select

                        'If drR.Pro_Des <> "" Then
                        '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des") & " - " & drR.Pro_Des
                        'Else
                        '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des")
                        'End If

                        'drR.Giacenza_Iniziale = Format(cdec(drR.Qta), "##0.###")
                        Giacenza_Iniziale = Qta_Verifica_Inizio

                        'drR.Giacenza_Finale = Format(cdec(drR.Qta), "##0.###")
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
                        DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Add(drR)

                    End If 'se la giacenza iniziale è <> 0

                Else
                    'formulato già inserito in scheda con qta verificata
                End If

            Next 'per ogni movimento

            'rendo permanenti le modifiche sul ds
            DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.AcceptChanges()

            Hash_Fertilizzanti = Nothing


        End If


        '##################################################################


        '--------------------------------------------------------------------------------------
        'ORDINO IL DATASET
        '(per farlo importo nell'ordine le datarow ordinate e poi elimino le vecchie righe)
        '--------------------------------------------------------------------------------------

        Dim Righe_Old As Integer
        Dim dr As DataRow

        Count = 0

        Righe_Old = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Count

        If Righe_Old > 0 Then

            'Ottiene una matrice di tutti gli oggetti DataRow che corrispondono al filtro, in base all'ordinamento specificato
            For Each dr In DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Select("", "Fer_Des, Mat_Des")

                DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.ImportRow(dr)

                Count += 1

            Next

            For i = 0 To Righe_Old - 1
                DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows(i).Delete()
            Next

            'rendo permanenti le modifiche sul ds
            DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.AcceptChanges()

        End If


    End Sub



    '###########################################################################
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 08/03/2012
    'a Buzzi di agrisol dava fastidio che un carico di magazzino (rilievo giacenza) fatto alla data di inizio del filtro
    'venisse visualizzato nella colonna qta acquistata e non giacenza iniziale
    'ATTENZIONE: MODIFICATA ROUTINE IN DATA 31/03/2009
    'altrimenti comparivano in scheda anche i prodotti entrati in magazzino dopo l'intervallo temporale selezionato
    Private Sub Carica_DSFertilizzantiMagazzino(ByRef DSFertilizzantiMagazzino As DS_FertilizzantiMagazzino)


        Dim Fertilizzanti_R As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R

        Dim objSQL As New AgronicaCoreDataProvider.DatatableUtility
        'Dim objSQL As New Codex_Utility_Sql_DistinctOnDT
        Dim strErr As String
        Dim sSql As String
        Dim i As Integer

        Dim Qta As Decimal
        Dim Giacenza_Iniziale As String
        Dim Giacenza_Finale As String

        Dim PrimaVolta As Boolean = True
        Dim Count As Integer = 0

        Dim strFer_Cod As String = ""
        Dim strMat_Cod As String = ""

        Dim objGiacenze As New AgronicaCoreStampeDAL.Magazzino
        Dim DT_Giacenze As DataTable


        Dim objFert As New AgronicaCoreMetaSchemaDAL.Fertilizzanti_R
        Dim DT_FertDitte As DataTable = objFert.Leggi_conDitte(0, 0,
                                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta,
                                            "",
                                            "",
                                            objParametri_Server)

        'Dim objFertDitte As New AgronicaCoreMetaSchemaDAL.FertilizzantixDitte_R
        'Dim DT_FertDitte As DataTable = objFertDitte.Leggi(0, 0, _
        '                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
        '                                    "", _
        '                                    "", _
        '                                    objParametri_Server)



        Dim DataFiltro_GiornoPrima As Date
        DataFiltro_GiornoPrima = CDate(QS_DataInizio).AddDays(-1)

        'lettura giacenze di tutti i fertilizzanti al giorno prima della data inizio del filtro
        DT_Giacenze = objGiacenze.SchedaGiacenzeMagazzino(DataFiltro_GiornoPrima,
                                                            Qs_Piva,
                                                            Qs_Sa_Cod,
                                                            Qs_Fabbricato_Cod,
                                                            FERTILIZZANTI,
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

        'cerco i rilievi giacenze di tutti i fertilizzanti registrati alla data di inizio del filtro
        DT_Rilievi = objGiacenze.SchedaMovimentiMagazzino(QS_DataInizio,
                                                        QS_DataInizio,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FERTILIZZANTI,
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

        'giacenze di tutti i fertilizzanti alla data di fine del filtro
        Dim Dt_Finali As DataTable
        Dt_Finali = objGiacenze.SchedaGiacenzeMagazzino(QS_DataFine,
                                                        Qs_Piva,
                                                        Qs_Sa_Cod,
                                                        Qs_Fabbricato_Cod,
                                                        FERTILIZZANTI,
                                                        0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                        False,
                                                        "", "", "", "", "", "", "", "", "", "", "",
                                                        "", "",
                                                        objParametri_Server, objParametri_Utenti)

        Dim HT_ProCod_Carichi As New Hashtable


        'Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto    
        sSql = ""

        'sSql += " SELECT DISTINCT "
        sSql += " SELECT  "
        sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ISNULL(Materie_Prime.Mat_Des, '') AS MAt_Des, "
        sSql += " Fertilizzanti.N, Fertilizzanti.P2O5, Fertilizzanti.K2O, "
        sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, Movimenti_dettagli.Pendente "

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
        sSql += "               Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod "

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
        sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FERTILIZZANTI) + " "
        '--------------
        'Modifica del 08/03/2012: escludo dalla lettura i carichi rilievo giacenze alla data di inizio del filtro (vengono conteggiati nelel giacenze iniziali)
        If FiltroRilievi <> "" Then
            sSql += " AND Movimenti_Dettagli.Id_Agenda NOT IN " + Agro_SQL_Save_Clausola_IN(CStr(FiltroRilievi), False) + " "
        End If
        '--------------

        sSql += " ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento"


        'carico il dataset coi dati..
        'objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
        '                    Session("ASG_Connessione_Server"), _
        '                    sSql, _
        '                    DSFertilizzantiMagazzino, _
        '                    DSFertilizzantiMagazzino.DataSetName, _
        '                    strErr)

        Dim dataProvider As New AgronicaCoreDataProvider.DataProvider

        dataProvider.EseguiQuery_Lettura(objParametri_Server, sSql, "Carica_DSFertilizzantiMagazzino", DSFertilizzantiMagazzino, "DS_FertilizzantiMagazzino")



        'controllo errori....
        If Not IsNothing(strErr) Then
            Throw New ApplicationException(strErr)
        Else

            Dim objSqlDis As New AgronicaCoreDataProvider.DatatableUtility
            'Dim objSqlDis As New Codex_Utility_Sql_DistinctOnDT

            Dim sFer_Cod() As String = objSqlDis.SelectDistinct(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino, "Pro_Cod")
            Dim sMat_Cod() As String = objSqlDis.SelectDistinct(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino, "Mat_Cod")

            Dim chiave As String

            If Not IsNothing(sFer_Cod) Then
                For i = 0 To UBound(sFer_Cod)
                    If sFer_Cod(i) <> "0" Then
                        chiave = sFer_Cod(i) & "|0"
                        If Not HT_ProCod_Carichi.Contains(chiave) Then
                            'modifica del 26/07/2012:
                            'memorizzo nell'HT gli fr_cod con carichi nell'intervallo
                            HT_ProCod_Carichi.Add(chiave, "")
                            strFer_Cod &= sFer_Cod(i) & ","
                        End If
                    End If
                Next
            End If
            If strFer_Cod <> "" Then
                strFer_Cod = Left(strFer_Cod, strFer_Cod.Length - 1)
            End If

            If Not IsNothing(sMat_Cod) Then
                For i = 0 To UBound(sMat_Cod)
                    If sMat_Cod(i) <> "0" Then
                        chiave = "0|" & sMat_Cod(i)
                        If Not HT_ProCod_Carichi.Contains(chiave) Then
                            'modifica del 26/07/2012:
                            'memorizzo nell'HT gli fr_cod con carichi nell'intervallo
                            HT_ProCod_Carichi.Add(chiave, "")
                            strMat_Cod &= sMat_Cod(i) & ","
                        End If
                    End If
                Next
            End If
            If strMat_Cod <> "" Then
                strMat_Cod = Left(strMat_Cod, strMat_Cod.Length - 1)
            End If

            '-----------------------------------------

            Dim Fer_Cod(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Count - 1) As Integer
            Dim Udm_Cod(DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Count - 1) As Integer

            Dim Codice_Prod As Integer


            For i = 0 To DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Count - 1

                Dim drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows(i)

                'Inserisco l'ANNO
                drR.Annata = QS_DataInizio & " - " & QS_DataFine


                'Inserisco la DITTA
                drR.Pro_Cod = IIf(Not IsDBNull(drR.Pro_Cod), drR.Pro_Cod, 0)
                drR.Mat_Cod = IIf(Not IsDBNull(drR.Mat_Cod), drR.Mat_Cod, 0)

                Select Case drR.Mat_Cod

                    Case 0
                        drR.Pro_Des = drR.Fer_Des
                        drR.Ditta_Des = Des_from_Cod(DT_FertDitte, "Fer_Cod", "Ditta_des", drR.Pro_Cod) 'Fertilizzanti_R.DittaDes_from_FerCod(drR.Pro_Cod, Nothing, objParametri_Server)

                        Fer_Cod(i) = drR.Pro_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                    Case Else
                        drR.Pro_Des = drR.Mat_Des
                        drR.Ditta_Des = "Concime aziendale" 'Fertilizzanti_R.DittaDes_from_FerCod(drR.Mat_Cod, Nothing, objParametri_Server)

                        Fer_Cod(i) = drR.Mat_Cod
                        Udm_Cod(i) = drR.Udm_Cod

                End Select

                'If drR.Pro_Des <> "" Then
                '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des") & " - " & drR.Pro_Des
                'Else
                '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des")
                'End If

                'Formatto la DATA MOVIMENTO
                drR.Data_Movimento = CDate(drR.Data_Movimento).ToShortDateString


                'se sono già stati fatti carichi di questo prodotto
                If (PrimaVolta = False) Then

                    If (Fer_Cod(i) = Fer_Cod(i - 1)) And (Udm_Cod(i) = Udm_Cod(i - 1)) Then

                        Giacenza_Iniziale = ""

                        Select Case drR.Mat_Cod
                            Case 0
                                Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                        drR.Pro_Cod,
                                                                        drR.Udm_Cod)

                            Case Else
                                Giacenza_Finale = Ricava_Giacenza_Finale2(Dt_Finali,
                                                                        drR.Mat_Cod,
                                                                        drR.Udm_Cod)

                        End Select

                        DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows(i - 1).Item("Giacenza_Finale") = ""

                    Else
                        'modifica del 08/03/2012

                        'Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze, _
                        '                                            DT_Rilievi, _
                        '                                            drR.Pro_Cod, _
                        '                                            drR.Udm_Cod)

                        'Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali, _
                        '                                       drR.Pro_Cod, _
                        '                                       drR.Udm_Cod)

                        'MODIFICA DEL 26/07/2012
                        Select Case drR.Mat_Cod
                            Case 0
                                Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze,
                                                                            DT_Rilievi,
                                                                            drR.Pro_Cod,
                                                                            drR.Udm_Cod)

                                Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                        drR.Pro_Cod,
                                                                        drR.Udm_Cod)

                            Case Else

                                Giacenza_Iniziale = Ricava_Giacenza_Iniziale2(DT_Giacenze,
                                                                            DT_Rilievi,
                                                                            drR.Mat_Cod,
                                                                            drR.Udm_Cod)

                                Giacenza_Finale = Ricava_Giacenza_Finale2(Dt_Finali,
                                                                            drR.Mat_Cod,
                                                                            drR.Udm_Cod)

                        End Select

                    End If

                Else
                    '--------------------------------------
                    'entra solo la prima volta
                    '--------------------------------------

                    'modifica del 08/03/2012

                    'Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze, _
                    '                                         DT_Rilievi, _
                    '                                         drR.Pro_Cod, _
                    '                                         drR.Udm_Cod)

                    'Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali, _
                    '                                   drR.Pro_Cod, _
                    '                                   drR.Udm_Cod)

                    'MODIFICA DEL 26/07/2012
                    Select Case drR.Mat_Cod
                        Case 0
                            Giacenza_Iniziale = Ricava_Giacenza_Iniziale(DT_Giacenze,
                                                          DT_Rilievi,
                                                          drR.Pro_Cod,
                                                          drR.Udm_Cod)

                            Giacenza_Finale = Ricava_Giacenza_Finale(Dt_Finali,
                                                                    drR.Pro_Cod,
                                                                    drR.Udm_Cod)

                        Case Else

                            Giacenza_Iniziale = Ricava_Giacenza_Iniziale2(DT_Giacenze,
                                                                       DT_Rilievi,
                                                                       drR.Mat_Cod,
                                                                       drR.Udm_Cod)

                            Giacenza_Finale = Ricava_Giacenza_Finale2(Dt_Finali,
                                                                        drR.Mat_Cod,
                                                                        drR.Udm_Cod)

                    End Select

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
                            drR.Qta = Math.Round(CDec(drR.Qta), 2)
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

        'lettura giacenze di tutti i fertilizzanti alla data inizio del filtro
        Dim DT_GiacInizio As DataTable
        DT_GiacInizio = objGiacenze.SchedaGiacenzeMagazzino(QS_DataInizio, _
                                                            Qs_Piva, _
                                                            Qs_Sa_Cod, _
                                                            Qs_Fabbricato_Cod, _
                                                            FERTILIZZANTI, _
                                                            0, 0, 0, 0, 0, 0, LOTTO_NONDEFINITO, _
                                                            True, _
                                                            "", "", "", "", "", "", "", "", "", "", "", _
                                                            "", "", _
                                                            objParametri_Server, objParametri_Utenti)

        Dim Mat_Cod, Pro_Cod, UdmCod As Integer
        Dim ProCod_GiacenzaInizio, ProCod_GiacenzaFine As Decimal
        Dim key As String

        If DT_GiacInizio.Rows.Count > 0 Then

            Dim filtro As String

            'Dim objFertDitte As New AgronicaCoreMetaSchemaDAL.FertilizzantixDitte_R
            'Dim DT_FertDitte As DataTable

            'If strFer_Cod <> "" Then
            '    filtro = "  Fertilizzanti.FER_COD NOT IN (" & strFer_Cod & ")"
            'End If

            ''escludo i fertilizzanti già elaborati, che di sicuro qui non analizzo
            ''in realtà andrebbero filtrati quelli di DT_GiacInizio
            'DT_FertDitte = objFertDitte.Leggi(0, 0, _
            '                                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
            '                                    filtro, _
            '                                    "", _
            '                                    objParametri_Server)

            Dim objMP As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
            Dim DT_MP As DataTable

            filtro = ""

            If strMat_Cod <> "" Then
                filtro = "  Materie_Prime.Mat_Cod NOT IN (" & Agro_SQL_Save_Clausola_IN(strMat_Cod, False) & ") "
            End If

            'escludo i fertilizzanti già elaborati, che di sicuro qui non analizzo
            'in realtà andrebbero filtrati quelli di DT_GiacInizio
            DT_MP = objMP.Leggi(Qs_Piva, 0, FERTILIZZANTI, 0, "", _
                                0, 0, 0, 0, 0, 0, 0, _
                                "", 0, "", True, _
                                False, "", _
                                 AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                                  filtro, _
                                "", _
                                objParametri_Server)

            For i = 0 To DT_GiacInizio.Rows.Count - 1

                Pro_Cod = DT_GiacInizio.Rows(i).Item("pro_cod")
                Mat_Cod = DT_GiacInizio.Rows(i).Item("Mat_Cod")

                key = CStr(Pro_Cod) & "|" & CStr(Mat_Cod)

                ' se il prodotto non è presente tra quelli movimentati nell'intervallo selezionato
                'devo aggiungerlo alla scheda con la sua giacenza iniziale e finale
                If Not HT_ProCod_Carichi.Contains(key) Then

                    ProCod_GiacenzaInizio = DT_GiacInizio.Rows(i).Item("Giacenza")

                    'oltre al filtro nella query, devo fare anche il filtro su codice
                    'perchè molti decimal vengono salvati come valori infinatamente piccoli
                    'ad esempio 0.00003680000000017003
                    If ProCod_GiacenzaInizio <> 0 And Not (ProCod_GiacenzaInizio < QTA_GiancenzeVisualizzate And ProCod_GiacenzaInizio > -QTA_GiancenzeVisualizzate) Then

                        UdmCod = DT_GiacInizio.Rows(i).Item("Udm_Cod")

                        If Pro_Cod <> 0 Then
                            ProCod_GiacenzaFine = Ricava_Giacenza_Finale(Dt_Finali,
                                                                        Pro_Cod,
                                                                        UdmCod)
                        Else
                            ProCod_GiacenzaFine = Ricava_Giacenza_Finale2(Dt_Finali,
                                                                            Mat_Cod,
                                                                            UdmCod)
                        End If

                        Dim drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.NewDS_FertilizzantiMagazzinoRow


                        drR.Pendente = 0 'Dt.Rows(i).Item("Pendente")

                        'Inserisco l'ANNO
                        drR.Annata = QS_DataInizio & " - " & QS_DataFine

                        drR.Elem_Cod = FERTILIZZANTI

                        drR.Pro_Cod = IIf(Not IsDBNull(Pro_Cod), Pro_Cod, 0)
                        drR.Mat_Cod = IIf(Not IsDBNull(Mat_Cod), Mat_Cod, 0)
                        drR.Udm_Cod = UdmCod
                        drR.Udm_Sim = CStr(DT_GiacInizio.Rows(i).Item("Udm_Sim"))

                        drR.Fer_Des = DT_GiacInizio.Rows(i).Item("Descrizione_Prodotto") 'Dt.Rows(i).Item("Fer_Des")
                        drR.Mat_Des = DT_GiacInizio.Rows(i).Item("Descrizione_Prodotto") 'Dt.Rows(i).Item("Mat_Des")

                        drR.Data_Movimento = ""

                        Select Case Mat_Cod
                            Case 0
                                drR.Pro_Des = DT_GiacInizio.Rows(i).Item("Descrizione_Prodotto") 'Dt.Rows(i).Item("Fer_Des")

                                Dettagli_Fertilizzanti(DT_FertDitte,
                                                        Pro_Cod,
                                                        drR)

                                'drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(Pro_Cod, Nothing, objParametri_Server)

                                'drR.N = Dt.Rows(i).Item("N")
                                'drR.P2O5 = Dt.Rows(i).Item("P2O5")
                                'drR.K2O = Dt.Rows(i).Item("K2O")

                            Case Else
                                drR.Pro_Des = DT_GiacInizio.Rows(i).Item("Descrizione_Prodotto") 'Dt.Rows(i).Item("Mat_Des")

                                Dettagli_MateriePrime(DT_MP,
                                                     Mat_Cod,
                                                     drR)

                                'drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(Mat_Cod, Nothing, objParametri_Server)

                                'drR.N = Dt.Rows(i).Item("M_N")
                                'drR.P2O5 = Dt.Rows(i).Item("M_P2O5")
                                'drR.K2O = Dt.Rows(i).Item("M_K2O")

                        End Select

                        drR.Cat_Des = DT_GiacInizio.Rows(i).Item("NomeComune")

                        'If drR.Pro_Des <> "" Then
                        '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des") & " - " & drR.Pro_Des
                        'Else
                        '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des")
                        'End If

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
                        DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Add(drR)

                    End If 'prodotto con giacenza valorizzata


                End If 'prodotto non presente nell'intervallo

            Next 'ciclo giacenze alla data inizio

        End If

        ''##################################################################


        ''Modifica del 31/03/2009: visto che ho eliminato la query di lettura delle giacenze non movimentate nel periodo temporale
        ''devo aggiungere una query per leggere i carichi dall'01/01/1900 alla data inizio,
        ''verificare la quantità e, se <> 0, visualizzarla nella scheda

        ''Leggo i CARICHI dei prodotti movimentati nell'intervallo scelto          

        'sSql = ""
        'sSql += " SELECT DISTINCT "
        'sSql += " Mov_Destinazioni.Qta, UnitaMisura.UDM_COD, UnitaMisura.UDM_SIM, "
        'sSql += " CategorieMagazzino.NomeComune AS Cat_Des, ISNULL(Fertilizzanti.Fer_Des, '') AS Fer_Des, ISNULL(Materie_Prime.Mat_Des, '') AS MAt_Des, "
        'sSql += " Fertilizzanti.N, Fertilizzanti.P2O5, Fertilizzanti.K2O, Materie_Prime.N AS M_N, Materie_Prime.P2O5 AS M_P2O5, Materie_Prime.K2O AS M_K2O, "
        'sSql += " Movimenti_dettagli.Elem_Cod, Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, "
        'sSql += " Movimenti_dettagli.Id_Agenda, Agenda.Lav_Cod, Movimenti.Data_Movimento, Movimenti_dettagli.Pendente "

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
        'sSql += "               Fertilizzanti ON Movimenti_dettagli.Pro_Cod = Fertilizzanti.Fer_Cod "

        'sSql += " WHERE  Movimenti_Dettagli.Piva = '" & SQL_SaveText(Qs_Piva) & "'   "
        'sSql += " AND Movimenti_Dettagli.Sa_Cod = " & SQL_SaveNum(Qs_Sa_Cod) & "   "
        'sSql += " AND Mov_Destinazioni.Id_Destinazione = " & SQL_SaveNum(Qs_Fabbricato_Cod) & " "
        'sSql += " AND Mov_Destinazioni.Tipo_Destinazione = " + CStr(MAGAZZINO) + " "
        'sSql += " AND (Movimenti.Cau_Mov = '" + CAU_CARICO + "')"
        'sSql += " AND Movimenti_Dettagli.Elem_Cod = " + CStr(FERTILIZZANTI) + " "

        ''modifica del 8/3/2012:
        ''metto in OR gli id_agenda che ho scartato sopra nell'elenco dei carichi
        ''perchè se il prodotto, oltre al carico alla data di inizio, non è stato più movimentato
        ''poi non viene visualizzato nel report
        'sSql += " AND ( "
        'sSql += " ( "
        ''non <= perchè l'= è già considerato nella query sopra
        'sSql += " Movimenti.Data_Movimento < " & SQL_SaveDate(QS_DataInizio)
        'sSql += " AND   Movimenti.Data_Movimento >= " & SQL_SaveDate(AGRODATAINIZIO)
        'sSql += ")"
        'If FiltroRilievi <> "" Then
        '    sSql += " OR "
        '    sSql += " ( "
        '    sSql += " Movimenti_Dettagli.Id_Agenda IN " + CStr(FiltroRilievi) + " "
        '    sSql += ")"
        'End If
        'sSql += "   )"

        ''escludo i prodotti già considerati sopra
        'If strFer_Cod <> "" Then
        '    sSql += " AND Movimenti_Dettagli.Pro_Cod NOT IN (" & strFer_Cod & ")"
        'End If

        'If strMat_Cod <> "" Then
        '    sSql += " AND Movimenti_Dettagli.Mat_Cod NOT IN (" & strMat_Cod & ")"
        'End If


        ''sSql += " ORDER BY Movimenti_dettagli.Pro_Cod, Movimenti_dettagli.Mat_Cod, UnitaMisura.UDM_COD, Movimenti.Data_Movimento"

        'sSql += "ORDER BY Fer_Des, Mat_Des ASC "


        'Dim Dt As DataTable

        ''carico il dataset coi dati..
        'Dt = objSQL.SqlSelect(objParametri_Server.StringaConnessione, _
        '                    Session("ASG_Connessione_Server"), _
        '                    sSql, _
        '                    1, _
        '                    strErr)

        ''controllo errori....
        'If Not IsNothing(strErr) Then
        '    Throw New ApplicationException(strErr)
        'Else

        '    Dim Qta_Verifica_Inizio As decimal = 0
        '    Dim Qta_Verifica_Fine As decimal = 0

        '    Dim Hash_Fertilizzanti As New Hashtable

        '    Dim Chiave_Giacenza As String
        '    Dim Elem_Cod, Pro_Cod, Mat_Cod, Udm_Cod As Integer

        '    'ciclo su tutte le righe del dataset per inserirgli l'anno
        '    For i = 0 To Dt.Rows.Count - 1

        '        Elem_Cod = Dt.Rows(i).Item("Elem_Cod")
        '        Pro_Cod = Dt.Rows(i).Item("Pro_Cod")
        '        Mat_Cod = Dt.Rows(i).Item("Mat_Cod")
        '        Udm_Cod = Dt.Rows(i).Item("Udm_Cod")

        '        Chiave_Giacenza = CStr(Elem_Cod) + "|" + CStr(Pro_Cod) + "|" + CStr(Mat_Cod) + "|" + CStr(Udm_Cod)

        '        If Not Hash_Fertilizzanti.Contains(Chiave_Giacenza) Then

        '            Hash_Fertilizzanti.Add(Chiave_Giacenza, "")

        '            'NON è vero che per le giacenze che non sono movimentate (e quindi non risultano dalla query sopra)
        '            'la giacenza iniziale è = a quella finale, perchè nel mezzo possono esserci stati degli scarichi!

        '            'leggo la giacenza iniziale
        '            Qta_Verifica_Inizio = NewCom_Verifica_Giacenze(Server, Session, Page, _
        '                                                   CStr(Qs_Piva), _
        '                                                   CInt(Qs_Sa_Cod), _
        '                                                   CInt(Qs_Fabbricato_Cod), _
        '                                                   Elem_Cod, _
        '                                                   Pro_Cod, _
        '                                                   Mat_Cod, _
        '                                                   0, _
        '                                                   0, _
        '                                                   "", _
        '                                                   0, _
        '                                                   udm_cod, _
        '                                                   CDate("01/01/1900"), _
        '                                                   CDate(QS_DataInizio), _
        '                                                   , )

        '            'leggo la giacenza finale
        '            Qta_Verifica_Fine = NewCom_Verifica_Giacenze(Server, Session, Page, _
        '                                        CStr(Qs_Piva), _
        '                                        CInt(Qs_Sa_Cod), _
        '                                        CInt(Qs_Fabbricato_Cod), _
        '                                        Elem_Cod, _
        '                                        Pro_Cod, _
        '                                        Mat_Cod, _
        '                                        0, _
        '                                        0, _
        '                                        "", _
        '                                        0, _
        '                                        udm_cod, _
        '                                        CDate("01/01/1900"), _
        '                                        CDate(QS_DataFine), _
        '                                        , )

        '            'FACCIO UN ARROTONDAMENTO PERCHè MOLTE VOLTE CAPITA UNA QTA INFINITESAMENTE PICCOLA
        '            Qta_Verifica_Inizio = Math.Round(Qta_Verifica_Inizio, 6)
        '            Qta_Verifica_Fine = Math.Round(Qta_Verifica_Fine, 6)

        '            'controllo la giacenza iniziale, perchè se è 0, è 0 anche la finale
        '            '(a meno che uno non abbia scaricato quello non aveva!)
        '            If Qta_Verifica_Inizio <> 0 Then

        '                Dim drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.NewDS_FertilizzantiMagazzinoRow

        '                '<xs:element name="Cat_Cod" type="xs:int" minOccurs="0" />
        '                '<xs:element name="Cau_Mov" type="xs:string" minOccurs="0" />
        '                '<xs:element name="Piva" type="xs:string" minOccurs="0" />
        '                '<xs:element name="Rag_Soc" type="xs:string" minOccurs="0" />
        '                '<xs:element name="Sa_Cod" type="xs:int" minOccurs="0" />
        '                '<xs:element name="Sa_Nome" type="xs:string" minOccurs="0" />
        '                '<xs:element name="Fabbicato_Cod" type="xs:int" minOccurs="0" />
        '                '<xs:element name="Fabbricato_Des" type="xs:string" minOccurs="0" />
        '                '<xs:element name="Ditta_Cod" type="xs:int" minOccurs="0" />
        '                '<xs:element name="Provincia" type="xs:string" minOccurs="0" />

        '                'If Not Dt.Rows(i).Item("N") Is DBNull.Value Then
        '                '    drR.N = Dt.Rows(i).Item("N")
        '                'Else
        '                '    drR.N = Dt.Rows(i).Item("M_N")
        '                'End If

        '                'If Not Dt.Rows(i).Item("P2O5") Is DBNull.Value Then
        '                '    drR.P2O5 = Dt.Rows(i).Item("P2O5")
        '                'Else
        '                '    drR.P2O5 = Dt.Rows(i).Item("M_P2O5")
        '                'End If

        '                'If Not Dt.Rows(i).Item("K2O") Is DBNull.Value Then
        '                '    drR.K2O = Dt.Rows(i).Item("K2O")
        '                'Else
        '                '    drR.K2O = Dt.Rows(i).Item("M_K2O")
        '                'End If

        '                drR.Pendente = Dt.Rows(i).Item("Pendente")

        '                'Inserisco l'ANNO
        '                drR.Annata = QS_DataInizio & " - " & QS_DataFine

        '                drR.Elem_Cod = Elem_Cod
        '                drR.cat_des = Dt.Rows(i).Item("Cat_Des")
        '                drR.Pro_Cod = IIf(Not IsDBNull(Pro_Cod), Pro_Cod, 0)
        '                drR.Mat_Cod = IIf(Not IsDBNull(Mat_Cod), Mat_Cod, 0)
        '                drR.udm_cod = udm_cod
        '                drr.Udm_Sim = CStr(Dt.Rows(i).Item("Udm_Sim"))

        '                drR.Fer_Des = Dt.Rows(i).Item("Fer_Des")
        '                drR.Mat_Des = Dt.Rows(i).Item("Mat_Des")

        '                drR.Data_Movimento = ""

        '                Select Case Mat_Cod
        '                    Case 0
        '                        drR.Pro_Des = Dt.Rows(i).Item("Fer_Des")
        '                        drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(Pro_Cod, Nothing, objParametri_Server)

        '                        drR.N = Dt.Rows(i).Item("N")
        '                        drR.P2O5 = Dt.Rows(i).Item("P2O5")
        '                        drR.K2O = Dt.Rows(i).Item("K2O")

        '                    Case Else
        '                        drR.Pro_Des = Dt.Rows(i).Item("Mat_Des")
        '                        drR.Ditta_Des = Fertilizzanti_R.DittaDes_from_FerCod(Mat_Cod, Nothing, objParametri_Server)

        '                        drR.N = Dt.Rows(i).Item("M_N")
        '                        drR.P2O5 = Dt.Rows(i).Item("M_P2O5")
        '                        drR.K2O = Dt.Rows(i).Item("M_K2O")

        '                End Select

        '                'If drR.Pro_Des <> "" Then
        '                '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des") & " - " & drR.Pro_Des
        '                'Else
        '                '    drR.Pro_Des = Dt.Rows(i).Item("Cat_Des")
        '                'End If

        '                'drR.Giacenza_Iniziale = Format(cdec(drR.Qta), "##0.###")
        '                Giacenza_Iniziale = Qta_Verifica_Inizio

        '                'drR.Giacenza_Finale = Format(cdec(drR.Qta), "##0.###")
        '                Giacenza_Finale = Qta_Verifica_Fine

        '                drR.Qta = ""

        '                '******************************************
        '                drR.Giacenza_Iniziale = Giacenza_Iniziale
        '                drR.Giacenza_Finale = Giacenza_Finale
        '                '******************************************

        '                Select Case Qs_Arrotondamento

        '                    Case 0 'nessun arrotondamento


        '                    Case 1 'arrotondamento all'intero
        '                        If drR.Giacenza_Iniziale <> "" Then
        '                            drR.Giacenza_Iniziale = Math.Round(cdec(drR.Giacenza_Iniziale), 0)
        '                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Giacenza_Finale <> "" Then
        '                            drR.Giacenza_Finale = Math.Round(cdec(drR.Giacenza_Finale), 0)
        '                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Qta <> "" Then
        '                            drR.Qta = Math.Round(cdec(drR.Qta), 0)
        '                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
        '                        End If


        '                    Case 2 'arrotondamento al primo decimale
        '                        If drR.Giacenza_Iniziale <> "" Then
        '                            drR.Giacenza_Iniziale = Math.Round(cdec(drR.Giacenza_Iniziale), 1)
        '                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Giacenza_Finale <> "" Then
        '                            drR.Giacenza_Finale = Math.Round(cdec(drR.Giacenza_Finale), 1)
        '                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Qta <> "" Then
        '                            drR.Qta = Math.Round(cdec(drR.Qta), 1)
        '                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                    Case 3 '2 decimali
        '                        If drR.Giacenza_Iniziale <> "" Then
        '                            drR.Giacenza_Iniziale = Math.Round(cdec(drR.Giacenza_Iniziale), 2)
        '                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Giacenza_Finale <> "" Then
        '                            drR.Giacenza_Finale = Math.Round(cdec(drR.Giacenza_Finale), 2)
        '                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Qta <> "" Then
        '                            drR.Qta = Math.Round(cdec(drR.Qta), 2)
        '                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                    Case 4 '3 decimali
        '                        If drR.Giacenza_Iniziale <> "" Then
        '                            drR.Giacenza_Iniziale = Math.Round(cdec(drR.Giacenza_Iniziale), 3)
        '                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Giacenza_Finale <> "" Then
        '                            drR.Giacenza_Finale = Math.Round(cdec(drR.Giacenza_Finale), 3)
        '                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Qta <> "" Then
        '                            drR.Qta = Math.Round(cdec(drR.Qta), 3)
        '                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                    Case 5 '4 decimali
        '                        If drR.Giacenza_Iniziale <> "" Then
        '                            drR.Giacenza_Iniziale = Math.Round(cdec(drR.Giacenza_Iniziale), 4)
        '                            drR.Giacenza_Iniziale = drR.Giacenza_Iniziale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Giacenza_Finale <> "" Then
        '                            drR.Giacenza_Finale = Math.Round(cdec(drR.Giacenza_Finale), 4)
        '                            drR.Giacenza_Finale = drR.Giacenza_Finale & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                        If drR.Qta <> "" Then
        '                            drR.Qta = Math.Round(cdec(drR.Qta), 4)
        '                            drR.Qta = drR.Qta & "  [" & drR.Udm_Sim & "]"
        '                        End If

        '                End Select

        '                Count += 1

        '                'aggiungo la riga
        '                DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows.Add(drR)

        '            End If 'se la giacenza iniziale è <> 0

        '        Else
        '            'formulato già inserito in scheda con qta verificata
        '        End If

        '    Next 'per ogni movimento

        '    'rendo permanenti le modifiche sul ds
        '    DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.AcceptChanges()

        '    Hash_Fertilizzanti = Nothing


        'End If


        '##################################################################

        'rendo permanenti le modifiche sul ds
        DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.AcceptChanges()

        '--------------------------------------------------------------------------------------
        'ORDINO IL DATASET
        '(per farlo importo nell'ordine le datarow ordinate e poi elimino le vecchie righe)
        '--------------------------------------------------------------------------------------

        Dim Righe_Old As Integer
        Dim dr As DataRow

        Count = 0

        Righe_Old = DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Count

        If Righe_Old > 0 Then

            'Ottiene una matrice di tutti gli oggetti DataRow che corrispondono al filtro, in base all'ordinamento specificato
            For Each dr In DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Select("", "Fer_Des, Mat_Des")

                DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.ImportRow(dr)

                Count += 1

            Next

            For i = 0 To Righe_Old - 1
                DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.Rows(i).Delete()
            Next

            'rendo permanenti le modifiche sul ds
            DSFertilizzantiMagazzino.DS_FertilizzantiMagazzino.AcceptChanges()

        End If


    End Sub

    '##############################################################
    Private Sub Dettagli_Fertilizzanti(ByVal DT As DataTable, _
                                        ByVal Fer_Cod As Integer, _
                                        ByRef drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow)

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow = DT.Select(" fer_Cod = " & Agro_SQL_SaveNum(Fer_Cod))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then

                drR.Ditta_Des = If(IsDBNull(Dr(0).Item("Ditta_Des")), "", Dr(0).Item("Ditta_Des"))
                drR.N = Dr(0).Item("N")
                drR.P2O5 = Dr(0).Item("P2O5")
                drR.K2O = Dr(0).Item("K2O")

            End If

        End If

    End Sub

    '##############################################################
    Private Sub Dettagli_MateriePrime(ByVal DT As DataTable, _
                                        ByVal Mat_Cod As Integer, _
                                        ByRef drR As DS_FertilizzantiMagazzino.DS_FertilizzantiMagazzinoRow)

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow

            Dr = DT.Select(" Mat_Cod = " & Agro_SQL_SaveNum(Mat_Cod))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 Then

                drR.Ditta_Des = "Concime aziendale"
                drR.N = Dr(0).Item("N")
                drR.P2O5 = Dr(0).Item("P2O5")
                drR.K2O = Dr(0).Item("K2O")

            End If

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
                Giacenza_Finale = DrGiac(0).Item("Giacenza")
            End If
        End If

        Return Giacenza_Finale

    End Function

    '#####################################################################
    'per i fertilizzanti aziendali
    Private Function Ricava_Giacenza_Iniziale2(ByVal Dt_Giacenze As DataTable, _
                                                ByVal Dt_Rilievi As DataTable, _
                                                ByVal Mat_cod As Integer, _
                                                ByVal Udm_cod As Integer) As Decimal

        Dim Giacenza_Iniziale As Decimal = 0

        If Not IsNothing(Dt_Giacenze) AndAlso Dt_Giacenze.Rows.Count > 0 Then
            Dim DrGiac() As DataRow
            DrGiac = Dt_Giacenze.Select(" Mat_cod = " + Agro_SQL_SaveNum(Mat_cod) + _
                                        " AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_cod))

            If Not IsNothing(DrGiac) AndAlso DrGiac.Length > 0 Then
                Giacenza_Iniziale = DrGiac(0).Item("Giacenza")
            End If
        End If

        If Not IsNothing(Dt_Rilievi) AndAlso Dt_Rilievi.Rows.Count > 0 Then
            Dim Dr() As DataRow
            Dr = Dt_Rilievi.Select(" Mat_cod = " + Agro_SQL_SaveNum(Mat_cod) + _
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
    'per i fertilizzanti aziendali
    Private Function Ricava_Giacenza_Finale2(ByVal Dt_Finale As DataTable, _
                                                ByVal Mat_cod As Integer, _
                                                ByVal Udm_cod As Integer) As Decimal

        Dim Giacenza_Finale As Decimal = 0

        If Not IsNothing(Dt_Finale) AndAlso Dt_Finale.Rows.Count > 0 Then
            Dim DrGiac() As DataRow
            DrGiac = Dt_Finale.Select(" Mat_cod = " + Agro_SQL_SaveNum(Mat_cod) + _
                                        " AND Udm_Cod = " + Agro_SQL_SaveNum(Udm_cod))

            If Not IsNothing(DrGiac) AndAlso DrGiac.Length > 0 Then
                Giacenza_Finale = DrGiac(0).Item("Giacenza")
            End If
        End If

        Return Giacenza_Finale

    End Function

    '##############################################################
    Private Function Des_from_Cod(ByVal DT As DataTable, _
                                    ByVal NomeCampoCod As String, _
                                    ByVal NomeCampoDes As String, _
                                    ByVal Codice As Integer) As String

        Dim Des As String = ""

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

            Dim Dr() As DataRow = DT.Select(" " & NomeCampoCod & " = " & Agro_SQL_SaveNum(Codice))

            If Not IsNothing(Dr) AndAlso Dr.Length > 0 AndAlso Not IsDBNull(Dr(0).Item(NomeCampoDes)) Then
                Des = Dr(0).Item(NomeCampoDes)
            End If

        End If

        Return Des

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
