Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ContattixRubrica_R
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Piva"></param>
    ''' <param name="Cod_Contatto"></param>
    ''' <param name="sa_cod">default 99</param>
    ''' <param name="Cod_Rubrica"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[garavini]	26/11/2010	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function LeggiContattoSpecifico(ByVal Piva As String,
                                           ByVal Cod_Contatto As String,
                                           ByVal sa_cod As Integer,
                                           ByVal Cod_Rubrica As Integer,
                                           ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                           ByVal xFiltroAggiuntivo As String,
                                           ByVal xOrderBy As String,
                                           ByRef objParametri As AgronicaCoreParametri
                                           ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContattixRubrica_R.LeggiContattoSpecifico()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            'Select Case xSelezioneVariabile

            '    Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi


            '    Case enumSelezioneVariabile.Selezione_TabellaCompleta


            '    Case enumSelezioneVariabile.Selezione_JoinDescrizioni


            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("  SELECT * " & _
          " FROM  ContattixRubrica, Rubrica " & _
          " WHERE ContattixRubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
          " AND   ContattixRubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
          " AND   Rubrica.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
          " AND   Rubrica.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
          " AND   ContattixRubrica.Cod_Rubrica = Rubrica.Cod_Rubrica " & _
          " AND   ContattixRubrica.Piva = '" & Agro_SQL_SaveText(Piva) & "' " & _
          " AND   ContattixRubrica.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")


            If Cod_Rubrica <> 0 Then
                StrSQL.Append(" AND ContattixRubrica.Cod_Rubrica = " & Agro_SQL_SaveNum(Cod_Rubrica) & " ")
            End If

            If sa_cod <> 99 Then
                StrSQL.Append(" AND ContattixRubrica.sa_cod = " & Agro_SQL_SaveNum(sa_cod) & " ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato >= 0 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato = -1 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '    Case enumSelezioneVariabile.Selezione_JoinCompleta



            'End Select
            '---------------------------------------------

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

    Public Function LeggiByCod_Risum(ByVal Cod_RisUm As Integer,
                                     ByVal Descrizione As String,
                                     ByVal DescrizioneLike As String,
                                     ByVal xFiltroAggiuntivo As String,
                                     ByVal xOrderBy As String,
                                     ByRef objParametri As AgronicaCoreParametri
                                     ) As DataTable

        'ByVal Piva As String,
        'ByVal Cod_Contatto As String,

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContattixRubrica_R.LeggiByCod_Risum()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            'End If

            'If Cod_Contatto = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            'End If

            If Cod_RisUm = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_RisUm obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT Risorse_Umane.piva, Risorse_Umane.cod_contatto, Risorse_Umane.cod_risum, Rubrica.numero, Rubrica.descr ")
            StrSQL.Append(" FROM  Rubrica ")
            StrSQL.Append(" INNER JOIN ContattiXRubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica ")
            StrSQL.Append(" INNER JOIN Risorse_Umane ON Risorse_Umane.piva = ContattiXRubrica.piva ")
            StrSQL.Append(" AND Risorse_Umane.cod_contatto = ContattiXRubrica.cod_contatto ")

            StrSQL.Append(" WHERE Rubrica.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Rubrica.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            'If Piva <> "" Then
            '    StrSQL.Append(" AND Risorse_Umane.Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            'End If

            'If Cod_Contatto <> "" Then
            '    StrSQL.Append(" AND Risorse_Umane.Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            'End If

            If Cod_RisUm <> 0 Then
                StrSQL.Append(" AND Risorse_Umane.Cod_RisUm = " & Agro_SQL_SaveNum(Cod_RisUm) & " ")
            End If

            If Descrizione <> "" Then
                StrSQL.Append(" AND descr = '" & Agro_SQL_SaveText(Descrizione) & "' ")
            End If

            If DescrizioneLike <> "" Then
                StrSQL.Append(" AND descr LIKE '%" & Agro_SQL_SaveText(DescrizioneLike) & "%' ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato >= 0 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato = -1 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '---------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        Return dt

    End Function


    Public Function LeggixMailGSB(ByVal xFiltroAggiuntivo As String,
                                  ByVal xOrderBy As String,
                                  ByRef objParametri As AgronicaCoreParametri
                                  ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContattixRubrica_R.LeggixMailGSB()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" Select Numero  ")
            StrSQL.Append(" FROM  Rubrica, contattixrubrica, risorse_umane  ")
            StrSQL.Append(" WHERE Rubrica.cod_rubrica = Contattixrubrica.Cod_Rubrica ")
            StrSQL.Append(" AND   ContattiXRubrica.Cod_Contatto = risorse_umane.Cod_Contatto ")
            StrSQL.Append(" AND   Rubrica.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Rubrica.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato >= 0 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato = -1 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '---------------------------------------------

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        Return dt

    End Function


    Public Function Esiste_Rubrica(ByVal Piva As String,
                                   ByVal Cod_Contatto As String,
                                   ByVal Descrizione As String,
                                   ByVal Numero As String,
                                   ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                   ByVal xFiltroAggiuntivo As String,
                                   ByRef objParametri As AgronicaCoreParametri
                                   ) As Boolean

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContattixRubrica_R.Esiste_Rubrica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Dim bRet As Boolean = False

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Numero = "" Then
                Throw New Exception("Parametro non corretto nella query (Numero obbligatorio)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  Rubrica ")
            StrSQL.Append(" INNER JOIN ContattiXRubrica ON Rubrica.cod_rubrica = ContattiXRubrica.Cod_Rubrica ")

            StrSQL.Append(" WHERE Rubrica.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Rubrica.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND Cod_Contatto = '" & Agro_SQL_SaveText(Cod_Contatto) & "' ")
            StrSQL.Append(" AND Numero = '" & Agro_SQL_SaveText(Numero) & "' ")

            If Descrizione <> "" Then
                StrSQL.Append(" AND descr = '" & Agro_SQL_SaveText(Descrizione) & "' ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato >= 0 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     Rubrica.Inviato = -1 ")
                    StrSQL.Append(" AND     ContattiXRubrica.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '---------------------------------------------

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, StrSQL.ToString, nomeRoutine)
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

    Public Function LeggiEmailCollegate_daPiva(ByVal Piva As String,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreAnagrafeDAL.ContattixRubrica_R.LeggiEmailCollegate_daPiva()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatoria)")
            End If

            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("SELECT cr.cod_rubrica, numero AS email")
            StrSQL.AppendLine("FROM ContattiXRubrica cr")
            StrSQL.AppendLine("LEFT JOIN Rubrica r on cr.Cod_Rubrica = r.cod_rubrica")
            StrSQL.AppendLine("WHERE cr.Piva = '" & Agro_SQL_SaveText(Piva) & "'")
            StrSQL.AppendLine("AND descr LIKE '%Mail%'")

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND     r.Inviato >= 0 ")
                    StrSQL.Append(" AND     cr.Inviato >= 0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND     r.Inviato = -1 ")
                    StrSQL.Append(" AND     cr.Inviato = -1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
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

Public Class ContattixRubrica_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '============================================================================
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Cod_Contatto As String,
                           ByVal Cod_Rubrica As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ContattixRubrica_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

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

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("INSERT INTO ContattixRubrica( ")
            StrSQL.Append("                    Piva,        ")
            StrSQL.Append("                    Sa_Cod,      ")
            StrSQL.Append("                    Cod_Contatto,      ")
            StrSQL.Append("                    Cod_Rubrica,      ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Cod_Rubrica) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_creazione) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Data_modifica) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(username_modifica) & "' ")
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


    '############################################################################
    '############################################################################
    '############################################################################

    '============================================================================
    Public Function Modifica(ByVal Cod_Contatto As String,
                             ByVal New_Piva As String,
                             ByVal New_Sa_Cod As Long,
                             ByVal Cod_Rubrica As Long,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ContattixRubrica_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Cod_Contatto = "" Then
                Throw New Exception("Parametro non corretto nella query (Cod_Contatto obbligatorio)")
            End If

            If Cod_Rubrica = 0 Then
                Throw New Exception("Parametro non corretto nella query (Cod_Rubrica obbligatorio)")
            End If

            '------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE ContattixRubrica SET ")
            StrSQL.Append("    Piva              = '" & Agro_SQL_SaveText(New_Piva) & "'  ")
            StrSQL.Append("   ,Sa_Cod            =  " & Agro_SQL_SaveNum(New_Sa_Cod) & "  ")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "'")
            StrSQL.Append(" AND   Cod_Rubrica = " & Cod_Rubrica & " ")

            '------------------------------
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

    '############################################################################
    '############################################################################
    '############################################################################

    '============================================================================
    Public Function Cancella(ByVal Cod_Contatto As String,
                             ByVal Cod_Rubrica As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ContattixRubrica_W.Cancella()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE ContattixRubrica ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     ContattixRubrica ")
                StrSQL.Append(" WHERE    1=1 ")

            End If

            If Cod_Contatto <> "" Then
                StrSQL.Append(" AND   Cod_Contatto = '" & Agro_SQL_SaveText(Trim(Cod_Contatto)) & "' ")
            End If

            If Cod_Rubrica <> 0 Then
                StrSQL.Append(" AND   Cod_Rubrica = " & Cod_Rubrica & " ")
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


    Public Function Aggungi_Telefono(PIVA As String, Cod_Contatto As String, SA_COD As Integer,
                                     telefono As String, Cod_Rubrica_Ret As Integer,
                                     objParametri As AgronicaCoreParametri
                                     ) As Boolean

        Const nomeRoutine = "AnagrafeCoreAnagrafeDAL.ContattixRubrica_W.Aggungi_Telefono()"

        Dim messaggioErrore As String = ""
        Dim xRisp As Boolean = False
        Cod_Rubrica_Ret = 0

        Try

            'per evitare di eliminare tutto
            If PIVA = "" Then
                Throw New Exception("Occorre specificare una Piva")
            End If

            If Cod_Contatto = "" Then
                Throw New Exception("Occorre specificare un Cod_Contatto")
            End If

            Dim ContattixRubrica_R As New AgronicaCoreAnagrafeDAL.ContattixRubrica_R
            Dim dt As DataTable = ContattixRubrica_R.LeggiContattoSpecifico(PIVA, Cod_Contatto, SA_COD,
                                          0, enumSelezioneVariabile.Selezione_JoinCompleta,
                                          " rubrica.numero = '" & Agro_SQL_SaveText(telefono.Replace("'", "")) & "' ",
                                          "", objParametri)

            '---------------------------------------------
            Dim Rubrica_Write As New AgronicaCoreAnagrafeDAL.Rubrica_Write


            Dim codRubrica As Integer = 0
            If dt.Rows.Count = 0 Then
                'scrivo il numero che mi ritorna il codice codice
                xRisp = Rubrica_Write.Aggiungi_Aggiorna(codRubrica, telefono, "Telefono",
                                                        AGRODATAINIZIO, AGRODATAFINE,
                                                        objParametri)
                'scrivo la voce in centri per rubrica
                If xRisp Then
                    xRisp = Scrivi(PIVA, SA_COD, Cod_Contatto, codRubrica,
                                   AGRODATAINIZIO, AGRODATAFINE,
                                   objParametri)
                End If
            ElseIf dt.Rows.Count > 0 Then
                'il numero c'è già, non faccio nulla
                ''For i = 0 To dt.Rows.Count - 1
                ''    cod_rubrica = dt.Rows(i).Item("Cod_Rubrica")
                ''    xRisp = xRisp Or Rubrica_Write.Aggiungi_Aggiorna(cod_rubrica, telefono, "Telefono", AGRODATAINIZIO, AGRODATAFINE, objParametri)
                ''Next
            Else
                Throw New Exception("")
            End If


            '--------------------------------------------------------------------------

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
