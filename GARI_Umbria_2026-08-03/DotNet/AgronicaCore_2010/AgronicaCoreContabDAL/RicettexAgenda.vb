Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class RicettexAgenda_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiPerAssociazionePK(ByVal Ricetta_Cod As Int32,
                          ByVal Ricetta_Operazione_Cod As Int32,
                          ByVal Id_Agenda As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByRef objParametri As AgronicaCoreParametri) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Read.LeggiPerAssociazionePK()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try
            StrSQL.Length = 0

            StrSQL.Append(" SELECT Ricetta_SuperUser, Id_Agenda, Ricetta_Cod, Ricetta_Operazione_Cod ")
            StrSQL.Append(" FROM  RicettexAgenda WITH(NOLOCK)")
            StrSQL.Append(" WHERE RicettexAgenda.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.Append(" AND   RicettexAgenda.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   RicettexAgenda.Inviato >=0 ")
                Case enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   RicettexAgenda.Inviato =-1 ")
                Case enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Id_Agenda Asc ")

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

    '##############################################################################################
    Public Function Leggi(ByVal Ricetta_Cod As Int32,
                          ByVal Ricetta_Operazione_Cod As Int32,
                          ByVal Id_Agenda As Int32,
                          ByVal Validita_Inizio As Date,
                          ByVal Validita_Fine As Date,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Read.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Id_Agenda = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  RicettexAgenda WITH(NOLOCK)")
                    StrSQL.Append(" WHERE RicettexAgenda.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   RicettexAgenda.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RicettexAgenda.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RicettexAgenda.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Id_Agenda Asc ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  RicettexAgenda WITH(NOLOCK)")
                    StrSQL.Append(" WHERE RicettexAgenda.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   RicettexAgenda.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

                    If objParametri.PivaSuperUser <> "" Then
                        StrSQL.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
                    End If

                    If Ricetta_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
                    End If

                    If Ricetta_Operazione_Cod <> 0 Then
                        StrSQL.Append(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   RicettexAgenda.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   RicettexAgenda.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Id_Agenda Asc ")
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

    Public Function LeggiRicettaOperazioneCodDaIdAgenda(ByVal idAgenda As Integer, ByRef objParametri As AgronicaCoreParametri) As DataTable

        If idAgenda = 0 Then
            Throw New ArgumentException("Id agenda deve essere maggiore di 0")
        End If

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Read.LeggiRicettaOperazioneCodDaIdAgenda()"

        Dim messaggioErrore As String = ""
        Dim sb As New Text.StringBuilder
        Dim dt As DataTable

        Try

            sb.Length = 0

            sb.AppendLine(" SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; ")

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     Ricetta_Operazione_Cod ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     RicettexAgenda ra ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     Id_Agenda = " & Agro_SQL_SaveNum(idAgenda) & "   ")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function LeggiGuidDiRicetteAssociateAlleOperazioni(ByVal listaDiIdAgenda As List(Of Integer),
                                                              ByRef objParametri As AgronicaCoreParametri) As DataTable

        If listaDiIdAgenda Is Nothing Then
            Throw New ArgumentNullException(NameOf(listaDiIdAgenda))
        End If

        If Not listaDiIdAgenda.Any() Then
            Throw New ArgumentException("La lista di id agenda non contiene nessun id agenda")
        End If

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Read.LeggiGuidDiRicetteAssociateAlleOperazioni()"

        Dim messaggioErrore As String = ""
        Dim sb As New Text.StringBuilder
        Dim dt As DataTable

        Try

            sb.Length = 0

            sb.AppendLine(" SELECT ")
            sb.AppendLine("     a.Id_Agenda, ISNULL(ro.APP_Ricetta_Operazione_Id, '') as GuidRicetta ")
            sb.AppendLine(" FROM ")
            sb.AppendLine("     Ricette_Operazioni ro ")
            sb.AppendLine(" JOIN ")
            sb.AppendLine("     RicettexAgenda ra ")
            sb.AppendLine("     ON ra.Ricetta_SuperUser = ro.Ricetta_SuperUser ")
            sb.AppendLine("     AND ra.Ricetta_Cod = ro.Ricetta_Cod ")
            sb.AppendLine("     AND ra.Ricetta_Operazione_Cod = ro.Ricetta_Operazione_Cod ")
            sb.AppendLine(" JOIN ")
            sb.AppendLine("     Agenda a ")
            sb.AppendLine("     ON a.Id_Agenda = ra.Id_Agenda ")
            sb.AppendLine(" WHERE ")
            sb.AppendLine("     a.Id_Agenda IN (" & Agro_SQL_Save_Clausola_IN(String.Join(",", listaDiIdAgenda), False) & ")")

            '--------------------------------------------------------------------------
            dt = EseguiQuery_Lettura(objParametri, sb.ToString, nomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            messaggioErrore = ex.Message
            Scrivi_LOG(objParametri, nomeRoutine, messaggioErrore)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & messaggioErrore)
        End Try

        Return dt

    End Function

    Public Function Leggi_Join_Ricette_Operazioni(ByVal Ricetta_Cod As Int32,
                                                    ByVal Ricetta_Operazione_Cod As Int32,
                                                    ByVal Id_Agenda As Int32,
                                                    ByVal Validita_Inizio As Date,
                                                    ByVal Validita_Fine As Date,
                                                    ByVal xFiltroAggiuntivo As String,
                                                    ByVal xOrderBy As String,
                                                    ByRef objParametri As AgronicaCoreParametri
                                                    ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Read.Leggi_Join_Ricette_Operazioni()"


        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Ricette_Operazioni.* ")
            StrSQL.AppendLine(" FROM  RicettexAgenda ")
            StrSQL.AppendLine(" INNER JOIN Ricette_Operazioni ")
            StrSQL.AppendLine(" ON  RicettexAgenda.Ricetta_SuperUser = Ricette_Operazioni.Ricetta_SuperUser ")
            StrSQL.AppendLine(" AND RicettexAgenda.Ricetta_Cod = Ricette_Operazioni.Ricetta_Cod ")
            StrSQL.AppendLine(" AND RicettexAgenda.Ricetta_Operazione_Cod = Ricette_Operazioni.Ricetta_Operazione_Cod ")
            StrSQL.AppendLine(" WHERE RicettexAgenda.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
            StrSQL.AppendLine(" AND   RicettexAgenda.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio))

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.AppendLine(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.AppendLine(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.AppendLine(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.AppendLine(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If


            If xFiltroAggiuntivo <> "" Then
                StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            If xOrderBy <> "" Then
                StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                StrSQL.AppendLine(" ORDER BY Ricetta_SuperUser, Ricetta_Cod, Ricetta_Operazione_Cod, Id_Agenda Asc ")
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

End Class


'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§
'§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§§



Public Class RicettexAgenda_W
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Scrivi(ByVal Ricetta_Cod As Int32,
                           ByVal Ricetta_Operazione_Cod As Int32,
                           ByVal Id_Agenda As Int32,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri,
                           Optional ByVal Data_creazione As Date = #2/1/1900#,
                           Optional ByVal Data_modifica As Date = #2/1/1900#,
                           Optional ByVal username_creazione As String = "",
                           Optional ByVal username_modifica As String = ""
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Write.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Data_creazione = #2/1/1900# Then
                Data_creazione = DateTime.Now
            End If

            If Data_modifica = #2/1/1900# Then
                Data_modifica = DateTime.Now
            End If

            If username_creazione = "" Then
                username_creazione = objParametri.UsernameOperazione
            End If

            If username_modifica = "" Then
                username_modifica = objParametri.UsernameOperazione
            End If

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO RicettexAgenda ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Ricetta_SuperUser,      Ricetta_Cod,   Ricetta_Operazione_Cod,   Id_Agenda, ")
            StrSQL.Append("          DataLock,  ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod))
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda))
            StrSQL.Append("         , 0 ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")

            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_creazione) & "  ")
            StrSQL.Append("			, " & Agro_SQL_SaveDateTime(Data_modifica) & "  ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_creazione) & "' ")
            StrSQL.Append("			,'" & Agro_SQL_SaveText(username_modifica) & "' ")

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


    '##############################################################################################
    Public Function Modifica(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Id_Agenda As Int32,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE RicettexAgenda SET ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If

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

    '##############################################################################################
    Public Function Modifica_Agenda(ByVal Ricetta_Cod As Int32,
                                    ByVal Ricetta_Operazione_Cod As Int32,
                                    ByVal Id_Agenda_OLD As Int32,
                                    ByVal Id_Agenda_NEW As Int32,
                                    ByVal Validita_Inizio As Date,
                                    ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreParametri
                                    ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Write.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If objParametri.PivaSuperUser = "" Then
                Throw New Exception("Parametro non corretto nella query (Ricetta_SuperUser obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE RicettexAgenda SET ")

            StrSQL.Append("   Id_Agenda          =  " & Agro_SQL_SaveNum(Id_Agenda_NEW) & " ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDateTime(DateTime.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE  Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Id_Agenda_OLD <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda_OLD) & "   ")
            End If

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


    '##############################################################################################
    Public Function Cancella(ByVal Ricetta_Cod As Int32,
                             ByVal Ricetta_Operazione_Cod As Int32,
                             ByVal Id_Agenda As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.RicettexAgenda_Write.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Ricetta_SuperUser = ""
        '   Ricetta_Cod = 0
        '   Ricetta_Operazione_Cod = 0
        '   Id_Agenda = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            'If Piva = "" Then
            '    Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            'End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE RicettexAgenda ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM RicettexAgenda ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If objParametri.PivaSuperUser <> "" Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'   ")
            End If

            If Ricetta_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Cod = " & Agro_SQL_SaveNum(Ricetta_Cod) & "   ")
            End If

            If Ricetta_Operazione_Cod <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Ricetta_Operazione_Cod = " & Agro_SQL_SaveNum(Ricetta_Operazione_Cod) & "   ")
            End If

            If Id_Agenda <> 0 Then
                StrSQL.Append(" AND RicettexAgenda.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
            End If


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
