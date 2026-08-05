Imports Newtonsoft.Json
Imports AgronicaCoreDataProvider.Agro_Math

''' <summary>
''' Classe che arrotonda a 3 decimali
''' </summary>
Public Class DecimalRoundingConverter
    Inherits JsonConverter

    Public Overrides Function CanConvert(objectType As Type) As Boolean
        Return objectType = GetType(Decimal) OrElse objectType = GetType(Nullable(Of Decimal))
    End Function

    Public Overrides Sub WriteJson(writer As JsonWriter, value As Object, serializer As JsonSerializer)
        If value IsNot Nothing Then
            Dim decimalValue As Decimal = CType(value, Decimal)
            writer.WriteValue(ArrotondaVal_3(decimalValue))
        Else
            writer.WriteNull()
        End If
    End Sub

    Public Overrides Function ReadJson(reader As JsonReader, objectType As Type, existingValue As Object, serializer As JsonSerializer) As Object
        If reader.TokenType = JsonToken.Null Then
            Return Nothing
        End If

        Return Convert.ToDecimal(reader.Value)
    End Function
End Class

