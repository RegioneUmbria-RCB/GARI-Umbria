Public Class CoreWS_LogEventiScrivi_APP
    Inherits APICallsBasic

    Public EntrateUsciteDaMemorizzare As AgronicaCoreModelloSTD.EntrateUscite

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal EntrateUsciteDaMemorizzare As AgronicaCoreModelloSTD.EntrateUscite)
        MyBase.New(objP_super_server, objP_server, objP_utenti)
        Me.EntrateUsciteDaMemorizzare = EntrateUsciteDaMemorizzare
    End Sub


End Class
