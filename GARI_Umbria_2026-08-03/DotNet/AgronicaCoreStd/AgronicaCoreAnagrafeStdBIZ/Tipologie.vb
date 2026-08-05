Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreDataProviderSTD.TipiEnumerativi
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models
Imports AgronicaCoreModelloSTD

Public Class Tipologie
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiTipologie() As List(Of APP_Tipologie)
        Dim xLettura = New Tipologie_R()
        Return xLettura.Leggi(dbContext)
    End Function

    Public Sub ScriviTipologie(listTipologie As List(Of APP_Tipologie), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numTipologie As Integer = listTipologie.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Tipologie_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each tipologia In listTipologie
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numTipologie
            xScrittura.Scrivi(dbContext, tipologia, commit)
        Next

    End Sub

End Class
