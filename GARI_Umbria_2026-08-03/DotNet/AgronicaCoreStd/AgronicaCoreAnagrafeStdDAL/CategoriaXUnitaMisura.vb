Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class CategorieXUnitaMisura_W

    Public Sub Scrivi(dbContext As GiasDbContext, categoria_um As APP_CategorieXUnitaMisura, commit As Boolean)

        dbContext.APP_CategorieXUnitaMisura.Add(categoria_um)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, categoria_um As APP_CategorieXUnitaMisura)

        If categoria_um Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_CategorieXUnitaMisura]")
        Else
            dbContext.APP_CategorieXUnitaMisura.Remove(categoria_um)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
