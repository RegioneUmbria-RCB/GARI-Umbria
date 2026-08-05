Imports System.Threading.Tasks
Imports AgronicaCoreDataProvider
Imports AgronicaCoreGisDAL
Imports Newtonsoft.Json
Imports AgronicaCoreModelsSTD.Gis
Imports AgronicaCoreDTOStd
Imports InData.Gis
Imports NetTopologySuite.IO
Imports ProjNet.CoordinateSystems
Imports ProjNet.CoordinateSystems.Transformations
Imports NetTopologySuite.Geometries
Public Class Gis_ClusteringCanoPy



    ' <summary>
    ' Esegue l'algoritmo di clustering Canopy su un elenco di centroidi per generare una lista di cluster.
    ' </summary>
    ' <param name="elencoCentroidi">Un DataTable con i centroidi da processare.</param>
    ' <param name="thresholdT1">La soglia di distanza (T1) per il clustering, in metri.</param>
    ' <param name="thresholdT2">La soglia di distanza (T2) per il clustering, in metri.</param>
    ' <returns>Una lista di oggetti ClusterItem che rappresentano i cluster generati.</returns>
    Public Function GeneraClusterCanopy(
        ByVal elencoCentroidi As DataTable,
        ByVal thresholdT1 As Double,
        ByVal thresholdT2 As Double
    ) As List(Of ClusterItem)

        Dim nomeRoutine As String = "AgronicaCoreGisBIZ.Gis__Clustering_R.GeneraClusterCanopy()"

        Try
            'CONVERTIRE L'ELENCO DEI CENTROIDI DA WGS84 ANGOLARE A METRICO
            Const SRID_WGS84 As Integer = 4326
            Const SRID_MERCATOR As Integer = 3857 ' WGS84 Pseudo Mercator

            Dim wktReader As New WKTReader()

            Dim punti = New Concurrent.ConcurrentBag(Of (EntitaCod As Integer, LayerCod As Integer, PuntoWGS84 As Point, PuntoMetrico As Point))()

            Dim opts = New ParallelOptions() With {
               .MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount\2)
            }

            Parallel.ForEach(elencoCentroidi.AsEnumerable(), opts, Sub(row)
                Try
                    Dim wkt As String = row("Centroide_GeoEntity_WKT").ToString()
                    If String.IsNullOrWhiteSpace(wkt) Then Exit Sub

                    ' converte la stringa wkt in un oggetto Geometry nello specifico Point
                    Dim puntoWGS84 = CType(wktReader.Read(wkt), Point)

                    ' classe helper per trasformare le coordinate
                    'puntoWGS84.Coordinate estrae la cordinata del punto la converte e la mette in un array
                    Dim coordinateTrasformate = GIS_CoordinateConverter.GetTransformCoordinatesVectors({puntoWGS84.Coordinate}, SRID_WGS84, SRID_MERCATOR)

                    If coordinateTrasformate IsNot Nothing AndAlso coordinateTrasformate.Length > 0 Then
                        Dim puntoMetrico As New Point(coordinateTrasformate(0)) With {.SRID = SRID_MERCATOR}
                        punti.Add((
                            CInt(row("Entita_Cod")),
                            CInt(row("LayerElementoGrafico_Cod")),
                            puntoWGS84,
                            puntoMetrico
                        ))
                    End If

                Catch
                End Try
            End Sub)

            ' SUDDIVIDERE PER LAYER 
            ' Raggruppamento dei punti in base al loro LayerElementoGrafico_Cod.
            Dim puntiPerLayer = punti.GroupBy(Function(p) p.LayerCod)

            ' la lista finale che conterrà i cluster di tutti i layer.
            Dim outputFinale As New List(Of ClusterItem)()

            ' APPLICARE L'ALGORITMO CANOPY (per singolo layer) 
            For Each gruppoLayer In puntiPerLayer

                ' La lista dei punti da processare per questo specifico layer.
                Dim puntiDaProcessare As New HashSet(Of (EntitaCod As Integer, LayerCod As Integer, PuntoWGS84 As Point, PuntoMetrico As Point))(gruppoLayer)
                ' La lista che conterrà i cluster trovati in questo layer.
                Dim clusterPerQuestoLayer As New List(Of List(Of (EntitaCod As Integer, LayerCod As Integer, PuntoWGS84 As Point, PuntoMetrico As Point)))()

                ' Dato l'elenco dei punti iniziale
                While puntiDaProcessare.Count > 0
                    ' Estrarre il primo punto dalla lista e rimuoverlo
                    Dim centroideCluster = puntiDaProcessare.First()
                    puntiDaProcessare.Remove(centroideCluster)

                    ' Utilizzare il punto appena tolto come inizio del nuovo cluster
                    Dim nuovoCluster As New List(Of (EntitaCod As Integer, LayerCod As Integer, PuntoWGS84 As Point, PuntoMetrico As Point))() From {centroideCluster}

                    Dim daRimuovere As New List(Of (Integer, Integer, Point, Point))()
                    For Each p In puntiDaProcessare

                        ' Calcola la distanza tra i punti (in metri) usando il metodo .Distance di NetTopologySuite.
                        Dim distanza As Double = centroideCluster.PuntoMetrico.Distance(p.PuntoMetrico)

                        ' Se la distanza è < T1, il punto appartiene al cluster
                        If distanza < thresholdT1 Then
                            nuovoCluster.Add(p)

                            ' Se la distanza è anche < T2, il punto viene rimosso dalla lista dei candidati
                            If distanza < thresholdT2 Then
                                daRimuovere.Add(p)
                            End If
                        End If
                    Next
                    
                    For Each p In daRimuovere
                        puntiDaProcessare.Remove(p)
                    Next

                    ' Aggiunge il cluster appena creato alla lista dei cluster di questo layer
                    clusterPerQuestoLayer.Add(nuovoCluster)
                End While

                ' GENERARE L'OGGETTO DI OUTPUT PER QUESTO LAYER 
                For Each cluster In clusterPerQuestoLayer
                    Dim semeCluster = cluster(0)

                    outputFinale.Add(New ClusterItem() With {
                        .Id = semeCluster.EntitaCod,
                        .Wkt_Centroid = semeCluster.PuntoWGS84.ToText(),
                        .ElementCount = cluster.Count
                    })
                Next
            Next

            Return outputFinale

        Catch ex As Exception
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message, ex)
        End Try
    End Function
End Class
