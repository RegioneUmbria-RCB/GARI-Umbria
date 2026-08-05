Imports System
Imports System.Collections.Generic
Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports Microsoft.VisualBasic
Imports Newtonsoft.Json

Public Class WebApiCaller
    Implements IDisposable

    Private _caller As String
    Private Property _hc As HttpClient
    Private Property _key As Tuple(Of String, String) = Nothing
    Public Sub New(ByVal baseUrl As String, ByVal token As Tuple(Of String, String), ByVal caller As String)
        _hc = New HttpClient()
        If token IsNot Nothing Then
            _key = token
        End If
        _caller = caller
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
                                      Optional ByVal DecodeCoreWSResponse As Boolean = False,
                                      Optional ByVal AllowAnonymous As Boolean = False,
                                      Optional ByVal ApiFormatParams As Boolean = False,
                                      Optional ByRef ResponseStatusCode As Integer = 0) As T
        Dim resp As HttpResponseMessage = Nothing
        Try
            Dim req As New HttpRequestMessage(method, _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams, ApiFormatParams))
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
            ResponseStatusCode = resp.StatusCode
            Dim strResp = ""

            If resp.IsSuccessStatusCode = False Then
                Select Case _caller
                    Case NameOf(ReUmbriaApiProvider)
                        If method = HttpMethod.Post Then
                            Throw New ReUmbriaApiProviderException("Errore in chiamata ad Api Regione Umbria" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams, ApiFormatParams) & vbCrLf & "Payload: " & bodyreq & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                        Else
                            Throw New ReUmbriaApiProviderException("Errore in chiamata ad Api Regione Umbria" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams, ApiFormatParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                        End If

                End Select
            Else
                strResp = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                If DecodeCoreWSResponse Then
                    strResp = strResp.Substring(5, strResp.Length - 6)
                End If
            End If

            Select Case GetType(T)
                Case GetType(Boolean)
                    Dim r As Object = resp.IsSuccessStatusCode
                    Return CType(r, T)
                Case Else
                    Return JsonConvert.DeserializeObject(Of T)(strResp)
            End Select

        Catch ex As ReUmbriaApiProviderException
            Throw New ReUmbriaApiProviderException(ex.Message, ex)
        Catch ex As Exception
            Throw New Exception(ex.Message, ex)

        End Try
    End Function

    Private Function BuildQueryString(ByVal QueryParams As Dictionary(Of String, String), ByVal ApiFormatParams As Boolean) As String
        Dim ret As String = ""
        If QueryParams Is Nothing Then
            Return ret
        End If

        If ApiFormatParams Then
            For Each key In QueryParams.Keys
                ret += "/" + key + "/" + QueryParams(key)
            Next
        Else
            For Each key In QueryParams.Keys
                If ret = "" Then
                    ret += "?"
                Else
                    ret += "&"
                End If
                ret += key + "=" + QueryParams(key)
            Next
        End If


        Return ret
    End Function


    Public Sub Dispose() Implements IDisposable.Dispose
        'Throw New NotImplementedException()
    End Sub

End Class
