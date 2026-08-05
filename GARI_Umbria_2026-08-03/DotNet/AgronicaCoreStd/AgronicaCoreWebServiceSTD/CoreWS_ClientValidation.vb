Public Class CoreWS_ClientValidation
    Inherits APICallsBasic
    Public Property xAppName As String
    Public Property xAppVersion As String
    Public Property xPlatform As String
    Public Property xEnvironment As String
    Public Sub New(
        ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String,
        xAppName As String,
        xAppVersion As String,
        xPlatform As String,
        xEnvironment As String
        )

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.xAppName = xAppName
        Me.xAppVersion = xAppVersion
        Me.xPlatform = xPlatform
        Me.xEnvironment = xEnvironment

    End Sub
End Class
