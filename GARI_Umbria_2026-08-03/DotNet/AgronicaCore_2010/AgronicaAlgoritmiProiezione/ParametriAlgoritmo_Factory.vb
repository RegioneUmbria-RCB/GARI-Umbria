Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Public Class ParametriAlgoritmo_Factory

    Public Function CreaParametri(Of T)(ByVal TipoAlgoritmo_Cod As Int32) As T

        Select Case TipoAlgoritmo_Cod
            Case TipiEnumerativi.enum_TipoAlgoritmoProiezione.Intersezione
                Return CType(CType(New Input_Intersezione_GEE, Object), T)
            Case TipiEnumerativi.enum_TipoAlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE,
                 TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE,
                 TipiEnumerativi.enum_TipoAlgoritmoProiezione.Proiezione_Vettoriale_su_Raster,
                 TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync,
                 TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly
                Return CType(CType(New Richiesta_Accodamento_GEE, Object), T)
            Case TipiEnumerativi.enum_TipoAlgoritmoProiezione.Richiamo_WS_Mappe
                Return CType(CType(New Richieste_WS_Mappe, Object), T)
            Case Else
                Throw New Exception("Tipologia Algoritmo non riconosciuto.")
        End Select

    End Function

End Class
