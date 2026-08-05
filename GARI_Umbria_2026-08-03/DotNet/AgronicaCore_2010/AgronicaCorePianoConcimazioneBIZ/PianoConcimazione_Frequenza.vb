Public Class PianoConcimazione_Frequenza

    Public Sub New()

    End Sub

    Public Function Frequenza(ByVal Input As PianoConcimazione_Frequenza_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_Frequenza_output

        Dim Output As New PianoConcimazione_Frequenza_output
        Dim Elemento As Frequenza

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_Frequenza_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           0, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New Frequenza
            Elemento.Descrizione = Dt.Rows(i).Item("Frequenza_Des")
            Elemento.Codice = Dt.Rows(i).Item("Id_Fre")
            Output.ListaFrequenza.Add(Elemento)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_Frequenza_input

    Public Regolamento_Cod As Integer
    Public Url As String

    Sub New()

    End Sub

End Class

Public Class PianoConcimazione_Frequenza_output

    Public ListaFrequenza As List(Of Frequenza)

    Public MessaggioErrore As String

    Public Sub New()

        ListaFrequenza = New List(Of Frequenza)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Frequenza

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class


