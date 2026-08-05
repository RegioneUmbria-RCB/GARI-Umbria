Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

' 
Public Class Analisi_Entita_Read
    Inherits AgronicaCoreDataProvider.DataProvider



    '##############################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' da utilizzare con Selezione_TabellaCompleta
    ''' </summary>
    ''' <param name="Analisi_Parametro_Cod">Analisi_Parametro_Cod di default = 0</param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[pierantoni] 05/01/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function Leggi(ByVal Analisi_Entita_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.Analisi_Entita_Read.Leggi()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
                    '------------------------------

                    'Query per il prelievo dei dati                 ' #### CLASSE ####

                    StrSQL.Append(" SELECT Analisi_Entita.* ")
                    StrSQL.Append(" FROM   Analisi_Entita ")
                    StrSQL.Append(" WHERE  Analisi_Entita.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND    Analisi_Entita.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Analisi_Entita_Cod <> 0 Then
                        StrSQL.Append(" AND Analisi_Entita.Analisi_Entita_Cod = " & Analisi_Entita_Cod & " ")
                    End If

                    '------------------------------

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta
                    
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