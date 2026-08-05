Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Xml
Imports AgronicaCoreDataProvider.Agro_Math


Public Class CertificatiPomodoro
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

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

#Region " Certificati Pomodoro "

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

        rptCertificato = New Rpt_CertificatoPomodoro

        RptRiepilogoImballiEntrata = New Rpt_RiepilogoImballi
        RptImballiUscita = New Rpt_ImballiUscita

    End Sub

#End Region


    '##################################################################################
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
            Numero_Copie = CInt(Stringa_Decodifica(CStr(Request.QueryString("nc")), _
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

        Str_Id_Agenda_Certificati = Session("Str_Id_Agenda_Certificati")

        Report = CInt(Session("ReportSelezionato"))

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))


        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String
        Dim Log_Errori As String = ""
        Dim Num_IdAgenda As Integer = 1
        Dim i As Integer
        Dim DT_Bolle As DataTable
        Dim Vet_IdAgenda As String()

        Select Case Report
            Case enum_CodificaStampe.Certificato_Pomodoro_Interno
                Nome_Documento = "CertificatoPomodoroInterno"
            Case enum_CodificaStampe.Certificato_Pomodoro_Esterno
                Nome_Documento = "CertificatoPomodoroEsterno"
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
                Dim DS As New DS_CertificatoPomodoro
                Dim DSRiepilogoImballiEntrata As New DS_Iva 'riciclo il dataset dell'iva, per gli imballi in entrata, se no fa casino con quelli in uscita
                Dim DSImballiUscita As New DS_ImballiUscita

                Try

                    'If PrintToPrinter = 0 Then
                    '    CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer
                    '    CrystalReportViewer1.Style.Add("LEFT", "-275px")
                    '    CrystalReportViewer1.Style.Add("TOP", "0px")
                    '    CrystalReportViewer1.Style.Add("POSITION", "Absolute")
                    '    Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)
                    'End If

                    Log_Errori = ""


                    Stampa_CertificatoPomodoro(DS, _
                                                 DSRiepilogoImballiEntrata, _
                                                DSImballiUscita, _
                                                Log_Errori, _
                                                Qs_Id_Agenda)

                    rptCertificato.OpenSubreport("Rpt_RiepilogoImballi.rpt").SetDataSource(DSRiepilogoImballiEntrata)

                    rptCertificato.OpenSubreport("Rpt_ImballiUscita.rpt").SetDataSource(DSImballiUscita)

                    '-------------------------------------------
                    'Nel caso di stampa del certificato esterno
                    'occorre bloccare la bolla
                    BloccaBolla_x_CertificatoEsterno(Log_Errori, _
                                                    Qs_Id_Agenda)



                Catch exc As Exception
                    Log_Errori += "- Caricamento: " + vbCrLf + exc.Message + vbCrLf
                End Try

                '-----------------------------------------
                '---- Salvataggio Log Errori -------------
                '-----------------------------------------

                Salva_Log(Log_Errori, Nome_Documento)

                '-----------------------------------------

                'If PrintToPrinter = 0 Then

                '    CrystalReportViewer1.DisplayToolbar = True

                '    'faccio il databind col visualizzatore dei reports...
                '    CrystalReportViewer1.ReportSource = rptCertificato
                '    CrystalReportViewer1.DataBind()

                'End If

                'array di dataset e data table
                'Dim dsRpt() As DataSet = {DS}
                Dim dsRpt() As DataSet = {DS, DSRiepilogoImballiEntrata, DSImballiUscita}

                'salvo il report nella sessione
                Session("DS") = dsRpt

            Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

                If Not IsNothing(Session("DS")) Then

                    'imposto la sorgente dati x il report...
                    rptCertificato.SetDataSource(CType(Session("DS")(0), DataSet))

                    'If PrintToPrinter = 0 Then

                    '    'faccio il databind col visualizzatore dei reports...
                    '    CrystalReportViewer1.ReportSource = rptCertificato
                    '    CrystalReportViewer1.DataBind()

                    'End If

                End If

            End If


            '==================================================================

            If PrintToPrinter = 0 Then

                Try
                    Session("Report") = rptCertificato
                    Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
                Catch ex As Exception
                    Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
                End Try

            Else

                Dim risp As Boolean = False
                Dim Mode_AgroWinSrvc_PrintUtility_PtP As Integer

                If Not IsNothing(ConfigurationSettings.AppSettings("Mode_AgroWinSrvc_PrintUtility_PtP")) AndAlso _
                    ConfigurationSettings.AppSettings("Mode_AgroWinSrvc_PrintUtility_PtP") <> "" Then
                    Mode_AgroWinSrvc_PrintUtility_PtP = CInt(ConfigurationSettings.AppSettings("Mode_AgroWinSrvc_PrintUtility_PtP"))
                Else
                    Mode_AgroWinSrvc_PrintUtility_PtP = 1
                End If


                Select Case Mode_AgroWinSrvc_PrintUtility_PtP

                    Case 0 'chiamata al servizio windows

                        ' Dichiara le variabili e restituisce le opzioni di esportazione.
                        Dim exportOpts As New ExportOptions
                        Dim diskOpts As New DiskFileDestinationOptions
                        exportOpts = rptCertificato.ExportOptions

                        ' Imposta il formato di esportazione.
                        exportOpts.ExportFormatType = ExportFormatType.CrystalReport
                        exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

                        ' Imposta le opzioni relative al file del disco.
                        Dim strPath, PathDatiServizioWinStampa, NomeFileRpt As String

                        'percorso dati
                        If Not IsNothing(ConfigurationSettings.AppSettings("PathDati_AgroWinSrvc_PrintUtility")) Then
                            PathDatiServizioWinStampa = CStr(ConfigurationSettings.AppSettings("PathDati_AgroWinSrvc_PrintUtility"))
                        Else
                            PathDatiServizioWinStampa = ""
                        End If

                        If Not PathDatiServizioWinStampa.EndsWith("\") Then PathDatiServizioWinStampa &= "\"

                        NomeFileRpt = Nome_Documento + "_" + MyRnd.Next.ToString() + "_" + CStr(Session("ASG_Utente_Username")) + "_" & Session.SessionID.ToString & ".rpt"

                        strPath = PathDatiServizioWinStampa + NomeFileRpt

                        diskOpts.DiskFileName = strPath
                        exportOpts.DestinationOptions = diskOpts

                        ' Esportazione del report.
                        rptCertificato.Export()

                        'risp = InviaStampaToServizioUtility(Request, rptCertificato, Log_Errori, PrintName, NomeFileRpt, Numero_Copie, Str_Flag_Fascicola, Start_Page, End_Page)
                        risp = InviaStampaToServizioUtility(Request, rptCertificato, Log_Errori, PrintName, NomeFileRpt,
                                                             "", "", objParametri_Server,
                                                             Numero_Copie, Str_Flag_Fascicola, Start_Page, End_Page)


                        '==========================================================
                    Case 1 'funzione print to printer

                        Try
                            rptCertificato.PrintOptions.PrinterName = PrintName

                            rptCertificato.PrintToPrinter(Numero_Copie, CBool(Str_Flag_Fascicola), Start_Page, End_Page)

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

                    Salva_Log(Log_Errori, Nome_Documento)


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
                          ByVal Nome_Documento As String)

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        'Dim Path_Errore, Str_Errore_Path, Nome_File As String
        Dim Nome_File As String

        If Log_Errori <> "" Then


            Log_Errori = Nome_Documento + _
                        ", piva = " + CStr(Piva) + _
                        ", id_agenda = " + CStr(Qs_Id_Agenda) + _
                        ", lav_cod = " + CStr(Lav_Cod) + _
            vbCrLf + vbCrLf + Log_Errori

            Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Piva)

            'GestioneFile_CreaCartellaNelPathWebConfig("Path_LogErrori_StampeEsportazioni", "Stampe_AccettazioneDaDiversi", Path_Errore, Str_Errore_Path)
            'If Str_Errore_Path = "" And Path_Errore <> "" Then
            'GestioneFile_CreaScriviFileConRicercaNome(Log_Errori, Path_Errore, Nome_File, Str_Errore_Path, )
            'End If

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                             "Stampe_AccettazioneDaDiversi", _
                                             Nome_File, _
                                             Session("ASG_Utente_Username"), _
                                             "CertificatiPomodoro.aspx", _
                                             Log_Errori)

        End If


    End Sub



    '#####################################################################################################
    Private Sub Stampa_CertificatoPomodoro(ByRef DS As DS_CertificatoPomodoro, _
                                            ByRef DSRiepilogoImballiEntrata As DS_Iva, _
                                            ByRef DSImballiUscita As DS_ImballiUscita, _
                                            ByRef Log_Errori As String, _
                                            ByVal Id_Agenda As Integer)


        '=============================================================
        'Dichiarazione variabili
        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim Dr As DS_CertificatoPomodoro.DS_CertificatoPomodoroRow
        Dim DT As DataTable
        Dim i As Integer
        Dim Numero_Bolla As String
        Dim x_Doc_Numero_Sin_Accettazione As String = ""
        Dim x_Doc_Numero_Accettazione As Integer = 0
        Dim x_Doc_Numero_Des_Accettazione As String = ""
        Dim x_Lunghezza_Sin_Accett As Integer
        Dim x_Lunghezza_Centro_Accett As Integer
        Dim x_Lunghezza_Des_Accett As Integer
        Dim x_CarattereFormattazione_Accett As String
        Dim x_Doc_Numero_Sin_Cert As String = ""
        Dim x_Doc_Numero_Cert As Integer = 0
        Dim x_Doc_Numero_Des_Cert As String = ""
        Dim Str_Bio_Conv As String
        Dim Str_Flag_Bio As String
        Dim Str_Surgelato As String
        Dim Str_Tipologia As String
        Dim x_Doc_Numero_Sin_DDTConf As String = ""
        Dim x_Doc_Numero_DDTConf As Integer = 0
        Dim x_Doc_Numero_Des_DDTConf As String = ""
        Dim Numero_DDTConf As String

        Dim Peso_Lordo, Tara_Veicolo, Tara_Imballi, Peso_Netto, Netto_Pag, Scarto As Double
        Dim Perc_Marcio, Perc_Verde, Perc_Inerti, Perc_Tot_Dif_Magg, Perc_Tot_Dif_Magg_Round As Double
        Dim Perc_Schiacciati, Perc_Immaturi, Perc_Scottature, Perc_Lesioni, Perc_Tot_Dif_Minori As Double
        Dim Grado_Brix, Indice_Prezzo_Grado_Brix As Double
        Dim Coefficiente, Franchigia As Double
        Dim Dif_MinoriXCoeff As Double
        Dim Dif_Magg_Franchigia As Double
        Dim Indice_Variazione_Prezzo As Double
        Dim Premio_Pomo_Tardivo As Double
        Dim Prezzo_Unitario_Contratto, Prezzo_Unitario_Finale As Double
        Dim Importo_Totale_Pag As Double


        '=============================================================
        'Lettura dei dati

        Try

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.CertificatoPomodoro_Stampa(Piva, _
                                                Id_Agenda, _
                                                0, 0, "", "", _
                                                 "", "", _
                                                 AGRODATAINIZIO, _
                                                 AGRODATAFINE, _
                                                 False, _
                                                 False, _
                                                "", "", _
                                                 "", "", _
                                                 objParametri_Server)

            'DT = NewCom_ADD_CertificatoPomodoro_Stampa(Server, Session, Page, _
            '                                                Piva, _
            '                                                Id_Agenda, _
            '                                                 0, 0, "", "", _
            '                                                 , , , , , , , )


        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '=============================================================
        'Valorizzazione del dataset

        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                '-----------------------------------------------------
                Dr = DS.DS_CertificatoPomodoro.NewDS_CertificatoPomodoroRow()
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

                            'Dr.Num_Certificato = .Item("Numero_Certificato_OLD")
                            x_Doc_Numero_Sin_Cert = .Item("Doc_Numero_Sin_Cert")
                            x_Doc_Numero_Cert = .Item("Doc_Numero_Cert")
                            x_Doc_Numero_Des_Cert = .Item("Doc_Numero_Des_Cert")

                            Dr.Num_Certificato = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                    x_Doc_Numero_Sin_Cert, _
                                                    x_Doc_Numero_Cert, _
                                                    x_Doc_Numero_Des_Cert)

                            'Dr.Data_Certificato = .Item("Data_Certificato_OLD")
                            Dr.Data_Certificato = .Item("Data_Certificato")

                            x_Doc_Numero_Sin_Accettazione = .Item("Doc_Numero_Sin")
                            x_Doc_Numero_Accettazione = .Item("Doc_Numero")
                            x_Doc_Numero_Des_Accettazione = .Item("Doc_Numero_Des")
                            x_Lunghezza_Sin_Accett = .Item("Lunghezza_Sin")
                            x_Lunghezza_Centro_Accett = .Item("Lunghezza_Centro")
                            x_Lunghezza_Des_Accett = .Item("Lunghezza_Des")
                            x_CarattereFormattazione_Accett = .Item("CarattereFormattazione")

                            Numero_Bolla = Ricava_NumeroDocumento_Con_Sequenza( _
                                                    x_Doc_Numero_Sin_Accettazione, _
                                                    x_Doc_Numero_Accettazione, _
                                                    x_Doc_Numero_Des_Accettazione, _
                                                    x_Lunghezza_Sin_Accett, _
                                                    x_Lunghezza_Centro_Accett, _
                                                    x_Lunghezza_Des_Accett, _
                                                    x_CarattereFormattazione_Accett)

                            Dr.Num_Bolla = Numero_Bolla
                            Dr.Data_Bolla = .Item("Data_Accett")
                            Dr.Cod_OPR_Contratto = .Item("Cod_Agrea_Contratto")

                            If InStr(CStr(.Item("Clausola")), "|") > 0 Then
                                Dr.Num_Clausola = CStr(.Item("Clausola")).Split("|")(0)
                                Dr.Data_Clausola = CStr(.Item("Clausola")).Split("|")(1)
                            Else
                                Dr.Num_Clausola = ""
                                Dr.Data_Clausola = ""
                            End If

                            If Dr.Num_Clausola <> "" And Dr.Data_Clausola <> "" Then
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

                            Dr.Prodotto = CStr(.Item("Veg_Des")).ToUpper + " " + CStr(objADDFun.TipologiaPomodoro_from_GrvaCod(.Item("Grva_Cod_Veg"))).ToUpper

                            If .Item("Flag_Surgelato") = 1 Then
                                Str_Surgelato = "SURGELATO"
                            Else
                                Str_Surgelato = ""
                            End If

                            Select Case CInt(.Item("Regolamento"))
                                Case enum_Cod_Regolamento.Regolamento_bio
                                    Str_Bio_Conv = "BIOLOGICO"
                                    Str_Flag_Bio = "X"
                                Case Else
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
                            Dr.Comune_Trasformatore = .Item("localita_Trasf") + " (" + .Item("comuni_prov_Trasf") + ")"
                            Dr.CAP_Trasformatore = .Item("cap_Trasf")
                            Dr.Prov_Trasformatore = .Item("comuni_prov_Trasf")
                            Dr.Codice_Ass_Industriale = .Item("Codice_Ass_Ind")
                            Dr.Ass_Industriale = obj_TabOP.AssociazioneIndutriale_from_CodiceAssociazioneIndutriale(.Item("Codice_Ass_Ind"), Session("ASG_objParametri_Server"))

                            x_Doc_Numero_Sin_DDTConf = .Item("Doc_Numero_Sin_Conf")
                            x_Doc_Numero_DDTConf = .Item("Doc_Numero_Conf")
                            x_Doc_Numero_Des_DDTConf = .Item("Doc_Numero_Des_Conf")

                            Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                    x_Doc_Numero_Sin_DDTConf, _
                                                    x_Doc_Numero_DDTConf, _
                                                    x_Doc_Numero_Des_DDTConf)

                            Dr.Num_DDT = Numero_DDTConf
                            Dr.Data_DDT = .Item("Data_Conf")
                            Dr.Rag_Soc_Vettore = .Item("Rag_Soc_Vettore")
                            Dr.Targa_Automezzo = .Item("Targa_Automezzo")
                            Dr.Targa_Rimorchio = .Item("Targa_Rimorchio")

                            Dr.Data_Accettazione = .Item("Data_Accett")
                            Dr.Ora_Accettazione = CDate(.Item("Ora_Accett")).ToShortTimeString
                            Dr.Num_Tagliando_Pesa = "RCD" + CStr(.Item("Tagliando_Pesa"))

                            Dr.Tondo_X = ""
                            Dr.Allungato_X = ""
                            objADDFun.TondoLungo_from_GrvaCod(.Item("Grva_Cod_Veg"), Dr.Tondo_X, Dr.Allungato_X)
                            Dr.Biologico = Str_Flag_Bio

                            'nel caso del pomodoro:
                            'peso lordo = peso totale (lordo + tara camion + tara imballi)
                            'tara = tara automezzo + tara imballi
                            'peso netto = netto
                            Tara_Veicolo = RoundNumber_ParteIntera(.Item("Tara_Veicolo"))
                            Tara_Imballi = RoundNumber_ParteIntera(.Item("Tara_Imballi"))
                            Peso_Netto = RoundNumber_ParteIntera(.Item("Qta_Raccolta"))
                            Peso_Lordo = RoundNumber_ParteIntera(Peso_Netto + Tara_Veicolo + Tara_Imballi)

                            Dr.Peso_Lordo = Format(Peso_Lordo, "#,###,##0.00")
                            Dr.Tara_Veicolo = Format(Tara_Veicolo, "#,###,##0.00")
                            Dr.Tara_Imballi = Format(Tara_Imballi, "#,###,##0.00")
                            Dr.Peso_Netto = Format(Peso_Netto, "#,###,##0.00")

                            If InStr(CStr(.Item("GradoBrix_IndicePrezzo")), "_") > 0 Then
                                Grado_Brix = CDbl(CStr(.Item("GradoBrix_IndicePrezzo")).Split("_")(0))
                                Indice_Prezzo_Grado_Brix = CDbl(CStr(.Item("GradoBrix_IndicePrezzo")).Split("_")(1))
                            Else
                                Grado_Brix = 0
                                Indice_Prezzo_Grado_Brix = 0
                            End If

                            Dr.Grado_Brix = Format(Grado_Brix, "#,###,##0.000")
                            Dr.Indice_prezzo_Grado_Brix = Format(Indice_Prezzo_Grado_Brix, "#,###,##0.000")

                            Coefficiente = .Item("Coefficiente_DifMinori")
                            Franchigia = .Item("Franchigia_DifMaggiori")
                            Prezzo_Unitario_Contratto = CDbl(.Item("Prezzo_Unitario")) * 1000
                            If .Item("Premio_PomoTardivo") = "" Then
                                Premio_Pomo_Tardivo = 0
                            Else
                                Premio_Pomo_Tardivo = CDbl(.Item("Premio_PomoTardivo"))
                            End If
                            Dr.Premio_Pomo_Tardivo = Format(Premio_Pomo_Tardivo, "#,###,##0.00")

                        End If

                        'per ogni riga
                        Select Case .Item("tipo_cod")

                            Case enum_DanniRaccolta.Marcio
                                Perc_Marcio = .Item("val_cod")

                            Case enum_DanniRaccolta.Verde
                                Perc_Verde = .Item("val_cod")

                            Case enum_DanniRaccolta.Inerti
                                Perc_Inerti = .Item("val_cod")

                            Case enum_DanniRaccolta.FruttiSchiacciati
                                Perc_Schiacciati = .Item("val_cod")

                            Case enum_DanniRaccolta.FruttiImmaturi
                                Perc_Immaturi = .Item("val_cod")

                            Case enum_DanniRaccolta.FruttiScottati
                                Perc_Scottature = .Item("val_cod")

                            Case enum_DanniRaccolta.FruttiLesionati
                                Perc_Lesioni = .Item("val_cod")

                        End Select


                        'solo una volta, all'ultima riga
                        If i = DT.Rows.Count - 1 Then

                            objADDFun.Calcola_Dati_Certificato_Pomodoro( _
                                                        Peso_Lordo, _
                                                        Tara_Veicolo, _
                                                        Tara_Imballi, _
                                                        Peso_Netto, _
                                                        Perc_Marcio, _
                                                        Perc_Verde, _
                                                        Perc_Inerti, _
                                                        Perc_Schiacciati, _
                                                        Perc_Immaturi, _
                                                        Perc_Scottature, _
                                                        Perc_Lesioni, _
                                                        Grado_Brix, _
                                                        Indice_Prezzo_Grado_Brix, _
                                                        Coefficiente, _
                                                        Franchigia, _
                                                        Premio_Pomo_Tardivo, _
                                                        Prezzo_Unitario_Contratto, _
                                                        Perc_Tot_Dif_Magg, _
                                                        Perc_Tot_Dif_Magg_Round, _
                                                        Netto_Pag, _
                                                        Scarto, _
                                                        Perc_Tot_Dif_Minori, _
                                                        Dif_MinoriXCoeff, _
                                                        Dif_Magg_Franchigia, _
                                                        Indice_Variazione_Prezzo, _
                                                        Prezzo_Unitario_Finale, _
                                                        Importo_Totale_Pag)

                            Dr.Perc_Marcio = Format(Perc_Marcio, "#,###,##0.000")
                            Dr.Perc_Verde = Format(Perc_Verde, "#,###,##0.000")
                            Dr.Perc_Inerti = Format(Perc_Inerti, "#,###,##0.000")
                            Dr.Perc_Tot_Dif_Magg = Format(Perc_Tot_Dif_Magg, "#,###,##0.000")
                            Dr.Perc_Tot_Dif_Magg_Round = Format(Perc_Tot_Dif_Magg_Round, "#,###,##0.000")

                            Dr.Perc_Schiacciati = Format(Perc_Schiacciati, "#,###,##0.000")
                            Dr.Perc_Immaturi = Format(Perc_Immaturi, "#,###,##0.000")
                            Dr.Perc_Scottature = Format(Perc_Scottature, "#,###,##0.000")
                            Dr.Perc_Lesioni = Format(Perc_Lesioni, "#,###,##0.000")
                            Dr.Perc_Tot_Dif_Minori = Format(Perc_Tot_Dif_Minori, "#,###,##0.000")

                            Scarto = RoundNumber_ParteIntera(Scarto)
                            Netto_Pag = RoundNumber_ParteIntera(Netto_Pag)

                            Dr.Scarto = Format(Scarto, "#,###,##0.00")
                            Dr.Netto_Pag = Format(Netto_Pag, "#,###,##0.00")

                            Dr.Tasso_Riduzione_1 = Format(Dif_MinoriXCoeff, "#,###,##0.000")
                            Dr.Tasso_Riduzione_2 = Format(Dif_Magg_Franchigia, "#,###,##0.000")
                            Dr.Indice_Var_Prezzo = Format(Indice_Variazione_Prezzo, "#,###,##0.000")
                            Dr.Prezzo_Unitario_Contratto = Format(Prezzo_Unitario_Contratto, "#,###,##0.00")
                            Dr.Prezzo_Unitario_Finale = Format(Prezzo_Unitario_Finale, "#,###,##0.00")
                            Dr.Importo_Totale_Pag = Format(Importo_Totale_Pag, "#,###,##0.00")

                        End If

                    End With


                Next

                '-----------------------------------------------------
                DS.DS_CertificatoPomodoro.Rows.Add(Dr)
                '-----------------------------------------------------

            End If


        Catch ex As Exception
            Log_Errori += "- Valorizzazione del dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try


        '=============================================================
        'Valorizzazione del report

        Select Case Report

            Case enum_CodificaStampe.Certificato_Pomodoro_Interno

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
            Dim num_colli As Integer = 0

            Carica_DSImballaggi_StampaBollaECertificato(objParametri_Server, _
                                                        Log_Errori, _
                                                        DSRiepilogoImballiEntrata, _
                                                        DSImballiUscita, _
                                                        Num_Colli, _
                                                        Tara_Imballi, _
                                                        True, _
                                                        True, _
                                                        Piva, _
                                                        Id_Agenda, _
                                                        Lav_Cod)


        Catch ex As Exception
            Log_Errori += "- Caricamento dataset Imballaggi: " + vbCrLf + ex.Message + vbCrLf
        End Try


        '=============================================================
        'Aggancio dataset

        Try

            'imposto il dataset sul report
            rptCertificato.SetDataSource(DS)

            RptRiepilogoImballiEntrata.SetDataSource(DSRiepilogoImballiEntrata)

            RptImballiUscita.SetDataSource(DSImballiUscita)


        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try



    End Sub


    '#####################################################################################################
    Private Sub BloccaBolla_x_CertificatoEsterno(ByRef Log_Errori As String, _
                                                    ByVal Id_Agenda As Integer)

        'Se sto stampando il certificato esterno
        If Report = enum_CodificaStampe.Certificato_Pomodoro_Esterno Then

            'controllo id_agenda valorizzato
            If Id_Agenda > 0 Then

                Dim obj_AgendaW As New AgronicaCoreContabDAL.Agenda_W

                obj_AgendaW.Agenda_Blocca(Piva, _
                                            0, _
                                            Id_Agenda, _
                                            CStr(Session("ASG_Utente_Username")), _
                                            Date.Now, _
                                            "", _
                                            Session("ASG_objParametri_Server"))

            Else
                'errore!
                Log_Errori += "Certificato Esterno: non è stato possibile bloccare la bolla, poichè l'id_agenda è nullo!"
            End If

        End If

    End Sub



End Class
