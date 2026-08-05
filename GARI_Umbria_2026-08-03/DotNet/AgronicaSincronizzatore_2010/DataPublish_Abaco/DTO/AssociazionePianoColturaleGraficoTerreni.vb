Public Class AppezzamentoTerreno
    Public Property id_appezzamento_padre As String
    Public Property id_appezzamento As String
    Public Property istat_prov As String
    Public Property istat_comu As String
    Public Property sezione As String
    Public Property foglio As String
    Public Property particella As String
    Public Property subalterno As String
    Public Property ultimo_aggiornamento As String

    Public Function GetKeyParticella() As String
        Return istat_prov + "/" + istat_comu + "/" + IIf(sezione Is Nothing, "", sezione) + "/" + foglio + "/" + particella + "/" + subalterno
    End Function

    Public Function GetKey() As String
        Return id_appezzamento + "/" + istat_prov + "/" + istat_comu + "/" + IIf(sezione Is Nothing, "", sezione) + "/" + foglio + "/" + particella + "/" + subalterno
    End Function
End Class

Public Class AppezzamentiTerreni
    Public Property nextKey As String
    Public Property records As List(Of AppezzamentoTerreno)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of AppezzamentoTerreno)
    End Sub
End Class

Public Class AppezzamentoTerrenoSync
    Inherits AppezzamentoTerreno
    Public Property cuaa As String
    Public Property campagna As Int64
    Public Property tipo_modifica As String
    Public Property data_eliminazione As String
End Class
Public Class AppezzamentoTerrenoChangeLog
    Public Property nextKey As String
    Public Property records As List(Of AppezzamentoTerrenoSync)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of AppezzamentoTerrenoSync)
    End Sub
End Class