

Namespace ShapeFile_ESRI

    ''' <summary>
    ''' The order of bytes provided to EndianBitConverter
    ''' </summary>
    Public Enum ProvidedOrder
        ''' <summary>
        ''' Value is stored as big-endian
        ''' </summary>
        Big
        ''' <summary>
        ''' Value is stored as little-endian
        ''' </summary>
        Little
    End Enum



    ''' <summary>
    ''' BitConverter methods that allow a different source byte order (only a subset of BitConverter)
    ''' </summary>
    Public Class EndianBitConverter

        ''' <summary>
        ''' Returns an integer from four bytes of a byte array
        ''' </summary>
        ''' <param name="value">bytes to convert</param>
        ''' <param name="startIndex">start index in value</param>
        ''' <param name="order">byte order of value</param>
        ''' <returns>the integer</returns>
        Public Shared Function ToInt32(value As Byte(), startIndex As Integer, order As ProvidedOrder) As Integer
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If

            Dim sizeof_int As Integer = Len(New Integer)

            If startIndex + sizeof_int > value.Length Then
                Throw New ArgumentException("startIndex invalid (not enough space in value to extract an integer", "startIndex")
            End If

            If BitConverter.IsLittleEndian AndAlso order = ProvidedOrder.Big Then

                Dim toConvert(sizeof_int - 1) As Byte
                Array.Copy(value, startIndex, toConvert, 0, sizeof_int)
                Array.Reverse(toConvert)

                Return BitConverter.ToInt32(toConvert, 0)
            End If

            Return BitConverter.ToInt32(value, startIndex)
        End Function

        ''' <summary>
        ''' Returns a double from eight bytes of a byte array
        ''' </summary>
        ''' <param name="value">bytes to convert</param>
        ''' <param name="startIndex">start index in value</param>
        ''' <param name="order">byte order of value</param>
        ''' <returns>the double</returns>
        Public Shared Function toDouble(value As Byte(), startIndex As Integer, order As ProvidedOrder) As Double
            If value Is Nothing Then
                Throw New ArgumentNullException("value")
            End If

            Dim sizeof_double As Integer = Len(New Double)

            If startIndex + sizeof_double > value.Length Then
                Throw New ArgumentException("startIndex invalid (not enough space in value to extract a double", "startIndex")
            End If

            If BitConverter.IsLittleEndian AndAlso order = ProvidedOrder.Big Then

                Dim toConvert(sizeof_double - 1) As Byte
                Array.Copy(value, startIndex, toConvert, 0, sizeof_double)
                Array.Reverse(toConvert)

                Return BitConverter.ToDouble(toConvert, 0)
            End If

            Return BitConverter.ToDouble(value, startIndex)
        End Function
    End Class

End Namespace

