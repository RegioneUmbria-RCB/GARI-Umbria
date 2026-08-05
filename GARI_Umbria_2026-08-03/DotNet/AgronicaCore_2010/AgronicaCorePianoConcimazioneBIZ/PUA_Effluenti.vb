Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class PUA_Effluenti

    Public Sub New()

    End Sub

    Public Function Effluenti(Input As PUA_Effluenti_input, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As PUA_Effluenti_output
        Dim Output As New PUA_Effluenti_output
        Dim Effluente As PUA_Effluente
        Try
            Dim objCore As New AgronicaCoreMetaSchemaDAL.Effluenti
            Dim dt As DataTable = objCore.Leggi(Input.Eff_Cod, Input.Regolamento_Cod, objParametri)

            Dim dtEffRif As DataTable
            Dim Eff_Rif As Decimal
            If Not IsNothing(Input.Includi_Efficienza_Rif) AndAlso Input.Includi_Efficienza_Rif = True Then
                Dim objEffRif As New AgronicaCoreMetaSchemaDAL.EfficienzaRiferimentoxTipiAllevamentixEffluenti_R
                dtEffRif = objEffRif.Leggi(-1, 0, Input.Regolamento_Cod, objParametri)
            End If

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For Each row In dt.Rows
                    Eff_Rif = 0
                    If Not IsNothing(Input.Includi_Efficienza_Rif) AndAlso Input.Includi_Efficienza_Rif = True AndAlso Not IsNothing(dtEffRif) AndAlso dtEffRif.Rows.Count > 0 Then
                        Dim DrEffRif() As DataRow
                        DrEffRif = dtEffRif.Select("eff_cod=" & row.Item("Eff_Cod"))
                        If Not DrEffRif Is Nothing AndAlso DrEffRif.Length > 0 Then
                            Eff_Rif = DrEffRif(0).Item("Efficienza_Val")
                        End If
                    End If

                    Effluente = New PUA_Effluente With {
                        .Regolamento_Cod = row.Item("Regolamento_Cod"),
                        .Eff_Des = row.Item("Eff_Des"),
                        .Eff_Cod = row.Item("Eff_Cod"),
                        .Tipo_Eff_Cod = row.Item("Tipo_Eff_Cod"),
                        .Tipo_Eff_Des = row.Item("Tipo_Eff_Des"),
                        .SpecieAllevamento = row.Item("SpecieAllevamento"),
                        .MatricePrevalente = row.Item("MatricePrevalente"),
                        .Fer_Cod = row.Item("Fer_Cod"),
                        .Fer_Des = row.Item("Fer_Des"),
                        .N = row.Item("N"),
                        .P2O5 = row.Item("P2O5"),
                        .K2O = row.Item("K2O"),
                        .MgO = row.Item("MgO"),
                        .Cu = row.Item("Cu"),
                        .CuPeso = row.Item("CuPeso"),
                        .Id_tp_fer = row.Item("id_tp_fer"),
                        .Id_tp_fer_des = row.Item("descrizione"),
                        .Udm_Sim = row.Item("Udm_Sim"),
                        .Udm_Cod = row.Item("Udm_Cod"),
                        .Efficienza_Rif = Eff_Rif
                    }
                    Output.ListaEffluenti.Add(Effluente)
                Next
            End If

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output
    End Function

      Public Function EffluentiDivietoSpandimenti(Input As PUA_Effluenti_input, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As PUA_Effluenti_output
        Dim Output As New PUA_Effluenti_output
        Dim Effluente As PUA_Effluente
        Try
            Dim objCore As New AgronicaCoreMetaSchemaDAL.Effluenti
            Dim dt As DataTable = objCore.LeggiDivietoSpandimenti(Input.Eff_Cod, Input.Regolamento_Cod, objParametri)

            If Not dt Is Nothing AndAlso dt.Rows.Count > 0 Then
                For Each row In dt.Rows
                    Effluente = New PUA_Effluente With {
                        .Regolamento_Cod = row.Item("Regolamento_Cod"),
                        .Eff_Des = row.Item("Eff_Div_Des"),
                        .Eff_Cod = row.Item("Eff_Div_Cod")
                    }
                    Output.ListaEffluenti.Add(Effluente)
                Next
            End If
        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output
    End Function

    Public Function EffluentiXFrequenza(ByVal Input As PUA_EffluentiXFrequenza_input,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                                   As PUA_EffluentiXFrequenza_output

        Dim Output As New PUA_EffluentiXFrequenza_output
        Dim Elemento As PUA_EffluentiXFrequenza

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_EffluentiXFrequenza_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           Input.Eff_Cod, Input.ID_Fre,
                           "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New PUA_EffluentiXFrequenza
            Elemento.Eff_Des = Dt.Rows(i).Item("Eff_Des")
            Elemento.Frequenza_Des = Dt.Rows(i).Item("Frequenza_Des")
            Elemento.Eff_Cod = Dt.Rows(i).Item("Eff_Cod")
            Elemento.Id_Fre = Dt.Rows(i).Item("ID_Fre")
            If Not IsDBNull(Dt.Rows(i).Item("N")) Then
                Elemento.N = Dt.Rows(i).Item("N")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("udm_cod")) Then
                Elemento.Udm_Cod = Dt.Rows(i).Item("Udm_Cod")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("udm_sim")) Then
                Elemento.Udm_Sim = Dt.Rows(i).Item("udm_sim")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("n_titolo")) Then
                Elemento.N_Titolo = Dt.Rows(i).Item("n_titolo")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("Fer_Cod")) Then
                Elemento.Fer_Cod = Dt.Rows(i).Item("Fer_Cod")
            End If
            If Not IsDBNull(Dt.Rows(i).Item("Fer_Des")) Then
                Elemento.Fer_Des = Dt.Rows(i).Item("Fer_Des")
            End If
            Output.ListaEffluentiXFrequenza.Add(Elemento)

        Next

        Return Output

    End Function


