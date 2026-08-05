

Imports System.IO
Imports System.Text


Namespace ShapeFile_ESRI


    ''' <summary>
    ''' Provides a readonly IEnumerable interface to an ESRI Shapefile.
    ''' NOTE - has not been designed to be thread safe
    ''' </summary>
    ''' <remarks>
    ''' See the ESRI Shapefile specification at http://www.esri.com/library/whitepapers/pdfs/shapefile.pdf
    ''' </remarks>
    Public Class ShapeFile
        Implements IDisposable, IEnumerator(Of Shape), IEnumerable(Of Shape)

        Private _disposed As Boolean
        Private _opened As Boolean
        Private _rawMetadataOnly As Boolean
        Private _currentIndex As Integer = -1
        Private _count As Integer
        Private _boundingBox As RectangleD
        Private _type As ShapeType
        Private _mainStream As Stream
        Private _indexStream As Stream
        Private _dbfStream As Stream
        Private _mainHeader As Header
        Private _indexHeader As Header
        Private _dbfReader As DBFReader

        ''' <summary>
        ''' Create a new Shapefile object.
        ''' </summary>
        Public Sub New()
            Me.New(Nothing)
        End Sub

        ''' <summary>
        ''' Create a new Shapefile object and open a Shapefile. Note that three files are required - 
        ''' the main file (.shp), the index file (.shx) and the dBASE table (.dbf). The three files 
        ''' must all have the same filename (i.e. shapes.shp, shapes.shx and shapes.dbf). Set path
        ''' to any one of these three files to open the Shapefile.
        ''' </summary>
        ''' <param name="path">Path to the .shp, .shx or .dbf file for this Shapefile</param>
        Public Sub New(path As String)

            If path IsNot Nothing Then

                Open(path)
            End If
        End Sub

        ''' <summary>
        ''' Create a new Shapefile object and open a Shapefile. Note that three files are required - 
        ''' the main file (.shp), the index file (.shx) and the dBASE table (.dbf). The three files 
        ''' must all have the same filename (i.e. shapes.shp, shapes.shx and shapes.dbf). Set path
        ''' to any one of these three files to open the Shapefile.
        ''' </summary>
        ''' <param name="path">Path to the .shp, .shx or .dbf file for this Shapefile</param>
        Public Sub Open(path As String)

            If _disposed Then
                Throw New ObjectDisposedException("Shapefile")
            End If

            If path Is Nothing Then
                Throw New ArgumentNullException("path")
            End If

            If path.Length <= 0 Then
                Throw New ArgumentException("path parameter is empty", "path")
            End If

            Dim shapefileMainPath As String = IO.Path.ChangeExtension(path, "shp")
            Dim shapefileIndexPath As String = IO.Path.ChangeExtension(path, "shx")
            Dim shapefileDbasePath As String = IO.Path.ChangeExtension(path, "dbf")

            If Not File.Exists(shapefileMainPath) Then
                Throw New FileNotFoundException("Shapefile main file not found", shapefileMainPath)
            End If
            If Not File.Exists(shapefileIndexPath) Then
                Throw New FileNotFoundException("Shapefile index file not found", shapefileIndexPath)
            End If
            If Not File.Exists(shapefileDbasePath) Then
                Throw New FileNotFoundException("Shapefile dBase file not found", shapefileDbasePath)
            End If

            _mainStream = File.Open(shapefileMainPath, FileMode.Open, FileAccess.Read, FileShare.Read)
            _indexStream = File.Open(shapefileIndexPath, FileMode.Open, FileAccess.Read, FileShare.Read)
            _dbfStream = File.Open(shapefileDbasePath, FileMode.Open, FileAccess.Read, FileShare.Read)

            If _mainStream.Length < Header.HeaderLength Then
                Throw New InvalidOperationException("Shapefile main file does not contain a valid header")
            End If

            If _indexStream.Length < Header.HeaderLength Then
                Throw New InvalidOperationException("Shapefile index file does not contain a valid header")
            End If

            ' read in and parse the headers
            Dim headerBytes(Header.HeaderLength - 1) As Byte
            _mainStream.Read(headerBytes, 0, Header.HeaderLength)
            _mainHeader = New Header(headerBytes)
            _indexStream.Read(headerBytes, 0, Header.HeaderLength)
            _indexHeader = New Header(headerBytes)

            ' set properties from the main header
            _type = _mainHeader.ShapeType
            _boundingBox = New RectangleD With {
                .Left = _mainHeader.XMin,
                .Top = _mainHeader.YMin,
                .Right = _mainHeader.XMax,
                .Bottom = _mainHeader.YMax
            }

            ' index header length Is in 16-bit words, including the header - number of 
            ' shapes Is the number of records (each 4 workds long) after subtracting the header bytes
            _count = (_indexHeader.FileLength - (Header.HeaderLength / 2)) / 4

            ' open the metadata database
            _dbfReader = New DBFReader(_dbfStream, Encoding.ASCII)

            _opened = True
        End Sub

        ''' <summary>
        ''' Close the Shapefile. Equivalent to calling Dispose().
        ''' </summary>
        Public Sub Close()
            Dispose()
        End Sub

        ''' <summary>
        ''' If true then only the IDataRecord (DataRecord) property is available to access metadata for each shape.
        ''' If false (the default) then metadata is also parsed into a string dictionary (use GetMetadataNames() and GetMetadata() to access)
        ''' </summary>
        ''' <returns></returns>
        Public Property RawMetadataOnly As Boolean
            Get
                Return _rawMetadataOnly
            End Get
            Set(value As Boolean)
                _rawMetadataOnly = value
            End Set
        End Property

        ''' <summary>
        ''' Gets the number of shapes in the Shapefile
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Count As Integer
            Get
                If _disposed Then
                    Throw New ObjectDisposedException("Shapefile")
                End If

                If Not _opened Then
                    Throw New InvalidOperationException("Shapefile not open.")
                End If

                Return _count
            End Get
        End Property

        ''' <summary>
        ''' Gets the bounding box for the Shapefile
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BoundingBox As RectangleD
            Get
                If _disposed Then
                    Throw New ObjectDisposedException("Shapefile")
                End If

                If Not _opened Then
                    Throw New InvalidOperationException("Shapefile not open.")
                End If

                Return _boundingBox
            End Get
        End Property

        ''' <summary>
        ''' Gets the ShapeType of the Shapefile
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Type As ShapeType
            Get
                If _disposed Then
                    Throw New ObjectDisposedException("Shapefile")
                End If

                If Not _opened Then
                    Throw New InvalidOperationException("Shapefile not open.")
                End If

                Return _type
            End Get
        End Property

