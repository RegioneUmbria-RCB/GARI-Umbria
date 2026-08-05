Imports AgronicaCoreAnagrafeStdDAL
Imports AgronicaCoreEntityFrameworkSTD
Imports AgronicaCoreEntityFrameworkSTD_POCO.Models

Public Class ImpiantiIrrigazioni
    Inherits BaseBiz

    Public Sub New(dbContext As GiasDbContext)
        MyBase.New(dbContext)
    End Sub

    Public Function LeggiImpiantiIrrigazioni() As List(Of APP_ImpiantiIrrigazioni)

        Dim xLettura = New ImpiantiIrrigazioni_R()
        Return xLettura.Leggi(dbContext)

    End Function

    Public Sub ScriviImpiantiIrrigazioni(listImpiantiIrrigazioni As List(Of APP_ImpiantiIrrigazioni), cancella As Boolean)

        Dim count As Integer = 0
        Dim commit As Boolean = False
        Dim numImpiantiIrrigazioni As Integer = listImpiantiIrrigazioni.Count
        Dim commitCount As Integer = 100
        Dim xScrittura = New ImpiantiIrrigazioni_W()

        If cancella Then
            xScrittura.Cancella(dbContext, Nothing)
        End If

        For Each impianto_irrigazione In listImpiantiIrrigazioni
            count += 1
            commit = count Mod commitCount = 0 OrElse count = numImpiantiIrrigazioni
            xScrittura.Scrivi(dbContext, impianto_irrigazione, commit)
        Next

    End Sub

    Public Sub CancellaImpiantiIrrigazioni(impianto_irrigazione As APP_ImpiantiIrrigazioni)

        Dim xScrittura = New ImpiantiIrrigazioni_W()
        xScrittura.Cancella(dbContext, impianto_irrigazione)

    End Sub
End Class
