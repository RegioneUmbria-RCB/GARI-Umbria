Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Operazioni_R


    ''' <summary>
    ''' Estrae lista Operazioni
    ''' </summary>
    ''' <param name="dbcontext"></param>
    ''' <param name="listaGruppiOperazioni">Lista di gruppi operazioni per filtro, se vuoto saranno lette le op. per qualsiasi gruppo op.</param>
    ''' <returns>lista di specie</returns>
    Public Function EstraiListaOperazioni(dbcontext As GiasDbContext, listaGruppiOperazioni As List(Of Integer)) As List(Of AgronicaCoreModelloSTD.Operazione)

        Dim rval As List(Of AgronicaCoreModelloSTD.Operazione) = (
           From i In dbcontext.APP_Operazioni
           Where listaGruppiOperazioni.Count = 0 OrElse (listaGruppiOperazioni.Contains(i.gru_cod))
           Select New AgronicaCoreModelloSTD.Operazione With {
                            .lav_cod = i.lav_cod,
                            .lav_des = i.lav_des
                            }
           ).Distinct().OrderBy(Function(f) f.lav_des).ToList()


        Return rval

    End Function

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Operazioni)

        Return dbContext.APP_Operazioni.ToList()

    End Function

    Public Function Leggi(dbContext As GiasDbContext, lav_cod As Integer) As APP_Operazioni

        Return (From o In dbContext.APP_Operazioni Where o.lav_cod = lav_cod Select o).FirstOrDefault

    End Function
End Class

Public Class Operazioni_W
    Public Sub Scrivi(dbContext As GiasDbContext, operazione As APP_Operazioni, commit As Boolean)

        dbContext.APP_Operazioni.Add(operazione)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, operazione As APP_Operazioni)

        If operazione Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Operazioni]")
        Else
            dbContext.APP_Operazioni.Remove(operazione)
            dbContext.SaveChanges()
        End If

    End Sub

End Class