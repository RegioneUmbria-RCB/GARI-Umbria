


Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore


Public Class MisuraXIndiciMaturita_W


    Public Sub Scrivi(dbContext As GiasDbContext, MisuraXIndiciMaturita As APP_MisuraXIndiciMaturita, commit As Boolean)

        dbContext.APP_MisuraXIndiciMaturita.Add(MisuraXIndiciMaturita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, MisuraXIndiciMaturita As APP_MisuraXIndiciMaturita)

        If MisuraXIndiciMaturita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_MisuraXIndiciMaturita]")
        Else
            dbContext.APP_MisuraXIndiciMaturita.Remove(MisuraXIndiciMaturita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class