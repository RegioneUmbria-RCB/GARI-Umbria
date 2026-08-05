Public Class PianoConcimazione_FasiCicloColturale

    Public Sub New()

    End Sub

    Public Function FasiCicloColturale(ByVal FasiInput As PianoConcimazione_FasiCicloColturale_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_FasiCicloColturale_output

        Dim FasiOutput As New PianoConcimazione_FasiCicloColturale_output
        Dim Fase As Fase

        Dim Dt As DataTable
        Dim objFasi As New AgronicaCoreMetaSchemaDAL.PC_FasiCicloColturalexGruppoVegetale_R

        Dt = objFasi.Leggi(FasiInput.Regolamento_Cod,
                           FasiInput.Veg_Cod,
                            "",
                            "",
                            objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Fase = New Fase
            Fase.Descrizione = Dt.Rows(i).Item("Fase_Des")
            Fase.Codice = Dt.Rows(i).Item("Id_Fase")
            FasiOutput.ListaFasi.Add(Fase)

        Next

        Return FasiOutput

    End Function

    Public Function FasiCicloColturale_StatoImpianto(ByVal FasiInput As PianoConcimazione_FasiCicloColturale_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_FasiCicloColturale_output

        Dim FasiOutput As New PianoConcimazione_FasiCicloColturale_output
        Dim Fase As Fase

        Dim Dt As DataTable
        Dim objFasi As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

        Dt = objFasi.FasiCicloColturale_Leggi(FasiInput.Veg_Cod,
                                              0,
                                              FasiInput.Regolamento_Cod,
                                                "", "", objParametri)

        Dim HashGrfiCod As New Hashtable

        For i = 0 To Dt.Rows.Count - 1

            If Not HashGrfiCod.ContainsKey(Dt.Rows(i).Item("grfi_cod")) Then
                HashGrfiCod.Add(Dt.Rows(i).Item("grfi_cod"), "")
                Fase = New Fase
                Fase.Descrizione = Dt.Rows(i).Item("grfi_Des")
                Fase.Codice = Dt.Rows(i).Item("grfi_cod")
                FasiOutput.ListaFasi.Add(Fase)
            End If

        Next

        Return FasiOutput

    End Function

End Class



Public Class PianoConcimazione_FasiCicloColturale_input

    Public Regolamento_Cod As Integer

    Public Veg_Cod As Integer

    Public Url As String


    Sub New()


    End Sub

End Class

Public Class PianoConcimazione_FasiCicloColturale_output

    Public ListaFasi As List(Of Fase)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFasi = New List(Of Fase)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Fase

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class
