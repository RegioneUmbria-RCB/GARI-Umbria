Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Prodotti_Giacenze
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiProdottiGiacenze(Piva As String) As List(Of APP_Prodotti_Giacenze)

        Dim xLettura = New Prodotti_Giacenze_R()
        Return xLettura.Leggi(dbContext, Piva)

    End Function

    Public Sub ScriviProdottiGiacenze(listProdottiGiacenze As List(Of APP_Prodotti_Giacenze), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numProdotti As Integer = listProdottiGiacenze.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Prodotti_Giacenze_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing, piva)
        End If

        For Each prodotto In listProdottiGiacenze
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numProdotti
            xScrittura.Scrivi(dbContext, prodotto, commit)
        Next

    End Sub

    Public Sub CancellaProdottiGiacenze(piva As String)

        Dim xScrittura = New Prodotti_Giacenze_W()
        xScrittura.Cancella(dbContext, Nothing, piva)

    End Sub

    Public Sub CancellaProdotto(prodotto_giacenza As APP_Prodotti_Giacenze)

        Dim xScrittura = New Prodotti_Giacenze_W()
        xScrittura.Cancella(dbContext, prodotto_giacenza, Nothing)

    End Sub
End Class
