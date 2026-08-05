' Filename:    ShapeDisplay.cs
' Description: A helper class for importing ESRI shapefiles and
'              creating/displaying shapes on a WPF canvas.
' Comments:    Uses the classes from ShapeFile.cs.
' 2007-01-29 nschan Initial revision.

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Text
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports System.Windows.Threading

Imports AgronicaSHPWrapper.TestShapeFile

Namespace TestShapeFile
    ''' <summary>
    ''' The GeometryType enumeration defines geometry types that
    ''' can be used when creating WPF shapes. Choosing a stream
    ''' geometry type can improve rendering performance.
    ''' </summary>
    Public Enum GeometryType
        ''' <summary>
        ''' Use path geometry containing path figures.
        ''' </summary>
        UsePathGeometry

        ''' <summary>
        ''' Use StreamGeometry with StreamGeometryContext class
        ''' to specify drawing instructions.
        ''' </summary>
        UseStreamGeometry

        ''' <summary>
        ''' Same as UseStreamGeometry except that the figures
        ''' will be unstroked for greater performance (borders
        ''' won't be displayed for the shapes).
        ''' </summary>
        UseStreamGeometryNotStroked
    End Enum

    ''' <summary>
    ''' ShapeDisplay is a helper class for importing ESRI shapefiles
    ''' and creating/displaying shapes on a WPF canvas. During the
    ''' import step, a progress window is displayed. This is implemented
    ''' using the WPF single-threaded programming model, which allows
    ''' tasks to be executed by a Dispatcher instance while keeping the
    ''' UI responsive.
    ''' </summary>
    Public Class ShapeDisplay
#Region "Delegates"
        ''' <summary>
        ''' Defines the prototype for a method that reads a block
        ''' of shapefile records. Such a method is intended to be
        ''' executed by the Dispatcher.
        ''' </summary>
        ''' <param name="info">Shapefile read information.</param>
        Public Delegate Sub ReadNextPrototype(info As ShapeFileReadInfo)

        ''' <summary>
        ''' Defines the prototype for a method that creates and displays
        ''' a set of WPF shapes. Such a method is intended to be
        ''' executed by the Dispatcher.
        ''' </summary>
        ''' <param name="info">Shapefile read information.</param>
        Public Delegate Sub DisplayNextPrototype(info As ShapeFileReadInfo)
#End Region

#Region "Constants"
        Private Const readShapesBlockingFactor As Integer = 50
        Private Const displayShapesBlockingFactor As Integer = 10
        Private Const baseLonLatText As String = "Lon/Lat: "
#End Region

#Region "Private fields"
        ' UI components.
        Private owner As Window
        Private canvas As Canvas
        Private dispatcher As Dispatcher
        Private progressWindow As ProgressWindow

        ' Used during reading of a shapefile.
        Private m_isReadingShapeFile As Boolean
        Private cancelReadShapeFile As Boolean
        Private wpfShapeCount As Integer

        ' Used during creation of WPF shapes.
        Private shapeList As New List(Of Shape)()
        Private m_geometryType As GeometryType = GeometryType.UseStreamGeometryNotStroked

        ' Transformation from lon/lat to canvas coordinates.
        Private shapeTransform As TransformGroup

        ' Combined view transformation (zoom and pan).
        Private viewTransform As New TransformGroup()
        Private zoomTransform As New ScaleTransform()
        Private panTransform As New TranslateTransform()

        ' For coloring of WPF shapes.
        Private shapeBrushes As Brush()
        Private rand As New Random(379013)
        Private strokeBrush As Brush = New SolidColorBrush(Color.FromArgb(150, 150, 150, 150))

        ' For panning operations.
        Private m_isPanningEnabled As Boolean = True
        Private prevMouseLocation As Point
        Private isMouseDragging As Boolean
        Private panTolerance As Double = 1

        ' Displaying lon/lat coordinates on the canvas.
        Private m_isDisplayLonLatEnabled As Boolean = True
        Private lonLatLabel As New Label()
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor for the ShapeDisplay class.
        ''' </summary>
        ''' <param name="owner">Window that acts as an owner to child windows.</param>
        ''' <param name="canvas">The canvas on which to create WPF shapes.</param>
        Public Sub New(owner As Window, canvas As Canvas)
            ' Keep reference to a Window to act as the owner for a progress window.
            If owner Is Nothing Then
                Throw New ArgumentNullException("owner")
            End If
            Me.owner = owner

            ' Keep reference to the canvas and add mouse event handlers
            ' for implementing panning.
            If canvas Is Nothing Then
                Throw New ArgumentNullException("canvas")
            End If
            Me.canvas = canvas
            AddHandler Me.canvas.MouseEnter, New MouseEventHandler(AddressOf canvas_MouseEnter)
            AddHandler Me.canvas.MouseDown, New System.Windows.Input.MouseButtonEventHandler(AddressOf canvas_MouseDown)
            AddHandler Me.canvas.MouseMove, New System.Windows.Input.MouseEventHandler(AddressOf canvas_MouseMove)
            AddHandler Me.canvas.MouseUp, New System.Windows.Input.MouseButtonEventHandler(AddressOf canvas_MouseUp)
            AddHandler Me.canvas.MouseLeave, New MouseEventHandler(AddressOf canvas_MouseLeave)

            ' Keep reference to the dispatcher for task execution.
            Me.dispatcher = Me.canvas.Dispatcher

            ' Add the zoom and pan transforms to the view transform.
            Me.viewTransform.Children.Add(Me.zoomTransform)
            Me.viewTransform.Children.Add(Me.panTransform)

            ' Configure the lon/lat label.
            Me.lonLatLabel.Opacity = 0.7
        End Sub
