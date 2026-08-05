Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class ParticelleCatastalixMacrousi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal Macrouso_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal Numero_Fascicolo As String = "",
                          Optional ByVal Data_Validazione_Fascicolo As Date = AGRODATAINIZIO
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Macrouso_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO, MACROUSO_COD, SUPERFICIE, Validita_Inizio, Validita_Fine ")
                    StrSQL.Append(" FROM  ParticelleCatastalixMacrousi ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
                    End If

                    If PROV <> "" Then
                        StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If Numero_Fascicolo <> "" Then
                        StrSQL.Append(" AND Numero_Fascicolo =  '" & Agro_SQL_SaveText(Numero_Fascicolo) & "' ")
                    End If

                    If Data_Validazione_Fascicolo <> AGRODATAINIZIO Then
                        StrSQL.Append(" AND Data_Validazione_Fascicolo =  " & Agro_SQL_SaveDateTime(Data_Validazione_Fascicolo) & " ")
                    End If

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
                    Else
                        StrSQL.Append(" ORDER BY Validita_inizio ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastalixMacrousi ")
                    StrSQL.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

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
                    Else
                        StrSQL.Append(" ORDER BY Validita_inizio ASC")
                    End If


                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT ParticelleCatastalixMacrousi.* , Macrousi.Macrouso_Des as Macrouso_Des ")
                    StrSQL.Append(" FROM  ParticelleCatastalixMacrousi")
                    StrSQL.Append(" INNER Join Macrousi ON ParticelleCatastalixMacrousi.Macrouso_Cod = Macrousi.Macrouso_Cod")
                    StrSQL.Append(" WHERE ParticelleCatastalixMacrousi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   ParticelleCatastalixMacrousi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Macrouso_Cod <> "" Then
                        StrSQL.Append(" AND ParticelleCatastalixMacrousi.Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ParticelleCatastalixMacrousi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ParticelleCatastalixMacrousi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParticelleCatastalixMacrousi.Validita_inizio ASC")
                    End If

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

    Public Function Leggi_DaCentro(ByVal Piva As String,
                                   ByVal Sa_Cod As Integer,
                                   ByVal PROV As String,
                                   ByVal COM As String,
                                   ByVal SEZIONE As String,
                                   ByVal FOGLIO As Int32,
                                   ByVal NUMERO As Int32,
                                   ByVal SUBALTERNO As String,
                                   ByVal Macrouso_Cod As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R.Leggi_DaCentro()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Macrouso_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT ParticelleCatastalixMacrousi.* , Macrousi.Macrouso_Des as Macrouso_Des ")
            strSql.Append(" FROM    ImpreseXParticelle INNER JOIN ")
            strSql.Append(" ParticelleCatastalixMacrousi ON ImpreseXParticelle.PROV = ParticelleCatastalixMacrousi.PROV AND ImpreseXParticelle.COM = ParticelleCatastalixMacrousi.COM AND ")
            strSql.Append(" ImpreseXParticelle.SEZIONE = ParticelleCatastalixMacrousi.SEZIONE AND ImpreseXParticelle.FOGLIO = ParticelleCatastalixMacrousi.FOGLIO AND  ")
            strSql.Append(" ImpreseXParticelle.NUMERO = ParticelleCatastalixMacrousi.NUMERO AND  ")
            strSql.Append(" ImpreseXParticelle.SUBALTERNO = ParticelleCatastalixMacrousi.SUBALTERNO INNER JOIN ")
            strSql.Append(" Macrousi ON ParticelleCatastalixMacrousi.Macrouso_Cod = Macrousi.Macrouso_Cod ")

            strSql.Append(" WHERE ParticelleCatastalixMacrousi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND   ParticelleCatastalixMacrousi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.Append(" AND ImpreseXParticelle.piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If Sa_Cod <> 0 Then
                strSql.Append(" AND ImpreseXParticelle.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If PROV <> "" Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   ParticelleCatastalixMacrousi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   ParticelleCatastalixMacrousi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY ParticelleCatastalixMacrousi.Validita_inizio ASC")
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

    Public Function Esiste_ParticelleCatastalixMacrousi(
                               ByVal Piva As String,
                               ByVal PROV As String,
                               ByVal COM As String,
                               ByVal SEZIONE As String,
                               ByVal FOGLIO As Int32,
                               ByVal NUMERO As Int32,
                               ByVal SUBALTERNO As String,
                               ByVal Macrouso_Cod As String,
                               ByVal xFiltroAggiuntivo As String,
                               ByRef objParametri As AgronicaCoreParametri
                               ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R.Esiste_ParticelleCatastalixMacrousi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Macrouso_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Dim bRet As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If PROV = "" Then
                Throw New Exception("Parametro non corretto nella query (Provincia obbligatoria)")
            End If

            If COM = "" Then
                Throw New Exception("Parametro non corretto nella query (Comune obbligatorio)")
            End If

            If SEZIONE = "" Then
                Throw New Exception("Parametro non corretto nella query (Sezione obbligatoria)")
            End If

            If FOGLIO = 0 Then
                Throw New Exception("Parametro non corretto nella query (Foglio obbligatorio)")
            End If

            If NUMERO = 0 Then
                Throw New Exception("Parametro non corretto nella query (Numero obbligatorio)")
            End If

            If SUBALTERNO = "" Then
                Throw New Exception("Parametro non corretto nella query (Subalterno obbligatorio)")
            End If

            If Macrouso_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (Macrouso_Cod obbligatorio)")
            End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  ParticelleCatastalixMacrousi ")
            strSql.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If PROV <> "" Then
                strSql.Append(" AND PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If

            If COM <> "" Then
                strSql.Append(" AND COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If

            If SEZIONE <> "" Then
                strSql.Append(" AND SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
            End If

            If FOGLIO <> 0 Then
                strSql.Append(" AND FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
            End If

            If NUMERO <> 0 Then
                strSql.Append(" AND NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
            End If

            If SUBALTERNO <> "" Then
                strSql.Append(" AND SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
            End If

            If Macrouso_Cod <> "" Then
                strSql.Append(" AND Macrouso_Cod =  '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

            If dt IsNot Nothing Then
                If dt.Rows.Count > 0 Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            bRet = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        dt = Nothing
        Return bRet

    End Function


    Public Function Leggi_Macrousi(ByVal Piva As String,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByVal xOrderBy As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_R.Leggi_Macrousi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            strSql.Length = 0
            strSql.Append(" SELECT DISTINCT ParticelleCatastalixMacrousi.Macrouso_Cod , Macrousi.Macrouso_Des as Macrouso_Des ")
            strSql.Append(" FROM ParticelleCatastalixMacrousi ")
            strSql.Append(" INNER JOIN Macrousi ON ParticelleCatastalixMacrousi.Macrouso_Cod = Macrousi.Macrouso_Cod ")

            strSql.Append(" WHERE ParticelleCatastalixMacrousi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND   ParticelleCatastalixMacrousi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If Piva <> "" Then
                strSql.Append(" AND ParticelleCatastalixMacrousi.piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   ParticelleCatastalixMacrousi.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   ParticelleCatastalixMacrousi.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                strSql.Append(" ORDER BY Macrouso_Des ASC")
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class ParticelleCatastalixMacrousi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Integer,
                           ByVal NUMERO As Integer,
                           ByVal SUBALTERNO As String,
                           ByVal Macrouso_Cod As String,
                           ByVal Superficie As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Numero_Fascicolo As String = "",
                           Optional ByVal Data_Validazione_Fascicolo As Date = AGRODATAINIZIO,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_creazione = #2/1/1900# Then
            Data_creazione = Now
        End If

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_creazione = "" Then
            username_creazione = objParametri.UsernameOperazione
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            strSql.Length = 0
            strSql.Append("INSERT INTO ParticelleCatastalixMacrousi(       ")
            strSql.Append("             PIVA,                                           ")
            strSql.Append("             PROV,               COM,            SEZIONE,     ")
            strSql.Append("             FOGLIO,             NUMERO,         SUBALTERNO,  ")
            strSql.Append("             Macrouso_Cod,   Superficie,                  ")
            strSql.Append("             Numero_Fascicolo,   Data_Validazione_Fascicolo,                  ")

            strSql.Append("             Inviato,            DataInvio, ")
            strSql.Append("             Data_Creazione,     Data_Modifica, ")
            strSql.Append("             UserName_Creazione, UserName_Modifica, ")
            strSql.Append("             Validita_Inizio,    Validita_Fine ")
            strSql.Append("             ) ")

            strSql.Append(" VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            strSql.Append("         ,'" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append("         , " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append("         , " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append("         ,'" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Macrouso_Cod) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Superficie) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Numero_Fascicolo) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_Validazione_Fascicolo) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")

            strSql.Append(") ")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            messaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            messaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Modifica(ByVal Piva As String,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Macrouso_Cod As String,
                             ByVal Superficie As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri,
                             Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                             Optional ByVal username_modifica As String = "",
                             Optional ByVal Numero_Fascicolo As String = Nothing,
                             Optional ByVal Data_Validazione_Fascicolo As Date? = Nothing
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        If Data_modifica = #2/1/1900# Then
            Data_modifica = Now
        End If

        If username_modifica = "" Then
            username_modifica = objParametri.UsernameOperazione
        End If

        Try

            If Macrouso_Cod = "" Then
                Throw New Exception("Parametro non corretto nella query (Macrouso_Cod obbligatorio)")
            End If

            strSql.Length = 0
            strSql.Append("UPDATE ParticelleCatastalixMacrousi SET ")
            strSql.Append("    Superficie        =  " & Agro_SQL_SaveNum(Superficie))
            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Data_modifica))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(username_modifica) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            If Not IsNothing(Numero_Fascicolo) Then
                strSql.AppendLine("   ,Numero_Fascicolo =  '" & Agro_SQL_SaveText(Numero_Fascicolo) & "' ")
            End If

            If Not IsNothing(Data_Validazione_Fascicolo) Then
                strSql.AppendLine("   ,Data_Validazione_Fascicolo =  " & Agro_SQL_SaveDate(Data_Validazione_Fascicolo) & " ")
            End If

            strSql.Append(" WHERE    PIVA        = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            strSql.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append(" AND      SUBALTERNO  = '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            strSql.Append(" AND      Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")

            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            messaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            messaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Cancella(ByVal PIVA As String,
                             ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Macrouso_Cod As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.ParticelleCatastalixMacrousi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE ParticelleCatastalixMacrousi ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("     ,Inviato = -1 ")

                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM ParticelleCatastalixMacrousi  ")

                strSql.Append(" WHERE  1=1 ")

            End If

            ' La clausola è la stessa per entrambe le query
            strSql.Append(" AND      PIVA        = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            strSql.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append(" AND      SUBALTERNO =  '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            If Macrouso_Cod <> "" Then
                strSql.Append(" AND      Macrouso_Cod = '" & Agro_SQL_SaveText(Macrouso_Cod) & "' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = "Particella: " & PROV & " " & COM & " " & SEZIONE & " " & CStr(FOGLIO) & " " & CStr(NUMERO) & " " & SUBALTERNO & vbCrLf
            messaggioErrore &= "Macrouso: " & CStr(Macrouso_Cod) & vbCrLf
            messaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

    Public Function Aggiorna_Validita_Fine_Fascicoli_Precedenti(ByVal Piva As String,
                                                                ByRef objParametri As AgronicaCoreParametri
                                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixMacrousi_W.Aggiorna_Validita_Fine_Fascicoli_Precedenti()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append(" UPDATE ParticelleCatastalixMacrousi       ")
            strSql.Append("        SET ParticelleCatastalixMacrousi.Validita_Fine = (  ")
            strSql.Append("             SELECT DATEADD(dd, -1,MIN(M2.Validita_Inizio))  ")
            strSql.Append("             FROM ParticelleCatastalixMacrousi M2  ")
            strSql.Append("             WHERE ParticelleCatastalixMacrousi.Data_Validazione_Fascicolo < M2.Data_Validazione_Fascicolo ")
            strSql.Append("             AND ParticelleCatastalixMacrousi.Piva = M2.Piva)   ")

            strSql.Append(" FROM ParticelleCatastalixMacrousi ")

            strSql.Append(" WHERE    PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")

            strSql.Append(" AND exists (SELECT * from ParticelleCatastalixMacrousi M2 ")
            strSql.Append("            WHERE ParticelleCatastalixMacrousi.Data_Validazione_Fascicolo < M2.Data_Validazione_Fascicolo  ")
            strSql.Append("            AND ParticelleCatastalixMacrousi.Piva = M2.Piva)   ")
            strSql.Append(" AND Numero_Fascicolo IS NOT NULL  ")
       
            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, strSql.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore &= "Errore: " & ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
