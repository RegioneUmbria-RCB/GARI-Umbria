Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class SaldoImballi
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_SaldoImballi As Rpt_SaldoImballi


#Region " SALDO IMBALLI "

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
        Rpt_SaldoImballi = New Rpt_SaldoImballi
    End Sub

#End Region


    Dim Data_Da, Data_A, Data_Giacenza As String
    Dim Codice_Conferente As String
    Dim Str_FiltroConf As String
    Dim Piva As String
    Dim Sa_Cod, Fabbricato_Cod, Mat_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Descr_Imballo As String
    Dim Descr_Magazzino As String

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Data_Da = Stringa_Decodifica(CStr(Request.QueryString("dd")), _
                         AgroKey_EncoderDecoder, _
                         Server)

        Data_A = Stringa_Decodifica(CStr(Request.QueryString("da")), _
                         AgroKey_EncoderDecoder, _
                         Server)

        Data_Giacenza = Stringa_Decodifica(CStr(Request.QueryString("dg")), _
                         AgroKey_EncoderDecoder, _
                         Server)

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
                          AgroKey_EncoderDecoder, _
                          Server))

        Descr_Imballo = Stringa_Decodifica(CStr(Request.QueryString("imb")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
                                                AgroKey_EncoderDecoder, _
                                                Server)

        RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)


        If RagSoc_Impresa = "" Then
            RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
        End If



        Codice_Conferente = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                AgroKey_EncoderDecoder, _
                                Server)


        If Codice_Conferente = "0" Then
            Codice_Conferente = ""
            'il codice non è stato passato, 
            'perchè si vogliono cercare tutti i conferenti
            'o il range di conferenti selezionato
        Else
            Codice_Conferente = CStr(Codice_Conferente)
        End If

        Str_FiltroConf = Session("Str_Codici_Conferenti")

        If Str_FiltroConf <> "" Then
            'è stato selezionato un range di codici
            Str_FiltroConf = " AND Risorse_Umane_Conferenti.settore_Des IN " & Str_FiltroConf
        Else
            Str_FiltroConf = ""
        End If




        '#################################################################################
        '#####  Genero il report
        '#################################################################################
        Dim Nome_Documento As String = "SaldoImballi"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_SaldoImballi

            Try

                Log_Errori = ""

                'FUNZIONE CHE FA TUTTO
                Stampa_SaldoImballi(DS, Log_Errori)

            Catch exc As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Data DA = " + CStr(Data_Da) + ", " + vbCrLf + _
                                "Data A = " + CStr(Data_A) + ", " + vbCrLf + _
                                "Data Giacenza = " + CStr(Data_Giacenza) + ", " + vbCrLf + _
                                "Codice Conferente = " + CStr(Codice_Conferente) + ", " + vbCrLf + _
                                "Stringa Filtro Conferenti = " + CStr(Str_FiltroConf) + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Sa_Cod) + ", " + vbCrLf + _
                                "Fabbricato_Cod = " + CStr(Fabbricato_Cod) + ", " + vbCrLf + _
                                "Mat_Cod = " + CStr(Mat_Cod) + ", " + vbCrLf + _
                                "Descrizione Imballo = " + CStr(Descr_Imballo) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_AccettazioneDaDiversi", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "SaldoImballi.aspx", _
                                                 Log_Errori)

            End If




            Try
                Session("Report") = Rpt_SaldoImballi
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        '==================================================================

    End Sub



    '#####################################################################################################
    Private Sub Stampa_SaldoImballi(ByRef DS As DS_SaldoImballi, _
                                    ByRef Log_Errori As String)


        Dim DT As DataTable
        Dim i As Integer


        CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = RagSoc_Impresa
        CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtStabilimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr_Magazzino
   
        If CDate(Data_Da) = AGRODATAINIZIO Then
            CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Da
        End If

        If CDate(Data_A) = AGRODATAFINE Then
            CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_A
        End If

        CType(Rpt_SaldoImballi.Section2.ReportObjects("TxtDataGiacenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Giacenza

        Try

            'DT = NewCom_ADD_SaldoImballi(Server, Session, Page, _
            '                                Piva, _
            '                                Sa_Cod, _
            '                                Fabbricato_Cod, _
            '                                Mat_Cod, _
            '                                Data_Da, _
            '                                Data_A, _
            '                                Data_Giacenza, _
            '                                Codice_Conferente, _
            '                                Str_FiltroConf, _
            '                                "")

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi


            DT = ADD.SaldoImballi(Piva, _
                                    Sa_Cod, _
                                    Fabbricato_Cod, _
                                    Mat_Cod, _
                                    Data_Da, _
                                    Data_A, _
                                    Data_Giacenza, _
                                    Codice_Conferente, _
                                    Str_FiltroConf, _
                                    AGRODATAINIZIO, _
                                    AGRODATAFINE, _
                                    "", "", _
                                    objParametri_Server)

        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                Dim DR As DS_SaldoImballi.DS_SaldoImballiRow
                Dim Codice_Conferente As String
                Dim RagSoc_Conferente As String
                Dim Codice_Imballo As String
                Dim Descr_Imballo As String
                Dim Totale_Entrate As Double
                Dim Totale_Uscite As Double
                Dim Differenza As Double
                Dim Giacenza_Attuale As Double


                For i = 0 To DT.Rows.Count - 1

                    'Modifica del 14/10/2010: è cambiata la query e le giacenze =0 sono già filtrate

                    'Giacenza_Attuale = DT.Rows(i).Item("Giacenza_Attuale")

                    ''Modifica del 25/08/2010: visualizzare solo gli imballi che hanno una giacenza attuale
                    'If Giacenza_Attuale <> 0 Then

                    '-----------------------------------------
                    'aggiungo la riga nel dataset
                    DR = DS.DS_SaldoImballi.NewDS_SaldoImballiRow

                    Codice_Conferente = DT.Rows(i).Item("Codice_Conferente")
                    RagSoc_Conferente = DT.Rows(i).Item("RagSoc_Conferente")
                    Codice_Imballo = DT.Rows(i).Item("Cod_Articolo")
                    Descr_Imballo = DT.Rows(i).Item("Mat_Des")
                    Totale_Entrate = DT.Rows(i).Item("Qta_Totale_Carichi")
                    Totale_Uscite = DT.Rows(i).Item("Qta_Totale_Scarichi")
                    Differenza = Totale_Entrate - Totale_Uscite

                    DR.Codice_Conferente = Codice_Conferente
                    DR.RagSoc_Conferente = RagSoc_Conferente
                    DR.Codice_Imballo = Codice_Imballo
                    DR.Descr_Imballo = Descr_Imballo
                    DR.Data_Saldo_Prec = ""
                    DR.Saldo_Prec = ""
                    DR.Totale_Entrate = Totale_Entrate
                    DR.Totale_Uscite = Totale_Uscite
                    DR.Differenza = Differenza
                    DR.Giacenza_Attuale = DT.Rows(i).Item("Giacenza")

                    DS.DS_SaldoImballi.Rows.Add(DR)
                    '---------------------------

                    'End If

                Next

            End If 'controllo sul dt

        Catch ex As Exception
            Log_Errori += "- Valorizzazione dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '####################################################

        Try

            'imposto il datset sul report
            Rpt_SaldoImballi.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

    End Sub

End Class
