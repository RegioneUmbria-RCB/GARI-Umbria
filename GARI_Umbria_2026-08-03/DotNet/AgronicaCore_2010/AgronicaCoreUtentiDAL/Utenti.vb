Imports System.Data.Common
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class Utenti_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    'Public Function Leggi_Da_Gias_Server(ByVal Username_Utente As String,
    '                                     ByVal FiltroAggiuntivo As String,
    '                                     ByRef objConnessione As DbConnection,
    '                                     ByVal StringaConnessione As String,
    '                                     ByVal FlagVisibilita As Int32,
    '                                     ByVal DirectoryLOG As String,
    '                                     ByVal FileLOG As String,
    '                                     ByVal IdentificatoreUtente As String
    '                                     ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_Da_Gias_Server()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Username_Utente
    '    '   FiltroAggiuntivo
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try
    '        '---------------------------------------------

    '        StrSQL.Length = 0
    '        StrSQL.Append(" SELECT * ")
    '        StrSQL.Append(" FROM    Utenti ")
    '        StrSQL.Append(" WHERE [USER] = '" & Agro_SQL_SaveText(Username_Utente) & "'  ")

    '        If FiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" " & FiltroAggiuntivo)
    '        End If

    '        Select Case FlagVisibilita
    '            Case 1  'Solo i NON CANCELLATI
    '                StrSQL.Append(" AND Inviato >= 0 ")
    '            Case 2  'Solo i CANCELLATI
    '                StrSQL.Append(" AND Inviato = -1 ")
    '            Case 3  'TUTTI
    '                '
    '            Case Else
    '                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '        End Select

    '        '---------------------------------------------

    '        DT = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine)

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT

    'End Function


    Public Function Leggi(ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal username As String = "") As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM    Utenti WITH(NOLOCK)")
                    StrSQL.Append(" WHERE 1=1 ")

                    If username <> "" Then
                        StrSQL.Append(" AND Username = '" & Agro_SQL_SaveText(username) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
            End Select

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


    Public Function LeggixLingue(ByVal username As String,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT Utenti.Username, Lingue.Lingua_Cod, Lingue.Descrizione, Lingue.CodiceISO, Lingue.Nome ")
                    StrSQL.Append(" FROM Utenti ")
                    StrSQL.Append(" JOIN Lingue ON Utenti.Lingua_Cod = Lingue.Lingua_Cod ")
                    StrSQL.Append(" WHERE 1=1 ")

                    If username <> "" Then
                        StrSQL.Append(" AND Utenti.Username = '" & Agro_SQL_SaveText(username) & "'")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If
            End Select

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

    '###################################################################################
    Public Function Esiste_Lingua(ByVal CodiceISO As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Esiste_Lingua()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = LeggiLingue(CodiceISO, "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function


    Public Function AgroWS_LoginSecure_Utente(ByVal UserName As String,
                                              ByVal Password As String,
                                              ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByVal xOrderBy As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.AgroWS_LoginSecure_Utente()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    stb.Length = 0

                    '--- Creo la Query SQL
                    stb.AppendLine(" Declare @Username varchar(100) ")
                    stb.AppendLine(" Declare @Password varchar(100) ")
                    stb.AppendLine(" Set @Username = '" & Agro_SQL_SaveText(UserName) & "' ")
                    stb.AppendLine(" Set @Password = '" & Agro_SQL_SaveText(Password) & "' ")

                    stb.AppendLine(" Select top 1 Utenti.UserName, Utenti_Dettagli.PIVA as PivaSuperUser   ")
                    stb.AppendLine("  ")
                    stb.AppendLine(" FROM    Utenti INNER JOIN  ")
                    stb.AppendLine("         Utenti_Dettagli ON Utenti.UserName = Utenti_Dettagli.UserName LEFT OUTER JOIN  ")
                    stb.AppendLine("         Utenti_Profili ON Utenti.UserName = Utenti_Profili.Utente LEFT OUTER JOIN  ")
                    stb.AppendLine("         Utenti_CodiciGiasPro ON Utenti.UserName = Utenti_CodiciGiasPro.UserName  ")
                    stb.AppendLine("  ")
                    stb.AppendLine(" WHERE   (Utenti.UserName = @Username) ")
                    stb.AppendLine(" And     (Utenti.Password = @Password)  ")

                    If xFiltroAggiuntivo <> "" Then
                        stb.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        stb.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    'Funzione chiamata dalla pagina starterkit_utenti_edit.aspx per la verifica dell'esistenza del nome del nuovo utente,
    'dunque è stato volontariamente omesso l'objparametri.
    'Public Function Leggi2(ByVal UserName As String,
    '                       ByVal xSelezioneVariabile As enumSelezioneVariabile,
    '                       ByVal xFiltroAggiuntivo As String,
    '                       ByVal xOrderBy As String,
    '                       ByRef objConnessione As DbConnection,
    '                       ByVal StringaConnessione As String,
    '                       ByVal FlagVisibilita As Int32,
    '                       ByVal DirectoryLOG As String,
    '                       ByVal FileLOG As String,
    '                       ByVal IdentificatoreUtente As String
    '                       ) As DataTable

    '    Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi2()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '   Username_Utente
    '    '   FiltroAggiuntivo
    '    '
    '    '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
    '    '   DirectoryLOG = ""           =>  viene usato il valore di default
    '    '   FileLOG = ""                =>  viene usato il valore di default
    '    '====================================================================================

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New Text.StringBuilder
    '    Dim dt As DataTable

    '    Try
    '        '---------------------------------------------
    '        Select Case xSelezioneVariabile

    '            Case enumSelezioneVariabile.Selezione_TabellaCompleta

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT * ")
    '                StrSQL.Append(" FROM  Utenti, Utenti_Dettagli ")
    '                StrSQL.Append(" WHERE Utenti.Username = Utenti_Dettagli.Username" & "  ")

    '                If UserName <> "" Then
    '                    StrSQL.Append(" AND Utenti.UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
    '                End If

    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , Nothing))
    '                End If

    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, Nothing))
    '                Else
    '                    StrSQL.Append(" ORDER BY Utenti.UserName ASC")
    '                End If

    '        End Select

    '        '--------------------------------------------------------------------------
    '        dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, StrSQL.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        messaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
    '        dt = Nothing
    '        Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
    '    End Try

    '    Return dt

    'End Function


    Public Function Leggi2(ByVal UserName As String,
                           ByVal xFiltroAggiuntivo As String,
                           ByVal xOrderBy As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi2()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Utenti, Utenti_Dettagli ")
            StrSQL.Append(" WHERE Utenti.Username = Utenti_Dettagli.Username" & "  ")

            If UserName <> "" Then
                StrSQL.Append(" AND Utenti.UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Utenti.UserName ASC")
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


    Public Function LeggiLockTentativi(ByVal UserName As String,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.LeggiLockTentativi()"

        '====================================================================================
        'Parametri opzionali :
        '   Username_Utente
        '   FiltroAggiuntivo
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT LockTentativi ")
            StrSQL.Append(" FROM  Utenti ")
            StrSQL.Append(" WHERE 1=1 ")

            If UserName <> "" Then
                StrSQL.Append(" AND Utenti.UserName = '" & Agro_SQL_SaveText(UserName) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Utenti.UserName ASC")
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



    Public Function Leggi_Da_Gias_Server(ByVal Username_Utente As String,
                                         ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_Da_Gias_Server()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM    Utenti ")
                    StrSQL.Append(" WHERE [USER] = '" & Agro_SQL_SaveText(Username_Utente) & "'  ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM    Utenti ")
                    StrSQL.Append(" WHERE [USER] = '" & Agro_SQL_SaveText(Username_Utente) & "'  ")

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta


            End Select

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

    Public Function Leggi_Da_Gias_Server(ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_Da_Gias_Server()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable
        Try
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM    Utenti ")
            StrSQL.Append(" WHERE 1=1  ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return dt
    End Function


    '##############################################################################################
    Public Function Leggi_DatiUtente_e_DatiSuperUser(ByVal UserName As String,
                                                     ByVal UserName_Profilo As String,
                                                     ByVal Password As String,
                                                     ByVal Id_Operazione As Integer,
                                                     ByVal Id_Attivita As Integer,
                                                     ByVal Data_Accesso As Date,
                                                     ByVal Ora_Accesso As Integer,
                                                     ByVal Flag_LeggiPermessiUtente As Boolean,
                                                     ByVal Id_Servizio As Integer,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_DatiUtente_e_DatiSuperUser()"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0
            stbQuery.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            stbQuery.AppendLine(" SELECT  Utenti.UserName, Utenti.Password, Utenti.Flag_Encrypted, Utenti.Lingua_Cod, ")

            stbQuery.AppendLine("         Dettagli_Utenti.Flag_Azienda_Persona,  ")
            stbQuery.AppendLine("         Dettagli_Utenti.Cognome,  ")
            stbQuery.AppendLine("         Dettagli_Utenti.Nome,  ")
            stbQuery.AppendLine("         Dettagli_Utenti.CodFisc, ")
            stbQuery.AppendLine("         Dettagli_Utenti.Rag_Soc,  ")
            stbQuery.AppendLine("         Dettagli_Utenti.PIVA,  ")
            stbQuery.AppendLine("         Dettagli_Utenti.UserNameCommerciale,  ")
            stbQuery.AppendLine("         Dettagli_Utenti.Flag_Azienda_Persona AS Utente_Flag_Azienda_Persona,  ")

            stbQuery.AppendLine("         Utenti_CodiciGiasPro.ProgressivoGIAS, Utenti_CodiciGiasPro.GiasOnline_Key, Utenti_CodiciGiasPro.CD_Key, ")

            stbQuery.AppendLine("         Utenti_Profili.Utente_Profilo, Utenti_Profili.Id_Servizio, ")
            stbQuery.AppendLine("         Utenti_Profili.Descrizione_1, Utenti_Profili.Descrizione_2,  ")

            stbQuery.AppendLine("         Dettagli_SuperUser.PIVA AS Piva_SuperUser, ")
            stbQuery.AppendLine("         Dettagli_SuperUser.CodFisc AS CodFisc_SuperUser, ")
            stbQuery.AppendLine("         Dettagli_SuperUser.Rag_Soc AS RagSoc_SuperUser,  ")

            stbQuery.AppendLine("         SuperUser.UserName AS Username_SuperUser, SuperUser.Password AS Password_SuperUser ")

            If Flag_LeggiPermessiUtente Then
                stbQuery.AppendLine("   , Utenti_Permessi.Id_Attivita, ")
                stbQuery.AppendLine("   Utenti_Permessi.Id_Operazione, ")
                stbQuery.AppendLine("   Utenti_Permessi.ID,  ")
                stbQuery.AppendLine("   Utenti_Permessi.Validita_Inizio, Utenti_Permessi.Validita_Fine   ")
            End If

            stbQuery.AppendLine("         , Dettagli_Utenti.email ")

            stbQuery.AppendLine(" FROM    Utenti (NOLOCK) INNER JOIN ")
            stbQuery.AppendLine("         Utenti_Dettagli Dettagli_Utenti   (NOLOCK) ON Utenti.UserName = Dettagli_Utenti.UserName  ")
            stbQuery.AppendLine("         INNER JOIN Utenti_Profili  (NOLOCK) ON Utenti.UserName = Utenti_Profili.Utente ")
            stbQuery.AppendLine("         INNER JOIN Utenti_CodiciGiasPro  (NOLOCK) ON Utenti_Profili.Utente_Profilo = Utenti_CodiciGiasPro.UserName ")
            stbQuery.AppendLine("         INNER JOIN Utenti_Dettagli Dettagli_SuperUser  (NOLOCK) ON Utenti_Profili.Utente_Profilo = Dettagli_SuperUser.UserName ")
            stbQuery.AppendLine("         INNER JOIN Utenti SuperUser  (NOLOCK) ON Dettagli_SuperUser.UserName = SuperUser.UserName ")

            If Flag_LeggiPermessiUtente Then
                stbQuery.AppendLine("       INNER JOIN Utenti_Permessi  (NOLOCK) ON Utenti.UserName = Utenti_Permessi.UserName      ")
            End If

            stbQuery.AppendLine("  WHERE    1 = 1 ")

            If Id_Servizio <> 0 Then
                stbQuery.AppendLine("  AND    (Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio.ToString) & ")        ")
            End If

            If Flag_LeggiPermessiUtente Then

                If Id_Servizio <> 0 Then
                    stbQuery.AppendLine("   AND (Utenti_Permessi.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio.ToString) & ")         ")
                End If

                If Id_Operazione <> -999 Then
                    stbQuery.AppendLine("   AND    (Utenti_Permessi.Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione.ToString) & ")   ")
                End If

                If Id_Attivita <> 0 Then
                    stbQuery.AppendLine("   AND    (Utenti_Permessi.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita.ToString) & ")   ")
                End If

                If Data_Accesso <> #1/1/1900# Then
                    stbQuery.AppendLine("   AND   Utenti_Permessi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Accesso) & "      ")
                    stbQuery.AppendLine("   AND   Utenti_Permessi.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Accesso) & "      ")
                End If

                If Ora_Accesso <> 0 Then
                    stbQuery.AppendLine("   AND Utenti_Permessi.Inizio_Ore <= " & Agro_SQL_SaveNum(Ora_Accesso.ToString) & "  ")
                    stbQuery.AppendLine("   And Utenti_Permessi.Fine_Ore >= " & Agro_SQL_SaveNum(Ora_Accesso.ToString) & " ")
                End If

            End If

            If UserName <> "" Then
                stbQuery.AppendLine("   AND    (Utenti.UserName = '" & Agro_SQL_SaveText(UserName) & "')   ")
            End If

            If Password <> "" Then
                stbQuery.AppendLine("   AND    (Utenti.Password = '" & Agro_SQL_SaveText(Password) & "')   ")
            End If

            If UserName_Profilo <> "" Then
                stbQuery.AppendLine("   AND    (Utenti_Profili.Utente_Profilo = '" & Agro_SQL_SaveText(UserName_Profilo) & "')   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stbQuery.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                stbQuery.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '---------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function



    Public Function Leggi_Superuser_e_ProgressivoGIAS(ByVal UserName As String,
                                                      ByVal Password As String,
                                                      ByVal Data_Accesso As Date,
                                                      ByVal Ora_Accesso As Integer,
                                                      ByVal Id_Servizio As Integer,
                                                      ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_Superuser_e_ProgressivoGIAS()"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0
            stbQuery.AppendLine(" SELECT  Utenti.UserName, Utenti.Password,  ")
            stbQuery.AppendLine(" 		Utenti.Flag_Encrypted, Utenti.Lingua_Cod,                  ")
            stbQuery.AppendLine(" 		Utenti_CodiciGiasPro.ProgressivoGIAS, Utenti_CodiciGiasPro.GiasOnline_Key, Utenti_CodiciGiasPro.CD_Key,           ")
            stbQuery.AppendLine(" 		Dettagli_SuperUser.PIVA AS Piva_SuperUser,           ")
            stbQuery.AppendLine(" 		Dettagli_SuperUser.CodFisc AS CodFisc_SuperUser,           ")
            stbQuery.AppendLine(" 		Dettagli_SuperUser.Rag_Soc AS RagSoc_SuperUser,            ")
            stbQuery.AppendLine(" 		SuperUser.UserName AS Username_SuperUser,  ")
            stbQuery.AppendLine(" 		SuperUser.Password AS Password_SuperUser, ")
            stbQuery.AppendLine(" 		SuperUser.Flag_Encrypted as Flag_Encrypted_Superuser ")
            stbQuery.AppendLine(" 		FROM    Utenti (NOLOCK)  ")
            stbQuery.AppendLine(" 		INNER JOIN          Utenti_Dettagli Dettagli_Utenti   (NOLOCK) ON Utenti.UserName = Dettagli_Utenti.UserName            ")
            stbQuery.AppendLine(" 		INNER JOIN Utenti_Profili  (NOLOCK) ON Utenti.UserName = Utenti_Profili.Utente           ")
            stbQuery.AppendLine(" 		INNER JOIN Utenti_CodiciGiasPro  (NOLOCK) ON Utenti_Profili.Utente_Profilo = Utenti_CodiciGiasPro.UserName           ")
            stbQuery.AppendLine(" 		INNER JOIN Utenti_Dettagli Dettagli_SuperUser  (NOLOCK) ON Utenti_Profili.Utente_Profilo = Dettagli_SuperUser.UserName           ")
            stbQuery.AppendLine(" 		INNER JOIN Utenti SuperUser  (NOLOCK) ON Dettagli_SuperUser.UserName = SuperUser.UserName     ")
            stbQuery.AppendLine(" 		INNER JOIN Utenti_Permessi  (NOLOCK) ON Utenti.UserName = Utenti_Permessi.UserName         ")
            stbQuery.AppendLine(" 		WHERE    1 = 1    ")

            If Id_Servizio <> 0 Then
                stbQuery.AppendLine("  AND    (Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio.ToString) & ")        ")
            End If

            stbQuery.AppendLine(" 	AND    (Utenti_Permessi.Id_Operazione = 0)  ")
            stbQuery.AppendLine(" 	AND    (Utenti_Permessi.Id_Attivita = 1)  ")

            If Data_Accesso <> #1/1/1900# Then
                stbQuery.AppendLine("   AND   Utenti_Permessi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Accesso) & "      ")
                stbQuery.AppendLine("   AND   Utenti_Permessi.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Accesso) & "      ")
            End If

            If Ora_Accesso <> 0 Then
                stbQuery.AppendLine("   AND Utenti_Permessi.Inizio_Ore <= " & Agro_SQL_SaveNum(Ora_Accesso.ToString) & "  ")
                stbQuery.AppendLine("   And Utenti_Permessi.Fine_Ore >= " & Agro_SQL_SaveNum(Ora_Accesso.ToString) & " ")
            End If

            If UserName <> "" Then
                stbQuery.AppendLine("   AND    (Utenti.UserName = '" & Agro_SQL_SaveText(UserName) & "')   ")
            End If

            If Password <> "" Then
                stbQuery.AppendLine("   AND    (Utenti.Password = '" & Agro_SQL_SaveText(Password) & "')   ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, stbQuery.ToString, nomeRoutine)
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
    'questa funzione non va eliminata
    'viene utilizzata da wS_Importa_GIAS per leggere i dati dell'utente e creare l'objparametri
    'non cancellare questa funzione perché viene utilizzata in casi in cui l'objparametri non è stato ancora creato
    Public Function Leggi_DatiUtente_e_DatiSuperUser(ByVal UserName As String,
                                                     ByVal UserName_Profilo As String,
                                                     ByVal Password As String,
                                                     ByVal Id_Operazione As Integer,
                                                     ByVal Id_Attivita As Integer,
                                                     ByVal Data_Accesso As Date,
                                                     ByVal Ora_Accesso As Integer,
                                                     ByVal Flag_LeggiPermessiUtente As Boolean,
                                                     ByVal Id_Servizio As Integer,
                                                     ByVal FiltroAggiuntivo As String,
                                                     ByRef objConnessione As DbConnection,
                                                     ByVal StringaConnessione As String,
                                                     ByVal FlagVisibilita As Int32,
                                                     ByVal DirectoryLOG As String,
                                                     ByVal FileLOG As String,
                                                     ByVal IdentificatoreUtente As String
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_DatiUtente_e_DatiSuperUser()"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New Text.StringBuilder
        Dim dt As DataTable

        'Try

        '    stbQuery.Length = 0

        '    stbQuery.AppendLine(" SELECT  Utenti.UserName, Utenti.Password,  ")

        '    stbQuery.AppendLine("         Dettagli_Utenti.Flag_Azienda_Persona,  ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.Cognome,  ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.Nome,  ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.CodFisc, ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.Rag_Soc,  ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.PIVA,  ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.UserNameCommerciale,  ")
        '    stbQuery.AppendLine("         Dettagli_Utenti.Flag_Azienda_Persona AS Utente_Flag_Azienda_Persona,  ")

        '    stbQuery.AppendLine("         Utenti_CodiciGiasPro.ProgressivoGIAS, Utenti_CodiciGiasPro.GiasOnline_Key, Utenti_CodiciGiasPro.CD_Key, ")

        '    stbQuery.AppendLine("         Utenti_Profili.Utente_Profilo, Utenti_Profili.Id_Servizio, ")
        '    stbQuery.AppendLine("         Utenti_Profili.Descrizione_1, Utenti_Profili.Descrizione_2,  ")

        '    stbQuery.AppendLine("         Dettagli_SuperUser.PIVA AS Piva_SuperUser, ")
        '    stbQuery.AppendLine("         Dettagli_SuperUser.CodFisc AS CodFisc_SuperUser, ")
        '    stbQuery.AppendLine("         Dettagli_SuperUser.Rag_Soc AS RagSoc_SuperUser,  ")

        '    stbQuery.AppendLine("         SuperUser.UserName AS Username_SuperUser, SuperUser.Password AS Password_SuperUser ")

        '    If Flag_LeggiPermessiUtente Then
        '        stbQuery.AppendLine("   , Utenti_Permessi.Id_Attivita, ")
        '        stbQuery.AppendLine("   Utenti_Permessi.Id_Operazione, ")
        '        stbQuery.AppendLine("   Utenti_Permessi.ID,  ")
        '        stbQuery.AppendLine("   Utenti_Permessi.Validita_Inizio, Utenti_Permessi.Validita_Fine   ")
        '    End If

        '    stbQuery.AppendLine(" FROM    Utenti  (NOLOCK) INNER JOIN ")
        '    stbQuery.AppendLine("         Utenti_Dettagli Dettagli_Utenti  (NOLOCK)  ON Utenti.UserName = Dettagli_Utenti.UserName  ")
        '    stbQuery.AppendLine("         INNER JOIN Utenti_Profili  (NOLOCK) ON Utenti.UserName = Utenti_Profili.Utente ")
        '    stbQuery.AppendLine("         INNER JOIN Utenti_CodiciGiasPro  (NOLOCK) ON Utenti_Profili.Utente_Profilo = Utenti_CodiciGiasPro.UserName ")
        '    stbQuery.AppendLine("         INNER JOIN Utenti_Dettagli  (NOLOCK) Dettagli_SuperUser ON Utenti_Profili.Utente_Profilo = Dettagli_SuperUser.UserName ")
        '    stbQuery.AppendLine("         INNER JOIN Utenti SuperUser  (NOLOCK) ON Dettagli_SuperUser.UserName = SuperUser.UserName ")

        '    If Flag_LeggiPermessiUtente Then
        '        stbQuery.AppendLine("       INNER JOIN Utenti_Permessi  (NOLOCK) ON Utenti.UserName = Utenti_Permessi.UserName      ")
        '    End If

        '    stbQuery.AppendLine("  WHERE    1 = 1 ")

        '    If Id_Servizio <> 0 Then
        '        stbQuery.AppendLine("  AND    (Utenti_Profili.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio.ToString) & ")        ")
        '    End If

        '    If Flag_LeggiPermessiUtente Then

        '        stbQuery.AppendLine("   AND (Utenti_Permessi.Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio.ToString) & ")         ")

        '        If Id_Operazione <> -999 Then
        '            stbQuery.AppendLine("   AND    (Utenti_Permessi.Id_Operazione = " & Agro_SQL_SaveNum(Id_Operazione.ToString) & ")   ")
        '        End If

        '        If Id_Attivita <> 0 Then
        '            stbQuery.AppendLine("   AND    (Utenti_Permessi.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita.ToString) & ")   ")
        '        End If

        '        If Data_Accesso <> #1/1/1900# Then
        '            stbQuery.AppendLine("   AND   Utenti_Permessi.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Accesso) & "      ")
        '            stbQuery.AppendLine("   AND   Utenti_Permessi.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Accesso) & "      ")
        '        End If

        '        If Ora_Accesso <> 0 Then
        '            stbQuery.AppendLine("   AND Utenti_Permessi.Inizio_Ore <= " & Agro_SQL_SaveNum(Ora_Accesso.ToString) & "  ")
        '            stbQuery.AppendLine("   And Utenti_Permessi.Fine_Ore >= " & Agro_SQL_SaveNum(Ora_Accesso.ToString) & " ")
        '        End If

        '    End If

        '    If UserName <> "" Then
        '        stbQuery.AppendLine("   AND    (Utenti.UserName = '" & Agro_SQL_SaveText(UserName) & "')   ")
        '    End If

        '    If Password <> "" Then
        '        stbQuery.AppendLine("   AND    (Utenti.Password = '" & Agro_SQL_SaveText(Password) & "')   ")
        '    End If

        '    If UserName_Profilo <> "" Then
        '        stbQuery.AppendLine("   AND    (Utenti_Profili.Utente_Profilo = '" & Agro_SQL_SaveText(UserName_Profilo) & "')   ")
        '    End If

        '    If FiltroAggiuntivo <> "" Then
        '        stbQuery.AppendLine(" " & Agro_SQL_Save_xFiltroAggiuntivo(FiltroAggiuntivo, , Nothing))
        '    End If

        '    'Select Case FlagVisibilita
        '    '    Case 1  'Solo i NON CANCELLATI
        '    '        stbQuery.AppendLine(" AND Utenti.Inviato >= 0 ")
        '    '    Case 2  'Solo i CANCELLATI
        '    '        stbQuery.AppendLine(" AND Utenti.Inviato = -1 ")
        '    '    Case 3  'TUTTI
        '    '        '
        '    '    Case Else
        '    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        '    'End Select

        '    '---------------------------------------------

        '    dt = EseguiQuery_Lettura(objConnessione, StringaConnessione, stbQuery.ToString, DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine)

        'Catch ex As Exception
        '    messaggioErrore = ex.Message
        '    Scrivi_LOG(DirectoryLOG, FileLOG, IdentificatoreUtente, nomeRoutine, messaggioErrore)
        '    dt = Nothing
        '    Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        'End Try

        Return dt

    End Function



    '##############################################################################################
    Public Function Leggi_SuperUser(ByVal Piva As String,
                                    ByVal StringaConnessione_Utenti As String
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_SuperUser()"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------

            stbQuery.Length = 0

            stbQuery.Append(" SELECT distinct Utenti.username,Utenti.password, Utenti_Dettagli.codFisc ")

            stbQuery.Append(" FROM    Utenti (NOLOCK)   ")
            stbQuery.Append("         INNER JOIN Utenti_Profili (NOLOCK)  ON Utenti.UserName = Utenti_Profili.Utente and Utenti.UserName = Utenti_Profili.Utente_Profilo ")
            stbQuery.Append("         INNER JOIN Utenti_Dettagli (NOLOCK)   ON Utenti_Profili.Utente_Profilo = Utenti_Dettagli.UserName ")

            stbQuery.Append(" where    1=1  ")
            If Piva <> "" Then
                stbQuery.Append("   AND    (Utenti_Dettagli.codFisc = '" & Agro_SQL_SaveText(Piva) & "')   ")
            End If
            
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(StringaConnessione_Utenti, stbQuery.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    '##############################################################################################
    Public Function Leggi_SuperUser(ByVal PivaSuperuser As String,
                                    ByRef SuperUser_Username As String,
                                    ByRef SuperUser_CodFiscale As String,
                                    ByRef SuperUser_Password As String,
                                    ByVal StringaConnessione_Utenti As String
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_SuperUser()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try

            dt = Leggi_SuperUser(PivaSuperuser, StringaConnessione_Utenti)

            If dt.Rows.Count = 1 Then
                SuperUser_Username = dt.Rows(0).Item("username")
                SuperUser_CodFiscale = dt.Rows(0).Item("codFisc")
                SuperUser_Password = dt.Rows(0).Item("password")
            Else
                Throw New Exception("DT.Rows.Count <> 1 ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return True

    End Function

    '##############################################################################################
    Public Sub Leggi_SuperUserPiva(ByRef Piva As String,
                                        ByVal StringaConnessione_Utenti As String)

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_SuperUserPiva()"

        Dim messaggioErrore As String = ""
        Dim stbQuery As New Text.StringBuilder
        Dim dt As DataTable

        Try

            stbQuery.Length = 0

            stbQuery.Append(" SELECT distinct ud.codFisc ")

            stbQuery.Append(" FROM    Utenti_Profili up ")
            stbQuery.Append("         INNER JOIN Utenti_Dettagli ud ON up.Utente_Profilo = ud.UserName ")

            stbQuery.Append(" WHERE   up.Utente = up.Utente_Profilo ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(StringaConnessione_Utenti, stbQuery.ToString, nomeRoutine)

            If dt.Rows.Count = 1 Then
                Piva = dt.Rows(0).Item("codFisc")
            Else
                Throw New Exception("DT.Rows.Count <> 1 ")
            End If

            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            messaggioErrore &= vbCrLf & "Piva:" & Piva & " " & vbCrLf
            messaggioErrore &= "StringaConnessione_Utenti:" & StringaConnessione_Utenti & " " & vbCrLf
            'Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    Function Password_From_UserName(ByVal UserName As String, ByVal ObjParametri_Utenti As AgronicaCoreParametri) As String

        Dim dt As DataTable = Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "", "", ObjParametri_Utenti, UserName)

        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Item("Password")
        End If
        Return ""
    End Function

    Function UserName_From_Password(ByVal Password As String, ByVal ObjParametri_Utenti As AgronicaCoreParametri) As String

        Dim dt As DataTable = Leggi(enumSelezioneVariabile.Selezione_TabellaCompleta,
                                    "  Utenti.Password = '" & Agro_SQL_SaveText(Password) & "' ",
                                    "", ObjParametri_Utenti)

        If dt.Rows.Count = 1 Then
            Return dt.Rows(0).Item("UserName")
        End If
        Return ""
    End Function

    Function UtenteDettagli_From_Email(ByVal email As String, ByVal ObjParametri_Utenti As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.UtenteDettagli_From_Email()"

        Dim messaggioErrore As String = ""
        Dim stb As New Text.StringBuilder
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

        Return dt

    End Function

    Function ContaUtenti(xFiltroAggiuntivo As String, ObjParametri_Utenti As AgronicaCoreParametri) As Integer
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.ContaUtenti()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable
        Try
            StrSQL.Append(" SELECT COUNT(0) ")
            StrSQL.Append(" FROM  Utenti ")
            StrSQL.Append(" WHERE 1=1 ")
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Utenti))
            End If
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Utenti, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                Return CType(dt.Rows(0)(0), Integer)
            Else
                Return -1
            End If
        Catch ex As Exception
            Scrivi_LOG(ObjParametri_Utenti, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
    End Function

    Function Leggi_Lingua(ByVal username As String,
                          ByVal xFiltroAggiuntivo As String, ByVal xOrderBy As String,
                          ByVal ObjParametri_Utenti As AgronicaCoreParametri
                          ) As Lingua

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_Lingua()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim ling As New Lingua

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT lingue.* ")
            StrSQL.Append(" FROM  Utenti inner join Lingue on utenti.lingua_cod=lingue.lingua_cod ")
            StrSQL.Append(" WHERE 1=1 ")

            If username <> "" Then
                StrSQL.Append(" AND Utenti.UserName = '" & Agro_SQL_SaveText(username) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Utenti))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Utenti))
            Else
                StrSQL.Append(" ORDER BY Utenti.UserName ASC")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Utenti, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                ling.CodiceISO = dt.Rows(0).Item("CodiceISO")
                ling.Lingua_cod = dt.Rows(0).Item("Lingua_cod")
                ling.Nome = dt.Rows(0).Item("Nome")
                ling.Descrizione = dt.Rows(0).Item("Descrizione")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return ling

    End Function

    Function LeggiLingue(ByVal CodiceISO As String,
                         ByVal xFiltroAggiuntivo As String,
                         ByVal xOrderBy As String,
                         ByVal ObjParametri_Utenti As AgronicaCoreParametri
                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.LeggiLingue()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM lingue ")
            StrSQL.Append(" WHERE 1=1 ")

            If CodiceISO <> "" Then
                StrSQL.Append(" AND lower(Lingue.CodiceISO) = lower('" & Agro_SQL_SaveText(CodiceISO) & "') ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , ObjParametri_Utenti))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, ObjParametri_Utenti))
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(ObjParametri_Utenti, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(ObjParametri_Utenti, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt


    End Function

    Public Function LeggiUtenteConGruppo(ByVal EscludiSuperUser As Boolean,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByVal xOrderBy As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.LeggiUtenteConGruppo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Utenti.* , Gruppi_Utente.Gruppi_Utente_cod , ISNULL(Gruppi_Utente.Gruppi_Utente_des,'') AS Gruppi_Utente_des,")
            StrSQL.AppendLine(" ISNULL(Utenti_Dettagli.Nome,'') AS Nome, ISNULL(Utenti_Dettagli.Cognome,'') AS Cognome")
            StrSQL.AppendLine(" FROM Utenti ")
            StrSQL.AppendLine(" LEFT JOIN Utenti_Dettagli ")
            StrSQL.AppendLine(" ON Utenti.UserName = Utenti_Dettagli.Username")
            StrSQL.AppendLine(" LEFT JOIN Utenti_xGruppi_Utente ")
            StrSQL.AppendLine(" ON Utenti.UserName = Utenti_xGruppi_Utente.UserName")
            StrSQL.AppendLine(" LEFT JOIN Gruppi_Utente ")
            StrSQL.AppendLine(" ON Utenti_xGruppi_Utente.Gruppi_Utente_cod = Gruppi_Utente.Gruppi_Utente_cod")
            StrSQL.AppendLine(" WHERE 1 = 1 ")


            If EscludiSuperUser Then
                StrSQL.AppendLine(" AND Utenti.UserName <> '" & objParametri.SuperUserUsername & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND  " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

    ''' <summary>
    ''' Legge gli utenti per la sincronizzazione con la tabella Utenti_Visibilita_Appoggio.
    ''' </summary>
    ''' <remarks>
    ''' Condizioni di lettura:
    ''' <list type="bullet">
    ''' <item>Id_Servizio = 5</item>
    ''' <item>DataUltimoRiportoUtentiVisibilitaAppoggio è null oppure è precedente alla data di modifica dell'utente</item>
    ''' <item>Filtro_Pratiche_Attivo = 1 (potrebbero esserci modifiche riguardanti le pratiche, necessario forzare la sincronizzazione)</item>
    ''' </list>
    ''' </remarks>
    Public Function Leggi_X_Sincronizzazione_Uuenti_Visibilita_Appoggio(ByRef objParametri As AgronicaCoreParametri) As DataTable
        Const nomeRoutine = "AgronicaCoreUtentiDAL.Utenti_Read.Leggi_X_Sincronizzazione_Uuenti_Visibilita_Appoggio()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable
        Try
            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("   u.UserName , up.Utente, ")
            StrSQL.AppendLine("   up.Descrizione_1, up.Descrizione_2, ")
            StrSQL.AppendLine("   up.Data_Modifica, up.DataUltimoRiportoUtentiVisibilitaAppoggio, ")
            StrSQL.AppendLine("   up.Filtro_Pratiche_Attivo, up.Operatore_Filtri ")
            StrSQL.AppendLine(" FROM utenti u (NOLOCK)")
            StrSQL.AppendLine(" INNER JOIN Utenti_Profili up ON u.UserName = up.Utente ")
            StrSQL.AppendLine(" WHERE up.Id_Servizio = 5 ")
            StrSQL.AppendLine("   AND (up.DataUltimoRiportoUtentiVisibilitaAppoggio IS NULL ")
            StrSQL.AppendLine("        OR ( ")
            StrSQL.AppendLine("            up.DataUltimoRiportoUtentiVisibilitaAppoggio IS NOT NULL ")
            StrSQL.AppendLine("            AND up.dataUltimoRiportoUtentiVisibilitaAppoggio < up.Data_Modifica ")
            StrSQL.AppendLine("        ) ")
            StrSQL.AppendLine("        OR filtro_pratiche_attivo = 1 ")
            StrSQL.AppendLine("   ) ")
            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return dt
    End Function

    ''' <summary>
    ''' Verifica che sia raggiungibile e attivo
    ''' </summary>
    ''' <param name="objP_Utenti"></param>
    ''' <returns></returns>
    Public Function IsAlive(ByRef objP_Utenti As AgronicaCoreParametri) As Boolean
        Dim nomeRoutine As String = "AgronicaCoreUtentiDAL.Utenti_Read.IsAlive()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable
        Dim r As Boolean = False

        Try
            strSql.Length = 0
            strSql.AppendLine(" SELECT TOP(1) * FROM Utenti ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objP_Utenti, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
                r = True
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objP_Utenti, nomeRoutine, messaggioErrore)
            r = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return r

    End Function




    '################################################################################
    Public Sub Nome_From_CF(ByVal UserName_CF As String,
                            ByRef UserName_Nome As String,
                            ByRef objParametri_Utenti As AgronicaCoreParametri)

        'DESCRIZIONE :        
        Dim ob As New AgronicaCoreUtentiDAL.Utenti_Dettagli_R
        Dim dt As DataTable = ob.Leggi("", 0, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta,
                " (Utenti_Dettagli.CodFisc = '" & Agro_SQL_SaveText(UserName_CF) & "') ", "", objParametri_Utenti)

        If dt.Rows.Count = 1 Then

            If Not IsDBNull(dt.Rows(0).Item("Cognome")) Then
                UserName_Nome = dt.Rows(0).Item("Cognome")
            End If

            If Not IsDBNull(dt.Rows(0).Item("Nome")) Then
                UserName_Nome &= IIf(UserName_Nome = String.Empty, "", " ") & dt.Rows(0).Item("Nome")
            End If

        Else
            UserName_Nome = ""
        End If

    End Sub

End Class


Public Class Utenti_Write
    Inherits AgronicaCoreDataProvider.DataProvider



    '#########################################################################

    ''' <param name="Lingua_Cod">Se 0, impostata a <tt>enum_AgroLingue.Italiano_it</tt> come default</param>
    Public Function Scrivi(ByVal UserName As String,
                           ByVal Password As String,
                           ByVal Lingua_Cod As Integer,
                           ByVal gestioneHashAbilitata As Boolean,
                           ByVal isPasswordHashed As Boolean,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional Tipologia_Cod As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.Scrivi()"

        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        '------------------------------
        If Not gestioneHashAbilitata AndAlso isPasswordHashed Then
            Throw New ArgumentException("Non è possibile salvare una password come hash se la gestione hash è disabilitata")
        End If

        Dim PasswordHashed As Integer = 0
        Dim PasswordToDB As String = Password

        If gestioneHashAbilitata Then
            PasswordHashed = 1

            If Not isPasswordHashed Then
                Dim objSicurezza As New Sicurezza
                PasswordToDB = objSicurezza.GeneraNuovoHash(Password)
            End If
        End If

        If Lingua_Cod = 0 Then
            Lingua_Cod = enum_AgroLingue.Italiano_it
        End If

        Try
            StrSQL.AppendLine("INSERT INTO Utenti ( ")
            StrSQL.AppendLine("    UserName, ")
            StrSQL.AppendLine("    Password, ")
            StrSQL.AppendLine("    Piva_SuperUser, ")
            StrSQL.AppendLine("    Flag_Encrypted , ")
            StrSQL.AppendLine("    Lingua_Cod, ")
            StrSQL.AppendLine("    Tipologia_Cod, ")
            StrSQL.AppendLine("    Flag, ")
            StrSQL.AppendLine("    Data_Creazione, ")
            StrSQL.AppendLine("    Data_Modifica ")
            StrSQL.AppendLine(") ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(UserName) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(PasswordToDB) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(PasswordHashed) & " ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Lingua_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Tipologia_Cod) & "  ")
            StrSQL.AppendLine("         , 0 ")
            StrSQL.AppendLine("         , GETDATE()  ")
            StrSQL.AppendLine("         , GETDATE()  ")
            StrSQL.AppendLine(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp

    End Function



    Public Function Scrivi_Superuser(ByVal username As String,
                                     ByVal password As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.Scrivi_Superuser()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dummy As Integer = -1

        Try

            '----- CREAZIONE del record in UTENTI

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Utenti ")
            StrSQL.Append("         (username, password) ")
            StrSQL.Append(" VALUES (")
            StrSQL.Append("         '" & Agro_SQL_SaveText(username) & "' , ")
            StrSQL.Append("         '" & Agro_SQL_SaveText(password) & "'  ")
            StrSQL.Append("         )")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp Then
                dummy = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dummy

    End Function


    Public Function Modifica_Superuser(ByVal username As String,
                                       ByVal password As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As Integer

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.Modifica_Superuser()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False
        Dim dummy As Integer = -1

        Try

            '----- MODIFICA del record in UTENTI

            StrSQL.Length = 0
            StrSQL.Append(" UPDATE Utenti SET ")
            StrSQL.Append(" password = '" & Agro_SQL_SaveText(password) & "' ")
            StrSQL.Append(" WHERE ")
            StrSQL.Append(" username = '" & Agro_SQL_SaveText(username) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If xRisp Then
                dummy = 0
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dummy

    End Function


    Public Function ModificaLockTentativi(ByVal UserName As String,
                                          ByVal NumeroTentativiAccesso As Integer,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.ModificaLockTentativi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine("declare @username nvarchar(1000) ")
            StrSQL.AppendLine("SET @username =  '" & Agro_SQL_SaveText(UserName) & "'")
            StrSQL.AppendLine("UPDATE Utenti SET ")
            StrSQL.AppendLine("       LockTentativi            = " & Agro_SQL_SaveNum(NumeroTentativiAccesso) & " ")
            StrSQL.AppendLine("       ,Data_Modifica           = " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine(" WHERE UserName = @Username ")

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

    'Public Function Modifica(ByVal UserName As String,
    '                         ByVal Password As String,
    '                         ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
    '                         ) As Boolean

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Utenti_Write.Modifica()"

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        '---------------------------------------------
    '        StrSQL.Length = 0

    '        StrSQL.Append("UPDATE Utenti SET ")
    '        StrSQL.Append("       Password                  = '" & Agro_SQL_SaveText(Password) & "' ")
    '        StrSQL.Append("       ,Data_Modifica           =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append(" WHERE UserName = '" & Replace(UserName, "'", "''") & "'")

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception

    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)

    '    End Try

    '    Return xRisp

    'End Function

    Public Function Modifica(ByVal UserName As String,
                             ByVal Password As String,
                             ByVal gestioneHashAbilitata As Boolean,
                             ByVal isPasswordHashed As Boolean,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Utenti_Write.Modifica_HashPassword()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        'TODO Aggiungere un controllo di obbligatorietà su UserName

        Try
            If Not gestioneHashAbilitata AndAlso isPasswordHashed Then
                Throw New ArgumentException("Non è possibile salvare una password come hash se la gestione hash è disabilitata")
            End If

            Dim PasswordHashed As Integer = 0
            Dim PasswordToDB As String = Password

            If gestioneHashAbilitata Then
                PasswordHashed = 1

                If Not isPasswordHashed Then
                    Dim objSicurezza As New Sicurezza
                    PasswordToDB = objSicurezza.GeneraNuovoHash(Password)
                End If
            End If


            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti SET ")
            StrSQL.Append("       Password               = '" & Agro_SQL_SaveText(PasswordToDB) & "' ")
            StrSQL.Append("       ,Flag_Encrypted        = " & PasswordHashed)
            StrSQL.Append("       ,LockTentativi         = 0")
            StrSQL.Append("       ,Flag                  = 0")
            StrSQL.Append("       ,Data_Modifica         =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,DataUltimaModificaPassword =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp
    End Function

    Public Function ModificaLingua(ByVal UserName As String,
                                   ByVal Lingua_cod As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.ModificaLingua()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti SET ")
            StrSQL.Append("        Lingua_cod    = " & Agro_SQL_SaveNum(Lingua_cod) & "  ")
            StrSQL.Append("       ,Data_Modifica =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "'")

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

    Public Function ModificaTipologia(ByVal UserName As String,
                                      ByVal Tipologia_cod As Integer,
                                      ByRef objParametri As AgronicaCoreParametri,
                                      Optional xFiltroAggiuntivo As String = ""
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.ModificaTipologia()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If String.IsNullOrEmpty(UserName) AndAlso String.IsNullOrEmpty(xFiltroAggiuntivo) Then
            Throw New ArgumentNullException("Nessun filtro sugli utenti indicato")
        End If

        Try
            StrSQL.AppendLine(" UPDATE Utenti SET ")
            StrSQL.AppendLine("     Tipologia_Cod    = " & Agro_SQL_SaveNum(Tipologia_cod) & "  ")
            StrSQL.AppendLine("   , Data_Modifica =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine(" WHERE 1=1")
            If Not String.IsNullOrEmpty(UserName) Then
                UserName = UserName.Trim
                If Not UserName.StartsWith("'") OrElse UserName.Contains(")") Then
                    UserName = Agro_SQL_SaveText(UserName)
                End If
                StrSQL.AppendLine(" AND UserName in ( " & Agro_SQL_Save_Clausola_IN(UserName, True) & " ) ")
            End If
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" AND " & Agro_SQL_SaveText(xFiltroAggiuntivo) & "'")
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp
    End Function

    Public Function ModificaTipologiaMassivo(
        ByVal users As IEnumerable(Of AgronicaCoreModelsSTD.profilazione.IUtente),
        ByVal Tipologia_cod As Integer,
        ByRef objParametri As AgronicaCoreParametri,
        Optional xFiltroAggiuntivo As String = ""
    ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.ModificaTipologia()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        If users.Count = 0 AndAlso String.IsNullOrEmpty(xFiltroAggiuntivo) Then
            Throw New ArgumentNullException("Nessun filtro sugli utenti indicato")
        End If
        Dim prepareFilter = Function(acc, u)
                                If u.index Mod 100 = 0 Then
                                    acc.name &= ", --" & u.index & vbNewLine & u.name
                                ElseIf u.index Mod 10 = 0 Then
                                    acc.name &= ", " & vbNewLine & u.name
                                Else
                                    acc.name &= ", " & u.name
                                End If
                                Return acc
                            End Function
        Dim usersFilter = users.AsParallel.
            Select(Function(u, i) New With {.name = "'" & u.UserName & "'", .index = i}).
            Aggregate(prepareFilter).name

        Try
            StrSQL.AppendLine(" UPDATE Utenti SET ")
            StrSQL.AppendLine("     Tipologia_Cod    = " & Agro_SQL_SaveNum(Tipologia_cod) & "  ")
            StrSQL.AppendLine("   , Data_Modifica =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine(" WHERE 1=1")
            If users.Any Then
                StrSQL.AppendLine(" AND UserName in ( " & usersFilter & " ) ")
            End If
            If Not String.IsNullOrEmpty(xFiltroAggiuntivo) Then
                StrSQL.AppendLine(" AND " & Agro_SQL_SaveText(xFiltroAggiuntivo) & "'")
            End If
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return xRisp
    End Function

    Public Function ModificaFlag(ByVal UserName As String,
                                 ByVal valoreFlag As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.ModificaFlag()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti SET ")
            StrSQL.Append("       flag                  = " & Agro_SQL_SaveNum(valoreFlag) & " ")
            StrSQL.Append("       ,Data_Modifica           =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "'")

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

    Public Function Modifica_FinestraTemporale(ByVal UserName As String,
                                               ByVal FinestraTemp_Inizio As Date,
                                               ByVal FinestraTemp_Fine As Date,
                                               ByRef objParametri_Server As AgronicaCoreParametri
                                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.Modifica_FinestraTemporale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti SET ")
            StrSQL.Append("       FinestraTemp_Inizio     = " & Agro_SQL_SaveDate(FinestraTemp_Inizio) & " ")
            StrSQL.Append("       ,FinestraTemp_Fine      = " & Agro_SQL_SaveDate(FinestraTemp_Fine) & " ")
            StrSQL.Append("       ,Data_Modifica          =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("       ,Username_Modifica      =  '" & Agro_SQL_SaveText(objParametri_Server.UsernameOperazione) & "' ")
            StrSQL.Append(" WHERE [User] = '" & Agro_SQL_SaveText(UserName) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function ModificaFlagSPID(ByVal UserName As String,
                                     ByVal FlagAccessoSPID As Boolean,
                                     ByRef objParametri_Utenti As AgronicaCoreParametri) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.ModificaFlagSPID()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim xRisp As Boolean

        Try
            Dim bitFlag As Integer = If(FlagAccessoSPID, 1, 0)
            StrSQL.AppendLine("UPDATE Utenti SET ")
            StrSQL.AppendLine("       Flag_Accesso_SPID     = " & Agro_SQL_SaveNum(bitFlag) & " ")
            StrSQL.AppendLine("       ,Data_Modifica          =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.AppendLine(" WHERE UserName = '" & Agro_SQL_SaveText(UserName) & "'")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Utenti, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri_Utenti, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return xRisp
    End Function

    '#########################################################################
    Public Function Utenti_GiasServer_Scrivi(ByRef NumeroRecordInteressati As Integer,
                                             ByVal UserName As String,
                                             ByVal FinestraTemp_Inizio As Date,
                                             ByVal FinestraTemp_Fine As Date,
                                             ByVal Codice_Fiscale As String,
                                             ByVal Nome_Resp As String,
                                             ByVal Ente_Resp As Integer,
                                             ByVal Inizio_Attivita As Date,
                                             ByVal Attivita As Integer,
                                             ByVal Calcolatore As Integer,
                                             ByVal Password As String,
                                             ByVal Tipo As Integer,
                                             ByVal Validita_Inizio As Date,
                                             ByVal Validita_Fine As Date,
                                             ByRef objParametri_Server As AgronicaCoreParametri
                                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.Utenti_GiasServer_Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Utenti ")
            StrSQL.Append(" ( [USER], FinestraTemp_Inizio, FinestraTemp_Fine, CODICE_FISCALE, NOME_RESP, ENTE_RESP, INIZIO_ATTIVITA, ATTIVITA, CALCOLATORE, PASSWORD, TIPO,  " & _
                           "  inviato, datainvio, Validazione, Data_Validazione, UserName_Validazione, Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Validita_Inizio, Validita_Fine ) ")

            StrSQL.Append(" VALUES ('" & Agro_SQL_SaveText(Trim(UserName)) & "', ")
            StrSQL.Append("" & Agro_SQL_SaveDate(FinestraTemp_Inizio) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveDate(FinestraTemp_Fine) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Codice_Fiscale)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Nome_Resp)) & "', ")
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Ente_Resp)) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveDate(Inizio_Attivita) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Attivita)) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Calcolatore)) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(Password)) & "', ")
            StrSQL.Append("" & Agro_SQL_SaveNum(Trim(Tipo)) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveNum(0) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveText("NULL") & ", ")
            StrSQL.Append("" & Agro_SQL_SaveNum(0) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveDate(AGRODATAINIZIO) & ", ")
            StrSQL.Append("'', ")
            StrSQL.Append("" & Agro_SQL_SaveDate(Now.Date) & ", ")
            StrSQL.Append("" & Agro_SQL_SaveDate(Now.Date) & ", ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(objParametri_Server.UsernameOperazione)) & "', ")
            StrSQL.Append("'" & Agro_SQL_SaveText(Trim(objParametri_Server.UsernameOperazione)) & "', ")
            StrSQL.Append(Agro_SQL_SaveDate(Validita_Inizio) & ", ")
            StrSQL.Append(Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica_Codice_Fiscale(ByVal Username As String,
                                            ByVal CODICE_FISCALE_NEW As String,
                                            ByVal CODICE_FISCALE_OLD As String,
                                            ByRef objParametri_Server As AgronicaCoreParametri
                                            ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Utenti_Write.Modifica_Codice_Fiscale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            StrSQL.Length = 0

            StrSQL.Append("UPDATE Utenti SET ")
            StrSQL.Append("       CODICE_FISCALE     = '" & Agro_SQL_SaveText(CODICE_FISCALE_NEW) & "', ")
            StrSQL.Append("       Data_Modifica     = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
            StrSQL.Append(" WHERE [User] = '" & Agro_SQL_SaveText(Username) & "'")
            StrSQL.Append(" AND  CODICE_FISCALE     = '" & Agro_SQL_SaveText(CODICE_FISCALE_OLD) & "' ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri_Server, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function




End Class
