Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class AvversitaxSpecie_R
    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_AvversitaxSpecie)

        Return dbContext.APP_AvversitaxSpecie.ToList()

    End Function

End Class

Public Class AvversitaxSpecie_W

    Public Sub Scrivi(dbContext As GiasDbContext, avversita_specie As APP_AvversitaxSpecie, commit As Boolean)

        dbContext.APP_AvversitaxSpecie.Add(avversita_specie)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, avversita_specie As APP_AvversitaxSpecie)

        If avversita_specie Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_AvversitaxSpecie]")
        Else
            dbContext.APP_AvversitaxSpecie.Remove(avversita_specie)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
