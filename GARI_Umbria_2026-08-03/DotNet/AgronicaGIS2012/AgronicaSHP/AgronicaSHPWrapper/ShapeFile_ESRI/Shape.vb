

Namespace ShapeFile_ESRI

    ''' <summary>
    ''' The ShapeType of a shape in a Shapefile
    ''' </summary>
    Public Enum ShapeType
        ''' <summary>
        ''' Null shape
        ''' </summary>
        Null = 0
        ''' <summary>
        ''' Point Shape
        ''' </summary>
        Point = 1
        ''' <summary>
        ''' PolyLine Shape 
        ''' </summary>
        PolyLine = 3
        ''' <summary>
        ''' Polygon Shape
        ''' </summary>
        Polygon = 5
        ''' <summary>
        ''' MultiPoint Shape
        ''' </summary>
        MultiPoint = 8
        ''' <summary>
        ''' PointZ Shape
        ''' </summary>
        PointZ = 11
        ''' <summary>
        ''' PolyLineZ Shape
        ''' </summary>
        PolyLineZ = 13
        ''' <summary>
        ''' PolygonZ Shape
        ''' </summary>
        PolygonZ = 15
        ''' <summary>
        ''' MultiPointZ Shape
        ''' </summary>
        MultiPointZ = 18
        ''' <summary>
        ''' PointM Shape
        ''' </summary>
        PointM = 21
        ''' <summary>
        ''' PolyLineM Shape
        ''' </summary>
        PolyLineM = 23
        ''' <summary>
        ''' PolygonM Shape
        ''' </summary>
        PolygonM = 25
        ''' <summary>
        ''' MultiPointM Shape
        ''' </summary>
        MultiPointM = 28
        ''' <summary>
        ''' MultiPatch Shape
        ''' </summary>
        MultiPatch = 31
    End Enum



    ''' <summary>
    ''' A simple double precision point
    ''' </summary>
    Public Structure PointD
        ''' <summary>
        ''' Gets or sets the X value
        ''' </summary>
        Public X As Double
        ''' <summary>
        ''' Gets or sets the Y value
        ''' </summary>
        Public Y As Double
    End Structure



    ''' <summary>
    ''' A simple double precision pointZ
    ''' </summary>
    Public Structure PointZ
        ''' <summary>
        ''' Gets or sets the X value
        ''' </summary>
        Public X As Double
        ''' <summary>
        ''' Gets or sets the Y value
        ''' </summary>
        Public Y As Double
        ''' <summary>
        ''' Gets or sets the Z value
        ''' </summary>
        Public Z As Double
        ''' <summary>
        ''' Gets or sets the M value
        ''' </summary>
        Public M As Double
    End Structure



    ''' <summary>
    ''' A simple double precision pointM
    ''' </summary>
    Public Structure PointM
        ''' <summary>
        ''' Gets or sets the X value
        ''' </summary>
        Public X As Double
        ''' <summary>
        ''' Gets or sets the Y value
        ''' </summary>
        Public Y As Double
        ''' <summary>
        ''' Gets or sets the M value
        ''' </summary>
        Public M As Double
    End Structure



    ''' <summary>
    ''' A simple double precision rectangle
    ''' </summary>
    Public Structure RectangleD
        ''' <summary>
        ''' Gets or sets the left value
        ''' </summary>
        Public Left As Double
        ''' <summary>
        ''' Gets or sets the top value
        ''' </summary>
        Public Top As Double
        ''' <summary>
        ''' Gets or sets the right value
        ''' </summary>
        Public Right As Double
        ''' <summary>
        ''' Gets or sets the bottom value
        ''' </summary>
        Public Bottom As Double
    End Structure



    Public Structure MinMaxArray
        Public _min As Double
        Public _max As Double
        Public _array As Double()
    End Structure

    ''' <summary>
    ''' Base Shapefile shape - contains only the shape type, record numbere and metadata 
    ''' </summary>
    Public MustInherit Class Shape

        Private ReadOnly _type As ShapeType
        Private ReadOnly _recordNumber As Integer
        Private ReadOnly _metadata As Dictionary(Of String, String)
        Private ReadOnly _metatype As Dictionary(Of String, Type)

        ''' <summary>
        ''' Get the ShapeType of this shape
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Type As ShapeType
            Get
                Return _type
            End Get
        End Property

        ''' <summary>
        ''' Gets the record number associated with this shape
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property RecordNumber As Integer
            Get
                Return _recordNumber
            End Get
        End Property

        ''' <summary>
        ''' Base Shapefile shape - contains only the shape type and metadata plus helper methods.
        ''' An instance of Shape is the Null ShapeType. If the Type field is not ShapeType.Null then
        ''' cast to the more specific shape (i.e. ShapePolygon).
        ''' </summary>
        ''' <param name="shapeType">The ShapeType of the shape</param>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape (optional)</param>
        Protected Friend Sub New(type As ShapeType, recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type))
            _type = type
            _recordNumber = recordNumber
            _metadata = metadata
            _metatype = metatype
        End Sub

        ''' <summary>
        ''' Gets the metadata (as a string) for a given name (key). Valid names
        ''' for this shape can be retrieved by calling GetMetadataNames().
        ''' </summary>
        ''' <param name="name">The name to retreieve</param>
        ''' <returns>The metadata string, or null if the requested name does not exist</returns>
        Public Function GetMetadata(name As String) As String
            Dim u_name As String = name.ToUpper
            If _metadata IsNot Nothing AndAlso _metadata.ContainsKey(u_name) Then
                Return _metadata(u_name)
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Gets the metatype (as a string) for a given name (key). Valid names
        ''' for this shape can be retrieved by calling GetMetadataNames().
        ''' </summary>
        ''' <param name="name">The name to retreieve</param>
        ''' <returns>The metatype type, or null if the requested name does not exist</returns>
        Public Function GetMetatype(name As String) As Type
            Dim u_name As String = name.ToUpper
            If _metatype IsNot Nothing AndAlso _metatype.ContainsKey(u_name) Then
                Return _metatype(u_name)
            End If
            Return Nothing
        End Function

        Public Function GetMetadataOf(name As String) As Object
            Dim u_name As String = name.ToUpper
            Dim tipo As Type
            If _metatype IsNot Nothing AndAlso _metatype.ContainsKey(u_name) Then
                tipo = _metatype(u_name)

                Select Case True
                    Case tipo.Equals(GetType(Integer))
                        Return Convert.ToInt32(_metadata(u_name))
                    Case tipo.Equals(GetType(Double))
                        'If Not Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(".") Then
                        '    _metadata(u_name).Replace(".", Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        'End If
                        'Return CType(CObj(_metadata(u_name)), Double)
                        Return Convert.ToDouble(_metadata(u_name), Globalization.CultureInfo.InvariantCulture)
                    Case tipo.Equals(GetType(Decimal))
                        'If Not Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator.Equals(".") Then
                        '    _metadata(u_name).Replace(".", Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
                        'End If
                        'Return CType(CObj(_metadata(u_name)), Decimal)

                        Return Convert.ToDecimal(_metadata(u_name), Globalization.CultureInfo.InvariantCulture)
                    Case tipo.Equals(GetType(String))
                        'Return CType(CObj(_metadata(u_name)), String)
                        Return _metadata(u_name).ToString()
                    Case tipo.Equals(GetType(Date))
                        'Return CType(CObj(_metadata(u_name)), Date)
                        Return Convert.ToDateTime(_metadata(u_name))
                    Case tipo.Equals(GetType(DateTime))
                        'Return CType(CObj(_metadata(u_name)), DateTime)
                        Return Convert.ToDateTime(_metadata(u_name))
                    Case tipo.Equals(GetType(Boolean))
                        'Return CType(CObj(_metadata(u_name)), Boolean)
                        Return Convert.ToBoolean(_metadata(u_name))
                    Case Else
                        'Return CType(CObj(_metadata(u_name)), String)
                        Return _metadata(u_name).ToString()
                End Select
            End If
            Return Nothing
        End Function

        Public Function GetAllMetadata() As Dictionary(Of String, String)
            Return _metadata
        End Function

        Public Function GetAllMetatype() As Dictionary(Of String, Type)
            Return _metatype
        End Function

        ''' <summary>
        ''' Gets an array of valid metadata names (keys) for this shape. Returns null if not metadata exists.
        ''' </summary>
        ''' <returns>Array of metadata names, or null of no metadata exists</returns>
        Public Function GetMetadataNames() As IEnumerable(Of String)
            If _metadata IsNot Nothing AndAlso _metadata.Keys.Count > 0 Then

                Dim names As New List(Of String)
                For Each key In _metadata.Keys
                    names.Add(key)
                Next
                Return names.ToArray()
            End If
            Return Nothing
        End Function

        Protected Function ReadPointD(data As Byte(), startIndex As Integer) As PointD
            Return New PointD With {
                .X = EndianBitConverter.toDouble(data, startIndex, ProvidedOrder.Little),
                .Y = EndianBitConverter.toDouble(data, startIndex + 8, ProvidedOrder.Little)
            }
        End Function

        Protected Function ReadPointM(data As Byte(), startIndex As Integer) As PointM
            Return New PointM With {
                .X = EndianBitConverter.toDouble(data, startIndex, ProvidedOrder.Little),
                .Y = EndianBitConverter.toDouble(data, startIndex + 8, ProvidedOrder.Little),
                .M = EndianBitConverter.toDouble(data, startIndex + 16, ProvidedOrder.Little)
            }
        End Function

        Protected Function ReadPointZ(data As Byte(), startIndex As Integer) As PointZ
            Return New PointZ With {
                .X = EndianBitConverter.toDouble(data, 4, ProvidedOrder.Little),
                .Y = EndianBitConverter.toDouble(data, 12, ProvidedOrder.Little),
                .Z = EndianBitConverter.toDouble(data, 20, ProvidedOrder.Little),
                .M = EndianBitConverter.toDouble(data, 28, ProvidedOrder.Little)
            }
        End Function

        ''' <summary>
        ''' Extracts a double precision rectangle (RectangleD) from a byte array - assumes that
        ''' the called has validated that there is enough space in the byte array for four doubles (32 bytes)
        ''' </summary>
        ''' <param name="value">byte array</param>
        ''' <param name="startIndex">start index in the array</param>
        ''' <returns>The RectangleD</returns>
        Protected Function ReadRectangelD(data As Byte(), startIndex As Integer) As RectangleD
            Return New RectangleD With {
                .Left = EndianBitConverter.toDouble(data, startIndex, ProvidedOrder.Little),
                .Top = EndianBitConverter.toDouble(data, startIndex + 8, ProvidedOrder.Little),
                .Right = EndianBitConverter.toDouble(data, startIndex + 16, ProvidedOrder.Little),
                .Bottom = EndianBitConverter.toDouble(data, startIndex + 24, ProvidedOrder.Little)
            }
        End Function

        Protected Function ReadMinMaxArray(data As Byte(), startIndex As Integer, numPoints As Integer) As MinMaxArray

            Dim mma As New MinMaxArray
            ReDim mma._array(numPoints - 1)

            mma._min = EndianBitConverter.toDouble(data, startIndex, ProvidedOrder.Little)
            mma._max = EndianBitConverter.toDouble(data, startIndex + 8, ProvidedOrder.Little)

            For i As Integer = 0 To numPoints - 1
                mma._array(i) = EndianBitConverter.toDouble(data, startIndex + 16 + (i * 8), ProvidedOrder.Little)
            Next

            Return mma
        End Function
    End Class



    Public Class ShapeNull
        Inherits Shape

        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type))
            MyBase.New(ShapeType.Null, recordNumber, metadata, metatype)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile Point Shape
    ''' </summary>
    Public Class ShapePoint
        Inherits Shape

        Private _point As PointD

        ''' <summary>
        ''' Gets the point
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Point As PointD
            Get
                Return _point
            End Get
        End Property

        ''' <summary>
        ''' A Shapefile Point Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.Point, recordNumber, metadata, metatype)

            ' Position     Field       Value   Type        Number  Order
            ' Byte 0       Shape Type  1       Integer     1       Little
            ' Byte 4       X           X       Double      1       Little
            ' Byte 12      Y           Y       Double      1       Little

            If shapeData Is Nothing Then
                Throw New ArgumentNullException("shapeData")
            End If

            If shapeData.Length <> 20 Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            _point = ReadPointD(shapeData, 4)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile PointZ Shape
    ''' </summary>
    Public Class ShapePointZ
        Inherits Shape

        Private _point As PointZ

        ''' <summary>
        ''' Gets the point
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Point As PointZ
            Get
                Return _point
            End Get
        End Property

        ''' <summary>
        ''' A Shapefile Point Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PointZ, recordNumber, metadata, metatype)

            ' Position     Field       Value   Type        Number  Order
            ' Byte 0       Shape Type  1       Integer     1       Little
            ' Byte 4       X           X       Double      1       Little
            ' Byte 12      Y           Y       Double      1       Little
            ' Byte 20      Z           Z       Double      1       Little
            ' Byte 28      Measure     M       Double      1       Little

            ' metadata is validated by the base class
            If shapeData Is Nothing Then
                Throw New ArgumentNullException("shapeData")
            End If

            ' validation - shapedata should be 4 + 8 + 8 + 8 + 8 = 36 bytes long

            ' M è optional???
            If shapeData.Length < 28 Then

                Throw New InvalidOperationException("Invalid shape data")
            End If

            If shapeData.Length < 36 Then

                _point = New PointZ With {
                    .X = EndianBitConverter.toDouble(shapeData, 4, ProvidedOrder.Little),
                    .Y = EndianBitConverter.toDouble(shapeData, 12, ProvidedOrder.Little),
                    .Z = EndianBitConverter.toDouble(shapeData, 20, ProvidedOrder.Little)
                }

                Return
            End If

            _point = ReadPointZ(shapeData, 4)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile PointM Shape
    ''' </summary>
    Public Class ShapePointM
        Inherits Shape

        Private _point As PointM

        ''' <summary>
        ''' Gets the point
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Point As PointM
            Get
                Return _point
            End Get
        End Property

        ''' <summary>
        ''' A Shapefile PointM Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PointM, recordNumber, metadata, metatype)

            ' Position     Field       Value   Type        Number  Order
            ' Byte 0       Shape Type  1       Integer     1       Little
            ' Byte 4       X           X       Double      1       Little
            ' Byte 12      Y           Y       Double      1       Little
            ' Byte 20      Measure     M       Double      1       Little

            ' metadata is validated by the base class
            If shapeData Is Nothing Then
                Throw New ArgumentNullException("shapeData")
            End If

            ' validation - shapedata should be 4 + 8 + 8 + 8 = 28 bytes long

            If shapeData.Length <> 28 Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            _point = ReadPointM(shapeData, 4)
        End Sub
    End Class



    Public MustInherit Class BoundedShape
        Inherits Shape

        ''' <summary>
        ''' Bounding Box
        ''' </summary>
        Private ReadOnly _boundingBox As RectangleD

        ''' <summary>
        ''' Gets the bounding box
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BoundingBox As RectangleD
            Get
                Return _boundingBox
            End Get
        End Property

        Protected Friend Sub New(type As ShapeType, recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(type, recordNumber, metadata, metatype)

            ' metadata is validated by the base class
            If shapeData Is Nothing Then
                Throw New ArgumentNullException("shapeData")
            End If

            ' Position     Field       Value       Type        Number      Order
            ' Byte 0       Shape Type  3 or 5      Integer     1           Little
            ' Byte 4       Box         Box         Double      4           Little

            ' validation step 1 - must have at least 4 + (4 * 8) bytes = 36
            If shapeData.Length < 36 Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            ' extract bounding box
            _boundingBox = ReadRectangelD(shapeData, 4)
        End Sub



    End Class



    Public MustInherit Class AbsShapePoly
        Inherits BoundedShape

        ''' <summary>
        ''' List of parts
        ''' </summary>
        Private _parts As List(Of PointD())

        ''' <summary>
        ''' Gets a list of parts (segments) for the PolyLine. Each part is an array of double precision points
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Parts As List(Of PointD())
            Get
                Return _parts
            End Get
        End Property

        Protected Friend Sub New(type As ShapeType, recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(type, recordNumber, metadata, metatype, shapeData)

            ' Position     Field       Value       Type        Number      Order
            ' Byte 0       Shape Type  3 or 5      Integer     1           Little
            ' Byte 4       Box         Box         Double      4           Little
            ' Byte 36      NumParts    NumParts    Integer     1           Little
            ' Byte 40      NumPoints   NumPoints   Integer     1           Little
            ' Byte 44      Parts       Parts       Integer     NumParts    Little
            ' Byte X       Points      Points      Point       NumPoints   Little
            '
            ' Note: X = 44 + 4 * NumParts

            ' validation step 1 - must have at least 4 + (4 * 8) + 4 + 4 bytes = 44
            If shapeData.Length < 44 Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            ' extract number of parts and number of points
            Dim numParts As Integer = EndianBitConverter.ToInt32(shapeData, 36, ProvidedOrder.Little)
            Dim numPoints As Integer = EndianBitConverter.ToInt32(shapeData, 40, ProvidedOrder.Little)

            Dim partsOffset As Integer = 44
            Dim pointsOffset As Integer = partsOffset + (4 * numParts)

            ' validation step 2 - we're expecting 44 + 4 * numParts + 16 * numPoints bytes total
            If shapeData.Length < pointsOffset + (16 * numPoints) Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            ' now extract the parts
            _parts = New List(Of PointD())

            For p = 0 To numParts - 1

                ' this is the index of the start of the part in the points array
                Dim currPointIndex As Integer = EndianBitConverter.ToInt32(shapeData, partsOffset + (4 * p), ProvidedOrder.Little)
                Dim nextPointIndex As Integer = numPoints

                If p < numParts - 1 Then

                    ' we need to get the next part
                    nextPointIndex = EndianBitConverter.ToInt32(shapeData, partsOffset + (4 * (p + 1)), ProvidedOrder.Little)
                End If

                Dim numPointsInPart As Integer = nextPointIndex - currPointIndex

                Dim points(numPointsInPart - 1) As PointD

                For pt = 0 To numPointsInPart - 1

                    points(pt) = ReadPointD(shapeData, pointsOffset + (16 * (currPointIndex + pt)))
                Next

                _parts.Add(points)
            Next
        End Sub

    End Class



    ''' <summary>
    ''' A Shapefile PolyLine  Shape
    ''' </summary>
    Public Class ShapePolyLine
        Inherits AbsShapePoly

        ''' <summary>
        ''' A Shapefile PolyLine Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="dataRecord">IDataRecord associated with the metadata</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PolyLine, recordNumber, metadata, metatype, shapeData)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile Polygon Shape
    ''' </summary>
    Public Class ShapePolygon
        Inherits AbsShapePoly

        ''' <summary>
        ''' A Shapefile Polygon Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="dataRecord">IDataRecord associated with the metadata</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.Polygon, recordNumber, metadata, metatype, shapeData)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile MultiPoint Shape
    ''' </summary>
    Public Class ShapeMultiPoint
        Inherits BoundedShape

        ''' <summary>
        ''' List of parts
        ''' </summary>
        Private _points As PointD()

        ''' <summary>
        ''' Gets the array of points
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Points As PointD()
            Get
                Return _points
            End Get
        End Property

        ''' <summary>
        ''' A Shapefile MultiPoint Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="dataRecord">IDataRecord associated with the metadata</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.MultiPoint, recordNumber, metadata, metatype, shapeData)

            ' Position     Field       Value       Type        Number      Order
            ' Byte 0       Shape Type  8           Integer     1           Little
            ' Byte 4       Box         Box         Double      4           Little
            ' Byte 36      NumPoints   Num Points  Integer     1           Little
            ' Byte 40      Points      Points      Point       NumPoints   Little

            If shapeData Is Nothing Then
                Throw New ArgumentNullException("shapeData")
            End If

            ' validation step 1 - must have at least 4 + (4 * 8) + 4 bytes = 40
            If shapeData.Length < 40 Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            Dim numPoints As Integer = EndianBitConverter.ToInt32(shapeData, 36, ProvidedOrder.Little)

            ' validation step 2 - we're expecting 16 * numPoints + 40 bytes total
            If shapeData.Length <> 40 + (16 * numPoints) Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            ' now extract the points
            ReDim _points(numPoints - 1)
            For pointNum As Integer = 0 To numPoints - 1

                _points(pointNum) = ReadPointD(shapeData, 40 + (16 * pointNum))
            Next
        End Sub
    End Class



    Public MustInherit Class AbsShapePolyZ
        Inherits AbsShapePoly

        Private _Z As MinMaxArray
        Private _M As MinMaxArray

        Public ReadOnly Property Zmin As Double
            Get
                Return _Z._min
            End Get
        End Property

        Public ReadOnly Property Zmax As Double
            Get
                Return _Z._max
            End Get
        End Property

        Public ReadOnly Property Z As Double()
            Get
                Return _Z._array
            End Get
        End Property

        Public ReadOnly Property Mmin As Double
            Get
                Return _M._min
            End Get
        End Property

        Public ReadOnly Property Mmax As Double
            Get
                Return _M._max
            End Get
        End Property

        Public ReadOnly Property M As Double()
            Get
                Return _M._array
            End Get
        End Property

        Protected Friend Sub New(type As ShapeType, recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(type, recordNumber, metadata, metatype, shapeData)

            ' Position     Field       Value       Type        Number      Order
            ' Byte 0       Shape Type  13          Integer     1           Little
            ' Byte 4       Box         Box         Double      4           Little
            ' Byte 36      NumParts    NumParts    Integer     1           Little
            ' Byte 40      NumPoints   NumPoints   Integer     1           Little
            ' Byte 44      Parts       Parts       Integer     NumParts    Little
            ' Byte X       Points      Points      Point       NumPoints   Little
            ' Byte Y       Zmin        Zmin        Double      1           Little
            ' Byte Y + 8*  Zmax        Zmax        Double      1           Little
            ' Byte Y + 16* Zarray      Zarray      Double      NumPoints   Little
            ' Byte Z*      Mmin        Mmin        Double      1           Little
            ' Byte Z + 8*  Mmax        Mmax        Double      1           Little
            ' Byte Z + 16* Marray      Marray      Double      NumPoints   Little
            '
            ' *optional

            Dim numParts As Integer = EndianBitConverter.ToInt32(shapeData, 36, ProvidedOrder.Little)
            Dim numPoints As Integer = EndianBitConverter.ToInt32(shapeData, 40, ProvidedOrder.Little)

            Dim ZOffset As Integer = 4 + 4 * 8 + 4 + 4 + 4 * numParts + 16 * numPoints
            Dim MOffset As Integer = ZOffset + 16 + (8 * numPoints)

            If shapeData.Length < MOffset Then
                Throw New InvalidOperationException("Invalid shape data")
            End If

            ' parse Z information
            _Z = ReadMinMaxArray(shapeData, ZOffset, numPoints)

            Dim expectedBytes As Integer = MOffset + 16 + (8 * numPoints)
            If shapeData.Length >= expectedBytes Then

                ' parse M information
                _M = ReadMinMaxArray(shapeData, MOffset, numPoints)
            End If
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile ShapePolyLineZ Shape
    ''' </summary>
    Public Class ShapePolyLineZ
        Inherits AbsShapePolyZ

        ''' <summary>
        ''' A Shapefile PolyLine Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="dataRecord">IDataRecord associated with the metadata</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PolyLineZ, recordNumber, metadata, metatype, shapeData)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile PolygonZ Shape
    ''' </summary>
    Public Class ShapePolygonZ
        Inherits AbsShapePoly

        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PolygonZ, recordNumber, metadata, metatype, shapeData)
        End Sub
    End Class



    Public MustInherit Class AbsShapePolyM
        Inherits AbsShapePoly

        Private _M As MinMaxArray

        Public ReadOnly Property Mmin As Double
            Get
                Return _M._min
            End Get
        End Property

        Public ReadOnly Property Mmax As Double
            Get
                Return _M._max
            End Get
        End Property

        Public ReadOnly Property M As Double()
            Get
                Return _M._array
            End Get
        End Property

        ''' <summary>
        ''' A Shapefile PolyLine Shape
        ''' </summary>
        ''' <param name="recordNumber">The record number in the Shapefile</param>
        ''' <param name="metadata">Metadata about the shape</param>
        ''' <param name="dataRecord">IDataRecord associated with the metadata</param>
        ''' <param name="shapeData">The shape record as a byte array</param>
        Protected Friend Sub New(type As ShapeType, recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(type, recordNumber, metadata, metatype, shapeData)

            ' Position     Field       Value       Type        Number      Order
            ' Byte 0       Shape Type  23          Integer     1           Little
            ' Byte 4       Box         Box         Double      4           Little
            ' Byte 36      NumParts    NumParts    Integer     1           Little
            ' Byte 40      NumPoints   NumPoints   Integer     1           Little
            ' Byte 44      Parts       Parts       Integer     NumParts    Little
            ' Byte X       Points      Points      Point       NumPoints   Little
            ' Byte Y*      Mmin        Mmin        Double      1           Little
            ' Byte Y + 8*  Mmax        Mmax        Double      1           Little
            ' Byte Y + 16* Marray      Marray      Double      NumPoints   Little
            '
            ' *optional

            Dim numParts As Integer = EndianBitConverter.ToInt32(shapeData, 36, ProvidedOrder.Little)
            Dim numPoints As Integer = EndianBitConverter.ToInt32(shapeData, 40, ProvidedOrder.Little)

            Dim MOffset As Integer = 4 + 4 * 8 + 4 + 4 + 4 * numParts + 16 * numPoints
            Dim expectedBytes As Integer = MOffset + 16 + (8 * numPoints)

            If shapeData.Length >= expectedBytes Then

                ' parse M information
                _M = ReadMinMaxArray(shapeData, MOffset, numPoints)
            End If
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile ShapePolyLineM Shape
    ''' </summary>
    Public Class ShapePolyLineM
        Inherits AbsShapePolyM

        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PolyLineM, recordNumber, metadata, metatype, shapeData)
        End Sub
    End Class



    ''' <summary>
    ''' A Shapefile ShapePolygonM Shape
    ''' </summary>
    Public Class ShapePolygonM
        Inherits AbsShapePolyM

        Protected Friend Sub New(recordNumber As Integer, metadata As Dictionary(Of String, String), metatype As Dictionary(Of String, Type), shapeData As Byte())
            MyBase.New(ShapeType.PolygonM, recordNumber, metadata, metatype, shapeData)
        End Sub
    End Class

End Namespace


