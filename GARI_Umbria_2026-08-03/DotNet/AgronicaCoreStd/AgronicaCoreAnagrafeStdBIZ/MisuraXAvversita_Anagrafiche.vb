
Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports Microsoft.EntityFrameworkCore.Storage


Public Class MisuraXAvversita_Anagrafiche
    Inherits BaseBiz


    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub


    Public Sub ScriviMisuraXAvversita_Anagrafiche(listMisuraXAvversita_Anagrafiche As List(Of APP_MisuraXAvversita_Anagrafiche), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numMisuraXAvversita_Anagrafiche As Integer = listMisuraXAvversita_Anagrafiche.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New MisuraXAvversita_Anagrafiche_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each MisuraXAvversita_Anagrafiche In listMisuraXAvversita_Anagrafiche
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numMisuraXAvversita_Anagrafiche
            xScrittura.Scrivi(dbContext, MisuraXAvversita_Anagrafiche, commit)
        Next

    End Sub

    Public Sub CancellaMisuraXAvversita_Anagrafiche(MisuraXAvversita_Anagrafiche As APP_MisuraXAvversita_Anagrafiche)

        Dim xScrittura = New MisuraXAvversita_Anagrafiche_W()
        xScrittura.Cancella(dbContext, MisuraXAvversita_Anagrafiche)

    End Sub
End Class
