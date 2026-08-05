Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class AttivitaXCentriAziendali_R

    Public Function Leggi(dbContext As GiasDbContext, Piva As String, Sa_Cod As Integer) As List(Of APP_AttivitaXCentri_Aziendali)

        Return dbContext.APP_AttivitaXCentri_Aziendali.Where(
            Function(item) (Piva = "" OrElse item.Piva = Piva) AndAlso (Sa_Cod = 0 OrElse item.Sa_Cod = Sa_Cod)
        ).OrderBy(Function(f) f.Id_Attivita).ToList()

    End Function

End Class

Public Class AttivitaXCentriAziendali_W
    Public Sub Scrivi(dbContext As GiasDbContext, attivita As APP_AttivitaXCentri_Aziendali, commit As Boolean)

        dbContext.APP_AttivitaXCentri_Aziendali.Add(attivita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, attivita As APP_AttivitaXCentri_Aziendali, piva As String)

        If attivita Is Nothing Then
            If String.IsNullOrEmpty(piva) Then
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_AttivitaXCentri_Aziendali] WHERE Piva IS NULL")
            Else
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_AttivitaXCentri_Aziendali] WHERE Piva={0}", piva)
            End If
        Else
            dbContext.APP_AttivitaXCentri_Aziendali.Remove(attivita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class