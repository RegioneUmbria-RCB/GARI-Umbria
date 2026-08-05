Public Class CoreWS_Gis(Of T)
    Inherits APICallsBasic

    Public InData As T

    Public Sub New(ByVal objP_super_server As String, ByVal objP_server As String, ByVal objP_utenti As String, ByVal inGisDataReadParam As T)

        MyBase.New(objP_super_server, objP_server, objP_utenti)

        Me.InData = inGisDataReadParam

    End Sub

End Class
