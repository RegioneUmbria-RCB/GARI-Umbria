Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.DataProviderExtensions

Public Class Programmazione_Entita_Codici_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi( _
                            ByVal Programmazione_Entita_Cod As Int32, _
                            ByVal Id_Cod As Int32, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '   ID_Cod = 0                  =>  si leggono tutti i codici
        '
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    'TODO
                    'modificare la query di selecet
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT ")
                    StrSQL.Append("         Programmazione_Entita_Codici.id_cod,  ")
                    StrSQL.Append("         Programmazione_Entita_Codici.val_cod,  ")
                    StrSQL.Append("         Programmazione_Entita_Codici.Validita_Inizio, Programmazione_Entita_Codici.Validita_Fine ")

                    StrSQL.Append(" FROM    Programmazione_Entita_Codici ")

                    StrSQL.Append(" WHERE   (Programmazione_Entita_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Programmazione_Entita_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    StrSQL.Append(" AND     (Programmazione_Entita_Codici.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser).Trim & "')  ")
                    StrSQL.Append(" AND     (Programmazione_Entita_Codici.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & ")  ")

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND     (Programmazione_Entita_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ") ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     (Programmazione_Entita_Codici.inviato >= 0) ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     (Programmazione_Entita_Codici.inviato = -1) ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Programmazione_Entita_Codici.Validita_Inizio ASC")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    'TODO
                    'modificare la query di selecet
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT ")
                    StrSQL.Append("         Programmazione_Entita_Codici.id_cod,  ")
                    StrSQL.Append("         Programmazione_Entita_Codici.val_cod,  ")
                    StrSQL.Append("         Programmazione_Entita_Codici.Validita_Inizio, Programmazione_Entita_Codici.Validita_Fine ")

                    StrSQL.Append(" FROM    Programmazione_Entita_Codici ")

                    StrSQL.Append(" WHERE   (Programmazione_Entita_Codici.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & ")  ")
                    StrSQL.Append(" AND     (Programmazione_Entita_Codici.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & ")  ")

                    StrSQL.Append(" AND     (Programmazione_Entita_Codici.Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser).Trim & "')  ")
                    StrSQL.Append(" AND     (Programmazione_Entita_Codici.Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & ")  ")

                    If Id_Cod <> 0 Then
                        StrSQL.Append(" AND     (Programmazione_Entita_Codici.id_cod = " & Agro_SQL_SaveNum(Id_Cod) & ") ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     (Programmazione_Entita_Codici.inviato >= 0) ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     (Programmazione_Entita_Codici.inviato = -1) ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Programmazione_Entita_Codici.Validita_Inizio ASC")
                    End If



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    '
                    '
                    '
                    '


            End Select



            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function



End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################



Public Class Programmazione_Entita_Codici_W
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi(
                           ByVal Programmazione_Entita_Cod As Int32,
                           ByVal Id_Cod As Int32,
                           ByVal Val_Cod As String,
                            ByVal Validita_Inizio As Date,
                            ByVal Validita_Fine As Date,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W.Scrivi()"

        '====================================================================================
        'Parametri opzionali :
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine("INSERT INTO Programmazione_Entita_Codici( ")
            StrSQL.AppendLine("                    Piva_SuperUser,      ")
            StrSQL.AppendLine("                    Programmazione_Entita_Cod,    ")
            StrSQL.AppendLine("                    Id_Cod,    ")
            StrSQL.AppendLine("                    Val_Cod,    ")
            StrSQL.AppendLine("                    Inviato, DataInvio, ")
            StrSQL.AppendLine("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.AppendLine("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.AppendLine("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.AppendLine("                    ) ")
            StrSQL.AppendLine("VALUES (")
            StrSQL.AppendLine("          '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveNum(Id_Cod) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(Val_Cod) & "' ")
            StrSQL.AppendLine("         , 0  ")
            StrSQL.AppendLine("         , Null  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.AppendLine("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.AppendLine(")")
            '---------------------------------------------

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Modifica(
                                ByVal Programmazione_Entita_Cod As Int32,
                                ByVal Id_Cod As Int32,
                                ByVal Val_Cod As String,
                                ByVal Validita_Inizio As Date,
                                ByVal Validita_Fine As Date,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W.Modifica()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.Append("UPDATE Programmazione_Entita_Codici SET ")
            StrSQL.Append("    Val_Cod           = '" & Agro_SQL_SaveText(Val_Cod) & "'")
            StrSQL.Append("   ,Inviato           =  0 ")
            StrSQL.Append("   ,DataInvio         =  Null ")
            StrSQL.Append("   ,Data_Modifica     =  " & Agro_SQL_SaveDate(Date.Now))
            StrSQL.Append("   ,UserName_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "'")
            StrSQL.Append("   ,Validita_Inizio   =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio))
            StrSQL.Append("   ,Validita_Fine     =  " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine))
            StrSQL.Append(" WHERE Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "'")
            StrSQL.Append(" AND   Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            StrSQL.Append(" AND   Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            '---------------------------------------------


            '----------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    '##############################################################################################
    Public Function Cancella(
                            ByVal Programmazione_Entita_Cod As Int32,
                            ByVal Id_Cod As Int32,
                                ByVal xFiltroAggiuntivo As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Programmazione_Entita_Codici_W.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try


            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE Programmazione_Entita_Codici ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE  Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")
                StrSQL.Append(" AND Inviato >= 0")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Programmazione_Entita_Codici ")
                StrSQL.Append(" WHERE    Piva_SuperUser = '" & Agro_SQL_SaveText(objParametri.PivaSuperUser) & "' ")


            End If

            If Id_Cod <> 0 Then
                StrSQL.Append(" AND Id_Cod = " & Agro_SQL_SaveNum(Id_Cod) & " ")
            End If

            If Programmazione_Entita_Cod <> 0 Then
                StrSQL.Append(" AND   Programmazione_Entita_Cod = " & Agro_SQL_SaveNum(Programmazione_Entita_Cod) & " ")
            End If


            '---------------------------------------------



            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function



    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class

