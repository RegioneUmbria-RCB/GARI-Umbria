Imports System.Data.OleDb
Imports System.Text
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDTOStd.Identity
Imports AgronicaCoreVisibilitaStd

Public Class Utenti_Profili_Read
    Inherits AgronicaCoreDataProvider.DataProvider
    
    Private Function StringListAggregator(utenti As IEnumerable(Of String), mettiApici As Boolean) As List(Of String)
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim uu = utenti.AsParallel.
            DefaultIfEmpty(String.Empty).
            Select(Function(str) If(mettiApici, "'" & str & "'", str)).
            Select(Function(str, i) New With {.name = str, .index = i}).
            GroupBy(Function(u) u.index \ 10_000).
            Select(Function(batch) batch.Aggregate(prepareFilter).name).
            ToList ' lista di stringhe aggregate
        Return uu
    End Function

    ''' <param name="usernames">Usernames degli utenti interessati. Se la collezione contiene piu' di 10.000 username, quelli in eccesso verranno ignorati.</param>
    Public Function LeggiMassivo(
        ByVal usernames As IEnumerable(Of String),
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim xIn = StringListAggregator(usernames, usernames.Count > 1).FirstOrDefault
        Return LeggiMassivo(xIn, xFiltroAggiuntivo, xOrderBy, objParametri)
    End Function

    ''' <param name="usernames">Usernames degli utenti interessati gia' formattati per essere inseriti nella clausola IN.
    ''' es. <tt>'user1', 'user2', 'user3', ...</tt></param>
    Public Function LeggiMassivo(
        ByVal usernames As String,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Read.LeggiMassivo()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Try
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM    Utenti_Profili (NOLOCK) ")
            StrSQL.AppendLine(" WHERE 1=1 ")
            If usernames.Contains(",") Then
                Dim conApici = usernames.Contains("',")
                StrSQL.AppendLine(" AND (Utente in ( " & Agro_SQL_Save_Clausola_IN(usernames, valoriStringa:=Not conApici, creaParametriSql:=Not conApici) & " )) ")
            Else
                usernames = usernames.Replace("'", "")
                StrSQL.AppendLine(" AND (Utente = '" & Agro_SQL_SaveText(usernames) & "' ) ")
            End If

            If objParametri.SuperUserUsername <> "" Then
                StrSQL.AppendLine(" AND (Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "') ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND (" & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & ") ")
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function Leggi(
       ByVal Username_Utente As String,
       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
       Optional xFiltroAggiuntivo As String = "",
       Optional xOrderBy As String = ""
   ) As DataTable
        Return Leggi(Username_Utente, enum_Id_Servizio.GiasOnline, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, xFiltroAggiuntivo, xOrderBy, objParametri)
    End Function

    Public Function Leggi(
        ByVal Username_Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Read.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM    Utenti_Profili (NOLOCK) ")
                    StrSQL.AppendLine(" WHERE 1=1 ")

                    If objParametri.SuperUserUsername <> "" Then
                        StrSQL.AppendLine(" AND (Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "') ")
                    End If

                    If Username_Utente <> "" Then
                        StrSQL.AppendLine(" AND (Utente = '" & Agro_SQL_SaveText(Username_Utente) & "') ")
                    End If

                    If Id_Servizio <> 0 Then
                        StrSQL.AppendLine(" AND (Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & ") ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '

            End Select

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function Leggi_2(
        ByVal Username_Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Read.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder With {.Length = 0}
        Dim DT As DataTable
        Try
            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM    Utenti_Profili ")
                    StrSQL.AppendLine(" WHERE 1=1 ")

                    If Username_Utente <> "" Then
                        StrSQL.AppendLine(" AND Utente = '" & Agro_SQL_SaveText(Username_Utente) & "' ")
                    End If

                    If Id_Servizio <> 0 Then
                        StrSQL.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select

            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return DT
    End Function

    Public Function Leggi_FiltroUtenteSQL(
        ByVal Username_Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As String
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Read.Leggi_FiltroUtenteSQL()"
        Dim DT As DataTable
        Try
            DT = Leggi(Username_Utente, Id_Servizio, enumSelezioneVariabile.Selezione_TabellaCompleta,
                       xFiltroAggiuntivo, xOrderBy, objParametri)

            If Not IsNothing(DT) OrElse DT.Rows.Count <> 0 Then
                Return DT.Rows(0).Item("Descrizione_2")
            Else
                Return ""
            End If
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Is_UtenteGiasLan(xFiltroAggiuntivo As String, ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean
        Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Profili_Read.Is_UtenteGiasLan()"
        Dim DT As DataTable
        Try
            DT = Leggi(objParametri_Utenti.UtenteUsername, enum_Id_Servizio.GiasLAN,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       xFiltroAggiuntivo, "", objParametri_Utenti)

            If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
                Return True
            End If
        Catch ex As Exception
            Scrivi_LOG(objParametri_Utenti, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return false
    End Function

    ''' <summary>
    ''' Legge le differenti visibilita' basandosi su Descrizione_1. Considera solo i record con Id_Servizio = 5 (GiasOnline)
    ''' </summary>
    Public Function GetDistinctVisibility(usernames As IEnumerable(Of String), objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.GetDistinctVisibility()"
        Dim xRisp As DataTable
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xIn = StringListAggregator(usernames, True).FirstOrDefault
        Try
            StrSQL.AppendLine(" SELECT COUNT(DISTINCT Descrizione_1), Descrizione_1 ")
            StrSQL.AppendLine(" FROM Utenti_Profili (NOLOCK) ")
            StrSQL.AppendLine(" WHERE Id_Servizio = " & CInt(enum_Id_Servizio.GiasOnline))
            StrSQL.AppendLine("     AND Utente in ( " & xIn & " ) ")
            StrSQL.AppendLine(" GROUP BY Descrizione_1 ")

            xRisp = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function


    ''' <returns>Lista di stringhe contenente le pive dei capostipiti della gerarchia imprese in visibilita</returns>
    Public Function GetVisibilityAncestors(username As String, objUtenti As AgronicaCoreParametri) As List(Of String)
        Dim DTProfilo = Leggi(
            username, CInt(AgronicaCoreDataProvider.TipiEnumerativi.enum_Id_Servizio.GiasOnline),
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            xFiltroAggiuntivo:="", xOrderBy:="",
            objUtenti
        )
        If DTProfilo.Rows.Count = 1 AndAlso DTProfilo.Rows(0).Item("Descrizione_1") <> "" Then
            Dim xmlString = CType(DTProfilo.Rows(0).Item("Descrizione_1"), String)
            If Not String.IsNullOrEmpty(xmlString) Then
                Return Regex.Matches(xmlString, "\""(([a-z0-9A-Z#]){11,})\""+", RegexOptions.None, TimeSpan.FromSeconds(3)).
                    Cast(Of RegularExpressions.Match)().
                    Select(Function(m) m.Value.Replace("""", "")).ToList()
            End If
        End If
        Return New List(Of String)
    End Function

    ''' <summary>
    ''' Controlla in base ai flags su Utenti_Profili se l'utente ha visibilita' totale.
    ''' Questa funzione rimane approssimativa, ritornando true solo dove e' la visibilita' totale e' ovvia.
    ''' Nel caso in cui sia necessario eseguire il calcolo della visibilita' per sapere se essa e' totale o meno,
    ''' questa funzione ritorna false, anche se l'utente potrebbe avere visibilita' totale.
    ''' Tale logica e' stata scelta per evitare di eseguire query complesse ogni volta serva l'informazione.
    ''' </summary>
    ''' <param name="sqlFilter">Filtro sql sulle imprese</param>
    ''' <param name="filterProcedures">Se il filtro sulle pratiche e' attivo o meno</param>
    ''' <param name="filterOperator">Operatore utilizzato per applicare il filtro pratiche</param>
    ''' <returns></returns>
    Public Function HasFullVisibility(sqlFilter As String, filterProcedures As Boolean, filterOperator As AgronicaCoreVisibilitaStd.PraticheFilterOperator) As Boolean
        Dim allBusiness = String.IsNullOrWhiteSpace(sqlFilter)
        If filterProcedures then
            Return allBusiness AndAlso filterOperator = AgronicaCoreVisibilitaStd.PraticheFilterOperator.Or
        Else
            Return allBusiness
        End If
    End function

    Public Function HasFullVisibility(username As String, objUtenti As AgronicaCoreParametri, optional idServizio As enum_Id_Servizio = enum_Id_Servizio.GiasOnline) As Boolean
        Dim DTProfilo = Leggi(
            username, CInt(idServizio),
            enumSelezioneVariabile.Selezione_TabellaCompleta,
            xFiltroAggiuntivo:="", xOrderBy:="",
            objUtenti
        )
        If DTProfilo.Rows.Count = 1 Then
            Dim filtroSQL = DTProfilo.Rows(0).Item("Descrizione_2")
            Dim filtroPratiche = DTProfilo.Rows(0).Item("filtro_pratiche_attivo")
            Dim operatoreStr = DTProfilo.Rows(0).Item("Operatore_Filtri")
            Dim operatore = If(operatoreStr.Equals("AND", StringComparison.OrdinalIgnoreCase), PraticheFilterOperator.And, PraticheFilterOperator.Or)
            Return HasFullVisibility(filtroSQL, filtroPratiche, operatore)
        Else
            Return False
        End If
    End Function

End Class


Public Class Utenti_Profili_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    Private Function StringListAggregator(utenti As IEnumerable(Of String), mettiApici As Boolean) As List(Of String)
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim uu = utenti.AsParallel.
            DefaultIfEmpty(String.Empty).
            Select(Function(str) If(mettiApici, "'" & str & "'", str)).
            Select(Function(str, i) New With {.name = str, .index = i}).
            GroupBy(Function(u) u.index \ 10_000).
            Select(Function(batch) batch.Aggregate(prepareFilter).name).
            ToList ' lista di stringhe aggregate
        Return uu
    End Function

    Public Function Scrivi(
        ByVal Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal Descrizione_1 As String,
        ByVal Descrizione_2 As String,
        ByVal Codice_1 As Int32,
        ByVal Codice_2 As Int32,
        ByVal Validita_Inizio As Date,
        ByVal Validita_Fine As Date,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.Scrivi()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine("INSERT INTO Utenti_Profili(       ")
            StrSQL.AppendLine("                    Utente, Utente_Profilo, Id_Servizio,  ")
            StrSQL.AppendLine("                    Descrizione_1, Descrizione_2, Codice_1, Codice_2,  ")
            StrSQL.AppendLine("                    Inviato,             DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,      Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,     Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(Utente) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Descrizione_1) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Descrizione_2) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Codice_1) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Codice_2) & " ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <param name="utenti">Collezione di username. Se la collezione contiene piu' di 10.000 username, quelli in eccesso verranno ignorati.</param>
    ''' <param name="objParametri">objParametri_Utenti</param>
    Public Function ScriviMassivo(
        utenti As IEnumerable(Of String),
        Descrizione_1 As String, Descrizione_2 As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
        Optional Codice_1 As Int32 = 0, Optional Codice_2 As Int32 = 0,
        Optional Id_Servizio As Int32 = AgronicaCoreDataProvider.TipiEnumerativi.enum_Id_Servizio.GiasOnline,
        Optional Validita_Inizio As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAINIZIO,
        Optional Validita_Fine As Date = AgronicaCoreDataProvider.CostantiPersonalizzate.AGRODATAFINE
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.ScriviMassivo()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim conApici = utenti.Count > 1
        Dim xIn = StringListAggregator(utenti, conApici).FirstOrDefault
        Try
            StrSQL.AppendLine(" ; WITH u_cte AS ( ")
            StrSQL.AppendLine("   SELECT UserName, getdate() as today FROM Utenti ")
            StrSQL.AppendLine("   WHERE UserName in ( " & Agro_SQL_Save_Clausola_IN(xIn, valoriStringa:=Not conApici, creaParametriSql:=Not conApici) & " ) ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" INSERT INTO Utenti_profili ")
            StrSQL.AppendLine(" (  Utente, Utente_Profilo, Id_Servizio, ")
            StrSQL.AppendLine("    Descrizione_1, Descrizione_2, Codice_1, Codice_2, ")
            StrSQL.AppendLine("    Inviato,             DataInvio, ")
            StrSQL.AppendLine("    Data_Creazione,      Data_Modifica, ")
            StrSQL.AppendLine("    UserName_Creazione,  UserName_Modifica, ")
            StrSQL.AppendLine("    Validita_Inizio,     Validita_Fine ")
            StrSQL.AppendLine("  ) ")
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   u_cte.UserName as Utente, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' as Utente_Profilo, ")
            StrSQL.AppendLine("   " & Agro_SQL_SaveNum(Id_Servizio) & " as Id_Servizio, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(Descrizione_1) & "' as Descrizione_1, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(Descrizione_2) & "' as Descrizione_2, ")
            StrSQL.AppendLine("   " & Agro_SQL_SaveNum(Codice_1) & " as Codice_1, ")
            StrSQL.AppendLine("   " & Agro_SQL_SaveNum(Codice_2) & " as Codice_2, ")
            StrSQL.AppendLine("   0 as inviato, Null as DataInvio, ")
            StrSQL.AppendLine("   u_cte.today as Data_Creazione, ")
            StrSQL.AppendLine("   u_cte.today as Data_Modifica, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Creazione, ")
            StrSQL.AppendLine("   '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' as Username_Modifica, ")
            StrSQL.AppendLine("   " & Agro_SQL_SaveDate(Validita_Inizio) & " as Validita_Inizio, ")
            StrSQL.AppendLine("   " & Agro_SQL_SaveDate(Validita_Fine) & " as Validita_Fine ")
            StrSQL.AppendLine(" FROM u_cte ")

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Modifica(
        ByVal Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal Descrizione_1 As String,
        ByVal Descrizione_2 As String,
        ByVal Codice_1 As Int32,
        ByVal Codice_2 As Int32,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.Modifica()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine("UPDATE Utenti_Profili SET ")
            StrSQL.AppendLine("       Descrizione_1 = '" & Agro_SQL_SaveText(Descrizione_1) & "'")
            StrSQL.AppendLine("       ,Descrizione_2 = '" & Agro_SQL_SaveText(Descrizione_2) & "'")
            StrSQL.AppendLine("       ,Codice_1 = " & Agro_SQL_SaveNum(Codice_1) & " ")
            StrSQL.AppendLine("       ,Codice_2 = " & Agro_SQL_SaveNum(Codice_2) & " ")
            StrSQL.AppendLine("       ,Inviato           =  0 ")
            StrSQL.AppendLine("       ,DataInvio         =  Null ")
            StrSQL.AppendLine("       ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.AppendLine("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.AppendLine("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.AppendLine(" WHERE Utente='" & Agro_SQL_SaveText(Utente) & "'")
            StrSQL.AppendLine(" AND   Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.AppendLine(" AND   Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Copia(
        template As String, targets As IEnumerable(Of String),
        copyHierarchy As Boolean, copyProcedures As Boolean,
        objParametri As AgronicaCoreParametri
    )
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.Copia()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim conApici = targets.Count > 1
        Dim xIn = StringListAggregator(targets, conApici).FirstOrDefault

        StrSQL.AppendLine($"UPDATE Utenti_Profili SET")
        StrSQL.AppendLine($"  Data_Modifica = GETDATE()")
        StrSQL.AppendLine($"  , Username_Modifica = '{Agro_SQL_SaveText(objParametri.UsernameOperazione)}'")
        If copyHierarchy then
            StrSQL.AppendLine($"  , Descrizione_1 = NEWDATA.Descrizione_1")
            StrSQL.AppendLine($"  , Descrizione_2 = NEWDATA.Descrizione_2")
        End If
        If copyProcedures then
            StrSQL.AppendLine($"  , filtro_pratiche_attivo = NEWDATA.filtro_pratiche_attivo")
            StrSQL.AppendLine($"  , operatore_filtri = NEWDATA.operatore_filtri")
        End If
        StrSQL.AppendLine($"FROM (")
        StrSQL.AppendLine($"  SELECT * FROM Utenti_Profili")
        StrSQL.AppendLine($"  WHERE Utente = '{Agro_SQL_SaveText(template)}'")
        StrSQL.AppendLine($"    AND Id_Servizio = {Agro_SQL_SaveNum(enum_Id_Servizio.GiasOnline)}")
        StrSQL.AppendLine($") NEWDATA")
        StrSQL.AppendLine($"WHERE Utenti_Profili.Id_Servizio = {Agro_SQL_SaveNum(enum_Id_Servizio.GiasOnline)}")
        StrSQL.AppendLine($"  AND Utenti_Profili.Utente IN ({Agro_SQL_Save_Clausola_IN(xIn, valoriStringa:=Not conApici, creaParametriSql:=Not conApici)})")

        Try
            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function Cancella(
        ByVal Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.Cancella()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.AppendLine(" UPDATE Utenti_Profili ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("      ,Inviato = -1 ")
                StrSQL.AppendLine(" WHERE    Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
                StrSQL.AppendLine(" AND Inviato >= 0")

                If Id_Servizio <> 0 Then
                    StrSQL.AppendLine(" AND  Id_Servizio =  " & Agro_SQL_SaveNum(Id_Servizio) & " ")
                End If

                If objParametri.SuperUserUsername <> "" Then
                    StrSQL.AppendLine(" AND  Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                End If

            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Utenti_Profili ")
                StrSQL.AppendLine(" WHERE    Utente = '" & Agro_SQL_SaveText(Utente) & "' ")

                If Id_Servizio <> 0 Then
                    StrSQL.AppendLine(" AND  Id_Servizio =  " & Agro_SQL_SaveNum(Id_Servizio) & " ")
                End If

                If objParametri.SuperUserUsername <> "" Then
                    StrSQL.AppendLine(" AND  Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    ''' <param name="utenti">Collezione di username. Se la collezione contiene piu' di 10.000 username, quelli in eccesso verranno ignorati.</param>
    ''' <param name="objParametri">objParametri_Utenti</param>
    Public Function CancellaMassivo(
        ByVal utenti As IEnumerable(Of String), ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    )
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.CancellaMassivo()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim conApici = utenti.Count > 1
        Dim xIn = StringListAggregator(utenti, conApici).FirstOrDefault
        Try
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.AppendLine(" UPDATE Utenti_Profili ")
                StrSQL.AppendLine(" SET ")
                StrSQL.AppendLine("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.AppendLine("    , Inviato = -1 ")
            Else
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM     Utenti_Profili ")
            End If

            StrSQL.AppendLine(" WHERE    Utente IN (" & Agro_SQL_Save_Clausola_IN(xIn, valoriStringa:=Not conApici, creaParametriSql:=Not conApici) & ") ")

            '--------------------------------------------------------------------------
            If objParametri.SuperUserUsername <> "" Then
                StrSQL.AppendLine(" AND  Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            End If
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function CancellaVisibilita(
        ByVal Utente As String,
        ByVal Id_Servizio As Int32,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Visibilita_Appoggio_W.Cancella()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine(" DELETE ")
            StrSQL.AppendLine(" FROM     Utenti_Visibilita ")
            StrSQL.AppendLine(" WITH(ROWLOCK) ")
            StrSQL.AppendLine(" WHERE PivaSuperUser='" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.AppendLine(" AND Username = '" & Agro_SQL_SaveText(Utente) & "' ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function ModificaDataUtentiVisibilitaAppoggio(
        ByVal Utente As String,
        ByVal Id_Servizio As Integer,
        ByVal nuovaData As DateTime,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.ModificaDataUtentiVisibilitaAppoggio()"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            StrSQL.AppendLine("UPDATE Utenti_Profili WITH (ROWLOCK) SET ")
            StrSQL.AppendLine("       DataUltimoRiportoUtentiVisibilitaAppoggio = " & Agro_SQL_SaveDateTime(nuovaData))
            StrSQL.AppendLine("WHERE  Utente='" & Agro_SQL_SaveText(Utente) & "'")
            StrSQL.AppendLine("AND    Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.AppendLine("AND    Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Public Function ModificaDataUtentiVisibilitaAppoggio(
        ByVal utenti As IEnumerable(Of String),
        ByVal nuovaData As DateTime,
        ByRef objParametri As AgronicaCoreParametri,
        Optional ByVal Id_Servizio As Integer = 5 'enum_Id_Servizio.GiasOnline
    ) As Boolean
        Dim NomeRoutine As String = "AnagrafeCoreUtentiDAL.Utenti_Profili_Write.ModificaDataUtentiVisibilitaAppoggio(batch)"
        Dim StrSQL As New System.Text.StringBuilder
        Try
            Dim listaUtenti As String = String.Join(",", utenti)
            StrSQL.AppendLine("UPDATE Utenti_Profili WITH (ROWLOCK) SET ")
            StrSQL.AppendLine("       DataUltimoRiportoUtentiVisibilitaAppoggio = " & Agro_SQL_SaveDateTime(nuovaData))
            StrSQL.AppendLine("WHERE  Utente IN (" & Agro_SQL_Save_Clausola_IN(listaUtenti, True) & ")")
            StrSQL.AppendLine("AND    Utente_Profilo = '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            StrSQL.AppendLine("AND    Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")

            Return EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, NomeRoutine, ex.Message)
            Throw New Exception("[" & NomeRoutine & "] : " & ex.Message)
        End Try
    End Function

End Class
