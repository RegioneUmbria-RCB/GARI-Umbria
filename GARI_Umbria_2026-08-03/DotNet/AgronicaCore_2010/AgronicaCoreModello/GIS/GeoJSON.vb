
Imports AgronicaCoreModello.Geometry
Imports Newtonsoft.Json

Public Class GeoJson(Of T)

    Public geoJsonCaricato As GeoJson_Shape(Of T)

End Class


Public Class GeoJson_Shape(Of T)

    Public type As String
    Public features As List(Of GeoJson_Feature(Of T))


End Class

Public Class GeoJson_Feature(Of T)

    Public type As String
    Public geometry As GeoJson_Geometry
    Public properties As T


End Class

Public Class GeoJson_Geometry

    Public Sub New()

    End Sub

    Public Sub New(GeoJson As String)

    End Sub

    Public Property type As String

    Public Property coordinates As Double()()()

End Class

Public Class GeoJson_EmptyProperties

End Class

''' <summary>
''' Defines the valid geometry types
''' </summary>
Public Class GeoJson_GeometryType
    Public Const Point As String = "Point"
    Public Const MultiPoint As String = "MultiPoint"
    Public Const LineString As String = "LineString"
    Public Const MultiLineString As String = "MultiLineString"
    Public Const Polygon As String = "Polygon"
    Public Const MultiPolygon As String = "MultiPolygon"
    Public Const GeometryCollection As String = "GeometryCollection"
End Class

Public Class GeoJSONPropertyTest
    Public Property letter As String
    Public Property color As String
    Public Property rank As String
    Public Property ascii As String

End Class



Public Class GeoJSONAgroGisProp
    Public Property layer As String
    Public Property StandardEntita_layerDiAppartenenza As Integer
    Public Property StandardEntita_layerDiAppartenenza_Des As String
    Public Property StandardEntita_layerDiAppartenenza_Icona32 As String
    Public Property id As String
    Public Property tipoicona As String
    Public Property zindex As String
    Public Property Entita_Cod As String
    Public Property veg_cod As String
    Public Property inserimento As String
    Public Property flag_gps As String
    Public Property etichetta As String
    Public Property modifica As String
    Public Property cancellazione As String
    Public Property informazioni As String
    Public Property chiavealbero As String
    Public Property Testo As String
    Public Property AppIdRate As String
    Public Property TipologiaGML As String
    Public Property InOsservazione As Integer
    Public Property Colore_Primario As String
    Public Property Colore_Retinatura As String
    Public Property Trasparenza As Integer
End Class

#Region "SqlSpatialConverter"
'Public Enum FeatureType
'    Point = 1
'    MultiPoint = 2
'    Linestring = 3
'    MultiLineString = 4
'    Polygon = 5
'    MultiPolygon = 6
'End Enum

'Public Class GeoJson_Geometry_New
'    Private _type As FeatureType

'    Public coordinates

'    Public Property type As String
'        Get
'            Return [Enum].GetName(GetType(FeatureType), _type)
'        End Get
'        Set(value As String)
'            _type = [Enum].Parse(GetType(FeatureType), value)
'        End Set
'    End Property

'    Public Sub New(type As FeatureType, data As String)
'        _type = type

'        Select Case type
'            Case FeatureType.Point
'                coordinates = JsonConvert.DeserializeObject(Of Double())(data)
'            Case FeatureType.MultiPoint
'                coordinates = JsonConvert.DeserializeObject(Of Double()())(data)
'            Case FeatureType.Linestring
'                coordinates = JsonConvert.DeserializeObject(Of Double()())(data)
'            Case FeatureType.MultiLineString
'                coordinates = JsonConvert.DeserializeObject(Of Double()()())(data)
'            Case FeatureType.Polygon
'                coordinates = JsonConvert.DeserializeObject(Of Double()()())(data)
'            Case FeatureType.MultiPolygon
'                coordinates = JsonConvert.DeserializeObject(Of Double()()()())(data)

'        End Select
'    End Sub

'    Public Sub New()

'    End Sub
'End Class

'Public Class GeoJson_New(Of T)

'    Public geoJsonCaricato As GeoJson_Shape_New(Of T)

'End Class

'Public Class GeoJson_Shape_New(Of T)

'    Public type As String
'    Public features As List(Of GeoJson_Feature_New(Of T))


'End Class

'Public Class GeoJson_Feature_New(Of T)

'    Public type As String
'    Public geometry As GeoJson_Geometry_New
'    Public properties As T


'End Class
#End Region