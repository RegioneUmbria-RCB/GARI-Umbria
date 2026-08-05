Public Class Ricette_Dettagli_Rilievi
    Inherits Ricette_Dettagli
    Implements iRicette_Dettagli

    Public Property FaseFenologica As Integer
    Public Property IndiceMaturita As Integer
    Public Property Avversita As Integer
    Public Property GruppoAvversita As Integer
    Public Property UnitaDiMisuraCod As Integer
    Public Property DataOraRilievo As DateTime
    Public Property QtaRilevata As Double
    Public Property Impianto As String
    Public Property Descrizione As String
    Public Property QtaRilevataString As String
End Class