End Class

Public Class PUA_Effluenti_input

    Public Regolamento_Cod As Integer
    Public Eff_Cod As Integer
    Public Tipo_Eff_Cod As Integer
    Public Includi_Efficienza_Rif As Boolean

    Public Sub New()
        Regolamento_Cod = 0
        Eff_Cod = 0
        Tipo_Eff_Cod = 0
        Includi_Efficienza_Rif = False
    End Sub

End Class

Public Class PUA_Effluenti_output

    Public ListaEffluenti As List(Of PUA_Effluente)
    Public MessaggioErrore As String

    Public Sub New()
        ListaEffluenti = New List(Of PUA_Effluente)
        MessaggioErrore = ""
    End Sub

End Class

Public Class PUA_Effluente

    Public Regolamento_Cod As Integer
    Public Eff_Des As String
    Public Eff_Cod As Integer
    Public Tipo_Eff_Cod As Integer
    Public Tipo_Eff_Des As String
    Public SpecieAllevamento As Integer
    Public MatricePrevalente As Integer
    Public Fer_Des As String
    Public Fer_Cod As Integer
    Public N As Decimal
    Public P2O5 As Decimal
    Public K2O As Decimal
    Public MgO As Decimal
    Public Cu As Decimal
    Public CuPeso As Decimal
    Public Id_tp_fer As Integer
    Public Id_tp_fer_des As String
    Public Udm_Sim As String
    Public Udm_Cod As Integer
    Public Efficienza_Rif As Decimal

    Public Sub New()
        Regolamento_Cod = 0
        Eff_Des = ""
        Eff_Cod = 0
        Tipo_Eff_Cod = 0
        Tipo_Eff_Des = ""
        SpecieAllevamento = 0
        MatricePrevalente = 0
        Fer_Cod = 0
        Fer_Des = ""
        N = 0
        P2O5 = 0
        K2O = 0
        MgO = 0
        Cu = 0
        CuPeso = 0
        Id_tp_fer = 0
        Id_tp_fer_des = ""
        Udm_Sim = ""
        Udm_Cod = 0
        Efficienza_Rif = 0
    End Sub

