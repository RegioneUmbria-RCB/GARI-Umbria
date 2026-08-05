Public Class HubIoT_ParametriConnessioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PivaSuperUser As String,
                          ByVal Piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     PivaSuperUser, ")
            strSql.AppendLine("     Piva, ")
            strSql.AppendLine("     Parametri ")
            strSql.AppendLine(" FROM  [HubIoT_ParametriConnessioni] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" And Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
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

    Public Function LeggiElenco(ByVal PivaSuperUser As String,
                          ByVal ElencoPive As List(Of String),
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_R.LeggiElenco()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" CREATE TABLE #pive ( Piva varchar(25) ); ")
            strSql.AppendLine(" ALTER TABLE #pive ALTER COLUMN Piva varchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS ")
            For Each Piva In ElencoPive
                strSql.AppendLine(" insert into #pive values ('" & Agro_SQL_SaveText(Piva) & "'); ")
            Next


            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     a.PivaSuperUser, ")
            strSql.AppendLine("     a.Piva, ")
            strSql.AppendLine("     a.Parametri ")
            strSql.AppendLine(" FROM  [HubIoT_ParametriConnessioni] a inner join ")
            strSql.AppendLine(" #pive b on (a.Piva=b.Piva) ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                strSql.AppendLine(" And a.PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri) & "; ")
            End If

            strSql.AppendLine("drop table #pive; ")

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
Public Class HubIoT_ParametriConnessioni_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function Scrivi(ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal Parametri As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                            Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                            Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                            Optional ByVal username_creazione As String = "",
                            Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_W.Scrivi()"


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
            StrSQL.AppendLine("INSERT INTO HubIoT_ParametriConnessioni ( PivaSuperUser, Piva, Parametri, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Parametri) & "' ")
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

    Public Function Modifica(ByVal PivaSuperUser As String,
                            ByVal Piva As String,
                            ByVal Parametri As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE HubIoT_ParametriConnessioni SET ")
            StrSQL.AppendLine("   Parametri='" & Agro_SQL_SaveText(Parametri) & "' ")
            StrSQL.AppendLine("   ,Inviato           =  0 ")
            StrSQL.AppendLine("   ,DataInvio         =  Null ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.AppendLine("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.AppendLine(" WHERE 1=1 ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
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

    Public Function Cancella(ByVal PivaSuperUser As String,
                             ByVal Piva As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreHubIoTDAL.HubIoT_ParametriConnessioni_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from HubIoT_ParametriConnessioni ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND PivaSuperUser = '" & Agro_SQL_SaveText(PivaSuperUser) & "' ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
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
