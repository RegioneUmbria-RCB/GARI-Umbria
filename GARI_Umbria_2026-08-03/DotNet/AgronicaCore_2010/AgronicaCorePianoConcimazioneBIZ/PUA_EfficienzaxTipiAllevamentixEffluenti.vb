Public Class PUA_EfficienzaxTipiAllevamentixEffluenti

    Public Sub New()

    End Sub

    Public Function EfficienzaxTipiAllevamentixEffluenti(ByVal Input As PUA_EfficienzaxTipiAllevamentixEffluenti_input,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As PUA_EfficienzaxTipiAllevamentixEffluenti_output

        Dim Output As New PUA_EfficienzaxTipiAllevamentixEffluenti_output
        Dim PUAEfficienza As PUA_Efficienza

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.EfficienzaxTipiAllevamentixEffluenti_R
            Dim dt As DataTable = objCore.Leggi(Input.All_Cod, Input.Em_Cod, Input.Efficienza_Cod,
                                                         Input.Eff_Cod, Input.Dose, Input.Regolamento_Cod, Input.Id_ClasseTessitura,
                                                         objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    PUAEfficienza = New PUA_Efficienza With {
                        .Regolamento_Cod = row.Item("Regolamento_Cod"),
                        .Efficienza_Cod = row.Item("Efficienza_Cod"),
                        .All_Cod = row.Item("All_Cod"),
                        .Eff_Cod = row.Item("Eff_Cod"),
                        .Dose = row.Item("Dose"),
                        .Id_ClasseTessitura = row.Item("Id_ClasseTessitura"),
                        .Efficienza_Val = row.Item("Efficienza_Val"),
                        .Considera_CoefficienteTempo_Efficienza = row.Item("Considera_CoefficienteTempo_Efficienza")
                    }

                    Output.ListaEfficienzaxTipiAllevamentixEffluenti.Add(PUAEfficienza)

                Next

            End If


        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

End Class

Public Class PUA_EfficienzaxTipiAllevamentixEffluenti_input

    Public Regolamento_Cod As Integer
    Public Efficienza_Cod As Integer
    Public Em_Cod As Integer
    Public All_Cod As Integer
    Public Eff_Cod As Integer
    Public Dose As Integer
    Public Id_ClasseTessitura As Integer
    Public Efficienza_Val As Decimal
    Public Url As String

    Public Sub New()
        Regolamento_Cod = 0
        Efficienza_Cod = 0
        Em_Cod = 0
        All_Cod = 0
        Eff_Cod = 0
        Dose = 0
        Id_ClasseTessitura = 0
        Efficienza_Val = 0
        Url = ""
    End Sub

End Class

Public Class PUA_EfficienzaxTipiAllevamentixEffluenti_output

    Public ListaEfficienzaxTipiAllevamentixEffluenti As List(Of PUA_Efficienza)
    Public MessaggioErrore As String

    Public Sub New()
        ListaEfficienzaxTipiAllevamentixEffluenti = New List(Of PUA_Efficienza)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_Efficienza

    Public Regolamento_Cod As Integer
    Public Efficienza_Cod As Integer
    Public All_Cod As Integer
    Public Eff_Cod As Integer
    Public Dose As Integer
    Public Id_ClasseTessitura As Integer
    Public Efficienza_Val As Decimal
    Public Considera_CoefficienteTempo_Efficienza As Integer

    Public Sub New()
        Regolamento_Cod = 0
        Efficienza_Cod = 0
        All_Cod = 0
        Eff_Cod = 0
        Dose = 0
        Id_ClasseTessitura = 0
        Efficienza_Val = 0
        Considera_CoefficienteTempo_Efficienza = 0
    End Sub

End Class



