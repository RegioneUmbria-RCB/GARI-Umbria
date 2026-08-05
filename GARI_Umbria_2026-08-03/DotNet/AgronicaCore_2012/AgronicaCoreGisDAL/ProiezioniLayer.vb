Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.UtilityProvider
Imports AgronicaCoreDataProvider.CostantiPersonalizzate

Public Class ProiezioniLayer_R
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function LeggiElencoConfigurazioni(ByVal cfgFilter As String,
                                              ByVal isAttivo As Boolean,
                                              ByVal gruppiUtente As List(Of Int32),
                                              ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiElencoConfigurazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("       LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Des ")
            StrSQL.AppendLine(" 	  , LAC.AttivaSuTuttiLayer ")
            StrSQL.AppendLine(" 	  , COALESCE(ALG.CanActivate, 0) AS CanActivate ")
            StrSQL.AppendLine(" 	  , COALESCE(ALG.CanEditCfg, 0) AS CanEditCfg ")
            StrSQL.AppendLine(" 	  , COALESCE(LAC.LayerAnalysisConfig_Cfg, '') AS LayerAnalysisConfig_Cfg ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_GUID ")
            StrSQL.AppendLine(" 	  , LACD.LayerAnalysisConfig_Algorithm_Param_Cod ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_Struct_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerElementiGrafici_GUID ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_struct_GUID ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig LAC ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" INNER JOIN dbo.GIS_LayerAnalysisConfig_DataStruct LACD ON ")
            StrSQL.AppendLine(" 	LACD.LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod")
            StrSQL.AppendLine(" 	AND LACD.PivaSuperUser = LAC.PivaSuperUser ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" 	INNER JOIN GIS_LayerAnalysisConfig_Algorithm ALG ON LAC.LayerAnalysisConfig_Algorithm_Cod = ALG.LayerAnalysisConfig_Algorithm_Cod")

            StrSQL.AppendLine("")
            StrSQL.AppendLine(String.Format(" WHERE LAC.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine("     AND (")
            StrSQL.AppendLine("         EXISTS(")
            StrSQL.AppendLine("             Select 1 from GIS_LayerAnalysisConfigXUtente ")
            StrSQL.AppendLine(String.Format("              Where UserName = '{0}' and LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod", Agro_SQL_SaveText(objParametri.UtenteUsername)))
            StrSQL.AppendLine("         )")
            StrSQL.AppendLine("         OR EXISTS(")
            StrSQL.AppendLine("             Select 1 from GIS_LayerAnalysisConfigXGruppiUtente ")
            StrSQL.AppendLine(String.Format("           Where Gruppi_Utente_cod IN ({0}) and LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod", String.Join(",", gruppiUtente)))
            StrSQL.AppendLine("          ) ")
            StrSQL.AppendLine(" ) ")

            If isAttivo Then
                StrSQL.AppendLine("     AND LAC.AttivaSuTuttiLayer = 1 ")
            End If

            If cfgFilter <> "" Then
                StrSQL.AppendLine(String.Format("     AND LAC.LayerAnalysisConfig_Cfg = '{0}'", Agro_SQL_SaveText(cfgFilter)))
            End If

            StrSQL.AppendLine(" ORDER BY LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 		 , LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 		 , LACD.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 		 , LACD.LayerAnalysisConfig_Algorithm_Param_Cod ")

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

    Public Function LeggiElencoConfigurazioniSuLayer(ByVal layer_cod As Int32,
                                                     ByVal tipologia_layer_cod As Int32,
                                                     ByVal entita_cod As Int32,
                                                     ByRef objParametri As AgronicaCoreParametri
                                                     ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiElencoConfigurazioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("       LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Des ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 	  , LACA.LayerAnalysisConfig_Algorithm_Des ")
            StrSQL.AppendLine(" 	  , LAC.AttivaSuTuttiLayer ")
            StrSQL.AppendLine(" 	  , ISNULL(LACEL.Entita_cod_1, 0) As Entita_Cod ")
            StrSQL.AppendLine(" 	  , LACEL.Validita_Inizio ")
            StrSQL.AppendLine(" 	  , LACEL.Validita_Fine ")

            StrSQL.AppendLine(String.Format(" , (SELECT COUNT(distinct Entita_Cod) FROM GIS_ElementiGrafici 
                                                 WHERE LayerElementiGrafici_Cod = {0} 
                                                 AND Validita_Inizio <= GETDATE() AND Validita_Fine > GETDATE()) NumEntitaMax ",
                                            Agro_SQL_SaveNum(layer_cod)))

            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig LAC ")
            StrSQL.AppendLine(" INNER JOIN dbo.GIS_LayerAnalysisConfig_Algorithm LACA ")
            StrSQL.AppendLine("     ON LACA.LayerAnalysisConfig_Algorithm_Cod = LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" LEFT JOIN dbo.GIS_LayerAnalysisConfig_Exec_Log LACEL ")
            StrSQL.AppendLine("     ON LACEL.LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine("     AND LACEL.AttivoPerElaborazione = 1 ")

            If entita_cod > 0 Then
                StrSQL.AppendLine(String.Format("     AND LACEL.Entita_cod_1 = {0} ", Agro_SQL_SaveNum(entita_cod)))
            End If

            StrSQL.AppendLine(String.Format(" WHERE LAC.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))

            StrSQL.AppendLine(String.Format(" AND EXISTS (SELECT 1 FROM dbo.GIS_LayerAnalysisConfig_DataStruct 
                                                         WHERE LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod 
                                                         AND LayerXConfig_Cod = 1 
                                                         AND LayerElementiGrafici_Cod = {0}
                                                         AND TipologiaLayer_Cod = {1}) ",
                                            Agro_SQL_SaveNum(layer_cod), Agro_SQL_SaveNum(tipologia_layer_cod)))

            StrSQL.AppendLine(" ORDER BY LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 		 , LACEL.Entita_cod_1 ")
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

    Public Function LeggiListaEsecuzioniAlgoritmi(ByRef objParametri As AgronicaCoreParametri,
                                                  ByVal noConf As Boolean,
                                                  ByVal ordinaPerAlgoritmo As Boolean,
                                                  ByVal TagName As String
                                                  ) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiListaEsecuzioniAlgoritmi()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" LACEL.GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine(" , ISNULL(LAC.LayerAnalysisConfig_Algorithm_Cod, 0) LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" , LACEL.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" , LACEL.Entita_cod_1 ")
            StrSQL.AppendLine(" , LACEL.Entita_cod_2 ")
            StrSQL.AppendLine(" , LACEL.Entita_cod_Risultato ")
            StrSQL.AppendLine(" , ISNULL(LACEL.Elaborazione_GUID, '') Elaborazione_GUID ")
            StrSQL.AppendLine(" , LACEL.Errori_Elaborazione ")
            StrSQL.AppendLine(" , ISNULL(LACEL.ParametriElaborazione, '') ParametriElaborazione ")

            StrSQL.AppendLine(" FROM GIS_LayerAnalysisConfig_Exec_Log LACEL ")
            StrSQL.AppendLine(" LEFT JOIN GIS_LayerAnalysisConfig LAC ")
            StrSQL.AppendLine("            ON LAC.LayerAnalysisConfig_Cod = LACEL.LayerAnalysisConfig_Cod ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(" LACEL.StatoElaborazione = 0 ")
            StrSQL.AppendLine(" And LACEL.AttivoPerElaborazione = 1 ")

            If noConf Then
                StrSQL.AppendLine(" And LACEL.LayerAnalysisConfig_Cod = 0 ")
            Else
                StrSQL.AppendLine(" And LACEL.LayerAnalysisConfig_Cod > 0 ")
            End If

            If TagName <> "" Then
                StrSQL.AppendLine(String.Format(" And LACEL.TagExecution = '{0}' ", Agro_SQL_SaveText(TagName)))
            End If

            If ordinaPerAlgoritmo Then
                StrSQL.AppendLine(" Order by LayerAnalysisConfig_Algorithm_Cod ")
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

    Public Function LeggiLayerDaConfigurazione(ByVal layerAnalysisConfig_Cod As Int32,
                                               ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiLayerDaConfigurazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine("       LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	  , ISNULL(LAC.LayerAnalysisConfig_GUID, '') As LayerAnalysisConfig_GUID ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Des ")
            StrSQL.AppendLine(" 	  , COALESCE(ALG.CanActivate, 0) AS CanActivate ")
            StrSQL.AppendLine(" 	  , COALESCE(ALG.CanEditCfg, 0) AS CanEditCfg ")
            StrSQL.AppendLine(" 	  , COALESCE(LAC.LayerAnalysisConfig_Cfg, '') AS LayerAnalysisConfig_Cfg ")
            StrSQL.AppendLine(" 	  , LAC.AttivaSuTuttiLayer ")
            StrSQL.AppendLine(" 	  , LACD.LayerAnalysisConfig_Algorithm_Param_Cod ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_Struct_Cod ")
            StrSQL.AppendLine(" 	  , ISNULL(LACD.TipologiaLayer_struct_GUID, '') As TipologiaLayer_struct_GUID ")
            StrSQL.AppendLine(" 	  , ISNULL(LEGADS.LayerElementiGrafici_Etichetta, '') as LayerElementiGrafici_Etichetta ")
            StrSQL.AppendLine(" 	  , ISNULL(PAR.GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod, 0) as GIS_LayerAnalysisConfig_AlgorithmType_Param_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	  , ISNULL(LACD.LayerElementiGrafici_GUID, '') As LayerElementiGrafici_GUID ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_Cod ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig LAC ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" INNER JOIN dbo.GIS_LayerAnalysisConfig_DataStruct LACD ON ")
            StrSQL.AppendLine(" 	LACD.LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod AND ")
            StrSQL.AppendLine(" 	LACD.PivaSuperUser = LAC.PivaSuperUser ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" LEFT JOIN dbo.GIS_LayerAnalysisConfig_Algorithm_Params PAR ON ")
            StrSQL.AppendLine(" 	PAR.LayerAnalysisConfig_Algorithm_Param_Cod = LACD.LayerAnalysisConfig_Algorithm_Param_Cod ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" LEFT JOIN GIS_LayerElementiGrafici_Anagrafica_DataStruct LEGADS ON ")
            StrSQL.AppendLine(" 	LEGADS.LayerElementiGrafici_Cod = LACD.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	AND LEGADS.TipologiaLayer_struct_cod = LACD.TipologiaLayer_Struct_Cod ")
            StrSQL.AppendLine("")
            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig_Algorithm ALG ON LAC.LayerAnalysisConfig_Algorithm_Cod = ALG.LayerAnalysisConfig_Algorithm_Cod")

            StrSQL.AppendLine(String.Format(" WHERE LAC.LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(layerAnalysisConfig_Cod)))

            StrSQL.AppendLine(" ORDER BY LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 		 , LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 		 , LACD.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 		 , LACD.LayerAnalysisConfig_Algorithm_Param_Cod ")

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

    Public Function LeggiElencoConfigurazioniSincro(ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiElencoConfigurazioniSincro()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT XYZ.* ")
            StrSQL.AppendLine(" FROM ( ")
            StrSQL.AppendLine("     SELECT ")

            StrSQL.AppendLine("       LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 	  , LAC.LayerAnalysisConfig_Des ")
            StrSQL.AppendLine(" 	  , LAC.AttivaSuTuttiLayer ")
            StrSQL.AppendLine(" 	  , LACD.LayerAnalysisConfig_Algorithm_Param_Cod ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_Struct_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 	  , LACD.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	  , LACD.TipologiaLayer_Cod ")
            StrSQL.AppendLine("       , (SELECT COUNT(distinct Entita_Cod) ")
            StrSQL.AppendLine(" 	     FROM GIS_ElementiGrafici ")
            StrSQL.AppendLine("          WHERE LayerElementiGrafici_Cod = LACD.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine("                 AND TipologiaLayer_cod = LACD.TipologiaLayer_Cod ")
            StrSQL.AppendLine("                 AND Validita_Inizio <= GETDATE() AND Validita_Fine > GETDATE()) as NumEntitaMax ")
            StrSQL.AppendLine(" 	  , ISNULL(LACEL.ElementiAttivi, 0) AS NumEntitaAttive ")

            StrSQL.AppendLine("     FROM dbo.GIS_LayerAnalysisConfig LAC ")

            StrSQL.AppendLine(" 	INNER JOIN dbo.GIS_LayerAnalysisConfig_DataStruct LACD ")
            StrSQL.AppendLine(" 		ON LACD.LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 		AND LACD.PivaSuperUser = LAC.PivaSuperUser ")

            StrSQL.AppendLine(" 	LEFT JOIN (SELECT LayerAnalysisConfig_Cod, COUNT(*) AS ElementiAttivi ")
            StrSQL.AppendLine(" 			   FROM dbo.GIS_LayerAnalysisConfig_Exec_Log ")
            StrSQL.AppendLine(" 			   WHERE AttivoPerElaborazione = 1 ")
            StrSQL.AppendLine(" 			   GROUP BY LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 		) AS LACEL ")
            StrSQL.AppendLine(" 		ON LACEL.LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod ")

            StrSQL.AppendLine("     WHERE ")

            StrSQL.AppendLine(String.Format(" LAC.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri_Server.PivaSuperUser)))

            StrSQL.AppendLine("     AND LAC.AttivaSuTuttiLayer = 1 ")
            StrSQL.AppendLine("     AND LACD.LayerXConfig_Cod = 1 ")
            StrSQL.AppendLine(" ) XYZ ")

            StrSQL.AppendLine(" WHERE XYZ.NumEntitaAttive < XYZ.NumEntitaMax ")

            StrSQL.AppendLine(" ORDER BY XYZ.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 		 , XYZ.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" 		 , XYZ.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 		 , XYZ.LayerAnalysisConfig_Algorithm_Param_Cod ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiLogEsecuzioni(ByVal Entita_Cod As Integer,
                                       ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiLogEsecuzioni()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT ")
            StrSQL.AppendLine(" LACELD.GIS_LayerAnalysisConfig_Exec_Log_Details_Cod ")
            StrSQL.AppendLine("     , L.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" 	, LAC.LayerAnalysisConfig_Des ")
            StrSQL.AppendLine("     , L.Entita_cod_1 ")
            StrSQL.AppendLine("     , L.Entita_cod_2 ")
            StrSQL.AppendLine("     , L.Entita_cod_Risultato ")
            StrSQL.AppendLine("     , COALESCE(LACELD.StatoElaborazione, L.StatoElaborazione) AS StatoElaborazione ")
            StrSQL.AppendLine("     , COALESCE(L.Messaggi, ISNULL(LACELD.Messaggi, '')) AS Messaggi ")
            StrSQL.AppendLine("     , COALESCE(LACELD.Data_Creazione, L.Data_Creazione) AS Data_Creazione ")
            StrSQL.AppendLine(" FROM GIS_LayerAnalysisConfig_Exec_Log L ")
            StrSQL.AppendLine(" LEFT JOIN GIS_LayerAnalysisConfig_Exec_Log_Details LACELD ")
            StrSQL.AppendLine("     ON LACELD.GIS_LayerAnalysisConfig_Exec_Log_Cod = L.GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig LAC ON LAC.LayerAnalysisConfig_Cod = L.LayerAnalysisConfig_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" L.Entita_cod_1 = {0} ", Agro_SQL_SaveNum(Entita_Cod)))
            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT

    End Function

    Public Function LeggiEsecuzioniAnalisiMappeSatellitari(ByVal poligonoWKT As String,
                                                           ByRef objParametri As AgronicaCoreParametri,
                                                           Optional ByVal Entita_cod As Integer = 0) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiEsecuzioniAnalisiMappeSatellitari()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0
            StrSQL.AppendLine(" SELECT Risultato_Json ")

            StrSQL.AppendLine(" FROM GIS_LayerAnalysisConfig_Exec_Log_Details D ")

            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig C ")
            StrSQL.AppendLine("     On C.LayerAnalysisConfig_Cod = D.LayerAnalysisConfig_Cod ")

            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig_Algorithm CFG ")
            StrSQL.AppendLine("     ON CFG.LayerAnalysisConfig_Algorithm_Cod = C.LayerAnalysisConfig_Algorithm_Cod ")

            StrSQL.AppendLine(" INNER JOIN GIS_ElementiGrafici G ")
            StrSQL.AppendLine("     ON D.Entita_cod_1 = G.Entita_Cod ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" CFG.LayerAnalysisConfig_AlgorithmType_Cod = {0} ", Agro_SQL_SaveNum(TipiEnumerativi.enum_TipoAlgoritmoProiezione.Analisi_Mappe_Satellitari_GEE)))
            StrSQL.AppendLine(String.Format(" AND G.Poligono_GeoEntity.STIntersects({0}) = 1 ", Agro_SQL_SaveGeograpyFromWKTString(poligonoWKT)))

            If Entita_cod > 0 Then
                StrSQL.AppendLine(String.Format(" AND G.Entita_Cod={0} ", Agro_SQL_SaveNum(Entita_cod)))
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

    Public Function LeggiAttivazioneEntitaScadute(ByRef objParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiEntitaScadute()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT distinct LACEL.GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine(" FROM GIS_ElementiGrafici EG ")
            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig_Exec_Log LACEL ")
            StrSQL.AppendLine(" 	ON LACEL.Entita_cod_1 = EG.Entita_Cod ")
            StrSQL.AppendLine(" 	And LACEL.AttivoPerElaborazione = 1 ")
            StrSQL.AppendLine(" WHERE EG.Validita_Inizio > GETDATE() Or EG.Validita_Fine <= GETDATE() ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri_Server, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri_Server, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT
    End Function

    Public Function VerificaEsistenzaConfigurazione(ByVal algoritmoProiezione_Cod As Integer,
                                                    ByVal layerParams As List(Of String),
                                                    ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.VerificaEsistenzaConfigurazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT LAC.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig LAC ")
            StrSQL.AppendLine(" INNER JOIN dbo.GIS_LayerAnalysisConfig_DataStruct LACD ON ")
            StrSQL.AppendLine(" 	LACD.LayerAnalysisConfig_Cod = LAC.LayerAnalysisConfig_Cod And ")
            StrSQL.AppendLine(" 	LACD.PivaSuperUser = LAC.PivaSuperUser ")

            StrSQL.AppendLine(String.Format(" WHERE LAC.PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND LAC.LayerAnalysisConfig_Algorithm_Cod = {0} ", Agro_SQL_SaveNum(algoritmoProiezione_Cod)))

            For Each param In layerParams
                StrSQL.AppendLine(param)
            Next

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return (DT IsNot Nothing AndAlso DT.Rows.Count > 0)

    End Function

    Public Function VerificaEsistenzaEsecuzione(ByVal configurazioneProiezione_Cod As Integer,
                                                ByVal entita_cod_1 As Int32,
                                                ByVal entita_cod_2 As Int32,
                                                ByVal entita_cod_Risultato As Int32,
                                                ByRef objParametri As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.VerificaEsistenzaEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig_Exec_Log ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))
            StrSQL.AppendLine(String.Format(" AND Entita_cod_1 = {0} ", Agro_SQL_SaveNum(entita_cod_1)))
            StrSQL.AppendLine(String.Format(" AND Entita_cod_2 = {0} ", Agro_SQL_SaveNum(entita_cod_2)))
            StrSQL.AppendLine(String.Format(" AND Entita_cod_Risultato = {0} ", Agro_SQL_SaveNum(entita_cod_Risultato)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return 0
        End If

        Return CInt(DT(0)("GIS_LayerAnalysisConfig_Exec_Log_Cod"))

    End Function

    Public Function VerificaAttivazioneEsecuzione(ByVal configurazioneProiezione_Cod As Integer,
                                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.VerificaEsistenzaEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT COUNT(*) ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig_Exec_Log ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        If DT Is Nothing OrElse DT.Rows.Count = 0 Then
            Return 0
        End If

        Return DT IsNot Nothing AndAlso DT.Rows.Count > 0

    End Function

    Public Function IsConfigurationAlreadyActive(ByVal configurazioneProiezione_Cod As Integer,
                                                 ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.IsConfigurationAlreadyActive()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("SELECT *")
            StrSQL.AppendLine("FROM dbo.GIS_LayerAnalysisConfig_Exec_Log")

            StrSQL.AppendLine("WHERE 1 = 1")

            StrSQL.AppendLine(String.Format("    AND LayerAnalysisConfig_Cod = {0}", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))
            StrSQL.AppendLine("    AND AttivoPerElaborazione = 1 ")

            '--------------------------------------------------------------------------
            DT = EseguiQuery_Lettura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try

        Return DT IsNot Nothing AndAlso DT.Rows.Count > 0

    End Function


    Public Function LeggiElencoPoligoniAttivi(ByVal configurazioneProiezione_Cod As Integer,
                                              ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.VerificaEsistenzaEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT distinct Entita_cod_1 ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig_Exec_Log ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))

            StrSQL.AppendLine(" AND AttivoPerElaborazione = 1 ")

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

    Public Function LeggiConfigurazioneDaGUID(ByVal guidConfigurazione As String,
                                              ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiConfigurazioneDaGUID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_GUID = '{0}' ", Agro_SQL_SaveText(guidConfigurazione)))

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
    Public Function LeggiConfigurazioneDaCodice(ByVal configurazione_cod As Int32,
                                                ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiConfigurazioneDaGUID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazione_cod)))

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

    Public Function LeggiEsecuzioneDaGUID(ByVal guidEsecuzione As String,
                                          ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiEsecuzioneDaGUID()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig_Exec_Log ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" Elaborazione_GUID = '{0}' ", Agro_SQL_SaveText(guidEsecuzione)))

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

    Public Function LeggiEsecuzioneDaCodice(ByVal codEsecuzione As String,
                                          ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiEsecuzioneDaCodice()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT * ")
            StrSQL.AppendLine(" FROM dbo.GIS_LayerAnalysisConfig_Exec_Log ")

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" GIS_LayerAnalysisConfig_Exec_Log_Cod = {0} ", Agro_SQL_SaveNum(codEsecuzione)))

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

    Public Function LeggiDatiEsecuzionePerScrittureEntitaGEE(ByVal esecuzione_cod As Int32,
                                                             ByRef objParametri As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiDatiEsecuzionePerScrittureEntitaGEE()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" SELECT DISTINCT ")
            StrSQL.AppendLine(" 	ALGT.LayerAnalysisConfig_AlgorithmType_Cod ")
            StrSQL.AppendLine(" 	, CFG.LayerXConfig_Cod ")
            StrSQL.AppendLine(" 	, LG.Entita_Cod_1 ")
            StrSQL.AppendLine(" 	, ANA.LayerElementiGrafici_Des ")
            StrSQL.AppendLine(" 	, ANA.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	, ISNULL(EG.ElementoGrafico_Des, '') As ElementoGrafico_Des ")
            StrSQL.AppendLine(" 	, CASE WHEN EG.Poligono_GeoEntity IS NULL THEN '' ELSE EG.Poligono_GeoEntity.STAsText() END As GeoData ")
            StrSQL.AppendLine(" FROM GIS_LAyerAnalysisConfig_Exec_Log LG ")

            StrSQL.AppendLine(" INNER Join GIS_LayerAnalysisConfig LAC ")
            StrSQL.AppendLine("     ON LAC.LayerAnalysisConfig_Cod = LG.LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" INNER Join GIS_LayerAnalysisConfig_Algorithm ALG ")
            StrSQL.AppendLine("     ON ALG.LayerAnalysisConfig_Algorithm_Cod = LAC.LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" INNER Join GIS_LayerAnalysisConfig_AlgorithmType ALGT ")
            StrSQL.AppendLine("     ON ALGT.LayerAnalysisConfig_AlgorithmType_Cod = ALG.LayerAnalysisConfig_AlgorithmType_Cod ")

            StrSQL.AppendLine(" INNER JOIN GIS_LayerAnalysisConfig_DataStruct CFG ")
            StrSQL.AppendLine(" 	ON CFG.LayerAnalysisConfig_Cod = LG.LayerAnalysisConfig_Cod ")

            StrSQL.AppendLine(" INNER JOIN GIS_LayerElementiGrafici_Anagrafica ANA ")
            StrSQL.AppendLine(" 	ON ANA.LayerElementiGrafici_Cod = CFG.LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" 	AND ANA.TipologiaLayer_Cod = CFG.TipologiaLayer_Cod ")

            StrSQL.AppendLine(" LEFT JOIN GIS_ElementiGrafici EG ")
            StrSQL.AppendLine(" 	ON EG.Entita_Cod = LG.Entita_cod_1 ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LG.GIS_LayerAnalysisConfig_Exec_Log_Cod = {0} ", Agro_SQL_SaveNum(esecuzione_cod)))
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

    Public Function LeggiElencoEstensioniAlgoritmiProiezione(ByVal configurazioneProiezione_Cod As Integer,
                                                             ByRef objParametri As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_R.LeggiElencoEstensioniAlgoritmiProiezione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim DT As DataTable

        Try


        Catch ex As Exception
            MessaggioErrore = ex.Message
            Scrivi_LOG(objParametri, NomeRoutine, MessaggioErrore)
            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "] : " & MessaggioErrore)
        End Try
        Return DT

    End Function

End Class
Public Class ProiezioniLayer_W
    Inherits AgronicaCoreDataProvider.DataProvider
    Public Function SalvaConfigurazioneProiezione(ByVal algoritmoProiezione_Cod As Int32,
                                                  ByVal configurazioneProiezione_Des As String,
                                                  ByVal configurazioneProiezione_GUID As String,
                                                  ByVal newConfId As Int32,
                                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SalvaConfigurazioneProiezione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_LayerAnalysisConfig ( ")
            StrSQL.AppendLine(" PivaSuperUser ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Des ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_GUID ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Algorithm_Cod ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newConfId)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(configurazioneProiezione_Des)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(configurazioneProiezione_GUID)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(algoritmoProiezione_Cod)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" ) ")
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

    Public Function SaveAlgorithmConfigurationCfg(ByVal configurazioneCod As Int32,
                                                  ByVal cfg As String,
                                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SalvaConfigurazioneProiezione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine("UPDATE GIS_LayerAnalysisConfig")
            StrSQL.AppendLine(String.Format("SET LayerAnalysisConfig_Cfg = '{0}'", cfg))
            StrSQL.AppendLine(String.Format("WHERE LayerAnalysisConfig_Cod = {0}", configurazioneCod))

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

    Public Function SetFlagAttivazioneInteroLayer(ByVal configurazioneProiezione_Cod As Integer,
                                                  ByVal isAttivo As Boolean,
                                                  ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SetFlagAttivazioneInteroLayer()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig ")

            StrSQL.AppendLine(String.Format(" SET AttivaSuTuttiLayer = {0} ", Agro_SQL_SaveNum(Convert.ToInt32(isAttivo))))
            StrSQL.AppendLine(" , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine(String.Format(" , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" PivaSuperUser = '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" AND LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))

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

    Public Function SetFlagAttivazioneEsecuzione(ByVal codiceEsecuzione As Int32,
                                                 ByVal isAttivo As Boolean,
                                                 ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SetFlagAttivazioneEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_Exec_Log ")

            StrSQL.AppendLine(String.Format(" SET AttivoPerElaborazione = {0} ", Agro_SQL_SaveNum(Convert.ToInt32(isAttivo))))
            StrSQL.AppendLine(" , Data_Modifica = GETDATE() ")
            StrSQL.AppendLine(String.Format(" , Username_Modifica = '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" WHERE ")
            StrSQL.AppendLine(String.Format(" GIS_LayerAnalysisConfig_Exec_Log_Cod = {0} ", Agro_SQL_SaveNum(codiceEsecuzione)))

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

    Public Function InserisciAttivazioneEsecuzione(ByVal newEsecuzioneId As Int32,
                                                   ByVal configurazioneProiezione_Cod As Int32,
                                                   ByVal entita_cod_1 As Int32,
                                                   ByVal entita_cod_2 As Int32,
                                                   ByVal entita_cod_Risultato As Int32,
                                                   ByRef objParametri As AgronicaCoreParametri,
                                                   Optional ByVal inputEsportazione As String = "",
                                                   Optional ByVal groupId As Integer = 0) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.InserisciAttivazioneEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_LayerAnalysisConfig_Exec_Log ( ")
            StrSQL.AppendLine(" GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" , Entita_cod_1 ")
            StrSQL.AppendLine(" , Entita_cod_2 ")
            StrSQL.AppendLine(" , Entita_cod_Risultato ")
            StrSQL.AppendLine(" , StatoElaborazione ")
            StrSQL.AppendLine(" , Elaborazione_GUID ")
            StrSQL.AppendLine(" , AttivoPerElaborazione ")
            StrSQL.AppendLine(" , ParametriElaborazione ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" , Group_Id ")
            StrSQL.AppendLine(" , TagExecution ")
            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(newEsecuzioneId)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(entita_cod_1)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(entita_cod_2)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(entita_cod_Risultato)))

            StrSQL.AppendLine(" , 0 ")
            StrSQL.AppendLine(" , null ")
            StrSQL.AppendLine(" , 1 ")

            If (Not inputEsportazione.Equals("")) Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(inputEsportazione)))
            Else
                StrSQL.AppendLine(" , null ")
            End If

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(groupId)))
            StrSQL.AppendLine(String.Format(" , '' "))
            StrSQL.AppendLine(" ) ")

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

    Public Function SalvaConfigurazioneProiezioneLayer(ByVal newConfId As Int32,
                                                       ByVal tipoRiga As Int32,
                                                       ByVal layerElementiGrafici_Cod As Int32,
                                                       ByVal tipologiaLayer_cod As Int32,
                                                       ByVal parametro_Cod As Int32,
                                                       ByVal tipologiaLayer_struct_cod As Int32,
                                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SalvaConfigurazioneProiezioneLayer()"

        Dim sequenze As New Agro_Sequenze
        Dim newDataStructId = sequenze.NuovoId_Tabella("GIS_LayerAnalysisConfig_DataStruct", 0, Int32.MaxValue, objParametri, True)

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_LayerAnalysisConfig_DataStruct ( ")
            StrSQL.AppendLine(" PivaSuperUser ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_DataStruct_Cod ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Algorithm_Param_Cod ")
            StrSQL.AppendLine(" , LayerXConfig_Cod ")
            StrSQL.AppendLine(" , LayerElementiGrafici_Cod ")
            StrSQL.AppendLine(" , TipologiaLayer_Cod ")
            StrSQL.AppendLine(" , TipologiaLayer_Struct_Cod ")
            StrSQL.AppendLine(" , LayerElementiGrafici_GUID ")
            StrSQL.AppendLine(" , TipologiaLayer_struct_GUID ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" '{0}' ", Agro_SQL_SaveText(objParametri.PivaSuperUser)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newConfId)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(newDataStructId)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(parametro_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(tipoRiga)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(layerElementiGrafici_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(tipologiaLayer_cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(tipologiaLayer_struct_cod)))

            StrSQL.AppendLine(String.Format(" , (SELECT LayerElementiGrafici_GUID 
                                                 FROM GIS_LayerElementiGrafici_Anagrafica 
                                                 WHERE LayerElementiGrafici_Cod = {0}
                                                 AND TipologiaLayer_cod = {1})", Agro_SQL_SaveNum(layerElementiGrafici_Cod), Agro_SQL_SaveNum(tipologiaLayer_cod)))

            StrSQL.AppendLine(String.Format(" , (SELECT TipologiaLayer_struct_GUID 
                                                 FROM GIS_LayerElementiGrafici_Anagrafica_DataStruct 
                                                 WHERE TipologiaLayer_struct_cod = {0})", Agro_SQL_SaveNum(tipologiaLayer_struct_cod)))

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" ) ")
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

    Public Function AggiornaStatusEsecuzioneAlgoritmo(ByVal codice_esecuzione As Integer,
                                                      ByVal status_esecuzione As Integer,
                                                      ByVal disattiva As Boolean,
                                                      ByVal messaggio As String,
                                                      ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.AggiornaStatusEsecuzioneAlgoritmo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_Exec_Log SET ")
            StrSQL.AppendLine(String.Format(" StatoElaborazione = {0} ", Agro_SQL_SaveNum(status_esecuzione)))

            If status_esecuzione = 0 Then
                StrSQL.AppendLine(" , Errori_Elaborazione = Errori_Elaborazione + 1 ")
            Else
                StrSQL.AppendLine(" , Messaggi = null ")
            End If

            If Not messaggio.Equals("") Then
                StrSQL.AppendLine(String.Format(" , Messaggi = ISNULL(Messaggi, '') + '{0}' ", Agro_SQL_SaveText(messaggio & ", ")))
            End If

            If disattiva Then
                StrSQL.AppendLine(" , AttivoPerElaborazione = 0 ")
            End If

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" GIS_LayerAnalysisConfig_Exec_Log_Cod = {0} ", Agro_SQL_SaveNum(codice_esecuzione)))

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

    Public Function InserisciLogEsecuzioneAlgoritmo(ByVal newDetailLogID As Int32,
                                                    ByVal esecuzione_cod As Int32,
                                                    ByVal configurazione_cod As Int32,
                                                    ByVal entita_cod_1 As Int32,
                                                    ByVal entita_cod_2 As Int32,
                                                    ByVal entita_cod_risultato As Int32,
                                                    ByVal statoElaborazione As Int32,
                                                    ByVal messaggi As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByVal Optional entita_GUID_1 As String = "",
                                                    ByVal Optional entita_GUID_2 As String = "",
                                                    ByVal Optional entita_GUID_Risultato As String = "",
                                                    ByVal Optional risultatoJson As String = "") As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.AggiornaStatusEsecuzioneAlgoritmo()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" INSERT INTO GIS_LayerAnalysisConfig_Exec_Log_Details ( ")
            StrSQL.AppendLine(" GIS_LayerAnalysisConfig_Exec_Log_Details_Cod ")
            StrSQL.AppendLine(" , GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine(" , LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine(" , Entita_cod_1 ")
            StrSQL.AppendLine(" , Entita_cod_2 ")
            StrSQL.AppendLine(" , Entita_cod_Risultato ")

            If Not entita_GUID_1.Equals("") Then
                StrSQL.AppendLine(" , Entita_GUID_1 ")
            End If
            If Not entita_GUID_2.Equals("") Then
                StrSQL.AppendLine(" , Entita_GUID_2 ")
            End If
            If Not entita_GUID_Risultato.Equals("") Then
                StrSQL.AppendLine(" , Entita_GUID_Risultato ")
            End If

            If Not risultatoJson.Equals("") Then
                StrSQL.AppendLine(" , Risultato_Json ")
            End If

            StrSQL.AppendLine(" , StatoElaborazione ")
            StrSQL.AppendLine(" , Messaggi ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")

            StrSQL.AppendLine(" ) VALUES ( ")

            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(newDetailLogID)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(esecuzione_cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(configurazione_cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(entita_cod_1)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(entita_cod_2)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(entita_cod_risultato)))

            If Not entita_GUID_1.Equals("") Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(entita_GUID_1)))
            End If
            If Not entita_GUID_2.Equals("") Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(entita_GUID_2)))
            End If
            If Not entita_GUID_Risultato.Equals("") Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(entita_GUID_Risultato)))
            End If

            If Not risultatoJson.Equals("") Then
                StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(risultatoJson)))
            End If

            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(statoElaborazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(messaggi)))

            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))

            StrSQL.AppendLine(" ) ")

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

    Public Function UpsertLogEsecuzioneAlgoritmoSat(ByVal newDetailLogID As Int32,
                                                    ByVal esecuzione_cod As Int32,
                                                    ByVal configurazione_cod As Int32,
                                                    ByVal entita_cod_1 As Int32,
                                                    ByVal entita_cod_2 As Int32,
                                                    ByVal entita_cod_risultato As Int32,
                                                    ByVal statoElaborazione As Int32,
                                                    ByVal messaggi As String,
                                                    ByVal codiceSensore As String,
                                                    ByRef objParametri As AgronicaCoreParametri,
                                                    ByVal Optional entita_GUID_1 As String = "",
                                                    ByVal Optional entita_GUID_2 As String = "",
                                                    ByVal Optional entita_GUID_Risultato As String = "",
                                                    ByVal Optional risultatoJson As String = "") As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.UpsertLogEsecuzioneAlgoritmoSat()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" MERGE dbo.GIS_LayerAnalysisConfig_Exec_Log_Details WITH (HOLDLOCK) AS T ")
            StrSQL.AppendLine(" USING ( SELECT ")
            StrSQL.AppendLine(String.Format("     {0} AS new_cod ", Agro_SQL_SaveNum(newDetailLogID)))
            StrSQL.AppendLine(String.Format("   , {0} AS new_exec_cod ", Agro_SQL_SaveNum(esecuzione_cod)))
            StrSQL.AppendLine(String.Format("   , {0} AS new_cfg_cod ", Agro_SQL_SaveNum(configurazione_cod)))
            StrSQL.AppendLine(String.Format("   , {0} AS new_ent1 ", Agro_SQL_SaveNum(entita_cod_1)))
            StrSQL.AppendLine(String.Format("   , {0} AS new_ent2 ", Agro_SQL_SaveNum(entita_cod_2)))
            StrSQL.AppendLine(String.Format("   , {0} AS new_ent_ris ", Agro_SQL_SaveNum(entita_cod_risultato)))
            StrSQL.AppendLine(String.Format("   , {0} AS new_stato ", Agro_SQL_SaveNum(statoElaborazione)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_messaggi ", Agro_SQL_SaveText(messaggi)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_guid1 ", Agro_SQL_SaveText(entita_GUID_1)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_guid2 ", Agro_SQL_SaveText(entita_GUID_2)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_guid_ris ", Agro_SQL_SaveText(entita_GUID_Risultato)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_json ", Agro_SQL_SaveText(risultatoJson)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_sensore ", Agro_SQL_SaveText(codiceSensore)))
            StrSQL.AppendLine(String.Format("   , '{0}' AS new_username ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(" ) AS S ")
            StrSQL.AppendLine(" ON ( ")
            StrSQL.AppendLine("     T.LayerAnalysisConfig_Cod = S.new_cfg_cod ")
            StrSQL.AppendLine("     AND T.Entita_cod_1 = S.new_ent1 ")
            StrSQL.AppendLine("     AND S.new_sensore <> '' ")
            StrSQL.AppendLine("     AND CHARINDEX('""CodiceSensore"":""' + S.new_sensore + '""', T.Risultato_Json) > 0 ")
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" WHEN MATCHED THEN UPDATE SET ")
            StrSQL.AppendLine("     T.Risultato_Json    = S.new_json ")
            StrSQL.AppendLine("   , T.Messaggi          = S.new_messaggi ")
            StrSQL.AppendLine("   , T.StatoElaborazione = S.new_stato ")
            StrSQL.AppendLine("   , T.Data_Modifica     = GETDATE() ")
            StrSQL.AppendLine("   , T.Username_Modifica = S.new_username ")
            StrSQL.AppendLine(" WHEN NOT MATCHED THEN INSERT ( ")
            StrSQL.AppendLine("     GIS_LayerAnalysisConfig_Exec_Log_Details_Cod ")
            StrSQL.AppendLine("   , GIS_LayerAnalysisConfig_Exec_Log_Cod ")
            StrSQL.AppendLine("   , LayerAnalysisConfig_Cod ")
            StrSQL.AppendLine("   , Entita_cod_1 ")
            StrSQL.AppendLine("   , Entita_cod_2 ")
            StrSQL.AppendLine("   , Entita_cod_Risultato ")
            StrSQL.AppendLine("   , Entita_GUID_1 ")
            StrSQL.AppendLine("   , Entita_GUID_2 ")
            StrSQL.AppendLine("   , Entita_GUID_Risultato ")
            StrSQL.AppendLine("   , Risultato_Json ")
            StrSQL.AppendLine("   , StatoElaborazione ")
            StrSQL.AppendLine("   , Messaggi ")
            StrSQL.AppendLine("   , Username_Creazione ")
            StrSQL.AppendLine("   , Username_Modifica ")
            StrSQL.AppendLine(" ) VALUES ( ")
            StrSQL.AppendLine("     S.new_cod ")
            StrSQL.AppendLine("   , S.new_exec_cod ")
            StrSQL.AppendLine("   , S.new_cfg_cod ")
            StrSQL.AppendLine("   , S.new_ent1 ")
            StrSQL.AppendLine("   , S.new_ent2 ")
            StrSQL.AppendLine("   , S.new_ent_ris ")
            StrSQL.AppendLine("   , S.new_guid1 ")
            StrSQL.AppendLine("   , S.new_guid2 ")
            StrSQL.AppendLine("   , S.new_guid_ris ")
            StrSQL.AppendLine("   , S.new_json ")
            StrSQL.AppendLine("   , S.new_stato ")
            StrSQL.AppendLine("   , S.new_messaggi ")
            StrSQL.AppendLine("   , S.new_username ")
            StrSQL.AppendLine("   , S.new_username ")
            StrSQL.AppendLine(" ) ; ")

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

    Public Function SalvaGUIDConfigurazione(ByVal layerAnalysisConfig_Cod As Int32,
                                            ByVal layerAnalysisConfig_GUID As String,
                                            ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SalvaGUIDConfigurazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig SET ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_GUID = '{0}' ", Agro_SQL_SaveText(layerAnalysisConfig_GUID)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(layerAnalysisConfig_Cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not xRisp Then Return xRisp

            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_DataStruct SET ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_GUID = '{0}' ", Agro_SQL_SaveText(layerAnalysisConfig_GUID)))

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(layerAnalysisConfig_Cod)))

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

    Public Function SalvaGUIDEsecuzione(ByVal esecuzione_cod As Integer,
                                        ByVal newGuidEsecuzione As String,
                                        ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.SalvaGUIDEsecuzione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_Exec_Log SET ")

            StrSQL.AppendLine(String.Format(" Elaborazione_GUID = '{0}' ", Agro_SQL_SaveText(newGuidEsecuzione)))
            StrSQL.AppendLine(" , Data_Modifica = GETDATE() ")

            StrSQL.AppendLine(" WHERE ")

            StrSQL.AppendLine(String.Format(" GIS_LayerAnalysisConfig_Exec_Log_Cod = {0} ", Agro_SQL_SaveNum(esecuzione_cod)))

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

    Public Function EliminaConfigurazione(ByVal configurazioneProiezione_Cod As Int32,
                                          ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.EliminaConfigurazione()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(" DELETE FROM GIS_LayerAnalysisConfig_DataStruct ")
            StrSQL.AppendLine(String.Format(" WHERE LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))

            '--------------------------------------------------------------------------
            xRisp = EseguiQuery_Scrittura(objParametri, StrSQL.ToString, NomeRoutine)
            '--------------------------------------------------------------------------

            If Not xRisp Then
                Throw New Exception("Errore nella cancellazione della configurazione.")
            End If

            StrSQL.Clear()

            StrSQL.AppendLine(" DELETE FROM GIS_LayerAnalysisConfig ")
            StrSQL.AppendLine(String.Format(" WHERE LayerAnalysisConfig_Cod = {0} ", Agro_SQL_SaveNum(configurazioneProiezione_Cod)))

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

    Public Function InserisciChronoImage(ByVal Entita_Cod As Integer,
                                         ByVal Data_Immagine As DateTime,
                                         ByVal Url_Immagine As String,
                                         ByVal AttivoPerElaborazione As Integer,
                                         ByRef objParametri As AgronicaCoreParametri
                                        ) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.InserisciChronoImage()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(String.Format(" IF NOT EXISTS(Select 1 from GIS_LayerAnalysisConfig_ChronoImage where Entita_Cod={0} and Data_Immagine = {1})", Agro_SQL_SaveNum(Entita_Cod), Agro_SQL_SaveDateTime(Data_Immagine)))
            StrSQL.AppendLine(" INSERT INTO GIS_LayerAnalysisConfig_ChronoImage ( ")
            StrSQL.AppendLine("   Entita_Cod ")
            StrSQL.AppendLine(" , Data_Immagine ")
            StrSQL.AppendLine(" , Url_Immagine ")
            StrSQL.AppendLine(" , AttivoPerElaborazione ")
            StrSQL.AppendLine(" , Inviato ")
            StrSQL.AppendLine(" , Data_Creazione ")
            StrSQL.AppendLine(" , Data_Modifica ")
            StrSQL.AppendLine(" , Username_Creazione ")
            StrSQL.AppendLine(" , Username_Modifica ")
            StrSQL.AppendLine(" , Validita_Inizio ")
            StrSQL.AppendLine(" , Validita_Fine ")
            StrSQL.AppendLine(" ) VALUES ( ")
            StrSQL.AppendLine(String.Format(" {0} ", Agro_SQL_SaveNum(Entita_Cod)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(Data_Immagine)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(Url_Immagine)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(AttivoPerElaborazione)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveNum(0)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , '{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(AGRODATAINIZIO)))
            StrSQL.AppendLine(String.Format(" , {0} ", Agro_SQL_SaveDateTime(AGRODATAFINE)))
            StrSQL.AppendLine(" ) ")
            StrSQL.AppendLine(" ELSE ")
            StrSQL.AppendLine(" UPDATE GIS_LayerAnalysisConfig_ChronoImage set ")
            StrSQL.AppendLine(String.Format("  Url_Immagine='{0}' ", Agro_SQL_SaveText(Url_Immagine)))
            StrSQL.AppendLine(String.Format(", Username_Modifica='{0}' ", Agro_SQL_SaveText(objParametri.UsernameOperazione)))
            StrSQL.AppendLine(String.Format(", Data_Modifica={0} ", Agro_SQL_SaveDateTime(DateTime.Now)))
            StrSQL.AppendLine(" WHERE 1=1 ")
            StrSQL.AppendLine(String.Format(" AND Entita_Cod={0} ", Agro_SQL_SaveNum(Entita_Cod)))
            StrSQL.AppendLine(String.Format(" AND Data_Immagine={0} ", Agro_SQL_SaveDateTime(Data_Immagine)))


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

    Public Function AssociaRichiesteEsecuzioneATagname(ByVal BatchSize As Integer,
                                                       ByVal TagExecution As String,
                                                       ByVal NoConf As Boolean,
                                                       ByRef objParametri As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "AgronicaCoreGisDAL.ProiezioniLayer_W.AssociaRichiestaEsecuzioneATagname()"

        Dim MessaggioErrore As String = ""
        Dim StrSQL As New System.Text.StringBuilder
        Dim xRisp As Boolean = False

        Try
            '---------------------------------------------
            StrSQL.Length = 0

            StrSQL.AppendLine(String.Format(" update dest With (ROWLOCK) Set dest.TagExecution=Case When (source.TagExecution Is null or source.TagExecution='') then '{0}' else source.TagExecution end  ", Agro_SQL_SaveText(TagExecution)))
            StrSQL.AppendLine("      from ")
            StrSQL.AppendLine("      GIS_LayerAnalysisConfig_Exec_Log dest inner join ")
            StrSQL.AppendLine(String.Format("  ( select top ({0}) a.GIS_LayerAnalysisConfig_Exec_Log_Cod ", Agro_SQL_SaveNum(BatchSize)))
            StrSQL.AppendLine(" 	,a.group_id ")
            StrSQL.AppendLine(" 	,a.TagExecution ")
            StrSQL.AppendLine("    From GIS_LayerAnalysisConfig_Exec_Log a WITH (UPDLOCK,ROWLOCK,READPAST) left Join ")
            StrSQL.AppendLine("  (Select group_id, MAX(GIS_LayerAnalysisConfig_Exec_Log_Cod) As GIS_LayerAnalysisConfig_Exec_Log_Cod from GIS_LayerAnalysisConfig_Exec_Log  WITH (UPDLOCK,ROWLOCK,READPAST) ")
            StrSQL.AppendLine("      where TagExecution <> '' ")
            If NoConf Then
                StrSQL.AppendLine(" And LayerAnalysisConfig_Cod = 0 ")
            Else
                StrSQL.AppendLine(" And LayerAnalysisConfig_Cod > 0 ")
            End If
            StrSQL.AppendLine(" Group by group_id) b ")
            StrSQL.AppendLine(" On a.group_id=b.group_id ")
            StrSQL.AppendLine(String.Format("  where  (a.AttivoPerElaborazione = 1 And (a.TagExecution is null or a.TagExecution ='' or a.TagExecution='{0}') ", Agro_SQL_SaveText(TagExecution)))
            If NoConf Then
                StrSQL.AppendLine(" And a.LayerAnalysisConfig_Cod = 0 ")
            Else
                StrSQL.AppendLine(" And a.LayerAnalysisConfig_Cod > 0 ")
            End If
            StrSQL.AppendLine("  ) ")
            StrSQL.AppendLine(" order by a.Group_Id asc ) source ")
            StrSQL.AppendLine(" On dest.GIS_LayerAnalysisConfig_Exec_Log_Cod=source.GIS_LayerAnalysisConfig_Exec_Log_Cod and dest.group_id=source.group_id And dest.AttivoPerElaborazione=1 And (dest.TagExecution is null or dest.TagExecution='') ")
            StrSQL.AppendLine(" And (dest.TagExecution is null or dest.TagExecution='') ")
            If NoConf Then
                StrSQL.AppendLine(" And dest.LayerAnalysisConfig_Cod = 0 ")
            Else
                StrSQL.AppendLine(" And dest.LayerAnalysisConfig_Cod > 0 ")
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
