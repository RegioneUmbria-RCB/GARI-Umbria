Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate


Public Class ParticelleCatastali_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Leggi(ByVal PROV As String,
                          ByVal COM As String,
                          ByVal SEZIONE As String,
                          ByVal FOGLIO As Int32,
                          ByVal NUMERO As Int32,
                          ByVal SUBALTERNO As String,
                          ByVal Id_Cod As Int32,
                          ByVal Val_Cod As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""                   =>  si leggono tutte le imprese
        '   Sa_Cod = 0
        '   Cod_Contatto = 0
        '   Id_Cod = 0
        '   Val_Cod = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  ParticelleCatastali_Codici.* ")
                    StrSQL.Append(" FROM    ParticelleCatastali_Codici ")
                    StrSQL.Append(" WHERE   (ParticelleCatastali_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (ParticelleCatastali_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND     (ParticelleCatastali_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND     (ParticelleCatastali_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ParticelleCatastali_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ParticelleCatastali_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParticelleCatastali_Codici.Validita_Inizio ASC")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT  ParticelleCatastali_Codici.* ")
                    StrSQL.Append(" FROM    ParticelleCatastali_Codici ")
                    StrSQL.Append(" WHERE   (ParticelleCatastali_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (ParticelleCatastali_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ") ")

                    If PROV <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.PROV = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
                    End If

                    If COM <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.COM = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
                    End If

                    If SEZIONE <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.SEZIONE = '" & Agro_SQL_SaveText(LCase(SEZIONE)) & "' ")
                    End If

                    If FOGLIO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.FOGLIO =  " & Agro_SQL_SaveNum(FOGLIO) & " ")
                    End If

                    If NUMERO <> 0 Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.NUMERO =  " & Agro_SQL_SaveNum(NUMERO) & " ")
                    End If

                    If SUBALTERNO <> "" Then
                        StrSQL.Append(" AND ParticelleCatastali_Codici.SUBALTERNO = '" & Agro_SQL_SaveText(Trim(LCase(SUBALTERNO))) & "' ")
                    End If

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND     (ParticelleCatastali_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ")  ")
                    End If

                    If Trim(Val_Cod) <> "" Then
                        StrSQL.Append(" AND     (ParticelleCatastali_Codici.val_cod like '%" & Agro_SQL_SaveText(Val_Cod) & "%')  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   ParticelleCatastali_Codici.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   ParticelleCatastali_Codici.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ParticelleCatastali_Codici.Validita_Inizio ASC")
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

Public Class ParticelleCatastali_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '==========================================================================================
    Public Function Scrivi(ByVal PROV As String,
                           ByVal COM As String,
                           ByVal SEZIONE As String,
                           ByVal FOGLIO As Int32,
                           ByVal NUMERO As Int32,
                           ByVal SUBALTERNO As String,
                           ByVal Id_Cod As Int32,
                           ByVal Val_Cod As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append("INSERT INTO ParticelleCatastali_Codici(       ")
            strSql.Append("             PROV,           COM,        SEZIONE, ")
            strSql.Append("             FOGLIO,             NUMERO,         SUBALTERNO, ")
            strSql.Append("             Id_Cod,    Val_Cod, ")

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
            strSql.Append("         ," & Agro_SQL_SaveNum(Trim(Id_Cod)) & " ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(Val_Cod) & "'  ")
            strSql.Append("         , " & Agro_SQL_SaveNum(objParametri.FlagVisibilita) & "")
            strSql.Append("         , Null  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
            strSql.Append("         ,'" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
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

    '##############################################################################################
    Public Function Modifica(ByVal PROV As String,
                             ByVal COM As String,
                             ByVal SEZIONE As String,
                             ByVal FOGLIO As Int32,
                             ByVal NUMERO As Int32,
                             ByVal SUBALTERNO As String,
                             ByVal Id_Cod As Int32,
                             ByVal Val_Cod As String,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append("UPDATE ParticelleCatastali_Codici SET ")
            strSql.Append("       Val_Cod             =  '" & Agro_SQL_SaveText(Val_Cod) & "',  ")
            strSql.Append("       Username_Modifica       =  '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "',  ")
            strSql.Append("       Data_Modifica       =  " & Agro_SQL_SaveDate(Date.Now) & " ")
            strSql.Append(" WHERE    PROV        = '" & Agro_SQL_SaveText(PROV) & "' ")
            strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(COM) & "' ")
            strSql.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append(" AND      FOGLIO      = " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append(" AND      Numero      = " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append(" AND      SUBALTERNO =  '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.Append(" AND      Id_cod =  " & Agro_SQL_SaveNum(Id_Cod) & "")

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

    '==========================================================================================
    Public Function Cancella_X_Dati_Valorizzati(ByVal PROV As String,
                                                ByVal COM As String,
                                                ByVal SEZIONE As String,
                                                ByVal FOGLIO As Int32,
                                                ByVal NUMERO As Int32,
                                                ByVal SUBALTERNO_obblig As String,
                                                ByVal Id_Cod_obblig As Int32,
                                                ByVal Val_Cod_obblig As String,
                                                ByVal xFiltroAggiuntivo As String,
                                                ByRef objParametri As AgronicaCoreParametri
                                                ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_W.Cancella_X_Dati_Valorizzati()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            strSql.Length = 0
            strSql.Append(" DELETE ")
            strSql.Append(" FROM    ParticelleCatastali_Codici ")
            strSql.Append(" WHERE   1=1")


            ' La clausola è la stessa per entrambe le query
            If PROV <> "" Then
                strSql.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            End If
            If COM <> "" Then
                strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            End If
            If SEZIONE <> "" Then
                strSql.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            End If
            If FOGLIO <> 0 Then
                strSql.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            End If
            If NUMERO <> 0 Then
                strSql.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            End If

            If SUBALTERNO_obblig = "" Then
                Throw New Exception(" SUBALTERNO_obblig ")
            End If

            If Id_Cod_obblig = 0 Then
                Throw New Exception(" Id_Cod_obblig ")
            End If

            If Val_Cod_obblig = "" Then
                Throw New Exception(" Val_Cod_obblig ")
            End If

            strSql.Append(" AND      SUBALTERNO =  '" & If(Agro_SQL_SaveText(SUBALTERNO_obblig, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO_obblig)), 0) & "' ")
            strSql.Append(" AND      Id_Cod      = " & Agro_SQL_SaveNum(Id_Cod_obblig) & " ")
            strSql.Append(" AND      Val_Cod         = '" & Agro_SQL_SaveText(Trim(Val_Cod_obblig)) & "' ")

            '---------------------------------------------
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
                             ByVal Id_Cod As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE ParticelleCatastali_Codici ")
                strSql.Append(" SET ")
                strSql.Append("          Validita_Fine = " & Agro_SQL_SaveDate(AGRODATAINIZIO) & " ")
                strSql.Append("         ,Username_Modifica = '" & Agro_SQL_SaveText(objParametri.LogDescrizioneUtente) & "' ")
                strSql.Append("         ,Inviato = -1 ")
                strSql.Append(" WHERE   Inviato >= 0")

            Else

                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM    ParticelleCatastali_Codici ")
                strSql.Append(" WHERE   1=1")

            End If

            ' La clausola è la stessa per entrambe le query
            strSql.Append(" AND      PROV        = '" & Agro_SQL_SaveText(Trim(PROV)) & "' ")
            strSql.Append(" AND      COM         = '" & Agro_SQL_SaveText(Trim(COM)) & "' ")
            strSql.Append(" AND      Sezione     = '" & If(Agro_SQL_SaveText(SEZIONE, False) <> "", Agro_SQL_SaveText(LCase(SEZIONE)), 0) & "'  ")
            strSql.Append(" AND      FOGLIO      =  " & If(Agro_SQL_SaveNum(FOGLIO, False) <> 0, Agro_SQL_SaveNum(FOGLIO), 0) & "  ")
            strSql.Append(" AND      Numero      =  " & If(Agro_SQL_SaveNum(NUMERO, False) <> 0, Agro_SQL_SaveNum(NUMERO), 0) & "  ")
            strSql.Append(" AND      SUBALTERNO =  '" & If(Agro_SQL_SaveText(SUBALTERNO, False) <> "", Agro_SQL_SaveText(LCase(SUBALTERNO)), 0) & "' ")
            strSql.Append(" AND      Id_Cod      = " & Agro_SQL_SaveNum(Id_Cod) & " ")

            '---------------------------------------------
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


    Public Function Aggiungi_Aggiorna(ByVal PROV As String,
                                      ByVal COM As String,
                                      ByVal SEZIONE As String,
                                      ByVal FOGLIO As Int32,
                                      ByVal NUMERO As Int32,
                                      ByVal SUBALTERNO As String,
                                      ByVal Id_Cod As Int32,
                                      ByVal Val_Cod As String,
                                      ByVal Validita_Inizio As Date,
                                      ByVal Validita_Fine As Date,
                                      ByVal filtro As String,
                                      ByRef objParametri As AgronicaCoreParametri
                                      ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_W.Aggiungi_Aggiorna()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False

        Try

            Dim objParticelleCatastaliCodiciR As New AgronicaCoreAnagrafeDAL.ParticelleCatastali_Codici_R

            Dim dtPC As DataTable = objParticelleCatastaliCodiciR.Leggi(PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,
                                               Id_Cod,
                                               Val_Cod,
                                               enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                                               filtro, "", objParametri)

            If dtPC.Rows.Count = 1 Then

                Modifica(PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,
                         Id_Cod,
                         Val_Cod,
                         filtro,
                         objParametri)

            ElseIf dtPC.Rows.Count = 0 Then

                Scrivi(PROV, COM, SEZIONE, FOGLIO, NUMERO, SUBALTERNO,
                       Id_Cod,
                       Val_Cod,
                       Validita_Inizio,
                       Validita_Fine,
                       objParametri)

            Else
                Throw New Exception("La query deve selezionare al massimo un solo record")
            End If

            xRisp = True

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            xRisp = False
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
