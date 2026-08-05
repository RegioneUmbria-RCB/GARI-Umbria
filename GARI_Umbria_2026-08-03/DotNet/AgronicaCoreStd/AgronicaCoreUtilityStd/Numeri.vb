Public Class Numeri

    Public Shared Function Arrotondamento(d As Decimal, Optional ByVal cifre As Integer = 3) As Decimal

        Return Math.Round(d, cifre)

    End Function

    Public Shared Function Arrotondamento(d As Double, Optional ByVal cifre As Integer = 3) As Double

        Return Math.Round(d, cifre)

    End Function

    Public Shared Function ArrotondamentoStringaOut(d As Double?, Optional ByVal cifre As Integer = 3) As String

        Dim d1 As Double
        If d Is Nothing Then
            Return ""
        Else
            d1 = CDbl(d)
        End If

        Return Arrotondamento(d1, cifre).ToString()
    End Function


    Public Shared Function ArrotondamentoStringaOut(d As Decimal?, Optional ByVal cifre As Integer = 3) As String

        Dim d1 As Double
        If d Is Nothing Then
            Return ""
        Else
            d1 = CDbl(d)
        End If

        Return Arrotondamento(d1, cifre).ToString()
    End Function

End Class
