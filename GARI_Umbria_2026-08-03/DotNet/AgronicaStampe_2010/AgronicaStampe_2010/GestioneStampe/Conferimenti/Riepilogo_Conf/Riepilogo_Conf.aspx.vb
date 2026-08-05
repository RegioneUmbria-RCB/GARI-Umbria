Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Riepilogo_Conf
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_Riepilogo_Conf As Rpt_Riepilogo_Conf


#Region " RIEPILOGO CONFERIMENTI "

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
        Rpt_Riepilogo_Conf = New Rpt_Riepilogo_Conf
    End Sub

#End Region


    Dim Data_Da, Data_A, Data_Giacenza As String
    Dim Piva As String
    Dim Sa_Cod, Fabbricato_Cod, Cod_RisUm As Integer
    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione
    Dim Codice_ConfCli As String = ""
    Dim RagSoc_ConfCli As String = ""
    Dim RagSoc_Impresa As String
    Dim Cod_Rapporto As Integer = 0
    Dim Mat_Cod As Integer = 0
    Dim Mat_Des As String = ""
    Dim Cod_Articolo As String = ""
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

        Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
                                          AgroKey_EncoderDecoder, _
                                          Server)

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
                          AgroKey_EncoderDecoder, _
                          Server))

        Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("dpr")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)


        RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)


        If RagSoc_Impresa = "" Then
            RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
        End If




        Codice_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                AgroKey_EncoderDecoder, _
                                Server)

        RagSoc_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("rsc")), _
                                AgroKey_EncoderDecoder, _
                                Server)

        Cod_Rapporto = CInt(Stringa_Decodifica(CStr(Request.QueryString("rc")), _
                 AgroKey_EncoderDecoder, _
                 Server))


        int_Configurazione_Moduli = CInt(Stringa_Decodifica(CStr(Request.QueryString("cm")), _
                           AgroKey_EncoderDecoder, _
                           Server))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################
        Dim Nome_Documento As String = "RiepilogoConferimenti"
        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_Riepilogo_Conf

            Try

                Log_Errori = ""

                'FUNZIONE CHE FA TUTTO
                Imposta_Dati_Stampa(DS, Log_Errori)

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
                                "Cod_risum = " + CStr(Cod_RisUm) + ", " + vbCrLf + _
                                "Piva = " + CStr(Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Sa_Cod) + ", " + vbCrLf + _
                                "Fabbricato_Cod = " + CStr(Fabbricato_Cod) + ", " + vbCrLf + _
                                "Mat_Cod = " + CStr(Mat_Cod) + ", " + vbCrLf + _
                                "Descrizione Imballo = " + CStr(Mat_Des) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Conferimento", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "Riepilogo_Conf.aspx", _
                                                 Log_Errori)

            End If




            Try
                Session("Report") = Rpt_Riepilogo_Conf
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        '==================================================================

    End Sub



    '#####################################################################################################
    Private Sub Imposta_Dati_Stampa(ByRef DS As DS_Riepilogo_Conf, _
                                    ByRef Log_Errori As String)



        Dim DT As DataTable
        Dim i As Integer
        Dim xFiltroAggiuntivo As String = ""
        Dim xOrderBy As String = ""

        '----------------------
        'Variabili per PARAMETRI da passare al report
        Dim Parametro_RagSoc_Impresa As String = ""
        Dim Parametro_Descr_Magazzino As String = ""
        Dim Parametro_Data_Da As String = ""
        Dim Parametro_Data_A As String = ""
        Dim Parametro_Data_Giacenza As String = ""
        Dim par_Configurazione_Moduli As String = ""
        Dim par_PesoNetto As String = ""
        Dim par_PesoLordo As String = ""
        Dim Totale_Lordo As Double = 0.0
        Dim Totale_Netto As Double = 0.0

        Try

            Dim objConf As New AgronicaCoreStampeDAL.ConferimentoAccettazione

            'Inizializzazione parametri
            Parametro_RagSoc_Impresa = RagSoc_Impresa
            Parametro_Descr_Magazzino = Descr_Magazzino
            If CDate(Data_Da) = AGRODATAINIZIO Then
                Parametro_Data_Da = ""
            Else
                Parametro_Data_Da = Data_Da
            End If
            If CDate(Data_A) = AGRODATAFINE Then
                Parametro_Data_A = ""
            Else
                Parametro_Data_A = Data_A
            End If

            xOrderBy = " DataOra_Ingresso, Id_Agenda, Mat_des, Lotto_Raccolta, Imballaggio, Contenitore "

            'Sa_Cod, _
            ' Fabbricato_Cod, _

            DT = objConf.RiepilogoConferimenti(Piva, _
                                               Mat_Cod, _
                                               Mat_Des, _
                                               Cod_Articolo, _
                                               Data_Da, _
                                               Data_A, _
                                               Codice_ConfCli, _
                                               RagSoc_ConfCli, _
                                               Cod_Rapporto, _
                                               xFiltroAggiuntivo, _
                                               xOrderBy, _
                                               int_Configurazione_Moduli, _
                                               objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try

            RiempiDati(DS, DT, Totale_Lordo, Totale_Netto, Log_Errori)


        Catch ex As Exception
            Log_Errori += "- Valorizzazione dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '####################################################

        Try

            'imposto il datset sul report
            Rpt_Riepilogo_Conf.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try
            Rpt_Riepilogo_Conf.SetParameterValue("par_RagSoc_Impresa", Parametro_RagSoc_Impresa)
            Rpt_Riepilogo_Conf.SetParameterValue("par_Descr_Magazzino", Parametro_Descr_Magazzino)
            Rpt_Riepilogo_Conf.SetParameterValue("par_Data_Da", Parametro_Data_Da)
            Rpt_Riepilogo_Conf.SetParameterValue("par_Data_A", Parametro_Data_A)
            Rpt_Riepilogo_Conf.SetParameterValue("par_Configurazione_Moduli", CStr(int_Configurazione_Moduli))
            Rpt_Riepilogo_Conf.SetParameterValue("par_PesoLordo", CStr(Totale_Lordo))
            Rpt_Riepilogo_Conf.SetParameterValue("par_PesoNetto", CStr(Totale_Netto))
        Catch ex As Exception
            Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub



    Private Sub RiempiDati(ByRef DS As DS_Riepilogo_Conf, ByVal DT As DataTable, ByRef Totale_Lordo As Double, ByRef Totale_Netto As Double, ByRef Log_Errori As String)

        Dim Msg_Errore As String = ""
        Dim DR As DS_Riepilogo_Conf.DT_Riepilogo_ConfRow
        Dim objMPCamp As New AgronicaCoreStampeDAL.OModuli_Referenze_Config_Dettagli_R
        '------------------

        Dim x_Data_Accettazione As Date = #1/1/1900#
        Dim x_Ora_Accettazione As DateTime
        Dim x_Doc_Numero_Sin_Accettazione As String
        Dim x_Doc_Numero_Accettazione As Integer
        Dim x_Doc_Numero_Des_Accettazione As String
        Dim Numero_Doc_Accettazione As String

        Dim x_Note As String = ""
        Dim x_DataOra_Ingresso As DateTime

        '------------------
        'Dati DDT Conf
        Dim x_Data_DDTConf As Date
        Dim x_Doc_Numero_Sin_DDTConf As String
        Dim x_Doc_Numero_DDTConf As Integer
        Dim x_Doc_Numero_Des_DDTConf As String
        Dim Numero_DDTConf, Data_DDTConf As String



        '------------------
        'ConfCli
        Dim x_Cod_RisUm As Integer
        Dim x_Rag_Soc As String

        '------------------------------
        'Movimenti Dettagli

        Dim Descrizione As String = ""
        Dim Dettagli_Prodotto As String = ""
        Dim Confezionamento_Imballi As String = ""
        Dim Confezionamento_Contenitori As String = ""
        Dim DettagliLotto As String = ""
        Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

        Dim MatDes As String
        'Dim Calibro As String
        'Dim Qualita As String
        'Dim Declassamento As String
        'Dim Marca As String
        'Dim Confezione As String
        Dim Contenitore As String
        Dim Imballaggio As String
        ' Dim Note As String

        Dim x_Qta As Double
        Dim x_Qta_Extra_Totale As Double
        Dim x_Udm_Cod_Extra As Integer
        Dim x_Tara As Double
        Dim x_Qta_Contenitori As Double = 0
        Dim x_Qta_Imballaggi As Double = 0
        Dim x_Lotto As String
        Dim x_Udm_Cod As Integer
        Dim x_Elem_Cod As Integer
        Dim x_Mat_Cod As Integer
        Dim x_Cal_Cod As Integer
        Dim x_Veg_Cod As Integer

        Dim Peso_Netto As Double
        Dim Peso_Lordo As Double
        Dim LavCod As Integer
        Dim TipoDoc As String
        Dim UdmDes As String
        Dim str_CellaStiva As String

        Dim int_ID_Agenda_new As Integer = 0
        Dim int_ID_Agenda As Integer = 0

        'Campi utilizzati per totalizzazione
        Dim wDescrizione As String = ""
        Dim wUdmDes As String = ""
        Dim wConfezionamento_Imballi As String = ""
        Dim wConfezionamento_Contenitori As String = ""
        Dim wstr_CellaStiva As String = ""
        Dim wPeso_Netto As Double
        Dim wPeso_Lordo As Double
        Dim wQta_Imballaggi As Double = 0
        Dim wQta_Contenitori As Double = 0

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Piva, objParametri_Server)

                For i = 0 To DT.Rows.Count - 1

                    'Inizializzazione variabili
                    Descrizione = ""
                    Confezionamento_Imballi = ""
                    Confezionamento_Contenitori = ""

                    With DT.Rows(i)

                        int_ID_Agenda_new = .Item("id_agenda")

                        ' Se è cambiata l'agenda devo aggiungere l'eventuale ultimo record di quella precedente
                        If (int_ID_Agenda_new <> int_ID_Agenda AndAlso int_ID_Agenda <> 0) Then

                            If Not String.IsNullOrEmpty(wDescrizione) OrElse
                                Not String.IsNullOrEmpty(wUdmDes) OrElse
                                Not String.IsNullOrEmpty(wConfezionamento_Imballi) OrElse
                                Not String.IsNullOrEmpty(wConfezionamento_Contenitori) OrElse
                                Not String.IsNullOrEmpty(wstr_CellaStiva) Then

                                'DATI DETTAGLI 
                                'Desscrizione prodotto + lotto
                                DR.Descrizione = wDescrizione

                                DR.Qta = Math.Round(wPeso_Netto, 0)
                                DR.PesoLordo = Math.Round(wPeso_Lordo)
                                DR.Udm_Des = wUdmDes

                                If wQta_Imballaggi > 0 Then
                                    DR.Confezionamento = CStr(wQta_Imballaggi) & " " & wConfezionamento_Imballi
                                Else
                                    DR.Confezionamento = ""
                                End If

                                If wQta_Contenitori > 0 Then
                                    DR.Contenitori = CStr(wQta_Contenitori) & " " & wConfezionamento_Contenitori
                                Else
                                    DR.Contenitori = ""
                                End If

                                If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                                    DR.Cella_Stiva = wstr_CellaStiva
                                End If

                                DS.DT_Riepilogo_Conf.Rows.Add(DR)

                                wUdmDes = ""
                                wDescrizione = ""
                                wConfezionamento_Imballi = ""
                                wConfezionamento_Contenitori = ""
                                wstr_CellaStiva = ""
                                wPeso_Netto = 0
                                wPeso_Lordo = 0
                                wQta_Imballaggi = 0
                                wQta_Contenitori = 0

                            End If

                        End If

                        '******************************
                        '***DATI INTESTAZIONE DOCUMENTO
                        '******************************



                        x_DataOra_Ingresso = .Item("DataOra_Ingresso")

                        x_Data_Accettazione = .Item("Data_Accett")
                        x_Ora_Accettazione = .Item("Ora_Accett")
                        x_Doc_Numero_Sin_Accettazione = .Item("Doc_Numero_Sin")
                        x_Doc_Numero_Accettazione = .Item("Doc_Numero")
                        x_Doc_Numero_Des_Accettazione = .Item("Doc_Numero_Des")



                        Numero_Doc_Accettazione = Ricava_NumeroDocumento_Senza_Sequenza( _
                                             x_Doc_Numero_Sin_Accettazione, _
                                             x_Doc_Numero_Accettazione, _
                                             x_Doc_Numero_Des_Accettazione)



                        x_Note = .Item("Note")



                        x_Data_DDTConf = .Item("Data_Conf")
                        If x_Data_DDTConf = AGRODATAINIZIO Then
                            Data_DDTConf = ""
                        Else
                            Data_DDTConf = x_Data_DDTConf
                        End If
                        x_Doc_Numero_Sin_DDTConf = .Item("Doc_Numero_Sin_Conf")
                        x_Doc_Numero_DDTConf = .Item("Doc_Numero_Conf")
                        x_Doc_Numero_Des_DDTConf = .Item("Doc_Numero_Des_Conf")

                        If x_Doc_Numero_DDTConf <> 0 Then
                            Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                    x_Doc_Numero_Sin_DDTConf, _
                                                    x_Doc_Numero_DDTConf, _
                                                    x_Doc_Numero_Des_DDTConf)
                        End If



                        '=============================================================
                        '------------------- ConfCli  ---------------------------

                        x_Cod_RisUm = .Item("Cod_RisUm")
                        If x_Cod_RisUm <> 0 Then
                            x_Rag_Soc = CStr(.Item("Rag_Soc_ConfCli"))
                            'x_Rag_Soc = CStr(.Item("Rag_Soc_ConfCli")) + CStr(.Item("Nome_ConfCli")) + " " + CStr(.Item("Cognome_ConfCli"))
                            'x_Cod_Contatto = .Item("Cod_Contatto_ConfCli")
                            'x_Codice_Fiscale = .Item("Codice_Fiscale_ConfCli")
                            'Ricava_Piva_Codicefiscale(x_Cod_Contatto, x_Codice_Fiscale, Piva_ConfCli, CodiceFiscale_ConfCli, Nothing)
                            'Piva_ConfCli = "Partita IVA: " & Piva_ConfCli
                            'CodiceFiscale_ConfCli = "Codice Fiscale: " & CodiceFiscale_ConfCli

                        End If


                        LavCod = .Item("Lav_Cod")
                        Select Case LavCod
                            Case 1054
                                TipoDoc = "Accettazione + DDT Ricevuto"
                            Case 1078
                                TipoDoc = "Accettazione + Auto DDT Emesso"
                            Case 1076
                                TipoDoc = "Accettazione + Distinta Carico"
                            Case 1025
                                TipoDoc = "DDT ricevuto"
                            Case 1077
                                TipoDoc = "Auto DDT Emesso"
                            Case 1075
                                TipoDoc = "Distinta carico"
                            Case 1052
                                TipoDoc = "Bolla c/conferimento verso diversi"

                        End Select



                        'valorizzo sul dataset i dati
                        'DrIntestazioneNew = DS.IntestazioneFattura.NewIntestazioneFatturaRow

                        'DrIntestazioneNew.SuperPiva = Piva

                        'DrIntestazioneNew.Piva = Piva_ConfCli
                        'DrIntestazioneNew.Scadenza = CodiceFiscale_ConfCli
                        'DrIntestazioneNew.VettoreRag_Soc = x_RagSoc_Vettore


                        '***************************
                        '*** DATI DETTAGLI *********
                        '***************************

                        MatDes = .Item("Mat_Des_Raccolta")

                        x_Qta = .Item("Qta")
                        'x_Qta_Extra = .Item("Qta_Extra")
                        'x_Qta_Sottoconfezioni = .Item("Qta_Sottoconfezioni")

                        '14/06/2017: ora in entrambe le query si chiamano uguali
                        'If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                        '    x_Qta_Imballaggi = .Item("Qta_Imballaggi")
                        '    Imballaggio = .Item("oimballaggio")
                        'Else
                        '    x_Qta_Imballaggi = .Item("xqtaimballaggio")
                        '    Imballaggio = .Item("ximballaggio")
                        'End If
                        x_Qta_Imballaggi = .Item("num_Imballi")
                        Imballaggio = .Item("imballaggio")

                        x_Qta_Contenitori = .Item("num_contenitori")
                        Contenitore = .Item("contenitore")

                        x_Qta_Extra_Totale = .Item("Qta_Extra_totale")
                        x_Udm_Cod_Extra = .Item("Udm_Cod_Extra")
                        x_Tara = .Item("Tara")

                        x_Udm_Cod = .Item("Udm_Cod_Raccolta")

                        x_Lotto = .Item("Lotto_raccolta")

                        x_Elem_Cod = .Item("Elem_Cod_Raccolta")
                        x_Mat_Cod = .Item("Mat_Cod_Raccolta")

                        '----------------------------------
                        'GESTIONE LOTTO PRODOTTI
                        DettagliLotto = objLotto.Gestione_LottoProdotto(Piva, x_Elem_Cod, x_Mat_Cod, x_Lotto, moduliCliente, objParametri_Server)

                        If DettagliLotto <> "" Then
                            MatDes += " " + DettagliLotto
                        End If
                        '----------------------------------

                        Descrizione = MatDes

                        '----------------------------------
                        'Recupera_Dettagli_Prodotto
                        x_Cal_Cod = .Item("Cal_Cod_Raccolta")
                        x_Veg_Cod = .Item("Veg_Cod")

                        Dettagli_Prodotto = objMPCamp.Recupera_Dettagli_Prodotto_XStampa( _
                                            Piva, x_Veg_Cod, x_Cal_Cod, _
                                            "", _
                                            objParametri_Server)

                        If Dettagli_Prodotto <> "" Then
                            Descrizione &= Dettagli_Prodotto
                        End If

                        'Calibro = .Item("ocalibro")
                        'Qualita = .Item("oqualita")
                        'Declassamento = .Item("odeclassamento")
                        'Marca = .Item("omarca")
                        'Confezione = .Item("oconfezione")
                        'SottoConfezione = .Item("osottoconfezione")
                        'Note = .Item("onote")

                        'If Declassamento <> "" Then
                        '    Descrizione &= " - " & Declassamento
                        'End If
                        'If Qualita <> "" Then
                        '    Descrizione &= " - " & Qualita
                        'End If
                        'If Calibro <> "" Then
                        '    Descrizione &= " - " & Calibro
                        'End If
                        'If Marca <> "" Then
                        '    Descrizione &= " - " & Marca
                        'End If
                        'If Note <> "" Then
                        '    Descrizione &= " - " & Note
                        'End If

                        If Imballaggio <> "" Then
                            'Confezionamento_Imballi = CStr(x_Qta_Imballaggi) & " " & Imballaggio
                            Confezionamento_Imballi = Imballaggio
                            'Qta_Imballaggi = x_Qta_Imballaggi
                        End If

                        If Contenitore <> "" Then
                            'Confezionamento_Contenitori = CStr(x_Qta_Contenitori) & " " & Contenitore
                            Confezionamento_Contenitori = Contenitore
                            'Qta_Contenitori = x_Qta_Contenitori
                        End If

                        Peso_Netto = x_Qta_Extra_Totale
                        Peso_Lordo = Peso_Netto + x_Tara
                        Totale_Lordo = Totale_Lordo + Math.Round(Peso_Lordo)
                        Totale_Netto = Totale_Netto + Math.Round(Peso_Netto)

                        UdmDes = .Item("Udm_Des")

                        If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                            str_CellaStiva = .Item("Cella_Stiva")
                        Else
                            str_CellaStiva = ""
                        End If

                        ' Se non è il primo giro per questo record di agenda ed è cambiato uno dei campi di rottura: aggiungo riga a dataTable
                        If Descrizione <> wDescrizione OrElse
                            UdmDes <> wUdmDes OrElse
                            Confezionamento_Imballi <> wConfezionamento_Imballi OrElse
                            Confezionamento_Contenitori <> wConfezionamento_Contenitori OrElse
                            str_CellaStiva <> wstr_CellaStiva Then

                            If Not String.IsNullOrEmpty(wDescrizione) OrElse
                               Not String.IsNullOrEmpty(wUdmDes) OrElse
                               Not String.IsNullOrEmpty(wConfezionamento_Imballi) OrElse
                               Not String.IsNullOrEmpty(wConfezionamento_Contenitori) OrElse
                               Not String.IsNullOrEmpty(wstr_CellaStiva) Then

                                'DATI DETTAGLI 
                                'Desscrizione prodotto + lotto
                                DR.Descrizione = wDescrizione

                                DR.Qta = Math.Round(wPeso_Netto, 0)
                                DR.PesoLordo = Math.Round(wPeso_Lordo)
                                DR.Udm_Des = wUdmDes

                                If wQta_Imballaggi > 0 Then
                                    DR.Confezionamento = CStr(wQta_Imballaggi) & " " & wConfezionamento_Imballi
                                Else
                                    DR.Confezionamento = ""
                                End If

                                If wQta_Contenitori > 0 Then
                                    DR.Contenitori = CStr(wQta_Contenitori) & " " & wConfezionamento_Contenitori
                                Else
                                    DR.Contenitori = ""
                                End If

                                If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                                    DR.Cella_Stiva = wstr_CellaStiva
                                End If

                                DS.DT_Riepilogo_Conf.Rows.Add(DR)

                                wUdmDes = ""
                                wDescrizione = ""
                                wConfezionamento_Imballi = ""
                                wConfezionamento_Contenitori = ""
                                wstr_CellaStiva = ""
                                wPeso_Netto = 0
                                wPeso_Lordo = 0
                                wQta_Imballaggi = 0
                                wQta_Contenitori = 0

                            End If

                            'Else
                            '    ' non è cambiato uno dei campi di rottura, sommo solo ai campi di totalizzazione
                            '    wPeso_Netto = wPeso_Netto + Peso_Netto
                            '    wPeso_Lordo = wPeso_Lordo + Peso_Lordo
                            '    wQta_Imballaggi = wQta_Imballaggi + x_Qta_Imballaggi
                            '    wQta_Contenitori = wQta_Contenitori + x_Qta_Contenitori

                        End If

                        '------------------------------------------
                        '------ IMPOSTAZIONE DATI NEL DATASET -----
                        '------------------------------------------

                        'DATI TESTATA
                        If (int_ID_Agenda_new <> int_ID_Agenda Or int_ID_Agenda_new = 0) Then

                            'aggiungo una nuova riga nel dataset
                            DR = DS.DT_Riepilogo_Conf.NewDT_Riepilogo_ConfRow

                            'DR.DataOra_Ingresso = Format(x_DataOra_Ingresso, "dd/MM/yyyy HH:mm")
                            DR.DataOra_Ingresso = Format(x_DataOra_Ingresso, "dd/MM/yyyy")


                            DR.NumDocAccettazione = Numero_Doc_Accettazione
                            DR.DataAccettazione = Format(x_Data_Accettazione, "dd/MM/yyyy")

                            DR.Num_DDT = Numero_DDTConf
                            DR.DataDDT = Data_DDTConf


                            Select Case LavCod
                                Case 1076
                                    DR.Num_DDT = "-"
                                    DR.DataDDT = "-"
                                Case 1025
                                    DR.NumDocAccettazione = "-"
                                    DR.DataAccettazione = "-"
                                Case 1077
                                    DR.NumDocAccettazione = "-"
                                    DR.DataAccettazione = "-"
                                Case 1075
                                    DR.Num_DDT = "-"
                                    DR.DataDDT = "-"
                                    DR.NumDocAccettazione = "-"
                                    DR.DataAccettazione = "-"
                            End Select


                            DR.TipoDoc = TipoDoc
                            DR.Rag_Soc = x_Rag_Soc
                        Else

                            If Descrizione <> wDescrizione OrElse
                                UdmDes <> wUdmDes OrElse
                                Confezionamento_Imballi <> wConfezionamento_Imballi OrElse
                                Confezionamento_Contenitori <> wConfezionamento_Contenitori OrElse
                                str_CellaStiva <> wstr_CellaStiva Then

                                'aggiungo una nuova riga nel dataset
                                DR = DS.DT_Riepilogo_Conf.NewDT_Riepilogo_ConfRow

                                DR.DataOra_Ingresso = ""

                                DR.NumDocAccettazione = ""
                                DR.DataAccettazione = ""

                                DR.Num_DDT = ""
                                DR.DataDDT = ""

                                DR.TipoDoc = ""
                                DR.Rag_Soc = ""

                            End If
                        End If


                        wDescrizione = Descrizione
                        wUdmDes = UdmDes
                        wConfezionamento_Imballi = Confezionamento_Imballi
                        wConfezionamento_Contenitori = Confezionamento_Contenitori
                        wstr_CellaStiva = str_CellaStiva
                        wPeso_Netto = wPeso_Netto + Peso_Netto
                        wPeso_Lordo = wPeso_Lordo + Peso_Lordo
                        wQta_Imballaggi = wQta_Imballaggi + x_Qta_Imballaggi
                        wQta_Contenitori = wQta_Contenitori + x_Qta_Contenitori

                        int_ID_Agenda = .Item("id_agenda")

                    End With

                Next

                ' Fine ciclo: devo aggiungere l'eventuale ultimo record
                If Not String.IsNullOrEmpty(wDescrizione) OrElse
                   Not String.IsNullOrEmpty(wUdmDes) OrElse
                   Not String.IsNullOrEmpty(wConfezionamento_Imballi) OrElse
                   Not String.IsNullOrEmpty(wConfezionamento_Contenitori) OrElse
                   Not String.IsNullOrEmpty(wstr_CellaStiva) Then

                    'DATI DETTAGLI 
                    'Desscrizione prodotto + lotto
                    DR.Descrizione = wDescrizione

                    DR.Qta = Math.Round(wPeso_Netto, 0)
                    DR.PesoLordo = Math.Round(wPeso_Lordo)
                    DR.Udm_Des = wUdmDes

                    If wQta_Imballaggi > 0 Then
                        DR.Confezionamento = CStr(wQta_Imballaggi) & " " & wConfezionamento_Imballi
                    Else
                        DR.Confezionamento = ""
                    End If

                    If wQta_Contenitori > 0 Then
                        DR.Contenitori = CStr(wQta_Contenitori) & " " & wConfezionamento_Contenitori
                    Else
                        DR.Contenitori = ""
                    End If

                    If int_Configurazione_Moduli = enum_Omni_Modulo_Generazione.FreshFood Then
                        DR.Cella_Stiva = wstr_CellaStiva
                    End If

                    DS.DT_Riepilogo_Conf.Rows.Add(DR)

                End If

            End If

        Catch ex As Exception
            Log_Errori += "- Elaborazione dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try
    End Sub

    Private Function Codice_Specie() As String
        Throw New NotImplementedException
    End Function



End Class





