
Imports AgronicaCoreDataProvider

Public Class FF_ConfigurazioneDiStampa_biz


    Public Function Leggi(ByVal FF_Stampa_Dettagli_Cod As Integer, ByVal FF_Stampanti_cod As Integer, ByVal objParametri_Server As AgronicaCoreParametri) As FF_ConfigurazioneDiStampa_obj


        Dim MessaggioErrore As String = ""
        Dim NomeRoutine As String = "FF_ConfigurazioneDiStampa_biz.Leggi"

        Dim rval As FF_ConfigurazioneDiStampa_obj = Nothing
        Try


            Dim lett As New FF_ConfigurazioniDiStampa_Dettagli_R
            Dim dt As DataTable = lett.Leggi( _
                FF_Stampa_Dettagli_Cod, _
                FF_Stampanti_cod, _
                "", _
                "", _
                objParametri_Server _
            )

            If dt.Rows.Count > 0 Then
                rval = New FF_ConfigurazioneDiStampa_obj()
                rval.layout_cod = dt.Rows(0)("FF_LayoutEtichette")
                rval.lingua_cod = dt.Rows(0)("FF_Lingua_Cod")
                rval.stampante_cod = dt.Rows(0)("FF_Stampanti_Cod")
                rval.Stampante_NomePerStampa = dt.Rows(0)("Nome_Per_Stampa")
                rval.TipoReport = dt.Rows(0)("FF_TipologiaEtichette_Cod")
                rval.id_mov_det = dt.Rows(0)("id_mov_det")
            End If

        Catch ex As Exception

            'uso questa funzione per ottenere il Messaggio..:
            MessaggioErrore = AgronicaCoreDataProvider.Gestione_Eccezioni_2015.MessaggioCompletoDataEccezione(ex, True, source:=True)

            'passo l'eccezione corrente al throw come inner exception ..:
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore, ex)


        End Try

        Return rval


    End Function

End Class
