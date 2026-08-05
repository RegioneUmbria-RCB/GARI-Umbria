Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Public Class LettureContatoriAziendali_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiAnagraficheContatori(ByVal piva As String,
                                              ByVal Sa_cod As Integer,
                                              ByVal StartDate As Date,
                                              ByVal EndDate As Date,
                                             ByVal xFiltroAggiuntivo As String,
                                             ByVal xOrderBy As String,
                                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R.LeggiAnagraficheContatori()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [Parco_Macchine] ")
            strSql.AppendLine(" WHERE 1 = 1 ")
            strSql.AppendLine(" and Class_Code='05.07' ")

            If Sa_cod >= -1 Then
                strSql.AppendLine(" And Sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
            End If

            If piva <> "" Then
                strSql.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            strSql.AppendLine(" AND (( validita_inizio <= " & Agro_SQL_SaveDate(StartDate) & " AND validita_fine >= " & Agro_SQL_SaveDate(StartDate) & ") ")
            strSql.AppendLine("      OR (validita_inizio <= " & Agro_SQL_SaveDate(EndDate) & " AND validita_fine >= " & Agro_SQL_SaveDate(EndDate) & ")) ")


            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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
            Throw New Exception("[" & nomeRoutine & "] :   " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi(ByVal ID As Integer,
                          ByVal Piva As String,
                          ByVal Id_Contatore As String,
                          ByVal DataLetturaInizio As DateTime,
                          ByVal DataLetturaFine As DateTime,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("     * ")
            strSql.AppendLine(" FROM  [LettureContatoriAziendali] ")
            strSql.AppendLine(" WHERE 1 = 1  ")

            If ID <> 0 Then
                strSql.AppendLine(" And Id = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If Piva <> "" Then
                strSql.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Contatore <> 0 Then
                strSql.AppendLine(" AND Id_Contatore = " & Agro_SQL_SaveNum(Id_Contatore) & " ")
            End If

            If DataLetturaInizio <> AGRODATAINIZIO Then
                strSql.AppendLine(" AND DataLettura >= " & Agro_SQL_SaveDateTime(DataLetturaInizio) & " ")
            End If

            If DataLetturaFine <> AGRODATAFINE Then
                strSql.AppendLine(" AND DataLettura <= " & Agro_SQL_SaveDateTime(DataLetturaFine) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.AppendLine(" ORDER BY DataLettura Desc")
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
Public Class LettureContatoriAziendali_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Id_Contatore As Integer,
                           ByVal DataLettura As DateTime,
                           ByVal Valore As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "") As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_W.Scrivi()"


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
            StrSQL.AppendLine("INSERT INTO LettureContatoriAziendali (PIVA, Id_Contatore, DataLettura, Valore, ")
            StrSQL.AppendLine("                         Inviato, DataInvio, ")
            StrSQL.AppendLine("                         Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                         UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                         Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                         ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("         '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Contatore) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(DataLettura) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Valore) & "  ")
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
                             ByVal Piva As String,
                             ByVal Id_Contatore As Integer,
                             ByVal DataLettura As DateTime,
                             ByVal Valore As Decimal,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_W.Modifica()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE LettureContatoriAziendali SET ")
            StrSQL.Append("   DataLettura             =  " & Agro_SQL_SaveDateTime(DataLettura) & "  ")
            StrSQL.Append("   ,Valore                 =  " & Agro_SQL_SaveNum(Valore) & "  ")
            StrSQL.Append("   ,Inviato                =  0 ")
            StrSQL.Append("   ,DataInvio              =  Null ")
            StrSQL.Append("   ,Data_Modifica          =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica      = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")


            StrSQL.Append(" WHERE  1=1 ")

            If Id <> 0 Then
                StrSQL.Append(" AND Id = " & Agro_SQL_SaveNum(Id) & "   ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Contatore <> 0 Then
                StrSQL.AppendLine(" AND Id_Contatore = " & Agro_SQL_SaveNum(Id_Contatore) & " ")
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
                             ByVal Piva As String,
                             ByVal Id_Contatore As Integer,
                             ByVal DataLetturaInizio As DateTime,
                             ByVal DataLetturaFine As DateTime,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreDomandaIrriguaDAL.LettureContatoriAziendali_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("DELETE from LettureContatoriAziendali ")
            StrSQL.AppendLine(" WHERE 1 = 1  ")

            If Id <> 0 Then
                StrSQL.AppendLine(" And Id = " & Agro_SQL_SaveNum(Id) & " ")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Contatore <> 0 Then
                StrSQL.AppendLine(" AND Id_Contatore = " & Agro_SQL_SaveNum(Id_Contatore) & " ")
            End If

            If DataLetturaInizio <> AGRODATAINIZIO Then
                StrSQL.AppendLine(" AND DataLettura >= " & Agro_SQL_SaveDate(DataLetturaInizio) & " ")
            End If

            If DataLetturaFine <> AGRODATAFINE Then
                StrSQL.AppendLine(" AND DataLettura <= " & Agro_SQL_SaveDate(DataLetturaFine) & " ")
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
