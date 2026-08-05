Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Epoche
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiEpoche() As List(Of APP_Epoche)

        Dim xLettura = New Epoche_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Function EstraiListaEpocheFiltrataPerSpecie(ByVal FiltroVeg_Cod_DestinazioneUso As String) As List(Of APP_Epoche)

        Dim leggi As New Epoche_R()
        Return leggi.EstraiListaEpocheFiltrataPerSpecie(dbContext, FiltroVeg_Cod_DestinazioneUso)

    End Function

    Public Sub ScriviEpoche(listEpoche As List(Of APP_Epoche), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numEpoche As Integer = listEpoche.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Epoche_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each epoca In listEpoche
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numEpoche
            xScrittura.Scrivi(dbContext, epoca, commit)
        Next

    End Sub

    Public Sub CancellaEpoche(epoca As APP_Epoche)

        Dim xScrittura = New Epoche_W()
        xScrittura.Cancella(dbContext, epoca)

    End Sub
End Class
