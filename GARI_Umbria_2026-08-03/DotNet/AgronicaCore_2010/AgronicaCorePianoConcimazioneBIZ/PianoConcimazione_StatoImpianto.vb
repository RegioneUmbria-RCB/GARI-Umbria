Public Class PianoConcimazione_StatoImpianto

    Public Sub New()

    End Sub

    Public Function StatoImpianto(ByVal StatoInput As PianoConcimazione_StatoImpianto_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_StatoImpianto_output

        Dim StatoOutput As New PianoConcimazione_StatoImpianto_output
        Dim StatoImp As StatoImpianto

        Dim Dt As DataTable
        Dim objStati As New AgronicaCoreMetaSchemaDAL.GruppoFinalita_R

        Dt = objStati.FasiCicloColturale_Leggi(StatoInput.Veg_Cod, StatoInput.Grfi_Cod, StatoInput.Regolamento_Cod, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            StatoImp = New StatoImpianto
            StatoImp.Descrizione = Dt.Rows(i).Item("grfi_des")
            StatoImp.Codice = Dt.Rows(i).Item("grfi_cod")
            StatoOutput.ListaStati.Add(StatoImp)

        Next

        Return StatoOutput

    End Function

End Class

Public Class PianoConcimazione_StatoImpianto_input

    Public Veg_Cod As Integer

    Public Grfi_Cod As Integer

    Public Regolamento_Cod As Integer

    Sub New()


    End Sub

End Class

Public Class PianoConcimazione_StatoImpianto_output

    Public ListaStati As List(Of StatoImpianto)

    Public MessaggioErrore As String

    Public Sub New()

        ListaStati = New List(Of StatoImpianto)
        MessaggioErrore = ""

    End Sub

End Class

Public Class StatoImpianto

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class