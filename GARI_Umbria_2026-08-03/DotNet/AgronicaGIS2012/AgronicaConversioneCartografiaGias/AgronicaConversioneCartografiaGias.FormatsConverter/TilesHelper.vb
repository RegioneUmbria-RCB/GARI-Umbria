

Imports System.Math
Imports AgronicaGIS2012.Commons


''' <summary>
''' da lat long a tile e vice-versa
''' </summary>
''' <remarks>https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#Lon..2Flat._to_tile_numbers</remarks>
Public Class TilesHelper

    Public Function LatLong2tileSlippyFormat(ByVal wkt As String, ByVal Zoom As String) As String

        Dim convertWkt As New WKT
        Dim listaCoordinate As List(Of xyz) =
            convertWkt.CreaCoordinateDaPunto(wkt)

        Return LatLong2tileSlippyFormat(listaCoordinate.First.Y, listaCoordinate.First.X, Zoom)

    End Function

    Public Function LatLong2tileSlippyFormat(ByVal latitudine As Double, ByVal longitudine As Double, ByVal Zoom As String) As String

        '        Dim latitude_radians As Double = decimalDegreeToRadians(latitudine)

        Dim rval As String

        Dim zoomInt As Integer

        If Not Integer.TryParse(Zoom, zoomInt) Then
            Throw New ArgumentException("Parametro Zoom non intero")
        End If

        Dim tmpTileY As Integer = lat2tiley(latitudine, zoomInt)
        Dim ymax = 1 << zoomInt

        Dim tileY As Integer = ymax - tmpTileY - 1
        Dim tileX = long2tilex(longitudine, Zoom)

        rval = Zoom & "\" & tileX & "\" & tileY & ".png"

        Return rval

    End Function

    Private Function decimalDegreeToRadians(DecimalDegree As Double) As Double
        Return DecimalDegree * PI / 180
    End Function

    Private Function long2tilex(ByVal lon As Double, ByVal z As Integer) As Integer
        Return CInt((Floor((lon + 180.0) / 360.0 * Pow(2.0, z))))
    End Function

    Private Function lat2tiley(ByVal lat As Double, ByVal z As Integer) As Integer
        Return CInt((Floor((1.0 - Log(Tan(lat * PI / 180.0) + 1.0 / Cos(lat * PI / 180.0)) / PI) / 2.0 * Pow(2.0, z))))
    End Function

    Private Function tilex2long(ByVal x As Integer, ByVal z As Integer) As Double
        Return x / Pow(2.0, z) * 360.0 - 180
    End Function

    Private Function tiley2lat(ByVal y As Integer, ByVal z As Integer) As Double
        Dim n As Double = PI - 2.0 * PI * y / Pow(2.0, z)
        Return 180.0 / PI * Atan(0.5 * (Exp(n) - Exp(-n)))
    End Function
End Class