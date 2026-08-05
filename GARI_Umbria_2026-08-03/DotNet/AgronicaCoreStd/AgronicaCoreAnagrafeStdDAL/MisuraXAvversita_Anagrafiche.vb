

Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore


Public Class MisuraXAvversita_Anagrafiche_W


    Public Sub Scrivi(dbContext As GiasDbContext, MisuraXAvversita_Anagrafiche As APP_MisuraXAvversita_Anagrafiche, commit As Boolean)

        dbContext.APP_MisuraXAvversita_Anagrafiche.Add(MisuraXAvversita_Anagrafiche)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, MisuraXAvversita_Anagrafiche As APP_MisuraXAvversita_Anagrafiche)

        If MisuraXAvversita_Anagrafiche Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_MisuraXAvversita_Anagrafiche]")
        Else
            dbContext.APP_MisuraXAvversita_Anagrafiche.Remove(MisuraXAvversita_Anagrafiche)
            dbContext.SaveChanges()
        End If

    End Sub

End Class