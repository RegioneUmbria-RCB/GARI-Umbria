
Public Class Oggetti

    Public Shared Sub SetPropertyValue(ByVal inputObject As Object, ByVal propertyName As String, ByVal propertyVal As Object)
        Dim type As Type = inputObject.GetType()
        Dim propertyInfo As System.Reflection.PropertyInfo = type.GetProperty(propertyName)
        Dim propertyType As Type = propertyInfo.PropertyType
        Dim targetType = If(IsNullableType(propertyInfo.PropertyType), Nullable.GetUnderlyingType(propertyInfo.PropertyType), propertyInfo.PropertyType)
        propertyVal = Convert.ChangeType(propertyVal, targetType)
        propertyInfo.SetValue(inputObject, propertyVal, Nothing)
    End Sub

    Private Shared Function IsNullableType(ByVal type As Type) As Boolean
        Return type.IsGenericType AndAlso (type.GetGenericTypeDefinition() Is GetType(Nullable(Of)))
    End Function

End Class
