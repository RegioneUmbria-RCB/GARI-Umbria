Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class LogEventi_R

    Public Function Leggi(dbContext As GiasDbContext, Evento As String) As List(Of APP_LogEventi)

        Return dbContext.APP_LogEventi.Where(
            Function(item) (Evento = "" OrElse item.Evento = Evento)
        ).OrderBy(Function(f) f.DataOraRilevata).ToList()

    End Function

    Public Function LeggiEventiDaInviare(dbContext As GiasDbContext) As List(Of APP_LogEventi)

        Return dbContext.APP_LogEventi.Where(
            Function(item) (item.inviato = 0)
        ).OrderBy(Function(f) f.DataOraRilevata).ToList()

    End Function

    Public Function LeggiUltimoEvento(dbContext As GiasDbContext, Evento As String) As APP_LogEventi

        Return dbContext.APP_LogEventi.Where(
            Function(item) (Evento = "" OrElse item.Evento = Evento)
        ).OrderByDescending(Function(f) f.DataOraRilevata).FirstOrDefault()

    End Function

End Class

Public Class LogEventi_W

    Public Sub Scrivi(dbContext As GiasDbContext, evento As APP_LogEventi, commit As Boolean)

        dbContext.APP_LogEventi.Add(evento)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Inviato(dbContext As GiasDbContext, evento As APP_LogEventi)
        evento.inviato = 1
        dbContext.APP_LogEventi.Update(evento)
        dbContext.SaveChanges()
    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, evento As APP_LogEventi)

        If evento Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_LogEventi]")
        Else
            dbContext.APP_LogEventi.Remove(evento)
            dbContext.SaveChanges()
        End If

    End Sub

End Class