
Namespace NetCDF

    Interface IValue
        Function Value() As Object
    End Interface

    Public Class ValueT(Of T)
        Implements IValue

        Private ReadOnly _value As T

        Public Sub New(v As T)
            _value = v
        End Sub

        Private Function Value() As Object Implements IValue.Value
            Return _value
        End Function
    End Class

    Public Class AttributeValue

        Private ReadOnly _value As IValue

        Public Sub New(type As NC_TYPE, bytes As Byte())

            Select Case type

                Case NC_TYPE.NC_BYTE
                    Throw New NotImplementedException()

                Case NC_TYPE.NC_CHAR

                    Dim value As String = ""
                    For Each b In bytes
                        value += Convert.ToChar(b)
                    Next
                    _value = New ValueT(Of String)(value)


                Case NC_TYPE.NC_SHORT
                    Throw New NotImplementedException()

                Case NC_TYPE.NC_INT
                    Throw New NotImplementedException()

                Case NC_TYPE.NC_FLOAT

                    Dim value As Single = NetCDFTools.byteToFloat(bytes, type)
                    _value = New ValueT(Of Single)(value)

                Case NC_TYPE.NC_DOUBLE
                    Throw New NotImplementedException()

            End Select
        End Sub

        Public Function Value() As Object
            Return _value.Value
        End Function
    End Class

    Public Class AttributeFactory

        Public Shared Function ReadAttributes(s As NetCDFStream) As Dictionary(Of String, AttributeValue)

            Dim dict As New Dictionary(Of String, AttributeValue)

            Dim attribute_tag As Integer = s.ReadInt32() ' should be 0xC
            Dim num_attrs As UInteger = s.ReadUInt32()

            If attribute_tag = &HC AndAlso num_attrs > 0 Then

                For i As UInteger = 1 To num_attrs

                    Dim name As String = s.ReadString()
                    Dim type As NC_TYPE = s.ReadUInt32()
                    Dim nelems As UInteger = s.ReadUInt32()
                    Dim bytes As Byte() = s.ReadBytes(type, nelems)

                    dict.Add(name, New AttributeValue(type, bytes))
                Next
            End If

            Return dict
        End Function

    End Class

End Namespace
