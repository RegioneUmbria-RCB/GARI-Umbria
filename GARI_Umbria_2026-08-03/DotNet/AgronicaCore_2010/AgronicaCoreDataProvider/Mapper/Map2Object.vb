Imports System.Reflection

Public Class Map2Object
    Public Shared Function DataTableToObjectList(Of T)(ByVal dt As DataTable) As List(Of T)
        Dim retObj As New List(Of T)

        Try
            For Each row As DataRow In dt.Rows
                Dim newObj As T

                Dim constructor As ConstructorInfo = GetType(T).GetConstructor(New Type() {})
                newObj = CType(constructor.Invoke(New Object() {}), T)

                For Each column As DataColumn In dt.Columns
                    Dim prop = newObj.GetType().GetProperty(column.ToString)
                    If prop IsNot Nothing AndAlso prop.CanWrite Then

                        prop.SetValue(newObj, If(row(column.ToString) Is DBNull.Value, Nothing, row(column.ToString)))
                    End If
                Next
                retObj.Add(newObj)
            Next
        Catch ex As Exception
            retObj = Nothing

        End Try

        Return retObj

    End Function

End Class
