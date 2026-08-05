Imports System.Data.Common
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Utenti_Dettagli_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi(ByVal Username_Utente As String,
                          ByVal IdServizio As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal IgnoraProfilo As Boolean = False,
                          Optional ByVal SoloUtentiValidi As Boolean = False
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentiDettagli_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    strSql.AppendLine(" SELECT UserName, Cognome, Nome, CodFisc ")
                    strSql.AppendLine(" FROM Utenti_Dettagli  (NOLOCK) ")
                    strSql.AppendLine(" WHERE 1=1 ")

                    If Username_Utente <> "" Then
                        strSql.AppendLine(" AND Utenti_Dettagli.UserName = '" & Agro_SQL_SaveText(Username_Utente) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.UserName ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    strSql.AppendLine(" SELECT * ")
                    strSql.AppendLine(" FROM Utenti_Dettagli  (NOLOCK) ")
                    strSql.AppendLine(" WHERE 1=1 ")

                    If Username_Utente <> "" Then
                        strSql.AppendLine(" AND Utenti_Dettagli.UserName = '" & Agro_SQL_SaveText(Username_Utente) & "' ")
                    End If

                    If SoloUtentiValidi Then
                        strSql.AppendLine(" AND Utenti_Dettagli.UserName in ")
                        strSql.AppendLine(" (SELECT DISTINCT UserName FROM Utenti_Permessi (NOLOCK) WHERE GETDATE() BETWEEN Validita_Inizio AND Validita_Fine) ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.UserName ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, ")
                    strSql.AppendLine("         Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.UserNameCommerciale, ")
                    strSql.AppendLine("         Utenti_Profili.Utente, Utenti_Profili.Utente_Profilo, Utenti_dettagli.email, Utenti_dettagli.Tel ")
                    strSql.AppendLine(" FROM    Utenti_Dettagli  (NOLOCK) ")
                    strSql.AppendLine("         INNER JOIN Utenti_Profili  (NOLOCK) ON Utenti_Dettagli.UserName = Utenti_Profili.Utente ")
                    strSql.AppendLine(" WHERE  1=1 ")

                    If IgnoraProfilo Then
                        If objParametri.SuperUserUsername <> "" Then
                            strSql.AppendLine("             AND Utenti_Profili.Utente_Profilo =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                        End If

                    Else
                        strSql.AppendLine("             AND Utenti_Profili.Utente_Profilo =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                    End If


                    If IdServizio <> 0 Then
                        strSql.AppendLine(" AND     Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(IdServizio))
                    End If

                    If Username_Utente <> "" Then
                        strSql.AppendLine(" AND     Utenti_Dettagli.UserName = '" & Agro_SQL_SaveText(Username_Utente) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.UserName ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("         Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, ")
                    strSql.AppendLine("         Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.UserNameCommerciale, ")
                    strSql.AppendLine("         Utenti_Profili.Utente, Utenti_Profili.Utente_Profilo, Utenti.Password ")

                    strSql.AppendLine(" FROM    Utenti_Dettagli  (NOLOCK) ")
                    strSql.AppendLine("         INNER JOIN Utenti_Profili  (NOLOCK) ON Utenti_Dettagli.UserName = Utenti_Profili.Utente ")
                    strSql.AppendLine("         INNER JOIN Utenti  (NOLOCK) ON Utenti_Dettagli.UserName = Utenti.UserName ")

                    strSql.AppendLine(" WHERE   1=1 ")
                    If IgnoraProfilo Then
                        If objParametri.SuperUserUsername <> "" Then
                            strSql.AppendLine("             AND Utenti_Profili.Utente_Profilo =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                        End If

                    Else
                        strSql.AppendLine("             AND Utenti_Profili.Utente_Profilo =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
                    End If
                    If IdServizio <> 0 Then
                        strSql.AppendLine(" AND     Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(IdServizio))
                    End If

                    If Username_Utente <> "" Then
                        strSql.AppendLine(" AND     Utenti_Dettagli.UserName = '" & Agro_SQL_SaveText(Username_Utente) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.UserName ")
                    End If
            End Select

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


    Public Function EsisteCodice_Fiscale(ByVal Codice_Fiscale As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentiDettagli_R.EsisteCodice_Fiscale()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable
        Dim exists As Boolean = False

        Try


            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT UserName, Cognome, Nome, CodFisc ")
            strSql.AppendLine(" FROM Utenti_Dettagli  (NOLOCK) ")
            strSql.AppendLine(" WHERE CodFisc = '" & Agro_SQL_SaveText(Codice_Fiscale) & "' ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count > 0 Then
                exists = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return exists

    End Function

    '###############################################################
    Public Function Esiste_UtenteImportatore_RecuperaDati(ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                          ByRef Username As String,
                                                          ByRef Password As String,
                                                          ByRef Cod_Fiscale As String
                                                          ) As Boolean

        Dim dt As DataTable
        Dim esiste As Boolean = False
        Dim filtro As String

        filtro = " ( LOWER(Utenti_Dettagli.Username) = 'importa' OR LOWER(Utenti_Dettagli.Username) = 'importatore' " &
                "   OR LOWER(Utenti_Dettagli.Cognome) = 'importatore' OR LOWER(Utenti_Dettagli.Nome) = 'importatore' " &
                "   OR LOWER(Utenti_Dettagli.UserNameCommerciale) = 'importatore' ) "

        dt = Leggi("", 0, enumSelezioneVariabile.Selezione_JoinCompleta,
                   filtro, "", objParametri_Utenti)

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            esiste = True
            Username = dt.Rows(0).Item("Username")
            Password = dt.Rows(0).Item("Password")
            Cod_Fiscale = dt.Rows(0).Item("CodFisc")
            If Cod_Fiscale = "" Then
                Cod_Fiscale = dt.Rows(0).Item("Piva")
            End If
        End If

        Return esiste

    End Function

    '##############################################################################################


    Public Function Leggi_conPermessi(ByVal IdServizio As Integer,
                                      ByVal Id_Attivita As Integer,
                                      ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByVal xOrderBy As String,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional ByVal Id_Operazione As Integer = -1
                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentiDettagli_R.Leggi_conPermessi()"

        Dim strSql As New StringBuilder With {.Length = 0}
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.AppendLine(" SELECT DISTINCT ")
                    strSql.AppendLine("     Utenti_Dettagli.UserName, Utenti_Permessi.Id_Attivita, Utenti_Permessi.Id_Operazione")
                    strSql.AppendLine(" FROM Utenti_Dettagli")
                    strSql.AppendLine(" LEFT JOIN Utenti_Permessi on Utenti_Permessi.UserName like Utenti_Dettagli.UserName")
                    strSql.AppendLine(" WHERE Id_Attivita > -1")
                    If Id_Operazione <> -1 Then
                        strSql.AppendLine(" AND     Utenti_Permessi.Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione))
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.UserName ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    strSql.AppendLine(" SELECT DISTINCT ")
                    strSql.AppendLine("         Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, ")
                    strSql.AppendLine("         Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.UserNameCommerciale, ")
                    'se l'utente non ha per qualche motivo il record in utentipermessi, la gestione utenti si inchioda (cast non valido da null a date)
                    'imposto validità fine = 01/01/1900 perché così si capisce che l'utente è scaduto
                    strSql.AppendLine("         ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio, ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900') AS Validita_Fine, ")
                    strSql.AppendLine("         Utenti_Permessi.Id_Attivita")

                    strSql.AppendLine(" FROM  Utenti_Dettagli ")
                    strSql.AppendLine(" LEFT OUTER JOIN Utenti_Permessi ON Utenti_Dettagli.UserName = Utenti_Permessi.UserName ")

                    strSql.AppendLine(" WHERE   Utenti_Permessi.Id_Servizio = " & Agro_SQL_SaveNum(IdServizio))
                    strSql.AppendLine(" AND     Utenti_Permessi.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita))

                    If Id_Operazione <> -1 Then
                        strSql.AppendLine(" AND     Utenti_Permessi.Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione))
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.UserName ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    'usata nelle NC

                    strSql.AppendLine(" SELECT DISTINCT ")
                    strSql.AppendLine("         Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, ")
                    strSql.AppendLine("         Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.UserNameCommerciale, ")
                    strSql.AppendLine("         Utenti_dettagli.email, Utenti_dettagli.Tel, ")
                    'se l'utente non ha per qualche motivo il record in utentipermessi, la gestione utenti si inchioda (cast non valido da null a date)
                    'imposto validità fine = 01/01/1900 perché così si capisce che l'utente è scaduto
                    strSql.AppendLine("         ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio, ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900') AS Validita_Fine, ")
                    strSql.AppendLine("         Utenti_Permessi.Id_Attivita")

                    strSql.AppendLine(" FROM  Utenti_Dettagli ")
                    strSql.AppendLine(" LEFT OUTER JOIN Utenti_Permessi ON Utenti_Dettagli.UserName = Utenti_Permessi.UserName ")

                    strSql.AppendLine(" WHERE   1=1 ")

                    If IdServizio <> 0 Then
                        strSql.AppendLine(" AND   Utenti_Permessi.Id_Servizio = " & Agro_SQL_SaveNum(IdServizio))
                    End If
                    If Id_Attivita <> 0 Then
                        strSql.AppendLine(" AND     Utenti_Permessi.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita))
                    End If
                    If Id_Operazione <> -1 Then
                        strSql.AppendLine(" AND     Utenti_Permessi.Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione))
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Utenti_Dettagli.Nome, Utenti_Dettagli.Cognome ")
                    End If
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function


    '##############################################################################################
    ''' <summary>
    ''' Lettura completa degli utenti.
    ''' Tabelle in join:
    ''' <list type="bullet">Utenti</list>
    ''' <list type="bullet">Utenti_Profili</list>
    ''' <list type="bullet">Utenti_Dettagli</list>
    ''' <list type="bullet">Utenti_Permessi</list>
    ''' <list type="bullet">Utenti_Tipologie</list>
    ''' <list type="bullet">Utenti_Gruppi (Solo se versione SQL è 2017 o superiore)</list>
    ''' </summary>
    ''' <param name="Id_Attivita">Indicare <tt>-1</tt> per non indicare filtri. Usato solo con <tt>enumSelezioneVariabile.Selezione_JoinCompleta</tt></param>
    ''' <param name="xSelezioneVariabile">
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_JoinDescrizioni</tt>: cerca di mettere in join con gruppi utenti</list>
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_JoinCompleta</tt>: cerca di mettere in join con gruppi utenti, carica anche Id_Attivita</list>
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_TabellaDatiMinimi</tt>: legge username, nome, cognome, rag_soc, CF, tipologia, validità permessi</list>
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_TabellaCompleta</tt>: legge tipologia, validità permessi e tutte le colonne da utenti e utenti_dettagli</list>
    ''' </param>
    Public Function Leggi_anchePermessi(
        ByVal IdServizio As Integer,
        ByVal Id_Attivita As Integer,
        ByVal xSelezioneVariabile As enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentiDettagli_R.Leggi_anchePermessi()"

        Dim strSql As New StringBuilder With {.Length = 0}
        Dim flagNoOrder = "-1"
        Dim dt As DataTable
        Dim sql2017 = False
        Dim major = VersioneSqlServer_Major(objParametri)
        If major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017 = True
        End If

        Try
            Dim creaCtePermessiNoIdAttivita = Sub()
                                                  strSql.AppendLine(" WITH Utenti_Permessicte As ( ")
                                                  strSql.AppendLine("   SELECT ")
                                                  strSql.AppendLine("     MIN(ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900')) AS Validita_Inizio, ")
                                                  strSql.AppendLine("     MAX(ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900')) AS Validita_Fine, ")
                                                  strSql.AppendLine("     Utenti_Permessi.UserName, Utenti_Permessi.Id_Servizio ")
                                                  strSql.AppendLine("   FROM Utenti_Permessi WITH(NOLOCK) ")
                                                  strSql.AppendLine("   GROUP BY Utenti_Permessi.UserName, Utenti_Permessi.Id_Servizio ")
                                                  strSql.AppendLine(" ) ")
                                              End Sub
            Dim qryCompletaConGruppi = Sub()
                                           If sql2017 Then
                                               strSql.AppendLine(" ,Utenti_Gruppi as ( ")
                                               strSql.AppendLine("   SELECT ")
                                               strSql.AppendLine("     Utenti_xGruppi_Utente.UserName, ")
                                               strSql.AppendLine("     STRING_AGG(Gruppi_Utente.Gruppi_Utente_cod, '|') as GruppiCod, ")
                                               strSql.AppendLine("     STRING_AGG(Gruppi_Utente.Gruppi_Utente_des, '|') as GruppiDes ")
                                               strSql.AppendLine("   FROM Utenti_xGruppi_Utente WITH(NOLOCK) ")
                                               strSql.AppendLine("     LEFT JOIN Gruppi_Utente WITH(NOLOCK) ON Gruppi_Utente.Gruppi_Utente_cod = Utenti_xGruppi_Utente.Gruppi_Utente_cod ")
                                               strSql.AppendLine("     GROUP BY Utenti_xGruppi_Utente.UserName ")
                                               strSql.AppendLine(" ) ")
                                           End If

                                           strSql.AppendLine(" SELECT ")
                                           strSql.AppendLine("     Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome ")
                                           strSql.AppendLine("   , Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, Utenti_Dettagli.UserNameCommerciale ")
                                           strSql.AppendLine("   , Utenti_Dettagli.Tel, Utenti_Dettagli.Email ")
                                           strSql.AppendLine("   , Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.Username_Creazione, Utenti_Dettagli.Username_Modifica ")
                                           strSql.AppendLine("   , Utenti.Password, Utenti.Flag_Encrypted ")
                                           strSql.AppendLine("   , Utenti.Piva_SuperUser, Utenti.Lingua_Cod ")
                                           strSql.AppendLine("   , Utenti.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des ")

                                           If sql2017 Then
                                               strSql.AppendLine("   , ISNULL(Utenti_Gruppi.GruppiCod, '') as GruppiCod ")
                                               strSql.AppendLine("   , ISNULL(Utenti_Gruppi.GruppiDes, '') as GruppiDes ")
                                           End If

                                           strSql.AppendLine("   , Utenti_Profili.Utente_Profilo ")
                                           strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio ")
                                           strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Fine, '01/01/1900') AS Validita_Fine ")
                                           strSql.AppendLine("   , IIF(Utenti_GDPR_Accettazione.Username IS NULL, 0, 1) AS GDPR ")
                                           strSql.AppendLine(" FROM Utenti_Profili WITH(NOLOCK) ")
                                           strSql.AppendLine("   INNER JOIN Utenti_Dettagli WITH(NOLOCK) ON Utenti_Profili.Utente = Utenti_Dettagli.UserName ")
                                           strSql.AppendLine("   LEFT OUTER JOIN  Utenti_Permessicte ON Utenti_Profili.Utente = Utenti_Permessicte.UserName And Utenti_Profili.Id_Servizio = Utenti_Permessicte.Id_Servizio ")
                                           strSql.AppendLine("   JOIN Utenti WITH(NOLOCK) ON Utenti.username = Utenti_Dettagli.UserName ")
                                           strSql.AppendLine("   LEFT JOIN Utenti_Tipologie WITH(NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod ")
                                           strSql.AppendLine("   LEFT JOIN Utenti_GDPR_Accettazione WITH(NOLOCK) ON Utenti.Username = Utenti_GDPR_Accettazione.Username ")

                                           If sql2017 Then
                                               strSql.AppendLine("   LEFT JOIN Utenti_Gruppi on Utenti_Gruppi.UserName = Utenti.UserName ")
                                           End If
                                       End Sub

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    If Id_Attivita <> -1 Then
                        strSql.AppendLine(" DECLARE @idAtt int = " & Agro_SQL_SaveNum(Id_Attivita) & "; ")
                    End If

                    strSql.AppendLine(" WITH Utenti_Permessicte As ( ")
                    strSql.AppendLine("   SELECT DISTINCT ")

                    If Id_Attivita <> -1 Then
                        strSql.AppendLine("     Utenti_Permessi.Id_Attivita ")
                    End If

                    strSql.AppendLine("     ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio, ")
                    strSql.AppendLine("     ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900') AS Validita_Fine, ")
                    strSql.AppendLine("     Utenti_Permessi.UserName, Utenti_Permessi.Id_Servizio ")
                    strSql.AppendLine("   FROM Utenti_Permessi WITH(NOLOCK) ")

                    If Id_Attivita <> -1 Then
                        strSql.AppendLine("   WHERE Utenti_Permessi.Id_Attivita = @idAtt ")
                    End If

                    strSql.AppendLine(" ) ")

                    qryCompletaConGruppi()

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    creaCtePermessiNoIdAttivita()
                    qryCompletaConGruppi()

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    creaCtePermessiNoIdAttivita()

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("     Utenti.UserName ")
                    strSql.AppendLine("   , Utenti_Dettagli.Nome, Utenti_Dettagli.Cognome, Utenti_Dettagli.Rag_Soc, Utenti_Dettagli.CodFisc ")
                    strSql.AppendLine("   , Utenti.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des ")
                    strSql.AppendLine("   , Utenti_Profili.Utente_Profilo ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Fine, '01/01/1900') AS Validita_Fine ")
                    strSql.AppendLine(" FROM Utenti_Profili WITH(NOLOCK) ")
                    strSql.AppendLine("   LEFT OUTER JOIN  Utenti_Permessicte ON Utenti_Profili.Utente = Utenti_Permessicte.UserName And Utenti_Profili.Id_Servizio = Utenti_Permessicte.Id_Servizio ")
                    strSql.AppendLine("   JOIN Utenti WITH(NOLOCK) ON Utenti.username = Utenti_Profili.Utente ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Tipologie WITH(NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Dettagli WITH(NOLOCK) ON Utenti.username = Utenti_Dettagli.username")

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    creaCtePermessiNoIdAttivita()

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("     Utenti.* ")
                    strSql.AppendLine("   , Utenti_Dettagli.* ")
                    strSql.AppendLine("   , Utenti.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des ")
                    strSql.AppendLine("   , Utenti_Profili.Utente_Profilo ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Fine, '01/01/1900') AS Validita_Fine ")
                    strSql.AppendLine(" FROM Utenti_Profili WITH(NOLOCK) ")
                    strSql.AppendLine("   LEFT OUTER JOIN  Utenti_Permessicte ON Utenti_Profili.Utente = Utenti_Permessicte.UserName And Utenti_Profili.Id_Servizio = Utenti_Permessicte.Id_Servizio ")
                    strSql.AppendLine("   JOIN Utenti WITH(NOLOCK) ON Utenti.username = Utenti_Profili.Utente ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Tipologie WITH(NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Dettagli WITH(NOLOCK) ON Utenti.username = Utenti_Dettagli.username")

            End Select

            strSql.AppendLine(" WHERE   Utenti_Profili.Utente_Profilo =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            strSql.AppendLine(" And Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(IdServizio))

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If String.IsNullOrWhiteSpace(xOrderBy) Then
                strSql.AppendLine(" ORDER BY Utenti.UserName ")
            ElseIf xOrderBy <> flagNoOrder Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function

    ''' <summary>
    ''' Lettura completa degli utenti.
    ''' Tabelle in join:
    ''' <list type="bullet">Utenti</list>
    ''' <list type="bullet">Utenti_Profili</list>
    ''' <list type="bullet">Utenti_Dettagli</list>
    ''' <list type="bullet">Utenti_Permessi</list>
    ''' <list type="bullet">Utenti_Tipologie</list>
    ''' <list type="bullet">Utenti_Gruppi (Solo se versione SQL è 2017 o superiore)</list>
    ''' <list type="bullet">AWS_log_cte</list>
    ''' </summary>
    ''' <param name="Id_Attivita">Indicare <tt>-1</tt> per non indicare filtri. Usato solo con <tt>enumSelezioneVariabile.Selezione_JoinCompleta</tt></param>
    ''' <param name="xSelezioneVariabile">
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_JoinDescrizioni</tt>: cerca di mettere in join con gruppi utenti</list>
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_JoinCompleta</tt>: cerca di mettere in join con gruppi utenti, carica anche Id_Attivita</list>
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_TabellaDatiMinimi</tt>: legge username, nome, cognome, rag_soc, CF, tipologia, validità permessi</list>
    ''' <list type="bullet"><tt>enumSelezioneVariabile.Selezione_TabellaCompleta</tt>: legge tipologia, validità permessi e tutte le colonne da utenti e utenti_dettagli</list>
    ''' </param>
    Public Function LeggiCompleta(
        ByVal IdServizio As Integer,
        ByVal Id_Attivita As Integer,
        ByVal xSelezioneVariabile As enumSelezioneVariabile,
        ByVal xFiltroAggiuntivo As String,
        ByVal xOrderBy As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable
        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentiDettagli_R.Leggi_anchePermessi()"

        Dim strSql As New StringBuilder With {.Length = 0}
        Dim flagNoOrder = "-1"
        Dim dt As DataTable
        Dim sql2017 = False
        Dim major = VersioneSqlServer_Major(objParametri)
        If major >= 14 Then 'major 14 corrisponde a sql server 2017 https://learn.microsoft.com/en-us/troubleshoot/sql/releases/download-and-install-latest-updates
            sql2017 = True
        End If

        Try
            Dim creaCtePermessiNoIdAttivita = Sub()
                                                  strSql.AppendLine(" WITH Utenti_Permessicte As ( ")
                                                  strSql.AppendLine("   SELECT ")
                                                  strSql.AppendLine("     MIN(ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900')) AS Validita_Inizio, ")
                                                  strSql.AppendLine("     MAX(ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900')) AS Validita_Fine, ")
                                                  strSql.AppendLine("     Utenti_Permessi.UserName, Utenti_Permessi.Id_Servizio ")
                                                  strSql.AppendLine("   FROM Utenti_Permessi WITH(NOLOCK) ")
                                                  strSql.AppendLine("   GROUP BY Utenti_Permessi.UserName, Utenti_Permessi.Id_Servizio ")
                                                  strSql.AppendLine(" ) ")
                                              End Sub
            Dim qryCompletaConGruppi = Sub()
                                           If sql2017 Then
                                               strSql.AppendLine(" ,Utenti_Gruppi as ( ")
                                               strSql.AppendLine("   SELECT ")
                                               strSql.AppendLine("     Utenti_xGruppi_Utente.UserName, ")
                                               strSql.AppendLine("     STRING_AGG(Gruppi_Utente.Gruppi_Utente_cod, '|') as GruppiCod, ")
                                               strSql.AppendLine("     STRING_AGG(Gruppi_Utente.Gruppi_Utente_des, '|') as GruppiDes ")
                                               strSql.AppendLine("   FROM Utenti_xGruppi_Utente WITH(NOLOCK) ")
                                               strSql.AppendLine("     LEFT JOIN Gruppi_Utente WITH(NOLOCK) ON Gruppi_Utente.Gruppi_Utente_cod = Utenti_xGruppi_Utente.Gruppi_Utente_cod ")
                                               strSql.AppendLine("     GROUP BY Utenti_xGruppi_Utente.UserName ")
                                               strSql.AppendLine(" ) ")
                                           End If
                                           strSql.AppendLine(" ,AWS_log_cte as ( ")
                                           strSql.AppendLine("   SELECT Utente_Username, MAX(Data_Richiesta) as UltimoAccesso, count(*) as NumeroAccessi ")
                                           strSql.AppendLine("   FROM AWS_log WITH(NOLOCK) ")
                                           strSql.AppendLine("   GROUP BY Utente_Username ")
                                           strSql.AppendLine(" ) ")

                                           strSql.AppendLine(" SELECT ")
                                           strSql.AppendLine("     Utenti_Dettagli.UserName, Utenti_Dettagli.Flag_Azienda_Persona, Utenti_Dettagli.Cognome, Utenti_Dettagli.Nome ")
                                           strSql.AppendLine("   , Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti_Dettagli.Rag_Soc, Utenti_Dettagli.UserNameCommerciale ")
                                           strSql.AppendLine("   , Utenti_Dettagli.Tel, Utenti_Dettagli.Email ")
                                           strSql.AppendLine("   , Utenti_Dettagli.Data_Creazione, Utenti_Dettagli.Data_Modifica, Utenti_Dettagli.Username_Creazione, Utenti_Dettagli.Username_Modifica ")
                                           strSql.AppendLine("   , Utenti.Password, Utenti.Flag_Encrypted ")
                                           strSql.AppendLine("   , Utenti.Piva_SuperUser, Utenti.Lingua_Cod ")
                                           strSql.AppendLine("   , Utenti.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des ")

                                           If sql2017 Then
                                               strSql.AppendLine("   , ISNULL(Utenti_Gruppi.GruppiCod, '') as GruppiCod ")
                                               strSql.AppendLine("   , ISNULL(Utenti_Gruppi.GruppiDes, '') as GruppiDes ")
                                           End If

                                           strSql.AppendLine("   , Utenti_Profili.Utente_Profilo ")
                                           strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio ")
                                           strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Fine, '01/01/1900') AS Validita_Fine ")
                                           strSql.AppendLine("   , IIF(Utenti_GDPR_Accettazione.Username IS NULL, 0, 1) AS GDPR ")
                                           strSql.AppendLine("   , AWS_log_cte.UltimoAccesso ")
                                           strSql.AppendLine("   , ISNULL(AWS_log_cte.NumeroAccessi, 0) as NumeroAccessi ")
                                           strSql.AppendLine(" FROM Utenti_Profili WITH(NOLOCK) ")
                                           strSql.AppendLine("   INNER JOIN Utenti_Dettagli WITH(NOLOCK) ON Utenti_Profili.Utente = Utenti_Dettagli.UserName ")
                                           strSql.AppendLine("   LEFT OUTER JOIN  Utenti_Permessicte ON Utenti_Profili.Utente = Utenti_Permessicte.UserName And Utenti_Profili.Id_Servizio = Utenti_Permessicte.Id_Servizio ")
                                           strSql.AppendLine("   JOIN Utenti WITH(NOLOCK) ON Utenti.username = Utenti_Dettagli.UserName ")
                                           strSql.AppendLine("   LEFT JOIN Utenti_Tipologie WITH(NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod ")
                                           strSql.AppendLine("   LEFT JOIN Utenti_GDPR_Accettazione WITH(NOLOCK) ON Utenti.Username = Utenti_GDPR_Accettazione.Username ")
                                           strSql.AppendLine("   LEFT JOIN AWS_log_cte on AWS_log_cte.Utente_Username = Utenti.UserName ")

                                           If sql2017 Then
                                               strSql.AppendLine("   LEFT JOIN Utenti_Gruppi on Utenti_Gruppi.UserName = Utenti.UserName ")
                                           End If
                                       End Sub

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    If Id_Attivita <> -1 Then
                        strSql.AppendLine(" DECLARE @idAtt int = " & Agro_SQL_SaveNum(Id_Attivita) & "; ")
                    End If

                    strSql.AppendLine(" WITH Utenti_Permessicte As ( ")
                    strSql.AppendLine("   SELECT DISTINCT ")

                    If Id_Attivita <> -1 Then
                        strSql.AppendLine("     Utenti_Permessi.Id_Attivita ")
                    End If

                    strSql.AppendLine("     ISNULL(Utenti_Permessi.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio, ")
                    strSql.AppendLine("     ISNULL(Utenti_Permessi.Validita_Fine, '01/01/1900') AS Validita_Fine, ")
                    strSql.AppendLine("     Utenti_Permessi.UserName, Utenti_Permessi.Id_Servizio ")
                    strSql.AppendLine("   FROM Utenti_Permessi WITH(NOLOCK) ")

                    If Id_Attivita <> -1 Then
                        strSql.AppendLine("   WHERE Utenti_Permessi.Id_Attivita = @idAtt ")
                    End If

                    strSql.AppendLine(" ) ")

                    qryCompletaConGruppi()

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    creaCtePermessiNoIdAttivita()
                    qryCompletaConGruppi()

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    creaCtePermessiNoIdAttivita()

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("     Utenti.UserName ")
                    strSql.AppendLine("   , Utenti_Dettagli.Nome, Utenti_Dettagli.Cognome, Utenti_Dettagli.Rag_Soc, Utenti_Dettagli.CodFisc ")
                    strSql.AppendLine("   , Utenti.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des ")
                    strSql.AppendLine("   , Utenti_Profili.Utente_Profilo ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Fine, '01/01/1900') AS Validita_Fine ")
                    strSql.AppendLine(" FROM Utenti_Profili WITH(NOLOCK) ")
                    strSql.AppendLine("   LEFT OUTER JOIN  Utenti_Permessicte ON Utenti_Profili.Utente = Utenti_Permessicte.UserName And Utenti_Profili.Id_Servizio = Utenti_Permessicte.Id_Servizio ")
                    strSql.AppendLine("   JOIN Utenti WITH(NOLOCK) ON Utenti.username = Utenti_Profili.Utente ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Tipologie WITH(NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Dettagli WITH(NOLOCK) ON Utenti.username = Utenti_Dettagli.username")

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    creaCtePermessiNoIdAttivita()

                    strSql.AppendLine(" SELECT ")
                    strSql.AppendLine("     Utenti.* ")
                    strSql.AppendLine("   , Utenti_Dettagli.* ")
                    strSql.AppendLine("   , Utenti.Tipologia_Cod, Utenti_Tipologie.Tipologia_Des ")
                    strSql.AppendLine("   , Utenti_Profili.Utente_Profilo ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Inizio, '01/01/1900' ) AS Validita_Inizio ")
                    strSql.AppendLine("   , ISNULL(Utenti_Permessicte.Validita_Fine, '01/01/1900') AS Validita_Fine ")
                    strSql.AppendLine(" FROM Utenti_Profili WITH(NOLOCK) ")
                    strSql.AppendLine("   LEFT OUTER JOIN  Utenti_Permessicte ON Utenti_Profili.Utente = Utenti_Permessicte.UserName And Utenti_Profili.Id_Servizio = Utenti_Permessicte.Id_Servizio ")
                    strSql.AppendLine("   JOIN Utenti WITH(NOLOCK) ON Utenti.username = Utenti_Profili.Utente ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Tipologie WITH(NOLOCK) ON Utenti.Tipologia_Cod = Utenti_Tipologie.Tipologia_Cod ")
                    strSql.AppendLine("   LEFT JOIN Utenti_Dettagli WITH(NOLOCK) ON Utenti.username = Utenti_Dettagli.username")

            End Select

            strSql.AppendLine(" WHERE   Utenti_Profili.Utente_Profilo =  '" & Agro_SQL_SaveText(objParametri.SuperUserUsername) & "' ")
            strSql.AppendLine(" And Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(IdServizio))

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If String.IsNullOrWhiteSpace(xOrderBy) Then
                strSql.AppendLine(" ORDER BY Utenti.UserName ")
            ElseIf xOrderBy <> flagNoOrder Then
                strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function


    '##############################################################################################
    Public Function Utenti_Dettagli_from_CF(ByVal CF As String,
                                            ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.NewCom_Utenti_Dettagli_from_CF()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  Utenti_Dettagli.* ")
                    strSql.AppendLine(" FROM  Utenti_Dettagli ")
                    strSql.AppendLine(" WHERE  CodFisc  = '" & Agro_SQL_SaveText(CF) & "'  ")

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0
                    strSql.AppendLine(" SELECT  Utenti_Dettagli.* ")
                    strSql.AppendLine(" FROM  Utenti_Dettagli ")
                    strSql.AppendLine(" WHERE  CodFisc  = '" & Agro_SQL_SaveText(CF) & "'  ")

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    '##############################################################################################
    Public Function Utenti_Dettagli_from_USERNAME(ByVal USERNAME As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.Utenti_Dettagli_from_USERNAME()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT  Utenti_Dettagli.* ")
            strSql.AppendLine(" FROM  Utenti_Dettagli ")
            strSql.AppendLine(" WHERE  USERNAME  = '" & Agro_SQL_SaveText(USERNAME) & "'  ")

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

    Public Function Dettagli_SuperUser_from_Piva(ByVal PivaSuperUser As String,
                                                 ByRef objConnessione As DbConnection,
                                                 ByVal StringaConnessione As String,
                                                 ByVal FlagVisibilita As Integer,
                                                 ByVal DirectoryLOG As String,
                                                 ByVal FileLOG As String,
                                                 ByVal IdentificatoreUtente As String,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.Dettagli_SuperUser_from_Piva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  Utenti_Dettagli.UserName, Utenti_Dettagli.Cognome, Utenti_Dettagli.PIVA, Utenti_Dettagli.CodFisc, Utenti.Password, ProgressivoGIAS ")
            strSql.AppendLine(" FROM  Utenti_Dettagli INNER JOIN Utenti ON Utenti_Dettagli.UserName = Utenti.UserName ")
            strSql.AppendLine("     INNER JOIN [Utenti_CodiciGiasPro] on [Utenti_CodiciGiasPro].username = Utenti.username ")
            strSql.AppendLine(" WHERE  CodFisc  = '" & Agro_SQL_SaveText(PivaSuperUser) & "'  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, strSql.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Dim customLOGParams As New CustomLOGParams With {
                .LogDescrizioneUtente = IdentificatoreUtente,
                .LogDirectory = DirectoryLOG,
                .LogFileName = FileLOG
            }
            Scrivi_LOG(objParametri,
                       nomeRoutine,
                       messaggioErrore,
                       CustomLOGParams:=customLOGParams)

            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Username_From_CodFisc(ByVal codFisc As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As String

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.Username_From_CodFisc()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            strSql.AppendLine(" SELECT  Username ")
            strSql.AppendLine(" FROM  Utenti_Dettagli ")
            strSql.AppendLine(" WHERE  CodFisc  = '" & Agro_SQL_SaveText(codFisc) & "'  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 1 Then
                Return dt.Rows(0).Item("Username")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ""

    End Function


    '##############################################################################################
    Public Function CodFisc_From_Email(ByVal email As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.CodFisc_From_Email()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  CodFisc ")
            strSql.AppendLine(" FROM  Utenti_Dettagli ")
            strSql.AppendLine(" WHERE  EMail  = '" & Agro_SQL_SaveText(email) & "'  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 1 Then
                Return dt.Rows(0).Item("CodFisc")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ""

    End Function

    '##############################################################################################
    Public Function CodFisc_From_Username(ByVal Username As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As String

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.CodFisc_From_Username()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT  CodFisc ")
            strSql.AppendLine(" FROM  Utenti_Dettagli ")
            strSql.AppendLine(" WHERE  Username  = '" & Agro_SQL_SaveText(Username) & "'  ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 1 Then
                Return dt.Rows(0).Item("CodFisc")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ""

    End Function

    '##############################################################################################
    Public Function NomeCognome_From_Username(ByVal username As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As String

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.NomeCognome_From_Username()"

        Dim messaggioErrore As String = ""
        Dim strNomeCognome As String
        Dim dt As DataTable

        Try
            'Cerco tra i codici fiscali ...
            dt = Utenti_Dettagli_from_USERNAME(username, objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                '--- Trovato il codice fiscale ---

                If dt.Rows(0).Item("Flag_Azienda_Persona") = 1 Then
                    'AZIENDA
                    strNomeCognome = dt.Rows(0).Item("Rag_Soc")
                Else
                    'PERSONA
                    strNomeCognome = dt.Rows(0).Item("Nome") & " " & dt.Rows(0).Item("Cognome")
                End If

            Else

                '--- Nessun codice fiscale corrisponde ---

                'Cerco tra le username ...
                dt = Leggi(objParametri.UtenteCodFiscale,
                           5,
                           enumSelezioneVariabile.Selezione_JoinDescrizioni,
                           "", "", objParametri)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                    '--- Trovata una Username ---

                    If dt.Rows(0).Item("Flag_Azienda_Persona") = 1 Then
                        'AZIENDA
                        strNomeCognome = dt.Rows(0).Item("Rag_Soc") & "  (#)"
                    Else
                        'PERSONA
                        strNomeCognome = dt.Rows(0).Item("Nome") & " " & dt.Rows(0).Item("Cognome") & "  (#)"
                    End If

                Else

                    '--- Nessuna Username corrisponde ---

                    strNomeCognome = "Utente non presente in archivio"

                End If

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strNomeCognome

    End Function

    '##############################################################################################
    Public Function NomeCognome_From_CodFisc(ByVal cf As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As String

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli.NomeCognome_From_CodFisc()"


        Dim messaggioErrore As String = ""
        Dim strNomeCognome As String = ""
        Dim dt As DataTable

        Try
            'Cerco tra i codici fiscali ...
            dt = Utenti_Dettagli_from_CF(cf, enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then

                '--- Trovato il codice fiscale ---

                If dt.Rows(0).Item("Flag_Azienda_Persona") = 1 Then
                    'AZIENDA
                    strNomeCognome = dt.Rows(0).Item("Rag_Soc")
                Else
                    'PERSONA
                    strNomeCognome = dt.Rows(0).Item("Nome") & " " & dt.Rows(0).Item("Cognome")
                End If

            Else

                ''--- Nessun codice fiscale corrisponde ---

                ''Cerco tra le username ...
                'DT = Leggi(objParametri.UtenteCodFiscale,
                '           5,
                '           enumSelezioneVariabile.Selezione_JoinDescrizioni,
                '           "", "", objParametri)

                'If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

                '    '--- Trovata una Username ---

                '    If DT.Rows(0).Item("Flag_Azienda_Persona") = 1 Then
                '        'AZIENDA
                '        StrNomeCognome = DT.Rows(0).Item("Rag_Soc") & "  (#)"
                '    Else
                '        'PERSONA
                '        StrNomeCognome = DT.Rows(0).Item("Nome") & " " & DT.Rows(0).Item("Cognome") & "  (#)"
                '    End If

                'Else

                '    '--- Nessuna Username corrisponde ---

                '    StrNomeCognome = "Utente non presente in archivio"

                'End If

            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return strNomeCognome

    End Function

    Function UserName_From_Email(ByVal email As String, ByVal ObjParametri_Utenti As AgronicaCoreParametri) As String

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli_R.UserName_From_Email()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine(" Select * ")
            stb.AppendLine(" From Utenti_Dettagli d ")
            stb.AppendLine(" Where email = '" & Agro_SQL_SaveText(email) & "'")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Utenti, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        If dt.Rows.Count() > 0 Then
            Return dt.Rows(0)("UserName")
        End If

        Return ""

    End Function

    Function Utenti_From_Email(ByVal email As String, ByVal ObjParametri_Utenti As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli_R.UserName_From_Email()"
        Dim stb As New StringBuilder With {.Length = 0}
        Dim dt As DataTable

        Try
            stb.AppendLine(" Select * ")
            stb.AppendLine(" From Utenti_Dettagli d ")
            stb.AppendLine(" Where email = '" & Agro_SQL_SaveText(email) & "'")
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Utenti, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(ObjParametri_Utenti, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

    Function LeggiTokenResetCredenziali(ByVal token As String, ByVal username As String, ByVal ObjParametri_Utenti As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli_R.LeggiTokenResetCredenziali()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0
            stb.AppendLine("SELECT * ")
            stb.AppendLine("FROM Utenti_Reset_Password ")
            stb.AppendLine("WHERE token = '" & Agro_SQL_SaveText(token) & "'")
            stb.AppendLine("AND username = '" & Agro_SQL_SaveText(username) & "'")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Utenti, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class Utenti_Dettagli_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '#########################################################################
    ''' -----------------------------------------------------------------------------
    ''' <history>
    ''' 	[pierantoni]	16/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Scrivi(ByVal UserName As String,
                           ByVal Cognome As String,
                           ByVal Nome As String,
                           ByVal Via As String,
                           ByVal Numero As String,
                           ByVal Citta As String,
                           ByVal Provincia As String,
                           ByVal CAP As String,
                           ByVal Tel As String,
                           ByVal Fax As String,
                           ByVal Email As String,
                           ByVal PIVA As String,
                           ByVal CodFisc As String,
                           ByVal Rag_Soc As String,
                           ByVal Flag_Azienda_Persona As Integer,
                           ByVal Flag_Contattabile As Integer,
                           ByVal UserNameCommerciale As String,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal cellulare As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Dettagli_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("INSERT INTO Utenti_Dettagli (")
            strSql.AppendLine("                    UserName ")
            strSql.AppendLine("                   , Cognome ")
            strSql.AppendLine("                   , Nome ")
            strSql.AppendLine("                   , Via ")
            strSql.AppendLine("                   , Numero ")
            strSql.AppendLine("                   , Citta ")
            strSql.AppendLine("                   , Provincia ")
            strSql.AppendLine("                   , CAP ")
            strSql.AppendLine("                   , Tel ")
            strSql.AppendLine("                   , Fax ")
            strSql.AppendLine("                   , Email ")
            strSql.AppendLine("                   , PIVA ")
            strSql.AppendLine("                   , CodFisc ")
            strSql.AppendLine("                   , Rag_Soc ")
            strSql.AppendLine("                   , Flag_Azienda_Persona ")
            strSql.AppendLine("                   , Flag_Contattabile ")
            strSql.AppendLine("                   , Flag ")
            strSql.AppendLine("                   , Data_Creazione ")
            strSql.AppendLine("                   , Data_Modifica ")
            strSql.AppendLine("                   , Username_Creazione")
            strSql.AppendLine("                   , Username_Modifica ")
            strSql.AppendLine("                   , UserNameCommerciale ")

            If cellulare <> "" Then
                strSql.AppendLine("                   , cellulare ")
            End If

            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(UserName.Trim.ToLower) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Cognome) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Nome) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Via) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Numero) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Citta) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Provincia) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(CAP) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Tel) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Fax) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Email) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(PIVA) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(CodFisc) & "' ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Rag_Soc) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Azienda_Persona) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Flag_Contattabile) & "  ")

            strSql.AppendLine("         , 0 ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'  ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(UserNameCommerciale) & "' ")

            If cellulare <> "" Then
                strSql.AppendLine("                   ,'" & Agro_SQL_SaveText(cellulare) & "' ")
            End If

            strSql.AppendLine(")")

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

    Public Function Scrivi(ByVal UserName As String,
                           ByVal Cognome As String,
                           ByVal Nome As String,
                           ByVal Tel As String,
                           ByVal Email As String,
                           ByVal PIVA As String,
                           ByVal CodFisc As String,
                           ByVal Rag_Soc As String,
                           ByVal Flag_Azienda_Persona As Integer,
                           ByVal UserNameCommerciale As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean
        Return Scrivi(UserName, Cognome, Nome,
                      String.Empty, String.Empty, String.Empty, String.Empty, String.Empty,
                      Tel, String.Empty, Email,
                      PIVA, CodFisc, Rag_Soc,
                      Flag_Azienda_Persona, 0, UserNameCommerciale,
                      objParametri)
    End Function

    '#########################################################################
    ''' -----------------------------------------------------------------------------
    ''' <history>
    ''' 	[pierantoni]	16/03/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Modifica(ByVal UserName As String,
                             ByVal Cognome As String,
                             ByVal Nome As String,
                             ByVal Via As String,
                             ByVal Numero As String,
                             ByVal Citta As String,
                             ByVal Provincia As String,
                             ByVal CAP As String,
                             ByVal Tel As String,
                             ByVal Fax As String,
                             ByVal Email As String,
                             ByVal PIVA As String,
                             ByVal CodFisc As String,
                             ByVal Rag_Soc As String,
                             ByVal Flag_Azienda_Persona As Integer,
                             ByVal Flag_Contattabile As Integer,
                             ByVal UserNameCommerciale As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Cellulare As String = ""
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Dettagli_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("UPDATE Utenti_Dettagli SET ")
            strSql.AppendLine("       Cognome                  = '" & Agro_SQL_SaveText(Cognome) & "' ")
            strSql.AppendLine("       ,Nome                    = '" & Agro_SQL_SaveText(Nome) & "' ")
            strSql.AppendLine("       ,Via                     = '" & Agro_SQL_SaveText(Via) & "' ")
            strSql.AppendLine("       ,Numero                  = '" & Agro_SQL_SaveText(Numero) & "' ")
            strSql.AppendLine("       ,Citta                   = '" & Agro_SQL_SaveText(Citta) & "' ")
            strSql.AppendLine("       ,Provincia               = '" & Agro_SQL_SaveText(Provincia) & "' ")
            strSql.AppendLine("       ,CAP                     = '" & Agro_SQL_SaveText(CAP) & "' ")
            strSql.AppendLine("       ,Tel                     = '" & Agro_SQL_SaveText(Tel) & "' ")
            strSql.AppendLine("       ,Fax                     = '" & Agro_SQL_SaveText(Fax) & "' ")
            strSql.AppendLine("       ,Email                   = '" & Agro_SQL_SaveText(Email) & "' ")
            strSql.AppendLine("       ,PIVA                    = '" & Agro_SQL_SaveText(PIVA) & "' ")
            strSql.AppendLine("       ,CodFisc                 = '" & Agro_SQL_SaveText(CodFisc) & "' ")
            strSql.AppendLine("       ,Rag_Soc                 = '" & Agro_SQL_SaveText(Rag_Soc) & "' ")
            strSql.AppendLine("       ,Flag_Azienda_Persona    =  " & Agro_SQL_SaveNum(Flag_Azienda_Persona) & "  ")
            strSql.AppendLine("       ,Flag_Contattabile       =  " & Agro_SQL_SaveNum(Flag_Contattabile) & "  ")
            strSql.AppendLine("       ,UserNameCommerciale     = '" & Agro_SQL_SaveText(UserNameCommerciale) & "' ")
            strSql.AppendLine("       ,Data_Modifica           =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.AppendLine("       ,Username_Modifica           =  '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "' ")

            If Cellulare <> "" Then
                strSql.AppendLine("       ,cellulare     = '" & Agro_SQL_SaveText(Cellulare) & "' ")
            End If

            strSql.AppendLine(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "'")

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

    Public Function Modifica_Parametrizzata(ByVal Username As String,
                                            ByVal Campo As String,
                                            ByVal Valore As Object,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri,
                                            Optional ByVal Flag_DataModifica As Boolean = False
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli_W.Modifica_Parametrizzata()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False
        Dim strAssegnamento As String = String.Empty

        ' a seconda del tipo del valore che devo aggiornare, formatto la query
        Dim stringa As Type = GetType(System.String)
        Dim data As Type = GetType(System.DateTime)
        Dim intero32 As Type = GetType(System.Int32)

        Try
            If Username = "" Then
                Throw New Exception("Parametro non corretto nella query (Username obbligatorio)")
            End If

            '---------------------------------------------

            Dim typeVal As Type = Valore.GetType()

            If typeVal.Equals(stringa) Then
                strAssegnamento = Campo & "= '" & Agro_SQL_SaveText(Valore.ToString) & "' "
            ElseIf typeVal.Equals(data) Then
                strAssegnamento = Campo & "= " & Agro_SQL_SaveDate(Valore.ToString) & " "
            Else
                strAssegnamento = Campo & "= " & Agro_SQL_SaveNum(Valore.ToString) & " "
            End If

            '---------------------------------------------

            strSql.Length = 0
            strSql.Append("UPDATE Utenti_Dettagli SET ")

            strSql.Append(strAssegnamento)

            If Flag_DataModifica Then
                strSql.Append(", Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now))
            End If

            strSql.Append(" WHERE Username = '" & Agro_SQL_SaveText(Trim(Username)) & "'")

            '---------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
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

    Public Function Modifica_TipologiaUtente(ByVal Username As String,
                                             ByVal Tipologia_Cod As String,
                                             ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli_W.Modifica_TipologiaUtente()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("UPDATE Utenti SET ")
            strSql.AppendLine("       Tipologia_Cod                  = '" & Agro_SQL_SaveText(Tipologia_Cod) & "' ")
            strSql.AppendLine("       ,Data_Modifica           =  " & Agro_SQL_SaveDate(Date.Now))

            strSql.AppendLine(" WHERE UserName = '" & Agro_SQL_SaveText(Username) & "'")

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

    Public Function Modifica_PivaSuperUserUtente(ByVal Username As String,
                                                 ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Dettagli_W.Modifica_PivaSuperUserUtente()"
        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False
        Dim Piva_SuperUser = objParametri.PivaSuperUser

        Try

            strSql.Length = 0

            strSql.AppendLine("UPDATE Utenti SET ")
            strSql.AppendLine("       Piva_SuperUser           = '" & Agro_SQL_SaveText(Piva_SuperUser) & "' ")
            strSql.AppendLine("       ,Data_Modifica           =  " & Agro_SQL_SaveDate(Date.Now))

            strSql.AppendLine(" WHERE UserName = '" & Agro_SQL_SaveText(Username) & "'")

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

    Public Function ModificaDettagliBasePersona(
        username As String, cognome As String, nome As String, tel As String, email As String, CF As String,
        usernameCommerciale As String, objParametri_Utenti As AgronicaCoreParametri
    )
        Dim flagPersona = 2
        Return Modifica(
            username, cognome, nome, Via:=String.Empty, Numero:=String.Empty, Citta:=String.Empty,
            Provincia:=String.Empty, CAP:=String.Empty, tel, Fax:=String.Empty, email, PIVA:=String.Empty,
            CF, Rag_Soc:=String.Empty, flagPersona, 0, usernameCommerciale,
            objParametri_Utenti
        )
    End Function

    Public Function ModificaDettagliBaseAzienda(
        username As String, piva As String, rag_soc As String, tel As String, email As String, CF As String,
        usernameCommerciale As String, objParametri_Utenti As AgronicaCoreParametri
    )
        Dim flagAzienda = 1
        Return Modifica(
            username, Cognome:=String.Empty, Nome:=String.Empty, Via:=String.Empty, Numero:=String.Empty,
            Citta:=String.Empty, Provincia:=String.Empty, CAP:=String.Empty, tel, Fax:=String.Empty, email,
            piva, CF, rag_soc, flagAzienda, 0, usernameCommerciale,
            objParametri_Utenti
        )
    End Function

    Public Function ScriviTokenResetCredenziali(ByVal Token As String,
                                                ByVal UserName As String,
                                                ByVal PeriodoValidita As Integer,
                                                ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Dettagli_W.ScriviTokenResetCredenziali()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine("INSERT INTO Utenti_Reset_Password (")
            strSql.AppendLine("                     Token ")
            strSql.AppendLine("                   , UserName ")
            strSql.AppendLine("                   , Timestamp_Generazione_Token ")
            strSql.AppendLine("                   , Timestamp_Scadenza_Token")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Token.Trim.ToLower) & "' ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(UserName.Trim.ToLower) & "' ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine($"        , DATEADD(MINUTE, {PeriodoValidita}, GETDATE()) ")
            strSql.AppendLine(")")

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
