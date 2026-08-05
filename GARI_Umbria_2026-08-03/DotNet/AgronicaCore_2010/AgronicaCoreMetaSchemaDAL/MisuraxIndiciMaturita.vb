Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi


Public Class MisuraxIndiciMaturita_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' Da usare con Selezione_JoinCompleta
    ''' </summary>
    ''' <param name="IND_MAT_COD"></param>
    ''' <param name="UDM_COD"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni]	18/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal IND_MAT_COD As Integer,
                          ByVal UDM_COD As Integer,
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile,
                                ByVal xFiltroAggiuntivo As String,
                                ByVal xOrderBy As String,
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraxIndiciMaturita_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  MisuraxIndiciMaturita , UnitaMisura , IndiciMaturita")
                    StrSQL.Append(" WHERE MisuraxIndiciMaturita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   MisuraxIndiciMaturita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   IndiciMaturita.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   MisuraxIndiciMaturita.IND_MAT_COD = IndiciMaturita.IND_MAT_COD ")
                    StrSQL.Append(" AND   MisuraxIndiciMaturita.UDM_COD = UnitaMisura.UDM_COD ")

                    If IND_MAT_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxIndiciMaturita.IND_MAT_COD = " & Agro_SQL_SaveNum(IND_MAT_COD) & "  ")
                    End If

                    If UDM_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxIndiciMaturita.UDM_COD = " & Agro_SQL_SaveNum(UDM_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   dbo.MisuraxIndiciMaturita.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.UnitaMisura.Inviato >=0 ")
                            StrSQL.Append(" AND   dbo.IndiciMaturita.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   dbo.MisuraxIndiciMaturita.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.UnitaMisura.Inviato =-1 ")
                            StrSQL.Append(" AND   dbo.IndiciMaturita.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY MisuraxIndiciMaturita.IND_MAT_COD ASC, MisuraxIndiciMaturita.UDM_COD ASC ")
                    End If

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

    Public Function LeggiMisureIndiciMaturita(ByVal ind_mat_cod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As DataTable
        Dim nomeRoutine As String = NameOf(LeggiMisureIndiciMaturita)

        Dim messaggioErrore As String = ""
        Dim strSql As New Text.StringBuilder
        Dim dt As DataTable

        Try
            strSql.Length = 0

            strSql.AppendLine(" SELECT ")
            strSql.AppendLine("        mim.Ind_Mat_Cod, mim.Udm_Cod, im.IND_MAT_DES, um.UDM_DES")
            strSql.AppendLine(" FROM ")
            strSql.AppendLine("        MisuraxIndiciMaturita mim")
            strSql.AppendLine(" INNER JOIN")
            strSql.AppendLine("        IndiciMaturita im")
            strSql.AppendLine("        ON im.Ind_Mat_Cod = mim.IND_MAT_COD")
            strSql.AppendLine(" INNER JOIN")
            strSql.AppendLine("        UnitaMisura um")
            strSql.AppendLine("        ON um.UDM_COD = mim.Udm_Cod")
            strSql.AppendLine(" WHERE")
            strSql.AppendLine("        1=1")

            If ind_mat_cod <> -1 Then
                strSql.AppendLine("        AND mim.Ind_Mat_Cod = " & Agro_SQL_SaveNum(ind_mat_cod) & " ")
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

End Class