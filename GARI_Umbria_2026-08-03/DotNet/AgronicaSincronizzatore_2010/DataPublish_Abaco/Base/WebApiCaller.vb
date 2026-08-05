Imports System.Net.Http
Imports System.Web
Imports System.Text
Imports Newtonsoft.Json
Imports AgronicaCoreDemetraBIZ

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
            If resp.IsSuccessStatusCode = False Then
                Select Case _caller
                    Case NameOf(ColdirettiIdentityProvider)
                        Throw New ColdirettiIdentityProviderException("Errore in chiamata a Coldiretti Keycloack" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                    Case NameOf(ColdirettiProvisioner)
                        If resp.StatusCode = Net.HttpStatusCode.BadRequest OrElse resp.StatusCode = Net.HttpStatusCode.NotFound Then
                            strResp = ""
                        Else
                            Dim err = JsonConvert.DeserializeObject(Of ProvisioningResponseError)(resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                            Throw New ColdirettiPDSException("Errore in chiamata a Coldiretti Provisioner" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & err.errors(0).status.ToString() + " - " + err.errors(0).detail)
                        End If
                    Case NameOf(WebApiDataPublish)
                        If resp.StatusCode = Net.HttpStatusCode.BadRequest Then
                            strResp = ""
                        Else
                            Throw New DataPublishException("Errore in chiamata a DataPublish" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                        End If
                    Case NameOf(AgronicaCoreWSController)
                        Throw New AgronicaCoreWSControllerException("Errore in chiamata a CoreWs Agronica" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & resp.StatusCode.ToString() + " - " + resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
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


        Catch ex As ColdirettiIdentityProviderException
            Throw New ColdirettiIdentityProviderException(ex.Message, ex)
        Catch ex As ColdirettiPDSException
            Throw New ColdirettiPDSException(ex.Message, ex)
        Catch ex As DataPublishException
            Throw New DataPublishException(ex.Message, ex)
        Catch ex As AgronicaCoreWSControllerException
            Throw New AgronicaCoreWSControllerException(ex.Message, ex)
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

    'Protected Overridable Sub Dispose(disposing As Boolean)
    '    If Not disposedValue Then
    '        If disposing Then
    '            ' TODO: eliminare lo stato gestito (oggetti gestiti)
    '        End If

    '        ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
    '        ' TODO: impostare campi di grandi dimensioni su Null
    '        disposedValue = True
    '    End If
    'End Sub

    '' ' TODO: eseguire l'override del finalizzatore solo se 'Dispose(disposing As Boolean)' contiene codice per liberare risorse non gestite
    '' Protected Overrides Sub Finalize()
    ''     ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
    ''     Dispose(disposing:=False)
    ''     MyBase.Finalize()
    '' End Sub

    'Public Sub Dispose() Implements IDisposable.Dispose
    '    ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
    '    Dispose(disposing:=True)
    '    GC.SuppressFinalize(Me)
    'End Sub
End Class
