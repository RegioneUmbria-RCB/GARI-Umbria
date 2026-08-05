Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Pua_TrattamentoEffluenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal PUA_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "Pua_TrattamentoEffluenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM PUA_TrattamentoEffluenti ")
            strSql.AppendLine(" WHERE 1=1 ")

            strSql.AppendLine(" AND Piva_SuperUser = '" & objParametri.PivaSuperUser & "' ")


            If PUA_Cod <> 0 Then
                strSql.AppendLine(" AND PUA_Cod=" & PUA_Cod & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.AppendLine(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.AppendLine(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

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


'#################################################################
'#################################################################
'#################################################################

Public Class Pua_TrattamentoEffluenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal PUA_Cod As Integer,
                           ByVal Cons_Cod As Integer,
                           ByVal Piva_SuperUser As String,
                           ByVal Tratt_Cod As Integer,
                           ByVal Compostato_Maturato As Decimal,
                           ByVal Efficienza_Cod As Integer,
                           ByVal Inerte_Aggiunto As Decimal,
                           ByVal Liq_Trattati As Decimal,
                           ByVal Regolamento_Cod As Integer,
                           ByVal Validita_Fine As DateTime,
                           ByVal Validita_Inizio As DateTime,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean


        Const nomeRoutine = "Pua_TrattamentoEffluenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If


            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO PUA_TrattamentoEffluenti ")

            strSql.AppendLine("              (")

            strSql.AppendLine("   [Piva_SuperUser] ")
            strSql.AppendLine("  ,[PUA_Cod] ")
            strSql.AppendLine("  ,[Cons_Cod] ")
            strSql.AppendLine("  ,[Tratt_Cod] ")
            strSql.AppendLine("  ,[Compostato_Maturato] ")
            strSql.AppendLine("  ,[Efficienza_Cod] ")
            strSql.AppendLine("  ,[Inerte_Aggiunto] ")
            strSql.AppendLine("  ,[Liq_Trattati] ")
            strSql.AppendLine("  ,[Regolamento_cod], ")

            strSql.AppendLine("              Inviato, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("              ) ")

            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine(" '" & Agro_SQL_SaveText(Piva_SuperUser) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Cons_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Tratt_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Compostato_Maturato) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Efficienza_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Inerte_Aggiunto) & " ")
            strSql.AppendLine("," & Agro_SQL_SaveNum(Liq_Trattati) & " ")
            strSql.AppendLine("," & Agro_SQL_SaveNum(Regolamento_Cod) & " ")

            strSql.AppendLine("         , 0  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine(") ")

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

    Public Function Cancella(ByVal PUA_Cod As Integer,
                             ByVal Cons_Cod As Integer,
                             ByVal Tratt_Cod As Integer,
                             ByVal Regolamento_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "Pua_TrattamentoEffluenti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE PUA_TrattamentoEffluenti ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0")
                strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     PUA_TrattamentoEffluenti ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            End If


            If Cons_Cod <> 0 Then
                strSql.AppendLine("  AND   Cons_Cod = " & Agro_SQL_SaveNum(Cons_Cod) & " ")
            End If

            If Tratt_Cod <> 0 Then
                strSql.AppendLine("  AND   Tratt_Cod = " & Agro_SQL_SaveNum(Tratt_Cod) & " ")
            End If

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine("  AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            '---------------------------------------------

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

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
