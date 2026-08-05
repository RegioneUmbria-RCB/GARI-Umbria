Imports CrystalDecisions.Shared
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class RegistroCommercializzazioneVini
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptIntestazione As Rpt_RegistroCommercializzazioneViniIntestazione        ' report contenitore

    Private rptCommercializzazione As Rpt_RegistroCommercializzazioneVini
    Private DsCommercializzazione As DS_RegistroCommercializzazioneVini

    Private rptRiepilogo As Rpt_RiepilogoCommercializzazioneVini
    Private DsRiepilogoCommericializzazione As DS_RiepilogoCommercializzazioneVini

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
    Dim QS_Cau_Mov As String
    Dim QS_Filtro_Stampa As String
    Dim Qs_Flag_VerificaRegistri As Boolean
    Dim QS_Flag_StampaNumeroVasca As Boolean
    Dim QS_Flag_StampaCapacitaVasca As Boolean
    Dim QS_Flag_StampaLottoTrasformazione As Boolean
    Dim Opt_Gestione_RegistroVinificazione As Integer

    'vanni, per cache
    Dim SQ_CacheDati As Boolean
    Dim Log_Errori As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " COMMERCIALIZZAZIONE "

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
        rptCommercializzazione = New Rpt_RegistroCommercializzazioneVini
        rptRiepilogo = New Rpt_RiepilogoCommercializzazioneVini
        rptIntestazione = New Rpt_RegistroCommercializzazioneViniIntestazione
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

        QS_Flag_StampaNumeroVasca = Stringa_Decodifica(Request.QueryString("fnv").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        QS_Flag_StampaCapacitaVasca = Stringa_Decodifica(Request.QueryString("fcv").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        QS_Flag_StampaLottoTrasformazione = Stringa_Decodifica(Request.QueryString("flt").ToString, _
                                                AgroKey_EncoderDecoder, _
                                                Server)

        Opt_Gestione_RegistroVinificazione = Stringa_Decodifica(Request.QueryString("rv").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        QS_Cau_Mov = Stringa_Decodifica(Request.QueryString("cm").ToString, _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        SQ_CacheDati = CBool(Request.QueryString("cache"))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        '22/03/2017: aumentato il timeout
        objParametri_Server.TimeoutQuery = 3600

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "Registro_Commercializzazione"
        Dim strLogRiepilogo As String

        Dim IdentificazioneDocumento As String = ""
        Dim cat_cod As enum_CategorieDocumenti

        cat_cod = enum_CategorieDocumenti.Cantina_RegistroCommercializzazione



        If Not Me.IsPostBack Then

            '--------------------------------------------
            ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
            '--------------------------------------------
            Dim DsCommercializzazioneVini As New DS_RegistroCommercializzazioneVini
            Dim DsRiepilogo As New DS_RiepilogoCommercializzazioneVini

            Log_Errori = ""

            Try

                ' If Qs_Id_Destinazione <> 0 Or Qs_Cal_Cod <> 0 Or Qs_Mat_Cod <> 0 Then
                If Qs_Flag_VerificaRegistri = True Then
                    'sono nella stampa di verifica, non quella ufficiale
                    CType(rptIntestazione.Section9.ReportObjects("TxtFiltro"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = QS_Filtro_Stampa
                    rptIntestazione.Section9.SectionFormat.EnableSuppress = False
                End If

                'Dim DataMeseInizio As Date
                'Dim DataMeseFine As Date
                Dim DataInizio As Date
                Dim DataFine As Date
                Dim DataReportInizio As Date
                Dim DataReportFine As Date
                'Dim DataTreMesiPrima As Date

                If QS_TipoStampa = 0 Then
                    'stampa mese
                    DataReportInizio = CDate("01/" & QS_Mese & "/" & QS_Anno)
                    DataReportFine = CDate(Date.DaysInMonth(CInt(QS_Anno), CInt(QS_Mese)) & "/" & QS_Mese & "/" & QS_Anno)

                Else
                    'stampa intervallo
                    DataReportInizio = CDate(QS_DataInizio)
                    DataReportFine = CDate(QS_DataFine)

                End If

 

                'sono usate dai riporti e dai riepiloghi (x le giacenze)
                DataInizio = AGRODATAINIZIO
                DataFine = DataReportFine

 

                Dim Lista_PrepCod As String = ""
                Dim Lista_IdTrasf_NoComm As String = ""

                If Opt_Gestione_RegistroVinificazione = 1 Then


                    Dim objLineePrep As New AgronicaCoreContabDAL.Linee_Preparazioni_R
                    Lista_PrepCod = objLineePrep.Lista_PreparazioneCod_xQuery(Qs_Piva, _
                                                                            enum_Omni_Modulo_Generazione.Cantine, _
                                                                            enum_Omni_Preparazione_Cod.Passaggio_RegVinificazione_Commercializzazione, _
                                                                            "", "", _
                                                                            objParametri_Server)

                    Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina
                    Lista_IdTrasf_NoComm = objStampe.Lista_IdTrasformazione_Lotti_NonPassatiACommercializzazione( _
                                                                            Qs_Piva, _
                                                                            Lista_PrepCod, _
                                                                            "", _
                                                                            objParametri_Server)
                End If


                ''x debug
                'Dim objCantina As New AgronicaCoreStampeDAL.RegistriCantina
                'Dim str As String = objCantina.SQL_Passaggi_DaARegistro_DaALotto( _
                '                                Qs_Piva, _
                '                                Lista_PrepCod, _
                '                                "", _
                '                                "", "", "", "")
                'Exit Sub

                Dim objHT_Cali As New Hashtable

                '====================================================
                '=== REGISTRO COMMERCIALIZZAZIONE =======
                Carica_DsRegistroCommercializzazione(DsCommercializzazioneVini, _
                                                        DataInizio, _
                                                        DataFine, _
                                                        DataReportInizio, _
                                                        DataReportFine, _
                                                        Opt_Gestione_RegistroVinificazione, _
                                                        Lista_PrepCod, _
                                                        Lista_IdTrasf_NoComm, _
                                                        objHT_Cali)

                '====================================================
                '=== RIEPILOGO REGISTRO COMMERCIALIZZAZIONE =======
                strLogRiepilogo = Carica_DsRiepilogoCommercializzazione(DsRiepilogo, _
                                                                        DataInizio, _
                                                                        DataFine, _
                                                                        DataReportInizio, _
                                                                        DataReportFine, _
                                                                        Opt_Gestione_RegistroVinificazione, _
                                                                        Lista_PrepCod, _
                                                                        Lista_IdTrasf_NoComm, _
                                                                         objHT_Cali)  ', DataTreMesiPrima)


                If SQ_CacheDati Then
                    CacheDati(DsCommercializzazioneVini, DsRiepilogo, DataReportInizio, DataReportFine)
                End If

                If strLogRiepilogo <> String.Empty Then
                    Log_Errori += "- Riepilogo: " + vbCrLf + strLogRiepilogo + vbCrLf
                End If

            Catch ex As Exception
                Log_Errori += "- PageLoad: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try

                If QS_StampaIntestazione = "1" Then
                    rptIntestazione.Section7.SectionFormat.EnableSuppress = True
                    CType(rptIntestazione.Section2.ReportObjects("TxtRagSoc4"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Qs_RagSoc + " " + Qs_Piva
                Else
                    rptIntestazione.Section2.SectionFormat.EnableSuppress = True
                End If

                ' carico i sottoreport
                rptIntestazione.OpenSubreport("Rpt_RegistroCommercializzazioneVini.rpt").SetDataSource(DsCommercializzazioneVini)
                rptIntestazione.OpenSubreport("Rpt_RiepilogoCommercializzazioneVini.rpt").SetDataSource(DsRiepilogo)

            Catch ex As Exception
                Log_Errori += "- Open sottoreport: " + vbCrLf + ex.Message + vbCrLf
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
            DsCommercializzazioneVini.Dispose()
            DsCommercializzazioneVini = Nothing

            DsRiepilogo.Dispose()
            DsRiepilogo = Nothing

            rptCommercializzazione.Close()
            rptCommercializzazione.Dispose()
            rptCommercializzazione = Nothing

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
                                                 "RegistroCommercializzazioneVini.aspx", _
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

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' cache dei dati per passaggio su manager
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[costa]	03/10/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Private Sub CacheDati(ByVal DS_comm As DS_RegistroCommercializzazioneVini, ByVal DS_comm_riepilogo As DS_RiepilogoCommercializzazioneVini, ByVal DataDa As Date, ByVal DataA As Date)

        Dim objCacheMe As New AgronicaCoreStampeDAL.RegistriCantina
        Dim rval As Integer
        
        Dim sa_cod As Integer = CInt(Qs_Sa_Cod)
        Dim Descrizione As String = _
            Qs_Piva & " - " & Qs_RagSoc & " - Periodo di riferimento, dal " & DataDa.ToShortDateString & " al " & DataA.ToShortDateString

        If QS_StampaRiporti Then
            Descrizione &= " - Con Stampa Riporti"
        Else
            Descrizione &= " - Senza Stampa Riporti"
        End If

        rval = objCacheMe.CacheDati_Commercializzazione( _
                    DS_comm, DS_comm_riepilogo, Qs_Piva, Descrizione, sa_cod, DataDa, DataA, objParametri_Server)


    End Sub


    '###########################################################################
    'riempie il ds con i riporti x il Registro di commercializzazione ...
    Private Sub Carica_DsRiportiCommercializzazione(ByRef DsRegistro As DS_RegistroCommercializzazioneVini, _
                                                    ByVal DataInizio As Date, _
                                                    ByVal DataFine As Date, _
                                                    ByVal DataReportInizio As Date, _
                                                    ByVal DataReportFine As Date)

        ''Dim strErr As String
        ''Dim stbQ As New System.Text.StringBuilder
        'Dim Dt As DataTable
        ''Dim objSql As New Codex_Utility.Sql
        'Dim Riga As DS_RegistroCommercializzazioneVini.DS_RegistroCommercializzazioneViniRow

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

        ''Dt = objSql.SqlSelect(objParametri_Server.StringaConnessione, Session("ASG_Connessione_Server"), stbQ.ToString, 1, strErr)

        'Dim objRiporti As New AgronicaCoreStampeDAL.RegistriCantina

        'Dt = objRiporti.RegistroCommercializzazioneRiporti(Qs_Piva, _
        '                                                        QS_Report, _
        '                                                        DataInizio, _
        '                                                        DataFine, _
        '                                                        DataReportInizio, _
        '                                                        DataReportFine, _
        '                                                        objParametri_Server)

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
        '                DsRegistro.DS_RegistroCommercializzazioneVini.AddDS_RegistroCommercializzazioneViniRow(Riga)
        '                ' Riga.Delete()   'elimino la riga prima di crearne una nuova
        '            End If

        '            Riga = DsRegistro.DS_RegistroCommercializzazioneVini.NewDS_RegistroCommercializzazioneViniRow

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
        '            Riga.Lotto = ""
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

        '        Dim strAnnata As String = String.Empty
        '        If AnnoProduzione <> String.Empty Or AliasLotto <> String.Empty Then
        '            strAnnata = IIf(AliasLotto = String.Empty, AnnoProduzione, AliasLotto)
        '        End If

        '        Riga.Mat_Des = Dt.Rows(i).Item("Cal_Des") & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata

        '        CalDes_Old = CalDes
        '        AnnoProduzione_Old = AnnoProduzione

        '        'If AddRiga Then
        '        '    DsRegistro.DS_RegistroCommercializzazioneVini.AddDS_RegistroCommercializzazioneViniRow(Riga)
        '        '    AddRiga = False
        '        'End If

        '        If i = Dt.Rows.Count - 1 Then
        '            DsRegistro.DS_RegistroCommercializzazioneVini.AddDS_RegistroCommercializzazioneViniRow(Riga)
        '        End If

        '    Next

        'End If


        ''Dim Riga As DS_RegistroCommercializzazioneVini.DS_RegistroCommercializzazioneViniRow

        ''Riga = DsRegistro.DS_RegistroCommercializzazioneVini.NewDS_RegistroCommercializzazioneViniRow

        ''Riga.Descrizione = "Riporti"
        ''Riga.CaricoKg = 500
        ''Riga.CaricoLt = 0
        ''Riga.ScaricoLt = 0
        ''Riga.ScaricoKg = 0
        ''Riga.Cau_Mov = 0
        ''Riga.Cod_Articolo = ""
        ''Riga.Data_Movimento = "01/01/2000"
        ''Riga.des_lib = ""
        ''Riga.Designazione = ""
        ''Riga.Elem_Cod = 0
        ''Riga.Id_Agenda = 0
        ''Riga.Lotto = ""
        ''Riga.Mat_Cod = 0
        ''Riga.Mat_Des = ""
        ''Riga.Mov_Desc = ""
        ''Riga.Mov_Det_Des = ""
        ''Riga.NDoc = ""
        ''Riga.Udm_Cod = 0


        ''DsRegistro.DS_RegistroCommercializzazioneVini.AddDS_RegistroCommercializzazioneViniRow(Riga)


    End Sub


    '###########################################################################
    'elenco movimenti commercializzazione
    Private Sub Carica_DsRegistroCommercializzazione(ByRef DsRegistro As DS_RegistroCommercializzazioneVini, _
                                                    ByVal DataInizio As Date, _
                                                    ByVal DataFine As Date, _
                                                    ByVal DataReportInizio As Date, _
                                                    ByVal DataReportFine As Date, _
                                                    ByVal Opt_Gestione_RegistroVinificazione As Integer, _
                                                    ByVal Lista_PrepCod As String, _
                                                    ByRef Lista_IdTrasf_NoComm As String, _
                                                    ByRef objHT_Cali As Hashtable)

        'ByRef DsRegistro As DS_RegistroCommercializzazioneVini, _
        Dim RigaDs As DS_RegistroCommercializzazioneVini.DS_RegistroCommercializzazioneViniRow
        Dim debug As Boolean

        Try

            ''------------------------------------------------
            '' RIPORTI DELLE TRASFORMAZIONI
            ''------------------------------------------------

            'If QS_StampaRiporti = "1" Then
            '    Carica_DsRiportiCommercializzazione(DsRegistro, DataInizio, DataFine, DataReportInizio, DataReportFine)
            'End If

            '----------------------------------
            'GESTIONE NUMERO DI VASCA
            Dim DT_Vasche As DataTable = Nothing
            Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine
            'opzione aggiunta il 24/04/2013
            If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then

                'aggiunto il 15/10/2012
                Dim objVasca As New AgronicaCoreAnagrafeDAL.Cantina_Vasche_R

                Try
                    DT_Vasche = objVasca.LeggiJoinUdm(Qs_Piva, _
                                                        0, 0, 0, _
                                                        "", "", _
                                                        objParametri_Server)
                Catch ex As Exception
                    Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
                End Try

            End If
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


            Dim objCommVini As New AgronicaCoreStampeDAL.RegistriCantina
            'Dim risp As Boolean
            Dim Dt_Comm As DataTable

            'modifica del 15/10/2014: non carico il dataset, ma ritorno il datatable
            'risp = objCommVini.RegistroCommercializzazione(DsRegistro, _
            '                                                DsRegistro.DS_RegistroCommercializzazioneVini.TableName, _
            '                                                Qs_Piva, _
            '                                                QS_Report, _
            '                                                DataReportInizio, _
            '                                                DataReportFine, _
            '                                                QS_Cod_Contatto_Terzi, _
            '                                                Opt_Gestione_RegistroVinificazione, _
            '                                                Lista_PrepCod, _
            '                                                Lista_IdTrasf_NoComm, _
            '                                                Qs_Sa_Cod, _
            '                                                Qs_Id_Destinazione, _
            '                                                Qs_Cal_Cod, _
            '                                                Qs_Mat_Cod, _
            '                                                QS_Linea_Cod, _
            '                                                QS_Cau_Mov, _
            '                                                objParametri_Server)

            Dt_Comm = objCommVini.RegistroCommercializzazione(Nothing, _
                                                            "Comm", _
                                                            Qs_Piva, _
                                                            QS_Report, _
                                                            DataReportInizio, _
                                                            DataReportFine, _
                                                            QS_Cod_Contatto_Terzi, _
                                                            Opt_Gestione_RegistroVinificazione, _
                                                            Lista_PrepCod, _
                                                            Lista_IdTrasf_NoComm, _
                                                            Qs_Sa_Cod, _
                                                            Qs_Id_Destinazione, _
                                                            Qs_Cal_Cod, _
                                                            Qs_Mat_Cod, _
                                                            QS_Linea_Cod, _
                                                            QS_Cau_Mov, _
                                                            objParametri_Server)


            'If risp = True Then
            If Not IsNothing(Dt_Comm) AndAlso Dt_Comm.Rows.Count Then

                'Dim strProdotti As String
                'Dim Dr() As DataRow
                'Dim DtMatDes As New DataTable
                'Dim DrMatDes() As DataRow
                'Dim DrNew As DataRow
                'Dim MatDes As String
                'Dim Identificativo As String
                'DtMatDes.Columns.Add(New DataColumn("Mat_Des", GetType(String)))
                'DtMatDes.Columns.Add(New DataColumn("CaricoLt", GetType(Double)))
                'DtMatDes.Columns.Add(New DataColumn("ScaricoLt", GetType(Double)))
                'DtMatDes.Columns.Add(New DataColumn("IndiceDT", GetType(Integer)))
                'Dim Destinazione_Cod As Integer

                Dim Capacita_Effettiva As String = ""
                Dim x_Udm_Sim_Extra As String
                Dim x_Udm_Des_Extra As String
                Dim x_Udm_Cod_Extra As Integer
                Dim DettagliLotto As String = ""
                Dim DettagliVasca As String = ""
                Dim i As Integer
                Dim strDescrizione As String
                Dim objLotto As New AgronicaCoreAnagrafeDAL.Materie_PrimexLC_R
                Dim Flag_AggiungiRiga As Boolean

                'l'ordinamento è per:
                'Movimenti.Ora, NDoc, Agenda.Id_Agenda, Movimenti.CAU_MOV DESC, Movimenti_dettagli.Elem_Cod DESC

                'Leggo i moduli installati
                Dim objO As New AgronicaCoreContabDAL.OGenerazioni_Anagrafe_Moduli_Log_R
                Dim moduliCliente = objO.Recupera_Moduli_Cliente(Qs_Piva, objParametri_Server)

                'For i = 0 To DsRegistro.DS_RegistroCommercializzazioneVini.Rows.Count - 1
                For i = 0 To Dt_Comm.Rows.Count - 1

                    'ad ogni giro azzero, di default devo inserire
                    Flag_AggiungiRiga = True

                    '15/10/14: verifica cali -> modificato il 14/07/2015 per gestire anche le altre materie prime
                    If Dt_Comm.Rows(i).Item("elem_Cod") = CALI_LAVORAZIONE Or Dt_Comm.Rows(i).Item("elem_Cod") = ALTRE_MATERIE Then
                        'verifico se questa riga di calo o altra materia deve essere stampata
                        'magari no, perchè il vino è in vinificazione e quindi è già stato escluso dalla query
                        '(il calo non ha la gestione del passaggioa registro e quindi non viene mai scartato,
                        'verifico qui da codice)
                        Flag_AggiungiRiga = objCantineHLP.Verifica_CaloPerdita_Movimenti(Dt_Comm, Dt_Comm.Rows(i).Item("id_agenda"), objHT_Cali, enum_AgroReportistica.Commercializzazione)
                    End If


                    If Flag_AggiungiRiga = True Then

                        If Not IsDBNull(Dt_Comm.Rows(i).Item("StrCampo_Registri")) AndAlso _
                                             Dt_Comm.Rows(i).Item("StrCampo_Registri") <> String.Empty Then

                            'modifica del 13/10/2014
                            'strDescrizione = objMR.Descrizione_from_MovimentixReport(Qs_Piva, _
                            '                                                            0, _
                            '                                                            Dt_Comm.Rows(i).Item("Id_Agenda"), _
                            '                                                            QS_Report, _
                            '                                                            "", objParametri_Server)

                            strDescrizione = objCantineHLP.DescMovXReport_from_IdAgenda(DT_MovXReport, Dt_Comm.Rows(i).Item("Id_Agenda"))

                            'Modifico la descrizione del movimento
                            Dt_Comm.Rows(i).Item("Descrizione") = _
                                                Dt_Comm.Rows(i).Item("Descrizione") & _
                                                    " (" & Dt_Comm.Rows(i).Item("StrCampo_Registri") & " " & _
                                                    strDescrizione & ")"

                        End If

                        '----------------------------------
                        'GESTIONE CAPACITA' EFFETTIVA  
                        Capacita_Effettiva = ""
                        'se peso_set > 0 -> allora capacità effettiva (lettura dei campi di movimenti_dettagli)
                        If Dt_Comm.Rows(i).Item("Peso_Set") > 0 Then

                            x_Udm_Cod_Extra = Dt_Comm.Rows(i).Item("udm_cod_extra")

                            Select Case x_Udm_Cod_Extra
                                Case enum_UnitaMisura.KG
                                    x_Udm_Sim_Extra = "kg"
                                Case enum_UnitaMisura.Litri
                                    x_Udm_Sim_Extra = "l"
                                Case Else
                                    Dim udmcore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                                    x_Udm_Des_Extra = udmcore.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametri_Server)

                            End Select

                            Capacita_Effettiva = CStr(Dt_Comm.Rows(i).Item("qta_extra")) + " " + x_Udm_Sim_Extra

                        End If
                        If Capacita_Effettiva <> "" Then
                            Dt_Comm.Rows(i).Item("Mat_Des") += " " + Capacita_Effettiva
                        End If
                        '----------------------------------


                        '----------------------------------
                        'GESTIONE LOTTO PRODOTTI
                        DettagliLotto = ""
                        DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva,
                                                                        Dt_Comm.Rows(i).Item("Elem_Cod"),
                                                                        Dt_Comm.Rows(i).Item("Mat_Cod"),
                                                                        Dt_Comm.Rows(i).Item("Lotto"),
                                                                        moduliCliente,
                                                                        objParametri_Server)

                        If DettagliLotto <> "" Then
                            'Dt_Comm.Rows(i).Item("Descrizione") += " " + DettagliLotto
                            Dt_Comm.Rows(i).Item("Mat_Des") += " " + DettagliLotto
                        End If
                        '----------------------------------
                        'GESTIONE NUMERO DI VASCA
                        'opzione aggiunta il 24/04/2013
                        If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then
                            'aggiunto il 15/10/2012
                            Try
                                DettagliVasca = ""
                                If Dt_Comm.Rows(i).Item("Tipo_Destinazione") = VASCA_ENOLOGICA Then

                                    DettagliVasca = objCantineHLP.IdentificativoCapacitaVasca_from_Codice( _
                                                                    QS_Flag_StampaNumeroVasca, _
                                                                     QS_Flag_StampaCapacitaVasca, _
                                                                    DT_Vasche, _
                                                                    Dt_Comm.Rows(i).Item("SaCod_Dest"), _
                                                                    Dt_Comm.Rows(i).Item("Id_Dest"))
                                End If
                                If DettagliVasca <> "" Then
                                    Dt_Comm.Rows(i).Item("Mat_Des") += DettagliVasca
                                End If

                            Catch ex As Exception
                                Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
                            End Try
                        End If
                        '----------------------------------

                        'nuova riga
                        RigaDs = DsRegistro.DS_RegistroCommercializzazioneVini.NewDS_RegistroCommercializzazioneViniRow

                        RigaDs.Id_Agenda = Dt_Comm.Rows(i).Item("Id_Agenda")
                        RigaDs.Data_Movimento = Dt_Comm.Rows(i).Item("Data_Movimento")
                        RigaDs.des_lib = Dt_Comm.Rows(i).Item("des_lib")
                        RigaDs.Descrizione = Dt_Comm.Rows(i).Item("Descrizione")
                        RigaDs.Designazione = Dt_Comm.Rows(i).Item("Designazione")
                        RigaDs.Cau_Mov = Dt_Comm.Rows(i).Item("Cau_Mov")
                        RigaDs.Mov_Desc = Dt_Comm.Rows(i).Item("Mov_Desc")
                        RigaDs.Elem_Cod = Dt_Comm.Rows(i).Item("Elem_Cod")
                        RigaDs.Mat_Cod = Dt_Comm.Rows(i).Item("Mat_Cod")
                        RigaDs.Mov_Det_Des = Dt_Comm.Rows(i).Item("Mov_Det_Des")
                        RigaDs.Mat_Des = Dt_Comm.Rows(i).Item("Mat_Des")
                        RigaDs.Lotto = Dt_Comm.Rows(i).Item("Lotto")

                        'modifica del 24/11/2014 (X La spinosa:
                        'se attiva l'opzione e se l'operazione comporta un'integrazione (separazione consistenze, declassamento, taglio/accorpamento, ecc)
                        'visualizzo il lotto (trasformazione_des)
                        If QS_Flag_StampaLottoTrasformazione = True And Dt_Comm.Rows(i).Item("Tipo_Integrazione") > 0 Then
                            RigaDs.Mat_Des = RigaDs.Mat_Des & " - Lotto: " & RigaDs.Lotto
                        End If

                        If Dt_Comm.Rows(i).Item("Udm_Cod") = enum_UnitaMisura.KG Then
                            RigaDs.Mat_Des += " Udm: Kg"
                        End If

                        RigaDs.Udm_Cod = Dt_Comm.Rows(i).Item("Udm_Cod")
                        RigaDs.Udm_Sim = Dt_Comm.Rows(i).Item("Udm_Sim")

                        RigaDs.Cod_Articolo = Dt_Comm.Rows(i).Item("Cod_Articolo")
                        RigaDs.NDoc = Dt_Comm.Rows(i).Item("NDoc")
                        RigaDs.CaricoLt = Dt_Comm.Rows(i).Item("CaricoLt")
                        RigaDs.ScaricoLt = Dt_Comm.Rows(i).Item("ScaricoLt")

                        RigaDs.ScaricoKg = 0
                        RigaDs.CaricoKg = 0

                        'Inserisco la riga
                        DsRegistro.DS_RegistroCommercializzazioneVini.Rows.Add(RigaDs)

                    Else
                        debug = True
                    End If 'Flag_AggiungiRiga


                    ''§§§§§§§§§§§§§§§§§§§§§§§§§§
                    ''BACKUP
                    ''For i = 0 To DsRegistro.DS_RegistroCommercializzazioneVini.Rows.Count - 1
                    'For i = 0 To Dt_Comm.Rows.Count - 1

                    '    If Not IsDBNull(DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("StrCampo_Registri")) AndAlso _
                    '           DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("StrCampo_Registri") <> String.Empty Then

                    '        'modifica del 13/10/2014
                    '        'strDescrizione = objMR.Descrizione_from_MovimentixReport(Qs_Piva, _
                    '        '                                                            0, _
                    '        '                                                            DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Id_Agenda"), _
                    '        '                                                            QS_Report, _
                    '        '                                                            "", objParametri_Server)

                    '        strDescrizione = objCantineHLP.DescMovXReport_from_IdAgenda(DT_MovXReport, DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Id_Agenda"))

                    '        'Modifico la descrizione del movimento
                    '        DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Descrizione") = _
                    '                                DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Descrizione") & _
                    '                                 " (" & DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("StrCampo_Registri") & " " & _
                    '                                 strDescrizione & ")"

                    '    End If

                    '    '----------------------------------
                    '    'GESTIONE CAPACITA' EFFETTIVA  
                    '    Capacita_Effettiva = ""
                    '    'se peso_set > 0 -> allora capacità effettiva (lettura dei campi di movimenti_dettagli)
                    '    If DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Peso_Set") > 0 Then

                    '        x_Udm_Cod_Extra = DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("udm_cod_extra")

                    '        Select Case x_Udm_Cod_Extra
                    '            Case enum_UnitaMisura.KG
                    '                x_Udm_Sim_Extra = "kg"
                    '            Case enum_UnitaMisura.Litri
                    '                x_Udm_Sim_Extra = "l"
                    '            Case Else
                    '                Dim udmcore As New AgronicaCoreMetaSchemaDAL.UnitaMisura_R
                    '                x_Udm_Des_Extra = udmcore.UdmDes_from_UdmCod(x_Udm_Cod_Extra, x_Udm_Sim_Extra, objParametri_Server)

                    '        End Select

                    '        Capacita_Effettiva = CStr(DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("qta_extra")) + " " + x_Udm_Sim_Extra

                    '    End If
                    '    If Capacita_Effettiva <> "" Then
                    '        DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Mat_Des") += " " + Capacita_Effettiva
                    '    End If
                    '    '----------------------------------


                    '    '----------------------------------
                    '    'GESTIONE LOTTO PRODOTTI
                    '    DettagliLotto = ""
                    '    DettagliLotto = objLotto.Gestione_LottoProdotto(Qs_Piva, _
                    '                                                    DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Elem_Cod"), _
                    '                                                    DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Mat_Cod"), _
                    '                                                    DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Lotto"), _
                    '                                                    objParametri_Server)

                    '    If DettagliLotto <> "" Then
                    '        'DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Descrizione") += " " + DettagliLotto
                    '        DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Mat_Des") += " " + DettagliLotto
                    '    End If
                    '    '----------------------------------
                    '    'GESTIONE NUMERO DI VASCA
                    '    'opzione aggiunta il 24/04/2013
                    '    If QS_Flag_StampaNumeroVasca = True Or QS_Flag_StampaCapacitaVasca = True Then
                    '        'aggiunto il 15/10/2012
                    '        Try
                    '            DettagliVasca = ""
                    '            If DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Tipo_Destinazione") = VASCA_ENOLOGICA Then

                    '                DettagliVasca = objCantineHLP.IdentificativoCapacitaVasca_from_Codice( _
                    '                                                QS_Flag_StampaNumeroVasca, _
                    '                                                 QS_Flag_StampaCapacitaVasca, _
                    '                                                DT_Vasche, _
                    '                                                DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("SaCod_Dest"), _
                    '                                                DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Id_Dest"))
                    '            End If
                    '            If DettagliVasca <> "" Then
                    '                DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Mat_Des") += DettagliVasca
                    '            End If

                    '        Catch ex As Exception
                    '            Log_Errori += "- Lettura del SERBATOIO/VASCA del vino sfuso: " + vbCrLf + ex.Message + vbCrLf
                    '        End Try
                    '    End If
                    '    '----------------------------------
                    'FINE BACKUP
                    ''§§§§§§§§§§§§§§§§§§§§§§§§§§
                    ''§§§§§§§§§§§§§§§§§§§§§§§§§§

                    '' ciclo sui riporti
                    'If QS_StampaRiporti = "1" Then

                    '    MatDes = DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Mat_Des").ToString

                    '    If CStr(DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("Descrizione")).StartsWith("Riporti") Then

                    '        DrMatDes = DtMatDes.Select("Mat_des = '" & MatDes.Replace("'", "''") & "'")

                    '        If DrMatDes.Length = 0 Then

                    '            DrNew = DtMatDes.NewRow
                    '            DrNew.Item("Mat_des") = MatDes

                    '            If IsDBNull(DrNew.Item("CaricoLt")) Then
                    '                DrNew.Item("CaricoLt") = 0
                    '            End If
                    '            If IsDBNull(DrNew.Item("ScaricoLt")) Then
                    '                DrNew.Item("ScaricoLt") = 0
                    '            End If

                    '            DrNew.Item("CaricoLt") += DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("CaricoLt")
                    '            DrNew.Item("ScaricoLt") += DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("ScaricoLt")

                    '            DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("CaricoLt") = 0
                    '            DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("ScaricoLt") = 0

                    '            ' salvo l'indice del datatable in cui dovrò salvare le qta
                    '            DrNew.Item("IndiceDT") = i

                    '            DtMatDes.Rows.Add(DrNew)

                    '            ' Dr = DsRegistro.DS_RegistroCommercializzazioneVini.Select()
                    '        Else

                    '            ' sicuramente c'è solo una riga con il Mat_Des cercato
                    '            DrMatDes(0).Item("CaricoLt") += DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("CaricoLt")
                    '            DrMatDes(0).Item("ScaricoLt") += DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("ScaricoLt")

                    '            DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("CaricoLt") = 0
                    '            DsRegistro.DS_RegistroCommercializzazioneVini.Rows(i).Item("ScaricoLt") = 0

                    '            DsRegistro.AcceptChanges()

                    '        End If

                    '    End If
                    'End If

                Next 'righe registro

                'If QS_StampaRiporti = "1" Then
                '    Dim index As Integer
                '    For i = 0 To DtMatDes.Rows.Count - 1

                '        index = DtMatDes.Rows(i).Item("IndiceDT")

                '        DsRegistro.DS_RegistroCommercializzazioneVini.Rows(index).Item("CaricoLt") = DtMatDes.Rows(i).Item("CaricoLt")
                '        DsRegistro.DS_RegistroCommercializzazioneVini.Rows(index).Item("ScaricoLt") = DtMatDes.Rows(i).Item("ScaricoLt")

                '    Next
                '    DsRegistro.AcceptChanges()
                'End If

            End If 'dt


        Catch ex As Exception
            DsRegistro.Clear()
            DsRegistro.AcceptChanges()
            Log_Errori += "- Carica_DsRegistroCommercializzazione: " + vbCrLf + ex.Message + vbCrLf
        End Try

    End Sub

    '###########################################################################
    '--- RIEPILOGO -----
    Private Function Carica_DsRiepilogoCommercializzazione(ByRef DsRiepilogo As DS_RiepilogoCommercializzazioneVini, _
                                                           ByVal DataRiepilogoInizio As Date, _
                                                           ByVal DataRiepilogoFine As Date, _
                                                           ByVal DataReportInizio As Date, _
                                                           ByVal DataReportFine As Date, _
                                                           ByVal Opt_Gestione_RegistroVinificazione As Integer, _
                                                            ByVal Lista_PrepCod As String, _
                                                            ByRef Lista_IdTrasf_NoComm As String, _
                                                            ByRef objHT_Cali As Hashtable) As String


        Dim strLog As String

        Dim Dt As DataTable
        Dim DtRiepilogo As DataTable
        Dim DT_Alias As DataTable

        Dim objRiepilogo As New AgronicaCoreStampeDAL.RegistriCantina
        Dim objSaldo As New AgronicaCoreStampeDAL.RegistriCantina
        Dim objCantineHLP As New AgronicaCoreContabHLP.Cantine

        Dim Riga As DS_RiepilogoCommercializzazioneVini.DS_RiepilogoCommercializzazioneViniRow
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
        Dim Udm_Cod As Integer

        Dim strLottiPrec As String
        Dim StrFiltroLottoConfigurazione As String

        Dim Lotto As String = String.Empty
        Dim Lotto_Old As String = String.Empty
        Dim SaldoLotto As Double = 0
        Dim CalcolaSaldo As Boolean = True

        Dim AddRiga As Boolean
        'Dim Flag_ConteggiaLt As Boolean
        Dim CaricoLtDaNonContare, ScaricoLtDaNonContare As Double

        Dim AnnoProduzione As String = String.Empty
        Dim AnnoProduzione_Old As String = String.Empty
        Dim AliasLotto As String = String.Empty

        If DataReportFine > Date.Today Then
            DataReportFine = Date.Today
        End If


        Try

            Dim objAlias As New AgronicaCoreAnagrafeDAL.Lotto_Configurazione_Alias_R
            DT_Alias = objAlias.Leggi(Qs_Piva, _
                                        0, _
                                        0, _
                                        "", _
                                        "", "", _
                                        objParametri_Server)

        Catch ex As Exception
            Log_Errori += "- Lettura degli alias dei lotti: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            DtRiepilogo = objRiepilogo.RegistroCommercializzazioneRiepilogo(Qs_Piva, _
                                                                            QS_Report, _
                                                                            DataRiepilogoInizio, _
                                                                            DataReportInizio, _
                                                                            DataReportFine, _
                                                                            QS_Cod_Contatto_Terzi, _
                                                                            Opt_Gestione_RegistroVinificazione, _
                                                                            Lista_PrepCod, _
                                                                            Lista_IdTrasf_NoComm, _
                                                                            Qs_Sa_Cod, _
                                                                            Qs_Id_Destinazione, _
                                                                            Qs_Cal_Cod, _
                                                                            Qs_Mat_Cod, _
                                                                            QS_Linea_Cod, _
                                                                            QS_Cau_Mov, _
                                                                            objParametri_Server)



        Catch ex As Exception
            Log_Errori += "- Lettura del riepilogo: " + vbCrLf + ex.Message + vbCrLf
        End Try


        If Not IsNothing(DtRiepilogo) Then

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
                Dr = DtRiepilogo.Select("1=1", "Elem_Cod DESC, Cal_Des, Annata DESC, Lotto DESC, Cifra_Start, Cifra_End")

                Dt = DtRiepilogo.Clone

                For i = 0 To Dr.Length - 1
                    Dt.ImportRow(Dr(i))
                Next


                Dim debug As Integer
                For i = 0 To Dt.Rows.Count - 1

                    ''ad ogni giro azzero, default conteggio sempre
                    'Flag_ConteggiaLt = True

                    Cifra_Start = Dt.Rows(i).Item("Cifra_Start")
                    Cifra_End = Dt.Rows(i).Item("Cifra_End")
                    Lotto_Cod = Dt.Rows(i).Item("Lotto_Cod")

                    AnnoProduzione = CStr(Dt.Rows(i).Item("Annata"))

                    CalCod = CInt(Dt.Rows(i).Item("Cal_Cod"))
                    CalDes = CStr(Dt.Rows(i).Item("Cal_Des"))

                    Lotto = CStr(Dt.Rows(i).Item("Lotto"))
                    SaldoLotto = 0

                    Udm_Cod = CStr(Dt.Rows(i).Item("Udm_Cod"))

                    If CalDes <> CalDes_Old Then
                        strLottiPrec = String.Empty
                    End If

                    If CalDes <> CalDes_Old Or AnnoProduzione <> AnnoProduzione_Old Then
                        'primo prodotto o nuovo prodotto                  

                        ' aggiungo la riga vecchia
                        If Not IsNothing(Riga) Then
                            'DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
                            '' Riga.Delete()   'elimino la riga prima di crearne una nuova
                            Aggiungi_Riga(DsRiepilogo, Riga, objSaldo, _
                                            DataRiepilogoInizio, DataReportInizio, _
                                            AnnoProduzione_Old, CalCod_Old, Cifra_Start_Old, Cifra_End_Old, _
                                            Opt_Gestione_RegistroVinificazione, _
                                            Lista_PrepCod, _
                                            Lista_IdTrasf_NoComm, _
                                            "")
                        End If

                        Riga = DsRiepilogo.DS_RiepilogoCommercializzazioneVini.NewDS_RiepilogoCommercializzazioneViniRow

                        Riga.DataFineRiepilogo = "Riepilogo al " + DataReportFine.ToShortDateString & Rag_Soc_CTerzi

                        ' inizializzo i valori della riga
                        Riga.CaricoKg = 0
                        Riga.CaricoLt = 0
                        Riga.ScaricoLt = 0
                        Riga.ScaricoKg = 0
                        Riga.SaldoPrima = 0

                        'AliasLotto = objAlias.LottoAlias_from_LottoVal(Qs_Piva, _
                        '                                                Dt.Rows(i).Item("Elem_Cod"), _
                        '                                                Dt.Rows(i).Item("Lotto_Cod"), _
                        '                                                AnnoProduzione, _
                        '                                                "", "", _
                        '                                                objParametri_Server)

                        AliasLotto = objCantineHLP.LottoAlias_from_LottoVal(DT_Alias, _
                                                                            Qs_Piva, _
                                                                            Dt.Rows(i).Item("Elem_Cod"), _
                                                                            Dt.Rows(i).Item("Lotto_Cod"), _
                                                                             AnnoProduzione)

                        CalcolaSaldo = True

                    End If

                    If Cifra_Start <> Cifra_Start_Old Or Cifra_End <> Cifra_End_Old Then
                        CalcolaSaldo = True
                    End If

                    If CalcolaSaldo = True Then

                        CalcolaSaldo = False

                        'azzero
                        StrFiltroLottoConfigurazione = ""

                        'MODIFICA DEL 29/10/2014
                        'LEONARDI CI HA DETTO CHE NON VANNO CONTEGGIATI I SALDI DEI CALI: LI ESCLUDIAMO DALLA QUERY DI RIEPILOGO
                        'quindi non entra più in questo if


                        '15/10/14: verifica cali -> modificato il 14/07/2015 per gestire le altre materie prime
                        'l'ho messo dentro al  If CalcolaSaldo = True Then
                        'così viene eseguito solo una volta per voce di riepilogo
                        '(altrimenti sottrae la qta per tutte le ripetizioni della voce di riepilogo)
                        If Dt.Rows(i).Item("elem_Cod") = CALI_LAVORAZIONE Or Dt.Rows(i).Item("elem_Cod") = ALTRE_MATERIE Then

                            'If Dt.Rows(i).Item("udm_Cod") = enum_UnitaMisura.Litri Then
                            CaricoLtDaNonContare = 0
                            ScaricoLtDaNonContare = 0
                            objCantineHLP.Verifica_CaloPerdita_Riepilogo(Dt.Rows(i).Item("mat_cod"), objHT_Cali, CaricoLtDaNonContare, ScaricoLtDaNonContare)
                            Riga.CaricoLt += CaricoLtDaNonContare
                            Riga.ScaricoLt += ScaricoLtDaNonContare
                            'Else
                            '    CaricoKGDaNonContare = 0
                            '    ScaricoKGDaNonContare = 0
                            '    objCantineHLP.Verifica_CaoPerdita_Riepilogo(Dt.Rows(i).Item("mat_cod"), objHT_Cali, CaricoKGDaNonContare, ScaricoKGDaNonContare)
                            '    Riga.CaricoKg += CaricoKGDaNonContare
                            '    Riga.ScaricoKg += ScaricoKGDaNonContare
                            'End If

                            ''x i cali non conteggio il saldo precedente
                            'SaldoLottoKg = 0
                            'SaldoLottoLt = 0

                        End If


                        'Else
                        'TUTTE LE ALTRE VOCI CHE NON SONO CALI

                        If Cifra_Start = 0 And Cifra_End = 0 And Lotto_Cod = 0 Then

                            StrFiltroLottoConfigurazione = " AND NOT EXISTS ( " & vbCrLf & _
                                                        "                   SELECT 1 " & vbCrLf & _
                                                        "                   FROM Materie_PrimexLotto_Configurazione  " & vbCrLf & _
                                                        "                   INNER JOIN Lotto_Configurazione  " & vbCrLf & _
                                                        "                   ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf & _
                                                        "                   AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod  " & vbCrLf & _
                                                        "                   AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva  " & vbCrLf & _
                                                        "                   AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod  " & vbCrLf & _
                                                        "                   WHERE  Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod " & vbCrLf & _
                                                        "                   AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod  " & vbCrLf & _
                                                        "                   AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva " & vbCrLf & _
                                                        "                   AND (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%' " & vbCrLf & _
                                                        "                   OR Lotto_Des_Estesa like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%vendemmia%')  " & vbCrLf & _
                                                        "                   ) " & vbCrLf
                        Else
                            StrFiltroLottoConfigurazione = "  AND EXISTS ( " & vbCrLf & _
                                                            "           SELECT 1" & vbCrLf & _
                                                            "           FROM Materie_PrimexLotto_Configurazione " & vbCrLf & _
                                                            "           INNER JOIN Lotto_Configurazione  " & vbCrLf & _
                                                            "           ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf & _
                                                            "           AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod  " & vbCrLf & _
                                                            "           AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva  " & vbCrLf & _
                                                            "           AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod  " & vbCrLf & _
                                                            "           WHERE  Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod  " & vbCrLf & _
                                                            "           AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod  " & vbCrLf & _
                                                            "           AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva " & vbCrLf & _
                                                            "           AND Materie_PrimexLotto_Configurazione.Cifra_Start =  " & Agro_SQL_SaveNum(Cifra_Start) & vbCrLf & _
                                                            "           AND Materie_PrimexLotto_Configurazione.Cifra_End =  " & Agro_SQL_SaveNum(Cifra_End) & vbCrLf & _
                                                            "           AND (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%' " & vbCrLf & _
                                                            "           OR Lotto_Des_Estesa like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%vendemmia%')  " & vbCrLf & _
                                                            "           ) " & vbCrLf
                            'correzione del 26/01/15: 
                            'non si può filtrare lotto_cod, perchè cambia tra bottiglie/condizionati/ecc
                            'uso il join su Lotto_Configurazione e cerco il lotto che corrisponde all'anno di produzione/vendemmia
                            '"           AND Materie_PrimexLotto_Configurazione.Lotto_Cod =  " & Agro_SQL_SaveNum(Lotto_Cod) & vbCrLf & _
                        End If

                        SaldoLotto = objSaldo.RegistroCommercializzazioneSaldoxRiepilogo( _
                                                                         Qs_Piva, _
                                                                         CalCod, _
                                                                         AnnoProduzione, _
                                                                         Cifra_Start, _
                                                                         Cifra_End, _
                                                                         DataRiepilogoInizio, _
                                                                         DataReportInizio, _
                                                                         False, _
                                                                         strLottiPrec, _
                                                                         QS_Cod_Contatto_Terzi, _
                                                                         Opt_Gestione_RegistroVinificazione, _
                                                                          Lista_PrepCod, _
                                                                          Lista_IdTrasf_NoComm, _
                                                                          Qs_Sa_Cod, _
                                                                          Qs_Id_Destinazione, _
                                                                          Qs_Mat_Cod, _
                                                                          QS_Linea_Cod, _
                                                                          StrFiltroLottoConfigurazione, _
                                                                          objParametri_Server)

                        'modifica del 23/01/2015: per gestire anagrafiche con stessa voce di riepilogo
                        'ma gestione anno di produzione diversa

                        ''if aggiunto in data 09/10/2014: se cifra_start e cifra_end sono a 0 o cmq si eguagliano, verrebbe un substring senza senso
                        'If Cifra_End - Cifra_Start <> 0 Then
                        '    strLottiPrec &= " AND SUBSTRING(Movimenti_Dettagli.Lotto, " & Cifra_Start & "," & Cifra_End - Cifra_Start + 1 & ")<>'" & AnnoProduzione & "' " + vbCrLf
                        'End If

                        If Cifra_Start = 0 And Cifra_End = 0 And Lotto_Cod = 0 Then

                            'se non c'è l'annata configurata, è l'ultima riga della stessa voce di riepilogo
                            'perchè sono ordinate in ordine decrescente di anno di produzione
                            'quindi bastano i NOT generati dai giri precedenti

                        Else

                            strLottiPrec &= " AND NOT ( " + vbCrLf

                            strLottiPrec &= " SUBSTRING(Movimenti_Dettagli.Lotto, " & Cifra_Start & "," & Cifra_End - Cifra_Start + 1 & ") = '" & AnnoProduzione & "' " + vbCrLf

                            strLottiPrec &= "  AND EXISTS ( " & vbCrLf & _
                                           "           SELECT 1" & vbCrLf & _
                                           "           FROM Materie_PrimexLotto_Configurazione " & vbCrLf & _
                                           "           INNER JOIN Lotto_Configurazione  " & vbCrLf & _
                                           "           ON Lotto_Configurazione.Piva_SuperUser = Materie_PrimexLotto_Configurazione.Piva_SuperUser   " & vbCrLf & _
                                           "           AND Lotto_Configurazione.Lotto_Cod = Materie_PrimexLotto_Configurazione.Lotto_Cod  " & vbCrLf & _
                                           "           AND Lotto_Configurazione.Piva = Materie_PrimexLotto_Configurazione.Piva  " & vbCrLf & _
                                           "           AND Lotto_Configurazione.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod  " & vbCrLf & _
                                           "           WHERE  Materie_Prime.Elem_Cod = Materie_PrimexLotto_Configurazione.Elem_Cod  " & vbCrLf & _
                                           "           AND Materie_Prime.Mat_Cod = Materie_PrimexLotto_Configurazione.Mat_Cod  " & vbCrLf & _
                                           "           AND Materie_Prime.Piva= Materie_PrimexLotto_Configurazione.Piva " & vbCrLf & _
                                           "           AND Materie_PrimexLotto_Configurazione.Cifra_Start =  " & Agro_SQL_SaveNum(Cifra_Start) & vbCrLf & _
                                           "           AND Materie_PrimexLotto_Configurazione.Cifra_End =  " & Agro_SQL_SaveNum(Cifra_End) & vbCrLf & _
                                           "           AND (Lotto_Des like '%Anno%produzione%' OR Lotto_Des like '%Anno%vendemmia%' " & vbCrLf & _
                                           "           OR Lotto_Des_Estesa like '%Anno%produzione%' OR Lotto_Des_Estesa like '%Anno%vendemmia%')  " & vbCrLf & _
                                           "           ) " & vbCrLf
                            'correzione del 26/01/15: 
                            'non si può filtrare lotto_cod, perchè cambia tra bottiglie/condizionati/ecc
                            'uso il join su Lotto_Configurazione e cerco il lotto che corrisponde all'anno di produzione/vendemmia
                            '"           AND Materie_PrimexLotto_Configurazione.Lotto_Cod =  " & Agro_SQL_SaveNum(Lotto_Cod) & vbCrLf & _

                            strLottiPrec &= " ) -- NOT " & vbCrLf

                        End If

                        'Lotto_Old = Lotto

                        'End If ' calo

                    End If

                    Riga.SaldoPrima += SaldoLotto

                    Riga.CaricoLt += Dt.Rows(i).Item("CaricoLt")
                    Riga.ScaricoLt += Dt.Rows(i).Item("ScaricoLt")

                    Dim strAnnata As String = String.Empty
                    If AnnoProduzione <> String.Empty Or AliasLotto <> String.Empty Then
                        strAnnata = IIf(AliasLotto = String.Empty, AnnoProduzione, AliasLotto)
                    End If

                    'Riga.Linea = Dt.Rows(i).Item("Cal_Des") & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata
                    Riga.Linea = CalDes & IIf(strAnnata <> String.Empty, " - ", String.Empty) & strAnnata

                    If Udm_Cod = enum_UnitaMisura.KG Then
                        Riga.Linea += " Udm: Kg"
                    End If

                    CalCod_Old = CalCod
                    CalDes_Old = CalDes
                    AnnoProduzione_Old = AnnoProduzione
                    Cifra_Start_Old = Cifra_Start
                    Cifra_End_Old = Cifra_End

                    ' se è l'ultima riga la aggiungo
                    If i = Dt.Rows.Count - 1 Then
                        'DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
                        Aggiungi_Riga(DsRiepilogo, Riga, objSaldo, _
                                           DataRiepilogoInizio, DataReportInizio, _
                                           AnnoProduzione, CalCod, Cifra_Start, Cifra_End, _
                                           Opt_Gestione_RegistroVinificazione, _
                                            Lista_PrepCod, _
                                            Lista_IdTrasf_NoComm, _
                                            "")
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
    Private Sub Aggiungi_Riga(ByRef DsRiepilogo As DS_RiepilogoCommercializzazioneVini, _
                                ByVal Riga As DS_RiepilogoCommercializzazioneVini.DS_RiepilogoCommercializzazioneViniRow, _
                                ByRef objSaldo As AgronicaCoreStampeDAL.RegistriCantina, _
                                ByVal DataRiepilogoInizio As Date, _
                                ByVal DataReportInizio As Date, _
                                ByVal AnnoProduzione As String, _
                                ByVal Cal_Cod As Integer, _
                                ByVal Cifra_Start As Integer, _
                                ByVal Cifra_End As Integer, _
                                ByVal Opt_Gestione_RegistroVinificazione As Integer, _
                                ByVal Lista_PrepCod As String, _
                                ByRef Lista_IdTrasf_NoComm As String, _
                                ByVal StrFiltroLottoConfigurazione As String)


        Riga.SaldoDopo = Riga.SaldoPrima + Riga.CaricoLt - Riga.ScaricoLt

        ''non va bene!
        ''perchè elimina i prodotti che hanno saldo 0 e che hanno saldo 0 tre mesi prima, 
        ''anche se sono stati movimentati nell'intervallo corrente
        ''e senza verificare il saldo dei due mesi prima
        'If Riga.SaldoDopo = 0 And SaldoTreMesiPrima = 0 Then
        '    'la giacenza è 0 e anche 3 mesi prima era 0, quindi non la inserisco nel riepilogo
        'Else
        '    'inserisco il prodotto nel riepilogo
        '    DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
        'End If

        'verifico se nell'intervallo corrente il prodotto non è stato caricato, non è stato scaricato e non è in giacenza
        If Riga.CaricoLt = 0 And Riga.ScaricoLt = 0 And Riga.SaldoDopo = 0 Then
            'verifico il saldo al mese prima
            Dim SaldoUnMesePrima As Double
            Dim DataUnMesePrima As Date

            'tolgo 1 giorno
            DataUnMesePrima = DateAdd(DateInterval.Day, -1, DataReportInizio)

            SaldoUnMesePrima = objSaldo.RegistroCommercializzazioneSaldoxRiepilogo( _
                                                                  Qs_Piva, _
                                                                  Cal_Cod, _
                                                                  AnnoProduzione, _
                                                                  Cifra_Start, _
                                                                  Cifra_End, _
                                                                  DataRiepilogoInizio, _
                                                                  DataUnMesePrima, _
                                                                  True, _
                                                                  "", _
                                                                  QS_Cod_Contatto_Terzi, _
                                                                  Opt_Gestione_RegistroVinificazione, _
                                                                    Lista_PrepCod, _
                                                                    Lista_IdTrasf_NoComm, _
                                                                     Qs_Sa_Cod, _
                                                                    Qs_Id_Destinazione, _
                                                                    Qs_Mat_Cod, _
                                                                    QS_Linea_Cod, _
                                                                    StrFiltroLottoConfigurazione, _
                                                                 objParametri_Server)

            'il mese prima era a saldo 0, verifico due mesi prima
            If SaldoUnMesePrima = 0 Then

                Dim SaldoDueMesiPrima As Double
                Dim DataDueMesiPrima As Date

                'tolgo 1 mese
                DataDueMesiPrima = DateAdd(DateInterval.Month, -1, DataReportInizio)
                'tolgo 1 giorno
                DataDueMesiPrima = DateAdd(DateInterval.Day, -1, DataDueMesiPrima)

                SaldoDueMesiPrima = objSaldo.RegistroCommercializzazioneSaldoxRiepilogo( _
                                                      Qs_Piva, _
                                                      Cal_Cod, _
                                                      AnnoProduzione, _
                                                      Cifra_Start, _
                                                      Cifra_End, _
                                                      DataRiepilogoInizio, _
                                                      DataDueMesiPrima, _
                                                      True, _
                                                      "", _
                                                      QS_Cod_Contatto_Terzi, _
                                                      Opt_Gestione_RegistroVinificazione, _
                                                        Lista_PrepCod, _
                                                        Lista_IdTrasf_NoComm, _
                                                         Qs_Sa_Cod, _
                                                        Qs_Id_Destinazione, _
                                                        Qs_Mat_Cod, _
                                                        QS_Linea_Cod, _
                                                        StrFiltroLottoConfigurazione, _
                                                        objParametri_Server)


                'due mesi prima era a saldo 0, verifico tre mesi prima
                If SaldoDueMesiPrima = 0 Then

                    Dim SaldoTreMesiPrima As Double
                    Dim DataTreMesiPrima As Date

                    'tolgo 2 mesi
                    DataTreMesiPrima = DateAdd(DateInterval.Month, -2, DataReportInizio)
                    'tolgo 1 giorno
                    DataTreMesiPrima = DateAdd(DateInterval.Day, -1, DataTreMesiPrima)

                    SaldoTreMesiPrima = objSaldo.RegistroCommercializzazioneSaldoxRiepilogo( _
                                         Qs_Piva, _
                                         Cal_Cod, _
                                         AnnoProduzione, _
                                         Cifra_Start, _
                                         Cifra_End, _
                                         DataRiepilogoInizio, _
                                         DataTreMesiPrima, _
                                         True, _
                                         "", _
                                         QS_Cod_Contatto_Terzi, _
                                         Opt_Gestione_RegistroVinificazione, _
                                         Lista_PrepCod, _
                                         Lista_IdTrasf_NoComm, _
                                          Qs_Sa_Cod, _
                                        Qs_Id_Destinazione, _
                                        Qs_Mat_Cod, _
                                        QS_Linea_Cod, _
                                        StrFiltroLottoConfigurazione, _
                                        objParametri_Server)

                    If SaldoTreMesiPrima = 0 Then
                        'non inserisco nel riepilogo
                    Else
                        'inserisco il prodotto nel riepilogo
                        DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
                    End If
                Else 'saldo due mesi prima
                    'inserisco il prodotto nel riepilogo
                    DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
                End If
            Else 'saldo un mese prima
                'inserisco il prodotto nel riepilogo
                DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
            End If
        Else 'prodotto movimentato nell'intervallo
            'è stato caricato o scaricato o è in giacenza
            'inserisco il prodotto nel riepilogo
            DsRiepilogo.DS_RiepilogoCommercializzazioneVini.AddDS_RiepilogoCommercializzazioneViniRow(Riga)
        End If

    End Sub




End Class
