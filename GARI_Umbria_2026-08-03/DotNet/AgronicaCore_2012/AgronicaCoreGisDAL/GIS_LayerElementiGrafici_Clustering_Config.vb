Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi
Imports System.Text
Imports System.Data
Public Class GIS_LayerElementiGrafici_Clustering_Config_R
    Inherits AgronicaCoreDataProvider.DataProvider2010
    '  DAL per le operazioni di LETTURA dalle tabelle di configurazione del clustering.

    ''' <summary>
    ''' Legge un profilo di configurazione completo (testata + dettagli).
    ''' </summary>
    ''' <param name="tipoAlgoritmoCod">Il tipo di algoritmo (fisso a 1 per ora).</param>
    ''' <param name="layerCod">L'ID del layer.</param>
    ''' <param name="utente">Lo username dell'utente.</param>
    ''' <param name="objParametri">L'oggetto di contesto.</param>
    ''' <returns>Un DataTable con il risultato della JOIN tra _Config e _Config_Detail.</returns>
    Public Function LeggiConfigurazione(
        ByVal tipoAlgoritmoCod As Integer,
        ByVal layerCod As Integer,
        ByVal utente As String,
        ByRef objParametri As AgronicaCoreParametri
    ) As DataTable

        Dim nomeRoutine As String = "AgronicaCoreGisDAL.GIS_LayerElementiGrafici_Clustering_Config_R.LeggiConfigurazione()"
        Dim strSQL As New StringBuilder
        Dim dt As DataTable

        Try
            strSQL.Length = 0
            strSQL.Append(" SELECT cfg.*, det.LayerElementiGrafici_Clustering_Config_Detail_Cod, det.Descrizione, det.Parametri")
            strSQL.Append(" FROM dbo.GIS_LayerElementiGrafici_Clustering_Config AS cfg")
            strSQL.Append(" LEFT JOIN dbo.GIS_LayerElementiGrafici_Clustering_Config_Detail AS det ON cfg.LayerElementiGrafici_Clustering_Config_Cod = det.LayerElementiGrafici_Clustering_Config_Cod")
            strSQL.Append(" WHERE cfg.LayerElementiGrafici_Clustering_Type_Cod = " & Agro_SQL_SaveNum(tipoAlgoritmoCod) & "")
            strSQL.Append("   AND cfg.LayerElementiGrafici_Cod = " & Agro_SQL_SaveNum(layerCod) & "")
            strSQL.Append("   AND cfg.Utente = '" & Agro_SQL_SaveText(utente) & "'")
            strSQL.Append("   AND cfg.Validita_Inizio <= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleFine) & " ")
            strSQL.Append("   AND cfg.Validita_Fine >= " & Agro_SQL_SaveDate(objParametri.FinestraTemporaleInizio) & " ")

            Select Case objParametri.FlagVisibilita
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloNonCancellati
                    strSQL.Append(" AND cfg.inviato >= 0 ")
                Case AgronicaCoreParametri.enumVisibilita.Visibilita_SoloCancellati
                    strSQL.Append(" AND cfg.inviato = -1 ")
            End Select

            dt = EseguiQuery_Lettura(objParametri, strSQL.ToString, nomeRoutine)

        Catch ex As Exception
            Scrivi_LOG(objParametri, nomeRoutine, ex.Message)
            dt = Nothing
            Throw New Exception("[" & nomeRoutine & "] : " & ex.Message)
        End Try

        Return dt
    End Function

End Class
Public Class GIS_LayerElementiGrafici_Clustering_Config_W
    Inherits AgronicaCoreDataProvider.DataProvider2010
End Class
