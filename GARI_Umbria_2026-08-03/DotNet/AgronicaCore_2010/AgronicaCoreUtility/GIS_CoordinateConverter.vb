Imports AgronicaCoreDataProvider
Imports System.Data
Imports System.Collections.Generic
'Imports GisSharpBlog
'Imports GisSharpBlog.NetTopologySuite.CoordinateSystems
'Imports GisSharpBlog.NetTopologySuite.CoordinateSystems.Transformations
'Imports GisSharpBlog.NetTopologySuite.IO
Imports NetTopologySuite.Geometries
Imports GeoAPI.CoordinateSystems
Imports GeoAPI.Geometries




Public Class GIS_CoordinateConverter

    ' --- DEFINIZIONE DELLE "RICETTE" PER I SISTEMI DI COORDINATE ---
    ' Queste costanti contengono la definizione testuale completa in formato WKT (Well-Known Text)
    ' per due sistemi di riferimento spaziale. La libreria ProjNet userà queste "ricette"
    ' per capire le regole matematiche di ogni sistema.

    ' WGS 84 (EPSG: 4326): È il sistema di coordinate geografiche standard usato dal GPS.
    ' Le sue unità sono i gradi angolari (latitudine e longitudine).
    ' GEOGCS = Geographic Coordinate System
    Private Const WGS_84 As String = "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"

    ' WGS 84 / Pseudo-Mercator (EPSG: 3857): È un sistema di coordinate proiettato (una mappa "piatta").
    ' È lo standard usato dalla maggior parte delle mappe web (Google Maps, OpenStreetMap).
    ' Le sue unità sono i metri.
    ' PROJCS = Projected Coordinate System
    Private Const WGS_84_PseudoMercator As String = "PROJCS[""WGS_1984_Web_Mercator_Auxiliary_Sphere"",GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]],PROJECTION[""Mercator_Auxiliary_Sphere""],PARAMETER[""False_Easting"",0.0],PARAMETER[""False_Northing"",0.0],PARAMETER[""Central_Meridian"",0.0],PARAMETER[""Standard_Parallel_1"",0.0],PARAMETER[""Auxiliary_Sphere_Type"",0.0],UNIT[""Meter"",1.0]]"


    ''' <summary>
    ''' Funzione "Factory" che costruisce un oggetto sistema di coordinate a partire dal suo codice EPSG.
    ''' </summary>
    ''' <param name="id_code">Il codice numerico EPSG (es. 4326).</param>
    ''' <returns>Un oggetto ICoordinateSystem che la libreria ProjNet può usare.</returns>
    Private Shared Function GetCoordinateSystemFromEPSG(ByVal id_code As Integer) As GeoAPI.CoordinateSystems.ICoordinateSystem
        ' Crea un'istanza della "fabbrica" di sistemi di coordinate.
        Dim csfactory = New CoordinateSystemFactory()

        ' Controlla il codice EPSG in input e restituisce l'oggetto corrispondente.
        Select Case id_code
            Case 4326 ' Se il codice è 4326 (WGS 84)...
                Return csfactory.CreateFromWkt(WGS_84) ' ...costruisci il sistema usando la "ricetta" WKT di WGS 84.

        ' (I seguenti casi sono esempi di come estendere la funzione)
            Case 32632
            ' Return csfactory.CreateFromWkt(UTWGS_84_UTM_zone_32N)
            Case 32633
            ' Return csfactory.CreateFromWkt(UTWGS_84_UTM_zone_33N)
            Case 3003
            ' Return csfactory.CreateFromWkt(MonteMario_Italy_Zone_1_3003)
            Case 3004
            ' Return csfactory.CreateFromWkt(MonteMario_Italy_Zone_2_3004)

            Case 3857 ' Se il codice è 3857 (Pseudo Mercator)...
                Return csfactory.CreateFromWkt(WGS_84_PseudoMercator) ' ...costruisci il sistema usando la "ricetta" WKT di Pseudo Mercator.

            Case Else ' Se il codice non è tra quelli che conosciamo...
                Throw New Exception("EPSG Code non mappato") ' ...lancia un errore per segnalare il problema.
        End Select
    End Function


    ''' <summary>
    ''' Esegue la trasformazione matematica di un array di coordinate da un sistema a un altro.
    ''' </summary>
    ''' <param name="coordinates">L'array di coordinate da convertire.</param>
    ''' <param name="srOri">Il codice EPSG di ORIGINE.</param>
    ''' <param name="srDest">Il codice EPSG di DESTINAZIONE.</param>
    ''' <returns>Un nuovo array di coordinate con i valori trasformati.</returns>
    ' Questa è la firma del metodo. Prende un array di coordinate e due interi (gli SRID).
    ' Promette di restituire un NUOVO array di coordinate.
    Private Shared Function GetTransformCoordinatesVectors(
    ByVal coordinates As NetTopologySuite.Geometries.Coordinate(),
    ByVal srOri As Integer,
    ByVal srDest As Integer
) As NetTopologySuite.Geometries.Coordinate() ' <-- NOTA 1: Il tipo di ritorno è un Array

        ' Crea una lista vuota per accumulare i risultati.
        Dim ret As New List(Of NetTopologySuite.Geometries.Coordinate)

        ' Crea l'oggetto "fabbrica" per le trasformazioni.
        Dim trf = New CoordinateTransformationFactory()

        Try
            ' Crea l'oggetto "trasformatore" che sa come passare da srOri a srDest.
            Dim tr = trf.CreateFromCoordinateSystems(GetCoordinateSystemFromEPSG(srOri), GetCoordinateSystemFromEPSG(srDest))

            ' --- RIGA CHIAVE E PROBLEMATICA ---
            ' Questa è la parte più confusa dell'esempio.
            Dim transformedPoint = tr.MathTransform.TransformList(ConvCoordinateToPointList(coordinates))

            ' Inizia un ciclo su ogni punto che è stato trasformato.
            For Each point In transformedPoint
                ' Aggiunge un nuovo oggetto Coordinate alla lista 'ret',
                ' copiando le proprietà X e Y dal punto trasformato.
                ret.Add(New NetTopologySuite.Geometries.Coordinate With {
                .X = point.X,
                .Y = point.Y
            })
            Next

        Catch ex As Exception
            ' In caso di errore, imposta la lista dei risultati a Nothing (nullo).
            ret = Nothing
            ' Rilancia l'eccezione, comunicando il fallimento.
            Throw New Exception(ex.Message, ex)
        End Try

        ' --- RIGA CHIAVE E PROBLEMATICA ---
        ' Converte la lista 'ret' in un array prima di restituirla, per rispettare la firma del metodo.
        Return ret.ToArray()
    End Function

End Class