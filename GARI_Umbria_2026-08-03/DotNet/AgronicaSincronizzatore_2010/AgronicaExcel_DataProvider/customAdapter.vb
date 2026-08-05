Imports System
Imports System.Data

Namespace DataObjectsHelpers

    Public Class CustomAdapter
        Inherits System.Data.Common.DbDataAdapter

        Public Function FillFromReader(ByRef DataTable As DataTable, ByVal dataReader As IDataReader) As Integer
            Return Me.Fill(DataTable, dataReader)
        End Function

        Protected Overrides Function CreateRowUpdatedEvent(ByVal a As DataRow, ByVal b As IDbCommand, ByVal c As StatementType, ByVal d As System.Data.Common.DataTableMapping) As System.Data.Common.RowUpdatedEventArgs
            Return CType(New EventArgs, System.Data.Common.RowUpdatedEventArgs)
        End Function

        Protected Overrides Function CreateRowUpdatingEvent(ByVal a As DataRow, ByVal b As IDbCommand, ByVal c As StatementType, ByVal d As System.Data.Common.DataTableMapping) As System.Data.Common.RowUpdatingEventArgs
            Return CType(New EventArgs, System.Data.Common.RowUpdatingEventArgs)
        End Function



    End Class






End Namespace
