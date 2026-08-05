Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Agronica_Log_Contatti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Contatti_NonInviati(ByVal tipoEsportazione As enum_Esportazioni_Sistema_Cod,
                                       pive_ammesse As List(Of String),
                                       xFiltroAggiuntivo As String,
                                       xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal origine_escludi As Integer = 0
                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.AgronicaLogAnalisi_R.Analisi_NonInviate()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" WITH ")

            stb.AppendLine(" log_contatti AS ( ")
            stb.AppendLine(" SELECT Piva, Cod_Contatto, Data_Ora_RegistrazioneLog, UltimaOperazione ")
            stb.AppendLine(" FROM Agronica_Log_Contatti_UltimaOperazione ")
            stb.AppendLine(" WHERE  Origine <> " & enum_SistemiEsterni.demetra)

            If pive_ammesse IsNot Nothing AndAlso pive_ammesse.Count > 0 Then
                stb.AppendLine(" AND Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", pive_ammesse), True) & ") ")
            End If

            stb.AppendLine(" ), ")

            stb.AppendLine(" log_invio_contatti AS ( ")
            stb.AppendLine(" SELECT Piva, Cod_Contatto, Max(Id_Log_Invio) id_log_invio ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Contatti ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione))
            If pive_ammesse IsNot Nothing AndAlso pive_ammesse.Count > 0 Then
                stb.AppendLine(" AND Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", pive_ammesse), True) & ") ")
            End If
            stb.AppendLine(" GROUP BY Piva, Cod_Contatto ")
            stb.AppendLine(" ), ")

            stb.AppendLine(" log_invio_chiamate  as ( ")
            stb.AppendLine(" SELECT ID, Esito, Data_Invio ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Chiamate ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione))
            stb.AppendLine(" ) ")


            stb.AppendLine(" Select log_contatti.Piva, log_contatti.Cod_Contatto, UltimaOperazione, ISNULL(log_invio_chiamate.Esito, '') AS Esito, ISNULL(log_invio_chiamate.ID, 0) AS ID_Chiamata ")
            stb.AppendLine(" From log_contatti ")
            stb.AppendLine(" Left OUTER JOIN log_invio_contatti ")
            stb.AppendLine(" On log_contatti.Piva = log_invio_contatti.Piva And log_contatti.Cod_Contatto = log_invio_contatti.Cod_Contatto ")
            stb.AppendLine(" Left OUTER JOIN log_invio_chiamate ")
            stb.AppendLine(" On (log_invio_contatti.id_log_invio = log_invio_chiamate.ID) ")

            stb.AppendLine(" WHERE 1 = 1 ")

            stb.AppendLine(" AND (log_invio_chiamate.Data_Invio Is NULL OR (log_contatti.Data_Ora_RegistrazioneLog >= log_invio_chiamate.Data_Invio And log_invio_chiamate.Esito in ('OK','BLK') ) OR (log_invio_chiamate.Esito = 'KO')) ")

            ' scarto le cancellazioni
            'stb.AppendLine(" and log_contatti.UltimaOperazione <> 3")

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

Public Class Agronica_Log_Contatti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal tipoOperazione As Integer,
                           ByVal Piva As String,
                           ByVal Cod_Contatto As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_CF As Integer,
                           ByVal Contatto_Des As String,
                           ByVal Note As String,
                           ByVal idServizio As enum_Id_Servizio,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal object_data As String = "",
                           Optional ByVal origine As Integer = enum_SistemiEsterni.gias
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreAnagrafeDAL.Agronica_Log_Contatti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If String.IsNullOrEmpty(Piva) Then
                Throw New Exception("Il parametro Piva non è valorizzato")
            End If

            If String.IsNullOrEmpty(Cod_Contatto) Then
                Throw New Exception("Il parametro Cod_Contatto non è valorizzato")
            End If

            Dim chiave As String = String.Format("{0}_{1}", Piva, Cod_Contatto)

            If IsNothing(Note) OrElse String.IsNullOrEmpty(Note) Then
                Note = String.Empty
            End If

            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Agronica_Log_Contatti ")
            strSql.AppendLine("             ( SuperUser, Utente, Chiave, Tipo_Operazione, ")
            strSql.AppendLine("               Piva, Cod_Contatto, Sa_Cod, Id_Cf, Contatto_Des, ")
            strSql.AppendLine("               Note, Data_Ora_RegistrazioneLog, Id_Servizio, object_data, Origine) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("           '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(chiave) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(tipoOperazione) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_CF) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveText_NULL(Contatto_Des) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Note) & "'  ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(CInt(idServizio)) & "  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(object_data) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(origine) & "  ")

            strSql.AppendLine("         )")

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
