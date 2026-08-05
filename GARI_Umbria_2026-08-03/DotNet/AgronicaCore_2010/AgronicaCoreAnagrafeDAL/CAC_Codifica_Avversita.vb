Imports AgronicaCoreDataProvider.UtilityProvider


Public Class CAC_Codifica_Avversita_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Codice_Avversita As String, _
                            ByVal Av_Cod_Gias As Integer, _
                            ByVal Av_Gru_Gias As Integer, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.CAC_Codifica_Avversita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  CAC_Codifica_Avversita ")
            StrSQL.Append(" WHERE 1=1 ")

            If Codice_Avversita <> "" Then
                StrSQL.Append(" AND Codice_Avversita = '" & Agro_SQL_SaveText(Codice_Avversita) & "' ")
            End If

            If Av_Cod_Gias <> 0 Then
                StrSQL.Append(" AND Av_Cod = " & Agro_SQL_SaveNum(Av_Cod_Gias) & " ")
            End If

            If Av_Gru_Gias <> 0 Then
                StrSQL.Append(" AND Av_Gru = " & Agro_SQL_SaveNum(Av_Gru_Gias) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            'Select Case objParametri.FlagVisibilita
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
            '        StrSQL.Append(" AND   Inviato >=0 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
            '        StrSQL.Append(" AND   Inviato =-1 ")
            '    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
            '        '...................................
            '    Case Else
            '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            'End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Descrizione ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception

            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

        End Try

        Return DT

    End Function


End Class
