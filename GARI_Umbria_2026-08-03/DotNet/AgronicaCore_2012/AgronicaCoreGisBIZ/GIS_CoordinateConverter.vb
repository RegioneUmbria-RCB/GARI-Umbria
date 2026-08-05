Imports AgronicaCoreDataProvider
Imports System.Data
Imports System.Collections.Generic
Imports NetTopologySuite.Geometries
Imports ProjNet.CoordinateSystems
Imports ProjNet.CoordinateSystems.Transformations




Public Class GIS_CoordinateConverter

    ' Queste costanti contengono la definizione in formato WKT per due sistemi di riferimento spaziale. 

    ' WGS 84 (EPSG: 4326): È il sistema di coordinate geografiche standard usato dal GPS.
    ' Le sue unità sono i gradi angolari (latitudine e longitudine).
    ' GEOGCS = Geographic Coordinate System
    Private Const WGS_84 As String = "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"

    ' WGS 84 / Pseudo-Mercator (EPSG: 3857): È un sistema di coordinate proiettato (una mappa "piatta").
    ' È lo standard usato dalla maggior parte delle mappe web (Google Maps, OpenStreetMap).
    ' Le sue unità sono i metri.
    ' PROJCS = Projected Coordinate System
    Private Const WGS_84_PseudoMercator As String = "PROJCS[""WGS_1984_Web_Mercator_Auxiliary_Sphere"",GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Mercator_Auxiliary_Sphere""],PARAMETER[""False_Easting"",0.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",0.0],PARAMETER[""Standard_Parallel_1"",0.0],PARAMETER[""Auxiliary_Sphere_Type"",0.0],UNIT[""Meter"",1.0]]"



    ' Funzione "Factory" che costruisce un oggetto sistema di coordinate a partire dal suo codice EPSG.
    ' <param name="id_code">Il codice numerico EPSG (es. 4326).</param>
    ' <returns>Un oggetto ICoordinateSystem che la libreria ProjNet può usare.</returns>
    Private Shared Function GetCoordinateSystemFromEPSG(ByVal id_code As Integer) As GeoAPI.CoordinateSystems.ICoordinateSystem
        ' Crea un'istanza della "fabbrica" di sistemi di coordinate.
        Dim csfactory = New CoordinateSystemFactory()

        ' Controlla il codice EPSG in input e restituisce l'oggetto corrispondente.
        Select Case id_code
            Case 4326
                Return csfactory.CreateFromWkt(WGS_84)

            Case 3857
                Return csfactory.CreateFromWkt(WGS_84_PseudoMercator)

            Case Else ' Se il codice non è tra quelli che conosciamo
                Throw New Exception("EPSG Code non mappato") ' lancia un errore per segnalare il problema.
        End Select
    End Function


    ' Esegue la trasformazione matematica di un array di coordinate da un sistema a un altro.
    ' <param name="coordinates">L'array di coordinate da convertire.</param>
    ' <param name="srOri">Il codice EPSG di ORIGINE.</param>
    ' <param name="srDest">Il codice EPSG di DESTINAZIONE.</param>
    ' <returns>Un nuovo array di coordinate con i valori trasformati.</returns>
    Public Shared Function GetTransformCoordinatesVectors(
    ByVal coordinates As NetTopologySuite.Geometries.Coordinate(),
    ByVal srOri As Integer,
    ByVal srDest As Integer
) As NetTopologySuite.Geometries.Coordinate()

        ' Crea una lista vuota per i risultati.
        Dim ret As New List(Of NetTopologySuite.Geometries.Coordinate)

        ' Crea l'oggetto per le trasformazioni.
        Dim trf = New CoordinateTransformationFactory()

        Try
            ' Crea l'oggetto che sa come passare da srOri a srDest.
            'Dim tr As GeoAPI.CoordinateSystems.Transformations.ICoordinateTransformation = trf.CreateFromCoordinateSystems(GetCoordinateSystemFromEPSG(srOri), GetCoordinateSystemFromEPSG(srDest))
            Dim tr = trf.CreateFromCoordinateSystems(
            IIf(srOri <> 3857, GetCoordinateSystemFromEPSG(srOri), ProjectedCoordinateSystem.WebMercator),
            IIf(srDest <> 3857, GetCoordinateSystemFromEPSG(srDest), ProjectedCoordinateSystem.WebMercator)
            )

            Dim transformedPoint = tr.MathTransform.TransformList(ConvCoordinateToPointList(coordinates))

            ' Inizia un ciclo su ogni punto che è stato trasformato.
            For Each point In transformedPoint
                ' Aggiunge un nuovo oggetto Coordinate alla lista 'ret',
                ' copiando le proprietà X e Y dal punto trasformato.
                ret.Add(New Coordinate With {
                .X = point.X,
                .Y = point.Y
            })
            Next

        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try

        Return ret.ToArray()
    End Function

    Private Shared Function ConvCoordinateToPointList(ByVal coordinates As NetTopologySuite.Geometries.Coordinate()) As List(Of GeoAPI.Geometries.Coordinate)
        Dim ret As New List(Of GeoAPI.Geometries.Coordinate)
        For Each point In coordinates
            ret.Add(New GeoAPI.Geometries.Coordinate(point.X, point.Y))
        Next
        Return ret
    End Function

End Class