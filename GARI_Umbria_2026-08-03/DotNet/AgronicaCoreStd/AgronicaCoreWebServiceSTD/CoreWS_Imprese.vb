Public Class CoreWS_Imprese
    Inherits APICallsBasic

    Public tipo As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal tipo As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.tipo = tipo

    End Sub

End Class
