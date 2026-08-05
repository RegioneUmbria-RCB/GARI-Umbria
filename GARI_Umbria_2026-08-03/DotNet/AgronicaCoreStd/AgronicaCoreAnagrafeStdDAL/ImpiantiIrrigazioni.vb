Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class ImpiantiIrrigazioni_R
    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_ImpiantiIrrigazioni)

        Return dbContext.APP_ImpiantiIrrigazioni.OrderBy(Function(f) f.Imp_Des).ToList()

    End Function
End Class

Public Class ImpiantiIrrigazioni_W
    Public Sub Scrivi(dbContext As GiasDbContext, impianto_irrigazione As APP_ImpiantiIrrigazioni, commit As Boolean)

        dbContext.APP_ImpiantiIrrigazioni.Add(impianto_irrigazione)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, impianto_irrigazione As APP_ImpiantiIrrigazioni)

        If impianto_irrigazione Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_ImpiantiIrrigazioni]")
        Else
            dbContext.APP_ImpiantiIrrigazioni.Remove(impianto_irrigazione)
            dbContext.SaveChanges()
        End If

    End Sub
End Class
