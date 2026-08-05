Public Class Ricette_Dettagli_Trattamento
    Inherits Ricette_Dettagli_Prodotti
    Implements iRicette_Dettagli

    Public Property AvversitaGruppo As AvversitaConGruppi

    Public ReadOnly Property Prodotto_Des_Altro() As String
        Get
            If AvversitaGruppo IsNot Nothing AndAlso AvversitaGruppo.Cod <> "0|0" Then
                Return AvversitaGruppo.DescrizioneAvversitaGruppo
            End If
            Return ""
        End Get
    End Property

End Class
