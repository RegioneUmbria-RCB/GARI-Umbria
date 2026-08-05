Public Class CoreWS_VisiteScrivi_APP
    Inherits APICallsBasic

    Public VisiteDaMemorizzare As AgronicaCoreModelloSTD.VisitePerScarico

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal VisiteDaMemorizzare As AgronicaCoreModelloSTD.VisitePerScarico)
        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.VisiteDaMemorizzare = VisiteDaMemorizzare

    End Sub

End Class
