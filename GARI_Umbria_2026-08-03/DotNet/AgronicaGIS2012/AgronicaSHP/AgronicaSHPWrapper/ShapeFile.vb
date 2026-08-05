' Filename:    ShapeFile.vb
' Description: Classes for reading ESRI shapefiles.
' Reference:   ESRI Shapefile Technical Description, July 1998.
'              http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
' 2007-01-22 nschan Initial revision.

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Text
Imports System.Windows
Imports AgronicaSHPWrapper.GeoAPI.DataStructures
Imports AgronicaSHPWrapper.SharpMap.Data.Providers.ShapeFile

Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider

Namespace TestShapeFile
    ''' <summary>
    ''' Enumeration defining the various shape types. Each shapefile
    ''' contains only one type of shape (e.g., all polygons or all
    ''' polylines).
    ''' </summary>
    Public Enum ShapeType
        ''' <summary>
        ''' Nullshape / placeholder record.
        ''' </summary>
        NullShape = 0

        ''' <summary>
        ''' Point record, for defining point locations such as a city.
        ''' </summary>
        Point = 1

        ''' <summary>
        ''' One or more sets of connected points. Used to represent roads,
        ''' hydrography, etc.
        ''' </summary>
        PolyLine = 3

        ''' <summary>
        ''' One or more sets of closed figures. Used to represent political
        ''' boundaries for countries, lakes, etc.
        ''' </summary>
        Polygon = 5

        ''' <summary>
        ''' A cluster of points represented by a single shape record.
        ''' </summary>
        Multipoint = 8

        'Partial support (non Z read)
        PointZ = 11

        ' Unsupported types:
        ' PolyLineZ = 13,        
        ' PolygonZ = 15,        
        ' MultiPointZ = 18,        
        ' PointM = 21,        
        ' PolyLineM = 23,        
        ' PolygonM = 25,        
        ' MultiPointM = 28,        
        ' MultiPatch = 31
    End Enum

    ''' <summary>
    ''' The ShapeFileHeader class represents the contents
    ''' of the fixed length, 100-byte file header at the
    ''' beginning of every shapefile.
    ''' </summary>
    Public Class ShapeFileHeader
#Region "Private fields"
        Private m_fileCode As Integer
        Private m_fileLength As Integer
        Private m_version As Integer
        Private m_shapeType As Integer

        ' Bounding box.
        Private m_xMin As Double
        Private m_yMin As Double
        Private m_xMax As Double
        Private m_yMax As Double
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for the ShapeFileHeader class.
        ''' </summary>
        Public Sub New()
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Indicate the fixed-length of this header in bytes.
        ''' </summary>
        Public Shared ReadOnly Property Length() As Integer
            Get
                Return 100
            End Get
        End Property

        ''' <summary>
        ''' Specifies the file code for an ESRI shapefile, which
        ''' should be the value, 9994.
        ''' </summary>
        Public Property FileCode() As Integer
            Get
                Return Me.m_fileCode
            End Get
            Set(value As Integer)
                Me.m_fileCode = value
            End Set
        End Property

        ''' <summary>
        ''' Specifies the length of the shapefile, expressed
        ''' as the number of 16-bit words in the file.
        ''' </summary>
        Public Property FileLength() As Integer
            Get
                Return Me.m_fileLength
            End Get
            Set(value As Integer)
                Me.m_fileLength = value
            End Set
        End Property

        ''' <summary>
        ''' Specifies the shapefile version number.
        ''' </summary>
        Public Property Version() As Integer
            Get
                Return Me.m_version
            End Get
            Set(value As Integer)
                Me.m_version = value
            End Set
        End Property

        ''' <summary>
        ''' Specifies the shape type for the file. A shapefile
        ''' contains only one type of shape.
        ''' </summary>
        Public Property ShapeType() As Integer
            Get
                Return Me.m_shapeType
            End Get
            Set(value As Integer)
                Me.m_shapeType = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the minimum x-position of the bounding
        ''' box for the shapefile (expressed in degrees longitude).
        ''' </summary>
        Public Property XMin() As Double
            Get
                Return Me.m_xMin
            End Get
            Set(value As Double)
                Me.m_xMin = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the minimum y-position of the bounding
        ''' box for the shapefile (expressed in degrees latitude).
        ''' </summary>
        Public Property YMin() As Double
            Get
                Return Me.m_yMin
            End Get
            Set(value As Double)
                Me.m_yMin = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the maximum x-position of the bounding
        ''' box for the shapefile (expressed in degrees longitude).
        ''' </summary>       
        Public Property XMax() As Double
            Get
                Return Me.m_xMax
            End Get
            Set(value As Double)
                Me.m_xMax = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the maximum y-position of the bounding
        ''' box for the shapefile (expressed in degrees latitude).
        ''' </summary>
        Public Property YMax() As Double
            Get
                Return Me.m_yMax
            End Get
            Set(value As Double)
                Me.m_yMax = value
            End Set
        End Property
#End Region

#Region "Public methods"
        ''' <summary>
        ''' Output some of the fields of the file header.
        ''' </summary>
        ''' <returns>A string representation of the file header.</returns>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendFormat("ShapeFileHeader: FileCode={0}, FileLength={1}, Version={2}, ShapeType={3}", Me.m_fileCode, Me.m_fileLength, Me.m_version, Me.m_shapeType)

            Return sb.ToString()
        End Function
#End Region
    End Class

    ''' <summary>
    ''' The ShapeFileRecord class represents the contents of
    ''' a shape record, which is of variable length.
    ''' </summary>
    Public Class ShapeFileRecord
