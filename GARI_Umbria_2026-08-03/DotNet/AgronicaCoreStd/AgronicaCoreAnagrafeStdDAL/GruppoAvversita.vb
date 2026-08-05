Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class GruppoAvversita_R
    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_GruppoAvversita)

        Return dbContext.APP_GruppoAvversita.OrderBy(Function(f) f.Av_Gru_Des).ToList()

    End Function

    Public Function Leggi(dbContext As GiasDbContext, av_Gru As Integer) As APP_GruppoAvversita

        Dim rval As APP_GruppoAvversita = (
            From g In dbContext.APP_GruppoAvversita
            Where g.Av_Gru = av_Gru
            ).FirstOrDefault


        Return rval

    End Function

End Class

Public Class GruppoAvversita_W
    Public Sub Scrivi(dbContext As GiasDbContext, gruppo_avversita As APP_GruppoAvversita, commit As Boolean)

        dbContext.APP_GruppoAvversita.Add(gruppo_avversita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, gruppo_avversita As APP_GruppoAvversita)

        If gruppo_avversita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_GruppoAvversita]")
        Else
            dbContext.APP_GruppoAvversita.Remove(gruppo_avversita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
