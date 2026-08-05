Public Class Carburanti_GHG_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
                         ByVal carCod As Integer,
                         ByVal udmCod As Integer,
                         ByVal direttivaCod As Integer,
                         ByVal codiceStato As String,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Carburanti_GHG_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Carburanti_GHG ")
            StrSQL.Append($" WHERE Validita_inizio <= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)} ")
            StrSQL.Append($" AND   Validita_Fine >= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)} ")

            If carCod <> 0 Then
                StrSQL.Append($" AND Car_Cod = {Agro_SQL_SaveNum(carCod)} ")
            End If
            If udmCod <> 0 Then
                StrSQL.Append($" AND Udm_Cod = {Agro_SQL_SaveNum(udmCod)} ")
            End If
            If direttivaCod <> 0 Then
                StrSQL.Append($" AND Direttiva_Cod =  {Agro_SQL_SaveNum(direttivaCod)} ")
            End If
            If codiceStato <> "" Then
                StrSQL.Append($" AND (Codice_Stato = '{Agro_SQL_SaveText(codiceStato)}' OR Codice_Stato IS NULL) ")
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
                StrSQL.Append($" ORDER BY {xOrderBy} ")
            Else
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


    Public Function LeggiStandard_Factor(ByVal carCod As Integer,
                                         ByVal udmCod As Integer,
                                         ByVal direttivaCod As Integer,
                                         ByVal codiceStato As String,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Carburanti_GHG_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Carburanti_GHG ")
            StrSQL.Append($" WHERE Validita_inizio <= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine)} ")
            StrSQL.Append($" AND   Validita_Fine >= {Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio)} ")

            StrSQL.Append(" AND Car_Cod In (0, " & Agro_SQL_SaveNum(carCod) & ") ")
            StrSQL.Append(" AND codice_Stato In ('', '" & Agro_SQL_SaveText(codiceStato) & "') ")

            If udmCod <> 0 Then
                StrSQL.Append($" And Udm_Cod = {Agro_SQL_SaveNum(udmCod)} ")
            End If
            If direttivaCod <> 0 Then
                StrSQL.Append($" And Direttiva_Cod =  {Agro_SQL_SaveNum(direttivaCod)} ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append($" AND {Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo,, objParametri)}")
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
            StrSQL.Append(" ORDER BY Car_Cod Desc, codice_Stato Desc ")

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
