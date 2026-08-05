Public Class CoreWS_Appezzamento
    Inherits APICallsBasic

    Public piva As String
    Public sa_cod As Integer
    Public appezza As Integer

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal sa_cod As Integer, ByVal appezza As Integer)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.piva = piva
        Me.sa_cod = sa_cod
        Me.appezza = appezza

    End Sub
End Class
