
Imports System.Runtime.InteropServices

Namespace NetCDF

    Public Class Variable

        Public ReadOnly name As String                                      ' name of the var
        Public ReadOnly dimId As Integer()                                  ' dimid.length = rank of variable. index into dim_list for variable shape
        Public ReadOnly attributes As Dictionary(Of String, AttributeValue) ' variable-specific attributes
        Private ReadOnly _type As NC_TYPE                                   ' type of variable in data
        Private ReadOnly _dataBytes As Byte()
        Private ReadOnly _length As Integer                                 ' the number Of data points

        Public Sub New(s As NetCDFStream, dimList As List(Of Dimension))

            name = s.ReadString()
            Dim rank As UInteger = s.ReadUInt32()
            ReDim dimId(rank - 1)
            For j = 0 To rank - 1
                dimId(j) = s.ReadInt32()
            Next

            attributes = AttributeFactory.ReadAttributes(s)

            _type = s.ReadUInt32()
            Dim vsize As UInteger = s.ReadUInt32() ' the amount Of space In bytes allocated In the data
            Dim begin As UInteger = s.ReadUInt32() ' the Byte offset Of the variable's data
            _dataBytes = s.ReadData(begin, vsize)
            _length = vsize / NetCDFTools.getTypeLength(_type)

            If rank = 1 Then
                _length = dimList.ElementAt(dimId(0)).length
            End If
        End Sub

        Public ReadOnly Property Length As Integer
            Get
                Return _length
            End Get
        End Property

        Public Function AsDouble(index As Integer) As Double
            Return NetCDFTools.byteToDouble(Data(index))
        End Function

        Public Function AsFloat(index As Integer) As Single
            Return NetCDFTools.byteToFloat(Data(index), _type)
        End Function

        Private Function Data(index As Integer) As Byte()

            Dim typesize As Integer = NetCDFTools.getTypeLength(_type)

            Dim clone(typesize - 1) As Byte

            'Create GChandle instance and pin variable required
            Dim handle As GCHandle = GCHandle.Alloc(clone, GCHandleType.Pinned)
            'get address of variable in pointer variable
            Dim AddrOfClone As IntPtr = handle.AddrOfPinnedObject()
            'Use copy method to copy array data to variable’s 
            Marshal.Copy(_dataBytes, (index * typesize), AddrOfClone, typesize)
            handle.Free()

            Return clone
        End Function

    End Class

End Namespace

