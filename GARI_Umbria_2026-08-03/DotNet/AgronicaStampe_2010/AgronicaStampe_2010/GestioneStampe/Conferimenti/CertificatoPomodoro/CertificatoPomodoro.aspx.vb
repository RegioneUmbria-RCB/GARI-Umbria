Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Xml


Public Class CertificatoPomodoro
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptCertificato As Rpt_CertificatoPomodoro
    Private RptRiepilogoImballiEntrata As Rpt_RiepilogoImballi
    Private RptImballiUscita As Rpt_ImballiUscita

    Dim Report As enum_CodificaStampe

    Dim Piva As String
    Dim PrintName As String
    Dim Lav_Cod As Integer
    Dim Qs_Id_Agenda As Integer
    Dim PrintToPrinter As Integer
    Dim Str_Id_Agenda_Certificati As String
    Dim Numero_Copie As Integer
    Dim Str_Flag_Fascicola As String
    Dim Start_Page As Integer
    Dim End_Page As Integer
    Dim MyRnd As New Random

    Dim Tara_Imballi_Vuoti As Decimal
    Dim Tara_Imballi_Riga As Decimal
    Dim Num_Righe_Bolla As Integer

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Private Sub CertificatoPomodoro_Init(sender As Object, e As EventArgs) Handles Me.Init
        rptCertificato = New Rpt_CertificatoPomodoro

        RptRiepilogoImballiEntrata = New Rpt_RiepilogoImballi
        RptImballiUscita = New Rpt_ImballiUscita

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load


        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                    AgroKey_EncoderDecoder,
                                    Server)

        Qs_Id_Agenda = CInt(Stringa_Decodifica(CStr(Request.QueryString("i")),
                                    AgroKey_EncoderDecoder,
                                    Server))

        Lav_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("l")),
                                    AgroKey_EncoderDecoder,
                                    Server))

        If Not IsNothing(Request.QueryString("ptp")) Then
            PrintToPrinter = Stringa_Decodifica(CStr(Request.QueryString("ptp")),
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            PrintToPrinter = 0
        End If

        If Not IsNothing(Request.QueryString("pn")) Then
            PrintName = Stringa_Decodifica(CStr(Request.QueryString("pn")),
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            PrintName = ""
        End If

        If Not IsNothing(Request.QueryString("nc")) Then
            Numero_Copie = CInt(Stringa_Decodifica(CStr(Request.QueryString("nc")),
                                        AgroKey_EncoderDecoder,
                                        Server))
        Else
            Numero_Copie = 1
        End If

        If Not IsNothing(Request.QueryString("ff")) Then
            Str_Flag_Fascicola = Stringa_Decodifica(CStr(Request.QueryString("ff")),
                                        AgroKey_EncoderDecoder,
                                        Server)
        Else
            Str_Flag_Fascicola = "true"
        End If

        If Not IsNothing(Request.QueryString("sp")) Then
            Start_Page = CInt(Stringa_Decodifica(CStr(Request.QueryString("sp")),
                                        AgroKey_EncoderDecoder,
                                        Server))
        Else
            Start_Page = 0
        End If

        If Not IsNothing(Request.QueryString("ep")) Then
            End_Page = CInt(Stringa_Decodifica(CStr(Request.QueryString("ep")),
                                        AgroKey_EncoderDecoder,
                                        Server))
        Else
            End_Page = 0
        End If

        Str_Id_Agenda_Certificati = Session("Str_Id_Agenda_Certificati")

        Report = CInt(Session("ReportSelezionato"))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim IdentificazioneDocumento As String
        Dim Nome_File As String = ""
        Dim Nome_Documento As String = ""
        Dim Log_Errori As String = ""
        Dim Num_IdAgenda As Integer = 1
        Dim i As Integer
        Dim Vet_IdAgenda As String()

        Select Case Report
            Case enum_CodificaStampe.Certificato_Pomodoro_Interno
                Nome_Documento = "CertificatoPomodoroInterno2011"
            Case enum_CodificaStampe.Certificato_Pomodoro_Esterno
                Nome_Documento = "CertificatoPomodoroEsterno2011"
        End Select

        If PrintToPrinter = 1 Then

            If Not IsNothing(Str_Id_Agenda_Certificati.Split("|")) Then
                Vet_IdAgenda = Str_Id_Agenda_Certificati.Split("|")
            Else
                Vet_IdAgenda(0) = Str_Id_Agenda_Certificati
            End If

            Num_IdAgenda = Vet_IdAgenda.Length

        End If


        For i = 0 To Num_IdAgenda - 1

            If PrintToPrinter = 1 Then

                Qs_Id_Agenda = CInt(Vet_IdAgenda(i))

                rptCertificato.Close()
                rptCertificato.Dispose()
                rptCertificato = Nothing

                rptCertificato = New Rpt_CertificatoPomodoro

                RptRiepilogoImballiEntrata.Close()
                RptRiepilogoImballiEntrata.Dispose()
                RptImballiUscita.Close()
                RptImballiUscita.Dispose()

                RptRiepilogoImballiEntrata = Nothing
                RptImballiUscita = Nothing

                RptRiepilogoImballiEntrata = New Rpt_RiepilogoImballi
                RptImballiUscita = New Rpt_ImballiUscita

            End If

            If Not Me.IsPostBack Then

                '--------------------------------------------
                ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
                '--------------------------------------------
                Dim DS As New DS_CertificatoPomo
                Dim DSRiepilogoImballiEntrata As New DS_Iva 'riciclo il dataset dell'iva, per gli imballi in entrata, se no fa casino con quelli in uscita
                Dim DSImballiUscita As New DS_ImballiUscita

                Try

                    Log_Errori = ""

                    'RinominaCaselleDiTesto()

                    Stampa_CertificatoPomodoro(DS,
                                                DSRiepilogoImballiEntrata,
                                                DSImballiUscita,
                                                Log_Errori,
                                                Qs_Id_Agenda)

                    '=============================================================
                    'Aggancio dataset

                    Try

                        'imposto il dataset sul report
                        rptCertificato.SetDataSource(DS)

                        RptRiepilogoImballiEntrata.SetDataSource(DSRiepilogoImballiEntrata)

                        RptImballiUscita.SetDataSource(DSImballiUscita)


                    Catch ex As Exception
                        Log_Errori &= "- Aggancio dataset: " & vbCrLf & ex.Message & vbCrLf
                    End Try

                    Try

                        rptCertificato.OpenSubreport("Rpt_RiepilogoImballi.rpt").SetDataSource(DSRiepilogoImballiEntrata)

                        rptCertificato.OpenSubreport("Rpt_ImballiUscita.rpt").SetDataSource(DSImballiUscita)

                    Catch ex As Exception
                        Log_Errori &= "- SetDataSource sottoreport imballi: " & vbCrLf & ex.Message & vbCrLf
                    End Try


                    '-------------------------------------------
                    'Nel caso di stampa del certificato esterno
                    'occorre bloccare la bolla
                    BloccaBolla_x_CertificatoEsterno(Log_Errori,
                                                    Qs_Id_Agenda)



                    IdentificazioneDocumento = Nome_Documento & "_p" & Piva '& "_" + Ident_Data & "_" & Ident_Numero & "_" & Ident_Contatto

                    If Log_Errori <> "" Then
                        Log_Errori = Nome_Documento & ", id_agenda = " & CStr(Qs_Id_Agenda) & vbCrLf & vbCrLf & Log_Errori
                    End If


                    Nome_File = "Log_Errori_" & IdentificazioneDocumento & CStr(Session("ASG_Utente_Username")) & "_" & CStr(Qs_Id_Agenda)


                Catch exc As Exception
                    Log_Errori &= "- PageLoad: " & vbCrLf & exc.Message & vbCrLf
                End Try

                Try


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
                    Log_Errori &= "- Gestione allegati: " & vbCrLf & ex.Message & vbCrLf
                End Try


                '-----------------------------------------
                '---- Salvataggio Log Errori -------------
                '-----------------------------------------
                If Log_Errori <> "" Then

                    Dim objLog As New GestioneLogStampe
                    objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                             "Stampe_Conferimento",
                                             Nome_File & ".txt",
                                             Session("ASG_Utente_Username"),
                                             "CertificatoPomodoro.aspx",
                                             Log_Errori)

                End If

            End If


            '==================================================================

            If PrintToPrinter = 0 Then

                ' per visualizzare l'anteprima, ma non c'è nè il bottone di stampa, nè quello di export
                Session("Report") = rptCertificato
                Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" & Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))

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
                        exportOpts = rptCertificato.ExportOptions

                        ' Imposta il formato di esportazione.
                        exportOpts.ExportFormatType = ExportFormatType.CrystalReport
                        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

                        ' Imposta le opzioni relative al file del disco.
                        Dim strPath, NomeFileRpt As String

                        If Not PathDatiServizioWinStampa.EndsWith("\") Then PathDatiServizioWinStampa &= "\"

                        NomeFileRpt = Nome_Documento & "_" & MyRnd.Next.ToString() & "_" & CStr(Session("ASG_Utente_Username")) & "_" & Session.SessionID.ToString & ".rpt"

                        strPath = PathDatiServizioWinStampa & NomeFileRpt

                        diskOpts.DiskFileName = strPath
                        exportOpts.DestinationOptions = diskOpts

                        ' Esportazione del report.
                        rptCertificato.Export()

                        risp = InviaStampaToServizioUtility(Request, rptCertificato, Log_Errori, PrintName, NomeFileRpt,
                                                            PathComandiServizioWinStampa, PathDatiServizioWinStampa, objParametri_Server,
                                                            Numero_Copie, Str_Flag_Fascicola, Start_Page, End_Page)

                        '==========================================================
                    Case 1 'funzione print to printer

                        Try
                            rptCertificato.PrintOptions.PrinterName = PrintName

                            rptCertificato.PrintToPrinter(Numero_Copie, CBool(Str_Flag_Fascicola), Start_Page, End_Page)

                            risp = True

                        Catch ex As Exception
                            Log_Errori &= "Stampa del report non riuscita."
                        End Try

                        '==========================================================
                    Case Else
                        'non gestito
                        Log_Errori &= "Il file di configurazione dell'AgronicaStampe non è impostato correttamente."
                        '==========================================================
                End Select


                If risp Then
                    'ok
                    Dim a As Integer = 1
                Else
                    '-----------------------------------------
                    '---- Salvataggio Log Errori -------------
                    '-----------------------------------------
                    If Log_Errori <> "" Then

                        Dim objLog As New GestioneLogStampe
                        objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Stampe_Conferimento",
                                                 Nome_File & ".txt",
                                                 Session("ASG_Utente_Username"),
                                                 "CertificatoPomodoro.aspx",
                                                 Log_Errori)

                    End If

                End If

            End If 'PrintToPrinter

        Next

        FileSystem.ChDir("C:\")

        If PrintToPrinter = 1 Then
            Dim strClose As String = "<script language='javascript'>window.close()</script>"
            Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        End If


    End Sub

    '#####################################################################################################
    ' in sospeso:
    'tara_imballi totale da mostrare?
    Private Sub Stampa_CertificatoPomodoro(ByRef DS As DS_CertificatoPomo,
                                                ByRef DSRiepilogoImballiEntrata As DS_Iva,
                                                ByRef DSImballiUscita As DS_ImballiUscita,
                                                ByRef Log_Errori As String,
                                                ByVal Id_Agenda As Integer)


        '=============================================================
        'Dichiarazione variabili

        Dim Dr As DS_CertificatoPomo.DT_CertificatoPomodoroRow
        Dim DT As DataTable = Nothing
        Dim i As Integer
        Dim Flag_Bio As Boolean
        'Dim Numero_Bolla As String
        'Dim x_Doc_Numero_Sin_Accettazione As String = ""
        'Dim x_Doc_Numero_Accettazione As Integer = 0
        'Dim x_Doc_Numero_Des_Accettazione As String = ""
        'Dim x_Lunghezza_Sin_Accett As Integer
        'Dim x_Lunghezza_Centro_Accett As Integer
        'Dim x_Lunghezza_Des_Accett As Integer
        'Dim x_CarattereFormattazione_Accett As String
        'Dim x_Doc_Numero_Sin_Cert As String = ""
        'Dim x_Doc_Numero_Cert As Integer = 0
        'Dim x_Doc_Numero_Des_Cert As String = ""
        Dim Str_Bio_Conv As String
        Dim Str_Flag_Bio As String
        Dim Str_Surgelato As String
        Dim Str_Tipologia As String
        Dim x_Doc_Numero_Sin_DDTConf As String = ""
        Dim x_Doc_Numero_DDTConf As Integer = 0
        Dim x_Doc_Numero_Des_DDTConf As String = ""
        Dim Numero_DDTConf As String

        'Dim Tipo_Peso As Integer
        Dim Peso_Lordo_Kg, Tara_Veicolo_Kg, Tara_Imballi_Kg, Peso_Netto_Kg, Netto_Pag_Kg, Scarto_Kg As Decimal
        Dim Peso_Lordo_Q, Tara_Veicolo_Q, Tara_Imballi_Q, Peso_Netto_Q, Netto_Pag_Q, Scarto_Q, Peso_Totale_Q, Peso_Totale_Kg As Decimal

        Dim Perc_Marcio, Perc_Verde, Perc_Inerti, Perc_Tot_Dif_Maggiori, Perc_Tot_Dif_Magg_Round As Decimal
        Dim Perc_Schiacciati, Perc_Immaturi, Perc_Scottature, Perc_Lesioni, Perc_Tot_Dif_Minori As Decimal
        Dim Grado_Brix, Indice_Prezzo_Grado_Brix As Decimal
        Dim Coefficiente, Franchigia, Premio_EuroTon_PomoBio As Decimal
        Dim Dif_MinoriXCoeff As Decimal
        Dim Magg_Rid_DifMaggiori As Decimal
        Dim Indice_Variazione_Prezzo As Decimal
        Dim Premio_Pomo_Tardivo As Decimal
        Dim Premio_Pomo_Bio As Decimal
        Dim Prezzo_Unitario_Contratto, Prezzo_Unitario_Finale_Kg, Prezzo_Unitario_Finale_Tn As Decimal
        Dim Importo_Totale_Pag As Decimal


        '=============================================================
        'Lettura dei dati

        Try

            Dim objConfPomo As New AgronicaCoreStampeDAL.Conferimento_Pomodoro

            DT = objConfPomo.CertificatoPomodoro_Stampa_PDF_XLS(Piva,
                                                    Id_Agenda,
                                                   0,
                                                   "",
                                                   "",
                                                   "",
                                                   0,
                                                   "",
                                                   "",
                                                   AGRODATAINIZIO,
                                                   AGRODATAFINE,
                                                   0,
                                                   0,
                                                   0,
                                                   "",
                                                   "",
                                                   False,
                                                   False,
                                                   "", "",
                                                    objParametri_Server)


        Catch ex As Exception
            Log_Errori &= "- Lettura dei dati: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try

        '=============================================================
        'Valorizzazione del dataset

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
                Dim objConfFun As New AgronicaCoreStampeDAL.Conferimento_Funzioni

                '-----------------------------------------------------
                Dr = DS.DT_CertificatoPomodoro.NewDT_CertificatoPomodoroRow()
                '-----------------------------------------------------

                'c'è un record per ogni riga in materie_prime_campionature
                'al momento solo con tipo= 'danno'
                'quindi una riga per ogni difetto maggiore e minore
                For i = 0 To DT.Rows.Count - 1

                    With DT.Rows(i)

                        'solo una volta, alla prima riga
                        If i = 0 Then

                            Dim obj_TabOP As New AgronicaCoreMetaSchemaDAL.Tabelle_OP_R

                            Dr.Piva = Piva
                            Dr.Anno = .Item("Anno_raccolto")

                            'x_Lunghezza_Sin_Accett = .Item("Lunghezza_Sin")
                            'x_Lunghezza_Centro_Accett = .Item("Lunghezza_Centro")
                            'x_Lunghezza_Des_Accett = .Item("Lunghezza_Des")
                            'x_CarattereFormattazione_Accett = .Item("CarattereFormattazione")

                            'x_Doc_Numero_Sin_Cert = .Item("Doc_Numero_Sin_Cert")
                            'x_Doc_Numero_Cert = .Item("Doc_Numero_Cert")
                            'x_Doc_Numero_Des_Cert = .Item("Doc_Numero_Des_Cert")

                            'Dr.Data_Certificato = .Item("Data_Certificato")
                            Dr.Data_Certificato = .Item("Data_Accett")

                            'x_Doc_Numero_Sin_Accettazione = .Item("Doc_Numero_Sin")
                            'x_Doc_Numero_Accettazione = .Item("Doc_Numero")
                            'x_Doc_Numero_Des_Accettazione = .Item("Doc_Numero_Des")

                            'Numero_Bolla = Ricava_NumeroDocumento_Con_Sequenza(
                            '                        x_Doc_Numero_Sin_Accettazione,
                            '                        x_Doc_Numero_Accettazione,
                            '                        x_Doc_Numero_Des_Accettazione,
                            '                        x_Lunghezza_Sin_Accett,
                            '                        x_Lunghezza_Centro_Accett,
                            '                        x_Lunghezza_Des_Accett,
                            '                        x_CarattereFormattazione_Accett)

                            Dr.Num_Bolla = .Item("Numero_Bolla")
                            Dr.Data_Bolla = .Item("Data_Accett")
                            Dr.Cod_OPR_Contratto = "" '.Item("Cod_Agrea_Contratto")
                            Dr.Data_Accettazione = .Item("Data_Accett")

                            'VECCHIO CONFERIMENTO
                            ''nota, Marco non salva più nel movimento del certificato (cau_mov=4070)
                            ''quindi il num_certificato è 0 (non so gli altri anni come faceva a venire stampato)
                            'Select Case CDate(Dr.Data_Accettazione).Year
                            '    Case 2011, 2012
                            '        'Dr.Num_Certificato = Ricava_NumeroDocumento_Senza_Sequenza( _
                            '        '                        x_Doc_Numero_Sin_Cert, _
                            '        '                        x_Doc_Numero_Cert, _
                            '        '                        x_Doc_Numero_Des_Cert)

                            '        Dr.Num_Certificato = Ricava_NumeroDocumento_Con_Sequenza(
                            '                                x_Doc_Numero_Sin_Cert,
                            '                                x_Doc_Numero_Cert,
                            '                                x_Doc_Numero_Des_Cert,
                            '                                x_Lunghezza_Sin_Accett,
                            '                                x_Lunghezza_Centro_Accett,
                            '                                x_Lunghezza_Des_Accett,
                            '                                x_CarattereFormattazione_Accett)
                            '    Case Else
                            '        Dr.Num_Certificato = Dr.Num_Bolla
                            'End Select
                            Dr.Num_Certificato = Dr.Num_Bolla

                            ''modifica del 30/07/2012:riattivazione clausole
                            'Select Case CDate(Dr.Data_Accettazione).Year

                            '    Case 2011
                            '        RinominaCaselleDiTesto_2011()
                            '        Dr.Num_Clausola = ""
                            '        Dr.Data_Clausola = ""
                            '        'If InStr(CStr(.Item("Clausola")), "|") > 0 Then
                            '        '    Dr.Num_Clausola = CStr(.Item("Clausola")).Split("|")(0)
                            '        '    Dr.Data_Clausola = CStr(.Item("Clausola")).Split("|")(1)
                            '        'Else
                            '        '    Dr.Num_Clausola = ""
                            '        '    Dr.Data_Clausola = ""
                            '        'End If
                            '    Case Else
                            '        'Dr.Num_Clausola = ""
                            '        'Dr.Data_Clausola = ""
                            '        RinominaCaselleDiTesto_2012()
                            '        If InStr(CStr(.Item("Clausola")), "|") > 0 Then
                            '            Dr.Num_Clausola = CStr(.Item("Clausola")).Split("|")(0)
                            '            Dr.Data_Clausola = CStr(.Item("Clausola")).Split("|")(1)
                            '        Else
                            '            Dr.Num_Clausola = ""
                            '            Dr.Data_Clausola = ""
                            '        End If
                            'End Select

                            If InStr(CStr(.Item("Clausola")), "|") > 0 Then
                                Dr.Num_Clausola = CStr(.Item("Clausola")).Split("|")(0)
                                Dr.Data_Clausola = CStr(.Item("Clausola")).Split("|")(1)
                            Else
                                Dr.Num_Clausola = ""
                                Dr.Data_Clausola = ""
                            End If

                            If Dr.Num_Clausola <> "" AndAlso Dr.Data_Clausola <> "" Then
                                Dr.Num_Contratto = ""
                                Dr.Data_Contratto = ""
                            Else
                                Dr.Num_Contratto = .Item("Contratto_Numero")

                                If CDate(.Item("Data_Stipulazione")) = AGRODATAINIZIO Then
                                    Dr.Data_Contratto = ""
                                Else
                                    Dr.Data_Contratto = CStr(.Item("Data_Stipulazione"))
                                End If
                            End If

                            Dr.Prodotto = CStr(.Item("Veg_Des")).ToUpper & " " & CStr(objADDFun.TipologiaPomodoro_from_GrvaCod(.Item("Grva_Cod_Veg"))).ToUpper

                            'If .Item("Flag_Surgelato") = 1 Then
                            '    Str_Surgelato = "SURGELATO"
                            'Else
                            '    Str_Surgelato = ""
                            'End If
                            Str_Surgelato = ""

                            Select Case CInt(.Item("Regolamento"))
                                Case enum_Cod_Regolamento.Regolamento_bio
                                    Flag_Bio = True
                                    Str_Bio_Conv = "BIOLOGICO"
                                    Str_Flag_Bio = "X"
                                Case Else
                                    Flag_Bio = False
                                    Str_Bio_Conv = "LOTTA INTEGRATA"
                                    Str_Flag_Bio = ""
                            End Select

                            Str_Tipologia = CStr(objADDFun.TipologiaPomodoro_from_GrvaCod(.Item("Grva_Cod_Veg"))).ToUpper
                            Dr.Tipologia = Str_Tipologia
                            If Str_Tipologia <> "" Then
                                Dr.Tipologia += " "
                            End If
                            Dr.Tipologia += Str_Surgelato
                            If Str_Surgelato <> "" Then
                                Dr.Tipologia += " "
                            End If

                            Dr.Tipologia += Str_Bio_Conv

                            Dr.Desc_Appezza = .Item("Descrizione_Appezzamento")
                            Dr.Varieta_Distretto = .Item("DescVarieta_OI")

                            Dr.Codice_OP = .Item("Codice_OP")
                            Dr.Piva_OP = .Item("Cod_Contatto_Conferente")
                            Dr.Cod_Fisc_OP = .Item("Codice_Fiscale_Conferente")
                            Dr.Rag_Soc_OP = .Item("Rag_Soc_Conferente")
                            Dr.Codice_Unione_OP = .Item("Codice_Unione_OP")
                            Dr.Rag_Soc_Unione_OP = obj_TabOP.UnioneOPEstesa_from_CodiceUnioneOP(.Item("Codice_Unione_OP"), Session("ASG_objParametri_Server"))

                            Dr.Piva_Produttore = .Item("Cod_Contatto_Produttore")
                            Dr.Cod_Fisc_Produttore = .Item("Codice_Fiscale_Produttore")
                            Dr.Rag_Soc_Produttore = .Item("Rag_Soc_Produttore")
                            Dr.Piva_Coop = .Item("Cod_Contatto_Coop")
                            Dr.Rag_Soc_Coop = .Item("Rag_Soc_Coop")
                            Dr.Piva_CentroLav = ""
                            Dr.Rag_Soc_CentroLav = ""
                            Dr.Codice_Trasformatore = .Item("Codice_Az_Trasf")
                            Dr.Codice_Stabilimento = .Item("Codice_Stab_Reg")
                            Dr.Piva_Trasformatore = .Item("Cod_Contatto_Trasf")
                            Dr.Cod_Fisc_Trasformatore = .Item("Codice_Fiscale_Trasf")
                            Dr.Rag_Soc_Trasformatore = .Item("Rag_Soc_Trasf")
                            Dr.Indirizzo_Trasformatore = .Item("ind_des_Trasf")
                            Dr.Comune_Trasformatore = .Item("localita_Trasf") & " (" & .Item("comuni_prov_Trasf") & ")"
                            Dr.CAP_Trasformatore = .Item("cap_Trasf")
                            Dr.Prov_Trasformatore = .Item("comuni_prov_Trasf")
                            Dr.Codice_Ass_Industriale = .Item("Codice_Ass_Ind")
                            Dr.Ass_Industriale = obj_TabOP.AssociazioneIndutriale_from_CodiceAssociazioneIndutriale(.Item("Codice_Ass_Ind"), Session("ASG_objParametri_Server"))

                            x_Doc_Numero_Sin_DDTConf = .Item("Doc_Numero_Sin_Conf")
                            x_Doc_Numero_DDTConf = .Item("Doc_Numero_Conf")
                            x_Doc_Numero_Des_DDTConf = .Item("Doc_Numero_Des_Conf")

                            Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza(
                                                    x_Doc_Numero_Sin_DDTConf,
                                                    x_Doc_Numero_DDTConf,
                                                    x_Doc_Numero_Des_DDTConf)

                            Dr.Num_DDT = Numero_DDTConf
                            Dr.Data_DDT = .Item("Data_Conf")
                            Dr.Rag_Soc_Vettore = .Item("Rag_Soc_Vettore")
                            Dr.Targa_Automezzo = .Item("Targa_Automezzo")
                            Dr.Targa_Rimorchio = .Item("Targa_Rimorchio")

                            Dr.Ora_Accettazione = CDate(.Item("Ora_Accett")).ToShortTimeString
                            Dr.Num_Tagliando_Pesa = "RCD" & CStr(.Item("Tagliando_Pesa"))

                            'Dr.Tondo_X = ""
                            'Dr.Allungato_X = ""
                            'TondoLungo_from_GrvaCod(.Item("Grva_Cod_Veg"), Dr.Tondo_X, Dr.Allungato_X)
                            Dr.Biologico = Str_Flag_Bio

                            '-------------------------------------------------------------------

                            'PESI - CONF NUOVO:
                            'bisogna leggere solo quelli su movimenti_dettagli
                            'peso netto = qta_extra_totale
                            'tara imballi = tara se impostata in riga
                            'il resto da calcolare

                            Peso_Totale_KG = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("peso_totale"))
                            Peso_Totale_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Peso_Totale_KG)

                            Tara_Veicolo_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("tara_veicolo"))
                            Tara_Veicolo_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Tara_Veicolo_Kg)

                            Tara_Imballi_Vuoti = .Item("tara_totale_imb_entrata")
                            Tara_Imballi_Riga = .Item("Tara_Dettaglio")
                            Num_Righe_Bolla = .Item("Num_Righe")

                            Tara_Imballi_Kg = objConfFun.Calcola_TaraImballi(Tara_Imballi_Vuoti,
                                                                                     Tara_Imballi_Riga,
                                                                                     Num_Righe_Bolla)

                            Tara_Imballi_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Tara_Imballi_Kg)
                            Tara_Imballi_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Tara_Imballi_Kg)

                            Peso_Netto_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("peso_netto"))
                            Peso_Netto_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Netto_Kg)
                            Peso_Netto_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Peso_Netto_Kg)

                            'Non va bene fare questa differenza, perchè potrebbero esserci più articoli
                            '.Item("peso_lordo") = objHLP.Calcola_PesoLordo_NewConf(.Item("peso_totale"), .Item("tara_veicolo"))
                            '.Item("peso_lordo") = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(.Item("peso_lordo"))

                            Peso_Lordo_Kg = .Item("peso_netto") + .Item("Tara_Imballi")
                            Peso_Lordo_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Lordo_Kg)
                            Peso_Lordo_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Peso_Lordo_Kg)

                            objADDFun.Calcola_NettoPagamento(Peso_Netto_Kg,
                                                            .Item("Variazione_Raccolta"),
                                                            .Item("Udm_Cod_Raccolta"),
                                                            Scarto_Kg,
                                                            Netto_Pag_Kg)

                            Scarto_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Scarto_Kg)
                            Scarto_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Scarto_Kg)

                            Netto_Pag_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Netto_Pag_Kg)
                            Netto_Pag_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Netto_Pag_Kg)

                            '-------------------------------------------------------------------

                            'OLD CONFERIMENTO

                            'Tipo_Peso = .Item("Tipo_Peso")

                            'Tara_Veicolo_Kg = .Item("Tara_Veicolo")
                            'Tara_Veicolo_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Tara_Veicolo_Kg)
                            'Tara_Veicolo_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Tara_Veicolo_Kg)

                            'Tara_Imballi_Kg = .Item("Tara_Imballi")
                            'Tara_Imballi_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Tara_Imballi_Kg)
                            'Tara_Imballi_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Tara_Imballi_Kg)

                            'Peso_Kg = .Item("Peso")
                            'Peso_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Kg)

                            'Peso_Lordo_Kg = Calcola_PesoLordo(Tipo_Peso, Peso_Kg, Tara_Imballi_Kg)
                            'Peso_Lordo_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Lordo_Kg)
                            'Peso_Lordo_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Peso_Lordo_Kg)

                            'Peso_Netto_Kg = Calcola_PesoNetto(Tipo_Peso, Peso_Kg, Tara_Imballi_Kg)
                            'Peso_Netto_Kg = AgronicaCoreDataProvider.Agro_Math.ArrotondaVal_0(Peso_Netto_Kg)
                            'Peso_Netto_Q = AgronicaCoreDataProvider.Agro_Math.Converti_daKG_aQL_ArrotondaVal2(Peso_Netto_Kg)

                            'Peso_Totale_Q = Calcola_PesoTotale(2, Peso_Lordo_Kg, Tara_Veicolo_Kg)

                            '''nel caso del pomodoro:
                            '''peso lordo = peso totale (lordo + tara camion + tara imballi)
                            '''tara = tara automezzo + tara imballi
                            '''peso netto = netto
                            ''Calcola_PesoTotale(2, _
                            ''                    Tara_Veicolo_Kg, Tara_Imballi_Kg, Peso_Netto_Kg, _
                            ''                    Tara_Veicolo_Q, Tara_Imballi_Q, Peso_Netto_Q, _
                            ''                    Peso_Lordo_Q)

                            '-------------------

                            'nel caso del pomodoro:
                            'peso lordo = peso totale
                            Dr.Peso_Lordo = Format(Peso_Totale_Q, "#,###,##0.00")
                            Dr.Tara_Veicolo = Format(Tara_Veicolo_Q, "#,###,##0.00")
                            Dr.Tara_Imballi = Format(Tara_Imballi_Q, "#,###,##0.00")
                            Dr.Peso_Netto = Format(Peso_Netto_Q, "#,###,##0.00")

                            'il totale dei difetti maggiori arrotondati con il giusto arrotondamento è salvato nel degrado
                            Perc_Tot_Dif_Magg_Round = .Item("Variazione_Raccolta")

                            'If InStr(CStr(.Item("GradoBrix_IndicePrezzo")), "_") > 0 Then
                            '    Grado_Brix = CDbl(CStr(.Item("GradoBrix_IndicePrezzo")).Split("_")(0))
                            '    Indice_Prezzo_Grado_Brix = CDbl(CStr(.Item("GradoBrix_IndicePrezzo")).Split("_")(1))
                            'Else
                            '    Grado_Brix = 0
                            '    Indice_Prezzo_Grado_Brix = 0
                            'End If
                            'Dr.Grado_Brix = Format(Grado_Brix, "#,###,##0.000")
                            'Dr.Indice_prezzo_Grado_Brix = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")
                            Indice_Prezzo_Grado_Brix = 0 'si calcola con la formula
                            Dr.Indice_prezzo_Grado_Brix = Indice_Prezzo_Grado_Brix

                            Coefficiente = .Item("Coefficiente_DifMinori")
                            Franchigia = .Item("Franchigia_DifMaggiori")
                            Premio_EuroTon_PomoBio = .Item("Premio_EuroTon_PomoBio")

                            Prezzo_Unitario_Contratto = CDbl(.Item("Prezzo_Unitario")) * 1000 'a tonnellata

                            Prezzo_Unitario_Finale_Kg = .Item("Prezzo_Unitario_Netto")

                            'If .Item("Premio_PomoTardivo") = "" Then
                            '    Premio_Pomo_Tardivo = 0
                            'Else
                            '    Premio_Pomo_Tardivo = CDbl(.Item("Premio_PomoTardivo")) 'a tonnellata
                            'End If

                            Premio_Pomo_Tardivo = CDbl(.Item("Premio_PomoTardivo")) 'a tonnellata

                            Premio_Pomo_Bio = CDbl(.Item("Premio_Pomo_Bio")) 'a tonnellata

                        End If 'fine solo prima riga

                        'per ogni riga
                        'Materie_Prime_Campionature.tipo IN ('ogradobrix','omarcio','overde','oinerti','ofruttiimmaturi','ofruttilesionati','ofruttischiacciati','ofruttiscottati) ")
                        Dim oVal As Decimal = 0
                        'Decimal.TryParse(.Field(Of String)("val_cod").Replace(".", ","), Globalization.NumberStyles.Number, Globalization.CultureInfo.GetCultureInfo("it"), oVal)
                        oVal = .Item("val_cod")

                        Select Case .Item("tipo")
                            Case "ogradobrix"
                                Grado_Brix = oVal
                                Dr.Grado_Brix = Grado_Brix
                            Case "omarcio"
                                Perc_Marcio = oVal
                            Case "overde"
                                Perc_Verde = oVal
                            Case "oinerti"
                                Perc_Inerti = oVal
                            Case "ofruttischiacciati"
                                Perc_Schiacciati = oVal
                            Case "ofruttiimmaturi"
                                Perc_Immaturi = oVal
                            Case "ofruttiscottati"
                                Perc_Scottature = oVal
                            Case "ofruttilesionati"
                                Perc_Lesioni = oVal
                        End Select

                        'solo una volta, all'ultima riga
                        If i = DT.Rows.Count - 1 Then

                            Select Case CDate(Dr.Data_Accettazione).Year

                                Case 2021 To 2025
                                    '/////////////////////////////////////////////
                                    '////////////      2021     ////////////////
                                    '/////////////////////////////////////////////
                                    objADDFun.Calcola_Dati_Certificato_Pomodoro_2021(Flag_Bio,
                                                                        Prezzo_Unitario_Contratto,
                                                                        Peso_Netto_Kg,
                                                                        Perc_Marcio,
                                                                        Perc_Verde,
                                                                        Perc_Inerti,
                                                                        Perc_Tot_Dif_Magg_Round,
                                                                        Perc_Schiacciati,
                                                                        Perc_Immaturi,
                                                                        Perc_Scottature,
                                                                        Perc_Lesioni,
                                                                        Grado_Brix,
                                                                        Indice_Prezzo_Grado_Brix,
                                                                        Coefficiente,
                                                                        Franchigia,
                                                                        Prezzo_Unitario_Finale_Kg,
                                                                        Scarto_Q,
                                                                        Netto_Pag_Q,
                                                                        Perc_Tot_Dif_Maggiori,
                                                                        Perc_Tot_Dif_Minori,
                                                                        Dif_MinoriXCoeff,
                                                                        Magg_Rid_DifMaggiori,
                                                                        Indice_Variazione_Prezzo,
                                                                        Prezzo_Unitario_Finale_Tn,
                                                                        Importo_Totale_Pag)

                                    Dr.Indice_prezzo_Grado_Brix = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

                                Case Else
                                    '/////////////////////////////////////////////
                                    '////////////           ////////////////
                                    '/////////////////////////////////////////////
                                    objADDFun.Calcola_Dati_Certificato_Pomodoro_2026(Flag_Bio,
                                                                            Prezzo_Unitario_Contratto,
                                                                            Peso_Netto_Kg,
                                                                            Perc_Marcio,
                                                                            Perc_Verde,
                                                                            Perc_Inerti,
                                                                            Perc_Tot_Dif_Magg_Round,
                                                                            Perc_Schiacciati,
                                                                            Perc_Immaturi,
                                                                            Perc_Scottature,
                                                                            Perc_Lesioni,
                                                                            Grado_Brix,
                                                                            Indice_Prezzo_Grado_Brix,
                                                                            Coefficiente,
                                                                            Franchigia,
                                                                            Prezzo_Unitario_Finale_Kg,
                                                                            Scarto_Q,
                                                                            Netto_Pag_Q,
                                                                            Perc_Tot_Dif_Maggiori,
                                                                            Perc_Tot_Dif_Minori,
                                                                            Dif_MinoriXCoeff,
                                                                            Magg_Rid_DifMaggiori,
                                                                            Indice_Variazione_Prezzo,
                                                                            Prezzo_Unitario_Finale_Tn,
                                                                            Importo_Totale_Pag)

                                    Dr.Indice_prezzo_Grado_Brix = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

                            End Select

                            Dr.Perc_Marcio = Format(Perc_Marcio, "#,###,##0.000")
                            Dr.Perc_Verde = Format(Perc_Verde, "#,###,##0.000")
                            Dr.Perc_Inerti = Format(Perc_Inerti, "#,###,##0.000")

                            Dr.Perc_Tot_Dif_Magg = Format(Perc_Tot_Dif_Maggiori, "#,###,##0.000")
                            Dr.Perc_Tot_Dif_Magg_Round = Format(Perc_Tot_Dif_Magg_Round, "#,###,##0.000")

                            Dr.Perc_Schiacciati = Format(Perc_Schiacciati, "#,###,##0.000")
                            Dr.Perc_Immaturi = Format(Perc_Immaturi, "#,###,##0.000")
                            Dr.Perc_Scottature = Format(Perc_Scottature, "#,###,##0.000")
                            Dr.Perc_Lesioni = Format(Perc_Lesioni, "#,###,##0.000")
                            Dr.Perc_Tot_Dif_Minori = Format(Perc_Tot_Dif_Minori, "#,###,##0.000")

                            Dr.Scarto = Format(Scarto_Q, "#,###,##0.00")
                            Dr.Netto_Pag = Format(Netto_Pag_Q, "#,###,##0.00")

                            Dr.Tasso_Riduzione_1 = Format(-1 * Dif_MinoriXCoeff, "#,###,##0.000")
                            Dr.Tasso_Riduzione_2 = Format(Magg_Rid_DifMaggiori, "#,###,##0.000")
                            Dr.Indice_Var_Prezzo = Format(Indice_Variazione_Prezzo, "#,###,##0.000")
                            Dr.Prezzo_Unitario_Contratto = Format(Prezzo_Unitario_Contratto, "#,###,##0.00")
                            Dr.Prezzo_Unitario_Finale = Format(Prezzo_Unitario_Finale_Tn, "#,###,##0.00")
                            Dr.Importo_Totale_Pag = Format(Importo_Totale_Pag, "#,###,##0.00")

                            Dr.Premio_Pomo_Biologico = Format(Premio_Pomo_Bio, "#,###,##0.00")
                            Dr.Premio_Pomo_Tardivo = Format(Premio_Pomo_Tardivo, "#,###,##0.00")

                        End If

                    End With


                Next

                '-----------------------------------------------------
                DS.DT_CertificatoPomodoro.Rows.Add(Dr)
                '-----------------------------------------------------

            End If


        Catch ex As Exception
            Log_Errori &= "- Valorizzazione del dataset: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        '=============================================================
        'Valorizzazione del report

        Select Case Report

            Case enum_CodificaStampe.Conf_Certificato_Pomodoro

                'sezione riepilogo: scarto kg
                rptCertificato.Section7.SectionFormat.EnableSuppress = True
                'sezione firme
                rptCertificato.Section12.SectionFormat.EnableSuppress = True

            Case enum_CodificaStampe.Certificato_Pomodoro_Esterno

                'sezione numerazione interna
                rptCertificato.Section11.SectionFormat.EnableSuppress = True

                'sezione dettagliata difetti
                rptCertificato.Section6.SectionFormat.EnableSuppress = True

        End Select

        '=============================================================
        'Riempi dataset Imballaggi

        Try

            'Carica_DSImballaggi_StampaBollaECertificato(objParametri_Server,
            '                                            Log_Errori,
            '                                            DSRiepilogoImballiEntrata,
            '                                            DSImballiUscita,
            '                                            True,
            '                                            True,
            '                                            Piva,
            '                                            Id_Agenda,
            '                                            Lav_Cod)

            Dim Tara_Imballi As Decimal = 0
            Dim num_colli As Integer = 0

            Carica_DSImballaggi_StampaBollaECertificato(objParametri_Server,
                                                        Log_Errori,
                                                        DSRiepilogoImballiEntrata,
                                                        DSImballiUscita,
                                                        num_colli,
                                                        Tara_Imballi,
                                                        True,
                                                        True,
                                                        Piva,
                                                        Id_Agenda,
                                                        Lav_Cod)


        Catch ex As Exception
            Log_Errori &= "- Caricamento dataset Imballaggi: " & vbCrLf & ex.Message & vbCrLf
        End Try



    End Sub


    '#####################################################################################################
    Private Sub BloccaBolla_x_CertificatoEsterno(ByRef Log_Errori As String,
                                                    ByVal Id_Agenda As Integer)

        'Se sto stampando il certificato esterno
        If Report = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then

            'controllo id_agenda valorizzato
            If Id_Agenda > 0 Then

                Dim obj_AgendaW As New AgronicaCoreContabDAL.Agenda_W

                obj_AgendaW.Agenda_Blocca(Piva,
                                            0,
                                            Id_Agenda,
                                            CStr(Session("ASG_Utente_Username")),
                                            Date.Now,
                                            "",
                                            Session("ASG_objParametri_Server"))

            Else
                'errore!
                Log_Errori += "Certificato Esterno: non è stato possibile bloccare la bolla, poichè l'id_agenda è nullo!"
            End If

        End If

    End Sub

End Class