#Region "IEnumerator Members"

        ''' <summary>
        ''' Gets the current shape in the collection
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Current As Shape Implements IEnumerator(Of Shape).Current
            Get
                If _disposed Then
                    Throw New ObjectDisposedException("Shapefile")
                End If

                If Not _opened Then
                    Throw New InvalidOperationException("Shapefile not open.")
                End If

                'get the metadata
                Dim metadata As Dictionary(Of String, String) = Nothing
                Dim metatype As Dictionary(Of String, Type) = Nothing

                If Not _rawMetadataOnly Then

                    metadata = _dbfReader.ReadRecord(_currentIndex)
                    metatype = _dbfReader.ReadRecordType(_currentIndex)
                End If

                'get the index record
                Dim indexHeaderBytes(7) As Byte
                _indexStream.Seek(Header.HeaderLength + _currentIndex * 8, SeekOrigin.Begin)
                _indexStream.Read(indexHeaderBytes, 0, indexHeaderBytes.Length)
                Dim contentOffsetInWords As Integer = EndianBitConverter.ToInt32(indexHeaderBytes, 0, ProvidedOrder.Big)
                Dim contentLengthInWords As Integer = EndianBitConverter.ToInt32(indexHeaderBytes, 4, ProvidedOrder.Big)

                'get the data chunk from the main file - need to factor in 8 byte record header
                Dim hdrBytesToRead As Integer = 8
                Dim recHeader(hdrBytesToRead - 1) As Byte
                Dim bytesToRead As Integer = (contentLengthInWords * 2)
                Dim shapeData(bytesToRead - 1) As Byte
                _mainStream.Seek(contentOffsetInWords * 2, SeekOrigin.Begin)
                _mainStream.Read(recHeader, 0, hdrBytesToRead)
                _mainStream.Read(shapeData, 0, bytesToRead)

                ' Creates a Shape object (or derived object) from a shape record

                If shapeData.Length < 4 Then
                    Throw New ArgumentException("shapeData must be at least 4 bytes long")
                End If

                ' shape data contains a header (shape number and content length)
                ' the first field in each shape is the shape type

                'Position  Field           Value                   Type        Order
                'Byte 0    Record          Number Record Number    Integer     Big
                'Byte 4    Content Length  Content Length          Integer     Big

                'Position  Field       Value                   Type        Number      Order
                'Byte 0    Shape Type  Shape Type              Integer     1           Little

                Dim recordNumber As Integer = EndianBitConverter.ToInt32(recHeader, 0, ProvidedOrder.Big)
                Dim shapeContentLengthInWords As Integer = EndianBitConverter.ToInt32(recHeader, 4, ProvidedOrder.Big)
                Dim type As ShapeType = EndianBitConverter.ToInt32(shapeData, 0, ProvidedOrder.Little)

                ' test that we have the expected amount of data - need to take the 8 byte header into account
                If shapeData.Length <> (shapeContentLengthInWords * 2) Then
                    Throw New InvalidOperationException("Shape data length does not match shape header length")
                End If

                Dim _shape As Shape = Nothing

                Select Case type

                    Case ShapeType.Null
                        _shape = New ShapeNull(recordNumber, metadata, metatype)

                    Case ShapeType.Point
                        _shape = New ShapePoint(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PolyLine
                        _shape = New ShapePolyLine(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.Polygon
                        _shape = New ShapePolygon(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.MultiPoint
                        _shape = New ShapeMultiPoint(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PointZ
                        _shape = New ShapePointZ(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PolyLineZ
                        _shape = New ShapePolyLineZ(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PolygonZ
                        _shape = New ShapePolygonZ(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.MultiPointZ
                        '_shape = New ShapeMultiPointZ(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PointM
                        _shape = New ShapePointM(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PolyLineM
                        _shape = New ShapePolyLineM(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.PolygonM
                        _shape = New ShapePolygonM(recordNumber, metadata, metatype, shapeData)

                    Case ShapeType.MultiPointM
                        '_shape = New ShapeMultiPointM(recordNumber, metadata, shapeData)

                    Case ShapeType.MultiPatch
                        '_shape = New ShapeMultiPointM(recordNumber, metadata, shapeData)

                End Select

                If _shape Is Nothing Then

                    Throw New NotImplementedException(String.Format("Shapetype {0} is not implemented", type))
                End If

                Return _shape
            End Get
        End Property

        ''' <summary>
        ''' Gets the current item in the collection
        ''' </summary>
        ''' <returns></returns>
        Private ReadOnly Property IEnumerator_Current As Object Implements IEnumerator.Current
            Get
                Return Me.Current()
            End Get
        End Property

        ''' <summary>
        ''' Reset the enumerator
        ''' </summary>
        Public Sub Reset() Implements IEnumerator.Reset
            If _disposed Then
                Throw New ObjectDisposedException("Shapefile")
            End If
            If Not _opened Then
                Throw New InvalidOperationException("Shapefile not open.")
            End If
            _currentIndex = -1
        End Sub

        ''' <summary>
        ''' Move to the next item in the collection (returns false if at the end)
        ''' </summary>
        ''' <returns></returns>
        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext

            If _disposed Then
                Throw New ObjectDisposedException("Shapefile")
            End If
            If Not _opened Then
                Throw New InvalidOperationException("Shapefile not open.")
            End If

            If _currentIndex < _count - 1 Then

                _currentIndex += 1

                Return True
            End If
            'reached the last shape
            Return False
        End Function

#End Region


#Region "IEnumerable members"

        Public Function GetEnumerator() As IEnumerator(Of Shape) Implements IEnumerable(Of Shape).GetEnumerator
            Return Me
        End Function

        Private Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me
        End Function

#End Region


#Region "IDisposable Members"

        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not _disposed Then

                If disposing Then

                    If _mainStream IsNot Nothing Then
                        _mainStream.Dispose()
                        '_mainStream = Nothing
                    End If

                    If _indexStream IsNot Nothing Then
                        _indexStream.Dispose()
                        '_indexStream = Nothing
                    End If

                    If _dbfReader IsNot Nothing Then
                        _dbfReader.Dispose()
                    End If

                    'CloseDb()
                End If

                _disposed = True
                _opened = False
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

#End Region

    End Class





    ''' <summary>
    ''' The header data for a Shapefile main file or Index file
    ''' </summary>
    Friend Class Header
        ''' <summary>
        ''' The length of a Shapefile header in bytes
        ''' </summary>
        Public Const HeaderLength As Integer = 100

        Private Const ExpectedFileCode As Integer = 9994
        Private Const ExpectedVersion As Integer = 1000

        Private _fileCode As Integer
        Private _fileLength As Integer
        Private _version As Integer
        Private _shapeType As ShapeType
        Private _xMin As Double
        Private _yMin As Double
        Private _xMax As Double
        Private _yMax As Double
        Private _zMin As Double
        Private _zMax As Double
        Private _mMin As Double
        Private _mMax As Double

        ''' <summary>
        ''' The header data for a Shapefile main file or Index file
        ''' </summary>
        ''' <param name="headerBytes">The first 100 bytes of the Shapefile main file or Index file</param>
        Public Sub New(headerBytes As Byte())

            If headerBytes Is Nothing Then
                Throw New ArgumentNullException("headerBytes")
            End If

            If headerBytes.Length <> HeaderLength Then
                Throw New InvalidOperationException(String.Format("headerBytes must be {0} bytes long", HeaderLength))
            End If

            'Position  Field           Value       Type        Order
            'Byte 0    File Code       9994        Integer     Big
            'Byte 4    Unused          0           Integer     Big
            'Byte 8    Unused          0           Integer     Big
            'Byte 12   Unused          0           Integer     Big
            'Byte 16   Unused          0           Integer     Big
            'Byte 20   Unused          0           Integer     Big
            'Byte 24   File Length     File Length Integer     Big
            'Byte 28   Version         1000        Integer     Little
            'Byte 32   Shape Type      Shape Type  Integer     Little
            'Byte 36   Bounding Box    Xmin        Double      Little
            'Byte 44   Bounding Box    Ymin        Double      Little
            'Byte 52   Bounding Box    Xmax        Double      Little
            'Byte 60   Bounding Box    Ymax        Double      Little
            'Byte 68*  Bounding Box    Zmin        Double      Little
            'Byte 76*  Bounding Box    Zmax        Double      Little
            'Byte 84*  Bounding Box    Mmin        Double      Little
            'Byte 92*  Bounding Box    Mmax        Double      Little

            _fileCode = EndianBitConverter.ToInt32(headerBytes, 0, ProvidedOrder.Big)

            If _fileCode <> ExpectedFileCode Then
                Throw New InvalidOperationException(String.Format("Header File code is {0}, expected {1}", _fileCode, ExpectedFileCode))
            End If

            _version = EndianBitConverter.ToInt32(headerBytes, 28, ProvidedOrder.Little)

            If _version <> ExpectedVersion Then
                Throw New InvalidOperationException(String.Format("Header version is {0}, expected {1}", _version, ExpectedVersion))
            End If

            _fileLength = EndianBitConverter.ToInt32(headerBytes, 24, ProvidedOrder.Big)
            _shapeType = EndianBitConverter.ToInt32(headerBytes, 32, ProvidedOrder.Little)
            _xMin = EndianBitConverter.toDouble(headerBytes, 36, ProvidedOrder.Little)
            _yMin = EndianBitConverter.toDouble(headerBytes, 44, ProvidedOrder.Little)
            _xMax = EndianBitConverter.toDouble(headerBytes, 52, ProvidedOrder.Little)
            _yMax = EndianBitConverter.toDouble(headerBytes, 60, ProvidedOrder.Little)
            _zMin = EndianBitConverter.toDouble(headerBytes, 68, ProvidedOrder.Little)
            _zMax = EndianBitConverter.toDouble(headerBytes, 76, ProvidedOrder.Little)
            _mMin = EndianBitConverter.toDouble(headerBytes, 84, ProvidedOrder.Little)
            _mMax = EndianBitConverter.toDouble(headerBytes, 92, ProvidedOrder.Little)
        End Sub
        ''' <summary>
        ''' Gets the FileCode
        ''' </summary>
        Public ReadOnly Property FileCode As Integer
            Get
                Return _fileCode
            End Get
        End Property
        ''' <summary>
        ''' Gets the file length, in 16-bit words, including the header bytes
        ''' </summary>
        Public ReadOnly Property FileLength As Integer
            Get
                Return _fileLength
            End Get
        End Property
        ''' <summary>
        ''' Gets the file version
        ''' </summary>
        Public ReadOnly Property Version As Integer
            Get
                Return _version
            End Get
        End Property
        ''' <summary>
        ''' Gets the ShapeType contained in this Shapefile
        ''' </summary>
        Public ReadOnly Property ShapeType As ShapeType
            Get
                Return _shapeType
            End Get
        End Property
        ''' <summary>
        ''' Gets min x for the bounding box
        ''' </summary>
        Public ReadOnly Property XMin As Double
            Get
                Return _xMin
            End Get
        End Property
        ''' <summary>
        ''' Gets min y for the bounding box
        ''' </summary>
        Public ReadOnly Property YMin As Double
            Get
                Return _yMin
            End Get
        End Property
        ''' <summary>
        ''' Gets max x for the bounding box 
        ''' </summary>
        Public ReadOnly Property XMax As Double
            Get
                Return _xMax
            End Get
        End Property
        ''' <summary>
        ''' Gets max y for the bounding box
        ''' </summary>
        Public ReadOnly Property YMax As Double
            Get
                Return _yMax
            End Get
        End Property
        ''' <summary>
        ''' Gets min z for the bounding box (0 if unused)
        ''' </summary>
        Public ReadOnly Property ZMin As Double
            Get
                Return _zMin
            End Get
        End Property
        ''' <summary>
        ''' Gets max z for the bounding box (0 if unused)
        ''' </summary>
        Public ReadOnly Property ZMax As Double
            Get
                Return _zMax
            End Get
        End Property
        ''' <summary>
        ''' Gets min m for the bounding box (0 if unused)
        ''' </summary>
        Public ReadOnly Property MMin As Double
            Get
                Return _mMin
            End Get
        End Property
        ''' <summary>
        ''' Gets max m for the bounding box (0 if unused)
        ''' </summary>
        Public ReadOnly Property MMax As Double
            Get
                Return _mMax
            End Get
        End Property
    End Class

End Namespace