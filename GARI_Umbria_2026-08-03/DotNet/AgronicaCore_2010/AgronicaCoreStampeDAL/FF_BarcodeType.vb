Imports Newtonsoft.Json.Linq

Public Class FF_BarcodeType


    Public Code128 As List(Of FF_BarcodeType_Barcode)
    Public PDF417 As List(Of FF_BarcodeType_Barcode)
    Public QRCode As List(Of FF_BarcodeType_Barcode)
    Public CP_ParametriCrystal As Boolean
End Class

Public Class FF_BarcodeType_Barcode

    Public CampoDa As String
    Public CampoA As String
    Public Posizione As Integer
    Public AI As String
    Public Lunghezza As Integer
    Public PadChar As String
    Public Algoritmo As Integer

    Public SeparatoreCampo As String
    Public SeparatoreChiaveVaore As String

    Public ListaMappaDescrizioni As List(Of FF_DB_Rimappatura)

    Public Sub New()
        ListaMappaDescrizioni = New List(Of FF_DB_Rimappatura)
    End Sub

End Class


Public Class FF_DB_Rimappatura
    Public Property NomeCampoDt As String
    Public Property NomeCampoDescrizione As String
End Class