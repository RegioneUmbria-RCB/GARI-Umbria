Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.My.Resources
Imports AgronicaCoreDataProvider.TipiEnumerativi


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class CategTipologiaDocumentiXUtenti_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal ID_Categoria As Integer,
                          ByVal ID_Tipologia As Integer?,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByRef objParametri_Server As AgronicaCoreParametri,
                          ByRef objParametri_Utenti As AgronicaCoreParametri,
                          Optional ByVal Autorizzato As Integer? = Nothing,
                          Optional ByVal Username As String = "",
                          Optional ByVal FiltroAggiuntivo As String = "",
                          Optional ByVal xOrderBy As String = "",
                          Optional ByVal FiltroImpreseVisibiliXUtente As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.CategTipologiaDocumentiXUtenti_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim pivaSuperUser = objParametri_Server.PivaSuperUser

        Try
            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT * ")
                    StrSQL.AppendLine(" FROM  CategTipologiaDocumentiXUtenti  ")
                    StrSQL.AppendLine(" WHERE PivaSuperUser =  " & Agro_SQL_SaveText_NULL(pivaSuperUser) & " ")

                    If Piva <> "" Then
                        StrSQL.AppendLine(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
                    End If

                    If ID_Categoria <> 0 Then
                        StrSQL.AppendLine(" AND ID_Categoria = " & Agro_SQL_SaveNum(ID_Categoria) & " ")
                    End If

                    If ID_Tipologia IsNot Nothing Then
                        StrSQL.AppendLine(" AND ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
                    End If

                    If Username <> "" Then
                        StrSQL.AppendLine(" AND Username  = " & Agro_SQL_SaveText_NULL(Username) & " ")
                    End If

                    If FiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" And (" & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri_Server) & ") ")
                    End If

                    If Autorizzato IsNot Nothing Then
                        StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.Autorizzato = " & Agro_SQL_SaveNum(Autorizzato) & " ")
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    Dim NomeDB_Utenti = String.Empty

                    If Not IsNothing(objParametri_Utenti) Then
                        NomeDB_Utenti = objParametri_Utenti.Recupera_NomeDB
                    Else
                        Throw New Exception("Specificare anche objParametri_Utenti")
                    End If

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT CategTipologiaDocumentiXUtenti.Username, Alert_Area.ID_Area As ID_Categoria, Alert_Area.Nome As Nome_Categoria,")
                    StrSQL.AppendLine(" ISNULL(Alert_Tipologia.ID_Tipologia,0)As ID_Tipologia, ISNULL(Alert_Tipologia.Nome,'') As Nome_Tipologia, CategTipologiaDocumentiXUtenti.Autorizzato As Autorizzato,")
                    StrSQL.AppendLine(" (CASE When  CategTipologiaDocumentiXUtenti.Autorizzato=1 THEN '" & Gias.GestioneCompleta & "' When  CategTipologiaDocumentiXUtenti.Autorizzato=2 THEN '" & Gias.SolaLettura & "' End) AS Valore_Autorizzato,")
                    StrSQL.AppendLine(" ISNULL(Utenti_Dettagli.Nome,'') As Nome_Utente, ISNULL(Utenti_Dettagli.Cognome,'') As Cognome_Utente ,")
                    StrSQL.AppendLine(" ISNULL(Gruppi_Utente.Gruppi_Utente_des,'') As Gruppo_Utente , Imprese.rag_soc As Rag_Soc, CategTipologiaDocumentiXUtenti.Piva, Replace(CategTipologiaDocumentiXUtenti.Username, '@','') as id_username ")
                    StrSQL.AppendLine(" FROM  CategTipologiaDocumentiXUtenti  ")
                    StrSQL.AppendLine(" LEFT JOIN Alert_Tipologia")
                    StrSQL.AppendLine(" ON Alert_Tipologia.ID_Tipologia = CategTipologiaDocumentiXUtenti.ID_Tipologia")
                    StrSQL.AppendLine(" LEFT JOIN Alert_Area")
                    StrSQL.AppendLine(" ON Alert_Area.ID_Area = CategTipologiaDocumentiXUtenti.ID_Categoria")
                    StrSQL.AppendLine(" LEFT JOIN Imprese")
                    StrSQL.AppendLine(" ON Imprese.PIVA = CategTipologiaDocumentiXUtenti.Piva")
                    StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ")
                    StrSQL.AppendLine(" ON Utenti_Dettagli.Username = CategTipologiaDocumentiXUtenti.UserName")
                    StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Utenti_xGruppi_Utente  Utenti_xGruppi_Utente")
                    StrSQL.AppendLine(" ON Utenti_xGruppi_Utente.UserName = CategTipologiaDocumentiXUtenti.UserName")
                    StrSQL.AppendLine(" LEFT JOIN " & NomeDB_Utenti & ".dbo.Gruppi_Utente Gruppi_Utente")
                    StrSQL.AppendLine(" ON Gruppi_Utente.Gruppi_Utente_cod = Utenti_xGruppi_Utente.Gruppi_Utente_cod")
                    StrSQL.AppendLine(" WHERE CategTipologiaDocumentiXUtenti.PivaSuperUser =  " & Agro_SQL_SaveText_NULL(pivaSuperUser) & " ")

                    If Piva <> "" AndAlso Not FiltroImpreseVisibiliXUtente Then
                        StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.Piva In (" & Agro_SQL_SaveText_NULL(Piva) & ") ")
                    End If

                    If ID_Categoria <> 0 Then
                        StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.ID_Categoria = " & Agro_SQL_SaveNum(ID_Categoria) & " ")
                    End If

                    If ID_Tipologia <> 0 Then
                        StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
                    End If

                    If Autorizzato IsNot Nothing Then
                        StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.Autorizzato = " & Agro_SQL_SaveNum(Autorizzato) & " ")
                    End If

                    If FiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" And (" & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri_Server) & ") ")
                    End If

                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri_Server))
                    End If

                    If FiltroImpreseVisibiliXUtente Then

                        '----------------------------------------------------------------
                        '--- Filtro associato all'utente 
                        '----------------------------------------------------------------

                        Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                        Dim UtenteProfiloImpreseSql As New List(Of String)()
                        Dim DtImpreseVisibili As DataTable

                        DtImpreseVisibili = objUtentiVisibilita.Leggi(enum_TipoEntita.Impresa, "", "", objParametri_Server)
                        If Not IsNothing(DtImpreseVisibili) Then
                            For i As Integer = 0 To DtImpreseVisibili.Rows.Count - 1
                                UtenteProfiloImpreseSql.Add("'" & DtImpreseVisibili.Rows(i).Item("Piva").ToString() & "'")
                            Next
                            If Not IsNothing(UtenteProfiloImpreseSql) AndAlso UtenteProfiloImpreseSql.Count > 0 Then
                                StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", UtenteProfiloImpreseSql.ToArray()), True) & ") ")
                            End If
                        End If

                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    '####### INIZIO FUNZIONI PER IL CONTROLLO DEI PERMESSI DOCUMENTALE ####### 
    '####### NB ####### 
    '####### 1. Le tabelle Alert_Area e Alert_Tipologia non devono essere  rinominate per poter richiamare e utilizzare correttamente queste Sub. ####### 
    '####### 2. Vengono controllati i permessi anche nella funzione Leggi_Elenco in Alert_Elenco applicati alla ricerca dei documenti. ####### 
    '####### 3. Vengono controllati i permessi anche nella funzione Leggi_AreaETipologia in Alert_Tipologia ma senza controllare la Piva. ####### 

    Public Sub Controllo_Se_Vengono_Gestiti_Permessi(ByRef iLivelloGerarchia As Integer, ByRef bPermessixUtente As Boolean,
                                                     ByVal piva As String, ByVal objParametri_Server As AgronicaCoreParametri)

        Dim dtPermessi As New DataTable

        Dim dtGerarchia As New DataTable

        If IsNothing(iLivelloGerarchia) OrElse IsNothing(objParametri_Server) Then
            Throw New Exception("Errore i Parametri passati alla sub " & Reflection.MethodInfo.GetCurrentMethod.Name & " non sono tutti valorizzati. ")
        End If

        'Il superuser vede tutto
        If objParametri_Server.UtenteUsername.ToLower() <> objParametri_Server.SuperUserUsername.ToLower() AndAlso
           piva <> "" Then

            'Controllo se sono stati inseriti dei Permessi

            dtPermessi = Leggi("", 0, Nothing,
                               enumSelezioneVariabile.Selezione_TabellaCompleta,
                               objParametri_Server, Nothing, Nothing)

            If Not IsNothing(dtPermessi) AndAlso dtPermessi.Rows.Count > 0 Then

                bPermessixUtente = True

                ''Lettura della tabella gerarchia imprese per determinarne il livello max                
                Dim GI As New AgronicaCoreAnagrafeDAL.GerarchiaImprese_R
                dtGerarchia = GI.LeggixFiglio("", enumSelezioneVariabile.Selezione_TabellaCompleta,
                                              "", "Livello Desc",
                                              objParametri_Server)

                If Not IsNothing(dtGerarchia) AndAlso dtGerarchia.Rows.Count > 0 Then
                    iLivelloGerarchia = dtGerarchia(0)("Livello")
                End If

            End If

        End If
    End Sub

    Public Sub Controllo_Permessi_SELECT(ByRef StrSQL As Text.StringBuilder,
                                         ByVal iLivelloGerarchia As Integer)

        If IsNothing(StrSQL) OrElse IsNothing(iLivelloGerarchia) Then
            Throw New Exception("Errore i Parametri passati alla sub " & Reflection.MethodInfo.GetCurrentMethod.Name & " non sono tutti valorizzati. ")
        End If

        StrSQL.AppendLine(" , 2 as Autorizzato, 0 as Duplicato, ")
        StrSQL.AppendLine(" IsNull(permessi.Autorizzato, 0) as Autorizzato_Piva, ")
        StrSQL.AppendLine(" Abs(IsNull(permessi.Id_Tipologia, 0)) as Autorizzato_Piva_Tipologia, ")

        If iLivelloGerarchia > 1 Then
            StrSQL.AppendLine(" IsNull(permessi_padre.Autorizzato, 0) as Autorizzato_Padre, ")
            StrSQL.AppendLine(" Abs(IsNull(permessi_padre.Id_Tipologia, 0)) as Autorizzato_Padre_Tipologia, ")
        Else
            StrSQL.AppendLine(" 0 as Autorizzato_Padre, ")
            StrSQL.AppendLine(" 0 as Autorizzato_Padre_Tipologia, ")
        End If

        If iLivelloGerarchia > 2 Then
            StrSQL.AppendLine(" IsNull(permessi_nonno.Autorizzato, 0) as Autorizzato_Nonno, ")
            StrSQL.AppendLine(" Abs(IsNull(permessi_nonno.Id_Tipologia, 0)) as Autorizzato_Nonno_Tipologia, ")
        Else
            StrSQL.AppendLine(" 0 as Autorizzato_Nonno, ")
            StrSQL.AppendLine(" 0 as Autorizzato_Nonno_Tipologia, ")
        End If

        If iLivelloGerarchia > 3 Then
            StrSQL.AppendLine(" IsNull(permessi_bis_nonno.Autorizzato, 0) as Autorizzato_Bis_Nonno, ")
            StrSQL.AppendLine(" Abs(IsNull(permessi_bis_nonno.Id_Tipologia, 0)) as Autorizzato_Bis_Nonno_Tipologia, ")
        Else
            StrSQL.AppendLine(" 0 as Autorizzato_Bis_Nonno, ")
            StrSQL.AppendLine(" 0 as Autorizzato_Bis_Nonno_Tipologia, ")
        End If

        If iLivelloGerarchia > 4 Then
            StrSQL.AppendLine(" IsNull(permessi_tris_nonno.Autorizzato, 0) as Autorizzato_Tris_Nonno, ")
            StrSQL.AppendLine(" Abs(IsNull(permessi_tris_nonno.Id_Tipologia, 0)) as Autorizzato_Tris_Nonno_Tipologia, ")
        Else
            StrSQL.AppendLine(" 0 as Autorizzato_Tris_Nonno, ")
            StrSQL.AppendLine(" 0 as Autorizzato_Tris_Nonno_Tipologia, ")
        End If

        If iLivelloGerarchia > 5 Then
            StrSQL.AppendLine(" IsNull(permessi_quad_nonno.Autorizzato, 0) as Autorizzato_Quad_Nonno, ")
            StrSQL.AppendLine(" Abs(IsNull(permessi_quad_nonno.Id_Tipologia, 0)) as Autorizzato_Quad_Nonno_Tipologia ")
        Else
            StrSQL.AppendLine(" 0 as Autorizzato_Quad_Nonno, ")
            StrSQL.AppendLine(" 0 as Autorizzato_Quad_Nonno_Tipologia ")
        End If
    End Sub

    Public Sub Controllo_Permessi_JOIN(ByRef StrSQL As Text.StringBuilder,
                                       ByVal iLivelloGerarchia As Integer,
                                       ByVal piva As String,
                                       ByVal controllaPermessiSoloCategoria As Boolean,
                                       ByRef objParametri_Server As AgronicaCoreParametri)


        If IsNothing(StrSQL) OrElse IsNothing(iLivelloGerarchia) OrElse
           IsNothing(objParametri_Server) OrElse String.IsNullOrEmpty(piva) Then
            Throw New Exception("Errore i Parametri passati alla sub " & Reflection.MethodInfo.GetCurrentMethod.Name & " non sono tutti valorizzati. ")
        End If

        'Ora se sono stati inseriti dei record nella tabella CategTipologiaDocumentiXUtenti,
        'l 'utente deve per forza essere autorizzato ad utilizzare quella Categoria oppure quella specifica Tipologia 

        '1. Controllo per Piva puntuale
        If controllaPermessiSoloCategoria Then
            StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi On ( permessi.PivaSuperUser = Alert_Area.PivaSuperUser And permessi.ID_Categoria = Alert_Area.ID_area And permessi.Autorizzato > 0 And permessi.Username = '" & objParametri_Server.UtenteUsername & "' And permessi.Piva = '" & Agro_SQL_SaveText(piva, False) & "') ")
        Else
            StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi On ( permessi.PivaSuperUser = Alert_Tipologia.PivaSuperUser And permessi.ID_Categoria = Alert_Tipologia.ID_area And ( permessi.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi.ID_Tipologia = 0 ) And permessi.Autorizzato > 0 And permessi.Username = '" & objParametri_Server.UtenteUsername & "' And permessi.Piva = '" & piva & "') ")
        End If


        If iLivelloGerarchia > 1 Then
            '2. Controllo per Piva Padre
            If controllaPermessiSoloCategoria Then
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_padre On ( permessi_padre.PivaSuperUser = Alert_Area.PivaSuperUser And permessi_padre.ID_Categoria = Alert_Area.ID_area And permessi_padre.Autorizzato > 0 And permessi_padre.Username = '" & objParametri_Server.UtenteUsername & "' ")
            Else
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_padre On ( permessi_padre.PivaSuperUser = Alert_Tipologia.PivaSuperUser And permessi_padre.ID_Categoria = Alert_Tipologia.ID_area And ( permessi_padre.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_padre.ID_Tipologia = 0 ) And permessi_padre.Autorizzato > 0 And permessi_padre.Username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            StrSQL.AppendLine(" and permessi_padre.Piva In (Select Padre from GerarchiaImprese Where Figlio = '" & piva & "'))  ")

        End If

        If iLivelloGerarchia > 2 Then
            '2. Controllo per Piva Nonno
            If controllaPermessiSoloCategoria Then
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_nonno On ( permessi_nonno.PivaSuperUser = Alert_Area.PivaSuperUser And permessi_nonno.ID_Categoria = Alert_Area.ID_area And permessi_nonno.Autorizzato > 0 And permessi_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            Else
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_nonno On ( permessi_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser And permessi_nonno.ID_Categoria = Alert_Tipologia.ID_area And ( permessi_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_nonno.ID_Tipologia = 0 ) And permessi_nonno.Autorizzato > 0 And permessi_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            StrSQL.AppendLine(" and permessi_nonno.Piva In (Select Padre from GerarchiaImprese GerarchiaImpreseNonno Where Figlio In (Select Padre from GerarchiaImprese GerarchiaImpresePadre Where Figlio = '" & piva & "')))  ")

        End If

        If iLivelloGerarchia > 3 Then
            '3. Controllo per Piva Bis-Nonno
            If controllaPermessiSoloCategoria Then
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_bis_nonno On ( permessi_bis_nonno.PivaSuperUser = Alert_Area.PivaSuperUser And permessi_bis_nonno.ID_Categoria = Alert_Area.ID_area And permessi_bis_nonno.Autorizzato > 0 And permessi_bis_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            Else
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_bis_nonno On ( permessi_bis_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser And permessi_bis_nonno.ID_Categoria = Alert_Tipologia.ID_area And ( permessi_bis_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_bis_nonno.ID_Tipologia = 0 ) And permessi_bis_nonno.Autorizzato > 0 And permessi_bis_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            StrSQL.AppendLine(" and permessi_bis_nonno.Piva In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseBisNonno Where Figlio In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseNonno2 Where Figlio In (Select Padre from GerarchiaImprese GerarchiaImpresePadre2 Where Figlio = '" & piva & "')))  ")
            StrSQL.AppendLine(" )")

        End If

        If iLivelloGerarchia > 4 Then
            '4. Controllo per Piva Tris-Nonno
            If controllaPermessiSoloCategoria Then
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_tris_nonno On ( permessi_tris_nonno.PivaSuperUser = Alert_Area.PivaSuperUser And permessi_tris_nonno.ID_Categoria = Alert_Area.ID_area And permessi_tris_nonno.Autorizzato > 0 And permessi_tris_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            Else
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_tris_nonno On ( permessi_tris_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser And permessi_tris_nonno.ID_Categoria = Alert_Tipologia.ID_area And ( permessi_tris_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_tris_nonno.ID_Tipologia = 0 ) And permessi_tris_nonno.Autorizzato > 0 And permessi_tris_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            StrSQL.AppendLine(" and permessi_tris_nonno.Piva In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseTrisNonno Where Figlio In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseBisNonno3 Where Figlio In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseNonno3 Where Figlio In (Select Padre from GerarchiaImprese GerarchiaImpresePadre3 Where Figlio = '" & piva & "')))  ")
            StrSQL.AppendLine(" ))")

        End If

        If iLivelloGerarchia > 5 Then
            '5. Controllo per Piva Quad-Nonno
            If controllaPermessiSoloCategoria Then
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_Quad_nonno On ( permessi_Quad_nonno.PivaSuperUser = Alert_Area.PivaSuperUser And permessi_Quad_nonno.ID_Categoria = Alert_Area.ID_area And permessi_Quad_nonno.Autorizzato > 0 And permessi_Quad_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            Else
                StrSQL.AppendLine(" Left Outer Join CategTipologiaDocumentiXUtenti permessi_Quad_nonno On ( permessi_Quad_nonno.PivaSuperUser = Alert_Tipologia.PivaSuperUser And permessi_Quad_nonno.ID_Categoria = Alert_Tipologia.ID_area And ( permessi_Quad_nonno.ID_Tipologia = Alert_Tipologia.ID_Tipologia Or permessi_Quad_nonno.ID_Tipologia = 0 ) And permessi_Quad_nonno.Autorizzato > 0 And permessi_Quad_nonno.Username = '" & objParametri_Server.UtenteUsername & "' ")
            End If

            StrSQL.AppendLine(" and permessi_Quad_nonno.Piva In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseQuadNonno Where Figlio In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseTrisNonno4 Where Figlio In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseBisNonno4 Where Figlio In ")
            StrSQL.AppendLine("       (Select Padre from GerarchiaImprese GerarchiaImpreseNonno4 Where Figlio In (Select Padre from GerarchiaImprese GerarchiaImpresePadre4 Where Figlio = '" & piva & "')))  ")
            StrSQL.AppendLine(" )))")

        End If
    End Sub

    Public Sub Controllo_Permessi_Filtro_WHERE(ByRef StrSQL As Text.StringBuilder,
                                               ByVal iLivelloGerarchia As Integer,
                                               ByVal tipoPermessoDaControllare As Integer,
                                               ByVal controllaPermessiSoloCategoria As Boolean)

        If IsNothing(StrSQL) OrElse IsNothing(iLivelloGerarchia) Then
            Throw New Exception("Errore i Parametri passati alla sub " & Reflection.MethodInfo.GetCurrentMethod.Name & " non sono tutti valorizzati. ")
        End If

        If tipoPermessoDaControllare = Tipo_Permesso_Documentale.Gestione_Completa AndAlso
           controllaPermessiSoloCategoria Then

            StrSQL.AppendLine(" And (isnull(permessi.Autorizzato, 0) = " & Tipo_Permesso_Documentale.Gestione_Completa)

            If iLivelloGerarchia > 1 Then
                StrSQL.AppendLine("  OR isnull(permessi_padre.Autorizzato, 0) = " & Tipo_Permesso_Documentale.Gestione_Completa)
            End If

            If iLivelloGerarchia > 2 Then
                StrSQL.AppendLine(" OR  isnull(permessi_nonno.Autorizzato, 0) = " & Tipo_Permesso_Documentale.Gestione_Completa)
            End If

            If iLivelloGerarchia > 3 Then
                StrSQL.AppendLine(" OR  isnull(permessi_bis_nonno.Autorizzato, 0) = " & Tipo_Permesso_Documentale.Gestione_Completa)
            End If

            If iLivelloGerarchia > 4 Then
                StrSQL.AppendLine(" OR  isnull(permessi_tris_nonno.Autorizzato, 0) = " & Tipo_Permesso_Documentale.Gestione_Completa)
            End If

            If iLivelloGerarchia > 5 Then
                StrSQL.AppendLine(" OR  isnull(permessi_quad_nonno.Autorizzato, 0) = " & Tipo_Permesso_Documentale.Gestione_Completa)
            End If

            StrSQL.AppendLine(" )")
        Else

            'Condizione almeno un autorizzazione valida
            StrSQL.AppendLine(" And ((isnull(permessi.Autorizzato, 0)   ")

            If iLivelloGerarchia > 1 Then
                StrSQL.AppendLine(" + isnull(permessi_padre.Autorizzato, 0)  ")
            End If

            If iLivelloGerarchia > 2 Then
                StrSQL.AppendLine(" + isnull(permessi_nonno.Autorizzato, 0)  ")
            End If

            If iLivelloGerarchia > 3 Then
                StrSQL.AppendLine(" + isnull(permessi_bis_nonno.Autorizzato, 0)  ")
            End If

            If iLivelloGerarchia > 4 Then
                StrSQL.AppendLine(" + isnull(permessi_tris_nonno.Autorizzato, 0)  ")
            End If

            If iLivelloGerarchia > 5 Then
                StrSQL.AppendLine(" + isnull(permessi_quad_nonno.Autorizzato, 0)  ")
            End If

            StrSQL.AppendLine(" ) > 0) ")

        End If

    End Sub

    Public Sub Controllo_Permessi_ORDER_BY(ByRef StrSQL As Text.StringBuilder,
                                           ByVal controllaPermessiSoloCategoria As Boolean)

        If IsNothing(StrSQL) Then
            Throw New Exception("Errore i Parametri passati alla sub " & Reflection.MethodInfo.GetCurrentMethod.Name & " non sono tutti valorizzati. ")
        End If

        Dim ordinamento As String = ""

        If controllaPermessiSoloCategoria Then
            ordinamento += "Alert_Area.ID_Area,"
        Else
            ordinamento += "Alert_Tipologia.Id_Tipologia,"
        End If

        ordinamento += "Autorizzato_Piva_Tipologia Desc, " &
                              "Autorizzato_Piva Asc, " &
                              "Autorizzato_Padre_Tipologia Desc, " &
                              "Autorizzato_Padre Asc, " &
                              "Autorizzato_Nonno_Tipologia Desc, " &
                              "Autorizzato_Nonno Asc, " &
                              "Autorizzato_Bis_Nonno_Tipologia Desc, " &
                              "Autorizzato_Bis_Nonno Asc, " &
                              "Autorizzato_Tris_Nonno_Tipologia Desc, " &
                              "Autorizzato_Tris_Nonno, " &
                              "Autorizzato_Quad_Nonno_Tipologia Desc, " &
                              "Autorizzato_Quad_Nonno "

        'Anna 29/04/22: Categorie e Tipologie in ordine alfabetico
        If controllaPermessiSoloCategoria Then
            ordinamento += ", Alert_Area.Nome"
        Else
            ordinamento += ", Alert_Tipologia.Nome"
        End If

        StrSQL.AppendLine(" Order by " & ordinamento)
    End Sub

    'Sub creata nel DAL perché non si poteva richiamare nel BIZ per dipendenza circolare
    Public Sub Controllo_Permessi_Filtro_DataTable(ByRef DT As DataTable,
                                                   ByVal tipoPermessoDaControllare As Integer,
                                                   ByVal controllaPermessiSoloCategoria As Boolean)

        If IsNothing(DT) OrElse IsNothing(tipoPermessoDaControllare) OrElse
           (tipoPermessoDaControllare <> Tipo_Permesso_Documentale.Gestione_Completa AndAlso tipoPermessoDaControllare <> Tipo_Permesso_Documentale.Lettura) Then
            Throw New Exception("Errore i Parametri passati alla sub " & Reflection.MethodInfo.GetCurrentMethod.Name & " non sono tutti valorizzati. ")
        End If

        If DT.Rows.Count > 0 Then


            Dim dtNew As New DataTable

            'Duplico la struttura nel nuovo datatable
            dtNew = DT.Copy()

            'Pulisco il datatable letto
            DT.Clear()

            Dim Last_Id_CategoriaTipologia As Integer = 0

            Dim bOk As Boolean = False

            'Usato per avere le Categorie/Tipologie autorizzate alla lettura
            Dim autorizzato As Integer = 0

            For Each dr As DataRow In dtNew.Rows

                Dim id As Integer

                If controllaPermessiSoloCategoria Then
                    id = dr("ID_Area")
                Else
                    id = dr("ID_Tipologia")
                End If

                If Last_Id_CategoriaTipologia <> 0 AndAlso Last_Id_CategoriaTipologia = id Then
                    dr("Duplicato") = 1
                Else

                    Last_Id_CategoriaTipologia = id

                    If tipoPermessoDaControllare = Tipo_Permesso_Documentale.Gestione_Completa Then

                        If Not controllaPermessiSoloCategoria Then

                            If dr("Autorizzato_Piva") = 0 Then

                                If dr("Autorizzato_Padre") = 0 Then

                                    If dr("Autorizzato_Nonno") = 0 Then

                                        If dr("Autorizzato_Bis_Nonno") = 0 Then

                                            If dr("Autorizzato_Tris_Nonno") = 0 Then

                                                If dr("Autorizzato_Quad_Nonno") = 0 Then
                                                    bOk = False
                                                ElseIf dr("Autorizzato_Quad_Nonno") = 1 Then
                                                    bOk = True
                                                Else
                                                    'Non va Bene
                                                    bOk = False
                                                End If

                                            ElseIf dr("Autorizzato_Tris_Nonno") = 1 Then
                                                bOk = True
                                            Else
                                                'Non va Bene
                                                bOk = False
                                            End If


                                        ElseIf dr("Autorizzato_Bis_Nonno") = 1 Then
                                            bOk = True
                                        Else
                                            'Non va Bene
                                            bOk = False
                                        End If


                                    ElseIf dr("Autorizzato_Nonno") = 1 Then
                                        bOk = True
                                    Else
                                        'Non va Bene
                                        bOk = False
                                    End If


                                ElseIf dr("Autorizzato_Padre") = 1 Then
                                    bOk = True
                                Else
                                    'Non va Bene
                                    bOk = False
                                End If



                            ElseIf dr("Autorizzato_Piva") = 1 Then
                                bOk = True
                            Else
                                'Non va Bene
                                bOk = False
                            End If


                            If bOk Then
                                dr("Autorizzato") = 1
                            End If

                        Else

                            dr("Autorizzato") = 1

                        End If

                    ElseIf tipoPermessoDaControllare = Tipo_Permesso_Documentale.Lettura Then

                        If dr("Autorizzato_Piva") = 0 Then

                            If dr("Autorizzato_Padre") = 0 Then

                                If dr("Autorizzato_Nonno") = 0 Then

                                    If dr("Autorizzato_Bis_Nonno") = 0 Then

                                        If dr("Autorizzato_Tris_Nonno") = 0 Then

                                            If dr("Autorizzato_Quad_Nonno") = 0 Then
                                                bOk = False
                                            ElseIf dr("Autorizzato_Quad_Nonno") > 0 Then
                                                bOk = True
                                                autorizzato = CInt(dr("Autorizzato_Quad_Nonno"))
                                            Else
                                                'Non va Bene
                                                bOk = False
                                            End If

                                        ElseIf dr("Autorizzato_Tris_Nonno") > 0 Then
                                            bOk = True
                                            autorizzato = CInt(dr("Autorizzato_Tris_Nonno"))
                                        Else
                                            'Non va Bene
                                            bOk = False
                                        End If


                                    ElseIf dr("Autorizzato_Bis_Nonno") > 0 Then
                                        bOk = True
                                        autorizzato = CInt(dr("Autorizzato_Bis_Nonno"))
                                    Else
                                        'Non va Bene
                                        bOk = False
                                    End If


                                ElseIf dr("Autorizzato_Nonno") > 0 Then
                                    bOk = True
                                    autorizzato = CInt(dr("Autorizzato_Nonno"))
                                Else
                                    'Non va Bene
                                    bOk = False
                                End If


                            ElseIf dr("Autorizzato_Padre") > 0 Then
                                bOk = True
                                autorizzato = CInt(dr("Autorizzato_Padre"))
                            Else
                                'Non va Bene
                                bOk = False
                            End If



                        ElseIf dr("Autorizzato_Piva") > 0 Then
                            bOk = True
                            autorizzato = CInt(dr("Autorizzato_Piva"))
                        Else
                            'Non va Bene
                            bOk = False
                        End If


                        If bOk Then
                            dr("Autorizzato") = autorizzato
                        End If
                    End If

                End If

            Next

            If tipoPermessoDaControllare = Tipo_Permesso_Documentale.Gestione_Completa Then
                Dim drValidi = dtNew.Select("Autorizzato = 1 And Duplicato = 0")

                If Not IsNothing(drValidi) AndAlso drValidi.Length > 0 Then

                    DT = dtNew.Select("Autorizzato = 1 And Duplicato = 0").CopyToDataTable()

                End If

            ElseIf tipoPermessoDaControllare = Tipo_Permesso_Documentale.Lettura Then
                Dim drValidi = dtNew.Select("Autorizzato > 0 And Duplicato = 0")

                If Not IsNothing(drValidi) AndAlso drValidi.Length > 0 Then

                    DT = dtNew.Select("Autorizzato > 0 And Duplicato = 0").CopyToDataTable()

                End If
            End If

        End If

    End Sub

    '####### FINE FUNZIONI PER IL CONTROLLO DEI PERMESSI DOCUMENTALE ####### 

End Class

Public Class CategTipologiaDocumentiXUtenti_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal User As String,
                           ByVal ID_Categoria As Integer,
                           ByVal ID_Tipologia As Integer,
                           ByVal Autorizzato As Integer,
                           ByVal validita_inizio As Date,
                           ByVal validita_fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.CategTipologiaDocumentiXUtenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO CategTipologiaDocumentiXUtenti  ")
            StrSQL.AppendLine("                   ( PivaSuperUser, Piva, Username, ID_Categoria, ID_Tipologia, Autorizzato , ")
            StrSQL.AppendLine("                     Inviato, DataInvio, Data_Creazione, Data_Modifica, Username_Creazione, ")
            StrSQL.AppendLine("                     Username_Modifica,  validita_inizio, validita_fine ")

            StrSQL.AppendLine(" ) ")

            StrSQL.AppendLine("VALUES (")

            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(User) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(ID_Categoria) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(ID_Tipologia) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Autorizzato) & "  ")
            StrSQL.AppendLine("         , 0 ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.AppendLine("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.AppendLine("		    ,'" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(validita_inizio) & " ")
            StrSQL.AppendLine("         ," & Agro_SQL_SaveDate(validita_fine) & " ")

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

    Public Function Modifica(ByVal Piva As String,
                             ByVal User As String,
                             ByVal ID_Categoria As Integer,
                             ByVal ID_Tipologia As Integer?,
                             ByVal Autorizzato As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.CategTipologiaDocumentiXUtenti_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE CategTipologiaDocumentiXUtenti  SET ")
            StrSQL.AppendLine("    Autorizzato           = " & Agro_SQL_SaveNum(Autorizzato) & " ")
            StrSQL.AppendLine("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.AppendLine("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'")

            StrSQL.AppendLine(" WHERE   CategTipologiaDocumentiXUtenti.PivaSuperUser  = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND  CategTipologiaDocumentiXUtenti.Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If

            If User <> "" Then
                StrSQL.AppendLine(" AND  CategTipologiaDocumentiXUtenti.Username = '" & Agro_SQL_SaveText(User) & "' ")
            End If

            If ID_Categoria <> 0 Then
                StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.ID_Categoria = " & Agro_SQL_SaveNum(ID_Categoria) & " ")
            End If

            If ID_Tipologia IsNot Nothing Then
                StrSQL.AppendLine(" AND CategTipologiaDocumentiXUtenti.ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
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


    Public Function Cancella(ByVal Piva As String,
                             ByVal User As String,
                             ByVal ID_Categoria As Integer,
                             ByVal ID_Tipologia As Integer,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal FiltroAggiuntivo As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreScadenziario_DAL.CategTipologiaDocumentiXUtenti_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

            Else
                StrSQL.Length = 0
                StrSQL.AppendLine(" DELETE ")
                StrSQL.AppendLine(" FROM CategTipologiaDocumentiXUtenti  ")
                StrSQL.AppendLine(" WHERE   PivaSuperUser        = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                If Piva <> "" Then
                    StrSQL.AppendLine(" AND    Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                End If

                If User <> "" Then
                    StrSQL.AppendLine(" AND    Username = '" & Agro_SQL_SaveText(User) & "' ")
                End If

                If ID_Categoria <> 0 Then
                    StrSQL.AppendLine(" AND    ID_Categoria = " & Agro_SQL_SaveNum(ID_Categoria) & " ")
                End If

                If ID_Tipologia <> 0 Then
                    StrSQL.AppendLine(" AND    ID_Tipologia = " & Agro_SQL_SaveNum(ID_Tipologia) & " ")
                End If

                If FiltroAggiuntivo <> "" Then
                    StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , objParametri))
                End If

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
