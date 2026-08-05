Public Class PUA_EpocheModalita

    Public Sub New()

    End Sub

    Public Function EpocheModalita(ByVal Input As PUA_EpocheModalita_input,
                                   ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                   ) AS PUA_EpocheModalita_output

        Dim Output As New PUA_EpocheModalita_output
        Dim EpocaModalita As PUA_EpocaModalita

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.EpocheModalita_R
            Dim dt As DataTable = objCore.LeggiConEfficienza(Input.EM_Cod, Input.Id_Gru,
                                                             Input.Veg_Cod, Input.Regolamento_Cod,
                                                             "", "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    EpocaModalita = New PUA_EpocaModalita With {
                        .EM_Cod = row.Item("EM_Cod"),
                        .EM_Des = row.Item("EM_Des"),
                        .Id_Gru = row.Item("Id_Gru"),
                        .Efficienza_Cod = row.Item("Efficienza_Cod"),
                        .Efficienza_Des = row.Item("Efficienza_Des")
                    }

                    Output.ListaEpocheModalita.Add(EpocaModalita)

                Next

            End If

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

End Class

Public Class PUA_EpocheModalita_input

    Public Regolamento_Cod As Integer
    Public EM_Cod As Integer
    Public Id_Gru As Integer
    Public Veg_Cod As Integer

    Public Sub New()
        Regolamento_Cod = 0
        EM_Cod = 0
        Id_Gru = 0
        Veg_Cod = 0
    End Sub

End Class

Public Class PUA_EpocheModalita_output

    Public ListaEpocheModalita As List(Of PUA_EpocaModalita)
    Public MessaggioErrore As String

    Public Sub New()
        ListaEpocheModalita = New List(Of PUA_EpocaModalita)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_EpocaModalita

    Public EM_Cod As Integer
    Public EM_Des As String
    Public Id_Gru As Integer
    Public Efficienza_Cod As Integer
    Public Efficienza_Des As String

    Public Sub New()
        EM_Cod = 0
        EM_Des = ""
        Id_Gru = 0
        Efficienza_Cod = 0
        Efficienza_Des = ""
    End Sub

End Class
