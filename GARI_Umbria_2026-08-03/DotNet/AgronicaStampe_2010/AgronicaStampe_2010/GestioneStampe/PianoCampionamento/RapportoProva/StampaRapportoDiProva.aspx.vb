Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCorePianidiCampionamentoBiz
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015

Public Class StampaRapportoDiProva
    Inherits System.Web.UI.Page

    Dim objParametriStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010

#Region "Variabili"
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private rapporto_prova As rapporto_prova
    Private rapporto_prova_Acquisti As rapporto_prova_Acquisti
    Private rapporto_prova_Altre As rapporto_prova_Altre
    Private Log_Errori As String
    Private nomeDoc As String
#End Region

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        rapporto_prova = New rapporto_prova
        rapporto_prova_Acquisti = New rapporto_prova_Acquisti
        rapporto_prova_Altre = New rapporto_prova_Altre
        Log_Errori = ""
        nomeDoc = ""

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        objParametri_Server = CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri)
        objParametri_Utenti = CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri)

        Dim dt As DataTable = Nothing
        Dim dt2 As DataTable = Nothing

        objParametriStampe = New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010
        objParametriStampe.Leggi()

        Dim ParametriStampaPDC As New ParametriStampaPDC

        AgronicaCoreUtility.XMLUtility.getObjectFromXml(objParametriStampe.Xml_Filtro, ParametriStampaPDC)


        Dim id_pdc_testata As Integer = ParametriStampaPDC.ID_PDC_Testata
        Dim ID_PDC_Campione As Integer = ParametriStampaPDC.ID_PDC_Campione
        Dim Analisi_Testata_Cod As Integer = ParametriStampaPDC.Analisi_Testata_Cod

        Dim objTipo As New AgronicaCoreAnagrafeDAL.Analisi_Testata_R
        Dim tipoAnalisi As Integer = objTipo.Leggi(Analisi_Testata_Cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server).Rows(0).Item("Analisi_Testata_Tipo")

        dt = objTipo.Leggi_Veg_Cod(Analisi_Testata_Cod, 0, AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        Dim leggi_veg_cod As Integer
        leggi_veg_cod = dt.Rows(0).Item("Veg_Cod")
        Dim stampa_selezionata As String

        If tipoAnalisi = 8 Then
            Dim objQ As New AgronicaCorePianidiCampionamentoDAL.Query_x_Report
            dt = objQ.Leggi_Dati_Analisi(id_pdc_testata, ID_PDC_Campione, Analisi_Testata_Cod, objParametri_Server)
            dt.TableName = "A"
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("Note_Campione") <> "" Then
                    dt.Rows(i).Item("Note_Campione") = dt.Rows(i).Item("Note_Campione").trim()
                End If
                If IsDBNull(dt.Rows(i).Item("Analisi_Dettaglio_Valore_1")) Then
                    dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = "<LDM"
                Else
                    If dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = "" Then
                        dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = "<LDM"
                    Else
                        'calcolo RMA
                        Dim ObjDownloadWs As New WS_CapitolatoCliente.AgroWS_CapitolatoCliente
                        ObjDownloadWs.Timeout = If(System.Configuration.ConfigurationManager.AppSettings("ws_timeout"), 600000)

                        Dim objWs As New AgronicaCoreVarieDAL.ConnessioneWS
                        Dim objAgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
                        objWs.NewWS(ObjDownloadWs,
                                        objAgroWebConfig.GiasOnline_WS_CapitolatoCliente_AgroWS_CapitolatoCliente,
                                        objParametri_Utenti)

                        Dim fam_cod As Integer = 0
                        Dim pa_cod As Integer = 0
                        If dt.Rows(i).Item("Analisi_Parametro_Cod") < 0 Then
                            fam_cod = Math.Abs(dt.Rows(i).Item("Analisi_Parametro_Cod"))
                        Else
                            pa_cod = Math.Abs(dt.Rows(i).Item("Analisi_Parametro_Cod"))
                        End If
                        Dim rma As String = ""
                        If leggi_veg_cod > 0 Then
                            rma = ObjDownloadWs.get_LMR(1, pa_cod, fam_cod, dt.Rows(i).Item("Veg_Cod"), dt.Rows(i).Item("data_inizio_analisi"))
                            If rma = "-1" Then
                                dt.Rows(i).Item("RMA") = ""
                            ElseIf IsNumeric(rma) = True Then
                                dt.Rows(i).Item("RMA") = Math.Round(CDbl(rma), 3)
                            Else
                                dt.Rows(i).Item("RMA") = "0"
                            End If
                        Else
                            dt.Rows(i).Item("RMA") = ""
                        End If

                        dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = Math.Round(CDbl(dt.Rows(i).Item("Analisi_Dettaglio_Valore_1").ToString.Replace(".", ",")), 3)
                    End If
                End If
            Next

            dt2 = objQ.Leggi_Dati_Intestazione_Rapporto_Prova(id_pdc_testata, objParametri_Server)
            dt2.TableName = "B"

            Dim dt3 As New DataTable
            dt3.Columns.Add("Logo", Type.GetType("System.Byte[]"))
            dt3.TableName = "LOGO"

            Dim dr As DataRow
            dr = dt3.NewRow
            dt3.Rows.Add(dr)

            Carica_Loghi(dt3)

            If dt.Rows(0).Item("Da_Campagna") = 0 Then

                rapporto_prova.Database.Tables("A").SetDataSource(dt)
                rapporto_prova.Database.Tables("b").SetDataSource(dt2)
                rapporto_prova.Database.Tables("LOGO").SetDataSource(dt3)

                stampa_selezionata = "rapporto_prova"
            Else

                rapporto_prova_Acquisti.Database.Tables("A").SetDataSource(dt)
                rapporto_prova_Acquisti.Database.Tables("b").SetDataSource(dt2)
                rapporto_prova_Acquisti.Database.Tables("LOGO").SetDataSource(dt3)

                stampa_selezionata = "rapporto_prova_Acquisti"
            End If

        Else
            Dim objQ As New AgronicaCorePianidiCampionamentoDAL.Query_x_Report
            dt = objQ.Leggi_Dati_Analisi_Generico(id_pdc_testata, ID_PDC_Campione, Analisi_Testata_Cod, objParametri_Server)
            dt.TableName = "A"
            Dim i As Integer
            For i = 0 To dt.Rows.Count - 1
                If dt.Rows(i).Item("Note_Campione") <> "" Then
                    dt.Rows(i).Item("Note_Campione") = dt.Rows(i).Item("Note_Campione").trim()
                End If
                If IsDBNull(dt.Rows(i).Item("Analisi_Dettaglio_Valore_1")) Then
                    dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = "<LDM"
                Else
                    If dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = "" Then
                        dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = "<LDM"
                    Else
                        'calcolo RMA

                        Dim fam_cod As Integer = 0
                        Dim pa_cod As Integer = 0
                        If dt.Rows(i).Item("Analisi_Parametro_Cod") < 0 Then
                            fam_cod = Math.Abs(dt.Rows(i).Item("Analisi_Parametro_Cod"))
                        Else
                            pa_cod = Math.Abs(dt.Rows(i).Item("Analisi_Parametro_Cod"))
                        End If

                        dt.Rows(i).Item("RMA") = dt.Rows(i).Item("Analisi_Dettaglio_MargineErrore_2")


                        dt.Rows(i).Item("Analisi_Dettaglio_Valore_1") = Math.Round(CDbl(dt.Rows(i).Item("Analisi_Dettaglio_Valore_1").ToString.Replace(".", ",")), 3)
                    End If
                End If
            Next


            dt2 = objQ.Leggi_Dati_Intestazione_Rapporto_Prova(id_pdc_testata, objParametri_Server)
            dt2.TableName = "B"

            Dim dt3 As New DataTable
            dt3.Columns.Add("Logo", Type.GetType("System.Byte[]"))
            dt3.TableName = "LOGO"

            Dim dr As DataRow
            dr = dt3.NewRow
            dt3.Rows.Add(dr)

            Carica_Loghi(dt3)

            rapporto_prova_Altre.Database.Tables("A").SetDataSource(dt)
            rapporto_prova_Altre.Database.Tables("b").SetDataSource(dt2)
            rapporto_prova_Altre.Database.Tables("LOGO").SetDataSource(dt3)

            stampa_selezionata = "rapporto_prova_Altre"
        End If

        Dim reportTemporaneo As String = CrystalHelper.getFileReportTemporaneo()

        Try
            'NOTA Come miglioramento, creare una enum_CategorieDocumenti specifica. Non è obbligatorio in quanto il file generato lo elimino
            Dim objCatDoc As New AgronicaCoreMetaSchemaDAL.CategorieDocumenti_R
            Dim Sottocartella As String = objCatDoc.Sottocartella(enum_CategorieDocumenti.RegistriCampagna, "", "", objParametri_Server)

            Dim rnd As New Random
            Dim nomeDoc_Estensione = nomeDoc & "_" & rnd.Next() & ".pdf"

            'Salvo il report in formato PDF
            Dim objGestFile As New AgronicaCoreVarieBIZ.GestioneFile
            Dim pathPdfGenerato = ""

            Select Case stampa_selezionata
                Case "rapporto_prova"
                    pathPdfGenerato = objGestFile.SalvaReportPdf(rapporto_prova,
                            enum_CategorieDocumenti.RegistriCampagna,
                            Sottocartella,
                            nomeDoc_Estensione,
                            objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
                Case "rapporto_prova_Acquisti"
                    pathPdfGenerato = objGestFile.SalvaReportPdf(rapporto_prova_Acquisti,
                            enum_CategorieDocumenti.RegistriCampagna,
                            Sottocartella,
                            nomeDoc_Estensione,
                            objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
                Case "rapporto_prova_Altre"
                    pathPdfGenerato = objGestFile.SalvaReportPdf(rapporto_prova_Altre,
                            enum_CategorieDocumenti.RegistriCampagna,
                            Sottocartella,
                            nomeDoc_Estensione,
                            objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
            End Select

            'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
            IO.File.Delete(pathPdfGenerato)
            'Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
            'Dim dummy = rptRichiestaCarb.FormatEngine.GetLastPageNumber(prc)

            Select Case stampa_selezionata
                Case "rapporto_prova"
                    rapporto_prova.SaveAs(reportTemporaneo, True)
                Case "rapporto_prova_Acquisti"
                    rapporto_prova_Acquisti.SaveAs(reportTemporaneo, True)
                Case "rapporto_prova_Altre"
                    rapporto_prova_Altre.SaveAs(reportTemporaneo, True)
            End Select

        Catch ex As Exception
            Log_Errori &= "- Salvataggio report temporaneo: " & vbCrLf & MessaggioCompletoDataEccezione(ex, True, source:=True) & vbCrLf
        End Try

        '-----------------------------------------
        '---- Salvataggio Log Errori -------------
        '-----------------------------------------          
        Dim Nome_File_Log As String

        If Log_Errori <> "" Then

            Log_Errori = nomeDoc & vbCrLf & vbCrLf & Log_Errori

            Nome_File_Log = "Log_Errori_" & nomeDoc & ".txt"

            Dim objLog As New GestioneLogStampe
            objLog.Gestione_LogErrori_Stampe(objParametri_Server,
                                            "Stampe_PdC_RapportoDiProva",
                                            Nome_File_Log,
                                            Session("ASG_Utente_Username"),
                                            nomeDoc,
                                            Log_Errori)

        End If


        GC.Collect()


        '-----------------------------------------
        '---- Redirect su VisualizzatoreReport ---
        '-----------------------------------------  
        Response.Redirect(VirtualPathUtility.ToAbsolute("~/GestioneStampe/VisualizzatoreReport.aspx") &
                    "?anteprima=" & Stringa_Codifica("0", AgroKey_EncoderDecoder, Server) &
                    "&tmpReportPath=" & Stringa_Codifica(reportTemporaneo, AgroKey_EncoderDecoder, Server) &
                    "&NomePdf=" & Stringa_Codifica(nomeDoc, AgroKey_EncoderDecoder, Server))


    End Sub

    Sub Carica_Loghi(ByVal DT As DataTable)

        Dim x(0) As Byte
        x(0) = 0
        Dim bmpFx As System.Drawing.Bitmap = New System.Drawing.Bitmap(1, 1)
        bmpFx.SetPixel(0, 0, System.Drawing.Color.White)
        Dim logox() As Byte
        Dim cx As New System.Drawing.ImageConverter
        logox = cx.ConvertTo(bmpFx, GetType(Byte()))

        Try
            Dim objParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri
            objParametri_Server = New AgronicaCoreDataProvider.AgronicaCoreParametri(Session("ASG_objParametri_Server"))
            'Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig(objParametri_Server, False)
            Dim objWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            'Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap("C:\AgroSorgenti - Copia\AgronicaAudit\AgronicaGlobalGap\AB_Immagini\Logo\Logo_Agronica.bmp")
            If objWebConfig.Path_Directory_Loghi_Cliente <> "" Then

                If Not objWebConfig.Path_Directory_Loghi_Cliente.EndsWith("\") Then
                    objWebConfig.Path_Directory_Loghi_Cliente &= "\"
                End If
                Dim inserito As Boolean = False
                ' metto l'immagine1

                Try
                    Dim path As String = objWebConfig.Path_Directory_Loghi_Cliente & objParametri_Server.PivaSuperUser & "_PdC_Logo.jpg"
                    If System.IO.File.Exists(path) Then
                        Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                        Dim logo() As Byte
                        Dim c As New System.Drawing.ImageConverter
                        logo = c.ConvertTo(bmpF, GetType(Byte()))
                        Dim i As Integer = 0
                        DT.Rows(0).Item("Logo") = logo
                        inserito = True
                    End If
                Catch ex As Exception

                End Try

                'If inserito Then
                DT.AcceptChanges()
                'End If

            End If

        Catch ex As Exception

        End Try

    End Sub

End Class