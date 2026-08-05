Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework_POCO
Imports System.Data.Entity
Imports Newtonsoft.Json

Public Class FF_ImportDatiMacchinaOperazioniAgenda_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function GetMovimentiAgendaDaAssociareConTaskDataSDF(ByVal PartitaIva As String,
                                                                ByVal CentroAziendale As Integer,
                                                                ByVal Appezzamento As Integer,
                                                                ByVal Impianto As Integer,
                                                                ByVal LavCod As String,
                                                                ByVal DataMovimentoRiferimento As DateTime,
                                                                ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                Optional ByVal xFiltroAggiuntivo As String = "",
                                                                Optional ByVal xOrderBy As String = ""
                                                               ) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_ImportDatiMacchinaOperazioniAgenda_R.GetMovimentiAgendaDaAssociareConTaskDataSDF()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Try

            strSql.Length = 0
            strSql.Append(" SELECT ")
            strSql.Append("     a.*, ")
            strSql.Append("     b.Data_Movimento ")
            strSql.Append(" from ")
            strSql.Append("   agenda a left join ")
            strSql.Append("   movimenti b on (a.piva=b.piva and a.sa_cod=b.sa_cod and a.id_agenda=b.id_agenda) left join ")
            strSql.Append("   Mov_Destinazioni c on (b.PIVA=c.Piva and b.Sa_Cod=c.Sa_Cod and b.Id_Agenda=c.Id_Agenda and b.Id_Mov=c.Id_Mov) ")
            strSql.Append(" WHERE  ")
            strSql.Append("     c.Tipo_destinazione=0  ")
            strSql.Append("     and a.Lav_Cod='" & Agro_SQL_SaveText(LavCod) & "' ")

            If PartitaIva <> "" Then
                strSql.Append(" And a.Piva = '" & Agro_SQL_SaveText(PartitaIva) & "' ")
            End If
            If CentroAziendale <> 0 Then
                strSql.Append(" And c.Sa_Cod = " & Agro_SQL_SaveNum(CentroAziendale) & " ")
            End If
            If Appezzamento <> 0 Then
                strSql.Append(" And c.Appezza = " & Agro_SQL_SaveNum(Appezzamento) & " ")
            End If
            If Impianto <> 0 Then
                strSql.Append(" And c.Id_Destinazione = " & Agro_SQL_SaveNum(Impianto) & " ")
            End If

            If DataMovimentoRiferimento > AGRODATAINIZIO And DataMovimentoRiferimento < AGRODATAFINE Then
                strSql.Append(" And b.Data_Movimento <= " & Agro_SQL_SaveDate(DataMovimentoRiferimento) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return dt
    End Function

    Public Function GetMovimentoAgendaAssociatoConTaskDataSDF(ByVal PartitaIva As String,
                                                                ByVal ProgettoCod As Integer,
                                                                ByVal CodiceOperazione As Integer,
                                                                ByVal id_agenda As Integer,
                                                                ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                                Optional ByVal xFiltroAggiuntivo As String = "",
                                                                Optional ByVal xOrderBy As String = ""
                                                               ) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_ImportDatiMacchinaOperazioniAgenda_R.GetMovimentoAgendaAssociatoConTaskDataSDF()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Try

            strSql.Length = 0
            strSql.Append(" SELECT ")
            strSql.Append("     * ")
            strSql.Append(" from ")
            strSql.Append("   FF_ImportDatiMacchineOperazioniAgenda ")
            strSql.Append(" WHERE  ")
            strSql.Append("     Cod_Operazione=" & Agro_SQL_SaveNum(CodiceOperazione) & " ")

            If PartitaIva <> "" Then
                strSql.Append(" And Piva = '" & Agro_SQL_SaveText(PartitaIva) & "' ")
            End If
            If ProgettoCod <> 0 Then
                strSql.Append(" And Cod_Impianto = " & Agro_SQL_SaveNum(ProgettoCod) & " ")
            End If
            If id_agenda <> 0 Then
                strSql.Append(" And id_agenda = " & Agro_SQL_SaveNum(id_agenda) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            dt.Columns.Add("cO2", GetType(Decimal))
            dt.Columns.Add("fuelConsumption", GetType(Decimal))
            dt.Columns.Add("vehicleVin")
            dt.Columns.Add("vehicleModel")
            dt.Columns.Add("vehicleBrand")
            dt.Columns.Add("taskDataEndDateTime", GetType(DateTime))

            For Each row In dt.Rows
                Dim objTaskData As AgronicaCoreDTOStd.SDF.TaskData =
                JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.SDF.TaskData)(row("payload"))
                row("cO2") = objTaskData.cO2
                row("fuelConsumption") = objTaskData.fuelConsumption
                row("vehicleVin") = objTaskData.vehicle.vin
                row("vehicleModel") = objTaskData.vehicle.model
                row("vehicleBrand") = objTaskData.vehicle.brand
                row("taskDataEndDateTime") = objTaskData.taskDataEndDateTime
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return dt
    End Function


    Public Function GetTaskDataAssociabiliConMovimentiAgenda(ByVal PartitaIva As String,
                                                               ByVal ProgettoCod As Integer,
                                                               ByVal CodiceOperazione As Integer,
                                                               ByRef ObjParametri_Server As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                                               Optional ByVal xFiltroAggiuntivo As String = "",
                                                               Optional ByVal xOrderBy As String = ""
                                                              ) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.FF_ImportDatiMacchinaOperazioniAgenda_R.GetMovimentoAgendaAssociatoConTaskDataSDF()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Try

            strSql.Length = 0
            strSql.Append(" SELECT ")
            strSql.Append("     * ")
            strSql.Append(" from ")
            strSql.Append("   FF_ImportDatiMacchineOperazioniAgenda ")
            strSql.Append(" WHERE  ")
            strSql.Append("     stato=" & Agro_SQL_SaveNum(2) & " ")
            strSql.Append("     and Cod_Operazione=" & Agro_SQL_SaveNum(CodiceOperazione) & " ")
            strSql.Append("     And id_agenda = " & Agro_SQL_SaveNum(0) & " ")

            If PartitaIva <> "" Then
                strSql.Append(" And Piva = '" & Agro_SQL_SaveText(PartitaIva) & "' ")
            End If
            If ProgettoCod <> 0 Then
                strSql.Append(" And Cod_Impianto = " & Agro_SQL_SaveNum(ProgettoCod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Server))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            dt.Columns.Add("cO2", GetType(Decimal))
            dt.Columns.Add("fuelConsumption", GetType(Decimal))
            dt.Columns.Add("vehicleVin")
            dt.Columns.Add("vehicleModel")
            dt.Columns.Add("vehicleBrand")
            dt.Columns.Add("taskDataEndDateTime", GetType(DateTime))

            For Each row In dt.Rows
                Dim objTaskData As AgronicaCoreDTOStd.SDF.TaskData =
                JsonConvert.DeserializeObject(Of AgronicaCoreDTOStd.SDF.TaskData)(row("payload"))
                row("cO2") = objTaskData.cO2
                row("fuelConsumption") = objTaskData.fuelConsumption
                row("vehicleVin") = objTaskData.vehicle.vin
                row("vehicleModel") = objTaskData.vehicle.model
                row("vehicleBrand") = objTaskData.vehicle.brand
                row("taskDataEndDateTime") = objTaskData.taskDataEndDateTime
            Next

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
        Return dt
    End Function

End Class
