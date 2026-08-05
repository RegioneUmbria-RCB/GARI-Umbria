Imports System.Net.Http
Imports Agronica.Helpers.OAuth2

Public Class ColdirettiIdentityProvider
    Inherits WebApiCaller
    Implements IDisposable

    Private Const ENDPOINT_GETUSER As String = "/admin/realms/{0}/users"
    Private Const ENDPOINT_GETTOKEN As String = "/realms/{0}/protocol/openid-connect/token"

    Public Sub New(ByVal baseUrl As String,
                   ByVal client_id As String,
                   ByVal grant_type As String,
                   ByVal username As String,
                   ByVal password As String)
        MyBase.New(baseUrl, Nothing, NameOf(ColdirettiIdentityProvider))

        GetKeyCloackGetToken("master", client_id, grant_type, username, password)
    End Sub

    Private Sub GetKeyCloackGetToken(ByVal realm As String,
                                     ByVal client_id As String,
                                     ByVal grant_type As String,
                                     ByVal username As String,
                                     ByVal password As String)

        Dim pars = New Dictionary(Of String, String)
        pars.Add("client_id", client_id)
        pars.Add("grant_type", grant_type)
        pars.Add("username", username)
        pars.Add("password", password)

        Dim token = ChiamaWS(Of OAuth2Token)(HttpMethod.Post, Nothing, pars, "", "x-wwww-form-urlencoded", getUrlEndpoint(ENDPOINT_GETTOKEN, realm), False, True)
        If token IsNot Nothing Then
            UpdateKey(New Tuple(Of String, String)(token.token_type, token.access_token))
        Else
            Throw New ColdirettiIdentityProviderException("Errore in recupero token di autenticazione key cloack")
        End If
        Return
    End Sub

    Public Function GetKeyCloackUserMail(ByVal realm As String, ByVal user As String) As String
        Dim ret As String = ""
        Dim pars = New Dictionary(Of String, String)
        pars.Add("username", user)
        Dim users = ChiamaWS(Of List(Of UserRappresentation))(HttpMethod.Get, pars, Nothing, "", "", getUrlEndpoint(ENDPOINT_GETUSER, realm))
        If users IsNot Nothing Then
            For Each usr In users
                If usr.username = user Then
                    ret = usr.email
                    Exit For
                End If
            Next
            If ret = "" Then
                Throw New ColdirettiIdentityProviderException(String.Format("Nessun utente trovato per {0}", user))
            End If
        Else
            Throw New ColdirettiIdentityProviderException(String.Format("Nessun utente trovato per {0}", user))
        End If
        Return ret
    End Function

    Private Function getUrlEndpoint(ByVal Endpoint As String, ByVal realm As String) As String
        Return String.Format(Endpoint, realm)
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        MyBase.Dispose()
    End Sub

End Class
