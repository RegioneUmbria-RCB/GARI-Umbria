Imports AgronicaCoreVarieBiz
Imports AgronicaCoreWebService

Public Class APICallsBasic
    Implements iAPICallsBasic

    Public objP_super_server As String
    Public objP_server As String
    Public objP_utenti As String



    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String)
        Me.objP_super_server = objP_super_server
        Me.objP_server = objP_server
        Me.objP_utenti = objP_utenti

    End Sub

End Class
