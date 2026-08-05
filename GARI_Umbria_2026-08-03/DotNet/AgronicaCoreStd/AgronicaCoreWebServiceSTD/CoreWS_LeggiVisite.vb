Public Class CoreWS_LeggiVisite
    Inherits APICallsBasic

    Public piva As String

    Public Sub New(ByVal piva As String, ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva

    End Sub

End Class
