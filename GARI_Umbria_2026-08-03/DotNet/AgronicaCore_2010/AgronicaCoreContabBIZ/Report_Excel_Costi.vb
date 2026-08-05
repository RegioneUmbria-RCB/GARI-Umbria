Imports System.IO
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreUtility
Imports ClosedXML.Excel
Imports Spire.Pdf
Imports Spire.Xls

Public Class Report_Excel_Costi
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Class dataTableExcel
        Public Property ObjImprese As New AgronicaCoreAnagrafeDAL.Imprese_Read

        Public Property DtImprese As DataTable
        Public Property ObjSpecie As New AgronicaCoreMetaSchemaDAL.SpecieVegetali_R
        Public Property DtSpecie As DataTable
        Public Property ObjCultivar As New AgronicaCoreMetaSchemaDAL.Cultivar_R
        Public Property DtCultivar As DataTable
        Public Property ObjImputazioni As New AgronicaCoreContabDAL.Imputazioni_R
        Public Property DtImputazioni As DataTable
        Public Property ObjMaterie As New AgronicaCoreAnagrafeDAL.Materie_Prime_R
        Public Property DtMaterie As DataTable

        Public Sub New(objParametri_Server As AgronicaCoreParametri)
            'Lettura delle Imprese
            DtImprese = ObjImprese.Leggi("", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            'Lettura delle Specie
            DtSpecie = ObjSpecie.Leggi(0, 0, "", "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            'Lettura delle Cultivar
            DtCultivar = ObjCultivar.Leggi(0, 0, "", AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri_Server)

            'Lettura delle Imputazioni
            DtImputazioni = ObjImputazioni.Leggi("", 0, "", "", objParametri_Server)

            'Lettura delle Materie Prime
            DtMaterie = ObjMaterie.Leggi3("", 0, -99, 0, "", True, "", "", objParametri_Server)

        End Sub
    End Class

#Region "Applica stili Excel"

    Private Shared Sub ApplicaStileCosti(worksheet As IXLWorksheet, rigaAttuale As Integer, colonna As Integer)
        worksheet.Cell(rigaAttuale, colonna).Style.Font.Bold = True
        worksheet.Cell(rigaAttuale, colonna).Style.Font.FontSize = 11
        worksheet.Cell(rigaAttuale, colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(207, 226, 243)
        worksheet.Cell(rigaAttuale, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        worksheet.Cell(rigaAttuale, colonna).Style.Font.FontName = "Calibri"

    End Sub

    Private Shared Sub ApplicaStileTipo(ByRef worksheet As IXLWorksheet, rigaTipo As Integer, colonna As Integer)
        worksheet.Cell(rigaTipo, colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 255, 0)
        worksheet.Cell(rigaTipo, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
    End Sub

    Private Shared Sub ApplicaStileIntestazione(ByRef worksheet As IXLWorksheet, rigaIntestazione As Integer, colonna As Integer)
        worksheet.Cell(rigaIntestazione, colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(255, 204, 153)
        worksheet.Cell(rigaIntestazione, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        worksheet.Cell(rigaIntestazione, colonna).Style.Font.Bold = True

    End Sub

    Private Shared Sub ApplicaStileGenerale(ByRef worksheet As IXLWorksheet, riga As Integer, colonna As Integer)
        worksheet.Cell(riga, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
    End Sub

    Private Shared Sub ApplicaStileIntestazioneProduzioneCampo(ByRef worksheet As IXLWorksheet, riga As Integer, colonna As Integer)
        worksheet.Cell(riga, colonna).Style.Font.Bold = True
        worksheet.Cell(riga, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.None)
        worksheet.Cell(riga, colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(171, 205, 239)
    End Sub

    Private Shared Sub ApplicaStileHATon(ByRef worksheet As IXLWorksheet, ByVal riga As Integer, ByVal colonna As Integer)
        worksheet.Cell(riga, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        worksheet.Cell(riga, colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(253, 233, 217)
    End Sub

    Private Shared Sub ApplicaStileColturaProdotto(ByRef worksheet As IXLWorksheet, ByVal riga As Integer, ByVal colonna As Integer)
        worksheet.Cell(riga, colonna).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin)
        worksheet.Cell(riga, colonna).Style.Fill.BackgroundColor = XLColor.FromArgb(235, 241, 222)
    End Sub

#End Region

#Region "Specchietto Excel"

    Private Sub InserisciSpecchiettoExcel(ByVal piva As String, ByVal tipologia As String, ByVal riga As Integer, ByVal colonna As Integer, ByVal datiExcel As dataTableExcel, ByRef worksheet As IXLWorksheet)

        Dim nomeAzienda = datiExcel.DtImprese.Select("Piva = '" & piva & "'")(0).Item("Rag_Soc")

        For i As Integer = 0 To 1
            ApplicaStileIntestazione(worksheet, riga + i, colonna)
            ApplicaStileIntestazione(worksheet, riga + i, colonna + 1)
        Next
        worksheet.Cell(riga, colonna).SetValue("Report")
        worksheet.Cell(riga, colonna + 1).SetValue(tipologia)
        riga += 1
        worksheet.Cell(riga, colonna).SetValue("Impresa")
        worksheet.Cell(riga, colonna + 1).SetValue(nomeAzienda)
    End Sub

#End Region

#Region "PDF/Excel"

    Public Function creaPdfExcel(ByVal aziende As List(Of String),
                                 ByVal sintesiPerAzienda As Integer,
                                 ByVal colturaPerAzienda As Integer,
                                 ByVal sintesiProduzioniColturaCampo As Integer,
                                 ByVal pianoColturale As Integer,
                                 ByVal tipoGenerazioneFile As Integer,
                                 ByVal dataDal As String,
                                 ByVal dataAl As String,
                                 ByVal dataAl_cdg As String,
                                 ByVal costiPersonaleSeparatamente As Integer,
                                 ByVal objParametri_Server As AgronicaCoreParametri,
                                 ByRef aziendeVuote As List(Of String),
                                 ByVal includiAziendeFiglie As Integer,
                                 ByVal DettaglioSpecieVarieta As Integer,
                                 ByVal VediDettagliOperazioni As Integer,
                                 ByRef erroreDatatable As String,
                                 Optional ByRef nomeFile As String = "",
                                 Optional ByRef estensione As String = ""
                                 ) As String
        ''Dim listaXLSX = creaVariReportExcel(aziende, sintesiPerAzienda, colturaPerAzienda, sintesiProduzioniColturaCampo, pianoColturale, 4, dataDal, dataAl, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable)
        Dim pathsPDF As New List(Of String)
        erroreDatatable = "["
        Dim datiExcel As New dataTableExcel(objParametri_Server)
        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim pathTemp As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)
        Dim pathFileZip As String = ""

        Select Case tipoGenerazioneFile
            Case 1
                For Each azienda In aziende
                    Dim pathPDFSingoli As New List(Of String)

                    If sintesiPerAzienda Then
                        Dim nomeFoglio = "Sintesi per SPV"
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport1(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, workbook, azienda, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If colturaPerAzienda Then
                        Dim nomeFoglio = "Coltura Per Azienda"
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport2(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If sintesiProduzioniColturaCampo Then
                        Dim nomeFoglio = "SintesiPCC"
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport3(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If pianoColturale Then
                        Dim nomeFoglio = "pianoColturale"
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport4(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If pathPDFSingoli.Count > 0 Then
                        pathsPDF.Add(unisciPDF(pathPDFSingoli, azienda))
                    End If
                Next

            Case 2

                estensione = ".zip"
                nomeFile = "ReportPDF" & "_" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") & ".zip"
                pathFileZip = pathTemp & nomeFile

                If sintesiPerAzienda Then
                    For Each azienda In aziende
                        Dim nomeFoglio = "Sintesi per SPV"
                        Dim pathPDFSingoli As New List(Of String)
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport1(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, workbook, azienda, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, azienda)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                        pathsPDF.AddRange(pathPDFSingoli)
                        AgroZip.AggiungiPiuFileAZip(pathFileZip, pathPDFSingoli, "SintesiAzienda\" & azienda)
                    Next
                End If

                If colturaPerAzienda Then
                    For Each azienda In aziende
                        Dim nomeFoglio = "Coltura"
                        Dim pathPDFSingoli As New List(Of String)
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport2(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, azienda)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                        pathsPDF.AddRange(pathPDFSingoli)
                        AgroZip.AggiungiPiuFileAZip(pathFileZip, pathPDFSingoli, "ColturaPerAzienda\" & azienda)
                    Next
                End If

                If sintesiProduzioniColturaCampo Then
                    For Each azienda In aziende
                        Dim nomeFoglio = "ColturaCampo"
                        Dim pathPDFSingoli As New List(Of String)
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport3(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, azienda)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                        pathsPDF.AddRange(pathPDFSingoli)
                        AgroZip.AggiungiPiuFileAZip(pathFileZip, pathPDFSingoli, "SintesiProduzioneColturaCampo\" & azienda)
                    Next
                End If

                If pianoColturale Then
                    For Each azienda In aziende
                        Dim nomeFoglio = "Piano Colturale"
                        Dim pathPDFSingoli As New List(Of String)
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport4(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, azienda)
                            pathPDFSingoli.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                        pathsPDF.AddRange(pathPDFSingoli)
                        AgroZip.AggiungiPiuFileAZip(pathFileZip, pathPDFSingoli, "PianoColturale\" & azienda)
                    Next
                End If

            Case 4
                For Each azienda In aziende
                    If sintesiPerAzienda Then
                        Dim nomeFoglio = "SPV " & azienda
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport1(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, workbook, azienda, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, String.Format("{0}_{1}_", "SintesiPerAzienda", azienda))
                            pathsPDF.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If colturaPerAzienda Then
                        Dim nomeFoglio = "ColturaSPV " & azienda
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport2(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, String.Format("{0}_{1}_", "SintesiAziendaColtura", azienda))
                            pathsPDF.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If sintesiProduzioniColturaCampo Then
                        Dim nomeFoglio = "SPCampo " & azienda
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport3(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, String.Format("{0}_{1}_", "SintesiProduziniColturaCampo", azienda))
                            pathsPDF.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If

                    If pianoColturale Then
                        Dim nomeFoglio = "PianoColturale " & azienda
                        Dim workbook = New XLWorkbook()
                        Dim rangeTabelle As New List(Of List(Of Integer))
                        If GenerazioneReport4(dataDal, dataAl, dataAl_cdg, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDatatable, datiExcel, azienda, workbook, nomeFoglio, rangeTabelle) Then
                            Dim pathExcelSuddiviso = suddividiTabelleExcel(workbook, rangeTabelle, String.Format("{0}_{1}_", "Pianocolturale", azienda))
                            pathsPDF.Add(convertiAPDF(pathExcelSuddiviso))
                        End If
                    End If
                Next
        End Select

        If tipoGenerazioneFile <> 2 AndAlso pathsPDF.Count > 0 Then
            estensione = ".zip"
            nomeFile = "ReportPDF" & "_" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") & ".zip"
            pathFileZip = pathTemp & nomeFile
            AgroZip.AggiungiPiuFileAZip(pathFileZip, pathsPDF)
        End If

        If erroreDatatable <> "[" Then
            erroreDatatable = erroreDatatable.Substring(0, erroreDatatable.Length - 1)
        End If
        erroreDatatable &= "]"

        FileSystemHelper.EliminaFiles(pathsPDF)
        Return pathFileZip

    End Function

    Public Function convertiAPDF(ByVal pathFileXlsx As String, Optional ByVal cartelleAggiuntive As String = "") As String
        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim pathFileTemp As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)

        Dim pathsPdf As New List(Of String)
        Dim workbook As Workbook = New Workbook()


        workbook.LoadFromFile(pathFileXlsx)

        For Each worksheet In workbook.Worksheets
            'worksheet.AllocatedRange.BorderInside(LineStyleType.Thin)
            worksheet.PageSetup.TopMargin = 0.1
            worksheet.PageSetup.BottomMargin = 0.1
            worksheet.PageSetup.LeftMargin = 0.1
            worksheet.PageSetup.RightMargin = 0.1
            'worksheet.PageSetup.PaperSize = PaperSizeType.PaperA4Rotated
        Next
        workbook.ConverterSetting.SheetFitToPage = True
        'workbook.ConverterSetting.SheetFitToWidth = True

        Dim arrayPath = pathFileXlsx.Split("\")
        Dim nomepdf As String = arrayPath.ElementAt(arrayPath.Count - 1).Split(".").ElementAt(0)
        Dim path = pathFileTemp & cartelleAggiuntive & nomepdf & ".pdf"
        workbook.SaveToFile(path, Spire.Xls.FileFormat.PDF)
        FileSystemHelper.EliminaFile(pathFileXlsx)
        Return path

    End Function

    Public Function unisciPDF(ByVal pathsPDF As List(Of String), ByVal nomeFile As String) As String
        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim pathFileTemp As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)

        Dim doc As PdfDocumentBase = PdfDocument.MergeFiles(pathsPDF.ToArray)
        Dim path = pathFileTemp & nomeFile & "___" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") & ".pdf"
        doc.Save(path, Spire.Pdf.FileFormat.PDF)
        FileSystemHelper.EliminaFiles(pathsPDF)
        Return path
    End Function

    Public Function salvaExcel(nome As String, path As String, ByRef workbook As XLWorkbook) As String
        Dim pathFileXlsx As String
        Dim nomeReport As String
        nomeReport = nome & "__" & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "")
        pathFileXlsx = path & nomeReport & ".xlsx"
        workbook.SaveAs(pathFileXlsx)
        Return pathFileXlsx
    End Function

    Public Function creaFileZip(nomeReport As String, listaFileXLSX As List(Of String),
                                Optional ByRef nomeFile As String = "",
                                Optional ByRef estensione As String = ""
                                ) As String

        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim path As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)

        nomeFile = nomeReport & Format(DateTime.Now, "yyyy-MM-dd").Replace(" ", "") & "_" & Format(DateTime.Now, "HHmm ssffff").Replace(" ", "") & ".zip"
        estensione = ".zip"
        Dim pathFileZip As String = path & nomeFile

        AgroZip.AggiungiPiuFileAZip(pathFileZip, listaFileXLSX, "", True) ' sovrascrivi sempre

        Return pathFileZip

    End Function

    Public Function suddividiTabelleExcel(ByRef workbook As XLWorkbook, ByVal rangeTabelle As List(Of List(Of Integer)), Optional nomeFile As String = "tempExcel") As String
        Dim nomeRoutine = "DW_CDG_Costi_Ricavi_Biz.suddividiTabelleExcel()"
        Dim messaggioErrore As String = ""
        Try

            Dim workbookSuddiviso = New XLWorkbook()
            For i As Integer = 0 To rangeTabelle.Count - 1
                Dim primaRiga = rangeTabelle(i)(0)
                Dim primaColonna = rangeTabelle(i)(1)
                Dim ultimaRiga = rangeTabelle(i)(2)
                Dim ultimaColonna = rangeTabelle(i)(3)


                Dim ws = workbookSuddiviso.Worksheets.Add(i.ToString)
                ws.Cell(1, 1).Value = workbook.Worksheet(1).Range(primaRiga, primaColonna, ultimaRiga, ultimaColonna)
                ws.PageSetup.FitToPages(1, 1)
                ws.Columns().AdjustToContents()
                ws.Rows().AdjustToContents()
            Next


            Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
            Dim path As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)
            Return salvaExcel(nomeFile, path, workbookSuddiviso)
        Catch ex As Exception
            messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

    Public Sub configurazioneGeneraleExcel(ByRef worksheet As IXLWorksheet)
        worksheet.PageSetup.FitToPages(1, 1)
        worksheet.PageSetup.Margins.Bottom = 0.0
        worksheet.PageSetup.Margins.Top = 0.0
        worksheet.PageSetup.Margins.Left = 0.0
        worksheet.PageSetup.Margins.Right = 0.0

        worksheet.PageSetup.Margins.Footer = 0.0
        worksheet.PageSetup.Margins.Header = 0.0
        worksheet.Rows().AdjustToContents()
        worksheet.Columns().AdjustToContents()
    End Sub

#End Region

#Region "Creazione tutti i report"

    Public Function creaVariReportExcel(ByVal aziende As List(Of String), ByVal sintesiPerAzienda As Integer,
                                        ByVal colturaPerAzienda As Integer, ByVal sintesiProduzioniColturaCampo As Integer,
                                        ByVal pianoColturale As Integer, ByVal tipoGenerazioneFile As Integer,
                                        ByVal dataDal As String, ByVal dataAl As String, dataAl_CDG As String,
                                        ByVal costiPersonaleSeparatamente As Integer, ByVal objParametri_Server As AgronicaCoreParametri,
                                        ByRef aziendeVuote As List(Of String), ByVal includiAziendeFiglie As Integer,
                                        ByVal DettaglioSpecieVarieta As Integer,
                                        ByVal VediDettagliOperazioni As Integer,
                                        ByRef erroreDataTable As String
                                        ) As List(Of String)

        Dim _AgroWebConfig As New AgronicaCoreGestioneRichieste.AgroWebConfig
        Dim path As String = FileSystemHelper.AggiungiSlashSeNonEsiste(_AgroWebConfig.PathFileTemporanei)


        erroreDataTable = "["

        Dim listaXLSX As New List(Of String)

        Dim datiExcel As New dataTableExcel(objParametri_Server)

        Select Case tipoGenerazioneFile
            Case 1
                For Each azienda In aziende
                    Dim workbook = New XLWorkbook()

                    If sintesiPerAzienda Then
                        Dim nomeFoglio = "Sintesi per SPV"
                        GenerazioneReport1(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, workbook, azienda, nomeFoglio)

                    End If
                    If colturaPerAzienda Then
                        Dim nomeFoglio = "Sintesi Coltura per SPV"
                        GenerazioneReport2(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    End If
                    If sintesiProduzioniColturaCampo Then
                        Dim nomeFoglio = "Sintesi Produzione ColturaCampo"
                        GenerazioneReport3(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    End If
                    If pianoColturale Then
                        Dim nomeFoglio = "Piano Colturale"
                        GenerazioneReport4(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    End If

                    If (workbook.Worksheets.Count > 0) Then
                        listaXLSX.Add(salvaExcel("AZIENDA_" & azienda & "__", path, workbook))
                    End If


                Next
            Case 2
                If sintesiPerAzienda Then
                    Dim workbook = New XLWorkbook()
                    For Each azienda In aziende
                        Dim nomeFoglio = azienda
                        GenerazioneReport1(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, workbook, azienda, nomeFoglio)

                    Next
                    If (workbook.Worksheets.Count > 0) Then
                        listaXLSX.Add(salvaExcel("REPORT_SintesiPerAzienda__", path, workbook))
                    End If

                End If

                If colturaPerAzienda Then
                    Dim workbook = New XLWorkbook()
                    For Each azienda In aziende
                        Dim nomeFoglio = azienda
                        GenerazioneReport2(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)

                    Next
                    If (workbook.Worksheets.Count > 0) Then
                        listaXLSX.Add(salvaExcel("REPORT_ColturaPerAzienda__", path, workbook))
                    End If

                End If

                If sintesiProduzioniColturaCampo Then
                    Dim workbook = New XLWorkbook()
                    For Each azienda In aziende
                        Dim nomeFoglio = azienda
                        GenerazioneReport3(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    Next
                    If (workbook.Worksheets.Count > 0) Then
                        listaXLSX.Add(salvaExcel("REPORT_sintesiProduzioniColturaCampo__", path, workbook))
                    End If


                End If

                If pianoColturale Then
                    Dim workbook = New XLWorkbook()
                    For Each azienda In aziende
                        Dim nomeFoglio = azienda
                        GenerazioneReport4(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    Next
                    If (workbook.Worksheets.Count > 0) Then
                        listaXLSX.Add(salvaExcel("REPORT_pianoColturale_", path, workbook))
                    End If



                End If


            Case 3
                Dim workbook = New XLWorkbook()
                For Each azienda In aziende

                    If sintesiPerAzienda Then
                        Dim nomeFoglio = "SPV " & azienda
                        GenerazioneReport1(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, workbook, azienda, nomeFoglio)
                    End If
                    If colturaPerAzienda Then
                        Dim nomeFoglio = "Coltura SPV " & azienda
                        GenerazioneReport2(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    End If
                    If sintesiProduzioniColturaCampo Then
                        Dim nomeFoglio = "ColturaCampo " & azienda
                        GenerazioneReport3(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)
                    End If
                    If pianoColturale Then
                        Dim nomeFoglio = "PianoColturale " & azienda
                        GenerazioneReport4(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio)

                    End If

                Next
                If workbook.Worksheets.Count > 0 Then
                    listaXLSX.Add(salvaExcel("REPORT_TUTTO__", path, workbook))
                End If

            Case 4
                For Each azienda In aziende
                    If sintesiPerAzienda Then
                        Dim nomeFoglio = "SPV " & azienda
                        Dim workbook = New XLWorkbook()
                        If GenerazioneReport1(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, workbook, azienda, nomeFoglio) Then
                            listaXLSX.Add(salvaExcel("SintesiAzienda__" & azienda, path, workbook))
                        End If

                    End If
                    If colturaPerAzienda Then
                        Dim workbook = New XLWorkbook()
                        Dim nomeFoglio = "ColturaSPV " & azienda
                        If GenerazioneReport2(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio) Then
                            listaXLSX.Add(salvaExcel("SintesiColturaAzienda__" & azienda, path, workbook))
                        End If
                    End If
                    If sintesiProduzioniColturaCampo Then
                        Dim workbook = New XLWorkbook()
                        Dim nomeFoglio = "SPCampo " & azienda
                        If GenerazioneReport3(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio) Then
                            listaXLSX.Add(salvaExcel("SPColtura&Campoa__" & azienda, path, workbook))
                        End If

                    End If
                    If pianoColturale Then
                        Dim workbook = New XLWorkbook()
                        Dim nomeFoglio = "PianoColturale " & azienda
                        If GenerazioneReport4(dataDal, dataAl, dataAl_CDG, costiPersonaleSeparatamente, objParametri_Server, aziendeVuote, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, erroreDataTable, datiExcel, azienda, workbook, nomeFoglio) Then
                            listaXLSX.Add(salvaExcel("PianoColturale__" & azienda, path, workbook))
                        End If


                    End If

                Next
        End Select

        If erroreDataTable <> "[" Then
            erroreDataTable = erroreDataTable.Substring(0, erroreDataTable.Length - 1)
        End If
        erroreDataTable &= "]"
        Return listaXLSX

    End Function

#End Region


#Region "Report 1 - Sintesi Azienda"

    Private Function GenerazioneReport1(dataDal As String, dataAl As String, dataAl_cdg As String, costiPersonaleSeparatamente As Integer, ByRef objParametri_Server As AgronicaCoreParametri, aziendeVuote As List(Of String), includiAziendeFiglie As Integer, DettaglioSpecieVarieta As Integer, VediDettagliOperazioni As Integer, ByRef erroreDataTable As String, datiExcel As dataTableExcel, ByRef workbook As XLWorkbook, azienda As String, nomeFoglio As String, Optional rangeTabelle As List(Of List(Of Integer)) = Nothing) As Boolean

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim pivaReale As String = objImp.Leggi_PivaReale(azienda, objParametri_Server)

        Try
            Dim dtintestazione As New DataTable
            Dim dtfinale As New DataTable
            Dim dtReport3 As New DataTable
            Dim dwCdgBiz As New AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ
            Dim strErrore = dwCdgBiz.Ricerca_DT_SPV(dtintestazione, dtfinale, dtReport3, azienda, dataDal, dataAl, dataAl_cdg, 1, costiPersonaleSeparatamente, includiAziendeFiglie, objParametri_Server, DettaglioSpecieVarieta, VediDettagliOperazioni)
            If strErrore <> "" Then
                erroreDataTable &= creazioneStringaErroreDatatable(2, azienda, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, strErrore) & ","
            End If
            workbook.Worksheets.Add(nomeFoglio)
            Dim worksheet = workbook.Worksheet(nomeFoglio)

            If (Not ExportToExcel_Report_SintesiAzienda(dtintestazione, dtfinale, objParametri_Server, azienda, worksheet, datiExcel, rangeTabelle)) Then
                aziendeVuote.Add(azienda & "?" & pivaReale & "?" & "Sintesi Per Azienda")
                workbook.Worksheets.Delete(nomeFoglio)
                Return False
            End If
        Catch ex As Exception
            erroreDataTable &= creazioneStringaErroreDatatable(1, pivaReale, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, ex.Message) & ","
        End Try
        Return True
    End Function

    Private Function ExportToExcel_Report_SintesiAzienda(ByVal dtintestazione As DataTable,
                                          ByVal dt As DataTable,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByVal piva As String,
                                          ByRef worksheet As IXLWorksheet,
                                          ByVal datiExcel As dataTableExcel,
                                          Optional ByRef rangeTabelle As List(Of List(Of Integer)) = Nothing
                                          ) As Boolean
        Try
            If ((dt Is Nothing OrElse dt.Rows.Count = 0) OrElse 
                (dtintestazione Is Nothing OrElse dtintestazione.Rows.Count = 0)) Then
                Return False
            End If
            If rangeTabelle Is Nothing Then
                rangeTabelle = New List(Of List(Of Integer))
            End If


            Dim rigaIntestazione = 5
            Dim rigaHA = rigaIntestazione + 1
            Dim rigaTon = rigaIntestazione + 2
            'riga tipo è la riga dove c'è scritto se la tabella ha i valori in €, €/Ha o €/Ton
            Dim rigaTipo = rigaIntestazione + 3
            Dim rigaAttuale As Integer = rigaIntestazione + 4


            Dim result = From rows In dt.AsEnumerable()
                         Group rows By Key = New With {.Gruppo1 = rows("Des_Attivita_Gruppo1"), .Gruppo2 = rows("Des_Attivita_Gruppo2")} Into Group
                         Select Group

            Dim resultIntest = From rows In dtintestazione.AsEnumerable()
                               Group rows By Key = New With {.Gruppo1 = rows("Des_Attivita_Gruppo1"), .Gruppo2 = rows("Des_Attivita_Gruppo2")} Into Group
                               Select Group

            Dim gruppo1 As String = ""
            Dim gruppo2 As String = ""

            Dim TotaleComplessivo As Double

            Dim DtImprese As DataTable = datiExcel.DtImprese


            'Lettura delle Imprese
            InserisciSpecchiettoExcel(piva, "Sintesi Per SPV", 1, 1, datiExcel, worksheet)

            Dim a = 1
            If a = 1 Then
                Dim range As New List(Of Integer)
                range.Add(1)
                range.Add(1)

                Dim ultimaColonna As Integer

                worksheet.Cell(rigaIntestazione, 1).SetValue("Tipo")
                worksheet.Cell(rigaIntestazione, 2).SetValue("€")
                worksheet.Cell(rigaIntestazione, 3).SetValue("INTESTAZIONE")
                'worksheet.Cell(rigaTipo, 3).SetValue("€")
                For colonna As Integer = 1 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                Next

                ApplicaStileIntestazione(worksheet, rigaIntestazione, 3)

                ApplicaStileTipo(worksheet, rigaTipo, 3)


                For colonna As Integer = 4 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    'applicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                    ApplicaStileGenerale(worksheet, rigaHA, colonna)
                    ApplicaStileGenerale(worksheet, rigaTon, colonna)
                    ApplicaStileTipo(worksheet, rigaTipo, colonna)
                Next

                'intestazione
                rigaAttuale = rigaHA


                '=================================================================================================================
                'Ragione Sociale
                '-----------------------------
                Dim dr_search As DataRow()

                For nc As Integer = 6 To dtintestazione.Columns.Count - 1

                    piva = dtintestazione.Columns(nc).ColumnName

                    If Trim(piva) <> "" Then

                        'Inserimento SPV come prima azienda in Array Intestazione
                        dr_search = DtImprese.Select("Piva = '" & piva & "'")

                        If dr_search.Length <> 0 Then
                            worksheet.Cell(rigaIntestazione, nc).SetValue(dr_search(0).Item("Rag_Soc"))
                        End If


                    End If

                Next

                '=================================================================================================================


                For Each item In resultIntest
                    TotaleComplessivo = 0

                    Dim drItem As DataRow = item.FirstOrDefault()

                    If drItem Is Nothing Then
                        Continue For
                    End If

                    'parte costi fissi e variabili e tipologia
                    'gruppo1 = if(drItem.Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, drItem.Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                    'worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)
                    'If gruppo1 <> String.Empty Then
                    '    applicaStileCosti(worksheet, rigaAttuale, 1)
                    'End If

                    'gruppo2 = if(drItem.Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, drItem.Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                    'worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)

                    'If gruppo2 <> String.Empty Then
                    '    applicaStileCosti(worksheet, rigaAttuale, 2)
                    'End If
                    'fine parte costi fissi e variabili


                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)

                    worksheet.Cell(rigaAttuale, "C").SetValue(drItem.Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "C").Style.Font.Bold = True

                    worksheet.Cell(rigaAttuale, "D").SetValue(drItem.Item("Lav_Des_Agenda"))

                    For colonna As Integer = 3 To drItem.ItemArray.Count()
                        ApplicaStileHATon(worksheet, rigaAttuale, colonna)
                    Next
                    'parte che mette HA e TON
                    For y As Integer = 6 To drItem.ItemArray.Count() - 1
                        If IsNumeric(drItem.ItemArray(y)) Then
                            Dim value As Double = CDbl(drItem.ItemArray(y))

                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)

                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"

                    Next

                    worksheet.Cell(rigaAttuale, drItem.ItemArray.Count()).SetValue(TotaleComplessivo)
                    worksheet.Cell(rigaAttuale, drItem.ItemArray.Count()).Style.NumberFormat.Format = "0.00"
                    rigaAttuale += 1
                Next


                rigaAttuale += 1

                If result.Count > 0 Then

                    Dim totaliPerAzienda As New List(Of Double)
                    Dim inizializzato As Boolean
                    inizializzato = False

                    'righe della tabella
                    For Each item In result
                        TotaleComplessivo = 0

                        Dim drItem As DataRow = item.FirstOrDefault()

                        If drItem Is Nothing Then
                            Continue For
                        End If

                        gruppo1 = If(drItem.Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, drItem.Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                        worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)
                        If gruppo1 <> String.Empty Then
                            ApplicaStileCosti(worksheet, rigaAttuale, 1)
                        End If
                        worksheet.Cell(rigaAttuale, "A").Style.Font.FontName = "Calibri"

                        gruppo2 = If(drItem.Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, drItem.Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                        worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)
                        If gruppo2 <> String.Empty Then
                            ApplicaStileCosti(worksheet, rigaAttuale, 2)

                        End If
                        worksheet.Cell(rigaAttuale, "C").SetValue(drItem.Item("Attivita_Des"))
                        ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                        ApplicaStileGenerale(worksheet, rigaAttuale, 4)
                        worksheet.Cell(rigaAttuale, "D").SetValue(drItem.Item("Lav_Des_Agenda"))
                        ApplicaStileGenerale(worksheet, rigaAttuale, 5)
                        worksheet.Cell(rigaAttuale, 4).Style.Border.SetRightBorder(XLBorderStyleValues.Thick)
                        Dim y As Integer


                        For y = 6 To drItem.ItemArray.Count() - 1
                            If IsNumeric(drItem.ItemArray(y)) Then
                                If Not inizializzato Then
                                    totaliPerAzienda.Add(drItem.ItemArray(y))
                                Else
                                    totaliPerAzienda(y - 6) += drItem.ItemArray(y)
                                End If
                                TotaleComplessivo += drItem.ItemArray(y)
                                ApplicaStileGenerale(worksheet, rigaAttuale, y)
                                worksheet.Cell(rigaAttuale, y).SetValue(drItem.ItemArray(y))
                            Else
                                worksheet.Cell(rigaAttuale, y).SetValue(0)
                            End If
                            worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"

                        Next
                        inizializzato = True

                        'totali di riga
                        Dim k As Integer = drItem.ItemArray.Count()
                        worksheet.Cell(rigaAttuale, k).SetValue(Math.Round(TotaleComplessivo, 2))
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)

                        worksheet.Cell(rigaAttuale, k).Style.NumberFormat.Format = "0.00"

                        worksheet.Cell(rigaIntestazione, k).SetValue("TOTALE COMPLESSIVO")
                        ApplicaStileGenerale(worksheet, rigaIntestazione, k)
                        ApplicaStileIntestazione(worksheet, rigaIntestazione, k)
                        ApplicaStileIntestazione(worksheet, rigaAttuale, k)
                        ApplicaStileGenerale(worksheet, rigaHA, k)
                        ApplicaStileGenerale(worksheet, rigaTon, k)
                        ApplicaStileTipo(worksheet, rigaTipo, k)

                        rigaAttuale += 1
                    Next



                    If totaliPerAzienda.Count > 0 Then
                        totaliPerAzienda.Add(totaliPerAzienda.Sum)
                    End If
                    Dim j = 6
                    For Each totale In totaliPerAzienda

                        worksheet.Cell(rigaAttuale, 3).SetValue("Totale gestione")

                        For i As Integer = 0 To 2

                            ApplicaStileTipo(worksheet, rigaAttuale + i, 3)
                            ApplicaStileTipo(worksheet, rigaAttuale + i, 4)
                            ApplicaStileTipo(worksheet, rigaAttuale + i, 5)
                            ApplicaStileTipo(worksheet, rigaAttuale + i, j)
                            worksheet.Cell(rigaAttuale + i, j).Style.NumberFormat.Format = "0.00"
                        Next

                        worksheet.Cell(rigaAttuale + 1, 3).SetValue("€/ha")
                        worksheet.Cell(rigaAttuale + 2, 3).SetValue("€/ton")

                        worksheet.Cell(rigaAttuale, j).SetValue(totale)
                        worksheet.Cell(rigaAttuale, j).Style.NumberFormat.Format = "0.00"

                        Dim valore = Math.Round(If(worksheet.Cell(rigaHA, j).Value <> 0, totale / worksheet.Cell(rigaHA, j).Value, 0), 2)
                        worksheet.Cell(rigaAttuale + 1, j).SetValue(valore)

                        valore = Math.Round(If(worksheet.Cell(rigaTon, j).Value <> 0, totale / worksheet.Cell(rigaTon, j).Value, 0), 2)
                        worksheet.Cell(rigaAttuale + 2, j).SetValue(valore)

                        j += 1
                    Next
                    ultimaColonna = j
                End If
                range.Add(rigaAttuale + 2)
                range.Add(ultimaColonna - 1)
                rangeTabelle.Add(range)
            End If
            'fine prima parte








            a = 2

            'i += 6
            rigaAttuale = rigaAttuale + 6
            If a = 2 Then
                rigaIntestazione = rigaAttuale + 1
                rigaHA = rigaAttuale + 2
                rigaTon = rigaAttuale + 3
                'riga tipo è la riga dove c'è scritto se la tabella ha i valori in €, €/Ha o €/Ton
                rigaTipo = rigaAttuale + 4
                rigaAttuale += 5
                Dim range As New List(Of Integer)
                range.Add(rigaIntestazione)
                range.Add(1)

                Dim ultimaColonna As Integer

                worksheet.Cell(rigaIntestazione, 1).SetValue("Tipo")
                worksheet.Cell(rigaIntestazione, 2).SetValue("€/Ha")
                worksheet.Cell(rigaIntestazione, 3).SetValue("INTESTAZIONE")
                'worksheet.Cell(rigaTipo, 3).SetValue("€")
                For colonna As Integer = 1 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                Next

                ApplicaStileIntestazione(worksheet, rigaIntestazione, 3)

                ApplicaStileTipo(worksheet, rigaTipo, 3)


                For colonna As Integer = 4 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    'applicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                    ApplicaStileGenerale(worksheet, rigaHA, colonna)
                    ApplicaStileGenerale(worksheet, rigaTon, colonna)
                    ApplicaStileTipo(worksheet, rigaTipo, colonna)
                Next

                'intestazione
                rigaAttuale = rigaHA


                '=================================================================================================================
                'Ragione Sociale
                '-------------------------------------------------------------------------------------------------------------------------------
                Dim dr_search As DataRow()

                For nc As Integer = 6 To dtintestazione.Columns.Count - 1

                    piva = dtintestazione.Columns(nc).ColumnName

                    If Trim(piva) <> "" Then

                        'Inserimento SPV come prima azienda in Array Intestazione
                        dr_search = DtImprese.Select("Piva = '" & piva & "'")

                        If dr_search.Length <> 0 Then
                            worksheet.Cell(rigaIntestazione, nc).SetValue(dr_search(0).Item("Rag_Soc"))
                        End If


                    End If

                Next

                '=================================================================================================================


                For Each item In resultIntest
                    TotaleComplessivo = 0

                    'parte costi fissi e variabili e tipologia
                    'gruppo1 = if(item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                    'worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)
                    'If gruppo1 <> String.Empty Then
                    '    applicaStileCosti(worksheet, rigaAttuale, 1)
                    'End If

                    'gruppo2 = if(item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                    'worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)

                    'If gruppo2 <> String.Empty Then
                    '    applicaStileCosti(worksheet, rigaAttuale, 2)
                    'End If
                    'fine parte costi fissi e variabili


                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)
                    For colonna As Integer = 3 To item.FirstOrDefault().ItemArray.Count()
                        ApplicaStileHATon(worksheet, rigaAttuale, colonna)
                    Next
                    worksheet.Cell(rigaAttuale, "C").SetValue(item.FirstOrDefault().Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "D").SetValue(item.FirstOrDefault().Item("Lav_Des_Agenda"))

                    'parte che mette HA e TON
                    For y As Integer = 6 To item.FirstOrDefault().ItemArray.Count() - 1
                        If IsNumeric(item.FirstOrDefault().ItemArray(y)) Then
                            Dim value As Double = CDbl(item.FirstOrDefault().ItemArray(y))

                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                            worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"
                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If

                    Next

                    worksheet.Cell(rigaAttuale, item.FirstOrDefault().ItemArray.Count()).SetValue(TotaleComplessivo)
                    worksheet.Cell(rigaAttuale, item.FirstOrDefault().ItemArray.Count()).Style.NumberFormat.Format = "0.00"
                    rigaAttuale += 1
                Next


                rigaAttuale += 1

                If result.Count > 0 Then

                    Dim totaliPerAzienda As New List(Of Double)
                    Dim inizializzato As Boolean
                    inizializzato = False

                    'righe della tabella
                    For Each item In result
                        TotaleComplessivo = 0

                        gruppo1 = If(item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                        worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)
                        If gruppo1 <> String.Empty Then
                            ApplicaStileCosti(worksheet, rigaAttuale, 1)
                        End If
                        worksheet.Cell(rigaAttuale, "A").Style.Font.FontName = "Calibri"

                        gruppo2 = If(item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                        worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)
                        If gruppo2 <> String.Empty Then
                            ApplicaStileCosti(worksheet, rigaAttuale, 2)

                        End If
                        worksheet.Cell(rigaAttuale, "C").SetValue(item.FirstOrDefault().Item("Attivita_Des"))
                        ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                        ApplicaStileGenerale(worksheet, rigaAttuale, 4)
                        worksheet.Cell(rigaAttuale, "D").SetValue(item.FirstOrDefault().Item("Lav_Des_Agenda"))
                        ApplicaStileGenerale(worksheet, rigaAttuale, 5)
                        worksheet.Cell(rigaAttuale, 4).Style.Border.SetRightBorder(XLBorderStyleValues.Thick)

                        Dim y As Integer


                        For y = 6 To item.FirstOrDefault().ItemArray.Count() - 1
                            Dim valoreHA = worksheet.Cell(rigaHA, y).Value
                            If IsNumeric(item.FirstOrDefault().ItemArray(y)) And valoreHA <> 0 Then
                                Dim value = item.FirstOrDefault().ItemArray(y) / valoreHA
                                TotaleComplessivo += value
                                worksheet.Cell(rigaAttuale, y).SetValue(value)
                            Else
                                worksheet.Cell(rigaAttuale, y).SetValue(0)
                            End If
                            ApplicaStileGenerale(worksheet, rigaAttuale, y)
                            worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"

                        Next
                        inizializzato = True

                        'totali di riga
                        Dim k As Integer = item.FirstOrDefault().ItemArray.Count()
                        worksheet.Cell(rigaAttuale, k).SetValue(Math.Round(TotaleComplessivo, 2))
                        ApplicaStileGenerale(worksheet, rigaAttuale, k)

                        worksheet.Cell(rigaAttuale, k).Style.NumberFormat.Format = "0.00"

                        worksheet.Cell(rigaIntestazione, k).SetValue("TOTALE COMPLESSIVO")
                        ApplicaStileGenerale(worksheet, rigaIntestazione, k)
                        ApplicaStileIntestazione(worksheet, rigaIntestazione, k)
                        ApplicaStileIntestazione(worksheet, rigaAttuale, k)
                        ApplicaStileGenerale(worksheet, rigaHA, k)
                        ApplicaStileGenerale(worksheet, rigaTon, k)
                        ApplicaStileTipo(worksheet, rigaTipo, k)

                        rigaAttuale += 1
                        ultimaColonna = k
                    Next




                End If
                range.Add(rigaAttuale + 2)
                range.Add(ultimaColonna)
                rangeTabelle.Add(range)
            End If



            configurazioneGeneraleExcel(worksheet)

            Return True
        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Dim nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Ricerca_DT_SPV().ExportToExcel_Report_SintesiAzienda"
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

#End Region

#Region "Report 2 - Sintesi Coltura/Azienda"

    Private Function GenerazioneReport2(dataDal As String, dataAl As String, dataAl_cdg As String, costiPersonaleSeparatamente As Integer, ByRef objParametri_Server As AgronicaCoreParametri, aziendeVuote As List(Of String), includiAziendeFiglie As Integer, DettaglioSpecieVarieta As Integer, VediDettagliOperazioni As Integer, ByRef erroreDataTable As String, datiExcel As dataTableExcel, azienda As String, workbook As XLWorkbook, nomeFoglio As String, Optional rangeTabelle As List(Of List(Of Integer)) = Nothing) As Boolean

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim pivaReale As String = objImp.Leggi_PivaReale(azienda, objParametri_Server)

        Try
            Dim dtintestazione As New DataTable
            Dim dtfinale As New DataTable
            Dim dtReport3 As New DataTable
            Dim dwCdgBiz As New AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ
            Dim strErrore = dwCdgBiz.Ricerca_DT_SPV(dtintestazione, dtfinale, dtReport3, azienda, dataDal, dataAl, dataAl_cdg, 2, costiPersonaleSeparatamente, includiAziendeFiglie, objParametri_Server, DettaglioSpecieVarieta, VediDettagliOperazioni)
            If strErrore <> "" Then
                erroreDataTable &= creazioneStringaErroreDatatable(2, azienda, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, strErrore) & ","
            End If
            workbook.Worksheets.Add(nomeFoglio)

            Dim worksheet = workbook.Worksheet(nomeFoglio)


            If (Not ExportToExcel_Report_SintesiColturaAzienda(dtintestazione, dtfinale, objParametri_Server, azienda, worksheet, datiExcel, rangeTabelle)) Then
                aziendeVuote.Add(azienda & "?" & pivaReale & "?" & "coltura Per Azienda")
                workbook.Worksheets.Delete(nomeFoglio)
                Return False
            End If
        Catch ex As Exception
            erroreDataTable &= creazioneStringaErroreDatatable(2, pivaReale, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, ex.Message) & ","
        End Try
        Return True
    End Function

    Private Function ExportToExcel_Report_SintesiColturaAzienda(ByVal dtintestazione As DataTable,
                                          ByVal dt As DataTable,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByVal piva As String,
                                          ByRef worksheet As IXLWorksheet,
                                          ByVal datiExcel As dataTableExcel,
                                          Optional ByRef rangeTabelle As List(Of List(Of Integer)) = Nothing
                                          ) As Boolean

        If ((dt Is Nothing OrElse dt.Rows.Count = 0) OrElse
            (dtintestazione Is Nothing OrElse dtintestazione.Rows.Count = 0)) Then
            Return False
        End If
        If rangeTabelle Is Nothing Then
            rangeTabelle = New List(Of List(Of Integer))
        End If
        Try


            Dim result = From rows In dt.AsEnumerable()
                         Group rows By Key = New With {.Gruppo1 = rows("Des_Attivita_Gruppo1"), .Gruppo2 = rows("Des_Attivita_Gruppo2")} Into Group
                         Select Group

            Dim resultIntest = From rows In dtintestazione.AsEnumerable()
                               Group rows By Key = New With {.Gruppo1 = rows("Des_Attivita_Gruppo1"), .Gruppo2 = rows("Des_Attivita_Gruppo2")} Into Group
                               Select Group



            Dim rigaAttuale As Integer = 4
            Dim gruppo1 As String = ""
            Dim gruppo2 As String = ""
            Dim TotaleComplessivo As Double


            Dim DtImprese As DataTable = datiExcel.DtImprese
            Dim DtSpecie As DataTable = datiExcel.DtSpecie
            Dim DtCultivar As DataTable = datiExcel.DtCultivar
            Dim DtImputazioni As DataTable = datiExcel.DtImputazioni
            Dim DtMaterie As DataTable = datiExcel.DtMaterie




            Dim rigaIntestazione
            Dim rigaColtura
            Dim rigaProdotto
            Dim rigaHA
            Dim rigaHASommabili
            Dim rigaTon
            'riga tipo e' la riga dove c' e scritto se la tabella ha i valori in €, €/Ha o €/Ton
            Dim rigaTipo

            InserisciSpecchiettoExcel(piva, "Sintesi Per SPV Coltura", 1, 1, datiExcel, worksheet)

            Dim a = 1
            If a = 1 Then

                rigaIntestazione = rigaAttuale + 1
                rigaColtura = rigaIntestazione + 1
                rigaProdotto = rigaColtura + 1
                rigaHA = rigaProdotto + 1
                rigaHASommabili = rigaHA + 1
                rigaTon = rigaHASommabili + 1
                rigaTipo = rigaTon + 1

                Dim range As New List(Of Integer)
                range.Add(1)
                range.Add(1)

                Dim ultimaColonna As Integer

                'Testata della tabella
                worksheet.Cell(String.Format("A{0}", rigaIntestazione)).SetValue("Tipo")
                worksheet.Cell(String.Format("B{0}", rigaIntestazione)).SetValue("€")
                For colonna As Integer = 1 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                Next
                worksheet.Cell(String.Format("C{0}", rigaIntestazione)).SetValue("INTESTAZIONE")
                ApplicaStileIntestazione(worksheet, rigaIntestazione, 3)
                worksheet.Cell(String.Format("C{0}", rigaColtura)).SetValue("COLTURA")
                worksheet.Cell(String.Format("C{0}", rigaColtura)).Style.Font.Bold = True
                worksheet.Cell(String.Format("C{0}", rigaProdotto)).SetValue("PRODOTTO RACCOLTO")
                worksheet.Cell(String.Format("C{0}", rigaProdotto)).Style.Font.Bold = True

                For i As Integer = 3 To 5
                    ApplicaStileColturaProdotto(worksheet, rigaColtura, i)
                    ApplicaStileColturaProdotto(worksheet, rigaProdotto, i)
                Next
                'worksheet.Cell("C5").SetValue("TON")

                ApplicaStileGenerale(worksheet, rigaHA, 3)

                'worksheet.Cell(String.Format("C{0}", rigaTipo)).SetValue("€")
                ApplicaStileTipo(worksheet, rigaTipo, 3)

                Dim x As Integer = 1
                For colonna As Integer = 4 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileTipo(worksheet, rigaTipo, colonna)
                Next

                'intestazione
                rigaAttuale = rigaHA


                Dim Veg_Cod As Integer = 0
                Dim Cul_Cod As Integer = 0
                Dim Mat_Cod As Integer = 0
                Dim Descrizione As String = ""
                Dim Imputazione_Cod As Integer = 0
                Dim rag_soc As String = ""
                Dim nc As Integer = 0
                Dim dr_search As DataRow()
                Dim Arrayp As String()

                For nc = 6 To dtintestazione.Columns.Count - 1

                    Arrayp = Split(dtintestazione.Columns(nc).ColumnName, "_")

                    '=================================================================================================================
                    'Ragione Sociale
                    '-------------------------------------------------------------------------------------------------------------------------------
                    piva = Arrayp(0)

                    If Trim(piva) <> "" Then

                        'Inserimento SPV come prima azienda in Array Intestazione
                        dr_search = DtImprese.Select("Piva = '" & piva & "'")

                        If dr_search.Length <> 0 Then
                            'worksheet.Cell(rigaIntestazione, nc).Style.Font.Bold = True
                            worksheet.Cell(rigaIntestazione, nc).SetValue(dr_search(0).Item("Rag_Soc"))
                        End If

                    End If

                    '=================================================================================================================
                    'Specie Vegetale/Imputazione
                    '-------------------------------------------------------------------------------------------------------------------------------
                    Select Case Arrayp(1)

                        Case "A" 'Specie Vegetale

                            Veg_Cod = Arrayp(2)
                            Cul_Cod = Arrayp(3)
                            Mat_Cod = Arrayp(4)
                            Descrizione = ""

                            Select Case Veg_Cod
                                Case 0
                                    Descrizione = Str_TerrenoNudo
                                    worksheet.Cell(rigaColtura, nc).SetValue(Descrizione)
                                Case Else

                                    If Mat_Cod <> 0 Then

                                        'Inserimento Materia Prima
                                        dr_search = DtMaterie.Select("Mat_Cod = " & Mat_Cod & " ")
                                        If dr_search.Length <> 0 Then
                                            Descrizione = dr_search(0).Item("Mat_Des")
                                        End If
                                        worksheet.Cell(rigaProdotto, nc).SetValue(Descrizione)


                                    End If
                                    'Inserimento Specie
                                    dr_search = DtSpecie.Select("Veg_Cod = " & Veg_Cod & " ")

                                    If dr_search.Length <> 0 Then

                                    End If
                                    Descrizione = dr_search(0).Item("Veg_Des")
                                    If Cul_Cod <> 0 Then
                                        'Inserimento Cultivar
                                        dr_search = DtCultivar.Select("Cul_Cod = " & Cul_Cod & " ")

                                        If dr_search.Length <> 0 Then
                                            Descrizione = Descrizione & " - " & dr_search(0).Item("Cul_Des")
                                        End If

                                    End If
                                    worksheet.Cell(rigaColtura, nc).SetValue(Descrizione)


                            End Select




                        Case "I" 'Imputazione

                            Imputazione_Cod = Arrayp(2)
                            dr_search = DtImputazioni.Select("Imputazione_Cod = " & Imputazione_Cod & " ")

                            If dr_search.Length <> 0 Then
                                worksheet.Cell(rigaColtura, nc).SetValue(dr_search(0).Item("Imputazione_Nome"))
                                'workseet.Cell(rigaColtura, nc).Style.Font.Bold = True
                            End If
                    End Select
                    ApplicaStileColturaProdotto(worksheet, rigaColtura, nc)
                    ApplicaStileColturaProdotto(worksheet, rigaProdotto, nc)

                Next

                '=================================================================================================================


                For Each item In resultIntest
                    TotaleComplessivo = 0

                    Dim drItem As DataRow = item.FirstOrDefault()

                    If drItem Is Nothing Then
                        Continue For
                    End If

                    worksheet.Cell(rigaAttuale, "C").SetValue(drItem.Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "C").Style.Font.Bold = True
                    worksheet.Cell(rigaAttuale, "D").SetValue(drItem.Item("Lav_Des_Agenda"))

                    For colonna As Integer = 3 To drItem.ItemArray.Count() - 1
                        ApplicaStileHATon(worksheet, rigaAttuale, colonna)
                    Next

                    For y As Integer = 6 To drItem.ItemArray.Count() - 1
                        If IsNumeric(drItem.ItemArray(y)) Then
                            Dim value = Math.Round(CDbl(drItem.ItemArray(y)), 2)
                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)
                    Next

                    If rigaAttuale <> rigaHA Then
                        worksheet.Cell(rigaAttuale, (drItem.ItemArray.Count())).SetValue(TotaleComplessivo)
                        ApplicaStileGenerale(worksheet, rigaAttuale, (drItem.ItemArray.Count()))
                        worksheet.Cell(rigaAttuale, (drItem.ItemArray.Count())).Style.NumberFormat.Format = "0.00"
                        ApplicaStileHATon(worksheet, rigaAttuale, (drItem.ItemArray.Count()))
                    End If
                    rigaAttuale += 1
                Next

                rigaAttuale += 1

                Dim totaliPerAziendColtura As New List(Of Double)
                Dim inizializzato As Boolean
                inizializzato = False


                'righe della tabella
                For Each item In result
                    TotaleComplessivo = 0

                    Dim drItem As DataRow = item.FirstOrDefault()

                    If drItem Is Nothing Then
                        Continue For
                    End If

                    gruppo1 = If(drItem.Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, drItem.Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                    worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)

                    If gruppo1 <> String.Empty Then
                        ApplicaStileCosti(worksheet, rigaAttuale, 1)
                    End If

                    gruppo2 = If(drItem.Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, drItem.Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                    worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)
                    If gruppo2 <> String.Empty Then
                        ApplicaStileCosti(worksheet, rigaAttuale, 2)
                    End If
                    worksheet.Cell(rigaAttuale, "C").SetValue(drItem.Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "D").SetValue(drItem.Item("Lav_Des_Agenda"))
                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 5)
                    worksheet.Cell(rigaAttuale, 4).Style.Border.SetRightBorder(XLBorderStyleValues.Thick)


                    For y As Integer = 6 To drItem.ItemArray.Count() - 1
                        If IsNumeric(drItem.ItemArray(y)) Then
                            Dim value = Math.Round(CDbl(drItem.ItemArray(y)), 2)
                            TotaleComplessivo += value
                            If Not inizializzato Then
                                totaliPerAziendColtura.Add(value)
                            Else
                                totaliPerAziendColtura(y - 6) += value
                            End If
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                        Else
                            If Not inizializzato Then
                                totaliPerAziendColtura.Add(0)
                            End If
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"


                    Next


                    'totali di riga
                    Dim k As Integer = drItem.ItemArray.Count()
                    worksheet.Cell(rigaAttuale, k).SetValue(TotaleComplessivo)
                    worksheet.Cell(rigaAttuale, k).Style.NumberFormat.Format = "0.00"
                    worksheet.Cell(rigaIntestazione, k).SetValue("TOTALE COMPLESSIVO")
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, k)
                    ApplicaStileIntestazione(worksheet, rigaAttuale, k)
                    ApplicaStileGenerale(worksheet, rigaHA, k)
                    ApplicaStileGenerale(worksheet, rigaTon, k)
                    ApplicaStileTipo(worksheet, rigaTipo, k)

                    inizializzato = True
                    rigaAttuale += 1
                Next

                If totaliPerAziendColtura.Count > 0 Then
                    Dim totale = totaliPerAziendColtura.Sum
                    totaliPerAziendColtura.Add(totale)
                End If

                Dim j = 6
                For Each totale In totaliPerAziendColtura


                    For i As Integer = 0 To 2

                        ApplicaStileTipo(worksheet, rigaAttuale + i, 3)
                        ApplicaStileTipo(worksheet, rigaAttuale + i, 4)
                        ApplicaStileTipo(worksheet, rigaAttuale + i, 5)
                        ApplicaStileTipo(worksheet, rigaAttuale + i, j)
                        worksheet.Cell(rigaAttuale + i, j).Style.NumberFormat.Format = "0.00"
                    Next
                    worksheet.Cell(rigaAttuale, 3).SetValue("totale gestione")

                    worksheet.Cell(rigaAttuale + 1, 3).SetValue("€/ha")

                    worksheet.Cell(rigaAttuale + 2, 3).SetValue("€/ton")

                    worksheet.Cell(rigaAttuale, j).SetValue(totale)

                    Dim valoreHA = 0
                    If totale = totaliPerAziendColtura.LastOrDefault() Then
                        valoreHA = worksheet.Cell(rigaHASommabili, j).Value

                    Else
                        valoreHA = worksheet.Cell(rigaHA, j).Value
                    End If

                    Dim valoreTOT_HA = Math.Round(If(valoreHA <> 0, totale / valoreHA, 0), 2)
                    worksheet.Cell(rigaAttuale + 1, j).SetValue(valoreTOT_HA)

                    Dim valoreTOT_TON = Math.Round(If(worksheet.Cell(rigaTon, j).Value <> 0, totale / worksheet.Cell(rigaTon, j).Value, 0), 2)
                    worksheet.Cell(rigaAttuale + 2, j).SetValue(valoreTOT_TON)




                    j += 1
                    ultimaColonna = j
                Next

                range.Add(rigaAttuale + 3)
                range.Add(j - 1)
                rangeTabelle.Add(range)
            End If
            rigaAttuale += 7

            a = 2
            If a = 2 Then
                Dim range As New List(Of Integer)

                range.Add(rigaAttuale)
                range.Add(1)
                rigaIntestazione = rigaAttuale + 1
                rigaColtura = rigaIntestazione + 1
                rigaProdotto = rigaColtura + 1
                rigaHA = rigaProdotto + 1
                rigaHASommabili = rigaHA + 1
                rigaTon = rigaHA + 1
                rigaTipo = rigaTon + 1
                'Riga Ton e Riga HaSommabili hanno lo stesso valore intenzionalmente, visto che nella tabella  €/Ton, non c'e la riga HaSommabili
                Dim ultimaColonna As Integer
                'Testata della tabella
                worksheet.Cell(String.Format("A{0}", rigaIntestazione)).SetValue("Tipo")
                worksheet.Cell(String.Format("B{0}", rigaIntestazione)).SetValue("€/Ton")
                For colonna As Integer = 1 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                Next
                worksheet.Cell(String.Format("C{0}", rigaIntestazione)).SetValue("INTESTAZIONE")
                ApplicaStileIntestazione(worksheet, rigaIntestazione, 3)
                worksheet.Cell(String.Format("C{0}", rigaColtura)).SetValue("COLTURA")
                worksheet.Cell(String.Format("C{0}", rigaColtura)).Style.Font.Bold = True
                worksheet.Cell(String.Format("C{0}", rigaProdotto)).SetValue("PRODOTTO RACCOLTO")
                worksheet.Cell(String.Format("C{0}", rigaProdotto)).Style.Font.Bold = True

                For i As Integer = 3 To 5
                    ApplicaStileColturaProdotto(worksheet, rigaProdotto, i)
                    ApplicaStileColturaProdotto(worksheet, rigaColtura, i)
                Next
                ApplicaStileGenerale(worksheet, rigaHA, 3)

                'worksheet.Cell(String.Format("C{0}", rigaTipo)).SetValue("€/Ton")
                ApplicaStileTipo(worksheet, rigaTipo, 3)

                Dim x As Integer = 1
                For colonna As Integer = 4 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileGenerale(worksheet, rigaColtura, colonna)
                    ApplicaStileGenerale(worksheet, rigaHA, colonna)
                    ApplicaStileTipo(worksheet, rigaTipo, colonna)
                Next

                'intestazione
                rigaAttuale = rigaHA


                Dim Veg_Cod As Integer = 0
                Dim Cul_Cod As Integer = 0
                Dim Mat_Cod As Integer = 0
                Dim Descrizione As String = ""
                Dim Imputazione_Cod As Integer = 0
                Dim rag_soc As String = ""
                Dim nc As Integer = 0
                Dim dr_search As DataRow()
                Dim Arrayp As String()

                For nc = 6 To dtintestazione.Columns.Count - 1

                    Arrayp = Split(dtintestazione.Columns(nc).ColumnName, "_")

                    '=================================================================================================================
                    'Ragione Sociale
                    '-------------------------------------------------------------------------------------------------------------------------------
                    piva = Arrayp(0)

                    If Trim(piva) <> "" Then

                        'Inserimento SPV come prima azienda in Array Intestazione
                        dr_search = DtImprese.Select("Piva = '" & piva & "'")

                        If dr_search.Length <> 0 Then
                            'worksheet.Cell(rigaIntestazione, nc).Style.Font.Bold = True
                            worksheet.Cell(rigaIntestazione, nc).SetValue(dr_search(0).Item("Rag_Soc"))
                        End If

                    End If

                    '=================================================================================================================
                    'Specie Vegetale/Imputazione
                    '-------------------------------------------------------------------------------------------------------------------------------
                    Select Case Arrayp(1)

                        Case "A" 'Specie Vegetale

                            Veg_Cod = Arrayp(2)
                            Cul_Cod = Arrayp(3)
                            Mat_Cod = Arrayp(4)
                            Descrizione = ""

                            Select Case Veg_Cod
                                Case 0
                                    Descrizione = Str_TerrenoNudo
                                    worksheet.Cell(rigaColtura, nc).SetValue(Descrizione)
                                Case Else

                                    If Mat_Cod <> 0 Then

                                        'Inserimento Materia Prima
                                        dr_search = DtMaterie.Select("Mat_Cod = " & Mat_Cod & " ")
                                        If dr_search.Length <> 0 Then
                                            Descrizione = dr_search(0).Item("Mat_Des")
                                        End If
                                        worksheet.Cell(rigaProdotto, nc).SetValue(Descrizione)


                                    End If
                                    'Inserimento Specie
                                    dr_search = DtSpecie.Select("Veg_Cod = " & Veg_Cod & " ")

                                    If dr_search.Length <> 0 Then

                                    End If
                                    Descrizione = dr_search(0).Item("Veg_Des")
                                    If Cul_Cod <> 0 Then
                                        'Inserimento Cultivar
                                        dr_search = DtCultivar.Select("Cul_Cod = " & Cul_Cod & " ")

                                        If dr_search.Length <> 0 Then
                                            Descrizione = Descrizione & " - " & dr_search(0).Item("Cul_Des")
                                        End If

                                    End If
                                    worksheet.Cell(rigaColtura, nc).SetValue(Descrizione)


                            End Select




                        Case "I" 'Imputazione

                            Imputazione_Cod = Arrayp(2)
                            dr_search = DtImputazioni.Select("Imputazione_Cod = " & Imputazione_Cod & " ")

                            If dr_search.Length <> 0 Then
                                worksheet.Cell(rigaColtura, nc).SetValue(dr_search(0).Item("Imputazione_Nome"))
                                'workseet.Cell(rigaColtura, nc).Style.Font.Bold = True
                            End If
                    End Select
                    ApplicaStileColturaProdotto(worksheet, rigaColtura, nc)
                    ApplicaStileColturaProdotto(worksheet, rigaProdotto, nc)
                Next

                '=================================================================================================================


                For Each item In resultIntest
                    TotaleComplessivo = 0

                    If rigaAttuale = rigaHASommabili Then
                        rigaHASommabili = -1
                        Continue For
                    End If
                    worksheet.Cell(rigaAttuale, "C").SetValue(item.FirstOrDefault().Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "C").Style.Font.Bold = True
                    worksheet.Cell(rigaAttuale, "D").SetValue(item.FirstOrDefault().Item("Lav_Des_Agenda"))

                    For colonna As Integer = 3 To item.FirstOrDefault().ItemArray.Count()
                        ApplicaStileHATon(worksheet, rigaAttuale, colonna)
                    Next
                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)

                    For y As Integer = 6 To item.FirstOrDefault().ItemArray.Count() - 1
                        If IsNumeric(item.FirstOrDefault().ItemArray(y)) Then
                            Dim value = Math.Round(CDbl(item.FirstOrDefault().ItemArray(y)), 2)
                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)
                    Next

                    worksheet.Cell(rigaAttuale, (item.FirstOrDefault().ItemArray.Count())).SetValue(TotaleComplessivo)
                    ApplicaStileGenerale(worksheet, rigaAttuale, (item.FirstOrDefault().ItemArray.Count()))
                    worksheet.Cell(rigaAttuale, (item.FirstOrDefault().ItemArray.Count())).Style.NumberFormat.Format = "0.00"
                    rigaAttuale += 1
                Next

                rigaAttuale += 1

                Dim totaliPerAziendColtura As New List(Of Double)
                Dim inizializzato As Boolean
                inizializzato = False


                'righe della tabella
                For Each item In result
                    TotaleComplessivo = 0

                    gruppo1 = If(item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                    worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)

                    If gruppo1 <> String.Empty Then
                        ApplicaStileCosti(worksheet, rigaAttuale, 1)
                    End If

                    gruppo2 = If(item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                    worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)
                    If gruppo2 <> String.Empty Then
                        ApplicaStileCosti(worksheet, rigaAttuale, 2)
                    End If
                    worksheet.Cell(rigaAttuale, "C").SetValue(item.FirstOrDefault().Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "D").SetValue(item.FirstOrDefault().Item("Lav_Des_Agenda"))
                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 5)
                    worksheet.Cell(rigaAttuale, 4).Style.Border.SetRightBorder(XLBorderStyleValues.Thick)


                    For y As Integer = 6 To item.FirstOrDefault().ItemArray.Count() - 1
                        Dim valoreTon = CDbl(worksheet.Cell(rigaTon, y).Value)
                        If IsNumeric(item.FirstOrDefault().ItemArray(y)) AndAlso valoreTon <> 0 Then
                            Dim value = Math.Round(CDbl(item.FirstOrDefault().ItemArray(y) / valoreTon), 2)
                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"

                    Next
                    ultimaColonna = item.FirstOrDefault().ItemArray.Count()

                    'totali di riga
                    Dim k As Integer = item.FirstOrDefault().ItemArray.Count()
                    worksheet.Cell(rigaAttuale, k).SetValue(TotaleComplessivo)
                    worksheet.Cell(rigaAttuale, k).Style.NumberFormat.Format = "0.00"
                    worksheet.Cell(rigaIntestazione, k).SetValue("TOTALE COMPLESSIVO")
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, k)
                    ApplicaStileIntestazione(worksheet, rigaAttuale, k)
                    ApplicaStileGenerale(worksheet, rigaHA, k)
                    ApplicaStileGenerale(worksheet, rigaTon, k)
                    ApplicaStileTipo(worksheet, rigaTipo, k)

                    inizializzato = True
                    rigaAttuale += 1
                Next


                range.Add(rigaAttuale)
                range.Add(ultimaColonna)
                rangeTabelle.Add(range)
            End If
            rigaAttuale += 7

            a = 3
            If a = 3 Then
                Dim range As New List(Of Integer)

                range.Add(rigaAttuale)
                range.Add(1)
                rigaIntestazione = rigaAttuale + 1
                rigaColtura = rigaIntestazione + 1
                rigaProdotto = rigaColtura + 1
                rigaHA = rigaProdotto + 1
                rigaHASommabili = rigaHA + 1
                rigaTon = rigaHA + 1
                rigaTipo = rigaTon + 1

                Dim ultimaColonna As Integer
                'Testata della tabella
                worksheet.Cell(String.Format("A{0}", rigaIntestazione)).SetValue("Tipo")
                worksheet.Cell(String.Format("B{0}", rigaIntestazione)).SetValue("€/HA")
                For colonna As Integer = 1 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                Next
                worksheet.Cell(String.Format("C{0}", rigaIntestazione)).SetValue("INTESTAZIONE")
                ApplicaStileIntestazione(worksheet, rigaIntestazione, 3)
                worksheet.Cell(String.Format("C{0}", rigaColtura)).SetValue("COLTURA")
                worksheet.Cell(String.Format("C{0}", rigaColtura)).Style.Font.Bold = True
                worksheet.Cell(String.Format("C{0}", rigaProdotto)).SetValue("PRODOTTO RACCOLTO")
                worksheet.Cell(String.Format("C{0}", rigaProdotto)).Style.Font.Bold = True
                For i As Integer = 3 To 5
                    ApplicaStileColturaProdotto(worksheet, rigaProdotto, i)
                    ApplicaStileColturaProdotto(worksheet, rigaColtura, i)
                Next
                ApplicaStileGenerale(worksheet, rigaHA, 3)

                'worksheet.Cell(String.Format("C{0}", rigaTipo)).SetValue("€/HA")
                ApplicaStileTipo(worksheet, rigaTipo, 3)

                Dim x As Integer = 1
                For colonna As Integer = 4 To result.FirstOrDefault().FirstOrDefault().ItemArray.Count()
                    worksheet.Cell(rigaIntestazione, colonna).SetValue("")
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, colonna)
                    ApplicaStileGenerale(worksheet, rigaColtura, colonna)
                    ApplicaStileGenerale(worksheet, rigaHA, colonna)
                    ApplicaStileTipo(worksheet, rigaTipo, colonna)
                Next

                'intestazione
                rigaAttuale = rigaHA


                Dim Veg_Cod As Integer = 0
                Dim Cul_Cod As Integer = 0
                Dim Mat_Cod As Integer = 0
                Dim Descrizione As String = ""
                Dim Imputazione_Cod As Integer = 0
                Dim rag_soc As String = ""
                Dim nc As Integer = 0
                Dim dr_search As DataRow()
                Dim Arrayp As String()

                For nc = 6 To dtintestazione.Columns.Count - 1

                    Arrayp = Split(dtintestazione.Columns(nc).ColumnName, "_")

                    '=================================================================================================================
                    'Ragione Sociale
                    '-------------------------------------------------------------------------------------------------------------------------------
                    piva = Arrayp(0)

                    If Trim(piva) <> "" Then

                        'Inserimento SPV come prima azienda in Array Intestazione
                        dr_search = DtImprese.Select("Piva = '" & piva & "'")

                        If dr_search.Length <> 0 Then
                            'worksheet.Cell(rigaIntestazione, nc).Style.Font.Bold = True
                            worksheet.Cell(rigaIntestazione, nc).SetValue(dr_search(0).Item("Rag_Soc"))
                        End If

                    End If

                    '=================================================================================================================
                    'Specie Vegetale/Imputazione
                    '-------------------------------------------------------------------------------------------------------------------------------
                    Select Case Arrayp(1)

                        Case "A" 'Specie Vegetale

                            Veg_Cod = Arrayp(2)
                            Cul_Cod = Arrayp(3)
                            Mat_Cod = Arrayp(4)
                            Descrizione = ""

                            Select Case Veg_Cod
                                Case 0
                                    Descrizione = Str_TerrenoNudo
                                    worksheet.Cell(rigaColtura, nc).SetValue(Descrizione)
                                Case Else

                                    If Mat_Cod <> 0 Then

                                        'Inserimento Materia Prima
                                        dr_search = DtMaterie.Select("Mat_Cod = " & Mat_Cod & " ")
                                        If dr_search.Length <> 0 Then
                                            Descrizione = dr_search(0).Item("Mat_Des")
                                        End If
                                        worksheet.Cell(rigaProdotto, nc).SetValue(Descrizione)


                                    End If
                                    'Inserimento Specie
                                    dr_search = DtSpecie.Select("Veg_Cod = " & Veg_Cod & " ")

                                    If dr_search.Length <> 0 Then

                                    End If
                                    Descrizione = dr_search(0).Item("Veg_Des")
                                    If Cul_Cod <> 0 Then
                                        'Inserimento Cultivar
                                        dr_search = DtCultivar.Select("Cul_Cod = " & Cul_Cod & " ")

                                        If dr_search.Length <> 0 Then
                                            Descrizione = Descrizione & " - " & dr_search(0).Item("Cul_Des")
                                        End If

                                    End If
                                    worksheet.Cell(rigaColtura, nc).SetValue(Descrizione)


                            End Select




                        Case "I" 'Imputazione

                            Imputazione_Cod = Arrayp(2)
                            dr_search = DtImputazioni.Select("Imputazione_Cod = " & Imputazione_Cod & " ")

                            If dr_search.Length <> 0 Then
                                worksheet.Cell(rigaColtura, nc).SetValue(dr_search(0).Item("Imputazione_Nome"))
                                'workseet.Cell(rigaColtura, nc).Style.Font.Bold = True
                            End If
                    End Select
                    ApplicaStileColturaProdotto(worksheet, rigaColtura, nc)
                    ApplicaStileColturaProdotto(worksheet, rigaProdotto, nc)
                Next

                '=================================================================================================================


                For Each item In resultIntest
                    TotaleComplessivo = 0
                    If rigaAttuale = rigaHASommabili Then
                        rigaHASommabili = -1
                        Continue For
                    End If
                    worksheet.Cell(rigaAttuale, "C").SetValue(item.FirstOrDefault().Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "C").Style.Font.Bold = True
                    worksheet.Cell(rigaAttuale, "D").SetValue(item.FirstOrDefault().Item("Lav_Des_Agenda"))

                    For colonna As Integer = 3 To item.FirstOrDefault().ItemArray.Count()
                        ApplicaStileHATon(worksheet, rigaAttuale, colonna)
                    Next
                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)

                    For y As Integer = 6 To item.FirstOrDefault().ItemArray.Count() - 1
                        If IsNumeric(item.FirstOrDefault().ItemArray(y)) Then
                            Dim value = Math.Round(CDbl(item.FirstOrDefault().ItemArray(y)), 2)
                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)
                    Next

                    worksheet.Cell(rigaAttuale, (item.FirstOrDefault().ItemArray.Count())).SetValue(TotaleComplessivo)
                    ApplicaStileGenerale(worksheet, rigaAttuale, (item.FirstOrDefault().ItemArray.Count()))
                    worksheet.Cell(rigaAttuale, (item.FirstOrDefault().ItemArray.Count())).Style.NumberFormat.Format = "0.00"
                    rigaAttuale += 1
                Next

                rigaAttuale += 1

                Dim totaliPerAziendColtura As New List(Of Double)
                Dim inizializzato As Boolean
                inizializzato = False


                'righe della tabella
                For Each item In result
                    TotaleComplessivo = 0

                    gruppo1 = If(item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper() <> gruppo1, item.FirstOrDefault().Item("Des_Attivita_Gruppo1").ToString().ToUpper(), String.Empty)
                    worksheet.Cell(rigaAttuale, "A").SetValue(gruppo1)

                    If gruppo1 <> String.Empty Then
                        ApplicaStileCosti(worksheet, rigaAttuale, 1)
                    End If

                    gruppo2 = If(item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper() <> gruppo2, item.FirstOrDefault().Item("Des_Attivita_Gruppo2").ToString().ToUpper(), String.Empty)
                    worksheet.Cell(rigaAttuale, "B").SetValue(gruppo2)
                    If gruppo2 <> String.Empty Then
                        ApplicaStileCosti(worksheet, rigaAttuale, 2)
                    End If
                    worksheet.Cell(rigaAttuale, "C").SetValue(item.FirstOrDefault().Item("Attivita_Des"))
                    worksheet.Cell(rigaAttuale, "D").SetValue(item.FirstOrDefault().Item("Lav_Des_Agenda"))
                    ApplicaStileGenerale(worksheet, rigaAttuale, 3)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 4)
                    ApplicaStileGenerale(worksheet, rigaAttuale, 5)
                    worksheet.Cell(rigaAttuale, 4).Style.Border.SetRightBorder(XLBorderStyleValues.Thick)


                    For y As Integer = 6 To item.FirstOrDefault().ItemArray.Count() - 1
                        Dim valoreHA = CDbl(worksheet.Cell(rigaHA, y).Value)
                        If IsNumeric(item.FirstOrDefault().ItemArray(y)) AndAlso valoreHA <> 0 Then
                            Dim value = Math.Round(CDbl(item.FirstOrDefault().ItemArray(y) / valoreHA), 2)
                            TotaleComplessivo += value
                            worksheet.Cell(rigaAttuale, y).SetValue(value)
                        Else
                            worksheet.Cell(rigaAttuale, y).SetValue(0)
                        End If
                        ApplicaStileGenerale(worksheet, rigaAttuale, y)
                        worksheet.Cell(rigaAttuale, y).Style.NumberFormat.Format = "0.00"

                    Next
                    ultimaColonna = item.FirstOrDefault().ItemArray.Count()

                    'totali di riga
                    Dim k As Integer = item.FirstOrDefault().ItemArray.Count()
                    worksheet.Cell(rigaAttuale, k).SetValue(TotaleComplessivo)
                    worksheet.Cell(rigaAttuale, k).Style.NumberFormat.Format = "0.00"
                    worksheet.Cell(rigaIntestazione, k).SetValue("TOTALE COMPLESSIVO")
                    ApplicaStileIntestazione(worksheet, rigaIntestazione, k)
                    ApplicaStileIntestazione(worksheet, rigaAttuale, k)
                    ApplicaStileGenerale(worksheet, rigaHA, k)
                    ApplicaStileGenerale(worksheet, rigaTon, k)
                    ApplicaStileTipo(worksheet, rigaTipo, k)

                    inizializzato = True
                    rigaAttuale += 1
                Next


                range.Add(rigaAttuale)
                range.Add(ultimaColonna)
                rangeTabelle.Add(range)
            End If



            configurazioneGeneraleExcel(worksheet)





            Return True


        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Dim nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Ricerca_DT_SPV().ExportToExcel_Report_SintesiColturaAzienda"
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try




    End Function

