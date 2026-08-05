Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class Finanziamenti_R
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Leggi(ByVal FIN_COD As Integer, _
                            ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                            ByVal xFiltroAggiuntivo As String, _
                            ByVal xOrderBy As String, _
                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                            ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaschemaDAL.Finanziamenti_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Finanziamenti ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


                    If FIN_COD <> 0 Then
                        StrSQL.Append(" AND FIN_COD =  " & Agro_SQL_SaveNum(FIN_COD) & "")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND     Inviato >= 0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND     Inviato = -1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY FIN_DES ")
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

    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="FinCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	26/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function FinDes_from_FinCod(ByVal FinCod As Integer, ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri) As String

        Dim Dt As DataTable

        'Recupero le informazioni		
        DT = Leggi(FinCod, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta, "", "", objParametri)

        If Not DT Is Nothing AndAlso DT.Rows.Count > 0 Then

            Return Dt.Rows(0).Item("fin_des")

        End If

    End Function

End Class
