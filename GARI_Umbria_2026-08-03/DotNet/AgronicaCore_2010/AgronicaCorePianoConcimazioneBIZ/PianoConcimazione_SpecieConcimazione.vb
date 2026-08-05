Public Class PianoConcimazione_SpecieConcimazione


    Public Sub New()

    End Sub

    Public Function SpecieConcimazione(ByVal Input As PianoConcimazione_SpecieConcimazione_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_SpecieConcimazione_output

        Dim Output As New PianoConcimazione_SpecieConcimazione_output
        Dim Specie As SpecieConcimazione

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_SpecieConcimazione_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           0, 0, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Specie = New SpecieConcimazione
            Specie.Descrizione = Dt.Rows(i).Item("veg_Des")
            Specie.Codice = Dt.Rows(i).Item("Veg_Cod")
            Specie.Id_Ciclo = Dt.Rows(i).Item("Id_Ciclo")
            Output.ListaSpecieConcimazione.Add(Specie)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_SpecieConcimazione_input

    Public Regolamento_Cod As Integer

    Sub New()


    End Sub

End Class

Public Class PianoConcimazione_SpecieConcimazione_output

    Public ListaSpecieConcimazione As List(Of SpecieConcimazione)

    Public MessaggioErrore As String

    Public Sub New()

        ListaSpecieConcimazione = New List(Of SpecieConcimazione)
        MessaggioErrore = ""

    End Sub

End Class

Public Class SpecieConcimazione

    Public Descrizione As String
    Public Codice As Integer
    Public Id_Ciclo As Integer

    Sub New()

        Descrizione = ""
        Codice = 0
        Id_Ciclo = 0

    End Sub



End Class
