
Public Class Codifica_ImpiantiIrrigui_Cliente_Input
    Public Cli_Cod As Integer
End Class

Public Class Codifica_ImpiantiIrrigui_Cliente_Output
    Public Class Elemento
        Public GIAS_Imp_Cod As Integer
        Public Cli_Imp_Cod As Integer
        Public Cli_Imp_Des As String
    End Class

    Public Lista As List(Of Elemento)
    Public Sub New()
        Lista = New List(Of Elemento)
    End Sub
End Class


Public Class Codifica_ImpiantiIrrigui_Cliente_R

    Public Function LeggiXCliente(obj As Codifica_ImpiantiIrrigui_Cliente_Input, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Codifica_ImpiantiIrrigui_Cliente_Output

        Dim Output As New Codifica_ImpiantiIrrigui_Cliente_Output

        Dim objCore As New AgronicaCoreMetaSchemaDAL.Codifica_ImpiantiIrrigui_Clienti_R

        Dim Dt As DataTable = objCore.LeggixCliente(obj.Cli_Cod, objParametri)

        For Each dr As DataRow In Dt.Rows

            Dim el As New Codifica_ImpiantiIrrigui_Cliente_Output.Elemento With {
                .GIAS_Imp_Cod = dr("Imp_Cod"),
                .Cli_Imp_Cod = dr("Imp_Cod_Cliente"),
                .Cli_Imp_Des = dr("Imp_Des_Cliente")
            }

            Output.Lista.Add(el)
        Next

        Return Output
    End Function

End Class

