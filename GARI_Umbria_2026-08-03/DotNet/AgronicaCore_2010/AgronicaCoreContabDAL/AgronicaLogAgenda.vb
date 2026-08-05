Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreEntityFramework
Imports Newtonsoft.Json

Public Class AgronicaLogAgenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ID As Integer,
                          ByVal SuperUser As String,
                          ByVal Utente As String,
                          ByVal Tipo_Operazione As enum_TipoOperazioneDB,
                          ByVal Id_Agenda As Integer,
                          ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Lav_Cod As Integer,
                          ByVal Id_Servizio As Integer,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   ID = 0
        '   SuperUser = ""
        '   Utente = ""
        '   Tipo_Operazione = 0
        '   Id_Agenda = 0
        '   Piva = ""
        '   Sa_Cod = 0
        '   Lav_Cod = 0
        '   Id_Servizio = 0
        '====================================================================================

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Agronica_Log_Agenda ")
            stb.AppendLine(" WHERE 1=1 ")

            If ID <> 0 Then
                stb.AppendLine(" AND ID = " & Agro_SQL_SaveNum(ID) & " ")
            End If

            If SuperUser <> "" Then
                stb.AppendLine(" AND SuperUser = '" & Agro_SQL_SaveText(SuperUser) & "' ")
            End If

            If Utente <> "" Then
                stb.AppendLine(" AND Utente = '" & Agro_SQL_SaveText(Utente) & "' ")
            End If

            If Tipo_Operazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND Tipo_Operazione = " & Agro_SQL_SaveNum(Tipo_Operazione) & " ")
            End If

            If Id_Agenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Id_Servizio <> 0 Then
                stb.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    Public Function LeggiDistinctAgendeConRaccoglitoreCod(
                      ByVal raccoglitoreCod As String,
                      ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.LeggiDistinctAgendeConRaccoglitoreCod()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT ")
            stb.AppendLine("    DISTINCT Id_Agenda, Piva, Lav_Cod ")
            stb.AppendLine(" FROM ")
            stb.AppendLine("    Agronica_Log_Agenda ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine("    Raccoglitore_Cod = " & Agro_SQL_SaveNum(raccoglitoreCod) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_UltimaOperazione(
                      ByVal SuperUser As String,
                      ByVal UltimaOperazione As enum_TipoOperazioneDB,
                      ByVal Id_Agenda As Integer,
                      ByVal Piva As String,
                      ByVal Sa_Cod As Integer,
                      ByVal Lav_Cod As Integer,
                      ByVal Id_Servizio As Integer,
                      ByVal xFiltroAggiuntivo As String,
                      ByVal xOrderBy As String,
                      ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                      ) As DataTable

        '====================================================================================
        'Parametri opzionali :
        '   ID = 0
        '   SuperUser = ""
        '   Utente = ""
        '   Tipo_Operazione = 0
        '   Id_Agenda = 0
        '   Piva = ""
        '   Sa_Cod = 0
        '   Lav_Cod = 0
        '   Id_Servizio = 0
        '====================================================================================

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.Leggi_UltimaOperazione()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM Agronica_Log_Agenda_UltimaOperazione ")
            stb.AppendLine(" WHERE 1=1 ")

            If SuperUser <> "" Then
                stb.AppendLine(" AND SuperUser = '" & Agro_SQL_SaveText(SuperUser) & "' ")
            End If

            If UltimaOperazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND UltimaOperazione = " & Agro_SQL_SaveNum(UltimaOperazione) & " ")
            End If

            If Id_Agenda <> 0 Then
                stb.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Piva <> "" Then
                stb.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Id_Servizio <> 0 Then
                stb.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

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

    ''' <summary>
    ''' Equivalente a Leggi_UltimaOperazione ma bypassa la view Agronica_Log_Agenda_UltimaOperazione
    ''' tramite tabelle temporanee indicizzate, eliminando il doppio full-scan su Agronica_Log_Agenda.
    ''' Pre-filtra per Piva, Id_Agenda e Lav_Cod (quando specificati) prima del calcolo del ROW_NUMBER.
    ''' I filtri su SuperUser, Sa_Cod, Id_Servizio e UltimaOperazione vengono applicati in post,
    ''' sulla temp table materializzata, poiché non è garantita la loro costanza per (Piva, Id_Agenda).
    ''' </summary>
    Public Function Leggi_UltimaOperazione_CTE(
                      ByVal SuperUser As String,
                      ByVal UltimaOperazione As enum_TipoOperazioneDB,
                      ByVal Id_Agenda As Integer,
                      ByVal Piva As String,
                      ByVal Sa_Cod As Integer,
                      ByVal Lav_Cod As Integer,
                      ByVal Id_Servizio As Integer,
                      ByVal xFiltroAggiuntivo As String,
                      ByVal xOrderBy As String,
                      ByRef objParametri As AgronicaCoreParametri,
                      Optional ByVal onlyOpCampagna As Boolean = False
                      ) As DataTable
        Const nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.Leggi_UltimaOperazione_CTE()"

        If onlyOpCampagna AndAlso Lav_Cod >= 1000 Then
            Throw New ArgumentException($"onlyOpCampagna=True richiede Lav_Cod < 1000, ma è stato passato Lav_Cod={Lav_Cod}.", NameOf(Lav_Cod))
        End If

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            ' -----------------------------------------------------------------------
            ' OPT: Bypass view Agronica_Log_Agenda_UltimaOperazione via temp tables.
            ' La view causa 2 scansioni complete di Agronica_Log_Agenda (doppio CTE
            ' self-join con ROW_NUMBER non filtrato) con sort spill su TempDB.
            '
            ' Step 1 – Unica scansione di Agronica_Log_Agenda con pre-filtri opzionali
            '          su Piva, Id_Agenda e Lav_Cod. Il ROW_NUMBER viene calcolato sul
            '          set già ridotto → nessun sort spill su TempDB.
            '          Pre-filtro sicuro: Piva e Id_Agenda sono chiavi della partizione;
            '          Lav_Cod è costante per ogni (Piva, Id_Agenda) nel log.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT la.SuperUser, la.Utente, la.Data_Ora_Lavorazione, la.Des_Lib, ")
            stb.AppendLine("        la.Id_Agenda, la.Piva, la.Sa_Cod, la.Lav_Cod, ")
            stb.AppendLine("        la.Data_Ora_RegistrazioneLog, la.Id_Servizio, la.Origine, ")
            stb.AppendLine("        la.Raccoglitore_Cod, la.Tipo_Operazione, ")
            stb.AppendLine("        ROW_NUMBER() OVER (PARTITION BY la.Piva, la.Id_Agenda ")
            stb.AppendLine("                           ORDER BY la.Data_Ora_RegistrazioneLog DESC, la.ID DESC) AS NumeroRiga ")
            stb.AppendLine(" INTO #CTE_Log ")
            stb.AppendLine(" FROM Agronica_Log_Agenda la (NOLOCK) ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If Piva <> "" Then
                stb.AppendLine("   AND la.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Id_Agenda <> 0 Then
                stb.AppendLine("   AND la.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Lav_Cod <> 0 Then
                stb.AppendLine("   AND la.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If onlyOpCampagna Then
                stb.AppendLine("   AND la.Lav_Cod < 1000 ")
            End If

            ' Step 2 – Indice su #CTE_Log per il self-join del passo successivo.
            stb.AppendLine(" CREATE UNIQUE INDEX IX_CTE_Log ON #CTE_Log (Piva, Id_Agenda, NumeroRiga) ")

            ' -----------------------------------------------------------------------
            ' Step 3 – Self-join su temp table indicizzata per calcolare UltimaOperazione.
            '          Logica CASE identica alla view Agronica_Log_Agenda_UltimaOperazione.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT ")
            stb.AppendLine("     u.SuperUser, u.Utente, u.Data_Ora_Lavorazione, u.Des_Lib, ")
            stb.AppendLine("     ISNULL(u.Id_Agenda, 0) AS Id_Agenda, ")
            stb.AppendLine("     ISNULL(u.Piva, '')     AS Piva, ")
            stb.AppendLine("     u.Sa_Cod, u.Lav_Cod, ")
            stb.AppendLine("     u.Data_Ora_RegistrazioneLog, u.Id_Servizio, u.Origine, ")
            stb.AppendLine("     ISNULL(u.Raccoglitore_Cod, 0) AS Raccoglitore_Cod, ")
            stb.AppendLine("     CASE ")
            stb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione IS NULL THEN 1 ") 'INSERT
            stb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione = 3    THEN 2 ") 're-INSERT dopo DELETE
            stb.AppendLine("         WHEN u.Tipo_Operazione = 2                              THEN 2 ") 'MODIFICA DIRETTA
            stb.AppendLine("         WHEN u.Tipo_Operazione = 3                              THEN 3 ") 'DELETE
            stb.AppendLine("         ELSE u.Tipo_Operazione ")
            stb.AppendLine("     END AS UltimaOperazione ")
            stb.AppendLine(" INTO #ultimoLogAgenda ")
            stb.AppendLine(" FROM #CTE_Log u ")
            stb.AppendLine(" LEFT JOIN #CTE_Log p ON p.Piva = u.Piva AND p.Id_Agenda = u.Id_Agenda AND p.NumeroRiga = 2 ")
            stb.AppendLine(" WHERE u.NumeroRiga = 1 ")

            stb.AppendLine(" DROP TABLE #CTE_Log ")

            ' Step 4 – Indice su #ultimoLogAgenda per la query finale con post-filtri.
            stb.AppendLine(" CREATE UNIQUE INDEX IX_UltimoLogAgenda ON #ultimoLogAgenda (Piva, Id_Agenda) ")
            stb.AppendLine(" INCLUDE (SuperUser, UltimaOperazione, Sa_Cod, Lav_Cod, Id_Servizio, ")
            stb.AppendLine("          Data_Ora_RegistrazioneLog, Raccoglitore_Cod, Origine) ")

            ' -----------------------------------------------------------------------
            ' Step 5 – Query finale su temp table materializzata con post-filtri.
            '          SuperUser, Sa_Cod, Id_Servizio e UltimaOperazione si applicano
            '          all'ultima riga del log; non pre-filtrabili senza alterare la semantica.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT * ")
            stb.AppendLine(" FROM #ultimoLogAgenda ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If SuperUser <> "" Then
                stb.AppendLine(" AND SuperUser = '" & Agro_SQL_SaveText(SuperUser) & "' ")
            End If

            If UltimaOperazione <> enum_TipoOperazioneDB.Lettura Then
                stb.AppendLine(" AND UltimaOperazione = " & Agro_SQL_SaveNum(UltimaOperazione) & " ")
            End If

            If Sa_Cod <> 0 Then
                stb.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Servizio <> 0 Then
                stb.AppendLine(" AND Id_Servizio = " & Agro_SQL_SaveNum(Id_Servizio) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            stb.AppendLine(" DROP TABLE #ultimoLogAgenda ")

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

    Public Function LeggiAttivitaCancellatexApp(ByVal piva As String,
                                            ByVal dataLavorazioneMin As Date,
                                            ByVal dataUltimaSincro As Date,
                                            ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.LeggiAttivitaCancellate()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable

        Try

            sb.Length = 0

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" select ")
            sb.AppendLine("     Id_Agenda ")
            sb.AppendLine(" from ")
            sb.AppendLine("     Agronica_Log_Agenda_UltimaOperazione")
            sb.AppendLine(" where")
            sb.AppendLine(" 	  Piva = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine(" 	  AND Data_Ora_Lavorazione >= " & Agro_SQL_SaveDate(dataLavorazioneMin) & " ")
            sb.AppendLine(" 	  AND Data_Ora_RegistrazioneLog >= " & Agro_SQL_SaveDateTime(dataUltimaSincro) & " ")
            sb.AppendLine(" 	  AND Lav_Cod IN (" & OPERAZIONI_GESTITE_APP_DEMETRA & ")")
            sb.AppendLine(" 	  AND UltimaOperazione = 3 ") 'cancellazioni

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function EsistonoModificheDopoLaDataXAttivitaApp(ByVal piva As String,
                                                ByVal dataLavorazioneMin As Date,
                                                ByVal dataModificheMin As Date,
                                                ByRef objParametriServer As AgronicaCoreParametri) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.EsistonoModificheDopoLaDataXAttivitaApp()"

        Dim messaggioErrore As String = ""
        Dim sb As New StringBuilder
        Dim dt As DataTable
        Dim result = True

        Try
            sb.Length = 0

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" SELECT TOP 1")
            sb.AppendLine("     1 ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Agronica_Log_Agenda_UltimaOperazione")
            sb.AppendLine(" WHERE ")
            sb.AppendLine(" 	  Piva = '" & Agro_SQL_SaveText(piva) & "'")
            sb.AppendLine(" 	  AND Data_Ora_Lavorazione >= " & Agro_SQL_SaveDate(dataLavorazioneMin) & " ")
            sb.AppendLine(" 	  AND Lav_Cod IN (" & OPERAZIONI_GESTITE_APP_DEMETRA & ")")
            sb.AppendLine("     AND Data_Ora_RegistrazioneLog >= " & Agro_SQL_SaveDateTime(dataModificheMin) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            result = dt IsNot Nothing AndAlso dt.Rows.Count > 0

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            result = True
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return result

    End Function

    Public Function LeggiDatiPerExportAttivitaConRaccoglitoreCod(ByVal raccoglitore_cod As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.LeggiDatiPerExportAttivitaConRaccoglitoreCod()"
        Dim messaggioErrore As String = ""
        Dim sb = New Text.StringBuilder()
        Dim dt As DataTable
        Try

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" select ")
            sb.AppendLine(" 	a.Id_Agenda, ad.ID, ad.Versione, ad.Tipo")
            sb.AppendLine(" from ")
            sb.AppendLine(" 	Agronica_Log_Agenda_UltimaOperazione a")
            sb.AppendLine(" join")
            sb.AppendLine(" 	app_dati ad")
            sb.AppendLine(" 	on a.Id_Agenda = SUBSTRING(ad.riferimento, 0, CHARINDEX('|',ad.riferimento,0))")
            sb.AppendLine(" where")
            sb.AppendLine(" 	a.Raccoglitore_Cod = " & Agro_SQL_SaveNum(raccoglitore_cod))
            sb.AppendLine("   AND ad.Tipo IN ('10', '90')") 'attivit� provenienti da app e da demetra
            sb.AppendLine(" 	and CHARINDEX('|', ad.Riferimento) > 0")
            sb.AppendLine(" 	AND SUBSTRING(ad.riferimento, 0, CHARINDEX('|',ad.riferimento,0)) > 0") 'ribaltate in agenda

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametriServer, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt


    End Function

    Public Function AgendeDaInviare_Demetra(ByVal tipoEsportazione As enum_Esportazioni_Sistema_Cod,
                                            ByVal tipiAttivitaDemetraApp As List(Of String),
                                            ByVal PivaAmmesse As List(Of String),
                                            ByVal OperazioniAmmesse As List(Of Integer),
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                            Optional ByVal filtroEsiti As List(Of String) = Nothing,
                                            Optional ByVal topNRows As Integer = 0
                                            ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.AgendeDaInviare()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            Dim ok = "OK"
            Dim blk = "BLK"
            Dim ko = "KO"

            Dim okBlk = New List(Of String) From {
                ok,
                blk
            }

            stb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            stb.AppendLine(" SELECT ")
            stb.AppendLine("    Piva, Id_Agenda, Data_Ora_RegistrazioneLog, Lav_Cod, Raccoglitore_Cod, Tipo_Operazione, ")
            stb.AppendLine("    ROW_NUMBER() OVER (PARTITION BY Piva, Id_Agenda ORDER BY Data_Ora_RegistrazioneLog DESC, ID DESC) AS NumeroRiga ")
            stb.AppendLine(" INTO ")
            stb.AppendLine("    #cteLogAgenda ")
            stb.AppendLine(" FROM ")
            stb.AppendLine("     Agronica_Log_Agenda ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine("    1 = 1 ")

            If OperazioniAmmesse IsNot Nothing AndAlso OperazioniAmmesse.Count > 0 Then
                stb.AppendLine("     AND Lav_Cod IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", OperazioniAmmesse), False) & ") ")
            End If

            If PivaAmmesse IsNot Nothing AndAlso PivaAmmesse.Count > 0 Then
                stb.AppendLine("     AND Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", PivaAmmesse), True) & ") ")
            End If

            stb.AppendLine(" ")

            stb.AppendLine(" SELECT ")
            stb.AppendLine("     ISNULL(u.Id_Agenda, 0) AS Id_Agenda, ")
            stb.AppendLine("     u.Lav_Cod, ISNULL(u.Piva, '') AS Piva, ")
            stb.AppendLine("     ISNULL(u.Raccoglitore_Cod, 0) AS Raccoglitore_Cod, u.Data_Ora_RegistrazioneLog, ")
            stb.AppendLine("     CASE ")
            stb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione IS NULL THEN 1 ")
            stb.AppendLine("         WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione = 3    THEN 2 ")
            stb.AppendLine("         WHEN u.Tipo_Operazione = 2                              THEN 2 ")
            stb.AppendLine("         WHEN u.Tipo_Operazione = 3                              THEN 3 ")
            stb.AppendLine("         ELSE u.Tipo_Operazione ")
            stb.AppendLine("     END AS UltimaOperazione ")
            stb.AppendLine(" INTO ")
            stb.AppendLine("     #ultimoLogAgenda ")
            stb.AppendLine(" FROM ")
            stb.AppendLine("     #cteLogAgenda u ")
            stb.AppendLine(" LEFT JOIN  ")
            stb.AppendLine("     #cteLogAgenda p")
            stb.AppendLine("     ON p.Piva = u.Piva ")
            stb.AppendLine("     AND p.Id_Agenda = u.Id_Agenda ")
            stb.AppendLine("     AND p.NumeroRiga = 2 ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine("     u.NumeroRiga = 1 ")

            stb.AppendLine(" ")

            stb.AppendLine(" SELECT ")
            stb.AppendLine("    Piva, Id_Agenda, Data_Ora_RegistrazioneLog, UltimaOperazione, Lav_Cod, Raccoglitore_Cod ")
            stb.AppendLine(" INTO ")
            stb.AppendLine("    #log_agenda ")
            stb.AppendLine(" FROM ")
            stb.AppendLine("    #ultimoLogAgenda ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine("    NOT EXISTS ")
            stb.AppendLine("    (   SELECT ")
            stb.AppendLine("            1 ")
            stb.AppendLine("        FROM ")
            stb.AppendLine("            Movimenti_Dettagli (NOLOCK)")
            stb.AppendLine("        WHERE ")
            stb.AppendLine("            #ultimoLogAgenda.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
            stb.AppendLine("            AND Contabilizzato = " & Agro_SQL_SaveNum(-1) & ")")

            stb.AppendLine(" ")

            stb.AppendLine("  SELECT ")
            stb.AppendLine(" 		ad.Piva, ad.Codice, TRY_CONVERT(INT, SUBSTRING(ad.riferimento,0,CHARINDEX('|',ad.riferimento,0))) ID_Agenda, ad.ID as GuidRicetta, ad.Versione, ")
            stb.AppendLine("      ISNULL(ad.Riferimento_Pianificata, '') as CodiceGiasPianificata, ISNULL(adP.Codice, '') as CodiceDemetraPianificata ")
            stb.AppendLine("  INTO ")
            stb.AppendLine(" 		#agendeDemetraEApp ")
            stb.AppendLine("  FROM ")
            stb.AppendLine(" 		APP_Dati (NOLOCK) ad ")
            stb.AppendLine("  LEFT JOIN ")
            stb.AppendLine("       APP_Dati (NOLOCK) adP ")
            stb.AppendLine("       ON adP.ID = ad.Riferimento_Pianificata ")
            stb.AppendLine("  WHERE ")
            stb.AppendLine("       ad.tipo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", tipiAttivitaDemetraApp), True) & ") ")
            stb.AppendLine("       AND SUBSTRING(ad.riferimento, 0, CHARINDEX('|', ad.riferimento,0)) > 0 ") 'operazioni provenienti da app e da demetra, ribaltate in agenda

            stb.AppendLine(" ")

            stb.AppendLine(" SELECT ")
            stb.AppendLine("    ID_Agenda, Max(Id_Log_Invio) id_log_invio ")
            stb.AppendLine(" INTO ")
            stb.AppendLine("    #log_invio_agenda ")
            stb.AppendLine(" FROM ")
            stb.AppendLine("    Agronica_Log_Invio_Agenda (NOLOCK) ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine("    Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione))
            stb.AppendLine(" GROUP BY  ")
            stb.AppendLine("    ID_Agenda ")

            stb.AppendLine(" ")

            stb.AppendLine(" Select ")
            If topNRows > 0 Then
                stb.AppendLine($"       TOP {topNRows} ")
            End If

            stb.AppendLine("        #log_agenda.Piva, #log_agenda.Id_Agenda, #log_agenda.Lav_Cod, UltimaOperazione, ISNULL(lic.Esito, '') AS Esito, ISNULL(lic.ID, 0) AS ID_Chiamata, ")
            stb.AppendLine("        ISNULL(#agendeDemetraEApp.Codice,'') As CodiceDemetra, #log_agenda.Raccoglitore_Cod, ISNULL(#agendeDemetraEApp.GuidRicetta, '') as GuidRicetta, ")
            stb.AppendLine("        ISNULL(#agendeDemetraEApp.Versione, '') as Versione, ")
            stb.AppendLine("        ISNULL(#agendeDemetraEApp.CodiceGiasPianificata, '') as CodiceGiasPianificata, ISNULL(#agendeDemetraEApp.CodiceDemetraPianificata, '') as CodiceDemetraPianificata ")

            stb.AppendLine(" FROM ")
            stb.AppendLine("    #log_agenda  ")
            stb.AppendLine(" LEFT JOIN ")
            stb.AppendLine("    #log_invio_agenda ")
            stb.AppendLine("    ON (#log_agenda.Id_Agenda = #log_invio_agenda.ID_Agenda) ")
            stb.AppendLine(" LEFT JOIN ")
            stb.AppendLine("    Agronica_Log_Invio_Chiamate (NOLOCK) lic ")
            stb.AppendLine("    ON #log_invio_agenda.id_log_invio = lic.ID ")
            stb.AppendLine("    AND Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione))
            stb.AppendLine(" LEFT JOIN ")
            stb.AppendLine("    #agendeDemetraEApp ")
            stb.AppendLine("    ON #log_agenda.Id_Agenda = #agendeDemetraEApp.ID_Agenda ")
            stb.AppendLine("    AND #log_agenda.Piva = #agendeDemetraEApp.Piva ")
            stb.AppendLine(" WHERE ")
            stb.AppendLine("    1 = 1  ")
            stb.AppendLine("    AND (")
            stb.AppendLine("         lic.Data_Invio IS NULL ")
            stb.AppendLine("         OR (#log_agenda.Data_Ora_RegistrazioneLog >= lic.Data_Invio AND lic.Esito IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", okBlk), True) & ")) ")
            stb.AppendLine("         OR (lic.Esito = '" & Agro_SQL_SaveText(ko) & "') ")
            stb.AppendLine("        )")

            If filtroEsiti IsNot Nothing AndAlso filtroEsiti.Any() Then
                ' Gestisci le stringhe vuote nella clausola IN
                Dim esitiFormatted = String.Join(",", filtroEsiti.Select(Function(e) If(String.IsNullOrEmpty(e), "''", e)))
                stb.AppendLine(" AND ISNULL(lic.Esito, '') IN (" & Agro_SQL_Save_Clausola_IN(esitiFormatted, True) & ") ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            stb.AppendLine(" ")

            stb.AppendLine(" DROP TABLE #cteLogAgenda ")
            stb.AppendLine(" DROP TABLE #ultimoLogAgenda ")
            stb.AppendLine(" DROP TABLE #log_agenda ")
            stb.AppendLine(" DROP TABLE #agendeDemetraEApp ")
            stb.AppendLine(" DROP TABLE #log_invio_agenda ")

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

    Public Function VisiteDaInviare_CAI(ByVal lavCod As Integer,
                                        ByVal tipoEsportazione As enum_Esportazioni_Sistema_Cod,
                                        ByVal PivaAmmesse As List(Of String),
                                        ByVal PivaEscluse As List(Of String),
                                        ByVal Retry_Status As List(Of String),
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.AgronicaLogAgenda_R.VisiteDaInviare_CAI()"

        Dim messaggioErrore As String = ""
        Dim stb As New StringBuilder
        Dim dt As DataTable

        Try

            stb.Length = 0

            ' -----------------------------------------------------------------------
            ' OPT-B v2: Lav_Cod è costante per ogni (Piva, Id_Agenda) nel log —
            ' un'agenda di tipo visita ha sempre Lav_Cod = 5007 su tutte le sue
            ' righe di log. È quindi sicuro pre-filtrare per Lav_Cod direttamente
            ' nella scansione che calcola il ROW_NUMBER, riducendo da 2 scansioni
            ' complete (40M righe) a 1 scansione filtrata dell'intera tabella.
            '
            ' Step 1 – Unica scansione di Agronica_Log_Agenda filtrata per Lav_Cod
            '          (e opzionalmente per Piva). Calcola il ROW_NUMBER in-stream
            '          su un set già ridotto → nessun sort spill su TempDB.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT la.Piva, la.Id_Agenda, la.Lav_Cod, ")
            stb.AppendLine("        la.Data_Ora_RegistrazioneLog, la.Raccoglitore_Cod, la.Tipo_Operazione, ")
            stb.AppendLine("        ROW_NUMBER() OVER (PARTITION BY la.Piva, la.Id_Agenda ")
            stb.AppendLine("                           ORDER BY la.Data_Ora_RegistrazioneLog DESC, la.ID DESC) AS NumeroRiga ")
            stb.AppendLine(" INTO #CTE_Log ")
            stb.AppendLine(" FROM Agronica_Log_Agenda la (NOLOCK) ")
            stb.AppendLine(" WHERE 1 = 1 ")

            If lavCod <> 0 Then
                stb.AppendLine("   AND la.Lav_Cod = " & Agro_SQL_SaveNum(lavCod) & " ")
            End If

            If PivaAmmesse IsNot Nothing AndAlso PivaAmmesse.Count > 0 Then
                stb.AppendLine("   AND la.Piva IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", PivaAmmesse), True) & ") ")
            End If

            If PivaEscluse IsNot Nothing AndAlso PivaEscluse.Count > 0 Then
                stb.AppendLine("   AND la.Piva NOT IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", PivaEscluse), True) & ") ")
            End If

            ' Step 2 – Indice su #CTE_Log per il self-join al passo successivo.
            stb.AppendLine(" CREATE UNIQUE INDEX IX_CTE_Log ON #CTE_Log (Piva, Id_Agenda, NumeroRiga) ")

            ' -----------------------------------------------------------------------
            ' Step 3 – Self-join su temp table indicizzata (poche centinaia di righe)
            '          per calcolare UltimaOperazione confrontando riga 1 e riga 2.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT u.Piva, u.Id_Agenda, u.Lav_Cod, ")
            stb.AppendLine("        u.Data_Ora_RegistrazioneLog, ")
            stb.AppendLine("        ISNULL(u.Raccoglitore_Cod, 0) AS Raccoglitore_Cod, ")
            stb.AppendLine("        CASE ")
            stb.AppendLine("            WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione IS NULL THEN 1 ")  ' INSERT
            stb.AppendLine("            WHEN u.Tipo_Operazione = 1 AND p.Tipo_Operazione = 3    THEN 2 ")  ' re-INSERT dopo DELETE
            stb.AppendLine("            WHEN u.Tipo_Operazione = 2                              THEN 2 ")  ' MODIFICA DIRETTA
            stb.AppendLine("            WHEN u.Tipo_Operazione = 3                              THEN 3 ")  ' DELETE
            stb.AppendLine("            ELSE u.Tipo_Operazione ")
            stb.AppendLine("        END AS UltimaOperazione ")
            stb.AppendLine(" INTO #ultimoLogAgenda ")
            stb.AppendLine(" FROM #CTE_Log u ")
            stb.AppendLine(" LEFT JOIN #CTE_Log p ON p.Piva = u.Piva AND p.Id_Agenda = u.Id_Agenda AND p.NumeroRiga = 2 ")
            stb.AppendLine(" WHERE u.NumeroRiga = 1 ")

            stb.AppendLine(" DROP TABLE #CTE_Log ")

            ' Step 4 – Indice su #ultimoLogAgenda per i join con i log invio.
            stb.AppendLine(" CREATE UNIQUE INDEX IX_UltimoLogAgenda ON #ultimoLogAgenda (Piva, Id_Agenda) ")
            stb.AppendLine(" INCLUDE (Lav_Cod, Data_Ora_RegistrazioneLog, UltimaOperazione, Raccoglitore_Cod) ")

            ' -----------------------------------------------------------------------
            ' Step 5 – Ultimo invio per ciascuna agenda (solo per questo tipo esportazione).
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT ID_Agenda, MAX(Id_Log_Invio) AS id_log_invio ")
            stb.AppendLine(" INTO #log_invio_agenda ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Agenda (NOLOCK) ")
            stb.AppendLine(" WHERE Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione) & " ")
            stb.AppendLine(" GROUP BY ID_Agenda ")

            stb.AppendLine(" CREATE UNIQUE INDEX IX_LogInvioAgenda ON #log_invio_agenda (ID_Agenda) INCLUDE (id_log_invio) ")

            ' -----------------------------------------------------------------------
            ' Step 6 – Dettaglio chiamata: solo le righe riferite agli id_log_invio
            '          raccolti al passo precedente (OPT-A applicato sulle temp).
            '          DISTINCT necessario perché più agendas possono condividere
            '          lo stesso id_log_invio, producendo duplicati nel join.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT DISTINCT lc.ID, lc.Esito, lc.Data_Invio ")
            stb.AppendLine(" INTO #log_invio_chiamate ")
            stb.AppendLine(" FROM Agronica_Log_Invio_Chiamate lc (NOLOCK) ")
            stb.AppendLine(" INNER JOIN #log_invio_agenda lia ON lia.id_log_invio = lc.ID ")
            stb.AppendLine(" WHERE lc.Tipo_Esportazione = " & Agro_SQL_SaveNum(tipoEsportazione) & " ")

            stb.AppendLine(" CREATE UNIQUE INDEX IX_LogInvioChiamate ON #log_invio_chiamate (ID) INCLUDE (Esito, Data_Invio) ")

            ' -----------------------------------------------------------------------
            ' Step 7 – Query finale su sole temp table materializzate e indicizzate.
            ' -----------------------------------------------------------------------
            stb.AppendLine(" SELECT ")
            stb.AppendLine("     ula.Piva, ")
            stb.AppendLine("     ula.Id_Agenda, ")
            stb.AppendLine("     ula.Lav_Cod, ")
            stb.AppendLine("     ula.UltimaOperazione, ")
            stb.AppendLine("     ISNULL(lic.Esito, '')  AS Esito, ")
            stb.AppendLine("     ISNULL(lic.ID,   0)    AS ID_Chiamata, ")
            stb.AppendLine("     ula.Raccoglitore_Cod ")
            stb.AppendLine(" FROM #ultimoLogAgenda ula ")
            stb.AppendLine(" LEFT OUTER JOIN #log_invio_agenda  lia ON lia.ID_Agenda      = ula.Id_Agenda ")
            stb.AppendLine(" LEFT OUTER JOIN #log_invio_chiamate lic ON lic.ID             = lia.id_log_invio ")
            stb.AppendLine(" WHERE 1 = 1 ")
            stb.AppendLine("     AND (lic.Data_Invio IS NULL ")
            stb.AppendLine("     OR  (ula.Data_Ora_RegistrazioneLog >= lic.Data_Invio ")
            stb.AppendLine("          AND lic.Esito IN ('OK','BLK')) ")

            ' Filtro sugli stati da eseguire nel retry dell'invio (se impostati)
            If Retry_Status.Count > 0 Then
                stb.AppendLine("     OR  lic.Esito IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", Retry_Status), True) & ")) ")
            Else
                stb.AppendLine("     ) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                stb.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                stb.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                stb.AppendLine(" ORDER BY ula.Data_Ora_RegistrazioneLog DESC ")
            End If

            stb.AppendLine(" DROP TABLE #ultimoLogAgenda ")
            stb.AppendLine(" DROP TABLE #log_invio_agenda ")
            stb.AppendLine(" DROP TABLE #log_invio_chiamate ")

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

End Class


'#################################################################
'#################################################################
'#################################################################


Public Class AgronicaLogAgenda_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Data_Ora_Lavorazione As Date,
                           ByVal Tipo_Operazione As Integer,
                           ByVal Des_Lib As String,
                           ByVal Id_Agenda As Long,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Long,
                           ByVal Lav_Cod As Integer,
                           ByVal Id_Servizio As Integer,
                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                           Optional ByVal agenda As Object = Nothing,
                           Optional ByVal Origine As Integer = -1,
                           Optional ByVal Raccoglitore_Cod As Integer = 0
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AgronicaLogAgenda_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Dim jsonAgenda = ""
        Try
            If agenda IsNot Nothing Then
                agenda.Id_Agenda = Id_Agenda
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                jsonAgenda = JsonConvert.SerializeObject(agenda, a)
            End If
        Catch ex As Exception
            'DT: errori nella produzione del json da loggare non devono compromettere la scrittura del record di log
        End Try

        Try

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" INSERT INTO Agronica_Log_Agenda ")
            strSql.AppendLine("             ( SuperUser, Utente, Data_Ora_Lavorazione, Tipo_Operazione, Des_Lib, ")
            strSql.AppendLine("               Id_Agenda, Piva, Sa_Cod, Lav_Cod, Data_Ora_RegistrazioneLog, Id_Servizio, ")
            strSql.AppendLine("               object_data, Origine, Raccoglitore_Cod ) ")

            strSql.AppendLine(" VALUES ( ")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDateTime(Data_Ora_Lavorazione) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Operazione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Des_Lib) & "'  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            strSql.AppendLine("         , GETDATE() ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Servizio) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(jsonAgenda) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Origine) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Raccoglitore_Cod) & "  ")
            strSql.AppendLine("         )")

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

    Public Function Scrivi_EF(ByVal Data_Ora_Lavorazione As Date,
                              ByVal Tipo_Operazione As Integer,
                              ByVal Des_Lib As String,
                              ByVal Id_Agenda As Long,
                              ByVal Piva As String,
                              ByVal Sa_Cod As Long,
                              ByVal Lav_Cod As Integer,
                              ByVal Id_Servizio As Integer,
                              ByRef GiasContext As Gias_DeveloperServer_Entities,
                              ByRef objParametriServer As AgronicaCoreDataProvider.AgronicaCoreParametri,
                              Optional ByVal agenda As Object = Nothing,
                              Optional ByVal Origine As Integer = -1,
                              Optional ByVal Raccoglitore_Cod As Integer = 0
                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AgronicaLogAgenda_W.Scrivi_EF()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Dim jsonAgenda = ""
        Try
            If agenda IsNot Nothing Then
                agenda.Id_Agenda = Id_Agenda
                Dim a As New JsonSerializerSettings With {.DateTimeZoneHandling = DateTimeZoneHandling.Local}
                jsonAgenda = JsonConvert.SerializeObject(agenda, a)
            End If
        Catch ex As Exception
            'DT: errori nella produzione del json da loggare non devono compromettere la scrittura del record di log
        End Try

        Try
            Dim log As New AgronicaCoreEntityFramework_POCO.Agronica_Log_Agenda
            log.SuperUser = objParametriServer.PivaSuperUser
            log.Utente = objParametriServer.UsernameOperazione
            log.Data_Ora_Lavorazione = Data_Ora_Lavorazione
            log.Tipo_Operazione = Tipo_Operazione
            log.Des_lib = Des_Lib
            log.Id_Agenda = Id_Agenda
            log.Piva = Piva
            log.Sa_Cod = Sa_Cod
            log.Lav_Cod = Lav_Cod
            log.Id_Servizio = Id_Servizio
            log.Data_Ora_RegistrazioneLog = DateTime.Now
            log.object_data = jsonAgenda
            log.Origine = Origine
            log.Raccoglitore_Cod = Raccoglitore_Cod

            GiasContext.Agronica_Log_Agenda.Add(log)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
