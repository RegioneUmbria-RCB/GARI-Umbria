Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.Agro_Math
Imports System.Xml

Partial Class BollaAccettazione
    Inherits System.Web.UI.Page


    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptBuonoAccettazione As CRBuonoAccettazione
    Private RptRiepilogoImballiEntrata As Rpt_RiepilogoImballi
    'Private RptRiepilogoImballiUscita As Rpt_RiepilogoImballi
    'Private RptRiepilogoImballiUscita As Rpt_RiepilogoImballiUscita
    Private RptImballiUscita As Rpt_ImballiUscita

    Dim Piva As String
    Dim PrintName As String
    Dim Lav_Cod As Integer
    Dim Qs_Id_Agenda As Integer
    Dim PrintToPrinter As Integer
    'Dim Validita_Inizio, Validita_Fine As String
    'Dim Da_NumeroBolla, A_NumeroBolla As String
    Dim Str_Id_Agenda_Bolle As String
    Dim Numero_Copie As Integer
    Dim Str_Flag_Fascicola As String
    Dim Start_Page As Integer
    Dim End_Page As Integer
    Dim MyRnd As New Random
    Dim Ident_Numero As String = ""
    Dim Ident_Data As String = ""
    Dim Ident_Contatto As String = ""

    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objAgroWebconfig As AgronicaCoreGestioneRichieste.AgroWebConfig


#Region " BOLLA ACCETTAZIONE "

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

        rptBuonoAccettazione = New CRBuonoAccettazione
        RptRiepilogoImballiEntrata = New Rpt_RiepilogoImballi
        RptImballiUscita = New Rpt_ImballiUscita
    End Sub

