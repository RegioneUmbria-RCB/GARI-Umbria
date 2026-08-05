Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class Note_Intervento_Utilizzo_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '###################################################################################
    Public Function NuovoId_NoteInterventoUtilizzo(ByRef objParametri As AgronicaCoreParametri) As Integer

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R.NuovoId_NoteInterventoUtilizzo()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable
        Dim notaGruppoCod As Integer = -1

        Try

            'StrSQL.Append("( SELECT (CASE(MAX(notautilizzo_cod)+1) WHEN 0 THEN 1 ELSE MAX(notautilizzo_cod)+1 END) as COD  FROM note_intervento_utilizzo )")

            StrSQL.Append(" SELECT ")
            StrSQL.Append(" ISNULL ( ")
            StrSQL.Append("             (CASE(MAX(notautilizzo_cod) + 1 ) ")
            StrSQL.Append("                 WHEN 0 THEN 1  ")
            StrSQL.Append("                 ELSE MAX(notautilizzo_cod) +1 END   ")
            StrSQL.Append("             )  ")
            StrSQL.Append("         , 1) AS COD  ")
            StrSQL.Append(" FROM note_intervento_utilizzo ")

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

    Public Function Leggi(ByVal nota_utilizzo_cod As Integer,
                          ByVal objParametri As AgronicaCoreParametri,
                          Optional ByVal TipoG2G As Integer = 0
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R.Leggi"

        Dim StrSQL As New Text.StringBuilder

        StrSQL.Append(" SELECT * ")
        StrSQL.Append(" FROM  Note_Intervento_Utilizzo AS NIU ")
        StrSQL.Append(" WHERE NIU.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
        StrSQL.Append(" AND   NIU.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
        StrSQL.Append(" AND   NIU.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
        If (nota_utilizzo_cod <> 0) Then
            StrSQL.Append(" AND   NIU.NotaUtilizzo_cod = " & Agro_SQL_SaveNum(nota_utilizzo_cod.ToString()) & "  ")
        End If
        StrSQL.Append(" ORDER BY NIU.NotaUtilizzo_Des")


        Select Case TipoG2G
            Case 1 'seleziona i nuovi dati.
                StrSQL.Append(" AND not exists ( ")
                StrSQL.Append(" select 1 from g2g_recode_NoteUtilizzo rr where rr.From_PivaSuperUser = NIU.pivaSuperUser and  rr.From_NotaUtilizzo_Cod = NIU.NotaUtilizzo_cod ")
                StrSQL.Append(" ) ")

            Case 2 'seleziona i dati modificati

        End Select

        Try  
            '--------------------------------------------------------------------------
            Return MyBase.EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MyBase.Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
            Return Nothing
        End Try

    End Function

    'ottiene la descrizione dalla stringa
    Public Function NotaUtilizzoDes_from_NotaUtilizzoCod(ByVal notaUtilizzo_cod As Integer,
                                                         ByVal agroParam As AgronicaCoreParametri
                                                         ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R.NotaUtilizzoDes_from_NotaUtilizzoCod"
        Dim StrSQL As New Text.StringBuilder

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT  ")

            StrSQL.Append(" NotaUtilizzo_Des ")

            StrSQL.Append(" FROM  Note_Intervento_Utilizzo ")

            StrSQL.Append(" WHERE PivaSuperUser = '" & Agro_SQL_SaveText(agroParam.PivaSuperUser) & "' ")
            StrSQL.Append(" AND   Validita_Inizio <= " & Agro_SQL_SaveDate(agroParam.FinestraTemporaleFine) & "  ")
            StrSQL.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(agroParam.FinestraTemporaleInizio) & "  ")
            StrSQL.Append(" AND NotaUtilizzo_Cod = " & Agro_SQL_SaveNum(notaUtilizzo_cod) & " ")


            '--------------------------------------------------------------------------
            Dim dt As DataTable = MyBase.EseguiQuery_Lettura(agroParam, StrSQL.ToString, nomeRoutine)
            If (dt.Rows.Count > 0) Then
                Return dt.Rows(0)(0).ToString()
            Else
                Return ""
            End If
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MyBase.Scrivi_LOG(agroParam, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

    End Function

    Public Function LeggiNote_X_Utilizzo(ByVal nota_utilizzo_cod As Integer, ByVal NotaGruppo_Cod As Integer,
                                         ByVal solo_note_con_utilizzo As Boolean,
                                         ByVal objParametri As AgronicaCoreParametri
                                         ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R.LeggiNote_X_Utilizzo"

        Dim StrSQL As New Text.StringBuilder
        Dim joinCondition As String = " INNER JOIN "
        'IN QUESTO caso le visualizzo tutte
        If (Not solo_note_con_utilizzo) Then
            joinCondition = " LEFT OUTER JOIN "
        End If

        StrSQL.Append(" SELECT NI.Nota_Cod, NI.Nota_Des, NIG.NotaGruppo_Cod, NIG.NotaGruppo_Des, ")
        StrSQL.Append(" ISNULL(NIU.NotaUtilizzo_Cod,0) AS NotaUtilizzo_Cod, ISNULL(NIU.NotaUtilizzo_Des,'') AS NotaUtilizzo_Des, NI.Visibile ")
        StrSQL.Append("FROM Note_Intervento AS NI INNER JOIN ")
        StrSQL.Append(" Note_Intervento_Gruppi AS NIG ON NI.NotaGruppo_Cod = NIG.NotaGruppo_Cod ")
        StrSQL.Append(joinCondition & " Note_Intervento_UtilizzoxGruppi AS NIUG ")
        StrSQL.Append(" ON NIUG.NotaGruppo_Cod = NIG.NotaGruppo_Cod " & joinCondition)
        StrSQL.Append(" Note_Intervento_Utilizzo AS NIU ON NIU.NotaUtilizzo_Cod = NIUG.NotaUtilizzo_Cod ")
        'altre clausole
        StrSQL.Append(" WHERE NI.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
        StrSQL.Append(" AND   NI.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
        StrSQL.Append(" AND   NI.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")

        If (nota_utilizzo_cod <> 0) Then
            StrSQL.Append(" AND   NIU.NotaUtilizzo_cod = " & Agro_SQL_SaveNum(nota_utilizzo_cod.ToString()) & "  ")
        End If

        If (NotaGruppo_Cod <> 0) Then
            StrSQL.Append(" AND   NIG.NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod.ToString()) & "  ")
        End If

        StrSQL.Append(" ORDER BY NIU.NotaUtilizzo_Des, NIG.NotaGruppo_Des, NI.Nota_Des")


        Try  '--------------------------------------------------------------------------
            Return MyBase.EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MyBase.Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
            Return Nothing
        End Try

    End Function


    Public Function LeggiNote_X_Utilizzo_Visibile(ByVal nota_utilizzo_cod As Integer,
                                                  ByVal NotaGruppo_Cod As Integer,
                                                  ByVal solo_note_con_utilizzo As Boolean,
                                                  ByVal xFiltroAggiuntivo As String,
                                                  ByVal objParametri As AgronicaCoreParametri,
                                                  Optional ByVal meno1tutti_0nonVisibili_1soloVisibili As Integer = - 1
                                                  ) As DataTable

        '-----------------------------------------
        '   A confronto con LeggiNote_X_Utilizzo legge solo i gruppi che hanno visibile = 1
        '   e all'interno di essi le note che hanno visibile = 1
        '
        '  In questo modo si vedono solo le note visibili in gruppi visibili
        '
        '------------------------------------------

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R.LeggiNote_X_Utilizzo_Visibile"

        Dim StrSQL As New Text.StringBuilder
        Dim joinCondition As String = " INNER JOIN "
        'IN QUESTO caso le visualizzo tutte
        If (Not solo_note_con_utilizzo) Then
            joinCondition = " LEFT OUTER JOIN "
        End If

        StrSQL.Append(" SELECT NI.Nota_Cod, NI.Nota_Des, NIG.NotaGruppo_Cod, NIG.NotaGruppo_Des, ")
        StrSQL.Append(" ISNULL(NIU.NotaUtilizzo_Cod,0) AS NotaUtilizzo_Cod, ISNULL(NIU.NotaUtilizzo_Des,'') AS NotaUtilizzo_Des, NI.Visibile, NIG.Visibile AS VisibileGruppo ")
        StrSQL.Append(" FROM Note_Intervento AS NI INNER JOIN ")
        StrSQL.Append(" Note_Intervento_Gruppi AS NIG ON NI.NotaGruppo_Cod = NIG.NotaGruppo_Cod ")
        StrSQL.Append(joinCondition & " Note_Intervento_UtilizzoxGruppi AS NIUG ")
        StrSQL.Append(" ON NIUG.NotaGruppo_Cod = NIG.NotaGruppo_Cod " & joinCondition)
        StrSQL.Append(" Note_Intervento_Utilizzo AS NIU ON NIU.NotaUtilizzo_Cod = NIUG.NotaUtilizzo_Cod ")
        'altre clausole
        StrSQL.Append(" WHERE NI.PivaSuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
        StrSQL.Append(" AND   NI.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & "  ")
        StrSQL.Append(" AND   NI.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & "  ")
        'visibilità singole note
        If (meno1tutti_0nonVisibili_1soloVisibili >= 0) Then
            StrSQL.Append(" AND   NI.Visibile = " & Agro_SQL_SaveNum(meno1tutti_0nonVisibili_1soloVisibili) & "  ")
            'visibilità gruppo
            StrSQL.Append(" AND   NIG.Visibile = " & Agro_SQL_SaveNum(meno1tutti_0nonVisibili_1soloVisibili) & "  ")
        End If


        If (nota_utilizzo_cod <> 0) Then
            StrSQL.Append(" AND   NIU.NotaUtilizzo_cod = " & Agro_SQL_SaveNum(nota_utilizzo_cod.ToString()) & "  ")
        End If

        If (NotaGruppo_Cod <> 0) Then
            StrSQL.Append(" AND   NIG.NotaGruppo_Cod = " & Agro_SQL_SaveNum(NotaGruppo_Cod.ToString()) & "  ")
        End If

        If xFiltroAggiuntivo <> "" Then
            StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
        End If

        StrSQL.Append(" ORDER BY NIU.NotaUtilizzo_Des, NIG.NotaGruppo_Des, NI.Nota_Des")


        Try  '--------------------------------------------------------------------------
            Return MyBase.EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MyBase.Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
            Return Nothing
        End Try

    End Function

End Class

Public Class Note_Intervento_Utilizzo_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Cancella(ByVal NotaUtilizzo_Cod As Integer,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_W.Cancella"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaUtilizzo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaUtilizzo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Note_Intervento_Utilizzo ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Note_Intervento_Utilizzo ")
                StrSQL.Append(" WHERE  1=1 ")
            End If

            StrSQL.Append(" AND PivaSuperUser    = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append(" AND NotaUtilizzo_Cod         =  " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "   ")

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
    Public Function Modifica(ByVal NotaUtilizzo_Cod As Integer,
                             ByVal NotaUtilizzo_Des As String,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_W.Modifica"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaUtilizzo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaUtilizzo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            'Query per l'inserimento dei dati                 ' #### CLASSE ####

            StrSQL.Append(" UPDATE Note_Intervento_Utilizzo SET ")
            StrSQL.Append(" NotaUtilizzo_Des             = '" & Agro_SQL_SaveText(NotaUtilizzo_Des) & "'  ")

            StrSQL.Append("   ,Inviato              =  0 ")
            StrSQL.Append("   ,DataInvio            =  Null ")
            StrSQL.Append("   ,Data_Modifica        =  " & Agro_SQL_SaveDateTime(Date.Now))
            StrSQL.Append("   ,UserName_Modifica    = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio      =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine        =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE   PivaSuperUser   = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            StrSQL.Append(" AND     NotaUtilizzo_Cod        =  " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "  ")

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

    Public Function Note_Intervento_Utilizzo_MarcaComeInviato(ByVal NotaUtilizzo_Cod As Int32,
                                                              ByVal Data_invio As DateTime,
                                                              ByVal xFiltroAggiuntivo As String,
                                                              ByRef objParametri As AgronicaCoreParametri
                                                              ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.NoteIntervento_W.Note_Intervento_Utilizzo_MarcaComeInviato()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If NotaUtilizzo_Cod = 0 Then
                Throw New Exception("Parametro non corretto nella query (NotaGruppo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Note_Intervento_Utilizzo SET ")
            StrSQL.Append("     inviato         =  -2 ")
            StrSQL.Append("    ,datainvio         =  " & Agro_SQL_SaveDateTime(Data_invio))

            StrSQL.Append(" WHERE   PivaSuperUser        = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")
            StrSQL.Append(" AND     NotaUtilizzo_Cod   = " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "  ")


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


    Public Function Scrivi(ByVal NotaUtilizzo_Cod As Integer,
                           ByVal NotaUtilizzo_Des As String,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByVal autoIncrementaNotaUtilizzo_Cod As Boolean,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Note_Intervento_Utilizzo_W.Scrivi"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (PivaSuperUser obbligatorio)")
            End If

            If NotaUtilizzo_Cod = 0 AndAlso Not autoIncrementaNotaUtilizzo_Cod Then
                Throw New Exception("Parametro non corretto nella query (NotaUtilizzo_Cod obbligatorio)")
            End If

            '---------------------------------------------
            Dim newId As Integer
            If autoIncrementaNotaUtilizzo_Cod Then

                Dim objNoteGruppi As New AgronicaCoreContabDAL.Note_Intervento_Utilizzo_R
                newId = objNoteGruppi.NuovoId_NoteInterventoUtilizzo(objParametri)

                'StrSQL.Length = 0
                'Dim DT As DataTable
                'StrSQL.Append("( SELECT (CASE(MAX(notautilizzo_cod)+1) WHEN 0 THEN 1 ELSE MAX(notautilizzo_cod)+1 END) as COD  FROM note_intervento_utilizzo )")
                'DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
                'Dim newID As Integer = DT.Rows(0).Item("Cod")
            End If

            StrSQL.Length = 0
            StrSQL.Append(" INSERT INTO Note_Intervento_Utilizzo (")
            StrSQL.Append("               [PivaSuperUser]  ,[NotaUtilizzo_Cod]   ,[NotaUtilizzo_Des], ")
            StrSQL.Append("               Inviato,            DataInvio, ")
            StrSQL.Append("               Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("               UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("               Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("               ) ")

            StrSQL.Append(" VALUES (")
            StrSQL.Append(" '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'  ")

            If (autoIncrementaNotaUtilizzo_Cod) Then
                'NB QUESTO PER EVITARE NEL CASO ABBIA SOLO GLI UTILIZZI DI DEFAULT DI AVERE IL RECORD SUCCESSIVO
                'CON UN CODICE = A 0
                StrSQL.Append(", " & newId)
            Else
                StrSQL.Append("         , " & Agro_SQL_SaveNum(NotaUtilizzo_Cod) & "  ")
            End If

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(NotaUtilizzo_Des) & "'  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDateTime(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
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

End Class
