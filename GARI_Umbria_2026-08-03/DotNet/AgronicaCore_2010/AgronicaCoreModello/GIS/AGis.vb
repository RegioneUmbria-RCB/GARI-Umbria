Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreModelsSTD.metaschema.utilizzi

Public Class AGis

End Class

Public Class GisDataReadParam
    Public Property piva As String
    Public Property sa_cod As String
    Public Property appezza As String
    Public Property id_imp As String
    Public Property TipologiaLayerSelezionata As String
    Public Property wktBoundaySTIntersects As String
    Public Property filtroTemporale As FiltroTemporale
    Public Property filtroTemporaleSingolaData As FiltroTemporale
    Public Property campo_cod As String
    Public Property veg_cod As UtilizzoTerreno
    Public Property cfgAlbero As ConfigurazioneAlbero
    Public Property cfgSementi As SementieriParametrizzazione
    Public Property layerElementiGrafici_cod As List(Of Integer)
    Public Property zoomLevel As Integer
End Class
