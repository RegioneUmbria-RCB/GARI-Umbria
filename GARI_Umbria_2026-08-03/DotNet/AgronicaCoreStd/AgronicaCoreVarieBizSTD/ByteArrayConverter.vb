Imports Newtonsoft.Json

Public Class ByteArrayConverter
    Inherits Newtonsoft.Json.JsonConverter

    Public Overrides Sub WriteJson(ByVal writer As JsonWriter, ByVal value As Object, ByVal serializer As JsonSerializer)
        If value Is Nothing Then
            writer.WriteNull()
            Return
        End If

        Dim data As Byte() = CType(value, Byte())
        writer.WriteStartArray()

        For i = 0 To data.Length - 1
            writer.WriteValue(data(i))
        Next

        writer.WriteEndArray()
    End Sub

    Public Overrides Function ReadJson(ByVal reader As JsonReader, ByVal objectType As Type, ByVal existingValue As Object, ByVal serializer As JsonSerializer) As Object
        If reader.TokenType = JsonToken.StartArray Then
            Dim byteList = New List(Of Byte)()

            While reader.Read()

                Select Case reader.TokenType
                    Case JsonToken.Integer
                        byteList.Add(Convert.ToByte(reader.Value))
                    Case JsonToken.EndArray
                        Return byteList.ToArray()
                    Case JsonToken.Comment
                    Case Else
                        Throw New Exception(String.Format("Unexpected token when reading bytes: {0}", reader.TokenType))
                End Select
            End While

            Throw New Exception("Unexpected end when reading bytes.")
        Else

        End If
    End Function

    Public Overrides Function CanConvert(ByVal objectType As Type) As Boolean
        Return objectType = GetType(Byte())
    End Function

End Class
