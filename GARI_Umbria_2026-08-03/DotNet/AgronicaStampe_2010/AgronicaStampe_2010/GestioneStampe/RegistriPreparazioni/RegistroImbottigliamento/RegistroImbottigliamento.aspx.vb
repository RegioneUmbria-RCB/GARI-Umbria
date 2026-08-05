Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza

Public Class RegistroImbottigliamento
    Inherits System.Web.UI.Page


    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptImbottigliamento As Rpt_RegistroImbottigliamento
    Private DsImbottigliamento As DS_RegistroImbottigliamento

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    Dim Qs_Sa_Nome As String
    Dim Qs_Sa_Cod As String
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim QS_Report As String
    Dim QS_TipoStampa As String
    Dim QS_Anno As String
    Dim QS_Mese As String
    Dim QS_StampaIntestazione As String
    Dim QS_Flag_StampaNumeroVasca As Boolean
    Dim QS_Flag_StampaCapacitaVasca As Boolean
    Dim QS_OptGestVisualNumVascaRegImbott As enum_RegImbottigliamento_NumVasca
    Dim Log_Errori As String = ""
    'Vatiabili per gestione Categorie
    Dim flag_docg, flag_dop, flag_igp, flag_tavola As Boolean
    Dim Flag_FiltroCategoria As Boolean
    Dim Parametro_FiltroRegistro As String = ""

    Dim QS_Cod_Contatto_Terzi, Rag_Soc_CTerzi As String
    Dim QS_Gestione_Conto_Terzi As enum_RegistroContoTerzi
    Dim CodContatto_CLavoro As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " IMBOTTIGLIAMENTO"

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
        rptImbottigliamento = New Rpt_RegistroImbottigliamento
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

        Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        Qs_Sa_Nome = Stringa_Decodifica(Request.QueryString("sn").ToString, _
                                             AgroKey_EncoderDecoder, _
                                             Server)

        QS_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        QS_DataInizio = Stringa_Decodifica(Request.QueryString("di").ToString, _
                                           AgroKey_EncoderDecoder, _
                                           Server)

        QS_DataFine = Stringa_Decodifica(Request.QueryString("df").ToString, _
                                         AgroKey_EncoderDecoder, _
                                         Server)

        'riattivati il 04/10/2010

        ' 0=mensile, 1=completa
        QS_TipoStampa = Stringa_Decodifica(Request.QueryString("t").ToString, _
                                         AgroKey_EncoderDecoder, _
                                         Server)

        QS_Anno = Stringa_Decodifica(Request.QueryString("a").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)

        QS_Mese = Stringa_Decodifica(Request.QueryString("m").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)

        QS_StampaIntestazione = Stringa_Decodifica(Request.QueryString("si").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)

        QS_OptGestVisualNumVascaRegImbott = Stringa_Decodifica(Request.QueryString("onv").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)

        QS_Flag_StampaNumeroVasca = Stringa_Decodifica(Request.QueryString("fnv").ToString, _
                            AgroKey_EncoderDecoder, _
                            Server)

        QS_Flag_StampaCapacitaVasca = Stringa_Decodifica(Request.QueryString("fcv").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)


        flag_docg = Stringa_Decodifica(Request.QueryString("fdocg").ToString, _
                     AgroKey_EncoderDecoder, _
                     Server)

        flag_dop = Stringa_Decodifica(Request.QueryString("fdop").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

        flag_igp = Stringa_Decodifica(Request.QueryString("figt").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

        flag_tavola = Stringa_Decodifica(Request.QueryString("ftav").ToString, _
                           AgroKey_EncoderDecoder, _
                           Server)

        If flag_docg = True And flag_dop = True And flag_igp = True And flag_tavola = True Then
            Flag_FiltroCategoria = False
        Else
            Flag_FiltroCategoria = True
        End If


        QS_Cod_Contatto_Terzi = Stringa_Decodifica(Request.QueryString("ct").ToString, _
                              AgroKey_EncoderDecoder, _
                              Server)

        If QS_Cod_Contatto_Terzi <> "" Then
            Rag_Soc_CTerzi = " " & Stringa_Decodifica(Request.QueryString("ctd").ToString, _
                                      AgroKey_EncoderDecoder, _
                                      Server)
        Else
            Rag_Soc_CTerzi = ""
        End If

        '10/02/16: modifica x andreola: stampa di tutti i c/lav insieme
        QS_Gestione_Conto_Terzi = Stringa_Decodifica(Request.QueryString("fct").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))

        '#################################################################################
        '#####  Genero il report
        '#################################################################################



        Dim Nome_Documento As String = "Registro_Imbottigliamento"
        Log_Errori = ""
        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_RegistroImbottigliamento


        If Not Me.IsPostBack Then


            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DsImbottigliamento As New DS_RegistroImbottigliamento
            Dim DataReportInizio As Date
            Dim DataReportFine As Date

            Log_Errori = ""

            Try

                If QS_TipoStampa = 0 Then
                    'stampa mese
                    DataReportInizio = CDate("01/" & QS_Mese & "/" & QS_Anno)
                    DataReportFine = CDate(Date.DaysInMonth(CInt(QS_Anno), CInt(QS_Mese)) & "/" & QS_Mese & "/" & QS_Anno)
                Else
                    'stampa intervallo
                    DataReportInizio = CDate(QS_DataInizio)
                    DataReportFine = CDate(QS_DataFine)
                End If

                Carica_DsRegistroImbottigliamento(DsImbottigliamento, DataReportInizio, DataReportFine)

            Catch ex As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                If QS_StampaIntestazione = "1" Then
                    'CType(rptImbottigliamento.Section11.ReportObjects("TxtRagSoc3"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                    rptImbottigliamento.Section10.SectionFormat.EnableSuppress = True
                Else
                    rptImbottigliamento.Section11.SectionFormat.EnableSuppress = True
                End If

                'rptImbottigliamento.Section8.SectionFormat.EnableSuppress = True

                'sorgente dati.....
                rptImbottigliamento.SetDataSource(DsImbottigliamento)

            Catch ex As Exception
                Log_Errori += "- Assegnazione dataset: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Parametro_FiltroRegistro += Qs_Sa_Nome

            Parametro_FiltroRegistro += "   Dal: " & DataReportInizio.ToShortDateString & " al " & DataReportFine.ToShortDateString

            If Flag_FiltroCategoria = True Then

                Parametro_FiltroRegistro += "   Categoria: "
                If flag_docg = True Then
                    Parametro_FiltroRegistro += "DOCG - "
                End If
                If flag_dop = True Then
                    Parametro_FiltroRegistro += "DOP - "
                End If
                If flag_igp = True Then
                    Parametro_FiltroRegistro += "IGP - "
                End If
                If flag_tavola = True Then
                    Parametro_FiltroRegistro += "TAVOLA - "
                End If
                Parametro_FiltroRegistro = Mid(Parametro_FiltroRegistro, 1, Parametro_FiltroRegistro.Length - 2)
            End If
            If Rag_Soc_CTerzi <> "" Then
                If CodContatto_CLavoro <> "" Then
                    If CodContatto_CLavoro <> Qs_Piva Then
                        Parametro_FiltroRegistro += "      C/lavoro: " & Rag_Soc_CTerzi & " - P.Iva: " & CodContatto_CLavoro
                    Else
                        Parametro_FiltroRegistro += "      " & Rag_Soc_CTerzi & " - P.Iva: " & CodContatto_CLavoro
                    End If
                End If
            Else
                If QS_Gestione_Conto_Terzi = enum_RegistroContoTerzi.RegistroSoloContoLavoro Then
                    Parametro_FiltroRegistro += "      Tutti i C/lavoro."
                End If
            End If


            '-----------------------------------------
            '---- SETTAGGIO PARAMETRI -------------
            '-----------------------------------------
            Try
                rptImbottigliamento.SetParameterValue("RagSoc_Piva", Qs_RagSoc + " " + Qs_Piva)
                rptImbottigliamento.SetParameterValue("Filtri", Parametro_FiltroRegistro)
            Catch ex As Exception
                Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
            End Try



            '-----------------------------------------
            '---- Gestione Salvataggio PDF -----------  
            '-----------------------------------------
            Try

                Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date


                If QS_DataInizio = "" Then
                    Data_Inizio_Allegati = CDate("01/" + CStr(QS_Mese) + "/" + CStr(QS_Anno))
                    Data_Fine_Allegati = CDate("01/" + CStr(QS_Mese) + "/" + CStr(QS_Anno))
                Else
                    Data_Inizio_Allegati = QS_DataInizio
                    Data_Fine_Allegati = QS_DataFine
                End If

                IdentificazioneDocumento += "_" + Format(Data_Inizio_Allegati, "yyyy_MM_dd") + "_" + Format(Data_Fine_Allegati, "yyyy_MM_dd")


                ' leggo la sottocartella da CategorieDocumenti
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(cat_cod, "", "", objParametri_Server)

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptImbottigliamento, _
                                           cat_cod, _
                                           Sottocartella, _
                                           Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                Dim AllegatiDocumentiCod As Integer

                AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva, _
                                                                 cat_cod, _
                                                                 Nome_Documento, _
                                                                 Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf", _
                                                                 Sottocartella, _
                                                                 "", "", "", "", _
                                                                 Data_Inizio_Allegati, _
                                                                 Data_Fine_Allegati, _
                                                                 objParametri_Server)


            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'MS Salvataggio del report su file temporaneo
            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptImbottigliamento.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'MS Close e dispose di Dataset e Report
            DsImbottigliamento.Dispose()
            DsImbottigliamento = Nothing

            rptImbottigliamento.Close()
            rptImbottigliamento.Dispose()
            rptImbottigliamento = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                        "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                        "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf + _
                                        "Data Inizio = " + CStr(QS_DataInizio) + ", " + vbCrLf + _
                                        "Data Fine = " + CStr(QS_DataFine) + ", " + vbCrLf + _
                                        "Stampa Intestazione = " + CStr(QS_StampaIntestazione) + ", " + vbCrLf + _
                                        vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                        Log_Errori

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + ".txt"

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Cantine", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "RegistroImbottigliamento.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------


            '==================================================================

            'MS Eliminato passaggio report in session: Session("Report") = rptImbottigliamento
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))

        End If

    End Sub



    '###########################################################################
    'riempie il ds x il Registro imbottigliamento...
    Private Sub Carica_DsRegistroImbottigliamento(ByRef DsRegistro As DS_RegistroImbottigliamento, _
                                                    ByVal DataReportInizio As Date, _
                                                    ByVal DataReportFine As Date)

        Dim i As Integer
        Dim Destinazione_Cod As Integer
        Dim objImb As New AgronicaCoreStampeDAL.RegistriCantina
        Dim risp As Boolean

        '----------------------------------
        'GESTIONE NUMERO DI VASCA
        Dim DT_Vasche As DataTable = Nothing
        Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine
        'aggiunta il 30/04/2013
        If QS_OptGestVisualNumVascaRegImbott = enum_RegImbottigliamento_NumVasca.NumVascaParametrico Then

            Dim objVasca As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R

            Try
                'elenco vasche in cui è stato scaricato 
                DT_Vasche = objVasca.LeggiVascaVinoSfusoMovimentato_byIdAgenda( _
                                                    Qs_Piva, _
                                                    0, _
                                                    CAU_SCARICO, _
                                                    DataReportInizio, _
                                                    DataReportFine, _
                                                    "", "", _
                                                    objParametri_Server)
            Catch ex As Exception
                Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If
        '----------------------------------


        risp = objImb.RegistroImbottigliamento(DsRegistro, _
                                                DsRegistro.DS_RegistroImbottigliamento.TableName, _
                                                Qs_Piva, _
                                                QS_Report, _
                                                QS_Gestione_Conto_Terzi, _
                                                QS_Cod_Contatto_Terzi, _
                                                 flag_docg, _
                                                 flag_dop, _
                                                 flag_igp, _
                                                 flag_tavola, _
                                                DataReportInizio, _
                                                DataReportFine, _
                                                objParametri_Server, _
                                                Qs_Sa_Cod)


        If risp = True Then

            Dim strDescrizione, str_MovXRpt, DettagliVasca As String
            Dim Capacita_Effettiva As String = ""
            Dim x_Udm_Sim_Extra As String
            Dim x_Udm_Des_Extra As String
            Dim x_Udm_Cod_Extra As Integer
            Dim objMR As New AgronicaCoreContabDAL.MovimentixReport_R
            Dim Categoria_Gias_Cod As Integer
            Dim Num_idoneita, LottoLinea As String
            Dim data_imbottigliamento As Date

            'Leggo i moduli installati
            Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
            Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

            For i = 0 To DsRegistro.DS_RegistroImbottigliamento.Rows.Count - 1

                DettagliVasca = ""
                strDescrizione = ""
                str_MovXRpt = ""

                '----------------------------------
                'GESTIONE LOTTO PRODOTTI
                Dim DettagliLotto As String = ""
                Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva,
                                                                 DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Elem_Cod"),
                                                                 DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Mat_Cod"),
                                                                 DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Lotto"),
                                                                 moduliCliente,
                                                                 objParametri_Server)

                If DettagliLotto <> "" Then
                    DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Lotto") = DettagliLotto
                End If
                '----------------------------------

                Select Case QS_OptGestVisualNumVascaRegImbott

                    Case enum_RegImbottigliamento_NumVasca.Nessuna

                        '------------------------

                    Case enum_RegImbottigliamento_NumVasca.NumVascaParametrico
                        'aggiunto il 30/04/2013
                        Try

                            DettagliVasca = objCantineHLP.IdentificativoCapacitaVasca_from_IdAgenda_RegImbottigliamento( _
                                                            QS_Flag_StampaNumeroVasca, _
                                                            QS_Flag_StampaCapacitaVasca, _
                                                            DT_Vasche, _
                                                            DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Id_agenda"))
                            'If DettagliVasca <> "" Then
                            '    DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Mat_Des") += DettagliVasca
                            'End If
                            strDescrizione = DettagliVasca

                        Catch ex As Exception
                            Log_Errori += "- Lettura DettagliVasca: " + vbCrLf + ex.Message + vbCrLf
                        End Try

                        '------------------------

                    Case enum_RegImbottigliamento_NumVasca.CampoNoteLibero

                        Try

                            If DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("StrCampo_Registri") <> String.Empty Then

                                'str_MovXRpt = Descrizione_From_MovimentixReport(Server, Session, Page, _
                                '                                                   Qs_Piva, Qs_Sa_Cod, _
                                '                                                   DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Id_Agenda"), _
                                '                                                   QS_Report)

                                str_MovXRpt = objMR.Descrizione_from_MovimentixReport(Qs_Piva, _
                                                                                        0, _
                                                                                        DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Id_Agenda"), _
                                                                                        QS_Report, _
                                                                                        "", objParametri_Server)

                                strDescrizione = " (" & DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("StrCampo_Registri") & " " & str_MovXRpt & ")"

                            End If

                        Catch ex As Exception
                            Log_Errori += "- Lettura Descrizione_from_MovimentixReport: " + vbCrLf + ex.Message + vbCrLf
                        End Try

                End Select


                Categoria_Gias_Cod = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Categoria_Gias_Cod")

                LottoLinea = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Trasformazione_Des")

                'nel caso di vino DOCG o DOP, leggo il numero di idoenità
                Select Case Categoria_Gias_Cod
                    Case enum_Omni_Gradi_Liberta.GL2_Docg, enum_Omni_Gradi_Liberta.GL2_Dop

                        data_imbottigliamento = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Data_Movimento")

                        Num_idoneita = objMR.NumeroIdoneita_Classificazioni_from_Lotto(Qs_Piva, LottoLinea, data_imbottigliamento, "", objParametri_Server)
                        If Num_idoneita <> "" Then
                            strDescrizione += " - Num. Idoneità: " & Num_idoneita
                        End If
                End Select


                If IsNumeric(DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("CodContatto_CLavoro")) Then
                    If CLng(DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("CodContatto_CLavoro")) < 0 Then
                        CodContatto_CLavoro = ""
                    Else
                        CodContatto_CLavoro = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("CodContatto_CLavoro")
                    End If
                Else
                    CodContatto_CLavoro = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("CodContatto_CLavoro")
                End If

                Select Case QS_Gestione_Conto_Terzi
                    Case enum_RegistroContoTerzi.RegistroSoloContoLavoro
                        If Trim(DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("RagSoc_CLavoro")) <> "" Then
                            strDescrizione += " - C/lavoro: " & Trim(DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("RagSoc_CLavoro")) & " - " & CodContatto_CLavoro
                        End If
                End Select

                If strDescrizione <> "" Then
                    'Modifico la descrizione del movimento
                    DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Des_Lib") = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Des_Lib") & strDescrizione
                End If

                '----------------------------------
                'GESTIONE CAPACITA' EFFETTIVA  
                Capacita_Effettiva = ""
                'se peso_set > 0 -> allora capacità effettiva (lettura dei campi di movimenti_dettagli)
                If DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Peso_Set") > 0 Then

                    x_Udm_Cod_Extra = DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("udm_cod_extra")

                    Select Case x_Udm_Cod_Extra
                        Case enum_UnitaMisura.KG
                            x_Udm_Sim_Extra = "kg"
                        Case enum_UnitaMisura.Litri
                            x_Udm_Sim_Extra = "l"
                        Case Else

                            'x_Udm_Des_Extra = UdmDes_from_UdmCod(Server, Session, Page, x_Udm_Cod_Extra, x_Udm_Sim_Extra)

                            Dim udmcore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                            x_Udm_Des_Extra = udmcore.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametri_Server)


                    End Select

                    Capacita_Effettiva = CStr(DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("qta_extra")) + " " + x_Udm_Sim_Extra

                End If
                If Capacita_Effettiva <> "" Then
                    DsRegistro.DS_RegistroImbottigliamento.Rows(i).Item("Mat_Des") += " " + Capacita_Effettiva
                End If
                '----------------------------------

            Next

        End If


    End Sub


End Class
