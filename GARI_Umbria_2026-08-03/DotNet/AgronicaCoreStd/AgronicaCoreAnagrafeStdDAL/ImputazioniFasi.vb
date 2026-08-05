Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class ImputazioniFasi_R

    Public Function Leggi(dbContext As GiasDbContext, Id_Attivita As Integer, Id_Imputazione As Integer, Piva As String) As List(Of APP_Imputazioni_Fasi)

        Return dbContext.APP_Imputazioni_Fasi.Where(
            Function(item) ((Id_Attivita = 0 OrElse item.Id_Attivita = Id_Attivita) AndAlso
                           (Id_Imputazione = 0 OrElse item.Imputazione_Cod = Id_Imputazione) AndAlso
                           (Piva = "" OrElse item.Piva = Piva))
        ).OrderBy(Function(f) f.Imputazione_Nome).ToList()

    End Function

End Class

Public Class ImputazioniFasi_W

    Public Sub Scrivi(dbContext As GiasDbContext, progetto As APP_Imputazioni_Fasi, commit As Boolean)

        dbContext.APP_Imputazioni_Fasi.Add(progetto)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, progetto As APP_Imputazioni_Fasi, piva As String)

        If progetto Is Nothing Then
            If String.IsNullOrEmpty(piva) Then
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Imputazioni_Fasi] WHERE Piva IS NULL")
            Else
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Imputazioni_Fasi] WHERE Piva={0}", piva)
            End If
        Else
            dbContext.APP_Imputazioni_Fasi.Remove(progetto)
            dbContext.SaveChanges()
        End If

    End Sub

End Class