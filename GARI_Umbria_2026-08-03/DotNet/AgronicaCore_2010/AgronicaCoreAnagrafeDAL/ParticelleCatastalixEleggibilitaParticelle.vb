Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class ParticelleCatastalixEleggibilitaParticelle_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal Eleggibilita_Cod As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Part_Cod = ""
        '   PROV = ""
        '   COM = ""
        '   SEZIONE = ""
        '   FOGLIO = 0
        '   NUMERO = 0
        '   SUBALTERNO = ""
        '   Eleggibilita_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastalixEleggibilitaParticelle ")
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

                    If Eleggibilita_Cod <> 0 Then
                        StrSQL.Append(" AND Eleggibilita_Cod =  " & Agro_SQL_SaveNum(Eleggibilita_Cod) & " ")
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

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ParticelleCatastalixEleggibilitaParticelle ")
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

                    If Eleggibilita_Cod <> 0 Then
                        StrSQL.Append(" AND Eleggibilita_Cod =  " & Agro_SQL_SaveNum(Eleggibilita_Cod) & " ")
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

    Public Function Esiste_ParticelleCatastalixEleggibilitaParticelle(
                           ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                           ByVal Eleggibilita_Cod As String,
                           ByVal xFiltroAggiuntivo As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_R.Esiste_ParticelleCatastalixEleggibilitaParticelle()"
        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Dim bRet As Boolean = False

        Try

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

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append(" SELECT * ")
            strSql.Append(" FROM  ParticelleCatastalixEleggibilitaParticelle ")
            strSql.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSql.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

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

            If Eleggibilita_Cod <> 0 Then
                strSql.Append(" AND Eleggibilita_Cod =  " & Agro_SQL_SaveNum(Eleggibilita_Cod) & " ")
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

End Class


'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class ParticelleCatastalixEleggibilitaParticelle_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                           ByVal Eleggibilita_Cod As Int32,
                           ByVal Superficie As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append("INSERT INTO ParticelleCatastalixEleggibilitaParticelle(       ")
            strSql.Append("             PROV,               COM,            SEZIONE,     ")
            strSql.Append("             FOGLIO,             NUMERO,         SUBALTERNO,  ")
            strSql.Append("             Eleggibilita_Cod,   Superficie,                  ")

            strSql.Append("             Inviato,            DataInvio, ")
            strSql.Append("             Data_Creazione,     Data_Modifica, ")
            strSql.Append("             UserName_Creazione, UserName_Modifica, ")
            strSql.Append("             Validita_Inizio,    Validita_Fine ")
            strSql.Append("             ) ")

            strSql.Append(" VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Trim(PROV)) & "'  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Trim(COM)) & "'  ")
            strSql.Append("         ,'" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append("         , " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append("         , " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append("         ,'" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Eleggibilita_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Superficie) & "  ")

            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
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


    Public Function Modifica(ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Eleggibilita_Cod As Int32,
                             ByVal Superficie As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastalixEleggibilitaParticelle_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Eleggibilita_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Eleggibilita_Cod obbligatorio)")
            'End If

            '---------------------------------------------
            strSql.Length = 0
            strSql.Append("UPDATE ParticelleCatastalixEleggibilitaParticelle SET ")
            strSql.Append("    Superficie        =  " & Agro_SQL_SaveNum(Superficie))
            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.Append(" AND      Sezione     = '" & IIf(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append(" AND      FOGLIO      =  " & IIf(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append(" AND      Numero      =  " & IIf(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append(" AND      SUBALTERNO  = '" & IIf(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            strSql.Append(" AND      Eleggibilita_Cod = " & Agro_SQL_SaveNum(Eleggibilita_Cod) & " ")

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


    Public Function Cancella(ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Eleggibilita_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine As String = "AgronicaCoreContabDAL.ParticelleCatastalixEleggibilitaParticelle_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Eleggibilita_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Eleggibilita_Cod obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE ParticelleCatastalixEleggibilitaParticelle ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("     ,Inviato = -1 ")

                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM ParticelleCatastalixEleggibilitaParticelle ")

                strSql.Append(" WHERE  1=1 ")

            End If

            ' La clausola è la stessa per entrambe le query
            strSql.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append(" AND      SUBALTERNO =  '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")

            If Eleggibilita_Cod <> 0 Then
                strSql.Append(" AND    Eleggibilita_Cod = " & Agro_SQL_SaveNum(Eleggibilita_Cod) & " ")
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

End Class
