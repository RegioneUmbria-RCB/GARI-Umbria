Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class EC_Imballi
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_EC_Imballi As Rpt_EC_Imballi

#Region " ESTRATTO CONTO IMBALLI "

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
        Rpt_EC_Imballi = New Rpt_EC_Imballi

    End Sub

#End Region


    Dim Data_Da, Data_A As String
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

        Dim Nome_Documento As String = "EstrattoConto_Imballi"
        Dim Log_Errori As String

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_EC_Imballi

            Try

                Log_Errori = ""

                'FUNZIONE CHE FA TUTTO
                Stampa_EstrattoConto_Imballi(DS, Log_Errori)


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
                                                 "EC_Imballi.aspx", _
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Try
                Session("Report") = Rpt_EC_Imballi
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If


    End Sub



    '#####################################################################################################
    Private Sub Stampa_EstrattoConto_Imballi(ByRef DS As DS_EC_Imballi, _
                                                ByRef Log_Errori As String)


        Dim DT As DataTable
        Dim i As Integer
        Dim Data_Saldo_Precedente As Date


        CType(Rpt_EC_Imballi.Section2.ReportObjects("TxtImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = RagSoc_Impresa
        CType(Rpt_EC_Imballi.Section2.ReportObjects("TxtStabilimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr_Magazzino

        If CDate(Data_Da) = AGRODATAFINE Then
            CType(Rpt_EC_Imballi.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_EC_Imballi.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Da
        End If

        If CDate(Data_A) = AGRODATAFINE Then
            CType(Rpt_EC_Imballi.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_EC_Imballi.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_A
        End If



        Data_Saldo_Precedente = CDate(Data_Da).AddDays(-1.0)

        Try

            'DT = NewCom_ADD_EstrattoConto_Imballi(Server, Session, Page, _
            '                                        Piva, _
            '                                        Sa_Cod, _
            '                                        Fabbricato_Cod, _
            '                                        Mat_Cod, _
            '                                        Data_Da, _
            '                                        Data_A, _
            '                                        Data_Saldo_Precedente, _
            '                                        Codice_Conferente, _
            '                                        Str_FiltroConf)

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.EstrattoConto_Imballi(Piva, _
                                          Sa_Cod, _
                                          Fabbricato_Cod, _
                                          Mat_Cod, _
                                          Data_Da, _
                                          Data_A, _
                                          Data_Saldo_Precedente, _
                                          Codice_Conferente, _
                                          Str_FiltroConf, _
                                          objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try



        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Dim DR As DS_EC_Imballi.DS_EC_ImballiRow

            Dim Codice_Conferente As String
            Dim RagSoc_Conferente As String
            Dim Codice_Imballo As String
            Dim Descr_Imballo As String
            Dim Numero_Doc As String
            Dim Data_Doc As String
            Dim Causale As String
            Dim Tipo_Movimento As String
            Dim Qta As Double
            Dim Ritirati As Double
            Dim Resi As Double
            Dim Lav_Cod As Integer

            Dim Tot_Ritirati As Integer = 0
            Dim Tot_Resi As Integer = 0
            Dim Giacenza_Finale As Integer = 0
            Dim Giacenza As Integer
            Dim Qta_Da_Sommare As Integer

            Dim Memo_Conf, Memo_Imballo As String

            For i = 0 To DT.Rows.Count - 1

                '-----------------------------------------
                'aggiungo la riga nel dataset
                DR = DS.DS_EC_Imballi.NewDS_EC_ImballiRow

                Codice_Conferente = DT.Rows(i).Item("Codice_Conferente")
                RagSoc_Conferente = DT.Rows(i).Item("RagSoc_Conferente")
                Codice_Imballo = DT.Rows(i).Item("Cod_Articolo")
                Descr_Imballo = DT.Rows(i).Item("Mat_Des")

                DR.Codice_Conferente = Codice_Conferente
                DR.RagSoc_Conferente = RagSoc_Conferente
                DR.Codice_Imballo = Codice_Imballo
                DR.Descr_Imballo = Descr_Imballo

                Lav_Cod = CInt(DT.Rows(i).Item("lav_cod"))

                'escludo il record della giacenza
                If Lav_Cod <> -1 Then

                    Data_Doc = DT.Rows(i).Item("Data_Doc")

                    If CInt(DT.Rows(i).Item("Lunghezza_Sin")) <> -1 Then

                        Numero_Doc = Ricava_NumeroDocumento_Con_Sequenza(DT.Rows(i).Item("Doc_Numero_Sin"), _
                                                                        DT.Rows(i).Item("Doc_Numero"), _
                                                                        DT.Rows(i).Item("Doc_Numero_Des"), _
                                                                        DT.Rows(i).Item("Lunghezza_Sin"), _
                                                                        DT.Rows(i).Item("Lunghezza_Centro"), _
                                                                        DT.Rows(i).Item("Lunghezza_Des"), _
                                                                        DT.Rows(i).Item("CarattereFormattazione"))
                    Else
                        Numero_Doc = Ricava_NumeroDocumento_Senza_Sequenza(CStr(DT.Rows(i).Item("Doc_Numero_Sin")), CDbl(DT.Rows(i).Item("Doc_Numero")), CStr(DT.Rows(i).Item("Doc_Numero_Des")))
                    End If
                Else
                    Data_Doc = Data_Saldo_Precedente
                    Numero_Doc = ""
                End If

                Select Case Lav_Cod
                    Case LAVCOD_ACCETTAZIONE_DIVERSI
                        DR.Data_Bolla = Data_Doc
                        DR.Numero_Bolla = Numero_Doc
                    Case -1
                        DR.Data_Doc = Data_Doc
                    Case Else
                        DR.Data_DDT = Data_Doc
                        DR.Numero_DDT = Numero_Doc
                End Select

                Causale = DT.Rows(i).Item("Causale")
                Qta = DT.Rows(i).Item("Qta")

                If Memo_Conf = "" And Memo_Imballo = "" Then
                    'primo giro
                    Memo_Conf = Codice_Conferente
                    Memo_Imballo = Codice_Imballo

                    Tot_Ritirati = 0
                    Tot_Resi = 0
                    Giacenza = 0
                Else
                    If Memo_Conf = Codice_Conferente And Memo_Imballo = Codice_Imballo Then
                        'sempre il gruppo
                    Else
                        'cambiato l'imballo e/o il conferente
                        'vanno azzerati i totali

                        Giacenza_Finale = Giacenza

                        'CType(Rpt_EC_Imballi.Section9.ReportObjects("TxtRiepilogoGiacenza"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Giacenza_Finale

                        Memo_Conf = Codice_Conferente
                        Memo_Imballo = Codice_Imballo

                        Tot_Ritirati = 0
                        Tot_Resi = 0
                        Giacenza = 0
                    End If
                End If

                Select Case Causale

                    Case "-1"
                        Tipo_Movimento = "SALDO PRECEDENTE"
                        Ritirati = Qta
                        Resi = 0
                        Tot_Ritirati += Ritirati
                        Qta_Da_Sommare = Ritirati

                    Case CAU_CARICO
                        Tipo_Movimento = "ENTRATA"
                        Ritirati = Qta
                        Resi = 0
                        Tot_Ritirati += Ritirati
                        Qta_Da_Sommare = Ritirati

                    Case CAU_SCARICO
                        Tipo_Movimento = "USCITA"
                        Resi = Qta
                        Ritirati = 0
                        Tot_Resi += Resi
                        Qta_Da_Sommare = -1 * Resi

                End Select

                Giacenza += Qta_Da_Sommare

                DR.Tipo_Movimento = Tipo_Movimento
                DR.Ritirati = Ritirati
                DR.Resi = Resi
                DR.Giacenza = Giacenza

                DS.DS_EC_Imballi.Rows.Add(DR)
                '---------------------------

            Next


        End If 'controllo sul dt

        '####################################################

        Try

            'imposto il datset sul report
            Rpt_EC_Imballi.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

    End Sub




End Class
