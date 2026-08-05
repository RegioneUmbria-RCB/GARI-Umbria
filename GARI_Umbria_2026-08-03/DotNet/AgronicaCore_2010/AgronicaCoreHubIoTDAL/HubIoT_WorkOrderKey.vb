Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class HubIoT_WorkOrderKey_R
    Inherits DataProvider

    Public Function LeggiByID_Stato(ByVal ID As Integer,
                              ByVal Stato As Integer,
                              ByVal xFiltroAggiuntivo As String,
                              ByVal xOrderBy As String,
                              ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_R.LeggiByID()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [HubIoT_WorkOrderKey] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If ID <> 0 Then
                strSql.AppendLine(" And ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If Stato <> 0 Then
                strSql.AppendLine(" And Stato = " & Agro_SQL_SaveNum(Stato) & " ")
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

    Public Function LeggiXWorkOrderId(ByVal PivaSuperUser As String,
                                      ByVal WorkOrderId As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_R.LeggiXWorkOrderId()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [HubIoT_WorkOrderKey] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If WorkOrderId <> 0 Then
                strSql.AppendLine(" And WorkOrderId = '" & Agro_SQL_SaveText(WorkOrderId) & "' ")
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

    Public Function Leggi(ByVal PivaSuperUser As String,
                          ByVal Piva As String,
                          ByVal sa_cod As Integer,
                          ByVal appezza As Integer,
                          ByVal id_Reg As Integer,
                          ByVal id_documento As Integer,
                          ByVal id_operazione_documento As Integer,
                          ByVal Mac_Cod As Integer,
                          ByVal VIN As String,
                          ByVal Stato As Integer,
                          ByVal Data_Registrazione As Date,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_R.LeggiXRicettaOperazioneMacchina()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     A.* ")
            strSql.AppendLine(" FROM  [HubIoT_WorkOrderKey] A left join ")
            strSql.AppendLine("       [Parco_Macchine] B on (A.Piva=B.Piva and A.Mac_Cod=B.Mac_Cod) ")
            strSql.AppendLine(" WHERE 1 = 1 ")
            strSql.AppendLine(" AND Entita_Origine=1 ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And A.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" And A.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If sa_cod <> 0 Then
                strSql.AppendLine(" And A.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If appezza <> 0 Then
                strSql.AppendLine(" And A.appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If

            If id_Reg <> 0 Then
                strSql.AppendLine(" And A.id_Reg = " & Agro_SQL_SaveNum(id_Reg) & " ")
            End If

            If id_documento <> 0 Then
                strSql.AppendLine(" And A.Id_documento = " & Agro_SQL_SaveNum(id_documento) & " ")
            End If

            If id_operazione_documento <> 0 Then
                strSql.AppendLine(" And A.id_documento_operazione = " & Agro_SQL_SaveNum(id_operazione_documento) & " ")
            End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" And B.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If Stato <> -1 Then
                strSql.AppendLine(" And A.Stato = " & Agro_SQL_SaveNum(Stato) & " ")
            End If

            If VIN <> "" Then
                strSql.AppendLine(" And B.VIN = '" & Agro_SQL_SaveText(VIN) & "' ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                strSql.AppendLine(" And A.Data_Registrazione = " & Agro_SQL_SaveDate(Data_Registrazione) & " ")
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

    Public Function LeggiXOperazionePianificataMacchina(ByVal PivaSuperUser As String,
                                                    ByVal Piva As String,
                                                    ByVal id_agenda As Integer,
                                                    ByVal Mac_Cod As Integer,
                                                    ByVal VIN As String,
                                                    ByVal Data_Registrazione As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_R.LeggiXRicettaOperazioneMacchina()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     A.* ")
            strSql.AppendLine(" FROM  [HubIoT_WorkOrderKey] A left join ")
            strSql.AppendLine("       [Parco_Macchine] B on (A.Piva=B.Piva and A.Mac_Cod=B.Mac_Cod) ")
            strSql.AppendLine(" WHERE 1 = 1 ")
            strSql.AppendLine(" AND Entita_Origine=2 ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And A.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" And A.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If id_agenda <> 0 Then
                strSql.AppendLine(" And A.id_documento = " & Agro_SQL_SaveNum(id_agenda) & " ")
            End If

            If Mac_Cod <> 0 Then
                strSql.AppendLine(" And B.Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If VIN <> "" Then
                strSql.AppendLine(" And B.VIN = '" & Agro_SQL_SaveText(VIN) & "' ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                strSql.AppendLine(" And A.Data_Registrazione = " & Agro_SQL_SaveDate(Data_Registrazione) & " ")
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

Public Class HubIoT_WorkOrderKey_W
    Inherits DataProvider

    Public Function Scrivi(ByVal Entita_Origine As Integer,
                           ByVal PivaSuperUser As String,
                           ByVal Piva As String,
                           ByVal sa_cod As Integer,
                           ByVal appezza As Integer,
                           ByVal id_Reg As Integer,
                           ByVal id_documento As Integer,
                           ByVal id_documento_operazione As Integer,
                           ByVal Mac_Cod As Integer,
                           ByVal Id_RegolaElaborazione As Integer,
                           ByVal Data_Registrazione As Date,
                           ByVal WorkOrderId As String,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                           Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_W.Scrivi()"


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
            StrSQL.AppendLine("INSERT INTO HubIoT_WorkOrderKey ( Entita_Origine, PivaSuperUser, Piva, sa_cod, ")
            StrSQL.AppendLine("                         appezza, id_reg, id_documento, ")
            StrSQL.AppendLine("                         id_documento_operazione, Mac_Cod, Id_RegolaElaborazione, ")
            StrSQL.AppendLine("                         Data_Registrazione, Stato, WorkOrderId, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("         " & Agro_SQL_SaveNum(Entita_Origine) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(sa_cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(appezza) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(id_Reg) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(id_documento) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(id_documento_operazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Mac_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_RegolaElaborazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Data_Registrazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(0) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(WorkOrderId) & "' ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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

    Public Function Modifica(ByVal ID As Integer,
                             ByVal Entita_Origine As Integer,
                             ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByVal sa_cod As Integer,
                             ByVal appezza As Integer,
                             ByVal id_Reg As Integer,
                             ByVal id_documento As Integer,
                             ByVal id_documento_operazione As Integer,
                             ByVal Mac_Cod As Integer,
                             ByVal Id_RegolaElaborazione As Integer,
                             ByVal WorkOrderId As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByVal Stato As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Validita_Inizio As Date = AGRODATAINIZIO,
                             Optional ByVal Validita_Fine As Date = AGRODATAFINE) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE HubIoT_WorkOrderKey SET ")
            StrSQL.AppendLine("   Stato=" & Agro_SQL_SaveNum(Stato) & " ")
            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE 1=1 ")

            If ID <> 0 Then
                StrSQL.AppendLine(" And ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If Entita_Origine <> 0 Then
                StrSQL.AppendLine(" And Entita_Origine = " & Agro_SQL_SaveNum(Entita_Origine) & " ")
            End If

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" And PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If sa_cod <> 0 Then
                StrSQL.AppendLine(" And sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine(" And appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If

            If id_Reg <> 0 Then
                StrSQL.AppendLine(" And id_Reg = " & Agro_SQL_SaveNum(id_Reg) & " ")
            End If

            If id_documento <> 0 Then
                StrSQL.AppendLine(" And id_documento = " & Agro_SQL_SaveNum(id_documento) & " ")
            End If

            If id_documento_operazione <> 0 Then
                StrSQL.AppendLine(" And id_documento_operazione = " & Agro_SQL_SaveNum(id_documento_operazione) & " ")
            End If

            If Mac_Cod <> 0 Then
                StrSQL.AppendLine(" And Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If Id_RegolaElaborazione <> 0 Then
                StrSQL.AppendLine(" And Id_RegolaElaborazione = " & Agro_SQL_SaveNum(Id_RegolaElaborazione) & " ")
            End If

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

    Public Function Cancella(ByVal ID As Integer,
                             ByVal Entita_Origine As Integer,
                             ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByVal sa_cod As Integer,
                             ByVal appezza As Integer,
                             ByVal id_Reg As Integer,
                             ByVal id_documento As Integer,
                             ByVal id_documento_operazione As Integer,
                             ByVal Mac_Cod As Integer,
                             ByVal Data_Registrazione As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_WorkOrderKey_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from HubIoT_WorkOrderKey ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If ID <> 0 Then
                StrSQL.AppendLine(" And ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If Entita_Origine <> 0 Then
                StrSQL.AppendLine(" And Entita_Origine = " & Agro_SQL_SaveNum(Entita_Origine) & " ")
            End If

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" And PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If sa_cod <> 0 Then
                StrSQL.AppendLine(" And sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If appezza <> 0 Then
                StrSQL.AppendLine(" And appezza = " & Agro_SQL_SaveNum(appezza) & " ")
            End If

            If id_Reg <> 0 Then
                StrSQL.AppendLine(" And id_Reg = " & Agro_SQL_SaveNum(id_Reg) & " ")
            End If

            If id_documento <> 0 Then
                StrSQL.AppendLine(" And id_documento = " & Agro_SQL_SaveNum(id_documento) & " ")
            End If

            If id_documento_operazione <> 0 Then
                StrSQL.AppendLine(" And id_documento_operazione = " & Agro_SQL_SaveNum(id_documento_operazione) & " ")
            End If

            If Mac_Cod <> 0 Then
                StrSQL.AppendLine(" And Mac_Cod = " & Agro_SQL_SaveNum(Mac_Cod) & " ")
            End If

            If Data_Registrazione <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" And Data_Registrazione = " & Agro_SQL_SaveDate(Data_Registrazione) & " ")
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
