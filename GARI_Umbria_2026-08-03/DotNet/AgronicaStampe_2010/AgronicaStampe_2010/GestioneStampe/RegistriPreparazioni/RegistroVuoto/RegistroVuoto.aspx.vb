Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class RegistroVuoto
    Inherits System.Web.UI.Page

    Private rptRegistroVuoto As RPT_RegistroVuoto
    Private DsRegistroVuoto As DS_RegistroVuoto

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    'Dim Qs_Sa_Cod As String
    Dim QS_Report As String
    Dim QS_NumPagine As String
    Dim QS_NumPagineDa As String
    Dim QS_NumPagineA As String
    Dim Qs_StampaIntestazione As String
    Dim Qs_ProgrSiglaRegistro As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " REGISTRO VUOTO "

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
        rptRegistroVuoto = New RPT_RegistroVuoto
    End Sub

#End Region

    '##############################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        'Faccio scadere subito la pagina memorizzata nella cache
        Response.Expires = 0

        '#################################################################################
        '#####  Recupero dati dalla QueryString 
        '#################################################################################

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_RagSoc = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                            AgroKey_EncoderDecoder, _
                            Server)

        QS_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        QS_NumPagine = Stringa_Decodifica(Request.QueryString("np").ToString, _
                                          AgroKey_EncoderDecoder, _
                                          Server)

        QS_NumPagineDa = Stringa_Decodifica(Request.QueryString("npda").ToString, _
                                          AgroKey_EncoderDecoder, _
                                          Server)

        QS_NumPagineA = Stringa_Decodifica(Request.QueryString("npa").ToString, _
                                          AgroKey_EncoderDecoder, _
                                          Server)

        Qs_StampaIntestazione = Stringa_Decodifica(Request.QueryString("si").ToString, _
                                                   AgroKey_EncoderDecoder, _
                                                   Server)

        Qs_ProgrSiglaRegistro = Stringa_Decodifica(Request.QueryString("pr").ToString, _
                                                AgroKey_EncoderDecoder, _
                                                Server)



        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "RegistroVuoto"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            Dim DsPagine As New DS_RegistroVuoto

            Try

                'LI NASCONDO TUTTI

                '2: VINIFICAZIONE DOC
                rptRegistroVuoto.Section2.SectionFormat.EnableSuppress = True
                '11 VINIFICAZIONE TAVOLA
                rptRegistroVuoto.Section11.SectionFormat.EnableSuppress = True
                '8 REGISTRO VINIFICAZIONE DOC E DA TAVOLA
                rptRegistroVuoto.Section8.SectionFormat.EnableSuppress = True

                '10: IMBOTTIGLIAMENTO
                rptRegistroVuoto.Section10.SectionFormat.EnableSuppress = True
                '9: COMMERCIALIZZAZIONE
                rptRegistroVuoto.Section9.SectionFormat.EnableSuppress = True
                '6: FRIZZANTI
                rptRegistroVuoto.Section6.SectionFormat.EnableSuppress = True
                '7: SPUMANTI
                rptRegistroVuoto.Section7.SectionFormat.EnableSuppress = True


                Carica_DsPagine(DsPagine)


                If Qs_StampaIntestazione = "1" Then

                    'LEGENDA:
                    '2 VINIFICAZIONE - V.Q.P.R.D 
                    '11 VINIFICAZIONE - VINI DA TAVOLA
                    '10 IMBOTTIGLIAMENTO
                    '9 GENERALE
                    '3 DETTAGLI

                    Select Case QS_Report

                        Case enum_AgroReportistica.Vinificazione_DOC
                            '2 VINIFICAZIONE - V.Q.P.R.D 
                            'CType(rptRegistroVuoto.Section2.ReportObjects("TxtRagSoc1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            'rptRegistroVuoto.Section2.SectionFormat.EnableSuppress = False
                            CType(rptRegistroVuoto.Section8.ReportObjects("TxtRagSoc1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            rptRegistroVuoto.Section8.SectionFormat.EnableSuppress = False

                        Case enum_AgroReportistica.Vinificazione_ViniTavola
                            '11 VINIFICAZIONE - VINI DA TAVOLA
                            'CType(rptRegistroVuoto.Section11.ReportObjects("TxtRagSoc2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            'rptRegistroVuoto.Section11.SectionFormat.EnableSuppress = False
                            CType(rptRegistroVuoto.Section8.ReportObjects("TxtRagSoc1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            rptRegistroVuoto.Section8.SectionFormat.EnableSuppress = False

                        Case enum_AgroReportistica.Imbottigliamento
                            '10 IMBOTTIGLIAMENTO
                            CType(rptRegistroVuoto.Section10.ReportObjects("TxtRagSoc3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            rptRegistroVuoto.Section10.SectionFormat.EnableSuppress = False

                        Case enum_AgroReportistica.Commercializzazione
                            '9 COMMERCIALIZZAZIONE
                            CType(rptRegistroVuoto.Section9.ReportObjects("TxtRagSoc4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            rptRegistroVuoto.Section9.SectionFormat.EnableSuppress = False

                        Case enum_AgroReportistica.Frizzanti
                            '6 FRIZZANTI
                            CType(rptRegistroVuoto.Section6.ReportObjects("TxtRagSoc5"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            rptRegistroVuoto.Section6.SectionFormat.EnableSuppress = False

                        Case enum_AgroReportistica.Spumanti
                            '7 SPUMANTI
                            CType(rptRegistroVuoto.Section7.ReportObjects("TxtRagSoc6"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                            rptRegistroVuoto.Section7.SectionFormat.EnableSuppress = False

                        Case enum_CodificaStampe.Registro_FattureAcquisto, _
                                enum_CodificaStampe.Registro_FattureVendita
                            'al momento l'intestazione la stampiamo insieme ai dati, non nel registro vuoto

                        Case Else

                    End Select

                End If


                'sorgente dati.....
                rptRegistroVuoto.SetDataSource(DsPagine)

            Catch exc As Exception
                Log_Errori += " " + vbCrLf + exc.Message + vbCrLf
            End Try



            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                      "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                      vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                      Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Cantine", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "RegistroVuoto.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------
        End If

        '==================================================================

        Try
            Session("Report") = rptRegistroVuoto
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
        Catch ex As Exception
            Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub


    '###########################################################################
    'riempie il ds x il Registro imbottigliamento...
    Private Sub Carica_DsPagine(ByRef DsRegistroVuoto As DS_RegistroVuoto)

        Dim i As Integer
        Dim RigaDs As DS_RegistroVuoto.DS_RegistroVuotoRow

        Dim Inizio As Integer = CInt(QS_NumPagineDa)
        Dim Fine As Integer = CInt(QS_NumPagineA)

        For i = Inizio To CInt(Fine)

            RigaDs = DsRegistroVuoto.DS_RegistroVuoto.NewDS_RegistroVuotoRow

            RigaDs.NumPagina = Qs_ProgrSiglaRegistro + "   " + (i).ToString & IIf(QS_NumPagine <> String.Empty, "/" & QS_NumPagine, "")

            'Inserisco la riga
            DsRegistroVuoto.DS_RegistroVuoto.Rows.Add(RigaDs)

        Next

    End Sub


End Class
