Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente

Public Class SchedaGiacenzeMagazzino_2
    Inherits System.Web.UI.Page


    Private rptStampa As Rpt_SchedaGiacenzeMagazzino
    Private rptFooterLogo As FooterLogo
    Private DSGiacenzeMagazzino As DS_GiacenzeMagazzino
    Private DSLogoFooter As New DS_LogoFooter

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_Sa_Cod As String
    Dim Qs_Fabbricato_Cod As String
    Dim Qs_Pro_Cod As String
    Dim Qs_Mat_Cod As String
    Dim Qs_Elem_Cod As String
    Dim Qs_Arrotondamento As Integer
    Dim Qs_Ordinamento As Integer
    Dim Qs_Filtro_ElemCod As String
    Dim Qs_FlagVisualizzaComposizione As Integer
    Dim Qs_Stampalotto As Integer
    Dim Qs_0Raggruppa_1Standard As Integer = 1

    Dim QS_DataStampa, Qs_Lotto As String
    Dim LinkPaginaStampa As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

#Region " GIACENZE MAGAZZINO "

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
        rptStampa = New Rpt_SchedaGiacenzeMagazzino
        rptFooterLogo = New FooterLogo
    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load



        '#################################################################################
        '#####  Recupero i valori dalla QueryString 
        '#################################################################################

        QS_DataStampa = Stringa_Decodifica(Request.QueryString("ds").ToString,
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

        Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString,
                        AgroKey_EncoderDecoder,
                        Server)

        Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString,
                        AgroKey_EncoderDecoder,
                        Server)

        Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString,
                        AgroKey_EncoderDecoder,
                        Server)

        Qs_Arrotondamento = Stringa_Decodifica(Request.QueryString("arr").ToString,
                        AgroKey_EncoderDecoder,
                        Server)

        Qs_Filtro_ElemCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("fec")),
                        AgroKey_EncoderDecoder,
                        Server)

        Qs_FlagVisualizzaComposizione = Stringa_Decodifica(Request.QueryString("fvc").ToString,
                        AgroKey_EncoderDecoder,
                        Server)

        Qs_Lotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lot")),
                   AgroKey_EncoderDecoder,
                   Server))

        Qs_Stampalotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sl")),
                                AgroKey_EncoderDecoder,
                                Server))

        Qs_0Raggruppa_1Standard = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("rag")),
                            AgroKey_EncoderDecoder,
                            Server))

        'Qs_Ordinamento = Stringa_Decodifica(Request.QueryString("ord").ToString, _
        '      AgroKey_EncoderDecoder, _
        '      Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "GiacenzeMagazzino"
        Dim Log_Errori As String = ""
        Dim IdentificazioneDocumento As String
        Dim nomeFilePdf As String

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
                '--------------------------------------------
                Dim DSGiacenzeMagazzino As New DS_GiacenzeMagazzino

                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read

                CType(rptStampa.Section1.ReportObjects("TextAzienda"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextCentro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextMagazzino"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(Qs_Piva, Qs_Sa_Cod, Qs_Fabbricato_Cod, objParametri_Server)
                CType(rptStampa.Section1.ReportObjects("TextData"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_DataStampa


                'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
                'If customLoghi IsNot Nothing Then
                '    rptStampa.Section5.ReportObjects("Text12").ObjectFormat.EnableSuppress = True
                '    rptStampa.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
                '    rptStampa.Section5.ReportObjects("Text8").ObjectFormat.EnableSuppress = True
                'End If

                Carica_DSGiacenzeMagazzino(DSGiacenzeMagazzino, Log_Errori)

                Dim drLogo = DSLogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DSLogoFooter.DT_LogoFooter.Rows.Add(drLogo)

                'sorgente dati.....
                rptStampa.SetDataSource(DSGiacenzeMagazzino)
                rptFooterLogo.SetDataSource(DSLogoFooter)
                rptStampa.OpenSubreport("FooterLogo.rpt").SetDataSource(DSLogoFooter)
            Catch exc As Exception
                Log_Errori += "- Aggancio dataset: " + vbCrLf + exc.Message + vbCrLf
                Log_Errori &= "FullStack:" & vbCrLf & exc.StackTrace & vbCrLf
            End Try

            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                If QS_DataStampa <> AGRODATAFINE Then
                    Data_Inizio_Allegati = AGRODATAINIZIO
                    Data_Fine_Allegati = QS_DataStampa
                    IdentificazioneDocumento += "_" + Format(QS_DataStampa, "yyyy_MM_dd")
                    'Else
                    '    Data_Inizio_Allegati = "01/01/" & CStr(Anno)
                    '    Data_Fine_Allegati = "31/12/" & CStr(Anno)
                End If

                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Magazzino_Giacenze, "", "", objParametri_Server)
                nomeFilePdf = Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf"
                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptStampa,
                                           enum_CategorieDocumenti.Magazzino_Giacenze,
                                           Sottocartella,
                                           NomeFilePdf,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva,
                                                                 enum_CategorieDocumenti.Magazzino_Giacenze,
                                                                 Nome_Documento,
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf",
                                                                 Sottocartella,
                                                                 Qs_Sa_Cod, Qs_Fabbricato_Cod, "", "",
                                                                 Data_Inizio_Allegati,
                                                                 Data_Fine_Allegati,
                                                                 objParametri_Server)



            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'MS Eliminato passaggio in session per passaggio report su file: Session("Report") = rptStampa

            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptStampa.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Str_Errore_Path, Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Data Stampa = " + CStr(QS_DataStampa) + ", " + vbCrLf + _
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf + _
                                "Fabbricato_Cod = " + CStr(Qs_Fabbricato_Cod) + ", " + vbCrLf + _
                                "Elem_Cod = " + CStr(Qs_Elem_Cod) + ", " + vbCrLf + _
                                "Mat_Cod = " + CStr(Qs_Mat_Cod) + ", " + vbCrLf + _
                                "Pro_Cod = " + CStr(Qs_Pro_Cod) + ", " + vbCrLf + _
                                "Arrotondamento = " + CStr(Qs_Arrotondamento) + ", " + vbCrLf + _
                                "Ordinamento = " + CStr(Qs_Ordinamento) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) & ".txt"

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Magazzino", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Magazzino", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "SchedaGiacenzeMagazzino_2.aspx", _
                                                 Log_Errori)

            End If
            '-----------------------------------------

            'MS Dispose del report una volta salvato su disco.
            rptStampa.Close()
            rptStampa.Dispose()
            rptStampa = Nothing

            GC.Collect()

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(nomeFilePdf, AgroKey_EncoderDecoder, Server))

        End If

        '==================================================================


    End Sub


    '###########################################################################
    Private Sub Carica_DSGiacenzeMagazzino(ByRef DSGiacenzeMagazzino As DS_GiacenzeMagazzino,
                                            ByRef Log_Errori As String)

        Dim i, j As Integer
        Dim Udm As String
        Dim Udm_Sim As String
        Dim Calibro As String
        Dim Giacenza As Decimal
        Dim Veg_Cod As Integer
        Dim Ind_Mat_Cod As Integer
        'Dim Cod_Articolo As String = ""
        Dim LottoImpianto As String = ""
        Dim debug As Boolean

        Dim RigaDs As DS_GiacenzeMagazzino.DS_GiacenzeMagazzinoRow
        Dim DT As DataTable
        Dim objGIACENZE As New AgronicaCoreStampeDAL.Magazzino
        Dim DTComposizioneFormulati As DataTable

        Try

            Dim filtro As String = ""

            If Qs_Lotto <> "" Then
                filtro = " AND Movimenti_dettagli.Lotto like '%" & Qs_Lotto & "%'"
            End If

            If Qs_0Raggruppa_1Standard = 0 Then
                DT = objGIACENZE.SchedaGiacenzeMagazzino_Raggruppamento(CDate(QS_DataStampa),
                                                                            CStr(Qs_Piva),
                                                                            CInt(Qs_Sa_Cod),
                                                                            CInt(Qs_Fabbricato_Cod),
                                                                            CInt(Qs_Elem_Cod),
                                                                            CInt(Qs_Pro_Cod),
                                                                            CInt(Qs_Mat_Cod),
                                                                            0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                                            True,
                                                                            filtro, filtro, filtro, filtro,
                                                                            filtro, filtro, filtro, filtro,
                                                                            filtro, filtro, filtro,
                                                                            "",
                                                                            objParametri_Server, objParametri_Utenti)
            Else
                DT = objGIACENZE.SchedaGiacenzeMagazzino(CDate(QS_DataStampa),
                                                            CStr(Qs_Piva),
                                                            CInt(Qs_Sa_Cod),
                                                            CInt(Qs_Fabbricato_Cod),
                                                            CInt(Qs_Elem_Cod),
                                                            CInt(Qs_Pro_Cod),
                                                            CInt(Qs_Mat_Cod),
                                                            0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                            True,
                                                            filtro, filtro, filtro, filtro,
                                                            filtro, filtro, filtro, filtro,
                                                            filtro, filtro, filtro,
                                                            "", "",
                                                            objParametri_Server, objParametri_Utenti)
            End If


            If Qs_FlagVisualizzaComposizione = 1 Then
                'DTComposizioneFormulati = ComposizioneFormulatiRecupera(DT)
                DTComposizioneFormulati = SchedeMagazzinoHelper.ComposizioneFormulatiRecupera(objParametri_Server, DT, Server, Session, Page)
            End If


            If Not DT Is Nothing Then

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                For i = 0 To DT.Rows.Count - 1

                    Giacenza = DT.Rows(i).Item("Giacenza")

                    'oltre al filtro nella query, devo fare anche il filtro su codice
                    'perchè molti decimal vengono salvati come valori infinatamente piccoli
                    'ad esempio 0.00003680000000017003
                    If Giacenza <> 0 And Not (Giacenza < QTA_GiancenzeVisualizzate And Giacenza > -QTA_GiancenzeVisualizzate) Then

                        'inserisco la riga nel dataset..
                        RigaDs = DSGiacenzeMagazzino.DS_GiacenzeMagazzino.NewDS_GiacenzeMagazzinoRow

                        RigaDs.Elem_Cod = DT.Rows(i).Item("Elem_Cod")
                        RigaDs.Cat_Des = DT.Rows(i).Item("NomeComune") 'DT.Rows(i).Item("Cat_Des")

                        RigaDs.Pro_Cod = DT.Rows(i).Item("Pro_Cod")
                        RigaDs.Mat_Cod = DT.Rows(i).Item("Mat_Cod")
                        RigaDs.Pro_Des = ""
                        RigaDs.Prodotto = ""

                        RigaDs.Cod_Progetto = DT.Rows(i).Item("Cod_Progetto")
                        RigaDs.Cal_Cod = DT.Rows(i).Item("Cal_Cod")
                        RigaDs.Lotto = DT.Rows(i).Item("Lotto")

                        RigaDs.Pro_Des = DT.Rows(i).Item("Descrizione_Prodotto")
                        'RigaDs.Prodotto = RigaDs.Cat_Des + ": " + RigaDs.Pro_Des
                        RigaDs.Prodotto = RigaDs.Pro_Des


                        Select Case RigaDs.Elem_Cod

                            Case FORMULATI

                                If Qs_FlagVisualizzaComposizione = False Then
                                    RigaDs.Prodotto += "   (N.Reg. " + CStr(RigaDs.Pro_Cod) + ")"
                                Else
                                    RigaDs.Prodotto += "   (N.Reg. " + CStr(RigaDs.Pro_Cod) + ")" &
                                    SchedeMagazzinoHelper.ComposizioneFormulatiDescrizioneAggiuntiva(objParametri_Server, RigaDs.Pro_Cod, DTComposizioneFormulati)
                                End If


                            Case SEMENTI

                                'If RigaDs.Lotto.ToLower <> "indefinito" And RigaDs.Lotto <> "" Then
                                '    RigaDs.Prodotto += " - Lotto: " + RigaDs.Lotto
                                'End If

                                If InStr(Qs_Filtro_ElemCod, CStr(SEMENTI) + ",") > 0 Then
                                    RigaDs.Prodotto += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                End If

                            Case TRASFORMATI_VEGETALI, SEMILAVORATI_VEGETALI,
                                TRASFORMATI_ANIMALI, SEMILAVORATI_ANIMALI

                                'direttamente nella select list della query
                                'If RigaDs.Cal_Cod <> 0 Then
                                '    If RigaDs.Cal_Cod < 0 Then
                                '        Calibro = DT.Rows(i).Item("Descrizione")
                                '    Else
                                '        Calibro = DT.Rows(i).Item("Cal_Des")
                                '    End If
                                'Else
                                '    Calibro = ""
                                'End If

                                'direttamente nella select list della query
                                'If RigaDs.Cod_Progetto <> 0 Then
                                '    LottoImpianto = CStr(DT.Rows(i).Item("Progetto_Nome"))
                                'Else
                                '    LottoImpianto = ""
                                'End If

                                'RigaDs.Prodotto += " | Calibro: " + Calibro + " | Lotto Impianto: " + LottoImpianto + " | Lotto: " + RigaDs.Lotto

                                'gestione lotto dedicata
                                'RigaDs.Prodotto += " - Lotto: " + RigaDs.Lotto

                                If InStr(Qs_Filtro_ElemCod, CStr(RigaDs.Elem_Cod)) > 0 Then
                                    RigaDs.Prodotto += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                End If

                            Case Else

                                If InStr(Qs_Filtro_ElemCod, CStr(RigaDs.Elem_Cod) + ",") > 0 Then
                                    RigaDs.Prodotto += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                End If

                                'ALTRE_MATERIE,MATERIE_VEGETALI,bENI_CONFEZ_VEGETALE,SEMILAVORATI_ANIMALI,MATERIE_ANIMALI,BENI_CONFEZ_ANIMALE,TRASFORMATI_ANIMALI

                        End Select

                        '----------------------------------
                        'GESTIONE LOTTO PRODOTTI
                        If Qs_Stampalotto = 1 Then
                            'STAMPA IN BASE A CONFIGURAZIONE SU PRODOTTO
                            Dim DettagliLotto As String = ""
                            Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                            DettagliLotto = objLotto.Gestione_LottoProdotto(DT.Rows(i).Item("Piva"),
                                                                             DT.Rows(i).Item("Elem_Cod"),
                                                                             DT.Rows(i).Item("Mat_Cod"),
                                                                             DT.Rows(i).Item("Lotto"),
                                                                             moduliCliente,
                                                                             objParametri_Server)

                            If DettagliLotto <> "" Then
                                RigaDs.Prodotto += " " + DettagliLotto
                            End If

                        Else
                            'STAMPA SEMPRE
                            If CStr(DT.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DT.Rows(i).Item("Lotto") <> "" Then
                                RigaDs.Prodotto += " - Lotto: " & DT.Rows(i).Item("Lotto")
                            End If
                        End If
                        '----------------------------------

                        RigaDs.Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                        RigaDs.Udm_Sim = DT.Rows(i).Item("Udm_Sim")

                        Select Case Qs_Arrotondamento

                            Case 0 'nessun arrotondamento
                                RigaDs.Str_Qta = CStr(Giacenza)

                            Case 1 'arrotondamento all'intero
                                Giacenza = Math.Round(Giacenza, 0, MidpointRounding.AwayFromZero)
                                RigaDs.Str_Qta = Format(Giacenza, "#,###,##0")

                            Case 2 'arrotondamento al primo decimale
                                Giacenza = Math.Round(Giacenza, 1, MidpointRounding.AwayFromZero)
                                RigaDs.Str_Qta = Format(Giacenza, "#,###,##0.0")

                            Case 3 '2 decimali
                                Giacenza = Math.Round(Giacenza, 2, MidpointRounding.AwayFromZero)
                                RigaDs.Str_Qta = Format(Giacenza, "#,###,##0.00")

                            Case 4 '3 decimali
                                Giacenza = Math.Round(Giacenza, 3, MidpointRounding.AwayFromZero)
                                RigaDs.Str_Qta = Format(Giacenza, "#,###,##0.000")

                            Case 5 '4 decimali
                                Giacenza = Math.Round(Giacenza, 4, MidpointRounding.AwayFromZero)
                                RigaDs.Str_Qta = Format(Giacenza, "#,###,##0.0000")

                        End Select

                        RigaDs.Qta = Giacenza

                        If Giacenza = 0 Then
                            'modifica del 5/3/2012, segnalazione di Buzzi
                            'se dopo l'arrotondamento la giacenza è 0, non inserisco la riga nel dataset
                            debug = True
                        Else
                            DSGiacenzeMagazzino.DS_GiacenzeMagazzino.Rows.Add(RigaDs)
                        End If


                    End If

                Next

            End If

            objGIACENZE = Nothing

        Catch ex As Exception
            Log_Errori += "Errore durante il caricamento del dataset: " + ex.Message
        End Try

    End Sub

    ''###########################################################################
    'Private Function ComposizioneFormulatiRecupera(ByRef DT As DataTable) As DataTable

    '    Dim ElencoFormulati As String = ","
    '    Dim i As Integer = 0
    '    Dim FrCod As Integer

    '    'Recupero l'elenco dei formulati 
    '    For i = 0 To DT.Rows.Count - 1
    '        FrCod = DT.Rows(i).Item("Pro_Cod")

    '        'Se l'elemento e' un formulato
    '        If DT.Rows(i).Item("Elem_Cod") = FORMULATI Then

    '            'Se l'elemento non e' gia' presente dentro la stringa ...
    '            If InStr(1, ElencoFormulati, "," & FrCod.ToString & ",", CompareMethod.Text) = 0 Then
    '                ElencoFormulati += FrCod.ToString & ","
    '            End If
    '        End If
    '    Next

    '    'Tolgo il primo e ultimo carattere (,) ... 
    '    If ElencoFormulati.Length > 0 Then
    '        ElencoFormulati = Mid(ElencoFormulati, 2)
    '        ElencoFormulati = Mid(ElencoFormulati, 1, ElencoFormulati.Length - 1)
    '    End If

    '    'Creo la stringa XML di richiesta

    '    Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XmlCredenziali As System.Xml.XmlElement
    '    Dim XmlParametri As System.Xml.XmlElement
    '    Dim XmlNodo As System.Xml.XmlElement
    '    Dim StringaXML As String

    '    Dim WsScheda As String
    '    Dim WsDoorKey As String
    '    Dim WsCodiceGias As String
    '    Dim WsUsernameSuperuser As String
    '    Dim WsPasswordSuperuser As String

    '    '===========================================================================================
    '    '   <CREDENZIALI scheda="..." doorkey= "..." codicegias="..." username="..." password="..." >
    '    '       <PARAMETRI fr_cod=""                        oppure  "239,150" />
    '    '   </CREDENZIALI>
    '    '===========================================================================================

    '    WsScheda = "1080"                                               'AWS_Fitofarmaci_Formulati_PrincipiAttivi
    '    WsDoorKey = "portone_grth45ksh4ghajnl32149"
    '    WsCodiceGias = Session("ASG_ProgressivoGIAS")
    '    WsUsernameSuperuser = Session("ASG_SuperUser_Username")         'Session("ASG_SuperUser_Username_Crypt")
    '    WsPasswordSuperuser = Session("ASG_SuperUser_Password")         'Session("ASG_SuperUser_Password_Crypt")

    '    'Creo il nodo CREDENZIALI
    '    XmlCredenziali = XmlDoc.CreateElement("CREDENZIALI")

    '    'Imposto gli attributi
    '    XmlCredenziali.SetAttribute("scheda", CStr(WsScheda))
    '    XmlCredenziali.SetAttribute("doorkey", CStr(WsDoorKey))
    '    XmlCredenziali.SetAttribute("codicegias", CStr(WsCodiceGias))
    '    XmlCredenziali.SetAttribute("username", CStr(WsUsernameSuperuser))
    '    XmlCredenziali.SetAttribute("password", CStr(WsPasswordSuperuser))

    '    'Imposto XmlParametri come figlio del documento principale
    '    XmlDoc.AppendChild(XmlCredenziali)

    '    'Creo il nodo PARAMETRI
    '    XmlParametri = XmlDoc.CreateElement("PARAMETRI")

    '    'Imposto gli attributi
    '    XmlParametri.SetAttribute("fr_cod", ElencoFormulati)

    '    'Imposto XmlParametri come figlio del documento principale
    '    XmlCredenziali.AppendChild(XmlParametri)

    '    'Restituisco in uscita la stringa creata
    '    StringaXML = XmlDoc.InnerXml

    '    'Distruggo gli oggetti
    '    XmlNodo = Nothing
    '    XmlParametri = Nothing
    '    'XmlDoc = Nothing

    '    '//////////////////////////////////////////////////////////
    '    '/////   Chiamo la funzione del webservice per recuperare la composizione
    '    '//////////////////////////////////////////////////////////

    '    Dim LinkWsFitofarmaci As String
    '    Dim WsRisposta As String = ""
    '    Dim objCoreWebService As New AgronicaCoreWebService.AgroWs
    '    StringaXML = objCoreWebService.AWS_Codifica_P(StringaXML)

    '    Try
    '        Dim WsFito As New WS_Fitofarmaci.AgroWS_Fitofarmaci

    '        If ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString = "" Then
    '            LinkWsFitofarmaci = Server.MapPath("https://ws.netagronica.it/AgronicaWebService/AgroWS_Fitofarmaci.asmx")
    '        Else
    '            LinkWsFitofarmaci = ConfigurationSettings.AppSettings("GiasOnline.WS_Fitofarmaci.AgroWS_Fitofarmaci").ToString
    '        End If

    '        WsFito.Url = LinkWsFitofarmaci
    '        WsFito.Timeout = 60000
    '        WsRisposta = WsFito.Formulati_PrincipiAttivi_2(StringaXML)

    '    Catch ex As Exception
    '        Throw New Exception("Webservice fitofarmaci : " & ex.Message)
    '    End Try


    '    '//////////////////////////////////////////////////////////
    '    '/////   Decodifico il risultato ==> DataTable
    '    '//////////////////////////////////////////////////////////

    '    Dim DTfor As New DataTable
    '    DTfor.Columns.Add(New DataColumn("Fr_Cod", GetType(Integer)))
    '    DTfor.Columns.Add(New DataColumn("Fr_Des", GetType(String)))
    '    DTfor.Columns.Add(New DataColumn("Elenco_ClassiTossicologiche", GetType(String)))
    '    DTfor.Columns.Add(New DataColumn("Elenco_PrincipiAttivi", GetType(String)))


    '    'Dim XmlDoc As New System.Xml.XmlDocument
    '    Dim XMLs_Formulati As System.Xml.XmlNodeList
    '    Dim XMLs_PrincipiAttivi As System.Xml.XmlNodeList
    '    Dim XMLs_ClassiTossicologiche As System.Xml.XmlNodeList

    '    Dim XmlRisultati As System.Xml.XmlElement
    '    Dim XmlFormulato As System.Xml.XmlElement
    '    Dim XmlPrincipioAttivo As System.Xml.XmlElement
    '    Dim XmlClasseTossicologica As System.Xml.XmlElement

    '    Dim DR As DataRow
    '    Dim ict As Integer = 0
    '    Dim ipa As Integer = 0
    '    Dim Testo As String = ""

    '    XmlDoc.LoadXml(WsRisposta)

    '    'Recupero l'elenco dei Formulati
    '    XMLs_Formulati = XmlDoc.GetElementsByTagName("FORMULATO")

    '    For i = 0 To XMLs_Formulati.Count - 1

    '        'Formulato i-esimo
    '        XmlFormulato = XMLs_Formulati.Item(i)

    '        'Creo la riga per il datatable
    '        DR = DTfor.NewRow

    '        DR.Item("Fr_Cod") = CInt(XmlFormulato.GetAttribute("fr_cod"))
    '        DR.Item("Fr_Des") = CStr(XmlFormulato.GetAttribute("fr_des"))

    '        '---------------------------------------------------
    '        'CLASSE TOSSICOLOGICA ==>  cod1§des1|cod2§des2
    '        'Recupero l'elenco delle Classi Tossicologiche

    '        XMLs_ClassiTossicologiche = XmlFormulato.GetElementsByTagName("CLASSE_TOSSICOLOGICA")
    '        Testo = ""
    '        For ict = 0 To XMLs_ClassiTossicologiche.Count - 1
    '            XmlClasseTossicologica = XMLs_ClassiTossicologiche.Item(ict)
    '            Testo &= CStr(XmlClasseTossicologica.GetAttribute("cltoss_cod")) & "§" & _
    '                     CStr(XmlClasseTossicologica.GetAttribute("cltoss_des")) & "|"
    '        Next
    '        Testo = Left(Testo, Testo.Length - 1)
    '        DR.Item("Elenco_ClassiTossicologiche") = Testo

    '        '---------------------------------------------------
    '        'PRINCIPI ATTIVI ==> cod1§des1§titolo1|cod2§des2§titolo2
    '        'Recupero l'elenco dei Principi Attivi

    '        XMLs_PrincipiAttivi = XmlFormulato.GetElementsByTagName("PRINCIPIO_ATTIVO")

    '        Testo = ""
    '        For ipa = 0 To XMLs_PrincipiAttivi.Count - 1
    '            XmlPrincipioAttivo = XMLs_PrincipiAttivi.Item(ipa)
    '            Testo &= CStr(XmlPrincipioAttivo.GetAttribute("pa_cod")) & "§" & _
    '                     CStr(XmlPrincipioAttivo.GetAttribute("pa_des")) & "§" & _
    '                     CStr(XmlPrincipioAttivo.GetAttribute("titolo")) & "|"
    '        Next
    '        Testo = Left(Testo, Testo.Length - 1)
    '        DR.Item("Elenco_PrincipiAttivi") = Testo

    '        '---------------------------------------------------
    '        DTfor.Rows.Add(DR)

    '    Next

    '    'Restituisco il risultato
    '    Return DTfor

    'End Function


    ''###########################################################################
    'Private Function ComposizioneFormulatiDescrizioneAggiuntiva(ByVal Fr_Cod As Integer, ByVal DtFor As DataTable) As String

    '    Dim DR As DataRow()
    '    Dim ElencoClassiTossicologiche As String = ""
    '    Dim ClassiTossicologiche As String()

    '    Dim ElencoPrincipiAttivi As String = ""
    '    Dim PrincipiAttivi As String()


    '    Dim ClToss_Cod As String = ""
    '    Dim ClToss_Des As String = ""
    '    Dim Pa_Cod As String = ""
    '    Dim Pa_Des As String = ""
    '    Dim Titolo As String = ""

    '    Dim i As Integer
    '    Dim Risultato As String = ""

    '    'Inizializzo
    '    Risultato = ""

    '    'Cerco il formulato corrente ...
    '    DR = DtFor.Select("Fr_Cod = " & Fr_Cod)

    '    'CLASSI TOSSICOLOGICHE
    '    ElencoClassiTossicologiche = DR(0).Item("Elenco_ClassiTossicologiche")
    '    If ElencoClassiTossicologiche <> "" Then

    '        Risultato = "   (Classe Toss. =  "
    '        ClassiTossicologiche = ElencoClassiTossicologiche.Split("|")
    '        For i = 0 To ClassiTossicologiche.Length - 1
    '            Risultato &= ClassiTossicologiche(i).Split("§")(0) & "  "
    '        Next
    '        Risultato &= ")"

    '    End If

    '    'PRINCIPI ATTIVI
    '    ElencoPrincipiAttivi = DR(0).Item("Elenco_PrincipiAttivi")
    '    If ElencoPrincipiAttivi <> "" Then
    '        Risultato &= vbCrLf
    '        PrincipiAttivi = ElencoPrincipiAttivi.Split("|")
    '        For i = 0 To PrincipiAttivi.Length - 1

    '            Pa_Cod = PrincipiAttivi(i).Split("§")(0)
    '            Pa_Des = PrincipiAttivi(i).Split("§")(1)
    '            Titolo = PrincipiAttivi(i).Split("§")(2)

    '            'Nota Bene : se il titolo e' "0" allora visualizzo solo la descrizione
    '            If cdec(Titolo) <> 0 Then
    '                Risultato &= "     " & Format(cdec(Titolo), "0.####") & " % - " & Pa_Des & vbCrLf
    '            Else
    '                Risultato &= "     " & "( " & Pa_Des & " )" & vbCrLf
    '            End If
    '        Next
    '    End If

    '    Return Risultato

    'End Function








End Class
