Imports System.Data.Entity
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreEntityFramework

Public Class ProgettixParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal PIVA As String,
                          ByVal Sa_Cod As Integer,
                          ByVal Appezza As Integer,
                          ByVal Id_Reg As Integer,
                          ByVal Progetto_Cod As Integer,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Integer,
                          ByVal NUMERO As Integer,
                          ByVal SUBALTERNO As String,
                          ByVal id_cod As Integer,
                          ByVal Val_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Appezza = 0
        '   Id_Reg = 0
        '   Progetto_Cod = 0
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Fine as xValidita_Fine ")
                    strSql.AppendLine(" FROM  ProgettixParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ProgettixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ProgettixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
                    End If

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If id_cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Cod = " & Agro_SQL_SaveNum(id_cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato >= 0 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato = -1 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY ProgettixParticelle.Validita_inizio ASC")
                    End If
                    '---------------------------------------------

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Fine as xValidita_Fine ")
                    strSql.AppendLine(" FROM  ProgettixParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ProgettixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ProgettixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")


                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Id_Reg <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
                    End If

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If id_cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Cod = " & Agro_SQL_SaveNum(id_cod) & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato >= 0 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato = -1 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY ProgettixParticelle.Validita_inizio ASC")
                    End If
                    '---------------------------------------------

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
    Public Function LeggixProgetto(ByVal PIVA As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal Appezza As Integer,
                                   ByVal Id_Reg As Integer,
                                   ByVal Progetto_Cod As Integer,
                                   ByVal id_cod As Integer,
                                   ByVal Val_Cod As String,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_R.LeggixProgetto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Appezza = 0
        '   Id_Reg = 0
        '   Progetto_Cod = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Fine as xValidita_Fine ")
                    strSql.AppendLine(" FROM  ProgettixParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ProgettixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ProgettixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Appezza = " & Appezza & " ")
                    End If

                    If Id_Reg <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Reg = " & Id_Reg & " ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Progetto_Cod = " & Progetto_Cod & " ")
                    End If

                    If id_cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Cod = " & id_cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato >= 0 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato = -1 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT ProgettixParticelle.*,    ParticelleCatastali.*, ISTAT.LOCALITA AS Comune,      ISTAT.COMUNI_PROV as Provincia , ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Fine as xValidita_Fine ")
                    strSql.AppendLine(" FROM  ProgettixParticelle , ParticelleCatastali , ISTAT ")
                    strSql.AppendLine(" WHERE ProgettixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ProgettixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ISTAT.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ISTAT.COM ")

                    If PIVA <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Piva = '" & Agro_SQL_SaveText(PIVA) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Sa_Cod = " & Sa_Cod & " ")
                    End If

                    If Appezza <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Appezza = " & Appezza & " ")
                    End If

                    If Id_Reg <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Reg = " & Id_Reg & " ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Progetto_Cod = " & Progetto_Cod & " ")
                    End If

                    If id_cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Cod = " & id_cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato >= 0 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato = -1 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
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
    Public Function LeggixParticella(ByVal PROV As String,
                                     ByVal COM As String,
                                     ByVal SEZIONE As String,
                                     ByVal FOGLIO As Integer,
                                     ByVal NUMERO As Integer,
                                     ByVal SUBALTERNO As String,
                                     ByVal id_cod As Integer,
                                     ByVal Val_Cod As String,
                                     ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_R.LeggixParticella()"

        '====================================================================================
        'Parametri opzionali :
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Fine as xValidita_Fine ")
                    strSql.AppendLine(" FROM  ProgettixParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ProgettixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ProgettixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If id_cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Cod = " & id_cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato >= 0 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato = -1 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY ProgettixParticelle.Validita_inizio ASC")
                    End If
                    '---------------------------------------------

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.AppendLine(" SELECT *, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Inizio as xValidita_Inizio, ")
                    strSql.AppendLine("          ProgettixParticelle.Validita_Fine as xValidita_Fine ")
                    strSql.AppendLine(" FROM  ProgettixParticelle , ParticelleCatastali ")
                    strSql.AppendLine(" WHERE ProgettixParticelle.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ParticelleCatastali.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.AppendLine(" AND   ProgettixParticelle.PROV = ParticelleCatastali.PROV ")
                    strSql.AppendLine(" AND   ProgettixParticelle.COM = ParticelleCatastali.COM ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SEZIONE = ParticelleCatastali.SEZIONE ")
                    strSql.AppendLine(" AND   ProgettixParticelle.FOGLIO = ParticelleCatastali.FOGLIO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.NUMERO = ParticelleCatastali.NUMERO ")
                    strSql.AppendLine(" AND   ProgettixParticelle.SUBALTERNO = ParticelleCatastali.SUBALTERNO ")

                    If PROV <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If id_cod <> 0 Then
                        strSql.AppendLine(" AND ProgettixParticelle.Id_Cod = " & id_cod & " ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        strSql.AppendLine(" AND ProgettixParticelle.Val_Cod Like '%" & Agro_SQL_SaveText(Val_Cod) & "%' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        strSql.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato >= 0 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato >= 0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.AppendLine(" AND     ProgettixParticelle.Inviato = -1 ")
                            strSql.AppendLine(" AND     ParticelleCatastali.Inviato = -1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.AppendLine(" ORDER BY ProgettixParticelle.Validita_inizio ASC")
                    End If
                    '---------------------------------------------

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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

Public Class ProgettixParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal PIVA As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Id_Reg As Integer,
                           ByVal Progetto_Cod As Integer,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Integer,
                           ByVal NUMERO As Integer,
                           ByVal SUBALTERNO As String,
                           ByVal id_cod As Integer,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
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
            strSql.AppendLine("INSERT INTO ProgettixParticelle(       ")
            strSql.AppendLine("                    PIVA, Sa_Cod, Appezza, Id_Reg, Progetto_Cod, ")
            strSql.AppendLine("                    PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,  ")
            strSql.AppendLine("                    Id_Cod, Val_Cod, ")
            strSql.AppendLine("                    Inviato,            DataInvio, ")
            strSql.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            strSql.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            strSql.AppendLine("                    Validita_Inizio,    Validita_Fine, ")
            strSql.AppendLine("                    Validazione,        Data_Validazione, UserName_Validazione ")
            strSql.AppendLine("                    ) ")
            strSql.AppendLine("VALUES (")
            strSql.AppendLine("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "'  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            strSql.AppendLine("         ,'" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.AppendLine("         , " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.AppendLine("         , " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.AppendLine("         ,'" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.AppendLine("         ," & Agro_SQL_SaveNum(id_cod) & " ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(Val_Cod) & "'  ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , Null  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.AppendLine("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.AppendLine("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.AppendLine("         , 0  ")
            strSql.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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

    '##############################################################################################
    Public Function Modifica(ByVal PIVA As String,
                             ByVal Sa_Cod As Long,
                             ByVal Appezza As Long,
                             ByVal Id_Reg As Long,
                             ByVal Progetto_Cod As Long,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Long,
                             ByVal NUMERO As Long,
                             ByVal SUBALTERNO As String,
                             ByVal id_cod As Long,
                             ByVal Val_Cod As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine("UPDATE ProgettixParticelle SET ")
            strSql.AppendLine("       Val_Cod  = '" & Agro_SQL_SaveText(Val_Cod) & "' ")
            strSql.AppendLine("       ,Inviato              =  0 ")
            strSql.AppendLine("       ,DataInvio            =  Null ")
            strSql.AppendLine("       ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.AppendLine("       ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.AppendLine("       ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.AppendLine("       ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.AppendLine(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            strSql.AppendLine(" AND      Sa_Cod      =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            strSql.AppendLine(" AND      Appezza     =  " & Agro_SQL_SaveNum(Appezza) & "  ")
            strSql.AppendLine(" AND      Id_Reg      =  " & Agro_SQL_SaveNum(Id_Reg) & "  ")
            strSql.AppendLine(" AND      Progetto_Cod =  " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            strSql.AppendLine(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.AppendLine(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.AppendLine(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.AppendLine(" AND      FOGLIO      = " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.AppendLine(" AND      Numero      = " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.AppendLine(" AND      SUBALTERNO  =  '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.AppendLine(" AND      Id_Cod      =  " & Agro_SQL_SaveNum(id_cod) & "  ")

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


    '##############################################################################################
    'Questa routine cancella UNA O PIU' associazioni Appezzamento-Particelle.
    'E' utilizzata in caso di cancellazione di una particella dal catasto
    Public Function CancellaxProgetto(ByVal Piva As String,
                                      ByVal Sa_Cod As Integer,
                                      ByVal Appezza As Integer,
                                      ByVal Id_Reg As Integer,
                                      ByVal Progetto_Cod As Integer,
                                      ByVal PROV As String,
                                      ByVal COM As String,
                                      ByVal SEZIONE As String,
                                      ByVal FOGLIO As Integer,
                                      ByVal NUMERO As Integer,
                                      ByVal SUBALTERNO As String,
                                      ByVal xFiltroAggiuntivo As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_W.CancellaxProgetto()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0 
        '   Appezza = 0 
        '   Id_Reg = 0 
        '   Progetto_Cod = 0 
        '   PROV = "" 
        '   COM = "" 
        '   SEZIONE = "" 
        '   FOGLIO = 0 
        '   NUMERO = 0 
        '   SUBALTERNO = "" 
        '====================================================================================

        '============================================================================
        'NOTA
        'Se Validita_Fine è un valore dummy allora eseguo una cancellazione reale
        'Se Validita_Fine ha un valore valido allora eseguo una cancellazione logica
        '  nella data impostata, modificando il valore del campo stesso
        '  e impostando la Username_Modifica
        'NOTA 2:
        'Se tento di cancellare fisicamente un record con INVIATO=1
        'allora pongo INVIATO=-1 e Validita_Fine="31/12/1899"
        'questo per consentire la ri-sincronizzazione col server
        '============================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.AppendLine(" UPDATE ProgettixParticelle ")
                strSql.AppendLine(" SET ")
                strSql.AppendLine("       Validita_Fine = " & Agro_SQL_SaveDate(#12/31/1899#) & " ")
                strSql.AppendLine("      ,Data_Modifica = " & Agro_SQL_SaveDateTime(Date.Now) & " ")
                strSql.AppendLine("      ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.AppendLine("      ,Inviato = -1 ")
                strSql.AppendLine(" WHERE PIVA <> '0' ")
                strSql.AppendLine(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.AppendLine(" DELETE ")
                strSql.AppendLine(" FROM     ProgettixParticelle ")
                strSql.AppendLine(" WHERE    Piva <> '0' ")

            End If

            If Piva <> "" Then
                strSql.Append(" AND   Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND   Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                strSql.Append(" AND   Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If

            If Id_Reg <> 0 Then
                strSql.Append(" AND   Id_Reg = " & Agro_SQL_SaveNum(Id_Reg) & " ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.Append(" AND   Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & " ")
            End If

            If PROV <> "" Then
                strSql.Append(" AND   Prov = '" & Agro_SQL_SaveText(PROV) & "' ")
            End If

            If COM <> "" Then
                strSql.Append(" AND   Com = '" & Agro_SQL_SaveText(COM) & "' ")
            End If

            If SEZIONE <> "" Then
                strSql.Append(" AND   Sezione = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                strSql.Append(" AND   Foglio = " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                strSql.Append(" AND   Numero = " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                strSql.Append(" AND   Subalterno = '" & Agro_SQL_SaveText(LCase(SUBALTERNO)) & "' ")
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


    '######################################################################################################
    Public Function ModificaChiave(ByVal PROV As String,
                                   ByVal COM As String,
                                   ByVal SEZIONE As String,
                                   ByVal FOGLIO As Integer,
                                   ByVal NUMERO As Integer,
                                   ByVal SUBALTERNO As String,
                                   ByVal PROV_Origine As String,
                                   ByVal COM_Origine As String,
                                   ByVal SEZIONE_Origine As String,
                                   ByVal FOGLIO_Origine As Integer,
                                   ByVal NUMERO_Origine As Integer,
                                   ByVal SUBALTERNO_Origine As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_W.ModificaChiave()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine(" UPDATE ProgettiXParticelle ")
            strSql.AppendLine(" SET  ")
            strSql.AppendLine(" PROV = '" & Agro_SQL_SaveText(PROV) & "', ")
            strSql.AppendLine(" COM = '" & Agro_SQL_SaveText(COM) & "', ")
            strSql.AppendLine(" SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "', ")
            strSql.AppendLine(" FOGLIO =" & FOGLIO & ", ")
            strSql.AppendLine(" NUMERO =" & NUMERO & ", ")
            strSql.AppendLine(" Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "',")
            strSql.AppendLine(" Data_Modifica =" & Agro_SQL_SaveDateTime(Date.Now) & ",")
            strSql.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")
            strSql.AppendLine(" WHERE PROV = '" & Agro_SQL_SaveText(PROV_Origine) & "' ")
            strSql.AppendLine(" AND COM = '" & Agro_SQL_SaveText(COM_Origine) & "' ")
            strSql.AppendLine(" AND SEZIONE ='" & Agro_SQL_SaveText(SEZIONE_Origine) & "' ")
            strSql.AppendLine(" AND FOGLIO =" & FOGLIO_Origine & " ")
            strSql.AppendLine(" AND NUMERO =" & NUMERO_Origine & " ")
            strSql.AppendLine(" AND Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO_Origine) & "'")

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

    Public Function ModificaChiave2(ByVal Piva As String,
                                    ByVal Sa_Cod As Integer,
                                    ByVal appezza As Integer,
                                    ByVal Id_Reg As Integer,
                                    ByVal Progetto_Cod As Integer,
                                    ByVal PROV As String,
                                    ByVal COM As String,
                                    ByVal SEZIONE As String,
                                    ByVal FOGLIO As Integer,
                                    ByVal NUMERO As Integer,
                                    ByVal SUBALTERNO As String,
                                    ByVal Piva_Origine As String,
                                    ByVal Sa_Cod_Origine As Integer,
                                    ByVal appezza_Origine As Integer,
                                    ByVal Id_Reg_Origine As Integer,
                                    ByVal Progetto_Cod_Origine As Integer,
                                    ByVal PROV_Origine As String,
                                    ByVal COM_Origine As String,
                                    ByVal SEZIONE_Origine As String,
                                    ByVal FOGLIO_Origine As Integer,
                                    ByVal NUMERO_Origine As Integer,
                                    ByVal SUBALTERNO_Origine As String,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_W.ModificaChiave2()"

        Dim messaggioErrore As String = ""
        Dim strSql As New StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.AppendLine(" UPDATE ProgettiXParticelle ")
            strSql.AppendLine(" SET  ")
            strSql.AppendLine(" PIVA = '" & Agro_SQL_SaveText(Piva) & "', ")
            strSql.AppendLine(" SA_COD = " & Sa_Cod & ", ")
            strSql.AppendLine(" APPEZZA = " & appezza & ", ")
            strSql.AppendLine(" ID_REG = " & Id_Reg & ", ")
            strSql.AppendLine(" PROGETTO_COD = " & Progetto_Cod & ", ")
            strSql.AppendLine(" PROV = '" & Agro_SQL_SaveText(PROV) & "', ")
            strSql.AppendLine(" COM = '" & Agro_SQL_SaveText(COM) & "', ")
            strSql.AppendLine(" SEZIONE = '" & Agro_SQL_SaveText(SEZIONE) & "', ")
            strSql.AppendLine(" FOGLIO =" & FOGLIO & ", ")
            strSql.AppendLine(" NUMERO =" & NUMERO & ", ")
            strSql.AppendLine(" Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO) & "',")
            strSql.AppendLine(" Data_Modifica =" & Agro_SQL_SaveDateTime(Date.Now) & ",")
            strSql.AppendLine(" Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UtenteCodFiscale) & "'")
            strSql.AppendLine(" WHERE PIVA = '" & Agro_SQL_SaveText(Piva_Origine) & "' ")
            strSql.AppendLine(" AND SA_COD = " & Sa_Cod_Origine & " ")
            strSql.AppendLine(" AND APPEZZA = " & appezza_Origine & " ")
            strSql.AppendLine(" AND ID_REG = " & Id_Reg_Origine & " ")
            strSql.AppendLine(" AND PROGETTO_COD = " & Progetto_Cod_Origine & " ")
            strSql.AppendLine(" AND PROV = '" & Agro_SQL_SaveText(PROV_Origine) & "' ")
            strSql.AppendLine(" AND COM = '" & Agro_SQL_SaveText(COM_Origine) & "' ")
            strSql.AppendLine(" AND SEZIONE ='" & Agro_SQL_SaveText(SEZIONE_Origine) & "' ")
            strSql.AppendLine(" AND FOGLIO =" & FOGLIO_Origine & " ")
            strSql.AppendLine(" AND NUMERO =" & NUMERO_Origine & " ")
            strSql.AppendLine(" AND Subalterno = '" & Agro_SQL_SaveText(SUBALTERNO_Origine) & "'")

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

#Region "Entity Framework"

    Public Sub ScriviModificaElimina(ByVal Piva As String,
                                     ByVal Sa_Cod As Integer,
                                     ByVal appezza As Integer,
                                     ByVal Id_Reg As Integer,
                                     ByVal Progetto_Cod As Integer,
                                     ByVal PROV As String,
                                     ByVal COM As String,
                                     ByVal SEZIONE As String,
                                     ByVal FOGLIO As Integer,
                                     ByVal NUMERO As Integer,
                                     ByVal SUBALTERNO As String,
                                     ByVal id_cod As Integer,
                                     ByVal Val_Cod As String,
                                     ByRef objParametriServer As AgronicaCoreParametri,
                                     ByRef GiasContext As Gias_DeveloperServer_Entities)

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ProgettixParticelle_W.ScriviModificaElimina()"
        Dim messaggioErrore As String = ""

        Try

            Dim pro_PartL = From ic In GiasContext.ProgettixParticelle
                            Where ic.PIVA = Piva AndAlso
                                  ic.sa_cod = Sa_Cod AndAlso
                                  ic.appezza = appezza AndAlso
                                  ic.Id_Reg = Id_Reg AndAlso
                                  ic.Progetto_Cod = 0 AndAlso
                                  ic.id_cod = id_cod AndAlso
                                  ic.PROV = PROV AndAlso
                                  ic.COM = COM AndAlso
                                  ic.SEZIONE = SEZIONE AndAlso
                                  ic.FOGLIO = FOGLIO AndAlso
                                  ic.NUMERO = NUMERO AndAlso
                                  ic.SUBALTERNO = SUBALTERNO
                            Select ic

            Dim operazione As enum_TipoOperazioneDB

            If pro_PartL.Count > 0 AndAlso Val_Cod <> "" Then
                operazione = enum_TipoOperazioneDB.Modifica
            ElseIf pro_PartL.Count > 0 AndAlso Val_Cod = "" Then
                operazione = enum_TipoOperazioneDB.Cancellazione
            ElseIf pro_PartL.Count = 0 AndAlso Val_Cod = "" Then
                operazione = enum_TipoOperazioneDB.Lettura
            ElseIf pro_PartL.Count = 0 AndAlso Val_Cod <> "" Then
                operazione = enum_TipoOperazioneDB.Scrittura
            End If

            Select Case operazione
                Case enum_TipoOperazioneDB.Scrittura

                    Dim proPart As New AgronicaCoreEntityFramework_POCO.ProgettixParticelle With {
                        .PIVA = Piva,
                        .sa_cod = Sa_Cod,
                        .appezza = appezza,
                        .Id_Reg = Id_Reg,
                        .Progetto_Cod = Progetto_Cod,
                        .id_cod = id_cod,
                        .PROV = PROV,
                        .COM = COM,
                        .SEZIONE = SEZIONE,
                        .FOGLIO = FOGLIO,
                        .NUMERO = NUMERO,
                        .SUBALTERNO = SUBALTERNO,
                        .val_cod = Val_Cod,
                        .inviato = 0,
                        .Data_Creazione = DateTime.Now,
                        .Data_Modifica = DateTime.Now,
                        .Validita_Inizio = AGRODATAINIZIO,
                        .Validita_Fine = AGRODATAFINE,
                        .Username_Creazione = objParametriServer.UsernameOperazione,
                        .Username_Modifica = objParametriServer.UsernameOperazione
                    }

                    GiasContext.ProgettixParticelle.Add(proPart)

                Case enum_TipoOperazioneDB.Modifica
                    Dim proPart = pro_PartL.FirstOrDefault
                    proPart.val_cod = Val_Cod
                    proPart.Data_Modifica = DateTime.Now
                    proPart.Username_Modifica = objParametriServer.UsernameOperazione

                    GiasContext.Entry(proPart).State = EntityState.Modified

                Case enum_TipoOperazioneDB.Cancellazione

                    Dim proPart = pro_PartL.FirstOrDefault
                    GiasContext.ProgettixParticelle.Remove(proPart)

            End Select

            GiasContext.SaveChanges()

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametriServer, nomeRoutine, messaggioErrore)
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

    End Sub

#End Region

End Class
