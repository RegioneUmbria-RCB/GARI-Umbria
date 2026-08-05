Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Parco_Macchine_R
    Public Function Leggi(dbContext As GiasDbContext, Piva As String, Mac_Cod As Integer, ByVal DataRiferimento As Date) As List(Of APP_Parco_Macchine)

        Return dbContext.APP_Parco_Macchine.Where(
            Function(item) ((Piva Is Nothing OrElse item.Sa_Cod = -1 OrElse item.Piva = Piva) AndAlso
            (Mac_Cod = 0 OrElse item.Mac_Cod = Mac_Cod) AndAlso (
                item.Validita_Inizio <= DataRiferimento AndAlso
                item.Validita_Fine >= DataRiferimento
            ))
        ).OrderBy(Function(f) f.Mac_Des).ToList()

    End Function
End Class

Public Class Parco_Macchine_W
    Public Sub Scrivi(dbContext As GiasDbContext, parco_macchine As APP_Parco_Macchine, commit As Boolean)

        dbContext.APP_Parco_Macchine.Add(parco_macchine)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, parco_macchine As APP_Parco_Macchine, piva As String)

        If parco_macchine Is Nothing Then
            If String.IsNullOrEmpty(piva) Then
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Parco_Macchine]") 'WHERE Sa_Cod = -1
            Else
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Parco_Macchine] WHERE Piva={0} AND Sa_Cod <> -1", piva)
            End If
        Else
            dbContext.APP_Parco_Macchine.Remove(parco_macchine)
            dbContext.SaveChanges()
        End If

    End Sub
End Class
