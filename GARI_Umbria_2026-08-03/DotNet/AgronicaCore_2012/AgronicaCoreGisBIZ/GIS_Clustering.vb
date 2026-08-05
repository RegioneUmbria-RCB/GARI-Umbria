Imports AgronicaCoreDataProvider
Imports AgronicaCoreGisDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreDTOStd
Imports InData.Gis

Public Class GIS_Clustering_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ' Recupera e deserializza le soglie di clustering per un dato algritmo e livello di zoom.
    Public Function LeggiSoglieClusterAlgorithm(
        ByVal algcod As Integer,
        ByVal zoomLevel As Integer,
        ByRef objParametri As AgronicaCoreParametri
    ) As GISClusterThreasholds
        Dim nomeRoutine As String = "AgronicaCoreGisBIZ.GIS_Clustering_R.LeggiSoglieClusterAlgorithm()"

        Try

            Dim dalSoglie As New GIS_LayerElementiGrafici_Clustering_Threasholds_R()


            Dim dtSoglie As DataTable = dalSoglie.LeggiSoglieClusterAlgorithm(algcod, zoomLevel, objParametri)

            If dtSoglie Is Nothing OrElse dtSoglie.Rows.Count = 0 Then
                Scrivi_LOG(objParametri, nomeRoutine, $"La soglia per il livello di Zoom {zoomLevel} non è stata trovata.")
                Return New GISClusterThreasholds() With {
                    .T1 = -1,
                    .T2 = -1
                }
            End If

            Dim jsonValue As String = dtSoglie.Rows(0)("Value").ToString()

            If String.IsNullOrWhiteSpace(jsonValue) Then
                Scrivi_LOG(objParametri, nomeRoutine, $"La soglia per il livello di Zoom {zoomLevel} è presente ma vuota.")
                Return New GISClusterThreasholds() With {
                    .T1 = -1,
                    .T2 = -1
                }
            End If

            Dim thresholds As GISClusterThreasholds = JsonConvert.DeserializeObject(Of GISClusterThreasholds)(jsonValue)

            Return thresholds

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Function

    ' Recupera un elenco di centroidi pre-calcolati che si trovano 
    ' all'interno di un dato bounding box e appartengono a specifici layer.

    ' <returns>Un DataTable con i centroidi wkt, l'ID del layer e l'ID dell'entità associata.</returns>
    Public Function LeggiCentroidiInBoundingBox(
        ByVal wkt_bbox As String, 'la stringa wkt del poligono che rappresenta la bbox
        ByVal layer_cods As List(Of Integer), 'lista di ID dei layer da includere nella ricerca
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreGisBIZ.GIS_Clustering_R.LeggiCentroidiInBoundingBox()"

        Try

            Dim dalCentroidi As New GIS_ElementiGrafici_Clustering_R()

            Dim dtCentroidi As DataTable = dalCentroidi.LeggiCentroidiInBoundingBox(wkt_bbox, layer_cods, objParametri)

            Return dtCentroidi

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiCentroidiEntitaAnagrafica(
    ByVal piva As String,
    ByVal sa_cod As Integer,
    ByVal appezza As Integer,
    ByVal id_reg As Integer,
    ByVal layer_cods As List(Of Integer),
    ByRef objParametri As AgronicaCoreParametri
) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreGisBIZ.GIS_Clustering_R.LeggiCentroidiEntitaAnagrafica()"

        Try

            Dim dalCentroidi As New GIS_ElementiGrafici_Clustering_R()

            Dim dtCentroidi As DataTable = dalCentroidi.LeggiCentroidiEntitaAnagrafica(piva, sa_cod, appezza, id_reg, layer_cods, objParametri)

            Return dtCentroidi

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class
Public Class GIS_Clustering_W
    Inherits AgronicaCoreDataProvider.DataProvider



End Class