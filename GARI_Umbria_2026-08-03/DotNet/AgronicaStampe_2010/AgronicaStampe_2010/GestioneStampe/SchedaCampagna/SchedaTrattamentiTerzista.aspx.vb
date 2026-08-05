Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SchedaTrattamentiTerzista
    Inherits System.Web.UI.Page


    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_TrattTerzista As Rpt_SchedaTrattamentiTerzista

#Region " Scheda Trattamenti Terzista "

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

        '----------------------------
        '   INIZIALIZZAZIONE REPORT
        '----------------------------
        Rpt_TrattTErzista = New Rpt_SchedaTrattamentiTerzista

    End Sub

#End Region

    Dim Piva, Data_Giorno, Data_Inizio, Data_Fine As String
    Private Qs_StampaDefinitiva As String

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        'Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")), _
        '                 AgroKey_EncoderDecoder, _
        '                 Server)

        'Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")), _
        '                 AgroKey_EncoderDecoder, _
        '                 Server)

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                            AgroKey_EncoderDecoder, _
                            Server)

        Data_Giorno = Stringa_Decodifica(CStr(Request.QueryString("dG")), _
                    AgroKey_EncoderDecoder, _
                    Server)

        Data_Inizio = Stringa_Decodifica(CStr(Request.QueryString("dI")), _
                    AgroKey_EncoderDecoder, _
                    Server)

        Data_Fine = Stringa_Decodifica(CStr(Request.QueryString("dF")), _
                    AgroKey_EncoderDecoder, _
                    Server)

        ' se è una stampa definitiva salvo il pdf e visualizzo l'anteprima, altrimenti visualizzo solo l'anteprima
        If Not IsNothing(Request.QueryString("stDef")) AndAlso Stringa_Decodifica(Request.QueryString("stDef").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server) <> "" Then
            Qs_StampaDefinitiva = Stringa_Decodifica(Request.QueryString("stDef").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)
        Else
            Qs_StampaDefinitiva = "0"       ' stampa di prova
        End If


        'Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
        '                          AgroKey_EncoderDecoder, _
        '                          Server))

        'Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
        '                          AgroKey_EncoderDecoder, _
        '                          Server))

        'Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
        '                  AgroKey_EncoderDecoder, _
        '                  Server))

        'Descr_Imballo = Stringa_Decodifica(CStr(Request.QueryString("imb")), _
        '                                    AgroKey_EncoderDecoder, _
        '                                    Server)

        'Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
        '                                        AgroKey_EncoderDecoder, _
        '                                        Server)

        'RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
        '                                   AgroKey_EncoderDecoder, _
        '                                   Server)

        'If RagSoc_Impresa = "" Then
        '    RagSoc_Impresa = RagSoc_from_Piva(Server, Session, Page, Piva)
        'End If

        'Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
        '             AgroKey_EncoderDecoder, _
        '             Server)

        'If Codice_Conferente = "0" Then
        '    Codice_Conferente = ""
        '    'il codice non è stato passato, 
        '    'perchè si vogliono cercare tutti i conferenti
        '    'o il range di conferenti selezionato
        'Else
        '    Codice_Conferente = CStr(Codice_Conferente)
        'End If

        'Str_FiltroConf = Session("Str_Codici_Conferenti")

        'If Str_FiltroConf <> "" Then
        '    'è stato selezionato un range di codici
        '    Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
        'Else
        '    Str_FiltroConf = ""
        'End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "SchedaTrattamentiTerzista"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            'Dim DS As New DS_EC_Imballi

            Try

                CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
                CrystalReportViewer1.Style.Add("LEFT", "-275px")
                CrystalReportViewer1.Style.Add("TOP", "0px")
                CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

                Log_Errori = ""

                'FUNZIONE CHE FA TUTTO
                Stampa_Spett(Log_Errori)


            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Path_Errore, Str_Errore_Path, Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + " " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_QDC", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "SchedaTrattamentiTerzista.aspx", _
                                                 Log_Errori)

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_QDC", Path_Errore, Str_Errore_Path)

                'If Str_Errore_Path = "" And Path_Errore <> "" Then

                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )

                'End If

            End If
            '-----------------------------------------

            If Qs_StampaDefinitiva = "1" Then

                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)
                objCatDoc = Nothing

                Dim DataOraStampa As String

                DataOraStampa = Right("00" & Now.Day.ToString, 2) & _
                                Right("00" & Now.Month.ToString, 2) & _
                                Right("00" & Now.Year.ToString, 4) & "_" & _
                                Right("00" & Now.Hour.ToString, 2) & _
                                Right("00" & Now.Minute.ToString, 2)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(Session("Report"), _
                                           enum_CategorieDocumenti.RegistriCampagna, _
                                           Sottocartella, _
                                           Nome_Documento & "_p" & Piva & "_d" & DataOraStampa + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                                                                 enum_CategorieDocumenti.RegistriCampagna, _
                                                                 "Nome_Documento", _
                                                                  Nome_Documento & "_p" & Piva & "_d" & DataOraStampa + ".pdf", _
                                                                 Sottocartella, _
                                                                 "", "", "", "", _
                                                                 CDate(Data_Inizio), _
                                                                 CDate(Data_Fine), _
                                                                 objParametri_Server)

            End If

            Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

            Exit Sub

        Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            If Not IsNothing(Session("DS")) Then

                'imposto la sorgente dati x il report...
                Rpt_TrattTerzista.SetDataSource(CType(Session("DS")(0), DataSet))

                'faccio il databind col visualizzatore dei reports...
                CrystalReportViewer1.ReportSource = Rpt_TrattTerzista
                CrystalReportViewer1.DataBind()

            End If

        End If

    End Sub


    '#################################################################################
    Private Sub Stampa_Spett(ByVal Log_Errori As String)

        Dim Anno As String

        'Anno = Ricava_Anno(Stringa_Decodifica(Request.QueryString("dG").ToString, _
        '                    AgroKey_EncoderDecoder, _
        '                    Server), _
        '                     Stringa_Decodifica(Request.QueryString("dI").ToString, _
        '                        AgroKey_EncoderDecoder, _
        '                        Server), _
        '                         Stringa_Decodifica(Request.QueryString("dF").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server))

        Anno = Ricava_Anno(Data_Giorno, Data_Inizio, Data_Fine)

        CType(Rpt_TrattTerzista.Section2.ReportObjects("TxtAnno"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Anno

        Dim Dt_Spett As DataTable
        Dim objInt As New AgronicaCoreStampeDAL.DocContab
        Dt_Spett = objInt.DatiIntestazioneImpresa(Piva, Log_Errori, "", "", objParametri_Server)

        If Not IsNothing(Dt_Spett) AndAlso Dt_Spett.Rows.Count > 0 Then

            Dim Riga1, Riga2 As String

            Riga1 = Dt_Spett.Rows(0).Item("rag_soc") + " (p.iva " + Piva + ")"
            Riga2 = Dt_Spett.Rows(0).Item("ind_impresa")

            If Dt_Spett.Rows(0).Item("frz_des") <> "" Then
                Riga2 += " " + Dt_Spett.Rows(0).Item("frz_des")
            End If

            Riga2 += " " + Dt_Spett.Rows(0).Item("cap")

            Riga2 += " " + Dt_Spett.Rows(0).Item("localita")

            Riga2 += " (" + Dt_Spett.Rows(0).Item("comuni_prov") + ") "

            CType(Rpt_TrattTerzista.Section2.ReportObjects("TxtSpettRiga1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Riga1
            CType(Rpt_TrattTerzista.Section2.ReportObjects("TxtSpettRiga2"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Riga2

        End If


        Session("Report") = Rpt_TrattTerzista



    End Sub






End Class
