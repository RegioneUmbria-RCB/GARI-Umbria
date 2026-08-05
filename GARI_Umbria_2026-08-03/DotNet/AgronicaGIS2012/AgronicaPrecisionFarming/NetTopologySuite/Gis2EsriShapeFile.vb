Imports NetTopologySuite
Imports NetTopologySuite.Features
Imports NetTopologySuite.IO.Esri.Shapefiles
Imports NetTopologySuite.IO.Esri
Imports System.IO
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json

Public Class Gis2EsriShapeFile

    Private _featureList As List(Of ShapeFileRecord)
    Private _reader As New NetTopologySuite.IO.WKTReader()
    Private _projection As String = "GEOGCS[""GCS_WGS_1984"",DATUM[""D_WGS_1984"",SPHEROID[""WGS_1984"",6378137.0,298.257223563]],PRIMEM[""Greenwich"",0.0],UNIT[""Degree"",0.0174532925199433]]"
    Private _appIdRate As String = "CAP_N+"

    Public Sub New(Optional ByVal appIdRate As String = "", Optional ByVal prj As String = "")
        _featureList = New List(Of ShapeFileRecord)
        If prj <> "" Then
            _projection = prj
        End If
        If appIdRate <> "" Then
            _appIdRate = appIdRate
        End If
    End Sub

    Private Function getGeometryFromWKT(ByVal wkt As String) As Geometries.Geometry
        Dim obj = _reader.Read(wkt)
        Try
            If obj.NumGeometries > 1 Then
                Throw New Exception("GeometryCollection non supported")
            End If
            If Not obj.IsValid() Then
                Throw New Exception("Invalid geometry")
            End If
        Catch ex As Exception
            obj = Nothing
        End Try
        Return obj
    End Function

    Private Sub CheckAndBackupFile(ByVal path As String)
        If File.Exists(path) Then
            Dim fname = System.IO.Path.GetFileName(path)
            Dim dir = System.IO.Path.GetDirectoryName(path)

            File.Move(path, System.IO.Path.Combine(dir, "bkp_" & fname))
        End If
    End Sub

    Public Sub Add(ByVal wkt As String, ByVal attribute As AttributeRecord)
        _featureList.Add(New ShapeFileRecord() With {
                            .feature = New Feature() With {.Geometry = getGeometryFromWKT(wkt)},
                            .attributes = attribute
                         })
    End Sub

    Public Function Read(ByVal index As Integer) As ShapeFileRecord
        If _featureList.Count < index Then
            Throw New Exception("Elemento alla posizione " + index + " non trovato")
        End If
        Return _featureList.Item(index)
    End Function

    Public Sub Delete(ByVal index As Integer)
        _featureList.RemoveAt(index)
    End Sub

    Public Sub Update(ByVal index As Integer, ByVal wkt As String, ByVal attribute As AttributeRecord)
        If _featureList.Count < index Then
            Throw New Exception("Elemento alla posizione " + index + " non trovato")
        End If
        If attribute IsNot Nothing Then
            _featureList.Item(index).attributes = attribute
        End If
        If wkt <> "" Then
            _featureList.Item(index).feature.Geometry = getGeometryFromWKT(wkt)
        End If
    End Sub

    ''' <summary>
    ''' Esportazione su shapefile
    ''' </summary>
    ''' <param name="path">
    ''' percorso di salvataggio del file esportato (viene verificata l'esistenza ed in caso creata la cartella)
    ''' </param>
    ''' <param name="filename">
    ''' nome del file senza estensione
    ''' </param>
    ''' <param name="backup">
    ''' parametro opzionale per le generazione di una copia di backup in caso esista già uno shape file con lo stesso nome
    ''' </param>
    Public Sub ExportToFile(ByVal path As String, ByVal filename As String, Optional ByVal backup As Boolean = False)
        If _featureList.Count <= 0 Then
            Throw New Exception("Nessun dato da esportare")
        End If

        If Not Directory.Exists(path) Then
            Directory.CreateDirectory(path)
        End If
        If backup Then
            CheckAndBackupFile(System.IO.Path.Combine(path, filename & ".dbf"))
            CheckAndBackupFile(System.IO.Path.Combine(path, filename & ".shp"))
            CheckAndBackupFile(System.IO.Path.Combine(path, filename & ".shx"))
            CheckAndBackupFile(System.IO.Path.Combine(path, filename & ".prj"))
        End If

        'definisco la struttura degli attributi
        Dim fldListDef As New List(Of Dbf.Fields.DbfField)

        For Each field In _featureList.Item(0).attributes.Attributes
            Select Case field.Type
                Case AttributeFieldType._Character
                    fldListDef.Add(New Dbf.Fields.DbfCharacterField(field.Nome, field.Length) With {.Value = field.Value})
                Case AttributeFieldType._Date
                    fldListDef.Add(New Dbf.Fields.DbfDateField(field.Nome) With {.Value = field.Value})
                Case AttributeFieldType._Int32
                    fldListDef.Add(New Dbf.Fields.DbfNumericInt32Field(field.Nome) With {.Value = field.Value})
                Case AttributeFieldType._Int64
                    fldListDef.Add(New Dbf.Fields.DbfNumericInt64Field(field.Nome, field.Length) With {.Value = field.Value})
                Case AttributeFieldType._Double
                    fldListDef.Add(New Dbf.Fields.DbfNumericDoubleField(field.Nome, field.Length, field.Precision) With {.Value = field.Value})
                Case AttributeFieldType._Double
                    fldListDef.Add(New Dbf.Fields.DbfFloatField(field.Nome, field.Length, field.Precision) With {.Value = field.Value})
                Case AttributeFieldType._Logical
                    fldListDef.Add(New Dbf.Fields.DbfLogicalField(field.Nome) With {.Value = field.Value})
            End Select
        Next
        Dim writerOpt = New Writers.ShapefileWriterOptions(IO.Esri.ShapeType.Polygon, fldListDef.ToArray())
        writerOpt.Projection = _projection
        Dim wrt = NetTopologySuite.IO.Esri.Shapefile.OpenWrite(System.IO.Path.Combine(path, filename + ".shp"), writerOpt)
        For Each feat In _featureList
            wrt.Write(New Feature(feat.feature.Geometry, Nothing))
        Next
        wrt.Dispose()

    End Sub

    Public Sub ExportShapeFileFromGeoJson(ByVal geojsonstr As String,
                                          ByVal pathExp As String,
                                          ByVal filename As String)
        Dim geojson As GeoJson_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp) = JsonConvert.DeserializeObject(Of GeoJson_New(Of AgronicaCoreModelsSTD.Gis.GeoJSONAgroGisProp))(geojsonstr)

        'metodo puntuale per la definizione e scrittura dello shape file con formattazione attributi
        Dim fldDef = New NetTopologySuite.IO.Esri.Dbf.Fields.DbfNumericDoubleField(_appIdRate, 12, 3)
        Dim writerOpt = New Writers.ShapefileWriterOptions(NetTopologySuite.IO.Esri.ShapeType.Polygon, fldDef)
        writerOpt.Projection = _projection

        Dim wrt = NetTopologySuite.IO.Esri.Shapefile.OpenWrite(Path.Combine(pathExp, filename + ".shp"), writerOpt)
        For Each obj In geojson.geoJsonCaricato.features.Where(Function(x) x.geometry.type.Equals(FeatureType.Polygon.ToString())).ToList()
            fldDef.Value = GetRateFromGeoJsonProperties(obj.properties.AppIdRate)("CAP_N+")
            wrt.Write(New Feature(GetPolygonFromCoordinates(obj.geometry.coordinates), Nothing))

        Next
        wrt.Dispose()
    End Sub


    Private Function GetRateFromGeoJsonProperties(ByVal propValue As String) As Dictionary(Of String, Object)
        Dim ret As New Dictionary(Of String, Object)
        Try
            Dim first = propValue.Split("|")
            Dim second = first(0).Split("§")
            ret.Add(second(0), Math.Round(CType(second(1).Replace(".", Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), Double), 3))
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function

    Private Function GetPolygonFromCoordinates(ByVal coords As Object) As NetTopologySuite.Geometries.Polygon
        Dim polygon As NetTopologySuite.Geometries.Polygon = Nothing
        Try

            Dim shell As NetTopologySuite.Geometries.LinearRing = Nothing
            Dim holes As List(Of NetTopologySuite.Geometries.LinearRing) = Nothing
            Dim coordinate As Double()()() = CType(coords, Double()()())

            Dim i As Integer = 0
            For i = 0 To coordinate.Length - 1
                If i = 0 Then
                    shell = GetCoordinatesArrayAsLinearRing(coordinate(i))
                Else
                    If holes Is Nothing Then
                        holes = New List(Of NetTopologySuite.Geometries.LinearRing)
                    End If
                    holes.Add(GetCoordinatesArrayAsLinearRing(coordinate(i)))
                End If
            Next
            If holes Is Nothing Then
                polygon = New NetTopologySuite.Geometries.Polygon(shell)
            Else
                polygon = New NetTopologySuite.Geometries.Polygon(shell, holes.ToArray())
            End If

        Catch ex As Exception
            polygon = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return polygon
    End Function

    Private Function GetCoordinatesArrayAsLinearRing(ByVal points As Double()()) As NetTopologySuite.Geometries.LinearRing
        Dim ret As NetTopologySuite.Geometries.LinearRing
        Try
            Dim pList As New List(Of NetTopologySuite.Geometries.Coordinate)
            For Each p In points
                pList.Add(New NetTopologySuite.Geometries.Coordinate(p(0), p(1)))
            Next
            ret = New NetTopologySuite.Geometries.LinearRing(pList.ToArray())
        Catch ex As Exception
            ret = Nothing
            Throw New Exception(ex.Message, ex)
        End Try
        Return ret
    End Function
End Class


Public Class ShapeFileRecord
    Public Property feature As Feature
    Public Property attributes As AttributeRecord

    Public Sub New()
        feature = New Feature
        attributes = New AttributeRecord
    End Sub
End Class
