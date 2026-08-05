Public Class CoreWS_Parco_Macchine_BTM
    Inherits APICallsBasic

    Public codice As Integer
    Public VIN As String
    Public BTM_Serial As String

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal codice As Integer, ByVal VIN As String, ByVal BTM_Serial As String)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.codice = codice
        Me.VIN = VIN
        Me.BTM_Serial = BTM_Serial

    End Sub
End Class
