
Public Class Codifica_SpecieVegetali_Input
    Public Cli_Cod As Integer
    Public Veg_Cod As Integer
    Public Cul_Cod As Integer
    Public Sub New()
        Cli_Cod = 0
        Veg_Cod = 0
        Cul_Cod = 0
    End Sub
End Class

Public Class Codifica_SpecieVegetali_Output
    Public Class Elemento
        Public Veg_Cod As Integer
        Public Cul_Cod As Integer
        Public Veg_Des As String
    End Class

    Public Lista As List(Of Elemento)
    Public Sub New()
        Lista = New List(Of Elemento)
    End Sub
End Class


Public Class Codifica_SpecieVegetaliCliente_Input
    Public Cli_Cod As Integer
End Class

Public Class Codifica_SpecieVegetaliCliente_Output
    Public Class Elemento
        Public GIAS_Veg_Cod As Integer
        Public GIAS_Cul_Cod As Integer
        Public GIAS_Grfi_Cod As Integer
        Public GIAS_Grva_Cod As Integer
        Public Cli_Veg_Cod As Integer
        Public Cli_Cul_Cod As Integer
        Public Cli_Veg_Des As String
    End Class

    Public Lista As List(Of Elemento)
    Public Sub New()
        Lista = New List(Of Elemento)
    End Sub
End Class

Public Class Codifica_SpecieVegetali_R
    Public Function Leggi(obj As Codifica_SpecieVegetali_Input, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Codifica_SpecieVegetali_Output

        Dim Output As New Codifica_SpecieVegetali_Output

        Dim objCore As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R

        Dim Dt As DataTable = objCore.LeggixCliVegCul(obj.Cli_Cod, obj.Veg_Cod, obj.Cul_Cod, objParametri)

        For Each dr As DataRow In Dt.Rows

            Dim el As New Codifica_SpecieVegetali_Output.Elemento
            el.Veg_Cod = dr("Veg_Cod_Cliente")
            el.Cul_Cod = dr("Cul_Cod_Cliente")
            el.Veg_Des = dr("Veg_Des_Cliente")

            Output.Lista.Add(el)

        Next

        Return Output
    End Function

    Public Function LeggiXCliente(obj As Codifica_SpecieVegetaliCliente_Input, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Codifica_SpecieVegetaliCliente_Output

        Dim Output As New Codifica_SpecieVegetaliCliente_Output

        Dim objCore As New AgronicaCoreMetaSchemaDAL.Codifica_SpecieVegetali_Clienti_R

        Dim Dt As DataTable = objCore.LeggixCliente(obj.Cli_Cod, objParametri)

        For Each dr As DataRow In Dt.Rows

            Dim el As New Codifica_SpecieVegetaliCliente_Output.Elemento With {
                .GIAS_Veg_Cod = dr("Veg_Cod"),
                .GIAS_Cul_Cod = dr("Cul_Cod"),
                .GIAS_Grfi_Cod = dr("Grfi_Cod"),
                .GIAS_Grva_Cod = dr("Grva_Cod"),
                .Cli_Veg_Cod = dr("Veg_Cod_Cliente"),
                .Cli_Cul_Cod = dr("Cul_Cod_Cliente"),
                .Cli_Veg_Des = dr("Veg_Des_Cliente")
            }

            Output.Lista.Add(el)
        Next

        Return Output
    End Function
End Class