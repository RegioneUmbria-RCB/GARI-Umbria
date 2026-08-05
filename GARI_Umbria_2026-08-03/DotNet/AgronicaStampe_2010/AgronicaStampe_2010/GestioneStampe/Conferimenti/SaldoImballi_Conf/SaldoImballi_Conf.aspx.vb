Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class SaldoImballi_Conf
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_SaldoImballi_Conf As Rpt_SaldoImballi_Conf


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
        Rpt_SaldoImballi_Conf = New Rpt_SaldoImballi_Conf
    End Sub

#End Region


    Dim Data_Da, Data_A, Data_Giacenza As String
    Dim Piva As String
    Dim Sa_Cod, Fabbricato_Cod, Cod_RisUm As Integer
    Dim Codice_ConfCli As String = ""
    Dim RagSoc_ConfCli As String = ""
    Dim FiltroAggConferenti As String
    Dim RagSoc_Impresa As String
    Dim Cod_Rapporto As Integer = 0
    Dim Mat_Cod As Integer = 0
    Dim Mat_Des As String = ""
    Dim Cod_Articolo As String = ""
    Dim Descr_Magazzino As String
    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione
    Dim Dettaglio_Stabilimento As Boolean = False

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Data_Da = CDate(Stringa_Decodifica(CStr(Request.QueryString("dd")),
                         AgroKey_EncoderDecoder,
                         Server)).ToShortDateString

        Data_A = CDate(Stringa_Decodifica(CStr(Request.QueryString("da")),
                         AgroKey_EncoderDecoder,
                         Server)).ToShortDateString

        Data_Giacenza = CDate(Stringa_Decodifica(CStr(Request.QueryString("dg")),
                         AgroKey_EncoderDecoder,
                         Server)).ToShortDateString

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

        Cod_Rapporto = CInt(Stringa_Decodifica(CStr(Request.QueryString("rc")), _
                      AgroKey_EncoderDecoder, _
                      Server))

        Dettaglio_Stabilimento = CBool(Stringa_Decodifica(CStr(Request.QueryString("ds")), _
                        AgroKey_EncoderDecoder, _
                        Server))

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
                          AgroKey_EncoderDecoder, _
                          Server))

        Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("imb")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)


        RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)


        Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("ru")),
                                  AgroKey_EncoderDecoder,
                                  Server))


        Codice_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                                AgroKey_EncoderDecoder, _
                                Server)

        RagSoc_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("rsc")),
                                AgroKey_EncoderDecoder,
                                Server)

        FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")



        int_Configurazione_Moduli = CInt(Stringa_Decodifica(CStr(Request.QueryString("cm")), _
                      AgroKey_EncoderDecoder, _
                      Server))



        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))



        '#################################################################################
        '#####  Genero il report
        '#################################################################################
        Dim Nome_Documento As String = "SaldoImballi"
        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_SaldoImballi_Conf

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
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Conferimento",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "SaldoImballi.aspx",
                                                 Log_Errori)

            End If




            Try
                Session("Report") = Rpt_SaldoImballi_Conf
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        '==================================================================

    End Sub



    '#####################################################################################################
    Private Sub Stampa_SaldoImballi(ByRef DS As DS_SaldoImballi_Conf, _
                                    ByRef Log_Errori As String)


        Dim DT As DataTable
        Dim i As Integer

        '----------------------
        'PARAMETRI
        Dim Parametro_RagSoc_Impresa As String = ""
        Dim Parametro_Descr_Magazzino As String = ""
        Dim Parametro_Data_Da As String = ""
        Dim Parametro_Data_A As String = ""
        Dim Parametro_Data_Giacenza As String = ""

        If RagSoc_Impresa = "" Then
            Dim objImpreseR As New AgronicaCoreAnagrafeDAL.Imprese_Read
            RagSoc_Impresa = objImpreseR.RagSoc_from_Piva(Piva, objParametri_Server)
        End If

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
        Parametro_Data_Giacenza = Data_Giacenza


        Try

            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            Dim xFiltroAggiuntivo As String = "1=1 " & vbCrLf

            '23/07/2019: non serve filtrare contemporaneamente cod_contatto e like su rag_soc (il cod_contatto è più importante)
            'modificato cmq per evitare il problema di report vuoto nel caso di contatti con lettere accentate nella rag_soc:
            'la pagina di pre stampa elimina le lettere accentate nel passaggio via querystring
            If Codice_ConfCli <> "" Then
                'xFiltroAggiuntivo = xFiltroAggiuntivo & " AND Contatti_ConfCli.Cod_Contatto = '" & Agro_SQL_SaveText(Codice_ConfCli) & "'   " + vbCrLf
                xFiltroAggiuntivo = xFiltroAggiuntivo & " AND Risorse_Umane_Conferenti.Settore_Des = '" & Agro_SQL_SaveText(Codice_ConfCli) & "'   " + vbCrLf
            Else
                If RagSoc_ConfCli <> "" Then
                    xFiltroAggiuntivo = xFiltroAggiuntivo & " AND Contatti_ConfCli.Rag_Soc LIKE '%" & Agro_SQL_SaveText(RagSoc_ConfCli) & "%'   " + vbCrLf
                End If
            End If

            'MP_Imballi.Cod_Articolo, MP_Imballi.Mat_Des 
            'If Mat_Cod <> 0 Then
            '    xFiltroAggiuntivo = xFiltroAggiuntivo & " AND MP_Imballi.Mat_Cod = " & Agro_SQL_SaveText(Mat_Cod) & "   " + vbCrLf
            'End If
            If Cod_Articolo <> "" Then
                xFiltroAggiuntivo = xFiltroAggiuntivo & " AND MP_Imballi.Cod_Articolo = '" & Agro_SQL_SaveText(Cod_Articolo) & "' " + vbCrLf
            End If
            'If Mat_Des <> "" Then
            '    xFiltroAggiuntivo = xFiltroAggiuntivo & " AND MP_Imballi.Mat_Des LIKE '%" & Agro_SQL_SaveText(Mat_Des) & "%'   " + vbCrLf
            'End If

            Dim objConf As New AgronicaCoreStampeDAL.ConferimentoAccettazione
            DT = objConf.SaldoImballi(Piva,
                                    Sa_Cod,
                                    Fabbricato_Cod,
                                    Mat_Cod,
                                    Cod_RisUm,
                                    FiltroAggConferenti,
                                    Data_Da,
                                    Data_A,
                                    Data_Giacenza,
                                    Cod_Rapporto,
                                    Dettaglio_Stabilimento,
                                    xFiltroAggiuntivo, "",
                                    int_Configurazione_Moduli,
                                    objParametri_Server)

        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                Dim DR As DS_SaldoImballi_Conf.DT_SaldoImballi_ConfRow
                Dim Codice_ConfCli As String
                Dim RagSoc_ConfCli As String
                Dim Codice_Stab As String
                Dim RagSoc_Stab As String
                Dim Codice_Imballo As String
                Dim Mat_Des As String
                Dim Totale_Entrate As Double
                Dim Totale_Uscite As Double
                Dim Differenza As Double

                For i = 0 To DT.Rows.Count - 1

                    'Modifica del 14/10/2010: è cambiata la query e le giacenze =0 sono già filtrate

                    'Giacenza_Attuale = DT.Rows(i).Item("Giacenza_Attuale")

                    ''Modifica del 25/08/2010: visualizzare solo gli imballi che hanno una giacenza attuale
                    'If Giacenza_Attuale <> 0 Then

                    '-----------------------------------------
                    'aggiungo la riga nel dataset
                    DR = DS.DT_SaldoImballi_Conf.NewDT_SaldoImballi_ConfRow

                    Codice_ConfCli = DT.Rows(i).Item("Codice_ConfCli")
                    RagSoc_ConfCli = DT.Rows(i).Item("RagSoc_ConfCli")
                    If Dettaglio_Stabilimento Then
                        Codice_Stab = DT.Rows(i).Item("Codice_Stab")
                        RagSoc_Stab = DT.Rows(i).Item("RagSoc_Stab")
                        RagSoc_ConfCli = RagSoc_ConfCli & " - " & RagSoc_Stab
                    End If
                    Codice_Imballo = DT.Rows(i).Item("Cod_Articolo")
                    Mat_Des = DT.Rows(i).Item("Mat_Des")
                    Totale_Entrate = DT.Rows(i).Item("Qta_Totale_Carichi")
                    Totale_Uscite = DT.Rows(i).Item("Qta_Totale_Scarichi")
                    Differenza = Totale_Entrate - Totale_Uscite

                    DR.Codice_Conferente = Codice_ConfCli
                    DR.RagSoc_Conferente = RagSoc_ConfCli
                    DR.Codice_Imballo = Codice_Imballo
                    DR.Descr_Imballo = Mat_Des
                    DR.Data_Saldo_Prec = ""
                    DR.Saldo_Prec = ""
                    DR.Totale_Entrate = Totale_Entrate
                    DR.Totale_Uscite = Totale_Uscite
                    DR.Differenza = Differenza
                    DR.Giacenza_Attuale = DT.Rows(i).Item("Giacenza")

                    DS.DT_SaldoImballi_Conf.Rows.Add(DR)
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
            Rpt_SaldoImballi_Conf.SetDataSource(DS)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try
            Rpt_SaldoImballi_Conf.SetParameterValue("par_RagSoc_Impresa", Parametro_RagSoc_Impresa)
            Rpt_SaldoImballi_Conf.SetParameterValue("par_Descr_Magazzino", Parametro_Descr_Magazzino)
            Rpt_SaldoImballi_Conf.SetParameterValue("par_Data_Da", Parametro_Data_Da)
            Rpt_SaldoImballi_Conf.SetParameterValue("par_Data_A", Parametro_Data_A)
            Rpt_SaldoImballi_Conf.SetParameterValue("par_Data_Giacenza", Parametro_Data_Giacenza)
            Rpt_SaldoImballi_Conf.SetParameterValue("par_Configurazione_Moduli", CStr(int_Configurazione_Moduli))


        Catch ex As Exception
            Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub

End Class
