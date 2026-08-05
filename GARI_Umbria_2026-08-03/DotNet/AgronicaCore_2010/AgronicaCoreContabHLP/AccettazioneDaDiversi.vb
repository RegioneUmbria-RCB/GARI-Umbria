Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AccettazioneDaDiversi

    '##################################################################################
    Public Function Ricava_PrefissoBolleAccettazione_FRG(ByVal Descr_Magazzino As String) As String

        Dim Prefisso_FRG As String
        Dim Codice_Alfonsine1_Larino7 As Integer

        Codice_Alfonsine1_Larino7 = CodStabilimentoFRG_from_DescStabilimentoFRG(Descr_Magazzino)

        Prefisso_FRG = Right(CStr(Date.Now.Year), 2) & CStr(Codice_Alfonsine1_Larino7)

        Return Prefisso_FRG

    End Function

    '##################################################################################
    Public Function CodStabilimentoFRG_from_DescStabilimentoFRG(ByVal Descr_Magazzino As String) As Integer

        If InStr(Descr_Magazzino.ToLower, "larino") > 0 Then
            Return 7
        Else
            Return 1
        End If

    End Function

    '###############################################################################################
    Public Sub Leggi_ChiaveMagazzino_Default(ByRef Piva As String, _
                                                ByRef Sa_Cod As Integer, _
                                                ByRef Fabbricato_Cod As Integer, _
                                                ByVal Impostazione_Valore_1 As String)

        If Impostazione_Valore_1 <> "" AndAlso InStr(Impostazione_Valore_1, "|") > 0 Then

            Piva = Impostazione_Valore_1.Split({"|"c})(0)
            Sa_Cod = CInt(Impostazione_Valore_1.Split({"|"c})(1))
            Fabbricato_Cod = CInt(Impostazione_Valore_1.Split({"|"c})(2))

        End If

    End Sub


End Class
