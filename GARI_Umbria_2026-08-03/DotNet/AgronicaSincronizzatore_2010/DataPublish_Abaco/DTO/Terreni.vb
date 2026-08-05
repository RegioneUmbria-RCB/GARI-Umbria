Public Class Terreno
    Public Property istat_prov As String
    Public Property istat_comu As String
    Public Property sezione As String
    Public Property foglio As String
    Public Property particella As String
    Public Property subalterno As String
    Public Property area_gis_mq As Int64?
    Public Property area_catasto_mq As Int64?
    Public Property area_condotta_mq As Int64?
    Public Property data_inizio_conduzione As String
    Public Property data_fine_conduzione As String
    Public Property codice_titolo As String
    Public Property titolo As String
    Public Property ultimo_aggiornamento As String

    Public Function GetKey() As String
        Return istat_prov + "/" + istat_comu + "/" + IIf(sezione Is Nothing, "", sezione) + "/" + foglio + "/" + particella + "/" + subalterno
    End Function
End Class

Public Class Terreni
    Public Property nextKey As String
    Public Property records As List(Of Terreno)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of Terreno)
    End Sub
End Class

Public Class TerrenoSync
    Inherits Terreno

    Public Property cuua As String
    Public Property tipo_modifica As String
    Public Property data_eliminazione As String
End Class

Public Class TerreniChangeLog
    Public Property nextKey As String
    Public Property records As List(Of TerrenoSync)
    Public Property count As Integer

    Public Sub New()
        records = New List(Of TerrenoSync)
    End Sub
End Class
