Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class RiepilogoProdotti
    Inherits System.Web.UI.Page

    Private rptStampa As Rpt_RiepilogoProdottiUtilizzati
    Private Log_Errori As String = ""

    '----- Gestione Querystring
    Dim Qs_Piva, Qs_CauMov As String
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
    Dim Qs_DataInizio, Qs_DataFine, Qs_Lotto As String
    Dim LinkPaginaStampa As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " RIEPILOGO PRODOTTI UTILIZZATI "

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
        rptStampa = New Rpt_RiepilogoProdottiUtilizzati

    End Sub

#End Region

    '###########################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        '#################################################################################
        '#####  Recupero i valori dalla QueryString 
        '#################################################################################

        Qs_DataInizio = Stringa_Decodifica(Request.QueryString("di").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_DataFine = Stringa_Decodifica(Request.QueryString("df").ToString, _
                   AgroKey_EncoderDecoder, _
                   Server)

        Qs_CauMov = Stringa_Decodifica(Request.QueryString("cm").ToString, _
              AgroKey_EncoderDecoder, _
              Server)

        Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Fabbricato_Cod = Stringa_Decodifica(Request.QueryString("f").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Elem_Cod = Stringa_Decodifica(Request.QueryString("e").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Pro_Cod = Stringa_Decodifica(Request.QueryString("pro").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mat").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Arrotondamento = Stringa_Decodifica(Request.QueryString("arr").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Filtro_ElemCod = AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("fec")), _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_FlagVisualizzaComposizione = Stringa_Decodifica(Request.QueryString("fvc").ToString, _
                        AgroKey_EncoderDecoder, _
                        Server)

        Qs_Lotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("lot")), _
                   AgroKey_EncoderDecoder, _
                   Server))

        Qs_Stampalotto = Trim(AgronicaCoreDataProvider.Sicurezza.Stringa_Decodifica(CStr(Request.QueryString("sl")), _
                                AgroKey_EncoderDecoder, _
                                Server))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim DSProdotti As New DS_RiepilogoProdotti

        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "RiepilogoProdottiUtilizzati"
        Dim IdentificazioneDocumento As String = Nome_Documento

        Dim Param_Periodo As String = ""
        Dim Param_RagSoc As String = ""
        Dim Param_Sa_Nome As String = ""
        Dim Param_Magazzino As String = ""
        Dim Param_Cau_Mov_Desc As String = ""

        If Not Me.IsPostBack Then

            Try

                'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

                'CrystalReportViewer1.Style.Add("LEFT", "-275px")
                'CrystalReportViewer1.Style.Add("TOP", "0px")
                'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

                Dim objImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read
                Param_RagSoc = objImprese.RagSoc_from_Piva(Qs_Piva, objParametri_Server)

                If Qs_Sa_Cod <> 0 Then
                    Dim objCentriAz As New AgronicaCoreAnagrafeDAL.CentriAziendali_Read
                    Param_Sa_Nome = objCentriAz.SaNome_from_SaCod(Qs_Piva, Qs_Sa_Cod, objParametri_Server)
                Else
                    Param_Sa_Nome = "Tutti i centri aziendali"
                End If
                If Qs_Fabbricato_Cod <> 0 Then
                    Param_Magazzino = AgronicaCoreAnagrafeDAL.Fabbricati_R.FabbricatoDes_from_FabbricatoCod(Qs_Piva, Qs_Sa_Cod, Qs_Fabbricato_Cod, objParametri_Server)
                Else
                    Param_Magazzino = "Tutti i magazzini"
                End If

                Param_Periodo = Qs_DataInizio & " - " & Qs_DataFine
                Select Case Qs_CauMov
                    Case CAU_CARICO
                        Param_Cau_Mov_Desc = "CARICHI"
                    Case CAU_SCARICO
                        Param_Cau_Mov_Desc = "SCARICHI"
                End Select


                Log_Errori = ""

                Stampa_RiepilogoProdottiUtilizzati(DSProdotti, Log_Errori)

            Catch exc As Exception
                Log_Errori += "- Stampa_RiepilogoProdottiUtilizzati: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try

                rptStampa.SetDataSource(DSProdotti)

            Catch exc As Exception
                Log_Errori += "- Aggancio dataset: " + vbCrLf + exc.Message + vbCrLf
            End Try


            '#########################################################

            Try

                rptStampa.SetParameterValue("Periodo", Param_Periodo)
                rptStampa.SetParameterValue("Rag_Soc", Param_RagSoc)
                rptStampa.SetParameterValue("Sa_Nome", Param_Sa_Nome)
                rptStampa.SetParameterValue("Magazzino", Param_Magazzino)
                rptStampa.SetParameterValue("Cau_Mov_Desc", Param_Cau_Mov_Desc)

            Catch ex As Exception
                Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
            End Try


            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

                Data_Inizio_Allegati = Qs_DataInizio
                Data_Fine_Allegati = Qs_DataFine
                IdentificazioneDocumento += "_" + Format(Qs_DataInizio, "yyyy-MM-dd") & "_" + Format(Qs_DataFine, "yyyy-MM-dd")
                'Else
                '    Data_Inizio_Allegati = "01/01/" & CStr(Anno)
                '    Data_Fine_Allegati = "31/12/" & CStr(Anno)


                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Magazzino_Giacenze, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptStampa, _
                                           enum_CategorieDocumenti.Magazzino_Giacenze, _
                                           Sottocartella, _
                                           Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva, _
                                                                 enum_CategorieDocumenti.Magazzino_Giacenze, _
                                                                 Nome_Documento, _
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                                                 Sottocartella, _
                                                                 Qs_Sa_Cod, Qs_Fabbricato_Cod, "", "", _
                                                                 Data_Inizio_Allegati, _
                                                                 Data_Fine_Allegati, _
                                                                 objParametri_Server)

            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

            'MS Eliminato passaggio in session per passaggio report su file: Session("Report") = rptStampa
            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptStampa.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            ''MS Dispose del report una volta salvato su disco.
            'rptStampa.Close()
            'rptStampa.Dispose()
            'rptStampa = Nothing

            GC.Collect()


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Str_Errore_Path, Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Data Inizio = " + CStr(Qs_DataInizio) + ", " + vbCrLf + _
                                "Data Fine = " + CStr(Qs_DataFine) + ", " + vbCrLf + _
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf + _
                                "Fabbricato_Cod = " + CStr(Qs_Fabbricato_Cod) + ", " + vbCrLf + _
                                "Elem_Cod = " + CStr(Qs_Elem_Cod) + ", " + vbCrLf + _
                                "Mat_Cod = " + CStr(Qs_Mat_Cod) + ", " + vbCrLf + _
                                "Pro_Cod = " + CStr(Qs_Pro_Cod) + ", " + vbCrLf + _
                                "Arrotondamento = " + CStr(Qs_Arrotondamento) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Magazzino", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "RiepilogoProdotti.aspx", _
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))


        End If

        '==================================================================


    End Sub


    '###########################################################################
    Private Sub Stampa_RiepilogoProdottiUtilizzati(ByRef DSProdotti As DS_RiepilogoProdotti, _
                                            ByRef Log_Errori As String)

        Dim i, j As Integer
        ' Dim Udm As String
        ' Dim Udm_Sim As String
        ' Dim Calibro As String
        Dim QtaTot As Decimal
        ' Dim Veg_Cod As Integer
        'Dim Ind_Mat_Cod As Integer
        'Dim Cod_Articolo As String = ""
        Dim LottoImpianto As String = ""
        ' Dim debug As Boolean

        Dim RigaDs As DS_RiepilogoProdotti.DT_RiepilogoProdottiRow
        Dim DT As DataTable
        Dim objGIACENZE As New AgronicaCoreStampeDAL.Magazzino
        Dim DTComposizioneFormulati As DataTable

        Try

            Dim filtro As String = ""

            If Qs_Lotto <> "" Then
                filtro = " AND Movimenti_dettagli.Lotto like '%" & Qs_Lotto & "%'"
            End If

            DT = objGIACENZE.RiepilogoProdottiUtilizzati(CDate(Qs_DataInizio),
                                                         CDate(Qs_DataFine),
                                                         CStr(Qs_Piva),
                                                         CInt(Qs_Sa_Cod),
                                                         CInt(Qs_Fabbricato_Cod),
                                                         CInt(Qs_Elem_Cod),
                                                         CInt(Qs_Pro_Cod),
                                                         CInt(Qs_Mat_Cod),
                                                         0, 0, 0, 0, LOTTO_NONDEFINITO,
                                                         Qs_CauMov,
                                                         filtro, "", "", "",
                                                         "", "", "", "",
                                                         "", "", "",
                                                         "", "", "",
                                                         "", "",
                                                         objParametri_Server, objParametri_Utenti)


            If Qs_FlagVisualizzaComposizione = 1 Then
                DTComposizioneFormulati = SchedeMagazzinoHelper.ComposizioneFormulatiRecupera(objParametri_Server, DT, Server, Session, Page)
            End If

            If Not DT Is Nothing Then

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                Dim DT_Fito As DataTable
                Dim DT_Fert As DataTable
                Dim DT_Trap As DataTable

                Try
                    '------------ FITOFARMACI ------------------
                    DT_Fito = DT.Clone

                    Dim Dr_Fito() As DataRow
                    Dr_Fito = DT.Select(" Elem_Cod = " & CStr(FORMULATI))

                    If Not IsNothing(Dr_Fito) AndAlso Dr_Fito.Length > 0 Then
                        'se ci sono formulati
                        For Each Dr In Dr_Fito
                            DT_Fito.ImportRow(Dr)
                        Next

                        Dim oRecuperaDitta As New AgronicaCoreWebService.Formulati_WS
                        oRecuperaDitta.RecuperaDittaInDescrizioneFormulati(DT_Fito, objParametri_Utenti, "pro_cod", "Descrizione", False, "Ditta")

                    End If
                Catch ex As Exception
                    Log_Errori &= "FITOFARMACI, recupero ditte errore: " & ex.Message & vbCrLf
                End Try
                '-----------------------------------------------

                Try
                    '------------ TRAPPOLE ------------------
                    Dim Dr_Tra() As DataRow
                    Dr_Tra = DT.Select(" Elem_Cod = " & CStr(TRAPPOLE))

                    Dim StrElencoTrapCod As String = ""
                    If Not IsNothing(Dr_Tra) AndAlso Dr_Tra.Length > 0 Then
                        'se ci sono trappole
                        For j = 0 To Dr_Tra.Length - 1
                            If j <> Dr_Tra.Length - 1 Then
                                StrElencoTrapCod &= CStr(Dr_Tra(j).Item("pro_cod")) & ","
                            Else
                                StrElencoTrapCod &= CStr(Dr_Tra(j).Item("pro_cod"))
                            End If
                        Next
                    End If
                    If StrElencoTrapCod <> "" Then
                        Dim objTrappole As New AgronicaCoreMetaSchemaDAL.Trappole_R
                        DT_Trap = objTrappole.DitteConcatenatexTrapCod(0, StrElencoTrapCod, objParametri_Server)
                    End If
                Catch ex As Exception
                    Log_Errori &= "TRAPPOLE, recupero ditte errore: " & ex.Message & vbCrLf
                End Try
                '-----------------------------------------------

                Try
                    '------------ FERTILIZZANTI ------------------
                    Dim Dr_Fert() As DataRow
                    Dr_Fert = DT.Select(" Elem_Cod = " & CStr(FERTILIZZANTI))

                    Dim StrElencoFerCod As String = ""
                    If Not IsNothing(Dr_Fert) AndAlso Dr_Fert.Length > 0 Then
                        'se ci sono fertilizzanti
                        For j = 0 To Dr_Fert.Length - 1
                            If j <> Dr_Fert.Length - 1 Then
                                StrElencoFerCod &= CStr(Dr_Fert(j).Item("pro_cod")) & ","
                            Else
                                StrElencoFerCod &= CStr(Dr_Fert(j).Item("pro_cod"))
                            End If
                        Next
                    End If
                    If StrElencoFerCod <> "" Then
                        DT_Fert = CaricaDt_FertilizzantixDitte(StrElencoFerCod)
                    End If
                Catch ex As Exception
                    Log_Errori &= "FERTILIZZANTI, recupero ditte errore: " & ex.Message & vbCrLf
                End Try
                '-----------------------------------------------

                For i = 0 To DT.Rows.Count - 1

                    QtaTot = DT.Rows(i).Item("QtaTot")

                    ''oltre al filtro nella query, devo fare anche il filtro su codice
                    ''perchè molti decimal vengono salvati come valori infinatamente piccoli
                    ''ad esempio 0.00003680000000017003
                    'If QtaTot <> 0 And Not (QtaTot < 0.000090000000000000006 And QtaTot > -0.000090000000000000006) Then

                    'inserisco la riga nel dataset..
                    RigaDs = DSProdotti.DT_RiepilogoProdotti.NewDT_RiepilogoProdottiRow

                    RigaDs.Elem_Cod = DT.Rows(i).Item("Elem_Cod")
                    RigaDs.Elem_Des = DT.Rows(i).Item("NomeComune")

                    RigaDs.Pro_Cod = DT.Rows(i).Item("Pro_Cod")
                    RigaDs.Mat_Cod = DT.Rows(i).Item("Mat_Cod")

                    If DT.Rows(i).Item("Pro_Cod") <> 0 Then
                        RigaDs.Codice = DT.Rows(i).Item("Pro_Cod")
                    Else
                        RigaDs.Codice = DT.Rows(i).Item("Mat_Cod")
                    End If

                    RigaDs.Descrizione = DT.Rows(i).Item("Descrizione_Prodotto")

                    RigaDs.Ditta = Ricava_Ditta(DT.Rows(i).Item("Elem_Cod"), DT.Rows(i).Item("Pro_Cod"), DT, DT_Fito, DT_Trap, DT_Fert)

                    Select Case RigaDs.Elem_Cod

                        Case FORMULATI

                            If Qs_FlagVisualizzaComposizione = 1 Then
                                RigaDs.Descrizione &= SchedeMagazzinoHelper.ComposizioneFormulatiDescrizioneAggiuntiva(objParametri_Server, RigaDs.Pro_Cod, DTComposizioneFormulati)
                            End If

                        Case Else

                            If InStr(Qs_Filtro_ElemCod, CStr(RigaDs.Elem_Cod) + ",") > 0 Then
                                If CStr(DT.Rows(i).Item("Cod_articolo")) <> "" Then
                                    RigaDs.Descrizione += " (Cod. " + CStr(DT.Rows(i).Item("Cod_articolo")) + ")"
                                End If
                            End If

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
                            RigaDs.Descrizione += " " + DettagliLotto
                        End If

                    Else
                        'STAMPA SEMPRE
                        If CStr(DT.Rows(i).Item("Lotto")).ToLower <> "indefinito" And DT.Rows(i).Item("Lotto") <> "" Then
                            RigaDs.Descrizione += " - Lotto: " & DT.Rows(i).Item("Lotto")
                        End If
                    End If
                    '----------------------------------

                    RigaDs.Udm_Cod = DT.Rows(i).Item("Udm_Cod")
                    RigaDs.Udm_Sim = DT.Rows(i).Item("Udm_Sim")

                    Select Case Qs_Arrotondamento

                        Case 0 'nessun arrotondamento
                            RigaDs.QtaTot = CStr(QtaTot)

                        Case 1 'arrotondamento all'intero
                            QtaTot = Math.Round(QtaTot, 0, MidpointRounding.AwayFromZero)
                            RigaDs.QtaTot = Format(QtaTot, "#,###,##0")

                        Case 2 'arrotondamento al primo decimale
                            QtaTot = Math.Round(QtaTot, 1, MidpointRounding.AwayFromZero)
                            RigaDs.QtaTot = Format(QtaTot, "#,###,##0.0")

                        Case 3 '2 decimali
                            QtaTot = Math.Round(QtaTot, 2, MidpointRounding.AwayFromZero)
                            RigaDs.QtaTot = Format(QtaTot, "#,###,##0.00")

                        Case 4 '3 decimali
                            QtaTot = Math.Round(QtaTot, 3, MidpointRounding.AwayFromZero)
                            RigaDs.QtaTot = Format(QtaTot, "#,###,##0.000")

                        Case 5 '4 decimali
                            QtaTot = Math.Round(QtaTot, 4, MidpointRounding.AwayFromZero)
                            RigaDs.QtaTot = Format(QtaTot, "#,###,##0.0000")

                    End Select

                    'If QtaTot = 0 Then
                    '    'modifica del 5/3/2012, segnalazione di Buzzi
                    '    'se dopo l'arrotondamento la QtaTot è 0, non inserisco la riga nel dataset
                    '    debug = True
                    'Else
                    '    DSProdotti.DS_GiacenzeMagazzino.Rows.Add(RigaDs)
                    'End If
                    DSProdotti.DT_RiepilogoProdotti.Rows.Add(RigaDs)

                    'End If

                Next

            End If

        objGIACENZE = Nothing

        Catch ex As Exception
            Log_Errori += "Errore durante il caricamento del dataset: " + ex.Message
        End Try

    End Sub

    Private Function Ricava_Ditta(Elem_Cod, pro_cod, DT, DT_Fito, DT_Trap, DT_Fert) As String

        Dim Ditta As String = ""

        Select Case Elem_Cod

            Case FORMULATI
                Dim objHLP As New AgronicaCoreContabHLP.Contabilita
                Ditta = objHLP.Des_from_Cod(DT_Fito, "pro_cod", "Ditta", pro_cod)

            Case FERTILIZZANTI
                Dim objHLP As New AgronicaCoreContabHLP.Contabilita
                Ditta = objHLP.Des_from_Cod(DT_Fert, "fer_cod", "Ditta", pro_cod)

            Case TRAPPOLE
                Dim objHLP As New AgronicaCoreContabHLP.Contabilita
                Ditta = objHLP.Des_from_Cod(DT_Trap, "trap_cod", "Ditta", pro_cod)

        End Select

        Return Ditta

    End Function

    Private Function CaricaDt_FertilizzantixDitte(ByVal StrElencoFerCod As String) As DataTable

        Dim objMS As New AgronicaCoreWebService.Fertilizzanti_WS
        Dim FiltroAgg As String = ""

        If StrElencoFerCod <> "" Then
            FiltroAgg = " Fertilizzanti.Fer_Cod IN ( " & StrElencoFerCod & ")"
        End If

        Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim DT_Fert As DataTable

        If Not IsNothing(objAgroWebConfig) AndAlso objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti <> "" Then

            Dim URL_WS_Fert As String = objAgroWebConfig.GiasOnline_WS_Fertilizzanti_IAgroAPI_Fertilizzanti & "/Fertilizzanti"

            DT_Fert = objMS.RecuperaDT_FertilizzantiConDitteConcatenate_daWS(URL_WS_Fert, _
                                                                             0, _
                                                                            0,
                                                                            0,
                                                                            False,
                                                                            False,
                                                                            AGRODATAINIZIO,
                                                                            AGRODATAFINE,
                                                                             FiltroAgg)

        Else
            Throw New Exception("URL WS Fertilizzanti non valorizzato.")
        End If

        Return DT_Fert

    End Function





End Class
