Namespace DXFImporter
    ''' <summary>
    ''' Summary description for DrawingObject.
    ''' </summary>
    Public Class DrawingObject
        Public shapeType As Integer
        Public indexNo As Integer

        Public Sub New(shapeID As Integer, ix As Integer)
            shapeType = shapeID

            indexNo = ix
        End Sub
    End Class
End Namespace