#Region "Private fields"
        ' Record Header.
        Private m_recordNumber As Integer
        Private m_contentLength As Integer

        ' Shape type.
        Private m_shapeType As Integer

        ' Bounding box for shape.
        Private m_xMin As Double
        Private m_yMin As Double
        Private m_xMax As Double
        Private m_yMax As Double

        ' Part indices and points array.
        Private m_parts As New Collection(Of Integer)()
        Private m_points As New Collection(Of System.Windows.Point)()

        ' Shape attributes from a row in the dBASE file.
        Private m_attributes As DataRow
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for the ShapeFileRecord class.
        ''' </summary>
        Public Sub New()
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Indicates the record number (or index) which starts at 1.
        ''' </summary>
        Public Property RecordNumber() As Integer
            Get
                Return Me.m_recordNumber
            End Get
            Set(value As Integer)
                Me.m_recordNumber = value
            End Set
        End Property

        ''' <summary>
        ''' Specifies the length of this shape record in 16-bit words.
        ''' </summary>
        Public Property ContentLength() As Integer
            Get
                Return Me.m_contentLength
            End Get
            Set(value As Integer)
                Me.m_contentLength = value
            End Set
        End Property

        ''' <summary>
        ''' Specifies the shape type for this record.
        ''' </summary>
        Public Property ShapeType() As Integer
            Get
                Return Me.m_shapeType
            End Get
            Set(value As Integer)
                Me.m_shapeType = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the minimum x-position of the bounding
        ''' box for the shape (expressed in degrees longitude).
        ''' </summary>
        Public Property XMin() As Double
            Get
                Return Me.m_xMin
            End Get
            Set(value As Double)
                Me.m_xMin = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the minimum y-position of the bounding
        ''' box for the shape (expressed in degrees latitude).
        ''' </summary>
        Public Property YMin() As Double
            Get
                Return Me.m_yMin
            End Get
            Set(value As Double)
                Me.m_yMin = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the maximum x-position of the bounding
        ''' box for the shape (expressed in degrees longitude).
        ''' </summary>
        Public Property XMax() As Double
            Get
                Return Me.m_xMax
            End Get
            Set(value As Double)
                Me.m_xMax = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the maximum y-position of the bounding
        ''' box for the shape (expressed in degrees latitude).
        ''' </summary>
        Public Property YMax() As Double
            Get
                Return Me.m_yMax
            End Get
            Set(value As Double)
                Me.m_yMax = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates the number of parts for this shape.
        ''' A part is a connected set of points, analogous to
        ''' a PathFigure in WPF.
        ''' </summary>
        Public ReadOnly Property NumberOfParts() As Integer
            Get
                Return Me.m_parts.Count
            End Get
        End Property

        ''' <summary>
        ''' Specifies the total number of points defining
        ''' this shape record.
        ''' </summary>
        Public ReadOnly Property NumberOfPoints() As Integer
            Get
                Return Me.m_points.Count
            End Get
        End Property

        ''' <summary>      
        ''' A collection of indices for the points array.
        ''' Each index identifies the starting point of the
        ''' corresponding part (or PathFigure using WPF
        ''' terminology).
        ''' </summary>
        Public ReadOnly Property Parts() As Collection(Of Integer)
            Get
                Return Me.m_parts
            End Get
        End Property

        ''' <summary>
        ''' A collection of all of the points defining the
        ''' shape record.
        ''' </summary>
        Public ReadOnly Property Points() As Collection(Of System.Windows.Point)
            Get
                Return Me.m_points
            End Get
        End Property

        ''' <summary>
        ''' Access the (dBASE) attribute values associated
        ''' with this shape record.
        ''' </summary>
        Public Property Attributes() As DataRow
            Get
                Return Me.m_attributes
            End Get
            Set(value As DataRow)
                Me.m_attributes = value
            End Set
        End Property
#End Region

#Region "Public methods"
        ''' <summary>
        ''' Output some of the fields of the shapefile record.
        ''' </summary>
        ''' <returns>A string representation of the record.</returns>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendFormat("ShapeFileRecord: RecordNumber={0}, ContentLength={1}, ShapeType={2}", Me.m_recordNumber, Me.m_contentLength, Me.m_shapeType)

            Return sb.ToString()
        End Function
#End Region
    End Class

    ''' <summary>
    ''' The ShapeFileReadInfo class stores information about a shapefile
    ''' that can be used by external clients during a shapefile read.
    ''' </summary>
    Public Class ShapeFileReadInfo
#Region "Private fields"
        Private m_fileName As String
        Private m_shapeFile As ShapeFile
        Private m_stream As Stream
        Private numBytesRead As Integer
        Private m_recordIndex As Integer
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for the ShapeFileReadInfo class.
        ''' </summary>
        Public Sub New()
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' The full pathname of the shapefile.
        ''' </summary>
        Public Property FileName() As String
            Get
                Return Me.m_fileName
            End Get
            Set(value As String)
                Me.m_fileName = value
            End Set
        End Property

        ''' <summary>
        ''' A reference to the shapefile instance.
        ''' </summary>
        Public Property ShapeFile() As ShapeFile
            Get
                Return Me.m_shapeFile
            End Get
            Set(value As ShapeFile)
                Me.m_shapeFile = value
            End Set
        End Property

        ''' <summary>
        ''' An opened file stream for a shapefile.
        ''' </summary>
        Public Property Stream() As Stream
            Get
                Return Me.m_stream
            End Get
            Set(value As Stream)
                Me.m_stream = value
            End Set
        End Property

        ''' <summary>
        ''' The number of bytes read from a shapefile so far.
        ''' Can be used to compute a progress value.
        ''' </summary>
        Public Property NumberOfBytesRead() As Integer
            Get
                Return Me.numBytesRead
            End Get
            Set(value As Integer)
                Me.numBytesRead = value
            End Set
        End Property

        ''' <summary>
        ''' A general-purpose record index.
        ''' </summary>
        Public Property RecordIndex() As Integer
            Get
                Return Me.m_recordIndex
            End Get
            Set(value As Integer)
                Me.m_recordIndex = value
            End Set
        End Property
#End Region

