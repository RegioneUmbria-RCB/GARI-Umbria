Public Class RasterCrop_In
    Public Property path As String
    Public Property wkt As String
    Public Property srid As Integer
End Class

Public Class RasterCrop_Out
    Public Property cropId As Guid
    Public Property saveDate As DateTime
End Class
