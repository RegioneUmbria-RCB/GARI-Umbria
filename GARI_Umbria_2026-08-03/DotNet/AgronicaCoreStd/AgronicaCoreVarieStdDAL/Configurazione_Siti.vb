Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Configurazione_Siti_R

    Public Function Leggi(dbContext As GiasDbContext, Chiave As String) As APP_Configurazione_Siti

        Return dbContext.APP_Configurazione_Siti.Where(Function(item) (item.Chiave = Chiave)).SingleOrDefault()

    End Function

End Class

Public Class Configurazione_Siti_W

    Public Sub Scrivi(dbContext As GiasDbContext, configurazione As APP_Configurazione_Siti)

        dbContext.APP_Configurazione_Siti.Add(configurazione)
        dbContext.SaveChanges()

    End Sub

    Public Sub Modifica(dbContext As GiasDbContext, configurazione As APP_Configurazione_Siti)

        dbContext.APP_Configurazione_Siti.Update(configurazione)
        dbContext.SaveChanges()

    End Sub


    Public Sub Cancella(dbContext As GiasDbContext, configurazione As APP_Configurazione_Siti)

        dbContext.APP_Configurazione_Siti.Remove(configurazione)
        dbContext.SaveChanges()

    End Sub

End Class

