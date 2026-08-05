Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class DomandaIrrigua_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Testata As Integer,
                          ByVal piva As String,
                          ByVal Stato As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                          Optional ByVal Validita_Inizio As DateTime = AGRODATAINIZIO,
                          Optional ByVal Validita_Fine As DateTime = AGRODATAFINE) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [DomandaIrrigua_Testata] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If Id_Testata > 0 Then
                strSql.AppendLine(" And Id = " & Agro_SQL_SaveNum(Id_Testata) & " ")
            End If

            If piva <> "" Then
                strSql.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If Stato > -1 Then
                strSql.AppendLine(" AND Stato = " & Agro_SQL_SaveNum(Stato) & " ")
            End If

            If Validita_Inizio <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Validita_Fine <> AGRODATAFINE Then
                strSql.AppendLine(" AND Validita_Fine <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
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

Public Class DomandaIrrigua_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id As Integer,
                           ByVal Piva As String,
                           ByVal N_Protocollo As String,
                           ByVal Data_Protocollo As DateTime,
                           ByVal TipoContratto As Integer,
                           ByVal Stato As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_W.Scrivi()"


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
            StrSQL.AppendLine("INSERT INTO DomandaIrrigua_Testata (Id, PIVA, N_Protocollo, Data_Protocollo, TipoContratto, Stato, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          " & Agro_SQL_SaveNum(Id) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(N_Protocollo) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Protocollo) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(TipoContratto) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Stato) & "  ")
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

    Public Function Modifica(ByVal Id As Integer,
                             ByVal N_Protocollo As String,
                             ByVal Data_Protocollo As DateTime,
                             ByVal TipoContratto As Integer,
                             ByVal Stato As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE DomandaIrrigua_Testata SET ")
            StrSQL.Append("    N_Protocollo           =  '" & Agro_SQL_SaveText(N_Protocollo) & "'  ")
            StrSQL.Append("   ,Data_Protocollo        =  " & Agro_SQL_SaveDateTime(Data_Protocollo) & "  ")
            StrSQL.Append("   ,TipoContratto          =  " & Agro_SQL_SaveNum(TipoContratto) & "  ")
            StrSQL.Append("   ,Stato                  =  " & Agro_SQL_SaveNum(Stato) & "  ")
            StrSQL.Append("   ,Inviato                =  0 ")
            StrSQL.Append("   ,DataInvio              =  Null ")
            StrSQL.Append("   ,Data_Modifica          =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica      = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE  1=1 ")

            If Id <> 0 Then
                StrSQL.Append(" AND Id = " & Agro_SQL_SaveNum(Id) & "   ")
            End If

            '----------------------------------------------------------------------
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

    Public Function Cancella(ByVal Id As Integer,
                                               ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.DomandaIrrigua_Testata_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from DomandaIrrigua_Testata ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If Id > 0 Then
                StrSQL.AppendLine(" AND id = " & Agro_SQL_SaveNum(Id) & " ")
            End If
            '----------------------------------------------------------------------
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