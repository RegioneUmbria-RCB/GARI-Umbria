Imports System.Data.Entity
Imports System.Data.Entity.Migrations
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.DataProviderExtensions
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework

Public Class Mov_Dettaglio_Tecnico_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Id_Agenda As Integer,
                          ByVal Id_Mov As Integer,
                          ByVal Id_Mov_Det As Integer,
                          ByVal Id_Reg_Dettaglio As Integer,
                          ByVal Lotto As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '   Sa_Cod = 0 
        '   Id_Agenda = 0    
        '   Id_Mov = 0
        '   Id_Reg_Dettaglio = 0
        '   Lotto = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Mov_Dettaglio_Tecnico WITH(NOLOCK) ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Id_Mov <> 0 Then
                        StrSQL.Append(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
                    End If

                    If Id_Reg_Dettaglio <> 0 Then
                        StrSQL.Append(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
                    End If

                    If Trim(Lotto) <> "" Then
                        StrSQL.Append(" AND Lotto = '" & Agro_SQL_SaveText(Lotto) & "'   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva Asc ")
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

    Public Function Leggi_x_agenda(ByVal Piva As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_x_agenda()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" select Distinct id_agenda , id_mov , ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")
            StrSQL.Append(" from Mov_Dettaglio_Tecnico ")
            StrSQL.Append("    LEFT OUTER JOIN Avversita on Avversita.av_cod=Mov_Dettaglio_Tecnico.av_cod and Mov_Dettaglio_Tecnico.av_cod<>0  ")
            StrSQL.Append("     LEFT OUTER JOIN GruppoAvversita on GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru and Mov_Dettaglio_Tecnico.av_gru<>0  ")

            StrSQL.Append(" WHERE ( Mov_Dettaglio_Tecnico.av_cod<>0 or Mov_Dettaglio_Tecnico.av_gru<>0) ")
            StrSQL.Append(" AND   (av_Des_Vol<> '' or av_gru_Des<>'') ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ID_Agenda Asc ")
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

    Public Function Leggi_x_agenda_Con_Id_Mov_Det(ByVal Piva As String,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal xOrderBy As String,
                                                  ByRef objParametri As AgronicaCoreParametri
                                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_x_agenda_Con_Id_Mov_Det()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""  
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" select Distinct id_agenda , id_mov , id_mov_det, ISNULL(avversita.av_Des_Vol, '') AS av_Des_Vol , ISNULL(GruppoAvversita.av_gru_Des, '') AS av_gru_Des ")
            StrSQL.Append(" from Mov_Dettaglio_Tecnico ")
            StrSQL.Append("    LEFT OUTER JOIN Avversita on Avversita.av_cod=Mov_Dettaglio_Tecnico.av_cod and Mov_Dettaglio_Tecnico.av_cod<>0  ")
            StrSQL.Append("     LEFT OUTER JOIN GruppoAvversita on GruppoAvversita.av_gru=Mov_Dettaglio_Tecnico.av_gru and Mov_Dettaglio_Tecnico.av_gru<>0  ")

            StrSQL.Append(" WHERE ( Mov_Dettaglio_Tecnico.av_cod<>0 or Mov_Dettaglio_Tecnico.av_gru<>0) ")
            StrSQL.Append(" AND   (av_Des_Vol<> '' or av_gru_Des<>'') ")

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY ID_Agenda Asc ")
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

    Public Function Leggi_x_visite_Rilievi(ByVal Piva As String,
                                            ByVal xFiltroAggiuntivo As String,
                                            ByVal xOrderBy As String,
                                            ByRef objParametri As AgronicaCoreParametri
                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_x_visite_Rilievi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" select Distinct Mov_Dettaglio_Tecnico.id_agenda , Mov_Dettaglio_Tecnico.id_mov , Mov_Dettaglio_Tecnico.id_mov_det, Mov_Dettaglio_Tecnico.ff_classe, agenda.lav_cod ")
            StrSQL.Append(" from Mov_Dettaglio_Tecnico inner join agenda on ")
            StrSQL.Append(" agenda.piva=Mov_Dettaglio_Tecnico.piva and agenda.id_agenda=Mov_Dettaglio_Tecnico.id_agenda  ")

            StrSQL.Append(" WHERE ( Mov_Dettaglio_Tecnico.ff_classe<>0) ")
            StrSQL.Append(" AND agenda.lav_cod in (" & Agro_SQL_SaveNum(LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA) & ", " & Agro_SQL_SaveNum(LAVCOD_DANNI_RACCOLTA) & ", " & Agro_SQL_SaveNum(LAVCOD_RILIEVO_INDICI_MATURITA) & ")  ")

            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Mov_Dettaglio_Tecnico.ID_Agenda Asc ")
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

    Public Function Leggi_x_visite_RilieviConAppezzamento(ByVal Piva As String,
                                                            ByVal xFiltroAggiuntivo As String,
                                                            ByVal xOrderBy As String,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_x_visite_RilieviConAppezzamento()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" select Distinct Mov_Dettaglio_Tecnico.id_agenda , Mov_Dettaglio_Tecnico.id_mov , Mov_Dettaglio_Tecnico.id_mov_det, Mov_Destinazioni.Qta, Mov_Dettaglio_Tecnico.ff_classe, Appezzamento.APP_NOME, agenda.lav_cod ")
            StrSQL.Append(" from Mov_Dettaglio_Tecnico inner join agenda on ")
            StrSQL.Append(" agenda.piva=Mov_Dettaglio_Tecnico.piva and agenda.id_agenda=Mov_Dettaglio_Tecnico.id_agenda  ")
            StrSQL.Append(" LEFT OUTER JOIN Mov_Destinazioni ON Mov_Dettaglio_Tecnico.Piva = Mov_Destinazioni.Piva AND Mov_Dettaglio_Tecnico.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Mov_Dettaglio_Tecnico.Id_Agenda = Mov_Destinazioni.Id_Agenda AND Mov_Dettaglio_Tecnico.Id_Mov = Mov_Destinazioni.Id_Mov AND Mov_Dettaglio_Tecnico.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det ")
            StrSQL.Append(" LEFT OUTER JOIN Appezzamento ON Mov_Destinazioni.PIVA = Appezzamento.Piva AND Mov_Destinazioni.SA_COD = Appezzamento.Sa_Cod AND Mov_Destinazioni.APPEZZA = Appezzamento.Appezza ")

            StrSQL.Append(" WHERE ( Mov_Dettaglio_Tecnico.ff_classe<>0) ")
            StrSQL.Append(" AND agenda.lav_cod in (" & Agro_SQL_SaveNum(LAVCOD_RILIEVO_INDICI_RESE_RACCOLTA) & ", " & Agro_SQL_SaveNum(LAVCOD_DANNI_RACCOLTA) & ", " & Agro_SQL_SaveNum(LAVCOD_RILIEVO_INDICI_MATURITA) & ")  ")

            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Mov_Dettaglio_Tecnico.ID_Agenda Asc ")
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

    Public Function Leggi_x_agenda_fasifenologiche(ByVal Piva As String,
                                                   ByVal xFiltroAggiuntivo As String,
                                                   ByVal xOrderBy As String,
                                                   ByRef objParametri As AgronicaCoreParametri
                                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_x_agenda_fasifenologiche()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;")
            StrSQL.Append(" select Distinct Mov_Dettaglio_Tecnico.id_agenda , Mov_Dettaglio_Tecnico.id_mov , Mov_Dettaglio_Tecnico.id_mov_det, Mov_Dettaglio_Tecnico.ff_classe ")
            StrSQL.Append(" from Mov_Dettaglio_Tecnico inner join agenda on ")
            StrSQL.Append(" agenda.piva=Mov_Dettaglio_Tecnico.piva and agenda.id_agenda=Mov_Dettaglio_Tecnico.id_agenda  ")

            StrSQL.Append(" WHERE ( Mov_Dettaglio_Tecnico.ff_classe<>0) ")
            StrSQL.Append(" AND agenda.lav_cod = " & Agro_SQL_SaveNum(LAVCOD_FASI_FENOLOGICHE) & "   ")

            If Piva <> "" Then
                StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Mov_Dettaglio_Tecnico.ID_Agenda Asc ")
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


    Public Function Leggi_FasiFenologiche(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Appezza As Integer,
                                          ByVal Id_Reg As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_FasiFenologiche()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT DISTINCT " _
                                & " Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, " _
                                & " Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Mov_Dettaglio_Tecnico.Data_Ril," _
                                & " Mov_Dettaglio_Tecnico.FF_Classe, Mov_Destinazioni.Validita_Inizio, Data_Movimento " _
                                & " FROM Movimenti INNER JOIN " _
                                & " Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND " _
                                & " Movimenti.Id_Agenda = Agenda.Id_Agenda INNER JOIN" _
                                & " Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND " _
                                & " Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN" _
                                & " Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND " _
                                & " Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " _
                                & " Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " _
                                & " Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN " _
                                & " Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND " _
                                & " Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND " _
                                & " Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND " _
                                & " Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND " _
                                & " Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det " _
                                & " WHERE (Agenda.Lav_Cod = 79) " _
                                & " AND (Movimenti.Cau_Mov = '2100')" _
                                & " AND (Movimenti.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ) " _
                                & " AND (Movimenti.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) " _
                                & " AND (Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "') " _
                                & " AND (Mov_Destinazioni.Validita_Inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ) " _
                                & " AND (Mov_Destinazioni.Validita_Inizio >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) " _
                                & " AND (Mov_Dettaglio_Tecnico.FF_Classe <> 0) ")

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Mov_Destinazioni.Validita_Inizio Asc")
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


#Region "Trappole - Inneschi"

    Function TrappolaCodicePersonalizzato_from_Trap_Num_Avv(ByVal PIVA As String,
                                                            ByVal Sa_Cod As Integer,
                                                            ByVal Trap_Num As Integer,
                                                            ByVal Sigla_AV As String,
                                                            ByRef objParametri As AgronicaCoreParametri
                                                            ) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.TrappolaCodicePersonalizzato_from_Trap_Num_Avv()"
        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        StrSQL.Length = 0

        StrSQL.Append(" SELECT Agenda.Des_Lib, Agenda.Lav_Cod, Movimenti.*, Movimenti_Dettagli.*, Mov_Dettaglio_Tecnico.* ")
        StrSQL.Append(" FROM  Agenda, Movimenti, Mov_Dettaglio_Tecnico, Movimenti_Dettagli ")
        'StrSQL.Append(" WHERE Mov_Dettaglio_Tecnico.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
        'StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
        StrSQL.Append(" WHERE   Agenda.Lav_Cod IN ( 107 , 118 , 121 , 122 , 150 ) " & "  ")

        'Join sulla Piva
        StrSQL.Append(" AND   Agenda.Piva = Movimenti.Piva ")
        StrSQL.Append(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
        StrSQL.Append(" AND   Movimenti_Dettagli.Piva = Mov_Dettaglio_Tecnico.Piva ")

        'Join sul Sa_Cod
        StrSQL.Append(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")

        'Join sul Id_Agenda
        StrSQL.Append(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
        StrSQL.Append(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
        StrSQL.Append(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")

        'Join su Id_Mov
        StrSQL.Append(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
        StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")

        'Join su Id_Mov_Det
        StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")


        If PIVA <> "" Then
            StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
        End If

        If Sa_Cod <> 0 Then
            StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
        End If

        If Trap_Num <> 0 Then
            StrSQL.Append(" AND  Mov_Dettaglio_Tecnico.Trap_Num =  " & Agro_SQL_SaveNum(Trap_Num) & "   ")
        End If

        If Trim(Sigla_AV) <> "" Then
            StrSQL.Append(" AND  Mov_Dettaglio_Tecnico.Sigla_AV = '" & Agro_SQL_SaveText(Sigla_AV) & "'   ")
        End If

        ' ''--------------------------------------------------------------------------
        ''Select Case objParametri.FlagVisibilita
        ''    Case enumVisibilita.Visibilita_SoloNonCancellati
        ''        StrSQL.Append(" AND   dbo.Agenda.Inviato >=0 ")
        ''        StrSQL.Append(" AND   dbo.Movimenti.Inviato >=0 ")
        ''        StrSQL.Append(" AND   dbo.Mov_Dettaglio_Tecnico.Inviato >=0 ")
        ''        StrSQL.Append(" AND   dbo.Movimenti_Dettagli.Inviato >=0 ")
        ''    Case enumVisibilita.Visibilita_SoloCancellati
        ''        StrSQL.Append(" AND   dbo.Agenda.Inviato =-1 ")
        ''        StrSQL.Append(" AND   dbo.Movimenti.Inviato =-1 ")
        ''        StrSQL.Append(" AND   dbo.Mov_Dettaglio_Tecnico.Inviato =-1 ")
        ''        StrSQL.Append(" AND   dbo.Movimenti_Dettagli.Inviato =-1 ")
        ''    Case enumVisibilita.Visibilita_Tutti
        ''        '...................................
        ''    Case Else
        ''        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
        ''End Select
        ' ''--------------------------------------------------------------------------

        dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return CInt(dt.Rows(0).Item("Freatimetro"))
        End If

        Return 0

    End Function


    Public Function Leggi_RilieviAvversita(ByVal Piva As String,
                                           ByVal Sa_Cod As Integer,
                                           ByVal Appezza As Integer,
                                           ByVal Id_Reg As Integer,
                                           ByVal Udm_Cod As Integer,
                                           ByVal Av_Cod As Integer,
                                           ByVal Validita_Inizio As Date,
                                           ByVal Validita_Fine As Date,
                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_RilieviAvversita()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT DISTINCT " &
                                " Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, " &
                                " Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Qta,  " &
                                " Mov_Dettaglio_Tecnico.FF_Classe, Mov_Dettaglio_Tecnico.Dett_Cod,  Mov_Destinazioni.Validita_Inizio " &
                                " FROM Movimenti INNER JOIN " &
                                " Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND " &
                                " Movimenti.Id_Agenda = Agenda.Id_Agenda INNER JOIN" &
                                " Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND " &
                                " Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN" &
                                " Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND " &
                                " Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " &
                                " Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " &
                                " Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN " &
                                " Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND " &
                                " Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND " &
                                " Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND " &
                                " Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND " &
                                " Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det " &
                                " WHERE (Agenda.Lav_Cod = 113) " &
                                " AND (Movimenti.Cau_Mov = '2100')" &
                                " AND (Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "') " &
                                " AND (Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")" &
                                " AND (Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ) " &
                                " AND (Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) " &
                                " AND (Mov_Dettaglio_Tecnico.Av_Cod <> 0) ")

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
                    End If

                    If Id_Reg <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Movimenti_dettagli.Udm_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Av_Cod <> 0 Then
                        StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Mov_Destinazioni.Validita_Inizio Asc")
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


    Public Function Leggi_RilieviTrappole(ByVal Piva As String,
                                          ByVal Sa_Cod As Integer,
                                          ByVal Appezza_Localizzazione As Integer,
                                          ByVal Id_Reg_Localizzazione As Integer,
                                          ByVal Udm_Cod As Integer,
                                          ByVal Av_Cod As Integer,
                                          ByVal Veg_Cod As Integer,
                                          ByVal Validita_Inizio As Date,
                                          ByVal Validita_Fine As Date,
                                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                          ByVal xFiltroAggiuntivo As String,
                                          ByVal xOrderBy As String,
                                          ByRef objParametri As AgronicaCoreParametri
                                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_RilieviTrappole()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT DISTINCT " &
                              " Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Movimenti.Data_Movimento, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, " &
                             " Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Qta, Mov_Dettaglio_Tecnico.Qta_Ril, " &
                             " Mov_Destinazioni.Validita_Inizio " &
                             " FROM Movimenti INNER JOIN " &
                             " Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND " &
                             " Movimenti.Id_Agenda = Agenda.Id_Agenda INNER JOIN" &
                             " Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND " &
                             " Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN" &
                             " Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND " &
                             " Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " &
                             " Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " &
                             " Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN " &
                             " Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND " &
                             " Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND " &
                             " Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND " &
                             " Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND " &
                             " Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det INNER JOIN " &
                             " Reg_Impianti ON Reg_Impianti.Piva = Mov_Destinazioni.Piva AND" &
                             " Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod AND" &
                             " Reg_Impianti.Appezza = Mov_Destinazioni.Appezza AND" &
                             " Reg_Impianti.Id_Reg = Mov_Destinazioni.Id_Destinazione INNER JOIN " &
                             " Cultivar ON Reg_Impianti.Cul_Cod = Cultivar.Cul_Cod " &
                             " WHERE (Agenda.Lav_Cod = 110) " &
                             " AND (Movimenti.Cau_Mov = '2100')" &
                             " AND (Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "') " &
                             " AND (Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")" &
                             " AND (Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ) " &
                             " AND (Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) " &
                             " AND (Mov_Dettaglio_Tecnico.Av_Cod <> 0) ")

                    If Appezza_Localizzazione <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza_Localizzazione) & "   ")
                    End If

                    If Id_Reg_Localizzazione <> 0 Then
                        StrSQL.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg_Localizzazione) & "   ")
                    End If

                    If Udm_Cod <> 0 Then
                        StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Dett_Cod = " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
                    End If

                    If Av_Cod <> 0 Then
                        StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "   ")
                    End If

                    If Veg_Cod <> 0 Then
                        StrSQL.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Mov_Dettaglio_Tecnico.Qta_Ril Desc, Mov_Destinazioni.Validita_Inizio Asc ")
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

    Public Function Leggi_ConfusioneDisorientamentoSessuale(ByVal Piva As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Appezza As Integer,
                                       ByVal Id_Reg As Integer,
                                       ByVal Lav_Cod As Integer,
                                       ByVal Av_Cod As Integer,
                                       ByVal Validita_Inizio As Date,
                                       ByVal Validita_Fine As Date,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Leggi_ConfusioneDisorientamentoSessuale()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0
            StrSQL.Append(" SELECT DISTINCT " &
                                " Agenda.Id_Agenda, Agenda.Lav_Cod, Agenda.des_lib, Mov_Destinazioni.Piva, Mov_Destinazioni.Sa_Cod, " &
                                " Mov_Destinazioni.Appezza, Mov_Destinazioni.Id_Destinazione, Mov_Destinazioni.Qta,  " &
                                " Mov_Dettaglio_Tecnico.FF_Classe, Mov_Dettaglio_Tecnico.Dett_Cod,  Mov_Destinazioni.Validita_Inizio " &
                                " FROM Movimenti INNER JOIN " &
                                " Agenda ON Movimenti.PIVA = Agenda.PIVA AND Movimenti.Sa_Cod = Agenda.Sa_Cod AND " &
                                " Movimenti.Id_Agenda = Agenda.Id_Agenda INNER JOIN" &
                                " Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod AND " &
                                " Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov INNER JOIN" &
                                " Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND " &
                                " Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND " &
                                " Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND " &
                                " Movimenti_dettagli.Id_Mov_Det = dbo.Mov_Destinazioni.Id_Mov_Det INNER JOIN " &
                                " Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND " &
                                " Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND " &
                                " Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND " &
                                " Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND " &
                                " Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det " &
                                " WHERE (Movimenti.Cau_Mov = '2050')" &
                                " AND (Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "') " &
                                " AND (Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & ")" &
                                " AND (Movimenti.Data_Movimento <= " & Agro_SQL_SaveDate(Validita_Fine) & "  ) " &
                                " AND (Movimenti.Data_Movimento >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ) " &
                                " AND (Mov_Dettaglio_Tecnico.Av_Cod <> 0) ")

            If Appezza <> 0 Then
                StrSQL.Append(" AND Mov_Destinazioni.Appezza = " & Agro_SQL_SaveNum(Appezza) & "   ")
            End If

            If Id_Reg <> 0 Then
                StrSQL.Append(" AND Mov_Destinazioni.Id_Destinazione = " & Agro_SQL_SaveNum(Id_Reg) & "   ")
            End If

            If Lav_Cod <> 0 Then
                StrSQL.Append(" AND Agenda.Lav_Cod = " & Agro_SQL_SaveNum(Lav_Cod) & "   ")
            Else
                StrSQL.Append(" AND Agenda.Lav_Cod IN (" & Agro_SQL_SaveNum(LAVCOD_DISORIENTAMENTO_SESSUALE) & "," & Agro_SQL_SaveNum(LAVCOD_CONFUSIONE_SESSUALE) & ") ")
            End If

            If Av_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & "   ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.Append(" ORDER BY Mov_Destinazioni.Validita_Inizio Asc")
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
    ''' Restituisce una tabella con le trappole installate in join con i reinneschi.
    ''' Se Una trappola non ha reinneschi le colonne relative al reinnesco sono null,
    ''' se una trappola ha più reinneschi si ha una riga per ciascun reinnesco
    ''' 
    ''' </summary>
    ''' <param name="Piva">Piva azienda,  fa parte della chiave di una trappola assieme alla Sigla_AV e sacod </param>
    ''' <param name="Sa_cod">Centro aziendale, fa parte della chiave di una trappola assieme alla Sigla_AV e Piva</param>
    ''' <param name="Sigla_AV">Sigla avversità della trappola (fa parte della chiave di una trappola assieme alla partita iva e sacod ) </param>
    ''' <param name="Appezza">Appezzamento di destinazione della trappola</param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="Data_Di_Controllo">Data di controllo delle trappole e reinneschi, 
    '''  Vengono considerate le trappole appartenenti all'anno corrente e che siano state installate prima della data selezionata,
    ''' i reinneschi mostrati sono relativi a quelle trappole e 
    ''' non vengono mostrati i reinneschi successivi alla data selezionata</param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Trappole_Installate_X_Reinneschi(ByVal Piva As String,
                                                     ByVal Sa_cod As Integer,
                                                     ByVal Sigla_AV As String,
                                                     ByVal Appezza As String,
                                                     ByVal Veg_Cod As Integer,
                                                     ByVal Cul_Cod As Integer,
                                                     ByVal Gru_Cod As Integer,
                                                     ByVal Data_Di_Controllo As Date,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Trappole_Installate_X_Reinneschi()"

        Dim messaggioErrore As String = ""
        Dim strQuery As String = ""
        Dim dt As DataTable

        Try

            strQuery = Trapole_Installate_X_Reinneschi_Query(False,
                                                             Piva,
                                                             Sa_cod,
                                                             Sigla_AV,
                                                             Appezza,
                                                             Veg_Cod,
                                                             Cul_Cod,
                                                             Gru_Cod,
                                                             Data_Di_Controllo,
                                                             xFiltroAggiuntivo,
                                                             objParametri)

            strQuery &= " ORDER BY Agenda.PIVA, Agenda.Sa_Cod, Mov_Dettaglio_Tecnico.Sigla_AV, Mov_Dettaglio_Tecnico.Trap_Num 	 "

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, nomeRoutine)
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
    ''' Restituisce una tabella con le trappole installate e l'ultimo reinnesco 
    ''' </summary>
    ''' <param name="Piva">Piva azienda,  fa parte della chiave di una trappola assieme alla Sigla_AV e sacod </param>
    ''' <param name="Sa_cod">Centro aziendale, fa parte della chiave di una trappola assieme alla Sigla_AV e Piva</param>
    ''' <param name="Sigla_AV">Sigla avversità della trappola (fa parte della chiave di una trappola assieme alla partita iva e sacod ) </param>
    ''' <param name="Appezza">Appezzamento di destinazione della trappola</param>
    ''' <param name="Veg_Cod"></param>
    ''' <param name="Cul_Cod"></param>
    ''' <param name="Gru_Cod"></param>
    ''' <param name="Data_Di_Controllo">Data di controllo delle trappole e reinneschi, 
    '''  Vengono considerate le trappole appartenenti all'anno corrente e che siano state installate prima della data selezionata,
    ''' il reinnesco è l'ultimo relativo a quella trappola e  non successivo alla data selezionata </param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Trappole_Installate_X_Ultimo_Reinnesco(ByVal Piva As String,
                                                           ByVal Sa_cod As Integer,
                                                           ByVal Sigla_AV As String,
                                                           ByVal Appezza As String,
                                                           ByVal Veg_Cod As Integer,
                                                           ByVal Cul_Cod As Integer,
                                                           ByVal Gru_Cod As Integer,
                                                           ByVal Data_Di_Controllo As Date,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As DataTable

        'non filtra solo gli ultimi reinneschi
        Throw New NotImplementedException

        Dim nomeRoutine As String = "AgronicaCoreMetaSchemaDAL.Trappole_Installate_X_Ultimo_Reinnesco()"

        Dim messaggioErrore As String = ""
        Dim strQuery As String = ""
        Dim dt As DataTable

        Try

            strQuery = Trapole_Installate_X_Reinneschi_Query(True,
                                                        Piva,
                                                        Sa_cod,
                                                        Sigla_AV,
                                                        Appezza,
                                                        Veg_Cod,
                                                        Cul_Cod,
                                                        Gru_Cod,
                                                        Data_Di_Controllo,
                                                        xFiltroAggiuntivo,
                                                        objParametri)

            strQuery &= " ORDER BY Agenda.PIVA, Agenda.Sa_Cod, Mov_Dettaglio_Tecnico.Sigla_AV, Mov_Dettaglio_Tecnico.Trap_Num 	 "

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strQuery.ToString, nomeRoutine)
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
    ''' 
    ''' 
    ''' </summary>
    ''' <param name="SoloUltimoReinnesco">join con solo l'ultimo reinnesco in base alla data di riferimento, fare attenzione ad evitare reinneschi nella stessa data</param>
    ''' <remarks></remarks>
    Private Function Trapole_Installate_X_Reinneschi_Query(
                                                    ByVal SoloUltimoReinnesco As Boolean,
                                                    ByVal Piva As String,
                                                    ByVal Sa_cod As Integer,
                                                    ByVal Sigla_AV As String,
                                                    ByVal Appezza As String,
                                                    ByVal Veg_Cod As Integer,
                                                    ByVal Cul_Cod As Integer,
                                                    ByVal Gru_Cod As Integer,
                                                    ByVal Data_Di_Controllo As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As String

        Dim strQuery As New Text.StringBuilder

        'questa query restituisce una riga per ciascuna installazione trappola e piu righe nel caso di piu reinneschi per la trappola
        strQuery.Append(" SELECT     Agenda.PIVA, Agenda.Sa_Cod, Centri_Aziendali.sa_nome, Reg_Impianti.ID_REG, Reg_Impianti.APPEZZA, Appezzamento.APP_NOME, Reg_Impianti.CUL_COD, 	 ")
        strQuery.Append("                       Cultivar.Cul_Des, Cultivar.Veg_Cod, SpecieVegetali.Veg_Des, SpecieVegetali.Gru_Cod,Agenda.Lav_Cod, Agenda.Id_Agenda, Agenda.des_lib, CONVERT(VARCHAR, Agenda.Validita_Inizio, 103) as Validita_Inizio,	 ")
        strQuery.Append("                       CONVERT(VARCHAR, Agenda.Validita_Fine, 103) as Validita_Fine, Movimenti.Id_Mov, Movimenti.Cau_Mov, Movimenti.Mov_Desc, Movimenti_dettagli.Id_Mov_Det, Movimenti_dettagli.Elem_Cod, 	 ")
        strQuery.Append("                       Movimenti_dettagli.Pro_Cod, Mov_Dettaglio_Tecnico.Sigla_AV, Mov_Dettaglio_Tecnico.Trap_Num, Mov_Dettaglio_Tecnico.Freatimetro, 	 ")
        strQuery.Append("                       Mov_Dettaglio_Tecnico.Ditta_Cod, Mov_Dettaglio_Tecnico.Dose, Mov_Dettaglio_Tecnico.Inn1_Data, Mov_Dettaglio_Tecnico.Av_Cod, 	 ")
        strQuery.Append("                       Mov_Dettaglio_Tecnico.Av_Gru, Mov_Dettaglio_Tecnico.Extra_Str, Reg_Impianti.Sup_Imp, Mov_Dettaglio_Tecnico.Id_Reg_Dettaglio, 	 ")
        strQuery.Append("                       Avversita.Av_Des_Vol, Avversita.Abbreviazione, Avversita.Av_Des_Lat, Trappole.TRAP_COD, Trappole.TRAP_DES, Trappole.TRAP_DUR, 	 ")
        strQuery.Append("                       Trappole.USO, Movimenti_dettagli.Udm_Cod,  Reinnesco.Lav_Cod AS Reinnesco_Lav_Cod, 	 ")
        strQuery.Append("                       Reinnesco.des_lib AS Reinnesco_des_lib, Reinnesco.Id_Agenda AS Reinnesco_Id_Agenda, Reinnesco.Freatimetro AS Reinnesco_Freatimetro, 	 ")
        strQuery.Append("                       Reinnesco.Sigla_AV AS Reinnesco_Sigla_AV, Reinnesco.PIVA AS Reinnesco_PIVA, Reinnesco.Sa_Cod AS Reinnesco_Sa_Cod, 	 ")
        strQuery.Append("                       Reinnesco.Trap_Num AS Reinnesco_Trap_Num, Reinnesco.Dose AS Reinnesco_Dose, CONVERT(VARCHAR, Reinnesco.Data_Reinnesco, 103) as Data_Reinnesco,	 ")
        strQuery.Append("                       Mov_Destinazioni.Qta2	 ")
        strQuery.Append(" FROM         Agenda INNER JOIN	 ")
        strQuery.Append("                       Movimenti ON Agenda.PIVA = Movimenti.PIVA AND Agenda.Id_Agenda = Movimenti.Id_Agenda AND Agenda.Sa_Cod = Movimenti.Sa_Cod INNER JOIN	 ")
        strQuery.Append("                       Movimenti_dettagli ON Movimenti.PIVA = Movimenti_dettagli.PIVA AND Movimenti.Id_Agenda = Movimenti_dettagli.Id_Agenda AND 	 ")
        strQuery.Append("                       Movimenti.Id_Mov = Movimenti_dettagli.Id_Mov AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod INNER JOIN	 ")
        strQuery.Append("                       Mov_Dettaglio_Tecnico ON Movimenti_dettagli.PIVA = Mov_Dettaglio_Tecnico.Piva AND 	 ")
        strQuery.Append("                       Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod AND Movimenti_dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda AND 	 ")
        strQuery.Append("                       Movimenti_dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det INNER JOIN	 ")
        strQuery.Append("                       Mov_Destinazioni ON Movimenti_dettagli.PIVA = Mov_Destinazioni.Piva AND Movimenti_dettagli.Id_Agenda = Mov_Destinazioni.Id_Agenda AND 	 ")
        strQuery.Append("                       Movimenti_dettagli.Id_Mov = Mov_Destinazioni.Id_Mov AND Movimenti_dettagli.Id_Mov_Det = Mov_Destinazioni.Id_Mov_Det AND 	 ")
        strQuery.Append("                       Movimenti_dettagli.Sa_Cod = Mov_Destinazioni.Sa_Cod INNER JOIN	 ")
        strQuery.Append("                       Reg_Impianti ON Mov_Destinazioni.Piva = Reg_Impianti.PIVA AND Mov_Destinazioni.Sa_Cod = Reg_Impianti.SA_COD AND 	 ")
        strQuery.Append("                       Mov_Destinazioni.Appezza = Reg_Impianti.APPEZZA AND Mov_Destinazioni.Id_Destinazione = Reg_Impianti.ID_REG INNER JOIN	 ")
        strQuery.Append("                       Trappole ON Movimenti_dettagli.Pro_Cod = Trappole.TRAP_COD INNER JOIN	 ")
        strQuery.Append("                       Cultivar ON Reg_Impianti.CUL_COD = Cultivar.Cul_Cod INNER JOIN	 ")
        strQuery.Append("                       SpecieVegetali ON Cultivar.Veg_Cod = SpecieVegetali.Veg_Cod INNER JOIN	 ")
        strQuery.Append("                       Appezzamento ON Reg_Impianti.PIVA = Appezzamento.PIVA AND Reg_Impianti.SA_COD = Appezzamento.SA_COD AND 	 ")
        strQuery.Append("                       Reg_Impianti.APPEZZA = Appezzamento.APPEZZA INNER JOIN	 ")
        strQuery.Append("                       Avversita ON Mov_Dettaglio_Tecnico.Av_Cod = Avversita.Av_Cod INNER JOIN	 ")
        strQuery.Append("                       Centri_Aziendali ON Agenda.PIVA = Centri_Aziendali.PIVA AND Agenda.Sa_Cod = Centri_Aziendali.sa_cod LEFT OUTER JOIN	 ")
        strQuery.Append("                           (       ")

        If Not SoloUltimoReinnesco Then
            'join con tutti i reinneschi sulla trappola in base alla data di riferimento
            strQuery.Append(" SELECT   Agenda_1.Validita_Inizio AS Data_Reinnesco, Agenda_1.Lav_Cod, Agenda_1.des_lib, Agenda_1.Id_Agenda, 	 ")
            strQuery.Append("                Mov_Dettaglio_Tecnico_1.Freatimetro, Mov_Dettaglio_Tecnico_1.Trap_Num, Mov_Dettaglio_Tecnico_1.Sigla_AV, Agenda_1.PIVA, 	 ")
            strQuery.Append("                Agenda_1.Sa_Cod, Mov_Dettaglio_Tecnico_1.Dose	 ")
            strQuery.Append(" FROM          Agenda AS Agenda_1 INNER JOIN	 ")
            strQuery.Append("               Mov_Dettaglio_Tecnico AS Mov_Dettaglio_Tecnico_1 ON Mov_Dettaglio_Tecnico_1.Piva = Agenda_1.PIVA AND 	 ")
            strQuery.Append("               Mov_Dettaglio_Tecnico_1.Sa_Cod = Agenda_1.Sa_Cod AND Mov_Dettaglio_Tecnico_1.Id_Agenda = Agenda_1.Id_Agenda	 ")
            'seleziono le lavorazioni di reinnesco trappole
            strQuery.Append("  WHERE      (Agenda_1.Lav_Cod = 150)   AND                      	 ")
            '                              filtro solo i reinneschi effettuati precedentemente alla data di controllo
            strQuery.Append("              Agenda_1.Validita_Inizio <=  " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ")
            'filtro solo le trappole dell'anno della data di controllo
            strQuery.Append(" AND YEAR ( Agenda_1.Validita_Inizio ) >=  YEAR (  " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ) ")

        Else
            'non filtra solo gli ultimi reinneschi
            Throw New NotImplementedException

            ''join con solo l'ultimo reinnesco in base alla data di riferimento, fare attenzione ad evitare reinneschi nella stessa data
            'strQuery.Append(" SELECT        Agenda_1.Validita_Inizio AS Data_Reinnesco, Agenda_1.Lav_Cod, 	 ")
            'strQuery.Append(" 			    Agenda_1.des_lib, Agenda_1.Id_Agenda, 	 ")
            'strQuery.Append("               Mov_Dettaglio_Tecnico_1.Freatimetro, 	 ")
            'strQuery.Append("               Mov_Dettaglio_Tecnico_1.Trap_Num, Mov_Dettaglio_Tecnico_1.Sigla_AV, Agenda_1.PIVA, 	 ")
            'strQuery.Append("               Agenda_1.Sa_Cod, Mov_Dettaglio_Tecnico_1.Dose	 ")
            'strQuery.Append("  FROM         Agenda AS Agenda_1 INNER JOIN	 ")
            'strQuery.Append("               Mov_Dettaglio_Tecnico AS Mov_Dettaglio_Tecnico_1 ON Mov_Dettaglio_Tecnico_1.Piva = Agenda_1.PIVA AND 	 ")
            'strQuery.Append("               Mov_Dettaglio_Tecnico_1.Sa_Cod = Agenda_1.Sa_Cod AND Mov_Dettaglio_Tecnico_1.Id_Agenda = Agenda_1.Id_Agenda	 ")
            'strQuery.Append(" 				inner join(	                                                                ")
            'strQuery.Append(" 				        SELECT     max(Agenda_1.Validita_Inizio) AS Data_Reinnesco, 	 ")
            'strQuery.Append(" 				                   Agenda_1.PIVA, Agenda_1.Sa_Cod, Mov_Dettaglio_Tecnico_1.Sigla_AV, Mov_Dettaglio_Tecnico_1.Trap_Num	 ")
            'strQuery.Append(" 				        FROM       Agenda AS Agenda_1 INNER JOIN	 ")
            'strQuery.Append("                                  Mov_Dettaglio_Tecnico AS Mov_Dettaglio_Tecnico_1 ON Mov_Dettaglio_Tecnico_1.Piva = Agenda_1.PIVA AND 	 ")
            'strQuery.Append("                                  Mov_Dettaglio_Tecnico_1.Sa_Cod = Agenda_1.Sa_Cod AND Mov_Dettaglio_Tecnico_1.Id_Agenda = Agenda_1.Id_Agenda	 ")
            'strQuery.Append("                        WHERE     (Agenda_1.Lav_Cod = 150)   AND                      	 ")
            ''                                           filtro solo i reinneschi effettuati precedentemente alla data di controllo
            'strQuery.Append("                                  Agenda_1.Validita_Inizio <=  " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ")
            'strQuery.Append(" 				         GROUP BY Agenda_1.PIVA, Agenda_1.Sa_Cod, Mov_Dettaglio_Tecnico_1.Sigla_AV, Mov_Dettaglio_Tecnico_1.Trap_Num	 ")
            'strQuery.Append(" 				)as TabInterna on 	 ")
            'strQuery.Append(" 				Agenda_1.PIVA =  TabInterna.PIVA and 	 ")
            'strQuery.Append(" 				Agenda_1.Sa_Cod =  TabInterna.Sa_Cod and 	 ")
            'strQuery.Append(" 				Mov_Dettaglio_Tecnico_1.Sigla_AV =  TabInterna.Sigla_AV and 	 ")
            'strQuery.Append(" 				Mov_Dettaglio_Tecnico_1.Trap_Num = TabInterna.Trap_Num and 	 ")
            'strQuery.Append(" 				Agenda_1.Validita_Inizio = TabInterna.Data_Reinnesco	 ")
            ''                               le lavorazioni di reinnesco trappole
            'strQuery.Append("  WHERE        (Agenda_1.Lav_Cod = 150)   AND                      	 ")
            ''                               filtro solo i reinneschi effettuati precedentemente alla data di controllo
            'strQuery.Append("               Agenda_1.Validita_Inizio <=  " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ")

        End If

        strQuery.Append("                             ) AS Reinnesco ON Mov_Dettaglio_Tecnico.Trap_Num = Reinnesco.Trap_Num AND 	 ")
        strQuery.Append("                       Mov_Dettaglio_Tecnico.Sigla_AV = Reinnesco.Sigla_AV AND Mov_Dettaglio_Tecnico.Piva = Reinnesco.PIVA AND 	 ")
        strQuery.Append("                       Mov_Dettaglio_Tecnico.Sa_Cod = Reinnesco.Sa_Cod	 ")

        'seleziono le lavorazioni di installazione trappole e catture di massa
        strQuery.Append(" WHERE     (Agenda.Lav_Cod IN (107, 122))   	 ")

        'filtro solo le trappole installate precedentemente alla data di controllo
        strQuery.Append(" AND Agenda.Validita_Inizio <=  " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ")
        'filtro solo le trappole dell'anno della data di controllo
        strQuery.Append(" AND YEAR ( Agenda.Validita_Inizio ) >=  YEAR (  " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ) ")
        'strQuery.Append(" AND Agenda.Validita_Fine >= " & Agro_SQL_SaveDate(Data_Di_Controllo) & " ")

        If Piva <> "" Then
            strQuery.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
        End If

        If Sa_cod <> 0 Then
            strQuery.Append(" AND Mov_Dettaglio_Tecnico.SA_COD = " & Agro_SQL_SaveNum(Sa_cod) & " ")
        End If

        If Appezza <> "" Then
            strQuery.Append(" AND Reg_Impianti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
        End If

        If Sigla_AV <> "" Then
            strQuery.Append(" AND Mov_Dettaglio_Tecnico.Sigla_AV = '" & Agro_SQL_SaveText(Sigla_AV) & "' ")
        End If

        If Cul_Cod <> 0 Then
            strQuery.Append(" AND Reg_Impianti.CUL_COD = " & Agro_SQL_SaveNum(Cul_Cod) & " ")
        End If

        If Veg_Cod <> 0 Then
            strQuery.Append(" AND Cultivar.Veg_Cod = " & Agro_SQL_SaveNum(Veg_Cod) & " ")
        End If

        If Gru_Cod <> 0 Then
            strQuery.Append(" AND SpecieVegetali.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod) & " ")
        End If

        '--------------------------------------------------------------------------
        If xFiltroAggiuntivo <> "" Then
            strQuery.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If
        '--------------------------------------------------------------------------

        Return strQuery.ToString

    End Function


    Public Function RilevaNumeroTrappola(ByVal piva As String,
                                         ByVal sacod As String,
                                         ByVal codavversita As String,
                                         ByRef objParametri_Server As AgronicaCoreParametri
                                         ) As Integer

        Dim dt As DataTable
        dt = Last_TrapNum(CStr(piva), CInt(sacod),
                          codavversita,
                          enumSelezioneVariabile.Selezione_JoinCompleta,
                          "", "", objParametri_Server)

        If dt Is Nothing OrElse dt.Rows.Count = 0 Then
            Return 1
        Else
            Dim s As Integer
            s = CInt(dt.Rows(0).Item("Trap_Num"))
            s += 1
            Return s
        End If

    End Function


    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="PIVA"></param>
    ''' <param name="Sa_Cod"></param>
    ''' <param name="Sigla_AV"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	14/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Last_TrapNum(ByVal PIVA As String,
                                 ByVal Sa_Cod As Integer,
                                 ByVal Sigla_AV As String,
                                 ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreParametri
                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Last_TrapNum()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Sigla_AV = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case enumSelezioneVariabile.Selezione_TabellaCompleta


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Agenda.Des_Lib, Agenda.Lav_Cod, Movimenti.*, Movimenti_Dettagli.*, Mov_Dettaglio_Tecnico.* ")
                    StrSQL.Append(" FROM  Agenda, Movimenti, Mov_Dettaglio_Tecnico, Movimenti_Dettagli ")
                    StrSQL.Append(" WHERE Mov_Dettaglio_Tecnico.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Mov_Dettaglio_Tecnico.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Agenda.Lav_Cod IN ( 107 , 118 , 121 , 122 , 150 ) " & "  ")

                    'Join sulla Piva
                    StrSQL.Append(" AND   Agenda.Piva = Movimenti.Piva ")
                    StrSQL.Append(" AND   Movimenti.Piva = Movimenti_Dettagli.Piva ")
                    StrSQL.Append(" AND   Movimenti_Dettagli.Piva = Mov_Dettaglio_Tecnico.Piva ")

                    'Join sul Sa_Cod
                    StrSQL.Append(" AND   Movimenti_Dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")

                    'Join sul Id_Agenda
                    StrSQL.Append(" AND   Agenda.Id_Agenda = Movimenti.Id_Agenda ")
                    StrSQL.Append(" AND   Movimenti.Id_Agenda = Movimenti_Dettagli.Id_Agenda ")
                    StrSQL.Append(" AND   Movimenti_Dettagli.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda ")

                    'Join su Id_Mov
                    StrSQL.Append(" AND   Movimenti.Id_Mov = Movimenti_Dettagli.Id_Mov ")
                    StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov ")

                    'Join su Id_Mov_Det
                    StrSQL.Append(" AND   Movimenti_Dettagli.Id_Mov_Det = Mov_Dettaglio_Tecnico.Id_Mov_Det ")


                    If PIVA <> "" Then
                        StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Trim(Sigla_AV) <> "" Then
                        StrSQL.Append(" AND  Mov_Dettaglio_Tecnico.Sigla_AV = '" & Agro_SQL_SaveText(Sigla_AV) & "'   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   dbo.Agenda.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.Movimenti.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.Mov_Dettaglio_Tecnico.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.Movimenti_Dettagli.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   dbo.Agenda.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.Movimenti.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.Mov_Dettaglio_Tecnico.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.Movimenti_Dettagli.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Mov_Dettaglio_Tecnico.Trap_Num DESC ")
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

    Function Trappole_Formulati_X_ImpiantiInfluenza(ByVal PIVA As String,
                                                    ByVal Sa_Cod As Integer,
                                                    ByVal Id_Agenda As Integer,
                                                    ByVal Raccoglitore_Cod As Integer,
                                                    ByVal Pro_Cod As Integer,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByVal objParametri As AgronicaCoreParametri,
                                                        Optional ByVal EstraiLotto As Boolean = False,
                                                        Optional joinImpiantixBIO As Boolean = False,
                                                        Optional CaricaNrDomandaACA As Boolean = False,
                                                        Optional sql2017 As Boolean = False
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Trappole_Formulati_X_ImpiantiInfluenza()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  ")

            StrSQL.AppendLine("     Agenda.Id_Agenda ")
            StrSQL.AppendLine("   , ISNULL(Agenda.Raccoglitore_Cod, 0) AS Raccoglitore_Cod ")
            StrSQL.AppendLine("	, Agenda.Piva AS Azienda ")
            StrSQL.AppendLine("   , Agenda.Sa_Cod AS Centro_Aziendale ")
            StrSQL.AppendLine("   , Mov_Destinazioni.Appezza AS Imp_Influ_Appezza ")
            StrSQL.AppendLine("   , Mov_Destinazioni.Id_Destinazione AS Imp_Influ_Id_Destinazione ")

            StrSQL.AppendLine("   , Appezzamento.app_nome ")
            StrSQL.AppendLine("   , ISNULL(Campi.Campo_Des,'') AS campo_des ")
            StrSQL.AppendLine("   , ISNULL((SELECT TOP 1 Appezzamento_Codici.val_cod FROM Appezzamento_Codici  ")
            StrSQL.AppendLine("             WHERE (Appezzamento_Codici.PIVA = Appezzamento.PIVA) ")
            StrSQL.AppendLine("             AND (Appezzamento_Codici.sa_cod = Appezzamento.sa_cod) ")
            StrSQL.AppendLine("             AND (Appezzamento_Codici.appezza = Appezzamento.appezza) ")
            StrSQL.AppendLine($"             AND (Appezzamento_Codici.id_cod = {CInt(enum_CodiciAnagrafe.Riferimento_Alfanumerico_Appezzamento)}) ), '') AS App_Nome_Breve ")

            If EstraiLotto Then
                StrSQL.Append(", ISNULL(Imprese_Progetti.Progetto_Nome,'') AS Lotto " & vbCrLf)
            End If

            If CaricaNrDomandaACA Then
                StrSQL.AppendLine(", ISNULL(CodiciACA.Nr_domanda_ACA, '') AS Nr_domanda_ACA ")
            End If

            StrSQL.AppendLine(" FROM  Agenda  ")

            StrSQL.AppendLine(" INNER JOIN Movimenti ON ")
            StrSQL.AppendLine("      Agenda.Piva  = Movimenti.Piva ")
            StrSQL.AppendLine("  AND Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine("  AND Agenda.Id_Agenda =  Movimenti.Id_Agenda ")

            StrSQL.AppendLine(" INNER JOIN Movimenti_dettagli ON ")
            StrSQL.AppendLine("     Movimenti.Piva  = Movimenti_dettagli.Piva ")
            StrSQL.AppendLine(" AND Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            StrSQL.AppendLine(" AND Movimenti.Id_Agenda =  Movimenti_dettagli.Id_Agenda ")
            StrSQL.AppendLine(" AND Movimenti.Id_Mov =  Movimenti_dettagli.Id_Mov ")

            StrSQL.AppendLine(" INNER JOIN Mov_Destinazioni ON")
            StrSQL.AppendLine("      Mov_Destinazioni.Piva  = Movimenti_dettagli.Piva ")
            StrSQL.AppendLine("  AND Mov_Destinazioni.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            StrSQL.AppendLine("  AND Mov_Destinazioni.Id_Agenda =  Movimenti_dettagli.Id_Agenda ")
            StrSQL.AppendLine("  AND Mov_Destinazioni.Id_Mov =  Movimenti_dettagli.Id_Mov ")
            StrSQL.AppendLine("  AND Mov_Destinazioni.Id_Mov_Det =  Movimenti_dettagli.Id_Mov_Det ")

            StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON")
            StrSQL.AppendLine("      Reg_Impianti.Piva  = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine("  AND Reg_Impianti.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine("  AND Reg_Impianti.appezza =  Mov_Destinazioni.appezza ")
            StrSQL.AppendLine("  AND Reg_Impianti.id_Reg =  Mov_Destinazioni.id_Destinazione ")

            StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON")
            StrSQL.AppendLine("      Imprese_Progetti.Piva  = Reg_Impianti.Piva ")
            StrSQL.AppendLine("  AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod ")
            StrSQL.AppendLine("  AND Imprese_Progetti.appezza =  Reg_Impianti.appezza ")
            StrSQL.AppendLine("  AND Imprese_Progetti.id_Reg =  Reg_Impianti.id_Reg ")
            'Esercizi attivi alla data
            StrSQL.AppendLine("  AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
            StrSQL.AppendLine("  AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")

            'Con regolamento BIO
            If joinImpiantixBIO Then
                StrSQL.AppendLine("  AND Imprese_Progetti.Regolamento_Cod = " & enum_Cod_Regolamento.Regolamento_bio & " ")
            End If

            StrSQL.AppendLine(" INNER JOIN Appezzamento ON ")
            StrSQL.AppendLine("     Appezzamento.Piva = Mov_Destinazioni.Piva ")
            StrSQL.AppendLine(" AND Appezzamento.Sa_Cod = Mov_Destinazioni.Sa_Cod ")
            StrSQL.AppendLine(" AND Appezzamento.appezza = Mov_Destinazioni.appezza ")

            StrSQL.AppendLine(" LEFT JOIN Campi ON ")
            StrSQL.AppendLine("     Appezzamento.Piva = Campi.Piva ")
            StrSQL.AppendLine(" AND Appezzamento.Sa_Cod = Campi.Sa_Cod ")
            StrSQL.AppendLine(" AND Appezzamento.campo_cod = Campi.campo_cod ")

            If CaricaNrDomandaACA Then
                'Nr domanda ACA
                StrSQL.AppendLine(" LEFT JOIN (")
                If sql2017 Then
                    StrSQL.AppendLine(" SELECT ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo,")
                    StrSQL.AppendLine("        STRING_AGG(aca.ContributoDes, ', ') Nr_domanda_ACA")
                    StrSQL.AppendLine(" FROM Imprese_ProgettiXContributi ipc")
                    StrSQL.AppendLine(" JOIN  Contributi aca")
                    StrSQL.AppendLine($" ON ipc.ContributoTipo = {CInt(ContributeType.ACA)}")
                    StrSQL.AppendLine(" AND ipc.ContributoTipo = aca.Tipo")
                    StrSQL.AppendLine(" AND ipc.ContributoCod = aca.ContributoCod")
                    StrSQL.AppendLine(" GROUP BY ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo")
                Else
                    StrSQL.AppendLine(" SELECT ipc_outer.Piva, ipc_outer.ProgettoCod, ipc_outer.ContributoTipo, ")
                    StrSQL.AppendLine("        STUFF((SELECT ', ' + aca.ContributoDes FROM Imprese_ProgettiXContributi ipc")
                    StrSQL.AppendLine("        INNER JOIN Contributi aca ON ipc.ContributoCod = aca.ContributoCod AND ipc.ContributoTipo = aca.Tipo ")
                    StrSQL.AppendLine("        WHERE ipc.Piva = ipc_outer.Piva AND ipc.ProgettoCod = ipc_outer.ProgettoCod AND ipc.ContributoTipo = ipc_outer.ContributoTipo ")
                    StrSQL.AppendLine("        FOR XML PATH('')), 1, 2, '') AS Nr_domanda_ACA")
                    StrSQL.AppendLine(" FROM ")
                    StrSQL.AppendLine($" (SELECT DISTINCT Piva, ProgettoCod, ContributoTipo FROM Imprese_ProgettiXContributi WHERE ContributoTipo = {CInt(ContributeType.ACA)}) ipc_outer")
                End If
                StrSQL.AppendLine(" ) CodiciACA")
                StrSQL.AppendLine(" ON Imprese_Progetti.Piva = CodiciACA.Piva")
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = CodiciACA.ProgettoCod")
            End If

            StrSQL.AppendLine(" WHERE Agenda.Lav_Cod IN (" & LAVCOD_INSTALLAZIONE_TRAPPOLE_CATTURE_MASSA & ") ")

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Mov_Destinazioni.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Destinazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Raccoglitore_Cod <> 0 Then
                StrSQL.AppendLine(" AND Agenda.Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            End If

            If Pro_Cod <> 0 Then
                StrSQL.AppendLine(" AND Movimenti_dettagli.Pro_Cod = " & Agro_SQL_SaveNum(Pro_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("" & nomeRoutine & " : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Function Trappole_107122_X_ImpiantiInfluenza(ByVal PIVA As String,
                                                 ByVal Sa_Cod As Integer,
                                                 ByVal Sigla_AV As String,
                                                 ByVal Trap_Num As Integer,
                                                 ByVal xFiltroAggiuntivo As String,
                                                 ByVal xOrderBy As String,
                                                 ByVal objParametri As AgronicaCoreParametri,
                                                    Optional ByVal LeftJoin_RegImpianti_ImpreseProgetti As Boolean = False,
                                                    Optional ByVal EstraiLotto As Boolean = False,
                                                    Optional joinImpiantixBIO As Boolean = False,
                                                    Optional CaricaNrDomandaACA As Boolean = False,
                                                    Optional sql2017 As Boolean = False,
                                                    Optional ByVal Id_Agenda As Integer = 0,
                                                    Optional ByVal Raccoglitore_Cod As Integer = 0
                                                 ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.Trappole_107122_X_ImpiantiInfluenza()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT  ")

            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Piva AS Azienda , ")
            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Sa_Cod AS Centro_Aziendale , ")
            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Trap_Num AS Numero_Trappola , ")
            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Freatimetro AS Numero_Personalizzato , ")
            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Sigla_AV AS Sigla_Avversita , ")
            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Av_Cod AS Codice_Avversita , ")
            StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.Ditta_Cod AS Codice_Ditta , ")

            StrSQL.AppendLine(" Imp_Install.Appezza           AS Imp_Install_Appezza , ")
            StrSQL.AppendLine(" Imp_Install.Id_Destinazione   AS Imp_Install_Id_Destinazione  , ")
            StrSQL.AppendLine(" Imp_Install.Qta               AS Imp_Install_Qta , ")

            StrSQL.AppendLine(" Imp_Influ.Appezza           AS Imp_Influ_Appezza , ")
            StrSQL.AppendLine(" Imp_Influ.Id_Destinazione   AS Imp_Influ_Id_Destinazione  , ")
            StrSQL.AppendLine(" Imp_Influ.Qta               AS Imp_Influ_Qta , ")

            '(19/10/2017) aggiunti per evitare letture dopo
            StrSQL.AppendLine(" App.app_nome , ")
            StrSQL.AppendLine(" isnull(Cam.Campo_Des,'') as campo_des , ")
            StrSQL.AppendLine(" ISNULL((SELECT TOP 1 Appezzamento_Codici.val_cod FROM Appezzamento_Codici  " & vbCrLf)
            StrSQL.AppendLine(" where (Appezzamento_Codici.PIVA = App.PIVA) " & vbCrLf)
            StrSQL.AppendLine(" and (Appezzamento_Codici.sa_cod = App.sa_cod) " & vbCrLf)
            StrSQL.AppendLine(" and (Appezzamento_Codici.appezza = App.appezza) " & vbCrLf)
            StrSQL.AppendLine(" and (Appezzamento_Codici.id_cod = 1104) ), '') AS App_Nome_Breve " & vbCrLf)

            If EstraiLotto Then
                StrSQL.AppendLine(", ISNULL(imp_p.Progetto_Nome,'') AS Lotto " & vbCrLf)
            End If

            StrSQL.AppendLine(" , Agenda.Id_Agenda ")
            StrSQL.AppendLine(" , ISNULL(Agenda.Raccoglitore_Cod, 0) AS Raccoglitore_Cod ")

            If CaricaNrDomandaACA Then
                StrSQL.AppendLine(", ISNULL(CodiciACA.Nr_domanda_ACA, '') AS Nr_domanda_ACA ")
            End If
            '------------Superflui-----------
            'StrSQL.AppendLine(" Agenda.* , ")
            'StrSQL.AppendLine(" Movimenti.* , ")
            'StrSQL.AppendLine(" Movimenti_dettagli.* , ")
            'StrSQL.AppendLine(" Mov_Dettaglio_Tecnico.*  ")
            '-----------------------

            StrSQL.AppendLine(" FROM  Agenda  ")

            StrSQL.AppendLine("     INNER JOIN [Movimenti] ")
            StrSQL.AppendLine("                 ON    Agenda.Piva  = Movimenti.Piva ")
            StrSQL.AppendLine("                 AND   Agenda.Sa_Cod = Movimenti.Sa_Cod ")
            StrSQL.AppendLine("                 AND   Agenda.Id_Agenda =  Movimenti.Id_Agenda ")

            StrSQL.AppendLine("     INNER JOIN [Movimenti_dettagli] ")
            StrSQL.AppendLine("                 ON    Movimenti.Piva  = Movimenti_dettagli.Piva ")
            StrSQL.AppendLine("                 AND   Movimenti.Sa_Cod = Movimenti_dettagli.Sa_Cod ")
            StrSQL.AppendLine("                 AND   Movimenti.Id_Agenda =  Movimenti_dettagli.Id_Agenda ")
            StrSQL.AppendLine("                 AND   Movimenti.Id_Mov =  Movimenti_dettagli.Id_Mov ")


            StrSQL.AppendLine("     INNER JOIN [Mov_Dettaglio_Tecnico] ")
            StrSQL.AppendLine("                 ON    Movimenti_dettagli.Piva  = Mov_Dettaglio_Tecnico.Piva ")
            StrSQL.AppendLine("                 AND   Movimenti_dettagli.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")
            StrSQL.AppendLine("                 AND   Movimenti_dettagli.Id_Agenda =  Mov_Dettaglio_Tecnico.Id_Agenda ")
            StrSQL.AppendLine("                 AND   Movimenti_dettagli.Id_Mov =  Mov_Dettaglio_Tecnico.Id_Mov ")
            StrSQL.AppendLine("                 AND   Movimenti_dettagli.Id_Mov_Det =  Mov_Dettaglio_Tecnico.Id_Mov_Det ")

            '--------------------------------
            '----------Impianto di installazione----------------------
            'join con destinazioni con stessa id_agenda e stesso id_movimento
            'SOPRATTUTTO join con lo stesso movimento dettaglio, in modo da avere solo impianto di installazione
            StrSQL.AppendLine("     INNER JOIN [Mov_Destinazioni] AS Imp_Install ")
            StrSQL.AppendLine("              ON   Mov_Dettaglio_Tecnico.Piva = Imp_Install.Piva  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Sa_Cod = Imp_Install.Sa_Cod  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Id_Agenda = Imp_Install.Id_Agenda  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Id_Mov = Imp_Install.Id_Mov  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Id_Mov_Det = Imp_Install.Id_Mov_Det  ")
            '--------------------------------
            '--------------------------------


            '--------------------------------
            '----------Impianti influenzati----------------------
            'join con destinazioni con stessa id_agenda e stesso id_movimento
            'NON join con lo stesso movimento dettaglio, tirerebbe su solo impianto di installazione
            StrSQL.AppendLine("     INNER JOIN [Mov_Destinazioni] AS Imp_Influ ")
            StrSQL.AppendLine("              ON   Mov_Dettaglio_Tecnico.Piva = Imp_Influ.Piva  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Sa_Cod = Imp_Influ.Sa_Cod  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Id_Agenda = Imp_Influ.Id_Agenda  ")
            StrSQL.AppendLine("              AND  Mov_Dettaglio_Tecnico.Id_Mov = Imp_Influ.Id_Mov  ")
            '--------------------------------
            '--------------------------------

            '(19/10/2017) aggiunti per evitare letture dopo
            StrSQL.AppendLine("     INNER JOIN [Appezzamento] AS App ")
            StrSQL.AppendLine("              ON   App.Piva = Imp_Influ.Piva  ")
            StrSQL.AppendLine("              AND  App.Sa_Cod = Imp_Influ.Sa_Cod  ")
            StrSQL.AppendLine("              AND  App.appezza = Imp_Influ.appezza  ")
            StrSQL.AppendLine("     LEFT JOIN [Campi] AS Cam ")
            StrSQL.AppendLine("              ON   App.Piva = Cam.Piva  ")
            StrSQL.AppendLine("              AND  App.Sa_Cod = Cam.Sa_Cod  ")
            StrSQL.AppendLine("              AND  App.campo_cod = Cam.campo_cod  ")

            If LeftJoin_RegImpianti_ImpreseProgetti Then
                StrSQL.AppendLine("     LEFT JOIN [Reg_Impianti] AS reg_imp ")
                StrSQL.AppendLine("              ON   App.Piva = reg_imp.Piva  ")
                StrSQL.AppendLine("              AND  App.Sa_Cod = reg_imp.Sa_Cod  ")
                StrSQL.AppendLine("              AND  App.appezza = reg_imp.appezza  ")
                StrSQL.AppendLine("     LEFT JOIN [Imprese_Progetti] AS imp_p ")
                StrSQL.AppendLine("              ON   reg_imp.Piva = imp_p.Piva  ")
                StrSQL.AppendLine("              AND  reg_imp.Sa_Cod = imp_p.Sa_Cod  ")
                StrSQL.AppendLine("              AND  reg_imp.appezza = imp_p.appezza  ")
                StrSQL.AppendLine("              AND  reg_imp.id_reg = imp_p.id_reg  ")
            End If

            If joinImpiantixBIO Then
                StrSQL.AppendLine(" INNER JOIN Reg_Impianti ON")
                StrSQL.AppendLine("      Reg_Impianti.Piva  = Imp_Influ.Piva ")
                StrSQL.AppendLine("  AND Reg_Impianti.Sa_Cod = Imp_Influ.Sa_Cod ")
                StrSQL.AppendLine("  AND Reg_Impianti.appezza =  Imp_Influ.appezza ")
                StrSQL.AppendLine("  AND Reg_Impianti.id_Reg =  Imp_Influ.id_Destinazione ")

                StrSQL.AppendLine(" INNER JOIN Imprese_Progetti ON")
                StrSQL.AppendLine("      Imprese_Progetti.Piva  = Reg_Impianti.Piva ")
                StrSQL.AppendLine("  AND Imprese_Progetti.Sa_Cod = Reg_Impianti.Sa_Cod ")
                StrSQL.AppendLine("  AND Imprese_Progetti.appezza =  Reg_Impianti.appezza ")
                StrSQL.AppendLine("  AND Imprese_Progetti.id_Reg =  Reg_Impianti.id_Reg ")
                'Esercizi attivi alla data
                StrSQL.AppendLine("  AND Movimenti.Data_Movimento >= Imprese_Progetti.Validita_Inizio ")
                StrSQL.AppendLine("  AND Movimenti.Data_Movimento <= Imprese_Progetti.Validita_Fine ")
                'Con regolamento BIO
                StrSQL.AppendLine("  AND Imprese_Progetti.Regolamento_Cod = " & enum_Cod_Regolamento.Regolamento_bio & " ")
            End If

            If CaricaNrDomandaACA Then
                'Nr domanda ACA
                StrSQL.AppendLine(" LEFT JOIN (")
                If sql2017 Then
                    StrSQL.AppendLine(" SELECT ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo,")
                    StrSQL.AppendLine("        STRING_AGG(aca.ContributoDes, ', ') Nr_domanda_ACA")
                    StrSQL.AppendLine(" FROM Imprese_ProgettiXContributi ipc")
                    StrSQL.AppendLine(" JOIN  Contributi aca")
                    StrSQL.AppendLine($" ON ipc.ContributoTipo = {CInt(ContributeType.ACA)}")
                    StrSQL.AppendLine(" AND ipc.ContributoTipo = aca.Tipo")
                    StrSQL.AppendLine(" AND ipc.ContributoCod = aca.ContributoCod")
                    StrSQL.AppendLine(" GROUP BY ipc.Piva, ipc.ProgettoCod, ipc.ContributoTipo")
                Else
                    StrSQL.AppendLine(" SELECT ipc_outer.Piva, ipc_outer.ProgettoCod, ipc_outer.ContributoTipo, ")
                    StrSQL.AppendLine("        STUFF((SELECT ', ' + aca.ContributoDes FROM Imprese_ProgettiXContributi ipc")
                    StrSQL.AppendLine("        INNER JOIN Contributi aca ON ipc.ContributoCod = aca.ContributoCod AND ipc.ContributoTipo = aca.Tipo ")
                    StrSQL.AppendLine("        WHERE ipc.Piva = ipc_outer.Piva AND ipc.ProgettoCod = ipc_outer.ProgettoCod AND ipc.ContributoTipo = ipc_outer.ContributoTipo ")
                    StrSQL.AppendLine("        FOR XML PATH('')), 1, 2, '') AS Nr_domanda_ACA")
                    StrSQL.AppendLine(" FROM ")
                    StrSQL.AppendLine($" (SELECT DISTINCT Piva, ProgettoCod, ContributoTipo FROM Imprese_ProgettiXContributi WHERE ContributoTipo = {CInt(ContributeType.ACA)}) ipc_outer")
                End If
                StrSQL.AppendLine(" ) CodiciACA")
                StrSQL.AppendLine(" ON Imprese_Progetti.Piva = CodiciACA.Piva")
                StrSQL.AppendLine(" AND Imprese_Progetti.Progetto_Cod = CodiciACA.ProgettoCod")
            End If


            StrSQL.AppendLine($" WHERE   Agenda.Lav_Cod IN ({CInt(LAVCOD_INSTALLAZIONE_TRAPPOLE)}, {CInt(LAVCOD_CATTURE_MASSA)}) ")

            'Filtro  sull'azienda, centro e trappola

            If PIVA <> "" Then
                StrSQL.AppendLine(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.AppendLine(" AND Mov_Dettaglio_Tecnico.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Trim(Sigla_AV) <> "" Then
                StrSQL.AppendLine(" AND  Mov_Dettaglio_Tecnico.Sigla_AV = '" & Agro_SQL_SaveText(Sigla_AV) & "'   ")
            End If

            If Trim(Trap_Num) <> 0 Then
                StrSQL.AppendLine(" AND  Mov_Dettaglio_Tecnico.Trap_Num = " & Agro_SQL_SaveNum(Trap_Num) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND Agenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")
            End If

            If Raccoglitore_Cod <> 0 Then
                StrSQL.AppendLine(" AND Agenda.Raccoglitore_Cod = " & Agro_SQL_SaveNum(Raccoglitore_Cod) & " ")
            End If

            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------

            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Azienda,Centro_Aziendale, Numero_Trappola, Imp_Install_Appezza, Imp_Influ_Appezza ")
            End If
            '--------------------------------------------------------------------------

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


    Function DataInstallazioneTrappola(ByVal PIVA As String,
                                       ByVal Sa_Cod As Integer,
                                       ByVal Sigla_AV As String,
                                       ByVal Trap_Num As Integer,
                                       ByVal objParametri As AgronicaCoreParametri
                                       ) As Date

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.DataInstallazioneTrappola()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Agenda.Validita_Inizio ")
            StrSQL.Append(" FROM  Agenda,  Mov_Dettaglio_Tecnico ")
            'Join sulla Piva
            StrSQL.Append(" WHERE   Agenda.Piva  = Mov_Dettaglio_Tecnico.Piva ")

            'Join sul Sa_Cod
            StrSQL.Append(" AND   Agenda.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod ")

            'Join sul Id_Agenda
            StrSQL.Append(" AND   Agenda.Id_Agenda =  Mov_Dettaglio_Tecnico.Id_Agenda ")

            'scelgo installazione trappole, catture massa
            'Public Const LAVCOD_INSTALLAZIONE_TRAPPOLE As Integer = 107
            'Public Const LAVCOD_CONFUSIONE_SESSUALE As Integer = 118
            'Public Const LAVCOD_DISORIENTAMENTO_SESSUALE As Integer = 121
            'Public Const LAVCOD_CATTURE_MASSA As Integer = 122
            StrSQL.Append(" AND   Agenda.Lav_Cod IN (107, 122, 118 ,121) ")

            If PIVA <> "" Then
                StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Piva = '" & Agro_SQL_SaveText(PIVA) & "'   ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Mov_Dettaglio_Tecnico.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Trim(Sigla_AV) <> "" Then
                StrSQL.Append(" AND  Mov_Dettaglio_Tecnico.Sigla_AV = '" & Agro_SQL_SaveText(Sigla_AV) & "'   ")
            End If

            If Trim(Trap_Num) <> 0 Then
                StrSQL.Append(" AND  Mov_Dettaglio_Tecnico.Trap_Num = " & Agro_SQL_SaveNum(Trap_Num) & "   ")
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

        If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Validita_Inizio")
        End If

        'errore
        Return AGRODATAINIZIO

    End Function


    Public Function controllaGiacenza_Trappole_Inneschi(ByVal Piva As String,
                                                        ByVal Sa_Cod As Integer,
                                                        ByVal Id_Destinazione As Integer,
                                                        ByVal Id_Agenda As Integer,
                                                        ByVal Trap_Cod As Integer,
                                                        ByVal Av_Cod As Integer,
                                                        ByVal NumTrappoleDaControllare As Integer,
                                                        ByVal NumInneschiDaControllare As Integer,
                                                        ByRef GiacenzeTrappoleAllaData As Integer,
                                                        ByRef GiacenzeInneschiAllaData As Integer,
                                                        ByRef GiacenzeTrappoleAttuale As Integer,
                                                        ByRef GiacenzeInneschiAttuale As Integer,
                                                        ByRef DataOperazione As Date,
                                                        ByRef Messaggio As String,
                                                        ByRef objParametri As AgronicaCoreParametri
                                                        ) As Boolean

        Messaggio = ""

        GiacenzeTrappoleAllaData = 0
        GiacenzeInneschiAllaData = 0
        GiacenzeTrappoleAttuale = 0
        GiacenzeInneschiAttuale = 0

        If NumTrappoleDaControllare > 0 Then

            'guardo se la quantità è conforme per la data di intervento
            LeggiGiacenze_Trappole(Piva, Sa_Cod, Id_Destinazione, Id_Agenda, Trap_Cod, CDate(DataOperazione), GiacenzeTrappoleAllaData, objParametri)
            If NumTrappoleDaControllare > GiacenzeTrappoleAllaData Then
                Messaggio = Messaggio & "ATTENZIONE!<br>La quantità di trappole <b>" &
                                    "</b> al <b>" & DataOperazione & "</b> è pari a <b>" & GiacenzeTrappoleAllaData & "</b> " & " quindi non sufficiente per lo scarico!<br><br> " & vbCr
            End If

            'guardo se la quantità è conforme per la data attuale
            LeggiGiacenze_Trappole(Piva, Sa_Cod, Id_Destinazione, Id_Agenda, Trap_Cod, Date.Today, GiacenzeTrappoleAttuale, objParametri)
            If NumTrappoleDaControllare > GiacenzeTrappoleAttuale Then
                Messaggio = Messaggio & "ATTENZIONE!<br>La Giacenza attuale di trappole <b>" &
                                                        "</b> (" & GiacenzeTrappoleAttuale & " unità) non è sufficiente per lo scarico!<br><br> "
            End If

        End If

        If NumInneschiDaControllare > 0 Then

            'guardo se la quantità è conforme per la data di intervento
            LeggiGiacenze_Inneschi(Piva, Sa_Cod, Id_Destinazione, Id_Agenda, Av_Cod, CDate(DataOperazione), GiacenzeInneschiAllaData, objParametri)
            If NumInneschiDaControllare > GiacenzeInneschiAllaData Then
                Messaggio = Messaggio & "ATTENZIONE!<br>La quantità di inneschi <b>" &
                                    "</b> al <b>" & DataOperazione & "</b> è pari a <b>" & GiacenzeInneschiAllaData & "</b> " & " quindi non sufficiente per lo scarico!<br><br> " & vbCr
            End If

            'guardo se la quantità è conforme per la data attuale
            LeggiGiacenze_Inneschi(Piva, Sa_Cod, Id_Destinazione, Id_Agenda, Av_Cod, Date.Today, GiacenzeInneschiAttuale, objParametri)
            If NumInneschiDaControllare > GiacenzeInneschiAttuale Then
                Messaggio = Messaggio & "ATTENZIONE!<br>La Giacenza attuale di inneschi <b>" &
                                                        "</b> (" & GiacenzeInneschiAttuale & " unità) non è sufficiente per lo scarico!<br><br> "
            End If

        End If

        If Messaggio <> "" Then
            Return False
        End If

        Return True

    End Function


    Public Sub LeggiGiacenze_Trappole(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Id_Destinazione As Integer,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Trap_Cod As Integer,
                                      ByVal DataControllo As Date,
                                      ByRef GiacenzaTrappole As Integer,
                                      ByRef objParametri As AgronicaCoreParametri)


        Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        'Leggo le giacenze delle TRAPPOLE
        If Trap_Cod <> 0 Then

            GiacenzaTrappole = objGiacenze.Verifica_Giacenze(Piva,
                                                             CInt(Sa_Cod),
                                                             CInt(Id_Destinazione),
                                                             TRAPPOLE,
                                                             CInt(Trap_Cod),
                                                             0,
                                                             0, 0,
                                                             LOTTO_NONDEFINITO,
                                                             0, 0,
                                                             AGRODATAINIZIO,
                                                             DataControllo,
                                                             Id_Agenda,
                                                             objParametri)

        End If

    End Sub

    Public Sub LeggiGiacenze_Inneschi(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Id_Destinazione As Integer,
                                      ByVal Id_Agenda As Integer,
                                      ByVal Av_Cod As Integer,
                                      ByVal DataControllo As Date,
                                      ByRef GiacenzaInneschi As Integer,
                                      ByRef objParametri As AgronicaCoreParametri)

        Dim objGiacenze As New AgronicaCoreContabDAL.Movimenti_Dettagli_R

        'Leggo le giacenze degli INNESCHI
        If Av_Cod <> 0 Then

            GiacenzaInneschi = objGiacenze.Verifica_Giacenze(Piva,
                                                             CInt(Sa_Cod),
                                                             CInt(Id_Destinazione),
                                                             INNESCHI,
                                                             CInt(Av_Cod),
                                                             0,
                                                             0, 0,
                                                             LOTTO_NONDEFINITO,
                                                             0, 0,
                                                             AGRODATAINIZIO,
                                                             DataControllo,
                                                             Id_Agenda,
                                                             objParametri)

        End If

    End Sub

#End Region


#Region "Piogge"

    Function LeggiPiogge(ByVal PIVA As String,
                         ByVal Sa_Cod As Integer,
                         ByVal ID_Agenda As Integer,
                         ByRef Data_Inizio As Date,
                         ByVal Data_Fine As Date,
                         ByVal xFiltroAggiuntivo As String,
                         ByRef objParametri As AgronicaCoreParametri
                         ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_R.LeggiPiogge()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0
            StrSQL.Append(" SELECT ")

            StrSQL.Append(" Imprese.rag_soc,  ")
            StrSQL.Append(" Centri_Aziendali.Sa_Nome,  ")

            StrSQL.Append(" Agenda.Id_Agenda,  ")
            StrSQL.Append(" Agenda.Lav_Cod, ")
            StrSQL.Append(" Agenda.PIVA, ")
            StrSQL.Append(" Agenda.Sa_Cod, ")
            StrSQL.Append(" Agenda.validita_inizio, ")

            StrSQL.Append(" Movimenti.Ora,   ")
            StrSQL.Append(" Movimenti.Mov_Desc, ")
            StrSQL.Append(" Mov_Dettaglio_Tecnico.Qta_Ril,  ")
            StrSQL.Append(" Mov_Dettaglio_Tecnico.Piezo1 ,   ")
            StrSQL.Append(" Mov_Dettaglio_Tecnico.Piezo2 , ")
            StrSQL.Append(" Mov_Dettaglio_Tecnico.Piezo3 , ")
            StrSQL.Append(" Mov_Dettaglio_Tecnico.Piezo4  ")

            StrSQL.Append(" FROM Agenda ")

            StrSQL.Append(" INNER JOIN ")
            StrSQL.Append(" Imprese ON ")
            StrSQL.Append(" Agenda.PIVA = Imprese.PIVA ")

            StrSQL.Append(" INNER JOIN ")
            StrSQL.Append(" Centri_Aziendali ON ")
            StrSQL.Append(" Centri_Aziendali.PIVA = Agenda.PIVA  ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" Centri_Aziendali.Sa_Cod=Agenda.Sa_Cod ")

            StrSQL.Append(" INNER JOIN ")
            StrSQL.Append(" Movimenti ON ")
            StrSQL.Append(" Agenda.PIVA = Movimenti.PIVA   ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" Agenda.Sa_Cod = Movimenti.Sa_Cod  ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" Agenda.Id_Agenda = Movimenti.Id_Agenda ")


            StrSQL.Append(" INNER JOIN ")
            StrSQL.Append(" Mov_Dettaglio_Tecnico ON ")
            StrSQL.Append(" Movimenti.PIVA = Mov_Dettaglio_Tecnico.Piva  ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" Movimenti.Sa_Cod = Mov_Dettaglio_Tecnico.Sa_Cod   ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" Movimenti.Id_Agenda = Mov_Dettaglio_Tecnico.Id_Agenda  ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" Movimenti.Id_Mov = Mov_Dettaglio_Tecnico.Id_Mov")

            StrSQL.Append(" WHERE ")
            StrSQL.Append(" (Agenda.Lav_Cod = " & Agro_SQL_SaveNum(LAVCOD_RILIEVO_PIOGGE) & ")  ")
            StrSQL.Append(" AND ")
            StrSQL.Append(" (Movimenti.Cau_Mov = '" & Agro_SQL_SaveText(CStr(enum_Agenda_Causali.RILIEVO_CAMPO)) & "') ")


            If PIVA <> "" Then
                StrSQL.Append(" AND Agenda.PIVA = '" & Agro_SQL_SaveText(PIVA) & "'  ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Agenda.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If ID_Agenda <> 0 Then
                StrSQL.Append(" AND Agenda.ID_Agenda = " & Agro_SQL_SaveNum(ID_Agenda) & "   ")
            End If

            If Not IsNothing(Data_Inizio) AndAlso IsDate(Data_Inizio) Then
                StrSQL.Append(" AND (Agenda.Validita_Inizio >= " & Agro_SQL_SaveDate(Data_Inizio) & " )  ")
            End If

            If Not IsNothing(Data_Fine) AndAlso IsDate(Data_Fine) Then
                StrSQL.Append(" AND (Agenda.Validita_Inizio <= " & Agro_SQL_SaveDate(Data_Fine) & "  ) ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            ''If xOrderBy <> "" Then
            ''    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            ''Else
            StrSQL.Append(" ORDER BY Agenda.PIVA Asc, Agenda.Sa_Cod Asc, Agenda.Validita_Inizio  desc")
            'End If

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

#End Region

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Mov_Dettaglio_Tecnico_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Id_Agenda As Integer,
                           ByVal Id_Mov As Integer,
                           ByVal Id_Mov_Det As Integer,
                           ByVal Id_Reg_Dettaglio As Integer,
                           ByVal Av_Cod As Integer,
                           ByVal Av_Gru As Integer,
                           ByVal Sigla_AV As String,
                           ByVal Data_Ril As Date,
                           ByVal Qta_Ril As Decimal,
                           ByVal Dose As Decimal,
                           ByVal Ditta_Cod As Integer,
                           ByVal Dett_Cod As Integer,
                           ByVal Id_Insetto As Integer,
                           ByVal FF_Classe As Integer,
                           ByVal Mg As Decimal,
                           ByVal N As Decimal,
                           ByVal K As Decimal,
                           ByVal P As Decimal,
                           ByVal Parziale As Integer,
                           ByVal Nitrati As Integer,
                           ByVal Freatimetro As Decimal,
                           ByVal Piezo1 As Decimal,
                           ByVal Piezo2 As Decimal,
                           ByVal Piezo3 As Decimal,
                           ByVal Piezo4 As Decimal,
                           ByVal Trap_Num As Integer,
                           ByVal Inn1_Data As Date,
                           ByVal Inn2_Data As Date,
                           ByVal Inn3_Data As Date,
                           ByVal Inn4_Data As Date,
                           ByVal Lotto As String,
                           ByVal Extra_Int As Integer,
                           ByVal Extra_Str As String,
                           ByVal Extra_Date As Date,
                           ByVal Soglia_Cod As Integer,
                           ByVal Soglia_Quantita As Decimal,
                           ByVal Soglia_Des As String,
                           ByVal Efficienza As Decimal,
                           ByVal Cu As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W.Scrivi()"
        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
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

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO Mov_Dettaglio_Tecnico ")
            strSql.Append("         ( Piva, Sa_Cod, Id_Agenda, Id_Mov, ")
            strSql.Append("         Id_Mov_Det, Id_Reg_Dettaglio, ")

            strSql.Append("         Av_Cod, Av_Gru, Sigla_AV, Data_Ril, Qta_Ril, Dose, ")
            strSql.Append("         Ditta_Cod, Dett_Cod, Id_Insetto, FF_Classe, Mg,  ")
            strSql.Append("         N, K, P, Parziale, Nitrati, Freatimetro, Piezo1,   ")
            strSql.Append("         Piezo2, Piezo3, Piezo4, Trap_Num, ")
            strSql.Append("         Inn1_Data, Inn2_Data, Inn3_Data, Inn4_Data, ")
            strSql.Append("         Lotto, Extra_Int, Extra_Str, Extra_Date, ")
            strSql.Append("         Soglia_Cod, Soglia_Quantita, Soglia_Des, Efficienza, Cu, ")

            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("          ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Av_Gru) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("         , " & If(Data_Ril = New Date, "Null", Agro_SQL_SaveDate(Data_Ril)) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(FF_Classe) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Mg) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(N) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(K) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(P) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Nitrati) & "  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Freatimetro) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo1) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo2) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo3) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Piezo4) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("         , " & If(Inn1_Data = New Date, "Null", Agro_SQL_SaveDate(Inn1_Data)) & "  ")
            strSql.Append("         , " & If(Inn2_Data = New Date, "Null", Agro_SQL_SaveDate(Inn2_Data)) & "  ")
            strSql.Append("         , " & If(Inn3_Data = New Date, "Null", Agro_SQL_SaveDate(Inn3_Data)) & "  ")
            strSql.Append("         , " & If(Inn4_Data = New Date, "Null", Agro_SQL_SaveDate(Inn4_Data)) & "  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Lotto) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.Append("         , " & If(Extra_Date = New Date, "Null", Agro_SQL_SaveDate(Extra_Date)) & "  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Soglia_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Soglia_Quantita) & "  ")
            strSql.Append("         , '" & Agro_SQL_SaveText(Soglia_Des) & "'  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Efficienza) & "  ")

            strSql.Append("         , " & Agro_SQL_SaveNum(Cu) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")

            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append(") ")

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

    '''<see cref="Mov_Dettaglio_Tecnico_W.ModificaPuntuale"/>
    <Obsolete("Usare la funzione Mov_Dettaglio_Tecnico_W.ModificaPuntuale")>
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Id_Reg_Dettaglio As Integer,
                             ByVal Av_Cod As Integer,
                             ByVal Av_Gru As Integer,
                             ByVal Sigla_AV As String,
                             ByVal Data_Ril As Date,
                             ByVal Qta_Ril As Decimal,
                             ByVal Dose As Decimal,
                             ByVal Ditta_Cod As Integer,
                             ByVal Dett_Cod As Integer,
                             ByVal Id_Insetto As Integer,
                             ByVal FF_Classe As Integer,
                             ByVal Mg As Decimal,
                             ByVal N As Decimal,
                             ByVal K As Decimal,
                             ByVal P As Decimal,
                             ByVal Parziale As Integer,
                             ByVal Nitrati As Integer,
                             ByVal Freatimetro As Decimal,
                             ByVal Piezo1 As Decimal,
                             ByVal Piezo2 As Decimal,
                             ByVal Piezo3 As Decimal,
                             ByVal Piezo4 As Decimal,
                             ByVal Trap_Num As Integer,
                             ByVal Inn1_Data As Date,
                             ByVal Inn2_Data As Date,
                             ByVal Inn3_Data As Date,
                             ByVal Inn4_Data As Date,
                             ByVal Lotto As String,
                             ByVal Extra_Int As Integer,
                             ByVal Extra_Str As String,
                             ByVal Extra_Date As Date,
                             ByVal Soglia_Cod As Integer,
                             ByVal Soglia_Quantita As Decimal,
                             ByVal Soglia_Des As String,
                             ByVal Efficienza As Decimal,
                             ByVal Cu As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W.Modifica()"
        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

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

            If Id_Mov = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            End If

            If Id_Mov_Det = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Mov_Det obbligatorio)")
            End If

            If Id_Reg_Dettaglio = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Reg_Dettaglio obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" UPDATE Mov_Dettaglio_Tecnico SET ")
            strSql.Append("      Qta_Ril        = " & Agro_SQL_SaveNum(Qta_Ril) & "  ")
            strSql.Append("     ,Data_Ril       =  " & Agro_SQL_SaveDate(Data_Ril) & "  ")
            strSql.Append("     ,Ditta_Cod      = " & Agro_SQL_SaveNum(Ditta_Cod) & "  ")
            strSql.Append("     ,Dett_Cod       = " & Agro_SQL_SaveNum(Dett_Cod) & "  ")
            strSql.Append("     ,Id_Insetto     = " & Agro_SQL_SaveNum(Id_Insetto) & "  ")
            strSql.Append("     ,FF_Classe      = " & Agro_SQL_SaveNum(FF_Classe) & "  ")
            strSql.Append("     ,Dose           = " & Agro_SQL_SaveNum(Dose) & "  ")
            strSql.Append("     ,Mg             = " & Agro_SQL_SaveNum(Mg) & "  ")
            strSql.Append("     ,N              = " & Agro_SQL_SaveNum(N) & "  ")
            strSql.Append("     ,K              = " & Agro_SQL_SaveNum(K) & "  ")
            strSql.Append("     ,P              = " & Agro_SQL_SaveNum(P) & "  ")
            strSql.Append("     ,Parziale       = " & Agro_SQL_SaveNum(Parziale) & "  ")
            strSql.Append("     ,Nitrati        = " & Agro_SQL_SaveNum(Nitrati) & "  ")
            strSql.Append("     ,Freatimetro    = " & Agro_SQL_SaveNum(Freatimetro) & "  ")
            strSql.Append("     ,Piezo1         = " & Agro_SQL_SaveNum(Piezo1) & "  ")
            strSql.Append("     ,Piezo2         = " & Agro_SQL_SaveNum(Piezo2) & "  ")
            strSql.Append("     ,Piezo3         = " & Agro_SQL_SaveNum(Piezo3) & "  ")
            strSql.Append("     ,Piezo4         = " & Agro_SQL_SaveNum(Piezo4) & "  ")
            strSql.Append("     ,Sigla_Av       = '" & Agro_SQL_SaveText(Sigla_AV) & "'  ")
            strSql.Append("     ,Trap_Num       = " & Agro_SQL_SaveNum(Trap_Num) & "  ")
            strSql.Append("     ,Lotto          = '" & Agro_SQL_SaveText(Lotto) & "'  ")
            strSql.Append("     ,Extra_Int      = " & Agro_SQL_SaveNum(Extra_Int) & "  ")
            strSql.Append("     ,Extra_Str      = '" & Agro_SQL_SaveText(Extra_Str) & "'  ")
            strSql.Append("     ,Extra_Date     =  " & Agro_SQL_SaveDate(Extra_Date))
            strSql.Append("     ,Inn1_Data      =  " & Agro_SQL_SaveDate(Inn1_Data))
            strSql.Append("     ,Inn2_Data      =  " & Agro_SQL_SaveDate(Inn2_Data))
            strSql.Append("     ,Inn3_Data      =  " & Agro_SQL_SaveDate(Inn3_Data))
            strSql.Append("     ,Inn4_Data      =  " & Agro_SQL_SaveDate(Inn4_Data))
            strSql.Append("     ,Av_Cod         = " & Agro_SQL_SaveNum(Av_Cod) & "  ")
            strSql.Append("     ,Av_Gru         = " & Agro_SQL_SaveNum(Av_Gru) & "  ")

            strSql.Append("     ,Soglia_Cod         = " & Agro_SQL_SaveNum(Soglia_Cod) & "  ")
            strSql.Append("     ,Soglia_Quantita    = " & Agro_SQL_SaveNum(Soglia_Quantita) & "  ")
            strSql.Append("     ,Soglia_Des         = '" & Agro_SQL_SaveText(Soglia_Des) & "'  ")
            strSql.Append("     ,Efficienza         = " & Agro_SQL_SaveNum(Efficienza) & "  ")
            strSql.Append("     ,Cu                 = " & Agro_SQL_SaveNum(Cu) & "  ")

            strSql.Append("     ,Inviato           =  0 ")
            strSql.Append("     ,DataInvio         =  Null ")
            strSql.Append("     ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.Append("     ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("     ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("     ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append(" AND   Sa_Cod            =  " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            strSql.Append(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            strSql.Append(" AND   Id_Mov            =  " & Agro_SQL_SaveNum(Id_Mov) & "  ")
            strSql.Append(" AND   Id_Mov_Det        =  " & Agro_SQL_SaveNum(Id_Mov_Det) & "  ")
            strSql.Append(" AND   Id_Reg_Dettaglio  =  " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "  ")

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
                                     ByVal Id_Mov As Integer,
                                     ByVal Id_Mov_Det As Integer,
                                     ByVal Id_Reg_Dettaglio As Integer,
                                     ByRef objParametri As AgronicaCoreParametri,
                                     Optional ByVal Av_Cod As Integer? = Nothing,
                                     Optional ByVal Av_Gru As Integer? = Nothing,
                                     Optional ByVal Sigla_AV As String = Nothing,
                                     Optional ByVal Data_Ril As Date? = Nothing,
                                     Optional ByVal Qta_Ril As Decimal? = Nothing,
                                     Optional ByVal Dose As Decimal? = Nothing,
                                     Optional ByVal Ditta_Cod As Integer? = Nothing,
                                     Optional ByVal Dett_Cod As Integer? = Nothing,
                                     Optional ByVal Id_Insetto As Integer? = Nothing,
                                     Optional ByVal FF_Classe As Integer? = Nothing,
                                     Optional ByVal Mg As Decimal? = Nothing,
                                     Optional ByVal N As Decimal? = Nothing,
                                     Optional ByVal K As Decimal? = Nothing,
                                     Optional ByVal P As Decimal? = Nothing,
                                     Optional ByVal Parziale As Integer? = Nothing,
                                     Optional ByVal Nitrati As Integer? = Nothing,
                                     Optional ByVal Freatimetro As Decimal? = Nothing,
                                     Optional ByVal Piezo1 As Decimal? = Nothing,
                                     Optional ByVal Piezo2 As Decimal? = Nothing,
                                     Optional ByVal Piezo3 As Decimal? = Nothing,
                                     Optional ByVal Piezo4 As Decimal? = Nothing,
                                     Optional ByVal Trap_Num As Integer? = Nothing,
                                     Optional ByVal Inn1_Data As Date? = Nothing,
                                     Optional ByVal Inn2_Data As Date? = Nothing,
                                     Optional ByVal Inn3_Data As Date? = Nothing,
                                     Optional ByVal Inn4_Data As Date? = Nothing,
                                     Optional ByVal Lotto As String = Nothing,
                                     Optional ByVal Extra_Int As Integer? = Nothing,
                                     Optional ByVal Extra_Str As String = Nothing,
                                     Optional ByVal Extra_Date As Date? = Nothing,
                                     Optional ByVal Soglia_Cod As Integer? = Nothing,
                                     Optional ByVal Soglia_Quantita As Decimal? = Nothing,
                                     Optional ByVal Soglia_Des As String = Nothing,
                                     Optional ByVal Efficienza As Decimal? = Nothing,
                                     Optional ByVal Cu As Decimal? = Nothing,
                                     Optional ByVal Validita_Inizio As Date? = Nothing,
                                     Optional ByVal Validita_Fine As Date? = Nothing,
                                     Optional ByVal xFiltroAggiuntivo As String = "",
                                     Optional ByVal Data_Modifica As DateTime = #2/1/1900#,
                                     Optional ByVal Username_Modifica As String = ""
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W.ModificaPuntuale()"

        '====================================================================================
        'Parametri opzionali :
        '   Tutti i valori non chiave (se impostati a nothing o non passati 
        '   non ne verrà fatto l'aggiornamento e rimarranno i valori precedenti)
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

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

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            'If Id_Mov = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Id_Mov obbligatorio)")
            'End If

            '---------------------------------------------
            strSql.Length = 0

            strSql.AppendLine(" UPDATE Mov_Dettaglio_Tecnico ")
            strSql.AppendLine(" SET Data_Modifica = " & Agro_SQL_SaveDateTime(Data_Modifica) & " ")
            strSql.AppendLine("   , Username_Modifica = '" & Agro_SQL_SaveText(Username_Modifica) & "' ")

            If Not IsNothing(Av_Cod) Then
                strSql.AppendLine("   , Av_Cod = " & Agro_SQL_SaveNum(Av_Cod) & " ")
            End If

            If Not IsNothing(Av_Gru) Then
                strSql.AppendLine("   , Av_Gru = " & Agro_SQL_SaveNum(Av_Gru) & " ")
            End If

            If Not IsNothing(Sigla_AV) Then
                strSql.AppendLine("   , Sigla_AV = '" & Agro_SQL_SaveText(Sigla_AV) & "' ")
            End If

            If Not IsNothing(Data_Ril) Then
                strSql.AppendLine("   , Data_Ril = " & Agro_SQL_SaveDate(Data_Ril) & " ")
            End If

            If Not IsNothing(Qta_Ril) Then
                strSql.AppendLine("   , Qta_Ril = " & Agro_SQL_SaveNum(Qta_Ril) & " ")
            End If

            If Not IsNothing(Dose) Then
                strSql.AppendLine("   , Dose = " & Agro_SQL_SaveNum(Dose) & " ")
            End If

            If Not IsNothing(Ditta_Cod) Then
                strSql.AppendLine("   , Ditta_Cod = " & Agro_SQL_SaveNum(Ditta_Cod) & " ")
            End If

            If Not IsNothing(Dett_Cod) Then
                strSql.AppendLine("   , Dett_Cod = " & Agro_SQL_SaveNum(Dett_Cod) & " ")
            End If

            If Not IsNothing(Id_Insetto) Then
                strSql.AppendLine("   , Id_Insetto = " & Agro_SQL_SaveNum(Id_Insetto) & " ")
            End If

            If Not IsNothing(FF_Classe) Then
                strSql.AppendLine("   , FF_Classe = " & Agro_SQL_SaveNum(FF_Classe) & " ")
            End If

            If Not IsNothing(Mg) Then
                strSql.AppendLine("   , Mg = " & Agro_SQL_SaveNum(Mg) & " ")
            End If

            If Not IsNothing(N) Then
                strSql.AppendLine("   , N = " & Agro_SQL_SaveNum(N) & " ")
            End If

            If Not IsNothing(K) Then
                strSql.AppendLine("   , K = " & Agro_SQL_SaveNum(K) & " ")
            End If

            If Not IsNothing(P) Then
                strSql.AppendLine("   , P = " & Agro_SQL_SaveNum(P) & " ")
            End If

            If Not IsNothing(Parziale) Then
                strSql.AppendLine("   , Parziale = " & Agro_SQL_SaveNum(Parziale) & " ")
            End If

            If Not IsNothing(Nitrati) Then
                strSql.AppendLine("   , Nitrati = " & Agro_SQL_SaveNum(Nitrati) & " ")
            End If

            If Not IsNothing(Freatimetro) Then
                strSql.AppendLine("   , Freatimetro = " & Agro_SQL_SaveNum(Freatimetro) & " ")
            End If

            If Not IsNothing(Piezo1) Then
                strSql.AppendLine("   , Piezo1 = " & Agro_SQL_SaveNum(Piezo1) & " ")
            End If

            If Not IsNothing(Piezo2) Then
                strSql.AppendLine("   , Piezo2 = " & Agro_SQL_SaveNum(Piezo2) & " ")
            End If

            If Not IsNothing(Piezo3) Then
                strSql.AppendLine("   , Piezo3 = " & Agro_SQL_SaveNum(Piezo3) & " ")
            End If

            If Not IsNothing(Piezo4) Then
                strSql.AppendLine("   , Piezo4 = " & Agro_SQL_SaveNum(Piezo4) & " ")
            End If

            If Not IsNothing(Trap_Num) Then
                strSql.AppendLine("   , Trap_Num = " & Agro_SQL_SaveNum(Trap_Num) & " ")
            End If

            If Not IsNothing(Inn1_Data) Then
                strSql.AppendLine("   , Inn1_Data = " & Agro_SQL_SaveDate(Inn1_Data) & " ")
            End If

            If Not IsNothing(Inn2_Data) Then
                strSql.AppendLine("   , Inn2_Data = " & Agro_SQL_SaveDate(Inn2_Data) & " ")
            End If

            If Not IsNothing(Inn3_Data) Then
                strSql.AppendLine("   , Inn3_Data = " & Agro_SQL_SaveDate(Inn3_Data) & " ")
            End If

            If Not IsNothing(Inn4_Data) Then
                strSql.AppendLine("   , Inn4_Data = " & Agro_SQL_SaveDate(Inn4_Data) & " ")
            End If

            If Not IsNothing(Lotto) Then
                strSql.AppendLine("   , Lotto = '" & Agro_SQL_SaveText(Lotto) & "' ")
            End If

            If Not IsNothing(Extra_Int) Then
                strSql.AppendLine("   , Extra_Int = " & Agro_SQL_SaveNum(Extra_Int) & " ")
            End If

            If Not IsNothing(Extra_Str) Then
                strSql.AppendLine("   , Extra_Str = '" & Agro_SQL_SaveText(Extra_Str) & "' ")
            End If

            If Not IsNothing(Extra_Date) Then
                strSql.AppendLine("   , Extra_Date = " & Agro_SQL_SaveDate(Extra_Date) & " ")
            End If

            If Not IsNothing(Soglia_Cod) Then
                strSql.AppendLine("   , Soglia_Cod = " & Agro_SQL_SaveNum(Soglia_Cod) & " ")
            End If

            If Not IsNothing(Soglia_Quantita) Then
                strSql.AppendLine("   , Soglia_Quantita = " & Agro_SQL_SaveNum(Soglia_Quantita) & " ")
            End If

            If Not IsNothing(Soglia_Des) Then
                strSql.AppendLine("   , Soglia_Des = '" & Agro_SQL_SaveText(Soglia_Des) & "' ")
            End If

            If Not IsNothing(Efficienza) Then
                strSql.AppendLine("   , Efficienza = " & Agro_SQL_SaveNum(Efficienza) & " ")
            End If

            If Not IsNothing(Cu) Then
                strSql.AppendLine("   , Cu = " & Agro_SQL_SaveNum(Cu) & " ")
            End If

            If Not IsNothing(Validita_Inizio) Then
                strSql.AppendLine("   , Validita_Inizio = " & Agro_SQL_SaveDate(Validita_Inizio) & " ")
            End If

            If Not IsNothing(Validita_Fine) Then
                strSql.AppendLine("   , Validita_Fine = " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            End If

            '----------------------------------------------------------------------

            strSql.AppendLine(" WHERE Piva = '" & Agro_SQL_SaveText(Piva) & "' ")

            If Sa_Cod <> 0 Then
                strSql.AppendLine(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            strSql.AppendLine(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & " ")

            If Id_Mov <> 0 Then
                strSql.AppendLine(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & " ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.AppendLine(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & " ")
            End If

            If Id_Reg_Dettaglio <> 0 Then
                strSql.AppendLine(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & " ")
            End If

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

    Public Function Modifica_QuantitaAcqua_DaPercentuale(ByVal Piva As String,
                                                         ByVal Id_Agenda As Integer,
                                                         ByVal Percentuale As Double,
                                                         ByVal xFiltroAggiuntivo As String,
                                                         ByRef objParametri As AgronicaCoreParametri
                                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Movimenti_Dettagli_W.Modifica_QuantitaAcqua_DaPercentuale()"
        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            strSql.Length = 0
            strSql.Append(" UPDATE Mov_Dettaglio_Tecnico SET ")
            strSql.Append("    Qta_Ril              = Qta_Ril * " & Agro_SQL_SaveNum(Percentuale) & "/100  ")

            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")

            strSql.Append(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append(" AND   Id_Agenda         =  " & Agro_SQL_SaveNum(Id_Agenda) & "  ")

            ''qta_ril positiva (acqua totale)
            strSql.Append(" AND Qta_Ril > 0 ")

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

    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Integer,
                             ByVal Id_Agenda As Integer,
                             ByVal Id_Mov As Integer,
                             ByVal Id_Mov_Det As Integer,
                             ByVal Id_Reg_Dettaglio As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Mov_Dettaglio_Tecnico_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Mov = 0
        '   Id_Mov_Det = 0
        '   Id_Reg_Dettaglio = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Id_Agenda = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Agenda obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then
                strSql.Length = 0
                strSql.Append(" UPDATE Mov_Dettaglio_Tecnico ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM Mov_Dettaglio_Tecnico ")
                strSql.Append(" WHERE  1=1 ")
            End If

            If Piva <> String.Empty Then
                strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If


            strSql.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")


            If Id_Mov <> 0 Then
                strSql.Append(" AND Id_Mov = " & Agro_SQL_SaveNum(Id_Mov) & "   ")
            End If

            If Id_Mov_Det <> 0 Then
                strSql.Append(" AND Id_Mov_Det = " & Agro_SQL_SaveNum(Id_Mov_Det) & "   ")
            End If

            If Id_Reg_Dettaglio <> 0 Then
                strSql.Append(" AND Id_Reg_Dettaglio = " & Agro_SQL_SaveNum(Id_Reg_Dettaglio) & "   ")
            End If

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

#Region "Entity Framework"

    Public Sub Scrivi(ByRef Mov_dettaglio_tecnico As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico,
                      ByRef GiasContext As Gias_DeveloperServer_Entities,
                      ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_W.Scrivi()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Mov_dettaglio_tecnico, objParametriServer)

            Mov_dettaglio_tecnico.data_creazione = DateTime.Now
            Mov_dettaglio_tecnico.data_modifica = DateTime.Now
            Mov_dettaglio_tecnico.username_creazione = objParametriServer.UsernameOperazione
            Mov_dettaglio_tecnico.username_modifica = objParametriServer.UsernameOperazione

            GiasContext.Mov_Dettaglio_Tecnico.Add(Mov_dettaglio_tecnico)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Modifica(ByRef Mov_dettaglio_tecnico As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico,
                        ByRef GiasContext As Gias_DeveloperServer_Entities,
                        ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_W.Modifica()"
        Dim messaggioErrore As String = ""

        Try

            Valorizza(Mov_dettaglio_tecnico, objParametriServer)

            Mov_dettaglio_tecnico.data_modifica = DateTime.Now
            Mov_dettaglio_tecnico.username_modifica = objParametriServer.UsernameOperazione

            GiasContext.Entry(Mov_dettaglio_tecnico).State = EntityState.Modified

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Valorizza(ByRef Mov_dettaglio_tecnico As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico,
                         ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_W.Valorizza()"
        Dim messaggioErrore As String = ""

        Try

            If Mov_dettaglio_tecnico.Qta_Ril Is Nothing Then
                Mov_dettaglio_tecnico.Qta_Ril = 0
            End If

            If Mov_dettaglio_tecnico.Data_Ril Is Nothing OrElse Mov_dettaglio_tecnico.Data_Ril < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.Data_Ril = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.Ditta_Cod Is Nothing Then
                Mov_dettaglio_tecnico.Ditta_Cod = 0
            End If

            If Mov_dettaglio_tecnico.Dett_Cod Is Nothing Then
                Mov_dettaglio_tecnico.Dett_Cod = 0
            End If

            If Mov_dettaglio_tecnico.Id_Insetto Is Nothing Then
                Mov_dettaglio_tecnico.Id_Insetto = 0
            End If

            If Mov_dettaglio_tecnico.FF_Classe Is Nothing Then
                Mov_dettaglio_tecnico.FF_Classe = 0
            End If

            If Mov_dettaglio_tecnico.Dose Is Nothing Then
                Mov_dettaglio_tecnico.Dose = 0
            End If

            If Mov_dettaglio_tecnico.Mg Is Nothing Then
                Mov_dettaglio_tecnico.Mg = 0
            End If

            If Mov_dettaglio_tecnico.N Is Nothing Then
                Mov_dettaglio_tecnico.N = 0
            End If

            If Mov_dettaglio_tecnico.K Is Nothing Then
                Mov_dettaglio_tecnico.K = 0
            End If

            If Mov_dettaglio_tecnico.P Is Nothing Then
                Mov_dettaglio_tecnico.P = 0
            End If

            If Mov_dettaglio_tecnico.Parziale Is Nothing Then
                Mov_dettaglio_tecnico.Parziale = 0
            End If

            If Mov_dettaglio_tecnico.Nitrati Is Nothing Then
                Mov_dettaglio_tecnico.Nitrati = 0
            End If

            If Mov_dettaglio_tecnico.Freatimetro Is Nothing Then
                Mov_dettaglio_tecnico.Freatimetro = 0
            End If

            If Mov_dettaglio_tecnico.Piezo1 Is Nothing Then
                Mov_dettaglio_tecnico.Piezo1 = 0
            End If

            If Mov_dettaglio_tecnico.Piezo2 Is Nothing Then
                Mov_dettaglio_tecnico.Piezo2 = 0
            End If

            If Mov_dettaglio_tecnico.Piezo3 Is Nothing Then
                Mov_dettaglio_tecnico.Piezo3 = 0
            End If

            If Mov_dettaglio_tecnico.Piezo4 Is Nothing Then
                Mov_dettaglio_tecnico.Piezo4 = 0
            End If

            If Mov_dettaglio_tecnico.Sigla_AV Is Nothing Then
                Mov_dettaglio_tecnico.Sigla_AV = ""
            End If

            If Mov_dettaglio_tecnico.Trap_Num Is Nothing Then
                Mov_dettaglio_tecnico.Trap_Num = 0
            End If

            If Mov_dettaglio_tecnico.Inn1_Data Is Nothing OrElse Mov_dettaglio_tecnico.Inn1_Data < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.Inn1_Data = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.Inn2_Data Is Nothing OrElse Mov_dettaglio_tecnico.Inn2_Data < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.Inn2_Data = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.Inn3_Data Is Nothing OrElse Mov_dettaglio_tecnico.Inn3_Data < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.Inn3_Data = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.Inn4_Data Is Nothing OrElse Mov_dettaglio_tecnico.Inn4_Data < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.Inn4_Data = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.Av_Cod Is Nothing Then
                Mov_dettaglio_tecnico.Av_Cod = 0
            End If

            If Mov_dettaglio_tecnico.Av_Gru Is Nothing Then
                Mov_dettaglio_tecnico.Av_Gru = 0
            End If

            If Mov_dettaglio_tecnico.Lotto Is Nothing Then
                Mov_dettaglio_tecnico.Lotto = ""
            End If

            If Mov_dettaglio_tecnico.Extra_Str Is Nothing Then
                Mov_dettaglio_tecnico.Extra_Str = ""
            End If

            If Mov_dettaglio_tecnico.Extra_Int Is Nothing Then
                Mov_dettaglio_tecnico.Extra_Int = 0
            End If

            If Mov_dettaglio_tecnico.Extra_Date Is Nothing OrElse Mov_dettaglio_tecnico.Extra_Date < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.Extra_Date = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.Soglia_Cod Is Nothing Then
                Mov_dettaglio_tecnico.Soglia_Cod = 0
            End If

            If Mov_dettaglio_tecnico.Soglia_Des Is Nothing Then
                Mov_dettaglio_tecnico.Soglia_Des = ""
            End If

            If Mov_dettaglio_tecnico.Soglia_Quantita Is Nothing Then
                Mov_dettaglio_tecnico.Soglia_Quantita = 0
            End If

            If Mov_dettaglio_tecnico.Efficienza Is Nothing Then
                Mov_dettaglio_tecnico.Efficienza = 0
            End If

            If Mov_dettaglio_tecnico.Cu Is Nothing Then
                Mov_dettaglio_tecnico.Cu = 0
            End If

            If Mov_dettaglio_tecnico.validita_inizio < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.validita_inizio = AGRODATAINIZIO
            End If

            If Mov_dettaglio_tecnico.validita_fine < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.validita_fine = AGRODATAFINE
            End If

            If Mov_dettaglio_tecnico.datainvio Is Nothing OrElse Mov_dettaglio_tecnico.datainvio < AGRODATAINIZIO Then
                Mov_dettaglio_tecnico.datainvio = DateTime.Now
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

    Public Sub Elimina(ByRef Mov_dettaglio_tecnico As AgronicaCoreEntityFramework_POCO.Mov_Dettaglio_Tecnico,
                       ByRef GiasContext As Gias_DeveloperServer_Entities,
                       ByRef objParametriServer As AgronicaCoreParametri)

        Const nomeRoutine = "ContabDAL.Mov_Dettaglio_Tecnico_W.Elimina()"
        Dim messaggioErrore As String = ""

        Try

            GiasContext.Mov_Dettaglio_Tecnico.Attach(Mov_dettaglio_tecnico)
            GiasContext.Mov_Dettaglio_Tecnico.Remove(Mov_dettaglio_tecnico)

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

#End Region

End Class
