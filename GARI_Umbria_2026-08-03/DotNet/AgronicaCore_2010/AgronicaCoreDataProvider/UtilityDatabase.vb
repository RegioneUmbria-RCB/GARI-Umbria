Imports System.Runtime.CompilerServices
Imports System.Threading.Tasks

Public Class UtilityDatabase
    ''' <summary>
    ''' Ritorna la stringa del comando SQL per impostare il livello di isolamento richiesto
    ''' </summary>
    ''' <param name="IsolationLevel">Livello di isolamento richiesto</param>
    ''' <returns>Stringa comando SQL:
    ''' <code>SET TRANSACTION ISOLATION LEVEL { IsolationLevel }</code>
    ''' </returns>
    Public Shared Function SetTransactionIsolationLevel(IsolationLevel As IsolationLevel) As String

        Const nomeRoutine As String = "AgronicaCoreDataProvider.UtilityDatabase.SetTransactionIsolationLevel()"

        Const comandoSqlBase As String = "SET TRANSACTION ISOLATION LEVEL"

        Dim sqlIsolationLevel As String

        Try

            Select Case IsolationLevel

                Case IsolationLevel.ReadUncommitted
                    sqlIsolationLevel = "READ UNCOMMITTED"

                Case IsolationLevel.ReadCommitted
                    sqlIsolationLevel = "READ COMMITTED"

                Case IsolationLevel.RepeatableRead
                    sqlIsolationLevel = "REPEATABLE READ"

                Case IsolationLevel.Snapshot
                    sqlIsolationLevel = "SNAPSHOT"

                Case IsolationLevel.Serializable
                    sqlIsolationLevel = "SERIALIZABLE"

                Case Else
                    Throw New Exception("Isolation Level non gestito")

            End Select

        Catch ex As Exception
            Dim messaggioErrore = ex.Message
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return String.Format("{0} {1}", comandoSqlBase, sqlIsolationLevel)

    End Function

End Class

Public Module UtilityDatabaseExtension

    <Extension()>
    Public Function ToExpandoObject(ByVal dt As DataTable) As List(Of IDictionary(Of String, Object))

        Dim listaFinale As New List(Of IDictionary(Of String, Object))

        If Not IsNothing(dt) Then
            Dim colCount = dt.Columns.Count

            For Each r As DataRow In dt.Rows
                Dim objExpando As New System.Dynamic.ExpandoObject()
                Dim obj As IDictionary(Of String, Object) = objExpando

                For i As Integer = 0 To colCount - 1
                    Dim key = r.Table.Columns(i).ColumnName.ToString()
                    Dim val = r(key)
                    obj(key) = val
                Next

                listaFinale.Add(obj)
            Next
        End If

        Return listaFinale

    End Function

    <Extension()>
    Public Iterator Function ToExpandoObject2(ByVal dt As DataTable) As IEnumerable(Of IDictionary(Of String, Object))

        Dim colCount = dt.Columns.Count

        For Each r As DataRow In dt.AsEnumerable
            Dim objExpando As New System.Dynamic.ExpandoObject()
            Dim obj As IDictionary(Of String, Object) = objExpando

            For i As Integer = 0 To colCount - 1
                Dim key = r.Table.Columns(i).ColumnName.ToString()
                Dim val = r(key)
                obj(key) = val
            Next

            Yield obj

        Next

    End Function

    <Extension()>
    Public Function ToExpandoObjectCaseInsensitive(ByVal dt As DataTable) As List(Of IDictionary(Of String, Object))

        Dim colCount = dt.Columns.Count
        Dim columnsNameMapping As New Dictionary(Of String, String)

        For i As Integer = 0 To colCount - 1
            columnsNameMapping.Add(dt.Columns(i).ColumnName, dt.Columns(i).ColumnName.ToLowerInvariant)
        Next

        Return dt.AsEnumerable.AsParallel.AsOrdered().Select(
            Function(r As DataRow)
                Dim objExpando As New System.Dynamic.ExpandoObject()
                Dim obj As IDictionary(Of String, Object) = objExpando
                For i As Integer = 0 To colCount - 1
                    Dim colName As String = r.Table.Columns(i).ColumnName
                    Dim key = colName.ToLowerInvariant
                    Dim val = r(colName)
                    obj(key) = val
                Next
                Return obj
            End Function).ToList()

    End Function

    <Extension()>
    Public Function ToDataTable(ByVal expandoObject As List(Of IDictionary(Of String, Object))) As DataTable

        Dim dt As New DataTable

        For Each obj In expandoObject
            Dim row As DataRow = dt.NewRow()
            For Each kvp In obj

            Next
        Next

    End Function

    <Extension()>
    Public Function ToDataTable(ByVal listaOperazioni As List(Of IDictionary(Of String, Object)), ByVal dt As DataTable) As DataTable

        Dim retVal As DataTable = dt.Clone()

        For Each elemento In listaOperazioni

            Dim row = retVal.NewRow()
            For Each kvp As KeyValuePair(Of String, Object) In elemento
                row(kvp.Key) = kvp.Value
            Next
            retVal.Rows.Add(row)

        Next

        Return retVal

    End Function


End Module