#Region "Public methods"
        ''' <summary>
        ''' Output some of the field values in the form of a string.
        ''' </summary>
        ''' <returns>A string representation of the field values.</returns>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendFormat("ShapeFileReadInfo: FileName={0}, ", Me.m_fileName)
            sb.AppendFormat("NumberOfBytesRead={0}, RecordIndex={1}", Me.numBytesRead, Me.m_recordIndex)

            Return sb.ToString()
        End Function
#End Region
    End Class

    ''' <summary>
    ''' The ShapeFile class represents the contents of a single
    ''' ESRI shapefile. This is the class which contains functionality
    ''' for reading shapefiles and their corresponding dBASE attribute
    ''' files.
    ''' </summary>
    ''' <remarks>
    ''' You can call the Read() method to import both shapes and attributes
    ''' at once. Or, you can open the file stream yourself and read the file
    ''' header or individual records one at a time. The advantage of this is
    ''' that it allows you to implement your own progress reporting functionality,
    ''' for example.
    ''' </remarks>
    Public Class ShapeFile
#Region "Constants"
        Private Const expectedFileCode As Integer = 9994
#End Region

#Region "Private static fields"
        Private Shared intBytes As Byte() = New Byte(3) {}
        Private Shared doubleBytes As Byte() = New Byte(7) {}
#End Region

#Region "Private fields"
        ' File Header.
        Private m_fileHeader As New ShapeFileHeader()

        ' Collection of Shapefile Records.
        Private m_records As New Collection(Of ShapeFileRecord)()
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for the ShapeFile class.
        ''' </summary>
        Public Sub New()
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Access the file header of this shapefile.
        ''' </summary>
        Public ReadOnly Property FileHeader() As ShapeFileHeader
            Get
                Return Me.m_fileHeader
            End Get
        End Property

        ''' <summary>
        ''' Access the collection of records for this shapefile.
        ''' </summary>
        Public ReadOnly Property Records() As Collection(Of ShapeFileRecord)
            Get
                Return Me.m_records
            End Get
        End Property
#End Region

