Public Class CoreWS_Contatti
    Inherits APICallsBasic

    Public piva As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva

    End Sub
End Class
