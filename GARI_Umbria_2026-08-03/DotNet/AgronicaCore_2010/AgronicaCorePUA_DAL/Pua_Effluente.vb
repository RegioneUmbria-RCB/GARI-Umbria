Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Pua_Effluente_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal Regolamento_Cod As Integer,
                          ByVal PUA_Cod As Integer,
                          ByVal Eff_Cod As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "Pua_Effluente_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.AppendLine(" SELECT * ")
            strSql.AppendLine(" FROM PUA_Effluente ")
            strSql.AppendLine(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Regolamento_Cod <> 0 Then
                strSql.AppendLine(" AND Regolamento_Cod=" & Agro_SQL_SaveNum(Regolamento_Cod) & " ")
            End If

            If PUA_Cod <> 0 Then
                strSql.AppendLine(" AND PUA_Cod=" & Agro_SQL_SaveNum(PUA_Cod) & " ")
            End If

            If Eff_Cod <> 0 Then
                strSql.AppendLine(" AND Eff_Cod=" & Agro_SQL_SaveNum(Eff_Cod) & " ")
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


'#################################################################
'#################################################################
'#################################################################

Public Class Pua_Effluente_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByRef ID As Integer,
                           ByVal PUA_Cod As Integer,
                           ByVal Eff_Cod As Integer,
                           ByVal Azoto_Qta As Decimal,
                           ByVal Azoto_Titoli As Decimal,
                           ByVal Capacita_stoccaggio As Decimal,
                           ByVal Carico As Decimal,
                           ByVal Giorni_stoccaggio As Integer,
                           ByVal Riempimento As Decimal,
                           ByVal Regolamento_cod As Integer,
                           ByVal Tipo_Allevamento As Integer,
                           ByVal Perc_Zootecnico As Decimal,
                           ByVal Matrice_Prevalente As Integer,
                           ByVal Flag_ProvenienzaEsterna As Integer,
                           ByVal Validita_Fine As DateTime,
                           ByVal Validita_Inizio As DateTime,
                           ByVal Data_Inizio_Divieto As DateTime,
                           ByVal Data_Fine_Divieto As DateTime,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean


        Const nomeRoutine = "Pua_Effluente_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If ID = 0 Then
                Dim ObjSequenze As New AgronicaCoreDataProvider.Agro_Sequenze
                'ID = ObjSequenze.Agronica_SequenzaTabelle_NuovoID("PUA_Effluente", objParametri)
                'Lavez - 12/07/2024 - normalizzazione chiamate a stack counter
                ID = ObjSequenze.NuovoId_Tabella("PUA_Effluente", 0, AgronicaCoreDataProvider.CostantiPersonalizzate.UpperBoundTabelle_Per_SequenzaTabelle_Topcode, objParametri)
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
            strSql.AppendLine(" INSERT INTO PUA_Effluente ")

            strSql.AppendLine("              (")

            strSql.AppendLine("   [Piva_SuperUser] ")
            strSql.AppendLine("  ,[PUA_Cod] ")
            strSql.AppendLine("  ,[Eff_Cod] ")
            strSql.AppendLine("  ,[Azoto_Qta] ")
            strSql.AppendLine("  ,[Azoto_Titoli] ")
            strSql.AppendLine("  ,[Capacita_stoccaggio] ")
            strSql.AppendLine("  ,[Carico] ")
            strSql.AppendLine("  ,[Giorni_stoccaggio] ")
            strSql.AppendLine("  ,[Riempimento] ")
            strSql.AppendLine("  ,[Regolamento_cod] ")
            strSql.AppendLine("  ,[Tipo_Allevamento] ")
            strSql.AppendLine("  ,[Perc_Zootecnico] ")
            strSql.AppendLine("  ,[Matrice_Prevalente] ")
            strSql.AppendLine("  ,[Flag_ProvenienzaEsterna] ")
            strSql.AppendLine("  ,Data_Inizio_Divieto,    Data_Fine_Divieto ")

            strSql.AppendLine("              ,Inviato, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Validita_Inizio,    Validita_Fine ")
            strSql.AppendLine("              ,ID ")
            strSql.AppendLine("              ) ")

            strSql.AppendLine(" VALUES ( ")

            strSql.AppendLine(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            strSql.AppendLine(", " & Agro_SQL_SaveNum(Eff_Cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Azoto_Qta) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Azoto_Titoli) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Capacita_stoccaggio) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Carico) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Giorni_stoccaggio) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Riempimento) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Regolamento_cod) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Tipo_Allevamento) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Perc_Zootecnico) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Matrice_Prevalente) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveNum(Flag_ProvenienzaEsterna) & " ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(Data_Inizio_Divieto) & "  ")
            strSql.AppendLine(", " & Agro_SQL_SaveDate(Data_Fine_Divieto) & "  ")

            strSql.AppendLine("         , 0  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveNum(ID) & "  ")

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
                             ByVal Eff_Cod As Integer,
                             ByVal Regolamento_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "Pua_Effluente_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE PUA_Effluente ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE  Inviato >= 0")
                strSql.AppendLine(" AND   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     PUA_Effluente ")
                strSql.AppendLine(" WHERE   Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.AppendLine("  AND   PUA_Cod = " & Agro_SQL_SaveNum(PUA_Cod) & " ")

            End If


            If Eff_Cod <> 0 Then
                strSql.AppendLine("  AND   Eff_Cod = " & Agro_SQL_SaveNum(Eff_Cod) & " ")
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
