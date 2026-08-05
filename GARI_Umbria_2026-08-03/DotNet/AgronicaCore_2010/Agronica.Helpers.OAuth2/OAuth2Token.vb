Imports Newtonsoft.Json

Public Class OAuth2Token
    Public Property access_token As String
    Public Property token_type As String
    Public Property expires_in As Long
    Public Property refresh_token As String
    Public Property refresh_expires_in As Long
    <JsonProperty("not-before-policy")>
    Public Property not_before_policy As Long
    Public Property session_state As String
    Public Property scope As String

    Public Sub New()
        access_token = ""
        token_type = ""
        refresh_token = ""
        expires_in = -1
        refresh_expires_in = -1
        not_before_policy = 0
        session_state = ""
        scope = ""
    End Sub
End Class

Public Class OAuth2Error
    <JsonProperty("error")>
    Public Property error_s As String
    Public Property error_description As String
    Public Property error_uri As String

    Public Sub New()
        error_s = ""
        error_description = ""
        error_uri = ""
    End Sub
End Class