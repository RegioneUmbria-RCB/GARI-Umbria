Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' Logger side-by-side per confrontare la SQL generata dal legacy
''' <c>Filtrone.CreaStringaQueryPerDTFiltrone</c> con quella prodotta
''' dalla shared lib <c>AgronicaCoreVisibilitaStd.VisibilitaQueryBuilder</c>.
'''
''' Scrive in append su <c>%TEMP%\AgronicaVisibilitaDiff.log</c>.
''' Thread-safe via SyncLock. Non deve MAI far fallire il login:
''' ogni errore interno &#232; silenziato.
''' </summary>
Public NotInheritable Class VisibilitaDiffLogger

    Private Shared ReadOnly _lock As New Object()
    Private Shared ReadOnly _whitespace As New Regex("\s+", RegexOptions.Compiled, TimeSpan.FromSeconds(3))

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Logga una riga di confronto SQL legacy vs shared lib.
    ''' </summary>
    ''' <param name="nomeFunzione">Nome della funzione chiamante (es. "_EF", "_GUID_NoTransaction").
    ''' Serve a distinguere il flow (import vs login) nei log.</param>
    ''' <param name="modalita">Modalit&#224; query usata dalla shared lib (es. "Full", "UvaLean").</param>
    ''' <param name="utenteUsername">Username analizzato.</param>
    ''' <param name="tipo">"Imprese" o "Centri".</param>
    ''' <param name="sqlLegacy">SQL prodotta da Filtrone.</param>
    ''' <param name="sqlNew">SQL prodotta da VisibilitaQueryBuilder.</param>
    ''' <param name="countLegacy">Righe effettivamente lette dal legacy (per context).</param>
    Public Shared Sub LogDiff(
            nomeFunzione As String,
            modalita As String,
            utenteUsername As String,
            tipo As String,
            sqlLegacy As String,
            sqlNew As String,
            countLegacy As Integer)
        Try
            Dim normLegacy = NormalizzaSql(sqlLegacy)
            Dim normNew = NormalizzaSql(sqlNew)
            Dim esito = If(String.Equals(normLegacy, normNew, StringComparison.OrdinalIgnoreCase), "OK", "DIFF")

            Dim sb As New StringBuilder()
            sb.Append("[").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")).Append("] ")
            sb.Append("fn=").Append(nomeFunzione).Append(" ")
            sb.Append("mod=").Append(modalita).Append(" ")
            sb.Append("user=").Append(utenteUsername).Append(" ")
            sb.Append("tipo=").Append(tipo).Append(" ")
            sb.Append("countLegacy=").Append(countLegacy).Append(" ")
            sb.Append("sqlMatch=").Append(esito)
            sb.AppendLine()

            If esito = "DIFF" Then
                sb.AppendLine("  --- LEGACY ---")
                sb.AppendLine("  " & sqlLegacy)
                sb.AppendLine("  --- NEW ---")
                sb.AppendLine("  " & sqlNew)
            End If

            SyncLock _lock
                File.AppendAllText(GetLogPath(), sb.ToString())
            End SyncLock
        Catch
            ' Mai far fallire il login per problemi di logging.
        End Try
    End Sub

    Private Shared Function GetLogPath() As String
        Return Path.Combine(Path.GetTempPath(), "AgronicaVisibilitaDiff.log")
    End Function

    Private Shared Function NormalizzaSql(sql As String) As String
        If String.IsNullOrWhiteSpace(sql) Then Return ""
        Return _whitespace.Replace(sql, " ").Trim()
    End Function

End Class
