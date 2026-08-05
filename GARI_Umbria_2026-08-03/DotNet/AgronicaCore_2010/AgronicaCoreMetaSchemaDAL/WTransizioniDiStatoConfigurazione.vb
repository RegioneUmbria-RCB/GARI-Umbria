Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate
Imports AgronicaCoreDataProvider

Public Class WTransizioniDiStatoConfigurazione_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal Servizio_Cod As Int32,
                          ByVal Stato_Origine_Cod As Int32,
                          ByVal Stato_Destinazione_Cod As Int32,
                          ByVal xSelezioneVariabile As AgronicaCoreParametri.enumSelezioneVariabile,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.WTransizioniDiStatoConfigurazione_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0


            Select Case xSelezioneVariabile
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_JoinDescrizioni

                    StrSQL.Append(" SELECT t.* ")
                    StrSQL.Append("        , o.WAnagraficaStati_Des as Stato_Origine_Des ")
                    StrSQL.Append("        , d.WAnagraficaStati_Des as Stato_Destinazione_Des ")
                    StrSQL.Append("        , s.Servizio_Des ")
                    StrSQL.Append(" FROM  WTransizioniDiStatoConfigurazione t ")
                    StrSQL.Append(" INNER JOIN WAnagraficaStati o ON t.Stato_Origine_Cod=o.WAnagraficaStati_Cod ")
                    StrSQL.Append(" INNER JOIN WAnagraficaStati d ON t.Stato_Destinazione_Cod=d.WAnagraficaStati_Cod ")
                    StrSQL.Append(" INNER JOIN Servizi s ON t.Servizio_cod=s.Servizio_Cod ")


                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumSelezioneVariabile.Selezione_TabellaCompleta

                    StrSQL.Append(" SELECT t.* ")
                    StrSQL.Append(" FROM  WTransizioniDiStatoConfigurazione t ")

            End Select

            StrSQL.Append(" WHERE 1=1 ")



            If Servizio_Cod <> 0 Then
                StrSQL.Append(" AND t.Servizio_Cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            End If

            If Stato_Origine_Cod <> 0 Then
                StrSQL.Append(" AND t.Stato_Origine_Cod = " & Agro_SQL_SaveNum(Stato_Origine_Cod) & " ")
            End If

            If Stato_Destinazione_Cod <> 0 Then
                StrSQL.Append(" AND t.Stato_Destinazione_Cod = " & Agro_SQL_SaveNum(Stato_Destinazione_Cod) & " ")
            End If


            '--------------------------------------------------------------------------
            If xFiltroAggiuntivo <> "" Then
                StrSQL.Append(" AND " & Agro_SQL_Save_xFiltroAggiuntivo(xFiltroAggiuntivo, , objParametri))
            End If
            '--------------------------------------------------------------------------
            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    StrSQL.Append(" AND   t.Inviato >=0 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    StrSQL.Append(" AND   t.Inviato =-1 ")
                Case AgronicaCoreDataProvider.AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    '...................................
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select
            '--------------------------------------------------------------------------
            If xOrderBy <> "" Then
                StrSQL.Append(" ORDER BY " & Agro_SQL_Save_xOrderBy(xOrderBy, objParametri))
            Else
                'StrSQL.Append(" ORDER BY t.Ordine ")
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
