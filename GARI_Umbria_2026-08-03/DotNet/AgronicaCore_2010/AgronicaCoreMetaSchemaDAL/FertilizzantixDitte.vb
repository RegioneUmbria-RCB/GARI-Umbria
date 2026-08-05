Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class FertilizzantixDitte_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '#############################################################################################
    Public Function Leggi( _
                            ByVal DITTA_COD As Integer, _
                            ByVal FER_COD As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.FertilizzantixDitte_R.Leggi()"

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
                    '
                    '
                    '
                    '

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '---------------------------------------------
                    StrSQL.Length = 0


                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  FertilizzantixDitte , Fertilizzanti , Ditte ")
                    StrSQL.Append(" WHERE FertilizzantixDitte.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   FertilizzantixDitte.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Fertilizzanti.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Ditte.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Ditte.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   FertilizzantixDitte.DITTA_COD = Ditte.DITTA_COD ")
                    StrSQL.Append(" AND   FertilizzantixDitte.FER_COD = Fertilizzanti.FER_COD ")

                    If DITTA_COD <> 0 Then
                        StrSQL.Append(" AND FertilizzantixDitte.DITTA_COD =  " & Agro_SQL_SaveNum(DITTA_COD) & "  ")
                    End If

                    If FER_COD <> 0 Then
                        StrSQL.Append(" AND FertilizzantixDitte.FER_COD =  " & Agro_SQL_SaveNum(FER_COD) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   FertilizzantixDitte.Inviato >=0 ")
                            StrSQL.Append(" AND   Fertilizzanti.Inviato >=0 ")
                            StrSQL.Append(" AND   Ditte.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   FertilizzantixDitte.Inviato =-1 ")
                            StrSQL.Append(" AND   Fertilizzanti.Inviato =-1 ")
                            StrSQL.Append(" AND   Ditte.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fertilizzanti.FER_DES ASC, Ditte.DITTA_DES ASC")
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
