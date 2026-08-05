Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class Parco_Macchine
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiParcoMacchine(Piva As String, ByVal DataRiferimento As Date) As List(Of APP_Parco_Macchine)

        Dim xLettura = New Parco_Macchine_R()
        Return xLettura.Leggi(dbContext, Piva, 0, DataRiferimento)

    End Function

    Public Sub ScriviParcoMacchine(listParcoMacchine As List(Of APP_Parco_Macchine), piva As String, cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numParcoMacchine As Integer = listParcoMacchine.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New Parco_Macchine_W()

        If cancella Then
            CancellaParcoMacchine(piva)
        End If

        For Each parco_macchine In listParcoMacchine
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numParcoMacchine
            xScrittura.Scrivi(dbContext, parco_macchine, commit)
        Next

    End Sub

    Public Sub CancellaParcoMacchine(parco_macchine As APP_Parco_Macchine)

        Dim xScrittura = New Parco_Macchine_W()
        xScrittura.Cancella(dbContext, parco_macchine, Nothing)

    End Sub

    Public Sub CancellaParcoMacchine(piva As String)

        Dim xScrittura = New Parco_Macchine_W()
        xScrittura.Cancella(dbContext, Nothing, piva)

    End Sub

End Class
