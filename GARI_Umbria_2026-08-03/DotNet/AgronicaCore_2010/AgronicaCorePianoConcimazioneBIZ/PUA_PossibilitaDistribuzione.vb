Public Class PUA_PossibilitaDistribuzione
    Public Sub New()

    End Sub

    Public Function PossibilitaDistribuzione(ByVal Input As PUA_PossibilitaDistribuzione_input,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                             ) As PUA_PossibilitaDistribuzione_output

        Dim Output As New PUA_PossibilitaDistribuzione_output
        Dim PossDistr As PossibilitaDistribuzione

        Try

            Dim objCore As New AgronicaCoreMetaSchemaDAL.PossibilitaDistribuzione_R
            Dim dt As DataTable = objCore.Leggi(Input.Mese, Input.TipoZona, Input.Veg_Cod, Input.Grfi_Cod,
                                                Input.Id_Gru, Input.Regolamento_Cod,
                                                "", "", objParametri)

            For Each row In dt.Rows

                PossDistr = New PossibilitaDistribuzione With {
                        .Regolamento_Cod = row.Item("Regolamento_Cod"),
                        .Veg_Cod = row.Item("Veg_Cod"),
                        .Grfi_Cod = row.Item("Grfi_Cod"),
                        .Id_Gru = row.Item("Id_Gru"),
                        .Mese = row.Item("Mese"),
                        .TipoZona = row.Item("TipoZona"),
                        .Liquame = row.Item("Liquame"),
                        .Palabile = row.Item("Palabile"),
                        .Chimico = row.Item("Chimico")
                    }

                Output.ListaPossibilitaDistribuzione.Add(PossDistr)

            Next

        Catch ex As Exception
            Output.MessaggioErrore = ex.Message
        End Try

        Return Output

    End Function

End Class

Public Class PUA_PossibilitaDistribuzione_input

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Id_Gru As Integer
    Public Mese As Integer
    Public TipoZona As String

    Public Sub New()
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
        Id_Gru = 0
        Mese = 0
        TipoZona = ""
    End Sub

End Class

Public Class PUA_PossibilitaDistribuzione_output

    Public ListaPossibilitaDistribuzione As List(Of PossibilitaDistribuzione)
    Public MessaggioErrore As String

    Public Sub New()

        ListaPossibilitaDistribuzione = New List(Of PossibilitaDistribuzione)
        MessaggioErrore = ""

    End Sub

End Class


Public Class PossibilitaDistribuzione

    Public Regolamento_Cod As Integer
    Public Veg_Cod As Integer
    Public Grfi_Cod As Integer
    Public Id_Gru As Integer
    Public Mese As Integer
    Public TipoZona As String
    Public Liquame As String
    Public Palabile As String
    Public Chimico As String

    Public Sub New()
        Regolamento_Cod = 0
        Veg_Cod = 0
        Grfi_Cod = 0
        Id_Gru = 0
        Mese = 0
        TipoZona = ""
        Liquame = ""
        Palabile = ""
        Chimico = ""
    End Sub

End Class
