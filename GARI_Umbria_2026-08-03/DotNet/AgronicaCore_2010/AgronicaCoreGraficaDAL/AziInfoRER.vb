Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider.AgronicaCoreParametri

Public Class AziInfoRER_Write
    Inherits AgronicaCoreDataProvider.DataProvider

    '##############################################################################################
    Public Function Scrivi( _
                            ByVal Piva As String, _
                            ByVal Sa_Cod As Integer, _
                            ByVal SfondoCod As Integer, _
                            ByVal Quadrante As Integer, _
                            ByVal MeteoAttivo As Integer, _
                            ByVal TipoMeteo As Integer, _
                            ByVal SfondoBackground As String, _
                                ByVal Validita_Inizio As Date, _
                                ByVal Validita_Fine As Date, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreContabDAL.AziInfoRER_Write.Scrivi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.Append("INSERT INTO AziInfoRER(Piva,Sa_Cod,SfondoCod,Quadrante,MeteoAttivo,TipoMeteo,SfondoBackground,Inviato,DataInvio,Data_Creazione,Data_Modifica,UserName_Creazione,UserName_Modifica,Validita_Inizio,Validita_Fine) ")
            StrSQL.Append("VALUES (")
            StrSQL.Append("          '" & Agro_SQL_SaveText(Trim(Piva)) & "' ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Sa_Cod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(SfondoCod) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(Quadrante) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(MeteoAttivo) & "  ")
            StrSQL.Append("         , " & Agro_SQL_SaveNum(TipoMeteo) & "  ")
            StrSQL.Append("         ,'" & Agro_SQL_SaveText(Trim(SfondoBackground)) & "' ")
            StrSQL.Append("         , 0, Null  ")
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



    Public Function Cancella( _
                                ByVal Piva As String, _
                                ByVal Sa_Cod As Integer, _
                                ByVal xFiltroAggiuntivo As String, _
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                    ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGraficaDAL.AziInfoRER_Write.Cancella()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try

            If Piva = "" Then
                Throw New Exception("Parametro non corretto nella query (Piva obbligatorio)")
            End If

            'If Sa_Cod = 0 Then
            '    Throw New Exception("Parametro non corretto nella query (Sa_Cod obbligatorio)")
            'End If

            '---------------------------------------------
            StrSQL.Length = 0

            '---------------------------------------------
            If objParametri.FlagCancellazioneLogica = enumCancellazioneLogica.CancellazioneLogica Then

                StrSQL.Length = 0
                StrSQL.Append(" UPDATE AziInfoRER ")
                StrSQL.Append(" SET ")
                StrSQL.Append("         Username_Modifica = '" & Agro_SQL_SaveText(objParametri.UsernameOperazione) & "' ")
                StrSQL.Append("         ,Data_Modifica= '" & Agro_SQL_SaveText(Date.Now) & "' ")
                StrSQL.Append("         ,Inviato = -1 ")
                StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If
                StrSQL.Append(" AND     Inviato >= 0 ")

            Else

                StrSQL.Length = 0
                StrSQL.Append(" DELETE ")
                StrSQL.Append(" FROM    AziInfoRER ")
                StrSQL.Append(" WHERE   Piva = '" & Agro_SQL_SaveText(Piva) & "' ")
                If Sa_Cod <> 0 Then
                    StrSQL.Append(" AND     Sa_Cod = " & Agro_SQL_SaveNum(Sa_Cod) & " ")
                End If
                StrSQL.Append(" AND     Inviato >= 0 ")

            End If
            '---------------------------------------------

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






End Class

'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################
'##############################################################################################


Public Class AziInfoRER_Read
    Inherits AgronicaCoreDataProvider.DataProvider

    '#############################################################################################
    Public Function Leggi( _
                          ByVal PIVA As String, _
                          ByVal Sa_Cod As Integer, _
                                ByVal xSelezioneVariabile As AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile, _
                                ByVal xFiltroAggiuntivo As String, _
                                ByVal xOrderBy As String, _
                                ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri _
                                ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreGraficaDAL.AziInfoRER_Read.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            Select Case xSelezioneVariabile

                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaDatiMinimi


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Length = 0

                    StrSQL.Append(" SELECT AziInfoRer.* ")
                    StrSQL.Append("  FROM  AziInfoRer ")

                    StrSQL.Append("  WHERE Validita_inizio < " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " " & _
                                  " AND   Validita_Fine > " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " " & _
                                  " AND   Piva='" & Agro_SQL_SaveText(PIVA) & "'" & _
                                  " AND   Sa_cod=" & Agro_vb_SaveNum(Sa_Cod))

                    '--------------------------------------------------------------------------
                    If xFiltroAggiuntivo <> "" Then
                        StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
                    End If
                    '--------------------------------------------------------------------------
                    Select Case objParametri.FlagVisibilita
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                            StrSQL.Append(" AND   AziInfoRer.Inviato >=0 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                            StrSQL.Append(" AND   AziInfoRer.Inviato =-1 ")
                        Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                            '...................................
                        Case Else
                            Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
                    End Select
                    '--------------------------------------------------------------------------
                    If xOrderBy <> "" Then
                        StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
                    End If


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