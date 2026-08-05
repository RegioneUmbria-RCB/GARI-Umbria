Public Class PUA_ListaTipiAllevamenti

    Public Sub New()

    End Sub

    Public Function ListaTipiAllevamenti(ByVal Input As PUA_ListaTipiAllevamenti_input,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As PUA_ListaTipiAllevamenti_output

        Dim Output As New PUA_ListaTipiAllevamenti_output
        Dim TipoAllevamento As PUA_TipoAllevamento

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.Lista_Tipi_Allevamenti_R
            Dim Filtro As String = ""
            If Input.SoloSpecieAllevamento = True Then
                Filtro = " all_cod <> 8 "
            End If
            Dim dt As DataTable = objCore.Leggi(Input.All_Cod, 0, 0, Input.Regolamento_Cod,
                                                Filtro, "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    TipoAllevamento = New PUA_TipoAllevamento With {
                        .Regolamento_Cod = row.Item("Regolamento_Cod"),
                        .All_Des = row.Item("All_Des"),
                        .All_Cod = row.Item("All_Cod"),
                        .Gen_Cod = row.Item("Gen_Cod"),
                        .Spe_Cod = row.Item("Spe_Cod"),
                        .Efficienza_Min = row.Item("Efficienza_Min"),
                        .Categoria = row.Item("Categoria")
                    }

                    Output.ListaTipiAllevamento.Add(TipoAllevamento)

                Next

            End If

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

End Class

Public Class PUA_ListaTipiAllevamenti_input

    Public Regolamento_Cod As Integer
    Public All_Cod As Integer
    Public SoloSpecieAllevamento As Boolean

    Public Sub New()
        Regolamento_Cod = 0
        All_Cod = 0
        SoloSpecieAllevamento = False
    End Sub

End Class

Public Class PUA_ListaTipiAllevamenti_output

    Public ListaTipiAllevamento As List(Of PUA_TipoAllevamento)
    Public MessaggioErrore As String

    Public Sub New()
        ListaTipiAllevamento = New List(Of PUA_TipoAllevamento)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_TipoAllevamento

    Public Regolamento_Cod As Integer
    Public All_Cod As Integer
    Public All_Des As String
    Public Gen_Cod As Integer
    Public Spe_Cod As Integer
    Public Efficienza_Min As Decimal
    Public Categoria As String

    Public Sub New()
        Regolamento_Cod = 0
        All_Des = ""
        All_Cod = 0
        Gen_Cod = 0
        Spe_Cod = 0
        Efficienza_Min = 0
        Categoria = ""
    End Sub

End Class

