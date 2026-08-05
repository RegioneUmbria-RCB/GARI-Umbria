Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class EC_BolleAccettazione
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_EC_Bolle As Rpt_EC_BolleAccettazione

#Region " ESTRATTO CONTO BOLLE ACCETTAZIONE "

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
        Rpt_EC_Bolle = New Rpt_EC_BolleAccettazione
    End Sub

#End Region


    Dim Data_Da, Data_A As String
    Dim Codice_Conferente As String
    Dim Codice_Specie As String
    Dim Codice_Prodotto As String
    Dim Str_FiltroConf As String
    Dim Str_FiltroSpecie As String
    Dim Piva As String
    'Dim Piva_Produttore As String
    Dim Mat_Cod As Integer
    Dim Sa_Cod, Fabbricato_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Descr_Specie As String
    Dim Descr_Prodotto As String
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

        'Piva_Produttore = Stringa_Decodifica(CStr(Request.QueryString("pp")), _
        '                                    AgroKey_EncoderDecoder, _
        '                                    Server)

        Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
                             AgroKey_EncoderDecoder, _
                             Server))

        Descr_Specie = Stringa_Decodifica(CStr(Request.QueryString("spe")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Descr_Prodotto = Stringa_Decodifica(CStr(Request.QueryString("pro")), _
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


        Codice_Specie = Stringa_Decodifica(CStr(Request.QueryString("cs")), _
                                AgroKey_EncoderDecoder, _
                                Server)

        If Codice_Specie = "0" Then
            Codice_Specie = ""
            'il codice non è stato passato, 
            'perchè si vogliono cercare tutte le specie
            'o il range di specie selezionato
        End If

        Str_FiltroSpecie = Session("Str_Codici_Specie")

        If Str_FiltroSpecie <> "" Then
            'è stato selezionato un range di codici
            Str_FiltroSpecie = " AND MP_Raccolta.Cod_Articolo IN " & Str_FiltroSpecie
        Else
            Str_FiltroSpecie = ""
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

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "EstrattoConto_Bolle"
        Dim Log_Errori As String

        Log_Errori = ""

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_EC_BolleAccettazione

            Try


                'FUNZIONE CHE FA TUTTO
                Stampa_EstrattoConto_Bolle(DS, Log_Errori)


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
                                "Codice Specie = " + CStr(Codice_Specie) + ", " + vbCrLf + _
                                "Stringa Filtro Specie = " + CStr(Str_FiltroSpecie) + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Sa_Cod) + ", " + vbCrLf + _
                                "Fabbricato_Cod = " + CStr(Fabbricato_Cod) + ", " + vbCrLf + _
                                "Mat_Cod = " + CStr(Mat_Cod) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username"))

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_AccettazioneDaDiversi", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                'GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_AccettazioneDaDiversi", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "EC_BolleAccettazione.aspx", _
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Try
                Session("Report") = Rpt_EC_Bolle
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        '==================================================================


    End Sub



    '#####################################################################################################
    Private Sub Stampa_EstrattoConto_Bolle(ByRef DS As DS_EC_BolleAccettazione, _
                                            ByRef Log_Errori As String)

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim DT As DataTable
        Dim i As Integer

        CType(Rpt_EC_Bolle.Section2.ReportObjects("TxtImpresa"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = RagSoc_Impresa
        CType(Rpt_EC_Bolle.Section2.ReportObjects("TxtStabilimento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Descr_Magazzino

        If CDate(Data_Da) = AGRODATAINIZIO Then
            CType(Rpt_EC_Bolle.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_EC_Bolle.Section2.ReportObjects("TxtDataDA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_Da
        End If

        If CDate(Data_A) = AGRODATAFINE Then
            CType(Rpt_EC_Bolle.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        Else
            CType(Rpt_EC_Bolle.Section2.ReportObjects("TxtDataA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Data_A
        End If


        Try

            'DT = NewCom_ADD_EstrattoConto_BolleAccettazione_Leggi(Server, Session, Page, _
            '                                                        Piva, _
            '                                                        Sa_Cod, _
            '                                                        Fabbricato_Cod, _
            '                                                        Codice_Specie, _
            '                                                        Codice_Conferente, _
            '                                                        "", _
            '                                                        Data_Da, _
            '                                                        Data_A, _
            '                                                        Mat_Cod, _
            '                                                        Str_FiltroSpecie, _
            '                                                        Str_FiltroConf, _
            '                                                        "")

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.EstrattoConto_Bolle(Piva, _
                                        Sa_Cod, _
                                        Fabbricato_Cod, _
                                        Codice_Specie, _
                                        Codice_Conferente, _
                                        "", _
                                        Data_Da, _
                                        Data_A, _
                                        Mat_Cod, _
                                        Str_FiltroSpecie, _
                                        Str_FiltroConf, _
                                        "", "", _
                                        objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Dim RagSoc_Conferente, Descr_Varieta As String
            Dim Data_Bolla, Numero_Bolla As String
            Dim Data_DDT, Numero_DDT As String
            Dim Tipo_Peso As Integer
            Dim Peso, Tara_Imballi As Double
            Dim Peso_Lordo, Peso_Netto As Double
            Dim Degrado_Perc, Degrado As Double
            Dim Netto_Pag As Double
            Dim Udm_Cod As Integer
            Dim Campionatura As String
            Dim Punteggio As Double


            Dim DR As DS_EC_BolleAccettazione.DS_EC_BolleAccettazioneRow


            For i = 0 To DT.Rows.Count - 1

                '-----------------------------------------
                'aggiungo la riga nel dataset
                DR = DS.DS_EC_BolleAccettazione.NewDS_EC_BolleAccettazioneRow


                Codice_Conferente = DT.Rows(i).Item("Codice_Conferente")
                RagSoc_Conferente = DT.Rows(i).Item("RagSoc_Conferente")

                Codice_Specie = DT.Rows(i).Item("Cod_Articolo_Raccolta")
                Codice_Prodotto = DT.Rows(i).Item("Codice_Prodotto")

                objADDFun.Ricava_Specie_Varieta_FRG(DT.Rows(i).Item("Mat_Des_Raccolta"), Descr_Specie, Descr_Varieta)


                Data_Bolla = DT.Rows(i).Item("Data_Bolla")

                Numero_Bolla = Ricava_NumeroDocumento_Con_Sequenza(DT.Rows(i).Item("Doc_Numero_Sin"), _
                                                                    DT.Rows(i).Item("Doc_Numero"), _
                                                                    DT.Rows(i).Item("Doc_Numero_Des"), _
                                                                    DT.Rows(i).Item("Lunghezza_Sin"), _
                                                                    DT.Rows(i).Item("Lunghezza_Centro"), _
                                                                    DT.Rows(i).Item("Lunghezza_Des"), _
                                                                    DT.Rows(i).Item("CarattereFormattazione"))

                Data_DDT = DT.Rows(i).Item("Data_Conf")

                Numero_DDT = Ricava_NumeroDocumento_Senza_Sequenza(CStr(DT.Rows(i).Item("Doc_Numero_Sin_Conf")), CDbl(DT.Rows(i).Item("Doc_Numero_Conf")), CStr(DT.Rows(i).Item("Doc_Numero_Des_Conf")))

                Tipo_Peso = DT.Rows(i).Item("Tipo_Peso")
                Peso = DT.Rows(i).Item("Peso")
                Tara_Imballi = DT.Rows(i).Item("Tara_Imballi")

                Peso_Lordo = Calcola_PesoLordo(Tipo_Peso, Peso, Tara_Imballi)

                Peso_Netto = DT.Rows(i).Item("Qta_Raccolta")
                Degrado_Perc = DT.Rows(i).Item("Variazione_Raccolta")
                Udm_Cod = DT.Rows(i).Item("Udm_Cod_Raccolta")

                objADDFun.Calcola_NettoPagamento(Peso_Netto, Degrado_Perc, Udm_Cod, Degrado, Netto_Pag)

                Campionatura = DT.Rows(i).Item("Calibro_Des")
                Punteggio = DT.Rows(i).Item("Punteggio")

                DR.Codice_Conferente = Codice_Conferente
                DR.RagSoc_Conferente = RagSoc_Conferente
                DR.Codice_Specie = Codice_Specie
                DR.Descr_Specie = Descr_Specie
                DR.Codice_Prodotto = Codice_Prodotto
                DR.Descr_Varieta = Descr_Varieta
                DR.Data_Bolla = Data_Bolla
                DR.Numero_Bolla = Numero_Bolla
                DR.Data_DDT = Data_DDT
                DR.Numero_DDT = Numero_DDT
                DR.Peso_Lordo = Peso_Lordo
                DR.Tara_Imballi = Tara_Imballi
                DR.Peso_Netto = Peso_Netto
                DR.Degrado_Perc = Degrado_Perc
                DR.Degrado = Degrado
                DR.Netto_Pag = Netto_Pag
                DR.Indice_Qualitativo = Campionatura
                DR.Punteggio = Punteggio


                DS.DS_EC_BolleAccettazione.Rows.Add(DR)
                '---------------------------

            Next


        End If 'controllo sul dt

        '####################################################

        Try

            'imposto il datset sul report
            Rpt_EC_Bolle.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

    End Sub


End Class
