Imports System.Text

Public Class GenerazioneXMLResult
    Public TotaleDaProcessare As Integer = 0
    Public TotaleGenerati As Integer = 0
    Public TotaleDaEliminare As Integer = 0
    Public TotaleEliminate As Integer = 0
    Public TotaleErroriEccezione As Integer = 0
    Public TotaleErroriPrecheck As Integer = 0
    Public TotaleBloccate As Integer = 0
    Public TotaleEstereNonGenerate As Integer = 0

    Public Overrides Function ToString() As String

        Dim sb = New StringBuilder()
        sb.AppendFormat("Documenti da generare -> {0}{1}", TotaleDaProcessare, Environment.NewLine)
        sb.AppendFormat("Documenti da eliminare -> {0}{1}", TotaleDaEliminare, Environment.NewLine)
        sb.AppendFormat("Documenti eliminati -> {0}{1}", TotaleEliminate, Environment.NewLine)
        sb.AppendFormat("Documenti bloccati -> {0}{1}", TotaleBloccate, Environment.NewLine)
        sb.AppendFormat("Documenti generati -> {0}{1}", TotaleGenerati, Environment.NewLine)
        sb.AppendFormat("Documenti non generati per check preliminari falliti -> {0}{1}", TotaleErroriPrecheck, Environment.NewLine)
        sb.AppendFormat("Documenti non generati per altri errori -> {0}{1}", TotaleErroriEccezione, Environment.NewLine)
        sb.AppendFormat("Documenti esteri non generati -> {0}{1}", TotaleEstereNonGenerate, Environment.NewLine)
        Return sb.ToString()

    End Function

End Class
