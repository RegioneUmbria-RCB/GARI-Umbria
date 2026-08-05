Public Class PianoConcimazione_MatriciOrganiche
    Public Sub New()

    End Sub

    Public Function MatriciOrganiche(ByVal Input As PianoConcimazione_MatriciOrganiche_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_MatriciOrganiche_output

        Dim Output As New PianoConcimazione_MatriciOrganiche_output
        Dim Elemento As MatriciOrganiche

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_MatriciOrganiche_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           Input.Codice, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New MatriciOrganiche
            Elemento.Descrizione = Dt.Rows(i).Item("Mat_O_Des")
            Elemento.Codice = Dt.Rows(i).Item("Id_Mat_O")
            Output.ListaMatriciOrganiche.Add(Elemento)

        Next

        Return Output

    End Function

    Public Function MatriciOrganicheXFrequenza(ByVal Input As PianoConcimazione_MatriciOrganicheXFrequenza_input,
                                                  ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                                   As PianoConcimazione_MatriciOrganicheXFrequenza_output

        Dim Output As New PianoConcimazione_MatriciOrganicheXFrequenza_output
        Dim Elemento As MatriciOrganicheXFrequenza

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_MatriciOrganicheXFrequenza_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           Input.Codice_MatriceOrganica, Input.Codice_Frequenza,
                           "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New MatriciOrganicheXFrequenza
            Elemento.Descrizione_MatriceOrganica = Dt.Rows(i).Item("Mat_O_Des")
            Elemento.Descrizione_Frequenza = Dt.Rows(i).Item("Frequenza_Des")
            Elemento.Codice_MatriceOrganica = Dt.Rows(i).Item("Id_Mat_O")
            Elemento.Codice_Frequenza = Dt.Rows(i).Item("ID_Fre")
            If Not IsDBNull(Dt.Rows(i).Item("N")) Then
                Elemento.N = Dt.Rows(i).Item("N")
            End If
            Output.ListaMatriciOrganicheXFrequenza.Add(Elemento)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_MatriciOrganiche_input

    Public Regolamento_Cod As Integer
    Public Codice As Integer

    Public Url As String

    Sub New()
        Codice = 0
    End Sub

End Class

Public Class PianoConcimazione_MatriciOrganiche_output

    Public ListaMatriciOrganiche As List(Of MatriciOrganiche)

    Public MessaggioErrore As String

    Public Sub New()

        ListaMatriciOrganiche = New List(Of MatriciOrganiche)
        MessaggioErrore = ""

    End Sub

End Class

Public Class MatriciOrganiche

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class

Public Class PianoConcimazione_MatriciOrganicheXFrequenza_input

    Public Regolamento_Cod As Integer
    Public Codice_MatriceOrganica As Integer
    Public Codice_Frequenza As Integer

    Public Url As String

    Sub New()



    End Sub

End Class

Public Class PianoConcimazione_MatriciOrganicheXFrequenza_output

    Public ListaMatriciOrganicheXFrequenza As List(Of MatriciOrganicheXFrequenza)

    Public MessaggioErrore As String

    Public Sub New()

        ListaMatriciOrganicheXFrequenza = New List(Of MatriciOrganicheXFrequenza)
        MessaggioErrore = ""

    End Sub

End Class

Public Class MatriciOrganicheXFrequenza

    Public Descrizione_MatriceOrganica As String
    Public Descrizione_Frequenza As String
    Public Codice_MatriceOrganica As Integer
    Public Codice_Frequenza As Integer
    Public N As Decimal


    Sub New()

        Descrizione_MatriceOrganica = ""
        Descrizione_Frequenza = ""
        Codice_MatriceOrganica = 0
        Codice_Frequenza = 0
        N = 0

    End Sub

End Class

