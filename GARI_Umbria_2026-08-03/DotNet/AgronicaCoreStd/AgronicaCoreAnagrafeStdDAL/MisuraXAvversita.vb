

Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class MisuraXAvversita_R

    Public Function Leggi(dbContext As GiasDbContext, veg_cod As Integer, av_cod As Integer, av_gru As Integer, ByVal udm_Cod As Integer) As List(Of AgronicaCoreModelloSTD.MisuraXAvversita)

        Dim listaMxAV As List(Of AgronicaCoreModelloSTD.MisuraXAvversita)
        listaMxAV = (From a In dbContext.APP_MisuraxAvversita
                     Join udm In dbContext.APP_CategorieXUnitaMisura
                       On a.UDM_COD Equals udm.Udm_Cod
                     Group Join av In dbContext.APP_Avversita.Where(Function(x) x.Av_Cod <> 0).DefaultIfEmpty()
                       On a.AV_COD Equals av.Av_Cod
                    Into aj = Group From av In aj.DefaultIfEmpty()
                     Group Join g In dbContext.APP_GruppoAvversita.Where(Function(x) x.Av_Gru <> 0).DefaultIfEmpty()
                        On g.Av_Gru Equals a.AV_GRU
                    Into gj = Group From g In gj.DefaultIfEmpty()
                     Where (a.VEG_COD = veg_cod OrElse veg_cod = 0) AndAlso
                           (a.AV_COD = av_cod OrElse av_cod = 0) AndAlso
                           (a.AV_GRU = av_gru OrElse av_gru = 0) AndAlso
                           (a.UDM_COD = udm_Cod OrElse udm_Cod = 0)
                     Select New AgronicaCoreModelloSTD.MisuraXAvversita With {
                            .av_cod = a.AV_COD,
                            .av_gru_cod = CInt(a.AV_GRU),
                            .udm_cod = a.UDM_COD,
                            .giustificazioneDP = False,
                            .MisuraAvversitaDescrizione = If(aj Is Nothing, "", av.Av_Des_Vol) & If(gj Is Nothing, "", g.Av_Gru_Des) & " | (" & udm.Udm_Sim & ")"
                           }).ToList().OrderBy(Function(x) x.MisuraAvversitaDescrizione).ToList()

        For Each e In listaMxAV
            If e.giustificazioneDP Then
                e.MisuraAvversitaDescrizione = "(*) " & e.MisuraAvversitaDescrizione
            End If
        Next

        Return listaMxAV
    End Function
End Class

Public Class MisuraXAvversita_W


    Public Sub Scrivi(dbContext As GiasDbContext, MisuraXAvversita As APP_MisuraxAvversita, commit As Boolean)

        dbContext.APP_MisuraxAvversita.Add(MisuraXAvversita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, MisuraXAvversita As APP_MisuraxAvversita)

        If MisuraXAvversita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_MisuraXAvversita]")
        Else
            dbContext.APP_MisuraxAvversita.Remove(MisuraXAvversita)
            dbContext.SaveChanges()
        End If

    End Sub

End Class