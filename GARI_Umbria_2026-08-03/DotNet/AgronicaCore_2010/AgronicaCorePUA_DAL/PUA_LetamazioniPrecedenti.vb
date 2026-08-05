Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider
Public Class PUA_LetamazioniPrecedenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Regolamento_Cod As Integer, ByVal PUA_Cod As Integer,
                            ByVal piva As String, ByVal sa_cod As Integer, ByVal appezza As Integer, ByVal id_reg As Integer, ByVal progetto_cod As Integer,
                                ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "PUA_LetamazioniPrecedenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM PUA_LetamazioniPrecedenti ")
            strSql.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND Regolamento_Cod=" & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If PUA_Cod <> 0 Then
                strSql.AppendLine(" AND PUA_Cod=" & Agro_SQL_SaveNum(PUA_Cod) & " ")
            End If

            If piva <> "" Then
                strSql.AppendLine(" AND  Piva = '" & Agro_SQL_SaveText(piva) & "' ")
            End If

            If sa_cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod=" & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If appezza <> 0 Then
                strSql.AppendLine(" AND appezza=" & Agro_SQL_SaveNum(appezza) & " ")
            End If

            If id_reg <> 0 Then
                strSql.AppendLine(" AND id_reg=" & Agro_SQL_SaveNum(id_reg) & " ")
            End If

            If progetto_cod <> 0 Then
                strSql.AppendLine(" AND progetto_cod=" & Agro_SQL_SaveNum(progetto_cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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


Public Class PUA_LetamazioniPrecedenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef ID As Integer,
                           ByVal Regolamento_cod As Integer,
                           ByVal PUA_Cod As Integer,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Progetto_Cod As Integer,
                           ByVal Eff_Cod As Integer,
                           ByVal Id_Fre As Integer,
                           ByVal Udm_Cod As Integer,
                           ByVal Qta As Decimal,
                           ByVal N_Titolo As Decimal,
                           ByVal Validita_Fine As DateTime,
                           ByVal Validita_Inizio As DateTime,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "PUA_LetamazioniPrecedenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try



            If ID = 0 Then
                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze

                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                'ID = ObjSequenze.Agronica_SequenzaTabelle_NuovoID("pua_letamazioniprecedenti", objParametri)
                ID = ObjSequenze.NuovoId_Tabella("pua_letamazioniprecedenti", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
            End If


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
            strSql.AppendLine(" INSERT INTO PUA_LetamazioniPrecedenti ")
            strSql.AppendLine("             (")
            strSql.AppendLine("  [Piva_SuperUser] ")
            strSql.AppendLine("  ,[ID] ")
            strSql.AppendLine("  ,[Piva] ")
            strSql.AppendLine("  ,[Sa_Cod] ")
            strSql.AppendLine("  ,[Appezza] ")
            strSql.AppendLine("  ,[Id_Reg] ")
            strSql.AppendLine("  ,[Progetto_Cod] ")
            strSql.AppendLine("  ,[Pua_Cod] ")
            strSql.AppendLine("  ,[Regolamento_CoD] ")
            strSql.AppendLine("  ,[Eff_Cod] ")
            strSql.AppendLine("  ,[Id_Fre] ")
            strSql.AppendLine("  ,[Udm_Cod] ")
            strSql.AppendLine("  ,[Qta] ")
            strSql.AppendLine("  ,[N_Titolo] ")

            strSql.AppendLine("              ,Inviato,  ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Validita_Inizio,    Validita_Fine ")

            strSql.AppendLine("                ) ")

            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ID) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "'")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Regolamento_cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Eff_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Fre) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Udm_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Qta) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(N_Titolo) & " ")

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
                             ByVal Regolamento_Cod As Integer,
                             ByVal Piva As String, ByVal Sa_Cod As Integer, ByVal Appezza As Integer, ByVal Id_Reg As Integer, ByVal Progetto_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PUA_LetamazioniPrecedenti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE PUA_LetamazioniPrecedenti ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0")
                strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     PUA_LetamazioniPrecedenti ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If

            If PUA_Cod <> 0 Then
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")
            End If
            If Regolamento_Cod <> 0 Then
                strSql.AppendLine("  AND   Regolamento_Cod = " & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If Piva <> "" Then
                strSql.AppendLine("  AND  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine("  AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If
            If Appezza <> 0 Then
                strSql.AppendLine("  AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            If Id_Reg <> 0 Then
                strSql.AppendLine("  AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If
            If Progetto_Cod <> 0 Then
                strSql.AppendLine("  AND   Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
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

    Public Function CancellaByID(ByVal ID As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "PUA_LetamazioniPrecedenti_W.CancellaByID()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.AppendLine(" UPDATE PUA_LetamazioniPrecedenti ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0")
                strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     PUA_LetamazioniPrecedenti ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            End If


            strSql.AppendLine("  AND   ID = " & Agro_SQL_SaveNum(ID) & " ")

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
