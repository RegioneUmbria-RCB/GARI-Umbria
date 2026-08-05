Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Namespace V2
    Public Class ParametriAlgoritmo_Factory

        Public Function CreaParametri(ByVal TipoAlgoritmo_Cod As Int32) As InputGEE

            Select Case TipoAlgoritmo_Cod
                Case TipiEnumerativi.enum_TipoAlgoritmoProiezione.Intersezione
                    Return New Input_Intersezione_GEE
                Case TipiEnumerativi.enum_TipoAlgoritmoProiezione.Caricamento_Su_Piattaforma_GEE,
                     TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE,
                     TipiEnumerativi.enum_TipoAlgoritmoProiezione.Proiezione_Vettoriale_su_Raster,
                     TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync,
                     TipiEnumerativi.enum_TipoAlgoritmoProiezione.Potential_Deforestation_Sync_Yearly
                    Return New Richiesta_Accodamento_GEE
                Case Else
                    Throw New Exception("Tipologia Algoritmo non riconosciuto.")
            End Select

        End Function

    End Class
End Namespace