End Class


Public Class PUA_EffluentiXFrequenza_input

    Public Regolamento_Cod As Integer
    Public Eff_Cod As Integer
    Public ID_Fre As Integer

    Public Url As String

    Sub New()


    End Sub

End Class

Public Class PUA_EffluentiXFrequenza_output

    Public ListaEffluentiXFrequenza As List(Of PUA_EffluentiXFrequenza)

    Public MessaggioErrore As String

    Public Sub New()

        ListaEffluentiXFrequenza = New List(Of PUA_EffluentiXFrequenza)
        MessaggioErrore = ""

    End Sub

End Class

Public Class PUA_EffluentiXFrequenza

    Public Eff_Des As String
    Public Frequenza_Des As String
    Public Eff_Cod As Integer
    Public Id_Fre As Integer
    Public N As Decimal
    Public Udm_Sim As String
    Public Udm_Cod As Integer
    Public N_Titolo As Decimal
    Public Fer_Cod As Integer
    Public Fer_Des As String

    Sub New()

        Eff_Des = ""
        Frequenza_Des = ""
        Eff_Cod = 0
        Id_Fre = 0
        N = 0
        Udm_Sim = ""
        Udm_Cod = 0
        N_Titolo = 0
        Fer_Cod = 0
        Fer_Des = ""

    End Sub

End Class

Public Class PUA_Effluente_Aziendale

    Public Regolamento_Cod As Integer
    Public Pua_Cod As Integer
    Public Eff_Cod As Integer
    Public Carico As Decimal
    Public Capacita_stoccaggio As Decimal
    Public Giorni_stoccaggio As Integer
    Public Riempimento As Decimal
    Public Azoto_Qta As Decimal
    Public Azoto_Titoli As Decimal
    Public Tipo_Allevamento As Integer
    Public Perc_Zootecnico As Decimal
    Public Matrice_Prevalente As Integer
    Public Flag_ProvenienzaEsterna As Integer
    Public Data_Inizio_Divieto As Date
    Public Data_Fine_Divieto As Date

    Public Eff_Des As String
    Public Fer_Cod As Integer
    Public Fer_Des As String
    Public Udm_Cod As Integer
    Public Udm_Sim As String

    Public ListaGiacenza As List(Of PUA_Effluente_Aziendale_Giacenza)

    Public Sub New()

        Regolamento_Cod = 0
        Pua_Cod = 0
        Eff_Cod = 0
        Carico = 0
        Capacita_stoccaggio = 0
        Giorni_stoccaggio = 0
        Riempimento = 0
        Azoto_Qta = 0
        Azoto_Titoli = 0
        Tipo_Allevamento = 0
        Perc_Zootecnico = 0
        Matrice_Prevalente = 0
        Flag_ProvenienzaEsterna = 0
        Data_Inizio_Divieto = AGRODATAINIZIO
        Data_Fine_Divieto = AGRODATAFINE

        Eff_Des = ""
        Fer_Cod = 0
        Fer_Des = ""
        Udm_Cod = 0
        Udm_Sim = ""

    End Sub

End Class

Public Class PUA_Effluente_Aziendale_Giacenza

    Public Data As Date
    Public Qta_Giacenza As Decimal
    Public Udm_Cod_Giacenza As Integer
    Public Qta_Stimata As Decimal
    Public Udm_Cod_Stimata As Integer
    Public Sub New()

        Data = AGRODATAINIZIO
        Qta_Giacenza = 0
        Udm_Cod_Giacenza = 0
        Qta_Stimata = 0
        Udm_Cod_Stimata = 0

    End Sub

End Class


