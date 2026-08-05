Public Class PUA_CoefficienteB_Coltura

    Public Sub New()

    End Sub

    Public Function Leggi(ByVal Input As PUA_CoefficienteB_Coltura_input,
                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                  ) As PUA_CoefficienteB_Coltura_output

        Dim Output As New PUA_CoefficienteB_Coltura_output
        Dim PUACoefficienteB As PUA_CoefficienteB

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.GruppoFinalitaxSpeciexEpoca_R

            Dim Filtro As String = ""
            If Not IsNothing(Input.Veg_Cod_Elenco) AndAlso Input.Veg_Cod_Elenco <> "" Then
                Filtro = " GruppoFinalitaxSpeciexEpoca.veg_cod IN (" & Input.Veg_Cod_Elenco & ") "
            End If

            Dim dt As DataTable

            Dim Includi_NMasResa As Boolean = False
            If Not IsNothing(Input.Includi_NMasResa) Then
                Includi_NMasResa = Input.Includi_NMasResa
            End If

            If Includi_NMasResa = True Then
                dt = objCore.Leggi_conLimitiAzotoxSpecie(Input.Regolamento_Cod, 0, Input.Veg_Cod, 0, Input.Grfi_Cod,
                                                Filtro, "", objParametri)
            Else
                dt = objCore.Leggi(Input.Regolamento_Cod, 0, Input.Veg_Cod, 0, Input.Grfi_Cod,
                                                Filtro, "", objParametri)
            End If

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then

                For Each row In dt.Rows

                    Dim N As Decimal = 0
                    Dim Resa As Decimal = 0
                    Dim FattoreCorrettivo_N As Decimal = 0

                    If Includi_NMasResa = True Then
                        If Not IsDBNull(row.Item("N")) Then
                            N = row.Item("N")
                        End If
                        If Not IsDBNull(row.Item("Resa")) Then
                            Resa = row.Item("Resa")
                        End If
                        If Not IsDBNull(row.Item("FattoreCorrettivo_N")) Then
                            FattoreCorrettivo_N = row.Item("FattoreCorrettivo_N")
                        End If
                    End If

                    PUACoefficienteB = New PUA_CoefficienteB With {
                                                .Regolamento_Cod = row.Item("Regolamento_Cod"),
                                                .Veg_Cod = row.Item("Veg_Cod"),
                                                .Grfi_Cod_RER = row.Item("Grfi_Cod"),
                                                .Grfi_Des_RER = row.Item("Grfi_Des"),
                                                .B_Perc = row.Item("B_Perc"),
                                                .Base = row.Item("Base"),
                                                .Coeff_Incr = row.Item("Coeff_Incr"),
                                                .N = N,
                                                .Resa = Resa,
                                                .FattoreCorrettivo_N = FattoreCorrettivo_N
                                            }

                    Output.ListaCoefficienteB.Add(PUACoefficienteB)

                Next

            End If


        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function


End Class

Public Class PUA_CoefficienteB_Coltura_input

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Veg_Cod_Elenco As String
    Public Grfi_Cod As Integer
    Public Url As String

    Public Includi_NMasResa As Boolean

    Public Sub New()
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
        Url = ""
        Includi_NMasResa = False
    End Sub

End Class

Public Class PUA_CoefficienteB_Coltura_output

    Public ListaCoefficienteB As List(Of PUA_CoefficienteB)
    Public MessaggioErrore As String

    Public Sub New()
        ListaCoefficienteB = New List(Of PUA_CoefficienteB)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_CoefficienteB

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Grfi_Cod_RER As Integer
    Public Grfi_Des_RER As String
    Public B_Perc As Decimal
    Public Base As Decimal
    Public Coeff_Incr As Decimal

    Public N As Decimal
    Public Resa As Decimal
    Public FattoreCorrettivo_N As Decimal



    Public Sub New()
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod_RER = 0
        Grfi_Des_RER = ""
        B_Perc = 0
        Base = 0
        Coeff_Incr = 0
        N = 0
        Resa = 0
        FattoreCorrettivo_N = 0
    End Sub

End Class




