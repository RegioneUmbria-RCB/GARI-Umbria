Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.Sicurezza
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.Gestione_Eccezioni_2015
Imports AgronicaCorePianidiCampionamentoBiz

Public Class StampaEtichetta
    Inherits System.Web.UI.Page

    Dim objParametriStampe As AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010

#Region "Variabili"
    Private objParametri_Server As AgronicaCoreParametri
    Private objParametri_Utenti As AgronicaCoreParametri
    Private report_etichetta As report_etichetta
    Private report_etichetta_terremerse As report_etichetta_terremerse
    Private Log_Errori As String
    Private nomeDoc As String
#End Region

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Init

        report_etichetta = New report_etichetta
        report_etichetta_terremerse = New report_etichetta_terremerse
        Log_Errori = ""
        nomeDoc = ""

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        objParametri_Server = CType(Session("ASG_objParametri_Server"), AgronicaCoreParametri)
        objParametri_Utenti = CType(Session("ASG_objParametri_Utenti"), AgronicaCoreParametri)

        objParametriStampe = New AgronicaCoreGestioneRichieste.ParametriAgronicaStampe_2010
        objParametriStampe.Leggi()

        Dim ParametriStampaPDC As New ParametriStampaPDC

        AgronicaCoreUtility.XMLUtility.getObjectFromXml(objParametriStampe.Xml_Filtro, ParametriStampaPDC)


        Dim Id_PDC_Testata As Integer = ParametriStampaPDC.ID_PDC_Testata
        Dim ID_PDC_Dettagli As Integer = ParametriStampaPDC.ID_PDC_Dettagli
        Dim ID_PDC_Campione As Integer = ParametriStampaPDC.ID_PDC_Campione
        Dim Analisi_Testata_Cod As Integer = ParametriStampaPDC.Analisi_Testata_Cod

        'leggo i dati per l'invio
        Dim objLeggiAnalisixMail As New AgronicaCorePianidiCampionamentoDAL.PDC_Analisi_R
        Dim DT As DataTable = objLeggiAnalisixMail.LeggiAnalisixMail(Id_PDC_Testata, ID_PDC_Dettagli, ID_PDC_Campione, Analisi_Testata_Cod, "", "", objParametri_Server)

        DT.Columns.Add("datiImpianto", System.Type.GetType("System.String"))

        Dim listaLFO As List(Of Integer) = (From r As DataRow In DT.Rows Where r.Item("tipo_campione") = 1 Select CInt(r.Item("id_lfo"))).ToList()

        If listaLFO.Count > 0 Then
            Dim objPDC_Dett As New AgronicaCorePianidiCampionamentoDAL.PDC_Dettagli_R
            Dim dtDett As DataTable = objPDC_Dett.Leggi(Id_PDC_Testata, 0, "", 0, 0, 0, 0, 0, 0, 0, " PDC_Dettagli.id_lfo IN (" & String.Join(",", listaLFO) & ") ", "", objParametri_Server)

            For Each dr As DataRow In DT.Rows
                If dr.Item("tipo_campione") = 1 Then
                    Dim lfo As Integer = dr.Item("id_lfo")
                    Dim strApp As String() = (From x As DataRow In dtDett Where x.Item("id_lfo") = lfo Select CStr(x.Item("sa_nome")) & " - " & CStr(x.Item("app_nome")) & " (Sup. " & CStr(x.Item("sup_imp")) & ")").ToArray()

                    dr.Item("datiImpianto") = String.Join(" / ", strApp)

                Else
                    dr.Item("datiImpianto") = CStr(dr.Item("sa_nome")) & " - " & CStr(dr.Item("app_nome")) & " (Sup. " & CStr(dr.Item("sup_imp")) & ")"
                End If

            Next
        Else
            For Each dr As DataRow In DT.Rows
                dr.Item("datiImpianto") = CStr(dr.Item("sa_nome")) & " - " & CStr(dr.Item("app_nome")) & " (Sup. " & CStr(dr.Item("sup_imp")) & ")"
            Next
        End If


        'controllo se in rag_soc è presente la piva 
        If IsNumeric(DT.Rows(0).Item("Rag_soc").split("-")(0)) Then
            Dim str_app As String = ""
            For j As Integer = 1 To DT.Rows(0).Item("Rag_Soc").ToString.Split("-").Length - 1
                If str_app <> "" Then
                    str_app &= "-"
                End If
                str_app &= DT.Rows(0).Item("Rag_Soc").split("-")(j)
            Next
            DT.Rows(0).Item("Rag_Soc") = str_app.Trim()
        End If

        'controllo se in rag_soc_padre è presente la piva 
        If Not IsDBNull(DT.Rows(0).Item("Rag_soc_padre")) AndAlso IsNumeric(DT.Rows(0).Item("Rag_soc_padre").split("-")(0)) Then
            Dim str_app As String = ""
            For j As Integer = 1 To DT.Rows(0).Item("Rag_soc_padre").ToString.Split("-").Length - 1
                If str_app <> "" Then
                    str_app &= "-"
                End If
                str_app &= DT.Rows(0).Item("Rag_soc_padre").split("-")(j)
            Next
            DT.Rows(0).Item("Rag_soc_padre") = str_app.Trim()
        End If

        Dim nomeFile As String = If(DT.Rows(0).Item("Rag_soc_padre") <> "", DT.Rows(0).Item("Rag_soc_padre") & "_", "")
        nomeFile &= DT.Rows(0).Item("Rag_soc") & "_" & DT.Rows(0).Item("Codice_Campione")

        If Not String.IsNullOrEmpty(DT.Rows(0).Item("Rag_Soc_Padre")) Then
            DT.Rows(0).Item("Rag_Soc") &= " (" & DT.Rows(0).Item("Rag_Soc_Padre") & ")"
        End If

        'remove html
        If Not IsDBNull(DT.Rows(0).Item("Analisi_Testata_Note1")) Then
            DT.Rows(0).Item("Analisi_Testata_Note1") = DT.Rows(0).Item("Analisi_Testata_Note1").ToString.Replace("<b>", "").Replace("</b>", "")
        End If
        'remove ___
        If Not IsDBNull(DT.Rows(0).Item("Altre_Molecole")) Then
            DT.Rows(0).Item("Altre_Molecole") = String.Join(", ", DT.Rows(0).Item("Altre_Molecole").ToString.Trim().Split({"___"}, StringSplitOptions.RemoveEmptyEntries))
        End If

        DT.TableName = "A"

        Dim dt3 As New DataTable("LOGO")
        dt3.Columns.Add("Logo", Type.GetType("System.Byte[]"))

        Dim pathlogo As String = Server.MapPath("./" & objParametri_Server.PivaSuperUser & "_Logo.jpg")
        dt3.Rows.Add(dt3.NewRow)

        Carica_Loghi(dt3)

        Dim pathFileTemplate As String = ""

        report_etichetta.Database.Tables("A").SetDataSource(DT)
        report_etichetta.Database.Tables("LOGO").SetDataSource(dt3)

        report_etichetta_terremerse.Database.Tables("A").SetDataSource(DT)
        report_etichetta_terremerse.Database.Tables("LOGO").SetDataSource(dt3)

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

            If objParametri_Server.PivaSuperUser = "00069880391" OrElse objParametri_Server.PivaSuperUser = "02538910403" Then 'Terreemerse, topfruit
                pathPdfGenerato = objGestFile.SalvaReportPdf(report_etichetta_terremerse,
                                        enum_CategorieDocumenti.RegistriCampagna,
                                        Sottocartella,
                                        nomeDoc_Estensione,
                                        objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
            Else
                pathPdfGenerato = objGestFile.SalvaReportPdf(report_etichetta,
                                        enum_CategorieDocumenti.RegistriCampagna,
                                        Sottocartella,
                                        nomeDoc_Estensione,
                                        objParametri_Server, New AgronicaCoreGestioneRichieste.AgroWebConfig)
            End If

            'Elimino il file perché non necessario, verrà generato per l'utente nel VisualizzatoreReport
            IO.File.Delete(pathPdfGenerato)
            'Dim prc As New CrystalDecisions.Shared.ReportPageRequestContext
            'Dim dummy = rptRichiestaCarb.FormatEngine.GetLastPageNumber(prc)

            If objParametri_Server.PivaSuperUser = "00069880391" OrElse objParametri_Server.PivaSuperUser = "02538910403" Then 'Terreemerse, topfruit
                report_etichetta_terremerse.SaveAs(reportTemporaneo, True)
            Else
                report_etichetta.SaveAs(reportTemporaneo, True)
            End If

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
                                            "Stampe_PdC_Etichetta",
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