Public Class Geometry
    Public Class GeometryDefinition
        Property type As String
        Property composition As GeometryShapeComposition

    End Class

    Public Class GeometryShapeComposition
        Property shapeform As List(Of GeometryShapeDefinition)
        Public Sub New()
            shapeform = New List(Of GeometryShapeDefinition)
        End Sub
    End Class

    Public Class GeometryShapeDefinition
        Property shape As GeometryShape
        Property hole As List(Of GeometryShape)
        Public Sub New()
            shape = New GeometryShape
            hole = New List(Of GeometryShape)
        End Sub
    End Class

    Public Class GeometryShape
        Inherits List(Of Point)

    End Class

    Public Class Point
            Property Latitude As Decimal
            Property Longitude As Decimal
        End Class
    End Class
