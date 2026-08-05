Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Coordinate_R

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_GIS)

        Return dbContext.APP_GIS.Where(
            Function(item) (item.inviato = 0)
        ).OrderBy(Function(f) f.DataOraRilevata).ToList()

    End Function

    Public Function LeggiUltimaCoordinata(dbContext As GiasDbContext) As APP_GIS

        Return dbContext.APP_GIS.OrderByDescending(Function(f) f.DataOraRilevata).FirstOrDefault()

    End Function

End Class

Public Class Coordinate_W

    Public Sub Scrivi(dbContext As GiasDbContext, coordinate As APP_GIS, commit As Boolean)

        dbContext.APP_GIS.Add(coordinate)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, coordinate As APP_GIS)

        If coordinate Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_GIS]")
        Else
            dbContext.APP_GIS.Remove(coordinate)
            dbContext.SaveChanges()
        End If

    End Sub
End Class