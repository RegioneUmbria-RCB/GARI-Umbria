Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class RegistroVinificazione
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptIntestazione As Rpt_RegistroVinificazioneIntestazione        ' report contenitore

    Private rptVinificazione As Rpt_RegistroVinificazione
    Private DsVinificazione As DS_RegistroVinificazione

    Private rptRiepilogo As Rpt_RiepilogoVinificazione
    Private DsRiepilogoVinificazione As DS_RiepilogoVinificazione

    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    Dim Qs_Sa_Cod, Qs_Id_Destinazione As Integer
    Dim Qs_Cal_Cod, Qs_Mat_Cod, QS_Linea_Cod As Integer
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    Dim QS_Report As String
    Dim QS_TipoStampa As String
    Dim QS_Anno As String
    Dim QS_Mese As String
    Dim QS_StampaIntestazione As String
    Dim QS_StampaRiporti As String
    Dim QS_Cod_Contatto_Terzi, Rag_Soc_CTerzi As String
    Dim QS_Filtro_Stampa As String
    Dim QS_Cau_Mov As String
    Dim Qs_Flag_VerificaRegistri As Boolean
    Dim QS_Flag_StampaNumeroVasca As Boolean
    Dim QS_Flag_StampaCapacitaVasca As Boolean
    Dim QS_Gestione_Conto_Terzi As enum_RegistroContoTerzi
    Dim QS_Flag_StampaLottoTrasformazione As Boolean
    '  Dim QS_Flag_GestioneRegistroVinificazione As Integer
    ' Dim Opt_Gestione_RegistroVinificazione As Integer
    Dim flag_docg, flag_dop, flag_igp, flag_tavola As Boolean
    Dim Flag_FiltroCategoria As Boolean
    Dim Parametro_FiltroRegistro As String = ""

    Dim Log_Errori As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " VINIFICAZIONE "

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

        'istanzio gli oggetti report
        rptVinificazione = New Rpt_RegistroVinificazione
        rptRiepilogo = New Rpt_RiepilogoVinificazione
        rptIntestazione = New Rpt_RegistroVinificazioneIntestazione

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



        If Not IsNothing(Request.QueryString("dest")) Then
            Qs_Id_Destinazione = Stringa_Decodifica(Request.QueryString("dest").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            Qs_Id_Destinazione = 0
        End If

        If Not IsNothing(Request.QueryString("cc")) Then
            Qs_Cal_Cod = Stringa_Decodifica(Request.QueryString("cc").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            Qs_Cal_Cod = 0
        End If

        If Not IsNothing(Request.QueryString("mc")) Then
            Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mc").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            Qs_Mat_Cod = 0
        End If

        If Not IsNothing(Request.QueryString("lc")) Then
            QS_Linea_Cod = Stringa_Decodifica(Request.QueryString("lc").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            QS_Linea_Cod = 0
        End If

        If Not IsNothing(Request.QueryString("fs")) Then
            QS_Filtro_Stampa = Stringa_Decodifica(Request.QueryString("fs").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            QS_Filtro_Stampa = 0
        End If

        If Not IsNothing(Request.QueryString("fvr")) Then
            Qs_Flag_VerificaRegistri = Stringa_Decodifica(Request.QueryString("fvr").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            Qs_Flag_VerificaRegistri = False
        End If

        QS_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
                                       AgroKey_EncoderDecoder, _
                                       Server)

        'riattivati il 04/10/2010
        QS_DataInizio = Stringa_Decodifica(Request.QueryString("di").ToString, _
                                           AgroKey_EncoderDecoder, _
                                           Server)

        QS_DataFine = Stringa_Decodifica(Request.QueryString("df").ToString, _
                                         AgroKey_EncoderDecoder, _
                                         Server)

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

        QS_StampaRiporti = Stringa_Decodifica(Request.QueryString("sr").ToString, _
                                              AgroKey_EncoderDecoder, _
                                              Server)

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

        'aggiunto il 03/06/2014 x stampa registro unico, ma voci diversificate x c/terzi
        QS_Gestione_Conto_Terzi = Stringa_Decodifica(Request.QueryString("fct").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        QS_Flag_StampaNumeroVasca = Stringa_Decodifica(Request.QueryString("fnv").ToString, _
                            AgroKey_EncoderDecoder, _
                            Server)

        QS_Flag_StampaCapacitaVasca = Stringa_Decodifica(Request.QueryString("fcv").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        QS_Flag_StampaLottoTrasformazione = Stringa_Decodifica(Request.QueryString("flt").ToString, _
                                        AgroKey_EncoderDecoder, _
                                        Server)


        QS_Cau_Mov = Stringa_Decodifica(Request.QueryString("cm").ToString, _
                              AgroKey_EncoderDecoder, _
                              Server)

 

        Flag_Docg = Stringa_Decodifica(Request.QueryString("fdocg").ToString, _
                             AgroKey_EncoderDecoder, _
                             Server)

        Flag_Dop = Stringa_Decodifica(Request.QueryString("fdop").ToString, _
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


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))


        '22/03/2017: aumentato il timeout
        objParametri_Server.TimeoutQuery = 3600

        '#################################################################################
        '#####  Genero il report
        '#################################################################################


        Dim Nome_Documento As String = "Registro_Vinificazione"

        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_RegistroVinificazione


        If Not Me.IsPostBack Then


            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DsVinificazione As New DS_RegistroVinificazione
            Dim DsRiepilogo As New DS_RiepilogoVinificazione


            '--------------------------------------------
            ' Registro_Vinificazione... Start
            '--------------------------------------------
            Registro_Vinificazione_Start(DsVinificazione, DsRiepilogo)


            Try

                If QS_StampaIntestazione = "1" Then

                    'toglie sezione vuota
                    rptIntestazione.Section2.SectionFormat.EnableSuppress = True

                    CType(rptIntestazione.Section12.ReportObjects("TxtRagSoc1"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva

                Else
 
                    'toglie sezione col titolo
                    rptIntestazione.Section12.SectionFormat.EnableSuppress = True
                End If


                ' carico i sottoreport
                rptIntestazione.OpenSubreport("Rpt_RegistroVinificazione.rpt").SetDataSource(DsVinificazione)
                rptIntestazione.OpenSubreport("Rpt_RiepilogoVinificazione.rpt").SetDataSource(DsRiepilogo)

            Catch ex As Exception
                Log_Errori += "- Open sottoreport: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- SETTAGGIO PARAMETRI -------------
            '-----------------------------------------

            Try

                rptIntestazione.SetParameterValue("FiltroRegistro", Parametro_FiltroRegistro)
  

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
                    Data_Fine_Allegati = CDate(Right("00" & CStr(Date.DaysInMonth(QS_Anno, QS_Mese)), 2) + "/" + CStr(QS_Mese) + "/" + CStr(QS_Anno))
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
                objGestFile.SalvaReportPdf(rptIntestazione, _
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
                rptIntestazione.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'MS Close e dispose di Dataset e Report
            DsVinificazione.Dispose()
            DsVinificazione = Nothing

            DsRiepilogo.Dispose()
            DsRiepilogo = Nothing

            rptVinificazione.Close()
            rptVinificazione.Dispose()
            rptVinificazione = Nothing

            rptRiepilogo.Close()
            rptRiepilogo.Dispose()
            rptRiepilogo = Nothing

            rptIntestazione.Close()
            rptIntestazione.Dispose()
            rptIntestazione = Nothing

            GC.Collect()


            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                                "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                                "Sa_Cod = " + CStr(Qs_Sa_Cod) + ", " + vbCrLf + _
                                "Anno = " + CStr(QS_Anno) + ", " + vbCrLf + _
                                "Mese = " + CStr(QS_Mese) + ", " + vbCrLf + _
                                "C/lavoro = " + CStr(Rag_Soc_CTerzi) + ", " + CStr(QS_Cod_Contatto_Terzi) + ", " + vbCrLf + _
                                "Modalità stampa c/lavoro = " + CStr(QS_Gestione_Conto_Terzi) + ", " + vbCrLf + _
                                vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                                Log_Errori

                If Rag_Soc_CTerzi <> "" Then
                    Nome_File = "Log_Errori_" + Nome_Documento + "_clavoro." + Left(Rag_Soc_CTerzi, 20) + "_anno" & QS_Anno + "_mese" + QS_Mese & ".txt"
                Else
                    Nome_File = "Log_Errori_" + Nome_Documento + "_" + Qs_RagSoc + "_anno" & QS_Anno + "_mese" + QS_Mese & ".txt"
                End If

                'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_Cantine", Path_Errore, Str_Errore_Path)
                'If Str_Errore_Path = "" And Path_Errore <> "" Then
                '    GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
                'End If

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "RegistroVinificazione.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------


            Try
                'MS eliminato passaggio in session del report: Session("Report") = rptIntestazione
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server))
            Catch ex As Exception
                Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
            End Try


        End If

        '==================================================================


    End Sub


    '###########################################################################
    Private Sub Registro_Vinificazione_Start(ByRef DsVinificazione As DS_RegistroVinificazione, _
                                            ByRef DsRiepilogo As DS_RiepilogoVinificazione)

        Dim strLogRiepilogo As String

        Try

            ' If Qs_Id_Destinazione <> 0 Or Qs_Cal_Cod <> 0 Or Qs_Mat_Cod <> 0 Then
            If Qs_Flag_VerificaRegistri = True Then
                'sono nella stampa di verifica, non quella ufficiale
                CType(rptIntestazione.Section10.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_Filtro_Stampa
                rptIntestazione.Section10.SectionFormat.EnableSuppress = False
            End If


            Dim DataInizio As Date
            Dim DataFine As Date
            Dim DataReportInizio As Date
            Dim DataReportFine As Date
            Dim DataTreMesiPrima As Date

            If QS_TipoStampa = 0 Then
                'stampa mese
                DataReportInizio = CDate("01/" & QS_Mese & "/" & QS_Anno)
                DataReportFine = CDate(Date.DaysInMonth(CInt(QS_Anno), CInt(QS_Mese)) & "/" & QS_Mese & "/" & QS_Anno)

                'tolgo 2 mesi
                DataTreMesiPrima = DateAdd(DateInterval.Month, -2, CDate("01/" & QS_Mese & "/" & QS_Anno))
                'tolgo 1 giorno
                DataTreMesiPrima = DateAdd(DateInterval.Day, -1, DataTreMesiPrima)
            Else
                'stampa intervallo
                DataReportInizio = CDate(QS_DataInizio)
                DataReportFine = CDate(QS_DataFine)

                'tolgo 2 mesi
                DataTreMesiPrima = DateAdd(DateInterval.Month, -2, DataReportInizio)
                'tolgo 1 giorno
                DataTreMesiPrima = DateAdd(DateInterval.Day, -1, DataTreMesiPrima)
            End If

            'sono usate dai riporti e dai riepiloghi (x le giacenze)
            DataInizio = AGRODATAINIZIO
            DataFine = DataReportFine


            Dim Lista_PrepCod As String = ""
            Dim objLineePrep As New AgronicaCoreContabDAL.Linee_Preparazioni_R
            Lista_PrepCod = objLineePrep.Lista_PreparazioneCod_xQuery(Qs_Piva, _
                                                                    enum_Omni_Modulo_Generazione.Cantine, _
                                                                    enum_Omni_Preparazione_Cod.Passaggio_RegVinificazione_Commercializzazione, _
                                                                    "", "", _
                                                                    objParametri_Server)



            Dim Lista_CodRisUm As String = ""
            Dim Lista_IdTrasf_NoComm As String = ""

            Select Case QS_Gestione_Conto_Terzi
                Case enum_RegistroContoTerzi.Nessuno, _
                   enum_RegistroContoTerzi.RegistroGlobale, _
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = reg c/terzi separato
                    If QS_Cod_Contatto_Terzi <> "" And QS_Cod_Contatto_Terzi <> Qs_Piva Then

                        '--- Se sono nella gestione C/Terzi e non ho selezionato l'impresa stessa
                        '-> devo recuperare l'elenco dei cod_risum del contatto (per filtrare la parte di agenda della query)
                        Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
                        Lista_CodRisUm = objRisUm.Lista_CodRisUm_ByChiaveContatto(Qs_Piva, _
                                                                                QS_Cod_Contatto_Terzi, _
                                                                                "", _
                                                                                True, _
                                                                                "", _
                                                                                objParametri_Server)
                    End If

                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    Dim objRisUm As New AgronicaCoreContabDAL.Linee_Produzioni_R
                    Lista_CodRisUm = objRisUm.Lista_CodRisUm_ByCodContattoTerzi(Qs_Piva, _
                                                                                "", _
                                                                                "", _
                                                                                objParametri_Server)
            End Select


            Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina
            Lista_IdTrasf_NoComm = objStampe.Lista_IdTrasformazione_Lotti_NonPassatiACommercializzazione( _
                                                                    Qs_Piva, _
                                                                    Lista_PrepCod, _
                                                                    "", _
                                                                    objParametri_Server)


            Dim str_idagenda_filtrocategoria As String = ""
            Dim str_matcod_filtrocategoria As String = ""
            Dim str_lotto_filtrocategoria As String = ""

            If Flag_FiltroCategoria = True Then

                Parametro_FiltroRegistro = "Registro "
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

                objStampe.PreFiltro_RegistroVinificazione_xCategoria(Qs_Piva, _
                                                                        QS_Report, _
                                                                        DataReportInizio, _
                                                                        DataReportFine, _
                                                                        flag_docg, _
                                                                        flag_dop, _
                                                                        flag_igp, _
                                                                        flag_tavola, _
                                                                        str_idagenda_filtrocategoria, _
                                                                        str_matcod_filtrocategoria, _
                                                                        str_lotto_filtrocategoria, _
                                                                        objParametri_Server)

            End If


            Dim objHT_Cali As New Hashtable

            'se è attiva la stampa del registro per categoria
            'e la query di pre-filtro non ha prodotto risultato
            'significa che non ci sono dati e quindi non devo neanche chiamare le due query dei registri
            If Flag_FiltroCategoria = True And str_idagenda_filtrocategoria = "" And str_matcod_filtrocategoria = "" Then
                Exit Sub
            End If

            '====================================================
            '=== REGISTRO VINIFICAZIONE =======
            'DataInizio, DataFine, _
            Carica_DsRegistroVinificazione(DsVinificazione, _
                                            DataReportInizio, DataReportFine, _
                                            Lista_CodRisUm, _
                                            Lista_PrepCod, _
                                            Lista_IdTrasf_NoComm, _
                                            str_idagenda_filtrocategoria, _
                                            str_matcod_filtrocategoria, _
                                            str_lotto_filtrocategoria, _
                                            objHT_Cali)


            '====================================================
            '=== RIEPILOGO REGISTRO VINIFICAZIONE =======

            Select Case QS_Gestione_Conto_Terzi

                Case enum_RegistroContoTerzi.Nessuno, _
                   enum_RegistroContoTerzi.RegistroGlobale, _
                         enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
                    '0 = se non c'è esiste conto terzi
                    '1 = se esiste, ma è stato richiesto registro unico
                    '3 = questa select va bene anche nel reg c/terzi separato
                    strLogRiepilogo = Carica_DsRiepilogoVinificazione(DsRiepilogo, _
                                                                        DataInizio, DataFine, _
                                                                        DataReportInizio, DataReportFine, DataTreMesiPrima, _
                                                                        Lista_CodRisUm, _
                                                                        Lista_PrepCod, _
                                                                        Lista_IdTrasf_NoComm, _
                                                                        str_idagenda_filtrocategoria, _
                                                                        str_matcod_filtrocategoria, _
                                                                        str_lotto_filtrocategoria, _
                                                                        objHT_Cali)

                Case enum_RegistroContoTerzi.RegistroUnicoDiversificato
                    '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
                    strLogRiepilogo = Carica_DsRiepilogoVinificazione_ContoLavorazioneUnificato( _
                                                                DsRiepilogo, _
                                                               DataInizio, DataFine, _
                                                               DataReportInizio, DataReportFine, DataTreMesiPrima, _
                                                               Lista_CodRisUm, _
                                                               Lista_PrepCod, _
                                                               Lista_IdTrasf_NoComm, _
                                                               str_idagenda_filtrocategoria, _
                                                                str_matcod_filtrocategoria, _
                                                                str_lotto_filtrocategoria, _
                                                                objHT_Cali)

            End Select


            If strLogRiepilogo <> String.Empty Then
                Log_Errori += "- Riepilogo: " + vbCrLf + strLogRiepilogo + vbCrLf
            End If

        Catch ex As Exception
            Log_Errori += "- PageLoad: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub



    '###########################################################################
    'riempie il ds con i riporti x il Registro di vinificazione ...
    Private Sub Carica_DsRiportiVinificazione(ByRef DsRegistro As DS_RegistroVinificazione, _
                                                ByVal DataInizio As Date, _
                                                ByVal DataFine As Date, _
                                                ByVal DataReportInizio As Date, _
                                                ByVal DataReportFine As Date)

        ''Dim strErr As String
        ''Dim stbQ As New System.Text.StringBuilder
        'Dim Dt As DataTable
        ''Dim objSql As New Codex_Utility.Sql
        'Dim Riga As DS_RegistroVinificazione.DS_RegistroVinificazioneRow

        'Dim i As Integer
        'Dim CalDes As String = String.Empty
        'Dim CalDes_Old As String = String.Empty

        ''Dim CaricoKg As Double = 0.0
        ''Dim ScaricoKg As Double = 0.0
        ''Dim CaricoLt As Double = 0.0
        ''Dim ScaricoLt As Double = 0.0

        'Dim AddRiga As Boolean

        'Dim AnnoProduzione As String = String.Empty
        'Dim AnnoProduzione_Old As String = String.Empty

        'Dim AliasLotto As String = String.Empty

        ''      Dt = objSql.SqlSelect(objParametri_Server.StringaConnessione, Session("ASG_Connessione_Server"), stbQ.ToString, 1, strErr)

        'Dim objRiporti As New AgronicaCoreStampeDAL.RegistriCantina

        'Dt = objRiporti.RegistroVinificazioneRiporti(Qs_Piva, _
        '                                                QS_Report, _
        '                                                DataInizio, _
        '                                                DataFine, _
        '                                                DataReportInizio, _
        '                                                DataReportFine, _
        '                                                objParametri_Server)

        'If Not IsNothing(Dt) Then

        '    For i = 0 To Dt.Rows.Count - 1

        '        If Dt.Rows(i).Item("Cifra_Start") = 0 And Dt.Rows(i).Item("Cifra_End") = 0 Then
        '            AnnoProduzione = ""
        '        Else
        '            AnnoProduzione = CStr(Dt.Rows(i).Item("Lotto")).Substring(Dt.Rows(i).Item("Cifra_Start") - 1, (CInt(Dt.Rows(i).Item("Cifra_End")) - CInt(Dt.Rows(i).Item("Cifra_Start")) + 1))
        '        End If
        '        CalDes = CStr(Dt.Rows(i).Item("Cal_Des"))

        '        If CalDes <> CalDes_Old Or AnnoProduzione <> AnnoProduzione_Old Then

        '            ' aggiungo la riga vechia
        '            If Not IsNothing(Riga) Then
        '                DsRegistro.DS_RegistroVinificazione.AddDS_RegistroVinificazioneRow(Riga)
        '                ' Riga.Delete()   'elimino la riga prima di crearne una nuova
        '            End If

        '            Riga = DsRegistro.DS_RegistroVinificazione.NewDS_RegistroVinificazioneRow

        '            ' inizializzo i valori della riga
        '            Riga.Descrizione = "Riporti"
        '            Riga.CaricoKg = 0
        '            Riga.CaricoLt = 0
        '            Riga.ScaricoLt = 0
        '            Riga.ScaricoKg = 0
        '            Riga.Cau_Mov = 0
        '            Riga.Cod_Articolo = ""
        '            Riga.Data_Movimento = "01/01/1900"
        '            Riga.des_lib = ""
        '            Riga.Designazione = ""
        '            Riga.Elem_Cod = 0
        '            Riga.Id_Agenda = 0
        '            'Riga.Lotto = ""
        '            Riga.Mat_Cod = 0
        '            Riga.Mat_Des = ""
        '            Riga.Mov_Desc = ""
        '            Riga.Mov_Det_Des = ""
        '            Riga.NDoc = ""
        '            Riga.Udm_Cod = 0


        '            AliasLotto = String.Empty
        '            Dim dtAlias As DataTable
        '            dtAlias = NewCom_LottoConfigurazioneAlias_Leggi(Server, Session, Page, Session("ASG_SuperUser_CodFiscale"), Qs_Piva, _
        '                                                  Dt.Rows(i).Item("Elem_Cod"), _
        '                                                  Dt.Rows(i).Item("Lotto_Cod"), _
        '                                                  AnnoProduzione, DataInizio, DataFine)

        '            If dtAlias.Rows.Count = 1 Then
        '                AliasLotto = dtAlias.Rows(0).Item("Lotto_ALias")
        '            End If
        '            dtAlias = Nothing

        '        End If

        '        Riga.CaricoLt += Dt.Rows(i).Item("CaricoLt")
        '        Riga.ScaricoLt += Dt.Rows(i).Item("ScaricoLt")
        '        Riga.CaricoKg += Dt.Rows(i).Item("CaricoKg")
        '        Riga.ScaricoKg += Dt.Rows(i).Item("ScaricoKg")

        '        Dim strAnnata As String = String.Empty
        '        If AnnoProduzione <> String.Empty Or AliasLotto <> String.Empty Then
        '            strAnnata = IIf(AliasLotto = String.Empty, AnnoProduzione, AliasLotto)
        '        End If

        '        Riga.Mat_Des = Dt.Rows(i).Item("Cal_Des") & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata

        '        CalDes_Old = CalDes
        '        AnnoProduzione_Old = AnnoProduzione

        '        If i = Dt.Rows.Count - 1 Then
        '            DsRegistro.DS_RegistroVinificazione.AddDS_RegistroVinificazioneRow(Riga)
        '        End If

        '    Next

        'End If

    End Sub



    '###########################################################################
    'elenco movimenti per reg. di vinificazione
    Private Sub Carica_DsRegistroVinificazione(ByRef DsRegistro As DS_RegistroVinificazione, _
                                                ByVal DataReportInizio As Date, _
                                                ByVal DataReportFine As Date, _
                                                ByVal Lista_CodRisUm As String, _
                                                ByVal Lista_PrepCod As String, _
                                                ByVal Lista_IdTrasf_NoComm As String, _
                                                ByVal str_idagenda_filtrocategoria As String, _
                                                ByVal str_matcod_filtrocategoria As String, _
                                                ByVal str_lotto_filtrocategoria As String, _
                                                ByRef objHT_Cali As Hashtable)

        ' ByVal Opt_Gestione_RegistroVinificazione As Integer, _
        'ByVal DataInizio As Date, _
        'ByVal DataFine As Date, _

        Dim i As Integer
        Dim RigaDs As DS_RegistroVinificazione.DS_RegistroVinificazioneRow
        Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
        Dim DT_Vasche As DataTable
        Dim objVasca As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R
        Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine
        Dim Flag_AggiungiRiga As Boolean
        Dim debug As Boolean
        Dim descrizione As String

        Try

            ''------------------------------------------------
            '' RIPORTI DELLE TRASFORMAZIONI
            ''------------------------------------------------
            'If QS_StampaRiporti = "1" Then
            '    Carica_DsRiportiVinificazione(DsRegistro, DataInizio, DataFine, DataReportInizio, DataReportFine)
            'End If

            '----------------------------------
            'GESTIONE NUMERO DI VASCA
            'aggiunto il 15/10/2012
            Try

                DT_Vasche = objVasca.LeggiJoinUdm(Qs_Piva, _
                                                    0, 0, 0, _
                                                    "", "", _
                                                    objParametri_Server)

            Catch ex As Exception
                Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
            End Try
            '----------------------------------

            '----------------------------------
            'modifica del 13/10/2014
            'GESTIONE MOVIMENTIXREPORT (NUM IDONEITA')
            Dim DT_MovXReport As DataTable = Nothing
            Dim objMR As New AgronicaCoreContabDAL.MovimentixReport_R

            Try
                Dim DataFineControllo As Date
                DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)

                Dim filtroMR As String = " ( Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + _
                                            " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " ) " + _
                                            " AND MovimentixReport.Descrizione <> '' "

                DT_MovXReport = objMR.LeggiJOINLineePreparazionixReport( _
                                                        Qs_Piva, _
                                                        0, 0, QS_Report, _
                                                        filtroMR, _
                                                        "", _
                                                        objParametri_Server)
            Catch ex As Exception
                Log_Errori += "- Lettura di MovimentiXReport: " + vbCrLf + ex.Message + vbCrLf
            End Try
            '----------------------------------


            Dim objVinificazione As New AgronicaCoreStampeDAL.RegistriCantina
            'Dim risp As Boolean
            Dim Dt_Vin As DataTable

            Dt_Vin = objVinificazione.RegistroVinificazione(DsRegistro, _
                                                            DsRegistro.DS_RegistroVinificazione.TableName, _
                                                            Qs_Piva, _
                                                            QS_Report, _
                                                            DataReportInizio, _
                                                            DataReportFine, _
                                                            QS_Cod_Contatto_Terzi, _
                                                             Lista_CodRisUm, _
                                                            Lista_PrepCod, _
                                                            Lista_IdTrasf_NoComm, _
                                                            Qs_Sa_Cod, _
                                                            Qs_Id_Destinazione, _
                                                            Qs_Cal_Cod, _
                                                            Qs_Mat_Cod, _
                                                            QS_Linea_Cod, _
                                                            QS_Cau_Mov, _
                                                            QS_Gestione_Conto_Terzi, _
                                                            str_idagenda_filtrocategoria, _
                                                            str_matcod_filtrocategoria, _
                                                            str_lotto_filtrocategoria, _
                                                            objParametri_Server)


            'If risp = True Then
            If Not IsNothing(Dt_Vin) AndAlso Dt_Vin.Rows.Count Then

                Dim strDescrizione As String = ""
                Dim DettagliLotto As String = ""
                Dim DettagliVasca As String = ""

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                'ordinamento per
                'Movimenti.Ora, Agenda.Id_Agenda, Movimenti.Cau_Mov DESC, Movimenti_dettagli.Elem_Cod DESC
                ' For i = 0 To DsRegistro.DS_RegistroVinificazione.Rows.Count - 1
                For i = 0 To Dt_Vin.Rows.Count - 1

                    'ad ogni giro azzero, di default devo inserire
                    Flag_AggiungiRiga = True

                    '15/10/14: verifica cali -> modificato il 14/07/2015 per gestire anche le altre materie prime
                    If Dt_Vin.Rows(i).Item("elem_Cod") = CALI_LAVORAZIONE Or Dt_Vin.Rows(i).Item("elem_Cod") = ALTRE_MATERIE Then
                        'verifico se questa riga di calo o altra materia deve essere stampata
                        'magari no, perchè il vino è in vinificazione e quindi è già stato escluso dalla query
                        '(il calo non ha la gestione del passaggioa registro e quindi non viene mai scartato,
                        'verifico qui da codice)
                        Flag_AggiungiRiga = objCantineHLP.Verifica_CaloPerdita_Movimenti(Dt_Vin, Dt_Vin.Rows(i).Item("id_agenda"), objHT_Cali, enum_AgroReportistica.Vinificazione_DOC)
                    End If


                    If Flag_AggiungiRiga = True Then

                        If Not IsDBNull(Dt_Vin.Rows(i).Item("StrCampo_Registri")) AndAlso _
                           Dt_Vin.Rows(i).Item("StrCampo_Registri") <> String.Empty Then

                            'strDescrizione = objMR.Descrizione_from_MovimentixReport(Qs_Piva, _
                            '                                                   0, _
                            '                                                   Dt_Vin.Rows(i).Item("Id_Agenda"), _
                            '                                                   QS_Report, _
                            '                                                   "", objParametri_Server)

                            strDescrizione = objCantineHLP.DescMovXReport_from_IdAgenda(DT_MovXReport, Dt_Vin.Rows(i).Item("Id_Agenda"))

                            'Modifico la descrizione del movimento
                            Dt_Vin.Rows(i).Item("Descrizione") = Dt_Vin.Rows(i).Item("Descrizione") & _
                                                                " (" & Dt_Vin.Rows(i).Item("StrCampo_Registri") & " " & _
                                                                strDescrizione & ")"

                        End If

                        '----------------------------------
                        'GESTIONE LOTTO PRODOTTI
                        DettagliLotto = ""
                        DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva,
                                                                        Dt_Vin.Rows(i).Item("Elem_Cod"),
                                                                        Dt_Vin.Rows(i).Item("Mat_Cod"),
                                                                        Dt_Vin.Rows(i).Item("Lotto"),
                                                                        moduliCliente,
                                                                        objParametri_Server)

                        If DettagliLotto <> "" Then
                            'DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Descrizione") += " " + DettagliLotto
                            Dt_Vin.Rows(i).Item("Mat_Des") += " " + DettagliLotto
                        End If

                        '----------------------------------
                        'GESTIONE NUMERO DI VASCA
                        'opzione aggiunta il 24/04/2013
                        If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then
                            'aggiunto il 15/10/2012
                            Try

                                DettagliVasca = ""
                                If Dt_Vin.Rows(i).Item("Tipo_Destinazione") = 13 Then

                                    DettagliVasca = objCantineHLP.IdentificativoCapacitaVasca_from_Codice( _
                                                                     QS_Flag_StampaNumeroVasca, _
                                                                    QS_Flag_StampaCapacitaVasca, _
                                                                   DT_Vasche, _
                                                                   Dt_Vin.Rows(i).Item("SaCod_Dest"), _
                                                                   Dt_Vin.Rows(i).Item("Id_Dest"))
                                End If
                                If DettagliVasca <> "" Then
                                    Dt_Vin.Rows(i).Item("Mat_Des") += DettagliVasca
                                End If

                            Catch ex As Exception
                                Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
                            End Try
                        End If
                        '----------------------------------

                        'nuova riga
                        RigaDs = DsRegistro.DS_RegistroVinificazione.NewDS_RegistroVinificazioneRow

                        RigaDs.Id_Agenda = Dt_Vin.Rows(i).Item("Id_Agenda")
                        RigaDs.Data_Movimento = Dt_Vin.Rows(i).Item("Data_Movimento")
                        RigaDs.des_lib = Dt_Vin.Rows(i).Item("des_lib")

                        '--------------------------------------------------
                        'modifica del 30/10/2015: togliete "+ accettazione e numero pesata"
                        'esempio: DDT Ricevuto + Accettazione (42) (n. 1200001 Rif:  RIVA MARCHETTI S.S. AZ. AGRICOLA)
                        Descrizione = Dt_Vin.Rows(i).Item("Descrizione")

                        If InStr(Descrizione, "+ Accettazione") > 0 Then

                            Dim vect_str As String()
                            vect_str = Descrizione.Split("(")

                            Select Case Dt_Vin.Rows(i).Item("lav_cod")
                                Case LAVCOD_ACCETTAZIONE_DIVERSI
                                    Descrizione = "DDT Ricevuto " & Replace(vect_str(2), ")", "")
                                Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                                    Descrizione = "Distinta Carico " & Replace(vect_str(2), ")", "")
                                Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                                    Descrizione = "Auto DDT " & Replace(vect_str(2), ")", "")
                            End Select

                        End If

                        'RigaDs.Descrizione = Dt_Vin.Rows(i).Item("Descrizione")
                        RigaDs.Descrizione = Descrizione
                        '--------------------------------------------------

                        RigaDs.Designazione = Dt_Vin.Rows(i).Item("Designazione")
                        RigaDs.Cau_Mov = Dt_Vin.Rows(i).Item("Cau_Mov")
                        RigaDs.Mov_Desc = Dt_Vin.Rows(i).Item("Mov_Desc")
                        RigaDs.Elem_Cod = Dt_Vin.Rows(i).Item("Elem_Cod")
                        RigaDs.Mat_Cod = Dt_Vin.Rows(i).Item("Mat_Cod")
                        RigaDs.Mov_Det_Des = Dt_Vin.Rows(i).Item("Mov_Det_Des")

                        RigaDs.Udm_Cod = Dt_Vin.Rows(i).Item("Udm_Cod")
                        RigaDs.Udm_Sim = Dt_Vin.Rows(i).Item("Udm_Sim")
                        RigaDs.Mat_Des = Dt_Vin.Rows(i).Item("Mat_Des")
                        RigaDs.Cod_Articolo = Dt_Vin.Rows(i).Item("Cod_Articolo")
                        RigaDs.NDoc = Dt_Vin.Rows(i).Item("NDoc")
                        RigaDs.CaricoLt = Dt_Vin.Rows(i).Item("CaricoLt")
                        RigaDs.ScaricoLt = Dt_Vin.Rows(i).Item("ScaricoLt")
                        RigaDs.ScaricoKg = Dt_Vin.Rows(i).Item("ScaricoKg")
                        RigaDs.CaricoKg = Dt_Vin.Rows(i).Item("CaricoKg")

                        'modifica del 24/11/2014 (X La spinosa:
                        'se attiva l'opzione e se l'operazione comporta un'integrazione (separazione consistenze, declassamento, taglio/accorpamento, ecc)
                        'visualizzo il lotto (trasformazione_des)
                        If QS_Flag_StampaLottoTrasformazione = True And Dt_Vin.Rows(i).Item("Tipo_Integrazione") > 0 Then
                            RigaDs.Mat_Des = RigaDs.Mat_Des & " - Lotto: " & Dt_Vin.Rows(i).Item("Lotto")
                        End If

                        'Inserisco la riga
                        DsRegistro.DS_RegistroVinificazione.Rows.Add(RigaDs)

                    Else
                        debug = True
                    End If 'Flag_AggiungiRiga


                    ''§§§§§§§§§§§§§§§§§§§§§§§§§§
                    ''BACKUP
                    'If Not IsDBNull(DsRegistro.DS_RegistroVinificazione.Rows(i).Item("StrCampo_Registri")) AndAlso _
                    '   DsRegistro.DS_RegistroVinificazione.Rows(i).Item("StrCampo_Registri") <> String.Empty Then

                    '    'strDescrizione = objMR.Descrizione_from_MovimentixReport(Qs_Piva, _
                    '    '                                                   0, _
                    '    '                                                   DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Id_Agenda"), _
                    '    '                                                   QS_Report, _
                    '    '                                                   "", objParametri_Server)

                    '    strDescrizione = objCantineHLP.DescMovXReport_from_IdAgenda(DT_MovXReport, DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Id_Agenda"))

                    '    'Modifico la descrizione del movimento
                    '    DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Descrizione") = _
                    '                            DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Descrizione") & _
                    '                             " (" & DsRegistro.DS_RegistroVinificazione.Rows(i).Item("StrCampo_Registri") & " " & _
                    '                             strDescrizione & ")"

                    'End If

                    ''----------------------------------
                    ''GESTIONE LOTTO PRODOTTI
                    'DettagliLotto = ""
                    'DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva, _
                    '                                                DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Elem_Cod"), _
                    '                                                DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Mat_Cod"), _
                    '                                                DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Lotto"), _
                    '                                                objParametri_Server)

                    'If DettagliLotto <> "" Then
                    '    'DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Descrizione") += " " + DettagliLotto
                    '    DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Mat_Des") += " " + DettagliLotto
                    'End If

                    ''----------------------------------
                    ''GESTIONE NUMERO DI VASCA
                    ''opzione aggiunta il 24/04/2013
                    'If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then
                    '    'aggiunto il 15/10/2012
                    '    Try

                    '        DettagliVasca = ""
                    '        If DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Tipo_Destinazione") = 13 Then

                    '            DettagliVasca = objCantineHLP.IdentificativoCapacitaVasca_from_Codice( _
                    '                                             QS_Flag_StampaNumeroVasca, _
                    '                                            QS_Flag_StampaCapacitaVasca, _
                    '                                           DT_Vasche, _
                    '                                           DsRegistro.DS_RegistroVinificazione.Rows(i).Item("SaCod_Dest"), _
                    '                                           DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Id_Dest"))
                    '        End If
                    '        If DettagliVasca <> "" Then
                    '            DsRegistro.DS_RegistroVinificazione.Rows(i).Item("Mat_Des") += DettagliVasca
                    '        End If

                    '    Catch ex As Exception
                    '        Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
                    '    End Try
                    'End If
                    ''----------------------------------

                Next

                DsRegistro.AcceptChanges()

            End If

        Catch ex As Exception
            DsRegistro.Clear()
            DsRegistro.AcceptChanges()
            Log_Errori += "- Carica_DsRegistroVinificazione: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub


    '###########################################################################
    Private Function Carica_DsRiepilogoVinificazione_ContoLavorazioneUnificato( _
                                                    ByRef DsRiepilogo As DS_RiepilogoVinificazione, _
                                                    ByVal DataRiepilogoInizio As Date, _
                                                    ByVal DataRiepilogoFine As Date, _
                                                    ByVal DataReportInizio As Date, _
                                                    ByVal DataReportFine As Date, _
                                                    ByVal DataTreMesiPrima As Date, _
                                                    ByVal Lista_CodRisUm As String, _
                                                    ByVal Lista_PrepCod As String, _
                                                    ByVal Lista_IdTrasf_NoComm As String, _
                                                    ByVal str_idagenda_filtrocategoria As String, _
                                                    ByVal str_matcod_filtrocategoria As String, _
                                                    ByVal str_lotto_filtrocategoria As String, _
                                                    ByRef objHT_Cali As Hashtable) As String

        Dim objHLP As New AgronicaCoreContabHLP.Contabilita
        Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine
        Dim strLog As String
        Dim i As Integer

        Dim CalCod As Integer
        Dim CalCod_Old As Integer
        Dim CalDes As String = String.Empty
        Dim CalDes_Old As String = String.Empty
        Dim Cifra_Start As Integer
        Dim Cifra_Start_Old As Integer
        Dim Cifra_End As Integer
        Dim Cifra_End_Old As Integer
        Dim Lotto_Cod As Integer
        Dim Cod_Contatto_Terzi As String = String.Empty
        Dim Cod_Contatto_Terzi_Old As String = String.Empty

        Dim strLottiPrec As String
        Dim rag_soc_contatto As String
        Dim DT_Contatti As DataTable

        Dim Lotto As String = String.Empty
        Dim Lotto_Old As String = String.Empty
        Dim SaldoLottoKg As Double = 0
        Dim SaldoLottoLt As Double = 0
        Dim CalcolaSaldo As Boolean = True

        Dim AddRiga As Boolean

        Dim AnnoProduzione As String = String.Empty
        Dim AnnoProduzione_Old As String = String.Empty
        Dim AliasLotto As String = String.Empty

        Dim Dt As DataTable
        Dim DtRiepilogo As DataTable

        Dim objSaldo As New AgronicaCoreStampeDAL.RegistriCantina
        Dim objRiepilogo As New AgronicaCoreStampeDAL.RegistriCantina
        Dim objAlias As New AgronicaCoreAnagrafeDAL.Lotto_Configurazione_Alias_R

        Dim Riga As DS_RiepilogoVinificazione.DS_RiepilogoVinificazioneRow
        Dim CaricoLtDaNonContare, ScaricoLtDaNonContare As Double
        Dim CaricoKGDaNonContare, ScaricoKGDaNonContare As Double

        If DataReportFine > Date.Today Then
            DataReportFine = Date.Today
        End If

        Try
            Dim objContatti As New AgronicaCoreAnagrafeDAL.Contatti_R

            DT_Contatti = objContatti.LeggiDatiMinimi("", "", "", objParametri_Server)

        Catch ex As Exception
            Log_Errori += "- Lettura dei contatti: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            DtRiepilogo = objRiepilogo.RegistroVinificazioneRiepilogo(Qs_Piva, _
                                                                        QS_Report, _
                                                                        DataRiepilogoInizio, _
                                                                        DataReportInizio, _
                                                                        DataReportFine, _
                                                                        QS_Cod_Contatto_Terzi, _
                                                                         Lista_CodRisUm, _
                                                                        Lista_PrepCod, _
                                                                        Lista_IdTrasf_NoComm, _
                                                                        Qs_Sa_Cod, _
                                                                        Qs_Id_Destinazione, _
                                                                        Qs_Cal_Cod, _
                                                                        Qs_Mat_Cod, _
                                                                        QS_Linea_Cod, _
                                                                        QS_Cau_Mov, _
                                                                        QS_Gestione_Conto_Terzi, _
                                                                        str_idagenda_filtrocategoria, _
                                                                        str_matcod_filtrocategoria, _
                                                                        str_lotto_filtrocategoria, _
                                                                        objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura del riepilogo: " + vbCrLf + ex.Message + vbCrLf
        End Try

        'aggiunto in data 19/10/2012 per uniformarlo al registro di commercializzazione
        If Not IsNothing(DtRiepilogo) AndAlso DtRiepilogo.Rows.Count > 0 Then

            Try

                ' aggiungo una colonna al dt
                DtRiepilogo.Columns.Add(New DataColumn("Annata", GetType(String)))

                'per la modifica alal query in cui restituisco in un colpo solo start, end e cod,
                'aggiungo le colonne
                DtRiepilogo.Columns.Add(New DataColumn("Cifra_Start", GetType(Integer)))
                DtRiepilogo.Columns.Add(New DataColumn("Cifra_End", GetType(Integer)))
                DtRiepilogo.Columns.Add(New DataColumn("Lotto_Cod", GetType(Integer)))

                For i = 0 To DtRiepilogo.Rows.Count - 1

                    Cifra_Start = CStr(DtRiepilogo.Rows(i).Item("ConfigLotto")).Split("|")(0)
                    Cifra_End = CStr(DtRiepilogo.Rows(i).Item("ConfigLotto")).Split("|")(1)
                    Lotto_Cod = CStr(DtRiepilogo.Rows(i).Item("ConfigLotto")).Split("|")(2)

                    If Cifra_Start = 0 And Cifra_End = 0 Then
                        AnnoProduzione = ""
                    Else
                        AnnoProduzione = CStr(DtRiepilogo.Rows(i).Item("Lotto")).Substring(Cifra_Start - 1, (CInt(Cifra_End) - CInt(Cifra_Start) + 1))
                    End If

                    DtRiepilogo.Rows(i).Item("Annata") = AnnoProduzione
                    DtRiepilogo.Rows(i).Item("Cifra_Start") = Cifra_Start
                    DtRiepilogo.Rows(i).Item("Cifra_End") = Cifra_End
                    DtRiepilogo.Rows(i).Item("Lotto_Cod") = Lotto_Cod
                Next

            Catch ex As System.ArgumentOutOfRangeException
                strLog = "Configurazione Lotto errata: " & DtRiepilogo.Rows(i).Item("Cal_Des") & " Lotto " & DtRiepilogo.Rows(i).Item("Lotto") & " caratteri " & Cifra_Start & "-" & Cifra_End
            End Try

            If strLog <> String.Empty Then
                Return strLog
            End If

            Try
                Dim Dr As DataRow()
                Dr = DtRiepilogo.Select("1=1", "Elem_Cod DESC, Cal_Des, Cod_Contatto_Terzi, Annata DESC, Lotto DESC, Cifra_Start, Cifra_End")

                Dt = DtRiepilogo.Clone

                For i = 0 To Dr.Length - 1
                    Dt.ImportRow(Dr(i))
                Next

            Catch ex As Exception
                strLog = "Errore select sul dt: " & ex.Message
            End Try

            If strLog <> String.Empty Then
                Return strLog
            End If

        End If

        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

            Try

                For i = 0 To Dt.Rows.Count - 1

                    Cifra_Start = Dt.Rows(i).Item("Cifra_Start")
                    Cifra_End = Dt.Rows(i).Item("Cifra_End")

                    AnnoProduzione = CStr(Dt.Rows(i).Item("Annata"))

                    Cod_Contatto_Terzi = CStr(Dt.Rows(i).Item("Cod_Contatto_Terzi"))
                    ''select rag_soc_contatto
                    'rag_soc_contatto = objHLP.Des_from_CodStr(DT_Contatti, "Cod_Contatto", "Rag_Soc", Cod_Contatto_Terzi)

                    CalCod = CInt(Dt.Rows(i).Item("Cal_Cod"))
                    CalDes = CStr(Dt.Rows(i).Item("Cal_Des"))
                    'CalDes = CStr(Dt.Rows(i).Item("Cal_Des")) & " -- " & rag_soc_contatto

                    Lotto = CStr(Dt.Rows(i).Item("Lotto"))
                    SaldoLottoKg = 0
                    SaldoLottoLt = 0

                    If CalDes <> CalDes_Old Then
                        strLottiPrec = String.Empty
                    End If


                    If CalDes <> CalDes_Old Or AnnoProduzione <> AnnoProduzione_Old Or Cod_Contatto_Terzi <> Cod_Contatto_Terzi_Old Then
                        'primo prodotto o nuovo prodotto                  

                        ' aggiungo la riga vecchia se sono in nuovo prodotto
                        If Not IsNothing(Riga) Then
                            Aggiungi_Riga(DsRiepilogo, Riga)
                        End If

                        Riga = DsRiepilogo.DS_RiepilogoVinificazione.NewDS_RiepilogoVinificazioneRow

                        Riga.DataFineRiepilogo = "Riepilogo al " + DataReportFine.ToShortDateString & Rag_Soc_CTerzi

                        'select rag_soc_contatto
                        rag_soc_contatto = objHLP.Des_from_CodStr(DT_Contatti, "Cod_Contatto", "Rag_Soc", Cod_Contatto_Terzi)

                        ' inizializzo i saldi 
                        Riga.SaldoPrimaKg = 0
                        Riga.SaldoPrimaLt = 0

                        ' inizializzo i valori della riga
                        Riga.CaricoKg = 0
                        Riga.CaricoLt = 0
                        Riga.ScaricoLt = 0
                        Riga.ScaricoKg = 0

                        AliasLotto = objAlias.LottoAlias_from_LottoVal(Qs_Piva, _
                                       Dt.Rows(i).Item("Elem_Cod"), _
                                       Dt.Rows(i).Item("Lotto_Cod"), _
                                       AnnoProduzione, _
                                       "", "", _
                                       objParametri_Server)

                        CalcolaSaldo = True

                    End If

                    If Cifra_Start <> Cifra_Start_Old Or Cifra_End <> Cifra_End_Old Then
                        CalcolaSaldo = True
                    End If

                    If CalcolaSaldo = True Then

                        CalcolaSaldo = False

                        '15/10/14: verifica cali -> modificato il 14/07/2015 per gestire le altre materie prime
                        'l'ho messo dentro al  If CalcolaSaldo = True Then
                        'così viene eseguito solo una volta per voce di riepilogo
                        '(altrimenti sottrae la qta per tutte le ripetizioni della voce di riepilogo)
                        If Dt.Rows(i).Item("elem_Cod") = CALI_LAVORAZIONE Or Dt.Rows(i).Item("elem_Cod") = ALTRE_MATERIE Then

                            If Dt.Rows(i).Item("udm_Cod") = enum_UnitaMisura.Litri Then
                                CaricoLtDaNonContare = 0
                                ScaricoLtDaNonContare = 0
                                objCantineHLP.Verifica_CaloPerdita_Riepilogo(Dt.Rows(i).Item("mat_cod"), objHT_Cali, CaricoLtDaNonContare, ScaricoLtDaNonContare)
                                Riga.CaricoLt += CaricoLtDaNonContare
                                Riga.ScaricoLt += ScaricoLtDaNonContare
                            Else
                                CaricoKGDaNonContare = 0
                                ScaricoKGDaNonContare = 0
                                objCantineHLP.Verifica_CaloPerdita_Riepilogo(Dt.Rows(i).Item("mat_cod"), objHT_Cali, CaricoKGDaNonContare, ScaricoKGDaNonContare)
                                Riga.CaricoKg += CaricoKGDaNonContare
                                Riga.ScaricoKg += ScaricoKGDaNonContare
                            End If

                            ''x i cali non conteggio il saldo precedente
                            'SaldoLottoKg = 0
                            'SaldoLottoLt = 0

                        End If

                        ' Else
                        'TUTTE LE ALTRE VOCI CHE NON SONO CALI

                        'occhio, qui utilizza cod_contatto_terzi (letto dal dt), non quello da querystring
                        'attenzione!! verificare strLottiPrec, _
                        SaldoLottoKg = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
                                                   Qs_Piva, _
                                                   CalCod, _
                                                   AnnoProduzione, _
                                                   Cifra_Start, _
                                                   Cifra_End, _
                                                   enum_UnitaMisura.KG, _
                                                   QS_Report, _
                                                   DataRiepilogoInizio, _
                                                   DataReportInizio, _
                                                   False, _
                                                   strLottiPrec, _
                                                   Cod_Contatto_Terzi, _
                                                    Lista_CodRisUm, _
                                                    Lista_PrepCod, _
                                                    Lista_IdTrasf_NoComm, _
                                                    Qs_Sa_Cod, _
                                                    Qs_Id_Destinazione, _
                                                    Qs_Mat_Cod, _
                                                    QS_Linea_Cod, _
                                                    str_idagenda_filtrocategoria, _
                                                    str_matcod_filtrocategoria, _
                                                    str_lotto_filtrocategoria, _
                                                    objParametri_Server)

                        'occhio, qui utilizza cod_contatto_terzi (letto dal dt), non quello da querystring
                        'attenzione!! verificare strLottiPrec, _
                        SaldoLottoLt = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
                                                   Qs_Piva, _
                                                   CalCod, _
                                                   AnnoProduzione, _
                                                   Cifra_Start, _
                                                   Cifra_End, _
                                                   enum_UnitaMisura.Litri, _
                                                   QS_Report, _
                                                   DataRiepilogoInizio, _
                                                   DataReportInizio, _
                                                   False, _
                                                   strLottiPrec, _
                                                   Cod_Contatto_Terzi, _
                                                   Lista_CodRisUm, _
                                                    Lista_PrepCod, _
                                                    Lista_IdTrasf_NoComm, _
                                                     Qs_Sa_Cod, _
                                                    Qs_Id_Destinazione, _
                                                    Qs_Mat_Cod, _
                                                    QS_Linea_Cod, _
                                                    str_idagenda_filtrocategoria, _
                                                    str_matcod_filtrocategoria, _
                                                    str_lotto_filtrocategoria, _
                                                    objParametri_Server)

                        If Cifra_End - Cifra_Start <> 0 Then
                            strLottiPrec &= " AND SUBSTRING(Movimenti_Dettagli.Lotto, " & Cifra_Start & "," & Cifra_End - Cifra_Start + 1 & ")<>'" & AnnoProduzione & "' " + vbCrLf
                        Else
                            strLottiPrec = String.Empty
                        End If

                        'End If 'calo

                    End If


                    Riga.CaricoLt += Dt.Rows(i).Item("CaricoLt")
                    Riga.ScaricoLt += Dt.Rows(i).Item("ScaricoLt")
                    Riga.CaricoKg += Dt.Rows(i).Item("CaricoKg")
                    Riga.ScaricoKg += Dt.Rows(i).Item("ScaricoKg")

                    Riga.SaldoPrimaKg += SaldoLottoKg
                    Riga.SaldoPrimaLt += SaldoLottoLt

                    Dim strAnnata As String = String.Empty
                    If AnnoProduzione <> String.Empty Or AliasLotto <> String.Empty Then
                        strAnnata = IIf(AliasLotto = String.Empty, AnnoProduzione, AliasLotto)
                    End If

                    Riga.Linea = CalDes & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata & " -- " & rag_soc_contatto
                    '  Riga.Linea = CalDes & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata

                    CalCod_Old = CalCod
                    CalDes_Old = CalDes
                    Cod_Contatto_Terzi_Old = Cod_Contatto_Terzi
                    AnnoProduzione_Old = AnnoProduzione
                    Cifra_Start_Old = Cifra_Start
                    Cifra_End_Old = Cifra_End

                    ' se è l'ultima riga la aggiungo
                    If i = Dt.Rows.Count - 1 Then
                        Aggiungi_Riga(DsRiepilogo, Riga)
                    End If

                Next

            Catch ex As Exception
                Log_Errori += "- Elaborazione del riepilogo conto lavorazione unificato: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        Return strLog

    End Function


    '###########################################################################
    Private Function Carica_DsRiepilogoVinificazione(ByRef DsRiepilogo As DS_RiepilogoVinificazione, _
                                                    ByVal DataRiepilogoInizio As Date, _
                                                    ByVal DataRiepilogoFine As Date, _
                                                    ByVal DataReportInizio As Date, _
                                                    ByVal DataReportFine As Date, _
                                                    ByVal DataTreMesiPrima As Date, _
                                                    ByVal Lista_CodRisUm As String, _
                                                    ByVal Lista_PrepCod As String, _
                                                    ByVal Lista_IdTrasf_NoComm As String, _
                                                    ByVal str_idagenda_filtrocategoria As String, _
                                                    ByVal str_matcod_filtrocategoria As String, _
                                                    ByVal str_lotto_filtrocategoria As String, _
                                                    ByRef objHT_Cali As Hashtable) As String

        'ByVal Opt_Gestione_RegistroVinificazione As Integer, _

        Dim strLog As String
        Dim i As Integer

        Dim CalCod As Integer
        Dim CalCod_Old As Integer
        Dim CalDes As String = String.Empty
        Dim CalDes_Old As String = String.Empty
        Dim Cifra_Start As Integer
        Dim Cifra_Start_Old As Integer
        Dim Cifra_End As Integer
        Dim Cifra_End_Old As Integer
        Dim Lotto_Cod As Integer

        Dim strLottiPrec As String

        Dim Lotto As String = String.Empty
        Dim Lotto_Old As String = String.Empty
        Dim SaldoLottoKg As Double = 0
        Dim SaldoLottoLt As Double = 0
        Dim CalcolaSaldo As Boolean = True

        Dim AddRiga As Boolean
        Dim CaricoLtDaNonContare, ScaricoLtDaNonContare As Double
        Dim CaricoKGDaNonContare, ScaricoKGDaNonContare As Double

        Dim AnnoProduzione As String = String.Empty
        Dim AnnoProduzione_Old As String = String.Empty
        Dim AliasLotto As String = String.Empty

        Dim Dt As DataTable
        Dim DtRiepilogo As DataTable

        Dim objSaldo As New AgronicaCoreStampeDAL.RegistriCantina
        Dim objRiepilogo As New AgronicaCoreStampeDAL.RegistriCantina
        Dim objAlias As New AgronicaCoreAnagrafeDAL.Lotto_Configurazione_Alias_R
        Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine

        Dim Riga As DS_RiepilogoVinificazione.DS_RiepilogoVinificazioneRow

        If DataReportFine > Date.Today Then
            DataReportFine = Date.Today
        End If

        Try

            DtRiepilogo = objRiepilogo.RegistroVinificazioneRiepilogo(Qs_Piva, _
                                                                        QS_Report, _
                                                                        DataRiepilogoInizio, _
                                                                        DataReportInizio, _
                                                                        DataReportFine, _
                                                                        QS_Cod_Contatto_Terzi, _
                                                                         Lista_CodRisUm, _
                                                                        Lista_PrepCod, _
                                                                        Lista_IdTrasf_NoComm, _
                                                                        Qs_Sa_Cod, _
                                                                        Qs_Id_Destinazione, _
                                                                        Qs_Cal_Cod, _
                                                                        Qs_Mat_Cod, _
                                                                        QS_Linea_Cod, _
                                                                        QS_Cau_Mov, _
                                                                        QS_Gestione_Conto_Terzi, _
                                                                        str_idagenda_filtrocategoria, _
                                                                        str_matcod_filtrocategoria, _
                                                                        str_lotto_filtrocategoria, _
                                                                        objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura del riepilogo: " + vbCrLf + ex.Message + vbCrLf
        End Try

        'aggiunto in data 19/10/2012 per uniformarlo al registro di commercializzazione
        If Not IsNothing(DtRiepilogo) AndAlso DtRiepilogo.Rows.Count > 0 Then

            Try

                ' aggiungo una colonna al dt
                DtRiepilogo.Columns.Add(New DataColumn("Annata", GetType(String)))

                'per la modifica alal query in cui restituisco in un colpo solo start, end e cod,
                'aggiungo le colonne
                DtRiepilogo.Columns.Add(New DataColumn("Cifra_Start", GetType(Integer)))
                DtRiepilogo.Columns.Add(New DataColumn("Cifra_End", GetType(Integer)))
                DtRiepilogo.Columns.Add(New DataColumn("Lotto_Cod", GetType(Integer)))

                For i = 0 To DtRiepilogo.Rows.Count - 1

                    Cifra_Start = CStr(DtRiepilogo.Rows(i).Item("ConfigLotto")).Split("|")(0)
                    Cifra_End = CStr(DtRiepilogo.Rows(i).Item("ConfigLotto")).Split("|")(1)
                    Lotto_Cod = CStr(DtRiepilogo.Rows(i).Item("ConfigLotto")).Split("|")(2)

                    If Cifra_Start = 0 And Cifra_End = 0 Then
                        AnnoProduzione = ""
                    Else
                        AnnoProduzione = CStr(DtRiepilogo.Rows(i).Item("Lotto")).Substring(Cifra_Start - 1, (CInt(Cifra_End) - CInt(Cifra_Start) + 1))
                    End If

                    DtRiepilogo.Rows(i).Item("Annata") = AnnoProduzione
                    DtRiepilogo.Rows(i).Item("Cifra_Start") = Cifra_Start
                    DtRiepilogo.Rows(i).Item("Cifra_End") = Cifra_End
                    DtRiepilogo.Rows(i).Item("Lotto_Cod") = Lotto_Cod
                Next

            Catch ex As System.ArgumentOutOfRangeException
                strLog = "Configurazione Lotto errata: " & DtRiepilogo.Rows(i).Item("Cal_Des") & " Lotto " & DtRiepilogo.Rows(i).Item("Lotto") & " caratteri " & Cifra_Start & "-" & Cifra_End
            End Try

            If strLog <> String.Empty Then
                Return strLog
            End If

            Dim Dr As DataRow()
            Dr = DtRiepilogo.Select("1=1", "Elem_Cod DESC, Cal_Des, Annata DESC, Lotto DESC, Cifra_Start, Cifra_End")

            Dt = DtRiepilogo.Clone

            For i = 0 To Dr.Length - 1
                Dt.ImportRow(Dr(i))
            Next

        End If


        If Not IsNothing(Dt) AndAlso Dt.Rows.Count > 0 Then

            Try

                For i = 0 To Dt.Rows.Count - 1

                    Cifra_Start = Dt.Rows(i).Item("Cifra_Start")
                    Cifra_End = Dt.Rows(i).Item("Cifra_End")

                    AnnoProduzione = CStr(Dt.Rows(i).Item("Annata"))

                    CalCod = CInt(Dt.Rows(i).Item("Cal_Cod"))
                    CalDes = CStr(Dt.Rows(i).Item("Cal_Des"))

                    Lotto = CStr(Dt.Rows(i).Item("Lotto"))
                    SaldoLottoKg = 0
                    SaldoLottoLt = 0

                    If CalDes <> CalDes_Old Then
                        strLottiPrec = String.Empty
                    End If


                    If CalDes <> CalDes_Old Or AnnoProduzione <> AnnoProduzione_Old Then
                        'primo prodotto o nuovo prodotto                  

                        ' aggiungo la riga vechia
                        If Not IsNothing(Riga) Then
                            Aggiungi_Riga(DsRiepilogo, Riga)
                        End If

                        Riga = DsRiepilogo.DS_RiepilogoVinificazione.NewDS_RiepilogoVinificazioneRow

                        Riga.DataFineRiepilogo = "Riepilogo al " + DataReportFine.ToShortDateString & Rag_Soc_CTerzi

                        ' inizializzo i saldi 
                        Riga.SaldoPrimaKg = 0
                        Riga.SaldoPrimaLt = 0
                        'Riga.SaldoDopoKg = 0 'boh
                        'Riga.SaldoDopoLt = 0 'boh

                        ' inizializzo i valori della riga
                        Riga.CaricoKg = 0
                        Riga.CaricoLt = 0
                        Riga.ScaricoLt = 0
                        Riga.ScaricoKg = 0

                        AliasLotto = objAlias.LottoAlias_from_LottoVal(Qs_Piva, _
                                                              Dt.Rows(i).Item("Elem_Cod"), _
                                                              Dt.Rows(i).Item("Lotto_Cod"), _
                                                              AnnoProduzione, _
                                                              "", "", _
                                                              objParametri_Server)

                        CalcolaSaldo = True

                    End If

                    If Cifra_Start <> Cifra_Start_Old Or Cifra_End <> Cifra_End_Old Then
                        CalcolaSaldo = True
                    End If

                    If CalcolaSaldo = True Then

                        CalcolaSaldo = False


                        'MODIFICA DEL 29/10/2014
                        'LEONARDI CI HA DETTO CHE NON VANNO CONTEGGIATI I SALDI DEI CALI: LI ESCLUDIAMO DALLA QUERY DI RIEPILOGO
                        'quindi non entra più in questo if

                        '15/10/14: verifica cali -> modificato il 14/07/2015 per gestire altre materie prime
                        'l'ho messo dentro al  If CalcolaSaldo = True Then
                        'così viene eseguito solo una volta per voce di riepilogo
                        '(altrimenti sottrae la qta per tutte le ripetizioni della voce di riepilogo)
                        If Dt.Rows(i).Item("elem_Cod") = CALI_LAVORAZIONE Or Dt.Rows(i).Item("elem_Cod") = ALTRE_MATERIE Then

                            If Dt.Rows(i).Item("udm_Cod") = enum_UnitaMisura.Litri Then
                                CaricoLtDaNonContare = 0
                                ScaricoLtDaNonContare = 0
                                objCantineHLP.Verifica_CaloPerdita_Riepilogo(Dt.Rows(i).Item("mat_cod"), objHT_Cali, CaricoLtDaNonContare, ScaricoLtDaNonContare)
                                Riga.CaricoLt += CaricoLtDaNonContare
                                Riga.ScaricoLt += ScaricoLtDaNonContare
                            Else
                                CaricoKGDaNonContare = 0
                                ScaricoKGDaNonContare = 0
                                objCantineHLP.Verifica_CaloPerdita_Riepilogo(Dt.Rows(i).Item("mat_cod"), objHT_Cali, CaricoKGDaNonContare, ScaricoKGDaNonContare)
                                Riga.CaricoKg += CaricoKGDaNonContare
                                Riga.ScaricoKg += ScaricoKGDaNonContare
                            End If

                            ''x i cali non conteggio il saldo precedente
                            'SaldoLottoKg = 0
                            'SaldoLottoLt = 0

                        End If


                        'Else
                        'TUTTE LE ALTRE VOCI CHE NON SONO CALI

                        'attenzione!! verificare strLottiPrec, _
                        SaldoLottoKg = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
                                                   Qs_Piva, _
                                                   CalCod, _
                                                   AnnoProduzione, _
                                                   Cifra_Start, _
                                                   Cifra_End, _
                                                   enum_UnitaMisura.KG, _
                                                   QS_Report, _
                                                   DataRiepilogoInizio, _
                                                   DataReportInizio, _
                                                   False, _
                                                   strLottiPrec, _
                                                   QS_Cod_Contatto_Terzi, _
                                                    Lista_CodRisUm, _
                                                    Lista_PrepCod, _
                                                    Lista_IdTrasf_NoComm, _
                                                    Qs_Sa_Cod, _
                                                    Qs_Id_Destinazione, _
                                                    Qs_Mat_Cod, _
                                                    QS_Linea_Cod, _
                                                    str_idagenda_filtrocategoria, _
                                                    str_matcod_filtrocategoria, _
                                                    str_lotto_filtrocategoria, _
                                                    objParametri_Server)

                        'attenzione!! verificare strLottiPrec, _
                        SaldoLottoLt = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
                                                   Qs_Piva, _
                                                   CalCod, _
                                                   AnnoProduzione, _
                                                   Cifra_Start, _
                                                   Cifra_End, _
                                                   enum_UnitaMisura.Litri, _
                                                   QS_Report, _
                                                   DataRiepilogoInizio, _
                                                   DataReportInizio, _
                                                   False, _
                                                   strLottiPrec, _
                                                   QS_Cod_Contatto_Terzi, _
                                                   Lista_CodRisUm, _
                                                    Lista_PrepCod, _
                                                    Lista_IdTrasf_NoComm, _
                                                     Qs_Sa_Cod, _
                                                    Qs_Id_Destinazione, _
                                                    Qs_Mat_Cod, _
                                                    QS_Linea_Cod, _
                                                    str_idagenda_filtrocategoria, _
                                                    str_matcod_filtrocategoria, _
                                                    str_lotto_filtrocategoria, _
                                                    objParametri_Server)

                        'if aggiunto in data 09/10/2014: se cifra_start e cifra_end sono a 0 o cmq si eguagliano, verrebbe un substring senza senso
                        If Cifra_End - Cifra_Start <> 0 Then
                            strLottiPrec &= " AND SUBSTRING(Movimenti_Dettagli.Lotto, " & Cifra_Start & "," & Cifra_End - Cifra_Start + 1 & ")<>'" & AnnoProduzione & "' " + vbCrLf
                        End If

                        'Lotto_Old = Lotto

                        'End If'calo

                    End If

                    Riga.CaricoLt += Dt.Rows(i).Item("CaricoLt")
                    Riga.ScaricoLt += Dt.Rows(i).Item("ScaricoLt")
                    Riga.CaricoKg += Dt.Rows(i).Item("CaricoKg")
                    Riga.ScaricoKg += Dt.Rows(i).Item("ScaricoKg")

                    Riga.SaldoPrimaKg += SaldoLottoKg
                    Riga.SaldoPrimaLt += SaldoLottoLt

                    Dim strAnnata As String = String.Empty
                    If AnnoProduzione <> String.Empty Or AliasLotto <> String.Empty Then
                        strAnnata = IIf(AliasLotto = String.Empty, AnnoProduzione, AliasLotto)
                    End If

                    Riga.Linea = CalDes & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata

                    CalCod_Old = CalCod
                    CalDes_Old = CalDes
                    AnnoProduzione_Old = AnnoProduzione
                    Cifra_Start_Old = Cifra_Start
                    Cifra_End_Old = Cifra_End

                    ' se è l'ultima riga la aggiungo
                    If i = Dt.Rows.Count - 1 Then
                        Aggiungi_Riga(DsRiepilogo, Riga)
                    End If

                Next

            Catch ex As Exception
                DsRiepilogo.Clear()
                DsRiepilogo.AcceptChanges()
                Log_Errori += "- Elaborazione del riepilogo: " + vbCrLf + ex.Message + vbCrLf
            End Try

        End If

        Return strLog

    End Function

    '###########################################################################
    ''Private Sub Aggiungi_Riga(ByRef DsRiepilogo As DS_RiepilogoVinificazione, _
    ''                            ByVal Riga As DS_RiepilogoVinificazione.DS_RiepilogoVinificazioneRow, _
    ''                            ByVal SaldoTreMesiPrimaKg As Double, _
    ''                            ByVal SaldoTreMesiPrimaLt As Double)
    'Private Sub Aggiungi_Riga(ByRef DsRiepilogo As DS_RiepilogoVinificazione, _
    '                            ByVal Riga As DS_RiepilogoVinificazione.DS_RiepilogoVinificazioneRow, _
    '                            ByRef objSaldo As AgronicaCoreStampeDAL.RegistriCantina, _
    '                            ByVal DataRiepilogoInizio As Date, _
    '                            ByVal DataReportInizio As Date, _
    '                            ByVal AnnoProduzione As String, _
    '                            ByVal Cal_Cod As Integer, _
    '                            ByVal Cifra_Start As Integer, _
    '                            ByVal Cifra_End As Integer, _
    '                            ByVal Opt_Gestione_RegistroVinificazione As Integer, _
    '                            ByVal Lista_PrepCod As String)
    Private Sub Aggiungi_Riga(ByRef DsRiepilogo As DS_RiepilogoVinificazione, _
                               ByVal Riga As DS_RiepilogoVinificazione.DS_RiepilogoVinificazioneRow _
                              )

        Riga.SaldoDopoKg = Riga.SaldoPrimaKg + Riga.CaricoKg - Riga.ScaricoKg

        Riga.SaldoDopoLt = Riga.SaldoPrimaLt + Riga.CaricoLt - Riga.ScaricoLt

        'MODIFICA DEL 22/10/2012:
        'nel reg di vinificazione non bisogna guardare i saldi dei mesi prima (se saldo=0), 
        'come in quello di commercializzazione

        'inserisco il prodotto nel riepilogo
        DsRiepilogo.DS_RiepilogoVinificazione.AddDS_RiepilogoVinificazioneRow(Riga)

        '''non va bene!
        '''perchè elimina i prodotti che hanno saldo 0 e che hanno saldo 0 tre mesi prima, 
        '''anche se sono stati movimentati nell'intervallo corrente
        '''e senza verificare il saldo dei due mesi prima
        ''If (Riga.SaldoDopoKg = 0 And SaldoTreMesiPrimaKg = 0) And (Riga.SaldoDopoLt = 0 And SaldoTreMesiPrimaLt = 0) Then
        ''    'la giacenza kg è 0 e anche 3 mesi prima era 0 e la giacenza lt è 0 e anche 3 mesi prima era 0, 
        ''    'quindi non inserisco la riga nel riepilogo
        ''    Dim pippo As Integer = 0
        ''Else
        ''    'inserisco il prodotto nel riepilogo
        ''    DsRiepilogo.DS_RiepilogoVinificazione.AddDS_RiepilogoVinificazioneRow(Riga)
        ''End If


        ''verifico se nell'intervallo corrente il prodotto non è stato caricato, non è stato scaricato e non è in giacenza
        'If Riga.CaricoLt = 0 And Riga.ScaricoLt = 0 And Riga.CaricoKg = 0 And Riga.ScaricoKg = 0 And Riga.SaldoDopoLt = 0 And Riga.SaldoDopoKg = 0 Then

        '    'verifico il saldo al mese prima
        '    Dim SaldoUnMesePrimaKg As Double
        '    Dim SaldoUnMesePrimaLt As Double
        '    Dim DataUnMesePrima As Date

        '    'tolgo 1 giorno
        '    DataUnMesePrima = DateAdd(DateInterval.Day, -1, DataReportInizio)

        '    SaldoUnMesePrimaKg = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
        '                                               Qs_Piva, _
        '                                               Cal_Cod, _
        '                                               AnnoProduzione, _
        '                                               Cifra_Start, _
        '                                                Cifra_End, _
        '                                               2, _
        '                                               QS_Report, _
        '                                               DataRiepilogoInizio, _
        '                                               DataUnMesePrima, _
        '                                               True, _
        '                                               "", _
        '                                               QS_Cod_Contatto_Terzi, _
        '                                               Opt_Gestione_RegistroVinificazione, _
        '                                                Lista_PrepCod, _
        '                                               objParametri_Server)

        '    SaldoUnMesePrimaLt = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
        '                                                           Qs_Piva, _
        '                                                           Cal_Cod, _
        '                                                           AnnoProduzione, _
        '                                                           Cifra_Start, _
        '                                                           Cifra_End, _
        '                                                           29, _
        '                                                           QS_Report, _
        '                                                           DataRiepilogoInizio, _
        '                                                           DataUnMesePrima, _
        '                                                           True, _
        '                                                           "", _
        '                                                           QS_Cod_Contatto_Terzi, _
        '                                                           Opt_Gestione_RegistroVinificazione, _
        '                                                            Lista_PrepCod, _
        '                                                            objParametri_Server)

        '    'il mese prima era a saldo 0, verifico due mesi prima
        '    If SaldoUnMesePrimaKg = 0 And SaldoUnMesePrimaLt = 0 Then

        '        Dim SaldoDueMesiPrimaKg As Double
        '        Dim SaldoDueMesiPrimaLt As Double
        '        Dim DataDueMesiPrima As Date

        '        'tolgo 1 mese
        '        DataDueMesiPrima = DateAdd(DateInterval.Month, -1, DataReportInizio)
        '        'tolgo 1 giorno
        '        DataDueMesiPrima = DateAdd(DateInterval.Day, -1, DataDueMesiPrima)

        '        SaldoDueMesiPrimaKg = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
        '                                                     Qs_Piva, _
        '                                                     Cal_Cod, _
        '                                                     AnnoProduzione, _
        '                                                     Cifra_Start, _
        '                                                      Cifra_End, _
        '                                                     2, _
        '                                                     QS_Report, _
        '                                                     DataRiepilogoInizio, _
        '                                                     DataDueMesiPrima, _
        '                                                     True, _
        '                                                     "", _
        '                                                     QS_Cod_Contatto_Terzi, _
        '                                                     Opt_Gestione_RegistroVinificazione, _
        '                                                    Lista_PrepCod, _
        '                                                     objParametri_Server)


        '        SaldoDueMesiPrimaLt = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
        '                                                               Qs_Piva, _
        '                                                               Cal_Cod, _
        '                                                               AnnoProduzione, _
        '                                                               Cifra_Start, _
        '                                                               Cifra_End, _
        '                                                               29, _
        '                                                               QS_Report, _
        '                                                               DataRiepilogoInizio, _
        '                                                               DataDueMesiPrima, _
        '                                                               True, _
        '                                                               "", _
        '                                                               QS_Cod_Contatto_Terzi, _
        '                                                               Opt_Gestione_RegistroVinificazione, _
        '                                                                Lista_PrepCod, _
        '                                                                objParametri_Server)

        '        'due mesi prima era a saldo 0, verifico tre mesi prima
        '        If SaldoDueMesiPrimaKg = 0 And SaldoDueMesiPrimaLt = 0 Then

        '            Dim SaldoTreMesiPrimaKg As Double
        '            Dim SaldoTreMesiPrimaLt As Double
        '            Dim DataTreMesiPrima As Date

        '            'tolgo 2 mesi
        '            DataTreMesiPrima = DateAdd(DateInterval.Month, -2, DataReportInizio)
        '            'tolgo 1 giorno
        '            DataTreMesiPrima = DateAdd(DateInterval.Day, -1, DataTreMesiPrima)


        '            SaldoTreMesiPrimaKg = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
        '                                              Qs_Piva, _
        '                                              Cal_Cod, _
        '                                              AnnoProduzione, _
        '                                              Cifra_Start, _
        '                                               Cifra_End, _
        '                                              2, _
        '                                              QS_Report, _
        '                                              DataRiepilogoInizio, _
        '                                              DataTreMesiPrima, _
        '                                              True, _
        '                                              "", _
        '                                              QS_Cod_Contatto_Terzi, _
        '                                              Opt_Gestione_RegistroVinificazione, _
        '                                                Lista_PrepCod, _
        '                                              objParametri_Server)


        '            SaldoTreMesiPrimaLt = objSaldo.RegistroVinificazioneSaldoxRiepilogo( _
        '                                                                   Qs_Piva, _
        '                                                                   Cal_Cod, _
        '                                                                   AnnoProduzione, _
        '                                                                   Cifra_Start, _
        '                                                                   Cifra_End, _
        '                                                                   29, _
        '                                                                   QS_Report, _
        '                                                                   DataRiepilogoInizio, _
        '                                                                   DataTreMesiPrima, _
        '                                                                   True, _
        '                                                                   "", _
        '                                                                   QS_Cod_Contatto_Terzi, _
        '                                                                   Opt_Gestione_RegistroVinificazione, _
        '                                                                    Lista_PrepCod, _
        '                                                                    objParametri_Server)

        '            If SaldoTreMesiPrimaKg = 0 And SaldoTreMesiPrimaLt = 0 Then
        '                'non inserisco nel riepilogo
        '            Else
        '                'inserisco il prodotto nel riepilogo
        '                DsRiepilogo.DS_RiepilogoVinificazione.AddDS_RiepilogoVinificazioneRow(Riga)
        '            End If
        '        Else 'saldo due mesi prima
        '            'inserisco il prodotto nel riepilogo
        '            DsRiepilogo.DS_RiepilogoVinificazione.AddDS_RiepilogoVinificazioneRow(Riga)
        '        End If
        '    Else 'saldo un mese prima
        '        'inserisco il prodotto nel riepilogo
        '        DsRiepilogo.DS_RiepilogoVinificazione.AddDS_RiepilogoVinificazioneRow(Riga)
        '    End If
        'Else 'prodotto movimentato nell'intervallo
        '    'è stato caricato o scaricato o è in giacenza
        '    'inserisco il prodotto nel riepilogo
        '    DsRiepilogo.DS_RiepilogoVinificazione.AddDS_RiepilogoVinificazioneRow(Riga)
        'End If



    End Sub


End Class
