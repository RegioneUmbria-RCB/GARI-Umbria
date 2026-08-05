Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class HubIoT_LogInvio_R
    Inherits DataProvider

    Public Function Leggi(ByVal WorkOrderId As String,
                          ByVal Data_Invio As DateTime,
                          ByVal Data_Ricezione As DateTime,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreHubIoTDAL.HubIoT_LogInvio_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [HubIoT_LogInvio] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If WorkOrderId <> "" Then
                strSql.AppendLine(" And WorkOrderId = '" & Agro_SQL_SaveText(WorkOrderId) & "' ")
            End If

            If Data_Invio <> AGRODATAINIZIO Then
                strSql.AppendLine(" And Data_Invio = " & Agro_SQL_SaveDate(Data_Invio) & " ")
            End If

            If Data_Ricezione <> AGRODATAFINE Then
                strSql.AppendLine(" And Data_Ricezione = " & Agro_SQL_SaveDate(Data_Ricezione) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt
    End Function

End Class

Public Class HubIoT_LogInvio_W
    Inherits DataProvider

    Public Function Scrivi(ByVal WorkOrderId As String,
                           ByVal Dati_Inviati As String,
                           ByVal Data_Invio As Date,
                           ByVal Esito_Invio As String,
                           ByVal Dati_Ricevuti As String,
                           ByVal Data_Ricezione As Date,
                           ByVal Esito_Ricezione As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                           Optional ByVal Validita_Fine As Date = AGRODATAFINE
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreHubIoTDAL.HubIoT_LogInvio_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO HubIoT_LogInvio ( WorkOrderId, Dati_Inviati, Data_Invio, Esito_Invio, ")
            StrSQL.AppendLine("                         Dati_Ricevuti, Data_Ricezione, Esito_Ricezione, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("         '" & Agro_SQL_SaveText(WorkOrderId) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Dati_Inviati) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Invio) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Esito_Invio) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Dati_Ricevuti) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Ricezione) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Esito_Ricezione) & "' ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function Modifica(ByVal WorkOrderId As String,
                             ByVal Dati_Inviati As String,
                             ByVal Data_Invio As Date,
                             ByVal Esito_Invio As String,
                             ByVal Dati_Ricevuti As String,
                             ByVal Data_Ricezione As Date,
                             ByVal Esito_Ricezione As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                             Optional ByVal Validita_Fine As Date = AGRODATAFINE
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreHubIoTDAL.HubIoT_LogInvio_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE HubIoT_LogInvio SET ")
            StrSQL.AppendLine("   Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            If Dati_Inviati <> "" Then
                StrSQL.AppendLine("   ,Dati_Inviati = '" & Agro_SQL_SaveText(Dati_Inviati) & "'")
            End If
            If Data_Invio <> AGRODATAINIZIO Then
                StrSQL.AppendLine("   ,Data_Invio     =  " & Agro_SQL_SaveDate(Data_Invio))
            End If
            If Esito_Invio <> "" Then
                StrSQL.AppendLine("   ,Esito_Invio = '" & Agro_SQL_SaveText(Esito_Invio) & "'")
            End If
            If Dati_Ricevuti <> "" Then
                StrSQL.AppendLine("   ,Dati_Ricevuti = '" & Agro_SQL_SaveText(Dati_Ricevuti) & "'")
            End If
            If Data_Ricezione <> AGRODATAFINE Then
                StrSQL.AppendLine("   ,Data_Ricezione     =  " & Agro_SQL_SaveDate(Data_Ricezione))
            End If
            If Esito_Ricezione <> "" Then
                StrSQL.AppendLine("   ,Esito_Ricezione = '" & Agro_SQL_SaveText(Esito_Ricezione) & "'")
            End If

            StrSQL.AppendLine(" WHERE 1=1 ")

            If WorkOrderId <> "" Then
                StrSQL.AppendLine(" And WorkOrderId = '" & Agro_SQL_SaveText(WorkOrderId) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp
    End Function

End Class