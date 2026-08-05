Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class GruppoAvversita
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiGruppoAvversita() As List(Of APP_GruppoAvversita)

        Dim xLettura = New GruppoAvversita_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Sub ScriviGruppoAvversita(listGruppoAvversita As List(Of APP_GruppoAvversita), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numGruppoAvversita As Integer = listGruppoAvversita.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New GruppoAvversita_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each gruppo_avversita In listGruppoAvversita
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numGruppoAvversita
            xScrittura.Scrivi(dbContext, gruppo_avversita, commit)
        Next

    End Sub

    Public Sub CancellaAvversita(gruppo_avversita As APP_GruppoAvversita)

        Dim xScrittura = New GruppoAvversita_W()
        xScrittura.Cancella(dbContext, gruppo_avversita)

    End Sub
End Class
