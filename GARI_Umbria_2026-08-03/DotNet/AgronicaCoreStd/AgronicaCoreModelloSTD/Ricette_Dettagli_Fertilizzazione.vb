Imports AgronicaCoreModelloSTD

Public Class Ricette_Dettagli_Fertilizzazione
    Inherits Ricette_Dettagli_Prodotti
    Implements iRicette_Dettagli


    Public Property N As Decimal

    Public Property P As Decimal

    Public Property K As Decimal

    Public Property CU As Decimal

    Public ReadOnly Property Prodotto_Des_Altro() As String
        Get
            Return "N: " & Decimal.Round(N, 2) & "; P: " & Decimal.Round(P, 2) & "; K: " & Decimal.Round(K, 2) & "; CU: " & Decimal.Round(CU, 2)
        End Get
    End Property

End Class
