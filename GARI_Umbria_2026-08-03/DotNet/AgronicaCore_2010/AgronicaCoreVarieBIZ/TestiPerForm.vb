Public Class TestiPerForm_R

    Public Function LeggiTestoPersonalizzaOppureDefault(
            ByVal WebForm As String,
            ByVal WebControlId As String,
            ByVal xFiltroAggiuntivo As String,
            ByVal xOrderBy As String,
            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
        ) As RispostaStandard

        Dim r As New RispostaStandard

        Try
            Dim lettura As New AgronicaCoreVarieDAL.TestiPerForm_R

            '1. se esiste personalizzato...
            Dim dtLettura As DataTable =
                lettura.Leggi(objParametri.PivaSuperUser, WebForm, WebControlId, xFiltroAggiuntivo, xOrderBy, objParametri)

            If dtLettura.Rows.Count = 1 Then
                r.RispostaStringa = dtLettura.Rows(0)("TestiPerForm_Des")

            Else

                dtLettura =
                    lettura.Leggi("", WebForm, WebControlId, xFiltroAggiuntivo, xOrderBy, objParametri)

                If dtLettura.Rows.Count = 1 Then
                    r.RispostaStringa = dtLettura.Rows(0)("TestiPerForm_Des")

                Else
                    r.RispostaOK = False
                    r.Errore = "Testo non trovato."
                    Return r
                End If

            End If

            r.RispostaOK = True

        Catch ex As Exception

            r.RispostaOK = False
            r.RispostaStringa = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

        End Try

        Return r

    End Function

End Class

