Imports CrystalDecisions.Shared
Imports CrystalDecisions.CrystalReports.Engine
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports System.Xml
Imports AgronicaCoreDataProvider.Agro_Math


Public Class RegistroCaricoScaricoPomodoro
    Inherits System.Web.UI.Page

    '----------------------------
    '   DICHIARAZIONE REPORT
    '----------------------------
    Private rptCaricoScaricoPomodoro As Rpt_RegistroCaricoScaricoPomodoro

    Dim Piva As String
    Dim Data_Stampa As String
    Dim Num_Pagina As Integer
    Dim Num_Riga As Integer
    Dim Tipo_Registro As enum_TipoRegistroCaricoScaricoPomodoro
    Dim Mat_Cod As Integer
    Dim Flag_SalvaProgressivi As Boolean

    Dim PrintName As String
    Dim PrintToPrinter As Integer
    Dim Numero_Copie As Integer
    Dim Str_Flag_Fascicola As String
    Dim Start_Page As Integer
    Dim End_Page As Integer

    Private Const MAX_NUM_DETTAGLI As Integer = 25

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri


#Region " Registro Carico e Scarico Pomodoro "

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

        rptCaricoScaricoPomodoro = New Rpt_RegistroCaricoScaricoPomodoro

    End Sub

#End Region


    '##################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load


        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        '##############################################################
        '############  Lettura Parametri Query String #################
        '##############################################################

        Piva = Stringa_Decodifica(CStr(Request.QueryString("p")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Data_Stampa = Stringa_Decodifica(CStr(Request.QueryString("d")), _
                                    AgroKey_EncoderDecoder, _
                                    Server)

        Num_Pagina = CInt(Stringa_Decodifica(CStr(Request.QueryString("pg")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Num_Riga = CInt(Stringa_Decodifica(CStr(Request.QueryString("r")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Tipo_Registro = CInt(Stringa_Decodifica(CStr(Request.QueryString("t")), _
                                  AgroKey_EncoderDecoder, _
                                  Server))

        Flag_SalvaProgressivi = CBool(Stringa_Decodifica(CStr(Request.QueryString("fsp")), _
                               AgroKey_EncoderDecoder, _
                               Server))

        Mat_Cod = CInt(Stringa_Decodifica(CStr(Request.QueryString("m")), _
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




        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        Dim Nome_Documento As String = "Registro_CaricoScarico_Pomodoro"
        Dim Log_Errori As String

        Dim Num_IdAgenda As Integer = 1
        Dim i As Integer
        Dim DT_Bolle As DataTable
        Dim Vet_IdAgenda As String()

        Dim Riga_UltimoValore As Integer = 0
        Dim Tot_Righe_Dettagli As Integer = 0


        If PrintToPrinter = 1 Then

            'If Not IsNothing(Str_Id_Agenda_Bolle.Split("|")) Then
            '    Vet_IdAgenda = Str_Id_Agenda_Bolle.Split("|")
            'Else
            '    Vet_IdAgenda(0) = Str_Id_Agenda_Bolle
            'End If

            'Num_IdAgenda = Vet_IdAgenda.Length

        End If


        For i = 0 To Num_IdAgenda - 1

            If PrintToPrinter = 1 Then

                'Qs_Id_Agenda = CInt(Vet_IdAgenda(i))

                'rptCaricoScaricoPomodoro.Close()
                'rptCaricoScaricoPomodoro.Dispose()
                'rptCaricoScaricoPomodoro = Nothing

                'rptCaricoScaricoPomodoro = New Object

            End If

            If Not Me.IsPostBack Then

                '--------------------------------------------
                ' DICHIARAZIONE  E INIZIALIZZAZIONE DATASET
                '--------------------------------------------
                Dim DS As New DS_RegistroCaricoScaricoPomodoro

                Try

                    Log_Errori = ""

                    Stampa_RegistroCaricoScaricoPomodoro(DS, _
                                    Log_Errori, _
                                    Tot_Righe_Dettagli, _
                                    Riga_UltimoValore)

                    'rptCaricoScaricoPomodoro.SetParameterValue("norows", MAX_NUM_DETTAGLI)

                    ''CType(rptCaricoScaricoPomodoro.Section4.ReportObjects("FNumPagine"), CrystalDecisions.CrystalReports.Engine.FieldObject).FieldFormat


                Catch exc As Exception
                    Log_Errori += "- Caricamento: " + vbCrLf + exc.Message + vbCrLf
                End Try
                '-----------------------------------------


                '-----------------------------------------
                '---- Salvataggio Log Errori -------------
                '-----------------------------------------

                Salva_Log(Log_Errori, Nome_Documento)


                '-----------------------------------------

                'array di dataset e data table
                'Dim dsRpt() As DataSet = {DS, DSRiepilogoImballiEntrata, DSImballiUscita}
                Dim dsRpt() As DataSet = {DS}

                'salvo il report nella sessione
                Session("DS") = dsRpt

            Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

                If Not IsNothing(Session("DS")) Then

                    'imposto la sorgente dati x il report...
                    rptCaricoScaricoPomodoro.SetDataSource(CType(Session("DS")(0), DataSet))

                End If

            End If


            '==================================================================

            If PrintToPrinter = 0 Then


                If Riga_UltimoValore <> 0 Then

                    Salva_Progressivi(Log_Errori, _
                                        Riga_UltimoValore, _
                                        Tot_Righe_Dettagli)

                End If


                Try
                    Session("Report") = rptCaricoScaricoPomodoro
                    Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
                Catch ex As Exception
                    Log_Errori += "- Export: " + vbCrLf + ex.Message + vbCrLf
                End Try

            Else

                'Dim risp As Boolean = False
                'Dim Mode_AgroWinSrvc_PrintUtility_PtP As Integer

                'If Not IsNothing(ConfigurationSettings.AppSettings("Mode_AgroWinSrvc_PrintUtility_PtP")) AndAlso _
                '    ConfigurationSettings.AppSettings("Mode_AgroWinSrvc_PrintUtility_PtP") <> "" Then
                '    Mode_AgroWinSrvc_PrintUtility_PtP = CInt(ConfigurationSettings.AppSettings("Mode_AgroWinSrvc_PrintUtility_PtP"))
                'Else
                '    Mode_AgroWinSrvc_PrintUtility_PtP = 1
                'End If


                'Select Case Mode_AgroWinSrvc_PrintUtility_PtP

                '    Case 0 'chiamata al servizio windows

                '        risp = InviaStampaToServizioUtility(Request, rptCaricoScaricoPomodoro, Log_Errori, PrintName, Numero_Copie, Str_Flag_Fascicola, Start_Page, End_Page)

                '        '==========================================================
                '    Case 1 'funzione print to printer

                '        Try
                '            rptCaricoScaricoPomodoro.PrintOptions.PrinterName = PrintName

                '            rptCaricoScaricoPomodoro.PrintToPrinter(Numero_Copie, CBool(Str_Flag_Fascicola), Start_Page, End_Page)

                '            risp = True

                '        Catch ex As Exception
                '            Log_Errori += "Stampa del report non riuscita."
                '        End Try

                '        '==========================================================
                '    Case Else
                '        'non gestito
                '        Log_Errori += "Il file di configurazione dell'AgronicaStampe non è impostato correttamente."
                '        '==========================================================
                'End Select


                'If risp = True Then
                '    'ok
                '    Dim a As Integer = 1
                'Else

                '    '-----------------------------------------
                '    '---- Salvataggio Log Errori -------------
                '    '-----------------------------------------

                '    Salva_Log(Log_Errori, Nome_Documento)

                'End If

            End If

        Next

        FileSystem.ChDir("C:\")

        If PrintToPrinter = 1 Then
            'Dim strClose As String = "<script language='javascript'>window.close()</script>"
            'Me.FindControl("Form1").Controls.Add(New LiteralControl(strClose))
        End If



    End Sub

    '#####################################################################################################
    Private Sub Salva_Log(ByRef Log_Errori As String, _
                          ByVal Nome_Documento As String)

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------
        Dim Nome_File As String

        If Log_Errori <> "" Then

            Log_Errori = Nome_Documento + _
                        ", piva = " + CStr(Piva) + _
                        ", Data di Stampa = " + CStr(Data_Stampa) + _
                        ", Numero Iniziale di Pagina = " + CStr(Num_Pagina) + _
                        ", Numero Iniziale di Riga = " + CStr(Num_Riga) + _
                        ", Tipo Report = " + CStr(Tipo_Registro) + _
                        ", Salva Progressivi = " + CStr(Flag_SalvaProgressivi) + _
                        vbCrLf + vbCrLf + Log_Errori

            Nome_File = "Log_Errori_" + Nome_Documento + "_" + CStr(Session("ASG_Utente_Username")) + "_" + CStr(Piva)

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server, _
                                             "Stampe_AccettazioneDaDiversi", _
                                             Nome_File, _
                                             Session("ASG_Utente_Username"), _
                                             "RegistroCaricoScaricoPomodoro.aspx", _
                                             Log_Errori)

        End If


    End Sub


    '#####################################################################################################
    Private Sub Stampa_RegistroCaricoScaricoPomodoro(ByRef DS As DS_RegistroCaricoScaricoPomodoro, _
                                                        ByRef Log_Errori As String, _
                                                        ByRef Tot_Righe_Dettagli As Integer, _
                                                        ByRef Riga_UltimoValore As Integer)


        '=============================================================
        'Dichiarazione variabili
        Dim objADDFun As New AgronicaCoreStampeDAL.AccettazioneDaDiversi_Funzioni
        Dim DT As DataTable
        Dim Dr_Intestazione As DS_RegistroCaricoScaricoPomodoro.DS_IntestazioneCaricoScaricoPomodoroRow
        Dim Dr_Dettagli As DS_RegistroCaricoScaricoPomodoro.DS_DettagliCaricoScaricoPomodoroRow
        Dim i As Integer
        Dim Descr_Specie As String
        Dim x_Doc_Numero_Sin_Accettazione As String = ""
        Dim x_Doc_Numero_Accettazione As Integer = 0
        Dim x_Doc_Numero_Des_Accettazione As String = ""
        Dim x_Lunghezza_Sin_Accett As Integer
        Dim x_Lunghezza_Centro_Accett As Integer
        Dim x_Lunghezza_Des_Accett As Integer
        Dim x_CarattereFormattazione_Accett As String
        Dim Numero_Bolla As String
        Dim x_Doc_Numero_Sin_DDTConf As String = ""
        Dim x_Doc_Numero_DDTConf As Integer = 0
        Dim x_Doc_Numero_Des_DDTConf As String = ""
        Dim Numero_DDTConf As String

        Dim x_Doc_Numero_Sin_Cert As String = ""
        Dim x_Doc_Numero_Cert As Integer = 0
        Dim x_Doc_Numero_Des_Cert As String = ""

        Dim Peso_Netto_TotPrec As Double = 0
        Dim Degrado_TotPrec As Double = 0
        Dim Netto_Pag_TotPrec As Double = 0

        Dim Peso_Netto_TotGiorno As Double = 0
        Dim Degrado_TotGiorno As Double = 0
        Dim Netto_Pag_TotGiorno As Double = 0

        Dim Peso_Netto_TotGenerale As Double = 0
        Dim Degrado_TotGenerale As Double = 0
        Dim Netto_Pag_TotGenerale As Double = 0

        Dim Peso_Netto As Double = 0
        Dim Degrado_Perc As Double = 0
        Dim Degrado As Double = 0
        Dim Netto_Pagamento As Double = 0
        Dim Udm_Cod As Integer

        Dim Data_Inizio As Date
        Dim Data_Fine As Date

        '=============================================================
        'Lettura dei dati


        Try

            Data_Inizio = "01/01/" + CStr(Year(CDate(Data_Stampa)))
            Data_Fine = DateAdd(DateInterval.Day, -1, CDate(Data_Stampa))

            'DT = NewCom_ADD_RegistroCaricoScaricoPomodoro_Leggi(Server, Session, Page, _
            '                                                    Piva, _
            '                                                    Mat_Cod, _
            '                                                    Tipo_Registro, _
            '                                                    Data_Stampa, _
            '                                                    , )

            Dim ADD As New AgronicaCoreStampeDAL.AccettazioneDaDiversi

            DT = ADD.RegistroCaricoScaricoPomodoro(Piva, _
                                                    Mat_Cod, _
                                                    Tipo_Registro, _
                                                    Data_Stampa, _
                                                     "", "", _
                                                     objparametri_server)



        Catch ex As Exception
            Log_Errori += "- Lettura dei dati: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '=============================================================
        'Valorizzazione del dataset

        Try

            Dr_Intestazione = DS.DS_IntestazioneCaricoScaricoPomodoro.NewDS_IntestazioneCaricoScaricoPomodoroRow()

            Dr_Intestazione.Piva = Piva
            Dr_Intestazione.Data_Stampa = Data_Stampa
            Dr_Intestazione.Num_Pagina = Num_Pagina
            Dr_Intestazione.Tipo_Registro = Tipo_Registro

            'numero di riga di partenza: sottraggo uno perchè dentro al ciclo sommo 1, anche al primo giro
            Num_Riga -= 1

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                Tot_Righe_Dettagli = DT.Rows.Count

                For i = 0 To Tot_Righe_Dettagli - 1

                    Dr_Dettagli = DS.DS_DettagliCaricoScaricoPomodoro.NewDS_DettagliCaricoScaricoPomodoroRow()

                    Dr_Dettagli.Riga = i + 1

                    'numero di riga in progressione con le pagine già stampate
                    Num_Riga += 1
                    Dr_Dettagli.Num_Reg = Num_Riga

                    With DT.Rows(i)

                        Select Case Tipo_Registro

                            Case enum_TipoRegistroCaricoScaricoPomodoro.Contrattato

                                'Dr_Dettagli.Num_Certificato = .Item("Numero_Certificato_OLD")
                                x_Doc_Numero_Sin_Cert = .Item("Doc_Numero_Sin_Cert")
                                x_Doc_Numero_Cert = .Item("Doc_Numero_Cert")
                                x_Doc_Numero_Des_Cert = .Item("Doc_Numero_Des_Cert")

                                Dr_Dettagli.Num_Certificato = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                                x_Doc_Numero_Sin_Cert, _
                                                                x_Doc_Numero_Cert, _
                                                                x_Doc_Numero_Des_Cert)

                                Dr_Dettagli.Data_Certificato = CDate(.Item("Data_Certificato")).ToShortDateString
                                Dr_Dettagli.Num_Contratto = .Item("Contratto_Numero")

                            Case enum_TipoRegistroCaricoScaricoPomodoro.NonContrattato

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

                                Dr_Dettagli.Num_Bolla = Numero_Bolla
                                Dr_Dettagli.Data_Bolla = CDate(.Item("Data_Bolla")).ToShortDateString

                        End Select

                        x_Doc_Numero_Sin_DDTConf = .Item("Doc_Numero_Sin_Conf")
                        x_Doc_Numero_DDTConf = .Item("Doc_Numero_Conf")
                        x_Doc_Numero_Des_DDTConf = .Item("Doc_Numero_Des_Conf")

                        Numero_DDTConf = Ricava_NumeroDocumento_Senza_Sequenza( _
                                                x_Doc_Numero_Sin_DDTConf, _
                                                x_Doc_Numero_DDTConf, _
                                                x_Doc_Numero_Des_DDTConf)

                        Dr_Dettagli.Num_DDT = Numero_DDTConf
                        Dr_Dettagli.Data_DDT = CDate(.Item("Data_Conf")).ToShortDateString

                        Dr_Dettagli.Rag_Soc_OP = .Item("RagSoc_Conferente")
                        Dr_Dettagli.Indirizzo_OP = .Item("ind_des_conferente")
                        Dr_Dettagli.Comune_OP = .Item("Comune_Conferente")
                        Dr_Dettagli.Prov_OP = .Item("prov_conferente")

                        'Descr_Specie = .Item("Mat_Des_Raccolta")

                        'x_Tipo_Peso = .Item("Tipo_Peso")
                        'x_Peso = .Item("Peso")
                        'x_Tara_Veicolo = .Item("Tara_Veicolo")
                        'x_Tara_Veicolo = Arrotonda_Unita(x_Tara_Veicolo)
                        'x_Tara_Imballi = .Item("Tara_Imballi")
                        'x_Tara_Imballi = Arrotonda_Unita(x_Tara_Imballi)

                        'Peso_Lordo = Calcola_PesoLordo(x_Tipo_Peso, x_Peso, x_Tara_Imballi)
                        'Peso_Lordo = Arrotonda_Unita(Peso_Lordo)

                        'Peso_Totale = Arrotonda_Unita(Peso_Lordo + x_Tara_Veicolo)

                        'Peso_Netto = Calcola_PesoNetto(x_Tipo_Peso, x_Peso, x_Tara_Imballi)
                        'Peso_Netto = Arrotonda_Unita(Peso_Netto)

                        Degrado_Perc = .Item("Variazione_Raccolta")
                        Peso_Netto = RoundNumber_ParteIntera(.Item("Qta_Raccolta"))
                        Udm_Cod = .Item("Udm_Cod_Raccolta")

                        objADDFun.Calcola_NettoPagamento(Peso_Netto, Degrado_Perc, Udm_Cod, Degrado, Netto_Pagamento)

                        Dr_Dettagli.Peso_Netto = Format(Peso_Netto, "#,###,##0")
                        Dr_Dettagli.Degrado = Format(Degrado, "#,###,##0")
                        Dr_Dettagli.Netto_Pag = Format(Netto_Pagamento, "#,###,##0")

                        Peso_Netto_TotGiorno += Peso_Netto
                        Degrado_TotGiorno += Degrado
                        Netto_Pag_TotGiorno += Netto_Pagamento

                    End With

                    '-----------------------------------------------------
                    DS.DS_DettagliCaricoScaricoPomodoro.Rows.Add(Dr_Dettagli)
                    '-----------------------------------------------------
                Next

            Else
                'NON CI SONO DATI
                Flag_SalvaProgressivi = False
            End If

            Select Case Tipo_Registro
                Case enum_TipoRegistroCaricoScaricoPomodoro.Contrattato
                    Descr_Specie = "POMODORO - CONTRATTATO"
                Case enum_TipoRegistroCaricoScaricoPomodoro.NonContrattato
                    Descr_Specie = "POMODORO - FUORI CONTRATTO"
            End Select

            Dr_Intestazione.Specie = Descr_Specie

            Num_Riga += 1
            Dr_Intestazione.Num_Reg_TotPrec = Num_Riga
            Num_Riga += 1
            Dr_Intestazione.Num_Reg_TotGiorno = Num_Riga
            Num_Riga += 1
            Dr_Intestazione.Num_Reg_TotGenerale = Num_Riga

            'Calcola_TotaliPrecedenti_RegCaricoScaricoPomodoro(Server, Session, Page, _
            '                                                    Tipo_Registro, _
            '                                                    Piva, _
            '                                                    Data_Inizio, _
            '                                                    Data_Fine, _
            '                                                    Peso_Netto_TotPrec, _
            '                                                    Degrado_TotPrec, _
            '                                                    Netto_Pag_TotPrec)

            objADDFun.Calcola_TotaliPrecedenti_RegCaricoScaricoPomodoro(objParametri_Server, _
                                                                Tipo_Registro, _
                                                                Piva, _
                                                                Data_Inizio, _
                                                                Data_Fine, _
                                                                Peso_Netto_TotPrec, _
                                                                Degrado_TotPrec, _
                                                                Netto_Pag_TotPrec)

            Dr_Intestazione.Peso_Netto_TotPrec = Format(Peso_Netto_TotPrec, "#,###,##0")
            Dr_Intestazione.Degrado_TotPrec = Format(Degrado_TotPrec, "#,###,##0")
            Dr_Intestazione.Netto_Pag_TotPrec = Format(Netto_Pag_TotPrec, "#,###,##0")

            Dr_Intestazione.Peso_Netto_TotGiorno = Format(Peso_Netto_TotGiorno, "#,###,##0")
            Dr_Intestazione.Degrado_TotGiorno = Format(Degrado_TotGiorno, "#,###,##0")
            Dr_Intestazione.Netto_Pag_TotGiorno = Format(Netto_Pag_TotGiorno, "#,###,##0")

            Peso_Netto_TotGenerale = Peso_Netto_TotGiorno + Peso_Netto_TotPrec
            Degrado_TotGenerale = Degrado_TotGiorno + Degrado_TotPrec
            Netto_Pag_TotGenerale = Netto_Pag_TotGiorno + Netto_Pag_TotPrec

            Dr_Intestazione.Peso_Netto_TotGenerale = Format(Peso_Netto_TotGenerale, "#,###,##0")
            Dr_Intestazione.Degrado_TotGenerale = Format(Degrado_TotGenerale, "#,###,##0")
            Dr_Intestazione.Netto_Pag_TotGenerale = Format(Netto_Pag_TotGenerale, "#,###,##0")

            'Pag_UltimoValore = Num_Pagina
            Riga_UltimoValore = Num_Riga

            '-----------------------------------------------------
            DS.DS_IntestazioneCaricoScaricoPomodoro.Rows.Add(Dr_Intestazione)
            '-----------------------------------------------------

        Catch ex As Exception
            Log_Errori += "- Valorizzazione del dataset: " + vbCrLf + ex.Message + vbCrLf + vbCrLf
        End Try

        '=============================================================
        'Aggancio il dataset

        Try

            'imposto il dataset sul report
            rptCaricoScaricoPomodoro.SetDataSource(DS)

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try



    End Sub


    '#####################################################################################################
    Private Sub Salva_Progressivi(ByRef Log_Errori As String, _
                                    ByVal Riga_UltimoValore As Integer, _
                                    ByVal Tot_Righe_Dettagli As Integer)


        If Flag_SalvaProgressivi = True Then

            Dim objSeqProg_W As New AgronicaCoreDataProvider.Sequenza_Progressivi_W
            Dim Codice_Riga As Integer
            Dim Codice_Pagina As Integer
            Dim Flag_Modifica As Boolean

            Dim Pag_UltimoValore As Integer
            Dim Num_Pag_Report As Integer

            Select Case Tipo_Registro
                Case enum_TipoRegistroCaricoScaricoPomodoro.Contrattato  'CONTRATTATO
                    Codice_Pagina = SEQ_PROG_NUMPAG_RegCS_PomoContrattato
                    Codice_Riga = SEQ_PROG_NUMRIGA_RegCS_PomoContrattato
                Case enum_TipoRegistroCaricoScaricoPomodoro.NonContrattato 'NON CONTRATTATO
                    Codice_Pagina = SEQ_PROG_NUMPAG_RegCS_PomoNoContrattato
                    Codice_Riga = SEQ_PROG_NUMRIGA_RegCS_PomoNoContrattato
            End Select


            Try

                'divido il numero di dettagli per il numero max di dettagli per pagina 
                '--> so quante pagine occuperanno i dettagli
                Num_Pag_Report = Math.Ceiling(Tot_Righe_Dettagli / MAX_NUM_DETTAGLI)

                'ultimo valore di pagina da salvare = numero pagina di inizio + numero pagine con dettagli + 1 (pagina finale di riepilogo) - 1 (perchè la prima pagina è già compresa nel num pagina di inizio)
                Pag_UltimoValore = Num_Pagina + Num_Pag_Report + 1 - 1

                Flag_Modifica = objSeqProg_W.Modifica_Base(Piva, _
                                                            Year(Data_Stampa), _
                                                            Codice_Pagina, _
                                                            "", _
                                                            "", _
                                                            0, _
                                                            Pag_UltimoValore, _
                                                            "", _
                                                            Session("ASG_objParametri_Server"))

                If Flag_Modifica = False Then
                    Log_Errori += "Si è verificato un errore durante il salvataggio del numero di pagina." + vbCrLf
                End If

            Catch ex As Exception
                Log_Errori += "Si è verificato il seguente errore durante il salvataggio del numero di pagina: " + ex.Message + vbCrLf
            End Try

            Flag_Modifica = False

            Try

                Flag_Modifica = objSeqProg_W.Modifica_Base(Piva, _
                                                            Year(Data_Stampa), _
                                                            Codice_Riga, _
                                                            "", _
                                                            "", _
                                                            0, _
                                                            Riga_UltimoValore, _
                                                            "", _
                                                            Session("ASG_objParametri_Server"))

                If Flag_Modifica = False Then
                    Log_Errori += "Si è verificato un errore durante il salvataggio del numero di riga." + vbCrLf
                End If

            Catch ex As Exception
                Log_Errori += "Si è verificato il seguente errore durante il salvataggio del numero di riga: " + ex.Message + vbCrLf
            End Try

        End If


    End Sub



End Class

