Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Avversita_R

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Avversita)

        Return dbContext.APP_Avversita.ToList()

    End Function

    Public Function Leggi(dbContext As GiasDbContext, av_cod As Integer) As APP_Avversita

        Dim rval As APP_Avversita

        rval = (From a In dbContext.APP_Avversita
                Where a.Av_Cod = av_cod).FirstOrDefault()

        Return rval

    End Function
    Public Function EstraiListaAvversitaFiltrataPerSpecie(dbContext As GiasDbContext, ByVal FiltroVeg_Cod_DestinazioneUso As String) As List(Of APP_Avversita)

        Dim FiltroVeg_Cod As Integer = 0
        If Not FiltroVeg_Cod_DestinazioneUso.Contains("/") Then
            FiltroVeg_Cod = CInt(FiltroVeg_Cod_DestinazioneUso)
        End If


        Dim rval As List(Of APP_Avversita) = (
              From m In dbContext.APP_Avversita
              Join mS In dbContext.APP_AvversitaxSpecie On m.Av_Cod Equals mS.Av_Cod
              Where mS.Veg_Cod = FiltroVeg_Cod
              Select m
          ).Distinct().OrderBy(Function(f) f.Av_Des_Vol).ToList()

        Return rval
    End Function


    ''' <summary>
    ''' legge una lista delle avversità filtrata per specie assieme ad una lista di gruppi di avversità
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="FiltroVeg_Cod_DestinazioneUso">se la stringa contiene il carattere '/' allora si tratta di una destinazione d'uso, quindi non viene applicato alcun filtro alle avversità</param>
    ''' <returns></returns>
    Public Function EstraiListaAvversitaFiltrataPerSpecieConGruppi(dbContext As GiasDbContext, ByVal FiltroVeg_Cod_DestinazioneUso As String) As List(Of AgronicaCoreModelloSTD.AvversitaConGruppi)

        Dim FiltroVeg_Cod As Integer = 0
        If Not FiltroVeg_Cod_DestinazioneUso.Contains("/") Then
            FiltroVeg_Cod = CInt(FiltroVeg_Cod_DestinazioneUso)
        End If

        Dim listaAvversita As List(Of AgronicaCoreModelloSTD.AvversitaConGruppi)

        If FiltroVeg_Cod = 0 Then
            listaAvversita = (
                From m In dbContext.APP_Avversita
                Select New AgronicaCoreModelloSTD.AvversitaConGruppi With {
                    .Cod = "0|" & m.Av_Cod.ToString,
                    .DescrizioneAvversitaGruppo = m.Av_Des_Vol
                    }
            ).Distinct().ToList()
        Else
            listaAvversita = (
                From m In dbContext.APP_Avversita
                Join mS In dbContext.APP_AvversitaxSpecie On m.Av_Cod Equals mS.Av_Cod
                Where (FiltroVeg_Cod = 0 OrElse mS.Veg_Cod = FiltroVeg_Cod)
                Select New AgronicaCoreModelloSTD.AvversitaConGruppi With {
                    .Cod = "0|" & m.Av_Cod.ToString,
                    .DescrizioneAvversitaGruppo = m.Av_Des_Vol
                    }
            ).Distinct().ToList()
        End If

        Dim rvalAvG As List(Of AgronicaCoreModelloSTD.AvversitaConGruppi) = listaAvversita.Concat(
            From g In dbContext.APP_GruppoAvversita
            Select New AgronicaCoreModelloSTD.AvversitaConGruppi With {
                    .Cod = g.Av_Gru.ToString & "|0",
                    .DescrizioneAvversitaGruppo = g.Av_Gru_Des
                    }
             ).OrderBy(Function(f) f.DescrizioneAvversitaGruppo).ToList()


        Return rvalAvG

    End Function


End Class

Public Class Avversita_W
    Public Sub Scrivi(dbContext As GiasDbContext, avversita As APP_Avversita, commit As Boolean)

        dbContext.APP_Avversita.Add(avversita)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, avversita As APP_Avversita)

        If avversita Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Avversita]")
        Else
            dbContext.APP_Avversita.Remove(avversita)
            dbContext.SaveChanges()
        End If

    End Sub
End Class