Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage

Public Class Avversita
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    ''' <summary>
    ''' legge una lista delle avversità filtrata per specie assieme ad una lista di gruppi di avversità
    ''' </summary>
    ''' <param name="dbContext"></param>
    ''' <param name="FiltroVeg_Cod_DestinazioneUso">se la stringa contiene il carattere '/' allora si tratta di una destinazione d'uso, quindi non viene applicato alcun filtro alle avversità</param>
    ''' <returns></returns>
    Public Function EstraiListaAvversitaFiltrataPerSpecieConGruppi(ByVal FiltroVeg_Cod_DestinazioneUso As String) As List(Of AgronicaCoreModelloSTD.AvversitaConGruppi)

        Dim leggi As New Avversita_R()
        Return leggi.EstraiListaAvversitaFiltrataPerSpecieConGruppi(dbContext, FiltroVeg_Cod_DestinazioneUso)

    End Function


    Public Function EstraiListaAvversitaFiltrataPerSpecie(ByVal FiltroVeg_Cod_DestinazioneUso As String) As List(Of APP_Avversita)

        Dim leggi As New Avversita_R()
        Return leggi.EstraiListaAvversitaFiltrataPerSpecie(dbContext, FiltroVeg_Cod_DestinazioneUso)

    End Function

    Public Function LeggiAvversita() As List(Of APP_Avversita)

        Dim xLettura = New Avversita_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Sub ScriviAvversita(listAvversita As List(Of APP_Avversita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numAvversita As Integer = listAvversita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Avversita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each avversita In listAvversita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numAvversita
            xScrittura.Scrivi(dbContext, avversita, commit)
        Next

    End Sub

    Public Sub CancellaAvversita(avversita As APP_Avversita)

        Dim xScrittura = New Avversita_W()
        xScrittura.Cancella(dbContext, avversita)

    End Sub

End Class
