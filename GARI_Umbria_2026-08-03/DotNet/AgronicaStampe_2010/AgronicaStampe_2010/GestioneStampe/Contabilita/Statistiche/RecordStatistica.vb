Public Class RecordStatistica

    Public Piva As String
    Public Anno_Movimento As String
    Public TipoRecord As Integer
    Public Esercizio As String
    Public UnitaMisura As String
    Public CodLiv1 As String
    Public DesLiv1 As String
    Public CodLiv2 As String
    Public DesLiv2 As String
    Public CodLiv3 As String
    Public DesLiv3 As String
    Public CodDet As String
    Public DesDet As String

    Public Val_01 As Decimal
    Public Val_02 As Decimal
    Public Val_03 As Decimal
    Public Val_04 As Decimal
    Public Val_05 As Decimal
    Public Val_06 As Decimal
    Public Val_07 As Decimal
    Public Val_08 As Decimal
    Public Val_09 As Decimal
    Public Val_10 As Decimal
    Public Val_11 As Decimal
    Public Val_12 As Decimal

    Public ValC_01 As Decimal
    Public ValC_02 As Decimal
    Public ValC_03 As Decimal
    Public ValC_04 As Decimal
    Public ValC_05 As Decimal
    Public ValC_06 As Decimal
    Public ValC_07 As Decimal
    Public ValC_08 As Decimal
    Public ValC_09 As Decimal
    Public ValC_10 As Decimal
    Public ValC_11 As Decimal
    Public ValC_12 As Decimal

    Public TotaleLivello1 As Decimal
    Public TotaleLivello2 As Decimal
    Public ToatleLivello3 As Decimal
    Public Totale As Decimal
    Public TotalePerPeriodoAnnoRif As Decimal
    Public Totale12MesiAnnoRif As Decimal
    Public TotalePerPeriodoAnnoPrec As Decimal
    Public Totale12MesiAnnoPrec As Decimal


    Public Key As String

    Public Function GetKeyWithUdm() As String
        Return ToString()
    End Function

    Public Function GetKeyWithoutUdm() As String
        Return String.Format("{0}{1}{2}{3}{4}", Piva, CodLiv1, CodLiv2, CodLiv3, CodDet)
    End Function

    Public Overrides Function ToString() As String
        Return String.Format("{0}{1}{2}{3}{4}{5}", Piva, CodLiv1, CodLiv2, CodLiv3, CodDet, UnitaMisura)
    End Function

    Public Function TotaleRecord() As Decimal
        Return Val_01 + Val_02 + Val_03 + Val_04 + Val_05 + Val_06 + Val_07 + Val_08 + Val_09 + Val_10 + Val_11 + Val_12
    End Function

End Class

