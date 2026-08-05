Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class EC_Imballi_Conf
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private Rpt_EC_Imballi_Conf As Rpt_EC_Imballi_Conf

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
        Rpt_EC_Imballi_Conf = New Rpt_EC_Imballi_Conf

    End Sub

#End Region


    Dim Data_Da, Data_A As String
    Dim Codice_ConfCli As String = ""
    Dim RagSoc_ConfCli As String = ""
    Dim FiltroAggConferenti As String
    Dim Piva As String
    Dim Cod_RisUm, Sa_Cod, Fabbricato_Cod As Integer
    Dim RagSoc_Impresa As String
    Dim Cod_Rapporto As Integer = 0
    Dim Mat_Cod As Integer = 0
    Dim Mat_Des As String = ""
    Dim Cod_Articolo As String = ""
    Dim int_Configurazione_Moduli As enum_Omni_Modulo_Generazione
    Dim Dettaglio_Stabilimento As Boolean = False
    Dim Descr_Magazzino As String

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

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                            AgroKey_EncoderDecoder, _
                            Server)

        Sa_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("s")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Fabbricato_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("f")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Cod_Rapporto = CInt(Stringa_Decodifica(CStr(Request.QueryString("rc")), _
                        AgroKey_EncoderDecoder, _
                        Server))

        Dettaglio_Stabilimento = CBool(Stringa_Decodifica(CStr(Request.QueryString("ds")), _
                        AgroKey_EncoderDecoder, _
                        Server))

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
                          AgroKey_EncoderDecoder, _
                          Server))

        Mat_Des = Stringa_Decodifica(CStr(Request.QueryString("imb")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Cod_Articolo = Stringa_Decodifica(CStr(Request.QueryString("ca")), _
                                            AgroKey_EncoderDecoder, _
                                            Server)

        Descr_Magazzino = Stringa_Decodifica(CStr(Request.QueryString("mag")), _
                                                AgroKey_EncoderDecoder, _
                                                Server)

        RagSoc_Impresa = Stringa_Decodifica(CStr(Request.QueryString("rs")), _
                                           AgroKey_EncoderDecoder, _
                                           Server)

        Codice_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("cc")), _
                         AgroKey_EncoderDecoder, _
                         Server)

        RagSoc_ConfCli = Stringa_Decodifica(CStr(Request.QueryString("rsc")),
                                AgroKey_EncoderDecoder,
                                Server)

        Cod_RisUm = CInt(Stringa_Decodifica(CStr(Request.QueryString("ru")),
                                  AgroKey_EncoderDecoder,
                                  Server))

        FiltroAggConferenti = Session("Str_CodRisUm_Conferenti")


        int_Configurazione_Moduli = CInt(Stringa_Decodifica(CStr(Request.QueryString("cm")), _
                      AgroKey_EncoderDecoder, _
                      Server))


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "EstrattoConto_Imballi"
        Dim Log_Errori As String = ""

        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DS As New DS_EC_Imballi_Conf

            Try

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
                                "Codice ConfCli = " + CStr(Codice_ConfCli) + ", " + vbCrLf + _
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
                                                 "EC_Imballi_Conf.aspx", _
                                                 Log_Errori)

            End If
            '-----------------------------------------

            Try
                Session("Report") = Rpt_EC_Imballi_Conf
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If


    End Sub



    '#####################################################################################################
    Private Sub Stampa_EstrattoConto_Imballi(ByRef DS As DS_EC_Imballi_Conf, _
                                                ByRef Log_Errori As String)


        Dim DT As DataTable
        Dim i As Integer
        Dim Data_Saldo_Precedente As Date

        Dim Parametro_RagSoc_Impresa As String = ""
        Dim Parametro_Descr_Magazzino As String = ""
        Dim Parametro_Data_Da As String = ""
        Dim Parametro_Data_A As String = ""

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


        Data_Saldo_Precedente = CDate(Data_Da).AddDays(-1.0)

        Try

            Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni
            FiltroAggConferenti = objConfFun.Ricava_FiltroCodRisUm_da_StrCodiciContatto(FiltroAggConferenti)

            Dim xFiltroAggiuntivo As String = "1=1" & vbCrLf

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
            DT = objConf.EstrattoConto_Imballi(Piva,
                                               Sa_Cod,
                                               Fabbricato_Cod,
                                               Mat_Cod,
                                               Cod_RisUm,
                                               FiltroAggConferenti,
                                               Data_Da,
                                               Data_A,
                                               Data_Saldo_Precedente,
                                               Cod_Rapporto,
                                               Dettaglio_Stabilimento,
                                               xFiltroAggiuntivo,
                                               "",
                                               int_Configurazione_Moduli,
                                               objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try



        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

            Dim DR As DS_EC_Imballi_Conf.DT_EC_Imballi_ConfRow


            Dim Codice_ConfCli As String
            Dim RagSoc_ConfCli As String
            Dim Codice_Stab As String
            Dim RagSoc_Stab As String
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

            Dim Memo_Conf As String = ""
            Dim Memo_Conf_Stab As String = ""
            Dim Memo_Imballo As String = ""

            For i = 0 To DT.Rows.Count - 1

                '-----------------------------------------
                'aggiungo la riga nel dataset
                DR = DS.DT_EC_Imballi_Conf.NewDT_EC_Imballi_ConfRow

                Codice_ConfCli = DT.Rows(i).Item("Codice_ConfCli")
                RagSoc_ConfCli = DT.Rows(i).Item("RagSoc_ConfCli")
                If Dettaglio_Stabilimento Then
                    Codice_Stab = DT.Rows(i).Item("Codice_Stab")
                    RagSoc_Stab = DT.Rows(i).Item("RagSoc_Stab")
                    RagSoc_ConfCli = RagSoc_ConfCli & " - " & RagSoc_Stab
                End If
                Codice_Imballo = DT.Rows(i).Item("Cod_Articolo")
                Descr_Imballo = DT.Rows(i).Item("Mat_Des")

                DR.Codice_Conferente = Codice_ConfCli
                DR.RagSoc_Conferente = RagSoc_ConfCli
                DR.Codice_Imballo = Codice_Imballo
                DR.Descr_Imballo = Descr_Imballo

                Lav_Cod = CInt(DT.Rows(i).Item("lav_cod"))

                'escludo il record della giacenza
                If Lav_Cod <> -1 Then

                    Data_Doc = DT.Rows(i).Item("Data_Doc")

                    Numero_Doc = Ricava_NumeroDocumento_Senza_Sequenza(CStr(DT.Rows(i).Item("Doc_Numero_Sin")), CDbl(DT.Rows(i).Item("Doc_Numero")), CStr(DT.Rows(i).Item("Doc_Numero_Des")))
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

                If Not Dettaglio_Stabilimento And Memo_Conf = "" And Memo_Imballo = "" Or
                    Dettaglio_Stabilimento And Memo_Conf = "" And Memo_Conf_Stab = "" And Memo_Imballo = "" Then
                    'primo giro
                    Memo_Conf = Codice_ConfCli
                    If Dettaglio_Stabilimento Then
                        Memo_Conf_Stab = Codice_Stab
                    End If
                    Memo_Imballo = Codice_Imballo

                    Tot_Ritirati = 0
                    Tot_Resi = 0
                    Giacenza = 0
                Else
                    If Not Dettaglio_Stabilimento And Memo_Conf = Codice_ConfCli And Memo_Imballo = Codice_Imballo Or
                        Dettaglio_Stabilimento And Memo_Conf = Codice_ConfCli And Memo_Conf_Stab = Codice_Stab And Memo_Imballo = Codice_Imballo Then
                        'sempre il gruppo
                    Else
                        'cambiato l'imballo e/o il ConfCli
                        'vanno azzerati i totali

                        Giacenza_Finale = Giacenza

                        Memo_Conf = Codice_ConfCli
                        If Dettaglio_Stabilimento Then
                            Memo_Conf_Stab = Codice_Stab
                        End If
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

                DS.DT_EC_Imballi_Conf.Rows.Add(DR)
                '---------------------------

            Next


        End If 'controllo sul dt

        '####################################################

        Try

            Rpt_EC_Imballi_Conf.SetDataSource(DS)

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        Try
            Rpt_EC_Imballi_Conf.SetParameterValue("par_RagSoc_Impresa", Parametro_RagSoc_Impresa)
            Rpt_EC_Imballi_Conf.SetParameterValue("par_Descr_Magazzino", Parametro_Descr_Magazzino)
            Rpt_EC_Imballi_Conf.SetParameterValue("par_Data_Da", Parametro_Data_Da)
            Rpt_EC_Imballi_Conf.SetParameterValue("par_Data_A", Parametro_Data_A)

        Catch ex As Exception
            Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub

End Class
