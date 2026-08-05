Imports AgronicaConversioneCartografiaGias.FormatsConverter
Imports AgronicaCoreModello.Geometry
Imports AgronicaCoreModelsSTD.Gis
Imports Newtonsoft.Json
Public Class GeoJsonParser
    Implements IWktConverter
    Private Class GeometryTypeTranslation
        Public Const Point As String = "POINT"
        Public Const MultiPoint As String = "MULTIPOINT"
        Public Const LineString As String = "LINESTRING"
        Public Const MultiLineString As String = "MULTILINESTRING"
        Public Const Polygon As String = "POLYGON"
        Public Const MultiPolygon As String = "MULTIPOLYGON"
        Public Const GeometryCollection As String = "GEOMETRYCOLLECTION"
    End Class

    Dim _geojson As GeoJson_Geometry_New
    Dim _geojsonstr As Object

    Public Sub New()

    End Sub

    Public Sub New(ByVal geojson As GeoJson_Geometry_New)
        _geojson = geojson
    End Sub

    Public Function ToWKTString() As String Implements IWktConverter.ToWKTString
        Dim wktString As String = ""
        If (_geojson Is Nothing) Then
            Throw New Exception("GeoJson object null")
        End If

        Select Case _geojson.type.ToUpper
            Case GeometryTypeTranslation.Point
                wktString = getWKTPoint(JsonConvert.SerializeObject(_geojson.coordinates))
            Case GeometryTypeTranslation.MultiPoint
                wktString = getWKTMultiPoint(JsonConvert.SerializeObject(_geojson.coordinates))
            Case GeometryTypeTranslation.LineString
                wktString = getWKTLineString(JsonConvert.SerializeObject(_geojson.coordinates))
            Case GeometryTypeTranslation.MultiLineString
                wktString = getWKTMultiLineString(JsonConvert.SerializeObject(_geojson.coordinates))
            Case GeometryTypeTranslation.Polygon
                wktString = getWKTPolygon(JsonConvert.SerializeObject(_geojson.coordinates))
            Case GeometryTypeTranslation.MultiPolygon
                wktString = getWKTMultiPolygon(JsonConvert.SerializeObject(_geojson.coordinates))
            Case GeometryTypeTranslation.GeometryCollection

            Case Else
                Throw New Exception("Geometry type not mapped.")
        End Select
        Return wktString
    End Function

    Private Function getWKTPoint(coordinates As String)
        Dim wkt As String = "POINT ("
        Dim tmp = JsonConvert.DeserializeObject(Of Double())(coordinates)

        wkt += tmp(0).ToString() + " " + tmp(1).ToString() + ")"

        Return wkt
    End Function

    Private Function getWKTMultiPoint(coordinates As String)
        Dim wkt As String = "MULTIPOINT ("
        Dim tmp = JsonConvert.DeserializeObject(Of Double()())(coordinates)

        For Each pnt In tmp
            wkt += pnt(0).ToString() + " " + pnt(1).ToString() + ", "
        Next
        wkt = wkt.Substring(0, wkt.Length - 2) + ")"
        Return wkt
    End Function

    Private Function getWKTLineString(coordinates As String)
        Dim wkt As String = "LINESTRING ("
        Dim tmp = JsonConvert.DeserializeObject(Of Double()())(coordinates)

        For Each pnt In tmp
            wkt += pnt(0).ToString() + " " + pnt(1).ToString() + ", "
        Next
        wkt = wkt.Substring(0, wkt.Length - 2) + ")"
        Return wkt
    End Function

    Private Function getWKTMultiLineString(coordinates As String)
        Dim wkt As String = "MULTILINESTRING ("
        Dim tmp = JsonConvert.DeserializeObject(Of Double()()())(coordinates)

        For Each inner In tmp
            wkt += "("
            For Each pnt In inner
                wkt += pnt(0).ToString() + " " + pnt(1).ToString() + ", "
            Next
            wkt = wkt.Substring(0, wkt.Length - 2)
            wkt += "),"
        Next
        wkt = wkt.Substring(0, wkt.Length - 1) + ")"
        Return wkt
    End Function

    Private Function getWKTPolygon(coordinates As String)
        Dim wkt As String = "POLYGON ("

        Dim tmp = JsonConvert.DeserializeObject(Of Double()()())(coordinates)

        For Each inner In tmp
            wkt += "("
            For Each pnt In inner
                wkt += pnt(0).ToString().Replace(",", ".") + " " + pnt(1).ToString().Replace(",", ".") + ", "
            Next
            wkt = wkt.Substring(0, wkt.Length - 2)
            wkt += "),"
        Next
        wkt = wkt.Substring(0, wkt.Length - 1) + ")"
        Return wkt
    End Function

    Private Function getWKTMultiPolygon(coordinates As String)
        Dim wkt As String = "MULTIPOLYGON ("

        Dim tmp = JsonConvert.DeserializeObject(Of Double()()()())(coordinates)

        For Each shape In tmp
            wkt += "("
            For Each inner In shape
                wkt += "("
                For Each pnt In inner
                    wkt += pnt(0).ToString() + " " + pnt(1).ToString() + ", "
                Next
                wkt = wkt.Substring(0, wkt.Length - 2)
                wkt += "),"
            Next
            wkt += "),"
        Next
        wkt = wkt.Substring(0, wkt.Length - 1) + ")"

        Return wkt
    End Function
End Class
