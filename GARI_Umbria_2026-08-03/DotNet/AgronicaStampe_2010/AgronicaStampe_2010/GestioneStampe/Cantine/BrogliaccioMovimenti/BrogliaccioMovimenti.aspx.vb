Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class BrogliaccioMovimenti
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptBrogliaccio As Rpt_BrogliaccioMovimenti
    Private DsCommercializzazione As DS_RegistroCommercializzazioneVini


    '----- Gestione Querystring
    Dim Qs_Piva As String
    Dim Qs_RagSoc As String
    'Dim Qs_Sa_Cod, Qs_Id_Destinazione As Integer
    'Dim Qs_Cal_Cod, Qs_Mat_Cod, QS_Linea_Cod As Integer
    Dim QS_DataInizio As String
    Dim QS_DataFine As String
    'Dim QS_Report As String
    Dim QS_TipoStampa As String
    Dim QS_Anno As String
    Dim QS_Mese As String
    'Dim QS_StampaIntestazione As String
    'Dim QS_StampaRiporti As String
    'Dim QS_Cod_Contatto_Terzi, Rag_Soc_CTerzi As String
    'Dim QS_Cau_Mov As String
    Dim QS_Filtro_Stampa As String
    'Dim Qs_Flag_VerificaRegistri As Boolean
    'Dim QS_Flag_StampaNumeroVasca As Boolean
    'Dim QS_Flag_StampaCapacitaVasca As Boolean
    'Dim QS_Flag_StampaLottoTrasformazione As Boolean
    ''Dim Opt_Gestione_RegistroVinificazione As Integer
    Dim QS_Gestione_Conto_Terzi As enum_RegistroContoTerzi
    'Dim flag_docg, flag_dop, flag_igp, flag_tavola As Boolean
    'Dim Flag_FiltroCategoria As Boolean

    'Dim SQ_CacheDati As Boolean
    Dim Log_Errori As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub BrogliaccioMovimenti_Init(sender As Object, e As System.EventArgs) Handles Me.Init
        rptBrogliaccio = New Rpt_BrogliaccioMovimenti
    End Sub


    '##############################################################
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

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


        'Qs_Sa_Cod = Stringa_Decodifica(Request.QueryString("s").ToString, _
        '                               AgroKey_EncoderDecoder, _
        '                               Server)

        'If Not IsNothing(Request.QueryString("dest")) Then
        '    Qs_Id_Destinazione = Stringa_Decodifica(Request.QueryString("dest").ToString, _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)
        'Else
        '    Qs_Id_Destinazione = 0
        'End If

        'If Not IsNothing(Request.QueryString("cc")) Then
        '    Qs_Cal_Cod = Stringa_Decodifica(Request.QueryString("cc").ToString, _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)
        'Else
        '    Qs_Cal_Cod = 0
        'End If

        'If Not IsNothing(Request.QueryString("mc")) Then
        '    Qs_Mat_Cod = Stringa_Decodifica(Request.QueryString("mc").ToString, _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)
        'Else
        '    Qs_Mat_Cod = 0
        'End If

        'If Not IsNothing(Request.QueryString("lc")) Then
        '    QS_Linea_Cod = Stringa_Decodifica(Request.QueryString("lc").ToString, _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)
        'Else
        '    QS_Linea_Cod = 0
        'End If

        If Not IsNothing(Request.QueryString("fs")) Then
            QS_Filtro_Stampa = Stringa_Decodifica(Request.QueryString("fs").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)
        Else
            QS_Filtro_Stampa = ""
        End If

        'If Not IsNothing(Request.QueryString("fvr")) Then
        '    Qs_Flag_VerificaRegistri = Stringa_Decodifica(Request.QueryString("fvr").ToString, _
        '                           AgroKey_EncoderDecoder, _
        '                           Server)
        'Else
        '    Qs_Flag_VerificaRegistri = False
        'End If

        'QS_Report = Stringa_Decodifica(Request.QueryString("rp").ToString, _
        '                               AgroKey_EncoderDecoder, _
        '                               Server)

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

        'QS_StampaIntestazione = Stringa_Decodifica(Request.QueryString("si").ToString, _
        '                                           AgroKey_EncoderDecoder, _
        '                                           Server)

        'QS_StampaRiporti = Stringa_Decodifica(Request.QueryString("sr").ToString, _
        '                                      AgroKey_EncoderDecoder, _
        '                                      Server)

        'QS_Cod_Contatto_Terzi = Stringa_Decodifica(Request.QueryString("ct").ToString, _
        '                             AgroKey_EncoderDecoder, _
        '                             Server)

        'If QS_Cod_Contatto_Terzi <> "" Then
        '    Rag_Soc_CTerzi = " " & Stringa_Decodifica(Request.QueryString("ctd").ToString, _
        '                              AgroKey_EncoderDecoder, _
        '                              Server)
        'Else
        '    Rag_Soc_CTerzi = ""
        'End If

        'aggiunto il 03/06/2014 x stampa registro unico, ma voci diversificate x c/terzi
        QS_Gestione_Conto_Terzi = Stringa_Decodifica(Request.QueryString("fct").ToString, _
                                   AgroKey_EncoderDecoder, _
                                   Server)

        'QS_Flag_StampaNumeroVasca = Stringa_Decodifica(Request.QueryString("fnv").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'QS_Flag_StampaCapacitaVasca = Stringa_Decodifica(Request.QueryString("fcv").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'QS_Flag_StampaLottoTrasformazione = Stringa_Decodifica(Request.QueryString("flt").ToString, _
        '                                        AgroKey_EncoderDecoder, _
        '                                        Server)

        'Opt_Gestione_RegistroVinificazione = Stringa_Decodifica(Request.QueryString("rv").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)

        'QS_Cau_Mov = Stringa_Decodifica(Request.QueryString("cm").ToString, _
        '                            AgroKey_EncoderDecoder, _
        '                            Server)


        'Flag_Docg = Stringa_Decodifica(Request.QueryString("fdocg").ToString, _
        '                     AgroKey_EncoderDecoder, _
        '                     Server)

        'Flag_Dop = Stringa_Decodifica(Request.QueryString("fdop").ToString, _
        '                   AgroKey_EncoderDecoder, _
        '                   Server)

        'flag_igp = Stringa_Decodifica(Request.QueryString("figt").ToString, _
        '                   AgroKey_EncoderDecoder, _
        '                   Server)

        'flag_tavola = Stringa_Decodifica(Request.QueryString("ftav").ToString, _
        '                   AgroKey_EncoderDecoder, _
        '                   Server)

        'If flag_docg = True And flag_dop = True And flag_igp = True And flag_tavola = True Then
        '    Flag_FiltroCategoria = False
        'Else
        '    Flag_FiltroCategoria = True
        'End If

        '  SQ_CacheDati = CBool(Request.QueryString("cache"))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '22/03/2017: aumentato il timeout
        objParametri_Server.TimeoutQuery = 3600

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "Brogliaccio_Movimenti"
        Dim strLogRiepilogo As String

        Dim IdentificazioneDocumento As String = ""
        Dim Data_Inizio_Allegati, Data_Fine_Allegati As Date
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_BrogliaccioMovimenti


        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DsVinificazione As New DS_RegistroVinificazione

            Log_Errori = ""

            Try

                'If Qs_Flag_VerificaRegistri = True Then
                '    'sono nella stampa di verifica, non quella ufficiale
                '    CType(rptIntestazione.Section9.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_Filtro_Stampa
                '    rptIntestazione.Section9.SectionFormat.EnableSuppress = False
                'End If

                Dim DataReportInizio As Date
                Dim DataReportFine As Date

                If QS_TipoStampa = 0 Then
                    'stampa mese
                    DataReportInizio = CDate("01/" & QS_Mese & "/" & QS_Anno)
                    DataReportFine = CDate(Date.DaysInMonth(CInt(QS_Anno), CInt(QS_Mese)) & "/" & QS_Mese & "/" & QS_Anno)

                Else
                    'stampa intervallo
                    DataReportInizio = CDate(QS_DataInizio)
                    DataReportFine = CDate(QS_DataFine)
                End If


                'Dim Lista_PrepCod As String = ""
                'Dim Lista_IdTrasf_NoComm As String = ""

                'If Opt_Gestione_RegistroVinificazione = 1 Then


                '    Dim objLineePrep As New AgronicaCoreContabDAL.Linee_Preparazioni_R
                '    Lista_PrepCod = objLineePrep.Lista_PreparazioneCod_xQuery(Qs_Piva, _
                '                                                            enum_Omni_Modulo_Generazione.Cantine, _
                '                                                            enum_Omni_Preparazione_Cod.Passaggio_RegVinificazione_Commercializzazione, _
                '                                                            "", "", _
                '                                                            objParametri_Server)

                '    Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina
                '    Lista_IdTrasf_NoComm = objStampe.Lista_IdTrasformazione_Lotti_NonPassatiACommercializzazione( _
                '                                                            Qs_Piva, _
                '                                                            Lista_PrepCod, _
                '                                                            "", _
                '                                                            objParametri_Server)
                'End If


                ''x debug
                'Dim objCantina As New AgronicaCoreStampeDAL.RegistriCantina
                'Dim str As String = objCantina.SQL_Passaggi_DaARegistro_DaALotto( _
                '                                Qs_Piva, _
                '                                Lista_PrepCod, _
                '                                "", _
                '                                "", "", "", "")
                'Exit Sub

                'Dim objHT_Cali As New Hashtable


                '====================================================
                Carica_BrogliaccioMovimenti(DsVinificazione, _
                                             DataReportInizio, _
                                            DataReportFine)


            Catch ex As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + ex.Message + vbCrLf
            End Try


            '-----------------------------------------
            '---- Gestione Salvataggio PDF -----------  
            '-----------------------------------------
            Try

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
                objGestFile.SalvaReportPdf(rptBrogliaccio, _
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
                rptBrogliaccio.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try


            'MS Close e dispose di Dataset e Report
            DsVinificazione.Dispose()
            DsVinificazione = Nothing


            rptBrogliaccio.Close()
            rptBrogliaccio.Dispose()
            rptBrogliaccio = Nothing

            GC.Collect()

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", " + vbCrLf + _
                     "Piva = " + CStr(Qs_Piva) + ", " + vbCrLf + _
                     "Data inizio = " + CStr(Data_Inizio_Allegati) + ", " + vbCrLf + _
                     "Data Fine = " + CStr(Data_Fine_Allegati) + ", " + vbCrLf + _
                      vbCrLf + vbCrLf + vbCrLf + vbCrLf + _
                     Log_Errori

                'If Rag_Soc_CTerzi <> "" Then
                '    Nome_File = "Log_Errori_" + Nome_Documento + "_clavoro." + Left(Rag_Soc_CTerzi, 20) + "_anno" & QS_Anno + "_mese" + QS_Mese & ".txt"
                'Else
                '    Nome_File = "Log_Errori_" + Nome_Documento + "_" + Qs_RagSoc + "_anno" & QS_Anno + "_mese" + QS_Mese & ".txt"
                'End If

                Nome_File = "Log_Errori_" + Nome_Documento + "_" + Qs_RagSoc + "_data_inizio" & CStr(Data_Inizio_Allegati) + "_data_fine" + CStr(Data_fine_Allegati) & ".txt"


                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                                 "Stampe_Cantine", _
                                                 Nome_File, _
                                                 Session("ASG_Utente_Username"), _
                                                 "BrogliaccioMovimenti.aspx", _
                                                 Log_Errori)



            End If
            '-----------------------------------------


            '==================================================================

            'MS eliminato il passaggio report in session:  Session("Report") = rptIntestazione
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                                  "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                                "&NomePdf=" & Stringa_Codifica(Nome_Documento, AgroKey_EncoderDecoder, Server))


        End If

    End Sub


    '###########################################################################
    Private Sub Carica_BrogliaccioMovimenti(ByRef DsRegistro As DS_RegistroVinificazione, _
                                                 ByVal DataReportInizio As Date, _
                                                ByVal DataReportFine As Date)

        Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina
        Dim RigaDs As DS_RegistroVinificazione.DS_RegistroVinificazioneRow
        Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine
        ' Dim Parametro_FiltroStampa As String = ""

        Try


            ''----------------------------------
            ''GESTIONE NUMERO DI VASCA
            'Dim DT_Vasche As DataTable = Nothing

            ''opzione aggiunta il 24/04/2013
            'If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then

            '    'aggiunto il 15/10/2012
            '    Dim objVasca As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R

            '    Try
            '        DT_Vasche = objVasca.LeggiJoinUdm(Qs_Piva, _
            '                                            0, 0, 0, _
            '                                            "", "", _
            '                                            objParametri_Server)
            '    Catch ex As Exception
            '        Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
            '    End Try

            'End If
            ''----------------------------------

            ''----------------------------------
            ''modifica del 13/10/2014
            ''GESTIONE MOVIMENTIXREPORT (NUM IDONEITA')
            'Dim DT_MovXReport As DataTable = Nothing
            'Dim objMR As New AgronicaCoreContabDAL.MovimentixReport_R

            'Try
            '    Dim DataFineControllo As Date
            '    DataFineControllo = DateAdd(DateInterval.Day, 1, DataReportFine)

            '    'Dim filtroMR As String = " ( Movimenti.Ora >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + _
            '    '                            " AND Movimenti.Ora < " & Agro_SQL_SaveDate(DataFineControllo) & " ) " + _
            '    '                            " AND MovimentixReport.Descrizione <> '' "

            '    Dim filtroMR As String = " ( Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(DataReportInizio) & " " + _
            '                            " AND Movimenti.Data_Movimento < " & Agro_SQL_SaveDate(DataFineControllo) & " ) " + _
            '                            " AND MovimentixReport.Descrizione <> '' "

            '    DT_MovXReport = objMR.LeggiJOINLineePreparazionixReport( _
            '                                            Qs_Piva, _
            '                                            0, 0, QS_Report, _
            '                                            filtroMR, _
            '                                            "", _
            '                                            objParametri_Server)
            'Catch ex As Exception
            '    Log_Errori += "- Lettura di MovimentiXReport: " + vbCrLf + ex.Message + vbCrLf
            'End Try
            ''----------------------------------

            'Dim Lista_CodRisUm As String = ""
            'Dim Lista_IdTrasf_NoComm As String = ""

            'Select Case QS_Gestione_Conto_Terzi
            '    Case enum_RegistroContoTerzi.Nessuno, _
            '       enum_RegistroContoTerzi.RegistroGlobale
            '        '0 = se non c'è esiste conto terzi
            '        '1 = se esiste, ma è stato richiesto registro unico

            '        If QS_Cod_Contatto_Terzi <> "" And QS_Cod_Contatto_Terzi <> Qs_Piva Then

            '            '--- Se sono nella gestione C/Terzi e non ho selezionato l'impresa stessa
            '            '-> devo recuperare l'elenco dei cod_risum del contatto (per filtrare la parte di agenda della query)
            '            Dim objRisUm As New AgronicaCoreAnagrafeDAL.Risorse_Umane_R
            '            Lista_CodRisUm = objRisUm.Lista_CodRisUm_ByChiaveContatto(Qs_Piva, _
            '                                                                    QS_Cod_Contatto_Terzi, _
            '                                                                    "", _
            '                                                                    True, _
            '                                                                    "", _
            '                                                                    objParametri_Server)
            '        End If

            '    Case enum_RegistroContoTerzi.RegistroUnicoDiversificato, _
            '             enum_RegistroContoTerzi.RegistroSeparatoContoTerzi
            '        '2= registro unico ma diversificato per c/lav (MODELLO RUGGERI)
            '        '3 = reg c/terzi separato
            '        Dim objRisUm As New AgronicaCoreContabDAL.Linee_Produzioni_R
            '        Lista_CodRisUm = objRisUm.Lista_CodRisUm_ByCodContattoTerzi(Qs_Piva, _
            '                                                                    "", _
            '                                                                    "", _
            '                                                                    objParametri_Server)
            'End Select
            ''----------------------------------


            'Dim str_idagenda_filtrocategoria As String = ""
            'Dim str_matcod_filtrocategoria As String = ""
            'Dim str_lotto_filtrocategoria As String = ""

            'If Flag_FiltroCategoria = True Then

            '    Parametro_FiltroRegistro = "Registro "
            '    If flag_docg = True Then
            '        Parametro_FiltroRegistro += "DOCG - "
            '    End If
            '    If flag_dop = True Then
            '        Parametro_FiltroRegistro += "DOP - "
            '    End If
            '    If flag_igp = True Then
            '        Parametro_FiltroRegistro += "IGP - "
            '    End If
            '    If flag_tavola = True Then
            '        Parametro_FiltroRegistro += "TAVOLA - "
            '    End If
            '    Parametro_FiltroRegistro = Mid(Parametro_FiltroRegistro, 1, Parametro_FiltroRegistro.Length - 2)

            '    objStampe.PreFiltro_RegistroVinificazione_xCategoria(Qs_Piva, _
            '                                                            QS_Report, _
            '                                                            DataReportInizio, _
            '                                                            DataReportFine, _
            '                                                            flag_docg, _
            '                                                            flag_dop, _
            '                                                            flag_igp, _
            '                                                            flag_tavola, _
            '                                                            str_idagenda_filtrocategoria, _
            '                                                            str_matcod_filtrocategoria, _
            '                                                            str_lotto_filtrocategoria, _
            '                                                            objParametri_Server)

            'End If


            'Dim objHT_Cali As New Hashtable

            ''se è attiva la stampa del registro per categoria
            ''e la query di pre-filtro non ha prodotto risultato
            ''significa che non ci sono dati e quindi non devo neanche chiamare le due query dei registri
            'If Flag_FiltroCategoria = True And str_idagenda_filtrocategoria = "" And str_matcod_filtrocategoria = "" Then
            '    Exit Sub
            'End If


            Dim Dt_Brogl As DataTable
            'Dt_Brogl = objStampe.BrogliaccioMovimenti(Qs_Piva, _
            '                                                DataReportInizio, _
            '                                                DataReportFine, _
            '                                                QS_Cod_Contatto_Terzi, _
            '                                                Lista_CodRisUm, _
            '                                                Qs_Sa_Cod, _
            '                                                Qs_Id_Destinazione, _
            '                                                Qs_Cal_Cod, _
            '                                                Qs_Mat_Cod, _
            '                                                QS_Linea_Cod, _
            '                                                QS_Cau_Mov, _
            '                                                QS_Gestione_Conto_Terzi, _
            '                                                str_idagenda_filtrocategoria, _
            '                                                str_matcod_filtrocategoria, _
            '                                                str_lotto_filtrocategoria, _
            '                                                objParametri_Server)

            Dt_Brogl = objStampe.BrogliaccioMovimenti(Qs_Piva, _
                                                          DataReportInizio, _
                                                          DataReportFine, _
                                                          "", _
                                                          "", _
                                                          0, _
                                                          0, _
                                                          0, _
                                                          0, _
                                                          0, _
                                                          "", _
                                                          QS_Gestione_Conto_Terzi, _
                                                          "", _
                                                          "", _
                                                          "", _
                                                          objParametri_Server)

            If Not IsNothing(Dt_Brogl) AndAlso Dt_Brogl.Rows.Count Then

                Dim Capacita_Effettiva As String = ""
                Dim x_Udm_Sim_Extra As String
                Dim x_Udm_Des_Extra As String
                Dim x_Udm_Cod_Extra As Integer
                Dim DettagliLotto As String = ""
                Dim DettagliVasca As String = ""
                Dim i As Integer
                Dim Descrizione, strDescrizione As String
                Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R

                'l'ordinamento è per:
                'Movimenti.Ora, NDoc, Agenda.Id_Agenda, Movimenti.CAU_MOV DESC, Movimenti_dettagli.Elem_Cod DESC

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente As List(Of Integer) = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                For i = 0 To Dt_Brogl.Rows.Count - 1

                    ''15/10/14: verifica cali -> modificato il 14/07/2015 per gestire anche le altre materie prime
                    'If Dt_Brogl.Rows(i).Item("elem_Cod") = CALI_LAVORAZIONE Or Dt_Brogl.Rows(i).Item("elem_Cod") = ALTRE_MATERIE Then
                    '    'verifico se questa riga di calo o altra materia deve essere stampata
                    '    'magari no, perchè il vino è in vinificazione e quindi è già stato escluso dalla query
                    '    '(il calo non ha la gestione del passaggioa registro e quindi non viene mai scartato,
                    '    'verifico qui da codice)
                    '    Flag_AggiungiRiga = objCantineHLP.Verifica_CaloPerdita_Movimenti(Dt_Brogl, Dt_Brogl.Rows(i).Item("id_agenda"), objHT_Cali, enum_AgroReportistica.Commercializzazione)
                    'End If

                    'Descrizione = Dt_Brogl.Rows(i).Item("Descrizione")
                    Descrizione = Dt_Brogl.Rows(i).Item("des_lib")

                    If InStr(Descrizione, "+ Accettazione") > 0 Then

                        Dim vect_str As String()
                        vect_str = Descrizione.Split("(")

                        Select Case Dt_Brogl.Rows(i).Item("lav_cod")
                            Case LAVCOD_ACCETTAZIONE_DIVERSI
                                Descrizione = "DDT Ricevuto " & Replace(vect_str(2), ")", "")
                            Case LAVCOD_DISTINTA_CARICO_ACCETTAZIONE
                                Descrizione = "Distinta Carico " & Replace(vect_str(2), ")", "")
                            Case LAVCOD_AUTO_DDT_EMESSO_ACCETTAZIONE
                                Descrizione = "Auto DDT " & Replace(vect_str(2), ")", "")
                        End Select

                    End If


                    If Not IsDBNull(Dt_Brogl.Rows(i).Item("StrCampo_Registri")) AndAlso _
                                         Dt_Brogl.Rows(i).Item("StrCampo_Registri") <> String.Empty Then

                        'strDescrizione = objCantineHLP.DescMovXReport_from_IdAgenda(DT_MovXReport, Dt_Brogl.Rows(i).Item("Id_Agenda"))

                        ''Modifico la descrizione del movimento
                        'Descrizione = Descrizione & _
                        '                        " (" & Dt_Brogl.Rows(i).Item("StrCampo_Registri") & " " & _
                        '                        strDescrizione & ")"

                        'Modifico la descrizione del movimento
                        Descrizione = Descrizione & _
                                                " (" & Dt_Brogl.Rows(i).Item("StrCampo_Registri") & " " & _
                                                Dt_Brogl.Rows(i).Item("desc_agg") & ")"

                    End If

                    '----------------------------------
                    'GESTIONE CAPACITA' EFFETTIVA  
                    Capacita_Effettiva = ""
                    'se peso_set > 0 -> allora capacità effettiva (lettura dei campi di movimenti_dettagli)
                    If Dt_Brogl.Rows(i).Item("Peso_Set") > 0 Then

                        'x_Udm_Cod_Extra = Dt_Brogl.Rows(i).Item("udm_cod_extra")

                        'Select Case x_Udm_Cod_Extra
                        '    Case enum_UnitaMisura.KG
                        '        x_Udm_Sim_Extra = "kg"
                        '    Case enum_UnitaMisura.Litri
                        '        x_Udm_Sim_Extra = "l"
                        '    Case Else
                        '        Dim udmcore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                        '        x_Udm_Des_Extra = udmcore.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametri_Server)
                        'End Select

                        x_Udm_Sim_Extra = Dt_Brogl.Rows(i).Item("Udm_Sim_Extra")

                        Capacita_Effettiva = CStr(Dt_Brogl.Rows(i).Item("qta_extra")) + " " + x_Udm_Sim_Extra

                    End If
                    If Capacita_Effettiva <> "" Then
                        Dt_Brogl.Rows(i).Item("Mat_Des") += " " + Capacita_Effettiva
                    End If
                    '----------------------------------


                    '----------------------------------
                    'GESTIONE LOTTO PRODOTTI
                    DettagliLotto = ""
                    DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva,
                                                                    Dt_Brogl.Rows(i).Item("Elem_Cod"),
                                                                    Dt_Brogl.Rows(i).Item("Mat_Cod"),
                                                                    Dt_Brogl.Rows(i).Item("Lotto"),
                                                                    moduliCliente,
                                                                    objParametri_Server)

                    If DettagliLotto <> "" Then
                        Dt_Brogl.Rows(i).Item("Mat_Des") += " " + DettagliLotto
                    End If
                    ''----------------------------------
                    ''GESTIONE NUMERO DI VASCA
                    'If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then
                    '    Try
                    '        DettagliVasca = ""
                    '        If Dt_Brogl.Rows(i).Item("Tipo_Destinazione") = VASCA_ENOLOGICA Then

                    '            DettagliVasca = objCantineHLP.IdentificativoCapacitaVasca_from_Codice( _
                    '                                            QS_Flag_StampaNumeroVasca, _
                    '                                             QS_Flag_StampaCapacitaVasca, _
                    '                                            DT_Vasche, _
                    '                                            Dt_Brogl.Rows(i).Item("SaCod_Dest"), _
                    '                                            Dt_Brogl.Rows(i).Item("Id_Dest"))
                    '        End If
                    '        If DettagliVasca <> "" Then
                    '            Dt_Brogl.Rows(i).Item("Mat_Des") += DettagliVasca
                    '        End If

                    '    Catch ex As Exception
                    '        Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
                    '    End Try
                    'End If
                    ''----------------------------------

                    'nuova riga
                    RigaDs = DsRegistro.DS_RegistroVinificazione.NewDS_RegistroVinificazioneRow

                    RigaDs.Id_Agenda = Dt_Brogl.Rows(i).Item("Id_Agenda")
                    RigaDs.Data_Movimento = Dt_Brogl.Rows(i).Item("Data_Movimento")
                    RigaDs.des_lib = Dt_Brogl.Rows(i).Item("des_lib")
                    RigaDs.Descrizione = Descrizione
                    RigaDs.Designazione = CStr(Dt_Brogl.Rows(i).Item("Azienda") & " - " & Dt_Brogl.Rows(i).Item("Linea_Des")).Trim '& " - " & Dt_Brogl.Rows(i).Item("Lotto_Linea").Trim
                    RigaDs.Cau_Mov = Dt_Brogl.Rows(i).Item("Cau_Mov")
                    RigaDs.Mov_Desc = "" 'Dt_Brogl.Rows(i).Item("Mov_Desc")
                    RigaDs.Elem_Cod = Dt_Brogl.Rows(i).Item("Elem_Cod")
                    RigaDs.Mat_Cod = Dt_Brogl.Rows(i).Item("Mat_Cod")
                    RigaDs.Mov_Det_Des = "" 'Dt_Brogl.Rows(i).Item("Mov_Det_Des")

                    If Dt_Brogl.Rows(i).Item("Udm_Cod") = enum_UnitaMisura.Numero Then
                        RigaDs.Mat_Des = "n." & CStr(Dt_Brogl.Rows(i).Item("Qta")) & " " & Dt_Brogl.Rows(i).Item("Mat_Des")
                    Else
                        RigaDs.Mat_Des = Dt_Brogl.Rows(i).Item("Mat_Des")
                    End If

                    ''modifica del 24/11/2014 (X La spinosa:
                    ''se attiva l'opzione e se l'operazione comporta un'integrazione (separazione consistenze, declassamento, taglio/accorpamento, ecc)
                    ''visualizzo il lotto (trasformazione_des)
                    'If QS_Flag_StampaLottoTrasformazione = True And Dt_Brogl.Rows(i).Item("Tipo_Integrazione") > 0 Then
                    '    RigaDs.Mat_Des = RigaDs.Mat_Des & " - Lotto: " & Dt_Brogl.Rows(i).Item("Lotto")
                    'End If

                    If Dt_Brogl.Rows(i).Item("Tipo_Integrazione") > 0 AndAlso (DettagliLotto = "" OrElse Not RigaDs.Mat_Des.Contains("Lotto")) Then
                        RigaDs.Mat_Des = RigaDs.Mat_Des & " - Lotto: " & Dt_Brogl.Rows(i).Item("Lotto")
                    End If

                    'If Dt_Brogl.Rows(i).Item("Udm_Cod") = enum_UnitaMisura.KG Then
                    '    RigaDs.Mat_Des += " Udm: Kg"
                    'End If

                    RigaDs.Udm_Cod = Dt_Brogl.Rows(i).Item("Udm_Cod")
                    RigaDs.Udm_Sim = Dt_Brogl.Rows(i).Item("Udm_Sim")

                    RigaDs.Cod_Articolo = Dt_Brogl.Rows(i).Item("Cod_Articolo")
                    RigaDs.NDoc = Dt_Brogl.Rows(i).Item("NDoc")
                    RigaDs.CaricoLt = Dt_Brogl.Rows(i).Item("CaricoLt")
                    RigaDs.ScaricoLt = Dt_Brogl.Rows(i).Item("ScaricoLt")

                    RigaDs.ScaricoKg = Dt_Brogl.Rows(i).Item("ScaricoKg")
                    RigaDs.CaricoKg = Dt_Brogl.Rows(i).Item("CaricoKg")

                    'Inserisco la riga
                    DsRegistro.DS_RegistroVinificazione.Rows.Add(RigaDs)

                Next 'righe registro

            End If 'dt

        Catch ex As Exception
            DsRegistro.Clear()
            DsRegistro.AcceptChanges()
            Log_Errori += "- Carica_BrogliaccioMovimenti: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            rptBrogliaccio.SetDataSource(DsRegistro)

        Catch ex As Exception
            Log_Errori += "- Set Dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try
            rptBrogliaccio.SetParameterValue("FiltroStampa", QS_Filtro_Stampa)
            rptBrogliaccio.SetParameterValue("Rag_Soc", Qs_RagSoc + " " + Qs_Piva)

        Catch ex As Exception
            Log_Errori += "- impostazione parametri: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub

End Class