#Region "Public methods"

        Private Property FileLengthInWords As Integer

        ''' <summary>
        ''' Read both shapes and attributes from the given
        ''' shapefile (and its corresponding dBASE file).
        ''' This is the top-level method for reading an ESRI
        ''' shapefile.
        ''' </summary>
        ''' <param name="fileName">Full pathname of the shapefile.</param>
        Public Sub Read(fileName As String)
            If String.IsNullOrEmpty(fileName) Then
                Throw New ArgumentNullException("fileName")
            End If

            ' Read shapes first (geometry).
            Me.ReadShapes(fileName)

            ' Construct name and path of dBASE file. It's basically
            ' the same name as the shapefile except with a .dbf extension.
            Dim dbaseFile As String = fileName.Replace(".shp", ".dbf")
            dbaseFile = dbaseFile.Replace(".SHP", ".DBF")

            ' Read the attributes.
            Me.ReadAttributes(dbaseFile)
        End Sub

        ''' <summary>
        ''' Read shapes (geometry) from the given shapefile.
        ''' </summary>
        ''' <param name="fileName">Full pathname of the shapefile.</param>
        Public Sub ReadShapes(fileName As String)
            Using stream As New FileStream(fileName, FileMode.Open, FileAccess.Read)
                Me.ReadShapes(stream)
            End Using
        End Sub

        ''' <summary>
        ''' Read shapes (geometry) from the given stream.
        ''' </summary>
        ''' <param name="stream">Input stream for a shapefile.</param>
        Public Sub ReadShapes(stream As Stream)
            ' Read the File Header.
            Me.ReadShapeFileHeader(stream)

            ' Read the shape records.
            Me.m_records.Clear()
            While True
                Try
                    Me.ReadShapeFileRecord(stream)
                Catch generatedExceptionName As IOException
                    ' Stop reading when EOF exception occurs.
                    Exit While
                End Try
            End While
        End Sub

        ''' <summary>
        ''' Read the file header of the shapefile.
        ''' </summary>
        ''' <param name="stream">Input stream.</param>
        Public Sub ReadShapeFileHeader(stream As Stream)
            ' File Code.
            Me.m_fileHeader.FileCode = ShapeFile.ReadInt32_BE(stream)
            If Me.m_fileHeader.FileCode <> ShapeFile.expectedFileCode Then
                Dim msg As String = [String].Format(System.Globalization.CultureInfo.InvariantCulture, "Invalid FileCode encountered. Expecting {0}.", ShapeFile.expectedFileCode)
                Throw New FileFormatException(msg)
            End If

            ' 5 unused values.
            ShapeFile.ReadInt32_BE(stream)
            ShapeFile.ReadInt32_BE(stream)
            ShapeFile.ReadInt32_BE(stream)
            ShapeFile.ReadInt32_BE(stream)
            ShapeFile.ReadInt32_BE(stream)

            ' File Length.
            Me.m_fileHeader.FileLength = ShapeFile.ReadInt32_BE(stream)

            ' Version.
            Me.m_fileHeader.Version = ShapeFile.ReadInt32_LE(stream)

            ' Shape Type.
            Me.m_fileHeader.ShapeType = ShapeFile.ReadInt32_LE(stream)

            ' Bounding Box.
            Me.m_fileHeader.XMin = ShapeFile.ReadDouble64_LE(stream)
            Me.m_fileHeader.YMin = ShapeFile.ReadDouble64_LE(stream)
            Me.m_fileHeader.XMax = ShapeFile.ReadDouble64_LE(stream)
            Me.m_fileHeader.YMax = ShapeFile.ReadDouble64_LE(stream)

            ' Adjust the bounding box in case it is too small.
            If Math.Abs(Me.m_fileHeader.XMax - Me.m_fileHeader.XMin) < 1 Then
                Me.m_fileHeader.XMin -= 5
                Me.m_fileHeader.XMax += 5
            End If
            If Math.Abs(Me.m_fileHeader.YMax - Me.m_fileHeader.YMin) < 1 Then
                Me.m_fileHeader.YMin -= 5
                Me.m_fileHeader.YMax += 5
            End If

            ' Skip the rest of the file header.
            stream.Seek(100, SeekOrigin.Begin)
        End Sub

        ''' <summary>
        ''' Read a shapefile record.
        ''' </summary>
        ''' <param name="stream">Input stream.</param>
        Public Function ReadShapeFileRecord(stream As Stream) As ShapeFileRecord
            Dim record As New ShapeFileRecord()

            ' Record Header.
            record.RecordNumber = ShapeFile.ReadInt32_BE(stream)
            record.ContentLength = ShapeFile.ReadInt32_BE(stream)

            ' Shape Type.
            record.ShapeType = ShapeFile.ReadInt32_LE(stream)

            ' Read the shape geometry, depending on its type.
            Select Case record.ShapeType
                Case CInt(ShapeType.NullShape)
                    ' Do nothing.
                    Exit Select
                Case CInt(ShapeType.Point)
                    ShapeFile.ReadPoint(stream, record)
                Case CInt(ShapeType.PointZ)
                    ShapeFile.ReadPoint25D(stream, record)
                    Exit Select
                Case CInt(ShapeType.PolyLine)
                    ' PolyLine has exact same structure as Polygon in shapefile.
                    ShapeFile.ReadPolygon(stream, record)
                    Exit Select
                Case CInt(ShapeType.Polygon)
                    ShapeFile.ReadPolygon(stream, record)
                    Exit Select
                Case CInt(ShapeType.Multipoint)
                    ShapeFile.ReadMultipoint(stream, record)
                    Exit Select
                Case Else
                    If True Then
                        Dim msg As String = [String].Format(System.Globalization.CultureInfo.InvariantCulture, "ShapeType {0} is not supported.", CInt(record.ShapeType))
                        Throw New FileFormatException(msg)
                    End If
            End Select

            ' Add the record to our internal list.
            Me.m_records.Add(record)

            Return record
        End Function

        ''' <summary>
        ''' Read the table from specified dBASE (DBF) file and
        ''' merge the rows with shapefile records.
        ''' </summary>
        ''' <remarks>
        ''' The filename of the dBASE file is expected to follow 8.3 naming
        ''' conventions. If it doesn't follow the convention, we try to
        ''' determine the 8.3 short name ourselves (but the name we come up
        ''' with may not be correct in all cases).
        ''' </remarks>
        ''' <param name="dbaseFile">Full file path of the dBASE (DBF) file.</param>
        Public Sub ReadAttributes(dbaseFile As String)
            If String.IsNullOrEmpty(dbaseFile) Then
                Throw New ArgumentNullException("dbaseFile")
            End If

            ' Check if the file exists. If it doesn't exist,
            ' this is not an error.
            If Not File.Exists(dbaseFile) Then
                Return
            End If

            ' Get the directory in which the dBASE (DBF) file resides.
            Dim fi As New FileInfo(dbaseFile)
            Dim directory As String = fi.DirectoryName

            ' Get the filename minus the extension.
            Dim fileNameNoExt As String = fi.Name.ToUpper(System.Globalization.CultureInfo.InvariantCulture)
            If fileNameNoExt.EndsWith(".DBF") Then
                fileNameNoExt = fileNameNoExt.Substring(0, fileNameNoExt.Length - 4)
            End If

            ' Convert to a short filename (may not work in every case!).
            If fileNameNoExt.Length > 8 Then
                If fileNameNoExt.Contains(" ") Then
                    Dim noSpaces As String = fileNameNoExt.Replace(" ", "")
                    If noSpaces.Length > 8 Then
                        fileNameNoExt = noSpaces
                    End If
                End If
                fileNameNoExt = fileNameNoExt.Substring(0, 6) & "~1"
            End If

            ' Set the connection string.
            Dim connectionString As String = ""
            Dim selectQuery As String = ""
            If Environment.Is64BitProcess Then
                connectionString = "PROVIDER=Microsoft.ACE.OLEDB.12.0;Data Source=" & directory & ";Extended Properties=dBASE 5.0;"
                selectQuery = "SELECT * FROM [" & fileNameNoExt & "#DBF];"
            Else
                connectionString = "PROVIDER=VFPOLEDB.1;Data Source=" & directory & ";"
                selectQuery = "SELECT * FROM [" & fileNameNoExt & "];"
            End If

            Dim connectionString2 As String = "PROVIDER=Microsoft.Jet.OLEDB.4.0;Data Source=" & directory & ";Extended Properties=dBASE 5.0;"

            ' Create a database connection object using the connection string.
            Dim connection As New OleDbConnection(connectionString)

            ' Create a database command on the connection using the select query.
            Dim command As New OleDbCommand(selectQuery, connection)


            'testa la connessione e la re-imposta

            Try
                ' Open the connection.          
                connection.Open()
            Catch e As Exception
                If connection.State = ConnectionState.Open Then
                    connection.Close()
                End If
                connection.ConnectionString = connectionString2
            Finally
            End Try



            Try
                ' Open the connection.          
                If connection.State <> ConnectionState.Open Then
                    connection.Open()
                End If

                ' Create a data adapter to fill a dataset.
                Dim dataAdapter As New OleDbDataAdapter()
                dataAdapter.SelectCommand = command
                Dim dataSet As New DataSet()
                dataSet.Locale = System.Globalization.CultureInfo.InvariantCulture
                dataAdapter.Fill(dataSet)

                ' Merge attributes into the shape file.
                If dataSet.Tables.Count > 0 Then
                    Me.MergeAttributes(dataSet.Tables(0))
                End If
            Catch generatedExceptionName As OleDbException
                ' Note: An exception will occur if the filename of the dBASE
                ' file does not follow 8.3 naming conventions. In this case,
                ' you must use its short (MS-DOS) filename.

                ' Rethrow the exception.
                Throw
            Finally
                ' Dispose of connection.
                DirectCast(connection, IDisposable).Dispose()
            End Try
        End Sub

        ''' <summary>
        ''' Output the File Header in the form of a string.
        ''' </summary>
        ''' <returns>A string representation of the file header.</returns>
        Public Overrides Function ToString() As String
            Return "ShapeFile: " & Me.m_fileHeader.ToString()
        End Function
