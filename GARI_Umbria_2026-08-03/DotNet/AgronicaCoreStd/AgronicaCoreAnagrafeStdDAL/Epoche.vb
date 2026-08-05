Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore

Public Class Epoche_R

    Public Function Leggi(dbContext As GiasDbContext) As List(Of APP_Epoche)

        Return dbContext.APP_Epoche.OrderBy(Function(f) f.Epoca_Des).ToList()

    End Function

    Public Function Leggi(dbContext As GiasDbContext, cod As Integer) As APP_Epoche

        Dim rval As APP_Epoche = (
            From g In dbContext.APP_Epoche
            Where g.Epoca_Cod = cod
            ).FirstOrDefault


        Return rval

    End Function

    Public Function EstraiListaEpocheFiltrataPerSpecie(dbContext As GiasDbContext, ByVal FiltroVeg_Cod_DestinazioneUso As String) As List(Of APP_Epoche)

        Dim FiltroVeg_Cod As Integer = 0
        If Not FiltroVeg_Cod_DestinazioneUso.Contains("/") Then
            FiltroVeg_Cod = CInt(FiltroVeg_Cod_DestinazioneUso)
        End If

        Dim rval As List(Of APP_Epoche) = (
              From m In dbContext.APP_Epoche
              Where m.Specie_Cod = FiltroVeg_Cod
              Select m
          ).Distinct().OrderBy(Function(f) f.Epoca_Des).ToList()

        Return rval
    End Function

End Class

Public Class Epoche_W
    Public Sub Scrivi(dbContext As GiasDbContext, epoca As APP_Epoche, commit As Boolean)

        dbContext.APP_Epoche.Add(epoca)

        If commit Then
            dbContext.SaveChanges()
        End If

    End Sub

    Public Sub Cancella(dbContext As GiasDbContext, epoca As APP_Epoche)

        If epoca Is Nothing Then
            dbContext.Database.ExecuteSqlCommandAsync("DELETE FROM [APP_Epoche]")
        Else
            dbContext.APP_Epoche.Remove(epoca)
            dbContext.SaveChanges()
        End If

    End Sub

End Class
