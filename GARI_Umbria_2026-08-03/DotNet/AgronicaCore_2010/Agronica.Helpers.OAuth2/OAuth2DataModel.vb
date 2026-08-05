Imports Newtonsoft.Json
Public Class OAuth2DataModel_Config
    Public Property ClientId As String
    Public Property ClientSecret As String
    Public Property WellKnown As String
    Public Property ApiUrl As String
    Public Property Scopes As String
    Public Property State As String
    Public Property ServerUrl As String
    Public Property Callback As String
    <JsonIgnore()>
    Public Property Token As OAuth2Token
    <JsonIgnore()>
    Public Property oAuthMetaData As Dictionary(Of String, Object)
    <JsonIgnore()>
    Public Property AuthToken As String
    Public Property UsernameJD As String


    Public Sub New()
        ClientId = ""
        ClientSecret = ""
        WellKnown = ""
        ApiUrl = ""
        Scopes = ""
        State = ""
        ServerUrl = ""
        Callback = ""
        Token = New OAuth2Token()
        oAuthMetaData = Nothing
        AuthToken = ""
        UsernameJD = ""
    End Sub

    Public Sub SetConnectionParameters(ByVal sClientId As String,
                                       ByVal sClientSecret As String,
                                       ByVal sWellKnown As String,
                                       ByVal sApiUrl As String,
                                       ByVal sScopes As String,
                                       ByVal sState As String,
                                       ByVal sServerUrl As String,
                                       ByVal sCallback As String)
        ClientId = sClientId
        ClientSecret = sClientSecret
        WellKnown = sWellKnown
        ApiUrl = sApiUrl
        Scopes = sScopes
        State = sState
        ServerUrl = sServerUrl
        Callback = sCallback
    End Sub
End Class

Public Class OAuth2_DataModel
    Public Property baseUrl As String
    Public Property wellKnown As String
    Public Property apiUrl As String

End Class

Public Class OAuth2_Metadata
    Public Property issuer As String
    Public Property authorization_endpoint As String
    Public Property token_endpoint As String
    Public Property introspection_endpoint As String
    Public Property userinfo_endpoint As String
    Public Property end_session_endpoint As String
    Public Property frontchannel_logout_session_supported As Boolean
    Public Property frontchannel_logout_supported As Boolean
    Public Property jwks_uri As String
    Public Property check_session_iframe As String
    Public Property grant_types_supported As List(Of String)
    Public Property response_types_supported As List(Of String)
    Public Property subject_types_supported As List(Of String)

End Class