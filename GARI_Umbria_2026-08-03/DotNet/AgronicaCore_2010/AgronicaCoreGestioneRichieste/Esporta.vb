Imports System.IO
Imports AgronicaCoreUtility
Imports ClosedXML.Excel


Public Class Esporta



    ' Uhalid 21/06/24, se c'e' bisogno di scaricare un excel da un webmethod si puo' usare questa funzione che ritorna il path al xlsx
    ' guardare AgronicaStampe_2010 statistiche_accesso per un esempio
    Public Shared Function EsportaExcelPath(dt As DataTable, NomeFile As String, Optional ByRef objAllegato As Varie.objAllegato = Nothing, Optional ByVal applyFormating As Boolean = False) As String
        Dim objAgroWebConfig As New AgroWebConfig
        Dim path_file As String = objAgroWebConfig.PathFileTemporanei

        If path_file = "" Then
            Throw New Exception("PathFileTemporanei in configurazione siti non impostato")
        End If

        path_file = FileSystemHelper.AggiungiSlashSeNonEsiste(path_file)

        If Not Directory.Exists(path_file) Then Directory.CreateDirectory(path_file)

        If NomeFile = "" Then
            NomeFile = "Esporta_Excel.xlsx"
        Else
            NomeFile &= ".xlsx"
        End If

        path_file &= NomeFile

        If IO.File.Exists(path_file) Then
            IO.File.Delete(path_file)
        End If

        dt.TableName = "Export"

        Dim workbook = New XLWorkbook()
        Dim ws = workbook.Worksheets.Add(dt)
        'If Not IsNothing(workbook.Worksheets.FirstOrDefault) Then
        '    workbook.Worksheets.First.Columns.AdjustToContents()
        'End If

        If applyFormating Then

            For j As Integer = 0 To dt.Columns.Count - 1

                Dim col = dt.Columns(j)
                Dim exportFormat As String = col.ExtendedProperties("ExportFormat")?.ToString()?.ToLower()

                Dim excelColumn = ws.Column(j + 1)

                Select Case exportFormat
                    Case "number"
                        excelColumn.Style.NumberFormat.Format = "0.00"
                    Case "date"
                        excelColumn.Style.NumberFormat.Format = "dd/mm/yyyy"
                    Case Else
                        excelColumn.Style.NumberFormat.Format = "@"
                End Select
            Next

        End If

        workbook.SaveAs(path_file)

        If objAllegato IsNot Nothing Then
            objAllegato.NomeFile = NomeFile
            objAllegato.Estensione = ".xlsx"
            objAllegato.File = My.Computer.FileSystem.ReadAllBytes(path_file)
        End If

        If IO.File.Exists(path_file) Then
            IO.File.Delete(path_file)
        End If

        Return path_file
    End Function

    'attenzione non usare dentro update panel o non funziona
    'attenzione non usare dentro update panel o non funziona
    'attenzione non usare dentro update panel o non funziona
    'attenzione non usare dentro update panel o non funziona
    Public Shared Sub EsportaExcel(ByVal dt As DataTable, ByVal NomeFile As String, ByRef page As System.Web.UI.Page)

        'attenzione non usare dentro update panel o non funziona
        'attenzione non usare dentro update panel o non funziona
        'attenzione non usare dentro update panel o non funziona
        'attenzione non usare dentro update panel o non funziona

        Dim objAgroWebConfig As New AgroWebConfig
        Dim path_file As String = objAgroWebConfig.PathFileTemporanei

        If path_file = "" Then
            Throw New Exception("PathFileTemporanei in configurazione siti non impostato")
        End If

        If Not System.IO.Directory.Exists(path_file) Then
            Throw New Exception("La directory PathFileTemporanei:" & path_file & "non esiste")
        End If

        path_file = FileSystemHelper.AggiungiSlashSeNonEsiste(path_file)

        If Not Directory.Exists(path_file) Then Directory.CreateDirectory(path_file)

        If NomeFile = "" Then
            NomeFile = "Esporta_Excel.xlsx"
        Else
            NomeFile &= ".xlsx"
        End If

        path_file &= NomeFile

        'If NomeFile <> "" Then
        '    path_file &= NomeFile & ".xlsx"
        'Else
        '    path_file &= "Esporta_Excel.xlsx"
        'End If


        If IO.File.Exists(path_file) Then
            IO.File.Delete(path_file)
        End If



        dt.TableName = "Export"

        Dim workbook = New XLWorkbook()
        workbook.Worksheets.Add(dt)
        workbook.SaveAs(path_file)
        Dim file As System.IO.FileStream
        file = IO.File.Open(path_file, FileMode.Open)
        Dim l As String = file.Length.ToString()
        file.Close()

        'attenzione non usare dentro update panel o non funziona

        page.Response.Clear()
        page.Response.ClearHeaders()
        page.Response.ClearContent()
        page.Response.AddHeader("content-disposition", "attachment; filename=" & NomeFile)
        page.Response.ContentType = "application/vnd.ms-excel"
        page.Response.AddHeader("Content-Length", l)
        page.Response.WriteFile(path_file)
        page.Response.End()
        IO.File.Delete(path_file)

    End Sub

    Public Function CreaAllegatoFittizioPDC() As String

        Dim objAgroWebConfig As New AgroWebConfig
        Dim pathFileTemporanei As String = objAgroWebConfig.PathFileTemporanei

        If pathFileTemporanei = "" Then
            Throw New Exception("PathFileTemporanei in configurazione siti non impostato")
        End If

        If Not System.IO.Directory.Exists(pathFileTemporanei) Then
            Throw New Exception("La directory PathFileTemporanei:" & pathFileTemporanei & "non esiste")
        End If


        'preparo l'allegato
        Dim workbook = New XLWorkbook()
        Dim path As String = FileSystemHelper.AggiungiSlashSeNonEsiste(pathFileTemporanei)
        Dim nomeFile As String = "test_xls"
        Dim pathFileXlsx As String = path & nomeFile & ".xlsx"
        Dim pathFileXls As String = path & nomeFile & ".xls"

        'Devo eliminare dalla cartella temporanea i file se esistono già, altrimenti non si riesce a scriverli
        If IO.File.Exists(pathFileXlsx) Then
            IO.File.Delete(pathFileXlsx)
        End If

        If IO.File.Exists(pathFileXls) Then
            IO.File.Delete(pathFileXls)
        End If


        Dim worksheet = workbook.Worksheets.Add("Export")
        'pivasuperuser
        worksheet.Cell("A1").SetValue("test")
        worksheet.Cell("B1").SetValue("test")
        'Chiave
        worksheet.Cell("A2").SetValue("test")
        worksheet.Cell("B2").SetValue("test")
        
        worksheet.Cell("B1").SetDataType(XLDataType.Text)
        worksheet.Cell("B2").SetDataType(XLDataType.Text)

        workbook.SaveAs(pathFileXlsx)
        workbook.Dispose()
        workbook = Nothing

        '26/02/2021: per esigenze di compatibilità dei laboratori rinominiamo il file xlsx in xls, anche se di fatto è un xlsx
        FileSystem.Rename(pathFileXlsx, pathFileXls)

        Return pathFileXls

    End Function

    Public Shared Sub EsportaExcelAbacoDPI(ByVal dt As DataTable, ByVal NomeFile As String, ByVal NomeTabella As String)

        'NomeFile &= ".xlsx"


        If IO.File.Exists(NomeFile) Then
            IO.File.Delete(NomeFile)
        End If

        dt.TableName = NomeTabella

        Dim workbook = New XLWorkbook()
        workbook.Worksheets.Add(dt)
        workbook.SaveAs(NomeFile)
        Dim file As System.IO.FileStream
        file = IO.File.Open(NomeFile, FileMode.Open)
        Dim l As String = file.Length.ToString()
        file.Close()

    End Sub



End Class