#End Region

#Region "Report 3 - Coltura Campo"

    Private Function GenerazioneReport3(dataDal As String, dataAl As String, dataAl_cdg As String, costiPersonaleSeparatamente As Integer, ByRef objParametri_Server As AgronicaCoreParametri, aziendeVuote As List(Of String), includiAziendeFiglie As Integer, DettaglioSpecieVarieta As Integer, VediDettagliOperazioni As Integer, ByRef erroreDataTable As String, datiExcel As dataTableExcel, azienda As String, workbook As XLWorkbook, nomeFoglio As String, Optional rangeTabelle As List(Of List(Of Integer)) = Nothing) As Boolean

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim pivaReale As String = objImp.Leggi_PivaReale(azienda, objParametri_Server)

        Try
            Dim dtEnergia As New DataTable
            Dim dtMercato As New DataTable
            Dim dtReport3 As New DataTable
            Dim dwCdgBiz As New AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ
            Dim strErrore = dwCdgBiz.Ricerca_DT_SPV(dtEnergia, dtMercato, dtReport3, azienda, dataDal, dataAl, dataAl_cdg, 3, costiPersonaleSeparatamente, includiAziendeFiglie, objParametri_Server, DettaglioSpecieVarieta, VediDettagliOperazioni)
            If strErrore <> "" Then
                erroreDataTable += creazioneStringaErroreDatatable(2, azienda, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, strErrore) & ","
            End If
            workbook.Worksheets.Add(nomeFoglio)

            Dim worksheet = workbook.Worksheet(nomeFoglio)


            If (Not ExportToExcel_Report_SintesiProduzioneColturaCampo(dtEnergia, dtMercato, objParametri_Server, azienda, worksheet, datiExcel, rangeTabelle)) Then
                aziendeVuote.Add(azienda & "?" & pivaReale & "?" & "Sintesi Produzione Coltura Campo")
                workbook.Worksheets.Delete(nomeFoglio)
                Return False
            End If
        Catch ex As Exception
            erroreDataTable &= creazioneStringaErroreDatatable(3, pivaReale, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, ex.Message) & ","

        End Try
        Return True
    End Function

    Private Function ExportToExcel_Report_SintesiProduzioneColturaCampo(ByVal dtEnergia As DataTable,
                                          ByVal dtMercato As DataTable,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByVal piva As String,
                                          ByRef worksheet As IXLWorksheet,
                                          ByVal datiExcel As dataTableExcel,
                                          Optional ByRef rangeTabelle As List(Of List(Of Integer)) = Nothing
                                          ) As Boolean
        Try


            If (dtEnergia IsNot Nothing AndAlso dtEnergia.Rows.Count = 0) AndAlso (dtMercato IsNot Nothing AndAlso dtMercato.Rows.Count = 0) Then
                Return False

            End If
            If rangeTabelle Is Nothing Then
                rangeTabelle = New List(Of List(Of Integer))
            End If
            Dim range As New List(Of Integer)


            InserisciSpecchiettoExcel(piva, "Sintesi Produzione Coltura Campo", 1, 1, datiExcel, worksheet)


            Dim rigaDestinazione = 5
            Dim rigaIntestazione = rigaDestinazione + 1
            Dim rigaIntestazione2 = rigaIntestazione + 1
            Dim rigaAttuale = rigaIntestazione2 + 1



            Dim colonnaColtura = 1
            Dim colonnaProdottoRaccolto = colonnaColtura + 1
            Dim colonnaTipologiaRaccolo = colonnaProdottoRaccolto + 1
            Dim colonnaIntestazione = colonnaTipologiaRaccolo + 1
            Dim colonnaAppezzamento = colonnaIntestazione + 1
            Dim colonnaHA = colonnaAppezzamento + 1
            Dim colonnaHASommabile = colonnaHA + 1
            Dim colonnaTON = colonnaHASommabile + 1
            Dim colonnaTHA = colonnaTON + 1
            Dim colonnaBMP = colonnaTHA + 1
            Dim colonnaM = colonnaBMP + 1
            Dim colonnaEuro = colonnaM + 1
            Dim colonnaEuroTon = colonnaEuro + 1
            Dim colonnaEuroHA = colonnaEuroTon + 1

            Dim DtImprese As DataTable = datiExcel.DtImprese


            For colonna As Integer = colonnaColtura To colonnaEuroHA
                ApplicaStileIntestazione(worksheet, rigaDestinazione, colonna)
                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaIntestazione, colonna)
                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaIntestazione2, colonna)
            Next


            worksheet.Cell(rigaDestinazione, colonnaColtura).Value = "DESTINAZIONE"
            worksheet.Cell(rigaDestinazione, colonnaIntestazione).Value = "Energia"
            worksheet.Cell(rigaDestinazione, colonnaIntestazione).Style.Font.Bold = False

            worksheet.Cell(rigaIntestazione2, colonnaColtura).Value = "COLTURA"
            worksheet.Cell(rigaIntestazione2, colonnaProdottoRaccolto).Value = "PRODOTTO RACCOLTO"
            worksheet.Cell(rigaIntestazione2, colonnaTipologiaRaccolo).Value = "PRINCIPALE / SECONDARIO"
            worksheet.Cell(rigaIntestazione2, colonnaIntestazione).Value = "INTESTAZIONE"
            worksheet.Cell(rigaIntestazione2, colonnaAppezzamento).Value = "APPEZZAMENTO"
            worksheet.Cell(rigaIntestazione, colonnaHA).Value = "Superficie"
            worksheet.Cell(rigaIntestazione2, colonnaHA).Value = "HA"
            worksheet.Cell(rigaIntestazione, colonnaHASommabile).Value = "Sup Sommabile"
            worksheet.Cell(rigaIntestazione2, colonnaHASommabile).Value = "HA"
            worksheet.Cell(rigaIntestazione, colonnaTON).Value = "Produzione"
            worksheet.Cell(rigaIntestazione2, colonnaTON).Value = "TON"
            worksheet.Cell(rigaIntestazione2, colonnaTHA).Value = "T/HA"
            worksheet.Cell(rigaIntestazione, colonnaBMP).Value = ("    " & "BMP" & "    ")
            worksheet.Cell(rigaIntestazione2, colonnaBMP).Value = ("    " & "M3/T" & "    ")
            worksheet.Cell(rigaIntestazione2, colonnaM).Value = ("    " & "M3" & "    ")
            worksheet.Cell(rigaIntestazione2, colonnaEuro).Value = ("€")
            worksheet.Cell(rigaIntestazione2, colonnaEuroTon).Value = ("€/Ton")
            worksheet.Cell(rigaIntestazione2, colonnaEuroHA).Value = ("€/HA")


            If (dtEnergia IsNot Nothing AndAlso dtEnergia.Rows.Count > 0) Then
                range.Add(1)
                range.Add(colonnaColtura)
                worksheet.Column(colonnaBMP).Width = 20




                Dim ultimaColtura As String = ""
                Dim ultimoProdottoRaccolto As String = ""
                Dim supTot As Double = 0
                Dim supTotSommabile As Double = 0
                Dim prodTot As Double = 0
                Dim m3Tot As Double = 0
                Dim euroTot As Double = 0


                For Each row As DataRow In dtEnergia.Rows



                    Dim coltura = row.Item("Coltura_Des")
                    Dim prodottoRaccolto = If(IsNothing(row.Item("Prodotto_Raccolto")), "", row.Item("Prodotto_Raccolto"))
                    'Dim tipologiaRaccolto = If(IsNothing(row.Item("tipologia_Raccolto")), "", row.Item("tipologia_Raccolto"))
                    Dim tipologiaRaccolto = If(CDbl(row.Item("Priorita")) = 0, "Principale", "Secondario")


                    Dim nomeAzienda = row.Item("Intestazione")

                    'serve a non scrivere su ogni riga la stessa cultura
                    If ultimaColtura <> coltura OrElse ultimoProdottoRaccolto <> prodottoRaccolto Then
                        'parte di sommatoria nel cambio di coltura
                        If ultimaColtura <> "" OrElse ultimoProdottoRaccolto <> "" Then
                            For colonna As Integer = colonnaIntestazione To colonnaEuroHA
                                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaAttuale, colonna)
                                ApplicaStileGenerale(worksheet, rigaAttuale, colonna)
                            Next
                            worksheet.Cell(rigaAttuale, colonnaIntestazione).Value = "Tot " & ultimaColtura & If((ultimoProdottoRaccolto) = "", "", " - " & ultimoProdottoRaccolto)

                            worksheet.Cell(rigaAttuale, colonnaHA).Value = supTot
                            worksheet.Cell(rigaAttuale, colonnaHA).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaHASommabile).Value = supTotSommabile
                            worksheet.Cell(rigaAttuale, colonnaHASommabile).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaTON).Value = prodTot
                            worksheet.Cell(rigaAttuale, colonnaTON).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaM).Value = m3Tot
                            worksheet.Cell(rigaAttuale, colonnaM).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaTHA).Value = (prodTot / supTot)
                            worksheet.Cell(rigaAttuale, colonnaTHA).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaEuro).Value = euroTot
                            worksheet.Cell(rigaAttuale, colonnaEuro).Style.NumberFormat.Format = "0.00"

                            worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(prodTot = 0, 0, (euroTot / prodTot))
                            worksheet.Cell(rigaAttuale, colonnaEuroTon).Style.NumberFormat.Format = "0.00"

                            worksheet.Cell(rigaAttuale, colonnaEuroHA).Value = If(supTot = 0, 0, (euroTot / supTot))
                            worksheet.Cell(rigaAttuale, colonnaEuroHA).Style.NumberFormat.Format = "0.00"

                            supTot = 0
                            supTotSommabile = 0
                            prodTot = 0
                            m3Tot = 0
                            euroTot = 0

                            rigaAttuale += 1


                        End If
                        worksheet.Cell(rigaAttuale, colonnaColtura).Value = coltura
                        ApplicaStileColturaProdotto(worksheet, rigaAttuale, colonnaColtura)

                        If prodottoRaccolto <> Nothing Then
                            worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                            ApplicaStileColturaProdotto(worksheet, rigaAttuale, colonnaProdottoRaccolto)

                            worksheet.Cell(rigaAttuale, colonnaTipologiaRaccolo).Value = tipologiaRaccolto
                            ApplicaStileColturaProdotto(worksheet, rigaAttuale, colonnaTipologiaRaccolo)
                        End If

                        ultimaColtura = coltura
                        ultimoProdottoRaccolto = prodottoRaccolto
                    End If

                    worksheet.Cell(rigaAttuale, colonnaIntestazione).Value = nomeAzienda
                    'worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                    worksheet.Cell(rigaAttuale, colonnaAppezzamento).Value = row.Item("Appezzamento")

                    Dim superfice As Double = Math.Round(CDbl(row.Item("Superficie")), 3)
                    worksheet.Cell(rigaAttuale, colonnaHA).Value = superfice
                    worksheet.Cell(rigaAttuale, colonnaHA).Style.NumberFormat.Format = "0.000"
                    supTot += superfice

                    Dim supSommabile As Double = Math.Round(CDbl(row.Item("Sommabile")), 3)
                    worksheet.Cell(rigaAttuale, colonnaHASommabile).Value = supSommabile
                    worksheet.Cell(rigaAttuale, colonnaHASommabile).Style.NumberFormat.Format = "0.000"
                    supTotSommabile += supSommabile


                    If superfice <> supSommabile Then
                        Dim k = 5
                    End If

                    Dim produzione As Double = Math.Round(CDbl(row.Item("Produzione")), 3)

                    worksheet.Cell(rigaAttuale, colonnaTON).Value = produzione
                    worksheet.Cell(rigaAttuale, colonnaTON).Style.NumberFormat.Format = "0.000"
                    prodTot += produzione

                    worksheet.Cell(rigaAttuale, colonnaTHA).Value = Math.Round(CDbl(row.Item("T_Ha")), 3)
                    worksheet.Cell(rigaAttuale, colonnaTHA).Style.NumberFormat.Format = "0.000"

                    Dim m3t As Double = Math.Round(CDbl(row.Item("M3_T")), 3)
                    Dim m3 = Math.Round(CDbl(row.Item("M3")), 3)
                    m3Tot += m3

                    worksheet.Cell(rigaAttuale, colonnaM).Value = m3
                    worksheet.Cell(rigaAttuale, colonnaM).Style.NumberFormat.Format = "0.000"
                    worksheet.Cell(rigaAttuale, colonnaBMP).Value = m3t
                    worksheet.Cell(rigaAttuale, colonnaBMP).Style.NumberFormat.Format = "0.000"

                    Dim euro As Double = Math.Round(CDbl(row.Item("Costo")), 2)
                    euroTot += euro

                    Dim euroTon As Double = Math.Round(CDbl(row.Item("Costo_Ton")), 2)
                    Dim euroHa As Double = Math.Round(CDbl(row.Item("Costo_Ha")), 2)

                    worksheet.Cell(rigaAttuale, colonnaEuro).Value = euro
                    worksheet.Cell(rigaAttuale, colonnaEuro).Style.NumberFormat.Format = "0.00"

                    worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = euroTon
                    worksheet.Cell(rigaAttuale, colonnaEuroTon).Style.NumberFormat.Format = "0.00"

                    worksheet.Cell(rigaAttuale, colonnaEuroHA).Value = euroHa
                    worksheet.Cell(rigaAttuale, colonnaEuroHA).Style.NumberFormat.Format = "0.00"



                    For colonna As Integer = colonnaIntestazione To colonnaEuroHA
                        ApplicaStileGenerale(worksheet, rigaAttuale, colonna)
                    Next
                    rigaAttuale += 1
                Next

                For colonna As Integer = colonnaIntestazione To colonnaEuroHA
                    ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaAttuale, colonna)
                    ApplicaStileGenerale(worksheet, rigaAttuale, colonna)
                    worksheet.Cell(rigaAttuale, colonnaIntestazione).Value = "Tot " & ultimaColtura & If((ultimoProdottoRaccolto) = "", "", " - " & ultimoProdottoRaccolto)

                Next

                For colonna As Integer = colonnaHA To colonnaEuroHA
                    worksheet.Cell(rigaAttuale, colonna).Style.NumberFormat.Format = "0.000"
                Next

                worksheet.Cell(rigaAttuale, colonnaHA).Value = supTot
                worksheet.Cell(rigaAttuale, colonnaHASommabile).Value = supTotSommabile
                worksheet.Cell(rigaAttuale, colonnaTON).Value = prodTot
                worksheet.Cell(rigaAttuale, colonnaM).Value = m3Tot
                worksheet.Cell(rigaAttuale, colonnaTHA).Value = If(supTot = 0, 0, (prodTot / supTot))
                worksheet.Cell(rigaAttuale, colonnaEuro).Value = euroTot
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(prodTot = 0, 0, (euroTot / prodTot))
                worksheet.Cell(rigaAttuale, colonnaEuroHA).Value = If(supTot = 0, 0, (euroTot / supTot))
                worksheet.Cell(rigaAttuale, colonnaEuro).Style.NumberFormat.Format = "0.00"
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Style.NumberFormat.Format = "0.00"
                worksheet.Cell(rigaAttuale, colonnaEuroHA).Style.NumberFormat.Format = "0.00"

                range.Add(rigaAttuale)
                range.Add(colonnaEuroHA)
                rangeTabelle.Add(range)

                rigaAttuale += 1

            End If


            rigaDestinazione = 5
            rigaIntestazione = rigaDestinazione + 1
            rigaIntestazione2 = rigaIntestazione + 1
            rigaAttuale = rigaIntestazione2 + 1

            colonnaColtura = colonnaEuroHA + 3
            colonnaProdottoRaccolto = colonnaColtura + 1
            colonnaTipologiaRaccolo = colonnaProdottoRaccolto + 1
            colonnaIntestazione = colonnaTipologiaRaccolo + 1
            colonnaAppezzamento = colonnaIntestazione + 1
            colonnaHA = colonnaAppezzamento + 1
            colonnaHASommabile = colonnaHA + 1
            colonnaTON = colonnaHASommabile + 1
            colonnaTHA = colonnaTON + 1
            colonnaEuro = colonnaTHA + 1
            colonnaEuroTon = colonnaEuro + 1
            colonnaEuroHA = colonnaEuroTon + 1





            For colonna As Integer = colonnaColtura To colonnaEuroHA
                ApplicaStileIntestazione(worksheet, rigaDestinazione, colonna)
                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaIntestazione, colonna)
                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaIntestazione2, colonna)
            Next


            worksheet.Cell(rigaDestinazione, colonnaColtura).Value = "DESTINAZIONE"
            worksheet.Cell(rigaDestinazione, colonnaIntestazione).Value = "Mercato"
            worksheet.Cell(rigaDestinazione, colonnaIntestazione).Style.Font.Bold = False
            worksheet.Cell(rigaIntestazione2, colonnaTipologiaRaccolo).Value = "PRINCIPALE / SECONDARIO"
            worksheet.Cell(rigaIntestazione2, colonnaColtura).Value = "COLTURA"
            worksheet.Cell(rigaIntestazione2, colonnaProdottoRaccolto).Value = "PRODOTTO RACCOLTO"
            worksheet.Cell(rigaIntestazione2, colonnaIntestazione).Value = "INTESTAZIONE"
            worksheet.Cell(rigaIntestazione2, colonnaAppezzamento).Value = "APPEZZAMENTO"
            worksheet.Cell(rigaIntestazione, colonnaHA).Value = "Superficie"
            worksheet.Cell(rigaIntestazione2, colonnaHA).Value = "HA"
            worksheet.Cell(rigaIntestazione, colonnaHASommabile).Value = "Sup Sommabile"
            worksheet.Cell(rigaIntestazione2, colonnaHASommabile).Value = "HA"
            worksheet.Cell(rigaIntestazione, colonnaTON).Value = "Produzione"
            worksheet.Cell(rigaIntestazione2, colonnaTON).Value = "TON"
            worksheet.Cell(rigaIntestazione2, colonnaTHA).Value = "T/HA"
            worksheet.Cell(rigaIntestazione2, colonnaEuro).Value = ("    " & "€" & "    ")
            worksheet.Cell(rigaIntestazione2, colonnaEuroTon).Value = ("    " & "€/Ton" & "    ")
            worksheet.Cell(rigaIntestazione2, colonnaEuroHA).Value = ("    " & "€/HA" & "    ")

            If dtMercato IsNot Nothing AndAlso dtMercato.Rows.Count > 0 Then


                Dim range2 As New List(Of Integer)


                Dim ultimaColtura = ""
                Dim ultimoProdottoRaccolto = ""
                Dim supTot As Double = 0
                Dim supTotSommabile As Double = 0
                Dim prodTot As Double = 0
                Dim euroTot As Double = 0
                range2.Add(1)
                range2.Add(colonnaColtura)



                For Each row As DataRow In dtMercato.Rows



                    Dim coltura = row.Item("Coltura_Des")
                    Dim prodottoRaccolto = If(IsNothing(row.Item("Prodotto_Raccolto")), "", row.Item("Prodotto_Raccolto"))
                    'Dim tipologiaRaccolto = If(IsNothing(row.Item("tipologia_Raccolto")), "", row.Item("tipologia_Raccolto"))
                    Dim tipologiaRaccolto = If(CDbl(row.Item("Priorita")) = 0, "Principale", "Secondario")

                    Dim nomeAzienda = row.Item("Intestazione")

                    'serve a non scrivere su ogni riga la stessa cultura
                    If ultimaColtura <> coltura OrElse ultimoProdottoRaccolto <> prodottoRaccolto Then
                        'parte di sommatoria nel cambio di coltura
                        If ultimaColtura <> "" OrElse ultimoProdottoRaccolto <> "" Then
                            For colonna As Integer = colonnaIntestazione To colonnaEuroHA
                                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaAttuale, colonna)
                                ApplicaStileGenerale(worksheet, rigaAttuale, colonna)
                            Next
                            worksheet.Cell(rigaAttuale, colonnaIntestazione).Value = "Tot " & ultimaColtura & If((ultimoProdottoRaccolto) = "", "", " - " & ultimoProdottoRaccolto)

                            worksheet.Cell(rigaAttuale, colonnaHA).Value = supTot
                            worksheet.Cell(rigaAttuale, colonnaHA).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaHASommabile).Value = supTotSommabile
                            worksheet.Cell(rigaAttuale, colonnaHASommabile).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaTON).Value = prodTot
                            worksheet.Cell(rigaAttuale, colonnaTON).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaTHA).Value = If(supTot = 0, 0, (prodTot / supTot))
                            worksheet.Cell(rigaAttuale, colonnaTHA).Style.NumberFormat.Format = "0.000"

                            worksheet.Cell(rigaAttuale, colonnaEuro).Value = euroTot
                            worksheet.Cell(rigaAttuale, colonnaEuro).Style.NumberFormat.Format = "0.00"

                            worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(prodTot = 0, 0, (euroTot / prodTot))
                            worksheet.Cell(rigaAttuale, colonnaEuroTon).Style.NumberFormat.Format = "0.00"

                            worksheet.Cell(rigaAttuale, colonnaEuroHA).Value = If(supTot = 0, 0, (euroTot / supTot))
                            worksheet.Cell(rigaAttuale, colonnaEuroHA).Style.NumberFormat.Format = "0.00"


                            supTot = 0
                            supTotSommabile = 0
                            prodTot = 0
                            euroTot = 0

                            rigaAttuale += 1


                        End If

                        worksheet.Cell(rigaAttuale, colonnaColtura).Value = coltura
                        ApplicaStileColturaProdotto(worksheet, rigaAttuale, colonnaColtura)

                        If prodottoRaccolto <> Nothing Then
                            worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                            ApplicaStileColturaProdotto(worksheet, rigaAttuale, colonnaProdottoRaccolto)

                            worksheet.Cell(rigaAttuale, colonnaTipologiaRaccolo).Value = tipologiaRaccolto
                            ApplicaStileColturaProdotto(worksheet, rigaAttuale, colonnaTipologiaRaccolo)
                        End If

                        'worksheet.Cell(rigaAttuale, colonnaColtura).Style.Font.Bold = True
                        ultimaColtura = coltura
                        ultimoProdottoRaccolto = prodottoRaccolto



                    End If

                    worksheet.Cell(rigaAttuale, colonnaIntestazione).Value = nomeAzienda

                    worksheet.Cell(rigaAttuale, colonnaAppezzamento).Value = row.Item("Appezzamento")

                    Dim superfice As Double = Math.Round(CDbl(row.Item("Superficie")), 3)
                    worksheet.Cell(rigaAttuale, colonnaHA).Value = superfice
                    worksheet.Cell(rigaAttuale, colonnaHA).Style.NumberFormat.Format = "0.000"
                    supTot += superfice


                    'Dim supSommabile As Double = Math.Round(CDbl(row.Item("supSommabile")), 3)
                    Dim supSommabile As Double = Math.Round(CDbl(row.Item("Sommabile")), 3)
                    worksheet.Cell(rigaAttuale, colonnaHASommabile).Value = supSommabile
                    worksheet.Cell(rigaAttuale, colonnaHASommabile).Style.NumberFormat.Format = "0.000"
                    supTotSommabile += supSommabile

                    Dim produzione As Double = Math.Round(CDbl(row.Item("Produzione")), 3)
                    worksheet.Cell(rigaAttuale, colonnaTON).Value = produzione
                    worksheet.Cell(rigaAttuale, colonnaTON).Style.NumberFormat.Format = "0.000"
                    prodTot += produzione

                    worksheet.Cell(rigaAttuale, colonnaTHA).Value = produzione / superfice
                    worksheet.Cell(rigaAttuale, colonnaTHA).Style.NumberFormat.Format = "0.000"

                    Dim euro As Double = Math.Round(CDbl(row.Item("Costo")), 2)
                    euroTot += euro


                    Dim euroTon As Double = Math.Round(CDbl(row.Item("Costo_Ton")), 2)
                    Dim euroHa As Double = Math.Round(CDbl(row.Item("Costo_Ha")), 2)

                    worksheet.Cell(rigaAttuale, colonnaEuro).Value = euro
                    worksheet.Cell(rigaAttuale, colonnaEuro).Style.NumberFormat.Format = "0.00"

                    worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = euroTon
                    worksheet.Cell(rigaAttuale, colonnaEuroTon).Style.NumberFormat.Format = "0.00"

                    worksheet.Cell(rigaAttuale, colonnaEuroHA).Value = euroHa
                    worksheet.Cell(rigaAttuale, colonnaEuroHA).Style.NumberFormat.Format = "0.00"


                    For colonna As Integer = colonnaIntestazione To colonnaEuroHA
                        ApplicaStileGenerale(worksheet, rigaAttuale, colonna)
                    Next
                    rigaAttuale += 1
                Next

                For colonna As Integer = colonnaIntestazione To colonnaEuroHA
                    ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaAttuale, colonna)
                    ApplicaStileGenerale(worksheet, rigaAttuale, colonna)
                    worksheet.Cell(rigaAttuale, colonnaIntestazione).Value = "Tot " & ultimaColtura & If((ultimoProdottoRaccolto) = "", "", " - " & ultimoProdottoRaccolto)

                Next

                For colonna As Integer = colonnaHA To colonnaEuroHA
                    worksheet.Cell(rigaAttuale, colonna).Style.NumberFormat.Format = "0.000"
                Next
                worksheet.Cell(rigaAttuale, colonnaHA).Value = supTot
                worksheet.Cell(rigaAttuale, colonnaHASommabile).Value = supTotSommabile
                worksheet.Cell(rigaAttuale, colonnaTON).Value = prodTot
                worksheet.Cell(rigaAttuale, colonnaTHA).Value = prodTot / supTot
                worksheet.Cell(rigaAttuale, colonnaEuro).Value = euroTot
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(prodTot = 0, 0, (euroTot / prodTot))
                worksheet.Cell(rigaAttuale, colonnaEuroHA).Value = If(supTot = 0, 0, (euroTot / supTot))
                worksheet.Cell(rigaAttuale, colonnaEuro).Style.NumberFormat.Format = "0.00"
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Style.NumberFormat.Format = "0.00"
                worksheet.Cell(rigaAttuale, colonnaEuroHA).Style.NumberFormat.Format = "0.00"

                range2.Add(rigaAttuale)
                range2.Add(colonnaEuroHA)
                rangeTabelle.Add(range2)

                rigaAttuale += 1



            End If



            configurazioneGeneraleExcel(worksheet)
            Return True
        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Dim nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Ricerca_DT_SPV().ExportToExcel_Report_SintesiProduzioneColturaCampo"
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Function

