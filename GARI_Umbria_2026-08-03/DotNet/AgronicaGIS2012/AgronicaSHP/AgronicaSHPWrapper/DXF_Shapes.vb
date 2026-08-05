'Imports System.Drawing
'Imports System.Drawing.Drawing2D

Namespace DXFImporter
#Region "Shape class - abstract"
    Public MustInherit Class Shape

        Public shapeIdentifier As Integer
        Public rotation As Integer
        Public highlighted As Boolean


    End Class
#End Region

#Region "Line class"
    Public Class Line
        Inherits DXFImporter.Shape
        Protected startPoint As Point
        Protected endPoint As Point

        Public Sub New(start As Point, [end] As Point, w As Integer)
            startPoint = start
            endPoint = [end]
            shapeIdentifier = 1

            rotation = 0
        End Sub


        Public Sub New()
        End Sub

        Public Overridable ReadOnly Property GetStartPoint() As Point
            Get
                Return startPoint
            End Get
        End Property


        Public Overridable ReadOnly Property GetEndPoint() As Point
            Get
                Return endPoint
            End Get
        End Property

    End Class
#End Region

#Region "Rectangle class"
    Public Class rectangle
        Inherits DXFImporter.Line
        Public Sub New(start As Point, [end] As Point, w As Integer, angle As Integer)
            startPoint = start
            endPoint = [end]
            shapeIdentifier = 2

            rotation = angle
        End Sub


        Private Function checkPosition(P1 As Point, P2 As Point, current As Point) As Double
            Dim m As Double = CDbl(P2.Y - P1.Y) / (P2.X - P1.X)
            Return ((current.Y - P1.Y) - (m * (current.X - P1.X)))
        End Function

    End Class
#End Region

#Region "Circle Class"
    Public Class circle
        Inherits DXFImporter.Shape
        Private centerPoint As Point
        Private radius As Double

        Private _CodiceAssociato As String
        Public Property CodiceAssociato() As String
            Get
                Return _CodiceAssociato
            End Get
            Set(value As String)
                _CodiceAssociato = Value
            End Set
        End Property

        Public Sub New(center As Point, r As Double, w As Integer)
            centerPoint = center
            radius = r
            shapeIdentifier = 3
            rotation = 0
        End Sub


        Public Property AccessCenterPoint() As Point
            Get
                Return centerPoint
            End Get
            Set(value As Point)
                centerPoint = value
            End Set
        End Property

        Public ReadOnly Property AccessRadius() As Double
            Get
                Return radius
            End Get
        End Property

    End Class
#End Region

#Region "Freehand Class - Not Completed Yet"
    Public Class FreehandTool
        Inherits DXFImporter.Shape
        Private linePoint As ArrayList

        Public Sub New(points As ArrayList, w As Integer)

            shapeIdentifier = 4
            rotation = 0

            linePoint = points
        End Sub


    End Class
#End Region

#Region "Polyline Class"

    Public Class polyline
        Inherits DXFImporter.Shape
        Private _listOfLines As ArrayList
        Public Property ListOfLines() As ArrayList
            Get
                Return _listOfLines
            End Get
            Set(value As ArrayList)
                _listOfLines = Value
            End Set
        End Property

        Private _Layer As String
        Public Property Layer() As String
            Get
                Return _Layer
            End Get
            Set(value As String)
                _Layer = Value
            End Set
        End Property

        Private _lType As String
        Public Property LType() As String
            Get
                Return _lType
            End Get
            Set(value As String)
                _lType = Value
            End Set
        End Property


        Private _CodiceAssociato As String
        Public Property CodiceAssociato() As String
            Get
                Return _CodiceAssociato
            End Get
            Set(value As String)
                _CodiceAssociato = Value
            End Set
        End Property


        Private _Colore As String
        Public Property Colore() As String
            Get
                Return _Colore
            End Get
            Set(value As String)
                _Colore = Value
            End Set
        End Property

        Public Sub New(w As Integer)
            _listOfLines = New ArrayList()


        End Sub

        Public Sub AppendLine(theLine As Line)
            _listOfLines.Add(theLine)
        End Sub

    End Class

#End Region

#Region "Arc Class"

    Public Class arc
        Inherits DXFImporter.Shape
        Private centerPoint As Point
        Private radius As Double

        Private startAngle As Double
        Private sweepAngle As Double

        Public Sub New(center As Point, r As Double, startangle__1 As Double, sweepangle__2 As Double, w As Integer)
            centerPoint = center
            radius = r
            startAngle = startangle__1
            sweepAngle = sweepangle__2
            shapeIdentifier = 3
            rotation = 0
        End Sub

        Public ReadOnly Property AccessStartAngle() As Double
            Get
                Return startAngle
            End Get
        End Property


        Public ReadOnly Property AccessSweepAngle() As Double
            Get
                Return sweepAngle
            End Get
        End Property


        Public Property AccessCenterPoint() As Point
            Get
                Return centerPoint
            End Get
            Set(value As Point)
                centerPoint = value
            End Set
        End Property

        Public ReadOnly Property AccessRadius() As Double
            Get
                Return radius
            End Get
        End Property

    End Class

#End Region
End Namespace
