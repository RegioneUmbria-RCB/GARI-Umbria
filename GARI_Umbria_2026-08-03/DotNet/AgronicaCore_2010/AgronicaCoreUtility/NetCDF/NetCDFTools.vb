

Namespace NetCDF

    Public Enum NC_TYPE
        NC_BYTE = 1
        NC_CHAR = 2
        NC_SHORT = 3
        NC_INT = 4
        NC_FLOAT = 5
        NC_DOUBLE = 6
    End Enum


    Public Class NetCDFTools

        ''' <summary>
        ''' True if the data read in Is in little endian, false otherwise (default false)
        ''' </summary>
        Private Shared ReadOnly DataIsLittleEndian As Boolean = False

        ''' <summary>
        ''' True if the computer Is little endian, false otherwise (default Is autocalculated to be true/false)
        ''' </summary>
        Private Shared ReadOnly ComputerIsLittleEndian As Boolean = BitConverter.IsLittleEndian

        Public Shared Function getTypeLength(t As NC_TYPE) As UInteger

            Select Case t
                Case NC_TYPE.NC_BYTE
                    Return 1

                Case NC_TYPE.NC_CHAR
                    Return 1

                Case NC_TYPE.NC_DOUBLE
                    Return 8

                Case NC_TYPE.NC_FLOAT
                    Return 4

                Case NC_TYPE.NC_INT
                    Return 4

                Case NC_TYPE.NC_SHORT
                    Return 2
            End Select

            Return 0
        End Function

        Public Shared Function EndianConversion(bytes As Byte()) As Byte()

            If ComputerIsLittleEndian = DataIsLittleEndian Then ' If the endian-ness Is the same, do nothing

                Return bytes
            End If

            'no matter which uses which endian, the data must be reversed
            Dim bytesNew(bytes.Length - 1) As Byte
            Array.Copy(bytes, bytesNew, bytes.Length)
            Array.Reverse(bytesNew)

            Return bytesNew
        End Function

        Public Shared Function byteToFloat(bytes As Byte(), type As NC_TYPE) As Single
            'convert any numeric byte type to a float
            Select Case type
                Case NC_TYPE.NC_SHORT
                    Return Convert.ToSingle(BitConverter.ToInt16(EndianConversion(bytes), 0))
                Case NC_TYPE.NC_INT
                    Return Convert.ToSingle(BitConverter.ToInt32(EndianConversion(bytes), 0))
                Case NC_TYPE.NC_DOUBLE
                    Return Convert.ToSingle(byteToDouble(bytes))
            End Select

            Return BitConverter.ToSingle(EndianConversion(bytes), 0)
        End Function

        Public Shared Function byteToDouble(bytes As Byte()) As Double
            Return BitConverter.ToDouble(EndianConversion(bytes), 0)
        End Function

    End Class

End Namespace


