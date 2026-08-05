Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Note_Intervento_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '###################################################################################
    Public Function NuovoId_NoteIntervento(ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_R.NuovoId_NoteIntervento()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim notaGruppoCod As Integer = -1

        Try

            ' StrSQL.Append("( SELECT (CASE(MAX(Nota_Cod)+1) WHEN 0 THEN 1 ELSE MAX(Nota_Cod)+1 END) as COD  FROM Note_Intervento )")

            StrSQL.Append(" SELECT ")
            StrSQL.Append(" ISNULL ( ")
            StrSQL.Append("             (CASE(MAX(Nota_Cod) + 1 ) ")
            StrSQL.Append("                 WHEN 0 THEN 1  ")
            StrSQL.Append("                 ELSE MAX(Nota_Cod) +1 END   ")
            StrSQL.Append("             )  ")
            StrSQL.Append("         , 1) AS COD  ")
            StrSQL.Append(" FROM Note_Intervento ")

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

    '###################################################################################
    Public Function Esiste_NotaCod(ByVal Nota_Cod As Integer,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_R.Esiste_NotaCod()"

        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim flagEsiste As Boolean = False

        Try

            dt = Leggi(Nota_Cod, 0,
                       enumSelezioneVariabile.Selezione_TabellaDatiMinimi,
                       "", "", objParametri)

            If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                flagEsiste = True
            End If

            dt = Nothing

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return flagEsiste

    End Function

    '============================================================================
    Public Function Leggi(ByVal Nota_Cod As Int32,
                          ByVal NotaGruppo_Cod As Int32,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Nota_Cod = 0 
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
                    StrSQL.Append("         ,[Nota_Cod] ")
                    StrSQL.Append("         ,[Nota_Des] ")
                    StrSQL.Append("         ,[NotaGruppo_Cod] ")
                    StrSQL.Append("         ,[visibile] ")
                    StrSQL.Append(" FROM  Note_Intervento ")

                    StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If NotaGruppo_Cod <> 0 Then
                        StrSQL.Append(" AND NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & " ")
                    End If

                    If Nota_Cod <> 0 Then
                        StrSQL.Append(" AND Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_NoteIntervento rr where rr.From_PivaSuperUser = Note_Intervento.pivaSuperUser and  rr.From_Nota_Cod = Note_Intervento.Nota_Cod ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati

                    End Select


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Note_Intervento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Note_Intervento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY NotaGruppo_Cod, Nota_Des ASC ")
                    End If

                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Note_Intervento ")
                    StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
                    StrSQL.Append(" AND   Validita_Inizio >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
                    StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

                    If NotaGruppo_Cod <> 0 Then
                        StrSQL.Append(" AND NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & " ")
                    End If

                    If Nota_Cod <> 0 Then
                        StrSQL.Append(" AND Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & " ")
                    End If

                    Select Case TipoG2G
                        Case 1 'seleziona i nuovi dati.
                            StrSQL.Append(" AND not exists ( ")
                            StrSQL.Append(" select 1 from g2g_recode_NoteIntervento rr where rr.From_PivaSuperUser = Note_Intervento.pivaSuperUser and  rr.From_Nota_Cod = Note_Intervento.Nota_Cod ")
                            StrSQL.Append(" ) ")

                        Case 2 'seleziona i dati modificati

                    End Select

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Note_Intervento.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Note_Intervento.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY NotaGruppo_Cod, Nota_Des ASC ")
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

    ''' <param name="Nota_Cod">Opzionale, indicare 0 per evitre il filtro.</param>
    ''' <param name="NotaGruppo_Cod">Opzionale, indicare 0 per evitre il filtro.</param>
    ''' <param name="NotaUtilizzo_Cod">Opzionale, indicare 0 per evitre il filtro.</param>
    Public Function Leggi_con_Utilizzo(ByVal Nota_Cod As Int32,
                                       ByVal NotaGruppo_Cod As Int32,
                                       ByVal NotaUtilizzo_Cod As Int32,
                                       ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                       ByVal xFiltroAggiuntivo As String,
                                       ByVal xOrderBy As String,
                                       ByRef objParametri As AgronicaCoreParametri,
                                       Optional ByVal meno1tutti_0nonVisibili_1soloVisibili As Integer = -1
                                       ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_R.Leggi_con_Utilizzo()"
        Dim StrSQL As New Text.StringBuilder With {.Length = 0}
        Dim dt As DataTable

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            '------------------------------------------------------------------
            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.AppendLine(" SELECT  ")
                    StrSQL.AppendLine("          Note_Intervento.[PivaSuperUser] ")
                    StrSQL.AppendLine("         ,Note_Intervento.[Nota_Cod] ")
                    StrSQL.AppendLine("         ,Note_Intervento.[Nota_Des] ")
                    StrSQL.AppendLine("         ,Note_Intervento.[NotaGruppo_Cod] ")
                    StrSQL.AppendLine("         ,Note_Intervento_Gruppi.[NotaGruppo_Des] ")
                    StrSQL.AppendLine("         ,Note_Intervento_Gruppi.[Tipo_Gruppo_Note] ")

                    StrSQL.AppendLine(" FROM  Note_Intervento WITH(NOLOCK)")

                    StrSQL.AppendLine(" INNER Join  Note_Intervento_Gruppi WITH(NOLOCK)")
                    StrSQL.AppendLine(" ON Note_Intervento.PivaSuperUser = Note_Intervento_Gruppi.PivaSuperUser  ")
                    StrSQL.AppendLine(" And Note_Intervento.NotaGruppo_Cod = Note_Intervento_Gruppi.NotaGruppo_Cod ")
                    StrSQL.AppendLine(" INNER Join Note_Intervento_UtilizzoxGruppi WITH(NOLOCK)")
                    StrSQL.AppendLine(" ON Note_Intervento_Gruppi.PivaSuperUser = Note_Intervento_UtilizzoxGruppi.PivaSuperUser  ")
                    StrSQL.AppendLine(" And Note_Intervento_Gruppi.NotaGruppo_Cod = Note_Intervento_UtilizzoxGruppi.NotaGruppo_Cod ")

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.AppendLine(" Select  ")
                    StrSQL.AppendLine("          Note_Intervento.[PivaSuperUser] ")
                    StrSQL.AppendLine("         , Note_Intervento.[Nota_Cod] ")
                    StrSQL.AppendLine("         , Note_Intervento.[Nota_Des] ")
                    StrSQL.AppendLine("         , Note_Intervento.[Visibile] As Nota_Visibile")
                    StrSQL.AppendLine("         , Note_Intervento.[NotaGruppo_Cod] ")
                    StrSQL.AppendLine("         , Note_Intervento_Gruppi.[NotaGruppo_Des] ")
                    StrSQL.AppendLine("         , Note_Intervento_Gruppi.[Tipo_Gruppo_Note] ")
                    StrSQL.AppendLine("         , Note_Intervento_Gruppi.[Visibile] as Gruppo_Visibile")

                    StrSQL.AppendLine(" FROM  Note_Intervento ")

                    StrSQL.AppendLine(" INNER Join Note_Intervento_Gruppi WITH(NOLOCK)")
                    StrSQL.AppendLine(" ON Note_Intervento.PivaSuperUser = Note_Intervento_Gruppi.PivaSuperUser  ")
                    StrSQL.AppendLine(" And Note_Intervento.NotaGruppo_Cod = Note_Intervento_Gruppi.NotaGruppo_Cod  ")
                    StrSQL.AppendLine(" INNER JOIN Note_Intervento_UtilizzoxGruppi WITH(NOLOCK) ")
                    StrSQL.AppendLine(" ON Note_Intervento_Gruppi.PivaSuperUser = Note_Intervento_UtilizzoxGruppi.PivaSuperUser  ")
                    StrSQL.AppendLine(" And Note_Intervento_Gruppi.NotaGruppo_Cod = Note_Intervento_UtilizzoxGruppi.NotaGruppo_Cod ")

                Case Else
                    Throw New NotImplementedException

            End Select

            StrSQL.AppendLine(" WHERE Note_Intervento.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                    StrSQL.AppendLine(" AND   Note_Intervento.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.AppendLine(" AND   Note_Intervento.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

            If Nota_Cod <> 0 Then
                StrSQL.AppendLine(" AND Note_Intervento.Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & " ")
            End If

            If NotaGruppo_Cod <> 0 Then
                StrSQL.AppendLine(" AND Note_Intervento.NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod) & " ")
            End If

            If NotaUtilizzo_Cod <> 0 Then
                StrSQL.AppendLine(" AND NotaUtilizzo_Cod = " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & " ")
            End If

            'visibilità singole note
            If (meno1tutti_0nonVisibili_1soloVisibili >= 0) Then
                StrSQL.AppendLine(" AND   Note_Intervento.Visibile = " & Agro_SQL_SaveNum(meno1tutti_0nonVisibili_1soloVisibili) & "  ")
                'visibilità gruppo
                StrSQL.AppendLine(" AND   Note_Intervento_Gruppi.Visibile = " & Agro_SQL_SaveNum(meno1tutti_0nonVisibili_1soloVisibili) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.AppendLine(" AND   Note_Intervento.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.AppendLine(" AND   Note_Intervento.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Note_Intervento.NotaGruppo_Cod, Note_Intervento.Nota_Des ASC ")
            End If

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------
        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try
        Return dt
    End Function


    '============================================================================
    Public Function NotaDesFromNotaCod(ByVal Nota_Cod As Int32,
                                       ByRef objParametri As AgronicaCoreParametri
                                       ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_R.NotaDesFromNotaCod()"

        '====================================================================================
        'Parametri opzionali :
        '   Nota_Cod = 0 
        '   NotaGruppo_Cod = 0    
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  ")
            StrSQL.Append("        [Nota_Des] ")

            StrSQL.Append(" FROM  Note_Intervento ")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")


            If Nota_Cod <> 0 Then
                StrSQL.Append(" AND Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & " ")
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

        If dt.Rows.Count > 0 Then
            Return dt.Rows(0).Item("Nota_Des")
        Else
            Return ""
        End If

    End Function

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§


Public Class Note_Intervento_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Note_Intervento_MarcaComeInviato(ByVal Nota_Cod As Int32,
                                                     ByVal Data_invio As DateTime,
                                                     ByVal xFiltroAggiuntivo As String,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.NoteIntervento_W.Note_Intervento_MarcaComeInviato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Nota_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Note_Intervento SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,data_invio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PivaSuperUser        = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" AND     Nota_Cod   = " & Agro_SQL_SaveNum(Nota_Cod) & "  ")

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
    Public Function Scrivi(ByVal Nota_Cod As Integer,
                           ByVal Nota_Des As String,
                           ByVal NotaGruppo_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal auto_increment_notaCod As Boolean,
                           ByVal Note_Valore_Numerico As Decimal,
                           ByVal Note_Valore_Stringa As String,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Nota_Cod = 0 AndAlso Not auto_increment_notaCod Then
                Throw New Exception("Parametro non corretto nella query (Nota_Cod obbligatorio)")
            End If

            '---------------------------------------------
            Dim newId As Integer
            If auto_increment_notaCod Then

                Dim objNoteGruppi As New AgronicaCoreContabDAL.Note_Intervento_R
                newId = objNoteGruppi.NuovoId_NoteIntervento(objParametri)

                'Dim DT As DataTable
                'StrSQL.Append("( SELECT (CASE(MAX(Nota_Cod)+1) WHEN 0 THEN 1 ELSE MAX(Nota_Cod)+1 END) as COD  FROM Note_Intervento )")
                'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                'Dim newID As Integer = DT.Rows(0).Item("Cod")
            End If

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Note_Intervento (")
            StrSQL.Append("               [PivaSuperUser]  ,[Nota_Cod]   ,[Nota_Des]   ,[NotaGruppo_Cod], ")
            StrSQL.Append("               Inviato,            DataInvio, ")
            StrSQL.Append("               Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("               UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("               Validita_Inizio,    Validita_Fine, ")
            StrSQL.Append("               Note_Valore_Numerico,    Visibile, ")
            StrSQL.Append("               Note_Valore_Stringa ")
            StrSQL.Append("               ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append("         '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If (auto_increment_notaCod) Then
                StrSQL.Append(", " & newId)
            Else
                StrSQL.Append("         , " & Agro_SQL_SaveNum(Nota_Cod) & "  ")
            End If

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Nota_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append("         , Null   ")
            StrSQL.Append("         , 1 ")
            StrSQL.Append("         , Null   ")
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
    Public Function Modifica(ByVal Nota_Cod As Integer,
                             ByVal Nota_Des As String,
                             ByVal NotaGruppo_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Nota_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Nota_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####

            StrSQL.Append(" UPDATE Note_Intervento SET ")
            StrSQL.Append("   Nota_Des             = '" & Agro_SQL_SaveText(Nota_Des) & "'  ")
            StrSQL.Append("   ,NotaGruppo_Cod       =  " & Agro_SQL_SaveNum(NotaGruppo_Cod) & "  ")

            StrSQL.Append("   ,Inviato              =  0 ")
            StrSQL.Append("   ,DataInvio            =  Null ")
            StrSQL.Append("   ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE   PivaSuperUser   = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND     Nota_Cod        =  " & Agro_SQL_SaveNum(Nota_Cod) & "  ")

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
    Public Function ModificaVisibile(ByVal Nota_Cod As Integer,
                                     ByVal visibile As Boolean,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_W.ModificaVisibile()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Nota_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Nota_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####

            StrSQL.Append(" UPDATE Note_Intervento SET ")
            StrSQL.Append("   Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Visibile        =  " & Agro_SQL_SaveBoolStrToInt(visibile) & "  ")

            StrSQL.Append(" WHERE   PivaSuperUser   = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND     Nota_Cod        =  " & Agro_SQL_SaveNum(Nota_Cod) & "  ")

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
    Public Function Cancella(ByVal Nota_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If Nota_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (Nota_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Note_Intervento ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Note_Intervento ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            StrSQL.Append(" AND PivaSuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND Nota_Cod         =  " & Agro_SQL_SaveNum(Nota_Cod) & "   ")

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
