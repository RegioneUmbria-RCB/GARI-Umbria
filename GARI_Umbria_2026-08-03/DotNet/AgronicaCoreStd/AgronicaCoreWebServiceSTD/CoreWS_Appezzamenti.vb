Public Class CoreWS_Appezzamenti
    Inherits APICallsBasic

    Public piva As String
    Public Data As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal Data As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva
        Me.Data = Data

    End Sub

End Class
