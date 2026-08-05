Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Impresa_Progetto_Fasi_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Piva As String,
                          ByVal Progetto_Cod As Integer,
                          ByVal Fase_Cod As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    strSql.Length = 0
                    strSql.Append(" SELECT * ")
                    strSql.Append(" FROM  Imprese_Progetto_Fasi ")
                    strSql.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    End If
                    If Progetto_Cod <> 0 Then
                        strSql.Append(" AND     Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.Append(" AND     Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
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

                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Piva ASC, Progetto_Cod ASC, Gru_Op Desc, Lav_Cod Desc")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    strSql.Length = 0
                    strSql.Append(" SELECT * ")
                    strSql.Append(" FROM  Imprese_Progetto_Fasi ")
                    strSql.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    strSql.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Piva <> "" Then
                        strSql.Append(" AND     Piva = '" & Agro_SQL_SaveText(Piva).Trim & "' ")
                    End If

                    If Progetto_Cod <> 0 Then
                        strSql.Append(" AND     Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
                    End If

                    If Fase_Cod <> 0 Then
                        strSql.Append(" AND     Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
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
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Piva ASC, Progetto_Cod ASC, Gru_Op Desc, Lav_Cod Desc")
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

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Impresa_Progetto_Fasi_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Scrivi(ByVal Piva As String,
                           ByVal Progetto_Cod As Integer,
                           ByVal Fase_Cod As Integer,
                           ByVal Fase_Des As String,
                           ByVal Data_Inizio_Prevista As Date,
                           ByVal Data_Fine_Prevista As Date,
                           ByVal Giudizio As String,
                           ByVal Budget As Decimal,
                           ByVal Lav_Cod As Integer,
                           ByVal Gru_Op As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As DateTime = #2/1/1900#,
                           Optional ByVal Data_modifica As DateTime = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_W.Scrivi()"

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
            strSql.Append(" INSERT INTO Imprese_Progetto_Fasi ")
            strSql.Append("                    ( Piva,        Progetto_Cod,         Fase_Cod, ")
            strSql.Append("                      Fase_Des,    Data_Inizio_Prevista, Data_Fine_Prevista, ")
            strSql.Append("                      Giudizio,    Lav_Cod,              Gru_Op, ")
            strSql.Append("                      Budget, ")
            strSql.Append("                      Inviato,            DataInvio, ")
            strSql.Append("                      Data_Creazione,     Data_Modifica, ")
            strSql.Append("                      UserName_Creazione, UserName_Modifica, ")
            strSql.Append("                      Validita_Inizio,    Validita_Fine ")
            strSql.Append("                     ) ")
            strSql.Append(" VALUES (")
            strSql.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Progetto_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Fase_Cod) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Fase_Des) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Giudizio) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Gru_Op) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Budget) & "  ")
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            strSql.Append(")")

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
    Public Function Modifica(ByVal Piva As String,
                             ByVal Progetto_Cod As Integer,
                             ByVal Fase_Cod As Integer,
                             ByVal Fase_Des As String,
                             ByVal Data_Inizio_Prevista As Date,
                             ByVal Data_Fine_Prevista As Date,
                             ByVal Giudizio As String,
                             ByVal Budget As Decimal,
                             ByVal Lav_Cod As Integer,
                             ByVal Gru_Op As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            If Progetto_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Progetto_Cod obbligatorio)")
            End If

            If Fase_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Fase_Cod obbligatorio)")
            End If

            strSql.Length = 0
            strSql.Append(" UPDATE Imprese_Progetto_Fasi SET ")
            strSql.Append("    Fase_Des              = '" & Agro_SQL_SaveText(Fase_Des) & "'  ")
            strSql.Append("   ,Data_Inizio_Prevista  =  " & Agro_SQL_SaveDate(Data_Inizio_Prevista) & "  ")
            strSql.Append("   ,Data_Fine_Prevista    =  " & Agro_SQL_SaveDate(Data_Fine_Prevista) & "  ")
            strSql.Append("   ,Giudizio              = '" & Agro_SQL_SaveText(Giudizio) & "'  ")
            strSql.Append("   ,Budget                =  " & Agro_SQL_SaveNum(Budget) & "  ")
            strSql.Append("   ,Lav_Cod               =  " & Agro_SQL_SaveNum(Lav_Cod) & "  ")
            strSql.Append("   ,Gru_Op                =  " & Agro_SQL_SaveNum(Gru_Op) & "  ")
            strSql.Append("   ,Inviato           = 0 ")
            strSql.Append("   ,DataInvio         = Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Now) & "  ")
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'  ")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE Progetto_Cod  =  " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            strSql.Append(" AND   Fase_Cod      =  " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
            
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
    
    '##############################################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Progetto_Cod As Integer,
                             ByVal Fase_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.Impresa_Progetto_Fasi_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE Imprese_Progetto_Fasi ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM  Imprese_Progetto_Fasi ")
                strSql.Append(" WHERE Inviato = 0  ")

            End If

            If Piva <> "" Then
                strSql.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'   ")
            End If

            If Progetto_Cod <> 0 Then
                strSql.Append(" AND Progetto_Cod = " & Agro_SQL_SaveNum(Progetto_Cod) & "   ")
            End If

            If Fase_Cod <> 0 Then
                strSql.Append(" AND Fase_Cod = " & Agro_SQL_SaveNum(Fase_Cod) & "   ")
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
