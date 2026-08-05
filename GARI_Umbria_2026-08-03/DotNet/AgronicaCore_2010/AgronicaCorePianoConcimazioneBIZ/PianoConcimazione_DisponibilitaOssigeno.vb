Public Class PianoConcimazione_DisponibilitaOssigeno

    Public Sub New()

    End Sub

    Public Function DisponibilitaOssigeno(ByVal Input As PianoConcimazione_DisponibilitaOssigeno_input,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) _
                                       As PianoConcimazione_DisponibilitaOssigeno_output

        Dim Output As New PianoConcimazione_DisponibilitaOssigeno_output
        Dim Elemento As DisponibilitaOssigeno

        Dim Dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_DisponibilitaOssigeno_R

        Dt = objCore.Leggi(Input.Regolamento_Cod,
                           0, "", "", objParametri)

        For i = 0 To Dt.Rows.Count - 1

            Elemento = New DisponibilitaOssigeno
            Elemento.Descrizione = Dt.Rows(i).Item("Descrizione")
            Elemento.Codice = Dt.Rows(i).Item("Id_Disp")
            Output.ListaDisponibilitaOssigeno.Add(Elemento)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_DisponibilitaOssigeno_input

    Public Regolamento_Cod As Integer

    Sub New()

    End Sub

End Class

Public Class PianoConcimazione_DisponibilitaOssigeno_output

    Public ListaDisponibilitaOssigeno As List(Of DisponibilitaOssigeno)

    Public MessaggioErrore As String

    Public Sub New()

        ListaDisponibilitaOssigeno = New List(Of DisponibilitaOssigeno)
        MessaggioErrore = ""

    End Sub

End Class

Public Class DisponibilitaOssigeno

    Public Descrizione As String
    Public Codice As Integer

    Sub New()

        Descrizione = ""
        Codice = 0

    End Sub

End Class

