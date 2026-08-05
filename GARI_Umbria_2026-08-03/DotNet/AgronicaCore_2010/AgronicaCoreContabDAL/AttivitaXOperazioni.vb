Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class AttivitaXOperazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Id_Attivita As Int32,
                          ByVal Lav_Cod As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal piva As String = "",
                          Optional ByVal xSelezioneVariabile As enumSelezioneVariabile = enumSelezioneVariabile.Selezione_JoinCompleta
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.AttivitaXOperazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali : 
        '   Lav_Cod=0
        '   ID_Attivita=0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
 
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT DISTINCT AO.Lav_Cod, A.Attivita_Poliannuale ")
                    StrSQL.AppendLine(" FROM  AttivitaxOperazioni AO ")
                    StrSQL.AppendLine(" INNER JOIN Attivita A ON AO.ID_Attivita = A.ID_Attivita ")
                    StrSQL.AppendLine(" WHERE AO.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   AO.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   AO.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Id_Attivita <> 0 Then
                        StrSQL.AppendLine(" AND AO.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND AO.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   AO.Inviato  >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   AO.Inviato  =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT DISTINCT ISNULL(A.ID_Attivita,'') AS ID_Attivita, A.[Desc] AS Descrizione, A.[Desc], A.Sigla, ISNULL(O.Lav_Cod,0) AS Lav_Cod, ISNULL(O.Lav_Des,'') AS Lav_Des, A.Tariffa_Cod, T.Tariffa_Des ")
                    StrSQL.AppendLine(" FROM  Attivita A ")
                    'StrSQL.AppendLine(" LEFT OUTER JOIN AttivitaxOperazioni AO ON A.ID_Attivita = AO.ID_Attivita AND A.Piva = AO.Piva_SuperUser ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN AttivitaxOperazioni AO ON A.ID_Attivita = AO.ID_Attivita ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Operazioni O ON AO.Lav_Cod = O.Lav_Cod ")
                    StrSQL.AppendLine(" LEFT OUTER JOIN Tariffe T ON T.Tariffa_Cod = A.Tariffa_Cod ")
                    StrSQL.AppendLine(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


                    If Id_Attivita <> 0 Then
                        StrSQL.AppendLine(" AND AO.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND AO.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    'Asteriscato momentaneamente in attesa di fare chiamate specifiche per ogni PIVA
                    'If piva <> "" Then
                    '    StrSQL.AppendLine(" AND (A.piva = '" & piva & "' or sa_cod = -1) ")
                    'End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   A.Inviato  >=0 ")
                    'StrSQL.AppendLine(" AND   AO.Inviato >=0 ")
                    'StrSQL.AppendLine(" AND   O.Inviato  >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   A.Inviato  =-1 ")
                    'StrSQL.AppendLine(" AND   AO.Inviato =-1 ")
                    'StrSQL.AppendLine(" AND   O.Inviato  =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY [Desc] ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT AO.Piva_SuperUser,AO.ID_Attivita, A.[Desc] , AO.LAV_COD, O.LAV_DES,")
                    StrSQL.AppendLine(" ISNULL(GO.GRU_COD,0) AS GRU_COD, ISNULL(GO.GRU_DES,'') AS GRU_DES")
                    StrSQL.AppendLine(" FROM  AttivitaxOperazioni AO ")
                    StrSQL.AppendLine(" LEFT JOIN Operazioni O ON AO.Lav_Cod = O.Lav_Cod ")
                    StrSQL.AppendLine(" LEFT JOIN GruppoOperazioni GO ON GO.GRU_COD = O.GRU_OP ")
                    StrSQL.AppendLine(" LEFT JOIN Attivita A ON AO.ID_Attivita = A.ID_Attivita ")
                    StrSQL.AppendLine(" WHERE AO.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.AppendLine(" AND   AO.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.AppendLine(" AND   AO.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


                    If Id_Attivita <> 0 Then
                        StrSQL.AppendLine(" AND AO.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                    End If

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND AO.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    Else
                        StrSQL.AppendLine(" AND ((GO.Tipo in ('C','Z') AND (GO.ATT_COD < 10000 AND GO.ATT_COD <> 2500)) OR O.Lav_Cod = 5007)")
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   AO.Inviato  >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   AO.Inviato  =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY AO.Data_Creazione Desc")
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


    Public Function Leggi_Solo_Attivita(ByVal Id_Attivita As Int32,
                                        ByVal Lav_Cod As Int32,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreParametri,
                                        Optional ByVal bInner_Join As Boolean = False,
                                        Optional ByVal Piva As String = "",
                                        Optional ByVal bDescrizione_Estesa As Boolean = False,
                                        Optional ByVal Budget As Integer = 2,
                                        Optional ByVal Raccoglitore_Cod As Integer = 0,
                                        Optional ByVal isAttivitaInterna As Boolean = False,
                                        Optional ByVal isTimeSheet As Boolean = False
                                        ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.AttivitaXOperazioni_R.Leggi_Solo_Attivita()"

        '====================================================================================
        'Parametri opzionali : 
        '   Lav_Cod=0
        '   ID_Attivita=0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT DISTINCT ISNULL(A.ID_Attivita,'') AS ID_Attivita, A.[Desc] AS Descrizione,  ")

            Select Case bDescrizione_Estesa
                Case True
                    StrSQL.AppendLine(" ( A.[Sigla] + ' - ' + A.[Desc]) AS [Desc] ")
                Case False
                    StrSQL.AppendLine("  A.[Desc] ")
            End Select

            StrSQL.AppendLine(" , a.attivita_poliannuale, a.Sa_Cod ")
            StrSQL.AppendLine(" , a.Sigla ")
            StrSQL.AppendLine(" FROM  Attivita A ")

            If Not bInner_Join Then
                StrSQL.AppendLine(" LEFT OUTER ")
            End If

            StrSQL.AppendLine(" JOIN AttivitaxOperazioni AO ON A.ID_Attivita = AO.ID_Attivita ")

            If Not bInner_Join Then
                StrSQL.AppendLine(" LEFT OUTER ")
            End If


            StrSQL.AppendLine(" JOIN Operazioni O ON AO.Lav_Cod = O.Lav_Cod ")
            StrSQL.AppendLine(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Piva <> "" Then
                StrSQL.AppendLine(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.AppendLine(" AND (A.Piva ='" & Agro_SQL_SaveText(Piva) & "' Or A.Sa_Cod = -1) ")
            Else
                'Marco L: Vecchia Gestione
                StrSQL.AppendLine(" AND   A.Piva = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            End If

            If Id_Attivita <> 0 Then
                StrSQL.AppendLine(" AND AO.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If


            Select Case Raccoglitore_Cod

                Case 0

                    If Lav_Cod <> 0 Then
                        StrSQL.AppendLine(" AND AO.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                Case Else 'Tutti i lav_cod validi per il raccoglitore

                    StrSQL.AppendLine(" AND AO.Lav_Cod In (Select Lav_Cod From Agenda Where Raccoglitore_Cod =  " & Agro_SQL_SaveNum(Raccoglitore_Cod) & ") ")

            End Select


            'Filtro Tipo Utilizzo
            Select Case Budget

                Case 0
                    'Consuntivo o Tutti
                    StrSQL.AppendLine(" AND A.Tipo_Utilizzo In (0,2) ")

                Case 1
                    'Budget o Tutti
                    StrSQL.AppendLine(" AND A.Tipo_Utilizzo In (1,2) ")

                Case Else
                    'Nessun Filtro

            End Select

            'Nel TimeSheet non faccio filtri sulle attività
            If Not isTimeSheet Then
                If isAttivitaInterna Then
                    StrSQL.AppendLine(" AND A.Attivita_Interna = 1 ")
                Else
                    StrSQL.AppendLine(" AND A.Attivita_Interna = 0")
                End If
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   A.Inviato  >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   A.Inviato  =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY [Desc] ASC ")
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


    Public Function LavCod_From_IdAttivita(ByVal Id_Attivita As Int32,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.AttivitaXOperazioni_R.LavCod_From_IdAttivita()"

        '====================================================================================
        'Parametri opzionali : 
        '   ID_Attivita=0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim intRet As Integer = 0

        Try

            '------------------------------------------------------------------

            StrSQL.Length = 0
            StrSQL.Append(" SELECT ISNULL(AO.Lav_Cod,0) AS Lav_Cod  ")
            StrSQL.Append(" FROM  Attivita A ")
            StrSQL.Append(" LEFT OUTER JOIN AttivitaxOperazioni AO ON A.ID_Attivita = AO.ID_Attivita ")
            StrSQL.Append(" WHERE A.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   A.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND   A.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

            If Id_Attivita <> 0 Then
                StrSQL.Append(" AND AO.Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   A.Inviato  >=0 ")
                    'StrSQL.Append(" AND   AO.Inviato >=0 ")
                    'StrSQL.Append(" AND   O.Inviato  >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   A.Inviato  =-1 ")
                    'StrSQL.Append(" AND   AO.Inviato =-1 ")
                    'StrSQL.Append(" AND   O.Inviato  =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
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

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then
            intRet = dt.Rows(0).Item("Lav_Cod")
        Else
            intRet = 0
        End If

        Return intRet

    End Function

End Class



Public Class AttivitaXOperazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Id_Attivita As Int32,
                           ByVal Lav_Cod As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AttivitaXOperazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO AttivitaXOperazioni( ")
            StrSQL.Append("                     Piva_Superuser,      ID_Attivita,        Lav_Cod, ")
            StrSQL.Append("                     Inviato,            DataInvio, ")
            StrSQL.Append("                     Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                     UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                     Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                     ) ")

            StrSQL.Append("VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & " ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(" )")

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


    Public Function Cancella(ByVal Id_Attivita As Int32,
                             ByVal Lav_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AttivitaXOperazioni_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE AttivitaXOperazioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva_Superuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND ID_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     AttivitaXOperazioni ")
                StrSQL.Append(" WHERE  Piva_Superuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND ID_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
                StrSQL.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                StrSQL.Append(" AND Inviato = 0")

            End If

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
