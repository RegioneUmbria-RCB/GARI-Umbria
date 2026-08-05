Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider


Public Class GruppoOperazioni_R
    Inherits AgronicaCoreDataProvider.DataProvider


    '##############################################################################################
    Public Function Leggi( _
                            ByVal Gru_Cod As Long, _
                            ByVal Tipo As String, _
                            ByVal Att_Cod As Integer, _
                            ByVal Cerca_GruDes As String, _
                            ByVal Flag_OpColturali As Boolean, _
                            ByVal Flag_OpZoo As Boolean, _
                            ByVal Flag_OpMacchine As Boolean, _
                            ByVal Flag_OpContabili As Boolean, _
                                    ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreAnagrafeDAL.GruppiOperazioni_R.Leggi()"

        '====================================================================================
        'Parametri opzionali :
        '====================================================================================

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            'If Cod_Indirizzo = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Cod_Indirizzo obbligatorio)")
            'End If
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi

                    StrSQL.Length = 0
                    StrSQL.AppendLine(" SELECT   GOper.GRU_COD, coalesce(GOperXL.Gru_DES, GOper.GRU_DES) AS Gru_DES, GOper.Tipo, GOper.ATT_COD  ")

                    StrSQL.AppendLine(" FROM  GruppoOperazioni  GOper ")
                    StrSQL.AppendLine(" LEFT JOIN GruppoOperazioni_XLingua GOperXL on GOper.Gru_COD = GOperXL.Gru_COD AND GOperXL.Lingua_Cod = " & objParametri.Lingua_Cod)


                    StrSQL.AppendLine(" WHERE   GOper.Validita_Inizio <=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleFine)))
                    StrSQL.AppendLine(" AND     GOper.Validita_Fine >=" & Agro_SQL_SaveDate(CDate(objParametri.FinestraTemporaleInizio)))

                    If Gru_Cod <> 0 Then
                        StrSQL.AppendLine(" AND GOper.Gru_Cod = " & Agro_SQL_SaveNum(Gru_Cod))
                    End If

                    If Tipo <> "" Then
                        StrSQL.AppendLine(" AND GOper.Tipo = '" & Agro_SQL_SaveText(Tipo) & "' ")
                    End If

                    If Att_Cod <> 0 Then
                        StrSQL.AppendLine(" AND GOper.Att_Cod = " & Agro_SQL_SaveNum(Att_Cod))
                    End If

                    If Cerca_GruDes <> "" Then
                        StrSQL.AppendLine(" AND GOper.Gru_Des LIKE '%" & Agro_SQL_SaveText(Cerca_GruDes) & "%' ")
                    End If

                    If Flag_OpColturali = True Then
                        StrSQL.AppendLine(" AND GOper.Tipo = 'C' ")
                    End If

                    If Flag_OpZoo = True Then
                        StrSQL.AppendLine(" AND GOper.Tipo = 'Z' ")
                    End If

                    If Flag_OpMacchine = True Then
                        StrSQL.AppendLine(" AND GOper.Tipo = 'P' ")
                    End If

                    If Flag_OpContabili = True Then
                        StrSQL.AppendLine(" AND GOper.Tipo = 'E' ")
                    End If

                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.AppendLine(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.AppendLine(" AND   GOper.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.AppendLine(" AND   GOper.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.AppendLine(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.AppendLine(" ORDER BY GOper.GRU_DES ")
                    End If

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta
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




    '################################################################################
    ''' -----------------------------------------------------------------------------
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="GruCod"></param>
    ''' <param name="objParametri"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' </remarks>
    ''' <history>
    ''' 	[magnani]	28/04/2011	Created
    ''' </history>
    ''' -----------------------------------------------------------------------------
    Public Function GruOper_Des_from_GruOper_Cod(ByVal GruCod As Integer, _
                                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                                ) As String

        Dim dt As DataTable

        'Recupero le informazioni		
        dt = Leggi(CInt(GruCod), "", 0, "", False, False, False, False, AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi, "", "", objParametri)

        If Not IsNothing(dt) AndAlso dt.Rows.Count <> 0 Then

            Return dt.Rows(0).Item("gru_des")

        End If

    End Function




    '##############################################################################
    Public Function Leggi_X_Tipo(ByVal tipo As String, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByVal xOrderBy As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.GruppoOperazioni_R.Leggi_X_Tipo"



        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  GruppoOperazioni ")
            StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")


            If Not tipo.Equals("") Then
                StrSQL.Append(" AND Tipo = '" + Agro_SQL_SaveText(tipo) + "'")
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
                StrSQL.Append(" ORDER BY Tipo ")
            End If


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
