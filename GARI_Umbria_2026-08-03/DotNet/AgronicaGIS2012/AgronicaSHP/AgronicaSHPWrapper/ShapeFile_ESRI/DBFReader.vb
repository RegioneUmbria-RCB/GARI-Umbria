

Imports System.Globalization
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text


Namespace ShapeFile_ESRI

    ''' <summary>
    ''' This class reads a dbf files
    ''' </summary>
    Friend Class DBFReader
        Implements IDisposable

        ''' <summary>
        ''' Dbf Version
        ''' </summary>
        Private Enum DBFVersion As Byte
            Unknown = 0
            FoxBase = &H2
            FoxBaseDBase3NoMemo = &H3
            VisualFoxPro = &H30
            VisualFoxProWithAutoIncrement = &H31
            dBase4SQLTableNoMemo = &H43
            dBase4SQLSystemNoMemo = &H63
            FoxBaseDBase3WithMemo = &H83
            dBase4WithMemo = &H8B
            dBase4SQLTableWithMemo = &HCB
            FoxPro2WithMemo = &HF5
            'FoxBASE = 0xFB
        End Enum


        <Flags>
        Private Enum DBFTableFlags As Byte
            None = &H0
            HasStructuralCDX = &H1
            HasMemoField = &H2
            IsDBC = &H4
        End Enum


        ''' <summary>
        ''' This is the file header for a DBF. We do this special layout with everything
        ''' packed so we can read straight from disk into the structure to populate it
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
        Private Structure DBFHeader
            ''' <summary>
            ''' The version
            ''' </summary>
            Public ReadOnly Version As DBFVersion
            ''' <summary>
            ''' The update year.
            ''' </summary>
            Public ReadOnly UpdateYear As Byte
            ''' <summary>
            ''' The update month
            ''' </summary>
            Public ReadOnly UpdateMonth As Byte
            ''' <summary>
            ''' The update day
            ''' </summary>
            Public ReadOnly UpdateDay As Byte
            ''' <summary>
            ''' The number of records
            ''' </summary>
            Public ReadOnly NumberOfRecords As Integer
            ''' <summary>
            ''' The length of the header
            ''' </summary>
            Public ReadOnly HeaderLenght As Short
            ''' <summary>
            ''' The length of the bytes records
            ''' </summary>
            Public ReadOnly RecordLength As Short

            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=16)>
            Public ReadOnly Reserved As Byte()

            ''' <summary>
            ''' Table Flags
            ''' </summary>
            Public ReadOnly TableFlags As DBFTableFlags
            ''' <summary>
            ''' Code Page Mark
            ''' </summary>
            Public ReadOnly CodePage As Byte
            ''' <summary>
            ''' Reserved, contains 0x00
            ''' </summary>
            Public ReadOnly EndOfHeader As Short
        End Structure


        <Flags>
        Private Enum DBFFieldFlags As Byte
            None = &H0
            System = &H1
            AllowNullValues = &H2
            Binary = &H4
            AutoIncrementing = &HC
        End Enum


        ''' <summary>
        ''' This is the field descriptor structure. There will be one of these for each column in the table
        ''' </summary>
        <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
        Private Structure DBFFieldDescriptor
            ''' <summary>
            ''' The field name
            ''' </summary>
            <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=11)>
            Public ReadOnly FieldName As String
            ''' <summary>
            ''' The field type
            ''' </summary>
            Public ReadOnly FieldType As Char
            ''' <summary>
            ''' The field address
            ''' </summary>
            Public ReadOnly Address As Integer
            ''' <summary>
            ''' The field length in bytes
            ''' </summary>
            Public ReadOnly FieldLength As Byte
            ''' <summary>
            ''' The field precision
            ''' </summary>
            Public ReadOnly DecimalCount As Byte
            ''' <summary>
            ''' Field Flags
            ''' </summary>
            Public ReadOnly Flags As DBFFieldFlags
            ''' <summary>
            ''' AutoIncrement next value
            ''' </summary>
            Public ReadOnly AutoIncrementNextValue As Integer
            ''' <summary>
            ''' AutoIncrement step value
            ''' </summary>
            Public ReadOnly AutoIncrementStepValue As Byte
            ''' <summary>
            ''' Reserved
            ''' </summary>
            <MarshalAs(UnmanagedType.ByValArray, SizeConst:=8)>
            Public ReadOnly Reserved As Byte()

            Public Overrides Function ToString() As String
                Return String.Format("{0} {1}", FieldName, FieldType)
            End Function
        End Structure


        ''' <summary>
        ''' Field Type
        ''' </summary>
        Private Enum DBFFieldType As Integer
            F_Character = Asc("C")
            F_Currency = Asc("Y")
            F_Numeric = Asc("N")
            F_Float = Asc("F")
            F_Date = Asc("D")
            F_DateTime = Asc("T")
            F_Double = Asc("B")
            F_Integer = Asc("I")
            F_Logical = Asc("L")
            F_Memo = Asc("M")
            F_General = Asc("G")
            F_Picture = Asc("P")
        End Enum


        Private _reader As BinaryReader
        Private _encoding As Encoding
        Private _header As DBFHeader
        Private _fields As List(Of DBFFieldDescriptor)

        ''' <summary>
        ''' instantiate a Dbf reader based on a stream and encoding type
        ''' </summary>
        ''' <param name="stream">The record number in the Shapefile</param>
        ''' <param name="encoding">Metadata about the shape</param>
        Public Sub New(stream As Stream, encoding As Encoding)

            _encoding = encoding
            _reader = New BinaryReader(stream, _encoding)

            'Read header

            Dim buffer As Byte() = _reader.ReadBytes(Marshal.SizeOf(Of DBFHeader))

            'Marshall the header into a DBFHeader structure
            Dim handle As GCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned)
            _header = Marshal.PtrToStructure(Of DBFHeader)(handle.AddrOfPinnedObject())
            handle.Free()

            _fields = New List(Of DBFFieldDescriptor)

            While _reader.PeekChar() <> 13

                buffer = _reader.ReadBytes(Marshal.SizeOf(Of DBFFieldDescriptor))
                handle = GCHandle.Alloc(buffer, GCHandleType.Pinned)

                Dim fieldDescriptor = Marshal.PtrToStructure(Of DBFFieldDescriptor)(handle.AddrOfPinnedObject())

                If (fieldDescriptor.Flags And DBFFieldFlags.System) <> DBFFieldFlags.System Then
                    _fields.Add(fieldDescriptor)
                End If

                handle.Free()
            End While
        End Sub

        Public Function ReadRecord(numRec As Integer) As Dictionary(Of String, String)

            If numRec >= _header.NumberOfRecords Then

                Return Nothing
            End If

            Dim fieldRecord = New Dictionary(Of String, String)

            _reader.BaseStream.Seek(_header.HeaderLenght + (_header.RecordLength * numRec), SeekOrigin.Begin)

            Dim row As Byte() = _reader.ReadBytes(_header.RecordLength)
            Dim position As Integer = 0

            For Each field In _fields

                Dim buffer(field.FieldLength - 1) As Byte
                Array.Copy(row, position + 1, buffer, 0, field.FieldLength)

                position += field.FieldLength

                fieldRecord(field.FieldName.ToUpper) = BufferToField(buffer, Asc(field.FieldType), field.Flags)
            Next

            Return fieldRecord
        End Function

        Private Function BufferToField(buffer As Byte(), fieldType As Integer, fieldFlags As DBFFieldFlags) As String

            Dim text As String = _encoding.GetString(buffer)

            If String.IsNullOrEmpty(text) Then
                text = String.Empty
            Else
                text = text.Replace(Convert.ToChar(0), "").Trim
            End If

            Select Case fieldType

                Case DBFFieldType.F_Character

                    Return text


                Case DBFFieldType.F_Currency, DBFFieldType.F_Numeric, DBFFieldType.F_Float, DBFFieldType.F_Double, DBFFieldType.F_Integer

                    If String.IsNullOrWhiteSpace(text) Then

                        If (fieldFlags And DBFFieldFlags.AllowNullValues) = DBFFieldFlags.AllowNullValues Then

                            Return Nothing
                        End If

                        Return "0"
                    End If

                    If fieldType = DBFFieldType.F_Integer Then

                        Return BitConverter.ToInt32(buffer, 0).ToString
                    End If

                    Return text


                Case DBFFieldType.F_Date

                    If String.IsNullOrWhiteSpace(text) Then

                        If (fieldFlags And DBFFieldFlags.AllowNullValues) = DBFFieldFlags.AllowNullValues Then

                            Return Nothing
                        End If

                        Return DateTime.MinValue.ToString
                    End If

                    Dim dt As Date = Date.MinValue

                    Try

                        dt = DateTime.ParseExact(text, "yyyyMMdd", CultureInfo.InvariantCulture)

                    Catch ex As Exception

                    End Try

                    Return dt.ToString

                Case DBFFieldType.F_DateTime

                    If String.IsNullOrWhiteSpace(text) OrElse BitConverter.ToInt64(buffer, 0) = 0 Then

                        If (fieldFlags And DBFFieldFlags.AllowNullValues) = DBFFieldFlags.AllowNullValues Then

                            Return Nothing
                        End If

                        Return DateTime.MinValue.ToString
                    End If

                    Return JulianToDateTime(BitConverter.ToInt64(buffer, 0)).ToString()


                Case DBFFieldType.F_Logical

                    If String.IsNullOrWhiteSpace(text) Then

                        If (fieldFlags And DBFFieldFlags.AllowNullValues) = DBFFieldFlags.AllowNullValues Then

                            Return Nothing
                        End If

                        Return "False"
                    End If

                    Return (buffer(0) = Asc("Y") OrElse buffer(0) = Asc("T")).ToString


                    'Case DBFFieldType.Memo:
                    'Case DBFFieldType.General:
                    'Case DBFFieldType.Picture:
                Case Else

                    Return buffer.ToString

            End Select

            Return Nothing
        End Function

        Public Function ReadRecordType(numRec As Integer) As Dictionary(Of String, Type)

            If numRec >= _header.NumberOfRecords Then

                Return Nothing
            End If

            Dim fieldRecord = New Dictionary(Of String, Type)

            _reader.BaseStream.Seek(_header.HeaderLenght + (_header.RecordLength * numRec), SeekOrigin.Begin)

            Dim row As Byte() = _reader.ReadBytes(_header.RecordLength)
            Dim position As Integer = 0

            For Each field In _fields

                Dim buffer(field.FieldLength - 1) As Byte
                Array.Copy(row, position + 1, buffer, 0, field.FieldLength)

                position += field.FieldLength

                fieldRecord(field.FieldName.ToUpper) = BufferToType(Asc(field.FieldType))
            Next

            Return fieldRecord
        End Function

        Private Function BufferToType(fieldType As Integer) As Type

            Select Case fieldType

                Case DBFFieldType.F_Character

                    Return GetType(String)

                Case DBFFieldType.F_Integer

                    Return GetType(Integer)

                Case DBFFieldType.F_Double

                    Return GetType(Double)

                Case DBFFieldType.F_Currency, DBFFieldType.F_Numeric, DBFFieldType.F_Float

                    Return GetType(Decimal)

                Case DBFFieldType.F_Date

                    Return GetType(Date)

                Case DBFFieldType.F_DateTime

                    Return GetType(DateTime)

                Case DBFFieldType.F_Logical

                    Return GetType(Boolean)

                Case Else

                    Return GetType(String)

            End Select

            Return Nothing
        End Function

        ''' <summary>
        ''' Convert a Julian Date as long to a .NET DateTime structure
        ''' Implemented from pseudo code at http://en.wikipedia.org/wiki/Julian_day
        ''' </summary>
        ''' <param name="julianDateAsLong">Julian Date to convert (days since 01/01/4713 BC)</param>
        ''' <returns></returns>
        Private Function JulianToDateTime(julianDateAsLong As Long) As DateTime

            If julianDateAsLong = 0 Then
                Return DateTime.MinValue
            End If

            Dim p As Double = Convert.ToDouble(julianDateAsLong)
            Dim s1 As Double = p + 68569
            Dim n As Double = Math.Floor(4 * s1 / 146097)
            Dim s2 As Double = s1 - Math.Floor(((146097 * n) + 3) / 4)
            Dim i As Double = Math.Floor(4000 * (s2 + 1) / 1461001)
            Dim s3 As Double = s2 - Math.Floor(1461 * i / 4) + 31
            Dim q As Double = Math.Floor(80 * s3 / 2447)
            Dim d As Double = s3 - Math.Floor(2447 * q / 80)
            Dim s4 As Double = Math.Floor(q / 11)
            Dim m As Double = q + 2 - (12 * s4)
            Dim j As Double = (100 * (n - 49)) + i + s4

            Return New DateTime(Convert.ToInt32(j), Convert.ToInt32(m), Convert.ToInt32(d))
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ''' <summary>
        ''' dispose the reader
        ''' </summary>
        ''' <param name="disposing"></param>
        Protected Sub Dispose(disposing As Boolean)

            If Not disposing Then
                Return
            End If

            If _reader IsNot Nothing Then
                _reader.Close()
                _reader.Dispose()
                _reader = Nothing
            End If
        End Sub

    End Class

End Namespace