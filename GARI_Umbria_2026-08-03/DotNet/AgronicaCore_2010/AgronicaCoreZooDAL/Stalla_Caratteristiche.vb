Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider

Public Class Stalla_Caratteristiche_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi( _
                            ByVal PIVA As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal STA_NUM As Integer, _
                            ByVal Cod_Fabb As String, _
                            ByVal Att_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Caratteristiche_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi
                    '
                    '
                    '
                    '


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT * ")
                    StrSQL.Append(" FROM  Stalla_Caratteristiche ")
                    StrSQL.Append(" WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
                    StrSQL.Append(" AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

                    If PIVA <> "" Then
                        StrSQL.Append(" AND PIVA = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                    End If

                    If Sa_Cod <> 0 Then
                        StrSQL.Append(" AND Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                    End If

                    If STA_NUM <> 0 Then
                        StrSQL.Append(" AND STA_NUM = " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                    End If

                    If Cod_Fabb <> "" Then
                        StrSQL.Append(" AND COD_FABB = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "'  ")
                    End If

                    If Att_Cod <> 0 Then
                        StrSQL.Append(" AND ATT_COD = " & Agro_SQL_SaveNum(Att_Cod) & "  ")
                    End If


                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   Stalla_Caratteristiche.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    Else
                        StrSQL.Append(" ORDER BY ATT_COD ASC ")
                    End If
                    '------------------------------------------------------------------
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


End Class



'############################################



Public Class Stalla_Caratteristiche_W
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Cancella( _
                                ByVal PIVA As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal STA_NUM As Integer, _
                                ByVal Cod_Fabb As String, _
                                ByVal Att_Cod As Integer, _
                                    ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Caratteristiche_W.Cancella()"


        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = AgronicaCoreDataProvider.AgronicaCoreParametri.enumCancellazioneLogica.CancellazioneLogica Then
                StrSQL.Length = 0

                StrSQL.Append(" UPDATE   Stalla_Caratteristiche ")
                StrSQL.Append(" SET ")
                StrSQL.Append("      ,Username_Modifica = '" & objParametri.UsernameOperazione & "' ")
                StrSQL.Append("      ,Inviato = -1 ")
                StrSQL.Append(" WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND   sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                StrSQL.Append(" AND   STA_NUM   =  " & Agro_SQL_SaveNum(STA_NUM) & "  ")
                
            Else
                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM     Stalla_Caratteristiche ")
                StrSQL.Append(" WHERE PIVA      = '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
                StrSQL.Append(" AND   sa_cod    =  " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
                StrSQL.Append(" AND   STA_NUM   =  " & Agro_SQL_SaveNum(STA_NUM) & "  ")
            End If

            If Cod_Fabb <> "" Then
                StrSQL.Append(" AND COD_FABB = '" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "'  ")
            End If

            If Att_Cod <> 0 Then
                StrSQL.Append(" AND ATT_COD = " & Agro_SQL_SaveNum(Att_Cod) & "  ")
            End If

            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function


    Public Function Scrivi( _
                                ByVal PIVA As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal STA_NUM As Integer, _
                                ByVal Cod_Fabb As String, _
                                ByVal Att_Cod As Integer, _
                                ByVal Valore As Decimal, _
                                    ByVal Validita_Inizio As Date, _
                                    ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreZooDAL.Stalla_Caratteristiche_W.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO Stalla_Caratteristiche(PIVA, Sa_Cod, STA_NUM, ")
            StrSQL.Append("                    COD_FABB, ATT_COD, VALORE, ")
            StrSQL.Append("                    Inviato, DataInvio, ")
            StrSQL.Append("                    Data_Creazione,     Data_Modifica, ")
            StrSQL.Append("                    UserName_Creazione, UserName_Modifica, ")
            StrSQL.Append("                    Validita_Inizio,    Validita_Fine ")
            StrSQL.Append("                    ) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(PIVA)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(STA_NUM) & " ")

            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(Cod_Fabb)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Att_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Valore) & "  ")
            StrSQL.Append("         , 0  ")
            StrSQL.Append("         , Null  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Date.Now) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Inizio) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveDate(Validita_Fine) & "  ")
            StrSQL.Append(")")

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            xRisp = False
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return xRisp

    End Function

End Class
