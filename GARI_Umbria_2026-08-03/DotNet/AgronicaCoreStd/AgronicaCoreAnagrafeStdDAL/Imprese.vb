Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Imprese_R
    Public Function Leggi(dbContext As GiasDbContext, Piva As String) As List(Of APP_Imprese)

        If String.IsNullOrEmpty(Piva) Then
            Return dbContext.APP_Imprese.OrderBy(Function(f) f.rag_soc).ToList()
        End If

        Dim rval As List(Of APP_Imprese) = (
            From azienda In dbContext.APP_Imprese
            Where azienda.piva = Piva
        ).OrderBy(Function(f) f.rag_soc).ToList()

        Return rval

    End Function

    Public Function Cerca(dbContext As GiasDbContext, Search As String) As List(Of APP_Imprese)

        Dim rval As List(Of APP_Imprese) = (
            From azienda In dbContext.APP_Imprese
            Where azienda.piva.Contains(Search) OrElse azienda.rag_soc.Contains(Search)
        ).OrderBy(Function(f) f.rag_soc).ToList()

        Return rval

    End Function

End Class

Public Class Imprese_W
    Public Sub Scrivi(dbContext As GiasDbContext, impresa As APP_Imprese, Optional commit As Boolean = True)

        dbContext.APP_Imprese.Add(impresa)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, impresa As APP_Imprese)

        If impresa Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Imprese]")
        Else
            dbContext.APP_Imprese.Remove(impresa)
            dbContext.SaveChanges()
        End If

    End Sub
End Class
