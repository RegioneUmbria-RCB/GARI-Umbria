Public Class CoreWS_Attivita
    Inherits APICallsBasic

    Public piva_superuser As String

    Public Sub New(ByVal piva_superuser As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)
        Me.piva_superuser = piva_superuser

    End Sub
End Class
