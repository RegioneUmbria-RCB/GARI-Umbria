Imports System.Net
Imports System.Net.Http
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json

Public Class ReUmbriaApiProvider
    Inherits WebApiCaller
    Implements IDisposable

    Private _loginUrl As String
    Private _username As String
    Private _password As String
    Private _pendenzeUrl As String
    Private _epsg As String
    Private _usePostMethod As Boolean

    Private MaxUnauthorizedResults = 1
    Private MaxTimeoutRequestResults = 1

    Public Sub New(ByVal baseUrl As String,
                   ByVal loginUrl As String,
                   ByVal username As String,
                   ByVal password As String,
                   ByVal pendenzeUrl As String,
                   ByVal epsg As String,
                   ByVal usePostMethod As Boolean)
        MyBase.New(baseUrl, Nothing, NameOf(ReUmbriaApiProvider))

        _loginUrl = loginUrl
        _username = username
        _password = password
        _pendenzeUrl = pendenzeUrl
        _epsg = epsg
        _usePostMethod = usePostMethod

        GetApiToken()
    End Sub

    Public Function LeggiPendenza(ByVal strPolygon As String, ByRef ResponseStatusCode As Integer) As AgronicaCoreDTOStd.InData.DataExchange.Pendenza
        Dim result = Nothing

        Dim pars As Dictionary(Of String, String) = Nothing
        Dim body As String = Nothing

        If _usePostMethod Then

            body = JsonConvert.SerializeObject(New With {.wkt = strPolygon, .epsg = _epsg})
        Else

            pars = New Dictionary(Of String, String)
            pars.Add("geom", strPolygon)
            pars.Add("epsg", _epsg)
        End If

        Dim UnauthorizedResultCount = 0
        Dim TimeoutRequestResultCount = 0
        Dim chiamataEffettuata = False
        ResponseStatusCode = 0

        While Not chiamataEffettuata
            Try
                'TODO: eliminare la riga sotto, il token richiesto solo se Unauthorized è ritornato e non prima di ogni chiamata 
                GetApiToken()

                If _usePostMethod Then

                    result = ChiamaWS(Of AgronicaCoreDTOStd.InData.DataExchange.Pendenza)(HttpMethod.Post, Nothing, Nothing, body, "application/json", _pendenzeUrl, False, False, False, ResponseStatusCode)
                Else

                    result = ChiamaWS(Of AgronicaCoreDTOStd.InData.DataExchange.Pendenza)(HttpMethod.Get, pars, Nothing, "", "application/json", _pendenzeUrl, False, False, True, ResponseStatusCode)
                End If

                chiamataEffettuata = True

            Catch ex As ReUmbriaApiProviderException
                Select Case ResponseStatusCode
                    Case HttpStatusCode.Unauthorized
                        'If UnauthorizedResultCount < MaxUnauthorizedResults Then
                        '    GetApiToken()
                        '    UnauthorizedResultCount += 1
                        'Else
                        '    chiamataEffettuata = True
                        '    Throw ex
                        'End If

                        'TODO: eliminare le due righe sotto e scommentare quelle sopra, il token andrebbe richiesto ad ogni Unauthorized code ritornato 
                        chiamataEffettuata = True
                        Throw ex

                    Case ResponseStatusCode = HttpStatusCode.RequestTimeout, HttpStatusCode.GatewayTimeout
                        If TimeoutRequestResultCount < MaxUnauthorizedResults Then
                            TimeoutRequestResultCount += 1
                        Else
                            chiamataEffettuata = True
                            Throw New ReUmbriaApiTimeoutException(ex.Message, ex)
                        End If

                    Case Else
                        chiamataEffettuata = True
                        Throw ex
                End Select
            End Try
        End While

        Return result
    End Function

    Private Sub GetApiToken()
        Dim bodyreq As New With {Key .username = _username, Key .password = _password}
        Dim bodyreqStr = JsonConvert.SerializeObject(bodyreq)

        Dim token = ChiamaWS(Of ApiToken)(HttpMethod.Post, Nothing, Nothing, bodyreqStr, "application/json", _loginUrl, False, True, False)
        If token IsNot Nothing Then
            UpdateKey(New Tuple(Of String, String)("JWT", token.auth_token))
        Else
            Throw New ReUmbriaApiLoginFailException("Errore in recupero token di autenticazione")
        End If
        Return
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        MyBase.Dispose()
    End Sub

End Class
