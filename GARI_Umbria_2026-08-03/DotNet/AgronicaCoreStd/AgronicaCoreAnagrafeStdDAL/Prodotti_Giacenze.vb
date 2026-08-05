Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Prodotti_Giacenze_R
    Public Function Leggi(dbContext As GiasDbContext, Piva As String) As List(Of APP_Prodotti_Giacenze)

        Return dbContext.APP_Prodotti_Giacenze.Where(
            Function(item) (Piva Is Nothing OrElse item.Piva = Piva)
        ).OrderBy(Function(f) f.Prodotto_Cod).ToList()

    End Function

    Public Function Leggi(
        dbContext As GiasDbContext,
        Piva As String,
        Sa_Cod As Integer,
        Tipo_Destinazione As Integer,
        Fabbricato_Cod As Integer,
        Elem_Cod As Integer,
        Prodotto_Cod As Integer,
        Lotto As String,
        Cal_Cod As Integer,
        Cod_Progetto As Integer,
        Udm_Cod As Integer
     ) As APP_Prodotti_Giacenze

        Dim rval As APP_Prodotti_Giacenze = (
            From p In dbContext.APP_Prodotti_Giacenze
            Where p.Piva = Piva AndAlso
                  p.Sa_Cod = Sa_Cod AndAlso
                  p.Tipo_Destinazione = Tipo_Destinazione AndAlso
                  p.Fabbricato_Cod = Fabbricato_Cod AndAlso
                  p.Elem_Cod = Elem_Cod AndAlso
                  p.Prodotto_Cod = Prodotto_Cod AndAlso
                  p.Cal_Cod = Cal_Cod AndAlso
                  p.Cod_Progetto = Cod_Progetto AndAlso
                  p.Udm_Cod = Udm_Cod
            ).FirstOrDefault

        Return rval

    End Function
End Class

Public Class Prodotti_Giacenze_W
    Public Sub Scrivi(dbContext As GiasDbContext, prodotti_giacenze As APP_Prodotti_Giacenze, commit As Boolean)

        dbContext.APP_Prodotti_Giacenze.Add(prodotti_giacenze)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, prodotti_giacenze As APP_Prodotti_Giacenze, piva As String)

        If prodotti_giacenze Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Prodotti_Giacenze] WHERE Piva={0}", piva)
        Else
            dbContext.APP_Prodotti_Giacenze.Remove(prodotti_giacenze)
            dbContext.SaveChanges()
        End If

    End Sub
End Class