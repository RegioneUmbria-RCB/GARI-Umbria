Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class UtentixAppezzamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Campo_Cod As Int32,
                          ByVal Appezza As Int32,
                          ByVal SoloNonBloccati As Boolean,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT *  ")
                    StrSQL.Append(" FROM  UtentixAppezzamenti , Appezzamento ")
                    StrSQL.Append(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UtentixAppezzamenti.PIVA = Appezzamento.PIVA ")
                    StrSQL.Append(" AND   UtentixAppezzamenti.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   UtentixAppezzamenti.Appezza = Appezzamento.Appezza ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND UtentixAppezzamenti.[User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND UtentixAppezzamenti.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND UtentixAppezzamenti.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND UtentixAppezzamenti.Appezza =  " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Appezzamento.Campo_Cod =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If

                    If SoloNonBloccati Then
                        StrSQL.Append(" AND Appezzamento.Blk_Flag <> -1 ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_appezzamenti rr where rr.From_Piva = Appezzamento.piva and rr.From_Sa_cod = Appezzamento.Sa_cod and rr.From_appezza = Appezzamento.Appezza ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.Append("and exists ( " & vbCrLf)
                            StrSQL.Append("    select 1 " & vbCrLf)
                            StrSQL.Append("    from g2g_recode_appezzamenti rr " & vbCrLf)
                            StrSQL.Append("    where rr.DataInvio < Appezzamento.Data_Modifica " & vbCrLf)
                            StrSQL.Append("    and rr.From_Piva = Appezzamento.piva  " & vbCrLf)
                            StrSQL.Append("    and rr.From_Sa_cod = Appezzamento.Sa_cod  " & vbCrLf)
                            StrSQL.Append("    and rr.From_Appezza = Appezzamento.Appezza " & vbCrLf)
                            StrSQL.Append(" ) " & vbCrLf)

                    End Select

                    'Non è indispensabile, ma è preferibile questo ordinamento

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   UtentixAppezzamenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   UtentixAppezzamenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Appezzamento.Validita_Fine Desc ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '---------------------------------------------
                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ,Appezzamento.username_creazione as Appezzamento_username_creazione,Appezzamento.Username_modifica as Appezzamento_Username_modifica,Appezzamento.data_creazione as Appezzamento_data_creazione,Appezzamento.data_modifica as Appezzamento_data_modifica ")
                    StrSQL.Append(" FROM  UtentixAppezzamenti , Appezzamento ")
                    StrSQL.Append(" WHERE Appezzamento.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Appezzamento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UtentixAppezzamenti.PIVA = Appezzamento.PIVA ")
                    StrSQL.Append(" AND   UtentixAppezzamenti.Sa_Cod = Appezzamento.Sa_Cod ")
                    StrSQL.Append(" AND   UtentixAppezzamenti.Appezza = Appezzamento.Appezza ")

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND UtentixAppezzamenti.[User] = '" & Replace(objParametri.PivaSuperUser, "'", "''") & "' ")
                    End If

                    If Piva <> "" Then
                        StrSQL.Append(" AND UtentixAppezzamenti.PIVA = '" & Agro_SQL_SaveText(Trim(Piva)) & "'")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND UtentixAppezzamenti.Sa_Cod =  " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                    End If

                    If Appezza <> 0 Then
                        StrSQL.Append(" AND UtentixAppezzamenti.Appezza =  " & Agro_SQL_SaveNum(Appezza) & " ")
                    End If

                    If Campo_Cod <> 0 Then
                        StrSQL.Append(" AND Appezzamento.Campo_Cod =  " & Agro_SQL_SaveNum(Campo_Cod) & " ")
                    End If

                    If SoloNonBloccati Then
                        StrSQL.Append(" AND Appezzamento.Blk_Flag <> -1 ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_appezzamenti rr where rr.From_Piva = Appezzamento.piva and rr.From_Sa_cod = Appezzamento.Sa_cod and rr.From_appezza = Appezzamento.Appezza ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati
                            StrSQL.Append("and exists ( " & vbCrLf)
                            StrSQL.Append("    select 1 " & vbCrLf)
                            StrSQL.Append("    from g2g_recode_appezzamenti rr " & vbCrLf)
                            StrSQL.Append("    where rr.DataInvio < Appezzamento.Data_Modifica " & vbCrLf)
                            StrSQL.Append("    and rr.From_Piva = Appezzamento.piva  " & vbCrLf)
                            StrSQL.Append("    and rr.From_Sa_cod = Appezzamento.Sa_cod  " & vbCrLf)
                            StrSQL.Append("    and rr.From_Appezza = Appezzamento.Appezza " & vbCrLf)
                            StrSQL.Append(" ) " & vbCrLf)

                    End Select

                    'Non è indispensabile, ma è preferibile questo ordinamento

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   UtentixAppezzamenti.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   UtentixAppezzamenti.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Appezzamento.Validita_Fine Desc ")
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



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class UtentixAppezzamenti_W
    Inherits AgronicaCoreDataProvider.DataProvider



    ''##############################################################################################
    Public Function Scrivi(ByVal PivaSuperuser As String,
                           ByVal Piva As String,
                           ByVal Sa_Cod As Integer,
                           ByVal Appezza As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
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
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO UtentixAppezzamenti(")
            StrSQL.Append("                    [User],           ")
            StrSQL.Append("                    PIVA,           ")
            StrSQL.Append("                    Sa_Cod,         ")
            StrSQL.Append("                    Appezza,         ")
            StrSQL.Append("                    Inviato,            DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Appezza) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("		    , " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("		    ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

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

    '##############################################################################################
    Public Function Modifica(ByVal PivaSuperuser As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Appezza As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If PivaSuperuser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperuser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Sa_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            End If

            If Appezza = 0 Then
                Throw New Exception("Parametro non corretto nella query (Appezza obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE UtentixAppezzamenti SET ")
            StrSQL.Append("        Inviato           =  0 ")
            StrSQL.Append("       ,DataInvio         =  Null ")
            StrSQL.Append("       ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now))
            StrSQL.Append("       ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("       ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("       ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE    [User] = '" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
            StrSQL.Append(" AND      Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND      Sa_Cod = " & Sa_Cod & "  ")
            StrSQL.Append(" AND      Appezza = " & Appezza & "  ")


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

    '##############################################################################################
    Public Function Cancella(ByVal PivaSuperuser As String,
                             ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Appezza As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si cancellano tutti gli UtentixAppezzamenti dell'impresa
        '   Appezza = 0          =>  si cancellano tutti gli UtentixAppezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperuser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperuser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE UtentixAppezzamenti ")
                StrSQL.Append(" SET ")
                StrSQL.Append("       Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND [User] = '" & Agro_SQL_SaveText(PivaSuperuser) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     UtentixAppezzamenti ")
                StrSQL.Append(" WHERE    Piva= '" & Agro_SQL_SaveText(Piva) & "' ")
                StrSQL.Append(" AND [User] = '" & Agro_SQL_SaveText(PivaSuperuser) & "' ")

            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")

            End If
            '---------------------------------------------

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

    '##############################################################################################
    Public Function Aggiorna_Validazione(ByVal Validazione As Int32,
                                         ByVal Piva As String,
                                         ByVal PivaSuperuser As String,
                                         ByVal Sa_Cod As Int32,
                                         ByVal Appezza As Int32,
                                         ByVal Data_Validazione As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W.Aggiorna_Validazione()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  vengono aggiornati tutti gli UtentixAppezzamenti dell'impresa
        '   Appezza = 0          =>  vengono aggiornati tutti gli UtentixAppezzamenti del centro aziendale
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If PivaSuperuser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperuser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE UtentixAppezzamenti SET ")
            StrSQL.Append("    Validazione          = " & Agro_SQL_SaveNum(Validazione))
            StrSQL.Append("   ,Username_Validazione = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Data_Validazione        = " & Agro_SQL_SaveDate(Data_Validazione))
            StrSQL.Append(" WHERE Piva='" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.Append(" AND [User] = '" & Agro_SQL_SaveText(PivaSuperuser) & "' ")

            If (Sa_Cod <> 0) Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            End If

            If (Appezza <> 0) Then
                StrSQL.Append(" AND Appezza = " & Agro_SQL_SaveNum(Appezza) & "  ")

            End If
            '---------------------------------------------

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
        Finally
            StrSQL = Nothing
        End Try

        Return xRisp

    End Function


    '###################################################################################################################################
    Public Function AggiornaValiditaInizio(ByVal Piva As String,
                                           ByVal Sa_Cod As Int32,
                                           ByVal Id_Campo As Int32,
                                           ByVal Appezza As Int32,
                                           ByVal Validita_Inizio As Date,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W.AggiornaValiditaInizio()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si aggiornano tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si aggiornano tutti gli impianti del centro aziendale
        '   Id_Campo = 0         =>  si aggiornano tutti gli impianti di tutti i campi del centro
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperuser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE UtentixAppezzamenti SET ")
            StrSQL.Append("              UtentixAppezzamenti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,UtentixAppezzamenti.Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append(" WHERE UtentixAppezzamenti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND   UtentixAppezzamenti.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   UtentixAppezzamenti.Validita_Inizio < " & Agro_SQL_SaveDate(Validita_Inizio))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   UtentixAppezzamenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND  UtentixAppezzamenti.Appezza IN ( Select Appezzamento.Appezza From Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   UtentixAppezzamenti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            '---------------------------------------------

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

    '###################################################################################################################################
    Public Function AggiornaValiditaFine(ByVal Piva As String,
                                         ByVal Sa_Cod As Int32,
                                         ByVal Id_Campo As Int32,
                                         ByVal Appezza As Int32,
                                         ByVal Validita_Fine As Date,
                                         ByVal xFiltroAggiuntivo As String,
                                         ByRef objParametri As AgronicaCoreParametri
                                         ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.UtentixAppezzamenti_W.AggiornaValiditaFine()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0           =>  si aggiornano tutti gli impianti dell'impresa
        '   Appezza = 0          =>  si aggiornano tutti gli impianti del centro aziendale
        '   Id_Campo = 0         =>  si aggiornano tutti gli impianti di tutti i campi del centro
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperuser obbligatorio)")
            End If

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("UPDATE UtentixAppezzamenti SET ")
            StrSQL.Append("              UtentixAppezzamenti.UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("             ,UtentixAppezzamenti.Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append(" WHERE UtentixAppezzamenti.Piva = '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append(" AND   UtentixAppezzamenti.[USER] = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   UtentixAppezzamenti.Validita_Inizio > " & Agro_SQL_SaveDate(Validita_Fine))

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND   UtentixAppezzamenti.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
            End If

            If Id_Campo <> 0 Then
                StrSQL.Append(" AND  UtentixAppezzamenti.Appezza IN ( Select Appezzamento.Appezza From Appezzamento " &
                                                                 " Where Campo_Cod = " & Agro_SQL_SaveNum(Id_Campo) &
                                                                 " And   Piva      = '" & Agro_SQL_SaveText(Trim(Piva)) & "' " &
                                                                 " And   Sa_Cod    = " & Agro_SQL_SaveNum(Sa_Cod) & ") ")
            End If

            If Appezza <> 0 Then
                StrSQL.Append(" AND   UtentixAppezzamenti.Appezza = " & Agro_SQL_SaveNum(Appezza) & " ")
            End If
            '---------------------------------------------

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
