

Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreModelloSTD
Imports Microsoft.EntityFrameworkCore


Public Class IndiciMaturita_R
    Public Function EstraiListaIndiciMaturitaFiltrataPerSpecie(dbContext As GiasDbContext, veg_Cod As Integer, IND_MAT_COD As Integer, udm_cod As Integer) As List(Of MisuraXindiciMaturita)

        Dim listaIm As List(Of MisuraXindiciMaturita) = (
            From ims In dbContext.APP_IndiciMaturitaxSpecieVegetali
            Join im In dbContext.APP_IndiciMaturita
                On ims.IND_MAT_COD Equals im.IND_MAT_COD
            Join m In dbContext.APP_MisuraXindiciMaturita
                On m.IND_MAT_COD Equals im.IND_MAT_COD
            Join udm In dbContext.APP_CategorieXUnitaMisura
                On udm.Udm_Cod Equals m.UDM_COD
            Where (ims.VEG_COD = veg_Cod OrElse veg_Cod = 0) AndAlso
                  (im.IND_MAT_COD = IND_MAT_COD OrElse IND_MAT_COD = 0) AndAlso
                  (m.UDM_COD = udm_cod OrElse udm_cod = 0)
            Select New MisuraXindiciMaturita With {
                .IND_MAT_COD = im.IND_MAT_COD,
                .udm_cod = m.UDM_COD,
                .MisuraIndiciMaturitaDescrizione = im.IND_MAT_DES & " (" & udm.Udm_des & ")"
                }
            ).ToList().OrderBy(Function(x) x.MisuraIndiciMaturitaDescrizione).ToList()

        Return listaIm
    End Function

End Class

Public Class IndiciMaturita_W


    Public Sub Scrivi(dbContext As GiasDbContext, IndiciMaturita As APP_IndiciMaturita, commit As Boolean)

        dbContext.APP_IndiciMaturita.Add(IndiciMaturita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, IndiciMaturita As APP_IndiciMaturita)

        If IndiciMaturita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_IndiciMaturita]")
        Else
            dbContext.APP_IndiciMaturita.Remove(IndiciMaturita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