#End Region


    '#####################################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Qs_Id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        Lav_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")), _
                                    AgroKey_EncoderDecoder, _
                                    Server))

        If Not IsNothing(Request.QueryString("ptp")) Then
            PrintToPrinter = Stringa_Decodifica(CStr(Request.QueryString("ptp")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)
        Else
            PrintToPrinter = 0
        End If

        If Not IsNothing(Request.QueryString("pn")) Then
            PrintName = Stringa_Decodifica(CStr(Request.QueryString("pn")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)
        Else
            PrintName = ""
        End If

        If Not IsNothing(Request.QueryString("nc")) Then
            numero_copie = CInt(Stringa_Decodifica(CStr(Request.QueryString("nc")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))
        Else
            Numero_Copie = 1
        End If

        If Not IsNothing(Request.QueryString("ff")) Then
            Str_Flag_Fascicola = Stringa_Decodifica(CStr(Request.QueryString("ff")), _
                                        AgroKey_EncoderDecoder, _
                                        Server)
        Else
            Str_Flag_Fascicola = "true"
        End If

        If Not IsNothing(Request.QueryString("sp")) Then
            Start_Page = CInt(Stringa_Decodifica(CStr(Request.QueryString("sp")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))
        Else
            Start_Page = 0
        End If

        If Not IsNothing(Request.QueryString("ep")) Then
            End_Page = CInt(Stringa_Decodifica(CStr(Request.QueryString("ep")), _
                                        AgroKey_EncoderDecoder, _
                                        Server))
        Else
            End_Page = 0
        End If

        Str_Id_Agenda_Bolle = Session("Str_Id_Agenda_Bolle")


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objAgroWebconfig = New AgronicaCoreGestioneRichieste.AgroWebConfig


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim IdentificazioneDocumento, NomeFile As String
        Dim Nome_Documento As String = "Bolla_Accettazione"
        Dim Log_Errori As String = ""
        Dim Num_IdAgenda As Integer = 1
        Dim i As Integer
        Dim DT_Bolle As DataTable
        Dim Vet_IdAgenda As String()

        If PrintToPrinter = 1 Then

            If Not IsNothing(Str_Id_Agenda_Bolle.Split("|")) Then
                Vet_IdAgenda = Str_Id_Agenda_Bolle.Split("|")
            Else
                Vet_IdAgenda(0) = Str_Id_Agenda_Bolle
            End If

            Num_IdAgenda = Vet_IdAgenda.Length

        End If


        For i = 0 To Num_IdAgenda - 1

            If PrintToPrinter = 1 Then

                Qs_Id_Agenda = CInt(Vet_IdAgenda(i))

                rptBuonoAccettazione.Close()
                rptBuonoAccettazione.Dispose()

                RptRiepilogoImballiEntrata.Close()
                RptRiepilogoImballiEntrata.Dispose()

                RptImballiUscita.Close()
                RptImballiUscita.Dispose()

                rptBuonoAccettazione = Nothing
                RptRiepilogoImballiEntrata = Nothing
                RptImballiUscita = Nothing

                rptBuonoAccettazione = New CRBuonoAccettazione
                RptRiepilogoImballiEntrata = New Rpt_RiepilogoImballi
                RptImballiUscita = New Rpt_ImballiUscita

            End If

            If Not Me.IsPostBack Then

                '--------------------------------------------
                ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
                '--------------------------------------------
                Dim DS As New DataSetFattura
                Dim DSRiepilogoImballiEntrata As New DS_Iva 'riciclo il dataset dell'iva, per gli imballi in entrata, se no fa casino con quelli in uscita
                Dim DSImballiUscita As New DS_ImballiUscita


                Try

                    Stampa_BollaAccettazione(DS, _
                                            DSRiepilogoImballiEntrata, _
                                            DSImballiUscita, _
                                            Log_Errori, _
                                            Qs_Id_Agenda)

                Catch exc As Exception
                    Log_Errori += "- PageLoad: " + vbCrLf + exc.Message + vbCrLf
                End Try

                Try

                    rptBuonoAccettazione.OpenSubreport("Rpt_RiepilogoImballi.rpt").SetDataSource(DSRiepilogoImballiEntrata)

                    rptBuonoAccettazione.OpenSubreport("Rpt_ImballiUscita.rpt").SetDataSource(DSImballiUscita)

                Catch ex As Exception
                    Log_Errori += "- SetDataSource sottoreport imballi: " + vbCrLf + ex.Message + vbCrLf
                End Try


                Try

                    'IdentificazioneDocumento = Nome_Documento + "_p" & Piva + "_" + Ident_Data & "_" & Ident_Numero & "_" & Ident_Contatto

                    'NomeFile = AgronicaCoreUtility.Stringhe.EliminaCaratteriSpecialiFile(IdentificazioneDocumento) & ".pdf"


                    '' leggo la sottocartella da CategorieDocumenti
                    'Dim Sottocartella As String
                    'Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                    'Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.AccettazioneDaDiversi_Bolla, "", "", objParametri_Server)
                    'objCatDoc = Nothing

                    '' salvo il report in formato PDF
                    'Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                    'objGestFile.SalvaReportPdf(rptBuonoAccettazione, _
                    '                           enum_CategorieDocumenti.AccettazioneDaDiversi_Bolla, _
                    '                           Sottocartella, _
                    '                           NomeFile, _
                    '                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                    'Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                    'Dim AllegatiDocumentiCod As Integer
                    'Dim Data_Inizio_Allegato, Data_Fine_Allegato As Date


                    'AllegatiDocumentiCod = objAllegati.SalvaAllegato(Piva, _
                    '                                                 enum_CategorieDocumenti.AccettazioneDaDiversi_Bolla, _
                    '                                                 "BollaAccettazione", _
                    '                                                 NomeFile, _
                    '                                                 Sottocartella, _
                    '                                                 Qs_Id_Agenda, "", "", "", _
                    '                                                 Data_Inizio_Allegato, _
                    '                                                 Data_Fine_Allegato, _
                    '                                                 objParametri_Server)

                Catch ex As Exception
                    Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
                End Try


                '-----------------------------------------
                '---- Salvataggio Log Errori -------------
                '-----------------------------------------
                Salva_Log(Log_Errori, Nome_Documento, IdentificazioneDocumento)
                '-----------------------------------------

            End If


            '==================================================================

            If PrintToPrinter = 0 Then
                ' per visualizzare l'anteprima, ma non c'è nè il bottone di stampa, nè quello di export
                Session("Report") = rptBuonoAccettazione
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

            Else
                Dim risp As Boolean = False
                Dim Mode_AgroWinSrvc_PrintUtility_PtP As Integer = 1
                Dim PathDatiServizioWinStampa As String = ""
                Dim PathComandiServizioWinStampa As String = ""

                'TO DO 11/02/2021: da debuggare
                Dim objConfigSiti As New AgronicaCoreVarieDAL.Configurazione_Siti_R
                objConfigSiti.Leggi_Config_AgroWinSrvcPrintUtility(objParametri_Server,
                                                                   Mode_AgroWinSrvc_PrintUtility_PtP,
                                                                   PathComandiServizioWinStampa,
                                                                   PathDatiServizioWinStampa)

                Select Case Mode_AgroWinSrvc_PrintUtility_PtP

                    Case 0 'chiamata al servizio windows

                        'TO DO 11/02/2021: verificare se occorrono ancora tutte le righe sotto o se alcune cose sono dei rimasugli vecchi
                        '(quello che ci siamo detti al tel)

                        ' Dichiara le variabili e restituisce le opzioni di esportazione.
                        Dim exportOpts As New ExportOptions
                        Dim diskOpts As New DiskFileDestinationOptions
                        exportOpts = rptBuonoAccettazione.ExportOptions

                        ' Imposta il formato di esportazione.
                        exportOpts.ExportFormatType = ExportFormatType.CrystalReport
                        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

                        ' Imposta le opzioni relative al file del disco.
                        Dim strPath, NomeFileRpt As String

                        If Not PathDatiServizioWinStampa.EndsWith("\") Then PathDatiServizioWinStampa &= "\"

                        NomeFileRpt = Nome_Documento + "_" + MyRnd.Next.ToString() + "_" + CStr(Session("ASG_Utente_Username")) + "_" & Session.SessionID.ToString & ".rpt"

                        strPath = PathDatiServizioWinStampa + NomeFileRpt

                        diskOpts.DiskFileName = strPath
                        exportOpts.DestinationOptions = diskOpts

                        ' Esportazione del report.
                        rptBuonoAccettazione.Export()

                        risp = InviaStampaToServizioUtility(Request, rptBuonoAccettazione, Log_Errori, PrintName, NomeFileRpt,
                                                            PathComandiServizioWinStampa, PathDatiServizioWinStampa, objParametri_Server,
                                                            Numero_Copie, Str_Flag_Fascicola, Start_Page, End_Page)

                        '==========================================================
                    Case 1 'funzione print to printer

                        Try
                            rptBuonoAccettazione.PrintOptions.PrinterName = PrintName

                            rptBuonoAccettazione.PrintToPrinter(Numero_Copie, CBool(Str_Flag_Fascicola), Start_Page, End_Page)

                            risp = True

                        Catch ex As Exception
                            Log_Errori += "Stampa del report non riuscita."
                        End Try

                        '==========================================================
                    Case Else
                        'non gestito
                        Log_Errori += "Il file di configurazione dell'AgronicaStampe non è impostato correttamente."
                        '==========================================================
                End Select


                If risp = True Then
                    'ok
                    Dim a As Integer = 1
                Else
                    '-----------------------------------------
                    '---- Salvataggio Log Errori -------------
                    '-----------------------------------------
                    Salva_Log(Log_Errori, Nome_Documento, IdentificazioneDocumento)
                    '-----------------------------------------
                End If

            End If

        Next

        FileSystem.ChDir("C:\")

        If PrintToPrinter = 1 Then
            Dim strClose As String = "<script language='javascript'>window.close()</script>"
            Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        End If


    End Sub


    '#####################################################################################################
    Private Sub Salva_Log(ByRef Log_Errori As String, _
                          ByVal Nome_Documento As String, _
                          ByVal IdentificazioneDocumento As String)

        Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        'Dim Path_Errore, Nome_File As String

        'If Log_Errori <> "" Then

        '    Log_Errori = Nome_Documento + ", id_agenda = " + CStr(Qs_Id_Agenda) + vbCrLf + vbCrLf + Log_Errori

        '    Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Qs_Id_Agenda)

        '    Dim objLog As New AgronicaCoreDataProvider.LogProvider
        '    If objAgroWeb.PathDirectoryLOG <> "" Then
        '        objParametri_Server.LogDirectory = ""
        '        Path_Errore = objAgroWeb.PathDirectoryLOG & "Stampe_Contabilita"
        '    Else
        '        Path_Errore = "C:\Agronica_LOG\Stampe_Contabilita"
        '    End If

        '    objLog.Scrivi_LOG(Path_Errore, Nome_File, Session("ASG_Utente_Username"), "Fattura_NotaAccredito.Page_Load", Log_Errori)

        'End If

        Dim Nome_File As String

        If Log_Errori <> "" Then

            Log_Errori = Nome_Documento + ", id_agenda = " + CStr(Qs_Id_Agenda) + vbCrLf + vbCrLf + Log_Errori

            Nome_File = "Log_Errori_" + IdentificazioneDocumento + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Qs_Id_Agenda)

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                             "Stampe_Conferimento",
                                             Nome_File & ".txt",
                                             Session("ASG_Utente_Username"),
                                             "BollaAccettazione.aspx",
                                             Log_Errori)

        End If

    End Sub



    '#####################################################################################################
    Private Sub Stampa_BollaAccettazione(ByRef DS As DataSetFattura, _
                                        ByRef DSRiepilogoImballiEntrata As DS_Iva, _
                                        ByRef DSImballiUscita As DS_ImballiUscita, _
                                        ByRef Log_Errori As String, _
                                        ByVal Id_Agenda As Integer)

        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni

        '------------------
        'Impresa
        Dim Indirizzo_Impresa As String
        Dim x_RagSoc_Impresa As String = ""
        Dim x_CodContatto_Impresa As String = ""
        Dim x_CodiceFiscale_Impresa As String = ""
        Dim x_IndDes_Impresa As String = ""
        Dim x_FrzDes_Impresa As String = ""
        Dim x_Cap_Impresa As String = ""
        Dim x_Comune_Impresa As String = ""
        Dim x_Provincia_Impresa As String = ""
        Dim x_RegImprese As String = ""
        Dim x_Provincia_RegImprese As String = ""
        Dim x_Stato_Impresa As String = ""
        Dim x_REA As String = ""
        Dim x_ISO As String = ""
        Dim x_AlboCoop As String = ""
        Dim x_CapitaleSociale As String = ""
        Dim x_Telefono As String = ""
        Dim x_Fax As String = ""
        Dim x_Cell As String = ""
        Dim x_Email As String = ""
        Dim x_SitoWeb As String = ""
        Dim Indirizzo_Fabbricato As String
        Dim x_Fabbricato_Des() As String
        Dim x_IndDes_Fabbricato() As String
        Dim x_FrzDes_Fabbricato() As String
        Dim x_Cap_Fabbricato() As String
        Dim x_Comune_Fabbricato() As String
        Dim x_Provincia_Fabbricato() As String

        '------------------
        'Dati Bolla
        Dim x_Data_Accettazione As Date = #1/1/1900#
        Dim x_Ora_Accettazione As String = ""
        Dim x_Doc_Numero_Sin_Accettazione As String = ""
        Dim x_Doc_Numero_Accettazione As Integer = 0
        Dim x_Doc_Numero_Des_Accettazione As String = ""
        Dim Numero_Bolla As String

        'Dim x_Peso As Double = 0
        Dim x_Peso_Totale As Decimal = 0
        Dim x_Note As String = ""
        Dim x_Tara_Veicolo As Decimal = 0
        'Dim x_Tara_Imballi As Double = 0
        Dim Tara_Imballi As Decimal = 0
        Dim x_Tipo_Peso As Integer = 0
        Dim x_ChkLayout_Join_Prodotti As Integer = 0

        'Dim Peso_Totale As Double = 0
        Dim Peso_Lordo As Double = 0
        Dim Peso_Netto As Double = 0
        Dim Tot_Degrado As Double = 0
        Dim Tot_NettoPagamento As Double = 0

        '------------------
        'Dati DDT Conf
        Dim x_Data_DDTConf As Date = #1/1/1900#
        'Dim x_Ora_DDTConf As String = ""
        Dim x_Colli As Integer = 0
        Dim x_Doc_Numero_Sin_DDTConf As String = ""
        Dim x_Doc_Numero_DDTConf As Integer = 0
        Dim x_Doc_Numero_Des_DDTConf As String = ""
        Dim Numero_DDTConf As String

        ''------------------
        ''Dati DDT Imballi
        'Dim x_Doc_Numero_Sin_DDtImballi As String
        'Dim x_Doc_Numero_DDTImballi As Double
        'Dim x_Doc_Numero_Des_DDTImballi As String
        'Dim Numero_DDTImballi As String

        ''------------------
        'Dim x_Aspetto As String = ""
        'Dim x_Causale_Trasporto As String = ""
        'Dim x_Tipo_Sconto As Integer = 0
        'Dim x_Natura_Beni As String = ""
        'Dim x_Modalita As Integer = 0
        'Dim x_Username_Note As String = ""
        'Dim x_Extra_Str As String = ""
        'Dim x_Extra_Int As Integer = 0
        'Dim x_Extra_Date As Date = #1/1/1900#
        'Dim x_ChkLayout_Bypass_Fatturato As Integer = 0
        'Dim x_Mov_Desc_Accettazione As String = ""
        'Dim x_Mov_Desc_MagazzinoRaccolta As String = ""
        'Dim x_Mov_Desc_DDTConf As String = ""
        'Dim x_Mov_Desc_DDTImballi As String = ""
        'Dim x_Data_Raccolta As Date
        'Dim x_Ora_Raccolta As String
        'Dim x_Progr_Protocollo_DDT As Integer = 0
        'Dim x_Progr_Registrazione_DDT As Integer = 0
        'Dim x_Data_Registrazione_DDT As Date = #1/1/1900#
        'Dim x_Progr_Protocollo_Accettazione As Integer = 0
        'Dim x_Progr_Registrazione_Accettazione As Integer = 0
        'Dim x_Data_Registrazione_Accettazione As Date = #1/1/1900#
        'Dim x_Num_Protocollo As Double = 0

        '------------------
        'Conferente
        Dim Piva_Conferente, CodiceFiscale_Conferente As String
        Dim x_Cod_RisUm As Integer = 0
        Dim x_Cod_Contatto As String = ""
        Dim x_Rag_Soc As String = ""
        Dim x_Codice_Fiscale As String = ""
        Dim x_Cod_IndirizzoRisUm As Integer = 0
        Dim x_Ind_Des As String = ""
        Dim x_Frz_Des As String = ""
        Dim x_Cap As String = ""
        Dim x_Comune As String = ""
        Dim x_Provincia As String = ""
        Dim x_Stato As String = ""

        '------------------
        'Coop
        Dim Piva_Coop, CodiceFiscale_Coop As String
        Dim x_Cod_RisUm_Altro As Integer
        Dim x_Cod_Contatto_Coop As String
        Dim x_Rag_Soc_Coop As String
        Dim x_Codice_Fiscale_Coop As String
        Dim x_Ind_Des_Coop As String = ""
        Dim x_Frz_Des_Coop As String = ""
        Dim x_Cap_Coop As String = ""
        Dim x_Comune_Coop As String = ""
        Dim x_Provincia_Coop As String = ""
        Dim x_Stato_Coop As String = ""

        '------------------
        'seconda Coop
        Dim Piva_Coop_2, CodiceFiscale_Coop_2 As String
        Dim x_Cod_RisUm_Coop_2 As Integer
        Dim x_Cod_Contatto_Coop_2 As String
        Dim x_Rag_Soc_Coop_2 As String
        Dim x_Codice_Fiscale_Coop_2 As String
        Dim x_Ind_Des_Coop_2 As String = ""
        Dim x_Frz_Des_Coop_2 As String = ""
        Dim x_Cap_Coop_2 As String = ""
        Dim x_Comune_Coop_2 As String = ""
        Dim x_Provincia_Coop_2 As String = ""
        Dim x_Stato_Coop_2 As String = ""


        '------------------
        'Produttore
        Dim Piva_Produttore, CodiceFiscale_Produttore As String
        Dim x_Cod_Destinazione As Integer = 0
        Dim x_CodContatto_Destinazione As String = ""
        Dim x_RagSoc_Destinazione As String = ""
        Dim x_CodiceFiscale_Destinazione As String = ""
        Dim x_Cod_IndirizzoDestinazione As Integer = 0
        Dim x_IndDes_Destinazione As String = ""
        Dim x_FrzDes_Destinazione As String = ""
        Dim x_Cap_Destinazione As String = ""
        Dim x_Comune_Destinazione As String = ""
        Dim x_Provincia_Destinazione As String = ""
        Dim x_Stato_Destinazione As String = ""

        '------------------
        'Vettore
        Dim Piva_Vettore, CodiceFiscale_Vettore As String
        Dim x_Mezzo As Integer = 0
        Dim x_Cod_Vettore As Integer = 0
        Dim x_Cod_IndirizzoVettore As Integer = 0
        Dim x_CodContatto_Vettore As String = ""
        Dim x_RagSoc_Vettore As String = ""
        Dim x_CodiceFiscale_Vettore As String = ""
        Dim x_IndDes_Vettore As String = ""
        Dim x_FrzDes_Vettore As String = ""
        Dim x_Cap_Vettore As String = ""
        Dim x_Comune_Vettore As String = ""
        Dim x_Provincia_Vettore As String = ""
        Dim x_Stato_Vettore As String = ""

        '------------------------------
        'Movimenti Dettagli
        Dim Descrizione As String = ""
        Dim Netto_Dettaglio As Double = 0
        Dim Degrado_Perc As Double = 0
        Dim Degrado As Double = 0
        Dim Netto_Pagamento As Double = 0
        Dim Punteggio As String = ""
        Dim Calibro As String = ""
        Dim Unita_Misura As String = ""
        Dim x_Elem_Cod As Integer
        Dim x_ChkLayOut_Hide As Integer = 0
        Dim x_Lotto As String = ""
        Dim x_Udm_Cod As Integer = 0
        Dim x_PesoCampione As Double = 0

        'Dim x_Mov_Det_Des As String = ""
        'Dim x_Elem_Cod As Integer = 0
        'Dim x_Pro_Cod As Integer = 0
        'Dim x_Mat_Cod As Integer = 0
        'Dim x_Cod_Progetto As Integer = 0
        'Dim x_Fase_Cod As Integer = 0
        'Dim x_Cal_Cod As Integer = 0
        'Dim x_Udm_Sim As String = ""
        'Dim x_Udm_Des As String = ""
        'Dim x_Udm_Cod_Extra As Integer = 0
        'Dim x_Udm_Sim_Extra As String = ""
        'Dim x_Udm_Des_Extra As String = ""
        'Dim x_Qta As Double = 0
        'Dim x_Qta_Extra As Double = 0
        'Dim x_Qta_Extra_Totale As Double = 0
        'Dim x_Prezzo_Unitario As Double = 0
        Dim x_Prezzo_Unitario_Netto As Double = 0
        'Dim x_Imponibile As Double = 0
        'Dim x_Imponibile_Netto As Double = 0
        'Dim x_Cod_IVA As Integer = 0
        'Dim x_Aliquota As String = ""
        'Dim x_IVA As Double = 0
        'Dim x_ChkIVA_Manuale As Integer = 0
        'Dim x_Cod_IVAIndetraibile As Integer = 0
        'Dim x_Sconto_Perc As Double = 0
        'Dim x_Sconto As Double = 0
        'Dim x_Prezzo_Effettivo As Double = 0
        'Dim x_Anno As Integer = 0
        'Dim x_Ric_Cod As Integer = 0
        'Dim x_Cod_Conto As Integer = 0
        'Dim x_Conto As String = ""
        'Dim x_Contabilizzato As Integer = 0
        'Dim x_Pendente As Integer = 0
        'Dim x_Tara As Double = 0
        'Dim x_GradoTenderometrico As String = ""
        'Dim x_Extra_Str_Dettagli As String = ""
        'Dim x_Extra_Int_Dettagli As Integer = 0
        'Dim x_Extra_Date_Dettagli As Date = #1/1/1900#
        'Dim x_Veg_Cod As Integer = 0
        'Dim x_Cul_Cod As Integer = 0

        ''------------------
        ''Varie
        'Dim Flag_Raggruppa As Boolean
        'Dim Hash_Gruppo As Hashtable
        'Dim Valore, Chiave As String
        'Dim Qta_Gruppo As Double


        Try

            '############################################################################################
            '####################### INTESTAZIONE DELLA BOLLA DI ACCETTAZIONE ###########################
            '############################################################################################


            Leggi_Intestazione_Impresa(objParametri_Server, _
                                        CInt(Session("ASG_ProgressivoGIAS")), _
                                        False, _
                                        False, _
                                        Piva, _
                                        Log_Errori, _
                                        x_RagSoc_Impresa, _
                                        x_CodContatto_Impresa, _
                                        x_CodiceFiscale_Impresa, _
                                        x_IndDes_Impresa, _
                                        x_FrzDes_Impresa, _
                                        x_Cap_Impresa, _
                                        x_Comune_Impresa, _
                                        x_Provincia_Impresa, _
                                        x_RegImprese, _
                                        x_Provincia_RegImprese, _
                                        x_REA, _
                                        x_ISO, _
                                        x_AlboCoop, _
                                        x_CapitaleSociale, _
                                        x_Telefono, _
                                        x_Fax, _
                                        x_Cell, _
                                        x_Email, _
                                        x_SitoWeb, _
                                        x_Fabbricato_Des, _
                                        x_IndDes_Fabbricato, _
                                        x_FrzDes_Fabbricato, _
                                        x_Cap_Fabbricato, _
                                        x_Comune_Fabbricato, _
                                        x_Provincia_Fabbricato, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        Nothing, _
                                        x_Stato_Impresa)

            Select Case Lav_Cod

                Case LAVCOD_ACCETTAZIONE_DIVERSI


                    '=============================================================
                    '-------------- IMPRESA CHE EMETTE BOLLA  -------------------

                    Indirizzo_Impresa = x_IndDes_Impresa + " " + x_FrzDes_Impresa + " " + x_Cap_Impresa + " " + x_Comune_Impresa + " " + "(" & x_Provincia_Impresa & ")"

                    'CType(rptBolla.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Piva
                    CType(rptBuonoAccettazione.Section2.ReportObjects("TxtPiva"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Partita IVA: " + Piva
                    CType(rptBuonoAccettazione.Section2.ReportObjects("TxtRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_RagSoc_Impresa
                    'CType(rptBolla.Section2.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Indirizzo_Impresa
                    CType(rptBuonoAccettazione.Section2.ReportObjects("TxtIndirizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sede legale: " + Indirizzo_Impresa
                    'CType(rptBolla.Section2.ReportObjects("TxtCF"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_CodiceFiscale_Impresa
                    CType(rptBuonoAccettazione.Section2.ReportObjects("TxtCF"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice Fiscale: " + x_CodiceFiscale_Impresa

                    If x_RegImprese <> "" Then
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtRegImprese"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Iscr. al n. " + x_RegImprese + " del Reg. Imprese Prov. di " + x_Provincia_RegImprese
                    End If
                    If x_REA <> "" Then
                        'CType(rptBolla.Section2.ReportObjects("TxtREA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_REA
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtREA"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "C.C.I.A.A.: " + x_REA
                    End If
                    If x_ISO <> "" Then
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtISO"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Codice ISO: " + x_ISO
                    End If
                    If x_AlboCoop <> "" Then
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtAlboCoop"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "N.Iscr. dell'Albo delle Soc.Coop. a Mutualità Prevalente: " + x_AlboCoop
                    End If
                    If x_CapitaleSociale <> "" Then
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtCapitaleSociale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_CapitaleSociale
                    End If
                    If x_Telefono <> "" Then
                        'CType(rptBolla.Section2.ReportObjects("TxtTelefono"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_Telefono
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtTelefono"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Telefono: " + x_Telefono
                    End If
                    If x_Fax <> "" Then
                        'CType(rptBolla.Section2.ReportObjects("TxtFax"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_Fax
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtFax"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Fax: " + x_Fax
                    End If
                    If x_Email <> "" Then
                        'CType(rptBolla.Section2.ReportObjects("TxtEmail"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_Email
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtEmail"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Email: " + x_Email
                    End If
                    If x_SitoWeb <> "" Then
                        'CType(rptBolla.Section2.ReportObjects("TxtSito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = x_SitoWeb
                        CType(rptBuonoAccettazione.Section2.ReportObjects("TxtSito"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Sito web: " + x_SitoWeb
                    End If

                    Select Case CInt(Session("ASG_ProgressivoGIAS"))

                        Case enum_CodiceGIAS_Clienti.Fruttagel

                            Dim i As Integer
                            For i = 0 To x_Fabbricato_Des.Length - 1

                                Indirizzo_Fabbricato = x_IndDes_Fabbricato(i) + " " + x_FrzDes_Fabbricato(i) + " " + x_Cap_Fabbricato(i) + " " + x_Comune_Fabbricato(i) + " " + "(" & x_Provincia_Fabbricato(i) & ")"

                                If InStr(1, x_Fabbricato_Des(i).ToLower, "alfonsine") > 0 Then
                                    CType(rptBuonoAccettazione.Section2.ReportObjects("TxtIndirizzoAlfonsine"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Stab. di Alfonsine: " + Indirizzo_Fabbricato
                                End If

                                If InStr(1, x_Fabbricato_Des(i).ToLower, "larino") > 0 Then
                                    CType(rptBuonoAccettazione.Section2.ReportObjects("TxtIndirizzoLarino"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Stab. di Larino: " + Indirizzo_Fabbricato
                                End If

                            Next

                    End Select



                    '===================================================================

            End Select

        Catch ex As Exception
            Log_Errori += "- Lettura dei dati dell'intestazione dell'impresa: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '############################################################################################
        '############################ Lettura della Bolla ###########################################
        '############################################################################################

        Dim DT As DataTable
        Dim DrDescrizioneNew As DataSetFattura.DescrizioneRow = DS.Descrizione.NewDescrizioneRow

        Try

            Dim ADD As New AgronicaCoreStampeDAL.FreshAndFood

            DT = ADD.StampaBolla_NuovoConf(Piva, _
                                        Id_Agenda, _
                                        "", "", _
                                        objParametri_Server)


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then

                Dim i As Integer

                For i = 0 To DT.Rows.Count - 1

                    If i = 0 Then

                        With DT.Rows(i)

                            'x_RagSoc_Impresa = .Item("AAA")
                            'x_CodContatto_Impresa = .Item("AAA")
                            'x_CodiceFiscale_Impresa = .Item("AAA")

                            x_Data_Accettazione = .Item("Data_Accett")
                            x_Ora_Accettazione = .Item("Ora_Accett")
                            x_Doc_Numero_Sin_Accettazione = .Item("Doc_Numero_Sin")
                            'x_Doc_Numero_Accettazione = .Item("Doc_Numero")
                            'x_Doc_Numero_Des_Accettazione = .Item("Doc_Numero_Des")
                            'x_Lunghezza_Sin_Accett = .Item("Lunghezza_Sin")
                            'x_Lunghezza_Centro_Accett = .Item("Lunghezza_Centro")
                            'x_Lunghezza_Des_Accett = .Item("Lunghezza_Des")
                            'x_CarattereFormattazione_Accett = .Item("CarattereFormattazione")

                            'Numero_Bolla = Ricava_NumeroDocumento_Con_Sequenza( _
                            '                        x_Doc_Numero_Sin_Accettazione, _
                            '                        x_Doc_Numero_Accettazione, _
                            '                        x_Doc_Numero_Des_Accettazione, _
                            '                        x_Lunghezza_Sin_Accett, _
                            '                        x_Lunghezza_Centro_Accett, _
                            '                        x_Lunghezza_Des_Accett, _
                            '                        x_CarattereFormattazione_Accett)

                            Numero_Bolla = .Item("Doc_Numero_Visualizzato")

                            'x_Note = .Item("Note")
                            x_Note = Replace(If(String.IsNullOrWhiteSpace(.Item("Extra_Str")), .Item("Note"), .Item("Extra_Str")), "?", "€")

                            '=====================================================================
                            ' RIEPILOGO PESO - DIFFERENZE RISPETTO A VECCHIO CONFERIMENTO
                            'questo ora è sempre = 0
                            x_Tipo_Peso = .Item("Tipo_Peso")

                            'questo è sempre il peso totale
                            'x_Peso = .Item("Peso")
                            x_Peso_Totale = .Item("Peso")
                            x_Tara_Veicolo = .Item("Tara_Veicolo")
                            x_Tara_Veicolo = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(x_Tara_Veicolo)

                            'qui ora ci salva sempre 0 (perchè mette solo la tara degli imballi della matrioska)
                            'x_Tara_Imballi = .Item("Tara_Imballi")
                            'x_Tara_Imballi = RoundNumber_ParteIntera(x_Tara_Imballi)

                            ' Peso_Lordo = Calcola_PesoLordo(x_Tipo_Peso, x_Peso, x_Tara_Imballi)
                            Peso_Lordo = x_Peso_Totale - x_Tara_Veicolo
                            Peso_Lordo = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Lordo)

                            'Peso_Totale = RoundNumber_ParteIntera(Peso_Lordo + x_Tara_Veicolo)

                            'Peso_Netto = Calcola_PesoNetto(x_Tipo_Peso, x_Peso, x_Tara_Imballi)
                            'Peso_Netto = RoundNumber_ParteIntera(Peso_Netto)
                            '=====================================================================

                            x_Data_DDTConf = .Item("Data_Conf")
                            'x_Ora_DDTConf = .Item("AAA")
                            x_Doc_Numero_Sin_DDTConf = .Item("Doc_Numero_Sin_Conf")
                            x_Doc_Numero_DDTConf = .Item("Doc_Numero_Conf")
                            x_Doc_Numero_Des_DDTConf = .Item("Doc_Numero_Des_Conf")
                            x_Colli = .Item("Colli")

                            Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                    x_Doc_Numero_Sin_DDTConf, _
                                                    x_Doc_Numero_DDTConf, _
                                                    x_Doc_Numero_Des_DDTConf)

                            'x_Doc_Numero_Sin_DDtImballi = .Item("Doc_Numero_Sin_ImballiEntrata")
                            'x_Doc_Numero_DDTImballi = .Item("Doc_Numero_ImballiEntrata")
                            'x_Doc_Numero_Des_DDTImballi = .Item("Doc_Numero_Des_ImballiEntrata")

                            'Numero_DDTImballi = Ricava_NumeroDocumento_Senza_Sequenza( _
                            '                       x_Doc_Numero_Sin_DDtImballi, _
                            '                       x_Doc_Numero_DDTImballi, _
                            '                       x_Doc_Numero_Des_DDTImballi)

                            'x_Extra_Str = .Item("Extra_Str")
                            'x_Natura_Beni = .Item("Natura_Beni")
                            'x_Aspetto = .Item("AAA")
                            'x_Causale_Trasporto = .Item("AAA")
                            'x_Tipo_Sconto = .Item("AAA")
                            'x_Modalita = .Item("AAA")
                            'x_Username_Note = .Item("AAA")
                            'x_Extra_Int = .Item("AAA")
                            'x_Extra_Date = .Item("AAA")
                            'x_ChkLayout_Bypass_Fatturato = .Item("AAA")
                            x_ChkLayout_Join_Prodotti = .Item("ChkLayout_Join_Prodotti")
                            'If x_ChkLayout_Join_Prodotti = 1 Then
                            '    Flag_Raggruppa = True
                            '    Hash_Gruppo = New Hashtable
                            'Else
                            '    Flag_Raggruppa = False
                            'End If

                            x_Cod_RisUm = .Item("Cod_RisUm")
                            If x_Cod_RisUm <> 0 Then
                                x_Rag_Soc = CStr(.Item("Rag_Soc_Conferente")) + CStr(.Item("Nome_Conferente")) + " " + CStr(.Item("Cognome_Conferente"))
                                x_Cod_Contatto = .Item("Cod_Contatto_Conferente")
                                x_Codice_Fiscale = .Item("Codice_Fiscale_Conferente")
                                Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_Cod_Contatto, x_Codice_Fiscale, Piva_Conferente, CodiceFiscale_Conferente, Nothing)
                                x_Cod_IndirizzoRisUm = .Item("Cod_IndirizzoRisUm")
                                If x_Cod_IndirizzoRisUm <> 0 Then
                                    x_Ind_Des = .Item("ind_des_Conferente")
                                    x_Frz_Des = .Item("frz_des_Conferente")
                                    x_Cap = .Item("cap_Conferente")
                                    x_Comune = .Item("localita_Conferente")
                                    x_Provincia = .Item("comuni_prov_Conferente")
                                    x_Stato = .Item("stato_Conferente")
                                    Sistema_Comune_Provincia(x_Comune, x_Provincia)
                                End If
                            End If

                            Ident_Numero = Numero_Bolla
                            Ident_Contatto = x_Rag_Soc
                            Ident_Data = x_Data_Accettazione.ToShortDateString

                            x_Cod_RisUm_Altro = .Item("Cod_RisUm_Altro")
                            If x_Cod_RisUm_Altro <> 0 Then
                                x_Rag_Soc_Coop = CStr(.Item("Rag_Soc_Coop")) + CStr(.Item("Nome_Coop")) + " " + CStr(.Item("Cognome_Coop"))
                                x_Cod_Contatto_Coop = .Item("Cod_Contatto_Coop")
                                x_Codice_Fiscale_Coop = .Item("Codice_Fiscale_Coop")
                                Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_Cod_Contatto_Coop, x_Codice_Fiscale_Coop, Piva_Coop, CodiceFiscale_Coop, Nothing)
                                x_Ind_Des_Coop = .Item("ind_des_Coop")
                                x_Frz_Des_Coop = .Item("frz_des_Coop")
                                x_Cap_Coop = .Item("cap_Coop")
                                x_Comune_Coop = .Item("localita_Coop")
                                x_Provincia_Coop = .Item("comuni_prov_Coop")
                                x_Stato_Coop = .Item("stato_Coop")
                                Sistema_Comune_Provincia(x_Comune_Coop, x_Provincia_Coop)
                            End If

                            x_Cod_RisUm_Coop_2 = .Item("Extra_Int")
                            If x_Cod_RisUm_Coop_2 <> 0 Then
                                x_Rag_Soc_Coop_2 = CStr(.Item("Rag_Soc_Coop_2")) + CStr(.Item("Nome_Coop_2")) + " " + CStr(.Item("Cognome_Coop_2"))
                                x_Cod_Contatto_Coop_2 = .Item("Cod_Contatto_Coop_2")
                                x_Codice_Fiscale_Coop_2 = .Item("Codice_Fiscale_Coop_2")
                                Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_Cod_Contatto_Coop_2, x_Codice_Fiscale_Coop_2, Piva_Coop_2, CodiceFiscale_Coop_2, Nothing)
                                x_Ind_Des_Coop_2 = .Item("ind_des_Coop_2")
                                x_Frz_Des_Coop_2 = .Item("frz_des_Coop_2")
                                x_Cap_Coop_2 = .Item("cap_Coop_2")
                                x_Comune_Coop_2 = .Item("localita_Coop_2")
                                x_Provincia_Coop_2 = .Item("comuni_prov_Coop_2")
                                x_Stato_Coop_2 = .Item("stato_Coop_2")
                                Sistema_Comune_Provincia(x_Comune_Coop_2, x_Provincia_Coop_2)
                            End If


                            x_Cod_Destinazione = .Item("Cod_Destinazione")
                            If x_Cod_Destinazione <> 0 Then
                                x_RagSoc_Destinazione = CStr(.Item("Rag_Soc_Produttore")) + CStr(.Item("Nome_Produttore")) + " " + CStr(.Item("Cognome_Produttore"))
                                x_CodContatto_Destinazione = .Item("Cod_Contatto_Produttore")
                                x_CodiceFiscale_Destinazione = .Item("Codice_Fiscale_Produttore")
                                Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_CodContatto_Destinazione, x_CodiceFiscale_Destinazione, Piva_Produttore, CodiceFiscale_Produttore, Nothing)
                                x_Cod_IndirizzoDestinazione = .Item("Cod_IndirizzoDestinazione")
                                If x_Cod_IndirizzoDestinazione <> 0 Then
                                    x_IndDes_Destinazione = .Item("Ind_Des_Produttore")
                                    x_FrzDes_Destinazione = .Item("Frz_Des_Produttore")
                                    x_Cap_Destinazione = .Item("Cap_Produttore")
                                    x_Comune_Destinazione = .Item("localita_Produttore")
                                    x_Provincia_Destinazione = .Item("comuni_prov_Produttore")
                                    x_Stato_Destinazione = .Item("Stato_Produttore")
                                    Sistema_Comune_Provincia(x_Comune_Destinazione, x_Provincia_Destinazione)
                                End If
                            End If

                            x_Mezzo = .Item("Mezzo")
                            x_Cod_Vettore = .Item("Cod_Vettore")
                            If x_Cod_Vettore <> 0 Then
                                x_RagSoc_Vettore = CStr(.Item("Rag_Soc_Vettore")) + CStr(.Item("Nome_Vettore")) + " " + CStr(.Item("Cognome_Vettore"))
                                x_CodContatto_Vettore = .Item("Cod_Contatto_Vettore")
                                x_CodiceFiscale_Vettore = .Item("Codice_Fiscale_Vettore")
                                Ricava_Piva_Codicefiscale_VersioneSenzaIDCF(x_CodContatto_Vettore, x_CodiceFiscale_Vettore, Piva_Vettore, CodiceFiscale_Vettore, Nothing)
                                x_Cod_IndirizzoVettore = .Item("Cod_IndirizzoVettore")
                                If x_Cod_IndirizzoVettore <> 0 Then
                                    x_IndDes_Vettore = .Item("Ind_Des_Vettore")
                                    x_FrzDes_Vettore = .Item("Frz_Des_Vettore")
                                    x_Cap_Vettore = .Item("Cap_Vettore")
                                    If x_Cap_Vettore = "00000" Then
                                        x_Cap_Vettore = ""
                                    End If
                                    x_Comune_Vettore = .Item("Localita_Vettore")
                                    x_Provincia_Vettore = .Item("Comuni_Prov_Vettore")
                                    x_Stato_Vettore = .Item("Stato_Vettore")
                                    Sistema_Comune_Provincia(x_Comune_Vettore, x_Provincia_Vettore)
                                End If
                            End If

                            'x_Mov_Desc_MagazzinoRaccolta = .Item("AAA")
                            'x_Mov_Desc_DDTConf = .Item("Mov_Desc_Conf")
                            'x_Mov_Desc_DDTImballi = .Item("AAA")
                            'x_Data_Raccolta = .Item("AAA")
                            'x_Ora_Raccolta = .Item("AAA")

                        End With

                    End If 'primo giro

                    '====================================================================================
                    '---------------- Prelevo le Informazioni sui dettagli ----------------------------

                    'With DT.Rows(i)

                    'Flag_Raggruppa
                    'x_Mov_Det_Des, _
                    'x_Elem_Cod, _
                    'x_Pro_Cod, _
                    'x_Mat_Cod, _
                    'x_Cod_Progetto, _
                    'x_Fase_Cod, _
                    'x_Cal_Cod, _
                    'x_Udm_Cod_Extra, _
                    'x_Udm_Sim_Extra, _
                    'x_Udm_Des_Extra, _
                    'x_Qta_Extra, _
                    'x_Qta_Extra_Totale, _
                    'x_Prezzo_Unitario, _
                    'x_Prezzo_Unitario_Netto, _
                    'x_Imponibile, _
                    'x_Imponibile_Netto, _
                    'x_Cod_IVA, _
                    'x_Aliquota, _
                    'x_IVA, _
                    'x_ChkIVA_Manuale, _
                    'x_Cod_IVAIndetraibile, _
                    'x_Sconto_Perc, _
                    'x_Sconto, _
                    'x_Prezzo_Effettivo, _
                    'x_Anno, _
                    'x_Ric_Cod, _
                    'x_Cod_Conto, _
                    'x_Conto, _
                    'x_Contabilizzato, _
                    'x_Pendente, _
                    'x_Tara, _
                    'x_Extra_Str, _
                    'x_Extra_Int, _
                    'x_Extra_Date, _

                    'End With

                    '===================================================================

                    x_ChkLayOut_Hide = DT.Rows(i).Item("ChkLayOut_Hide")

                    'se x_ChkLayOut_Hide=1 non si vuole visualizzare il dettaglio
                    If x_ChkLayOut_Hide <> 1 Then

                        With DT.Rows(i)

                            x_elem_cod = .Item("Elem_Cod_Raccolta")

                            Select Case x_elem_cod

                                Case RIGA_DESCRIZIONE_LIBERA

                                    Descrizione = .Item("Mov_Det_Des_Raccolta")
                                    Descrizione = Replace(Descrizione, "§", "<br>")
                                    Descrizione = Replace(Descrizione, "?", "€")

                                Case TRASFORMATI_VEGETALI

                                    Descrizione = .Item("Mat_Des_Raccolta")
                                    Degrado_Perc = .Item("Variazione_Raccolta")
                                    Punteggio = .Item("punteggio")
                                    Calibro = .Item("Calibro")
                                    'Quantita = .Item("Qta_Raccolta")
                                    Netto_Dettaglio = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("qta_extra_totale_Raccolta"))
                                    Peso_Netto += Netto_Dettaglio
                                    x_Udm_Cod = .Item("Udm_Cod_Raccolta")
                                    Unita_Misura = .Item("Udm_Sim_Raccolta")
                                    x_Lotto = .Item("Lotto_raccolta")
                                    x_Prezzo_Unitario_Netto = .Item("Prezzo_Unitario_Netto")
                                    'x_PesoCampione = .Item("Peso_Campione")
                                    objADDFun.Calcola_NettoPagamento(Netto_Dettaglio, Degrado_Perc, x_Udm_Cod, Degrado, Netto_Pagamento)

                                    Tot_Degrado += Degrado
                                    Tot_NettoPagamento += Netto_Pagamento

                                Case BENI_CONFEZ_VEGETALE

                            End Select

                        End With

                        ''se si vuole visualizzare i dettagli così come sono
                        'If x_ChkLayout_Join_Prodotti <> 1 Then

                        '------------------------------------
                        '------ VISUALIZZAZIONE NORMALE -----
                        '------------------------------------

                        DrDescrizioneNew = DS.Descrizione.NewDescrizioneRow

                        DrDescrizioneNew.Contatore = i

                        DrDescrizioneNew.Descrizione = Descrizione

                        'RICICLO IL CAMPO SCONTO PER IL DEGRADO
                        DrDescrizioneNew.Sconto = CStr(Degrado_Perc) + "%"

                        ''RICICLO IL CAMPO SUPERPIVA PER IL PESO CAMPIONE -> che non visualizzo
                        'DrDescrizioneNew.SuperPiva = CStr(x_PesoCampione) + " (" + Unita_Misura + ")"

                        If Punteggio <> "0" Then
                            DrDescrizioneNew.SuperPiva = Punteggio
                        End If

                        'RICICLO IL CAMPO IVA PER IL CALIBRO E GRADO TENDEROMETRICO
                        If Calibro = "0" Then
                            Calibro = ""
                        End If
                        DrDescrizioneNew.Iva = Calibro

                        '16/02/2021: richiesta 2021 di stampare sulla riga il peso netto a pagamento al posto del peso netto
                        'DrDescrizioneNew.Qta = Format(Netto_Dettaglio, "#,###,##0")
                        DrDescrizioneNew.Qta = Format(Netto_Pagamento, "#,###,##0")

                        DrDescrizioneNew.Udm_Des = Unita_Misura

                        DrDescrizioneNew.Prezzo_Netto = Format(x_Prezzo_Unitario_Netto, "#,###,##0.00###")

                        DS.Descrizione.Rows.Add(DrDescrizioneNew)

                        'Else 'bisogna raggruppare i dettagli

                        '    '------------------------------------
                        '    '------ RAGGRUPPAMENTO DETTAGLI -----
                        '    '------------------------------------

                        '    'raggruppo per specie- varietà - calibro (non posso usare il codice perchè è un progressivo) - unità misura - prezzo - sconto
                        '    Chiave = CStr(x_Veg_Cod) + "|" + CStr(x_Cul_Cod) + "|" + Nome_Calibro + "|" + CStr(x_Udm_Cod) + "|" + CStr(x_Prezzo_Unitario) + "|" + CStr(x_Sconto_Perc)

                        '    'se non è già presente
                        '    If Not Hash_Gruppo.Contains(Chiave) Then

                        '        Valore = x_Descrizione + "|" + x_Udm_Sim + "|" + CStr(x_Qta)

                        '        'inserisco il dettaglio
                        '        Hash_Gruppo.Add(Chiave, Valore)

                        '    Else
                        '        'il dettaglio è già presente
                        '        'devo incrementare la quantità

                        '        'prelevo la quantità del dettaglio al momento salvata
                        '        Qta_Gruppo = CDbl(CStr(Hash_Gruppo.Item(Chiave)).Split("|")(2))

                        '        'aggiungo la qta del dettaglio ripetuto
                        '        Qta_Gruppo += x_Qta

                        '        'preparo il nuovo valore
                        '        Valore = x_Descrizione + "|" + x_Udm_Sim + "|" + CStr(Qta_Gruppo)

                        '        'aggiorno il valore
                        '        Hash_Gruppo.Item(Chiave) = Valore

                        '    End If

                        'End If

                    End If

                    '===================================================================

                Next

                Peso_Netto = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Netto)

            End If

        Catch ex As Exception
            Log_Errori += "- Elaborazione dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


        '############################################################################################
        '############################ Print Dati Bolla ##############################################
        '############################################################################################

        Dim DrIntestazioneNew As DataSetFattura.IntestazioneFatturaRow = DS.IntestazioneFattura.NewIntestazioneFatturaRow

        Try

            '=============================================================
            '----------------------- DATI BOLLA --------------------------

            'per la visualizzazione del logo viene letto questo campo
            DrIntestazioneNew.SuperPiva = Piva

            'riciclo il campo aspetto, poichè il campo data_Movimento viene visto come data e non come stringa
            'e le date vengono visualizzate in formato americano da Fruttagel
            DrIntestazioneNew.Aspetto = Format(x_Data_Accettazione, "dd/MM/yyyy")
            DrIntestazioneNew.Doc_Numero = Numero_Bolla
            DrIntestazioneNew.NSBanca = CType(x_Data_DDTConf, DateTime)
            DrIntestazioneNew.VSBanca = Numero_DDTConf
            DrIntestazioneNew.Note = x_Note
            DrIntestazioneNew.Colli = x_Colli

            Select Case CInt(Session("ASG_ProgressivoGIAS"))

                Case enum_CodiceGIAS_Clienti.Fruttagel

                    Select Case Right(x_Doc_Numero_Sin_Accettazione, 1)
                        'se il prefisso termina con 1 è alfonsine
                        Case "1"
                            CType(rptBuonoAccettazione.Section2.ReportObjects("TxtTitoloBolla"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "BOLLA DI ACCETTAZIONE: ALFONSINE"
                            'se il prefisso termina con 7 è larino
                        Case "7"
                            CType(rptBuonoAccettazione.Section2.ReportObjects("TxtTitoloBolla"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "BOLLA DI ACCETTAZIONE: LARINO"
                    End Select

            End Select

            '=============================================================
            '------------------------ VETTORE --------------------------
            'è il trasporto a cura di: Cedente [0], Cessionario [1], Vettore [2]
            Select Case x_Mezzo
                Case enum_TrasportoACaricoDi.Cedente
                    DrIntestazioneNew.Mezzo = "CEDENTE"
                Case enum_TrasportoACaricoDi.Cessionario
                    DrIntestazioneNew.Mezzo = "CESSIONARIO"
                Case enum_TrasportoACaricoDi.Vettore
                    DrIntestazioneNew.Mezzo = "VETTORE"
                    DrIntestazioneNew.VettoreRag_Soc = x_RagSoc_Vettore
                    DrIntestazioneNew.VettorePiva = Piva_Vettore
                    DrIntestazioneNew.VettoreInd_Des = x_IndDes_Vettore
                    DrIntestazioneNew.VettoreFrz_Des = x_FrzDes_Vettore
                    DrIntestazioneNew.VettoreCap = x_Cap_Vettore
                    DrIntestazioneNew.VettoreComune = x_Comune_Vettore
                    If x_Provincia_Vettore <> "" Then
                        DrIntestazioneNew.VettoreProvincia = "(" & x_Provincia_Vettore & ")"
                    Else
                        DrIntestazioneNew.VettoreProvincia = ""
                    End If
            End Select

            'Select Case x_Cod_Vettore

            '    Case 0
            '        Select Case Lav_Cod
            '            Case LAVCOD_ACCETTAZIONE_DIVERSI
            '                If x_Mezzo = 0 Then
            '                    DrIntestazioneNew.Mezzo = "CONFERENTE"
            '                Else
            '                    DrIntestazioneNew.Mezzo = "DESTINATARIO"
            '                End If
            '        End Select

            '    Case Else
            '        DrIntestazioneNew.Mezzo = "VETTORE"
            '        DrIntestazioneNew.VettoreRag_Soc = x_RagSoc_Vettore
            '        DrIntestazioneNew.VettorePiva = Piva_Vettore
            '        DrIntestazioneNew.VettoreInd_Des = x_IndDes_Vettore
            '        DrIntestazioneNew.VettoreFrz_Des = x_FrzDes_Vettore
            '        DrIntestazioneNew.VettoreCap = x_Cap_Vettore
            '        DrIntestazioneNew.VettoreComune = x_Comune_Vettore
            '        If x_Provincia_Vettore <> "" Then
            '            DrIntestazioneNew.VettoreProvincia = "(" & x_Provincia_Vettore & ")"
            '        Else
            '            DrIntestazioneNew.VettoreProvincia = ""
            '        End If
            'End Select


            '/////////////////////////////////////////////////////////////

            Select Case Lav_Cod


                Case LAVCOD_ACCETTAZIONE_DIVERSI

                    '-------------------------------------------------------------
                    '--------------- ACCETTAZIONE DA DIVERSI ---------------------
                    '-------------------------------------------------------------

                    '=============================================================
                    '------------------- CONFERENTE  ---------------------------

                    DrIntestazioneNew.Rag_Soc = x_Rag_Soc
                    DrIntestazioneNew.Piva = "Partita IVA: " + Piva_Conferente '+ " - Codice Fiscale: " + CodiceFiscale_Conferente
                    If CodiceFiscale_Conferente <> "" Then
                        'riciclo il campo Scadenza
                        DrIntestazioneNew.Scadenza = "Codice Fiscale: " + CodiceFiscale_Conferente
                    End If
                    DrIntestazioneNew.Ind_Des = x_Ind_Des
                    DrIntestazioneNew.Frz_Des = x_Frz_Des
                    DrIntestazioneNew.Cap = x_Cap
                    DrIntestazioneNew.Comune = x_Comune
                    If x_Provincia <> "" Then
                        DrIntestazioneNew.Provincia = "(" & x_Provincia & ")"
                    End If


                    '=============================================================
                    '--------------------- COOPERATIVA --------------------------

                    If x_Cod_RisUm_Altro <> 0 Then

                        'riciclo i campi superuser e telefono
                        DrIntestazioneNew.SuperRag_Soc = x_Rag_Soc_Coop
                        DrIntestazioneNew.Telefono = "Partita IVA: " + Piva_Coop '+ " - Codice Fiscale: " + CodiceFiscale_Coop
                        If CodiceFiscale_Coop <> "" Then
                            'riciclo il campo Tipo_Pagamento
                            DrIntestazioneNew.Tipo_Pagamento = "Codice Fiscale: " + CodiceFiscale_Coop
                        End If
                        DrIntestazioneNew.SuperInd_Des = x_Ind_Des_Coop
                        DrIntestazioneNew.SuperFrz_Des = x_Frz_Des_Coop
                        DrIntestazioneNew.SuperCap = x_Cap_Coop
                        DrIntestazioneNew.SuperComune = x_Comune_Coop
                        If x_Provincia_Coop <> "" Then
                            DrIntestazioneNew.SuperProvincia = "(" & x_Provincia_Coop & ")"
                        End If

                    End If

                    '=============================================================

                    '=============================================================
                    '--------------------- COOPERATIVA 2 --------------------------

                    If x_Cod_RisUm_Coop_2 <> 0 Then

                        DrIntestazioneNew.Coop2_Rag_Soc = x_Rag_Soc_Coop_2
                        DrIntestazioneNew.Coop2_Piva = "Partita IVA: " + Piva_Coop_2
                        If CodiceFiscale_Coop_2 <> "" Then
                            DrIntestazioneNew.Coop2_CodFisc = "Codice Fiscale: " + CodiceFiscale_Coop_2
                        End If
                        DrIntestazioneNew.Coop2_Ind_Des = x_Ind_Des_Coop_2
                        DrIntestazioneNew.Coop2_Frz_Des = x_Frz_Des_Coop_2
                        DrIntestazioneNew.Coop2_Cap = x_Cap_Coop_2
                        DrIntestazioneNew.Coop2_Comune = x_Comune_Coop_2
                        If x_Provincia_Coop_2 <> "" Then
                            DrIntestazioneNew.Coop2_Provincia = "(" & x_Provincia_Coop_2 & ")"
                        End If

                    End If

                    '=============================================================


                    '=============================================================
                    '--------------------- PRODUTTORE --------------------------

                    If x_Cod_Destinazione <> 0 Then

                        DrIntestazioneNew.DestinazioneRag_Soc = x_RagSoc_Destinazione
                        DrIntestazioneNew.DestinazionePiva = "Partita IVA: " + Piva_Produttore
                        If CodiceFiscale_Produttore <> "" Then
                            'riciclo il campo causale_trasporto
                            'DrIntestazioneNew.DestinazionePiva += " - Codice Fiscale: " + CodiceFiscale_Produttore
                            DrIntestazioneNew.Causale_Trasporto = "Codice Fiscale: " + CodiceFiscale_Produttore
                        End If

                        If x_Cod_IndirizzoDestinazione <> 0 Then

                            DrIntestazioneNew.DestinazioneInd_Des = x_IndDes_Destinazione
                            DrIntestazioneNew.DestinazioneFrz_Des = x_FrzDes_Destinazione
                            DrIntestazioneNew.DestinazioneCap = x_Cap_Destinazione
                            DrIntestazioneNew.DestinazioneComune = x_Comune_Destinazione
                            If x_Provincia_Destinazione <> "" Then
                                DrIntestazioneNew.DestinazioneProvincia = "(" & x_Provincia_Destinazione & ")"
                            End If

                        End If

                    End If

            End Select

            '####################################################

            DS.IntestazioneFattura.Rows.Add(DrIntestazioneNew)

            '####################################################

            ''se impostato il raggruppamento
            'If x_ChkLayout_Join_Prodotti = 1 Then

            '    Dim Key As Object
            '    Dim i As Integer = 0

            '    For Each Key In Hash_Gruppo.Keys

            '        DrDescrizioneNew = DS.Descrizione.NewDescrizioneRow

            '        DrDescrizioneNew.Contatore = i

            '        DrDescrizioneNew.SuperPiva = Piva

            '        DrDescrizioneNew.Descrizione = CStr(Hash_Gruppo.Item(Key)).Split("|")(0)

            '        DrDescrizioneNew.Udm_Des = CStr(Hash_Gruppo.Item(Key)).Split("|")(1)

            '        DrDescrizioneNew.Qta = CStr(Hash_Gruppo.Item(Key)).Split("|")(2)

            '        DS.Descrizione.Rows.Add(DrDescrizioneNew)

            '        i += 1

            '    Next

            'End If

            '======================================================================

        Catch ex As Exception
            Log_Errori += "- Visualizzazione dei dettagli della Bolla di Accettazione: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '====================================================================================
        'RIEPILOGO IMBALLAGGI

        'modifica del 30/06/2010: spostato il codice nel modulo (utilizzato anche da certificato)

        '=============================================================
        'Riempi dataset Imballaggi

        Dim num_colli As Integer = 0

        Try
            Carica_DSImballaggi_StampaBollaECertificato(objParametri_Server, _
                                                        Log_Errori, _
                                                        DSRiepilogoImballiEntrata, _
                                                        DSImballiUscita, _
                                                        num_colli, _
                                                        Tara_Imballi, _
                                                        True, _
                                                        True, _
                                                        Piva, _
                                                        Id_Agenda, _
                                                        Lav_Cod)

        Catch ex As Exception
            Log_Errori += "- Caricamento dataset Imballaggi: " + vbCrLf + ex.Message + vbCrLf
        End Try

        'Try
        '    ' DrIntestazioneNew.Colli = x_Colli
        '    DS.IntestazioneFattura.Rows(0).Item("colli") = num_colli
        'Catch ex As Exception
        '    Log_Errori += "- Set numero colli: " + vbCrLf + ex.Message + vbCrLf
        'End Try


        '=============================================================
        'RIEPILOGO PESO

        Try

            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtPesoTotale"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(x_Peso_Totale, "#,###,##0")
            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtTaraVeicolo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(x_Tara_Veicolo, "#,###,##0")
            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtPesoLordo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Peso_Lordo, "#,###,##0")
            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtTaraImballi"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tara_Imballi, "#,###,##0")
            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtPesoNetto"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Peso_Netto, "#,###,##0")
            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtVariazione"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_Degrado, "#,###,##0")
            CType(rptBuonoAccettazione.Section9.ReportObjects("TxtPesoPagamento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = Format(Tot_NettoPagamento, "#,###,##0")

        Catch ex As Exception
            Log_Errori += "- Riepilogo del peso: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '=============================================================

        Try

            'imposto il datset sul report
            rptBuonoAccettazione.SetDataSource(DS)

            'imposto il datset sul report
            RptRiepilogoImballiEntrata.SetDataSource(DSRiepilogoImballiEntrata)

            'imposto il datset sul report
            RptImballiUscita.SetDataSource(DSImballiUscita)

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try


    End Sub




End Class
