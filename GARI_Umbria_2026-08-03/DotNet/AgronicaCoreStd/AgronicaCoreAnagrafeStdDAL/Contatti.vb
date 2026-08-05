Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Contatti_R

    Public Function Leggi(dbContext As GiasDbContext, Piva As String, Cod_RisUm As Integer) As List(Of APP_Contatti)

        Return dbContext.APP_Contatti.Where(
            Function(item) ((Piva Is Nothing OrElse item.Sa_Cod = -1 OrElse item.Piva = Piva) AndAlso
                            (Cod_RisUm = 0 OrElse item.Cod_RisUm = Cod_RisUm))
        ).OrderBy(Function(f) f.rag_soc).ThenBy(Function(f) f.Cognome).ThenBy(Function(f) f.Nome).ToList()


    End Function

    Public Function LeggiBadge(dbContext As GiasDbContext, NrBadge As String) As APP_Contatti

        Return dbContext.APP_Contatti.Where(Function(item) (item.NrBadge = NrBadge)).ToList().FirstOrDefault()

    End Function

    Public Function EstraiListaContatti(dbContext As GiasDbContext, FiltroPiva As String, ByVal FiltroSa_Cod As Integer) As List(Of APP_Contatti)


        Dim rval As List(Of APP_Contatti) = (
            From m In dbContext.APP_Contatti
            Where (m.Sa_Cod = -1 OrElse (
                m.Piva = FiltroPiva AndAlso m.Sa_Cod = 0
                ) OrElse (
            m.Piva = FiltroPiva AndAlso m.Sa_Cod = FiltroSa_Cod
        ))
        ).ToList

        Return rval


    End Function

End Class

Public Class Contatti_W
    Public Sub Scrivi(dbContext As GiasDbContext, contatto As APP_Contatti, commit As Boolean)

        dbContext.APP_Contatti.Add(contatto)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, contatto As APP_Contatti, piva As String)

        If contatto Is Nothing Then
            If String.IsNullOrEmpty(piva) Then
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Contatti]") ' WHERE Sa_Cod = -1
            Else
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Contatti] WHERE Piva={0} AND Sa_Cod <> -1", piva)
            End If
        Else
            dbContext.APP_Contatti.Remove(contatto)
            dbContext.SaveChanges()
        End If

    End Sub
End Class
