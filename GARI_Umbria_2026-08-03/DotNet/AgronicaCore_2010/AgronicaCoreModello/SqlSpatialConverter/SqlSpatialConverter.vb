Public Class SqlSpatialConverter(Of T)
    Implements IWktConverter, IGeoJsonConverter, IGeometry
    Dim _object As T

    Public Sub New(ByVal str As String)
        _object = Activator.CreateInstance(GetType(T), str)
    End Sub

    Public Sub New(ByVal obj As AgronicaCoreModelsSTD.Gis.GeoJson_Geometry_New)
        _object = Activator.CreateInstance(GetType(T), obj)
    End Sub

    Public Function ToGeoJson() As AgronicaCoreModelsSTD.Gis.GeoJson_Geometry_New Implements IGeoJsonConverter.ToGeoJson
        Try
            Dim _methodInfo = GetType(T).GetMethod("ToGeoJson")
            If _methodInfo Is Nothing Then
                Throw New NotImplementedException()
            End If
            Return _methodInfo.Invoke(_object, Nothing)
        Catch ex As Exception
            Throw New NotImplementedException()
        End Try

    End Function

    Public Function ToWKTString() As String Implements IWktConverter.ToWKTString
        Try
            Try
                Dim _methodInfo = GetType(T).GetMethod("ToWKTString")
                If _methodInfo Is Nothing Then
                    Throw New NotImplementedException()
                End If
                Return _methodInfo.Invoke(_object, Nothing)
            Catch ex As Exception
                Throw New NotImplementedException()
            End Try
        Catch ex As Exception
            Throw New NotImplementedException()
        End Try

    End Function

    Public Function getGeometry() As Geometry.GeometryDefinition Implements IGeometry.getGeometry
        Try
            Try
                Dim _methodInfo = GetType(T).GetMethod("getGeometry")
                If _methodInfo Is Nothing Then
                    Throw New NotImplementedException()
                End If
                Return _methodInfo.Invoke(_object, Nothing)
            Catch ex As Exception
                Throw New NotImplementedException()
            End Try
        Catch ex As Exception
            Throw New NotImplementedException()
        End Try
    End Function
End Class
