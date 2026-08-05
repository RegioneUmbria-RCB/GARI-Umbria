Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.AgronicaCoreParametri


Public Class Trasformazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '################################################################################
    Public Function Leggi_Operazioni_PassaggioACommercializzazione(ByVal Piva As String,
                                                                   ByVal Lista_PrepCod_PassaggiAComm As String,
                                                                   ByVal Lotto As String,
                                                                   ByVal xFiltroAggiuntivo1 As String,
                                                                   ByVal xFiltroAggiuntivo2 As String,
                                                                   ByVal xFiltroAggiuntivo3 As String,
                                                                   ByVal xOrderBy As String,
                                                                   ByRef objParametri As AgronicaCoreParametri
                                                                   ) As DataTable

        'ByVal Lista_IdTrasf_NoComm As String,

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_R.Leggi_Operazioni_PassaggioACommercializzazione"
        Dim messaggioErrore As String = ""
        Dim dt As DataTable
        Dim strSql As New Text.StringBuilder

        Try

            strSql.Length = 0

            Dim objStampe As New AgronicaCoreStampeDAL.RegistriCantina
            strSql.Append(objStampe.SQL_Operazioni_PassaggioACommercializzazione(Piva,
                                                                                 Lista_PrepCod_PassaggiAComm,
                                                                                 Lotto,
                                                                                 xFiltroAggiuntivo1,
                                                                                 xFiltroAggiuntivo2,
                                                                                 xFiltroAggiuntivo3))

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

        Return dt

    End Function


    '#####################################################################
    Public Function Leggi(ByVal Piva As String,
                          ByVal Sa_Cod As Int32,
                          ByVal Id_Trasformazione As Int32,
                          ByVal Linea_Cod As Int32,
                          ByVal Preparazione_Cod As Int32,
                          ByVal Id_Agenda As Int32,
                          ByVal Stato As Integer,
                          ByVal Cau_Mov As String,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable

        Const nomeRoutine As String = "AgronicaCoreContabDAL.Trasformazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0 
        '   Id_Trasformazione = 0    
        '   Linea_Cod = 0
        '   Preparazione_Cod = 0
        '   Id_Agenda = 0    
        '   Stato = 0
        '   Cau_Mov = ""
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Trasformazioni.*, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des,  ")
                    StrSQL.Append("         Linee_Produzioni.Linea_Classe_Cod, Linee_Produzioni.ChkCantine ")
                    StrSQL.Append(" FROM    Trasformazioni ")
                    StrSQL.Append(" LEFT OUTER JOIN Linee_Produzioni ON (Trasformazioni.Linea_Cod = Linee_Produzioni.Linea_Cod) ")
                    StrSQL.Append(" LEFT OUTER JOIN Linee_Preparazioni ON (Trasformazioni.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod) ")
                    StrSQL.Append(" WHERE   Trasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND     Trasformazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Trasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Trasformazione <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
                    End If

                    If Linea_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
                    End If

                    If Preparazione_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Stato <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Stato = " & Agro_SQL_SaveNum(Stato) & "   ")
                    End If



                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Trasformazioni.Inviato >=0 ")
                            StrSQL.Append(" AND   Linee_Produzioni.Inviato >=0 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Trasformazioni.Inviato =-1 ")
                            StrSQL.Append(" AND   Linee_Produzioni.Inviato =-1 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Trasformazioni.Piva, Trasformazioni.Data_Creazione ASC ")
                    End If



                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    '------------------------------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT  Trasformazioni.*, Linee_Preparazioni.Preparazione_Des, Linee_Produzioni.Linea_Des,  ")
                    StrSQL.Append("         Linee_Produzioni.Linea_Classe_Cod, Linee_Produzioni.ChkCantine ")
                    StrSQL.Append(" FROM    Trasformazioni ")
                    StrSQL.Append(" LEFT OUTER JOIN Linee_Produzioni ON (Trasformazioni.Linea_Cod = Linee_Produzioni.Linea_Cod) ")
                    StrSQL.Append(" LEFT OUTER JOIN Linee_Preparazioni ON (Trasformazioni.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod) ")
                    StrSQL.Append(" WHERE   Trasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND     Trasformazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND     Trasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))


                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Trasformazione <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
                    End If

                    If Linea_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
                    End If

                    If Preparazione_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If

                    If Stato <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Stato = " & Agro_SQL_SaveNum(Stato) & "   ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Trasformazioni.Inviato >=0 ")
                            StrSQL.Append(" AND   Linee_Produzioni.Inviato >=0 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Trasformazioni.Inviato =-1 ")
                            StrSQL.Append(" AND   Linee_Produzioni.Inviato =-1 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Trasformazioni.Piva, Trasformazioni.Data_Creazione ASC ")
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

    '##################################################################
    Public Function LeggiTrasformazioni_Agenda(ByVal Piva As String,
                                               ByVal Sa_Cod As Int32,
                                               ByVal Id_Trasformazione As Int32,
                                               ByVal Linea_Cod As Int32,
                                               ByVal Preparazione_Cod As Int32,
                                               ByVal Id_Agenda As Int32,
                                               ByVal xSelezioneVariabile As enumSelezioneVariabile,
                                               ByVal xFiltroAggiuntivo As String,
                                               ByVal xOrderBy As String,
                                               ByRef objParametri As AgronicaCoreParametri
                                               ) As DataTable

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_R.LeggiTrasformazioni_Agenda()"

        '====================================================================================
        'Parametri opzionali :
        '   Sa_Cod = 0            
        '   Id_Trasformazione = 0           
        '   Linea_Cod = 0  =
        '   Preparazione_Cod = 0  
        '   Id_Agenda = 0 
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim dt As DataTable

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '------------------------------------------------------------------

            Select Case xSelezioneVariabile

                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Agenda.*, Linee_Preparazioni.Preparazione_Des, Trasformazioni.Trasformazione_Des, Trasformazioni.Stato  ")
                    StrSQL.Append(" FROM   Agenda, Linee_Preparazioni, Trasformazioni ")
                    StrSQL.Append(" WHERE  Trasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND    Trasformazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Trasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Agenda.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Agenda.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Agenda.Piva = Trasformazioni.Piva ")
                    StrSQL.Append(" AND    Agenda.Piva = Linee_Preparazioni.Piva ")
                    StrSQL.Append(" AND    Agenda.Id_Trasformazione = Trasformazioni.Id_Trasformazione ")
                    StrSQL.Append(" AND    Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod ")


                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Trasformazione <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
                    End If

                    If Linea_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
                    End If

                    If Preparazione_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato >=0 ")
                            StrSQL.Append(" AND   Trasformazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato =-1 ")
                            StrSQL.Append(" AND   Trasformazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Agenda.Piva, Agenda.Validita_Fine ASC ")
                    End If


                Case enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Distinct Agenda.*, Linee_Preparazioni.Preparazione_Des, Trasformazioni.Trasformazione_Des, Trasformazioni.Stato  ")
                    StrSQL.Append(" FROM   Agenda, Linee_Preparazioni, Trasformazioni ")
                    StrSQL.Append(" WHERE  Trasformazioni.Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                    StrSQL.Append(" AND    Trasformazioni.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Trasformazioni.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Agenda.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Agenda.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND    Agenda.Piva = Trasformazioni.Piva ")
                    StrSQL.Append(" AND    Agenda.Piva = Linee_Preparazioni.Piva ")
                    StrSQL.Append(" AND    Agenda.Id_Trasformazione = Trasformazioni.Id_Trasformazione ")
                    StrSQL.Append(" AND    Agenda.Preparazione_Cod = Linee_Preparazioni.Preparazione_Cod ")


                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
                    End If

                    If Id_Trasformazione <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
                    End If

                    If Linea_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Linea_Cod = " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
                    End If

                    If Preparazione_Cod <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Preparazione_Cod = " & Agro_SQL_SaveNum(Preparazione_Cod) & "   ")
                    End If

                    If Id_Agenda <> 0 Then
                        StrSQL.Append(" AND Trasformazioni.Id_Agenda = " & Agro_SQL_SaveNum(Id_Agenda) & "   ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Agenda.Inviato >=0 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato >=0 ")
                            StrSQL.Append(" AND   Trasformazioni.Inviato >=0 ")
                        Case enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Agenda.Inviato =-1 ")
                            StrSQL.Append(" AND   Linee_Preparazioni.Inviato =-1 ")
                            StrSQL.Append(" AND   Trasformazioni.Inviato =-1 ")
                        Case enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Agenda.Piva, Agenda.Validita_Fine ASC ")
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


Public Class Trasformazioni_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '########################################################################
    Public Function Scrivi(ByVal Piva As String,
                           ByVal Sa_Cod As Int32,
                           ByVal Id_Trasformazione As Int32,
                           ByVal Trasformazione_Des As String,
                           ByVal Stato As Int16,
                           ByVal Linea_Cod As Int32,
                           ByVal Preparazione_Cod As Int32,
                           ByVal Id_Agenda As Int32,
                           ByVal Cau_Mov As String,
                           ByVal Step_Giorni As Integer,
                           ByVal Ora1 As Date,
                           ByVal Ora2 As Date,
                           ByVal Ora3 As Date,
                           ByVal Ora4 As Date,
                           ByVal Colore As Int32,
                           ByVal Monitor As Integer,
                           ByVal Note As String,
                           ByVal Tipo_Stima As Int32,
                           ByVal Elem_Cod As Int32,
                           ByVal Mat_Cod As Int32,
                           ByVal Udm_Cod As Int32,
                           ByVal Stima As Decimal,
                           ByVal Validita_Inizio As Date,
                           ByVal Validita_Fine As Date,
                           ByRef objParametri As AgronicaCoreParametri
                           ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Scrivi()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append(" INSERT INTO Trasformazioni ")
            StrSQL.Append("         ( ")
            StrSQL.Append("          Piva,         Sa_Cod,            Id_Trasformazione,  Trasformazione_Des,           ")
            StrSQL.Append("          Linea_Cod,    Preparazione_Cod,  Colore,             Monitor,                      ")
            StrSQL.Append("          Stato,        Id_Agenda,         Cau_Mov,            Step_Giorni,                  ")
            StrSQL.Append("          Ora1,         Ora2,              Ora3,               Ora4,                  Note,  ")
            StrSQL.Append("          Tipo_Stima,   Elem_Cod,          Mat_Cod,            Udm_Cod,               Stima, ")

            StrSQL.Append("          Inviato,            DataInvio, ")
            StrSQL.Append("          Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("          UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("          Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("         ) ")

            StrSQL.Append(" VALUES ( ")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Trasformazione) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trasformazione_Des) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Linea_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Preparazione_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Colore) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Monitor) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Stato) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Id_Agenda) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Step_Giorni) & "  ")
            StrSQL.Append("         , " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora1)), ".", ":") & "  ")
            StrSQL.Append("         , " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora2)), ".", ":") & "  ")
            StrSQL.Append("         , " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora3)), ".", ":") & "  ")
            StrSQL.Append("         , " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora4)), ".", ":") & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Tipo_Stima) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Elem_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Mat_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Udm_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Stima) & "  ")

            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
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


    '###############################################################################
    Public Function Modifica(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Id_Trasformazione As Int32,
                             ByVal Trasformazione_Des As String,
                             ByVal Stato As Int16,
                             ByVal Linea_Cod As Int32,
                             ByVal Preparazione_Cod As Int32,
                             ByVal Id_Agenda As Int32,
                             ByVal Cau_Mov As String,
                             ByVal Step_Giorni As Int16,
                             ByVal Ora1 As Date,
                             ByVal Ora2 As Date,
                             ByVal Ora3 As Date,
                             ByVal Ora4 As Date,
                             ByVal Colore As Int32,
                             ByVal Monitor As Int16,
                             ByVal Note As String,
                             ByVal Tipo_Stima As Int32,
                             ByVal Elem_Cod As Int32,
                             ByVal Mat_Cod As Int32,
                             ByVal Udm_Cod As Int32,
                             ByVal Stima As Decimal,
                             ByVal Validita_Inizio As Date,
                             ByVal Validita_Fine As Date,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Modifica()"

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            If Id_Trasformazione = 0 Then
                Throw New Exception("Parametro non corretto nella query (Id_Trasformazione obbligatorio)")
            End If

            '---------------------------------------------

            StrSQL.Length = 0

            StrSQL.Append(" UPDATE Trasformazioni SET ")
            StrSQL.Append("    Trasformazione_Des      = '" & Agro_SQL_SaveText(Trasformazione_Des) & "'  ")
            StrSQL.Append("   ,Linea_Cod               =  " & Agro_SQL_SaveNum(Linea_Cod) & "   ")
            StrSQL.Append("   ,Preparazione_Cod        =  " & Agro_SQL_SaveNum(Preparazione_Cod) & "   ")
            StrSQL.Append("   ,Colore                  =  " & Agro_SQL_SaveNum(Colore) & "   ")
            StrSQL.Append("   ,Monitor                 =  " & Agro_SQL_SaveNum(Monitor) & "   ")
            StrSQL.Append("   ,Stato                   =  " & Agro_SQL_SaveNum(Stato) & "   ")
            StrSQL.Append("   ,Cau_Mov                 = '" & Agro_SQL_SaveText(Cau_Mov) & "'  ")
            StrSQL.Append("   ,Step_Giorni             =  " & Agro_SQL_SaveNum(Step_Giorni) & "   ")
            StrSQL.Append("   ,Ora1                    =  " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora1)), ".", ":") & "  ")
            StrSQL.Append("   ,Ora2                    =  " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora2)), ".", ":") & "  ")
            StrSQL.Append("   ,Ora3                    =  " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora3)), ".", ":") & "  ")
            StrSQL.Append("   ,Ora4                    =  " & Replace(Agro_SQL_SaveDateTime(Validita_Inizio, CStr(Ora4)), ".", ":") & "  ")
            StrSQL.Append("   ,Note                    = '" & Agro_SQL_SaveText(Note) & "'  ")
            StrSQL.Append("   ,Tipo_Stima              =  " & Agro_SQL_SaveNum(Tipo_Stima) & "   ")
            StrSQL.Append("   ,Elem_Cod                =  " & Agro_SQL_SaveNum(Elem_Cod) & "   ")
            StrSQL.Append("   ,Mat_Cod                 =  " & Agro_SQL_SaveNum(Mat_Cod) & "   ")
            StrSQL.Append("   ,Udm_Cod                 =  " & Agro_SQL_SaveNum(Udm_Cod) & "   ")
            StrSQL.Append("   ,Stima                   =  " & Agro_SQL_SaveNum(Stima) & "   ")

            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(Validita_Inizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(Validita_Fine))

            StrSQL.Append(" WHERE Piva              = '" & Agro_SQL_SaveText(Piva) & "'  ")
            StrSQL.Append(" AND   Id_Trasformazione =  " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")


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

    '#########################################################################
    Public Function Cancella(ByVal Piva As String,
                             ByVal Sa_Cod As Int32,
                             ByVal Id_Trasformazione As Int32,
                             ByVal xFiltroAggiuntivo As String,
                             ByRef objParametri As AgronicaCoreParametri
                             ) As Boolean

        Const nomeRoutine = "AgronicaCoreContabDAL.Trasformazioni_W.Cancella()"

        '====================================================================================
        'Parametri opzionali :
        '   Piva = ""
        '   Sa_Cod = 0
        '   Id_Trasformazione = 0
        '====================================================================================

        Dim messaggioErrore As String = ""
        Dim StrSQL As New Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Trasformazioni ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Inviato >= 0 ")

            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM Trasformazioni ")
                StrSQL.Append(" WHERE  1=1 ")

            End If

            If Piva <> "" Then
                StrSQL.Append(" AND Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
            End If

            If Sa_Cod <> 0 Then
                StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "   ")
            End If

            If Id_Trasformazione <> 0 Then
                StrSQL.Append(" AND Id_Trasformazione = " & Agro_SQL_SaveNum(Id_Trasformazione) & "   ")
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
