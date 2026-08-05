Public Class CoreWS_RicettaScrivi_APP
    Inherits APICallsBasic


    Public RicetteDaMemorizzare As AgronicaCoreModelloSTD.RicettePerScarico


    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal RicettaDaMemorizzare As AgronicaCoreModelloSTD.RicettePerScarico)
        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.RicetteDaMemorizzare = RicettaDaMemorizzare

    End Sub

End Class
