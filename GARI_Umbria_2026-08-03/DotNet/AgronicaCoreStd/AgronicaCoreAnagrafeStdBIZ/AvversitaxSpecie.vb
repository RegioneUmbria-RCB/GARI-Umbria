Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class AvversitaxSpecie
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiAvversitaSpecie() As List(Of APP_AvversitaxSpecie)

        Dim xLettura = New AvversitaxSpecie_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Sub ScriviAvversitaSpecie(listAvversitaSpecie As List(Of APP_AvversitaxSpecie), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numAvversitaSpecie As Integer = listAvversitaSpecie.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New AvversitaxSpecie_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each avversita_specie In listAvversitaSpecie
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numAvversitaSpecie
            xScrittura.Scrivi(dbContext, avversita_specie, commit)
        Next

    End Sub

    Public Sub CancellaAvversitaSpecie(avversita_specie As APP_AvversitaxSpecie)

        Dim xScrittura = New AvversitaxSpecie_W()
        xScrittura.Cancella(dbContext, avversita_specie)

    End Sub
End Class
