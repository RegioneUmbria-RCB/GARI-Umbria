Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class FormeGiuridiche_R

    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################
    Public Function Leggi( _
                            ByVal Fg_Cod As Int32, _
                            ByVal Fg_Des As String, _
                            ByVal Validita_Inizio As Date, _
                            ByVal Validita_Fine As Date, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FormeGiuridiche_R.Leggi()"

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

                    StrSQL.Append(" SELECT Fg_Cod , Fg_Des  ")

                    StrSQL.Append(" FROM FormeGiuridiche ")

                    StrSQL.Append(" WHERE FormeGiuridiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   FormeGiuridiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fg_Cod <> 0 Then
                        StrSQL.Append(" AND Fg_Cod =" & Agro_SQL_SaveNum(Fg_Cod) & " ")
                    End If

                    If Fg_Des <> "" Then
                        StrSQL.Append(" AND Fg_DES LIKE '%" & Agro_SQL_SaveText(Fg_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   FormeGiuridiche.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   FormeGiuridiche.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fg_Des ASC")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append("SELECT * FROM FormeGiuridiche ")

                    StrSQL.Append(" WHERE FormeGiuridiche.Validita_inizio <= " & Agro_SQL_SaveDate(Validita_Fine) & " ")
                    StrSQL.Append(" AND   FormeGiuridiche.Validita_Fine >= " & Agro_SQL_SaveDate(Validita_Inizio) & " ")

                    If Fg_Cod <> 0 Then
                        StrSQL.Append(" AND FormeGiuridiche.Fg_Cod =" & Agro_SQL_SaveNum(Fg_Cod) & " ")
                    End If

                    If Fg_Des <> "" Then
                        StrSQL.Append(" AND FormeGiuridiche.Fg_DES LIKE '%" & Agro_SQL_SaveText(Fg_Des) & "%' ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   FormeGiuridiche.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   FormeGiuridiche.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fg_Des ASC")
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
