
Imports System.IO

Namespace NetCDF

    Public Class NetCDFStream
        Implements IDisposable

        Private ReadOnly srcStream As Stream
        Private disposedValue As Boolean

        Public ReadOnly Property Position As Long
            Get
                Return srcStream.Position
            End Get
        End Property

        Public Sub New(s As Stream)

            srcStream = s

            srcStream.Seek(0, SeekOrigin.Begin)

            ' should be "CDF"
            Dim magic As String = Convert.ToChar(srcStream.ReadByte()) + Convert.ToChar(srcStream.ReadByte()) + Convert.ToChar(srcStream.ReadByte())

            ' should be 1
            Dim version As Integer = srcStream.ReadByte()

            disposedValue = False
        End Sub

        Public Function ReadUInt32() As UInteger
            Dim bytes(3) As Byte
            srcStream.Read(bytes, 0, 4)
            Return BitConverter.ToUInt32(NetCDFTools.EndianConversion(bytes), 0)
        End Function

        Public Function ReadInt32() As Integer
            Dim bytes(3) As Byte
            srcStream.Read(bytes, 0, 4)
            Return BitConverter.ToInt32(NetCDFTools.EndianConversion(bytes), 0)
        End Function

        Public Function ReadString() As String

            Dim length As Integer = ReadInt32()

            Dim str As String = ""

            For i = 1 To length
                str += Convert.ToChar(srcStream.ReadByte())
            Next

            Padding(str.Length)

            Return str
        End Function

        Public Function ReadData(offset As Long, size As Long) As Byte()

            Dim oldPos As Long = srcStream.Position

            srcStream.Seek(offset, SeekOrigin.Begin)

            Dim bytes(size - 1) As Byte

            srcStream.Read(bytes, 0, size)

            srcStream.Seek(oldPos, SeekOrigin.Begin)

            Return bytes
        End Function

        Public Function ReadBytes(type As NC_TYPE, length As UInteger) As Byte()

            Dim typeLength As UInteger = NetCDFTools.getTypeLength(type)
            Dim arrSize As UInteger = length * typeLength
            Dim valueArray As Byte() = New Byte(arrSize - 1) {}

            ' read in the values
            srcStream.Read(valueArray, 0, arrSize)

            ' read in the padding bytes to the 4-byte boundary
            Padding(arrSize)

            Return valueArray
        End Function

        Private Sub Padding(length As UInteger)
            Dim remainder = 4 - length Mod 4
            If remainder = 4 Then
                Return
            End If
            For i = 1 To remainder
                srcStream.ReadByte()
            Next
        End Sub

        Protected Overridable Sub Dispose(disposing As Boolean)

            If Not disposedValue Then

                If disposing Then
                    ' TODO: eliminare lo stato gestito (oggetti gestiti)

                    Try

                        srcStream.Close()
                        srcStream.Dispose()

                    Catch ex As Exception
                    End Try

                End If

                ' TODO: liberare risorse non gestite (oggetti non gestiti) ed eseguire l'override del finalizzatore
                ' TODO: impostare campi di grandi dimensioni su Null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: eseguire l'override del finalizzatore solo se 'Dispose(disposing As Boolean)' contiene codice per liberare risorse non gestite
        ' Protected Overrides Sub Finalize()
        '     ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Non modificare questo codice. Inserire il codice di pulizia nel metodo 'Dispose(disposing As Boolean)'
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub

    End Class

End Namespace
