Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

''' -----------------------------------------------------------------------------
''' Project	 : AgronicaCoreMetaSchemaDAL
''' Class	 : MisuraxAusiliari_W
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[magnani]	14/04/2011	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class MisuraxAusiliari_W
    Inherits AgronicaCoreDataProvider.DataProvider
    'in scrittura uso la finestra in lettura ricavo da objparametri
    'ByVal FinestraTemp_Inizio As Date, _
    'ByVal FinestraTemp_Fine As Date, _
    'FinestraTemp_Inizio As Date = CDate("01/01/1900")
    'FinestraTemp_Fine As Date = CDate("31/12/2100")
End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################

''' -----------------------------------------------------------------------------
''' Project	 : AgronicaCoreMetaSchemaDAL
''' Class	 : MisuraxAusiliari_R
''' 
''' -----------------------------------------------------------------------------
''' <summary>
''' 
''' </summary>
''' <remarks>
''' </remarks>
''' <history>
''' 	[magnani]	14/04/2011	Created
''' </history>
''' -----------------------------------------------------------------------------
Public Class MisuraxAusiliari_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal AUS_COD As Long, _
                                    ByVal UDM_COD As Integer, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.MisuraxAusiliari_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Dim FinestraTemp_Inizio As Date = objParametri.FinestraTemporaleInizio
        Dim FinestraTemp_Fine As Date = objParametri.FinestraTemporaleFine
        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  MisuraxAusiliari , UnitaMisura , Ausiliari ")
                    StrSQL.Append(" WHERE MisuraxAusiliari.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   MisuraxAusiliari.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Ausiliari.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Ausiliari.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   MisuraxAusiliari.AUS_COD = Ausiliari.AUS_COD ")
                    StrSQL.Append(" AND   MisuraxAusiliari.UDM_COD = UnitaMisura.UDM_COD ")

                    If AUS_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxAusiliari.AUS_COD = " & Agro_SQL_SaveNum(AUS_COD) & "  ")
                    End If

                    If UDM_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxAusiliari.UDM_COD = " & Agro_SQL_SaveNum(UDM_COD) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY MisuraxAusiliari.AUS_COD ASC, MisuraxAusiliari.UDM_COD ASC ")
                    End If


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  MisuraxAusiliari , UnitaMisura , Ausiliari ")
                    StrSQL.Append(" WHERE MisuraxAusiliari.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   MisuraxAusiliari.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Ausiliari.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Ausiliari.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   MisuraxAusiliari.AUS_COD = Ausiliari.AUS_COD ")
                    StrSQL.Append(" AND   MisuraxAusiliari.UDM_COD = UnitaMisura.UDM_COD ")

                    If AUS_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxAusiliari.AUS_COD = " & Agro_SQL_SaveNum(AUS_COD) & "  ")
                    End If

                    If UDM_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxAusiliari.UDM_COD = " & Agro_SQL_SaveNum(UDM_COD) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY MisuraxAusiliari.AUS_COD ASC, MisuraxAusiliari.UDM_COD ASC ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Length = 0
                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  MisuraxAusiliari , UnitaMisura , Ausiliari ")
                    StrSQL.Append(" WHERE MisuraxAusiliari.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   MisuraxAusiliari.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   UnitaMisura.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   Ausiliari.Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Ausiliari.Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")
                    StrSQL.Append(" AND   MisuraxAusiliari.AUS_COD = Ausiliari.AUS_COD ")
                    StrSQL.Append(" AND   MisuraxAusiliari.UDM_COD = UnitaMisura.UDM_COD ")

                    If AUS_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxAusiliari.AUS_COD = " & Agro_SQL_SaveNum(AUS_COD) & "  ")
                    End If

                    If UDM_COD <> 0 Then
                        StrSQL.Append(" AND MisuraxAusiliari.UDM_COD = " & Agro_SQL_SaveNum(UDM_COD) & "  ")
                    End If

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------

                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select

                    '--------------------------------------------------------------------------

                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY MisuraxAusiliari.AUS_COD ASC, MisuraxAusiliari.UDM_COD ASC ")
                    End If

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
