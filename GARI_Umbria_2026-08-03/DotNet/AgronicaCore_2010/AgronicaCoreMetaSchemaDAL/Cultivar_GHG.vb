Public Class Cultivar_GHG_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                     ByVal direttivaCod As Integer,
                     ByVal codiceStato As String,
                     ByVal vegCod As Integer,
                     ByVal culCod As Integer,
                     ByVal xFiltroAggiuntivo As String,
                     ByVal xOrderBy As String,
                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Cultivar_GHG_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Cultivar_GHG ")
            StrSQL.Append($" WHERE Validita_inizio <= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)} ")
            StrSQL.Append($" AND   Validita_Fine >= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)} ")

            If direttivaCod <> 0 Then
                StrSQL.Append($" AND Direttiva_Cod =  {Agro_SQL_SaveNum(direttivaCod)} ")
            End If
            If codiceStato <> "" Then
                StrSQL.Append($" AND (Codice_Stato = '{Agro_SQL_SaveText(codiceStato)}' Or Codice_Stato IS NULL) ")
            End If
            If vegCod <> 0 Then
                StrSQL.Append($" AND Veg_Cod =  {Agro_SQL_SaveNum(vegCod)} ")
            End If
            If culCod <> 0 Then
                StrSQL.Append($" AND Cul_Cod =  {Agro_SQL_SaveNum(culCod)} ")
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
                StrSQL.Append(" ORDER BY Codice_Stato DESC ")
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
