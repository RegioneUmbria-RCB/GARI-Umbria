Public Class ParametroReportLocalizzabile

    Public NomeReport As String
    Public NomeSezione As String
    Public Sottoreport As Boolean
    Public NomeParametro As String
    Public ValoreParametro As String

End Class

Public Class DataBaseEntry

    Public Target As String
    Public NomeSezione As String
    Public NomeProprieta As String
    Public ValoreProprieta As String

    Public Function GetListFromDB(ByVal dbTraduzioni As DataTable) As List(Of DataBaseEntry)

        Dim lista = New List(Of DataBaseEntry)

        If Not dbTraduzioni Is Nothing AndAlso dbTraduzioni.Rows.Count > 0 Then

            For Each dr As DataRow In dbTraduzioni.Rows
                Dim parametriCampo = dr.Item("TxtReport_Descrizione").ToString().Split("|")
                Dim valoreProprieta = parametriCampo(1)

                Dim parametriTarget = parametriCampo(0).Split(".")
                Dim target As String = parametriTarget(0)
                Dim nomeSezione As String = parametriTarget(1)
                Dim nomeProprieta As String = parametriTarget(2)

                lista.Add(New DataBaseEntry With {.Target = target, .NomeSezione = nomeSezione,
                          .NomeProprieta = nomeProprieta, .ValoreProprieta = valoreProprieta})
            Next

        End If

        Return lista



    End Function



End Class