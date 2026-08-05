Imports System.ComponentModel
Imports System.Reflection

Public Class Oggetti_DatatableUtility


    Public Shared Function CreateDataTable(Of T)(data As List(Of T)) As DataTable

        Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(T))
        Dim table As New DataTable()

        For i As Integer = 0 To props.Count - 1
            Dim prop As PropertyDescriptor = props(i)
            table.Columns.Add(prop.Name, prop.PropertyType)
            Debug.Print("prop:" & prop.Name)
        Next

        Dim values As Object() = New Object(props.Count - 1) {}

        For Each item As T In data
            For i As Integer = 0 To values.Length - 1
                If props(i).PropertyType = GetType(DateTime) Then
                    Dim currDT As DateTime = DirectCast(props(i).GetValue(item), DateTime)
                    values(i) = currDT.ToUniversalTime()
                Else
                    values(i) = props(i).GetValue(item)
                End If
            Next
            table.Rows.Add(values)
        Next

        Return table

    End Function



End Class
