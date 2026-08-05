Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage


Public Class IndiciMaturitaxSpecieVegetali
    Inherits BaseBiz


    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    Public Sub ScriviIndiciMaturitaxSpecieVegetali(listIndiciMaturitaxSpecieVegetali As List(Of APP_IndiciMaturitaxSpecieVegetali), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numIndiciMaturitaxSpecieVegetali As Integer = listIndiciMaturitaxSpecieVegetali.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New IndiciMaturitaxSpecieVegetali_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each IndiciMaturitaxSpecieVegetali In listIndiciMaturitaxSpecieVegetali
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numIndiciMaturitaxSpecieVegetali
            xScrittura.Scrivi(dbContext, IndiciMaturitaxSpecieVegetali, commit)
        Next

    End Sub

    Public Sub CancellaIndiciMaturitaxSpecieVegetali(IndiciMaturitaxSpecieVegetali As APP_IndiciMaturitaxSpecieVegetali)

        Dim xScrittura = New IndiciMaturitaxSpecieVegetali_W()
        xScrittura.Cancella(dbContext, IndiciMaturitaxSpecieVegetali)

    End Sub
End Class
