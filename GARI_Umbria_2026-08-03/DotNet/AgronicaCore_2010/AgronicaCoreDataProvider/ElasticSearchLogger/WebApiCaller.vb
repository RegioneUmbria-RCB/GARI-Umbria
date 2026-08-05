Imports System.Net.Http
Imports System.Web
Imports System.Text
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Public Class WebApiCaller
    Inherits LogProvider
    Private Property _hc As HttpClient
    Private Property _key As Tuple(Of String, String) = Nothing
    Public Sub New(ByVal baseUrl As String, ByVal token As Tuple(Of String, String))
        _hc = New HttpClient()
        If token IsNot Nothing Then
            _key = token
        End If
        _hc.BaseAddress = New Uri(baseUrl)
    End Sub
    Protected Sub UpdateKey(ByVal token As Tuple(Of String, String))
        _key = token
    End Sub
    Protected Function ChiamaWS(Of T)(ByVal method As HttpMethod,
                                      ByVal QueryParams As Dictionary(Of String, String),
                                      ByVal BodyParams As Dictionary(Of String, String),
                                      ByVal bodyreq As String,
                                      ByVal bodytype As String,
                                      ByVal endpoint As String,
                                        Optional ByVal AllowAnonymous As Boolean = False) As T
        Dim resp As HttpResponseMessage = Nothing
        Try
            Dim req As New HttpRequestMessage(method, _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams))
            If _key IsNot Nothing And AllowAnonymous = False Then
                req.Headers.Add("Authorization", _key.Item1 + " " + _key.Item2)
            End If
            If method = HttpMethod.Post OrElse method = HttpMethod.Put Then
                If bodytype = "x-wwww-form-urlencoded" And BodyParams IsNot Nothing Then
                    req.Content = New FormUrlEncodedContent(BodyParams)
                Else
                    req.Content = New StringContent(bodyreq, Encoding.UTF8, bodytype)
                End If
            End If
            resp = _hc.SendAsync(req).GetAwaiter().GetResult()
            Dim strResp = ""
            If resp.IsSuccessStatusCode = True Then
                strResp = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            End If
            Select Case GetType(T)
                Case GetType(Boolean)
                    Dim r As Object = resp.IsSuccessStatusCode
                    Return CType(r, T)
                Case Else
                    Return JsonConvert.DeserializeObject(Of T)(resp.IsSuccessStatusCode)
            End Select
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)
        End Try
    End Function
    Private Function BuildQueryString(ByVal QueryParams As Dictionary(Of String, String)) As String
        Dim ret As String = ""
        If QueryParams Is Nothing Then
            Return ret
        End If
        For Each key In QueryParams.Keys
            If ret = "" Then
                ret += "?"
            Else
                ret += "&"
            End If
            ret += key + "=" + QueryParams(key)
        Next
        Return ret
    End Function
End Class

