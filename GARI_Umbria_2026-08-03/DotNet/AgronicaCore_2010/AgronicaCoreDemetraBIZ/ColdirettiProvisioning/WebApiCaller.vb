Imports System.Net.Http
Imports System.Web
Imports System.Text
Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider

Public Class WebApiCaller
    Inherits LogProvider
    Implements IDisposable

    Private _caller As String
    Private Property _hc As HttpClient
    Private Property _key As Tuple(Of String, String) = Nothing
    Private _ObjParametriServer As AgronicaCoreParametri

    Public Sub New(ByVal baseUrl As String, ByVal token As Tuple(Of String, String), ByVal caller As String, ByRef ObjParametriServer As AgronicaCoreParametri)
        _hc = New HttpClient()
        If token IsNot Nothing Then
            _key = token
        End If
        _caller = caller
        _hc.BaseAddress = New Uri(baseUrl)
        _ObjParametriServer = ObjParametriServer
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
            If resp.IsSuccessStatusCode = False Then
                Select Case _caller
                    Case NameOf(ColdirettiProvisioner)
                        Try
                            Dim err = JsonConvert.DeserializeObject(Of ProvisioningResponseError)(resp.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                            Throw New ColdirettiPDSException("Errore in chiamata a Coldiretti Provisioner" & vbCrLf & "Request: " & vbCrLf & _hc.BaseAddress.AbsoluteUri + endpoint + BuildQueryString(QueryParams) & vbCrLf & "Result: " & err.errors(0).status.ToString() + " - " + err.errors(0).detail)
                        Catch ex As JsonException
                            Scrivi_LOG(_ObjParametriServer, "Coldiretti Provisioner caller - JsonException", getStackErrors(ex))
                            Throw New ColdirettiPDSException("Errore in chiamata a Coldiretti Provisioner " & vbCrLf & "Problema in decodifica risposta provisioning. Formato non riconosciuto")
                        Catch ex As Exception
                            Scrivi_LOG(_ObjParametriServer, "Coldiretti Provisioner caller - Exception", getStackErrors(ex))
                            Throw New ColdirettiPDSException("Errore in chiamata a Coldiretti Provisioner " & vbCrLf & "Errore generico. controllare il log")
                        End Try
                End Select

            Else
                strResp = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            End If

            Select Case GetType(T)
                Case GetType(Boolean)
                    Dim r As Object = resp.IsSuccessStatusCode
                    Return CType(r, T)
                Case Else
                    Return JsonConvert.DeserializeObject(Of T)(resp.IsSuccessStatusCode)
            End Select

        Catch ex As ColdirettiPDSException
            Throw New ColdirettiPDSException(ex.Message, ex)
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

    Private Function getStackErrors(ByVal ex As Exception)
        Dim ret As String = ""
        If ex IsNot Nothing Then
            ret &= ex.Message & vbCrLf
            If ex.InnerException IsNot Nothing Then
                getInnerError(ret, ex.InnerException)
            End If
        End If
        ret &= ex.StackTrace
        Return ret
    End Function
    Private Sub getInnerError(ByRef msg As String, ByVal ex As Exception)
        If ex IsNot Nothing Then
            msg &= ex.Message & vbCrLf
            If ex.InnerException IsNot Nothing Then
                getInnerError(msg, ex.InnerException)
            End If
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        'Throw New NotImplementedException()
    End Sub
End Class
