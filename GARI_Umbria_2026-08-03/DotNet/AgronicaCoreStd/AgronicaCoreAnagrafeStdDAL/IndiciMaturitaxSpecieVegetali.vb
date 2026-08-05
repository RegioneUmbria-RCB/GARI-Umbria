
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore


Public Class IndiciMaturitaxSpecieVegetali_W


    Public Sub Scrivi(dbContext As GiasDbContext, IndiciMaturita As APP_IndiciMaturitaxSpecieVegetali, commit As Boolean)

        dbContext.APP_IndiciMaturitaxSpecieVegetali.Add(IndiciMaturita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, IndiciMaturitaxSpecieVegetali As APP_IndiciMaturitaxSpecieVegetali)

        If IndiciMaturitaxSpecieVegetali Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_IndiciMaturitaxSpecieVegetali]")
        Else
            dbContext.APP_IndiciMaturitaxSpecieVegetali.Remove(IndiciMaturitaxSpecieVegetali)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
