Option Strict Off

Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreAnagrafeDAL
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.ConnessioniTransazioni
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports AgronicaCorePianoConcimazioneBIZ
Imports CrystalDecisions.Shared

Public Class PUA_Stampa
    Inherits System.Web.UI.Page

    Dim objParametri_Super_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Utenti As AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim personalizzazioniGraficheCliente As PersonalizzazioniGraficheCliente = Nothing

    Public Qs_Piva As String
    Public Qs_PUA_Cod As Integer
    Public Qs_Regolamento_Cod As Integer
    Public Qs_Tipo As enum_PUA_Tipo
    Dim Qs_Anteprima As String

    Dim Log_Errori As String = ""
    Dim Nome_Documento As String = "PUA"

    Private rptPua As RPT_PUA
    Private rptFooterLogo As FooterLogo

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Session("ASG_Utente_Username") = "" Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If

        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))
        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Utenti = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Utenti"))

        Dim UtenteAbilitato As Boolean

        Dim objPermessi As New AgronicaCoreUtentiDAL.Utenti_Permessi_R
        UtenteAbilitato = objPermessi.Controlla_Permessi_Utente(
                            Session("ASG_Utente_Username"),
                            Session("ASG_IdServizio"),
                            TipiEnumerativi.enum_Security_Attivita.Gest_PUA_2,
                            TipiEnumerativi.enum_Security_Operazione.Lettura,
                            Date.Now,
                            "",
                            objParametri_Utenti)

        If UtenteAbilitato = False Then
            Response.Redirect("Messaggi/AccessoNegato.htm")
        End If


        If Not IsNothing(Request.QueryString("tipo")) Then
            Qs_Tipo = Stringa_Decodifica(Request.QueryString("tipo").ToString,
                                                AgroKey_EncoderDecoder,
                                                Server)

        End If

        If Not IsNothing(Request.QueryString("p")) Then
            Qs_Piva = Stringa_Decodifica(Request.QueryString("p").ToString,
                                    AgroKey_EncoderDecoder,
                                    Server)

        End If


        If Not IsNothing(Request.QueryString("q")) Then
            Qs_PUA_Cod = Stringa_Decodifica(Request.QueryString("q").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)

        End If

        If Not IsNothing(Request.QueryString("r")) Then
            Qs_Regolamento_Cod = Stringa_Decodifica(Request.QueryString("r").ToString,
                                            AgroKey_EncoderDecoder,
                                            Server)

        End If

        If Not IsNothing(Request.QueryString("anteprima")) Then

            Qs_Anteprima = Stringa_Decodifica(Request.QueryString("anteprima").ToString,
                                                      AgroKey_EncoderDecoder,
                                                      Server)
        Else
            Qs_Anteprima = "0"
        End If

        personalizzazioniGraficheCliente = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        'Dim CrystalReportViewer1 As CrystalDecisions.Web.CrystalReportViewer

        If Not Me.IsPostBack Then

            'CrystalReportViewer1 = New CrystalDecisions.Web.CrystalReportViewer

            'CrystalReportViewer1.Style.Add("LEFT", "-275px")
            'CrystalReportViewer1.Style.Add("TOP", "0px")
            'CrystalReportViewer1.Style.Add("POSITION", "Absolute")
            'Me.FindControl("Form1").Controls.Add(CrystalReportViewer1)

            rptPua = New RPT_PUA
            rptFooterLogo = New FooterLogo

            Dim Perc_Zootecnico_Testata As Decimal = 0
            Dim DataInizio As Date
            Dim DataFine As Date
            Dim Sa_Cod As Integer = 0

            Try
                Dati_Pua(Qs_Piva, Qs_PUA_Cod, Qs_Regolamento_Cod, objParametri_Server, rptPua, Perc_Zootecnico_Testata, DataInizio, DataFine, Sa_Cod)
            Catch ex As Exception
                Log_Errori += "- Dati_Pua: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim DsEffluenti As New DS_PUA_Effluenti
            Dim DsEffluentiDistribuiti As New DS_PUA_Effluenti

            Dim HashFerCodDigestati As New Hashtable
            Dim HashFerCodLetami As New Hashtable
            Dim HashFerCodLiquami As New Hashtable
            Dim str_FerCod_Org As String = ""

            Try
                publicCarica_DsEffluenti(DsEffluenti, DsEffluentiDistribuiti, Qs_Piva, Sa_Cod, Qs_PUA_Cod, Qs_Regolamento_Cod, DataInizio, DataFine, Perc_Zootecnico_Testata, objParametri_Server, rptPua,
                                     str_FerCod_Org, HashFerCodDigestati, HashFerCodLetami, HashFerCodLiquami)
            Catch ex As Exception
                Log_Errori += "- publicCarica_DsEffluenti: " + vbCrLf + ex.Message + vbCrLf
            End Try


            Dim DsPianoDistribuzione As New DS_PUA_PianoDistribuzione
            Dim DsPianoDistribuzioneBilancio As New DS_PUA_PianoDistribuzione
            Dim DS_LogoFooter As New DS_LogoFooter

            Dim listPiano As New List(Of PUA_Appezzamento)
            Try
                publicCarica_DsPianoDistribuzione(DsPianoDistribuzione, DsPianoDistribuzioneBilancio, listPiano,
                                              Qs_Piva, Sa_Cod, Qs_PUA_Cod, Qs_Regolamento_Cod, Qs_Tipo, DataInizio, DataFine,
                                              str_FerCod_Org, HashFerCodDigestati, HashFerCodLetami, HashFerCodLiquami,
                                               objParametri_Server, rptPua)
            Catch ex As Exception
                Log_Errori += "- publicCarica_DsPianoDistribuzione: " + vbCrLf + ex.Message + vbCrLf
            End Try


            Dim DsFertilizzazioni As New DS_Pua_Fertilizzazioni
            Try
                publicCarica_DsFertilizzazioni(DsFertilizzazioni, Qs_Piva, Qs_PUA_Cod, Qs_Regolamento_Cod, DataInizio, DataFine, listPiano,
                                               objParametri_Server, rptPua)
            Catch ex As Exception
                Log_Errori += "- publicCarica_DsFertilizzazioni: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                Dim drLogo = DS_LogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
                Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(personalizzazioniGraficheCliente, Log_Errori, objParametri_Server)
                If logo.LogoStampe IsNot Nothing Then
                    drLogo.Logo = logo.LogoStampe
                    drLogo.TestoPostLogo = logo.TestoPostLogo
                    drLogo.TestoPreLogo = logo.TestoPreLogo
                End If
                DS_LogoFooter.DT_LogoFooter.Rows.Add(drLogo)
                rptPua.OpenSubreport("FooterLogo.rpt").SetDataSource(DS_LogoFooter)
            Catch ex As Exception
                Log_Errori += "- Carica Loghi: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Dim strFile As String = ""
            Dim NomeFile As String = "PUA_" & Qs_PUA_Cod & "_" & Qs_Piva & ".pdf"

            Try
                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptPua,
                                           enum_CategorieDocumenti.PianoConcimazione,
                                           "Piano Concimazione",
                                           NomeFile,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

                'strFile = SalvaPdf(objParametri_Server, rptBilancioNPK, NomeFile)
            Catch ex As Exception
                Log_Errori += "- SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
            End Try

            Try
                If strFile <> "" Then
                    Dim objAllegati As New AgronicaCoreScadenziario_BIZ.Allegati
                    Dim AllegatiDocumentiCod As Integer

                    Dim anno As Integer
                    If Not IsNothing(Session("Anno")) AndAlso IsNumeric(Session("Anno")) Then
                        anno = CInt(Session("Anno"))
                    Else
                        anno = Now.Year
                    End If

                    AllegatiDocumentiCod = objAllegati.SalvaAllegato(Qs_Piva.ToString,
                                                                 enum_CategorieDocumenti.PianoConcimazione,
                                                                 "Piano Concimazione",
                                                                 NomeFile,
                                                                 Session("Sottocartella"),
                                                                 Qs_PUA_Cod, Qs_Piva.ToString, "", "",
                                                                 CDate("01/01/" & anno.ToString),
                                                                 CDate("31/12/" & anno.ToString),
                                                                 objParametri_Server)

                    Dim objPC_dettagli_W As New AgronicaCorePianoConcimazioneDAL.PianoConcimazione_Dettagli_W
                    'INSERISCO IL CODICE ALLEGATO NELLA TABELLA Piano_Concimazione_Dettagli del PC salvato
                    If Not IsNothing(AllegatiDocumentiCod) AndAlso AllegatiDocumentiCod <> 0 Then
                        If Not objPC_dettagli_W.UpdateCodAllegato(Qs_PUA_Cod, 0, Qs_Piva.ToString, AllegatiDocumentiCod, objParametri_Server) Then
                            Throw New Exception("Non sono riuscito ad associare l'allegato al corrente Piano di Concimanzione. PC_cod =" & Qs_PUA_Cod.ToString)
                        End If
                    End If
                End If
            Catch ex As Exception

            End Try

            'Dim strFile As String = SalvaPdf(objParametri_Server, rptPua)
            Dim reportTemporano As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptPua.SaveAs(reportTemporano, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " + CStr(Qs_Piva) +
                 ", PUA_Cod = " + CStr(Qs_PUA_Cod) + vbCrLf + vbCrLf + Log_Errori

                Dim Nome_File As String = "Log_Errori_" + Nome_Documento

                Dim objLog As New AgronicaCoreDataProvider.LogProvider
                objLog.Gestione_LogErrori(objParametri_Server,
                                      "PUA",
                                       Nome_File & ".txt",
                                       Session("ASG_Utente_Username"),
                                        "PUA_Stampa",
                                         Log_Errori)

            End If

            If Qs_Anteprima = "1" Then
                'Session("Report") = rptPua
                'Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server))
                Response.Redirect("..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server))
            Else

                'Session("Report") = rptPua

                Dim UrlStampa As String
                Dim UrlFiltro As String

                'UrlStampa = "../VisualizzatoreReport.aspx" &
                '        "?anteprima=" + Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                '        "&pdf=" + Stringa_Codifica(strFile, AgroKey_EncoderDecoder, Server) & ""

                UrlStampa = "../VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) +
                              "&tmpReportPath=" + Stringa_Codifica(reportTemporano, AgroKey_EncoderDecoder, Server) &
                              "&NomePdf=" & Stringa_Codifica(NomeFile, AgroKey_EncoderDecoder, Server)

                UrlFiltro = "../Filtro_StampaScadenza.aspx" &
                       "?a=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Attivita.Gest_PUA_2), AgroKey_EncoderDecoder, Server) +
                       "&o=" + Stringa_Codifica(CStr(TipiEnumerativi.enum_Security_Operazione.Modifica), AgroKey_EncoderDecoder, Server) &
                       "&piva=" + Stringa_Codifica(Qs_Piva, AgroKey_EncoderDecoder, Server) &
                       "&pc=" + Stringa_Codifica(CStr(Qs_PUA_Cod), AgroKey_EncoderDecoder, Server) &
                       "&scadenziario=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) &
                       "&pdf=" + Stringa_Codifica(strFile, AgroKey_EncoderDecoder, Server)

                Dim strOpen As String = "<script language='javascript'>" & vbNewLine &
                                    "window.open('" & UrlStampa & "');" & vbNewLine &
                                    "location.href='" & UrlFiltro & "';" & vbNewLine &
                                    "</script>"

                Me.Page.FindControl("Form1").Controls.Add(New LiteralControl(strOpen))


                Exit Sub

            End If

            'CrystalReportViewer1.ReportSource = rptPua
            'CrystalReportViewer1.DataBind()

            'Else 'ad ogni post back devo impostare la fonte dati x il visualizzatore..

            '    If Not IsNothing(Session("DS")) Then

            '        'imposto la sorgente dati x il report...
            '        rptPua.SetDataSource(CType(Session("DS")(0), DataSet))

            '        'faccio il databind col visualizzatore dei reports...
            '        CrystalReportViewer1.ReportSource = rptPua

            '        CrystalReportViewer1.DataBind()

            '    End If

        End If

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------

        ''==================================================================

        '' Dichiara le variabili e restituisce le opzioni di esportazione.
        'Dim exportOpts As New ExportOptions
        'Dim diskOpts As New DiskFileDestinationOptions

        'exportOpts = rptPua.ExportOptions

        '' Imposta il formato di esportazione.
        'exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
        'exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

        '' Imposta le opzioni relative al file del disco.
        'Dim strPath As String
        'Dim PathFileTemporanei As String

        'If ConfigurationSettings.AppSettings("CartellaFileTemporanei").ToString = "" Then
        '    PathFileTemporanei = Server.MapPath("../../File_Temporanei")
        'Else
        '    PathFileTemporanei = ConfigurationSettings.AppSettings("CartellaFileTemporanei").ToString
        'End If

        'strPath = PathFileTemporanei & "\PUA_" & Qs_PUA_Cod & "_" & Session("ASG_Utente_Username") & ".pdf"
        'diskOpts.DiskFileName = strPath
        'exportOpts.DestinationOptions = diskOpts

        '' Esportazione del report.
        'rptPua.Export()

        '' Con il seguente codice il file pdf viene scritto 
        ''  nel browser del client.
        'Response.ClearContent()
        'Response.ClearHeaders()
        'Response.ContentType = "application/pdf"
        'Response.WriteFile(strPath)
        'Response.Flush()
        'Response.Close()

        '' il file esportato viene eliminato dal disco
        'System.IO.File.Delete(strPath)



    End Sub


    Public Sub Dati_Pua(ByVal Piva As String, ByVal Pua_Cod As Integer, ByVal Regolamento_Cod As AgronicaCoreDataProvider.TipiEnumerativi.enum_PUARegolamenti,
                               ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, ByRef rptPua As RPT_PUA, ByRef Perc_Zootecnico_Testata As Decimal,
                               ByRef DataInizio As Date, ByRef DataFine As Date, ByRef Sa_Cod As Integer)

        Dim Dt As New DataTable
        Dim Rag_Soc As String = ""
        Dim strErr As String = ""

        CType(rptPua.Section1.ReportObjects("TextMetodo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        CType(rptPua.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        CType(rptPua.Section1.ReportObjects("TextRegolamento"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = GetRegolamentoDes(Regolamento_Cod)
        CType(rptPua.Section1.ReportObjects("TextValidita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        CType(rptPua.Section1.ReportObjects("TextNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""
        CType(rptPua.Section1.ReportObjects("TextDichiarazioneNonUtilizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ""

        If Piva <> "" Then
            Dim objRagSoc As New AgronicaCoreAnagrafeDAL.Imprese_Read
            CType(rptPua.Section1.ReportObjects("TextRagSoc"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = objRagSoc.RagSoc_from_Piva(Piva, objParametri_Server)
        End If

        'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
        'If personalizzazioniGraficheCliente IsNot Nothing Then
        '    rptPua.Section5.ReportObjects("Text9").ObjectFormat.EnableSuppress = True
        '    rptPua.Section5.ReportObjects("Picture3").ObjectFormat.EnableSuppress = True
        '    rptPua.Section5.ReportObjects("Text6").ObjectFormat.EnableSuppress = True
        'End If

        Dim objTest As New AgronicaCorePUA_DAL.PUA_Testata_R
        Dim dtTest As DataTable = objTest.Leggi(Regolamento_Cod, Pua_Cod, Piva, AGRODATAINIZIO, AGRODATAFINE, "", "", objParametri_Server)

        If Not dtTest Is Nothing AndAlso dtTest.Rows.Count > 0 Then

            Dim ValiditaInizio As String
            Dim ValiditaFine As String
            ValiditaInizio = dtTest.Rows(0).Item("Validita_inizio")
            ValiditaFine = dtTest.Rows(0).Item("Validita_Fine")
            DataInizio = dtTest.Rows(0).Item("Validita_inizio")
            DataFine = dtTest.Rows(0).Item("Validita_Fine")

            Sa_Cod = 0
            If Not IsDBNull(dtTest.Rows(0).Item("sa_cod")) Then
                Sa_Cod = dtTest.Rows(0).Item("sa_cod")
            End If

            If ValiditaInizio = "01/01/1900" Then
                ValiditaInizio = ""
            End If
            If ValiditaFine = "31/12/2100" Then
                ValiditaFine = ""
            End If

            If ValiditaInizio <> "" Or ValiditaFine <> "" Then
                CType(rptPua.Section1.ReportObjects("TextValidita"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = ValiditaInizio &
                                                                                                                            IIf(ValiditaInizio <> "" And ValiditaFine <> "", " - ", "") _
                                                                                                                            & ValiditaFine
            End If

            If Not IsDBNull(dtTest.Rows(0).Item("Note")) Then
                CType(rptPua.Section1.ReportObjects("TextNota"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = dtTest.Rows(0).Item("Note")
            End If

            If Not IsDBNull(dtTest.Rows(0).Item("Perc_Zootecnico")) Then
                Perc_Zootecnico_Testata = dtTest.Rows(0).Item("Perc_Zootecnico")
            End If

            Select Case dtTest.Rows(0).Item("pua_tipo")
                Case enum_PUA_Tipo.Completo
                    CType(rptPua.Section1.ReportObjects("TextMetodo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Metodo Bilancio"
                Case enum_PUA_Tipo.Semplificato
                    CType(rptPua.Section1.ReportObjects("TextMetodo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Metodo Semplificato"
            End Select

            If Not IsDBNull(dtTest.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) AndAlso CInt(dtTest.Rows(0).Item("Flag_NonUtilizzo_Fertilizzanti")) > 0 Then
                CType(rptPua.Section1.ReportObjects("TextDichiarazioneNonUtilizzo"), CrystalDecisions.CrystalReports.Engine.TextObject).Text = "Dichiarazione di Non Utilizzo Fertilizzanti"
            End If

        End If

    End Sub


    Private Sub publicCarica_DsEffluenti(ByRef DsEffluenti As DS_PUA_Effluenti, ByRef DsEffluentiDistribuiti As DS_PUA_Effluenti,
                                         piva As String, sa_cod As Integer,
                                         pua_cod As Integer, regolamento_cod As Integer,
                                         DataInizio As Date, DataFine As Date,
                                         Perc_Zootecnico_Testata As Decimal,
                                                    objParametri_Server As AgronicaCoreParametri,
                                                    ByRef rptPua As RPT_PUA,
                                                    ByRef str_FerCod_Org As String, ByRef HashFerCodDigestati As Hashtable,
                                                    ByRef HashFerCodLetami As Hashtable, ByRef HashFerCodLiquami As Hashtable)

        Try

            Dim objEff As New AgronicaCorePUA_DAL.Pua_Effluente_R
            Dim DtEffluenti As DataTable = objEff.Leggi(regolamento_cod, pua_cod, 0, " azoto_qta >0 " & If(pua_cod = 0, " AND 1=0", ""), "", objParametri_Server)

            Dim objEffluentiOutput As AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_output

            If regolamento_cod <> 0 Then
                Dim objPC As New AgronicaCoreWebService.PianoConcimazione_WS
                Dim objEffluentiInput As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluenti_input
                objEffluentiInput.Regolamento_Cod = regolamento_cod
                objEffluentiInput.Includi_Efficienza_Rif = True
                objEffluentiOutput = objPC.Effluenti(objEffluentiInput, objParametri_Server, objParametri_Super_Server)
            End If

            Dim Perc_Zootecnico As Decimal = 100
            If Not objEffluentiOutput Is Nothing Then
                For Each eff As PUA_Effluente In objEffluentiOutput.ListaEffluenti
                    Perc_Zootecnico = 100
                    If eff.MatricePrevalente = 1 AndAlso Not DtEffluenti Is Nothing AndAlso DtEffluenti.Select("eff_cod=" & eff.Eff_Cod).Length > 0 Then
                        Perc_Zootecnico = DtEffluenti.Select("eff_cod=" & eff.Eff_Cod)(0).Item("perc_zootecnico")
                    End If

                    If eff.SpecieAllevamento = 1 Then
                        str_FerCod_Org &= eff.Fer_Cod & ","
                    End If
                    If eff.MatricePrevalente = 1 Then
                        HashFerCodDigestati.Add(eff.Fer_Cod, Perc_Zootecnico)
                    End If

                    Select Case eff.Id_tp_fer
                        Case enum_PUA_TipoFertilizzante.Ammendante
                            HashFerCodLetami.Add(eff.Fer_Cod, Perc_Zootecnico)
                        Case enum_PUA_TipoFertilizzante.Liquame
                            HashFerCodLiquami.Add(eff.Fer_Cod, Perc_Zootecnico)
                    End Select
                Next
            End If

            If str_FerCod_Org <> "" Then
                str_FerCod_Org = "(" & Left(str_FerCod_Org, str_FerCod_Org.Length - 1) & ")"
            End If


            DtEffluenti.Columns.Add(New DataColumn("eff_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("tipo_eff_cod", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("tipo_eff_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("udm_cod", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("udm_sim", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("flag_tipo_allevamento", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("flag_matrice_prevalente", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("flag_provenienzaesterna_des", GetType(String)))
            DtEffluenti.Columns.Add(New DataColumn("FertilizzanteUsato", GetType(Decimal)))
            DtEffluenti.Columns.Add(New DataColumn("EffRif", GetType(Decimal)))
            DtEffluenti.Columns.Add(New DataColumn("EffCons", GetType(Decimal)))
            DtEffluenti.Columns.Add(New DataColumn("fer_cod", GetType(Integer)))
            DtEffluenti.Columns.Add(New DataColumn("fer_des", GetType(String)))

            Dim Eff_Cod As Integer
            Dim Eff_Des As String
            Dim Fer_Cod As Integer
            Dim Fer_Des As String
            Dim Tipo_Eff_Cod As Integer
            Dim Tipo_Eff_Des As String
            Dim Udm_Cod As Integer
            Dim Udm_Sim As String
            Dim Flag_Tipo_Allevamento As Integer
            Dim Flag_Matrice_Prevalente As Integer
            Dim Flag_ProvenienzaEsterna_Des As String
            Dim FertilizzanteUsato As Decimal
            Dim EffPesataConNDistribuito As Decimal
            Dim NDistribuito As Decimal
            Dim EffRif As Decimal
            Dim EffCons As Decimal
            Dim CaricoTot As Decimal
            Dim RiempimentoTot As Decimal

            Dim dr As DS_PUA_Effluenti.DT_Pua_EffluentiRow

            Dim drD As DS_PUA_Effluenti.DT_Pua_Effluenti_DistribuitiRow

            Dim HashEffCod As New Hashtable

            If Not DtEffluenti Is Nothing Then

                For i = 0 To DtEffluenti.Rows.Count - 1

                    Eff_Cod = DtEffluenti.Rows(i).Item("eff_cod")
                    Eff_Des = ""
                    Tipo_Eff_Cod = 0
                    Tipo_Eff_Des = ""
                    Udm_Cod = 0
                    Udm_Sim = ""
                    Flag_Tipo_Allevamento = 0
                    Flag_Matrice_Prevalente = 0
                    Flag_ProvenienzaEsterna_Des = "No"
                    FertilizzanteUsato = 0
                    Fer_Cod = 0
                    Fer_Des = ""
                    EffRif = 0
                    EffCons = 0
                    EffPesataConNDistribuito = 0
                    NDistribuito = 0

                    If Not IsNothing(objEffluentiOutput) Then
                        Dim Effluente As New AgronicaCorePianoConcimazioneBIZ.PUA_Effluente
                        Effluente = objEffluentiOutput.ListaEffluenti.Where(Function(x) x.Eff_Cod = Eff_Cod)(0)
                        If Not Effluente Is Nothing Then
                            Eff_Des = Effluente.Eff_Des
                            Tipo_Eff_Cod = Effluente.Tipo_Eff_Cod
                            Tipo_Eff_Des = Effluente.Tipo_Eff_Des
                            Udm_Cod = Effluente.Udm_Cod
                            Udm_Sim = Effluente.Udm_Sim
                            Flag_Tipo_Allevamento = Effluente.SpecieAllevamento
                            Flag_Matrice_Prevalente = Effluente.MatricePrevalente
                            Fer_Cod = Effluente.Fer_Cod
                            Fer_Des = Effluente.Fer_Des
                            EffRif = Effluente.Efficienza_Rif
                        End If
                    End If

                    ' perc_zootecnico
                    ' se non presenti sull'effluente (versione nuova)
                    ' prendo quelli della testata pua
                    If Not IsNumeric(DtEffluenti.Rows(i).Item("perc_zootecnico")) Then
                        DtEffluenti.Rows(i).Item("perc_zootecnico") = 0
                    End If
                    If DtEffluenti.Rows(i).Item("perc_zootecnico") = 0 And Flag_Matrice_Prevalente = 1 Then
                        DtEffluenti.Rows(i).Item("perc_zootecnico") = Perc_Zootecnico_Testata
                    End If

                    If IsDBNull(DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna")) Then
                        DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna") = 0
                    End If

                    Flag_ProvenienzaEsterna_Des = If(DtEffluenti.Rows(i).Item("Flag_ProvenienzaEsterna") = 1, "Sì", "No")

                    DtEffluenti.Rows(i).Item("Eff_Des") = Eff_Des
                    DtEffluenti.Rows(i).Item("tipo_eff_cod") = Tipo_Eff_Cod
                    DtEffluenti.Rows(i).Item("tipo_eff_des") = Tipo_Eff_Des
                    DtEffluenti.Rows(i).Item("Udm_Cod") = Udm_Cod
                    DtEffluenti.Rows(i).Item("Udm_Sim") = Udm_Sim
                    DtEffluenti.Rows(i).Item("flag_tipo_allevamento") = Flag_Tipo_Allevamento
                    DtEffluenti.Rows(i).Item("flag_matrice_prevalente") = Flag_Matrice_Prevalente
                    DtEffluenti.Rows(i).Item("flag_provenienzaEsterna_des") = Flag_ProvenienzaEsterna_Des
                    DtEffluenti.Rows(i).Item("fer_cod") = Fer_Cod
                    DtEffluenti.Rows(i).Item("fer_des") = Fer_Des
                    DtEffluenti.Rows(i).Item("EffRif") = EffRif

                    Dim strPeriodiDivieto As String = ""
                    Dim objEffDiv As New AgronicaCorePUA_DAL.Pua_Effluente_PeriodoDivieto_R
                    Dim DtEffluentiDiv As DataTable = objEffDiv.Leggi_DaEffluente(regolamento_cod, pua_cod, 0, 0, Eff_Cod, "", "", objParametri_Server)

                    If Not DtEffluentiDiv Is Nothing AndAlso DtEffluentiDiv.Rows.Count > 0 Then

                        Dim strInizioDivieto As String = ""
                        Dim strFineDivieto As String = ""

                        For Each DrEffDiv As DataRow In DtEffluentiDiv.Rows

                            If Not IsDBNull(DrEffDiv.Item("Data_Divieto_DA")) AndAlso CDate(DrEffDiv.Item("Data_Divieto_DA")) <> AGRODATAINIZIO Then
                                strInizioDivieto = CDate(DrEffDiv.Item("Data_Divieto_DA")).ToShortDateString
                            End If
                            If Not IsDBNull(DrEffDiv.Item("Data_Divieto_A")) AndAlso CDate(DrEffDiv.Item("Data_Divieto_A")) <> AGRODATAFINE Then
                                strFineDivieto = CDate(DrEffDiv.Item("Data_Divieto_A")).ToShortDateString
                            End If

                            If strPeriodiDivieto <> "" Then
                                strPeriodiDivieto &= vbCrLf
                            End If

                            If strInizioDivieto <> "" And strFineDivieto <> "" Then
                                strPeriodiDivieto &= strInizioDivieto & " - " & strFineDivieto
                            ElseIf strInizioDivieto <> "" And strFineDivieto = "" Then
                                strPeriodiDivieto &= strInizioDivieto
                            ElseIf strInizioDivieto = "" And strFineDivieto <> "" Then
                                strPeriodiDivieto &= strFineDivieto
                            End If

                        Next

                    End If

                    'Dim strInizioDivieto As String = ""
                    'Dim strFineDivieto As String = ""
                    'If Not IsDBNull(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")) Then
                    '    If CDate(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")) = AGRODATAINIZIO Or CDate(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")) = AGRODATAFINE Then
                    '        DtEffluenti.Rows(i).Item("Data_Inizio_Divieto") = DBNull.Value
                    '    Else
                    '        strInizioDivieto = CDate(DtEffluenti.Rows(i).Item("Data_Inizio_Divieto")).ToShortDateString
                    '    End If
                    'End If
                    'If Not IsDBNull(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")) Then
                    '    If CDate(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")) = AGRODATAINIZIO Or CDate(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")) = AGRODATAFINE Then
                    '        DtEffluenti.Rows(i).Item("Data_Fine_Divieto") = DBNull.Value
                    '    Else
                    '        strFineDivieto = CDate(DtEffluenti.Rows(i).Item("Data_Fine_Divieto")).ToShortDateString
                    '    End If
                    'End If

                    dr = DsEffluenti.DT_Pua_Effluenti.NewDT_Pua_EffluentiRow

                    dr.flag_provenienza_esterna_des = DtEffluenti.Rows(i).Item("flag_provenienzaEsterna_des")
                    dr.eff_des = DtEffluenti.Rows(i).Item("Eff_Des")
                    dr.perc_zootecnico = DtEffluenti.Rows(i).Item("perc_zootecnico")
                    dr.udm_sim = DtEffluenti.Rows(i).Item("Udm_Sim")
                    dr.carico = DtEffluenti.Rows(i).Item("carico")
                    dr.capacita_stoccaggio = DtEffluenti.Rows(i).Item("capacita_stoccaggio")
                    dr.giorni_stoccaggio = DtEffluenti.Rows(i).Item("giorni_stoccaggio")
                    dr.riempimento = DtEffluenti.Rows(i).Item("riempimento")
                    dr.azoto_qta = DtEffluenti.Rows(i).Item("azoto_qta")
                    dr.azoto_titoli = DtEffluenti.Rows(i).Item("azoto_titoli")

                    dr.periodo_divieto = strPeriodiDivieto

                    'If strInizioDivieto <> "" And strFineDivieto <> "" Then
                    '    dr.periodo_divieto = strInizioDivieto & " - " & strFineDivieto
                    'ElseIf strInizioDivieto <> "" And strFineDivieto = "" Then
                    '    dr.periodo_divieto = strInizioDivieto
                    'ElseIf strInizioDivieto = "" And strFineDivieto <> "" Then
                    '    dr.periodo_divieto = strFineDivieto
                    'End If

                    If Not IsDBNull(DtEffluenti.Rows(i).Item("EffRif")) AndAlso CDec(DtEffluenti.Rows(i).Item("EffRif")) > 0 Then
                        dr.eff_rif = DtEffluenti.Rows(i).Item("EffRif")
                    Else
                        dr.eff_rif = "n.d."
                    End If

                    DsEffluenti.DT_Pua_Effluenti.AddDT_Pua_EffluentiRow(dr)

                    If Not HashEffCod.ContainsKey(Eff_Cod) Then
                        HashEffCod.Add(Eff_Cod, Fer_Cod)
                    End If

                Next


                If Not HashEffCod Is Nothing AndAlso DtEffluenti.Rows.Count > 0 Then

                    For Each Eff_Cod In HashEffCod.Keys

                        Dim DrEffCod() As DataRow = DtEffluenti.Select("eff_cod=" & Eff_Cod)

                        Eff_Des = ""
                        Udm_Cod = 0
                        Udm_Sim = ""
                        Fer_Cod = 0
                        Fer_Des = ""
                        CaricoTot = 0
                        RiempimentoTot = 0
                        EffRif = 0
                        NDistribuito = 0

                        If Not DrEffCod Is Nothing AndAlso DrEffCod.Length > 0 Then
                            Eff_Des = DrEffCod(0).Item("eff_des")
                            Udm_Cod = DrEffCod(0).Item("Udm_Cod")
                            Udm_Sim = DrEffCod(0).Item("Udm_Sim")
                            Fer_Cod = DrEffCod(0).Item("Fer_Cod")
                            Fer_Des = DrEffCod(0).Item("Fer_Des")
                            EffRif = DrEffCod(0).Item("EffRif")
                            For Each drtmp In DrEffCod
                                CaricoTot += CDec(drtmp.Item("carico"))
                                RiempimentoTot += CDec(drtmp.Item("riempimento"))
                            Next
                        End If

                        EffRif = EffRif * 100

                        Dim objRecuperaDati As New AgronicaCorePUA_DAL.PUA_Apporti_DAL
                        FertilizzanteUsato = objRecuperaDati.FertilizzanteDistribuitoIntervallo(objParametri_Server, piva, DataInizio, DataFine, Fer_Cod)

                        If FertilizzanteUsato > 0 Then

                            NDistribuito = objRecuperaDati.NDistribuitoIntervallo(objParametri_Server, piva, DataInizio, DataFine, Fer_Cod)

                            EffPesataConNDistribuito = objRecuperaDati.EfficienzaPesataConNDistribuito(objParametri_Server, piva, DataInizio, DataFine, Fer_Cod)
                            EffCons = EffPesataConNDistribuito / NDistribuito
                            'EffCons = EffCons * 100

                            'converto l'unita di misura (restitutita in kg io l)
                            Select Case Udm_Cod
                                Case enum_UnitaMisura.Quintali
                                    FertilizzanteUsato = FertilizzanteUsato / 100
                                Case enum_UnitaMisura.Metri_Cubi
                                    FertilizzanteUsato = FertilizzanteUsato / 1000
                            End Select
                        End If

                        drD = DsEffluentiDistribuiti.DT_Pua_Effluenti_Distribuiti.NewDT_Pua_Effluenti_DistribuitiRow

                        drD.eff_des = Eff_Des
                        drD.udm_sim = Udm_Sim
                        drD.carico = Agro_Math.ArrotondaVal_2(CaricoTot)
                        drD.riempimento = Agro_Math.ArrotondaVal_2(RiempimentoTot)
                        drD.fertilizzante_usato = Agro_Math.ArrotondaVal_2(FertilizzanteUsato)
                        drD.azoto_qta = NDistribuito

                        If EffRif > 0 Then
                            drD.eff_rif = Agro_Math.ArrotondaVal_2(EffRif)
                        Else
                            drD.eff_rif = "n.d."
                        End If

                        drD.eff_cons = Agro_Math.ArrotondaVal_2(EffCons)

                        DsEffluentiDistribuiti.DT_Pua_Effluenti_Distribuiti.AddDT_Pua_Effluenti_DistribuitiRow(drD)

                    Next

                End If

            End If

            rptPua.OpenSubreport("RPT_PUA_Effluenti.rpt").SetDataSource(DsEffluenti)
            rptPua.OpenSubreport("RPT_PUA_EffluentiDistribuiti.rpt").SetDataSource(DsEffluentiDistribuiti)

        Catch ex As Exception

            Dim strErr As String = ex.Message

        End Try


    End Sub

    Private Sub publicCarica_DsPianoDistribuzione(ByRef DsPianoDistribuzione As DS_PUA_PianoDistribuzione, ByRef DsPianoDistribuzioneBilancio As DS_PUA_PianoDistribuzione,
                                                 ByRef listPiano As List(Of PUA_Appezzamento),
                                                 piva As String, sa_cod As Integer, pua_cod As Integer, regolamento_cod As Integer, tipo As Integer,
                                                  DataInizio As Date, DataFine As Date,
                                                     str_FerCod_Org As String, HashFerCodDigestati As Hashtable,
                                                     HashFerCodLetami As Hashtable, HashFerCodLiquami As Hashtable, objParametri_Server As AgronicaCoreParametri,
                                                ByRef rptPua As RPT_PUA)

        Try


            Dim objImpiantiR As New Reg_Impianti_Read
            Dim dt As DataTable = objImpiantiR.Leggi_Dati_Impianti_per_PUA_NEW(piva, sa_cod, pua_cod, regolamento_cod,
                                                                                    DataInizio, DataFine,
                                                                                       1,
                                                                                       str_FerCod_Org, HashFerCodDigestati,
                                                                                       enum_PUA_Modalita.Modalita_Verifica, HashFerCodLetami, HashFerCodLiquami,
                                                                                       "", "App_Nome", objParametri_Server)

            Dim dtTN As DataTable = objImpiantiR.Leggi_Dati_Impianti_per_PUA_NEW(piva, sa_cod, pua_cod, regolamento_cod,
                                                                                    DataInizio, DataFine,
                                                                                       2,
                                                                                       "", Nothing,
                                                                                       enum_PUA_Modalita.Modalita_Verifica, Nothing, Nothing,
                                                                                       "", "App_Nome", objParametri_Server)


            Dim dictStatoImpianto As New Dictionary(Of Integer, List(Of Fase))

            Dim listaPrecessioni As List(Of Precessione) = GetListaPrecessioniDes(regolamento_cod, tipo)
            Dim listaUbicazioni As List(Of Ubicazione) = GetListaUbicazioniDes(regolamento_cod)
            Dim listaTipiAcqua As List(Of TipoAcqua) = GetListaTipiAcquaDes(regolamento_cod)

            'estraggo i singoli veg_cod
            Dim listaReseMas As List(Of PianoConcimazione_LimiteMAS_output)
            Dim listaFinalita As List(Of Finalita)
            Dim listaCoefficienteB As List(Of PUA_CoefficienteB)
            Dim HashVegCod As New Hashtable
            Dim veg_cod_elenco As String
            If Not dt Is Nothing Then
                For Each row In dt.Rows
                    If Not HashVegCod.ContainsKey(row.Item("veg_cod")) Then
                        HashVegCod.Add(row.Item("veg_cod"), "")
                        veg_cod_elenco &= row.Item("veg_cod") & ","
                    End If
                Next
                If veg_cod_elenco <> "" Then
                    listaReseMas = GetListaReseMas(regolamento_cod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
                    listaFinalita = GetListaFinalitaRer(regolamento_cod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
                    listaCoefficienteB = GetListaCoefficienteB(regolamento_cod, Left(veg_cod_elenco, veg_cod_elenco.Length - 1))
                End If
            End If


            Dim dtZvnServer As DataTable = Nothing
            Dim dtZvnMetaschema As DataTable = Nothing
            LeggiZoneVulnerabili(regolamento_cod, DataInizio, DataFine, dtZvnServer, dtZvnMetaschema, objParametri_Server)



            Dim count As Integer = 1

            Dim dict As New Dictionary(Of String, List(Of String))

            Dim SeparatoreChiave As Char = "_"
            Dim VulnerabileStr As String = " <b>(V)</b>"

            ' Giulia: 9/1/2020: Nella pagina web, passando da web service, l'N_Fabbisogno, viene inizializzato e mostrato a 0,
            ' se non l'ho ancora ribaltato sull'impianto quello del db è -1, ma per i conteggi devo far finta che sia 0

            If Not dt Is Nothing Then

                For Each row In dt.Rows

                    Dim chiaveDistinta As String = row.Item("Piva") & SeparatoreChiave &
                                                   row.Item("Sa_Cod") & SeparatoreChiave &
                                                   row.Item("Appezza") & SeparatoreChiave &
                                                   row.Item("Id_Reg") & SeparatoreChiave &
                                                   row.Item("Progetto_Cod")
                    'Dim chiavePart As String = row.Item("PROV") & ":" & row.Item("COM") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    Dim chiavePart As String = ""
                    If row.Item("PROV") <> "" Then
                        chiavePart = row.Item("provincia") & ":_" & row.Item("comune") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    End If
                    Dim objPiano As PUA_Appezzamento

                    If Not dict.ContainsKey(chiaveDistinta) Then

                        Dim nFabbDatabaseStr As String = row.Item("N_Fabbisogno")
                        Dim nFabbDatabase As Decimal = -1
                        If IsNumeric(nFabbDatabaseStr) Then
                            nFabbDatabase = CDec(nFabbDatabaseStr)
                        End If

                        Dim nFabbDatabaseOrgStr As String = row.Item("N_Fabbisogno_Organico")
                        Dim nFabbDatabaseOrg As Decimal = -1
                        If IsNumeric(nFabbDatabaseOrgStr) Then
                            nFabbDatabaseOrg = CDec(nFabbDatabaseOrgStr)
                        End If

                        'è la prima volta dell'impianto, quindi devo creare tutto
                        objPiano = New PUA_Appezzamento With {
                            .Chiave = chiaveDistinta,
                            .Piva = row.Item("Piva"),
                            .Sa_Cod = row.Item("Sa_Cod"),
                            .Campo_Cod = row.Item("Campo_Cod"),
                            .appezza = row.Item("Appezza"),
                            .id_reg = row.Item("Id_Reg"),
                            .Progetto_Cod = row.Item("Progetto_Cod"),
                            .Veg_Cod = row.Item("Veg_Cod"),
                            .Grfi_Cod = row.Item("Grfi_Cod"),
                            .Grfi_Cod_Concimazione = row.Item("Grfi_Cod_Concimazione"),
                            .B_Perc = 0,
                            .Appezzamento = row.Item("App_Nome") & " - " & row.Item("veg_des"),
                            .Superficie = row.Item("Sup_Imp"),
                            .ValiditaInizio = row.Item("Validita_Inizio_Impianto"),
                            .ValiditaFine = row.Item("Validita_Fine_Impianto"),
                            .DurataColtura = row.Item("Validita_Inizio_Impianto") & vbCrLf & row.Item("Validita_Fine_Impianto"),
                            .StatoImpiantoCod = row.Item("Stato_Impianto"),
                            .Ciclo = row.Item("Ciclo_Cod"),
                            .CicloDes = row.Item("Ciclo_Des"),
                            .Resa = row.Item("Resa"),
                            .Id_AnagrafeVincoli = row.Item("Id_AnaVincoli"),
                            .Pua_Cod = row.Item("Pua_Cod"),
                            .Regolamento_Cod = row.Item("PUA_Regolamento_Cod"),
                            .Data_Pua = row.Item("DataPua"),
                            .AnalisiTestataCod = row.Item("Analisi_Testata_Cod"),
                            .AnalisiTestataDes = row.Item("Analisi_Testata_Des"),
                            .Sabbia = row.Item("sabbia"),
                            .Argilla = row.Item("argilla"),
                            .So = row.Item("So"),
                            .PrecessioneCod = row.Item("Veg_Cod_Prec"),
                            .UbicazioneCod = row.Item("Ubicazione_Cod"),
                            .TipoAcquaCod = row.Item("TipoAcqua_Cod"),
                            .N_FertilizzazioniPrecedenti = row.Item("N_FertilizzazioniPrecedenti"),
                            .N_Fabbisogno_Database = nFabbDatabase,
                            .N_Fabbisogno = If(nFabbDatabaseOrg >= 0, nFabbDatabaseOrg, 0),
                            .N_FabbisognoComplessivo = If(nFabbDatabase >= 0, nFabbDatabase, 0) * row.Item("Sup_Imp"),
                            .N_FabbisognoSoddisfatto = CDec(row.Item("N_FabbisognoSoddisfatto")),
                            .N_Zootecnico = CDec(row.Item("N_FabbisognoSoddisfattoOrganico")) + CDec(row.Item("N_SoddisfattoDigestato")),
                            .LimiteMas = 0,
                            .N_TotaleSoddisfatto = CDec(row.Item("N_TotaleSoddisfatto")),
                            .N_Zootecnico_Letame = CDec(row.Item("N_Zootecnico_Letame")),
                            .N_Zootecnico_Liquame = CDec(row.Item("N_Zootecnico_Liquame"))
                        }

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            objPiano.ZVN = True
                        End If


                        objPiano.ParticelleVincoli.Add(objParticella)


                        '------ Catasto
                        objPiano.Catasto = chiavePart '& If(objParticella.ZVN = True, VulnerabileStr, "")

                        '------ StatoImpiantoDes
                        objPiano.StatoImpiantoDes = GetStatoImpiantoDes(dictStatoImpianto,
                                                                        regolamento_cod, objPiano.Veg_Cod,
                                                                        objPiano.StatoImpiantoCod)
                        '------ PrecessioneDes
                        objPiano.PrecessioneDes = GetPrecessioneDes(listaPrecessioni, objPiano.PrecessioneCod)

                        '------ UbicazioneDes
                        objPiano.UbicazioneDes = GetUbicazioneDes(listaUbicazioni, objPiano.UbicazioneCod)

                        '------ TipoAcquaDes
                        objPiano.TipoAcquaDes = GetTipoAcquaDes(listaTipiAcqua, objPiano.TipoAcquaCod)

                        If objPiano.Grfi_Cod_Concimazione > 0 Then
                            objPiano.Grfi_Des_Concimazione = GetFinalitaRerDes(listaFinalita, objPiano.Grfi_Cod_Concimazione)
                            objPiano.B_Perc = GetCoefficienteB(listaCoefficienteB, objPiano.Veg_Cod, objPiano.Grfi_Cod_Concimazione)
                        End If

                        listPiano.Add(objPiano)

                        dict.Add(chiaveDistinta, New List(Of String) From {chiavePart})


                    Else

                        'devo solo prendere la parte della particella
                        Dim idx As Integer = listPiano.FindIndex(Function(x) x.Chiave = chiaveDistinta)

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            listPiano(idx).ZVN = True
                        End If

                        listPiano(idx).Catasto = listPiano(idx).Catasto & "|" & chiavePart '& If(objParticella.ZVN = True, VulnerabileStr, "")
                        listPiano(idx).ParticelleVincoli.Add(objParticella)

                    End If

                    count += 1



                Next

            End If


            'aggiungo i terreni nudi
            If Not dtTN Is Nothing Then

                For Each row In dtTN.Rows

                    Dim chiaveDistinta As String = row.Item("Piva") & SeparatoreChiave &
                                                   row.Item("Sa_Cod") & SeparatoreChiave &
                                                   row.Item("Appezza") & SeparatoreChiave &
                                                   row.Item("Id_Reg") & SeparatoreChiave &
                                                   row.Item("Progetto_Cod")
                    'Dim chiavePart As String = row.Item("PROV") & ":" & row.Item("COM") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    Dim chiavePart As String = ""
                    If row.Item("PROV") <> "" Then
                        chiavePart = row.Item("provincia") & ":_" & row.Item("comune") & ":_" & row.Item("SEZIONE") & ":_" & row.Item("FOGLIO") & ":_" & row.Item("NUMERO") & ":_" & row.Item("SUBALTERNO")
                    End If
                    Dim objPiano As PUA_Appezzamento

                    If Not dict.ContainsKey(chiaveDistinta) Then

                        Dim nFabbDatabaseStr As String = row.Item("N_Fabbisogno")
                        Dim nFabbDatabase As Decimal = -1
                        If IsNumeric(nFabbDatabaseStr) Then
                            nFabbDatabase = CDec(nFabbDatabaseStr)
                        End If

                        Dim nFabbDatabaseOrgStr As String = row.Item("N_Fabbisogno_Organico")
                        Dim nFabbDatabaseOrg As Decimal = -1
                        If IsNumeric(nFabbDatabaseOrgStr) Then
                            nFabbDatabaseOrg = CDec(nFabbDatabaseOrgStr)
                        End If

                        'è la prima volta dell'impianto, quindi devo creare tutto
                        objPiano = New PUA_Appezzamento With {
                            .Chiave = chiaveDistinta,
                            .Piva = row.Item("Piva"),
                            .Sa_Cod = row.Item("Sa_Cod"),
                            .Campo_Cod = row.Item("Campo_Cod"),
                            .appezza = row.Item("Appezza"),
                            .id_reg = row.Item("Id_Reg"),
                            .Progetto_Cod = row.Item("Progetto_Cod"),
                            .Veg_Cod = row.Item("Veg_Cod"),
                            .Grfi_Cod = row.Item("Grfi_Cod"),
                            .Grfi_Cod_Concimazione = row.Item("Grfi_Cod_Concimazione"),
                            .B_Perc = 0,
                            .Appezzamento = row.Item("App_Nome") & " - " & row.Item("veg_des"),
                            .Superficie = row.Item("Sup_Imp"),
                            .ValiditaInizio = row.Item("Validita_Inizio_Impianto"),
                            .ValiditaFine = row.Item("Validita_Fine_Impianto"),
                            .DurataColtura = row.Item("Validita_Inizio_Impianto") & vbCrLf & row.Item("Validita_Fine_Impianto"),
                            .StatoImpiantoCod = row.Item("Stato_Impianto"),
                            .Ciclo = row.Item("Ciclo_Cod"),
                            .CicloDes = row.Item("Ciclo_Des"),
                            .Resa = row.Item("Resa"),
                            .Id_AnagrafeVincoli = row.Item("Id_AnaVincoli"),
                            .Pua_Cod = row.Item("Pua_Cod"),
                            .Regolamento_Cod = row.Item("PUA_Regolamento_Cod"),
                            .Data_Pua = row.Item("DataPua"),
                            .AnalisiTestataCod = row.Item("Analisi_Testata_Cod"),
                            .AnalisiTestataDes = row.Item("Analisi_Testata_Des"),
                            .Sabbia = row.Item("sabbia"),
                            .Argilla = row.Item("argilla"),
                            .So = row.Item("So"),
                            .PrecessioneCod = row.Item("Veg_Cod_Prec"),
                            .UbicazioneCod = row.Item("Ubicazione_Cod"),
                            .TipoAcquaCod = row.Item("TipoAcqua_Cod"),
                            .N_FertilizzazioniPrecedenti = row.Item("N_FertilizzazioniPrecedenti"),
                            .N_Fabbisogno_Database = nFabbDatabase,
                            .N_Fabbisogno = If(nFabbDatabaseOrg >= 0, nFabbDatabaseOrg, 0),
                            .N_FabbisognoComplessivo = If(nFabbDatabase >= 0, nFabbDatabase, 0) * row.Item("Sup_Imp"),
                            .N_FabbisognoSoddisfatto = 0,
                            .N_Zootecnico = 0,
                            .LimiteMas = 0
                        }



                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            objPiano.ZVN = True
                        End If

                        objPiano.ParticelleVincoli.Add(objParticella)


                        '------ Catasto
                        objPiano.Catasto = chiavePart '& If(objParticella.ZVN = True, VulnerabileStr, "")

                        '------ StatoImpiantoDes
                        objPiano.StatoImpiantoDes = GetStatoImpiantoDes(dictStatoImpianto,
                                                                        regolamento_cod, objPiano.Veg_Cod,
                                                                        objPiano.StatoImpiantoCod)

                        '------ PrecessioneDes
                        objPiano.PrecessioneDes = GetPrecessioneDes(listaPrecessioni, objPiano.PrecessioneCod)

                        '------ UbicazioneDes
                        objPiano.UbicazioneDes = GetUbicazioneDes(listaUbicazioni, objPiano.UbicazioneCod)

                        '------ TipoAcquaDes
                        objPiano.TipoAcquaDes = GetTipoAcquaDes(listaTipiAcqua, objPiano.TipoAcquaCod)


                        listPiano.Add(objPiano)

                        dict.Add(chiaveDistinta, New List(Of String) From {chiavePart})

                    Else

                        'devo solo prendere la parte della particella
                        Dim idx As Integer = listPiano.FindIndex(Function(x) x.Chiave = chiaveDistinta)

                        Dim objParticella As New PUA_ParticellaVincoloAgronomico With {
                            .Part_PROV = row.Item("PROV"),
                            .Part_COM = row.Item("COM"),
                            .Part_SEZIONE = row.Item("SEZIONE"),
                            .Part_FOGLIO = row.Item("FOGLIO"),
                            .Part_NUMERO = row.Item("NUMERO"),
                            .Part_SUBALTERNO = row.Item("SUBALTERNO")
                        }

                        '------ Zone Vulnerabili (ZVN)
                        Dim flagVulnerabile As Boolean = IsZonaVulnerabile(objParticella.Part_PROV, objParticella.Part_COM,
                                                                           objParticella.Part_SEZIONE, objParticella.Part_FOGLIO,
                                                                           objParticella.Part_NUMERO, objParticella.Part_SUBALTERNO,
                                                                           dtZvnServer, dtZvnMetaschema)

                        objParticella.ZVN = flagVulnerabile

                        'C'è almeno una particella vulnerabile, quindi tutto l'appezzamento viene considerato vulnerabile
                        If objParticella.ZVN = True Then
                            listPiano(idx).ZVN = True
                        End If

                        listPiano(idx).Catasto = listPiano(idx).Catasto & "|" & chiavePart '& If(objParticella.ZVN = True, VulnerabileStr, "")
                        listPiano(idx).ParticelleVincoli.Add(objParticella)

                    End If

                    count += 1
                Next

            End If


            'riempio i DATASET
            Dim dr As DS_PUA_PianoDistribuzione.DT_Pua_PianoDistribuzioneRow
            Dim drB As DS_PUA_PianoDistribuzione.DT_Pua_PianoDistribuzioneRow
            Dim drBTotali As DS_PUA_PianoDistribuzione.DT_Pua_Bilancio_TotaliRow

            Dim NUtile_Tot As Decimal = 0
            Dim NTotale_Tot As Decimal = 0
            Dim Ind_Eff_Tot As Decimal = 0

            Dim Ha_ZVN As Decimal = 0
            Dim Ha_Ord As Decimal = 0
            Dim Ha_Tot As Decimal = 0

            Dim Ha_ZVN_SecondoRaccolto As Decimal = 0
            Dim Ha_Ord_SecondoRaccolto As Decimal = 0
            Dim Ha_Tot_SecondoRaccolto As Decimal = 0

            Dim Ha_ZVN_Fertilizzati As Decimal = 0
            Dim Ha_Ord_Fertilizzati As Decimal = 0
            Dim Ha_Tot_Fertilizzati As Decimal = 0
            Dim Ha_ZVN_Fertilizzati_Zoo As Decimal = 0
            Dim Ha_Ord_Fertilizzati_Zoo As Decimal = 0
            Dim Ha_Tot_Fertilizzati_Zoo As Decimal = 0

            Dim NZoo_ZVN As Decimal = 0
            Dim NZoo_Ord As Decimal = 0
            Dim NZoo_Tot As Decimal = 0
            Dim Media_NZoo_ZVN As Decimal = 0
            Dim Media_NZoo_Ord As Decimal = 0
            Dim Media_NZoo_Tot As Decimal = 0

            Dim NZoo_Let_ZVN As Decimal = 0
            Dim NZoo_Let_Ord As Decimal = 0
            Dim NZoo_Let_Tot As Decimal = 0
            Dim Media_NZoo_Let_ZVN As Decimal = 0
            Dim Media_NZoo_Let_Ord As Decimal = 0
            Dim Media_NZoo_Let_Tot As Decimal = 0

            Dim NZoo_Liq_ZVN As Decimal = 0
            Dim NZoo_Liq_Ord As Decimal = 0
            Dim NZoo_Liq_Tot As Decimal = 0
            Dim Media_NZoo_Liq_ZVN As Decimal = 0
            Dim Media_NZoo_Liq_Ord As Decimal = 0
            Dim Media_NZoo_Liq_Tot As Decimal = 0

            For Each item As PUA_Appezzamento In listPiano

                dr = DsPianoDistribuzione.DT_Pua_PianoDistribuzione.NewDT_Pua_PianoDistribuzioneRow
                drB = DsPianoDistribuzioneBilancio.DT_Pua_PianoDistribuzione.NewDT_Pua_PianoDistribuzioneRow

                dr.Appezzamento = item.Appezzamento
                drB.Appezzamento = item.Appezzamento

                dr.Catasto = item.Catasto
                drB.Catasto = item.Catasto

                If item.ZVN = True Then
                    dr.Zvn = "V"
                    drB.Zvn = "V"
                Else
                    dr.Zvn = ""
                    drB.Zvn = ""
                End If

                dr.DurataColtura = item.DurataColtura

                dr.Superficie = item.Superficie
                drB.Superficie = item.Superficie

                If item.Ciclo = 1 Then
                    dr.CicloDes = "S"
                Else
                    dr.CicloDes = "P"
                End If

                dr.Grfi_Des_Concimazione = item.Grfi_Des_Concimazione

                dr.B_Perc = item.B_Perc

                Select Case item.StatoImpiantoCod
                    Case enum_Stato_Impianto.Impianto_Produzione, enum_Stato_Impianto.Anno_Impianto
                        dr.StatoImpiantoDes = "Prod."
                    Case Else
                        dr.StatoImpiantoDes = "All."
                End Select

                dr.Resa = item.Resa

                dr.PrecessioneDes = item.PrecessioneDes

                dr.N_FertilizzazioniPrecedenti = item.N_FertilizzazioniPrecedenti

                If item.UbicazioneDes = "Pianura, Collina, Montagna" Then
                    dr.UbicazioneDes = "P,C,M"
                End If

                If item.TipoAcquaCod = 1 Then
                    dr.TipoAcquaDes = "Sott."
                End If

                dr.AnalisiTestataDes = item.AnalisiTestataDes

                dr.N_Fabbisogno = item.N_Fabbisogno
                drB.N_Fabbisogno = item.N_Fabbisogno


                dr.N_FabbisognoSoddisfatto = item.N_FabbisognoSoddisfatto
                drB.N_FabbisognoSoddisfatto = item.N_FabbisognoSoddisfatto

                drB.N_TotaleSoddisfatto = item.N_TotaleSoddisfatto

                drB.N_BilancioAzotato_Utile = drB.N_FabbisognoSoddisfatto - drB.N_Fabbisogno
                drB.N_BilancioAzotato_Totale = drB.N_TotaleSoddisfatto - drB.N_Fabbisogno

                drB.Assorbimento = item.B_Perc * item.Resa * 10
                drB.Indice_Efficienza_Azotata = 0
                If drB.N_TotaleSoddisfatto <> 0 Then
                    drB.Indice_Efficienza_Azotata = drB.Assorbimento / drB.N_TotaleSoddisfatto * 100
                End If

                dr.N_Zootecnico = item.N_Zootecnico

                DsPianoDistribuzione.DT_Pua_PianoDistribuzione.AddDT_Pua_PianoDistribuzioneRow(dr)
                DsPianoDistribuzioneBilancio.DT_Pua_PianoDistribuzione.AddDT_Pua_PianoDistribuzioneRow(drB)

                'calcolo gli indici di bilancio per le medie aziendali

                If item.ZVN = True Then

                    NZoo_ZVN += item.N_Zootecnico * item.Superficie
                    NZoo_Liq_ZVN += item.N_Zootecnico_Liquame * item.Superficie
                    NZoo_Let_ZVN += item.N_Zootecnico_Letame * item.Superficie

                    'escludo dal calcolo della sup aziendale le colture secondarie
                    If item.Ciclo = 0 Then
                        If item.ValiditaInizio > item.Data_Pua Then
                            Ha_ZVN_SecondoRaccolto += item.Superficie
                        Else
                            Ha_ZVN += item.Superficie
                            If item.N_TotaleSoddisfatto > 0 Then
                                Ha_ZVN_Fertilizzati += item.Superficie
                            End If
                            If item.N_Zootecnico > 0 Then
                                Ha_ZVN_Fertilizzati_Zoo += item.Superficie
                            End If
                        End If
                    Else
                        Ha_ZVN_SecondoRaccolto += item.Superficie
                    End If

                Else
                    NZoo_Ord += item.N_Zootecnico * item.Superficie
                    NZoo_Liq_Ord += item.N_Zootecnico_Liquame * item.Superficie
                    NZoo_Let_Ord += item.N_Zootecnico_Letame * item.Superficie
                    'escludo dal calcolo della sup aziendale le colture secondarie
                    'If item.Ciclo = 0 Then
                    '    Ha_Ord += item.Superficie
                    'End If
                    If item.Ciclo = 0 Then
                        If item.ValiditaInizio > item.Data_Pua Then
                            Ha_Ord_SecondoRaccolto += item.Superficie
                        Else
                            Ha_Ord += item.Superficie
                            If item.N_TotaleSoddisfatto > 0 Then
                                Ha_Ord_Fertilizzati += item.Superficie
                            End If
                            If item.N_Zootecnico > 0 Then
                                Ha_Ord_Fertilizzati_Zoo += item.Superficie
                            End If
                        End If
                    Else
                        Ha_Ord_SecondoRaccolto += item.Superficie
                    End If

                End If

                NZoo_Tot += item.N_Zootecnico * item.Superficie
                NZoo_Let_Tot += item.N_Zootecnico_Letame * item.Superficie
                NZoo_Liq_Tot += item.N_Zootecnico_Liquame * item.Superficie
                'escludo dal calcolo della sup aziendale le colture secondarie
                'If item.Ciclo = 0 Then
                '    Ha_Tot += item.Superficie
                'End If
                If item.Ciclo = 0 Then
                    If item.ValiditaInizio > item.Data_Pua Then
                        Ha_Tot_SecondoRaccolto += item.Superficie
                    Else
                        Ha_Tot += item.Superficie
                        If item.N_TotaleSoddisfatto > 0 Then
                            Ha_Tot_Fertilizzati += item.Superficie
                        End If
                        If item.N_Zootecnico > 0 Then
                            Ha_Tot_Fertilizzati_Zoo += item.Superficie
                        End If
                    End If
                Else
                    Ha_Tot_SecondoRaccolto += item.Superficie
                End If

                NUtile_Tot += drB.N_BilancioAzotato_Utile
                NTotale_Tot += drB.N_BilancioAzotato_Totale
                Ind_Eff_Tot += drB.Indice_Efficienza_Azotata

            Next


            'calcolo gli indici di bilancio per le medie aziendali

            If Ha_ZVN > 0 Then

                If (NZoo_ZVN > 0) Then
                    Media_NZoo_ZVN = NZoo_ZVN / Ha_ZVN
                End If
                If (NZoo_Let_ZVN > 0) Then
                    Media_NZoo_Let_ZVN = NZoo_Let_ZVN / Ha_ZVN
                End If
                If (NZoo_Liq_ZVN > 0) Then
                    Media_NZoo_Liq_ZVN = NZoo_Liq_ZVN / Ha_ZVN
                End If

            End If

            If (Ha_Ord > 0) Then

                If (NZoo_Ord > 0) Then
                    Media_NZoo_Ord = NZoo_Ord / Ha_Ord
                End If
                If (NZoo_Let_Ord > 0) Then
                    Media_NZoo_Let_Ord = NZoo_Let_Ord / Ha_Ord
                End If
                If (NZoo_Liq_Ord > 0) Then
                    Media_NZoo_Liq_Ord = NZoo_Liq_Ord / Ha_Ord
                End If
            End If

            If (Ha_Tot > 0) Then

                If (NZoo_Tot > 0) Then
                    Media_NZoo_Tot = NZoo_Tot / Ha_Tot
                End If
                If (NZoo_Let_Tot > 0) Then
                    Media_NZoo_Let_Tot = NZoo_Let_Tot / Ha_Tot
                End If
                If (NZoo_Liq_Tot > 0) Then
                    Media_NZoo_Liq_Tot = NZoo_Liq_Tot / Ha_Tot
                End If
            End If


            'Imposto i totali (tocca farli con un altro DT)

            drBTotali = DsPianoDistribuzioneBilancio.DT_Pua_Bilancio_Totali.NewDT_Pua_Bilancio_TotaliRow()

            drBTotali.Txt_NUtile = Agro_Math.ArrotondaVal_2(NUtile_Tot)
            drBTotali.Txt_NTotale = Agro_Math.ArrotondaVal_2(NTotale_Tot)
            drBTotali.Txt_IndiceEff = Agro_Math.ArrotondaVal_2(Ind_Eff_Tot)

            drBTotali.Txt_Ha_ZVN = Agro_Math.ArrotondaVal_4(Ha_ZVN)
            drBTotali.Txt_Ha_Ord = Agro_Math.ArrotondaVal_4(Ha_Ord)
            drBTotali.Txt_Ha_Tot = Agro_Math.ArrotondaVal_4(Ha_Tot)

            drBTotali.Txt_Ha_ZVN_SecondoRaccolto = Agro_Math.ArrotondaVal_4(Ha_ZVN_SecondoRaccolto)
            drBTotali.Txt_Ha_Ord_SecondoRaccolto = Agro_Math.ArrotondaVal_4(Ha_Ord_SecondoRaccolto)
            drBTotali.Txt_Ha_Tot_SecondoRaccolto = Agro_Math.ArrotondaVal_4(Ha_Tot_SecondoRaccolto)

            drBTotali.Txt_Ha_ZVN_Fertilizzati = Agro_Math.ArrotondaVal_4(Ha_ZVN_Fertilizzati)
            drBTotali.Txt_Ha_Ord_Fertilizzati = Agro_Math.ArrotondaVal_4(Ha_Ord_Fertilizzati)
            drBTotali.Txt_Ha_Tot_Fertilizzati = Agro_Math.ArrotondaVal_4(Ha_Tot_Fertilizzati)
            drBTotali.Txt_Ha_ZVN_Fertilizzati_Zoo = Agro_Math.ArrotondaVal_4(Ha_ZVN_Fertilizzati_Zoo)
            drBTotali.Txt_Ha_Ord_Fertilizzati_Zoo = Agro_Math.ArrotondaVal_4(Ha_Ord_Fertilizzati_Zoo)
            drBTotali.Txt_Ha_Tot_Fertilizzati_Zoo = Agro_Math.ArrotondaVal_4(Ha_Tot_Fertilizzati_Zoo)

            drBTotali.Txt_NZoo_Tot_ZVN = Agro_Math.ArrotondaVal_2(NZoo_ZVN)
            drBTotali.Txt_NZoo_Tot_Ord = Agro_Math.ArrotondaVal_2(NZoo_Ord)
            drBTotali.Txt_NZoo_Tot_Media = Agro_Math.ArrotondaVal_2(NZoo_Tot)
            drBTotali.Txt_NZoo_Tot_Let_ZVN = Agro_Math.ArrotondaVal_2(NZoo_Let_ZVN)
            drBTotali.Txt_NZoo_Tot_Let_Ord = Agro_Math.ArrotondaVal_2(NZoo_Let_Ord)
            drBTotali.Txt_NZoo_Tot_Let_Media = Agro_Math.ArrotondaVal_2(NZoo_Let_Tot)
            drBTotali.Txt_NZoo_Tot_Liq_ZVN = Agro_Math.ArrotondaVal_2(NZoo_Liq_ZVN)
            drBTotali.Txt_NZoo_Tot_Liq_Ord = Agro_Math.ArrotondaVal_2(NZoo_Liq_Ord)
            drBTotali.Txt_NZoo_Tot_Liq_Media = Agro_Math.ArrotondaVal_2(NZoo_Liq_Tot)

            drBTotali.Txt_NZoo_ZVN = Agro_Math.ArrotondaVal_2(Media_NZoo_ZVN)
            drBTotali.Txt_NZoo_Ord = Agro_Math.ArrotondaVal_2(Media_NZoo_Ord)
            drBTotali.Txt_NZoo_Media = Agro_Math.ArrotondaVal_2(Media_NZoo_Tot)

            drBTotali.Txt_NLetame_ZVN = Agro_Math.ArrotondaVal_2(Media_NZoo_Let_ZVN)
            drBTotali.Txt_NLetame_Ord = Agro_Math.ArrotondaVal_2(Media_NZoo_Let_Ord)
            drBTotali.Txt_NLetame_Tot = Agro_Math.ArrotondaVal_2(Media_NZoo_Let_Tot)

            drBTotali.Txt_NLiquame_ZVN = Agro_Math.ArrotondaVal_2(Media_NZoo_Liq_ZVN)
            drBTotali.Txt_NLiquame_Ord = Agro_Math.ArrotondaVal_2(Media_NZoo_Liq_Ord)
            drBTotali.Txt_NLiquame_Tot = Agro_Math.ArrotondaVal_2(Media_NZoo_Liq_Tot)

            DsPianoDistribuzioneBilancio.DT_Pua_Bilancio_Totali.AddDT_Pua_Bilancio_TotaliRow(drBTotali)


            rptPua.OpenSubreport("RPT_PUA_PianoDistribuzione.rpt").SetDataSource(DsPianoDistribuzione)
            rptPua.OpenSubreport("RPT_PUA_Bilancio.rpt").SetDataSource(DsPianoDistribuzioneBilancio)

        Catch ex As Exception

            Dim strErr As String = ex.Message

        End Try

    End Sub

    Private Sub publicCarica_DsFertilizzazioni(ByRef DsFertilizzazioni As DS_Pua_Fertilizzazioni,
                                                 piva As String, pua_cod As Integer, regolamento_cod As Integer,
                                                 DataInizio As Date, DataFine As Date, listPiano As List(Of PUA_Appezzamento),
                                                objParametri_Server As AgronicaCoreParametri,
                                                ByRef rptPua As RPT_PUA)

        Try

            Dim objRicette As New AgronicaCoreContabDAL.Ricette_R
            Dim stato As Integer = 300
            Dim filtroQuery As String = "ro.W_Anagrafica_Stati_Cod = " & stato
            'escludo gli scarichi
            Dim dtRicette As DataTable = objRicette.Leggi_xFertilizzazioniPUA(piva, 0, 0, pua_cod, DataInizio, DataFine, True, filtroQuery, "", objParametri_Server)

            Dim dr As DS_Pua_Fertilizzazioni.DT_Pua_FertilizzazioniRow

            If Not dtRicette Is Nothing Then


                Dim listaEpoche As List(Of EpocheModalitaxSpecie) = GetListaEpocheDes(regolamento_cod)

                For i = 0 To dtRicette.Rows.Count - 1

                    dr = DsFertilizzazioni.DT_Pua_Fertilizzazioni.NewDT_Pua_FertilizzazioniRow

                    dr.Data = CDate(dtRicette.Rows(i).Item("ricetta_operazione_data")).ToShortDateString
                    dr.Appezzamento = dtRicette.Rows(i).Item("App_Nome") & " - " & dtRicette.Rows(i).Item("veg_des_op")
                    dr.Fer_Des = dtRicette.Rows(i).Item("fer_des")

                    If Not listPiano Is Nothing AndAlso listPiano.Count > 0 Then
                        Dim obj = (From l In listPiano Where l.Piva = dtRicette.Rows(i).Item("piva_dest") And l.Sa_Cod = dtRicette.Rows(i).Item("sa_cod_dest") And l.appezza = dtRicette.Rows(i).Item("appezza_dest") And l.id_reg = dtRicette.Rows(i).Item("id_reg_dest") Select l).FirstOrDefault()
                        If Not obj Is Nothing Then
                            Select Case obj.ZVN
                                Case True
                                    dr.ZVN = "V"
                                Case Else
                                    dr.ZVN = ""
                            End Select
                        End If
                    End If

                    dr.Superficie = dtRicette.Rows(i).Item("sup_trattata")
                    dr.N = dtRicette.Rows(i).Item("n")
                    dr.Efficienza = dtRicette.Rows(i).Item("efficienza")
                    dr.DoseHa = Math.Round(dtRicette.Rows(i).Item("qta"), 3)
                    dr.DoseTot = Math.Round(dtRicette.Rows(i).Item("Qta_Extra_Totale"), 3)
                    dr.UdmSim = dtRicette.Rows(i).Item("udm_sim")

                    Dim Dose_Kg As Decimal = dtRicette.Rows(i).Item("qta")
                    Dim DoseTot_Kg As Decimal = dtRicette.Rows(i).Item("Qta_Extra_Totale")

                    'riporto in kg per il calcolo di N
                    Select Case dtRicette.Rows(i).Item("extra_int")
                        Case enum_UnitaMisura.Quintali
                            Dose_Kg = Dose_Kg * 100
                            DoseTot_Kg = DoseTot_Kg * 100
                        Case enum_UnitaMisura.Metri_Cubi, enum_UnitaMisura.Tonnellate
                            Dose_Kg = Dose_Kg * 1000
                            DoseTot_Kg = DoseTot_Kg * 1000
                    End Select

                    dr.NNetto = Math.Round(Dose_Kg * dtRicette.Rows(i).Item("n") / 100, 3)
                    dr.NNettoTot = Math.Round(DoseTot_Kg * dtRicette.Rows(i).Item("n") / 100, 3)

                    dr.NUtile = Math.Round(Dose_Kg * dtRicette.Rows(i).Item("n") * dtRicette.Rows(i).Item("efficienza") / 100, 3)
                    dr.NUtileTot = Math.Round(DoseTot_Kg * dtRicette.Rows(i).Item("n") * dtRicette.Rows(i).Item("efficienza") / 100, 3)

                    dr.EpocaCod = dtRicette.Rows(i).Item("EpocaCod")
                    If dtRicette.Rows(i).Item("EpocaCod") <> 0 Then
                        dr.EpocaDes = GetEpocaDes(listaEpoche, dtRicette.Rows(i).Item("EpocaCod"))
                    End If

                    DsFertilizzazioni.DT_Pua_Fertilizzazioni.AddDT_Pua_FertilizzazioniRow(dr)

                Next

            End If

            rptPua.OpenSubreport("RPT_Pua_Fertilizzazioni.rpt").SetDataSource(DsFertilizzazioni)

        Catch ex As Exception

            Dim strErr As String = ex.Message

        End Try


    End Sub

    Public Function SalvaPdf(ByVal objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                         ByRef rpt As RPT_PUA) As String

        ' Dichiara le variabili e restituisce le opzioni di esportazione.
        Dim exportOpts As New ExportOptions
        Dim diskOpts As New DiskFileDestinationOptions
        Dim strPath As String = ""
        Dim PathCartella As String
        Dim Sottocartella As String

        Try

            exportOpts = rpt.ExportOptions

            ' Imposta il formato di esportazione.
            exportOpts.ExportFormatType = ExportFormatType.PortableDocFormat
            exportOpts.ExportDestinationType = ExportDestinationType.DiskFile

            ' leggo la sottocartella da CategorieDocumenti
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.PUA, "", "", objParametri)
            Session("Sottocartella") = Sottocartella    ' mi serve nel piano concimazione massivo
            objCatDoc = Nothing

            Dim objAgroWeb As New AgronicaCoreGestioneRichieste.AgroWebConfig
            If Sottocartella <> "" Then
                PathCartella = objAgroWeb.GestioneAllegati_Repository & Sottocartella
            Else
                If objAgroWeb.GestioneAllegati_Repository.EndsWith("\") Then
                    PathCartella = objAgroWeb.GestioneAllegati_Repository.Substring(0, objAgroWeb.GestioneAllegati_Repository.Length - 1)
                Else
                    PathCartella = objAgroWeb.GestioneAllegati_Repository
                End If
            End If

            ' se il path non esiste, creo tutte le cartelle e sottocartelle
            If Not System.IO.Directory.Exists(PathCartella) Then
                System.IO.Directory.CreateDirectory(PathCartella)
            End If

            Dim NomeFile As String = "PUA_" & Qs_PUA_Cod & "_" & Qs_Piva & ".pdf"

            strPath = PathCartella & "\" & NomeFile

            diskOpts.DiskFileName = strPath
            exportOpts.DestinationOptions = diskOpts

            ' Esportazione del report.
            rpt.Export()


            Dim objEntita As AgronicaCoreScadenziario.Alert_Entita
            Dim objScriviEntita As AgronicaCoreScadenziario.Alert_Entita_W
            Dim objLeggiEntita As AgronicaCoreScadenziario.Alert_Entita_R
            Dim objAllegatiDocumenti As AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
            Dim objSeq As AgronicaCoreDataProvider.Agro_Sequenze


            Try
                objEntita = New AgronicaCoreScadenziario.Alert_Entita
                objScriviEntita = New AgronicaCoreScadenziario.Alert_Entita_W
                objAllegatiDocumenti = New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W

                objLeggiEntita = New AgronicaCoreScadenziario.Alert_Entita_R

                Dim strFiltro As String = "Allegati_Documenti_NomeFile='" & NomeFile & "' AND SottoCartella = '" & Sottocartella & "'"
                Dim dtDoc As DataTable
                dtDoc = objLeggiEntita.Leggi_con_documenti(0, strFiltro, "", objParametri_Server)

                If dtDoc.Rows.Count = 0 Then

                    ' apro la connessione e la transazione (primo parametri dell'interfaccia)
                    AgronicaCoreDataProvider.ConnessioniTransazioni.ApriConnessione(True, objParametri_Server)

                    ' forzati
                    Dim Base, Top As Integer
                    Base = 0
                    Top = 2000000000

                    Dim Allegati_Documenti_Cod As Integer

                    Dim Anno As Integer = Session("Anno")

                    'creo l'entita e il documento associato
                    Dim objDoc As New AgronicaCoreAnagrafeDAL.Allegati_Documenti_W
                    objDoc.Scrivi(Qs_Piva,
                                  "PUA",
                                  enum_CategorieDocumenti.PUA,
                                  NomeFile,
                                  Nothing, Nothing,
                                  Sottocartella,
                                  CDate("01/01/" & Anno), CDate("31/03/" & Anno + 1),
                                  Allegati_Documenti_Cod,
                                  objParametri_Server)


                    objSeq = New AgronicaCoreDataProvider.Agro_Sequenze
                    objEntita.ID_Alert_Entita = objSeq.NuovoId_Tabella("Alert_Entita", Base, Top, objParametri_Server)
                    objEntita.Allegati_Documenti_Cod = Allegati_Documenti_Cod
                    objEntita.PUA_Cod = Qs_PUA_Cod
                    objEntita.Piva = Qs_Piva
                    objEntita.PivaSuperUser = objParametri_Server.PivaSuperUser
                    objEntita.TipoEntita_Cod = enum_TipoEntita.PUA

                    'scrivo l alert entita
                    objScriviEntita.Scrivi(objEntita, CDate("01/01/" & Anno), CDate("31/03/" & Anno + 1), objParametri_Server)

                    ' commit della transazione
                    ChiudiTransazione(1, objParametri_Server)
                    ChiudiConnessione(objParametri_Server)

                End If

            Catch ex As Exception
                Log_Errori += "- Gestione allegati: " + vbCrLf + ex.Message + vbCrLf
            End Try

        Catch ex As Exception
            Log_Errori += "- SalvaPdf: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Return strPath

    End Function

#Region "Get Liste tabelle da Web Service"

    Private Shared Function GetListaStatiImpiantiDes(ByVal regolamentoCod As Integer, ByVal vegCod As Integer) As List(Of Fase)

        Dim objParametriIngresso As New PianoConcimazione_FasiCicloColturale_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod = vegCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_FasiCicloColturale_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.FasiCicloColturaleStatoImpianto(objParametriIngresso)

        Return objParametriUscita.ListaFasi

    End Function

    Private Shared Function GetListaPrecessioniDes(ByVal regolamentoCod As Integer, ByVal puaTipo As Integer) As List(Of Precessione)

        Dim objParametriIngresso As New PianoConcimazione_Precessione_input With {
            .Regolamento_Cod = regolamentoCod,
            .PUA_Tipo = puaTipo,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Precessione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Precessione(objParametriIngresso)

        Return objParametriUscita.ListaPrecessione

    End Function

    Private Shared Function GetListaUbicazioniDes(ByVal regolamentoCod As Integer) As List(Of Ubicazione)

        Dim objParametriIngresso As New PianoConcimazione_Ubicazione_input With {
            .Regolamento_Cod = regolamentoCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_Ubicazione_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.Ubicazione(objParametriIngresso)

        Return objParametriUscita.ListaUbicazione

    End Function

    Private Shared Function GetListaTipiAcquaDes(ByVal regolamentoCod As Integer) As List(Of TipoAcqua)

        Dim objParametriIngresso As New PianoConcimazione_TipoAcqua_input With {
            .Regolamento_Cod = regolamentoCod,
            .Url = ""
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_TipoAcqua_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.TipoAcqua(objParametriIngresso)

        Return objParametriUscita.ListaTipiAcqua

    End Function

    Private Shared Function GetListaFinalitaRer(ByVal regolamentoCod As Integer, ByVal vegcod_elenco As String) As List(Of Finalita)

        Dim objParametriIngresso As New PianoConcimazione_FinalitaRER_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod_Elenco = vegcod_elenco
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscita As PianoConcimazione_FinalitaRER_output = objPC_WS.FinalitaRER(objParametriIngresso)

        Return objParametriUscita.ListaFinalita

    End Function

    Private Shared Function GetListaCoefficienteB(ByVal regolamentoCod As Integer, ByVal vegcod_elenco As String) As List(Of PUA_CoefficienteB)

        Dim objParametriIngresso As New PUA_CoefficienteB_Coltura_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod_Elenco = vegcod_elenco
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscita As PUA_CoefficienteB_Coltura_output = objPC_WS.CoefficienteB_Coltura(objParametriIngresso)

        Return objParametriUscita.ListaCoefficienteB

    End Function

    Private Shared Function GetListaReseMas(ByVal regolamentoCod As Integer, ByVal vegcod_elenco As String) As List(Of PianoConcimazione_LimiteMAS_output)

        Dim objParametriIngressoMAS As New PianoConcimazione_LimiteMAS_input With {
            .Regolamento_Cod = regolamentoCod,
            .Veg_Cod_Elenco = vegcod_elenco
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscitaMASElenco As PianoConcimazione_LimiteMAS_Elenco_output = objPC_WS.LimiteMASElenco(objParametriIngressoMAS)

        Return objParametriUscitaMASElenco.ListaLimitiMas

    End Function

    Private Shared Function GetRegolamentoDes(ByVal regolamentoCod As Integer) As String

        Dim objParametriIngresso As New PianoConcimazione_Regolamenti_input With {
            .Regolamento_Cod = regolamentoCod
        }

        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        Dim objParametriUscitaElenco As PianoConcimazione_Regolamenti_output = objPC_WS.Regolamenti(objParametriIngresso)

        Dim descrizione As String = ""

        If Not objParametriUscitaElenco.ListaRegolamenti Is Nothing AndAlso objParametriUscitaElenco.ListaRegolamenti.Count > 0 Then
            Dim obj = (From l In objParametriUscitaElenco.ListaRegolamenti Where l.Codice = regolamentoCod Select l).FirstOrDefault()
            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetListaEpocheDes(ByVal regolamentoCod As Integer) As List(Of EpocheModalitaxSpecie)

        Dim objParametriIngresso As New PianoConcimazione_EpocheModalitaxSpecie_input With {
            .Regolamento_Cod = regolamentoCod
        }

        Dim objParametriUscita As New AgronicaCorePianoConcimazioneBIZ.PianoConcimazione_EpocheModalitaxSpecie_output
        Dim objPC_WS As New AgronicaCoreWebService.PianoConcimazione_WS
        objParametriUscita = objPC_WS.EpocheModalitaxSpecie(objParametriIngresso)

        Return objParametriUscita.ListaEpocheModalitaxSpecie

    End Function

#End Region

#Region "Ricava Descrizioni"

    Private Shared Function GetStatoImpiantoDes(ByRef dictStatoImpianto As Dictionary(Of Integer, List(Of Fase)),
                                                ByVal regolamentoCod As Integer,
                                                ByVal vegCod As Integer,
                                                ByVal statoImpiantoCod As Integer
                                                ) As String

        Dim descrizione As String = ""

        Dim listaStatiImpianti As List(Of Fase)
        If dictStatoImpianto.ContainsKey(vegCod) Then
            'avevo già letto gli stati, li recupero dal dizionario senza rileggere
            listaStatiImpianti = dictStatoImpianto(vegCod)
        Else
            'non avevo ancora letto gli stati per questo veg_cod, quindi li leggo
            listaStatiImpianti = GetListaStatiImpiantiDes(regolamentoCod, vegCod)
            dictStatoImpianto.Add(vegCod, listaStatiImpianti)
        End If

        If Not listaStatiImpianti Is Nothing AndAlso listaStatiImpianti.Count > 0 Then
            Dim obj = (From l In listaStatiImpianti Where l.Codice = statoImpiantoCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetPrecessioneDes(ByRef listaPrecessioni As List(Of Precessione), ByVal precessioneCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaPrecessioni Is Nothing AndAlso listaPrecessioni.Count > 0 Then
            Dim obj = (From l In listaPrecessioni Where l.Codice = precessioneCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetUbicazioneDes(ByRef listaUbicazioni As List(Of Ubicazione), ByVal ubicazioneCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaUbicazioni Is Nothing AndAlso listaUbicazioni.Count > 0 Then
            Dim obj = (From l In listaUbicazioni Where l.Codice = ubicazioneCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetTipoAcquaDes(ByRef listaTipiAcqua As List(Of TipoAcqua), ByVal tipoAcquaCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaTipiAcqua Is Nothing AndAlso listaTipiAcqua.Count > 0 Then
            Dim obj = (From l In listaTipiAcqua Where l.Codice = tipoAcquaCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetFinalitaRerDes(ByVal listaFinalita As List(Of Finalita), ByVal Grfi_Cod_Rer As Integer) As String

        Dim descrizione As String = ""

        If Not listaFinalita Is Nothing AndAlso listaFinalita.Count > 0 Then
            Dim obj = (From l In listaFinalita Where l.Codice = Grfi_Cod_Rer Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Descrizione
            End If
        End If

        Return descrizione

    End Function

    Private Shared Function GetCoefficienteB(ByVal listaCoefficienteB As List(Of PUA_CoefficienteB),
                                     ByVal Veg_Cod As Integer, ByVal Grfi_Cod_RER As Integer
                                     ) As Decimal

        Dim B_Perc As Decimal = 0

        If Not listaCoefficienteB Is Nothing AndAlso listaCoefficienteB.Count > 0 Then
            Dim obj = (From l In listaCoefficienteB Where l.Veg_Cod = Veg_Cod And l.Grfi_Cod_RER = Grfi_Cod_RER Select l).FirstOrDefault()
            If Not obj Is Nothing Then
                B_Perc = obj.B_Perc
            End If
        End If

        Return B_Perc

    End Function

    Private Shared Function GetEpocaDes(ByRef listaEpoche As List(Of EpocheModalitaxSpecie), ByVal emCod As Integer) As String

        Dim descrizione As String = ""

        If Not listaEpoche Is Nothing AndAlso listaEpoche.Count > 0 Then
            Dim obj = (From l In listaEpoche Where l.Epoca_Cod = emCod Select l).FirstOrDefault()

            If Not obj Is Nothing Then
                descrizione = obj.Epoca_Des
            End If
        End If

        Return descrizione

    End Function

#End Region

#Region "Zone Vulnerabili"

    Private Shared Function IsZonaVulnerabile(ByVal prov As String, ByVal com As String,
                                              ByVal sezione As String, ByVal foglio As Integer,
                                              ByVal numero As Integer, ByVal subalterno As String,
                                              ByRef dtZvnServer As DataTable, ByRef dtZvnMetaschema As DataTable
                                              ) As Boolean

        Dim flagVulnerabile As Boolean = False

        If Not dtZvnServer Is Nothing AndAlso dtZvnServer.Rows.Count > 0 Then

            flagVulnerabile = dtZvnServer.AsEnumerable().Any(Function(x) x.Item("PROV") = prov AndAlso
                                                                         x.Item("COM") = com AndAlso
                                                                         x.Item("SEZIONE") = sezione AndAlso
                                                                         x.Item("FOGLIO") = foglio AndAlso
                                                                         x.Item("NUMERO") = numero AndAlso
                                                                         x.Item("SUBALTERNO") = subalterno)

        End If

        If flagVulnerabile = False AndAlso
           Not dtZvnMetaschema Is Nothing AndAlso dtZvnMetaschema.Rows.Count > 0 Then

            'Se non l'ho trovato nel server lo cerco nel metaschema

            flagVulnerabile = dtZvnMetaschema.AsEnumerable().Any(Function(x) x.Item("PROV") = prov AndAlso
                                                                             x.Item("COM") = com AndAlso
                                                                             x.Item("SEZIONE") = sezione AndAlso
                                                                             x.Item("FOGLIO") = foglio AndAlso
                                                                             x.Item("NUMERO") = numero AndAlso
                                                                             x.Item("SUBALTERNO") = subalterno)

        End If

        Return flagVulnerabile

    End Function

    Private Shared Sub LeggiZoneVulnerabili(ByVal regCod As Integer, ByVal dataInizio As Date, ByVal dataFine As Date,
                                            ByRef dtZvnServer As DataTable,
                                            ByRef dtZvnMetaschema As DataTable,
                                            ByRef objParametriServer As AgronicaCoreParametri)

        Dim filtroDate As String = " (Validita_inizio <= " & Agro_SQL_SaveDate(dataFine) & ")  AND     (Validita_Fine >= " & Agro_SQL_SaveDate(dataInizio) & ") "

        'lettura zone vulnerabili
        Dim objPV As New AgronicaCoreAnagrafeDAL.ZonexParticelle_R
        dtZvnServer = objPV.Leggi(-17,
                                  "", "", "", 0, 0, "",
                                  enumSelezioneVariabile.Selezione_TabellaCompleta,
                                  filtroDate,
                                  "", objParametriServer)


        '----------------------------------------
        'lettura fasce (la metto in un try catch in caso non esista la tabella nel DB PianoConcimazione_Pua e di conseguenza la vista)
        Try
            'TODO: introdurre lettura zone vulnerabili PUA tramite WebService e non in locale?!?
            Dim objPVF As New AgronicaCoreMetaSchemaDAL.ParticelleCatastali_Vulnerabili_R
            dtZvnMetaschema = objPVF.Leggi("", "", "", 0, 0, "",
                                           regCod,
                                           " Fascia_Cod <>0 AND " & filtroDate,
                                           "", objParametriServer)
        Catch ex As Exception

        End Try

    End Sub

#End Region

End Class