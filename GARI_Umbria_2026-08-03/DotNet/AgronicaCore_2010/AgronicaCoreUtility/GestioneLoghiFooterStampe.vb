Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider
Imports Newtonsoft.Json.Linq

Public Class GestioneLoghiFooterStampe

    Public Function creaLogoBianco() As Byte()
        Dim bmpImg As New Drawing.Bitmap(1, 1)
        bmpImg.SetPixel(0, 0, Drawing.Color.White)

        Dim cx As New Drawing.ImageConverter
        Dim logoBianco() As Byte
        logoBianco = cx.ConvertTo(bmpImg, GetType(Byte()))
        Return logoBianco
    End Function

End Class

