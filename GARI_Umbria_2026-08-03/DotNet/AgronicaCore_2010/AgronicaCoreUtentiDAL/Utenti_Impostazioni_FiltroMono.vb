Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider
Imports System.Text
Imports AgronicaCoreModelsSTD.utente






'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Utenti_Impostazioni_FiltroMono_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(
                        ByVal Impostazione_Cod As Integer,
                        ByVal ID_0 As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Veg_Cod = 0           =>  tutti i record
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '//////////////////////////////////////////////////////////////////////
                    '//////////////////////////////////////////////////////////////////////


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT      *  ")
                    StrSQL.Append(" FROM        Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append(" WHERE     1=1  ")
                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
                    End If

                    If objParametri.UsernameOperazione <> "" Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "')   ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If ID_0 <> 0 Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.ID_0 = " & Agro_SQL_SaveNum(ID_0) & ")   ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY  Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT      Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod,   ")
                    StrSQL.Append("             Utenti_Impostazioni_FiltroMono.ID_0, Utenti_Impostazioni.Impostazione_Valore_1, Utenti_Impostazioni.Impostazione_Valore_2, ")
                    StrSQL.Append("             Utenti_Impostazioni.Impostazione_Valore_3, Utenti_Impostazioni.Impostazione_Valore_4,Utenti_Impostazioni_FiltroMono.Str_0  ")
                    StrSQL.Append(" FROM        Utenti_Impostazioni_FiltroMono ")
                    StrSQL.Append(" INNER JOIN  Utenti_Impostazioni ON ")
                    StrSQL.Append("             Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser ")
                    StrSQL.Append("             AND  Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName ")
                    StrSQL.Append("             AND Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
                    StrSQL.Append(" WHERE     1=1  ")
                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")
                    End If

                    If objParametri.UsernameOperazione <> "" Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "')   ")
                    End If

                    If Impostazione_Cod <> 0 Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
                    End If

                    If ID_0 <> 0 Then
                        StrSQL.Append(" AND     (Utenti_Impostazioni_FiltroMono.ID_0 = " & Agro_SQL_SaveNum(ID_0) & ")   ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.Username, Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '                    '
                    '


            End Select


            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function


    '##############################################################################################
    'a differenza della 1, può filtrare su una username passata e non su quella dell'objparametri
    Public Function Leggi2(ByVal Username_1Utente_o_2SuperUser As Integer,
                            ByVal UsernameUtente As String,
                                ByVal Impostazione_Cod As Integer,
                                ByVal ID_0 As Integer,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R.Leggi2()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT      Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod,   ")
            StrSQL.Append("             Utenti_Impostazioni_FiltroMono.ID_0, Utenti_Impostazioni.Impostazione_Valore_1, Utenti_Impostazioni.Impostazione_Valore_2, ")
            StrSQL.Append("             Utenti_Impostazioni.Impostazione_Valore_3, Utenti_Impostazioni.Impostazione_Valore_4, Utenti_Impostazioni_FiltroMono.Str_0  ")
            StrSQL.Append(" FROM        Utenti_Impostazioni_FiltroMono ")
            StrSQL.Append(" INNER JOIN  Utenti_Impostazioni ON ")
            StrSQL.Append("             Utenti_Impostazioni.Piva_SuperUser = Utenti_Impostazioni_FiltroMono.Piva_SuperUser ")
            StrSQL.Append("             AND  Utenti_Impostazioni.UserName = Utenti_Impostazioni_FiltroMono.UserName ")
            StrSQL.Append("             AND Utenti_Impostazioni.Impostazione_Cod = Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
            StrSQL.Append(" WHERE     Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Username_1Utente_o_2SuperUser = 2 Then
                StrSQL.Append(" AND Utenti_Impostazioni_FiltroMono.Username = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            Else
                If UsernameUtente = "" Then
                    StrSQL.Append(" AND     Utenti_Impostazioni_FiltroMono.Username = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'   ")
                Else
                    StrSQL.Append(" AND     Utenti_Impostazioni_FiltroMono.Username = '" & Agro_SQL_SaveText(UsernameUtente) & "'   ")
                End If
            End If

            If Impostazione_Cod <> 0 Then
                StrSQL.Append(" AND     Utenti_Impostazioni_FiltroMono.Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & "   ")
            End If

            If ID_0 <> 0 Then
                StrSQL.Append(" AND     Utenti_Impostazioni_FiltroMono.ID_0 = " & Agro_SQL_SaveNum(ID_0) & "   ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Utenti_Impostazioni_FiltroMono.Piva_SuperUser, Utenti_Impostazioni_FiltroMono.Username, Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")

            End If

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    '########################################################
    Public Function RecuperaFiltroSQLMatPrime_from_AreaGIAS(ByVal AreaGIAS As Integer,
                                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                            ) As String

        Dim dt As DataTable
        Dim FiltroSQL As String = ""

        dt = Leggi2(2, "",
                    enum_Impostazioni_Utenti.SuperUser_FiltroSQL_MateriePrime,
                    AreaGIAS,
                     "", "",
                     objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            FiltroSQL = dt.Rows(0).Item("Str_0")
        End If

        Return FiltroSQL

    End Function

    Public Function LeggiScalare(username As String, impostazioni As IEnumerable(Of Integer), objParametri As AgronicaCoreParametri) As IEnumerable(Of Utente_Impostazioni)
        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_R.LeggiScalare()"
        Dim strSql As New StringBuilder With {.Length = 0}
        If impostazioni.Count = 0 Then
            Return New List(Of Utente_Impostazioni)
        End If

        Dim tipologia = GetTipologiaCod(username, objParametri).ToString
        Dim filtroUtenti = "( " & {username, tipologia, objParametri.SuperUserUsername}.
            Select(Function(str) "'" & str & "'").
            Aggregate(Function(acc, str) acc & ", " & str) & " )"
        Dim filtroCodici = "( " & impostazioni.Select(Function(x) x.ToString).
            Aggregate(Function(acc, str) acc & ", " & str) & " )"
        Try
            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("  Utenti_Impostazioni_FiltroMono.UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod, ")
            strSql.AppendLine("   STRING_AGG(Utenti_Impostazioni_FiltroMono.ID_0, '|') as ID_0, ")
            strSql.AppendLine("   STRING_AGG(Utenti_Impostazioni_FiltroMono.Str_0, '|') as Str_0 ")
            strSql.AppendLine(" FROM Utenti_Impostazioni_FiltroMono ")
            strSql.AppendLine(" WHERE Utenti_Impostazioni_FiltroMono.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.AppendLine("       AND Utenti_Impostazioni_FiltroMono.Username in " & Agro_SQL_Save_Clausola_IN(filtroUtenti, True) & " ")
            strSql.AppendLine("       AND Utenti_Impostazioni_FiltroMono.Impostazione_Cod in " & Agro_SQL_Save_Clausola_IN(filtroCodici) & " ")
            strSql.AppendLine(" GROUP BY Utenti_Impostazioni_FiltroMono.UserName, Utenti_Impostazioni_FiltroMono.Impostazione_Cod ")
            strSql.AppendLine(" UNION ")
            strSql.AppendLine(" SELECT 'DEFAULT' AS UserName, Guida_Impostazioni.Impostazione_Cod, Guida_Impostazioni.Valore_Default as ID_0, Guida_Impostazioni.Valore_Default as Str_0 ")
            strSql.AppendLine(" FROM Guida_Impostazioni ")
            strSql.AppendLine(" WHERE Guida_Impostazioni.Validita_Inizio <= getdate() AND Guida_Impostazioni.Validita_Fine >= getdate() ")
            strSql.AppendLine("       AND Guida_Impostazioni.Impostazione_Cod in " & Agro_SQL_Save_Clausola_IN(filtroCodici) & " ")

            Dim dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            Dim values = dt.Select.
                GroupBy(Function(r) r("Impostazione_Cod")).
                Select(Function(gr)
                           If gr.FirstOrDefault(Function(row) row("username") = username) IsNot Nothing Then
                               Return gr.FirstOrDefault(Function(row) row("username") = username)
                           ElseIf gr.FirstOrDefault(Function(row) row("username") = tipologia) IsNot Nothing Then
                               Return gr.FirstOrDefault(Function(row) row("username") = tipologia)
                           ElseIf gr.FirstOrDefault(Function(row) row("username") = objParametri.SuperUserUsername) IsNot Nothing Then
                               Return gr.FirstOrDefault(Function(row) row("username") = objParametri.SuperUserUsername)
                           Else
                               Return gr.FirstOrDefault(Function(row) row("username") = "DEFAULT")
                           End If
                       End Function).
                Select(Function(row) New Utente_Impostazioni With {
                    .Impostazione_Cod = row("Impostazione_Cod"),
                    .Valore = row("ID_0"),
                    .Username = row("username")
                }).ToList()
            Return values
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Private Function GetTipologiaCod(username As String, objPUtenti As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim objUtentiDal As New Utenti_Read
        Return objUtentiDal.Leggi2(username, String.Empty, String.Empty, objPUtenti).
            Select.Select(Function(row) If(IsDBNull(row("Tipologia_Cod")), 0, CInt(row("Tipologia_Cod")))).
            DefaultIfEmpty(0).
            First
    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§

Public Class Utenti_Impostazioni_FiltroMono_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella(ByVal Username As String, _
                               ByVal Impostazione_Cod As Integer, _
                               ByVal xFiltroAggiuntivo As String, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                               ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Utenti_Impostazioni_FiltroMono ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE  ")
                StrSQL.Append(" FROM Utenti_Impostazioni_FiltroMono ")
            End If

            StrSQL.Append(" WHERE    (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

            StrSQL.Append(" AND     (Username = '" & Agro_SQL_SaveText(Username) & "')   ")

            If Impostazione_Cod <> 0 Then
                StrSQL.Append(" AND     (Impostazione_Cod = " & Agro_SQL_SaveNum(Impostazione_Cod) & ")   ")
            End If

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

    ''' <summary>
    ''' Salva un nuovo record sulla tabella <tt>Utenti_Impostazioni_FiltroMono</tt>
    ''' per l'utente che ha effettuato la login.
    ''' Per specificare l'utente per cui salvare il record fare riferimento a
    ''' <see cref="Scrivi(String, Integer, Integer, String, Date, Date, ByRef AgronicaCoreParametri)"/>
    ''' </summary>
    ''' <param name="Impostazione_Cod"></param>
    ''' <param name="ID_0"></param>
    ''' <param name="Str_0"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Scrivi(
        ByVal Impostazione_Cod As Integer,
        ByVal ID_0 As Integer,
        ByVal Str_0 As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Return Scrivi(
            objParametri.UtenteUsername, Impostazione_Cod,
            ID_0, Str_0,
            Validita_Inizio, Validita_Fine,
            objParametri
        )
    End Function


    'aggiunto STR_0 valorizzare come "" se nn si vuole salvare
    ''' <summary>
    ''' Salva un nuovo record sulla tabella <tt>Utenti_Impostazioni_FiltroMono</tt>
    ''' per l'utente specificato.
    ''' </summary>
    ''' <param name="username">Username dell'utente per cui salvare il record. Default: <tt>objParametri.UtenteUsername</tt></param>
    ''' <param name="Impostazione_Cod"></param>
    ''' <param name="ID_0"></param>
    ''' <param name="Str_0"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>True se l'operazione è andata a buon fine, False altrimenti</returns>
    Public Function Scrivi(
        ByVal username As String,
        ByVal Impostazione_Cod As Integer,
        ByVal ID_0 As Integer,
        ByVal Str_0 As String,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W.Scrivi()"
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO Utenti_Impostazioni_FiltroMono ")
            StrSQL.AppendLine(" (  Piva_SuperUser, UserName, Impostazione_Cod, ID_0, Str_0, ")
            StrSQL.AppendLine("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            StrSQL.AppendLine(" VALUES ('" + Agro_SQL_SaveText(objParametri.PivaSuperUser) + "', ")

            If username = "" Then
                StrSQL.AppendLine("'" + Agro_SQL_SaveText(Trim(objParametri.UtenteUsername)) + "', ")
            Else
                StrSQL.AppendLine("'" + Agro_SQL_SaveText(username.Trim().ToLower) + "', ")
            End If

            StrSQL.AppendLine("" + Agro_SQL_SaveNum(Impostazione_Cod) + ", ")
            StrSQL.AppendLine("" + Agro_SQL_SaveNum(ID_0) + ", ")
            StrSQL.AppendLine("'" + Agro_SQL_SaveText(Str_0) + "', ")

            StrSQL.AppendLine("" + Agro_SQL_SaveNum(0) + ", ")
            StrSQL.AppendLine("" + Agro_SQL_SaveText("NULL") + ", ")
            StrSQL.AppendLine(Agro_SQL_SaveDate(Date.Now) + ", ")
            StrSQL.AppendLine(Agro_SQL_SaveDate(Date.Now) & ", ")
            StrSQL.AppendLine("'" + Agro_SQL_SaveText(objParametri.UsernameOperazione) + "', ")
            StrSQL.AppendLine("'" + Agro_SQL_SaveText(Trim(objParametri.UsernameOperazione)) + "', ")
            StrSQL.AppendLine(Agro_SQL_SaveDate(Validita_Inizio) + ", ")
            StrSQL.AppendLine(Agro_SQL_SaveDate(Validita_Fine) + " ")
            StrSQL.AppendLine(" ) ")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function


    Public Function ApplicaProfilo( _
                            ByVal TipologiaCod As String, _
                            ByVal Username As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Utenti_Impostazioni_FiltroMono ")
            StrSQL.Append(" (  Piva_SuperUser, UserName, Impostazione_Cod, ID_0, ")
            StrSQL.Append("  inviato, datainvio, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            StrSQL.Append(" SELECT Piva_SuperUser, '" & Agro_SQL_SaveText(Username) & "' , Impostazione_Cod, ID_0, ")
            StrSQL.Append(" inviato, datainvio, GETDATE(), GETDATE(), Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine  ")
            StrSQL.Append(" FROM Utenti_Impostazioni_FiltroMono ")
            StrSQL.Append(" WHERE UserName= '" & Agro_SQL_SaveText(TipologiaCod) & "' ")

            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function



    Public Function CancellaBySTR_0(ByVal Str_0 As String, _
                               ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                              ) As Boolean

        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Impostazioni_FiltroMono_W.CancellaBySTR_0()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Utenti_Impostazioni_FiltroMono ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE  ")
                StrSQL.Append(" FROM Utenti_Impostazioni_FiltroMono ")
            End If

            StrSQL.Append(" WHERE    (Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "')   ")

            StrSQL.Append(" AND     (Str_0 = '" & Agro_SQL_SaveText(Str_0) & "')   ")


            StrSQL.Append(" AND     (UserName = '" & Agro_SQL_SaveText(Trim(objParametri.UtenteUsername)) & "')   ")



            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return xRisp

    End Function

End Class