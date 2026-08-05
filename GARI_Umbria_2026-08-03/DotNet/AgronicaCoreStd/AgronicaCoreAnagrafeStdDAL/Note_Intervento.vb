Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Note_Intervento_R
    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Note_Intervento)

        Return dbContext.APP_Note_Intervento.ToList()

    End Function

End Class

Public Class Note_Intervento_W
    Public Sub Scrivi(dbContext As GiasDbContext, note_intervento As APP_Note_Intervento, commit As Boolean)

        dbContext.APP_Note_Intervento.Add(note_intervento)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, note_intervento As APP_Note_Intervento)

        If note_intervento Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Note_Intervento]")
        Else
            dbContext.APP_Note_Intervento.Remove(note_intervento)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
