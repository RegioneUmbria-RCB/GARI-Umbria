Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Stampe_Massive_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi( _
                         ByVal Piva As String, _
                         ByVal Stampa_Cod As Int32, _
                         ByVal Validita_Inizio As Date, _
                         ByVal Validita_Fine As Date, _
                             ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                             ByVal xFiltroAggiuntivo As String, _
                             ByVal xOrderBy As String, _
                             ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                             ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreStampeDAL.Stampe_Massive.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '
        '   objConnessione = nothing    =>  viene creata (e distrutta) con StringaConnessione
        '   DirectoryLOG = ""           =>  viene usato il valore di default
        '   FileLOG = ""                =>  viene usato il valore di default
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT Piva , Stampa_Cod  ")

                    StrSQL.Append(" FROM Stampe_Massive ")

                    StrSQL.Append(" WHERE Stampe_Massive.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Stampe_Massive.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND Piva LIKE '%" & Agro_SQL_SaveText(Piva) & "%' ")
                    End If
                    If Stampa_Cod <> 0 Then
                        StrSQL.Append(" AND Stampa_Cod =" & Agro_SQL_SaveNum(Stampa_Cod) & " ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stampe_Massive.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stampe_Massive.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva,Stampa_Cod ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM Stampe_Massive ")

                    StrSQL.Append(" WHERE Stampe_Massive.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   Stampe_Massive.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                   If Piva <> "" Then
                        StrSQL.Append(" AND Piva LIKE '%" & Agro_SQL_SaveText(Piva) & "%' ")
                    End If
                    If Stampa_Cod <> 0 Then
                        StrSQL.Append(" AND Stampa_Cod =" & Agro_SQL_SaveNum(Stampa_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stampe_Massive.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stampe_Massive.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Piva,Stampa_Cod ASC")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0

                    StrSQL.Append("SELECT S.Piva, S.Stampa_Cod, I.Rag_Soc FROM Stampe_Massive S INNER JOIN Imprese I ON S.Piva = I.Piva ")

                    StrSQL.Append(" WHERE S.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   S.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Piva <> "" Then
                        StrSQL.Append(" AND S.Piva LIKE '%" & Agro_SQL_SaveText(Piva) & "%' ")
                    End If
                    If Stampa_Cod <> 0 Then
                        StrSQL.Append(" AND S.Stampa_Cod =" & Agro_SQL_SaveNum(Stampa_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   S.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   S.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY I.Rag_Soc, S.Piva, S.Stampa_Cod ASC")
                    End If



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
