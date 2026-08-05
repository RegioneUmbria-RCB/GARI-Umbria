Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class CentriAziendali_R

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Centri_Aziendali)

        Return dbContext.APP_Centri_Aziendali.ToList()

    End Function

    ''' <summary>
    ''' Estrae la lista di centri aziendali
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="FiltroPiva">Imposta il filtro sulla piva se diverso da stringa vuota</param>
    ''' <returns></returns>
    Public Function EstraiListaCentriAziendali(dbContext As GiasDbContext, FiltroPiva As String) As List(Of AgronicaCoreModelloSTD.Centri_Aziendali)

        Dim rval As List(Of AgronicaCoreModelloSTD.Centri_Aziendali) = (
            From c In dbContext.APP_Centri_Aziendali
            Join i In dbContext.APP_Imprese On c.piva Equals i.piva
            Where c.piva = FiltroPiva OrElse FiltroPiva = ""
            Distinct Select New AgronicaCoreModelloSTD.Centri_Aziendali With {
                             .Piva = c.piva,
                             .Sa_Cod = c.sa_cod,
                             .Rag_Soc = i.rag_soc,
                             .Sa_Nome = c.sa_nome
                             }
            ).Distinct().OrderBy(Function(f) f.Rag_Soc).ThenBy(Function(g) g.Sa_Nome).ToList()

        Return rval

    End Function

End Class

Public Class CentriAziendali_W

    Public Sub Scrivi(dbContext As GiasDbContext, centro As APP_Centri_Aziendali, commit As Boolean)

        dbContext.APP_Centri_Aziendali.Add(centro)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, centro As APP_Centri_Aziendali, piva As String)

        If centro Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Centri_Aziendali] WHERE piva={0}", piva)
        Else
            dbContext.APP_Centri_Aziendali.Remove(centro)
            dbContext.SaveChanges()
        End If

    End Sub

End Class