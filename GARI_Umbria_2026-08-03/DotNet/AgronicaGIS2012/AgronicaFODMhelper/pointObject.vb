
Imports AgronicaGIS2012.Commons

Public Class PointObject
    Public Value As Double

    Private _xyz As New xyz
    Public Property Xyz() As xyz
        Get
            Return _xyz
        End Get
        Set(value As xyz)
            _xyz = Value
        End Set
    End Property

    Public Property XCoord As Double
        Get
            Return _xyz.X
        End Get
        Set(value As Double)
            _xyz.X = value
        End Set
    End Property

    Public Property YCoord As Double
        Get
            Return _xyz.Y
        End Get
        Set(value As Double)
            _xyz.Y = value
        End Set
    End Property

End Class
