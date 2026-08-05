Public Class CoreWS_Utenti_R
    Inherits APICallsBasic

    Public CodiceFiscale As String
    Public MinutiValiditaLoginMemorizzato As Integer

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal CodiceFiscale As String, ByVal MinutiValiditaLoginMemorizzato As Integer)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.CodiceFiscale = CodiceFiscale
        Me.MinutiValiditaLoginMemorizzato = MinutiValiditaLoginMemorizzato

    End Sub


End Class
