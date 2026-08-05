
Imports System.Data.OleDb
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class WAnagrafica_Stati_R
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function Leggi(ByVal WAnagraficaStati_Cod As Int32,
                          ByVal WWorkflow_Cod As Int32,
                          ByVal xFiltroAggiuntivo As String,
                          ByVal xOrderBy As String,
                          ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                          ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.WAnagrafica_Stati_R.Leggi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.Append(" SELECT * ")
            StrSQL.Append(" FROM  WAnagraficaStati ")
            StrSQL.Append(" WHERE 1=1 ")

            If WAnagraficaStati_Cod <> 0 Then
                StrSQL.Append(" AND WAnagraficaStati_Cod = " & Agro_SQL_SaveNum(WAnagraficaStati_Cod) & " ")
            End If

            If WWorkflow_Cod <> 0 Then
                StrSQL.Append(" AND WWorkflow_Cod = " & Agro_SQL_SaveNum(WWorkflow_Cod) & " ")
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

    Public Function StatiDaServizio(ByVal Servizio_Cod As Int32,
                                    ByVal xFiltroAggiuntivo As String,
                                    ByVal xOrderBy As String,
                                    ByRef objParametri As AgronicaCoreDataProvider.AgronicaCoreParametri
                                    ) As DataTable


        Dim NomeRoutine As String = "AgronicaCoreMetaSchemaDAL.WAnagrafica_Stati_R.StatiDaServizio()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try

            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT Stato_Origine_Cod, WAnagraficaStati.WAnagraficaStati_Des ")
            StrSQL.AppendLine(" FROM WTransizioniDiStatoConfigurazione ")
            StrSQL.AppendLine(" JOIN WAnagraficaStati ON WTransizioniDiStatoConfigurazione.Stato_Origine_Cod = WAnagraficaStati.WAnagraficaStati_Cod ")
            StrSQL.AppendLine(" WHERE Servizio_cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")
            StrSQL.AppendLine(" UNION ")
            StrSQL.AppendLine(" SELECT Stato_Destinazione_cod, WAnagraficaStati.WAnagraficaStati_Des ")
            StrSQL.AppendLine(" FROM WTransizioniDiStatoConfigurazione ")
            StrSQL.AppendLine(" JOIN WAnagraficaStati ON WTransizioniDiStatoConfigurazione.Stato_Destinazione_cod = WAnagraficaStati.WAnagraficaStati_Cod ")
            StrSQL.AppendLine(" WHERE Servizio_cod = " & Agro_SQL_SaveNum(Servizio_Cod) & " ")


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
