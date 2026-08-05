Public Class PianoConcimazione_TipoAcqua

    Public Sub New()

    End Sub

    Public Function TipoAcqua(ByVal Input As PianoConcimazione_TipoAcqua_input,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                              ) As PianoConcimazione_TipoAcqua_output

        Dim Output As New PianoConcimazione_TipoAcqua_output
        Dim acqua As TipoAcqua

        Dim dt As DataTable
        Dim objCore As New AgronicaCoreMetaSchemaDAL.PC_TipoAcqua_R

        dt = objCore.Leggi(Input.Regolamento_Cod, Input.TipoZona, Input.Codice,
                           "", "", objParametri)

        For i = 0 To dt.Rows.Count - 1

            acqua = New TipoAcqua With {
                .Codice = dt.Rows(i).Item("TipoAcqua_Cod"),
                .Descrizione = dt.Rows(i).Item("TipoAcqua_Des"),
                .TipoZona = dt.Rows(i).Item("TipoZona")
            }
            If Not IsDBNull(dt.Rows(i).Item("N")) Then
                acqua.N = dt.Rows(i).Item("N")
            End If

            Output.ListaTipiAcqua.Add(acqua)

        Next

        Return Output

    End Function

End Class

Public Class PianoConcimazione_TipoAcqua_input

    Public Regolamento_Cod As Integer
    Public Codice As Integer
    Public TipoZona As String
    Public Url As String

    Sub New()
        Regolamento_Cod = 0
        Codice = -1
        TipoZona = ""
        Url = ""
    End Sub

End Class

Public Class PianoConcimazione_TipoAcqua_output

    Public ListaTipiAcqua As List(Of TipoAcqua)

    Public MessaggioErrore As String

    Public Sub New()
        ListaTipiAcqua = New List(Of TipoAcqua)
        MessaggioErrore = ""
    End Sub

End Class

Public Class TipoAcqua

    Public Codice As Integer
    Public Descrizione As String
    Public N As Decimal
    Public TipoZona As String

    Sub New()
        Descrizione = ""
        Codice = -1
        N = 0
        TipoZona = ""
    End Sub

End Class
