
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class PrincipiAttiviXFamigliePrincipiAttivi
    Inherits AgronicaCoreDataProvider.DataProvider

    Dim hashTable As Hashtable = New Hashtable()

    Public Function Leggi(ByVal Fam_COD As Integer,
                          ByVal PA_COD As Integer,
                          ByVal Contesto As Integer,
                          ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttiviXFamigliePrincipiAttivi.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Append(" SELECT pa_cod, Fam_Cod , FamigliePrincipiAttivi_Contesto_COD FROM  PrincipiAttiviXFamigliePrincipiAttivi " &
                                    " WHERE PrincipiAttiviXFamigliePrincipiAttivi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                                    " AND   PrincipiAttiviXFamigliePrincipiAttivi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Fam_COD <> 0 Then
                        StrSQL.Append(" AND PrincipiAttiviXFamigliePrincipiAttivi.Fam_COD =  " & Agro_SQL_SaveNum(Fam_COD) & "  ")
                    End If
                    If PA_COD <> 0 Then
                        StrSQL.Append(" AND PrincipiAttiviXFamigliePrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If
                    If Contesto <> 0 Then
                        StrSQL.Append(" AND PrincipiAttiviXFamigliePrincipiAttivi.Contesto =  " & Agro_SQL_SaveNum(Contesto) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY fam_cod, pa_cod ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT * FROM  PrincipiAttiviXFamigliePrincipiAttivi " &
                    " WHERE PrincipiAttiviXFamigliePrincipiAttivi.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " &
                    " AND   PrincipiAttiviXFamigliePrincipiAttivi.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Fam_COD <> 0 Then
                        StrSQL.Append(" AND PrincipiAttiviXFamigliePrincipiAttivi.Fam_COD =  " & Agro_SQL_SaveNum(Fam_COD) & "  ")
                    End If
                    If PA_COD <> 0 Then
                        StrSQL.Append(" AND PrincipiAttiviXFamigliePrincipiAttivi.PA_COD =  " & Agro_SQL_SaveNum(PA_COD) & "  ")
                    End If
                    If Contesto <> 0 Then
                        StrSQL.Append(" AND PrincipiAttiviXFamigliePrincipiAttivi.Contesto =  " & Agro_SQL_SaveNum(Contesto) & "  ")
                    End If


                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY fam_cod, pa_cod ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni,
                     AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta



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

    Public Function Leggi_Cached(ByVal Fam_COD As Integer,
                                 ByVal PA_COD As Integer,
                                 ByVal Contesto As Integer,
                                 ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                 ByVal xFiltroAggiuntivo As String,
                                 ByVal xOrderBy As String,
                                 ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                 ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.PrincipiAttiviXFamigliePrincipiAttivi.Leggi_Cached()"

        Dim MessaggioErrore As String = ""
        Dim DtRame As New DataTable

        Try

            Dim key As String = "key_" & Fam_COD & "_" & PA_COD & "_" & Contesto
            If hashTable.Contains(key) Then
                Return hashTable(key)
            End If

            Dim LeggiPARame As New AgronicaCoreMetaSchemaDAL.PrincipiAttiviXFamigliePrincipiAttivi
            DtRame =
                LeggiPARame.Leggi(Fam_COD,
                                  PA_COD,
                                  Contesto,
                                  xSelezioneVariabile,
                                  xFiltroAggiuntivo,
                                  xOrderBy,
                                  objParametri)


            hashTable.Add(key, DtRame)

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DtRame = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DtRame

    End Function



End Class
