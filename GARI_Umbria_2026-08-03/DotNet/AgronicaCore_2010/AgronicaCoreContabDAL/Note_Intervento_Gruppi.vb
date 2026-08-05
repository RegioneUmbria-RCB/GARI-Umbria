Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Note_Intervento_Gruppi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '###################################################################################
    Public Function NuovoId_NoteInterventoGruppi(ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_R.NuovoId_NoteInterventoGruppi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim notaGruppoCod As Integer = -1

        Try

            StrSQL.Append(" SELECT ")
            StrSQL.Append(" ISNULL ( ")
            StrSQL.Append("             (CASE(MAX(NotaGruppo_Cod) + 1 ) ")
            StrSQL.Append("                 WHEN 0 THEN 1  ")
            StrSQL.Append("                 ELSE MAX(NotaGruppo_Cod) +1 END   ")
            StrSQL.Append("             )  ")
            StrSQL.Append("         , 1) AS COD  ")
            StrSQL.Append(" FROM Note_Intervento_Gruppi ")

            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)

            notaGruppoCod = dt.Rows(0).Item("Cod")

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return notaGruppoCod

    End Function

    '============================================================================
    Public Function Leggi(ByVal NotaGruppo_Cod As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   NotaGruppo_Cod = 0    
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  ")
                    StrSQL.Append("          [PivaSuperUser] ")
                    StrSQL.Append("         ,[NotaGruppo_Cod] ")
                    StrSQL.Append("         ,[NotaGruppo_Des] ")
                    StrSQL.Append("         ,[visibile] ")
                    StrSQL.Append(" FROM  Note_Intervento_Gruppi ")

                    StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If NotaGruppo_Cod <> 0 Then
                        StrSQL.Append(" AND NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & " ")
                    End If


                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_NoteGruppi rr where rr.From_PivaSuperUser = Note_Intervento_Gruppi.pivaSuperUser and  rr.From_NotaGruppo_Cod = Note_Intervento_Gruppi.NotaGruppo_Cod ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati

                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Note_Intervento_Gruppi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Note_Intervento_Gruppi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY NotaGruppo_Des ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Note_Intervento_Gruppi ")

                    StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND   Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If NotaGruppo_Cod <> 0 Then
                        StrSQL.Append(" AND NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_NoteGruppi rr where rr.From_PivaSuperUser = Note_Intervento_Gruppi.pivaSuperUser and  rr.From_NotaGruppo_Cod = Note_Intervento_Gruppi.NotaGruppo_Cod ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Note_Intervento_Gruppi.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Note_Intervento_Gruppi.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY NotaGruppo_Des ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                Case enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

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
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Note_Intervento_Gruppi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Note_Intervento_Grupp_MarcaComeInviato(ByVal NotaGruppo_Cod As Int32,
                                                           ByVal Data_invio As DateTime,
                                                           ByVal xFiltroAggiuntivo As String,
                                                           ByRef objParametri As AgronicaCoreParametri
                                                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_W.Note_Intervento_Grupp_MarcaComeInviato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If NotaGruppo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Note_Intervento_Gruppi SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,data_invio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PivaSuperUser        = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" AND     NotaGruppo_Cod   = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")

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

    '============================================================================
    Public Function Scrivi(ByVal NotaGruppo_Cod As Integer,
                           ByVal NotaGruppo_Des As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal auto_increment_NotaGruppo_Cod As Boolean,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaGruppo_Cod = 0 AndAlso Not auto_increment_NotaGruppo_Cod Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            Dim newId As Integer

            If auto_increment_NotaGruppo_Cod Then
                Dim objNoteGruppi As New AgronicaCoreContabDAL.Note_Intervento_Gruppi_R
                newId = objNoteGruppi.NuovoId_NoteInterventoGruppi(objParametri)
                'StrSQL.Length = 0
                'Dim DT As DataTable
                'StrSQL.Append("( SELECT (CASE(MAX(NotaGruppo_Cod)+1) WHEN 0 THEN 1 ELSE MAX(NotaGruppo_Cod)+1 END) as COD  FROM Note_Intervento_Gruppi )")
                'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                'newID = DT.Rows(0).Item("Cod")
            End If

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Note_Intervento_Gruppi (")
            StrSQL.Append("               [PivaSuperUser]  ,[NotaGruppo_Cod]   ,[NotaGruppo_Des],  ")
            StrSQL.Append("               Inviato,            DataInvio, ")
            StrSQL.Append("               Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("               UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("               Validita_Inizio,    Validita_Fine, Visibile ")
            StrSQL.Append("               ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If auto_increment_NotaGruppo_Cod Then
                StrSQL.Append(", " & newId)
            Else
                StrSQL.Append(", " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")
            End If

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(NotaGruppo_Des) & "'  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , 1  ")
            StrSQL.Append(") ")

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

    '============================================================================
    Public Function Modifica(ByVal NotaGruppo_Cod As Integer,
                             ByVal NotaGruppo_Des As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal Visibile As Int16,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaGruppo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####

            StrSQL.Append(" UPDATE Note_Intervento_Gruppi SET ")
            StrSQL.Append("     NotaGruppo_Des       = '" & Agro_SQL_SaveText(NotaGruppo_Des) & "'  ")

            StrSQL.Append("   ,Inviato              =  0 ")
            StrSQL.Append("   ,DataInvio            =  Null ")
            StrSQL.Append("   ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))
            StrSQL.Append("   ,Visibile             =  " & Agro_SQL_SaveNum(Visibile))

            StrSQL.Append(" WHERE   PivaSuperUser   = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND     NotaGruppo_Cod  =  " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")

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


    Public Function ModificaVisibile(ByVal NotaGruppo_Cod As Integer,
                                     ByVal visibile As Boolean,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_W.ModificaVisibile()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaGruppo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####

            StrSQL.Append(" UPDATE Note_Intervento_Gruppi SET ")

            StrSQL.Append("   Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Visibile             =  " & Agro_SQL_SaveBoolStrToInt(visibile))

            StrSQL.Append(" WHERE   PivaSuperUser   = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND     NotaGruppo_Cod  =  " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")

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


    '============================================================================
    Public Function Cancella(ByVal NotaGruppo_Cod As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Gruppi_R.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaGruppo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Note_Intervento_Gruppi ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Note_Intervento_Gruppi ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            StrSQL.Append(" AND PivaSuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND NotaGruppo_Cod   =  " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "   ")

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
