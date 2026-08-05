Imports System.Web.Services
Imports System.ComponentModel
Imports AgronicaCoreVarieBIZ
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports System.IO
Imports System.Runtime.Caching

' Per consentire la chiamata di questo servizio Web dallo script utilizzando ASP.NET AJAX, rimuovere il commento dalla riga seguente.
<System.Web.Script.Services.ScriptService()>
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")>
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Public Class KendoGridServerExport
    Inherits System.Web.Services.WebService

    Private Const CacheKey As String = "ChunkData"

    <WebMethod(EnableSession:=True, CacheDuration:=43200)>
    <Script.Services.ScriptMethod()>
    Public Function StoreDataChunk(ByVal fileName As String, ByVal chunk As Integer, ByVal data As String
                                  ) As RispostaStandard

        Dim r As New RispostaStandard()
        Dim chunkDinctionary As Dictionary(Of String, List(Of KeyValuePair(Of Int32, String))) = New Dictionary(Of String, List(Of KeyValuePair(Of Integer, String)))
        Dim chunkData As List(Of KeyValuePair(Of Int32, String)) = New List(Of KeyValuePair(Of Integer, String))
        Dim memCacher As New MemoryCacher()

        Try

            Dim cachedValue = memCacher.GetValue(CacheKey)
            If Not cachedValue Is Nothing Then
                chunkDinctionary = DirectCast(cachedValue, Dictionary(Of String, List(Of KeyValuePair(Of Int32, String))))
            Else
                memCacher.Add(CacheKey, chunkDinctionary, DateTimeOffset.UtcNow.AddHours(1))
            End If

            If Not chunkDinctionary.ContainsKey(fileName) Then
                chunkDinctionary.Add(fileName, chunkData)
            Else
                chunkData = chunkDinctionary(fileName)
            End If

            If Not chunkData.Any(Function(s) s.Key = chunk) Then
                chunkData.Add(New KeyValuePair(Of Int32, String)(chunk, data))
            End If


            memCacher.SetValue(CacheKey, chunkDinctionary)

            r.RispostaOK = True
            r.RispostaStringa = ""

        Catch ex As Exception
            memCacher.Delete(CacheKey)
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Function ExportToExcel(ByVal options As String, ByVal model As String
                                  ) As rispostaStandard(Of Byte())

        Dim r As New rispostaStandard(Of Byte())
        Dim byteStream As Byte() = Nothing
        Dim sb As New StringBuilder()
        Dim chunkData As List(Of KeyValuePair(Of Int32, String)) = New List(Of KeyValuePair(Of Integer, String))
        Dim dic As Dictionary(Of String, List(Of KeyValuePair(Of Int32, String))) = Nothing
        Dim memCacher As New MemoryCacher()

        Try

            Dim columns = JsonConvert.DeserializeObject(Of List(Of JObject))(model)
            Dim fileOption = JsonConvert.DeserializeObject(Of JObject)(options)
            Dim fileName = fileOption.GetValue("fileName")

            Dim settingLoc As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
            Dim rows As List(Of JObject) = New List(Of JObject)()

            Dim cachedValue = memCacher.GetValue(CacheKey)
            If Not cachedValue Is Nothing Then
                dic = DirectCast(cachedValue, Dictionary(Of String, List(Of KeyValuePair(Of Int32, String))))
                If dic.ContainsKey(fileName) Then
                    chunkData = dic(fileName).OrderBy(Function(s) s.Key).ToList()
                    For Each kvp As KeyValuePair(Of Int32, String) In chunkData
                        rows.AddRange(JsonConvert.DeserializeObject(Of List(Of JObject))(kvp.Value, settingLoc))
                    Next
                End If
            End If

            Dim dt As New DataTable()
            dt.TableName = "Esportazione"

            Dim columnNamePamming = New Dictionary(Of String, String)
            Dim field As JToken
            'Aggiungo le colonne
            For Each jc As JObject In columns
                Dim columnTitle As String = jc.GetValue("title").ToString()
                field = jc.GetValue("field")
                Dim hidden = False
                If jc.GetValue("hidden") IsNot Nothing Then
                    hidden = jc.GetValue("hidden")
                End If
                If (Not IsNothing(field)) AndAlso Not hidden Then
                    Dim columnName As String = field.ToString()
                    dt.Columns.Add(columnTitle)
                    If Not columnNamePamming.ContainsKey(columnName) Then
                        columnNamePamming.Add(columnName, columnTitle)
                    End If
                End If
            Next

            'Aggiungo le righe
            For Each row As JObject In rows

                Dim newRow = dt.NewRow()

                For Each prop As JProperty In row.Properties
                    Dim columnName As String = ""
                    If columnNamePamming.ContainsKey(prop.Name) Then
                        columnName = columnNamePamming(prop.Name)
                    End If
                    If Not String.IsNullOrEmpty(columnName) Then
                        newRow(columnName) = prop.Value
                    End If

                Next

                dt.Rows.Add(newRow)

            Next

            Dim ms As MemoryStream = New MemoryStream()
            Dim workbook = New ClosedXML.Excel.XLWorkbook()
            Dim ws = workbook.Worksheets.Add(dt)

            If rows.Count > 100 Then
                ws.Columns().AdjustToContents(1, 100)
            Else
                ws.Columns().AdjustToContents()
            End If

            For Each col In ws.ColumnsUsed()
                If col.Width < 10 Then col.Width = 10
            Next

            workbook.SaveAs(ms)

            r.RispostaOK = True
            r.RispostaStringa = ms.ToArray()

            If Not dic Is Nothing Then
                dic.Remove(fileName)
            End If
            memCacher.SetValue(CacheKey, dic)

        Catch ex As Exception

            memCacher.Delete(CacheKey)
            r.RispostaOK = False
            r.Errore = "Errore durante l'operazione: " & vbCrLf &
                AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

End Class

Public Class MemoryCacher

    Public Function GetValue(key As String) As Object
        Dim MemoryCache As MemoryCache = MemoryCache.Default
        Return MemoryCache.Get(key)
    End Function

    Public Function Add(ByVal key As String, ByVal value As Object, ByVal absExpiration As DateTimeOffset) As Boolean
        Dim memoryCache As MemoryCache = MemoryCache.[Default]
        Return memoryCache.Add(key, value, absExpiration)
    End Function

    Public Sub Delete(ByVal key As String)
        Dim memoryCache As MemoryCache = MemoryCache.[Default]

        If memoryCache.Contains(key) Then
            memoryCache.Remove(key)
        End If
    End Sub

    Public Sub SetValue(key As String, ByVal value As Object)
        Dim cached = GetValue(key)
        If Not cached Is Nothing Then
            cached = value
        End If

    End Sub

End Class