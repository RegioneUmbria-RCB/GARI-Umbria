Public Class Lavorazione_Malavasi
    Public lottoCalibrato As String
    Public pesoSecondaScelta As Decimal = 0
    Public numeroBins As Integer = 0
    Public dataInizioCalibratura As DateTime
    Public dataFineCalibratura As DateTime
    Public esitoCalibratura As List(Of Calibratura_Malavasi)
End Class

Public Class Calibratura_Malavasi
    Public calibro As String
    Public pesoProdotti As Decimal
End Class