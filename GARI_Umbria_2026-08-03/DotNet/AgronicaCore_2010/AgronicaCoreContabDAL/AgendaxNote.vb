Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class AgendaxNote_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    Public Function Leggi(ByVal Id_Agenda As Integer,
                          ByVal Nota_Cod As Integer,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.AgendaxNote_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Id_Agenda = 0
        '   Nota_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    strSql.Length = 0

                    strSql.Append(" SELECT * ")
                    strSql.Append(" FROM  AgendaxNote WITH(NOLOCK)")
                    strSql.Append(" WHERE Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.Append(" AND   Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        strSql.Append(" AND Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.Append(" AND Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Nota_Cod <> 0 Then
                        strSql.Append(" AND Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & "   ")
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
                        strSql.Append(" ORDER BY Piva_SuperUser, Id_Agenda, Nota_Cod Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    strSql.Length = 0

                    strSql.Append(" SELECT * ")
                    strSql.Append(" FROM  AgendaxNote WITH(NOLOCK)")
                    strSql.Append(" WHERE AgendaxNote.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.Append(" AND   AgendaxNote.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        strSql.Append(" AND AgendaxNote.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.Append(" AND AgendaxNote.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Nota_Cod <> 0 Then
                        strSql.Append(" AND AgendaxNote.Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   AgendaxNote.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   AgendaxNote.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Piva_SuperUser, Id_Agenda, Nota_Cod Asc ")
                    End If



                Case enumSelezioneVariabile.Selezione_JoinDescrizioni
                    strSql.Length = 0

                    strSql.Append(" SELECT AgendaxNote.*, Nota_Des ")
                    strSql.Append(" FROM  AgendaxNote WITH(NOLOCK)")
                    strSql.Append(" INNER Join Note_Intervento WITH(NOLOCK)")
                    strSql.Append(" ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")
                    strSql.Append(" WHERE AgendaxNote.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    strSql.Append(" And   AgendaxNote.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        strSql.Append(" And AgendaxNote.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Id_Agenda <> 0 Then
                        strSql.Append(" AND AgendaxNote.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Nota_Cod <> 0 Then
                        strSql.Append(" AND AgendaxNote.Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            strSql.Append(" AND   AgendaxNote.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            strSql.Append(" AND   AgendaxNote.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        strSql.Append(" ORDER BY Piva_SuperUser, Id_Agenda, AgendaxNote.Nota_Cod Asc ")
                    End If


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


    '##############################################################################################

    ''' <summary>
    ''' Costruisce una stringa con l'elenco delle note selezionate partendo dall'id_agenda una di seguito all'altra
    ''' </summary>
    ''' <param name="Id_Agenda"></param>
    ''' <param name="Validita_Inizio"></param>
    ''' <param name="Validita_Fine"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns>Stringa con l'elenco delle note associate all'id_agenda</returns>
    ''' <remarks></remarks>
    Public Function ElencoDescrizioniInStringa(ByVal Id_Agenda As Integer,
                                               ByVal Validita_Inizio As Date,
                                               ByVal Validita_Fine As Date,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As String

        Const nomeRoutine = "AgronicaCoreContabDAL.AgendaxNote_R.ElencoDescrizioniInStringa()"

        '====================================================================================
        'Parametri opzionali :
        '   Id_Agenda = 0
        '   Nota_Cod = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try

            '------------------------------------------------------------------

            strSql.Length = 0

            strSql.Append(" SELECT distinct Nota_Des ")
            strSql.Append(" FROM  AgendaxNote INNER JOIN Note_Intervento ")
            strSql.Append(" ON AgendaxNote.Nota_Cod = Note_Intervento.Nota_Cod ")
            strSql.Append(" WHERE AgendaxNote.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            strSql.Append(" AND   AgendaxNote.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append(" AND AgendaxNote.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.Append(" AND AgendaxNote.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If



            If xFiltroAggiuntivo <> "" Then
                strSql.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    strSql.Append(" AND   AgendaxNote.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    strSql.Append(" AND   AgendaxNote.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

        If Not IsNothing(dt) AndAlso dt.Rows.Count > 0 Then
            Dim strNote As String = ""

            For c As Integer = 0 To dt.Rows.Count - 1
                strNote &= String.Format("{0}{1}", dt.Rows(c).Item("Nota_Des"), vbCrLf)
            Next

            Return strNote
        Else
            Return ""
        End If

    End Function

End Class



'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class AgendaxNote_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(ByVal Id_Agenda As Integer,
                           ByVal Nota_Cod As Integer,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AgendaxNote_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

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

            '---------------------------------------------
            strSql.Length = 0

            strSql.Append(" INSERT INTO AgendaxNote ")
            strSql.Append("         ( ")
            strSql.Append("          Piva_SuperUser,      Id_Agenda,   Nota_Cod, ")
            strSql.Append("          Inviato,            DataInvio, ")
            strSql.Append("          Data_Creazione,     Data_Modifica, ")
            strSql.Append("          UserName_Creazione, UserName_Modifica, ")
            strSql.Append("          Validita_Inizio,    Validita_Fine ")
            strSql.Append("         ) ")

            strSql.Append(" VALUES ( ")
            strSql.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            strSql.Append("         , " & Agro_SQL_SaveNum(Id_Agenda))
            strSql.Append("         , " & Agro_SQL_SaveNum(Nota_Cod))
            strSql.Append("         , 0  ")
            strSql.Append("         , Null  ")

            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            strSql.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            strSql.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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
    Public Function Modifica(ByVal Id_Agenda As Integer,
                             ByVal Nota_Cod As Integer,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AgendaxNote_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            strSql.Length = 0

            strSql.Append(" UPDATE AgendaxNote SET ")

            strSql.Append("   ,Inviato           =  0 ")
            strSql.Append("   ,DataInvio         =  Null ")
            strSql.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(Date.Now))
            strSql.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            strSql.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            strSql.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            strSql.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Id_Agenda <> 0 Then
                strSql.Append(" AND AgendaxNote.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Nota_Cod <> 0 Then
                strSql.Append(" AND AgendaxNote.Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & "   ")
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


    '##############################################################################################
    Public Function Cancella(ByVal Id_Agenda As Integer,
                             ByVal Nota_Cod As Integer,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.AgendaxNote_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva_SuperUser = ""
        '   Nota_Cod = 0
        '   Id_Agenda = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva_SuperUser obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                strSql.Length = 0
                strSql.Append(" UPDATE AgendaxNote ")
                strSql.Append(" SET ")
                strSql.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                strSql.Append("      ,Inviato = -1 ")
                strSql.Append(" WHERE  Inviato >= 0 ")

            Else
                strSql.Length = 0
                strSql.Append(" DELETE ")
                strSql.Append(" FROM AgendaxNote ")
                strSql.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                strSql.Append(" AND AgendaxNote.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Id_Agenda <> 0 Then
                strSql.Append(" AND AgendaxNote.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            If Nota_Cod <> 0 Then
                strSql.Append(" AND AgendaxNote.Nota_Cod = " & Agro_SQL_SaveNum(Nota_Cod) & "   ")
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
