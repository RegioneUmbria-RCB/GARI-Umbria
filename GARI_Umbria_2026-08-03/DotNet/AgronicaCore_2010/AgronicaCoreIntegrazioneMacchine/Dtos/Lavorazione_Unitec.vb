Public Class Lavorazione_Unitec
    Public lottoCalibrato As String
    Public dataOraInizio As DateTime = Nothing
    Public dataOraFine As DateTime = Nothing
    Public esitoCalibratura As List(Of Calibratura_Unitec)
End Class

Public Class Calibratura_Unitec
    Public classe As String
    Public numeroProdotti As Integer
    Public pesoProdotti As Decimal
End Class