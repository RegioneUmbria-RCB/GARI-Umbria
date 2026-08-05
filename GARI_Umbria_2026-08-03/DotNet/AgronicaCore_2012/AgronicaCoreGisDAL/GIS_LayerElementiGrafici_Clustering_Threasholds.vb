Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports System.Data

Public Class GIS_LayerElementiGrafici_Clustering_Threasholds_R
    Inherits AgronicaCoreDataProvider.DataProvider2010

    ' Recupera le soglie di clustering per un dato algritmo e livello di zoom.

    'non lasciare mai una funzione senza il tipo associato 
    Public Function LeggiSoglieClusterAlgorithm(ByVal algCode As Integer,
                                                ByVal zoomLevel As Integer,
                                                ByRef objParametriServer As AgronicaCoreParametri) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Clustering_Threasholds_R.LeggiSoglieClusterAlgorithm()"
        Dim strSQL As New StringBuilder
        Dim dt As DataTable

        Try
            If algCode <= 0 Or zoomLevel < 0 Then
                Throw New Exception("Parametri non validi (tipoAlgoritmoCod e zoomLevel).")
            End If

            strSQL.Length = 0
            strSQL.Append(" SELECT * ")
            strSQL.Append(" FROM   GIS_LayerElementiGrafici_Clustering_Threasholds ")
            strSQL.Append(" WHERE  LayerElementiGrafici_Clustering_Type_Cod = " & Agro_SQL_SaveNum(algCode) & " ")
            strSQL.Append("   AND  ZoomLevel = " & Agro_SQL_SaveNum(zoomLevel) & " ")


            strSQL.Append("   AND  Validita_Inizio <= " & Agro_SQL_SaveDate(objParametriServer.FinestraTemporaleFine) & " ")
            strSQL.Append("   AND  Validita_Fine >= " & Agro_SQL_SaveDate(objParametriServer.FinestraTemporaleInizio) & " ")

            Select Case objParametriServer.FlagVisibilita
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND inviato >= 0 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND inviato = -1 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_Tutti
                    ' Nessun filtro aggiuntivo
                Case Else
                    Throw New Exception("Parametro non corretto nella query (FlagVisibilita)")
            End Select

            dt = EseguiQuery_Lettura(objParametriServer, strSQL.ToString, nomeRoutine)


        Catch ex As Exception
            Scrivi_LOG(objParametriServer, nomeRoutine, ex.Message)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt

    End Function
End Class

Public Class GIS_LayerElementiGrafici_Clustering_Threasholds_W
    Inherits AgronicaCoreDataProvider.DataProvider2010
End Class