
Imports System.Text
Imports AgronicaCoreDataProvider
Imports AgronicaCoreDataProvider.TipiEnumerativi

Public Class IOT
    Inherits AgronicaCoreDataProvider.DataProvider

    Public Function LeggiDispositivo(id_dispositivo As Integer, needSensors As Boolean, objParametriServer As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "IOT.LeggiDispositivo"

        Try

            Dim sql As New StringBuilder

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ID INTEGER = " & id_dispositivo.ToString())
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	z.Id ")
            sql.AppendLine("	, Dispositivo = z.Nome ")
            sql.AppendLine("	, Lat = COALESCE(GeoEntity.EnvelopeCenter().Lat, Lat_Dec) ")
            sql.AppendLine("	, Lng = COALESCE(GeoEntity.EnvelopeCenter().Long, Lng_Dec) ")
            sql.AppendLine("	, FlagReale ")
            sql.AppendLine("	, RifFornitore ")
            sql.AppendLine("	, Fornitore = f.Descrizione ")
            sql.AppendLine("FROM IoT_Dispositivi z ")
            sql.AppendLine("INNER JOIN IoT_Fornitori f ON f.Id = z.Id_Fornitore ")
            sql.AppendLine("WHERE z.Id = @ID ")

            If needSensors Then
                sql.AppendLine()
                sql.AppendLine("SELECT ")
                sql.AppendLine("	sxs1.Id_Sensore ")
                sql.AppendLine("	, Sensore = COALESCE(sxs1.Etichetta, s1.Etichetta, '') ")
                sql.AppendLine("	, Tipo = t1.Tipo ")
                sql.AppendLine("	, t1.UM ")
                sql.AppendLine("	, Id_DispositivoOrigine = z2.Id ")
                sql.AppendLine("	, DispositivoOrigine = z2.Nome ")
                sql.AppendLine("	, dati.Last_Update ")
                sql.AppendLine("FROM IoT_DispositiviXSensori sxs1 ")
                sql.AppendLine("INNER JOIN IoT_Sensori s1 on s1.Id = sxs1.Id_Sensore ")
                sql.AppendLine("INNER JOIN IoT_TipiSensore t1 ON t1.Id = s1.Id_TipoSensore ")
                sql.AppendLine("INNER JOIN IoT_Dispositivi z1 ON z1.Id = sxs1.Id_Dispositivo ")
                sql.AppendLine("LEFT JOIN IoT_DispositiviXSensori sxs2 ON sxs2.Id_Sensore = sxs1.Id_Sensore AND z1.FlagReale = 0 ")
                sql.AppendLine("LEFT JOIN IoT_Dispositivi z2 ON z2.Id = sxs2.Id_Dispositivo AND z2.FlagReale = 1 ")
                sql.AppendLine("LEFT JOIN ( ")
                sql.AppendLine("	SELECT Id_Sensore, Last_Update = MAX(DataOra) FROM IoT_DatiOrari GROUP BY Id_Sensore ")
                sql.AppendLine(") dati ON dati.Id_Sensore = sxs1.Id_Sensore ")
                sql.AppendLine("WHERE z1.Id = @ID AND (z1.FlagReale = 1 OR z2.Id IS NOT NULL) ")
                sql.AppendLine("ORDER BY sxs1.Ordine ")
            End If

            Dim DS As New DataSet

            EseguiQuery_Lettura(objParametriServer, sql.ToString(), NomeRoutine, DS, "Dati_IoT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)

        End Try

        Return Nothing
    End Function

    Public Function LeggiTipologiaDispositivi(PIVA_Superuser As String, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IOT.LeggiTipologiaDispositivi"

        Try

            'enum_Meteo_Tiposorgente.Gias_RER               "Gias (Stazioni Regione E.R.)"
            'enum_Meteo_Tiposorgente.Gias_RER_Quadranti     "Gias (Quadranti Regione E.R.)"
            'enum_Meteo_Tiposorgente.Pubbliche              "Stazioni pubbliche"
            'enum_Meteo_Tiposorgente.RetiPartner            "Tutte le stazioni in visibilità"
            'enum_Meteo_Tiposorgente.Aziendali              "Solo le stazioni aziendali"

            'TODO: Tabella che permetta la gestione delle sorgenti meteo visibili da PIVA_Superuser diverse...
            Dim sql As String = "SELECT Id, Descrizione FROM IoT_Sorgenti ORDER By Ordine"

            Return EseguiQuery_Lettura(ObjParametri_Server, sql, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)

        End Try

        Return Nothing
    End Function

    Public Function LeggiDispositiviConDistanza(ByVal TipoSorgente As enum_IoT_Tiposorgente, ByVal lat As Double, ByVal lng As Double, ByVal piva_superuser As String, ByVal piva As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IOT.LeggiDispositiviConDistanza"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim sql_T1 As New StringBuilder
        Dim sql_T2 As New StringBuilder
        Dim DT As DataTable = Nothing

        Try

            sql_T1.Clear()
            sql_T2.Clear()

            Select Case TipoSorgente

            '    Case enum_Meteo_Tiposorgente.Gias_RER

                '        sql_T1.AppendLine("     SELECT S.ID_Stazione AS ID, Stazione_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ")
                '        sql_T1.AppendLine("     FROM TB_Stazioni S ")
                '        sql_T1.AppendLine("		INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ")
                '        sql_T1.AppendLine("		INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ")

                '        sql_T2.AppendLine("		SELECT S.ID_Stazione AS ID, MAX(Tempo) AS UltimoAggiornamento ")
                '        sql_T2.AppendLine("		FROM TB_Stazioni S ")
                '        sql_T2.AppendLine("		LEFT JOIN Dati_Meteo_SHH D ON D.ID_Stazione = S.ID_Stazione ")
                '        sql_T2.AppendLine("		GROUP BY S.ID_Stazione")

                '    Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                '        sql_T1.AppendLine("     SELECT Q.ID_Quadrante AS ID, Quadrante_Des AS Descrizione, '' AS Fornitore, '' AS RifFornitore, replace(replace(Q.X_LON_GC, 'E', ''), ',', '.') AS Lat, replace(replace(Q.Y_LAT_GC, 'N', ''), ',', '.') AS Lng ")
                '        sql_T1.AppendLine("     FROM TB_Quadranti Q ")

                '        sql_T2.AppendLine("		SELECT Q.ID_Quadrante AS ID, MAX(Tempo) AS UltimoAggiornamento ")
                '        sql_T2.AppendLine("		FROM TB_Quadranti Q ")
                '        sql_T2.AppendLine("		LEFT JOIN Dati_Meteo_QHH D ON D.ID_Quadrante = Q.ID_Quadrante ")
                '        sql_T2.AppendLine("		GROUP BY Q.ID_Quadrante ")

                Case enum_IoT_Tiposorgente.Aziendali

                    sql_T1.AppendLine("		SELECT ")
                    sql_T1.AppendLine("         ID = S.Id ")
                    sql_T1.AppendLine("         , Descrizione = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
                    sql_T1.AppendLine("         , Fornitore = CASE WHEN vis.Anonima = 0 THEN COALESCE(vis.DescrizioneAggiuntiva, F.Descrizione, '') ELSE '' END ")
                    sql_T1.AppendLine("         , RifFornitore = CASE WHEN vis.Anonima = 0 THEN S.RifFornitore ELSE '' END ")
                    sql_T1.AppendLine("         , Lat = CAST(S.Lat_Dec AS VARCHAR(50)) ")
                    sql_T1.AppendLine("         , Lng = CAST(S.Lng_Dec AS VARCHAR(50)) ")
                    sql_T1.AppendLine("     FROM IoT_Dispositivi S ")
                    sql_T1.AppendLine("     INNER JOIN IoT_Fornitori F ON F.Id = S.Id_Fornitore ")
                    sql_T1.AppendLine("     INNER JOIN ( ")
                    sql_T1.AppendLine("         SELECT Id_Dispositivo, DescrizioneAggiuntiva, Anonima FROM ")
                    sql_T1.AppendLine("         ( ")
                    sql_T1.AppendLine("             SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima, ROW_NUMBER() OVER(PARTITION BY Id_Dispositivo ORDER BY Id_Dispositivo ASC, Proprietario DESC) AS Cnt ")
                    sql_T1.AppendLine("             FROM ( ")
                    sql_T1.AppendLine("		                SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("		                FROM IoT_VisibilitaDispositivi ")
                    sql_T1.AppendLine("                     WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(piva_superuser) & "' AND PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("		                UNION ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("             		SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("		                FROM IoT_VisibilitaDispositivi ")
                    sql_T1.AppendLine("		                WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(piva_superuser) & "' AND PIVA = '*' ")
                    sql_T1.AppendLine("             ) TBL1 ")

                    If TipoSorgente = enum_IoT_Tiposorgente.Aziendali Then
                        sql_T1.AppendLine("             WHERE Proprietario = 1 --se solo dispositivi aziendali ")
                    End If

                    sql_T1.AppendLine("         ) TBL2 ")
                    sql_T1.AppendLine("         WHERE Cnt = 1 ")
                    sql_T1.AppendLine("     ) vis on vis.Id_Dispositivo = S.Id")

                    sql_T2.AppendLine("		SELECT Id_Dispositivo AS ID, MAX(UltimoAggiornamento) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		FROM IoT_DispositiviXSensori sxs ")
                    sql_T2.AppendLine("		LEFT JOIN (SELECT Id_Sensore, MAX(DataOra) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("		    FROM IoT_DatiOrari ")
                    sql_T2.AppendLine("		    GROUP BY Id_Sensore) s ON s.Id_Sensore = sxs.Id_Sensore ")
                    sql_T2.AppendLine("		GROUP BY Id_Dispositivo ")

            End Select

            sql.Clear()

            sql.AppendLine("DECLARE @LAT FLOAT = " & lat.ToString(Globalization.CultureInfo.InvariantCulture))
            sql.AppendLine("DECLARE @LNG FLOAT = " & lng.ToString(Globalization.CultureInfo.InvariantCulture))
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	CASE WHEN Distanza IS NULL THEN 1 ELSE 0 END AS Ordine ")
            sql.AppendLine("	, * ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		T1.ID ")
            sql.AppendLine("		, T1.Descrizione ")
            sql.AppendLine("		, T1.Fornitore ")
            sql.AppendLine("		, T1.RifFornitore ")
            sql.AppendLine("		, T2.UltimoAggiornamento ")
            sql.AppendLine("		, CASE WHEN (Lat IS NULL OR Lng IS NULL OR @LAT = 0 OR @LNG = 0) THEN NULL ELSE ")
            sql.AppendLine("			geography::STGeomFromText('POINT(' + T1.Lng + ' ' + T1.Lat + ')', 4326).STDistance(geography::STGeomFromText('POINT(' + CAST(@LNG AS VARCHAR(50)) + ' ' + CAST(@LAT AS VARCHAR(50)) + ')', 4326)) ")
            sql.AppendLine("		END AS Distanza ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine()
            sql.Append(sql_T1.ToString())
            sql.AppendLine()
            sql.AppendLine("	) T1 ")
            sql.AppendLine("	INNER JOIN ( ")
            sql.AppendLine()
            sql.Append(sql_T2.ToString())
            sql.AppendLine()
            sql.AppendLine("	) T2 ON T2.ID = T1.ID ")
            sql.AppendLine(") T3 ")
            sql.AppendLine("ORDER BY Ordine, Distanza, Descrizione ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT

    End Function

    Public Class LatLng
        Public Lat As Decimal
        Public Lng As Decimal
    End Class

    Public Function LeggiDispositiviAutorizzati(piva_superuser As String, piva As String, tipo_sorgente As Integer, coord As LatLng, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IOT.LeggiDispositiviAutorizzati"
        Dim DT As DataTable

        Try

            Dim sql As New StringBuilder
            Dim sql_T1 As New StringBuilder
            Dim sql_T2 As New StringBuilder

            sql.Clear()
            sql_T1.Clear()
            sql_T2.Clear()

            Select Case tipo_sorgente

                Case enum_IoT_Tiposorgente.Aziendali

                    sql_T1.AppendLine("     SELECT ")
                    sql_T1.AppendLine("         Id_Dispositivo = S.Id ")
                    sql_T1.AppendLine("         , Nome_Dispositivo = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
                    sql_T1.AppendLine("			, Lat = Lat_Dec ")
                    sql_T1.AppendLine("			, Lng = Lng_Dec ")
                    sql_T1.AppendLine("			, Fornitore = F.Descrizione ")
                    sql_T1.AppendLine("			, RifFornitore ")
                    sql_T1.AppendLine("			, FlagReale ")
                    sql_T1.AppendLine(" 	FROM IoT_Dispositivi S ")
                    sql_T1.AppendLine("     INNER JOIN IoT_Fornitori F ON F.Id = S.Id_Fornitore ")
                    sql_T1.AppendLine("     INNER JOIN ( ")
                    sql_T1.AppendLine("		    SELECT Id_Dispositivo, DescrizioneAggiuntiva, Anonima FROM ")
                    sql_T1.AppendLine("		    ( ")
                    sql_T1.AppendLine("			    SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima, ROW_NUMBER() OVER(PARTITION BY Id_Dispositivo ORDER BY Id_Dispositivo ASC, Proprietario DESC) AS Cnt ")
                    sql_T1.AppendLine("	            FROM ( ")
                    sql_T1.AppendLine("				    SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("				    FROM IoT_VisibilitaDispositivi ")
                    sql_T1.AppendLine("				    WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(piva_superuser) & "' AND PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("				    UNION ")
                    sql_T1.AppendLine()
                    sql_T1.AppendLine("				    SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql_T1.AppendLine("				    FROM IoT_VisibilitaDispositivi ")
                    sql_T1.AppendLine("				    WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(piva_superuser) & "' AND PIVA = '*' ")
                    sql_T1.AppendLine("		        ) TBL1 ")

                    If tipo_sorgente = enum_IoT_Tiposorgente.Aziendali Then
                        sql_T1.AppendLine("             WHERE Proprietario = 1 --se solo dispositivi aziendali ")
                    End If

                    sql_T1.AppendLine("         ) TBL2 ")
                    sql_T1.AppendLine("         WHERE Cnt = 1 ")
                    sql_T1.AppendLine("     ) vis on vis.Id_Dispositivo = S.Id ")

                    sql_T2.AppendLine("     SELECT Id_Dispositivo AS ID, MAX(UltimoAggiornamento) AS UltimoAggiornamento ")
                    sql_T2.AppendLine("     FROM IoT_DispositiviXSensori sxs ")
                    sql_T2.AppendLine("     LEFT JOIN ( ")
                    sql_T2.AppendLine("         SELECT Id_Sensore, MAX(DataOra) AS UltimoAggiornamento FROM IoT_DatiOrari GROUP BY Id_Sensore ")
                    sql_T2.AppendLine("     ) s ON s.Id_Sensore = sxs.Id_Sensore ")
                    sql_T2.AppendLine("		GROUP BY Id_Dispositivo ")

                Case enum_IoT_Tiposorgente.Pubbliche

                    If coord Is Nothing Then

                        Return Nothing
                    End If

                    sql_T1.AppendLine("	SELECT")
                    sql_T1.AppendLine("		Id_Dispositivo = S.Id")
                    sql_T1.AppendLine("		, Nome_Dispositivo = F.Descrizione + ' (' + S.Nome + ')'")
                    sql_T1.AppendLine("		, Lat = S.GeoEntity.EnvelopeCenter().Lat")
                    sql_T1.AppendLine("		, Lng = S.GeoEntity.EnvelopeCenter().Long")
                    sql_T1.AppendLine("		, Fornitore = F.Descrizione")
                    sql_T1.AppendLine("		, RifFornitore")
                    sql_T1.AppendLine("		, FlagReale")
                    sql_T1.AppendLine("	FROM (")
                    sql_T1.AppendLine("		SELECT TOP 1")
                    sql_T1.AppendLine("			Id_Dispositivo = Id")
                    sql_T1.AppendLine("			, Dist = GeoEntity.STDistance(@SRCPOINT)")
                    sql_T1.AppendLine("		FROM IoT_Dispositivi")
                    sql_T1.AppendLine("		WHERE Categoria = 1")
                    sql_T1.AppendLine("		ORDER BY Dist")
                    sql_T1.AppendLine("	) Z")
                    sql_T1.AppendLine("	INNER JOIN IoT_Dispositivi S ON S.Id = Z.Id_Dispositivo")
                    sql_T1.AppendLine("	INNER JOIN IoT_Fornitori F ON F.Id = S.Id_Fornitore")

                    sql_T2.AppendLine("	SELECT sxs.Id_Dispositivo AS ID, MAX(UltimoAggiornamento) AS UltimoAggiornamento")
                    sql_T2.AppendLine("	FROM IoT_DispositiviXSensori sxs")
                    sql_T2.AppendLine("	INNER JOIN DISP_CTE z on z.Id_Dispositivo = sxs.Id_Dispositivo")
                    sql_T2.AppendLine("    LEFT JOIN (")
                    sql_T2.AppendLine("		SELECT Id_Sensore, MAX(DataOra) AS UltimoAggiornamento FROM IoT_DatiOrari GROUP BY Id_Sensore")
                    sql_T2.AppendLine("	) s ON s.Id_Sensore = sxs.Id_Sensore")
                    sql_T2.AppendLine("	GROUP BY sxs.Id_Dispositivo")

            End Select

            sql.AppendLine("DECLARE @SRCPOINT AS GEOGRAPHY = NULL ")

            If coord IsNot Nothing Then

                sql.AppendLine("SET @SRCPOINT = GEOGRAPHY::Point(" & coord.Lat.ToString(Globalization.CultureInfo.InvariantCulture) & ", " & coord.Lng.ToString(Globalization.CultureInfo.InvariantCulture) & ", 4326) ")
            End If

            sql.AppendLine()
            sql.AppendLine(";WITH DISP_CTE AS ( ")
            sql.Append(sql_T1.ToString())
            sql.AppendLine("), ")
            sql.AppendLine("AGG_CTE AS ( ")
            sql.Append(sql_T2.ToString())
            sql.AppendLine(") ")
            sql.AppendLine()
            sql.AppendLine("SELECT Ordine = CASE WHEN Distanza IS NULL THEN 1 ELSE 0 END, * ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		S.* ")
            sql.AppendLine("        , A.UltimoAggiornamento ")
            sql.AppendLine("		, Distanza = CASE WHEN (@SRCPOINT IS NULL OR Lat IS NULL OR Lng IS NULL) THEN NULL ELSE ")
            sql.AppendLine("			GEOGRAPHY::Point(S.Lat, S.Lng, 4326).STDistance(@SRCPOINT) ")
            sql.AppendLine("		END ")
            sql.AppendLine("	FROM DISP_CTE S ")
            sql.AppendLine("	LEFT JOIN AGG_CTE A ON A.ID = S.Id_Dispositivo ")
            sql.AppendLine(") T ")
            sql.AppendLine("ORDER BY Ordine, Distanza, Nome_Dispositivo ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return DT
    End Function


    Public Function LeggiAnagraficaDispositivi(piva_superuser As String, piva As String, id_dispositivo As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IoT.LeggiAnagraficaDispositivi"
        Dim DT As DataTable

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("DECLARE @PIVA_SUPERUSER VARCHAR(50) = '" & Agro_SQL_SaveText(piva_superuser) & "' ")
            sql.AppendLine("DECLARE @PIVA VARCHAR(50) = '" & Agro_SQL_SaveText(piva) & "' ")
            sql.AppendLine("DECLARE @NO_UPDATE DATETIME = 0 ")
            sql.AppendLine()
            sql.AppendLine(";WITH DISPOSITIVI_AUTORIZZATI_CTE AS ( ")
            sql.AppendLine("	SELECT disp.Id, FlagReale, aut.Proprietario ")
            sql.AppendLine("	FROM IoT_Dispositivi disp ")
            sql.AppendLine("	INNER JOIN ( ")
            sql.AppendLine("		SELECT Id_Dispositivo, Proprietario FROM ( ")
            sql.AppendLine("			SELECT Id_Dispositivo, Proprietario, RowNum = ROW_NUMBER() OVER(PARTITION BY Id_Dispositivo ORDER BY Id_Dispositivo ASC, Proprietario DESC) ")
            sql.AppendLine("				FROM ( ")
            sql.AppendLine("					SELECT Id_Dispositivo, Proprietario FROM IoT_VisibilitaDispositivi WHERE PIVA_Superuser = @PIVA_SUPERUSER And PIVA = @PIVA And Anonima = 0 ")
            sql.AppendLine("					UNION ")
            sql.AppendLine("					SELECT Id_Dispositivo, Proprietario FROM IoT_VisibilitaDispositivi WHERE PIVA_Superuser = @PIVA_SUPERUSER And PIVA = '*' AND Anonima = 0 ")
            sql.AppendLine("				) T ")
            sql.AppendLine("		) T WHERE RowNum = 1 ")
            sql.AppendLine("	) aut ON aut.Id_Dispositivo = disp.Id ")
            sql.AppendLine("), ")
            sql.AppendLine()
            sql.AppendLine("DISPOSITIVI_SENSORI_CTE AS ( ")
            sql.AppendLine("	SELECT Id_Dispositivo = z.Id, z.FlagReale, z.Proprietario, Id_Sensore, sxs.Ordine, Id_DispositivoOrigine = NULL ")
            sql.AppendLine("	FROM DISPOSITIVI_AUTORIZZATI_CTE z ")
            sql.AppendLine("	INNER JOIN IoT_DispositiviXSensori sxs ON sxs.Id_Dispositivo = z.Id ")
            sql.AppendLine("	WHERE z.FlagReale = 1 ")
            sql.AppendLine()
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine()
            sql.AppendLine("	SELECT Id_Dispositivo, FlagReale, Proprietario, Id_Sensore, Ordine, Id_DispositivoOrigine FROM ( ")
            sql.AppendLine("		SELECT Id_Dispositivo = z.Id, z.FlagReale, z.Proprietario, sxs1.Id_Sensore, sxs1.Ordine,Id_DispositivoOrigine = z1.Id, Idx = ROW_NUMBER() OVER (PARTITION BY sxs1.Id_Dispositivo, sxs1.Id_Sensore ORDER BY sxs2.Id_Dispositivo) ")
            sql.AppendLine("		FROM DISPOSITIVI_AUTORIZZATI_CTE z ")
            sql.AppendLine("		INNER JOIN IoT_DispositiviXSensori sxs1 ON sxs1.Id_Dispositivo = z.Id ")
            sql.AppendLine("		LEFT JOIN IoT_DispositiviXSensori sxs2 ON sxs2.Id_Sensore = sxs1.Id_Sensore ")
            sql.AppendLine("		LEFT JOIN IoT_Dispositivi z1 ON z1.Id = sxs2.Id_Dispositivo AND z1.FlagReale = 1 ")
            sql.AppendLine("		WHERE z.FlagReale = 0 ")
            sql.AppendLine("	) T WHERE Idx = 1 ")
            sql.AppendLine(") ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	cte.Id_Dispositivo ")
            sql.AppendLine("	, Fornitore = f.Descrizione ")
            sql.AppendLine("	, Dispositivo = z1.Nome ")
            sql.AppendLine("	, z1.Lat_Dec ")
            sql.AppendLine("	, z1.Lng_Dec ")
            sql.AppendLine("	, cte.FlagReale ")
            sql.AppendLine("	, z1.RifFornitore ")
            sql.AppendLine("	, cte.Proprietario ")
            sql.AppendLine("	, cte.Id_Sensore ")
            sql.AppendLine("	, Sensore = COALESCE(sxs.Etichetta, s.Etichetta, '') ")
            sql.AppendLine("	, Tipo_Sensore = t.Tipo ")
            sql.AppendLine("	, t.UM ")
            sql.AppendLine("	, OutputConfig = COALESCE(sxs.OutputConfig, t.DefaultOutputConfig) ")
            sql.AppendLine("	, Id_DispositivoOrigine = COALESCE(cte.Id_DispositivoOrigine, 0) ")
            sql.AppendLine("	, DispositivoOrigine = COALESCE(z2.Nome, '') ")
            sql.AppendLine("	, Last_Update = COALESCE(dati.Last_Update, @NO_UPDATE) ")
            sql.AppendLine("FROM DISPOSITIVI_SENSORI_CTE cte ")
            sql.AppendLine("INNER JOIN IoT_DispositiviXSensori sxs ON sxs.Id_Dispositivo = cte.Id_Dispositivo AND sxs.Id_Sensore = cte.Id_Sensore ")
            sql.AppendLine("INNER JOIN IoT_Dispositivi z1 ON z1.Id = cte.Id_Dispositivo ")
            sql.AppendLine("LEFT JOIN IoT_Dispositivi z2 ON z2.Id = cte.Id_DispositivoOrigine ")
            sql.AppendLine("INNER JOIN IoT_Sensori s ON s.Id = cte.Id_Sensore ")
            sql.AppendLine("INNER JOIN IoT_TipiSensore t ON t.Id = s.Id_TipoSensore ")
            sql.AppendLine("INNER JOIN IoT_Fornitori f ON f.Id = z1.Id_Fornitore ")
            sql.AppendLine("LEFT JOIN ( ")
            sql.AppendLine("	SELECT Id_Sensore, Last_Update = MAX(DataOra) FROM IoT_DatiOrari GROUP BY Id_Sensore ")
            sql.AppendLine(") dati ON dati.Id_Sensore = cte.Id_Sensore ")

            If id_dispositivo > 0 Then

                sql.AppendLine("WHERE cte.id_dispositivo = " & id_dispositivo.ToString)
            End If

            sql.AppendLine("ORDER BY cte.id_dispositivo, cte.Ordine ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

    Public Function LeggiIdDispositivi(ByVal tblSensori As TblSensoriSorgente, ByVal tblDati As TblDatiSorgente, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "IoT.LeggiIdDispositivi"
        Dim Riepilogo As DataTable

        Try
            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SELECT DISTINCT s." & tblSensori.CampoIdDispositivo)
            sql.AppendLine("	FROM " & tblDati.NomeTabella & " d JOIN " & tblSensori.NomeTabella & " s ON d.Id_Sensore = s.Id ")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Riepilogo
    End Function


    Public Function LeggiTipoSensoriPerDispositivo(ByVal id_dispositivo As Integer, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IoT.LeggiTipoSensoriPerDispositivo"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim DT As DataTable = Nothing

        Try

            sql.Clear()

            sql.AppendLine("DECLARE @ID_DISPOSITIVO INT = " & id_dispositivo & " ")
            sql.AppendLine("DECLARE @NO_UPDATE DATETIME = 0 ")
            sql.AppendLine()
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id_dispositivo ")
            sql.AppendLine("	, Id_Sensore ")
            sql.AppendLine("	, Id_TipoSensore ")
            sql.AppendLine("	, Tipo ")
            sql.AppendLine("	, UM ")
            sql.AppendLine("FROM ")
            sql.AppendLine("    IoT_TipiSensore A ")
            sql.AppendLine("    INNER JOIN IoT_Sensori B on (A.Id=B.Id_TipoSensore) ")
            sql.AppendLine("    INNER JOIN IoT_DispositiviXSensori C on (C.Id_Sensore=B.Id) ")

            If id_dispositivo > 0 Then

                sql.AppendLine("WHERE id_dispositivo = @ID_DISPOSITIVO")
            End If

            sql.AppendLine("ORDER BY id_dispositivo,Id_Sensore ")

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT
    End Function

    Public Class AnagDispositivo
        Public Property Id As Integer
        Public Property Nome As String
        Public Property Lat As Decimal?
        Public Property Lng As Decimal?
        Public Class Sensore
            Public Property Id As Integer
            Public Property Etichetta As String
            Public Property OutputConfig As String
        End Class
        Public Property Sensori As List(Of Sensore)
        Public Property NoteVisibilita As String
        Public Sub New()
            Id = 0
            Nome = ""
            Lat = Nothing
            Lng = Nothing
            Sensori = New List(Of Sensore)
            NoteVisibilita = ""
        End Sub
    End Class

    Public Function AggiornaAnagraficaDispositivi(piva_superuser As String, piva As String, Anag As AnagDispositivo, ObjParametri_Server As AgronicaCoreParametri) As Integer

        Dim NomeRoutine As String = "IoT.AggiornaAnagraficaDispositivi"

        Dim Id_Stazione As Integer = 0
        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @PIVA_SUPERUSER VARCHAR(50) = '" & Agro_SQL_SaveText(piva_superuser) & "' ")
            sql.AppendLine("DECLARE @PIVA VARCHAR(50) = '" & Agro_SQL_SaveText(piva) & "' ")
            sql.AppendLine("DECLARE @ID_DISPOSITIVO AS INTEGER = " & Anag.Id.ToString)
            sql.AppendLine("DECLARE @DISPOSITIVO AS VARCHAR(MAX) = '" & Agro_SQL_SaveText(Anag.Nome) & "' ")
            sql.Append("DECLARE @LAT AS FLOAT = ")
            If Anag.Lat.HasValue Then
                sql.AppendLine(Convert.ToString(Anag.Lat.Value, Globalization.CultureInfo.InvariantCulture))
            Else
                sql.AppendLine("NULL ")
            End If
            sql.Append("DECLARE @LNG AS FLOAT = ")
            If Anag.Lng.HasValue Then
                sql.AppendLine(Convert.ToString(Anag.Lng.Value, Globalization.CultureInfo.InvariantCulture))
            Else
                sql.AppendLine("NULL ")
            End If
            sql.AppendLine()
            sql.AppendLine("DECLARE @SXS AS TABLE (ID_DISPOSITIVO INTEGER, ID_SENSORE INTEGER, ORDINE INTEGER, ETICHETTA VARCHAR(MAX), OUTPUTCONFIG VARCHAR(MAX)) ")
            sql.AppendLine("INSERT INTO @SXS VALUES ")
            Dim ordine As Integer = 1
            For Each sens In Anag.Sensori
                If ordine > 1 Then
                    sql.AppendLine(", ")
                End If
                sql.Append("(@ID_DISPOSITIVO, " & sens.Id.ToString & ", " & ordine.ToString & ", '" & Agro_SQL_SaveText(sens.Etichetta) & "', ")
                If String.IsNullOrEmpty(sens.OutputConfig) Then
                    sql.Append("NULL")
                Else
                    sql.Append("'" & Agro_SQL_SaveText(sens.OutputConfig) & "'")
                End If
                sql.Append(")")
                ordine += 1
            Next
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("IF @ID_DISPOSITIVO <= 0 ")
            sql.AppendLine("BEGIN ")
            sql.AppendLine()
            sql.AppendLine("--Inserimento nuovo dispositivo virtuale ")
            sql.AppendLine()
            sql.AppendLine("	SET @ID_DISPOSITIVO = (SELECT MAX(Id) FROM IoT_Dispositivi) + 1 ")
            sql.AppendLine()
            sql.AppendLine("	DECLARE @ID_FORNITORE AS INTEGER = ( ")
            sql.AppendLine("		SELECT TOP 1 Id_Fornitore FROM ( ")
            sql.AppendLine("			SELECT Id_Fornitore, CNT = COUNT(*) FROM ( ")
            sql.AppendLine("				SELECT staz.Id_Fornitore ")
            sql.AppendLine("				FROM IoT_Dispositivi disp ")
            sql.AppendLine("				INNER JOIN ( ")
            sql.AppendLine("					SELECT sxs_o.id_dispositivo ")
            sql.AppendLine("					FROM @SXS sxs ")
            sql.AppendLine("					INNER JOIN IoT_DispositiviXSensori sxs_o ON sxs.ID_SENSORE = sxs_o.Id_Sensore AND sxs_o.FlagReale = 1 ")
            sql.AppendLine("				) T ON T.id_dispositivo = disp.Id ")
            sql.AppendLine("			) T GROUP BY Id_Fornitore ")
            sql.AppendLine("		) T	ORDER BY CNT DESC ")
            sql.AppendLine("	) ")
            sql.AppendLine()
            sql.AppendLine("	DECLARE @RIFFORNITORE AS VARCHAR(MAX) = ( ")
            sql.AppendLine("		SELECT TOP 1 RifFornitore FROM ( ")
            sql.AppendLine("			SELECT RifFornitore, CNT = COUNT(*) FROM ( ")
            sql.AppendLine("				SELECT RifFornitore = TRIM(SUBSTRING(staz.RifFornitore, 1, CHARINDEX(':', staz.RifFornitore) - 1)) ")
            sql.AppendLine("				FROM IoT_Dispositivi disp ")
            sql.AppendLine("				INNER JOIN ( ")
            sql.AppendLine("					SELECT sxs_o.id_dispositivo ")
            sql.AppendLine("					FROM @SXS sxs ")
            sql.AppendLine("					INNER JOIN IoT_DispositiviXSensori sxs_o ON sxs.ID_SENSORE = sxs_o.Id_Sensore AND sxs_o.FlagReale = 1 ")
            sql.AppendLine("				) T ON T.id_dispositivo = staz.Id ")
            sql.AppendLine("			) T GROUP BY RifFornitore ")
            sql.AppendLine("		) T ORDER BY CNT DESC ")
            sql.AppendLine("	) ")
            sql.AppendLine()
            sql.AppendLine("	DECLARE @NOTE AS VARCHAR(MAX) = '" & Agro_SQL_SaveText(Anag.NoteVisibilita) & "' ")
            sql.AppendLine()
            sql.AppendLine("	INSERT INTO IoT_Dispositivi (Id, Id_Fornitore, Nome, Lat_Dec, Lng_Dec, Altitudine, FlagReale, FlagSIM, RifFornitore, SystemTimeZoneInfo) ")
            sql.AppendLine("	VALUES (@ID_DISPOSITIVO, @ID_FORNITORE, @DISPOSITIVO, @LAT, @LNG, NULL, 0, 0, @RIFFORNITORE, NULL) ")
            sql.AppendLine()
            sql.AppendLine("	INSERT INTO IoT_DispositiviXSensori (id_dispositivo, Id_Sensore, FlagReale, OutputConfig, Ordine, Etichetta) ")
            sql.AppendLine("	SELECT  ")
            sql.AppendLine("		id_dispositivo = @ID_DISPOSITIVO ")
            sql.AppendLine("		, ID_SENSORE ")
            sql.AppendLine("		, FlagReale = 0 ")
            sql.AppendLine("		, OutputConfig ")
            sql.AppendLine("		, Ordine ")
            sql.AppendLine("		, Etichetta ")
            sql.AppendLine("	FROM @SXS ")
            sql.AppendLine()
            sql.AppendLine("	INSERT INTO IoT_VisibilitaDispositivi (id_dispositivo, PIVA_Superuser, PIVA, Proprietario, DescrizioneAggiuntiva, Note, Anonima) ")
            sql.AppendLine("	SELECT id_dispositivo = @ID_DISPOSITIVO, PIVA_Superuser = @PIVA_SUPERUSER, PIVA = @PIVA, Proprietario = 1, DescrizioneAggiuntiva = Descrizione, Note = @NOTE, Anonima = 0 ")
            sql.AppendLine("	FROM IoT_Fornitori WHERE Id = @ID_FORNITORE ")
            sql.AppendLine()
            sql.AppendLine("	SELECT ID_DISPOSITIVO_AGGIORNATO = @ID_DISPOSITIVO ")
            sql.AppendLine("END ")
            sql.AppendLine("ELSE ")
            sql.AppendLine("BEGIN ")
            sql.AppendLine()
            sql.AppendLine("	--Modifica stazione esistente ")
            sql.AppendLine()
            sql.AppendLine("	DECLARE @ALLOW_UPDATE AS INTEGER = ( ")
            sql.AppendLine("		SELECT Proprietario ")
            sql.AppendLine("		FROM IoT_Dispositivi disp  ")
            sql.AppendLine("		INNER JOIN IoT_VisibilitaDispositivi vis ON vis.id_dispositivo = disp.Id AND vis.PIVA_Superuser = @PIVA_SUPERUSER AND vis.PIVA = @PIVA ")
            sql.AppendLine("		WHERE disp.Id = @ID_DISPOSITIVO ")
            sql.AppendLine("	) ")
            sql.AppendLine()
            sql.AppendLine("	SET @ALLOW_UPDATE = (SELECT COALESCE(@ALLOW_UPDATE, 0)) ")
            sql.AppendLine()
            sql.AppendLine("	IF  @ALLOW_UPDATE = 1 ")
            sql.AppendLine("	BEGIN ")
            sql.AppendLine()
            sql.AppendLine("		UPDATE IoT_Dispositivi SET  ")
            sql.AppendLine("		Nome = @DISPOSITIVO, ")
            sql.AppendLine("		Lat_Dec = @LAT, ")
            sql.AppendLine("		Lng_Dec = @LNG ")
            sql.AppendLine("		WHERE Id = @ID_DISPOSITIVO ")
            sql.AppendLine()
            sql.AppendLine("		DELETE FROM IoT_DispositiviXSensori  ")
            sql.AppendLine("		WHERE id_dispositivo = @ID_DISPOSITIVO AND Id_Sensore NOT IN (SELECT ID_SENSORE FROM @SXS) ")
            sql.AppendLine()
            sql.AppendLine("		MERGE INTO IoT_DispositiviXSensori AS t ")
            sql.AppendLine("		USING  ")
            sql.AppendLine("			(SELECT * FROM @SXS) AS s ")
            sql.AppendLine("			ON t.id_dispositivo = s.id_dispositivo AND t.Id_Sensore = s.Id_Sensore ")
            sql.AppendLine("		WHEN MATCHED THEN ")
            sql.AppendLine("			UPDATE SET Ordine = s.Ordine, Etichetta = s.Etichetta, OutputConfig = s.OutputConfig ")
            sql.AppendLine("		WHEN NOT MATCHED THEN ")
            sql.AppendLine("			INSERT (id_dispositivo, Id_Sensore, FlagReale, OutputConfig, Ordine, Etichetta) ")
            sql.AppendLine("			VALUES (s.id_dispositivo, s.Id_Sensore, 0, s.OutputConfig, s.Ordine, s.Etichetta); ")
            sql.AppendLine()
            sql.AppendLine("		SELECT ID_DISPOSITIVO_AGGIORNATO = @ID_DISPOSITIVO ")
            sql.AppendLine("	END ")
            sql.AppendLine("	ELSE ")
            sql.AppendLine("	BEGIN ")
            sql.AppendLine()
            sql.AppendLine("		SELECT ID_DISPOSITIVO_AGGIORNATO = 0 ")
            sql.AppendLine("	END ")
            sql.AppendLine("END ")

            Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

                Id_Stazione = DT.Rows(0)("ID_DISPOSITIVO_AGGIORNATO")
            End If

        Catch ex As Exception

            Id_Stazione = 0
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return Id_Stazione
    End Function


    Public Function LeggiVisibilitaDispositivo(Id_dispositivo As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IOT.LeggiVisibilitaDispositivo"
        Dim DT As DataTable = Nothing

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SELECT PIVA_Superuser, PIVA, Proprietario, FlagReale ")
            sql.AppendLine("FROM IoT_Dispositivi disp ")
            sql.AppendLine("INNER JOIN IoT_VisibilitaDispositivi vis ON vis.Id_Dispositivo = disp.Id ")
            sql.AppendLine("WHERE isp.Id = " & Id_dispositivo)

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            DT = Nothing
            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return DT
    End Function

    Public Function EliminaDispositivo(id_dispositivo As Integer, ObjParametri_Server As AgronicaCoreParametri) As Boolean

        Dim NomeRoutine As String = "IOT.EliminaDispositivo"
        Dim xRisp As Boolean = False

        Try
            Dim sql As New StringBuilder

            sql.Clear()

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @ID_DISPOSITIVO AS INTEGER = " & id_dispositivo)
            sql.AppendLine("DECLARE @FLAG_OK AS BIT = 1 ")
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY ")
            sql.AppendLine()
            sql.AppendLine("    BEGIN TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("	DELETE FROM IoT_VisibilitaDispositivi WHERE id_dispositivo = @ID_DISPOSITIVO ")
            sql.AppendLine("	DELETE FROM IoT_DispositiviXSensori WHERE id_dispositivo = @ID_DISPOSITIVO ")
            sql.AppendLine("	DELETE FROM IoT_Dispositivi WHERE Id = @ID_DISPOSITIVO ")
            sql.AppendLine()
            sql.AppendLine("    COMMIT TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("END TRY ")
            sql.AppendLine("BEGIN CATCH ")
            sql.AppendLine()
            sql.AppendLine("    ROLLBACK TRANSACTION ")
            sql.AppendLine()
            sql.AppendLine("    SET @FLAG_OK = 0 ")
            sql.AppendLine()
            sql.AppendLine("END CATCH ")
            sql.AppendLine()
            sql.AppendLine("SELECT Flag_OK = @FLAG_OK ")

            Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            If DT IsNot Nothing AndAlso DT.Rows.Count >= 1 Then

                xRisp = DT.Rows(0)("Flag_OK")
            End If

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
        End Try

        Return xRisp
    End Function


    'Public Function LeggiQuadrantiConDistanza(ByVal utm_x As Integer, ByVal utm_y As Integer, ByVal limiteDistanza As Integer, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

    '    Dim NomeRoutine As String = "MeteoNT.LeggiQuadrantiConDistanza"
    '    Dim MessaggioErrore As String = ""
    '    Dim sql As New StringBuilder
    '    Dim DT As DataTable = Nothing

    '    Try

    '        sql.Clear()

    '        sql.AppendLine("DECLARE @X FLOAT = " & utm_x)
    '        sql.AppendLine("DECLARE @Y FLOAT = " & utm_y)
    '        sql.AppendLine()
    '        sql.AppendLine("SELECT TOP 10 ID_Quadrante, Quadrante_Des, X_UTM, Y_UTM, Altitudine, Distanza ")
    '        sql.AppendLine("FROM ( ")
    '        sql.AppendLine("	SELECT ")
    '        sql.AppendLine("        ID_Quadrante")
    '        sql.AppendLine("        , Quadrante_Des ")
    '        sql.AppendLine("        , X_UTM ")
    '        sql.AppendLine("        , Y_UTM ")
    '        sql.AppendLine("        , Altitudine ")
    '        sql.AppendLine("        , SQRT(POWER((X_UTM - @X), 2) + POWER( (Y_UTM - @Y ), 2) ) AS Distanza ")
    '        sql.AppendLine("	FROM TB_Quadranti ")
    '        sql.AppendLine(") T ")
    '        If limiteDistanza > 0 Then
    '            sql.AppendLine("WHERE Distanza <= " & limiteDistanza)
    '        End If
    '        sql.AppendLine("ORDER BY Distanza ")

    '        DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '    Catch ex As Exception

    '        DT = Nothing
    '        MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
    '        Throw New Exception(MessaggioErrore)

    '    End Try

    '    Return DT
    'End Function

    Public Function LeggiInfoDispositivo(TipoSorgente As enum_IoT_Tiposorgente, id_dispositivo As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IoT.LeggiInfoDispositivo"
        Dim DT As DataTable

        Try

            Dim sql As New StringBuilder

            sql.Clear()

            Select Case TipoSorgente

                Case enum_IoT_Tiposorgente.Aziendali, enum_IoT_Tiposorgente.Pubbliche

                    Dim srcTableDati As String = "IoT_DatiOrari"

                    sql.AppendLine("SELECT Nome AS Dispositivo_Des, UltimoAggiornamento FROM IoT_Dispositivi s ")
                    sql.AppendLine("INNER JOIN ( ")
                    sql.AppendLine("	SELECT st.Id, MAX(DataOra) AS UltimoAggiornamento ")
                    sql.AppendLine("	FROM IoT_Dispositivi st ")
                    sql.AppendLine("	INNER JOIN IoT_DispositiviXSensori sxs ON sxs.id_dispositivo = st.Id ")
                    sql.AppendLine("	INNER JOIN " & srcTableDati & " dati ON dati.Id_Sensore = sxs.Id_Sensore ")
                    sql.AppendLine("	WHERE st.Id = " & id_dispositivo.ToString())
                    sql.AppendLine("	GROUP BY st.Id ")
                    sql.AppendLine(") d ON d.Id = s.Id ")

            End Select

            DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return DT
    End Function

    Public Function LeggiElencoDispositivi(piva_superuser As String, piva As String, TipoSorgente As enum_IoT_Tiposorgente, CodiciDispositivi() As Integer, FlagSensori As Boolean, Separatore As String, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IOT.LeggiElencoDispositivi"
        Dim MessaggioErrore As String = ""
        Dim sql As New StringBuilder
        Dim DT As DataTable = Nothing

        Try

            sql.Clear()

            sql.AppendLine("SELECT ")
            sql.AppendLine("    TipoSorgente = " & TipoSorgente & " ")
            sql.AppendLine("    , DISP.* ")
            sql.AppendLine("FROM ( ")

            Select Case TipoSorgente

                Case enum_IoT_Tiposorgente.Aziendali

                    sql.AppendLine("	SELECT ")
                    sql.AppendLine("		Id_Dispositivo = S.Id ")
                    sql.AppendLine("		, Nome_Dispositivo = CASE WHEN vis.Anonima = 0 THEN S.Nome ELSE F.Descrizione + ' (' + S.RifFornitore + ')' END ")
                    sql.AppendLine("		, Fornitore = F.Descrizione ")
                    sql.AppendLine("		, RifFornitore ")
                    sql.AppendLine("		, FlagReale ")
                    sql.AppendLine("	FROM IoT_Dispositivi S ")
                    sql.AppendLine("	INNER JOIN IoT_Fornitori F ON F.Id = S.Id_Fornitore ")
                    sql.AppendLine("	INNER JOIN ( ")
                    sql.AppendLine("		SELECT Id_Dispositivo, DescrizioneAggiuntiva, Anonima FROM ")
                    sql.AppendLine("		( ")
                    sql.AppendLine("			SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima, ROW_NUMBER() OVER(PARTITION BY Id_Dispositivo ORDER BY Id_Dispositivo ASC, Proprietario DESC) AS Cnt ")
                    sql.AppendLine("	        FROM ( ")
                    sql.AppendLine("				SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql.AppendLine("				FROM IoT_VisibilitaDispositivi ")
                    sql.AppendLine("				WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(piva_superuser) & "' AND PIVA = '" & Agro_SQL_SaveText(piva) & "' ")
                    sql.AppendLine()
                    sql.AppendLine("				UNION ")
                    sql.AppendLine()
                    sql.AppendLine("				SELECT Id_Dispositivo, DescrizioneAggiuntiva, Proprietario, Anonima ")
                    sql.AppendLine("				FROM IoT_VisibilitaDispositivi ")
                    sql.AppendLine("				WHERE PIVA_Superuser = '" & Agro_SQL_SaveText(piva_superuser) & "' AND PIVA = '*' ")
                    sql.AppendLine("		    ) TBL1 ")

                    If TipoSorgente = enum_IoT_Tiposorgente.Aziendali Then
                        sql.AppendLine("            WHERE Proprietario = 1 --se solo dispositivi aziendali ")
                    End If

                    sql.AppendLine("	    ) TBL2 ")
                    sql.AppendLine("	    WHERE Cnt = 1 ")
                    sql.AppendLine("	) vis on vis.Id_Dispositivo = S.Id ")

                Case enum_IoT_Tiposorgente.Pubbliche

                    sql.AppendLine("	SELECT")
                    sql.AppendLine("		Id_Dispositivo = S.Id")
                    sql.AppendLine("		, Nome_Dispositivo = F.Descrizione + ' (' + S.Nome + ')'")
                    sql.AppendLine("		, Fornitore = F.Descrizione")
                    sql.AppendLine("		, RifFornitore")
                    sql.AppendLine("		, FlagReale")
                    sql.AppendLine("	FROM IoT_Dispositivi S")
                    sql.AppendLine("	INNER JOIN IoT_Fornitori F ON F.Id = S.Id_Fornitore")
                    sql.AppendLine("	WHERE Categoria = 1")

            End Select

            sql.AppendLine(") DISP ")
            sql.AppendLine("WHERE DISP.Id_Dispositivo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", CodiciDispositivi)) & ") ")

            If Not FlagSensori Then

                DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

            Else

                Dim _sql As New StringBuilder
                _sql.Clear()

                _sql.AppendLine("SELECT ")
                _sql.AppendLine("   DISPOSITIVI.* ")

                If TipoSorgente = enum_IoT_Tiposorgente.Aziendali OrElse
                    TipoSorgente = enum_IoT_Tiposorgente.Pubbliche Then

                    _sql.AppendLine("   , SENSORI.Sensori ")

                Else

                    _sql.AppendLine("   , Sensori = '' ")

                End If

                _sql.AppendLine("FROM ( ")
                _sql.AppendLine()
                _sql.AppendLine()
                _sql.AppendLine()
                _sql.AppendLine(sql.ToString())
                _sql.AppendLine()
                _sql.AppendLine()
                _sql.AppendLine()
                _sql.AppendLine(") DISPOSITIVI ")

                If TipoSorgente = enum_IoT_Tiposorgente.Aziendali OrElse
                    TipoSorgente = enum_IoT_Tiposorgente.Pubbliche Then

                    _sql.AppendLine("INNER JOIN ( ")
                    _sql.AppendLine()
                    _sql.AppendLine()
                    _sql.AppendLine()
                    _sql.AppendLine("SELECT Id_dispositivo, Sensori = STRING_AGG(Sensore, '" & Agro_SQL_SaveText(Separatore) & "') ")
                    _sql.AppendLine("FROM ( ")
                    _sql.AppendLine("	SELECT Id_dispositivo, Sensore = COALESCE(sxs.Etichetta, sens.Etichetta, '') ")
                    _sql.AppendLine("	FROM IoT_DispositiviXSensori sxs ")
                    _sql.AppendLine("	INNER JOIN IoT_Sensori sens ON sens.Id = sxs.Id_Sensore ")
                    _sql.AppendLine("	WHERE sxs.Id_dispositivo IN (" & Agro_SQL_Save_Clausola_IN(String.Join(", ", CodiciDispositivi)) & ") ")
                    _sql.AppendLine("	ORDER BY Ordine OFFSET 0 ROWS ")
                    _sql.AppendLine(") T ")
                    _sql.AppendLine("GROUP BY Id_dispositivo ")
                    _sql.AppendLine()
                    _sql.AppendLine()
                    _sql.AppendLine()
                    _sql.AppendLine(") SENSORI ON SENSORI.Id_dispositivo = DISPOSITIVI.Id_dispositivo ")

                End If

                DT = EseguiQuery_Lettura(ObjParametri_Server, _sql.ToString, NomeRoutine)

            End If

        Catch ex As Exception

            DT = Nothing
            MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return DT
    End Function

    'Public Function GetDataUltimaStoricizzazioneMeteoNT(ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
    '    Dim NomeRoutine As String = "MeteoNT.GetDataUltimaStoricizzazioneMeteoNT"
    '    Dim Riepilogo As DataTable

    '    Try
    '        Dim sql As New StringBuilder

    '        sql.Clear()
    '        sql.AppendLine("SELECT TOP(1) DataUltimaStoricizzazione FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[LogEventi]")

    '        Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '    Catch ex As Exception

    '        Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
    '        Throw New Exception(MessaggioErrore)

    '    End Try

    '    Return Riepilogo
    'End Function

    'Public Function StoricizzazioneMeteoNT(ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
    '    Dim NomeRoutine As String = "MeteoNT.StoricizzazioneMeteoNT"
    '    Dim Riepilogo As DataTable

    '    Try
    '        Dim sql As New StringBuilder

    '        sql.Clear()
    '        sql.AppendLine("SET NOCOUNT ON ")
    '        sql.AppendLine()
    '        sql.AppendLine("-- TipiSensore")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @NewTipiSensore TABLE(Id int, Tipo VARCHAR(50), UM VARCHAR(10), FunAggreg VARCHAR(50), Misura VARCHAR(MAX), DefaultOutputConfig VARCHAR(MAX), IsUpdated bit)")
    '        sql.AppendLine("DECLARE @UpdatedTipiSensore TABLE(Id int)")
    '        sql.AppendLine("    INSERT INTO @NewTipiSensore")
    '        sql.AppendLine("SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_TipiSensore]")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_TIPISENSORE INT = @@ROWCOUNT; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_TIPISENSORE INT = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("-- Fornitori")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @NewFornitori TABLE(Id int, Descrizione VARCHAR(MAX), IsUpdated bit)")
    '        sql.AppendLine("DECLARE @UpdatedFornitori TABLE(Id int)")
    '        sql.AppendLine("INSERT INTO @NewFornitori")
    '        sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_Fornitori]")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_FORNITORI INT = @@ROWCOUNT; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_FORNITORI INT = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("-- Sensori")
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewSensori"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpSensoriUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpNewSensori(Id int, Id_TipoSensore int, Etichetta VARCHAR(MAX), ChiaveImport VARCHAR(100), IsUpdated bit)")
    '        sql.AppendLine("CREATE TABLE #TmpSensoriUpdated(Id int)")
    '        sql.AppendLine("INSERT INTO #TmpNewSensori")
    '        sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_Sensori]")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_SENSORI INT = @@ROWCOUNT; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_SENSORI INT = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("-- Stazioni")
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewStazioni"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpStazioniUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpNewStazioni(Id int, Id_Fornitore int, Nome VARCHAR(MAX), Lat_Dec FLOAT, Lng_Dec FLOAT, Altitudine FLOAT, FlagReale BIT, FlagSIM BIT, RifFornitore VARCHAR(MAX), SystemTimeZoneInfo VARCHAR(MAX), IsUpdated bit)")
    '        sql.AppendLine("CREATE TABLE #TmpStazioniUpdated(Id int)")
    '        sql.AppendLine("INSERT INTO #TmpNewStazioni")
    '        sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_Stazioni]")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_STAZIONI INT = @@ROWCOUNT; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_STAZIONI INT = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("-- StazioniXSensori")
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewStazioniXSensori"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpStazioniXSensoriUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpNewStazioniXSensori(Id_Stazione int, Id_Sensore int, FlagReale BIT, OutputConfig VARCHAR(MAX), Ordine INT, Etichetta VARCHAR(MAX), IsUpdated bit)")
    '        sql.AppendLine("CREATE TABLE #TmpStazioniXSensoriUpdated(Id_Stazione int, Id_Sensore int)")
    '        sql.AppendLine("INSERT INTO #TmpNewStazioniXSensori")
    '        sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_StazioniXSensori]")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_STAZIONIXSENSORI INT = @@ROWCOUNT; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_STAZIONIXSENSORI INT = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("-- DatiOrari")
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewDatiOrari"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpDatiOrariUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @LastYearFirstDay DATETIME = CAST((DATEADD(yy, -1, DATEADD(d, -1 * DATEPART(dd, getdate()) + 1, DATEADD(MM, -1 * DATEPART(MM, getdate()) + 1, GETDATE())))) as date)")
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpNewDatiOrari(Id_Sensore int, DataOra DATETIME, Valore Real, Confidenza SMALLINT, IsUpdated bit, UNIQUE CLUSTERED (Id_Sensore, DataOra))")
    '        sql.AppendLine("CREATE TABLE #TmpDatiOrariUpdated(Id_Sensore int, DataOra DATETIME, UNIQUE CLUSTERED (Id_Sensore, DataOra))")
    '        sql.AppendLine("INSERT INTO #TmpNewDatiOrari")
    '        sql.AppendLine("    SELECT *, 0 FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_DatiOrari] ")
    '        sql.AppendLine("    WHERE DataOra < @LastYearFirstDay AND Confidenza > 0 ")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_DATIORARI BIGINT = ROWCOUNT_BIG(); ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_DATIORARI BIGINT = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("BEGIN TRY ")
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    UPDATE FORNITORI")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE t")
    '        sql.AppendLine("    SET t.Descrizione = s.Descrizione")
    '        sql.AppendLine("    OUTPUT INSERTED.Id INTO @UpdatedFornitori(Id)")
    '        sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Fornitori] t")
    '        sql.AppendLine("    INNER JOIN @NewFornitori s ON s.Id = t.Id")
    '        sql.AppendLine()
    '        sql.AppendLine("    DECLARE @RESULT_UPDATE_FORNITORI INT = @@ROWCOUNT")
    '        sql.AppendLine()
    '        sql.AppendLine("    IF (@RESULT_UPDATE_FORNITORI < @RESULT_TOTAL_FORNITORI) -- if all rows were updates then skip, else insert remaining")
    '        sql.AppendLine("    BEGIN")
    '        sql.AppendLine("        UPDATE s")
    '        sql.AppendLine("        SET s.IsUpdated = 1")
    '        sql.AppendLine("        FROM @NewFornitori s")
    '        sql.AppendLine("        INNER JOIN @UpdatedFornitori u ON u.Id = s.Id;")
    '        sql.AppendLine()
    '        sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Fornitori](Id, Descrizione)")
    '        sql.AppendLine("        SELECT Id, Descrizione FROM @NewFornitori")
    '        sql.AppendLine("        WHERE IsUpdated = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_FORNITORI = @@ROWCOUNT")
    '        sql.AppendLine("    END;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    UPDATE TIPI SENSORE")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE t")
    '        sql.AppendLine("    SET t.Tipo = s.Tipo, t.UM = s.UM, t.FunAggreg = s.FunAggreg, t.Misura = s.Misura, t.DefaultOutputConfig = s.DefaultOutputConfig")
    '        sql.AppendLine("    OUTPUT INSERTED.Id INTO @UpdatedTipiSensore(s.Id)")
    '        sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_TipiSensore] t")
    '        sql.AppendLine("    INNER JOIN @NewTipiSensore s ON s.Id = t.Id")
    '        sql.AppendLine()
    '        sql.AppendLine("    DECLARE @RESULT_UPDATE_TIPISENSORE INT = @@ROWCOUNT")
    '        sql.AppendLine()
    '        sql.AppendLine("    IF (@RESULT_UPDATE_TIPISENSORE < @RESULT_TOTAL_TIPISENSORE) -- if all rows were updates then skip, else insert remaining")
    '        sql.AppendLine("    BEGIN")
    '        sql.AppendLine("        UPDATE s")
    '        sql.AppendLine("        SET s.IsUpdated = 1")
    '        sql.AppendLine("        FROM @NewTipiSensore s")
    '        sql.AppendLine("        INNER JOIN @UpdatedTipiSensore u ON u.Id = s.Id;")
    '        sql.AppendLine()
    '        sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_TipiSensore]")
    '        sql.AppendLine("        SELECT Id, Tipo, UM, FunAggreg, Misura, DefaultOutputConfig FROM @NewTipiSensore")
    '        sql.AppendLine("        WHERE IsUpdated = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_TIPISENSORE = @@ROWCOUNT")
    '        sql.AppendLine("    END;")
    '        sql.AppendLine("    ")
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    UPDATE SENSORI")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE t")
    '        sql.AppendLine("    SET t.Id_TipoSensore = s.Id_TipoSensore, t.Etichetta = s.Etichetta, t.ChiaveImport = s.ChiaveImport")
    '        sql.AppendLine("    OUTPUT INSERTED.Id INTO #TmpSensoriUpdated(s.Id)")
    '        sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Sensori] t")
    '        sql.AppendLine("    INNER JOIN #TmpNewSensori s ON s.Id = t.Id")
    '        sql.AppendLine()
    '        sql.AppendLine("    DECLARE @RESULT_UPDATE_SENSORI INT = @@ROWCOUNT")
    '        sql.AppendLine()
    '        sql.AppendLine("    IF (@RESULT_UPDATE_SENSORI < @RESULT_TOTAL_SENSORI) -- if all rows were updates then skip, else insert remaining")
    '        sql.AppendLine("    BEGIN")
    '        sql.AppendLine("        UPDATE s")
    '        sql.AppendLine("        SET s.IsUpdated = 1")
    '        sql.AppendLine("        FROM #TmpNewSensori s")
    '        sql.AppendLine("        INNER JOIN #TmpSensoriUpdated u ON u.Id = s.Id;")
    '        sql.AppendLine()
    '        sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Sensori]")
    '        sql.AppendLine("        SELECT Id, Id_TipoSensore, Etichetta, ChiaveImport FROM #TmpNewSensori")
    '        sql.AppendLine("        WHERE IsUpdated = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_SENSORI = @@ROWCOUNT")
    '        sql.AppendLine("    END;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    UPDATE STAZIONI")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE t")
    '        sql.AppendLine("    SET t.Id_Fornitore = s.Id_Fornitore, t.Nome = s.Nome, t.Lat_Dec = s.Lat_Dec, t.Lng_Dec = s.Lng_Dec, t.Altitudine = s.Altitudine, t.FlagReale = s.FlagReale, t.FlagSIM = s.FlagSIM, t.RifFornitore = s.RifFornitore, t.SystemTimeZoneInfo = s.SystemTimeZoneInfo")
    '        sql.AppendLine("    OUTPUT INSERTED.Id INTO #TmpStazioniUpdated(s.Id)")
    '        sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Stazioni] t")
    '        sql.AppendLine("    INNER JOIN #TmpNewStazioni s ON s.Id = t.Id")
    '        sql.AppendLine()
    '        sql.AppendLine("    DECLARE @RESULT_UPDATE_STAZIONI INT = @@ROWCOUNT")
    '        sql.AppendLine()
    '        sql.AppendLine("    IF (@RESULT_UPDATE_STAZIONI < @RESULT_TOTAL_STAZIONI) -- if all rows were updates then skip, else insert remaining")
    '        sql.AppendLine("    BEGIN")
    '        sql.AppendLine("        UPDATE s")
    '        sql.AppendLine("        SET s.IsUpdated = 1")
    '        sql.AppendLine("        FROM #TmpNewStazioni s")
    '        sql.AppendLine("        INNER JOIN #TmpStazioniUpdated u ON u.Id = s.Id;")
    '        sql.AppendLine()
    '        sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_Stazioni]")
    '        sql.AppendLine("        SELECT Id, Id_Fornitore, Nome, Lat_Dec, Lng_Dec, Altitudine, FlagReale, FlagSIM, RifFornitore, SystemTimeZoneInfo FROM #TmpNewStazioni")
    '        sql.AppendLine("        WHERE IsUpdated = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_STAZIONI = @@ROWCOUNT")
    '        sql.AppendLine("    END;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    UPDATE STAZIONIXSENSORI")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE t")
    '        sql.AppendLine("    SET t.FlagReale = s.FlagReale, t.OutputConfig = s.OutputConfig, t.Ordine = s.Ordine, t.Etichetta = s.Etichetta")
    '        sql.AppendLine("    OUTPUT INSERTED.Id_Stazione, INSERTED.Id_Sensore INTO #TmpStazioniXSensoriUpdated(Id_Stazione, Id_Sensore)")
    '        sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_StazioniXSensori] t")
    '        sql.AppendLine("    INNER JOIN #TmpNewStazioniXSensori s ON s.Id_Stazione = t.Id_Stazione AND s.Id_Sensore = t.Id_Sensore")
    '        sql.AppendLine()
    '        sql.AppendLine("    DECLARE @RESULT_UPDATE_STAZIONIXSENSORI INT = @@ROWCOUNT")
    '        sql.AppendLine()
    '        sql.AppendLine("    IF (@RESULT_UPDATE_STAZIONIXSENSORI < @RESULT_TOTAL_STAZIONIXSENSORI) -- if all rows were updates then skip, else insert remaining")
    '        sql.AppendLine("    BEGIN")
    '        sql.AppendLine("        UPDATE s")
    '        sql.AppendLine("        SET s.IsUpdated = 1")
    '        sql.AppendLine("        FROM #TmpNewStazioniXSensori s")
    '        sql.AppendLine("        INNER JOIN #TmpStazioniXSensoriUpdated u ON u.Id_Stazione = s.Id_Stazione AND u.Id_Sensore = s.Id_Sensore;")
    '        sql.AppendLine()
    '        sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_StazioniXSensori]")
    '        sql.AppendLine("        SELECT Id_Stazione, Id_Sensore, FlagReale, OutputConfig, Ordine, Etichetta FROM #TmpNewStazioniXSensori")
    '        sql.AppendLine("        WHERE IsUpdated = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_STAZIONIXSENSORI = @@ROWCOUNT")
    '        sql.AppendLine("    END;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    UPDATE DATIORARI")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE t")
    '        sql.AppendLine("    SET t.Valore = s.Valore, t.Confidenza = s.Confidenza")
    '        sql.AppendLine("    OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpDatiOrariUpdated(Id_Sensore, DataOra)")
    '        sql.AppendLine("    FROM [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_DatiOrari] t")
    '        sql.AppendLine("    INNER JOIN #TmpNewDatiOrari s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra")
    '        sql.AppendLine()
    '        sql.AppendLine("    DECLARE @RESULT_UPDATE_DATIORARI INT = ROWCOUNT_BIG()")
    '        sql.AppendLine()
    '        sql.AppendLine("    IF (@RESULT_UPDATE_DATIORARI < @RESULT_TOTAL_DATIORARI) -- if all rows were updates then skip, else insert remaining")
    '        sql.AppendLine("    BEGIN")
    '        sql.AppendLine()
    '        sql.AppendLine("        UPDATE s")
    '        sql.AppendLine("        SET s.IsUpdated = 1")
    '        sql.AppendLine("        FROM #TmpNewDatiOrari s")
    '        sql.AppendLine("        INNER JOIN #TmpDatiOrariUpdated u ON (u.Id_Sensore = s.Id_Sensore AND u.DataOra = s.DataOra);")
    '        sql.AppendLine()
    '        sql.AppendLine("        INSERT INTO [ZZ_AgronicaMeteoSuite_Storico].[dbo].[MeteoNT_DatiOrari]")
    '        sql.AppendLine("        SELECT Id_Sensore, DataOra, Valore, Confidenza FROM #TmpNewDatiOrari")
    '        sql.AppendLine("        WHERE IsUpdated = 0;")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_DATIORARI = ROWCOUNT_BIG()")
    '        sql.AppendLine("    END;")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    Delete DatiOrari")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    DELETE FROM [AgronicaMeteoSuite].[dbo].[MeteoNT_DatiOrari]")
    '        sql.AppendLine("        WHERE DataOra < @LastYearFirstDay")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/********************************************************************************************************************************************")
    '        sql.AppendLine("    Update data ultima storicizzazione")
    '        sql.AppendLine("*********************************************************************************************************************************************/")
    '        sql.AppendLine()
    '        sql.AppendLine("    UPDATE [ZZ_AgronicaMeteoSuite_Storico].[dbo].[LogEventi] SET DataUltimaStoricizzazione = GETDATE()")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("END TRY")
    '        sql.AppendLine("BEGIN CATCH")
    '        sql.AppendLine("    IF @@TRANCOUNT > 0")
    '        sql.AppendLine("        BEGIN;")
    '        sql.AppendLine("            ROLLBACK")
    '        sql.AppendLine("        END;")
    '        sql.AppendLine("END CATCH;")
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewSensori"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpSensoriUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewStazioni"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpStazioniUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewStazioniXSensori"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpStazioniXSensoriUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpNewDatiOrari"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpDatiOrariUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine("--Ritorno un riepilogo ")
    '        sql.AppendLine()
    '        sql.AppendLine("SELECT T.* FROM (VALUES ")
    '        sql.AppendLine("('MeteoNT_Fornitori', @RESULT_TOTAL_FORNITORI, @RESULT_UPDATE_FORNITORI, @RESULT_INSERT_FORNITORI),")
    '        sql.AppendLine("('MeteoNT_TipiSensore', @RESULT_TOTAL_TIPISENSORE, @RESULT_UPDATE_TIPISENSORE, @RESULT_INSERT_TIPISENSORE), ")
    '        sql.AppendLine("('MeteoNT_Sensori', @RESULT_TOTAL_SENSORI, @RESULT_UPDATE_SENSORI, @RESULT_INSERT_SENSORI),")
    '        sql.AppendLine("('MeteoNT_Stazioni', @RESULT_TOTAL_STAZIONI, @RESULT_UPDATE_STAZIONI, @RESULT_INSERT_STAZIONI),")
    '        sql.AppendLine("('MeteoNT_StazioniXSensori', @RESULT_TOTAL_STAZIONIXSENSORI, @RESULT_UPDATE_STAZIONIXSENSORI, @RESULT_INSERT_STAZIONIXSENSORI),")
    '        sql.AppendLine("('MeteoNT_DatiOrari', @RESULT_TOTAL_DATIORARI, @RESULT_UPDATE_DATIORARI, @RESULT_INSERT_DATIORARI)")
    '        sql.AppendLine(") T(Tabella, [TotalCnt], [Update], [Insert]);")

    '        Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '    Catch ex As Exception

    '        Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
    '        Throw New Exception(MessaggioErrore)

    '    End Try

    '    Return Riepilogo
    'End Function

    Public Class TblSensoriSorgente
        Public ReadOnly NomeTabella As String
        Public ReadOnly CampoId As String
        Public ReadOnly ChiaveImport As String
        Public ReadOnly CampoIdDispositivo As String
        Public Sub New(_tabella As String, _id As String, _chiave As String, _IdDispositivo As String)
            NomeTabella = _tabella
            CampoId = _id
            ChiaveImport = _chiave
            CampoIdDispositivo = _IdDispositivo
        End Sub
    End Class

    Public Class TblDatiSorgente
        Public ReadOnly NomeTabella As String
        Public ReadOnly CampoIdSensore As String
        Public ReadOnly CampoDataOra As String
        Public ReadOnly CampoValore As String
        Public ReadOnly CampoElaborato As String
        Public ReadOnly ValoreUTC As String
        'Private _filterCondition As String
        Public Sub New(_tabella As String, _idSensore As String, _dataOra As String, _valore As String, _elaborato As String, _utc As Boolean)
            NomeTabella = _tabella
            CampoIdSensore = _idSensore
            CampoDataOra = _dataOra
            CampoValore = _valore
            CampoElaborato = _elaborato
            ValoreUTC = If(_utc, "1", "0")
            '_filterCondition = ""
        End Sub
        'Public Property FilterCondition As String
        '    Get
        '        Return _filterCondition
        '    End Get
        '    Set(value As String)
        '        _filterCondition = value
        '    End Set
        'End Property
    End Class

    Private Function DropTmpTable(ByVal tblname As String) As String
        Dim sql As New StringBuilder
        sql.AppendLine("BEGIN TRY ")
        sql.AppendLine("    DROP TABLE " & tblname)
        sql.AppendLine("END TRY ")
        sql.AppendLine("BEGIN CATCH ")
        sql.AppendLine("END CATCH ")
        Return sql.ToString
    End Function

    Public Function EliminaDatiSorgenteByDispositivoId(ByVal tblSensori As TblSensoriSorgente, ByVal tblDati As TblDatiSorgente, ByVal dispositivoid As String, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable
        Dim NomeRoutine As String = "IOT.EliminaDatiSorgente"
        Dim RetValue As Boolean = True

        Dim Riepilogo As DataTable

        Console.WriteLine(tblDati)

        Try
            Dim sql As New StringBuilder

            sql.AppendLine("DECLARE @dataEliminazione DATETIME = (")
            sql.AppendLine("    SELECT TOP(1) d." & tblDati.CampoDataOra & " FROM " & tblDati.NomeTabella & " d JOIN " & tblSensori.NomeTabella & " s ON d.Id_Sensore = s.Id")
            sql.AppendLine("    WHERE s." & tblSensori.CampoIdDispositivo & " = " & dispositivoid)
            sql.AppendLine("    AND Elaborato = 0 AND DATEDIFF(month, d." & tblDati.CampoDataOra & ", GETDATE()) > 2")
            sql.AppendLine("    ORDER BY d." & tblDati.CampoDataOra & " asc);")
            sql.AppendLine()
            sql.AppendLine("DELETE d FROM " & tblDati.NomeTabella & " d JOIN " & tblSensori.NomeTabella & " s ON d.Id_Sensore = s.Id")
            sql.AppendLine("WHERE s." & tblSensori.CampoIdDispositivo & " = " & dispositivoid)
            sql.AppendLine("AND Elaborato = 1 AND s.Trasferibile = 1 AND d." & tblDati.CampoDataOra & " < COALESCE(DATEADD(HOUR, DATEDIFF(HOUR, 0, (@dataEliminazione)), 0), (DATEADD(mm, -2, DATEADD(HOUR, DATEDIFF(HOUR, 0, (GETDATE())), 0))));")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)

        End Try

        Return Riepilogo
    End Function

    Public Function ScriviDatiOrari(tblSensori As TblSensoriSorgente, tblDati As TblDatiSorgente, confidenza As Integer, ObjParametri_Server As AgronicaCoreParametri) As DataTable

        Dim NomeRoutine As String = "IoT.ScriviDatiOrari"

        Dim Riepilogo As DataTable

        Try

            Dim sql As New StringBuilder

            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()

            sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpDatiSorgente (Src_Id INT NOT NULL, Id_Dispositivo INT NOT NULL, Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL, Valore REAL NOT NULL, IsUpdate BIT NOT NULL); ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpDatiSorgente ON #TmpDatiSorgente (Id_Dispositivo, Id_Sensore, DataOra); ")
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpUpdated"))
            sql.AppendLine()
            sql.AppendLine("CREATE TABLE #TmpUpdated (Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL); ")
            sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpUpdated ON  #TmpUpdated (Id_Sensore, DataOra) ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("INSERT INTO #TmpDatiSorgente (Src_Id, Id_Dispositivo, Id_Sensore, DataOra, Valore, IsUpdate) ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("    T.Src_Id ")
            sql.AppendLine("    , T.Id_Dispositivo ")
            sql.AppendLine("    , T.Id_Sensore ")
            sql.AppendLine("    , DataOra = CASE WHEN D.UTC = 1 THEN CAST(D.DataOra AT TIME ZONE 'UTC' AT TIME ZONE T.SystemTimeZoneInfo AS DATETIME) ELSE D.DataOra END ")
            sql.AppendLine("    , D.Valore ")
            sql.AppendLine("    , 0 ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		Src_Id = src.Id ")
            sql.AppendLine("        , disp.Id AS Id_Dispositivo ")
            sql.AppendLine("        , sens.Id AS Id_Sensore ")
            sql.AppendLine("        , SystemTimeZoneInfo = COALESCE(disp.SystemTimeZoneInfo, 'W. Europe Standard Time') ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine()
            sql.AppendLine("		SELECT " & tblSensori.CampoId & ", ChiaveImport = " & tblSensori.ChiaveImport)
            sql.AppendLine("		FROM " & tblSensori.NomeTabella)
            sql.AppendLine()
            sql.AppendLine("	) src ")
            sql.AppendLine("    INNER JOIN IoT_Sensori			sens	ON sens.ChiaveImport = src.ChiaveImport ")
            sql.AppendLine("    INNER JOIN IoT_DispositiviXSensori	sxs		ON sxs.Id_Sensore = sens.Id ")
            sql.AppendLine("    INNER JOIN IoT_Dispositivi			disp	ON disp.Id = sxs.Id_Dispositivo ")
            sql.AppendLine("    WHERE sxs.FlagReale = 1 ")
            sql.AppendLine(") T ")
            sql.AppendLine("INNER JOIN ( ")
            sql.AppendLine()
            sql.AppendLine("	SELECT Id_Sensore = " & tblDati.CampoIdSensore & ", DataOra = " & tblDati.CampoDataOra & ", Valore = " & tblDati.CampoValore & ", UTC = " & tblDati.ValoreUTC)
            sql.AppendLine("	FROM " & tblDati.NomeTabella)
            sql.AppendLine("	WHERE " & tblDati.CampoElaborato & " = 0 ")
            'If Not String.IsNullOrEmpty(tblDati.FilterCondition) Then
            '    sql.AppendLine("	AND " & tblDati.FilterCondition)
            'End If
            sql.AppendLine()
            sql.AppendLine(") D On T.Src_Id = D.Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DECLARE @RESULT_TOTAL_ORARI INT = @@ROWCOUNT; ")
            sql.AppendLine("DECLARE @RESULT_UPDATE_ORARI INT = 0; ")
            sql.AppendLine("DECLARE @RESULT_INSERT_ORARI INT = 0; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("WITH cte AS ( ")
            sql.AppendLine("    SELECT ")
            sql.AppendLine("		Src_Id ")
            sql.AppendLine("		, Id_Dispositivo ")
            sql.AppendLine("		, Id_Sensore ")
            sql.AppendLine("		, DataOra ")
            sql.AppendLine("		, Dupl = ROW_NUMBER() OVER (PARTITION BY Id_Dispositivo, Id_Sensore, DataOra ORDER BY Id_Dispositivo, Id_Sensore, DataOra) ")
            sql.AppendLine("     FROM #TmpDatiSorgente ")
            sql.AppendLine(") ")
            sql.AppendLine("DELETE FROM cte ")
            sql.AppendLine("WHERE Dupl > 1; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("SET @RESULT_TOTAL_ORARI = @RESULT_TOTAL_ORARI - @@ROWCOUNT; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("BEGIN TRY ")
            sql.AppendLine()
            sql.AppendLine("	BEGIN TRAN; ")
            sql.AppendLine()
            sql.AppendLine("	/************************************************************************************************** ")
            sql.AppendLine("	Inserisco i dati nella tabella DatiOrari  ")
            sql.AppendLine("	**************************************************************************************************/ ")
            sql.AppendLine()
            sql.AppendLine("	UPDATE t ")
            sql.AppendLine("	SET t.Valore = s.Valore ")
            sql.AppendLine("    OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpUpdated (Id_Sensore, DataOra) ")
            sql.AppendLine("    FROM IoT_DatiOrari t ")
            sql.AppendLine("    INNER JOIN #TmpDatiSorgente s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra ")
            sql.AppendLine()
            sql.AppendLine("	SET @RESULT_UPDATE_ORARI = @@ROWCOUNT ")
            sql.AppendLine()
            sql.AppendLine("	IF (@RESULT_UPDATE_ORARI < @RESULT_TOTAL_ORARI) -- if all rows were updates then skip, else insert remaining ")
            sql.AppendLine("	BEGIN ")
            sql.AppendLine()
            sql.AppendLine("		UPDATE s ")
            sql.AppendLine("		SET s.IsUpdate = 1 ")
            sql.AppendLine("		FROM #TmpDatiSorgente s ")
            sql.AppendLine("		INNER JOIN #TmpUpdated u ON u.Id_Sensore = s.Id_sensore AND u.DataOra = s.DataOra ")
            sql.AppendLine()
            sql.AppendLine("		INSERT INTO IoT_DatiOrari (Id_Sensore, DataOra, Valore, Confidenza) ")
            sql.AppendLine("        SELECT Id_Sensore, DataOra, Valore, " & confidenza & " as Confidenza ")
            sql.AppendLine("        FROM #TmpDatiSorgente ")
            sql.AppendLine("        WHERE IsUpdate = 0 ")
            sql.AppendLine()
            sql.AppendLine("        SET @RESULT_INSERT_ORARI = @@ROWCOUNT ")
            sql.AppendLine()
            sql.AppendLine("	END; ")
            sql.AppendLine()
            sql.AppendLine("	/************************************************************************************************** ")
            sql.AppendLine("	Aggiorno il flag elaborato per ogni sensore coinvolto ")
            sql.AppendLine("	**************************************************************************************************/ ")
            sql.AppendLine()
            sql.AppendLine("	UPDATE d ")
            sql.AppendLine("	SET " & tblDati.CampoElaborato & " = 1 ")
            sql.AppendLine("	FROM " & tblDati.NomeTabella & " d ")
            sql.AppendLine("	INNER JOIN (SELECT DISTINCT Src_Id FROM #TmpDatiSorgente) s On s.Src_Id = d." & tblDati.CampoIdSensore)
            sql.AppendLine("	WHERE " & tblDati.CampoElaborato & " = 0 ")
            'If Not String.IsNullOrEmpty(tblDati.FilterCondition) Then
            '    sql.AppendLine("	AND " & tblDati.FilterCondition)
            'End If
            sql.AppendLine()
            sql.AppendLine("	COMMIT TRAN; ")
            sql.AppendLine()
            sql.AppendLine("END TRY ")
            sql.AppendLine("BEGIN CATCH ")
            sql.AppendLine()
            sql.AppendLine("	IF (@@TRANCOUNT > 0) ")
            sql.AppendLine("	BEGIN ")
            sql.AppendLine("		ROLLBACK; ")
            sql.AppendLine("	END; ")
            sql.AppendLine()
            sql.AppendLine("END CATCH; ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
            sql.AppendLine()
            sql.AppendLine(DropTmpTable("#TmpUpdated"))
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("--Ritorno un riepilogo ")
            sql.AppendLine()
            sql.AppendLine("SELECT T.* FROM (VALUES ")
            sql.AppendLine("('IoT_DatiOrari', @RESULT_TOTAL_ORARI, @RESULT_UPDATE_ORARI, @RESULT_INSERT_ORARI) ")
            sql.AppendLine(") T(Tabella, [TotalCnt], [Update], [Insert]) ")

            Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

        Catch ex As Exception

            Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
            Throw New Exception(MessaggioErrore)
        End Try

        Return Riepilogo
    End Function

    'Public Function ScriviDatiSorgente(ByVal tblSensori As TblSensoriSorgente, ByVal tblDati As TblDatiSorgente, ByVal ObjParametri_Server As AgronicaCoreParametri) As DataTable

    '    Dim NomeRoutine As String = "MeteoNT.ScriviDatiSorgente"
    '    Dim RetValue As Boolean = True

    '    Dim Riepilogo As DataTable = Nothing

    '    Try

    '        Dim sql As New StringBuilder

    '        sql.AppendLine("SET NOCOUNT ON ")
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpDatiSorgente (Src_Id INT NOT NULL, Id_Stazione INT NOT NULL, Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL, Valore REAL NOT NULL, IsUpdate BIT NOT NULL); ")
    '        sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpDatiSorgente ON #TmpDatiSorgente (Id_Stazione, Id_Sensore, DataOra); ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpDatiAggregati"))
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpDatiAggregati (Id_Stazione INT NOT NULL, Id_Sensore INT NOT NULL, DataOraSrc DATETIME NOT NULL, DataOra DATETIME NOT NULL, Valore REAL NOT NULL, TipoSensore INT NOT NULL, FunAggreg VARCHAR(50) NOT NULL) ")
    '        sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpDatiAggregati ON #TmpDatiAggregati (Id_Stazione, Id_Sensore, DataOraSrc); ")
    '        sql.AppendLine("CREATE INDEX IX_TmpDatiAggregati_FunAggreg ON #TmpDatiAggregati (FunAggreg) ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine("CREATE TABLE #TmpUpdated (Id_Sensore INT NOT NULL, DataOra DATETIME NOT NULL); ")
    '        sql.AppendLine("CREATE CLUSTERED INDEX IX_TmpUpdated ON  #TmpUpdated (Id_Sensore, DataOra) ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("INSERT INTO #TmpDatiSorgente (Src_Id, Id_Stazione, Id_Sensore, DataOra, Valore, IsUpdate) ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("    T.Src_Id ")
    '        sql.AppendLine("    , T.Id_Stazione ")
    '        sql.AppendLine("    , T.Id_Sensore ")
    '        sql.AppendLine("    , DataOra = CASE WHEN D.UTC = 1 THEN CAST(D.DataOra AT TIME ZONE 'UTC' AT TIME ZONE T.SystemTimeZoneInfo AS DATETIME) ELSE D.DataOra END ")
    '        sql.AppendLine("    , D.Valore ")
    '        sql.AppendLine("    , 0 ")
    '        sql.AppendLine("FROM ( ")
    '        sql.AppendLine("	SELECT ")
    '        sql.AppendLine("		Src_Id = src.Id ")
    '        sql.AppendLine("        , staz.Id AS Id_Stazione ")
    '        sql.AppendLine("        , sens.Id AS Id_Sensore ")
    '        sql.AppendLine("        , SystemTimeZoneInfo = COALESCE(staz.SystemTimeZoneInfo, 'W. Europe Standard Time') ")
    '        sql.AppendLine("	FROM ( ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT " & tblSensori.CampoId & ", ChiaveImport = " & tblSensori.ChiaveImport)
    '        sql.AppendLine("		FROM " & tblSensori.NomeTabella)
    '        sql.AppendLine()
    '        sql.AppendLine("	) src ")
    '        sql.AppendLine("    INNER JOIN MeteoNT_Sensori			sens	ON sens.ChiaveImport = src.ChiaveImport ")
    '        sql.AppendLine("    INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = sens.Id ")
    '        sql.AppendLine("    INNER JOIN MeteoNT_Stazioni			staz	ON staz.Id = sxs.Id_Stazione ")
    '        sql.AppendLine("    WHERE sxs.FlagReale = 1 ")
    '        sql.AppendLine(") T ")
    '        sql.AppendLine("INNER JOIN ( ")
    '        sql.AppendLine()
    '        sql.AppendLine("	SELECT Id_Sensore = " & tblDati.CampoIdSensore & ", DataOra = " & tblDati.CampoDataOra & ", Valore = " & tblDati.CampoValore & ", UTC = " & tblDati.ValoreUTC)
    '        sql.AppendLine("	FROM " & tblDati.NomeTabella & " WHERE " & tblDati.CampoElaborato & " = 0 ")
    '        sql.AppendLine()
    '        sql.AppendLine(") D On T.Src_Id = D.Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_SORGENTE INT = @@ROWCOUNT; ")
    '        sql.AppendLine("DECLARE @RESULT_UPDATE_SORGENTE INT = 0; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_SORGENTE INT = 0; ")
    '        sql.AppendLine("DECLARE @RESULT_TOTAL_ORARI INT = 0; ")
    '        sql.AppendLine("DECLARE @RESULT_UPDATE_ORARI INT = 0; ")
    '        sql.AppendLine("DECLARE @RESULT_INSERT_ORARI INT = 0; ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("WITH cte AS ( ")
    '        sql.AppendLine("    SELECT ")
    '        sql.AppendLine("		Src_Id ")
    '        sql.AppendLine("		, Id_Stazione ")
    '        sql.AppendLine("		, Id_Sensore ")
    '        sql.AppendLine("		, DataOra ")
    '        sql.AppendLine("		, Dupl = ROW_NUMBER() OVER (PARTITION BY Id_Stazione, Id_Sensore, DataOra ORDER BY Id_Stazione, Id_Sensore, DataOra) ")
    '        sql.AppendLine("     FROM #TmpDatiSorgente ")
    '        sql.AppendLine(") ")
    '        sql.AppendLine("DELETE FROM cte ")
    '        sql.AppendLine("WHERE Dupl > 1; ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("SET @RESULT_TOTAL_SORGENTE = @RESULT_TOTAL_SORGENTE - @@ROWCOUNT; ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("BEGIN TRY ")
    '        sql.AppendLine()
    '        sql.AppendLine("	BEGIN TRAN; ")
    '        sql.AppendLine()
    '        sql.AppendLine("	/************************************************************************************************** ")
    '        sql.AppendLine("	Aggiorno/Inserisco i dati nella tabella DatiSorgente ")
    '        sql.AppendLine("	**************************************************************************************************/ ")
    '        sql.AppendLine()
    '        sql.AppendLine("	UPDATE t ")
    '        sql.AppendLine("	SET t.Valore = s.Valore ")
    '        sql.AppendLine("    OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpUpdated (Id_Sensore, DataOra) ")
    '        sql.AppendLine("    FROM MeteoNT_DatiSorgente t ")
    '        sql.AppendLine("    INNER JOIN #TmpDatiSorgente s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra ")
    '        sql.AppendLine()
    '        sql.AppendLine("	SET @RESULT_UPDATE_SORGENTE = @@ROWCOUNT ")
    '        sql.AppendLine()
    '        sql.AppendLine("	IF (@RESULT_UPDATE_SORGENTE < @RESULT_TOTAL_SORGENTE) -- if all rows were updates then skip, else insert remaining ")
    '        sql.AppendLine("	BEGIN ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UPDATE s ")
    '        sql.AppendLine("		SET s.IsUpdate = 1 ")
    '        sql.AppendLine("		FROM #TmpDatiSorgente s ")
    '        sql.AppendLine("		INNER JOIN #TmpUpdated u ON u.Id_Sensore = s.Id_sensore AND u.DataOra = s.DataOra ")
    '        sql.AppendLine()
    '        sql.AppendLine("		INSERT INTO MeteoNT_DatiSorgente (Id_Sensore, DataOra, Valore) ")
    '        sql.AppendLine("        SELECT Id_Sensore, DataOra, Valore ")
    '        sql.AppendLine("        FROM #TmpDatiSorgente ")
    '        sql.AppendLine("        WHERE IsUpdate = 0 ")
    '        sql.AppendLine()
    '        sql.AppendLine("        SET @RESULT_INSERT_SORGENTE = @@ROWCOUNT ")
    '        sql.AppendLine()
    '        sql.AppendLine("	END; ")
    '        sql.AppendLine()
    '        sql.AppendLine("	/************************************************************************************************** ")
    '        sql.AppendLine("	Aggiorno il flag elaborato per ogni sensore coinvolto ")
    '        sql.AppendLine("	**************************************************************************************************/ ")
    '        sql.AppendLine()
    '        sql.AppendLine("	UPDATE d ")
    '        sql.AppendLine("	SET " & tblDati.CampoElaborato & " = 1 ")
    '        sql.AppendLine("	FROM " & tblDati.NomeTabella & " d ")
    '        sql.AppendLine("	INNER JOIN (SELECT DISTINCT Src_Id FROM #TmpDatiSorgente) s On s.Src_Id = d." & tblDati.CampoIdSensore)
    '        sql.AppendLine("	WHERE " & tblDati.CampoElaborato & " = 0 ")
    '        sql.AppendLine()
    '        sql.AppendLine("	/************************************************************************************************** ")
    '        sql.AppendLine("	Produco i dati aggregati da dati originali Tabella DatiSorgente -> Tabelle DatiOrari ")
    '        sql.AppendLine("	Stessi sensori aggregati secondo la funzione di aggregazione propria del tipo sensore ")
    '        sql.AppendLine("	**************************************************************************************************/ ")
    '        sql.AppendLine()
    '        sql.AppendLine("	INSERT INTO #TmpDatiAggregati (Id_Stazione, DataOraSrc, DataOra, Id_Sensore, Valore, TipoSensore, FunAggreg) ")
    '        sql.AppendLine("	SELECT ")
    '        sql.AppendLine("		sxs.Id_Stazione ")
    '        sql.AppendLine("		, DataOraSrc = DataOra ")
    '        sql.AppendLine("		, DataOra = DATEADD(hh, DATEDIFF(hh, '19000101', DataOra), '19000101') ")
    '        sql.AppendLine("		, sxs.Id_Sensore ")
    '        sql.AppendLine("		, dati.Valore ")
    '        sql.AppendLine("		, Id_TipoSensore ")
    '        sql.AppendLine("		, FunAggreg ")
    '        sql.AppendLine("	FROM ( ")
    '        sql.AppendLine("		SELECT Id_Sensore, Min_DT = DATEADD(hh, DATEDIFF(hh, '19000101', MIN(DataOra)), '19000101'), Max_DT = DATEADD(hh, DATEDIFF(hh, '19000101', MAX(DataOra)) + 1, '19000101') ")
    '        sql.AppendLine("		FROM #TmpDatiSorgente ")
    '        sql.AppendLine("		GROUP BY Id_Sensore ")
    '        sql.AppendLine("	) app ")
    '        sql.AppendLine("	INNER JOIN MeteoNT_Sensori			sens	ON sens.Id = app.Id_Sensore ")
    '        sql.AppendLine("	INNER JOIN MeteoNT_TipiSensore		tipi	ON tipi.Id = sens.Id_TipoSensore ")
    '        sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = sens.Id ")
    '        sql.AppendLine("	INNER JOIN MeteoNT_DatiSorgente		dati	ON dati.Id_Sensore = sens.Id ")
    '        sql.AppendLine("	WHERE sxs.FlagReale = 1 AND app.Min_DT <= dati.DataOra AND dati.DataOra < app.Max_DT ")
    '        sql.AppendLine()
    '        sql.AppendLine("	/************************************************************************************************** ")
    '        sql.AppendLine("	AGGREGAZIONE PER SENSORI VETTORIALI ")
    '        sql.AppendLine()
    '        sql.AppendLine("	La funzione di aggregazione di un tipo vettoriale è definita come avgvett(x) con x il tipo del sensore che identifica ")
    '        sql.AppendLine("	la direzione (angolo) del vettore la cui velocità (magnitudine) è il tipo stesso. ")
    '        sql.AppendLine("	Ad esempio per una coppia di vettori che acquisiscono la direzione e velocità del vento ")
    '        sql.AppendLine("	la direzione (Id Tipo = 6, ad esempio) non ha funzione di aggregazione nella tabella dei TipiSensore, ")
    '        sql.AppendLine("	mentre la velocità (Id Tipo = 7 ad esempio) ha come funzione di aggregazione avgvett(6) ")
    '        sql.AppendLine("	ad indicare che il tipo 7 sarà accoppiato al tipo 6 nei calcoli di aggregazione. ")
    '        sql.AppendLine()
    '        sql.AppendLine("	N.B. Fallisce se in uno stesso insieme ho due sensori dello stesso tipo, ad esempio due anemometri, ")
    '        sql.AppendLine("	che hanno la stessa funzione di aggreazione avgvett(x) ")
    '        sql.AppendLine("	**************************************************************************************************/ ")
    '        sql.AppendLine()
    '        sql.AppendLine("	DECLARE @TBL_AGGR_VET TABLE(DataOra DATETIME, Id_SensoreDir INT, Dir REAL, Id_SensoreVel INT, Vel REAL, Confidenza SMALLINT) ")
    '        sql.AppendLine("	INSERT INTO @TBL_AGGR_VET ")
    '        sql.AppendLine("	SELECT ")
    '        sql.AppendLine("		DataOra ")
    '        sql.AppendLine("		, Id_SensoreDir ")
    '        sql.AppendLine("		, Dir = CASE WHEN ABS(Vel_eo) < 0.000001 AND ABS(Vel_ns) < 0.000001 THEN 0 ")
    '        sql.AppendLine("				ELSE DEGREES(ATN2(Vel_eo, Vel_ns) + (CASE WHEN ATN2(Vel_eo, Vel_ns) < PI() THEN PI() ELSE -PI() END)) END ")
    '        sql.AppendLine("		, Id_SensoreVel ")
    '        sql.AppendLine("		, Vel = SQRT(SQUARE(Vel_ns) + SQUARE(Vel_eo)) ")
    '        sql.AppendLine("		, Confidenza ")
    '        sql.AppendLine("	FROM ( ")
    '        sql.AppendLine("		SELECT ")
    '        sql.AppendLine("			DataOra ")
    '        sql.AppendLine("			, Id_SensoreDir ")
    '        sql.AppendLine("			, Id_SensoreVel ")
    '        sql.AppendLine("			, Vel_ns = -SUM(Vel_ns) / CAST(COUNT(*) AS REAL) ")
    '        sql.AppendLine("			, Vel_eo = -SUM(Vel_eo) / CAST(COUNT(*) AS REAL) ")
    '        sql.AppendLine("			, COUNT(*) AS Confidenza ")
    '        sql.AppendLine("		FROM ( ")
    '        sql.AppendLine("			SELECT ")
    '        sql.AppendLine("				tblDir.DataOra ")
    '        sql.AppendLine("				, Id_SensoreDir = tblDir.Id_Sensore ")
    '        sql.AppendLine("				, Id_SensoreVel = tblVel.Id_Sensore ")
    '        sql.AppendLine("				, Vel_ns = COS(RADIANS(tblDir.Valore)) * tblVel.Valore ")
    '        sql.AppendLine("				, Vel_eo = SIN(RADIANS(tblDir.Valore)) * tblVel.Valore ")
    '        sql.AppendLine("			FROM ( ")
    '        sql.AppendLine("				SELECT DISTINCT ")
    '        sql.AppendLine("					Id_Stazione ")
    '        sql.AppendLine("					, DataOraSrc ")
    '        sql.AppendLine("					, DataOra ")
    '        sql.AppendLine("					, TipoDir = CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT) ")
    '        sql.AppendLine("					, TipoVel = TipoSensore ")
    '        sql.AppendLine("				FROM #TmpDatiAggregati ")
    '        sql.AppendLine("				WHERE FunAggreg LIKE 'avgvett(%)' ")
    '        sql.AppendLine("			) t1 ")
    '        sql.AppendLine("			INNER JOIN #TmpDatiAggregati tblDir ON tblDir.Id_Stazione = t1.Id_Stazione AND tblDir.DataOraSrc = t1.DataOraSrc AND tblDir.TipoSensore = t1.TipoDir ")
    '        sql.AppendLine("			INNER JOIN #TmpDatiAggregati tblVel ON tblVel.Id_Stazione = t1.Id_Stazione AND tblVel.DataOraSrc = t1.DataOraSrc AND tblVel.TipoSensore = t1.TipoVel ")
    '        sql.AppendLine("		) t2 ")
    '        sql.AppendLine("		GROUP BY DataOra, Id_SensoreDir, Id_SensoreVel ")
    '        sql.AppendLine("	) t3 ")
    '        sql.AppendLine()
    '        sql.AppendLine("	/************************************************************************************************** ")
    '        sql.AppendLine("	Inserisco i dati nella tabella DatiOrari ")
    '        sql.AppendLine("	**************************************************************************************************/ ")
    '        sql.AppendLine()
    '        sql.AppendLine("	DECLARE @TBL_ORARI TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL, Confidenza SMALLINT, IsUpdate BIT NOT NULL DEFAULT (0)) ")
    '        sql.AppendLine("	INSERT INTO @TBL_ORARI (Id_Sensore, DataOra, Valore, Confidenza) ")
    '        sql.AppendLine("	SELECT Id_Sensore, DataOra, Valore, Confidenza ")
    '        sql.AppendLine("	FROM ( ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = AVG(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'avg' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = SUM(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'sum' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = MAX(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'max' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = MIN(Valore), Confidenza = COUNT(*) FROM #TmpDatiAggregati WHERE FunAggreg = 'min' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore ")
    '        sql.AppendLine("			, Valore = CASE WHEN CAST( SUM( CASE WHEN Valore = 0 THEN 1 ELSE 0 END ) AS REAL) / CAST( COUNT(*) AS REAL) >= 0.5 THEN 0 ELSE 100 END ")
    '        sql.AppendLine("			, Confidenza = COUNT(*) ")
    '        sql.AppendLine("		FROM #TmpDatiAggregati WHERE FunAggreg = 'bagnatura' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore = Id_SensoreDir, Valore = Dir, Confidenza FROM @TBL_AGGR_VET ")
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore = Id_SensoreVel, Valore = Vel, Confidenza FROM @TBL_AGGR_VET ")
    '        sql.AppendLine("	) s1 ")
    '        sql.AppendLine()
    '        sql.AppendLine("	SET @RESULT_TOTAL_ORARI = @@ROWCOUNT ")
    '        sql.AppendLine()
    '        sql.AppendLine("	DELETE FROM #TmpUpdated ")
    '        sql.AppendLine()
    '        sql.AppendLine("	UPDATE t ")
    '        sql.AppendLine("	SET t.Valore = s.Valore, t.Confidenza = s.Confidenza ")
    '        sql.AppendLine("	OUTPUT INSERTED.Id_Sensore, INSERTED.DataOra INTO #TmpUpdated (Id_Sensore, DataOra) ")
    '        sql.AppendLine("	FROM MeteoNT_DatiOrari t ")
    '        sql.AppendLine("	INNER JOIN @TBL_ORARI s ON s.Id_Sensore = t.Id_Sensore AND s.DataOra = t.DataOra ")
    '        sql.AppendLine()
    '        sql.AppendLine("	SET @RESULT_UPDATE_ORARI = @@ROWCOUNT ")
    '        sql.AppendLine()
    '        sql.AppendLine("	IF (@RESULT_UPDATE_ORARI < @RESULT_TOTAL_ORARI) -- if all rows were updates then skip, else insert remaining ")
    '        sql.AppendLine("	BEGIN ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UPDATE s ")
    '        sql.AppendLine("		SET s.IsUpdate = 1 ")
    '        sql.AppendLine("		FROM @TBL_ORARI s ")
    '        sql.AppendLine("		INNER JOIN #TmpUpdated u ON u.Id_Sensore = s.Id_sensore AND u.DataOra = s.DataOra ")
    '        sql.AppendLine()
    '        sql.AppendLine("		INSERT INTO MeteoNT_DatiOrari (Id_Sensore, DataOra, Valore, Confidenza) ")
    '        sql.AppendLine("		SELECT Id_Sensore, DataOra, Valore, Confidenza ")
    '        sql.AppendLine("		FROM @TBL_ORARI ")
    '        sql.AppendLine("		WHERE IsUpdate = 0 ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SET @RESULT_INSERT_ORARI = @@ROWCOUNT ")
    '        sql.AppendLine()
    '        sql.AppendLine("	END; ")
    '        sql.AppendLine()
    '        sql.AppendLine("	COMMIT TRAN; ")
    '        sql.AppendLine()
    '        sql.AppendLine("END TRY ")
    '        sql.AppendLine("BEGIN CATCH ")
    '        sql.AppendLine()
    '        sql.AppendLine("	IF (@@TRANCOUNT > 0) ")
    '        sql.AppendLine("	BEGIN ")
    '        sql.AppendLine("		ROLLBACK; ")
    '        sql.AppendLine("	END; ")
    '        sql.AppendLine()
    '        sql.AppendLine("	--THROW; ")
    '        sql.AppendLine("END CATCH; ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpDatiSorgente"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpDatiAggregati"))
    '        sql.AppendLine()
    '        sql.AppendLine(DropTmpTable("#TmpUpdated"))
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("--Ritorno un riepilogo ")
    '        sql.AppendLine()
    '        sql.AppendLine("SELECT T.* FROM (VALUES ")
    '        sql.AppendLine("('MeteoNT_DatiSorgente', @RESULT_TOTAL_SORGENTE, @RESULT_UPDATE_SORGENTE, @RESULT_INSERT_SORGENTE), ")
    '        sql.AppendLine("('MeteoNT_DatiOrari', @RESULT_TOTAL_ORARI, @RESULT_UPDATE_ORARI, @RESULT_INSERT_ORARI) ")
    '        sql.AppendLine(") T(Tabella, [TotalCnt], [Update], [Insert]) ")

    '        Riepilogo = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '    Catch ex As Exception

    '        Dim MessaggioErrore As String = "[" & NomeRoutine & "]:" & ex.Message & " "
    '        Throw New Exception(MessaggioErrore)
    '    End Try

    '    Return Riepilogo
    'End Function

    'Private Function ScriviDatiSorgente_(ByVal tabellaSource As String, ByVal sqlSource As String, ByVal flagUTC As Boolean, ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim NomeRoutine As String = "MeteoNT.ScriviDatiSorgente"
    '    Dim MessaggioErrore As String = ""
    '    Dim sqlFrom As New StringBuilder
    '    Dim sql As New StringBuilder
    '    Dim RetValue As Boolean = True

    '    Try

    '        sqlFrom.Clear()
    '        sqlFrom.Append("FROM (")
    '        sqlFrom.Append(sqlSource)
    '        sqlFrom.Append(") dati ")
    '        sqlFrom.AppendLine("INNER JOIN MeteoNT_Sensori			nt_s	ON nt_s.ChiaveImport = dati.ChiaveImport")
    '        sqlFrom.AppendLine("INNER JOIN MeteoNT_StazioniXSensori	sxs		ON sxs.Id_Sensore = nt_s.Id ")
    '        sqlFrom.AppendLine("INNER JOIN MeteoNT_Stazioni			staz	ON staz.Id = sxs.Id_Stazione ")
    '        sqlFrom.AppendLine("WHERE sxs.FlagReale = 1")

    '        sql.Clear()

    '        sql.AppendLine("SET NOCOUNT ON ")
    '        sql.AppendLine()
    '        sql.AppendLine("BEGIN TRANSACTION ")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @TBLRESULT TABLE(Tabella VARCHAR(50), Operazione VARCHAR(20), Conteggio INT) ")
    '        sql.AppendLine("DECLARE @RiepilogoMerge TABLE(Operazione VARCHAR(20)) ")
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("Leggo i dati per la tabella specifica del fornitore")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("DECLARE @SRCTBL TABLE(Id_Stazione INT, Id_Sensore INT, DataOra DATETIME, Valore REAL) ")
    '        sql.AppendLine("INSERT INTO @SRCTBL ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("	staz.Id AS Id_Stazione ")
    '        sql.AppendLine("    , nt_s.Id AS Id_Sensore ")
    '        If flagUTC Then
    '            sql.AppendLine("	, CAST(DataOraUTC AT TIME ZONE 'UTC' AT TIME ZONE ISNULL(staz.SystemTimeZoneInfo, 'W. Europe Standard Time') AS DATETIME) AS DataOra ")
    '        Else
    '            sql.AppendLine("	, DataOra ")
    '        End If
    '        sql.AppendLine("	, Valore ")
    '        sql.Append(sqlFrom.ToString)
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("Inserisco i dati nella tabella DatiSorgente")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("MERGE INTO MeteoNT_DatiSorgente AS t ")
    '        sql.AppendLine("USING ")
    '        sql.AppendLine("	(SELECT Id_Sensore, DataOra, Valore FROM @SRCTBL) AS s ")
    '        sql.AppendLine("ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
    '        sql.AppendLine("WHEN MATCHED THEN ")
    '        sql.AppendLine("	UPDATE SET Valore = s.Valore")
    '        sql.AppendLine("WHEN NOT MATCHED THEN ")
    '        sql.AppendLine("	INSERT (Id_Sensore, DataOra, Valore) ")
    '        sql.AppendLine("	VALUES (s.Id_Sensore, s.DataOra, s.Valore) ")
    '        sql.AppendLine("OUTPUT $action INTO @RiepilogoMerge; ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("INSERT INTO @TBLRESULT ")
    '        sql.AppendLine("SELECT 'MeteoNT_DatiSorgente', Operazione, COUNT(*) AS Conteggio FROM @RiepilogoMerge GROUP BY Operazione ")
    '        sql.AppendLine()
    '        sql.AppendLine("DELETE FROM @RiepilogoMerge ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("Aggiorno il flag elaborato per ogni sensore coinvolto")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("UPDATE " & tabellaSource)
    '        sql.AppendLine("SET Elaborato = 1 ")
    '        sql.AppendLine("WHERE Elaborato = 0 ")
    '        sql.AppendLine("AND Id_Sensore IN ( ")
    '        sql.AppendLine("SELECT DISTINCT dati.Id_Sensore ")
    '        sql.Append(sqlFrom.ToString)
    '        sql.AppendLine(")")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("Per ogni sensore memorizzo le date (orarie) min/max")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("DECLARE @SENSORI_DT table(Id_Sensore INT, Min_DataOra DATETIME, Max_DataOra DATETIME) ")
    '        sql.AppendLine("INSERT INTO @SENSORI_DT ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("	Id_Sensore ")
    '        sql.AppendLine("	, DATEADD(hh, DATEDIFF(hh, '19000101', MIN(DataOra)), '19000101') AS Min_DataOra ")
    '        sql.AppendLine("	, DATEADD(hh, DATEDIFF(hh, '19000101', MAX(DataOra)) + 1, '19000101') AS Max_DataOra ")
    '        sql.AppendLine("FROM @SRCTBL ")
    '        sql.AppendLine("GROUP BY Id_Sensore")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("Produco i dati aggregati da dati originali Tabella DatiSorgente -> Tabelle DatiOrari")
    '        sql.AppendLine("Stessi sensori aggregati secondo la funzione di aggregazione propria del tipo sensore")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("DECLARE @TMPTABLE TABLE(Id_Stazione INT, DataOraSrc DATETIME, DataOra DATETIME, Id_Sensore INT, Valore REAL, TipoSensore INT, FunAggreg VARCHAR(50)) ")
    '        sql.AppendLine("INSERT INTO @TMPTABLE ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("    stazXsens.Id_Stazione ")
    '        sql.AppendLine("	, DataOra AS DataOraSrc ")
    '        sql.AppendLine("	, DATEADD(hh, DATEDIFF(hh, '19000101', DataOra), '19000101') AS DataOra ")
    '        sql.AppendLine("	, dati.Id_Sensore ")
    '        sql.AppendLine("	, Valore ")
    '        sql.AppendLine("	, Id_TipoSensore ")
    '        sql.AppendLine("	, FunAggreg ")
    '        sql.AppendLine("FROM MeteoNT_DatiSorgente           dati ")
    '        sql.AppendLine("INNER JOIN MeteoNT_Sensori          sens        ON sens.Id = dati.Id_Sensore ")
    '        sql.AppendLine("INNER JOIN MeteoNT_TipiSensore      tipi        ON tipi.Id = sens.Id_TipoSensore ")
    '        sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori stazXsens   ON stazXsens.Id_Sensore = dati.Id_Sensore")
    '        sql.AppendLine("INNER JOIN @SENSORI_DT              sdt         ON sdt.Id_Sensore = dati.Id_Sensore ")
    '        sql.AppendLine("WHERE sdt.Min_DataOra <= dati.DataOra AND dati.DataOra < sdt.Max_DataOra ")
    '        sql.AppendLine("AND stazXsens.FlagReale = 1 ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("AGGREGAZIONE PER SENSORI VETTORIALI")
    '        sql.AppendLine()
    '        sql.AppendLine("La funzione di aggregazione di un tipo vettoriale è definita come avgvett(x) con x il tipo del sensore che identifica")
    '        sql.AppendLine("la direzione (angolo) del vettore la cui velocità (magnitudine) è il tipo stesso.")
    '        sql.AppendLine("Ad esempio per una coppia di vettori che acquisiscono la direzione e velocità del vento ")
    '        sql.AppendLine("la direzione (Id Tipo = 6, ad esempio) non ha funzione di aggregazione nella tabella dei TipiSensore, ")
    '        sql.AppendLine("mentre la velocità (Id Tipo = 7 ad esempio) ha come funzione di aggregazione avgvett(6)")
    '        sql.AppendLine("ad indicare che il tipo 7 sarà accoppiato al tipo 6 nei calcoli di aggregazione.")
    '        sql.AppendLine()
    '        sql.AppendLine("N.B. Fallisce se in uno stesso insieme ho due sensori dello stesso tipo, ad esempio due anemometri, ")
    '        sql.AppendLine("che hanno la stessa funzione di aggreazione avgvett(x)")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("DECLARE @TBL_AGGR_VET TABLE(DataOra DATETIME, Id_SensoreDir INT, Dir REAL, Id_SensoreVel INT, Vel REAL, Confidenza SMALLINT) ")
    '        sql.AppendLine("INSERT INTO @TBL_AGGR_VET ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("	DataOra ")
    '        sql.AppendLine("	, Id_SensoreDir ")
    '        sql.AppendLine("	, CASE WHEN ABS(Vel_eo) < 0.000001 AND ABS(Vel_ns) < 0.000001 ")
    '        sql.AppendLine("		THEN 0 ")
    '        sql.AppendLine("		ELSE DEGREES(ATN2(Vel_eo, Vel_ns) + (CASE WHEN ATN2(Vel_eo, Vel_ns) < PI() THEN PI() ELSE -PI() END)) ")
    '        sql.AppendLine("		END AS Dir ")
    '        sql.AppendLine("	, Id_SensoreVel ")
    '        sql.AppendLine("	, SQRT(SQUARE(Vel_ns) + SQUARE(Vel_eo)) AS Vel ")
    '        sql.AppendLine("	, Confidenza ")
    '        sql.AppendLine("FROM ( ")
    '        sql.AppendLine("	SELECT ")
    '        sql.AppendLine("		DataOra ")
    '        sql.AppendLine("		, Id_SensoreDir ")
    '        sql.AppendLine("		, Id_SensoreVel ")
    '        sql.AppendLine("		, -SUM(Vel_ns) / CAST(COUNT(*) AS REAL) AS Vel_ns ")
    '        sql.AppendLine("		, -SUM(Vel_eo) / CAST(COUNT(*) AS REAL) AS Vel_eo ")
    '        sql.AppendLine("		, COUNT(*) AS Confidenza ")
    '        sql.AppendLine("	FROM ( ")
    '        sql.AppendLine("		SELECT ")
    '        sql.AppendLine("			tblDir.DataOra ")
    '        sql.AppendLine("			, tblDir.Id_Sensore AS Id_SensoreDir ")
    '        sql.AppendLine("			, tblVel.Id_Sensore AS Id_SensoreVel ")
    '        sql.AppendLine("			, COS(RADIANS(tblDir.Valore)) * tblVel.Valore AS Vel_ns ")
    '        sql.AppendLine("			, SIN(RADIANS(tblDir.Valore)) * tblVel.Valore AS Vel_eo ")
    '        sql.AppendLine("		FROM ( ")
    '        sql.AppendLine("			SELECT DISTINCT ")
    '        sql.AppendLine("				Id_Stazione ")
    '        sql.AppendLine("				, DataOraSrc ")
    '        sql.AppendLine("				, DataOra ")
    '        sql.AppendLine("				, CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT) AS TipoDir ")
    '        sql.AppendLine("				, TipoSensore AS TipoVel ")
    '        sql.AppendLine("			FROM @TMPTABLE ")
    '        sql.AppendLine("			WHERE FunAggreg LIKE 'avgvett(%)' ")
    '        sql.AppendLine("		) t1 ")
    '        sql.AppendLine("		INNER JOIN @TMPTABLE tblDir ON tblDir.Id_Stazione = t1.Id_Stazione AND tblDir.DataOraSrc = t1.DataOraSrc AND tblDir.TipoSensore = t1.TipoDir ")
    '        sql.AppendLine("		INNER JOIN @TMPTABLE tblVel ON tblVel.Id_Stazione = t1.Id_Stazione AND tblVel.DataOraSrc = t1.DataOraSrc AND tblVel.TipoSensore = t1.TipoVel ")
    '        sql.AppendLine("	) t2 ")
    '        sql.AppendLine("	GROUP BY DataOra, Id_SensoreDir, Id_SensoreVel ")
    '        sql.AppendLine(") t3 ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("/**************************************************************************************************")
    '        sql.AppendLine("Inserisco i dati nella tabella DatiOrari")
    '        sql.AppendLine("**************************************************************************************************/")
    '        sql.AppendLine("MERGE INTO MeteoNT_DatiOrari AS t ")
    '        sql.AppendLine("USING ")
    '        sql.AppendLine("	(SELECT Id_Sensore, DataOra, Valore, Confidenza ")
    '        sql.AppendLine("	FROM ( ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore, COUNT(*) AS Confidenza ")
    '        sql.AppendLine("		FROM @TMPTABLE ")
    '        sql.AppendLine("		WHERE FunAggreg = 'avg' ")
    '        sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore, COUNT(*) AS Confidenza ")
    '        sql.AppendLine("		FROM @TMPTABLE ")
    '        sql.AppendLine("		WHERE FunAggreg = 'sum' ")
    '        sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore, COUNT(*) AS Confidenza ")
    '        sql.AppendLine("		FROM @TMPTABLE ")
    '        sql.AppendLine("		WHERE FunAggreg = 'max' ")
    '        sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT DataOra, Id_Sensore, CASE WHEN CAST( SUM( CASE WHEN Valore = 0 THEN 1 ELSE 0 END ) AS REAL) / CAST( COUNT(*) AS REAL) >= 0.5 THEN 0 ELSE 100 END AS Valore, COUNT(*) AS Confidenza ")
    '        sql.AppendLine("		FROM @TMPTABLE ")
    '        sql.AppendLine("		WHERE FunAggreg = 'bagnatura' ")
    '        sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT DataOra, Id_SensoreDir AS Id_Sensore, Dir AS Valore, Confidenza ")
    '        sql.AppendLine("		FROM @TBL_AGGR_VET ")
    '        sql.AppendLine()
    '        sql.AppendLine("		UNION ")
    '        sql.AppendLine()
    '        sql.AppendLine("		SELECT DataOra, Id_SensoreVel AS Id_Sensore, Vel AS Valore, Confidenza ")
    '        sql.AppendLine("		FROM @TBL_AGGR_VET ")
    '        sql.AppendLine("	) s1 ")
    '        sql.AppendLine(") AS s ")
    '        sql.AppendLine("ON t.Id_Sensore = s.Id_Sensore AND t.DataOra = s.DataOra ")
    '        sql.AppendLine("WHEN MATCHED THEN ")
    '        sql.AppendLine("	UPDATE SET Valore = s.Valore, Confidenza = s.Confidenza ")
    '        sql.AppendLine("WHEN NOT MATCHED THEN ")
    '        sql.AppendLine("	INSERT (Id_Sensore, DataOra, Valore, Confidenza) ")
    '        sql.AppendLine("	VALUES (s.Id_Sensore, s.DataOra, s.Valore, s.Confidenza) ")
    '        sql.AppendLine("OUTPUT $action INTO @RiepilogoMerge; ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("INSERT INTO @TBLRESULT ")
    '        sql.AppendLine("SELECT 'MeteoNT_DatiOrari', Operazione, COUNT(*) AS Conteggio FROM @RiepilogoMerge GROUP BY Operazione ")
    '        sql.AppendLine()
    '        sql.AppendLine("--Ritorno un riepilogo")
    '        sql.AppendLine("SELECT * FROM @TBLRESULT ")
    '        sql.AppendLine()
    '        sql.AppendLine("COMMIT TRANSACTION ")

    '        Dim DT As DataTable = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '        'RIEPILOGO RITORNATO...
    '        'MeteoNT_DatiSorgente	INSERT	xxx
    '        'MeteoNT_DatiSorgente	UPDATE	yyy
    '        'MeteoNT_DatiOrari	    INSERT	zzz
    '        'MeteoNT_DatiOrari	    UPDATE	vvv

    '    Catch ex As Exception

    '        RetValue = False
    '        MessaggioErrore = "[" & NomeRoutine & "]:" & ex.Message & " "
    '        Throw New Exception(MessaggioErrore)

    '    End Try

    '    Return RetValue

    'End Function

    'Public Function ScriviDatiSorgente_WiNet_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOraUTC ")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'WINET-' + CAST(s.Id_Rete AS VARCHAR(10)) + '-' + CAST(s.Id_Nodo AS VARCHAR(10)) + '-' + CAST(s.Id_Sensore AS VARCHAR(10)) AS ChiaveImport ")
    '    sql.AppendLine("FROM WiNet2_Dati			d ")
    '    sql.AppendLine("INNER JOIN WiNet2_Sensori	s	ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("WiNet2_Dati", sql.ToString, True, ObjParametri_Server)
    'End Function

    'Public Function ScriviDatiSorgente_AS_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOraUTC ")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'AS-' + CAST(s.Id_Sistema AS VARCHAR(10)) + '-' + CAST(s.Id_Unita AS VARCHAR(10)) + '-' + CAST(s.Id_Sensore AS VARCHAR(10)) AS ChiaveImport ")
    '    sql.AppendLine("FROM AS_Dati			d ")
    '    sql.AppendLine("INNER JOIN AS_Sensori	s	ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("AS_Dati", sql.ToString, True, ObjParametri_Server)
    'End Function

    'Public Function ScriviDatiSorgente_NetSens_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOraUTC ")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'NETSENS-' + CAST(s.Id_Stazione AS VARCHAR(10)) + '-' + CAST(s.Id_Unita AS VARCHAR(10)) + '-' + CAST(s.Id_Sensore AS VARCHAR(10)) AS ChiaveImport ")
    '    sql.AppendLine("FROM NetSens_Dati		    d ")
    '    sql.AppendLine("INNER JOIN NetSens_Sensori	s	ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("NetSens_Dati", sql.ToString, True, ObjParametri_Server)
    'End Function

    'Public Function ScriviDatiSorgente_Pessl_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOra")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'PESSL-' + TRIM(Id_Station) + '-' + CAST(Ch AS VARCHAR(20)) + '-' + Mac + '-' + Serial + '-' + CAST(Code AS VARCHAR(20)) + '-' + Aggr AS ChiaveImport ")
    '    sql.AppendLine("FROM Pessl_Dati		        d ")
    '    sql.AppendLine("INNER JOIN Pessl_Sensori	s	ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("Pessl_Dati", sql.ToString, False, ObjParametri_Server)
    'End Function

    'Public Function ScriviDatiSorgente_GreenPlanet_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOra")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'GP-' + CAST(Id_Stazione AS VARCHAR(10)) + '-' + TRIM(Sensore) AS ChiaveImport ")
    '    sql.AppendLine("FROM GreenPlanet_Dati          d ")
    '    sql.AppendLine("INNER JOIN GreenPlanet_Sensori s ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("GreenPlanet_Dati", sql.ToString, False, ObjParametri_Server)
    'End Function

    'Public Function ScriviDatiSorgente_A2A_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOraUTC ")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'A2A-' + TRIM(Id_Stazione) + '-' + TRIM(MeasureName) AS ChiaveImport ")
    '    sql.AppendLine("FROM A2A_Dati d ")
    '    sql.AppendLine("INNER JOIN A2A_Sensori s ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("A2A_Dati", sql.ToString, True, ObjParametri_Server)
    'End Function

    'Public Function ScriviDatiSorgente_DigiFarm_(ByVal ObjParametri_Server As AgronicaCoreParametri) As Boolean

    '    Dim sql As New StringBuilder
    '    sql.Clear()
    '    sql.AppendLine("SELECT ")
    '    sql.AppendLine("	d.Id_Sensore ")
    '    sql.AppendLine("	, DataOra ")
    '    sql.AppendLine("	, Valore ")
    '    sql.AppendLine("	, 'DF-' + TRIM(Id_Stazione) + '-' + TRIM(SensorId) AS ChiaveImport ")
    '    sql.AppendLine("FROM DigiFarm_Dati d ")
    '    sql.AppendLine("INNER JOIN DigiFarm_Sensori s ON s.Id = d.Id_Sensore ")
    '    sql.AppendLine("WHERE d.Elaborato = 0 ")

    '    Return ScriviDatiSorgente_("DigiFarm_Dati", sql.ToString, False, ObjParametri_Server)
    'End Function




#Region "Lettura dati"

    Private Function CreateTableDati(ByVal tablename As String) As String

        Dim sql As New StringBuilder

        sql.Append(DropTableDati(tablename))
        sql.AppendLine("CREATE TABLE #" & tablename & " (DataOra DATETIME, Id_Sensore INT, Valore REAL) ")
        sql.AppendLine()

        Return sql.ToString
    End Function

    Private Function DropTableDati(ByVal tablename As String) As String

        Dim sql As New StringBuilder

        sql.AppendLine("BEGIN TRY")
        sql.AppendLine("    DROP TABLE #" & tablename)
        sql.AppendLine("END TRY")
        sql.AppendLine("BEGIN CATCH")
        sql.AppendLine("END CATCH")
        sql.AppendLine()

        Return sql.ToString
    End Function

    Public Enum enum_GranularitaDati
        Sorgente = 0
        Orari = 1
        Giornalieri = 2
    End Enum


    Public Function LeggiDati(TipoSorgente As enum_IoT_Tiposorgente, IdDispositivo As Integer, DataInizio As DateTime, DataFine As DateTime, granularita As enum_GranularitaDati, SogliaTermica As Decimal, SogliaFabbisognoFreddo As Decimal, ObjParametri_Server As AgronicaCoreParametri) As DataSet

        Dim NomeRoutine As String = "IOT.LeggiDati"

        Try

            Dim sql As New StringBuilder

            sql.Clear()
            sql.AppendLine("SET NOCOUNT ON ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @DISPOSITIVO INT = " & CStr(IdDispositivo))
            sql.AppendLine("DECLARE @DATAINIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizio))
            sql.AppendLine("DECLARE @DATAFINE DATETIME = " & Agro_SQL_SaveDateTime(DataFine))
            sql.AppendLine("DECLARE @GRANULARITA_GG BIT = " & If(granularita = enum_GranularitaDati.Giornalieri, "1", "0"))
            sql.AppendLine("DECLARE @SOGLIA_TERMICA REAL = " & SogliaTermica.ToString(Globalization.CultureInfo.InvariantCulture))
            sql.AppendLine("DECLARE @SOGLIA_FABBISOGNO_FREDDO REAL = " & SogliaFabbisognoFreddo.ToString(Globalization.CultureInfo.InvariantCulture))
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_SENSORI TABLE(Id INT, Id_Src INT, Id_TipoSensore INT, Etichetta VARCHAR(MAX), Ordine INT, UM VARCHAR(10), FunAggreg VARCHAR(50), Misura VARCHAR(MAX), OutputConfig VARCHAR(MAX), NomeColonna VARCHAR(MAX), FlagOutput BIT, OutputRiepilogo VARCHAR(MAX), conv_xOffs REAL, conv_Mult REAL, conv_Denom REAL, conv_yOffs REAL) ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_DATI TABLE(DataOraSrc DATETIME, DataOra DATETIME, Id_Sensore INT, Valore REAL, TipoSensore INT, FunAggreg VARCHAR(50)) ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_TMPDERIVATI TABLE(Id_TipoSensoreSrc INT, FunDeriv VARCHAR(20), Descrizione VARCHAR(MAX), Ordine INT, Misura VARCHAR(MAX), OutputConfig VARCHAR(MAX), UM VARCHAR(10), DailyOnlyFlag BIT, HourlyFlagOutput BIT) ")
            sql.AppendLine("INSERT INTO @TBL_TMPDERIVATI VALUES ")
            sql.AppendLine("(1, 'min', ' [min]', 1, NULL, '{""serie"": ""rangeArea"", ""fieldProp"": ""fromField"", ""opacity"": 0.25, ""sensor_postfix"": "" min/max""}', NULL, 0, 0), ")
            sql.AppendLine("(1, 'max', ' [max]', 2, NULL, '{""serie"": ""rangeArea"", ""fieldProp"": ""toField"", ""opacity"": 0.25, ""sensor_postfix"": "" min/max""}', NULL, 0, 0) ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_SXS TABLE(Id_Dispositivo INT, Id_Sensore INT, Id_TipoSensore INT, Ordine INT, OutputConfig VARCHAR(MAX), Etichetta VARCHAR(MAX)) ")

            Select Case TipoSorgente

                'Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                '    sql.AppendLine("INSERT INTO @TBL_SXS VALUES ")
                '    sql.AppendLine("(@STAZIONE, 1, 1, 1, NULL, 'Temperatura aria'), ")
                '    sql.AppendLine("(@STAZIONE, 2, 3, 2, NULL, 'Umidità relativa'), ")
                '    sql.AppendLine("(@STAZIONE, 3, 4, 4, NULL, 'Bagnatura fogliare'), ")
                '    sql.AppendLine("(@STAZIONE, 4, 2, 3, NULL, 'Pioggia') ")
                '    sql.AppendLine()
                '    sql.AppendLine("DECLARE @ID_SENSORE INTEGER = (SELECT MAX(Id_Sensore) FROM @TBL_SXS) ")

                Case enum_IoT_Tiposorgente.Aziendali, enum_IoT_Tiposorgente.Pubbliche

                    sql.AppendLine("INSERT INTO @TBL_SXS ")
                    sql.AppendLine("SELECT Id_Dispositivo, Id_Sensore, Id_TipoSensore, Ordine, OutputConfig, COALESCE(sxs.Etichetta, s.Etichetta) ")
                    sql.AppendLine("FROM IOT_DispositiviXSensori sxs ")
                    sql.AppendLine("INNER JOIN IOT_Sensori s ON s.Id = sxs.Id_Sensore ")
                    sql.AppendLine("WHERE Id_Dispositivo = @DISPOSITIVO")
                    sql.AppendLine()
                    sql.AppendLine("DECLARE @ID_SENSORE INTEGER = (SELECT MAX(Id) FROM IOT_Sensori) ")

            End Select

            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TBL_SENSORI ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id = sxs.Id_Sensore ")
            sql.AppendLine("	, Id_Src = sxs.Id_Sensore ")
            sql.AppendLine("	, Id_TipoSensore ")
            sql.AppendLine("	, sxs.Etichetta ")
            sql.AppendLine("	, Ordine * 10 ")
            sql.AppendLine("	, UM ")
            sql.AppendLine("	, FunAggreg ")
            sql.AppendLine("	, Misura ")
            sql.AppendLine("	, OutputConfig = COALESCE(sxs.OutputConfig, tipi.DefaultOutputConfig, '') ")
            sql.AppendLine("	, NomeColonna = 'S_' + CAST(sxs.Id_Sensore AS VARCHAR(10)) ")
            sql.AppendLine("	, FlagOutput = 1 ")
            sql.AppendLine("	, OutputRiepilogo = sxs.Etichetta ")
            sql.AppendLine("	, 0 ")
            sql.AppendLine("	, 1 ")
            sql.AppendLine("	, 1 ")
            sql.AppendLine("	, 0 ")
            sql.AppendLine("FROM @TBL_SXS sxs ")
            sql.AppendLine("INNER JOIN ( ")
            sql.AppendLine("	SELECT Id, Misura, UM, FunAggreg, DefaultOutputConfig FROM IOT_TipiSensore WHERE Id = 0 ")
            sql.AppendLine("	UNION ")
            sql.AppendLine("	SELECT Id, Misura = COALESCE(Misura, Tipo),  UM, FunAggreg, DefaultOutputConfig FROM IoT_TipiSensore WHERE Id <> 0 ")
            sql.AppendLine(") tipi ON tipi.Id = sxs.Id_TipoSensore ")
            sql.AppendLine("WHERE sxs.id_dispositivo = @DISPOSITIVO ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TBL_SENSORI ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	Id = @ID_SENSORE + Offset ")
            sql.AppendLine("	, Id_Src ")
            sql.AppendLine("	, Id_TipoSensore ")
            sql.AppendLine("	, Etichetta ")
            sql.AppendLine("	, Ordine = Ordine + Offset ")
            sql.AppendLine("	, UM ")
            sql.AppendLine("	, FunAggreg ")
            sql.AppendLine("	, Misura ")
            sql.AppendLine("	, OutputConfig ")
            sql.AppendLine("	, NomeColonna = 'SD_' + UPPER(FunAggreg) + '_' + CAST(Id_Src AS VARCHAR(10)) ")
            sql.AppendLine("	, FlagOutput = IIF(@GRANULARITA_GG = 1 OR T.HourlyFlagOutput = 1, 1, 0) ")
            sql.AppendLine("	, OutputRiepilogo ")
            sql.AppendLine("	, 0 ")
            sql.AppendLine("	, 1 ")
            sql.AppendLine("	, 1 ")
            sql.AppendLine("	, 0 ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT ")
            sql.AppendLine("		Offset = ROW_NUMBER() OVER (ORDER BY sens.id, deriv.Ordine) ")
            sql.AppendLine("		, Id_Src = sens.Id ")
            sql.AppendLine("		, Id_TipoSensore = sens.Id_TipoSensore ")
            sql.AppendLine("		, Etichetta = sens.Etichetta + deriv.Descrizione ")
            sql.AppendLine("		, Ordine = sens.Ordine ")
            sql.AppendLine("		, UM = COALESCE(deriv.UM, sens.UM) ")
            sql.AppendLine("		, FunAggreg = deriv.FunDeriv ")
            sql.AppendLine("		, Misura = COALESCE(deriv.Misura, sens.Misura) ")
            sql.AppendLine("		, OutputConfig = deriv.OutputConfig ")
            sql.AppendLine("		, OutputRiepilogo = sens.Etichetta ")
            sql.AppendLine("		, HourlyFlagOutput = deriv.HourlyFlagOutput ")
            sql.AppendLine("	FROM @TBL_SENSORI sens ")
            sql.AppendLine("	INNER JOIN @TBL_TMPDERIVATI deriv ON deriv.Id_TipoSensoreSrc = sens.Id_TipoSensore AND (@GRANULARITA_GG = 1 OR deriv.DailyOnlyFlag = 0) ")
            sql.AppendLine(") T ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("--y = ((x + xOffset) * multiplicand / denominator) + yOffset ")
            sql.AppendLine()
            sql.AppendLine("DECLARE @TBL_CONV TABLE(Id_TipoSrc INT, Id_TipoDst INT, xOffs REAL, Mult REAL, Denom REAL, yOffs REAL) ")
            sql.AppendLine("INSERT INTO @TBL_CONV VALUES ")
            sql.AppendLine("(9, 10, 0.0, 3600.0, 1000.0, 0.0),	--Velocità vento:	m/s -> Km/h (vettoriale) ")
            sql.AppendLine("(12, 13, 0.0, 3600.0, 1000.0, 0.0),	--Velocità raffica:	m/s -> Km/h (vettoriale) ")
            sql.AppendLine("(23, 24, 0.0, 3600.0, 1000.0, 0.0),	--Velocità vento:	m/s -> Km/h (scalare) ")
            sql.AppendLine("(25, 26, 0.0, 3600.0, 1000.0, 0.0),	--Velocità raffica:	m/s -> Km/h (scalare) ")
            sql.AppendLine("(33, 32, -815.0, 1, 994.0, 0.0)		--Conducibilità elettrica: VIC -> dS/m ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("UPDATE sens ")
            sql.AppendLine("SET Id_TipoSensore = conv.Id_TipoDst, UM = tipi.UM, Misura = COALESCE(tipi.Misura, tipi.Tipo), conv_xOffs = conv.xOffs, conv_Mult = conv.Mult, conv_Denom = conv.Denom, conv_yOffs = conv.yOffs ")
            sql.AppendLine("FROM @TBL_SENSORI sens ")
            sql.AppendLine("INNER JOIN @TBL_CONV conv ON conv.Id_TipoSrc = sens.Id_TipoSensore ")
            sql.AppendLine("INNER JOIN IOT_TipiSensore tipi ON tipi.Id = conv.Id_TipoDst ")
            sql.AppendLine()
            sql.AppendLine()

            Dim tblDati As String = "IOT_DatiOrari"
            Dim whereDati As String = ""

            Select Case TipoSorgente

                'Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

                '    Dim Tabella_RER As String = "Dati_Meteo_SHH"
                '    Dim Campo_ID As String = "ID_Stazione"

                '    If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

                '        Tabella_RER = "Dati_Meteo_QHH"
                '        Campo_ID = "ID_Quadrante"
                '    End If

                '    sql.AppendLine("; WITH SRC_CTE AS ( ")
                '    sql.AppendLine("    SELECT Tempo, Temp_Media, UmiditaRelativa, Bagnatura, Precipitazione FROM " & Tabella_RER & " WHERE " & Campo_ID & " = @STAZIONE AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE ")
                '    sql.AppendLine("), ")
                '    sql.AppendLine("DATI_RER AS ( ")
                '    sql.AppendLine("	SELECT Id_Sensore = 1, DataOra = Tempo, Valore = Temp_Media FROM SRC_CTE ")
                '    sql.AppendLine("	UNION ")
                '    sql.AppendLine("	SELECT Id_Sensore = 2, DataOra = Tempo, Valore = UmiditaRelativa FROM SRC_CTE ")
                '    sql.AppendLine("	UNION ")
                '    sql.AppendLine("	SELECT Id_Sensore = 3, DataOra = Tempo, Valore = Bagnatura FROM SRC_CTE ")
                '    sql.AppendLine("	UNION ")
                '    sql.AppendLine("	SELECT Id_Sensore = 4, DataOra = Tempo, Valore = Precipitazione FROM SRC_CTE ")
                '    sql.AppendLine(") ")

                '    tblDati = "DATI_RER"

                Case enum_IoT_Tiposorgente.Aziendali, enum_IoT_Tiposorgente.Pubbliche

                    If TipoSorgente <> enum_IoT_Tiposorgente.Pubbliche AndAlso granularita = enum_GranularitaDati.Sorgente Then

                        tblDati = "IOT_DatiOrari"
                    End If

                    whereDati = "WHERE @DATAINIZIO <= DataOra AND DataOra <= @DATAFINE "

            End Select

            sql.AppendLine()
            sql.AppendLine("INSERT INTO @TBL_DATI ")
            sql.AppendLine("SELECT ")
            sql.AppendLine("	DataOraSrc = DataOra ")
            sql.AppendLine("	, DataOra = CASE WHEN @GRANULARITA_GG = 1 THEN DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra)) ELSE DataOra END ")
            sql.AppendLine("	, Id_Sensore = sens.Id ")
            sql.AppendLine("	, Valore = ((Valore + sens.conv_xOffs) * sens.conv_Mult / sens.conv_Denom) + sens.conv_yOffs ")
            sql.AppendLine("	, Id_TipoSensore ")
            sql.AppendLine("	, FunAggreg ")
            sql.AppendLine("FROM ( ")
            sql.AppendLine("	SELECT Id_Sensore = sens.Id, DataOra, Valore ")
            sql.AppendLine("	FROM " & tblDati & " dati ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI sens ON sens.Id_Src = dati.Id_Sensore ")
            sql.AppendLine("	" & whereDati)
            sql.AppendLine(") D ")
            sql.AppendLine("INNER JOIN @TBL_SENSORI sens ON sens.Id = D.Id_Sensore ")
            sql.AppendLine()
            sql.AppendLine()
            sql.Append(CreateTableDati("DatiIoT"))
            sql.AppendLine()
            sql.AppendLine("IF @GRANULARITA_GG = 1 ")
            sql.AppendLine("BEGIN ")
            sql.AppendLine()
            'AGGREGAZIONE PER SENSORI VETTORIALI
            'N.B.Fallisce se in uno stesso insieme ho due sensori dello stesso tipo, ad esempio due anemometri, 
            'che hanno la stessa funzione di aggreazione avgvett(x, y)
            sql.AppendLine("    DECLARE @TBL_AGGR_VET TABLE(DataOra DATETIME, Id_SensoreDir INT, Dir REAL, Id_SensoreVel INT, Vel REAL) ")
            sql.AppendLine("    INSERT INTO @TBL_AGGR_VET ")
            sql.AppendLine("    SELECT  ")
            sql.AppendLine("        DataOra ")
            sql.AppendLine("	    , Id_SensoreDir ")
            sql.AppendLine("	    , CASE WHEN ABS(Vel_eo) < 0.000001 AND ABS(Vel_ns) < 0.000001 ")
            sql.AppendLine("		    THEN 0 ")
            sql.AppendLine("		    ELSE DEGREES(ATN2(Vel_eo, Vel_ns) + (CASE WHEN ATN2(Vel_eo, Vel_ns) < PI() THEN PI() ELSE -PI() END)) ")
            sql.AppendLine("		    END AS Dir ")
            sql.AppendLine("	    , Id_SensoreVel ")
            sql.AppendLine("	    , SQRT(SQUARE(Vel_ns) + SQUARE(Vel_eo)) AS Vel ")
            sql.AppendLine("    FROM ( ")
            sql.AppendLine("	    SELECT ")
            sql.AppendLine("	    	DataOra ")
            sql.AppendLine("	    	, Id_SensoreDir ")
            sql.AppendLine("	    	, Id_SensoreVel ")
            sql.AppendLine("	    	, -SUM(Vel_ns) / CAST(COUNT(*) AS REAL) AS Vel_ns ")
            sql.AppendLine("	    	, -SUM(Vel_eo) / CAST(COUNT(*) AS REAL) AS Vel_eo ")
            sql.AppendLine("	    FROM ( ")
            sql.AppendLine("	    	SELECT ")
            sql.AppendLine("	    		tblDir.DataOra ")
            sql.AppendLine("	    		, tblDir.Id_Sensore AS Id_SensoreDir  ")
            sql.AppendLine("	    		, tblVel.Id_Sensore AS Id_SensoreVel  ")
            sql.AppendLine("	    		, COS(RADIANS(tblDir.Valore)) * tblVel.Valore AS Vel_ns ")
            sql.AppendLine("	    		, SIN(RADIANS(tblDir.Valore)) * tblVel.Valore AS Vel_eo  ")
            sql.AppendLine("	    	FROM ( ")
            sql.AppendLine("	    		SELECT DISTINCT ")
            sql.AppendLine("	    			DataOraSrc ")
            sql.AppendLine("	    			, DataOra ")
            sql.AppendLine("	    			, CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT) AS TipoDir ")
            sql.AppendLine("	    			, TipoSensore AS TipoVel ")
            sql.AppendLine("	    		FROM @TBL_DATI ")
            sql.AppendLine("	    		WHERE FunAggreg LIKE 'avgvett(%)' ")
            sql.AppendLine("	    	) t1 ")
            sql.AppendLine("	    	INNER JOIN @TBL_DATI tblDir ON tblDir.DataOraSrc = t1.DataOraSrc AND tblDir.TipoSensore = t1.TipoDir ")
            sql.AppendLine("	    	INNER JOIN @TBL_DATI tblVel ON tblVel.DataOraSrc = t1.DataOraSrc AND tblVel.TipoSensore = t1.TipoVel ")
            sql.AppendLine("	    ) t2 ")
            sql.AppendLine("	    GROUP BY DataOra, Id_SensoreDir, Id_SensoreVel ")
            sql.AppendLine("    ) t3 ")
            sql.AppendLine()
            sql.AppendLine("	INSERT INTO #DatiIoT ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='avg' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, MIN(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='min' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='max' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='sum' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	UNION --Ore di bagnatura ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, SUM( CASE WHEN Valore > 0 THEN 1 ELSE 0 END ) AS Valore FROM @TBL_DATI WHERE FunAggreg='bagnatura' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_SensoreDir AS Id_Sensore, MIN(Dir) AS Valore FROM @TBL_AGGR_VET GROUP BY DataOra, Id_SensoreDir ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_SensoreVel AS Id_Sensore, MIN(Vel) AS Valore FROM @TBL_AGGR_VET GROUP BY DataOra, Id_SensoreVel ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, Valore = SUM(IIF(0 > Tmp_Valore, 0, Tmp_Valore)) OVER(PARTITION BY Id_Sensore ORDER BY DataOra, Id_Sensore) ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, AVG(Valore) - @SOGLIA_TERMICA AS Tmp_Valore FROM @TBL_DATI WHERE FunAggreg='t_sum' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	) T ")
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine("	SELECT DataOra, Id_Sensore, Valore = SUM(Tmp_Valore) OVER(PARTITION BY Id_Sensore ORDER BY DataOra, Id_Sensore) ")
            sql.AppendLine("	FROM ( ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, Tmp_Valore = SUM( CASE WHEN Valore < @SOGLIA_FABBISOGNO_FREDDO THEN 1 ELSE 0 END ) FROM @TBL_DATI WHERE FunAggreg='t_cnt_freddo' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	) T ")
            sql.AppendLine("END ")
            sql.AppendLine("ELSE ")
            sql.AppendLine("BEGIN ")
            sql.AppendLine("	INSERT INTO #DatiIoT ")
            sql.AppendLine()
            sql.AppendLine("	SELECT DataOra, Id_Sensore, Valore ")
            sql.AppendLine("	FROM @TBL_DATI d ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI s ON s.Id = d.Id_Sensore ")
            sql.AppendLine("	WHERE s.FunAggreg <> 't_sum' ")
            sql.AppendLine()
            sql.AppendLine("	UNION ALL ")
            sql.AppendLine()
            sql.AppendLine("	SELECT DataOra, Id_Sensore, Valore = SUM(IIF(0 > Valore, 0, Valore)) OVER(PARTITION BY Id_Sensore ORDER BY DataOra, Id_Sensore) FROM ( ")
            sql.AppendLine("		SELECT DataOra, Id_Sensore, Valore = AVG(Valore) - @SOGLIA_TERMICA ")
            sql.AppendLine("		FROM @TBL_DATI d ")
            sql.AppendLine("		INNER JOIN @TBL_SENSORI s ON s.Id = d.Id_Sensore ")
            sql.AppendLine("		WHERE s.FunAggreg = 't_sum' ")
            sql.AppendLine("		GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("	) T ")
            sql.AppendLine("END ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DELETE FROM @TBL_SENSORI WHERE Id NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiIoT) ")
            sql.AppendLine()
            sql.AppendLine()
            sql.AppendLine("DECLARE @cols AS VARCHAR(MAX) ")
            sql.AppendLine("DECLARE @cols_name AS VARCHAR(MAX) ")
            sql.AppendLine()
            sql.AppendLine("SET @cols = STUFF((SELECT ', ' + QUOTENAME(Id) FROM @TBL_SENSORI WHERE FlagOutput = 1 ORDER BY Ordine FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
            sql.AppendLine("SET @cols_name = STUFF((SELECT ', ' + QUOTENAME(Id) + ' AS ' + QUOTENAME(NomeColonna) FROM @TBL_SENSORI WHERE FlagOutput = 1 ORDER BY Ordine FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
            sql.AppendLine()
            sql.AppendLine("--Se non esistono Dati @cols è NULL ")
            sql.AppendLine("IF @cols IS NOT NULL ")
            sql.AppendLine("BEGIN ")
            sql.AppendLine()
            sql.AppendLine("    --Tabella Dati ")
            sql.AppendLine("    EXECUTE('SELECT DataOra, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiIoT) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
            sql.AppendLine()
            sql.AppendLine("    --Anagrafica colonne")
            sql.AppendLine("    SELECT NomeColonna, Sensore = Etichetta, UM, TipoSensore = Id_TipoSensore, Misura = COALESCE(Misura, ''), FunAggreg, Configurazione = OutputConfig FROM @TBL_SENSORI WHERE FlagOutput = 1 ORDER BY Ordine ")
            sql.AppendLine()
            sql.AppendLine("    --Riepilogo ")
            sql.AppendLine("	SELECT S.OutputRiepilogo AS Sensore, S.FunAggreg, T.Valore, S.UM FROM ( ")
            sql.AppendLine("		SELECT Id_Sensore, AVG(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='avg' GROUP BY Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT Id_Sensore, MIN(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='min' GROUP BY Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT Id_Sensore, MAX(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='max' GROUP BY Id_Sensore ")
            sql.AppendLine("		UNION ")
            sql.AppendLine("		SELECT Id_Sensore, SUM(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='sum' GROUP BY Id_Sensore ")
            sql.AppendLine("	    UNION ")
            sql.AppendLine("		SELECT Id_Sensore, MAX(Valore) As Valore FROM ( ")
            sql.AppendLine("			SELECT DataOra, Id_Sensore, Valore = SUM(IIF(0 > Tmp_Valore, 0, Tmp_Valore)) OVER(PARTITION BY Id_Sensore ORDER BY DataOra, Id_Sensore) ")
            sql.AppendLine("			FROM ( ")
            sql.AppendLine("				SELECT DataOra, Id_Sensore, AVG(Valore) - @SOGLIA_TERMICA AS Tmp_Valore FROM @TBL_DATI ")
            sql.AppendLine("				WHERE FunAggreg='t_sum' GROUP BY DataOra, Id_Sensore ")
            sql.AppendLine("			) T ")
            sql.AppendLine("		) T GROUP BY Id_Sensore ")
            sql.AppendLine("	    UNION ")
            sql.AppendLine("		SELECT Id_Sensore, Valore = SUM( CASE WHEN Valore < @SOGLIA_FABBISOGNO_FREDDO THEN 1 ELSE 0 END ) FROM @TBL_DATI WHERE FunAggreg='t_cnt_freddo' GROUP BY Id_Sensore ")
            sql.AppendLine("	) T ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI S ON S.Id = T.Id_Sensore ")
            sql.AppendLine("    LEFT JOIN @TBL_SXS sxs ON sxs.Id_Sensore = S.Id ")
            sql.AppendLine("	WHERE sxs.Id_TipoSensore IS NULL OR sxs.Id_TipoSensore IN (1, 2) ")
            sql.AppendLine("	ORDER BY S.Id_TipoSensore, S.Id_Src, S.Ordine ")
            sql.AppendLine()
            sql.AppendLine("    --Riepilogo")
            sql.AppendLine("	; WITH Dati_CTE AS ( ")
            sql.AppendLine("		SELECT Id_Sensore, TipoSensore, T.FunAggreg, DataOra, val_last = Valore FROM ( ")
            sql.AppendLine("			SELECT Id_Sensore, TipoSensore, FunAggreg, DataOra, Valore, Idx = ROW_NUMBER() OVER (PARTITION BY Id_Sensore ORDER BY DataOra DESC) ")
            sql.AppendLine("			FROM @TBL_DATI D ")
            sql.AppendLine("		) T ")
            sql.AppendLine("		INNER JOIN @TBL_SENSORI S ON S.Id = T.Id_Sensore ")
            sql.AppendLine("		WHERE S.Id = S.Id_Src --Solo i sensori reali, non i derivati ")
            sql.AppendLine("		AND Idx = 1 --Ultimo valore in ordine cronologico ")
            sql.AppendLine("	) ")
            sql.AppendLine("	SELECT S.NomeColonna, S.Etichetta, S.UM, T1.DataOra, T1.val_last, T2.val_avg, T2.val_min, T2.val_max, T3.val_sum, T4.val_dir ")
            sql.AppendLine("    FROM Dati_CTE T1 ")
            sql.AppendLine("	LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore, val_avg = AVG(Valore), val_min = Min(Valore), val_max = MAX(Valore) FROM @TBL_DATI WHERE FunAggreg='avg' GROUP BY Id_Sensore ")
            sql.AppendLine("	) T2 ON T2.Id_Sensore = T1.Id_Sensore ")
            sql.AppendLine("	LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore, val_sum = SUM(Valore) FROM @TBL_DATI WHERE FunAggreg='sum' GROUP BY Id_Sensore ")
            sql.AppendLine("	) T3 ON T3.Id_Sensore = T1.Id_Sensore ")
            sql.AppendLine("	LEFT JOIN ( ")
            sql.AppendLine("		SELECT Id_Sensore = TT2.Id_SensoreVel, val_dir = Dati_CTE.val_last FROM ( ")
            sql.AppendLine("			SELECT DISTINCT Id_SensoreVel = TVel.Id_Src, Id_SensoreDir = TDir.Id_Src ")
            sql.AppendLine("			FROM ( ")
            sql.AppendLine("				SELECT TipoDir = CAST(REPLACE(REPLACE(FunAggreg, 'avgvett(', ''), ')', '') AS INT), TipoVel = Id_TipoSensore ")
            sql.AppendLine("				FROM @TBL_SENSORI ")
            sql.AppendLine("				WHERE FunAggreg LIKE 'avgvett(%)' ")
            sql.AppendLine("			) TT1 ")
            sql.AppendLine("			INNER JOIN @TBL_SENSORI TDir ON TDir.Id_TipoSensore = TT1.TipoDir ")
            sql.AppendLine("			INNER JOIN @TBL_SENSORI TVel ON TVel.Id_TipoSensore = TT1.TipoVel ")
            sql.AppendLine("			INNER JOIN IOT_DispositiviXSensori sxs1 ON sxs1.Id_Sensore = TDir.Id_Src AND sxs1.FlagReale = 1 ")
            sql.AppendLine("			INNER JOIN IOT_DispositiviXSensori sxs2 ON sxs2.Id_Sensore = TVel.Id_Src AND sxs2.FlagReale = 1 AND sxs1.Id_dispositivo = sxs2.Id_dispositivo ")
            sql.AppendLine("		) TT2 ")
            sql.AppendLine("		INNER JOIN Dati_CTE ON Dati_CTE.Id_Sensore = TT2.Id_SensoreDir ")
            sql.AppendLine("	) T4 ON T4.Id_Sensore = T1.Id_Sensore ")
            sql.AppendLine("	INNER JOIN @TBL_SENSORI S ON S.Id = T1.Id_Sensore ")
            sql.AppendLine("	WHERE T1.FunAggreg <> '' ")
            sql.AppendLine()
            sql.AppendLine("END ")
            sql.AppendLine()
            sql.Append(DropTableDati("DatiIoT"))

            Dim DS As New DataSet

            EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine, DS, "Dati_IOT")

            Return DS

        Catch ex As Exception

            Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
        End Try

        Return Nothing
    End Function

    'Public Function LeggiDatiPerDSS(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer,
    '                                SogliaBagnaturaPerc As Decimal, SogliaBagnaturaMin As Decimal,
    '                                DataInizio As DateTime, DataFine As DateTime,
    '                                SogliaDistanza_mt As Integer,
    '                                ObjParametri_Server As AgronicaCoreParametri) As DataSet

    '    Dim NomeRoutine As String = "MeteoNT.LeggiDatiPerDSS"

    '    Try

    '        Dim sql As New StringBuilder

    '        sql.Clear()
    '        sql.AppendLine("SET NOCOUNT ON ")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @ID_STAZIONE INT = " & CStr(IdStazione))
    '        sql.AppendLine("DECLARE @ID_PUBBLICA INT = -1")
    '        sql.AppendLine("DECLARE @ID_RER INT = -1")
    '        sql.AppendLine("DECLARE @DATAINIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizio))
    '        sql.AppendLine("DECLARE @DATAFINE DATETIME = " & Agro_SQL_SaveDateTime(DataFine))
    '        sql.AppendLine()

    '        'Tipi sensori in tabella
    '        '1	- Temperatura aria
    '        '2	- Pluviometro
    '        '3	- Umidità relativa
    '        '4	- Bagnatura fogliare superiore
    '        '6	- Bagnatura fogliare superiore (%)
    '        '22 - Bagnatura fogliare (min) 
    '        '35 - Bagnatura fogliare (h) 
    '        '42 - Bagnatura fogliare (0-1) 

    '        sql.AppendLine("DECLARE @MAP_TIPI TABLE(Id_Tipo INT, Des_Tipo VARCHAR(10), Soglia_Bagn REAL) ")
    '        sql.AppendLine("INSERT INTO @MAP_TIPI VALUES ")
    '        sql.AppendLine("(1, 'Temp', 0), ")
    '        sql.AppendLine("(2, 'Prec', 0), ")
    '        sql.AppendLine("(3, 'UmRel', 0), ")
    '        sql.AppendLine("(-4, 'Bagn', " & Convert.ToString(SogliaBagnaturaPerc, Globalization.CultureInfo.InvariantCulture) & "), ")
    '        sql.AppendLine("(-6, 'Bagn', " & Convert.ToString(SogliaBagnaturaPerc, Globalization.CultureInfo.InvariantCulture) & "), ")
    '        sql.AppendLine("(-22, 'Bagn', " & Convert.ToString(SogliaBagnaturaMin, Globalization.CultureInfo.InvariantCulture) & "), ")
    '        sql.AppendLine("(-35, 'Bagn', 0), ")
    '        sql.AppendLine("(-42, 'Bagn', 0) ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine()

    '        Select Case TipoSorgente

    '            Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

    '                Dim table As String = "Dati_Meteo_QHH"
    '                Dim column As String = "ID_Quadrante"

    '                If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER Then

    '                    table = "Dati_Meteo_SHH"
    '                    column = "ID_Stazione"
    '                End If

    '                'Tenere in considerazione MapSensori per i nomi della colonna???
    '                sql.AppendLine("SELECT DataOra = Tempo, Temp = Temp_Media, Prec = Precipitazione, UmRel = UmiditaRelativa, Bagn = Bagnatura ")
    '                sql.AppendLine("FROM " & table)
    '                sql.AppendLine("WHERE " & column & " = @ID_STAZIONE AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE ")
    '                sql.AppendLine()
    '                sql.AppendLine("SELECT DISTINCT ColName = Des_Tipo FROM @MAP_TIPI ")

    '            Case enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Pubbliche

    '                sql.AppendLine("DECLARE @TIPO_BAGN INT = ABS((SELECT MAX(Id_Tipo) FROM @MAP_TIPI WHERE Id_Tipo < 0)) ")
    '                sql.AppendLine()
    '                sql.AppendLine()
    '                sql.AppendLine()

    '                If TipoSorgente <> enum_Meteo_Tiposorgente.Pubbliche Then

    '                    sql.AppendLine("DECLARE @LAT AS REAL, @LNG AS REAL ")
    '                    sql.AppendLine("SELECT @LAT = Lat_Dec, @LNG = Lng_Dec FROM MeteoNT_Stazioni WHERE Id = @ID_STAZIONE ")
    '                    sql.AppendLine()
    '                    sql.AppendLine("IF @LAT IS NOT NULL AND @LNG IS NOT NULL ")
    '                    sql.AppendLine("BEGIN ")
    '                    sql.AppendLine()
    '                    sql.AppendLine("	DECLARE @SRC_PT GEOGRAPHY = GEOGRAPHY::Point(@LAT, @LNG, 4326)")
    '                    sql.AppendLine("	DECLARE @SOGLIA_DISTANZA REAL = " & SogliaDistanza_mt)
    '                    sql.AppendLine()
    '                    sql.AppendLine("	SELECT @ID_PUBBLICA = Id FROM ( ")
    '                    sql.AppendLine("		SELECT TOP 1 Id, Distanza = @SRC_PT.STDistance(GeoEntity) FROM MeteoNT_Stazioni ")
    '                    sql.AppendLine("		WHERE Categoria = 1 ")
    '                    sql.AppendLine("		ORDER BY Distanza ")
    '                    sql.AppendLine("	) T WHERE Distanza < @SOGLIA_DISTANZA ")
    '                    sql.AppendLine()
    '                    sql.AppendLine("	SELECT @ID_RER = Id FROM ( ")
    '                    sql.AppendLine("		SELECT TOP 1 Id, Distanza = @SRC_PT.STDistance(GEOGRAPHY::Point(T1.Lat, T1.Lng, 4326)) ")
    '                    sql.AppendLine("		FROM ( ")
    '                    sql.AppendLine("			SELECT Id = ID_Quadrante, Lat = replace(replace(X_LON_GC, 'E', ''), ',', '.'), Lng = replace(replace(Y_LAT_GC, 'N', ''), ',', '.') FROM TB_Quadranti ")
    '                    sql.AppendLine("		) T1 ")
    '                    sql.AppendLine("		WHERE Lat IS NOT NULL AND Lng IS NOT NULL ")
    '                    sql.AppendLine("		ORDER BY Distanza ")
    '                    sql.AppendLine("	) T2 WHERE Distanza <= @SOGLIA_DISTANZA ")
    '                    sql.AppendLine()
    '                    sql.AppendLine("END ")
    '                    sql.AppendLine()
    '                    sql.AppendLine()
    '                    sql.AppendLine()
    '                End If

    '                sql.AppendLine("DECLARE @TMP_TBL TABLE(DataOra DATETIME, Id_TipoSensore INT, Valore REAL, Ordine INT) ")
    '                sql.AppendLine()
    '                sql.AppendLine("INSERT INTO @TMP_TBL ")
    '                sql.AppendLine("SELECT ")
    '                sql.AppendLine("	DataOra ")
    '                sql.AppendLine("	, Id_TipoSensore = CASE WHEN Id_TipoSensore > 0 THEN Id_TipoSensore ELSE @TIPO_BAGN END ")
    '                sql.AppendLine("	, Valore = CASE WHEN Id_TipoSensore > 0 THEN Valore ELSE CASE WHEN Valore > SogliaBagn THEN 1 ELSE 0 END END ")
    '                sql.AppendLine("	, Ordine ")
    '                sql.AppendLine("FROM MeteoNT_DatiOrari dati ")
    '                sql.AppendLine("INNER JOIN ( ")
    '                sql.AppendLine()
    '                sql.AppendLine("	SELECT ")
    '                sql.AppendLine("		sens.Id ")
    '                sql.AppendLine("		, Id_TipoSensore = map.Id_Tipo ")
    '                sql.AppendLine("		, Ordine = CASE WHEN sxs.Id_Stazione = @ID_STAZIONE THEN sxs.Ordine ELSE 1000 + sxs.Ordine END ")
    '                sql.AppendLine("		, SogliaBagn = map.Soglia_Bagn ")
    '                sql.AppendLine("	FROM MeteoNT_Sensori sens ")
    '                sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs ON sxs.Id_Sensore = sens.Id ")
    '                sql.AppendLine("	INNER JOIN @MAP_TIPI map ON ABS(map.Id_Tipo) = sens.Id_TipoSensore ")
    '                sql.AppendLine("	WHERE (sxs.Id_Stazione = @ID_STAZIONE OR sxs.Id_Stazione = @ID_PUBBLICA) ")
    '                sql.AppendLine()
    '                sql.AppendLine(") sens ON sens.Id = dati.Id_Sensore ")
    '                sql.AppendLine("WHERE @DATAINIZIO <= DataOra AND DataOra <= @DATAFINE ")
    '                sql.AppendLine()
    '                sql.AppendLine()
    '                sql.AppendLine("; WITH DATI_RER AS ( ")
    '                sql.AppendLine("	SELECT * FROM Dati_Meteo_QHH WHERE ID_Quadrante = @ID_RER AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE ")
    '                sql.AppendLine(") ")
    '                sql.AppendLine()
    '                sql.AppendLine("INSERT INTO @TMP_TBL ")
    '                sql.AppendLine("SELECT Tempo, Id_Sensore, Valore, Ordine = 1000000 + Id_Sensore FROM ( ")
    '                sql.AppendLine("	SELECT Tempo, Id_Sensore = 1, Valore = Temp_Media FROM DATI_RER ")
    '                sql.AppendLine("	UNION ALL ")
    '                sql.AppendLine("	SELECT Tempo, Id_Sensore = 2, Valore = Precipitazione FROM DATI_RER ")
    '                sql.AppendLine("	UNION ALL ")
    '                sql.AppendLine("	SELECT Tempo, Id_sensore = 3, Valore = UmiditaRelativa FROM DATI_RER ")
    '                sql.AppendLine("	UNION ALL ")
    '                sql.AppendLine("	SELECT Tempo, Id_Sensore = @TIPO_BAGN, Valore = Bagnatura FROM DATI_RER ")
    '                sql.AppendLine(") T ")
    '                sql.AppendLine()

    '                sql.Append(CreateTableDati("DatiMeteo"))

    '                sql.AppendLine("INSERT INTO #DatiMeteo ")
    '                sql.AppendLine("SELECT DataOra, Id_Sensore = Id_TipoSensore, Valore FROM ")
    '                sql.AppendLine("( ")
    '                sql.AppendLine("	SELECT DataOra, Id_TipoSensore, Valore, RowNum = ROW_NUMBER() OVER (PARTITION BY DataOra, Id_TipoSensore ORDER BY DataOra, Id_TipoSensore, Ordine) ")
    '                sql.AppendLine("	FROM @TMP_TBL ")
    '                sql.AppendLine(") T ")
    '                sql.AppendLine("WHERE RowNum = 1 ")
    '                sql.AppendLine("ORDER BY DataOra, Id_TipoSensore ")
    '                sql.AppendLine()
    '                sql.AppendLine()
    '                sql.AppendLine()
    '                sql.AppendLine("DECLARE @SENSORI TABLE(Id_Sensore INT, ColName VARCHAR(MAX)) ")
    '                sql.AppendLine("INSERT INTO @SENSORI ")
    '                sql.AppendLine("SELECT Id_Sensore, ColName FROM ( ")
    '                sql.AppendLine("    SELECT dm.Id_Sensore, ColName = map.Des_Tipo ")
    '                sql.AppendLine("    FROM (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) dm ")
    '                sql.AppendLine("    INNER JOIN @MAP_TIPI map ON ABS(map.Id_Tipo) = dm.Id_Sensore ")
    '                sql.AppendLine(") S ")
    '                sql.AppendLine()
    '                sql.AppendLine()
    '                sql.AppendLine()
    '                sql.AppendLine("DECLARE @cols AS VARCHAR(MAX) ")
    '                sql.AppendLine("DECLARE @cols_name AS VARCHAR(MAX) ")
    '                sql.AppendLine()
    '                sql.AppendLine("SET @cols = STUFF((SELECT ', ' + QUOTENAME(Id_Sensore) FROM @SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
    '                sql.AppendLine("SET @cols_name = STUFF((SELECT ', ' + ColName + ' = ' + QUOTENAME(Id_Sensore) FROM @SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
    '                sql.AppendLine()
    '                sql.AppendLine("IF @cols IS NOT NULL ")
    '                sql.AppendLine("BEGIN ")
    '                sql.AppendLine()
    '                sql.AppendLine("    EXECUTE('SELECT DataOra, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
    '                sql.AppendLine()
    '                sql.AppendLine("    SELECT DISTINCT ColName FROM @SENSORI ")
    '                sql.AppendLine()
    '                sql.AppendLine("    SELECT DISTINCT T.Des_Tipo FROM @MAP_TIPI T LEFT JOIN @SENSORI S ON S.ColName = T.Des_Tipo WHERE S.ColName IS NULL ")
    '                sql.AppendLine()
    '                sql.AppendLine("END ")
    '                sql.AppendLine()

    '                sql.Append(DropTableDati("DatiMeteo"))

    '        End Select

    '        Dim DS As New DataSet

    '        EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine, DS, "DatiMeteo_NT")

    '        Return DS

    '    Catch ex As Exception

    '        Throw New Exception("[" & NomeRoutine & "]:" & ex.Message & " ")
    '    End Try

    '    Return Nothing
    'End Function


    'Public Class InfoPerDSS
    '    Public Lat As Decimal
    '    Public Lng As Decimal
    '    Public TimeZone As String
    'End Class

    'Public Function LeggiInfoPerDSS(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer, ObjParametri_Server As AgronicaCoreParametri) As InfoPerDSS

    '    Dim NomeRoutine As String = "MeteoNT.LeggiInfoPerDSS"

    '    Dim lat As Decimal? = Nothing
    '    Dim lng As Decimal? = Nothing
    '    Dim timezone As String = ""

    '    Try

    '        Dim sql As New StringBuilder

    '        sql.Clear()

    '        sql.AppendLine("DECLARE @ID_STAZIONE AS INTEGER = " & IdStazione.ToString)
    '        sql.AppendLine()

    '        Select Case TipoSorgente

    '            Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

    '                sql.AppendLine("SELECT ")
    '                sql.AppendLine("	A.Id_Src ")
    '                sql.AppendLine("	, A.Id_Reale ")
    '                sql.AppendLine("	, Lat_Dec = COALESCE(z1.Lat_Dec, z2.Lat_Dec) ")
    '                sql.AppendLine("	, Lng_Dec = COALESCE(z1.Lng_Dec, z2.Lng_Dec) ")
    '                sql.AppendLine("	, SystemTimeZone = COALESCE(z1.SystemTimeZoneInfo, z2.SystemTimeZoneInfo, 'W. Europe Standard Time') ")
    '                sql.AppendLine("FROM ( ")
    '                sql.AppendLine("	SELECT DISTINCT	Id_Src = z1.Id, Id_Reale = z2.Id ")
    '                sql.AppendLine("	FROM MeteoNT_Stazioni z1 ")
    '                sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs1 ON sxs1.Id_Stazione = z1.Id ")
    '                sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs2 ON sxs2.Id_Sensore = sxs1.Id_Sensore AND sxs2.FlagReale = 1 ")
    '                sql.AppendLine("	INNER JOIN MeteoNT_Stazioni z2 ON z2.Id = sxs2.Id_Stazione ")
    '                sql.AppendLine("	WHERE z1.Id = @ID_STAZIONE ")
    '                sql.AppendLine(") A ")
    '                sql.AppendLine("INNER JOIN MeteoNT_Stazioni z1 ON z1.Id = A.Id_Src ")
    '                sql.AppendLine("INNER JOIN MeteoNT_Stazioni z2 ON z2.Id = A.Id_Reale ")

    '            Case enum_Meteo_Tiposorgente.Gias_RER

    '                sql.AppendLine("SELECT ")
    '                sql.AppendLine("	Id_Src = S.ID_Stazione ")
    '                sql.AppendLine("	, Id_Reale = S.ID_Stazione ")
    '                sql.AppendLine("	, Lat_Dec = REPLACE(REPLACE(Q.X_LON_GC, 'E', ''), ',', '.') ")
    '                sql.AppendLine("	, Lng_Dec = REPLACE(REPLACE(Q.Y_LAT_GC, 'N', ''), ',', '.') ")
    '                sql.AppendLine("	, SystemTimeZone = 'W. Europe Standard Time' ")
    '                sql.AppendLine("FROM TB_Stazioni S ")
    '                sql.AppendLine("INNER JOIN TB_QuadrantixStazioni QS ON S.ID_Stazione = QS.ID_Stazione ")
    '                sql.AppendLine("INNER JOIN TB_Quadranti Q ON Q.ID_Quadrante = QS.ID_Quadrante ")
    '                sql.AppendLine("WHERE S.ID_Stazione = @ID_STAZIONE ")

    '            Case enum_Meteo_Tiposorgente.Gias_RER_Quadranti

    '                sql.AppendLine("SELECT ")
    '                sql.AppendLine("	Id_Src = Q.ID_Quadrante ")
    '                sql.AppendLine("	, Id_Reale = Q.ID_Quadrante ")
    '                sql.AppendLine("	, Lat_Dec = REPLACE(REPLACE(Q.X_LON_GC, 'E', ''), ',', '.') ")
    '                sql.AppendLine("	, Lng_Dec = REPLACE(REPLACE(Q.Y_LAT_GC, 'N', ''), ',', '.') ")
    '                sql.AppendLine("	, SystemTimeZone = 'W. Europe Standard Time' ")
    '                sql.AppendLine("FROM TB_Quadranti Q ")
    '                sql.AppendLine("WHERE Q.ID_Quadrante = @ID_STAZIONE ")

    '        End Select

    '        Dim DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '        If DT IsNot Nothing AndAlso DT.Rows.Count > 0 Then

    '            Dim r As Integer = 0
    '            While r < DT.Rows.Count AndAlso Not (lat.HasValue AndAlso lng.HasValue)

    '                Dim row = DT.Rows(r)

    '                If Not IsDBNull(row("Lat_Dec")) AndAlso Not IsDBNull(row("Lng_Dec")) Then

    '                    lat = Convert.ToDecimal(row("Lat_Dec"), Globalization.CultureInfo.InvariantCulture)
    '                    lng = Convert.ToDecimal(row("Lng_Dec"), Globalization.CultureInfo.InvariantCulture)
    '                    timezone = row("SystemTimeZone")
    '                End If

    '                r += 1
    '            End While

    '        End If

    '    Catch ex As Exception

    '        lat = Nothing
    '        lng = Nothing
    '        timezone = ""

    '    End Try

    '    If Not (lat.HasValue AndAlso lng.HasValue) Then
    '        lat = 44.168704206487483
    '        lng = 12.268798020116076
    '    End If

    '    If String.IsNullOrEmpty(timezone) Then
    '        timezone = "W. Europe Standard Time"
    '    End If

    '    Return New InfoPerDSS With {
    '        .Lat = lat.Value,
    '        .Lng = lng.Value,
    '        .TimeZone = timezone
    '    }
    'End Function

    'Public Function LeggiDatiPerPiogge(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer,
    '                                   DataInizio As DateTime, DataFine As DateTime,
    '                                   ObjParametri_Server As AgronicaCoreParametri) As DataTable

    '    Dim NomeRoutine As String = "MeteoNT.LeggiDatiPerPiogge"
    '    Dim DT As DataTable

    '    Try

    '        Dim sql As New StringBuilder

    '        sql.Clear()
    '        sql.AppendLine("SET NOCOUNT ON ")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @STAZIONE INT = " & CStr(IdStazione))
    '        sql.AppendLine("DECLARE @DATAINIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizio))
    '        sql.AppendLine("DECLARE @DATAFINE DATETIME = " & Agro_SQL_SaveDateTime(DataFine))
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @MAP_SENSORI TABLE(Id_Sensore INT, Id_TipoSensore INT, FunAggreg VARCHAR(10), ColName VARCHAR(50)) ")
    '        sql.AppendLine("INSERT INTO @MAP_SENSORI VALUES ")
    '        sql.AppendLine("(NULL, 1, NULL, 'Temp'), ")
    '        sql.AppendLine("(NULL, 2, NULL, 'Prec'), ")
    '        sql.AppendLine("(NULL, 3, NULL, 'UmRel'), ")
    '        sql.AppendLine("(-1, 1, 'min', 'TempMin'), ")
    '        sql.AppendLine("(-2, 1, 'max', 'TempMax') ")
    '        sql.AppendLine()

    '        Select Case TipoSorgente

    '            Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

    '                sql.AppendLine("UPDATE s ")
    '                sql.AppendLine("SET s.Id_Sensore  = s.Id_TipoSensore ")
    '                sql.AppendLine("FROM @MAP_SENSORI s ")
    '                sql.AppendLine("WHERE s.Id_Sensore IS NULL ")

    '            Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

    '                sql.AppendLine("UPDATE s ")
    '                sql.AppendLine("SET s.Id_Sensore = ss.Id_Sensore ")
    '                sql.AppendLine("FROM @MAP_SENSORI s ")
    '                sql.AppendLine("INNER JOIN ( ")
    '                sql.AppendLine("	SELECT Id_Sensore, Id_TipoSensore ")
    '                sql.AppendLine("	FROM MeteoNT_Sensori s ")
    '                sql.AppendLine("	INNER JOIN MeteoNT_StazioniXSensori sxs ON sxs.Id_Sensore = s.Id ")
    '                sql.AppendLine("	WHERE sxs.Id_Stazione = @STAZIONE ")
    '                sql.AppendLine(") ss ON ss.Id_TipoSensore = s.Id_TipoSensore AND s.Id_Sensore IS NULL ")

    '        End Select

    '        sql.AppendLine()
    '        sql.AppendLine("UPDATE s ")
    '        sql.AppendLine("SET s.FunAggreg = t.FunAggreg ")
    '        sql.AppendLine("FROM @MAP_SENSORI s ")
    '        sql.AppendLine("INNER JOIN MeteoNT_TipiSensore t ON t.Id = s.Id_TipoSensore ")
    '        sql.AppendLine("WHERE s.FunAggreg IS NULL ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @TMPTABLE TABLE(DataOra DATETIME, Id_Sensore INT, Valore REAL, TipoSensore INT, FunAggreg VARCHAR(50)) ")
    '        sql.AppendLine()

    '        Select Case TipoSorgente

    '            Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

    '                Dim Tabella_RER As String = "Dati_Meteo_SHH"
    '                Dim ID_Stazione As String = "ID_Stazione"

    '                If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

    '                    Tabella_RER = "Dati_Meteo_QHH"
    '                    ID_Stazione = "ID_Quadrante"
    '                End If

    '                sql.AppendLine("; WITH TMPDATI_CTE AS ( ")
    '                sql.AppendLine("	SELECT DataOra = DATEADD(dd, 0, DATEDIFF(dd, 0, Tempo)), Temp_Media, Precipitazione, UmiditaRelativa ")
    '                sql.AppendLine("	FROM " & Tabella_RER)
    '                sql.AppendLine("	WHERE " & ID_Stazione & " = @STAZIONE And @DATAINIZIO <= Tempo And Tempo <= @DATAFINE ")
    '                sql.AppendLine("), ")
    '                sql.AppendLine("DATI_CTE AS ( ")
    '                sql.AppendLine("	SELECT DataOra, Id_Sensore = 1, Valore = Temp_Media FROM TMPDATI_CTE ")
    '                sql.AppendLine("	UNION ALL ")
    '                sql.AppendLine("	SELECT DataOra, Id_Sensore = 2, Valore = Precipitazione FROM TMPDATI_CTE ")
    '                sql.AppendLine("	UNION ALL ")
    '                sql.AppendLine("	SELECT DataOra, Id_Sensore = 3, Valore = UmiditaRelativa FROM TMPDATI_CTE ")
    '                sql.AppendLine(") ")

    '            Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

    '                sql.AppendLine("; WITH DATI_CTE AS ( ")
    '                sql.AppendLine("	SELECT DataOra = DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra)), d.Id_Sensore, Valore ")
    '                sql.AppendLine("	FROM MeteoNT_DatiOrari d ")
    '                sql.AppendLine("	INNER JOIN @MAP_SENSORI s ON s.Id_Sensore = d.Id_Sensore ")
    '                sql.AppendLine("	WHERE @DATAINIZIO <= DataOra AND DataOra <= @DATAFINE ")
    '                sql.AppendLine(") ")

    '        End Select

    '        sql.AppendLine()
    '        sql.AppendLine("INSERT INTO @TMPTABLE ")
    '        sql.AppendLine("SELECT DataOra, d.Id_Sensore, Valore, Id_TipoSensore, FunAggreg ")
    '        sql.AppendLine("FROM DATI_CTE d ")
    '        sql.AppendLine("INNER JOIN @MAP_SENSORI s ON s.Id_Sensore = d.Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine("UNION ALL ")
    '        sql.AppendLine()
    '        sql.AppendLine("SELECT DataOra, s2.Id_Sensore, Valore, s2.Id_TipoSensore, s2.FunAggreg ")
    '        sql.AppendLine("FROM DATI_CTE d ")
    '        sql.AppendLine("INNER JOIN @MAP_SENSORI s1 ON s1.Id_Sensore = d.Id_Sensore ")
    '        sql.AppendLine("INNER JOIN @MAP_SENSORI s2 ON s2.Id_TipoSensore = s1.Id_TipoSensore AND S2.Id_Sensore < 0 ")
    '        sql.AppendLine()
    '        sql.Append(CreateTableDati("DatiMeteo"))

    '        sql.AppendLine("INSERT INTO #DatiMeteo ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='avg' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ALL ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='sum' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ALL ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, MIN(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='min' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ALL ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore FROM @TMPTABLE WHERE FunAggreg='max' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @SENSORI TABLE(Id_Sensore INT, ColName VARCHAR(MAX)) ")
    '        sql.AppendLine("INSERT INTO @SENSORI ")
    '        sql.AppendLine("SELECT s.Id_sensore, map.ColName ")
    '        sql.AppendLine("FROM (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) s ")
    '        sql.AppendLine("INNER JOIN @MAP_SENSORI map ON map.Id_Sensore = s.Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @cols AS VARCHAR(MAX) ")
    '        sql.AppendLine("DECLARE @cols_name AS VARCHAR(MAX) ")
    '        sql.AppendLine()
    '        sql.AppendLine("SET @cols = STUFF((SELECT ', ' + QUOTENAME(Id_Sensore) FROM @SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
    '        sql.AppendLine("SET @cols_name = STUFF((SELECT ', ' + QUOTENAME(Id_Sensore) + ' AS ' + ColName FROM @SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
    '        sql.AppendLine()
    '        sql.AppendLine("--Se non esistono Dati @cols è NULL ")
    '        sql.AppendLine("IF @cols IS NOT NULL ")
    '        sql.AppendLine("BEGIN ")
    '        sql.AppendLine()
    '        sql.AppendLine("    EXECUTE('SELECT DataOra, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
    '        sql.AppendLine()
    '        sql.AppendLine("END ")
    '        sql.AppendLine()
    '        sql.Append(DropTableDati("DatiMeteo"))

    '        DT = EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine)

    '    Catch ex As Exception

    '        Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
    '    End Try

    '    Return DT
    'End Function

    'Public Function LeggiDatiPerIrrigazione(TipoSorgente As enum_Meteo_Tiposorgente, IdStazione As Integer,
    '                                        DataInizio As DateTime, DataFine As DateTime,
    '                                        ObjParametri_Server As AgronicaCoreParametri) As DataSet

    '    Dim NomeRoutine As String = "MeteoNT.LeggiDatiPerIrrigazione"
    '    Dim sql As New StringBuilder
    '    Dim DS As New DataSet

    '    Try

    '        sql.Clear()

    '        sql.AppendLine("SET NOCOUNT ON ")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @STAZIONE INT = " & CStr(IdStazione))
    '        sql.AppendLine("DECLARE @DATAINIZIO DATETIME = " & Agro_SQL_SaveDateTime(DataInizio))
    '        sql.AppendLine("DECLARE @DATAFINE DATETIME = " & Agro_SQL_SaveDateTime(DataFine))
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @TBL_ALIAS TABLE(Id INT, Id_TipoSensoreSrc INT, Alias VARCHAR(50), FunAggreg VARCHAR(50)) ")
    '        sql.AppendLine("INSERT INTO @TBL_ALIAS VALUES ")
    '        sql.AppendLine("(NULL, 1, 'Tmed', NULL), ")
    '        sql.AppendLine("(NULL, 2, 'RainMM', NULL), ")
    '        sql.AppendLine("(NULL, 15, NULL, NULL), ")
    '        sql.AppendLine("(-1, 1, 'Tmin', 'min'), ")
    '        sql.AppendLine("(-2, 1, 'Tmax', 'max'), ")
    '        sql.AppendLine("(-3, 2, 'RainH', 'cnt_gt_0') ")
    '        sql.AppendLine()
    '        sql.AppendLine("UPDATE @TBL_ALIAS SET FunAggreg = (SELECT FunAggreg FROM MeteoNT_TipiSensore WHERE Id = Id_TipoSensoreSrc) ")
    '        sql.AppendLine("WHERE FunAggreg IS NULL ")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @TMP_Sensori TABLE(Id INT, Id_TipoSensore INT, Etichetta VARCHAR(MAX)) ")

    '        Select Case TipoSorgente

    '            Case enum_Meteo_Tiposorgente.RetiPartner, enum_Meteo_Tiposorgente.Aziendali, enum_Meteo_Tiposorgente.Pubbliche

    '                sql.AppendLine("INSERT INTO @TMP_Sensori ")
    '                sql.AppendLine("SELECT Id, Id_TipoSensore, Etichetta = COALESCE(sxs.Etichetta, sens.Etichetta) ")
    '                sql.AppendLine("FROM MeteoNT_Sensori sens ")
    '                sql.AppendLine("INNER JOIN MeteoNT_StazioniXSensori sxs ON sxs.Id_Sensore = sens.Id ")
    '                sql.AppendLine("WHERE sxs.Id_Stazione = @STAZIONE ")

    '            Case enum_Meteo_Tiposorgente.Gias_RER, enum_Meteo_Tiposorgente.Gias_RER_Quadranti

    '                sql.AppendLine("INSERT INTO @TMP_Sensori VALUES ")
    '                sql.AppendLine("(1, 1, 'Temperatura aria'), ")
    '                sql.AppendLine("--(2, 3, 'Umidità relativa'), ")
    '                sql.AppendLine("--(3, 4, 'Bagnatura fogliare'), ")
    '                sql.AppendLine("(4, 2, 'Pioggia') ")

    '        End Select

    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @TBL_SENSORI TABLE(Id INT, Id_Src INT, Etichetta VARCHAR(MAX), FunAggreg VARCHAR(10)) ")
    '        sql.AppendLine("INSERT INTO @TBL_SENSORI ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("	COALESCE(alias.Id, sens.Id) AS Id ")
    '        sql.AppendLine("	, sens.Id AS Id_Src ")
    '        sql.AppendLine("	, COALESCE(alias.Alias, 'UT_' + CAST(sens.Id AS VARCHAR(10))) AS Etichetta --sens.Etichetta) AS Etichetta ")
    '        sql.AppendLine("	, alias.FunAggreg ")
    '        sql.AppendLine("FROM @TMP_Sensori sens ")
    '        sql.AppendLine("INNER JOIN @TBL_ALIAS alias on alias.Id_TipoSensoreSrc = sens.Id_TipoSensore ")
    '        sql.AppendLine()

    '        Dim tblDati As String = "MeteoNT_DatiOrari"

    '        If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER OrElse TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then

    '            Dim Tabella_RER As String = "Dati_Meteo_SHH"
    '            Dim WhereClause As String = "ID_Stazione"

    '            If TipoSorgente = enum_Meteo_Tiposorgente.Gias_RER_Quadranti Then
    '                Tabella_RER = "Dati_Meteo_QHH"
    '                WhereClause = "ID_Quadrante"
    '            End If

    '            WhereClause = "WHERE " & WhereClause & " = @STAZIONE AND @DATAINIZIO <= Tempo AND Tempo <= @DATAFINE "

    '            sql.AppendLine("DECLARE @TMP_DATI TABLE(Id_Sensore INT, DataOra DATETIME, Valore REAL); ")
    '            sql.AppendLine("WITH DATI_CTE AS ( ")
    '            sql.AppendLine("    SELECT Tempo, Temp_Media, Precipitazione FROM " & Tabella_RER & " " & WhereClause)
    '            sql.AppendLine(") ")
    '            sql.AppendLine("INSERT INTO @TMP_DATI ")
    '            sql.AppendLine("SELECT 1, Tempo, Temp_Media FROM DATI_CTE ")
    '            sql.AppendLine("--UNION ")
    '            sql.AppendLine("--SELECT 2, Tempo, UmiditaRelativa FROM DATI_CTE ")
    '            sql.AppendLine("--UNION ")
    '            sql.AppendLine("--SELECT 3, Tempo, Bagnatura FROM DATI_CTE ")
    '            sql.AppendLine("UNION ")
    '            sql.AppendLine("SELECT 4, Tempo, Precipitazione FROM DATI_CTE ")
    '            sql.AppendLine()

    '            tblDati = "@TMP_DATI"
    '        End If

    '        sql.AppendLine("DECLARE @TBL_DATI TABLE(DataOra DATETIME, Id_Sensore INT, Valore REAL, FunAggreg VARCHAR(50)) ")
    '        sql.AppendLine("INSERT INTO @TBL_DATI ")
    '        sql.AppendLine("SELECT ")
    '        sql.AppendLine("	DataOra = DATEADD(dd, 0, DATEDIFF(dd, 0, DataOra)) ")
    '        sql.AppendLine("    , Id_Sensore = sens.Id ")
    '        sql.AppendLine("    , Valore ")
    '        sql.AppendLine("    , FunAggreg ")
    '        sql.AppendLine("FROM " & tblDati & " dati ")
    '        sql.AppendLine("INNER JOIN @TBL_SENSORI sens ON sens.Id_Src = dati.Id_Sensore ")

    '        If TipoSorgente = enum_Meteo_Tiposorgente.Aziendali OrElse
    '            TipoSorgente = enum_Meteo_Tiposorgente.RetiPartner OrElse
    '            TipoSorgente = enum_Meteo_Tiposorgente.Pubbliche Then

    '            sql.AppendLine("WHERE @DATAINIZIO <= DataOra AND DataOra <= @DATAFINE ")
    '        End If

    '        sql.AppendLine()
    '        sql.Append(CreateTableDati("DatiMeteo"))

    '        sql.AppendLine("INSERT INTO #DatiMeteo ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, AVG(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='avg' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, MIN(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='min' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, MAX(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='max' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, SUM(Valore) AS Valore FROM @TBL_DATI WHERE FunAggreg='sum' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine("UNION ")
    '        sql.AppendLine("SELECT DataOra, Id_Sensore, COUNT(CASE WHEN Valore > 0 THEN 1 END) AS Valore FROM @TBL_DATI WHERE FunAggreg='cnt_gt_0' GROUP BY DataOra, Id_Sensore ")
    '        sql.AppendLine()
    '        sql.AppendLine("DELETE FROM @TBL_SENSORI WHERE Id NOT IN (SELECT DISTINCT Id_Sensore FROM #DatiMeteo) ")
    '        sql.AppendLine()
    '        sql.AppendLine("DECLARE @cols AS VARCHAR(MAX) ")
    '        sql.AppendLine("DECLARE @cols_name AS VARCHAR(MAX) ")
    '        sql.AppendLine()
    '        sql.AppendLine("SET @cols = STUFF((SELECT ', ' + QUOTENAME(Id) FROM @TBL_SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
    '        sql.AppendLine("SET @cols_name = STUFF((SELECT ', ' + QUOTENAME(Id) + ' AS ' + QUOTENAME(Etichetta) FROM @TBL_SENSORI FOR XML PATH(''), TYPE).value('.', 'VARCHAR(MAX)'), 1, 1, '') ")
    '        sql.AppendLine()
    '        sql.AppendLine("--Se non esistono Dati @cols è NULL ")
    '        sql.AppendLine("IF @cols IS NOT NULL ")
    '        sql.AppendLine("BEGIN ")
    '        sql.AppendLine()
    '        sql.AppendLine("    EXECUTE('SELECT DataOra As Date, ' + @cols_name + ' FROM (SELECT DataOra, Id_Sensore, Valore FROM #DatiMeteo) x PIVOT (MAX(Valore) FOR Id_Sensore IN (' + @cols + ')) p ORDER BY DataOra') ")
    '        sql.AppendLine()
    '        sql.AppendLine("	SELECT col = s1.Etichetta, eti = s2.Etichetta ")
    '        sql.AppendLine("	FROM @TBL_SENSORI s1 ")
    '        sql.AppendLine("	INNER JOIN MeteoNT_Sensori s2 on s2.Id = s1.Id_Src ")
    '        sql.AppendLine()
    '        sql.AppendLine("END ")
    '        sql.AppendLine()
    '        sql.Append(DropTableDati("DatiMeteo"))

    '        EseguiQuery_Lettura(ObjParametri_Server, sql.ToString, NomeRoutine, DS, "DatiMeteo_NT")

    '        If DS.Tables.Count < 2 Then

    '            DS = Nothing
    '        End If

    '    Catch ex As Exception

    '        Throw New Exception("[" & NomeRoutine & "]:" & ex.Message)
    '    End Try

    '    Return DS
    'End Function

#End Region

End Class
