Public Class GIS_LayerTiles_R
    ' Aggiornare il file con le chiamate al DAL se ti servono, non cercare un altro modo, teniamo un po' di coerenza nel codice
End Class

Public Class GIS_LayerTiles_W
    ' Aggiornare il file con le chiamate al DAL se ti servono, non cercare un altro modo, teniamo un po' di coerenza nel codice

    Public Function DeleteAllTilesPerLayer(pivaSuperUser As String,
                                           layerCod As Int32,
                                           objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim objLayerTiles As New AgronicaCoreGisDAL.GIS_LayerTiles_W
        Return objLayerTiles.DeleteAllTilesPerLayer(pivaSuperUser, layerCod, objParametriServer)
    End Function
End Class
