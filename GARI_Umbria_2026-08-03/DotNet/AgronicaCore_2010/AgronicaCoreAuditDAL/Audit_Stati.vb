Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Audit_Stati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(
        ByVal Audit_Tipo As Int32,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Stati_R.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Audit_Stati ")
            StrSQL.AppendLine(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine(" AND   Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Validita_Fine DESC ")
            End If

            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiStatiWorkflow(
        ByVal Audit_Tipo As Int32,
        ByVal Audit_Stato As Int32,
        ByVal Servizio_Cod As Int32,
        ByVal Stato_Origine_Cod As Int32,
        ByVal Stato_Destinazione_Cod As Int32,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Stati_R.LeggiStatiWorkflow()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.AppendLine(" SELECT t.* ")
                    StrSQL.AppendLine("        , o.WAnagraficaStati_Des as Stato_Origine_Des ")
                    StrSQL.AppendLine("        , d.WAnagraficaStati_Des as Stato_Destinazione_Des ")
                    StrSQL.AppendLine("        , s.Servizio_Des ")
                    StrSQL.AppendLine(" FROM  WTransizioniDiStatoAudit t ")
                    StrSQL.AppendLine(" INNER JOIN WAnagraficaStati o ON t.Stato_Origine_Cod=o.WAnagraficaStati_Cod ")
                    StrSQL.AppendLine(" INNER JOIN WAnagraficaStati d ON t.Stato_Destinazione_Cod=d.WAnagraficaStati_Cod ")
                    StrSQL.AppendLine(" INNER JOIN Servizi s ON t.Servizio_cod=s.Servizio_Cod ")

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.AppendLine(" SELECT t.* ")
                    StrSQL.AppendLine(" FROM  WTransizioniDiStatoAudit t ")

            End Select

            StrSQL.AppendLine(" WHERE 1=1 ")
            If Audit_Tipo <> 0 Then
                StrSQL.AppendLine(" AND t.Audit_Tipo = " & Agro_SQL_SaveNum(Audit_Tipo) & " ")
            End If
            If Audit_Stato <> 0 Then
                StrSQL.AppendLine(" AND t.Audit_Stato = " & Agro_SQL_SaveNum(Audit_Stato) & " ")
            End If
            If Servizio_Cod <> 0 Then
                StrSQL.AppendLine(" AND t.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If
            If Stato_Origine_Cod <> 0 Then
                StrSQL.AppendLine(" AND t.Stato_Origine_Cod = " & Agro_SQL_SaveNum(Stato_Origine_Cod) & " ")
            End If
            If Stato_Destinazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND t.Stato_Destinazione_Cod = " & Agro_SQL_SaveNum(Stato_Destinazione_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   t.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   t.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function LeggiAreeWorkflow(
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional xFiltroAggiuntivo As String = "",
        Optional xOrderBy As String = ""
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreAuditDAL.Audit_Stati_R.LeggiAreeWorkflow()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM  Alert_Area ")
            StrSQL.AppendLine(" WHERE 1=1")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            return EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class
