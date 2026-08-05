
Imports System.drawing
Imports System.IO

Public Class Barcode_PDF417

    Public Function CreaBarcode(ByVal sToCreate As String, ByVal pYHeight As Double, ByVal pAspectRatio As Double) As Byte()

        Dim creaPDF417 As New PDF417.PDF417(sToCreate, pYHeight, pAspectRatio)


        Dim ms As New MemoryStream
        creaPDF417.toBitmap().Save(ms, Imaging.ImageFormat.Png)
        Return ms.GetBuffer()

    End Function

    
End Class