#End Region

#Region "Properties"
        ''' <summary>
        ''' Indicates if a shapefile read operation is in progress.
        ''' </summary>
        Public ReadOnly Property IsReadingShapeFile() As Boolean
            Get
                Return Me.m_isReadingShapeFile
            End Get
        End Property

        ''' <summary>
        ''' Indicates if we can perform a zoom operation. This is true
        ''' the shape transform has been set (meaning at least one shapefile
        ''' has been loaded).
        ''' </summary>
        Public ReadOnly Property CanZoom() As Boolean
            Get
                Return (Me.shapeTransform IsNot Nothing)
            End Get
        End Property

        ''' <summary>
        ''' Indicates if panning is enabled or not. This
        ''' applies to both mouse and keyboard panning.
        ''' </summary>
        Public Property IsPanningEnabled() As Boolean
            Get
                Return Me.m_isPanningEnabled
            End Get
            Set(value As Boolean)
                Me.m_isPanningEnabled = value
            End Set
        End Property

        ''' <summary>
        ''' Indicates if display of lon/lat coordinates
        ''' is enabled or not.
        ''' </summary>
        Public Property IsDisplayLonLatEnabled() As Boolean
            Get
                Return Me.m_isDisplayLonLatEnabled
            End Get
            Set(value As Boolean)
                If Me.m_isDisplayLonLatEnabled <> value Then
                    Me.m_isDisplayLonLatEnabled = value
                    If Me.m_isDisplayLonLatEnabled Then
                        Me.DisplayLonLatDefault()
                    Else
                        Me.canvas.Children.Remove(Me.lonLatLabel)
                    End If
                End If
            End Set
        End Property

        ''' <summary>
        ''' Specifies the geometry type to use when creating WPF shapes.
        ''' </summary>
        Public Property GeometryType() As GeometryType
            Get
                Return Me.m_geometryType
            End Get
            Set(value As GeometryType)
                Me.m_geometryType = value
            End Set
        End Property
#End Region

#Region "Public methods"
        ''' <summary>
        ''' Read shapes and attributes from the given shapefile.
        ''' </summary>
        ''' <param name="fileName">Full pathname of a shapefile.</param>
        Public Sub ReadShapeFile(fileName As String)
            Me.m_isReadingShapeFile = True
            Me.cancelReadShapeFile = False

            ' Create an object to store shapefile info during the read.
            Dim info As New ShapeFileReadInfo()
            info.FileName = fileName
            info.ShapeFile = New ShapeFile()
            info.Stream = Nothing
            info.NumberOfBytesRead = 0
            info.RecordIndex = 0

            Try
                ' Read the File Header first.
                info.Stream = New FileStream(fileName, FileMode.Open, FileAccess.Read)
                info.ShapeFile.ReadShapeFileHeader(info.Stream)
                info.NumberOfBytesRead = ShapeFileHeader.Length

                ' Schedule the first read of shape file records using the dispatcher.
                Me.dispatcher.BeginInvoke(DispatcherPriority.Normal, New ReadNextPrototype(AddressOf Me.ReadNextShapeRecord), info)
            Catch ex As IOException
                Me.EndReadShapeFile(info)
                MessageBox.Show(ex.Message)
            End Try
        End Sub

        ''' <summary>
        ''' Request the current shapefile read operation to be cancelled.
        ''' </summary>
        Public Sub myCancelReadShapeFile()
            Me.cancelReadShapeFile = True
            Me.HideProgress()
        End Sub

        ''' <summary>
        ''' Reset the canvas.
        ''' </summary>
        Public Sub ResetCanvas()
            ' End reading of the shapefile.
            Me.EndReadShapeFile()

            ' Clear the canvas.
            Me.canvas.Children.Clear()
            Me.wpfShapeCount = 0

            ' Reset transformations.
            Me.panTransform.X = 0
            Me.panTransform.Y = 0
            Me.zoomTransform.ScaleX = 1
            Me.zoomTransform.ScaleY = 1
            Me.shapeTransform = Nothing
        End Sub

        ''' <summary>
        ''' Perform a zoom operation about the current center
        ''' of the canvas.
        ''' </summary>
        ''' <param name="zoomFactor">Zoom multiplication factor (1, 2, 4, etc).</param>
        Public Sub Zoom(zoomFactor As Double)
            ' Compute the coordinates of the center of the canvas
            ' in terms of pre-view transformation values. We do this
            ' by applying the inverse of the view transform.
            Dim canvasCenter As New Point(Me.canvas.ActualWidth / 2, Me.canvas.ActualHeight / 2)
            canvasCenter = Me.viewTransform.Inverse.Transform(canvasCenter)

            ' Temporarily reset the panning transformation.
            Me.panTransform.X = 0
            Me.panTransform.Y = 0

            ' Set the new zoom transformation scale factors.
            Me.zoomTransform.ScaleX = zoomFactor
            Me.zoomTransform.ScaleY = zoomFactor

            ' Apply the updated view transform to the canvas center.
            ' This gives us the updated location of the center point
            ' on the canvas. By differencing this with the desired
            ' center of the canvas, we can determine the ideal panning
            ' transformation parameters.
            Dim canvasLocation As Point = Me.viewTransform.Transform(canvasCenter)
            Me.panTransform.X = Me.canvas.ActualWidth / 2 - canvasLocation.X
            Me.panTransform.Y = Me.canvas.ActualHeight / 2 - canvasLocation.Y
        End Sub

        ''' <summary>
        ''' Perform a panning operation given X and Y factor values
        ''' which can be thought of as a fraction of the canvas actual
        ''' width or height.
        ''' </summary>
        ''' <param name="factorX">Fraction of canvas actual width to pan horizontally.</param>
        ''' <param name="factorY">Fraction of canvas actual height to pan vertically.</param>
        Public Sub Pan(factorX As Double, factorY As Double)
            If Not Me.m_isPanningEnabled Then
                Return
            End If

            Me.panTransform.X += (factorX * Me.canvas.ActualWidth)
            Me.panTransform.Y += (factorY * Me.canvas.ActualHeight)
        End Sub

        ''' <summary>
        ''' Save the owner object (the main window) to XAML.
        ''' This method may take a long time to run if there
        ''' are many objects on the canvas (shapefile > 1 Mb).
        ''' </summary>
        ''' <param name="stream">Output stream for writing the XAML.</param>
        Public Sub SaveToXaml(stream As Stream)
            System.Windows.Markup.XamlWriter.Save(Me.owner, stream)
        End Sub
#End Region

#Region "Transformations"
        ''' <summary>
        ''' Computes a transformation so that the shapefile geometry
        ''' will maximize the available space on the canvas and be
        ''' perfectly centered as well.
        ''' </summary>
        ''' <param name="info">Shapefile information.</param>
        ''' <returns>A transformation object.</returns>
        Private Function CreateShapeTransform(info As ShapeFileReadInfo) As TransformGroup
            ' Bounding box for the shapefile.
            Dim xmin As Double = info.ShapeFile.FileHeader.XMin
            Dim xmax As Double = info.ShapeFile.FileHeader.XMax
            Dim ymin As Double = info.ShapeFile.FileHeader.YMin
            Dim ymax As Double = info.ShapeFile.FileHeader.YMax

            ' Width and height of the bounding box.
            Dim width As Double = Math.Abs(xmax - xmin)
            Dim height As Double = Math.Abs(ymax - ymin)

            ' Aspect ratio of the bounding box.
            Dim aspectRatio As Double = width / height

            ' Aspect ratio of the canvas.
            Dim canvasRatio As Double = Me.canvas.ActualWidth / Me.canvas.ActualHeight

            ' Compute a scale factor so that the shapefile geometry
            ' will maximize the space used on the canvas while still
            ' maintaining its aspect ratio.
            Dim scaleFactor As Double = 1.0
            If aspectRatio < canvasRatio Then
                scaleFactor = Me.canvas.ActualHeight / height
            Else
                scaleFactor = Me.canvas.ActualWidth / width
            End If

            ' Compute the scale transformation. Note that we flip
            ' the Y-values because the lon/lat grid is like a cartesian
            ' coordinate system where Y-values increase upwards.
            Dim xformScale As New ScaleTransform(scaleFactor, -scaleFactor)

            ' Compute the translate transformation so that the shapefile
            ' geometry will be centered on the canvas.
            Dim xformTrans As New TranslateTransform()
            xformTrans.X = (Me.canvas.ActualWidth - (xmin + xmax) * scaleFactor) / 2
            xformTrans.Y = (Me.canvas.ActualHeight + (ymin + ymax) * scaleFactor) / 2

            ' Add the two transforms to a transform group.
            Dim xformGroup As New TransformGroup()
            xformGroup.Children.Add(xformScale)
            xformGroup.Children.Add(xformTrans)

            Return xformGroup
        End Function
#End Region

#Region "Brushes for gradient coloring"
        ''' <summary>
        ''' Create a set of linear gradient brushes which we can use
        ''' as a random pool for assignment to WPF shapes. A higher
        ''' gradient factor results in a stronger gradient effect.
        ''' </summary>
        ''' <param name="gradientFactor">Gradient factor from 0 to 1.</param>
        ''' <param name="gradientAngle">Direction of gradient in degrees.</param>
        Private Sub CreateShapeBrushes(gradientFactor As Double, gradientAngle As Double)
            ' Pick a set of base colors for the brushes.
            Dim colors__1 As Color() = New Color() {Colors.Crimson, Colors.ForestGreen, Colors.RoyalBlue, Colors.Navy, Colors.DarkSeaGreen, Colors.LightSlateGray, _
             Colors.DarkKhaki, Colors.Olive, Colors.Indigo, Colors.Violet}

            ' Create one brush per color.
            Me.shapeBrushes = New Brush(colors__1.Length - 1) {}
            For i As Integer = 0 To Me.shapeBrushes.Length - 1
                Me.shapeBrushes(i) = New LinearGradientBrush(ShapeDisplay.GetAdjustedColor(colors__1(i), gradientFactor), colors__1(i), gradientAngle)
            Next
        End Sub

        ''' <summary>
        ''' Given an input color, return an adjusted color using a
        ''' factor value which ranges from 0 to 1. The larger the factor,
        ''' the lighter the adjusted color. A factor of 0 means no adjustment
        ''' to the input color.
        ''' </summary>
        ''' <remarks>
        ''' Note that the alpha component of the input color is not adjusted.
        ''' </remarks>
        ''' <param name="inColor">Input color.</param>
        ''' <param name="factor">Color adjustment factor, from 0 to 1.</param>
        ''' <returns>An adjusted color value.</returns>
        Private Shared Function GetAdjustedColor(inColor As Color, factor As Double) As Color
            Dim red As Integer = inColor.R + CInt((255 - inColor.R) * factor)
            red = Math.Max(0, red)
            red = Math.Min(255, red)

            Dim green As Integer = inColor.G + CInt((255 - inColor.G) * factor)
            green = Math.Max(0, green)
            green = Math.Min(255, green)

            Dim blue As Integer = inColor.B + CInt((255 - inColor.B) * factor)
            blue = Math.Max(0, blue)
            blue = Math.Min(255, blue)

            Return Color.FromArgb(inColor.A, CByte(red), CByte(green), CByte(blue))
        End Function

        ''' <summary>
        ''' Get the next brush that can be used to fill a WPF shape.
        ''' </summary>
        ''' <returns>A randomly selected brush.</returns>
        Private Function GetRandomShapeBrush() As Brush
            Dim index As Integer = Me.rand.[Next]() Mod Me.shapeBrushes.Length
            Return Me.shapeBrushes(index)
        End Function
#End Region

#Region "Reading ESRI shapes"
        ''' <summary>
        ''' Read a block of shape file records and possibly schedule
        ''' the next read with the dispatcher.
        ''' </summary>
        ''' <param name="info">Shapefile read information.</param>
        Private Sub ReadNextShapeRecord(info As ShapeFileReadInfo)
            If Me.cancelReadShapeFile Then
                Return
            End If

            Try
                ' Read a block of shape records.
                For i As Integer = 0 To ShapeDisplay.readShapesBlockingFactor - 1
                    Dim record As ShapeFileRecord = info.ShapeFile.ReadShapeFileRecord(info.Stream)
                    info.NumberOfBytesRead += (4 + record.ContentLength) * 2
                Next
            Catch ex As FileFormatException
                Me.EndReadShapeFile(info)
                MessageBox.Show(ex.Message)

                Return
            Catch generatedExceptionName As IOException
                ' Display the end progress (100 percent).
                Me.ShowProgress("Reading shapefile...", 100)

                ' Read attributes from the associated dBASE file.
                Me.ReadDbaseAttributes(info)

                ' Display shapes on the canvas.
                If info.ShapeFile.Records.Count > 0 Then
                    Me.DisplayShapes(info)
                Else
                    Me.EndReadShapeFile(info)
                End If

                Return
            End Try

            ' Display the current progress.
            Dim progressValue As Double = info.NumberOfBytesRead * 100.0 / (info.ShapeFile.FileHeader.FileLength * 2)
            progressValue = Math.Min(100, progressValue)
            Me.ShowProgress("Reading shapefile...", progressValue)

            ' Schedule the next read at Background priority.
            Me.dispatcher.BeginInvoke(DispatcherPriority.Background, New ReadNextPrototype(AddressOf Me.ReadNextShapeRecord), info)
        End Sub

        ''' <summary>
        ''' Perform some cleanup at the end of reading a shapefile.
        ''' </summary>
        Private Sub EndReadShapeFile()
            Me.HideProgress()
            Me.m_isReadingShapeFile = False
            Me.cancelReadShapeFile = True
        End Sub

        ''' <summary>
        ''' Perform some cleanup at the end of reading a shapefile.
        ''' </summary>
        ''' <param name="info">Shapefile read information.</param>
        Private Sub EndReadShapeFile(info As ShapeFileReadInfo)
            If info IsNot Nothing AndAlso info.Stream IsNot Nothing Then
                info.Stream.Close()
                info.Stream.Dispose()
                info.Stream = Nothing
            End If

            Me.EndReadShapeFile()
        End Sub

        ''' <summary>
        ''' Read dBASE file attributes.
        ''' </summary>
        ''' <param name="info">Shapefile read information.</param>
        Private Sub ReadDbaseAttributes(info As ShapeFileReadInfo)
            ' Read attributes from the associated dBASE file.
            Try
                Dim dbaseFile As String = info.FileName.Replace(".shp", ".dbf")
                dbaseFile = dbaseFile.Replace(".SHP", ".DBF")
                info.ShapeFile.ReadAttributes(dbaseFile)
            Catch ex As OleDbException
                ' Note: An exception will occur if the filename of the dBASE
                ' file does not follow 8.3 naming conventions. In this case,
                ' you must use its short (MS-DOS) filename.
                MessageBox.Show(ex.Message)

                ' Activate the window.
                Me.owner.Activate()
            End Try
        End Sub
#End Region

#Region "Creating / displaying WPF shapes"
        ''' <summary>
        ''' Create a WPF shape given a shapefile record.
        ''' </summary>
        ''' <param name="shapeName">The name of the WPF shape.</param>
        ''' <param name="record">Shapefile record.</param>
        ''' <returns>The created WPF shape.</returns>
        Private Function CreateWPFShape(shapeName As String, record As ShapeFileRecord) As Shape
            ' Create a new geometry.
            Dim geometry As Geometry
            If Me.m_geometryType = GeometryType.UsePathGeometry Then
                geometry = Me.CreatePathGeometry(record)
            Else
                geometry = Me.CreateStreamGeometry(record)
            End If

            ' Transform the geometry based on current zoom and pan settings.
            geometry.Transform = Me.viewTransform

            ' Create a new WPF Path.
            Dim path As New System.Windows.Shapes.Path()

            ' Assign the geometry to the path and set its name.
            path.Data = geometry
            path.Name = shapeName

            ' Set path properties.
            path.StrokeThickness = 0.5
            If record.ShapeType = CInt(ShapeType.Polygon) Then
                path.Stroke = Me.strokeBrush
                path.Fill = Me.GetRandomShapeBrush()
            Else
                path.Stroke = Brushes.DimGray
            End If

            ' Return the created WPF shape.
            Return path
        End Function

        ''' <summary>
        ''' Create a PathGeometry given a shapefile record.
        ''' </summary>
        ''' <param name="record">Shapefile record.</param>
        ''' <returns>A PathGeometry instance.</returns>
        Private Function CreatePathGeometry(record As ShapeFileRecord) As Geometry
            ' Create a new geometry.
            Dim geometry As New PathGeometry()

            ' Add figures to the geometry.
            For i As Integer = 0 To record.NumberOfParts - 1
                ' Create a new path figure.
                Dim figure As New PathFigure()

                ' Determine the starting index and the end index
                ' into the points array that defines the figure.
                Dim start As Integer = record.Parts(i)
                Dim [end] As Integer
                If record.NumberOfParts > 1 AndAlso i <> (record.NumberOfParts - 1) Then
                    [end] = record.Parts(i + 1)
                Else
                    [end] = record.NumberOfPoints
                End If

                ' Add line segments to the figure.
                For j As Integer = start To [end] - 1
                    Dim pt As System.Windows.Point = record.Points(j)

                    ' Transform from lon/lat to canvas coordinates.
                    pt = Me.shapeTransform.Transform(pt)

                    If j = start Then
                        figure.StartPoint = pt
                    Else
                        figure.Segments.Add(New LineSegment(pt, True))
                    End If
                Next

                ' Add the new figure to the geometry.
                geometry.Figures.Add(figure)
            Next

            ' Return the created path geometry.
            Return geometry
        End Function

        ''' <summary>
        ''' Create a StreamGeometry given a shapefile record.
        ''' </summary>
        ''' <param name="record">Shapefile record.</param>
        ''' <returns>A StreamGeometry instance.</returns>
        Private Function CreateStreamGeometry(record As ShapeFileRecord) As Geometry
            ' Create a new stream geometry.
            Dim geometry As New StreamGeometry()

            ' Obtain the stream geometry context for drawing each part.
            Using ctx As StreamGeometryContext = geometry.Open()
                ' Draw figures.
                For i As Integer = 0 To record.NumberOfParts - 1
                    ' Determine the starting index and the end index
                    ' into the points array that defines the figure.
                    Dim start As Integer = record.Parts(i)
                    Dim [end] As Integer
                    If record.NumberOfParts > 1 AndAlso i <> (record.NumberOfParts - 1) Then
                        [end] = record.Parts(i + 1)
                    Else
                        [end] = record.NumberOfPoints
                    End If

                    ' Draw the figure.
                    For j As Integer = start To [end] - 1
                        Dim pt As System.Windows.Point = record.Points(j)

                        ' Transform from lon/lat to canvas coordinates.
                        pt = Me.shapeTransform.Transform(pt)

                        ' Decide if the line segments are stroked or not. For the
                        ' PolyLine type it must be stroked.
                        Dim isStroked As Boolean = (record.ShapeType = CInt(ShapeType.PolyLine)) OrElse Not (Me.m_geometryType = GeometryType.UseStreamGeometryNotStroked)

                        ' Register the drawing instruction.
                        If j = start Then
                            ctx.BeginFigure(pt, True, False)
                        Else
                            ctx.LineTo(pt, isStroked, True)
                        End If
                    Next
                Next
            End Using

            ' Return the created stream geometry.
            Return geometry
        End Function

        ''' <summary>
        ''' Create a WPF shape to represent a shapefile point or
        ''' multipoint record.
        ''' </summary>
        ''' <param name="shapeName">The name of the WPF shape.</param>
        ''' <param name="record">Shapefile record.</param>
        ''' <returns>The created WPF shape.</returns>
        Private Function CreateWPFPoint(shapeName As String, record As ShapeFileRecord) As Shape
            ' Create a new geometry.
            Dim geometry As New GeometryGroup()

            ' Add ellipse geometries to the group.
            For Each pt As Point In record.Points
                ' Create a new ellipse geometry.
                Dim ellipseGeo As New EllipseGeometry()

                ' Transform center point of the ellipse from lon/lat to
                ' canvas coordinates.
                ellipseGeo.Center = Me.shapeTransform.Transform(pt)

                ' Set the size of the ellipse.
                ellipseGeo.RadiusX = 0.1
                ellipseGeo.RadiusY = 0.1

                ' Add the ellipse to the geometry group.
                geometry.Children.Add(ellipseGeo)
            Next

            ' Transform the geometry based on current zoom and pan settings.
            geometry.Transform = Me.viewTransform

            ' Add the geometry to a new Path and set path properties.
            Dim path As New System.Windows.Shapes.Path()
            path.Data = geometry
            path.Name = shapeName
            path.Fill = Brushes.Crimson
            path.StrokeThickness = 1
            path.Stroke = Brushes.DimGray

            ' Return the created WPF shape.
            Return path
        End Function

        ''' <summary>
        ''' Begin creating and displaying WPF shapes on the canvas.
        ''' </summary>
        Private Sub DisplayShapes(info As ShapeFileReadInfo)
            ' Create shape brushes.
            If Me.shapeBrushes Is Nothing Then
                Me.CreateShapeBrushes(0.4, 45)
            End If

            ' Set up the transformation for WPF shapes.            
            If Me.shapeTransform Is Nothing Then
                Me.shapeTransform = Me.CreateShapeTransform(info)
            End If

            ' Schedule display of the first block of shapefile records
            ' using the dispatcher.
            Me.dispatcher.BeginInvoke(DispatcherPriority.Normal, New DisplayNextPrototype(AddressOf Me.DisplayNextShapeRecord), info)
        End Sub

        ''' <summary>
        ''' Display a block of shape records as WPF shapes, and schedule
        ''' the next display with the dispatcher if needed.
        ''' </summary>
        ''' <param name="info">Shapefile read information.</param>
        Private Sub DisplayNextShapeRecord(info As ShapeFileReadInfo)
            If Me.cancelReadShapeFile Then
                Return
            End If

            ' Create a block of WPF shapes and add them to the shape list. 
            Me.shapeList.Clear()
            Dim index As Integer = info.RecordIndex
            While index < (info.RecordIndex + ShapeDisplay.displayShapesBlockingFactor)
                If index >= info.ShapeFile.Records.Count Then
                    Exit While
                End If

                Dim record As ShapeFileRecord = info.ShapeFile.Records(index)

                ' Set the name of the WPF shape.
                Me.wpfShapeCount += 1
                Dim shapeName As String = [String].Format(System.Globalization.CultureInfo.InvariantCulture, "Shape{0}", Me.wpfShapeCount)

                ' Create the WPF shape.
                Dim shape As Shape
                If record.NumberOfParts = 0 Then
                    shape = Me.CreateWPFPoint(shapeName, record)
                Else
                    shape = Me.CreateWPFShape(shapeName, record)
                End If

                ' Set a tooltip for the shape that displays up to 5 attribute values.
                shape.ToolTip = shape.Name
                If record.Attributes IsNot Nothing Then
                    Dim attr As String = [String].Empty
                    'for (int i = 0; i < Math.Min(5, record.Attributes.ItemArray.GetLength(0)); i++)
                    For i As Integer = 0 To record.Attributes.ItemArray.GetLength(0) - 1
                        attr += (", " & record.Attributes(i).ToString())
                    Next
                    shape.ToolTip += attr
                End If

                ' Add the shape to the shape list.                
                Me.shapeList.Add(shape)

                ' If the record just processed is very large, then don't process
                ' any further records.
                If record.Points.Count > 5000 Then
                    index += 1
                    Exit While
                End If
                index += 1
            End While

            ' Set the record index to read next (as part of the
            ' next dispatched task).
            info.RecordIndex = index

            ' Add the newly created WPF shapes to the canvas.
            For Each shape As Shape In Me.shapeList
                Me.canvas.Children.Add(shape)
            Next
            Me.shapeList.Clear()

            ' Display the current progress.
            Dim progressValue As Double = (index * 100.0) / info.ShapeFile.Records.Count
            progressValue = Math.Min(100, progressValue)
            Me.ShowProgress("Creating WPF shapes...", progressValue)

            ' See if we need to dispatch another display operation.
            If index < info.ShapeFile.Records.Count Then
                ' Schedule the next display at Background priority.
                Me.dispatcher.BeginInvoke(DispatcherPriority.Background, New DisplayNextPrototype(AddressOf Me.DisplayNextShapeRecord), info)
            Else
                ' End the progress.
                Me.ShowProgress("Creating WPF shapes...", 100)
                Me.EndReadShapeFile(info)
            End If
        End Sub
#End Region

#Region "Displaying progress"
        ''' <summary>
        ''' Show the progress window with the given progress value.
        ''' </summary>
        ''' <param name="progressText">Progress text to display.</param>
        ''' <param name="progressValue">Progress value from 0 to 100 percent.</param>
        Private Sub ShowProgress(progressText As String, progressValue As Double)
            If Me.progressWindow Is Nothing Then
                ' Create a new progress window.
                Me.progressWindow = New ProgressWindow()
                Me.progressWindow.Owner = Me.owner

                AddHandler Me.progressWindow.Cancel, New CancelEventHandler(AddressOf Me.progressWindow_Cancel)
                AddHandler Me.progressWindow.Closed, New System.EventHandler(AddressOf Me.progressWindow_Closed)
                Me.progressWindow.Title = "Import Shapefile"
            End If

            ' Show the progress window with new progress text and value.
            Me.progressWindow.ProgressText = progressText
            Me.progressWindow.ProgressValue = progressValue
            Me.progressWindow.Show()
        End Sub

        ''' <summary>
        ''' Hide the progress window.
        ''' </summary>
        Private Sub HideProgress()
            If Me.progressWindow IsNot Nothing Then
                Me.progressWindow.Close()
            End If
        End Sub

        ''' <summary>
        ''' Handle the Cancel button being pressed in the progress window.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub progressWindow_Cancel(sender As Object, e As CancelEventArgs)
            Me.EndReadShapeFile()
        End Sub

        ''' <summary>
        ''' Handle closing of the progress window.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub progressWindow_Closed(sender As Object, e As EventArgs)
            Me.progressWindow = Nothing
        End Sub
#End Region

#Region "Lon/lat coordinates"
        ''' <summary>
        ''' Given a canvas position, convert it to a longitude
        ''' and latitude coordinate.
        ''' </summary>
        ''' <param name="canvasPosition">A canvas position or location.</param>
        ''' <returns>The corresponding point in lon/lat coordinates.</returns>
        Private Function GetLonLatCoordinates(canvasPosition As Point) As Point
            ' Apply the inverse of the view transformation.
            Dim p1 As Point = Me.viewTransform.Inverse.Transform(canvasPosition)

            ' Apply the inverse of the shape transformation.
            If Me.shapeTransform IsNot Nothing Then
                Dim p2 As Point = Me.shapeTransform.Inverse.Transform(p1)
                Return p2
            End If

            Return p1
        End Function

        ''' <summary>
        ''' Given a lon/lat coordinate, determine the corresponding
        ''' display text.
        ''' </summary>
        ''' <param name="lonLat">A lon/lat coordinate.</param>
        ''' <returns>Formatted lon/lat display text.</returns>
        Private Shared Function GetLonLatDisplayText(lonLat As Point) As String
            Dim text As String = ShapeDisplay.baseLonLatText
            If lonLat.X < -180 OrElse lonLat.X > 180 OrElse lonLat.Y < -90 OrElse lonLat.Y > 90 Then
                text += "n/a"
            Else
                text += [String].Format(System.Globalization.CultureInfo.InvariantCulture, "{0:0.00}, {1:0.00}", lonLat.X, lonLat.Y)
            End If

            Return text
        End Function

        ''' <summary>
        ''' Given a canvas position, display the corresponding
        ''' lon/lat coordinates on the canvas.
        ''' </summary>
        ''' <param name="canvasPosition">A canvas position or location.</param>
        Private Sub DisplayLonLatCoord(canvasPosition As Point)
            ' Remove the lon/lat label from the canvas first.
            Me.canvas.Children.Remove(Me.lonLatLabel)

            If Me.shapeTransform IsNot Nothing Then
                ' Convert from canvas position to lon/lat coordinates.
                Dim lonLat As Point = Me.GetLonLatCoordinates(canvasPosition)

                ' Convert lon/lat value to a display string.
                Me.lonLatLabel.Content = ShapeDisplay.GetLonLatDisplayText(lonLat)

                ' Add the label back to the canvas. This ensures that
                ' it will appear on top of all other canvas elements.
                Me.canvas.Children.Add(Me.lonLatLabel)
            End If
        End Sub

        ''' <summary>
        ''' Display some default text in the lon/lat label.
        ''' </summary>
        Private Sub DisplayLonLatDefault()
            ' Remove the lon/lat label from the canvas first.
            Me.canvas.Children.Remove(Me.lonLatLabel)

            ' Set the default text to display.
            Me.lonLatLabel.Content = ShapeDisplay.baseLonLatText & "n/a"

            ' Add the label back to the canvas. This ensures that
            ' it will appear on top of all other canvas elements.
            Me.canvas.Children.Add(Me.lonLatLabel)
        End Sub
#End Region

#Region "Mouse handlers"
        ''' <summary>
        ''' Handle the MouseEnter event for the canvas.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub canvas_MouseEnter(sender As Object, e As MouseEventArgs)
            Me.isMouseDragging = False
        End Sub

        ''' <summary>
        ''' Handle the MouseDown event for the canvas.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub canvas_MouseDown(sender As Object, e As MouseButtonEventArgs)
            If Me.shapeTransform Is Nothing Then
                Return
            End If

            ' Display lon/lat coordinates if needed.
            Dim canvasPosition As Point = e.GetPosition(Me.canvas)
            If Me.m_isDisplayLonLatEnabled Then
                Me.DisplayLonLatCoord(canvasPosition)
            End If

            ' Update previous mouse location for start of dragging.
            Me.prevMouseLocation = canvasPosition
            Me.isMouseDragging = True
        End Sub

        ''' <summary>
        ''' Handle the MouseMove event for the canvas.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub canvas_MouseMove(sender As Object, e As MouseEventArgs)
            ' Implement panning with the mouse. 
            If Me.isMouseDragging Then
                If Not Me.m_isPanningEnabled Then
                    Return
                End If

                ' Obtain the current mouse location and compute the
                ' difference with the previous mouse location.
                Dim currMouseLocation As Point = e.GetPosition(Me.canvas)
                Dim xOffset As Double = currMouseLocation.X - Me.prevMouseLocation.X
                Dim yOffset As Double = currMouseLocation.Y - Me.prevMouseLocation.Y

                ' To avoid panning on every single mouse move, we check
                ' if the movement is larger than the pan tolerance.
                If Math.Abs(xOffset) > Me.panTolerance OrElse Math.Abs(yOffset) > Me.panTolerance Then
                    Me.panTransform.X += xOffset
                    Me.panTransform.Y += yOffset

                    Me.prevMouseLocation = currMouseLocation
                End If
            Else
                ' Display lon/lat coordinates if needed.
                If Me.m_isDisplayLonLatEnabled Then
                    Me.DisplayLonLatCoord(e.GetPosition(Me.canvas))
                End If
            End If
        End Sub

        ''' <summary>
        ''' Handle the MouseUp event for the canvas.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub canvas_MouseUp(sender As Object, e As MouseButtonEventArgs)
            Me.isMouseDragging = False
        End Sub

        ''' <summary>
        ''' Handle the MouseLeave event for the canvas.
        ''' </summary>
        ''' <param name="sender">Event sender.</param>
        ''' <param name="e">Event arguments.</param>
        Private Sub canvas_MouseLeave(sender As Object, e As MouseEventArgs)
            Me.isMouseDragging = False
        End Sub
#End Region
    End Class
End Namespace

' END

