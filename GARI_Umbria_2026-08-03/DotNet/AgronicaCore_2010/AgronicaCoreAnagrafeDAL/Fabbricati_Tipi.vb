Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Fabbricati_Tipi_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi( _
                        ByVal Tipo_Fabbricato_Cod As Long, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Tipi_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    'TODO
                    'modificare la query
                    '---------------------------------------------
                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT   * ")
                    StrSQL.Append(" FROM     Fabbricati_Tipi ")
                    StrSQL.Append(" WHERE    Fabbricati_Tipi.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND      Fabbricati_Tipi.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If Tipo_Fabbricato_Cod <> 0 Then
                        StrSQL.Append(" AND  Fabbricati_Tipi.Tipo_Fabbricato_Cod = " & Agro_SQL_SaveNum(Tipo_Fabbricato_Cod) & " ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   (Fabbricati_Tipi.inviato >= 0) ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   (Fabbricati_Tipi.inviato =-1) ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY Fabbricati_Tipi.Tipo_Fabbricato_Des")
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




    '##############################################################################################
    Public Function TipoFabbricatoDes_from_TipoFabbricatoCod( _
                        ByVal TipoFabbricatoCod As Integer, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As String

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Fabbricati_Tipi_R.TipoFabbricatoDes_from_TipoFabbricatoCod()"
        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable
        Dim Tipo_Fabbricato_Des As String = ""


        DT = Leggi(TipoFabbricatoCod, _
                    AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, _
                    "", _
                    "", _
                    objParametri)

        If Not IsNothing(DT) AndAlso DT.Rows.Count > 0 Then
            Tipo_Fabbricato_Des = DT.Rows(0).Item("Tipo_Fabbricato_Des")
        End If

        Return Tipo_Fabbricato_Des

    End Function



End Class



'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

