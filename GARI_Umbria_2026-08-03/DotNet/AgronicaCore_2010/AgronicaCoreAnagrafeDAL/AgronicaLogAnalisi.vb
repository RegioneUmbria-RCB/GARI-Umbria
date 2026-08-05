Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class AgronicaLogAnalisi_R
    Inherits DataProvider

    Public Function Leggi(ID As Integer,
                          SuperUser As String,
                          Utente As String,
                          Tipo_Operazione As enum_TipoOperazioneDB,
                          Tipo_Analisi As enum_AnalisiTipo,
                          Analisi_Testata_Cod As Integer,
                          Id_Servizio As Integer,
                          xFiltroAggiuntivo As String,
                          xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                            Optional Origine As Integer = 0,
                            Optional Piva As String = ""
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Agronica_Log_Analisi ")

            stb.AppendLine(" WHERE 1=1 ")

            If ID <> 0 Then
                stb.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If SuperUser <> "" Then
                stb.AppendLine(" AND SuperUser = '" & Agro_SQL_SaveText(SuperUser) & "' ")
            End If

            If Utente <> "" Then
                stb.AppendLine(" AND Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
            End If

            If Tipo_Operazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND Tipo_Operazione = " & Agro_SQL_SaveNum(Tipo_Operazione) & " ")
            End If

            If Tipo_Analisi <> 0 Then
                stb.AppendLine(" AND Tipo_Analisi = " & Agro_SQL_SaveNum(Tipo_Analisi) & " ")
            End If

            If Analisi_Testata_Cod <> 0 Then
                stb.AppendLine(" AND Analisi_Testata_Cod = " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            End If

            If Id_Servizio <> 0 Then
                stb.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If

            If Origine <> 0 Then
                stb.AppendLine(" AND Origine = " & Agro_SQL_SaveNum(Origine) & " ")
            End If

            If Piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Analisi_NonInviate(ByVal Tipo_Esportazione As enum_Esportazioni_Sistema_Cod,
                                       Piva_Ammesse As List(Of String),
                                       xFiltroAggiuntivo As String,
                                       xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Origine_Escludi As Integer = 0
                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_R.Analisi_NonInviate()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" WITH cte_key_export_type AS ( ")
            stb.AppendLine(" SELECT Analisi_Testata_Cod, Max(Id_Log_Invio) id_log_invio ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Analisi ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione) & " ")
            stb.AppendLine(" GROUP BY Analisi_Testata_Cod ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" cte_id_analisi AS ( ")
            stb.AppendLine(" SELECT Analisi_Testata_Cod, MAX(ID) AS max_id  ")
            stb.AppendLine(" FROM Agronica_Log_Analisi ")
            stb.AppendLine(" GROUP BY Analisi_Testata_Cod ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" cte_max_id_esito_export_key_type AS ( ")
            stb.AppendLine(" SELECT ID, Esito, Data_Invio ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Chiamate ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(Tipo_Esportazione) & " ")
            stb.AppendLine(" ) ")

            stb.AppendLine(" SELECT max_id_log_analisi.Analisi_Testata_Cod, log_analisi.Tipo_Operazione, ISNULL(log_invio_esito.Esito, '') AS Esito, ISNULL(log_invio_esito.ID, '') AS ID_Chiamata, log_analisi.utente, log_analisi.Piva  ")
            stb.AppendLine("        , CASE WHEN log_analisi.Tipo_Operazione = '3' THEN log_analisi.object_data ELSE '' END AS object_data -- SOLO IN CASO DI CANCELLAZIONE CI ANDIAMO A RECUPERARE object_data DA CUI SIAMO IN GRADO DI ESTRARRE LA PIVA PER CERCARE IL CUAA ")
            stb.AppendLine(" FROM Agronica_Log_Analisi log_analisi ")

            stb.AppendLine(" INNER JOIN cte_id_analisi max_id_log_analisi ")
            stb.AppendLine(" ON (log_analisi.Analisi_Testata_Cod = max_id_log_analisi.Analisi_Testata_Cod AND log_analisi.ID = max_id_log_analisi.max_id) ")

            stb.AppendLine(" LEFT OUTER JOIN cte_key_export_type log_invio ")
            stb.AppendLine(" ON (max_id_log_analisi.Analisi_Testata_Cod = log_invio.Analisi_Testata_Cod) ")

            stb.AppendLine(" LEFT OUTER JOIN cte_max_id_esito_export_key_type log_invio_esito ")
            stb.AppendLine(" ON (log_invio.id_log_invio = log_invio_esito.ID) ")

            stb.AppendLine(" WHERE 1 = 1 ")

            stb.AppendLine(" AND (log_invio_esito.Data_Invio IS NULL OR (log_analisi.Data_Ora_RegistrazioneLog >= log_invio_esito.Data_Invio AND log_invio_esito.Esito IN ('OK','BLK')) OR (log_invio_esito.Esito = 'KO')) ")

            If Origine_Escludi <> 0 Then
                stb.AppendLine(" AND log_analisi.Origine <> " & Agro_SQL_SaveNum(Origine_Escludi) & " ")
            End If

            If Piva_Ammesse IsNot Nothing AndAlso Piva_Ammesse.Count > 0 Then
                stb.AppendLine(" AND log_analisi.Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", Piva_Ammesse), True) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
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

Public Class AgronicaLogAnalisi_W
    Inherits DataProvider
    Public Function Scrivi(Tipo_Operazione As enum_TipoOperazioneDB,
                           Tipo_Analisi As enum_AnalisiTipo,
                           Analisi_Testata_Cod As Integer,
                           Analisi_Testata_Des As String,
                           Analisi_Testata_Data_Inizio As Date,
                           Note As String,
                           Id_Servizio As enum_Id_Servizio,
                           ByRef objParametri As AgronicaCoreParametri,
                                Optional object_data As String = "",
                                Optional Origine As Integer = -1,
                                Optional Piva As String = ""
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Analisi_Testata_Cod = 0 Then
                Throw New Exception("Analisi_Testata_Cod (chiave analisi) NON può essere vuota")
            End If

            Dim timestamp As Date = Date.Now()

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Agronica_Log_Analisi ")
            strSql.AppendLine("             ( SuperUser, Utente, Tipo_Operazione,  ")
            strSql.AppendLine("               Tipo_Analisi, Analisi_Testata_Cod, Analisi_Testata_Des, Analisi_Testata_Data_Inizio, ")
            strSql.AppendLine("               Note, Data_Ora_RegistrazioneLog, Id_Servizio, object_data, Origine, Piva ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("           '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Operazione) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Analisi) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Analisi_Testata_Cod) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Analisi_Testata_Des) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Analisi_Testata_Data_Inizio) & " ")

            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(timestamp) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(CInt(Id_Servizio)) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(object_data) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Origine) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")

            strSql.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function
End Class
