Imports AgronicaControlli_2010
Imports AgronicaControlli_2010.UtilityPersonalizzazioniGraficheCliente
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreModelsSTD.PersonalizzazioniGraficheCliente
Imports CrystalDecisions.Shared

Public Class ModelloTerzistiBio
    Inherits System.Web.UI.Page

    Private rptModelloTerzisti As Rpt_ModelloTerzistiBio
    Private rptFooterLogo As FooterLogo
    Private Log_Errori As String

    Dim Qs_Piva As String
    Dim Qs_Centri As String
    Dim Qs_MatCod As String
    Dim Qs_Fornitori As String
    Dim Qs_LivelliDettaglio As Integer
    Dim Qs_DataInizio As Date
    Dim Qs_DataFine As Date
    Dim periodo As String

    Dim Param_Rag_Soc As String = ""

    Dim objParametri_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri
    Dim objParametri_Super_Server As New AgronicaCoreDataProvider.AgronicaCoreParametri

    Dim customLoghi As PersonalizzazioniGraficheCliente = Nothing

    '#####################################################################################
    Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init

        rptModelloTerzisti = New Rpt_ModelloTerzistiBio
        rptFooterLogo = New FooterLogo

    End Sub

    '#####################################################################################
    Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Qs_Piva = Stringa_Decodifica(CStr(Request.QueryString("p")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_Centri = Stringa_Decodifica(CStr(Request.QueryString("s")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_MatCod = Stringa_Decodifica(CStr(Request.QueryString("m")),
                                   AgroKey_EncoderDecoder,
                                   Server).Replace("-", "").Replace("|", ", ")

        Qs_Fornitori = Stringa_Decodifica(CStr(Request.QueryString("for")),
                                   AgroKey_EncoderDecoder,
                                   Server).Replace("|", ", ")

        Qs_LivelliDettaglio = Stringa_Decodifica(CStr(Request.QueryString("ld")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_DataInizio = Stringa_Decodifica(CStr(Request.QueryString("di")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        Qs_DataFine = Stringa_Decodifica(CStr(Request.QueryString("df")),
                                   AgroKey_EncoderDecoder,
                                   Server)

        periodo = "" & MonthName(Qs_DataInizio.Month) & " " & "/" & MonthName(Qs_DataFine.Month)

        If (Qs_DataInizio.Year <> Qs_DataFine.Year) Then
            periodo = periodo.Replace(" ", " " & Qs_DataInizio.Year) & " " & Qs_DataFine.Year
        Else
            periodo = periodo & " " & Qs_DataFine.Year
        End If

        objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
        objParametri_Super_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Super_Server"))

        customLoghi = UtilityPersonalizzazioniGraficheCliente.LeggiPersonalizzazioniGraficheCliente(objParametri_Server, objParametri_Super_Server)

        '#################################################################################
        '#####  Genero il report
        '#################################################################################

        Dim Nome_Documento As String = "ModelloTerzistiBio"
        Dim IdentificazioneDocumento As String = ""

        If Not Me.IsPostBack Then

            Try

                '--------------------------------------------
                ' LETTURA DEI DATI
                '--------------------------------------------
                Stampa_ModelloTerzistiBio()

            Catch exc As Exception
                Log_Errori += "- Lettura dati: " + vbCrLf + exc.Message + vbCrLf
            End Try

            Try
                Dim Sottocartella As String
                Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
                Sottocartella = objCatDoc.Sottocartella(enum_CategorieDocumenti.Biologico_MateriePrime, "", "", objParametri_Server)

                Dim NomeFilePDF = Nome_Documento + "_p" & Qs_Piva + "_" + IdentificazioneDocumento + ".pdf"

                ' salvo il report in formato PDF
                Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
                objGestFile.SalvaReportPdf(rptModelloTerzisti,
                                           enum_CategorieDocumenti.Biologico_MateriePrime,
                                           Sottocartella,
                                           NomeFilePDF,
                                           objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)

            Catch ex As Exception

            End Try

            Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()
            Try
                rptModelloTerzisti.SaveAs(reportTemporaneo, True)
            Catch ex As Exception
                Log_Errori += "- Salvataggio report temporaneo: " + vbCrLf + ex.Message + vbCrLf
            End Try

            '-----------------------------------------
            '---- Salvataggio Log Errori -------------
            '-----------------------------------------          
            Dim Nome_File As String

            If Log_Errori <> "" Then

                Log_Errori = Nome_Documento + ", Partita Iva = " & CStr(Qs_Piva) & vbCrLf & vbCrLf & Log_Errori

                Nome_File = "Log_Errori_" & Nome_Documento & ".txt"

                Dim objLog As New GestioneLogStampe
                objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                                 "Biologico",
                                                 Nome_File,
                                                 Session("ASG_Utente_Username"),
                                                 "ModelloTerzistiBio",
                                                 Log_Errori)

            End If

            rptModelloTerzisti.Close()
            rptModelloTerzisti.Dispose()
            rptModelloTerzisti = Nothing

            GC.Collect()


            '-----------------------------------------
            '---- Redirect su VisualizzatoreReport ---
            '-----------------------------------------  
            Response.Redirect("..\..\VisualizzatoreReport.aspx?anteprima=" + Stringa_Codifica("1", AgroKey_EncoderDecoder, Server) +
                                "&tmpReportPath=" + Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server))



        End If

    End Sub

    '#####################################################################################################
    Private Sub Stampa_ModelloTerzistiBio()

        Dim DS_Terzisti As New DS_ModelloTerzistiBio
        Dim DR_Terzisti As DS_ModelloTerzistiBio.DT_ModelloTerzistiBioRow
        Dim DS_LogoFooter As New DS_LogoFooter
        Dim DT As DataTable = Nothing


        '-----------------------------------------
        '---- Query di lettura  -------------
        '-----------------------------------------   

        Try

            Dim objTerz As New AgronicaCoreStampeDAL.ModelloTerzistiBio
            Dim xFiltroAggiuntivo As String = ""
            Dim xOrderBy As String = " Fornitore, i.Progetto_Nome, Progetto, CD.Data_Creazione "

            DT = objTerz.LeggiMovimentiTerzisti(Qs_Piva, Qs_Centri, Qs_MatCod, Qs_DataInizio, Qs_DataFine, Qs_Fornitori,
                                                xFiltroAggiuntivo, xOrderBy,
                                                objParametri_Server)



        Catch ex As Exception
            Log_Errori &= "query Stampa_ModelloTerzistiBio: " & vbCrLf & ex.Message & vbCrLf & vbCrLf
        End Try


        Try

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then

                For i = 0 To DT.Rows.Count - 1

                    DR_Terzisti = DS_Terzisti.DT_ModelloTerzistiBio.NewRow

                    DR_Terzisti.Attività_Svolta = DT.Rows(i).Item("attivita")

                    DR_Terzisti.OP = DT.Rows(i).Item("Progetto_Nome")

                    DR_Terzisti.Progetto = DT.Rows(i).Item("Progetto")

                    DR_Terzisti.Codice = If(IsDBNull(DT.Rows(i).Item("Progetto_Des")), "Terreno nudo", DT.Rows(i).Item("Progetto_Des"))

                    DR_Terzisti.Descrizione_Coltura = DT.Rows(i).Item("Coltura")

                    DR_Terzisti.Fornitore = DT.Rows(i).Item("Fornitore")

                    DR_Terzisti.Descrizione_Campo = DT.Rows(i).Item("Descrizione Campo")

                    DR_Terzisti.Descrizione_Campo = DR_Terzisti.Descrizione_Campo.Split("-").Last

                    DR_Terzisti.Campo = If(IsDBNull(DT.Rows(i).Item("Campo")), "", DT.Rows(i).Item("Campo"))

                    DR_Terzisti.Data = DT.Rows(i).Item("Data_Creazione")

                    DR_Terzisti._Sup__da_Piano_Agricolo = DT.Rows(i).Item("Superficie")

                    DR_Terzisti._Sup__Realmente_Lavorata = DT.Rows(i).Item("Superficie_Lavorata")

                    'If (DR_Terzisti._Sup__Realmente_Lavorata > DR_Terzisti._Sup__da_Piano_Agricolo) Then
                    '    DR_Terzisti._Sup__Realmente_Lavorata = DR_Terzisti._Sup__da_Piano_Agricolo
                    'End If


                    DR_Terzisti.Data = If(Qs_LivelliDettaglio = 1, DR_Terzisti.Data.Split(" ").First, periodo)

                    DS_Terzisti.DT_ModelloTerzistiBio.Rows.Add(DR_Terzisti)

                Next

            End If

            Dim drLogo = DS_LogoFooter.DT_LogoFooter.NewDT_LogoFooterRow
            Dim logo As ConfigurazioneLoghiStampe = TrovaLogoFooterStampe(customLoghi, Log_Errori, objParametri_Server)
            If logo.LogoStampe IsNot Nothing Then
                drLogo.Logo = logo.LogoStampe
                drLogo.TestoPostLogo = logo.TestoPostLogo
                drLogo.TestoPreLogo = logo.TestoPreLogo
            End If
            DS_LogoFooter.DT_LogoFooter.Rows.Add(drLogo)

        Catch ex As Exception
            Log_Errori += "- caricamento dataset: " + vbCrLf + ex.Message + vbCrLf
        End Try

        Try

            '--------------------------------------------
            ' AGGANCIO DATASET AL REPORT
            '--------------------------------------------
            rptModelloTerzisti.SetDataSource(DS_Terzisti)
            rptModelloTerzisti.SetDataSource(DS_LogoFooter)
            rptModelloTerzisti.OpenSubreport("FooterLogo.rpt").SetDataSource(DS_LogoFooter)

            'In caso le personalizzazioni siano attive, nascondo il logo e ragione sociale Agronica.
            'If customLoghi IsNot Nothing Then
            '    rptModelloTerzisti.Section5.ReportObjects("Picture5").ObjectFormat.EnableSuppress = True
            '    rptModelloTerzisti.Section5.ReportObjects("Text9").ObjectFormat.EnableSuppress = True
            'End If

        Catch ex As Exception
            Log_Errori += "- Aggancio dataset al report: " + vbCrLf + ex.Message + vbCrLf
        End Try

        '#########################################################

        Try

            '--------------------------------------------
            ' IMPOSTAZIONE PARAMETRI
            '(va fatto dopo il SetDataSource altrimenti da errore! )
            '--------------------------------------------

            'rptModelloTerzisti.SetParameterValue("Rag_Soc", If(DT.Rows.Count > 0, DT.Rows(0).Item("Fornitore"), "Null"))

        Catch ex As Exception
            Log_Errori &= "- impostazione parametri: " & vbCrLf & ex.Message & vbCrLf
        End Try

    End Sub

End Class