#End Region

#Region "Private methods"
        ''' <summary>
        ''' Read a 4-byte integer using little endian (Intel)
        ''' byte ordering.
        ''' </summary>
        ''' <param name="stream">Input stream to read.</param>
        ''' <returns>The integer value.</returns>
        Private Shared Function ReadInt32_LE(stream As Stream) As Integer
            For i As Integer = 0 To 3
                Dim b As Integer = stream.ReadByte()
                If b = -1 Then
                    Throw New EndOfStreamException()
                End If
                intBytes(i) = CByte(b)
            Next

            Return BitConverter.ToInt32(intBytes, 0)
        End Function

        ''' <summary>
        ''' Read a 4-byte integer using big endian
        ''' byte ordering.
        ''' </summary>
        ''' <param name="stream">Input stream to read.</param>
        ''' <returns>The integer value.</returns>
        Private Shared Function ReadInt32_BE(stream As Stream) As Integer
            For i As Integer = 3 To 0 Step -1
                Dim b As Integer = stream.ReadByte()
                If b = -1 Then
                    Throw New EndOfStreamException()
                End If
                intBytes(i) = CByte(b)
            Next

            Return BitConverter.ToInt32(intBytes, 0)
        End Function


        ''' <summary>
        ''' Read an 8-byte double using little endian (Intel)
        ''' byte ordering.
        ''' </summary>
        ''' <param name="stream">Input stream to read.</param>
        ''' <returns>The double value.</returns>
        Private Shared Function ReadDouble64_LE(stream As Stream) As Double
            For i As Integer = 0 To 7
                Dim b As Integer = stream.ReadByte()
                If b = -1 Then
                    Throw New EndOfStreamException()
                End If
                doubleBytes(i) = CByte(b)
            Next

            Return BitConverter.ToDouble(doubleBytes, 0)
        End Function

        ''' <summary>
        ''' Read a shapefile Point record.
        ''' </summary>
        ''' <param name="stream">Input stream.</param>
        ''' <param name="record">Shapefile record to be updated.</param>
        Private Shared Sub ReadPoint(stream As Stream, record As ShapeFileRecord)
            ' Points - add a single point.
            Dim p As System.Windows.Point = New System.Windows.Point()
            p.X = ShapeFile.ReadDouble64_LE(stream)
            p.Y = ShapeFile.ReadDouble64_LE(stream)
            record.Points.Add(p)

            ' Bounding Box.
            record.XMin = p.X
            record.YMin = p.Y
            record.XMax = record.XMin
            record.YMax = record.YMin
        End Sub

        ''' <summary>
        ''' Read a shapefile Point25D record.
        ''' </summary>
        ''' <param name="stream">Input stream.</param>
        ''' <param name="record">Shapefile record to be updated.</param>
        Private Shared Sub ReadPoint25D(stream As Stream, record As ShapeFileRecord)
            ' Points - add a single point.
            Dim p As System.Windows.Point = New System.Windows.Point()
            p.X = ShapeFile.ReadDouble64_LE(stream)
            p.Y = ShapeFile.ReadDouble64_LE(stream)
            Dim Z As Double = ShapeFile.ReadDouble64_LE(stream)
            Dim Measure As Double = ShapeFile.ReadDouble64_LE(stream)
            record.Points.Add(p)

            ' Bounding Box.
            record.XMin = p.X
            record.YMin = p.Y
            record.XMax = record.XMin
            record.YMax = record.YMin
        End Sub

        ''' <summary>
        ''' Read a shapefile MultiPoint record.
        ''' </summary>
        ''' <param name="stream">Input stream.</param>
        ''' <param name="record">Shapefile record to be updated.</param>
        Private Shared Sub ReadMultipoint(stream As Stream, record As ShapeFileRecord)
            ' Bounding Box.
            record.XMin = ShapeFile.ReadDouble64_LE(stream)
            record.YMin = ShapeFile.ReadDouble64_LE(stream)
            record.XMax = ShapeFile.ReadDouble64_LE(stream)
            record.YMax = ShapeFile.ReadDouble64_LE(stream)

            ' Num Points.
            Dim numPoints As Integer = ShapeFile.ReadInt32_LE(stream)

            ' Points.           
            For i As Integer = 0 To numPoints - 1
                Dim p As New System.Windows.Point()
                p.X = ShapeFile.ReadDouble64_LE(stream)
                p.Y = ShapeFile.ReadDouble64_LE(stream)
                record.Points.Add(p)
            Next
        End Sub

        ''' <summary>
        ''' Read a shapefile Polygon record.
        ''' </summary>
        ''' <param name="stream">Input stream.</param>
        ''' <param name="record">Shapefile record to be updated.</param>
        Private Shared Sub ReadPolygon(stream As Stream, record As ShapeFileRecord)
            ' Bounding Box.
            record.XMin = ShapeFile.ReadDouble64_LE(stream)
            record.YMin = ShapeFile.ReadDouble64_LE(stream)
            record.XMax = ShapeFile.ReadDouble64_LE(stream)
            record.YMax = ShapeFile.ReadDouble64_LE(stream)

            ' Num Parts and Points.
            Dim numParts As Integer = ShapeFile.ReadInt32_LE(stream)
            Dim numPoints As Integer = ShapeFile.ReadInt32_LE(stream)

            ' Parts.           
            For i As Integer = 0 To numParts - 1
                record.Parts.Add(ShapeFile.ReadInt32_LE(stream))
            Next

            ' Points.           
            For i As Integer = 0 To numPoints - 1
                Dim p As New System.Windows.Point()
                p.X = ShapeFile.ReadDouble64_LE(stream)
                p.Y = ShapeFile.ReadDouble64_LE(stream)
                record.Points.Add(p)
            Next
        End Sub

        ''' <summary>
        ''' Merge data rows from the given table with
        ''' the shapefile records.
        ''' </summary>
        ''' <param name="table">Attributes table.</param>
        Private Sub MergeAttributes(table As DataTable)
            ' For each data row, assign it to a shapefile record.
            Dim index As Integer = 0
            For Each row As DataRow In table.Rows
                If index >= Me.m_records.Count Then
                    Exit For
                End If
                Me.m_records(index).Attributes = row
                index += 1
            Next
        End Sub
#End Region


#Region "Operazioni di scrittura"

        Public Function ComputeShapeFileSizeInWords() As Int32

            Dim totalLen As Int32 = ShapeFileConstants.HeaderSizeBytes / 2
            For Each en In Records
                totalLen += en.ContentLength
            Next

            Return totalLen
        End Function

        Private _shapeFileWriter As BinaryWriter
        Private _shapeFileStream As FileStream

        Private _indexFileWriter As BinaryWriter
        Private _indexFileStream As FileStream



        Private _recordOffsetInWords As Long
        Public Sub Save(ByVal FileName As String)

            'cancella i vecchi file
            If My.Computer.FileSystem.FileExists(FileName) Then
                My.Computer.FileSystem.DeleteFile(FileName)
            End If

            If My.Computer.FileSystem.FileExists(FileName.Replace(".shp", ".shx")) Then
                My.Computer.FileSystem.DeleteFile(FileName.Replace(".shp", ".shx"))
            End If

            If My.Computer.FileSystem.FileExists(FileName.Replace(".shp", ".dbf")) Then
                My.Computer.FileSystem.DeleteFile(FileName.Replace(".shp", ".dbf"))
            End If

            If My.Computer.FileSystem.FileExists(FileName.Replace(".shp", ".prj")) Then
                My.Computer.FileSystem.DeleteFile(FileName.Replace(".shp", ".prj"))
            End If

            'Shapefile
            _shapeFileStream = New FileStream(FileName,
                                              FileMode.OpenOrCreate,
                                              FileAccess.ReadWrite,
                                              FileShare.Write,
                                              4096,
                                              FileOptions.None)

            _shapeFileWriter = New BinaryWriter(_shapeFileStream)

            WriteHeader(_shapeFileWriter, m_fileHeader.FileLength)
            _recordOffsetInWords = ShapeFileConstants.HeaderSizeBytes


            writeRecords()

            _shapeFileWriter.Flush()
            _shapeFileWriter.Close()
            _shapeFileWriter.Dispose()


            'file di indice
            _indexFileStream = New FileStream(FileName.Replace(".shp", ".shx"),
                                              FileMode.OpenOrCreate,
                                              FileAccess.ReadWrite,
                                              FileShare.Write,
                                              4096,
                                              FileOptions.None)

            _indexFileWriter = New BinaryWriter(_indexFileStream)



            '        'Extents = ShapeFile.GetExtents()

            WriteHeader(_indexFileWriter, computeIndexLengthInWords())

            Dim offSet As Int32 = 50
            For Each entry As ShapeFileRecord In Records
                _indexFileWriter.Write(ByteEncoder.GetBigEndian(offSet))
                _indexFileWriter.Write(ByteEncoder.GetBigEndian(entry.ContentLength))
                offSet += (entry.ContentLength + (TestShapeFile.ShapeFileConstants.IndexRecordByteLength / 2)) 'Gabriele
            Next
            _indexFileWriter.Flush()
            _indexFileWriter.Close()
            _indexFileWriter.Dispose()


            'file di database
            CreaFileDBF(FileName.Replace(".shp", ".dbf"))


            '  Vanni, 13/12/2013 16:58:21: file di proiezione
            My.Computer.FileSystem.WriteAllText(FileName.Replace(".shp", ".prj"), getProjection, False)


        End Sub

        Private Function getProjection() As String
            Return _
            "GEOGCS[""WGS 84"",DATUM[""WGS_1984"",SPHEROID[""WGS 84"",6378137,298.257223563,AUTHORITY[""EPSG"",""7030""]],AUTHORITY[""EPSG"",""6326""]],PRIMEM[""Greenwich"",0,AUTHORITY[""EPSG"",""8901""]],UNIT[""degree"",0.01745329251994328,AUTHORITY[""EPSG"",""9122""]],AUTHORITY[""EPSG"",""4326""]]"
        End Function


        Private Sub CreaFileDBF(ByVal path As String)


            Dim provider As String = "PROVIDER=Microsoft.ACE.OLEDB.12.0;"
            Dim provider2 As String = "Provider=Microsoft.Jet.OLEDB.4.0;"

            Dim connectionString As String = provider &
                     "Data Source=" & My.Computer.FileSystem.GetFileInfo(path).DirectoryName & ";" &
                     "Extended Properties=dBASE IV"

            Dim connectionString2 As String = provider2 &
                     "Data Source=" & My.Computer.FileSystem.GetFileInfo(path).DirectoryName & ";" &
                     "Extended Properties=dBASE IV"

            Dim dBaseConnection As New System.Data.OleDb.OleDbConnection(connectionString)

            Try
                ' Open the connection (su connessione "Provider").          
                dBaseConnection.Open()
            Catch e As Exception
                If dBaseConnection.State = ConnectionState.Open Then
                    dBaseConnection.Close()
                End If
                dBaseConnection.ConnectionString = connectionString2
            Finally
            End Try

            'Open the connection (su connessione "Provider2").          
            If Not dBaseConnection.State = ConnectionState.Open Then
                dBaseConnection.Open()
            End If


            'New table
            Dim dBaseCommand As System.Data.OleDb.OleDbCommand
            Dim tableName As String = My.Computer.FileSystem.GetFileInfo(path).Name

            Dim insertStmt As String = "INSERT INTO [" & tableName & "]"
            Dim apici As String = ""
            Dim Campo As String = ""
            Dim valore As String = ""
            Dim valori As String = ""
            Dim CampiCreate As String = ""

            '--------------------------CREAZIONE SCHEMA TABELLA------------------------------
            'cerco il record con il maggior numero di campi
            Dim maxRighe As Integer = 0
            Dim recTemplate As New ShapeFileRecord
            For Each rec In Records
                If rec.Attributes.Table.Columns.Count > maxRighe Then
                    recTemplate = rec
                    maxRighe = rec.Attributes.Table.Columns.Count
                End If
            Next
            'creo la tabella sulla base di quel record
            For Each dc As DataColumn In recTemplate.Attributes.Table.Columns
                Select Case dc.DataType.Name
                    Case "String"
                        CampiCreate = CampiCreate & "[" & dc.ColumnName & "] TEXT(200),"
                    Case "DateTime"
                        CampiCreate = CampiCreate & "[" & dc.ColumnName & "] DATE,"
                    Case "Double"
                        CampiCreate = CampiCreate & "[" & dc.ColumnName & "] NUMERIC,"
                End Select
            Next

            Dim SQLCreateCommand As String = "CREATE TABLE [" & tableName & "](" & CampiCreate.TrimEnd(",") & ")"
            dBaseCommand = New System.Data.OleDb.OleDbCommand(SQLCreateCommand, dBaseConnection)
            dBaseCommand.ExecuteNonQuery()

            '--------------------------AGGIUNGO I RECORD ALLA TABELLA------------------------------
            For Each rec In Records
                apici = ""
                Campo = ""
                valore = ""
                valori = ""

                Try
                    For Each dc As DataColumn In recTemplate.Attributes.Table.Columns
                        Campo &= "[" & dc.ColumnName & "],"

                        'se nel mio record esiste il campo del template
                        If rec.Attributes.Table.Columns.Contains(dc.ColumnName) Then
                            Select Case dc.DataType.Name
                                Case "String"
                                    apici = "'"
                                    Dim originalValue = rec.Attributes(dc.ColumnName).ToString
                                    originalValue = originalValue.Replace(vbTab, " ").Trim
                                    valore = Agro_SQL_SaveText(originalValue.Substring(0, Math.Min(200, originalValue.Length)))
                                    'valore = Agro_SQL_SaveText(rec.Attributes(dc.ColumnName).ToString)
                                Case "DateTime"
                                    apici = "'"
                                    valore = rec.Attributes(dc.ColumnName).ToString
                                Case "Double"
                                    apici = ""
                                    valore = rec.Attributes(dc.ColumnName).ToString.Replace(",", ".")
                            End Select

                            If rec.Attributes(dc.ColumnName) Is System.DBNull.Value Then
                                valore = "null"
                            End If
                        Else 'se il campo non è contenuto, lo imposto a NULL
                            valore = "null"
                        End If

                        valori = valori & apici & valore & apici & ","
                    Next
                Catch ex As Exception
                    CreaRigaVuota(recTemplate.Attributes.Table.Columns, Campo, valori)
                End Try

                If Campo <> "" Then
                    dBaseCommand.CommandText = insertStmt.Replace(".dbf", "#dbf") & " (" & Campo.TrimEnd(",") & ") VALUES (" & valori.TrimEnd(",") & ")"
                    dBaseCommand.ExecuteNonQuery()
                End If

            Next

            dBaseConnection.Close()
            dBaseConnection.Dispose()

            My.Computer.FileSystem.CopyFile(path, path & "abc")
            My.Computer.FileSystem.DeleteFile(path, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            My.Computer.FileSystem.RenameFile(path & "abc", path.Split("\")(path.Split("\").Length - 1))

        End Sub

        Private Function CreaRigaVuota(columns As DataColumnCollection,
                                       ByRef Campo As String,
                                       ByRef valori As String) As String

            Dim apici As String = ""
            Dim valore As String = ""

            Campo = ""
            valori = ""

            For Each dc As DataColumn In columns
                Campo &= "[" & dc.ColumnName & "],"

                'se nel mio record esiste il campo del template
                Select Case dc.DataType.Name
                    Case "String"
                        apici = "'"
                        valore = ""
                    Case "DateTime"
                        apici = "'"
                        valore = CostantiPersonalizzate.AGRODATAINIZIO.ToString("dd/MM/yyyy")
                    Case "Double"
                        apici = ""
                        valore = "0.0"
                End Select

                valori = valori & apici & valore & apici & ","
            Next

        End Function


#Region "File writing helper functions"

        Private Function computeIndexLengthInWords() As Int32
            Return ((Records.Count * ShapeFileConstants.IndexRecordByteLength) _
             + ShapeFileConstants.HeaderSizeBytes) / 2

        End Function


        Private Sub WriteHeader(ByRef writer As BinaryWriter, ByVal length As Int32)

            Dim dummyInt As Integer = 0
            Dim dummyDbl As Double = 0

            writer.Seek(0, SeekOrigin.Begin)
            writer.Write(ByteEncoder.GetBigEndian(ShapeFileConstants.HeaderStartCode))
            For idx = 1 To 5
                writer.Write(ByteEncoder.GetBigEndian(dummyInt))
            Next
            'writer.Write(New [Byte](19) {})
            writer.Write(ByteEncoder.GetBigEndian(length))
            writer.Write(ByteEncoder.GetLittleEndian(m_fileHeader.Version))
            writer.Write(ByteEncoder.GetLittleEndian(m_fileHeader.ShapeType))
            writer.Write(ByteEncoder.GetLittleEndian(m_fileHeader.XMin))
            writer.Write(ByteEncoder.GetLittleEndian(m_fileHeader.YMin))
            writer.Write(ByteEncoder.GetLittleEndian(m_fileHeader.XMax))
            writer.Write(ByteEncoder.GetLittleEndian(m_fileHeader.YMax))
            For idx = 1 To 4
                writer.Write(ByteEncoder.GetLittleEndian(dummyDbl))
            Next
            'writer.Write(New [Byte](31) {})
            ' Z-values and M-values
        End Sub

        Private Sub writeRecords()
            For Each rec In Records
                writeGeometry(rec, rec.RecordNumber, rec.ContentLength)
            Next
        End Sub

        Private Sub writeGeometry(g As ShapeFileRecord, recordNumber As UInt32, recordLengthInWords As Int32)

            '_shapeFileStream.Position = _recordOffsetInWords * 2

            _shapeFileWriter.Write(ByteEncoder.GetBigEndian(recordNumber))
            _shapeFileWriter.Write(ByteEncoder.GetBigEndian(recordLengthInWords))

            If g Is Nothing Then
                _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(0))
            End If

            Select Case g.ShapeType
                Case ShapeType.Point
                    writePoint(g)
                    Exit Select
                Case ShapeType.PolyLine
                    writeLineString(g)
                    'todo, ripristina versione originale da c#
                    'If TypeOf g Is ILineString Then
                    '    writeLineString(TryCast(g, ILineString))
                    'ElseIf TypeOf g Is IMultiLineString Then
                    '    writeMultiLineString(TryCast(g, IMultiLineString))
                    'End If
                    Exit Select
                Case ShapeType.Polygon
                    writePolygon(g)
                    Exit Select
                    '              Case ShapeType.Multipoint
                    '                  writeMultiPoint(TryCast(g, IMultiPoint))
                    '                  Exit Select
                    'Case ShapeType.PointZ, ShapeType.PolyLineZ, ShapeType.PolygonZ, ShapeType.MultiPointZ, ShapeType.PointM, ShapeType.PolyLineM, _
                    '	ShapeType.PolygonM, ShapeType.MultiPointM, ShapeType.MultiPatch, ShapeType.Null, Else
                    '                  Throw New NotSupportedException([String].Format("Writing geometry type {0} " & "is not supported in the " & "current version.", ShapeType))
            End Select

            '_shapeFileWriter.Flush()
            _recordOffsetInWords += recordLengthInWords
        End Sub

        Private Sub writeCoordinate(x As [Double], y As [Double])
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(x))
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(y))
        End Sub

        Private Sub writePoint(point As ShapeFileRecord)
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(DirectCast(ShapeType.Point, Int32)))
            writeCoordinate(point.Points(0).X, point.Points(0).Y)
        End Sub

        Private Sub writeBoundingBox(box As ShapeFileRecord)
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.XMin))
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.YMin))
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.XMax))
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(box.YMax))
        End Sub

        'Private Sub writeMultiPoint(multiPoint As IMultiPoint)
        '    _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(DirectCast(ShapeType.MultiPoint, Int32)))
        '    writeBoundingBox(multiPoint.Extents)
        '    _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(multiPoint.Count))

        '    For Each point As IPoint In DirectCast(multiPoint, IEnumerable(Of IPoint))
        '        writeCoordinate(point(Ordinates.X), point(Ordinates.Y))
        '    Next
        'End Sub

        Private Sub writePolygon(polygon As ShapeFileRecord)
            'Dim parts As Int32() = New Int32(polygon.Parts.Count) {}
            'Dim allPoints As New ArrayList()
            'Dim currentPartsIndex As Int32 = 0

            'parts(System.Math.Max(System.Threading.Interlocked.Increment(currentPartsIndex), currentPartsIndex - 1)) = 0
            'allPoints.AddRange(polygon.ExteriorRing.Coordinates)

            'For Each ring As ILinearRing In polygon.InteriorRing
            '    parts(System.Math.Max(System.Threading.Interlocked.Increment(currentPartsIndex), currentPartsIndex - 1)) = allPoints.Count
            '    allPoints.AddRange(ring.Coordinates)
            'Next

            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(DirectCast(ShapeType.Polygon, Int32)))
            writePolySegments(polygon, polygon.Parts, polygon.Points, polygon.NumberOfPoints)
        End Sub


        Private Sub writePolySegments(extents As ShapeFileRecord, parts As Collection(Of Integer), points As Collection(Of System.Windows.Point), pointCount As Int32)
            writeBoundingBox(extents)
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(parts.Count))
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(pointCount))

            For Each partIndex As Int32 In parts
                _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(partIndex + 1))
            Next

            For Each point In points
                writeCoordinate(point.X, point.Y)
            Next
        End Sub

        Private Sub writeLineString(lineString As ShapeFileRecord)
            _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(DirectCast(ShapeType.PolyLine, Int32)))
            Dim parts As New Collection(Of Integer)
            parts.Add(0)
            writePolySegments(lineString, parts, lineString.Points, lineString.Points.Count)
        End Sub

        'Private Sub writeMultiLineString(multiLineString As IMultiLineString)
        '    Dim parts As Int32() = New Int32(multiLineString.Count - 1) {}
        '    Dim allPoints As New ArrayList()

        '    Dim currentPartsIndex As Int32 = 0

        '    For Each line As ILineString In DirectCast(multiLineString, IEnumerable(Of ILineString))
        '        parts(System.Math.Max(System.Threading.Interlocked.Increment(currentPartsIndex), currentPartsIndex - 1)) = allPoints.Count
        '        allPoints.AddRange(line.Coordinates)
        '    Next

        '    _shapeFileWriter.Write(ByteEncoder.GetLittleEndian(DirectCast(ShapeType.PolyLine, Int32)))
        '    writePolySegments(multiLineString.Extents, parts, allPoints, allPoints.Count)
        'End Sub


#End Region


#End Region
    End Class
End Namespace

' END

