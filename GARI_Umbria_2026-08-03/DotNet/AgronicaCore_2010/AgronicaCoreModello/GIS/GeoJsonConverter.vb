Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq
Imports AgronicaCoreModelsSTD.Gis

Public Class GeoJsonConverter
    Inherits JsonConverter

    Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
        Try
            Dim jo = JObject.Load(reader)
            Dim type As FeatureType = [Enum].Parse(GetType(FeatureType), jo("type"))

            Dim item As Object = Nothing
            Select Case type
                Case FeatureType.Point
                    item = JsonConvert.DeserializeObject(Of Double())(jo("coordinates"))
                Case FeatureType.MultiPoint
                    item = JsonConvert.DeserializeObject(Of Double()())(jo("coordinates"))
                Case FeatureType.Linestring
                    item = JsonConvert.DeserializeObject(Of Double()())(jo("coordinates"))
                Case FeatureType.MultiLineString
                    item = JsonConvert.DeserializeObject(Of Double()()())(jo("coordinates"))
                Case FeatureType.Polygon
                    item = JsonConvert.DeserializeObject(Of Double()()())(jo("coordinates"))
                Case FeatureType.MultiPolygon
                    item = JsonConvert.DeserializeObject(Of Double()()()())(jo("coordinates"))

            End Select
            Return item
        Catch ex As Exception
            Return Nothing
        End Try

    End Function

    Public Overrides Function CanConvert(objectType As Type) As Boolean
        If GetType(Object).IsAssignableFrom(objectType) Then
            Return True
        End If

        Return False
    End Function

    Public Overrides ReadOnly Property CanWrite As Boolean
        Get
            Return False
        End Get
    End Property
End Class
