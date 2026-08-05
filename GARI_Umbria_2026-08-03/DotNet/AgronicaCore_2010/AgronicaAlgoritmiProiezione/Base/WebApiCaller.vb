Imports System.Net.Http
Imports System.Web
Imports System.Text
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider
Imports System.Net.Mime

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

    Public Sub UpdateKey(ByVal token As Tuple(Of String, String))
        _key = token
    End Sub

    Public Function ChiamaWS(Of T)(ByVal method As HttpMethod,
                                   ByVal QueryParams As Dictionary(Of String, String),
                                   ByVal BodyParams As Dictionary(Of String, String),
                                   ByVal bodyreq As String,
                                   ByVal bodytype As String,
                                   ByVal endpoint As String,
                                   Optional ByVal DecodeCoreWSResponse As Boolean = False,
                                   Optional ByVal AllowAnonymous As Boolean = False,
                                   Optional ByVal SaveStreamToFile As Boolean = False,
                                   Optional ByVal savePath As String = "") As T


        Dim resp As HttpResponseMessage = Nothing
        Try
            Dim req As New HttpRequestMessage(method, _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams))
            If _key IsNot Nothing And AllowAnonymous = False Then
                req.Headers.Add(_key.Item1, _key.Item2)
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
            Dim byteResp As Byte() = Nothing
            If resp.IsSuccessStatusCode = False Then
                'Select Case _caller
                '    Case NameOf(ColdirettiIdentityProvider)
                '        Throw New ColdirettiIdentityProviderException("Errore in chiamata a Coldiretti Keycloack" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                '    Case NameOf(ColdirettiProvisioner)
                '        If resp.StatusCode = Net.HttpStatusCode.BadRequest OrElse resp.StatusCode = Net.HttpStatusCode.NotFound Then
                '            strResp = ""
                '        Else
                '            Dim err = JsonConvert.DeserializeObject(Of ProvisioningResponseError)(resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                '            Throw New ColdirettiPDSException("Errore in chiamata a Coldiretti Provisioner" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & err.errors(0).status.ToString() + " - " + err.errors(0).detail)
                '        End If
                '    Case NameOf(WebApiDataPublish)
                '        If resp.StatusCode = Net.HttpStatusCode.BadRequest Then
                '            strResp = ""
                '        Else
                '            Throw New DataPublishException("Errore in chiamata a DataPublish" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                '        End If
                '    Case NameOf(AgronicaCoreWSController)
                '        Throw New AgronicaCoreWSControllerException("Errore in chiamata a CoreWs Agronica" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                'End Select
            Else
                Select Case resp.Content.Headers.ContentType.ToString()
                    Case "image/tiff", "image/bmp", "application/octet-stream"
                        byteResp = resp.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult()
                        If byteResp.Length = 0 Then

                            Throw New Exception("Stream vuoto")
                        End If

                    Case Else
                        strResp = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
                        If DecodeCoreWSResponse Then
                            strResp = strResp.Substring(5, strResp.Length - 6)
                        End If
                End Select
            End If
            Select Case GetType(T)
                Case GetType(Boolean)
                    Dim r As Object = resp.IsSuccessStatusCode
                    Return CType(r, T)
                Case GetType(Byte())
                    If SaveStreamToFile Then
                        Dim fname As String = ""
                        Dim fNameProp As String = resp.Content.Headers.ContentDisposition.ToString().Split(";").Where(Function(x) x.Trim().StartsWith("filename=")).FirstOrDefault().Trim()
                        If fNameProp <> "" Then
                            fname = fNameProp.Split("=")(1)
                        End If
                        If fname = "" Then
                            Throw New Exception("Filename vuoto o non trovato")
                        Else
                            IO.File.WriteAllBytes(IO.Path.Combine(savePath, fname), byteResp)
                        End If
                    End If
                    Dim r As Object = byteResp
                    Return CType(r, T)
                Case Else
                    Return JsonConvert.DeserializeObject(Of T)(strResp)
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
    Public Sub Dispose() Implements IDisposable.Dispose
        'Throw New NotImplementedException()
    End Sub

End Class

