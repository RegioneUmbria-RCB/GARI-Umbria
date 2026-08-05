Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class ConduzioneTraFila_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    'TECN_COD=0 è significativo
    'se non si vuole filtrare, passare -1
    Public Function Leggi(ByVal TECN_COD As Long, _
                            ByVal Gru_Cod As Long, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.ConduzioneTraFila_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  ConduzioneTerrenoTraFila , GruppoVegetale ")
                    StrSQL.Append(" WHERE GruppoVegetale.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   GruppoVegetale.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If TECN_COD <> -1 Then
                        StrSQL.Append(" AND ConduzioneTerrenoTraFila.TECN_COD =  " & Agro_SQL_SaveNum(TECN_COD) & "  ")
                    End If

                    If Gru_Cod <> 0 Then
                        StrSQL.Append(" AND ConduzioneTerrenoTraFila.GRU_COD =  " & Agro_SQL_SaveNum(Gru_Cod) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     GruppoVegetale.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     GruppoVegetale.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append("  ORDER BY GruppoVegetale.GRU_DES ASC, ConduzioneTerrenoTraFila.TECN_DES asc")
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


    '##############################################################################################
    Public Function TecnDes_from_TecnCod_TRA(ByVal TecnCod As Integer, _
                                             ByVal GruCod As Integer, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchema.ConduzioneTraFila.TecnDes_From_TecnCod_TRA()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        'Recupero le informazioni		


        Dim objCOM As New AgronicaCoreMetaSchemaDAL.ConduzioneTraFila_R


        DT = objCOM.Leggi(CInt(TecnCod), _
                    CInt(GruCod), _
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    "", _
                    "", _
                    objParametri)



        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

            Return DT.Rows(0).Item("tecn_des")

        End If
        Return ""

    End Function





End Class





