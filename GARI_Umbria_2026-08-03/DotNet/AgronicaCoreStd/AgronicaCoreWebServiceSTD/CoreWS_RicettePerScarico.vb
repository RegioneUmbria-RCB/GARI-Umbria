Public Class CoreWS_RicettePerScarico
    Inherits APICallsBasic

    Public Piva As String
    Public DataDa As String
    Public DataA As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal piva As String, ByVal DataDa As String, ByVal DataA As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.Piva = piva
        Me.DataDa = DataDa
        Me.DataA = DataA

    End Sub





End Class
