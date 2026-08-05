Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.My.Resources

Public Class Budget_Testata_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                Optional leggiDatiRibaltamento As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreBudegtDAL.Budget_Testata_R.Leggi()"
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT bt.*, i.Rag_Soc As Azienda, lc.Listino_Des As Listino_Des_Costi, lr.Listino_Des As Listino_Des_Ricavi, ")
            StrSQL.AppendLine(" CASE WHEN bt.Sa_Cod = 0 THEN 'No' ELSE 'Si' END As Sa_Cod_Desc, ")
            StrSQL.AppendLine(" CASE WHEN bt.In_Uso = 0 THEN 'No' ELSE 'Si' END As In_Uso_Desc, ")
            StrSQL.AppendLine(" tb.ID AS Tipo_Budget, tb.Descrizione AS Tipo_Budget_Des ")

            If leggiDatiRibaltamento Then
                StrSQL.AppendLine(" , CASE WHEN ISNULL((SELECT TOP(1) ID FROM Ribaltamento_Appezzamento WHERE Budget_Id_Testata = bt.Id_Budget), -1)  > 0")
                StrSQL.AppendLine("   THEN 1 ELSE 0 END AS Ribaltato")
                StrSQL.AppendLine(" , CASE WHEN ISNULL((SELECT TOP(1) ID FROM Ribaltamento_Appezzamento WHERE Budget_Id_Testata = bt.Id_Budget), -1)  > 0")
                StrSQL.AppendLine("   THEN '" + Gias.Si + "' ELSE '" + Gias.No + "' END AS Ribaltato_Des")
            Else
                StrSQL.AppendLine(" , 0 AS Ribaltato")
                StrSQL.AppendLine(" , '" + Gias.No + "' AS Ribaltato_Des")
            End If

            StrSQL.AppendLine(" FROM  Budget_Testata bt")

            StrSQL.AppendLine(" JOIN Imprese i on i.Piva = bt.Piva ")
            StrSQL.AppendLine(" LEFT JOIN Listini_Prezzi lc on lc.Listino_Cod = bt.Listino_Cod_Costi ")
            StrSQL.AppendLine(" LEFT JOIN Listini_Prezzi lr on lr.Listino_Cod = bt.Listino_Cod_Ricavi ")
            StrSQL.AppendLine(" LEFT JOIN Tipologia_Budget tb on tb.ID = bt.Tipo_Budget ")
            StrSQL.AppendLine(" WHERE (bt.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR bt.Sa_Cod = -1) ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   bt.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   bt.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Id_Budget Desc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Dim messaggioErrore As String = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_BudgetAttivo(ByVal Piva As String,
                                       ByVal Id_Budget As Integer,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreBudegtDAL.Budget_Testata_R.Leggi_BudgetAttivo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM ( ")
            StrSQL.AppendLine("     SELECT bt.Id_Budget, (bt.Nome_Budget + '  Rev. ' + CAST(bt.Revisione as varchar(10))) As Nome_Budget, bt.Sa_Cod, bt.Piva ")
            StrSQL.AppendLine("     FROM  Budget_Testata bt")

            Select Case Id_Budget

                Case 0

                    StrSQL.AppendLine("     WHERE bt.In_Uso = 1 ")
                    StrSQL.AppendLine("     AND (bt.Piva = '" & Agro_SQL_SaveText(Piva) & "' OR bt.Sa_Cod = -1) ")

                Case Else

                    'Lettura puntuale per preservare il dato salvato
                    StrSQL.AppendLine("    WHERE bt.Id_Budget = " & Agro_SQL_SaveNum(Id_Budget))

            End Select

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("     AND   bt.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("     AND   bt.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.AppendLine(" UNION SELECT 0, 'Nessuno', 1, '') As budgets")

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY budgets.Sa_Cod Asc ")
            End If


            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function leggiElencoBdgTestata(
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                       Optional ByVal Piva As String = "",
                                       Optional ByVal inUso As Boolean = True,
                                       Optional ByVal tipoBudget As Integer = 1,
                                       Optional ByVal ID_Budget As Integer = 0
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreBudegtDAL.Budget_Testata_R.leggiElencoBdgTestata()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT * ")
            StrSQL.AppendLine("FROM ( ")
            StrSQL.AppendLine("     SELECT bt.Id_Budget, (bt.Nome_Budget + '  Rev. ' + CAST(bt.Revisione as varchar(10))) As Nome_Budget, bt.Sa_Cod")
            StrSQL.AppendLine("     FROM Budget_Testata bt")

            StrSQL.AppendLine("     WHERE 1 = 1")

            If inUso Then
                StrSQL.AppendLine("     AND bt.In_Uso = 1")
            End If

            If Piva <> "" Then
                StrSQL.AppendLine("     AND bt.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            End If

            If ID_Budget <> 0 Then
                StrSQL.AppendLine("     AND bt.ID_Budget = " & Agro_SQL_SaveNum(ID_Budget) & " ")
            End If

            StrSQL.AppendLine("     AND Tipo_Budget = " & Agro_SQL_SaveNum(tipoBudget))

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine("     AND bt.Inviato >=0")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine("     AND bt.Inviato =-1")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------

            StrSQL.AppendLine("UNION SELECT 0, 'Nessuno', 1 ) As budgets")

            If xOrderBy <> "" Then
                StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine("ORDER BY budgets.Sa_Cod Asc ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
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


Public Class Budget_Testata_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Id_Budget As Integer,
                           ByVal Nome_Budget As String,
                           ByVal Revisione As Integer,
                           ByVal In_Uso As Integer,
                           ByVal Tipo_Budget As Integer,
                           ByVal Sa_Cod As Integer,
                           ByVal Listino_Cod_Costi As Integer,
                           ByVal Listino_Cod_Ricavi As Integer,
                           ByVal Validita_Inizio As DateTime,
                           ByVal Validita_Fine As DateTime,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreBudegtDAL.Budget_Testata_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = Date.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = Date.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            StrSQL.Length = 0

            StrSQL.AppendLine("INSERT INTO Budget_Testata ")
            StrSQL.AppendLine("            (Piva_SuperUser,     Piva,  Id_Budget, ")
            StrSQL.AppendLine("             Nome_Budget,        Revisione, In_Uso, Tipo_Budget, ")
            StrSQL.AppendLine("             Sa_Cod, Listino_Cod_Costi, Listino_Cod_Ricavi, ")
            StrSQL.AppendLine("             Inviato,            DataInvio, ")
            StrSQL.AppendLine("             Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("             UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("             Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("             ) ")

            StrSQL.AppendLine("VALUES ( ")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , '" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Budget) & " ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Nome_Budget) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Revisione) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(In_Uso) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Budget) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Listino_Cod_Costi) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Listino_Cod_Ricavi) & " ")

            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(" )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    '#########################################################################
    Public Function Modifica(ByVal ID_Budget As Integer,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                             Optional ByVal Piva As String = Nothing,
                             Optional ByVal Nome_Budget As String = Nothing,
                             Optional ByVal Sa_Cod As Integer? = Nothing,
                             Optional ByVal Revisione As Integer? = Nothing,
                             Optional ByVal In_Uso As Integer? = Nothing,
                             Optional ByVal Tipo_Budget As Integer? = Nothing,
                             Optional ByVal Listino_Cod_Costi As Integer? = Nothing,
                             Optional ByVal Listino_Cod_Ricavi As Integer? = Nothing,
                             Optional ByVal Validita_Inizio As DateTime? = Nothing,
                             Optional ByVal Validita_Fine As DateTime? = Nothing,
                             Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                             Optional ByVal Username_Modifica As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreBudegtDAL.Budget_Testata_W.Modifica()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Data_Modifica = #2/1/1900# Then
                Data_Modifica = Date.Now
            End If

            If Username_Modifica = "" Then
                Username_Modifica = objParametri.UsernameOperazione
            End If

            If ID_Budget = 0 Then
                Throw New Exception("Parametro non corretto nella query (ID_Budget obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Budget_Testata ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Piva) Then
                strSql.AppendLine("   , Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Not IsNothing(Nome_Budget) Then
                strSql.AppendLine("   , Nome_Budget = '" & Agro_SQL_SaveText(Nome_Budget) & "' ")
            End If

            If Not IsNothing(Revisione) Then
                strSql.AppendLine("   , Revisione = " & Agro_SQL_SaveNum(Revisione) & " ")
            End If

            If Not IsNothing(Sa_Cod) Then
                strSql.AppendLine("   , Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Not IsNothing(Listino_Cod_Costi) Then
                strSql.AppendLine("   , Listino_Cod_Costi = " & Agro_SQL_SaveNum(Listino_Cod_Costi) & " ")
            End If

            If Not IsNothing(Listino_Cod_Ricavi) Then
                strSql.AppendLine("   , Listino_Cod_Ricavi = " & Agro_SQL_SaveNum(Listino_Cod_Ricavi) & " ")
            End If

            If Not IsNothing(In_Uso) Then
                strSql.AppendLine("   , In_Uso = " & Agro_SQL_SaveNum(In_Uso) & " ")
            End If

            If Not IsNothing(Tipo_Budget) Then
                strSql.AppendLine("   , Tipo_Budget = " & Agro_SQL_SaveNum(Tipo_Budget) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            '---------------------------------------------            

            strSql.AppendLine(" WHERE ID_Budget = " & Agro_SQL_SaveNum(ID_Budget) & " ")

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


    '#########################################################################
    Public Function Cancella(ByVal Id_Budget As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreBudegtDAL.Testata_Budget_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            'If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

            '    StrSQL.Length = 0
            '    StrSQL.Append(" UPDATE Budget_Testata ")
            '    StrSQL.Append(" SET ")
            '    StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            '    StrSQL.Append("      ,Inviato = -1 ")
            '    StrSQL.Append(" WHERE Inviato >= 0")

            'Else

            StrSQL.Length = 0
            StrSQL.Append(" DELETE ")
            StrSQL.Append(" FROM     Budget_Testata ")
            StrSQL.Append(" WHERE Inviato = 0")

            'End If

            StrSQL.Append(" AND Id_Budget = " & Agro_SQL_SaveNum(Id_Budget) & " ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
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
