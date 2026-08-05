Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class ParoleChiavexImprese_R

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal ParolaChiave As String,
                          ByVal Piva As String,
                          ByVal Sa_cod As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParoleChiavexImprese_R.Leggi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try


            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Pivasuperuser, parolachiave, Piva, sa_cod FROM ParoleChiavexImprese  ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND     PivaSuperuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")

                    If ParolaChiave <> "" Then
                        StrSQL.Append(" AND ParolaChiave = '" & Agro_SQL_SaveText(ParolaChiave) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * FROM ParoleChiavexImprese  ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND     PivaSuperuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")

                    If ParolaChiave <> "" Then
                        StrSQL.Append(" AND ParolaChiave = '" & Agro_SQL_SaveText(ParolaChiave) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    'Select Case objParametri.FlagVisibilita
                    '    Case enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato >=0 ")
                    '    Case enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato =-1 ")
                    '    Case enumVisibilita.Visibilita_Tutti
                    '        '...................................
                    '    Case Else
                    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    'End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParolaChiave, Piva, Sa_cod ASC")
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


    Public Function Leggi_Tutto(ByVal ParolaChiave As String,
                                ByVal Piva As String,
                                ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreParametri
                                ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParoleChiavexImprese_R.Leggi_Tutto()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Pivasuperuser, parolachiave, Piva, sa_cod FROM ParoleChiavexImprese  ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND     PivaSuperuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If ParolaChiave <> "" Then
                        StrSQL.Append(" AND ParolaChiave = '" & Agro_SQL_SaveText(ParolaChiave) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  * FROM ParoleChiavexImprese  ")
                    StrSQL.Append(" WHERE   Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND     PivaSuperuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    End If

                    If ParolaChiave <> "" Then
                        StrSQL.Append(" AND ParolaChiave = '" & Agro_SQL_SaveText(ParolaChiave) & "' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    'Select Case objParametri.FlagVisibilita
                    '    Case enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato >=0 ")
                    '    Case enumVisibilita.Visibilita_SoloNonCancellati
                    '        StrSQL.Append(" AND   Inviato =-1 ")
                    '    Case enumVisibilita.Visibilita_Tutti
                    '        '...................................
                    '    Case Else
                    '        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    'End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParolaChiave, Piva, Sa_cod ASC")
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

End Class

'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class ParoleChiavexImprese_W

    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal ParolaChiave As String,
                           ByVal Piva As String,
                           ByVal Sa_cod As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParoleChiavexImprese_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append("INSERT INTO ParoleChiavexImprese( ")
            strSql.Append(" PivaSuperuser,   ParolaChiave, ")
            strSql.Append(" Piva,   Sa_cod, ")
            strSql.Append(" Inviato,            DataInvio, ")
            strSql.Append(" Data_Creazione,     Data_Modifica, ")
            strSql.Append(" UserName_Creazione, UserName_Modifica, ")
            strSql.Append(" Validita_Inizio,    Validita_Fine ")
            strSql.Append(" ) ")

            strSql.Append("VALUES ( ")
            strSql.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append(" ,'" & Agro_SQL_SaveText(ParolaChiave) & "' ")
            strSql.Append(" ,'" & Agro_SQL_SaveText(Piva) & "' ")
            strSql.Append(" ," & Agro_SQL_SaveNum(Sa_cod) & " ")

            strSql.Append(" , 0  ")
            strSql.Append(" , Null  ")
            strSql.Append(" , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append(" , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append(" ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            strSql.Append(" , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append(" , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.Append(" )")

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


    'Public Function Modifica(ByVal PivaSuperuser As String,
    '                         ByVal ParolaChiave As String,
    '                         ByVal Piva As String,
    '                         ByVal Sa_cod As Int32,
    '                         ByVal Validita_Inizio As Date,
    '                         ByVal Validita_Fine As Date,
    '                         ByVal xFiltroAggiuntivo As String,
    '                         ByRef objParametri As AgronicaCoreParametri
    '                         ) As Boolean

    '    Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParoleChiavexImprese_W.Modifica()"

    '    Dim messaggioErrore As String = ""
    '    Dim StrSQL As New Text.StringBuilder
    '    Dim xRisp As Boolean = False

    '    Try

    '        StrSQL.Length = 0

    '        'Query per la modifica dei dati                 ' #### CLASSE ####
    '        StrSQL.Append("UPDATE ParoleChiavexImprese SET ")

    '        StrSQL.Append("    PivaSuperuser     = '" & Agro_SQL_SaveText(PivaSuperuser) & "'")
    '        StrSQL.Append("   ,ParolaChiave     = '" & Agro_SQL_SaveText(ParolaChiave) & "'")
    '        StrSQL.Append("   ,Piva     = '" & Agro_SQL_SaveText(Piva) & "'")
    '        StrSQL.Append("   ,sa_cod     = " & Agro_SQL_SaveNum(Sa_cod))

    '        StrSQL.Append("   ,Inviato           =  0 ")
    '        StrSQL.Append("   ,DataInvio         =  Null ")
    '        StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
    '        StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
    '        StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
    '        StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine) & " ")

    '        StrSQL.Append(" WHERE PivaSuperuser = '" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
    '        StrSQL.Append(" AND ParolaChiave = '" & Agro_SQL_SaveText(ParolaChiave) & "' ")
    '        StrSQL.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
    '        StrSQL.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")

    '        '----------------------------------------------------------------------
    '        If xFiltroAggiuntivo <> "" Then
    '            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '        End If

    '        '--------------------------------------------------------------------------
    '        xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, nomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
    '        xRisp = False
    '        Throw New Exception("[" & NomeRoutine & "] : " & messaggioErrore)
    '    End Try

    '    Return xRisp

    'End Function


    Public Function Cancella(ByVal ParolaChiave As String,
                             ByVal Piva As String,
                             ByVal Sa_cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParoleChiavexImprese_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE ParoleChiavexImprese ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  PivaSuperuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
                strSql.Append(" AND Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM     ParoleChiavexImprese ")
                strSql.Append(" WHERE  PivaSuperuser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                strSql.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                strSql.Append(" AND     sa_cod = " & Agro_SQL_SaveNum(Sa_cod) & " ")
                strSql.Append(" AND Inviato = 0")

            End If

            If ParolaChiave <> "" Then
                strSql.Append(" AND ParolaChiave = '" & Agro_SQL_SaveText(ParolaChiave) & "' ")
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

End Class
