Public Class Fertilizzanti_GHG_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         ByVal direttivaCod As Integer,
                         ByVal codiceStato As String,
                         ByVal tpCod As Integer,
                         ByVal ferCod As Integer,
                         ByVal data As Date,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Fertilizzanti_GHG_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Fertilizzanti_GHG ")
            StrSQL.Append(" WHERE 1 = 1 ")
            If direttivaCod <> 0 Then
                StrSQL.Append($" AND Direttiva_Cod =  {Agro_SQL_SaveNum(direttivaCod)} ")
            End If
            If codiceStato <> "" Then
                StrSQL.Append($" AND (Codice_Stato = '{Agro_SQL_SaveText(codiceStato)}' Or Codice_Stato is NULL) ")
            End If
            If tpCod <> 0 Then
                StrSQL.Append($" AND Tp_Cod =  {Agro_SQL_SaveNum(tpCod)} ")
            End If
            If ferCod <> 0 Then
                StrSQL.Append($" AND (Fer_Cod =  {Agro_SQL_SaveNum(ferCod)} Or Fer_Cod is NULL) ")
            End If
            If data <> Nothing Then
                StrSQL.Append($" AND Validita_Inizio <= {Agro_SQL_SaveDate(data)} ")
                StrSQL.Append($" AND Validita_Fine >= {Agro_SQL_SaveDate(data)} ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append($" AND {xFiltroAggiuntivo}")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Inviato >= 0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Inviato = -1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append($" ORDER BY {xOrderBy}")
            Else
                ' Ordine discedente per avere i campi con NULL in fondo
                StrSQL.Append(" ORDER BY Fer_Cod DESC, Codice_Stato DESC ")
            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception($"[{NomeRoutine}] : {MessaggioErrore}")
        End Try

        Return DT

    End Function

End Class
