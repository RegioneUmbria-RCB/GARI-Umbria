Imports System.Data.Entity
Imports System.Text.RegularExpressions
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework
Imports AgronicaCoreEntityFramework_POCO

Public Class Agenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '################################################################################
    Public Function Piva_Ha_Operazioni(ByVal Piva As String,
                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                       ) As Boolean

        Dim DT As DataTable
        Dim Flag_esiste As Boolean

        DT = Leggi_Distinct_Piva(Piva, 0,
                                 " LAV_COD <> 1008", "",
                                 objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count <> 0 Then
            Flag_esiste = True
        End If

        Return Flag_esiste

    End Function

    '################################################################################
    Public Function Leggi_Distinct_Piva(ByVal Piva As String,
                                        ByVal Lav_Cod As Int32,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_Distinct_Piva()"

        '====================================================================================
        'Parametri opzionali :
        '   Lav_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT DISTINCT Piva ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE 1 = 1  ")

            If Piva <> "" Then
                strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Lav_Cod <> 0 Then
                strSql.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Agenda.Inviato =-1 ")
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

    Public Function LeggiDistinctLavCod(ByVal idAgendaList As List(Of Integer), ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As List(Of Integer)
        Dim lavCodList = New List(Of Integer)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.LeggiDistinctLavCod()"

        Dim messaggioErrore As String = ""
        Dim sb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try
            sb.Length = 0
            sb.AppendLine("SELECT DISTINCT Lav_Cod")
            sb.AppendLine("FROM Agenda")
            sb.AppendLine($"WHERE Id_Agenda IN ({String.Join(", ", idAgendaList)})")

            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            For Each row As DataRow In dt.Rows
                If Not IsNothing(row("Lav_Cod")) Then
                    lavCodList.Add(CInt(row("Lav_Cod")))
                End If
            Next
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return lavCodList
    End Function

    Public Function Leggi_Distinct_Anno(ByVal Piva As String,
                                        ByVal Lav_Cod As Integer,
                                        ByVal xFiltroAggiuntivo As String,
                                        ByVal xOrderBy As String,
                                        ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_Distinct_Anno()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Lav_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT DISTINCT YEAR(Validita_Inizio) AS Anno ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE 1 = 1  ")

            If Piva <> "" Then
                strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Lav_Cod <> 0 Then
                strSql.Append(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Anno DESC ")
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

    Public Function Lav_Cod_From_Id_Agenda(ByVal Id_Agenda As Integer,
                                           ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                           ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Lav_Cod_From_Piva_Sa_Cod_Id_Agenda()"

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT lav_cod ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then
                Throw New Exception(" Nessuna op agenda trovata ")
            End If
            If dt.Rows.Count > 1 Then
                Throw New Exception(" Troppe op agenda trovate ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return CInt(dt.Rows(0).Item("lav_cod"))

    End Function

    Public Function IdAgende_From_Piva_Raccoglitore_Cod(ByVal Piva As String,
                                                       ByVal Raccoglitore_Cod As Integer,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.IdAgende_From_Piva_Raccoglitore_Cod()"

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT Id_Agenda ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE 1=1 ")

            strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            strSql.Append(" AND Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")

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


    Public Function Raccoglitore_Cod_From_Piva_Id_Agenda(ByVal Piva As String,
                                                       ByVal Id_Agenda As Integer,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Raccoglitore_Cod_From_Piva_Id_Agenda()"

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT raccoglitore_cod ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE 1=1 ")

            strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            strSql.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then
                Throw New Exception(" Nessuna op agenda trovata ")
            End If
            If dt.Rows.Count > 1 Then
                Throw New Exception(" Troppe op agenda trovate ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return CInt(dt.Rows(0).Item("raccoglitore_cod"))

    End Function

    Public Function Lav_Cod_From_Piva_Sa_Cod_Id_Agenda(ByVal Piva As String,
                                                       ByVal Sa_Cod As Integer,
                                                       ByVal Id_Agenda As Integer,
                                                       ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                                       ) As Integer

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Lav_Cod_From_Piva_Sa_Cod_Id_Agenda()"

        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT lav_cod ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE 1=1 ")

            strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            strSql.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")

            strSql.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt.Rows.Count = 0 Then
                Throw New Exception(" Nessuna op agenda trovata ")
            End If
            If dt.Rows.Count > 1 Then
                Throw New Exception(" Troppe op agenda trovate ")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return CInt(dt.Rows(0).Item("lav_cod"))

    End Function

    Public Function NumetoTotale(ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As Integer
        Dim strSql As New System.Text.StringBuilder
        strSql.Append(" SELECT count(*) as Numero ")
        strSql.Append(" FROM  Agenda ")
        Dim dt As DataTable = EseguiQuery_Lettura(objParametri, strSql.ToString, "NumetoTotale")
        Return dt.Rows(0).Item("Numero")
    End Function

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Lav_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0,
                          Optional ByVal Audit_Cod As Integer = 0,
                          Optional ByVal Raccoglitore_Cod As Integer = 0,
                          Optional ByVal DescrizioneOperazione_xDocumentale As Boolean = False
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Lav_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" Select * ")
                    strSql.AppendLine(" From Agenda WITH(NOLOCK)")

                    strSql.AppendLine("  Where 1 = 1 ")

                    If Piva <> "" Then
                        strSql.AppendLine(" And Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
                    End If

                    If Lav_Cod <> 0 Then
                        strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND   Agenda.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY Agenda.Lav_Cod ASC, Agenda.Des_Lib ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                    If TipoG2G = 3 Then

                        strSql.Length = 0
                        strSql.AppendLine(" select rr.FromPiva as Piva, rr.FromSa_cod as Sa_cod, rr.FromId_Agenda as ID_Agenda ")
                        strSql.AppendLine("  from G2G_recode_Agenda rr WITH(NOLOCK)")
                        strSql.AppendLine("  where not exists( ")
                        strSql.AppendLine(" 	select 1 ")
                        strSql.AppendLine(" 	from agenda aa WITH(NOLOCK)")
                        strSql.AppendLine(" 	where aa.Id_Agenda = rr.FromId_Agenda ")
                        strSql.AppendLine(" 	and aa.piva = rr.FromPiva ")
                        strSql.AppendLine("  ) ")
                        strSql.AppendLine(" AND rr.FromPiva = '" & Agro_SQL_SaveText(Piva) & "' ")

                    Else

                        strSql.Length = 0
                        strSql.AppendLine(" SELECT * ")
                        If DescrizioneOperazione_xDocumentale = True Then
                            strSql.AppendLine(" , Operazioni.LAV_DES AS Operazione")
                        End If
                        strSql.AppendLine(" FROM  Agenda WITH(NOLOCK)")

                        If DescrizioneOperazione_xDocumentale = True Then
                            strSql.AppendLine(" LEFT JOIN Operazioni WITH(NOLOCK)")
                            strSql.AppendLine(" ON Operazioni.LAV_COD = Agenda.Lav_Cod")
                        End If

                        If TipoG2G = 1 Then
                            strSql.AppendLine(" WHERE Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                            strSql.AppendLine(" And Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                        Else
                            strSql.AppendLine(" WHERE Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                            strSql.AppendLine(" And   Agenda.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                        End If

                        If Piva <> "" Then
                            strSql.AppendLine(" And Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                        End If

                        If Sa_Cod <> 0 Then
                            strSql.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                        End If

                        If Id_Agenda <> 0 Then
                            strSql.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
                        End If

                        If Lav_Cod <> 0 Then
                            strSql.AppendLine(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                        End If

                        Select Case TipoG2G
                            Case 1 'seleziona i nuovi dati.
                                strSql.AppendLine(" AND not exists ( ")
                                strSql.AppendLine(" select 1 from g2g_recode_agenda rr WITH(NOLOCK)")
                                strSql.AppendLine(" where rr.FromPiva = Agenda.PIVA And rr.FromSa_cod = Agenda.Sa_Cod And rr.FromId_Agenda = Agenda.Id_Agenda ")
                                strSql.AppendLine(" ) ")

                            Case 2 'seleziona i dati modificati

                                strSql.AppendLine("And exists ( ")
                                strSql.AppendLine(" 	select 1 ")
                                strSql.AppendLine(" 	from G2G_Recode_Agenda rr WITH(NOLOCK)")
                                strSql.AppendLine(" 	where rr.DataInvio < Agenda.Data_Modifica ")
                                strSql.AppendLine(" 	And rr.FromPiva = Agenda.piva  ")
                                strSql.AppendLine(" 	And rr.FromSa_cod = Agenda.Sa_cod  ")
                                strSql.AppendLine(" 	And  rr.FromId_Agenda = Agenda.Id_Agenda ")
                                strSql.AppendLine(" ) ")

                        End Select

                        If Audit_Cod <> 0 Then
                            strSql.AppendLine(" And Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & " ")
                        End If

                        If Raccoglitore_Cod <> 0 Then
                            strSql.AppendLine(" And Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
                        End If

                        If xFiltroAggiuntivo <> "" Then
                            strSql.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If
                        '--------------------------------------------------------------------------
                        Select Case objParametri.FlagVisibilita
                            Case enumVisibilita.Visibilita_SoloNonCancellati
                                strSql.AppendLine(" And   Agenda.Inviato >=0 ")
                            Case enumVisibilita.Visibilita_SoloCancellati
                                strSql.AppendLine(" And   Agenda.Inviato =-1 ")
                            Case enumVisibilita.Visibilita_Tutti
                                '...................................
                            Case Else
                                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                        End Select
                        '--------------------------------------------------------------------------
                        If xOrderBy <> "" Then
                            strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        Else
                            strSql.AppendLine(" ORDER BY Agenda.Lav_Cod ASC, Agenda.Des_Lib ASC ")
                        End If

                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "]  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiReverse(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Lav_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0,
                          Optional ByVal Audit_Cod As Integer = 0,
                          Optional ByVal Raccoglitore_Cod As Integer = 0,
                          Optional ByVal From_PivaSuperUser As String = ""
                          ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.LeggiReverse()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Lav_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                    If TipoG2G = 3 Then

                        strSql.Length = 0
                        strSql.AppendLine(" select rr.FromPiva as Piva, rr.FromSa_cod as Sa_cod, rr.FromId_Agenda as ID_Agenda ")
                        strSql.AppendLine("  from G2G_recode_Agenda rr ")
                        strSql.AppendLine("  where Not exists( ")
                        strSql.AppendLine(" 	select 1 ")
                        strSql.AppendLine(" 	from agenda aa ")
                        strSql.AppendLine(" 	where aa.Id_Agenda = rr.ToId_Agenda ")
                        strSql.AppendLine(" 	And aa.piva = rr.ToPiva ")
                        strSql.AppendLine("  ) ")
                        strSql.AppendLine(" And rr.FromPiva = '" & Agro_SQL_SaveText(Piva) & "' ")
                        strSql.AppendLine(" AND rr.From_PivaSuperUser = '" & Agro_SQL_SaveText(From_PivaSuperUser) & "' ")

                    Else

                        strSql.Length = 0
                        strSql.AppendLine(" SELECT * ")
                        strSql.AppendLine(" FROM  Agenda ")

                        If TipoG2G = 1 Then
                            strSql.AppendLine(" WHERE Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                            strSql.AppendLine(" AND Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                        Else
                            strSql.AppendLine(" WHERE Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                            strSql.AppendLine(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                        End If

                        If Piva <> "" Then
                            strSql.AppendLine(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                        End If

                        If Sa_Cod <> 0 Then
                            strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                        End If

                        If Id_Agenda <> 0 Then
                            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
                        End If

                        If Lav_Cod <> 0 Then
                            strSql.AppendLine(" AND Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
                        End If

                        Select Case TipoG2G
                            Case 1 'seleziona i nuovi dati.
                                strSql.AppendLine(" AND not exists ( ")
                                strSql.AppendLine(" select 1 from g2g_recode_agenda rr where rr.ToPiva = Agenda.piva and rr.ToSa_cod = Agenda.Sa_cod and  rr.ToId_Agenda = Agenda.id_agenda AND rr.From_PivaSuperUser = '" & Agro_SQL_SaveText(From_PivaSuperUser) & "' ")
                                strSql.AppendLine(" ) ")

                            Case 2 'seleziona i dati modificati

                                strSql.AppendLine("And exists ( ")
                                strSql.AppendLine(" 	Select 1 ")
                                strSql.AppendLine(" 	from G2G_Recode_Agenda rr ")
                                strSql.AppendLine(" 	where rr.DataInvio < Agenda.Data_Modifica ")
                                strSql.AppendLine(" 	And rr.ToPiva = Agenda.PIVA  ")
                                strSql.AppendLine(" 	And rr.ToSa_cod = Agenda.Sa_cod  ")
                                strSql.AppendLine(" 	And rr.ToId_Agenda = Agenda.Id_Agenda ")
                                strSql.AppendLine("     And rr.From_PivaSuperUser = '" & Agro_SQL_SaveText(From_PivaSuperUser) & "' ")
                                strSql.AppendLine(" ) ")

                        End Select

                        If Audit_Cod <> 0 Then
                            strSql.AppendLine(" And Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & " ")
                        End If

                        If Raccoglitore_Cod <> 0 Then
                            strSql.AppendLine(" And Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
                        End If

                        If xFiltroAggiuntivo <> "" Then
                            strSql.AppendLine(" And " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                        End If
                        '--------------------------------------------------------------------------
                        Select Case objParametri.FlagVisibilita
                            Case enumVisibilita.Visibilita_SoloNonCancellati
                                strSql.AppendLine(" And   Agenda.Inviato >=0 ")
                            Case enumVisibilita.Visibilita_SoloCancellati
                                strSql.AppendLine(" And   Agenda.Inviato =-1 ")
                            Case enumVisibilita.Visibilita_Tutti
                                '...................................
                            Case Else
                                Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                        End Select
                        '--------------------------------------------------------------------------
                        If xOrderBy <> "" Then
                            strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                        Else
                            strSql.AppendLine(" ORDER BY Lav_Cod ASC, Des_Lib ASC ")
                        End If

                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
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

    Public Function Leggi_Agenda_BC(ByVal piva As String,
                                    ByVal idAgenda As Integer,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_R.Leggi_Agenda_BC()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Movimenti.Id_Agenda, Agenda.Lav_Cod, Movimenti.Id_Mov, Movimenti.Cau_Mov  ")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine(" AND  Agenda.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            strSql.AppendLine(" AND  Movimenti.Cau_Mov In ('4000', '7300', '7350') ")
            'strSql.AppendLine(" AND  EXISTS (SELECT * FROM Movimenti_Dettagli ")
            'strSql.AppendLine("              WHERE Movimenti_Dettagli.Piva = Movimenti.Piva ")
            'strSql.AppendLine("              AND   Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            'strSql.AppendLine("              AND   Movimenti_Dettagli.Elem_Cod In (205, 305)) ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Agenda_Riferimento_BC(ByVal piva As String,
                                                ByVal idAgenda As Integer,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_R.Leggi_Agenda_Riferimento_BC()"
        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Movimenti.Id_Agenda, Agenda.Lav_Cod, Movimenti.Id_Mov, Movimenti.Cau_Mov  ")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti ON Agenda.Piva = Movimenti.Piva AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine(" INNER JOIN Mov_Dettagli_Riferimenti ON Agenda.Piva = Mov_Dettagli_Riferimenti.Piva AND Agenda.Id_Agenda = Mov_Dettagli_Riferimenti.Id_Agenda ")
            strSql.AppendLine(" WHERE Mov_Dettagli_Riferimenti.Piva_Rif = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine(" AND  Mov_Dettagli_Riferimenti.Id_Agenda_Rif = " & Agro_SQL_SaveNum(idAgenda) & " ")
            strSql.AppendLine(" AND  Movimenti.Cau_Mov In ('4000', '7350', '7300') ")
            strSql.AppendLine(" AND  EXISTS (SELECT * FROM Movimenti_Dettagli ")
            strSql.AppendLine("              WHERE Movimenti_Dettagli.Piva = Movimenti.Piva ")
            strSql.AppendLine("              AND   Movimenti_Dettagli.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.AppendLine("              AND   Movimenti_Dettagli.Elem_Cod In (205, 305)) ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiMovimentoBeniConfezionamento(ByVal piva As String,
                                                      ByVal idAgenda As Integer,
                                                      ByVal idMov As Integer,
                                                      ByVal cauMov As String,
                                                      ByVal filtroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_R.LeggiMovimentoBeniConfezionamento()"
        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.AppendLine(" SELECT Movimenti_Dettagli.Qta_Extra as Qta_Extra, Movimenti_Dettagli.Qta_Extra as Tara, Movimenti_Dettagli.*,  Mov_Destinazioni.*, ")
            strSql.AppendLine("        Materie_Prime.Mat_Cod, Materie_Prime.Mat_Des, Materie_Prime.Cod_Articolo, ISNULL(Tabella_Cod, 0) AS Tabella_Cod,  ")
            strSql.AppendLine("        Materie_Prime.ChkImballaggio, Materie_Prime.ChkContenitore, ")
            strSql.AppendLine("        m_Int.Data_Movimento, m_Int.Doc_Numero_Sin, m_Int.Doc_Numero, m_Int.Doc_Numero_Des, m_Int.Doc_Numero_Visualizzato ")
            strSql.AppendLine(" FROM Agenda ")
            strSql.AppendLine(" INNER JOIN Movimenti m_Int ON Agenda.Piva = m_Int.Piva AND Agenda.Id_Agenda = m_Int.Id_Agenda ")
            strSql.AppendLine("            AND m_Int.Cau_Mov = '" & CAU_REGISTRAZIONI & "' ")
            strSql.AppendLine(" INNER JOIN Movimenti m_Mag ON Agenda.Piva = m_Mag.Piva AND Agenda.Id_Agenda = m_Mag.Id_Agenda ")
            strSql.AppendLine("            AND m_Mag.Cau_Mov = '" & Agro_SQL_SaveText(cauMov) & "' ")
            strSql.AppendLine(" INNER JOIN Movimenti_Dettagli ON m_Mag.Piva = Movimenti_Dettagli.Piva AND m_Mag.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND m_Mag.Id_Mov = Movimenti_Dettagli.Id_Mov ")
            strSql.AppendLine(" INNER JOIN Mov_Destinazioni ON Mov_Destinazioni.Piva = Movimenti_Dettagli.Piva AND Mov_Destinazioni.Id_Agenda = Movimenti_Dettagli.Id_Agenda AND Mov_Destinazioni.Id_Mov = Movimenti_Dettagli.Id_Mov AND Mov_Destinazioni.Id_Mov_Det = Movimenti_Dettagli.Id_Mov_Det ")
            strSql.AppendLine(" INNER JOIN Materie_Prime ON Movimenti_Dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_Dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")
            strSql.AppendLine(" LEFT OUTER JOIN oTabelle_Parametri ON Materie_Prime.Mat_Cod = oTabelle_Parametri.Mat_Cod_Generazione_Link ")
            strSql.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            strSql.AppendLine(" AND   Agenda.Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & " ")
            strSql.AppendLine(" AND   m_Mag.Id_Mov = " & Agro_SQL_SaveNum(idMov) & " ")
            strSql.AppendLine(" AND   Movimenti_Dettagli.Elem_Cod In (205, 305) ")
            strSql.AppendLine(" AND   Mov_Destinazioni.Tipo_Destinazione = " & CostantiPersonalizzate.MAGAZZINO & " ")

            If filtroAggiuntivo <> "" Then
                strSql.Append(" AND   " & Agro_SQL_Save_xFiltroAggiuntivo(filtroAggiuntivo, , objParametri) & " ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " :  " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_con_Movimenti_xStatistiche(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                     ByVal DataInizio As Date,
                                                     ByVal DataFine As Date,
                                                     ByVal Username As String,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByVal xOrderBy As String,
                                                     ByRef objParametri As AgronicaCoreParametri,
                                                     Optional ByVal Applica_VisibilitaUtente As Boolean = False
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_R.Leggi_con_Movimenti_xStatistiche()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Lav_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0
            strSql.Append(" SELECT Movimenti.Cau_Mov, Agenda.Username_Creazione, Agenda.Data_Creazione , Agenda.id_agenda, Movimenti.data_movimento ")
            strSql.Append("     FROM         Agenda INNER JOIN ")
            strSql.Append("       Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON Agenda.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            ''strSQL.Append(" WHERE Agenda.Data_Creazione <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            ''strSQL.Append(" AND   Agenda.Data_Creazione >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            'strSql.Append(" WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            'strSql.Append(" AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            'carichi, trattamenti, rilievi in campo, rilievi alla raccolta, lavorazioni
            strSql.Append(" AND   Movimenti.cau_mov IN ('7300','2050','2100','2200','2300') ")
            '(01/03/2019 fede) escluse dal conteggio le operazioni salvate in automatico
            strSql.Append(" AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append(" AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Lav_Cod ASC, Des_Lib ASC ")
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

    Public Function Leggi_con_Movimenti_xStatistiche_DistinctPiva(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                  ByVal DataInizio As Date,
                                                                  ByVal DataFine As Date,
                                                                  ByVal Username As String,
                                                                  ByVal xFiltroAggiuntivo As String,
                                                                  ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreParametri,
                                                                  Optional ByVal Applica_VisibilitaUtente As Boolean = False
                                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_con_Movimenti_xStatistiche_DistinctPiva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0
            strSql.Append(" SELECT DISTINCT Agenda.PIVA ")
            strSql.Append(" FROM  Agenda INNER JOIN ")
            strSql.Append("       Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Sa_Cod = Movimenti.Sa_Cod AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON Agenda.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select

            ''strSQL.Append(" WHERE Agenda.Data_Creazione <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            ''strSQL.Append(" AND   Agenda.Data_Creazione >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
            'strSql.Append(" WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            'strSql.Append(" AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            'carichi, trattamenti, rilievi in campo, rilievi alla raccolta, lavorazioni
            strSql.Append(" AND   Movimenti.cau_mov IN ('7300','2050','2100','2200','2300') ")
            '(01/03/2019 fede) escluse dal conteggio le operazioni salvate in automatico
            strSql.Append(" AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append(" AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY  Agenda.Piva ")
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

    Public Function Leggi_NumOperazioni_xStatistiche_DistinctPiva(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                  ByVal DataInizio As Date,
                                                                  ByVal DataFine As Date,
                                                                  ByVal Username As String,
                                                                  ByVal xFiltroAggiuntivo As String,
                                                                  ByVal xOrderBy As String,
                                                                  ByRef objParametri As AgronicaCoreParametri,
                                                                  Optional ByVal Applica_VisibilitaUtente As Boolean = False,
                                                                  Optional ByVal Dettagli_QDC As Boolean = False
                                                                  ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_NumOperazioni_xStatistiche_DistinctPiva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try


            strSql.Length = 0
            strSql.Append(" select  ")

            strSql.Append("  ISNULL(( SELECT  top 1  Lista_Regioni.regione_des  " & vbCrLf)
            strSql.Append("            From Lista_Regioni   " & vbCrLf)
            strSql.Append("            right Join Lista_Province On Lista_Regioni.reg=Lista_Province.REG " & vbCrLf)
            strSql.Append("            right Join Indirizzi On Indirizzi.pro_cod_istat = Lista_Province.PROV " & vbCrLf)
            strSql.Append("            right join ImpresexIndirizzi on   Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo " & vbCrLf)
            strSql.Append("            WHERE  i.PIVA = ImpresexIndirizzi.PIVA ), ' ') as Regione  " & vbCrLf)

            strSql.Append(" , ISNULL(( SELECT  top 1  istat.COMUNI_PROV  " & vbCrLf)
            strSql.Append("            From istat   " & vbCrLf)
            strSql.Append("            right Join Indirizzi On istat.PROV=Indirizzi.pro_cod_istat And istat.COM=Indirizzi.com_cod_istat " & vbCrLf)
            strSql.Append("            right join ImpresexIndirizzi on   Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo " & vbCrLf)
            strSql.Append("            WHERE  i.PIVA = ImpresexIndirizzi.PIVA ), ' ') as Provincia  " & vbCrLf)

            strSql.Append(" , ISNULL(( SELECT  top 1  istat.LOCALITA  " & vbCrLf)
            strSql.Append("            From istat   " & vbCrLf)
            strSql.Append("            right Join Indirizzi On istat.PROV=Indirizzi.pro_cod_istat And istat.COM=Indirizzi.com_cod_istat " & vbCrLf)
            strSql.Append("            right join ImpresexIndirizzi on   Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo " & vbCrLf)
            strSql.Append("            WHERE  i.PIVA = ImpresexIndirizzi.PIVA ), ' ') as Comune  " & vbCrLf)

            strSql.Append(" , ISNULL(( SELECT  top 1  imprese1.rag_soc + ' - ' + imprese1.piva  " & vbCrLf)
            strSql.Append("            From imprese imprese1    " & vbCrLf)
            strSql.Append("            right Join GerarchiaImprese On GerarchiaImprese.padre = imprese1.piva " & vbCrLf)
            strSql.Append("            WHERE  i.PIVA = GerarchiaImprese.figlio ), ' ') as Referente  " & vbCrLf)

            strSql.Append(" , i.rag_soc as RagioneSociale, ag.piva as Piva ")

            strSql.Append(" , ISNULL(( SELECT  top 1  val_cod     FROM Imprese_Codici  " & vbCrLf)
            strSql.Append("            WHERE  i.PIVA = Imprese_Codici.PIVA And id_cod = '1010'), '') as Cuaa  " & vbCrLf)

            strSql.Append(" , ag.Num_Operazioni_Campagna  ")

            If Dettagli_QDC Then

                strSql.Append(" , ag.Num_Trattamenti, ag.Num_Concimazioni, ag.Num_Operazioni_Campagna_Altre ")

                strSql.Append(" , ISNULL(( SELECT  top 1  convert(varchar, pratiche_stati_attuali.validita_inizio, 3) ")
                strSql.Append(" From pratiche, pratiche_stati_attuali ")
                strSql.Append(" WHERE  i.PIVA = Pratiche.Piva ")
                strSql.Append(" And pratiche.pratica_cod = pratiche_stati_attuali.pratica_cod ")
                strSql.Append(" And pratiche.Servizio_Cod = 2004 And pratiche_stati_attuali.Stato_Cod In (2003, 2005) ")
                strSql.Append(" And pratiche_stati_attuali.validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                strSql.Append(" And pratiche_stati_attuali.validita_inizio >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                strSql.Append(" ), ' ') as Data_Stampa_QDC ")

            End If

            strSql.Append(" , ag.Num_Operazioni_Magazzino ")


            strSql.Append("  , ag.Num_PUA, ag.Num_PianiConcimazione ")


            strSql.Append(" from imprese i inner join  ")

            strSql.Append(" (  ")
            strSql.Append("     select i.piva  ")
            strSql.Append("     , isnull(b.operazioni,0) as Num_Operazioni_Campagna  ")

            If Dettagli_QDC Then

                strSql.Append("    ,isnull(b_trattamenti.operazioni, 0) As Num_Trattamenti ")
                strSql.Append("    ,isnull(b_concimazioni.operazioni, 0) As Num_Concimazioni ")
                strSql.Append("    ,isnull(b_altre.operazioni, 0) As Num_Operazioni_Campagna_Altre ")

            End If

            strSql.Append("    , isnull(a.operazioni,0) as Num_Operazioni_Magazzino  ")

            strSql.Append("     ,isnull(c.operazioni,0) as Num_PUA,isnull(d.operazioni,0) as Num_PianiConcimazione  ")

            strSql.Append("     from ")
            strSql.Append("     (  ")
            strSql.Append("         select piva from imprese  ")
            strSql.Append("     where 1=1  ")
            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            strSql.Append("     ) i  ")

            'CARICHI MAGAZZINO
            strSql.Append("    LEFT JOIN  (  ")
            strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
            strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA And Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append("         INNER JOIN imprese On agenda.piva=imprese.piva ")
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         And   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                Case Else
                    strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         And   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            End Select
            strSql.Append("         And   Movimenti.cau_mov In ('7300') ")

            '(01/03/2019 fede) elimino dal conteggio gli scarichi registrati in automatico
            strSql.Append("         AND   agenda.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
            strSql.Append("     ) a  ")
            strSql.Append("     on i.piva=a.PIVA  ")

            strSql.Append("     LEFT join  ")

            'OPERAZIONI CAMPAGNA
            strSql.Append("     (  ")
            strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
            strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
            strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                Case Else
                    strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            End Select
            strSql.Append("         AND   Movimenti.cau_mov IN ('2050','2100','2200','2300') ")
            '(01/03/2019 fede) elimino dal conteggio le operazioni registratie in automatico
            strSql.Append("         AND   agenda.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Agenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Agenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
            strSql.Append("     ) b  ")

            strSql.Append("     on i.piva=b.piva   ")


            If Dettagli_QDC Then

                'OPERAZIONI CAMPAGNA - TRATTAMENTI
                strSql.Append("     LEFT join  ")
                strSql.Append("     (  ")
                strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
                strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.Append("         AND   Movimenti.cau_mov IN ('2050') ")
                '(01/03/2019 fede) elimino dal conteggio le operazioni registratie in automatico
                strSql.Append("         AND   agenda.username_creazione <> '" & objParametri.PivaSuperUser & "'")

                If Username <> "" Then
                    strSql.Append("     AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
                End If

                If xFiltroAggiuntivo <> "" Then
                    strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        strSql.Append(" AND   Agenda.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        strSql.Append(" AND   Agenda.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select

                strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
                strSql.Append("     ) b_trattamenti  ")

                strSql.Append("     on i.piva=b_trattamenti.piva   ")






                'OPERAZIONI CAMPAGNA - CONCIMAZIONI
                strSql.Append("     LEFT join  ")
                strSql.Append("     (  ")
                strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
                strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.Append("         AND   Movimenti.cau_mov IN ('2300') ")
                '(01/03/2019 fede) elimino dal conteggio le operazioni registratie in automatico
                strSql.Append("         AND   agenda.username_creazione <> '" & objParametri.PivaSuperUser & "'")

                If Username <> "" Then
                    strSql.Append("     AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
                End If

                If xFiltroAggiuntivo <> "" Then
                    strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        strSql.Append(" AND   Agenda.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        strSql.Append(" AND   Agenda.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select

                strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
                strSql.Append("     ) b_concimazioni  ")

                strSql.Append("     on i.piva=b_concimazioni.piva   ")






                'OPERAZIONI CAMPAGNA - ALTRE
                strSql.Append("     LEFT join  ")
                strSql.Append("     (  ")
                strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
                strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.Append("         AND   Movimenti.cau_mov IN ('2100','2200') ")
                '(01/03/2019 fede) elimino dal conteggio le operazioni registratie in automatico
                strSql.Append("         AND   agenda.username_creazione <> '" & objParametri.PivaSuperUser & "'")

                If Username <> "" Then
                    strSql.Append("     AND   Agenda.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
                End If

                If xFiltroAggiuntivo <> "" Then
                    strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                End If
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        strSql.Append(" AND   Agenda.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        strSql.Append(" AND   Agenda.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select

                strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
                strSql.Append("     ) b_altre  ")

                strSql.Append("     on i.piva=b_altre.piva   ")


            End If



            strSql.Append("     LEFT join  ")

            'PUA
            strSql.Append("     (  ")
            strSql.Append("         SELECT DISTINCT PUA_Testata.PIVA,imprese.rag_soc,count(*) as operazioni   ")
            strSql.Append("         from PUA_Testata INNER JOIN imprese on PUA_Testata.piva=imprese.piva ")
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         WHERE PUA_Testata.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         AND   PUA_Testata.Validita_Fine >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                Case Else
                    strSql.Append("         WHERE PUA_Testata.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         AND   PUA_Testata.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            End Select
            '(01/03/2019 fede) elimino dal conteggio le operazioni registratie in automatico
            strSql.Append("         AND   PUA_Testata.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   PUA_Testata.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   PUA_Testata.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   PUA_Testata.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.Append("         group by PUA_Testata.piva, Imprese.rag_soc ")
            strSql.Append("     ) c  ")

            strSql.Append("     on i.piva=c.piva   ")


            strSql.Append("     LEFT join  ")

            'PIANI CONCIMAZIONE
            strSql.Append("     (  ")
            strSql.Append("         SELECT DISTINCT PianoConcimazione_Dettagli.PC_Dettagli_PIVA  as piva ,imprese.rag_soc,count(*) as operazioni   ")
            strSql.Append("         from PianoConcimazione_Dettagli INNER JOIN imprese on PianoConcimazione_Dettagli.PC_Dettagli_PIVA=imprese.piva ")
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         WHERE PianoConcimazione_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         AND   PianoConcimazione_Dettagli.Validita_Fine >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                Case Else
                    strSql.Append("         WHERE PianoConcimazione_Dettagli.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("         AND   PianoConcimazione_Dettagli.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            End Select
            '(01/03/2019 fede) elimino dal conteggio le operazioni registratie in automatico
            strSql.Append("         AND   PianoConcimazione_Dettagli.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   PianoConcimazione_Dettagli.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   PianoConcimazione_Dettagli.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   PianoConcimazione_Dettagli.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            strSql.Append("         group by PianoConcimazione_Dettagli.PC_Dettagli_PIVA, Imprese.rag_soc ")
            strSql.Append("     ) d  ")

            strSql.Append("    on i.piva=d.piva  ")

            strSql.Append(" ) ag on i.piva=ag.piva  ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON i.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            strSql.Append(" and not (Num_Operazioni_Campagna=0 and Num_Operazioni_Magazzino=0 and Num_pua=0 and Num_PianiConcimazione=0)  ")

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY i.rag_soc ")
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

    Public Function Leggi_NumOperazioni_xStatistiche_DistinctPiva_New(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                      ByVal DataInizio As Date,
                                                                      ByVal DataFine As Date,
                                                                      ByVal Username As String,
                                                                      ByVal ElencoInsertPive As String,
                                                                      ByVal xFiltroAggiuntivo As String,
                                                                      ByVal xOrderBy As String,
                                                                      ByRef objParametri As AgronicaCoreParametri,
                                                                      Optional ByVal Applica_VisibilitaUtente As Boolean = True,
                                                                      Optional ByVal Dettagli_QDC As Boolean = False,
                                                                      Optional ByVal Solo_Movimentati As Boolean = True
                                                                      ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_NumOperazioni_xStatistiche_DistinctPiva_New()"


        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable
        Dim user As String

        If Username <> "" Then
            user = " = '" & Username
        Else
            user = " <> '" & objParametri.PivaSuperUser
        End If

        Try


            strSql.Length = 0

            '33 è la lunghezza del filtro vuoto, dunque se è maggiore implica che ci sono dei filtri selezionati
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine(" CREATE TABLE #pive ( Piva varchar(25) ); ")
                strSql.AppendLine(" ALTER TABLE #pive ALTER COLUMN Piva varchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS ")
                strSql.AppendLine(ElencoInsertPive)
            End If

            strSql.AppendLine("WITH PraticheData as ( ")
            strSql.AppendLine(" SELECT convert(varchar, pratiche_stati_attuali.validita_inizio, 3) as Data_Stampa_QDC, pratiche.Piva  ")
            strSql.AppendLine(" From pratiche, pratiche_stati_attuali ")
            strSql.AppendLine(" WHERE ")
            strSql.AppendLine(" pratiche.pratica_cod = pratiche_stati_attuali.pratica_cod ")
            strSql.AppendLine(" And pratiche.Servizio_Cod = 2004 And pratiche_stati_attuali.Stato_Cod In (2003, 2005) ")
            strSql.AppendLine(" And pratiche_stati_attuali.validita_inizio <= " & Agro_SQL_SaveDate(DataFine) & "  ")
            strSql.AppendLine(" And pratiche_stati_attuali.validita_inizio >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            strSql.AppendLine(" ), ")

            strSql.AppendLine(" agr as ( ")
            strSql.AppendLine("	select i.piva  As Piva     ,  ")
            strSql.AppendLine("                isnull(b.operazioni,0) as Num_Operazioni_Campagna      ,  ")
            strSql.AppendLine("                isnull(a.operazioni,0) as Num_Operazioni_Magazzino       , ")
            strSql.AppendLine("                isnull(c.operazioni,0) as Num_PUA, ")

            If Dettagli_QDC Then

                strSql.Append(" b_trattamenti.operazioni As Num_Trattamenti, b_concimazioni.operazioni As Num_Concimazioni,  ")

                strSql.Append(" b_altre.operazioni As Num_Operazioni_Campagna_Altre  , pra.Data_Stampa_QDC  , ")

                strSql.AppendLine("                isnull(nonUtFert.operazioni,0) as Dichiarazione_NonUtilizzo_Fertilizzanti, ")
                strSql.AppendLine("                isnull(nonUtTrat.operazioni,0) as Dichiarazione_NonUtilizzo_Trattamenti, ")

            End If

            strSql.AppendLine("                isnull(d.operazioni,0) as Num_PianiConcimazione        ")
            strSql.AppendLine("                from      ( select imprese.piva  ")
            strSql.AppendLine("                            from imprese ")
            If Applica_VisibilitaUtente Then
                strSql.AppendLine("                              INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON imprese.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username = '" & objParametri.UtenteUsername & "'")
            End If
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine("                        where EXISTS (select * from #pive where imprese.PIVA = #pive.Piva)       ")
            End If
            strSql.AppendLine("		                    ) i  ")
            strSql.AppendLine("							LEFT JOIN  ( SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni             ")
            strSql.AppendLine("										 from agenda  ")
            strSql.AppendLine("										 INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  ")
            strSql.AppendLine("											And Agenda.Id_Agenda = Movimenti.Id_Agenda           ")
            strSql.AppendLine("										 INNER JOIN imprese On agenda.piva=imprese.piva           ")
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.AppendLine("										 WHERE Movimenti.data_movimento <=  " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.AppendLine("											 And   Movimenti.data_movimento >=  " & Agro_SQL_SaveDate(DataInizio) & " ")
                Case Else
                    strSql.Append("										WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("										AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            End Select
            strSql.AppendLine("											 And   Movimenti.cau_mov In ('7300')  AND Agenda.Username_Creazione " & user & "'")
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine("                                          And EXISTS(select * from #pive where imprese.PIVA = #pive.Piva) ")
            End If
            strSql.AppendLine("											 And   Agenda.Inviato >=0          group by agenda.piva, Imprese.rag_soc) a        ")
            strSql.AppendLine("									On i.piva= a.PIVA        ")
            strSql.AppendLine("							LEFT join ( Select DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) As operazioni  ")
            strSql.AppendLine("										from agenda  ")
            strSql.AppendLine("										INNER JOIN Movimenti On Agenda.PIVA = Movimenti.PIVA  ")
            strSql.AppendLine("											And Agenda.Id_Agenda = Movimenti.Id_Agenda           ")
            strSql.AppendLine("										INNER JOIN imprese On agenda.piva=imprese.piva           ")
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.AppendLine("										 WHERE Movimenti.data_movimento <=  " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.AppendLine("											 And   Movimenti.data_movimento >=  " & Agro_SQL_SaveDate(DataInizio) & " ")
                Case Else
                    strSql.Append("										WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                    strSql.Append("										AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
            End Select
            strSql.AppendLine("											And   Movimenti.cau_mov In ('2050','2100','2200','2300')           ")
            strSql.AppendLine("											AND   Agenda.Username_Creazione " & user & "' ")
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine("											AND  EXISTS (select * from #pive where imprese.PIVA = #pive.Piva) ")
            End If
            strSql.AppendLine("											AND   Agenda.Inviato >=0          group by agenda.piva, Imprese.rag_soc      ) b        ")
            strSql.AppendLine("									on i.piva=b.piva         ")


            If Dettagli_QDC Then

                strSql.AppendLine(" LEFT JOIN PraticheData pra on i.piva = pra.piva ")

                'OPERAZIONI CAMPAGNA - TRATTAMENTI
                strSql.Append("     LEFT join  ")
                strSql.Append("     (  ")
                strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
                strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.Append("         AND   Movimenti.cau_mov IN ('2050') ")

                strSql.Append("         AND   Agenda.Username_Creazione " & user & "' ")


                'If xFiltroAggiuntivo <> "" Then
                '    strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, False, objParametri))
                'End If
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        strSql.Append(" AND   Agenda.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        strSql.Append(" AND   Agenda.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select

                strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
                strSql.Append("     ) b_trattamenti  ")

                strSql.Append("     on i.piva=b_trattamenti.piva   ")

                'OPERAZIONI CAMPAGNA - CONCIMAZIONI
                strSql.Append("     LEFT join  ")
                strSql.Append("     (  ")
                strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
                strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.Append("         AND   Movimenti.cau_mov IN ('2300') ")

                strSql.Append("         AND   Agenda.Username_Creazione " & user & "' ")

                'If xFiltroAggiuntivo <> "" Then
                '        strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, False, objParametri))
                '    End If
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        strSql.Append(" AND   Agenda.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        strSql.Append(" AND   Agenda.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select

                strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
                strSql.Append("     ) b_concimazioni  ")

                strSql.Append("     on i.piva=b_concimazioni.piva   ")

                'OPERAZIONI CAMPAGNA - ALTRE
                strSql.Append("     LEFT join  ")
                strSql.Append("     (  ")
                strSql.Append("         SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni   ")
                strSql.Append("         from agenda INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                strSql.Append("         INNER JOIN imprese on agenda.piva=imprese.piva ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.Append("         AND   Movimenti.cau_mov IN ('2100','2200') ")
                strSql.Append("         AND   Agenda.Username_Creazione " & user & "' ")


                'If xFiltroAggiuntivo <> "" Then
                '        strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, False, objParametri))
                '    End If
                Select Case objParametri.FlagVisibilita
                    Case enumVisibilita.Visibilita_SoloNonCancellati
                        strSql.Append(" AND   Agenda.Inviato >=0 ")
                    Case enumVisibilita.Visibilita_SoloCancellati
                        strSql.Append(" AND   Agenda.Inviato =-1 ")
                    Case enumVisibilita.Visibilita_Tutti
                        '...................................
                    Case Else
                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                End Select

                strSql.Append("         group by agenda.piva, Imprese.rag_soc ")
                strSql.Append("     ) b_altre  ")

                strSql.Append("     on i.piva=b_altre.piva   ")


            End If

            strSql.AppendLine("							LEFT join ( SELECT DISTINCT PUA_Testata.PIVA,imprese.rag_soc,count(*) as operazioni             ")
            strSql.AppendLine("										from PUA_Testata  ")
            strSql.AppendLine("										INNER JOIN imprese on PUA_Testata.piva=imprese.piva           ")
            strSql.AppendLine("										WHERE PUA_Testata.Validita_Inizio <=  " & Agro_SQL_SaveDate(DataFine) & "   ")
            strSql.AppendLine("											AND   PUA_Testata.Validita_Fine >=  " & Agro_SQL_SaveDate(DataInizio) & "   ")
            strSql.AppendLine("											AND   PUA_Testata.username_creazione " & user & "' ")
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine("											AND  EXISTS (select * from #pive where imprese.PIVA = #pive.Piva) ")
            End If
            strSql.AppendLine("											AND   PUA_Testata.Inviato >=0           ")
            strSql.AppendLine("										group by PUA_Testata.piva, Imprese.rag_soc      ) c        ")
            strSql.AppendLine("									on i.piva=c.piva         ")
            strSql.AppendLine("							LEFT join ( SELECT DISTINCT PianoConcimazione_Dettagli.PC_Dettagli_PIVA  as piva , ")
            strSql.AppendLine("											imprese.rag_soc,count(*) as operazioni  ")
            strSql.AppendLine("										from PianoConcimazione_Dettagli  ")
            strSql.AppendLine("										INNER JOIN imprese on PianoConcimazione_Dettagli.PC_Dettagli_PIVA=imprese.piva           ")
            strSql.AppendLine("										WHERE PianoConcimazione_Dettagli.Validita_Inizio <= " & Agro_SQL_SaveDate(DataFine) & "    ")
            strSql.AppendLine("											AND   PianoConcimazione_Dettagli.Validita_Fine >=  " & Agro_SQL_SaveDate(DataInizio) & "  ")
            strSql.AppendLine("											AND   PianoConcimazione_Dettagli.username_creazione " & user & "' ")
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine("											AND  EXISTS (select * from #pive where imprese.PIVA = #pive.Piva) ")
            End If
            strSql.AppendLine("											AND   PianoConcimazione_Dettagli.Inviato >=0           ")
            strSql.AppendLine("										group by PianoConcimazione_Dettagli.PC_Dettagli_PIVA, Imprese.rag_soc      ) d       ")
            strSql.AppendLine("									on i.piva=d.piva ")

            If Dettagli_QDC Then

                strSql.AppendLine("                           LEFT JOIN  ( SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni      ")
                strSql.AppendLine("                                   from agenda  ")
                strSql.AppendLine("                                   INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  ")
                strSql.AppendLine("                                   And Agenda.Id_Agenda = Movimenti.Id_Agenda           ")
                strSql.AppendLine("                                   INNER JOIN imprese On agenda.piva=imprese.piva           ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.AppendLine("											AND   Agenda.Username_Creazione " & user & "' ")
                strSql.AppendLine("                                       And   Agenda.Lav_Cod = 165 ")
                If ElencoInsertPive.Length > 33 Then
                    strSql.AppendLine("											AND  EXISTS (select * from #pive where imprese.PIVA = #pive.Piva) ")
                End If
                strSql.AppendLine("                                       And   Agenda.Inviato >=0          group by agenda.piva, Imprese.rag_soc) nonUtFert     ")
                strSql.AppendLine("                           On i.piva= nonUtFert.PIVA        ")
                strSql.AppendLine("                           LEFT JOIN  ( SELECT DISTINCT Agenda.PIVA,imprese.rag_soc,count(*) as operazioni             ")
                strSql.AppendLine("                                   from agenda  ")
                strSql.AppendLine("                                   INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA  ")
                strSql.AppendLine("                                   And Agenda.Id_Agenda = Movimenti.Id_Agenda           ")
                strSql.AppendLine("                                   INNER JOIN imprese On agenda.piva=imprese.piva           ")
                Select Case FiltroData_Operazione1_Registrazione2
                    Case 1
                        strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                    Case Else
                        strSql.Append("         WHERE agenda.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "  ")
                        strSql.Append("         AND   agenda.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "  ")
                End Select
                strSql.AppendLine("											AND   Agenda.Username_Creazione " & user & "' ")
                strSql.AppendLine("                                       And   Agenda.Lav_Cod = 166")
                If ElencoInsertPive.Length > 33 Then
                    strSql.AppendLine("											AND  EXISTS (select * from #pive where imprese.PIVA = #pive.Piva) ")
                End If
                strSql.AppendLine("                                       And   Agenda.Inviato >=0          group by agenda.piva, Imprese.rag_soc) nonUtTrat        ")
                strSql.AppendLine("                           On i.piva= nonUtTrat.PIVA")

            End If
            strSql.AppendLine(" ), ")
            strSql.AppendLine("ag as ( ")
            strSql.AppendLine("	select agr.*, CASE WHEN ISNULL(i.partitaIvaReale, '') = '' THEN agr.PIVA ELSE i.partitaIvaReale END PivaReale   ")
            strSql.AppendLine("	from agr ")
            strSql.AppendLine("	LEFT JOIN Imprese i")
            strSql.AppendLine("	ON agr.Piva = i.PIVA ")
            If Solo_Movimentati Then
                strSql.AppendLine("	where not (agr.Num_Operazioni_Campagna= 0 And agr.Num_Operazioni_Magazzino = 0 And agr.Num_pua = 0 And agr.Num_PianiConcimazione = 0) ")
            End If
            strSql.AppendLine("), ")

            strSql.AppendLine("impreses As ( ")
            strSql.AppendLine("Select    ISNULL(( Select  top 1  Lista_Regioni.regione_des   ")
            strSql.AppendLine("            From Lista_Regioni    ")
            strSql.AppendLine("            right Join Lista_Province On Lista_Regioni.reg=Lista_Province.REG  ")
            strSql.AppendLine("            right Join Indirizzi On Indirizzi.pro_cod_istat = Lista_Province.PROV  ")
            strSql.AppendLine("            right join ImpresexIndirizzi On   Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo  ")
            strSql.AppendLine("            WHERE  i.PIVA = ImpresexIndirizzi.PIVA ), ' ') as Regione   ")
            strSql.AppendLine(" , istat.COMUNI_PROV as Provincia   ")
            strSql.AppendLine(" , istat.LOCALITA as Comune   ")
            strSql.AppendLine(" , i1.rag_soc + ' - ' + i1.piva as Referente   ")
            strSql.AppendLine(" , i.rag_soc as RagioneSociale,  ")
            strSql.AppendLine("  ic.val_cod as Cuaa , ")
            strSql.AppendLine("  i.Piva, ")
            strSql.AppendLine("  CASE WHEN ISNULL(i.partitaIvaReale, '') = '' THEN i.PIVA ELSE i.partitaIvaReale END PivaReale  ")
            strSql.AppendLine(" from imprese i  ")
            strSql.AppendLine("  join ImpresexIndirizzi on i.PIVA = ImpresexIndirizzi.PIVA ")
            strSql.AppendLine("  Join Indirizzi On Indirizzi.cod_indirizzo = ImpresexIndirizzi.cod_indirizzo  ")
            strSql.AppendLine("  join istat on istat.PROV=Indirizzi.pro_cod_istat And istat.COM=Indirizzi.com_cod_istat  ")
            strSql.AppendLine(" join Imprese_Codici ic on i.PIVA = ic.PIVA And id_cod = '1010' ")
            strSql.AppendLine("  Join GerarchiaImprese  on i.PIVA = GerarchiaImprese.figlio  ")
            strSql.AppendLine("  join imprese i1 On GerarchiaImprese.padre = i1.piva  ")
            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine("  where EXISTS (select * from #pive where i.PIVA = #pive.Piva)  ")
            End If
            strSql.AppendLine(" ) ")
            strSql.AppendLine(" ")
            strSql.AppendLine(" select a.* from ( ")
            strSql.AppendLine(" select imp.piva, CASE WHEN ISNULL(impreses.PivaReale, '') = '' THEN imp.PivaReale ELSE impreses.PivaReale END PivaReale , ")
            strSql.AppendLine(" imp.Num_Operazioni_Campagna, imp.Num_Operazioni_Magazzino, imp.Num_PUA, imp.Num_PianiConcimazione, ")
            If Dettagli_QDC Then
                strSql.AppendLine(" imp.Dichiarazione_NonUtilizzo_Fertilizzanti, imp.Dichiarazione_NonUtilizzo_Trattamenti, ")
            End If
            strSql.AppendLine(" impreses.Regione, impreses.Provincia, impreses.Comune, impreses.Referente, impreses.RagioneSociale, impreses.Cuaa, ROW_NUMBER() OVER (PARTITION BY imp.Piva order by imp.Piva) AS RowNumber   ")
            strSql.AppendLine(" from ag imp ")
            strSql.AppendLine(" left join impreses on imp.piva=impreses.piva) as a ")
            strSql.AppendLine(" Where a.RowNumber = 1 ")

            If xFiltroAggiuntivo <> "" Then
                strSql.AppendLine("AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo) & " ")
            End If

            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY a.RagioneSociale ")
            End If

            strSql.AppendLine(" ")

            If ElencoInsertPive.Length > 33 Then
                strSql.AppendLine(" Drop Table #pive ")
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

    Public Sub Leggi_xStatistiche_LeggiSpecieDestinazioniUso(ByVal SelectList As String, ByRef stb As System.Text.StringBuilder)

        stb.AppendLine("     Select SpecieVegetali.veg_cod, SpecieVegetali.veg_des, " & SelectList)
        stb.AppendLine("     From Reg_Impianti  ")
        stb.AppendLine("         inner Join cultivar  ")
        stb.AppendLine("             On cultivar.Cul_Cod = Reg_Impianti.CUL_COD  ")
        stb.AppendLine("  ")
        stb.AppendLine("      inner Join  SpecieVegetali  ")
        stb.AppendLine("             On SpecieVegetali.Veg_Cod = Cultivar.Veg_Cod  ")
        stb.AppendLine("  ")
        stb.AppendLine("  where Reg_Impianti.CUL_COD <> 0  ")
        stb.AppendLine("  ")
        stb.AppendLine("     union ")
        stb.AppendLine("  ")
        stb.AppendLine("     Select -Reg_Impianti_Codici.id_cod as veg_cod, Codici_Anagrafe.descrizione As veg_des, " & SelectList)
        stb.AppendLine("     From Reg_Impianti  ")
        stb.AppendLine("         inner Join Reg_Impianti_Codici  ")
        stb.AppendLine("             On Reg_Impianti.PIVA = Reg_Impianti_Codici.PIVA  ")
        stb.AppendLine("          And Reg_Impianti.SA_COD = Reg_Impianti_Codici.sa_cod  ")
        stb.AppendLine("          And Reg_Impianti.APPEZZA = Reg_Impianti_Codici.appezza  ")
        stb.AppendLine("          And Reg_Impianti.ID_REG = Reg_Impianti_Codici.Id_Reg  ")
        stb.AppendLine("          And Reg_Impianti_Codici.id_cod >= 3000 And Reg_Impianti_Codici.id_cod < 4000  ")
        stb.AppendLine("      inner Join Codici_Anagrafe  ")
        stb.AppendLine("             On Codici_Anagrafe.codice = Reg_Impianti_Codici.id_cod ")
        stb.AppendLine("  ")
        stb.AppendLine("  where Reg_Impianti.CUL_COD = 0  ")
        stb.AppendLine("  ")
        stb.AppendLine("     union ")
        stb.AppendLine("  ")
        stb.AppendLine("  ")
        stb.AppendLine("     Select 0 as veg_cod, 'Terreno Nudo' as veg_des, " & SelectList)
        stb.AppendLine("     From Reg_Impianti ")
        stb.AppendLine("     Where Not exists( ")
        stb.AppendLine("      select 1  ")
        stb.AppendLine("         From Reg_Impianti_Codici ic ")
        stb.AppendLine("      Where Reg_Impianti.PIVA = ic.PIVA ")
        stb.AppendLine("          And Reg_Impianti.SA_COD = ic.sa_cod  ")
        stb.AppendLine("          And Reg_Impianti.APPEZZA = ic.appezza  ")
        stb.AppendLine("          And Reg_Impianti.ID_REG = ic.Id_Reg  ")
        stb.AppendLine("          And ic.id_cod >= 3000 And ic.id_cod < 4000  ")
        stb.AppendLine("      ) ")
        stb.AppendLine("  And  Reg_Impianti.CUL_COD  = 0 ")

    End Sub

    Public Function Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        ByVal estraiQueryNoDati As Boolean,
                                                                        ByRef strQuery As String,
                                                                        ByVal Applica_VisibilitaUtente As Boolean,
                                                                        ByVal FiltroAggiuntivo_Imprese As String,
                                                                        ByVal JoinTabellaImprese As Boolean
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_NumOperazioni_xStatistiche_DistinctPiva()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0

            strSql.Append(" select  " & vbCrLf)

            strSql.Append("  ISNULL(Lista_Regioni.regione_des, '') as Regione  " & vbCrLf)

            strSql.Append(" , ISNULL(istat.COMUNI_PROV  , '') as Provincia  " & vbCrLf)

            strSql.Append(" , ISNULL(istat.LOCALITA  , '') as Comune " & vbCrLf)

            strSql.Append(" , imprese.rag_soc as Ragione_Sociale, imprese.Piva  " & vbCrLf)

            strSql.Append(" , ISNULL(CUAA.Val_cod, '') as Cuaa  " & vbCrLf)

            strSql.Append(" , ISNULL(GerarchiaImprese.Padre, ' ') as Referente    " & vbCrLf)

            strSql.Append(" , ISNULL(( Rappresentante_Legale.Cognome + ' ' + Rappresentante_Legale.Nome ), ' ') as Rappresentante_Legale    " & vbCrLf)

            strSql.Append(", convert(varchar,xPrimaRegistrazione.PrimaOperazione,103) as DataPrimaOperazione " & vbCrLf)
            strSql.Append(", convert(varchar,xPrimaRegistrazione.UltimaOperazione,103) as DataUltimaOperazione " & vbCrLf)

            strSql.Append(" , xSpecie.Veg_Des as Uso " & vbCrLf)
            strSql.Append(" , xSpecie.Veg_Cod " & vbCrLf)

            strSql.Append(" , xSpecie.conteggio as N_Operazioni " & vbCrLf)
            strSql.Append(" , xSpecieSuperficie.SupTrattata " & vbCrLf)
            strSql.Append(" , convert(varchar,xPrimaRegistrazione.PrimaRegistrazione,103) as Data_Prima_Registrazione " & vbCrLf)
            strSql.Append(" , xConteggioUtenti.conteggio as N_Utenti " & vbCrLf)

            strSql.Append(" From imprese " & vbCrLf)
            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append(" INNER JOIN @Table_Pive p ON imprese.Piva = p.Piva " & vbCrLf)
            End If
            strSql.Append(" CROSS APPLY (SELECT TOP 1 * FROM ImpresexIndirizzi WHERE Imprese.PIVA = ImpresexIndirizzi.PIVA) ii " & vbCrLf)
            strSql.Append(" INNER JOIN Indirizzi ON ii.cod_indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            strSql.Append(" LEFT JOIN ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat AND ISTAT.COM = Indirizzi.com_cod_istat " & vbCrLf)
            strSql.Append(" LEFT JOIN Lista_Province ON ISTAT.PROV = Lista_Province.PROV " & vbCrLf)
            strSql.Append(" LEFT JOIN Lista_Regioni ON Lista_Province.REG = Lista_Regioni.Reg " & vbCrLf)
            strSql.Append(" LEFT JOIN Imprese_Codici CUAA ON Imprese.Piva = CUAA.Piva AND CUAA.ID_Cod = 1010 " & vbCrLf)
            strSql.Append(" CROSS APPLY (SELECT TOP 1 * FROM GerarchiaImprese WHERE Imprese.PIVA = GerarchiaImprese.figlio) GerarchiaImprese " & vbCrLf)
            strSql.Append(" CROSS APPLY (SELECT  top 1  Contatti.* " & vbCrLf)
            strSql.Append(" 			  From Risorse_Umane " & vbCrLf)
            strSql.Append("             INNER Join Contatti On Risorse_Umane.cod_contatto = Contatti.Cod_Contatto And contatti.piva=imprese.piva " & vbCrLf)
            strSql.Append("             WHERE Risorse_Umane.Cod_Rapporto = -1 AND Risorse_Umane.Piva = Imprese.Piva ) Rappresentante_Legale " & vbCrLf)

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON imprese.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            strSql.Append(" inner Join ( " & vbCrLf)
            strSql.Append("     select distinct veg_cod, veg_des, piva , anno " & vbCrLf)
            strSql.Append("     from ( " & vbCrLf)

            Leggi_xStatistiche_LeggiSpecieDestinazioniUso(" Reg_Impianti.Piva ", strSql)

            strSql.Append("     ) coltura " & vbCrLf)

            strSql.Append("     inner Join ( " & vbCrLf)
            strSql.Append("     select distinct year(Movimenti.data_movimento) as anno " & vbCrLf)
            strSql.Append("     from Movimenti " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         WHERE Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         WHERE Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("     And   Movimenti.cau_mov IN ('2050','2100','2200','2300') " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")


            strSql.Append("     ) a on 1=1 " & vbCrLf)
            strSql.Append(" ) reg " & vbCrLf)
            strSql.Append(" On reg.PIVA = imprese.piva " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append(" 		select veg_cod, veg_des, piva, anno, count(*) as conteggio  " & vbCrLf)
            strSql.Append(" 		from ( " & vbCrLf)
            strSql.Append(" 			SELECT DISTINCT coltura.veg_cod, coltura.veg_des, Mov_Destinazioni.Piva, mov_destinazioni.id_agenda , year(Movimenti.data_movimento) as anno " & vbCrLf)
            strSql.Append("             From (  " & vbCrLf)

            Leggi_xStatistiche_LeggiSpecieDestinazioniUso(" Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg ", strSql)

            strSql.Append("             ) coltura " & vbCrLf)
            strSql.Append("             inner Join Mov_Destinazioni  " & vbCrLf)
            strSql.Append("                 On coltura.PIVA=Mov_Destinazioni.piva  " & vbCrLf)
            strSql.Append(" 				And coltura.sa_cod=Mov_Destinazioni.sa_cod  " & vbCrLf)
            strSql.Append(" 				And coltura.appezza=Mov_Destinazioni.appezza  " & vbCrLf)
            strSql.Append(" 				And coltura.id_reg=Mov_Destinazioni.id_destinazione " & vbCrLf)
            strSql.Append(" 			inner Join movimenti " & vbCrLf)
            strSql.Append("                 On movimenti.piva = mov_destinazioni.piva  " & vbCrLf)
            strSql.Append(" 				And movimenti.id_agenda = mov_destinazioni.id_agenda " & vbCrLf)
            strSql.Append(" 				And movimenti.id_mov = mov_destinazioni.Id_Mov " & vbCrLf)
            strSql.Append(" 			where Mov_Destinazioni.Tipo_Destinazione = 0  " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         AND Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         AND Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append(" 			And   Movimenti.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append(" 		) xSpec1 " & vbCrLf)
            strSql.Append(" 		group by xSpec1.Piva, xspec1.veg_cod, xSpec1.Veg_Des, xSpec1.anno " & vbCrLf)
            strSql.Append("    ) xSpecie " & vbCrLf)
            strSql.Append("    On xSpecie.piva = Imprese.piva  " & vbCrLf)
            strSql.Append("    And xSpecie.Veg_Des = reg.Veg_Des " & vbCrLf)
            strSql.Append("    and xSpecie.anno = reg.anno " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append("    		select xSpec2.Veg_Des, xSpec2.Piva, anno, sum(Sup_Imp) as SupTrattata " & vbCrLf)
            strSql.Append("            from ( " & vbCrLf)
            strSql.Append("                SELECT DISTINCT  " & vbCrLf)
            strSql.Append("    				  veg_des" & vbCrLf)
            strSql.Append("    				, mov_destinazioni.piva " & vbCrLf)
            strSql.Append("    				, mov_destinazioni.sa_cod " & vbCrLf)
            strSql.Append("    				, mov_destinazioni.appezza " & vbCrLf)
            strSql.Append("    				, mov_destinazioni.id_Destinazione as id_Reg " & vbCrLf)
            strSql.Append("    				, Coltura.Sup_Imp " & vbCrLf)
            strSql.Append("    				, year(Movimenti.data_movimento) as anno " & vbCrLf)
            strSql.Append("    			From (  " & vbCrLf)

            Leggi_xStatistiche_LeggiSpecieDestinazioniUso(" Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg, Reg_Impianti.Sup_Imp ", strSql)

            strSql.Append("                ) Coltura " & vbCrLf)

            strSql.Append("                inner Join Mov_Destinazioni " & vbCrLf)
            strSql.Append("                    On Coltura.PIVA=mov_destinazioni.piva  " & vbCrLf)
            strSql.Append("    				And Coltura.sa_cod=mov_destinazioni.sa_cod  " & vbCrLf)
            strSql.Append("    				And Coltura.appezza=mov_destinazioni.appezza  " & vbCrLf)
            strSql.Append("    				And Coltura.id_reg=mov_destinazioni.id_destinazione " & vbCrLf)
            strSql.Append("    			 inner Join movimenti " & vbCrLf)
            strSql.Append("                    On movimenti.piva = mov_destinazioni.piva  " & vbCrLf)
            strSql.Append("    				And movimenti.id_agenda = mov_destinazioni.id_agenda " & vbCrLf)
            strSql.Append("    				And movimenti.id_mov = mov_destinazioni.Id_Mov " & vbCrLf)
            strSql.Append("    			where Mov_Destinazioni.Tipo_Destinazione = 0  " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         AND Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         AND Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("    			And   Movimenti.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append("    		) xSpec2 " & vbCrLf)
            strSql.Append("    		group by xSpec2.Piva, xSpec2.Veg_Des , xSpec2.anno " & vbCrLf)
            strSql.Append("       ) xSpecieSuperficie " & vbCrLf)
            strSql.Append("       On xSpecieSuperficie.piva = Imprese.piva     " & vbCrLf)
            strSql.Append("       And xSpecieSuperficie.Veg_Des = reg.Veg_Des " & vbCrLf)
            strSql.Append("       and xSpecieSuperficie.anno = reg.anno " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append("    		Select agenda.piva " & vbCrLf)
            strSql.Append("    		       , year(Movimenti.data_movimento) as anno " & vbCrLf)
            strSql.Append("    		       , cast(min(agenda.data_modifica) as date) As PrimaRegistrazione " & vbCrLf)
            strSql.Append("    		       , cast(min(Movimenti.data_movimento) as date) As PrimaOperazione  " & vbCrLf)
            strSql.Append("    		       , cast(max(Movimenti.data_movimento)as date) As UltimaOperazione " & vbCrLf)
            strSql.Append("    		       , veg_des " & vbCrLf)
            strSql.Append("            From agenda  " & vbCrLf)
            strSql.Append("            inner Join movimenti " & vbCrLf)
            strSql.Append("                 On movimenti.piva = agenda.piva  " & vbCrLf)
            strSql.Append("    				And movimenti.id_agenda = agenda.id_agenda	 " & vbCrLf)
            strSql.Append("            inner Join mov_destinazioni " & vbCrLf)
            strSql.Append("                 On movimenti.piva = mov_destinazioni.piva   " & vbCrLf)
            strSql.Append("                 And movimenti.id_agenda = mov_destinazioni.id_agenda  " & vbCrLf)
            strSql.Append("                 And movimenti.id_mov = mov_destinazioni.Id_Mov  " & vbCrLf)

            strSql.Append("            inner Join (  " & vbCrLf)

            Leggi_xStatistiche_LeggiSpecieDestinazioniUso(" Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg ", strSql)

            strSql.Append("            ) Coltura  " & vbCrLf)

            strSql.Append("                 On Coltura.PIVA=mov_destinazioni.piva   " & vbCrLf)
            strSql.Append("                 And Coltura.sa_cod=mov_destinazioni.sa_cod   " & vbCrLf)
            strSql.Append("                 And Coltura.appezza=mov_destinazioni.appezza   " & vbCrLf)
            strSql.Append("                 And Coltura.id_reg=mov_destinazioni.id_destinazione " & vbCrLf)

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   Movimenti.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            strSql.Append("             And   Mov_Destinazioni.Tipo_Destinazione = 0      " & vbCrLf)
            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append("    		group by agenda.piva  , year(Movimenti.data_movimento) , Veg_Des  " & vbCrLf)
            strSql.Append("    	)  xPrimaRegistrazione " & vbCrLf)
            strSql.Append("    	On xPrimaRegistrazione.PIVA = Imprese.piva  " & vbCrLf)
            strSql.Append("    	and xPrimaRegistrazione.anno=reg.anno  " & vbCrLf)
            strSql.Append("    	And xPrimaRegistrazione.Veg_Des = reg.Veg_Des  " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append("    		select piva, anno, veg_des, count(*) as conteggio  " & vbCrLf)
            strSql.Append("    		from ( " & vbCrLf)
            strSql.Append("    			select distinct agenda.piva, year(Movimenti.data_movimento) as anno, agenda.Username_Modifica, veg_des " & vbCrLf)
            strSql.Append("            From agenda  " & vbCrLf)
            strSql.Append("            inner Join movimenti " & vbCrLf)
            strSql.Append("                 On movimenti.piva = agenda.piva  " & vbCrLf)
            strSql.Append("    				And movimenti.id_agenda = agenda.id_agenda	 " & vbCrLf)
            strSql.Append("            inner Join mov_destinazioni " & vbCrLf)
            strSql.Append("                 On movimenti.piva = mov_destinazioni.piva   " & vbCrLf)
            strSql.Append("                 And movimenti.id_agenda = mov_destinazioni.id_agenda  " & vbCrLf)
            strSql.Append("                 And movimenti.id_mov = mov_destinazioni.Id_Mov  " & vbCrLf)
            strSql.Append("            inner Join (  " & vbCrLf)

            Leggi_xStatistiche_LeggiSpecieDestinazioniUso(" Reg_Impianti.piva, Reg_Impianti.sa_cod, Reg_Impianti.appezza, Reg_Impianti.id_reg ", strSql)

            strSql.Append("            ) Coltura  " & vbCrLf)

            strSql.Append("                 On Coltura.PIVA=mov_destinazioni.piva   " & vbCrLf)
            strSql.Append("                 And Coltura.sa_cod=mov_destinazioni.sa_cod   " & vbCrLf)
            strSql.Append("                 And Coltura.appezza=mov_destinazioni.appezza   " & vbCrLf)
            strSql.Append("                 And Coltura.id_reg=mov_destinazioni.id_destinazione " & vbCrLf)

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   Movimenti.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            strSql.Append("             And   Mov_Destinazioni.Tipo_Destinazione = 0      " & vbCrLf)
            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append("    		) xUtenti1 " & vbCrLf)
            strSql.Append("    		group by piva ,anno, veg_des " & vbCrLf)
            strSql.Append("    	) xConteggioUtenti " & vbCrLf)
            strSql.Append("    	On xConteggioUtenti.piva = imprese.piva  " & vbCrLf)
            strSql.Append("    	and xConteggioUtenti.anno = reg.anno  " & vbCrLf)
            strSql.Append("    	And xConteggioUtenti.Veg_Des = reg.Veg_Des  " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.Append(" UNION  " & vbCrLf)

            strSql.Append(" select  " & vbCrLf)


            strSql.Append("  ISNULL(regione_des, '') as Regione  " & vbCrLf)
            strSql.Append("  , ISNULL(COMUNI_PROV  , '') as Provincia  " & vbCrLf)
            strSql.Append("  , ISNULL(LOCALITA  , '') as Comune  " & vbCrLf)

            strSql.Append(" , imprese.rag_soc as Ragione_Sociale, imprese.Piva  " & vbCrLf)

            strSql.Append("   , ISNULL(CUAA, '') as Cuaa  " & vbCrLf)

            strSql.Append("  , ISNULL(Padre, ' ') as Referente    " & vbCrLf)
            strSql.Append("  , ISNULL(( Cognome + ' ' + Nome ), ' ') as Rappresentante_Legale    " & vbCrLf)

            strSql.Append(", convert(varchar,xPrimaRegistrazione.PrimaOperazione,103) as DataPrimaOperazione " & vbCrLf)
            strSql.Append(", convert(varchar,xPrimaRegistrazione.UltimaOperazione,103) as DataUltimaOperazione " & vbCrLf)

            strSql.Append(" , ' Movimenti Magazzino' as Uso " & vbCrLf)
            strSql.Append(" , -1 as Veg_Cod " & vbCrLf)

            strSql.Append(" , xMagazzino.conteggio as N_Operazioni " & vbCrLf)
            strSql.Append(" , 0 as SupTrattata " & vbCrLf)
            strSql.Append(" , convert(varchar,xPrimaRegistrazione.PrimaRegistrazione,103) as Data_Prima_Registrazione " & vbCrLf)
            strSql.Append(" , xConteggioUtenti.conteggio as N_Utenti " & vbCrLf)

            strSql.Append(" From ( " & vbCrLf)
            strSql.Append("    select imprese.piva , imprese.rag_soc, anno, Lista_Regioni.regione_Des, istat.COMUNI_PROV, istat.LOCALITA, CUAA.val_cod as CUAA, GerarchiaImprese.Padre, Rappresentante_Legale.Nome, Rappresentante_Legale.Cognome " & vbCrLf)
            strSql.Append("    From imprese  " & vbCrLf)

            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append(" INNER JOIN @Table_Pive p ON imprese.Piva = p.Piva " & vbCrLf)
            End If

            strSql.Append(" CROSS APPLY (SELECT TOP 1 * FROM ImpresexIndirizzi WHERE Imprese.PIVA = ImpresexIndirizzi.PIVA) ii " & vbCrLf)
            strSql.Append(" INNER JOIN Indirizzi ON ii.cod_indirizzo = Indirizzi.cod_indirizzo " & vbCrLf)
            strSql.Append(" LEFT JOIN ISTAT ON ISTAT.PROV = Indirizzi.pro_cod_istat AND ISTAT.COM = Indirizzi.com_cod_istat " & vbCrLf)
            strSql.Append(" LEFT JOIN Lista_Province ON ISTAT.PROV = Lista_Province.PROV " & vbCrLf)
            strSql.Append(" LEFT JOIN Lista_Regioni ON Lista_Province.REG = Lista_Regioni.Reg " & vbCrLf)
            strSql.Append(" LEFT JOIN Imprese_Codici CUAA ON Imprese.Piva = CUAA.Piva AND CUAA.ID_Cod = 1010 " & vbCrLf)
            strSql.Append(" CROSS APPLY (SELECT TOP 1 * FROM GerarchiaImprese WHERE Imprese.PIVA = GerarchiaImprese.figlio) GerarchiaImprese " & vbCrLf)
            strSql.Append(" CROSS APPLY (SELECT  top 1  Contatti.* " & vbCrLf)
            strSql.Append(" 			  From Risorse_Umane " & vbCrLf)
            strSql.Append("             INNER Join Contatti On Risorse_Umane.cod_contatto = Contatti.Cod_Contatto And contatti.piva=imprese.piva " & vbCrLf)
            strSql.Append("             WHERE Risorse_Umane.Cod_Rapporto = -1 AND Risorse_Umane.Piva = Imprese.Piva ) Rappresentante_Legale " & vbCrLf)


            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON imprese.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append("       select distinct year(Movimenti.data_movimento) as anno " & vbCrLf)
            strSql.Append("       From Movimenti " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("       And Movimenti.cau_mov In ('2050','2100','2200','2300')  " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            strSql.Append("       ) a On 1=1  " & vbCrLf)
            strSql.Append("    ) imprese " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append(" 		select agenda.piva, year(Movimenti.data_movimento) as anno, count(*) as conteggio  " & vbCrLf)
            strSql.Append(" 		from Agenda " & vbCrLf)
            strSql.Append(" 		INNER JOIN Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append(" 			And   Movimenti.cau_mov IN ('7300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append("    		group by agenda.piva, year(Movimenti.data_movimento)  " & vbCrLf)
            strSql.Append("    ) xMagazzino " & vbCrLf)
            strSql.Append("    On xMagazzino.piva = Imprese.piva  " & vbCrLf)
            strSql.Append("    and xMagazzino.anno = Imprese.anno  " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append("    		Select agenda.piva " & vbCrLf)
            strSql.Append("    		       , year(Movimenti.data_movimento) as anno " & vbCrLf)
            strSql.Append("    		       , cast(min(agenda.data_modifica) as date) As PrimaRegistrazione " & vbCrLf)
            strSql.Append("    		       , cast(min(Movimenti.data_movimento) as date) As PrimaOperazione  " & vbCrLf)
            strSql.Append("    		       , cast(max(Movimenti.data_movimento) as date) As UltimaOperazione " & vbCrLf)
            strSql.Append("            From agenda  " & vbCrLf)
            strSql.Append("            inner Join movimenti " & vbCrLf)
            strSql.Append("                    On movimenti.piva = agenda.piva  " & vbCrLf)
            strSql.Append("    				And movimenti.id_agenda = agenda.id_agenda	 " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append(" 			And   Movimenti.cau_mov IN ('7300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append("    		group by agenda.piva, year(Movimenti.data_movimento)  " & vbCrLf)
            strSql.Append("    	)  xPrimaRegistrazione " & vbCrLf)
            strSql.Append("    	On xPrimaRegistrazione.PIVA = Imprese.piva  " & vbCrLf)
            strSql.Append("    	and xPrimaRegistrazione.anno = Imprese.anno  " & vbCrLf)

            strSql.Append("    inner Join ( " & vbCrLf)
            strSql.Append("    		select piva, anno, count(*) as conteggio  " & vbCrLf)
            strSql.Append("    		from ( " & vbCrLf)
            strSql.Append("    			select distinct agenda.piva, year(Movimenti.data_movimento) as anno, agenda.Username_Modifica " & vbCrLf)
            strSql.Append("                From agenda  " & vbCrLf)
            strSql.Append("            inner Join movimenti " & vbCrLf)
            strSql.Append("                    On movimenti.piva = agenda.piva  " & vbCrLf)
            strSql.Append("    				And movimenti.id_agenda = agenda.id_agenda			 " & vbCrLf)
            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where Movimenti.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where Movimenti.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   Movimenti.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append(" 			And   Movimenti.cau_mov IN ('7300')      " & vbCrLf)
            strSql.Append("         AND   Movimenti.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   Movimenti.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If
            strSql.Append("    		) xUtenti1 " & vbCrLf)
            strSql.Append("    		group by piva, anno " & vbCrLf)
            strSql.Append("    	) xConteggioUtenti " & vbCrLf)
            strSql.Append("    	On xConteggioUtenti.piva = imprese.piva  " & vbCrLf)
            strSql.Append("    	and xConteggioUtenti.anno = imprese.anno  " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY imprese.rag_soc, DataPrimaOperazione desc, uso asc ")
            End If

            '--------------------------------------------------------------------------
            If estraiQueryNoDati = True Then
                strQuery = strSql.ToString
            Else
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Function Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod_ConUtenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xFiltroAggiuntivo1 As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri_Server As AgronicaCoreParametri,
                                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                        ByVal Applica_VisibilitaUtente As Boolean,
                                                                        ByVal FiltroAggiuntivo_Imprese As String
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_NumOperazioni_xStatistiche_DistinctPiva_ConUtenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim str_Esporta_Dettagli As String = ""
            Dim str_OpCamp As String = ""
            Dim str_OpMag As String = ""
            Dim str_TabellaPive As String = ""

            Leggi_NumOperazioni_xStatistiche_DistinctPivaVegCod(FiltroData_Operazione1_Registrazione2, DataInizio, DataFine, Username,
                                                               xFiltroAggiuntivo, "", objParametri_Server, True, str_Esporta_Dettagli, Applica_VisibilitaUtente, FiltroAggiuntivo_Imprese, True)

            Leggi_xStatistiche_OperazioniCampagnaSpecie_Utenti(FiltroData_Operazione1_Registrazione2, DataInizio, DataFine, Username,
                                                               xFiltroAggiuntivo1, "", objParametri_Server, objParametri_Utenti, True, str_OpCamp, Applica_VisibilitaUtente, FiltroAggiuntivo_Imprese, True)

            Leggi_xStatistiche_OperazioniMagazzino_Utenti(FiltroData_Operazione1_Registrazione2, DataInizio, DataFine, Username, True,
                                                               xFiltroAggiuntivo1, "", objParametri_Server, objParametri_Utenti, True, str_OpMag, Applica_VisibilitaUtente, FiltroAggiuntivo_Imprese, True)

            If FiltroAggiuntivo_Imprese <> "" Then
                Creazione_TabellaPive(FiltroAggiuntivo_Imprese, str_TabellaPive)
            End If

            strSql.Length = 0

            strSql.AppendLine(" SET NOCOUNT ON " & vbCrLf)

            If FiltroAggiuntivo_Imprese <> "" Then
                strSql.AppendLine(str_TabellaPive)
            End If

            strSql.AppendLine(" DECLARE @Table_OpCamp TABLE (piva varchar(100), id_agenda INT, Lav_Cod INT, des_lib varchar(MAX), data_movimento datetime, Veg_Cod INT, Username_Creazione varchar(MAX), utente varchar(MAX)) ")
            strSql.AppendLine(" INSERT INTO @Table_OpCamp ")

            strSql.AppendLine(str_OpCamp & vbCrLf)

            strSql.AppendLine(" DECLARE @Table_OpMag TABLE (piva varchar(100), id_agenda INT, Lav_Cod INT, des_lib varchar(MAX), data_movimento datetime, Username_Creazione varchar(MAX), utente varchar(MAX)) ")
            strSql.AppendLine(" INSERT INTO @Table_OpMag ")

            strSql.AppendLine(str_OpMag & vbCrLf)

            strSql.AppendLine(" DECLARE @Table_Dett TABLE (Regione varchar(MAX), Provincia varchar(MAX), Comune varchar(MAX), Ragione_Sociale varchar(MAX), Piva varchar(100), Cuaa varchar(MAX), Referente varchar(MAX) ")
            strSql.AppendLine(" , Rappresentante_Legale varchar(MAX), DataPrimaOperazione datetime, DataUltimaOperazione datetime, Uso varchar(MAX), Veg_Cod INT, N_Operazioni INT, SupTrattata float ")
            strSql.AppendLine(" , Data_Prima_Registrazione datetime, N_Utenti INT, Utenti varchar(MAX) DEFAULT '') ")
            strSql.AppendLine(" INSERT INTO @Table_Dett (Regione, Provincia, Comune, Ragione_Sociale, Piva, Cuaa, Referente, Rappresentante_Legale, DataPrimaOperazione, DataUltimaOperazione, Uso, Veg_Cod, N_Operazioni, SupTrattata, Data_Prima_Registrazione, N_Utenti) ")

            strSql.AppendLine(str_Esporta_Dettagli & vbCrLf)

            strSql.AppendLine(" UPDATE td ")
            strSql.AppendLine(" SET Utenti=( ")
            strSql.AppendLine("     STUFF(( ")
            strSql.AppendLine("     SELECT DISTINCT ',' + m.utente FROM @Table_OpMag m ")
            strSql.AppendLine("     WHERE m.piva=td.piva AND m.data_movimento <= td.dataultimaoperazione AND m.data_movimento >= td.DataPrimaOperazione ")
            strSql.AppendLine("     FOR XML PATH('') ")
            strSql.AppendLine("     ), 1, 1, '' ) ")
            strSql.AppendLine(" ) ")
            strSql.AppendLine(" FROM @Table_Dett td ")
            strSql.AppendLine(" WHERE Veg_Cod=-1 " & vbCrLf)

            strSql.AppendLine(" UPDATE td ")
            strSql.AppendLine(" SET Utenti=( ")
            strSql.AppendLine("     STUFF(( ")
            strSql.AppendLine("     SELECT DISTINCT ',' + c.utente FROM @Table_OpCamp c ")
            strSql.AppendLine("     WHERE c.piva=td.piva AND c.veg_cod=td.veg_cod AND c.data_movimento <= td.dataultimaoperazione AND c.data_movimento >= td.DataPrimaOperazione ")
            strSql.AppendLine("     FOR XML PATH('') ")
            strSql.AppendLine("     ), 1, 1, '' ) ")
            strSql.AppendLine(" ) ")
            strSql.AppendLine(" FROM @Table_Dett td ")
            strSql.AppendLine(" WHERE Veg_Cod<>-1 " & vbCrLf)

            strSql.AppendLine(" SELECT * FROM @Table_Dett ")
            strSql.AppendLine(" ORDER BY Ragione_Sociale, DataPrimaOperazione desc, Uso asc ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Creazione_TabellaPive(ByVal FiltroAggiuntivo_Imprese As String, ByRef str_TabellaPive As String)
        str_TabellaPive = " DECLARE @Table_Pive TABLE (piva varchar(25)) " & vbCrLf
        str_TabellaPive &= " INSERT INTO @Table_Pive " & vbCrLf
        str_TabellaPive &= " SELECT Piva FROM Imprese WHERE Piva IN ( " + Agro_SQL_Save_Clausola_IN(FiltroAggiuntivo_Imprese, True) + " ) " & vbCrLf
    End Function

    Public Function Leggi_xStatistiche_OperazioniCampagna_Utenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                           ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                  Optional ByVal Applica_VisibilitaUtente As Boolean = False
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_OperazioniCampagna_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            'SPECIE VEGETALI
            strSql.Append(" select  distinct " & vbCrLf)

            'strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, CONVERT(Date,m.Data_Movimento,120) as data_movimento  " & vbCrLf)
            'strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)
            strSql.Append(" a.piva, a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva  ")
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

    Public Function Leggi_xStatistiche_OperazioniCampagna_Utenti_New(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal insertString As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                        Optional ByVal Applica_VisibilitaUtente As Boolean = False
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_OperazioniCampagna_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            '33 è la lunghezza del filtro vuoto, dunque se è maggiore implica che ci sono dei filtri selezionati
            If insertString.Length > 33 Then
                strSql.AppendLine(" CREATE TABLE #pive ( Piva varchar(25) ); ")
                strSql.AppendLine(" ALTER TABLE #pive ALTER COLUMN Piva varchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS ")
                strSql.AppendLine(insertString)
            End If

            'SPECIE VEGETALI
            strSql.Append(" select  distinct " & vbCrLf)

            'strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, CONVERT(Date,m.Data_Movimento,120) as data_movimento  " & vbCrLf)
            'strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)
            strSql.Append(" a.piva, a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If insertString.Length > 33 Then
                strSql.AppendLine(" AND EXISTS (select * from #pive where a.PIVA = #pive.Piva) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva  ")
            End If

            If insertString.Length > 33 Then
                strSql.AppendLine(" Drop Table #pive ")
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


    Public Function Leggi_xStatistiche_OperazioniCampagnaSpecie_Utenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                        ByVal estraiQueryNoDati As Boolean,
                                                                        ByRef strQuery As String,
                                                                        ByVal Applica_VisibilitaUtente As Boolean,
                                                                        ByVal FiltroAggiuntivo_Imprese As String,
                                                                        ByVal JoinTabellaImprese As Boolean
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_OperazioniCampagna_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            'SPECIE VEGETALI
            strSql.Append(" select  " & vbCrLf)

            strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, CONVERT(Date,m.Data_Movimento,120) as data_movimento  " & vbCrLf)
            'strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, m.Data_Movimento  " & vbCrLf)
            strSql.Append(" , c.Veg_Cod  " & vbCrLf)
            strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append("     INNER JOIN @Table_Pive p ON a.Piva = p.Piva " & vbCrLf)
            End If

            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner Join Mov_Destinazioni md ON m.piva=md.piva and m.id_agenda=md.id_agenda  and m.Id_Mov=md.Id_Mov " & vbCrLf)
            strSql.Append("     inner join Reg_Impianti r on r.piva=md.piva and r.sa_cod=md.Sa_Cod and r.APPEZZA=md.Appezza and r.ID_REG=md.Id_Destinazione  And md.Tipo_Destinazione = 0" & vbCrLf)
            strSql.Append("     inner join cultivar c on r.CUL_COD=c.Cul_Cod " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.Append(" UNION  " & vbCrLf)

            ' TERRENI NUDI 
            strSql.Append(" select  " & vbCrLf)


            strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, CONVERT(Date,m.Data_Movimento,120) as data_movimento  " & vbCrLf)
            'strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, m.Data_Movimento  " & vbCrLf)
            strSql.Append(" ,0 as Veg_Cod  " & vbCrLf)
            strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append("     INNER JOIN @Table_Pive p ON a.Piva = p.Piva " & vbCrLf)
            End If

            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner Join Mov_Destinazioni md ON m.piva=md.piva and m.id_agenda=md.id_agenda  and m.Id_Mov=md.Id_Mov " & vbCrLf)
            strSql.Append("     inner join Reg_Impianti r on r.piva=md.piva and r.sa_cod=md.Sa_Cod and r.APPEZZA=md.Appezza and r.ID_REG=md.Id_Destinazione  And md.Tipo_Destinazione = 0" & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            strSql.Append(" AND r.cul_cod=0 " & vbCrLf)


            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            strSql.Append(" UNION  " & vbCrLf)

            'DESTINAZIONI USO
            strSql.Append(" select  " & vbCrLf)

            strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, CONVERT(Date,m.Data_Movimento,120) as data_movimento  " & vbCrLf)
            'strSql.Append(" a.piva, a.id_agenda, a.Lav_Cod, a.des_lib, m.Data_Movimento  " & vbCrLf)
            strSql.Append(" , -rc.id_cod as Veg_Cod " & vbCrLf)
            strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append("     INNER JOIN @Table_Pive p ON a.Piva = p.Piva " & vbCrLf)
            End If

            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner Join Mov_Destinazioni md ON m.piva=md.piva and m.id_agenda=md.id_agenda  and m.Id_Mov=md.Id_Mov " & vbCrLf)
            strSql.Append("     inner join Reg_Impianti r on r.piva=md.piva and r.sa_cod=md.Sa_Cod and r.APPEZZA=md.Appezza and r.ID_REG=md.Id_Destinazione  And md.Tipo_Destinazione = 0" & vbCrLf)
            strSql.Append("     inner join Reg_Impianti_Codici rc on r.piva=rc.piva and r.sa_cod=rc.Sa_Cod and r.APPEZZA=rc.Appezza and r.ID_REG=rc.Id_Reg and Progetto_Cod=0 " & vbCrLf)
            strSql.Append("         and rc.id_cod >= 3000 And rc.id_cod < 4000 " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('2050','2100','2200','2300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            strSql.Append(" AND r.cul_cod=0 " & vbCrLf)

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva , veg_cod ")
            End If

            '--------------------------------------------------------------------------
            If estraiQueryNoDati = True Then
                strQuery = strSql.ToString
            Else
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_xStatistiche_OperazioniMagazzino_Utenti(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal DistinctIdAgenda As Boolean,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                        ByVal estraiQueryNoDati As Boolean,
                                                                        ByRef strQuery As String,
                                                                        ByVal Applica_VisibilitaUtente As Boolean,
                                                                        ByVal FiltroAggiuntivo_Imprese As String,
                                                                        ByVal JoinTabellaImprese As Boolean
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_OperazioniMagazzino_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            strSql.Append(" select a.piva " & vbCrLf)
            If DistinctIdAgenda = True Then
                strSql.Append(" , a.id_agenda, a.Lav_Cod, a.des_lib, cast(m.Data_Movimento as date) as data_movimento  " & vbCrLf)
            End If
            strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append("     INNER JOIN @Table_Pive p ON a.Piva = p.Piva " & vbCrLf)
            End If
            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner Join Mov_Destinazioni md ON m.piva=md.piva and m.id_agenda=md.id_agenda  and m.Id_Mov=md.Id_Mov And md.Tipo_Destinazione = 20 " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('7300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva  ")
            End If

            '--------------------------------------------------------------------------
            If estraiQueryNoDati = True Then
                strQuery = strSql.ToString
            Else
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_xStatistiche_OperazioniMagazzino_Utenti_New(ByVal FiltroData_Operazione1_Registrazione2 As Integer,
                                                                        ByVal DataInizio As Date,
                                                                        ByVal DataFine As Date,
                                                                        ByVal Username As String,
                                                                        ByVal DistinctIdAgenda As Boolean,
                                                                        ByVal insertString As String,
                                                                        ByVal xFiltroAggiuntivo As String,
                                                                        ByVal xOrderBy As String,
                                                                        ByRef objParametri As AgronicaCoreParametri,
                                                                        ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                                        ByVal estraiQueryNoDati As Boolean,
                                                                        ByRef strQuery As String,
                                                                        ByVal Applica_VisibilitaUtente As Boolean,
                                                                        ByVal FiltroAggiuntivo_Imprese As String,
                                                                        ByVal JoinTabellaImprese As Boolean
                                                                        ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_xStatistiche_OperazioniMagazzino_Utenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)


        Try

            strSql.Length = 0

            '33 è la lunghezza del filtro vuoto, dunque se è maggiore implica che ci sono dei filtri selezionati
            If insertString.Length > 33 Then
                strSql.AppendLine(" CREATE TABLE #pive ( Piva varchar(25) ); ")
                strSql.AppendLine(" ALTER TABLE #pive ALTER COLUMN Piva varchar(25) COLLATE SQL_Latin1_General_CP850_CI_AS ")
                strSql.AppendLine(insertString)
            End If

            strSql.Append(" select DISTINCT a.piva " & vbCrLf)
            If DistinctIdAgenda = True Then
                strSql.Append(" , a.id_agenda, a.Lav_Cod, a.des_lib, cast(m.Data_Movimento as date) as data_movimento  " & vbCrLf)
            End If
            strSql.Append(" , a.Username_Creazione, ud.nome + ' ' + ud.Cognome as utente " & vbCrLf)

            strSql.Append(" From agenda a " & vbCrLf)
            If JoinTabellaImprese And FiltroAggiuntivo_Imprese <> "" Then
                strSql.Append("     INNER JOIN @Table_Pive p ON a.Piva = p.Piva " & vbCrLf)
            End If
            strSql.Append("     inner Join Movimenti m on a.piva=m.piva and a.Id_Agenda=m.Id_Agenda " & vbCrLf)
            strSql.Append("     inner Join Mov_Destinazioni md ON m.piva=md.piva and m.id_agenda=md.id_agenda  and m.Id_Mov=md.Id_Mov And md.Tipo_Destinazione = 20 " & vbCrLf)
            strSql.Append("     inner join " & NomeDB_Utenti & ".dbo.Utenti_Dettagli ud on ud.CodFisc=a.Username_Creazione ")

            If Applica_VisibilitaUtente = True Then
                strSql.Append("     INNER JOIN Utenti_Visibilita_Appoggio (NOLOCK) ON a.PIVA = Utenti_Visibilita_Appoggio.PIVA AND Entita_Cod = 1 and Utenti_Visibilita_Appoggio.username ='" & objParametri.UtenteUsername & "'")
            End If

            Select Case FiltroData_Operazione1_Registrazione2
                Case 1
                    strSql.Append("         where m.data_movimento <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_movimento >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
                Case Else
                    strSql.Append("         where m.data_creazione <= " & Agro_SQL_SaveDate(DataFine) & "   " & vbCrLf)
                    strSql.Append("         AND   m.data_creazione >= " & Agro_SQL_SaveDate(DataInizio) & "   " & vbCrLf)
            End Select
            strSql.Append("             And   m.cau_mov IN ('7300')      " & vbCrLf)
            strSql.Append("         AND   m.username_creazione <> '" & objParametri.PivaSuperUser & "'")

            If Username <> "" Then
                strSql.Append("     AND   m.Username_Creazione='" & Agro_SQL_SaveText(Username) & "'")
            End If

            If insertString.Length > 33 Then
                strSql.AppendLine(" AND EXISTS (select * from #pive where a.PIVA = #pive.Piva) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append("     AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY piva  ")
            End If

            If insertString.Length > 33 Then
                strSql.AppendLine(" Drop Table #pive  ")
            End If

            '--------------------------------------------------------------------------
            If estraiQueryNoDati = True Then
                strQuery = strSql.ToString
            Else
                dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function


    Public Sub PivaSacod_From_IdAgenda(ByVal Id_Agenda As String, ByRef Piva As String, ByRef Sa_Cod As Integer, ByRef objParametri_Server As AgronicaCoreParametri)
        Dim dt As DataTable = Leggi("", 0, Id_Agenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
        If dt.Rows.Count = 1 Then
            Piva = dt.Rows(0).Item("Piva")
            Sa_Cod = dt.Rows(0).Item("Sa_Cod")
        Else
            Throw New Exception("dt.Rows.Count <> 1")
        End If
    End Sub

    Public Sub LineaCodPreparazioneCod_From_IdAgenda(ByVal piva As String,
                                                     ByVal idAgenda As Integer,
                                                     ByRef lineaCod As Integer,
                                                     ByRef preparazioneCod As Integer,
                                                     ByRef objParametri As AgronicaCoreParametri)

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.LineaCodPreparazioneCod_From_IdAgenda()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable

        Try
            dt = Leggi(piva, 0, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count = 1 Then
                lineaCod = dt.Rows(0).Item("LINEA_COD")
                preparazioneCod = dt.Rows(0).Item("PREPARAZIONE_COD")
            Else
                lineaCod = 0
                preparazioneCod = 0
                Throw New Exception("dt.Rows.Count <> 1")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function Leggi_UltimaModifica_DataMovimento(ByVal piva As String,
                                                       ByVal idAgenda As Integer,
                                                       ByVal xFiltroAggiuntivo As String,
                                                       ByRef usernameModifica As String,
                                                       ByRef dataMovimento As Date,
                                                       ByRef objParametri As AgronicaCoreParametri
                                                       ) As DateTime

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_R.Leggi_UltimaModifica()"
        Dim messaggioErrore As String = ""

        Dim dt As DataTable
        Dim dataModifica As DateTime = AGRODATAINIZIO

        Try
            dt = Leggi(piva, 0, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count = 1 Then
                dataModifica = dt.Rows(0).Item("Data_Modifica")
                usernameModifica = dt.Rows(0).Item("Username_Modifica")
                dataMovimento = dt.Rows(0).Item("Validita_Inizio")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dataModifica

    End Function

    Public Sub Leggi_DatiPrecedenti_Agenda(ByVal piva As String,
                                           ByVal idAgenda As Integer,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef usernameModifica As String,
                                           ByRef dataMovimento As Date,
                                           ByRef dataModifica As DateTime,
                                           ByRef objParametri As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_R.Leggi_DatiPrecedenti_Agenda()"
        Dim messaggioErrore As String = ""

        Dim dt As DataTable
        'Dim dataModifica As DateTime = AGRODATAINIZIO

        Try
            dt = Leggi(piva, 0, idAgenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, xFiltroAggiuntivo, "", objParametri)

            If Not IsNothing(dt) AndAlso dt.Rows.Count = 1 Then
                dataModifica = dt.Rows(0).Item("Data_Modifica")
                usernameModifica = dt.Rows(0).Item("Username_Modifica")
                dataMovimento = dt.Rows(0).Item("Validita_Inizio")
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function Carica_Operazioni_ZooTecnichexAgenda_Old(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Validita_Inizio As Date,
                                                         ByVal Validita_Fine As Date,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                         ByRef objParametri_Server As AgronicaCoreParametri
                                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If Not IsNothing(DtCentriVisibili) Then
                    For Each dr As DataRow In DtCentriVisibili.Rows
                        FiltroCentri &= dr.Item("sa_cod") & ","
                    Next
                End If
            End If

            Stb.Length = 0

            '
            ' LE OPERAZIONI ZOOTECNICHE
            '
            Stb.AppendLine(" SELECT /*DISTINCT*/ Agenda.Piva ")
            Stb.AppendLine(" , Agenda.Sa_Cod ")
            Stb.AppendLine(" , Agenda.Id_Agenda ")
            Stb.AppendLine(" , Movimenti_dettagli.Id_Mov_Det ")
            Stb.AppendLine(" , Agenda.Lav_Cod ")
            Stb.AppendLine(" , Agenda.Des_Lib ")
            Stb.AppendLine(" , Agenda.Tipo_Accettazione ")
            Stb.AppendLine(" , Movimenti.Data_Movimento ")
            Stb.AppendLine(" , Movimenti.Ora ")
            Stb.AppendLine(" , Movimenti.Mov_Desc ")
            Stb.AppendLine(" , Agenda.Username_Creazione ")
            Stb.AppendLine(" , Movimenti.Cau_Mov ")
            Stb.AppendLine(" , Agenda.Blocco_Flag ")
            Stb.AppendLine(" , ISNULL((Dettagli.Cognome + ' ' + Dettagli.Nome), 'N.D.') AS Tecnico ")
            Stb.AppendLine(" , -1 AS Tipo_Destinazione ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Elem_Cod, 0) AS Elem_Cod ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Mat_Cod, 0) AS Mat_Cod ")
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Pro_Cod, 0) AS Pro_Cod ")
            Stb.AppendLine(" , ISNULL(Materie_Prime.Mat_Des, '') AS Mat_Des ")
            Stb.AppendLine(" , '' AS Cod_Articolo ")
            Stb.AppendLine(" , ISNULL(( SELECT TOP 1 Centri_Aziendali.sa_nome FROM Centri_Aziendali WHERE Centri_Aziendali.sa_cod = Agenda.Sa_Cod and Centri_Aziendali.piva = Agenda.piva ) , '') AS sa_nome ")

            'modifica per avversita
            'Stb.AppendLine(" , ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol ")
            'Stb.AppendLine(" , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")

            Stb.AppendLine(" , i.rag_soc ")
            Stb.AppendLine(" , Operazioni.lav_des ")
            Stb.AppendLine(" , GruppoOperazioni.gru_Des ")
            Stb.AppendLine(" , ISNULL(GruppoOperazioni.tipo, 'C') as tipo ")


            '(09/10/2017 MarcoG) aggiunto il campo contabilizzato per la pianificazione
            Stb.AppendLine(" , ISNULL(Movimenti_dettagli.Contabilizzato, 0) AS contabilizzato ")

            '(12/10/2017 MarcoG) aggiunto le note a checkbox
            Stb.AppendLine(" , ISNULL(Note_Intervento.Nota_Des,'') AS Nota_Des ")

            '(23/11/2017 MarcoG) aggiunte attività per altre operazioni
            Stb.AppendLine(" , ISNULL(Attivita.Sigla, '') AS AttivitaSigla ")
            Stb.AppendLine(" , ISNULL(Attivita.[Desc], '') AS AttivitaDesc ")

            '(09/05/2018 MarcoG)  aggiunta la dicitura per il rif di DDT e Fatture XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
            Stb.AppendLine(" , '' AS RifDdtFatture ")
            Stb.AppendLine(" FROM Movimenti_dettagli ")
            Stb.AppendLine("  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("    RIGHT JOIN " & NomeDB_Utenti & ".dbo.Utenti_Dettagli Dettagli  ")
            Stb.AppendLine("  ")
            Stb.AppendLine("    RIGHT JOIN  Agenda ")
            Stb.AppendLine("  ")
            Stb.AppendLine("    INNER JOIN Movimenti  ")
            Stb.AppendLine("        ON Agenda.PIVA = Movimenti.PIVA ")


            'vanni ,verifica sa_cod
            'StbQuery.Append(" AND Agenda.Sa_Cod = Movimenti.Sa_Cod " )

            Stb.AppendLine("        AND Agenda.Id_Agenda = Movimenti.Id_Agenda  ")
            Stb.AppendLine("        ON Dettagli.CodFisc = Agenda.Username_Creazione  ")
            Stb.AppendLine("        ON Movimenti_dettagli.PIVA = Movimenti.PIVA  ")

            'vanni ,verifica sa_cod
            'StbQuery.Append(" AND Movimenti_dettagli.Sa_Cod = Movimenti.Sa_Cod " )

            Stb.AppendLine("        AND Movimenti_dettagli.Id_Agenda = Movimenti.Id_Agenda  ")
            Stb.AppendLine("        AND Movimenti_dettagli.Id_Mov = Movimenti.Id_Mov  ")


            'Modifica per gruppi operazioni e tipo
            Stb.AppendLine("    LEFT JOIN dbo.Operazioni ")
            Stb.AppendLine("        On Agenda.Lav_Cod = Operazioni.Lav_Cod ")

            Stb.AppendLine("    LEFT JOIN dbo.GruppoOperazioni ")
            Stb.AppendLine("        On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")

            Stb.AppendLine("    inner join imprese i ")
            Stb.AppendLine("        On i.piva = Agenda.PIVA ")

            '(12/10/2017 MarcoG)  aggiunte note testuali e a checkbox
            Stb.AppendLine("   LEFT JOIN AgendaxNote ")
            Stb.AppendLine("   ON Agenda.Id_Agenda = AgendaxNote.Id_Agenda ")
            Stb.AppendLine("   LEFT JOIN Note_Intervento ")
            Stb.AppendLine("   ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")

            '(23/11/2017 MarcoG)  aggiunte attività per altre operazioni
            Stb.AppendLine("   LEFT JOIN Attivita ")
            Stb.AppendLine("   ON Agenda.Id_Attivita = Attivita.Id_Attivita ")

            Stb.AppendLine("   LEFT JOIN Materie_Prime ")
            Stb.AppendLine("   ON Movimenti_dettagli.Elem_Cod = Materie_Prime.Elem_Cod AND Movimenti_dettagli.Mat_Cod = Materie_Prime.Mat_Cod ")


            Stb.AppendLine(" WHERE Agenda.Piva = '" & Agro_SQL_SaveText(Piva) & "'")

            '--------------------------
            'modifica per magazzino e contabilita
            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod))
            Else
                If FiltroCentri <> "" Then
                    Stb.AppendLine(" AND Agenda.sa_cod IN (" & Agro_SQL_Save_Clausola_IN(Left(FiltroCentri, FiltroCentri.Length - 1), False) & ") ")
                End If
            End If
            'If Sa_Cod <> 0 Then
            '    StbQuery.Append(" AND ( Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   or   Agenda.Sa_Cod =0 )   " )
            'End If
            '--------------------------

            '--------------------------
            'modifica per magazzino e contabilita (1000,1001,1020,1021,1022,1023,1025,1031,1033
            'venivano escluse le operazioni contabili e di magazzino
            Stb.AppendLine(" AND (Agenda.Lav_Cod >= " & LAVCOD_NASCITA_ANIMALI & " AND Agenda.Lav_Cod < " & LAVCOD_REVISIONE_MACCHINE & ") ")
            '--------------------------


            '--------------------------
            'modifica per magazzino e contabilita
            Stb.AppendLine(" AND Movimenti.Cau_Mov IN ('" & CAU_ANIMALE & "','" &
                                                                  CAU_ANALISI_LATTE & "','" &
                                                                  CAU_ALIMENTAZIONE & "','" &
                                                                  CAU_LETTIERE & "','" &
                                                                  CAU_MUNGITURA & "','" &
                                                                  CAU_MACELLAZIONE & "','" &
                                                                  CAU_RILIEVI_PRODUZIONI & "','" &
                                                                  CAU_EVENTI & "','" &
                                                                  CAU_VISUALIZZAZIONE_CONSISTENZE & "','" &
                                                                  CAU_CARICO_CONSISTENZE & "','" &
                                                                  CAU_PESATURA_ANIMALI & "','" &
                                                                  CAU_LAVORAZIONE_ZOO & "','" &
                                                                  CAU_SCARICO_CONSISTENZE & "','" &
                                                                  CAU_TRATTAMENTO_ZOO & "') ")
            '--------------------------

            'FACCIO IL CAST PER COMPRENDERE ANCHE LE OPERAZIONI NELLO STESSO GIORNO DELLA DATA FINE
            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Carica_Operazioni_ZooTecnichexAgenda(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Validita_Inizio As Date,
                                                         ByVal Validita_Fine As Date,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                         ByRef objParametri_Server As AgronicaCoreParametri
                                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If Not IsNothing(DtCentriVisibili) AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For Each dr As DataRow In DtCentriVisibili.Rows
                        FiltroCentri &= dr.Item("sa_cod") & ","
                    Next
                    FiltroCentri = FiltroCentri.Substring(0, FiltroCentri.Length - 1)
                End If
            End If

            Dim Validita_FineGiacenze = Validita_Fine
            If Validita_Fine < AGRODATAFINE Then
                Validita_FineGiacenze = Validita_Fine.AddDays(1)
            End If


            Stb.Length = 0



            'Movimenti di carico e spostamenti dei capi in giacenza
            Stb.AppendLine("--Movimenti di carico e spostamenti dei capi in giacenza")
            Dim movpos_TT As String = Crea_movsposTT(Piva, Sa_Cod, Validita_Inizio, Validita_FineGiacenze)
            Stb.AppendLine(movpos_TT)


            'Ricava l'ultimo carico o spostamento effettuato su ogni capo
            Stb.AppendLine("--Ricava l'ultimo carico o spostamento effettuato su ogni capo ")
            Dim lastmovpos_TT As String = Crea_lastmovsposTT()
            Stb.AppendLine(lastmovpos_TT)


            'JOIN su centri, stalle e raggruppamenti degli ultimi movimenti ricavati
            Stb.AppendLine("--JOIN su centri, stalle e raggruppamenti degli ultimi movimenti ricavati ")
            Dim agn_cte As String = Crea_agnTT()
            Stb.AppendLine(agn_cte)

            'Tabella temporanea con Spostamenti
            Stb.AppendLine("--Tabella spostamenti ")
            Dim spostamenti_tt As String = Crea_SpostamentiTT(Piva, Sa_Cod, Validita_Inizio, Validita_Fine, FiltroCentri, objParametri_Server)
            Stb.AppendLine(spostamenti_tt)

            'CTE per prodotti aggregati usata per trattamenti/alimentazione
            Stb.AppendLine("-- Prodotti Aggregati CTE per trattamenti/alimentazione")
            Stb.AppendLine(Crea_ProdottiAggreagatiCTE(Piva, Validita_Inizio, Validita_Fine, objParametri_Server))


            'Carichi/Scarichi
            Stb.AppendLine(" --Carichi/Scarichi  ")
            Dim qryCarichiScarichi As String = Crea_CarichiScarichi(Piva, Sa_Cod, Validita_Inizio, Validita_Fine, FiltroCentri, objParametri_Server)
            Stb.AppendLine(qryCarichiScarichi)
            Stb.AppendLine(" UNION ")

            'Spostamenti
            Stb.AppendLine(" --Spostamenti  ")
            Dim qrySpostamenti As String = Crea_Spostamenti()
            Stb.AppendLine(qrySpostamenti)
            Stb.AppendLine("  UNION ")

            'Trattamenti/Alimentazione
            Stb.AppendLine(" --Trattamenti/Alimentazione  ")
            Dim qryTrattamenti As String = Crea_Trattamenti(Piva, Sa_Cod, Validita_Inizio, Validita_Fine, FiltroCentri, objParametri_Server)
            Stb.AppendLine(qryTrattamenti)
            Stb.AppendLine("  UNION ")

            'Pesatura/Altre Lavorazioni
            Stb.AppendLine(" --Pesatura/Altre Lavorazioni  ")
            Dim qryAltreLavorazioni As String = Crea_AltreLavorazioni(Piva, Sa_Cod, Validita_Inizio, Validita_Fine, FiltroCentri, objParametri_Server)
            Stb.AppendLine(qryAltreLavorazioni)

            If LivelloCompatibilita(objParametri_Server) >= 150 Then
                Stb.AppendLine(" OPTION (USE HINT ('FORCE_LEGACY_CARDINALITY_ESTIMATION')) ")
            End If

            Stb.AppendLine(" DROP TABLE #spostamenti ")
            Stb.AppendLine(" DROP TABLE #movspos ")
            Stb.AppendLine(" DROP TABLE #last_movspos ")
            Stb.AppendLine(" DROP TABLE #agn_cte ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Carica_Operazioni_ZooTecnichexAgenda_CTE(ByVal Piva As String,
                                                         ByVal Sa_Cod As Integer,
                                                         ByVal Validita_Inizio As Date,
                                                         ByVal Validita_Fine As Date,
                                                         ByVal xOrderBy As String,
                                                         ByRef objParametri_Utenti As AgronicaCoreParametri,
                                                         ByRef objParametri_Server As AgronicaCoreParametri
                                                         ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            Dim NomeDB_Utenti As String = objParametri_Utenti.StringaConnessione.Split(";")(2).Split("=")(1)
            Dim FiltroCentri As String = ""

            If Sa_Cod = 0 Then
                'leggo se ci sono filtri sui centri
                Dim objUtentiVisibilita As New AgronicaCoreUtentiDAL.Utenti_Visibilita_Appoggio_R
                Dim DtCentriVisibili As DataTable = objUtentiVisibilita.Leggi(enum_TipoEntita.Centro, " Piva='" & Agro_SQL_SaveText(Piva) & "'", "", objParametri_Server)
                If Not IsNothing(DtCentriVisibili) AndAlso DtCentriVisibili.Rows.Count > 0 Then
                    For Each dr As DataRow In DtCentriVisibili.Rows
                        FiltroCentri &= dr.Item("sa_cod") & ","
                    Next
                    FiltroCentri = FiltroCentri.Substring(0, FiltroCentri.Length - 1)
                End If
            End If

            Dim Validita_FineGiacenze = Validita_Fine.AddDays(1)

            Stb.Length = 0

            'Movimenti di carico e spostamenti dei capi in giacenza
            Stb.AppendLine("--Movimenti di carico e spostamenti dei capi in giacenza").
                AppendLine("WITH movspos AS ( ").
                AppendLine("    SELECT ").
                AppendLine("        mdes.Piva, ").
                AppendLine("        mdes.Sa_Cod, ").
                AppendLine("        mdes.Id_Destinazione, ").
                AppendLine("        md.Cod_Progetto, ").
                AppendLine("        m.Cau_Mov, ").
                AppendLine("        m.Id_Mov, ").
                AppendLine("        m.Data_Movimento ").
                AppendLine("    FROM Agenda a (NOLOCK) ").
                AppendLine("    INNER JOIN Movimenti m (NOLOCK) ON m.PIVA = a.PIVA AND m.Id_Agenda = a.Id_Agenda ").
                AppendLine("    INNER JOIN Movimenti_dettagli md (NOLOCK) ON md.PIVA = m.Piva AND md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov ").
                AppendLine("    INNER JOIN Mov_Destinazioni mdes (NOLOCK) ON mdes.PIVA = md.Piva AND mdes.Id_Agenda = md.Id_Agenda AND mdes.Id_Mov = md.Id_Mov AND mdes.Id_Mov_Det = md.Id_Mov_Det ").
                AppendLine("    INNER JOIN Zoo_Animali ON md.Cod_Progetto = Zoo_Animali.Cod_Progetto ").
                AppendLine("    WHERE 1=1 ")

            If Piva <> "" Then
                Stb.AppendLine("        AND a.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " ")
            End If

            If Sa_Cod <> 0 Then
                Stb.AppendLine("        AND mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            Stb.Append("        AND a.Lav_Cod IN (").Append(Agro_SQL_Save_Clausola_IN(String.
                    Join(",", {LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI, LAVCOD_SPOSTAMENTI_ZOO}.ToArray))).
                AppendLine(") ").
                Append("        AND m.Cau_Mov IN (").Append(CAU_CARICO_CONSISTENZE).AppendLine(") ").
                AppendLine("        AND m.Data_Movimento >= CONVERT(DateTime,'1900/01/01',120) ").
                AppendLine("        AND m.Data_Movimento <=" & Agro_SQL_SaveDate(Validita_FineGiacenze)).
                Append("        AND md.Elem_Cod = ").Append(ZOO_CONSISTENZA).AppendLine(" ").
                AppendLine("        AND md.Jolly_Int = 0 ").
                AppendLine("        AND Zoo_Animali.Validita_Fine >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ").
                Append("        AND mdes.Tipo_Destinazione IN (").Append(TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA).AppendLine(") ").
                AppendLine("), ")

            'Ricava l'ultimo carico o spostamento effettuato su ogni capo
            Stb.AppendLine("--Ricava l'ultimo carico o spostamento effettuato su ogni capo ").
                AppendLine("last_movspos AS (").
                AppendLine("    SELECT ").
                AppendLine("        movspos.PIVA, ").
                AppendLine("        movspos.Sa_Cod, ").
                AppendLine("        movspos.Id_Destinazione, ").
                AppendLine("        movspos.Cod_Progetto, ").
                AppendLine("        movspos.Cau_Mov, ").
                AppendLine("        movspos.Data_Movimento, ").
                AppendLine("        ROW_NUMBER() OVER (PARTITION BY movspos.Cod_Progetto ORDER BY movspos.Data_Movimento DESC) as rn ").
                AppendLine("    FROM movspos ").
                AppendLine("), ")

            'JOIN su centri, stalle e raggruppamenti degli ultimi movimenti ricavati
            Stb.AppendLine("--JOIN su centri, stalle e raggruppamenti degli ultimi movimenti ricavati ").
                AppendLine("agn_cte AS ( ").
                AppendLine("    SELECT ").
                AppendLine("        last_movspos.Cod_Progetto, ").
                AppendLine("        last_movspos.Sa_Cod, ").
                AppendLine("        last_movspos.Cau_Mov, ").
                AppendLine("        ca.sa_nome, ").
                AppendLine("        last_movspos.Data_Movimento, ").
                AppendLine("        sta.STA_NUM, ").
                AppendLine("        sta.STA_DES ").
                AppendLine("    FROM last_movspos ").
                AppendLine("    INNER JOIN Centri_Aziendali ca (NOLOCK) ON ca.PIVA = last_movspos.Piva AND ca.sa_cod = last_movspos.Sa_Cod ").
                AppendLine("    INNER JOIN Stalla_Raggruppamenti stra (NOLOCK) ON last_movspos.Id_Destinazione = stra.Raggruppamento_Cod ").
                AppendLine("    INNER JOIN Stalla sta (NOLOCK) ON sta.PIVA = stra.PIVA AND sta.sa_cod= stra.sa_cod AND sta.STA_NUM = stra.STA_NUM ").
                AppendLine("    WHERE last_movspos.rn = 1 ").
                AppendLine(") ")

            'Stb.AppendLine("  with  ")
            'Stb.AppendLine(" agn_cte as ")
            'Stb.AppendLine("  ( ")
            'Stb.AppendLine("  select   ")
            ''Stb.AppendLine(" 	a.Id_Agenda,  ")
            ''Stb.AppendLine(" 	a.PIVA,  ")
            'Stb.AppendLine(" 	md.Cod_Progetto, ")
            ''Stb.AppendLine(" 	md.Lotto, ")
            ''Stb.AppendLine(" 	md.Udm_Cod, ")
            ''Stb.AppendLine(" 	UnitaMisura.Udm_Des, ")
            ''Stb.AppendLine(" 	UnitaMisura.Udm_Sim, ")
            'Stb.AppendLine(" 	mdes.Sa_Cod, ")
            ''Stb.AppendLine(" 	mdes.Id_Destinazione, ")
            ''Stb.AppendLine(" 	mdes.Tipo_Destinazione, ")
            ''Stb.AppendLine(" 	mdes.Qta, ")
            ''Stb.AppendLine(" 	md.Id_Mov, ")
            ''Stb.AppendLine(" 	md.Id_Mov_Det, ")
            'Stb.AppendLine(" 	m.Cau_Mov, ")
            ''Stb.AppendLine(" 	i.rag_soc, ")
            ''Stb.AppendLine(" 	ic.val_cod as cuaa, ")
            'Stb.AppendLine(" 	Centri_Aziendali.sa_nome, ")
            'Stb.AppendLine(" 	m.Data_Movimento, ")
            ''Stb.AppendLine("    Stalla_Raggruppamenti.Raggruppamento_Cod, ")
            ''Stb.AppendLine("    Stalla_Raggruppamenti.Raggruppamento_Des, ")
            'Stb.AppendLine("    STALLA.STA_NUM, ")
            'Stb.AppendLine("    STALLA.STA_DES ")
            'Stb.AppendLine(" 	from agenda a (NOLOCK)  ")
            ''Stb.AppendLine(" 	inner join imprese i (NOLOCK) on i.PIVA = a.PIVA ")
            ''Stb.AppendLine(" 	inner join imprese_codici ic (NOLOCK) on i.piva = ic.piva and ic.id_cod = 1010 ")
            'Stb.AppendLine(" 	inner join Movimenti m (NOLOCK) on	a.Id_Agenda = m.Id_Agenda  ")
            'Stb.AppendLine(" 	inner join Movimenti_dettagli md (NOLOCK) on md.Id_Agenda = m.Id_Agenda  AND md.Id_Mov = m.Id_Mov  ")
            'Stb.AppendLine(" 	inner join mov_destinazioni mdes (NOLOCK) on mdes.Id_Agenda = md.Id_Agenda and mdes.Id_Mov = md.Id_Mov and mdes.Id_Mov_Det = md.Id_Mov_Det ")
            'Stb.AppendLine(" 	inner join Centri_Aziendali (NOLOCK) ON mdes.Piva = Centri_Aziendali.Piva and mdes.Sa_Cod=Centri_Aziendali.sa_cod  ")
            ''Stb.AppendLine(" 	INNER Join UnitaMisura (NOLOCK) ON UnitaMisura.Udm_Cod = md.Udm_Cod ")
            'Stb.AppendLine(" 	INNER JOIN Stalla_Raggruppamenti (NOLOCK)  ON mdes.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            'Stb.AppendLine("    INNER JOIN Stalla (NOLOCK)  ON Stalla_Raggruppamenti.PIVA = Stalla.PIVA ")
            'Stb.AppendLine(" 	       AND Stalla_Raggruppamenti.sa_cod= Stalla.sa_cod ")
            'Stb.AppendLine(" 		   AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
            'Stb.AppendLine(" 	where  1 = 1 ")
            'If Piva <> "" Then
            '    Stb.AppendLine(" And a.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "      ")
            'End If

            'If Sa_Cod <> 0 Then
            '    Stb.AppendLine(" And mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            'End If
            'Stb.AppendLine(" 	and a.lav_Cod IN (3000,3001,3034) ")
            'Stb.AppendLine(" 	And m.Cau_Mov In ('3700')   ")
            'Stb.AppendLine(" 	And m.Data_Movimento >=  CONVERT(DateTime,'1900/01/01',120)     ")
            'Stb.AppendLine(" 	And m.Data_Movimento <=    " & Agro_SQL_SaveDate(Validita_FineGiacenze) & "      ")
            'Stb.AppendLine(" 	And md.Elem_Cod = 300  ")
            'Stb.AppendLine(" 	And md.Jolly_Int = 0  ")
            'Stb.AppendLine(" 	And mdes.Tipo_Destinazione IN (21)  ")
            'Stb.AppendLine(" ) ")

            'Stb.AppendLine(" , ")
            'Stb.AppendLine(" giac_cte as ( ")
            'Stb.AppendLine(" 	SELECT   ")
            'Stb.AppendLine("   agn_cte.Piva, agn_cte.Rag_Soc As Impresa,  ")
            'Stb.AppendLine("   agn_cte.cuaa, Stalla.BDN_Codice_Azienda,  ")
            'Stb.AppendLine("   agn_cte.Cod_Progetto AS Cod_Animale,  ")
            'Stb.AppendLine("   agn_cte.Lotto,  ")
            'Stb.AppendLine("   agn_cte.Udm_Cod,  ")
            'Stb.AppendLine("   agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            'Stb.AppendLine("   agn_cte.Sa_Cod,  ")
            'Stb.AppendLine("   agn_cte.sa_nome,  ")
            'Stb.AppendLine("   agn_cte.Id_Destinazione,  ")
            'Stb.AppendLine("   agn_cte.Tipo_Destinazione,  ")
            'Stb.AppendLine("   Lista_AUSL.denominazione AS AUSL_DES,  ")
            'Stb.AppendLine("   MIN(agn_cte.Data_Movimento) as mindata,")
            'Stb.AppendLine("   IIF (MAX(agn_cte.Data_Movimento) = MIN(agn_cte.Data_Movimento), CONVERT(datetime, '2100-12-31 00:00:00.000', 120), MAX(agn_cte.Data_Movimento)) as maxdata,")
            'Stb.AppendLine("   COALESCE(Fabbricati.Fabbricato_Des, '') As STA_DES,  ")
            'Stb.AppendLine(" 	COALESCE(Fabbricati.Fabbricato_Cod, 0) As STA_NUM,  ")
            'Stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Des, '') as Raggruppamento_Des,  ")
            'Stb.AppendLine("   COALESCE(Stalla_Raggruppamenti.Raggruppamento_Cod, 0) as Raggruppamento_Cod,  ")
            'Stb.AppendLine("   Convert(INTEGER, SUM( Case When agn_cte.CAU_MOV In ('7350','3750')              ")
            'Stb.AppendLine("           THEN -(agn_cte.qta)             ")
            'Stb.AppendLine("           Else agn_cte.qta             ")
            'Stb.AppendLine("           End)) As Giacenza   ")
            'Stb.AppendLine("  From agn_cte  ")
            'Stb.AppendLine("  INNER Join Stalla_Raggruppamenti (NOLOCK) ON agn_cte.Sa_Cod = Stalla_Raggruppamenti.sa_cod  ")
            'Stb.AppendLine("                                 AND agn_cte.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            'Stb.AppendLine("                                 AND agn_cte.Tipo_Destinazione = 21  ")
            'Stb.AppendLine("  INNER JOIN Fabbricati (NOLOCK) ON Stalla_Raggruppamenti.PIVA = Fabbricati.Piva AND Stalla_Raggruppamenti.sa_cod = Fabbricati.SA_COD AND Stalla_Raggruppamenti.STA_NUM = Fabbricati.Fabbricato_Cod ")
            'Stb.AppendLine("  INNER JOIN Stalla (NOLOCK) ON Stalla.PIVA = Fabbricati.Piva AND Stalla.sa_cod = Fabbricati.SA_COD AND Stalla.STA_NUM = Fabbricati.Fabbricato_Cod  ")
            'Stb.AppendLine("  INNER JOIN Indirizzi (NOLOCK) ON Fabbricati.Indirizzo_Cod = Indirizzi.cod_indirizzo ")
            'Stb.AppendLine("  LEFT JOIN IstatxDistretti (NOLOCK) ON IstatxDistretti.pro_cod = Indirizzi.pro_cod_istat AND IstatxDistretti.com_cod = Indirizzi.com_cod_istat ")
            'Stb.AppendLine("  LEFT JOIN Lista_Distretti (NOLOCK) ON Lista_Distretti.distretto_id = IstatxDistretti.distretto_id ")
            'Stb.AppendLine("  LEFT JOIN Lista_AUSL (NOLOCK) ON Lista_Distretti.asl_id = Lista_AUSL.asl_id ")
            'Stb.AppendLine("   where 1=1  ")
            'If Piva <> "" Then
            '    Stb.AppendLine(" And agn_cte.Piva = " & Agro_SQL_SaveText_NULL(Piva) & "     ")
            'End If
            'If Sa_Cod <> 0 Then
            '    Stb.AppendLine(" And agn_cte.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & "      ")
            'End If
            'Stb.AppendLine("  GROUP BY agn_cte.Piva, agn_cte.Rag_Soc, agn_cte.cuaa, Stalla.BDN_Codice_Azienda, ")
            'Stb.AppendLine("  agn_cte.Cod_Progetto,  ")
            'Stb.AppendLine("  agn_cte.Lotto,  ")
            'Stb.AppendLine("  agn_cte.Udm_Cod, ")
            'Stb.AppendLine("  Fabbricati.Fabbricato_Des, ")
            'Stb.AppendLine("  Fabbricati.Fabbricato_Cod, ")
            'Stb.AppendLine("  agn_cte.Sa_Cod,  ")
            'Stb.AppendLine("  agn_cte.sa_nome, ")
            'Stb.AppendLine("  agn_cte.Id_Destinazione,  ")
            'Stb.AppendLine("  agn_cte.Tipo_Destinazione,  ")
            'Stb.AppendLine("  agn_cte.Udm_Des, agn_cte.Udm_Sim,  ")
            'Stb.AppendLine("  Lista_AUSL.denominazione,  ")
            'Stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Des,  ")
            'Stb.AppendLine("  Stalla_Raggruppamenti.Raggruppamento_Cod  ")
            'Stb.AppendLine("  --HAVING Convert(INTEGER, SUM(Case When agn_cte.Cau_Mov In ('7350','3750') THEN -(agn_cte.qta) ELSE agn_cte.qta END)) <> 0   ")
            'Stb.AppendLine(" ) ")

            'Carichi/Scarichi
            Stb.AppendLine(" --Carichi/Scarichi  ")
            Stb.AppendLine(" SELECT  ")
            Stb.AppendLine("     Agenda.Piva   ")
            Stb.AppendLine("   , Centri_Aziendali.Sa_Cod   ")
            Stb.AppendLine("   , Agenda.Id_Agenda   ")
            Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
            Stb.AppendLine("   , Agenda.Lav_Cod   ")
            'Stb.AppendLine("   , Agenda.Des_Lib   ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("   , Agenda.Blocco_Flag ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
            Stb.AppendLine("   , Movimenti.Ora   ")
            'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
            Stb.AppendLine("   , 0 as ID_Mov_Det ")
            Stb.AppendLine("   , '' as Info ")
            Stb.AppendLine("   , '' as Dettagli ")
            Stb.AppendLine("   , 0 as Veg_Cod ")
            Stb.AppendLine("   , Agenda.Username_Creazione   ")
            'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
            Stb.AppendLine("   , Agenda.Blocco_Flag   ")
            Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
            Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
            Stb.AppendLine("   , 0 AS Elem_Cod   ")
            Stb.AppendLine("   , 0 AS Mat_Cod   ")
            Stb.AppendLine("   , 0 AS Pro_Cod   ")
            Stb.AppendLine("   , '' AS Mat_Des   ")
            Stb.AppendLine("   , '' AS Cod_Articolo   ")
            Stb.AppendLine("   , Centri_Aziendali.sa_nome  ")
            Stb.AppendLine("   , i.rag_soc   ")
            Stb.AppendLine("   , Operazioni.lav_des   ")
            Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
            Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
            Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
            Stb.AppendLine("   , '' as tipo_colore ")
            Stb.AppendLine("   , 0 AS contabilizzato   ")
            Stb.AppendLine("   , '' as LottiProduzione ")
            Stb.AppendLine("   , '' AS Nota_Des   ")
            Stb.AppendLine("   , '' AS Note ")
            Stb.AppendLine("   , '' as Costi_Operatori ")
            Stb.AppendLine("   , '' as Costi_Macchine ")
            Stb.AppendLine("   , 0 as Sup_Trattata ")
            Stb.AppendLine("   , '' as LottiImpianto ")
            Stb.AppendLine("   , '' as Descrizione_Unica ")
            Stb.AppendLine("   , '' as Prodotti ")
            Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
            Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
            Stb.AppendLine("   , '' AS RifDdtFatture  ")
            Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
            'Stb.AppendLine("   , STRING_AGG(Zoo_Animali.Matricola, ', ') As Dettaglio_Tecnico ")
            Stb.AppendLine("   , IIF(Mov_Lav.Extra_Str IS NULL OR TRIM(Mov_Lav.Extra_Str) = '', Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', '), Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + COALESCE(Mov_Lav.Extra_Str + ': ', '') + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ')) As Dettaglio_Tecnico ")
            Stb.AppendLine("   , '' AS Segnalazioni ")
            Stb.AppendLine("   , '' As PermessoModifica ")

            Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
            Stb.AppendLine(" JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
            Stb.AppendLine(" LEFT JOIN Movimenti (NOLOCK)  Mov_Lav ON Agenda.Id_Agenda = Mov_Lav.ID_Agenda AND Mov_Lav.Cau_Mov = '4000' ")
            Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
            Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
            Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
            Stb.AppendLine(" JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.ID_Agenda = Movimenti_dettagli.Id_Agenda  ")
            Stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Elem_Cod = 300 ")
            Stb.AppendLine(" JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            Stb.AppendLine(" 					AND Mov_Destinazioni.Tipo_Destinazione = 21 ")
            Stb.AppendLine("  JOIN Stalla_Raggruppamenti (NOLOCK)  ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            Stb.AppendLine("  JOIN Stalla (NOLOCK)  ON Stalla_Raggruppamenti.Piva = Stalla.PIVA ")
            Stb.AppendLine(" 			AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
            Stb.AppendLine(" 			AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
            Stb.AppendLine(" JOIN Centri_Aziendali (NOLOCK)  ON Stalla.Piva = Centri_Aziendali.Piva AND Stalla.Sa_Cod = Centri_Aziendali.sa_cod ")
            Stb.AppendLine(" JOIN Zoo_Animali (NOLOCK)  ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")

            Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3000, 3001, 3002, 3003, 3004, 3004, 3034,3035,3037))  ")
            Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3700','3750')  ")

            If Piva <> "" Then
                Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If FiltroCentri <> "" Then
                Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
            End If

            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine("  GROUP BY Agenda.Piva  ")
            Stb.AppendLine("  , Centri_Aziendali.Sa_Cod  ")
            Stb.AppendLine("  , Agenda.Id_Agenda  ")
            Stb.AppendLine("  , Agenda.Lav_Cod  ")
            'Stb.AppendLine("  , Agenda.Des_Lib  ")
            Stb.AppendLine("  , Movimenti.Data_Movimento  ")
            Stb.AppendLine("  , Movimenti.Ora  ")
            Stb.AppendLine("  , Mov_Lav.Extra_Str ")
            Stb.AppendLine("  , Agenda.Username_Creazione  ")
            'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
            Stb.AppendLine("  , Agenda.Blocco_Flag  ")
            Stb.AppendLine("  , Centri_Aziendali.sa_nome ")
            Stb.AppendLine("  , i.rag_soc  ")
            Stb.AppendLine("  , Operazioni.lav_des  ")
            Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
            Stb.AppendLine("  , GruppoOperazioni.tipo ")
            Stb.AppendLine("  , Attivita.Sigla ")
            Stb.AppendLine("  , Attivita.[Desc] ")
            Stb.AppendLine("  , Utenti.[user] ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("   , Stalla.STA_DES ")
            Stb.AppendLine("   ")
            Stb.AppendLine(" UNION ")

            'Spostamenti
            Stb.AppendLine(" --Spostamenti  ")
            Stb.AppendLine(" SELECT  ")
            Stb.AppendLine("     Agenda.Piva   ")
            Stb.AppendLine("   , Centri_Aziendali.Sa_Cod   ")
            Stb.AppendLine("   , Agenda.Id_Agenda   ")
            Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
            Stb.AppendLine("   , Agenda.Lav_Cod   ")
            'Stb.AppendLine("   , Agenda.Des_Lib   ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("   , Agenda.Blocco_Flag ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
            Stb.AppendLine("   , Movimenti.Ora   ")
            'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
            Stb.AppendLine("   , 0 as ID_Mov_Det ")
            Stb.AppendLine("   , '' as Info ")
            Stb.AppendLine("   , '' as Dettagli ")
            Stb.AppendLine("   , 0 as Veg_Cod ")
            Stb.AppendLine("   , Agenda.Username_Creazione   ")
            'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
            Stb.AppendLine("   , Agenda.Blocco_Flag   ")
            Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
            Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
            Stb.AppendLine("   , 0 AS Elem_Cod   ")
            Stb.AppendLine("   , 0 AS Mat_Cod   ")
            Stb.AppendLine("   , 0 AS Pro_Cod   ")
            Stb.AppendLine("   , '' AS Mat_Des   ")
            Stb.AppendLine("   , '' AS Cod_Articolo   ")
            Stb.AppendLine("   , Centri_Aziendali.sa_nome  ")
            Stb.AppendLine("   , i.rag_soc   ")
            Stb.AppendLine("   , Operazioni.lav_des   ")
            Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
            Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
            Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
            Stb.AppendLine("   , '' as tipo_colore ")
            Stb.AppendLine("   , 0 AS contabilizzato   ")
            Stb.AppendLine("   , '' as LottiProduzione ")
            Stb.AppendLine("   , '' AS Nota_Des   ")
            Stb.AppendLine("   , '' AS Note ")
            Stb.AppendLine("   , '' as Costi_Operatori ")
            Stb.AppendLine("   , '' as Costi_Macchine ")
            Stb.AppendLine("   , 0 as Sup_Trattata ")
            Stb.AppendLine("   , '' as LottiImpianto ")
            Stb.AppendLine("   , '' as Descrizione_Unica ")
            Stb.AppendLine("   , '' as Prodotti ")
            Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
            Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
            Stb.AppendLine("   , '' AS RifDdtFatture  ")
            Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
            'Stb.AppendLine("   , STRING_AGG(Zoo_Animali.Matricola, ', ') As Dettaglio_Tecnico ")
            Stb.AppendLine("   , IIF(Mov_Lav.Extra_Str IS NULL OR TRIM(Mov_Lav.Extra_Str) = '', Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', '), Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + COALESCE(Mov_Lav.Extra_Str + ': ', '') + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ')) As Dettaglio_Tecnico ")
            Stb.AppendLine("   , STRING_AGG(CAST(IIF(Zoo_Animali.Matricola IS NULL, Movimenti_dettagli.Mov_Det_Des, NULL) AS NVARCHAR(MAX)), ', ') AS Segnalazioni ")
            Stb.AppendLine("   , '' As PermessoModifica ")

            Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
            Stb.AppendLine(" JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
            Stb.AppendLine(" LEFT JOIN Movimenti (NOLOCK)  Mov_Lav ON Agenda.Id_Agenda = Mov_Lav.ID_Agenda AND Mov_Lav.Cau_Mov = '4000' ")
            Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
            Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
            Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
            Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.ID_Agenda = Movimenti_dettagli.Id_Agenda  ")
            Stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Elem_Cod = 300 ")
            Stb.AppendLine(" JOIN Mov_Destinazioni (NOLOCK)  ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            Stb.AppendLine(" 					AND Mov_Destinazioni.Tipo_Destinazione = 21 ")
            Stb.AppendLine("  JOIN Stalla_Raggruppamenti (NOLOCK)  ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
            Stb.AppendLine("  JOIN Stalla (NOLOCK)  ON Stalla_Raggruppamenti.Piva = Stalla.PIVA ")
            Stb.AppendLine(" 			AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
            Stb.AppendLine(" 			AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
            Stb.AppendLine(" JOIN Centri_Aziendali (NOLOCK)  ON Stalla.Piva = Centri_Aziendali.Piva AND Stalla.Sa_Cod = Centri_Aziendali.sa_cod ")
            Stb.AppendLine(" LEFT JOIN Zoo_Animali (NOLOCK)  ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")

            Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3030))  ")
            Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3700')  ")

            If Piva <> "" Then
                Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If FiltroCentri <> "" Then
                Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
            End If

            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine("  GROUP BY Agenda.Piva  ")
            Stb.AppendLine("  , Centri_Aziendali.Sa_Cod  ")
            Stb.AppendLine("  , Agenda.Id_Agenda  ")
            Stb.AppendLine("  , Agenda.Lav_Cod  ")
            'Stb.AppendLine("  , Agenda.Des_Lib  ")
            Stb.AppendLine("  , Movimenti.Data_Movimento  ")
            Stb.AppendLine("  , Movimenti.Ora  ")
            Stb.AppendLine("  , Mov_Lav.Extra_Str ")
            Stb.AppendLine("  , Agenda.Username_Creazione  ")
            'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
            Stb.AppendLine("  , Agenda.Blocco_Flag  ")
            Stb.AppendLine("  , Centri_Aziendali.sa_nome ")
            Stb.AppendLine("  , i.rag_soc  ")
            Stb.AppendLine("  , Operazioni.lav_des  ")
            Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
            Stb.AppendLine("  , GruppoOperazioni.tipo ")
            Stb.AppendLine("  , Attivita.Sigla ")
            Stb.AppendLine("  , Attivita.[Desc] ")
            Stb.AppendLine("  , Utenti.[user] ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("   , Stalla.STA_DES ")
            Stb.AppendLine("   ")
            Stb.AppendLine("  UNION ")

            'Trattamenti/Alimentazione
            Stb.AppendLine(" --Trattamenti/Alimentazione  ")
            Stb.AppendLine(" SELECT  ")
            Stb.AppendLine("     Agenda.Piva   ")
            Stb.AppendLine("   , g.Sa_Cod   ")
            Stb.AppendLine("   , Agenda.Id_Agenda   ")
            Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
            Stb.AppendLine("   , Agenda.Lav_Cod   ")
            'Stb.AppendLine("   , Agenda.Des_Lib   ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("   , Agenda.Blocco_Flag ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
            Stb.AppendLine("   , Movimenti.Ora   ")
            'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
            Stb.AppendLine("   , 0 as ID_Mov_Det ")
            Stb.AppendLine("   , '' as Info ")
            Stb.AppendLine("   , '' as Dettagli ")
            Stb.AppendLine("   , 0 as Veg_Cod ")
            Stb.AppendLine("   , Agenda.Username_Creazione   ")
            'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
            Stb.AppendLine("   , Agenda.Blocco_Flag   ")
            Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
            Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
            Stb.AppendLine("   , 0 AS Elem_Cod   ")
            Stb.AppendLine("   , 0 AS Mat_Cod   ")
            Stb.AppendLine("   , COALESCE(dettagli_farmaci.pro_Cod, dettagli_mat.pro_cod, 0) AS Pro_Cod   ")
            Stb.AppendLine("   , COALESCE(Farmaci.Denominazione, Materie_Prime.Mat_Des, '') AS Mat_Des   ")
            Stb.AppendLine("   , '' AS Cod_Articolo   ")
            Stb.AppendLine("   , g.sa_nome  ")
            Stb.AppendLine("   , i.rag_soc   ")
            Stb.AppendLine("   , Operazioni.lav_des   ")
            Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
            Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
            Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
            Stb.AppendLine("   , '' as tipo_colore ")
            Stb.AppendLine("   , 0 AS contabilizzato   ")
            Stb.AppendLine("   , '' as LottiProduzione ")
            Stb.AppendLine("   , '' AS Nota_Des   ")
            Stb.AppendLine("   , '' AS Note ")
            Stb.AppendLine("   , '' as Costi_Operatori ")
            Stb.AppendLine("   , '' as Costi_Macchine ")
            Stb.AppendLine("   , 0 as Sup_Trattata ")
            Stb.AppendLine("   , '' as LottiImpianto ")
            Stb.AppendLine("   , '' as Descrizione_Unica ")
            Stb.AppendLine("   , COALESCE(Farmaci.Denominazione, Materie_Prime.Mat_Des, '') as Prodotti  ")
            Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
            Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
            Stb.AppendLine("   , '' AS RifDdtFatture  ")
            Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
            Stb.AppendLine("   , IIF(g.STA_DES IS NULL, '', g.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ')  + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ') as Dettaglio_Tecnico ")
            Stb.AppendLine("   , '' AS Segnalazioni ")
            Stb.AppendLine("   , '' As PermessoModifica ")

            Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
            Stb.AppendLine(" JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
            Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
            Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
            Stb.AppendLine(" LEFT JOIN Centri_Aziendali (NOLOCK)  ON Agenda.Piva = Centri_Aziendali.Piva AND Agenda.Sa_Cod = Centri_Aziendali.sa_cod ")
            Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
            Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  dettagli_farmaci ON Movimenti.ID_Agenda = dettagli_farmaci.Id_Agenda  ")
            Stb.AppendLine(" 					AND Movimenti.Id_Mov = dettagli_farmaci.Id_Mov ")
            Stb.AppendLine(" 					AND dettagli_farmaci.Elem_Cod = 307 ")
            Stb.AppendLine(" LEFT JOIN Farmaci (NOLOCK)  ON dettagli_farmaci.Pro_Cod = Farmaci.Farm_Cod ")
            Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  dettagli_mat ON Movimenti.ID_Agenda = dettagli_mat.Id_Agenda  ")
            Stb.AppendLine(" 					AND Movimenti.Id_Mov = dettagli_mat.Id_Mov ")
            Stb.AppendLine(" 					AND dettagli_mat.Elem_Cod <> 307 ")
            Stb.AppendLine(" LEFT JOIN Materie_Prime (NOLOCK)  ON dettagli_mat.Mat_Cod = Materie_Prime.Mat_Cod ")
            Stb.AppendLine(" LEFT JOIN Mov_Destinazioni (NOLOCK)  ON Movimenti.ID_Agenda = Mov_Destinazioni.Id_Agenda  ")
            Stb.AppendLine(" 					AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
            Stb.AppendLine(" 					AND Mov_Destinazioni.Tipo_Destinazione = 1 ")
            Stb.AppendLine(" 					AND Mov_Destinazioni.Id_Destinazione > 0 ")
            Stb.AppendLine(" LEFT JOIN Zoo_Animali (NOLOCK)  ON Mov_Destinazioni.ID_Destinazione = Zoo_Animali.Cod_Progetto ")
            'Stb.AppendLine(" LEFT JOIN giac_cte g ON Zoo_animali.Cod_Progetto = g.Cod_Animale  ")
            'Stb.AppendLine("				AND  Movimenti.Data_Movimento >= g.mindata ")
            'Stb.AppendLine("				AND  Movimenti.Data_Movimento <= g.maxData ")
            'Stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM agn_cte WHERE agn_cte.Cau_Mov = '3700' AND agn_cte.Cod_Progetto = Zoo_Animali.Cod_Progetto ORDER BY agn_cte.Data_Movimento DESC ) g  ")
            Stb.AppendLine(" LEFT JOIN agn_cte g ON g.Cod_Progetto = Zoo_Animali.Cod_Progetto  ")
            Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")

            Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3005,3008,3009,3010,3011,3012,3013,3014, ")
            Stb.AppendLine(" 3015,3016,3017,3018,3019,3020,3021,3022,3023,3024,3025,3026,3027,3028,3029, ")
            Stb.AppendLine(" 3031,3032,3036,3037))  ")
            Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3860','3450','3200', '3850')  ")

            If Piva <> "" Then
                Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If FiltroCentri <> "" Then
                Stb.AppendLine(" AND g.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
            End If

            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND g.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine("  GROUP BY Agenda.Piva  ")
            Stb.AppendLine("  , g.Sa_Cod  ")
            Stb.AppendLine("  , Agenda.Id_Agenda  ")
            Stb.AppendLine("  , Agenda.Lav_Cod  ")
            'Stb.AppendLine("  , Agenda.Des_Lib  ")
            Stb.AppendLine("  , Movimenti.Data_Movimento  ")
            Stb.AppendLine("  , Movimenti.Ora  ")
            'Stb.AppendLine("  , Movimenti.Mov_Desc  ")
            Stb.AppendLine("  , dettagli_farmaci.pro_Cod ")
            Stb.AppendLine("  , dettagli_mat.pro_cod ")
            Stb.AppendLine("  , Farmaci.Denominazione ")
            Stb.AppendLine("  , Materie_Prime.Mat_Des ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("  , Agenda.Username_Creazione  ")
            'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
            Stb.AppendLine("  , Agenda.Blocco_Flag  ")
            Stb.AppendLine("  , g.sa_nome  ")
            Stb.AppendLine("  , i.rag_soc  ")
            Stb.AppendLine("  , Operazioni.lav_des  ")
            Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
            Stb.AppendLine("  , GruppoOperazioni.tipo ")
            Stb.AppendLine("  , Attivita.Sigla ")
            Stb.AppendLine("  , Attivita.[Desc] ")
            Stb.AppendLine("  , Utenti.[user] ")
            Stb.AppendLine("  , g.STA_DES ")
            Stb.AppendLine(" ")

            Stb.AppendLine("  UNION ")


            'Pesatura/Altre Lavorazioni
            Stb.AppendLine(" --Pesatura/Altre Lavorazioni  ")
            Stb.AppendLine(" SELECT  ")
            Stb.AppendLine("     Agenda.Piva   ")
            Stb.AppendLine("   , g.Sa_Cod   ")
            Stb.AppendLine("   , Agenda.Id_Agenda   ")
            Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
            Stb.AppendLine("   , Agenda.Lav_Cod   ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            'Stb.AppendLine("   , Agenda.Des_Lib   ")
            Stb.AppendLine("   , Agenda.Blocco_Flag ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
            Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
            Stb.AppendLine("   , Movimenti.Ora   ")
            'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
            Stb.AppendLine("   , 0 as ID_Mov_Det ")
            Stb.AppendLine("   , '' as Info ")
            Stb.AppendLine("   , '' as Dettagli ")
            Stb.AppendLine("   , 0 as Veg_Cod ")
            Stb.AppendLine("   , Agenda.Username_Creazione   ")
            'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
            Stb.AppendLine("   , Agenda.Blocco_Flag   ")
            Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
            Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
            Stb.AppendLine("   , 0 AS Elem_Cod   ")
            Stb.AppendLine("   , 0 AS Mat_Cod   ")
            Stb.AppendLine("   , 0 AS Pro_Cod   ")
            Stb.AppendLine("   , '' AS Mat_Des   ")
            Stb.AppendLine("   , '' AS Cod_Articolo   ")
            Stb.AppendLine("   , g.sa_nome  ")
            Stb.AppendLine("   , i.rag_soc   ")
            Stb.AppendLine("   , Operazioni.lav_des   ")
            Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
            Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
            Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
            Stb.AppendLine("   , '' as tipo_colore ")
            Stb.AppendLine("   , 0 AS contabilizzato   ")
            Stb.AppendLine("   , '' as LottiProduzione ")
            Stb.AppendLine("   , '' AS Nota_Des   ")
            Stb.AppendLine("   , '' AS Note ")
            Stb.AppendLine("   , '' as Costi_Operatori ")
            Stb.AppendLine("   , '' as Costi_Macchine ")
            Stb.AppendLine("   , 0 as Sup_Trattata ")
            Stb.AppendLine("   , '' as LottiImpianto ")
            Stb.AppendLine("   , '' as Descrizione_Unica ")
            Stb.AppendLine("   , '' as Prodotti ")
            Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
            Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
            Stb.AppendLine("   , '' AS RifDdtFatture  ")
            Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
            Stb.AppendLine("   , IIF(g.STA_DES IS NULL, '' + + 'N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' : ', g.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ')  + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ') As Dettaglio_Tecnico ")
            Stb.AppendLine("   , '' AS Segnalazioni ")
            Stb.AppendLine("   , '' As PermessoModifica ")

            Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
            Stb.AppendLine(" JOIN Movimenti  (NOLOCK) ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
            Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
            Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
            Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
            Stb.AppendLine(" LEFT JOIN Centri_Aziendali (NOLOCK)  ON Agenda.Piva = Centri_Aziendali.Piva AND Agenda.Sa_Cod = Centri_Aziendali.sa_cod ")
            Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
            Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.ID_Agenda = Movimenti_dettagli.Id_Agenda  ")
            Stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
            Stb.AppendLine(" 					AND Movimenti_dettagli.Elem_Cod = 300 ")
            Stb.AppendLine(" LEFT JOIN Zoo_Animali (NOLOCK)  ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            'Stb.AppendLine(" LEFT JOIN giac_cte g ON Zoo_animali.Cod_Progetto = g.Cod_Animale   ")
            'Stb.AppendLine(" 				AND  Movimenti.Data_Movimento >= g.mindata  ")
            'Stb.AppendLine(" 				AND  Movimenti.Data_Movimento <= g.maxData  ")
            'Stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM agn_cte WHERE agn_cte.Cau_Mov = '3700' AND agn_cte.Cod_Progetto = Zoo_Animali.Cod_Progetto ORDER BY agn_cte.Data_Movimento DESC ) g  ")
            Stb.AppendLine(" LEFT JOIN agn_cte g ON g.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
            Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")
            'Stb.AppendLine(" WHERE (Agenda.Lav_Cod >= 3000 AND Agenda.Lav_Cod < 4000 ) ")
            Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3033, 3006, 3007) ) ")
            Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3800', '3550', '3100', '3001')  ")

            If Piva <> "" Then
                Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If FiltroCentri <> "" Then
                Stb.AppendLine(" AND g.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
            End If

            If Sa_Cod <> 0 Then
                Stb.AppendLine(" AND g.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
            Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

            Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
            Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

            Stb.AppendLine("  GROUP BY Agenda.Piva  ")
            Stb.AppendLine("  , g.Sa_Cod  ")
            Stb.AppendLine("  , Agenda.Id_Agenda  ")
            Stb.AppendLine("  , Agenda.Lav_Cod  ")
            'Stb.AppendLine("  , Agenda.Des_Lib  ")
            Stb.AppendLine("  , Movimenti.Data_Movimento  ")
            Stb.AppendLine("  , Movimenti.Ora  ")
            'Stb.AppendLine("  , Movimenti.Mov_Desc  ")
            Stb.AppendLine("  , Agenda.Username_Creazione  ")
            'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
            Stb.AppendLine("  , Agenda.Blocco_Flag  ")
            Stb.AppendLine("  , g.sa_nome ")
            Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
            Stb.AppendLine("  , i.rag_soc  ")
            Stb.AppendLine("  , Operazioni.lav_des  ")
            Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
            Stb.AppendLine("  , GruppoOperazioni.tipo ")
            Stb.AppendLine("  , Attivita.Sigla ")
            Stb.AppendLine("  , Attivita.[Desc] ")
            Stb.AppendLine("  , Utenti.[user] ")
            Stb.AppendLine("  , g.STA_DES  ")



            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri_Server, Stb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Private Function Crea_movsposTT(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_FineGiacenze As Date) As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine("    SELECT ").
    AppendLine("        mdes.Piva, ").
    AppendLine("        mdes.Sa_Cod, ").
    AppendLine("        mdes.Id_Destinazione, ").
    AppendLine("        md.Cod_Progetto, ").
    AppendLine("        m.Cau_Mov, ").
    AppendLine("        m.Id_Mov, ").
    AppendLine("        m.Data_Movimento ").
    AppendLine("        INTO #movspos ").
    AppendLine("    FROM Agenda a (NOLOCK) ").
    AppendLine("    INNER JOIN Movimenti m (NOLOCK) ON m.PIVA = a.PIVA AND m.Id_Agenda = a.Id_Agenda ").
    AppendLine("    INNER JOIN Movimenti_dettagli md (NOLOCK) ON md.PIVA = m.Piva AND md.Id_Agenda = m.Id_Agenda AND md.Id_Mov = m.Id_Mov ").
    AppendLine("    INNER JOIN Mov_Destinazioni mdes (NOLOCK) ON mdes.PIVA = md.Piva AND mdes.Id_Agenda = md.Id_Agenda AND mdes.Id_Mov = md.Id_Mov AND mdes.Id_Mov_Det = md.Id_Mov_Det ").
    AppendLine("    INNER JOIN Zoo_Animali ON md.Cod_Progetto = Zoo_Animali.Cod_Progetto AND md.Piva = Zoo_Animali.Piva AND Zoo_Animali.Sa_Cod = 0 ").
    AppendLine("    WHERE 1=1 ")

        If Piva <> "" Then
            Stb.AppendLine("        AND a.PIVA = " & Agro_SQL_SaveText_NULL(Piva) & " ")
        End If

        If Sa_Cod <> 0 Then
            Stb.AppendLine("        AND mdes.sa_cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        Stb.Append("        AND a.Lav_Cod IN (").Append(Agro_SQL_Save_Clausola_IN(String.
        Join(",", {LAVCOD_NASCITA_ANIMALI, LAVCOD_INCREMENTO_CONSISTENZE_ZOO, LAVCOD_ACQUISTO_ANIMALI, LAVCOD_SPOSTAMENTI_ZOO}.ToArray))).
    AppendLine(") ").
    Append("        AND m.Cau_Mov IN (").Append(CAU_CARICO_CONSISTENZE).AppendLine(") ").
    AppendLine("        AND m.Data_Movimento >= CONVERT(DateTime,'1900/01/01',120) ").
    AppendLine("        AND m.Data_Movimento <=" & Agro_SQL_SaveDate(Validita_FineGiacenze)).
    Append("        AND md.Elem_Cod = ").Append(ZOO_CONSISTENZA).AppendLine(" ").
    AppendLine("        AND md.Jolly_Int = 0 ").
    AppendLine("        AND Zoo_Animali.Validita_Fine >=  " & Agro_SQL_SaveDate(Validita_Inizio) & " ").
    Append("        AND mdes.Tipo_Destinazione IN (").Append(TIPO_DESTINAZIONE_RAGGRUPPAMENTO_STALLA).AppendLine(") ")

        Stb.AppendLine(" ALTER TABLE #movspos alter column Piva nvarchar(25) NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #movspos alter column Cod_Progetto int NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #movspos alter column Id_Mov int NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #movspos ADD PRIMARY KEY CLUSTERED (Piva, Cod_Progetto, Id_Mov) ")

        Return Stb.ToString
    End Function

    Private Function Crea_lastmovsposTT() As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine("    SELECT ").
                AppendLine("        movspos.PIVA, ").
                AppendLine("        movspos.Sa_Cod, ").
                AppendLine("        movspos.Id_Destinazione, ").
                AppendLine("        movspos.Cod_Progetto, ").
                AppendLine("        movspos.Cau_Mov, ").
                AppendLine("        movspos.Data_Movimento, ").
                AppendLine("        ROW_NUMBER() OVER (PARTITION BY movspos.Cod_Progetto ORDER BY movspos.Data_Movimento DESC) as rn ").
                AppendLine("        INTO #last_movspos ").
                AppendLine("    FROM #movspos movspos ")

        Stb.AppendLine(" ALTER TABLE #last_movspos alter column Cod_Progetto int NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #last_movspos alter column rn int NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #last_movspos ADD PRIMARY KEY CLUSTERED (Cod_Progetto, rn) ")

        Return Stb.ToString
    End Function

    Private Function Crea_agnTT() As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine("    SELECT ").
                AppendLine("        last_movspos.Cod_Progetto, ").
                AppendLine("        last_movspos.Sa_Cod, ").
                AppendLine("        last_movspos.Cau_Mov, ").
                AppendLine("        ca.sa_nome, ").
                AppendLine("        last_movspos.Data_Movimento, ").
                AppendLine("        sta.STA_NUM, ").
                AppendLine("        sta.STA_DES ").
                AppendLine("    INTO #agn_cte ").
                AppendLine("    FROM #last_movspos last_movspos ").
                AppendLine("    INNER JOIN Centri_Aziendali ca (NOLOCK) ON ca.PIVA = last_movspos.Piva COLLATE SQL_Latin1_General_CP850_CI_AS AND ca.sa_cod = last_movspos.Sa_Cod ").
                AppendLine("    INNER JOIN Stalla_Raggruppamenti stra (NOLOCK) ON last_movspos.Id_Destinazione = stra.Raggruppamento_Cod ").
                AppendLine("    INNER JOIN Stalla sta (NOLOCK) ON sta.PIVA = stra.PIVA AND sta.sa_cod= stra.sa_cod AND sta.STA_NUM = stra.STA_NUM ").
                AppendLine("    WHERE last_movspos.rn = 1 ")

        Stb.AppendLine(" ALTER TABLE #agn_cte alter column Cod_Progetto int NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #agn_cte alter column Data_Movimento datetime NOT NULL ")
        Stb.AppendLine(" ALTER TABLE #agn_cte ADD PRIMARY KEY CLUSTERED (Cod_Progetto, Data_Movimento) ")

        Return Stb.ToString
    End Function

    Private Function Crea_CarichiScarichi(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal FiltroCentri As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine(" SELECT  ")
        Stb.AppendLine("     Agenda.Piva   ")
        Stb.AppendLine("   , Centri_Aziendali.Sa_Cod   ")
        Stb.AppendLine("   , Agenda.Id_Agenda   ")
        Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
        Stb.AppendLine("   , Agenda.Lav_Cod   ")
        'Stb.AppendLine("   , Agenda.Des_Lib   ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("   , Agenda.Blocco_Flag ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
        Stb.AppendLine("   , Movimenti.Ora   ")
        'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
        Stb.AppendLine("   , 0 as ID_Mov_Det ")
        Stb.AppendLine("   , '' as Info ")
        Stb.AppendLine("   , '' as Dettagli ")
        Stb.AppendLine("   , 0 as Veg_Cod ")
        Stb.AppendLine("   , Agenda.Username_Creazione   ")
        'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
        Stb.AppendLine("   , Agenda.Blocco_Flag   ")
        Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
        Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
        Stb.AppendLine("   , 0 AS Elem_Cod   ")
        Stb.AppendLine("   , 0 AS Mat_Cod   ")
        Stb.AppendLine("   , 0 AS Pro_Cod   ")
        Stb.AppendLine("   , '' AS Mat_Des   ")
        Stb.AppendLine("   , '' AS Cod_Articolo   ")
        Stb.AppendLine("   , Centri_Aziendali.sa_nome  ")
        Stb.AppendLine("   , i.rag_soc   ")
        Stb.AppendLine("   , Operazioni.lav_des   ")
        Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
        Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
        Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
        Stb.AppendLine("   , '' as tipo_colore ")
        Stb.AppendLine("   , 0 AS contabilizzato   ")
        Stb.AppendLine("   , '' as LottiProduzione ")
        Stb.AppendLine("   , '' AS Nota_Des   ")
        Stb.AppendLine("   , '' AS Note ")
        Stb.AppendLine("   , '' as Costi_Operatori ")
        Stb.AppendLine("   , '' as Costi_Macchine ")
        Stb.AppendLine("   , 0 as Sup_Trattata ")
        Stb.AppendLine("   , '' as LottiImpianto ")
        Stb.AppendLine("   , '' as Descrizione_Unica ")
        Stb.AppendLine("   , '' as Prodotti ")
        Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
        Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
        Stb.AppendLine("   , '' AS RifDdtFatture  ")
        Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
        'Stb.AppendLine("   , STRING_AGG(Zoo_Animali.Matricola, ', ') As Dettaglio_Tecnico ")
        Stb.AppendLine("   , IIF(Mov_Lav.Extra_Str IS NULL OR TRIM(Mov_Lav.Extra_Str) = '', Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', '), Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + COALESCE(Mov_Lav.Extra_Str + ': ', '') + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ')) As Dettaglio_Tecnico ")
        Stb.AppendLine("   , '' AS Segnalazioni ")
        Stb.AppendLine("   , '' As PermessoModifica ")

        Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
        Stb.AppendLine(" JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
        Stb.AppendLine(" LEFT JOIN Movimenti (NOLOCK)  Mov_Lav ON Agenda.Id_Agenda = Mov_Lav.ID_Agenda AND Mov_Lav.Cau_Mov = '4000' ")
        Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
        Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
        Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
        Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
        Stb.AppendLine(" JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.ID_Agenda = Movimenti_dettagli.Id_Agenda  ")
        Stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Elem_Cod = 300 ")
        Stb.AppendLine(" JOIN Mov_Destinazioni  (NOLOCK) ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
        Stb.AppendLine(" 					AND Mov_Destinazioni.Tipo_Destinazione = 21 ")
        Stb.AppendLine("  JOIN Stalla_Raggruppamenti (NOLOCK)  ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
        Stb.AppendLine("  JOIN Stalla (NOLOCK)  ON Stalla_Raggruppamenti.Piva = Stalla.PIVA ")
        Stb.AppendLine(" 			AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
        Stb.AppendLine(" 			AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
        Stb.AppendLine(" JOIN Centri_Aziendali (NOLOCK)  ON Stalla.Piva = Centri_Aziendali.Piva AND Stalla.Sa_Cod = Centri_Aziendali.sa_cod ")
        Stb.AppendLine(" JOIN Zoo_Animali (NOLOCK)  ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
        Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")

        Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3000, 3001, 3002, 3003, 3004, 3004, 3034,3035,3037))  ")
        Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3700','3750')  ")

        If Piva <> "" Then
            Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        If FiltroCentri <> "" Then
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
        End If

        If Sa_Cod <> 0 Then
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))


        Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))


        Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

        Stb.AppendLine("  GROUP BY Agenda.Piva  ")
        Stb.AppendLine("  , Centri_Aziendali.Sa_Cod  ")
        Stb.AppendLine("  , Agenda.Id_Agenda  ")
        Stb.AppendLine("  , Agenda.Lav_Cod  ")
        'Stb.AppendLine("  , Agenda.Des_Lib  ")
        Stb.AppendLine("  , Movimenti.Data_Movimento  ")
        Stb.AppendLine("  , Movimenti.Ora  ")
        Stb.AppendLine("  , Mov_Lav.Extra_Str ")
        Stb.AppendLine("  , Agenda.Username_Creazione  ")
        'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
        Stb.AppendLine("  , Agenda.Blocco_Flag  ")
        Stb.AppendLine("  , Centri_Aziendali.sa_nome ")
        Stb.AppendLine("  , i.rag_soc  ")
        Stb.AppendLine("  , Operazioni.lav_des  ")
        Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
        Stb.AppendLine("  , GruppoOperazioni.tipo ")
        Stb.AppendLine("  , Attivita.Sigla ")
        Stb.AppendLine("  , Attivita.[Desc] ")
        Stb.AppendLine("  , Utenti.[user] ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("   , Stalla.STA_DES ")
        Stb.AppendLine("   ")
        Return Stb.ToString
    End Function


    Private Function Crea_Spostamenti() As String
        Dim Stb As New System.Text.StringBuilder

        Stb.AppendLine(" SELECT PIVA, sa_cod, Id_Agenda, ID, Lav_Cod, Tipo_Accettazione, Blocco_Flag,   ")
        Stb.AppendLine(" --Tipo_Destinazione,  ")
        Stb.AppendLine(" [Data], Data2, Ora, ID_Mov_Det, Info, Dettagli, Veg_Cod,  ")
        Stb.AppendLine(" CAST(Username_Creazione as nvarchar(25)) as Username_Creazione,  ")
        Stb.AppendLine(" Blocco_Flag,  ")
        Stb.AppendLine(" Tecnico, Tipo_Accettazione, Elem_Cod, Mat_Cod, Pro_Cod, Mat_Des, Cod_Articolo, sa_nome, rag_soc, LAV_DES, Operazione_DES, GRU_DES, tipo, tipo_colore, contabilizzato, LottiProduzione, Nota_Des, Note,  ")
        Stb.AppendLine(" Costi_Operatori, Costi_Macchine, Sup_Trattata, LottiImpianto, Descrizione_Unica, Prodotti, AttivitaSigla, AttivitaDesc, RifDdtFatture,  ")
        Stb.AppendLine(" Creatore_Intervento,   ")
        Stb.AppendLine(" STA_DES + ' - Destinazione: ' + STRING_AGG(Raggruppamento_Des, ',') + ' ' + ' (N. Capi:' + CAST(SUM(NumeroCapi) as varchar(250)) + ' ) ' + ': ' + STRING_AGG(CAST(Matricole as nvarchar(max)), ', '),  ")
        Stb.AppendLine(" Segnalazioni, PermessoModifica ")
        Stb.AppendLine(" FROM #spostamenti ")
        Stb.AppendLine(" GROUP BY PIVA, sa_cod, Id_Agenda, ID, lav_Cod, Tipo_Accettazione, Blocco_Flag, Tipo_Destinazione, [Data], Data2, Ora, ID_Mov_Det, Info, Dettagli, Veg_Cod, Username_Creazione, Tecnico, Tipo_Accettazione, Elem_Cod, Mat_Cod, Pro_Cod, Mat_Des, Cod_Articolo, sa_nome, rag_soc, LAV_DES, Operazione_DES, GRU_DES, tipo, tipo_colore, contabilizzato, LottiProduzione, Nota_Des, Note, Costi_Operatori, Costi_Macchine, Sup_Trattata, LottiImpianto, Descrizione_Unica, Prodotti, AttivitaSigla, AttivitaDesc, RifDdtFatture, Creatore_Intervento, STA_DES, Segnalazioni, PermessoModifica ")

        Return Stb.ToString
    End Function

    Private Function Crea_ProdottiAggreagatiCTE(piva As String, Validita_Inizio As Date, Validita_Fine As Date, ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim Stb As New System.Text.StringBuilder()

        Stb.AppendLine(";WITH ProdottiDistinti AS (")
        Stb.AppendLine("  SELECT DISTINCT")
        Stb.AppendLine("    m.Id_Agenda,")
        Stb.AppendLine("    m.Id_Mov,")
        Stb.AppendLine("    COALESCE(f.Denominazione COLLATE DATABASE_DEFAULT, mp.Mat_Des COLLATE DATABASE_DEFAULT, '') AS Prodotto")
        Stb.AppendLine("  FROM Movimenti m")
        Stb.AppendLine("  INNER JOIN Agenda a ON m.Id_Agenda = a.Id_Agenda")
        Stb.AppendLine("  LEFT JOIN Movimenti_dettagli md ON m.ID_Agenda = md.Id_Agenda AND m.Id_Mov = md.Id_Mov")
        Stb.AppendLine("  LEFT JOIN Farmaci f ON md.Pro_Cod = f.Farm_Cod AND md.Elem_Cod = 307")
        Stb.AppendLine("  LEFT JOIN Materie_Prime mp ON md.Mat_Cod = mp.Mat_Cod AND md.Elem_Cod <> 307")
        Stb.AppendLine("  WHERE COALESCE(f.Denominazione COLLATE DATABASE_DEFAULT, mp.Mat_Des COLLATE DATABASE_DEFAULT, '') <> ''")
        Stb.AppendLine("    AND a.PIVA = '" & Agro_SQL_SaveText(piva) & "'")
        Stb.AppendLine("    AND a.Lav_Cod IN (3005,3008,3009,3010,3011,3012,3013,3014,")
        Stb.AppendLine("                      3015,3016,3017,3018,3019,3020,3021,3022,3023,3024,3025,3026,3027,3028,3029,")
        Stb.AppendLine("                      3031,3032,3036,3037)")
        Stb.AppendLine("    AND m.Cau_Mov IN ('3860','3450','3200', '3850')")
        Stb.AppendLine("    AND CAST(m.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
        Stb.AppendLine("    AND CAST(m.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))
        Stb.AppendLine("    AND m.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))
        Stb.AppendLine("    AND m.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))
        Stb.AppendLine(" GROUP BY m.PIVA, m.Id_Agenda, m.Id_Mov, f.Denominazione, mp.Mat_Des ")
        Stb.AppendLine("),")
        Stb.AppendLine("ProdottiAggregati AS (")
        Stb.AppendLine("  SELECT")
        Stb.AppendLine("    Id_Agenda,")
        Stb.AppendLine("    Id_Mov,")
        Stb.AppendLine("    STRING_AGG(Prodotto, ', ') AS ListaProdotti  ")
        Stb.AppendLine("  FROM ProdottiDistinti")
        Stb.AppendLine("  GROUP BY Id_Agenda, Id_Mov")
        Stb.AppendLine(")")

        Return Stb.ToString()
    End Function
    Private Function Crea_SpostamentiTT(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal FiltroCentri As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine(" SELECT  ")
        Stb.AppendLine("     Agenda.Piva   ")
        Stb.AppendLine("   , Centri_Aziendali.Sa_Cod   ")
        Stb.AppendLine("   , Agenda.Id_Agenda   ")
        Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
        Stb.AppendLine("   , Agenda.Lav_Cod   ")
        'Stb.AppendLine("   , Agenda.Des_Lib   ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("   , Agenda.Blocco_Flag ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
        Stb.AppendLine("   , Movimenti.Ora   ")
        'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
        Stb.AppendLine("   , 0 as ID_Mov_Det ")
        Stb.AppendLine("   , '' as Info ")
        Stb.AppendLine("   , '' as Dettagli ")
        Stb.AppendLine("   , 0 as Veg_Cod ")
        Stb.AppendLine("   , Agenda.Username_Creazione   ")
        'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
        'Stb.AppendLine("   , Agenda.Blocco_Flag   ")
        Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
        Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
        Stb.AppendLine("   , 0 AS Elem_Cod   ")
        Stb.AppendLine("   , 0 AS Mat_Cod   ")
        Stb.AppendLine("   , 0 AS Pro_Cod   ")
        Stb.AppendLine("   , '' AS Mat_Des   ")
        Stb.AppendLine("   , '' AS Cod_Articolo   ")
        Stb.AppendLine("   , Centri_Aziendali.sa_nome  ")
        Stb.AppendLine("   , i.rag_soc   ")
        Stb.AppendLine("   , Operazioni.lav_des   ")
        Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
        Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
        Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
        Stb.AppendLine("   , '' as tipo_colore ")
        Stb.AppendLine("   , 0 AS contabilizzato   ")
        Stb.AppendLine("   , '' as LottiProduzione ")
        Stb.AppendLine("   , '' AS Nota_Des   ")
        Stb.AppendLine("   , '' AS Note ")
        Stb.AppendLine("   , '' as Costi_Operatori ")
        Stb.AppendLine("   , '' as Costi_Macchine ")
        Stb.AppendLine("   , 0 as Sup_Trattata ")
        Stb.AppendLine("   , '' as LottiImpianto ")
        Stb.AppendLine("   , '' as Descrizione_Unica ")
        Stb.AppendLine("   , '' as Prodotti ")
        Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
        Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
        Stb.AppendLine("   , '' AS RifDdtFatture  ")
        Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
        Stb.AppendLine("   , Stalla.STA_DES ")
        Stb.AppendLine("   , Stalla_Raggruppamenti.Raggruppamento_Des ")
        Stb.AppendLine("   , STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ') as Matricole ")
        'Stb.AppendLine("   , STRING_AGG(Zoo_Animali.Matricola, ', ') As Dettaglio_Tecnico ")
        'Stb.AppendLine("   , IIF(Mov_Lav.Extra_Str IS NULL OR TRIM(Mov_Lav.Extra_Str) = '', Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', '), Stalla.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ' + COALESCE(Mov_Lav.Extra_Str + ': ', '') + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ')) As Dettaglio_Tecnico ")
        Stb.AppendLine("   , STRING_AGG(CAST(IIF(Zoo_Animali.Matricola IS NULL, Movimenti_dettagli.Mov_Det_Des, NULL) AS NVARCHAR(MAX)), ', ') AS Segnalazioni ")
        Stb.AppendLine("   , '' As PermessoModifica ")
        Stb.AppendLine("   , COUNT(*) as NumeroCapi ")
        Stb.AppendLine("   INTO #spostamenti ")
        Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
        Stb.AppendLine(" JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
        Stb.AppendLine(" LEFT JOIN Movimenti (NOLOCK)  Mov_Lav ON Agenda.Id_Agenda = Mov_Lav.ID_Agenda AND Mov_Lav.Cau_Mov = '4000' ")
        Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
        Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
        Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
        Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
        Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.ID_Agenda = Movimenti_dettagli.Id_Agenda  ")
        Stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Elem_Cod = 300 ")
        Stb.AppendLine(" JOIN Mov_Destinazioni (NOLOCK)  ON Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
        Stb.AppendLine(" 					AND Mov_Destinazioni.Tipo_Destinazione = 21 ")
        Stb.AppendLine("  JOIN Stalla_Raggruppamenti (NOLOCK)  ON Mov_Destinazioni.Id_Destinazione = Stalla_Raggruppamenti.Raggruppamento_Cod ")
        Stb.AppendLine("  JOIN Stalla (NOLOCK)  ON Stalla_Raggruppamenti.Piva = Stalla.PIVA ")
        Stb.AppendLine(" 			AND Stalla_Raggruppamenti.sa_cod = Stalla.sa_cod ")
        Stb.AppendLine(" 			AND Stalla_Raggruppamenti.STA_NUM = Stalla.STA_NUM ")
        Stb.AppendLine(" JOIN Centri_Aziendali (NOLOCK)  ON Stalla.Piva = Centri_Aziendali.Piva AND Stalla.Sa_Cod = Centri_Aziendali.sa_cod ")
        Stb.AppendLine(" LEFT JOIN Zoo_Animali (NOLOCK)  ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
        Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")

        Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3030))  ")
        Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3700')  ")

        If Piva <> "" Then
            Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        If FiltroCentri <> "" Then
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
        End If

        If Sa_Cod <> 0 Then
            Stb.AppendLine(" AND Centri_Aziendali.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

        Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))

        Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

        Stb.AppendLine("  GROUP BY Agenda.Piva  ")
        Stb.AppendLine("  , Centri_Aziendali.Sa_Cod  ")
        Stb.AppendLine("  , Agenda.Id_Agenda  ")
        Stb.AppendLine("  , Agenda.Lav_Cod  ")
        'Stb.AppendLine("  , Agenda.Des_Lib  ")
        Stb.AppendLine("  , Movimenti.Data_Movimento  ")
        Stb.AppendLine("  , Movimenti.Ora  ")
        Stb.AppendLine("  , Mov_Lav.Extra_Str ")
        Stb.AppendLine("  , Agenda.Username_Creazione  ")
        'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
        Stb.AppendLine("  , Agenda.Blocco_Flag  ")
        Stb.AppendLine("  , Centri_Aziendali.sa_nome ")
        Stb.AppendLine("  , i.rag_soc  ")
        Stb.AppendLine("  , Operazioni.lav_des  ")
        Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
        Stb.AppendLine("  , GruppoOperazioni.tipo ")
        Stb.AppendLine("  , Attivita.Sigla ")
        Stb.AppendLine("  , Attivita.[Desc] ")
        Stb.AppendLine("  , Utenti.[user] ")
        Stb.AppendLine("  , Stalla_Raggruppamenti.Raggruppamento_Des ")
        Stb.AppendLine("  , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("  , Stalla.STA_DES ")
        Stb.AppendLine("   ")
        Return Stb.ToString
    End Function

    Private Function Crea_Trattamenti(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal FiltroCentri As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine(" SELECT  ")
        Stb.AppendLine("     Agenda.Piva   ")
        Stb.AppendLine("   , g.Sa_Cod   ")
        Stb.AppendLine("   , Agenda.Id_Agenda   ")
        Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
        Stb.AppendLine("   , Agenda.Lav_Cod   ")
        'Stb.AppendLine("   , Agenda.Des_Lib   ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("   , Agenda.Blocco_Flag ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
        Stb.AppendLine("   , Movimenti.Ora   ")
        'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
        Stb.AppendLine("   , 0 as ID_Mov_Det ")
        Stb.AppendLine("   , '' as Info ")
        Stb.AppendLine("   , '' as Dettagli ")
        Stb.AppendLine("   , 0 as Veg_Cod ")
        Stb.AppendLine("   , Agenda.Username_Creazione   ")
        'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
        Stb.AppendLine("   , Agenda.Blocco_Flag   ")
        Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
        Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
        Stb.AppendLine("   , 0 AS Elem_Cod   ")
        Stb.AppendLine("   , 0 AS Mat_Cod   ")

        'Stb.AppendLine("   , COALESCE(dettagli_farmaci.pro_Cod, dettagli_mat.pro_cod, 0) AS Pro_Cod   ")
        Stb.AppendLine("   , COALESCE(dettagli_mat.pro_cod, 0) AS Pro_Cod   ")

        'Stb.AppendLine("   , COALESCE(Farmaci.Denominazione, Materie_Prime.Mat_Des, '') AS Mat_Des   ")
        '        Stb.AppendLine("   , STRING_AGG(CAST(COALESCE(Farmaci.Denominazione, Materie_Prime.Mat_Des, '') AS nvarchar(max)), ', ') AS Mat_Des   ")
        Stb.AppendLine("  , ISNULL(pa.ListaProdotti, '') AS Mat_Des   ")
        Stb.AppendLine("   , '' AS Cod_Articolo   ")
        Stb.AppendLine("   , g.sa_nome  ")
        Stb.AppendLine("   , i.rag_soc   ")
        Stb.AppendLine("   , Operazioni.lav_des   ")
        Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
        Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
        Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
        Stb.AppendLine("   , '' as tipo_colore ")
        Stb.AppendLine("   , 0 AS contabilizzato   ")
        Stb.AppendLine("   , '' as LottiProduzione ")
        Stb.AppendLine("   , '' AS Nota_Des   ")
        Stb.AppendLine("   , '' AS Note ")
        Stb.AppendLine("   , '' as Costi_Operatori ")
        Stb.AppendLine("   , '' as Costi_Macchine ")
        Stb.AppendLine("   , 0 as Sup_Trattata ")
        Stb.AppendLine("   , '' as LottiImpianto ")
        Stb.AppendLine("   , '' as Descrizione_Unica ")
        'Stb.AppendLine("   , COALESCE(Farmaci.Denominazione, Materie_Prime.Mat_Des, '') as Prodotti  ")
        '        Stb.AppendLine("   , STRING_AGG(CAST(COALESCE(Farmaci.Denominazione, Materie_Prime.Mat_Des, '') AS nvarchar(max)), ', ') as Prodotti ")
        Stb.appendline("  , ISNULL(pa.ListaProdotti, '') as Prodotti ")
        Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
        Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
        Stb.AppendLine("   , '' AS RifDdtFatture  ")
        Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")

        'Stb.AppendLine("   , IIF(g.STA_DES IS NULL, '', g.STA_DES + ' (N. Capi:' + CAST(COUNT(DISTINCT Zoo_Animali.Cod_Progetto) as varchar(250)) + ' ) ' + ': ')  + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ') as Dettaglio_Tecnico ")
        Stb.AppendLine("   , IIF(g.STA_DES IS NULL, '', g.STA_DES ")
        Stb.AppendLine("       + ' (N. Capi:' + CAST(COUNT(DISTINCT Zoo_Animali.Cod_Progetto) as varchar(250)) ")
        Stb.AppendLine("       + ' ) ' + ': ') ")
        Stb.AppendLine("       + STUFF( ")
        Stb.AppendLine("           (SELECT DISTINCT ', ' + CAST(za_inner.Matricola AS NVARCHAR(MAX)) ")
        Stb.AppendLine("           FROM Mov_Destinazioni md_inner (NOLOCK) ")
        Stb.AppendLine("           JOIN Zoo_Animali za_inner (NOLOCK) ")
        Stb.AppendLine("               ON md_inner.ID_Destinazione = za_inner.Cod_Progetto ")
        Stb.AppendLine("           LEFT JOIN #agn_cte g_inner ")
        Stb.AppendLine("               ON g_inner.Cod_Progetto = za_inner.Cod_Progetto ")
        Stb.AppendLine("           WHERE md_inner.Id_Agenda = Agenda.Id_Agenda ")
        Stb.AppendLine("               AND md_inner.Id_Mov = Movimenti.Id_Mov ")
        Stb.AppendLine("               AND ISNULL(g_inner.STA_DES, '') = ISNULL(g.STA_DES, '') ")
        Stb.AppendLine("               AND md_inner.Tipo_Destinazione = 1 ")
        Stb.AppendLine("               AND md_inner.Id_Destinazione > 0 ")
        Stb.AppendLine("           ORDER BY ', ' + CAST(za_inner.Matricola AS NVARCHAR(MAX)) ")
        Stb.AppendLine("           FOR XML PATH(''), TYPE ")
        Stb.AppendLine("       ).value('.', 'NVARCHAR(MAX)'), 1, 2, '' ")
        Stb.AppendLine("    ) AS Dettaglio_Tecnico")

        'stb.AppendLine("  IIF(g.STA_DES IS NULL, '', g.STA_DES + ' (N. Capi:' + CAST(COUNT(DISTINCT Zoo_Animali.Cod_Progetto) as varchar(250)) + ' ) ' + ': ')  
        '                     + (SELECT STRING_AGG(CAST(T.Matricola as nvarchar(max)), ', ')
        '                        FROM (SELECT DISTINCT Zoo_Animali.Matricola FROM Zoo_Animali) AS T) as Dettaglio_Tecnico")
        Stb.AppendLine("   , '' AS Segnalazioni ")
        Stb.AppendLine("   , '' As PermessoModifica ")

        Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
        Stb.AppendLine(" JOIN Movimenti (NOLOCK)  ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
        Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
        Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
        Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
        Stb.AppendLine(" LEFT JOIN Centri_Aziendali (NOLOCK)  ON Agenda.Piva = Centri_Aziendali.Piva AND Agenda.Sa_Cod = Centri_Aziendali.sa_cod ")
        Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
        Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  dettagli_farmaci ON Movimenti.ID_Agenda = dettagli_farmaci.Id_Agenda  ")
        Stb.AppendLine(" 					AND Movimenti.Id_Mov = dettagli_farmaci.Id_Mov ")
        Stb.AppendLine(" 					AND dettagli_farmaci.Elem_Cod = 307 ")
        Stb.AppendLine(" LEFT JOIN Farmaci (NOLOCK)  ON dettagli_farmaci.Pro_Cod = Farmaci.Farm_Cod ")
        Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  dettagli_mat ON Movimenti.ID_Agenda = dettagli_mat.Id_Agenda  ")
        Stb.AppendLine(" 					AND Movimenti.Id_Mov = dettagli_mat.Id_Mov ")
        Stb.AppendLine(" 					AND dettagli_mat.Elem_Cod <> 307 ")
        Stb.AppendLine(" LEFT JOIN Materie_Prime (NOLOCK)  ON dettagli_mat.Mat_Cod = Materie_Prime.Mat_Cod ")
        Stb.AppendLine(" LEFT JOIN Mov_Destinazioni (NOLOCK)  ON Movimenti.ID_Agenda = Mov_Destinazioni.Id_Agenda  ")
        Stb.AppendLine(" 					AND Movimenti.Id_Mov = Mov_Destinazioni.Id_Mov ")
        Stb.AppendLine(" 					AND Mov_Destinazioni.Tipo_Destinazione = 1 ")
        Stb.AppendLine(" 					AND Mov_Destinazioni.Id_Destinazione > 0 ")
        Stb.AppendLine(" LEFT JOIN Zoo_Animali (NOLOCK)  ON Mov_Destinazioni.ID_Destinazione = Zoo_Animali.Cod_Progetto ")
        stb.AppendLine("LEFT JOIN ProdottiAggregati pa ON Movimenti.ID_Agenda = pa.Id_Agenda AND Movimenti.Id_Mov = pa.Id_Mov")
        'Stb.AppendLine(" LEFT JOIN giac_cte g ON Zoo_animali.Cod_Progetto = g.Cod_Animale  ")
        'Stb.AppendLine("				AND  Movimenti.Data_Movimento >= g.mindata ")
        'Stb.AppendLine("				AND  Movimenti.Data_Movimento <= g.maxData ")
        'Stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM agn_cte WHERE agn_cte.Cau_Mov = '3700' AND agn_cte.Cod_Progetto = Zoo_Animali.Cod_Progetto ORDER BY agn_cte.Data_Movimento DESC ) g  ")
        Stb.AppendLine(" LEFT JOIN #agn_cte g ON g.Cod_Progetto = Zoo_Animali.Cod_Progetto  ")
        Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")

        Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3005,3008,3009,3010,3011,3012,3013,3014, ")
        Stb.AppendLine(" 3015,3016,3017,3018,3019,3020,3021,3022,3023,3024,3025,3026,3027,3028,3029, ")
        Stb.AppendLine(" 3031,3032,3036,3037))  ")
        Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3860','3450','3200', '3850')  ")

        If Piva <> "" Then
            Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        If FiltroCentri <> "" Then
            Stb.AppendLine(" AND g.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
        End If

        If Sa_Cod <> 0 Then
            Stb.AppendLine(" AND g.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

        Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))

        Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

        Stb.AppendLine("  GROUP BY Agenda.Piva  ")
        Stb.AppendLine("  , g.Sa_Cod  ")
        Stb.AppendLine("  , Agenda.Id_Agenda  ")
        Stb.AppendLine("  , Agenda.Lav_Cod  ")
        'Stb.AppendLine("  , Agenda.Des_Lib  ")
        Stb.AppendLine("  , Movimenti.Data_Movimento  ")
        Stb.AppendLine("  , Movimenti.Ora  ")
        'Stb.AppendLine("  , Movimenti.Mov_Desc  ")
        'Stb.AppendLine("  , dettagli_farmaci.pro_Cod ")
        Stb.AppendLine("  , dettagli_mat.pro_cod ")
        'Stb.AppendLine("  , Farmaci.Denominazione ")
        'Stb.AppendLine("  , Materie_Prime.Mat_Des ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("  , Agenda.Username_Creazione  ")
        'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
        Stb.AppendLine("  , Agenda.Blocco_Flag  ")
        Stb.AppendLine("  , g.sa_nome  ")
        Stb.AppendLine("  , i.rag_soc  ")
        Stb.AppendLine("  , Operazioni.lav_des  ")
        Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
        Stb.AppendLine("  , GruppoOperazioni.tipo ")
        Stb.AppendLine("  , Attivita.Sigla ")
        Stb.AppendLine("  , Attivita.[Desc] ")
        Stb.AppendLine("  , Utenti.[user] ")
        Stb.AppendLine("  , g.STA_DES ")
        Stb.AppendLine(" , pa.ListaProdotti  ")
        Stb.AppendLine(" , Movimenti.PIVA  ")
        Stb.AppendLine(" , Movimenti.Id_Agenda  ")
        Stb.AppendLine(" , Movimenti.Id_Mov  ")
        Stb.AppendLine(" ")
        Return Stb.ToString
    End Function

    Private Function Crea_AltreLavorazioni(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal FiltroCentri As String,
                                          ByRef objParametri_Server As AgronicaCoreParametri) As String
        Dim Stb As New System.Text.StringBuilder
        Stb.AppendLine(" SELECT  ")
        Stb.AppendLine("     Agenda.Piva   ")
        Stb.AppendLine("   , g.Sa_Cod   ")
        Stb.AppendLine("   , Agenda.Id_Agenda   ")
        Stb.AppendLine("   , Agenda.Id_Agenda  as ID ")
        Stb.AppendLine("   , Agenda.Lav_Cod   ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        'Stb.AppendLine("   , Agenda.Des_Lib   ")
        Stb.AppendLine("   , Agenda.Blocco_Flag ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data] ")
        Stb.AppendLine("   , Movimenti.Data_Movimento as [Data2] ")
        Stb.AppendLine("   , Movimenti.Ora   ")
        'Stb.AppendLine("   , Movimenti.Mov_Desc   ")
        Stb.AppendLine("   , 0 as ID_Mov_Det ")
        Stb.AppendLine("   , '' as Info ")
        Stb.AppendLine("   , '' as Dettagli ")
        Stb.AppendLine("   , 0 as Veg_Cod ")
        Stb.AppendLine("   , Agenda.Username_Creazione   ")
        'Stb.AppendLine("   , Movimenti.Cau_Mov   ")
        Stb.AppendLine("   , Agenda.Blocco_Flag   ")
        Stb.AppendLine("   , 'N.D.' AS Tecnico   ")
        Stb.AppendLine("   , -1 AS Tipo_Destinazione   ")
        Stb.AppendLine("   , 0 AS Elem_Cod   ")
        Stb.AppendLine("   , 0 AS Mat_Cod   ")
        Stb.AppendLine("   , 0 AS Pro_Cod   ")
        Stb.AppendLine("   , '' AS Mat_Des   ")
        Stb.AppendLine("   , '' AS Cod_Articolo   ")
        Stb.AppendLine("   , g.sa_nome  ")
        Stb.AppendLine("   , i.rag_soc   ")
        Stb.AppendLine("   , Operazioni.lav_des   ")
        Stb.AppendLine("   , Operazioni.lav_des  as [Operazione_DES] ")
        Stb.AppendLine("   , GruppoOperazioni.gru_Des   ")
        Stb.AppendLine("   , ISNULL(GruppoOperazioni.tipo, 'C') as tipo   ")
        Stb.AppendLine("   , '' as tipo_colore ")
        Stb.AppendLine("   , 0 AS contabilizzato   ")
        Stb.AppendLine("   , '' as LottiProduzione ")
        Stb.AppendLine("   , '' AS Nota_Des   ")
        Stb.AppendLine("   , '' AS Note ")
        Stb.AppendLine("   , '' as Costi_Operatori ")
        Stb.AppendLine("   , '' as Costi_Macchine ")
        Stb.AppendLine("   , 0 as Sup_Trattata ")
        Stb.AppendLine("   , '' as LottiImpianto ")
        Stb.AppendLine("   , '' as Descrizione_Unica ")
        Stb.AppendLine("   , '' as Prodotti ")
        Stb.AppendLine("   , ISNULL(Attivita.Sigla, '') AS AttivitaSigla   ")
        Stb.AppendLine("   , ISNULL(Attivita.[Desc], '') AS AttivitaDesc   ")
        Stb.AppendLine("   , '' AS RifDdtFatture  ")
        Stb.AppendLine("   , Utenti.[user] as Creatore_Intervento ")
        Stb.AppendLine("   , IIF(g.STA_DES IS NULL, '' + + 'N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' : ', g.STA_DES + ' (N. Capi:' + CAST(COUNT(*) as varchar(250)) + ' ) ' + ': ')  + STRING_AGG(CAST(Zoo_Animali.Matricola as nvarchar(max)), ', ') As Dettaglio_Tecnico ")
        Stb.AppendLine("   , '' AS Segnalazioni ")
        Stb.AppendLine("   , '' As PermessoModifica ")

        Stb.AppendLine(" FROM Agenda (NOLOCK)  ")
        Stb.AppendLine(" JOIN Movimenti  (NOLOCK) ON Agenda.Id_Agenda = Movimenti.ID_Agenda ")
        Stb.AppendLine(" JOIN dbo.Operazioni (NOLOCK)  On Agenda.Lav_Cod = Operazioni.Lav_Cod ")
        Stb.AppendLine(" JOIN dbo.GruppoOperazioni (NOLOCK)  On Operazioni.GRU_OP = GruppoOperazioni.GRU_COD ")
        Stb.AppendLine(" JOIN imprese (NOLOCK)  i On i.piva = Agenda.PIVA ")
        Stb.AppendLine(" LEFT JOIN Centri_Aziendali (NOLOCK)  ON Agenda.Piva = Centri_Aziendali.Piva AND Agenda.Sa_Cod = Centri_Aziendali.sa_cod ")
        Stb.AppendLine(" LEFT JOIN Attivita (NOLOCK)  ON Agenda.Id_Attivita = Attivita.Id_Attivita ")
        Stb.AppendLine(" LEFT JOIN Movimenti_dettagli (NOLOCK)  ON Movimenti.ID_Agenda = Movimenti_dettagli.Id_Agenda  ")
        Stb.AppendLine(" 					AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov ")
        Stb.AppendLine(" 					AND Movimenti_dettagli.Elem_Cod = 300 ")
        Stb.AppendLine(" LEFT JOIN Zoo_Animali (NOLOCK)  ON Movimenti_dettagli.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
        'Stb.AppendLine(" LEFT JOIN giac_cte g ON Zoo_animali.Cod_Progetto = g.Cod_Animale   ")
        'Stb.AppendLine(" 				AND  Movimenti.Data_Movimento >= g.mindata  ")
        'Stb.AppendLine(" 				AND  Movimenti.Data_Movimento <= g.maxData  ")
        'Stb.AppendLine(" OUTER APPLY (SELECT TOP 1 * FROM agn_cte WHERE agn_cte.Cau_Mov = '3700' AND agn_cte.Cod_Progetto = Zoo_Animali.Cod_Progetto ORDER BY agn_cte.Data_Movimento DESC ) g  ")
        Stb.AppendLine(" LEFT JOIN #agn_cte g ON g.Cod_Progetto = Zoo_Animali.Cod_Progetto ")
        Stb.AppendLine(" JOIN Utenti (NOLOCK)  ON Agenda.Username_Creazione = Utenti.CODICE_FISCALE ")
        'Stb.AppendLine(" WHERE (Agenda.Lav_Cod >= 3000 AND Agenda.Lav_Cod < 4000 ) ")
        Stb.AppendLine(" WHERE (Agenda.Lav_Cod IN (3033, 3006, 3007) ) ")
        Stb.AppendLine("  AND Movimenti.Cau_Mov IN ('3800', '3550', '3100', '3001')  ")

        If Piva <> "" Then
            Stb.AppendLine(" AND i.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        If FiltroCentri <> "" Then
            Stb.AppendLine(" AND g.Sa_Cod IN (" & Agro_SQL_Save_Clausola_IN(FiltroCentri) & ") ")
        End If

        If Sa_Cod <> 0 Then
            Stb.AppendLine(" AND g.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
        End If

        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) >= " & Agro_SQL_SaveDate(Validita_Inizio))
        Stb.AppendLine(" AND CAST(Movimenti.Data_Movimento as date) <= " & Agro_SQL_SaveDate(Validita_Fine))

        Stb.AppendLine(" AND Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleInizio))

        Stb.AppendLine(" AND Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(objParametri_Server.FinestraTemporaleFine))

        Stb.AppendLine("  GROUP BY Agenda.Piva  ")
        Stb.AppendLine("  , g.Sa_Cod  ")
        Stb.AppendLine("  , Agenda.Id_Agenda  ")
        Stb.AppendLine("  , Agenda.Lav_Cod  ")
        'Stb.AppendLine("  , Agenda.Des_Lib  ")
        Stb.AppendLine("  , Movimenti.Data_Movimento  ")
        Stb.AppendLine("  , Movimenti.Ora  ")
        'Stb.AppendLine("  , Movimenti.Mov_Desc  ")
        Stb.AppendLine("  , Agenda.Username_Creazione  ")
        'Stb.AppendLine("  , Movimenti.Cau_Mov  ")
        Stb.AppendLine("  , Agenda.Blocco_Flag  ")
        Stb.AppendLine("  , g.sa_nome ")
        Stb.AppendLine("   , Agenda.Tipo_Accettazione ")
        Stb.AppendLine("  , i.rag_soc  ")
        Stb.AppendLine("  , Operazioni.lav_des  ")
        Stb.AppendLine("  , GruppoOperazioni.gru_Des  ")
        Stb.AppendLine("  , GruppoOperazioni.tipo ")
        Stb.AppendLine("  , Attivita.Sigla ")
        Stb.AppendLine("  , Attivita.[Desc] ")
        Stb.AppendLine("  , Utenti.[user] ")
        Stb.AppendLine("  , g.STA_DES  ")
        Return Stb.ToString
    End Function

    '################################################################################
    Public Function Leggi_Raccoglitore(ByVal Raccoglitore_Cod As Integer,
                                       ByVal Piva As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.Leggi_Raccoglitore()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  Agenda ")
            strSql.Append(" WHERE 1 = 1  ")

            If Raccoglitore_Cod <> 0 Then
                strSql.Append(" AND Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            End If

            If Piva <> "" Then
                strSql.Append(" AND Piva = " & Agro_SQL_SaveText_NULL(Piva) & " ")
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

    Public Function LeggiAgendePerApp(ByVal piva As String, ByVal dataRif As Date, ByVal dataUltimaSincro As Date, ByVal soloOperazioniConTuttiGliImpiantiAttivi As Boolean, ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.LeggiAgendexApp()"

        Dim messaggioErrore As String = ""
        Dim sb = New Text.StringBuilder()
        Dim dt As DataTable

        Try

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine("WITH #cteAgendeDemetraEApp as ( ")
            sb.AppendLine(" SELECT ")
            sb.AppendLine("    	Piva, SUBSTRING(riferimento,0,CHARINDEX('|',riferimento,0)) ID_Agenda, ID as GuidRicetta, Versione, Tipo, ")
            sb.AppendLine("       SUBSTRING(riferimento, CHARINDEX('|', riferimento)+1,  LEN(riferimento) - CHARINDEX('|', riferimento)) CodiceRicetta ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("    	APP_Dati (NOLOCK) ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     tipo IN('10', '90') ")
            sb.AppendLine("     AND SUBSTRING(riferimento, 0, CHARINDEX('|',riferimento,0)) > 0 ")
            sb.AppendLine("),")

            sb.AppendLine(" ")

            sb.AppendLine("#cteAgendeFiltrate AS ( ")
            sb.AppendLine(" SELECT ")
            sb.AppendLine("	 a.Piva, a.Id_Agenda, a.Lav_Cod, a.Raccoglitore_Cod, ISNULL(ada.GuidRicetta, '') as GuidRicetta, ISNULL(ada.Versione, '') as Versione, ISNULL(ada.Tipo, '') as Tipo, ")
            sb.AppendLine("    ISNULL(ada.CodiceRicetta, 0) as CodiceRicetta, ISNULL(ad.ID, '') as CodiceGiasPianificata ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("	 Agenda a (NOLOCK) ")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("    Agronica_Log_Agenda_UltimaOperazione au ")
            sb.AppendLine("    ON a.Id_Agenda = au.Id_Agenda ")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("    RicetteXAgenda ra ")
            sb.AppendLine("    ON ra.Id_Agenda = a.Id_Agenda ")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("    Ricette_Operazioni ro ")
            sb.AppendLine("    ON ro.Ricetta_Operazione_Cod = ra.Ricetta_Operazione_Cod ")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("     Ricette_Operazioni roPianificata ")
            sb.AppendLine("     on roPianificata.Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod_RIF ")
            sb.AppendLine(" LEFT JOIN ")
            sb.AppendLine("     APP_Dati ad ")
            sb.AppendLine("     ON ad.ID = LEFT(roPianificata.APP_Ricetta_Operazione_ID, CHARINDEX('|', roPianificata.APP_Ricetta_Operazione_ID + '|') - 1) ")
            sb.AppendLine(" LEFT JOIN")
            sb.AppendLine("	 #cteAgendeDemetraEApp ada")
            sb.AppendLine("	 ON a.Id_Agenda = ada.ID_Agenda")
            sb.AppendLine("	 AND a.PIVA = ada.Piva")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("	 a.Piva = '" & Agro_SQL_SaveText(piva) & "'")
            If Not soloOperazioniConTuttiGliImpiantiAttivi Then
                sb.AppendLine("    AND a.Validita_Inizio >= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("    AND au.Data_Ora_RegistrazioneLog >= " & Agro_SQL_SaveDateTime(dataUltimaSincro) & " ")
            End If

            sb.AppendLine("    AND a.Lav_Cod IN (" & OPERAZIONI_GESTITE_APP_DEMETRA & ")")
            sb.AppendLine(" AND NOT EXISTS ")
            sb.AppendLine(" ( SELECT ")
            sb.AppendLine("       1 ")
            sb.AppendLine("   FROM ")
            sb.AppendLine("       Movimenti_Dettagli md (NOLOCK)")
            sb.AppendLine("   WHERE ")
            sb.AppendLine("       a.Id_Agenda = md.Id_Agenda ")
            sb.AppendLine("       AND md.Contabilizzato = -1 )") 'rimuove le attività pianificate
            sb.AppendLine(")")

            If soloOperazioniConTuttiGliImpiantiAttivi Then
                sb.AppendLine(", ")
                sb.AppendLine(" ")
                sb.AppendLine("#cteImpiantiAttivi as ( ")
                sb.AppendLine(" SELECT  ")
                sb.AppendLine("	af.Id_Agenda, ")
                sb.AppendLine("	CASE  ")
                sb.AppendLine("           WHEN EXISTS ( ")
                sb.AppendLine("               SELECT  ")
                sb.AppendLine("					1 ")
                sb.AppendLine("               FROM  ")
                sb.AppendLine("					Imprese_Progetti es ")
                sb.AppendLine("               JOIN ")
                sb.AppendLine("					Reg_Impianti imp ")
                sb.AppendLine("					ON imp.piva = es.piva ")
                sb.AppendLine("					and imp.SA_COD = es.Sa_Cod ")
                sb.AppendLine("					and imp.APPEZZA = es.Appezza ")
                sb.AppendLine("					and imp.ID_REG = es.Id_Reg ")
                sb.AppendLine("			   WHERE  ")
                sb.AppendLine("					es.piva = mdest.piva ")
                sb.AppendLine("                   AND es.sa_cod = mdest.sa_cod ")
                sb.AppendLine("                   AND es.appezza = mdest.appezza ")
                sb.AppendLine("                   AND es.id_reg = mdest.Id_Destinazione ")
                sb.AppendLine("				    AND imp.Validita_Inizio <= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("					AND imp.Validita_Fine >= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("				    AND es.Validita_Inizio <= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("					AND es.Validita_Fine >= " & Agro_SQL_SaveDate(dataRif) & " ")
                sb.AppendLine("		   ) ")
                sb.AppendLine("           THEN 1 ")
                sb.AppendLine("           ELSE 0 ")
                sb.AppendLine("       END AS EsercizioValido ")
                sb.AppendLine(" FROM  ")
                sb.AppendLine("	#cteAgendeFiltrate af ")
                sb.AppendLine(" JOIN ")
                sb.AppendLine("	Mov_Destinazioni mdest ")
                sb.AppendLine("	on mdest.Id_Agenda = af.Id_Agenda ")
                sb.AppendLine("	and mdest.Piva = af.PIVA ")
                sb.AppendLine(" WHERE  ")
                sb.AppendLine("	mdest.Tipo_Destinazione = 0 ")
                sb.AppendLine(") ")

                sb.AppendLine(" ")

                sb.AppendLine(" SELECT  ")
                sb.AppendLine(" 	*  ")
                sb.AppendLine(" FROM  ")
                sb.AppendLine(" 	#cteAgendeFiltrate ")
                sb.AppendLine(" WHERE ")
                sb.AppendLine(" 	NOT EXISTS(SELECT ")
                sb.AppendLine(" 					1 ")
                sb.AppendLine(" 			   FROM ")
                sb.AppendLine(" 					#cteImpiantiAttivi ")
                sb.AppendLine(" 			   WHERE ")
                sb.AppendLine(" 					#cteImpiantiAttivi.Id_Agenda = #cteAgendeFiltrate.Id_Agenda ")
                sb.AppendLine(" 					AND #cteImpiantiAttivi.EsercizioValido = 0)")

            Else
                sb.AppendLine(" ")
                sb.AppendLine(" SELECT  ")
                sb.AppendLine(" 	*  ")
                sb.AppendLine(" FROM  ")
                sb.AppendLine(" 	#cteAgendeFiltrate ")

            End If

            sb.AppendLine("OPTION (RECOMPILE); ")

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

    Public Function LeggiIdAgendaELavCodPerAttivitaConRaccoglitoreCod(ByVal raccoglitore_cod As Integer, ByRef objParametriServer As AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_R.LeggiIdAgendaELavCodPerAttivitaConRaccoglitoreCod()"

        Dim messaggioErrore As String = ""
        Dim sb = New Text.StringBuilder()
        Dim dt As DataTable
        Try

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" select ")
            sb.AppendLine(" 	a.Id_Agenda, a.Lav_Cod")
            sb.AppendLine(" from ")
            sb.AppendLine(" 	agenda a")
            sb.AppendLine(" where")
            sb.AppendLine(" 	a.Raccoglitore_Cod = " & Agro_SQL_SaveNum(raccoglitore_cod))

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

End Class




'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§




Public Class Agenda_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Lav_Cod As Integer,
                           ByVal Linea_Cod As Integer,
                           ByVal Preparazione_Cod As Integer,
                           ByVal Id_Trasformazione As Integer,
                           ByVal Des_Lib As String,
                           ByVal Tipo_Accettazione As Int16,
                           ByVal Blocco_Flag As Integer,
                           ByVal Blocco_Username As String,
                           ByVal Blocco_Data As Date,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = "",
                           Optional ByVal Audit_Cod As Integer = 0,
                           Optional ByVal Stato_Export As Integer = 0,
                           Optional ByVal Stato_Export_2 As Integer = 0,
                           Optional ByVal Tipo_Visibilita As Integer = 0,
                           Optional ByVal ChkCoge_Manuale As Integer = 0,
                           Optional ByVal Id_Attivita As Integer = 0,
                           Optional ByVal Modulo As Integer = 0,
                           Optional ByVal Raccoglitore_Cod As Integer = 0,
                           Optional ByVal Split As Integer = 0,
                           Optional ByVal Pratica_Cod As Integer = 0,
                           Optional ByVal Origine As String = "",
                           Optional ByVal Stato_Cod As Integer = 0,
                           Optional ByVal DaRemoto As Integer = 0
                           ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Lav_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Lav_Cod = 0)")
            End If

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


            '---------------------------------------------
            strSql.Length = 0
            strSql.AppendLine(" INSERT INTO Agenda ")
            strSql.AppendLine("             (Piva,    Sa_Cod,    Id_Agenda, ")
            strSql.AppendLine("              Lav_Cod, Linea_Cod, Preparazione_Cod, ")
            strSql.AppendLine("              Id_Trasformazione,  Des_Lib,           Tipo_Accettazione, ")
            strSql.AppendLine("              Audit_Cod,  Raccoglitore_Cod,        Stato_Export,      Stato_Export_2, ")
            strSql.AppendLine("              Tipo_Visibilita,    ChkCoge_Manuale,   Id_Attivita, ")
            strSql.AppendLine("              Modulo, Split, Pratica_Cod, Origine, Stato_Cod, DaRemoto, ")

            strSql.AppendLine("              Inviato,            DataInvio, ")
            strSql.AppendLine("              Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("              UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("              Validita_Inizio,    Validita_Fine, ")
            strSql.AppendLine("              Blocco_Flag,        Blocco_Username,   Blocco_Data ")
            strSql.AppendLine("              ) ")

            strSql.AppendLine(" VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Linea_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Preparazione_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Trasformazione) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Des_Lib) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Accettazione) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Audit_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Stato_Export) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Stato_Export_2) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Tipo_Visibilita) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(ChkCoge_Manuale) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Modulo) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Split) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Origine) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(DaRemoto) & " ")

            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")

            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")


            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            strSql.AppendLine("         , '" & Agro_SQL_SaveText(Blocco_Username) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Blocco_Data) & " ")
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


    ' Giulia: 12/10/2017: funzione usata da SicurezzaLavoro_Edit in AgronicaAudit
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Lav_Cod As Integer,
                             ByVal Linea_Cod As Integer,
                             ByVal Preparazione_Cod As Integer,
                             ByVal Id_Trasformazione As Integer,
                             ByVal Des_Lib As String,
                             ByVal Tipo_Accettazione As Integer,
                             ByVal Blocco_Flag As Integer,
                             ByVal Blocco_Username As String,
                             ByVal Blocco_Data As Date,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Audit_Cod As Integer = 0
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Lav_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Lav_Cod = 0)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" UPDATE Agenda SET ")
            strSql.Append("    Lav_Cod           =  " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            strSql.Append("   ,Linea_Cod         =  " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
            strSql.Append("   ,Preparazione_Cod  =  " & Agro_SQL_SaveNum(Preparazione_Cod) & "   ")
            strSql.Append("   ,Id_Trasformazione =  " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
            strSql.Append("   ,Des_Lib           = '" & Agro_SQL_SaveText(Des_Lib) & "'   ")
            strSql.Append("   ,Tipo_Accettazione =  " & Agro_SQL_SaveNum(Tipo_Accettazione) & "    ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            strSql.Append("   ,Blocco_Flag       =  " & Agro_SQL_SaveNum(Blocco_Flag))
            strSql.Append("   ,Blocco_Username   =  '" & Agro_SQL_SaveText(Blocco_Username) & "'")
            strSql.Append("   ,Blocco_Data       =  " & Agro_SQL_SaveDate(Blocco_Data))
            strSql.Append("   ,Audit_Cod       =  " & Agro_SQL_SaveNum(Audit_Cod))

            strSql.Append(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.Append(" AND   Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "   ")

            '----------------------------------------------------------------------
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

    Public Function ModificaDataCreazione(
        ByVal Piva As String,
        ByVal Sa_Cod As Integer,
        ByVal Id_Agenda As Integer,
        ByVal DataCreazione As DateTime,
        ByVal xFiltroAggiuntivo As String,
        ByRef objParametri As AgronicaCoreParametri
        ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If



            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" UPDATE Agenda SET ")
            strSql.Append(" Data_Creazione  =  " & Agro_SQL_SaveDateTime(DataCreazione) & "   ")

            strSql.Append(" WHERE Piva      = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append(" AND   Sa_Cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.Append(" AND   Id_Agenda =  " & Agro_SQL_SaveNum(Id_Agenda) & "   ")

            '----------------------------------------------------------------------
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


    Public Function ModificaPuntuale(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal Id_Agenda As Integer,
                                     ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri,
                                     Optional ByVal Lav_Cod As Integer? = Nothing,
                                     Optional ByVal Des_Lib As String = Nothing,
                                     Optional ByVal Blocco_Flag As Integer? = Nothing,
                                     Optional ByVal Blocco_Data As Date? = Nothing,
                                     Optional ByVal Blocco_Username As String = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal Linea_Cod As Integer? = Nothing,
                                     Optional ByVal Preparazione_Cod As Integer? = Nothing,
                                     Optional ByVal Id_Trasformazione As Integer? = Nothing,
                                     Optional ByVal Tipo_Accettazione As Integer? = Nothing,
                                     Optional ByVal Stato_Export As Integer? = Nothing,
                                     Optional ByVal Stato_Export_2 As Integer? = Nothing,
                                     Optional ByVal Tipo_Visibilita As Integer? = Nothing,
                                     Optional ByVal ChkCoge_Manuale As Integer? = Nothing,
                                     Optional ByVal Id_Attivita As Integer? = Nothing,
                                     Optional ByVal Modulo As Integer? = Nothing,
                                     Optional ByVal Audit_Cod As Integer? = Nothing,
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = "",
                                     Optional ByVal Raccoglitore_Cod As Integer? = Nothing,
                                     Optional ByVal Split As Integer? = Nothing,
                                     Optional ByVal Pratica_Cod As Integer? = Nothing,
                                     Optional ByVal Origine As String = Nothing,
                                     Optional ByVal Stato_Cod As Integer? = Nothing,
                                     Optional ByVal DaRemoto As Integer? = Nothing
                                     ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.ModificaPuntuale()"

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

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Lav_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Lav_Cod = 0)")
            End If

            'per alcuni tipi di movimento dettaglio il sa_cod è valorizzato a 0 (non valorizzato)
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Lav_Cod) Then
                strSql.AppendLine("   , Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & " ")
            End If

            If Not IsNothing(Des_Lib) Then
                strSql.AppendLine("   , Des_Lib = '" & Agro_SQL_SaveText(Des_Lib) & "' ")
            End If

            If Not IsNothing(Blocco_Flag) Then
                strSql.AppendLine("   , Blocco_Flag = " & Agro_SQL_SaveNum(Blocco_Flag) & " ")
            End If

            If Not IsNothing(Blocco_Data) Then
                strSql.AppendLine("   , Blocco_Data = " & Agro_SQL_SaveDate(Blocco_Data) & " ")
            End If

            If Not IsNothing(Blocco_Username) Then
                strSql.AppendLine("   , Blocco_Username = '" & Agro_SQL_SaveText(Blocco_Username) & "' ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            If Not IsNothing(Linea_Cod) Then
                strSql.AppendLine("   , Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & " ")
            End If

            If Not IsNothing(Preparazione_Cod) Then
                strSql.AppendLine("   , Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & " ")
            End If

            If Not IsNothing(Id_Trasformazione) Then
                strSql.AppendLine("   , Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & " ")
            End If

            If Not IsNothing(Tipo_Accettazione) Then
                strSql.AppendLine("   , Tipo_Accettazione = " & Agro_SQL_SaveNum(Tipo_Accettazione) & " ")
            End If

            If Not IsNothing(Stato_Export) Then
                strSql.AppendLine("   , Stato_Export = " & Agro_SQL_SaveNum(Stato_Export) & " ")
            End If

            If Not IsNothing(Stato_Export_2) Then
                strSql.AppendLine("   , Stato_Export_2 = " & Agro_SQL_SaveNum(Stato_Export_2) & " ")
            End If

            If Not IsNothing(Tipo_Visibilita) Then
                strSql.AppendLine("   , Tipo_Visibilita = " & Agro_SQL_SaveNum(Tipo_Visibilita) & " ")
            End If

            If Not IsNothing(ChkCoge_Manuale) Then
                strSql.AppendLine("   , ChkCoge_Manuale = " & Agro_SQL_SaveNum(ChkCoge_Manuale) & " ")
            End If

            If Not IsNothing(Id_Attivita) Then
                strSql.AppendLine("   , Id_Attivita = " & Agro_SQL_SaveNum(Id_Attivita) & " ")
            End If

            If Not IsNothing(Modulo) Then
                strSql.AppendLine("   , Modulo = " & Agro_SQL_SaveNum(Modulo) & " ")
            End If

            If Not IsNothing(Audit_Cod) Then
                strSql.AppendLine("   , Audit_Cod = " & Agro_SQL_SaveNum(Audit_Cod) & " ")
            End If

            If Not IsNothing(Raccoglitore_Cod) Then
                strSql.AppendLine("   , Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            End If

            If Not IsNothing(Split) Then
                strSql.AppendLine("   , Split = " & Agro_SQL_SaveNum(Split) & " ")
            End If

            If Not IsNothing(Pratica_Cod) Then
                strSql.AppendLine("   , Pratica_Cod = " & Agro_SQL_SaveNum(Pratica_Cod) & " ")
            End If

            If Not IsNothing(Origine) Then
                strSql.AppendLine("   , Origine = '" & Agro_SQL_SaveText(Origine) & "' ")
            End If

            If Not IsNothing(Stato_Cod) Then
                strSql.AppendLine("   , Stato_Cod = " & Agro_SQL_SaveNum(Stato_Cod) & " ")
            End If

            If Not IsNothing(DaRemoto) Then
                strSql.AppendLine("   , DaRemoto = " & Agro_SQL_SaveNum(DaRemoto) & " ")
            End If
            '---------------------------------------------            

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

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

    Public Function Modifica_x_Trasferimento(ByVal sa_cod As Integer,
                                             ByVal Str_id_agenda As String,
                                             ByRef objParametri As AgronicaCoreParametri
                                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Modifica_x_Trasferimento()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Str_id_agenda = "" Then
                Throw New Exception("Parametro non corretto nella query (Str_id_agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE agenda SET ")
            strSql.Append("    sa_cod    = " & Agro_SQL_SaveNum(sa_cod) & "  ")


            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.Append(" WHERE    (" & Str_id_agenda & ")")
            'esclusi record con sa_cod=0 (DDT e Fatture)
            strSql.Append(" AND Sa_Cod <> 0 ")


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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '   Id_Agenda = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Agenda ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")

                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Agenda ")

                strSql.Append(" WHERE  1=1 ")

            End If

            strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            strSql.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")

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


    'fatto per eliminare i riferimenti alla sola operazione agenda, infatti la 
    'cancellazione dei riferimenti quando elimino un'operazione agenda dal menu si ha di solito con
    'IPNO_Delete che cancella i riferimenti tra movimenti e non tra sola operazione agenda 
    '(come nel caso di raccolta e cura per tabacco) che hanno -1 in mov e movdet
    Public Function CancellaRiferimentiAgenda(ByVal Piva As String,
                                              ByVal Sa_Cod As Integer,
                                              ByVal Id_Agenda As Integer,
                                              ByVal xFiltroAggiuntivo As String,
                                              ByRef objParametri As AgronicaCoreParametri
                                              ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.CancellaRiferimentiAgenda()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then


            Else
                strSql.Length = 0
                strSql.Append(" DELETE FROM  Mov_Dettagli_Riferimenti  ")

                strSql.Append(" WHERE  1=1 ")

            End If

            strSql.Append(" AND (( ")

            strSql.Append(" Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            If Sa_Cod <> 0 Then
                strSql.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If
            strSql.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            strSql.Append(" AND Id_Mov = -1   ")
            strSql.Append(" AND Id_Mov_Det = -1   ")

            strSql.Append(" ) OR ( ")

            strSql.Append(" Piva_Rif = '" & Agro_SQL_SaveText(Piva) & "' ")
            If Sa_Cod <> 0 Then
                strSql.Append(" AND Sa_Cod_Rif = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If
            strSql.Append(" AND Id_Agenda_Rif = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            strSql.Append(" AND Id_Mov_Rif = -1   ")
            strSql.Append(" AND Id_Mov_Det_Rif = -1   ")

            strSql.Append(" ))")


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


    '============================================================================
    Public Function Agenda_Blocca(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal Id_Agenda As Integer,
                                  ByVal Blocco_Username As String,
                                  ByVal Blocco_Data As Date,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda SET ")
            strSql.AppendLine("     Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(Blocco_Username) & "'  ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data))

            strSql.AppendLine(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.AppendLine(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")


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

    Public Function Agenda_Blocca(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal strId_Agenda As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If strId_Agenda = "" Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda SET ")
            strSql.AppendLine("     Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" WHERE  Id_Agenda   IN (" & Agro_SQL_Save_Clausola_IN(strId_Agenda) & ")  ")

            If Piva <> "" Then
                strSql.AppendLine(" AND   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

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

    Public Function Agenda_Blocca(ByVal Piva As String,
                                  ByVal data_inizio As DateTime,
                                  ByVal data_fine As DateTime,
                                  ByVal bloccaSoloSeNonGiaBloccati As Boolean,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda SET ")
            strSql.AppendLine("     Blocco_Flag         =  1 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" WHERE validita_inizio >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            If bloccaSoloSeNonGiaBloccati = True Then
                strSql.AppendLine(" AND Blocco_Flag = 0 ")
            End If

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

    '============================================================================
    Public Function Agenda_MarcaComeInviato(ByVal Piva As String,
                                            ByVal Sa_Cod As Integer,
                                            ByVal Id_Agenda As Integer,
                                            ByVal Data_invio As DateTime,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_MarcaComeInviato()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE Agenda SET ")
            strSql.Append("     inviato         =  -2 ")
            strSql.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            strSql.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")


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


    ''============================================================================
    ''Nel COM+ Blocco_Data era opzionale (default cDate("01/01/1900"))
    ''se non si vuole salvare la data si sblocco, passare 01/01/1900
    Public Function Agenda_Sblocca(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Id_Agenda As Integer,
                                   ByVal Blocco_Username As String,
                                   ByVal Blocco_Data As Date,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try



            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE Agenda SET ")
            strSql.Append("     Blocco_Flag         =  0 ")
            strSql.Append("    ,Blocco_Username     = '" & Agro_SQL_SaveText(Blocco_Username) & "'  ")
            strSql.Append("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data))

            strSql.Append(" WHERE   Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If Piva <> "" Then
                strSql.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            '----------------------------------------------------------------------
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

    'a differenza della precedente non serve il sa_cod
    Public Function Agenda_Sblocca2(ByVal Piva As String,
                                    ByVal Id_Agenda As Integer,
                                    ByVal Blocco_Username As String,
                                    ByVal Blocco_Data As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda SET ")
            strSql.AppendLine("     Blocco_Flag =  0 ")
            strSql.AppendLine("     , Blocco_Username = '" & Agro_SQL_SaveText(Blocco_Username) & "'  ")
            strSql.AppendLine("     , Blocco_Data =  " & Agro_SQL_SaveDate(Blocco_Data))

            strSql.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")


            '----------------------------------------------------------------------
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

    Public Function Agenda_Sblocca(ByVal Piva As String,
                                   ByVal data_inizio As DateTime,
                                   ByVal data_fine As DateTime,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Sblocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda SET ")
            strSql.AppendLine("     Blocco_Flag         =  0 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.AppendLine(" AND validita_inizio >= " & Agro_SQL_SaveDate(data_inizio))
            strSql.AppendLine(" AND validita_inizio <= " & Agro_SQL_SaveDate(data_fine))

            strSql.AppendLine(" AND Blocco_Flag = 1 ")

            '----------------------------------------------------------------------
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

    Public Function Agenda_Sblocca_AzzeraStatoExport(ByVal Piva As String,
                                                     ByVal Sa_Cod As Integer,
                                                     ByVal Id_Agenda As Integer,
                                                     ByVal Blocco_Username As String,
                                                     ByVal Blocco_Data As Date,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE Agenda SET ")
            strSql.Append("     Stato_Export         =  0 ")
            strSql.Append("    , Blocco_Flag         =  0 ")
            strSql.Append("    , Blocco_Username     = '" & Agro_SQL_SaveText(Blocco_Username) & "'  ")
            strSql.Append("    , Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data))

            strSql.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")


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
    Public Function Agenda_Sblocca(ByVal Piva As String,
                                  ByVal Sa_Cod As Integer,
                                  ByVal strId_Agenda As String,
                                  ByVal xFiltroAggiuntivo As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try


            If strId_Agenda = "" Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Agenda SET ")
            strSql.AppendLine("     Blocco_Flag         =  0 ")
            strSql.AppendLine("    ,Blocco_Username     = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.AppendLine("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Now))

            strSql.AppendLine(" WHERE  Id_Agenda   IN (" & Agro_SQL_Save_Clausola_IN(strId_Agenda) & ")  ")

            If Piva <> "" Then
                strSql.AppendLine(" AND   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            End If
            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

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

    Public Function Agenda_Annulla_Modifica_Stato_ExportZespri(
        ByVal idAgendaPerUpdate As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Modifica_Stato_ExportZespri()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If idAgendaPerUpdate = "" Then
                Throw New Exception("Parametro non corretto nella query (idAgendaPEerUpdate)")
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE  Agenda SET ")
            strSql.Append("           Stato_Export  = " & Agro_SQL_SaveNum(0) & "  ")
            strSql.Append("         , Stato_Export_2  = " & Agro_SQL_SaveNum(0) & "  ")
            'strSql.Append("         , Blocco_Flag  = " & Agro_SQL_SaveNum(0) & "  ")
            'strSql.Append("         , Blocco_Username  = '" & Agro_SQL_SaveText("") & "'  ")
            'strSql.Append("         , Blocco_Data  = " & Agro_SQL_SaveDate(AGRODATAINIZIO) & "  ")

            strSql.Append(" WHERE   Id_Agenda in (" & Agro_SQL_Save_Clausola_IN(idAgendaPerUpdate) & ")")

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

    Public Function Agenda_Modifica_Stato_ExportZespri(
        ByVal idAgendaPerUpdate As String,
        ByVal Stato_Export As Integer,
        ByVal Stato_Export_2 As Integer,
        ByRef objParametri As AgronicaCoreParametri
    ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Modifica_Stato_ExportZespri()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If idAgendaPerUpdate = "" Then
                Throw New Exception("Parametro non corretto nella query (idAgendaPEerUpdate)")
            End If


            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE  Agenda SET ")
            strSql.Append("         Stato_Export  = " & Agro_SQL_SaveNum(Stato_Export) & "  ")
            strSql.Append("         , Stato_Export_2  = " & Agro_SQL_SaveNum(Stato_Export_2) & "  ")
            'strSql.Append("         , Blocco_Flag  = " & Agro_SQL_SaveNum(1) & "  ")
            'strSql.Append("         , Blocco_Username  = '" & Agro_SQL_SaveText(objParametri.UtenteUsername) & "'  ")
            'strSql.Append("         , Blocco_Data  = " & Agro_SQL_SaveDate(Now.Date) & "  ")

            strSql.Append(" WHERE   Id_Agenda in (" & Agro_SQL_Save_Clausola_IN(idAgendaPerUpdate) & ")")

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

    Public Function Agenda_Modifica_Stato_Export(ByVal Piva As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Id_Agenda As Integer,
                                                 ByVal Stato_Export As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri
                                                 ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Modifica_Stato_Export()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE  Agenda SET ")
            strSql.Append("         Stato_Export  = " & Agro_SQL_SaveNum(Stato_Export) & "  ")

            strSql.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

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

    Public Function Agenda_Modifica_Stato_Export_2(ByVal Piva As String,
                                                   ByVal Sa_Cod As Integer,
                                                   ByVal Id_Agenda As Integer,
                                                   ByVal Stato_Export_2 As Integer,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Modifica_Stato_Export_2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'Non va bene, perché nelle operazioni contabili il sa_cod deve essere =0
            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE  Agenda SET ")
            strSql.Append("         Stato_Export_2  = " & Agro_SQL_SaveNum(Stato_Export_2) & "  ")

            strSql.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

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


    '============================================================================
    Public Function Agenda_SettaFlag_OpDaRiconfermare(ByVal Piva As String,
                                                      ByVal Id_Agenda As Integer,
                                                      ByVal Blocco_Username As String,
                                                      ByVal Blocco_Data As Date,
                                                      ByVal xFiltroAggiuntivo As String,
                                                      ByRef objParametri As AgronicaCoreParametri
                                                      ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Blocca()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE Agenda SET ")
            strSql.Append("     Blocco_Flag         =  2 ")
            strSql.Append("    ,Blocco_Username     = '" & Agro_SQL_SaveText(Blocco_Username) & "'  ")
            strSql.Append("    ,Blocco_Data         =  " & Agro_SQL_SaveDate(Blocco_Data))

            strSql.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

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

    '########################################################################################
    ''' <summary>
    ''' Dato un hashtable con gli id_agenda delle operazioni sull'impianto
    ''' chiama la funziona che setta il flag
    ''' </summary>
    Public Function Flagga_OpColturali_DaVerificare(ByRef objParametri_Server As AgronicaCoreParametri,
                                                    ByVal Hash_IdAgenda_ToFlag As Hashtable
                                                    ) As Boolean


        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Flagga_OpColturali_DaVerificare()"
        Dim messaggioErrore As String = ""
        Dim Flag_UpdateFlagOK As Boolean = False

        Try

            If Not IsNothing(Hash_IdAgenda_ToFlag) AndAlso Hash_IdAgenda_ToFlag.Count > 0 Then

                Dim MyKeys As ICollection
                Dim Key As Object
                Dim piva As String
                Dim id_agenda As Integer

                MyKeys = Hash_IdAgenda_ToFlag.Keys()

                For Each Key In MyKeys

                    piva = Hash_IdAgenda_ToFlag(Key)
                    id_agenda = CInt(Key)

                    'update del campo flag nella tabella agenda
                    Flag_UpdateFlagOK = Agenda_SettaFlag_OpDaRiconfermare(
                                                    piva,
                                                    id_agenda,
                                                    objParametri_Server.UtenteUsername,
                                                    Date.Now,
                                                    "",
                                                    objParametri_Server)


                Next

            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Flag_UpdateFlagOK = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return Flag_UpdateFlagOK


    End Function


    '########################################################################################
    Public Function Agenda_Modifica_IdAttivita(ByVal Piva As String,
                                               ByVal Sa_Cod As Integer,
                                               ByVal Id_Agenda As Integer,
                                               ByVal Id_Attivita As Integer,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As Boolean

        Dim nomeRoutine As String = "AgronicaCoreContabDAL.Agenda_W.Agenda_Modifica_IdAttivita()"

        Dim messaggioErrore As String = ""
        Dim strSql As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        '------------------------------

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE  Agenda SET ")
            strSql.Append("         Id_Attivita  = " & Agro_SQL_SaveNum(Id_Attivita) & "  ")

            strSql.Append(" WHERE   PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append(" AND     Sa_Cod      = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append(" AND     Id_Agenda   = " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

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

    Public Sub Scrivi(ByRef Agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.Scrivi()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Agenda, objParametriServer)

            Agenda.Data_Creazione = DateTime.Now
            Agenda.Data_Modifica = DateTime.Now
            Agenda.Username_Creazione = objParametriServer.UsernameOperazione
            Agenda.Username_Modifica = objParametriServer.UsernameOperazione

            Dim agronica_log_agenda As New AgronicaLogAgenda_W
            agronica_log_agenda.Scrivi_EF(Agenda.Validita_Inizio, 1, Agenda.des_lib, Agenda.Id_Agenda, Agenda.PIVA, Agenda.Sa_Cod, Agenda.Lav_Cod, 5, GiasContext, objParametriServer)

            GiasContext.Agenda.Add(Agenda)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub


    Public Sub Modifica(ByRef Agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Agenda, objParametriServer)

            Agenda.Data_Modifica = DateTime.Now
            Agenda.Username_Modifica = objParametriServer.UsernameOperazione

            Dim agronica_log_agenda As New AgronicaLogAgenda_W
            agronica_log_agenda.Scrivi_EF(Agenda.Validita_Inizio, 2, Agenda.des_lib, Agenda.Id_Agenda, Agenda.PIVA, Agenda.Sa_Cod, Agenda.Lav_Cod, 5, GiasContext, objParametriServer)

            GiasContext.Entry(Agenda).State = EntityState.Modified

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Valorizza(ByRef Agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.Valorizza()"
        Dim messaggioErrore As String = ""

        Try

            If Agenda.PIVA Is Nothing Then
                Throw New Exception("Agenda - PIVA non valorizzata")
            End If

            If Agenda.Lav_Cod Is Nothing Or Agenda.Lav_Cod = 0 Then
                Throw New Exception("Agenda - Lav_Cod non valorizzato")
            End If

            If Agenda.des_lib Is Nothing Then
                Agenda.des_lib = ""
            End If

            If Agenda.Blocco_Flag Is Nothing Then
                Agenda.Blocco_Flag = 0
            End If

            If Agenda.Blocco_Data Is Nothing Then
                Agenda.Blocco_Data = AGRODATAINIZIO
            End If

            If Agenda.Blocco_Username Is Nothing Then
                Agenda.Blocco_Username = ""
            End If

            If Agenda.inviato Is Nothing Then
                Agenda.inviato = 0
            End If

            If Agenda.Data_Creazione Is Nothing Then
                Agenda.Data_Creazione = DateTime.Now
            End If

            If Agenda.Username_Creazione Is Nothing Then
                Agenda.Username_Creazione = objParametriServer.UsernameOperazione
            End If

            If Agenda.Stato_Export Is Nothing Then
                Agenda.Stato_Export = 0
            End If

            If Agenda.Stato_Export_2 Is Nothing Then
                Agenda.Stato_Export_2 = 0
            End If

            If Agenda.Tipo_Visibilita Is Nothing Then
                Agenda.Tipo_Visibilita = 0
            End If

            If Agenda.ChkCoge_Manuale Is Nothing Then
                Agenda.ChkCoge_Manuale = 0
            End If

            If Agenda.Id_Attivita Is Nothing Then
                Agenda.Id_Attivita = 0
            End If

            If Agenda.Modulo Is Nothing Then
                Agenda.Modulo = 0
            End If

            If Agenda.Audit_Cod Is Nothing Then
                Agenda.Audit_Cod = 0
            End If

            If Agenda.Raccoglitore_Cod Is Nothing Then
                Agenda.Raccoglitore_Cod = 0
            End If

            If Agenda.Split Is Nothing Then
                Agenda.Split = 0
            End If

            If Agenda.Pratica_Cod Is Nothing Then
                Agenda.Pratica_Cod = 0
            End If

            If Agenda.Validita_Inizio < AGRODATAINIZIO Then
                Agenda.Validita_Inizio = AGRODATAINIZIO
            End If

            If Agenda.Validita_Fine < AGRODATAINIZIO Then
                Agenda.Validita_Fine = AGRODATAFINE
            End If

            If Agenda.Origine Is Nothing Then
                Agenda.Origine = ""
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Function Elimina_Testata(ByRef Agenda As AgronicaCoreEntityFramework_POCO.Agenda,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.Elimina_Testata()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Agenda.Remove(Agenda)

            Return True

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Function

    Public Function Elimina_Testata(ByVal Piva As String,
                                    ByVal ID_Agenda As Integer,
                                    ByRef GiasContext As Gias_DeveloperServer_Entities,
                                    ByRef objParametriServer As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.Elimina_Testata()"
        Dim messaggioErrore As String = ""

        Try
            Dim Agenda = (From a In GiasContext.Agenda Where a.PIVA = Piva AndAlso a.Id_Agenda = ID_Agenda).First

            If Agenda IsNot Nothing AndAlso Agenda.Id_Agenda <> 0 Then
                Elimina_Testata(Agenda, GiasContext, objParametriServer)
            End If

            Return True
        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Function





    Public Function RenameAllegato(ByVal Piva As String, ByVal Id_Agenda As Integer, ByVal File_Name As String, ByRef objParametri_Server As AgronicaCoreParametri) As String
        Const nomeRoutine = "AgronicaCoreContabDAL.Agenda_W.RenameAllegato()"
        Dim messaggioErrore As String = ""

        Try

            '==================================================================================================================================================================
            If Id_Agenda <> 0 Then
                'Modifica Nome File

                Dim ArrayEst() As String
                Dim Allegati_Documenti_Estensione As String = ""
                Dim Operazione_Riferimento As String = "Visita"
                Dim invalidChars As String = "[<>:""/\|?*]"

                'Verifico il lav_cod dell'agenda di riferimento
                Dim ObjAgenda_Rif As New AgronicaCoreContabDAL.Mov_Dettagli_Riferimenti_R
                Dim DTAgenda_Rif As DataTable = ObjAgenda_Rif.Leggi(Piva, 0, Id_Agenda, 0, 0, 0, "", "", "", objParametri_Server)

                If DTAgenda_Rif.Rows.Count <> 0 Then

                    'Visite collegata a riferimento
                    Dim DrRiferimenti() As DataRow
                    DrRiferimenti = DTAgenda_Rif.Select("Lav_Cod = 5007")
                    If DrRiferimenti IsNot Nothing AndAlso DrRiferimenti.Length > 0 Then

                        'Determino il Lav_Des da tabella operazionixlingue
                        Dim ObjOperazioni = New AgronicaCoreMetaSchemaDAL.Operazioni_R
                        Dim DtOperazioni As DataTable = ObjOperazioni.Leggi(DTAgenda_Rif(0)("Lav_Cod_Rif"), 0, 0, 0, 0, "", "", True, True, True, True, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                        If DtOperazioni.Rows.Count > 0 Then
                            Operazione_Riferimento = DtOperazioni(0)("Lav_Des")
                            Operazione_Riferimento = Regex.Replace(Operazione_Riferimento, invalidChars, String.Empty, RegexOptions.None, TimeSpan.FromSeconds(3))
                            Operazione_Riferimento = Operazione_Riferimento.Trim()
                        End If

                        'Determino Estensione
                        ArrayEst = Split(File_Name, ".")
                        If UBound(ArrayEst) > 0 Then
                            Allegati_Documenti_Estensione = ArrayEst(UBound(ArrayEst))
                        End If

                        'Formattazione Allegato
                        If Not String.IsNullOrEmpty(Allegati_Documenti_Estensione) Then
                            File_Name = File_Name.Replace(".", "")
                            Dim index As Integer = File_Name.LastIndexOf(Allegati_Documenti_Estensione)
                            File_Name = File_Name.Insert(index, "_" & Operazione_Riferimento & "_" & DTAgenda_Rif(0)("Id_Agenda_Rif"))
                            index = index + Len(Operazione_Riferimento) + Len(CStr(DTAgenda_Rif(0)("Id_Agenda_Rif"))) + 2
                            File_Name = File_Name.Insert(index, ".")
                        End If

                    End If

                    '...............

                Else

                    Dim ObjAgenda As New AgronicaCoreContabDAL.Agenda_R
                    Dim DTAgenda As DataTable = ObjAgenda.Leggi(Piva, 0, Id_Agenda, 0, enumSelezioneVariabile.Selezione_TabellaCompleta, "Agenda.Lav_Cod = 5007", "", objParametri_Server)

                    If DTAgenda.Rows.Count <> 0 Then
                        'Visite standalone --> non collegata a riferimento

                        'Determino il Lav_Des da tabella operazionixlingue
                        Dim ObjOperazioni = New AgronicaCoreMetaSchemaDAL.Operazioni_R
                        Dim DtOperazioni As DataTable = ObjOperazioni.Leggi(DTAgenda(0)("Lav_Cod"), 0, 0, 0, 0, "", "", True, True, True, True, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri_Server)
                        If DtOperazioni.Rows.Count > 0 Then
                            Operazione_Riferimento = DtOperazioni(0)("Lav_Des")
                            Operazione_Riferimento = Regex.Replace(Operazione_Riferimento, invalidChars, String.Empty, RegexOptions.None, TimeSpan.FromSeconds(3))
                            Operazione_Riferimento = Operazione_Riferimento.Trim()
                        End If

                        'Determino Estensione
                        ArrayEst = Split(File_Name, ".")
                        If UBound(ArrayEst) > 0 Then
                            Allegati_Documenti_Estensione = ArrayEst(UBound(ArrayEst))
                        End If

                        'Formattazione Allegato
                        If Not String.IsNullOrEmpty(Allegati_Documenti_Estensione) Then
                            File_Name = File_Name.Replace(".", "")
                            Dim index As Integer = File_Name.LastIndexOf(Allegati_Documenti_Estensione)
                            File_Name = File_Name.Insert(index, "_" & Operazione_Riferimento & "_" & DTAgenda(0)("Id_Agenda"))
                            index = index + Len(Operazione_Riferimento) + Len(CStr(DTAgenda(0)("Id_Agenda"))) + 2
                            File_Name = File_Name.Insert(index, ".")
                        End If

                    End If

                End If

            End If

            '==================================================================================================================================================================
            Return File_Name

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try
    End Function

End Class