#End Region

#Region "Report 4 - Piano Colturale"

    Private Function GenerazioneReport4(dataDal As String, dataAl As String, dataAl_cdg As String, costiPersonaleSeparatamente As Integer, ByRef objParametri_Server As AgronicaCoreParametri, aziendeVuote As List(Of String), includiAziendeFiglie As Integer, DettaglioSpecieVarieta As Integer, VediDettagliOperazioni As Integer, ByRef erroreDataTable As String, datiExcel As dataTableExcel, azienda As String, workbook As XLWorkbook, nomeFoglio As String, Optional rangeTabelle As List(Of List(Of Integer)) = Nothing) As Boolean

        Dim objImp As New AgronicaCoreAnagrafeDAL.Imprese_Read
        Dim pivaReale As String = objImp.Leggi_PivaReale(azienda, objParametri_Server)

        Try
            Dim dtEnergia As New DataTable
            Dim dtMercato As New DataTable
            Dim dtSAU As New DataTable
            Dim dwCdgBiz As New AgronicaCoreContabBIZ.DW_CDG_Costi_Ricavi_BIZ
            Dim strErrore = dwCdgBiz.Ricerca_DT_SPV(dtEnergia, dtMercato, dtSAU, azienda, dataDal, dataAl, dataAl_cdg, 4, costiPersonaleSeparatamente, includiAziendeFiglie, objParametri_Server, DettaglioSpecieVarieta, VediDettagliOperazioni)
            If strErrore <> "" Then
                erroreDataTable &= creazioneStringaErroreDatatable(2, azienda, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, strErrore) & ","
            End If
            workbook.Worksheets.Add(nomeFoglio)
            Dim worksheet = workbook.Worksheet(nomeFoglio)

            If (Not ExportToExcel_Report_PianoColturale(dtEnergia, dtMercato, dtSAU, objParametri_Server, azienda, worksheet, datiExcel, rangeTabelle)) Then
                aziendeVuote.Add(azienda & "?" & pivaReale & "?" & "Piano Colturale")
                workbook.Worksheets.Delete(nomeFoglio)
                Return False
            End If
        Catch ex As Exception
            erroreDataTable &= creazioneStringaErroreDatatable(4, pivaReale, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, ex.Message) & ","
        End Try
        Return True
    End Function

    Private Function ExportToExcel_Report_PianoColturale(ByVal dtEnergia As DataTable,
                                          ByVal dtMercato As DataTable,
                                          ByVal dtSAU As DataTable,
                                          ByRef objParametri_Server As AgronicaCoreParametri,
                                          ByVal piva As String,
                                          ByRef worksheet As IXLWorksheet,
                                          ByVal datiExcel As dataTableExcel,
                                          Optional ByRef rangeTabelle As List(Of List(Of Integer)) = Nothing
                                          ) As Boolean
        Try

            If (dtMercato IsNot Nothing AndAlso dtMercato.Rows.Count = 0) AndAlso (dtEnergia IsNot Nothing AndAlso dtEnergia.Rows.Count = 0) Then
                Return False
            End If
            If rangeTabelle Is Nothing Then
                rangeTabelle = New List(Of List(Of Integer))
            End If

            InserisciSpecchiettoExcel(piva, "Piano Colturale", 1, 1, datiExcel, worksheet)

            Dim range As New List(Of Integer)
            Dim rigaAttuale = 5

            'prima tabella
            range.Add(1)
            range.Add(1)

            Dim colonnaAzienda = 1
            Dim colonnaAppezzamento = colonnaAzienda + 1
            Dim colonnaSAU = colonnaAppezzamento + 1
            Dim colonnaSAUSommabile = colonnaSAU + 1

            Dim colonnaPrimoRaccolto = colonnaSAUSommabile + 1
            Dim colonnaProdottoRaccolto = colonnaPrimoRaccolto + 1
            Dim colonnaDestionazione = colonnaProdottoRaccolto + 1

            Dim colonnaSecondoRaccolto = colonnaDestionazione + 1
            Dim colonnaSecondoProdottoRaccolto = colonnaSecondoRaccolto + 1
            Dim colonnaDestionazioneSecondo = colonnaSecondoProdottoRaccolto + 1



            worksheet.Cell(rigaAttuale, colonnaAzienda).Value = "Azienda"
            worksheet.Cell(rigaAttuale, colonnaAppezzamento).Value = "Appezzamento"
            worksheet.Cell(rigaAttuale, colonnaSAU).Value = "SAU"
            worksheet.Cell(rigaAttuale, colonnaSAUSommabile).Value = "SAU Sommabile"
            worksheet.Cell(rigaAttuale, colonnaPrimoRaccolto).Value = "Primo raccolto"
            worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = "Prodotto raccolto"
            worksheet.Cell(rigaAttuale, colonnaDestionazione).Value = "Destinazione"
            worksheet.Cell(rigaAttuale, colonnaSecondoRaccolto).Value = "Secondo raccolto"
            worksheet.Cell(rigaAttuale, colonnaSecondoProdottoRaccolto).Value = "Prodotto raccolto"
            worksheet.Cell(rigaAttuale, colonnaDestionazioneSecondo).Value = "Destinazione"

            For i As Integer = colonnaAzienda To colonnaDestionazioneSecondo
                ApplicaStileIntestazione(worksheet, rigaAttuale, i)
            Next
            rigaAttuale += 1

            Dim sauSommabileTot As Double = 0
            For Each row In dtSAU.Rows

                worksheet.Cell(rigaAttuale, colonnaAzienda).Value = row.Item("Intestazione")
                worksheet.Cell(rigaAttuale, colonnaAppezzamento).Value = row.Item("Appezzamento")

                Dim sau = Math.Round(row.Item("SAU"), 3)
                worksheet.Cell(rigaAttuale, colonnaSAU).Value = sau
                worksheet.Cell(rigaAttuale, colonnaSAU).Style.NumberFormat.Format = "0.00"

                Dim sauSommabile = Math.Round(row.Item("SAU_Sommabile"), 3)
                sauSommabileTot += sauSommabile
                worksheet.Cell(rigaAttuale, colonnaSAUSommabile).Value = sauSommabile
                worksheet.Cell(rigaAttuale, colonnaSAUSommabile).Style.NumberFormat.Format = "0.00"

                worksheet.Cell(rigaAttuale, colonnaPrimoRaccolto).Value = row.Item("Primo_raccolto")
                worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = row.Item("Prodotto_Raccolto1")
                worksheet.Cell(rigaAttuale, colonnaDestionazione).Value = row.Item("Destinazione1")
                worksheet.Cell(rigaAttuale, colonnaSecondoRaccolto).Value = row.Item("Secondo_raccolto")
                worksheet.Cell(rigaAttuale, colonnaSecondoProdottoRaccolto).Value = row.Item("Prodotto_Raccolto2")
                worksheet.Cell(rigaAttuale, colonnaDestionazioneSecondo).Value = row.Item("Destinazione2")



                For i As Integer = colonnaAzienda To colonnaDestionazioneSecondo
                    ApplicaStileGenerale(worksheet, rigaAttuale, i)
                Next
                rigaAttuale += 1

            Next

            For i As Integer = colonnaAzienda To colonnaDestionazioneSecondo
                ApplicaStileTipo(worksheet, rigaAttuale, i)
            Next
            worksheet.Cell(rigaAttuale, colonnaAzienda).Value = "Totale"
            worksheet.Cell(rigaAttuale, colonnaSAUSommabile).Value = sauSommabileTot
            worksheet.Cell(rigaAttuale, colonnaSAUSommabile).Style.NumberFormat.Format = "0.00"

            range.Add(rigaAttuale)
            range.Add(colonnaDestionazioneSecondo)
            rangeTabelle.Add(range.ToList())
            range.Clear()
            'fine prima tabella

            rigaAttuale += 2

            range.Add(rigaAttuale)
            range.Add(1)



            Dim colonnaColtura = 1
            colonnaProdottoRaccolto = colonnaColtura + 1
            Dim colonnaSuperfice = colonnaProdottoRaccolto + 1
            Dim colonnaSuperficeSommabile = colonnaSuperfice + 1
            Dim colonnaMercato = colonnaSuperficeSommabile + 1
            Dim colonnaMercatoSommabile = colonnaMercato + 1
            Dim colonnaEnergia = colonnaMercatoSommabile + 1
            Dim colonnaEnergiaSommabile = colonnaEnergia + 1


            worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Coltura"
            worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = "Prodotto raccolto"
            worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = "Superficie"
            worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = "Superficie Sommabile"
            worksheet.Cell(rigaAttuale, colonnaMercato).Value = "Mercato"
            worksheet.Cell(rigaAttuale, colonnaMercatoSommabile).Value = "Mercato Sommabile"
            worksheet.Cell(rigaAttuale, colonnaEnergia).Value = "Energia"
            worksheet.Cell(rigaAttuale, colonnaEnergiaSommabile).Value = "Energia Sommabile"

            For i As Integer = colonnaColtura To colonnaEnergiaSommabile
                ApplicaStileIntestazione(worksheet, rigaAttuale, i)
            Next
            rigaAttuale += 1


            Dim dizionarioIndici As New Dictionary(Of String, Integer)
            Dim tempRiga = rigaAttuale

            If (dtMercato IsNot Nothing AndAlso dtMercato.Rows.Count > 0) Then

                For Each row In dtMercato.Rows
                    Dim coltura = row.Item("Coltura_Des")
                    Dim prodottoRaccolto = row.Item("ProdottoRaccolto")
                    Dim ha = row.Item("SuperficieTot")
                    Dim haSommabile = row.Item("SommabileTot")
                    Dim key = coltura & If(IsNothing(prodottoRaccolto), "", "-" & prodottoRaccolto)
                    dizionarioIndici(key) = rigaAttuale

                    worksheet.Cell(rigaAttuale, colonnaColtura).Value = coltura
                    worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                    worksheet.Cell(rigaAttuale, colonnaSuperfice).FormulaA1 = String.Format("=E{0}+G{0}", rigaAttuale)
                    worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).FormulaA1 = String.Format("=F{0}+H{0}", rigaAttuale)
                    worksheet.Cell(rigaAttuale, colonnaMercato).Value = ha
                    worksheet.Cell(rigaAttuale, colonnaMercatoSommabile).Value = haSommabile
                    worksheet.Cell(rigaAttuale, colonnaEnergia).Value = 0
                    worksheet.Cell(rigaAttuale, colonnaEnergiaSommabile).Value = 0
                    For i As Integer = colonnaColtura To colonnaProdottoRaccolto
                        ApplicaStileColturaProdotto(worksheet, rigaAttuale, i)
                    Next
                    For i As Integer = colonnaSuperfice To colonnaEnergiaSommabile
                        ApplicaStileGenerale(worksheet, rigaAttuale, i)
                        worksheet.Cell(rigaAttuale, i).Style.NumberFormat.Format = "0.00"
                    Next
                    rigaAttuale += 1

                Next
            End If

            If (dtEnergia IsNot Nothing AndAlso dtEnergia.Rows.Count > 0) Then
                For Each row In dtEnergia.Rows
                    Dim coltura = row.Item("Coltura_Des")
                    Dim prodottoRaccolto = row.Item("ProdottoRaccolto")
                    Dim ha = row.Item("SuperficieTot")
                    Dim haSommabile = row.Item("SommabileTot")
                    Dim key = coltura + If(IsNothing(prodottoRaccolto), "", "-" & prodottoRaccolto)
                    If dizionarioIndici.ContainsKey(key) Then
                        worksheet.Cell(dizionarioIndici.Item(key), colonnaEnergia).Value = ha
                        worksheet.Cell(dizionarioIndici.Item(key), colonnaEnergiaSommabile).Value = haSommabile
                    Else
                        worksheet.Cell(rigaAttuale, colonnaColtura).Value = coltura
                        worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                        worksheet.Cell(rigaAttuale, colonnaSuperfice).FormulaA1 = String.Format("=E{0}+G{0}", rigaAttuale)
                        worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).FormulaA1 = String.Format("=F{0}+H{0}", rigaAttuale)
                        worksheet.Cell(rigaAttuale, colonnaMercato).Value = 0
                        worksheet.Cell(rigaAttuale, colonnaMercatoSommabile).Value = 0
                        worksheet.Cell(rigaAttuale, colonnaEnergia).Value = ha
                        worksheet.Cell(rigaAttuale, colonnaEnergiaSommabile).Value = haSommabile
                        For i As Integer = colonnaColtura To colonnaProdottoRaccolto
                            ApplicaStileColturaProdotto(worksheet, rigaAttuale, i)
                        Next
                        For i As Integer = colonnaSuperfice To colonnaEnergiaSommabile
                            ApplicaStileGenerale(worksheet, rigaAttuale, i)
                            worksheet.Cell(rigaAttuale, i).Style.NumberFormat.Format = "0.00"
                        Next
                        rigaAttuale += 1
                    End If


                Next

            End If


            For i As Integer = colonnaColtura To colonnaEnergiaSommabile
                ApplicaStileTipo(worksheet, rigaAttuale, i)
            Next
            worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Totale"
            worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).FormulaA1 = String.Format("=SUM(D{0}:D{1})", tempRiga, rigaAttuale - 1)
            worksheet.Cell(rigaAttuale, colonnaMercatoSommabile).FormulaA1 = String.Format("=SUM(F{0}:F{1})", tempRiga, rigaAttuale - 1)
            worksheet.Cell(rigaAttuale, colonnaEnergiaSommabile).FormulaA1 = String.Format("=SUM(H{0}:H{1})", tempRiga, rigaAttuale - 1)


            rigaAttuale += 3

            If (dtMercato IsNot Nothing AndAlso dtMercato.Rows.Count > 0) Then

                colonnaColtura = 1
                colonnaProdottoRaccolto = colonnaColtura + 1
                colonnaSuperfice = colonnaProdottoRaccolto + 1
                colonnaSuperficeSommabile = colonnaSuperfice + 1
                Dim colonnaProduzione = colonnaSuperficeSommabile + 1
                Dim colonnaTon = colonnaProduzione + 1
                Dim colonnaEuro = colonnaTon + 1
                Dim colonnaEuroTon = colonnaEuro + 1
                Dim colonnaEuroHa = colonnaEuroTon + 1

                worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Produzione a mercato"
                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaAttuale, colonnaColtura)
                rigaAttuale += 1
                worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Coltura"
                worksheet.Range(String.Format("A{0}:A{1}", rigaAttuale, rigaAttuale + 1)).Merge()
                worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = "Prodotto raccolto"
                worksheet.Range(String.Format("B{0}:B{1}", rigaAttuale, rigaAttuale + 1)).Merge()
                worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = "Superficie"
                worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = "Sup Sommabile"
                worksheet.Cell(rigaAttuale, colonnaProduzione).Value = "Produzione"
                For i As Integer = colonnaColtura To colonnaProduzione
                    ApplicaStileIntestazione(worksheet, rigaAttuale, i)
                Next
                rigaAttuale += 1
                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileIntestazione(worksheet, rigaAttuale, i)
                Next
                worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = "ha"
                worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = "ha"
                worksheet.Cell(rigaAttuale, colonnaProduzione).Value = "t/ha"
                worksheet.Cell(rigaAttuale, colonnaTon).Value = "t"
                worksheet.Cell(rigaAttuale, colonnaEuro).Value = "€"
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = "€/Ton"
                worksheet.Cell(rigaAttuale, colonnaEuroHa).Value = "€/ha"

                rigaAttuale += 1


                tempRiga = rigaAttuale
                For Each row In dtMercato.Rows
                    Dim coltura = row.Item("Coltura_Des")
                    Dim prodottoRaccolto = row.Item("ProdottoRaccolto")
                    Dim ha = row.Item("SuperficieTot")
                    Dim haSommabile = row.Item("SommabileTot")
                    Dim ton = row.Item("ProduzioneTot")
                    Dim euro = row.Item("CostoTot")


                    worksheet.Cell(rigaAttuale, colonnaColtura).Value = coltura
                    worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                    worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = ha
                    worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = haSommabile
                    worksheet.Cell(rigaAttuale, colonnaProduzione).Value = If(ha = 0, 0, ton / ha)
                    worksheet.Cell(rigaAttuale, colonnaTon).Value = ton
                    worksheet.Cell(rigaAttuale, colonnaEuro).Value = euro
                    worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(ton = 0, 0, euro / ton)
                    worksheet.Cell(rigaAttuale, colonnaEuroHa).Value = If(ha = 0, 0, euro / ha)

                    For i As Integer = colonnaColtura To colonnaProdottoRaccolto
                        ApplicaStileColturaProdotto(worksheet, rigaAttuale, i)
                    Next
                    For i As Integer = colonnaSuperfice To colonnaEuroHa
                        ApplicaStileGenerale(worksheet, rigaAttuale, i)
                        worksheet.Cell(rigaAttuale, i).Style.NumberFormat.Format = "0.00"
                    Next
                    rigaAttuale += 1

                Next
                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileGenerale(worksheet, rigaAttuale, i)
                    If i > colonnaProdottoRaccolto Then

                        worksheet.Cell(rigaAttuale, i).Style.NumberFormat.Format = "0.00"
                    End If
                Next
                worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Totale"
                worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).FormulaA1 = String.Format("=SUM(D{0}:D{1})", tempRiga, rigaAttuale - 1)
                worksheet.Cell(rigaAttuale, colonnaEuro).FormulaA1 = String.Format("=SUM(G{0}:G{1})", tempRiga, rigaAttuale - 1)
                worksheet.Cell(rigaAttuale, colonnaTon).FormulaA1 = String.Format("=SUM(F{0}:F{1})", tempRiga, rigaAttuale - 1)
                Dim euroTot = (worksheet.Cell(rigaAttuale, colonnaEuro).Value)
                Dim tonTot = (worksheet.Cell(rigaAttuale, colonnaTon).Value)
                Dim haTot = (worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value)
                worksheet.Cell(rigaAttuale, colonnaProduzione).Value = If(haTot = 0, 0, tonTot / haTot)
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(tonTot = 0, 0, euroTot / tonTot)
                worksheet.Cell(rigaAttuale, colonnaEuroHa).Value = If(haTot = 0, 0, euroTot / haTot)
                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileTipo(worksheet, rigaAttuale, i)
                Next
                rigaAttuale += 3
            End If



            If (dtEnergia IsNot Nothing AndAlso dtEnergia.Rows.Count > 0) Then




                colonnaColtura = 1
                colonnaProdottoRaccolto = colonnaColtura + 1
                colonnaSuperfice = colonnaProdottoRaccolto + 1
                colonnaSuperficeSommabile = colonnaSuperfice + 1
                Dim colonnaProduzione = colonnaSuperficeSommabile + 1
                Dim colonnaTon = colonnaProduzione + 1
                Dim colonnaBMP = colonnaTon + 1
                Dim colonnaM3 = colonnaBMP + 1
                Dim colonnaEuro = colonnaM3 + 1
                Dim colonnaEuroTon = colonnaEuro + 1
                Dim colonnaEuroHa = colonnaEuroTon + 1

                worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Produzione a energia"
                ApplicaStileIntestazioneProduzioneCampo(worksheet, rigaAttuale, 1)
                rigaAttuale += 1
                worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Coltura"
                worksheet.Range(String.Format("A{0}:A{1}", rigaAttuale, rigaAttuale + 1)).Merge()
                worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = "Prodotto raccolto"
                worksheet.Range(String.Format("B{0}:B{1}", rigaAttuale, rigaAttuale + 1)).Merge()
                worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = "Superficie"
                worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = "Sup Sommabile"
                worksheet.Cell(rigaAttuale, colonnaProduzione).Value = "Produzione"
                worksheet.Cell(rigaAttuale, colonnaBMP).Value = "BMP"



                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileIntestazione(worksheet, rigaAttuale, i)
                Next
                rigaAttuale += 1
                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileIntestazione(worksheet, rigaAttuale, i)
                Next
                worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = "ha"
                worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = "ha"
                worksheet.Cell(rigaAttuale, colonnaProduzione).Value = "t/ha"
                worksheet.Cell(rigaAttuale, colonnaTon).Value = "t"
                worksheet.Cell(rigaAttuale, colonnaBMP).Value = "m3/ha"
                worksheet.Cell(rigaAttuale, colonnaM3).Value = "m3"
                worksheet.Cell(rigaAttuale, colonnaEuro).Value = "€"
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = "€/Ton"
                worksheet.Cell(rigaAttuale, colonnaEuroHa).Value = "€/ha"

                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileGenerale(worksheet, rigaAttuale, i)
                Next
                rigaAttuale += 1

                tempRiga = rigaAttuale
                For Each row In dtEnergia.Rows
                    Dim coltura = row.Item("Coltura_Des")
                    Dim prodottoRaccolto = row.Item("ProdottoRaccolto")
                    Dim ha = row.Item("SuperficieTot")
                    Dim haSommabile = row.Item("SommabileTot")
                    Dim ton = row.Item("ProduzioneTot")
                    Dim m3 = row.Item("m3Tot")
                    Dim euro = row.Item("CostoTot")


                    worksheet.Cell(rigaAttuale, colonnaColtura).Value = coltura
                    worksheet.Cell(rigaAttuale, colonnaProdottoRaccolto).Value = prodottoRaccolto
                    worksheet.Cell(rigaAttuale, colonnaSuperfice).Value = ha
                    worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value = haSommabile
                    worksheet.Cell(rigaAttuale, colonnaProduzione).Value = ton / ha
                    worksheet.Cell(rigaAttuale, colonnaTon).Value = ton
                    worksheet.Cell(rigaAttuale, colonnaBMP).Value = m3 / ha
                    worksheet.Cell(rigaAttuale, colonnaM3).Value = m3
                    worksheet.Cell(rigaAttuale, colonnaEuro).Value = euro
                    worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(ton = 0, 0, euro / ton)
                    worksheet.Cell(rigaAttuale, colonnaEuroHa).Value = If(ha = 0, 0, euro / ha)

                    For i As Integer = colonnaColtura To colonnaProdottoRaccolto
                        ApplicaStileColturaProdotto(worksheet, rigaAttuale, i)
                    Next
                    For i As Integer = colonnaSuperfice To colonnaEuroHa
                        ApplicaStileGenerale(worksheet, rigaAttuale, i)
                        worksheet.Cell(rigaAttuale, i).Style.NumberFormat.Format = "0.00"
                    Next
                    rigaAttuale += 1

                Next
                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileGenerale(worksheet, rigaAttuale, i)
                    If i > 2 Then

                        worksheet.Cell(rigaAttuale, i).Style.NumberFormat.Format = "0.00"
                    End If
                Next
                worksheet.Cell(rigaAttuale, colonnaColtura).Value = "Totale"
                worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).FormulaA1 = String.Format("=SUM(D{0}:D{1})", tempRiga, rigaAttuale - 1)
                worksheet.Cell(rigaAttuale, colonnaTon).FormulaA1 = String.Format("=SUM(F{0}:F{1})", tempRiga, rigaAttuale - 1)
                worksheet.Cell(rigaAttuale, colonnaM3).FormulaA1 = String.Format("=SUM(H{0}:H{1})", tempRiga, rigaAttuale - 1)
                If (worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value > 0) Then
                    worksheet.Cell(rigaAttuale, colonnaProduzione).Value = (worksheet.Cell(rigaAttuale, colonnaTon).Value) / (worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value)
                    worksheet.Cell(rigaAttuale, colonnaBMP).Value = (worksheet.Cell(rigaAttuale, colonnaM3).Value) / (worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value)
                End If
                worksheet.Cell(rigaAttuale, colonnaEuro).FormulaA1 = String.Format("=SUM(I{0}:I{1})", tempRiga, rigaAttuale - 1)
                Dim euroTot = (worksheet.Cell(rigaAttuale, colonnaEuro).Value)
                Dim tonTot = (worksheet.Cell(rigaAttuale, colonnaTon).Value)
                Dim haTot = (worksheet.Cell(rigaAttuale, colonnaSuperficeSommabile).Value)
                worksheet.Cell(rigaAttuale, colonnaEuroTon).Value = If(tonTot = 0, 0, euroTot / tonTot)
                worksheet.Cell(rigaAttuale, colonnaEuroHa).Value = If(haTot = 0, 0, euroTot / haTot)
                For i As Integer = colonnaColtura To colonnaEuroHa
                    ApplicaStileTipo(worksheet, rigaAttuale, i)
                Next
            End If
            range.Add(rigaAttuale)
            range.Add(11)
            rangeTabelle.Add(range.ToList())
            range.Clear()






            configurazioneGeneraleExcel(worksheet)

            Return True
        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Dim nomeRoutine = "DW_CDG_Costi_Ricavi_BIZ.Ricerca_DT_SPV().ExportToExcel_Report_PianoColturale"
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Function

#End Region

#Region "Utility"

    Private Function creazioneStringaErroreDatatable(ByVal tipoReport As Integer,
                                                    ByVal azienda As String,
                                                    ByVal dataDal As String,
                                                    ByVal dataAl As String,
                                                    ByVal costiPersonaleSeparatamente As Integer,
                                                    ByVal includiAziendeFiglie As Integer,
                                                    ByVal DettaglioSpecieVarieta As Integer,
                                                    ByVal VediDettagliOperazioni As Integer,
                                                    ByVal errore As String) As String
        Dim erroreDatatable As String = "{"
        erroreDatatable += String.Format("""report"": ""{0}"", ""azienda"": ""{1}"" , ""dataDal"":""{2}"", ""dataAl"":""{3}"", ""costiPersonale"": ""{4}"", ""includiFiglie"": ""{5}"", ""dettaglioSpecieVarieta"":""{6}"", ""vediDettagliOperazioni"":""{7}"", ""errore"":""{8}""",
                                         tipoReport, azienda, dataDal, dataAl, costiPersonaleSeparatamente, includiAziendeFiglie, DettaglioSpecieVarieta, VediDettagliOperazioni, errore)
        erroreDatatable += "}"
        Return erroreDatatable
    End Function

#End Region

End Class
