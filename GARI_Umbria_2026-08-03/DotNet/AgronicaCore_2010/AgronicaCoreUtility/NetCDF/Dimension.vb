
Namespace NetCDF

    Public Class Dimension

        Public ReadOnly name As String              ' the name of the dimension
        Public ReadOnly length As Integer           ' the length of the dimension
        Public ReadOnly record_dimension As Boolean ' true if this a 'record dimension'

        Public Sub New(s As NetCDFStream)
            name = s.ReadString()
            length = s.ReadInt32()
            ' if dim_length is 0, then this is the record dimension
            record_dimension = length = 0
        End Sub

    End Class

End Namespace
