Imports CrystalDecisions.CrystalReports
Imports CrystalDecisions.Shared

Namespace DatasetExporter

    ''' -----------------------------------------------------------------------------
    ''' Project	 : Exporter
    ''' Class	 : Exporter.DatasetExporter.DatasetToRpt
    ''' 
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Classe per la generazione di un oggetto report document da un dataset e un path di file rpt
    ''' </summary>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[michele.venturi]	08/09/2006	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Class DatasetToRPT

        Private Shared Sub AggiungiImmagineCampoBlob(ByVal path As String, ByVal nomecampo As String, ByRef dt As DataTable)

            If System.IO.File.Exists(path) = True Then
                Dim bmpF As System.Drawing.Bitmap = New System.Drawing.Bitmap(path)
                Dim logo() As Byte
                Dim c As New System.Drawing.ImageConverter
                logo = c.ConvertTo(bmpF, GetType(Byte()))

                For Each dr In dt.Rows
                    dr.Item(nomecampo) = logo
                Next

            End If


        End Sub



        ''' <summary>
        ''' Crea il report
        ''' </summary>
        ''' <param name="dati"></param>
        ''' <param name="pathFileRPTTemplate"></param>
        ''' <param name="pathFileRPTOutput"></param>
        ''' <param name="parametri"></param>
        ''' <param name="AggiungiImmagine">path immagine|nome tabella|nome campo</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreaReport(ByVal dati As DataSet, ByVal pathFileRPTTemplate As String, ByVal pathFileRPTOutput As String, Optional ByVal parametri As Hashtable = Nothing, _
                                          Optional ByVal AggiungiImmagine As String = "") As Boolean

            Try
                Dim rpt As New Engine.ReportDocument

                'path immagine|nome tabella|nome campo
                Dim DatiImmagine As String() = AggiungiImmagine.Split("|")

                With rpt
                    ' carico il file .rpt
                    .Load(pathFileRPTTemplate, CrystalDecisions.[Shared].OpenReportMethod.OpenReportByTempCopy)

                    ' associo i dati
                    If dati.Tables.Count > 1 Then
                        Dim i As Integer = 0
                        For Each reportTable As CrystalDecisions.CrystalReports.Engine.Table In .Database.Tables
                            dati.Tables(i).TableName = reportTable.Name
                            If DatiImmagine.Length > 1 AndAlso dati.Tables(i).TableName = DatiImmagine(1) Then
                                AggiungiImmagineCampoBlob(DatiImmagine(0), DatiImmagine(2), dati.Tables(i))
                            End If
                            i += 1
                        Next

                        For Each subRep As CrystalDecisions.CrystalReports.Engine.ReportDocument In .Subreports
                            subRep.SetDataSource(dati)
                        Next
                        .SetDataSource(dati)
                    Else
                        .SetDataSource(dati.Tables(0))
                    End If

                    ' passo i parametri se esistono 
                    If Not parametri Is Nothing Then
                        Dim ide As IDictionaryEnumerator = parametri.GetEnumerator
                        While ide.MoveNext
                            ' Key contiene il nome del parametro
                            ' Value contiene il valore del parametro
                            .SetParameterValue(ide.Key, ide.Value)
                        End While
                    End If

                    ''.SaveAs(pathFileRPTOutput, CrystalDecisions.[Shared].ReportFileFormat.VSNetFileFormat)
                    ' esportazione in formato RPT
                    Dim crExportOptions As ExportOptions
                    Dim crDiskFileDestinationOptions As New DiskFileDestinationOptions
                    crDiskFileDestinationOptions.DiskFileName = pathFileRPTOutput
                    crExportOptions = rpt.ExportOptions
                    With crExportOptions
                        .DestinationOptions = crDiskFileDestinationOptions
                        .ExportDestinationType = ExportDestinationType.DiskFile
                        .ExportFormatType = ExportFormatType.CrystalReport
                    End With

                    .Export()

                    ' operazioni utili per non saturare la coda dei processi Crystal Reports
                    .Close()
                    .Dispose()

                End With
            Catch ex As Exception
                Throw New ApplicationException("Creazione report in formato RPT non riuscita!", ex)
            End Try

            Return True

        End Function

    End Class

End Namespace
