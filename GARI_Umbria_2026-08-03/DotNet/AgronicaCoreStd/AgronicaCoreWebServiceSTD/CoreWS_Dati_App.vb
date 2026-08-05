Public Class CoreWS_Dati_App
    Inherits APICallsBasic

    Public tipo As String
    Public piva As String
    Public data As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal tipo As String, ByVal piva As String, ByVal data As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.tipo = tipo
        Me.piva = piva
        Me.data = data

    End Sub
End Class
