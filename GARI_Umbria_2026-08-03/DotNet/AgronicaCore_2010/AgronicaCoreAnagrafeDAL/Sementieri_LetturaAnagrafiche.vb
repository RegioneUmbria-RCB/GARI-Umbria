Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider



Public Class Sementieri_LetturaAnagrafiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiImpreseMovimentateDatoUsername( _
                                ByVal UserName As String, _
                                ByVal Validita_Inizio As DateTime, _
                                ByVal Validita_Fine As DateTime, _
                                        ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                        ByVal xOrderBy As String, _
                                        ByRef objParametri_server As AgronicaCoreDataProvider.AgronicaCoreParametri, _
                                        ByRef objParametri_utenti As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                        ) As DataTable

        Dim NomeRoutine As String = "AnagrafeCoreAnagrafeDAL.Sementieri_LetturaAnagrafiche_R.LeggiImpreseMovimentateDatoUsername()"

        Dim MessaggioErrore As String = ""
        Dim Stb As New System.Text.StringBuilder
        Dim DT_Utenti As DataTable
        Dim DT As DataTable
        Dim i As Integer
        i = 0
        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi



                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    Stb.Length = 0
                    Stb.Append(" SELECT  piva ")
                    Stb.Append("     FROM    Utenti_Dettagli ")
                    Stb.Append(" where     [Username] = '" & Agro_SQL_SaveText(UserName) & "'   ")



                    '--------------------------------------------------------------------------
                    DT_Utenti = EseguiQuery_Lettura(objParametri_utenti, Stb.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------

                    Dim piva As String = DT_Utenti.Rows(0)("Piva")
                    Stb.Length = 0

                    'Stb.Append("exec sp_executesql N'" & vbCrLf)


                    Stb.Append(" select * " & vbCrLf)
                    Stb.Append(" from ( " & vbCrLf)
                    Stb.Append("    select 1 as ordine, '" & Agro_SQL_SaveText(piva) & "' as padre, i.piva as figlio, i.rag_soc " & vbCrLf)
                    Stb.Append("    from imprese i " & vbCrLf)
                    Stb.Append("    where i.piva=  '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                    Stb.Append("    union all " & vbCrLf)

                    Stb.Append("    select distinct 2 as ordine, '" & Agro_SQL_SaveText(piva) & "' as padre, i.piva as figlio, i.rag_soc " & vbCrLf)
                    Stb.Append("    from imprese i " & vbCrLf)
                    Stb.Append("        inner join reg_impianti r on i.piva = r.piva " & vbCrLf)
                    Stb.Append("    where r.codice_fiscale_tecnico =  '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                    Stb.Append("    and  " & Agro_SQL_SaveDate(Validita_Inizio) & " <= r.Validita_Fine " & vbCrLf)
                    Stb.Append("    and " & Agro_SQL_SaveDate(Validita_Fine) & "  >= r.validita_fine " & vbCrLf)

                    Stb.Append("    union " & vbCrLf)

                    Stb.Append("    select distinct 2 as ordine, '" & Agro_SQL_SaveText(piva) & "' as padre, i.piva as figlio, i.rag_soc " & vbCrLf)
                    Stb.Append("    from imprese i " & vbCrLf)
                    Stb.Append("        inner join programmazione_Entita r on i.piva = r.piva " & vbCrLf)
                    Stb.Append("    where r.codice_fiscale_tecnico =  '" & Agro_SQL_SaveText(piva) & "' " & vbCrLf)
                    Stb.Append("    and  " & Agro_SQL_SaveDate(Validita_Inizio) & " <= r.Validita_Fine " & vbCrLf)
                    Stb.Append("    and " & Agro_SQL_SaveDate(Validita_Fine) & "  >= r.validita_fine " & vbCrLf)
                    Stb.Append(" ) aa order by aa.ordine, aa.rag_soc  " & vbCrLf)

                    'Stb.Append("'," & vbCrLf)

                    ''parametri, dichiarazione
                    'Stb.Append(" N'@Piva varchar(50), @validita_inizio datetime, @validita_fine datetime" & vbCrLf)

                    ''parametri, assegnazione
                    'Stb.Append(", @piva='" & piva & "' " & vbCrLf)
                    'Stb.Append(", @validita_inizio=" & Agro_SQL_SaveDate(Validita_Inizio) & " " & vbCrLf)
                    'Stb.Append(", @validita_fine=" & Agro_SQL_SaveDate(Validita_Fine) & " " & vbCrLf)




                    '--------------------------------------------------------------------------
                    DT = EseguiQuery_Lettura(objParametri_server, Stb.ToString, NomeRoutine)
                    '--------------------------------------------------------------------------


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



            
        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try


        Return DT

    End Function




End Class
