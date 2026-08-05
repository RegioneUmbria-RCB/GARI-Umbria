
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class GruppiOperazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    ''##############################################################################################
    'Public Function Leggi( _
    '                        ByVal Gru_Cod As Long, _
    '                        ByVal Tipo As String, _
    '                        ByVal Att_Cod As Integer, _
    '                        ByVal Cerca_GruDes As String, _
    '                        ByVal Flag_OpColturali As Boolean, _
    '                        ByVal Flag_OpZoo As Boolean, _
    '                        ByVal Flag_OpMacchine As Boolean, _
    '                        ByVal Flag_OpContabili As Boolean, _
    '                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
    '                                ByVal xFiltroAggiuntivo As String, _
    '                                ByVal xOrderBy As String, _
    '                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                ) As DataTable

    '    Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.GruppiOperazioni_R.Leggi()"

    '    '====================================================================================
    '    'Parametri opzionali :
    '    '====================================================================================

    '    Dim MessaggioErrore As String = ""
    '    Dim StrSQL As New System.Text.StringBuilder
    '    Dim DT As DataTable

    '    Try

    '        'If Cod_Indirizzo = 0 Then
    '        '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
    '        'End If
    '        Select Case xSelezioneVariabile

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

    '                StrSQL.Length = 0
    '                StrSQL.Append(" SELECT   GruppoOperazioni.GRU_COD, GruppoOperazioni.GRU_DES, GruppoOperazioni.Tipo, GruppoOperazioni.ATT_COD  ")

    '                StrSQL.Append(" FROM    GruppoOperazioni ")

    '                StrSQL.Append(" WHERE   GruppoOperazioni.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)))
    '                StrSQL.Append(" AND     GruppoOperazioni.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)))

    '                If Gru_Cod <> 0 Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
    '                End If

    '                If Tipo <> "" Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
    '                End If

    '                If Att_Cod <> 0 Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Att_Cod = " & Agro_SQL_SaveNum(Att_Cod))
    '                End If

    '                If Cerca_GruDes <> "" Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Gru_Des LIKE '%" & Agro_SQL_SaveText(Cerca_GruDes) & "%' ")
    '                End If

    '                If Flag_OpColturali = True Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Tipo = 'C' ")
    '                End If

    '                If Flag_OpZoo = True Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Tipo = 'Z' ")
    '                End If

    '                If Flag_OpMacchine = True Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Tipo = 'P' ")
    '                End If

    '                If Flag_OpContabili = True Then
    '                    StrSQL.Append(" AND GruppoOperazioni.Tipo = 'E' ")
    '                End If

    '                If xFiltroAggiuntivo <> "" Then
    '                    StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
    '                End If
    '                '--------------------------------------------------------------------------
    '                Select Case objParametri.FlagVisibilita
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
    '                        StrSQL.Append(" AND   Inviato >=0 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
    '                        StrSQL.Append(" AND   Inviato =-1 ")
    '                    Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
    '                        '...................................
    '                    Case Else
    '                        Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
    '                End Select
    '                '--------------------------------------------------------------------------
    '                If xOrderBy <> "" Then
    '                    strSql.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
    '                Else
    '                    StrSQL.Append(" ORDER BY GruppoOperazioni.GRU_DES ")
    '                End If

    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni
    '            Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinCompleta


    '        End Select


    '        '--------------------------------------------------------------------------
    '        DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
    '        '--------------------------------------------------------------------------

    '    Catch ex As Exception
    '        MessaggioErrore = ex.Message
    '        Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
    '        DT = Nothing
    '        Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
    '    End Try

    '    Return DT

    'End Function




    ''################################################################################
    '''' -----------------------------------------------------------------------------
    '''' <summary>
    '''' 
    '''' </summary>
    '''' <param name="GruCod"></param>
    '''' <param name="objParametri"></param>
    '''' <returns></returns>
    '''' <remarks>
    '''' </remarks>
    '''' <history>
    '''' 	[magnani]	28/04/2011	Created
    '''' </history>
    '''' -----------------------------------------------------------------------------
    'Public Function GruOper_Des_from_GruOper_Cod(ByVal GruCod As Integer, _
    '                                            ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
    '                                            ) As String

    '    Dim dt As DataTable

    '    'Recupero le informazioni		
    '    dt = Leggi(CInt(GruCod), "", 0, "", False, False, False, False, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

    '    If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

    '        Return dt.Rows(0).Item("gru_des")

    '    End If

    'End Function






End Class

