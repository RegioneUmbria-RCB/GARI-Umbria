Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class AttivitaXOperazioni_R

    Public Function Leggi(dbContext As GiasDbContext, Lav_Cod As Integer) As List(Of APP_AttivitaXOperazioni)

        Return dbContext.APP_AttivitaXOperazioni.Where(
            Function(item) (Lav_Cod = 0 OrElse item.Lav_Cod = Lav_Cod)
        ).OrderBy(Function(f) f.Descrizione).ToList()

    End Function

End Class

Public Class AttivitaXOperazioni_W
    Public Sub Scrivi(dbContext As GiasDbContext, attivita As APP_AttivitaXOperazioni, commit As Boolean)

        dbContext.APP_AttivitaXOperazioni.Add(attivita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, attivita As APP_AttivitaXOperazioni)

        If attivita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_AttivitaXOperazioni]")
        Else
            dbContext.APP_AttivitaXOperazioni.Remove(attivita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
