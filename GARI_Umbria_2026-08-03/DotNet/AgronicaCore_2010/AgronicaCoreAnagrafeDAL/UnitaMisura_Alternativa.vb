Imports System.Text
Imports AgronicaCoreDataProvider.AgronicaCoreParametri
Imports AgronicaCoreDataProvider

Public Class UnitaMisura_Alternativa_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="Udm_From"></param>
    ''' <param name="Udm_Alt"></param>
    ''' <param name="xSelezioneVariabile"></param>
    ''' <param name="xFiltroAggiuntivo"></param>
    ''' <param name="xOrderBy"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    Public Function Leggi(ByVal Udm_From As Integer,
                          ByVal Udm_Alt As Integer,
                          ByVal xSelezioneVariabile As enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AnagrafeDAL.UnitaMisura_Alternativa_Read.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New StringBuilder
        Dim DT As DataTable

        Try
            StrSQL.Length = 0

            Select Case xSelezioneVariabile
                Case enumSelezioneVariabile.Selezione_TabellaDatiMinimi, enumSelezioneVariabile.Selezione_TabellaCompleta

                    'SELECT
                    StrSQL.AppendLine("SELECT UDM_COD_FROM, UDM_COD_ALT_TO, Tasso_Conv, DATA_AGG, Validita_Inizio, Validita_Fine, ")
                    StrSQL.AppendLine("       Data_Creazione, Data_Modifica, Username_Creazione, Username_Modifica, Inviato, DataInvio ")
                    StrSQL.AppendLine("FROM Conversione_UnitaMisura_Alternative ")

                    'WHERE
                    StrSQL.AppendLine("WHERE Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("      AND Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Not IsNothing(Udm_From) AndAlso Udm_From <> 0 Then
                        StrSQL.AppendLine("      AND UDM_COD_FROM = " & Agro_SQL_SaveNum(Udm_From) & " ")
                    End If

                    If Not IsNothing(Udm_Alt) AndAlso Udm_Alt <> 0 Then
                        StrSQL.AppendLine("      AND UDM_COD_ALT_TO = " & Agro_SQL_SaveNum(Udm_Alt) & " ")
                    End If

                    If Not IsNothing(xFiltroAggiuntivo) AndAlso xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine("      AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine("      AND Inviato >=0 ")
                        Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine("      AND Inviato =-1 ")
                        Case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    'ORDER BY
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If

                Case enumSelezioneVariabile.Selezione_JoinDescrizioni, enumSelezioneVariabile.Selezione_JoinCompleta
                    'SELECT
                    StrSQL.AppendLine("SELECT ConversioneUdm.UDM_COD_FROM AS UDM_FROM, ")
                    StrSQL.AppendLine("       ConversioneUdm.UDM_COD_ALT_TO AS UDM_ALT, ")
                    StrSQL.AppendLine("       UnitaMisura.UDM_DES AS UDM_DES_FROM, ")
                    StrSQL.AppendLine("       UnitaMisura.UDM_SIM AS UDM_SIM_FROM, ")
                    StrSQL.AppendLine("       UnitaMisura_Alternative.UDM_DES AS UDM_DES_ALT, ")
                    StrSQL.AppendLine("       UnitaMisura_Alternative.UDM_SIM AS UDM_SIM_ALT, ")
                    StrSQL.AppendLine("       ConversioneUdm.Tasso_Conv, ")
                    StrSQL.AppendLine("       ConversioneUdm.DATA_AGG, ConversioneUdm.Validita_Inizio, ConversioneUdm.Validita_Fine, ")
                    StrSQL.AppendLine("       ConversioneUdm.Data_Creazione, ConversioneUdm.Data_Modifica, ")
                    StrSQL.AppendLine("       ConversioneUdm.Username_Creazione, ConversioneUdm.Username_Modifica, ")
                    StrSQL.AppendLine("       ConversioneUdm.Inviato, ConversioneUdm.DataInvio ")
                    StrSQL.AppendLine("FROM Conversione_UnitaMisura_Alternative AS ConversioneUdm ")

                    'JOIN
                    StrSQL.AppendLine("INNER JOIN UnitaMisura ON UnitaMisura.UDM_COD = ConversioneUdm.UDM_COD_FROM ")
                    StrSQL.AppendLine("INNER JOIN UnitaMisura_Alternative ON UnitaMisura_Alternative.UDM_COD_ALT = ConversioneUdm.UDM_COD_ALT_TO ")

                    'WHERE
                    StrSQL.AppendLine("WHERE ConversioneUdm.Validita_inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.AppendLine("      AND ConversioneUdm.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If Not IsNothing(Udm_From) AndAlso Udm_From <> 0 Then
                        StrSQL.AppendLine("      AND ConversioneUdm.UDM_COD_FROM = " & Agro_SQL_SaveNum(Udm_From) & " ")
                    End If

                    If Not IsNothing(Udm_Alt) AndAlso Udm_Alt <> 0 Then
                        StrSQL.AppendLine("      AND ConversioneUdm.UDM_COD_ALT_TO = " & Agro_SQL_SaveNum(Udm_Alt) & " ")
                    End If

                    If Not IsNothing(xFiltroAggiuntivo) AndAlso xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine("      AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri) & " ")
                    End If

                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine("      AND ConversioneUdm.Inviato >=0 ")
                        Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine("      AND ConversioneUdm.Inviato =-1 ")
                        Case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------

                    'ORDER BY
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine("ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
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

End Class

Public Class UnitaMisura_Alternativa_Write
    Inherits AgronicaCoreDataProvider.DataProvider

End Class

