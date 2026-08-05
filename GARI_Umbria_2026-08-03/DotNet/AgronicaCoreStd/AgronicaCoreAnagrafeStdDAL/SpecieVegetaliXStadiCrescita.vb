
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore
Public Class SpecieVegetaliXStadiCrescita_R
    Public Function EstraiListaSpecieVegetaliXStadiCrescitaFiltrataPerSpecieBBCH(dbContext As GiasDbContext, veg_Cod As Integer, Cod_SS As Integer) As List(Of AgronicaCoreModelloSTD.SpecieVegetaliXStadiCrescita)



        Dim listaFF As List(Of AgronicaCoreModelloSTD.SpecieVegetaliXStadiCrescita)
        listaFF = (
               From m In dbContext.APP_SpecieVegetaliXStadiCrescita
               Where ((veg_Cod = 0 OrElse m.Veg_Cod = veg_Cod) AndAlso (Cod_SS = 0 OrElse m.Cod_SS = Cod_SS))
               Select New AgronicaCoreModelloSTD.SpecieVegetaliXStadiCrescita With {
                    .Cod_SS = m.Cod_SS,
                    .Veg_Cod = m.Veg_Cod,
                    .ID_BBCH = m.ID_BBCH,
                    .Cod_MS = m.Cod_MS,
                    .Progressivo = m.Progressivo,
                    .Descrizione = m.Descrizione,
                    .FF_Cod = m.FF_Cod,
                    .Flag_Fioritura = m.Flag_Fioritura,
                    .Flag_Visibile = m.Flag_Visibile
                   }
           ).Distinct().ToList()

        For Each ff In listaFF
            If CBool(ff.Flag_Fioritura) Then
                ff.Descrizione = "(*) " & ff.Descrizione
            End If
        Next

        Return listaFF
    End Function
End Class
Public Class SpecieVegetaliXStadiCrescita_W


        Public Sub Scrivi(dbContext As GiasDbContext, SpecieVegetaliXStadiCrescita As APP_SpecieVegetaliXStadiCrescita, commit As Boolean)

            dbContext.APP_SpecieVegetaliXStadiCrescita.Add(SpecieVegetaliXStadiCrescita)

            If commit Then
                dbContext.SaveChanges()
            End If

        End Sub

        Public Sub Cancella(dbContext As GiasDbContext, SpecieVegetaliXStadiCrescita As APP_SpecieVegetaliXStadiCrescita)

            If SpecieVegetaliXStadiCrescita Is Nothing Then
                dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_SpecieVegetaliXStadiCrescita]")
            Else
                dbContext.APP_SpecieVegetaliXStadiCrescita.Remove(SpecieVegetaliXStadiCrescita)
                dbContext.SaveChanges()
            End If

        End Sub

    End Class
