Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class AllevamentoxSpecie_R
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################

    Public Function Leggi(ByVal FORAL_COD As Long, _
                      ByVal VEG_COD As Long, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.AllevamentoxSpecie.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  FormeAllevamentoxSpecieVegetali , SpecieVegetali , FormeAllevamento ")
                    StrSQL.Append(" WHERE FormeAllevamentoxSpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   FormeAllevamentoxSpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   SpecieVegetali.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   FormeAllevamento.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   FormeAllevamento.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   FormeAllevamentoxSpecieVegetali.FORAL_COD = FormeAllevamento.FORAL_COD ")
                    StrSQL.Append(" AND   FormeAllevamentoxSpecieVegetali.VEG_COD = SpecieVegetali.VEG_COD ")


                    If FORAL_COD <> 0 Then
                        StrSQL.Append(" AND FormeAllevamentoxSpecieVegetali.FORAL_COD =  " & Agro_SQL_SaveNum(FORAL_COD) & "  ")
                    End If

                    If VEG_COD <> 0 Then
                        StrSQL.Append(" AND FormeAllevamentoxSpecieVegetali.VEG_COD =  " & Agro_SQL_SaveNum(VEG_COD) & "  ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     FormeAllevamentoxSpecieVegetali.Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     FormeAllevamentoxSpecieVegetali.Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY SpecieVegetali.VEG_DES ASC, FormeAllevamento.FORAL_DES ASC ")
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
    Public Function ForalDes_from_ForalCod(ByVal ForalCod As Integer, _
                                            ByVal VegCod As Integer, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As String

        Dim NomeRoutine As String = "AgronicaCoreMetaSchema.Istat_R.ForalDes_from_ForalCod()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable


        'Recupero le informazioni		


        Dim objCOM As New AgronicaCoreMetaSchemaDAL.AllevamentoxSpecie_R


        DT = objCOM.Leggi(CInt(ForalCod), _
                            CInt(VegCod), _
                            AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                            "", _
                            "", _
                            objParametri)



        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

            Return DT.Rows(0).Item("foral_des")

        End If
        Return ""

    End Function


End Class





