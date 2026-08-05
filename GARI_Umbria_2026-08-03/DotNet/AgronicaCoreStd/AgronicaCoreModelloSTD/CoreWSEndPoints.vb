Public Class CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoInData

    Public Property ChiaveAlbero As String
    Public Property hiddenPunti_Nuovo As String
    Public Property Area As String
    Public Property ElementoGrafico_Des As String
End Class

Public Class CoreWSGisEndPoints_SalvaNuovoElementoGraficoDaChiaveAlberoOutData

    Public Property Messaggio As String

End Class



Public Class CoreWSGisEndPoints_ModificaImpianto2019InData
    Public Property entita_cod As String
    Public Property nome_appezza As String
    Public Property sup_imp As String
    Public Property data_inizio As String
    Public Property data_fine As String
    Public Property specie As String
    Public Property varieta As String
    Public Property finalita As String
    Public Property tipologia As String
    Public Property via_stringa As String
    Public Property hiddenPunti_modifica As String
    Public Property lotto As String
    Public Property codice_socio As String
    Public Property CodiciAnagrafeAggiuntivi As String
    Public Property ModificaAnagrafica As Boolean
End Class

Public Class CoreWSGisEndPoints_ModificaImpianto2019OutData


    Public Property Messaggio As String


End Class


Public Class STBufferGeoJsonPolygonInData
    Public Property Metri As Integer
    Public Property PolygonGeoJson As GeoJson_Geometry

End Class

Public Class STBufferGeoJsonPolygonOutData
    Public Property PolygonGeoJson As GeoJson_Geometry
End Class