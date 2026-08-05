Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage


Public Class MisuraXIndiciMaturita
    Inherits BaseBiz


    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    Public Sub ScriviMisuraXIndiciMaturita(listMisuraXIndiciMaturita As List(Of APP_MisuraXIndiciMaturita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numMisuraXIndiciMaturita As Integer = listMisuraXIndiciMaturita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New MisuraXIndiciMaturita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each MisuraXIndiciMaturita In listMisuraXIndiciMaturita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numMisuraXIndiciMaturita
            xScrittura.Scrivi(dbContext, MisuraXIndiciMaturita, commit)
        Next

    End Sub

    Public Sub CancellaMisuraXIndiciMaturita(MisuraXIndiciMaturita As APP_MisuraXIndiciMaturita)

        Dim xScrittura = New MisuraXIndiciMaturita_W()
        xScrittura.Cancella(dbContext, MisuraXIndiciMaturita)

    End Sub
End Class
