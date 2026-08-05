Public Class PianoConcimazione_Ubicazione

    Public Sub New()

    End Sub

    Public Function Ubicazione(ByVal Input As PianoConcimazione_Ubicazione_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_Ubicazione_output

        Dim Output As New PianoConcimazione_Ubicazione_output
        Dim Elemento As Ubicazione

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_Ubicazione_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           Input.Codice, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New Ubicazione
            Elemento.Descrizione = Dt.Rows(i).Item("Ubicazione_Des")
            Elemento.Codice = Dt.Rows(i).Item("Ubicazione_Cod")
            If Not IsDBNull(Dt.Rows(i).Item("Deposizione_Anno")) Then
                Elemento.Deposizione_Anno = Dt.Rows(i).Item("Deposizione_Anno")
            End If
            Output.ListaUbicazione.Add(Elemento)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_Ubicazione_input

    Public Regolamento_Cod As Integer
    Public Codice As Integer
    Public Url As String

    Sub New()

        Codice = 0

    End Sub

End Class

Public Class PianoConcimazione_Ubicazione_output

    Public ListaUbicazione As List(Of Ubicazione)

    Public MessaggioErrore As String

    Public Sub New()

        ListaUbicazione = New List(Of Ubicazione)
        MessaggioErrore = ""

    End Sub

End Class

Public Class Ubicazione

    Public Descrizione As String
    Public Codice As Integer
    Public Deposizione_Anno As Decimal

    Sub New()

        Descrizione = ""
        Codice = 0
        Deposizione_Anno = 0

    End Sub

End Class
