Public Class PUA_CoefficienteTempo_Coltura

    Public Sub New()

    End Sub

    Public Function Leggi(ByVal Input As PUA_CoefficienteTempo_Coltura_input,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As PUA_CoefficienteTempo_Coltura_output

        Dim Output As New PUA_CoefficienteTempo_Coltura_output
        Dim PUACoefficienteTempo As PUA_CoefficienteTempo

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpeciexEpoca_R

            Dim Filtro As String = ""
            If Not IsNothing(Input.Veg_Cod_Elenco) AndAlso Input.Veg_Cod_Elenco <> "" Then
                Filtro = " GruppoFinalitaxSpeciexEpoca.veg_cod IN (" & Input.Veg_Cod_Elenco & ") "
            End If

            Dim dt As DataTable = objCore.Leggi(Input.Regolamento_Cod, 0, Input.Veg_Cod, 0, Input.Grfi_Cod,
                                                Filtro, "", objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    PUACoefficienteTempo = New PUA_CoefficienteTempo With {
                        .Regolamento_Cod = row.Item("Regolamento_Cod"),
                        .Veg_Cod = row.Item("Veg_Cod"),
                        .Grfi_Cod_RER = row.Item("Grfi_Cod"),
                        .Coeff_Temp = row.Item("Coeff_Temp")
                    }

                    Output.ListaCoefficienteTempo.Add(PUACoefficienteTempo)

                Next

            End If


        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

End Class

Public Class PUA_CoefficienteTempo_Coltura_input

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Veg_Cod_Elenco As String
    Public Grfi_Cod As Integer
    Public Url As String

    Public Sub New()
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
    End Sub

End Class

Public Class PUA_CoefficienteTempo_Coltura_output

    Public ListaCoefficienteTempo As List(Of PUA_CoefficienteTempo)
    Public MessaggioErrore As String

    Public Sub New()
        ListaCoefficienteTempo = New List(Of PUA_CoefficienteTempo)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_CoefficienteTempo

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Grfi_Cod_RER As Integer
    Public Coeff_Temp As Decimal
    'Public Base As Decimal
    'Public Coeff_Incr As Decimal

    Public Sub New()
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod_RER = 0
        Coeff_Temp = 0
        'Base = 0
        'Coeff_Incr = 0
    End Sub

End Class



