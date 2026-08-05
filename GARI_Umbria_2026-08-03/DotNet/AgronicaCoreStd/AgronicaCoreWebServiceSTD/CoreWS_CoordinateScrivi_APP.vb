Public Class CoreWS_CoordinateScrivi_APP
    Inherits APICallsBasic

    Public CoordinateDaMemorizzare As AgronicaCoreModelloSTD.Coordinate

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal CoordinateDaMemorizzare As AgronicaCoreModelloSTD.Coordinate)
        MyBase.New(objP_super_server, objP_server, objP_utenti)
        Me.CoordinateDaMemorizzare = CoordinateDaMemorizzare
    End Sub

End Class
