Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class BilancioConfronto
    Inherits System.Web.UI.Page

    Private rptBilancioConfronto As Rpt_BilancioConfronto

    Dim Piva, Rag_Soc As String
    Dim Log As String = ""
    Dim Data_Inizio As String
    Dim Data_Fine As String
    Dim Anno As Integer
    Dim AnnoConfronto As Integer
    Dim Filtro_Conti As String
    Dim Report As Integer
    Dim Log_Errori As String = ""


#Region " REPORT BILANCIO CONFRONTO"

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
        rptBilancioConfronto = New Rpt_BilancioConfronto

    End Sub

#End Region

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Rag_Soc = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Report = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))


        Anno = CInt(Stringa_Decodifica(CStr(Request.QueryString("a")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        AnnoConfronto = CInt(Stringa_Decodifica(CStr(Request.QueryString("ac")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("di")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("df")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Filtro_Conti = Stringa_Decodifica(CStr(Request.QueryString("fc")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento, IdentificazioneDocumento As String

        Nome_Documento = "Bilancio_Confronto"
        IdentificazioneDocumento = CStr(Anno)

        'If Filtro_Conti <> "" Then
        '    IdentificazioneDocumento += "_filtro"
        'End If
        'If Chk_CE = 1 And Chk_SP = 0 Then
        '    IdentificazioneDocumento += "_CE"
        'End If
        'If Chk_CE = 0 And Chk_SP = 1 Then
        '    IdentificazioneDocumento += "_SP"
        'End If

        'Dim ObjReport As Object
        ' Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        ' If Not Me.IsPostBack Then

        'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

        'CrystalReportViewer1.Style.Add("LEFT", "-275px")
        'CrystalReportViewer1.Style.Add("TOP", "0px")
        'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
        'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

        Dim DSBilancioConfronto As New DS_BilancioConfronto

        Try

            StampaBilancio(DSBilancioConfronto)

        Catch exc As Exception
            Log_Errori += "- StampaBilancio: " + vbCrLf + exc.Message + vbCrLf
        End Try

        Try

            Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date

            If Data_Inizio <> AGRODATAINIZIO And Data_Fine <> AGRODATAFINE Then
                Data_Inizio_Allegati = Data_Inizio
                Data_Fine_Allegati = Data_Fine
                IdentificazioneDocumento += "_" + Format(Data_Inizio, "yyyy_MM_dd") + "_" + Format(Data_Fine, "yyyy_MM_dd")
            Else
                Data_Inizio_Allegati = "01/01/" & CStr(Anno)
                Data_Fine_Allegati = "31/12/" & CStr(Anno)
            End If

            ' leggo la sottocartella da CategorieDocumenti
            Dim Sottocartella As String
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Bilancio, "", "", objParametri_Server)

            ' salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            objGestFile.SalvaReportPdf(rptBilancioConfronto, _
                                       enum_CategorieDocumenti.Bilancio, _
                                       Sottocartella, _
                                       Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                       objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
            Dim AllegatiDocumentiCod As Integer

            AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                                                             enum_CategorieDocumenti.Bilancio, _
                                                             Nome_Documento, _
                                                             Nome_Documento + "_p" & Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                                             Sottocartella, _
                                                             "", "", "", "", _
                                                             Data_Inizio_Allegati, _
                                                             Data_Fine_Allegati, _
                                                             objParametri_Server)



        Catch ex As Exception
            Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim Nome_File As String

        If Log_Errori <> "" Then

            Log_Errori = Nome_Documento +
                        ", anno = " + CStr(Anno) +
                        ", Data_Inizio = " + CStr(Data_Inizio) +
                        ", Data_Fine = " + CStr(Data_Fine) +
                        ", Filtro_Conti = " + CStr(Filtro_Conti) +
                        vbCrLf + vbCrLf + Log_Errori

            Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                             "Stampe_Contabilita",
                                             Nome_File & ".txt",
                                             Session("ASG_Utente_Username"),
                                             "BilancioConfronto.aspx",
                                             Log_Errori)

        End If

        '-----------------------------------------

        Session("Report") = rptBilancioConfronto
        Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

    End Sub

    '#####################################################################################################
    Private Sub StampaBilancio(ByRef DSBilancioConfronto As DS_BilancioConfronto)

        Dim DT_Intestazione As DataTable
        Dim i, j As Integer
        Dim objIntest As New AgronicaCoreStampeDAL.DocContab
        DT_Intestazione = objIntest.DatiIntestazioneImpresa(Piva, Log, "", "", objParametri_Server)

        If Log = "" Then

            If DT_Intestazione.Rows.Count <> 0 Then

                CType(rptBilancioConfronto.Section1.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Piva")
                'CType(rptBilancioConfronto.Section1.ReportObjects("TxtCodFisc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("Codice_Fiscale")
                CType(rptBilancioConfronto.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("rag_soc")
                'CType(rptBilancioConfronto.Section1.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("ind_impresa")
                'CType(rptBilancioConfronto.Section1.ReportObjects("TxtComProv"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = DT_Intestazione.Rows(0).Item("CAP") + " " + DT_Intestazione.Rows(0).Item("frz_des") + " - " + DT_Intestazione.Rows(0).Item("LOCALITA") + " (" + DT_Intestazione.Rows(0).Item("COMUNI_PROV") + ") "

                'CType(rptBilancioConfronto.Section1.ReportObjects("TxtDataInizio"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Inizio
                'CType(rptBilancioConfronto.Section1.ReportObjects("TxtDataFine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Fine

            End If

        Else
            Throw New Exception(Log)
        End If


        '########################################################################
        '########################################################################

        Dim objPianoConti As New FunzionixPianoConti
        Dim objPC As New AgronicaCoreStampeDAL.PianoConti
        Dim risp As Boolean

        Select Case Report

            '-------------------------------------------------------------
            '--------------- CONFRONTO BILANCI DI VERIFICA ---------------
            '-------------------------------------------------------------

            Case enum_CodificaStampe.Bilanci_DiVerifica_Confronto

                CType(rptBilancioConfronto.Section1.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Piva
                CType(rptBilancioConfronto.Section1.ReportObjects("TxtAzAgr"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Rag_Soc

                CType(rptBilancioConfronto.Section1.ReportObjects("TxtTitolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text += "  " + CStr(Anno) + " - " + CStr(AnnoConfronto)
                CType(rptBilancioConfronto.Section1.ReportObjects("TxtAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = CStr(Anno)

                CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text, CStr(CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text).Length - 4) + CStr(Anno)
                'CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Saldo Conto Anno " & CStr(Anno)

                CType(rptBilancioConfronto.Section2.ReportObjects("TxtPesoAnalitico"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(CType(rptBilancioConfronto.Section2.ReportObjects("TxtPesoAnalitico"), CrystalDecisions.CrystalReports.Engine.TextObject).Text, CStr(CType(rptBilancioConfronto.Section2.ReportObjects("TxtPesoAnalitico"), CrystalDecisions.CrystalReports.Engine.TextObject).Text).Length - 4) + CStr(Anno)

                CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text, CStr(CType(rptBilancioConfronto.Section2.ReportObjects("TxtAnalitico2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text).Length - 4) + CStr(AnnoConfronto)

                CType(rptBilancioConfronto.Section2.ReportObjects("TxtSaldoTotale1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(CType(rptBilancioConfronto.Section2.ReportObjects("TxtSaldoTotale1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text, CStr(CType(rptBilancioConfronto.Section2.ReportObjects("TxtSaldoTotale1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text).Length - 4) + CStr(Anno)

                'CType(rptBilancioConfronto.Section2.ReportObjects("TxtPesoSaldoTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(CType(rptBilancioConfronto.Section2.ReportObjects("TxtPesoSaldoTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text, CStr(CType(rptBilancioConfronto.Section2.ReportObjects("TxtPesoSaldoTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text).Length - 4) + CStr(Anno)
                CType(rptBilancioConfronto.Section2.ReportObjects("TxtPeso"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Anno " & CStr(Anno)

                CType(rptBilancioConfronto.Section2.ReportObjects("TxtSaldoTotale2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Left(CType(rptBilancioConfronto.Section2.ReportObjects("TxtSaldoTotale2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text, CStr(CType(rptBilancioConfronto.Section2.ReportObjects("TxtSaldoTotale2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text).Length - 4) + CStr(AnnoConfronto)


                Dim Filtro_Conti_1 As String = ""
                Dim Filtro_Conti_2 As String = ""
                Dim Filtro_Conti_3 As String = ""

                Dim SaldoTotale_1 As Double
                Dim SaldoTotale_2 As Double

                Dim Saldo_1 As Double
                Dim Saldo_2 As Double

                Dim SaldoTotale_A_1 As Double = 0
                Dim SaldoTotale_A_2 As Double = 0
                Dim SaldoTotale_B_1 As Double = 0
                Dim SaldoTotale_B_2 As Double = 0

                Dim TotaleGruppo As Double

                Dim Differenza_Perc_Analitico, Differenza_Perc_SaldoTotale As Double
                Dim PesoAnalitico_Perc_TotaleGruppo, PesoTotale_Perc_TotaleGruppo As Double

                'Dim Str_Id_Ricl_1_i, Str_Id_Ricl_2_i As String
                'Dim Str_Id_Ricl_1_j, Str_Id_Ricl_2_j As String
                'Dim Len_Id_Ricl_1, Len_Id_Ricl_2 As Integer
                Dim Str_Id_Ricl_i, Str_Id_Ricl_j As String
                Dim Len_Id_Ricl_i, Len_Id_Ricl_j As Integer

                risp = objPC.PianoContiEconomici_SaldoConto_A_B_Confronto(SaldoTotale_A_1, _
                                                               SaldoTotale_B_1, _
                                                               SaldoTotale_A_2, _
                                                               SaldoTotale_B_2, _
                                                               Piva, _
                                                               Anno, _
                                                               AnnoConfronto, _
                                                               objParametri_Server)

                SaldoTotale_B_1 = Math.Abs(SaldoTotale_B_1)
                SaldoTotale_B_2 = Math.Abs(SaldoTotale_B_2)

                CType(rptBilancioConfronto.Section1.ReportObjects("TxtSaldoA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text += Format(SaldoTotale_A_1, "##,###,##0.00")
                CType(rptBilancioConfronto.Section1.ReportObjects("TxtSaldoB"), CrystalDecisions.CrystalReports.Engine.TextObject).Text += Format(SaldoTotale_B_1, "##,###,##0.00")

                'filtro avanzato
                objPianoConti.Prepara_FiltroQuery_IdRicl_ConfrontoBilanci(Filtro_Conti, Filtro_Conti_1, Filtro_Conti_2, Filtro_Conti_3)

                'recupero i dati            
                risp = objPC.PianoContiEconomici_Confronto_LeggiDS(DSBilancioConfronto, _
                                                            DSBilancioConfronto.DS_BilancioConfronto.TableName, _
                                                            Piva, _
                                                            Anno, _
                                                            AnnoConfronto, _
                                                            Filtro_Conti_1, _
                                                            Filtro_Conti_2, _
                                                            Filtro_Conti_3, _
                                                            "", _
                                                            objParametri_Server)


                If Not IsNothing(DSBilancioConfronto.DS_BilancioConfronto) Then

                    If DSBilancioConfronto.DS_BilancioConfronto.Rows.Count <> 0 Then


                        For i = 0 To DSBilancioConfronto.DS_BilancioConfronto.Rows.Count - 1

                            SaldoTotale_1 = 0
                            SaldoTotale_2 = 0

                            Str_Id_Ricl_i = CStr(DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Id_Ricl_Conto"))

                            'Str_Id_Ricl_1_i = CStr(DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Id_Riclassificazione_1"))

                            'Str_Id_Ricl_2_i = CStr(DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Id_Riclassificazione_2"))

                            'non c'è più bisogno, ho messo il select case dentro alla query
                            'If Str_Id_Ricl_1_i <> "" Then
                            '    Id_Ricl_Conto = Str_Id_Ricl_1_i
                            'ElseIf Str_Id_Ricl_2_i <> "" Then
                            '    Id_Ricl_Conto = Str_Id_Ricl_2_i
                            'Else
                            '    Id_Ricl_Conto = ""
                            'End If
                            'DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Id_Ricl_Conto") = Id_Ricl_Conto

                            Saldo_1 = Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Saldo_1")))
                            Saldo_2 = Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Saldo_2")))

                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Saldo_1") = Format(Saldo_1, "##,###,##0.00")
                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Saldo_2") = Format(Saldo_2, "##,###,##0.00")

                            If Str_Id_Ricl_i <> "" Then

                                Select Case Str_Id_Ricl_i.ToUpper

                                    Case "A"
                                        SaldoTotale_1 = SaldoTotale_A_1
                                        SaldoTotale_2 = SaldoTotale_A_2

                                    Case "B"
                                        SaldoTotale_1 = SaldoTotale_B_1
                                        SaldoTotale_2 = SaldoTotale_B_2

                                    Case Else

                                        If Filtro_Conti = "" Then
                                            'se non ho impostato un filtro sui conti, allora la query mi ha restituito tutti i conti
                                            'e posso calcolare i saldi totali, scorrendo il datatable

                                            Len_Id_Ricl_i = Str_Id_Ricl_i.Length

                                            For j = 0 To DSBilancioConfronto.DS_BilancioConfronto.Rows.Count - 1

                                                Str_Id_Ricl_j = CStr(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Id_Ricl_Conto"))

                                                If Left(Str_Id_Ricl_j, Len_Id_Ricl_i) = Str_Id_Ricl_i Then

                                                    SaldoTotale_1 += Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Saldo_1")))

                                                    SaldoTotale_2 += Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Saldo_2")))

                                                End If

                                            Next

                                            '=================================================================== 
                                        Else
                                            'se ho impostato un filtro sui conti, allora la query mi ha restituito solo i conti selezionati
                                            ' e non posso calcolare i saldi totali scorrendo il datatable, ho bisogno di eseguire una query per il calcolo del saldo

                                            risp = objPC.PianoContiEconomici_SaldoConto_Confronto(SaldoTotale_1, _
                                                                                                  SaldoTotale_2, _
                                                                                                Str_Id_Ricl_i, _
                                                                                                Piva, _
                                                                                                Anno, _
                                                                                                AnnoConfronto, _
                                                                                                objParametri_Server)


                                        End If 'Filtro_Conti

                                End Select

                            End If 'id_riclassificazione_1 <> ""


                            'A, B, C, D
                            Select Case Left(Str_Id_Ricl_i.ToUpper, 1)
                                Case "A"
                                    TotaleGruppo = SaldoTotale_A_1
                                Case "B"
                                    TotaleGruppo = SaldoTotale_B_1
                                Case Else
                                    TotaleGruppo = 0
                            End Select


                            '-----


                            ''If Str_Id_Ricl_1_i <> "" Then

                            ''    Select Case Str_Id_Ricl_1_i.ToUpper

                            ''        Case "A"
                            ''            SaldoTotale_1 = SaldoTotale_A_1
                            ''            SaldoTotale_2 = SaldoTotale_A_2

                            ''        Case "B"
                            ''            SaldoTotale_1 = SaldoTotale_B_1
                            ''            SaldoTotale_2 = SaldoTotale_B_2

                            ''        Case Else

                            ''            If Filtro_Conti = "" Then
                            ''                'se non ho impostato un filtro sui conti, allora la query mi ha restituito tutti i conti
                            ''                'e posso calcolare i saldi totali, scorrendo il datatable

                            ''                Len_Id_Ricl_1 = Str_Id_Ricl_1_i.Length

                            ''                For j = 0 To DSBilancioConfronto.DS_BilancioConfronto.Rows.Count - 1

                            ''                    Str_Id_Ricl_1_j = CStr(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Id_Riclassificazione_1"))

                            ''                    If Left(Str_Id_Ricl_1_j, Len_Id_Ricl_1) = Str_Id_Ricl_1_i Then

                            ''                        SaldoTotale_1 += Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Saldo_1")))

                            ''                        SaldoTotale_2 += Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Saldo_2")))

                            ''                    End If

                            ''                Next

                            ''                '=================================================================== 
                            ''            Else
                            ''                'se ho impostato un filtro sui conti, allora la query mi ha restituito solo i conti selezionati
                            ''                ' e non posso calcolare i saldi totali scorrendo il datatable, ho bisogno di eseguire una query per il calcolo del saldo

                            ''                NewCom_PianoConti_SaldoConto_Confronto(Server, Session, Page, _
                            ''                                                        SaldoTotale_1, SaldoTotale_2, _
                            ''                                                        DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Id_Ricl_Conto"), _
                            ''                                                        Piva, _
                            ''                                                        Anno, _
                            ''                                                        AnnoConfronto)


                            ''            End If 'Filtro_Conti

                            ''    End Select

                            ''End If 'id_riclassificazione_1 <> ""


                            '''A, B, C, D
                            ''Select Case Left(Str_Id_Ricl_1_i.ToUpper, 1)
                            ''    Case "A"
                            ''        TotaleGruppo = SaldoTotale_A_1
                            ''    Case "B"
                            ''        TotaleGruppo = SaldoTotale_B_1
                            ''    Case Else
                            ''        TotaleGruppo = 0
                            ''End Select


                            '''-----

                            ''If Str_Id_Ricl_2_i <> "" And Str_Id_Ricl_1_i = "" Then

                            ''    Select Case Str_Id_Ricl_1_i.ToUpper

                            ''        Case "A"
                            ''            SaldoTotale_2 = SaldoTotale_A_2

                            ''        Case "B"
                            ''            SaldoTotale_2 = SaldoTotale_B_2

                            ''        Case Else

                            ''            If Filtro_Conti = "" Then
                            ''                'se non ho impostato un filtro sui conti, allora la query mi ha restituito tutti i conti
                            ''                'e posso calcolare i saldi totali, scorrendo il datatable


                            ''                Len_Id_Ricl_2 = Str_Id_Ricl_2_i.Length

                            ''                For j = 0 To DSBilancioConfronto.DS_BilancioConfronto.Rows.Count - 1

                            ''                    Str_Id_Ricl_2_j = CStr(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Id_Riclassificazione_2"))

                            ''                    If Left(Str_Id_Ricl_2_j, Len_Id_Ricl_2) = Str_Id_Ricl_2_i Then

                            ''                        SaldoTotale_2 += Math.Abs(CDbl(DSBilancioConfronto.DS_BilancioConfronto.Rows(j).Item("Saldo_2")))

                            ''                    End If

                            ''                Next

                            ''                '=================================================================== 
                            ''            Else
                            ''                'se ho impostato un filtro sui conti, allora la query mi ha restituito solo i conti selezionati
                            ''                ' e non posso calcolare i saldi totali scorrendo il datatable, ho bisogno di eseguire una query per il calcolo del saldo

                            ''                NewCom_PianoConti_SaldoConto_Confronto(Server, Session, Page, _
                            ''                                                        SaldoTotale_1, SaldoTotale_2, _
                            ''                                                        DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Id_Ricl_Conto"), _
                            ''                                                        Piva, _
                            ''                                                        Anno, _
                            ''                                                        AnnoConfronto)
                            ''                SaldoTotale_1 = 0


                            ''            End If 'Filtro_Conti

                            ''    End Select

                            ''    ''A, B, C, D
                            ''    'Select Case Str_Id_Ricl_2_i.ToUpper
                            ''    '    Case "A"
                            ''    '        TotaleGruppo = SaldoTotale_A_2
                            ''    '    Case "B"
                            ''    '        TotaleGruppo = SaldoTotale_B_2
                            ''    '    Case Else
                            ''    '        TotaleGruppo = 0
                            ''    'End Select

                            ''End If 'id_riclassificazione_2 <> "" and  id_riclassificazione_1 = ""


                            '===================================================================                  
                            '===================================================================                  

                            'calcolo la differenza in percentuale
                            '-1 * ( 100 : ANNO_2 = X : (ANNO_2 - ANNO_1) ) 
                            ' X = -1 * (((ANNO_2 - ANNO_1) * 100) / ANNO_2)

                            If Saldo_2 <> 0 Then
                                Differenza_Perc_Analitico = Math.Round((-1 * (((Saldo_2 - Saldo_1) * 100) / Saldo_2)), 2)
                                Select Case Differenza_Perc_Analitico
                                    Case Is > 1000
                                        Differenza_Perc_Analitico = 1000
                                    Case Is < -1000
                                        Differenza_Perc_Analitico = -1000
                                End Select
                            Else
                                Differenza_Perc_Analitico = 100
                            End If

                            If SaldoTotale_2 <> 0 Then
                                Differenza_Perc_SaldoTotale = Math.Round((-1 * (((SaldoTotale_2 - SaldoTotale_1) * 100) / SaldoTotale_2)), 2)
                                Select Case Differenza_Perc_SaldoTotale
                                    Case Is > 1000
                                        Differenza_Perc_SaldoTotale = 1000
                                    Case Is < -1000
                                        Differenza_Perc_SaldoTotale = -1000
                                End Select
                            Else
                                Differenza_Perc_SaldoTotale = 100
                            End If

                            'calcolo il peso in percentuale
                            '100 : A = X : SALDO  ---> X = (100 * SALDO) / A
                            '100 : B = X : SALDO  ---> X = (100 * SALDO) / B
                            ''100 : C = X : SALDO  ---> X = (100 * SALDO) / C
                            ''100 : D = X : SALDO  ---> X = (100 * SALDO) / D

                            If TotaleGruppo <> 0 Then
                                PesoAnalitico_Perc_TotaleGruppo = Math.Round(((100 * Saldo_1) / TotaleGruppo), 2)
                                PesoTotale_Perc_TotaleGruppo = Math.Round((100 * SaldoTotale_1) / TotaleGruppo, 2)
                            Else
                                PesoAnalitico_Perc_TotaleGruppo = 0
                                PesoTotale_Perc_TotaleGruppo = 0
                            End If

                            '----> visualizzo

                            If Saldo_1 = SaldoTotale_1 Then

                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("SaldoTotale_1") = ""

                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("PesoTotale_Perc_TotaleGruppo") = ""
                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("PesoTotale_TotaleGruppo") = 0

                                If Saldo_2 = SaldoTotale_2 Then

                                    DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("SaldoTotale_2") = ""

                                    'Differenza_SaldoTotale
                                    DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_Perc_SaldoTotale") = ""
                                    DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_SaldoTotale") = 0

                                Else

                                    DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("SaldoTotale_2") = Format(Math.Abs(SaldoTotale_2), "##,###,##0.00")

                                    'Differenza_SaldoTotale
                                    DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_Perc_SaldoTotale") = Format(Differenza_Perc_SaldoTotale, "##,###,##0.00") + " %"
                                    DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_SaldoTotale") = 0

                                End If

                            Else

                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("SaldoTotale_1") = Format(Math.Abs(SaldoTotale_1), "##,###,##0.00")

                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("PesoTotale_Perc_TotaleGruppo") = Format(PesoTotale_Perc_TotaleGruppo, "##,###,##0.00") + " %"
                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("PesoTotale_TotaleGruppo") = 0

                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("SaldoTotale_2") = Format(Math.Abs(SaldoTotale_2), "##,###,##0.00")

                                'Differenza_SaldoTotale
                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_Perc_SaldoTotale") = Format(Differenza_Perc_SaldoTotale, "##,###,##0.00") + " %"
                                DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_SaldoTotale") = 0

                            End If

                            'Differenza_Analitico  
                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_Perc_Analitico") = Format(Differenza_Perc_Analitico, "##,###,##0.00") + " %"
                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("Differenza_Analitico") = 0

                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("TotaleGruppo") = TotaleGruppo

                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("PesoAnalitico_Perc_TotaleGruppo") = Format(PesoAnalitico_Perc_TotaleGruppo, "##,###,##0.00") + " %"
                            DSBilancioConfronto.DS_BilancioConfronto.Rows(i).Item("PesoAnalitico_TotaleGruppo") = 0



                        Next

                    End If

                End If


                'imposto il dataset sul report
                rptBilancioConfronto.SetDataSource(DSBilancioConfronto)


                '########################################################################
                '########################################################################

        End Select



    End Sub




End Class

