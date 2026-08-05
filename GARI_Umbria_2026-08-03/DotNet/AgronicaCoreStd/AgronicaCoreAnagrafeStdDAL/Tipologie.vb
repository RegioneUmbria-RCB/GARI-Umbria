Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Tipologie_R

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Tipologie)

        Return dbContext.APP_Tipologie.OrderBy(Function(f) f.Nome_Area).ThenBy(Function(g) g.Nome_Tipologia).ToList()

    End Function

End Class

Public Class Tipologie_W

    Public Sub Scrivi(dbContext As GiasDbContext, tipologia As APP_Tipologie, commit As Boolean)

        dbContext.APP_Tipologie.Add(tipologia)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, tipologia As APP_Tipologie)

        If tipologia Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Tipologie]")
        Else
            dbContext.APP_Tipologie.Remove(tipologia)
            dbContext.SaveChanges()
        End If

    End Sub

End Class